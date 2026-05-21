using AutoMapper;
using FluentValidation;
using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.Exceptions;
using ShopMgmt.Application.Interfaces;
using ShopMgmt.Application.Interfaces.Repositories;
using ShopMgmt.Application.Interfaces.Services;
using ShopMgmt.Domain.Entities;

namespace ShopMgmt.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly IAuditRecorder _auditRecorder;
    private readonly IValidator<CreateCategoryDto> _createValidator;
    private readonly IValidator<UpdateCategoryDto> _updateValidator;

    public CategoryService(
        ICategoryRepository categoryRepository,
        IMapper mapper,
        IAuditRecorder auditRecorder,
        IValidator<CreateCategoryDto> createValidator,
        IValidator<UpdateCategoryDto> updateValidator)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _auditRecorder = auditRecorder;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        var result = new List<CategoryDto>();

        foreach (var category in categories)
        {
            var dto = _mapper.Map<CategoryDto>(category);
            dto.MaterialCount = await _categoryRepository.GetMaterialCountAsync(category.CategoryId, cancellationToken);
            result.Add(dto);
        }

        return result;
    }

    public async Task<CategoryDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Category {id} was not found.");

        var dto = _mapper.Map<CategoryDto>(category);
        dto.MaterialCount = await _categoryRepository.GetMaterialCountAsync(id, cancellationToken);
        return dto;
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(dto, cancellationToken);

        if (await _categoryRepository.GetByNameAsync(dto.Name, cancellationToken) is not null)
            throw new ConflictException($"Category '{dto.Name}' already exists.");

        var category = _mapper.Map<Category>(dto);
        var created = await _categoryRepository.AddAsync(category, cancellationToken);

        await _auditRecorder.RecordAsync("Create", nameof(Category), created.CategoryId, $"Created category '{created.Name}'", cancellationToken);

        var result = _mapper.Map<CategoryDto>(created);
        result.MaterialCount = 0;
        return result;
    }

    public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(dto, cancellationToken);

        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Category {id} was not found.");

        var duplicate = await _categoryRepository.GetByNameAsync(dto.Name, cancellationToken);
        if (duplicate is not null && duplicate.CategoryId != id)
            throw new ConflictException($"Category '{dto.Name}' already exists.");

        category.Name = dto.Name;
        category.Description = dto.Description;
        await _categoryRepository.UpdateAsync(category, cancellationToken);

        await _auditRecorder.RecordAsync("Update", nameof(Category), id, $"Updated category '{category.Name}'", cancellationToken);

        var result = _mapper.Map<CategoryDto>(category);
        result.MaterialCount = await _categoryRepository.GetMaterialCountAsync(id, cancellationToken);
        return result;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Category {id} was not found.");

        var materialCount = await _categoryRepository.GetMaterialCountAsync(id, cancellationToken);
        if (materialCount > 0)
            throw new ConflictException($"Cannot delete category '{category.Name}' because {materialCount} material(s) reference it.");

        await _categoryRepository.DeleteAsync(category, cancellationToken);
        await _auditRecorder.RecordAsync("Delete", nameof(Category), id, $"Deleted category '{category.Name}'", cancellationToken);
    }
}
