using PFC.Application.Common;
using PFC.Dto.Dashboard;

namespace PFC.Application.Interfaces;

public interface IDashboardService
{
    Task<Result<DashboardSummaryResponse>> GetDashboardSummary(DateOnly? date, CancellationToken cancellationToken);
}
