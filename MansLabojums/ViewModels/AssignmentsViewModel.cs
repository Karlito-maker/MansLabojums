using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MansLabojums.Models;
using Microsoft.Maui.Controls;

namespace MansLabojums.ViewModels
{
    public class AssignmentsViewModel : BaseViewModel
    {
        public ObservableCollection<Assignment> Assignments { get; set; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public AssignmentsViewModel()
        {
            Title = "Uzdevumi";
            Assignments = new ObservableCollection<Assignment>();
            AddCommand = new Command(async () => await AddAssignment());
            EditCommand = new Command<Assignment>(async (assignment) => await EditAssignment(assignment));
            DeleteCommand = new Command<Assignment>(async (assignment) => await DeleteAssignment(assignment));

            LoadAssignments();
        }

        async void LoadAssignments()
        {
            var assignments = await App.Database.GetAssignmentsAsync();
            Assignments.Clear();
            foreach (var assignment in assignments)
            {
                Assignments.Add(assignment);
            }
        }

        async Task AddAssignment()
        {
            // Šeit var pievienot logiku, lai saņemtu lietotāja ievadi caur dialogu
            var newAssignment = new Assignment { Name = "Jauns uzdevums", Description = "Apraksts" };
            await App.Database.SaveAssignmentAsync(newAssignment);
            Assignments.Add(newAssignment);
        }

        async Task EditAssignment(Assignment assignment)
        {
            // Šeit var pievienot logiku uzdevuma labošanai
            assignment.Name = "Atjaunināts uzdevums";
            await App.Database.SaveAssignmentAsync(assignment);
            LoadAssignments();
        }

        async Task DeleteAssignment(Assignment assignment)
        {
            await App.Database.DeleteAssignmentAsync(assignment);
            Assignments.Remove(assignment);
        }
    }
}
