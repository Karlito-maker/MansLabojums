using Microsoft.Data.Sqlite; // svarīgi: vajag Microsoft.Data.Sqlite NuGet pakotni
using System;
using System.Collections.Generic;
using MansLabojums.Models; // ja lietojat modeļus "Teacher", "Student" utml.

namespace MansLabojums.Helpers
{
    public static class DatabaseHelper
    {
        private static readonly string ConnectionString = ConfigHelper.GetConnectionString();

        public static void InitializeDatabase()
        {
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();

                // Teachers
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Teachers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Surname TEXT NOT NULL,
                    Gender TEXT NOT NULL,
                    ContractDate TEXT NOT NULL
                );
                ";
                command.ExecuteNonQuery();

                // Students
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Students (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Surname TEXT NOT NULL,
                    Gender TEXT NOT NULL,
                    StudentIdNumber INTEGER NOT NULL
                );
                ";
                command.ExecuteNonQuery();

                // Courses
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Courses (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    TeacherId INTEGER NOT NULL,
                    FOREIGN KEY (TeacherId) REFERENCES Teachers (Id)
                );
                ";
                command.ExecuteNonQuery();

                // Assignments
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Assignments (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Description TEXT NOT NULL,
                    Deadline TEXT NOT NULL,
                    CourseId INTEGER NOT NULL,
                    FOREIGN KEY (CourseId) REFERENCES Courses (Id)
                );
                ";
                command.ExecuteNonQuery();

                // Submissions
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Submissions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    AssignmentId INTEGER NOT NULL,
                    StudentId INTEGER NOT NULL,
                    SubmissionTime TEXT NOT NULL,
                    Score INTEGER NOT NULL,
                    FOREIGN KEY (AssignmentId) REFERENCES Assignments (Id),
                    FOREIGN KEY (StudentId) REFERENCES Students (Id)
                );
                ";
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

                // Seed Teachers
                command.CommandText = @"
                INSERT OR IGNORE INTO Teachers (Id, Name, Surname, Gender, ContractDate)
                VALUES (1, 'Jānis', 'Bērziņš', 'Male', '2022-01-01');
                ";
                command.ExecuteNonQuery();

                // Seed Students
                command.CommandText = @"
                INSERT OR IGNORE INTO Students (Id, Name, Surname, Gender, StudentIdNumber)
                VALUES (1, 'Anna', 'Kalniņa', 'Female', 12345);
                ";
                command.ExecuteNonQuery();

                // Seed Courses
                command.CommandText = @"
                INSERT OR IGNORE INTO Courses (Id, Name, TeacherId)
                VALUES (1, 'Matemātika', 1),
                       (2, 'Fizika', 1);
                ";
                command.ExecuteNonQuery();

                // Seed Assignments
                command.CommandText = @"
                INSERT OR IGNORE INTO Assignments (Id, Description, Deadline, CourseId)
                VALUES (1, 'Mājas darbs par algebru', '2024-12-31', 1);
                ";
                command.ExecuteNonQuery();

                // Seed Submissions
                command.CommandText = @"
                INSERT OR IGNORE INTO Submissions (Id, AssignmentId, StudentId, SubmissionTime, Score)
                VALUES (1, 1, 1, '2024-12-01 10:00', 95);
                ";
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Neizdevās veikt testa datu ievadi.", ex);
            }
        }

        // -------------------------------------------------
        // TEACHERS
        // -------------------------------------------------
        public static List<(int Id, string Name, string Surname, string Gender, string ContractDate)> GetTeachers()
        {
            var teachers = new List<(int, string, string, string, string)>();
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Name, Surname, Gender, ContractDate FROM Teachers;";
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    teachers.Add((
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetString(3),
                        reader.GetString(4)
                    ));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Neizdevās nolasīt pasniedzējus.", ex);
            }
            return teachers;
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
                    VALUES ($name, $surname, $gender, $contractDate);
                ";
                command.Parameters.AddWithValue("$name", name);
                command.Parameters.AddWithValue("$surname", surname);
                command.Parameters.AddWithValue("$gender", gender);
                command.Parameters.AddWithValue("$contractDate", contractDate);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Neizdevās pievienot pasniedzēju.", ex);
            }
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
                    SET Name=$name, Surname=$surname, Gender=$gender, ContractDate=$contractDate
                    WHERE Id=$id;
                ";
                command.Parameters.AddWithValue("$name", name);
                command.Parameters.AddWithValue("$surname", surname);
                command.Parameters.AddWithValue("$gender", gender);
                command.Parameters.AddWithValue("$contractDate", contractDate);
                command.Parameters.AddWithValue("$id", id);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Neizdevās atjaunot pasniedzēju.", ex);
            }
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
            catch (Exception ex)
            {
                throw new Exception("Neizdevās dzēst pasniedzēju.", ex);
            }
        }

        // -------------------------------------------------
        // STUDENTS
        // -------------------------------------------------
        public static List<(int Id, string Name, string Surname, string Gender, int StudentIdNumber)> GetStudents()
        {
            var students = new List<(int, string, string, string, int)>();
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Name, Surname, Gender, StudentIdNumber FROM Students;";
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    students.Add((
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetString(3),
                        reader.GetInt32(4)
                    ));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Neizdevās nolasīt studentus.", ex);
            }
            return students;
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
                    VALUES ($name, $surname, $gender, $studentIdNumber);
                ";
                command.Parameters.AddWithValue("$name", name);
                command.Parameters.AddWithValue("$surname", surname);
                command.Parameters.AddWithValue("$gender", gender);
                command.Parameters.AddWithValue("$studentIdNumber", studentIdNumber);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Neizdevās pievienot studentu.", ex);
            }
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
                    SET Name=$name, Surname=$surname
                    WHERE Id=$id;
                ";
                command.Parameters.AddWithValue("$name", name);
                command.Parameters.AddWithValue("$surname", surname);
                command.Parameters.AddWithValue("$id", id);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Neizdevās atjaunot studenta datus.", ex);
            }
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
            catch (Exception ex)
            {
                throw new Exception("Neizdevās dzēst studentu.", ex);
            }
        }

        // -------------------------------------------------
        // COURSES
        // -------------------------------------------------
        public static List<(int Id, string CourseName, string TeacherName)> GetCourses()
        {
            var list = new List<(int, string, string)>();
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT c.Id, c.Name AS CourseName,
                           (t.Name || ' ' || t.Surname) AS TeacherName
                    FROM Courses c
                    INNER JOIN Teachers t ON c.TeacherId = t.Id;
                ";
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    list.Add((
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetString(2)
                    ));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Neizdevās nolasīt kursus.", ex);
            }
            return list;
        }

        // JAUNAS METODES KURSU CRUD:
        public static void AddCourse(string courseName, int teacherId)
        {
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Courses (Name, TeacherId)
                    VALUES ($name, $teacherId);
                ";
                command.Parameters.AddWithValue("$name", courseName);
                command.Parameters.AddWithValue("$teacherId", teacherId);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Neizdevās pievienot kursu.", ex);
            }
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
                    SET Name = $courseName,
                        TeacherId = $teacherId
                    WHERE Id = $id;
                ";
                command.Parameters.AddWithValue("$courseName", courseName);
                command.Parameters.AddWithValue("$teacherId", teacherId);
                command.Parameters.AddWithValue("$id", courseId);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Neizdevās labot kursu.", ex);
            }
        }

        public static void DeleteCourse(int courseId)
        {
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    DELETE FROM Courses
                    WHERE Id = $id;
                ";
                command.Parameters.AddWithValue("$id", courseId);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Neizdevās dzēst kursu.", ex);
            }
        }

        // -------------------------------------------------
        // ASSIGNMENTS
        // -------------------------------------------------
        public static List<Dictionary<string, object>> GetAssignments(int? courseId = null)
        {
            var assignments = new List<Dictionary<string, object>>();
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();
                using var command = connection.CreateCommand();

                if (courseId.HasValue)
                {
                    command.CommandText = @"
                        SELECT a.Id, a.Description, a.Deadline, a.CourseId,
                               c.Name AS CourseName
                        FROM Assignments a
                        INNER JOIN Courses c ON a.CourseId = c.Id
                        WHERE a.CourseId=$courseId;
                    ";
                    command.Parameters.AddWithValue("$courseId", courseId.Value);
                }
                else
                {
                    command.CommandText = @"
                        SELECT a.Id, a.Description, a.Deadline, a.CourseId,
                               c.Name AS CourseName
                        FROM Assignments a
                        INNER JOIN Courses c ON a.CourseId = c.Id;
                    ";
                }

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var dict = new Dictionary<string, object>
                    {
                        { "Id", reader.GetInt32(0) },
                        { "Description", reader.GetString(1) },
                        { "Deadline", reader.GetString(2) },
                        { "CourseId", reader.GetInt32(3) },
                        { "CourseName", reader.GetString(4) }
                    };
                    assignments.Add(dict);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Neizdevās nolasīt uzdevumus.", ex);
            }
            return assignments;
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
                    VALUES ($description, $deadline, $courseId);
                ";
                command.Parameters.AddWithValue("$description", description);
                command.Parameters.AddWithValue("$deadline", deadline.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("$courseId", courseId);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Neizdevās pievienot uzdevumu.", ex);
            }
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
                    SET Description=$description,
                        Deadline=$deadline,
                        CourseId=$courseId
                    WHERE Id=$id;
                ";
                command.Parameters.AddWithValue("$description", description);
                command.Parameters.AddWithValue("$deadline", deadline.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("$courseId", courseId);
                command.Parameters.AddWithValue("$id", id);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Neizdevās atjaunot uzdevumu.", ex);
            }
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
            catch (Exception ex)
            {
                throw new Exception("Neizdevās dzēst uzdevumu.", ex);
            }
        }

        // -------------------------------------------------
        // SUBMISSIONS
        // -------------------------------------------------
        public static List<Dictionary<string, object>> GetSubmissions()
        {
            var submissions = new List<Dictionary<string, object>>();
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT s.Id,
                           a.Description AS AssignmentDescription,
                           (st.Name || ' ' || st.Surname) AS StudentName,
                           s.SubmissionTime,
                           s.Score
                    FROM Submissions s
                    INNER JOIN Assignments a ON s.AssignmentId = a.Id
                    INNER JOIN Students st ON s.StudentId = st.Id;
                ";
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var dict = new Dictionary<string, object>
                    {
                        { "Id", reader.GetInt32(0) },
                        { "AssignmentDescription", reader.GetString(1) },
                        { "StudentName", reader.GetString(2) },
                        { "SubmissionTime", reader.GetString(3) },
                        { "Score", reader.GetInt32(4) }
                    };
                    submissions.Add(dict);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Neizdevās nolasīt iesniegumus.", ex);
            }
            return submissions;
        }

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
                        (SELECT Id FROM Assignments WHERE Description=$assignmentDescription),
                        (SELECT Id FROM Students WHERE Name=$studentName),
                        $submissionTime,
                        $score
                    );
                ";
                command.Parameters.AddWithValue("$assignmentDescription", assignmentDescription);
                command.Parameters.AddWithValue("$studentName", studentName);
                command.Parameters.AddWithValue("$submissionTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                command.Parameters.AddWithValue("$score", score);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Neizdevās pievienot iesniegumu.", ex);
            }
        }

        public static void UpdateSubmission(int id, int score)
        {
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE Submissions
                    SET Score=$score
                    WHERE Id=$id;
                ";
                command.Parameters.AddWithValue("$score", score);
                command.Parameters.AddWithValue("$id", id);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Neizdevās atjaunot iesniegumu.", ex);
            }
        }

        public static void DeleteSubmission(int id)
        {
            try
            {
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Submissions WHERE Id=$id;";
                command.Parameters.AddWithValue("$id", id);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Neizdevās dzēst iesniegumu.", ex);
            }
        }
    }
}
