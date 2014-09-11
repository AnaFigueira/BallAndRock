using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace BallAndRock
{
    public class Ball : INotifyPropertyChanged
    {
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

        public Ball(int x, int y, int size, string color)
        {
            this._x = x;
            this._y = y;
            this._size = size;
            this._color = color;
            this._imagePos = 0;
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
