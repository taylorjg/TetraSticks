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

                BoardControl.DrawRotatedTetraStick(new RotatedTetraStick(Model.TetraSticks.F, Orientation.North, true), new Coords(4, 1));
                BoardControl.DrawRotatedTetraStick(new RotatedTetraStick(Model.TetraSticks.H, Orientation.South, true), new Coords(0, 2));
                BoardControl.DrawRotatedTetraStick(new RotatedTetraStick(Model.TetraSticks.I, Orientation.East), new Coords(0, 0));
                BoardControl.DrawRotatedTetraStick(new RotatedTetraStick(Model.TetraSticks.J, Orientation.East), new Coords(0, 4));
                BoardControl.DrawRotatedTetraStick(new RotatedTetraStick(Model.TetraSticks.N, Orientation.South), new Coords(0, 0));
                BoardControl.DrawRotatedTetraStick(new RotatedTetraStick(Model.TetraSticks.O, Orientation.North), new Coords(4, 0));
                BoardControl.DrawRotatedTetraStick(new RotatedTetraStick(Model.TetraSticks.P, Orientation.North), new Coords(3, 0));
                BoardControl.DrawRotatedTetraStick(new RotatedTetraStick(Model.TetraSticks.R, Orientation.South), new Coords(1, 3));
                BoardControl.DrawRotatedTetraStick(new RotatedTetraStick(Model.TetraSticks.T, Orientation.North), new Coords(1, 0));
                BoardControl.DrawRotatedTetraStick(new RotatedTetraStick(Model.TetraSticks.U, Orientation.South), new Coords(1, 4));
                BoardControl.DrawRotatedTetraStick(new RotatedTetraStick(Model.TetraSticks.V, Orientation.West), new Coords(3, 3));
                BoardControl.DrawRotatedTetraStick(new RotatedTetraStick(Model.TetraSticks.W, Orientation.South), new Coords(2, 2));
                BoardControl.DrawRotatedTetraStick(new RotatedTetraStick(Model.TetraSticks.X, Orientation.North), new Coords(0, 0));
                BoardControl.DrawRotatedTetraStick(new RotatedTetraStick(Model.TetraSticks.Y, Orientation.West, true), new Coords(4, 2));
                BoardControl.DrawRotatedTetraStick(new RotatedTetraStick(Model.TetraSticks.Z, Orientation.North, true), new Coords(2, 1));
            };
        }
    }
}
