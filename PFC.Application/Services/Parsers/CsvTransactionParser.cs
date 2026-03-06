using System.Globalization;

namespace PFC.Application.Services.Parsers;

internal sealed class CsvTransactionParser : ITransactionFileParser
{
    public IEnumerable<RawTransaction> Parse(Stream stream)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);

        reader.ReadLine(); // skip header

        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // Split into max 4 parts: Data, Valor, Identificador, Descrição
            var parts = line.Split(',', 4);
            if (parts.Length < 4)
                continue;

            if (!DateOnly.TryParseExact(parts[0].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                continue;

            if (!decimal.TryParse(parts[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
                continue;

            var externalId = parts[2].Trim();
            var description = parts[3].Trim();

            yield return new RawTransaction(externalId, date, amount, description);
        }
    }
}
