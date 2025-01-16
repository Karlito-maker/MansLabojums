using MansLabojums.Helpers;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MansLabojums.Views
{
    public partial class AssignmentsPage : ContentPage
    {
        // Rādām sarakstā
        public class AssignmentDisplay
        {
            public int Id { get; set; }
            public string TitleText { get; set; } = "";
            public string DetailText { get; set; } = "";
            public DateTime Deadline { get; set; }
            public int CourseId { get; set; }
        }

        // Modelis CoursePicker
        public class CourseItem
        {
            public int Id { get; set; }
            public string CourseName { get; set; } = "";
        }

        private ObservableCollection<AssignmentDisplay> _assignments = new();
        private AssignmentDisplay _selected;

        // Picker saraksts
        public ObservableCollection<CourseItem> CourseList { get; set; } = new();
        public CourseItem SelectedCourse { get; set; }

        public AssignmentsPage()
        {
            InitializeComponent();

            AssignmentsListView.ItemsSource = _assignments;
            // lai var b. c. 
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
            _selected = null;
            DeleteButton.IsEnabled = false;
            EditButton.IsEnabled = false;

            var list = DatabaseHelper.GetAssignments();
            foreach (var a in list)
            {
                _assignments.Add(new AssignmentDisplay
                {
                    Id = a.Id,
                    Deadline = a.Deadline,
                    CourseId = a.CourseId,
                    TitleText = $"[{a.Id}] {a.Description}",
                    DetailText = $"Termiņš: {a.Deadline:yyyy-MM-dd}, CourseId={a.CourseId}"
                });
            }
        }

        private void LoadCourses()
        {
            CourseList.Clear();
            // var variant = DatabaseHelper.GetCourses(); 
            // or better => GetCoursesWithTeacherName
            var crows = DatabaseHelper.GetCourses();
            foreach (var row in crows)
            {
                // "Id", "Name", "TeacherId"
                int cid = (int)row["Id"];
                string cname = row["Name"].ToString()!;
                CourseList.Add(new CourseItem
                {
                    Id = cid,
                    CourseName = $"{cid}: {cname}"
                });
            }
        }

        private void OnAssignmentSelected(object sender, SelectedItemChangedEventArgs e)
        {
            _selected = (AssignmentDisplay)e.SelectedItem;
            bool hasSel = (_selected != null);
            EditButton.IsEnabled = hasSel;
            DeleteButton.IsEnabled = hasSel;
        }

        private void OnAddAssignmentClicked(object sender, EventArgs e)
        {
            string desc = DescriptionEntry.Text?.Trim();
            string deadlineStr = DeadlineEntry.Text?.Trim();
            if (SelectedCourse == null)
            {
                DisplayAlert("Kļūda", "Izvēlieties kursu!", "OK");
                return;
            }

            if (string.IsNullOrEmpty(desc) ||
                string.IsNullOrEmpty(deadlineStr) ||
                !DateTime.TryParse(deadlineStr, out DateTime dl))
            {
                DisplayAlert("Kļūda", "Nepareizi ievadīts apraksts vai datums", "OK");
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
            if (_selected == null) return;

            // var oldDesc => parse from TitleText "[1] Algebras.."
            // var splittedTitle = _selected.TitleText.Split(']');
            // or simpler, just ask user for new desc
            string oldLine = _selected.TitleText; // "[1] ...desc"
            // drošāks variants
            string newDesc = await DisplayPromptAsync("Labot uzdevumu", "Jauns apraksts:", initialValue: oldLine);

            // Deadline
            string dStr = _selected.Deadline.ToString("yyyy-MM-dd");
            string newDeadline = await DisplayPromptAsync("Labot uzdevumu", "Jauns Deadline:", initialValue: dStr);

            // CourseId vietā -> user can pick from a dropdown at runtime, or ask
            // laika trūkuma dēļ varam piedāvāt Prompt
            string newCidStr = await DisplayPromptAsync("Labot uzdevumu", "Jauns CourseId:", initialValue: _selected.CourseId.ToString());

            if (!string.IsNullOrEmpty(newDesc) &&
                DateTime.TryParse(newDeadline, out DateTime ndl) &&
                int.TryParse(newCidStr, out int nCid))
            {
                try
                {
                    DatabaseHelper.UpdateAssignment(_selected.Id, newDesc, ndl, nCid);
                    LoadAssignments();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Kļūda", ex.Message, "OK");
                }
            }
        }

        private async void OnDeleteAssignmentClicked(object sender, EventArgs e)
        {
            if (_selected == null) return;

            bool confirm = await DisplayAlert("Dzēst uzdevumu?", _selected.TitleText, "Jā", "Nē");
            if (confirm)
            {
                try
                {
                    DatabaseHelper.DeleteAssignment(_selected.Id);
                    LoadAssignments();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Kļūda", ex.Message, "OK");
                }
            }
        }
    }
}
