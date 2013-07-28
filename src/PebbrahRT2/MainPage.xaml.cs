using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PebbrahRT2.Templates;
using Windows.Devices.Geolocation;
using Windows.Devices.Sensors;
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

        private double m;
        

        private double speed = 1;

        private bool autoEnabled = false;
        
        private bool triggerRampDown = false;

        private List<Venue> venues = new List<Venue>();

        Random rnd = new Random();

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Status.Text = "Status: " + "App started";

            //compass
            var compass = Windows.Devices.Sensors.Compass.GetDefault();
            compass.ReportInterval = 10;
            //compass.ReadingChanged += compass_ReadingChanged;
            Status.Text = "Status: " + "Compass started";

            //geo
            var gloc = new Geolocator();
            gloc.GetGeopositionAsync();
            gloc.ReportInterval = 60000;
            gloc.PositionChanged += gloc_PositionChanged;
            Status.Text = "Status: " + "Geo started";

            //Accelerometer
            var aclom = Accelerometer.GetDefault();
            aclom.ReportInterval = 1;
            aclom.ReadingChanged += aclom_ReadingChanged;

            //foursquare
            await GetEstablishmentsfromWed();

            //camera
            var mediaCapture = new MediaCapture();
            await mediaCapture.InitializeAsync();
            CaptureElement.Source = mediaCapture;
            await mediaCapture.StartPreviewAsync();
            Status.Text = "Status: " + "Camera feed running";
        }

        private async Task<Establishment> GetEstablishmentsfromWed()
        {
            //shitty code
            string url = "https://api.foursquare.com/v2/venues/search?ll=51.50758,-0.131537&limit=" + fourSquareLimit +
                         "&client_id=LTXFP1QPO2LIF5EWMG3X0CPH5CW0NPKMVUJPBJTO5BH001KG&client_secret=3AQHWW4XRMYWF0L13TQC4P2KLJXN2ICUC2IL00NFKEBXH4VX&v=20130622";
            var req = (HttpWebRequest) HttpWebRequest.Create(url);
            req.ContentType = "application/json";
            req.Method = "GET";
            var resp = await req.GetResponseAsync();
            var respStream = resp.GetResponseStream();

            string clean;

            using (var sr = new StreamReader(respStream))
            {
                clean = sr.ReadToEnd();
            }

            venues = JsonConvert.DeserializeObject<FoursquareResponseRootObject>(clean).response.venues;

            foreach (var venue in venues)
            {
                StackPanel aStackPanel = new StackPanel();
                aStackPanel.HorizontalAlignment = HorizontalAlignment.Center;

                Ellipse aEllipse = new Ellipse();

                aEllipse.Height = 50;
                aEllipse.Width = aEllipse.Height;

                aEllipse.Fill = new SolidColorBrush(Colors.Gray) {Opacity = 0.75};

                aEllipse.StrokeThickness = 2;
                aEllipse.Stroke = new SolidColorBrush(Colors.Black) {Opacity = 1};

                TextBlock aTextBlock = new TextBlock();
                aTextBlock.Text = venue.name;
                aTextBlock.FontSize = 12;

                ////mafs

                venue.location.myStarting = 0 + (geoLong - venue.location.lng)*1000000; //debug
                aStackPanel.Margin = new Thickness(0, 0, Math.Floor(venue.location.myStarting), 0);
                aStackPanel.Name = venue.name;

                placeElementNames.Add(aStackPanel.Name);

                aStackPanel.Children.Add(aEllipse);
                aStackPanel.Children.Add(aTextBlock);
                PlacesStack.Children.Add(aStackPanel);
            }
            return null;
        }

        private void gloc_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            geoLat = args.Position.Coordinate.Latitude;
            geoLong = args.Position.Coordinate.Longitude;
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Thing1.Text = geoLat.ToString() + " " + geoLong.ToString();
                });
        }

        //private void compass_ReadingChanged(Windows.Devices.Sensors.Compass sender,
        //                                    Windows.Devices.Sensors.CompassReadingChangedEventArgs args)
        void aclom_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            
            if (!isCompassUpdating)
            {
                isCompassUpdating = true;
                //double rawMinusOne = compassReadingRaw;

                if (autoEnabled)
                {
                    if (triggerRampDown)
                    {
                        m = m - speed;
                    }
                    else if (!triggerRampDown)
                    {
                        m = m + speed;
                    }
                    if (m > 1300*2)
                    {
                        m = 0;
                    }

                    if (m < 0)
                    {
                        m = 1300*2;
                    }
                }
                else
                {
                    //m = Math.Floor((rawMinusOne - compassReadingRaw)*1);
                    m = m + Math.Floor(args.Reading.AccelerationX*100);
                    if (m > 1300*2)
                    {
                        m = m - 1300*2;
                    }

                    if (m < -1300*2)
                    {
                        m = m + 1300*2;
                    }
                }

                this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        Thing2.Text = m.ToString();

                        foreach (var venue in venues)
                        {
                            
   
                            var x = Math.Floor(venue.location.myStarting + m);
                            if (x > 1300)
                            {
                                x = x - 1300*2;
                            }
                            else if (x < -1300)
                            {
                                x = x + 1300*2;
                            }

                            (this.FindName(venue.name) as StackPanel).Margin = new Thickness(0, 0, x, 0);
                            
                        }
                        isCompassUpdating = false;
                    });
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            triggerRampDown = !triggerRampDown;
        }

        private void ButtonBase_OnClickUp(object sender, RoutedEventArgs e)
        {
            speed++;
        }

        private void ButtonBase_OnClickDown(object sender, RoutedEventArgs e)
        {
            speed--;
        }
    }
}
