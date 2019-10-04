using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/**
 * PostViewModel is a wrapper for InstagramPost and InstagramUser,
 * acting as a ViewModel for these in order to be able to pass both
 * as a single object in the View that handles displaying a post.
 * 
 * ViewModel consists of an InstagramPost and the comments associated
 * with that post.
 */

namespace FeedChecker.Library
{
    public class PostViewModel
    {
        public InstagramPost Post { get; set; }
        public IList<InstagramComment> Comments { get; set; }
    }
}