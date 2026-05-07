using Microsoft.AspNetCore.Mvc.Rendering;

namespace UCL_Ticketverkoop.ViewModels
{
    public class HotelSearchVM
    {
        public string? SelectedCity { get; set; }
        public List<SelectListItem> Cities { get; set; } = new();
        public List<HotelRecommendationVM> Hotels { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public bool HasSearched { get; set; }
    }

    public class HotelRecommendationVM
    {
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Stars { get; set; }
        public string? PhotoUrl { get; set; }
        public string BookingUrl { get; set; } = string.Empty;
    }
}
