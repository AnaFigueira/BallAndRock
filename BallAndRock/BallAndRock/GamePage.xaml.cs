using BallAndRock.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Sensors;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace BallAndRock
{
    public sealed partial class GamePage : Page
    {
        #region Properties and Variables

        #region Rock Properties

        private const int _maxSpeed = 15; // Max speed for rocks
        private const int _minSpeed = 1; // Min speed for rocks
        private const int _rockSize = 25; // 25x25 pixels
        private const int _maxRocks = 6; // Max number of rocks at the same time on screen
        private const double _rockInitPos = -50; // Positions rock outside of canvas
        private int _nRocks = 0; // Current number of rocks
        private int _totalRocks = 0; // Serves as a rock id
        private Dictionary<string, BitmapImage> _rockTypes = null; // Holds the bitmapImages for the rocks
        private Dictionary<Rock, Image> RockImages; // Holds the rocks and respective Images present in the Canvas
        private int _nImagesPerRock = 5; // Number of images per rock

        #endregion Rock Properties

        #region Ball Properties

        private const double ballSize = 60; // 60x60 pixels
        private double ballHeight; // Height at which the ball is placed
        private Ball ball; // Ball object
        private Image _imgBall; // Image that holds the ball
        private int _numberBallImages = 5; // Number of images used to animate the ball
        private List<BitmapImage> BallImages; // List of ball images

        #endregion Ball Properties

        #region Screen properties

        private double screenHeight = 0;
        private double screenWidth = 0;
        private double minWidth = 0;

        #endregion Screen properties

        #region Others

        private DispatcherTimer _rotateImagesTimer; // Timer to rotate the images
        private DispatcherTimer _rockReleaseTimer; // Timer to release rocks
        private DispatcherTimer _addRockTimer; // Time to add rock
        private int _score = 0; // Current user score
        private Random rand; // Pseudo-random source
        private bool _gameOver = false; // Game over flag
        private Accelerometer _accelerometer; // Accelerometer

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        #endregion Others

        #endregion Properties and Variables

        #region Game Methods

        /// <summary>
        /// Class constructor
        /// </summary>
        public GamePage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            rand = new Random((int)DateTime.Now.Ticks);
            HideStatusBar();

            LoadBallImages();
            LoadRockTypes();

            _accelerometer = Accelerometer.GetDefault();

            if (_accelerometer != null)
            {
                uint minReportInterval = _accelerometer.MinimumReportInterval;
                uint reportInterval = minReportInterval > 16 ? minReportInterval : 16;
                _accelerometer.ReportInterval = reportInterval;
                _accelerometer.ReadingChanged += _accelerometer_ReadingChanged;
            }

            _addRockTimer = new DispatcherTimer();
            _addRockTimer.Tick += _addRockTimer_Tick;
            _addRockTimer.Interval = new TimeSpan(0, 0, 0, 0, rand.Next(1, 10000));
            _addRockTimer.Start();

            _rockReleaseTimer = new DispatcherTimer();
            _rockReleaseTimer.Tick += _rockReleaseTimer_Tick;
            _rockReleaseTimer.Interval = new TimeSpan(0, 0, 0, 0, rand.Next(1, 1500));
            _rockReleaseTimer.Start();

            _rotateImagesTimer = new DispatcherTimer();
            _rotateImagesTimer.Tick += _rotateImagesTimer_Tick;
            _rotateImagesTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            _rotateImagesTimer.Start();

            CompositionTarget.Rendering += GameLoop;
        }

        /// <summary>
        /// Timer to release rocks.
        /// </summary>
        private void _addRockTimer_Tick(object sender, object e)
        {
            if (_nRocks < _maxRocks)
            {
                _nRocks++;
                _addRockTimer.Interval = new TimeSpan(0, 0, 0, 0, rand.Next(1, _nRocks * 10000));
            }
        }

        /// <summary>
        /// Handles the rotation of images in order to give some animation to the game.
        /// </summary>
        private void _rotateImagesTimer_Tick(object sender, object e)
        {
            ball.ImagePos++;
            if (ball.ImagePos == _numberBallImages)
                ball.ImagePos = 0;

            _imgBall.Source = BallImages[ball.ImagePos];

            for (int i = 0; i < RockImages.Count; i++)
            {
                Rock rock = RockImages.ElementAt(i).Key;
                rock.ImagePos++;

                if (rock.ImagePos == _nImagesPerRock)
                    rock.ImagePos = 0;
                string r = rock.RockType;
                r = r.Remove(r.Length - 1, 1);
                r += rock.ImagePos;
                RockImages.ElementAt(i).Key.ImagePos = rock.ImagePos;
                RockImages.ElementAt(i).Value.Source = _rockTypes[r];
            }
        }

        /// <summary>
        /// Randomly creates a new rock to be released in game
        /// </summary>
        private void _rockReleaseTimer_Tick(object sender, object e)
        {
            if (RockImages.Count < _nRocks)
            {
                AddRock();
            }
            _rockReleaseTimer.Interval = new TimeSpan(0, 0, 0, 0, rand.Next(1, 1000));
        }

        /// <summary>
        /// Loads the images for the ball
        /// </summary>
        private void LoadBallImages()
        {
            BallImages = new List<BitmapImage>();
            for (int i = 0; i < _numberBallImages; i++)
                BallImages.Add(new BitmapImage(new Uri("ms-appx://MyAssembly/Content/Sprites/" + i + ".png")));
        }

        /// <summary>
        /// Handles the accelerometer readings
        /// </summary>
        private async void _accelerometer_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (ball != null)
                {
                    AccelerometerReading reading = args.Reading;
                    double acel = reading.AccelerationX / 9.8 / 100;
                    double initSpeed = ball.Speed;
                    double initX = ball.X;
                    uint t = _accelerometer.ReportInterval;

                    ball.Speed = initSpeed + (acel * t);
                    ball.X = initX + (initSpeed * t) + ((0.5) * acel * (t * t));

                    if (ball.X < minWidth)
                    {
                        ball.X = minWidth;
                        ball.Speed = 0;
                    }
                    if (ball.X > screenWidth)
                    {
                        ball.X = screenWidth;
                        ball.Speed = 0;
                    }
                }
            });
        }

        private void uiCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            // Get screen width and height to be used
            screenWidth = Window.Current.Bounds.Width - 65;
            screenHeight = Window.Current.Bounds.Height;

            // Initialize ball to be in the center of the screen at the bottom
            AddBall();

            //Initialize the dictionary that will hold the rocks
            RockImages = new Dictionary<Rock, Image>();
        }

        /// <summary>
        /// Game Loop
        /// </summary>
        private void GameLoop(object sender, object e)
        {
            if (!_gameOver)
            {
                CheckForCollisions();

                UpdateBall();

                UpdateRocks();

                UpdateScore();
            }
        }

        /// <summary>
        /// Updates the score label.
        /// The number turns green if the previous highscore has been passed.
        /// </summary>
        private void UpdateScore()
        {
            // Update highscore if new score is higher
            if (_score > Int32.Parse(ApplicationData.Current.LocalSettings.Values["HighScore"].ToString()))
                uiTbScore.Foreground = new SolidColorBrush(Colors.Green);

            uiTbScore.Text = _score.ToString();
        }

        /// <summary>
        /// Checks if there are any collisions and if there are, the game ends
        /// </summary>
        private void CheckForCollisions()
        {
            for (int i = 0; i < RockImages.Count; i++)
            {
                Rock rock = RockImages.ElementAt(i).Key;
                if (Math.Abs(ball.X - rock.X) < (0.5 * ball.Size) + (0.5 * rock.Size) &&
                    Math.Abs(ball.Y - rock.Y) < (0.5 * ball.Size) + (0.5 * rock.Size))
                {
                    uiExplosionSound.Play();
                    _gameOver = true;
                    CompositionTarget.Rendering -= GameLoop;

                    // Update highscore if new score is higher
                    if (Int32.Parse(ApplicationData.Current.LocalSettings.Values["HighScore"].ToString()) < _score)
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values["highScore"] = _score.ToString();

                    Frame.Navigate(typeof(MessageInfoPage),
                        new MessageParameters("Game Over!",
                        "Play again?", true));
                }
            }
        }

        /// <summary>
        /// Adds ball to the game.
        /// </summary>
        private void AddBall()
        {
            ballHeight = screenHeight - ballSize;
            ball = new Ball(screenWidth / 2, ballHeight, ballSize, 0, "0.png");
            _imgBall = new Image();
            _imgBall.Width = 65;
            _imgBall.Height = 65;
            _imgBall.SetValue(Canvas.LeftProperty, ball.X);
            _imgBall.SetValue(Canvas.TopProperty, ball.Y);
            _imgBall.Source = BallImages[0];

            uiCanvas.Children.Add(_imgBall);
        }

        /// <summary>
        /// Updates ball location
        /// </summary>
        private void UpdateBall()
        {
            if (ball.X < minWidth)
                ball.X = minWidth;
            if (ball.X > screenWidth)
                ball.X = screenWidth;

            _imgBall.SetValue(Canvas.LeftProperty, ball.X);
            _imgBall.SetValue(Canvas.TopProperty, ball.Y);
        }

        /// <summary>
        /// Function that adds a new rock to the Canvas.
        /// The generated rock is put at a random start at the top of the canvas.
        /// The speed is random, as well as the image.
        /// </summary>
        private void AddRock()
        {
            _totalRocks++;
            RockImages.Add(new Rock(_totalRocks, (double)rand.Next((int)minWidth, (int)screenWidth), _rockInitPos, _rockSize, rand.Next((int)_minSpeed, (int)_maxSpeed), _rockTypes.ElementAt(rand.Next(0, _rockTypes.Count)).Key), new Image() { Name = _totalRocks.ToString() });

            Rock rock = RockImages.Keys.Where(i => i.RockNumber == _totalRocks).First();

            RockImages.Values.Where(i => i.Name == _totalRocks.ToString()).First().SetValue(Canvas.LeftProperty, rock.X);
            RockImages.Values.Where(i => i.Name == _totalRocks.ToString()).First().SetValue(Canvas.TopProperty, rock.Y);
            RockImages.Values.Where(i => i.Name == _totalRocks.ToString()).First().Source = _rockTypes[rock.RockType];

            uiCanvas.Children.Add(RockImages.Values.Where(i => i.Name == _totalRocks.ToString()).FirstOrDefault());
        }

        /// <summary>
        /// Updates the rocks location
        /// </summary>
        private void UpdateRocks()
        {
            List<Rock> toBeRemoved = new List<Rock>();
            for (int i = 0; i < RockImages.Count; i++)
            {
                Rock kRock = RockImages.ElementAt(i).Key;
                kRock.Y += kRock.Speed;

                // If rock passed the screen (reached the bottom), remove ball
                if (kRock.Y >= screenHeight)
                {
                    _score++;
                    uiSurvivedSound.Play();
                    uiTbScore.Text = _score.ToString();
                    IEnumerable<Image> images = uiCanvas.Children.OfType<Image>();
                    foreach (Image c in images)
                    {
                        if (c.Name == RockImages[kRock].Name)
                        {
                            uiCanvas.Children.Remove(c); // Removes ball from canvas
                            toBeRemoved.Add(kRock);
                        }
                    }
                }
                else
                {
                    RockImages[kRock].SetValue(Canvas.LeftProperty, kRock.X);
                    RockImages[kRock].SetValue(Canvas.TopProperty, kRock.Y);
                }

                RockImages.ElementAt(i).Key.ImagePos = kRock.ImagePos;
                RockImages.ElementAt(i).Key.X = kRock.X;
                RockImages.ElementAt(i).Key.Y = kRock.Y;
            }
            // Remove rocks from dictionary
            foreach (var r in toBeRemoved)
                RockImages.Remove(r);
        }

        /// <summary>
        /// Loads the rock images.
        /// </summary>
        private void LoadRockTypes()
        {
            _rockTypes = new Dictionary<string, BitmapImage>();

            for (int i = 0; i < _nImagesPerRock; i++)
            {
                _rockTypes.Add("a1000" + i, new BitmapImage(new Uri("ms-appx://MyAssembly/Content/Sprites/a1000" + i + ".png")));
                _rockTypes.Add("a3000" + i, new BitmapImage(new Uri("ms-appx://MyAssembly/Content/Sprites/a3000" + i + ".png")));
                _rockTypes.Add("a4000" + i, new BitmapImage(new Uri("ms-appx://MyAssembly/Content/Sprites/a4000" + i + ".png")));
                //_rockTypes.Add("b1000" + i, new BitmapImage(new Uri("ms-appx://MyAssembly/Content/Sprites/b1000" + i + ".png")));
                //_rockTypes.Add("b3000" + i, new BitmapImage(new Uri("ms-appx://MyAssembly/Content/Sprites/b3000" + i + ".png")));
                //_rockTypes.Add("b4000" + i, new BitmapImage(new Uri("ms-appx://MyAssembly/Content/Sprites/b4000" + i + ".png")));
            }
        }

        #endregion Game Methods

        #region Status Bar

        /// <summary>
        /// Hides the Windows status bar
        /// </summary>
        private async void HideStatusBar()
        {
            var statusBar = StatusBar.GetForCurrentView();
            await statusBar.HideAsync();
        }

        /// <summary>
        /// Displays a status bar that is incorporated into the app background
        /// </summary>
        private void DiscreteStatusBar()
        {
            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
        }

        #endregion Status Bar

        #region Navigation

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _gameOver = true;
            CompositionTarget.Rendering -= GameLoop;
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion NavigationHelper registration

        #endregion Navigation
    }
}