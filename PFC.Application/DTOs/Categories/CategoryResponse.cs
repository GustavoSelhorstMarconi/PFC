using PFC.Domain.Enums;

namespace PFC.Application.DTOs.Categories;

public sealed class CategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public CategoryType Type { get; set; }
    public string Color { get; set; } = null!;
    public string? Icon { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
