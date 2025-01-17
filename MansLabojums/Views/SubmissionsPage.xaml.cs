using MansLabojums.Helpers;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MansLabojums.Views
{
    public partial class SubmissionsPage : ContentPage
    {
        // Modeļa apraksts, lai rādītu sarakstā
        public class SubmissionDisplay
        {
            public int Id { get; set; }
            public int AssignmentId { get; set; }
            public int StudentId { get; set; }
            public string AssignmentDescription { get; set; } = "";
            public string StudentName { get; set; } = "";
            public string SubmissionTime { get; set; } = "";
            public int Score { get; set; }

            public string DisplayTitle => $"[{Id}] {AssignmentDescription}";
            public string DisplayDetail => $"Students: {StudentName}, Score={Score}, {SubmissionTime}";
        }

        // Picker items
        public class AssignmentItem
        {
            public int Id { get; set; }
            public string DisplayText { get; set; } = ""; // "[1] Algebras.."
        }
        public class StudentItem
        {
            public int Id { get; set; }
            public string DisplayText { get; set; } = ""; // "[1] Pēteris Ozoliņš"
        }

        private ObservableCollection<SubmissionDisplay> _subs = new();
        private SubmissionDisplay _selectedSub;

        public ObservableCollection<AssignmentItem> AssignmentList { get; set; } = new();
        public ObservableCollection<StudentItem> StudentList { get; set; } = new();

        public AssignmentItem SelectedAssignment { get; set; }
        public StudentItem SelectedStudent { get; set; }

        public SubmissionsPage()
        {
            InitializeComponent();
            SubmissionsListView.ItemsSource = _subs;
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadSubmissions();
            LoadAssignments();
            LoadStudents();
        }

        // Ielasa submissionus
        private void LoadSubmissions()
        {
            _subs.Clear();
            _selectedSub = null;
            EditButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;

            var list = DatabaseHelper.GetSubmissionsWithIDs();
            foreach (var row in list)
            {
                _subs.Add(new SubmissionDisplay
                {
                    Id = (int)row["Id"],
                    AssignmentId = (int)row["AssignmentId"],
                    StudentId = (int)row["StudentId"],
                    AssignmentDescription = row["AssignmentDescription"].ToString()!,
                    StudentName = row["StudentName"].ToString()!,
                    SubmissionTime = row["SubmissionTime"].ToString()!,
                    Score = (int)row["Score"]
                });
            }
        }

        private void LoadAssignments()
        {
            AssignmentList.Clear();
            var assigns = DatabaseHelper.GetAssignments();
            foreach (var a in assigns)
            {
                AssignmentList.Add(new AssignmentItem
                {
                    Id = a.Id,
                    DisplayText = $"[{a.Id}] {a.Description}"
                });
            }
        }

        private void LoadStudents()
        {
            StudentList.Clear();
            var studs = DatabaseHelper.GetStudents();
            foreach (var s in studs)
            {
                StudentList.Add(new StudentItem
                {
                    Id = s.Id,
                    DisplayText = $"[{s.Id}] {s.Name} {s.Surname}"
                });
            }
        }

        private void OnSubmissionSelected(object sender, SelectedItemChangedEventArgs e)
        {
            _selectedSub = (SubmissionDisplay)e.SelectedItem;
            bool hasSel = (_selectedSub != null);
            EditButton.IsEnabled = hasSel;
            DeleteButton.IsEnabled = hasSel;
        }

        // Pievieno, izmantojot ID, bet dropdown nodrošina, ka ID eksistē
        private void OnAddSubmissionClicked(object sender, EventArgs e)
        {
            if (SelectedAssignment == null || SelectedStudent == null)
            {
                DisplayAlert("Kļūda", "Nepareizi atlasīts uzdevums vai students!", "OK");
                return;
            }

            if (!int.TryParse(ScoreEntry.Text, out int sc))
            {
                DisplayAlert("Kļūda", "Nederīgs score!", "OK");
                return;
            }

            try
            {
                DatabaseHelper.AddSubmissionByIds(SelectedAssignment.Id, SelectedStudent.Id, sc);
                ClearAddForm();
                LoadSubmissions();
            }
            catch (Exception ex)
            {
                DisplayAlert("Kļūda", ex.Message, "OK");
            }
        }

        private void ClearAddForm()
        {
            AssignmentPicker.SelectedItem = null;
            StudentPicker.SelectedItem = null;
            ScoreEntry.Text = "";
            SelectedAssignment = null;
            SelectedStudent = null;
        }

        private void OnCancelAddClicked(object sender, EventArgs e)
        {
            ClearAddForm();
        }

        // Rediģē – atļaujam tikai labot Score
        private async void OnEditSubmissionClicked(object sender, EventArgs e)
        {
            if (_selectedSub == null) return;

            string oldScore = _selectedSub.Score.ToString();
            string newScoreStr = await DisplayPromptAsync("Labot iesniegumu", "Jauns rezultāts:", initialValue: oldScore);
            if (int.TryParse(newScoreStr, out int newSc))
            {
                try
                {
                    DatabaseHelper.UpdateSubmission(_selectedSub.Id, newSc);
                    LoadSubmissions();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Kļūda", ex.Message, "OK");
                }
            }
        }

        private async void OnDeleteSubmissionClicked(object sender, EventArgs e)
        {
            if (_selectedSub == null) return;

            bool confirm = await DisplayAlert("Dzēst iesniegumu?",
                _selectedSub.AssignmentDescription,
                "Jā", "Nē");
            if (!confirm) return;

            try
            {
                DatabaseHelper.DeleteSubmission(_selectedSub.Id);
                LoadSubmissions();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kļūda", ex.Message, "OK");
            }
        }
    }
}
