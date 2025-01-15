using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MansLabojums.Models;
using MansLabojums.Helpers;

namespace MansLabojums.Views
{
    public partial class AssignmentsPage : ContentPage
    {
        public AssignmentsPage()
        {
            InitializeComponent();
            BindingContext = new AssignmentsViewModel();
        }
    }

    public class AssignmentsViewModel : BaseViewModel
    {
        public ObservableCollection<Assignment> Assignments { get; set; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand GenerateTestDataCommand { get; }

        public AssignmentsViewModel()
        {
            Title = "Uzdevumi";
            Assignments = new ObservableCollection<Assignment>();
            AddCommand = new Command(async () => await AddAssignment());
            EditCommand = new Command<Assignment>(async (assignment) => await EditAssignment(assignment));
            DeleteCommand = new Command<Assignment>(async (assignment) => await DeleteAssignment(assignment));
            GenerateTestDataCommand = new Command(async () => await GenerateTestData());

            LoadAssignments().ConfigureAwait(false);
        }

        async Task LoadAssignments()
        {
            var assignments = await DatabaseHelper.GetAssignmentsAsync();
            Assignments.Clear();
            foreach (var assignment in assignments)
            {
                Assignments.Add(assignment);
            }
        }

        async Task AddAssignment()
        {
            var assignment = new Assignment();
            // Here you can implement a modal page to get user input
            // await App.Current.MainPage.Navigation.PushAsync(new AssignmentDetailPage(assignment));
            await LoadAssignments();
        }

        async Task EditAssignment(Assignment assignment)
        {
            // Open the detail page for editing
            // await App.Current.MainPage.Navigation.PushAsync(new AssignmentDetailPage(assignment));
            await LoadAssignments();
        }

        async Task DeleteAssignment(Assignment assignment)
        {
            await DatabaseHelper.DeleteAssignmentAsync(assignment);
            Assignments.Remove(assignment);
        }

        async Task GenerateTestData()
        {
            var testAssignments = new List<Assignment>
            {
                new Assignment { Name = "Test Assignment 1", Description = "Description 1" },
                new Assignment { Name = "Test Assignment 2", Description = "Description 2" },
                new Assignment { Name = "Test Assignment 3", Description = "Description 3" },
            };

            foreach (var assignment in testAssignments)
            {
                await DatabaseHelper.SaveAssignmentAsync(assignment);
                Assignments.Add(assignment);
            }
        }
    }
}


