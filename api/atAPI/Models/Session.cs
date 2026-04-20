namespace Attendia.Models
{
    public class Session
    {
        public Guid SessionID { get; set; }
        public string ClassCRN { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? ExpiryTime { get; set; }
        public bool RequiresImage { get; set; }
    }
}