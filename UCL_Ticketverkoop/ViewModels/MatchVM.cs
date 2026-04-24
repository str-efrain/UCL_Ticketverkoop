using Domains.Entities;

namespace UCL_Ticketverkoop.ViewModels
{
    public class MatchVM
    {
        public int Id { get; set; }
        public Club HomeTeam { get; set; }
        public Club AwayTeam { get; set; }
        public DateTime MatchDate { get; set; }
    }
}
