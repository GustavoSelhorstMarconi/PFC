using PFC.Dto.Transactions;

namespace PFC.Dto.Dashboard;

public class TransactionsByMonthResponse
{
    public int Month { get; set; }
    public int Year { get; set; }
    public List<TransactionResponse> Transactions { get; set; }
}
