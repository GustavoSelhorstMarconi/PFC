using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PFC.API.Extensions;
using PFC.Application.Interfaces;
using PFC.Dto.Goals;

namespace PFC.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class GoalsController : ControllerBase
{
    private readonly IGoalService _goalService;

    public GoalsController(IGoalService goalService)
    {
        _goalService = goalService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGoalRequest request, CancellationToken cancellationToken)
    {
        var result = await _goalService.CreateGoalAsync(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _goalService.GetUserGoalsAsync(cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateGoalRequest request, CancellationToken cancellationToken)
    {
        var result = await _goalService.UpdateGoalAsync(id, request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("{id:guid}/contribute")]
    public async Task<IActionResult> Contribute([FromRoute] Guid id, [FromQuery] decimal amount, CancellationToken cancellationToken)
    {
        var result = await _goalService.AddContributionAsync(id, amount, cancellationToken);
        return result.ToActionResult();
    }
}
