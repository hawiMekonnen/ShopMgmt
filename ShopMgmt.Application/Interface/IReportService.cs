using System;
using System.Threading.Tasks;

namespace ShopMgmt.Application.Interface;

public interface IReportService
{
    Task<byte[]> GenerateStockSummaryPdfAsync();
    Task<byte[]> GenerateUsageByShopPdfAsync(int? shopId, DateTime? from, DateTime? to);
    Task<byte[]> GenerateAuditTrailCsvAsync(DateTime? from, DateTime? to);
    Task<byte[]> GenerateActiveAlertsCsvAsync();
}
