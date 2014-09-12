using BallAndRock.Common;
using System;
using Windows.Devices.Sensors;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BallAndRock
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public MainPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            uiTbHighScore.Text = Windows.Storage.ApplicationData.Current.LocalSettings.Values["highScore"].ToString();

            HideStatusBar();

            // Locks portrait mode for all application. Global setting.
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
        }

        /// <summary>
        /// Function that hides the status bar.
        /// </summary>
        private async void HideStatusBar()
        {
            var statusBar = StatusBar.GetForCurrentView();
            await statusBar.HideAsync();
        }

        /// <summary>
        /// Function that incorporates the status bar into the background of the application.
        /// </summary>
        private void DiscreteStatusBar()
        {
            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }

        private void uiBtStartGame_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GamePage));
        }

        private void uiAppBtGameInstructions_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(InstructionsPage));
        }

        private void uiAppBtAbout_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Should be handled by the store, but just in case
            if (Accelerometer.GetDefault() == null) 
            {
                uiBtStartGame.IsEnabled = false;
                uiTbInfo.Text = "Your phone doesn't support this game because it doesn't have an accelerometer. Sorry!";
            }
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
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #endregion Navigation
    }
}