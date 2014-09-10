using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace BallAndRock
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InstructionsPage : Page
    {
        public InstructionsPage()
        {
            this.InitializeComponent();
            uiTbInstructions.Text = "Stay Away from the Rocks!\nKeep the ball safe from the rocks. Move the ball by rotating the phone!\nThat's it!\nPretty simple, hum?";
            
        }


        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if(rootFrame != null && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
                e.Handled = true;
            }

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

        }
    }
}
