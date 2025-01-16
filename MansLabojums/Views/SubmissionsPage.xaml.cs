/******************************************************
 * MansLabojums/Views/SubmissionsPage.xaml.cs
 ******************************************************/
using Microsoft.Maui.Controls;
using MansLabojums.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MansLabojums.Views
{
    public partial class SubmissionsPage : ContentPage
    {
        // Vietējā klases definīcija, lai rādītu Submission sarakstā
        public class SubmissionDisplay
        {
            public int Id { get; set; }
            public string AssignmentDescription { get; set; } = "";
            public string StudentName { get; set; } = "";
            public string SubmissionTime { get; set; } = "";
            public int Score { get; set; }
            public int AssignmentId { get; set; }
            public int StudentId { get; set; }

            public string DisplayTitle => $"[{Id}] Uzdevums: {AssignmentDescription}";
            public string DisplayDetail => $"Students: {StudentName}, Score={Score}, {SubmissionTime}";
        }

        // Uzdevums Picker
        public class AssignmentItem
        {
            public int Id { get; set; }
            public string AssignmentDesc { get; set; } = "";
        }

        // Students Picker
        public class StudentItem
        {
            public int Id { get; set; }
            public string StudentName { get; set; } = "";
        }

        // Kolekcijas
        private ObservableCollection<SubmissionDisplay> _subs = new();
        public ObservableCollection<AssignmentItem> AssignmentList { get; set; } = new();
        public ObservableCollection<StudentItem> StudentList { get; set; } = new();

        // Izvēlētie
        public AssignmentItem SelectedAssignment { get; set; }
        public StudentItem SelectedStudent { get; set; }

        public SubmissionsPage()
        {
            InitializeComponent();

            // BindingContext 
            BindingContext = this;

            SubmissionsListView.ItemsSource = _subs;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadSubmissions();
            LoadAssignments();
            LoadStudents();
        }

        private void LoadSubmissions()
        {
            _subs.Clear();
            // get extended data
            var list = DatabaseHelper.GetSubmissionsWithIDs();
            // sagaidām "Id", "AssignmentId", "StudentId", "AssignmentDescription", "StudentName", "SubmissionTime", "Score"
            foreach (var dict in list)
            {
                _subs.Add(new SubmissionDisplay
                {
                    Id = (int)dict["Id"],
                    AssignmentId = (int)dict["AssignmentId"],
                    StudentId = (int)dict["StudentId"],
                    AssignmentDescription = dict["AssignmentDescription"].ToString()!,
                    StudentName = dict["StudentName"].ToString()!,
                    SubmissionTime = dict["SubmissionTime"].ToString()!,
                    Score = (int)dict["Score"]
                });
            }
        }

        private void LoadAssignments()
        {
            AssignmentList.Clear();
            var assignments = DatabaseHelper.GetAssignments();
            // return List<Assignment> => { Id, Description, Deadline, CourseId }
            foreach (var a in assignments)
            {
                AssignmentList.Add(new AssignmentItem
                {
                    Id = a.Id,
                    AssignmentDesc = $"{a.Id}: {a.Description}"
                });
            }
        }

        private void LoadStudents()
        {
            StudentList.Clear();
            var studs = DatabaseHelper.GetStudents();
            foreach (var s in studs)
            {
                StudentList.Add(new StudentItem
                {
                    Id = s.Id,
                    StudentName = $"{s.Id}: {s.Name} {s.Surname}"
                });
            }
        }

        private void OnAddSubmissionClicked(object sender, EventArgs e)
        {
            // Paņemam user-ievadīto Score
            string scoreStr = ScoreEntry.Text?.Trim();
            if (!int.TryParse(scoreStr, out int sc))
            {
                DisplayAlert("Kļūda", "Lūdzu ievadiet skaitlisku Score!", "OK");
                return;
            }

            // Pārbaudām, vai ir atlasīts Assignment un Students
            if (SelectedAssignment == null || SelectedStudent == null)
            {
                DisplayAlert("Kļūda", "Lūdzu izvēlieties uzdevumu un studentu!", "OK");
                return;
            }

            try
            {
                // DatabaseHelper.AddSubmission(....) – bet tam vajag desc un studName
                // Mēs varam to drusku apiet: DatabaseHelper spēj meklēt assignment pēc “Description” un
                // studentu pēc “Name”. Tāpēc:
                // assignmentDescription => aId:desc (mums vajag tikai desc)
                // studentName => sId: vards (mums vajag vārdu)

                // Piemēram, sadalām:
                string[] aSplit = SelectedAssignment.AssignmentDesc.Split(':');
                string pureAssignmentDesc = (aSplit.Length > 1) ? aSplit[1].Trim() : "???";

                string[] sSplit = SelectedStudent.StudentName.Split(':');
                // "1: Pēteris Ozoliņš" => sSplit[0]="1", sSplit[1]="Pēteris Ozoliņš"
                string pureStudentName = (sSplit.Length > 1) ? sSplit[1].Trim() : "???";

                DatabaseHelper.AddSubmission(pureAssignmentDesc, pureStudentName, sc);

                // Iztīrām
                AssignmentPicker.SelectedItem = null;
                StudentPicker.SelectedItem = null;
                ScoreEntry.Text = "";
                SelectedAssignment = null;
                SelectedStudent = null;

                // Atjaunojam sarakstu
                LoadSubmissions();
            }
            catch (Exception ex)
            {
                DisplayAlert("Kļūda", $"Neizdevās pievienot iesniegumu: {ex.Message}", "OK");
            }
        }

        private async void OnEditSubmissionClicked(object sender, EventArgs e)
        {
            if (SubmissionsListView.SelectedItem is SubmissionDisplay selSub)
            {
                string oldScore = selSub.Score.ToString();
                string newScoreStr = await DisplayPromptAsync("Labot iesniegumu",
                                                              "Jauns rezultāts:",
                                                              initialValue: oldScore);
                if (int.TryParse(newScoreStr, out int newScore))
                {
                    try
                    {
                        DatabaseHelper.UpdateSubmission(selSub.Id, newScore);
                        LoadSubmissions();
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Kļūda", "Neizdevās labot iesniegumu: " + ex.Message, "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Kļūda", "Lūdzu ievadiet skaitli!", "OK");
                }
            }
            else
            {
                await DisplayAlert("Brīdinājums", "Nav izvēlēts neviens iesniegums!", "OK");
            }
        }

        private async void OnDeleteSubmissionClicked(object sender, EventArgs e)
        {
            if (SubmissionsListView.SelectedItem is SubmissionDisplay selSub)
            {
                bool confirm = await DisplayAlert("Dzēst iesniegumu?", selSub.AssignmentDescription, "Jā", "Nē");
                if (confirm)
                {
                    try
                    {
                        DatabaseHelper.DeleteSubmission(selSub.Id);
                        LoadSubmissions();
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Kļūda", "Neizdevās dzēst iesniegumu: " + ex.Message, "OK");
                    }
                }
            }
            else
            {
                await DisplayAlert("Brīdinājums", "Nav izvēlēts neviens iesniegums!", "OK");
            }
        }
    }
}

