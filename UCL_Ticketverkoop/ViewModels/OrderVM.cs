using Domains.Entities;
using Microsoft.AspNetCore.Identity;

namespace UCL_Ticketverkoop.ViewModels
{
    public class OrderVM
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public List<SubscriptionVM> Abonnements { get; set; } = new List<SubscriptionVM>();
        public UserVM User { get; set; } = new UserVM();
        public double? TotalPrice { get; set; }
        public List<TicketVM> Tickets { get; set; } = new List<TicketVM>();
    }
}
