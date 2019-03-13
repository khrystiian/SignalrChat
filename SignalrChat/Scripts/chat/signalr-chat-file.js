(function () {
    var hub = $.connection.chatHub;  //REFACTORING REQUIRED JAVASCRIPT :(

    //start connection
    $.connection.hub.start(function () {
        $("#send").click(function () {
            var peerUsr = document.getElementById('peer').innerHTML;
            hub.server.send(peerUsr, $('#message-to-send').val());
        });

        //Populate online users list and get the message for the selected user
        hub.server.getConnectedUsersWithMessages().done(function (users) {

            var profile = document.getElementById('profile').innerHTML;
            for (var user of users) {
                $('#chat-list').append('<a href="/Home/Chat?Name=' + user.Name + '"><li class= "clearfix">'
                    + '<img src='
                    + user.Base64Image
                    + ' style="border-radius:50%;margin-top:8px" width="45" height="45" />'
                    + '<div class="about">'
                    + '<div class="name" style="color:white">' + user.Name + '</div>'
                    + '<div class="status">'
                    + '<i class="fa fa-circle online"></i> online</div></div></li></a>');
                for (var msg of user.Messages) {
                    if (msg.FromUsername === msg.ToUsername && msg.FromUsername === profile) {

                        //requests
                        var template = Handlebars.compile($("#message-template").html());
                            var context = {
                                messageOutput: msg.Msg,
                                time: msg.DateTime
                            };
                        $('.chat-history').find('ul').append(template(context));
                        scrollToButtom();
                        
                        // responses
                        var templateResponse = Handlebars.compile($("#message-response-template").html());
                            var contextResponse = {
                                response: msg.Msg,
                                time: msg.DateTime,
                                partner: msg.FromUsername
                            };
                        $('.chat-history').find('ul').append(templateResponse(contextResponse));


                                scrollToButtom();                            
                    }
                    else if (msg.FromUsername === profile && msg.ToUsername !== profile) {
                        //requests
                        var template1 = Handlebars.compile($("#message-template").html());
                        var context1 = {
                            messageOutput: msg.Msg,
                            time: msg.DateTime
                        };
                        $('.chat-history').find('ul').append(template1(context1));
                        scrollToButtom();
                    }
                    else if (msg.FromUsername !== profile && msg.ToUsername === profile) {
                        // responses
                        var templateResponse2 = Handlebars.compile($("#message-response-template").html());
                        var contextResponse2 = {
                            response: msg.Msg,
                            time: msg.DateTime,
                            partner: msg.FromUsername
                        };
                        $('.chat-history').find('ul').append(templateResponse2(contextResponse2));
                        scrollToButtom();      
                    }
                }
            }
        });
    });

    //Receive messages
    hub.client.message = function (caller, msg) {
        scrollToButtom();
        var datetime = new Date().toLocaleString();
        if (msg.trim() !== "") {

            //requests
            var template = Handlebars.compile($("#message-template").html());
            if (caller.indexOf("You") !== -1) {
                var context = {
                    messageOutput: msg,
                    time: datetime
                };
                $('.chat-history').find('ul').append(template(context));
            }
            scrollToButtom();
            $('#message-to-send').val(" ");


            // responses
            var templateResponse = Handlebars.compile($("#message-response-template").html());
            if (caller.indexOf("You") === -1) {
                var contextResponse = {
                    response: msg,
                    time: datetime,
                    partner: caller
                };
                var peer = document.getElementById('peer');
                peer.innerHTML = caller;

                //Short timeout before append the message
                setTimeout(function () {
                    $('.chat-history').find('ul').append(templateResponse(contextResponse));
                    scrollToButtom();
                }.bind(this), 500);
            }


        }
    };

    //Press ENTER key to send messages
    $("#message-to-send").keypress(function (e) {
        if (e.which === 13) {
            $('#send').click();
        }
    });

    function scrollToButtom() {
        $('.chat-history').scrollTop($('.chat-history')[0].scrollHeight);
    }

})();

