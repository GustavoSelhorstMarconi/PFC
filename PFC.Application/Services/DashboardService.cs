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

    public async Task<Result<DashboardSummaryResponse>> GetDashboardSummary(DateOnly? fromDate, DateOnly? toDate, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var today = DateOnly.FromDateTime(DateTime.Now);
        var from = fromDate ?? today.AddMonths(-1);
        var to = toDate ?? today;

        var transactions = await _transactionRepository
            .GetByUserIdAsync(userId, null, null, cancellationToken);

        if (!transactions.Any())
        {
            return Result.Success(new DashboardSummaryResponse());
        }

        var totalBalance = transactions
            .Sum(t => t.Type == TransactionType.Income ? t.Amount : -t.Amount);

        var monthIncome = transactions
            .Where(t => t.Date >= from && t.Date <= to && t.Type == TransactionType.Income)
            .Sum(t => (decimal?)t.Amount) ?? 0;

        var monthExpense = transactions
            .Where(t => t.Date >= from && t.Date <= to && t.Type == TransactionType.Expense)
            .Sum(t => (decimal?)t.Amount) ?? 0;

        return new DashboardSummaryResponse
        {
            TotalBalance = totalBalance,
            MonthIncome = monthIncome,
            MonthExpense = monthExpense,
            MonthResult = monthIncome - monthExpense
        };
    }

    public async Task<Result<IEnumerable<MonthlyIncomeExpenseResponse>>> GetMonthlyIncomeExpenseHistory(DateOnly? fromDate, DateOnly? toDate, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var today = DateOnly.FromDateTime(DateTime.Now);
        var from = fromDate.HasValue ? fromDate.Value : today.AddMonths(-1);
        var to = toDate.HasValue ? toDate.Value : today;

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

    public async Task<Result<CategoryTotalsResponse>> GetCategoryTotals(DateOnly? fromDate, DateOnly? toDate, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var today = DateOnly.FromDateTime(DateTime.Now);
        var from = fromDate ?? today.AddMonths(-1);
        var to = toDate ?? today;

        var transactions = await _transactionRepository.GetCategoryTotalsByRangeAsync(userId, from, to, [TransactionType.Income, TransactionType.Expense], cancellationToken);

        var incomes = transactions
            .Where(transaction => transaction.Type == TransactionType.Income);
        var expenses = transactions
            .Where(transaction => transaction.Type == TransactionType.Expense);

        var response = new CategoryTotalsResponse
        {
            IncomeTotals = incomes.Select(x => new CategoryTotalItemResponse
            {
                CategoryName = x.Name,
                Color = x.Color,
                Total = x.Total
            }),
            ExpenseTotals = expenses.Select(x => new CategoryTotalItemResponse
            {
                CategoryName = x.Name,
                Color = x.Color,
                Total = x.Total
            })
        };

        return Result.Success(response);
    }
}
