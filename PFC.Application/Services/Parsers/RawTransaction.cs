namespace PFC.Application.Services.Parsers;

internal sealed record RawTransaction(
    string ExternalId,
    DateOnly Date,
    decimal Amount,
    string Description);
