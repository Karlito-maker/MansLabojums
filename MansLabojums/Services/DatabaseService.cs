using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using MansLabojums.Models;

namespace MansLabojums.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Assignment>().Wait();
            _database.CreateTableAsync<Teacher>().Wait();
            _database.CreateTableAsync<Student>().Wait();
            _database.CreateTableAsync<Course>().Wait();
            _database.CreateTableAsync<Submission>().Wait();
        }

        // --- Assignments ---
        public Task<List<Assignment>> GetAssignmentsAsync()
        {
            return _database.Table<Assignment>().ToListAsync();
        }

        public Task<int> SaveAssignmentAsync(Assignment assignment)
        {
            if (assignment.Id != 0)
            {
                return _database.UpdateAsync(assignment);
            }
            else
            {
                return _database.InsertAsync(assignment);
            }
        }

        public Task<int> DeleteAssignmentAsync(Assignment assignment)
        {
            return _database.DeleteAsync(assignment);
        }

        // --- Teachers ---
        public Task<List<Teacher>> GetTeachersAsync()
        {
            return _database.Table<Teacher>().ToListAsync();
        }

        public Task<int> SaveTeacherAsync(Teacher teacher)
        {
            if (teacher.Id != 0)
            {
                return _database.UpdateAsync(teacher);
            }
            else
            {
                return _database.InsertAsync(teacher);
            }
        }

        public Task<int> DeleteTeacherAsync(Teacher teacher)
        {
            return _database.DeleteAsync(teacher);
        }

        // --- Students ---
        public Task<List<Student>> GetStudentsAsync()
        {
            return _database.Table<Student>().ToListAsync();
        }

        public Task<int> SaveStudentAsync(Student student)
        {
            if (student.Id != 0)
            {
                return _database.UpdateAsync(student);
            }
            else
            {
                return _database.InsertAsync(student);
            }
        }

        public Task<int> DeleteStudentAsync(Student student)
        {
            return _database.DeleteAsync(student);
        }

        // --- Courses ---
        public Task<List<Course>> GetCoursesAsync()
        {
            return _database.Table<Course>().ToListAsync();
        }

        public Task<int> SaveCourseAsync(Course course)
        {
            if (course.Id != 0)
            {
                return _database.UpdateAsync(course);
            }
            else
            {
                return _database.InsertAsync(course);
            }
        }

        public Task<int> DeleteCourseAsync(Course course)
        {
            return _database.DeleteAsync(course);
        }

        // --- Submissions ---
        public Task<List<Submission>> GetSubmissionsAsync()
        {
            return _database.Table<Submission>().ToListAsync();
        }

        public Task<int> SaveSubmissionAsync(Submission submission)
        {
            if (submission.Id != 0)
            {
                return _database.UpdateAsync(submission);
            }
            else
            {
                return _database.InsertAsync(submission);
            }
        }

        public Task<int> DeleteSubmissionAsync(Submission submission)
        {
            return _database.DeleteAsync(submission);
        }
    }
}
