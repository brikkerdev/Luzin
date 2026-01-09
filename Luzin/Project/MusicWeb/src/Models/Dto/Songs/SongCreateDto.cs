using MusicWeb.src.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace MusicWeb.src.Models.Dtos.Songs;

public sealed class SongCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int ArtistId { get; set; }
}
