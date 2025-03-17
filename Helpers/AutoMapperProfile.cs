using AutoMapper;
using Farma_api.Dto.Batch;
using Farma_api.Dto.Category;
using Farma_api.Dto.Grn;
using Farma_api.Dto.Product;
using Farma_api.Dto.Profile;
using Farma_api.Dto.Reports;
using Farma_api.Dto.Sales;
using Farma_api.Dto.User;
using Farma_api.Models;
using Batch_BatchCreateDto = Farma_api.Dto.Batch.BatchCreateDto;
using BatchCreateDto = Farma_api.Dto.Batch.BatchCreateDto;

namespace Farma_api.Helpers;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Categoria, CategoryDto>();
        CreateMap<Categoria, CategorySelectDto>();
        CreateMap<CategoryCreateDto, Categoria>();
        CreateMap<CategoryUpdateDto, Categoria>();

        CreateMap<Producto, ProductDto>()
            .ForMember(dest => dest.CategoriaNombre,
                opt => opt.MapFrom(src => src.Categoria!.Descripcion))
            .ForMember(dest => dest.StockTotal,
                opt => opt.MapFrom(src => src.Lotes.Sum(l => l.StockLote)));
        CreateMap<ProductCreateDto, Producto>();
        CreateMap<ProductBatchCreateDto, Lote>();
        CreateMap<ProductUpdateDto, Producto>();
        CreateMap<Producto, ProductSaleDto>()
            .ForMember(dest => dest.StockDisponible,
                opt => opt.MapFrom(src => src.Lotes.Sum(l => l.StockLote)));
        CreateMap<Producto, ProductWithBatchDto>()
            .ForMember(dest => dest.CategoriaNombre,
                opt => opt.MapFrom(src => src.Categoria!.Descripcion))
            .ForMember(dest => dest.StockTotal,
                opt => opt.MapFrom(src => src.Lotes.Sum(l => l.StockLote)));

        CreateMap<Producto, ProductGrnDto>();
        CreateMap<Lote, BatchDto>();
        CreateMap<Lote, BatchGrnDto>();
        CreateMap<Batch_BatchCreateDto, Lote>();
        CreateMap<BatchUpdateDto, Lote>();

        CreateMap<Usuario, UserDto>()
            .ForMember(dest => dest.RolNombre,
                opt => opt.MapFrom(src => src.Rol.Valor));
        CreateMap<UserCreateDto, Usuario>();
        CreateMap<UserUpdateDto, Usuario>();
        CreateMap<Usuario, ProfileDto>()
            .ForMember(dest => dest.RolNombre,
                opt => opt.MapFrom(src => src.Rol.Valor));

        CreateMap<SaleCreateDto, Venta>();
        CreateMap<Venta, SaleDto>()
            .ForMember(dest => dest.UsuarioNombre, opt => opt.MapFrom(src => src.Usuario.Nombre))
            .ForMember(dest => dest.FechaRegistro,
                opt => opt.MapFrom(src =>
                    new UtcToTimeZoneResolver<Venta, SaleDto?>("SA Pacific Standard Time").Resolve(src, null,
                        src.FechaRegistro, null)));

        CreateMap<Venta, SaleHistoryDto>().ForMember(dest => dest.FechaRegistro,
            opt => opt.MapFrom(src =>
                new UtcToTimeZoneResolver<Venta, SaleHistoryDto?>("SA Pacific Standard Time").Resolve(src, null,
                    src.FechaRegistro, null)));
        CreateMap<SaleDetailCreateDto, DetalleVenta>();
        CreateMap<DetalleVenta, SaleDetailDto>().ReverseMap();

        CreateMap<GrnCreateDto, Entrada>();
        CreateMap<GrnDetailCreateDto, DetalleEntrada>();
        CreateMap<Entrada, GrnDto>();
        CreateMap<DetalleEntrada, GrnDetailDto>();
        CreateMap<Entrada, GrnHistoryDto>().ForMember(dest => dest.FechaRegistro,
            opt => opt.MapFrom(src =>
                new UtcToTimeZoneResolver<Entrada, GrnHistoryDto?>("SA Pacific Standard Time").Resolve(src, null,
                    src.FechaRegistro, null)));

        CreateMap<DetalleVenta, SalesReportDto>().ForMember(dest => dest.FechaRegistro,
                opt => opt.MapFrom(src =>
                    new UtcToTimeZoneResolver<DetalleVenta, SalesReportDto?>("SA Pacific Standard Time").Resolve(
                        src, null, src.Venta.FechaRegistro, null)))
            .ForMember(dest => dest.Correlativo,
                opt => opt.MapFrom(src => src.Venta.Correlativo))
            .ForMember(dest => dest.Documento,
                opt => opt.MapFrom(src => src.Venta.Documento))
            .ForMember(dest => dest.Estado,
                opt => opt.MapFrom(src => src.Venta.Estado))
            .ForMember(dest => dest.ClienteNombre,
                opt => opt.MapFrom(src => src.Venta.ClienteNombre))
            .ForMember(dest => dest.ClienteDni,
                opt => opt.MapFrom(src => src.Venta.ClienteDni))
            .ForMember(dest => dest.SubTotal,
                opt => opt.MapFrom(src => src.Total));
        CreateMap<DetalleEntrada, GrnReportDto>().ForMember(dest => dest.FechaRegistro,
                opt => opt.MapFrom(src => src.Entrada.FechaRegistro))
            .ForMember(dest => dest.Correlativo,
                opt => opt.MapFrom(src => src.Entrada.Correlativo))
            .ForMember(dest => dest.Documento,
                opt => opt.MapFrom(src => src.Entrada.Documento))
            .ForMember(dest => dest.Proveedor,
                opt => opt.MapFrom(src => src.Entrada.Proveedor))
            .ForMember(dest => dest.SubTotal,
                opt => opt.MapFrom(src => src.Total))
            .ForMember(dest => dest.Estado,
                opt => opt.MapFrom(src => src.Entrada.Estado));
    }
}