namespace UCL_Ticketverkoop.ViewModels
{
    public class MatchVM
    {
        public int Id { get; set; }
        public int HomeTeamId { get; set; }
        public string HomeTeamName { get; set; }
        public string HomeTeamLogoUrl { get; set; }
        public string AwayTeamName { get; set; }
        public string AwayTeamLogoUrl { get; set; }
        public string StadiumName { get; set; }
        public string StadiumId { get; set; }
        public int SeizoenId { get; set; }
        public string SeizoenName { get; set; }
        public List<VakVM> vaks { get; set; } = new List<VakVM>();
        public DateTime MatchDate { get; set; }
    }
}
