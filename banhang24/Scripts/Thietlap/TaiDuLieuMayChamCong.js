var urlApiChamCong = '/api/DanhMuc/NS_NhanSuAPI/';
let ArrYear = [];
let dNow = new Date();
let YearNow = dNow.getFullYear();
let MonthNow = dNow.getMonth() + 1;
let YearSelected = YearNow;
for (let i = 1; i <= 5; i++) {
    ArrYear.push(YearNow)
    YearNow = YearNow - 1;
}
var VueMayChamCong = new Vue({
    el: '#vMayChamCong',
    data: {
        lstMayChamCong: [],
        IDMayChamCongSelected: ""
    },
    methods: {
        LoadMayChamCong: function () {
            let self = this;
            if (VueChiNhanh.databind.data.length !== 0) {
                let lstIdChiNhanh = [""];
                let myData = {};
                VueChiNhanh.databind.data.filter(p => p.CNChecked === true).forEach(p => lstIdChiNhanh.push(p.ID));
                myData.IDs = lstIdChiNhanh;
                $.ajax({
                    traditional: true,
                    url: urlApiChamCong + 'GetListMayChamCongByChiNhanh',
                    type: 'POST',
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    data: myData,
                    success: function (data) {
                        if (data.dataSoure.data.length !== 0 && data.dataSoure.data[0].ID !== self.IDMayChamCongSelected) {
                            self.IDMayChamCongSelected = data.dataSoure.data[0].ID;
                            VueDataMayChamCong.LoadDataFromDatabase(true);
                        }
                        else if (data.dataSoure.data.length === 0){
                            self.IDMayChamCongSelected = "";
                            VueDataMayChamCong.LoadDataFromDatabase(true);
                        }
                        self.lstMayChamCong = data.dataSoure.data;
                    }
                });
            }
            else {
                setTimeout(self.LoadMayChamCong, 250);
            }
        },
        
    }
});
VueMayChamCong.LoadMayChamCong();

var VueDataMayChamCong = new Vue({
    el: '#tblDataMayChamCong',
    data: {
        CongTho: {
            data: [],
            ListPage: [],
            PageView: '',
            //isprev = CurrentPage > 3 && page > 5,
            //isnext = CurrentPage < page - 2 && page > 5,
            NumOfPage: 0
        }
    },
    methods: {
        LoadDataFromDatabase: function (resetpage = false) {
            let self = this;
            if (resetpage) {
                VuePageList.currentPage = 1;
            }
            let myData = {};
            myData.IDMayChamCong = VueMayChamCong.IDMayChamCongSelected;
            myData.InYear = VueInYear.YearSelected;
            myData.InMonth = VueInMonth.MonthSelected;
            myData.PageSize = VuePageList.PageSize;
            myData.CurrentPage = VuePageList.currentPage;
            $.ajax({
                traditional: true,
                url: urlApiChamCong + 'GetDuLieuCongTho',
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: myData,
                success: function (data) {
                    self.CongTho = data.dataSoure;
                    VuePageList.ListPage = self.CongTho.ListPage;
                    VuePageList.NumberOfPage = self.CongTho.NumOfPage;
                    VuePageList.PageView = self.CongTho.PageView;
                    VuePageList.CheckIsprevIsnext();
                }
            });
        },
        LoadDataFromDevice: function () {
            VueModalLoading.ShowModal('Đang kết nối máy chấm công');
            VueModalLoading.LoadTextStatus("TaiDuLieuMayChamCong");
            let self = this;
            let myData = {};
            myData.IDMayChamCong = VueMayChamCong.IDMayChamCongSelected;
            myData.InYear = VueInYear.YearSelected;
            myData.InMonth = VueInMonth.MonthSelected;
            $.ajax({
                traditional: true,
                url: urlApiChamCong + 'TaiDuLieuMayChamCong',
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: myData,
                success: function (data) {
                    if (data.res) {
                        ShowMessage_Success(data.mess);
                        self.LoadDataFromDatabase(true);
                    }
                    else {
                        ShowMessage_Danger(data.mess);
                    }
                }
            });
            //console.log("End LoadDataFromDevice");
        }
    }
});

var VueInYear = new Vue({
    el: '#YSel',
    data: {
        ListYear: ArrYear,
        YearSelected: YearSelected
    },
    methods: {
        onChange: function () {
            VueDataMayChamCong.LoadDataFromDatabase(true);
        }
    }
})

var VueInMonth = new Vue({
    el: '#MSel',
    data: {
        ListMonth: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12],
        MonthSelected: MonthNow
    },
    methods: {
        onChange: function () {
            VueDataMayChamCong.LoadDataFromDatabase(true);
        }
    }
})

var filterTarget = "TaiDuLieuMayChamCong";