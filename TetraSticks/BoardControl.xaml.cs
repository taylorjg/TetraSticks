using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using TetraSticks.Model;

namespace TetraSticks
{
    public partial class BoardControl
    {
        private readonly Color _gridColour = Color.FromArgb(0x80, 0xCD, 0x85, 0x3F);
        private const int GridLineThickness = 4;
        private const int GridLineHalfThickness = GridLineThickness / 2;

        public BoardControl()
        {
            InitializeComponent();
        }

        public void DrawGrid()
        {
            DrawGridLines();
        }

        public void DrawRotatedTetraStick(RotatedTetraStick rotatedTetraStick)
        {
            foreach (var line in rotatedTetraStick.Lines)
                DrawLine(line.ToArray());
        }

        private void DrawLine(params Coords[] pts)
        {
            var aw = ActualWidth;
            var ah = ActualHeight;
            var sw = (aw - GridLineThickness) / 5;
            var sh = (ah - GridLineThickness) / 5;

            var transformedPts = pts.Select(pt => new Point(pt.X * sw + GridLineHalfThickness, (5 - pt.Y) * sh + GridLineHalfThickness)).ToList();
            var polyLineSegment = new PolyLineSegment(transformedPts, true);
            var pathFigure = new PathFigure { StartPoint = transformedPts.First() };
            pathFigure.Segments.Add(polyLineSegment);
            var pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);
            var path = new Path
            {
                Stroke = new SolidColorBrush(Colors.BlueViolet),
                StrokeThickness = GridLineThickness * 2,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeLineJoin = PenLineJoin.Round,
                Data = pathGeometry
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
                    Y2 = row * sh + GridLineHalfThickness
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
                    Y2 = ah
                };
                BoardCanvas.Children.Add(line);
            }
        }
    }
}
