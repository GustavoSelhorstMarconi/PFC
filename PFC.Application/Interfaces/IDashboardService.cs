using PFC.Application.Common;
using PFC.Dto.Dashboard;

namespace PFC.Application.Interfaces;

public interface IDashboardService
{
    Task<Result<MonthlySummaryDto>> GetMonthlySummaryAsync(int month, int year, CancellationToken cancellationToken);
}
