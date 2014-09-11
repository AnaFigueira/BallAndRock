using System.ComponentModel;

namespace BallAndRock
{
    public class Ball : INotifyPropertyChanged
    {
        #region Properties & Constructors

        /// <summary>
        /// X Coordinate
        /// </summary>
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

        /// <summary>
        /// Y Coordinate
        /// </summary>
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

        /// <summary>
        /// Image size
        /// </summary>
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

        /// <summary>
        /// Ball speed
        /// </summary>
        public double _speed;

        public double Speed
        {
            get { return _speed; }
            set
            {
                this._speed = value;
                NotifyPropertyChanged("Speed");
            }
        }

        /// <summary>
        ///  Image number. Used for animating the image
        /// </summary>
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
        /// Ball image file name
        /// </summary>
        private string _ballFile;

        public string BallFile
        {
            get { return _ballFile; }
            set
            {
                this._ballFile = value;
                NotifyPropertyChanged("BallFile");
            }
        }

        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="x">X Coordinate</param>
        /// <param name="y">Y Coordinate</param>
        /// <param name="size">Image Size</param>
        /// <param name="speed">Speed</param>
        /// <param name="fileName">Name of image file</param>
        public Ball(int x, int y, int size, int speed, string fileName)
        {
            this._x = x;
            this._y = y;
            this._size = size;
            this._speed = speed;
            this._ballFile = fileName;
            this._imagePos = 0;
        }

        #endregion Properties & Constructors

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion PropertyChanged
    }
}