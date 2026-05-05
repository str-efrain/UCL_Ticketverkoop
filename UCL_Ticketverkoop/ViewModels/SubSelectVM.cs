namespace UCL_Ticketverkoop.ViewModels
{
    public class SubSelectVM
    {
       public ClubVM Club { get; set; }
        public VakVM Vak { get; set; }
        public HashSet<string> TakenSeats { get; set; } = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
    }
}
