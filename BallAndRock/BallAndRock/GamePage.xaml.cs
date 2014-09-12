using BallAndRock.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.Devices.Sensors;
using Windows.Graphics.Display;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace BallAndRock
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
        #region Properties and Variables

        #region Rock Properties

        private const int _maxSpeed = 15;
        private const int _minSpeed = 1;
        private const int _rockSize = 25;
        private const int _maxRocks = 10;
        private const double _rockInitPos = -50;
        private int _nRocks = 1;
        private int _totalRocks = 0;
        private Dictionary<int, string> _rockTypes = null;
        private Dictionary<Rock, Image> RockImages;

        #endregion Rock Properties

        #region Ball Properties

        private const double ballSize = 60;
        private double ballHeight;
        private Ball ball;
        private Image _imgBall;

        #endregion Ball Properties

        #region Screen properties

        private double screenHeight = 0;
        private double screenWidth = 0;
        private double minWidth =0;

        #endregion Screen properties

        #region Others

        private DispatcherTimer timer;
        private Stopwatch _timer;
        private long _lastMiliseconds = 0;
        private int _score = 0;
        private Random rand;
        private bool _gameOver = false;
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        //private Inclinometer _inclinometer;
        private Accelerometer _accelerometer;
        #endregion Others

        #endregion Properties and Variables

        public GamePage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            HideStatusBar();
            rand = new Random((int)DateTime.Now.Ticks);
            LoadRockTypes();
            _timer = new Stopwatch();
            _timer.Start();
            _accelerometer = Accelerometer.GetDefault();
            //_inclinometer = Inclinometer.GetDefault();

            if (_accelerometer != null)
            {
                uint minReportInterval = _accelerometer.MinimumReportInterval;
                uint reportInterval = minReportInterval > 16 ? minReportInterval : 16;
                _accelerometer.ReportInterval = reportInterval;
                _accelerometer.ReadingChanged += _accelerometer_ReadingChanged;
            }

            //if (_inclinometer != null)
            //{
            //    // Establish the report interval for all scenarios
            //    uint minReportInterval = _inclinometer.MinimumReportInterval;
            //    uint reportInterval = minReportInterval > 16 ? minReportInterval : 16;
            //    _inclinometer.ReportInterval = reportInterval;

            //    // Establish the event handler
            //    _inclinometer.ReadingChanged += _inclinometer_ReadingChanged;
            //}

            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, rand.Next(1, 1500));
            timer.Start();

            CompositionTarget.Rendering += GameLoop;
        }

        private async void _accelerometer_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (ball != null)
                {
                    AccelerometerReading reading = args.Reading;
                    double acel = reading.AccelerationX /9.8 / 100;
                    double initSpeed = ball.Speed;
                    double initX = ball.X;
                    uint t = _accelerometer.ReportInterval;

                    ball.Speed = initSpeed + (acel * t);
                    ball.X =  initX + (initSpeed * t) + ((0.5) * acel * (t * t));

                    if (ball.X < minWidth)
                    {
                        ball.X = minWidth;
                        ball.Speed = 0;//-ball.Speed; // Bouncing
                    }
                    if (ball.X > screenWidth)
                    {
                        ball.X = screenWidth;
                        ball.Speed = 0;//-ball.Speed; // Bouncing
                    }
                }
            });
        }

        //async void _inclinometer_ReadingChanged(Inclinometer sender, InclinometerReadingChangedEventArgs args)
        //{
        //    if (ball != null)
        //    {
        //        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        //        {
        //            InclinometerReading reading = args.Reading;
        //            ball.X += reading.RollDegrees / 2;
                
        //        });

        //    }
        //}

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        private void uiCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            screenWidth = Window.Current.Bounds.Width-65;
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
            if(!_gameOver)
            {
                CheckForCollisions();
                
                UpdateBall();

                UpdateRocks();
            }
        }

        private void CheckForCollisions()
        {
            for (int i = 0; i < RockImages.Count; i++)
            {
                Rock rock = RockImages.ElementAt(i).Key;
                if (Math.Abs(ball.X - rock.X) < (0.5 * ball.Size) + (0.5 * rock.Size) &&
                    Math.Abs(ball.Y - rock.Y) < (0.5 * ball.Size) + (0.5 * rock.Size))
                {
                    _gameOver = true;
                    CompositionTarget.Rendering -= GameLoop;

                    // Update highscore if new score is higher
                    if(Int32.Parse(ApplicationData.Current.LocalSettings.Values["HighScore"].ToString()) < _score)
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values["highScore"] = _score.ToString();

                    Frame.Navigate(typeof(MessageInfoPage),
                        new MessageParameters("Game Over!",
                        "Sorry! Play again?", true, true));
                }
            }
        }

        /// <summary>
        /// Randomly creates a new rock to be released in game
        /// </summary>
        private void timer_Tick(object sender, object e)
        {
            if (RockImages.Count < _nRocks)
            {
                AddRock();
            }
            timer.Interval = new TimeSpan(0, 0, 0, 0, rand.Next(1, 1000));
        }

        private void AddBall()
        {
            ballHeight = screenHeight - ballSize;
            ball = new Ball(screenWidth / 2, ballHeight, ballSize, 0, "2.png");
            _imgBall = new Image();
            _imgBall.Width = 65;
            _imgBall.Height = 65;
            _imgBall.SetValue(Canvas.LeftProperty, ball.X);
            _imgBall.SetValue(Canvas.TopProperty, ball.Y);
            _imgBall.Source = new BitmapImage(new Uri("ms-appx://MyAssembly/Content/Sprites/" + ball.BallFile));

            uiCanvas.Children.Add(_imgBall);
        }

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
            RockImages.Add(new Rock(_totalRocks, (double) rand.Next((int)minWidth, (int)screenWidth), _rockInitPos, _rockSize, rand.Next((int)_minSpeed, (int)_maxSpeed), _rockTypes[rand.Next(0, _rockTypes.Count-1)]), new Image() { Name = _totalRocks.ToString() });

            Rock rock = RockImages.Keys.Where(i => i.RockNumber == _totalRocks).First();

            RockImages.Values.Where(i => i.Name == _totalRocks.ToString()).First().SetValue(Canvas.LeftProperty, rock.X);
            RockImages.Values.Where(i => i.Name == _totalRocks.ToString()).First().SetValue(Canvas.TopProperty, rock.Y);
            RockImages.Values.Where(i => i.Name == _totalRocks.ToString()).First().Source = new BitmapImage(new Uri("ms-appx://MyAssembly/Content/Sprites/" + rock.RockType + "0.png"));

            uiCanvas.Children.Add(RockImages.Values.Where(i => i.Name == _totalRocks.ToString()).FirstOrDefault());
        }

        private void UpdateRocks()
        {
            // Increases the number of rocks by one every 5 seconds until max number of rocks is reached.
            if (_timer.ElapsedMilliseconds - _lastMiliseconds > 5000)
            {
                if (_nRocks < _maxRocks)
                    _nRocks++;

                _lastMiliseconds = _timer.ElapsedMilliseconds;
            }

            List<Rock> toBeRemoved = new List<Rock>();
            for (int i = 0; i < RockImages.Count; i++)
            {
                Rock kRock = RockImages.ElementAt(i).Key;
                kRock.Y += kRock.Speed;
                kRock.ImagePos++;

                if (kRock.ImagePos == 10)
                    kRock.ImagePos = 0;

                // If rock passed the screen (reached the bottom), remove ball
                if (kRock.Y >= screenHeight)
                {
                    _score++;
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
        /// Loads the dictionary with the color
        /// </summary>
        private void LoadRockTypes()
        {
            _rockTypes = new Dictionary<int, string>();
            _rockTypes.Add(0, "a1000");
            _rockTypes.Add(1, "a3000");
            _rockTypes.Add(2, "a4000");
            _rockTypes.Add(3, "b1000");
            _rockTypes.Add(4, "b3000");
            _rockTypes.Add(5, "b4000");
        }

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

        #region Navigation
        ///// <summary>
        ///// Back button override
        ///// </summary>
        //private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        //{
        //    Frame rootFrame = Window.Current.Content as Frame;
        //    if (rootFrame != null && rootFrame.CanGoBack)
        //    {
        //        rootFrame.GoBack();
        //        e.Handled = true;
        //    }
        //}

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