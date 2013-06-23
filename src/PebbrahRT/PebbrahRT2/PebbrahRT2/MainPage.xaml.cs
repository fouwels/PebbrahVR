using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PebbrahRT2.Templates;
using Windows.Devices.Geolocation;
using Windows.Media.Capture;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PebbrahRT2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string fourSquareLimit = "10";
        private List<string> placeElementNames = new List<string>();

        private bool isCompassUpdating = false;
        private double compassReadingRaw;

        private double geoLat;
        private double geoLong;


        public MainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Status.Text = "Status: " + "App started";

            //compass
            var compass = Windows.Devices.Sensors.Compass.GetDefault();
            //compass.ReportInterval = 1;
            compass.ReadingChanged += compass_ReadingChanged;
            Status.Text = "Status: " + "Compass started";

            //geo
            var gloc = new Geolocator();
            gloc.GetGeopositionAsync();
            gloc.ReportInterval = 60000;
            gloc.PositionChanged += gloc_PositionChanged;
            Status.Text = "Status: " + "Geo started";

            //foursquare
            await GetEstablishmentsfromWed();

            //camera
            var mediaCapture = new MediaCapture();
            await mediaCapture.InitializeAsync();
            CaptureElement.Source = mediaCapture;
            await mediaCapture.StartPreviewAsync();
            Status.Text = "Status: " + "Camera feed running";
        }

        async Task<Establishment> GetEstablishmentsfromWed()
        {
            //shitty code
            string url = "https://api.foursquare.com/v2/venues/search?ll=51.50758,-0.131537&limit=" + fourSquareLimit + "&client_id=LTXFP1QPO2LIF5EWMG3X0CPH5CW0NPKMVUJPBJTO5BH001KG&client_secret=3AQHWW4XRMYWF0L13TQC4P2KLJXN2ICUC2IL00NFKEBXH4VX&v=20130622";
            var req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.ContentType = "application/json";
            req.Method = "GET";
            var resp = await req.GetResponseAsync();
            var respStream = resp.GetResponseStream();

            string clean;

            using (var sr = new StreamReader(respStream))
            {
                 clean = sr.ReadToEnd();
            }

            string debug = clean;

            var venues = JsonConvert.DeserializeObject<FoursquareResponseRootObject>(clean).response.venues;

            foreach (var venue in venues)
            {
                StackPanel aStackPanel = new StackPanel();
                aStackPanel.HorizontalAlignment = HorizontalAlignment.Center;
                
                Ellipse aEllipse = new Ellipse();

                aEllipse.Height = 50;
                aEllipse.Width = aEllipse.Height;

                aEllipse.Fill = new SolidColorBrush(Colors.Gray) {Opacity = 0.75};

                aEllipse.StrokeThickness = 2;
                aEllipse.Stroke = new SolidColorBrush(Colors.Black) { Opacity = 1 };

                TextBlock aTextBlock = new TextBlock();
                aTextBlock.Text = venue.name;
                aTextBlock.FontSize = 12;

                //mafs

                var usLat = geoLat;
                var usLong = geoLong;
                var themLat = venue.location.lat;
                var themLong = venue.location.lng;

                var x = Math.Tanh((themLong - usLong)/(themLat - usLat));
                double y = 0;
                
                if (usLat < themLat)
                {
                    if (usLong < themLong)
                    {
                        y = 0 + x;
                    }
                    if (usLong > themLong)
                    {
                        y = 360 - x;
                    }
                }
                if (usLat > themLat)
                {
                    if (usLong < themLong)
                    {
                        y = 180 + x;
                    }
                    if (usLong > themLong)
                    {
                        y = 180 - x;
                    }
                }

                //end mafs


                //stuff
                aStackPanel.Margin = new Thickness(0, 0, Math.Floor(Convert.ToDouble(Thing2.Text) + y) , 0);
                aStackPanel.Name = venue.name;

                placeElementNames.Add(aStackPanel.Name);

                aStackPanel.Children.Add(aEllipse);
                aStackPanel.Children.Add(aTextBlock);
                PlacesStack.Children.Add(aStackPanel);
            }
            return null;
        }
        void gloc_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            geoLat = args.Position.Coordinate.Latitude;
            geoLong = args.Position.Coordinate.Longitude;
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Thing1.Text = geoLat.ToString() + " " + geoLong.ToString();
                });
        }

        void compass_ReadingChanged(Windows.Devices.Sensors.Compass sender, Windows.Devices.Sensors.CompassReadingChangedEventArgs args)
        {
               if (!isCompassUpdating)
               {
                   isCompassUpdating = true;
                   double rawMinusOne = compassReadingRaw;
                   compassReadingRaw = args.Reading.HeadingMagneticNorth;

                   this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                   {
                       Thing2.Text = compassReadingRaw.ToString();

                       foreach (var name in placeElementNames)
                       {
                           (this.FindName(name) as StackPanel).Margin = new Thickness(0, 0, Math.Floor(rawMinusOne - compassReadingRaw), 0);
                       }

                   });
                   isCompassUpdating = false;
               }

        }
    }
}
