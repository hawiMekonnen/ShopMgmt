using System;
using ShopMgmt.Domain.Entities;

namespace ShopMgmt.Domain.Entities
{
    public class ServiceabilityCheck
    {
        public int CheckId { get; set; }
        public int BatchId { get; set; }
        public StockBatch Batch { get; set; } = null!;
        public int TechnicianId { get; set; }
        public User Technician { get; set; } = null!;
        public DateTime CheckedAt { get; set; }
        public bool Passed { get; set; }
        public string? ReferenceDocument { get; set; }
        public string? Notes { get; set; }
    }
}
