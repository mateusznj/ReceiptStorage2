using System;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Device.Location;

namespace ReceiptStorage.Extensions
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PlaceHelper
    {

        [JsonProperty]
        public int distance { get; set; }

        [JsonProperty]
        public string title { get; set; }

        [JsonProperty]
        public double averageRating { get; set; }

        [JsonProperty]
        public string icon { get; set; }

        [JsonProperty]
        public string vicinity { get; set; }

        [JsonProperty]
        public string type { get; set; }

        [JsonProperty]
        public string href { get; set; }

        [JsonProperty]
        public string id { get; set; }

        public GeoCoordinate position { get; set; }

        [JsonProperty]
        public Category category { get; set; }

    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Category
    {
        [JsonProperty]
        public string id { get; set; }

        [JsonProperty]
        public string title { get; set; }

        [JsonProperty]
        public string href { get; set; }

        [JsonProperty]
        public string type { get; set; }
    }


}
