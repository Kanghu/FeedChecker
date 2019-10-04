using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/**
 * InstagramSession is the class that handles interaction with Instagram's API,
 * such as requesting the access token and getting latest media. Communicating
 * with the API through HTTP requests and deserializing JSON responses is all
 * dealt with in this class, returning only the model objects we are interested in. 
 * 
 * Each instance of 'InstagramSession' represents a distinct connection with the API
 * (thus having an unique 'access_token' and user associated with it).
 * */

namespace FeedChecker.Library
{
    public class InstagramSession
    {
        public string AccessToken { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectURL { get; set; }
        public string Code { get; set; }

        /* Constructor is initialized with all the required data to set up
         * a connection with the API. */
        public InstagramSession(string clientID, string clientSecret, string redirectURL, string code)
        {
            this.ClientID = clientID;
            this.ClientSecret = clientSecret;
            this.RedirectURL = redirectURL;
            this.Code = code;
        }

        /* Requests an access token through Instagram API.
         * A valid acces token is required for fetching any
         * kind of data. Once the token has been retrieved, it
         * is stored in 'this.AccessToken' for further requests. */
        public bool RequestAccesToken()
        {
            /* Initialize the identification parameters that must be
             * POST'ed along with the code to Instagram's access_token endpoint */
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("client_id", this.ClientID);
            parameters.Add("client_secret", this.ClientSecret);
            parameters.Add("grant_type", "authorization_code");   // The only supported value
            parameters.Add("redirect_uri", this.RedirectURL);
            parameters.Add("code", this.Code);

            /* A WebClient is used to POST the authentication data 
            * in exchange for an OAuthToken. */
            using (WebClient exchangeClient = new WebClient())
            {
                var response = exchangeClient.UploadValues("https://api.instagram.com/oauth/access_token", "POST", parameters);

                /* Get response consisting of an array of 
                * bytes and convert it into string (JSON format) */
                var result = System.Text.Encoding.Default.GetString(response);

                /* Response comes as an OAuthToken encoded as a JSON
                *  string. The access token can be found with key "access_token". */
                JObject authToken = JObject.Parse(result);
                JToken access = authToken["access_token"];
                this.AccessToken = access.ToObject<String>();
            }

            /* Returns a boolean based on whether
             * or not acces token retrieval was succesful.*/
            return (this.AccessToken != null);
        }

        /* Get latest media through the 'media' endpoint. Response comes
         * in JSON format which is parsed and deserialized into a List of
         * type <InstagramComment>. After fetching all media, the list
         * is returned. */
        public IList<InstagramPost> GetMedia()
        {
            /* Prepare a GET request to the media endpoint */
            string mediaEndpoint = "https://api.instagram.com/v1/users/self/media/recent/?access_token=" + this.AccessToken;
            HttpWebRequest mediaRequest = (HttpWebRequest)HttpWebRequest.Create(mediaEndpoint);
            mediaRequest.Method = "GET";
            string mediaJson;

            /* Read the response into a string. 'using' is utilized
            *  to properly dispose of 'response' after reading is finished. */
            using (HttpWebResponse response = (HttpWebResponse)mediaRequest.GetResponse())
            {
                Stream responseStream = response.GetResponseStream();
                StreamReader responseReader = new StreamReader(responseStream);
                mediaJson = responseReader.ReadToEnd();
                responseStream.Close();
                responseReader.Close();
            }
            
            /* Parse the children tokens of 'mediaToken[data]', consisting
             * of JSON objects representing posts. Deserialize each
             * object and add it into a IList<InstagramPostModel> */
            JObject mediaToken = JObject.Parse(mediaJson);
            IList <JToken> mediaList = mediaToken["data"].Children().ToList();
            IList <InstagramPost> posts = new List<InstagramPost>();

            foreach(JToken postToken in mediaList)
            {
                posts.Add(postToken.ToObject<InstagramPost>());
            }

            /* Returns a list of recent media */
            return posts;
        }

        /* Get all known information about the user (owner of 'AccessToken'). 
         * Response comes in JSON format with information about the user
         * accessible by key 'data'. */
        public InstagramUser GetUser()
        {
            /* Prepare a GET request to the media endpoint */
            string userEndpoint = "https://api.instagram.com/v1/users/self/?access_token=" + this.AccessToken;
            HttpWebRequest userRequest = (HttpWebRequest)HttpWebRequest.Create(userEndpoint);
            userRequest.Method = "GET";
            string userJson;

            /* Read the response into a string. 'using' is utilized
             * to properly dispose of 'response' after reading is finished */
            using (HttpWebResponse response = (HttpWebResponse)userRequest.GetResponse())
            {
                Stream responseStream = response.GetResponseStream();
                StreamReader responseReader = new StreamReader(responseStream);
                userJson = responseReader.ReadToEnd();
                responseStream.Close();
                responseReader.Close();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
            }

            /* Parse the JSON 'data' field to access data about the user */
            JObject responseToken = JObject.Parse(userJson);
            JToken userToken = responseToken["data"];

            /* Deserialize the response into an InstagramUser object */
            InstagramUser currentUser = userToken.ToObject<InstagramUser>();
            return currentUser;
        }

        /* Get a list of recent comments on media object identified by
         * 'mediaID'. After establishing communication with 'comments' 
         * endpoint, the JSON list of comments is parsed and deserialized
         * into a list of type <InstagramComment> that is returned back */
        public IList<InstagramComment> GetComments(string mediaID)
        {
            /* Prepare a GET request to the comments endpoint */
            string commentsEndpoint = "https://api.instagram.com/v1/media/" + mediaID
                + "/comments?access_token=" + this.AccessToken;
            HttpWebRequest commentRequest = (HttpWebRequest)HttpWebRequest.Create(commentsEndpoint);
            commentRequest.Method = "GET";
            string commentsJson;

            /* Read the response into a string. 'using' is utilized
             * to properly dispose of 'response' after reading is finished */
            using (HttpWebResponse response = (HttpWebResponse)commentRequest.GetResponse())
            {
                Stream responseStream = response.GetResponseStream();
                StreamReader responseReader = new StreamReader(responseStream);
                commentsJson = responseReader.ReadToEnd();
                responseStream.Close();
                responseReader.Close();
            }

            /* Parse the children tokens of 'commentsToken[data]', consisting
            * of JSON objects representing comments. Deserialize each
            * object and add it into a IList<InstagramComment> */
            JObject commentsToken = JObject.Parse(commentsJson);
            IList<JToken> commentList = commentsToken["data"].Children().ToList();
            IList<InstagramComment> comments = new List<InstagramComment>();

            foreach (JToken commentToken in commentList)
            {
                comments.Add(commentToken.ToObject<InstagramComment>());
            }

            /* Return a list of comments associated with specified media */
            return comments;
        }
    }
}