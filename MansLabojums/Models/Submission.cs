using Microsoft.EntityFrameworkCore;
using SQLite;

namespace MansLabojums.Models
{
    public class Submission
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public int StudentId { get; set; }
        public string Content { get; set; }
    }
}
