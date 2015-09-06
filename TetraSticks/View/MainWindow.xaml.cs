using TetraSticks.ViewModel;

namespace TetraSticks.View
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel(
                BoardControl,
                new WpfDispatcher(Dispatcher));

            ContentRendered += (_, __) =>
            {
                BoardControl.DrawGrid();
            };
        }
    }
}
