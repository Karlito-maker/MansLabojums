using MansLabojums.Helpers;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MansLabojums.Views
{
    public partial class StudentsPage : ContentPage
    {
        // Modelis saraksta attēlošanai
        public class StudentDisplay
        {
            public int Id { get; set; }
            public string FullName { get; set; } = "";
            public string IdNumberText { get; set; } = "";
        }

        private ObservableCollection<StudentDisplay> _students = new();
        private StudentDisplay _selectedStudent;

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

        // Nolasām studentus no DB un ieliekam sarakstā
        private void LoadStudents()
        {
            _students.Clear();
            _selectedStudent = null;
            EditButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;

            var list = DatabaseHelper.GetStudents(); // atgriež List<Student>
            foreach (var s in list)
            {
                _students.Add(new StudentDisplay
                {
                    Id = s.Id,
                    FullName = s.Name + " " + s.Surname,
                    IdNumberText = "ID# " + s.StudentIdNumber
                });
            }
        }

        private void OnStudentSelected(object sender, SelectedItemChangedEventArgs e)
        {
            _selectedStudent = (StudentDisplay)e.SelectedItem;
            bool hasSel = (_selectedStudent != null);
            EditButton.IsEnabled = hasSel;
            DeleteButton.IsEnabled = hasSel;
        }

        // Pievienojam jaunu studentu
        private void OnAddStudentClicked(object sender, EventArgs e)
        {
            string name = NameEntry.Text?.Trim();
            string surname = SurnameEntry.Text?.Trim();
            string gender = GenderPicker.SelectedItem as string;
            string idNumStr = IdNumberEntry.Text?.Trim();

            // Pārbaudām ievades laukus
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

        private void ClearAddForm()
        {
            NameEntry.Text = "";
            SurnameEntry.Text = "";
            GenderPicker.SelectedItem = null;
            IdNumberEntry.Text = "";
        }

        private void OnCancelAddClicked(object sender, EventArgs e)
        {
            ClearAddForm();
        }

        private async void OnEditStudentClicked(object sender, EventArgs e)
        {
            if (_selectedStudent == null) return;

            // Pašreizējie lauki
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

        // Dzēšam studentu, pirms tam dzēšam visas Submissions, kas atsaucas uz šo studentu
        private async void OnDeleteStudentClicked(object sender, EventArgs e)
        {
            if (_selectedStudent == null) return;

            bool confirm = await DisplayAlert(
                "Dzēst studentu?",
                $"Vai tiešām vēlaties dzēst: {_selectedStudent.FullName}?",
                "Jā", "Nē");

            if (confirm)
            {
                try
                {
                    // 1) Iegūstam visus Submissions
                    var subs = DatabaseHelper.GetSubmissionsWithIDs();
                    // 2) Atliek atrast tos, kuru StudentId = _selectedStudent.Id
                    foreach (var dict in subs)
                    {
                        int stId = (int)dict["StudentId"];
                        int subId = (int)dict["Id"];
                        if (stId == _selectedStudent.Id)
                        {
                            // Dzēšam iesniegumu
                            DatabaseHelper.DeleteSubmission(subId);
                        }
                    }

                    // Tagad droši var dzēst studentu
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
