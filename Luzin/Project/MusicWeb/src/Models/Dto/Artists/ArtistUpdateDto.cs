using System.ComponentModel.DataAnnotations;
using MusicWeb.src.Models.Entities;

namespace MusicWeb.src.Models.Dtos.Artists
{
    public sealed class ArtistUpdateDto
    {
        public string Name { get; init; } = string.Empty;

        public void Apply(Artist entity)
        {
            entity.Name = Name;
        }
    }
}