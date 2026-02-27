using Microsoft.AspNetCore.Mvc;
using PFC.API.Extensions;
using PFC.Application.Interfaces;
using PFC.Dto.Debts;

namespace PFC.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DebtsController : ControllerBase
{
    private readonly IDebtService _debtService;

    public DebtsController(IDebtService debtService)
    {
        _debtService = debtService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDebtRequest request, CancellationToken cancellationToken)
    {
        var result = await _debtService.CreateDebtAsync(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _debtService.GetUserDebtsAsync(cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateDebtRequest request, CancellationToken cancellationToken)
    {
        var result = await _debtService.UpdateDebtAsync(id, request, cancellationToken);
        return result.ToActionResult();
    }
}
