/******************************************************
 * MansLabojums/Views/StudentsPage.xaml.cs
 ******************************************************/
using MansLabojums.Models;
using MansLabojums.Helpers;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;

namespace MansLabojums.Views
{
    public partial class StudentsPage : ContentPage
    {
        // Vietējais modelis sarakstam
        public class StudentDisplay
        {
            public int Id { get; set; }
            public string FullName { get; set; } = "";
            public string Gender { get; set; } = "";
            public string StudentIdNumberStr { get; set; } = "";
        }

        private ObservableCollection<StudentDisplay> _students = new();

        public StudentsPage()
        {
            InitializeComponent();
            StudentsListView.ItemsSource = _students;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadStudents();
        }

        private void LoadStudents()
        {
            _students.Clear();

            // Pieņemam, ka DatabaseHelper.GetStudents() atgriež List<Student>
            var list = DatabaseHelper.GetStudents();
            foreach (var st in list)
            {
                _students.Add(new StudentDisplay
                {
                    Id = st.Id,
                    FullName = $"{st.Name} {st.Surname}",
                    Gender = st.Gender,
                    StudentIdNumberStr = $"ID#: {st.StudentIdNumber}"
                });
            }
        }

        private async void OnAddStudentClicked(object sender, EventArgs e)
        {
            string name = await DisplayPromptAsync("Jauns students", "Vārds:");
            string surname = await DisplayPromptAsync("Jauns students", "Uzvārds:");
            string gender = await DisplayActionSheet("Dzimums:", "Atcelt", null, "Male", "Female");
            string idNumStr = await DisplayPromptAsync("Jauns students", "StudentIdNumber (skaitlis):");

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(surname) &&
                (gender == "Male" || gender == "Female") &&
                int.TryParse(idNumStr, out int sid))
            {
                try
                {
                    DatabaseHelper.AddStudent(name, surname, gender, sid);
                    LoadStudents();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Kļūda", "Neizdevās pievienot studentu: " + ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Kļūda", "Nederīgi dati!", "OK");
            }
        }

        private async void OnEditStudentClicked(object sender, EventArgs e)
        {
            if (StudentsListView.SelectedItem is StudentDisplay selStudent)
            {
                string[] nm = selStudent.FullName.Split(' ');
                string oldName = nm.Length > 0 ? nm[0] : "";
                string oldSurname = nm.Length > 1 ? nm[1] : "";
                string newName = await DisplayPromptAsync("Labot studentu", "Jauns vārds:", initialValue: oldName);
                string newSurname = await DisplayPromptAsync("Labot studentu", "Jauns uzvārds:", initialValue: oldSurname);

                if (!string.IsNullOrEmpty(newName) && !string.IsNullOrEmpty(newSurname))
                {
                    try
                    {
                        DatabaseHelper.UpdateStudent(selStudent.Id, newName, newSurname);
                        LoadStudents();
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Kļūda", $"Neizdevās labot: {ex.Message}", "OK");
                    }
                }
            }
            else
            {
                await DisplayAlert("Brīdinājums", "Nav izvēlēts neviens students!", "OK");
            }
        }

        private async void OnDeleteStudentClicked(object sender, EventArgs e)
        {
            if (StudentsListView.SelectedItem is StudentDisplay selStudent)
            {
                bool confirm = await DisplayAlert("Dzēst studentu?", selStudent.FullName, "Jā", "Nē");
                if (confirm)
                {
                    try
                    {
                        DatabaseHelper.DeleteStudent(selStudent.Id);
                        LoadStudents();
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Kļūda", $"Neizdevās dzēst studentu: {ex.Message}", "OK");
                    }
                }
            }
            else
            {
                await DisplayAlert("Brīdinājums", "Nav izvēlēts neviens students!", "OK");
            }
        }
    }
}






