namespace MusicWeb.src.Models.Dtos.Songs;

public sealed class SongSetGenresDto
{
    public List<int> GenreIds { get; set; } = new();
}