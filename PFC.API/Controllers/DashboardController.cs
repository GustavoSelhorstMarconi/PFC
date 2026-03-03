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
    public async Task<IActionResult> GetSummary([FromQuery] DateOnly? date, CancellationToken cancellationToken)
    {
        var summary = await _dashboardService.GetDashboardSummary(date, cancellationToken);
        return summary.ToOkActionResult();
    }

    [HttpGet("income-expense-history")]
    public async Task<IActionResult> GetIncomeExpenseHistory(CancellationToken cancellationToken)
    {
        var history = await _dashboardService.GetMonthlyIncomeExpenseHistory(cancellationToken);
        return history.ToOkActionResult();
    }
}
