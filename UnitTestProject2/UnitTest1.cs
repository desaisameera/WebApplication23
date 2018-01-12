using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Mvc;
using System.Web.Routing;
using WebApplication2.Controllers;

namespace UnitTestProject2
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Search_Valid_Tweet_With_Valid_Input_And_Case_Insensitive()
        {
            String[] tweets = {"Wish granted! New #TDX18 learning bootcamps are a great way to get on the fast track to mastering Salesforce: http://sforce.co/2mtu757 ",
                                "Tweet with your answer: What business book's helped you the most? ",
                                " Evidence suggests jamming on tunes at work can make you more productive. ",
                                "At @Salesforce, we are proud to stand with more than 115 industry leaders urging Congress to immediately pass a permanent, bipartisan legislative solution to protect Dreamers.  #ProtectDreamers",
                                "Trailblazer @CristinaaBran risked everything to find the career she’d been dreaming of. This is her story:… https://t.co/dr0kEpwqbO",
                                "Snazzy new data from @CommerceCloud shows 50% of holiday shoppers were done buying gifts by December 3:… https://t.co/BMJrGNFrGJ",
                                "Salesforce Tower will feature the largest on-site water recycling system in a commercial high-rise building in t… https://t.co/ODbujaFAGO",
                                "@Ryoh_4242 #crushingit",
                                " If you’re looking for inspiration, this thread is all you need. https://t.co/ucepsT2K2L",
                                "Tell us what you think of Salesforce's Twitter presence: http://sforce.co/2qMiIlq "
                              };
            String searchText = "salesforce";
            List<int> expectedResults = new List<int>();
            expectedResults.Add(0);
            expectedResults.Add(3);
            expectedResults.Add(6);
            expectedResults.Add(9);
            var hc = new HomeController();
            JsonResult actualResult = hc.SearchTweets(searchText, tweets) as JsonResult;
            IDictionary<string,object> data = (IDictionary<string,object>)new System.Web.Routing.RouteValueDictionary(actualResult.Data);
            CollectionAssert.AreEqual(expectedResults, data["data"] as ICollection);

        }
        [TestMethod]
        public void Search_With_Empty()
        {
            String[] tweets = {"Wish granted! New #TDX18 learning bootcamps are a great way to get on the fast track to mastering Salesforce: http://sforce.co/2mtu757 ",
                                "Tweet with your answer: What business book's helped you the most? ",
                                " Evidence suggests jamming on tunes at work can make you more productive. ",
                                "At @Salesforce, we are proud to stand with more than 115 industry leaders urging Congress to immediately pass a permanent, bipartisan legislative solution to protect Dreamers.  #ProtectDreamers",
                                "Trailblazer @CristinaaBran risked everything to find the career she’d been dreaming of. This is her story:… https://t.co/dr0kEpwqbO",
                                "Snazzy new data from @CommerceCloud shows 50% of holiday shoppers were done buying gifts by December 3:… https://t.co/BMJrGNFrGJ",
                                "Salesforce Tower will feature the largest on-site water recycling system in a commercial high-rise building in t… https://t.co/ODbujaFAGO",
                                "@Ryoh_4242 #crushingit",
                                " If you’re looking for inspiration, this thread is all you need. https://t.co/ucepsT2K2L",
                                "Tell us what you think of Salesforce's Twitter presence: http://sforce.co/2qMiIlq "
                              };
            String searchText = String.Empty;
            String expectedResult = "Please enter text to search.";
            var hc = new HomeController();
            JsonResult actualResult = hc.SearchTweets(searchText, tweets) as JsonResult;
            IDictionary<string, object> data = (IDictionary<string, object>)new System.Web.Routing.RouteValueDictionary(actualResult.Data);
            Assert.AreEqual(expectedResult, data["message"]);

        }

        [TestMethod]
        public void Search_With_Null()
        {
            String[] tweets = {"Wish granted! New #TDX18 learning bootcamps are a great way to get on the fast track to mastering Salesforce: http://sforce.co/2mtu757 ",
                                "Tweet with your answer: What business book's helped you the most? ",
                                " Evidence suggests jamming on tunes at work can make you more productive. ",
                                "At @Salesforce, we are proud to stand with more than 115 industry leaders urging Congress to immediately pass a permanent, bipartisan legislative solution to protect Dreamers.  #ProtectDreamers",
                                "Trailblazer @CristinaaBran risked everything to find the career she’d been dreaming of. This is her story:… https://t.co/dr0kEpwqbO",
                                "Snazzy new data from @CommerceCloud shows 50% of holiday shoppers were done buying gifts by December 3:… https://t.co/BMJrGNFrGJ",
                                "Salesforce Tower will feature the largest on-site water recycling system in a commercial high-rise building in t… https://t.co/ODbujaFAGO",
                                "@Ryoh_4242 #crushingit",
                                " If you’re looking for inspiration, this thread is all you need. https://t.co/ucepsT2K2L",
                                "Tell us what you think of Salesforce's Twitter presence: http://sforce.co/2qMiIlq "
                              };
            String searchText = null;
            String expectedResult = "Please enter text to search.";
            var hc = new HomeController();
            JsonResult actualResult = hc.SearchTweets(searchText, tweets) as JsonResult;
            IDictionary<string, object> data = (IDictionary<string, object>)new System.Web.Routing.RouteValueDictionary(actualResult.Data);
            Assert.AreEqual(expectedResult, data["message"]);

        }
        [TestMethod]
        public void Search_With_NoResults()
        {
            String[] tweets = {"Wish granted! New #TDX18 learning bootcamps are a great way to get on the fast track to mastering Salesforce: http://sforce.co/2mtu757 ",
                                "Tweet with your answer: What business book's helped you the most? ",
                                " Evidence suggests jamming on tunes at work can make you more productive. ",
                                "At @Salesforce, we are proud to stand with more than 115 industry leaders urging Congress to immediately pass a permanent, bipartisan legislative solution to protect Dreamers.  #ProtectDreamers",
                                "Trailblazer @CristinaaBran risked everything to find the career she’d been dreaming of. This is her story:… https://t.co/dr0kEpwqbO",
                                "Snazzy new data from @CommerceCloud shows 50% of holiday shoppers were done buying gifts by December 3:… https://t.co/BMJrGNFrGJ",
                                "Salesforce Tower will feature the largest on-site water recycling system in a commercial high-rise building in t… https://t.co/ODbujaFAGO",
                                "@Ryoh_4242 #crushingit",
                                " If you’re looking for inspiration, this thread is all you need. https://t.co/ucepsT2K2L",
                                "Tell us what you think of Salesforce's Twitter presence: http://sforce.co/2qMiIlq "
                              };
            String searchText = "sameera";
            List<int> expectedResults = new List<int>();
            var hc = new HomeController();
            JsonResult actualResult = hc.SearchTweets(searchText, tweets) as JsonResult;
            IDictionary<string, object> data = (IDictionary<string, object>)new System.Web.Routing.RouteValueDictionary(actualResult.Data);
            CollectionAssert.AreEqual(expectedResults, data["data"] as ICollection);
        }
        [TestMethod]
        public void Search_With_More_Than_10()
        {
            String[] tweets = {"Wish granted! New #TDX18 learning bootcamps are a great way to get on the fast track to mastering Salesforce: http://sforce.co/2mtu757 ",
                                "Tweet with your answer: What business book's helped you the most? ",
                                " Evidence suggests jamming on tunes at work can make you more productive. ",
                                "At @Salesforce, we are proud to stand with more than 115 industry leaders urging Congress to immediately pass a permanent, bipartisan legislative solution to protect Dreamers.  #ProtectDreamers",
                                "Trailblazer @CristinaaBran risked everything to find the career she’d been dreaming of. This is her story:… https://t.co/dr0kEpwqbO",
                                "Snazzy new data from @CommerceCloud shows 50% of holiday shoppers were done buying gifts by December 3:… https://t.co/BMJrGNFrGJ",
                                "Salesforce Tower will feature the largest on-site water recycling system in a commercial high-rise building in t… https://t.co/ODbujaFAGO",
                                "@Ryoh_4242 #crushingit",
                                " If you’re looking for inspiration, this thread is all you need. https://t.co/ucepsT2K2L",
                                "Tell us what you think of Salesforce's Twitter presence: http://sforce.co/2qMiIlq ",
                                "Wish granted! New #TDX18 learning bootcamps are a great way to get on the fast track to mastering Salesforce: http://sforce.co/2mtu757 ",
                                "Tweet with your answer: What business book's helped you the most? ",
                                " Evidence suggests jamming on tunes at work can make you more productive. ",
                                "At @Salesforce, we are proud to stand with more than 115 industry leaders urging Congress to immediately pass a permanent, bipartisan legislative solution to protect Dreamers.  #ProtectDreamers",
                                "Trailblazer @CristinaaBran risked everything to find the career she’d been dreaming of. This is her story:… https://t.co/dr0kEpwqbO",
                                "Snazzy new data from @CommerceCloud shows 50% of holiday shoppers were done buying gifts by December 3:… https://t.co/BMJrGNFrGJ",
                                "Salesforce Tower will feature the largest on-site water recycling system in a commercial high-rise building in t… https://t.co/ODbujaFAGO",
                                "@Ryoh_4242 #crushingit",
                                " If you’re looking for inspiration, this thread is all you need. https://t.co/ucepsT2K2L",
                                "Tell us what you think of Salesforce's Twitter presence: http://sforce.co/2qMiIlq "
                              };
            String searchText = "salesforce";
            String expectedResults = "There more tweet array contains more than 10 elements.";
            var hc = new HomeController();
            JsonResult actualResult = hc.SearchTweets(searchText, tweets) as JsonResult;
            IDictionary<string, object> data = (IDictionary<string, object>)new System.Web.Routing.RouteValueDictionary(actualResult.Data);
            Assert.AreEqual(expectedResults, data["message"]);
        }

        [TestMethod]
        public void Search_With_No_Tweets()
        {
            String[] tweets = {};
            String searchText = "salesforce";
            String expectedResults = "There are no tweets to search.";
            var hc = new HomeController();
            JsonResult actualResult = hc.SearchTweets(searchText, tweets) as JsonResult;
            IDictionary<string, object> data = (IDictionary<string, object>)new System.Web.Routing.RouteValueDictionary(actualResult.Data);
            Assert.AreEqual(expectedResults, data["message"]);
        }

        [TestMethod]
        public void Search_With_Null_Tweets()
        {
            String[] tweets = null;
            String searchText = "salesforce";
            String expectedResults = "There are no tweets to search.";
            var hc = new HomeController();
            JsonResult actualResult = hc.SearchTweets(searchText, tweets) as JsonResult;
            IDictionary<string, object> data = (IDictionary<string, object>)new System.Web.Routing.RouteValueDictionary(actualResult.Data);
            Assert.AreEqual(expectedResults, data["message"]);
        }
    }
}
