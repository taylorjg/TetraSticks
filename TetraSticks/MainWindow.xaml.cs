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

                var tetraStick = Model.TetraSticks.All.First(ts => ts.Tag == "Z");
                BoardControl.DrawTetraStick(tetraStick);
            };
        }
    }
}
