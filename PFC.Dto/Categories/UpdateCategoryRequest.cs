namespace PFC.Dto.Categories;

public sealed class UpdateCategoryRequest
{
    public string Name { get; set; } = null!;
    public string Color { get; set; } = null!;
    public string? Icon { get; set; }
    public bool IsActive { get; set; }
}
