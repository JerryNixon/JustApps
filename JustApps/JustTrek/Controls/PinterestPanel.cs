using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JustTrek.Controls
{
    public class PinterestPanel : Panel
    {
        public double ColumnMinWidth
        {
            get { return (double)GetValue(ColumnMinWidthProperty); }
            set { SetValue(ColumnMinWidthProperty, value); }
        }
        public static readonly DependencyProperty ColumnMinWidthProperty =
            DependencyProperty.Register(nameof(ColumnMinWidth), typeof(double), typeof(PinterestPanel), new PropertyMetadata(300d));

        protected override Size MeasureOverride(Size size)
        {
            var info = DetermineColumn(size);
            foreach (var child in Children)
            {
                var targetIndex = info.ShortestIndex();
                var potentialSize = new Size(info.Width, size.Height);
                child.Measure(potentialSize);
                var desiredSize = child.DesiredSize;
                info.Columns[targetIndex] += desiredSize.Height;
            }
            return new Size(size.Width, info.Columns.Max());
        }

        protected override Size ArrangeOverride(Size size)
        {
            var info = DetermineColumn(size);
            foreach (var child in Children)
            {
                var targetIndex = info.ShortestIndex();
                var elementSize = child.DesiredSize;
                var bounds = new Rect(info.Width * targetIndex, info.Columns[targetIndex], info.Width, elementSize.Height);
                child.Arrange(bounds);
                info.Columns[targetIndex] += elementSize.Height;
            }
            return new Size(size.Width, info.Columns.Max());
        }

        public class ColumnInfo
        {
            public double[] Columns { get; set; }
            public double Width { get; set; }
            public int ShortestIndex() => Array.IndexOf(Columns, Columns.Min());
        }

        private ColumnInfo DetermineColumn(Size size)
        {
            var count = (int)(size.Width / ColumnMinWidth);
            var info = new ColumnInfo
            {
                Width = ColumnMinWidth,
                Columns = new double[count],
            };
            var mod = size.Width % ColumnMinWidth;
            if (mod != 0)
            {
                info.Width += ColumnMinWidth * mod;
            }
            return info;
        }
    }

}
