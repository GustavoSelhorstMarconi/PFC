using PFC.Application.Common;
using PFC.Application.Interfaces;
using PFC.Domain.Entities;
using PFC.Domain.Exceptions;
using PFC.Domain.Interfaces;
using PFC.Domain.Models;
using PFC.Dto.Categories;
using PFC.Dto.Common;

namespace PFC.Application.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBaseRepository<Category> _baseRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICurrentUserService currentUserService, IBaseRepository<Category> baseRepository, ICategoryRepository categoryRepository)
    {
        _currentUserService = currentUserService;
        _baseRepository = baseRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<CategoryResponse>> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
            throw new BadRequestException("Invalid request");

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadRequestException("Name is required");

        if (string.IsNullOrWhiteSpace(request.Color))
            throw new BadRequestException("Color is required");

        var userId = _currentUserService.GetUserId();

        var existing = await _categoryRepository.GetByUserIdAndNameAsync(userId, request.Name, cancellationToken);
        if (existing is not null)
            throw new ConflictException("Categoria com esse nome já existe para o usuário");

        var category = new Category(userId, request.Name, request.Type, request.Color, request.Icon);

        await _baseRepository.AddAsync(category, cancellationToken);
        await _baseRepository.SaveChangesAsync(cancellationToken);

        var response = new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Type = category.Type,
            Color = category.Color,
            Icon = category.Icon,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };

        return Result.Success(response);
    }

    public async Task<Result<CategoryResponse>> UpdateCategoryAsync(Guid categoryId, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
            throw new BadRequestException("Invalid request");

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadRequestException("Name is required");

        if (string.IsNullOrWhiteSpace(request.Color))
            throw new BadRequestException("Color is required");

        var userId = _currentUserService.GetUserId();

        var category = await _baseRepository.GetByIdAsync(categoryId, cancellationToken);

        if (category is null)
            throw new NotFoundException("Category not found");

        if (category.UserId is not null && category.UserId != userId)
            throw new UnauthorizedException();

        var existing = await _categoryRepository.GetByUserIdAndNameAsync(userId, request.Name, cancellationToken);
        if (existing is not null && existing.Id != category.Id)
            throw new ConflictException("Categoria com esse nome já existe para o usuário");

        category.Update(request.Name, request.Color, request.Icon, request.IsActive);

        _baseRepository.Update(category);
        await _baseRepository.SaveChangesAsync(cancellationToken);

        var response = new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Type = category.Type,
            Color = category.Color,
            Icon = category.Icon,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };

        return Result.Success(response);
    }

    public async Task<Result<IEnumerable<CategoryResponse>>> GetUserCategoriesAsync(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var categories = await _categoryRepository.GetByUserIdAsync(userId, cancellationToken);

        var result = categories.Select(c => new CategoryResponse
        {
            Id = c.Id,
            Name = c.Name,
            Type = c.Type,
            Color = c.Color,
            Icon = c.Icon,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList();

        return Result.Success<IEnumerable<CategoryResponse>>(result);
    }

    public async Task<Result<PagedResponse<CategoryResponse>>> GetUserCategoriesPagedAsync(PagedRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var (items, totalCount) = await _categoryRepository.GetByUserIdPagedAsync(userId, request, cancellationToken);

        var response = new PagedResponse<CategoryResponse>
        {
            Items = items.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type,
                Color = c.Color,
                Icon = c.Icon,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }).ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return Result.Success(response);
    }
}
