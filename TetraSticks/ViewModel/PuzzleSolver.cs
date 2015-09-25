using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DlxLib;
using TetraSticks.Extensions;
using TetraSticks.Model;

namespace TetraSticks.ViewModel
{
    public class PuzzleSolver
    {
        private readonly Action<IImmutableList<PlacedTetraStick>> _onSolutionFound;
        private readonly Action<IImmutableList<PlacedTetraStick>> _onSearchStep;
        private readonly Action _onDoneSolving;
        private readonly TetraStick _tetraStickToOmit;
        private readonly SynchronizationContext _synchronizationContext;
        private readonly CancellationToken _cancellationToken;

        public PuzzleSolver(
            TetraStick tetraStickToOmit,
            Action<IImmutableList<PlacedTetraStick>> onSolutionFound,
            Action<IImmutableList<PlacedTetraStick>> onSearchStep,
            Action onDoneSolving,
            SynchronizationContext synchronizationContext,
            CancellationToken cancellationToken)
        {
            _tetraStickToOmit = tetraStickToOmit;
            _onSolutionFound = onSolutionFound;
            _onSearchStep = onSearchStep;
            _onDoneSolving = onDoneSolving;
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
            var dlx = new Dlx(_cancellationToken);

            dlx.SearchStep += (_, searchStepEventArgs) =>
            {
                var placedTetraSticks = searchStepEventArgs.RowIndexes.Select(idx => rows[idx]).ToImmutableList();
                _synchronizationContext.Post(_onSearchStep, placedTetraSticks);
            };

            //var solutions = dlx.Solve(matrix, d => d, r => r, 75);

            //foreach (var solution in solutions)
            //{
            //    var placedTetraSticks = solution.RowIndexes.Select(idx => rows[idx]).ToImmutableList();
            //    _synchronizationContext.Post(_onSolutionFound, placedTetraSticks);
            //}

            var solution = dlx.Solve(matrix, d => d, r => r, 75).First();

            {
                var placedTetraSticks = solution.RowIndexes.Select(idx => rows[idx]).ToImmutableList();
                _synchronizationContext.Post(_onSolutionFound, placedTetraSticks);
            }

            _synchronizationContext.Post(_onDoneSolving);
        }
    }
}
