using System.Net.Http.Json;
using ShopMgmt.Application.DTOs;

namespace ShopMgmt.Web.Services;

public class MaterialApiClient
{
    private readonly HttpClient _http;

    public MaterialApiClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<MaterialListItemDto>> GetAllAsync()
        => await _http.GetFromJsonAsync<IReadOnlyList<MaterialListItemDto>>("api/materials") ?? [];

    public async Task<MaterialDetailDto?> GetByIdAsync(int id)
        => await _http.GetFromJsonAsync<MaterialDetailDto>($"api/materials/{id}");

    public async Task<MaterialDetailDto?> CreateAsync(CreateMaterialDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/materials", dto);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<MaterialDetailDto>()
            : null;
    }

    public async Task<MaterialDetailDto?> UpdateAsync(int id, UpdateMaterialDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/materials/{id}", dto);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<MaterialDetailDto>()
            : null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/materials/{id}");
        return response.IsSuccessStatusCode;
    }
}
