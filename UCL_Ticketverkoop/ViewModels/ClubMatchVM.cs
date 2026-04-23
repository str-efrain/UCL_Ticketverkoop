using Microsoft.AspNetCore.Mvc.Rendering;

namespace UCL_Ticketverkoop.ViewModels
{
    public class ClubMatchVM
    {
        public int? ClubID { get; set; }
        public IEnumerable<MatchVM> Matches { get; set; } = new List<MatchVM>();
        public IEnumerable<SelectListItem>? Clubs { get; set; }
    }
}
