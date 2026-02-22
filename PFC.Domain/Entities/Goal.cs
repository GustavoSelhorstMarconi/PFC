namespace PFC.Domain.Entities;

public sealed class Goal : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public string Name { get; private set; } = null!;
    public decimal TargetAmount { get; private set; }
    public decimal CurrentAmount { get; private set; }
    public DateTime? Deadline { get; private set; }
    public bool IsActive { get; private set; } = true;

    public List<Transaction> Transactions { get; private set; } = new();

    private Goal() { }

    public Goal(Guid userId, string name, decimal targetAmount, DateTime? deadline = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty");

        if (targetAmount <= 0)
            throw new ArgumentException("TargetAmount must be greater than zero");

        if (deadline.HasValue && deadline.Value.Date <= DateTime.UtcNow.Date)
            throw new ArgumentException("Deadline must be a future date");

        UserId = userId;
        Name = name.Trim();
        TargetAmount = decimal.Round(targetAmount, 2);
        CurrentAmount = 0m;
        Deadline = deadline?.Date;
        IsActive = true;
    }

    public void Update(string name, decimal targetAmount, DateTime? deadline)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty");

        if (targetAmount <= 0)
            throw new ArgumentException("TargetAmount must be greater than zero");

        if (deadline.HasValue && deadline.Value.Date <= DateTime.UtcNow.Date)
            throw new ArgumentException("Deadline must be a future date");

        if (CurrentAmount > targetAmount)
            throw new ArgumentException("TargetAmount cannot be less than CurrentAmount");

        Name = name.Trim();
        TargetAmount = decimal.Round(targetAmount, 2);
        Deadline = deadline?.Date;
        SetUpdated();
    }

    public void AddContribution(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Contribution amount must be greater than zero");

        var newAmount = decimal.Round(CurrentAmount + amount, 2);
        if (newAmount > TargetAmount)
            throw new ArgumentException("Contribution would exceed TargetAmount");

        CurrentAmount = newAmount;
        SetUpdated();
    }

    public void RemoveContribution(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero");

        var newAmount = decimal.Round(CurrentAmount - amount, 2);
        if (newAmount < 0)
            throw new ArgumentException("CurrentAmount cannot be negative");

        CurrentAmount = newAmount;
        SetUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdated();
    }

    public bool IsCompleted() => CurrentAmount >= TargetAmount;
}
