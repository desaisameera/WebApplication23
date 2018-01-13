var tweetArray = new Array;
$(document).ready(function () {
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
});