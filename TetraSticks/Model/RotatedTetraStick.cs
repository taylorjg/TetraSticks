using System.Collections.Generic;
using System.Linq;

namespace TetraSticks.Model
{
    public class RotatedTetraStick
    {
        public RotatedTetraStick(TetraStick tetraStick, Orientation orientation)
        {
            TetraStick = tetraStick;
            Orientation = orientation;
        }

        public TetraStick TetraStick { get; }
        public Orientation Orientation { get; }

        public int Width {
            get
            {
                var coords = TetraStick.Lines.SelectMany(l => l).ToList();
                var minX = coords.Min(c => c.X);
                var maxX = coords.Max(c => c.X);
                var minY = coords.Min(c => c.Y);
                var maxY = coords.Max(c => c.Y);

                if (Orientation == Orientation.North || Orientation == Orientation.South)
                {
                    return maxX - minX;
                }

                return maxY - minY;
            }
        }

        public int Height {
            get
            {
                var coords = TetraStick.Lines.SelectMany(l => l).ToList();
                var minX = coords.Min(c => c.X);
                var maxX = coords.Max(c => c.X);
                var minY = coords.Min(c => c.Y);
                var maxY = coords.Max(c => c.Y);

                if (Orientation == Orientation.North || Orientation == Orientation.South)
                {
                    return maxY - minY;
                }

                return maxX - minX;
            }
        }

        public IEnumerable<Coords>[] Lines
        {
            get
            {
                return null;
            }
        }
    }
}
