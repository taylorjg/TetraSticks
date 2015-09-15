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
        private const int TetraStickThickness = 10;
        private const int TetraStickHalfThickness = TetraStickThickness / 2;
        private const int TetraStickInset = TetraStickThickness * 2;
        private const int TetraStickSmallCornerRadius = TetraStickInset - TetraStickHalfThickness;
        private const int TetraStickLargeCornerRadius = TetraStickInset + TetraStickHalfThickness;
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

        public void Reset()
        {
            RemoveChildrenWithTagType(TagType.TetraStick);
        }

        public void AddPlacedTetraStick(PlacedTetraStick placedTetraStick)
        {
            var colour = TetraStickColours.TetraStickToColour(placedTetraStick.TetraStick);
            var path = new Path
            {
                Data = CombineGeometries(placedTetraStick.Lines.Select(LineToGeometry).ToImmutableList()),
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(Colors.Black),
                Fill = new SolidColorBrush(colour),
                Tag = MakeTag(TagType.TetraStick, placedTetraStick)
            };
            BoardCanvas.Children.Add(path);
        }

        public void AddPlacedTetraSticks(IEnumerable<PlacedTetraStick> placedTetraSticks)
        {
            foreach (var placedTetraStick in placedTetraSticks)
                AddPlacedTetraStick(placedTetraStick);
        }

        private IImmutableList<FrameworkElement> GetTetraStickFrameworkElements()
        {
            return BoardCanvas.Children
                .OfType<FrameworkElement>()
                .Where(fe => GetTagType(fe) == TagType.TetraStick)
                .ToImmutableList();
        }

        public void RemovePlacedTetraStick(PlacedTetraStick placedTetraStick)
        {
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var frameworkElement in GetTetraStickFrameworkElements())
            {
                if (GetPlacedTetraStick(frameworkElement).TetraStick.Tag != placedTetraStick.TetraStick.Tag) continue;
                BoardCanvas.Children.Remove(frameworkElement);
                return;
            }
        }

        public bool IsPlacedTetraStickOnBoard(PlacedTetraStick placedTetraStick)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var frameworkElement in GetTetraStickFrameworkElements())
                if (GetPlacedTetraStick(frameworkElement).TetraStick.Tag == placedTetraStick.TetraStick.Tag)
                    return true;

            return false;
        }

        public bool IsPlacedTetraStickOnBoardCorrectly(PlacedTetraStick placedTetraStick)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var frameworkElement in GetTetraStickFrameworkElements())
            {
                var pst = GetPlacedTetraStick(frameworkElement);
                if (pst.TetraStick.Tag != placedTetraStick.TetraStick.Tag) continue;
                return (Equals(pst.Location, placedTetraStick.Location) &&
                        pst.Orientation == placedTetraStick.Orientation &&
                        pst.ReflectionMode == placedTetraStick.ReflectionMode);
            }

            return false;
        }

        public void RemovePlacedTetraSticksOtherThan(IEnumerable<PlacedTetraStick> placedTetraSticks)
        {
            var tagsToKeep = placedTetraSticks.Select(pts => pts.TetraStick.Tag).ToImmutableList();

            var elementsToRemove = GetTetraStickFrameworkElements()
                .Where(fe => !tagsToKeep.Contains(GetPlacedTetraStick(fe).TetraStick.Tag))
                .ToImmutableList();

            foreach (var element in elementsToRemove)
                BoardCanvas.Children.Remove(element);
        }

        private static TagType GetTagType(FrameworkElement frameworkElement)
        {
            return ((Tuple<TagType, PlacedTetraStick>)frameworkElement.Tag).Item1;
        }

        private static PlacedTetraStick GetPlacedTetraStick(FrameworkElement frameworkElement)
        {
            return ((Tuple<TagType, PlacedTetraStick>)frameworkElement.Tag).Item2;
        }

        private static Tuple<TagType, PlacedTetraStick> MakeTag(TagType tagType, PlacedTetraStick placedTetraStick)
        {
            return Tuple.Create(tagType, placedTetraStick);
        }

        private Geometry LineToGeometry(IImmutableList<Coords> line)
        {
            var lineSegments = LineToLineSegments(line);
            var combinedLineSegments = CombineConsecutiveLineSegments(lineSegments);
            var segments = SortPathSegmentCollection(BuildPathSegmentCollection(combinedLineSegments));
            return HandleClosedPaths(combinedLineSegments, segments);
        }

        private Geometry HandleClosedPaths(IImmutableList<Tuple<Coords, Coords, Direction>> combinedLineSegments, PathSegmentCollection segments)
        {
            var lastLineSegment = combinedLineSegments.Last();
            var firstLineSegment = combinedLineSegments.First();

            if (Equals(firstLineSegment.Item1, lastLineSegment.Item2))
            {
                // Only the 'O' tetra stick has a closed line.
                // The following code relies on the 'O' tetra stick
                // line going Up then Right then Down then Left.
                // Ideally, it should not rely on this but life is
                // too short!

                Debug.Assert(segments.Count%2 == 0);
                var halfLength = segments.Count/2;
                var innerSegments = segments.Skip(halfLength).ToList();
                var outerSegments = segments.Take(halfLength).ToList();

                var cornerSegments = new PathSegmentCollection();
                AddCornerDownThenRight(cornerSegments, lastLineSegment.Item2);

                outerSegments[0] = cornerSegments[0];
                innerSegments.RemoveAt(0);
                innerSegments.Add(cornerSegments[1]);

                var outerGeometry = new PathGeometry
                {
                    Figures = new PathFigureCollection
                    {
                        new PathFigure
                        {
                            StartPoint = CoordsToInsetLowerLeftVertical(lastLineSegment.Item2),
                            Segments = new PathSegmentCollection(outerSegments)
                        }
                    }
                };

                var innerGeometry = new PathGeometry
                {
                    Figures = new PathFigureCollection
                    {
                        new PathFigure
                        {
                            StartPoint = CoordsToInsetLowerRightVertical(lastLineSegment.Item2),
                            Segments = new PathSegmentCollection(innerSegments)
                        }
                    }
                };

                return new CombinedGeometry(GeometryCombineMode.Exclude, outerGeometry, innerGeometry);
            }

            return new PathGeometry
            {
                Figures = new PathFigureCollection
                    {
                        new PathFigure
                        {
                            StartPoint = GetStartingPoint(combinedLineSegments),
                            Segments = segments
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

            // TODO: Can this be done using Aggregate ?
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

            // TODO: Can this be done using Aggregate ?
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
                if (!isLast) AddAppropriateCorner(segments, lineSegment, lineSegments[i + 1].Item3);
                AddAppropriateFinishEndCap(segments, lineSegment, isLast);
            }
            return segments;
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

        // TODO: improve the signature of this method
        private void AddAppropriateCorner(PathSegmentCollection segments, Tuple<Coords, Coords, Direction> lineSegment, Direction nextDirection)
        {
            var currentDirection = lineSegment.Item3;
            switch (currentDirection)
            {
                case Direction.Left:
                    if (nextDirection == Direction.Up)
                        AddCornerLeftThenUp(segments, lineSegment.Item2);
                    else
                        AddCornerLeftThenDown(segments, lineSegment.Item2);
                    break;
                case Direction.Right:
                    if (nextDirection == Direction.Up)
                        AddCornerRightThenUp(segments, lineSegment.Item2);
                    else
                        AddCornerRightThenDown(segments, lineSegment.Item2);
                    break;
                case Direction.Up:
                    if (nextDirection == Direction.Left)
                        AddCornerUpThenLeft(segments, lineSegment.Item2);
                    else
                        AddCornerUpThenRight(segments, lineSegment.Item2);
                    break;
                case Direction.Down:
                    if (nextDirection == Direction.Left)
                        AddCornerDownThenLeft(segments, lineSegment.Item2);
                    else
                        AddCornerDownThenRight(segments, lineSegment.Item2);
                    break;
                default:
                    throw new InvalidOperationException("...");
            }
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

        private void AddCornerLeftThenUp(PathSegmentCollection segments, Coords coords)
        {
            segments.Add(new ArcSegment
            {
                Point = CoordsToInsetLowerRightVertical(coords),
                Size = new Size(TetraStickSmallCornerRadius, TetraStickSmallCornerRadius),
                SweepDirection = SweepDirection.Clockwise
            });

            segments.Add(new ArcSegment
            {
                Point = CoordsToInsetLowerLeftHorizontal(coords),
                Size = new Size(TetraStickLargeCornerRadius, TetraStickLargeCornerRadius)
            });
        }

        private void AddCornerLeftThenDown(PathSegmentCollection segments, Coords coords)
        {
            segments.Add(new ArcSegment
            {
                Point = CoordsToInsetUpperLeftVertical(coords),
                Size = new Size(TetraStickLargeCornerRadius, TetraStickLargeCornerRadius)
            });

            segments.Add(new ArcSegment
            {
                Point = CoordsToInsetLowerLeftHorizontal(coords),
                Size = new Size(TetraStickSmallCornerRadius, TetraStickSmallCornerRadius),
                SweepDirection = SweepDirection.Clockwise
            });
        }

        private void AddCornerRightThenUp(PathSegmentCollection segments, Coords coords)
        {
            segments.Add(new ArcSegment
            {
                Point = CoordsToInsetLowerRightVertical(coords),
                Size = new Size(TetraStickLargeCornerRadius, TetraStickLargeCornerRadius)
            });

            segments.Add(new ArcSegment
            {
                Point = CoordsToInsetUpperRightHorizontal(coords),
                Size = new Size(TetraStickSmallCornerRadius, TetraStickSmallCornerRadius),
                SweepDirection = SweepDirection.Clockwise
            });
        }

        private void AddCornerRightThenDown(PathSegmentCollection segments, Coords coords)
        {
            segments.Add(new ArcSegment
            {
                Point = CoordsToInsetUpperLeftVertical(coords),
                Size = new Size(TetraStickSmallCornerRadius, TetraStickSmallCornerRadius),
                SweepDirection = SweepDirection.Clockwise
            });

            segments.Add(new ArcSegment
            {
                Point = CoordsToInsetUpperRightHorizontal(coords),
                Size = new Size(TetraStickLargeCornerRadius, TetraStickLargeCornerRadius)
            });
        }

        private void AddCornerUpThenLeft(PathSegmentCollection segments, Coords coords)
        {
            segments.Add(new ArcSegment
            {
                Point = CoordsToInsetUpperRightHorizontal(coords),
                Size = new Size(TetraStickLargeCornerRadius, TetraStickLargeCornerRadius)
            });

            segments.Add(new ArcSegment
            {
                Point = CoordsToInsetUpperLeftVertical(coords),
                Size = new Size(TetraStickSmallCornerRadius, TetraStickSmallCornerRadius),
                SweepDirection = SweepDirection.Clockwise
            });
        }

        private void AddCornerUpThenRight(PathSegmentCollection segments, Coords coords)
        {
            segments.Add(new ArcSegment
            {
                Point = CoordsToInsetLowerLeftHorizontal(coords),
                Size = new Size(TetraStickSmallCornerRadius, TetraStickSmallCornerRadius),
                SweepDirection = SweepDirection.Clockwise
            });

            segments.Add(new ArcSegment
            {
                Point = CoordsToInsetUpperLeftVertical(coords),
                Size = new Size(TetraStickLargeCornerRadius, TetraStickLargeCornerRadius)
            });
        }

        private void AddCornerDownThenLeft(PathSegmentCollection segments, Coords coords)
        {
            segments.Add(new ArcSegment
            {
                Point = CoordsToInsetUpperRightHorizontal(coords),
                Size = new Size(TetraStickSmallCornerRadius, TetraStickSmallCornerRadius),
                SweepDirection = SweepDirection.Clockwise
            });

            segments.Add(new ArcSegment
            {
                Point = CoordsToInsetLowerRightVertical(coords),
                Size = new Size(TetraStickLargeCornerRadius, TetraStickLargeCornerRadius)
            });
        }

        private void AddCornerDownThenRight(PathSegmentCollection segments, Coords coords)
        {
            segments.Add(new ArcSegment
            {
                Point = CoordsToInsetLowerLeftHorizontal(coords),
                Size = new Size(TetraStickLargeCornerRadius, TetraStickLargeCornerRadius)
            });

            segments.Add(new ArcSegment
            {
                Point = CoordsToInsetLowerRightVertical(coords),
                Size = new Size(TetraStickSmallCornerRadius, TetraStickSmallCornerRadius),
                SweepDirection = SweepDirection.Clockwise
            });
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
                    Tag = MakeTag(TagType.GridLine, null)
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
                    Tag = MakeTag(TagType.GridLine, null)
                };
                BoardCanvas.Children.Add(line);
            }
        }

        private void RemoveChildrenWithTagType(TagType tagType)
        {
            foreach (var fe in GetTetraStickFrameworkElements())
                BoardCanvas.Children.Remove(fe);
        }
    }
}
