using PFC.Application.Common;
using PFC.Dto.Categories;

namespace PFC.Application.Interfaces;

public interface ICategoryService
{
    Task<Result<CategoryResponse>> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken);
    Task<Result<CategoryResponse>> UpdateCategoryAsync(Guid categoryId, UpdateCategoryRequest request, CancellationToken cancellationToken);
    Task<Result<IEnumerable<CategoryResponse>>> GetUserCategoriesAsync(CancellationToken cancellationToken);
}
