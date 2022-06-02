var Homeknocout = function () {
    var self = this;
    //===============================
    // Khai báo chung
    //===============================
    self.CComent = ko.observable();
    self.CStore = ko.observable();
    self.CNews = ko.observable();
    self.CNumberAccess = ko.observable();
    self.CUserOnline = ko.observable();
    self.ListCountry = ko.observableArray();
    self.LisstPageView = ko.observableArray();
    //self.listNam = ko.observableArray();
    //self.listQuy = ko.observableArray();
    //self.listThang = ko.observableArray();
    self.Type = ko.observable(null);

    self.PageItem = ko.observable();
    self.pageCount = ko.observable();
    self.page = ko.observable();

    //===============================
    // Phân trang 
    //===============================
    self.ClickPrevious = function () {
        if (self.page() > 1) {
            var object = {
                Page: self.page() - 1, 
                Limit: $('#SelectedLimit').val()
            };

            FilterGrid(object);
        }
    }

    self.ClickNext = function () {
        if (self.page() < self.pageCount()) {
            var object = {
                Page: self.page() + 1,
                Limit: $('#SelectedLimit').val()
            };
            FilterGrid(object);
        }
    }

    self.netPageKeyup = function (d, e) {
        if (e.keyCode === 13) {
            if (self.page() > self.pageCount()
                || self.pageCount() === 1
                || !$.isNumeric(self.page())) {
                self.page(1);
            }
            var object = {
                Page: self.page(),
                Limit: $('#SelectedLimit').val()
            };
            FilterGrid(object);

        }
    }
 $('#SelectedLimit').on('change', function () {
        self.page(1);
        var object = {
            Page: self.page(),
            Limit: $('#SelectedLimit').val()
        };
        FilterGrid(object);
    });
    //===============================
    // Tìm kiếm gridview chung
    //===============================
    function FilterGrid(model) {
        $.ajax({
            url: '/Open24Api/ApiHome/SearchPageView',
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: model,
            success: function (result) {
                if (result.res === true) {
                    self.LisstPageView(result.DataSoure.Data);
                    self.PageItem(result.DataSoure.PageItem);
                    self.pageCount(result.DataSoure.PageCount);
                    self.page(result.DataSoure.Page);
                }
                else {
                    alert(result.mess);
                }
            },
            error: function () {
                alert("Đã xảy ra lỗi.");
            }
        });
    }

    //===============================
    // Load dữ liệu trong admin hiện thị biểu đồ
    //===============================
    function GetGroupUser() {
        var url = '/Open24Api/ApiHome/GetCount?filter=' + self.Type() +
            '&tungay=' + $('#datetimepicker6').find("input").val() +
            '&denngay=' + $('#datetimepicker7').find("input").val();
        $.getJSON(url, function (result) {
            if (result.res === true) {
                self.CStore(result.DataSoure.countStore);
                self.CNews(result.DataSoure.countNews);
            }
            else {
                console.log(result.mess);
            }

        });

        
    }
    GetGroupUser();
    function loadUserOnline() {
        var url = '/Open24Api/ApiHome/Loadsonguoitruycap?filter=' + self.Type() +
            '&tungay=' + $('#datetimepicker6').find("input").val() +
            '&denngay=' + $('#datetimepicker7').find("input").val();
        $.getJSON(url, function (result) {
            if (result.res === true) {
                self.CNumberAccess(result.DataSoure);
            }
            else {
                console.log(result.mess);
            }

        });

    }
    loadUserOnline();
    //===============================
    // Load số người onl
    //===============================
    function loadsonguoiOnl() {
        $.getJSON("/Open24Api/ApiBase/getSonguoionl", function (data) {
            self.CUserOnline(data);
        });
        setTimeout(loadsonguoiOnl, 250000);
    }
    loadsonguoiOnl();

    //===============================
    // Load page view
    //===============================
    function loadPage() {
        $.getJSON("/Open24Api/ApiHome/GetPageView", function (result) {
            if (result.res === true) {
                self.LisstPageView(result.DataSoure.Data);
                self.PageItem(result.DataSoure.PageItem);
                self.pageCount(result.DataSoure.PageCount);
                self.page(result.DataSoure.Page);
            }
            else {
                console.log(result.mess);
            }

        });
        setTimeout(loadPage, 800000);
    }
    loadPage();

     //===============================
    // Load List Country
    //===============================
    function loadCountryName() {
        $.getJSON("/Open24Api/ApiHome/loadCountryName", function (result) {
            if (result.res === true) {
                self.ListCountry(result.DataSoure);
            }
            else {
                console.log(result.mess);
            }

        });
    }
    loadCountryName();
    //===============================
    // Cập nhật lại thống kê
    //===============================
    self.capnhat = function () {
        self.Type($('#selected').val());
        loadUserOnline();
        GetGroupUser();
    }
    return self;
}
var knocoutHome = new Homeknocout();
ko.applyBindings(knocoutHome);

//===============================
// Hiện thị Datetime
//===============================
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
//===============================
// Hiện thị biểu đồ thống kê
//===============================
window.onload = function () {
    //===============================
    // Biểu đồ lượng người dùng truy cập gần đây nhât
    //===============================
    var dataPoints = [];
    var options = {
        animationEnabled: true,
        theme: "light2",
        axisX: {
            labelFontSize: 13,
            labelFontColor: "rgb(2, 37, 72)", labelFontWeight: "bold"
        },
        axisY: {
            labelFontSize: 13,
            includeZero: false, labelFontColor: "rgb(2, 37, 72)"
        },
        data: [{
            toolTipContent: "<span style='color:red'> {label}</span><br/>lượt truy cập:<span style='color:red'> {y} </span>",
            markerBorderColor: "red",
            markerBorderThickness: 1,
            type: "area",
            color: "#014D65",
            dataPoints: dataPoints
        }]
    };
    function addData(data) {
        if (data.res === true) {
            dataPoints.splice(0, dataPoints.length);
            Array.prototype.push.apply(dataPoints, data.DataSoure);
            $("#chartContainer").CanvasJSChart(options);
        }
        else {
            AlertError(data.mess);
        }
    }
    $.getJSON('/Open24Api/ApiHome/LoadChartVisit?type=' + $('#selectedType').val() +
                '&filter=' + $('#selectedFilter').val() +
                '&countryName=' + $('#selectedCountryName').val() +
                '&tungay=' + $('#datetimepicker1').find("input").val() +
                '&denngay=' + $('#datetimepicker2').find("input").val(), addData);
    $('#buttonUser').click(function () {
        var url = '/Open24Api/ApiHome/LoadChartVisit?type=' + $('#selectedType').val() +
            '&filter=' + $('#selectedFilter').val() +
            '&countryName=' + $('#selectedCountryName').val() +
            '&tungay=' + $('#datetimepicker1').find("input").val() +
            '&denngay=' + $('#datetimepicker2').find("input").val();
        $.getJSON(url, addData);
    });

    //===============================
    // Số cửa hàng đăng ký gần đây nhât
    //===============================
    var dataStore = [];
    var optionStore = {
        animationEnabled: true,
        theme: "light2",
        axisX: {
            labelFontSize: 13,
            labelFontColor: "rgb(2, 37, 72)",
            labelFontWeight: "bold"
        },
        axisY: {
            labelFontSize: 13,
            includeZero: false, labelFontColor: "rgb(2, 37, 72)"
        },
        data: [{
            toolTipContent: "Ngày:<span style='color:red'> {label}</span><br/>lượt đăng ký:<span style='color:red'> {y} </span>",
            markerBorderColor: "red",
            markerBorderThickness: 1,
            type: "area",
            color: "#014D65",
            dataPoints: dataStore
        }]
    };
    $.getJSON("/Open24Api/ApiStoreRegistration/LoadChartStore", function (data) {
        Array.prototype.push.apply(dataStore, data);
        $("#chartStore").CanvasJSChart(optionStore);
    });

    //===============================
    //Mật độ đăng ký cửa hàng tại các tỉnh thành
    //===============================
    var dataCity = [];
    var optionCity = {
        animationEnabled: true,
        theme: "light2",
        axisX: {
            labelFontSize: 13,
            labelFontColor: "rgb(2, 37, 72)",
            labelFontWeight: "bold"
        },
        axisY: {
            labelFontSize: 13,
            includeZero: false,
            labelFontColor: "rgb(2, 37, 72)"
        },
        data: [{
            toolTipContent: "Tỉnh / Thành phố:<span style='color:red'> {label}</span><br/>lượt đăng ký:<span style='color:red'> {y} </span>",
            markerBorderColor: "red",
            markerBorderThickness: 3,
            type: "column",
            dataPoints: dataCity
        }]
    };
    $.getJSON("/Open24Api/ApiStoreRegistration/LoadChartStoreCity", function (data) {
        Array.prototype.push.apply(dataCity, data);
        $("#chartStoreSCity").CanvasJSChart(optionCity);
    });

    //===============================
    //tỷ lệ các thiết bị truy cập vào web
    //===============================
    var dataSystem = [];
    var optionSystem = {
        animationEnabled: true,
        theme: "light2",
        data: [{
            type: "pie",
            startAngle: 25,
            toolTipContent: "{label}: <span style='color:red'>{y}%</span>",
            showInLegend: "true",
            legendText: "{label}",
            indexLabelFontSize: 16,
            indexLabel: "{label} - {y}%",
            dataPoints: dataSystem
        }]
    };
    $.getJSON("/Open24Api/ApiHome/LoadChartSystem", function (data) {
        if (data.res === true) {
            Array.prototype.push.apply(dataSystem, data.DataSoure);
            $("#chartSystem").CanvasJSChart(optionSystem);
        } else {
            console.log(data.mess);
        }
    });

    //===============================
    //tỷ lệ các thiết bị truy cập vào web
    //===============================
    var dataDevice = [];
    var optionDevice = {
        animationEnabled: true,
        theme: "light2",
        data: [{
            type: "pie",
            startAngle: 25,
            toolTipContent: "{label}: <span style='color:red'>{y}%</span>",
            showInLegend: "true",
            legendText: "{label}",
            indexLabelFontSize: 16,
            indexLabel: "{label} - {y}%",
            dataPoints: dataDevice
        }]
    };
    $.getJSON("/Open24Api/ApiHome/LoadChartDevice", function (data) {
        if (data.res === true) {
            Array.prototype.push.apply(dataDevice, data.DataSoure);
            $("#chartDevice").CanvasJSChart(optionDevice);
        } else {
            console.log(data.mess);
        }
    });

    //===============================
    //Lượng người truy cập tại các quốc gia
    //===============================
    var dataCountry = [];
    var optionCountry = {

        animationEnabled: true,
        axisX: {
            interval: 1,
            labelFontSize: 13,

        },
        axisY2: {
            labelFormatter: function (e) {
                return e.value+"%";
            }, 
            labelFontSize: 13,
            interlacedColor: "rgba(1,77,101,.2)",
            gridColor: "rgba(1,77,101,.1)",
        },
        data: [{
            type: "bar",
            name: "companies",
            toolTipContent: "{label}: <span style='color:red'>{y}%</span>",
            axisYType: "secondary",
            color: "#014D65",
            dataPoints: dataCountry
        }]
    };
    $.getJSON("/Open24Api/ApiHome/LoadChartCountry", function (data) {
        if (data.res === true) {
            Array.prototype.push.apply(dataCountry, data.DataSoure);
            $("#chartCountry").CanvasJSChart(optionCountry);
        } else {
            console.log(data.mess);
        }
    });

    //===============================
    //Lượng người truy cập tại các tỉnh VN
    //===============================
    var dataCityVn = [];
    var optionCityVn = {

        animationEnabled: true,
        axisX: {
            interval: 1,
            labelFontSize: 13,

        },
        axisY2: {
            labelFormatter: function (e) {
                return e.value + "%";
            },
            labelFontSize: 13,
            interlacedColor: "rgba(1,77,101,.2)",
            gridColor: "rgba(1,77,101,.1)",
        },
        data: [{
            type: "bar",
            name: "companies",
            toolTipContent: "{label}: <span style='color:red'>{y}%</span>",
            axisYType: "secondary",
            color: "#014D65",
            dataPoints: dataCityVn
        }]
    };
    $.getJSON("/Open24Api/ApiHome/LoadChartCityVn", function (data) {
        if (data.res === true) {
            Array.prototype.push.apply(dataCityVn, data.DataSoure);
            $("#chartCityVn").CanvasJSChart(optionCityVn);
        } else {
            console.log(data.mess);
        }
    });

}