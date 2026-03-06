namespace PFC.Dto.Import;

public sealed class ConfirmImportRequest
{
    public List<ConfirmImportItem> Items { get; set; } = [];
}
