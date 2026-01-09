namespace MusicWeb.src.Models.Entities
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;

        public int ArtistId { get; set; }
        public Artist? Artist { get; set; }

        public List<SongGenre> SongGenres { get; set; } = new();
    }
}