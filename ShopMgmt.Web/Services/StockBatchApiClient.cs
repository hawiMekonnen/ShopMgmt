using System.Net.Http.Json;
using ShopMgmt.Application.DTOs;

namespace ShopMgmt.Web.Services;

public class StockBatchApiClient
{
    private readonly HttpClient _http;

    public StockBatchApiClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<StockBatchDto>> GetByMaterialIdAsync(int materialId)
        => await _http.GetFromJsonAsync<IReadOnlyList<StockBatchDto>>($"api/materials/{materialId}/batches") ?? [];

    public async Task<StockBatchDto?> ReceiveAsync(int materialId, CreateStockBatchDto dto)
    {
        var response = await _http.PostAsJsonAsync($"api/materials/{materialId}/batches", dto);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<StockBatchDto>()
            : null;
    }
}
