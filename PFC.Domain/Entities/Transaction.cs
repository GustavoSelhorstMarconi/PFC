using PFC.Domain.Enums;

namespace PFC.Domain.Entities;

public sealed class Transaction : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public Guid AccountId { get; private set; }
    public Account Account { get; private set; } = null!;

    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    public TransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public DateOnly Date { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    public Guid? GoalId { get; private set; }
    public Goal? Goal { get; private set; }

    public Guid? DebtId { get; private set; }
    public Debt? Debt { get; private set; }

    public Guid? RecurrenceId { get; private set; }
    public Recurrence? Recurrence { get; private set; }

    private Transaction() { }

    public Transaction(Guid userId, Guid accountId, Guid categoryId, TransactionType type, decimal amount, DateOnly date, Guid? goalId, Guid? debtId, Guid? recurrenceId, string? description = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero");

        if (description != null && description.Length > 300)
            throw new ArgumentException("Description cannot exceed 300 characters");

        UserId = userId;
        AccountId = accountId;
        CategoryId = categoryId;
        Type = type;
        Amount = decimal.Round(amount, 2);
        Date = date;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        IsActive = true;
        GoalId = goalId;
        DebtId = debtId;
        RecurrenceId = recurrenceId;
    }

    public void Update(Guid accountId, Guid categoryId, TransactionType type, decimal amount, DateOnly date, string? description, Guid? goalId, Guid? debtId)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero");

        if (description != null && description.Length > 300)
            throw new ArgumentException("Description cannot exceed 300 characters");

        AccountId = accountId;
        CategoryId = categoryId;
        Type = type;
        Amount = decimal.Round(amount, 2);
        Date = date;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        GoalId = goalId;
        DebtId = debtId;
        SetUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdated();
    }
}
