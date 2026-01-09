using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicWeb.Services.Song.Interfaces;
using MusicWeb.src.Auth;
using MusicWeb.src.Exceptions;
using MusicWeb.src.Models.Dtos.Common;
using MusicWeb.src.Models.Dtos.Songs;

namespace MusicWeb.src.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SongsController : ControllerBase
{
    private readonly ISongService _songs;

    public SongsController(ISongService songs) => _songs = songs;

    [HttpGet("all")]
    [Authorize(Policy = AuthorizationPolicies.RequireAnyRole)]
    [ProducesResponseType(typeof(List<SongReadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<SongReadDto>>> GetAll(CancellationToken ct)
        => Ok(await _songs.GetAllAsync(ct));

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.RequireAnyRole)]
    [ProducesResponseType(typeof(PagedResult<SongReadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<SongReadDto>>> GetPaged([FromQuery] SongFilterQuery query, CancellationToken ct)
        => Ok(await _songs.GetPagedAsync(query, ct));

    [HttpGet("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.RequireAnyRole)]
    [ProducesResponseType(typeof(SongReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SongReadDto>> GetById(int id, CancellationToken ct)
        => Ok(await _songs.GetByIdAsync(id, ct));

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.CanManageContent)]
    [ProducesResponseType(typeof(SongReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SongReadDto>> Create([FromBody] SongCreateDto dto, CancellationToken ct)
    {
        var created = await _songs.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}/genres")]
    [Authorize(Policy = AuthorizationPolicies.CanManageContent)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SetGenres(int id, [FromBody] SongSetGenresDto dto, CancellationToken ct)
    {
        await _songs.SetGenresAsync(id, dto.GenreIds, ct);
        return NoContent();
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.CanManageContent)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(int id, [FromBody] SongUpdateDto dto, CancellationToken ct)
    {
        await _songs.UpdateAsync(id, dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.CanManageContent)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _songs.DeleteAsync(id, ct);
        return NoContent();
    }
}