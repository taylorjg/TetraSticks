﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TetraSticks.Model
{
    public class RotatedTetraStick
    {
        public RotatedTetraStick(TetraStick tetraStick, Orientation orientation, bool reflected = false)
        {
            TetraStick = tetraStick;
            Orientation = orientation;
            Reflected = reflected;
            _lazyWidth = new Lazy<int>(CalculateWidth);
            _lazyHeight = new Lazy<int>(CalculateHeight);
            _lazyInteriorJunctionPoints = new Lazy<IEnumerable<Coords>>(CalculateInteriorJunctionPoints);
            _lazyLines = new Lazy<IEnumerable<IEnumerable<Coords>>>(CalculateLines);
        }

        public string Tag => TetraStick.Tag;
        public IEnumerable<Coords> InteriorJunctionPoints => _lazyInteriorJunctionPoints.Value;
        public IEnumerable<IEnumerable<Coords>> Lines => _lazyLines.Value;
        private TetraStick TetraStick { get; }
        private Orientation Orientation { get; }
        private bool Reflected { get; }
        private int Width => _lazyWidth.Value;
        private int Height => _lazyHeight.Value;

        private Coords TransformCoords(Coords coords)
        {
            switch (Orientation)
            {
                case Orientation.North:
                    return ApplyReflectionMode(coords);

                case Orientation.South:
                    return ApplyReflectionMode(new Coords(Width - coords.X, Height - coords.Y));

                case Orientation.East:
                    return ApplyReflectionMode(new Coords(coords.Y, Height - coords.X));

                case Orientation.West:
                    return ApplyReflectionMode(new Coords(Width - coords.Y, coords.X));

                default:
                    throw new InvalidOperationException($"Unknown orientation, \"{Orientation}\".");
            }
        }

        private Coords ApplyReflectionMode(Coords coords)
        {
            return Reflected ? new Coords(Width - coords.X, coords.Y) : coords;
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
            return TetraStick.InteriorJunctionPoints.Select(TransformCoords);
        }

        private IEnumerable<IEnumerable<Coords>> CalculateLines()
        {
            return TetraStick.Lines.Select(line => line.Select(TransformCoords));
        }
    }
}
