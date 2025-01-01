using Notetaking_App.Themes;
using Notetaking_App.ViewModel;
using System.Windows;

namespace Notetaking_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool darkTheme = false;
        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel vm = new MainWindowViewModel();
            DataContext = vm;

            // Default to light theme on start
            AppTheme.ChangeTheme(new Uri("Themes/LightThemeColors.xaml", UriKind.Relative));
        }

        public void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            darkTheme = !darkTheme;

            if(darkTheme)
            {
                AppTheme.ChangeTheme(new Uri("Themes/DarkThemeColors.xaml", UriKind.Relative));
                ThemeToggleBtn.Content = "☀";
            }
            else
            {
                AppTheme.ChangeTheme(new Uri("Themes/LightThemeColors.xaml", UriKind.Relative));
                ThemeToggleBtn.Content = "🌙";
            }
        }
    }

}