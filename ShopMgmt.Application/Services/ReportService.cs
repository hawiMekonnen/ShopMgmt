using System;
using System.IO;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using CsvHelper;
using System.Globalization;
using ShopMgmt.Application.Interface;
using ShopMgmt.Application.DTOS;

namespace ShopMgmt.Application.Services;

public class ReportService : IReportService
{
    public async Task<byte[]> GenerateStockSummaryPdfAsync()
    {
        // Placeholder PDF generation using QuestPDF
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.Content().Text("Stock Summary Report – generated placeholder");
            });
        });

        using var memory = new MemoryStream();
        document.GeneratePdf(memory);
        return await Task.FromResult(memory.ToArray());
    }

    public async Task<byte[]> GenerateUsageByShopPdfAsync(int? shopId, DateTime? from, DateTime? to)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.Content().Text($"Usage Report for Shop {(shopId.HasValue ? shopId.Value.ToString() : "All")}.\nPeriod: {(from?.ToString("yyyy-MM-dd") ?? "-")} - {(to?.ToString("yyyy-MM-dd") ?? "-")}" );
            });
        });

        using var memory = new MemoryStream();
        document.GeneratePdf(memory);
        return await Task.FromResult(memory.ToArray());
    }

    public async Task<byte[]> GenerateAuditTrailCsvAsync(DateTime? from, DateTime? to)
    {
        using var memory = new MemoryStream();
        using var writer = new StreamWriter(memory, leaveOpen: true);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        // Header placeholder
        csv.WriteField("LogId");
        csv.WriteField("Action");
        csv.WriteField("Entity");
        csv.WriteField("EntityId");
        csv.WriteField("PerformedBy");
        csv.WriteField("Timestamp");
        csv.WriteField("Details");
        await csv.NextRecordAsync();
        // No data rows – placeholder
        await writer.FlushAsync();
        return memory.ToArray();
    }

    public async Task<byte[]> GenerateActiveAlertsCsvAsync()
    {
        using var memory = new MemoryStream();
        using var writer = new StreamWriter(memory, leaveOpen: true);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteField("AlertId");
        csv.WriteField("MaterialName");
        csv.WriteField("Type");
        csv.WriteField("Threshold");
        csv.WriteField("CurrentQuantity");
        csv.WriteField("TriggeredAt");
        await csv.NextRecordAsync();
        await writer.FlushAsync();
        return await Task.FromResult(memory.ToArray());
    }
}
