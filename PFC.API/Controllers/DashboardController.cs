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
    public async Task<IActionResult> GetSummary([FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate, CancellationToken cancellationToken)
    {
        var summary = await _dashboardService.GetDashboardSummary(fromDate, toDate, cancellationToken);
        return summary.ToOkActionResult();
    }

    [HttpGet("income-expense-history")]
    public async Task<IActionResult> GetIncomeExpenseHistory([FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate, CancellationToken cancellationToken)
    {
        var history = await _dashboardService.GetMonthlyIncomeExpenseHistory(fromDate, toDate, cancellationToken);
        return history.ToOkActionResult();
    }

    [HttpGet("category-totals")]
    public async Task<IActionResult> GetCategoryTotals([FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate, CancellationToken cancellationToken)
    {
        var totals = await _dashboardService.GetCategoryTotals(fromDate, toDate, cancellationToken);
        return totals.ToOkActionResult();
    }

    [HttpGet("transactions-by-month")]
    public async Task<IActionResult> GetTransactionsByMonth([FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate, CancellationToken cancellationToken)
    {
        var totals = await _dashboardService.GetTransactionsByMonth(fromDate, toDate, cancellationToken);
        return totals.ToOkActionResult();
    }

    [HttpGet("investment-evolution")]
    public async Task<IActionResult> GetInvestmentEvolution([FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate, CancellationToken cancellationToken)
    {
        var totals = await _dashboardService.GetInvestmentEvolution(fromDate, toDate, cancellationToken);
        return totals.ToOkActionResult();
    }
}
