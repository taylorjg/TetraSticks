using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace TetraSticks.Model
{
    public static class RowBuilder
    {
        public static IImmutableList<PlacedTetraStick> BuildRows(IImmutableList<TetraStick> tetraSticks)
        {
            var locations = (
                from x in Enumerable.Range(0, 5)
                from y in Enumerable.Range(0, 5)
                select new Coords(x, y)
                ).ToImmutableList();
            var orientations = Enum.GetValues(typeof (Orientation)).Cast<Orientation>().ToImmutableList();
            var reflectionModes = Enum.GetValues(typeof (ReflectionMode)).Cast<ReflectionMode>().ToImmutableList();

            var placedTetraSticks =
                from tetraStick in tetraSticks
                from location in locations
                from orientation in orientations
                from reflectionMode in reflectionModes
                let placedTetraStick = new PlacedTetraStick(tetraStick, location, orientation, reflectionMode)
                where IsFullyWithinGrid(placedTetraStick)
                select placedTetraStick;

            placedTetraSticks = placedTetraSticks.Distinct(new PlacedTetraStickComparer());
            placedTetraSticks = PinTetraStickToBeOrientedNorth(tetraSticks.First(), placedTetraSticks);

            return placedTetraSticks.ToImmutableList();
        }

        private static IEnumerable<PlacedTetraStick> PinTetraStickToBeOrientedNorth(
            TetraStick firstTetraStick,
            IEnumerable<PlacedTetraStick> placedTetraSticks)
        {
            var firstTag = firstTetraStick.Tag;

            Func<PlacedTetraStick, bool> isNotFirst = pts => pts.TetraStick.Tag != firstTag;
            Func<PlacedTetraStick, bool> isOrientedNorth = pts => pts.Orientation == Orientation.North;

            return placedTetraSticks.Where(pts => isNotFirst(pts) || isOrientedNorth(pts));
        }

        private static bool IsFullyWithinGrid(PlacedTetraStick placedTetraStick)
        {
            var location = placedTetraStick.Location;

            return location.X >= 0 && location.X < 5 &&
                   location.Y >= 0 && location.Y < 5 &&
                   location.X + placedTetraStick.Width <= 5 &&
                   location.Y + placedTetraStick.Height <= 5;
        }

        private class PlacedTetraStickComparer : IEqualityComparer<PlacedTetraStick>
        {
            public bool Equals(PlacedTetraStick x, PlacedTetraStick y)
            {
                if (x.TetraStick.Tag != y.TetraStick.Tag) return false;
                var xPoints = x.Lines.SelectMany(line => line).Distinct().ToImmutableList();
                var yPoints = y.Lines.SelectMany(line => line).Distinct().ToImmutableList();
                return !xPoints.Except(yPoints).Any() && !yPoints.Except(xPoints).Any();
            }

            public int GetHashCode(PlacedTetraStick placedTetraStick)
            {
                return placedTetraStick.TetraStick.Tag.GetHashCode();
            }
        }
    }
}
