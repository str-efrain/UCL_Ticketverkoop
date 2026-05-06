using Domains.Entities;

namespace UCL_Ticketverkoop.ViewModels
{
    public class SubscriptionVM
    {
        public int Id { get; set; }
        public ClubVM Club { get; set; }

        public VakVM Vak { get; set; }

        public string Seat { get; set; } = null!;

        public int SeizoenId { get; set; }
        public string SeizoenName { get; set; }

    }
}
