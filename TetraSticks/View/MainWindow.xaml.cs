using TetraSticks.Model;

namespace TetraSticks.View
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            ContentRendered += (_, __) =>
            {
                BoardControl.DrawGrid();

                var tetraStick = Model.TetraSticks.H;
                var rotatedTetraStick = new RotatedTetraStick(tetraStick, Orientation.North);
                BoardControl.DrawRotatedTetraStick(rotatedTetraStick);
            };
        }
    }
}
