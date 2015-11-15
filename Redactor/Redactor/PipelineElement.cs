using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Redactor
{
    abstract class PipelineElement :
            DrawingVisual
    {
        protected double _width;
        protected double _height;
        private Point _centerPoint;
        private bool _focused = false;
        private bool _selected = false;
        private bool _error = false;
        private TranslateTransform _selfTranslate;

        public double width
        {
            get
            {
                return _width;
            }
        }
        public double height
        {
            get
            {
                return _height;
            }
        }
        public Point center
        {
            get
            {
                return _centerPoint;
            }
            set
            {
                _centerPoint = value;
                _selfTranslate.X = value.X;
                _selfTranslate.Y = value.Y;
            }
        }
        public void AddCenter(double addX, double addY)
        {
            _centerPoint.X += addX;
            _centerPoint.Y += addY;
            _selfTranslate.X += addX;
            _selfTranslate.Y += addY;
        }
        public bool focused
        {
            get
            {
                return _focused;
            }
            set
            {
                _focused = value;
                if (value)
                {
                    this.Opacity = 0.5;
                }
                else
                {
                    this.Opacity = 1;
                }
            }
        }
        public bool selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                Draw();
            }
        }
        public bool error
        {
            get
            {
                return error;
            }
            set
            {
                _error = value;
                if (value)
                {
                    this.Opacity = 0.7;
                }
                else
                {
                    this.Opacity = 1;
                }
            }
        }
        public Rect GetRect()
        {
            return new Rect(new Point(_centerPoint.X - _width / 2, _centerPoint.Y - _height / 2), new Size(_width, _height));
        }
        abstract public ContextMenu GetMenu();

        abstract protected void Draw();
        
        public PipelineElement()
        {
            this._selfTranslate = new TranslateTransform(0, 0);
            
            var transforms = new TransformGroup();
            transforms.Children.Add(this._selfTranslate);
            this.Transform = transforms;
            /*
            this.width = _width;
            this.height = _height;
            this.center = _centerPoint;
            this._angle = 0;
            this.AddRotation(_angle);

            var transforms = new TransformGroup();
            transforms.Children.Add(this._selfScale);
            transforms.Children.Add(this._selfRotate);
            transforms.Children.Add(this._selfTranslate);

            this.Transform = transforms;

            Draw();*/
        }
    }
}
