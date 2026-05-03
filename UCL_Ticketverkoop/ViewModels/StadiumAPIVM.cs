namespace UCL_Ticketverkoop.ViewModels
{
    public class StadiumAPIVM
    {
        public string name { get; set; } = null!;

        public string? city { get; set; }

        public virtual List<ClubAPIVM> clubs { get; set; } = new List<ClubAPIVM>();

        public List<VakVM> vaks { get; set; } = new List<VakVM>();
    }
}
