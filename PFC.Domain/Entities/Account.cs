using PFC.Domain.Enums;

namespace PFC.Domain.Entities;

public sealed class Account : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = null!;
    public AccountType Type { get; private set; }
    public decimal InitialBalance { get; private set; }
    public bool IsActive { get; private set; } = true;
    public User User { get; private set; }
    public List<Transaction> Transactions { get; private set; } = new();

    private Account() { }

    public Account(Guid userId, string name, AccountType type, decimal initialBalance)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty");

        if (initialBalance < 0)
            throw new ArgumentException("Initial balance cannot be negative");

        UserId = userId;
        Name = name.Trim();
        Type = type;
        InitialBalance = decimal.Round(initialBalance, 2);
        IsActive = true;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty");

        Name = name.Trim();
        SetUpdated();
    }

    public void ChangeStatus(bool isActive)
    {
        IsActive = isActive;
        SetUpdated();
    }
}
