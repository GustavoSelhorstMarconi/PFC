using PFC.Application.Common;
using PFC.Application.Interfaces;
using PFC.Domain.Enums;
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

    public async Task<Result<DashboardSummaryResponse>> GetDashboardSummary(DateOnly? date, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var referenceDate = date ?? DateOnly.FromDateTime(DateTime.Now);
        var dateEnd = referenceDate.AddMonths(1);

        var transactions = await _transactionRepository
            .GetByUserIdAsync(userId, null, null, cancellationToken);

        if (!transactions.Any())
        {
            return Result.Success(new DashboardSummaryResponse());
        }

        var totalBalance = transactions
            .Sum(t => t.Type == TransactionType.Income ? t.Amount : -t.Amount);

        var monthIncome = transactions
            .Where(t => t.Date >= referenceDate && t.Date < dateEnd && t.Type == TransactionType.Income)
            .Sum(t => (decimal?)t.Amount) ?? 0;

        var monthExpense = transactions
            .Where(t => t.Date >= referenceDate && t.Date < dateEnd && t.Type == TransactionType.Expense)
            .Sum(t => (decimal?)t.Amount) ?? 0;

        return new DashboardSummaryResponse
        {
            TotalBalance = totalBalance,
            MonthIncome = monthIncome,
            MonthExpense = monthExpense,
            MonthResult = monthIncome - monthExpense
        };
    }

    public async Task<Result<IEnumerable<MonthlyIncomeExpenseResponse>>> GetMonthlyIncomeExpenseHistory(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var today = DateOnly.FromDateTime(DateTime.Now);
        var from = new DateOnly(today.Year, today.Month, 1).AddMonths(-11);
        var to = new DateOnly(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

        var monthlyData = await _transactionRepository.GetMonthlyIncomeExpenseAsync(userId, startDate: from, endDate: to, cancellationToken);

        var response = monthlyData.Select(m => new MonthlyIncomeExpenseResponse
        {
            Month = m.Month,
            Year = m.Year,
            Income = m.Income,
            Expense = m.Expense
        });

        return Result.Success(response);
    }
}
