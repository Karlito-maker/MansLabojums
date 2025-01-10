namespace MansLabojums.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public int StudentIdNumber { get; set; }
    }
}

