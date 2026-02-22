using PFC.Domain.Enums;

namespace PFC.Dto.Accounts;

public sealed class CreateAccountRequest
{
    public string Name { get; set; } = null!;
    public AccountType Type { get; set; }
    public decimal InitialBalance { get; set; }
}
