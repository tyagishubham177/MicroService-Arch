using AutoMapper;
using ShubT.Services.OrderAPI.DTOs;
using ShubT.Services.OrderAPI.Models;

namespace ShubT.Services.OrderAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<OrderHeaderDTO, CartHeaderDTO>()
                .ForMember(dest => dest.CartTotal, u => u.MapFrom(src => src.OrderTotal))
                .ForMember(dest => dest.DiscountTotal, u => u.MapFrom(src => src.Discount))
                .ReverseMap();

            CreateMap<CartDetailsDTO, OrderDetailsDTO>()
                .ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.ProductDTO.Name))
                .ForMember(dest => dest.Price, u => u.MapFrom(src => src.ProductDTO.Price));

            CreateMap<OrderDetailsDTO, CartDetailsDTO>();

            CreateMap<OrderHeader, OrderHeaderDTO>().ReverseMap();
            CreateMap<OrderDetailsDTO, OrderDetails>().ReverseMap();
        }
    }
}
