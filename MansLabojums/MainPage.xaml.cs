using Microsoft.Maui.Controls;
using MansLabojums.Helpers;
using System;

namespace MansLabojums
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnInitDbClicked(object sender, EventArgs e)
        {
            try
            {
                DatabaseHelper.InitializeDatabase();
                await DisplayAlert("Info", "Datubāze inicializēta veiksmīgi!", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }

        private async void OnSeedDataClicked(object sender, EventArgs e)
        {
            try
            {
                DatabaseHelper.SeedData();
                await DisplayAlert("Info", "Testa dati pievienoti.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }

        private void NavigateToStudentsPage(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//StudentsPage");
        }

        private void NavigateToAssignmentsPage(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//AssignmentsPage");
        }

        private void NavigateToSubmissionsPage(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//SubmissionsPage");
        }

        private void NavigateToCoursesPage(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//CoursePage");
        }

        private void NavigateToTeachersPage(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//TeachersPage");
        }
    }
}
