/******************************************************
 * MansLabojums/Views/CoursesPage.xaml.cs
 * Kur var pievienot kursu ar skolotāja izvēli Picker
 ******************************************************/
using MansLabojums.Helpers;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MansLabojums.Views
{
    public partial class CoursesPage : ContentPage
    {
        // Klase, ko rādam sarakstā
        public class CourseDisplay
        {
            public int Id { get; set; }
            public string CourseName { get; set; } = "";
            public string TeacherName { get; set; } = "";
        }

        // Skolotāja modelis lai `Picker` rādītu vārdu + uzvārdu
        public class TeacherItem
        {
            public int Id { get; set; }
            public string FullName { get; set; } = "";
        }

        // Observables
        private ObservableCollection<CourseDisplay> _courses = new();
        public ObservableCollection<TeacherItem> TeacherList { get; set; } = new();

        // Picker atlase
        public TeacherItem SelectedTeacher { get; set; }

        public CoursesPage()
        {
            InitializeComponent();

            // Piesaistām kontekstu, lai var lietot TeacherPicker binding
            BindingContext = this;

            CoursesListView.ItemsSource = _courses;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadCourses();
            LoadTeachers();
        }

        private void LoadCourses()
        {
            _courses.Clear();
            var list = DatabaseHelper.GetCoursesWithTeacherName();
            // sagaidām "Id", "CourseName", "TeacherName"

            foreach (var row in list)
            {
                _courses.Add(new CourseDisplay
                {
                    Id = (int)row["Id"],
                    CourseName = row["CourseName"].ToString()!,
                    TeacherName = row["TeacherName"].ToString()!
                });
            }
        }

        private void LoadTeachers()
        {
            TeacherList.Clear();
            var teachers = DatabaseHelper.GetTeachers();
            // atgriež List<Teacher>
            foreach (var t in teachers)
            {
                TeacherList.Add(new TeacherItem
                {
                    Id = t.Id,
                    FullName = $"{t.Name} {t.Surname}"
                });
            }
        }

        private void OnAddCourseClicked(object sender, EventArgs e)
        {
            // Paņemam ierakstīto kursa nosaukumu
            string cName = CourseNameEntry.Text?.Trim();
            // Paņemam atlasīto teacher
            var teacher = SelectedTeacher;

            if (string.IsNullOrEmpty(cName) || teacher == null)
            {
                DisplayAlert("Kļūda", "Lūdzu ievadiet kursa nosaukumu un izvēlieties skolotāju!", "OK");
                return;
            }

            try
            {
                DatabaseHelper.AddCourse(cName, teacher.Id);
                LoadCourses();

                // Iztīrām ievadlauku un nullejam atlasīto skolotāju
                CourseNameEntry.Text = "";
                SelectedTeacher = null;
                TeacherPicker.SelectedItem = null;
            }
            catch (Exception ex)
            {
                DisplayAlert("Kļūda", $"Neizdevās pievienot kursu: {ex.Message}", "OK");
            }
        }

        private async void OnEditCourseClicked(object sender, EventArgs e)
        {
            if (CoursesListView.SelectedItem is CourseDisplay selC)
            {
                // var ierosināt jauno nosaukumu
                string newName = await DisplayPromptAsync("Labot kursu", "Jauns nosaukums:", initialValue: selC.CourseName);
                // Piemēram, var arī vienkārši lūgt jaunam teacherId. 
                // Uz doto brīdi parādām prompt
                string newTidStr = await DisplayPromptAsync("Labot kursu", "Jauns TeacherId:", initialValue: "1");

                if (!string.IsNullOrEmpty(newName) && int.TryParse(newTidStr, out int newTid))
                {
                    DatabaseHelper.UpdateCourse(selC.Id, newName, newTid);
                    LoadCourses();
                }
            }
            else
            {
                await DisplayAlert("Brīdinājums", "Nav izvēlēts neviens kurss!", "OK");
            }
        }

        private async void OnDeleteCourseClicked(object sender, EventArgs e)
        {
            if (CoursesListView.SelectedItem is CourseDisplay selC)
            {
                bool confirm = await DisplayAlert("Dzēst kursu?", selC.CourseName, "Jā", "Nē");
                if (confirm)
                {
                    DatabaseHelper.DeleteCourse(selC.Id);
                    LoadCourses();
                }
            }
            else
            {
                await DisplayAlert("Brīdinājums", "Nav izvēlēts neviens kurss!", "OK");
            }
        }
    }
}

