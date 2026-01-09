using FluentAssertions;
using MusicWeb.src.Models.Entities;
using MusicWeb.src.Repositories.Genres;
using MusicWeb.Tests.Fixtures;
using Xunit;

namespace MusicWeb.src.MusicWeb.Tests.Repositories;

public class GenreRepositoryTests : RepositoryTestBase, IAsyncLifetime
{
    private GenreRepository _sut = null!;

    public async Task InitializeAsync()
    {
        await InitializeAsync(withSeed: true);
        _sut = new GenreRepository(Context);
    }

    public new async Task DisposeAsync() => await base.DisposeAsync();

    [Fact]
    public async Task GetAllAsync_WhenGenresExist_ReturnsAllGenres()
    {
        // Act
        var result = await _sut.GetAllAsync(CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsGenresOrderedById()
    {
        // Act
        var result = await _sut.GetAllAsync(CancellationToken);

        // Assert
        result.Select(g => g.Id).Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ReturnsEmptyList()
    {
        // Arrange
        await InitializeAsync(withSeed: false);
        _sut = new GenreRepository(Context);

        // Act
        var result = await _sut.GetAllAsync(CancellationToken);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_WhenGenreExists_ReturnsGenre()
    {
        // Act
        var result = await _sut.GetByIdAsync(1, CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Rock");
    }

    [Fact]
    public async Task GetByIdAsync_WhenGenreDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _sut.GetByIdAsync(999, CancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    public async Task GetByIdAsync_WithInvalidIds_ReturnsNull(int invalidId)
    {
        // Act
        var result = await _sut.GetByIdAsync(invalidId, CancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenValidGenre_AddsToDatabase()
    {
        // Arrange
        var newGenre = new Genre { Name = "Electronic" };

        // Act
        await _sut.AddAsync(newGenre, CancellationToken);
        await _sut.SaveChangesAsync(CancellationToken);

        // Assert
        var allGenres = await _sut.GetAllAsync(CancellationToken);
        allGenres.Should().HaveCount(4);
        allGenres.Should().Contain(g => g.Name == "Electronic");
    }

    [Fact]
    public async Task AddAsync_GeneratesNewId()
    {
        // Arrange
        var newGenre = new Genre { Name = "Electronic" };

        // Act
        await _sut.AddAsync(newGenre, CancellationToken);
        await _sut.SaveChangesAsync(CancellationToken);

        // Assert
        newGenre.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task UpdateNameAsync_WhenGenreExists_UpdatesSuccessfully()
    {
        // Arrange
        const int genreId = 1;
        const string newName = "Hard Rock";

        // Act
        var result = await _sut.UpdateNameAsync(genreId, newName, CancellationToken);
        await _sut.SaveChangesAsync(CancellationToken);

        // Assert
        result.Should().BeTrue();

        var updated = await _sut.GetByIdAsync(genreId, CancellationToken);
        updated!.Name.Should().Be(newName);
    }

    [Fact]
    public async Task UpdateNameAsync_WhenGenreDoesNotExist_ReturnsFalse()
    {
        // Act
        var result = await _sut.UpdateNameAsync(999, "New Name", CancellationToken);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_WhenGenreExists_DeletesSuccessfully()
    {
        // Arrange
        const int genreId = 3;

        // Act
        var result = await _sut.DeleteAsync(genreId, CancellationToken);
        await _sut.SaveChangesAsync(CancellationToken);

        // Assert
        result.Should().BeTrue();

        var deleted = await _sut.GetByIdAsync(genreId, CancellationToken);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenGenreDoesNotExist_ReturnsFalse()
    {
        // Act
        var result = await _sut.DeleteAsync(999, CancellationToken);

        // Assert
        result.Should().BeFalse();
    }
}