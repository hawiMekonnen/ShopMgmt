using System;
using System.Threading.Tasks;

namespace ShopMgmt.Application.Interfaces.Services;

public interface IReportService
{
    Task<byte[]> GenerateStockSummaryPdfAsync();
    Task<byte[]> GenerateUsageByShopPdfAsync(int? shopId, DateTime? from, DateTime? to);
    Task<byte[]> GenerateAuditTrailCsvAsync(DateTime? from, DateTime? to);
    Task<byte[]> GenerateActiveAlertsCsvAsync();
}
