namespace PFC.Application.Services.Parsers;

internal interface ITransactionFileParser
{
    IEnumerable<RawTransaction> Parse(Stream stream);
}
