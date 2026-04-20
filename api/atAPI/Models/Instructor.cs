namespace Attendia.Models
{
    public class Instructor
    {
        public int InstructorID { get; set; }
        public string InstructorMail { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public string InstructorPassword { get; set; } = string.Empty;
    }
}