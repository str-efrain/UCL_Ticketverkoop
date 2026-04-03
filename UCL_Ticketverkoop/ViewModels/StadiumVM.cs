using Domains.Entities;

namespace UCL_Ticketverkoop.ViewModels
{
    public class StadiumVM
    {
        public string name { get; set; } = null!;

        public string? city { get; set; }

        public virtual List<ClubVM> clubs { get; set; } = new List<ClubVM>();

        public  List<VakVM> vaks{ get; set; } = new List<VakVM>();
    }
}
