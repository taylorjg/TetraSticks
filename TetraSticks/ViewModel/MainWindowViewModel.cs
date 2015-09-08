using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using TetraSticks.Model;

namespace TetraSticks.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IBoardControl _boardControl;
        private TetraStick _tetraStickToOmit;
        private readonly ObservableCollection<IImmutableList<PlacedTetraStick>> _solutions;
        private int? _currentSolutionIndex;
        private RelayCommand _solveCommand;
        private RelayCommand _showNextSolutionCommand;
        private RelayCommand _cancelCommand;
        private bool _solving;
        private CancellationTokenSource _cancellationTokenSource;

        public MainWindowViewModel(IBoardControl boardControl)
        {
            _boardControl = boardControl;
            _solutions = new ObservableCollection<IImmutableList<PlacedTetraStick>>();
            TetraStickToOmit = TetraSticksToOmit.First();
        }

        public static IEnumerable<TetraStick> TetraSticksToOmit => new[]
        {
            Model.TetraSticks.H,
            Model.TetraSticks.J,
            Model.TetraSticks.L,
            Model.TetraSticks.N,
            Model.TetraSticks.Y
        };

        public TetraStick TetraStickToOmit {
            get
            {
                return _tetraStickToOmit;
            }
            set
            {
                _tetraStickToOmit = value;
                RaiseCommonPropertyChangedEvents();
            }
        }

        public bool Solving {
            get { return _solving; }
            set
            {
                _solving = value;
                RaiseCommonPropertyChangedEvents();
            }
        }

        public int? CurrentSolutionIndex {
            get { return _currentSolutionIndex; }
            set
            {
                _currentSolutionIndex = value;
                RaiseCommonPropertyChangedEvents();
            }
        }

        public string FormattedStats => CurrentSolutionIndex.HasValue
            ? $"{CurrentSolutionIndex + 1}/{Solutions.Count}"
            : string.Empty;

        public ObservableCollection<IImmutableList<PlacedTetraStick>> Solutions => _solutions;

        public ICommand SolveCommand => 
            _solveCommand ?? (_solveCommand = new RelayCommand(OnSolve, OnCanSolve));

        public ICommand ShowNextSolutionCommand =>
            _showNextSolutionCommand ?? (_showNextSolutionCommand= new RelayCommand(OnShowNextSolution, OnCanShowNextSolution));

        public ICommand CancelCommand => 
            _cancelCommand ?? (_cancelCommand = new RelayCommand(OnCancel, OnCanCancel));

        private void OnSolve()
        {
            Solutions.Clear();
            CurrentSolutionIndex = null;

            _boardControl.Clear();
            _cancellationTokenSource = new CancellationTokenSource();

            Solving = true;

            var puzzleSolver = new PuzzleSolver(
                TetraStickToOmit,
                OnSolutionFound,
                OnDoneSolving,
                SynchronizationContext.Current,
                _cancellationTokenSource.Token);

            puzzleSolver.SolvePuzzle();
        }

        private bool OnCanSolve()
        {
            return !Solving;
        }

        private void OnShowNextSolution()
        {
            CurrentSolutionIndex = (CurrentSolutionIndex.HasValue) ? (CurrentSolutionIndex + 1) : 0;
            Debug.Assert(CurrentSolutionIndex.HasValue);
            DisplaySolution(_solutions[CurrentSolutionIndex.Value]);
        }

        private bool OnCanShowNextSolution()
        {
            return CurrentSolutionIndex.HasValue && CurrentSolutionIndex.Value < (_solutions.Count - 1);
        }

        private void OnCancel()
        {
            _cancellationTokenSource.Cancel();
        }

        private bool OnCanCancel()
        {
            return Solving;
        }

        private void OnSolutionFound(IImmutableList<PlacedTetraStick> solution)
        {
            _solutions.Add(solution);

            if (CurrentSolutionIndex == null)
                OnShowNextSolution();

            RaiseCommonPropertyChangedEvents();
        }

        private void OnDoneSolving()
        {
            Solving = false;
        }

        private void DisplaySolution(IEnumerable<PlacedTetraStick> solution)
        {
            _boardControl.Clear();
            _boardControl.DrawPlacedTetraSticks(solution);
        }

        private void RaiseCommonPropertyChangedEvents()
        {
            RaisePropertyChanged(() => CurrentSolutionIndex);
            RaisePropertyChanged(() => FormattedStats);
            RaisePropertyChanged(() => TetraStickToOmit);

            _solveCommand?.RaiseCanExecuteChanged();
            _showNextSolutionCommand?.RaiseCanExecuteChanged();
            _cancelCommand?.RaiseCanExecuteChanged();
        }
    }
}
