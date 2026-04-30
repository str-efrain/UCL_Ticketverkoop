using AutoMapper;
using Domains.Entities;
using Elfie.Serialization;
using Microsoft.AspNetCore.Identity;
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
                .ForMember(dest => dest.StadiumName, opt => opt.MapFrom(src => src.Stadium.Name))
                .ForMember(dest => dest.TicketCount, opt => opt.MapFrom(src => src.Tickets.Count))
                .ForMember(dest => dest.AbonnementCount, opt => opt.MapFrom(src => src.Abonnements.Count));

            CreateMap<Match, MatchVM>()
                .ForMember(dest => dest.HomeTeamName, opt => opt.MapFrom(src => src.HomeClub.Name))
                .ForMember(dest => dest.HomeTeamLogoUrl, opt => opt.MapFrom(src => src.HomeClub.LogoUrl))
                .ForMember(dest => dest.AwayTeamName, opt => opt.MapFrom(src => src.AwayClub.Name))
                .ForMember(dest => dest.AwayTeamLogoUrl, opt => opt.MapFrom(src => src.AwayClub.LogoUrl))
                .ForMember(dest => dest.StadiumName, opt => opt.MapFrom(src => src.HomeClub.Stadium.Name))
                .ForMember(dest => dest.StadiumId, opt => opt.MapFrom(src => src.HomeClub.Stadium.Id))
                .ForMember(dest => dest.vaks, opt => opt.MapFrom(src => src.HomeClub.Stadium.Vaks));


            CreateMap<Match, MatchAPIVM>()
                .ForMember(dest => dest.HomeTeamName, opt => opt.MapFrom(src => src.HomeClub.Name))
                .ForMember(dest => dest.AwayTeamName, opt => opt.MapFrom(src => src.AwayClub.Name))
                .ForMember(dest => dest.StadiumName, opt => opt.MapFrom(src => src.HomeClub.Stadium.Name));

            CreateMap<IdentityUser, UserVM>();
        }
    }
}
