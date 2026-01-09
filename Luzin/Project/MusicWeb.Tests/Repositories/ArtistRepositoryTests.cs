using System.Data;
using Dapper;
using FluentAssertions;
using MusicWeb.src.Models.Entities;
using MusicWeb.src.Repositories;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace MusicWeb.Tests.Repositories;

public class ArtistRepositoryTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .Build();

    private IDbConnection _connection = null!;
    private DapperArtistRepository _sut = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        _connection = new NpgsqlConnection(_postgres.GetConnectionString());
        await _connection.OpenAsync();

        await _connection.ExecuteAsync("""
            CREATE TABLE artists (
                id SERIAL PRIMARY KEY,
                name VARCHAR(200) NOT NULL
            );
            
            CREATE TABLE songs (
                id SERIAL PRIMARY KEY,
                title VARCHAR(200) NOT NULL,
                text TEXT NOT NULL,
                artist_id INTEGER NOT NULL REFERENCES artists(id)
            );
        """);

        await SeedDataAsync();
        _sut = new DapperArtistRepository(_connection);
    }

    public async Task DisposeAsync()
    {
        _connection.Dispose();
        await _postgres.DisposeAsync();
    }

    private async Task SeedDataAsync()
    {
        await _connection.ExecuteAsync("""
            INSERT INTO artists (name) VALUES ('Artist One');
            INSERT INTO artists (name) VALUES ('Artist Two');
            INSERT INTO artists (name) VALUES ('Artist Three');
            
            INSERT INTO songs (title, text, artist_id) VALUES ('Song 1', 'Text 1', 1);
            INSERT INTO songs (title, text, artist_id) VALUES ('Song 2', 'Text 2', 1);
            INSERT INTO songs (title, text, artist_id) VALUES ('Song 3', 'Text 3', 2);
        """);
    }

    [Fact]
    public async Task GetAllAsync_WhenArtistsExist_ReturnsAllArtists()
    {
        var result = await _sut.GetAllAsync(CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsArtistsOrderedById()
    {
        var result = await _sut.GetAllAsync(CancellationToken.None);

        result.Select(a => a.Id).Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task GetByIdAsync_WhenArtistExists_ReturnsArtist()
    {
        var result = await _sut.GetByIdAsync(1, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Artist One");
    }

    [Fact]
    public async Task GetByIdAsync_WhenArtistDoesNotExist_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync(999, CancellationToken.None);

        result.Should().BeNull();
    }

    [Theory]
    [InlineData(1, "Artist One")]
    [InlineData(2, "Artist Two")]
    [InlineData(3, "Artist Three")]
    public async Task GetByIdAsync_ReturnsCorrectArtist(int id, string expectedName)
    {
        var result = await _sut.GetByIdAsync(id, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Name.Should().Be(expectedName);
    }

    [Fact]
    public async Task AddAsync_WhenValidArtist_AddsToDatabase()
    {
        var newArtist = new Artist { Name = "New Artist" };

        await _sut.AddAsync(newArtist, CancellationToken.None);

        var allArtists = await _sut.GetAllAsync(CancellationToken.None);
        allArtists.Should().HaveCount(4);
        allArtists.Should().Contain(a => a.Name == "New Artist");
    }

    [Fact]
    public async Task AddAsync_AssignsIdToNewArtist()
    {
        var newArtist = new Artist { Name = "New Artist" };

        await _sut.AddAsync(newArtist, CancellationToken.None);

        newArtist.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task UpdateNameAsync_WhenArtistExists_UpdatesName()
    {
        const int artistId = 1;
        const string newName = "Updated Artist Name";

        var result = await _sut.UpdateNameAsync(artistId, newName, CancellationToken.None);

        result.Should().BeTrue();
        var updatedArtist = await _sut.GetByIdAsync(artistId, CancellationToken.None);
        updatedArtist!.Name.Should().Be(newName);
    }

    [Fact]
    public async Task UpdateNameAsync_WhenArtistDoesNotExist_ReturnsFalse()
    {
        var result = await _sut.UpdateNameAsync(999, "New Name", CancellationToken.None);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_WhenArtistExists_RemovesArtist()
    {
        await _connection.ExecuteAsync("INSERT INTO artists (name) VALUES ('Artist To Delete')");
        var artistId = await _connection.ExecuteScalarAsync<int>("SELECT MAX(id) FROM artists");

        var result = await _sut.DeleteAsync(artistId, CancellationToken.None);

        result.Should().BeTrue();
        var deletedArtist = await _sut.GetByIdAsync(artistId, CancellationToken.None);
        deletedArtist.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenArtistDoesNotExist_ReturnsFalse()
    {
        var result = await _sut.DeleteAsync(999, CancellationToken.None);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task CreateArtistWithSongsAsync_CreatesArtistAndSongs()
    {
        var artist = new Artist { Name = "Transaction Artist" };
        var songs = new List<Song>
        {
            new() { Title = "Trans Song 1", Text = "Lyrics 1" },
            new() { Title = "Trans Song 2", Text = "Lyrics 2" }
        };

        var result = await _sut.CreateArtistWithSongsAsync(artist, songs, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Songs.Should().HaveCount(2);
        result.Songs.Should().AllSatisfy(s =>
        {
            s.Id.Should().BeGreaterThan(0);
            s.ArtistId.Should().Be(result.Id);
        });
    }

    [Fact]
    public async Task CreateArtistWithSongsAsync_WithEmptySongs_CreatesOnlyArtist()
    {
        var artist = new Artist { Name = "Solo Artist" };
        var songs = new List<Song>();

        var result = await _sut.CreateArtistWithSongsAsync(artist, songs, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Songs.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateArtistWithSongsAsync_AllSongsHaveCorrectArtistId()
    {
        var artist = new Artist { Name = "Artist With Songs" };
        var songs = new List<Song>
        {
            new() { Title = "Song A", Text = "Text A" },
            new() { Title = "Song B", Text = "Text B" },
            new() { Title = "Song C", Text = "Text C" }
        };

        var result = await _sut.CreateArtistWithSongsAsync(artist, songs, CancellationToken.None);

        result.Songs.Should().HaveCount(3);
        result.Songs.Should().OnlyContain(s => s.ArtistId == result.Id);
    }

    [Fact]
    public async Task GetAllWithSongCountAsync_ReturnsCorrectSongCounts()
    {
        var result = await _sut.GetAllWithSongCountAsync(CancellationToken.None);

        result.Should().HaveCount(3);

        var artistOne = result.First(a => a.Name == "Artist One");
        artistOne.SongCount.Should().Be(2);

        var artistTwo = result.First(a => a.Name == "Artist Two");
        artistTwo.SongCount.Should().Be(1);

        var artistThree = result.First(a => a.Name == "Artist Three");
        artistThree.SongCount.Should().Be(0);
    }

    [Fact]
    public async Task GetByIdWithSongCountAsync_ReturnsCorrectSongCount()
    {
        var result = await _sut.GetByIdWithSongCountAsync(1, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Artist One");
        result.SongCount.Should().Be(2);
    }

    [Fact]
    public async Task GetByIdWithSongCountAsync_WhenNoSongs_ReturnsZeroCount()
    {
        var result = await _sut.GetByIdWithSongCountAsync(3, CancellationToken.None);

        result.Should().NotBeNull();
        result!.SongCount.Should().Be(0);
    }
}