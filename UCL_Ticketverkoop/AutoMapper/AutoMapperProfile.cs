using AutoMapper;
using Domains.Entities;
using Elfie.Serialization;
using UCL_Ticketverkoop.ViewModels;

namespace UCL_Ticketverkoop.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {


            CreateMap<Club, ClubVM>()

                .ForMember(dest => dest.StadiumName, opt => opt.MapFrom(src => src.Stadium != null ? src.Stadium.Name : "Geen stadion"));

                

            CreateMap<Stadium, StadiumVM>();

            CreateMap<Vak, VakVM>()
            .ForMember(dest => dest.stadiumName, opt => opt.MapFrom(src => src.Stadium.Name))
            .ForMember(dest => dest.ticketCount, opt => opt.MapFrom(src => src.Tickets.Count))
            .ForMember(dest => dest.abonnementCount, opt => opt.MapFrom(src => src.Abonnements.Count));


            //CreateMap<TSource, TDestination>();
        }
    }
}
