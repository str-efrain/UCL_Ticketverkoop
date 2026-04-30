namespace UCL_Ticketverkoop.ViewModels
{
    public class MatchVM
    {
        public int Id { get; set; }
        public string HomeTeamName { get; set; }
        public string HomeTeamLogoUrl { get; set; }
        public string AwayTeamName { get; set; }
        public string AwayTeamLogoUrl { get; set; }
        public string StadiumName { get; set; }
        public string StadiumId { get; set; }
        public DateTime MatchDate { get; set; }
    }
}
