var Open24SignalR = (function () {
    'use strict';
    var ConnectionInfo = {
        Username: "",
        Subdomain: "",
        DeviceId: "",
        ConnectionId: "",
    };
    //Connection state: Connected, Reconnecting
    /*var hostUrl = "https://localhost:44385/";*/
    var hostUrl = "https://signalr.open24.vn/";
    var urlIcon = "";
    var callbackfunction = function (state) { };
    var connection = new signalR.HubConnectionBuilder().withUrl(hostUrl + "chatHub").withAutomaticReconnect({
        nextRetryDelayInMilliseconds: retryContext => {
            if (retryContext.elapsedMilliseconds < 60000) {
                // If we've been reconnecting for less than 60 seconds so far,
                // wait between 0 and 10 seconds before the next reconnect attempt.
                return Math.random() * 10000;
            } else {
                // If we've been reconnecting for more than 60 seconds so far, stop reconnecting.
                return null;
            }
        }
    }).build();
    var CreateConnection = function (option, callback) {
        ConnectionInfo.DeviceId = getDeviceId();
        if (option !== undefined) {
            callbackfunction = callback;
            ConnectionInfo.Username = option.Username === undefined ? "" : option.Username;
            ConnectionInfo.Subdomain = option.Subdomain === undefined ? "" : option.Subdomain;
            urlIcon = option.UrlIcon === undefined ? "" : option.UrlIcon;
            if (option.HostUrl !== undefined) {
                hostUrl = option.HostUrl;
            }
            if (ConnectionInfo.Username !== "" && ConnectionInfo.Subdomain !== "" && ConnectionInfo.DeviceId !== "") {
                StartSignalRConnection();
            }
            else {
                console.log("Create fail");
            }
        }
        else {
            console.log("Create fail");
        }
    }

    function StartSignalRConnection() {
        connection.start().then(function () {
            console.log("connected");
            ConnectionInfo.ConnectionId = connection.connectionId;
            UpdateConnectionInfo();
        }).catch(function (err) {
            console.error(err.toString());
            return false;
        });
    }

    connection.on("MessageWithTitleBody", function (message) {
        SignalRNotify(message.title, urlIcon, message.body);
    });

    connection.onreconnecting(error => {
        callbackfunction(connection.state);
        console.assert(connection.state === signalR.HubConnectionState.Reconnecting);
    });

    connection.onreconnected(connectionId => {
        ConnectionInfo.ConnectionId = connection.connectionId;
        UpdateConnectionInfo();
    });
    function UpdateConnectionInfo() {
        $.ajax({
            url: hostUrl + "api/Connection/UpdateConectionInfo",
            type: 'POST',
            dataType: 'json',
            data: JSON.stringify(ConnectionInfo),
            contentType: "application/json; charset=UTF-8",
            success: function (data) {
            }
        });
        callbackfunction(connection.state);
    }

    function uuidv4() {
        return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
            (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
        );
    };

    function getCookie(cname) {
        let name = cname + "=";
        let decodedCookie = decodeURIComponent(document.cookie);
        let ca = decodedCookie.split(';');
        for (let i = 0; i < ca.length; i++) {
            let c = ca[i];
            while (c.charAt(0) == ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) == 0) {
                return c.substring(name.length, c.length);
            }
        }
        return "";
    };

    function setCookie(cname, cvalue, exdays) {
        const d = new Date();
        d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
        let expires = "expires=" + d.toUTCString();
        document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
    };


    function getDeviceId() {
        let deviceId = getCookie("deviceId");
        if (deviceId === "") {
            deviceId = uuidv4();
        }
        setCookie("deviceId", deviceId, 7);
        return deviceId;
    };

    function SignalRNotify(title, urlIcon, body) {
        var option = {
            icon: urlIcon,
            body: body
        };
        // Let's check if the browser supports notifications
        if (!("Notification" in window)) {
            alert("This browser does not support desktop notification");
        }
        // Let's check whether notification permissions have already been granted
        else if (Notification.permission === "granted") {
            // If it's okay let's create a notification
            var notification = new Notification(title, option);
        }

        // Otherwise, we need to ask the user for permission
        else if (Notification.permission !== "denied") {
            Notification.requestPermission().then(function (permission) {
                // If the user accepts, let's create a notification
                if (permission === "granted") {
                    var notification = new Notification(title, option);
                }
            });
        }
    };

    var ConnectionState = function () {
        return connection.state;
    }

    return {
        CreateConnection: CreateConnection,
        ConnectionState: ConnectionState,
        Connection: connection
    };
})();