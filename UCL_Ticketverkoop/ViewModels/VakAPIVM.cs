namespace UCL_Ticketverkoop.ViewModels
{
    public class VakAPIVM
    {
        public string StadiumName { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int Rows { get; set; }
        public int SeatsPerRow { get; set; }
        public string? Ring { get; set; }
        public decimal Price { get; set; }
    }
}
