namespace PFC.Dto.Import;

public sealed class ConfirmImportResponse
{
    public int ImportedCount { get; set; }
    public int FailedCount { get; set; }
    public string? ErrorFileCsvBase64 { get; set; }
}
