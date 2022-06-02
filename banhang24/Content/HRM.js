
//thành phần nhân viên
Highcharts.chart('tpnv', {
    chart: {
        plotBackgroundColor: null, plotBorderWidth: null, plotShadow: false, type: 'pie'
    }
    , title: '', tooltip: {
        pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
    }
    , accessibility: {
        point: {
            valueSuffix: '%'
        }
    }
    , plotOptions: {
        pie: {
            allowPointSelect: true, cursor: 'pointer', dataLabels: {
                enabled: true, format: '<b>{point.name}</b>: {point.percentage:.1f} %'
            }
        }
    }
    , series: [{
        name: 'Brands', colorByPoint: true, data: [{
            name: 'Kĩ thuật', y: 25, sliced: true, selected: true
        }
            , {
            name: 'Kinh doanh', y: 60
        }
            , {
            name: 'Chăm sóc khách hàng', y: 10
        }
            , {
            name: 'Quản trị', y: 5
        }
        ]
    }
    ]
}

);
//Quỹ lương theo tháng
var chart = Highcharts.chart('totalsalary', {
    chart: {
        type: 'column'
    }
    , title: '', xAxis: {
        categories: ['Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6', 'Tháng háng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12']
    }
    , plotOptions: {
        series: {
            allowPointSelect: true
        }
    }
    , series: [ //    {
        //    name: "Tháng trước", color:"var(--color-main)",
        //    data: [29.9, 71.5, 106.4, 129.2, 144.0, 176.0, 135.6, 148.5, 216.4, 194.1, 95.6, 54.4]
        //},
        {
            name: "Chi nhánh A", color: "var(--color-primary)", data: [29, 71, 104, 12, 140, 10, 135, 185, 214, 141, 96, 54]
        }
    ]
}

);
//Biểu đồ nhân viên theo từng chi nhánh
Highcharts.chart('brandchart', {
    chart: {
        type: 'pie'
    }
    , title: {
        text: ''
    }
    , tooltip: {
        headerFormat: '', pointFormat: '<span style="color:{point.color}">\u25CF</span> <b> {point.name}</b><br/>' + 'Area (square km): <b>{point.y}</b><br/>' + 'Population density (people per square km): <b>{point.z}</b><br/>'
    }
    , series: [{
        minPointSize: 10, innerSize: '60%', zMin: 0, name: 'countries', data: [{
            name: 'Spain', y: 505370, z: 92.9
        }
            , {
            name: 'France', y: 551500, z: 118.7
        }
            , {
            name: 'Poland', y: 312685, z: 124.6
        }
            , {
            name: 'Czech Republic', y: 78867, z: 137.5
        }
            , {
            name: 'Italy', y: 301340, z: 201.8
        }
            , {
            name: 'Switzerland', y: 41277, z: 214.5
        }
            , {
            name: 'Germany', y: 357022, z: 235.6
        }
        ]
    }
    ]
}

);
Highcharts.chart('staffchart_1', {
    chart: {
        type: 'bar'
    },
    title: {
        text: ''
    },
    xAxis: {
        categories: ['Apples', 'Oranges', 'Pears', 'Grapes', 'Bananas']
    },
    yAxis: {
        min: 0,
        title: {
            text: ''
        }
    },
    legend: {
        reversed: true
    },
    plotOptions: {
        series: {
            stacking: 'normal'
        }
    },
    series: [{
        name: 'John',
        data: [5, 3, 4, 7, 2]
    }, {
        name: 'Jane',
        data: [2, 2, 3, 2, 1]
    }, {
        name: 'Joe',
        data: [3, 4, 4, 2, 5]
    }]
});

$(".eventonmonth li").on("click", function () {
    if (($(this).find("i").html() === "library_add") || ($(this).hasClass("showmore"))) return;
    //alert($(this).find("i").html());
    {
    $(".eventomonth-detail li").find("i").html($(this).find("i").html());
    $(".eventomonth-detail li").find("label").html($(this).find("label").html());
    $(this).siblings().removeClass("active");
    $(this).addClass("active");
    }

});
$(".showmore").click(function () {
    $(".eventomonth-detail li:nth-child(n+6)").fadeToggle();
    
    $(this).find("i").toggle();

});