using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DlxLib;
using TetraSticks.Model;

namespace TetraSticks.ViewModel
{
    public class PuzzleSolver
    {
        private readonly Action<object> _solutionFoundHandler;
        private readonly TetraStick _tetraStickToOmit;
        private readonly SynchronizationContext _synchronizationContext;
        private readonly CancellationToken _cancellationToken;

        public PuzzleSolver(
            TetraStick tetraStickToOmit,
            Action<object> solutionFoundHandler,
            SynchronizationContext synchronizationContext,
            CancellationToken cancellationToken)
        {
            _tetraStickToOmit = tetraStickToOmit;
            _solutionFoundHandler = solutionFoundHandler;
            _synchronizationContext = synchronizationContext;
            _cancellationToken = cancellationToken;
        }

        public void SolvePuzzle()
        {
            Task.Factory.StartNew(
                SolvePuzzleInBackground,
                _cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        private void SolvePuzzleInBackground()
        {
            var tetraSticks = Model.TetraSticks.All.Where(ts => ts.Tag != _tetraStickToOmit.Tag).ToImmutableList();
            var rows = RowBuilder.BuildRows(tetraSticks);
            var matrix = DlxMatrixBuilder.BuildDlxMatrix(tetraSticks, rows);
            var dlx = new Dlx();
            var firstSolution = dlx.Solve(matrix, d => d, r => r, 75).First();
            var placedTetraSticks = firstSolution.RowIndexes.Select(idx => rows[idx]);
            _synchronizationContext.Post(new SendOrPostCallback(_solutionFoundHandler), placedTetraSticks);
        }
    }
}
