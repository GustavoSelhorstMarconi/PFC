using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PFC.API.Extensions;
using PFC.Application.Interfaces;
using PFC.Dto.Recurrences;

namespace PFC.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class RecurrencesController : ControllerBase
{
    private readonly IRecurrenceService _recurrenceService;

    public RecurrencesController(IRecurrenceService recurrenceService)
    {
        _recurrenceService = recurrenceService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRecurrenceRequest request, CancellationToken cancellationToken)
    {
        var result = await _recurrenceService.CreateRecurrenceAsync(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _recurrenceService.GetUserRecurrencesAsync(cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRecurrenceRequest request, CancellationToken cancellationToken)
    {
        var result = await _recurrenceService.UpdateRecurrenceAsync(id, request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _recurrenceService.DeactivateRecurrenceAsync(id, cancellationToken);
        return result.ToNoContentActionResult();
    }

    [HttpGet("projection")]
    public async Task<IActionResult> Projection([FromQuery] DateTime from, [FromQuery] DateTime to, CancellationToken cancellationToken)
    {
        var result = await _recurrenceService.GetProjectedOccurrencesAsync(from, to, cancellationToken);
        return result.ToActionResult();
    }
}
