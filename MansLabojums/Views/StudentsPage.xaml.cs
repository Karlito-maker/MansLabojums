using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MansLabojums.Models;
using MansLabojums.Helpers;

namespace MansLabojums.Views
{
    public partial class StudentsPage : ContentPage
    {
        public StudentsPage()
        {
            InitializeComponent();
            BindingContext = new StudentsViewModel();
        }
    }

    public class StudentsViewModel : BaseViewModel
    {
        public ObservableCollection<Student> Students { get; set; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand GenerateTestDataCommand { get; }

        public StudentsViewModel()
        {
            Title = "Studenti";
            Students = new ObservableCollection<Student>();
            AddCommand = new Command(async () => await AddStudent());
            EditCommand = new Command<Student>(async (student) => await EditStudent(student));
            DeleteCommand = new Command<Student>(async (student) => await DeleteStudent(student));
            GenerateTestDataCommand = new Command(async () => await GenerateTestData());

            LoadStudents().ConfigureAwait(false);
        }

        async Task LoadStudents()
        {
            var students = await DatabaseHelper.GetStudentsAsync();
            Students.Clear();
            foreach (var student in students)
            {
                Students.Add(student);
            }
        }

        async Task AddStudent()
        {
            var student = new Student();
            // Here you can implement a modal page to get user input
            // await App.Current.MainPage.Navigation.PushAsync(new StudentDetailPage(student));
            await LoadStudents();
        }

        async Task EditStudent(Student student)
        {
            // Open the detail page for editing
            // await App.Current.MainPage.Navigation.PushAsync(new StudentDetailPage(student));
            await LoadStudents();
        }

        async Task DeleteStudent(Student student)
        {
            await DatabaseHelper.DeleteStudentAsync(student);
            Students.Remove(student);
        }

        async Task GenerateTestData()
        {
            var testStudents = new List<Student>
            {
                new Student { Name = "Test Student 1", StudentId = "S1001" },
                new Student { Name = "Test Student 2", StudentId = "S1002" },
                new Student { Name = "Test Student 3", StudentId = "S1003" },
            };

            foreach (var student in testStudents)
            {
                await DatabaseHelper.SaveStudentAsync(student);
                Students.Add(student);
            }
        }
    }
}






