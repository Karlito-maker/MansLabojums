using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MansLabojums.Models;
using MansLabojums.Helpers;

namespace MansLabojums.Views
{
    public partial class TeachersPage : ContentPage
    {
        public TeachersPage()
        {
            InitializeComponent();
            BindingContext = new TeachersViewModel();
        }
    }

    public class TeachersViewModel : BaseViewModel
    {
        public ObservableCollection<Teacher> Teachers { get; set; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand GenerateTestDataCommand { get; }

        public TeachersViewModel()
        {
            Title = "Pasniedzēji";
            Teachers = new ObservableCollection<Teacher>();
            AddCommand = new Command(async () => await AddTeacher());
            EditCommand = new Command<Teacher>(async (teacher) => await EditTeacher(teacher));
            DeleteCommand = new Command<Teacher>(async (teacher) => await DeleteTeacher(teacher));
            GenerateTestDataCommand = new Command(async () => await GenerateTestData());

            LoadTeachers().ConfigureAwait(false);
        }

        async Task LoadTeachers()
        {
            var teachers = await DatabaseHelper.GetTeachersAsync();
            Teachers.Clear();
            foreach (var teacher in teachers)
            {
                Teachers.Add(teacher);
            }
        }

        async Task AddTeacher()
        {
            var teacher = new Teacher();
            // Here you can implement a modal page to get user input
            // await App.Current.MainPage.Navigation.PushAsync(new TeacherDetailPage(teacher));
            await LoadTeachers();
        }

        async Task EditTeacher(Teacher teacher)
        {
            // Open the detail page for editing
            // await App.Current.MainPage.Navigation.PushAsync(new TeacherDetailPage(teacher));
            await LoadTeachers();
        }

        async Task DeleteTeacher(Teacher teacher)
        {
            await DatabaseHelper.DeleteTeacherAsync(teacher);
            Teachers.Remove(teacher);
        }

        async Task GenerateTestData()
        {
            var testTeachers = new List<Teacher>
            {
                new Teacher { Name = "Test Teacher 1", Subject = "Subject 1" },
                new Teacher { Name = "Test Teacher 2", Subject = "Subject 2" },
                new Teacher { Name = "Test Teacher 3", Subject = "Subject 3" },
            };

            foreach (var teacher in testTeachers)
            {
                await DatabaseHelper.SaveTeacherAsync(teacher);
                Teachers.Add(teacher);
            }
        }
    }
}


