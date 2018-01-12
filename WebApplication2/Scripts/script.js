var tweetArray = new Array;
$(document).ready(function () {
    // all custom jQuery will go here
        $.ajax({
            url: "/Home/GetTweets",
            data: {
                username: "salesforce",
                n:10,
            },
            dataType: "json",
            type: "GET",
            success: function (data) {
                if (!data.success) {
                    alert(data.message);
                }
                else {
                    tweetData = data.data;  //make it global
                    var profilePicuteURL = tweetData[0].UserProfileImageURL;
                    $(".user-image").attr("src", profilePicuteURL); //set all profile pictures
                    $.each(tweetData, function (index, value) {
                        var number = index + 1;
                        $("#tweet-content-" + number).html(value.TweetContent);
                        tweetArray.push(value.TweetContent);
                        $("#user-name-"+number).text(value.UserName);
                        $("#user-screen-name-"+number).text(value.UserScreenName);
                        $("#retweet-"+number).text(value.NumberOfReTweets);
                        $("#tweet-date-" + number).text(value.TweetDate);
                        if (value.MediaURL) {
                            debugger;
                            var x = $("#iframe-" + number);
                            $("#media-" + number).attr("src", value.MediaURL);
                            $("#media-" + number).show();
                        }
                    });
            }
        },
        error: function (xhr, textStatus, thrownError) {
            alert("Something went wrong");
        }
        });

    /*Code for search box*/
    /////*https://stackoverflow.com/questions/4220126/run-javascript-function-when-user-finishes-typing-instead-of-on-key-up*/

        
    //    var typingTimer;                //timer identifier
    //    var waitTime = 2000;  //time in ms, 5 second for example
    //    var $input = $('#search-bar');
    //    debugger;
    ////on keyup, start the countdown
    //    $input.on('keyup', function () {
    //        debugger;
    //        clearTimeout(typingTimer);
    //        typingTimer = setTimeout(searchTweets, waitTime);
    //    });

    ////on keydown, clear the countdown 
    //    $input.on('keydown', function () {
    //        debugger;
    //        clearTimeout(typingTimer);
    //    });
        
    //    function searchTweets() {
    //        alert("In search Tweets");
    //    }
});