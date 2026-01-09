using Microsoft.EntityFrameworkCore;
using MusicWeb.src.Data;
using MusicWeb.src.Models.Entities;

namespace MusicWeb.Tests.Fixtures;

public static class TestDbContextFactory
{
    public static ApiDbContext Create(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<ApiDbContext>()
            .UseInMemoryDatabase(databaseName: dbName ?? Guid.NewGuid().ToString())
            .Options;

        var context = new ApiDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }

    public static async Task<ApiDbContext> CreateWithSeedAsync(string? dbName = null)
    {
        var context = Create(dbName);
        await SeedDataAsync(context);
        return context;
    }

    private static async Task SeedDataAsync(ApiDbContext context)
    {
        // Seed Artists
        var artists = new[]
        {
            new Artist { Id = 1, Name = "Artist One" },
            new Artist { Id = 2, Name = "Artist Two" },
            new Artist { Id = 3, Name = "Artist Three" }
        };
        context.Artists.AddRange(artists);

        // Seed Genres
        var genres = new[]
        {
            new Genre { Id = 1, Name = "Rock" },
            new Genre { Id = 2, Name = "Pop" },
            new Genre { Id = 3, Name = "Jazz" }
        };
        context.Genres.AddRange(genres);

        // Seed Songs
        var songs = new[]
        {
            new Song { Id = 1, Title = "Song One", Text = "Lyrics one", ArtistId = 1 },
            new Song { Id = 2, Title = "Song Two", Text = "Lyrics two", ArtistId = 1 },
            new Song { Id = 3, Title = "Another Song", Text = "Lyrics three", ArtistId = 2 },
            new Song { Id = 4, Title = "Test Song", Text = "Test lyrics", ArtistId = 3 }
        };
        context.Songs.AddRange(songs);

        // Seed SongGenres
        var songGenres = new[]
        {
            new SongGenre { SongId = 1, GenreId = 1 },
            new SongGenre { SongId = 1, GenreId = 2 },
            new SongGenre { SongId = 2, GenreId = 1 },
            new SongGenre { SongId = 3, GenreId = 2 },
            new SongGenre { SongId = 4, GenreId = 3 }
        };
        context.SongGenres.AddRange(songGenres);

        await context.SaveChangesAsync();
    }
}