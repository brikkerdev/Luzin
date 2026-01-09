using MusicWeb.src.Models.Dtos.Common;

namespace MusicWeb.src.Models.Dtos.Songs;

public class SongFilterQuery : PaginationQuery
{
    public string? Search { get; set; }
    public int? ArtistId { get; set; }
    public int? GenreId { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }

    public bool IsDescending =>
        string.Equals(SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
}