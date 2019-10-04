using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

/* Represents an Instagram post. Class is built around
 * the JSON Object that represents posts in Instagram's API. */

namespace FeedChecker.Library
{
    public class InstagramPost
    {
        public string ID { get; set; }
        public InstagramUser User { get; set; }
        [JsonProperty("created_time")]
        public int CreatedTime { get; set; }
        public Counter Likes { get; set; }
        public Counter Comments { get; set; }
        public string Type { get; set; }
        public string Link { get; set; }
        public MediaVariants Images { get; set; }
        public MediaVariants Videos { get; set; }
        public LocationInfo Location { get; set; }
        public CaptionInfo Caption { get; set; }

        public class Counter
        {
            public int Count { get; set; }
        }

        public class MediaVariants
        {
            [JsonProperty("low_resolution")]
            public MediaInfo LowResolution { get; set; }
            public MediaInfo Thumbnail { get; set; }
            [JsonProperty("standard_resolution")]
            public MediaInfo StandardResolution { get; set; }
        }

        public class MediaInfo
        {
            public string Url { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }

        public class LocationInfo
        {
            public string ID { get; set; }
            public int Latitude { get; set; }
            public int Longitude { get; set; }
            [JsonProperty("street_address")]
            public string StreetAddress { get; set; }
            public string Name { get; set; }
        }

        public class CaptionInfo
        {
            public string ID { get; set; }
            [JsonProperty("created_time")]
            public int CreatedTime { get; set; }
            public string Text { get; set; }
            public InstagramUser From { get; set; }
        }
    }
}