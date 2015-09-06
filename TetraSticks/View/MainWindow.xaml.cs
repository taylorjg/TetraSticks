using System.Collections.Immutable;
using System.Linq;
using DlxLib;
using TetraSticks.Model;
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
                BoardControl.DrawGrid();

                var tetraStickToOmit = Model.TetraSticks.L;
                var tetraSticks = Model.TetraSticks.All.Where(ts => ts.Tag != tetraStickToOmit.Tag).ToImmutableList();
                var rows = RowBuilder.BuildRows(tetraSticks);
                var matrix = DlxMatrixBuilder.BuildDlxMatrix(tetraSticks, rows);
                var dlx = new Dlx();
                var firstSolution = dlx.Solve(matrix, d => d, r => r, 75).First();
                var placedTetraSticks = firstSolution.RowIndexes.Select(idx => rows[idx]);
                BoardControl.DrawPlacedTetraSticks(placedTetraSticks);
            };
        }
    }
}
