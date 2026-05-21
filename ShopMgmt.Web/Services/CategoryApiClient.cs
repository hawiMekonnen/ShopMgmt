using System.Net.Http.Json;
using ShopMgmt.Application.DTOs;

namespace ShopMgmt.Web.Services;

public class CategoryApiClient
{
    private readonly HttpClient _http;

    public CategoryApiClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync()
        => await _http.GetFromJsonAsync<IReadOnlyList<CategoryDto>>("api/categories") ?? [];

    public async Task<CategoryDto?> GetByIdAsync(int id)
        => await _http.GetFromJsonAsync<CategoryDto>($"api/categories/{id}");

    public async Task<CategoryDto?> CreateAsync(CreateCategoryDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/categories", dto);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<CategoryDto>()
            : null;
    }

    public async Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/categories/{id}", dto);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<CategoryDto>()
            : null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/categories/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<string?> GetErrorAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return null;
        return await response.Content.ReadAsStringAsync();
    }
}
