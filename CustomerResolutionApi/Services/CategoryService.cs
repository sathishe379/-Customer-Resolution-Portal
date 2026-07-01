using AutoMapper;
using CustomerResolutionApi.DTOs;
using CustomerResolutionApi.Models;
using CustomerResolutionApi.Repositories.Interfaces;
using CustomerResolutionApi.Services.Interfaces;

namespace CustomerResolutionApi.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<CategoryResponse>>(categories);
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
    {
        var category = new Category { CategoryName = request.CategoryName };
        var created = await _categoryRepository.CreateAsync(category);
        return _mapper.Map<CategoryResponse>(created);
    }
}
