using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.Interfaces.Repositories;
using ShopMgmt.Application.Interfaces.Services;
using ShopMgmt.Domain.Entities;
using ShopMgmt.Domain.Enums;
using ShopMgmt.Infrastructure.Context;

namespace ShopMgmt.Application.Services;

public class ServiceabilityCheckService : IServiceabilityCheckService
{
    private readonly IServiceabilityCheckRepository _checkRepository;
    private readonly IStockBatchRepository _stockBatchRepository;
    private readonly AppDbContext _context;

    public ServiceabilityCheckService(
        IServiceabilityCheckRepository checkRepository,
        IStockBatchRepository stockBatchRepository,
        AppDbContext context)
    {
        _checkRepository = checkRepository;
        _stockBatchRepository = stockBatchRepository;
        _context = context;
    }

    public async Task<ServiceabilityCheckDto> RecordCheckAsync(CreateServiceabilityCheckDto dto, CancellationToken cancellationToken = default)
    {
        // Ensure batch exists
        var batch = await _stockBatchRepository.GetByIdAsync(dto.BatchId, cancellationToken)
            ?? throw new ArgumentException($"Batch {dto.BatchId} not found");

        var check = new ServiceabilityCheck
        {
            BatchId = dto.BatchId,
            TechnicianId = dto.TechnicianId,
            Passed = dto.Passed,
            ReferenceDocument = dto.ReferenceDocument,
            Notes = dto.Notes,
            CheckedAt = DateTime.UtcNow
        };

        // Persist check
        await _checkRepository.AddAsync(check, cancellationToken);

        // Update batch status based on result
        if (!dto.Passed)
        {
            batch.Status = MaterialStatus.Quarantined;
            batch.QuarantineDate = DateTime.UtcNow;
            batch.QuarantineReason = dto.Notes ?? "Failed serviceability check";
        }
        else
        {
            batch.Status = MaterialStatus.Serviceable;
            // Clear quarantine fields if any
            batch.QuarantineDate = null;
            batch.QuarantineReason = null;
        }

        // Save changes to batch
        await _context.SaveChangesAsync(cancellationToken);

        // Return DTO
        return new ServiceabilityCheckDto
        {
            CheckId = check.CheckId,
            BatchId = check.BatchId,
            TechnicianId = check.TechnicianId,
            CheckedAt = check.CheckedAt,
            Passed = check.Passed,
            ReferenceDocument = check.ReferenceDocument,
            Notes = check.Notes
        };
    }

    public async Task<IReadOnlyList<ServiceabilityCheckDto>> GetHistoryByBatchAsync(int batchId, CancellationToken cancellationToken = default)
    {
        var checks = await _checkRepository.GetByBatchIdAsync(batchId, cancellationToken);
        return checks.Select(c => new ServiceabilityCheckDto
        {
            CheckId = c.CheckId,
            BatchId = c.BatchId,
            TechnicianId = c.TechnicianId,
            CheckedAt = c.CheckedAt,
            Passed = c.Passed,
            ReferenceDocument = c.ReferenceDocument,
            Notes = c.Notes
        }).ToList();
    }
}
