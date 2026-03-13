using PFC.Application.Common;
using PFC.Domain.Models;
using PFC.Dto.Categories;
using PFC.Dto.Common;

namespace PFC.Application.Interfaces;

public interface ICategoryService
{
    Task<Result<CategoryResponse>> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken);
    Task<Result<CategoryResponse>> UpdateCategoryAsync(Guid categoryId, UpdateCategoryRequest request, CancellationToken cancellationToken);
    Task<Result<IEnumerable<CategoryResponse>>> GetUserCategoriesAsync(CancellationToken cancellationToken);
    Task<Result<PagedResponse<CategoryResponse>>> GetUserCategoriesPagedAsync(PagedRequest request, CancellationToken cancellationToken);
}
