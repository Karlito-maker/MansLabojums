using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MansLabojums.Models;
using Microsoft.Maui.Controls;

namespace MansLabojums.ViewModels
{
    public class CoursesViewModel : BaseViewModel
    {
        public ObservableCollection<Course> Courses { get; set; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public CoursesViewModel()
        {
            Title = "Kursi";
            Courses = new ObservableCollection<Course>();
            AddCommand = new Command(async () => await AddCourse());
            EditCommand = new Command<Course>(async (course) => await EditCourse(course));
            DeleteCommand = new Command<Course>(async (course) => await DeleteCourse(course));

            LoadCourses();
        }

        async void LoadCourses()
        {
            var courses = await App.Database.GetCoursesAsync();
            Courses.Clear();
            foreach (var course in courses)
            {
                Courses.Add(course);
            }
        }

        async Task AddCourse()
        {
            var newCourse = new Course { CourseName = "Jauns kurss", TeacherId = 1 };
            await App.Database.SaveCourseAsync(newCourse);
            Courses.Add(newCourse);
        }

        async Task EditCourse(Course course)
        {
            course.CourseName = "Atjaunināts kurss";
            await App.Database.SaveCourseAsync(course);
            LoadCourses();
        }

        async Task DeleteCourse(Course course)
        {
            await App.Database.DeleteCourseAsync(course);
            Courses.Remove(course);
        }
    }
}
