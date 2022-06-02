

var StoreRegistration = function (item) {
    var self = this;
    var sort = 1;
    var colum = 6;
    //===============================
    // Khai báo chung
    //===============================
    self.LisstStoreRegistration = ko.observableArray(item);
    self.pageCount = ko.observable();
    self.PageIten = ko.observable();
    self.page = ko.observable(1);
    self.koSearch = ko.observable('');
    self.listcolumSort = ko.observableArray();

    self.koStatus = ko.observable(true);
    self.koHoten = ko.observable();
    self.koTenCuaHang = ko.observable();
    self.koPhone = ko.observable();
    self.ListHSD = ko.observableArray();
    self.ListGoiDichVu = ko.observableArray();

    self.koGhiChu = ko.observable();
    self.koVersion = ko.observable(0);
    self.HanSuDung = ko.observable();

     //===============================
    // Phân trang chi tiết
    //===============================
    self.ListServiceSms = ko.observableArray();
    self.ListServiceRecharge = ko.observableArray();
    self.c_pageCount = ko.observable();
    self.c_PageItem = ko.observable();
    self.c_page = ko.observable(1);
    self.c_koSearch = ko.observable();
    self.c_ClickPrevious = function () {
        if (self.page() > 1) {
            self.page(self.page() - 1)
            c_FilterGrid();
        }
    }

    self.c_ClickNext = function () {
        if (self.c_page() < self.c_pageCount()) {
            self.c_page(self.c_page() + 1)
            c_FilterGrid();
        }
    }

    self.c_netPageKeyup = function (d, e) {
        if (e.keyCode === 13) {
            if (self.c_page() > self.c_pageCount()
                || self.c_pageCount() === 1
                || !$.isNumeric(self.c_page())) {
                self.c_page(1);
            }
            c_FilterGrid();

        }
    }

    $('#c_SelectedLimit1').on('change', function () {
        self.c_page(1);
        c_FilterGrid();
    });
    $('#c_SelectedLimit2').on('change', function () {
        self.c_page(1);
        c_FilterGrid();
    });
    self.url_page = ko.observable();
    self.tyoe_tab = ko.observable(1);
    function c_FilterGrid() {
        var limit = $('#c_SelectedLimit2').val();
        if (self.tyoe_tab() === 1) {
            limit=$('#c_SelectedLimit1').val();
        }
        var model = {
            Search: self.c_koSearch(),
            Page: self.c_page(),
            Limit: limit,
        };
        $.ajax({
            url: self.url_page(),
            type: 'POST',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: model,
            success: function (result) {
                if (result.res === true) {
                    if (self.tyoe_tab() === 1) {

                        self.ListServiceSms(result.DataSoure.Data);
                    }
                    else{
                        self.ListServiceRecharge(result.DataSoure.Data);
                    }
                    self.c_PageItem(result.DataSoure.PageItem);
                    self.c_pageCount(result.DataSoure.PageCount);
                    self.c_page(result.DataSoure.Page);
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

    self.LoadServiceSms = function (item) {
        self.tyoe_tab(1);
        self.c_koSearch(item.Mobile);
        self.url_page('/Open24Api/ApiHome/SearchDetailServiceSms');
        c_FilterGrid();
    }
    self.LoadServiceRecharge = function (item) {
        self.tyoe_tab(2);
        self.c_koSearch(item.Mobile);
        self.url_page('/Open24Api/ApiHome/SearchDetailRechargeService');
        c_FilterGrid();
    }

    self.suppliersms = ko.observableArray();
    function loadsuppliersms() {
        $.ajax({
            url: '/Open24Api/ApiHome/GetSupplierSms',
            type: 'GET',
            dataType: 'json',
            success: function (result) {
                if (result.res === true) {
                    self.suppliersms(result.DataSoure);
                }
                else {
                    alert(result.mess);
                }
            }
        });
    }
    loadsuppliersms();
    self.priceactived = ko.observable();
    self.ID_ServiceSms = ko.observable();
    self.changeStatus = function (item) {
        self.ID_ServiceSms(item.ID);
        $('#selected-Status').val(item.Status);
        $('#selected-suppliersms').val(item.ID_SupplierSms);
        self.priceactived(item.Price);
        
        $('#myModalServiceSms').modal('show');
        if (item.Status !== '2' && item.Status !== 2) {
            $('.pricea-ctived').show();
        }
        else {

            $('.pricea-ctived').hide();
        }
    };
    self.saveStatus = function () {
        var model = {
            ID: self.ID_ServiceSms(),
            ID_SupplierSms: $('#selected-suppliersms').val(),
            Status: $('#selected-Status').val(),
            Price: self.priceactived()
        };
        $.ajax({
            url: '/Open24Api/ApiHome/UpdateServiceSms',
            type: 'POST',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: model,
            success: function (result) {
                if (result.res === true) {
                    AlertNotice(result.mess);
                    c_FilterGrid();
                    $('#myModalServiceSms').modal('hide');
                }
                else {
                    AlertError(result.mess);
                }
            },
            error: function () {
                alert("Đã xảy ra lỗi.");
            }
        });
    }
    self.ThemMoiDichVunapTien = function (item) {
        vmdichvunaptien.Insert(item.Mobile, item.Key);
    }
    self.CapNhatPhieuNap = function (item) {
        vmdichvunaptien.Update(item);
    }
    self.XoaPhieuNhap = function (item) {
        if (confirm('Bạn có chắc chắn muốn xóa không?')) {
            $.ajax({
                url: '/Open24Api/ApiHome/RemovePhieuNapTien?Key='+item.ID,
                type: 'GET',
                success: function (result) {
                    if (result.res === true) {
                        AlertNotice(result.mess);
                        c_FilterGrid();
                    }
                    else {
                        AlertError(result.mess)
                    }
                }
            });
        } else {
            return;
        }
    }
    $('body').on("SucsessPhieuNapTien", function () {
        c_FilterGrid();
    });
    //===============================
    // Nhập trang
    //===============================
    self.netPageKeyup = function (d, e) {
        if (e.keyCode === 13) { 
            if (self.page() > self.pageCount()
                || self.pageCount() === 1
                || !$.isNumeric(self.page())) {
                self.page(1);
            }
            FilterGrid();

        }
    }

    //===============================
    // Next trang
    //===============================
    self.ClickNext = function () {
        if (self.page() < self.pageCount()) {
            self.page(self.page() + 1);
            FilterGrid();
        }

    }
    //===============================
    // click Tìm kiếm gridview
    //===============================
    self.SearchGrid = function (d,e) {
        if (e.keyCode === 13) {
            self.page(1);
            FilterGrid();
        }

    }
    //===============================
    // back trang
    //===============================
    self.ClickPrevious = function () {
        if (self.page() > 1) {
            self.page(self.page() - 1);

            FilterGrid();
        }
    }
    //===============================
    // select số bản ghi trang
    //===============================
    $('#SelectedLimit').on('change', function () {
        self.page(1);
        FilterGrid();

    });
    
    //===============================
    // Cập nhật cửa hàng
    //===============================
    var datetime;
    self.btnUpdate = function (value) {
        self.koPhone(value.Mobile);
        self.koStatus(value.Status);
        self.koHoten(value.Name)
        self.koTenCuaHang(value.TenCuaHang);
        self.koVersion(value.Version);
        $('#selected_GoiDichVu').val(value.ID_GoiDichVu);
        $('.gia-han-nam').val(null);
        $('.gia-han-thang').val(null);
        $('.gia-han-ngay').val(null);
        self.koGhiChu('');
        var $datepicker = $('#datetimepickerUpdate');
        if (value.ExpiryDate !== undefined
            && value.ExpiryDate !== null
            && value.ExpiryDate.replace(/\s+/g, '') !== "") {
            var date = moment(value.ExpiryDate).format('DD/MM/YYYY');

            $datepicker.val(date);
            self.HanSuDung(new Date(date.split('/')[2], parseInt(date.split('/')[1]) - 1, date.split('/')[0]));
            if (self.HanSuDung() <= new Date()) {
                self.HanSuDung(new Date())
                var date1 = moment(new Date()).format('DD/MM/YYYY');
                $('#datetimepickerUpdateNews').val(date1);
            }
            else {
                $('#datetimepickerUpdateNews').val(date);
            }
        }
        else {
            self.HanSuDung(new Date());
            var date = moment(new Date()).format('DD/MM/YYYY');
            $('#datetimepickerUpdateNews').val(date);
            $datepicker.val(null);
        }
      
        datetime = new Date(self.HanSuDung());
        $('#myModal').modal('show');
    }
    $('.gia-han-nam').on('change', function () {
        LoadDatetim();
    });
    $('.gia-han-thang').on('change', function () {
       
        LoadDatetim();
    });
    $('.gia-han-ngay').on('change', function () {
        LoadDatetim();
    });
    function LoadDatetim() {
        var thang = self.HanSuDung().getMonth();
        var ngay = self.HanSuDung().getDate();
        var nam = self.HanSuDung().getFullYear();
        datetime.setDate(1);
        if (!localValidate.CheckNull($('.gia-han-nam').val())) {
            nam += parseInt($('.gia-han-nam').val());
        }
        datetime.setFullYear(nam);
        if (!localValidate.CheckNull($('.gia-han-thang').val())) {
            thang += parseInt($('.gia-han-thang').val());
        }
        datetime.setMonth(thang);
        if (!localValidate.CheckNull($('.gia-han-ngay').val())) {
            ngay += parseInt($('.gia-han-ngay').val());
        };
        datetime.setDate(ngay);
        var date = moment(datetime).format('DD/MM/YYYY');
        $('#datetimepickerUpdateNews').val(date);
    }
    self.SaveUpdateStore = function ()
    {
        var date = $('#datetimepickerUpdateNews').val();
        if (date === null || date === '') {
            AlertError("Vui lòng nhập hạn sử dụng");
            return false;
        }
        else if ($('#selected_GoiDichVu').val() === null) {
            AlertError("Vui lòng chọn gói dịch vụ");
            return false;
        }
        else {
            var datesplit = date.split('/');
            date = datesplit[1] + "/" + datesplit[0] + "/" + datesplit[2];
            var thoigiangiahan = null;
            if ($('.gia-han-nam').val() !== undefined && $('.gia-han-thang').val() !== undefined && $('.gia-han-ngay').val() !== undefined) {
                thoigiangiahan = $('.gia-han-nam').val() + "_" + $('.gia-han-thang').val() + "_" + $('.gia-han-ngay').val();
            }
            var model = {
                Mobile: self.koPhone(),
                ExpiryDate: date,
                Version: self.koVersion(),
                GhiChu: self.koGhiChu(),
                Status: self.koStatus(),
                ID_GoiDichVu: $('#selected_GoiDichVu').val(),
                ThoGianGiaHan: thoigiangiahan
            }
            $.ajax({
                data: model,
                url: '/Open24Api/ApiStoreRegistration/UpdateStore',
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (result) {
                    if (result.res === true) {
                        AlertNotice(result.mess);
                        getStoreRegistration();
                        $('#myModal').modal('hide');
                    }
                    else {
                        AlertError(result.mess);
                    }
                },
                error: function () {
                    alert("Đã xảy ra lỗi.");
                }
            });
        }
    }
    //===============================
    // Xem mã kích hoạt
    //===============================
    self.ViewMa = function (value)
    {
        if (value.MaKichHoat !== null && value.MaKichHoat !== '') {
            Showmessage("Mã kích hoạt SĐT <span style='color:green'>" + value.Mobile + " </span> là <span style='color:green'>" + value.MaKichHoat + "</span>");
        }
        else {
            Showmessage("SĐT <span style='color:green'>" + value.Mobile + "</span> chưa có mã kích hoạt");
        }
    }

    //===============================
    // Tìm kiếm gridview chung
    //===============================
    function FilterGrid() {
        var object = {
            Search: self.koSearch(),
            Page: self.page(),
            Limit: $('#SelectedLimit').val(),
            TypeHsd: $('#SelectedHSD').val(),
            Version: $('#SelectedVersion').val(),
            Status: $("#SelectedStatus").val() === '3' ? null :( $("#SelectedStatus").val()==='1'?1:0),
            Columname: colum,
            Sort: sort
        };
        $.ajax({
            url: '/AdminPage/StoreRegistration/GetDataForShearch',
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: 'application/json',
            data: ko.toJSON({
                daTatable: object
            }),
            success: function (result) {
                self.page(result.Page);
                self.pageCount(result.PageCount);
                self.LisstStoreRegistration(result.Data);
                self.PageIten(result.PageItem);
            },
            error: function () {
                alert("Đã xảy ra lỗi.");
            }
        });
    }
    self.ListHistory = ko.observableArray();
    self.LoadHistory = function (model) {
        $.ajax({
            url: '/Open24Api/ApiStoreRegistration/GetHistory?subdomain=' + model.Key,
            type: 'GET',
            contentType: 'application/json',
            success: function (result) {
                if (result.res === true) {
                    self.ListHistory(result.DataSoure);
                }
                else {
                    console.log(result.mess);
                }
            },
            error: function () {
                alert("Đã xảy ra lỗi.");
            }
        });
    }

    //===============================
    // Load dữ liệu cửa hàng đăng ký
    //===============================
    function getStoreRegistration() {
        $("#iconSort").remove();
        var search = sessionStorage.getItem("SearchSubdomain");
        self.koSearch(search);
        sessionStorage.removeItem("SearchSubdomain");
        FilterGrid(); 
        $.ajax({
            url: '/Open24Api/ApiStoreRegistration/GetDataFirst',
            type: 'GET',
            contentType: 'application/json',
            async: true,
            success: function (result) {
                if (result.res === true)
                {
                    self.ListHSD(result.DataSoure.ListHSD);
                    self.ListGoiDichVu(result.DataSoure.GoiDichVu);
                }
                else {
                    AlertError(result.mess);
                }
            },
            error: function () {
                alert("Đã xảy ra lỗi.");
            }
        });
    };

    getStoreRegistration();

    //===============================
    // SortGrid
    //===============================
 

    $('#sortmobile').click(function () {
        colum = 0;
        SortGrid(this);
    });
    $('#sortname').click(function () {
        colum = 1;
        SortGrid(this);
    });

    $('#sortcreatedate').click(function () {
        colum = 2;
        SortGrid(this);
    });
    $('#sortbusines').click(function () {
        colum = 3;
        SortGrid(this);
    });

    $('#sortexpiryDate').click(function () {
        colum = 4;
        SortGrid(this);
    });
    $('#sortstatus').click(function () {
        colum = 5;
        SortGrid(this);
    });
    function SortGrid(item) {
        $("#iconSort").remove();
        if (sort === 0) {
            sort = 1;
            item.innerHTML += " <i  id='iconSort' class='fa fa-caret-down pull-right' aria-hidden='true'></i>";
        }
        else {
            sort = 0;
            item.innerHTML += " <i  id='iconSort' class='fa fa-caret-up pull-right' aria-hidden='true'></i>";
        }
        FilterGrid();  
    };
    $('#SelectedStatus').change(function () {
        self.page(1);
        FilterGrid();  

    });
    $('#SelectedHSD').change(function () {
        self.page(1);
        FilterGrid();

    });
    $('#SelectedVersion').change(function () {
        self.page(1);
        FilterGrid();

    });

    self.ExportExcel = function () {
        var status = $("#SelectedStatus").val() === '3' ? null : ($("#SelectedStatus").val() === '1' ? 1 : 0);
        var text = self.koSearch();
        if (self.koSearch() === undefined || self.koSearch() === null) {
            text = '';
        }
        window.open('/AdminPage/StoreRegistration/ExportExcel?TypeHsd=' + $('#SelectedHSD').val() + '&Status=' + status + '&Version=' + $('#SelectedVersion').val() + '&text=' + text);
       
    }
};
ko.applyBindings(new StoreRegistration());

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
// Hiện thị trạng thái
//===============================
function ConvertTrangthai(value) {
    if (value === 0) {
        return "Hủy"
    }
    else if (value === 1) {
        return "Kích hoạt"
    }
    else {
        return "Chờ kích hoạt";
    }
};