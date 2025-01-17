/******************************************************
 * MansLabojums/Views/AssignmentsPage.xaml.cs
 * Lapā var pievienot, labot un dzēst uzdevumus.
 * Lai var dzēst "testa datus", pirms assignment dzēšanas
 * tiks dzēsti visas ar to saistītās submissions (code cascade).
 ******************************************************/
using MansLabojums.Helpers;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MansLabojums.Views
{
    public partial class AssignmentsPage : ContentPage
    {
        // Modelis, lai rādītu sarakstā
        public class AssignmentDisplay
        {
            public int Id { get; set; }
            public string TitleText { get; set; } = "";   // Piem. "[1] Algebras mājas darbs"
            public string DetailText { get; set; } = "";  // Piem. "Termiņš: 2024-12-31, CourseId=1"
            public int CourseId { get; set; }
        }

        private ObservableCollection<AssignmentDisplay> _assignments = new();
        private AssignmentDisplay _selectedAssignment;

        public AssignmentsPage()
        {
            InitializeComponent();
            AssignmentsListView.ItemsSource = _assignments;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadAssignments();
        }

        // Nolasām visus uzdevumus no DB un parādām sarakstā
        private void LoadAssignments()
        {
            _assignments.Clear();
            _selectedAssignment = null;
            EditButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;

            var list = DatabaseHelper.GetAssignments();
            // Piemēram, satur: Id, Description, Deadline, CourseId
            foreach (var a in list)
            {
                // Taisām "TitleText" un "DetailText"
                string tt = $"[{a.Id}] {a.Description}";
                string dt = $"Termiņš: {a.Deadline:yyyy-MM-dd}, CourseId={a.CourseId}";
                _assignments.Add(new AssignmentDisplay
                {
                    Id = a.Id,
                    TitleText = tt,
                    DetailText = dt,
                    CourseId = a.CourseId
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

        // Pievieno jaunu assignment
        private void OnAddAssignmentClicked(object sender, EventArgs e)
        {
            string desc = DescriptionEntry.Text?.Trim();
            string dlStr = DeadlineEntry.Text?.Trim();
            string cIdStr = CourseIdEntry.Text?.Trim();

            // Pārbaudām ievadītos laukus
            if (string.IsNullOrEmpty(desc) ||
                string.IsNullOrEmpty(dlStr) ||
                !DateTime.TryParse(dlStr, out DateTime dl) ||
                string.IsNullOrEmpty(cIdStr) ||
                !int.TryParse(cIdStr, out int cid))
            {
                DisplayAlert("Kļūda", "Nepareizi ievadīti lauki Uzdevumam!", "OK");
                return;
            }

            try
            {
                // Pievienojam DB
                DatabaseHelper.AddAssignment(desc, dl, cid);
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
            CourseIdEntry.Text = "";
        }

        private void OnCancelAddClicked(object sender, EventArgs e)
        {
            ClearAddForm();
        }

        private async void OnEditAssignmentClicked(object sender, EventArgs e)
        {
            if (_selectedAssignment == null) return;

            // Šeit varam ierosināt jauno aprakstu / termiņu / courseId
            // Sadalām TitleText "[1] Algebras mājas darbs"?
            // Vienkāršībai: 
            string oldDesc = _selectedAssignment.TitleText;
            string newDesc = await DisplayPromptAsync("Labot uzdevumu", "Jauns apraksts:", initialValue: oldDesc);

            // Deadline
            string[] detailSplit = _selectedAssignment.DetailText.Split(',');
            // "Termiņš: 2024-12-31" -> u.c.
            string oldDl = "2024-12-31";
            int oldCid = _selectedAssignment.CourseId;
            // Mēģināsim izlobīt, bet drošāk vienkārši:
            string newDlStr = await DisplayPromptAsync("Labot uzdevumu", "Jauns termiņš (YYYY-MM-DD):", initialValue: "2024-12-31");
            string newCidStr = await DisplayPromptAsync("Labot uzdevumu", "Jauns CourseId:", initialValue: oldCid.ToString());

            if (!string.IsNullOrEmpty(newDesc) &&
                DateTime.TryParse(newDlStr, out DateTime newDl) &&
                int.TryParse(newCidStr, out int newCid))
            {
                try
                {
                    // Pārcērtam "[1]" no newDesc, ja vajag
                    int bracketPos = newDesc.IndexOf(']');
                    string pureDesc = (bracketPos >= 0 && bracketPos < newDesc.Length - 1)
                        ? newDesc.Substring(bracketPos + 1).Trim()
                        : newDesc;

                    DatabaseHelper.UpdateAssignment(_selectedAssignment.Id, pureDesc, newDl, newCid);
                    LoadAssignments();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Kļūda", ex.Message, "OK");
                }
            }
        }

        // Dzēšam assignment, pirms tam dzēšam submissions, kas atsaucas uz to assignment
        private async void OnDeleteAssignmentClicked(object sender, EventArgs e)
        {
            if (_selectedAssignment == null) return;

            bool confirm = await DisplayAlert(
                "Dzēst uzdevumu?",
                $"Dzēst: {_selectedAssignment.TitleText}",
                "Jā", "Nē");

            if (confirm)
            {
                try
                {
                    // 1) Iegūstam visus Submissions
                    var allSubs = DatabaseHelper.GetSubmissionsWithIDs();
                    // 2) Dzēšam tās, kas atsaucas uz _selectedAssignment.Id
                    foreach (var sdict in allSubs)
                    {
                        int assId = (int)sdict["AssignmentId"];
                        int subId = (int)sdict["Id"];
                        if (assId == _selectedAssignment.Id)
                        {
                            // Dzēšam submission
                            DatabaseHelper.DeleteSubmission(subId);
                        }
                    }
                    // 3) Tagad var droši dzēst pašu assignment
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
}
