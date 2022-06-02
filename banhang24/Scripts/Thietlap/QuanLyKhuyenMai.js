
var vmKhuyenMai = new Vue({
    created: function () {

    },
    data: {

    },
    methods: {

    }
})
var ViewModel = function () {
    var self = this;
    var trangthai1;
    var trangthai2;
    var _maseach_KM = null;
    var _Trangthai_seach = 0;
    var _nameKM;
    var _noteKM;
    var _numberRowns = 10;
    var _numberPage = 1;
    var _numberPage_LS = 1;
    var _numberRowns_LS = 10;
    var rdHoatDong;
    var rdKhongHoatDong;
    var dk;
    var _nameDionVi = null;
    var _nameNhanVien = null;
    var _nameNhomKhachHang = null;
    var _id_ChiNhanh = null;
    var _id_NhanVien = null;
    var _id_NhanVienLS = $('.idnhanvien').text();
    var _id_DonViLS = $('#hd_IDdDonVi').val();// lấy ID chi nhánh
    var _id_NhomKhachHang = null;
    var _boolChiNhanh = true;
    var _boolNhanVien = true;
    var _boolNhomKhachHang = true;
    var _deleteNguoiBan = 1;
    var dkremove = 0;
    var tk = null; // timkiem nhomhang

    self.DonViChosed = ko.observableArray();
    //Khai báo biến khuyến mại
    var AllPage = 1;
    var AllPage_LS = 1;
    var _ID_KhuyenMai_Edit = null;
    var _maKM = null;
    var _tenKM = null;
    var _ghichuKM = null;
    var _trangthaiKM = true;
    var _hinhthucKM = 11;
    var _loaiKM = 1;
    var _ngayapdungKM = null;
    var _thangapdungKM = null;
    var _thuapdungKM = null;
    var _gioapdungKM = null;
    var _apdungngaysinhnhat = 0;
    var _tatcadonvi = true;
    var _tatcadoituong = true;
    var _tatcanhanvien = true;
    var _nguoisua = null;
    var _id_DonViQuyDoi = null;
    var dateTime = new Date();
    var _timeStart = moment(dateTime).format('YYYY-MM-DD HH:mm');
    var _timeEnd = moment(new Date(dateTime.getFullYear(), dateTime.getMonth() + 5, 1)).format('YYYY-MM-DD HH:mm');
    self.ThoiGianBatDauKM = ko.observableArray();
    self.ThoiGianKetThucKM = ko.observableArray();
    self.ThoiGianBatDauKM(moment(_timeStart).format('DD/MM/YYYY HH:mm'));
    self.ThoiGianKetThucKM(moment(_timeEnd).format('DD/MM/YYYY HH:mm'));
    self.RowsStart = ko.observable('0');
    self.RowsEnd = ko.observable('10');
    self.RowsStart_LS = ko.observable('0');
    self.RowsEnd_LS = ko.observable('10');
    self.RowsKhuyenMai = ko.observable();
    self.PagesKhuyenMai = ko.observableArray();
    self.BH_LichSuKhuyenMai = ko.observableArray();
    self.Rows_LichSuKhuyenMai = ko.observable();
    self.Pages_LichSuKhuyenMai = ko.observableArray();
    self.CurrentPage = ko.observable(1);
    self.currentPage_LS = ko.observable(1);

    var _IDNguoiDung = $('.idnguoidung').text();
    var _IDchinhanh = $('#hd_IDdDonVi').val();
    self.TieuDe = ko.observable('Thêm chương trình khuyến mại');
    var maHH_search = null;
    var _nameDonViSeach = $('#hd_IDdDonVi').val();
    self.TenChiNhanh = ko.observable();
    self.searchDonVi = ko.observableArray()
    var _id_DonVi = $('#hd_IDdDonVi').val();
    var _IDDoiTuong = $('.idnguoidung').text();
    var ReportUri = '/api/DanhMuc/ReportAPI/';

    self.ItemKMChiTiet = ko.observableArray();
    self.BH_KhuyenMai_ChiTiet = ko.observableArray();
    self.mangThangKM = ko.observableArray();
    self.mangNgayKM = ko.observableArray();
    self.mangThuKM = ko.observableArray();
    self.mangGioKM = ko.observableArray();
    self.MangChiNhanh = ko.observableArray();
    self.MangChiNhanh_Use = ko.observableArray();
    self.MangNhanVien = ko.observableArray();
    self.MangNhomKhachHang = ko.observableArray();
    self.CT_ApDungKM = ko.observableArray();
    self.CT_HangHoaKM = ko.observableArray();
    self.CT_KhuyenMai_ChiTiet = ko.observableArray();
    self.selectedHangHoa = ko.observable();
    self.selectedHangHoaMua = ko.observable();
    self.HangHoas = ko.observableArray();
    self.none = ko.observableArray();
    self.DonVis = ko.observableArray();
    self.NhomHangHoas = ko.observableArray();
    self.NhomDoiTuongs = ko.observableArray();
    self.NhanViens = ko.observableArray();
    var BH_DonViUri = '/api/DanhMuc/DM_DonViAPI/';
    var NhanVienUri = '/api/DanhMuc/NS_NhanVienAPI/';
    var NguoiDungUri = '/api/DanhMuc/HT_NguoiDungAPI/';
    var BH_HoaDonUri = '/api/DanhMuc/BH_HoaDonAPI/';
    var BH_XuatHuyUri = '/api/DanhMuc/BH_XuatHuyAPI/';
    var BH_KhuyenMaiUri = '/api/DanhMuc/BH_KhuyenMaiAPI/';
    self.MaHangHoa_Search = ko.observable();
    var _nameChiNhanhKM = null;
    //var _id_ChiNhanh_Use;
    var _deleteChiNhanh = 1;
    var _ID_theoSL = 1;
    var Lc_GiamGiaHD = [{ ID: 1, TongTienHang: '0', GiamGia: null, GiamGiaTheoPhanTram: true }];
    var Lc_TangHang = [{ ID: 1, TongTienHang: '0', SoLuong: 1, ID_DonViQuiDoi: null, TenHangHoa: null, ID_NhomHangHoa: null, TenNhomHangHoa: null }];
    var Lc_GiamGiaHang = [{ ID: 1, TongTienHang: '0', GiamGia: null, GiamGiaTheoPhanTram: true, SoLuong: 1, ID_DonViQuiDoi: null, TenHangHoa: null, ID_NhomHangHoa: null, TenNhomHangHoa: null }];
    var Lc_TangDiem = [{ ID: 1, TongTienHang: '0', DiemCong: null, GiamGiaTheoPhanTram: true }];
    var Lc_MuaHangGiamGia = [{ ID: 1, SoLuongMua: 1, ID_DonViQuiDoiMua: null, TenHangHoaMua: null, ID_NhomHangHoaMua: null, TenNhomHangHoaMua: null, GiamGia: null, GiamGiaTheoPhanTram: true, SoLuong: 1, ID_DonViQuiDoi: null, TenHangHoa: null, ID_NhomHangHoa: null, TenNhomHangHoa: null }];
    var Lc_MuaHangTangHang = [{ ID: 1, SoLuongMua: 1, ID_DonViQuiDoiMua: null, TenHangHoaMua: null, ID_NhomHangHoaMua: null, TenNhomHangHoaMua: null, SoLuong: 1, ID_DonViQuiDoi: null, TenHangHoa: null, ID_NhomHangHoa: null, TenNhomHangHoa: null }];
    var Lc_MuaHangTangDiem = [{ ID: 1, SoLuongMua: 1, ID_DonViQuiDoiMua: null, TenHangHoaMua: null, ID_NhomHangHoaMua: null, TenNhomHangHoaMua: null, DiemCong: null, GiamGiaTheoPhanTram: true }];
    var Lc_GiaBanTheoSL = [{ ID: 1, SoLuongMua: 1, ID_DonViQuiDoiMua: null, TenHangHoaMua: null, ID_NhomHangHoaMua: null, TenNhomHangHoaMua: null, GiaKhuyenMai: null, }];
    var Lc_ThemHangHoa = [{ ID_addHH: 1, SoLuongMua: 1, ID_DonViQuiDoi: null, TenHangHoa: null, ID_NhomHangHoa: null, TenNhomHangHoa: null, GiaKhuyenMai: null, GiamGia: null, GiamGiaTheoPhanTram: null, IDRandom: CreateIDRandom('KM24_'), TypeSelect: '1' }]
    self.HD_GiamGiaHD = ko.observableArray(Lc_GiamGiaHD);
    self.HD_TangHang = ko.observableArray(Lc_TangHang);
    self.SaveHD_TangHang = ko.observableArray();
    self.HD_GiamGiaHang = ko.observableArray(Lc_GiamGiaHang);
    self.HD_TangDiem = ko.observableArray(Lc_TangDiem);
    self.HH_MuaHangGiamGia = ko.observableArray(Lc_MuaHangGiamGia);
    self.HH_MuaHangTangHang = ko.observableArray(Lc_MuaHangTangHang);
    self.HH_MuaHangTangDiem = ko.observableArray(Lc_MuaHangTangDiem);
    self.HH_GiaBanTheoSL = ko.observableArray(Lc_GiaBanTheoSL);
    self.ThemHangHoa = ko.observableArray(Lc_ThemHangHoa);
    self.DonViTinhKM = ko.observable('VNĐ');
    self.filteredDM_KhuyenMai = ko.observableArray();
    self.TongTrangKhuyenMai = ko.observableArray();
    self.TongHangKhuyenMai = ko.observable('0');
    self.rownStart = ko.observable('1');
    self.rownEnd = ko.observable('15');
    var mang1 = ['Giảm giá hóa đơn', 'Tặng hàng', 'Giảm giá hàng', 'Tặng điểm']
    self.HinhthucKM = ko.observableArray(mang1);
    self.txtKhuyenMai = ko.observable('Hóa đơn');
    var mangThang = ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12']
    self.ThangKM = ko.observableArray(mangThang);
    self.SeachThangKM = ko.observableArray(mangThang);
    var mangNgay = ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12', '13', '14', '15', '16', '17', '18', '19', '20', '21', '22', '23', '24', '25', '26', '27', '28', '29', '30', '31']
    self.NgayKM = ko.observableArray(mangNgay);
    var mangThu = ['2', '3', '4', '5', '6', '7', '8']
    self.ThuKM = ko.observableArray(mangThu);
    var mangGio = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12', '13', '14', '15', '16', '17', '18', '19', '20', '21', '22', '23']
    self.GioKM = ko.observableArray(mangGio);
    self.selectThangKM = ko.observable(null);
    self.pageSizes = [10,20,30, 40, 50];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.SelectHT = ko.observable(self.HinhthucKM()[0]);
    self.KhuyenMai_CapNhat = ko.observable();
    self.KhuyenMai_SaoChep = ko.observable();
    self.KhuyenMai_ThemMoi = ko.observable();
    self.KhuyenMai_XemDs = ko.observable();
    self.KhuyenMai_Xoa = ko.observable();
    self.Filter_Expired = ko.observable(2);
    self.Filter_TypePromotion = ko.observable(0);
    self.StatusActive = ko.observable(1);

    function Page_Load() {
        getQuyen_NguoiDung();
        getDonVi();
        getnameuse();
        GetAllNhomHH();
        getNhomDoiTuong();
    }

    Page_Load();

    function getQuyen_NguoiDung() {
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "KhuyenMai_CapNhat", "GET").done(function (data) {
            self.KhuyenMai_CapNhat(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "KhuyenMai_SaoChep", "GET").done(function (data) {
            self.KhuyenMai_SaoChep(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "KhuyenMai_ThemMoi", "GET").done(function (data) {
            self.KhuyenMai_ThemMoi(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "KhuyenMai_XemDS", "GET").done(function (data) {
            self.KhuyenMai_XemDs(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "KhuyenMai_Xoa", "GET").done(function (data) {
            self.KhuyenMai_Xoa(data);
        })
    }

    self.NoteMaKM = function () {
        _maseach_KM = $('#EnterMaKM').val();
    }

    self.checkKichHoat = function () {
        _trangthaiKM = true;
    }
    self.checkChuaApDung = function () {
        _trangthaiKM = false;
    }
    $('.choseSuKienKM input').on('click', function () {
        if ($(this).val() === '1') {
            _trangthaiKM = true;
        }
        else {
            _trangthaiKM = false;
        }
    })
    $('#EnterMaKM').keypress(function (e) {
        if (e.keyCode == 13) {
            _numberPage = 1;
            GetListPromotion();
        }
    });

    self.Click_IconSearch = function () {
        _numberPage = 1;
        GetListPromotion();
    }
    self.SelectNameKM = function () {
        _tenKM = $('#txtNameKM').val();
    }
    self.NoteKM = function () {
        _ghichuKM = $('#txtNoteKM').val();
    }

    self.Note_MaKhuyenMai = function () {
        _maKM = $('#txt_NoteMaKM').val();
    }
    //ẩn chọn
    $(function () {
        $('#choose_DonVi').removeAttr('data-toggle');
        $('#choose-NguoiBan').removeAttr('data-toggle');
        $('#choose-NhomKhachHang').removeAttr('data-toggle');
        $('#choose_NgaySN').removeAttr('data-toggle');
    });
    $('#chinhanhfalse').on('click', function () {
        _tatcadonvi = false;
        $('#choose_DonVi').attr("data-toggle", "dropdown");
    });
    $('#chinhanhtrue').on('click', function () {
        //debugger;
        _tatcadonvi = true;
        $('#choose_DonVi').removeAttr('data-toggle');
        self.MangChiNhanh([]);
        $('#choose_DonVi input').remove();
        $('#choose_DonVi').append('<input type="text" class="dropdown form-control" placeholder="Chọn chi nhánh áp dụng...">');
        $('#selec-all-DonVi li').each(function () {
            $(this).find('i').remove();
        });
        //getDonVi();
    });

    $('#checkKhachHangFalse').on('click', function () {
        _tatcadoituong = false;
        $('#choose-NhomKhachHang').attr("data-toggle", "dropdown");
    });
    $('#checkKhachHangTrue').on('click', function () {
        _tatcadoituong = true;
        $('#choose-NhomKhachHang').removeAttr('data-toggle');
        self.MangNhomKhachHang([]);
        $('#choose-NhomKhachHang input').remove();
        $('#choose-NhomKhachHang').append('<input type="text" class="dropdown" placeholder="Chọn nhóm khách hàng áp dụng...">');
        $('#selec-all-NhomKhachHang li').each(function () {
            $(this).find('i').remove();
        });
    });

    $('#checkNguoiBanFalse').on('click', function () {
        _tatcanhanvien = false;
        $('#choose-NguoiBan').attr("data-toggle", "dropdown");
    });
    $('#checkNguoiBanTrue').on('click', function () {
        _tatcanhanvien = true;
        $('#choose-NguoiBan').removeAttr('data-toggle');
        self.MangNhanVien([]);
        $('#choose-NguoiBan input').remove();
        $('#choose-NguoiBan').append('<input type="text" class="dropdown" placeholder="Chọn người bán áp dụng...">');
        $('#selec-all-NguoiBan li').each(function () {
            $(this).find('i').remove();
        });
    });
    //$('.choseAllKM input').on('click', function () {
    //    _Trangthai_seach = 0;
    //    _numberPage = 1;
    //    GetListPromotion();
    //});
    //$('.choseHoatDong input').on('click', function () {
    //    _Trangthai_seach = 1;
    //    _numberPage = 1;
    //    GetListPromotion();
    //});
    //$('.choseKhongHoatDong input').on('click', function () {
    //    _Trangthai_seach = 2;
    //    _numberPage = 1;
    //    GetListPromotion();
    //});

    self.EventPageSize = function () {
        
        _numberRowns = $('#txtRownSelect').val();
        _numberPage = 1;
        GetListPromotion();
    }
    self.EventThangKM = function () {
        _thangKM = $('#txtThangKM').val();
    }
    //load table 
    $(function () {
        $(".table-reduced").hide();
        $(".table-reducedhh").hide();
        $(".giamgiahoadon").show();
        $(this).addClass("checkicon");
        $(this).next("li").removeClass("checkicon");
    });
    self.selectedHinhThuc = function (item) {
        dk = $('#txtSelectHT').val();
        if (dk == 'Giảm giá hóa đơn') {
            _hinhthucKM = 11;
            $(".table-reduced").hide();
            $(".table-reducedhh").hide();
            $(".giamgiahoadon").show();
            $(this).addClass("checkicon");
            $(this).next("li").removeClass("checkicon");
            self.DonViTinhKM('VNĐ')
            self.SaveHD_TangHang(self.HD_GiamGiaHD());
            self.HD_GiamGiaHD([]);
            for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
                self.HD_GiamGiaHD.push(self.SaveHD_TangHang()[i]);
            }
        }
        else if (dk == 'Tặng hàng') {
            _hinhthucKM = 12;
            $(".table-reduced").hide();
            $(".table-reducedhh").hide();
            $(".tanghang").show();
            $(this).addClass("checkicon");
            $(this).next("li").removeClass("checkicon");
            self.DonViTinhKM('VNĐ')
            self.SaveHD_TangHang(self.HD_TangHang());
            self.HD_TangHang([]);
            for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
                self.HD_TangHang.push(self.SaveHD_TangHang()[i]);
            }
        }
        else if (dk == 'Giảm giá hàng') {
            _hinhthucKM = 13;
            $(".table-reduced").hide();
            $(".table-reducedhh").hide();
            $(".giamgiahang").show();
            $(this).addClass("checkicon");
            $(this).next("li").removeClass("checkicon");
            self.DonViTinhKM('VNĐ')
            for (var i = 0; i < self.HD_GiamGiaHang().length; i++) {
                var $this = $('#dvtGiamGiaHang_' + self.HD_GiamGiaHang()[i].ID);
                if (self.HD_GiamGiaHang()[i].GiamGiaTheoPhanTram) {
                    $this.addClass('active-re');
                }
                else {
                    $this.removeClass('active-re');
                }
            }
            self.SaveHD_TangHang(self.HD_GiamGiaHang());
            self.HD_GiamGiaHang([]);
            for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
                self.HD_GiamGiaHang.push(self.SaveHD_TangHang()[i]);
            }
        }
        else if (dk == 'Tặng điểm') {
            _hinhthucKM = 14;
            $(".table-reduced").hide();
            $(".table-reducedhh").hide();
            $(".tangdiem").show();
            $(this).addClass("checkicon");
            $(this).next("li").removeClass("checkicon");
            self.DonViTinhKM('Điểm')
            self.SaveHD_TangHang(self.HD_TangDiem());
            self.HD_TangDiem([]);
            for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
                self.HD_TangDiem.push(self.SaveHD_TangHang()[i]);
            }
        }
        else if (dk == 'Mua hàng giảm giá hàng') {
            _hinhthucKM = 21;
            $(".table-reducedhh").hide();
            $(".table-reduced").hide();
            $(".HangHoa").show();
            $(this).addClass("checkicon");
            $(this).prev("li").removeClass("checkicon");
            self.DonViTinhKM('VNĐ')
            self.SaveHD_TangHang(self.HH_MuaHangGiamGia());
            self.HH_MuaHangGiamGia([]);
            for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
                self.HH_MuaHangGiamGia.push(self.SaveHD_TangHang()[i]);
            }
        }
        else if (dk == 'Mua hàng tặng hàng') {
            _hinhthucKM = 22;
            $(".table-reducedhh").hide();
            $(".table-reduced").hide();
            $(".muahangtanghang2").show();
            $(this).addClass("checkicon");
            $(this).prev("li").removeClass("checkicon");
            self.DonViTinhKM('VNĐ')
            self.SaveHD_TangHang(self.HH_MuaHangTangHang());
            self.HH_MuaHangTangHang([]);
            for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
                self.HH_MuaHangTangHang.push(self.SaveHD_TangHang()[i]);
            }
        }
        else if (dk == 'Mua hàng tặng điểm') {
            _hinhthucKM = 23;
            $(".table-reducedhh").hide();
            $(".table-reduced").hide();
            $(".muahangtangdiem2").show();
            $(this).addClass("checkicon");
            $(this).prev("li").removeClass("checkicon");
            self.DonViTinhKM('Điểm')
            self.SaveHD_TangHang(self.HH_MuaHangTangDiem());
            self.HH_MuaHangTangDiem([]);
            for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
                self.HH_MuaHangTangDiem.push(self.SaveHD_TangHang()[i]);
            }
        }
        else {
            _hinhthucKM = 24;
            $(".table-reducedhh").hide();
            $(".table-reduced").hide();
            $(".tangtheosoluongmua").show();
            $('.vnd24').hide();
            $(this).addClass("checkicon");
            $(this).prev("li").removeClass("checkicon");
            self.DonViTinhKM('VNĐ')
            self.SaveHD_TangHang(self.HH_GiaBanTheoSL());
            self.HH_GiaBanTheoSL([]);
            for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
                self.HH_GiaBanTheoSL.push(self.SaveHD_TangHang()[i]);
            }
            setValueSelect_KM24();
        }
        //dk = null;
    }
    $('#txtSelectHT li').on('click', function () {
        console.log($(this).text())
    });
    //tạo mới khuyến mại
    //Kiểm tra ký tự đặc biệt
    function isValid(str) {
        return !/[~`!@#$%\^&*()+=\-\[\]\\';,/{}|\\":<>\?]/g.test(str);
    };
    self.selectedHD = function () {
        _loaiKM = 1;
        var mang = ['Giảm giá hóa đơn', 'Tặng hàng', 'Giảm giá hàng', 'Tặng điểm']
        self.HinhthucKM(mang);
        self.txtKhuyenMai('Hóa đơn');
        self.SelectHT(self.HinhthucKM()[0]);
        $(".table-reduced").hide();
        $(".table-reducedhh").hide();
        $(".giamgiahoadon").show();
        $(this).addClass("checkicon");
        $(this).next("li").removeClass("checkicon");
        dk == 'Giảm giá hóa đơn'
    }
    self.selectedHH = function () {
        _loaiKM = 2;
        var mang = ['Mua hàng giảm giá hàng', 'Mua hàng tặng hàng', 'Mua hàng tặng điểm', 'Giảm giá bán theo số lượng mua']
        self.HinhthucKM(mang);
        self.txtKhuyenMai('Hàng hóa');
        self.SelectHT(self.HinhthucKM()[0]);
        $(".table-reduced").hide();
        $(".table-reducedhh").hide();
        $(".HangHoa").show();
        $(this).addClass("checkicon");
        $(this).prev("li").removeClass("checkicon");
        dk == 'Mua hàng giảm giá hàng'
    }

    self.addHangHoa = function (item) {
        var ob = {
            ID_addHH: item.ID,
            SoLuongMua: 1,
            ID_DonViQuiDoi: null,
            TenHangHoa: '',
            ID_NhomHangHoa: null,
            TenNhomHangHoa: null,
            GiaKhuyenMai: null,
            GiamGia: null,
            GiamGiaTheoPhanTram: null,
            IDRandom: CreateIDRandom('KM24_'),
            TypeSelect: '1',
        }
        //Lc_ThemHangHoa.push(ob);
        //localStorage.setItem('Lc_ThemHangHoa', JSON.stringify(Lc_ThemHangHoa));
        self.ThemHangHoa.push(ob);
    }
    self.DeleteGiamGiaHD = function (item) {
        if (dk == 'Giảm giá hóa đơn') {
            self.HD_GiamGiaHD.remove(item);
            localStorage.setItem('Lc_GiamGiaHD', JSON.stringify(self.HD_GiamGiaHD()));
        }
        else if (dk == 'Tặng hàng') {
            self.HD_TangHang.remove(item);
            localStorage.setItem('Lc_TangHang', JSON.stringify(self.HD_TangHang()))
        }
        else if (dk == 'Giảm giá hàng') {
            self.HD_GiamGiaHang.remove(item);
            localStorage.setItem('Lc_GiamGiaHang', JSON.stringify(self.HD_GiamGiaHang()))
        }
        else if (dk == 'Tặng điểm') {
            self.HD_TangDiem.remove(item);
            localStorage.setItem('Lc_TangDiem', JSON.stringify(self.HD_TangDiem()))
        }
        else if (dk == 'Mua hàng giảm giá hàng') {
            self.HH_MuaHangGiamGia.remove(item);
            localStorage.setItem('Lc_MuaHangGiamGia', JSON.stringify(self.HH_MuaHangGiamGia()))
        }
        else if (dk == 'Mua hàng tặng hàng') {
            self.HH_MuaHangTangHang.remove(item);
            localStorage.setItem('Lc_MuaHangTangHang', JSON.stringify(self.HH_MuaHangTangHang()))
        }
        else if (dk == 'Mua hàng tặng điểm') {
            self.HH_MuaHangTangDiem.remove(item);
            localStorage.setItem('Lc_MuaHangTangDiem', JSON.stringify(self.HH_MuaHangTangDiem()))
        }
        else if (dk == 'Giảm giá bán theo số lượng mua') {
            self.HH_GiaBanTheoSL.remove(item);
            localStorage.setItem('Lc_GiaBanTheoSL', JSON.stringify(self.HH_GiaBanTheoSL()))
            console.log(self.HH_GiaBanTheoSL());
        }
        else {
            self.HD_GiamGiaHD.remove(item);
            localStorage.setItem('Lc_GiamGiaHD', JSON.stringify(self.HD_GiamGiaHD()));
        }
    }
    self.DeleteHH = function (item) {
        self.ThemHangHoa.remove(item);
        //localStorage.setItem('Lc_ThemHangHoa', JSON.stringify(self.ThemHangHoa()))
    }
    var _IDadd;
    self.ThemDieuKienKM = function () {
        if (_hinhthucKM == 11) {
            _IDadd = self.HD_GiamGiaHD().length + 1;
            var ob1 = {
                ID: _IDadd,
                TongTienHang: '0',
                GiamGia: null,
                GiamGiaTheoPhanTram: true
            }
            self.HD_GiamGiaHD.push(ob1);
            console.log(self.HD_GiamGiaHD())
        }
        else if (_hinhthucKM == 12) {
            _IDadd = self.HD_TangHang().length + 1;
            var ob = {
                ID: _IDadd,
                TongTienHang: '0',
                SoLuong: 1,
                ID_DonViQuiDoi: null,
                TenHangHoa: null,
                ID_NhomHangHoa: null,
                TenNhomHangHoa: null,
            }
            self.HD_TangHang.push(ob);
            console.log(self.HD_TangHang())
        }
        else if (_hinhthucKM == 13) {
            _IDadd = self.HD_GiamGiaHang().length + 1;
            var ob = {
                ID: _IDadd,
                TongTienHang: '0',
                GiamGia: null,
                GiamGiaTheoPhanTram: true,
                SoLuong: 1,
                ID_DonViQuiDoi: null,
                TenHangHoa: null,
                ID_NhomHangHoa: null,
                TenNhomHangHoa: null
            }

            self.HD_GiamGiaHang.push(ob);
            console.log(self.HD_GiamGiaHang())
        }
        else if (_hinhthucKM == 14) {
            _IDadd = self.HD_TangDiem().length + 1;
            var ob = {
                ID: _IDadd,
                TongTienHang: '0',
                DiemCong: null,
                GiamGiaTheoPhanTram: true,
            }
            self.HD_TangDiem.push(ob);
            console.log(self.HD_TangDiem());
        }
        else if (_hinhthucKM == 21) {
            _IDadd = self.HH_MuaHangGiamGia().length + 1;
            var ob = {
                ID: _IDadd,
                SoLuongMua: 1,
                ID_DonViQuiDoiMua: null,
                TenHangHoaMua: null,
                ID_NhomHangHoaMua: null,
                TenNhomHangHoaMua: null,
                GiamGia: null,
                GiamGiaTheoPhanTram: true,
                SoLuong: 1,
                ID_DonViQuiDoi: null,
                TenHangHoa: null,
                ID_NhomHangHoa: null,
                TenNhomHangHoa: null
            }
            self.HH_MuaHangGiamGia.push(ob);
            console.log(self.HH_MuaHangGiamGia())
        }
        else if (_hinhthucKM == 22) {
            _IDadd = self.HH_MuaHangTangHang().length + 1;
            var ob = {
                ID: _IDadd,
                SoLuongMua: 1,
                ID_DonViQuiDoiMua: null,
                TenHangHoaMua: null,
                ID_NhomHangHoaMua: null,
                TenNhomHangHoaMua: null,
                SoLuong: 1,
                ID_DonViQuiDoi: null,
                TenHangHoa: null,
                ID_NhomHangHoa: null,
                TenNhomHangHoa: null
            }
            self.HH_MuaHangTangHang.push(ob);
            console.log(self.HH_MuaHangTangHang())
        }
        else if (_hinhthucKM == 23) {
            _IDadd = self.HH_MuaHangTangDiem().length + 1;
            var ob = {
                ID: _IDadd,
                SoLuongMua: 1,
                ID_DonViQuiDoiMua: null,
                TenHangHoaMua: null,
                ID_NhomHangHoaMua: null,
                TenNhomHangHoaMua: null,
                DiemCong: null,
                GiamGiaTheoPhanTram: true
            }
            self.HH_MuaHangTangDiem.push(ob);
            console.log(self.HH_MuaHangTangDiem())
        }
        else if (_hinhthucKM == 24) {
            _ID_theoSL = _ID_theoSL + 1;
            var ob = {
                ID: _ID_theoSL,
                SoLuongMua: 1,
                ID_DonViQuiDoiMua: null,
                TenHangHoaMua: null,
                ID_NhomHangHoaMua: null,
                TenNhomHangHoaMua: null,
                GiaKhuyenMai: null,
            }
            self.HH_GiaBanTheoSL.push(ob);
            //thêm 1 đơn vị hàng
            var ob1 = {
                ID_addHH: _ID_theoSL,
                SoLuongMua: 1,
                ID_DonViQuiDoi: null,
                TenHangHoa: null,
                ID_NhomHangHoa: null,
                TenNhomHangHoa: null,
                GiaKhuyenMai: null,
                GiamGia: null,
                GiamGiaTheoPhanTram: null,
                IDRandom: CreateIDRandom('KM24_'),
                TypeSelect: '1',
            }
            self.ThemHangHoa.push(ob1);
        }
        else {
            _IDadd = self.HD_GiamGiaHD().length + 1;
            var ob1 = {
                ID: _IDadd,
                TongTienHang: '0',
                GiamGia: null,
                GiamGiaTheoPhanTram: true
            }
            self.HD_GiamGiaHD.push(ob1);
            console.log(self.HD_GiamGiaHD())
        }
    }
    $('#ClickPhanTram').on('click', function (item) {
        if (_hinhthucKM == 11) {
            //console.log(item.ID, self.HD_GiamGiaHD()[item.ID -1].GiamGiaTheoPhanTram);
            var dk = self.HD_GiamGiaHD()[item.ID - 1].GiamGiaTheoPhanTram
            if (dk == true) {
                self.HD_GiamGiaHD.replace(self.HD_GiamGiaHD()[item.ID - 1], { ID: item.ID, TongTienHang: item.TongTienHang, GiamGia: item.GiamGia, GiamGiaTheoPhanTram: false });
            }
            else {
                self.HD_GiamGiaHD.replace(self.HD_GiamGiaHD()[item.ID - 1], { ID: item.ID, TongTienHang: item.TongTienHang, GiamGia: item.GiamGia, GiamGiaTheoPhanTram: true });
            }
            console.log(2, self.HD_GiamGiaHD()[item.ID - 1].GiamGiaTheoPhanTram);
        }
    });
    self.SelectedDVT = function (item) {
        var dk = item.GiamGiaTheoPhanTram;
        if (_hinhthucKM == 11) {
            if (dk == true) {
                self.HD_GiamGiaHD.replace(item, { ID: item.ID, TongTienHang: item.TongTienHang, GiamGia: item.GiamGia, GiamGiaTheoPhanTram: false });
            }
            else {
                self.HD_GiamGiaHD.replace(item, { ID: item.ID, TongTienHang: item.TongTienHang, GiamGia: item.GiamGia, GiamGiaTheoPhanTram: true });
            }
            var $this = $('#dvt_' + item.ID);
            if (dk) {
                $this.removeClass('active-re');
            }
            else {
                $this.addClass('active-re');
            }
            console.log(self.HD_GiamGiaHD());
        }
        if (_hinhthucKM == 13) {
            if (dk == true) {
                self.HD_GiamGiaHang.replace(item, { ID: item.ID, TongTienHang: item.TongTienHang, GiamGia: item.GiamGia, GiamGiaTheoPhanTram: false, SoLuong: item.SoLuong, ID_DonViQuiDoi: item.ID_DonViQuiDoi, TenHangHoa: item.TenHangHoa, ID_NhomHangHoa: item.ID_NhomHangHoa, TenNhomHangHoa: item.TenNhomHangHoa });
            }
            else {
                self.HD_GiamGiaHang.replace(item, { ID: item.ID, TongTienHang: item.TongTienHang, GiamGia: item.GiamGia, GiamGiaTheoPhanTram: true, SoLuong: item.SoLuong, ID_DonViQuiDoi: item.ID_DonViQuiDoi, TenHangHoa: item.TenHangHoa, ID_NhomHangHoa: item.ID_NhomHangHoa, TenNhomHangHoa: item.TenNhomHangHoa });
            }
            var $this = $('#dvtGiamGiaHang_' + item.ID);
            if (dk) {
                $this.removeClass('active-re');
            }
            else {
                $this.addClass('active-re');
            }
            console.log(self.HD_GiamGiaHang())
        }
        if (_hinhthucKM == 14) {
            if (dk) {
                self.HD_TangDiem.replace(item, { ID: item.ID, TongTienHang: item.TongTienHang, DiemCong: item.DiemCong, GiamGiaTheoPhanTram: false })
            }
            else {
                self.HD_TangDiem.replace(item, { ID: item.ID, TongTienHang: item.TongTienHang, DiemCong: item.DiemCong, GiamGiaTheoPhanTram: true })
            }
            var $this = $('#Diem_' + item.ID);
            if (dk) {
                $this.removeClass('active-re');
            }
            else {
                $this.addClass('active-re');
            }
            console.log(self.HD_TangDiem())
        }
        if (_hinhthucKM == 21) {
            if (dk) {
                self.HH_MuaHangGiamGia.replace(item, { ID: item.ID, SoLuongMua: item.SoLuongMua, ID_DonViQuiDoiMua: item.ID_DonViQuiDoiMua, TenHangHoaMua: item.TenHangHoaMua, ID_NhomHangHoaMua: item.ID_NhomHangHoaMua, TenNhomHangHoaMua: item.TenNhomHangHoaMua, GiamGia: item.GiamGia, GiamGiaTheoPhanTram: false, SoLuong: item.SoLuong, ID_DonViQuiDoi: item.ID_DonViQuiDoi, TenHangHoa: item.TenHangHoa, ID_NhomHangHoa: item.ID_NhomHangHoa, TenNhomHangHoa: item.TenNhomHangHoa });
            }
            else {
                self.HH_MuaHangGiamGia.replace(item, { ID: item.ID, SoLuongMua: item.SoLuongMua, ID_DonViQuiDoiMua: item.ID_DonViQuiDoiMua, TenHangHoaMua: item.TenHangHoaMua, ID_NhomHangHoaMua: item.ID_NhomHangHoaMua, TenNhomHangHoaMua: item.TenNhomHangHoaMua, GiamGia: item.GiamGia, GiamGiaTheoPhanTram: true, SoLuong: item.SoLuong, ID_DonViQuiDoi: item.ID_DonViQuiDoi, TenHangHoa: item.TenHangHoa, ID_NhomHangHoa: item.ID_NhomHangHoa, TenNhomHangHoa: item.TenNhomHangHoa });
            }
            var $this = $('#dvtMuaHangGiamGia_' + item.ID);
            if (dk) {
                $this.removeClass('active-re');
            }
            else {
                $this.addClass('active-re');
            }
            console.log(self.HH_MuaHangGiamGia())
        }
        if (_hinhthucKM == 23) {
            if (dk) {
                self.HH_MuaHangTangDiem.replace(item, { ID: item.ID, SoLuongMua: item.SoLuongMua, ID_DonViQuiDoiMua: item.ID_DonViQuiDoiMua, TenHangHoaMua: item.TenHangHoaMua, ID_NhomHangHoaMua: item.ID_NhomHangHoaMua, TenNhomHangHoaMua: item.TenNhomHangHoaMua, DiemCong: item.DiemCong, GiamGiaTheoPhanTram: false });
            }
            else {
                self.HH_MuaHangTangDiem.replace(item, { ID: item.ID, SoLuongMua: item.SoLuongMua, ID_DonViQuiDoiMua: item.ID_DonViQuiDoiMua, TenHangHoaMua: item.TenHangHoaMua, ID_NhomHangHoaMua: item.ID_NhomHangHoaMua, TenNhomHangHoaMua: item.TenNhomHangHoaMua, DiemCong: item.DiemCong, GiamGiaTheoPhanTram: true });
            }
            var $this = $('#dvtMuaHangTangDiem_' + item.ID);
            if (dk) {
                $this.removeClass('active-re');
            }
            else {
                $this.addClass('active-re');
            }
            console.log(self.HH_MuaHangTangDiem())
        }
    }
    // lấy về danh sách đơn vị
    //self.mangID_DonVi = ko.observableArray();
    //function getDonVi() {
    //    ajaxHelper(BH_DonViUri + "GetListDonVi_Use?ID_NguoiDung=" + _IDNguoiDung, "GET").done(function (data) {
    //        self.DonVis(data);
    //        var myDataDonVi = {};
    //        myDataDonVi.objDonVi = self.DonVis();
    //        for (var i = 0; i < self.DonVis().length; i++) {
    //            if (i == 0) {
    //                _nameChiNhanhKM = self.DonVis()[i].ID;
    //            }
    //            else {
    //                _nameChiNhanhKM = self.DonVis()[i].ID + "," + _nameChiNhanhKM;
    //            }
    //        }
    //        getAllNSNhanVien();
    //        _deleteChiNhanh = 1;
    //        _boolChiNhanh = true;
    //        _id_ChiNhanh_Use = _nameChiNhanhKM;
    //        GetListPromotion();
    //    });
    //}
    //getDonVi();

    //load đơn vị
    function getDonVi() {
        ajaxHelper(BH_DonViUri + "GetDonVi_byUserSearch?ID_NguoiDung=" + _IDDoiTuong + "&TenDonVi=" + _nameChiNhanhKM, "GET").done(function (data) {
            self.DonVis(data);
            self.searchDonVi(data);
            if (self.DonVis().length < 2)
                $('.showChiNhanh').hide();
            else
                $('.showChiNhanh').show();
            for (var i = 0; i < self.DonVis().length; i++) {
                if (i == 0) {
                    _nameChiNhanhKM = self.DonVis()[i].ID;
                }
                else {
                    _nameChiNhanhKM = self.DonVis()[i].ID + "," + _nameChiNhanhKM;
                }
            }
            getAllNSNhanVien();
            for (var i = 0; i < self.DonVis().length; i++) {
                if (self.DonVis()[i].ID == _nameDonViSeach) {
                    self.TenChiNhanh(self.DonVis()[i].TenDonVi);
                    self.SelectedDonVi_Use(self.DonVis()[i]);
                }
            }
        });
    }
    self.IDSelectedDV = ko.observableArray();
    $(document).on('click', '.per_ac1 li', function () {
        var ch = $(this).index();
        $(this).remove();
        var li = document.getElementById("selec-person");
        var list = li.getElementsByTagName("li");
        for (var i = 0; i < list.length; i++) {
            $("#selec-person ul li").eq(ch).find(".fa-check").css("display", "none");
        }
        var nameDV = _nameDonViSeach.split('-');
        _nameDonViSeach = null;
        for (var i = 0; i < nameDV.length; i++) {
            if (nameDV[i].trim() != $(this).text().trim()) {
                if (_nameDonViSeach == null) {
                    _nameDonViSeach = nameDV[i];
                }
                else {
                    _nameDonViSeach = nameDV[i] + "-" + _nameDonViSeach;
                }
            }
        }
        if (_nameDonViSeach.trim() == "null") {
        }
        else {
        }

    })

    self.CloseDonVi_Use = function (item) {
        _nameDonViSeach = null;
        var TenChiNhanh;
        self.MangChiNhanh_Use.remove(item);
        for (var i = 0; i < self.MangChiNhanh_Use().length; i++) {
            if (_nameDonViSeach == null) {
                _nameDonViSeach = self.MangChiNhanh_Use()[i].ID;
                TenChiNhanh = self.MangChiNhanh_Use()[i].TenDonVi;
            }
            else {
                _nameDonViSeach = self.MangChiNhanh_Use()[i].ID + "," + _nameDonViSeach;
                TenChiNhanh = TenChiNhanh + ", " + self.MangChiNhanh_Use()[i].TenDonVi;
            }
        }
        if (self.MangChiNhanh_Use().length === 0) {
            $("#NoteNameDonVi_Use").attr("placeholder", "Chọn chi nhánh...");
            TenChiNhanh = 'Tất cả chi nhánh.'
            for (var i = 0; i < self.searchDonVi().length; i++) {
                if (_nameDonViSeach == null)
                    _nameDonViSeach = self.searchDonVi()[i].ID;
                else
                    _nameDonViSeach = self.searchDonVi()[i].ID + "," + _nameDonViSeach;
            }
        }
        self.TenChiNhanh(TenChiNhanh);
        $('#selec-all-DonVi_Use li').each(function () {
            if ($(this).attr('id_use') === item.ID) {
                $(this).find('i').remove();
            }
        });
        _numberPage = 1;
        GetListPromotion();
    }

    self.SelectedDonVi_Use = function (item) {
        _nameDonViSeach = null;
        var TenChiNhanh;
        var arrIDDonVi = [];
        for (var i = 0; i < self.MangChiNhanh_Use().length; i++) {
            if ($.inArray(self.MangChiNhanh_Use()[i], arrIDDonVi) === -1) {
                arrIDDonVi.push(self.MangChiNhanh_Use()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDDonVi) === -1) {
            self.MangChiNhanh_Use.push(item);
            $('#NoteNameDonVi_Use').removeAttr('placeholder');
            for (var i = 0; i < self.MangChiNhanh_Use().length; i++) {
                if (_nameDonViSeach == null) {
                    _nameDonViSeach = self.MangChiNhanh_Use()[i].ID;
                    TenChiNhanh = self.MangChiNhanh_Use()[i].TenDonVi;
                }
                else {
                    _nameDonViSeach = self.MangChiNhanh_Use()[i].ID + "," + _nameDonViSeach;
                    TenChiNhanh = TenChiNhanh + ", " + self.MangChiNhanh_Use()[i].TenDonVi;
                }
            }
            self.TenChiNhanh(TenChiNhanh);
            _numberPage = 1;
            GetListPromotion();
        }
        //thêm dấu check vào đối tượng được chọn
        $('#selec-all-DonVi_Use li').each(function () {
            if ($(this).attr('id_use') === item.ID) {
                $(this).find('i').remove();
                $(this).append('<i class="fa fa-check check-after-li"></i>')
            }
        });

    }
    //lọc đơn vị
    self.NoteNameDonVi_Use = function () {
        var arrDonVi = [];
        var itemSearch = locdau($('#NoteNameDonVi_Use').val().toLowerCase());
        for (var i = 0; i < self.searchDonVi().length; i++) {
            var locdauInput = locdau(self.searchDonVi()[i].TenDonVi).toLowerCase();
            var R = locdauInput.split(itemSearch);
            if (R.length > 1) {
                arrDonVi.push(self.searchDonVi()[i]);
            }
        }
        self.DonVis(arrDonVi);
        if ($('#NoteNameDonVi_Use').val() == "") {
            self.DonVis(self.searchDonVi());
        }
    }
    $('#NoteNameDonVi_Use').keypress(function (e) {
        if (e.keyCode == 13 && self.DonVis().length > 0) {
            self.SelectedDonVi_Use(self.DonVis()[0]);
        }
    });

    self.Filter_TypePromotion.subscribe(function () {
        GetListPromotion();
    });

    self.Filter_Expired.subscribe(function () {
        GetListPromotion();
    });

    self.StatusActive.subscribe(function () {
        GetListPromotion();
    });

    function GetListPromotion() {
        var txt = $('#EnterMaKM').val();
        var arrIDDonVi = [];
        for (var i = 0; i < self.MangChiNhanh_Use().length; i++) {
            if ($.inArray(self.MangChiNhanh_Use()[i], arrIDDonVi) === -1) {
                arrIDDonVi.push(self.MangChiNhanh_Use()[i].ID);
            }
        }
        if (arrIDDonVi.length === 0) {
            arrIDDonVi = [_id_DonViLS];
        }
        var obj = {
            IDChiNhanhs: arrIDDonVi,
            TextSearch: txt,
            StatusActive: self.StatusActive(),
            TypePromotion: self.Filter_TypePromotion(),
            Expired: self.Filter_Expired(),
            CurrentPage: self.CurrentPage() - 1,
            PageSize: self.pageSize(),
        };
        console.log(obj)
        ajaxHelper(BH_KhuyenMaiUri + 'GetListPromotion', 'POST', obj).done(function (x) {
            if (x.res === true) {
             
                if (x.data.length > 0) {
                    AllPage = x.ToTalPage;
                    self.filteredDM_KhuyenMai(x.data);
                    self.PagesKhuyenMai([]);
                    for (let i = 0; i < x.TotalPage; i++) {
                        self.PagesKhuyenMai.push({ SoTrang: i + 1 });
                    }
                    self.RowsStart((_numberPage - 1) * _numberRowns + 1);
                    self.RowsEnd((_numberPage - 1) * _numberRowns + self.filteredDM_KhuyenMai().length);
                }
                else {
                    self.filteredDM_KhuyenMai([]);
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                self.RowsKhuyenMai(x.TotalRow);
                self.ReserPage();
            }
            else {
                ShowMessage_Danger(x.mes);
            }
        });
    }
    //lấy về danh sách chương trình khuyến mại
    function getListKhuyenMai() {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('KhuyenMai_XemDS', lc_CTQuyen) > -1) {
            var lc_KhuyenMaiLS = localStorage.getItem('Ls_KhuyenMai');
            if (lc_KhuyenMaiLS != null) {
                $('#EnterMaKM').val(lc_KhuyenMaiLS);
                _maseach_KM = lc_KhuyenMaiLS;
                hidewait('table_h');
                ajaxHelper(BH_KhuyenMaiUri + "GetListChuongTrinhKM?maKM=" + _maseach_KM + "&Chinhanh=" + _nameDonViSeach + "&TrangThai=" + _Trangthai_seach + "&sohang=" + _numberRowns + "&page=" + _numberPage, "GET").done(function (data) {
                    self.filteredDM_KhuyenMai(data.LstData);
                    LoadHtmlGrid();
                    if (self.filteredDM_KhuyenMai().length != 0) {
                        self.RowsStart((_numberPage - 1) * _numberRowns + 1);
                        self.RowsEnd((_numberPage - 1) * _numberRowns + self.filteredDM_KhuyenMai().length)
                    }
                    else {
                        self.RowsStart('0');
                        self.RowsEnd('0');
                    }
                    self.RowsKhuyenMai(data.Rowcount);
                    self.PagesKhuyenMai(data.LstPageNumber);
                    AllPage = self.PagesKhuyenMai().length;
                    self.ReserPage();
                    localStorage.removeItem('Ls_KhuyenMai');
                });
                $("div[id ^= 'wait']").text("");
            }
            else {
                hidewait('table_h');
                ajaxHelper(BH_KhuyenMaiUri + "GetListChuongTrinhKM?maKM=" + _maseach_KM + "&Chinhanh=" + _nameDonViSeach + "&TrangThai=" + _Trangthai_seach + "&sohang=" + _numberRowns + "&page=" + _numberPage, "GET").done(function (data) {
                    self.filteredDM_KhuyenMai(data.LstData);
                    console.log(22, data);

                    LoadHtmlGrid();
                    if (self.filteredDM_KhuyenMai().length != 0) {
                        self.RowsStart((_numberPage - 1) * _numberRowns + 1);
                        self.RowsEnd((_numberPage - 1) * _numberRowns + self.filteredDM_KhuyenMai().length)
                    }
                    else {
                        self.RowsStart('0');
                        self.RowsEnd('0');
                    }
                    self.RowsKhuyenMai(data.Rowcount);
                    self.PagesKhuyenMai(data.LstPageNumber);
                    AllPage = self.PagesKhuyenMai().length;
                    self.ReserPage();
                    //console.log(self.filteredDM_KhuyenMai());
                });
                $("div[id ^= 'wait']").text("");
            }
        }
    }
    var _id_LSKhuyenMai = null;
    self.getList_LichSuKhuyenMai = function () {
        ajaxHelper(BH_KhuyenMaiUri + "getList_LichSuKhuyenMai?ID_KhuyenMai=" + _id_LSKhuyenMai + "&numberPage=" + _numberPage_LS + "&PageSize=" + _numberRowns_LS, "GET").done(function (data) {
            self.BH_LichSuKhuyenMai(data.LstData);
            console.log(data.LstData);
            if (self.BH_LichSuKhuyenMai().length > 0) {
                self.RowsStart_LS((_numberPage_LS - 1) * _numberRowns_LS + 1);
                self.RowsEnd_LS((_numberPage_LS - 1) * _numberRowns_LS + self.BH_LichSuKhuyenMai().length)
            }
            else {
                self.RowsStart_LS('0');
                self.RowsEnd_LS('0');
            }
            self.Rows_LichSuKhuyenMai(data.Rowcount);
            self.Pages_LichSuKhuyenMai(data.LstPageNumber);
            AllPage_LS = self.Pages_LichSuKhuyenMai().length;
            self.ReserPage_LS();

            //self.RowsStart_LS((_numberPage_LS - 1) * _numberRowns_LS + 1);
            //self.RowsEnd_LS((_numberPage_LS - 1) * _numberRowns_LS + self.BH_LichSuKhuyenMai().length);
            //self.ReserPage_LS();
        });
    }
    function hidewait(o) {
        $('.' + o).append('<div id="wait"><img src="/Content/images/wait.gif" width="64" height="64" /><div class="happy-wait">' +
            ' </div>' +
            '</div>')
    }
    self.gotoNextPage = function (item) {
        try {
            var kt = item.SoTrang;
            _numberPage = item.SoTrang;
            self.CurrentPage(item.SoTrang);
            GetListPromotion();
        }
        catch (e) {
        }

    }
    // Phân trang lich su khuyen mai
    self.gotoNextPage_LS = function (item) {
        try {
            var kt = item.SoTrang;
            _numberPage_LS = item.SoTrang;
            self.getList_LichSuKhuyenMai();
        }
        catch (e) {
        }

    }
    self.GetClass_LS = function (page) {
        return (page.SoTrang === self.currentPage_LS()) ? "click" : "";
    };
    self.selecPage_LS = function () {
        //AllPage_LS = self.Pages_LichSuKhuyenMai().length;
        if (AllPage_LS > 4) {
            for (var i = 3; i < AllPage_LS; i++) {
                self.Pages_LichSuKhuyenMai.pop(i + 1);
            }
            self.Pages_LichSuKhuyenMai.push({ SoTrang: '4' });
            self.Pages_LichSuKhuyenMai.push({ SoTrang: '5' });
        }
        else {
            for (var i = 0; i < 6; i++) {
                self.Pages_LichSuKhuyenMai.pop(i);
            }
            for (var j = 0; j < AllPage_LS; j++) {
                self.Pages_LichSuKhuyenMai.push({ SoTrang: j + 1 });
            }
        }
        $('#StartPage_LS' + _id_LSKhuyenMai).hide();
        $('#BackPage_LS' + _id_LSKhuyenMai).hide();
        $('#NextPage_LS' + _id_LSKhuyenMai).show();
        $('#EndPage_LS' + _id_LSKhuyenMai).show();
    }
    self.ReserPage_LS = function (item) {
        self.selecPage_LS();
        if (_numberPage_LS > 1 && AllPage_LS > 5/* && nextPage < AllPage - 1*/) {
            if (_numberPage_LS > 3 && _numberPage_LS < AllPage_LS - 1) {
                for (var i = 0; i < 5; i++) {
                    self.Pages_LichSuKhuyenMai.replace(self.Pages_LichSuKhuyenMai()[i], { SoTrang: parseInt(_numberPage_LS) + i - 2 });
                }
            }
            else if (parseInt(_numberPage_LS) === parseInt(AllPage_LS) - 1) {
                for (var i = 0; i < 5; i++) {
                    self.Pages_LichSuKhuyenMai.replace(self.Pages_LichSuKhuyenMai()[i], { SoTrang: parseInt(_numberPage_LS) + i - 3 });
                }
            }
            else if (_numberPage_LS == AllPage_LS) {
                for (var i = 0; i < 5; i++) {
                    self.Pages_LichSuKhuyenMai.replace(self.Pages_LichSuKhuyenMai()[i], { SoTrang: parseInt(_numberPage_LS) + i - 4 });
                }
            }
        }
        self.currentPage_LS(parseInt(_numberPage_LS));
        if (_numberPage_LS > 1) {
            $('#StartPage_LS' + _id_LSKhuyenMai).show();
            $('#BackPage_LS' + _id_LSKhuyenMai).show();
        }
        else {
            $('#StartPage_LS' + _id_LSKhuyenMai).hide();
            $('#BackPage_LS' + _id_LSKhuyenMai).hide();
        }
        if (_numberPage_LS == AllPage_LS) {
            $('#NextPage_LS' + _id_LSKhuyenMai).hide();
            $('#EndPage_LS' + _id_LSKhuyenMai).hide();
        }
        else {
            $('#NextPage_LS' + _id_LSKhuyenMai).show();
            $('#EndPage_LS' + _id_LSKhuyenMai).show();
        }
    }
    self.NextPage_LS = function (item) {
        if (_numberPage_LS < AllPage_LS) {
            _numberPage_LS = _numberPage_LS + 1;
            self.ReserPage_LS();
            self.getList_LichSuKhuyenMai();
        }
    };
    self.BackPage_LS = function (item) {
        if (_numberPage_LS > 1) {
            _numberPage_LS = _numberPage_LS - 1;
            self.ReserPage_LS();
            self.getList_LichSuKhuyenMai();
        }
    };
    self.EndPage_LS = function (item) {
        _numberPage_LS = AllPage_LS;
        self.ReserPage_LS();
        self.getList_LichSuKhuyenMai();
    };
    self.StartPage_LS = function (item) {
        _numberPage_LS = 1;
        self.getList_LichSuKhuyenMai();
        self.ReserPage_LS();
    };
    // Phân trang
    self.GetClass = function (page) {
        return (page.SoTrang === self.CurrentPage()) ? "click" : "";
    };
    self.selecPage = function () {
        AllPage = self.PagesKhuyenMai().length;
        if (AllPage > 4) {
            for (var i = 3; i < AllPage; i++) {
                self.PagesKhuyenMai.pop(i + 1);
            }
            self.PagesKhuyenMai.push({ SoTrang: '4' });
            self.PagesKhuyenMai.push({ SoTrang: '5' });
        }
        else {
            for (var i = 0; i < 6; i++) {
                self.PagesKhuyenMai.pop(i);
            }
            for (var j = 0; j < AllPage; j++) {
                self.PagesKhuyenMai.push({ SoTrang: j + 1 });
            }
        }
        $('#StartPage').hide();
        $('#BackPage').hide();
        $('#NextPage').show();
        $('#EndPage').show();
    }
    self.ReserPage = function (item) {
        self.selecPage();
        if (_numberPage > 1 && AllPage > 5/* && nextPage < AllPage - 1*/) {
            if (_numberPage > 3 && _numberPage < AllPage - 1) {
                for (var i = 0; i < 5; i++) {
                    self.PagesKhuyenMai.replace(self.PagesKhuyenMai()[i], { SoTrang: parseInt(_numberPage) + i - 2 });
                }
            }
            else if (parseInt(_numberPage) === parseInt(AllPage) - 1) {
                for (var i = 0; i < 5; i++) {
                    self.PagesKhuyenMai.replace(self.PagesKhuyenMai()[i], { SoTrang: parseInt(_numberPage) + i - 3 });
                }
            }
            else if (_numberPage == AllPage) {
                for (var i = 0; i < 5; i++) {
                    self.PagesKhuyenMai.replace(self.PagesKhuyenMai()[i], { SoTrang: parseInt(_numberPage) + i - 4 });
                }
            }
        }
        self.CurrentPage(parseInt(_numberPage));
        if (_numberPage > 1) {
            $('#StartPage').show();
            $('#BackPage').show();
        }
        else {
            $('#StartPage').hide();
            $('#BackPage').hide();
        }
        if (_numberPage == AllPage) {
            $('#NextPage').hide();
            $('#EndPage').hide();
        }
        else {
            $('#NextPage').show();
            $('#EndPage').show();
        }
    }
    self.NextPage = function (item) {
        if (_numberPage < AllPage) {
            _numberPage = _numberPage + 1;
            self.ReserPage();
            GetListPromotion();
        }
    };
    self.BackPage = function (item) {
        if (_numberPage > 1) {
            _numberPage = _numberPage - 1;
            self.ReserPage();
            GetListPromotion();
        }
    };
    self.EndPage = function (item) {
        _numberPage = AllPage;
        self.ReserPage();
        GetListPromotion();
    };
    self.StartPage = function (item) {
        _numberPage = 1;
        GetListPromotion();
        self.ReserPage();
        //self.gotoNextPage();
    };


    //lấy về danh sách hàng hóa
    //function getListHangHoa() {
    //    ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "GetListHangHoas_QuyDoiNH?iddonvi=" + _IDchinhanh, 'GET').done(function (data) {
    //        self.HangHoas(data);
    //        //console.log(self.HangHoas())
    //    })
    //}
    //getListHangHoa();
    // tim kiem JqAuto hàng hóa
    self.filterFind = function (item, inputString) {
        var itemSearch = locdau(item.TenHangHoa).toLowerCase();
        var itemSearch1 = locdau(item.MaHangHoa).toLowerCase();

        var locdauInput = locdau(inputString).toLowerCase();
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
    self.HD_TangHang_selectHH = function (item) {
        console.log(item.ID)
    }
    // lựa chọn hàng hóa cụ thể
    var _KieuNhomHang;
    var _note_ID_TangHang;
    var _note_TongTienHang_TangHang;
    var _note_GiamGia_TangHang;
    var _note_DonViTinh_TangHang;
    var _note_SoLuong_TangHang;
    var _note_SoLuongMua_TangHang;
    var _note_ID_DonViQuiDoi_TangHang;
    var _note_TenHangHoa_TangHang;
    var _note_ID_NhomHangHoa_TangHang;
    var _note_TenNhomHH_TangHang;
    var _note_ID_DonViQuiDoiMua_TangHang;
    var _note_TenHangHoaMua_TangHang;
    var _note_ID_NhomHangHoaMua_TangHang;
    var _note_TenNhomHHMua_TangHang;
    var _note_ID_NhomHangHoa_Select;
    var _note_ID_NhomHangHoaMua_Select;
    var _note_TenNhomHH_Select;
    self.resetHD_TangHang = function () {

    }
    self.selectedHangHoa.subscribe(function (newValue) {
        //ajaxHelper(BH_KhuyenMaiUri + "GetHangHoa_ByIDQuyDoi/" + newValue, 'GET').done(function (data) {
        //    _id_DonViQuyDoi = data.ID_DonViQuiDoi;
        //    _note_ID_NhomHangHoa_TangHang = null;
        //    _note_TenHangHoa_TangHang = data.TenHangHoa;
        //    //console.log(_id_DonViQuyDoi);
        //    if (_hinhthucKM == 12) {
        //        self.HD_TangHang.replace(SaveItem, {
        //            ID: SaveItem.ID,
        //            TongTienHang: SaveItem.TongTienHang,
        //            SoLuong: SaveItem.SoLuong,
        //            ID_DonViQuiDoi: _id_DonViQuyDoi,
        //            TenHangHoa: _note_TenHangHoa_TangHang,
        //            ID_NhomHangHoa: null,
        //            TenNhomHangHoa: null
        //        })
        //        console.log(self.HD_TangHang());
        //        self.SaveHD_TangHang(self.HD_TangHang());
        //        self.HD_TangHang([]);
        //        for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
        //            self.HD_TangHang.push(self.SaveHD_TangHang()[i]);
        //        }
        //        //self.HD_TangHang(self.SaveHD_TangHang);
        //    }
        //    else if (_hinhthucKM == 13) {
        //        console.log(SaveItem);
        //        self.HD_GiamGiaHang.replace(SaveItem, {
        //            ID: SaveItem.ID,
        //            TongTienHang: SaveItem.TongTienHang,
        //            GiamGia: SaveItem.GiamGia,
        //            GiamGiaTheoPhanTram: SaveItem.GiamGiaTheoPhanTram,
        //            SoLuong: SaveItem.SoLuong,
        //            ID_DonViQuiDoi: _id_DonViQuyDoi,
        //            TenHangHoa: _note_TenHangHoa_TangHang,
        //            ID_NhomHangHoa: null,
        //            TenNhomHangHoa: null
        //        });
        //        console.log(self.HD_GiamGiaHang());
        //        self.SaveHD_TangHang(self.HD_GiamGiaHang());
        //        self.HD_GiamGiaHang([]);
        //        for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
        //            self.HD_GiamGiaHang.push(self.SaveHD_TangHang()[i]);
        //        }
        //        for (var i = 0; i < self.HD_GiamGiaHang().length; i++) {
        //            var $this = $('#dvtGiamGiaHang_' + self.HD_GiamGiaHang()[i].ID);
        //            if (self.HD_GiamGiaHang()[i].GiamGiaTheoPhanTram) {
        //                $this.addClass('active-re');
        //            }
        //            else {
        //                $this.removeClass('active-re');
        //            }
        //        }

        //    }
        //    else if (_hinhthucKM == 21) {
        //        self.HH_MuaHangGiamGia.replace(SaveItem, {
        //            ID: SaveItem.ID,
        //            SoLuongMua: SaveItem.SoLuongMua,
        //            ID_DonViQuiDoiMua: SaveItem.ID_DonViQuiDoiMua,
        //            TenHangHoaMua: SaveItem.TenHangHoaMua,
        //            ID_NhomHangHoaMua: SaveItem.ID_NhomHangHoaMua,
        //            TenNhomHangHoaMua: SaveItem.TenNhomHangHoaMua,
        //            GiamGia: SaveItem.GiamGia,
        //            GiamGiaTheoPhanTram: SaveItem.GiamGiaTheoPhanTram,
        //            SoLuong: SaveItem.SoLuong,
        //            ID_DonViQuiDoi: _id_DonViQuyDoi,
        //            TenHangHoa: _note_TenHangHoa_TangHang,
        //            ID_NhomHangHoa: null,
        //            TenNhomHangHoa: null
        //        })
        //        console.log(self.HH_MuaHangGiamGia());
        //        self.SaveHD_TangHang(self.HH_MuaHangGiamGia());
        //        self.HH_MuaHangGiamGia([]);
        //        for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
        //            self.HH_MuaHangGiamGia.push(self.SaveHD_TangHang()[i]);
        //        }
        //        for (var i = 0; i < self.HH_MuaHangGiamGia().length; i++) {
        //            var $this = $('#dvtMuaHangGiamGia_' + self.HH_MuaHangGiamGia()[i].ID);
        //            if (self.HH_MuaHangGiamGia()[i].GiamGiaTheoPhanTram) {
        //                $this.addClass('active-re');
        //            }
        //            else {
        //                $this.removeClass('active-re');
        //            }
        //        }

        //    }
        //    else if (_hinhthucKM == 22) {
        //        self.HH_MuaHangTangHang.replace(SaveItem, {
        //            ID: SaveItem.ID,
        //            SoLuongMua: SaveItem.SoLuongMua,
        //            ID_DonViQuiDoiMua: SaveItem.ID_DonViQuiDoiMua,
        //            TenHangHoaMua: SaveItem.TenHangHoaMua,
        //            ID_NhomHangHoaMua: SaveItem.ID_NhomHangHoaMua,
        //            TenNhomHangHoaMua: SaveItem.TenNhomHangHoaMua,
        //            SoLuong: SaveItem.SoLuong,
        //            ID_DonViQuiDoi: _id_DonViQuyDoi,
        //            TenHangHoa: _note_TenHangHoa_TangHang,
        //            ID_NhomHangHoa: null,
        //            TenNhomHangHoa: null
        //        })
        //        console.log(self.HH_MuaHangTangHang())
        //        self.SaveHD_TangHang(self.HH_MuaHangTangHang());
        //        self.HH_MuaHangTangHang([]);
        //        for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
        //            self.HH_MuaHangTangHang.push(self.SaveHD_TangHang()[i]);
        //        }
        //    }
        //});

    });

    self.selectedHangHoaMua.subscribe(function (newValue) {
    });
    var SaveItem;
    var dk_Enter = 1;
    self.clickTangHang = function (item) {
        SaveItem = item;
        dk_Enter = 1;
    }
    self.clickTangHangMua = function (item) {
        SaveItem = item;
        dk_Enter = 2;
    }

    function getChiTietHangHoaByID(id) {
        var lc_PhieuHuy = localStorage.getItem('Lc_TangHang'); // khai báo cache chứa danh sách hàng hóa trong phiếu hủy
        ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "GetHangHoa_ByIDQuyDoi?id=" + id + '&iddonvi=' + _IDchinhanh, 'GET').done(function (data) {
            var found = -1;
            var soluongNew;
            var ob1 = {
                ID: data.ID, // ID_DonViQuyDoi
                ID_DonViQuiDoi: data.ID_DonViQuiDoi,
                SoLuong: 1,
                TenHangHoa: data.TenHangHoa,
                TonKho: data.TonKho,
                GiaTriHuy: data.GiaVon,
                //ID_DonViQuiDoi: data.ID,
                MaHangHoa: data.MaHangHoa,
                TrangThaiMoPhieu: 2
                /*TonCNNhap: 0,*/
            }
            // console.log("cthd:" + JSON.stringify(ob1))
            if (lc_PhieuHuy == null) { // trả về danh sách trống khi cache rỗng
                lc_PhieuHuy = [];
            }
            else {
                lc_PhieuHuy = JSON.parse(lc_PhieuHuy);

                for (var i = 0; i < lc_PhieuHuy.length; i++) {
                    if (lc_PhieuHuy[i].ID == id) {
                        found = i;
                        soluongNew = parseInt(lc_PhieuHuy[i].SoLuong);
                        break;
                    }
                }
            }

            if (found < 0) {
                lc_PhieuHuy.unshift(ob1); // add hàng hóa được chọn vào
                localStorage.setItem('lc_PhieuHuy', JSON.stringify(lc_PhieuHuy)); // lưu cache
            }
            else {

                for (var i = 0; i < lc_PhieuHuy.length; i++) {
                    if (lc_PhieuHuy[i].ID == id) {
                        // remove 1 item at index = i
                        lc_PhieuHuy.splice(i, 1); // gỡ bỏ đối tượng đã được chọn trước đó
                        break;
                    }
                }
                // luu cache sau khi xoa
                // localStorage.setItem('lc_PhieuHuy', JSON.stringify(lc_PhieuHuy));
                ob1.SoLuong = soluongNew + 1;
                ob1.GiaTriHuy = ob1.SoLuong * ob1.GiaVon;
                lc_PhieuHuy.unshift(ob1);
                localStorage.setItem('lc_PhieuHuy', JSON.stringify(lc_PhieuHuy));
            }
            self.newHoaDon().BH_HoaDon_ChiTiet(lc_PhieuHuy);
            self.resetCache();
        });
    }
    //Lựa chọn kiểu áp dụng ngày sinh nhật
    $('.selectApDungSN li').on('click', function () {
        _apdungngaysinhnhat = $(this).val();
        console.log(_apdungngaysinhnhat);
    })
    $('.selectApDungSN input').on('click', function () {
        if (_apdungngaysinhnhat == 0) {
            _apdungngaysinhnhat = 1;
            $('#choose_NgaySN').attr("data-toggle", "dropdown");
        }
        else {
            _apdungngaysinhnhat = 0;
            $('#choose_NgaySN').removeAttr('data-toggle');
            $(this).parentsUntil(".checkdram ").find(".blue").html('Ngày sinh nhật');
            $(".checkdram ul li i").hide();
        }
        //console.log(_apdungngaysinhnhat);
    })
    self.CheckNgaySinhNhat = function () {
        if (_apdungngaysinhnhat == 0) {
            _apdungngaysinhnhat = 1;
        }
        else {
            _apdungngaysinhnhat = 0;
        }
        console.log(_apdungngaysinhnhat);
    }
    //Lựa chọn ngày
    $('.choseNgayKhuyenMai li').on('click', function () {
        var ob = {
            NgayKhuyenMai: $(this).val()
        }
        self.mangNgayKM.push(ob);
    });
    self.SelectedNgayKM = function (item) {
        var arrIDNgayKM = [];
        for (var i = 0; i < self.mangNgayKM().length; i++) {
            if ($.inArray(self.mangNgayKM()[i], arrIDNgayKM) === -1) {
                arrIDNgayKM.push(self.mangNgayKM()[i]);
            }
        }
        if ($.inArray(item, arrIDNgayKM) === -1) {
            self.mangNgayKM.push(item);
        }
        //thêm dấu check vào đối tượng được chọn
        //$('#selec-all-NgayKM li').each(function () {
        //    if ($(this).attr('id') === item) {
        //        $(this).find('i').remove();
        //        $(this).append('<i class="fa fa-check check-after-li"></i>')
        //    }
        //});

        $('#NoteNgayKhuyenMai').val('');
        self.NgayKM(mangNgay);
        //đánh dấu check
        for (var i = 0; i < self.mangNgayKM().length; i++) {
            $('#selec-all-NgayKM li').each(function () {
                if ($(this).attr('id') === self.mangNgayKM()[i]) {
                    $(this).find('i').remove();
                    $(this).append('<i class="fa fa-check check-after-li"></i>')
                }
            });
        }
        console.log(self.mangNgayKM())
    }
    self.CloseNgayKM = function (item) {
        self.mangNgayKM.remove(item);
        $('#selec-all-NgayKM li').each(function () {
            if ($(this).attr('id') === item) {
                $(this).find('i').remove();
            }
        });
        console.log(self.mangNgayKM())
    }
    //Lựa chọn tháng
    self.SelectedThangKM = function (item) {
        var arrIDThangKM = [];
        for (var i = 0; i < self.mangThangKM().length; i++) {
            if ($.inArray(self.mangThangKM()[i], arrIDThangKM) === -1) {
                arrIDThangKM.push(self.mangThangKM()[i]);
            }
        }
        if ($.inArray(item, arrIDThangKM) === -1) {
            self.mangThangKM.push(item);
        }
        //thêm dấu check vào đối tượng được chọn
        //$('#selec-all-ThangKM li').each(function () {
        //    if ($(this).attr('id') === item) {
        //        $(this).find('i').remove();
        //        $(this).append('<i class="fa fa-check check-after-li"></i>')
        //    }
        //});
        $('#NoteThangKhuyenMai').val('');
        self.ThangKM(mangThang);
        //đánh dấu check
        for (var i = 0; i < self.mangThangKM().length; i++) {
            $('#selec-all-ThangKM li').each(function () {
                if ($(this).attr('id') === self.mangThangKM()[i]) {
                    $(this).find('i').remove();
                    $(this).append('<i class="fa fa-check check-after-li"></i>')
                }
            });
        }
        console.log(self.mangThangKM())
    }
    self.CloseThangKM = function (item) {
        self.mangThangKM.remove(item);
        $('#selec-all-ThangKM li').each(function () {
            if ($(this).attr('id') === item) {
                $(this).find('i').remove();
            }
        });
        console.log(self.mangThangKM())
    }
    $('.choseThangKhuyenMai li').on('click', function () {
        var ob = {
            ThangKhuyenMai: $(this).val()
        }
        self.mangThangKM.push(ob);
    });
    //lựa chọn thứ KM
    self.SelectedThuKM = function (item) {
        var arrIDThuKM = [];
        for (var i = 0; i < self.mangThuKM().length; i++) {
            if ($.inArray(self.mangThuKM()[i], arrIDThuKM) === -1) {
                arrIDThuKM.push(self.mangThuKM()[i]);
            }
        }
        if ($.inArray(item, arrIDThuKM) === -1) {
            self.mangThuKM.push(item);
        }
        $('#NoteThuKhuyenMai').val('');
        self.ThuKM(mangThu);
        //đánh dấu check
        for (var i = 0; i < self.mangThuKM().length; i++) {
            $('#selec-all-ThuKM li').each(function () {
                if ($(this).attr('id') === self.mangThuKM()[i]) {
                    $(this).find('i').remove();
                    $(this).append('<i class="fa fa-check check-after-li"></i>')
                }
            });
        }
        console.log(self.mangThuKM())
    }
    self.CloseThuKM = function (item) {
        self.mangThuKM.remove(item);
        $('#selec-all-ThuKM li').each(function () {
            if ($(this).attr('id') === item) {
                $(this).find('i').remove();
            }
        });
        console.log(self.mangThuKM())
    }
    $('.choseThuKhuyenMai li').on('click', function () {
        var ob = {
            ThuKhuyenMai: $(this).val()
        }
        self.mangThuKM.push(ob);
    });
    //lựa chọn giờ KM
    self.SelectedGioKM = function (item) {
        var arrIDGioKM = [];
        for (var i = 0; i < self.mangGioKM().length; i++) {
            if ($.inArray(self.mangGioKM()[i], arrIDGioKM) === -1) {
                arrIDGioKM.push(self.mangGioKM()[i]);
            }
        }
        if ($.inArray(item, arrIDGioKM) === -1) {
            self.mangGioKM.push(item);
        }
        //thêm dấu check vào đối tượng được chọn
        $('#selec-all-GioKM li').each(function () {
            if ($(this).attr('id') === item) {
                $(this).find('i').remove();
                $(this).append('<i class="fa fa-check check-after-li"></i>')
            }
        });
        console.log(self.mangGioKM())
    }
    self.CloseGioKM = function (item) {
        self.mangGioKM.remove(item);
        $('#selec-all-GioKM li').each(function () {
            if ($(this).attr('id') === item) {
                $(this).find('i').remove();
            }
        });
        console.log(self.mangGioKM())
    }
    $('.choseGioKhuyenMai li').on('click', function () {
        var ob = {
            GioKhuyenMai: $(this).val()
        }
        self.mangGioKM.push(ob);
    });
    //Lua chon nhom khach hang
    self.closeNhomKhachHang = function (item) {
        self.MangNhomKhachHang.remove(item);
        if (self.MangNhomKhachHang().length === 0) {
            $('#choose-NhomKhachHang').append('<input type="text" class="dropdown" placeholder="Chọn nhóm khách hàng áp dụng...">');
        }
        $('#selec-all-NhomKhachHang li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
    }
    self.SelectedNhomKhachHang = function (item) {
        var arrIDNhomKH = [];
        for (var i = 0; i < self.MangNhomKhachHang().length; i++) {
            if ($.inArray(self.MangNhomKhachHang()[i], arrIDNhomKH) === -1) {
                arrIDNhomKH.push(self.MangNhomKhachHang()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDNhomKH) === -1) {
            self.MangNhomKhachHang.push(item);
        }
        $('#choose-NhomKhachHang input').remove();
        //thêm dấu check vào đối tượng được chọn
        $('#selec-all-NhomKhachHang li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
                $(this).append('<i class="fa fa-check check-after-li"></i>')
            }
        });
        console.log(self.MangNhomKhachHang())

        //_id_NhomKhachHang = item.ID;
        //_nameNhomKhachHang = item.TenNhomDoiTuong;
        ////console.log(_id_NhomKhachHang, _nameNhomKhachHang);
        //self.NoteNhomKhachHang();
    }
    //Lựa chọn người bán
    self.CloseNhanVien = function (item) {
        self.MangNhanVien.remove(item);
        if (self.MangNhanVien().length === 0) {
            $('#choose-NguoiBan').append('<input type="text" class="dropdown" placeholder="Chọn người bán áp dụng...">');
        }
        $('#selec-all-NguoiBan li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        console.log(self.MangNhanVien())
    }
    self.SelectedNhanVien = function (item) {
        //_id_NhanVien = item.ID;
        //_nameNhanVien = item.TenNhanVien;
        //self.NoteNhanVien();

        var arrIDNhanVien = [];
        for (var i = 0; i < self.MangNhanVien().length; i++) {
            if ($.inArray(self.MangNhanVien()[i], arrIDNhanVien) === -1) {
                arrIDNhanVien.push(self.MangNhanVien()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDNhanVien) === -1) {
            self.MangNhanVien.push(item);
        }
        $('#choose-NguoiBan input').remove();
        //thêm dấu check vào đối tượng được chọn
        $('#selec-all-NguoiBan li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
                $(this).append('<i class="fa fa-check check-after-li"></i>')
            }
        });
        console.log(self.MangNhanVien())
    }
    // lựa chọn đơn vị
    self.CloseDonVi = function (item) {
        _nameChiNhanhKM = null;
        self.MangChiNhanh.remove(item);
        for (var i = 0; i < self.MangChiNhanh().length; i++) {
            if (_nameChiNhanhKM == null)
                _nameChiNhanhKM = self.MangChiNhanh()[i].ID;
            else
                _nameChiNhanhKM = self.MangChiNhanh()[i].ID + "," + _nameChiNhanhKM;
        }
        if (self.MangChiNhanh().length === 0) {
            $('#choose_DonVi').append('<input type="text" class="dropdown" placeholder="Chọn chi nhánh áp dụng...">');
            getDonVi();
        }
        // remove check
        $('#selec-all-DonVi li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        getAllNSNhanVien();
    }
    self.SelectedDonVi = function (item) {
        _nameChiNhanhKM = null;
        var arrIDDonVi = [];
        for (var i = 0; i < self.MangChiNhanh().length; i++) {
            if ($.inArray(self.MangChiNhanh()[i], arrIDDonVi) === -1) {
                arrIDDonVi.push(self.MangChiNhanh()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDDonVi) === -1) {
            self.MangChiNhanh.push(item);
            for (var i = 0; i < self.MangChiNhanh().length; i++) {
                if (_nameChiNhanhKM == null)
                    _nameChiNhanhKM = self.MangChiNhanh()[i].ID;
                else
                    _nameChiNhanhKM = self.MangChiNhanh()[i].ID + "," + _nameChiNhanhKM;
            }

        }
        $('#choose_DonVi input').remove();
        //thêm dấu check vào đối tượng được chọn
        $('#selec-all-DonVi li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
                $(this).append('<i class="fa fa-check check-after-li"></i>')
            }
        });
        getAllNSNhanVien();
    }
    

    //lấy về tên người dùng
    function getnameuse() {
        ajaxHelper(BH_KhuyenMaiUri + "getNameNguoiDung?ID_NguoiDung=" + _IDNguoiDung, "GET").done(function (data) {
            _nguoisua = data;
        })
    }

   
    var _tennhanvien_seach = null;
    function getAllNSNhanVien() {
        ajaxHelper(NhanVienUri + "getListNhanVien_DonVi?ID_ChiNhanh=" + _nameChiNhanhKM + "&nameNV=" + _tennhanvien_seach, 'GET').done(function (data) {
            self.NhanViens(data);
        });
    }
    //getAllNSNhanVien();
    self.ShowNhomHang = function (item) {
        _KieuNhomHang = 1;
        SaveItem = item;
        $('#listgroupkm').modal("show")
    }
    self.ShowNhomHangMua = function (item) {
        _KieuNhomHang = 2;
        SaveItem = item;
        $('#listgroupkm').modal("show")
    }
    self.SelectID_NhomHangHoa = function (item) {
        _note_ID_NhomHangHoa_Select = item.ID;
        _note_TenNhomHH_Select = item.TenNhomHangHoa;
        var $this = $('#input_NhomHang_' + item.ID)
        $(".list-choose ul li div").removeClass('SelectID_NhomHangKM');
        $this.addClass('SelectID_NhomHangKM');
    }
    self.SelectedNhomHH = function () {
        if (_KieuNhomHang == 1) {
            _note_ID_NhomHangHoa_TangHang = _note_ID_NhomHangHoa_Select;
            if (_hinhthucKM == 12) {
                self.HD_TangHang.replace(SaveItem, {
                    ID: SaveItem.ID,
                    TongTienHang:
                    SaveItem.TongTienHang,
                    SoLuong: SaveItem.SoLuong,
                    ID_DonViQuiDoi: _note_ID_DonViQuiDoi_TangHang,
                    TenHangHoa: null,
                    ID_NhomHangHoa: _note_ID_NhomHangHoa_TangHang,
                    TenNhomHangHoa: _note_TenNhomHH_Select
                })
                console.log(self.HD_TangHang());
            }
            else if (_hinhthucKM == 13) {
                self.HD_GiamGiaHang.replace(SaveItem, {
                    ID: SaveItem.ID,
                    TongTienHang: SaveItem.TongTienHang,
                    GiamGia: SaveItem.GiamGia,
                    GiamGiaTheoPhanTram: SaveItem.GiamGiaTheoPhanTram,
                    SoLuong: SaveItem.SoLuong,
                    ID_DonViQuiDoi: null,
                    TenHangHoa: null,
                    ID_NhomHangHoa: _note_ID_NhomHangHoa_TangHang,
                    TenNhomHangHoa: _note_TenNhomHH_Select
                });
                console.log(self.HD_GiamGiaHang());
                for (var i = 0; i < self.HD_GiamGiaHang().length; i++) {
                    var $this = $('#dvtGiamGiaHang_' + self.HD_GiamGiaHang()[i].ID);
                    if (self.HD_GiamGiaHang()[i].GiamGiaTheoPhanTram) {
                        $this.addClass('active-re');
                    }
                    else {
                        $this.removeClass('active-re');
                    }
                }
            }
            else if (_hinhthucKM == 21) {
                self.HH_MuaHangGiamGia.replace(SaveItem, {
                    ID: SaveItem.ID,
                    SoLuongMua: SaveItem.SoLuongMua,
                    ID_DonViQuiDoiMua: SaveItem.ID_DonViQuiDoiMua,
                    TenHangHoaMua: SaveItem.TenHangHoaMua,
                    ID_NhomHangHoaMua: SaveItem.ID_NhomHangHoaMua,
                    TenNhomHangHoaMua: SaveItem.TenNhomHangHoaMua,
                    GiamGia: SaveItem.GiamGia,
                    GiamGiaTheoPhanTram: SaveItem.GiamGiaTheoPhanTram,
                    SoLuong: SaveItem.SoLuong,
                    ID_DonViQuiDoi: null,
                    TenHangHoa: null,
                    ID_NhomHangHoa: _note_ID_NhomHangHoa_TangHang,
                    TenNhomHangHoa: _note_TenNhomHH_Select
                })
                console.log(self.HH_MuaHangGiamGia());
                for (var i = 0; i < self.HH_MuaHangGiamGia().length; i++) {
                    var $this = $('#dvtMuaHangGiamGia_' + self.HH_MuaHangGiamGia()[i].ID);
                    if (self.HH_MuaHangGiamGia()[i].GiamGiaTheoPhanTram) {
                        $this.addClass('active-re');
                    }
                    else {
                        $this.removeClass('active-re');
                    }
                }
            }
            else if (_hinhthucKM == 22) {
                self.HH_MuaHangTangHang.replace(SaveItem, {
                    ID: SaveItem.ID,
                    SoLuongMua: SaveItem.SoLuongMua,
                    ID_DonViQuiDoiMua: SaveItem.ID_DonViQuiDoiMua,
                    TenHangHoaMua: SaveItem.TenHangHoaMua,
                    ID_NhomHangHoaMua: SaveItem.ID_NhomHangHoaMua,
                    TenNhomHangHoaMua: SaveItem.TenNhomHangHoaMua,
                    SoLuong: SaveItem.SoLuong,
                    ID_DonViQuiDoi: null,
                    TenHangHoa: null,
                    ID_NhomHangHoa: _note_ID_NhomHangHoa_TangHang,
                    TenNhomHangHoa: _note_TenNhomHH_Select
                })
                console.log(self.HH_MuaHangTangHang())
            }
        }
        else {
            _note_ID_NhomHangHoaMua_TangHang = _note_ID_NhomHangHoa_Select;
            if (_hinhthucKM == 21) {
                self.HH_MuaHangGiamGia.replace(SaveItem, {
                    ID: SaveItem.ID,
                    SoLuongMua: SaveItem.SoLuongMua,
                    ID_DonViQuiDoiMua: null,
                    TenHangHoaMua: null,
                    ID_NhomHangHoaMua: _note_ID_NhomHangHoaMua_TangHang,
                    TenNhomHangHoaMua: _note_TenNhomHH_Select,
                    GiamGia: SaveItem.GiamGia,
                    GiamGiaTheoPhanTram: SaveItem.GiamGiaTheoPhanTram,
                    SoLuong: SaveItem.SoLuong,
                    ID_DonViQuiDoi: SaveItem.ID_DonViQuiDoi,
                    TenHangHoa: SaveItem.TenHangHoa,
                    ID_NhomHangHoa: SaveItem.ID_NhomHangHoa,
                    TenNhomHangHoa: SaveItem.TenNhomHangHoa
                })
                console.log(self.HH_MuaHangGiamGia());
                for (var i = 0; i < self.HH_MuaHangGiamGia().length; i++) {
                    var $this = $('#dvtMuaHangGiamGia_' + self.HH_MuaHangGiamGia()[i].ID);
                    if (self.HH_MuaHangGiamGia()[i].GiamGiaTheoPhanTram) {
                        $this.addClass('active-re');
                    }
                    else {
                        $this.removeClass('active-re');
                    }
                }
            }
            else if (_hinhthucKM == 22) {
                self.HH_MuaHangTangHang.replace(SaveItem, {
                    ID: SaveItem.ID,
                    SoLuongMua: SaveItem.SoLuongMua,
                    ID_DonViQuiDoiMua: null,
                    TenHangHoaMua: null,
                    ID_NhomHangHoaMua: _note_ID_NhomHangHoaMua_TangHang,
                    TenNhomHangHoaMua: _note_TenNhomHH_Select,
                    SoLuong: SaveItem.SoLuong,
                    ID_DonViQuiDoi: SaveItem.ID_DonViQuiDoi,
                    TenHangHoa: SaveItem.TenHangHoa,
                    ID_NhomHangHoa: SaveItem.ID_NhomHangHoa,
                    TenNhomHangHoa: SaveItem.TenNhomHangHoa
                })
                console.log(self.HH_MuaHangTangHang())
            }
            else if (_hinhthucKM == 23) {
                self.HH_MuaHangTangDiem.replace(SaveItem, {
                    ID: SaveItem.ID,
                    SoLuongMua: SaveItem.SoLuongMua,
                    ID_DonViQuiDoiMua: null,
                    TenHangHoaMua: null,
                    ID_NhomHangHoaMua: _note_ID_NhomHangHoaMua_TangHang,
                    TenNhomHangHoaMua: _note_TenNhomHH_Select,
                    DiemCong: SaveItem.DiemCong,
                    GiamGiaTheoPhanTram: SaveItem.GiamGiaTheoPhanTram
                })
                console.log(self.HH_MuaHangTangDiem())
                for (var i = 0; i < self.HH_MuaHangTangDiem().length; i++) {
                    var $this = $('#dvtMuaHangTangDiem_' + self.HH_MuaHangTangDiem()[i].ID);
                    if (self.HH_MuaHangTangDiem()[i].GiamGiaTheoPhanTram) {
                        $this.addClass('active-re');
                    }
                    else {
                        $this.removeClass('active-re');
                    }
                }
            }
            else if (_hinhthucKM == 24) {
                self.HH_GiaBanTheoSL.replace(SaveItem, {
                    ID: SaveItem.ID,
                    SoLuongMua: 1,
                    ID_DonViQuiDoiMua: null,
                    TenHangHoaMua: null,
                    ID_NhomHangHoaMua: _note_ID_NhomHangHoaMua_TangHang,
                    TenNhomHangHoaMua: _note_TenNhomHH_Select,
                    GiaKhuyenMai: null
                })
                console.log(self.HH_GiaBanTheoSL());
            }
        }
        $('#listgroupkm').modal("hide")
    }
    //$('.list-choose li').on('click', function (item) {
    //    console.log('a')
    //    console.log(item.ID)
    //});
    //Lấy về danh sách nhóm hàng hóa
    //function getNhomHangHoa() {
    //    ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "GetDM_NhomHangHoa", 'GET').done(function (data) {
    //        self.NhomHangHoas(data);
    //        //console.log(self.NhomHangHoas());
    //    })
    //}
    //getNhomHangHoa();

    function GetAllNhomHH() {
        self.NhomHangHoas([]);
        ajaxHelper(ReportUri + "GetListID_NhomHangHoa?TenNhomHang=" + tk, 'GET').done(function (data) {
            for (var i = 0; i < data.length; i++) {
                if (data[i].ID_Parent == null) {
                    var objParent = {
                        ID: data[i].ID,
                        TenNhomHangHoa: data[i].TenNhomHang,
                        Childs: [],
                    }
                    for (var j = 0; j < data.length; j++) {
                        if (data[j].ID !== data[i].ID && data[j].ID_Parent === data[i].ID) {
                            var objChild =
                                {
                                    ID: data[j].ID,
                                    TenNhomHangHoa: data[j].TenNhomHang,
                                    ID_Parent: data[i].ID,
                                    Child2s: []
                                };
                            for (var k = 0; k < data.length; k++) {
                                if (data[k].ID_Parent !== null && data[k].ID_Parent === data[j].ID) {
                                    var objChild2 =
                                        {
                                            ID: data[k].ID,
                                            TenNhomHangHoa: data[k].TenNhomHang,
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
            if (self.NhomHangHoas().length > 10) {
                $('.close-goods').css('display', 'block');
            }
        });
    };

    self.NoteNhomHang = function () {
        tk = $('#SeachNhomHang').val();
        GetAllNhomHH();
    };

    function getNhomDoiTuong() {
        ajaxHelper("api/DanhMuc/DM_NhomDoiTuongAPI/" + "GetDM_NhomDoiTuong?loaiDoiTuong=" + 1, 'GET').done(function (data) {
            if (data != null) {
                self.NhomDoiTuongs(data);
            }
        })
    }

    //self.NoteNhomHang = function () {
    //    var tk = $('#SeachNhomHang').val();
    //    console.log(tk);
    //    if (tk.trim() != '') {
    //        ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "SeachDM_NhomHangHoa?TenNhom=" + tk, 'GET').done(function (data) {
    //            self.NhomHangHoas(data);
    //        })
    //    }
    //    else {
    //        ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "GetDM_NhomHangHoa", 'GET').done(function (data) {
    //            self.NhomHangHoas(data);
    //        })
    //    }
    //}

    self.SaveKhuyenMai = function () {
        document.getElementById("save_KhuyenMai").disabled = true;
        document.getElementById("save_KhuyenMai").lastChild.data = " Đang lưu";
        var chek_maKM = null;
        ajaxHelper(BH_KhuyenMaiUri + "get_CheckKhuyenMai?MaKM=" + _maKM, "GET").done(function (data) {
            chek_maKM = data;
            if (chek_maKM != null & _dieukienSave == 1) {
                ShowMessage_Danger(chek_maKM);
            }
            else {
                if (_tenKM == null) {
                    ShowMessage_Danger("Vui lòng nhập tên chương trình khuyến mại");
                }
                else {
                    console.log(_timeStart, _timeEnd);

                    if (new Date(_timeStart) > new Date(_timeEnd)) {
                        ShowMessage_Danger("Ngày bắt đầu khuyến mại không được lớn hơn ngày kết thúc");
                    }
                    else {
                        self.selectGiaTriKM();
                        self.CT_ApDungKM([]);
                        self.CT_HangHoaKM([]);
                    }
                }
            }
            document.getElementById("save_KhuyenMai").disabled = false;
            document.getElementById("save_KhuyenMai").lastChild.data = " Lưu";
        });
    }
    self.LuuChuongTrinhKhuyenMai = function () {
        if (!isValid(_maKM)) {
            ShowMessage_Danger("Mã phiếu không được nhập kí tự đặc biệt");
            return false;
        }
        var KhuyenMai = {
            MaKhuyenMai: _maKM,
            TenKhuyenMai: _tenKM,
            GhiChu: _ghichuKM,
            TrangThai: _trangthaiKM,
            HinhThuc: _hinhthucKM,
            LoaiKhuyenMai: _loaiKM,
            NgayApDung: _ngayapdungKM,
            ThangApDung: _thangapdungKM,
            ThuApDung: _thuapdungKM,
            GioApDung: _gioapdungKM,
            ApDungNgaySinhNhat: _apdungngaysinhnhat,
            TatCaDonVi: _tatcadonvi,
            TatCaDoiTuong: _tatcadoituong,
            TatCaNhanVien: _tatcanhanvien,
            NguoiTao: _nguoisua,
        };
        var myData = {};
        myData.objKhuyenMai = KhuyenMai;
        myData.objKhuyenMaiApDung = self.CT_ApDungKM();
        myData.objKhuyenMaiChiTiet = self.CT_HangHoaKM();
        console.log(myData);
        callAjaxAdd(myData); //insert khuyến mại
    }

    self.EditChuongTrinhKhuyenMai = function () {
        if (!isValid(_maKM)) {
            ShowMessage_Danger("Mã phiếu không được nhập kí tự đặc biệt");
            return false;
        }
        var KhuyenMai = {
            ID: _ID_KhuyenMai_Edit,
            MaKhuyenMai: _maKM,
            TenKhuyenMai: _tenKM,
            GhiChu: _ghichuKM,
            TrangThai: _trangthaiKM,
            HinhThuc: _hinhthucKM,
            LoaiKhuyenMai: _loaiKM,
            NgayApDung: _ngayapdungKM,
            ThangApDung: _thangapdungKM,
            ThuApDung: _thuapdungKM,
            GioApDung: _gioapdungKM,
            ApDungNgaySinhNhat: _apdungngaysinhnhat,
            TatCaDonVi: _tatcadonvi,
            TatCaDoiTuong: _tatcadoituong,
            TatCaNhanVien: _tatcanhanvien,
            NguoiSua: _nguoisua,
        };
        var myData = {};
        myData.objKhuyenMai = KhuyenMai;
        myData.objKhuyenMaiApDung = self.CT_ApDungKM();
        myData.objKhuyenMaiChiTiet = self.CT_HangHoaKM();
        console.log(self.CT_ApDungKM())
        console.log(self.CT_HangHoaKM())
        callAjaxEdit(myData); //edit khuyến mại
    }
    function callAjaxEdit(myData) {
        $.ajax({
            data: myData,
            url: BH_KhuyenMaiUri + "PutBH_KhuyenMai?dateStart=" + _timeStart + "&dateEnd=" + _timeEnd + "&ID_DonVi=" + _id_DonViLS + "&ID_NhanVien=" + _id_NhanVienLS,
            type: 'PUT',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",

            success: function (item) {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật chương trình khuyến mại thành công", "success");
                self.CT_ApDungKM([]);
                self.CT_HangHoaKM([]); $('#khuyenmaip').modal('hide');
                sleep(500).then(() => { clickloadForm('Promotion'); });
                //window.location.href = '/#/Promotion';
            },
            error: function (jqXHR, textStatus, errorThrown) {
                ShowMessage_Danger("Cập nhật chương trình khuyến mại không thành công");
            },
            complete: function (item) {
            }
        })
    }
    function callAjaxAdd(myData) {
        $.ajax({
            data: myData,
            url: BH_KhuyenMaiUri + "PostBH_KhuyenMai?dateStart=" + _timeStart + "&dateEnd=" + _timeEnd + "&ID_DonVi=" + _id_DonViLS + "&ID_NhanVien=" + _id_NhanVienLS,
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (item) {
                ShowMessage_Success("Thêm mới chương trình khuyến mại thành công");
                self.CT_ApDungKM([]);
                self.CT_HangHoaKM([]);
                $('#khuyenmaip').modal('hide');
                sleep(500).then(() => { clickloadForm('Promotion'); });
                //window.location.href = '/#/Promotion';
            },
            error: function (jqXHR, textStatus, errorThrown) {
                ShowMessage_Danger("Thêm mới chương trình khuyến mại không thành công");
            },
            complete: function (item) {
                localStorage.removeItem('Lc_KhuyenMaiApDung');
                localStorage.removeItem('Lc_KhuyenMaiChiTiet');
            }
        })
    }
    $('#datetimepicker_mask1').change(function (e) {
        var thisDate = $(this).val().trim();
        var a = thisDate.split(" ");
        _timeEnd = a[0].split("/").reverse().join("-") + " " + a[1];
        console.log(_timeEnd);
    });
    $('#datetimepicker_mask').change(function (e) {
        var thisDate = $(this).val().trim();
        var a = thisDate.split(" ");
        _timeStart = a[0].split("/").reverse().join("-") + " " + a[1];
        console.log(_timeStart);
    });
    $('#txtTimeStart').on('dp.change', function (e) {
        var thisDate = $(this).val().trim();
        var a = thisDate.split(" ");
        _timeStart = a[0].split("/").reverse().join("-") + " " + a[1] + " " + a[2];
        //_timeStart = moment(_timeStart).format('YYYY-MM-DD hh:mm A')
        console.log(_timeStart);

    });
    $('#txtTimeEnd').on('dp.change', function (e) {
        var thisDate = $(this).val().trim();
        var a = thisDate.split(" ");
        _timeEnd = a[0].split("/").reverse().join("-") + " " + a[1] + " " + a[2];
        //_timeStart = moment(_timeStart).format('YYYY-MM-DD hh:mm A')
        console.log(_timeEnd);
    });

    self.NoteChiNhanh = function () {
        var ob = {
            ID_DonVi: _id_ChiNhanh,
            TenDonVi: _nameDionVi
        }
        self.MangChiNhanh.push(ob)
    }
    self.NoteNhanVien = function () {
        var ob = {
            ID_NhaVien: _id_NhanVien,
            TenNhanVien: _nameNhanVien,
        }
        self.MangNhanVien.push(ob)
    }
    self.NoteNhomKhachHang = function () {
        var ob = {
            ID_NhomKhachHang: _id_NhomKhachHang,
            TenNhomKhachHang: _nameNhomKhachHang,
        }
        self.MangNhomKhachHang.push(ob)
    }
    self.ApDung_KM = function () {
        // var Lc_KhuyenMaiApDung = [];
        var ob2 = {
            ID_DonVi: _id_ChiNhanh,
            ID_NhanVien: _id_NhanVien,
            ID_NhomKhachHang: _id_NhomKhachHang,
            TenDonVi: _nameDionVi,
            TenNhanVien: _nameNhanVien,
            TenNhomDoiTuong: _nameNhomKhachHang
        }
        self.CT_ApDungKM.push(ob2);
        //Lc_KhuyenMaiApDung.unshift(ob2);
        localStorage.setItem('Lc_KhuyenMaiApDung', JSON.stringify(self.CT_ApDungKM()));
        console.log(self.CT_ApDungKM());
    }
    self.ApDung_KM_NguoiBan = function () {
        if (_boolChiNhanh == true) {
            self.ApDung_KM_ChiNhanh();
        }
        else {
            var k = self.CT_ApDungKM().length;
            for (var i = 0; i < k; i++) {
                _id_ChiNhanh = self.CT_ApDungKM()[i].ID_DonVi;
                _nameDionVi = self.CT_ApDungKM()[i].TenDonVi;
                if (dkremove == 1) {
                    self.CT_ApDungKM.splice(i, 1);
                    k = k - 1;
                    i = i - 1;
                }
                self.ApDung_KM_ChiNhanh();
            }
        }
    }
    self.ApDungKhuyenMai = function () {
        if (self.MangChiNhanh().length > 0) {
            for (var i = 0; i < self.MangChiNhanh().length; i++) {
                if (self.MangNhanVien().length > 0) {
                    for (var j = 0; j < self.MangNhanVien().length; j++) {
                        if (self.MangNhomKhachHang().length > 0) {
                            for (var l = 0; l < self.MangNhomKhachHang().length; l++) {
                                _id_NhomKhachHang = self.MangNhomKhachHang()[l].ID;
                                _nameNhomKhachHang = self.MangNhomKhachHang()[l].TenNhomDoiTuong;
                                _id_NhanVien = self.MangNhanVien()[j].ID;
                                _nameNhanVien = self.MangNhanVien()[j].TenNhanVien;
                                _id_ChiNhanh = self.MangChiNhanh()[i].ID;
                                _nameDionVi = self.MangChiNhanh()[i].TenDonVi;
                                self.ApDung_KM();
                            }
                        }
                        else {

                            _id_NhomKhachHang = null;
                            _nameNhomKhachHang = null;
                            _id_NhanVien = self.MangNhanVien()[j].ID;
                            _nameNhanVien = self.MangNhanVien()[j].TenNhanVien;
                            _id_ChiNhanh = self.MangChiNhanh()[i].ID;
                            _nameDionVi = self.MangChiNhanh()[i].TenDonVi;
                            self.ApDung_KM();
                        }
                    }
                }
                else {
                    if (self.MangNhomKhachHang().length > 0) {
                        for (var l = 0; l < self.MangNhomKhachHang().length; l++) {
                            _id_NhomKhachHang = self.MangNhomKhachHang()[l].ID;
                            _nameNhomKhachHang = self.MangNhomKhachHang()[l].TenNhomDoiTuong;
                            _id_NhanVien = null;
                            _nameNhanVien = null;
                            _id_ChiNhanh = self.MangChiNhanh()[i].ID;
                            _nameDionVi = self.MangChiNhanh()[i].TenDonVi;
                            self.ApDung_KM();
                        }
                    }
                    else {

                        _id_NhomKhachHang = null;
                        _nameNhomKhachHang = null;
                        _id_NhanVien = null;
                        _nameNhanVien = null;
                        _id_ChiNhanh = self.MangChiNhanh()[i].ID;
                        _nameDionVi = self.MangChiNhanh()[i].TenDonVi;
                        self.ApDung_KM();
                    }
                }
            }
        }
        else {
            if (self.MangNhanVien().length > 0) {
                for (var j = 0; j < self.MangNhanVien().length; j++) {
                    if (self.MangNhomKhachHang().length > 0) {
                        for (var l = 0; l < self.MangNhomKhachHang().length; l++) {
                            _id_NhomKhachHang = self.MangNhomKhachHang()[l].ID;
                            _nameNhomKhachHang = self.MangNhomKhachHang()[l].TenNhomDoiTuong;
                            _id_NhanVien = self.MangNhanVien()[j].ID;
                            _nameNhanVien = self.MangNhanVien()[j].TenNhanVien;
                            _id_ChiNhanh = null;
                            _nameDionVi = null;
                            self.ApDung_KM();
                        }
                    }
                    else {

                        _id_NhomKhachHang = null;
                        _nameNhomKhachHang = null;
                        _id_NhanVien = self.MangNhanVien()[j].ID;
                        _nameNhanVien = self.MangNhanVien()[j].TenNhanVien;
                        _id_ChiNhanh = null;
                        _nameDionVi = null;
                        self.ApDung_KM();
                    }
                }
            }
            else {
                if (self.MangNhomKhachHang().length > 0) {
                    for (var l = 0; l < self.MangNhomKhachHang().length; l++) {
                        _id_NhomKhachHang = self.MangNhomKhachHang()[l].ID;
                        _nameNhomKhachHang = self.MangNhomKhachHang()[l].TenNhomDoiTuong;
                        _id_NhanVien = null;
                        _nameNhanVien = null;
                        _id_ChiNhanh = null;
                        _nameDionVi = null;
                        self.ApDung_KM();
                    }
                }
                else {
                    _id_NhomKhachHang = null;
                    _nameNhomKhachHang = null;
                    _id_NhanVien = null;
                    _nameNhanVien = null;
                    _id_ChiNhanh = null;
                    _nameDionVi = null;
                    self.ApDung_KM();
                }
            }
        }
    }
    self.getTimeKM = function () {
        if (self.mangThangKM() != null) {
            for (var i = 0; i < self.mangThangKM().length; i++) {
                if (_thangapdungKM == null) {
                    _thangapdungKM = self.mangThangKM()[i];
                }
                else {
                    _thangapdungKM = _thangapdungKM + "_" + self.mangThangKM()[i];
                }
            }
            //console.log(_thangapdungKM)
        }
        if (self.mangNgayKM() != null) {
            for (var i = 0; i < self.mangNgayKM().length; i++) {
                if (_ngayapdungKM == null) {
                    _ngayapdungKM = self.mangNgayKM()[i];
                }
                else {
                    _ngayapdungKM = _ngayapdungKM + "_" + self.mangNgayKM()[i];
                }
            }
            //console.log(_ngayapdungKM)
        }
        if (self.mangThuKM() != null) {
            for (var i = 0; i < self.mangThuKM().length; i++) {
                if (_thuapdungKM == null) {
                    _thuapdungKM = self.mangThuKM()[i];
                }
                else {
                    _thuapdungKM = _thuapdungKM + "_" + self.mangThuKM()[i];
                }
            }
            // console.log(_thuapdungKM)
        }
        if (self.mangGioKM() != null) {
            for (var i = 0; i < self.mangGioKM().length; i++) {
                if (_gioapdungKM == null) {
                    _gioapdungKM = self.mangGioKM()[i];
                }
                else {
                    _gioapdungKM = _gioapdungKM + "_" + self.mangGioKM()[i];
                }
            }
            //console.log(_gioapdungKM)
        }
    }
    var _ID_KMCT = null;
    var _ID_KMAP = null;
    var _tongtienhang = null;
    var _giamgia = null;
    var _giamgiatheophantram = true;
    var _ID_DonViQuiDoi = null;
    var _ID_NhomHangHoa = null;
    var _soluong = null;
    var _ID_DonViQuiDoiMua = null;
    var _ID_NhomHangHoaMua = null;
    var _soluongmua = null;
    var _giakhuyenmai = null;
    self.selectGiaTriKM = function () {
        self.resetCT_HangHoaKM();
        var thuchienLuu = 1;
        if (_hinhthucKM == 11) {
            for (var i = 0; i < self.HD_GiamGiaHD().length; i++) {
                _tongtienhang = self.HD_GiamGiaHD()[i].TongTienHang;
                _giamgia = self.HD_GiamGiaHD()[i].GiamGia;
                _giamgiatheophantram = self.HD_GiamGiaHD()[i].GiamGiaTheoPhanTram;
                if (_tongtienhang <= 0) {
                    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập tổng tiền hàng.");
                    thuchienLuu = 2;
                    break;
                }
                if (_giamgia <= 0) {
                    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập giá trị giảm giá.");
                    thuchienLuu = 2;
                    break;
                }
                if (_giamgia > 100 && _giamgiatheophantram == true) {
                    ShowMessage_Danger("Hình thức khuyến mại: Giá trị giảm giá theo phần trăm không được lớn hơn 100%.");
                    thuchienLuu = 2;
                    break;
                }
                self.getChiTietKhuyenMai();
            }
        }
        else if (_hinhthucKM == 12) {
            for (var i = 0; i < self.HD_TangHang().length; i++) {
                _tongtienhang = self.HD_TangHang()[i].TongTienHang;
                _soluong = self.HD_TangHang()[i].SoLuong;
                _ID_DonViQuiDoi = self.HD_TangHang()[i].ID_DonViQuiDoi;
                _ID_NhomHangHoa = self.HD_TangHang()[i].ID_NhomHangHoa;
                if (_tongtienhang <= 0) {
                    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập tổng tiền hàng.");
                    thuchienLuu = 2;
                    break;
                }
                if (_soluong <= 0) {
                    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập số lượng cho Hàng/Nhóm hàng được chọn.");
                    thuchienLuu = 2;
                    break;
                }
                if (_ID_DonViQuiDoi == null & _ID_NhomHangHoa == null) {
                    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập Hàng/Nhóm hàng được tặng.");
                    thuchienLuu = 2;
                    break;
                }
                self.getChiTietKhuyenMai();
            }
        }
        else if (_hinhthucKM == 13) {
            for (var i = 0; i < self.HD_GiamGiaHang().length; i++) {
                _tongtienhang = self.HD_GiamGiaHang()[i].TongTienHang;
                _giamgia = self.HD_GiamGiaHang()[i].GiamGia;
                _giamgiatheophantram = self.HD_GiamGiaHang()[i].GiamGiaTheoPhanTram;
                _soluong = self.HD_GiamGiaHang()[i].SoLuong;
                _ID_DonViQuiDoi = self.HD_GiamGiaHang()[i].ID_DonViQuiDoi;
                _ID_NhomHangHoa = self.HD_GiamGiaHang()[i].ID_NhomHangHoa;
                if (_tongtienhang <= 0) {
                    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập tổng tiền hàng.");
                    thuchienLuu = 2;
                    break;
                }
                if (_giamgia <= 0) {
                    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập giá trị giảm giá.");
                    thuchienLuu = 2;
                    break;
                }
                if (_giamgia > 100 && _giamgiatheophantram == true) {
                    ShowMessage_Danger("Hình thức khuyến mại: Giá trị giảm giá theo phần trăm không được lớn hơn 100%.");
                    thuchienLuu = 2;
                    break;
                }
                if (_soluong <= 0) {
                    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập số lượng cho Hàng/Nhóm hàng được giảm giá.");
                    thuchienLuu = 2;
                    break;
                }
                if (_ID_DonViQuiDoi == null & _ID_NhomHangHoa == null) {
                    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập Hàng/Nhóm hàng được giảm giá.");
                    thuchienLuu = 2;
                    break;
                }
                self.getChiTietKhuyenMai();
            }
        }
        else if (_hinhthucKM == 14) {
            for (var i = 0; i < self.HD_TangDiem().length; i++) {
                _tongtienhang = self.HD_TangDiem()[i].TongTienHang;
                _giamgia = self.HD_TangDiem()[i].DiemCong;
                _giamgiatheophantram = self.HD_TangDiem()[i].GiamGiaTheoPhanTram;
                if (_tongtienhang <= 0) {
                    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập tổng tiền hàng.");
                    thuchienLuu = 2;
                    break;
                }
                if (_giamgia <= 0) {
                    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập số Điểm cộng.");
                    thuchienLuu = 2;
                    break;
                }
                self.getChiTietKhuyenMai();
            }
        }
        else if (_hinhthucKM == 21) {
            for (var i = 0; i < self.HH_MuaHangGiamGia().length; i++) {
                _soluongmua = self.HH_MuaHangGiamGia()[i].SoLuongMua;
                _ID_DonViQuiDoiMua = self.HH_MuaHangGiamGia()[i].ID_DonViQuiDoiMua;
                _ID_NhomHangHoaMua = self.HH_MuaHangGiamGia()[i].ID_NhomHangHoaMua;
                _giamgia = self.HH_MuaHangGiamGia()[i].GiamGia;
                _giamgiatheophantram = self.HH_MuaHangGiamGia()[i].GiamGiaTheoPhanTram;
                _soluong = self.HH_MuaHangGiamGia()[i].SoLuong;
                _ID_DonViQuiDoi = self.HH_MuaHangGiamGia()[i].ID_DonViQuiDoi;
                _ID_NhomHangHoa = self.HH_MuaHangGiamGia()[i].ID_NhomHangHoa;
                if (_soluongmua <= 0) {
                    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập Số lượng cho Hàng/Nhóm hàng mua.");
                    thuchienLuu = 2;
                    break;
                }
                if (_ID_DonViQuiDoiMua == null & _ID_NhomHangHoaMua == null) {
                    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập Hàng/Nhóm hàng mua.");
                    thuchienLuu = 2;
                    break;
                }
                if (_giamgia <= 0) {
                    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập giá trị Giảm giá.");
                    thuchienLuu = 2;
                    break;
                }
                if (_giamgia > 100 && _giamgiatheophantram == true) {
                    ShowMessage_Danger("Hình thức khuyến mại: Giá trị giảm giá theo phần trăm không được lớn hơn 100%.");
                    thuchienLuu = 2;
                    break;
                }
                if (_soluong <= 0) {
                    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập Số lượng cho Hàng/Nhóm hàng được giảm giá.");
                    thuchienLuu = 2;
                    break;
                }
                if (_ID_DonViQuiDoi == null & _ID_NhomHangHoa == null) {
                    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập Hàng/Nhóm hàng được giảm giá.");
                    thuchienLuu = 2;
                    break;
                }
                self.getChiTietKhuyenMai();
            }
        }
        else if (_hinhthucKM == 22) {
            for (var i = 0; i < self.HH_MuaHangTangHang().length; i++) {
                _soluongmua = self.HH_MuaHangTangHang()[i].SoLuongMua;
                _ID_DonViQuiDoiMua = self.HH_MuaHangTangHang()[i].ID_DonViQuiDoiMua;
                _ID_NhomHangHoaMua = self.HH_MuaHangTangHang()[i].ID_NhomHangHoaMua;
                _soluong = self.HH_MuaHangTangHang()[i].SoLuong;
                _ID_DonViQuiDoi = self.HH_MuaHangTangHang()[i].ID_DonViQuiDoi;
                _ID_NhomHangHoa = self.HH_MuaHangTangHang()[i].ID_NhomHangHoa;
                if (_soluongmua <= 0) {
                    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập Số lượng cho Hàng/Nhóm hàng mua.");
                    thuchienLuu = 2;
                    break;
                }
                if (_ID_DonViQuiDoiMua == null & _ID_NhomHangHoaMua == null) {
                    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập Hàng/Nhóm hàng mua.");
                    thuchienLuu = 2;
                    break;
                }
                if (_soluong <= 0) {
                    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập Số lượng cho Hàng/Nhóm hàng được tặng.");
                    thuchienLuu = 2;
                    break;
                }
                if (_ID_DonViQuiDoi == null & _ID_NhomHangHoa == null) {
                    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập Hàng/Nhóm hàng được tặng.");
                    thuchienLuu = 2;
                    break;
                }
                self.getChiTietKhuyenMai();
            }
        }
        else if (_hinhthucKM == 23) {
            for (var i = 0; i < self.HH_MuaHangTangDiem().length; i++) {
                _soluongmua = self.HH_MuaHangTangDiem()[i].SoLuongMua;
                _ID_DonViQuiDoiMua = self.HH_MuaHangTangDiem()[i].ID_DonViQuiDoiMua;
                _ID_NhomHangHoaMua = self.HH_MuaHangTangDiem()[i].ID_NhomHangHoaMua;
                _giamgia = self.HH_MuaHangTangDiem()[i].DiemCong;
                _giamgiatheophantram = self.HH_MuaHangTangDiem()[i].GiamGiaTheoPhanTram;
                if (_soluongmua <= 0) {
                    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập Số lượng cho Hàng/Nhóm hàng mua.");
                    thuchienLuu = 2;
                    break;
                }
                if (_ID_DonViQuiDoiMua == null & _ID_NhomHangHoaMua == null) {
                    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập Hàng/Nhóm hàng mua.");
                    thuchienLuu = 2;
                    break;
                }
                if (_giamgia <= 0) {
                    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập số Điểm cộng.");
                    thuchienLuu = 2;
                    break;
                }
                self.getChiTietKhuyenMai();
            }
        }
        else if (_hinhthucKM == 24) {
            //console.log(self.HH_GiaBanTheoSL());
            for (var i = 0; i < self.ThemHangHoa().length; i++) {
                _soluongmua = self.ThemHangHoa()[i].SoLuongMua;
                _giakhuyenmai = self.ThemHangHoa()[i].GiaKhuyenMai;
                _ID_DonViQuiDoiMua = self.HH_GiaBanTheoSL()[parseInt(self.ThemHangHoa()[i].ID_addHH) - 1].ID_DonViQuiDoiMua;
                _ID_NhomHangHoaMua = self.HH_GiaBanTheoSL()[parseInt(self.ThemHangHoa()[i].ID_addHH) - 1].ID_NhomHangHoaMua;
                _giamgia = self.ThemHangHoa()[i].GiamGia;
                _giamgiatheophantram = self.ThemHangHoa()[i].GiamGiaTheoPhanTram;
                if (_ID_DonViQuiDoiMua == null & _ID_NhomHangHoaMua == null) {
                    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập Hàng/Nhóm hàng mua.");
                    thuchienLuu = 2;
                    break;
                }
                if (_soluongmua <= 0) {
                    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập Số lượng từ.");
                    thuchienLuu = 2;
                    break;
                }
                //if (_giakhuyenmai <= 0) {
                //    ShowMessage_Danger("Hình thức khuyến mại: Vui lòng nhập Giá bán khuyến mại.");
                //    thuchienLuu = 2;
                //    break;
                //}
                self.getChiTietKhuyenMai();
            }
        }
        if (thuchienLuu == 1) {
            self.ApDungKhuyenMai();
            self.getTimeKM();
            console.log(_dieukienSave);
            if (_dieukienSave == 1 || _dieukienSave == 3) {
                self.LuuChuongTrinhKhuyenMai();
            }
            else {
                self.EditChuongTrinhKhuyenMai();
            }
        }
    }
    self.resetCT_HangHoaKM = function () {
        _tongtienhang = null;
        _giamgia = null;
        _giamgiatheophantram = true;
        _ID_DonViQuiDoi = null;
        _ID_NhomHangHoa = null;
        _soluong = null;
        _ID_DonViQuiDoiMua = null;
        _ID_NhomHangHoaMua = null;
        _soluongmua = null;
        _giakhuyenmai = null;
    }
    self.getChiTietKhuyenMai = function () {
        var ob = {
            TongTienHang: _tongtienhang,
            GiamGia: _giamgia,
            GiamGiaTheoPhanTram: _giamgiatheophantram,
            ID_DonViQuiDoi: _ID_DonViQuiDoi,
            ID_NhomHangHoa: _ID_NhomHangHoa,
            SoLuong: _soluong,
            ID_DonViQuiDoiMua: _ID_DonViQuiDoiMua,
            ID_NhomHangHoaMua: _ID_NhomHangHoaMua,
            SoLuongMua: _soluongmua,
            GiaKhuyenMai: _giakhuyenmai
        }
        self.CT_HangHoaKM.push(ob);
        //Lc_KhuyenMaiApDung.unshift(ob2);
        localStorage.setItem('Lc_KhuyenMaiChiTiet', JSON.stringify(self.CT_HangHoaKM()));
        console.log(self.CT_HangHoaKM());
    }
    self.loadDieuKienKM = function (item, e) {
        _id_LSKhuyenMai = item.ID;
        //console.log(1, item.ID);
        _hinhthucKM = item.KieuHinhThuc;
        switch (_hinhthucKM) {
            case 11:
                dk = "Giảm giá hóa đơn";
                break;
            case 12:
                dk = "Tặng hàng";
                break;
            case 13:
                dk = 'Giảm giá hàng';
                break;
            case 14:
                dk = 'Tặng điểm';
                break;
            case 21:
                dk = 'Mua hàng giảm giá hàng';
                break;
            case 22:
                dk = 'Mua hàng tặng hàng';
                break;
            case 23:
                dk = 'Mua hàng tặng điểm';
                break;
            case 24:
                dk = 'Giảm giá bán theo số lượng mua';
                break;
        }

        ajaxHelper(BH_KhuyenMaiUri + "getChiTiet_KhuyenMai?ID_KhuyenMai=" + item.ID, "GET").done(function (data) {
            self.BH_KhuyenMai_ChiTiet(data);
            SetHeightShowDetail($(e.currentTarget));
            console.log(self.BH_KhuyenMai_ChiTiet());
        });
        //ajaxHelper(BH_KhuyenMaiUri + "getList_LichSuKhuyenMai?ID_KhuyenMai=" + item.ID + "&numberPage=" + _numberPage_LS + "&PageSize=" + _numberRowns_LS, "GET").done(function (data) {
        //    self.BH_LichSuKhuyenMai(data.LstData);
        //    console.log(data);
        //    if (self.BH_LichSuKhuyenMai().length > 0) {
        //        self.RowsStart_LS((_numberPage_LS - 1) * _numberRowns_LS + 1);
        //        self.RowsEnd_LS((_numberPage_LS - 1) * _numberRowns_LS + self.BH_LichSuKhuyenMai().length)
        //    }
        //    else {
        //        self.RowsStart_LS('0');
        //        self.RowsEnd_LS('0');
        //    }
        //    self.Rows_LichSuKhuyenMai(data.Rowcount);
        //    self.Pages_LichSuKhuyenMai(data.LstPageNumber);
        //    AllPage_LS = self.Pages_LichSuKhuyenMai().length;
        //    self.ReserPage_LS();
        //});
        self.selectDieuKienKM(item.HinhThuc);
    }
    self.selectDieuKienKM = function (item) {
        if (item == 'Hóa đơn - Giảm giá hóa đơn') {
            $(".tableDK").hide();
            $(".HD_11").show();
        }
        if (item == "Hóa đơn - Tặng hàng") {
            $(".tableDK").hide();
            $(".HD_12").show();
        }
        if (item == "Hóa đơn - Giảm giá hàng") {
            $(".tableDK").hide();
            $(".HD_13").show();
        }
        if (item == "Hóa đơn - Tặng Điểm") {
            $(".tableDK").hide();
            $(".HD_14").show();
        }
        if (item == "Hàng hóa - Mua hàng giảm giá hàng") {
            $(".tableDK").hide();
            $(".HH_21").show();
        }
        if (item == "Hàng hóa - Mua hàng tặng hàng") {
            $(".tableDK").hide();
            $(".HH_22").show();
        }
        if (item == "Hàng hóa - Mua hàng tặng điểm") {
            $(".tableDK").hide();
            $(".HH_23").show();
        }
        if (item == "Hàng hóa - Mua hàng giảm giá theo số lượng mua") {
            $(".tableDK").hide();
            $(".HH_24").show();
        }
    }
    //Sao chép
    self.CopyKhuyenMai = function (item) {
        _dieukienSave = 3;
        self.TieuDe('Sao chép chương trình khuyến mại');
        self.UpdateKhuyenMaiChiTiet(item);
    }
    self.UpdateKhuyenMai = function (item) {
        _dieukienSave = 2;
        self.TieuDe('Cập nhật chương trình khuyến mại');
        self.UpdateKhuyenMaiChiTiet(item);
    }

    function setValueSelect_KM24() {
        $('.munber-bought > div').each(function () {
            var id = $(this).attr('id');
            var ddl = $(this).find('select');
            var itemHH = $.grep(self.ThemHangHoa(), function (x) {
                return x.IDRandom === id;
            });
            if (itemHH.length > 0) {
                if (itemHH[0].GiamGia == null) {
                    $(ddl).val('1');
                }
                else {
                    $(ddl).val('2');
                }
            }
        })
    }

    // cập nhật
    self.CN_maKM = ko.observable();
    self.CN_TenKM = ko.observable();
    self.CN_TrangThaiKM = ko.observable('1');
    self.CN_Ghichu = ko.observable();
    self.CN_CheckApDungSN = ko.observable();
    self.KieuApDungSN = ko.observable('Ngày sinh nhật')
    self.CN_checkChiNhanh = ko.observable('1');
    self.CN_checkNguoiBan = ko.observable('1');
    self.CN_checkNhomKhachHang = ko.observable('1');
    self.UpdateKhuyenMaiChiTiet = function (item) {
        //self.TieuDe('Cập nhật chương trình khuyến mại');
        $('.trade-tabb li').each(function () {
            $(this).removeClass('active');
        });
        $('#htKM').addClass('active');
        $('#thoigian').removeClass('active');
        $('#phamviapdung').removeClass('active');
        $('#hinhthuc').addClass('active');
        _ID_KhuyenMai_Edit = item.ID;
        if (_dieukienSave == 2) {
            self.CN_maKM(item.MaKhuyenMai);
            _maKM = item.MaKhuyenMai;
        }
        else {
            self.CN_maKM([]);
            _maKM = null;
        }
        self.CN_TenKM(item.TenKhuyenMai);
        _tenKM = item.TenKhuyenMai;
        self.CN_Ghichu(item.GhiChu);
        _ghichuKM = item.GhiChu;
        self.CN_CheckApDungSN(item.ValueApDungSN);
        self.ThoiGianBatDauKM(moment(item.ThoiGianBatDau).format('DD/MM/YYYY HH:mm'));
        self.ThoiGianKetThucKM(moment(item.ThoiGianKetThuc).format('DD/MM/YYYY HH:mm'));

        var thisDate = moment(item.ThoiGianBatDau).format('DD/MM/YYYY HH:mm').trim();
        var a = thisDate.split(" ");
        _timeStart = a[0].split("/").reverse().join("-") + " " + a[1];

        var thisDate1 = moment(item.ThoiGianKetThuc).format('DD/MM/YYYY HH:mm').trim();
        var b = thisDate1.split(" ");
        _timeEnd = b[0].split("/").reverse().join("-") + " " + b[1];

        //_timeStart = moment(item.ThoiGianBatDau).format('YYYY-MM HH:mm');
        //_timeEnd = moment(item.ThoiGianKetThuc).format('DD/MM/YYYY HH:mm');
        console.log(_timeStart, _timeEnd);
        if (item.TrangThai == 'Chưa áp dụng') {
            self.CN_TrangThaiKM('2');
            _trangthaiKM = false;
        }
        else {
            self.CN_TrangThaiKM('1');
            _trangthaiKM = true;
        }
        _loaiKM = item.LoaiKhuyenMai;
        //_hinhthucKM = item.KieuHinhThuc;
        if (item.LoaiKhuyenMai == 1) {
            var mang = ['Giảm giá hóa đơn', 'Tặng hàng', 'Giảm giá hàng', 'Tặng điểm']
            self.HinhthucKM(mang);
            self.txtKhuyenMai("Hóa đơn")
        }
        else {
            self.txtKhuyenMai('Hàng hóa')
            var mang = ['Mua hàng giảm giá hàng', 'Mua hàng tặng hàng', 'Mua hàng tặng điểm', 'Giảm giá bán theo số lượng mua']
            self.HinhthucKM(mang);
        }
        if (item.KieuHinhThuc == 11 || item.KieuHinhThuc == 21) {
            self.SelectHT(self.HinhthucKM()[0]);
            if (item.KieuHinhThuc == 11) {
                self.selectedHinhThuc();
                self.HD_GiamGiaHD(self.BH_KhuyenMai_ChiTiet())
                for (var i = 0; i < self.HD_GiamGiaHD().length; i++) {
                    var $this = $('#dvt_' + self.HD_GiamGiaHD()[i].ID);
                    if (self.HD_GiamGiaHD()[i].GiamGiaTheoPhanTram) {
                        $this.addClass('active-re');
                    }
                    else {
                        $this.removeClass('active-re');
                    }
                }

            }
            else {
                self.selectedHinhThuc();
                self.HH_MuaHangGiamGia(self.BH_KhuyenMai_ChiTiet())
                for (var i = 0; i < self.HH_MuaHangGiamGia().length; i++) {
                    var $this = $('#dvtMuaHangGiamGia_' + self.HH_MuaHangGiamGia()[i].ID);
                    if (self.HH_MuaHangGiamGia()[i].GiamGiaTheoPhanTram) {
                        $this.addClass('active-re');
                    }
                    else {
                        $this.removeClass('active-re');
                    }
                }
            }
        }
        if (item.KieuHinhThuc == 12 || item.KieuHinhThuc == 22) {
            self.selectedHinhThuc();
            self.SelectHT(self.HinhthucKM()[1]);
            if (item.KieuHinhThuc == 12) {
                self.HD_TangHang(self.BH_KhuyenMai_ChiTiet())
                console.log(self.HD_TangHang());
            }
            else {
                self.selectedHinhThuc();
                self.HH_MuaHangTangHang(self.BH_KhuyenMai_ChiTiet())
            }
        }
        if (item.KieuHinhThuc == 13 || item.KieuHinhThuc == 23) {

            self.SelectHT(self.HinhthucKM()[2]);
            if (item.KieuHinhThuc == 13) {
                self.selectedHinhThuc();
                self.HD_GiamGiaHang(self.BH_KhuyenMai_ChiTiet())
                console.log(self.HD_GiamGiaHang())
                for (var i = 0; i < self.HD_GiamGiaHang().length; i++) {
                    var $this = $('#dvtGiamGiaHang_' + self.HD_GiamGiaHang()[i].ID);
                    if (self.HD_GiamGiaHang()[i].GiamGiaTheoPhanTram) {
                        $this.addClass('active-re');
                    }
                    else {
                        $this.removeClass('active-re');
                    }
                }
            }
            else {
                self.selectedHinhThuc();
                self.HH_MuaHangTangDiem(self.BH_KhuyenMai_ChiTiet())
                for (var i = 0; i < self.HH_MuaHangTangDiem().length; i++) {
                    var $this = $('#dvtMuaHangTangDiem_' + self.HH_MuaHangTangDiem()[i].ID);
                    if (self.HH_MuaHangTangDiem()[i].GiamGiaTheoPhanTram) {
                        $this.addClass('active-re');
                    }
                    else {
                        $this.removeClass('active-re');
                    }
                }
            }
        }
        if (item.KieuHinhThuc == 14 || item.KieuHinhThuc == 24) {
            self.SelectHT(self.HinhthucKM()[3]);
            if (item.KieuHinhThuc == 14) {
                self.selectedHinhThuc();
                self.HD_TangDiem(self.BH_KhuyenMai_ChiTiet());
                for (var i = 0; i < self.HD_TangDiem().length; i++) {
                    var $this = $('#Diem_' + self.HD_TangDiem()[i].ID);
                    if (self.HD_TangDiem()[i].GiamGiaTheoPhanTram) {
                        $this.addClass('active-re');
                    }
                    else {
                        $this.removeClass('active-re');
                    }
                }
            }
            else {
                self.selectedHinhThuc();
                self.ItemKMChiTiet(self.BH_KhuyenMai_ChiTiet());
                ajaxHelper(BH_KhuyenMaiUri + "getListHangHoaKM?ID_KhuyenMai=" + item.ID, "GET").done(function (data) {
                    self.HH_GiaBanTheoSL(data);
                    for (var i = 0; i < self.HH_GiaBanTheoSL().length; i++) {
                        var newItem = self.HH_GiaBanTheoSL()[i];
                        self.HH_GiaBanTheoSL.replace(newItem, {
                            ID: i + 1,
                            ID_DonViQuiDoiMua: newItem.ID_DonViQuiDoiMua,
                            ID_NhomHangHoaMua: newItem.ID_NhomHangHoaMua,
                            TenHangHoaMua: newItem.TenHangHoaMua,
                            TenNhomHangHoaMua: newItem.TenNhomHangHoaMua
                        })
                        for (var j = 0; j < self.BH_KhuyenMai_ChiTiet().length; j++) {
                            var newThemHH = self.BH_KhuyenMai_ChiTiet()[j];
                            if ((newThemHH.ID_DonViQuiDoiMua == newItem.ID_DonViQuiDoiMua & newItem.ID_NhomHangHoaMua == null)
                                || (newItem.ID_DonViQuiDoiMua == null & newThemHH.ID_NhomHangHoaMua == newItem.ID_NhomHangHoaMua)) {
                                self.ItemKMChiTiet.replace(newThemHH, {
                                    ID: newThemHH.ID,
                                    ID_addHH: i + 1,
                                    SoLuongMua: newThemHH.SoLuongMua,
                                    GiaKhuyenMai: newThemHH.GiaKhuyenMai,
                                    GiamGia: newThemHH.GiamGia,
                                    GiamGiaTheoPhanTram: newThemHH.GiamGiaTheoPhanTram,
                                    IDRandom: CreateIDRandom('KM24_'),
                                })
                            }
                        }
                    }
                    self.ThemHangHoa(self.ItemKMChiTiet());
                    // selected GiaBan/GiamGia
                    setValueSelect_KM24();
                });
            }
            $('#txtNameKM').focus();
        }
        var ThangApDungKM = item.ThangApDung.split('Tháng')
        var NgayApDungKM = item.NgayApDung.split('Ngày')
        var TKM = item.ThuApDung.replace("Chủ nhật", "Thứ 8");
        var ThuApDungKM = TKM.split('Thứ')
        var GioApDungKM = item.GioApDung.split('Giờ');
        self.mangThangKM([]);
        self.mangNgayKM([]);
        self.mangThuKM([]);
        self.mangGioKM([]);
        for (var i = 1; i < ThangApDungKM.length; i++) {
            self.mangThangKM.push(ThangApDungKM[i].replace(",", "").trim());
            $('#selec-all-ThangKM li').each(function () {
                if ($(this).attr('id') === ThangApDungKM[i].replace(",", "").trim()) {
                    $(this).find('i').remove();
                    $(this).append('<i class="fa fa-check check-after-li"></i>')
                }
            });
        }
        for (var i = 1; i < NgayApDungKM.length; i++) {
            self.mangNgayKM.push(NgayApDungKM[i].replace(",", "").trim());
            $('#selec-all-NgayKM li').each(function () {
                if ($(this).attr('id') === NgayApDungKM[i].replace(",", "").trim()) {
                    $(this).find('i').remove();
                    $(this).append('<i class="fa fa-check check-after-li"></i>')
                }
            });
        }
        for (var i = 1; i < ThuApDungKM.length; i++) {
            self.mangThuKM.push(ThuApDungKM[i].replace(",", "").trim());
            $('#selec-all-ThuKM li').each(function () {
                if ($(this).attr('id') === ThuApDungKM[i].replace(",", "").trim()) {
                    $(this).find('i').remove();
                    $(this).append('<i class="fa fa-check check-after-li"></i>')
                }
            });
        }
        for (var i = 1; i < GioApDungKM.length; i++) {
            self.mangGioKM.push(GioApDungKM[i].replace(",", "").trim());
            $('#selec-all-GioKM li').each(function () {
                if ($(this).attr('id') === GioApDungKM[i].replace(",", "").trim()) {
                    $(this).find('i').remove();
                    $(this).append('<i class="fa fa-check check-after-li"></i>')
                }
            });
        }
        if (self.CN_CheckApDungSN() != 0) {
            self.CN_CheckApDungSN('1');
            $('#choose_NgaySN').attr("data-toggle", "dropdown");
            if (item.ValueApDungSN == 1) {
                _apdungngaysinhnhat = 1;
                $(this).parentsUntil(".checkdram ").find(".blue").html('Ngày sinh nhật');
                self.KieuApDungSN('Ngày sinh nhật')
            }
            if (item.ValueApDungSN == 2) {
                _apdungngaysinhnhat = 2;
                $(this).parentsUntil(".checkdram ").find(".blue").html('Tuần sinh nhật');
                self.KieuApDungSN('Tuần sinh nhật')
            }
            if (item.ValueApDungSN == 3) {
                _apdungngaysinhnhat = 3;
                $(this).parentsUntil(".checkdram ").find(".blue").html('Tháng sinh nhật');
                self.KieuApDungSN('Tháng sinh nhật')
            }
        }
        else {
            _apdungngaysinhnhat = 0;
            $('#choose_NgaySN').removeAttr('data-toggle');
            $(this).parentsUntil(".checkdram ").find(".blue").html('Ngày sinh nhật');
            self.KieuApDungSN('Ngày sinh nhật')
            $(".checkdram ul li i").hide();
        }
        ajaxHelper(BH_KhuyenMaiUri + "getLisDonViKM?ID_KhuyenMai=" + item.ID, "GET").done(function (data) {
            self.MangChiNhanh(data);
            if (self.MangChiNhanh().length > 0) {
                self.CN_checkChiNhanh('2');
                _tatcadonvi = false;
                $('#choose_DonVi').attr("data-toggle", "dropdown");
                $('#choose_DonVi input').remove();
                for (var i = 0; i < self.MangChiNhanh().length; i++) {
                    $('#selec-all-DonVi li').each(function () {
                        if ($(this).attr('id') === self.MangChiNhanh()[i].ID) {
                            $(this).find('i').remove();
                            $(this).append('<i class="fa fa-check check-after-li"></i>')
                        }
                    });
                }
            }
            else {
                self.CN_checkChiNhanh('1');
                _tatcadonvi = true;
                $('#choose_DonVi').removeAttr('data-toggle');
                $('#choose_DonVi input').remove();
                self.MangChiNhanh([]);
                $('#choose_DonVi').append('<input type="text" class="dropdown form-control" placeholder="Chọn chi nhánh áp dụng...">');
                $('#selec-all-DonVi li').each(function () {
                    $(this).find('i').remove();
                });
            }
        });
        ajaxHelper(BH_KhuyenMaiUri + "getlistNhanViemKM?ID_KhuyenMai=" + item.ID, "GET").done(function (data) {
            self.MangNhanVien(data);
            if (self.MangNhanVien().length > 0) {
                self.CN_checkNguoiBan('2');
                _tatcanhanvien = false;
                $('#choose-NguoiBan').attr("data-toggle", "dropdown");
                $('#choose-NguoiBan input').remove();
                for (var i = 0; i < self.MangNhanVien().length; i++) {
                    $('#selec-all-NguoiBan li').each(function () {
                        if ($(this).attr('id') === self.MangNhanVien()[i].ID) {
                            $(this).find('i').remove();
                            $(this).append('<i class="fa fa-check check-after-li"></i>')
                        }
                    });
                }
            }
            else {
                self.CN_checkNguoiBan('1');
                _tatcanhanvien = true;
                $('#choose-NguoiBan').removeAttr('data-toggle');
                $('#choose-NguoiBan input').remove();
                self.MangNhanVien([]);
                $('#choose-NguoiBan').append('<input type="text" class="dropdown form-control" placeholder="Chọn người bán áp dụng...">');
                $('#selec-all-NguoiBan li').each(function () {
                    $(this).find('i').remove();
                });
            }
        });
        ajaxHelper(BH_KhuyenMaiUri + "getlistNhomHangKM?ID_KhuyenMai=" + item.ID, "GET").done(function (data) {
            self.MangNhomKhachHang(data);
            if (self.MangNhomKhachHang().length > 0) {
                self.CN_checkNhomKhachHang('2');
                _tatcadoituong = false;
                $('#choose-NhomKhachHang').attr("data-toggle", "dropdown");
                $('#choose-NhomKhachHang input').remove();
                for (var i = 0; i < self.MangNhomKhachHang().length; i++) {
                    $('#selec-all-NhomKhachHang li').each(function () {
                        if ($(this).attr('id') === self.MangNhomKhachHang()[i].ID) {
                            $(this).find('i').remove();
                            $(this).append('<i class="fa fa-check check-after-li"></i>')
                        }
                    });
                }
            }
            else {
                self.CN_checkNhomKhachHang('1');
                _tatcadoituong = true;
                $('#choose-NhomKhachHang').removeAttr('data-toggle');
                $('#choose-NhomKhachHang input').remove();
                self.MangNhomKhachHang([]);
                $('#choose-NhomKhachHang').append('<input type="text" class="dropdown form-control" placeholder="Chọn nhóm khách hàng áp dụng...">');
                $('#selec-all-NhomKhachHang li').each(function () {
                    $(this).find('i').remove();
                });
            }
        });
        //self.selectedHinhThuc();
    }
    function resertCheck() {
        _apdungngaysinhnhat = 0;
        $('#choose_NgaySN').removeAttr('data-toggle');
        $(".checkdram ").find(".blue").html('Ngày sinh nhật');
        $(".checkdram ul li i").hide();
    };
    var _dieukienSave = 1;
    self.TaoMoi = function () {
        self.TieuDe('Thêm mới chương trình khuyến mại');
        // console.log(self.KieuApDungSN());
        //if (_dieukienSave != 1) {
        //    _dieukienSave = 1;
        $('.trade-tabb li').each(function () {
            $(this).removeClass('active');
        });
        $('#htKM').addClass('active');
        $('#thoigian').removeClass('active');
        $('#phamviapdung').removeClass('active');
        $('#hinhthuc').addClass('active');
        $(".table-reduced").hide();
        $(".table-reducedhh").hide();
        $('.giamgiahoadon').show();
        self.CN_maKM([]);
        _maKM = null;
        self.CN_TenKM([]);
        _tenKM = null
        self.CN_TrangThaiKM('1');
        _trangthaiKM = true;
        self.CN_Ghichu([]);
        _ghichuKM = null
        $('#txtNoteKM').val("");
        self.CN_CheckApDungSN(0);
        resertCheck();
        self.CN_checkChiNhanh('1');
        _tatcadonvi = true
        $('#choose_DonVi').removeAttr('data-toggle');
        $('#choose_DonVi input').remove();
        self.MangChiNhanh([]);
        $('#choose_DonVi').append('<input type="text" class="dropdown form-control" placeholder="Chọn chi nhánh áp dụng...">');
        $('#selec-all-DonVi li').each(function () {
            $(this).find('i').remove();
        });
        self.CN_checkNguoiBan('1');
        _tatcanhanvien = true;
        $('#choose-NguoiBan').removeAttr('data-toggle');
        $('#choose-NguoiBan input').remove();
        self.MangNhanVien([]);
        $('#choose-NguoiBan').append('<input type="text" class="dropdown" placeholder="Chọn người bán áp dụng...">');
        $('#selec-all-NguoiBan li').each(function () {
            $(this).find('i').remove();
        });
        self.CN_checkNhomKhachHang('1');
        _tatcadoituong = true;
        $('#choose-NhomKhachHang').removeAttr('data-toggle');
        $('#choose-NhomKhachHang input').remove();
        self.MangNhomKhachHang([]);
        $('#choose-NhomKhachHang').append('<input type="text" class="dropdown" placeholder="Chọn nhóm khách hàng áp dụng...">');
        $('#selec-all-NhomKhachHang li').each(function () {
            $(this).find('i').remove();
        });
        var dateTime = new Date();
        _timeStart = moment(dateTime).format('YYYY-MM-DD HH:mm');
        _timeEnd = moment(new Date(dateTime.getFullYear(), dateTime.getMonth() + 5, 1)).format('YYYY-MM-DD HH:mm');
        self.ThoiGianBatDauKM(moment(_timeStart).format('DD/MM/YYYY HH:mm'));
        self.ThoiGianKetThucKM(moment(_timeEnd).format('DD/MM/YYYY HH:mm'));
        self.mangThangKM([]);
        $('#selec-all-ThangKM li').each(function () {
            $(this).find('i').remove();
        });
        self.mangNgayKM([]);
        $('#selec-all-NgayKM li').each(function () {
            $(this).find('i').remove();
        });
        self.mangThuKM([]);
        $('#selec-all-ThuKM li').each(function () {
            $(this).find('i').remove();
        });
        self.mangGioKM([]);
        $('#selec-all-GioKM li').each(function () {
            $(this).find('i').remove();
        });
        $('#choose_DonVi').removeAttr('data-toggle');
        $('#choose-NguoiBan').removeAttr('data-toggle');
        $('#choose-NhomKhachHang').removeAttr('data-toggle');
        $('#choose_NgaySN').removeAttr('data-toggle');
        Lc_GiamGiaHD = [{ ID: 1, TongTienHang: '0', GiamGia: null, GiamGiaTheoPhanTram: true }];
        Lc_TangHang = [{ ID: 1, TongTienHang: '0', SoLuong: 1, ID_DonViQuiDoi: null, TenHangHoa: null, ID_NhomHangHoa: null, TenNhomHangHoa: null }];
        Lc_GiamGiaHang = [{ ID: 1, TongTienHang: '0', GiamGia: null, GiamGiaTheoPhanTram: true, SoLuong: 1, ID_DonViQuiDoi: null, TenHangHoa: null, ID_NhomHangHoa: null, TenNhomHangHoa: null }];
        Lc_TangDiem = [{ ID: 1, TongTienHang: '0', DiemCong: null, GiamGiaTheoPhanTram: true }];
        Lc_MuaHangGiamGia = [{ ID: 1, SoLuongMua: 1, ID_DonViQuiDoiMua: null, TenHangHoaMua: null, ID_NhomHangHoaMua: null, TenNhomHangHoaMua: null, GiamGia: null, GiamGiaTheoPhanTram: true, SoLuong: 1, ID_DonViQuiDoi: null, TenHangHoa: null, ID_NhomHangHoa: null, TenNhomHangHoa: null }];
        Lc_MuaHangTangHang = [{ ID: 1, SoLuongMua: 1, ID_DonViQuiDoiMua: null, TenHangHoaMua: null, ID_NhomHangHoaMua: null, TenNhomHangHoaMua: null, SoLuong: 1, ID_DonViQuiDoi: null, TenHangHoa: null, ID_NhomHangHoa: null, TenNhomHangHoa: null }];
        Lc_MuaHangTangDiem = [{ ID: 1, SoLuongMua: 1, ID_DonViQuiDoiMua: null, TenHangHoaMua: null, ID_NhomHangHoaMua: null, TenNhomHangHoaMua: null, DiemCong: null, GiamGiaTheoPhanTram: true }];
        Lc_GiaBanTheoSL = [{ ID: 1, SoLuongMua: 1, ID_DonViQuiDoiMua: null, TenHangHoaMua: null, ID_NhomHangHoaMua: null, TenNhomHangHoaMua: null, GiaKhuyenMai: null, }];
        Lc_ThemHangHoa = [{ ID_addHH: 1, SoLuongMua: 1, ID_DonViQuiDoi: null, TenHangHoa: null, ID_NhomHangHoa: null, TenNhomHangHoa: null, GiaKhuyenMai: null, GiamGia: null, GiamGiaTheoPhanTram: null, IDRandom: CreateIDRandom('KM24_'), TypeSelect: '1' }]
        self.HD_GiamGiaHD(Lc_GiamGiaHD);
        self.HD_TangHang(Lc_TangHang);
        self.HD_GiamGiaHang(Lc_GiamGiaHang);
        self.HD_TangDiem(Lc_TangDiem);
        self.HH_MuaHangGiamGia(Lc_MuaHangGiamGia);
        self.HH_MuaHangTangHang(Lc_MuaHangTangHang);
        self.HH_MuaHangTangDiem(Lc_MuaHangTangDiem);
        self.HH_GiaBanTheoSL(Lc_GiaBanTheoSL);
        self.ThemHangHoa(Lc_ThemHangHoa);
        _loaiKM = 1;
        _hinhthucKM = 11;
        self.txtKhuyenMai("Hóa đơn");
        self.HinhthucKM(mang1);
        self.SelectHT(self.HinhthucKM()[0]);
        // $('#txtNameKM').focus();
        //}
    }
    var _ID_KhuyenMai_Delete;
    var _Ma_KhuyenMai_Delete;
    var itemDelete;
    self.maKhuyenMaiDelete = ko.observable();
    self.modalDelete = function (item) {
        itemDelete = item;
        $('#modalpopup_deleteHD').modal('show');
        _ID_KhuyenMai_Delete = item.ID;
        _Ma_KhuyenMai_Delete = item.MaKhuyenMai;
        self.maKhuyenMaiDelete(_Ma_KhuyenMai_Delete);
    };
    self.xoaKhuyenMai = function () {
        ajaxHelper(BH_KhuyenMaiUri + "deleteKhuyenMai?ID_KhuyenMai=" + _ID_KhuyenMai_Delete + "&ID_DonVi=" + _id_DonViLS + "&ID_NhanVien=" + _id_NhanVienLS, "GET").done(function (data) {
            var str = data;
            if (str.length > 0) {
                if (str == 'HD')
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Chương trình khuyến mại: " + _Ma_KhuyenMai_Delete + " đã được áp dụng", "danger");
                else
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Xóa bỏ mã chương trình khuyến mại: " + _Ma_KhuyenMai_Delete + " không thành công", "danger");
            }
            else {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Xóa bỏ mã chương trình khuyến mại: " + _Ma_KhuyenMai_Delete + " thành công", "success");
                //self.filteredDM_KhuyenMai.remove(itemDelete);
                GetListPromotion();
            }
        });
        $('#modalpopup_deleteHD').modal('hide');
    }
    // lọc tháng KM
    self.NoteThangKM = function () {

        var arrIDThangKM = [];
        var itemSearch = locdau($('#NoteThangKhuyenMai').val().toLowerCase());
        for (var i = 0; i < self.SeachThangKM().length; i++) {

            var locdauInput = locdau("Tháng " + self.SeachThangKM()[i]).toLowerCase();
            var R = locdauInput.split(itemSearch);
            if (R.length > 1) {
                arrIDThangKM.push(self.SeachThangKM()[i]);
            }
        }
        self.ThangKM(arrIDThangKM);
        if ($('#NoteThangKhuyenMai').val() == "") {
            self.ThangKM(mangThang);
        }
    }
    $('#NoteThangKhuyenMai').keypress(function (e) {
        if (e.keyCode == 13 && self.ThangKM().length > 0) {
            var mang = $('#NoteThangKhuyenMai').val().split(' ');
            self.SelectedThangKM(mang[mang.length - 1]);
        }
    });
    // lọc Ngày KM
    self.searchNgayKM = ko.observableArray(mangNgay);
    self.NoteNgayKM = function () {
        var arrIDNgayKM = [];
        var itemSearch = locdau($('#NoteNgayKhuyenMai').val().toLowerCase());
        for (var i = 0; i < self.searchNgayKM().length; i++) {
            var locdauInput = locdau("Ngày " + self.searchNgayKM()[i]).toLowerCase();
            var R = locdauInput.split(itemSearch);
            if (R.length > 1) {
                arrIDNgayKM.push(self.searchNgayKM()[i]);
            }
        }
        self.NgayKM(arrIDNgayKM);
        if ($('#NoteNgayKhuyenMai').val() == "") {
            self.NgayKM(mangNgay);
        }
    }
    $('#NoteNgayKhuyenMai').keypress(function (e) {
        if (e.keyCode == 13 && self.NgayKM().length > 0) {
            self.SelectedNgayKM(self.NgayKM()[0]);
        }
    });
    // lọc Thứ KM
    self.searchThuKM = ko.observableArray(mangThu);
    self.NoteThuKM = function () {

        var arrIDThuKM = [];
        var locdauInput;
        var itemSearch = locdau($('#NoteThuKhuyenMai').val().toLowerCase());
        for (var i = 0; i < self.searchThuKM().length; i++) {
            if (self.searchThuKM()[i] === '8')
                locdauInput = locdau("Chủ nhật").toLowerCase();
            else
                locdauInput = locdau("Thứ " + self.searchThuKM()[i]).toLowerCase();
            var R = locdauInput.split(itemSearch);
            if (R.length > 1) {
                arrIDThuKM.push(self.searchThuKM()[i]);
            }
        }
        self.ThuKM(arrIDThuKM);
        if ($('#NoteThuKhuyenMai').val() == "") {
            self.ThuKM(mangThu);
        }
    }
    $('#NoteThuKhuyenMai').keypress(function (e) {
        if (e.keyCode == 13 && self.ThuKM().length > 0) {
            self.SelectedThuKM(self.ThuKM()[0]);
        }
    });
    self.SelectedHHEnterkey = function () {
        if (_hinhthucKM == 21) {
            if (dk_Enter == 1)
                getChiTietHangHoaByMaHH($('#txtHH21' + SaveItem.ID).val().toUpperCase());
            else
                getChiTietHangHoaByMaHH($('#txtHH212' + SaveItem.ID).val().toUpperCase());
        }
        else if (_hinhthucKM == 12) {
            getChiTietHangHoaByMaHH($('#txtHH12' + SaveItem.ID).val().toUpperCase());
        }
        else if (_hinhthucKM == 13) {
            getChiTietHangHoaByMaHH($('#txtHH13' + SaveItem.ID).val().toUpperCase());
        }
        else if (_hinhthucKM == 22) {
            if (dk_Enter == 1)
                getChiTietHangHoaByMaHH($('#txtHH22' + SaveItem.ID).val().toUpperCase());
            else
                getChiTietHangHoaByMaHH($('#txtHH222' + SaveItem.ID).val().toUpperCase());
        }
        else if (_hinhthucKM == 23) {
            getChiTietHangHoaByMaHH($('#txtHH23' + SaveItem.ID).val().toUpperCase());
        }
        else if (_hinhthucKM == 24) {
            getChiTietHangHoaByMaHH($('#txtHH24' + SaveItem.ID).val().toUpperCase());
        }
    }
    function getChiTietHangHoaByMaHH(MaHH) {
        console.log(MaHH);
        if (dk_Enter == 1) {
            ajaxHelper(BH_KhuyenMaiUri + "getHangHoaKMBy_MaHangHoa?maHH=" + MaHH, 'GET').done(function (data) {
                console.log(data);
                _note_ID_DonViQuiDoiMua_TangHang = data[0].ID;
                _note_TenHangHoaMua_TangHang = data[0].TenHangHoaFull;
                _note_ID_NhomHangHoaMua_TangHang = null;
                if (_hinhthucKM == 21) {
                    self.HH_MuaHangGiamGia.replace(SaveItem, {
                        ID: SaveItem.ID,
                        SoLuongMua: SaveItem.SoLuongMua,
                        ID_DonViQuiDoiMua: _note_ID_DonViQuiDoiMua_TangHang,
                        TenHangHoaMua: _note_TenHangHoaMua_TangHang,
                        ID_NhomHangHoaMua: null,
                        TenNhomHangHoaMua: null,
                        GiamGia: SaveItem.GiamGia,
                        GiamGiaTheoPhanTram: SaveItem.GiamGiaTheoPhanTram,
                        SoLuong: SaveItem.SoLuong,
                        ID_DonViQuiDoi: SaveItem.ID_DonViQuiDoi,
                        TenHangHoa: SaveItem.TenHangHoa,
                        ID_NhomHangHoa: SaveItem.ID_NhomHangHoa,
                        TenNhomHangHoa: SaveItem.TenNhomHangHoa
                    })
                    self.SaveHD_TangHang(self.HH_MuaHangGiamGia());
                    self.HH_MuaHangGiamGia([]);
                    for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
                        self.HH_MuaHangGiamGia.push(self.SaveHD_TangHang()[i]);
                    }
                    for (var i = 0; i < self.HH_MuaHangGiamGia().length; i++) {
                        var $this = $('#dvtMuaHangGiamGia_' + self.HH_MuaHangGiamGia()[i].ID);
                        if (self.HH_MuaHangGiamGia()[i].GiamGiaTheoPhanTram) {
                            $this.addClass('active-re');
                        }
                        else {
                            $this.removeClass('active-re');
                        }
                    }
                }
                else if (_hinhthucKM == 22) {
                    self.HH_MuaHangTangHang.replace(SaveItem, {
                        ID: SaveItem.ID,
                        SoLuongMua: SaveItem.SoLuongMua,
                        ID_DonViQuiDoiMua: _note_ID_DonViQuiDoiMua_TangHang,
                        TenHangHoaMua: _note_TenHangHoaMua_TangHang,
                        ID_NhomHangHoaMua: null,
                        TenNhomHangHoaMua: null,
                        SoLuong: SaveItem.SoLuong,
                        ID_DonViQuiDoi: SaveItem.ID_DonViQuiDoi,
                        TenHangHoa: SaveItem.TenHangHoa,
                        ID_NhomHangHoa: SaveItem.ID_NhomHangHoa,
                        TenNhomHangHoa: SaveItem.TenNhomHangHoa
                    })
                    self.SaveHD_TangHang(self.HH_MuaHangTangHang());
                    self.HH_MuaHangTangHang([]);
                    for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
                        self.HH_MuaHangTangHang.push(self.SaveHD_TangHang()[i]);
                    }
                }
                else if (_hinhthucKM == 23) {
                    self.HH_MuaHangTangDiem.replace(SaveItem,
                        {
                            ID: SaveItem.ID,
                            SoLuongMua: SaveItem.SoLuongMua,
                            ID_DonViQuiDoiMua: _note_ID_DonViQuiDoiMua_TangHang,
                            TenHangHoaMua: _note_TenHangHoaMua_TangHang,
                            ID_NhomHangHoaMua: SaveItem.ID_NhomHangHoaMua,
                            TenNhomHangHoaMua: SaveItem.TenNhomHangHoaMua,
                            DiemCong: SaveItem.DiemCong,
                            GiamGiaTheoPhanTram: SaveItem.GiamGiaTheoPhanTram
                        })
                    self.SaveHD_TangHang(self.HH_MuaHangTangDiem());
                    self.HH_MuaHangTangDiem([]);
                    for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
                        self.HH_MuaHangTangDiem.push(self.SaveHD_TangHang()[i]);
                    }
                    for (var i = 0; i < self.HH_MuaHangTangDiem().length; i++) {
                        var $this = $('#dvtMuaHangTangDiem_' + self.HH_MuaHangTangDiem()[i].ID);
                        if (self.HH_MuaHangTangDiem()[i].GiamGiaTheoPhanTram) {
                            $this.addClass('active-re');
                        }
                        else {
                            $this.removeClass('active-re');
                        }
                    }
                }
                else if (_hinhthucKM == 24) {
                    debugger
                    self.HH_GiaBanTheoSL.replace(SaveItem, {
                        ID: SaveItem.ID,
                        SoLuongMua: 1,
                        ID_DonViQuiDoiMua: _note_ID_DonViQuiDoiMua_TangHang,
                        TenHangHoaMua: _note_TenHangHoaMua_TangHang,
                        ID_NhomHangHoaMua: null,
                        TenNhomHangHoaMua: null,
                        GiaKhuyenMai: null
                    })
                    console.log(self.HH_GiaBanTheoSL());
                    self.SaveHD_TangHang(self.HH_GiaBanTheoSL());
                    self.HH_GiaBanTheoSL([]);
                    for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
                        self.HH_GiaBanTheoSL.push(self.SaveHD_TangHang()[i]);
                    }
                }
            });
        }
        else {
            ajaxHelper(BH_KhuyenMaiUri + "getHangHoaKMBy_MaHangHoa?maHH=" + MaHH, 'GET').done(function (data) {
                _id_DonViQuyDoi = data[0].ID;
                _note_ID_NhomHangHoa_TangHang = null;
                _note_TenHangHoa_TangHang = data[0].TenHangHoaFull;
                //console.log(_id_DonViQuyDoi);
                if (_hinhthucKM == 12) {
                    self.HD_TangHang.replace(SaveItem, {
                        ID: SaveItem.ID,
                        TongTienHang: SaveItem.TongTienHang,
                        SoLuong: SaveItem.SoLuong,
                        ID_DonViQuiDoi: _id_DonViQuyDoi,
                        TenHangHoa: _note_TenHangHoa_TangHang,
                        ID_NhomHangHoa: null,
                        TenNhomHangHoa: null
                    })
                    console.log(self.HD_TangHang());
                    self.SaveHD_TangHang(self.HD_TangHang());
                    self.HD_TangHang([]);
                    for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
                        self.HD_TangHang.push(self.SaveHD_TangHang()[i]);
                    }
                    //self.HD_TangHang(self.SaveHD_TangHang);
                }
                else if (_hinhthucKM == 13) {
                    console.log(SaveItem);
                    self.HD_GiamGiaHang.replace(SaveItem, {
                        ID: SaveItem.ID,
                        TongTienHang: SaveItem.TongTienHang,
                        GiamGia: SaveItem.GiamGia,
                        GiamGiaTheoPhanTram: SaveItem.GiamGiaTheoPhanTram,
                        SoLuong: SaveItem.SoLuong,
                        ID_DonViQuiDoi: _id_DonViQuyDoi,
                        TenHangHoa: _note_TenHangHoa_TangHang,
                        ID_NhomHangHoa: null,
                        TenNhomHangHoa: null
                    });
                    console.log(self.HD_GiamGiaHang());
                    self.SaveHD_TangHang(self.HD_GiamGiaHang());
                    self.HD_GiamGiaHang([]);
                    for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
                        self.HD_GiamGiaHang.push(self.SaveHD_TangHang()[i]);
                    }
                    for (var i = 0; i < self.HD_GiamGiaHang().length; i++) {
                        var $this = $('#dvtGiamGiaHang_' + self.HD_GiamGiaHang()[i].ID);
                        if (self.HD_GiamGiaHang()[i].GiamGiaTheoPhanTram) {
                            $this.addClass('active-re');
                        }
                        else {
                            $this.removeClass('active-re');
                        }
                    }

                }
                else if (_hinhthucKM == 21) {
                    self.HH_MuaHangGiamGia.replace(SaveItem, {
                        ID: SaveItem.ID,
                        SoLuongMua: SaveItem.SoLuongMua,
                        ID_DonViQuiDoiMua: SaveItem.ID_DonViQuiDoiMua,
                        TenHangHoaMua: SaveItem.TenHangHoaMua,
                        ID_NhomHangHoaMua: SaveItem.ID_NhomHangHoaMua,
                        TenNhomHangHoaMua: SaveItem.TenNhomHangHoaMua,
                        GiamGia: SaveItem.GiamGia,
                        GiamGiaTheoPhanTram: SaveItem.GiamGiaTheoPhanTram,
                        SoLuong: SaveItem.SoLuong,
                        ID_DonViQuiDoi: _id_DonViQuyDoi,
                        TenHangHoa: _note_TenHangHoa_TangHang,
                        ID_NhomHangHoa: null,
                        TenNhomHangHoa: null
                    })
                    console.log(self.HH_MuaHangGiamGia());
                    self.SaveHD_TangHang(self.HH_MuaHangGiamGia());
                    self.HH_MuaHangGiamGia([]);
                    for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
                        self.HH_MuaHangGiamGia.push(self.SaveHD_TangHang()[i]);
                    }
                    for (var i = 0; i < self.HH_MuaHangGiamGia().length; i++) {
                        var $this = $('#dvtMuaHangGiamGia_' + self.HH_MuaHangGiamGia()[i].ID);
                        if (self.HH_MuaHangGiamGia()[i].GiamGiaTheoPhanTram) {
                            $this.addClass('active-re');
                        }
                        else {
                            $this.removeClass('active-re');
                        }
                    }

                }
                else if (_hinhthucKM == 22) {
                    self.HH_MuaHangTangHang.replace(SaveItem, {
                        ID: SaveItem.ID,
                        SoLuongMua: SaveItem.SoLuongMua,
                        ID_DonViQuiDoiMua: SaveItem.ID_DonViQuiDoiMua,
                        TenHangHoaMua: SaveItem.TenHangHoaMua,
                        ID_NhomHangHoaMua: SaveItem.ID_NhomHangHoaMua,
                        TenNhomHangHoaMua: SaveItem.TenNhomHangHoaMua,
                        SoLuong: SaveItem.SoLuong,
                        ID_DonViQuiDoi: _id_DonViQuyDoi,
                        TenHangHoa: _note_TenHangHoa_TangHang,
                        ID_NhomHangHoa: null,
                        TenNhomHangHoa: null
                    })
                    console.log(self.HH_MuaHangTangHang())
                    self.SaveHD_TangHang(self.HH_MuaHangTangHang());
                    self.HH_MuaHangTangHang([]);
                    for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
                        self.HH_MuaHangTangHang.push(self.SaveHD_TangHang()[i]);
                    }
                }
            });
        }


    }
    self.getChiTietHangHoaByMaHH_byItem = function (item) {
        if (dk_Enter == 1) {
            _note_ID_DonViQuiDoiMua_TangHang = item.ID_DonViQuiDoi;
            _note_TenHangHoaMua_TangHang = item.TenHangHoaFull;
            _note_ID_NhomHangHoaMua_TangHang = null;
            if (_hinhthucKM == 21) {
                self.HH_MuaHangGiamGia.replace(SaveItem, {
                    ID: SaveItem.ID,
                    SoLuongMua: SaveItem.SoLuongMua,
                    ID_DonViQuiDoiMua: _note_ID_DonViQuiDoiMua_TangHang,
                    TenHangHoaMua: _note_TenHangHoaMua_TangHang,
                    ID_NhomHangHoaMua: null,
                    TenNhomHangHoaMua: null,
                    GiamGia: SaveItem.GiamGia,
                    GiamGiaTheoPhanTram: SaveItem.GiamGiaTheoPhanTram,
                    SoLuong: SaveItem.SoLuong,
                    ID_DonViQuiDoi: SaveItem.ID_DonViQuiDoi,
                    TenHangHoa: SaveItem.TenHangHoa,
                    ID_NhomHangHoa: SaveItem.ID_NhomHangHoa,
                    TenNhomHangHoa: SaveItem.TenNhomHangHoa
                })
                self.SaveHD_TangHang(self.HH_MuaHangGiamGia());
                self.HH_MuaHangGiamGia([]);
                for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
                    self.HH_MuaHangGiamGia.push(self.SaveHD_TangHang()[i]);
                }
                for (var i = 0; i < self.HH_MuaHangGiamGia().length; i++) {
                    var $this = $('#dvtMuaHangGiamGia_' + self.HH_MuaHangGiamGia()[i].ID);
                    if (self.HH_MuaHangGiamGia()[i].GiamGiaTheoPhanTram) {
                        $this.addClass('active-re');
                    }
                    else {
                        $this.removeClass('active-re');
                    }
                }
            }
            else if (_hinhthucKM == 22) {
                self.HH_MuaHangTangHang.replace(SaveItem, {
                    ID: SaveItem.ID,
                    SoLuongMua: SaveItem.SoLuongMua,
                    ID_DonViQuiDoiMua: _note_ID_DonViQuiDoiMua_TangHang,
                    TenHangHoaMua: _note_TenHangHoaMua_TangHang,
                    ID_NhomHangHoaMua: null,
                    TenNhomHangHoaMua: null,
                    SoLuong: SaveItem.SoLuong,
                    ID_DonViQuiDoi: SaveItem.ID_DonViQuiDoi,
                    TenHangHoa: SaveItem.TenHangHoa,
                    ID_NhomHangHoa: SaveItem.ID_NhomHangHoa,
                    TenNhomHangHoa: SaveItem.TenNhomHangHoa
                })
                self.SaveHD_TangHang(self.HH_MuaHangTangHang());
                self.HH_MuaHangTangHang([]);
                for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
                    self.HH_MuaHangTangHang.push(self.SaveHD_TangHang()[i]);
                }
            }
            else if (_hinhthucKM == 23) {
                self.HH_MuaHangTangDiem.replace(SaveItem,
                    {
                        ID: SaveItem.ID,
                        SoLuongMua: SaveItem.SoLuongMua,
                        ID_DonViQuiDoiMua: _note_ID_DonViQuiDoiMua_TangHang,
                        TenHangHoaMua: _note_TenHangHoaMua_TangHang,
                        ID_NhomHangHoaMua: SaveItem.ID_NhomHangHoaMua,
                        TenNhomHangHoaMua: SaveItem.TenNhomHangHoaMua,
                        DiemCong: SaveItem.DiemCong,
                        GiamGiaTheoPhanTram: SaveItem.GiamGiaTheoPhanTram
                    })
                self.SaveHD_TangHang(self.HH_MuaHangTangDiem());
                self.HH_MuaHangTangDiem([]);
                for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
                    self.HH_MuaHangTangDiem.push(self.SaveHD_TangHang()[i]);
                }
                for (var i = 0; i < self.HH_MuaHangTangDiem().length; i++) {
                    var $this = $('#dvtMuaHangTangDiem_' + self.HH_MuaHangTangDiem()[i].ID);
                    if (self.HH_MuaHangTangDiem()[i].GiamGiaTheoPhanTram) {
                        $this.addClass('active-re');
                    }
                    else {
                        $this.removeClass('active-re');
                    }
                }
            }
            else if (_hinhthucKM == 24) {
                debugger
                self.HH_GiaBanTheoSL.replace(SaveItem, {
                    ID: SaveItem.ID,
                    SoLuongMua: 1,
                    ID_DonViQuiDoiMua: _note_ID_DonViQuiDoiMua_TangHang,
                    TenHangHoaMua: _note_TenHangHoaMua_TangHang,
                    ID_NhomHangHoaMua: null,
                    TenNhomHangHoaMua: null,
                    GiaKhuyenMai: null
                })
                console.log(self.HH_GiaBanTheoSL());
                self.SaveHD_TangHang(self.HH_GiaBanTheoSL());
                //self.HH_GiaBanTheoSL([]);
                //for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
                //    self.HH_GiaBanTheoSL.push(self.SaveHD_TangHang()[i]);
                //}
            }
        }
        else {
            _id_DonViQuyDoi = item.ID_DonViQuiDoi;
            _note_ID_NhomHangHoa_TangHang = null;
            _note_TenHangHoa_TangHang = item.TenHangHoaFull;
            if (_hinhthucKM == 12) {
                self.HD_TangHang.replace(SaveItem, {
                    ID: SaveItem.ID,
                    TongTienHang: SaveItem.TongTienHang,
                    SoLuong: SaveItem.SoLuong,
                    ID_DonViQuiDoi: _id_DonViQuyDoi,
                    TenHangHoa: _note_TenHangHoa_TangHang,
                    ID_NhomHangHoa: null,
                    TenNhomHangHoa: null
                })
                console.log(self.HD_TangHang());
                self.SaveHD_TangHang(self.HD_TangHang());
                self.HD_TangHang([]);
                for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
                    self.HD_TangHang.push(self.SaveHD_TangHang()[i]);
                }
            }
            else if (_hinhthucKM == 13) {
                console.log(SaveItem);
                self.HD_GiamGiaHang.replace(SaveItem, {
                    ID: SaveItem.ID,
                    TongTienHang: SaveItem.TongTienHang,
                    GiamGia: SaveItem.GiamGia,
                    GiamGiaTheoPhanTram: SaveItem.GiamGiaTheoPhanTram,
                    SoLuong: SaveItem.SoLuong,
                    ID_DonViQuiDoi: _id_DonViQuyDoi,
                    TenHangHoa: _note_TenHangHoa_TangHang,
                    ID_NhomHangHoa: null,
                    TenNhomHangHoa: null
                });
                console.log(self.HD_GiamGiaHang());
                self.SaveHD_TangHang(self.HD_GiamGiaHang());
                self.HD_GiamGiaHang([]);
                for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
                    self.HD_GiamGiaHang.push(self.SaveHD_TangHang()[i]);
                }
                for (var i = 0; i < self.HD_GiamGiaHang().length; i++) {
                    var $this = $('#dvtGiamGiaHang_' + self.HD_GiamGiaHang()[i].ID);
                    if (self.HD_GiamGiaHang()[i].GiamGiaTheoPhanTram) {
                        $this.addClass('active-re');
                    }
                    else {
                        $this.removeClass('active-re');
                    }
                }

            }
            else if (_hinhthucKM == 21) {
                self.HH_MuaHangGiamGia.replace(SaveItem, {
                    ID: SaveItem.ID,
                    SoLuongMua: SaveItem.SoLuongMua,
                    ID_DonViQuiDoiMua: SaveItem.ID_DonViQuiDoiMua,
                    TenHangHoaMua: SaveItem.TenHangHoaMua,
                    ID_NhomHangHoaMua: SaveItem.ID_NhomHangHoaMua,
                    TenNhomHangHoaMua: SaveItem.TenNhomHangHoaMua,
                    GiamGia: SaveItem.GiamGia,
                    GiamGiaTheoPhanTram: SaveItem.GiamGiaTheoPhanTram,
                    SoLuong: SaveItem.SoLuong,
                    ID_DonViQuiDoi: _id_DonViQuyDoi,
                    TenHangHoa: _note_TenHangHoa_TangHang,
                    ID_NhomHangHoa: null,
                    TenNhomHangHoa: null
                })
                console.log(self.HH_MuaHangGiamGia());
                self.SaveHD_TangHang(self.HH_MuaHangGiamGia());
                self.HH_MuaHangGiamGia([]);
                for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
                    self.HH_MuaHangGiamGia.push(self.SaveHD_TangHang()[i]);
                }
                for (var i = 0; i < self.HH_MuaHangGiamGia().length; i++) {
                    var $this = $('#dvtMuaHangGiamGia_' + self.HH_MuaHangGiamGia()[i].ID);
                    if (self.HH_MuaHangGiamGia()[i].GiamGiaTheoPhanTram) {
                        $this.addClass('active-re');
                    }
                    else {
                        $this.removeClass('active-re');
                    }
                }

            }
            else if (_hinhthucKM == 22) {
                self.HH_MuaHangTangHang.replace(SaveItem, {
                    ID: SaveItem.ID,
                    SoLuongMua: SaveItem.SoLuongMua,
                    ID_DonViQuiDoiMua: SaveItem.ID_DonViQuiDoiMua,
                    TenHangHoaMua: SaveItem.TenHangHoaMua,
                    ID_NhomHangHoaMua: SaveItem.ID_NhomHangHoaMua,
                    TenNhomHangHoaMua: SaveItem.TenNhomHangHoaMua,
                    SoLuong: SaveItem.SoLuong,
                    ID_DonViQuiDoi: _id_DonViQuyDoi,
                    TenHangHoa: _note_TenHangHoa_TangHang,
                    ID_NhomHangHoa: null,
                    TenNhomHangHoa: null
                })
                console.log(self.HH_MuaHangTangHang())
                self.SaveHD_TangHang(self.HH_MuaHangTangHang());
                self.HH_MuaHangTangHang([]);
                for (var i = 0; i < self.SaveHD_TangHang().length; i++) {
                    self.HH_MuaHangTangHang.push(self.SaveHD_TangHang()[i]);
                }
            }
        }
    }
    //===============================
    // Load lai form lưu cache bộ lọc 
    // trên grid 
    //===============================
    function LoadHtmlGrid() {
        if (window.localStorage) {
            var current = localStorage.getItem('QLkhuyenmai');
            if (!current) {
                current = [{
                    NameClass: ".m4",
                    NameId: "e4"
                },
                {
                    NameClass: ".m5",
                    NameId: "e5"
                }];
                for (var i = 0; i < current.length; i++) {
                    $(current[i].NameClass).addClass("operation");
                    document.getElementById(current[i].NameId).checked = false;

                }
                localStorage.setItem('QLkhuyenmai', JSON.stringify(current));
            } else {
                current = JSON.parse(current);
                for (var i = 0; i < current.length; i++) {
                    $(current[i].NameClass).addClass("operation");
                    document.getElementById(current[i].NameId).checked = false;

                }
            }
        }
    }
    //===============================
    // Add Các tham số cần lưu lại để 
    // cache khi load lại form
    //===============================
    function addClass(name, id, value) {

        var current = localStorage.getItem('QLkhuyenmai');
        if (!current) {
            current = [{
                NameClass: ".m4",
                NameId: "e4"
            },
            {
                NameClass: ".m5",
                NameId: "e5"
            }];
        } else {
            current = JSON.parse(current);
        }
        if (current.length > 0) {
            for (var i = 0; i < current.length; i++) {
                if (current[i].NameId === id.toString()) {
                    current.splice(i, 1);
                    break;
                }
                if (i == current.length - 1) {
                    current.push({
                        NameClass: name,
                        NameId: id,
                        Value: value
                    });
                    break;
                }
            }
        }
        else {
            current.push({
                NameClass: name,
                NameId: id,
                Value: value
            });
        }
        localStorage.setItem('QLkhuyenmai', JSON.stringify(current));
    }
    $("#e0").click(function () {
        $(".m0").toggle();
        addClass(".m0", "e0", $(this).val());
    });
    $("#e1").click(function () {
        $(".m1").toggle();
        addClass(".m1", "e1", $(this).val());
    });
    $("#e2").click(function () {
        $(".m2").toggle();
        addClass(".m2", "e2", $(this).val());
    });
    $("#e3").click(function () {
        $(".m3").toggle();
        addClass(".m3", "e3", $(this).val());
    });
    $("#e4").click(function () {
        $(".m4").toggle();
        addClass(".m4", "e4", $(this).val());
    });
    $("#e5").click(function () {
        $(".m5").toggle();
        addClass(".m5", "e5", $(this).val());
    });
    $("#e6").click(function () {
        $(".m6").toggle();
        addClass(".m6", "e6", $(this).val());
    });
    $("#e7").click(function () {
        $(".m7").toggle();
        addClass(".m7", "e7", $(this).val());
    });

    self.ddlKM_byNumber = ko.observableArray([{ ID: 1, Text: 'Giá bán' }, { ID: 2, Text: 'Giảm giá' }]);

    self.ChangeType_24 = function (item) {
        var $this = event.currentTarget;
        var divNVD = $($this).parent().next().next();
        $($this).parent().next().val(0);// reset value for input
        var type = $($this).val();

        var idRandom = item.IDRandom;
        if (type === '2') {
            $(divNVD).show();
            $(divNVD).addClass('active-re');
            for (var i = 0; i < self.ThemHangHoa().length; i++) {
                if (self.ThemHangHoa()[i].IDRandom === idRandom) {
                    self.ThemHangHoa()[i].GiaKhuyenMai = 0;
                    self.ThemHangHoa()[i].GiamGia = 0;
                    self.ThemHangHoa()[i].GiamGiaTheoPhanTram = true;
                    break;
                }
            }
        }
        else {
            $(divNVD).hide();
            for (var i = 0; i < self.ThemHangHoa().length; i++) {
                if (self.ThemHangHoa()[i].IDRandom === idRandom) {
                    self.ThemHangHoa()[i].GiaKhuyenMai = 0;
                    self.ThemHangHoa()[i].GiamGia = null;
                    self.ThemHangHoa()[i].GiamGiaTheoPhanTram = null;
                    break;
                }
            }
        }
    }

    self.ClickVND_24 = function (item) {
        var $this = event.currentTarget;
        $($this).prev().val(0);// reset input
        var idRandom = item.IDRandom;

        var isPTram = true;
        if ($($this).hasClass('active-re')) {// dang vnd: khong co class active
            isPTram = false;
        }
        //else {
        //    isPTram = true;
        //}

        // vnd --> %: not active-re --> have active-re

        for (var i = 0; i < self.ThemHangHoa().length; i++) {
            if (self.ThemHangHoa()[i].IDRandom === idRandom) {
                self.ThemHangHoa()[i].GiamGiaTheoPhanTram = isPTram;
                self.ThemHangHoa()[i].GiamGia = 0;
                break;
            }
        }
    }

    self.editGiaKhuyenMai = function (item) {
        var idRandom = item.IDRandom;
        var $this = event.currentTarget;
        var gtri = $($this).val();

        for (var i = 0; i < self.ThemHangHoa().length; i++) {
            if (self.ThemHangHoa()[i].IDRandom === idRandom) {
                if (self.ThemHangHoa()[i].GiamGia == null) {// 2 dau ==
                    formatNumberObj($this);
                    self.ThemHangHoa()[i].GiaKhuyenMai = formatNumberToInt(gtri);
                    self.ThemHangHoa()[i].GiamGiaTheoPhanTram = null;
                }
                else {
                    if (self.ThemHangHoa()[i].GiamGiaTheoPhanTram) {
                        if (formatNumberToFloat(gtri) > 100) {
                            gtri = 100;
                            $($this).val(100);
                        }
                        self.ThemHangHoa()[i].GiamGia = gtri;
                    }
                    else {
                        formatNumberObj($this);
                        self.ThemHangHoa()[i].GiamGia = formatNumberToInt(gtri);
                    }
                    self.ThemHangHoa()[i].GiaKhuyenMai = 0;
                }
                break;
            }
        }
    }
}
//ko.applyBindings(new ViewModel());
var vmKhuyenMai = new ViewModel();
ko.applyBindings(vmKhuyenMai);

//var vmXuatHuy = new ViewModel();
//ko.applyBindings(vmXuatHuy);
function keypressEnterSelected(e) {
    if (e.keyCode == 13) {
        vmKhuyenMai.SelectedHHEnterkey();
    }
}
function itemSelected(item) {
    vmKhuyenMai.getChiTietHangHoaByMaHH_byItem(item);
}

function getNumber(e, obj) {
    var elementAfer = $(obj).next();
    if (elementAfer.css('display') == 'none') {
        return keypressNumber(e);
    }
    else {
        if (elementAfer.hasClass('active-re')) {// %
            var keyCode = window.event.keyCode || e.which;
            if (keyCode < 48 || keyCode > 57) {
                // cho phep nhap dau .
                if (keyCode === 8 || keyCode === 127 || keyCode === 46) {
                    return;
                }
                return false;
            }
        }
        else {
            // chi cho phep nhap so
            return keypressNumber(e);
        }
    }
}
