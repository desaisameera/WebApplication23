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
                    var tweetData = data.data;
                    var profilePicuteURL = tweetData[0].UserProfileImageURL;
                    $(".user-image").attr("src", profilePicuteURL); //set all profile pictures
                    $.each(tweetData, function (index, value) {
                        //debugger;
                        var number = index + 1;
                        $("#tweet-content-"+number).html(value.TweetContent);
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
            //notifier.logixNotifier('notifyError', xhr.status + ", " + textStatus + ", " + thrownError);
        }
    });
});