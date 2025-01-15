using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MansLabojums.Models;
using MansLabojums.Helpers;

namespace MansLabojums.Views
{
    public partial class SubmissionsPage : ContentPage
    {
        public SubmissionsPage()
        {
            InitializeComponent();
            BindingContext = new SubmissionsViewModel();
        }
    }

    public class SubmissionsViewModel : BaseViewModel
    {
        public ObservableCollection<Submission> Submissions { get; set; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand GenerateTestDataCommand { get; }

        public SubmissionsViewModel()
        {
            Title = "Iesniegumi";
            Submissions = new ObservableCollection<Submission>();
            AddCommand = new Command(async () => await AddSubmission());
            EditCommand = new Command<Submission>(async (submission) => await EditSubmission(submission));
            DeleteCommand = new Command<Submission>(async (submission) => await DeleteSubmission(submission));
            GenerateTestDataCommand = new Command(async () => await GenerateTestData());

            LoadSubmissions().ConfigureAwait(false);
        }

        async Task LoadSubmissions()
        {
            var submissions = await DatabaseHelper.GetSubmissionsAsync();
            Submissions.Clear();
            foreach (var submission in submissions)
            {
                Submissions.Add(submission);
            }
        }

        async Task AddSubmission()
        {
            var submission = new Submission();
            // Here you can implement a modal page to get user input
            // await App.Current.MainPage.Navigation.PushAsync(new SubmissionDetailPage(submission));
            await LoadSubmissions();
        }

        async Task EditSubmission(Submission submission)
        {
            // Open the detail page for editing
            // await App.Current.MainPage.Navigation.PushAsync(new SubmissionDetailPage(submission));
            await LoadSubmissions();
        }

        async Task DeleteSubmission(Submission submission)
        {
            await DatabaseHelper.DeleteSubmissionAsync(submission);
            Submissions.Remove(submission);
        }

        async Task GenerateTestData()
        {
            var testSubmissions = new List<Submission>
            {
                new Submission { AssignmentId = 1, StudentId = 1, Content = "Submission 1" },
                new Submission { AssignmentId = 2, StudentId = 2, Content = "Submission 2" },
                new Submission { AssignmentId = 3, StudentId = 3, Content = "Submission 3" },
            };

            foreach (var submission in testSubmissions)
            {
                await DatabaseHelper.SaveSubmissionAsync(submission);
                Submissions.Add(submission);
            }
        }
    }
}


