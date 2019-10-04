using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

/* Represents an Instagram user. Class is built around
 * the JSON Object that represents users in Instagram's API. */

namespace FeedChecker.Library
{ 
    public class InstagramUser
    {
        public string ID { get; set; }
        public string Username { get; set; }

        [JsonProperty("profile_picture")]
        public string ProfilePicture { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }
        public string Bio { get; set; }
        public string Website { get; set; }
        public bool IsBussiness { get; set; }
        public Counter Counts { get; set; }

        public class Counter
        {
            public int Media { get; set; }
            public int Follows { get; set; }
            [JsonProperty("followed_by")]
            public int FollowedBy { get; set; }
        }
    }
}