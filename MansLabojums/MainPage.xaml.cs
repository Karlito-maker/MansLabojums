using Microsoft.Maui.Controls;
using MansLabojums.Helpers;
using System.Threading.Tasks;

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
            await InitializeDatabaseAsync();
        }

        private async void OnSeedDataClicked(object sender, EventArgs e)
        {
            await SeedDataAsync();
        }

        private async void NavigateToStudentsPage(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(Views.StudentsPage));
        }

        private async void NavigateToAssignmentsPage(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(Views.AssignmentsPage));
        }

        private async void NavigateToSubmissionsPage(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(Views.SubmissionsPage));
        }

        private async void NavigateToCoursesPage(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(Views.CoursePage));
        }

        private async void NavigateToTeachersPage(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(Views.TeachersPage));
        }

        private async Task InitializeDatabaseAsync()
        {
            await DatabaseHelper.InitializeDatabase();
            await DisplayAlert("Datubāze", "Datubāze inicializēta veiksmīgi!", "OK");
        }

        private async Task SeedDataAsync()
        {
            await DatabaseHelper.SeedDataAsync();
            await DisplayAlert("Testa dati", "Testa dati ievadīti veiksmīgi!", "OK");
        }
    }
}











