var ViewModelHD = function () {
    var self = this;
    var BH_HoaDonUri = '/api/DanhMuc/BH_HoaDonAPI/';
    var DMDoiTuongUri = "/api/DanhMuc/DM_DoiTuongAPI/";
    var DMHangHoaUri = '/api/DanhMuc/DM_HangHoaAPI/';
    var Quy_HoaDonUri = '/api/DanhMuc/Quy_HoaDonAPI/';
    var CSKHUri = '/api/DanhMuc/ChamSocKhachHangAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/';

    var id_donvi = $('#hd_IDdDonVi').val();// get from @Html.Hidden
    var userLogin = $('#txtUserLogin').val();
    var _id_NhanVien = $('.idnhanvien').text();
    var _IDNguoiDung = $('.idnguoidung').text();

    self.TodayBC = ko.observable('Toàn thời gian');
    self.TenChiNhanh = ko.observableArray();
    self.HoaDons = ko.observableArray();
    self.BH_HoaDon_ChiTiet = ko.observableArray();
    self.NhanViens = ko.observableArray();
    self.PhongBans = ko.observableArray();
    self.selectedTheKH = ko.observable();
    self.GiaBans = ko.observableArray();
    self.NgayLapHD_Update = ko.observable();
    self.NgayApDung_Update = ko.observable();
    self.NgayHetHan_Update = ko.observable();
    self.isGara = ko.observable(false)
    self.InforHDprintf = ko.observableArray();
    self.CTHoaDonPrint = ko.observableArray();
    self.CTHoaDonPrintMH = ko.observableArray();

    var url = '$/BanLe';
    switch (VHeader.IdNganhNgheKinhDoanh.toUpperCase()) {
        case 'C16EDDA0-F6D0-43E1-A469-844FAB143014':
            self.isGara(true);
            url = 'g/Gara';
            break;
    }

    // lọc hàng hóa
    self.TT_HoanThanh = ko.observable(true);
    self.TT_DaHuy = ko.observable(false);
    self.TT_TamLuu = ko.observable(true);
    self.TT_GiaoHang = ko.observable(true);

    // Quyen user
    self.Quyen_NguoiDung = ko.observableArray();
    self.RoleView_ServicePackage = ko.observable(false);
    self.RoleInsert_ServicePackage = ko.observable(false);
    self.RoleUpdate_ServicePackage = ko.observable(false);
    self.RoleDelete_ServicePackage = ko.observable(false);
    self.RoleExport_ServicePackage = ko.observable(false);
    self.Show_BtnUpdate = ko.observable(false);
    self.Show_BtnCopy = ko.observable(false);
    self.Show_BtnEdit = ko.observable(false);
    self.Show_BtnDelete = ko.observable(false);
    self.Show_BtnExcelDetail = ko.observable(false);
    self.Show_BtnThanhToanCongNo = ko.observable(false);
    self.Show_BtnOpenHD = ko.observable(false);
    self.Show_BtnUpdateSoQuy = ko.observable(false);
    self.Show_BtnDeleteSoQuy = ko.observable(false);
    self.Allow_ChangeTimeSoQuy = ko.observable(false);
    self.Role_PrintHoaDon = ko.observable(false);
    self.Role_HoaHongDichVu_Edit = ko.observable(false);
    self.Role_HoaHongHoaDon_Edit = ko.observable(false);
    self.Role_EditChietKhauNVHoaDon = ko.observable(false);
    self.filter = ko.observable();
    self.filterMaHDGoc = ko.observable();
    self.filterFind = ko.observable();

    self.selectedNV = ko.observable();
    self.ID_NhanVieUpdateHD = ko.observable();
    self.error = ko.observable();
    self.booleanAdd = ko.observable(true);
    self.ID_DonViQuiDoi = ko.observable();

    self.filterBangGia = ko.observable();
    self.filterPhongBan = ko.observable();
    self.PhongBanChosed = ko.observableArray();
    self.GiaBanChosed = ko.observableArray();
    self.filterNgayLapHD = ko.observable("0");
    self.filterNgayLapHD_Input = ko.observable(); // ngày cụ thể
    self.filterNgayLapHD_Quy = ko.observable(6);

    self.BH_HoaDonChiTiets = ko.observableArray(); // split HoaDon = Hoa Don + Chi Tiet
    self.HoaDonDoiTra = ko.observableArray();
    self.LichSuThanhToan = ko.observableArray();
    self.LichSuTraHang = ko.observableArray();
    self.LichSuThanhToanDH = ko.observableArray();
    self.TongSLuong = ko.observable();
    self.TongGiamGiaHang = ko.observable(0);
    self.TongTienHangChuaCK = ko.observable(0);
    self.MaHoaDonParent = ko.observable();
    self.CongTy = ko.observableArray(); // get infor congty
    self.ThietLap = ko.observableArray(); // ThietLapTinhNang HeThong
    self.ChotSo_ChiNhanh = ko.observableArray();
    self.ThietLap_TichDiem = ko.observableArray();
    self.DMKhuyenMai = ko.observableArray();
    self.KM_KMApDung = ko.observableArray();
    self.ChiTietDoiTuong = ko.observableArray();
    self.NhomHangHoas = ko.observableArray();
    self.AllNhanVien_CKHoaDon = ko.observableArray();

    self.TongSoLuongHang = ko.observable(0);
    self.TongTienPhuTung = ko.observable(0);
    self.TongTienDichVu = ko.observable(0);
    self.TongTienPhuTung_TruocCK = ko.observable(0);
    self.TongTienDichVu_TruocCK = ko.observable(0);
    self.TongTienPhuTung_TruocVAT = ko.observable(0);
    self.TongTienDichVu_TruocVAT = ko.observable(0);

    self.TongThue_PhuTung = ko.observable(0);
    self.TongCK_PhuTung = ko.observable(0);
    self.TongThue_DichVu = ko.observable(0);
    self.TongCK_DichVu = ko.observable(0);
    self.TongSL_DichVu = ko.observable(0);
    self.TongSL_PhuTung = ko.observable(0);

    self.columsort = ko.observable(null);
    self.sort = ko.observable(null);
    // PThucThanhToan
    self.PThucChosed = ko.observableArray();
    self.PTThanhToan = ko.observableArray([
        { ID: '1', TenPhuongThuc: 'Tiền mặt' },
        { ID: '2', TenPhuongThuc: 'Thẻ' }
    ]);

    // sum at footer
    self.TongTienHang = ko.observable();
    self.TongTienThue = ko.observable();
    self.TongChiPhi = ko.observable();
    self.TongKhachTra = ko.observable();
    self.TongGiamGia = ko.observable();
    self.TongGiamGiaKM = ko.observable();
    self.TongThanhToan = ko.observable();
    self.TongKhachNo = ko.observable();
    self.TongPhaiTraKhach = ko.observable(0);
    self.TongTienDoiDiem = ko.observable(0);
    self.TongTienTheGTri = ko.observable(0);
    self.TongTienMat = ko.observable(0);
    self.TongChuyenKhoan = ko.observable(0);
    self.TongPOS = ko.observable(0);

    // Lap phieu thu
    self.ThuTuKhach = ko.observable();
    self.ThoiGian_ThanhToan = ko.observable(moment(new Date()).format('DD/MM/YYYY HH:mm'));
    self.CongVaoTK = ko.observable();
    self.ListHDisDebit = ko.observableArray();
    self.SelectPT = ko.observable();
    self.GhiChu_PhieuThu = ko.observable();
    self.NoHienTai = ko.observable();
    self.NoSau = ko.observable();
    self.TongTT_PhieuThu = ko.observable(0);
    self.ItemHoaDon = ko.observableArray();
    self.TienThua_PT = ko.observable(0);
    self.ListIDNhanVienQuyen = ko.observableArray();

    //lọc theo đơn vị
    self.ChiNhanhs = ko.observableArray();
    self.MangNhomDV = ko.observableArray();
    self.MangIDDV = ko.observableArray();
    self.filterHangHoa_ChiTietHD = ko.observable();

    // phan trang CTHD
    self.PageSize_CTHD = ko.observable(10);
    self.currentPage_CTHD = ko.observable(0);
    self.fromitem_CTHD = ko.observable(1);
    self.toitem_CTHD = ko.observable();
    self.PageCount_CTHD = ko.observable();
    self.TotalRecord_CTHD = ko.observable(0);

    self.ListCheckBox = ko.observableArray();
    self.NumberColum_Div2 = ko.observable();

    var sLoai = 'gói dịch vụ';

    function Load_Page() {
        GetListIDNhanVien_byUserLogin();
        getAllChiNhanh();
        GetCauHinhHeThong();
        getListNhanVien();
        getAllPhongBan();
        getAllGiaBan();
        GetDataChotSo();
        GetDM_NhomDoiTuong_ChiTiets();
        GetInforCongTy();
        loadMauIn();
        GetAllQuy_KhoanThuChi();
        GetDM_TaiKhoanNganHang();
        loadCheckbox();
        GetHT_TichDiem();
        GetAllNhomHangHoas();
        GetKM_CTKhuyenMai();
    }

    Load_Page();

    var Key_Form = 'Key_ServicePackage';

    function SetDefault_HideColumn() {
        var arrHideColumn = ['khuvuc', 'phuongxa', 'quanhuyen', 'chinhanhbanhang', 'email', , 'didong', 'nguoitao', 'tiendoidiem', 'tienthegiatri', 'tienmat', 'chuyenkhoan', 'tienpos'];
        var cacheHideColumn = localStorage.getItem(Key_Form);
        if (cacheHideColumn == null || cacheHideColumn === '[]') {
            // hide default some column
            for (var i = 0; i < arrHideColumn.length; i++) {
                LocalCaches.AddColumnHidenGrid(Key_Form, arrHideColumn[i], arrHideColumn[i]);
            }
        }
    }

    function loadCheckbox() {
        $.getJSON("api/DanhMuc/BaseApi/GetCheckedStatic?type=" + $('#pageID').val(), function (data) {
            if (!self.isGara()) {
                data = $.grep(data, function (x) {
                    return $.inArray(x.Key, ['bienso']) === -1;
                })
            }
            self.ListCheckBox(data);
            self.NumberColum_Div2(Math.ceil(data.length / 2));
            HideShowColumn();
        });
    }

    function HideShowColumn() {
        SetDefault_HideColumn();
        loadHtmlGrid();
    }
    function loadHtmlGrid() {
        LocalCaches.LoadFirstColumnGrid(Key_Form, $('#myList ul li input[type = checkbox]'), self.ListCheckBox());
    }
    $('#myList').on('change', 'ul li input[type = checkbox]', function () {
        var valueCheck = $(this).val();
        // valueCheck (1) = class name, valueCheck(2) = value  --> pass to func 
        // add/remove class is hidding in list cache {NameClass, Value}
        LocalCaches.AddColumnHidenGrid(Key_Form, valueCheck, valueCheck);
        $('.' + valueCheck).toggle();
    });
    $('#myList').on('click', 'ul li', function (i) {
        if ($(this).find('input[type = checkbox]').is(':checked')) {
            $(this).find('input[type = checkbox]').prop("checked", false);
        }
        else {
            $(this).find('input[type = checkbox]').prop("checked", true);
        }
        var valueCheck = $(this).find('input[type = checkbox]').val();
        LocalCaches.AddColumnHidenGrid(Key_Form, valueCheck, valueCheck);
        $('.' + valueCheck).toggle();
    });

    function CheckQuyenExist(maquyen) {
        var role = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen === maquyen;
        });
        return role.length > 0;
    }

    //load quyền
    function loadQuyenIndex() {
        ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetHT_NhomNguoiDung?idnguoidung=" + _IDNguoiDung + '&iddonvi=' + id_donvi, 'GET').done(function (data) {

            if (data.ID !== null) {
                self.Quyen_NguoiDung(data.HT_Quyen_NhomDTO);
                HideShowButton_HoaDon();

                self.Role_PrintHoaDon(CheckQuyenExist('GoiDichVu_In'));
                self.Show_BtnUpdateSoQuy(CheckQuyenExist('SoQuy_CapNhat'));
                self.Show_BtnDeleteSoQuy(CheckQuyenExist('SoQuy_Xoa'));
                self.Allow_ChangeTimeSoQuy(CheckQuyenExist('SoQuy_ThayDoiThoiGian'));
                self.Role_EditChietKhauNVHoaDon(CheckQuyenExist('GoiDichVu_SuaDoiChietKhauNVHoaDon'));
                self.Role_HoaHongDichVu_Edit(CheckQuyenExist('BanHang_HoaDongDichVu_CapNhat'))
                self.Role_HoaHongHoaDon_Edit(CheckQuyenExist('BanHang_HoaDongHoaDon_CapNhat'));
            }
            else {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + 'Không có quyền xem danh sách ' + sLoai, 'danger');
            }
        });
    }

    function GetListIDNhanVien_byUserLogin() {
        ajaxHelper(CSKHUri + 'GetListNhanVienLienQuanByIDLoGin_inDepartment?idnvlogin=' + _id_NhanVien
            + '&idChiNhanh=' + id_donvi + '&funcName=' + funcName, 'GET').done(function (data) {
                self.ListIDNhanVienQuyen(data);

                loadQuyenIndex();
            })
    }

    function GetKM_CTKhuyenMai() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/BH_KhuyenMaiAPI/' + 'GetKM_CTKhuyenMai?idDonVi=' + id_donvi, 'GET').done(function (data) {
                if (data !== null) {
                    self.DMKhuyenMai(data);
                    GetList_KMApDung();
                }
            });
        }
    }
    function Check_KhuyenMai_Active(idKhuyenMai) {
        var itemKM = $.grep(self.DMKhuyenMai(), function (x) {
            return x.ID_KhuyenMai === idKhuyenMai;
        });
        if (itemKM.length > 0) {
            // check Han su dung
            var now = moment(new Date()).format('YYYY-MM-DD');
            var ngayHetHan = moment(itemKM[0].ThoiGianKetThuc).format('YYYY-MM-DD');
            if (ngayHetHan >= now) {
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    }

    function GetList_KMApDung() {
        for (let i = 0; i < self.DMKhuyenMai().length; i++) {
            var itemKM = self.DMKhuyenMai()[i];
            var objKhuyenMai = {
                ID_KhuyenMai: null,
                TenKhuyenMai: "",
                HinhThuc: "", // 21: Mua hang giam hang; 22: mua hang tang hang; 23: mua hang tang diem; 24: gia ban theo SL Mua
                HinhThucKhuyenMai: "",
                Months: [],
                Dates: [],
                Hours: [],
                Days: [],
                ApDungNgaySinh: 0,
                ID_QuyDoiMuas: [],
                ID_QuyDoiTangs: [],
                ID_NhomHHMuas: [],
                ID_NhomHHTangs: [],
                ID_NhomKHs: [],
                ID_NhanViens: [],
                DM_KhuyenMai_ChiTiet: [],
                Note_HinhThuc: '', // ghi chu tuong ung voi tung loai hinh thuc KM
            };
            objKhuyenMai.ID_KhuyenMai = itemKM.ID;
            objKhuyenMai.TenKhuyenMai = itemKM.TenKhuyenMai;
            objKhuyenMai.HinhThucKhuyenMai = itemKM.TenHinhThucKM;
            objKhuyenMai.HinhThuc = itemKM.HinhThuc;
            objKhuyenMai.ApDungNgaySinh = itemKM.ApDungNgaySinhNhat;
            if (itemKM.ThangApDung !== '') {
                objKhuyenMai.Months = itemKM.ThangApDung.split('_');
            }
            if (itemKM.NgayApDung !== '') {
                objKhuyenMai.Dates = itemKM.NgayApDung.split('_');
            }
            if (itemKM.ThuApDung !== '') {
                objKhuyenMai.Days = itemKM.ThuApDung.split('_');
            }
            if (itemKM.GioApDung !== '') {
                objKhuyenMai.Hours = itemKM.GioApDung.split('_');
            }
            for (let j = 0; j < itemKM.DM_KhuyenMai_ApDung.length; j++) {
                var itemKM_AD = itemKM.DM_KhuyenMai_ApDung[j];
                if (itemKM_AD.ID_NhomKhachHang !== null && $.inArray(itemKM_AD.ID_NhomKhachHang, objKhuyenMai.ID_NhomKHs) === -1) {
                    objKhuyenMai.ID_NhomKHs.push(itemKM_AD.ID_NhomKhachHang);
                }
                if (itemKM_AD.ID_NhanVien !== null && $.inArray(itemKM_AD.ID_NhanVien, objKhuyenMai.ID_NhanViens) === -1) {
                    objKhuyenMai.ID_NhanViens.push(itemKM_AD.ID_NhanVien);
                }
            }
            for (let j = 0; j < itemKM.DM_KhuyenMai_ChiTiet.length; j++) {
                var itemKMCT = itemKM.DM_KhuyenMai_ChiTiet[j];
                if (itemKMCT.ID_DonViQuiDoiMua !== null && $.inArray(itemKMCT.ID_DonViQuiDoiMua, objKhuyenMai.ID_QuyDoiMuas) === -1) {
                    objKhuyenMai.ID_QuyDoiMuas.push(itemKMCT.ID_DonViQuiDoiMua);
                }
                if (itemKMCT.ID_DonViQuiDoi !== null && $.inArray(itemKMCT.ID_DonViQuiDoi, objKhuyenMai.ID_QuyDoiTangs) === -1) {
                    objKhuyenMai.ID_QuyDoiTangs.push(itemKMCT.ID_DonViQuiDoi);
                }
                if (itemKMCT.ID_NhomHangHoa !== null) {
                    var arrNhomChildTang = GetAll_IDNhomChild_ofNhomHH(itemKMCT.ID_NhomHangHoa);
                    if (arrNhomChildTang.length > 0) {
                        for (let k = 0; k < arrNhomChildTang.length; k++) {
                            if ($.inArray(arrNhomChildTang[k], objKhuyenMai.ID_NhomHHTangs) === -1) {
                                objKhuyenMai.ID_NhomHHTangs.push(arrNhomChildTang[k]);
                            }
                        }
                    }
                }
                if (itemKMCT.ID_NhomHangHoaMua !== null) {
                    var arrNhomChildMua = GetAll_IDNhomChild_ofNhomHH(itemKMCT.ID_NhomHangHoaMua);
                    if (arrNhomChildMua.length > 0) {
                        for (let k = 0; k < arrNhomChildMua.length; k++) {
                            if ($.inArray(arrNhomChildMua[k], objKhuyenMai.ID_NhomHHMuas) === -1) {
                                objKhuyenMai.ID_NhomHHMuas.push(arrNhomChildMua[k]);
                            }
                        }
                    }
                }
            }
            // sort to do check SoLuongMua tương ứng với GiaKhuyenMai
            itemKM.DM_KhuyenMai_ChiTiet = itemKM.DM_KhuyenMai_ChiTiet.sort(function (a, b) {
                var x = a.SoLuongMua,
                    y = b.SoLuongMua;
                return x < y ? -1 : x > y ? 1 : 0;
            });
            objKhuyenMai.DM_KhuyenMai_ChiTiet = itemKM.DM_KhuyenMai_ChiTiet;
            self.KM_KMApDung.push(objKhuyenMai);
        }
        // remove KMai tang diem if HeThong khong cai dat tinh nang tich diem
        if (self.ThietLap().TinhNangTichDiem === false) {
            var listKM = $.grep(self.KM_KMApDung(), function (x) {
                return x.HinhThuc !== 14 && x.HinhThuc !== 23;
            });
            self.KM_KMApDung(listKM);
        }
        //console.log(2, self.KM_KMApDung())
    }
    function CheckKM_IsApDung(idNhanVien) {
        var isApDung = false;
        var dtNow = new Date();
        var _month = (dtNow.getMonth() + 1).toString(); // 1-12 (+1 because getMoth() return 0-11)
        var _date = (dtNow.getDate()).toString(); // 1- 31
        var _hours = (dtNow.getHours()).toString(); // 1-24
        var _day = (dtNow.getDay() + 1).toString(); // mon:2, tues:3, wed:4, thur:5, fri:6, sat: 7, sun: 8
        var _weekofMonth = Math.ceil(dtNow.getDate() / 7); // get week of Month ( 1- 5);
        var idNhomKH = null;
        var ngaysinhFull = 0;
        var ngaysinh = 0;
        var thangsinh = 0;
        var tuansinh = 0;
        var ctDoiTuong = self.ChiTietDoiTuong();
        // if chose KH --> get idNhomKH + ngaysinh
        if (ctDoiTuong !== null && ctDoiTuong.length > 0) {
            idNhomKH = ctDoiTuong[0].ID_NhomDoiTuong.toLowerCase();
            ngaysinhFull = ctDoiTuong[0].NgaySinh_NgayTLap;
        }
        if (ngaysinhFull !== 0 && ngaysinhFull !== null) {
            var dtNgaySinh = new Date(moment(ngaysinhFull).format('YYYY-MM-DD')); // must format 'YYYY-MM-DD'
            // get day, moth from dayFull
            ngaysinh = dtNgaySinh.getDate();
            thangsinh = (dtNgaySinh.getMonth() + 1).toString();
            tuansinh = Math.ceil(dtNgaySinh.getDate() / 7);
        }
        // get list KM by ID_DoiTuong and ID_NhanVien
        var arrKM_forDT = [];
        for (let i = 0; i < self.KM_KMApDung().length; i++) {
            // get KM apply for HangHoa
            var xItem = self.KM_KMApDung()[i];
            if (xItem.ApDungNgaySinh !== 0) {
                // if chose KH
                if (ctDoiTuong !== null && ctDoiTuong.length > 0) {
                    // check ID_NhanVien, ID_nhomKH
                    if ((xItem.Months.length === 0 || $.inArray(_month, xItem.Months) > -1)
                        && (xItem.Dates.length === 0 || $.inArray(_date, xItem.Dates) > -1)
                        && (xItem.Days.length === 0 || $.inArray(_day, xItem.Days) > -1)
                        && (xItem.Hours.length === 0 || $.inArray(_hours, xItem.Hours) > -1)
                        && (xItem.ID_NhanViens.length === 0 || $.inArray(idNhanVien, xItem.ID_NhanViens) > -1)
                        && (xItem.ID_NhomKHs.length === 0 || $.inArray(idNhomKH, xItem.ID_NhomKHs) > -1)) {
                        switch (xItem.ApDungNgaySinh) {
                            case 1: // ap dung ngay sinh theo ngay
                                if (ngaysinh.toString() === _date) {
                                    isApDung = true;
                                }
                                break;
                            case 2: // ap dung ngay sinh theo tuan
                                if (tuansinh.toString() === _weekofMonth) {
                                    isApDung = true;
                                }
                                break;
                            case 3: // ap dung ngay sinh theo thang
                                if (thangsinh.toString() === _month) {
                                    isApDung = true;;// esc for KM_KMApDung
                                }
                                break;
                        }
                    }
                }
            }
            else {
                // if ApDungNgaySinh = 0
                if ((xItem.Months.length === 0 || $.inArray(_month, xItem.Months) > -1)
                    && (xItem.Dates.length === 0 || $.inArray(_date, xItem.Dates) > -1)
                    && (xItem.Days.length === 0 || $.inArray(_day, xItem.Days) > -1)
                    && (xItem.Hours.length === 0 || $.inArray(_hours, xItem.Hours) > -1)
                    && (xItem.ID_NhanViens.length === 0 || $.inArray(idNhanVien, xItem.ID_NhanViens) > -1)
                    && (xItem.ID_NhomKHs.length === 0 || $.inArray(idNhomKH, xItem.ID_NhomKHs) > -1)) {
                    isApDung = true;
                }
            }
        }
        return isApDung;
    }
    function GetAllNhomHangHoas() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/DM_NhomHangHoaAPI/' + 'GetDM_NhomHangHoa', 'GET').done(function (data) {
                if (data !== null) {
                    for (let i = 0; i < data.length; i++) {
                        if (data[i].ID_Parent === null) {
                            var objParent = {
                                ID: data[i].ID,
                                TenNhomHangHoa: data[i].TenNhomHangHoa,
                                Childs: [],
                            }
                            for (let j = 0; j < data.length; j++) {
                                if (data[j].ID !== data[i].ID && data[j].ID_Parent === data[i].ID) {
                                    var objChild =
                                    {
                                        ID: data[j].ID,
                                        TenNhomHangHoa: data[j].TenNhomHangHoa,
                                        ID_Parent: data[i].ID,
                                        Child2s: []
                                    };
                                    for (let k = 0; k < data.length; k++) {
                                        if (data[k].ID_Parent !== null && data[k].ID_Parent === data[j].ID) {
                                            var objChild2 =
                                            {
                                                ID: data[k].ID,
                                                TenNhomHangHoa: data[k].TenNhomHangHoa,
                                                ID_Parent: data[j].ID,
                                            };
                                            objChild.Child2s.push(objChild2);
                                        }
                                    }
                                    objParent.Childs.push(objChild);
                                }
                            }
                            self.NhomHangHoas.push(objParent);
                        }
                    }
                }
            });
        }
    };

    function GetAll_IDNhomChild_ofNhomHH(arrIDNhomHang) {
        var arrNhomHHChilds = [];
        // get all IDChild of ID_Parent
        var lc_NhomHangHoas = self.NhomHangHoas();
        for (let j = 0; j < lc_NhomHangHoas.length; j++) {
            if (lc_NhomHangHoas[j].Childs.length > 0 && $.inArray(lc_NhomHangHoas[j].Childs[0].ID_Parent, arrIDNhomHang) > -1) {
                for (let k = 0; k < lc_NhomHangHoas[j].Childs.length; k++) {
                    arrNhomHHChilds.push(lc_NhomHangHoas[j].Childs[k].ID);
                    if (lc_NhomHangHoas[j].Childs[k].Child2s.length > 0) {
                        for (let i = 0; i < lc_NhomHangHoas[j].Childs[k].Child2s.length; i++) {
                            arrNhomHHChilds.push(lc_NhomHangHoas[j].Childs[k].Child2s[i].ID);
                        }
                    }
                }
            }
        }
        // add ID_Parent into arrNhomHHChilds
        for (let i = 0; i < arrIDNhomHang.length; i++) {
            arrNhomHHChilds.push(arrIDNhomHang[i]);
        }
        return arrNhomHHChilds;
    }

    self.clickbanhang = function () {
        localStorage.setItem('fromGoiDichVu', true);
        window.open(url, '_blank');
    }

    function ResetColumnSort() {
        self.columsort('');
        self.sort(0);
    }

    self.selectedCN = function (item) {
        $("#iconSort").remove();
        ResetColumnSort();
        var arrDV = [];
        for (var i = 0; i < self.MangNhomDV().length; i++) {
            if ($.inArray(self.MangNhomDV()[i], arrDV) === -1) {
                arrDV.push(self.MangNhomDV()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrDV) === -1) {
            self.MangNhomDV.push(item);
        }
        SearchHoaDon(false, false);
        //$('#choose_DonVi input').remove();
        // thêm dấu check vào đối tượng được chọn
        $('#selec-all-DV li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
                $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
            }
        });
        $('#choose_TenDonVi input').remove();
    }

    self.CloseDV = function (item) {
        self.MangNhomDV.remove(item);
        if (self.MangNhomDV().length === 0) {
            $('#choose_TenDonVi').append('<input type="text" id="dllChiNhanh" readonly="readonly" class="dropdown" placeholder="Chọn chi nhánh">');
        }
        SearchHoaDon(false, false);
        // remove check
        $('#selec-all-DV li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
    }

    self.showPopupNCC = function () {
        self.resetNhaCungCap();
        $('#modalPopuplg_NCC').modal('show');
        $('#modalPopuplg_NCC').on('shown.bs.modal', function () {
            $('#txtTenDoiTuong').select();
        })
        $('#lblTitleNCC').html("Thêm nhà cung cấp")
    };

    self.HuyHoaDon_updateChoThanhToan = function (item) {
        var msgDialog = '';
        var idHoaDon = item.ID;

        if (self.ChotSo_ChiNhanh().length > 0) {
            var ngayLapHD = moment(item.NgayLapHoaDon).format('YYYY-MM-DD HH:mm');
            var dtChotSo = moment(self.ChotSo_ChiNhanh()[0].NgayChotSo, 'YYYY-MM-DD HH:mm:ii').format('YYYY-MM-DD HH:mm');
            if (ngayLapHD <= dtChotSo) {
                ShowMessage_Danger('Hóa đơn đã chốt sổ. Không thể hủy');
                return false;
            }
        }

        // chek goiDV da sudung todo
        ajaxHelper(BH_HoaDonUri + 'CheckGoiDV_isUsed?idHoaDon=' + idHoaDon, 'GET').done(function (x) {
            if (x.res == false) {
                if (x.mes == '') {
                    // huy hoadon : neu dang co tra hang --> khong duoc huy 
                    // huy dat hang: neu dang co HD tao tu HD dat hang --> khong duoc huy
                    ajaxHelper(BH_HoaDonUri + 'GetDSHoaDon_chuaHuy_byIDDatHang/' + idHoaDon, 'GET').done(function (x) {
                        if (x == true) {
                            ShowMessage_Danger('Gói dịch vụ đã có đổi trả, không thể hủy');
                            return;
                        }
                        else {
                            msgDialog = 'Có muốn hủy hóa đơn <b>' + item.MaHoaDon + '</b> cùng những phiếu liên quan không?';
                            dialogConfirm('Thông báo xóa', msgDialog, function () {
                                $.ajax({
                                    type: "POST",
                                    url: BH_HoaDonUri + "Huy_HoaDon?id=" + idHoaDon + '&nguoiSua=' + userLogin + '&iddonvi=' + id_donvi,
                                    dataType: 'json',
                                    contentType: 'application/json',
                                    success: function (result) {
                                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật " + sLoai + " thành công", "success");
                                        SearchHoaDon(false, false);

                                        if (item.LoaiHoaDon !== 3) {
                                            //ajaxHelper(BH_HoaDonUri + 'UpdateGiaVonDM_GiaVon?id=' + idHoaDon + '&iddonvi=' + item.ID_DonVi +
                                            //    '&ngaynew=' + item.NgayLapHoaDon + '&loai=3', 'GET').done(function (data) {
                                            //    });

                                            // HuyHD : tru diem (cong diem am)
                                            // Huy TraHang: cong diem
                                            // HuyDatHang: khong thuc hien gi ca

                                            var diemGiaoDich = item.DiemGiaoDich;
                                            if (diemGiaoDich > 0 && item.ID_DoiTuong !== null) {
                                                diemGiaoDich = -diemGiaoDich;
                                                ajaxHelper(DMDoiTuongUri + 'HuyHD_UpdateDiem?idDoiTuong=' + item.ID_DoiTuong + '&diemGiaoDich=' + diemGiaoDich, 'POST').done(function (data) {
                                                });
                                            }
                                        }

                                        // insert Ht_NhatKySuDung
                                        var objDiary = {
                                            ID_NhanVien: _id_NhanVien,
                                            ID_DonVi: id_donvi,
                                            ChucNang: 'Danh mục ' + sLoai,
                                            NoiDung: "Xóa " + sLoai + ": " + item.MaHoaDon,
                                            NoiDungChiTiet: "Xóa " + sLoai + ": " + item.MaHoaDon.concat(', Người xóa: ', userLogin),
                                            LoaiNhatKy: 3,
                                            ID_HoaDon: idHoaDon,
                                            LoaiHoaDon: 19,
                                            ThoiGianUpdateGV: item.NgayLapHoaDon,
                                        };
                                        Post_NhatKySuDung_UpdateGiaVon(objDiary);
                                    },
                                    error: function (error) {
                                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cập nhật trạng thái thất bại", "danger");
                                    },
                                    complete: function () {
                                        $('#modalPopuplgDelete').modal('hide');
                                    }
                                });

                                UpdateNhom_KhachHang(item);
                            })
                        }
                    });
                }
            }
            else {
                ShowMessage_Danger('Gói dịch vụ đã được sử dụng, không thể hủy');
            }
        })
    }

    self.GetID_NhanVien = function (item) {
        self.ID_NhanVieUpdateHD(item.ID_NhanVien); //--> get to do updateHoaDon
    }

    self.updateHoaDon = function (formElement) {
        var id = formElement.ID;
        var maHoaDon = formElement.MaHoaDon;
        var idNhanVien = self.ID_NhanVieUpdateHD();
        var ngaylapHDOld = formElement.NgayLapHoaDon;

        if (idNhanVien === undefined) {
            // if not change ID_NhanVien --> get from DB
            idNhanVien = formElement.ID_NhanVien;
            // if ID_NhanVien in DB = null --> get ID_NhanVien login
            if (idNhanVien === null) {
                idNhanVien = _id_NhanVien;
            }
        }

        if (self.NgayLapHD_Update() === undefined) {
            self.NgayLapHD_Update(moment(formElement.NgayLapHoaDon).format('DD/MM/YYYY HH:mm'));
        }
        if (self.NgayApDung_Update() === undefined) {
            self.NgayApDung_Update(formElement.NgayApDungGoiDV);
        }
        if (self.NgayHetHan_Update() === undefined) {
            self.NgayHetHan_Update(formElement.HanSuDungGoiDV);
        }

        var check = CheckNgayLapHD_format(self.NgayLapHD_Update(), 1 ,formElement.ID_DonVi);

        if (!check) {
            return;
        }

        var ngaylapHD = moment(self.NgayLapHD_Update(), 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm:ss');
        var ngayApDung = self.NgayApDung_Update();
        if (ngayApDung !== null) {
            ngayApDung = moment(self.NgayApDung_Update(), 'DD/MM/YYYY').format('YYYY-MM-DD HH:mm:ss');
        }
        var ngayHetHan = self.NgayHetHan_Update();
        if (ngayHetHan !== null) {
            ngayHetHan = moment(self.NgayHetHan_Update(), 'DD/MM/YYYY').format('YYYY-MM-DD HH:mm:ss');
        }

        // comapredate todo update GiaVon (alway Ngay min)
        ngaylapHDOld = moment(ngaylapHDOld).format('YYYY-MM-DD HH:mm:ss');// alway NgayLapHoaDon old (Tinh said 2019.06.20)
        //if (ngaylapHD < ngaylapHDOld) {
        //    ngaylapHDOld = ngaylapHD;
        //}

        var HoaDon = {
            ID: id,
            MaHoaDon: maHoaDon,
            ID_NhanVien: idNhanVien,
            DienGiai: formElement.DienGiai,
            NguoiSua: userLogin,
            NgayLapHoaDon: ngaylapHD,
            NgayApDungGoiDV: ngayApDung,
            HanSuDungGoiDV: ngayHetHan,
        };

        var myData = {};
        myData.id = id;
        myData.objNewHoaDon = HoaDon;

        $.ajax({
            data: myData,
            url: BH_HoaDonUri + "PutBH_HoaDon2",
            type: 'PUT',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",

            success: function (x) {
                if (x.res == true) {
                    SearchHoaDon(false, false);
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật " + sLoai + " thành công", "success");

                    var objDiary = {
                        ID_NhanVien: _id_NhanVien,
                        ID_DonVi: id_donvi,
                        ChucNang: 'Danh mục ' + sLoai,
                        NoiDung: "Cập nhật  " + sLoai + ": " + maHoaDon,
                        LoaiNhatKy: 2,
                        ID_HoaDon: id,
                        LoaiHoaDon: 19,
                        ThoiGianUpdateGV: ngaylapHDOld,
                    };
                    Post_NhatKySuDung_UpdateGiaVon(objDiary);
                }
                else {
                    ShowMessage_Danger("Cập nhật hóa đơn thất bại");
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
            },
            complete: function (item) {
            }
        })
    }

    // Change Status at menuLeft
    var status = '';
    $('.clStatus').click(function () {
        var $val = $(this).val();
        if ($(this).is(':checked')) {
            if (status.indexOf($val) > -1) {
                status = status;
            }
            else {
                status = status + "," + $val;
            }
        }
        else {
            status = status.replace(',' + $val, '')
        }
        if (status !== '') {
            var arrStatus = status.split(',');
            switch (arrStatus.length) {
                case 2:
                    if ($.inArray('0', arr)) {
                        alert();
                    }
                    else {
                        alert();
                    }
                    break;
                case 3:
                    break;
                default:

            }
        }
    })
    // end Status

    $('#txtMaHD, #txtMaHDGoc').keypress(function (e) {
        $("#iconSort").remove();
        ResetColumnSort();
        if (e.keyCode === 13 || e.which === 13) {
            // reset currentPage if is finding at other page > 1
            self.currentPage(0);
            SearchHoaDon(false, false);
        }
    })

    //phân trang
    self.pageSizes = [10, 20, 30, 40, 50];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.currentPage = ko.observable(0);
    self.fromitem = ko.observable(0);
    self.toitem = ko.observable(0);
    self.PageCount = ko.observable(0);
    self.TotalRecord = ko.observable(0);

    // tim kiem JqAuto hàng hóa
    self.filterFind = function (item, inputString) {
        var itemSearch = locdau(item.TenHangHoa);
        var itemSearch1 = locdau(item.MaHangHoa);

        var locdauInput = locdau(inputString);
        var arr = itemSearch.split(/\s+/);
        var arr1 = itemSearch1.split(/\s+/);

        var sThreechars = '';
        var sThreechars1 = '';
        for (var i = 0; i < arr.length; i++) {
            sThreechars += arr[i].toString().split('')[0];
        }
        for (var i = 0; i < arr1.length; i++) {
            sThreechars1 += arr1[i].toString().split('')[0];
        }
        return itemSearch.indexOf(locdauInput) > -1 ||
            itemSearch1.indexOf(locdauInput) > -1 ||
            sThreechars.indexOf(locdauInput) > -1 ||
            sThreechars1.indexOf(locdauInput) > -1;
    }

    self.ResetCurrentPage = function () {
        $("#iconSort").remove();
        ResetColumnSort();

        self.currentPage(0);
        var lenData = self.HoaDons().length;
        self.PageCount(Math.ceil(lenData / self.pageSize()));
    };

    function getAllPhongBan() {
        ajaxHelper('/api/DanhMuc/DM_ViTriAPI/' + "GetListViTris", 'GET').done(function (data) {
            self.PhongBans(data);
        });
    }

    function getAllGiaBan() {
        ajaxHelper("/api/DanhMuc/DM_GiaBanAPI/" + "GetDM_GiaBanByIDDonVi?iddonvi=" + id_donvi, 'GET').done(function (data) {
            if (data !== null && data.length > 0) {
                self.GiaBans(data);
            }
            var objBGChung = {
                ID: '00000000-0000-0000-0000-000000000000',
                //ID: null,
                TenGiaBan: 'Bảng giá chung',
            }
            self.GiaBans.unshift(objBGChung);
        })
    }

    function getListNhanVien() {
        ajaxHelper("/api/DanhMuc/NS_NhanVienAPI/" + "GetNS_NhanVien_InforBasic?idDonVi=" + id_donvi, 'GET').done(function (data) {
            self.NhanViens(data);

            vmThanhToan.listData.NhanViens = self.NhanViens();
            vmHoaHongHoaDon.listData.NhanViens = self.NhanViens();
            vmHoaHongDV.listData.NhanViens = self.NhanViens();
        });
    }

    self.ShowColumn_LoHang = ko.observable(false); // hide/show colum LoHang in CTHD
    function GetCauHinhHeThong() {
        ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/' + "GetCauHinhHeThong/" + id_donvi, 'GET').done(function (data) {
            self.ThietLap(data);
            self.ShowColumn_LoHang(data.LoHang);
        });
    }

    function getAllChiNhanh() {
        ajaxHelper('/api/DanhMuc/DM_DonViAPI/' + "GetListDonViByIDNguoiDung?idnhanvien=" + _id_NhanVien, 'GET').done(function (data) {
            self.ChiNhanhs(data);
            vmThanhToan.listData.ChiNhanhs = data;

            var obj = {
                ID: id_donvi,
                TenDonVi: $('#_txtTenDonVi').html()
            }

            self.MangNhomDV.push(obj);
            $('#selec-all-DV li').each(function () {
                if ($(this).attr('id') === id_donvi) {
                    $(this).find('.fa-check').remove();
                    $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                }
            });
            $('#choose_TenDonVi input').remove();
        });
    }

    self.GetClassHD = function (page) {
        return ((page.pageNumber - 1) === self.currentPage()) ? "click" : "";
    };

    self.SelectedPB = function (item) {
        $("#iconSort").remove();
        ResetColumnSort();
        var arrIDPB = [];
        for (var i = 0; i < self.PhongBanChosed().length; i++) {
            if ($.inArray(self.PhongBanChosed()[i].ID, arrIDPB) === -1) {
                arrIDPB.push(self.PhongBanChosed()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDPB) === -1) {
            self.PhongBanChosed.push(item);
        }
        $('#choose-PB input').remove();

        // add check after li
        $('#selec-all-PB li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
                $(this).append('<i class="fa fa-check check-after-li"></i>')
            }
        });

        self.currentPage(0);
        SearchHoaDon(false, false);
    }

    self.ClosePhongBan = function (item) {
        $("#iconSort").remove();
        ResetColumnSort();
        self.PhongBanChosed.remove(item);
        if (self.PhongBanChosed().length === 0) {
            $('#choose-PB').append('<input type="text" class="dropdown form-control" placeholder="Chọn phòng/bàn">');
        }

        // remove check
        $('#selec-all-PB li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        SearchHoaDon(false, false);
    }

    self.SelectedGiaBan = function (item) {
        $("#iconSort").remove();
        ResetColumnSort();
        var arrID_BangGia = [];
        for (var i = 0; i < self.GiaBanChosed().length; i++) {
            if ($.inArray(self.GiaBanChosed()[i].ID, arrID_BangGia) === -1) {
                arrID_BangGia.push(self.GiaBanChosed()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrID_BangGia) === -1) {
            self.GiaBanChosed.push(item);
        }
        $('#choose-GB input').remove();

        // add check after li
        $('#selec-all-GB li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
                $(this).append('<i class="fa fa-check check-after-li"></i>')
            }
        });

        self.currentPage(0);
        SearchHoaDon(false, false);
    }

    self.CloseBangGia = function (item) {
        $("#iconSort").remove();
        ResetColumnSort();
        self.GiaBanChosed.remove(item);
        if (self.GiaBanChosed().length === 0) {
            $('#choose-GB').append('<input type="text" class="dropdown form-control" placeholder="Chọn bảng giá">');
        }

        // remove check
        $('#selec-all-GB li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        SearchHoaDon(false, false);
    }

    self.ChosePTThanhToan = function (item) {
        $("#iconSort").remove();
        ResetColumnSort();
        var arrIDPB = [];
        for (var i = 0; i < self.PThucChosed().length; i++) {
            if ($.inArray(self.PThucChosed()[i].ID, arrIDPB) === -1) {
                arrIDPB.push(self.PThucChosed()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDPB) === -1) {
            self.PThucChosed.push(item);
        }
        $('#choose-PThuc input').remove();

        // add check after li
        $('#selec-pt-ThanhToan li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
                $(this).append('<i class="fa fa-check check-after-li"></i>')
            }
        });

        self.currentPage(0);
        SearchHoaDon(false, false);
    }

    self.ClosePhuongThuc = function (item) {
        $("#iconSort").remove();
        ResetColumnSort();
        self.PThucChosed.remove(item);
        if (self.PThucChosed().length === 0) {
            $('#choose-PThuc').append('<input type="text" class="dropdown form-control" placeholder="Chọn phương thức">');
        }

        // remove check
        $('#selec-pt-ThanhToan li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        SearchHoaDon(false, false);
    }

    self.Click_IconSearch = function () {
        ResetColumnSort();
        self.currentPage(0);
        SearchHoaDon(false, false);
    }

    $('.choseNgayTao li').on('click', function () {
        $("#iconSort").remove();
        ResetColumnSort();
        $('#txtNgayTao').val($(this).text());
        self.filterNgayLapHD_Quy($(this).val());
        self.currentPage(0);
        SearchHoaDon(false, false);
    });

    self.DownloadFileTeamplateXLSX = function (pathFile) {
        var url = "/api/DanhMuc/DM_HangHoaAPI/Download_fileExcel?fileSave=" + pathFile;
        window.location.href = url;
    }

    function SearchHoaDon(isGoToNext, isExport) {
        var arrDV = [];

        $('.line-right').height(0).css("margin-top", "0px");
        var maHDFind = localStorage.getItem('FindHD');
        if (maHDFind !== null) {
            self.filter(maHDFind);
            self.filterNgayLapHD('0');
            self.filterNgayLapHD_Quy(0);
        }

        var txtMaHDon = self.filter();
        var txtMaHDgoc = self.filterMaHDGoc(); // HD tra hang
        if (!commonStatisJs.CheckNull(txtMaHDon)) {
            txtMaHDon = txtMaHDon.trim();
        }

        var arrIDPB = [];
        for (var i = 0; i < self.PhongBanChosed().length; i++) {
            arrIDPB.push(self.PhongBanChosed()[i].ID);
        }

        var arrIDBangGia = [];
        for (var i = 0; i < self.GiaBanChosed().length; i++) {
            arrIDBangGia.push(self.GiaBanChosed()[i].ID);
        }

        if (arrIDPB.length === 0) {
            arrIDPB = null;
        }

        if (arrIDBangGia.length === 0) {
            arrIDBangGia = null;
        }

        // search PTThanhToan
        var ptThuc = "0";// find all (TienMat + The)
        if (self.PThucChosed().length == 1) {
            ptThuc = self.PThucChosed()[0].ID; // 1: TienMat, 2: The
        }

        if (txtMaHDon === undefined) {
            txtMaHDon = "";
        }

        if (txtMaHDgoc === undefined) {
            txtMaHDgoc = "";
        }

        var sTenChiNhanhs = '';
        for (var i = 0; i < self.MangNhomDV().length; i++) {
            if ($.inArray(self.MangNhomDV()[i], arrDV) === -1) {
                arrDV.push(self.MangNhomDV()[i].ID);
                sTenChiNhanhs += self.MangNhomDV()[i].TenDonVi + ',';
            }
        }
        sTenChiNhanhs = Remove_LastComma(sTenChiNhanhs);// use when export excel
        self.MangIDDV(arrDV);

        // avoid error in Store procedure
        if (self.MangIDDV().length === 0) {
            self.MangIDDV([id_donvi]);
        }

        var arrIDNhanVien = [];
        for (var i = 0; i < self.ListIDNhanVienQuyen().length; i++) {
            if ($.inArray(self.ListIDNhanVienQuyen()[i], arrIDNhanVien) === -1) {
                arrIDNhanVien.push(self.ListIDNhanVienQuyen()[i]);
            }
        }

        // trang thai hoadon
        var statusInvoice = 1;
        if (self.TT_DaHuy()) {
            if (self.TT_HoanThanh()) {
                if (self.TT_TamLuu()) {
                    if (self.TT_GiaoHang()) {
                        statusInvoice = 0; // Huy + HoanThanh +TamLuu + GiaoHang (All)
                    }
                    else {
                        statusInvoice = 1; // Huy + HoanThanh +TamLuu
                    }

                } else {
                    if (self.TT_GiaoHang()) {
                        statusInvoice = 2; // Huy + HoanThanh + GiaoHang
                    }
                    else {
                        statusInvoice = 3; // Huy + HoanThanh
                    }
                }
            }
            else {
                if (self.TT_TamLuu()) {
                    if (self.TT_GiaoHang()) {
                        statusInvoice = 4; // Huy + TamLuu + GiaoHang
                    }
                    else {
                        statusInvoice = 5; // Huy + TamLuu
                    }
                }
                else {
                    if (self.TT_GiaoHang()) {
                        statusInvoice = 6; // Huy + GiaoHang
                    }
                    else {
                        statusInvoice = 7; // Huy
                    }
                }
            }
        }
        else {
            if (self.TT_HoanThanh()) {
                if (self.TT_TamLuu()) {
                    if (self.TT_GiaoHang()) {
                        statusInvoice = 8; // HoanThanh +TamLuu + GiaoHang
                    }
                    else {
                        statusInvoice = 9; // HoanThanh +TamLuu
                    }

                } else {
                    if (self.TT_GiaoHang()) {
                        statusInvoice = 10; // HoanThanh + GiaoHang
                    }
                    else {
                        statusInvoice = 11; //  HoanThanh
                    }
                }
            }
            else {
                if (self.TT_TamLuu()) {
                    if (self.TT_GiaoHang()) {
                        statusInvoice = 12; // TamLuu + GiaoHang
                    }
                    else {
                        statusInvoice = 13; // TamLuu
                    }
                }
                else {
                    if (self.TT_GiaoHang()) {
                        statusInvoice = 14; // GiaoHang
                    }
                    else {
                        statusInvoice = 15; // Khong check cai nao ca
                    }
                }
            }
        }

        // NgayLapHoaDon
        var _now = new Date();  //current date of week
        var currentWeekDay = _now.getDay();
        var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1;
        var dayStart = '';
        var dayEnd = '';
        var dateChose = '';

        if (self.filterNgayLapHD() === '0') {

            switch (self.filterNgayLapHD_Quy()) {
                case 0:
                    // all
                    self.TodayBC('Toàn thời gian');
                    dayStart = '2016-01-01';
                    dayEnd = moment(_now).add('days', 1).format('YYYY-MM-DD');
                    break;
                case 1:
                    // hom nay
                    self.TodayBC('Hôm nay');
                    dayStart = moment(_now).format('YYYY-MM-DD');
                    dayEnd = moment(_now).add('days', 1).format('YYYY-MM-DD');
                    break;
                case 2:
                    // hom qua
                    self.TodayBC('Hôm qua');
                    dayEnd = moment(_now).format('YYYY-MM-DD');
                    dayStart = moment(_now).subtract('days', 1).format('YYYY-MM-DD');
                    break;
                case 3:
                    // tuan nay
                    self.TodayBC('Tuần này');
                    dayStart = moment().startOf('week').add('days', 1).format('YYYY-MM-DD');
                    dayEnd = moment().endOf('week').add('days', 2).format('YYYY-MM-DD');
                    break;
                case 4:
                    // tuan truoc
                    self.TodayBC('Tuần trước');
                    dayStart = moment().weekday(-6).format('YYYY-MM-DD');
                    dayEnd = moment(dayStart, 'YYYY-MM-DD').add(7, 'days').format('YYYY-MM-DD'); // add day in moment.js
                    break;
                case 5:
                    // 7 ngay qua
                    self.TodayBC('7 ngày qua');
                    dayEnd = moment(_now).format('YYYY-MM-DD');
                    dayStart = moment(_now).subtract('days', 7).format('YYYY-MM-DD');
                    break;
                case 6:
                    // thang nay
                    self.TodayBC('Tháng này');
                    dayStart = moment().startOf('month').format('YYYY-MM-DD');
                    dayEnd = moment().endOf('month').add('days', 1).format('YYYY-MM-DD'); // add them 1 ngày 01-month-year --> compare in SQL
                    break;
                case 7:
                    // thang truoc
                    self.TodayBC('Tháng trước');
                    dayStart = moment().subtract('months', 1).startOf('month').format('YYYY-MM-DD');
                    dayEnd = moment().subtract('months', 1).endOf('month').add('days', 1).format('YYYY-MM-DD');
                    break;
                case 10:
                    // quy nay
                    self.TodayBC('Quý này');
                    dayStart = moment().startOf('quarter').format('YYYY-MM-DD');
                    dayEnd = moment().endOf('quarter').add('days', 1).format('YYYY-MM-DD');
                    break;
                case 11:
                    // quy truoc = currQuarter -1; // if (currQuarter -1 == 0) --> (assign = 1)
                    self.TodayBC('Quý trước');
                    var prevQuarter = moment().quarter() - 1;
                    if (prevQuarter === 0) {
                        // get quy 4 cua nam truoc 01/10/... -->  31/21/...
                        var prevYear = moment().year() - 1;
                        dayStart = prevYear + '-' + '10-01';
                        dayEnd = prevYear + '-' + '12-31';
                    }
                    else {
                        dayStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                        dayEnd = moment().quarter(prevQuarter).endOf('quarter').add('days', 1).format('YYYY-MM-DD');
                    }
                    break;
                case 12:
                    // nam nay
                    self.TodayBC('Năm này');
                    dayStart = moment().startOf('year').format('YYYY-MM-DD');
                    dayEnd = moment().endOf('year').add('days', 1).format('YYYY-MM-DD');
                    break;
                case 13:
                    // nam truoc
                    self.TodayBC('Năm trước');
                    var prevYear = moment().year() - 1;
                    dayStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                    dayEnd = moment().year(prevYear).endOf('year').add('days', 1).format('YYYY-MM-DD');
                    break;
            }
        }
        else {
            var arrDate = self.filterNgayLapHD_Input().split('-');

            dayStart = moment(arrDate[0], 'DD/MM/YYYY').format('YYYY-MM-DD');
            dayEnd = moment(arrDate[1], 'DD/MM/YYYY').add('days', 1).format('YYYY-MM-DD');

            self.TodayBC('Từ ' + moment(arrDate[0], 'DD/MM/YYYY').format('DD/MM/YYYY') + ' đến ' + moment(arrDate[1], 'DD/MM/YYYY').format('DD/MM/YYYY'));
        }

        var hasPermission = self.RoleView_ServicePackage();

        var Params_GetListHoaDon = {
            CurrentPage: self.currentPage(),
            PageSize: self.pageSize(),
            LoaiHoaDon: loaiHoaDon,
            MaHoaDon: txtMaHDon,
            MaHoaDonGoc: txtMaHDgoc,
            ID_ChiNhanhs: self.MangIDDV(),
            ID_ViTris: arrIDPB,
            ID_BangGias: arrIDBangGia,
            ID_NhanViens: arrIDNhanVien,
            NguoiTao: userLogin,
            TrangThai: statusInvoice,
            NgayTaoHD_TuNgay: dayStart,
            NgayTaoHD_DenNgay: dayEnd,
            TrangThai_SapXep: self.sort(),
            Cot_SapXep: self.columsort(),
            PTThanhToan: ptThuc,
            ColumnsHide: columnHide,
            ValueText: sTenChiNhanhs,
        }

        if (isExport) {
            $('.table-reponsive').gridLoader();
            ajaxHelper(BH_HoaDonUri + 'ExportExcel_GoiDichVu', 'POST', Params_GetListHoaDon).done(function (url) {
                $('.table-reponsive').gridLoader({ show: false });
                if (url !== "") {
                    self.DownloadFileTeamplateXLSX(url);
                }
            })

            var objDiary = {
                ID_NhanVien: _id_NhanVien,
                ID_DonVi: id_donvi,
                ChucNang: "Gói dịch vụ",
                NoiDung: 'Xuất excel danh sách gói dịch vụ',
                LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
            };
            Insert_NhatKyThaoTac_1Param(objDiary);
        }
        else {
            if (hasPermission) {
                // get list HoaDon
                //hidewait('content-table');
                $('.table-reponsive').gridLoader();
                ajaxHelper(BH_HoaDonUri + 'GetAllHoaDons_Where_PassObject', 'POST', Params_GetListHoaDon).done(function (data) {
                    //$("div[id ^= 'wait']").text("");
                    $('.table-reponsive').gridLoader({ show: false });

                    if (data !== null) {
                        self.HoaDons(data.lstCH);
                        self.TotalRecord(data.Rowcount);
                        self.PageCount(data.pageCount);

                        self.TongTienThue(data.TongTienThue);
                        self.TongTienHang(data.TongTienHang);
                        self.TongThanhToan(data.TongThanhToan);
                        self.TongGiamGia(data.TongGiamGia);
                        self.TongGiamGiaKM(data.TongGiamGiaKM);
                        self.TongKhachTra(data.TongKhachTra);
                        self.TongTienDoiDiem(data.TienDoiDiem);
                        self.TongTienTheGTri(data.ThuTuThe);

                        var mat = data.lstCH.reduce(function (_this, val) {
                            return _this + val.TienMat;
                        }, 0);
                        self.TongTienMat(mat);

                        var ck = data.lstCH.reduce(function (_this, val) {
                            return _this + val.ChuyenKhoan;
                        }, 0);
                        self.TongChuyenKhoan(ck);

                        var pos = data.lstCH.reduce(function (_this, val) {
                            return _this + val.TienATM;
                        }, 0);
                        self.TongPOS(pos);

                        // tinh tien khach no = Sum Conlai (at footer)
                        var lstHDHuy = $.grep(data.lstCH, function (x) {
                            return x.ChoThanhToan === null;
                        });
                        var sumKhachCanTra_HDHuy = lstHDHuy.reduce(function (_this, val) {
                            return _this + val.PhaiThanhToan;
                        }, 0);
                        // vi HD Huy: KhachCanTra >0, nhung KhachDaTra =0, nen khong tinh vao tien khach no (OK)
                        var conlai = data.TongThanhToan - data.TongKhachTra - sumKhachCanTra_HDHuy;
                        self.TongKhachNo(conlai);
                    }
                });
            }

            localStorage.removeItem('FindHD');
        }
    }

    self.Change_Stattus = function () {
        $("#iconSort").remove();
        ResetColumnSort();
        self.currentPage(0);
        SearchHoaDon(false, false);
    }

    self.filterNgayLapHD.subscribe(function (newVal) {
        $("#iconSort").remove();
        ResetColumnSort();
        self.currentPage(0);
        SearchHoaDon(false, false);
    });

    self.PageResults_HoaDon = ko.computed(function () {

        var first = self.currentPage() * self.pageSize();
        if (self.HoaDons() !== null) {
            return self.HoaDons().slice(first, first + self.pageSize());
        }
    });

    self.PageList_Display = ko.computed(function () {

        var arrPage = [];

        var allPage = self.PageCount();
        var currentPage = self.currentPage();

        if (allPage > 0) {
            if (allPage > 4) {

                var i = 0;
                if (currentPage === 0) {
                    i = parseInt(self.currentPage()) + 1;
                }
                else {
                    i = self.currentPage();
                }

                if (allPage >= 5 && currentPage > allPage - 5) {
                    if (currentPage >= allPage - 2) {
                        // get 5 trang cuoi cung
                        for (var i = allPage - 5; i < allPage; i++) {
                            var obj = {
                                pageNumber: i + 1,
                            };
                            arrPage.push(obj);
                        }
                    }
                    else {
                        // get currentPage - 2 , currentPage, currentPage + 2
                        if (currentPage == 1) {
                            for (var j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                                var obj = {
                                    pageNumber: j + 1,
                                };
                                arrPage.push(obj);
                            }
                        } else {
                            for (var j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                                var obj = {
                                    pageNumber: j + 1,
                                };
                                arrPage.push(obj);
                            }
                        }
                    }
                }
                else {
                    // get 5 trang dau
                    if (i >= 2) {
                        while (arrPage.length < 5) {
                            var obj = {
                                pageNumber: i - 1,
                            };
                            arrPage.push(obj);
                            i = i + 1;
                        }
                    }
                    else {
                        while (arrPage.length < 5) {
                            var obj = {
                                pageNumber: i,
                            };
                            arrPage.push(obj);
                            i = i + 1;
                        }
                    }
                }
            }
            else {
                // neu chi co 1 trang --> khong hien thi DS trang
                if (allPage > 1) {
                    for (var i = 0; i < allPage; i++) {
                        var obj = {
                            pageNumber: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
            }

            self.fromitem((self.currentPage() * self.pageSize()) + 1);
            if (((self.currentPage() + 1) * self.pageSize()) > self.HoaDons().length) {
                var fromItem = (self.currentPage() + 1) * self.pageSize();
                if (fromItem < self.TotalRecord()) {
                    self.toitem((self.currentPage() + 1) * self.pageSize());
                }
                else {
                    self.toitem(self.TotalRecord());
                }
            } else {
                self.toitem((self.currentPage() * self.pageSize()) + self.pageSize());
            }

            HideShowColumn();
            SetCheck_Input();
        }
        return arrPage;
    });

    self.VisibleStartPage = ko.computed(function () {
        if (self.PageList_Display().length > 0) {
            return self.PageList_Display()[0].pageNumber !== 1;
        }
    });

    self.VisibleEndPage = ko.computed(function () {
        if (self.PageList_Display().length > 0) {
            return self.PageList_Display()[self.PageList_Display().length - 1].pageNumber !== self.PageCount();
        }
    });

    self.GoToPageHD = function (page) {
        if (page.pageNumber !== '.') {
            self.currentPage(page.pageNumber - 1);
        }
    };

    function SetCheck_Input() {
        // find in list and set check
        var countCheck = 0;
        $('#tb tr td.check-group input').each(function (x) {
            var id = $(this).attr('id');
            if ($.inArray(id, arrIDCheck) > -1) {
                $(this).prop('checked', true);
                countCheck += 1;
            }
            else {
                $(this).prop('checked', false);
            }
        });

        // set again check header
        var ckHeader = $('#tb thead tr th:eq(0) input')
        if (countCheck == self.PageResults_HoaDon().length) {
            ckHeader.prop('checked', true);
        }
        else {
            ckHeader.prop('checked', false);
        }
    }

    self.StartPage = function () {
        self.currentPage(0);
    }

    self.BackPage = function () {
        if (self.currentPage() > 1) {
            self.currentPage(self.currentPage() - 1);
        }
    }

    self.GoToNextPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.currentPage() + 1);
        }
    }

    self.EndPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.PageCount() - 1);
        }
    }

    //sort by cột trong bảng hóa đơn
    $('#tb thead tr').on('click', 'th', function () {
        var id = $(this).attr('id');

        switch (id) {
            case "txtMaHoaDon":
                self.columsort("MaHoaDon");
                break;
            case "txtNgayCapNhat":
                self.columsort("NgayCapNhat");
                break;
            case "txtMaHoaDonGoc":
                self.columsort("MaHoaDonGoc");
                break;
            case "txtThoiGian":
                self.columsort("ThoiGian");
                break;
            case "txtKhachHang":
                self.columsort("KhachHang");
                break;
            case "txtEmail":
                self.columsort("Email");
                break;
            case "txtSoDT":
                self.columsort("SoDienThoai");
                break;
            case "txtDiaChi":
                self.columsort("DiaChi");
                break;
            case "txtKhuVuc":
                self.columsort("KhuVuc");
                break;
            case "txtPhuongXa":
                self.columsort("PhuongXa");
                break;
            case "txtNguoiBan":
                self.columsort("NguoiBan");
                break;
            case "txtNguoiTao":
                self.columsort("NguoiTao");
                break;
            case "txtGhiChu":
                self.columsort("GhiChu");
                break;
            case "txtTongTienHang":
                self.columsort("TongTienHang");
                break;
            case "txtGiamGia":
                self.columsort("GiamGia");
                break;
            case "txtKhachCanTra":
                self.columsort("KhachCanTra");
                break;
            case "txtThoiGianGiao":
                self.columsort("ThoiGianGiao");
                break;
            case "txtKhachDaTra":
                self.columsort("KhachDaTra");
                break;
            case "txtPhiTraHang":
                self.columsort("TongChiPhi");
                break;
        }

        SortGrid(id);
    });

    function SortGrid(item) {
        $("#iconSort").remove();
        if (self.sort() === 0) {
            self.sort(1);
            $('#' + item).append(' ' + "<i id='iconSort' class='fa fa-caret-down' aria-hidden='true'></i>");
        }
        else {
            self.sort(0);
            $('#' + item).append(' ' + "<i id='iconSort' class='fa fa-caret-up' aria-hidden='true'></i>");
        }
        SearchHoaDon(false, false);
    };

    self.gotoHoaDonGoc = function (item) {
        localStorage.setItem('FindHD', item.MaHoaDonGoc);
        window.location.href = '/#/Invoices';
    }

    self.gotoSoQuy = function (item) {
        localStorage.setItem('FindMaPhieuChi', item.MaPhieuChi);
        window.location.href = '/#/CashFlow';
    }

    self.gotoHoaDonTH = function (item) {
        switch (loaiHoaDon) {
            case 1:
                localStorage.setItem('FindHD', item.MaHoaDonGoc);
                window.location.href = '/#/Returns';
                break;
            case 6:
                // if click from HDDoiTra --> get MaHDparent
                localStorage.setItem('FindHD', self.MaHoaDonParent());
                window.location.href = '/#/Returns';
                break;
        }
    }

    self.gotoKhachHang = function (item, type) {
        switch (type) {
            case 1:
                localStorage.setItem('FindKhachHang', item.MaDoiTuong);
                window.open('/#/Customers', '_blank');
                break;
            case 3:
                if (!commonStatisJs.CheckNull(item.BienSo)) {
                    window.open('/#/DanhSachXe?' + item.BienSo, '_blank');
                }
                else {
                    self.LoadChiTietHD(item);
                }
                break;
        }
    }
    //===============================
    // triger khi đặt hàng thành công
    // Hóa đơn
    //===============================
    $("body").on('ChangeHoaDon', function () {
        SearchHoaDon(false, false);
    });
    //===============================
    // triger khi đặt hàng thành công
    // TraHang
    //===============================
    $("body").on('ChangeTraHang', function () {
        SearchHoaDon(false, false);
    });
    //===============================
    var columnHide = '';
    self.ExportExcel_HoaDon = function () {
        columnHide = '';
        // get list column hide
        var cacheHideColumn2 = localStorage.getItem(Key_Form);
        if (cacheHideColumn2 !== null) {
            cacheHideColumn2 = JSON.parse(cacheHideColumn2);

            var arrColumn = [];
            var tdClass = $('#tb thead tr th');
            for (var i = 0; i < cacheHideColumn2.length; i++) {
                var itemFor = cacheHideColumn2[i];
                if (itemFor.Value !== undefined) {
                    $(tdClass).each(function (index) {
                        var className = $(this).attr('class');
                        if (className !== undefined && className.indexOf(itemFor.Value) > -1) {
                            if ($.inArray(itemFor.Value, arrColumn) === -1) {
                                arrColumn.push(itemFor.Value);
                                columnHide += (index - 1) + '_';
                            }
                        }
                    })
                }
            }
        }
        if (!self.isGara()) {
            var lstColumn = columnHide.split('_');
            lstColumn = lstColumn.filter(x => x !== '');
            var lstAfter = [];
            for (let i = 0; i < lstColumn.length; i++) {
                let itFor = parseInt(lstColumn[i])
                if (itFor > 4) {
                    lstAfter.push(itFor + 1);
                }
                else {
                    lstAfter.push(itFor);
                }
            }

            columnHide = '';
            for (var i = 0; i < lstAfter.length; i++) {
                columnHide += lstAfter[i].toString() + '_';
            }
        }
        SearchHoaDon(false, true);
    }
    self.ExportExcel_ChiTietHoaDon = function (item) {
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: id_donvi,
            ChucNang: "Hóa đơn",
            NoiDung: "Xuất báo cáo hóa đơn chi tiết theo mã: " + item.MaHoaDon,
            LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        var myData = {};
        myData.objDiary = objDiary;
        $.ajax({
            url: DiaryUri + "post_NhatKySuDung",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: myData,
            success: function (item) {
            },
            statusCode: {
                404: function () {
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ghi nhật ký sử dụng thất bại", "danger");
            },
            complete: function () {
                var columnHide = '';
                if (self.ThietLap().LoHang === false) {
                    columnHide = '3'; // hide: LoHang
                }
                var url = BH_HoaDonUri + 'ExportExcel__ChiTietHoaDon?ID_HoaDon=' + item.ID + '&loaiHoaDon=' + loaiHoaDon + '&columHides=' + columnHide;
                window.location.href = url;
            }
        })
    }

    // import ton gdv
    self.visibleImport = ko.observable(true);
    self.fileNameExcel = ko.observable("Lựa chọn file Excel...");
    self.loiExcel = ko.observableArray();
    self.DanhSachGoiDV_Import = ko.observableArray();

    $(".filterFileSelect").hide();
    $(".btnImportExcel").hide();
    $(".BangBaoLoi").hide();

    self.DownloadFileTeamplateXLS = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_SoDuDauKyGoiDichVu.xls";
        window.open(url);
    }

    self.DownloadFileTeamplateXLSX_TonGoiDV = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_SoDuDauKyGoiDichVu.xlsx";
        window.open(url)
    }

    self.notvisibleImport = ko.computed(function () {
        return !self.visibleImport();
    });

    self.refreshFileSelect = function () {
        self.visibleImport(true);
        self.fileNameExcel("Lựa chọn file Excel...");
        $(".filterFileSelect").hide();
        $(".btnImportExcel").hide();
        document.getElementById('imageUploadForm').value = "";
    }

    self.SelectedFileImport = function (vm, evt) {
        if (evt.target.files.length > 0) {
            self.visibleImport(false);
            self.fileNameExcel(evt.target.files[0].name)
            $(".filterFileSelect").show();
            $(".btnImportExcel").show();
            $(".NoteImport").show();
            $(".BangBaoLoi").hide();
        }
    }

    self.ShowModalImportTonDV = function () {
        $('#modalImportTonGDV').modal('show');
    }

    self.ImportExcel_TonGDV = function (x) {
        $('.NoteImport').gridLoader();

        var typeExcel = ['application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', 'application/vnd.ms-excel'];
        var sError = '';

        var formData = new FormData();
        var totalFiles = document.getElementById("imageUploadForm").files.length;
        if (totalFiles === 0) {
            ShowMessage_Danger('Vui lòng chọn file để import');
            return false;
        }
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("imageUploadForm").files[i];
            if (file.type !== "" && $.inArray(file.type, typeExcel) === -1) {
                sError += file.name + ', ';
            }
            else {
                formData.append("imageUploadForm", file);
            }
        }

        if (sError !== '') {
            ShowMessage_Danger('File ' + Remove_LastComma(sError) + ' không đúng định dạng');
            $('.BangBaoLoi').gridLoader({ show: false });
            $('.NoteImport').gridLoader({ show: false });
            self.visibleImport(false);
            return false;
        }

        $.ajax({
            type: "POST",
            url: BH_HoaDonUri + "ImportExcel_TonGDV?idDonVi=" + id_donvi + '&idNhanVien=' + _id_NhanVien + '&nguoitao=' + userLogin,
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (x) {
                $('.BangBaoLoi').gridLoader({ show: false });
                $('.NoteImport').gridLoader({ show: false });
                if (x.res === false) {
                    if (x.mes === '') {
                        self.loiExcel(x.data);
                        self.visibleImport(true);
                        $(".BangBaoLoi").show();
                        $(".NoteImport").hide();
                        $(".filterFileSelect").hide();
                        $(".btnImportExcel").hide();
                    }
                    else {
                        ShowMessage_Danger(x.mes);
                    }
                }
                else {
                    ShowMessage_Success("Import tồn gói dịch vụ thành công");
                    document.getElementById('imageUploadForm').value = "";
                    self.visibleImport(true);
                    $(".NoteImport").show();
                    $(".filterFileSelect").hide();
                    $(".btnImportExcel").hide();
                    $(".BangBaoLoi").hide();
                    $("#modalImportTonGDV").modal("hide");
                    SearchHoaDon(false, false);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                ShowMessage_Danger('Import hóa đơn thất bại');
                $('.BangBaoLoi').gridLoader({ show: false });
                $('.NoteImport').gridLoader({ show: false });
            }
        })
    }

    // use enable/disable txtNgayLapHD, dropdown NVien
    self.ThayDoi_NgayLapHD = ko.observable(false);
    self.ThayDoi_NVienBan = ko.observable(false);

    self.LoadChiTietHD = function (item, e) {
        self.Enable_NgayLapHD(!VHeader.CheckKhoaSo(moment(item.NgayLapHoaDon).format('YYYY-MM-DD'), item.ID_DonVi));
        self.NgayLapHD_Update(undefined);

        self.filterHangHoa_ChiTietHD(undefined);

        $('.txtNgayLapHD').datetimepicker({
            timepicker: true,
            mask: true,
            format: 'd/m/Y H:i',
            maxDate: new Date(),
            onChangeDateTime: function (dp, $input) {
                self.NgayLapHD_Update($input.val());
                CheckNgayLapHD_format(self.NgayLapHD_Update(), 1, item.ID_DonVi);
            }
        });

        $('.txtNgayApDung').datetimepicker({
            timepicker: false,
            mask: true,
            format: 'd/m/Y',
            onChangeDateTime: function (dp, $input) {
                self.NgayApDung_Update($input.val());
            }
        });

        $('.txtNgayHetHan').datetimepicker({
            timepicker: false,
            mask: true,
            format: 'd/m/Y',
            onChangeDateTime: function (dp, $input) {
                self.NgayHetHan_Update($input.val());
                // Check format type
                CheckNgayLapHD_format(self.NgayHetHan_Update(), 3, item.ID_DonVi);

                // check ngayhethan < ngayapdung
                var ss = self.NgayApDung_Update();
                if (ss !== null && ss !== undefined) {
                    var ngayApdung = moment(self.NgayApDung_Update(), 'DD/MM/YYYY').format('YYYY-MM-DD');
                    if (self.NgayApDung_Update() === undefined) {
                        ngayApdung = moment(item.NgayApDungGoiDV).format('YYYY-MM-DD');
                    }
                    var ngayHetHan = moment($input.val(), 'DD/MM/YYYY').format('YYYY-MM-DD');
                    if (ngayHetHan < ngayApdung) {
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> '
                            + "Ngày hết hạn không được nhỏ hơn ngày áp dụng " + sLoai, "danger");
                        return false;
                    }
                }
            }
        });

        var $thiss = $(event.currentTarget).closest('tr').next().find('td').find('.op-object-detail').find('.loadcthd');
        var css = $(event.currentTarget).closest('tr').next().css('display');
        $(".op-js-tr-hide").hide();
        if (css === 'none') {
            $(event.currentTarget).closest('tr').next().toggle();
        }
        $thiss.gridLoader();
        self.BH_HoaDonChiTiets([]);
        ajaxHelper(BH_HoaDonUri + 'SP_GetChiTietHD_byIDHoaDon_ChietKhauNV?idHoaDon=' + item.ID, 'GET').done(function (data) {
            $thiss.gridLoader({
                show: false
            });
            if (data !== null) {
                var sluong = 0;
                for (var i = 0; i < data.length; i++) {
                    sluong += data[i].SoLuong;
                    if (data[i].MaHangHoa.indexOf('{DEL}') > -1) {
                        data[i].MaHangHoa = data[i].MaHangHoa.substr(0, data[i].MaHangHoa.length - 5);
                        data[i].Del = '{Xóa}';
                    } else {
                        data[i].Del = "";
                    }

                    // USE FOR SPA (add BH_NhanVienThucHien) (only BanHang, not (DatHang + TraHang))
                    let lstNV_TV = '';
                    let listBH_NVienThucHienOld = data[i].BH_NhanVienThucHien;

                    // remove BH_NhanVienThucHien old, and add again
                    data[i].BH_NhanVienThucHien = [];
                    for (var j = 0; j < listBH_NVienThucHienOld.length; j++) {
                        let itemFor = listBH_NVienThucHienOld[j];

                        // addNvienTuVan_ThucHien
                        let tienCK = itemFor.TienChietKhau;
                        let gtriPtramCK = itemFor.PT_ChietKhau;
                        let isPTram = gtriPtramCK > 0 ? true : false;
                        let gtriCK_TV = 0;

                        if (isPTram) {
                            gtriCK_TV = gtriPtramCK;
                            lstNV_TV += itemFor.TenNhanVien + ', ';
                        }
                        else {
                            gtriCK_TV = tienCK;
                            lstNV_TV += itemFor.TenNhanVien + ', ';
                        }

                        let idRandom = CreateIDRandom('CKNV_');
                        let itemNV = {
                            IDRandom: idRandom,
                            ID_NhanVien: itemFor.ID_NhanVien,
                            TenNhanVien: itemFor.TenNhanVien,
                            ThucHien_TuVan: false,
                            TacVu: 4,// ban goi DV
                            TheoYeuCau: false,
                            TienChietKhau: tienCK,
                            PT_ChietKhau: gtriPtramCK,
                            HeSo: itemFor.HeSo,
                            TinhChietKhauTheo: itemFor.TinhChietKhauTheo,
                            TinhHoaHongTruocCK: itemFor.TinhHoaHongTruocCK,
                        }
                        data[i].BH_NhanVienThucHien.push(itemNV);
                    }

                    data[i].GhiChu_NVThucHien = '';
                    data[i].GhiChu_NVThucHienPrint = '';
                    data[i].GhiChu_NVTuVan = (lstNV_TV === '' ? '' : '- Bán gói DV: ' + Remove_LastComma(lstNV_TV));
                    data[i].GhiChu_NVTuVanPrint = (lstNV_TV === '' ? '' : Remove_LastComma(lstNV_TV));
                }

                self.BH_HoaDonChiTiets(data);
                SetHeightShowDetail($(e.currentTarget));
                self.TongSLuong(sluong);

                var tonggiamgiahang = self.BH_HoaDonChiTiets().reduce(function (x, item) {
                    return x + (item.TienChietKhau * item.SoLuong);
                }, 0);
                var tongtienhangchuaCK = self.BH_HoaDonChiTiets().reduce(function (x, item) {
                    return x + (item.SoLuong * item.DonGia);
                }, 0);

                var arrHH = self.BH_HoaDonChiTiets().filter(x => x.LaHangHoa);
                var arrDV = self.BH_HoaDonChiTiets().filter(x => x.LaHangHoa === false);

                var tongDV = 0, tongDV_truocVAT = 0, tongDV_truocCK = 0;
                var tongHH = 0, tongHH_truocVAT = 0, tongHH_truocCK = 0;
                var DV_tongthue = 0, DV_tongCK = 0, DV_tongSL = 0;
                var HH_tongthue = 0, HH_tongCK = 0, HH_tongSL = 0;
                for (let k = 0; k < arrHH.length; k++) {
                    let itFor = arrHH[k];
                    let soluong = formatNumberToFloat(itFor.SoLuong);
                    HH_tongSL += soluong;
                    tongHH += formatNumberToFloat(itFor.ThanhToan);
                    tongHH_truocVAT += formatNumberToFloat(itFor.ThanhTien);
                    tongHH_truocCK += soluong * formatNumberToFloat(itFor.DonGia);
                    HH_tongthue += soluong * formatNumberToFloat(itFor.TienThue);
                    HH_tongCK += soluong * formatNumberToFloat(itFor.TienChietKhau);
                }
                for (let k = 0; k < arrDV.length; k++) {
                    let itFor = arrDV[k];
                    let soluong = formatNumberToFloat(itFor.SoLuong);
                    DV_tongSL += soluong;
                    tongDV += formatNumberToFloat(itFor.ThanhToan);
                    tongDV_truocVAT += formatNumberToFloat(itFor.ThanhTien);
                    tongDV_truocCK += soluong * formatNumberToFloat(itFor.DonGia);
                    DV_tongthue += soluong * formatNumberToFloat(itFor.TienThue);
                    DV_tongCK += soluong * formatNumberToFloat(itFor.TienChietKhau);
                }

                self.TongSoLuongHang(HH_tongSL + DV_tongSL);
                self.TongGiamGiaHang(tonggiamgiahang);
                self.TongTienHangChuaCK(tongtienhangchuaCK);

                self.TongTienPhuTung(tongHH);
                self.TongTienDichVu(tongDV);
                self.TongTienPhuTung_TruocCK(tongHH_truocCK);
                self.TongTienDichVu_TruocCK(tongDV_truocCK);
                self.TongTienPhuTung_TruocVAT(tongHH_truocVAT);
                self.TongTienDichVu_TruocVAT(tongDV_truocVAT);

                self.TongThue_PhuTung(HH_tongthue);
                self.TongCK_PhuTung(HH_tongCK);
                self.TongThue_DichVu(DV_tongthue);
                self.TongCK_DichVu(DV_tongCK);
                self.TongSL_DichVu(DV_tongSL);
                self.TongSL_PhuTung(HH_tongSL);
            }
            else {
                SetHeightShowDetail($(e.currentTarget));
                self.BH_HoaDonChiTiets(data);
            }
        });

        vmThanhPhanCombo.GetAllCombo_byIDHoaDon(item.ID);
        vmThanhToan.GetSoDuTheGiaTri(item.ID_DoiTuong);

        GetLichSuThanhToan(item.ID);

        var roleInsertQuy = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen.indexOf('SoQuy_ThemMoi') > -1;
        });

        if (self.RoleUpdate_ServicePackage()) {
            // role change NgayLapHD
            var roleChangeNgayLapHD = $.grep(self.Quyen_NguoiDung(), function (x) {
                return x.MaQuyen.indexOf('GoiDichVu_ThayDoiThoiGian') > -1;
            });

            // role change NVien ban
            var roleChangeNVien = $.grep(self.Quyen_NguoiDung(), function (x) {
                return x.MaQuyen.indexOf('GoiDichVu_ThayDoiNhanVien') > -1;
            });

            var changeNgayLapHD = roleChangeNgayLapHD.length > 0;
            self.ThayDoi_NgayLapHD(changeNgayLapHD);

            var changeNVien = roleChangeNVien.length > 0;
            self.ThayDoi_NVienBan(changeNVien);

            if (changeNgayLapHD || changeNVien) {
                self.Show_BtnUpdate(true);
            }
            else {
                self.Show_BtnUpdate(false);
            }

            if (item.ChoThanhToan === true) {
                var roleOpenHDTam = $.grep(self.Quyen_NguoiDung(), function (x) {
                    return x.MaQuyen.indexOf('HoaDon_CapNhatHDTamLuu') > -1;
                });
                if (roleOpenHDTam.length > 0) {
                    self.Show_BtnOpenHD(true);
                }
                else {
                    self.Show_BtnOpenHD(false);
                }
            }
            else {
                self.Show_BtnOpenHD(false);

            }
        }
        else {
            self.Show_BtnOpenHD(false);
        }

        // chi update neu HD khong tao từ HDTraHang
        if (CheckQuyenExist('GoiDichVu_SuaDoi') && item.ChoThanhToan === false && item.ID_HoaDon === null) {
            self.Show_BtnEdit(true);
        }
        else {
            self.Show_BtnEdit(false);
        }

        if (self.RoleExport_ServicePackage()) {
            self.Show_BtnExcelDetail(true);
        }
        else {
            self.Show_BtnExcelDetail(false);
        }

        if (self.RoleDelete_ServicePackage()) {
            if (item.ChoThanhToan != null) {
                self.Show_BtnDelete(true);
            }
            else {
                self.Show_BtnDelete(false);
            }
        }

        if (roleInsertQuy.length > 0) {
            if (item.PhaiThanhToan > item.KhachDaTra) {
                self.Show_BtnThanhToanCongNo(true);
            }
            else {
                self.Show_BtnThanhToanCongNo(false);
            }
        }
        else {
            self.Show_BtnThanhToanCongNo(false);
        }

        var roleCopy_ServicePackage = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen.indexOf('GoiDichVu_SaoChep') > -1;
        });

        if (self.RoleInsert_ServicePackage() && roleCopy_ServicePackage.length > 0) {
            self.Show_BtnCopy(true);
        }
        else {
            self.Show_BtnCopy(false);
        }

        ajaxHelper(BH_HoaDonUri + 'GetChietKhauNV_byIDHoaDon?idHoaDon=' + item.ID, 'GET').done(function (x) {
            if (x.res === true) {
                self.AllNhanVien_CKHoaDon(x.data);
                item.BH_NhanVienThucHiens = x.data;
            }
        });
    }

    self.Enable_NgayLapHD = ko.observable(true);

    function CheckNgayLapHD_format(valDate, loaiNgay, idChiNhanh = null) {
        // 1.NgayLapHD, 2.NgayApDung GoiDV, 3.NgayHetHan GoiDV

        var dateNow = moment(new Date()).format('YYYY-MM-DD HH:mm');
        var ngayLapHD = moment(valDate, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');

        if (valDate === '') {
            ShowMessage_Danger('Vui lòng nhập ngày lập ' + sLoai)
            return false;
        }

        if (valDate.indexOf('_') > -1) {
            ShowMessage_Danger('Ngày lập gói dịch vụ chưa đúng định dạng');
            return false;
        }

        switch (loaiNgay) {
            case 1:
                if (ngayLapHD > dateNow) {
                    ShowMessage_Danger('Ngày lập gói dịch vụ vượt quá thời gian hiện tại');
                    return false;
                }
                let chotSo = VHeader.CheckKhoaSo(moment(valDate, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD'), idChiNhanh);
                if (chotSo) {
                    ShowMessage_Danger('Ngày lập gói dịch vụ phải sau thời gian chốt sổ ' + VHeader.warning.ChotSo.NgayChotSo);
                    return false;
                }
                break;
            case 2:
            case 3:
                // not doing
                break;
        }
        return true;
    }

    function GetDataChotSo() {
        ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/GetDataChotSo?idChiNhanh=' + id_donvi, 'GET').done(function (data) {
            if (data !== null && data.length > 0) {
                self.ChotSo_ChiNhanh(data);
            }
            GetAll_LoHang();
        })
    }

    function GetLichSuThanhToan(idHoaDon) {
        // load data from Quy_HoaDon
        ajaxHelper(Quy_HoaDonUri + 'GetQuyHoaDon_byIDHoaDon?idHoaDon=' + idHoaDon, 'GET').done(function (data) {
            self.LichSuThanhToan(data);
        });
    }

    self.VisibleHisTraHang = ko.computed(function () {
        if (self.LichSuTraHang() === null) {
            return false;
        }
        else {
            return self.LichSuTraHang().length > 0;
        }
    })

    self.VisibleHis_HisHDofDH = ko.computed(function () {
        if (self.LichSuThanhToanDH() === null) {
            return false;
        }
        else {
            var count = 0;
            for (var i = 0; i < self.LichSuThanhToanDH().length; i++) {
                if (self.LichSuThanhToanDH()[i].LoaiHoaDon === 1) {
                    // Da tao HD from Dat hang
                    count += 1;
                }
            }
            if (count === 0) {
                return false;
            }
            else {
                return true;
            }
        }
    })

    self.VisibleHuyHD = ko.computed(function () {
        // if have TraHang --> hide btnHuyHoaDon
        if (self.LichSuTraHang() === null) {
            return true;
        }
        else {
            return false;
        }
    })

    self.VisibleHDDoiTra = ko.computed(function () {
        if (self.HoaDonDoiTra() === null) {
            return false;
        }
        else {
            return true;
        }
    })

    self.arrFilterBangGia = ko.computed(function () {
        var _filter = self.filterBangGia();
        return arrFilter = ko.utils.arrayFilter(self.GiaBans(), function (prod) {
            var chon = true;
            var arr = locdau(prod.TenGiaBan).split(/\s+/);
            var sSearch = '';

            for (var i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }

            if (chon && _filter) {
                chon = (locdau(prod.TenGiaBan).indexOf(locdau(_filter)) >= 0 ||
                    sSearch.indexOf(locdau(_filter)) >= 0
                );
            }
            return chon;
        });
    });

    self.arrFilterPhongBan = ko.computed(function () {
        var _filter = self.filterPhongBan();
        return arrFilter = ko.utils.arrayFilter(self.PhongBans(), function (prod) {
            var chon = true;
            var arr = locdau(prod.TenViTri).split(/\s+/);
            var sSearch = '';

            for (var i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }

            if (chon && _filter) {
                chon = (locdau(prod.TenViTri).indexOf(locdau(_filter)) >= 0 ||
                    sSearch.indexOf(locdau(_filter)) >= 0
                );
            }
            return chon;
        });
    });

    self.Change_LoaiMauIn = function (maChungTu) {
        dathangTeamplate = maChungTu;
        loadMauIn();
    }

    self.InHoaDon = function (item) {
        var cthdFormat = GetCTHDPrint_Format(self.BH_HoaDonChiTiets());
        self.CTHoaDonPrint(cthdFormat);

        var itemHDFormat = GetInforHDPrint(item, false);
        self.InforHDprintf(itemHDFormat);
        GetMauIn_ByMaLoaiChunghTu(dathangTeamplate);
    }

    function GetMauIn_ByMaLoaiChunghTu(maChungTu) {
        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=' + maChungTu + '&idDonVi=' + id_donvi,
            type: 'GET',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                var data = result;
                data = data.concat('<script src="/Scripts/knockout-3.4.2.js"></script>');
                data = data.concat("<script > var item4 =[], item5 =[]; var item1=" + JSON.stringify(self.CTHoaDonPrint())
                    + "; var item4=[], item5=[]; var item2=" + JSON.stringify(self.CTHoaDonPrintMH())
                    + ";var item3=" + JSON.stringify(self.InforHDprintf()) + "; </script>");
                data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>"); // MauInTeamplate.js: used to bind data in knockout
                PrintExtraReport(data); // assign content HTML into frame
            }
        });
    }

    self.InPhieuThu = function (item) {

        var temp = phieuThuTeamplate;
        if (item.LoaiHoaDon === 12) {
            temp = phieuChiTeamplate;
        }

        var itemHDFormat = GetInforPhieuThu(item);
        self.InforHDprintf(itemHDFormat);

        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=' + temp + '&idDonVi=' + id_donvi,
            type: 'GET',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                var data = result;
                data = data.concat('<script src="/Scripts/knockout-3.4.2.js"></script>');
                data = data.concat("<script > var item1=[] ; var item2=[], item4 =[], item5 =[] ;var item3=" + JSON.stringify(itemHDFormat) + "; </script>");
                data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                PrintExtraReport(data);
            }
        });
    }

    self.PrintMany = function () {
        // array contain HoaDon + CTHD
        var arrHoaDon = [];

        for (var i = 0; i < arrIDCheck.length; i++) {
            var itemHD = $.grep(self.HoaDons(), function (x) {
                return x.ID === arrIDCheck[i];
            });

            if (itemHD.length > 0) {
                // format infor HoaDon
                var itemHDPrint = GetInforHDPrint(itemHD[0], false);
                itemHDPrint.BH_HoaDon_ChiTiet = [];
                itemHDPrint.CTHoaDonPrintMH = [];
                arrHoaDon.push(itemHDPrint);
            }
        }

        ajaxHelper(BH_HoaDonUri + 'GetChiTietHD_MultipleHoaDon?arrID_HoaDon=' + arrIDCheck, 'GET').done(function (data) {
            if (data !== null) {
                for (let i = 0; i < arrHoaDon.length; i++) {
                    let hdFor = arrHoaDon[i];
                    let sumGiamGiaHang = 0, sumThanhTienTruocCK = 0, sumSoLuong = 0;
                    for (let j = 0; j < data.length; j++) {
                        if (data[j].ID_HoaDon === hdFor.ID) {
                            let ctFor = $.extend({}, true, data[j]);
                            let price = formatNumberToInt(ctFor.DonGia);
                            let sale = formatNumberToInt(ctFor.GiamGia);
                            let giaban = formatNumberToInt(ctFor.GiaBan);
                            let soluong = formatNumberToInt(ctFor.SoLuong);
                            sumGiamGiaHang += sale;
                            sumThanhTienTruocCK += price * soluong;
                            sumSoLuong += soluong;
                            data[j].DonGia = formatNumber(price);
                            data[j].TienChietKhau = formatNumber(sale * soluong);
                            data[j].GiaBan = formatNumber(giaban);
                            data[j].SoLuong = formatNumber(soluong);
                            data[j].ThanhTien = formatNumber(data[j].ThanhTien);
                            data[j].TenHangHoa = formatNumber(data[j].TenHangHoaFull);
                            data[j].ThanhTienTruocCK = formatNumber(soluong * price);
                            data[j].ThanhToan = formatNumber(ctFor.ThanhToan);
                            data[j].TienThue = formatNumber(ctFor.TienThue);

                            let nvTH = '', nvTV = '';
                            for (let k = 0; k < data[j].BH_NhanVienThucHien.length; k++) {
                                let nvien = data[j].BH_NhanVienThucHien[k];
                                if (nvien.ThucHien_TuVan) {
                                    nvTH += nvien.TenNhanVien + ', ';
                                }
                                else {
                                    nvTV += nvien.TenNhanVien + ', ';
                                }
                            }
                            data[j].GhiChu_NVThucHien = (nvTH === '' ? '' : '- Thực hiện: ' + Remove_LastComma(nvTH));
                            data[j].GhiChu_NVTuVan = (nvTV === '' ? '' : '- Tư vấn: ' + Remove_LastComma(nvTV));
                            data[j].GhiChu_NVThucHienPrint = (nvTH === '' ? '' : Remove_LastComma(nvTH));
                            data[j].GhiChu_NVTuVanPrint = (nvTV === '' ? '' : Remove_LastComma(nvTV));

                            arrHoaDon[i].BH_HoaDon_ChiTiet.push(data[j]);
                        }
                    }
                    arrHoaDon[i].TongGiamGiaHang = formatNumber(sumGiamGiaHang);
                    arrHoaDon[i].TongTienHangChuaCK = formatNumber(sumThanhTienTruocCK);
                    arrHoaDon[i].TongSoLuongHang = formatNumber(sumSoLuong);
                }

                $.ajax({
                    url: '/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=' + dathangTeamplate
                        + '&idDonVi=' + id_donvi + '&printMultiple=true',
                    type: 'GET',
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (result) {
                        var data = result;
                        data = data.concat('<script src="/Scripts/knockout-3.4.2.js"></script>');
                        data = data.concat('<script> function formatNumber(number) { ' +
                            'if (number === undefined || number === null) { ' +
                            ' return 0;' +
                            ' } ' +
                            ' else { ' +
                            '  return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","); ' +
                            '  } ' +
                            '   }</script>');
                        data = data.concat("<script > var item1=" + JSON.stringify(arrHoaDon) + "; </script>");
                        data = data.concat('<script> var dataMauIn = function () {' +
                            'var self = this;' +
                            'self.ListHoaDon_ChiTietHoaDonPrint = ko.observableArray(item1);' +
                            'self.Count_ListHoaDons = ko.computed(function () { ' +
                            'return self.ListHoaDon_ChiTietHoaDonPrint().length;' +
                            '})' +
                            '};' +
                            'ko.applyBindings(new dataMauIn()) </script>');
                        PrintExtraReport_Multiple(data);
                    }
                });
            }
        })
    }

    function GetInforPhieuThu(objHD) {
        objHD.TenCuaHang = self.CongTy()[0].TenCongTy;
        objHD.DiaChiCuaHang = self.CongTy()[0].DiaChi;
        objHD.LogoCuaHang = Open24FileManager.hostUrl + self.CongTy()[0].DiaChiNganHang;
        objHD.DienThoaiCuaHang = self.CongTy()[0].SoDienThoai;

        objHD.ChiNhanhBanHang = objHD.TenChiNhanh;
        objHD.DienThoaiChiNhanh = objHD.DienThoaiChiNhanh;

        objHD.MaPhieu = objHD.MaHoaDon;
        objHD.NgayLapHoaDon = moment(objHD.NgayLapHoaDon).format('DD/MM/YYYY HH:mm:ss');
        objHD.NguoiNopTien = objHD.NguoiNopTien;
        objHD.NguoiNhanTien = objHD.NguoiNopTien;
        objHD.DiaChiKhachHang = self.ItemHoaDon().DiaChiKhachHang;
        objHD.DienThoaiKhachHang = self.ItemHoaDon().DienThoai;
        objHD.NoiDungThu = objHD.NoiDungThu;
        objHD.TienBangChu = DocSo(formatNumberToInt(objHD.TongTienThu));
        objHD.GiaTriPhieu = formatNumber(objHD.TongTienThu);

        return objHD;
    }

    function GetInforHDPrint(objHD, isDoiTraHang) {
        var objPrint = $.extend({}, objHD);
        var phaiThanhToan = formatNumberToInt(objHD.PhaiThanhToan);
        var daThanhToan = formatNumberToInt(objHD.KhachDaTra);
        objPrint.TenNhaCungCap = objPrint.TenDoiTuong;
        objPrint.DienThoaiKhachHang = objPrint.DienThoai;
        objPrint.TongTichDiem = formatNumber(objPrint.DiemSauGD);
        objPrint.NhanVienBanHang = objPrint.TenNhanVien;
        if (objPrint.NgaySinh_NgayTLap != null) {
            objPrint.NgaySinh_NgayTLap = moment(NgaySinh_NgayTLap).format('DD/MM/YYYY');
        }

        var tongcong = formatNumberToInt(objPrint.TongTienHang) - formatNumberToInt(objPrint.TongGiamGia) + formatNumberToInt(objPrint.TongChiPhi);

        objPrint.NgayLapHoaDon = moment(objHD.NgayLapHoaDon).format('DD/MM/YYYY HH:mm:ss');
        objPrint.Ngay = moment(objHD.NgayLapHoaDon).format('DD');
        objPrint.Thang = moment(objHD.NgayLapHoaDon).format('MM');
        objPrint.Nam = moment(objHD.NgayLapHoaDon).format('YYYY');

        objPrint.TongTienHang = formatNumber(objPrint.TongTienHang);
        objPrint.PhaiThanhToan = formatNumber(phaiThanhToan);
        objPrint.DaThanhToan = formatNumber(daThanhToan);
        objPrint.DiemGiaoDich = formatNumber(objPrint.DiemGiaoDich);
        objPrint.TienThua = 0;

        objPrint.TongSoLuongHang = formatNumber(self.TongSoLuongHang());
        objPrint.TongGiamGiaHang = formatNumber(self.TongGiamGiaHang());
        objPrint.TongTienDichVu = formatNumber(self.TongTienDichVu());
        objPrint.TongTienDichVu_TruocVAT = formatNumber(self.TongTienDichVu_TruocVAT());
        objPrint.TongTienDichVu_TruocCK = formatNumber(self.TongTienDichVu_TruocCK());
        objPrint.TongTienPhuTung = formatNumber(self.TongTienPhuTung());
        objPrint.TongTienPhuTung_TruocVAT = formatNumber(self.TongTienPhuTung_TruocVAT());
        objPrint.TongTienPhuTung_TruocCK = formatNumber(self.TongTienPhuTung_TruocCK());
        objPrint.TongTienHangChuaCK = formatNumber(self.TongTienHangChuaCK());
        objPrint.TongGiamGiaHD_HH = formatNumber(self.TongGiamGiaHang() + objHD.TongGiamGia + objHD.KhuyeMai_GiamGia);
        objPrint.TongTienHDSauGiamGia = formatNumber(objPrint.TongTienHang - objPrint.TongGiamGiaKM_HD);

        objPrint.TongThue_PhuTung = self.TongThue_PhuTung();
        objPrint.TongCK_PhuTung = self.TongCK_PhuTung();
        objPrint.TongThue_DichVu = self.TongThue_DichVu();
        objPrint.TongCK_DichVu = self.TongCK_DichVu();
        objPrint.TongSL_DichVu = self.TongSL_DichVu();
        objPrint.TongSL_PhuTung = self.TongSL_PhuTung();

        if (isDoiTraHang) {
            // doi tra hang
            tongcong = tongcong - formatNumberToInt(objPrint.TongTienTraHang);

            // {TienTraKhach} --> PhaiTraKhach
            // {KhachCanTra} --> PhaiThanhToan
            // {DaThanhToan} --> DaThanhToan
            if (tongcong < 0) {
                // mua < tra
                objPrint.PhaiTraKhach = formatNumber(-tongcong);
                objPrint.TongCong = formatNumber(-tongcong);
                objPrint.PhaiThanhToan = 0;
            }
            else {
                // mua > tra --> khach phai tra
                objPrint.PhaiTraKhach = 0;
                objPrint.PhaiThanhToan = formatNumber(tongcong);
                objPrint.TongCong = formatNumber(tongcong);
            }
            objPrint.TongTienTraHang = formatNumber(objPrint.TongTienTraHang);
            objPrint.TongChiPhi = formatNumber(objPrint.PhiTraHang);
            objPrint.TongTienHoaDonMua = objPrint.TongTienHang;
        }
        else {
            // tra hang
            objPrint.TongTienTraHang = objPrint.TongTienHang;
            objPrint.TongChiPhi = formatNumber(objPrint.TongChiPhi);
            objPrint.TongCong = formatNumber(tongcong);
        }

        var conno = formatNumberToInt(objPrint.PhaiThanhToan) - daThanhToan;
        objPrint.NoTruoc = 0;
        objPrint.NoSau = formatNumber(conno);
        objPrint.TienKhachThieu = formatNumber(conno);
        objPrint.ChiPhiNhap = objPrint.TongChiPhi;
        objPrint.GhiChu = objPrint.DienGiai;
        objPrint.TienBangChu = DocSo(formatNumberToInt(objPrint.TongCong));

        if (self.CongTy().length > 0) {
            objPrint.LogoCuaHang = Open24FileManager.hostUrl + self.CongTy()[0].DiaChiNganHang;
            objPrint.TenCuaHang = self.CongTy()[0].TenCongTy;
            objPrint.DiaChiCuaHang = self.CongTy()[0].DiaChi;
            objPrint.DienThoaiCuaHang = self.CongTy()[0].SoDienThoai;
        }
        objPrint.TenChiNhanh = objPrint.TenDonVi; // chi nhanh ban hang

        objPrint.TienMat = formatNumber3Digit(objPrint.TienMat);
        objPrint.TienATM = formatNumber3Digit(objPrint.TienATM);
        objPrint.TongTaiKhoanThe = formatNumber(vmThanhToan.theGiaTriCus.TongNapThe);
        objPrint.TongSuDungThe = formatNumber(vmThanhToan.theGiaTriCus.SuDungThe);
        objPrint.SoDuConLai = formatNumber(vmThanhToan.theGiaTriCus.SoDuTheGiaTri);
        objPrint.TienDoiDiem = formatNumber(objPrint.TienDoiDiem);
        objPrint.TienTheGiaTri = formatNumber(objPrint.ThuTuThe);

        let pthuc = '';
        if (objHD.TienMat > 0) {
            pthuc += 'Tiền mặt, ';
        }
        if (objHD.TienATM > 0) {
            pthuc += 'POS, ';
        }
        if (objHD.ChuyenKhoan > 0) {
            pthuc += 'Chuyển khoản, ';
        }
        if (objHD.ThuTuThe > 0) {
            pthuc += 'Thẻ giá trị, ';
        }
        if (objHD.TienDoiDiem > 0) {
            pthuc += 'Điểm, ';
        }
        if (objHD.TienDatCoc > 0) {
            pthuc += 'Tiền cọc, ';
        }
        objPrint.PhuongThucTT = Remove_LastComma(pthuc);

        let nvHoaDon = '';
        let nvHoaDon_inCK = '';
        if (objPrint.BH_NhanVienThucHiens !== null && objPrint.BH_NhanVienThucHiens.length > 0) {
            for (let i = 0; i < objPrint.BH_NhanVienThucHiens.length; i++) {
                let nv = objPrint.BH_NhanVienThucHiens[i];
                nvHoaDon += nv.TenNhanVien + ', ';
                nvHoaDon_inCK += nv.TenNhanVien.concat(nv.PT_ChietKhau > 0 ? ' ('.concat(nv.PT_ChietKhau, ' %)') : ' ('.concat(formatNumber(nv.TienChietKhau), ')'), ', ');
            }
        }
        objPrint.ChietKhauNVHoaDon = Remove_LastComma(nvHoaDon);
        objPrint.ChietKhauNVHoaDon_InGtriCK = Remove_LastComma(nvHoaDon_inCK);
        return objPrint;
    }

    function GetInforCongTy() {
        ajaxHelper('/api/DanhMuc/HT_API/' + 'GetHT_CongTy', 'GET').done(function (data) {
            if (data != null) {
                self.CongTy(data);
            }
        });
    }

    function GetCTHDPrint_Format(arrCTHD) {
        var arr = [];
        var thuocTinh = '';
        for (var i = 0; i < arrCTHD.length; i++) {
            let itFor = $.extend({}, arrCTHD[i]);
            let price = formatNumberToInt(itFor.DonGia);
            let sale = formatNumberToInt(itFor.GiamGia);
            let giaban = formatNumberToInt(itFor.GiaBan);

            thuocTinh = arrCTHD[i].ThuocTinh_GiaTri;
            thuocTinh = thuocTinh == null || thuocTinh === '' ? '' : thuocTinh.substr(1);

            itFor.DonGia = formatNumber(price);
            itFor.TienChietKhau = formatNumber(sale);
            itFor.GiaBan = formatNumber(giaban);
            itFor.SoLuong = formatNumber3Digit(itFor.SoLuong);
            itFor.ThanhTien = formatNumber3Digit(itFor.ThanhTien);
            itFor.ThuocTinh_GiaTri = thuocTinh;

            // nvthuchien, tuvan co in %ck 
            let th_CoCK = '';
            let tv_CoCK = '';
            if (itFor.BH_NhanVienThucHien != null) {
                for (let j = 0; j < itFor.BH_NhanVienThucHien.length; j++) {
                    let for2 = itFor.BH_NhanVienThucHien[j];
                    if (for2.ThucHien_TuVan) {
                        th_CoCK += for2.TenNhanVien.concat(for2.PT_ChietKhau > 0 ? ' ('.concat(for2.PT_ChietKhau, ' %)') : ' ('.concat(formatNumber(for2.TienChietKhau), ')'), ', ');
                    }
                    else {
                        tv_CoCK += for2.TenNhanVien.concat(for2.PT_ChietKhau > 0 ? ' ('.concat(for2.PT_ChietKhau, ' %)') : ' ('.concat(formatNumber(for2.TienChietKhau), ')'), ', ');
                    }
                }
            }
            itFor.NVThucHienDV_CoCK = Remove_LastComma(th_CoCK);
            itFor.NVTuVanDV_CoCK = Remove_LastComma(tv_CoCK);

            let lstCombo = $.grep(vmThanhPhanCombo.AllComBo_ofHD, function (x) {
                return x.ID_ParentCombo === itFor.ID_ParentCombo;
            });
            if (lstCombo.length > 0) {
                itFor.ThanhPhanComBo = lstCombo;
                //itFor = AssignThanhPhanComBo_toCTHD(itFor);
            }
            else {
                itFor.ThanhPhanComBo = [];
            }
            arr.push(itFor);
        }
        return arr;
    }

    function AssignThanhPhanComBo_toCTHD(itemCT) {
        if (itemCT.ThanhPhanComBo !== null && itemCT.ThanhPhanComBo !== undefined) {
            for (let k = 0; k < itemCT.ThanhPhanComBo.length; k++) {
                let combo = itemCT.ThanhPhanComBo[k];
                combo.IDRandom = CreateIDRandom('combo_');
                combo = AssignNVThucHien_toCTHD(combo);
                combo = AssignTPDinhLuong_toCTHD(combo);
                let dateLot1 = GetNgaySX_NgayHH(combo);
                combo.NgaySanXuat = dateLot1.NgaySanXuat;
                combo.NgayHetHan = dateLot1.NgayHetHan;
                combo.LotParent = false;
                combo.DM_LoHang = [];

                combo.TongPhiDichVu = combo.PhiDichVu * combo.SoLuong;
                if (combo.LaPTPhiDichVu) {
                    combo.TongPhiDichVu = RoundDecimal(combo.PhiDichVu * combo.SoLuong * combo.DonGia / 100, 3);
                }

                combo.LoaiHoaDon = itemCT.LoaiHoaDon;
                combo.MaHoaDon = itemCT.MaHoaDon;
                combo.ID_ViTri = itemCT.ID_ViTri;
                combo.TenViTri = itemCT.TenPhongBan;

                combo.SoLuongMacDinh = itemCT.SoLuong === 0 ? combo.SoLuong : combo.SoLuong / itemCT.SoLuong;
                combo.SoLuongDaMua = 0;
                combo.CssWarning = false;
                combo.IsKhuyenMai = false;
                combo.IsOpeningKMai = false;
                combo.TenKhuyenMai = '';
                combo.HangHoa_KM = [];
                combo.UsingService = false;
                combo.ListDonViTinh = [];
                combo.ShowWarningQuyCach = false;
                combo.SoLuongQuyCach = 0;
                combo.HangCungLoais = [];
                combo.LaConCungLoai = false;
                combo.TimeStart = 0;
                combo.QuaThoiGian = 0;
                combo.TimeRemain = 0;
                combo.ThoiGian = moment(new Date()).format('YYYY-MM-DD HH:mm');
            }
        }
        else {
            itemCT.ThanhPhanComBo = [];
        }
        return itemCT;
    }

    self.AllLot = ko.observableArray();
    function GetAll_LoHang() {
        var timeChotSo = '2016-01-01';
        if (self.ChotSo_ChiNhanh().length > 0) {
            timeChotSo = self.ChotSo_ChiNhanh()[0].NgayChotSo;
        }
        if (navigator.onLine) {
            ajaxHelper(DMHangHoaUri + "SP_GetAll_DMLoHang?iddonvi=" + id_donvi + '&timeChotSo=' + timeChotSo, 'GET').done(function (data) {
                if (data !== null) {
                    self.AllLot(data);
                }
            });
        }
    }

    localStorage.removeItem('lcCTHDSaoChep');
    localStorage.removeItem('lcHDSaoChep');

    function AssignNVThucHien_toCTHD(itemCT) {
        var listBH_NVienThucHienOld = itemCT.BH_NhanVienThucHien;
        itemCT.BH_NhanVienThucHien = [];// reset BH_NhanVienThucHien old, and add again

        var lstNV_TH = '';
        var lstNV_TV = '';
        var lstNV_TH_Print = '';
        var lstNV_TV_Print = '';

        for (let j = 0; j < listBH_NVienThucHienOld.length; j++) {
            let itemFor = listBH_NVienThucHienOld[j];
            let isNVThucHien = itemFor.ThucHien_TuVan;
            let tienCK = itemFor.TienChietKhau;
            let gtriPtramCK = itemFor.PT_ChietKhau;
            let isPTram = gtriPtramCK > 0 ? true : false;

            let gtriCK_TH = 0;
            let gtriCK_TV = 0;
            let tacVu = 1;
            let ckMacDinh = gtriPtramCK;

            if (isNVThucHien) {
                if (itemFor.TheoYeuCau) {
                    tacVu = 3;  // thuchien theo yeucau
                }
                else {
                    tacVu = 1;
                }
                if (isPTram) {
                    gtriCK_TH = gtriPtramCK;
                    lstNV_TH += itemFor.TenNhanVien + ' (' + gtriCK_TH + ' %), ';
                }
                else {
                    gtriCK_TH = tienCK;
                    lstNV_TH += itemFor.TenNhanVien + ' (' + formatNumber(gtriCK_TH) + ' đ), ';
                    ckMacDinh = tienCK / itemFor.HeSo / itemCT.SoLuong;
                }
                lstNV_TH_Print += itemFor.TenNhanVien + ', ';
            }
            else {
                tacVu = 2;
                if (isPTram) {
                    gtriCK_TV = gtriPtramCK;
                    lstNV_TV += itemFor.TenNhanVien + ' (' + gtriCK_TV + ' %), ';
                }
                else {
                    gtriCK_TV = tienCK;
                    lstNV_TV += itemFor.TenNhanVien + ' (' + formatNumber(gtriCK_TV) + ' đ), ';
                    ckMacDinh = tienCK / itemFor.HeSo / itemCT.SoLuong;
                }
                lstNV_TV_Print += itemFor.TenNhanVien + ', ';
            }

            let idRandom = CreateIDRandom('IDRandomCK_');
            let itemNV = {
                IDRandom: idRandom,
                ID_NhanVien: itemFor.ID_NhanVien,
                TenNhanVien: itemFor.TenNhanVien,
                ThucHien_TuVan: isNVThucHien,
                TienChietKhau: tienCK,
                PT_ChietKhau: gtriPtramCK,
                TheoYeuCau: itemFor.TheoYeuCau,
                TacVu: tacVu,
                HeSo: itemFor.HeSo,
                TinhChietKhauTheo: itemFor.TinhChietKhauTheo,
                ChietKhauMacDinh: ckMacDinh,
                TinhHoaHongTruocCK: itemFor.TinhHoaHongTruocCK,
            }
            itemCT.BH_NhanVienThucHien.push(itemNV);
        }

        itemCT.GhiChu_NVThucHien = (lstNV_TH === '' ? '' : 'Thực hiện: ' + Remove_LastComma(lstNV_TH));
        itemCT.GhiChu_NVThucHienPrint = (lstNV_TH_Print === '' ? '' : Remove_LastComma(lstNV_TH_Print));
        itemCT.GhiChu_NVTuVan = (lstNV_TV === '' ? '' : 'Tư vấn: ' + Remove_LastComma(lstNV_TV));
        itemCT.GhiChu_NVTuVanPrint = (lstNV_TV_Print === '' ? '' : Remove_LastComma(lstNV_TV_Print));
        itemCT.HoaHongTruocChietKhau =
            listBH_NVienThucHienOld != null && listBH_NVienThucHienOld.length > 0 ?
                listBH_NVienThucHienOld[0].TinhHoaHongTruocCK : itemCT.HoaHongTruocChietKhau;
        return itemCT;
    }

    function AssignTPDinhLuong_toCTHD(itemCT) {
        if (itemCT.ThanhPhan_DinhLuong !== null && itemCT.ThanhPhan_DinhLuong !== undefined) {
            for (let k = 0; k < itemCT.ThanhPhan_DinhLuong.length; k++) {
                let tpDL = itemCT.ThanhPhan_DinhLuong[k];
                itemCT.ThanhPhan_DinhLuong[k].STT = k + 1;
                itemCT.ThanhPhan_DinhLuong[k].isDefault = false;
                itemCT.ThanhPhan_DinhLuong[k].IDRandom = CreateIDRandom('TPDL_');
                itemCT.ThanhPhan_DinhLuong[k].SoLuongQuyCach = tpDL.SoLuong * tpDL.QuyCach;
                itemCT.ThanhPhan_DinhLuong[k].SoLuongDinhLuong_BanDau = tpDL.SoLuong / itemCT.SoLuong;
                itemCT.ThanhPhan_DinhLuong[k].SoLuongMacDinh = itemCT.ThanhPhan_DinhLuong[k].SoLuongDinhLuong_BanDau; // assign = SoLuongDinhLuong_BanDau
                itemCT.ThanhPhan_DinhLuong[k].GiaVonAfter = tpDL.SoLuong * tpDL.GiaVon;
            }
        }
        else {
            itemCT.ThanhPhan_DinhLuong = [];
        }
        return itemCT;
    }

    function GetPTThue_PTChietKhauHang(item) {
        var ptCKHang = 0;
        var arrCTsort = self.BH_HoaDonChiTiets();
        var arr = $.grep(arrCTsort, function (x) {
            return x.PTChietKhau === arrCTsort[0].PTChietKhau;
        });
        if (arr.length === arrCTsort.length) {
            ptCKHang = arrCTsort[0].PTChietKhau;
        }
        return {
            PTChietKhauHH: ptCKHang,
            PTThueHD: item.PTThueHoaDon,
        }
    }

    self.SaoChepHD = function (item, type) {
        var newHD = $.extend({}, item);
        var phaiTT = formatNumberToFloat(newHD.PhaiThanhToan);
        var khachdatra = formatNumberToFloat(newHD.KhachDaTra);
        let diemquydoi = 0;
        let tiendoidiem = 0;
        if (self.ThietLap_TichDiem() !== null && self.ThietLap_TichDiem().length > 0) {
            tiendoidiem = newHD.TienDoiDiem;
            diemquydoi = Math.floor(tiendoidiem * self.ThietLap_TichDiem()[0].DiemThanhToan / self.ThietLap_TichDiem()[0].TienThanhToan);
        }
        var maHD = 'Copy' + newHD.MaHoaDon;
        switch (type) {
            case 0:// saochep
                newHD.KhachDaTra = 0;
                newHD.DaThanhToan = newHD.PhaiThanhToan;
                newHD.TTBangDiem = tiendoidiem;
                newHD.DiemQuyDoi = diemquydoi;
                newHD.DiemGiaoDichDB = 0;
                newHD.TrangThaiHD = 1;
                newHD.TienGui = newHD.ChuyenKhoan;
                newHD.TienTheGiaTri = newHD.ThuTuThe;
                SetCacheHD_CTHD(newHD, maHD);
                if (self.isGara()) {
                    localStorage.setItem('gara_CreateFrom', 'TN_copyHD');
                }
                else {
                    localStorage.setItem('createHDfrom', 6);
                }
                break;
            case 1:// update HDTamLuu
                maHD = newHD.MaHoaDon;
                newHD.TrangThaiHD = 3;
                newHD.TTBangDiem = 0;
                newHD.DiemQuyDoi = 0;
                newHD.DiemGiaoDichDB = 0;
                newHD.DaThanhToan = phaiTT - khachdatra; // số tiền còn lại phaiTT --> bind at BanHang
                newHD.TienMat = newHD.DaThanhToan;
                newHD.TienATM = 0;
                newHD.TienGui = 0;
                newHD.TienTheGiaTri = 0;
                newHD.ID_TaiKhoanPos = null;
                newHD.ID_TaiKhoanChuyenKhoan = null;
                SetCacheHD_CTHD(newHD, maHD);
                if (self.isGara()) {
                    localStorage.setItem('gara_CreateFrom', 'TN_updateHD');
                }
                else {
                    localStorage.setItem('createHDfrom', 7);
                }
                break;
            case 2:// updateHD daTT
                ajaxHelper(BH_HoaDonUri + 'ServicePackage_CheckUsed?idHoaDon=' + newHD.ID, 'GET').done(function (x) {
                    if (x === true) {
                        ShowMessage_Danger('Gói dịch vụ đã được sử dụng');
                        return false;
                    }
                    maHD = newHD.MaHoaDon;
                    newHD.TrangThaiHD = 8;
                    newHD.TTBangDiem = 0;
                    newHD.DiemQuyDoi = 0;
                    newHD.DiemGiaoDichDB = newHD.DiemGiaoDich; // tru diem giaodich HD cu
                    newHD.DaThanhToan = phaiTT - khachdatra;
                    newHD.TienMat = newHD.DaThanhToan; // = số tiền còn lại phaiTT
                    newHD.TienATM = 0;
                    newHD.TienGui = 0;
                    newHD.TienTheGiaTri = 0;
                    newHD.ID_TaiKhoanPos = null;
                    newHD.ID_TaiKhoanChuyenKhoan = null;
                    SetCacheHD_CTHD(newHD, maHD);
                    if (self.isGara()) {
                        localStorage.setItem('gara_CreateFrom', 'TN_updateHD');
                    }
                    else {
                        localStorage.setItem('createHDfrom', 8);
                    }
                });
                break;
        }
    }

    function SetDefaultPropertiesCTHD(itemCT, mahoadon) {
        itemCT.MaHoaDon = mahoadon;
        itemCT.ChatLieu = '';
        itemCT.LoaiHoaDon = 19;
        itemCT.SrcImage = null;
        itemCT.CssWarning = false;
        itemCT.IsKhuyenMai = false;
        itemCT.IsOpeningKMai = false;
        itemCT.TenKhuyenMai = '';
        itemCT.HangHoa_KM = [];
        itemCT.DiemKhuyenMai = 0;
        itemCT.ID_ChiTietDinhLuong = null;
        itemCT.ThanhPhanComBo = [];
        itemCT.UsingService = false;
        itemCT.ListDonViTinh = [];
        itemCT.ShowEditQuyCach = false;
        itemCT.ShowWarningQuyCach = false;
        itemCT.SoLuongQuyCach = 0;
        itemCT.HangCungLoais = [];
        itemCT.LaConCungLoai = false;
        itemCT.ID_ViTri = null;
        itemCT.TenViTri = '';
        itemCT.TimeStart = 0;
        itemCT.QuaThoiGian = 0;
        itemCT.TimeRemain = 0;
        itemCT.ThoiGian = moment(new Date()).format('YYYY-MM-DD HH:mm');
        itemCT.ThoiGianThucHien = 0;
        return itemCT;
    }

    function GetNgaySX_NgayHH(ctDoing) {
        var ngaysx = ctDoing.NgaySanXuat;
        if (!commonStatisJs.CheckNull(ngaysx)) {
            ngaysx = moment(ngaysx).format('DD/MM/YYYY');
        }
        var hansd = ctDoing.NgayHetHan;
        if (!commonStatisJs.CheckNull(hansd)) {
            hansd = moment(hansd).format('DD/MM/YYYY');
        }
        return {
            NgaySanXuat: ngaysx,
            NgayHetHan: hansd,
        }
    }

    function SetCacheHD_CTHD(item, maHD) {
        var note_KMaiHD = '';
        var lstKMCTHD = localStorage.getItem('productKM_HoaDon');
        if (lstKMCTHD !== null) {
            lstKMCTHD = JSON.parse(lstKMCTHD);
        }
        else {
            lstKMCTHD = [];
        }
        // remove item old in KhuyenMai if same MaHoaDon && add new
        lstKMCTHD = $.grep(lstKMCTHD, function (x) {
            return x.MaHoaDon !== maHD;
        });

        if (self.BH_HoaDonChiTiets() !== null) {
            var giamgiaKM_HD = item.KhuyeMai_GiamGia;

            var obj = GetPTThue_PTChietKhauHang(item);
            item.PTChietKhauHH = obj.PTChietKhauHH;
            item.TongTienHangChuaCK = self.TongTienHangChuaCK();
            item.TongGiamGiaHang = self.TongGiamGiaHang();

            item.IsSaoChep = true;
            item.MaHoaDonDB = maHD;
            item.IsHDDatHang = false; // assign IsHDDatHang = false --> deleteNCC at BanLe_TraHang
            item.Status = 1;
            item.LoaiHoaDon = loaiHoaDon;
            item.ChoThanhToan = false;
            item.YeuCau = 1;
            item.StatusOffline = false;
            item.DVTinhGiam = 'VND';
            if (item.TongChietKhau > 0) {
                item.DVTinhGiam = '%';
            }
            item.TongGiaGocHangTra = 0;
            item.TongTienTra = 0;
            item.PTGiamDB = 0;
            item.TongGiamGiaDB = 0;
            item.PhaiThanhToanDB = 0;
            item.HoanTraTamUng = 0;
            item.IsKhuyenMaiHD = false;
            item.IsOpeningKMaiHD = false;
            item.KhuyeMai_GiamGia = 0;
            item.TongGiamGiaKM_HD = item.TongGiamGia + item.KhuyeMai_GiamGia;

            item.DiemHienTai = 0;
            item.DiemKhuyenMai = 0; 
            item.DiemCong = 0;
            item.CreateTime = 0;
            item.ID_ViTri = null;
            item.TenViTriHD = '';
            item.PTThueDB = 0;
            item.TongThueDB = 0;
            // apply giam gia theo nhom
            item.ID_NhomDTApplySale = null;
            item.ThoiGianThucHien = 0;
            item.IsActive = '';
            item.TongTienKhuyenMai_CT = '';
            item.TongGiamGiaKhuyenMai_CT = '';
            item.TongChiPhiHangTra = 0;

            // get chietkhau nv hoadon
            for (let k = 0; k < item.BH_NhanVienThucHiens.length; k++) {
                item.BH_NhanVienThucHiens[k].IDRandom = CreateIDRandom('CKHD_');
                item.BH_NhanVienThucHiens[k].ChietKhauMacDinh = item.BH_NhanVienThucHiens[k].PT_ChietKhau;
                if (item.BH_NhanVienThucHiens[k].TinhChietKhauTheo === 3)
                    item.BH_NhanVienThucHiens[k].ChietKhauMacDinh = item.BH_NhanVienThucHiens[k].TienChietKhau / item.BH_NhanVienThucHiens[k].HeSo;
            }

            // order by SoThuTu ASC --> group Hang Hoa by LoHang
            var arrCTsort = self.BH_HoaDonChiTiets().sort(function (a, b) {
                var x = a.SoThuTu,
                    y = b.SoThuTu;
                return x < y ? -1 : x > y ? 1 : 0;
            });

            var arrIDQuiDoi = [];
            var cthdLoHang = [];

            for (let i = 0; i < arrCTsort.length; i++) {
                let cthd = $.extend({}, arrCTsort[i]);
                let idKhuyenMai = cthd.ID_KhuyenMai;

                // if TangKem of HangHoa thì ID_KhuyenMai == null --> not push CTHD
                if (cthd.ID_TangKem !== null) {
                    if (idKhuyenMai === null || idKhuyenMai === const_GuidEmpty) {
                        continue;
                    }
                }

                cthd = SetDefaultPropertiesCTHD(cthd, item.MaHoaDon);
                cthd.SoLuongDaMua = 0;
                cthd.TienChietKhau = cthd.GiamGia;
                cthd.MaHoaDon = item.MaHoaDon;
                cthd.DVTinhGiam = '%';
                cthd.GiaBan = cthd.DonGia;
                cthd.ThanhTien = (cthd.GiaBan - cthd.TienChietKhau) * cthd.SoLuong;
                if (cthd.TienChietKhau > 0 && cthd.PTChietKhau === 0) {
                    cthd.DVTinhGiam = 'VND';
                }
                cthd.ID_ChiTietGoiDV = null;

                // PhiDichVu, LaPTPhiDichVu (get from DB store)
                cthd.TongPhiDichVu = 0;
                //cthd.TongPhiDichVu = cthd.SoLuong * cthd.PhiDichVu;
                //if (cthd.LaPTPhiDichVu) {
                //    cthd.TongPhiDichVu = Math.round(cthd.SoLuong * cthd.GiaBan / 100);
                //}

                // lo hang
                var quanLiTheoLo = cthd.QuanLyTheoLoHang;
                cthd.DM_LoHang = [];
                cthd.LotParent = quanLiTheoLo;

                let dateLot = GetNgaySX_NgayHH(cthd);
                cthd.NgaySanXuat = dateLot.NgaySanXuat;
                cthd.NgayHetHan = dateLot.NgayHetHan;

                // get tpcombo
                let combo = $.grep(vmThanhPhanCombo.AllComBo_ofHD, function (x) {
                    return x.ID_ParentCombo === cthd.ID_ParentCombo;
                });
                if (combo.length > 0) {
                    for (let k = 0; k < combo.length; k++) {
                        combo[k].IDRandom = CreateIDRandom('combo_');
                        combo[k] = AssignNVThucHien_toCTHD(combo[k]);
                        combo[k] = AssignTPDinhLuong_toCTHD(combo[k]);

                        let dateLot1 = GetNgaySX_NgayHH(combo[k]);
                        combo[k].NgaySanXuat = dateLot1.NgaySanXuat;
                        combo[k].NgayHetHan = dateLot1.NgayHetHan;
                        combo[k].LotParent = false;
                        combo[k].DM_LoHang = [];

                        combo[k].LoaiHoaDon = cthd.LoaiHoaDon;
                        combo[k].MaHoaDon = cthd.MaHoaDon;
                        combo[k].ID_ViTri = cthd.ID_ViTri;
                        combo[k].TenViTri = cthd.TenPhongBan;

                        combo[k].SoLuongMacDinh = cthd.SoLuong === 0 ? combo[k].SoLuong : combo[k].SoLuong / cthd.SoLuong;
                        combo[k].SoLuongDaMua = 0;
                        combo[k].CssWarning = false;
                        combo[k].IsKhuyenMai = false;
                        combo[k].IsOpeningKMai = false;
                        combo[k].TenKhuyenMai = '';
                        combo[k].HangHoa_KM = [];
                        combo[k].UsingService = false;
                        combo[k].ListDonViTinh = [];
                        combo[k].ShowWarningQuyCach = false;
                        combo[k].SoLuongQuyCach = 0;
                        combo[k].HangCungLoais = [];
                        combo[k].LaConCungLoai = false;
                        combo[k].TimeStart = 0;
                        combo[k].QuaThoiGian = 0;
                        combo[k].TimeRemain = 0;
                        combo[k].ThoiGian = moment(new Date()).format('YYYY-MM-DD HH:mm');
                    }
                }
                cthd.ThanhPhanComBo = combo;
                cthd = AssignNVThucHien_toCTHD(cthd);
                cthd = AssignTPDinhLuong_toCTHD(cthd);

                // check KhuyenMai
                if (idKhuyenMai !== null && idKhuyenMai !== '00000000-0000-0000-0000-000000000000') {
                    let itemKM = $.grep(self.KM_KMApDung(), function (x) {
                        return x.ID_KhuyenMai === idKhuyenMai;
                    });
                    if (self.ThietLap().KhuyenMai === true && CheckKM_IsApDung(item.ID_NhanVien) && itemKM.length > 0) {
                        // if TangKem of HoaDon (ID_TangKem = null, ID_KhuyenMai !=null)
                        // if TangKem of HangHoa (ID_TangKem = ID_QuiDoi of HangTang, ID_KhuyenMai == null)
                        // save cache KhuyenMai of HoaDon
                        if (cthd.TangKem && $.inArray(itemKM[0].HinhThuc, [11, 12, 13, 14]) > -1) {// neu khuyenmai hoadon
                            // if was push KhuyenMai HoaDon --> not push 
                            for (let m = 0; m < lstKMCTHD.length; m++) {
                                if (lstKMCTHD[m].ID_KhuyenMai === idKhuyenMai && lstKMCTHD[m].MaHoaDon === cthd.MaHoaDon) {
                                    continue;
                                }
                            }
                            // find all Hang KhuyenMai of HoaDon
                            var hangTangHoaDon = $.grep(arrCTsort, function (x) {
                                return x.ID_KhuyenMai === idKhuyenMai;
                            });
                            var exitsKM = $.grep(lstKMCTHD, function (x) {
                                return x.ID_KhuyenMai === item.ID_KhuyenMai;
                            })
                            if (exitsKM.length === 0) {
                                let noteDetail = '';
                                for (let m = 0; m < hangTangHoaDon.length; m++) {
                                    // assign proprties hangTangHoaDon
                                    hangTangHoaDon[m] = SetDefaultPropertiesCTHD(hangTangHoaDon[m], maHD);
                                    hangTangHoaDon[m].SoLuongDaMua = 0;
                                    hangTangHoaDon[m].TienChietKhau = hangTangHoaDon[m].GiamGia;
                                    hangTangHoaDon[m].DVTinhGiam = '%';
                                    hangTangHoaDon[m].GiaBan = hangTangHoaDon[m].DonGia;
                                    hangTangHoaDon[m].ThanhTien = (hangTangHoaDon[m].GiaBan - hangTangHoaDon[m].TienChietKhau) * hangTangHoaDon[m].SoLuong;
                                    if (hangTangHoaDon[m].TienChietKhau > 0 && hangTangHoaDon[m].PTChietKhau === 0) {
                                        hangTangHoaDon[m].DVTinhGiam = 'VND';
                                    }
                                    hangTangHoaDon[m].CssWarning = false;
                                    hangTangHoaDon[m].ID_ChiTietGoiDV = null;
                                    hangTangHoaDon[m].TongPhiDichVu = 0;
                                    hangTangHoaDon[m].PhiDichVu = 0;
                                    hangTangHoaDon[m].LaPTPhiDichVu = false;
                                    // lo hang
                                    quanLiTheoLo = false;
                                    idLoHang = hangTangHoaDon[m].ID_LoHang;
                                    if (idLoHang !== null && idLoHang !== '00000000-0000-0000-0000-000000000000') {
                                        quanLiTheoLo = true;
                                        itemLot = $.grep(self.AllLot(), function (x) {
                                            return x.ID === idLoHang;
                                        });
                                    }
                                    hangTangHoaDon[m].QuanLyTheoLoHang = quanLiTheoLo;
                                    hangTangHoaDon[m].DM_LoHang = [];
                                    hangTangHoaDon[m].ID_LoHang = idLoHang;
                                    hangTangHoaDon[m].LotParent = quanLiTheoLo;
                                    noteDetail += hangTangHoaDon[m].SoLuong + ' ' + hangTangHoaDon[m].MaHangHoa + ', ';
                                    lstKMCTHD.push(hangTangHoaDon[m]);
                                    localStorage.setItem('productKM_HoaDon', JSON.stringify(lstKMCTHD))
                                }
                                // find CTKhuyenMai was ApDung in this HoaDon
                                let tongTienHang = 0;
                                let isGiamGiaPT = 0;
                                let gtriGiamGia = 0;
                                // sort DM_KhuyenMai_ChiTiet by TongTienHang (get DM gan nhat voi TongTienHang)
                                itemKM[0].DM_KhuyenMai_ChiTiet = itemKM[0].DM_KhuyenMai_ChiTiet.sort(function (a, b) {
                                    var x = a.TongTienHang, y = b.TongTienHang;
                                    return x > y ? 1 : x < y ? -1 : 0;
                                });
                                for (let j = 0; j < itemKM[0].DM_KhuyenMai_ChiTiet.length; j++) {
                                    if (itemKM[0].DM_KhuyenMai_ChiTiet[j].TongTienHang <= item.TongTienHang) {
                                        gtriGiamGia = itemKM[0].DM_KhuyenMai_ChiTiet[j].GiamGia;
                                        tongTienHang = itemKM[0].DM_KhuyenMai_ChiTiet[j].TongTienHang;
                                        isGiamGiaPT = itemKM[0].DM_KhuyenMai_ChiTiet[j].GiamGiaTheoPhanTram;
                                    }
                                }
                                note_KMaiHD = itemKM[0].TenKhuyenMai + ': Tổng tiền hàng từ ' + formatNumber(tongTienHang);
                                switch (itemKM[0].HinhThuc) {
                                    case 12:// tang hang
                                        note_KMaiHD += ' tặng ' + Remove_LastComma(noteDetail);
                                        break;
                                    case 13:// giam gia hang
                                        item.KhuyeMai_GiamGia = giamgiaKM_HD;
                                        note_KMaiHD += ' giảm giá '.concat(isGiamGiaPT ? gtriGiamGia + ' %' : formatNumber(gtriGiamGia) + ' Đ', ' cho ', Remove_LastComma(noteDetail));
                                        break;
                                }
                                // update infor cache HoaDon when apply KhuyenMai
                                item.KhuyenMai_GhiChu = note_KMaiHD;
                                item.IsKhuyenMaiHD = true;
                                item.IsOpeningKMaiHD = true;
                            }
                            continue; // not add hangtangHoaDon in cache CTHD
                        }
                        else {
                            // find hang tang kem of this HangHoa
                            let hhTangKem = $.grep(arrCTsort, function (x) {
                                return x.ID_TangKem === cthd.ID_DonViQuiDoi;
                            });
                            var txtKhuyenMai_Last = '';
                            // kmai theo hanghoa or diemcong
                            if (hhTangKem.length > 0 || cthd.DiemKhuyenMai > 0) {
                                // chi add neu khong phai khuyenmai diemcong
                                if (cthd.TangKem === false && cthd.DiemKhuyenMai === 0) {
                                    // get all HangTang of this HangHoa
                                    for (let m = 0; m < hhTangKem.length; m++) {
                                        // assign proprties hhTangKem
                                        hhTangKem[m] = SetDefaultPropertiesCTHD(hhTangKem[m], item.MaHoaDon);
                                        hhTangKem[m].SoLuongDaMua = 0;
                                        hhTangKem[m].TienChietKhau = hhTangKem[m].GiamGia;
                                        hhTangKem[m].DVTinhGiam = '%';
                                        hhTangKem[m].GiaBan = hhTangKem[m].DonGia;
                                        hhTangKem[m].ThanhTien = (hhTangKem[m].GiaBan - hhTangKem[m].TienChietKhau) * hhTangKem[m].SoLuong;
                                        if (hhTangKem[m].TienChietKhau > 0 && hhTangKem[m].PTChietKhau === 0) {
                                            hhTangKem[m].DVTinhGiam = 'VND';
                                        }
                                        hhTangKem[m].CssWarning = false;
                                        hhTangKem[m].ID_ChiTietGoiDV = null;
                                        hhTangKem[m].TongPhiDichVu = 0;
                                        hhTangKem[m].PhiDichVu = 0;
                                        hhTangKem[m].LaPTPhiDichVu = false;
                                        // lo hang
                                        quanLiTheoLo = false;
                                        idLoHang = hhTangKem[m].ID_LoHang;
                                        if (idLoHang !== null && idLoHang !== '00000000-0000-0000-0000-000000000000') {
                                            quanLiTheoLo = true;
                                            itemLot = $.grep(self.AllLot(), function (x) {
                                                return x.ID === idLoHang;
                                            });
                                        }
                                        hhTangKem[m].QuanLyTheoLoHang = quanLiTheoLo;
                                        hhTangKem[m].DM_LoHang = [];
                                        hhTangKem[m].ID_LoHang = idLoHang;
                                        hhTangKem[m].LotParent = quanLiTheoLo;
                                        cthd.HangHoa_KM.push(hhTangKem[m]);
                                        // VD: 3 hang 001, 1 hang 002
                                        txtKhuyenMai_Last += hhTangKem[m].SoLuong + ' ' + hhTangKem[m].MaHangHoa + ', ';
                                    }
                                    txtKhuyenMai_Last = Remove_LastComma(txtKhuyenMai_Last);
                                }

                                // find all Hang with same ID_KhuyenMai
                                let cthd_sameKM = $.grep(arrCTsort, function (x) {
                                    return x.ID_KhuyenMai === idKhuyenMai;
                                });
                                let textKM_First = '';
                                for (let k = 0; k < cthd_sameKM.length; k++) {
                                    textKM_First += cthd_sameKM[k].SoLuong + ' ' + cthd_sameKM[k].MaHangHoa + ', ';
                                }
                                textKM_First = 'Khi mua ' + Remove_LastComma(textKM_First);
                                // set ghichu KhuyenMai for CTHD
                                let isGiamGiaPTram = false;
                                let gtriGiamGia = 0;
                                let giaKhuyenMai = 0;
                                let loaiKM_note = '';
                                let soluongMua_ThucTe = 0;
                                let itemCTApDung = [];
                                let arrNhomChilds = [];
                                // sort DM_KhuyenMai_ChiTiet by SoLuongMua (get DM gan nhat voi SoLuongMua)
                                itemKM[0].DM_KhuyenMai_ChiTiet = itemKM[0].DM_KhuyenMai_ChiTiet.sort(function (a, b) {
                                    var x = a.SoLuongMua, y = b.SoLuongMua;
                                    return x > y ? 1 : x < y ? -1 : 0;
                                });

                                for (let k = 0; k < itemKM[0].DM_KhuyenMai_ChiTiet.length; k++) {
                                    var itemCT = itemKM[0].DM_KhuyenMai_ChiTiet[k];
                                    // check ID_QuiDoi Mua thuoc KMai {ID_QuiDoiMua OR ID_NhomHHMua }
                                    arrNhomChilds = GetAll_IDNhomChild_ofNhomHH([itemCT.ID_NhomHangHoaMua]);
                                    if ($.inArray(cthd.ID_DonViQuiDoi, itemKM[0].ID_QuyDoiMuas) > -1
                                        || $.inArray(cthd.ID_NhomHangHoa, arrNhomChilds) > -1) {
                                        // if Kmai by ID_QuiDoi
                                        if (cthd.ID_DonViQuiDoi === itemCT.ID_DonViQuiDoiMua) {
                                            //isTangNhom = false;
                                            itemCTApDung = itemCT;
                                        }
                                        else {
                                            // Kmai by Nhom
                                            itemCTApDung = itemCT;
                                        }
                                        if (itemCTApDung != []) {
                                            break;
                                        }
                                    }
                                }
                                if (itemCTApDung != []) {
                                    isGiamGiaPTram = itemCTApDung.GiamGiaTheoPhanTram;
                                    gtriGiamGia = itemCTApDung.GiamGia;
                                    giaKhuyenMai = itemCTApDung.GiaKhuyenMai;
                                    isGiamGiaPTram = itemCTApDung.GiamGiaTheoPhanTram;
                                    // KMai by Nhom
                                    if (itemCTApDung.ID_NhomHangHoaMua !== null) {
                                        // get all CTHD thuoc nhom KM --> tinh soluong mua
                                        for (let n = 0; n < arrCTsort.length; n++) {
                                            if ($.inArray(arrCTsort[n].ID_NhomHangHoa, arrNhomChilds) > -1) {
                                                soluongMua_ThucTe += arrCTsort[n].SoLuong;
                                            }
                                        }
                                    }
                                    else {
                                        // Kmai by ID_QuiDoi
                                        soluongMua_ThucTe = cthd.SoLuong;
                                    }
                                }
                                loaiKM_note = textKM_First;
                                switch (itemKM[0].HinhThuc) {
                                    case 21:
                                        if (isGiamGiaPTram) {
                                            loaiKM_note += ' giảm giá ' + gtriGiamGia + '% cho ' + txtKhuyenMai_Last;
                                        }
                                        else {
                                            loaiKM_note += ' giảm giá ' + gtriGiamGia + ' cho ' + txtKhuyenMai_Last;
                                        }
                                        break;
                                    case 22:
                                        loaiKM_note += ' tặng ' + txtKhuyenMai_Last;
                                        break;
                                    case 23:
                                        if (isGiamGiaPTram) {
                                            loaiKM_note += ' tặng ' + gtriGiamGia + '% điểm ';
                                        }
                                        else {
                                            // nhân theo Soluong Mua (chi tinh Soluong tai thoi diem mua hien tai)
                                            gtriGiamGia = Math.floor(gtriGiamGia * Math.floor(soluongMua_ThucTe / itemCTApDung.SoLuongMua));
                                            loaiKM_note += ' tặng ' + gtriGiamGia + ' điểm ';
                                        }
                                        break;
                                    case 24:
                                        loaiKM_note += ' giá ' + formatNumber(giaKhuyenMai);
                                        break;
                                }
                                // if hanghoa same ID_KhuyenMai --> neu da add KMai: not assign IsOpeningKMai, ISKhuyenMai = true
                                var existKM = $.grep(arrCTsort, function (x) {
                                    return x.ID_KhuyenMai === idKhuyenMai && x.IsKhuyenMai === true;
                                });
                                if (existKM.length === 0) {
                                    cthd.IsOpeningKMai = true;
                                    cthd.IsKhuyenMai = true;
                                    cthd.TenKhuyenMai = itemKM[0].TenKhuyenMai + ': ' + loaiKM_note;
                                }
                            }// end if hangTangKem > 0
                        } // end else KM HangHoa
                    } // end if IsApDung
                }
                // check exist in cthdLoHang
                if ($.inArray(cthd.ID_DonViQuiDoi, arrIDQuiDoi) === -1) {
                    arrIDQuiDoi.unshift(cthd.ID_DonViQuiDoi);
                    cthd.SoThuTu = cthdLoHang.length + 1;
                    cthd.IDRandom = CreateIDRandom('RandomCT_');
                    if (quanLiTheoLo) {
                        let objLot = $.extend({}, cthd);
                        objLot.HangCungLoais = [];
                        objLot.DM_LoHang = [];
                        cthd.DM_LoHang.push(objLot);
                    }
                    cthdLoHang.push(cthd);
                }
                else {
                    // find in cthdLoHang with same ID_QuiDoi
                    for (let j = 0; j < cthdLoHang.length; j++) {
                        if (cthdLoHang[j].ID_DonViQuiDoi === cthd.ID_DonViQuiDoi) {
                            if (quanLiTheoLo) {
                                let objLot = $.extend({}, cthd);
                                objLot.LotParent = false;
                                objLot.HangCungLoais = [];
                                objLot.DM_LoHang = [];
                                objLot.IDRandom = CreateIDRandom('RandomCT_');;
                                cthdLoHang[j].DM_LoHang.push(objLot);
                            }
                            else {
                                cthd.IDRandom = CreateIDRandom('RandomCT_');
                                cthd.LaConCungLoai = true;
                                cthdLoHang[j].HangCungLoais.push(cthd);
                            }
                            break;
                        }
                    }
                }
            }

            // sort CTHD by SoThuTu desc
            cthdLoHang = cthdLoHang.sort(function (a, b) {
                var x = a.SoThuTu, y = b.SoThuTu;
                return x < y ? 1 : x > y ? -1 : 0;
            });

            // khuyenMai: hinhthuc 11, 14: giam gia HD, cong diem HD (todo 14: congdiem)
            if (item.ID_KhuyenMai !== null && item.ID_KhuyenMai !== '00000000-0000-0000-0000-000000000000') {
                let itemKM = $.grep(self.KM_KMApDung(), function (x) {
                    return x.ID_KhuyenMai === item.ID_KhuyenMai;
                });
                let exitsKM2 = $.grep(lstKMCTHD, function (x) {
                    return x.ID_KhuyenMai === item.ID_KhuyenMai;
                })
                // if not exist KM in hangKM _HoaDon
                if (exitsKM2.length === 0 && self.ThietLap().KhuyenMai === true && CheckKM_IsApDung(item.ID_NhanVien) && itemKM.length > 0) {
                    // find CTKhuyenMai was ApDung in this HoaDon
                    let tongTienHang = 0;
                    let isGiamGiaPT = 0;
                    let gtriGiamGia = 0;
                    // sort DM_KhuyenMai_ChiTiet by TongTienHang (get DM gan nhat voi TongTienHang)
                    itemKM[0].DM_KhuyenMai_ChiTiet = itemKM[0].DM_KhuyenMai_ChiTiet.sort(function (a, b) {
                        var x = a.TongTienHang, y = b.TongTienHang;
                        return x > y ? 1 : x < y ? -1 : 0;
                    });
                    for (let j = 0; j < itemKM[0].DM_KhuyenMai_ChiTiet.length; j++) {
                        if (itemKM[0].DM_KhuyenMai_ChiTiet[j].TongTienHang <= item.TongTienHang) {
                            gtriGiamGia = itemKM[0].DM_KhuyenMai_ChiTiet[j].GiamGia;
                            tongTienHang = itemKM[0].DM_KhuyenMai_ChiTiet[j].TongTienHang;
                            isGiamGiaPT = itemKM[0].DM_KhuyenMai_ChiTiet[j].GiamGiaTheoPhanTram;
                        }
                    }
                    note_KMaiHD = itemKM[0].TenKhuyenMai + ': Tổng tiền hàng từ ' + formatNumber(tongTienHang);
                    switch (itemKM[0].HinhThuc) {
                        case 11:
                            note_KMaiHD += ' giảm giá ' + gtriGiamGia + ' ' + (isGiamGiaPT ? '%' : 'Đ') + ' cho hóa đơn';
                            break;
                        case 14:
                            note_KMaiHD += ' tặng ' + gtriGiamGia + ' ' + (isGiamGiaPT ? ' % điểm' : ' điểm') + ' cho hóa đơn';
                            break;
                    }
                    // update infor cache HoaDon when apply KhuyenMai
                    item.KhuyenMai_GhiChu = note_KMaiHD;
                    item.IsKhuyenMaiHD = true;
                    item.IsOpeningKMaiHD = true;
                    item.KhuyeMai_GiamGia = giamgiaKM_HD;
                }
            }
            delete item['BH_HoaDon_ChiTiet'];

            localStorage.setItem('lcCTHDSaoChep', JSON.stringify(cthdLoHang));
            localStorage.setItem('lcHDSaoChep', JSON.stringify(item));

            window.open(url, '_blank');
        }
        else {
            ShowMessage_Danger('Không có chi tiết hóa đơn')
            return false;
        }
    }


    self.showPopThanhToan = function (item) {
        item.SoDuTheGiaTri = vmThanhToan.theGiaTriCus.SoDuTheGiaTri;
        if (self.CongTy().length > 0) {
            vmThanhToan.inforCongTy = {
                TenCongTy: self.CongTy()[0].TenCongTy,
                DiaChiCuaHang: self.CongTy()[0].DiaChi,
                LogoCuaHang: Open24FileManager.hostUrl + self.CongTy()[0].DiaChiNganHang,
                TenChiNhanh: VHeader.TenDonVi,
            };
        }
        vmThanhToan.showModalThanhToan(item);
    }

    $('#txtNgayTaoInput').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        SearchHoaDon(false, false);
    });

    function GetDM_TaiKhoanNganHang() {
        if (navigator.onLine) {
            ajaxHelper(Quy_HoaDonUri + 'GetAllTaiKhoanNganHang_ByDonVi?idDonVi=' + id_donvi, 'GET').done(function (x) {
                if (x.res === true) {
                    vmThanhToan.listData.AccountBanks = x.data;
                }
            })
        }
    }

    function GetAllQuy_KhoanThuChi() {
        ajaxHelper(Quy_HoaDonUri + 'GetQuy_KhoanThuChi', 'GET').done(function (x) {
            if (x.res === true) {
                vmThanhToan.listData.KhoanThuChis = x.data;
            }
        })
    }

    self.formatDateTime = function () {
        $('.datepicker_mask').datetimepicker(
            {
                format: "d/m/Y H:i",
                mask: true,
                timepicker: false,
            });
    }

    // Check quyen user
    function HideShowButton_HoaDon() {
        var arrRoleHD = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen.indexOf('GoiDichVu_') > -1;
        });

        var itemView = $.grep(arrRoleHD, function (x) {
            return x.MaQuyen.indexOf('GoiDichVu_XemDS') > -1;
        });

        if (itemView.length > 0) {
            self.RoleView_ServicePackage(true);
            $('#btnViewCheck').show();
            $('.bangchung').show();
            $('#myList').css('display', '');
            SearchHoaDon(false, false);
        }
        else {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + 'Không có quyền xem danh sách ' + sLoai, 'danger');
        }

        // them moi
        var itemInsert = $.grep(arrRoleHD, function (x) {
            return x.MaQuyen.indexOf('GoiDichVu_ThemMoi') > -1;
        });

        if (self.RoleView_ServicePackage() && itemInsert.length > 0) {
            self.RoleInsert_ServicePackage(true);
            $('.clickbanhang').css('display', 'block');
        }

        // cap nhat
        var itemUpdate = $.grep(arrRoleHD, function (x) {
            return x.MaQuyen.indexOf('GoiDichVu_CapNhat') > -1;
        });

        if (self.RoleView_ServicePackage() && itemUpdate.length > 0) {
            self.RoleUpdate_ServicePackage(true);
        }

        // xoa
        var itemDelete = $.grep(arrRoleHD, function (x) {
            return x.MaQuyen.indexOf('GoiDichVu_Xoa') > -1;
        });

        if (self.RoleView_ServicePackage() && itemDelete.length > 0) {
            self.RoleDelete_ServicePackage(true);
        }
        // xuat file
        var itemExport = $.grep(arrRoleHD, function (x) {
            return x.MaQuyen.indexOf('GoiDichVu_XuatFile') > -1;
        });

        if (self.RoleView_ServicePackage() && itemExport.length > 0) {
            self.RoleExport_ServicePackage(true);
            $('#btnExport').show();
        }
    }

    self.ListTypeMauIn = ko.observableArray();

    function loadMauIn() {
        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/GetListMauIn?typeChungTu=' + dathangTeamplate + '&idDonVi=' + id_donvi,
            type: 'GET',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                self.ListTypeMauIn(result);
            }
        });

    }
    self.PrinDatHang = function (item, key) {
        var cthdFormat = GetCTHDPrint_Format(self.BH_HoaDonChiTiets());
        self.CTHoaDonPrint(cthdFormat);

        var itemHDFormat = GetInforHDPrint(item, false);
        self.InforHDprintf(itemHDFormat);

        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/GetContentFIlePrint?idMauIn=' + key,
            type: 'GET',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                var data = result;
                data = data.concat('<script src="/Scripts/knockout-3.4.2.js"></script>');
                data = data.concat("<script > var item1=" + JSON.stringify(self.CTHoaDonPrint())
                    + "; var item4=[], item5=[]; var item2=" + JSON.stringify(self.CTHoaDonPrintMH())
                    + ";var item3=" + JSON.stringify(itemHDFormat) + "; </script>");
                data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                PrintExtraReport(data);
            }
        });
    }

    // check format date
    const isCorrectFormat = (dateString, format) => {
        return moment(dateString, format, true).isValid()
    }

    // auto update NhomKhach(TraHang --> nâng nhóm , HuyHD --> hạ nhóm)
    self.DM_NhomDoiTuong_ChiTiets = ko.observableArray();

    function GetDM_NhomDoiTuong_ChiTiets() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/DM_NhomDoiTuongAPI/' + 'GetDM_NhomDoiTuong_ChiTiets?idDonVi=' + id_donvi, 'GET').done(function (x) {
                let data = x.data;
                if (data.length > 0) {
                    self.DM_NhomDoiTuong_ChiTiets(data);
                }
            });
        }
    }

    function UpdateNhom_KhachHang(itemHD) {

        if (itemHD.ID_DoiTuong !== null && itemHD.ID_DoiTuong !== undefined) {

            // get arr DM_NhomDoiTuong_ChiTiets with condition 1,2,3,4,5
            var arrNhom12345 = $.grep(self.DM_NhomDoiTuong_ChiTiets(), function (x) {
                return x.LoaiDieuKien < 6;
            });

            // get list ID_NhomDoiTuong in DM_NhomDoiTuong_ChiTiets (contain duplicate)
            var arrID_NhomDT = [];
            for (var i = 0; i < arrNhom12345.length; i++) {
                arrID_NhomDT.push(arrNhom12345[i].ID_NhomDoiTuong);
            }
            // return array Json {value: ID_NhomDoiTuong, count: number}
            var arrJson = compressArray(arrID_NhomDT);

            // get infor MuaHang of KH from DB by ID
            var itemDT = [];
            var wasChotSo = self.ChotSo_ChiNhanh().length > 0;
            var timeEnd = moment(new Date()).add('days', 1).format('YYYY-MM-DD');
            //ajaxHelper(DMDoiTuongUri + 'GetInforMuaHang_ofKhachHang?idDoiTuong=' + itemHD.ID_DoiTuong, 'GET').done(function (data) {
            ajaxHelper(DMDoiTuongUri + 'GetInforKhachHang_ByID?idDoiTuong=' + itemHD.ID_DoiTuong + '&idChiNhanh=' + id_donvi
                + '&timeStart=' + '2016-01-01' + '&timeEnd=' + timeEnd
                + '&wasChotSo=' + wasChotSo, 'GET').done(function (data) {

                    if (data !== null) {
                        itemDT = data;

                        // * HuyHD:
                        // 1. TongBanTruTraHang =  TongBanTruTraHang - PhaiTT 
                        // 2. TongBan = TongBan - PhaiTT
                        // 4. SoLanMua = SoLanMua - 1;
                        if (itemHD.LoaiHoaDon === 1) {
                            itemDT[0].TongBanTruTraHang = itemDT[0].TongBanTruTraHang - itemHD.PhaiThanhToan;
                            itemDT[0].TongBan = itemDT[0].TongBan - itemHD.PhaiThanhToan;
                            itemDT[0].SoLanMuaHang = itemDT[0].SoLanMuaHang - 1;
                        }

                        // * HuyTraHang
                        // 1. TongBanTruTraHang =  TongBanTruTraHang + PhaiTT 
                        // 2. TongBan = TongBan + PhaiTT
                        if (itemHD.LoaiHoaDon === 6) {
                            itemDT[0].TongBanTruTraHang = itemDT[0].TongBanTruTraHang + itemHD.PhaiThanhToan;
                            itemDT[0].TongBan = itemDT[0].TongBan + itemHD.PhaiThanhToan;
                        }

                        // ThanhToan No
                        if (itemHD.LoaiHoaDon === 11) {
                            itemDT[0].NoHienTai = itemDT[0].NoHienTai - formatNumberToInt(itemHD.TongTienThu);
                            if (itemDT[0].NoHienTai < 0) {
                                itemDT[0].NoHienTai = 0;
                            }
                        }

                        if (itemDT.length > 0) {
                            var arrNhom = []; // mang chua tat cac nhom thoa man DK (contain duplicate)

                            // LoaiDieuKien = 1: (TongBanTruTraHang)
                            var nhomDK1 = $.grep(arrNhom12345, function (x) {
                                return x.LoaiDieuKien === 1 && x.TuDongCapNhat === true;
                            });

                            for (var i = 0; i < nhomDK1.length; i++) {
                                switch (nhomDK1[i].LoaiSoSanh) {
                                    case 1: // >
                                        if (itemDT[0].TongBanTruTraHang > nhomDK1[i].GiaTriSo) {
                                            arrNhom.push(nhomDK1[i].ID_NhomDoiTuong);
                                        }
                                        break;
                                    case 2: // >=
                                        if (itemDT[0].TongBanTruTraHang >= nhomDK1[i].GiaTriSo) {
                                            arrNhom.push(nhomDK1[i].ID_NhomDoiTuong);
                                        }
                                        break;
                                    case 3: // =
                                        if (itemDT[0].TongBanTruTraHang === nhomDK1[i].GiaTriSo) {
                                            arrNhom.push(nhomDK1[i].ID_NhomDoiTuong);
                                        }
                                        break;
                                    case 5: // <
                                        if (itemDT[0].TongBanTruTraHang < nhomDK1[i].GiaTriSo) {
                                            arrNhom.push(nhomDK1[i].ID_NhomDoiTuong);
                                        }
                                        break;
                                    case 4: // <=
                                        if (itemDT[0].TongBanTruTraHang <= nhomDK1[i].GiaTriSo) {
                                            arrNhom.push(nhomDK1[i].ID_NhomDoiTuong);
                                        }
                                        break;
                                }
                            }

                            // LoaiDieuKien = 2  (TongBan)
                            var nhomDK2 = $.grep(arrNhom12345, function (x) {
                                return x.LoaiDieuKien === 2 && x.TuDongCapNhat === true;
                            });

                            for (var i = 0; i < nhomDK2.length; i++) {
                                switch (nhomDK2[i].LoaiSoSanh) {
                                    case 1: // >
                                        if (itemDT[0].TongBan > nhomDK2[i].GiaTriSo) {
                                            arrNhom.push(nhomDK2[i].ID_NhomDoiTuong);
                                        }
                                        break;
                                    case 2: // >=
                                        if (itemDT[0].TongBan >= nhomDK2[i].GiaTriSo) {
                                            arrNhom.push(nhomDK2[i].ID_NhomDoiTuong);
                                        }
                                        break;
                                    case 3: // =
                                        if (itemDT[0].TongBan === nhomDK2[i].GiaTriSo) {
                                            arrNhom.push(nhomDK2[i].ID_NhomDoiTuong);
                                        }
                                        break;
                                    case 5: // <
                                        if (itemDT[0].TongBan < nhomDK2[i].GiaTriSo) {
                                            arrNhom.push(nhomDK2[i].ID_NhomDoiTuong);
                                        }
                                        break;
                                    case 4: // <
                                        if (itemDT[0].TongBan <= nhomDK2[i].GiaTriSo) {
                                            arrNhom.push(nhomDK2[i].ID_NhomDoiTuong);
                                        }
                                        break;
                                }
                            }

                            // LoaiDieuKien = 3 (ThoiGianMuaHang) TODO

                            // LoaiDieuKien = 4  (SoLanMua)
                            var nhomDK4 = $.grep(arrNhom12345, function (x) {
                                return x.LoaiDieuKien === 4 && x.TuDongCapNhat === true;
                            });

                            for (var i = 0; i < nhomDK4.length; i++) {

                                switch (nhomDK4[i].LoaiSoSanh) {
                                    case 1: // >
                                        if (itemDT[0].SoLanMuaHang > nhomDK4[i].GiaTriSo) {
                                            arrNhom.push(nhomDK4[i].ID_NhomDoiTuong);
                                        }
                                        break;
                                    case 2: // >=
                                        if (itemDT[0].SoLanMuaHang >= nhomDK4[i].GiaTriSo) {
                                            arrNhom.push(nhomDK4[i].ID_NhomDoiTuong);
                                        }
                                        break;
                                    case 3: // =
                                        if (itemDT[0].SoLanMuaHang === nhomDK4[i].GiaTriSo) {
                                            arrNhom.push(nhomDK4[i].ID_NhomDoiTuong);
                                        }
                                        break;
                                    case 5: // <
                                        if (itemDT[0].SoLanMuaHang < nhomDK4[i].GiaTriSo) {
                                            arrNhom.push(nhomDK4[i].ID_NhomDoiTuong);
                                        }
                                        break;
                                    case 4: // <=
                                        if (itemDT[0].SoLanMuaHang <= nhomDK4[i].GiaTriSo) {
                                            arrNhom.push(nhomDK4[i].ID_NhomDoiTuong);
                                        }
                                        break;
                                }
                            }


                            // LoaiDieuKien = 5  (NoHienTai)
                            var nhomDK5 = $.grep(arrNhom12345, function (x) {
                                return x.LoaiDieuKien === 5 && x.TuDongCapNhat === true;
                            });

                            for (var i = 0; i < nhomDK5.length; i++) {

                                switch (nhomDK5[i].LoaiSoSanh) {
                                    case 1: // >
                                        if (itemDT[0].NoHienTai > nhomDK5[i].GiaTriSo) {
                                            arrNhom.push(nhomDK5[i].ID_NhomDoiTuong);
                                        }
                                        break;
                                    case 2: // >=
                                        if (itemDT[0].NoHienTai >= nhomDK5[i].GiaTriSo) {
                                            arrNhom.push(nhomDK5[i].ID_NhomDoiTuong);
                                        }
                                        break;
                                    case 3: // =
                                        if (itemDT[0].NoHienTai === nhomDK5[i].GiaTriSo) {
                                            arrNhom.push(nhomDK5[i].ID_NhomDoiTuong);
                                        }
                                        break;
                                    case 5: // <
                                        if (itemDT[0].NoHienTai < nhomDK5[i].GiaTriSo) {
                                            arrNhom.push(nhomDK5[i].ID_NhomDoiTuong);
                                        }
                                        break;
                                    case 4: // <=
                                        if (itemDT[0].NoHienTai <= nhomDK5[i].GiaTriSo) {
                                            arrNhom.push(nhomDK5[i].ID_NhomDoiTuong);
                                        }
                                        break;
                                }
                            }

                            // 6.ThangSinh 7.Tuoi 8.GioiTinh 9.KhuVuc 10.VungMien (not update because not update infor KH)

                            // get arr ID_Nhom old
                            var arrIDNhomOld = '';
                            var idNhomDTs = '';

                            if (itemDT[0].ID_NhomDoiTuong !== null && itemDT[0].ID_NhomDoiTuong !== undefined) {
                                idNhomDTs = itemDT[0].ID_NhomDoiTuong.toLowerCase();
                                arrIDNhomOld = idNhomDTs.split(',');
                            }

                            // get mang nhom sau khi add {value: id, count: number}
                            var arrJson_AfterAdd = compressArray(arrNhom);

                            // * REMOVE NHOM
                            if (itemHD.LoaiHoaDon === 1) {
                                // get ID in arrJson_AfterAdd
                                var arrIDAdd = [];
                                for (var i = 0; i < arrJson_AfterAdd.length; i++) {
                                    arrIDAdd.push(arrJson_AfterAdd[i].value);
                                }

                                // get arrID_Nhom in arrJson
                                var arrIDNhomChiTiet = [];
                                for (var i = 0; i < arrJson.length; i++) {
                                    arrIDNhomChiTiet.push(arrJson[i].value);
                                }

                                // compare 2 array: arrIDNhomOld(old) and arrIDAdd(new);
                                // return ID_Nhom exist in 'old', exist in arrIDNhomChiTiet, but not exist in 'new'
                                var arrIDRemove = [];
                                for (var i = 0; i < arrIDNhomOld.length; i++) {
                                    if ($.inArray(arrIDNhomOld[i].trim(), arrIDAdd) === -1 && $.inArray(arrIDNhomOld[i].trim(), arrIDNhomChiTiet) > -1) {
                                        arrIDRemove.push(arrIDNhomOld[i]);
                                    }
                                }
                                console.log('arrIDRemove ', arrIDRemove)

                                for (var i = 0; i < arrIDRemove.length; i++) {
                                    // remove ID_NhomDoiTuong in DM_DoiTuong_Nhom with ID_DoiTuong
                                    ajaxHelper(DMDoiTuongUri + 'DeleteNhom_ofDoiTuong?idDoiTuong=' + itemHD.ID_DoiTuong + '&idNhom=' + arrIDRemove[i], 'PUT').done(function (x) {
                                    })
                                }
                            }

                            // * ADD NHOM
                            if (itemHD.LoaiHoaDon === 6 || itemHD.LoaiHoaDon === 11) {
                                var arrAddDB = [];
                                for (var i = 0; i < arrJson.length; i++) {
                                    for (var j = 0; j < arrJson_AfterAdd.length; j++) {
                                        if (arrJson_AfterAdd[j].value === arrJson[i].value) {
                                            if (arrJson_AfterAdd[j].count === arrJson[i].count) {

                                                // neu doituong da thuoc nhom nay --> khong can add nua
                                                if (idNhomDTs.indexOf(arrJson_AfterAdd[j].value) === -1) {
                                                    var DM_DoiTuong_Nhom = {
                                                        ID_DoiTuong: itemHD.ID_DoiTuong,
                                                        ID_NhomDoiTuong: arrJson_AfterAdd[j].value,
                                                    };
                                                    arrAddDB.push(DM_DoiTuong_Nhom);
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                                console.log('arrAddDB ', arrAddDB)
                                if (arrAddDB.length > 0) {
                                    Insert_ManyNhom(arrAddDB);
                                }
                            }
                        }
                    }
                })
        }
    }

    function Insert_ManyNhom(lstNhom) {

        var myData = {};
        myData.lstDM_DoiTuong_Nhom = lstNhom;

        $.ajax({
            data: myData,
            url: DMDoiTuongUri + "PostDM_DoiTuong_Nhom",
            type: 'POST',
            async: false,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",

            success: function (item) {

            },
            statusCode: {
                404: function () {
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Thêm mới nhóm khách hàng thất bại!", "danger");
            },
            complete: function () {
            }
        })
    }

    // search and paging CTHD
    self.PageResult_CTHoaDons = ko.computed(function (item) {

        // filter
        var filter = self.filterHangHoa_ChiTietHD();
        var arrFilter = ko.utils.arrayFilter(self.BH_HoaDonChiTiets(), function (prod) {

            var chon = true;
            var ipLodau = locdau(filter);
            var maHH = locdau(prod.MaHangHoa);
            var tenHH = locdau(prod.TenHangHoa);
            var maLoHang = locdau(prod.MaLoHang);
            var kitudau = GetChartStart(tenHH);

            if (chon && filter) {
                chon = maHH.indexOf(ipLodau) > -1 || tenHH.indexOf(ipLodau) > -1
                    || maLoHang.indexOf(ipLodau) > -1 || kitudau.indexOf(ipLodau) > -1
                    ;
            }
            return chon;
        });

        var lenData = arrFilter.length;
        self.PageCount_CTHD(Math.ceil(lenData / self.PageSize_CTHD()));
        self.TotalRecord_CTHD(lenData);

        // paging
        var first = self.currentPage_CTHD() * self.PageSize_CTHD();
        if (arrFilter !== null) {
            return arrFilter.slice(first, first + self.PageSize_CTHD());
        }
    })

    self.PageList_CTHD = ko.computed(function () {
        var arrPage = [];

        var allPage = self.PageCount_CTHD();
        var currentPage = self.currentPage_CTHD();

        if (allPage > 4) {

            var i = 0;
            if (currentPage === 0) {
                i = parseInt(self.currentPage()) + 1;
            }
            else {
                i = self.currentPage();
            }

            if (allPage >= 5 && currentPage > allPage - 5) {
                if (currentPage >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (var i = allPage - 5; i < allPage; i++) {
                        var obj = {
                            pageNumber: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    if (currentPage == 1) {
                        for (var j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    }
                    else {
                        // get currentPage - 2 , currentPage, currentPage + 2
                        for (var j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                            var obj = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    }
                }
            }
            else {
                // get 5 trang dau
                if (i >= 2) {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumber: i - 1,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
                else {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumber: i,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
            }
        }
        else {
            // neu chi co 1 trang --> khong hien thi DS trang
            if (allPage > 1) {
                for (var i = 0; i < allPage; i++) {
                    var obj = {
                        pageNumber: i + 1,
                    };
                    arrPage.push(obj);
                }
            }
        }

        if (self.PageResult_CTHoaDons().length > 0) {

            self.fromitem_CTHD((self.currentPage_CTHD() * self.PageSize_CTHD()) + 1);

            if (((self.currentPage_CTHD() + 1) * self.PageSize_CTHD()) > self.PageResult_CTHoaDons().length) {
                var ss = (self.currentPage_CTHD() + 1) * self.PageSize_CTHD();
                var fromItem = (self.currentPage_CTHD() + 1) * self.PageSize_CTHD();
                if (fromItem < self.TotalRecord_CTHD()) {
                    self.toitem_CTHD((self.currentPage_CTHD() + 1) * self.PageSize_CTHD());
                }
                else {
                    self.toitem_CTHD(self.TotalRecord_CTHD());
                }
            } else {
                self.toitem_CTHD((self.currentPage_CTHD() * self.PageSize_CTHD()) + self.PageSize_CTHD());
            }
        }
        return arrPage;
    });

    self.VisibleStartPage_CTHD = ko.computed(function () {
        if (self.PageList_CTHD().length > 0) {
            return self.PageList_CTHD()[0].pageNumber !== 1;
        }
    });

    self.VisibleEndPage_CTHD = ko.computed(function () {
        if (self.PageList_CTHD().length > 0) {
            return self.PageList_CTHD()[self.PageList_CTHD().length - 1].pageNumber !== self.PageCount_CTHD();
        }
    })

    self.ResetCurrentPage_CTHD = function () {
        self.currentPage_CTHD(0);
    };

    self.GoToPage_CTHD = function (page) {
        self.currentPage_CTHD(page.pageNumber - 1);
    };

    self.GetClass_CTHD = function (page) {
        return ((page.pageNumber - 1) === self.currentPage_CTHD()) ? "click" : "";
    };

    self.StartPage_CTHD = function () {
        self.currentPage_CTHD(0);
    }

    self.BackPage_CTHD = function () {
        if (self.currentPage_CTHD() > 1) {
            self.currentPage_CTHD(self.currentPage_CTHD() - 1);
        }
    }

    self.GoToNextPage_CTHD = function () {
        if (self.currentPage_CTHD() < self.PageCount_CTHD() - 1) {
            self.currentPage_CTHD(self.currentPage_CTHD() + 1);
        }
    }

    self.EndPage_CTHD = function () {
        if (self.currentPage_CTHD() < self.PageCount_CTHD() - 1) {
            self.currentPage_CTHD(self.PageCount_CTHD() - 1);
        }
    }
    // show infor HoaDon/PhieuThu/Chi
    self.Modal_HoaDons = ko.observableArray();
    self.TongSLHang = ko.observable(0);
    self.LoaiHoaDon_MoPhieu = ko.observable(0);
    self.MaHoaDon_MoPhieu = ko.observable('');

    self.ShowPopup_InforHD_PhieuThu = function (item, itHD) {
        self.MaHoaDon_MoPhieu(item.MaHoaDon);
        vmThanhToan.showModalUpdate(item.ID, itHD.ConNo);
    }

    self.ClickMoPhieu = function () {
        localStorage.setItem('FindHD', self.MaHoaDon_MoPhieu());

        var url = '';

        switch (self.LoaiHoaDon_MoPhieu()) {
            case 6:
                localStorage.setItem('FindHD', self.MaHoaDon_MoPhieu());
                url = "/#/Returns";
                break;
            case 11:
            case 12:
                localStorage.removeItem('FindHD');
                localStorage.setItem('FindMaPhieuChi', self.MaHoaDon_MoPhieu());
                url = '/#/CashFlow';
                break;
        }
        if (url !== '') {
            window.open(url);
        }
    }

    self.Goto_LoHang = function (item) {
        localStorage.setItem('FindLoHang', item.MaLoHang);
        var url = "/#/Shipment";
        window.open(url);
    }

    function GetHT_TichDiem() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/HT_API/' + 'GetHT_CauHinh_TichDiemChiTiet?idDonVi=' + id_donvi, 'GET').done(function (obj) {
                if (obj.res === true) {
                    self.ThietLap_TichDiem(obj.data);
                }
            });
        }
    }

    self.showModalEditCKHoaDon = function (item) {
        let tongTT = formatNumberToFloat(item.PhaiThanhToan);
        let daTT = formatNumberToFloat(item.KhachDaTra) + formatNumberToFloat(item.BaoHiemDaTra);
        let obj = {
            ID: item.ID,
            LoaiHoaDon: item.LoaiHoaDon,
            MaHoaDon: item.MaHoaDon,
            TongThanhToan: tongTT,
            TongTienThue: item.TongTienThue,
            ThucThu: daTT - item.ThuTuThe - item.TienDoiDiem,
            DaThuTruoc: daTT,
            ConNo: tongTT - daTT,
            TongPhiNganHang: 0,
        }
        vmHoaHongHoaDon.GetChietKhauHoaDon_byID(obj);
    }

    self.showModalEditCKDichVu = function (item) {
        vmChiTietHoaDon.showModalChiTietHoaDon(item.ID);
    }

    $('#ThuTienHoaDonModal').on('hidden.bs.modal', function () {
        if (vmThanhToan.saveOK) {
            SearchHoaDon();
        }
    })

    self.showTPComBo = function (item) {
        item.LoaiHoaDon = 19;
        vmThanhPhanCombo.showModalUpdate(item);
    }
};
var modelGiaoDich = new ViewModelHD();
ko.applyBindings(modelGiaoDich, document.getElementById('divPage'));

function hidewait(o) {
    $('.' + o).append('<div id="wait"><img src="/Content/images/wait.gif" width="64" height="64" /><div class="happy-wait">' +
        ' </div>' +
        '</div>')
}

var arrIDchose = [];
function selectedVT(obj) {
    if ($(obj).children().length === 0) {
        $(obj).append('<i class="fa fa-check" aria-hidden="true"></i><i class="fa fa-times"></i>');
    }
    if ($(obj).children().length > 0) {
        arrIDchose.push($(obj).attr('id'));
    }
}

$(function () {
    $('input[type=text]').click(function () {
        $(this).select();
    });
})

$(function () {
    $('.daterange').daterangepicker({
        locale: {
            "format": 'DD/MM/YYYY',
            "separator": " - ",
            "applyLabel": "Tìm kiếm",
            "cancelLabel": "Hủy",
            "fromLabel": "Từ",
            "toLabel": "Đến",
            "customRangeLabel": "Custom",
            "daysOfWeek": [
                "CN",
                "T2",
                "T3",
                "T4",
                "T5",
                "T6",
                "T7"
            ],
            "monthNames": [
                "Tháng 1",
                "Tháng 2",
                "Tháng 3",
                "Tháng 4",
                "Tháng 5",
                "Tháng 6",
                "Tháng 7",
                "Tháng 8",
                "Tháng 9",
                "Tháng 10",
                "Tháng 11",
                "Tháng 12"
            ],
            "firstDay": 1
        }
    });
});

var arrIDCheck = [];

function SetCheckAll(obj) {
    var isChecked = $(obj).is(":checked");
    $('.check-group input[type=checkbox]').each(function () {
        $(this).prop('checked', isChecked);
    })
    if (isChecked) {
        $('.check-group input[type=checkbox]').each(function () {
            var thisID = $(this).attr('id');
            if (thisID !== undefined && !(jQuery.inArray(thisID, arrIDCheck) > -1)) {
                arrIDCheck.push(thisID);
            }
        });
    }
    else {
        $('.check-group input[type=checkbox]').each(function () {
            var thisID = $(this).attr('id');
            for (var i = 0; i < arrIDCheck.length; i++) {
                if (arrIDCheck[i] === thisID) {
                    arrIDCheck.splice(i, 1);
                    break;
                }
            }
        })
    }

    if (arrIDCheck.length > 0) {
        $('#divThaoTac').show();
        $('.choose-commodity').show().trigger("RemoveClassForButtonNew");
        $('#count').text(arrIDCheck.length);
    }
    else {
        $('#divThaoTac').hide();
        $('.choose-commodity').hide().trigger("addClassForButtonNew");
    }
}

function ChoseHoaDon(obj) {
    var thisID = $(obj).attr('id');

    if ($(obj).is(':checked')) {
        if ($.inArray(thisID, arrIDCheck) === -1) {
            arrIDCheck.push(thisID);
        }
    }
    else {
        //remove item in arrID
        arrIDCheck = arrIDCheck.filter(x => x !== thisID);
    }

    if (arrIDCheck.length > 0) {
        $('#divThaoTac').show();
        $('.choose-commodity').show().trigger("RemoveClassForButtonNew");
        $('#count').text(arrIDCheck.length);
    }
    else {
        $('#divThaoTac').hide();
        $('.choose-commodity').hide().trigger("addClassForButtonNew");
    }

    // count input is checked
    var countCheck = 0;
    $('#tb tr td.check-group input').each(function (x) {
        var id = $(this).attr('id');
        if ($.inArray(id, arrIDCheck) > -1) {
            countCheck += 1;
        }
    });

    // set check for header
    var ckHeader = $('#tb thead tr th:eq(0) input');
    var lenList = $('#tb tbody tr.prev-tr-hide').length;
    if (countCheck === lenList) {
        ckHeader.prop('checked', true);
    }
    else {
        ckHeader.prop('checked', false);
    }
}
function RemoveAllCheck() {
    $('input[type=checkbox]').prop('checked', false);
    arrIDCheck = [];
    $('#divThaoTac').hide();
    $('.choose-commodity').hide();
}