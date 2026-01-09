namespace MusicWeb.src.Models.Dtos.Artists;

public sealed class ArtistWithSongsCreateDto
{
    public string Name { get; init; } = string.Empty;
    public List<SongInArtistCreateDto> Songs { get; init; } = new();
}
