using System.Windows;

namespace TetraSticks
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ContentRendered += (_, __) =>
            {
                BoardControl.DrawGrid();

                BoardControl.DrawSegmentHorizontal(new Point(3, 3));

                BoardControl.DrawSegmentVertical(new Point(4, 1));

                BoardControl.DrawLine(
                    new Point(0, 0),
                    new Point(1, 0),
                    new Point(1, 1),
                    new Point(2, 1));

                BoardControl.DrawLine(new Point(1, 4), new Point(1, 5));
                BoardControl.DrawLine(new Point(1, 4), new Point(1, 3));
                BoardControl.DrawLine(new Point(1, 4), new Point(0, 4));
                BoardControl.DrawLine(new Point(1, 4), new Point(2, 4));
            };
        }
    }
}
