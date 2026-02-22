using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PFC.API.Extensions;
using PFC.Application.Interfaces;
using PFC.Dto.Transactions;

namespace PFC.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransactionRequest request, CancellationToken cancellationToken)
    {
        var result = await _transactionService.CreateTransactionAsync(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? month, [FromQuery] int? year, CancellationToken cancellationToken)
    {
        var result = await _transactionService.GetUserTransactionsAsync(month, year, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateTransactionRequest request, CancellationToken cancellationToken)
    {
        var result = await _transactionService.UpdateTransactionAsync(id, request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _transactionService.DeleteTransactionAsync(id, cancellationToken);
        return result.ToNoContentActionResult();
    }
}
