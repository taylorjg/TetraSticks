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

                var tetraStickToOmit = Model.TetraSticks.L;
                var tetraSticks = Model.TetraSticks.All.Where(ts => ts.Tag != tetraStickToOmit.Tag).ToList();
                var rows = RowBuilder.BuildRows(tetraSticks).ToArray();
                var matrix = DlxMatrixBuilder.BuildDlxMatrix(tetraSticks, rows);
                var dlx = new DlxLib.Dlx();
                var solutions = dlx.Solve(matrix, d => d, r => r);
                var firstSolution = solutions.First();
                var solutionRows = firstSolution.RowIndexes.Select(idx => rows[idx]);
                foreach (var row in solutionRows) BoardControl.DrawPlacedTetraStick(row);
            };
        }
    }
}
