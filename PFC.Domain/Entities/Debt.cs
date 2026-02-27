namespace PFC.Domain.Entities;

public sealed class Debt : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public string Name { get; private set; } = null!;
    public decimal TotalAmount { get; private set; }
    public decimal RemainingAmount { get; private set; }
    public decimal? InterestRate { get; private set; }
    public DateOnly? DueDate { get; private set; }
    public bool IsActive { get; private set; } = true;

    public List<Transaction> Transactions { get; private set; } = new();

    private Debt() { }

    public Debt(Guid userId, string name, decimal totalAmount, decimal? interestRate = null, DateOnly? dueDate = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty");

        if (totalAmount <= 0)
            throw new ArgumentException("TotalAmount must be greater than zero");

        if (dueDate.HasValue && dueDate.Value <= DateOnly.FromDateTime(DateTime.Now))
            throw new ArgumentException("DueDate must be a future date");

        UserId = userId;
        Name = name.Trim();
        TotalAmount = decimal.Round(totalAmount, 2);
        RemainingAmount = TotalAmount;
        InterestRate = interestRate.HasValue ? decimal.Round(interestRate.Value, 2) : null;
        DueDate = dueDate;
        IsActive = true;
    }

    public void Update(string name, decimal totalAmount, decimal? interestRate, DateOnly? dueDate, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty");

        if (totalAmount <= 0)
            throw new ArgumentException("TotalAmount must be greater than zero");

        if (dueDate.HasValue && dueDate.Value <= DateOnly.FromDateTime(DateTime.Now))
            throw new ArgumentException("DueDate must be a future date");

        if (RemainingAmount > totalAmount)
            throw new ArgumentException("TotalAmount cannot be less than RemainingAmount");

        Name = name.Trim();
        TotalAmount = decimal.Round(totalAmount, 2);
        InterestRate = interestRate.HasValue ? decimal.Round(interestRate.Value, 2) : null;
        DueDate = dueDate;
        IsActive = isActive;
        SetUpdated();
    }

    public void RegisterPayment(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero");

        var newRemaining = decimal.Round(RemainingAmount - amount, 2);
        if (newRemaining < 0)
            throw new ArgumentException("Payment would make RemainingAmount negative");

        RemainingAmount = newRemaining;
        SetUpdated();
    }

    public void ReversePayment(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero");

        var newRemaining = decimal.Round(RemainingAmount + amount, 2);
        if (newRemaining > TotalAmount)
            throw new ArgumentException("Reversed payment would exceed TotalAmount");

        RemainingAmount = newRemaining;
        SetUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdated();
    }

    public bool IsPaid() => RemainingAmount == 0m;
}
