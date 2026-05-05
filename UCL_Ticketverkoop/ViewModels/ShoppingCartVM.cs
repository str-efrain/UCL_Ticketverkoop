namespace UCL_Ticketverkoop.ViewModels
{
    public class ShoppingCartVM
    {
        public List<TicketVM> Tickets { get; set; } = new List<TicketVM>();
        public List<SubscriptionVM> Subscriptions { get; set; }
        public double TotalPrice { get; set; }

    }
}
