using System.Collections.Generic;
using TetraSticks.Model;

namespace TetraSticks.ViewModel
{
    public interface IBoardControl
    {
        void DrawGrid();
        void Clear();
        void DrawPlacedTetraSticks(IEnumerable<PlacedTetraStick> placedTetraSticks);
    }
}
