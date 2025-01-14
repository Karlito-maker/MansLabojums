using Microsoft.EntityFrameworkCore;
using SQLite;

namespace MansLabojums.Models
{
    public class Student
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string StudentId { get; set; }
    }
}

