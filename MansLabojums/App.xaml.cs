using System.IO;
using Microsoft.Maui.Controls;
using MansLabojums.Services;

namespace MansLabojums
{
    public partial class App : Application
    {
        private static DatabaseService _database;

        public static DatabaseService Database
        {
            get
            {
                if (_database == null)
                {
                    var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MansLabojums.db3");
                    _database = new DatabaseService(dbPath);
                }
                return _database;
            }
        }

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}
