var sstime = sstime || (function () {
    var sDatetime;
    var tout;
    var tintervalGetServertime;
    var host = location.host === "localhost:44332" ? "" : "https://tserver1.open24.vn";
    var LastUpdate;
    var getDatetimeServer = function () {
        var timeresult;
        try {
            $.ajax({
                url: host + "/api/time/GetDateTime",
                type: "GET",
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (time) {
                    timeresult = new Date(time);
                },
                async: false,
                
            }).fail(function () {
                console.log("Localtime");
                timeresult = new Date();
            });
        }
        catch (err)
        {
            console.log(err.message);
            timeresult = new Date();
        }
        LastUpdate = new Date();
        return timeresult;
    };
    var TimeoutSetDateTime = function () {
        clearInterval(tout);
        sDatetime = getDatetimeServer();
        tout = setInterval(function () {
            sDatetime.setSeconds(sDatetime.getSeconds() + 1);
        }, 1000);
    };
    var getDatetime = function () {
        return sDatetime;
    };
    TimeoutSetDateTime();

    $(window).focus(function () {
        TimeoutSetDateTime();
    });

    return {
        GetDateTimeServer: getDatetimeServer,
        GetDatetime: getDatetime
    }
})();