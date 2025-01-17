using MansLabojums.Helpers;
using Microsoft.Maui.Controls;
using System;

namespace MansLabojums.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnInitDbClicked(object sender, EventArgs e)
        {
            // Inicializē DB, ja nav jau
            try
            {
                DatabaseHelper.InitializeDatabase();
                DisplayAlert("Info", "Datubāze inicializēta!", "OK");
            }
            catch (Exception ex)
            {
                DisplayAlert("Kļūda", ex.Message, "OK");
            }
        }

        private void OnSeedDataClicked(object sender, EventArgs e)
        {
            // Ievadām testa datus, ja tie vēl nav
            try
            {
                DatabaseHelper.SeedData();
                DisplayAlert("Info", "Testa dati pievienoti!", "OK");
            }
            catch (Exception ex)
            {
                DisplayAlert("Kļūda", ex.Message, "OK");
            }
        }

        private void NavigateToStudentsPage(object sender, EventArgs e)
        {
            Navigation.PushAsync(new StudentsPage());
        }

        private void NavigateToTeachersPage(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TeachersPage());
        }

        private void NavigateToCoursesPage(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CoursesPage());
        }

        private void NavigateToAssignmentsPage(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AssignmentsPage());
        }

        private void NavigateToSubmissionsPage(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SubmissionsPage());
        }
    }
}
