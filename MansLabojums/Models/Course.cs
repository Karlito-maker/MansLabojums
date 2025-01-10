namespace MansLabojums.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int TeacherId { get; set; }
    }
}
