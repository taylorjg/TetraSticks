using TetraSticks.ViewModel;

namespace TetraSticks.View
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel(BoardControl);

            ContentRendered += (_, __) =>
            {
                (BoardControl as IBoardControl).DrawGrid();
            };
        }
    }
}
