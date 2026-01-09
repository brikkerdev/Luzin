using System.Text.Json;
using StackExchange.Redis;

namespace MusicWeb.Services.Caching;

public sealed class RedisCache : IRedisCache
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IDatabase _db;

    public RedisCache(IConnectionMultiplexer mux)
    {
        _db = mux.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct)
    {
        var value = await _db.StringGetAsync(key);
        if (!value.HasValue) return default;

        try
        {
            return JsonSerializer.Deserialize<T>(value!, JsonOptions);
        }
        catch
        {
            return default;
        }
    }

    public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(value, JsonOptions);
        return _db.StringSetAsync(key, json, ttl);
    }

    public Task RemoveAsync(string key, CancellationToken ct)
        => _db.KeyDeleteAsync(key);
}