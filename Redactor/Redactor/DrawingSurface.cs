using System;
using System.Collections.Generic;
using System.Globalization;
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
        /*class RedactingPipelineObjects : pipelineObject
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
        };*/

        /*class Barrel : pipelineObject
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
        };*/

        class Barrel : PipelineObject
        {
            public Barrel(Point spaceLeftTopPoint, double spaceWidth, double spaceHeight, double angle)
            {
                _leftTopPoint = spaceLeftTopPoint;
                _width = spaceWidth;
                _height = spaceHeight;
                _angle = angle;
                draw();
            }

            protected override void draw()
            {
                using (DrawingContext dc = this.RenderOpen())
                {
                    var overlayImage = new BitmapImage(new Uri(@"C:\projects\c#\Redactor\Redactor\barrel.png"));
                    dc.DrawImage(overlayImage, new Rect(_leftTopPoint.X, _leftTopPoint.Y, _width, _height));
                }
            }
        }

        class Position : DrawingVisual
        {
            private Point leftTopPoint;

            public Position()
            {
                draw(new Point(0,0), 1);
            }

            public void draw(Point point, double zoom)
            {
                using (DrawingContext dc = this.RenderOpen())
                {
                    dc.DrawText(
                        new FormattedText(
                            "["+point.X.ToString() + ", " + point.Y.ToString()+"], X"+zoom.ToString(),
                            CultureInfo.GetCultureInfo("en-us"),
                            FlowDirection.LeftToRight,
                            new Typeface("Verdana"),
                            14, 
                            System.Windows.Media.Brushes.Black
                        ), 
                        new Point(0, 0));
                }
            }
        }

        VisualCollection visuals;
        DrawingVisual currentFocusedVisual;

        VisualCollection currentCheckedVisual;

        bool isDragVisual;
        bool isDragSpace;
        Point dragStart;

        Point leftTopSpacePoint;
        double zoom;

        public DrawingSurface()
        {
            currentFocusedVisual = null;
            currentCheckedVisual = null;
            isDragVisual = false;
            isDragSpace = false;
            visuals = new VisualCollection(this);
            leftTopSpacePoint = new Point(0, 0);
            zoom = 1;

            this.Loaded += new RoutedEventHandler(DrawingSurface_Loaded);
            this.MouseMove += new MouseEventHandler(DrawingSurface_MouseMove);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(DrawingSurface_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(DrawingSurface_MouseLeftButtonUp);
            this.MouseWheel += new MouseWheelEventHandler(DrawingSurface_MouseWheel);
        }

        private void DrawingSurface_Loaded(object sender, RoutedEventArgs e)
        {
            var obj = new Barrel(new Point(100,100), 50, 70, 0);
            visuals.Add((DrawingVisual)obj);
            visuals.Add((DrawingVisual)(new Position()));
        }

        private void DrawingSurface_MouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(this);
            if (isDragVisual)
            {
                ((PipelineObject)currentFocusedVisual).setLeftTopPoint(new Point(leftTopSpacePoint.X + position.X, leftTopSpacePoint.Y + position.Y));
            }
            else if (isDragSpace)
            {
                leftTopSpacePoint.X += (dragStart.X - position.X );
                leftTopSpacePoint.Y += (dragStart.Y - position.Y);
                ((Position)visuals[visuals.Count - 1]).draw(leftTopSpacePoint, zoom);
                for (var i = 0; i < visuals.Count-1; i++)
                {
                    ((PipelineObject)visuals[i]).addTranslation(position.X - dragStart.X, position.Y - dragStart.Y);
                }
                dragStart = position;
            }
            else
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
        }

        private void DrawingSurface_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(this);
            dragStart = position;
            if (currentFocusedVisual != null)
            {
                isDragVisual = true;
            }
            else
            {
                isDragSpace = true;
            }
        }

        private void DrawingSurface_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(this);
            if (isDragVisual)
            {
                ((PipelineObject)currentFocusedVisual).setLeftTopPoint(new Point(leftTopSpacePoint.X + position.X, leftTopSpacePoint.Y + position.Y));
                var obj = (PipelineObject)currentFocusedVisual;
                isDragVisual = false;
                /*if (!obj.check)
                {
                    obj.check = true;
                    visuals.Add((DrawingVisual)(new RedactingPipelineObjects(obj)));
                }*/
            }
            else
            {
                isDragSpace = false;
            }
        }

        private void DrawingSurface_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            zoom += (double)e.Delta / 1000.0;
            if (zoom < 0.1)
            {
                zoom = 0.1;
            }
            ((Position)visuals[visuals.Count - 1]).draw(leftTopSpacePoint, zoom);
            for (var i = 0; i < visuals.Count - 1; i++)
            {
                ((PipelineObject)visuals[i]).setZoom(zoom, e.GetPosition(this));
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
