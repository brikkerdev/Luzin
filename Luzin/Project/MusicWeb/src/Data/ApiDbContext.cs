using Microsoft.EntityFrameworkCore;
using MusicWeb.src.Models.Entities;
using MusicWeb.src.Models.Enums;

namespace MusicWeb.src.Data;

public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

    public DbSet<Song> Songs => Set<Song>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Artist> Artists => Set<Artist>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<SongGenre> SongGenres => Set<SongGenre>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Artist>(e =>
        {
            e.ToTable("artists");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Name).HasColumnName("name").HasMaxLength(200).IsRequired();

            e.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<Song>(e =>
        {
            e.ToTable("songs");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
            e.Property(x => x.Text).HasColumnName("text").IsRequired();

            e.Property(x => x.ArtistId).HasColumnName("artist_id").IsRequired();

            e.HasOne(x => x.Artist)
                .WithMany(a => a.Songs)
                .HasForeignKey(x => x.ArtistId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => x.ArtistId).HasDatabaseName("ix_songs_artist_id");
            e.HasIndex(x => x.Title).HasDatabaseName("ix_songs_title");
        });

        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id)
                .HasColumnName("id");

            e.Property(x => x.Username)
                .HasColumnName("username")
                .HasMaxLength(100)
                .IsRequired();

            e.Property(x => x.PasswordHash)
                .HasColumnName("password_hash")
                .IsRequired();

            e.Property(x => x.Role)
                .HasColumnName("role")
                .HasDefaultValue(Role.User)
                .HasConversion<int>()
                .IsRequired();

            e.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            e.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at");

            e.HasIndex(x => x.Username).IsUnique();
            e.HasIndex(x => x.Role).HasDatabaseName("ix_users_role");
        });

        modelBuilder.Entity<Genre>(e =>
        {
            e.ToTable("genres");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();

            e.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<SongGenre>(e =>
        {
            e.ToTable("song_genres");
            e.HasKey(x => new { x.SongId, x.GenreId });

            e.Property(x => x.SongId).HasColumnName("song_id");
            e.Property(x => x.GenreId).HasColumnName("genre_id");

            e.HasOne(x => x.Song)
                .WithMany(s => s.SongGenres)
                .HasForeignKey(x => x.SongId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Genre)
                .WithMany(g => g.SongGenres)
                .HasForeignKey(x => x.GenreId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}