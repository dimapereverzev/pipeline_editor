using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Redactor
{
    class DrawingSurface : Canvas
    {
        class RedactingPipelineObjects : pipelineObject
        {
            public RedactingPipelineObjects(pipelineObject checkedObj):
                base(checkedObj.center, checkedObj.width, checkedObj.height)
            {
                using (DrawingContext dc = this.RenderOpen())
                {
                    var pen = new Pen(Brushes.Red, 2);
                    dc.DrawRectangle((Brush)null, pen, checkedObj.getRect());
                }

            }
        };

        class Barrel : pipelineObject
        {
            public Barrel():
                base(new Point(60, 60), 40, 40)
            {
                using (DrawingContext dc = this.RenderOpen())
                {
                    var overlayImage = new BitmapImage(new Uri(@"C:\projects\c#\Redactor\Redactor\barrel.png"));
                    dc.DrawImage(overlayImage,
                           new Rect(-20, -20, 40, 40));
                }
            }
        };

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
            var obj = new Barrel();
            visuals.Add((DrawingVisual)obj);
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
                ((pipelineObject)currentFocusedVisual).center = position;
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
                ((pipelineObject)currentFocusedVisual).center = position;
                var obj = (pipelineObject)currentFocusedVisual;
                isDragVisual = false;
                /*if (!obj.check)
                {
                    obj.check = true;
                    visuals.Add((DrawingVisual)(new RedactingPipelineObjects(obj)));
                }*/
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
