using PFC.Domain.ValueObjects;

namespace PFC.Domain.Entities;

public sealed class User : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;
    public List<Account> Accounts { get; private set; } = new();
    public List<Category> Categories { get; private set; } = new();
    public List<Transaction> Transactions { get; private set; } = new();
    public List<Recurrence> Recurrences { get; private set; } = new();
    public List<Goal> Goals { get; private set; } = new();

    private User() { }

    public User(string name, Email email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty");

        Name = name.Trim();
        Email = email.Address;
        PasswordHash = passwordHash;
    }

    public void ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty");

        Name = name.Trim();
        SetUpdated();
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        SetUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdated();
    }
}