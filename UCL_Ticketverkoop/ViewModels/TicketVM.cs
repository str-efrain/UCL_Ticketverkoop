namespace UCL_Ticketverkoop.ViewModels
{
    public class TicketVM
    {
        public int Id { get; set; }
        public MatchVM Match { get; set; }

        public VakVM Vak { get; set; }

        public string Ring { get; set; }
        public string Seat { get; set; }
        public string? Status { get; set; }
    }
}
