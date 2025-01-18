
using Microsoft.Maui.Controls;

namespace MansLabojums
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        
            MainPage = new AppShell();
        }
    }
}

