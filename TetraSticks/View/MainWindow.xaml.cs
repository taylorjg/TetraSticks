using System;
using System.Linq;
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

                //BoardControl.DrawPlacedTetraStick(new PlacedTetraStick(Model.TetraSticks.F, new Coords(4, 1), Orientation.North, ReflectionMode.MirrorY));
                //BoardControl.DrawPlacedTetraStick(new PlacedTetraStick(Model.TetraSticks.H, new Coords(0, 2), Orientation.South, ReflectionMode.MirrorY));
                //BoardControl.DrawPlacedTetraStick(new PlacedTetraStick(Model.TetraSticks.I, new Coords(0, 0), Orientation.East, ReflectionMode.Normal));
                //BoardControl.DrawPlacedTetraStick(new PlacedTetraStick(Model.TetraSticks.J, new Coords(0, 4), Orientation.East, ReflectionMode.Normal));
                //BoardControl.DrawPlacedTetraStick(new PlacedTetraStick(Model.TetraSticks.N, new Coords(0, 0), Orientation.South, ReflectionMode.Normal));
                //BoardControl.DrawPlacedTetraStick(new PlacedTetraStick(Model.TetraSticks.O, new Coords(4, 0), Orientation.North, ReflectionMode.Normal));
                //BoardControl.DrawPlacedTetraStick(new PlacedTetraStick(Model.TetraSticks.P, new Coords(3, 0), Orientation.North, ReflectionMode.Normal));
                //BoardControl.DrawPlacedTetraStick(new PlacedTetraStick(Model.TetraSticks.R, new Coords(1, 3), Orientation.South, ReflectionMode.Normal));
                //BoardControl.DrawPlacedTetraStick(new PlacedTetraStick(Model.TetraSticks.T, new Coords(1, 0), Orientation.North, ReflectionMode.Normal));
                //BoardControl.DrawPlacedTetraStick(new PlacedTetraStick(Model.TetraSticks.U, new Coords(1, 4), Orientation.South, ReflectionMode.Normal));
                //BoardControl.DrawPlacedTetraStick(new PlacedTetraStick(Model.TetraSticks.V, new Coords(3, 3), Orientation.West, ReflectionMode.Normal));
                //BoardControl.DrawPlacedTetraStick(new PlacedTetraStick(Model.TetraSticks.W, new Coords(2, 2), Orientation.South, ReflectionMode.Normal));
                //BoardControl.DrawPlacedTetraStick(new PlacedTetraStick(Model.TetraSticks.X, new Coords(0, 0), Orientation.North, ReflectionMode.Normal));
                //BoardControl.DrawPlacedTetraStick(new PlacedTetraStick(Model.TetraSticks.Y, new Coords(4, 2), Orientation.West, ReflectionMode.MirrorY));
                //BoardControl.DrawPlacedTetraStick(new PlacedTetraStick(Model.TetraSticks.Z, new Coords(2, 1), Orientation.North, ReflectionMode.MirrorY));

                var rows = new[]
                {
                    new PlacedTetraStick(Model.TetraSticks.F, new Coords(4, 1), Orientation.North, ReflectionMode.MirrorY),
                    new PlacedTetraStick(Model.TetraSticks.H, new Coords(0, 2), Orientation.South, ReflectionMode.MirrorY),
                    new PlacedTetraStick(Model.TetraSticks.I, new Coords(0, 0), Orientation.East, ReflectionMode.Normal),
                    new PlacedTetraStick(Model.TetraSticks.J, new Coords(0, 4), Orientation.East, ReflectionMode.Normal),
                    new PlacedTetraStick(Model.TetraSticks.N, new Coords(0, 0), Orientation.South, ReflectionMode.Normal),
                    new PlacedTetraStick(Model.TetraSticks.O, new Coords(4, 0), Orientation.North, ReflectionMode.Normal),
                    new PlacedTetraStick(Model.TetraSticks.P, new Coords(3, 0), Orientation.North, ReflectionMode.Normal),
                    new PlacedTetraStick(Model.TetraSticks.R, new Coords(1, 3), Orientation.South, ReflectionMode.Normal),
                    new PlacedTetraStick(Model.TetraSticks.T, new Coords(1, 0), Orientation.North, ReflectionMode.Normal),
                    new PlacedTetraStick(Model.TetraSticks.U, new Coords(1, 4), Orientation.South, ReflectionMode.Normal),
                    new PlacedTetraStick(Model.TetraSticks.V, new Coords(3, 3), Orientation.West, ReflectionMode.Normal),
                    new PlacedTetraStick(Model.TetraSticks.W, new Coords(2, 2), Orientation.South, ReflectionMode.Normal),
                    new PlacedTetraStick(Model.TetraSticks.X, new Coords(0, 0), Orientation.North, ReflectionMode.Normal),
                    new PlacedTetraStick(Model.TetraSticks.Y, new Coords(4, 2), Orientation.West, ReflectionMode.MirrorY),
                    new PlacedTetraStick(Model.TetraSticks.Z, new Coords(2, 1), Orientation.North, ReflectionMode.MirrorY)
                };

                foreach (var row in rows)
                    BoardControl.DrawPlacedTetraStick(row);

                var tetraStickToOmit = Model.TetraSticks.L;
                var tetraSticks = Model.TetraSticks.All.Where(ts => ts.Tag != tetraStickToOmit.Tag).ToList();
                //var rows = RowBuilder.BuildRows(tetraSticks).ToList();
                var matrix = DlxMatrixBuilder.BuildDlxMatrix(tetraSticks, rows).ToList();
                var dlx = new DlxLib.Dlx();
                var solutions = dlx.Solve(matrix, d => d, r => r).ToList();
                System.Diagnostics.Debug.WriteLine($"Number of solutions found: {solutions.Count}");
            };
        }
    }
}
