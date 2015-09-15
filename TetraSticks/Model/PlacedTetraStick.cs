using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace TetraSticks.Model
{
    public class PlacedTetraStick
    {
        public PlacedTetraStick(TetraStick tetraStick, Coords location, Orientation orientation, ReflectionMode reflectionMode)
        {
            TetraStick = tetraStick;
            Location = location;
            Orientation = orientation;
            ReflectionMode = reflectionMode;
            _lazyWidth = new Lazy<int>(CalculateWidth);
            _lazyHeight = new Lazy<int>(CalculateHeight);
            _lazyInteriorJunctionPoints = new Lazy<IImmutableList<Coords>>(CalculateInteriorJunctionPoints);
            _lazyLines = new Lazy<IEnumerable<IImmutableList<Coords>>>(CalculateLines);
        }

        public TetraStick TetraStick { get; }
        public Coords Location { get; }
        public Orientation Orientation { get; }
        public ReflectionMode ReflectionMode { get; }
        public int Width => _lazyWidth.Value;
        public int Height => _lazyHeight.Value;
        public IImmutableList<Coords> InteriorJunctionPoints => _lazyInteriorJunctionPoints.Value;
        public IEnumerable<IImmutableList<Coords>> Lines => _lazyLines.Value;

        private Coords ApplyTransform(Coords coords)
        {
            return ApplyLocation(
                ApplyReflectionMode(
                    ApplyOrientation(coords)));
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

        private Coords ApplyLocation(Coords coords)
        {
            return new Coords(Location.X + coords.X, Location.Y + coords.Y);
        }

        private readonly Lazy<int> _lazyWidth;
        private readonly Lazy<int> _lazyHeight;
        private readonly Lazy<IImmutableList<Coords>> _lazyInteriorJunctionPoints;
        private readonly Lazy<IEnumerable<IImmutableList<Coords>>> _lazyLines;

        private int CalculateWidth()
        {
            var coords = TetraStick.Lines.SelectMany(line => line).ToImmutableList();
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
            var coords = TetraStick.Lines.SelectMany(line => line).ToImmutableList();
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

        private IImmutableList<Coords> CalculateInteriorJunctionPoints()
        {
            return TetraStick.InteriorJunctionPoints.Select(ApplyTransform).ToImmutableList();
        }

        private IEnumerable<IImmutableList<Coords>> CalculateLines()
        {
            return TetraStick.Lines
                .Select(line => line.Select(ApplyTransform).ToImmutableList() as IImmutableList<Coords>);
        }

        public override string ToString()
        {
            return $"TetraStick: {TetraStick.Tag}; Location: {Location}; Orientation: {Orientation}; ReflectionMode: {ReflectionMode}";
        }
    }
}
