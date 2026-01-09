namespace MusicWeb.src.Models.Dtos.Artists
{
    public sealed class ArtistReadDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public int SongCount { get; init; }
    }
}