using System.Data;
using Dapper;
using MusicWeb.src.Models.Dtos.Artists;
using MusicWeb.src.Models.Entities;

namespace MusicWeb.src.Repositories;

public sealed class DapperArtistRepository : IArtistRepository
{
    private readonly IDbConnection _conn;

    public DapperArtistRepository(IDbConnection conn)
    {
        _conn = conn;
    }

    public async Task<List<Artist>> GetAllAsync(CancellationToken ct)
    {
        const string sql = """
            SELECT id AS "Id", name AS "Name"
            FROM artists
            ORDER BY id;
        """;

        var rows = await _conn.QueryAsync<Artist>(
            new CommandDefinition(sql, cancellationToken: ct));

        return rows.AsList();
    }

    public async Task<Artist?> GetByIdAsync(int id, CancellationToken ct)
    {
        const string sql = """
            SELECT id AS "Id", name AS "Name"
            FROM artists
            WHERE id = @id;
        """;

        return await _conn.QueryFirstOrDefaultAsync<Artist>(
            new CommandDefinition(sql, new { id }, cancellationToken: ct));
    }

    public async Task<List<ArtistReadDto>> GetAllWithSongCountAsync(CancellationToken ct)
    {
        const string sql = """
            SELECT
                a.id   AS "Id",
                a.name AS "Name",
                COALESCE(COUNT(s.id), 0)::int AS "SongCount"
            FROM artists a
            LEFT JOIN songs s ON s.artist_id = a.id
            GROUP BY a.id, a.name
            ORDER BY a.id;
        """;

        var rows = await _conn.QueryAsync<ArtistReadDto>(
            new CommandDefinition(sql, cancellationToken: ct));

        return rows.AsList();
    }

    public async Task<ArtistReadDto?> GetByIdWithSongCountAsync(int id, CancellationToken ct)
    {
        const string sql = """
            SELECT
                a.id   AS "Id",
                a.name AS "Name",
                COALESCE(COUNT(s.id), 0)::int AS "SongCount"
            FROM artists a
            LEFT JOIN songs s ON s.artist_id = a.id
            WHERE a.id = @id
            GROUP BY a.id, a.name;
        """;

        return await _conn.QueryFirstOrDefaultAsync<ArtistReadDto>(
            new CommandDefinition(sql, new { id }, cancellationToken: ct));
    }

    public async Task AddAsync(Artist entity, CancellationToken ct)
    {
        const string sql = """
            INSERT INTO artists (name)
            VALUES (@Name)
            RETURNING id;
        """;

        var newId = await _conn.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { entity.Name }, cancellationToken: ct));

        entity.Id = newId;
    }

    public async Task<bool> UpdateNameAsync(int id, string name, CancellationToken ct)
    {
        const string sql = """
            UPDATE artists
            SET name = @name
            WHERE id = @id;
        """;

        var affected = await _conn.ExecuteAsync(
            new CommandDefinition(sql, new { id, name }, cancellationToken: ct));

        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        const string sql = """
            DELETE FROM artists
            WHERE id = @id;
        """;

        var affected = await _conn.ExecuteAsync(
            new CommandDefinition(sql, new { id }, cancellationToken: ct));

        return affected > 0;
    }

    public async Task<Artist> CreateArtistWithSongsAsync(Artist artist, List<Song> songs, CancellationToken ct)
    {
        if (_conn.State != ConnectionState.Open)
            _conn.Open();

        using var transaction = _conn.BeginTransaction();

        try
        {
            const string insertArtistSql = """
                INSERT INTO artists (name)
                VALUES (@Name)
                RETURNING id;
            """;

            var artistId = await _conn.ExecuteScalarAsync<int>(
                new CommandDefinition(insertArtistSql, new { artist.Name }, transaction, cancellationToken: ct));

            artist.Id = artistId;

            if (songs.Count > 0)
            {
                const string insertSongSql = """
                    INSERT INTO songs (title, text, artist_id)
                    VALUES (@Title, @Text, @ArtistId)
                    RETURNING id;
                """;

                foreach (var song in songs)
                {
                    song.ArtistId = artistId;
                    var songId = await _conn.ExecuteScalarAsync<int>(
                        new CommandDefinition(insertSongSql,
                            new { song.Title, song.Text, song.ArtistId },
                            transaction,
                            cancellationToken: ct));
                    song.Id = songId;
                }
            }

            transaction.Commit();
            artist.Songs = songs;
            return artist;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public Task<int> SaveChangesAsync(CancellationToken ct) => Task.FromResult(0);
}