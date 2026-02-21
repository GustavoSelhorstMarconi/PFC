using PFC.Domain.Enums;

namespace PFC.Application.DTOs.Categories;

public sealed class CreateCategoryRequest
{
    public string Name { get; set; } = null!;
    public CategoryType Type { get; set; }
    public string Color { get; set; } = null!;
    public string? Icon { get; set; }
}
