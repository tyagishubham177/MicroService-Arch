using AutoMapper;
using ShubT.Services.CouponsAPI.DTOs;
using ShubT.Services.CouponsAPI.Models;

namespace ShubT.Services.CouponsAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Coupon, CouponDTO>().ReverseMap();
        }
    }
}
