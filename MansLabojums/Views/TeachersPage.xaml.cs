/******************************************************
 * MansLabojums/Views/TeachersPage.xaml.cs
 ******************************************************/
using MansLabojums.Models;
using MansLabojums.Helpers;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;

namespace MansLabojums.Views
{
    public partial class TeachersPage : ContentPage
    {
        // Vietējais modelis, lai sarakstā parādītu:
        // * Skolotāja vārdu/uzvārdu
        // * Dzimumu, Līguma datumu
        // * Kursu sarakstu, ko pasniedz
        public class TeacherDisplay
        {
            public int Id { get; set; }
            public string TeacherFullName { get; set; } = "";
            public string Gender { get; set; } = "";
            public string ContractDateStr { get; set; } = "";
            public string CoursesText { get; set; } = ""; // Piemērs: "Matemātika, Fizika"
        }

        private ObservableCollection<TeacherDisplay> _teachers = new();

        public TeachersPage()
        {
            InitializeComponent();
            TeachersListView.ItemsSource = _teachers;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadTeachers();
        }

        private void LoadTeachers()
        {
            _teachers.Clear();

            // Iegūstam skolotāju sarakstu
            var teachers = DatabaseHelper.GetTeachers();
            foreach (var t in teachers)
            {
                // No DatabaseHelper iegūstam šī skolotāja kursus
                var teacherCourses = DatabaseHelper.GetCoursesByTeacherId(t.Id);
                string coursesText = teacherCourses.Count > 0
                    ? $"Kursi: {string.Join(", ", teacherCourses)}"
                    : "(Nav kursu)";

                _teachers.Add(new TeacherDisplay
                {
                    Id = t.Id,
                    TeacherFullName = $"{t.Name} {t.Surname}",
                    Gender = t.Gender,
                    ContractDateStr = $"Līguma datums: {t.ContractDate:yyyy-MM-dd}",
                    CoursesText = coursesText
                });
            }
        }

        private async void OnAddTeacherClicked(object sender, EventArgs e)
        {
            string name = await DisplayPromptAsync("Jauns pasniedzējs", "Vārds:");
            string surname = await DisplayPromptAsync("Jauns pasniedzējs", "Uzvārds:");
            string gender = await DisplayActionSheet("Dzimums:", "Atcelt", null, "Male", "Female");
            string dateStr = await DisplayPromptAsync("Jauns pasniedzējs", "Līguma datums (YYYY-MM-DD):");

            if (!string.IsNullOrEmpty(name) &&
                !string.IsNullOrEmpty(surname) &&
                (gender == "Male" || gender == "Female") &&
                DateTime.TryParse(dateStr, out DateTime cDate))
            {
                try
                {
                    DatabaseHelper.AddTeacher(name, surname, gender, cDate.ToString("yyyy-MM-dd"));
                    LoadTeachers();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Kļūda", ex.Message, "Labi");
                }
            }
            else
            {
                await DisplayAlert("Kļūda", "Nav korekti dati!", "Labi");
            }
        }

        private async void OnEditTeacherClicked(object sender, EventArgs e)
        {
            if (TeachersListView.SelectedItem is TeacherDisplay selTeacher)
            {
                // Izvelkam esošos datus
                string[] nm = selTeacher.TeacherFullName.Split(' ');
                string oldName = (nm.Length > 0) ? nm[0] : "";
                string oldSurname = (nm.Length > 1) ? nm[1] : "";
                string oldGender = selTeacher.Gender;
                string oldDateStr = selTeacher.ContractDateStr.Replace("Līguma datums: ", "");

                string newName = await DisplayPromptAsync("Labot pasniedzēju", "Vārds:", initialValue: oldName);
                string newSurname = await DisplayPromptAsync("Labot pasniedzēju", "Uzvārds:", initialValue: oldSurname);
                string newGender = await DisplayActionSheet("Dzimums:", "Atcelt", null, "Male", "Female");
                string newDateStr = await DisplayPromptAsync("Labot pasniedzēju", "Līguma datums (YYYY-MM-DD):", initialValue: oldDateStr);

                if (!string.IsNullOrEmpty(newName) &&
                    !string.IsNullOrEmpty(newSurname) &&
                    (newGender == "Male" || newGender == "Female") &&
                    DateTime.TryParse(newDateStr, out DateTime newDate))
                {
                    try
                    {
                        DatabaseHelper.UpdateTeacher(selTeacher.Id, newName, newSurname, newGender, newDate.ToString("yyyy-MM-dd"));
                        LoadTeachers();
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Kļūda", ex.Message, "Labi");
                    }
                }
                else
                {
                    await DisplayAlert("Kļūda", "Nav korekti dati!", "Labi");
                }
            }
            else
            {
                await DisplayAlert("Brīdinājums", "Nav izvēlēts neviens pasniedzējs!", "Labi");
            }
        }

        private async void OnDeleteTeacherClicked(object sender, EventArgs e)
        {
            if (TeachersListView.SelectedItem is TeacherDisplay selTeacher)
            {
                bool confirm = await DisplayAlert("Dzēst pasniedzēju?", selTeacher.TeacherFullName, "Jā", "Nē");
                if (confirm)
                {
                    try
                    {
                        DatabaseHelper.DeleteTeacher(selTeacher.Id);
                        LoadTeachers();
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Kļūda", ex.Message, "Labi");
                    }
                }
            }
            else
            {
                await DisplayAlert("Brīdinājums", "Nav izvēlēts neviens pasniedzējs!", "Labi");
            }
        }
    }
}


