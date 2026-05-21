using System.Collections.Generic;

namespace ShopMgmt.Application.DTOS;

public class AuditLogPagedDto
{
    public List<AuditLogDto> Items { get; set; } = new List<AuditLogDto>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
