using System.Globalization;
using System.Xml.Linq;

namespace PFC.Application.Services.Parsers;

internal sealed class OfxTransactionParser : ITransactionFileParser
{
    public IEnumerable<RawTransaction> Parse(Stream stream)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);
        var content = reader.ReadToEnd();

        var xmlStart = content.IndexOf("<OFX>", StringComparison.OrdinalIgnoreCase);
        if (xmlStart < 0)
            return [];

        XDocument doc;
        try
        {
            doc = XDocument.Parse(content[xmlStart..]);
        }
        catch
        {
            return [];
        }

        return doc.Descendants("STMTTRN")
            .Select(ParseTransaction)
            .Where(t => t is not null)
            .Select(t => t!);
    }

    private static RawTransaction? ParseTransaction(XElement element)
    {
        var fitid = element.Element("FITID")?.Value;
        var dtPosted = element.Element("DTPOSTED")?.Value;
        var trnAmt = element.Element("TRNAMT")?.Value;
        var memo = element.Element("MEMO")?.Value ?? string.Empty;

        if (fitid is null || dtPosted is null || trnAmt is null)
            return null;

        // DTPOSTED format: 20260223000000[-3:BRT] — first 8 chars are yyyyMMdd
        if (dtPosted.Length < 8)
            return null;

        if (!DateOnly.TryParseExact(dtPosted[..8], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return null;

        if (!decimal.TryParse(trnAmt, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
            return null;

        return new RawTransaction(fitid, date, amount, memo);
    }
}
