using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MansLabojums.Models;
using Microsoft.Maui.Controls;

namespace MansLabojums.ViewModels
{
    public class StudentsViewModel : BaseViewModel
    {
        public ObservableCollection<Student> Students { get; set; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public StudentsViewModel()
        {
            Title = "Studenti";
            Students = new ObservableCollection<Student>();
            AddCommand = new Command(async () => await AddStudent());
            EditCommand = new Command<Student>(async (student) => await EditStudent(student));
            DeleteCommand = new Command<Student>(async (student) => await DeleteStudent(student));

            LoadStudents();
        }

        async void LoadStudents()
        {
            var students = await App.Database.GetStudentsAsync();
            Students.Clear();
            foreach (var student in students)
            {
                Students.Add(student);
            }
        }

        async Task AddStudent()
        {
            var newStudent = new Student { Name = "Jauns students", StudentId = "ID123" };
            await App.Database.SaveStudentAsync(newStudent);
            Students.Add(newStudent);
        }

        async Task EditStudent(Student student)
        {
            student.Name = "Atjaunināts students";
            await App.Database.SaveStudentAsync(student);
            LoadStudents();
        }

        async Task DeleteStudent(Student student)
        {
            await App.Database.DeleteStudentAsync(student);
            Students.Remove(student);
        }
    }
}

