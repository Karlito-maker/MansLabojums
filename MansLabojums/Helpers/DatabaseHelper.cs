using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MansLabojums.Models;

namespace MansLabojums.Helpers
{
    public static class DatabaseHelper
    {
        private static SQLiteAsyncConnection? database;

        public static SQLiteAsyncConnection Database
        {
            get
            {
                if (database == null)
                {
                    var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MansLabojums.db3");
                    database = new SQLiteAsyncConnection(dbPath);
                    InitializeDatabase().Wait();
                }
                return database;
            }
        }

        public static async Task InitializeDatabase()
        {
            if (database != null)
            {
                await database.CreateTableAsync<Assignment>();
                await database.CreateTableAsync<Teacher>();
                await database.CreateTableAsync<Student>();
                await database.CreateTableAsync<Course>();
                await database.CreateTableAsync<Submission>();
            }
        }

        // --- Assignments ---
        public static Task<List<Assignment>> GetAssignmentsAsync()
        {
            return Database.Table<Assignment>().ToListAsync();
        }

        public static Task<int> SaveAssignmentAsync(Assignment assignment)
        {
            if (assignment.Id != 0)
            {
                return Database.UpdateAsync(assignment);
            }
            else
            {
                return Database.InsertAsync(assignment);
            }
        }

        public static Task<int> DeleteAssignmentAsync(Assignment assignment)
        {
            return Database.DeleteAsync(assignment);
        }

        // --- Teachers ---
        public static Task<List<Teacher>> GetTeachersAsync()
        {
            return Database.Table<Teacher>().ToListAsync();
        }

        public static Task<int> SaveTeacherAsync(Teacher teacher)
        {
            if (teacher.Id != 0)
            {
                return Database.UpdateAsync(teacher);
            }
            else
            {
                return Database.InsertAsync(teacher);
            }
        }

        public static Task<int> DeleteTeacherAsync(Teacher teacher)
        {
            return Database.DeleteAsync(teacher);
        }

        // --- Students ---
        public static Task<List<Student>> GetStudentsAsync()
        {
            return Database.Table<Student>().ToListAsync();
        }

        public static Task<int> SaveStudentAsync(Student student)
        {
            if (student.Id != 0)
            {
                return Database.UpdateAsync(student);
            }
            else
            {
                return Database.InsertAsync(student);
            }
        }

        public static Task<int> DeleteStudentAsync(Student student)
        {
            return Database.DeleteAsync(student);
        }

        // --- Courses ---
        public static Task<List<Course>> GetCoursesAsync()
        {
            return Database.Table<Course>().ToListAsync();
        }

        public static Task<int> SaveCourseAsync(Course course)
        {
            if (course.Id != 0)
            {
                return Database.UpdateAsync(course);
            }
            else
            {
                return Database.InsertAsync(course);
            }
        }

        public static Task<int> DeleteCourseAsync(Course course)
        {
            return Database.DeleteAsync(course);
        }

        // --- Submissions ---
        public static Task<List<Submission>> GetSubmissionsAsync()
        {
            return Database.Table<Submission>().ToListAsync();
        }

        public static Task<int> SaveSubmissionAsync(Submission submission)
        {
            if (submission.Id != 0)
            {
                return Database.UpdateAsync(submission);
            }
            else
            {
                return Database.InsertAsync(submission);
            }
        }

        public static Task<int> DeleteSubmissionAsync(Submission submission)
        {
            return Database.DeleteAsync(submission);
        }

        // Seed Data
        public static async Task SeedDataAsync()
        {
            // Seed Teachers
            var teachers = new List<Teacher>
            {
                new Teacher { Name = "Anna", Subject = "Mathematics" },
                new Teacher { Name = "Peter", Subject = "Physics" }
            };

            foreach (var teacher in teachers)
            {
                await SaveTeacherAsync(teacher);
            }

            // Seed Students
            var students = new List<Student>
            {
                new Student { Name = "John", StudentId = "S1001" },
                new Student { Name = "Mary", StudentId = "S1002" }
            };

            foreach (var student in students)
            {
                await SaveStudentAsync(student);
            }

            // Seed Courses
            var courses = new List<Course>
            {
                new Course { CourseName = "Algebra", TeacherId = 1 },
                new Course { CourseName = "Physics", TeacherId = 2 }
            };

            foreach (var course in courses)
            {
                await SaveCourseAsync(course);
            }

            // Seed Assignments
            var assignments = new List<Assignment>
            {
                new Assignment { Name = "Assignment 1", Description = "Description 1", CourseId = 1 },
                new Assignment { Name = "Assignment 2", Description = "Description 2", CourseId = 2 }
            };

            foreach (var assignment in assignments)
            {
                await SaveAssignmentAsync(assignment);
            }

            // Seed Submissions
            var submissions = new List<Submission>
            {
                new Submission { AssignmentId = 1, StudentId = 1, Content = "Submission 1" },
                new Submission { AssignmentId = 2, StudentId = 2, Content = "Submission 2" }
            };

            foreach (var submission in submissions)
            {
                await SaveSubmissionAsync(submission);
            }
        }
    }
}




