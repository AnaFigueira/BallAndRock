using BallAndRock.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Phone.UI.Input;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
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
        Dictionary<int, string> colors = null;
        private Stopwatch _timer;
        private long lastMiliseconds = 0;
        private const int ballSize = 50;
        private const int rockSize = 50;
        private int screenHeight = 0;
        private int minHeight = -50;
        private int minWidth = -15;
        private int screenWidth = 0;
        private Ball ball;
        private int _speed=5;
        private int _maxRocks = 10;
        private int _nRocks = 1;
        private int _totalRocks = 0;
        private int _score = 0;
        private bool _isPlaying = false;

        private Dictionary<Rock, Image> RockImages;
        
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private Accelerometer _accelerometer;
        Random rand;

        public GamePage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            HideStatusBar();
            rand = new Random((int)DateTime.Now.Ticks);
            LoadColors();
            _timer = new Stopwatch();
            _timer.Start();
            _accelerometer = Accelerometer.GetDefault();
            if(_accelerometer!=null)
            {
                uint minReportInterval = _accelerometer.MinimumReportInterval;
                uint reportInterval = minReportInterval > 16 ? minReportInterval : 16;
                _accelerometer.ReportInterval = reportInterval;
                _accelerometer.ReadingChanged += _accelerometer_ReadingChanged;
            }
            else
            {
                //TODO: Display Error message and don't proceed.
            }

            CompositionTarget.Rendering += GameLoop;
            _isPlaying = true;
        }

        async void _accelerometer_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if(ball!=null)
                {
                    AccelerometerReading reading = args.Reading;
                    ball.X += (int)(reading.AccelerationX  * 100);
                }
            });
        }

        void LoadColors()
        {
            colors = new Dictionary<int, string>();
            colors.Add(0, "a1000");
            colors.Add(1, "a3000");
            colors.Add(2, "a4000");
            colors.Add(3, "b1000");
            colors.Add(4, "b3000");
            colors.Add(5, "b4000");
        }

        private void GameLoop(object sender, object e)
        {
            if (ball.X <= minWidth ) ball.X = minWidth;
            if (ball.X > screenWidth) ball.X = screenWidth;

            uiImgBall.SetValue(Canvas.LeftProperty, ball.X);
            uiImgBall.SetValue(Canvas.TopProperty, ball.Y);

            if(_timer.ElapsedMilliseconds - lastMiliseconds > 2000)
            {
                if(_speed<20)
                    _speed++;
                if(_nRocks < _maxRocks)
                    _nRocks++;
                if (RockImages.Count < _nRocks)
                {
                    for (int i = 0; i < _nRocks; i++ )
                        AddRock();
                }
                lastMiliseconds = _timer.ElapsedMilliseconds;
            }
            List<Rock> toBeRemoved = new List<Rock>();
            for (int i = 0; i < RockImages.Count; i++)
            {
                RockImages.ElementAt(i).Key.Y += _speed;
                RockImages.ElementAt(i).Key.ImagePos++;

                if (RockImages.ElementAt(i).Key.ImagePos == 10)
                    RockImages.ElementAt(i).Key.ImagePos = 0;

                if (RockImages.ElementAt(i).Key.Y >= screenHeight)
                {
                    _score++;
                    uiTbScore.Text = _score.ToString();
                    IEnumerable<Image> images = uiCanvas.Children.OfType<Image>();
                    foreach (Image c in images)
                     {
                        
                         if (c.Name == RockImages.ElementAt(i).Value.Name)
                         {
                            uiCanvas.Children.Remove(c);
                            toBeRemoved.Add(RockImages.ElementAt(i).Key);
                         }
                     }
                }
                else
                {
                    RockImages.ElementAt(i).Value.SetValue(Canvas.LeftProperty, RockImages.ElementAt(i).Key.X);
                    RockImages.ElementAt(i).Value.SetValue(Canvas.TopProperty, RockImages.ElementAt(i).Key.Y);
                }
            }

            foreach(var r in toBeRemoved)
            {
               
                RockImages.Remove(r);
            }


            //uiImgRock.Source = new BitmapImage(new Uri("ms-appx://MyAssembly/Content/Sprites/"+ rock.Color + rock.ImagePos + ".png"));
        }

        void AddRock()
        {
            _totalRocks++;
            RockImages.Add(new Rock(_totalRocks, rand.Next(minWidth, screenWidth), -50, rockSize, _speed, colors[rand.Next(0, 5)]), new Image() {Name= _totalRocks.ToString()});

            Rock rock = RockImages.Keys.Where(i => i.RockNumber == _totalRocks).First();

            RockImages.Values.Where(i => i.Name == _totalRocks.ToString()).First().SetValue(Canvas.LeftProperty, rock.X);
            RockImages.Values.Where(i => i.Name == _totalRocks.ToString()).First().SetValue(Canvas.TopProperty, rock.Y);
            RockImages.Values.Where(i => i.Name == _totalRocks.ToString()).First().Source = new BitmapImage(new Uri("ms-appx://MyAssembly/Content/Sprites/" + rock.Color + "0.png"));

            uiCanvas.Children.Add(RockImages.Values.Where(i => i.Name == _totalRocks.ToString()).FirstOrDefault());
        }


        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
                e.Handled = true;
            }
        }

        
        private void uiCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            // Must use ActualHeight/ActualWidth
            screenHeight = (int)uiCanvas.ActualHeight+50;
            screenWidth = (int)uiCanvas.ActualWidth - 50;
            int ballHeight = screenHeight - 75;
            // Initialize ball to be in the center of the screen
            
            ball = new Ball(screenWidth / 2, ballHeight, ballSize, colors[rand.Next(0,5)]);
            uiImgBall.SetValue(Canvas.LeftProperty, ball.X);
            uiImgBall.SetValue(Canvas.TopProperty, ball.Y);

            uiImgBall.Source = new BitmapImage(new Uri("ms-appx://MyAssembly/Content/Sprites/" + ball.Color + "0.png"));

            RockImages = new Dictionary<Rock, Image>();

            Rock rock = new Rock(_totalRocks, minWidth, 200, rockSize, 1, colors[rand.Next(0, 5)]);
            Image rockimg= new Image();

            rockimg.SetValue(Canvas.LeftProperty, rock.X);
            rockimg.SetValue(Canvas.TopProperty, rock.Y);
            uiCanvas.Children.Add(rockimg);
            rockimg.Source = new BitmapImage(new Uri("ms-appx://MyAssembly/Content/Sprites/" + rock.Color + "0.png"));
        }


        private async void HideStatusBar()
        {
            var statusBar = StatusBar.GetForCurrentView();
            await statusBar.HideAsync();
        }

        private void DiscreteStatusBar()
        {
            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        private void ReleaseRock()
        {
            int column = rand.Next(1, 7);
            BitmapImage image = new BitmapImage(new Uri("ms-resource://MyAssembly/Content/Sprites/a10000.png"));

            image.SetValue(Grid.RowProperty, 0);
            image.SetValue(Grid.ColumnProperty, column);
        }





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
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion


        #endregion

       
       
    }
}
