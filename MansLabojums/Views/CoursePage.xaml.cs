/******************************************************
 * MansLabojums/Views/CoursesPage.xaml.cs
 * Kursi. Lai var dzēst "testa datus", pirms Course
 * dzēšanas šeit dzēsīsim tam piesaistītos Assignments un Submissions.
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
        // Kursu attēlošanai sarakstā
        public class CourseDisplay
        {
            public int Id { get; set; }
            public string DisplayText { get; set; } = "";  // Piemēram "[1] Matemātika"
            public string TeacherText { get; set; } = "";  // Piemēram "Skolotājs: Anna Kalniņa"
        }

        private ObservableCollection<CourseDisplay> _courses = new();
        private CourseDisplay _selectedCourse;

        public CoursesPage()
        {
            InitializeComponent();
            CoursesListView.ItemsSource = _courses;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadCourses();
        }

        private void LoadCourses()
        {
            _courses.Clear();
            _selectedCourse = null;
            EditButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;

            // Iegūstam no DB visu "CoursesWithTeacherName"
            var list = DatabaseHelper.GetCoursesWithTeacherName();
            // sagaidām "Id", "CourseName", "TeacherName"
            foreach (var row in list)
            {
                int cid = (int)row["Id"];
                string cname = row["CourseName"].ToString()!;
                string tname = row["TeacherName"].ToString()!;

                _courses.Add(new CourseDisplay
                {
                    Id = cid,
                    DisplayText = $"[{cid}] {cname}",
                    TeacherText = $"Skolotājs: {tname}"
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
            string cName = CourseNameEntry.Text?.Trim();
            string tIdStr = TeacherIdEntry.Text?.Trim();
            if (string.IsNullOrEmpty(cName) ||
                string.IsNullOrEmpty(tIdStr) ||
                !int.TryParse(tIdStr, out int tid))
            {
                DisplayAlert("Kļūda", "Nepareizi ievadīts kursa nosaukums vai teacherId!", "OK");
                return;
            }

            try
            {
                DatabaseHelper.AddCourse(cName, tid);
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
            TeacherIdEntry.Text = "";
        }

        private void OnCancelAddClicked(object sender, EventArgs e)
        {
            ClearAddForm();
        }

        private async void OnEditCourseClicked(object sender, EventArgs e)
        {
            if (_selectedCourse == null) return;

            // Paņemam info no "[1] Matemātika"
            string oldLine = _selectedCourse.DisplayText;
            string newLine = await DisplayPromptAsync("Labot kursu", "Jauns nosaukums (ar ID iekavās, ja gribat)?", initialValue: oldLine);

            string newTidStr = await DisplayPromptAsync("Labot kursu", "Jauns teacherId:", initialValue: "1");

            if (!string.IsNullOrEmpty(newLine) &&
                int.TryParse(newTidStr, out int newTid))
            {
                // Izlobam kursa nosaukumu
                int bracketPos = newLine.IndexOf(']');
                string pureName = (bracketPos >= 0 && bracketPos < newLine.Length - 1)
                    ? newLine.Substring(bracketPos + 1).Trim()
                    : newLine;

                try
                {
                    DatabaseHelper.UpdateCourse(_selectedCourse.Id, pureName, newTid);
                    LoadCourses();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Kļūda", ex.Message, "OK");
                }
            }
        }

        // Dzēšam kursu "kaskadēti": 1) dzēšam tam piederīgos uzdevumus un to Submissions
        private async void OnDeleteCourseClicked(object sender, EventArgs e)
        {
            if (_selectedCourse == null) return;

            bool confirm = await DisplayAlert(
                "Dzēst kursu?",
                $"Vai tiešām vēlaties dzēst kursu: {_selectedCourse.DisplayText}?",
                "Jā", "Nē");

            if (confirm)
            {
                try
                {
                    // 1) Iegūstam Assignments, Submissions
                    var allAssigns = DatabaseHelper.GetAssignments();
                    var allSubs = DatabaseHelper.GetSubmissionsWithIDs();

                    // 2) atrodam assignmentus, kam CourseId = _selectedCourse.Id
                    foreach (var a in allAssigns)
                    {
                        if (a.CourseId == _selectedCourse.Id)
                        {
                            // 3) Dzēšam submission, kas piesaistīti assignmentId = a.Id
                            foreach (var sd in allSubs)
                            {
                                int subAssId = (int)sd["AssignmentId"];
                                int subId = (int)sd["Id"];
                                if (subAssId == a.Id)
                                {
                                    DatabaseHelper.DeleteSubmission(subId);
                                }
                            }
                            // 4) Dzēšam assignment
                            DatabaseHelper.DeleteAssignment(a.Id);
                        }
                    }

                    // 5) Tagad droši dzēšam course
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
}
