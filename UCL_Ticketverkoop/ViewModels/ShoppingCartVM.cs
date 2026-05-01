namespace UCL_Ticketverkoop.ViewModels
{
    public class ShoppingCartVM
    {
        public List<TicketVM> Tickets { get; set; } = new List<TicketVM>();
        public double TotalPrice { get; set; }

    }
}
