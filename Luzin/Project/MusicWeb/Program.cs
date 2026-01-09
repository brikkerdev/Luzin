using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MusicWeb.Services.Auth;
using MusicWeb.Services.Caching;
using MusicWeb.Services.Song.Interfaces;
using MusicWeb.Services.Songs;
using MusicWeb.src.Auth;
using MusicWeb.src.Data;
using MusicWeb.src.Middleware;
using MusicWeb.src.Repositories;
using MusicWeb.src.Repositories.Genres;
using MusicWeb.src.Repositories.Songs;
using MusicWeb.src.Services.Artist;
using MusicWeb.src.Services.Artist.Interfaces;
using MusicWeb.src.Services.Auth;
using MusicWeb.src.Services.Auth.Interfaces;
using MusicWeb.src.Services.Genres;
using MusicWeb.src.Services.Genres.Interfaces;
using MusicWeb.src.Services.Users;
using MusicWeb.src.Services.Users.Interfaces;
using MusicWeb.src.Validation.Artists;
using MusicWeb.src.Validation.Songs;
using Npgsql;
using OpenTelemetry.Metrics;
using StackExchange.Redis;
using System.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddValidatorsFromAssemblyContaining<SongCreateDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<SongUpdateDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ArtistCreateDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ArtistUpdateDtoValidator>();

builder.Services.AddScoped<ISongRepository, SongRepository>();
builder.Services.AddScoped<IArtistRepository, DapperArtistRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();

builder.Services.AddScoped<ISongService, SongService>();
builder.Services.AddScoped<IArtistService, ArtistService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<IRedisCache, RedisCache>();

var connectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("Connection string 'Postgres' not found.");
var redisConnectionString = builder.Configuration.GetValue<string>("Redis:ConnectionString") ?? "localhost:6379";

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
{
    var options = ConfigurationOptions.Parse(redisConnectionString);
    options.AbortOnConnectFail = false;
    return ConnectionMultiplexer.Connect(options);
});

builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "postgresql", tags: new[] { "db", "sql", "postgresql" })
    .AddRedis(redisConnectionString, name: "redis", tags: new[] { "cache", "redis" });

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics.AddMeter(MusicWeb.Observability.SongMetrics.MeterName);
        metrics.AddAspNetCoreInstrumentation();
        metrics.AddPrometheusExporter();
    });

var jwtKey = builder.Configuration["Jwt:Key"] ?? "dev-only-change-me-to-a-long-secret";
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = "ApiKeyOrJwt";
        options.DefaultChallengeScheme = "ApiKeyOrJwt";
    })
    .AddPolicyScheme("ApiKeyOrJwt", "API Key or JWT", options =>
    {
        options.ForwardDefaultSelector = context =>
        {
            if (context.Request.Headers.ContainsKey("X-Api-Key"))
                return ApiKeyOptions.Scheme;

            return "Bearer";
        };
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = !string.IsNullOrWhiteSpace(jwtIssuer),
            ValidIssuer = jwtIssuer,
            ValidateAudience = !string.IsNullOrWhiteSpace(jwtAudience),
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    })
    .AddScheme<ApiKeyOptions, ApiKeyAuthenticationHandler>(ApiKeyOptions.Scheme, o =>
    {
        o.HeaderName = "X-Api-Key";
        o.ConfigurationSectionName = "ApiKeys";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiKeyOnly", policy =>
    {
        policy.AddAuthenticationSchemes(ApiKeyOptions.Scheme);
        policy.RequireAuthenticatedUser();
    });

    options.AddPolicy("JwtOnly", policy =>
    {
        policy.AddAuthenticationSchemes("Bearer");
        policy.RequireAuthenticatedUser();
    });

    options.AddPolicy(AuthorizationPolicies.RequireAdmin, policy =>
        policy.RequireRole(Roles.Admin));

    options.AddPolicy(AuthorizationPolicies.RequireManagerOrAdmin, policy =>
        policy.RequireRole(Roles.Admin, Roles.Manager));

    options.AddPolicy(AuthorizationPolicies.RequireAnyRole, policy =>
        policy.RequireRole(Roles.All));

    options.AddPolicy(AuthorizationPolicies.CanManageContent, policy =>
        policy.RequireRole(Roles.Admin, Roles.Manager));

    options.AddPolicy(AuthorizationPolicies.CanDeleteContent, policy =>
        policy.RequireRole(Roles.Admin));

    options.AddPolicy(AuthorizationPolicies.CanManageUsers, policy =>
        policy.RequireRole(Roles.Admin));
});

builder.Services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(connectionString));

builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "MusicWeb API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        Name = "X-Api-Key",
        In = ParameterLocation.Header,
        Description = "Enter your API key"
    });

    c.OperationFilter<AuthorizeOperationFilter>();
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

app.UseRequestLogging();
app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    await DbSeeder.SeedAdminAsync(db, hasher);
}

app.UseAuthentication();
app.UseAuthorization();

app.MapPrometheusScrapingEndpoint();
app.MapControllers();
app.Run();