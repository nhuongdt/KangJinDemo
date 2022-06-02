var View = function () {
    var self = this;
    self.error = ko.observable();
    //list bài viết
    self.listNewsHome = ko.observableArray();
    self.customerSlider = ko.observableArray();
    //Trang tin tức----------------------------------------------------------------------------------------------
    function getAllArticle() {
        //danh sách bài viết trang chủ
        ajaxHelper('/Open24Api/PostAPI/' + "GetAllArticleNewsHome", 'GET').done(function (data) {
            self.listNewsHome(data);
            //console.log(data);
            //console.log($("#").attr('id'));
        });
    }
    getAllArticle();
    //===============================
    // click xem thên tin tức trang chủ
    //===============================
    self.nextNews = function ()
    {
        localStorage.setItem("LoadScroll", "1");
        location.href = '/tin-tuc';
    }

    //===============================
    // Load slider 
    //===============================
    function GetValueCustomer() {
        $.ajax({
            url: '/Open24Api/ApiCustomer/GetShowSlider',
            type: 'GET',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                if (result.res === true) {
                    self.customerSlider(result.DataSoure);
                    jssor_1_slider_init();
                }
                else {
                    alert(result.mess);
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
        async: false,
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

    /*#region responsive code begin*/

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
    /*#endregion responsive code end*/
};