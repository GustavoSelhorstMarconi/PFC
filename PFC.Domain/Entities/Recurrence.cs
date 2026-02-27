using PFC.Domain.Enums;

namespace PFC.Domain.Entities;

public sealed class Recurrence : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public Guid AccountId { get; private set; }
    public Account Account { get; private set; } = null!;

    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    public TransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public string? Description { get; private set; }

    public RecurrenceFrequency Frequency { get; private set; }
    public int Interval { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Recurrence() { }

    public Recurrence(Guid userId, Guid accountId, Guid categoryId, TransactionType type, decimal amount, string? description,
        RecurrenceFrequency frequency, int interval, DateOnly startDate, DateOnly? endDate)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero");

        if (interval < 1)
            throw new ArgumentException("Interval must be at least 1");

        if (endDate.HasValue && endDate.Value <= startDate)
            throw new ArgumentException("EndDate must be greater than StartDate");

        if (description != null && description.Length > 300)
            throw new ArgumentException("Description cannot exceed 300 characters");

        UserId = userId;
        AccountId = accountId;
        CategoryId = categoryId;
        Type = type;
        Amount = decimal.Round(amount, 2);
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Frequency = frequency;
        Interval = interval;
        StartDate = startDate;
        EndDate = endDate;
        IsActive = true;
    }

    public void Update(Guid accountId, Guid categoryId, TransactionType type, decimal amount, string? description,
        RecurrenceFrequency frequency, int interval, bool isActive, DateOnly startDate, DateOnly? endDate)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero");

        if (interval < 1)
            throw new ArgumentException("Interval must be at least 1");

        if (endDate.HasValue && endDate.Value <= startDate)
            throw new ArgumentException("EndDate must be greater than StartDate");

        if (description != null && description.Length > 300)
            throw new ArgumentException("Description cannot exceed 300 characters");

        AccountId = accountId;
        CategoryId = categoryId;
        Type = type;
        Amount = decimal.Round(amount, 2);
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Frequency = frequency;
        Interval = interval;
        IsActive = isActive;
        StartDate = startDate;
        EndDate = endDate;
        SetUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdated();
    }
}
