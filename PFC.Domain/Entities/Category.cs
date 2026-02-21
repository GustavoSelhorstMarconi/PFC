using PFC.Domain.Enums;

namespace PFC.Domain.Entities;

public sealed class Category : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public string Name { get; private set; } = null!;
    public CategoryType Type { get; private set; }
    public string Color { get; private set; } = null!;
    public string? Icon { get; private set; }
    public bool IsActive { get; private set; } = true;
    public List<Transaction> Transactions { get; private set; } = new();

    private Category() { }

    public Category(Guid userId, string name, CategoryType type, string color, string? icon = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty");

        if (!Enum.IsDefined(typeof(CategoryType), type))
            throw new ArgumentException("Invalid category type");

        if (string.IsNullOrWhiteSpace(color) || !IsValidHex(color))
            throw new ArgumentException("Color must be a valid hex string like #FFAABB");

        UserId = userId;
        Name = name.Trim();
        Type = type;
        Color = color.ToUpper();
        Icon = string.IsNullOrWhiteSpace(icon) ? null : icon.Trim();
        IsActive = true;
    }

    public void Update(string name, string color, string? icon)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty");

        if (string.IsNullOrWhiteSpace(color) || !IsValidHex(color))
            throw new ArgumentException("Color must be a valid hex string like #FFAABB");

        Name = name.Trim();
        Color = color.ToUpper();
        Icon = string.IsNullOrWhiteSpace(icon) ? null : icon.Trim();
        SetUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdated();
    }

    private static bool IsValidHex(string value)
    {
        if (value.Length != 7) return false;
        if (!value.StartsWith('#')) return false;
        for (int i = 1; i < 7; i++)
        {
            char c = value[i];
            bool isHex = (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');
            if (!isHex) return false;
        }
        return true;
    }
}
