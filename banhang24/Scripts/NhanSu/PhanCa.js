var ViewModal = function () {
    var self = this;
    self.MaPhieu = ko.observable();
    var _IDchinhanh = $('#hd_IDdDonVi').val();
    var _id_NhanVien = $('.idnhanvien').text();
    self.NguoiTao = ko.observable(null);
    self.TodayBC = ko.observable();
    self.TrangThai = ko.observable(1);
    self.LoaiCa = ko.observable(1);
    self.Text_search = ko.observable();
    self.check_TrangThaiPhanCa = ko.observable('2');
    $("#datetimepicker_mask").attr("disabled", "disabled");
    $("#txtNguoiTao").attr("disabled", "disabled");
    // lấy về danh sách đơn vị
    var BH_DonViUri = '/api/DanhMuc/DM_DonViAPI/';
    var NhanVienUri = '/api/DanhMuc/NS_NhanVienAPI/';
    var NS_NhanSuUri = '/api/DanhMuc/Ns_NhanSuAPI/';
    self.DonVis = ko.observableArray()
    self.listAddChiNhanh = ko.observableArray();
    var ID_news = 1;
    function refreshChiNhanh() {
        self.listAddChiNhanh([]);
        ID_news = 1;
        self.listAddChiNhanh.push({
            ID: ID_news,
            ID_ChiNhanh: null,
            Text_ChiNhanh: "",
            ID_PhongBan: null,
            Text_PhongBan: "",
            listPhongBan: [],
            LaMacDinh: true
        });
    }
    refreshChiNhanh();
    function LoadingForm(IsShow) {
        $('.table-reponsive').gridLoader({ show: IsShow });
    }
    function LoadPhongBanChiNhanh(chinhanhId) {
        var result = self.listAddChiNhanh().filter(o => o.ID_ChiNhanh === chinhanhId);
        for (var i = 0; i < result.length; i++) {
            CallAjaxPhongBanChang(result[i]);
        }
    }
    function getDonVi() {
        ajaxHelper(BH_DonViUri + "GetListDonVi1", "GET").done(function (data) {
            self.DonVis(data);
        });
    }
    getDonVi();
    self.SeletecChiNhanh = function (root, item) {
        root.ID_ChiNhanh = item.ID;
        root.Text_ChiNhanh = item.TenDonVi;
        root.ID_PhongBan = null;
        root.listPhongBan = [];
        root.Text_PhongBan = null;
        self.listAddChiNhanh.refresh();
        $.getJSON(NhanVienUri + "GetTreePhongBan?chinhanhId=" + item.ID, function (data) {
            root.listPhongBan = data;
            self.listAddChiNhanh.refresh();
        });
    }
    self.clickInputPhongBan = function (root) {
        $.getJSON(NhanVienUri + "GetTreePhongBan?chinhanhId=" + root.ID_ChiNhanh, function (data) {
            console.log(data);
            root.listPhongBan = data;
            self.listAddChiNhanh.refresh();
        });
    }
    self.Seletecphongban = function (root, item) {
        if (root.ID_ChiNhanh !== null) {
            if (self.listAddChiNhanh().some(o => o.ID_ChiNhanh === root.ID_ChiNhanh && o.ID_PhongBan === item.id)) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> Phòng ban chi nhánh này đã được chọn', "danger");
            }
            else {
                root.ID_PhongBan = item.id;
                root.Text_PhongBan = item.text;
                self.listAddChiNhanh.refresh();

            }
            console.log(self.listAddChiNhanh());
        }
        else {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> Vui lòng chọn chi nhánh trước khi chọn phòng ban', "danger");
        }

    }
    // load Nhân viên
    self.nameNhanVien = ko.observable();
    self.getNameNhanVien = function () {
        ajaxHelper('/api/DanhMuc/TQ_DoanhThuAPI/' + "getNameNhanVien?ID_NhanVien=" + _id_NhanVien, "GET").done(function (data) {
            self.nameNhanVien(data);
        });
    }
    self.getNameNhanVien();
    $('.choose_TrangThai input').on("click", function () {
        var genderMaleCheckbox = $('#gender_male_checkbox').prop('checked');
        if (genderMaleCheckbox == true) {
            self.TrangThai(1);
        }
        else {
            self.TrangThai(0);
        }
        console.log(self.TrangThai());
    });
    // thêm mới phân ca
    self.add_PhanCa = function () {
        self.NguoiTao(self.nameNhanVien());
        $('#datetimepicker_mask').datetimepicker({
            timepicker: true,
            mask: true,
            format: 'd/m/Y H:i',
            value: new Date()
        });
        $('#modalthemmoiphanca').modal('show');
    }
    // sửa phân ca
    self.edit_PhanCa = function (item) {
        self.NguoiTao(item.NguoiTao);
        $('#datetimepicker_mask').datetimepicker({
            timepicker: true,
            mask: true,
            format: 'd/m/Y H:i',
            value: moment(item.NgayTao).format('DD/MM/YYYY HH:mm')
        });
    }
    // chọn thời gian
    $('#datetime_TuNgay').keypress(function (e) {
        if (e.keyCode == 13) {
            dktime = $(this).val();
            thisDate = $(this).val();
            var t = thisDate.split(" ");
            var t1 = t[0].split("/").reverse().join("-")
            thisDate = moment(t1).format('MM/DD/YYYY')
            _tonkhoStart = moment(new Date(thisDate)).format('YYYY-MM-DD');
            var dt = new Date(thisDate);
            _tonkhoEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
            console.log(_timeEnd);
            if (thisDate != 'Invalid date') {
                self.TodayBC('Thời gian: ' + $(this).val())
                self.LoadReport();
            }
            else {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Định dạng thời gian không chính xác!", "danger");
            }
        }
    });
    $('#datetime_TuNgay').on('change.dp', function (e) {
        dktime = $(this).val();
        thisDate = $(this).val();
        var t = thisDate.split(" ");
        var t1 = t[0].split("/").reverse().join("-")
        thisDate = moment(t1).format('MM/DD/YYYY')
        _tonkhoStart = moment(new Date(thisDate)).format('YYYY-MM-DD');
        var dt = new Date(thisDate);
        _tonkhoEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
    });
    $('#datetime_DenNgay').keypress(function (e) {
        if (e.keyCode == 13) {
            dktime = $(this).val();
            thisDate = $(this).val();
            var t = thisDate.split(" ");
            var t1 = t[0].split("/").reverse().join("-")
            thisDate = moment(t1).format('MM/DD/YYYY')
            _tonkhoStart = moment(new Date(thisDate)).format('YYYY-MM-DD');
            var dt = new Date(thisDate);
            _tonkhoEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
            console.log(_timeEnd);
            if (thisDate != 'Invalid date') {
                self.TodayBC('Thời gian: ' + $(this).val())
                self.LoadReport();
            }
            else {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Định dạng thời gian không chính xác!", "danger");
            }
        }
    });
    $('#datetime_DenNgay').on('change.dp', function (e) {
        dktime = $(this).val();
        thisDate = $(this).val();
        var t = thisDate.split(" ");
        var t1 = t[0].split("/").reverse().join("-")
        thisDate = moment(t1).format('MM/DD/YYYY')
        _tonkhoStart = moment(new Date(thisDate)).format('YYYY-MM-DD');
        var dt = new Date(thisDate);
        _tonkhoEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
        if (thisDate != 'Invalid date') {
            self.TodayBC('Thời gian: ' + $(this).val())
            self.LoadReport();
        }
        else {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Định dạng thời gian không chính xác!", "danger");
        }
    });

    // load danh sách phân ca
    self.LoadReport = function () {

    };
    $('.menuCheckboxnews li input').on('click', function (item) {
        self.check_TrangThaiPhanCa($(this).val());
        console.log(self.check_TrangThaiPhanCa());
        self.getList_NhanVienPhanCa();
    });
    $('#LoaiCa li').on('click', function () {
        self.LoaiCa($(this).val());
        console.log(self.LoaiCa());
        if (self.LoaiCa() == 1)
        {
            self.Text_search($('#text_seachTuan').val());
        }
        else if (self.LoaiCa() == 2) {
            self.Text_search($('#text_seachThang').val());
        }
        else {
            self.Text_search($('#text_seachMacDinh').val());
        }
        self.getList_NhanVienPhanCa();
    });
    $('#text_seachThang').keypress(function (e) {
        if (e.keyCode == 13) {
            console.log($(this).val());
            self.Text_search($(this).val());
            self.getList_NhanVienPhanCa();
        }
    });
    $('#text_seachTuan').keypress(function (e) {
        if (e.keyCode == 13) {
            console.log($(this).val());
            self.Text_search($(this).val());
            self.getList_NhanVienPhanCa();
        }
    });
    $('#text_seachMacDinh').keypress(function (e) {
        if (e.keyCode == 13) {
            console.log($(this).val());
            self.Text_search($(this).val());
            self.getList_NhanVienPhanCa();
        }
    });
    self.NhanVien_CaTuan = ko.observableArray();
    self.NhanVien_CaThang = ko.observableArray();
    self.NhanVien_CaMacDinh = ko.observableArray();
    self.getList_NhanVienPhanCa = function () {
        LoadingForm(true);
        var array_Seach = {
            MaNhanVien: self.Text_search(),
            ID_ChiNhanh: self.listAddChiNhanh()[0].ID_ChiNhanh,
            ID_PhongBan: self.listAddChiNhanh()[0].ID_PhongBan,
            LoaiCa: self.LoaiCa(),
            TrangThai: self.check_TrangThaiPhanCa(),
            TuNgay: new Date(),
        }
        console.log(array_Seach);
        if (self.LoaiCa() == 1)
        {
            ajaxHelper(NS_NhanSuUri + "getList_NhanVienCaTuan", "POST", array_Seach).done(function (data) {
                self.NhanVien_CaTuan(data.LstData);
                console.log(data.LstData);
                LoadingForm(false);
            });
        }
        else if (self.LoaiCa() == 2) {
            ajaxHelper(NS_NhanSuUri + "getList_NhanVienCaThang", "POST", array_Seach).done(function (data) {
                self.NhanVien_CaThang(data.LstData);
                console.log(data.LstData);
                LoadingForm(false);
            });
        }
        else
        {
            ajaxHelper(NS_NhanSuUri + "getList_NhanVienCaMacDinh", "POST", array_Seach).done(function (data) {
                self.NhanVien_CaMacDinh(data.LstData);
                console.log(data.LstData);
                LoadingForm(false);
            });
        }
    }
};
ko.applyBindings(new ViewModal());
ko.observableArray.fn.refresh = function () {
    var data = this().slice(0);
    this([]);
    this(data);
};