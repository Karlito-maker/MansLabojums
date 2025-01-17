using MansLabojums.Helpers;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MansLabojums.Views
{
    public partial class AssignmentsPage : ContentPage
    {
        // Displejam uzdevumu
        public class AssignmentDisplay
        {
            public int Id { get; set; }
            public string TitleText { get; set; } = ""; // "[1] Algebras.."
            public string DetailText { get; set; } = ""; // "Termiņš: .."
            public int CourseId { get; set; }
        }

        // Kursu dropdown
        public class CourseItem
        {
            public int Id { get; set; }
            public string DisplayText { get; set; } = ""; // "[1] Matemātika"
        }

        private ObservableCollection<AssignmentDisplay> _assignments = new();
        private AssignmentDisplay _selectedAssignment;

        public ObservableCollection<CourseItem> CourseList { get; set; } = new();
        public CourseItem SelectedCourse { get; set; }

        public AssignmentsPage()
        {
            InitializeComponent();
            AssignmentsListView.ItemsSource = _assignments;
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadAssignments();
            LoadCourses();
        }

        private void LoadAssignments()
        {
            _assignments.Clear();
            _selectedAssignment = null;
            EditButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;

            // Izgūstam assignmentus
            var list = DatabaseHelper.GetAssignments();
            foreach (var a in list)
            {
                string title = $"[{a.Id}] {a.Description}";
                string detail = $"Termiņš: {a.Deadline:yyyy-MM-dd}, CourseId={a.CourseId}";
                _assignments.Add(new AssignmentDisplay
                {
                    Id = a.Id,
                    TitleText = title,
                    DetailText = detail,
                    CourseId = a.CourseId
                });
            }
        }

        private void LoadCourses()
        {
            CourseList.Clear();
            var cRows = DatabaseHelper.GetCourses();
            // satur "Id", "Name", "TeacherId"
            foreach (var row in cRows)
            {
                int cid = (int)row["Id"];
                string cname = row["Name"].ToString()!;
                CourseList.Add(new CourseItem
                {
                    Id = cid,
                    DisplayText = $"[{cid}] {cname}"
                });
            }
        }

        private void OnAssignmentSelected(object sender, SelectedItemChangedEventArgs e)
        {
            _selectedAssignment = (AssignmentDisplay)e.SelectedItem;
            bool hasSel = (_selectedAssignment != null);
            EditButton.IsEnabled = hasSel;
            DeleteButton.IsEnabled = hasSel;
        }

        private void OnAddAssignmentClicked(object sender, EventArgs e)
        {
            string desc = DescriptionEntry.Text?.Trim();
            string dlStr = DeadlineEntry.Text?.Trim();
            if (SelectedCourse == null)
            {
                DisplayAlert("Kļūda", "Nav izvēlēts kurss!", "OK");
                return;
            }
            if (string.IsNullOrEmpty(desc) ||
                string.IsNullOrEmpty(dlStr) ||
                !DateTime.TryParse(dlStr, out DateTime dl))
            {
                DisplayAlert("Kļūda", "Nepareizi ievadīts apraksts vai datums!", "OK");
                return;
            }

            try
            {
                DatabaseHelper.AddAssignment(desc, dl, SelectedCourse.Id);
                ClearAddForm();
                LoadAssignments();
            }
            catch (Exception ex)
            {
                DisplayAlert("Kļūda", ex.Message, "OK");
            }
        }

        private void ClearAddForm()
        {
            DescriptionEntry.Text = "";
            DeadlineEntry.Text = "";
            SelectedCourse = null;
            CoursePicker.SelectedItem = null;
        }

        private void OnCancelAddClicked(object sender, EventArgs e)
        {
            ClearAddForm();
        }

        private async void OnEditAssignmentClicked(object sender, EventArgs e)
        {
            if (_selectedAssignment == null) return;

            // Paņemam aprakstu no TitleText => "[1] Algebras.."
            int bracketPos = _selectedAssignment.TitleText.IndexOf(']');
            string oldDesc = (bracketPos >= 0 && bracketPos < _selectedAssignment.TitleText.Length - 1)
                ? _selectedAssignment.TitleText.Substring(bracketPos + 1).Trim()
                : _selectedAssignment.TitleText;

            string newDesc = await DisplayPromptAsync("Labot uzdevumu",
                                                      "Jauns apraksts:",
                                                      initialValue: oldDesc);
            string oldDl = _selectedAssignment.DetailText;
            // "Termiņš: 2024-12-31, CourseId=1"
            // Principā parse
            // bet vienkāršības labad:
            string newDlStr = await DisplayPromptAsync("Labot uzdevumu",
                                                       "Jauns Deadline (YYYY-MM-DD):",
                                                       initialValue: "2024-01-01");
            // TeacherId var labot? Labāk course:
            // var course = . un prompt
            var cList = DatabaseHelper.GetCourses();
            List<string> items = new();
            foreach (var c in cList)
            {
                items.Add($"ID={(int)c["Id"]}: {c["Name"]}");
            }
            items.Add("Atcelt");
            string pick = await DisplayActionSheet("Izvēlieties kursu", "Atcelt", null, items.ToArray());
            if (pick == "Atcelt" || string.IsNullOrEmpty(pick)) return;

            // Piem. "ID=2: Fizika"
            int cId = 0;
            if (pick.StartsWith("ID="))
            {
                string[] sp = pick.Split(':');
                if (sp.Length > 0)
                {
                    string part1 = sp[0]; // "ID=2"
                    string[] eq = part1.Split('=');
                    if (eq.Length > 1)
                    {
                        int.TryParse(eq[1], out cId);
                    }
                }
            }

            if (string.IsNullOrEmpty(newDesc) ||
                !DateTime.TryParse(newDlStr, out DateTime ndl) ||
                cId <= 0)
            {
                // nav korekti
                return;
            }

            try
            {
                DatabaseHelper.UpdateAssignment(_selectedAssignment.Id, newDesc, ndl, cId);
                LoadAssignments();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kļūda", ex.Message, "OK");
            }
        }

        // Dzēš assignment => pirms tam dzēš Submissions
        private async void OnDeleteAssignmentClicked(object sender, EventArgs e)
        {
            if (_selectedAssignment == null) return;

            bool confirm = await DisplayAlert("Dzēst uzdevumu?",
                _selectedAssignment.TitleText,
                "Jā", "Nē");
            if (!confirm) return;

            try
            {
                // Dzēš Submissions, kas norāda uz assignment
                var allSubs = DatabaseHelper.GetSubmissionsWithIDs();
                foreach (var sdict in allSubs)
                {
                    int assId = (int)sdict["AssignmentId"];
                    if (assId == _selectedAssignment.Id)
                    {
                        int subId = (int)sdict["Id"];
                        DatabaseHelper.DeleteSubmission(subId);
                    }
                }
                // Tagad droši dzēšam assignment
                DatabaseHelper.DeleteAssignment(_selectedAssignment.Id);

                LoadAssignments();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kļūda", ex.Message, "OK");
            }
        }
    }
}
