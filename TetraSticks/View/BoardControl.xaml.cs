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
                        StartPoint = GetStartingPoint(combinedLineSegments),
                        Segments = sortedSegments
                    }
                }
            };
        }

        private Point GetStartingPoint(IEnumerable<Tuple<Coords, Coords, Direction>> line)
        {
            var first = line.First();
            switch (first.Item3)
            {
                case Direction.Left:
                    return CoordsToInsetLowerRightHorizontal(first.Item1);
                case Direction.Right:
                    return CoordsToInsetUpperLeftHorizontal(first.Item1);
                case Direction.Up:
                    return CoordsToInsetLowerLeftVertical(first.Item1);
                case Direction.Down:
                    return CoordsToInsetUpperRightVertical(first.Item1);
                default:
                    throw new InvalidOperationException("...");
            }
        }

        private static IImmutableList<Tuple<Coords, Coords, Direction>> LineToLineSegments(IImmutableList<Coords> coords)
        {
            Func<Coords, Coords, Direction> calculateDirection = (c1, c2) =>
            {
                if (c1.Y == c2.Y && c1.X > c2.X) return Direction.Left;
                if (c1.Y == c2.Y && c1.X < c2.X) return Direction.Right;
                if (c1.X == c2.X && c1.Y < c2.Y) return Direction.Up;
                if (c1.X == c2.X && c1.Y > c2.Y) return Direction.Down;
                throw new InvalidOperationException("...");
            };

            var result = new List<Tuple<Coords, Coords, Direction>>();

            // Can this be done using Aggregate ?
            for (var i = 1; i < coords.Count; i++)
            {
                var coords1 = coords[i - 1];
                var coords2 = coords[i];
                var direction = calculateDirection(coords1, coords2);
                result.Add(Tuple.Create(coords1, coords2, direction));
            }

            return result.ToImmutableList();
        }

        private static IImmutableList<Tuple<Coords, Coords, Direction>> CombineConsecutiveLineSegments(IImmutableList<Tuple<Coords, Coords, Direction>> lineSegments)
        {
            var result = new List<Tuple<Coords, Coords, Direction>>();

            var currentLineSegments = new List<Tuple<Coords, Coords, Direction>> {lineSegments[0]};

            // Can this be done using Aggregate ?
            for (var i = 1; i < lineSegments.Count; i++)
            {
                var nextLineSegment = lineSegments[i];

                if (nextLineSegment.Item3 == currentLineSegments[0].Item3)
                {
                    currentLineSegments.Add(nextLineSegment);
                }
                else
                {
                    // TODO: Extract this duplicated code into a local Func ?
                    var combinedLineSegment = Tuple.Create(
                        currentLineSegments.First().Item1,
                        currentLineSegments.Last().Item2,
                        currentLineSegments.First().Item3);
                    result.Add(combinedLineSegment);

                    currentLineSegments = new List<Tuple<Coords, Coords, Direction>> {nextLineSegment};
                }
            }

            if (currentLineSegments.Any())
            {
                // TODO: Extract this duplicated code into a local Func ?
                var combinedLineSegment = Tuple.Create(
                    currentLineSegments.First().Item1,
                    currentLineSegments.Last().Item2,
                    currentLineSegments.First().Item3);
                result.Add(combinedLineSegment);
            }

            return result.ToImmutableList();
        }

        private PathSegmentCollection BuildPathSegmentCollection(IImmutableList<Tuple<Coords, Coords, Direction>> lineSegments)
        {
            var segments = new PathSegmentCollection();
            for (var i = 0; i < lineSegments.Count; i++)
            {
                var lineSegment = lineSegments[i];
                var isFirst = i == 0;
                var isLast = i == lineSegments.Count - 1;

                AddAppropriateStartEndCap(segments, lineSegment, isFirst);
                AddAppropriateLine(segments, lineSegment);
                AddAppropriateCorner(segments, lineSegment, isLast);
                AddAppropriateFinishEndCap(segments, lineSegment, isLast);
            }
            return segments;
        }

        private void AddAppropriateStartEndCap(PathSegmentCollection segments, Tuple<Coords, Coords, Direction> lineSegment, bool isFirst)
        {
            if (!isFirst) return;
            switch (lineSegment.Item3)
            {
                case Direction.Left:
                    AddEndCapRight(segments, lineSegment.Item1);
                    break;
                case Direction.Right:
                    AddEndCapLeft(segments, lineSegment.Item1);
                    break;
                case Direction.Up:
                    AddEndCapBottom(segments, lineSegment.Item1);
                    break;
                case Direction.Down:
                    AddEndCapTop(segments, lineSegment.Item1);
                    break;
                default:
                    throw new InvalidOperationException("...");
            }
        }

        private void AddAppropriateFinishEndCap(PathSegmentCollection segments, Tuple<Coords, Coords, Direction> lineSegment, bool isLast)
        {
            if (!isLast) return;
            switch (lineSegment.Item3)
            {
                case Direction.Left:
                    AddEndCapLeft(segments, lineSegment.Item2);
                    break;
                case Direction.Right:
                    AddEndCapRight(segments, lineSegment.Item2);
                    break;
                case Direction.Up:
                    AddEndCapTop(segments, lineSegment.Item2);
                    break;
                case Direction.Down:
                    AddEndCapBottom(segments, lineSegment.Item2);
                    break;
                default:
                    throw new InvalidOperationException("...");
            }
        }

        private void AddAppropriateLine(PathSegmentCollection segments, Tuple<Coords, Coords, Direction> lineSegment)
        {
            switch (lineSegment.Item3)
            {
                case Direction.Left:
                    AddLineLeftHorizontal(segments, lineSegment.Item1, lineSegment.Item2);
                    break;
                case Direction.Right:
                    AddLineRightHorizontal(segments, lineSegment.Item1, lineSegment.Item2);
                    break;
                case Direction.Up:
                    AddLineUpVertical(segments, lineSegment.Item1, lineSegment.Item2);
                    break;
                case Direction.Down:
                    AddLineDownVertical(segments, lineSegment.Item1, lineSegment.Item2);
                    break;
                default:
                    throw new InvalidOperationException("...");
            }
        }

        private void AddAppropriateCorner(PathSegmentCollection segments, Tuple<Coords, Coords, Direction> lineSegment, bool isLast)
        {
            if (isLast) return;
            AddCornerRightThenUp(segments, lineSegment.Item2);
        }

        private static PathSegmentCollection SortPathSegmentCollection(PathSegmentCollection segments)
        {
            var n = segments.Count - 2;
            Debug.Assert(n % 2 == 0);
            var m = n / 2;

            // TODO: use better variables for v1, v2 and v3
            var v1 = Enumerable.Range(0, m).ToList();
            var v2 = v1.Select(x => x * 2 + 1);
            var v3 = v1.Select(x => x * 2 + 2);

            var sortedSegments = new List<PathSegment> { segments.First() };
            sortedSegments.AddRange(v2.Select(x => segments[x]));
            sortedSegments.Add(segments.Last());
            sortedSegments.AddRange(v3.Select(x => segments[x]).Reverse());

            return new PathSegmentCollection(sortedSegments);
        }

        private static Geometry CombineGeometries(IReadOnlyList<Geometry> geometries)
        {
            Debug.Assert(geometries.Count > 0);
            if (geometries.Count == 1) return geometries.First();
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
            segments.Add(new ArcSegment
            {
                Point = CoordsToInsetUpperLeftVertical(coords),
                Size = new Size(TetraStickHalfThickness, TetraStickHalfThickness)
            });
        }

        private void AddEndCapBottom(PathSegmentCollection segments, Coords coords)
        {
            segments.Add(new ArcSegment
            {
                Point = CoordsToInsetLowerRightVertical(coords),
                Size = new Size(TetraStickHalfThickness, TetraStickHalfThickness)
            });
        }

        private void AddLineRightHorizontal(PathSegmentCollection segments, Coords coords1, Coords coords2)
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

        private void AddLineLeftHorizontal(PathSegmentCollection segments, Coords coords1, Coords coords2)
        {
            segments.Add(new LineSegment
            {
                Point = CoordsToInsetUpperLeftHorizontal(coords2)
            });

            segments.Add(new LineSegment
            {
                Point = CoordsToInsetLowerRightHorizontal(coords1)
            });
        }

        private void AddLineUpVertical(PathSegmentCollection segments, Coords coords1, Coords coords2)
        {
            segments.Add(new LineSegment
            {
                Point = CoordsToInsetUpperRightVertical(coords2)
            });

            segments.Add(new LineSegment
            {
                Point = CoordsToInsetLowerLeftVertical(coords1)
            });
        }

        private void AddLineDownVertical(PathSegmentCollection segments, Coords coords1, Coords coords2)
        {
            segments.Add(new LineSegment
            {
                Point = CoordsToInsetLowerLeftVertical(coords2)
            });

            segments.Add(new LineSegment
            {
                Point = CoordsToInsetUpperRightVertical(coords1)
            });
        }

        private void AddCornerRightThenUp(PathSegmentCollection segments, Coords coords)
        {
            segments.Add(new LineSegment
            {
                Point = CoordsToInsetLowerRightVertical(coords)
            });

            segments.Add(new LineSegment
            {
                Point = CoordsToInsetUpperRightHorizontal(coords)
            });
        }

        //private void AddCornerRightThenDown(PathSegmentCollection segments, Coords coords)
        //{
        //}

        //private void AddCornerLeftThenUp(PathSegmentCollection segments, Coords coords)
        //{
        //}

        //private void AddCornerLeftThenDown(PathSegmentCollection segments, Coords coords)
        //{
        //}

        //private void AddCornerUpThenLeft(PathSegmentCollection segments, Coords coords)
        //{
        //}

        //private void AddCornerUpThenRight(PathSegmentCollection segments, Coords coords)
        //{
        //}

        //private void AddCornerDownThenLeft(PathSegmentCollection segments, Coords coords)
        //{
        //}

        //private void AddCornerDownThenRight(PathSegmentCollection segments, Coords coords)
        //{
        //}

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

        private Point CoordsToInsetUpperLeftVertical(Coords coords)
        {
            return new Point(
                coords.X * _sw + GridLineHalfThickness - TetraStickHalfThickness,
                (5 - coords.Y) * _sh + GridLineHalfThickness + TetraStickInset);
        }

        private Point CoordsToInsetUpperRightVertical(Coords coords)
        {
            return new Point(
                coords.X * _sw + GridLineHalfThickness + TetraStickHalfThickness,
                (5 - coords.Y) * _sh + GridLineHalfThickness + TetraStickInset);
        }

        private Point CoordsToInsetLowerLeftVertical(Coords coords)
        {
            return new Point(
                coords.X * _sw + GridLineHalfThickness - TetraStickHalfThickness,
                (5 - coords.Y) * _sh + GridLineHalfThickness - TetraStickInset);
        }

        private Point CoordsToInsetLowerRightVertical(Coords coords)
        {
            return new Point(
                coords.X * _sw + GridLineHalfThickness + TetraStickHalfThickness,
                (5 - coords.Y) * _sh + GridLineHalfThickness - TetraStickInset);
        }

        public void CombinedGeometryExperiment()
        {
            var line1 = ImmutableList.Create(
                new Coords(0, 0),
                new Coords(1, 0),
                new Coords(1, 1));

            //var line2 = ImmutableList.Create(
            //    new Coords(0, 1),
            //    new Coords(1, 1),
            //    new Coords(2, 1),
            //    new Coords(3, 1));

            var tetraStick = new TetraStick("V", ImmutableList<Coords>.Empty, line1);
            //var tetraStick = new TetraStick("V", ImmutableList<Coords>.Empty, line1, line2);

            //var tetraStick = Model.TetraSticks.X;
            var location = new Coords(1, 1);
            var orientation = Orientation.North;
            var reflectionMode = ReflectionMode.Normal;

            var tempPlacedTetraStick = new PlacedTetraStick(
                tetraStick,
                location,
                orientation,
                reflectionMode);
            DrawPlacedTetraStick2(tempPlacedTetraStick);
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
