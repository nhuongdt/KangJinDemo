var View = function () {
    var self = this;
    self.error = ko.observable();
    self.listNewsHome = ko.observableArray();
    self.customerSlider = ko.observableArray();
    function getAllArticle() {
        ajaxHelper('/Open24Api/PostAPI/' + "GetAllArticleNewsHome", 'GET').done(function (data) {
            self.listNewsHome(data);
        });
    }
    getAllArticle();
    self.nextNews = function () {
        localStorage.setItem("LoadScroll", "1");
        location.href = '/tin-tuc';
    }
    function GetValueCustomer() {
        $.ajax({
            url: '/Open24Api/ApiCustomer/GetShowSlider',
            type: 'GET',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                if (result.res === true) {
                    self.customerSlider(result.DataSoure);
                    jssor_1_slider_init();
                }
                else {
                    console.log(result.mess);
                }
            },
            error: function (result) {
                exception(result);
            }
        });
    }
    GetValueCustomer();

    self.showdangky = function () {
        $('#modalPopup_CHDK').modal('show');
    }
}
ko.applyBindings(new View());

function ajaxHelper(uri, method, data) {
    return $.ajax({
        type: method,
        url: uri,
        dataType: 'json',
        contentType: 'application/json',
        data: data ? JSON.stringify(data) : null,
        statusCode: {
            404: function () {
                console.log("Page not found");
            },
        }
    })
        .fail(function (jqXHR, textStatus, errorThrown) {
            console.log(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
        });
}
function ConvertDate(config) {
    if (config === undefined
        || config === null
        || config.replace(/\s+/g, '') === "") {
        return "";
    }
    else {
        var a = moment(config).format('DD/MM/YYYY');
        return a;
    }
}
jssor_1_slider_init = function () {

    var jssor_1_options = {
        $AutoPlay: 1,
        $AutoPlaySteps: 5,
        $SlideDuration: 160,
        $SlideWidth: 200,
        $SlideSpacing: 3,
        $Cols: 5,
        $Align: 390,
        $ArrowNavigatorOptions: {
            $Class: $JssorArrowNavigator$,
            $Steps: 5
        },
        $BulletNavigatorOptions: {
            $Class: $JssorBulletNavigator$
        }
    };

    var jssor_1_slider = new $JssorSlider$("jssor_1", jssor_1_options);
    var MAX_WIDTH = 980;

    function ScaleSlider() {
        var containerElement = jssor_1_slider.$Elmt.parentNode;
        var containerWidth = containerElement.clientWidth;

        if (containerWidth) {

            var expectedWidth = Math.min(MAX_WIDTH || containerWidth, containerWidth);

            jssor_1_slider.$ScaleWidth(expectedWidth);
        }
        else {
            window.setTimeout(ScaleSlider, 30);
        }
    }

    ScaleSlider();

    $Jssor$.$AddEvent(window, "load", ScaleSlider);
    $Jssor$.$AddEvent(window, "resize", ScaleSlider);
    $Jssor$.$AddEvent(window, "orientationchange", ScaleSlider);
};
var reSrc = 'https://www.youtube.com/embed/_EZwS3JZIfo?rel=0&controls=0&autoplay=1';
$('body').on('hidden.bs.modal', '.modal', function () {
    $('#myVideo').attr("src", '');
});
function changeVideo() {
    $('#myVideo').each(function () {
        var frame = document.getElementById("myVideo");
        frame.contentWindow.postMessage(
            '{"event":"command","func":"playVideo","args":""}',
            '*');
    });
    $('#myVideo').attr("src", reSrc);
    $("#myModal").modal("show");
}
function SubStringContent(input, lenth = 100) {

    if (input !== null && input.length > lenth) {
        return input.substring(0, lenth) + "...";

    }
    return input;
};
//$(function () {
//    $('.slider-banner-web').slideshow({
//        interval: 8000
//    });
//    $('.btn-baner-1').show();
//    $('.slider-banner-mobile').slideshow({
//        interval: 8000
//    });
//});
