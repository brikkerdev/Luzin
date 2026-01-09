using FluentAssertions;
using MusicWeb.src.Models.Dtos.Songs;
using MusicWeb.src.Models.Entities;
using MusicWeb.src.Repositories.Songs;
using MusicWeb.Tests.Fixtures;
using Xunit;

namespace MusicWeb.src.MusicWeb.Tests.Repositories;

public class SongRepositoryTests : RepositoryTestBase, IAsyncLifetime
{
    private SongRepository _sut = null!;

    public async Task InitializeAsync()
    {
        await InitializeAsync(withSeed: true);
        _sut = new SongRepository(Context);
    }

    public new async Task DisposeAsync() => await base.DisposeAsync();

    [Fact]
    public async Task GetAllAsync_WhenSongsExist_ReturnsAllSongs()
    {
        // Act
        var result = await _sut.GetAllAsync(CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(4);
    }

    [Fact]
    public async Task GetAllAsync_IncludesArtistNavigation()
    {
        // Act
        var result = await _sut.GetAllAsync(CancellationToken);

        // Assert
        result.Should().AllSatisfy(s => s.Artist.Should().NotBeNull());
    }

    [Fact]
    public async Task GetAllAsync_IncludesSongGenres()
    {
        // Act
        var result = await _sut.GetAllAsync(CancellationToken);

        // Assert
        var songWithGenres = result.First(s => s.Id == 1);
        songWithGenres.SongGenres.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsCorrectPageSize()
    {
        // Arrange
        var query = new SongFilterQuery { Page = 1, PageSize = 2 };

        // Act
        var (items, total) = await _sut.GetPagedAsync(query, CancellationToken);

        // Assert
        items.Should().HaveCount(2);
        total.Should().Be(4);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsCorrectPage()
    {
        // Arrange
        var query = new SongFilterQuery { Page = 2, PageSize = 2 };

        // Act
        var (items, total) = await _sut.GetPagedAsync(query, CancellationToken);

        // Assert
        items.Should().HaveCount(2);
        items.First().Id.Should().Be(3); // Third song (second page)
    }

    [Fact]
    public async Task GetPagedAsync_WithSearchFilter_ReturnsMatchingSongs()
    {
        // Arrange
        var query = new SongFilterQuery { Search = "Song One", Page = 1, PageSize = 10 };

        // Act
        var (items, total) = await _sut.GetPagedAsync(query, CancellationToken);

        // Assert
        items.Should().HaveCount(1);
        items.First().Title.Should().Contain("Song One");
    }

    [Fact]
    public async Task GetPagedAsync_WithArtistIdFilter_ReturnsArtistSongs()
    {
        // Arrange
        var query = new SongFilterQuery { ArtistId = 1, Page = 1, PageSize = 10 };

        // Act
        var (items, total) = await _sut.GetPagedAsync(query, CancellationToken);

        // Assert
        items.Should().HaveCount(2);
        items.Should().AllSatisfy(s => s.ArtistId.Should().Be(1));
    }

    [Fact]
    public async Task GetPagedAsync_WithGenreIdFilter_ReturnsSongsWithGenre()
    {
        // Arrange
        var query = new SongFilterQuery { GenreId = 1, Page = 1, PageSize = 10 };

        // Act
        var (items, total) = await _sut.GetPagedAsync(query, CancellationToken);

        // Assert
        items.Should().HaveCount(2); // Songs 1 and 2 have genre 1 (Rock)
    }

    [Theory]
    [InlineData("title", false)]
    [InlineData("title", true)]
    [InlineData("artist", false)]
    [InlineData("artist", true)]
    [InlineData("id", false)]
    [InlineData("id", true)]
    public async Task GetPagedAsync_WithSorting_ReturnsSortedResults(string sortBy, bool isDescending)
    {
        // Arrange
        var query = new SongFilterQuery
        {
            SortBy = sortBy,
            SortDirection = isDescending ? "desc" : "asc",
            Page = 1,
            PageSize = 10
        };

        // Act
        var (items, _) = await _sut.GetPagedAsync(query, CancellationToken);

        // Assert
        items.Should().NotBeEmpty();

        if (sortBy == "title")
        {
            if (isDescending)
                items.Select(s => s.Title).Should().BeInDescendingOrder();
            else
                items.Select(s => s.Title).Should().BeInAscendingOrder();
        }
    }

    [Fact]
    public async Task GetPagedAsync_WithMultipleFilters_ReturnsFilteredResults()
    {
        // Arrange
        var query = new SongFilterQuery
        {
            ArtistId = 1,
            GenreId = 1,
            Page = 1,
            PageSize = 10
        };

        // Act
        var (items, total) = await _sut.GetPagedAsync(query, CancellationToken);

        // Assert
        items.Should().HaveCount(2); // Songs 1 and 2 belong to artist 1 and have genre 1
    }

    [Fact]
    public async Task GetPagedAsync_SearchByArtistName_ReturnsMatchingSongs()
    {
        // Arrange
        var query = new SongFilterQuery { Search = "Artist One", Page = 1, PageSize = 10 };

        // Act
        var (items, _) = await _sut.GetPagedAsync(query, CancellationToken);

        // Assert
        items.Should().HaveCount(2); // Artist One has 2 songs
    }

    [Fact]
    public async Task GetPagedAsync_EmptySearch_ReturnsAllSongs()
    {
        // Arrange
        var query = new SongFilterQuery { Search = "", Page = 1, PageSize = 10 };

        // Act
        var (items, total) = await _sut.GetPagedAsync(query, CancellationToken);

        // Assert
        total.Should().Be(4);
    }

    [Fact]
    public async Task GetPagedAsync_LastPage_ReturnsRemainingItems()
    {
        // Arrange
        var query = new SongFilterQuery { Page = 2, PageSize = 3 };

        // Act
        var (items, total) = await _sut.GetPagedAsync(query, CancellationToken);

        // Assert
        items.Should().HaveCount(1); // 4 total, page 2 with size 3 = 1 item
        total.Should().Be(4);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSongExists_ReturnsSongWithRelations()
    {
        // Act
        var result = await _sut.GetByIdAsync(1, CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Artist.Should().NotBeNull();
        result.SongGenres.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_WhenSongDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _sut.GetByIdAsync(999, CancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_IncludesGenresViaJoinTable()
    {
        // Act
        var result = await _sut.GetByIdAsync(1, CancellationToken);

        // Assert
        result!.SongGenres.Should().HaveCount(2);
        result.SongGenres.Select(sg => sg.Genre!.Name)
            .Should().Contain(new[] { "Rock", "Pop" });
    }

    [Fact]
    public async Task AddAsync_WhenValidSong_AddsSongToDatabase()
    {
        // Arrange
        var newSong = new Song
        {
            Title = "New Song",
            Text = "New lyrics",
            ArtistId = 1
        };

        // Act
        await _sut.AddAsync(newSong, CancellationToken);
        await _sut.SaveChangesAsync(CancellationToken);

        // Assert
        var songs = await _sut.GetAllAsync(CancellationToken);
        songs.Should().HaveCount(5);
        songs.Should().Contain(s => s.Title == "New Song");
    }

    [Fact]
    public async Task AddAsync_GeneratesId()
    {
        // Arrange
        var newSong = new Song
        {
            Title = "New Song",
            Text = "New lyrics",
            ArtistId = 1
        };

        // Act
        await _sut.AddAsync(newSong, CancellationToken);
        await _sut.SaveChangesAsync(CancellationToken);

        // Assert
        newSong.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task UpdateAsync_WhenSongExists_UpdatesSuccessfully()
    {
        // Arrange
        const int songId = 1;
        const string newTitle = "Updated Title";

        // Act
        var result = await _sut.UpdateAsync(songId, song => song.Title = newTitle, CancellationToken);
        await _sut.SaveChangesAsync(CancellationToken);

        // Assert
        result.Should().BeTrue();

        var updated = await _sut.GetByIdAsync(songId, CancellationToken);
        updated!.Title.Should().Be(newTitle);
    }

    [Fact]
    public async Task UpdateAsync_WhenSongDoesNotExist_ReturnsFalse()
    {
        // Act
        var result = await _sut.UpdateAsync(999, song => song.Title = "New", CancellationToken);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateAsync_CanUpdateMultipleProperties()
    {
        // Arrange
        const int songId = 1;

        // Act
        var result = await _sut.UpdateAsync(songId, song =>
        {
            song.Title = "New Title";
            song.Text = "New Text";
            song.ArtistId = 2;
        }, CancellationToken);
        await _sut.SaveChangesAsync(CancellationToken);

        // Assert
        result.Should().BeTrue();

        var updated = await _sut.GetByIdAsync(songId, CancellationToken);
        updated!.Title.Should().Be("New Title");
        updated.Text.Should().Be("New Text");
        updated.ArtistId.Should().Be(2);
    }

    [Fact]
    public async Task DeleteAsync_WhenSongExists_DeletesSuccessfully()
    {
        // Arrange
        const int songId = 1;

        // Act
        var result = await _sut.DeleteAsync(songId, CancellationToken);
        await _sut.SaveChangesAsync(CancellationToken);

        // Assert
        result.Should().BeTrue();

        var deleted = await _sut.GetByIdAsync(songId, CancellationToken);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenSongDoesNotExist_ReturnsFalse()
    {
        // Act
        var result = await _sut.DeleteAsync(999, CancellationToken);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_DecreasesSongCount()
    {
        // Arrange
        var initialCount = (await _sut.GetAllAsync(CancellationToken)).Count;

        // Act
        await _sut.DeleteAsync(1, CancellationToken);
        await _sut.SaveChangesAsync(CancellationToken);

        // Assert
        var finalCount = (await _sut.GetAllAsync(CancellationToken)).Count;
        finalCount.Should().Be(initialCount - 1);
    }
}