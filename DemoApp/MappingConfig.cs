using AutoMapper;
using DemoApp.Models;
using DemoApp.Models.DTO;

namespace DemoApp
{
    public class MappingConfig:Profile
    {
        public MappingConfig() 
        {
            CreateMap<Coupon, CouponDTO>().ReverseMap();
            CreateMap<Coupon, CreateCouponDTO>().ReverseMap();
        }  
    }
}
