using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopMgmt.Application.Interface;

namespace ShopMgmt.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    // GET /api/reports/stock-summary/pdf
    [HttpGet("stock-summary/pdf")]
    public async Task<IActionResult> StockSummaryPdf()
    {
        var bytes = await _reportService.GenerateStockSummaryPdfAsync();
        return File(bytes, "application/pdf", $"stock-summary-{DateTime.UtcNow:yyyyMMdd}.pdf");
    }

    // GET /api/reports/usage/pdf?shopId=1&from=2025-01-01&to=2025-12-31
    [HttpGet("usage/pdf")]
    public async Task<IActionResult> UsagePdf(
        [FromQuery] int? shopId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        var bytes = await _reportService.GenerateUsageByShopPdfAsync(shopId, from, to);
        return File(bytes, "application/pdf", $"usage-{DateTime.UtcNow:yyyyMMdd}.pdf");
    }

    // GET /api/reports/audit-trail/csv?from=2025-01-01&to=2025-12-31
    [HttpGet("audit-trail/csv")]
    public async Task<IActionResult> AuditTrailCsv(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        var bytes = await _reportService.GenerateAuditTrailCsvAsync(from, to);
        return File(bytes, "text/csv", $"audit-trail-{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    // GET /api/reports/alerts/csv
    [HttpGet("alerts/csv")]
    public async Task<IActionResult> AlertsCsv()
    {
        var bytes = await _reportService.GenerateActiveAlertsCsvAsync();
        return File(bytes, "text/csv", $"alerts-{DateTime.UtcNow:yyyyMMdd}.csv");
    }
}
