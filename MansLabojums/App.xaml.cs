﻿// App.xaml.cs
using Microsoft.Maui.Controls;

namespace MansLabojums
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // Norādam, ka Shell (AppShell) būs galvenā lapa
            MainPage = new AppShell();
        }
    }
}

