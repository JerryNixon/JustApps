using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Utils;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace JustLines.Controls
{
    public class DotPanel : Panel
    {
        public int Rows { get; set; }
        public int Columns { get; set; }

        public DotPanel()
        {
            Loaded += DotPanel_Loaded;
            Margin = new Thickness(40);
        }

        private void DotPanel_Loaded(object sender, RoutedEventArgs e)
        {
            CreateItems();
        }

        private void CreateItems()
        {
            Children.Clear();
            foreach (var column in Enumerable.Range(0, Columns))
            {
                foreach (var row in Enumerable.Range(0, Rows))
                {
                    var bottomItem = new BottomItem
                    {
                        Row = row,
                        Column = column,
                    };
                    Children.Add(bottomItem);
                    var topItem = new TopItem
                    {
                        Row = row,
                        Column = column,
                    };
                    Children.Add(topItem);

                    bottomItem.PointerEntered += (s, e) =>
                    {
                        bottomItem.Background = Colors.Red.ToSolidColorBrush();
                    };
                    bottomItem.PointerExited += (s, e) =>
                    {
                        bottomItem.Background = null;
                    };
                    bottomItem.ManipulationStarted += (s, e) =>
                    {
                        // nothing
                    };
                    bottomItem.ManipulationDelta += (s, e) =>
                    {
                        topItem.Transform.TranslateX += e.Delta.Translation.X;
                        topItem.Transform.TranslateY += e.Delta.Translation.Y;
                    };
                    bottomItem.ManipulationCompleted += (s, e) =>
                    {
                        var sb = new Storyboard();
                        var ax = new DoubleAnimation { To = 0, Duration = TimeSpan.FromMilliseconds(300) };
                        Storyboard.SetTarget(ax, topItem.Transform);
                        Storyboard.SetTargetProperty(ax, "TranslateX");
                        sb.Children.Add(ax);
                        var ay = new DoubleAnimation { To = 0, Duration = TimeSpan.FromMilliseconds(300) };
                        Storyboard.SetTarget(ay, topItem.Transform);
                        Storyboard.SetTargetProperty(ay, "TranslateY");
                        sb.Children.Add(ay);
                        sb.Begin();
                    };
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize) => availableSize;

        protected override Size ArrangeOverride(Size finalSize)
        {
            var columnWidth = finalSize.Width / (Columns - 1);
            var rowHeight = finalSize.Height / (Rows - 1);
            foreach (var item in Children.OfType<BottomItem>())
            {
                var left = columnWidth * item.Column - columnWidth * .5;
                var top = rowHeight * item.Row - rowHeight * .5;
                item.Arrange(new Rect(left, top, item.Width, item.Height));
            }
            foreach (var item in Children.OfType<TopItem>())
            {
                var left = columnWidth * item.Column - columnWidth * .5;
                var top = rowHeight * item.Row - rowHeight * .5;
                item.Arrange(new Rect(left, top, item.Width, item.Height));
            }
            return finalSize;
        }
    }

    public abstract class ItemBase : Grid
    {
        public int Row;
        public int Column;

        public ItemBase() { }
    }

    public class BottomItem : ItemBase
    {
        public BottomItem() : base()
        {
            Opacity = .5;
            Width = 200;
            Height = 200;
            Margin = new Thickness(12);
            ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            Children.Add(new Ellipse
            {
                Fill = Colors.Blue.ToSolidColorBrush(),
                Width = 75,
                Height = 75,
            });

            Children.Add(Left = BuildTriangle(new Point(0, .5), new Point(1, 0), new Point(1, 1), VerticalAlignment.Center, HorizontalAlignment.Left, Colors.Orange.ToSolidColorBrush()));
            Children.Add(Top = BuildTriangle(new Point(0, 1), new Point(1, 1), new Point(.5, 0), VerticalAlignment.Top, HorizontalAlignment.Center, Colors.Orange.ToSolidColorBrush()));
            Children.Add(Right = BuildTriangle(new Point(0, 0), new Point(1, .5), new Point(0, 1), VerticalAlignment.Center, HorizontalAlignment.Right, Colors.Orange.ToSolidColorBrush()));
            Children.Add(Bottom = BuildTriangle(new Point(.5, 1), new Point(0, 0), new Point(1, 0), VerticalAlignment.Bottom, HorizontalAlignment.Center, Colors.Orange.ToSolidColorBrush()));
        }

        public Path Left { get; set; }
        public Path Top { get; set; }
        public Path Right { get; set; }
        public Path Bottom { get; set; }

        public CompositeTransform Transform => RenderTransform as CompositeTransform;
        Path BuildTriangle(Point p1, Point p2, Point p3, VerticalAlignment vertical, HorizontalAlignment horizontal, SolidColorBrush color)
        {
            var figure = new PathFigure { IsFilled = true, IsClosed = true, StartPoint = p1 };
            figure.Segments.Add(new LineSegment { Point = figure.StartPoint });
            figure.Segments.Add(new LineSegment { Point = p2 });
            figure.Segments.Add(new LineSegment { Point = p3 });

            var figures = new PathFigureCollection();
            figures.Add(figure);

            var path = new Path
            {
                VerticalAlignment = vertical,
                HorizontalAlignment = horizontal,
                Stretch = Stretch.Fill,
                Fill = color,
                Width = 25,
                Height = 25,
                Data = new PathGeometry { FillRule = FillRule.Nonzero, Figures = figures }
            };
            return path;
        }
    }

    public class TopItem : ItemBase
    {
        public TopItem() : base()
        {
            Width = 150;
            Height = 150;
            Opacity = .5;
            IsHitTestVisible = false;
            RenderTransform = new CompositeTransform();
            Children.Add(new Ellipse
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Fill = Colors.Green.ToSolidColorBrush(),
                Width = 50,
                Height = 50,
            });
        }

        public CompositeTransform Transform => RenderTransform as CompositeTransform;
    }

    //public class DotPanelx : Grid
    //{
    //    public int Rows { get; set; }
    //    public int Columns { get; set; }

    //    public DotPanelx()
    //    {
    //        Loaded += DotPanel_Loaded;
    //        Margin = new Thickness(40);
    //    }

    //    private void DotPanel_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    //    {
    //        for (int i = 1; i < Rows; i++)
    //        {
    //            RowDefinitions.Add(new RowDefinition { Height = new Windows.UI.Xaml.GridLength(1, GridUnitType.Star) });
    //        }
    //        RowDefinitions.Last().Height = new GridLength(0);

    //        for (int i = 1; i < Columns; i++)
    //        {
    //            ColumnDefinitions.Add(new ColumnDefinition { Width = new Windows.UI.Xaml.GridLength(1, GridUnitType.Star) });
    //        }
    //        ColumnDefinitions.Last().Width = new GridLength(0);

    //        foreach (var column in ColumnDefinitions.Select((x, i) => i))
    //        {
    //            foreach (var row in RowDefinitions.Select((x, i) => i))
    //            {
    //                var rectangle = new Rectangle { Fill = new SolidColorBrush(Colors.Gainsboro), Margin = new Thickness(4) };
    //                SetColumn(rectangle, column);
    //                SetRow(rectangle, row);
    //                Children.Add(rectangle);

    //                CreateEllipse(column, row);
    //            }
    //        }
    //    }

    //    private void CreateEllipse(int column, int row)
    //    {
    //        var firstColumn = column == 0;
    //        var lastColumn = column + 1 == ColumnDefinitions.Count();
    //        var firstRow = row == 0;
    //        var lastRow = row + 1 == RowDefinitions.Count();

    //        var lineThickness = 6;
    //        var thumbSize = new Size(50, 50);
    //        var gridSize = new Size(ColumnDefinitions.First().ActualWidth, RowDefinitions.First().ActualHeight);
    //        var anchorSize = new Size(gridSize.Width * 2, gridSize.Height * 2);
    //        var boundarySize = new Size(gridSize.Width, gridSize.Height);

    //        var anchor = new Ellipse
    //        {
    //            Height = anchorSize.Height,
    //            Width = anchorSize.Width,
    //            Fill = new SolidColorBrush(Colors.Green),
    //            VerticalAlignment = VerticalAlignment.Top,
    //            HorizontalAlignment = HorizontalAlignment.Left,
    //            Opacity = .25,
    //            IsHitTestVisible = false,
    //            RenderTransform = new CompositeTransform
    //            {
    //                TranslateX = -gridSize.Width * .5 + thumbSize.Width * .5,
    //                TranslateY = 0,
    //            },
    //        };
    //        SetColumn(anchor, column);
    //        SetRow(anchor, row);

    //        var ellipse = new Ellipse
    //        {
    //            Height = thumbSize.Height,
    //            Width = thumbSize.Width,
    //            Fill = new SolidColorBrush(Colors.Green),
    //            VerticalAlignment = VerticalAlignment.Top,
    //            HorizontalAlignment = HorizontalAlignment.Left,
    //            Opacity = .5,
    //            RenderTransform = new CompositeTransform
    //            {
    //                TranslateX = -thumbSize.Width * .5,
    //                TranslateY = -thumbSize.Height * .5,
    //            },
    //        };
    //        SetColumn(ellipse, column);
    //        SetRow(ellipse, row);

    //        var line = new Line
    //        {
    //            X1 = 0,
    //            Y1 = 0,
    //            X2 = 100,
    //            Y2 = 100,
    //            Stroke = new SolidColorBrush(Colors.Red),
    //            StrokeThickness = lineThickness,
    //            VerticalAlignment = VerticalAlignment.Top,
    //            HorizontalAlignment = HorizontalAlignment.Left,
    //            IsHitTestVisible = false,
    //            RenderTransform = new CompositeTransform
    //            {
    //                TranslateX = 0,
    //                TranslateY = 0,
    //            },
    //        };
    //        SetColumn(line, column);
    //        SetRow(line, row);

    //        Children.Add(anchor);
    //        Children.Add(line);
    //        Children.Add(ellipse);

    //        var revert = new Storyboard();

    //        var aex = new DoubleAnimation { To = 0, Duration = TimeSpan.FromMilliseconds(300) };
    //        Storyboard.SetTargetProperty(aex, "TranslateX");
    //        Storyboard.SetTarget(aex, ellipse.RenderTransform as CompositeTransform);
    //        revert.Children.Add(aex);

    //        var aey = new DoubleAnimation { To = 0, Duration = TimeSpan.FromMilliseconds(300) };
    //        Storyboard.SetTargetProperty(aey, "TranslateY");
    //        Storyboard.SetTarget(aey, ellipse.RenderTransform as CompositeTransform);
    //        revert.Children.Add(aey);

    //        var alx = new DoubleAnimation { To = 0, Duration = TimeSpan.FromMilliseconds(300), EnableDependentAnimation = true };
    //        Storyboard.SetTargetProperty(alx, "X2");
    //        Storyboard.SetTarget(alx, line);
    //        revert.Children.Add(alx);

    //        var aly = new DoubleAnimation { To = 0, Duration = TimeSpan.FromMilliseconds(300), EnableDependentAnimation = true };
    //        Storyboard.SetTargetProperty(aly, "Y2");
    //        Storyboard.SetTarget(aly, line);
    //        revert.Children.Add(aly);

    //        ellipse.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
    //        ellipse.ManipulationStarted += (s, e) =>
    //        {
    //            ellipse.Tag = e.Position;
    //        };
    //        ellipse.ManipulationCompleted += (s, e) =>
    //        {
    //            revert.Begin();
    //        };
    //        ellipse.ManipulationDelta += (s, e) =>
    //        {
    //            var transform = ellipse.RenderTransform as CompositeTransform;
    //            var point = e.Cumulative.Translation;

    //            double normx = (point.X - 0) / boundarySize.Width - 0.5;
    //            double normy = (point.Y - 0) / boundarySize.Height - 0.5;
    //            var contains = (normx * normx + normy * normy) < 0.25;

    //            if (true)
    //            {
    //                if (firstRow && e.Cumulative.Translation.Y > 0)
    //                {
    //                    // do nothing
    //                }
    //                else if (lastRow)
    //                {
    //                }
    //                else
    //                {
    //                    line.X2 = e.Cumulative.Translation.X;
    //                    transform.TranslateX += e.Delta.Translation.X;
    //                }
    //                line.Y2 = e.Cumulative.Translation.Y;
    //                transform.TranslateY += e.Delta.Translation.Y;
    //            }
    //        };
    //    }
    //}
}
