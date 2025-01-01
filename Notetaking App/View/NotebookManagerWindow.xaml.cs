using System.Windows;
using Notetaking_App.ViewModel;

namespace Notetaking_App.View
{
    /// <summary>
    /// Interaction logic for NotebookManagerWindow.xaml
    /// </summary>
    public partial class NotebookManagerWindow : Window
    {
        public NotebookManagerWindow()
        {
            InitializeComponent();
        }
        public void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
