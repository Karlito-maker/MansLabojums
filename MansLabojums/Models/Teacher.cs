namespace MansLabojums.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        // Saglabāsim kā DateTime
        public DateTime ContractDate { get; set; }
    }
}