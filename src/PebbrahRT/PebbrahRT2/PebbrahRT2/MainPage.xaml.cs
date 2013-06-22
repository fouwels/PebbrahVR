using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PebbrahRT2.Templates;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PebbrahRT2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Status.Text = "Status: " + "App started";

            var mediaCapture = new MediaCapture();
            await mediaCapture.InitializeAsync();
            CaptureElement.Source = mediaCapture;
    
            mediaCapture.StartPreviewAsync();

            Status.Text = "Status: " + "Camera feed running";

            var compass = Windows.Devices.Sensors.Compass.GetDefault();
            compass.ReadingChanged += compass_ReadingChanged;

            Geolocator gloc = new Geolocator();
            gloc.GetGeopositionAsync();
            gloc.PositionChanged += gloc_PositionChanged;
        }

        List<Establishment> getEstablishmentsfromWed()
        {
            //do shizzle
            return testDataSource();
        }
        void gloc_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Thing1.Text = args.Position.Coordinate.Latitude.ToString() +
                              args.Position.Coordinate.Longitude.ToString();
            });
        }

        void compass_ReadingChanged(Windows.Devices.Sensors.Compass sender, Windows.Devices.Sensors.CompassReadingChangedEventArgs args)
        {

            string asdf = args.Reading.HeadingMagneticNorth.ToString();

            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Thing2.Text = asdf;
            });

        }
        private List<Establishment> testDataSource()
        {
            var establishments = new List<Establishment>();
            
            establishments.Add(new Establishment { GPSLat = 51.507453, GPSLong = -0.131649, Name = "Home" });
            establishments.Add(new Establishment { GPSLat = 51.507463, GPSLong = -0.131659, Name = "Bar1" });
            establishments.Add(new Establishment { GPSLat = 51.507443, GPSLong = -0.131639, Name = "Bar2" });
            establishments.Add(new Establishment { GPSLat = 51.507463, GPSLong = -0.131669, Name = "Restaurant2" });
            establishments.Add(new Establishment { GPSLat = 51.507433, GPSLong = -0.131639, Name = "Restaurant3" });
            return establishments;
        }
    }
}
