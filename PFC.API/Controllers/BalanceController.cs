using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PFC.API.Extensions;
using PFC.Application.Interfaces;

namespace PFC.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class BalanceController : ControllerBase
{
    private readonly IBalanceService _balanceService;

    public BalanceController(IBalanceService balanceService)
    {
        _balanceService = balanceService;
    }

    [HttpGet("total")]
    public async Task<IActionResult> GetTotal(CancellationToken cancellationToken)
    {
        var total = await _balanceService.GetTotalBalanceAsync(cancellationToken);
        return total.ToOkActionResult();
    }

    [HttpGet("accounts")]
    public async Task<IActionResult> GetAccounts(CancellationToken cancellationToken)
    {
        var accounts = await _balanceService.GetAccountsBalancesAsync(cancellationToken);
        return accounts.ToOkActionResult();
    }
}
