using System.ComponentModel;

namespace BallAndRock
{
    public class Rock : INotifyPropertyChanged
    {
        #region Properties & constructors

        /// <summary>
        /// Rock number. Works as an id.
        /// </summary>
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

        /// <summary>
        /// X Coordinate
        /// </summary>
        private double _x;

        public double X
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
        private double _y;

        public double Y
        {
            get { return _y; }
            set
            {
                this._y = value;
                NotifyPropertyChanged("Y");
            }
        }

        /// <summary>
        /// Rock Size e.g., 50x50
        /// </summary>
        private double _size;

        public double Size
        {
            get { return _size; }
            set
            {
                this._size = value;
                NotifyPropertyChanged("Size");
            }
        }

        /// <summary>
        /// Rock Speed
        /// </summary>
        private double _speed;

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
        /// Image number. For each set of rocktype there are 10 images.
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
        /// Type of the rock. There are 6 different types
        /// </summary>
        private string _rockType;

        public string RockType
        {
            get { return _rockType; }
            set
            {
                this._rockType = value;
                NotifyPropertyChanged("RockType");
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
        /// <param name="color">String to identify the rocktype file name</param>
        public Rock(int rockNumber, double x, double y, double size, double speed, string rockType)
        {
            this._rockNumber = rockNumber;
            this._x = x;
            this._y = y;
            this._size = size;
            this._speed = speed;
            this._imagePos = 0;
            this._rockType = rockType;
        }

        #endregion Properties & constructors

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