using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

/* Represents an Instagram post. Class is built around
 * the JSON Object that represents comments in Instagram's API. */

namespace FeedChecker.Library
{
    public class InstagramComment
    {
        [JsonProperty("created_time")]
        public int CreatedTime { get; set; }
        public string Text { get; set; }
        public InstagramUser From { get; set; }
        public string ID { get; set; }
    }
}