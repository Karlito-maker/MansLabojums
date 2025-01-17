using MansLabojums.Helpers;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MansLabojums.Views
{
    public partial class TeachersPage : ContentPage
    {
        // Skatā parādītais modelis
        public class TeacherDisplay
        {
            public int Id { get; set; }
            public string DisplayName { get; set; } = "";
            public string GenderDateText { get; set; } = "";
            public string CoursesText { get; set; } = "";
        }

        private ObservableCollection<TeacherDisplay> _teachers = new();
        private TeacherDisplay _selectedTeacher;

        public TeachersPage()
        {
            InitializeComponent();
            TeachersListView.ItemsSource = _teachers;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadTeachers();
        }

        // Ielādējam visus teacher datus
        private void LoadTeachers()
        {
            _teachers.Clear();
            _selectedTeacher = null;
            EditButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;

            var list = DatabaseHelper.GetTeachers();
            foreach (var t in list)
            {
                // Noskaidrojam, kādus kursus viņš pasniedz
                var cList = DatabaseHelper.GetCourses();
                // Tālāk atlasām tos, kur c["TeacherId"] == t.Id
                List<string> teacherCourses = new();
                foreach (var cd in cList)
                {
                    int teacherId = (int)cd["TeacherId"];
                    if (teacherId == t.Id)
                    {
                        // Paņemam kursa nosaukumu
                        string cname = cd["Name"].ToString()!;
                        teacherCourses.Add(cname);
                    }
                }

                string cText = (teacherCourses.Count > 0)
                    ? "Kursi: " + string.Join(", ", teacherCourses)
                    : "(Nav kursu)";

                _teachers.Add(new TeacherDisplay
                {
                    Id = t.Id,
                    DisplayName = t.Name + " " + t.Surname,
                    GenderDateText = t.Gender + ", " + t.ContractDate.ToString("yyyy-MM-dd"),
                    CoursesText = cText
                });
            }
        }

        private void OnTeacherSelected(object sender, SelectedItemChangedEventArgs e)
        {
            _selectedTeacher = (TeacherDisplay)e.SelectedItem;
            bool hasSel = (_selectedTeacher != null);
            EditButton.IsEnabled = hasSel;
            DeleteButton.IsEnabled = hasSel;
        }

        // Pievieno pasniedzēju
        private void OnAddTeacherClicked(object sender, EventArgs e)
        {
            string name = NameEntry.Text?.Trim();
            string surname = SurnameEntry.Text?.Trim();
            string gender = GenderPicker.SelectedItem as string;
            string cDateStr = ContractDateEntry.Text?.Trim();

            // Pārbaudām
            if (string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(surname) ||
                string.IsNullOrEmpty(gender) ||
                !DateTime.TryParse(cDateStr, out DateTime cDate))
            {
                DisplayAlert("Kļūda", "Nepareizi ievadīti dati!", "OK");
                return;
            }

            try
            {
                DatabaseHelper.AddTeacher(name, surname, gender, cDate.ToString("yyyy-MM-dd"));
                ClearAddForm();
                LoadTeachers();
            }
            catch (Exception ex)
            {
                DisplayAlert("Kļūda", ex.Message, "OK");
            }
        }

        private void ClearAddForm()
        {
            NameEntry.Text = "";
            SurnameEntry.Text = "";
            GenderPicker.SelectedItem = null;
            ContractDateEntry.Text = "";
        }

        private void OnCancelAddClicked(object sender, EventArgs e)
        {
            ClearAddForm();
        }

        private async void OnEditTeacherClicked(object sender, EventArgs e)
        {
            if (_selectedTeacher == null) return;

            string[] nm = _selectedTeacher.DisplayName.Split(' ');
            string oldName = nm.Length > 0 ? nm[0] : "";
            string oldSurname = nm.Length > 1 ? nm[1] : "";

            string[] gd = _selectedTeacher.GenderDateText.Split(',');
            string oldGender = gd.Length > 0 ? gd[0].Trim() : "Male";
            string oldDate = gd.Length > 1 ? gd[1].Trim() : "2022-01-01";

            string newName = await DisplayPromptAsync("Labot pasniedzēju", "Vārds:", initialValue: oldName);
            string newSurname = await DisplayPromptAsync("Labot pasniedzēju", "Uzvārds:", initialValue: oldSurname);
            string newGender = await DisplayActionSheet("Dzimums:", "Atcelt", null, "Male", "Female");
            if (newGender == "Atcelt" || string.IsNullOrEmpty(newGender)) return;
            string newDateStr = await DisplayPromptAsync("Labot pasniedzēju", "Datums:", initialValue: oldDate);

            if (!string.IsNullOrEmpty(newName) && !string.IsNullOrEmpty(newSurname) &&
                DateTime.TryParse(newDateStr, out DateTime nd) &&
                (newGender == "Male" || newGender == "Female"))
            {
                try
                {
                    DatabaseHelper.UpdateTeacher(_selectedTeacher.Id, newName, newSurname, newGender, nd.ToString("yyyy-MM-dd"));
                    LoadTeachers();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Kļūda", ex.Message, "OK");
                }
            }
        }

        // Dzēšam teacher, pirms tam dzēšam visus ar viņu saistītos datus (Courses -> Assignments -> Submissions).
        private async void OnDeleteTeacherClicked(object sender, EventArgs e)
        {
            if (_selectedTeacher == null) return;

            bool confirm = await DisplayAlert(
                "Dzēst pasniedzēju?",
                $"Vai tiešām vēlaties dzēst: {_selectedTeacher.DisplayName}?",
                "Jā", "Nē");

            if (confirm)
            {
                try
                {
                    // 1) Iegūstam sarakstu ar visiem coursiem
                    var allCourses = DatabaseHelper.GetCourses();
                    // 2) Atlasām, kuri pieder šim teacher
                    List<int> teacherCourseIds = new();
                    foreach (var c in allCourses)
                    {
                        int tId = (int)c["TeacherId"];
                        int courseId = (int)c["Id"];
                        if (tId == _selectedTeacher.Id)
                        {
                            teacherCourseIds.Add(courseId);
                        }
                    }

                    // 3) Pirms dzēst course, dzēšam tam piederīgos assignmentus un submission
                    //   * Iegūstam assignmentus
                    var allAssigns = DatabaseHelper.GetAssignments();
                    //   * Iegūstam submissionus
                    var allSubs = DatabaseHelper.GetSubmissionsWithIDs();

                    // 4) Dzēšam piesaistītos assignmentus un to submissions
                    foreach (var asn in allAssigns)
                    {
                        if (teacherCourseIds.Contains(asn.CourseId))
                        {
                            // Tātad tie pieder teacher
                            // Dzēšam submissions, kas atsaucas uz asn.Id
                            foreach (var sdict in allSubs)
                            {
                                int subAssId = (int)sdict["AssignmentId"];
                                int subId = (int)sdict["Id"];
                                if (subAssId == asn.Id)
                                {
                                    // Dzēšam submission
                                    DatabaseHelper.DeleteSubmission(subId);
                                }
                            }
                            // Tagad var droši dzēst pašu assignment
                            DatabaseHelper.DeleteAssignment(asn.Id);
                        }
                    }

                    // 5) Tagad dzēšam courses
                    foreach (int cId in teacherCourseIds)
                    {
                        DatabaseHelper.DeleteCourse(cId);
                    }

                    // 6) Beigās varam droši dzēst teacher
                    DatabaseHelper.DeleteTeacher(_selectedTeacher.Id);

                    LoadTeachers();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Kļūda", ex.Message, "OK");
                }
            }
        }
    }
}
