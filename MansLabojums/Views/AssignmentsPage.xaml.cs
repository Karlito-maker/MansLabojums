using Microsoft.Maui.Controls;
using MansLabojums.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MansLabojums.Views
{
    public partial class AssignmentsPage : ContentPage
    {
        // Atļaujam selectedAssignment būt null
        private Dictionary<string, object>? selectedAssignment = null;

        // Sākotnēji definējam allCourses kā jaunu, tukšu listi, lai nav brīdinājumu
        private List<(int Id, string CourseName, string TeacherName)> allCourses = new();

        public AssignmentsPage()
        {
            InitializeComponent();
            LoadAssignments();
            LoadCourses();
        }

        private void LoadAssignments(int? courseId = null)
        {
            try
            {
                var assignments = DatabaseHelper.GetAssignments(courseId);
                AssignmentsListView.ItemsSource = assignments;
            }
            catch (Exception ex)
            {
                DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }

        private void LoadCourses()
        {
            try
            {
                allCourses = DatabaseHelper.GetCourses();
            }
            catch (Exception ex)
            {
                DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }

        private void OnAssignmentSelected(object sender, SelectedItemChangedEventArgs e)
        {
            selectedAssignment = e.SelectedItem as Dictionary<string, object>;
        }

        private async void OnAddAssignmentClicked(object sender, EventArgs e)
        {
            if (allCourses.Count == 0)
            {
                await DisplayAlert("Brīdinājums", "Vispirms pievienojiet kursus!", "Labi");
                return;
            }

            string[] courseNames = allCourses.Select(c => c.CourseName).ToArray();
            string chosen = await DisplayActionSheet("Izvēlies kursu", "Atcelt", null, courseNames);
            if (chosen == "Atcelt" || string.IsNullOrEmpty(chosen)) return;

            var chosenCourse = allCourses.FirstOrDefault(c => c.CourseName == chosen);
            if (chosenCourse.Id == 0)
            {
                await DisplayAlert("Kļūda", "Nav atrasts izvēlētais kurss.", "Labi");
                return;
            }

            string description = await DisplayPromptAsync("Jauns uzdevums", "Uzdevuma apraksts:");
            if (string.IsNullOrEmpty(description)) return;

            string deadlineStr = await DisplayPromptAsync("Jauns uzdevums", "Termiņš (YYYY-MM-DD):");
            if (!DateTime.TryParse(deadlineStr, out DateTime deadline))
            {
                await DisplayAlert("Kļūda", "Nepareizs datuma formāts!", "Labi");
                return;
            }

            try
            {
                DatabaseHelper.AddAssignment(description, deadline, chosenCourse.Id);
                LoadAssignments();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }

        private async void OnEditAssignmentClicked(object sender, EventArgs e)
        {
            if (selectedAssignment == null)
            {
                await DisplayAlert("Brīdinājums", "Nav izvēlēts neviens uzdevums.", "Labi");
                return;
            }

            int assignmentId = (int)selectedAssignment["Id"];
            string oldDescription = selectedAssignment["Description"].ToString();
            string oldDeadline = selectedAssignment["Deadline"].ToString();

            string[] courseNames = allCourses.Select(c => c.CourseName).ToArray();
            string chosen = await DisplayActionSheet("Izvēlies kursu", "Atcelt", null, courseNames);
            if (chosen == "Atcelt" || string.IsNullOrEmpty(chosen)) return;

            var chosenCourse = allCourses.FirstOrDefault(c => c.CourseName == chosen);
            if (chosenCourse.Id == 0)
            {
                await DisplayAlert("Kļūda", "Kurss nav atrasts.", "Labi");
                return;
            }

            string newDescription = await DisplayPromptAsync("Labot uzdevumu", "Apraksts:", initialValue: oldDescription);
            if (string.IsNullOrEmpty(newDescription)) return;

            string newDeadlineStr = await DisplayPromptAsync("Labot uzdevumu", "Termiņš (YYYY-MM-DD):", initialValue: oldDeadline);
            if (!DateTime.TryParse(newDeadlineStr, out DateTime newDeadline))
            {
                await DisplayAlert("Kļūda", "Nepareizs datuma formāts!", "Labi");
                return;
            }

            try
            {
                DatabaseHelper.UpdateAssignment(assignmentId, newDescription, newDeadline, chosenCourse.Id);
                LoadAssignments();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }

        private async void OnDeleteAssignmentClicked(object sender, EventArgs e)
        {
            if (selectedAssignment == null)
            {
                await DisplayAlert("Brīdinājums", "Nav izvēlēts neviens uzdevums.", "Labi");
                return;
            }

            bool confirm = await DisplayAlert("Dzēst uzdevumu", "Tiešām dzēst šo uzdevumu?", "Jā", "Nē");
            if (!confirm) return;

            int assignmentId = (int)selectedAssignment["Id"];

            try
            {
                DatabaseHelper.DeleteAssignment(assignmentId);
                LoadAssignments();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kļūda", ex.Message, "Labi");
            }
        }

        private async void OnFilterByCourseClicked(object sender, EventArgs e)
        {
            if (allCourses.Count == 0)
            {
                await DisplayAlert("Brīdinājums", "Vispirms pievienojiet kursus!", "Labi");
                return;
            }

            string[] courseNames = allCourses.Select(c => c.CourseName).ToArray();
            string chosen = await DisplayActionSheet("Filtrēt pēc kursa", "Atcelt", null, courseNames);
            if (chosen == "Atcelt" || string.IsNullOrEmpty(chosen)) return;

            var chosenCourse = allCourses.FirstOrDefault(c => c.CourseName == chosen);
            if (chosenCourse.Id == 0) return;

            LoadAssignments(chosenCourse.Id);
        }
    }
}
