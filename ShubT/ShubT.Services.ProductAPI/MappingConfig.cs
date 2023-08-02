using AutoMapper;
using ShubT.Services.ProductAPI.DTOs;
using ShubT.Services.ProductAPI.Models;

namespace ShubT.Services.ProductAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Product, ProductDTO>().ReverseMap();
        }
    }
}
