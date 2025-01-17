/******************************************************
 * MansLabojums/Views/SubmissionsPage.xaml.cs
 * Pilnīgs variants ar ID pieeju Submissions
 ******************************************************/
using Microsoft.Maui.Controls;
using MansLabojums.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MansLabojums.Views
{
    public partial class SubmissionsPage : ContentPage
    {
        // Dati sarakstam
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
            public string DisplayDetail => $"Student: {StudentName}, Score={Score}, {SubmissionTime}";
        }

        // Picker items
        public class AssignmentItem
        {
            public int Id { get; set; }
            public string DisplayText { get; set; } = ""; // ex: "[1] Algebras mājas darbs"
        }
        public class StudentItem
        {
            public int Id { get; set; }
            public string DisplayText { get; set; } = ""; // ex: "[2] Ilze Liepa"
        }

        // Kolekcijas
        private ObservableCollection<SubmissionDisplay> _subs = new();
        public ObservableCollection<AssignmentItem> AssignmentList { get; set; } = new();
        public ObservableCollection<StudentItem> StudentList { get; set; } = new();

        // Atlase
        private SubmissionDisplay _selectedSub;

        // Atlase no pickeriem
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

        // Ielasa esošos Submissions no DB
        private void LoadSubmissions()
        {
            _subs.Clear();
            _selectedSub = null;
            EditButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;

            var list = DatabaseHelper.GetSubmissionsWithIDs();
            // satur: "Id", "AssignmentId", "StudentId", "AssignmentDescription", "StudentName", "SubmissionTime", "Score"

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

        // Ielasa assignment ID un description, lai var atlasīt
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

        // Ielasa studentus
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

        // Pievienot, izmantojot ID – drošāk, nekā ar vārdiem
        private void OnAddSubmissionClicked(object sender, EventArgs e)
        {
            if (SelectedAssignment == null || SelectedStudent == null)
            {
                DisplayAlert("Kļūda", "Lūdzu izvēlieties uzdevumu un studentu!", "OK");
                return;
            }

            string scStr = ScoreEntry.Text?.Trim();
            if (!int.TryParse(scStr, out int sc))
            {
                DisplayAlert("Kļūda", "Ievadiet derīgu skaitlisku Score!", "OK");
                return;
            }

            try
            {
                // Tagad tieši ID:
                DatabaseHelper.AddSubmissionByIds(
                    SelectedAssignment.Id,
                    SelectedStudent.Id,
                    sc
                );

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

        // Labot – šeit tikai Score labojam
        private async void OnEditSubmissionClicked(object sender, EventArgs e)
        {
            if (_selectedSub == null) return;

            string oldScoreStr = _selectedSub.Score.ToString();
            string newScoreStr = await DisplayPromptAsync("Labot iesniegumu", "Jauns rezultāts:", initialValue: oldScoreStr);

            if (int.TryParse(newScoreStr, out int newScr))
            {
                try
                {
                    DatabaseHelper.UpdateSubmission(_selectedSub.Id, newScr);
                    LoadSubmissions();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Kļūda", ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Kļūda", "Nederīgs skaitlis!", "OK");
            }
        }

        private async void OnDeleteSubmissionClicked(object sender, EventArgs e)
        {
            if (_selectedSub == null) return;

            bool confirm = await DisplayAlert("Dzēst iesniegumu?", _selectedSub.AssignmentDescription, "Jā", "Nē");
            if (confirm)
            {
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
}

