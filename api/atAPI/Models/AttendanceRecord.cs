namespace Attendia.Models
{
    public class AttendanceRecord
    {
        public Guid RecordID { get; set; }
        public Guid SessionID { get; set; }
        public string StudentID { get; set; } = string.Empty;
        public DateTime CheckInTime { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsApproved { get; set; }
    }
}
