using PFC.Domain.Enums;

namespace PFC.Dto.Accounts;

public sealed class UpdateAccountRequest
{
    public string Name { get; set; } = null!;
    public AccountType Type { get; set; }
    public bool IsActive { get; set; }
}
