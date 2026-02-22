using PFC.Domain.Enums;

namespace PFC.Dto.Accounts;

public sealed class AccountResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public AccountType Type { get; set; }
    public decimal InitialBalance { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
