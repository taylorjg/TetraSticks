using System;
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
            _lazyWidth = new Lazy<int>(CalculateWidth);
            _lazyHeight = new Lazy<int>(CalculateHeight);
        }

        public TetraStick TetraStick { get; }
        public Orientation Orientation { get; }
        public int Width => _lazyWidth.Value;
        public int Height => _lazyHeight.Value;

        public IEnumerable<Coords>[] Lines
        {
            get
            {
                return TetraStick.Lines;
            }
        }

        private readonly Lazy<int> _lazyWidth;
        private readonly Lazy<int> _lazyHeight;

        private int CalculateWidth()
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

        private int CalculateHeight()
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
}
