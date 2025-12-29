using AutoMapper;
using Comet.Domain.Entities;
using Comet.ViewModels.Models;

namespace Comet.Services.AutoMapper
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductVM>()
                .ForMember(d => d.CurrentHighestBid,
                    o => o.MapFrom(s =>
                        s.Bids.OrderByDescending(b => b.Amount)
                              .Select(b => b.Amount)
                              .FirstOrDefault()))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.LibertyUser.FullName))
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.BuyerUser.CompanyName));

            CreateMap<ProductImportVM, Product>();
            CreateMap<Product, ProductDetailsVM>()
            .IncludeBase<Product, ProductVM>()
            .ForMember(dest => dest.Bids, opt => opt.MapFrom(src => src.Bids));
        }
    }
}
