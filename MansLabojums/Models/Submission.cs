namespace MansLabojums3.Models
{
    public class Submission
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public int StudentId { get; set; }
        public string SubmissionTime { get; set; } = null!;
        public int Score { get; set; }
    }
}
