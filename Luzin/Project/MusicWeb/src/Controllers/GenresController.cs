using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicWeb.src.Auth;
using MusicWeb.src.Exceptions;
using MusicWeb.src.Models.Dtos.Genres;
using MusicWeb.src.Services.Genres.Interfaces;

namespace MusicWeb.src.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class GenresController : ControllerBase
{
    private readonly IGenreService _genres;

    public GenresController(IGenreService genres) => _genres = genres;

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.RequireAnyRole)]
    [ProducesResponseType(typeof(List<GenreReadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<GenreReadDto>>> GetAll(CancellationToken ct)
        => Ok(await _genres.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.RequireAnyRole)]
    [ProducesResponseType(typeof(GenreReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GenreReadDto>> GetById(int id, CancellationToken ct)
        => Ok(await _genres.GetByIdAsync(id, ct));

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.CanManageContent)]
    [ProducesResponseType(typeof(GenreReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<GenreReadDto>> Create([FromBody] GenreCreateDto dto, CancellationToken ct)
    {
        var created = await _genres.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.CanManageContent)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(int id, [FromBody] GenreUpdateDto dto, CancellationToken ct)
    {
        await _genres.UpdateAsync(id, dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.CanDeleteContent)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _genres.DeleteAsync(id, ct);
        return NoContent();
    }
}