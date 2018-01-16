/////*https://stackoverflow.com/questions/4220126/run-javascript-function-when-user-finishes-typing-instead-of-on-key-up*/
    var typingTimer;                //timer identifier
    var waitTime = 1000;  //time in ms, 1 second for example
    var $input = $('#search-bar');
    
    $input.on('keyup', function () {
        clearTimeout(typingTimer);
        typingTimer = setTimeout(searchTweets, waitTime);
    });
    //on keydown, clear the countdown 
    $input.on('keydown', function () {
        clearTimeout(typingTimer);
    });
    function searchTweets() {
        for (var index = 1; index <= 10; index++) {
            $('#list-' + index).hide();
            $('#separator-'+index).hide();
        }
        var searchText = $('#search-bar').val();
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
                        var filteredTweetIndices = data.data;
                        var profilePicuteURL = tweetData[0].UserProfileImageURL;
                        $(".user-image").attr("src", profilePicuteURL); //set all profile pictures
                        $.each(filteredTweetIndices, function (index, value) {
                            var number = value + 1;
                            $("#list-" + number).show();
                            $('#separator-' + number).show();
                        });
                    }
                },
                error: function (xhr, textStatus, thrownError) {
                    alert(xhr.responseText);
                }
            });
        }
        else {
            for (var index = 1; index <= 10; index++) {
                $("#list-" + index).show();
                $('#separator-' + index).show();
            }
        }

    }