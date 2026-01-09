using System.ComponentModel.DataAnnotations;

namespace MusicWeb.src.Models.Dtos.Artists
{
    public sealed class ArtistCreateDto
    {
        public string Name { get; init; } = string.Empty;
    }
}