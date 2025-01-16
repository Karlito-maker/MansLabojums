/******************************************************
 * MansLabojums/Views/StudentsPage.xaml.cs
 * Pēdējā (jaunākā) pilnā versija
 ******************************************************/
using MansLabojums.Helpers;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;

namespace MansLabojums.Views
{
    public partial class StudentsPage : ContentPage
    {
        // Modelis, ko rādām sarakstā
        public class StudentDisplay
        {
            public int Id { get; set; }
            public string FullName { get; set; } = "";
            public string IdNumberText { get; set; } = "";
        }

        private ObservableCollection<StudentDisplay> _students = new();
        private StudentDisplay _selectedStudent; // pašlaik atlasītais

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

        // Atjauno studentu sarakstu
        private void LoadStudents()
        {
            _students.Clear();
            // No DatabaseHelper dabūjam studentu sarakstu
            var list = DatabaseHelper.GetStudents();
            foreach (var s in list)
            {
                _students.Add(new StudentDisplay
                {
                    Id = s.Id,
                    FullName = s.Name + " " + s.Surname,
                    IdNumberText = "ID# " + s.StudentIdNumber
                });
            }

            // Atiestatām atlasīto studentu
            StudentsListView.SelectedItem = null;
            EditButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            _selectedStudent = null;
        }

        // Kad lietotājs sarakstā izvēlas studentu
        private void OnStudentSelected(object sender, SelectedItemChangedEventArgs e)
        {
            _selectedStudent = (StudentDisplay)e.SelectedItem;
            bool hasSelection = (_selectedStudent != null);

            EditButton.IsEnabled = hasSelection;
            DeleteButton.IsEnabled = hasSelection;
        }

        // Pievienot jaunu studentu
        private void OnAddStudentClicked(object sender, EventArgs e)
        {
            string name = NameEntry.Text?.Trim();
            string surname = SurnameEntry.Text?.Trim();
            string gender = GenderPicker.SelectedItem as string;
            string idNumStr = IdNumberEntry.Text?.Trim();

            // Pārbaudām vai ievades lauki ir korekti
            if (string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(surname) ||
                string.IsNullOrEmpty(gender) ||
                string.IsNullOrEmpty(idNumStr) ||
                !int.TryParse(idNumStr, out int sidNum))
            {
                DisplayAlert("Kļūda", "Nepareizi ievadīti dati!", "OK");
                return;
            }

            try
            {
                DatabaseHelper.AddStudent(name, surname, gender, sidNum);
                ClearAddForm();
                LoadStudents();
            }
            catch (Exception ex)
            {
                DisplayAlert("Kļūda", $"Neizdevās pievienot studentu: {ex.Message}", "OK");
            }
        }

        // Iztīram formu
        private void ClearAddForm()
        {
            NameEntry.Text = "";
            SurnameEntry.Text = "";
            GenderPicker.SelectedItem = null;
            IdNumberEntry.Text = "";
        }

        // “Cancel” poga formā
        private void OnCancelAddClicked(object sender, EventArgs e)
        {
            ClearAddForm();
        }

        // Labot atlasīto studentu
        private async void OnEditStudentClicked(object sender, EventArgs e)
        {
            if (_selectedStudent == null) return;

            string[] nm = _selectedStudent.FullName.Split(' ');
            string oldName = (nm.Length > 0) ? nm[0] : "";
            string oldSurname = (nm.Length > 1) ? nm[1] : "";

            string newName = await DisplayPromptAsync("Labot studentu", "Vārds:", initialValue: oldName);
            string newSurname = await DisplayPromptAsync("Labot studentu", "Uzvārds:", initialValue: oldSurname);

            if (!string.IsNullOrEmpty(newName) && !string.IsNullOrEmpty(newSurname))
            {
                try
                {
                    DatabaseHelper.UpdateStudent(_selectedStudent.Id, newName, newSurname);
                    LoadStudents();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Kļūda", ex.Message, "OK");
                }
            }
        }

        // Dzēst atlasīto studentu
        private async void OnDeleteStudentClicked(object sender, EventArgs e)
        {
            if (_selectedStudent == null) return;

            bool confirm = await DisplayAlert("Dzēst studentu?", _selectedStudent.FullName, "Jā", "Nē");
            if (confirm)
            {
                try
                {
                    DatabaseHelper.DeleteStudent(_selectedStudent.Id);
                    LoadStudents();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Kļūda", ex.Message, "OK");
                }
            }
        }
    }
}

