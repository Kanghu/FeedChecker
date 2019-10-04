using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/**
 * MediaViewModel is a wrapper for InstagramPost and InstagramUser,
 * acting as a ViewModel for these in order to be able to pass both
 * as a single object in the View that handles displaying media. 
 * 
 * ViewModel consists of an InstagramUser together with a list
 * of recent posts associated with that user.
 */

namespace FeedChecker.Library
{
    public class MediaViewModel
    {
        public InstagramUser User { get; set; }
        public IList<InstagramPost> Media { get; set; }
    }
}