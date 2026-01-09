namespace MusicWeb.src.Models.Dtos.Songs;

public sealed class SongReadDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;

    public int ArtistId { get; set; }
    public string ArtistName { get; set; } = string.Empty;

    public List<int> Genres { get; set; }
}