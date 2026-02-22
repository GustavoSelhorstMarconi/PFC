using PFC.Application.Common;
using PFC.Application.Interfaces;
using PFC.Domain.Interfaces;
using PFC.Dto.Dashboard;

namespace PFC.Application.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICurrentUserService _currentUserService;

    public DashboardService(ITransactionRepository transactionRepository, ICurrentUserService currentUserService)
    {
        _transactionRepository = transactionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<MonthlySummaryDto>> GetMonthlySummaryAsync(int month, int year, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var totals = await _transactionRepository.GetTotalsByUserAsync(userId, month, year, cancellationToken);

        var categories = await _transactionRepository.GetExpenseTotalsByCategoryAsync(userId, month, year, cancellationToken);

        var dto = new MonthlySummaryDto
        {
            TotalIncome = totals.TotalIncome,
            TotalExpense = totals.TotalExpense,
            Balance = totals.TotalIncome - totals.TotalExpense,
            ExpenseByCategory = categories.Select(c => new CategoryExpenseDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Color = c.Color,
                Total = c.Total
            }).ToList()
        };

        return Result.Success(dto);
    }
}
