using PFC.Application.Common;
using PFC.Dto.Import;

namespace PFC.Application.Interfaces;

public interface IImportService
{
    Task<Result<ImportPreviewResponse>> PreviewAsync(Stream fileStream, string fileName, CancellationToken cancellationToken);
    Task<Result<ConfirmImportResponse>> ConfirmAsync(ConfirmImportRequest request, CancellationToken cancellationToken);
}
