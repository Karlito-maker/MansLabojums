using Microsoft.EntityFrameworkCore;
using SQLite;

namespace MansLabojums.Models
{
    public class Teacher
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
    }
}
