using System.Linq;

namespace TetraSticks
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            ContentRendered += (_, __) =>
            {
                BoardControl.DrawGrid();

                var f = Model.TetraSticks.All.First();
                BoardControl.DrawTetraStick(f);
            };
        }
    }
}
