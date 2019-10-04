using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using FeedChecker.Library;

namespace FeedChecker.Controllers
{
    public class InstagramController : Controller
    {
        // GET: Instagram
        public ActionResult Index()
        {
            return View();
        }

        // POST: Instagram
        /* This action represents a POST request that is triggered when 
         * the user would like to authenticate (e.g. Login button pressed).
         * According to InstagramAPI, the user must then be redirected to an
         * authorization URL where the authentication process takes place.
         * After authentication, the user is redirected to 'Access' action. */
        [HttpPost]
        public ActionResult Index(FormCollection notUsed)
        {
            /* Extract client ID and configured redirect URL from 'web.config' */
            string clientID = WebConfigurationManager.AppSettings["Instagram.ClientID"];
            string redirectURL = WebConfigurationManager.AppSettings["Instagram.RedirectUrl"];
            
            /* Create the authorization URL as specified in API docs */
            String authorizationURL = "https://api.instagram.com/oauth/authorize/?client_id=" + clientID
                + "&redirect_uri=" + redirectURL + "&response_type=code";

            return Redirect(authorizationURL);
        }

        // GET: Instagram/Access
        /* The Access action is the redirect configured in Instagram API's settings.
         * Once authentication has been succesful, the user is redirected here, with
         * a code parameter that must be exchanged for the access_token. In case of 
         * denial by user, the 'error' parameter will contain a value. */
        public ActionResult Access(string code, string error)
        {
            /* Either user has denied access or
             * there is no code parameter present. */
            if(error != null || code == null)
            {
                return RedirectToAction("Index");
            }

            /* Extract authentication data necessary for communication with Instagram servers.
             * Initialize an instance of InstagramSession, the class that handles communication
             * with the API. */
            string clientID = ConfigurationManager.AppSettings["Instagram.ClientID"];
            string clientSecret = ConfigurationManager.AppSettings["Instagram.ClientSecret"];
            string redirectURL = WebConfigurationManager.AppSettings["Instagram.RedirectUrl"];
            InstagramSession currentSession = new InstagramSession(clientID, clientSecret, redirectURL, code);
            
            /* Authentication succesful, access token has been retrieved.
             * Store current InstagramSession and redirect to 'Media'
             * controller for the purpose of fetching and displaying data. */
            if(currentSession.RequestAccesToken())
            {
                this.Session["CurrentSession"] = currentSession;
                return RedirectToAction("Media");
            }

            /* Authentication failed, back to homepage. */
            return RedirectToAction("Index");
        }
        
        // GET: Instagram/Media
        /* Returns latest media of currently authenticated user. The InstagramSession
         * object for the current session is preserved through .NET Session State.
         * */
        public ActionResult Media()
        {
            /* No current session could be found, back to the homepage. */
            if (this.Session["CurrentSession"] == null)
                return RedirectToAction("Index");

            /* Access the current InstagramSession and fetch user data/media. */
            InstagramSession currentSession = (InstagramSession) this.Session["CurrentSession"];
            IList<InstagramPost> recentMedia = currentSession.GetMedia();
            InstagramUser currentUser = currentSession.GetUser();

            /* Wrap the user and media into a single model and pass
             that to the view for displaying purposes. */
            MediaViewModel viewModel = new MediaViewModel { User = currentUser, Media = recentMedia };
            return View(viewModel);
        }
        
        public ActionResult Post(string mediaID)
        {
            /* No current session could be found, back to the homepage. */
            if (this.Session["CurrentSession"] == null)
                return RedirectToAction("Index");

            /* Access the current InstagramSession and fetch all media. */
            InstagramSession currentSession = (InstagramSession)this.Session["CurrentSession"];
            IList<InstagramPost> recentMedia = currentSession.GetMedia();

            /* Search for the specific post mentioned in 'mediaID' */
            foreach(InstagramPost post in recentMedia)
            {
                /* Post has been found */
                if (post.ID == mediaID)
                {
                    /* Fetch the comments of post in discussion,
                     * create a ViewModel and return it. */
                    IList<InstagramComment> comments = currentSession.GetComments(mediaID);
                    PostViewModel viewModel = new PostViewModel { Post = post, Comments = comments };
                    return View(viewModel);
                }
            }

            /* No media matching specified mediaID has been found */
            return RedirectToAction("Index");
        }
    }
}