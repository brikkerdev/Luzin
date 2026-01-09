using MusicWeb.src.Data;

namespace MusicWeb.Tests.Fixtures;

public abstract class RepositoryTestBase : IAsyncDisposable
{
    protected ApiDbContext Context { get; private set; } = null!;
    protected CancellationToken CancellationToken => CancellationToken.None;

    protected async Task InitializeAsync(bool withSeed = true)
    {
        Context = withSeed
            ? await TestDbContextFactory.CreateWithSeedAsync()
            : TestDbContextFactory.Create();
    }

    public async ValueTask DisposeAsync()
    {
        if (Context != null)
        {
            await Context.DisposeAsync();
        }
    }
}