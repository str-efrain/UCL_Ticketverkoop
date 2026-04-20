using Domains.Entities;

namespace UCL_Ticketverkoop.ViewModels
{
    public class VakVM
    {
        public int id { get; set; }

        public int stadiumId { get; set; }
        public string stadiumName { get; set; } = null!;

        public string name { get; set; } = null!;

        public int capacity { get; set; }

        public string? ring { get; set; }

        public decimal price { get; set; }

        public int ticketCount { get; set; }

        public int abonnementCount { get; set; }
    }
}

