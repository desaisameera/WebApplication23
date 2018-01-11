using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class TweetViewModel
    {
        public String UserName { get; set; }
        public String UserScreenName { get; set; }
        public String UserProfileImageURL { get; set; }
        public String TweetContent { get; set; }
        public int NumberOfReTweets { get; set; }
        public String TweetDate { get; set; }   //Switched from DateTime to String. Giving me error while convert the twitter data to DateTime
        public String MediaURL { get; set; }
    }
}