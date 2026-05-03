using Domains.Entities;

namespace UCL_Ticketverkoop.ViewModels
{
    public class VakVM
    {
        public int Id { get; set; }
        public string StadiumName { get; set; } = null!;

        public string Name { get; set; } = null!;

        public int Rows { get; set; }
        public int SeatsPerRow { get; set; }

        public string? Ring { get; set; }

        public double Price { get; set; }
    }
}

