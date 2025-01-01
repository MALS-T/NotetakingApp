using System.Windows;
using System.Windows.Controls;

namespace Notetaking_App.View
{
    /// <summary>
    /// Interaction logic for DrawingWindow.xaml
    /// </summary>
    public partial class DrawingWindow : Window
    {
        public bool confirmed;
        public DrawingWindow()
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
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            confirmed = true;
            Close();
        }

        private void TogglePenEraser_Click(object sender, RoutedEventArgs e)
        {
            if((string)PenEraserToggleBtn.Content == "◨")
            {
                PenEraserToggleBtn.Content = "🖊";
            }
            else
            {
                PenEraserToggleBtn.Content = "◨";
            }
        }
    }
}
