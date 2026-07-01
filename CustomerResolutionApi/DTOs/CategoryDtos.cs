namespace CustomerResolutionApi.DTOs;

public class CreateCategoryRequest
{
    public string CategoryName { get; set; } = string.Empty;
}

public class CategoryResponse
{
    public int Id { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}
