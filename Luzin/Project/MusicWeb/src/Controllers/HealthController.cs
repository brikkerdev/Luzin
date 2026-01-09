using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MusicWeb.src.Controllers;

[ApiController]
[Route("api")]
[Produces("application/json")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;

    public HealthController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    [HttpGet("health")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Health(CancellationToken ct)
    {
        var report = await _healthCheckService.CheckHealthAsync(ct);

        return report.Status == HealthStatus.Healthy
            ? Ok("ok")
            : StatusCode(StatusCodes.Status503ServiceUnavailable, "unhealthy");
    }

    [HttpGet("health/live")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult Live() => Ok("ok");

    [HttpGet("health/ready")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Ready(CancellationToken ct)
    {
        var report = await _healthCheckService.CheckHealthAsync(ct);

        return report.Status == HealthStatus.Healthy
            ? Ok("ok")
            : StatusCode(StatusCodes.Status503ServiceUnavailable, "not ready");
    }
}