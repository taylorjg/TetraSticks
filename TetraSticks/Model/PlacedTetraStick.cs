using System;
using System.Collections.Generic;
using System.Linq;

namespace TetraSticks.Model
{
    public class PlacedTetraStick
    {
        public PlacedTetraStick(TetraStick tetraStick, Coords location, Orientation orientation, ReflectionMode reflectionMode)
        {
            Location = location;
            TetraStick = tetraStick;
            Orientation = orientation;
            ReflectionMode = reflectionMode;
            _lazyWidth = new Lazy<int>(CalculateWidth);
            _lazyHeight = new Lazy<int>(CalculateHeight);
            _lazyInteriorJunctionPoints = new Lazy<IEnumerable<Coords>>(CalculateInteriorJunctionPoints);
            _lazyLines = new Lazy<IEnumerable<IEnumerable<Coords>>>(CalculateLines);
        }

        public string Tag => TetraStick.Tag;
        public IEnumerable<Coords> InteriorJunctionPoints => _lazyInteriorJunctionPoints.Value;
        public IEnumerable<IEnumerable<Coords>> Lines => _lazyLines.Value;
        private TetraStick TetraStick { get; }
        public Coords Location { get; }
        private Orientation Orientation { get; }
        private ReflectionMode ReflectionMode { get; }
        public int Width => _lazyWidth.Value;
        public int Height => _lazyHeight.Value;

        private Coords ApplyTransform(Coords coords)
        {
            return ApplyReflectionMode(ApplyOrientation(coords));
        }

        private Coords ApplyOrientation(Coords coords)
        {
            switch (Orientation)
            {
                case Orientation.North:
                    return coords;

                case Orientation.South:
                    return new Coords(Width - coords.X, Height - coords.Y);

                case Orientation.East:
                    return new Coords(coords.Y, Height - coords.X);

                case Orientation.West:
                    return new Coords(Width - coords.Y, coords.X);

                default:
                    throw new InvalidOperationException($"Unknown orientation, \"{Orientation}\".");
            }
        }

        private Coords ApplyReflectionMode(Coords coords)
        {
            switch (ReflectionMode)
            {
                case ReflectionMode.Normal:
                    return coords;

                case ReflectionMode.MirrorY:
                    return new Coords(Width - coords.X, coords.Y);

                default:
                    throw new InvalidOperationException($"Unknown reflection mode, \"{ReflectionMode}\".");
            }
        }

        private readonly Lazy<int> _lazyWidth;
        private readonly Lazy<int> _lazyHeight;
        private readonly Lazy<IEnumerable<Coords>> _lazyInteriorJunctionPoints;
        private readonly Lazy<IEnumerable<IEnumerable<Coords>>> _lazyLines;

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

        private IEnumerable<Coords> CalculateInteriorJunctionPoints()
        {
            return TetraStick.InteriorJunctionPoints.Select(ApplyTransform);
        }

        private IEnumerable<IEnumerable<Coords>> CalculateLines()
        {
            return TetraStick.Lines.Select(line => line.Select(ApplyTransform));
        }

        public override string ToString()
        {
            return $"TetraStick: {TetraStick.Tag}; Location: {Location}; Orientation: {Orientation}; ReflectionMode: {ReflectionMode}";
        }
    }
}
