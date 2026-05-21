using System;
namespace ShopMgmt.Application.DTOS;
public class ResolveAlertDto
{
    public string ResolvedNote { get; set; } = string.Empty;
    public int ResolvedBy { get; set; }
}
