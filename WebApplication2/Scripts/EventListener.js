/////*https://stackoverflow.com/questions/4220126/run-javascript-function-when-user-finishes-typing-instead-of-on-key-up*/
    var typingTimer;                //timer identifier
    var waitTime = 2000;  //time in ms, 2 second for example
    var $input = $('#search-bar');
    
    $input.on('keyup', function () {
        clearTimeout(typingTimer);
        typingTimer = setTimeout(searchTweets, waitTime);
    });
    //on keydown, clear the countdown 
    $input.on('keydown', function () {
        //debugger;
        clearTimeout(typingTimer);
    });
    function searchTweets() {
        //$('#tweet-content-div').hide();
        for (var index = 1; index <= 10; index++) {
            $("#list-" + index).hide();
        }
        var searchText = $('#search-bar').val();
        debugger;
        if (searchText) {
            //call the method in controller
            $.ajax({
                url: "/Home/SearchTweets",
                data: {
                    searchText: searchText,
                    tweetContent: tweetArray
                },
                dataType: "json",
                type: "POST",
                success: function (data) {
                    if (!data.success) {
                        alert(data.message);
                    }
                    else {
                        debugger;
                        var filteredTweetIndices = data.data;
                        var profilePicuteURL = tweetData[0].UserProfileImageURL;
                        $(".user-image").attr("src", profilePicuteURL); //set all profile pictures
                        $.each(filteredTweetIndices, function (index, value){
                            debugger;
                            var number = value + 1;
                            $("#list-" + number).show();
                            //var number = value + 1;
                            //$("#tweet-content-" + number).html(tweetData[value].TweetContent);
                            //$("#user-name-" + number).text(tweetData[value].UserName);
                            //$("#user-screen-name-" + number).text(tweetData[value].UserScreenName);
                            //$("#retweet-" + number).text(tweetData[value].NumberOfReTweets);
                            //$("#tweet-date-" + number).text(tweetData[value].TweetDate);
                            //if (tweetData[value].MediaURL) {
                            //    debugger;
                            //    var x = $("#iframe-" + number);
                            //    $("#media-" + number).attr("src", value.MediaURL);
                            //    $("#media-" + number).show();
                            //}
                        });
                    }
                    //$('#tweet-content-div').show();
                },
                error: function (xhr, textStatus, thrownError) {
                    alert("Something went wrong");
                }
            });
        }
        else {
            $('#tweet-content-div').show();
            alert("Please enter search text");
        }

    }