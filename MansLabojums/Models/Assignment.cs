using Microsoft.EntityFrameworkCore;
using SQLite;

namespace MansLabojums.Models
{
    public class Assignment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CourseId { get; set; }
    }
}

