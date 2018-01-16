using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;
using WebApplication2.Models;
using C = WebApplication2.Models.User;
using Constant = WebApplication2.Models.Constants;
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
        /// <summary>
        /// Reference from https://stackoverflow.com/questions/6378573/what-does-a-twitter-verify-credentials-look-like
        /// Connection code from https://www.codeproject.com/Articles/676313/Twitter-API-v-with-OAuth
        /// </summary>
        /// <param name="username">twitter username</param>
        /// <param name="n">number of tweets</param>
        /// <returns></returns>

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

                //Remove the trailing ampersand char last 3 chars - %26
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
                HttpWebResponse webResponse = webRequest.GetResponse() as HttpWebResponse;
                Stream dataStream = webResponse.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);

                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                dynamic jsonObject = JsonConvert.DeserializeObject(responseFromServer);
                var tweets = BuildTweets(jsonObject);
                return Json(new { success = true, data = tweets }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SearchTweets(String searchText, String[]tweetContent)
        {
            //JsonResult result = null;
            List<int> filteredTweets = new List<int>();
            try
            {
                if (String.IsNullOrEmpty(searchText))
                {
                    return Json(new { success = false, message = "Please enter text to search." });
                }
                if (tweetContent == null || tweetContent.Length < 1)
                {
                    return Json(new { success = false, message = "There are no tweets to search." });
                }
                if (tweetContent != null && tweetContent.Length > 10)
                {
                    return Json(new { success = false, message = "There more tweet array contains more than 10 elements." });
                }
                searchText = searchText.ToLower(); //for case insensitive comparison
                for (int index = 0; index < tweetContent.Length; index++)
                {
                    var content = tweetContent[index].ToLower(); //case insensitive comparison
                    if (content.Contains(searchText))
                    {
                        filteredTweets.Add(index);
                    }
                }
                return Json(new { success = true, data = filteredTweets }, JsonRequestBehavior.AllowGet);
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
                    String unprocessedDate = tweets[count].created_at;
                     
                    String mediaURL = String.Empty;
                    String tweetContent = String.Empty;
                    GetMediaURLFromContent(tweet, ref mediaURL, ref tweetContent);
                    var url = tweets[count].user.url;
                    TweetViewModel tempTweet = new TweetViewModel
                    {
                        UserName = tweets[count].user.name,
                        UserScreenName = tweets[count].user.screen_name,
                        UserProfileImageURL = tweets[count].user.profile_image_url_https,
                        TweetContent = tweetContent,
                        NumberOfReTweets = tweets[count].retweet_count,
                        TweetDate = ProcessDate(unprocessedDate),   //Tried to Parse the String to DateTime but it is giving me error. Also wanted to convert the UTC time to local, but since it is not date I cannot
                        MediaURL = url
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
        /// <summary>
        /// Code from https://www.codeproject.com/Articles/676313/Twitter-API-v-with-OAuth
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string EncodeCharacters(string data)
        {
            //as per OAuth Core 1.0 Characters in the unreserved character set MUST NOT be encoded
            //unreserved = ALPHA, DIGIT, '-', '.', '_', '~'
            switch (data)
            {
                case "!":
                    data = data.Replace("!", "%21");
                    break;
                case "'" :
                    data = data.Replace("'", "%27");
                    break;
                case "(" :
                    data = data.Replace("(", "%28");
                    break;
                case ")" :
                    data = data.Replace(")", "%29");
                    break;
                case "*" :
                    data = data.Replace("*", "%2A");
                    break;
                case "," :
                    data = data.Replace(",", "%2C");
                    break;
                default:
                    break;
            }
            return data;
        }
        /// <summary>
        /// Method to get the media url from the tweet content to embed the media
        /// </summary>
        /// <param name="tweetText" is the text from twitter api></param>
        /// <param name="mediaURL"></param>
        /// <param name="tweetContent" tweet content sans media url></param>
        private void GetMediaURLFromContent(String tweetText, ref String mediaURL, ref String tweetContent)
        {
            try
            {
                if (String.IsNullOrEmpty(tweetText))
                {
                    return;
                }
                String wordToSearch = "http";
                int firstIndex = tweetText.IndexOf(wordToSearch);
                int lastIndex = tweetText.LastIndexOf(wordToSearch);
                //There is no media url
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
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// Method that takes the UTC date time of the tweet in twitter api format
        /// and converts the date time to user time zone and 
        /// returns the date time of the tweet in user friendly format
        /// E.g Tue Jan 16 01:48:24 +0000 2018 this is the twitter api date where +0000 indicates UTC
        /// This method will return 1/15/2018 8:48:00 PM
        /// </summary>
        /// <param name="twitterDate"></param>
        /// <returns></returns>
        private String ProcessDate(String twitterDate)
        {
            /*
             * Twitter API returns date as Mon Jan 15 01:00:05 +0000 2018
             * Note: year is the end
             * C# uses date as Mon 15 Jan 2018 Time Offset
             * first need to convert to this format
             */
            DateTime date = DateTime.MinValue;
            String stringDate = String.Empty;
            String processedDate = String.Empty;
            if (String.IsNullOrEmpty(twitterDate))
            {
                return processedDate;
            }
            if (twitterDate.Length != 30)   //Check if it is in incorrect format
            {
                return String.Empty;
            }            
            try
            {
                String format = "ddd dd MMM yyyy h:mm tt zzz";
                CultureInfo provider = CultureInfo.InvariantCulture;
                StringBuilder dateStringBuilder = new StringBuilder();
                var dateWithUTC = twitterDate.Substring(4);
                String dayOfWeek = twitterDate.Substring(0,3);
                String month = twitterDate.Substring(4, 3);
                String day = twitterDate.Substring(8, 2);
                String year = twitterDate.Substring(26);
                String time = twitterDate.Substring(11, 8);
                String hours = twitterDate.Substring(11, 2);
                String minutes = twitterDate.Substring(14, 2);
                String timeZone = twitterDate.Substring(20, 5);     //timeZone = +0000. C# requires +00:00. Since by default the timezone is UTC, using it as String
                String militaryTime = hours + ":" + minutes;
                String standardTime = DateTime.Parse(militaryTime).ToString(@"h\:mm\ tt");  //convert military time to standard time because DateTime.ParseExact requires standard time                  
                stringDate = dayOfWeek + " " + day + " " + month + " " + year + " " + standardTime + " " + Constant.DEFAULT_TIME_ZONE;
                date = DateTime.ParseExact(stringDate, format, provider);
                processedDate = date.ToLocalTime().ToString();
                return processedDate;
            }
            catch (FormatException)
            {
                Console.WriteLine("{0} is not in correct format", stringDate);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return processedDate;
        }
    }
}