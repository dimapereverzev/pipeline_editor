using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Redactor
{
    class pipelineObject: DrawingVisual
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
        
    }
}
