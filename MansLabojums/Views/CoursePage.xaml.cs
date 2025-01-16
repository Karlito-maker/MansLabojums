using MansLabojums.Helpers;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MansLabojums.Views
{
    public partial class CoursesPage : ContentPage
    {
        // Rādām sarakstā
        public class CourseDisplay
        {
            public int Id { get; set; }
            public string CourseLabel { get; set; } = "";
            public string TeacherLabel { get; set; } = "";
        }

        // Skolotāju dropdown 
        public class TeacherItem
        {
            public int Id { get; set; }
            public string TeacherFullName { get; set; } = "";
        }

        private ObservableCollection<CourseDisplay> _courses = new();
        private CourseDisplay _selected;

        public ObservableCollection<TeacherItem> TeacherList { get; set; } = new();
        public TeacherItem SelectedTeacher { get; set; }

        public CoursesPage()
        {
            InitializeComponent();
            CoursesListView.ItemsSource = _courses;
            BindingContext = this;
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
            _selected = null;
            EditButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;

            var list = DatabaseHelper.GetCoursesWithTeacherName();
            // sagaidām  "Id", "CourseName", "TeacherName"
            foreach (var row in list)
            {
                int cid = (int)row["Id"];
                string cname = row["CourseName"].ToString()!;
                string tname = row["TeacherName"].ToString()!;
                _courses.Add(new CourseDisplay
                {
                    Id = cid,
                    CourseLabel = $"[{cid}] {cname}",
                    TeacherLabel = $"Pasniedz: {tname}"
                });
            }
        }

        private void LoadTeachers()
        {
            TeacherList.Clear();
            var teachers = DatabaseHelper.GetTeachers();
            foreach (var t in teachers)
            {
                TeacherList.Add(new TeacherItem
                {
                    Id = t.Id,
                    TeacherFullName = $"{t.Name} {t.Surname}"
                });
            }
        }

        private void OnCourseSelected(object sender, SelectedItemChangedEventArgs e)
        {
            _selected = (CourseDisplay)e.SelectedItem;
            bool hasSel = (_selected != null);
            EditButton.IsEnabled = hasSel;
            DeleteButton.IsEnabled = hasSel;
        }

        private void OnAddCourseClicked(object sender, EventArgs e)
        {
            string cname = CourseNameEntry.Text?.Trim();
            if (SelectedTeacher == null)
            {
                DisplayAlert("Kļūda", "Izvēlieties skolotāju!", "OK");
                return;
            }

            if (string.IsNullOrEmpty(cname))
            {
                DisplayAlert("Kļūda", "Nav kursa nosaukums!", "OK");
                return;
            }

            try
            {
                DatabaseHelper.AddCourse(cname, SelectedTeacher.Id);
                ClearAddForm();
                LoadCourses();
            }
            catch (Exception ex)
            {
                DisplayAlert("Kļūda", ex.Message, "OK");
            }
        }

        private void ClearAddForm()
        {
            CourseNameEntry.Text = "";
            SelectedTeacher = null;
            TeacherPicker.SelectedItem = null;
        }

        private void OnCancelAddClicked(object sender, EventArgs e)
        {
            ClearAddForm();
        }

        private async void OnEditCourseClicked(object sender, EventArgs e)
        {
            if (_selected == null) return;

            // Iegūstam ID=??, ierosinām prompt
            string oldTitle = _selected.CourseLabel; // "[2] Fizika"
            string newName = await DisplayPromptAsync("Labot kursu", "Jauns nosaukums:", initialValue: oldTitle);

            // Tagad teacher:
            // var newTeacherId = ??  -> reāli var parādīt citu picker, bet vienkāršības labad => prompt
            string newTStr = await DisplayPromptAsync("Labot kursu", "Jauns TeacherId:", initialValue: "1");

            if (!string.IsNullOrEmpty(newName) && int.TryParse(newTStr, out int newTid))
            {
                // Izvelkam ID no oldTitle? 
                // newName satur “[2] Fizika”, mēs gribam “Fizika”
                int bracketPos = newName.IndexOf(']');
                string pureName = (bracketPos >= 0 && bracketPos < newName.Length - 1)
                                  ? newName.Substring(bracketPos + 1).Trim()
                                  : newName;

                try
                {
                    DatabaseHelper.UpdateCourse(_selected.Id, pureName, newTid);
                    LoadCourses();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Kļūda", ex.Message, "OK");
                }
            }
        }

        private async void OnDeleteCourseClicked(object sender, EventArgs e)
        {
            if (_selected == null) return;

            bool confirm = await DisplayAlert("Dzēst kursu?", _selected.CourseLabel, "Jā", "Nē");
            if (confirm)
            {
                try
                {
                    DatabaseHelper.DeleteCourse(_selected.Id);
                    LoadCourses();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Kļūda", ex.Message, "OK");
                }
            }
        }
    }
}
