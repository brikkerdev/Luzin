using FluentValidation;
using MusicWeb.src.Models.Dtos.Artists;

namespace MusicWeb.src.Validation.Artists;

public sealed class ArtistCreateDtoValidator : AbstractValidator<ArtistCreateDto>
{
    public ArtistCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}