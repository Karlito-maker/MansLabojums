using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using MansLabojums.Helpers;

namespace MansLabojums
{
    public partial class App : Application
    {
        public static DbContext DbContext { get; private set; } = null!;

        public App()
        {
            InitializeComponent();
            DbContext = new DbContext();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new MainPage());
            return window;
        }
    }
}
