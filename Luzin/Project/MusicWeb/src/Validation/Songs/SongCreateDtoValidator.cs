using FluentValidation;
using MusicWeb.src.Models.Dtos.Songs;

namespace MusicWeb.src.Validation.Songs;

public sealed class SongCreateDtoValidator : AbstractValidator<SongCreateDto>
{
    public SongCreateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Text)
            .NotEmpty();

        RuleFor(x => x.ArtistId)
            .GreaterThan(0);
    }
}