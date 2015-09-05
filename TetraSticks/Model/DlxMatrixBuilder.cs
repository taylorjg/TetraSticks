using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TetraSticks.Model
{
    public static class DlxMatrixBuilder
    {
        public static IEnumerable<List<int>> BuildDlxMatrix(IEnumerable<TetraStick> tetraSticks, IEnumerable<PlacedTetraStick> placedTetraSticks)
        {
            var tagToIndexDict = tetraSticks
                .Zip(Enumerable.Range(0, int.MaxValue), Tuple.Create)
                .ToDictionary(t => t.Item1.Tag, t => t.Item2);
            return placedTetraSticks.Select(placedTetraStick => BuildDlxMatrixRow(tagToIndexDict, placedTetraStick));
        }

        private static string BitsToString(IEnumerable<int> xs)
        {
            return string.Join("", xs.Select(x => Convert.ToString(x)));
        }

        private static void DumpLines(IEnumerable<IEnumerable<Coords>> lines)
        {
            foreach (var line in lines)
                DumpLine(line);
        }

        private static void DumpLine(IEnumerable<Coords> line)
        {
            var pts = string.Join(", ", line.Select(pt => pt.ToString()));
            Debug.WriteLine($"[{pts}]");
        }

        private static void DumpSegments(Tuple<IEnumerable<Tuple<Coords, Coords>>, IEnumerable<Tuple<Coords, Coords>>> t)
        {
            Debug.WriteLine("hs:");
            DumpSegments(t.Item1);

            Debug.WriteLine("vs:");
            DumpSegments(t.Item2);
        }

        private static void DumpSegments(IEnumerable<Tuple<Coords, Coords>> segments)
        {
            var pts = string.Join(", ", segments.Select(pair => $"{pair.Item1} => {pair.Item2}"));
            Debug.WriteLine($"[{pts}]");
        }

        private static List<int> BuildDlxMatrixRow(Dictionary<string, int> tagToIndexDict, PlacedTetraStick placedTetraStick)
        {
            // 15 columns for the 15 tetra sticks (map tag name to index, map is zip of tetraSticks with [0..])
            // 25 columns for Hxy (0 <= x < 5, 0 <= y < 5) index is x*5+y
            // 25 columns for Vxy (0 <= x < 5, 0 <= y < 5) index is x*5+y
            // 16 columns for Ixy (0 < x < 5, 0 < y < 5)   index is (x-1)*4+(y-1)

            var arr1 = new int[15]; // the 15 tetra sticks
            var arr2 = new int[25]; // Hxy where 0 <= x < 5, 0 <= y < 5
            var arr3 = new int[25]; // Vxy where 0 <= x < 5, 0 <= y < 5
            // var arr4 = new int[16]; // Ixy where 0 < x < 5, 0 < y < 5

            arr1[tagToIndexDict[placedTetraStick.Tag]] = 1;

            Debug.WriteLine("Before normalisation:");
            DumpLines(placedTetraStick.Lines);

            var t = NormaliseLines(placedTetraStick.Lines);

            Debug.WriteLine("After normalisation:");
            DumpSegments(t);

            var hs = t.Item1;
            var vs = t.Item2;

            foreach (var h in hs)
            {
                var x = h.Item1.X;
                var y = h.Item1.Y;
                arr2[x*5 + y] = 1;
            }

            foreach (var v in vs)
            {
                var x = v.Item1.X;
                var y = v.Item1.Y;
                arr3[x*5 + y] = 1;
            }

            // foreach (var i in placedTetraStick.InteriorJunctionPoints)
            // {
            //     // check that i is not on the edge
            //     // set appropriate index in arr4
            // }

            // var arrs = new[] {arr1, arr2, arr3, arr4};
            var arrs = new[] {arr1, arr2, arr3};

            Debug.WriteLine($"{BitsToString(arr1)} {BitsToString(arr2)} {BitsToString(arr3)}");

            return arrs.SelectMany(arr => arr).ToList();
        }

        private static Tuple<IEnumerable<Tuple<Coords, Coords>>, IEnumerable<Tuple<Coords, Coords>>> NormaliseLines(IEnumerable<IEnumerable<Coords>> lines)
        {
            var segments = lines.SelectMany(NormaliseLine).Distinct().ToList();
            Debug.Assert(segments.Count == 4);
            var hs = segments.Where(pair => pair.Item1.Y == pair.Item2.Y);
            var vs = segments.Where(pair => pair.Item1.X == pair.Item2.X);
            return Tuple.Create(hs, vs);
        }

        private static IEnumerable<Tuple<Coords, Coords>> NormaliseLine(IEnumerable<Coords> line)
        {
            return LineToSegments(line).Select(NormaliseSegment);
        }

        private static IEnumerable<Tuple<Coords, Coords>> LineToSegments(IEnumerable<Coords> line)
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

        private class PlacedTetraStickComparer : IEqualityComparer<PlacedTetraStick>
        {
            public bool Equals(PlacedTetraStick x, PlacedTetraStick y)
            {
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
