using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicWeb.src.Auth;
using MusicWeb.src.Exceptions;
using MusicWeb.src.Models.Dtos.Artists;
using MusicWeb.src.Services.Artist.Interfaces;

namespace MusicWeb.src.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class ArtistsController : ControllerBase
{
    private readonly IArtistService _artists;

    public ArtistsController(IArtistService artists) => _artists = artists;

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.RequireAnyRole)]
    [ProducesResponseType(typeof(List<ArtistReadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<ArtistReadDto>>> GetAll(CancellationToken ct)
        => Ok(await _artists.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.RequireAnyRole)]
    [ProducesResponseType(typeof(ArtistReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ArtistReadDto>> GetById(int id, CancellationToken ct)
        => Ok(await _artists.GetByIdAsync(id, ct));

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.CanManageContent)]
    [ProducesResponseType(typeof(ArtistReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ArtistReadDto>> Create([FromBody] ArtistCreateDto dto, CancellationToken ct)
    {
        var created = await _artists.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPost("with-songs")]
    [Authorize(Policy = AuthorizationPolicies.CanManageContent)]
    [ProducesResponseType(typeof(ArtistReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ArtistReadDto>> CreateWithSongs([FromBody] ArtistWithSongsCreateDto dto, CancellationToken ct)
    {
        var created = await _artists.CreateWithSongsAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.CanManageContent)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(int id, [FromBody] ArtistUpdateDto dto, CancellationToken ct)
    {
        await _artists.UpdateAsync(id, dto, ct);
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
        await _artists.DeleteAsync(id, ct);
        return NoContent();
    }
}