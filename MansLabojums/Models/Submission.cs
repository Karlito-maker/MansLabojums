namespace MansLabojums.Models
{
    public class Submission
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public int StudentId { get; set; }
        public DateTime SubmissionTime { get; set; }
        public int Score { get; set; }
    }
}