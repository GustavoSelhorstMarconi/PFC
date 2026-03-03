using PFC.Application.Common;
using PFC.Dto.Dashboard;

namespace PFC.Application.Interfaces;

public interface IDashboardService
{
    Task<Result<DashboardSummaryResponse>> GetDashboardSummary(DateOnly? fromDate, DateOnly? toDate, CancellationToken cancellationToken);
    Task<Result<IEnumerable<MonthlyIncomeExpenseResponse>>> GetMonthlyIncomeExpenseHistory(DateOnly? fromDate, DateOnly? toDate, CancellationToken cancellationToken);
    Task<Result<CategoryTotalsResponse>> GetCategoryTotals(DateOnly? fromDate, DateOnly? toDate, CancellationToken cancellationToken);
}
