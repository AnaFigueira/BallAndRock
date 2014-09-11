using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Phone.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace BallAndRock
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {


        public MainPage()
        {
            this.InitializeComponent();

            HideStatusBar();

            // Locks portrait mode for all application. Global setting.
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }



        //void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        //{
        //    Frame rootFrame = Window.Current.Content as Frame;
        //    if (rootFrame != null && rootFrame.CanGoBack)
        //    {

        //        rootFrame.GoBack();
        //        e.Handled = true;

        //    }

        //}

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
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
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
            //HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        #region Navigation

        #endregion





    }
}
