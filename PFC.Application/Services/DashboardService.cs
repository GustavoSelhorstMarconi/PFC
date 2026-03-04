using PFC.Application.Common;
using PFC.Application.Interfaces;
using PFC.Domain.Enums;
using PFC.Domain.Interfaces;
using PFC.Dto.Dashboard;
using PFC.Dto.Transactions;

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

        var transactionsInvestment = transactions
            .Where(t => t.Account.Type == AccountType.Investment);

        var transactionsOutvestment = transactions
            .Where(t => t.Account.Type != AccountType.Investment);

        var totalBalance = transactionsOutvestment
            .Sum(t => t.Type == TransactionType.Income ? t.Amount : -t.Amount);

        var totalInvestiments = transactionsInvestment
            .Sum(t => t.Amount);

        var monthIncome = transactionsOutvestment
            .Where(t => t.Date >= from && t.Date <= to && t.Type == TransactionType.Income)
            .Sum(t => (decimal?)t.Amount) ?? 0;

        var monthExpense = transactionsOutvestment
            .Where(t => t.Date >= from && t.Date <= to && t.Type == TransactionType.Expense)
            .Sum(t => (decimal?)t.Amount) ?? 0;

        var monthInvestiments = transactionsInvestment
            .Where(t => t.Date >= from && t.Date <= to)
            .Sum(t => (decimal?)t.Amount) ?? 0;

        return new DashboardSummaryResponse
        {
            TotalBalance = totalBalance,
            TotalInvestiment = totalInvestiments,
            MonthIncome = monthIncome,
            MonthExpense = monthExpense,
            MonthInvestiment = monthInvestiments,
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

    public async Task<Result<IEnumerable<TransactionsByMonthResponse>>> GetTransactionsByMonth(DateOnly? fromDate, DateOnly? toDate, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var today = DateOnly.FromDateTime(DateTime.Now);
        var from = fromDate.HasValue ? fromDate.Value : today.AddMonths(-4);
        var to = toDate.HasValue ? toDate.Value : today;

        var transactions = await _transactionRepository.GetTransactionsByRangeDate(userId, from, to, cancellationToken);

        var response = transactions
            .GroupBy(t => new { t.Date.Year, t.Date.Month })
            .Select(g => new TransactionsByMonthResponse
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Transactions = g.Select(t => new TransactionResponse
                {
                    Id = t.Id,
                    AccountName = t.Account.Name,
                    CategoryName = t.Category.Name,
                    Type = t.Type,
                    Amount = t.Amount,
                    Date = t.Date,
                    Description = t.Description,
                    IsActive = t.IsActive,
                }).ToList()
            });

        return Result.Success(response);
    }
}
