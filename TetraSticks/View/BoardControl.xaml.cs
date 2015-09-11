using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using TetraSticks.Model;
using TetraSticks.ViewModel;

namespace TetraSticks.View
{
    public partial class BoardControl : IBoardControl
    {
        private readonly Color _gridColour = Color.FromArgb(0x80, 0xCD, 0x85, 0x3F);
        private const int GridLineThickness = 4;
        private const int GridLineHalfThickness = GridLineThickness / 2;
        private const int GridLineDoubleThickness = GridLineThickness * 2;
        private const int TetraStickThickness = 8;
        private const int TetraStickHalfThickness = TetraStickThickness / 2;
        private const int TetraStickInset = TetraStickThickness * 2;
        private double _sw;
        private double _sh;

        private enum TagType
        {
            GridLine,
            TetraStick
        }

        private enum Direction
        {
            Left,
            Right,
            Up,
            Down
        };

        private enum Disposition
        {
            First,
            Middle,
            Last
        }

        public BoardControl()
        {
            InitializeComponent();
        }

        public void DrawGrid()
        {
            DrawGridLines();
        }

        public void Clear()
        {
            RemoveChildrenWithTagType(TagType.TetraStick);
        }

        public void DrawPlacedTetraSticks(IEnumerable<PlacedTetraStick> placedTetraSticks)
        {
            foreach (var placedTetraStick in placedTetraSticks)
                DrawPlacedTetraStick(placedTetraStick);
        }

        private void DrawPlacedTetraStick(PlacedTetraStick placedTetraStick)
        {
            var colour = TetraStickColours.TetraStickToColour(placedTetraStick.TetraStick);
            foreach (var line in placedTetraStick.Lines)
                DrawLine(colour, line);
        }

        private void DrawLine(Color colour, IEnumerable<Coords> coords)
        {
            var aw = ActualWidth;
            var ah = ActualHeight;
            var sw = (aw - GridLineThickness) / 5;
            var sh = (ah - GridLineThickness) / 5;

            IImmutableList<Point> transformedPts = coords
                .Select(coord => new Point(
                    (coord.X)*sw + GridLineHalfThickness,
                    (5 - coord.Y)*sh + GridLineHalfThickness))
                .ToImmutableList();

            transformedPts = PullLineEndsIn(transformedPts);

            var polyLineSegment = new PolyLineSegment(transformedPts, true);
            var pathFigure = new PathFigure { StartPoint = transformedPts.First() };
            pathFigure.Segments.Add(polyLineSegment);
            var pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);
            var path = new Path
            {
                Stroke = new SolidColorBrush(colour),
                StrokeThickness = GridLineDoubleThickness,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeLineJoin = PenLineJoin.Round,
                Data = pathGeometry,
                Tag = TagType.TetraStick
            };
            BoardCanvas.Children.Add(path);
        }

        private void DrawPlacedTetraStick2(PlacedTetraStick placedTetraStick)
        {
            var colour = TetraStickColours.TetraStickToColour(placedTetraStick.TetraStick);
            var path = new Path
            {
                Data = CombineGeometries(placedTetraStick.Lines.Select(LineToPathGeometry).ToImmutableList()),
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(Colors.Black),
                Fill = new SolidColorBrush(colour)
            };
            BoardCanvas.Children.Add(path);
        }

        private PathGeometry LineToPathGeometry(IImmutableList<Coords> line)
        {
            var lineSegments = LineToLineSegments(line);
            var combinedLineSegments = CombineConsecutiveLineSegments(lineSegments);
            var segments = BuildPathSegmentCollection(combinedLineSegments);
            var sortedSegments = SortPathSegmentCollection(segments);
            return new PathGeometry
            {
                Figures = new PathFigureCollection
                {
                    new PathFigure
                    {
                        StartPoint = new Point(0, 0), // TODO: use proper value here...
                        Segments = sortedSegments
                    }
                }
            };
        }

        private static IImmutableList<Tuple<Coords, Coords, Direction, Disposition>> LineToLineSegments(IImmutableList<Coords> coords)
        {
            Func<Coords, Coords, Direction> f1 = (c1, c2) =>
            {
                if (c1.Y == c2.Y && c1.X > c2.X) return Direction.Left;
                if (c1.Y == c2.Y && c1.X < c2.X) return Direction.Right;
                if (c1.X == c2.X && c1.Y < c2.Y) return Direction.Up;
                if (c1.X == c2.X && c1.Y > c2.Y) return Direction.Down;
                throw new InvalidOperationException("...");
            };

            Func<int, Disposition> f2 = i =>
            {
                if (i == 1) return Disposition.First;
                if (i == coords.Count - 1) return Disposition.Last;
                return Disposition.Middle;
            };

            var result = new List<Tuple<Coords, Coords, Direction, Disposition>>();

            for (var i = 1; i < coords.Count; i++)
            {
                var coords1 = coords[i - 1];
                var coords2 = coords[i];
                var direction = f1(coords1, coords2);
                var disposition = f2(i);
                result.Add(Tuple.Create(coords1, coords2, direction, disposition));
            }

            return result.ToImmutableList();
        }

        private static IImmutableList<Tuple<Coords, Coords, Direction, Disposition>> CombineConsecutiveLineSegments(IImmutableList<Tuple<Coords, Coords, Direction, Disposition>> lineSegments)
        {
            var result = new List<Tuple<Coords, Coords, Direction, Disposition>>();

            var currentLineSegments = new List<Tuple<Coords, Coords, Direction, Disposition>> {lineSegments[0]};
            for (var i = 1; i < lineSegments.Count; i++)
            {
                var nextLineSegment = lineSegments[i];
                if (nextLineSegment.Item3 == currentLineSegments[0].Item3)
                {
                    currentLineSegments.Add(nextLineSegment);
                }
                else
                {
                    var combinedLineSegment = Tuple.Create(
                        currentLineSegments.First().Item1,
                        currentLineSegments.Last().Item2,
                        currentLineSegments.First().Item3,
                        Disposition.Middle);
                    result.Add(combinedLineSegment);
                    currentLineSegments = new List<Tuple<Coords, Coords, Direction, Disposition>> {nextLineSegment};
                }
            }

            if (currentLineSegments.Any())
            {
                var combinedLineSegment = Tuple.Create(
                    currentLineSegments.First().Item1,
                    currentLineSegments.Last().Item2,
                    currentLineSegments.First().Item3,
                    Disposition.Middle);
                result.Add(combinedLineSegment);
            }

            result[0] = Tuple.Create(result.First().Item1, result.First().Item2, result.First().Item3, Disposition.First);
            result[result.Count - 1] = Tuple.Create(result.Last().Item1, result.Last().Item2, result.Last().Item3, Disposition.Last);

            // What about case of I where there will be only 1 combined segment ?
            // Disposition.First and Disposition.Last ???
            // I guess we could make Disposition a bit flags enum ?
            // Or add a new enum value FirstAndLast ?
            // Or could we get away with simply Middle/End ?

            return result.ToImmutableList();
        }

        private PathSegmentCollection BuildPathSegmentCollection(IImmutableList<Tuple<Coords, Coords, Direction, Disposition>> lineSegments)
        {
            return null;
        }

        private static PathSegmentCollection SortPathSegmentCollection(PathSegmentCollection segments)
        {
            var sortedSegments = new List<PathSegment> {segments.First()};
            var n = segments.Count - 2;
            Debug.Assert(n%2 == 0);
            var m = n/2;
            var v1 = Enumerable.Range(0, m).ToList();
            var v2 = v1.Select(x => x * 2 + 1);
            var v3 = v1.Select(x => x * 2 + 2);
            sortedSegments.AddRange(v2.Select(x => segments[x]));
            sortedSegments.Add(segments.Last());
            sortedSegments.AddRange(v3.Select(x => segments[x]).Reverse());
            return new PathSegmentCollection(sortedSegments);
        }

        private static CombinedGeometry CombineGeometries(IReadOnlyList<Geometry> geometries)
        {
            Debug.Assert(geometries.Count >= 2);
            var seed = new CombinedGeometry(GeometryCombineMode.Union, geometries[0], geometries[1]);
            return geometries.Skip(2).Aggregate(seed, (cg, g) => new CombinedGeometry(GeometryCombineMode.Union, cg, g));
        }

        private void AddEndCapLeft(PathSegmentCollection segments, Coords coords)
        {
            segments.Add(new ArcSegment
            {
                Point = CoordsToInsetLowerLeftHorizontal(coords),
                Size = new Size(TetraStickHalfThickness, TetraStickHalfThickness)
            });
        }

        private void AddEndCapRight(PathSegmentCollection segments, Coords coords)
        {
            segments.Add(new ArcSegment
            {
                Point = CoordsToInsetUpperRightHorizontal(coords),
                Size = new Size(TetraStickHalfThickness, TetraStickHalfThickness)
            });
        }

        private void AddEndCapTop(PathSegmentCollection segments, Coords coords)
        {
        }

        private void AddEndCapBottom(PathSegmentCollection segments, Coords coords)
        {
        }

        private void AddLineHorizontal(PathSegmentCollection segments, Coords coords1, Coords coords2)
        {
            segments.Add(new LineSegment
            {
                Point = CoordsToInsetLowerRightHorizontal(coords2)
            });

            segments.Add(new LineSegment
            {
                Point = CoordsToInsetUpperLeftHorizontal(coords1)
            });
        }

        private void AddLineVertical(PathSegmentCollection segments, Coords coords1, Coords coords2)
        {
        }

        private void AddCornerRightThenUp(PathSegmentCollection segments, Coords coords)
        {
        }

        private void AddCornerRightThenDown(PathSegmentCollection segments, Coords coords)
        {
        }

        private void AddCornerLeftThenUp(PathSegmentCollection segments, Coords coords)
        {
        }

        private void AddCornerLeftThenDown(PathSegmentCollection segments, Coords coords)
        {
        }

        private void AddCornerUpThenLeft(PathSegmentCollection segments, Coords coords)
        {
        }

        private void AddCornerUpThenRight(PathSegmentCollection segments, Coords coords)
        {
        }

        private void AddCornerDownThenLeft(PathSegmentCollection segments, Coords coords)
        {
        }

        private void AddCornerDownThenRight(PathSegmentCollection segments, Coords coords)
        {
        }

        private Point CoordsToInsetUpperLeftHorizontal(Coords coords)
        {
            return new Point(
                coords.X * _sw + GridLineHalfThickness + TetraStickInset,
                (5 - coords.Y) * _sh + GridLineHalfThickness - TetraStickHalfThickness);
        }

        private Point CoordsToInsetLowerLeftHorizontal(Coords coords)
        {
            return new Point(
                coords.X * _sw + GridLineHalfThickness + TetraStickInset,
                (5 - coords.Y) * _sh + GridLineHalfThickness + TetraStickHalfThickness);
        }

        private Point CoordsToInsetUpperRightHorizontal(Coords coords)
        {
            return new Point(
                coords.X * _sw + GridLineHalfThickness - TetraStickInset,
                (5 - coords.Y) * _sh + GridLineHalfThickness - TetraStickHalfThickness);
        }

        private Point CoordsToInsetLowerRightHorizontal(Coords coords)
        {
            return new Point(
                coords.X * _sw + GridLineHalfThickness - TetraStickInset,
                (5 - coords.Y) * _sh + GridLineHalfThickness + TetraStickHalfThickness);
        }

        public void CombinedGeometryExperiment()
        {
            var coords1 = new Coords(2, 2);
            var coords2 = new Coords(3, 2);

            var g1 = new PathGeometry();
            var pf1 = new PathFigure
            {
                StartPoint = CoordsToInsetUpperLeftHorizontal(coords1)
            };
            AddEndCapLeft(pf1.Segments, coords1);
            AddLineHorizontal(pf1.Segments, coords1, coords2);
            //pf1.Segments.Add(new LineSegment
            //{
            //    Point = CoordsToInsetLowerRightHorizontal(coords2)
            //});
            AddEndCapRight(pf1.Segments, coords2);
            //pf1.Segments.Add(new LineSegment
            //{
            //    Point = CoordsToInsetUpperLeftHorizontal(coords1)
            //});
            var sortedPf1Segments = SortPathSegmentCollection(pf1.Segments);
            pf1.Segments = sortedPf1Segments;
            g1.Figures.Add(pf1);

            var coords3 = new Coords(0, 4);
            var coords4 = new Coords(3, 4);

            var g2 = new PathGeometry();
            var pf2 = new PathFigure
            {
                StartPoint = CoordsToInsetUpperLeftHorizontal(coords3)
            };
            AddEndCapLeft(pf2.Segments, coords3);
            pf2.Segments.Add(new LineSegment
            {
                Point = CoordsToInsetLowerRightHorizontal(coords4)
            });
            AddEndCapRight(pf2.Segments, coords4);
            pf2.Segments.Add(new LineSegment
            {
                Point = CoordsToInsetUpperLeftHorizontal(coords3)
            });
            g2.Figures.Add(pf2);

            var path = new Path
            {
                Data = CombineGeometries(ImmutableList.Create(g1, g2)),
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(Colors.Black),
                Fill = new SolidColorBrush(TetraStickColours.TetraStickToColour(Model.TetraSticks.V))
            };

            BoardCanvas.Children.Add(path);
        }

        private static IImmutableList<Point> PullLineEndsIn(IImmutableList<Point> transformedPts)
        {
            return PullEndIn(PullStartIn(transformedPts));
        }

        private static IImmutableList<Point> PullStartIn(IImmutableList<Point> transformedPts)
        {
            var newStart = PullPointIn(transformedPts[0], transformedPts[1]);
            var rest = transformedPts.Skip(1);
            return ImmutableList.CreateRange(new[] {newStart}.Concat(rest));
        }

        private static IImmutableList<Point> PullEndIn(IImmutableList<Point> transformedPts)
        {
            var count = transformedPts.Count;
            var newEnd = PullPointIn(transformedPts[count - 1], transformedPts[count - 2]);
            var rest = transformedPts.Take(transformedPts.Count - 1);
            return ImmutableList.CreateRange(rest.Concat(new[] {newEnd}));
        }

        private static Point PullPointIn(Point pt1, Point pt2)
        {
            // Horizontal
            if (Math.Abs(pt1.Y - pt2.Y) < 0.001)
            {
                return pt1.X > pt2.X ? new Point(pt1.X - GridLineDoubleThickness, pt1.Y) : new Point(pt1.X + GridLineDoubleThickness, pt1.Y);
            }

            // Vertical
            if (Math.Abs(pt1.X - pt2.X) < 0.001)
            {
                return pt1.Y > pt2.Y ? new Point(pt1.X, pt1.Y - GridLineDoubleThickness) : new Point(pt1.X, pt1.Y + GridLineDoubleThickness);
            }

            throw new InvalidOperationException("...");
        }

        private void DrawGridLines()
        {
            var aw = ActualWidth;
            var ah = ActualHeight;
            var sw = (aw - GridLineThickness) / 5;
            var sh = (ah - GridLineThickness) / 5;
            _sw = sw;
            _sh = sh;

            var gridLineBrush = new SolidColorBrush(_gridColour);

            // Horizontal grid lines
            for (var row = 0; row <= 5; row++)
            {
                var line = new Line
                {
                    Stroke = gridLineBrush,
                    StrokeThickness = GridLineThickness,
                    X1 = 0,
                    Y1 = row * sh + GridLineHalfThickness,
                    X2 = aw,
                    Y2 = row * sh + GridLineHalfThickness,
                    Tag = TagType.GridLine
                };
                BoardCanvas.Children.Add(line);
            }

            // Vertical grid lines
            for (var col = 0; col <= 5; col++)
            {
                var line = new Line
                {
                    Stroke = gridLineBrush,
                    StrokeThickness = GridLineThickness,
                    X1 = col * sw + GridLineHalfThickness,
                    Y1 = 0,
                    X2 = col * sw + GridLineHalfThickness,
                    Y2 = ah,
                    Tag = TagType.GridLine
                };
                BoardCanvas.Children.Add(line);
            }
        }

        private void RemoveChildrenWithTagType(TagType tagType)
        {
            var elementsToRemove = new List<UIElement>();

            // ReSharper disable LoopCanBeConvertedToQuery
            foreach (var element in BoardCanvas.Children)
            {
                var frameworkElement = element as FrameworkElement;
                if (frameworkElement?.Tag as TagType? == tagType)
                {
                    elementsToRemove.Add(frameworkElement);
                }
            }
            // ReSharper restore LoopCanBeConvertedToQuery

            foreach (var element in elementsToRemove)
            {
                BoardCanvas.Children.Remove(element);
            }
        }
    }
}
