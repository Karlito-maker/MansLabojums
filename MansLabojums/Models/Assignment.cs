namespace MansLabojums.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Deadline { get; set; }
        public int CourseId { get; set; }
    }
}
