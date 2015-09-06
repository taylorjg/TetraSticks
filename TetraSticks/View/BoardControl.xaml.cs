using System.Collections.Generic;
using System.Collections.Immutable;
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

        private enum TagType
        {
            GridLine,
            TetraStick
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

            var transformedPts = coords
                .Select(coord => new Point(
                    (coord.X)*sw + GridLineHalfThickness,
                    (5 - coord.Y)*sh + GridLineHalfThickness))
                .ToImmutableList();

            var polyLineSegment = new PolyLineSegment(transformedPts, true);
            var pathFigure = new PathFigure { StartPoint = transformedPts.First() };
            pathFigure.Segments.Add(polyLineSegment);
            var pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);
            var path = new Path
            {
                Stroke = new SolidColorBrush(colour),
                StrokeThickness = GridLineThickness * 2,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeLineJoin = PenLineJoin.Round,
                Data = pathGeometry,
                Tag = TagType.TetraStick
            };
            BoardCanvas.Children.Add(path);
        }

        private void DrawGridLines()
        {
            var aw = ActualWidth;
            var ah = ActualHeight;
            var sw = (aw - GridLineThickness) / 5;
            var sh = (ah - GridLineThickness) / 5;

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
