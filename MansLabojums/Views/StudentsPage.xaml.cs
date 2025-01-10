using Microsoft.Maui.Controls;
using MansLabojums.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MansLabojums.Views
{
    public partial class StudentsPage : ContentPage
    {
        private (int Id, string Name, string Surname, string Gender, int StudentIdNumber)? selectedStudent;

        public StudentsPage()
        {
            InitializeComponent();
            LoadStudents();
        }

        private void LoadStudents()
        {
            try
            {
                var students = DatabaseHelper.GetStudents();

                // Pievienojam .BindingContext tieši ListView, vai varam ielikt vienkārši itemsource
                StudentsListView.ItemsSource = students
                    .Select(s => new
                    {
                        s.Id,
                        s.Name,
                        s.Surname,
                        s.Gender,
                        s.StudentIdNumber
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }

        private void OnStudentSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            dynamic item = e.SelectedItem;
            selectedStudent = (item.Id, item.Name, item.Surname, item.Gender, item.StudentIdNumber);
        }

        private async void OnAddStudentClicked(object sender, EventArgs e)
        {
            string name = await DisplayPromptAsync("Pievienot studentu", "Ievadiet vārdu:");
            if (string.IsNullOrEmpty(name)) return;

            string surname = await DisplayPromptAsync("Pievienot studentu", "Ievadiet uzvārdu:");
            if (string.IsNullOrEmpty(surname)) return;

            string gender = await DisplayActionSheet("Izvēlieties dzimumu", "Atcelt", null, "Male", "Female");
            if (gender == "Atcelt" || string.IsNullOrEmpty(gender)) return;

            string studentIdStr = await DisplayPromptAsync("Pievienot studentu", "Ievadiet studenta ID:");
            if (!int.TryParse(studentIdStr, out int studentIdNumber))
            {
                await DisplayAlert("Kļūda", "Nepareizs skaitlis!", "Labi");
                return;
            }

            try
            {
                DatabaseHelper.AddStudent(name, surname, gender, studentIdNumber);
                LoadStudents();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }

        private async void OnEditStudentClicked(object sender, EventArgs e)
        {
            if (!selectedStudent.HasValue)
            {
                await DisplayAlert("Brīdinājums", "Nav izvēlēts neviens students.", "Labi");
                return;
            }

            var current = selectedStudent.Value;
            string newName = await DisplayPromptAsync("Labot studentu", "Jauns vārds:", initialValue: current.Name);
            if (string.IsNullOrEmpty(newName)) return;

            string newSurname = await DisplayPromptAsync("Labot studentu", "Jauns uzvārds:", initialValue: current.Surname);
            if (string.IsNullOrEmpty(newSurname)) return;

            try
            {
                DatabaseHelper.UpdateStudent(current.Id, newName, newSurname);
                LoadStudents();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }

        private async void OnDeleteStudentClicked(object sender, EventArgs e)
        {
            if (!selectedStudent.HasValue)
            {
                await DisplayAlert("Brīdinājums", "Nav izvēlēts neviens students.", "Labi");
                return;
            }

            bool confirm = await DisplayAlert("Dzēst studentu", "Vai tiešām vēlaties dzēst šo studentu?", "Jā", "Nē");
            if (!confirm) return;

            try
            {
                DatabaseHelper.DeleteStudent(selectedStudent.Value.Id);
                LoadStudents();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }
    }
}
