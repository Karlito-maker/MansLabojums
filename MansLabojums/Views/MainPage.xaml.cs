/******************************************************
 * MansLabojums/Views/MainPage.xaml.cs
 ******************************************************/
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
            try
            {
                DatabaseHelper.InitializeDatabase();
                DisplayAlert("Info", "Datubāze inicializēta veiksmīgi!", "OK");
            }
            catch (Exception ex)
            {
                DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }

        private void OnSeedDataClicked(object sender, EventArgs e)
        {
            try
            {
                DatabaseHelper.SeedData();
                DisplayAlert("Info", "Testa dati pievienoti!", "OK");
            }
            catch (Exception ex)
            {
                DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }

        private void NavigateToStudentsPage(object sender, EventArgs e)
        {
            Navigation.PushAsync(new StudentsPage());
        }

        private void NavigateToAssignmentsPage(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AssignmentsPage());
        }

        private void NavigateToSubmissionsPage(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SubmissionsPage());
        }

        private void NavigateToCoursesPage(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CoursesPage());
        }

        private void NavigateToTeachersPage(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TeachersPage());
        }
    }
}
