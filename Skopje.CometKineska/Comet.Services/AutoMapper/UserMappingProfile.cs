using AutoMapper;
using Comet.Domain.Entities;
using Comet.ViewModels.ModelsUser;

namespace Comet.Services.AutoMapper
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<BuyerUser, BuyerUserVM>();
            CreateMap<LibertyUser, LibertyUserVM>();

            CreateMap<CreateBuyerUserVM, BuyerUser>();
            CreateMap<CreateLibertyUserVM, LibertyUser>();
        }
    }
}
