using AutoMapper;
using Domains.Entities;
using UCL_Ticketverkoop.ViewModels;

namespace UCL_Ticketverkoop.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {


            CreateMap<Club, ClubVM>()
                .ForMember(dest => dest.stadiumName, opt => opt.MapFrom(src => src.Stadium != null ? src.Stadium.Name : "Geen stadion"));
            //CreateMap<TSource, TDestination>();
        }
    }
}
