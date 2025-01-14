using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MansLabojums.Models;
using Microsoft.Maui.Controls;

namespace MansLabojums.ViewModels
{
    public class TeachersViewModel : BaseViewModel
    {
        public ObservableCollection<Teacher> Teachers { get; set; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public TeachersViewModel()
        {
            Title = "Pasniedzēji";
            Teachers = new ObservableCollection<Teacher>();
            AddCommand = new Command(async () => await AddTeacher());
            EditCommand = new Command<Teacher>(async (teacher) => await EditTeacher(teacher));
            DeleteCommand = new Command<Teacher>(async (teacher) => await DeleteTeacher(teacher));

            LoadTeachers();
        }

        async void LoadTeachers()
        {
            var teachers = await App.Database.GetTeachersAsync();
            Teachers.Clear();
            foreach (var teacher in teachers)
            {
                Teachers.Add(teacher);
            }
        }

        async Task AddTeacher()
        {
            var newTeacher = new Teacher { Name = "Jauns pasniedzējs", Subject = "Priekšmets" };
            await App.Database.SaveTeacherAsync(newTeacher);
            Teachers.Add(newTeacher);
        }

        async Task EditTeacher(Teacher teacher)
        {
            teacher.Name = "Atjaunināts pasniedzējs";
            await App.Database.SaveTeacherAsync(teacher);
            LoadTeachers();
        }

        async Task DeleteTeacher(Teacher teacher)
        {
            await App.Database.DeleteTeacherAsync(teacher);
            Teachers.Remove(teacher);
        }
    }
}
