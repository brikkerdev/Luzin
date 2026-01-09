using FluentValidation;
using MusicWeb.src.Models.Dtos.Genres;

public sealed class GenreCreateDtoValidator : AbstractValidator<GenreCreateDto>
{
    public GenreCreateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}