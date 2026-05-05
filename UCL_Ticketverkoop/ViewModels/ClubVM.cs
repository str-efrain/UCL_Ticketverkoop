namespace UCL_Ticketverkoop.ViewModels
{
    public class ClubVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public StadiumVM Stadium { get; set; } 
    }
}
