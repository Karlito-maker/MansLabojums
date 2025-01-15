using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MansLabojums.Models;
using MansLabojums.Helpers;

namespace MansLabojums.Views
{
    public partial class CoursePage : ContentPage
    {
        public CoursePage()
        {
            InitializeComponent();
            BindingContext = new CoursesViewModel();
        }
    }

    public class CoursesViewModel : BaseViewModel
    {
        public ObservableCollection<Course> Courses { get; set; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand GenerateTestDataCommand { get; }

        public CoursesViewModel()
        {
            Title = "Kursi";
            Courses = new ObservableCollection<Course>();
            AddCommand = new Command(async () => await AddCourse());
            EditCommand = new Command<Course>(async (course) => await EditCourse(course));
            DeleteCommand = new Command<Course>(async (course) => await DeleteCourse(course));
            GenerateTestDataCommand = new Command(async () => await GenerateTestData());

            LoadCourses().ConfigureAwait(false);
        }

        async Task LoadCourses()
        {
            var courses = await DatabaseHelper.GetCoursesAsync();
            Courses.Clear();
            foreach (var course in courses)
            {
                Courses.Add(course);
            }
        }

        async Task AddCourse()
        {
            var course = new Course();
            // Here you can implement a modal page to get user input
            // await App.Current.MainPage.Navigation.PushAsync(new CourseDetailPage(course));
            await LoadCourses();
        }

        async Task EditCourse(Course course)
        {
            // Open the detail page for editing
            // await App.Current.MainPage.Navigation.PushAsync(new CourseDetailPage(course));
            await LoadCourses();
        }

        async Task DeleteCourse(Course course)
        {
            await DatabaseHelper.DeleteCourseAsync(course);
            Courses.Remove(course);
        }

        async Task GenerateTestData()
        {
            var testCourses = new List<Course>
            {
                new Course { CourseName = "Test Course 1", TeacherId = 1 },
                new Course { CourseName = "Test Course 2", TeacherId = 2 },
                new Course { CourseName = "Test Course 3", TeacherId = 3 },
            };

            foreach (var course in testCourses)
            {
                await DatabaseHelper.SaveCourseAsync(course);
                Courses.Add(course);
            }
        }
    }
}






