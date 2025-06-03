using Avalonia.Media;
using System;

namespace Squares_Avalonia.ViewModels
{
    public class SquaresField : ViewModelBase
    {
        private bool _isDrawn;
        private bool _isEnabled;
        private int _width;
        private int _height;

        public bool IsDrawn
        {
            get { return _isDrawn; }
            set
            {
                if (_isDrawn != value)
                {
                    _isDrawn = value;
                    OnPropertyChanged(nameof(IsDrawn));
                }
            }
        }
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }
        public int Width
        {
            get { return _width; }
            set
            {
                if (value != _width)
                {
                    _width = value;
                    OnPropertyChanged(nameof(Width));
                }
            }
        }
        public int Height
        {
            get { return _height; }
            set
            {
                if (value != _height)
                {
                    _height = value;
                    OnPropertyChanged(nameof(Height));
                }
            }
        }
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsVertical { get; set; }
        public int Top { get; set; }
        public int Left { get; set; }
        public Tuple<int, int, bool> XYV { get { return new Tuple<int, int, bool>(X, Y, IsVertical); } }
        public DelegateCommand? DrawCommand { get; set; }
    }
}
