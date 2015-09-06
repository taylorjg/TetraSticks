using System;
using System.Collections.Generic;
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
        private readonly Action<IEnumerable<PlacedTetraStick>> _solutionFoundHandler;
        private readonly TetraStick _tetraStickToOmit;
        private readonly IDispatcher _dispatcher;
        private readonly CancellationToken _cancellationToken;

        public PuzzleSolver(
            TetraStick tetraStickToOmit,
            Action<IEnumerable<PlacedTetraStick>> solutionFoundHandler,
            IDispatcher dispatcher,
            CancellationToken cancellationToken)
        {
            _solutionFoundHandler = solutionFoundHandler;
            _tetraStickToOmit = tetraStickToOmit;
            _dispatcher = dispatcher;
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
            _dispatcher.Invoke(_solutionFoundHandler, placedTetraSticks);
        }
    }
}
