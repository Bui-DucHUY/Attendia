namespace Attendia.Models
{
    public class Classroom
    {
        public string ClassCRN { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string? ClassDescription { get; set; }
        public int InstructorID { get; set; }
    }
}