using MansLabojums.Helpers;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MansLabojums.Views
{
    public partial class CoursesPage : ContentPage
    {
        // Kursu saraksta atveidošanai
        public class CourseDisplay
        {
            public int Id { get; set; }
            public string CourseLabel { get; set; } = "";
            public string TeacherLabel { get; set; } = "";
        }

        // Skolotāju dropdown items
        public class TeacherItem
        {
            public int Id { get; set; }
            public string FullName { get; set; } = "";
        }

        private ObservableCollection<CourseDisplay> _courses = new();
        private CourseDisplay _selectedCourse;

        // Picker saraksts
        public ObservableCollection<TeacherItem> TeacherList { get; set; } = new();
        public TeacherItem SelectedTeacher { get; set; }

        public CoursesPage()
        {
            InitializeComponent();
            CoursesListView.ItemsSource = _courses;
            BindingContext = this; // lai TeacherPicker Binding strādā
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
            _selectedCourse = null;
            EditButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;

            // Iegūstam kursus + skolotāju vārdu
            var list = DatabaseHelper.GetCoursesWithTeacherName();
            // Katrā row: "Id", "CourseName", "TeacherName"
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
            var teacherModels = DatabaseHelper.GetTeachers();
            foreach (var t in teacherModels)
            {
                TeacherList.Add(new TeacherItem
                {
                    Id = t.Id,
                    FullName = t.Name + " " + t.Surname
                });
            }
        }

        private void OnCourseSelected(object sender, SelectedItemChangedEventArgs e)
        {
            _selectedCourse = (CourseDisplay)e.SelectedItem;
            bool hasSel = (_selectedCourse != null);
            EditButton.IsEnabled = hasSel;
            DeleteButton.IsEnabled = hasSel;
        }

        private void OnAddCourseClicked(object sender, EventArgs e)
        {
            string cname = CourseNameEntry.Text?.Trim();
            if (SelectedTeacher == null)
            {
                DisplayAlert("Kļūda", "Nav izvēlēts skolotājs!", "OK");
                return;
            }

            if (string.IsNullOrEmpty(cname))
            {
                DisplayAlert("Kļūda", "Lūdzu ievadiet kursa nosaukumu!", "OK");
                return;
            }

            try
            {
                // Pievienojam kursu ar TeacherId no SelectedTeacher
                DatabaseHelper.AddCourse(cname, SelectedTeacher.Id);
                ClearAddForm();
                LoadCourses();
            }
            catch (Exception ex)
            {
                DisplayAlert("Kļūda", $"Neizdevās pievienot kursu: {ex.Message}", "OK");
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

        // Labot kursu – varam Prompt lūgt jaunu courseName un nodrošināt dropdown teacher
        private async void OnEditCourseClicked(object sender, EventArgs e)
        {
            if (_selectedCourse == null) return;

            // Lūdzam jaunu kursa nosaukumu promptā
            string oldLabel = _selectedCourse.CourseLabel; 
            
            int bracketPos = oldLabel.IndexOf(']');
            string oldCName = (bracketPos >= 0 && bracketPos < oldLabel.Length - 1)
                ? oldLabel.Substring(bracketPos + 1).Trim()
                : oldLabel; // rezerves variants

            string newName = await DisplayPromptAsync("Labot kursu",
                                                      "Jauns kursa nosaukums:",
                                                      initialValue: oldCName);

            
            var teachers = DatabaseHelper.GetTeachers();
            List<string> items = new();
            foreach (var t in teachers)
            {
                items.Add($"ID={t.Id}: {t.Name} {t.Surname}");
            }
            items.Add("Atcelt");

            string teacherSelected = await DisplayActionSheet("Izvēlieties skolotāju", "Atcelt", null, items.ToArray());
            if (teacherSelected == "Atcelt" || string.IsNullOrEmpty(teacherSelected)) return;

            
            int teacherId = 0;
            if (teacherSelected.StartsWith("ID="))
            {
               
                string[] splitted = teacherSelected.Split(':');
                if (splitted.Length > 0)
                {
                    string part = splitted[0].Trim(); 
                    string[] eq = part.Split('=');
                    if (eq.Length > 1)
                    {
                        if (!int.TryParse(eq[1], out teacherId))
                        {
                            teacherId = 0;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(newName) || teacherId <= 0)
            {
                // ja neatpazinām
                return;
            }

            try
            {
                DatabaseHelper.UpdateCourse(_selectedCourse.Id, newName, teacherId);
                LoadCourses();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kļūda", ex.Message, "OK");
            }
        }

        // Dzēšam kursu pirms tam dzēšam tā assignmentus un submissionus
        private async void OnDeleteCourseClicked(object sender, EventArgs e)
        {
            if (_selectedCourse == null) return;

            bool confirm = await DisplayAlert("Dzēst kursu?",
                $"{_selectedCourse.CourseLabel}",
                "Jā", "Nē");
            if (!confirm) return;

            try
            {
                // 1) atrodam assignmentus, kas pieder tam kursam
                var allAssigns = DatabaseHelper.GetAssignments();
                var allSubs = DatabaseHelper.GetSubmissionsWithIDs();

                foreach (var asn in allAssigns)
                {
                    if (asn.CourseId == _selectedCourse.Id)
                    {
                        // Dzēšam submissions
                        foreach (var sdict in allSubs)
                        {
                            int subAssId = (int)sdict["AssignmentId"];
                            int subId = (int)sdict["Id"];
                            if (subAssId == asn.Id)
                            {
                                DatabaseHelper.DeleteSubmission(subId);
                            }
                        }
                        // Tagad dzēšam assignment
                        DatabaseHelper.DeleteAssignment(asn.Id);
                    }
                }

                // 2) Dzēšam pašu kursu
                DatabaseHelper.DeleteCourse(_selectedCourse.Id);

                LoadCourses();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kļūda", ex.Message, "OK");
            }
        }
    }
}
