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
    class DrawingSurface : Panel
    {
        class Barrel:
            PipelineElement
        {
            private void Rotate90(object sender, RoutedEventArgs e)
            {
                MessageBox.Show("HELLO");
            }

            protected override void Draw()
            {
                using (DrawingContext dc = this.RenderOpen())
                {
                    var rect = new Rect(-width / 2, -height / 2, width, height);
                    var overlayImage = new BitmapImage(new Uri(@"D:\Downloads\Redactor\Redactor\Redactor\barrel.png"));
                    dc.DrawImage(overlayImage, rect);
                    if (selected)
                    {
                        var pen = new Pen(Brushes.LightSkyBlue, 1);
                        pen.DashStyle = DashStyles.Dash;
                        dc.DrawRectangle(Brushes.Transparent, pen, rect);
                    }
                }
            }

            public override ContextMenu GetMenu()
            {
                var rotate90 = new MenuItem();
                rotate90.Header = "Повернуть на 90 градусов";
                rotate90.Click += new RoutedEventHandler(Rotate90);
                var menu = new ContextMenu();
                menu.Items.Add(rotate90);
                return menu;
            }

            public Barrel(Point center):
                base()
            {
                _width = 100;
                _height = 100;
                this.center = center;
                Draw();
            }
        }

        private VisualCollection visuals;
        private List<DrawingVisual> selectedElements = new List<DrawingVisual>();
        private DrawingVisual selector = new DrawingVisual();
        private Point dragStartPoint;
        private Point dragPrevPoint;
        private bool dragEnable;
        private int state;
        private enum actions
        {
            MOUSE_LEFTBUTTON_DOWN,
            MOUSE_LEFTBUTTON_UP,
            MOUSE_RIGHTBUTTON_DOWN,
            MOUSE_RIGHTBUTTON_UP,
            MOUSE_MOVE,
            DEFAULT_ACTION,
            DRAGGING_ELEMENT,
            DRAGGING_SURFACE,
            MULTISELECTING
        }
        private List<DrawingVisual> hittedElements = new List<DrawingVisual>();

        public DrawingSurface()
        {
            visuals = new VisualCollection(this);
            state = (int)actions.DEFAULT_ACTION;
            selector.Opacity = 0.5;

            this.Loaded += new RoutedEventHandler(DrawingSurface_Loaded);
            this.MouseMove += new MouseEventHandler(DrawingSurface_MouseMove);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(DrawingSurface_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(DrawingSurface_MouseLeftButtonUp);
            this.MouseRightButtonDown += new MouseButtonEventHandler(DrawingSurface_MouseRightButtonDown);
            this.MouseRightButtonUp += new MouseButtonEventHandler(DrawingSurface_MouseRightButtonUp);
            //this.MouseWheel += new MouseWheelEventHandler(DrawingSurface_MouseWheel);
        }

        private void DrawingSurface_Loaded(object sender, RoutedEventArgs e)
        {
            var el = new Barrel(new Point(100, 100));
            AddElement(el);
            el = new Barrel(new Point(210, 210));
            AddElement(el);
        }

        private void DrawingSurface_KeyUp(object sender, KeyEventArgs e)
        {
            
        }

        private void DrawingSurface_MouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(this);

            switch ((actions)state)
            {
                case actions.MOUSE_LEFTBUTTON_DOWN:
                    GetElementsInPoint(position);
                    if (hittedElements.Count > 0)
                    {
                        if (!(hittedElements[0] as PipelineElement).selected)
                        {
                            ClearSelection();
                            SelectElement(hittedElements[0]);
                        }
                        state = (int)actions.DRAGGING_ELEMENT;
                    }
                    else
                    {
                        state = (int)actions.DRAGGING_SURFACE;
                    }
                    foreach (var element in selectedElements)
                    {
                        ElementToTop(element);
                    }
                    dragStartPoint = dragPrevPoint = position;
                    break;
                case actions.MOUSE_RIGHTBUTTON_DOWN:
                    dragStartPoint = position;
                    visuals.Add(selector);
                    state = (int)actions.MULTISELECTING;
                    break;
                case actions.MULTISELECTING:
                    DrawingSurface_MultiSelecting(position);
                    break;
                case actions.DRAGGING_ELEMENT:
                    DrawingSurface_DragElement(position);
                    break;
            }
        }

        private void DrawingSurface_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(this);

            state = (int)actions.MOUSE_LEFTBUTTON_DOWN;
        }
        private void DrawingSurface_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(this);

            switch ((actions)state)
            {
                case actions.MOUSE_LEFTBUTTON_DOWN:
                    DrawingSurface_Mouse_LeftButton_Click(position);
                    break;
                case actions.DRAGGING_ELEMENT:
                    DrawingSurface_StopDragElement(position);
                    break;
            }

            state = (int)actions.MOUSE_LEFTBUTTON_UP;
        }

        private void DrawingSurface_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(this);
            state = (int)actions.MOUSE_RIGHTBUTTON_DOWN;
        }
        private void DrawingSurface_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(this);
            switch ((actions)state)
            {
                case actions.MULTISELECTING:
                    DrawingSurface_StopMultiselecting(position);
                    break;

                case actions.MOUSE_RIGHTBUTTON_DOWN:
                    DrawingSurface_OpenContextMenu(position);
                    break;
            }
            state = (int)actions.MOUSE_RIGHTBUTTON_UP;
        }


        private void DrawingSurface_Mouse_LeftButton_Click(Point point)
        {
            GetElementsInPoint(point);

            ClearSelection();

            if (hittedElements.Count > 0)
            {
                SelectElement(hittedElements[0]);
            }
        }

        private void DrawingSurface_DragElement(Point point)
        {
            foreach(var visual in selectedElements){
                var element = visual as PipelineElement;
                element.AddCenter(point.X - dragPrevPoint.X, point.Y - dragPrevPoint.Y);
            }
            dragEnable = true;
            foreach (var visual in selectedElements)
            {
                var element = visual as PipelineElement;
                GetElementsInArea(element.GetRect());
                if (hittedElements.Count > 1)
                {
                    element.error = true;
                    dragEnable = false;
                }
                else
                {
                    element.error = false;
                }
            }
            dragPrevPoint = point;
        }
        private void DrawingSurface_StopDragElement(Point point)
        {
            if (!dragEnable)
            {
                foreach (var visual in selectedElements)
                {
                    var element = visual as PipelineElement;
                    element.AddCenter(dragStartPoint.X - point.X, dragStartPoint.Y - point.Y);
                    element.error = false;
                }
            }
        }
       
        private void DrawingSurface_MultiSelecting(Point point)
        {
            using (DrawingContext dc = selector.RenderOpen())
            {
                dc.DrawRectangle(Brushes.Blue, new Pen(Brushes.BlueViolet, 1), new Rect(dragStartPoint, point));
            }
        }
        private void DrawingSurface_StopMultiselecting(Point point)
        {
            ClearSelection();
            visuals.Remove(selector);
            using (DrawingContext dc = selector.RenderOpen())
            {
            }
            GetElementsInArea(new Rect(dragStartPoint, point));
            foreach (var i in hittedElements)
            {
                SelectElement(i);
            }
        }

        private void DrawingSurface_OpenContextMenu(Point point)
        {
            GetElementsInPoint(point);
            if (hittedElements.Count > 0)
            {
                (hittedElements[0] as PipelineElement).GetMenu().IsOpen = true;
            }
        }

        private void SelectElement(DrawingVisual visual)
        {
            (visual as PipelineElement).selected = true;
            selectedElements.Add(visual);
        }

        private void ClearSelection()
        {
            for (var i = 0; i < selectedElements.Count; i++)
            {
                (selectedElements[i] as PipelineElement).selected = false;
            }
            selectedElements.Clear();
        }

        private void GetElementsInPoint(Point point)
        {
            hittedElements.Clear();
            VisualTreeHelper.HitTest(
                this, 
                null,
                new HitTestResultCallback(MainHitTestResult),
                new PointHitTestParameters(point)
            );
        }
        private void GetElementsInArea(Rect area)
        {
            hittedElements.Clear();
            VisualTreeHelper.HitTest(
                this,
                null,
                new HitTestResultCallback(MainHitTestResult),
                new GeometryHitTestParameters(new RectangleGeometry(area))
            );
        }
        public HitTestResultBehavior MainHitTestResult(HitTestResult result)
        {
            if ((result.VisualHit as DrawingVisual) != null)
            {
                hittedElements.Add(result.VisualHit as DrawingVisual);
            }
            return HitTestResultBehavior.Continue;
        }

        private void ElementToTop(DrawingVisual el)
        {
            visuals.Remove(el);
            visuals.Add(el);
        }

        private void AddElement(DrawingVisual el)
        {
            visuals.Add(el);
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
