using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PFC.API.Extensions;
using PFC.Application.Interfaces;

namespace PFC.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] int month, [FromQuery] int year, CancellationToken cancellationToken)
    {
        if (month < 1 || month > 12)
            return BadRequest(new { error = "Invalid month" });

        if (year < 2000)
            return BadRequest(new { error = "Invalid year" });

        var summary = await _dashboardService.GetMonthlySummaryAsync(month, year, cancellationToken);
        return summary.ToOkActionResult();
    }
}
