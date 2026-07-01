using CustomerResolutionApi.DTOs;

namespace CustomerResolutionApi.Services.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponse>> GetAllAsync();
    Task<CategoryResponse> CreateAsync(CreateCategoryRequest request);
}
