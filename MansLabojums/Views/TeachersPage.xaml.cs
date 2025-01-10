using Microsoft.Maui.Controls;
using MansLabojums.Helpers;
using System;
using System.Linq;

namespace MansLabojums.Views
{
    public partial class TeachersPage : ContentPage
    {
        private (int Id, string Name, string Surname, string Gender, string ContractDate)? selectedTeacher;

        public TeachersPage()
        {
            InitializeComponent();
            LoadTeachers();
        }

        private void LoadTeachers()
        {
            try
            {
                var teachers = DatabaseHelper.GetTeachers();
                TeachersListView.ItemsSource = teachers
                    .Select(t => new
                    {
                        t.Id,
                        t.Name,
                        t.Surname,
                        t.Gender,
                        t.ContractDate
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }

        private void OnTeacherSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;
            dynamic teacher = e.SelectedItem;
            selectedTeacher = (teacher.Id, teacher.Name, teacher.Surname, teacher.Gender, teacher.ContractDate);
        }

        private async void OnAddTeacherClicked(object sender, EventArgs e)
        {
            string name = await DisplayPromptAsync("Jauns pasniedzējs", "Vārds:");
            if (string.IsNullOrEmpty(name)) return;

            string surname = await DisplayPromptAsync("Jauns pasniedzējs", "Uzvārds:");
            if (string.IsNullOrEmpty(surname)) return;

            string gender = await DisplayActionSheet("Dzimums", "Atcelt", null, "Male", "Female");
            if (gender == "Atcelt" || string.IsNullOrEmpty(gender)) return;

            string contractDateStr = await DisplayPromptAsync("Jauns pasniedzējs", "Līguma datums (YYYY-MM-DD):");
            if (string.IsNullOrEmpty(contractDateStr)) return;

            try
            {
                DatabaseHelper.AddTeacher(name, surname, gender, contractDateStr);
                LoadTeachers();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }

        private async void OnEditTeacherClicked(object sender, EventArgs e)
        {
            if (!selectedTeacher.HasValue)
            {
                await DisplayAlert("Brīdinājums", "Nav izvēlēts neviens pasniedzējs.", "Labi");
                return;
            }

            var current = selectedTeacher.Value;

            string newName = await DisplayPromptAsync("Labot pasniedzēju", "Jauns vārds:", initialValue: current.Name);
            if (string.IsNullOrEmpty(newName)) return;

            string newSurname = await DisplayPromptAsync("Labot pasniedzēju", "Jauns uzvārds:", initialValue: current.Surname);
            if (string.IsNullOrEmpty(newSurname)) return;

            string gender = await DisplayActionSheet("Dzimums", "Atcelt", null, "Male", "Female");
            if (gender == "Atcelt" || string.IsNullOrEmpty(gender)) return;

            string contractDateStr = await DisplayPromptAsync("Labot pasniedzēju", "Līguma datums (YYYY-MM-DD):", initialValue: current.ContractDate);
            if (string.IsNullOrEmpty(contractDateStr)) return;

            try
            {
                DatabaseHelper.UpdateTeacher(current.Id, newName, newSurname, gender, contractDateStr);
                LoadTeachers();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }

        private async void OnDeleteTeacherClicked(object sender, EventArgs e)
        {
            if (!selectedTeacher.HasValue)
            {
                await DisplayAlert("Brīdinājums", "Nav izvēlēts neviens pasniedzējs.", "Labi");
                return;
            }

            bool confirm = await DisplayAlert("Dzēst pasniedzēju", "Vai tiešām vēlaties dzēst šo pasniedzēju?", "Jā", "Nē");
            if (!confirm) return;

            try
            {
                DatabaseHelper.DeleteTeacher(selectedTeacher.Value.Id);
                LoadTeachers();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }
    }
}
