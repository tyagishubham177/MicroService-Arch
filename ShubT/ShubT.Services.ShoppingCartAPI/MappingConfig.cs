using AutoMapper;
using ShubT.Services.ShoppingCartAPI.DTOs;
using ShubT.Services.ShoppingCartAPI.Models;

namespace ShubT.Services.ShoppingCartAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<CartHeader, CartHeaderDTO>().ReverseMap();
            CreateMap<CartDetails, CartDetailsDTO>().ReverseMap();
        }
    }
}
