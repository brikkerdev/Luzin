namespace MusicWeb.src.Models.Dtos.Genres;

public sealed class GenreReadDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int SongCount { get; set; }
}