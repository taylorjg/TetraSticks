using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace TetraSticks.Model
{
    public static class DlxMatrixBuilder
    {
        public static IImmutableList<IImmutableList<int>> BuildDlxMatrix(
            IEnumerable<TetraStick> tetraSticks,
            IEnumerable<PlacedTetraStick> placedTetraSticks)
        {
            var tagToIndexDict = tetraSticks
                .Zip(Enumerable.Range(0, int.MaxValue), Tuple.Create)
                .ToImmutableDictionary(t => t.Item1.Tag, t => t.Item2);

            return placedTetraSticks
                .Select(placedTetraStick => BuildDlxMatrixRow(tagToIndexDict, placedTetraStick))
                .ToImmutableList();
        }

        private static IImmutableList<int> BuildDlxMatrixRow(
            IReadOnlyDictionary<string, int> tagToIndexDict,
            PlacedTetraStick placedTetraStick)
        {
            var arr1 = new int[15]; // the 15 tetra sticks
            var arr2 = new int[30]; // Hxy where 0 <= x < 5, 0 <= y <= 5
            var arr3 = new int[30]; // Vxy where 0 <= x <= 5, 0 <= y < 5
            var arr4 = new int[16]; // Ixy where 0 < x < 5, 0 < y < 5

            arr1[tagToIndexDict[placedTetraStick.TetraStick.Tag]] = 1;

            var tuple = NormaliseLines(placedTetraStick.Lines);
            var hs = tuple.Item1;
            var vs = tuple.Item2;

            foreach (var h in hs)
            {
                var x = h.Item1.X;
                var y = h.Item1.Y;
                arr2[y*5 + x] = 1;
            }

            foreach (var v in vs)
            {
                var x = v.Item1.X;
                var y = v.Item1.Y;
                arr3[x*5 + y] = 1;
            }

            foreach (var pt in placedTetraStick.InteriorJunctionPoints)
            {
                if (pt.X > 0 && pt.X < 5 && pt.Y > 0 && pt.Y < 5)
                {
                    var x = pt.X - 1;
                    var y = pt.Y - 1;
                    arr4[x*4 + y] = 1;
                }
            }

            var arrs = new[] {arr1, arr2, arr3, arr4};

            return arrs.SelectMany(arr => arr).ToImmutableList();
        }

        private static Tuple<IEnumerable<Tuple<Coords, Coords>>, IEnumerable<Tuple<Coords, Coords>>> NormaliseLines(
            IEnumerable<IImmutableList<Coords>> lines)
        {
            var segments = lines.SelectMany(NormaliseLine).Distinct().ToImmutableList();
            Debug.Assert(segments.Count == 4);
            var hs = segments.Where(pair => pair.Item1.Y == pair.Item2.Y);
            var vs = segments.Where(pair => pair.Item1.X == pair.Item2.X);
            return Tuple.Create(hs, vs);
        }

        private static IEnumerable<Tuple<Coords, Coords>> NormaliseLine(IImmutableList<Coords> line)
        {
            return LineToSegments(line).Select(NormaliseSegment);
        }

        private static IEnumerable<Tuple<Coords, Coords>> LineToSegments(IImmutableList<Coords> line)
        {
            var seed = new List<Tuple<Coords, Coords>> { Tuple.Create(new Coords(999, 999), line.First()) };
            var tuples = line.Skip(1).Aggregate(seed, (acc, current) =>
            {
                var previous = acc.Last().Item2;
                acc.Add(Tuple.Create(previous, current));
                return acc;
            });
            return tuples.Skip(1);
        }

        private static Tuple<Coords, Coords> NormaliseSegment(Tuple<Coords, Coords> pair)
        {
            // Hxy
            if (pair.Item1.Y == pair.Item2.Y)
            {
                Debug.Assert(Math.Abs(pair.Item1.X - pair.Item2.X) == 1);
                return pair.Item2.X > pair.Item1.X ? pair : Tuple.Create(pair.Item2, pair.Item1);
            }

            // Vxy
            if (pair.Item1.X == pair.Item2.X)
            {
                Debug.Assert(Math.Abs(pair.Item1.Y - pair.Item2.Y) == 1);
                return pair.Item2.Y > pair.Item1.Y ? pair : Tuple.Create(pair.Item2, pair.Item1);
            }

            throw new InvalidOperationException($"Found a segment that is not horizontal or vertical: ({pair.Item1}, {pair.Item2})");
        }
    }
}
