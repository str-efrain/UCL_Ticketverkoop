namespace UCL_Ticketverkoop.ViewModels
{
    public class TicketVM
    {
        public MatchVM Match { get; set; }

        public VakVM Vak { get; set; }
        public int Seat { get; set; }
        public string? Status { get; set; }
    }
}
