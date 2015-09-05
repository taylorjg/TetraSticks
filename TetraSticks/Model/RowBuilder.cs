using System;
using System.Collections.Generic;
using System.Linq;

namespace TetraSticks.Model
{
    public static class RowBuilder
    {
        public static IEnumerable<PlacedTetraStick> BuildRows(IEnumerable<TetraStick> tetraSticks)
        {
            var locations =
                (from x in Enumerable.Range(0, 5)
                from y in Enumerable.Range(0, 5)
                select new Coords(x, y)).ToList();
            var orientations = Enum.GetValues(typeof (Orientation)).Cast<Orientation>().ToList();
            var reflectionModes = Enum.GetValues(typeof (ReflectionMode)).Cast<ReflectionMode>().ToList();

            var placedTetraSticks =
                from tetraStick in tetraSticks
                from location in locations
                from orientation in orientations
                from reflectionMode in reflectionModes
                let placedTetraStick = new PlacedTetraStick(tetraStick, location, orientation, reflectionMode)
                where IsFullyWithinGrid(placedTetraStick)
                select placedTetraStick;

            return placedTetraSticks.Distinct(new PlacedTetraStickComparer());
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
                if (x.Tag != y.Tag) return false;
                var xPoints = x.Lines.SelectMany(line => line).Distinct().ToList();
                var yPoints = y.Lines.SelectMany(line => line).Distinct().ToList();
                return !xPoints.Except(yPoints).Any() && !yPoints.Except(xPoints).Any();
            }

            public int GetHashCode(PlacedTetraStick placedTetraStick)
            {
                return placedTetraStick.Tag.GetHashCode();
            }
        }
    }
}
