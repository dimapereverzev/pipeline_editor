using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Redactor
{
    abstract class PipelineObject : DrawingVisual
    {
        protected Point _leftTopPoint;
        protected double _width;
        protected double _height;
        protected double _angle;
        
        private ScaleTransform _scaleTransform;
        private TranslateTransform _translateTransform;

        public PipelineObject()
        {
            var transformGroup = new TransformGroup();
            _scaleTransform = new ScaleTransform(1, 1, 0, 0);
            _translateTransform = new TranslateTransform(0, 0);
            transformGroup.Children.Add(_scaleTransform);
            transformGroup.Children.Add(_translateTransform);
            this.Transform = transformGroup;
        }

        abstract protected void draw();

        public void setSize(double width, double height)
        {
            _width = width;
            _height = height;
            draw();
        }

        public void setAngle(double angle)
        {
            _angle = angle;
            draw();
        }

        public void setLeftTopPoint(Point leftTopPoint)
        {
            _leftTopPoint = leftTopPoint;
            draw();
        }

        public void setZoom(double magnification, Point zoomCenter)
        {
            _scaleTransform.CenterX = zoomCenter.X;
            _scaleTransform.CenterY = zoomCenter.Y;
            _scaleTransform.ScaleX = magnification;
            _scaleTransform.ScaleY = magnification;
        }

        public void addTranslation(double translateX, double translateY)
        {
            _translateTransform.X += translateX;
            _translateTransform.Y += translateY;
        }
    }
    /*class pipelineObject: DrawingVisual
    {
        // Матрицы трансформации вращения, увеличения и переноса
        private RotateTransform _rotateTransform;
        private ScaleTransform _scaleTransform;
        private TranslateTransform _translateTransform;

        // Выбрана ли фигура
        private bool _check;

        //Высота и ширина фигуры
        private double _width;
        private double _height;

        public pipelineObject()
        {

        }
        // Конструктор
        public pipelineObject(Point _center, double _width, double _height) : 
            base()
        {
            this._width = _width;
            this._height = _height;

            _check = false;

            var transformGroup = new TransformGroup();
            _rotateTransform = new RotateTransform(0, _center.X, _center.Y);
            _scaleTransform = new ScaleTransform(1, 1, _center.X, _center.Y);
            _translateTransform = new TranslateTransform(_center.X, _center.Y);
            transformGroup.Children.Add(_rotateTransform);
            transformGroup.Children.Add(_scaleTransform);
            transformGroup.Children.Add(_rotateTransform);
            transformGroup.Children.Add(_translateTransform);
            this.Transform = transformGroup;
        }

        // Задание размера
        public double zoom
        {
            get
            {
                return _scaleTransform.ScaleX;
            }
            set
            {
                if (value > 0)
                {
                    _scaleTransform.ScaleX = value;
                    _scaleTransform.ScaleY = value;
                }
            }
        }

        // Задание размера
        public double rotation
        {
            get
            {
                return _rotateTransform.Angle;
            }
            set
            {
                _rotateTransform.Angle = value;
            }
        }

        // Задание центра
        public Point center
        {
            get
            {
                return new Point(_translateTransform.X, _translateTransform.Y);
            }
            set
            {
                _translateTransform.X = value.X;
                _translateTransform.Y = value.Y;
                _scaleTransform.CenterX = value.X;
                _scaleTransform.CenterY = value.Y;
                _rotateTransform.CenterX = value.X;
                _rotateTransform.CenterY = value.Y;
            }
        }

        public bool check
        {
            get
            {
                return _check;
            }
            set
            {
                _check = value;
            }
        }

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

        public Rect getRect()
        {
            return new Rect(new Point(-_width/2, -_height/2), new Size(_width, _height));
        }
        
    }*/
}
