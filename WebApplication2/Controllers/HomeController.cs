using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
//using System.Text.ASCIIEncoding;
//using System.Text.Encoding;
using System.Web.Http;
using System.Web.Mvc;
using WebApplication2.Models;
using C = WebApplication2.Models.User;
using System.Web.Script.Serialization;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetTweets(String username, int n)
        {
            try
            {
                /* Step 1 - Establish the connection*/
                string url = "https://api.twitter.com/1.1/statuses/user_timeline.json?screen_name=" + username + "&count=" + n.ToString();
                string oauthconsumerkey = C.CONSUMER_KEY;
                string oauthtoken = C.ACCESS_TOKEN;
                string oauthconsumersecret = C.CONSUMER_SECRET;
                string oauthtokensecret = C.ACCESS_SECRET;
                string oauthsignaturemethod = "HMAC-SHA1";
                string oauthversion = "1.0";
                string oauthnonce = Convert.ToBase64String(
                  new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
                TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                string oauthtimestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();
                SortedDictionary<string, string> basestringParameters = new SortedDictionary<string, string>();
                basestringParameters.Add("screen_name", username);
                basestringParameters.Add("count", n.ToString());
                basestringParameters.Add("oauth_version", oauthversion);
                basestringParameters.Add("oauth_consumer_key", oauthconsumerkey);
                basestringParameters.Add("oauth_nonce", oauthnonce);
                basestringParameters.Add("oauth_signature_method", oauthsignaturemethod);
                basestringParameters.Add("oauth_timestamp", oauthtimestamp);
                basestringParameters.Add("oauth_token", oauthtoken);
                //Build the signature string
                StringBuilder baseString = new StringBuilder();
                baseString.Append("GET" + "&");
                baseString.Append(EncodeCharacters(Uri.EscapeDataString(url.Split('?')[0]) + "&"));
                foreach (KeyValuePair<string, string> entry in basestringParameters)
                {
                    baseString.Append(EncodeCharacters(Uri.EscapeDataString(entry.Key + "=" + entry.Value + "&")));
                }

                //Remove the trailing ambersand char last 3 chars - %26
                string finalBaseString = baseString.ToString().Substring(0, baseString.Length - 3);

                //Build the signing key
                string signingKey = EncodeCharacters(Uri.EscapeDataString(oauthconsumersecret)) + "&" +
                EncodeCharacters(Uri.EscapeDataString(oauthtokensecret));

                //Sign the request
                HMACSHA1 hasher = new HMACSHA1(new ASCIIEncoding().GetBytes(signingKey));
                string oauthsignature = Convert.ToBase64String(
                  hasher.ComputeHash(new ASCIIEncoding().GetBytes(finalBaseString)));

                //Tell Twitter we don't do the 100 continue thing
                ServicePointManager.Expect100Continue = false;

                //authorization header
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(@url);
                StringBuilder authorizationHeaderParams = new StringBuilder();
                authorizationHeaderParams.Append("OAuth ");
                authorizationHeaderParams.Append("oauth_nonce=" + "\"" + Uri.EscapeDataString(oauthnonce) + "\",");
                authorizationHeaderParams.Append("oauth_signature_method=" + "\"" + Uri.EscapeDataString(oauthsignaturemethod) + "\",");
                authorizationHeaderParams.Append("oauth_timestamp=" + "\"" + Uri.EscapeDataString(oauthtimestamp) + "\",");
                authorizationHeaderParams.Append("oauth_consumer_key=" + "\"" + Uri.EscapeDataString(oauthconsumerkey) + "\",");
                if (!string.IsNullOrEmpty(oauthtoken))
                    authorizationHeaderParams.Append("oauth_token=" + "\"" + Uri.EscapeDataString(oauthtoken) + "\",");
                authorizationHeaderParams.Append("oauth_signature=" + "\"" + Uri.EscapeDataString(oauthsignature) + "\",");
                authorizationHeaderParams.Append("oauth_version=" + "\"" + Uri.EscapeDataString(oauthversion) + "\"");
                webRequest.Headers.Add("Authorization", authorizationHeaderParams.ToString());

                webRequest.Method = "GET";
                webRequest.ContentType = "application/x-www-form-urlencoded";

                //Allow us a reasonable timeout in case Twitter's busy
                webRequest.Timeout = 3 * 60 * 1000;
                //try
                //{
                //Proxy settings
                // webRequest.Proxy = new WebProxy("enter proxy details/address");
                HttpWebResponse webResponse = webRequest.GetResponse() as HttpWebResponse;
                Stream dataStream = webResponse.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);

                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // var JsonResponse = JObject.Parse(responseFromServer);
                //JObject ojObject = (JObject)JsonResponse["response"];
                //var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                dynamic jsonObject = JsonConvert.DeserializeObject(responseFromServer);
                //JsonConvert.DeserializeObject<IEnumerable<T>>()
                var tweets = BuildTweets(jsonObject);
                return Json(new { success = true, data = tweets }, JsonRequestBehavior.AllowGet);
                // }


            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        private Queue<TweetViewModel> BuildTweets(dynamic tweets)
        {
            Queue<TweetViewModel> processedTweets = new Queue<TweetViewModel>();
            try
            {
                int count = 0;
                while (count < 10)
                {

                    String tweet = tweets[count].text;
                    String mediaURL = String.Empty;
                    String tweetContent = String.Empty;
                    GetMediaURLFromContent(tweet, ref mediaURL, ref tweetContent);
                    
                    TweetViewModel tempTweet = new TweetViewModel
                    {
                        UserName = tweets[count].user.name,
                        UserScreenName = tweets[count].user.screen_name,
                        UserProfileImageURL = tweets[count].user.profile_image_url_https,
                        TweetContent = tweetContent,
                        NumberOfReTweets = tweets[count].retweet_count,
                        TweetDate = tweets[count].created_at,   //Tried to Parse the String to DateTime but it is giving me error. Also wanted to convert the UTC time to local, but since it is not date I cannot
                        MediaURL = mediaURL
                    };
                    count++;
                    processedTweets.Enqueue(tempTweet);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return processedTweets;
        }
        private string EncodeCharacters(string data)
        {
            //as per OAuth Core 1.0 Characters in the unreserved character set MUST NOT be encoded
            //unreserved = ALPHA, DIGIT, '-', '.', '_', '~'
            if (data.Contains("!"))
                data = data.Replace("!", "%21");
            if (data.Contains("'"))
                data = data.Replace("'", "%27");
            if (data.Contains("("))
                data = data.Replace("(", "%28");
            if (data.Contains(")"))
                data = data.Replace(")", "%29");
            if (data.Contains("*"))
                data = data.Replace("*", "%2A");
            if (data.Contains(","))
                data = data.Replace(",", "%2C");

            return data;
        }
        private void GetMediaURLFromContent(String tweetText, ref String mediaURL, ref String tweetContent)
        {
            //String mediaURL = String.Empty;
            try
            {
                if (String.IsNullOrEmpty(tweetText))
                {
                    return;
                }
                String wordToSearch = "http";
                int firstIndex = tweetText.IndexOf(wordToSearch);
                int lastIndex = tweetText.LastIndexOf(wordToSearch);
                if (firstIndex == lastIndex)
                {
                    tweetContent = tweetText;
                    return;
                }
                else
                {
                    mediaURL = tweetText.Substring(lastIndex);
                    tweetContent = tweetText.Substring(0, lastIndex);
                }
                //return mediaURL;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}