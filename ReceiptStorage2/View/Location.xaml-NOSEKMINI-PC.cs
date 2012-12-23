using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReceiptStorage.Extensions;
using ReceiptStorage.Utilities;
using RestSharp;

namespace ReceiptStorage.View
{
    public partial class Location : PhoneApplicationPage
    {
        private GeoCoordinateWatcher watcher = null;
        private GeoCoordinate myLocation;
        private string nokiaPlacesHereUrl = "http://demo.places.nlp.nokia.com/places/v1/discover/here";
        //private string nokiaPlaceUrl = "http://demo.places.nlp.nokia.com/places/v1/places/";
        private string nokiaPlacesAppID = SettingsApi.NokiaAppId;
        private string nokiaPlacesAppCode = SettingsApi.NokiaAppToken;
        private List<PlaceHelper> places;
        
        public Location()
        {
            InitializeComponent();
            mapPlaces.CredentialsProvider = new ApplicationIdCredentialsProvider(SettingsApi.BingApiKey);
        }

        private void Location_Loaded(object sender, RoutedEventArgs e)
        {
            WatcherStart();
        }
        private void Location_Click(object sender, EventArgs e)
        {
            WatcherStart();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            // Return to the main page.
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void WatcherStart()
        {
            if (watcher == null)
            {
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default); //Default
                watcher.StatusChanged += WatcherStatusChanged;
            }
            if (watcher.Status == GeoPositionStatus.Disabled)
            {
                watcher.StatusChanged -= WatcherStatusChanged;
                MessageBox.Show("Location services must be enabled on your phone.");
                return;
            }
            watcher.Start();
        }

        private void WatcherStatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {

            if (e.Status == GeoPositionStatus.Ready)
            {
                //Pushpin p = new Pushpin();
                myLocation = watcher.Position.Location;
                //p.Template = this.Resources["pinMyLoc"] as ControlTemplate;
                //p.Location = myLocation;
                
                //mapControl.Items.Add(p);
                mapPlaces.SetView(myLocation, 17);
                mapPlaces.ZoomBarVisibility = Visibility.Visible;
                watcher.Stop();
                this.GetPlaces(myLocation);
            }
        }

        private void GetPlaces(GeoCoordinate location)
        {
            string url = string.Format("{0}?at={1},{2}&app_id={3}&app_code={4}&tf=plain&pretty=true", nokiaPlacesHereUrl, location.Latitude.ToString().Replace(",", "."), location.Longitude.ToString().Replace(",", "."), nokiaPlacesAppID, nokiaPlacesAppCode);
            var client = new RestClient(url);
            var request = new RestRequest(string.Empty, Method.GET);
            client.ExecuteAsync<RestRequest>(request, (response) =>
            {
                places = this.ParsePlaces(response.Content);
                this.AddPushpins(places);
            });
        }

        private List<PlaceHelper> ParsePlaces(string json)
        {
            List<PlaceHelper> places = new List<PlaceHelper>();
            if (json == string.Empty)
                return places;

            JToken jToken = JObject.Parse(json)["results"]["items"];
            if (jToken == null)
                return places;

            IList<JToken> jlList = jToken.Children().ToList();
            foreach (JToken token in jlList)
            {
                PlaceHelper place = JsonConvert.DeserializeObject<PlaceHelper>(token.ToString());

                if (place.type == "urn:nlp-types:place")
                {
                    IList<JToken> jTokenListPosition = token["position"].ToList();
                    GeoCoordinate position = new GeoCoordinate();
                    position.Latitude = double.Parse(jTokenListPosition[0].ToString());
                    position.Longitude = double.Parse(jTokenListPosition[1].ToString());
                    place.position = position;

                    places.Add(place);
                }
            }

            return places;
        }

        private void AddPushpins(List<PlaceHelper> places)
        {
            mapPlaces.Children.Clear();

            Pushpin pushpinMe = new Pushpin();
            pushpinMe.Background = new SolidColorBrush(Colors.Green);
            pushpinMe.Location = myLocation;
            pushpinMe.Content = "Twoja pozcyja";
            mapPlaces.Children.Add(pushpinMe);

            foreach (PlaceHelper place in places)
            {
                Pushpin pushpin = new Pushpin();
                pushpin.Background = new SolidColorBrush(Colors.Black);
                pushpin.Opacity = 1.0;
                pushpin.Location = place.position;
                pushpin.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(PushpinTap);
                pushpin.Content = place.title;
                mapPlaces.Children.Add(pushpin);
            }
        }

        private void PushpinTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //ignoreMapTap = true;
            Pushpin pushpin = (sender as Pushpin);
            PlaceHelper place = places.Where(p => p.position == pushpin.Location).FirstOrDefault();
            this.DataContext = place;
            if (NavigationService.CanGoBack)
            {
                PhoneApplicationService.Current.State["place"] = place;
                NavigationService.GoBack();

            }
        }


    }
}
