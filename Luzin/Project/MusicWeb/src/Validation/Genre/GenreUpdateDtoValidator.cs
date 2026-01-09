using FluentValidation;
using MusicWeb.src.Models.Dtos.Genres;

public sealed class GenreUpdateDtoValidator : AbstractValidator<GenreUpdateDto>
{
    public GenreUpdateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}