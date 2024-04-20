const connection = new signalR.HubConnectionBuilder()
                        .withUrl("/onlineUsersHub")
                        .configureLogging(signalR.LogLevel.Information)
                        .build();

                    connection.start().then(function () {
                        console.log("Connected to OnlineUsersHub");
                    }).catch(function (err) {
                        return console.error(err.toString());
                    });

                    connection.on("UpdateOnlineUsersCount", function (onlineUsersCount) {
                        console.log("Online users count: " + onlineUsersCount);
                    });