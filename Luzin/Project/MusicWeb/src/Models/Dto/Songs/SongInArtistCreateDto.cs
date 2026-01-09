namespace MusicWeb.src.Models.Dtos.Artists;

public sealed class SongInArtistCreateDto
{
    public string Title { get; init; } = string.Empty;
    public string Text { get; init; } = string.Empty;
}