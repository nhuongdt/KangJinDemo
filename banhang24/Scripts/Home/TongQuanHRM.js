function View() {
    var self = this;
    //===============================
    // Khai báo chung
    //===============================
    self.DiaryBH = ko.observableArray();
    self.Title = ko.observable();
    self.UserGirl = ko.observable();
    self.UserBoy = ko.observable();
    self.UserAll = ko.observable();
    self.NameChartLoaiHopDong = ko.observable("Hôm nay");
    self.NameChartPhongBan = ko.observable("Tháng này");
    var APiHRM = '/api/DanhMuc/ApiOpen24HRM/';
    function LoadCChart(date) {
        $.getJSON(APiHRM + "GetNhanSu?donviId=" + $('#hd_IDdDonVi').val()+"&date=" + date, function (data) {
            if (data.res === true) {
                self.UserAll(data.dataSoure.all);
                self.UserBoy(data.dataSoure.boy);
                self.UserGirl(data.dataSoure.girl);
                self.Title(data.dataSoure.title);
            }
        });
    }
    LoadCChart();
    $.getJSON(APiHRM + "GetHoatDongHRM?donviId=" + $('#hd_IDdDonVi').val()  , function (data) {
        if (data.res === true) {
            self.DiaryBH(data.dataSoure);
        }
    });
    $('#datein').dcalendar().on('dateselected', function (e) {
        LoadCChart(e.date);
    });
    //===============================
    // Chart loại hợp đồng
    //===============================
    function loadChartColumn(type) {

        $.getJSON(APiHRM + "GetChartColumnDashBoard?donviId=" + $('#hd_IDdDonVi').val() +"&type=" + type, function (data) {
            if (data.res === true) {
                optionsLoaiHopDong.series[0].data = data.dataSoure;
                Highcharts.chart(optionsLoaiHopDong)
                self.ChartLoaiHopDong(data.dataSoure);
            }
        });
    }
    loadChartColumn();

    $('#ChartLoaiHopDongSI').on('click', 'ul li', function () {
        $(this).closest('ul').toggle();
        self.NameChartLoaiHopDong($(this).find('a').text())
        var id = $(this).data('id');
        loadChartColumn(id);
        $('#ChartLoaiHopDongSI ul li').each(function () {
            $(this).find('a').find('i').remove();
            if (id === $(this).data('id')) {
                $(this).find('a').append('<i class="fa fa-check check-after-litr"></i>');
            }
        });
    });
    self.ChartLoaiHopDong = ko.observableArray();
    var optionsLoaiHopDong = {
        chart: {
            renderTo: 'ChartHopDong',
            type: 'column'
        },
        title: {
            text: ''
        },
        subtitle: {
            text: ''
        },
        xAxis: {
            type: 'category'
        },
        yAxis: {
            title: {
                text: 'Số lượng nhân viên'
            }

        },
        legend: {
            enabled: false
        },
        plotOptions: {
            series: {
                borderWidth: 1,
                dataLabels: {
                    enabled: true,
                    format: '{point.y} người'
                }
            }
        },

        tooltip: {
            headerFormat: '<span style="font-size:11px">{series.name}</span><br>',
            pointFormat: '<span>{point.name}</span>: <b><span style="color:var(--color-main);">{point.y} người</span> </b> <br/>'
        },

        series: [
            {
                name: "Loại hợp đồng",
                colorByPoint: true,
                data: self.ChartLoaiHopDong()
            }
        ]
    };

   //===============================
    // Chart phòng ban
    //===============================
    $('#ChartPhongBanSI').on('click', 'ul li', function () {
        $(this).closest('ul').toggle();
        self.NameChartPhongBan($(this).find('a').text())
        var id = $(this).data('id');
        loadChartLine(id);
        $('#ChartPhongBanSI ul li').each(function () {
            $(this).find('a').find('i').remove();
            if (id === $(this).data('id')) {
                $(this).find('a').append('<i class="fa fa-check check-after-litr"></i>');
            }
        });
    });
    function loadChartLine(type) {

        $.getJSON(APiHRM + "GetChartLineDashBoard?donviId=" + $('#hd_IDdDonVi').val() +"&type=" + type, function (data) {
            if (data.res === true) {
                optionsPhongBan.tooltip.pointFormat = data.dataSoure.pointFormat;
                optionsPhongBan.series = data.dataSoure.data;
                Highcharts.chart(optionsPhongBan)
                self.ChartPhongBan(data.dataSoure);
            }
        });
    }
    loadChartLine();

    self.ChartPhongBan = ko.observableArray();
    var optionsPhongBan= {
        chart: {
            renderTo: 'ChartPhongBan',
            type: 'line'
        }, title: {
            text: ''
        },
        subtitle: {
            text: ''
        },
        yAxis: {
            title: {
                text: ''
            }
        },
        plotOptions: {
            series: {
                pointStart: 1
            }
        },
        tooltip: {
            headerFormat: '<b>Tên phòng: <span style="color:var(--color-main);">{series.name}</span></b><br />',
            pointFormat: '<b>Thời gian: <span style="color:var(--color-main);">{point.x}</span></b><br /> <b>Số nhân viên : <span style="color:var(--color-main);">{point.y}</span></b>'
        },
        series: []
    };
   
};
ko.applyBindings(new View());
