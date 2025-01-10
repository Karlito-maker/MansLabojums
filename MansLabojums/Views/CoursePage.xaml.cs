using Microsoft.Maui.Controls;
using MansLabojums.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MansLabojums.Views
{
    public partial class CoursePage : ContentPage
    {
        // Markējam selectedCourse kā nullable, lai nav CS8618 brīdinājumu
        private (int Id, string CourseName, string TeacherName)? selectedCourse = null;

        // teachersList arī inicializējam, lai nav null
        private List<(int Id, string Name, string Surname, string Gender, string ContractDate)> teachersList = new();

        public CoursePage()
        {
            InitializeComponent();
            LoadCourses();
            LoadTeachers();
        }

        private void LoadCourses()
        {
            try
            {
                var courses = DatabaseHelper.GetCourses();
                CoursesListView.ItemsSource = courses;
            }
            catch (Exception ex)
            {
                DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }

        private void LoadTeachers()
        {
            try
            {
                teachersList = DatabaseHelper.GetTeachers();
            }
            catch (Exception ex)
            {
                DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }

        private void OnCourseSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;

            // e.SelectedItem sagaidām, ka ir (int, string, string)
            var item = (ValueTuple<int, string, string>)e.SelectedItem;
            selectedCourse = (item.Item1, item.Item2, item.Item3);
        }

        private async void OnAddCourseClicked(object sender, EventArgs e)
        {
            if (teachersList.Count == 0)
            {
                await DisplayAlert("Brīdinājums", "Nav pasniedzēju! Vispirms pievienojiet pasniedzējus.", "Labi");
                return;
            }

            string newCourseName = await DisplayPromptAsync("Jauns kurss", "Ieraksti kursa nosaukumu:");
            if (string.IsNullOrEmpty(newCourseName)) return;

            var teacherNames = teachersList.Select(t => $"{t.Name} {t.Surname}").ToArray();
            string chosenTeacher = await DisplayActionSheet("Izvēlies pasniedzēju", "Atcelt", null, teacherNames);
            if (chosenTeacher == "Atcelt" || string.IsNullOrEmpty(chosenTeacher)) return;

            var teacher = teachersList.FirstOrDefault(t => $"{t.Name} {t.Surname}" == chosenTeacher);
            if (teacher.Id == 0)
            {
                await DisplayAlert("Kļūda", "Neizdevās atrast izvēlēto pasniedzēju!", "Labi");
                return;
            }

            try
            {
                DatabaseHelper.AddCourse(newCourseName, teacher.Id);
                LoadCourses();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }

        private async void OnEditCourseClicked(object sender, EventArgs e)
        {
            if (!selectedCourse.HasValue)
            {
                await DisplayAlert("Brīdinājums", "Nav izvēlēts neviens kurss!", "Labi");
                return;
            }

            var current = selectedCourse.Value;

            string newCourseName = await DisplayPromptAsync("Labot kursu", "Kursa nosaukums:", initialValue: current.CourseName);
            if (string.IsNullOrEmpty(newCourseName)) return;

            var teacherNames = teachersList.Select(t => $"{t.Name} {t.Surname}").ToArray();
            string chosenTeacher = await DisplayActionSheet("Izvēlies pasniedzēju", "Atcelt", null, teacherNames);
            if (chosenTeacher == "Atcelt" || string.IsNullOrEmpty(chosenTeacher)) return;

            var teacher = teachersList.FirstOrDefault(t => $"{t.Name} {t.Surname}" == chosenTeacher);
            if (teacher.Id == 0)
            {
                await DisplayAlert("Kļūda", "Neizdevās atrast izvēlēto pasniedzēju!", "Labi");
                return;
            }

            try
            {
                DatabaseHelper.UpdateCourse(current.Id, newCourseName, teacher.Id);
                LoadCourses();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }

        private async void OnDeleteCourseClicked(object sender, EventArgs e)
        {
            if (!selectedCourse.HasValue)
            {
                await DisplayAlert("Brīdinājums", "Nav izvēlēts neviens kurss!", "Labi");
                return;
            }

            bool confirm = await DisplayAlert("Dzēst kursu", "Vai tiešām vēlaties dzēst šo kursu?", "Jā", "Nē");
            if (!confirm) return;

            try
            {
                DatabaseHelper.DeleteCourse(selectedCourse.Value.Id);
                LoadCourses();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }
    }
}

