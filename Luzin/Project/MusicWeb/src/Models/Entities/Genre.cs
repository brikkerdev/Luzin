namespace MusicWeb.src.Models.Entities
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public List<SongGenre> SongGenres { get; set; } = new();
    }
}