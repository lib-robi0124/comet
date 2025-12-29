using AutoMapper;
using Comet.Domain.Entities;
using Comet.ViewModels.Models;

namespace Comet.Services.AutoMapper
{
    public class BidMappingProfile : Profile
    {
        public BidMappingProfile()
        {
            // Bid to DTO
            CreateMap<Bid, BidVM>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.BuyerUser.CompanyName))
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.Product.ProductCode));

            //CreateMap<Bid, BidDetailVM>()
            //    .IncludeBase<Bid, BidVM>();

            //// DTO to Bid
            //CreateMap<CreateBidVM, Bid>();
            //CreateMap<UpdateBidVM, Bid>();
        }
    }
}
