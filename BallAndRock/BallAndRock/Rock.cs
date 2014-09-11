using System.ComponentModel;
using Windows.UI.Xaml.Media.Imaging;

namespace BallAndRock
{
    public class Rock : INotifyPropertyChanged
    {
        private int _rockNumber;
        public int RockNumber
        {
            get { return _rockNumber; }
            set
            {
                this._rockNumber = value;
                NotifyPropertyChanged("RockNumber");
            }
        }

        private int _x;
        public int X
        {
            get { return _x; }
            set
            {
                this._x = value;
                NotifyPropertyChanged("X");
            }
        }

        private int _y;

        public int Y
        {
            get { return _y; }
            set
            {
                this._y = value;
                NotifyPropertyChanged("Y");
            }
        }
        private int _size;

        public int Size
        {
            get { return _size; }
            set
            {
                this._size = value;
                NotifyPropertyChanged("Size");
            }
        }

        private int _speed;

        public int Speed
        {
            get { return _speed; }
            set
            {
                this._speed = value;
                NotifyPropertyChanged("Speed");
            }
        }

        private int _imagePos;

        public int ImagePos
        {
            get { return _imagePos; }
            set
            {
                this._imagePos = value;
                NotifyPropertyChanged("ImagePos");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string _color;
        public string Color
        {
            get { return _color; }
            set
            {
                this._color = value;
                NotifyPropertyChanged("Color");
            }
        }

        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="rockNumber">Number of the rock (id)</param>
        /// <param name="x">X position of the rock</param>
        /// <param name="y">Y position of the rock</param>
        /// <param name="size">Size of the rock image</param>
        /// <param name="speed">Initial speed of the rock</param>
        /// <param name="color">String to identify the color file name</param>
        public Rock(int rockNumber, int x, int y, int size, int speed, string color)
        {
            this._rockNumber = rockNumber;
            this._x = x;
            this._y = y;
            this._size = size;
            this._speed = speed;
            this._imagePos = 0;
            this._color = color;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}