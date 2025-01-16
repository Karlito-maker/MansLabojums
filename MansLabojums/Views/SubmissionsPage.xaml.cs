using MansLabojums.Helpers;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MansLabojums.Views
{
    public partial class SubmissionsPage : ContentPage
    {
        public class SubmissionDisplay
        {
            public int Id { get; set; }
            public string AssignmentDescription { get; set; } = "";
            public string StudentName { get; set; } = "";
            public string SubmissionTime { get; set; } = "";
            public int Score { get; set; }
            // lai var rādīt
            public string DisplayTitle => $"[{Id}] {AssignmentDescription}";
            public string DisplayDetail => $"Student: {StudentName}, Score={Score}, {SubmissionTime}";
        }

        // Picker: assignment
        public class AssignmentItem
        {
            public int Id { get; set; }
            public string AssignmentDesc { get; set; } = "";
        }
        // Picker: student
        public class StudentItem
        {
            public int Id { get; set; }
            public string StudentName { get; set; } = "";
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

        private void LoadSubmissions()
        {
            _subs.Clear();
            _selectedSub = null;
            EditButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;

            var list = DatabaseHelper.GetSubmissionsWithIDs();
            foreach (var dict in list)
            {
                _subs.Add(new SubmissionDisplay
                {
                    Id = (int)dict["Id"],
                    AssignmentDescription = dict["AssignmentDescription"].ToString()!,
                    StudentName = dict["StudentName"].ToString()!,
                    SubmissionTime = dict["SubmissionTime"].ToString()!,
                    Score = (int)dict["Score"]
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
                    AssignmentDesc = $"[{a.Id}] {a.Description}"
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
                    StudentName = $"[{s.Id}] {s.Name} {s.Surname}"
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
                DisplayAlert("Kļūda", "Ievadiet derīgu skaitli Score!", "OK");
                return;
            }

            try
            {
                // Mūsu DBHelper meklē uzdevumu pēc description, studentu pēc Name
                // Tāpēc atdalīsim desc => "Kustības vienādojumi" no "[2]" 
                string[] aSplit = SelectedAssignment.AssignmentDesc.Split(']');
                string pureDesc = (aSplit.Length > 1) ? aSplit[1].Trim() : "???";

                string[] sSplit = SelectedStudent.StudentName.Split(']');
                string pureStudent = (sSplit.Length > 1) ? sSplit[1].Trim() : "???";

                DatabaseHelper.AddSubmission(pureDesc, pureStudent, sc);
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

        private async void OnEditSubmissionClicked(object sender, EventArgs e)
        {
            if (_selectedSub == null) return;

            string oldScore = _selectedSub.Score.ToString();
            string newScoreStr = await DisplayPromptAsync("Labot iesniegumu", "Jauns rezultāts:", initialValue: oldScore);
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

