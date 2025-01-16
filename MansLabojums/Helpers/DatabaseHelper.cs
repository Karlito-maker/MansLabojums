/******************************************************
 * MansLabojums/Helpers/DatabaseHelper.cs
 * PILNĪBĀ UZLABOTA UN PILNA VERSIJA
 ******************************************************/
using Microsoft.Data.Sqlite;
using MansLabojums.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MansLabojums.Helpers
{
    public static class DatabaseHelper
    {
        private static readonly string ConnectionString = ConfigHelper.GetConnectionString();

        // ------------------- 1) INITIALIZATION & SEED -------------------
        public static void InitializeDatabase()
        {
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();

                // Teachers tabula
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Teachers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Surname TEXT NOT NULL,
                    Gender TEXT NOT NULL,
                    ContractDate TEXT NOT NULL
                );";
                command.ExecuteNonQuery();

                // Students tabula
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Students (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Surname TEXT NOT NULL,
                    Gender TEXT NOT NULL,
                    StudentIdNumber INTEGER NOT NULL
                );";
                command.ExecuteNonQuery();

                // Courses tabula
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Courses (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    TeacherId INTEGER NOT NULL,
                    FOREIGN KEY (TeacherId) REFERENCES Teachers (Id)
                );";
                command.ExecuteNonQuery();

                // Assignments tabula
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Assignments (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Description TEXT NOT NULL,
                    Deadline TEXT NOT NULL,
                    CourseId INTEGER NOT NULL,
                    FOREIGN KEY (CourseId) REFERENCES Courses (Id)
                );";
                command.ExecuteNonQuery();

                // Submissions tabula
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Submissions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    AssignmentId INTEGER NOT NULL,
                    StudentId INTEGER NOT NULL,
                    SubmissionTime TEXT NOT NULL,
                    Score INTEGER NOT NULL,
                    FOREIGN KEY (AssignmentId) REFERENCES Assignments (Id),
                    FOREIGN KEY (StudentId) REFERENCES Students (Id)
                );";
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Neizdevās inicializēt datubāzi.", ex);
            }
        }

        public static void SeedData()
        {
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();

                // Teachers
                command.CommandText = @"
                INSERT OR IGNORE INTO Teachers (Id, Name, Surname, Gender, ContractDate)
                VALUES 
                  (1, 'Jānis', 'Bērziņš', 'Male', '2022-01-01'),
                  (2, 'Anna', 'Kalniņa', 'Female', '2023-05-01');
                ";
                command.ExecuteNonQuery();

                // Students
                command.CommandText = @"
                INSERT OR IGNORE INTO Students (Id, Name, Surname, Gender, StudentIdNumber)
                VALUES
                  (1, 'Pēteris', 'Ozoliņš', 'Male', 12345),
                  (2, 'Ilze', 'Liepa', 'Female', 54321);
                ";
                command.ExecuteNonQuery();

                // Courses
                command.CommandText = @"
                INSERT OR IGNORE INTO Courses (Id, Name, TeacherId)
                VALUES
                  (1, 'Matemātika', 1),
                  (2, 'Fizika', 2);
                ";
                command.ExecuteNonQuery();

                // Assignments
                command.CommandText = @"
                INSERT OR IGNORE INTO Assignments (Id, Description, Deadline, CourseId)
                VALUES
                  (1, 'Algebras mājas darbs', '2024-12-31', 1),
                  (2, 'Kustības vienādojumi', '2024-10-10', 2);
                ";
                command.ExecuteNonQuery();

                // Submissions
                command.CommandText = @"
                INSERT OR IGNORE INTO Submissions (Id, AssignmentId, StudentId, SubmissionTime, Score)
                VALUES
                  (1, 1, 1, '2024-09-01 10:00', 95),
                  (2, 2, 2, '2024-09-02 12:15', 88);
                ";
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Neizdevās pievienot testa datus.", ex);
            }
        }

        // ------------------- 2) TEACHERS CRUD -------------------
        public static List<Teacher> GetTeachers()
        {
            var list = new List<Teacher>();
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Name, Surname, Gender, ContractDate FROM Teachers;";
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string dateStr = reader.GetString(4);
                    var date = DateTime.Parse(dateStr);

                    list.Add(new Teacher
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Surname = reader.GetString(2),
                        Gender = reader.GetString(3),
                        ContractDate = date
                    });
                }
            }
            catch { }
            return list;
        }

        public static void AddTeacher(string name, string surname, string gender, string contractDate)
        {
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Teachers (Name, Surname, Gender, ContractDate)
                    VALUES ($n, $s, $g, $d);
                ";
                command.Parameters.AddWithValue("$n", name);
                command.Parameters.AddWithValue("$s", surname);
                command.Parameters.AddWithValue("$g", gender);
                command.Parameters.AddWithValue("$d", contractDate);
                command.ExecuteNonQuery();
            }
            catch { }
        }

        public static void UpdateTeacher(int id, string name, string surname, string gender, string contractDate)
        {
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE Teachers
                    SET Name=$n, Surname=$s, Gender=$g, ContractDate=$d
                    WHERE Id=$id;
                ";
                command.Parameters.AddWithValue("$n", name);
                command.Parameters.AddWithValue("$s", surname);
                command.Parameters.AddWithValue("$g", gender);
                command.Parameters.AddWithValue("$d", contractDate);
                command.Parameters.AddWithValue("$id", id);
                command.ExecuteNonQuery();
            }
            catch { }
        }

        public static void DeleteTeacher(int id)
        {
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Teachers WHERE Id=$id;";
                command.Parameters.AddWithValue("$id", id);
                command.ExecuteNonQuery();
            }
            catch { }
        }

        /// <summary>
        /// Palīgmetode: lai pie TeachersPage var redzēt, kādus kursus šis pasniedzējs pasniedz
        /// </summary>
        public static List<string> GetCoursesByTeacherId(int teacherId)
        {
            var result = new List<string>();
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT Name FROM Courses
                    WHERE TeacherId=$tid;
                ";
                command.Parameters.AddWithValue("$tid", teacherId);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(reader.GetString(0));
                }
            }
            catch { }

            return result;
        }

        // ------------------- 3) STUDENTS CRUD -------------------
        public static List<Student> GetStudents()
        {
            var list = new List<Student>();
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT Id, Name, Surname, Gender, StudentIdNumber 
                    FROM Students;
                ";
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new Student
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Surname = reader.GetString(2),
                        Gender = reader.GetString(3),
                        StudentIdNumber = reader.GetInt32(4)
                    });
                }
            }
            catch { }
            return list;
        }

        public static void AddStudent(string name, string surname, string gender, int studentIdNumber)
        {
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Students (Name, Surname, Gender, StudentIdNumber)
                    VALUES ($n, $s, $g, $sidNum);
                ";
                command.Parameters.AddWithValue("$n", name);
                command.Parameters.AddWithValue("$s", surname);
                command.Parameters.AddWithValue("$g", gender);
                command.Parameters.AddWithValue("$sidNum", studentIdNumber);
                command.ExecuteNonQuery();
            }
            catch { }
        }

        public static void UpdateStudent(int id, string name, string surname)
        {
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE Students
                    SET Name=$n, Surname=$s
                    WHERE Id=$id;
                ";
                command.Parameters.AddWithValue("$n", name);
                command.Parameters.AddWithValue("$s", surname);
                command.Parameters.AddWithValue("$id", id);
                command.ExecuteNonQuery();
            }
            catch { }
        }

        public static void DeleteStudent(int id)
        {
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Students WHERE Id=$id;";
                command.Parameters.AddWithValue("$id", id);
                command.ExecuteNonQuery();
            }
            catch { }
        }

        // ------------------- 4) COURSES CRUD -------------------
        public static List<Dictionary<string, object>> GetCourses()
        {
            var results = new List<Dictionary<string, object>>();
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Name, TeacherId FROM Courses;";
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var dict = new Dictionary<string, object>();
                    dict["Id"] = reader.GetInt32(0);
                    dict["Name"] = reader.GetString(1);
                    dict["TeacherId"] = reader.GetInt32(2);
                    results.Add(dict);
                }
            }
            catch { }
            return results;
        }

        public static void AddCourse(string name, int teacherId)
        {
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Courses (Name, TeacherId)
                    VALUES ($cn, $tid);
                ";
                command.Parameters.AddWithValue("$cn", name);
                command.Parameters.AddWithValue("$tid", teacherId);
                command.ExecuteNonQuery();
            }
            catch { }
        }

        public static void UpdateCourse(int courseId, string courseName, int teacherId)
        {
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE Courses
                    SET Name=$cn, TeacherId=$tid
                    WHERE Id=$id;
                ";
                command.Parameters.AddWithValue("$cn", courseName);
                command.Parameters.AddWithValue("$tid", teacherId);
                command.Parameters.AddWithValue("$id", courseId);
                command.ExecuteNonQuery();
            }
            catch { }
        }

        public static void DeleteCourse(int courseId)
        {
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Courses WHERE Id=$id;";
                command.Parameters.AddWithValue("$id", courseId);
                command.ExecuteNonQuery();
            }
            catch { }
        }

        /// <summary>
        /// Lai CoursesPage var redzēt TeacherName pie katra kursa
        /// </summary>
        public static List<Dictionary<string, object>> GetCoursesWithTeacherName()
        {
            var list = new List<Dictionary<string, object>>();
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT c.Id,
                           c.Name AS CourseName,
                           (t.Name || ' ' || t.Surname) AS TeacherName
                    FROM Courses c
                    JOIN Teachers t ON c.TeacherId = t.Id;
                ";
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var dict = new Dictionary<string, object>();
                    dict["Id"] = reader.GetInt32(0);
                    dict["CourseName"] = reader.GetString(1);
                    dict["TeacherName"] = reader.GetString(2);
                    list.Add(dict);
                }
            }
            catch { }
            return list;
        }

        // ------------------- 5) ASSIGNMENTS CRUD -------------------
        public static List<Assignment> GetAssignments()
        {
            var list = new List<Assignment>();
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Description, Deadline, CourseId FROM Assignments;";
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var dateStr = reader.GetString(2);
                    list.Add(new Assignment
                    {
                        Id = reader.GetInt32(0),
                        Description = reader.GetString(1),
                        Deadline = DateTime.Parse(dateStr),
                        CourseId = reader.GetInt32(3)
                    });
                }
            }
            catch { }
            return list;
        }

        public static void AddAssignment(string description, DateTime deadline, int courseId)
        {
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Assignments (Description, Deadline, CourseId)
                    VALUES ($desc, $dl, $cid);
                ";
                command.Parameters.AddWithValue("$desc", description);
                command.Parameters.AddWithValue("$dl", deadline.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("$cid", courseId);
                command.ExecuteNonQuery();
            }
            catch { }
        }

        public static void UpdateAssignment(int id, string description, DateTime deadline, int courseId)
        {
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE Assignments
                    SET Description=$desc, Deadline=$dl, CourseId=$cid
                    WHERE Id=$id;
                ";
                command.Parameters.AddWithValue("$desc", description);
                command.Parameters.AddWithValue("$dl", deadline.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("$cid", courseId);
                command.Parameters.AddWithValue("$id", id);
                command.ExecuteNonQuery();
            }
            catch { }
        }

        public static void DeleteAssignment(int id)
        {
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Assignments WHERE Id=$id;";
                command.Parameters.AddWithValue("$id", id);
                command.ExecuteNonQuery();
            }
            catch { }
        }

        // ------------------- 6) SUBMISSIONS CRUD -------------------
        /// <summary>
        /// Vienkāršāka versija, kas atgriež: Id, AssignmentDescription, StudentName, SubmissionTime, Score
        /// </summary>
        public static List<Dictionary<string, object>> GetSubmissions()
        {
            var list = new List<Dictionary<string, object>>();
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT s.Id,
                           a.Description AS AssignmentDescription,
                           st.Name AS StudentName,
                           s.SubmissionTime,
                           s.Score
                    FROM Submissions s
                    INNER JOIN Assignments a ON s.AssignmentId = a.Id
                    INNER JOIN Students st ON s.StudentId = st.Id;
                ";
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var dict = new Dictionary<string, object>();
                    dict["Id"] = reader.GetInt32(0);
                    dict["AssignmentDescription"] = reader.GetString(1);
                    dict["StudentName"] = reader.GetString(2);
                    dict["SubmissionTime"] = reader.GetString(3);
                    dict["Score"] = reader.GetInt32(4);
                    list.Add(dict);
                }
            }
            catch { }
            return list;
        }

        /// <summary>
        /// Plašāka versija, lai varam SubmissionsPage rādīt/atlasīt ar ID
        /// 
        /// (Id, AssignmentId, StudentId, AssignmentDescription, StudentName, SubmissionTime, Score)
        /// </summary>
        public static List<Dictionary<string, object>> GetSubmissionsWithIDs()
        {
            var list = new List<Dictionary<string, object>>();
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT s.Id,
                           s.AssignmentId,
                           s.StudentId,
                           a.Description AS AssignmentDescription,
                           st.Name AS StudentName,
                           s.SubmissionTime,
                           s.Score
                    FROM Submissions s
                    INNER JOIN Assignments a ON s.AssignmentId = a.Id
                    INNER JOIN Students st ON s.StudentId = st.Id;
                ";
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var dict = new Dictionary<string, object>();
                    dict["Id"] = reader.GetInt32(0);
                    dict["AssignmentId"] = reader.GetInt32(1);
                    dict["StudentId"] = reader.GetInt32(2);
                    dict["AssignmentDescription"] = reader.GetString(3);
                    dict["StudentName"] = reader.GetString(4);
                    dict["SubmissionTime"] = reader.GetString(5);
                    dict["Score"] = reader.GetInt32(6);
                    list.Add(dict);
                }
            }
            catch { }
            return list;
        }

        /// <summary>
        /// Pievienot iesniegumu, meklējot assignment un studentu pēc Name / Description
        /// (kā tika rādīts SubmissionsPage)
        /// </summary>
        public static void AddSubmission(string assignmentDescription, string studentName, int score)
        {
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Submissions (AssignmentId, StudentId, SubmissionTime, Score)
                    VALUES (
                        (SELECT Id FROM Assignments WHERE Description=$ad),
                        (SELECT Id FROM Students WHERE Name=$sn),
                        $tNow,
                        $sc
                    );
                ";
                command.Parameters.AddWithValue("$ad", assignmentDescription);
                command.Parameters.AddWithValue("$sn", studentName);
                command.Parameters.AddWithValue("$tNow", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                command.Parameters.AddWithValue("$sc", score);

                command.ExecuteNonQuery();
            }
            catch { }
        }

        public static void UpdateSubmission(int submissionId, int newScore)
        {
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE Submissions
                    SET Score=$scr
                    WHERE Id=$id;
                ";
                command.Parameters.AddWithValue("$scr", newScore);
                command.Parameters.AddWithValue("$id", submissionId);
                command.ExecuteNonQuery();
            }
            catch { }
        }

        public static void DeleteSubmission(int submissionId)
        {
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Submissions WHERE Id=$id;";
                command.Parameters.AddWithValue("$id", submissionId);
                command.ExecuteNonQuery();
            }
            catch { }
        }
    }
}
