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
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrentUserService _currentUserService;

    public DashboardService(ITransactionRepository transactionRepository, IAccountRepository accountRepository, ICurrentUserService currentUserService)
    {
        _transactionRepository = transactionRepository;
        _accountRepository = accountRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<DashboardSummaryResponse>> GetDashboardSummary(DateOnly? fromDate, DateOnly? toDate, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var (from, to) = GetDateRange(fromDate, toDate, -1);

        var accounts = await _accountRepository.GetByUserIdAsync(userId, cancellationToken);

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

        var totalAccountsInvestment = accounts
            .Where(a => a.Type == AccountType.Investment)
            .Sum(a => a.InitialBalance);

        var totalAccountsOutvestment = accounts
            .Where(a => a.Type != AccountType.Investment)
            .Sum(a => a.InitialBalance);

        var totalBalance = transactionsOutvestment
            .Sum(t => t.Type == TransactionType.Income ? t.Amount : -t.Amount);

        var totalInvestments = transactionsInvestment
            .Sum(t => t.Amount);

        var monthIncome = transactionsOutvestment
            .Where(t => t.Date >= from && t.Date <= to && t.Type == TransactionType.Income)
            .Sum(t => (decimal?)t.Amount) ?? 0;

        var monthExpense = transactionsOutvestment
            .Where(t => t.Date >= from && t.Date <= to && t.Type == TransactionType.Expense)
            .Sum(t => (decimal?)t.Amount) ?? 0;

        var monthInvestments = transactionsInvestment
            .Where(t => t.Date >= from && t.Date <= to)
            .Sum(t => (decimal?)t.Amount) ?? 0;

        return new DashboardSummaryResponse
        {
            TotalBalance = totalAccountsOutvestment + totalBalance,
            TotalInvestment = totalAccountsInvestment + totalInvestments,
            MonthIncome = monthIncome,
            MonthExpense = monthExpense,
            MonthInvestment = monthInvestments,
            MonthResult = monthIncome - monthExpense
        };
    }

    public async Task<Result<IEnumerable<MonthlyIncomeExpenseResponse>>> GetMonthlyIncomeExpenseHistory(DateOnly? fromDate, DateOnly? toDate, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var (from, to) = GetDateRange(fromDate, toDate, -1);

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

        var (from, to) = GetDateRange(fromDate, toDate, -1);

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

        var (from, to) = GetDateRange(fromDate, toDate, -4);

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

    public async Task<Result<IEnumerable<InvestmentEvolutionResponse>>> GetInvestmentEvolution(DateOnly? fromDate, DateOnly? toDate, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var (from, to) = GetDateRange(fromDate, toDate, -12);

        var investmentAccounts = await _accountRepository.GetInvestmentAccountsAsync(userId, cancellationToken);

        var investmentTransactions = await _transactionRepository.GetInvestmentTransactionsAsync(userId, to, cancellationToken);

        var result = new List<InvestmentEvolutionResponse>();
        var current = new DateOnly(from.Year, from.Month, 1);
        var end = new DateOnly(to.Year, to.Month, 1);

        while (current <= end)
        {
            var endOfMonth = new DateOnly(current.Year, current.Month, DateTime.DaysInMonth(current.Year, current.Month));

            var initialBalances = investmentAccounts
                .Where(a => DateOnly.FromDateTime(a.CreatedAt) <= endOfMonth)
                .Sum(a => a.InitialBalance);

            var transactionSum = investmentTransactions
                .Where(t => t.Date <= endOfMonth)
                .Sum(t => t.Type == TransactionType.Income ? t.Amount : -t.Amount);

            result.Add(new InvestmentEvolutionResponse
            {
                Month = current.Month,
                Year = current.Year,
                InvestmentValue = initialBalances + transactionSum
            });

            current = current.AddMonths(1);
        }

        return Result.Success<IEnumerable<InvestmentEvolutionResponse>>(result);
    }

    private (DateOnly from, DateOnly to) GetDateRange(DateOnly? fromDate, DateOnly? toDate, int monthDiff)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var from = fromDate.HasValue ? fromDate.Value : today.AddMonths(monthDiff);
        var to = toDate.HasValue ? toDate.Value : today;
        return (from, to);
    }
}
