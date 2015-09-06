using System.Collections.Generic;
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
        private readonly IDispatcher _dispatcher;
        private TetraStick _tetraStickToOmit;
        private RelayCommand _tetraStickToOmitChangedCommand;
        private RelayCommand _solveCommand;

        public MainWindowViewModel(IBoardControl boardControl, IDispatcher dispatcher)
        {
            _boardControl = boardControl;
            _dispatcher = dispatcher;
            TetraStickToOmit = TetraSticksToOmit.First();
        }

        public IEnumerable<TetraStick> TetraSticksToOmit => new[]
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
                RaisePropertyChanged(() => TetraStickToOmit);
            }
        }

        public ICommand TetraStickToOmitChangedCommand => 
            _tetraStickToOmitChangedCommand ?? (_tetraStickToOmitChangedCommand = new RelayCommand(OnTetraStickToOmitChanged));

        public ICommand SolveCommand => 
            _solveCommand ?? (_solveCommand = new RelayCommand(OnSolve));

        private void OnTetraStickToOmitChanged()
        {
        }

        private void OnSolve()
        {
            _boardControl.Clear();
            var puzzleSolver = new PuzzleSolver(
                TetraStickToOmit,
                _boardControl.DrawPlacedTetraSticks,
                _dispatcher,
                CancellationToken.None);
            puzzleSolver.SolvePuzzle();
        }
    }
}
