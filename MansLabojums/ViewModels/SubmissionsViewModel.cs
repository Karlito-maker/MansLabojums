using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MansLabojums.Models;
using Microsoft.Maui.Controls;

namespace MansLabojums.ViewModels
{
    public class SubmissionsViewModel : BaseViewModel
    {
        public ObservableCollection<Submission> Submissions { get; set; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public SubmissionsViewModel()
        {
            Title = "Iesniegumi";
            Submissions = new ObservableCollection<Submission>();
            AddCommand = new Command(async () => await AddSubmission());
            EditCommand = new Command<Submission>(async (submission) => await EditSubmission(submission));
            DeleteCommand = new Command<Submission>(async (submission) => await DeleteSubmission(submission));

            LoadSubmissions();
        }

        async void LoadSubmissions()
        {
            var submissions = await App.Database.GetSubmissionsAsync();
            Submissions.Clear();
            foreach (var submission in submissions)
            {
                Submissions.Add(submission);
            }
        }

        async Task AddSubmission()
        {
            var newSubmission = new Submission { AssignmentId = 1, StudentId = 1, Content = "Jauns iesniegums" };
            await App.Database.SaveSubmissionAsync(newSubmission);
            Submissions.Add(newSubmission);
        }

        async Task EditSubmission(Submission submission)
        {
            submission.Content = "Atjaunināts iesniegums";
            await App.Database.SaveSubmissionAsync(submission);
            LoadSubmissions();
        }

        async Task DeleteSubmission(Submission submission)
        {
            await App.Database.DeleteSubmissionAsync(submission);
            Submissions.Remove(submission);
        }
    }
}

