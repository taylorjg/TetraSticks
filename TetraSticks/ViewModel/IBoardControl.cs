using System.Collections.Generic;
using TetraSticks.Model;

namespace TetraSticks.ViewModel
{
    public interface IBoardControl
    {
        void DrawGrid();
        void Reset();
        void AddPlacedTetraStick(PlacedTetraStick placedTetraStick);
        void AddPlacedTetraSticks(IEnumerable<PlacedTetraStick> placedTetraSticks);
        void RemovePlacedTetraStick(PlacedTetraStick placedTetraStick);
        bool IsPlacedTetraStickOnBoard(PlacedTetraStick placedTetraStick);
        bool IsPlacedTetraStickOnBoardCorrectly(PlacedTetraStick placedTetraStick);
        void RemovePlacedTetraSticksOtherThan(IEnumerable<PlacedTetraStick> placedTetraSticks);
    }
}
