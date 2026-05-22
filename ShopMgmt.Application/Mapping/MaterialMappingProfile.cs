using AutoMapper;
using ShopMgmt.Application.DTOs;
using ShopMgmt.Application.Models;
using ShopMgmt.Domain.Entities;

namespace ShopMgmt.Application.Mapping;

public class MaterialMappingProfile : Profile
{
    public MaterialMappingProfile()
    {
        CreateMap<Category, CategoryDto>()
            .ForMember(d => d.MaterialCount, opt => opt.Ignore());

        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>();

        CreateMap<MaterialListRow, MaterialListItemDto>()
            .ForMember(d => d.MaterialId, opt => opt.MapFrom(s => s.Material.MaterialId))
            .ForMember(d => d.PartNumber, opt => opt.MapFrom(s => s.Material.PartNumber))
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Material.Name))
            .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Material.Description))
            .ForMember(d => d.AircraftTypes, opt => opt.MapFrom(s => s.Material.AircraftTypes))
            .ForMember(d => d.CategoryId, opt => opt.MapFrom(s => s.Material.CategoryId))
            .ForMember(d => d.Unit, opt => opt.MapFrom(s => s.Material.Unit))
            .ForMember(d => d.UnitPrice, opt => opt.MapFrom(s => s.Material.UnitPrice))
            .ForMember(d => d.MinStock, opt => opt.MapFrom(s => s.Material.MinStock))
            .ForMember(d => d.DefaultShopId, opt => opt.MapFrom(s => s.Material.DefaultShopId))
            .ForMember(d => d.ReorderPlaced, opt => opt.MapFrom(s => s.Material.ReorderPlaced))
            .ForMember(d => d.ReorderNote, opt => opt.MapFrom(s => s.Material.ReorderNote));

        CreateMap<Material, MaterialDetailDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.Name))
            .ForMember(d => d.OnHand, opt => opt.Ignore())
            .ForMember(d => d.Blocked, opt => opt.Ignore())
            .ForMember(d => d.Reserved, opt => opt.Ignore())
            .ForMember(d => d.Available, opt => opt.Ignore())
            .ForMember(d => d.StockValue, opt => opt.Ignore())
            .ForMember(d => d.RecentBatches, opt => opt.Ignore());

        CreateMap<CreateMaterialDto, Material>()
            .ForMember(m => m.CreatedAt, opt => opt.Ignore())
            .ForMember(m => m.ReorderPlaced, opt => opt.Ignore())
            .ForMember(m => m.ReorderNote, opt => opt.Ignore());

        CreateMap<MaterialInventorySnapshot, MaterialInventoryDto>();

        CreateMap<StockBatch, StockBatchDto>();
        CreateMap<CreateStockBatchDto, StockBatch>()
            .ForMember(b => b.MaterialId, opt => opt.Ignore())
            .ForMember(b => b.BatchId, opt => opt.Ignore())
            .ForMember(b => b.Status, opt => opt.Ignore());
    }
}
