using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DlxLib;
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
                var tetraSticks = Model.TetraSticks.All.Where(ts => ts.Tag != tetraStickToOmit.Tag).ToImmutableList();
                var rows = RowBuilder.BuildRows(tetraSticks);
                var matrix = DlxMatrixBuilder.BuildDlxMatrix(tetraSticks, rows);
                var dlx = new Dlx();
                var firstSolution = dlx.Solve(matrix, d => d, r => r).First();
                var solutionRows = firstSolution.RowIndexes.Select(idx => rows[idx]);
                foreach (var row in solutionRows) BoardControl.DrawPlacedTetraStick(row);
            };
        }
    }
}
