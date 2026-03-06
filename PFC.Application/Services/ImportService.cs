using System.Globalization;
using System.Text;
using PFC.Application.Common;
using PFC.Application.Interfaces;
using PFC.Application.Services.Parsers;
using PFC.Domain.Entities;
using PFC.Domain.Enums;
using PFC.Domain.Exceptions;
using PFC.Domain.Interfaces;
using PFC.Dto.Import;

namespace PFC.Application.Services;

public sealed class ImportService : IImportService
{
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;
    private static readonly string[] AllowedExtensions = [".csv", ".ofx"];

    private readonly ICurrentUserService _currentUserService;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBaseRepository<Transaction> _transactionRepository;

    public ImportService(
        ICurrentUserService currentUserService,
        ICategoryRepository categoryRepository,
        IBaseRepository<Transaction> transactionRepository)
    {
        _currentUserService = currentUserService;
        _categoryRepository = categoryRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<ImportPreviewResponse>> PreviewAsync(Stream fileStream, string fileName, CancellationToken cancellationToken)
    {
        ValidateFile(fileStream, fileName);

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        ITransactionFileParser parser = extension == ".csv"
            ? new CsvTransactionParser()
            : new OfxTransactionParser();

        var rawTransactions = parser.Parse(fileStream).ToList();

        if (rawTransactions.Count == 0)
            throw new BadRequestException("No transactions found in the file");

        var userId = _currentUserService.GetUserId();
        var categories = (await _categoryRepository.GetByUserIdAsync(userId, cancellationToken)).ToList();

        var items = rawTransactions.Select(raw =>
        {
            var type = raw.Amount >= 0 ? TransactionType.Income : TransactionType.Expense;
            var amount = Math.Abs(raw.Amount);
            var suggestedCategoryId = FindMatchingCategory(raw.Description, type, categories);

            return new ImportTransactionItem
            {
                ExternalId = raw.ExternalId,
                Date = raw.Date,
                Amount = amount,
                Type = type,
                Description = raw.Description,
                SuggestedCategoryId = suggestedCategoryId
            };
        }).ToList();

        return Result.Success(new ImportPreviewResponse { Transactions = items });
    }

    public async Task<Result<ConfirmImportResponse>> ConfirmAsync(ConfirmImportRequest request, CancellationToken cancellationToken)
    {
        if (request?.Items is null || request.Items.Count == 0)
            throw new BadRequestException("No items to import");

        var userId = _currentUserService.GetUserId();
        var errors = new List<ConfirmImportItem>();

        foreach (var item in request.Items)
        {
            try
            {
                var transaction = new Transaction(
                    userId,
                    item.AccountId,
                    item.CategoryId,
                    item.Type,
                    item.Amount,
                    item.Date,
                    null,
                    null,
                    null,
                    item.Description);

                await _transactionRepository.AddAsync(transaction, cancellationToken);
                await _transactionRepository.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                errors.Add(item);
            }
        }

        var imported = request.Items.Count - errors.Count;
        var errorFileCsvBase64 = errors.Count > 0 ? GenerateErrorCsv(errors) : null;

        return Result.Success(new ConfirmImportResponse
        {
            ImportedCount = imported,
            FailedCount = errors.Count,
            ErrorFileCsvBase64 = errorFileCsvBase64
        });
    }

    private static void ValidateFile(Stream fileStream, string fileName)
    {
        if (fileStream is null || fileStream.Length == 0)
            throw new BadRequestException("File is required");

        if (fileStream.Length > MaxFileSizeBytes)
            throw new BadRequestException("File size must not exceed 5MB");

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            throw new BadRequestException("Only CSV and OFX files are allowed");
    }

    private static Guid? FindMatchingCategory(string description, TransactionType type, List<Category> categories)
    {
        var normalizedDescription = NormalizeText(description);

        return categories
            .Where(c => c.IsActive && c.Type == (CategoryType)(int)type)
            .FirstOrDefault(c => normalizedDescription.Contains(NormalizeText(c.Name)))
            ?.Id;
    }

    private static string NormalizeText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalized.Length);

        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        return sb.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
    }

    private static string GenerateErrorCsv(List<ConfirmImportItem> errors)
    {
        var sb = new StringBuilder();
        sb.AppendLine("ExternalId,AccountId,CategoryId,Type,Amount,Date,Description");

        foreach (var item in errors)
        {
            sb.AppendLine(string.Join(',',
                item.ExternalId,
                item.AccountId,
                item.CategoryId,
                (int)item.Type,
                item.Amount.ToString(CultureInfo.InvariantCulture),
                item.Date.ToString("yyyy-MM-dd"),
                $"\"{item.Description?.Replace("\"", "\"\"") ?? ""}\""));
        }

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(sb.ToString()));
    }
}
