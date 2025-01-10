using Microsoft.Maui.Controls;
using MansLabojums.Helpers;
using System;
using System.Collections.Generic;

namespace MansLabojums.Views
{
    public partial class SubmissionsPage : ContentPage
    {
        // Atļaujam selectedSubmission būt null, lai nav CS8618
        private Dictionary<string, object>? selectedSubmission = null;

        public SubmissionsPage()
        {
            InitializeComponent();
            LoadSubmissions();
        }

        private void LoadSubmissions()
        {
            try
            {
                var subs = DatabaseHelper.GetSubmissions();
                SubmissionsListView.ItemsSource = subs;
            }
            catch (Exception ex)
            {
                DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }

        private void OnSubmissionSelected(object sender, SelectedItemChangedEventArgs e)
        {
            selectedSubmission = e.SelectedItem as Dictionary<string, object>;
        }

        private async void OnAddSubmissionClicked(object sender, EventArgs e)
        {
            string assignmentDescription = await DisplayPromptAsync("Pievienot iesniegumu", "Uzdevuma apraksts:");
            if (string.IsNullOrEmpty(assignmentDescription)) return;

            string studentName = await DisplayPromptAsync("Pievienot iesniegumu", "Studenta vārds:");
            if (string.IsNullOrEmpty(studentName)) return;

            string scoreStr = await DisplayPromptAsync("Pievienot iesniegumu", "Ievadiet rezultātu:");
            if (!int.TryParse(scoreStr, out int score))
            {
                await DisplayAlert("Kļūda", "Nepareizs skaitlis!", "Labi");
                return;
            }

            try
            {
                DatabaseHelper.AddSubmission(assignmentDescription, studentName, score);
                LoadSubmissions();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }

        private async void OnEditSubmissionClicked(object sender, EventArgs e)
        {
            if (selectedSubmission == null)
            {
                await DisplayAlert("Brīdinājums", "Nav izvēlēts neviens iesniegums.", "Labi");
                return;
            }

            int submissionId = (int)selectedSubmission["Id"];
            string oldScore = selectedSubmission["Score"].ToString();

            string newScoreStr = await DisplayPromptAsync("Labot iesniegumu", "Jauns rezultāts:", initialValue: oldScore);
            if (!int.TryParse(newScoreStr, out int newScore))
            {
                await DisplayAlert("Kļūda", "Nepareizs skaitlis!", "Labi");
                return;
            }

            try
            {
                DatabaseHelper.UpdateSubmission(submissionId, newScore);
                LoadSubmissions();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }

        private async void OnDeleteSubmissionClicked(object sender, EventArgs e)
        {
            if (selectedSubmission == null)
            {
                await DisplayAlert("Brīdinājums", "Nav izvēlēts neviens iesniegums.", "Labi");
                return;
            }

            int submissionId = (int)selectedSubmission["Id"];
            bool confirm = await DisplayAlert("Dzēst iesniegumu", "Vai tiešām vēlaties dzēst šo iesniegumu?", "Jā", "Nē");
            if (!confirm) return;

            try
            {
                DatabaseHelper.DeleteSubmission(submissionId);
                LoadSubmissions();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }
    }
}
