using Microsoft.EntityFrameworkCore;
using SQLite;

namespace MansLabojums.Models
{
    public class Course
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string CourseName { get; set; }
        public int TeacherId { get; set; }
    }
}

