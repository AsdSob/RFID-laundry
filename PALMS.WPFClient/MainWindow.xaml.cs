using System;
using System.Windows;
using System.Configuration;
using PALMS.ViewModels;

namespace PALMS.WPFClient
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!((sender as MainWindow)?.DataContext is MainViewModel dataContext)) return;

            dataContext.Initialize();
        }
    }
}
