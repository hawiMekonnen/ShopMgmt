using System.Globalization;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ShopMgmt.Application.Interfaces.Services;
using ShopMgmt.Infrastructure.Context;

namespace ShopMgmt.Infrastructure.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _context;

    public ReportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> GenerateStockSummaryPdfAsync()
    {
        var materials = await _context.Materials
            .Include(m => m.Category)
            .Include(m => m.StockBatches)
            .Include(m => m.Usages)
            .AsNoTracking()
            .ToListAsync();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.Header()
                    .Text("Ethiopian Airlines – Stock Summary Report")
                    .FontSize(16).Bold().AlignCenter();

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(3);
                        c.RelativeColumn(2);
                        c.RelativeColumn(2);
                        c.RelativeColumn(2);
                    });

                    table.Header(h =>
                    {
                        h.Cell().Text("Material").Bold();
                        h.Cell().Text("Category").Bold();
                        h.Cell().Text("On-Hand Qty").Bold();
                        h.Cell().Text("Unit Price").Bold();
                    });

                    foreach (var m in materials)
                    {
                        var received = m.StockBatches.Sum(b => b.QuantityReceived);
                        var used = m.Usages.Sum(u => u.QuantityUsed);
                        var onHand = received - used;

                        table.Cell().Text(m.Name);
                        table.Cell().Text(m.Category?.Name ?? "-");
                        table.Cell().Text(onHand.ToString("F2"));
                        table.Cell().Text(m.UnitPrice.ToString("C"));
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                    x.Span(" of ");
                    x.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    public async Task<byte[]> GenerateUsageByShopPdfAsync(int? shopId, DateTime? from, DateTime? to)
    {
        var query = _context.MaterialUsages
            .Include(u => u.Material)
            .Include(u => u.Shop)
            .AsQueryable();

        if (shopId.HasValue)
            query = query.Where(u => u.ShopId == shopId.Value);
        if (from.HasValue)
            query = query.Where(u => u.UsedAt >= from.Value);
        if (to.HasValue)
            query = query.Where(u => u.UsedAt <= to.Value);

        var usages = await query.AsNoTracking().OrderByDescending(u => u.UsedAt).ToListAsync();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.Header()
                    .Text($"Ethiopian Airlines – Usage Report" +
                          $"{(shopId.HasValue ? $" (Shop {shopId})" : "")}")
                    .FontSize(16).Bold().AlignCenter();

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(2);
                        c.RelativeColumn(2);
                        c.RelativeColumn(2);
                        c.RelativeColumn(2);
                        c.RelativeColumn(2);
                    });

                    table.Header(h =>
                    {
                        h.Cell().Text("Date").Bold();
                        h.Cell().Text("Material").Bold();
                        h.Cell().Text("Shop").Bold();
                        h.Cell().Text("Qty Used").Bold();
                        h.Cell().Text("Tail No.").Bold();
                    });

                    foreach (var u in usages)
                    {
                        table.Cell().Text(u.UsedAt.ToString("yyyy-MM-dd"));
                        table.Cell().Text(u.Material?.Name ?? "-");
                        table.Cell().Text(u.Shop?.Name ?? "-");
                        table.Cell().Text(u.QuantityUsed.ToString("F2"));
                        table.Cell().Text(u.TailNumber ?? "-");
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                    x.Span(" of ");
                    x.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    public async Task<byte[]> GenerateAuditTrailCsvAsync(DateTime? from, DateTime? to)
    {
        var query = _context.AuditLogs.Include(l => l.User).AsQueryable();

        if (from.HasValue)
            query = query.Where(l => l.Timestamp >= from.Value);
        if (to.HasValue)
            query = query.Where(l => l.Timestamp <= to.Value);

        var logs = await query.OrderByDescending(l => l.Timestamp).AsNoTracking().ToListAsync();

        using var ms = new MemoryStream();
        using var writer = new StreamWriter(ms, leaveOpen: true);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csv.WriteRecords(logs.Select(l => new
        {
            l.LogId,
            l.Action,
            l.Entity,
            l.EntityId,
            PerformedBy = l.User?.Name ?? l.PerformedBy.ToString(),
            Timestamp = l.Timestamp.ToString("o"),
            l.Details
        }));

        await writer.FlushAsync();
        return ms.ToArray();
    }

    public async Task<byte[]> GenerateActiveAlertsCsvAsync()
    {
        var alerts = await _context.Alerts
            .Include(a => a.Material)
            .Where(a => a.ResolvedAt == null)
            .AsNoTracking()
            .ToListAsync();

        using var ms = new MemoryStream();
        using var writer = new StreamWriter(ms, leaveOpen: true);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csv.WriteRecords(alerts.Select(a => new
        {
            a.AlertId,
            MaterialName = a.Material?.Name ?? "-",
            Type = a.Type.ToString(),
            a.Threshold,
            a.CurrentQuantity,
            TriggeredAt = a.TriggeredAt.ToString("o")
        }));

        await writer.FlushAsync();
        return ms.ToArray();
    }
}
