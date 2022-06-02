
$.ajax({
    type: 'GET',
    url: "https://geoip-db.com/json/",
    success: function (data) {
        UpdateUserVisit(JSON.parse(data))
    },
    timeout: 4000,      // 4 seconds
    error: function (qXHR, textStatus, errorThrown) {
        if (textStatus === "timeout") {
            console.log(qXHR);
        }
    }
});

function UpdateUserVisit(data)
{
    $.ajax({
        type: 'GET',
        url: '/Open24Api/AppChat/AppChat?ip=' + data.IPv4 + '&name=' + data.country_name + '&city=' + data.city,
        timeout: 4000,      // 4 seconds
        error: function (qXHR, textStatus, errorThrown) {
            if (textStatus === "timeout") {
                console.log(qXHR);
            }
        }
    });
}
function CheclAppChat() {
    $.getJSON('/Open24Api/AppChat/CheckAppchat').done(function (data) { });
    setTimeout(CheclAppChat, 120000);
}
setTimeout(CheclAppChat, 120000);
 