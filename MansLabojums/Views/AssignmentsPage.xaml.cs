/******************************************************
 * MansLabojums/Views/AssignmentsPage.xaml.cs
 ******************************************************/
using MansLabojums.Models;
using MansLabojums.Helpers;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;

namespace MansLabojums.Views
{
    public partial class AssignmentsPage : ContentPage
    {
        // Vietējais “view model” sarakstam
        public class AssignmentDisplay
        {
            public int Id { get; set; }
            public string Description { get; set; } = "";
            public DateTime Deadline { get; set; }
            public int CourseId { get; set; }

            public string DisplayTitle => $"ID: {Id}, {Description}";
            public string DisplayDetail => $"Termiņš: {Deadline:yyyy-MM-dd}, CourseID={CourseId}";
        }

        private ObservableCollection<AssignmentDisplay> _assignments = new();

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

        private void LoadAssignments()
        {
            _assignments.Clear();

            var list = DatabaseHelper.GetAssignments();
            foreach (var a in list)
            {
                _assignments.Add(new AssignmentDisplay
                {
                    Id = a.Id,
                    Description = a.Description,
                    Deadline = a.Deadline,
                    CourseId = a.CourseId
                });
            }
        }

        private async void OnAddAssignmentClicked(object sender, EventArgs e)
        {
            string desc = await DisplayPromptAsync("Jauns uzdevums", "Apraksts:");
            string dlStr = await DisplayPromptAsync("Jauns uzdevums", "Deadline (YYYY-MM-DD):");
            string cStr = await DisplayPromptAsync("Jauns uzdevums", "CourseId (skaitlis):");

            if (!string.IsNullOrEmpty(desc) &&
                DateTime.TryParse(dlStr, out DateTime dl) &&
                int.TryParse(cStr, out int cid))
            {
                try
                {
                    DatabaseHelper.AddAssignment(desc, dl, cid);
                    LoadAssignments();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Kļūda", $"Neizdevās pievienot uzdevumu: {ex.Message}", "OK");
                }
            }
            else
            {
                await DisplayAlert("Kļūda", "Nederīgi dati!", "OK");
            }
        }

        private async void OnEditAssignmentClicked(object sender, EventArgs e)
        {
            if (AssignmentsListView.SelectedItem is AssignmentDisplay selA)
            {
                string newDesc = await DisplayPromptAsync("Labot uzdevumu", "Jauns apraksts:", initialValue: selA.Description);
                string newDeadlineStr = await DisplayPromptAsync("Labot uzdevumu", "Jauns termiņš (YYYY-MM-DD):", initialValue: selA.Deadline.ToString("yyyy-MM-dd"));
                string newCidStr = await DisplayPromptAsync("Labot uzdevumu", "Jauns CourseId:", initialValue: selA.CourseId.ToString());

                if (!string.IsNullOrEmpty(newDesc) &&
                    DateTime.TryParse(newDeadlineStr, out DateTime newDl) &&
                    int.TryParse(newCidStr, out int newCid))
                {
                    try
                    {
                        DatabaseHelper.UpdateAssignment(selA.Id, newDesc, newDl, newCid);
                        LoadAssignments();
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Kļūda", $"Neizdevās labot uzdevumu: {ex.Message}", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Kļūda", "Nederīgi dati!", "OK");
                }
            }
            else
            {
                await DisplayAlert("Brīdinājums", "Nav izvēlēts neviens uzdevums!", "OK");
            }
        }

        private async void OnDeleteAssignmentClicked(object sender, EventArgs e)
        {
            if (AssignmentsListView.SelectedItem is AssignmentDisplay selA)
            {
                bool confirm = await DisplayAlert("Dzēst uzdevumu?", selA.Description, "Jā", "Nē");
                if (confirm)
                {
                    try
                    {
                        DatabaseHelper.DeleteAssignment(selA.Id);
                        LoadAssignments();
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Kļūda", $"Neizdevās dzēst uzdevumu: {ex.Message}", "OK");
                    }
                }
            }
            else
            {
                await DisplayAlert("Brīdinājums", "Nav izvēlēts neviens uzdevums!", "OK");
            }
        }
    }
}
