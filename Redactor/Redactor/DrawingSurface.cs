using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Redactor
{
    class DrawingSurface : Canvas
    {
        VisualCollection visuals;
        DrawingVisual currentFocusedVisual;
        DrawingVisual currentCheckedVisual;
        bool isDragVisual;
        Point dragStart;

        public DrawingSurface()
        {
            currentFocusedVisual = null;
            currentCheckedVisual = null;
            isDragVisual = false;
            visuals = new VisualCollection(this);

            this.Loaded += new RoutedEventHandler(DrawingSurface_Loaded);
            this.MouseMove += new MouseEventHandler(DrawingSurface_MouseMove);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(DrawingSurface_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(DrawingSurface_MouseLeftButtonUp);
        }

        void DrawingSurface_Loaded(object sender, RoutedEventArgs e)
        {
            int x = 0;

            for (int i = 0; i <= 2; i++)
            {
                DrawingVisual visual = new DrawingVisual();
                using (DrawingContext dc = visual.RenderOpen())
                {
                    dc.DrawRectangle(Brushes.Red, new Pen(Brushes.Black, 2),
                    new Rect(new Point(0 + x, 0), new Size(40, 40)));
                }
                visuals.Add(visual);
                x += 60;
            }
        }

        void DrawingSurface_MouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(this);
            if (!isDragVisual)
            {
                DrawingVisual visual = this.GetVisual(position);
                if (visual == null)
                {
                    if (currentFocusedVisual != null)
                    {
                        currentFocusedVisual.Opacity = 1;
                        currentFocusedVisual = null;
                    }
                }
                else
                {
                    if (currentFocusedVisual == null)
                    {
                        visual.Opacity = 0.5;
                        currentFocusedVisual = visual;
                    }
                    else if (currentFocusedVisual != visual)
                    {
                        currentFocusedVisual.Opacity = 1;
                        visual.Opacity = 0.5;
                        currentFocusedVisual = visual;
                    }
                }
            }
            else
            {
                using (DrawingContext dc = currentFocusedVisual.RenderOpen())
                {
                    dc.DrawRectangle(Brushes.Red, new Pen(Brushes.Black, 2),
                    new Rect(position, new Size(40, 40)));
                }
            }
        }

        void DrawingSurface_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(this);
            if (currentFocusedVisual != null)
            {
                isDragVisual = true;
                dragStart = position;
            }
        }

        void DrawingSurface_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(this);
            if (isDragVisual)
            {
                DrawingVisual visual = this.GetVisual(position);
                if (visual != null)
                {
                    using (DrawingContext dc = currentFocusedVisual.RenderOpen())
                    {
                        dc.DrawRectangle(Brushes.Red, new Pen(Brushes.Black, 2),
                        new Rect(dragStart, new Size(40, 40)));
                    }
                    currentFocusedVisual.Opacity = 1;
                    visual.Opacity = 0.5;
                    currentFocusedVisual = visual;

                }
                isDragVisual = false;
            }
        }

        public DrawingVisual GetVisual(Point point)
        {
            HitTestResult result = VisualTreeHelper.HitTest(this, point);
            return result.VisualHit as DrawingVisual;
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= visuals.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            return visuals[index];
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return visuals.Count;
            }
        }
    }
}
