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
  
                
            CreateMap<Club, ClubAPIVM>();

            CreateMap<Stadium, StadiumVM>()
                .ForMember(dest => dest.clubs, opt => opt.MapFrom(src => src.Clubs))
                .ForMember(dest => dest.vaks, opt => opt.MapFrom(src => src.Vaks));

            CreateMap<Stadium, StadiumAPIVM>()
                .ForMember(dest => dest.clubs, opt => opt.MapFrom(src => src.Clubs))
                .ForMember(dest => dest.vaks, opt => opt.MapFrom(src => src.Vaks));

            CreateMap<Vak, VakVM>()
                .ForMember(dest => dest.StadiumName, opt => opt.MapFrom(src => src.Stadium.Name));
            
            CreateMap<Club, ClubVM>()
            .ForMember(dest => dest.Stadium, opt => opt.MapFrom(src => src.Stadium));


            CreateMap<Match, MatchVM>()
                .ForMember(dest => dest.HomeTeamName, opt => opt.MapFrom(src => src.HomeClub.Name))
                .ForMember(dest => dest.HomeTeamLogoUrl, opt => opt.MapFrom(src => src.HomeClub.LogoUrl))
                .ForMember(dest => dest.AwayTeamName, opt => opt.MapFrom(src => src.AwayClub.Name))
                .ForMember(dest => dest.AwayTeamLogoUrl, opt => opt.MapFrom(src => src.AwayClub.LogoUrl))
                .ForMember(dest => dest.StadiumName, opt => opt.MapFrom(src => src.HomeClub.Stadium.Name))
                .ForMember(dest => dest.StadiumId, opt => opt.MapFrom(src => src.HomeClub.Stadium.Id))
                .ForMember(dest => dest.vaks, opt => opt.MapFrom(src => src.HomeClub.Stadium.Vaks))
                .ForMember(dest => dest.SeizoenName, opt => opt.MapFrom(src => src.Seizoen.Name))
                .ForMember(dest => dest.HomeTeamId, opt => opt.MapFrom(src => src.HomeClub.Id))
                .ForMember(dest=>dest.SeizoenId, opt => opt.MapFrom(src => src.Seizoen.Id));   


            CreateMap<Match, MatchAPIVM>()
                .ForMember(dest => dest.HomeTeamName, opt => opt.MapFrom(src => src.HomeClub.Name))
                .ForMember(dest => dest.AwayTeamName, opt => opt.MapFrom(src => src.AwayClub.Name))
                .ForMember(dest => dest.StadiumName, opt => opt.MapFrom(src => src.HomeClub.Stadium.Name))
                .ForMember(dest => dest.SeizoenName, opt => opt.MapFrom(src => src.Seizoen.Name));

            CreateMap<IdentityUser, UserVM>();
            
            CreateMap<Ticket, TicketVM>()
                .ForMember(dest => dest.Match, opt => opt.MapFrom(src => src.Match))
                .ForMember(dest => dest.Vak, opt => opt.MapFrom(src => src.Vak))
                .ForMember(dest => dest.Ring, opt => opt.MapFrom(src => src.Vak.Ring));

            CreateMap<Abonnement, SubscriptionVM>()
                .ForMember(dest => dest.Club, opt => opt.MapFrom(src => src.Club))
                .ForMember(dest => dest.Vak, opt => opt.MapFrom(src => src.Vak));

            CreateMap<Order, OrderVM>()
             .ForMember(dest => dest.Tickets, opt => opt.MapFrom(src => src.Tickets))
             .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
             .ForMember(dest => dest.Abonnements, opt => opt.MapFrom(src => src.Abonnements));




        }
    }
}
