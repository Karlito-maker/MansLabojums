namespace KMansLabojums.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public string Deadline { get; set; } = null!;
        public int CourseId { get; set; }
    }
}
