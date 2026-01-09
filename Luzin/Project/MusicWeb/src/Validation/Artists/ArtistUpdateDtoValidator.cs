using FluentValidation;
using MusicWeb.src.Models.Dtos.Artists;

namespace MusicWeb.src.Validation.Artists;

public sealed class ArtistUpdateDtoValidator : AbstractValidator<ArtistUpdateDto>
{
    public ArtistUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}