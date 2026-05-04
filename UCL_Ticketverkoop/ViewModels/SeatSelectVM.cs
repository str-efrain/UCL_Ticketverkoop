using System.Collections.Generic;

namespace UCL_Ticketverkoop.ViewModels
{
    public class SeatSelectVM
    {
        public MatchVM match { get; set; }
        public int VakId { get; set; }
        public HashSet<string> TakenSeats { get; set; } = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
    }
}
