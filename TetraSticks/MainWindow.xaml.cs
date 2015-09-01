using System.Windows;

namespace TetraSticks
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ContentRendered += (_, __) => BoardControl.DrawGrid();
        }
    }
}
