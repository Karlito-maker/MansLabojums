using Microsoft.Maui.Controls;

namespace MansLabojums
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute(nameof(Views.StudentsPage), typeof(Views.StudentsPage));
            Routing.RegisterRoute(nameof(Views.AssignmentsPage), typeof(Views.AssignmentsPage));
            Routing.RegisterRoute(nameof(Views.SubmissionsPage), typeof(Views.SubmissionsPage));
            Routing.RegisterRoute(nameof(Views.CoursePage), typeof(Views.CoursePage));
            Routing.RegisterRoute(nameof(Views.TeachersPage), typeof(Views.TeachersPage));
        }
    }
}

















