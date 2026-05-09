using Microsoft.AspNetCore.Mvc.Rendering;

namespace UCL_Ticketverkoop.ViewModels
{
    public class ClubMatchVM
    {
        public int? ClubID { get; set; }
        public int? SeasonID { get; set; }
        public string? SelectedSeasonName { get; set; }
        public IEnumerable<MatchVM> Matches { get; set; } = new List<MatchVM>();
        public IEnumerable<SelectListItem>? Clubs { get; set; }
        public IEnumerable<SelectListItem>? Seasons { get; set; }
    }
}
