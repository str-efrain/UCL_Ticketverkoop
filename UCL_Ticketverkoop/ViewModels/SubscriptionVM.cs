using Domains.Entities;

namespace UCL_Ticketverkoop.ViewModels
{
    public class SubscriptionVM
    {
        public int Id { get; set; }
        public int ClubId { get; set; }

        public int VakId { get; set; }

        public string Seat { get; set; } = null!;

        public int SeizoenId { get; set; }
    }
}
