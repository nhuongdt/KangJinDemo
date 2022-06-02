ViewModelTongQuan = function () {
    var self = this;
    self.DoanhThuTT = ko.observableArray();
    self.DoanhThuToDay = ko.observableArray();
    self.ThucThuToDay = ko.observableArray();
    var BH_DoanhThuUri = '/api/DanhMuc/TQ_DoanhThuAPI/';
    var BH_DonViUri = '/api/DanhMuc/DM_DonViAPI/';
    var _IDchinhanh = $('#hd_IDdDonVi').val(); // lấy ID chi nhánh _header.cshtml
    self.ID_DonViSeach = ko.observable(_IDchinhanh);
    self.ID_DonViSeachThucThu = ko.observable(_IDchinhanh);
    var _useDangNhap = $('#txtTenTaiKhoan').text();
    self.useDangNhap = ko.observable(_useDangNhap);
    var _id_NhanVien = $('.idnhanvien').text();
    var _IDNhomNguoiDung = $('.idnhomnguoidung').text();
    var _IDNguoiDung = $('.idnguoidung').text();
    var ReportUri = '/api/DanhMuc/ReportAPI/';
    var _IDDoiTuong = $('.idnguoidung').text();
    var _loadHangHoa = "'";
    var _data;
    var _loadDate;
    var _loadDateThucThu;
    var _dataDS;
    var _MauSoDVT
    var _NgayDoanhSo = "Tháng này";
    var _NgayThucThu = "Tháng này";
    var _NgayDoanhSo_ChiNhanh = "Tháng này";
    self.NgayLoadDoanhSo = ko.observable("Tháng này");
    self.NgayLoadThucThu = ko.observable("Tháng này");
    self.NgayLoadDoanhSo_ChiNhanh = ko.observable("Tháng này");
    self.NgayBaoCao = ko.observable("Tháng này");
    self.DiaryBH = ko.observableArray();
    var _NgayLoadBieu = "Tháng này";
    var _LoaiBieu = "Theo doanh thu";
    var timeStart;
    var timeEnd;
    var datime1 = new Date();
    var datime_DR = new Date();
    self.TongDoanhSo = ko.observable('0');
    self.TongThucThu = ko.observable('0');
    self.DonViTinh = ko.observable();
    self.DonViTinhDS = ko.observable();
    timeStart = moment(new Date(datime1.getFullYear(), datime1.getMonth(), 1)).format('YYYY-MM-DD');
    timeEnd = moment(new Date(datime1.getFullYear(), datime1.getMonth() + 1, 1)).format('YYYY-MM-DD');
    var timeStart_CN = moment(new Date(datime1.getFullYear(), datime1.getMonth(), 1)).format('YYYY-MM-DD');
    var timeEnd_CN = moment(new Date(datime1.getFullYear(), datime1.getMonth() + 1, 1)).format('YYYY-MM-DD');

    self.IsShowTienThu = ko.observable(true);
    self.ResultHD = ko.observableArray(0);
    self.ResultDoanhSo = ko.observableArray();
    self._ThanhTien = ko.observable(0);// hoadon
    self._DoanhThu = ko.observable(0);
    self._SoLuongHDBan = ko.observable(0);
    self.GDV_SoLuongBan = ko.observable(0);
    self.GDV_ThanhTien = ko.observable(0);
    self.TongThu_CongNo = ko.observable(0);
    self.Role_GoiDichVu = ko.observable(false);
    self._SoLuongHDTra = ko.observable(0);
    self.HD_CungKy = ko.observable(0);
    self.Thu_CungKy = ko.observable(0);
    self.Chi_CungKy = ko.observable(0);
    self.HD_TangGiam = ko.observable();
    self.KH_GiaoDichLanDau = ko.observable();
    self.KH_TaoMoi = ko.observable();
    self.TQThu_TienThu = ko.observable();
    self.TQThu_TienMat = ko.observable();
    self.TQThu_NganHang = ko.observable();
    self.TQChi_TienThu = ko.observable();
    self.TQChi_TienMat = ko.observable();
    self.TQChi_NganHang = ko.observable();
    self.KH_QuayLai = ko.observable();
    self.KH_Tong = ko.observable();
    self.KH_CungKy = ko.observable(0);
    self.KH_TangGiam = ko.observable();
    self.nameNhanVien = ko.observable();
    self.TongQuan_XemDS_PhongBan = ko.observable();
    self.TongQuan_XemDS_HeThong = ko.observable();

    self.ResultPhieuTra = ko.observableArray();
    self.ResultMoneyPT = ko.observableArray();
    self._TraHang = ko.observable(0);
    self.Select_DangBieu = ko.observable('1');
    self.nameDay = ko.observable();
    self.ToDay = ko.observable('hôm nay');
    var datime2 = new Date();
    var timeStart2 = datime2.getFullYear() + "-" + (datime2.getMonth() + 1) + "-" + datime2.getDate();
    var timeEnd2 = moment(new Date(datime2.setDate(datime2.getDate() + 1))).format('YYYY-MM-DD');
    // Khai báo đối tượng Date
    var dateTD = new Date();
    // self.ToDay(moment(dateTD).format('DD/MM/YYYY'))

    // Lấy số thứ tự của ngày hiện tại
    var current_day = dateTD.getDay();
    // Lấy tên thứ của ngày hiện tại
    function loadHello() {
        switch (current_day) {
            case 0:
                self.nameDay("Chủ nhật");
                break;
            case 1:
                self.nameDay("Thứ 2");
                break;
            case 2:
                self.nameDay("Thứ 3");
                break;
            case 3:
                self.nameDay("Thứ 4");
                break;
            case 4:
                self.nameDay("Thứ 5");
                break;
            case 5:
                self.nameDay("Thứ 6");
                break;
            case 6:
                self.nameDay("Thứ 7");
        }
    }
    loadHello();
    // load danh mục đơn vị
    var _nameDonViSeach = null;
    self.DonVis = ko.observableArray(0);
    self.numberDonVi = ko.observable();
    self.numberDonViThucThu = ko.observable();
    self.nameSelectDonViDT = ko.observable('Chi nhánh đã chọn');
    self.nameSelectDonViTT = ko.observable('Chi nhánh đã chọn');
    self.searchDonVi = ko.observableArray();
    self.DonViThucThu = ko.observableArray(0);
    self.searchDonViThucThu = ko.observableArray(0);

    self.Top10_LoaiBieuDo = ko.observable(1);
    self.Top10_ListAllDonVi = ko.observableArray();
    self.Top10_ArrFilterDonVi = ko.observableArray();
    self.Top10_showDV = ko.observable(false);
    self.Top10_txtSearch = ko.observable();

    function getDonVi() {
        ajaxHelper(BH_DonViUri + "GetDonVi_byUserSearch?ID_NguoiDung=" + _IDDoiTuong + "&TenDonVi=" + _nameDonViSeach, "GET").done(function (data) {
            data = data.map(p => ({
                ID: p.ID,
                TenDonVi: p.TenDonVi,
                SoDienThoai: p.SoDienThoai,
                checkSearch: _IDchinhanh === p.ID ? 1 : 0
            }));
            self.DonVis(data);
            self.searchDonVi(data);
            self.DonViThucThu(data);
            self.searchDonViThucThu(data);
            self.numberDonVi(self.DonVis().length);
            self.numberDonViThucThu(self.DonViThucThu().length);
            if (self.DonVis().length < 2)
                $('.showChiNhanh').hide();
            else
                $('.showChiNhanh').show();


            let obj = {
                ID: null,
                TenDonVi: 'Tất cả',
                SoDienThoai: '',
                checkSearch: 0,
            }
            self.Top10_ListAllDonVi(data);
            self.Top10_ListAllDonVi.unshift(obj);
            self.Top10_ArrFilterDonVi(self.Top10_ListAllDonVi());
            self.Top10_showDV(data.length > 1);

            self.ID_DonViSeach(self.DonVis().filter(p => p.checkSearch === 1).map(p => p.ID).toString());
            self.ID_DonViSeachThucThu(self.ID_DonViSeach());
            loadQuyenIndex();
        });
    }
    getDonVi()
    //lọc đơn vị
    self.NoteNameDonViDoanhThu = function () {
        var arrDonVi = [];
        var itemSearch = locdau($('#NoteNameDonVi').val().toLowerCase());
        for (var i = 0; i < self.searchDonVi().length; i++) {
            var locdauInput = locdau(self.searchDonVi()[i].TenDonVi).toLowerCase();
            var R = locdauInput.split(itemSearch);
            if (R.length > 1) {
                arrDonVi.push(self.searchDonVi()[i]);
            }
        }
        self.DonVis(arrDonVi);
        if ($('#NoteNameDonVi').val() == "") {
            self.DonVis(self.searchDonVi());
        }
    }
    self.value_DoanhThu = function (item) {
        var dk = item.checkSearch;
        if (dk) {
            item.checkSearch = 1;
            var idsearch = self.ID_DonViSeach() + ',' + item.ID;
            self.ID_DonViSeach(idsearch);
            self.numberDonVi(self.numberDonVi() + 1);
            if (self.numberDonVi() == self.DonVis().length)
                self.nameSelectDonViDT('Toàn bộ chi nhánh');
        }
        else {
            item.checkSearch = 0;
            var res = self.ID_DonViSeach().replace(item.ID + ',', "");
            res = self.ID_DonViSeach().replace(',' + item.ID, "");
            self.ID_DonViSeach(res);
            self.numberDonVi(self.numberDonVi() - 1);
            self.nameSelectDonViDT('Chi nhánh đã chọn');
        }
        self.getBieuDoanhSo();
    }
    //lọc đơn vị
    self.NoteNameDonViThucThu = function () {
        var arrDonVi = [];
        var itemSearch = locdau($('#NoteNameDonViThucThu').val().toLowerCase());
        for (var i = 0; i < self.searchDonViThucThu().length; i++) {
            var locdauInput = locdau(self.searchDonViThucThu()[i].TenDonVi).toLowerCase();
            var R = locdauInput.split(itemSearch);
            if (R.length > 1) {
                arrDonVi.push(self.searchDonViThucThu()[i]);
            }
        }
        self.DonViThucThu(arrDonVi);
        if ($('#NoteNameDonViThucThu').val() == "") {
            self.DonViThucThu(self.searchDonViThucThu());
        }
    }
    self.value_ThucThu = function (item) {
        var dk = item.checkSearch;
        if (dk) {
            item.checkSearch = 1;
            var idsearch = self.ID_DonViSeachThucThu() + ',' + item.ID;
            self.ID_DonViSeachThucThu(idsearch);
            self.numberDonViThucThu(self.numberDonViThucThu() + 1);
            if (self.numberDonViThucThu() == self.DonVis().length)
                self.nameSelectDonViTT('Toàn bộ chi nhánh');
        }
        else {
            item.checkSearch = 0;
            var res = self.ID_DonViSeachThucThu().replace(item.ID + ',', "");
            res = self.ID_DonViSeachThucThu().replace(',' + item.ID, "");
            self.ID_DonViSeachThucThu(res);
            self.numberDonViThucThu(self.numberDonViThucThu() - 1);
            self.nameSelectDonViTT('Chi nhánh đã chọn');
        }
        self.getBieuThucThu();
    }
    var nameChar;
    function ajaxHelper(uri, method, data) {
        return $.ajax({
            type: method,
            url: uri,
            dataType: 'json',
            contentType: 'application/json',
            data: data ? JSON.stringify(data) : null,
            statusCode: {
                404: function () {
                },
            }
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
            });
    }

    var timer1 = null; var pastime = 0;
    $('#MainLayout').on('timersuccess', function () {
        getDiary();
        getList_SuKienToDay();
    });
    self.CauHinhHeThong = ko.observable();
    function loadQuyenIndex() {
        var arrQuyen = [];
        ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetHT_NhomNguoiDung?idnguoidung=" + _IDNguoiDung + '&iddonvi=' + _IDchinhanh, 'GET').done(function (data) {
            if (data.HT_Quyen_NhomDTO.length > 0) {
                for (var i = 0; i < data.HT_Quyen_NhomDTO.length; i++) {
                    arrQuyen.push(data.HT_Quyen_NhomDTO[i].MaQuyen);
                }
            }
            localStorage.setItem('lc_CTQuyen', JSON.stringify(arrQuyen));
            if ($.inArray('TongQuan', arrQuyen) > -1) {
                $('.tongquan').css('display', 'block');
            }
            pastime = pastime + 1;
            if (pastime === 3) {
                pastime = 0;
                $('#MainLayout').trigger('timersuccess');
            }
            self.getDoanhThuToday();
            self.getThucThuToday();
            getDMDoanhThuTT();
            self.ResultToday();
        });
        //quyền xem hệ thống
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _IDchinhanh + "&MaQuyen=" + "TongQuan_XemDS_HeThong", "GET").done(function (data) {
            self.TongQuan_XemDS_HeThong(data);
            pastime = pastime + 1;
            if (pastime === 3) {
                pastime = 0;
                $('#MainLayout').trigger('timersuccess');
            }
        })
        // quyền xem phòng ban
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _IDchinhanh + "&MaQuyen=" + "TongQuan_XemDS_PhongBan", "GET").done(function (data) {
            self.TongQuan_XemDS_PhongBan(data);
            pastime = pastime + 1;
            if (pastime === 3) {
                pastime = 0;
                $('#MainLayout').trigger('timersuccess');
            }
        })

        ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/' + 'GetCauHinhHeThong/' + _IDchinhanh, 'GET').done(function (data) {
            localStorage.setItem('lc_CTThietLap', JSON.stringify(data));
            var CauHinhHeThongPM = JSON.parse(localStorage.getItem("lc_CTThietLap"));
            self.CauHinhHeThong(CauHinhHeThongPM.LoHang);
        });

    }



    //var nuberChiNhanh = $('#divLstChiNhanh ul li').length;
    //if (nuberChiNhanh > 1)
    //{ $('.showDoanhThuChiNhanh').show(); }
    //else
    //{
    //    $('.showDoanhThuChiNhanh').hide();
    //}


    $('#panel-contentChiNhanh_Column').hide();
    $('.rdo_checkDangBieu li').on('click', function () {
        self.Select_DangBieu($(this).val());
        dkChart = $(this).val();
        if ($(this).val() == 1) {
            self.getDoanhThuChiNhanh_Pie();
            $('#panel-contentChiNhanh_Column').hide();
            $('#panel-contentChiNhanh').show();
        }
        else {
            self.getDoanhThuChiNhanh_Column();
            $('#panel-contentChiNhanh').hide();
            $('#panel-contentChiNhanh_Column').show();
        }
    });

    $('.getDateDoanhSo a').on('click', function () {
        _NgayDoanhSo = $(this).text();
        self.NgayLoadDoanhSo(_NgayDoanhSo);
        self.getBieuDoanhSo();
        //$('#doimau').addClass("yellow");
    });
    $('.getDateThucThu a').on('click', function () {
        _NgayThucThu = $(this).text();
        self.NgayLoadThucThu(_NgayThucThu);
        self.getBieuThucThu();
        //$('#doimau').addClass("yellow");
    });
    $('.getDateDoanhSo_ChiNhanh a').on('click', function () {
        _NgayDoanhSo_ChiNhanh = $(this).text();
        self.NgayLoadDoanhSo_ChiNhanh(_NgayDoanhSo_ChiNhanh);
        self.getBieuDoanhSo_ChiNhanh();
        //$('#doimau').addClass("yellow");
    });
    $('.choseNgayTaoBieu a').on('click', function () {
        _NgayLoadBieu = $(this).text();
        self.NgayBaoCao(_NgayLoadBieu);
        self.getBieuHangHoa();
    });
    $('.choseLoaiBieu a').on('click', function () {
        _LoaiBieu = $(this).text().trim();
        let val = $(this).parent().attr('value');
        self.Top10_LoaiBieuDo(val)
        self.getBieuHangHoa();
    });

    self.Top10_SearchDonVi = function () {
        let txt = self.Top10_txtSearch();
        let arr = [];
        if (commonStatisJs.CheckNull(txt)) {
            arr = self.Top10_ListAllDonVi();
        }
        txt = locdau(txt);
        arr = $.grep(self.Top10_ListAllDonVi(), function (x) {
            return locdau(x.TenDonVi).indexOf(txt) > -1;
        })
        self.Top10_ArrFilterDonVi(arr);
    }

    self.Top10_ChoseChiNhanh = function (item) {
        let $this = $(event.currentTarget);
        let isCheck = $this.is(':checked');

        if (item.ID === null) {
            $this.closest('ul').find('li input').prop('checked', isCheck);

            isCheck = isCheck ? 1 : 0;
            for (let i = 0; i < self.Top10_ListAllDonVi().length; i++) {
                let itFor = self.Top10_ListAllDonVi()[i];
                if (itFor.ID !== null) {
                    self.Top10_ListAllDonVi()[i].checkSearch = isCheck;
                }
            }
        }
        else {
            for (let i = 0; i < self.Top10_ListAllDonVi().length; i++) {
                let itFor = self.Top10_ListAllDonVi()[i];
                if (itFor.ID === item.ID) {
                    self.Top10_ListAllDonVi()[i].checkSearch = isCheck ? 1 : 0;
                    break;
                }
            }
            let arrChosed = self.Top10_ListAllDonVi().filter(x => x.checkSearch === 1 && x.ID != null);
            let isCheckAll = arrChosed.length === self.Top10_ListAllDonVi().length - 1;
            self.Top10_ListAllDonVi()[0].checkSearch = isCheckAll ? 1 : 0;
            $this.closest('ul').find('li:eq(0) input').prop('checked', isCheckAll);
        }

        if (parseInt(self.Top10_LoaiBieuDo()) === 1) {
            getDMDoanhThuTT();
        }
        else {
            self.getDMDoanhThuSL();
        }
    }

    self.ChangeShowThuChi = function () {
        self.IsShowTienThu(!self.IsShowTienThu());
    }
    //tóm tắt tình hình kinh doanh trong ngày
    self.ResultToday = function () {
        ajaxHelper(BH_DoanhThuUri + "TongQuan_DoanhThuToDay?dayStart=" + timeStart2 + "&dayEnd=" + timeEnd2 + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
            //self.ResultDoanhSo(data.LstData);
            if (data.LstData.length > 0) {
                self._SoLuongHDBan(data.LstData[0].HD_SoLuongBan);
                self._ThanhTien(data.LstData[0].HD_ThanhTien);
                self.GDV_SoLuongBan(data.LstData[0].GDV_SoLuongBan);
                self.GDV_ThanhTien(data.LstData[0].GDV_ThanhTien);
                self._TraHang(data.LstData[0].GiaTriTra);
                self._DoanhThu(data.LstData[0].DoanhThuThangNay);
                self._SoLuongHDTra(data.LstData[0].SoLuongTra);
                if (data.LstData[0].SoSanhCungKy >= 0) {
                    self.HD_CungKy(data.LstData[0].SoSanhCungKy);
                    $('.HD_CungKyGiam').hide();
                    $('.HD_CungKyTang').show();
                }
                else {
                    self.HD_CungKy(data.LstData[0].SoSanhCungKy * (-1));
                    $('.HD_CungKyGiam').show();
                    $('.HD_CungKyTang').hide();
                }
            }
            else {
                self._ThanhTien(0);
                self._TraHang(0);
                self._DoanhThu(0);
                self._SoLuongHDBan(0);
                self._SoLuongHDTra(0);
                self.GDV_SoLuongBan(0);
                self.GDV_ThanhTien(0);
                $('.HD_CungKyGiam').hide();
                $('.HD_CungKyTang').hide();
            }
        });

        ajaxHelper(BH_DoanhThuUri + "getNameNhanVien?ID_NhanVien=" + _id_NhanVien, "GET").done(function (data) {
            self.nameNhanVien(data);
        });

        ajaxHelper(BH_DoanhThuUri + "getList_SoSanhCungKyKhachHang?dayStart=" + timeStart2 + "&dayEnd=" + timeEnd2 + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
            if (data.LstData.length > 0) {
                if (data.LstData[0].SoSanhCungKy >= 0) {
                    self.KH_CungKy(data.LstData[0].SoSanhCungKy);
                    //self.KH_TangGiam("Tăng")
                    $('.KH_CungKyGiam').hide();
                    $('.KH_CungKyTang').show();
                }
                else {
                    self.KH_CungKy(data.LstData[0].SoSanhCungKy * (-1));
                    //self.KH_TangGiam("Giảm")
                    $('.KH_CungKyGiam').show();
                    $('.KH_CungKyTang').hide();
                }
            }
            else {
                $('.KH_CungKyGiam').hide();
                $('.KH_CungKyTang').hide();
            }
        });
        ajaxHelper(BH_DoanhThuUri + "getList_TongQuanKhachHang?dayStart=" + timeStart2 + "&dayEnd=" + timeEnd2 + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
            self.KH_GiaoDichLanDau(data.LstData[0].KhachHangGiaoDichLanDau);
            self.KH_QuayLai(data.LstData[0].KhachHangQuayLai);
            self.KH_TaoMoi(data.LstData[0].KhachHangTaoMoi);
            self.KH_Tong(self.KH_GiaoDichLanDau() + self.KH_QuayLai());
        });
        ajaxHelper(BH_DoanhThuUri + "getList_TongQuanThuChi?dayStart=" + timeStart2 + "&dayEnd=" + timeEnd2 + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
            if (data.LstData.length > 0) {
                self.TQThu_TienThu(data.LstData[0].TienThu_Thu);
                self.TQThu_TienMat(data.LstData[0].TienMat_Thu);
                self.TQThu_NganHang(data.LstData[0].NganHang_Thu);
                self.TQChi_TienThu(data.LstData[0].TienThu_Chi);
                self.TQChi_TienMat(data.LstData[0].TienMat_Chi);
                self.TQChi_NganHang(data.LstData[0].NganHang_Chi);
                self.TongThu_CongNo(data.LstData[0].ThuNo_Tong);
            }
            else {
                self.TQThu_TienThu(0);
                self.TQThu_TienMat(0);
                self.TQThu_NganHang(0);
                self.TQChi_TienThu(0);
                self.TQChi_TienMat(0);
                self.TQChi_NganHang(0);
                self.TongThu_CongNo(0);
            }
        });
        ajaxHelper(BH_DoanhThuUri + "getList_SoSanhCungKyThuChi?dayStart=" + timeStart2 + "&dayEnd=" + timeEnd2 + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
            if (data.LstData.length > 0) {
                if (data.LstData[0].ThuCungKy >= 0) {
                    self.Thu_CungKy(data.LstData[0].ThuCungKy);
                    //self.KH_TangGiam("Tăng")
                    $('.Thu_CungKyGiam').hide();
                    $('.Thu_CungKyTang').show();
                }
                else {
                    self.Thu_CungKy(data.LstData[0].ThuCungKy * (-1));
                    //self.KH_TangGiam("Giảm")
                    $('.Thu_CungKyGiam').show();
                    $('.Thu_CungKyTang').hide();
                }
                if (data.LstData[0].ChiCungKy >= 0) {
                    self.Chi_CungKy(data.LstData[0].ChiCungKy);
                    //self.KH_TangGiam("Tăng")
                    $('.Chi_CungKyGiam').hide();
                    $('.Chi_CungKyTang').show();
                }
                else {
                    self.Chi_CungKy(data.LstData[0].ChiCungKy * (-1));
                    //self.KH_TangGiam("Giảm")
                    $('.Chi_CungKyGiam').show();
                    $('.Chi_CungKyTang').hide();
                }
            }
            else {
                $('.Thu_CungKyGiam').hide();
                $('.Thu_CungKyTang').hide();
                $('.Chi_CungKyGiam').hide();
                $('.Chi_CungKyTang').hide();
            }
        });
    }
    self.getBieuThucThu = function () {
        var datime = new Date();
        if (_NgayThucThu === "Hôm nay") {
            timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
            timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            // self.getDoanhThuToday();
            self.getThucThuToHour();
        }
        else if (_NgayThucThu === "Hôm qua") {
            var dt1 = new Date();
            var dt2 = new Date();
            timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
            timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
            //self.getDoanhThuToday();
            self.getThucThuToHour();
        }
        else if (_NgayThucThu.trim() === "Tháng này") {
            timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
            timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
            self.getThucThuToday();
        }
        else if (_NgayThucThu === "Tháng trước") {
            timeStart = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
            timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
            self.getThucThuToday();
        }
    }
    self.getBieuDoanhSo = function () {
        var datime = new Date();
        if (_NgayDoanhSo === "Hôm nay") {
            timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
            timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            // self.getDoanhThuToday();
            self.getDoanhThuToHour();
        }
        else if (_NgayDoanhSo === "Hôm qua") {
            var dt1 = new Date();
            var dt2 = new Date();
            timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
            timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
            //self.getDoanhThuToday();
            self.getDoanhThuToHour();
        }
        else if (_NgayDoanhSo.trim() === "Tháng này") {
            timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
            timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
            self.getDoanhThuToday();
        }
        else if (_NgayDoanhSo === "Tháng trước") {
            timeStart = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
            timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
            self.getDoanhThuToday();
        }
    }
    var dkChart = 1;
    self.getBieuDoanhSo_ChiNhanh = function () {
        var datime = new Date();
        if (_NgayDoanhSo_ChiNhanh === "Hôm nay") {
            timeStart_CN = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
            timeEnd_CN = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            if (dkChart == 1)
                self.getDoanhThuChiNhanh_Pie();
            else
                self.getDoanhThuChiNhanh_Column();
        }
        else if (_NgayDoanhSo_ChiNhanh === "Hôm qua") {
            var dt1 = new Date();
            var dt2 = new Date();
            timeStart_CN = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
            timeEnd_CN = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
            if (dkChart == 1)
                self.getDoanhThuChiNhanh_Pie();
            else
                self.getDoanhThuChiNhanh_Column();
        }
        else if (_NgayDoanhSo_ChiNhanh.trim() === "Tháng này") {
            timeStart_CN = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
            timeEnd_CN = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
            if (dkChart == 1)
                self.getDoanhThuChiNhanh_Pie();
            else
                self.getDoanhThuChiNhanh_Column();
        }
        else if (_NgayDoanhSo_ChiNhanh === "Tháng trước") {
            timeStart_CN = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
            timeEnd_CN = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
            if (dkChart == 1)
                self.getDoanhThuChiNhanh_Pie();
            else
                self.getDoanhThuChiNhanh_Column();
        }
    }

    self.data_DoanhSo = ko.observableArray();
    self.dvt_Doanhso = ko.observable();
    self.date_Doanhso = ko.observable();
    self.arrNT = ko.observableArray();
    self.arrDS = ko.observableArray();
    self.DonViTinh_DoanhSo = ko.observable();
    self.DonViTinh_ThucThu = ko.observable();
    self.data_ThucThu = ko.observableArray();
    self.arrTT = ko.observableArray();
    self.arrDSTT = ko.observableArray();
    self.dvt_ThucThu = ko.observable();
    self.date_ThucThu = ko.observable();
    self.getDoanhThuToday = function () {
        self.date_Doanhso("Ngày: ");
        self.arrNT([]);
        self.arrDS([]);
        self.data_DoanhSo([])
        var array_Seach = {
            dayStart: timeStart,
            dayEnd: timeEnd,
            ID_NguoiDung: _IDNguoiDung,
            ID_DonVi: self.ID_DonViSeach()
        }
        ajaxHelper(BH_DoanhThuUri + "TongQuan_BieuDoDoanhThuToDay", "POST", array_Seach).done(function (data) {
            //ajaxHelper(BH_DoanhThuUri + "TongQuan_BieuDoDoanhThuToDay?dayStart=" + timeStart + "&dayEnd=" + timeEnd + "&ID_NguoiDung=" + _IDNguoiDung + "&ID_DonVi=" + self.ID_DonViSeach(), "GET").done(function (data) {
            self.DoanhThuToDay(data.LstData);
            var DVTDS
            var dvt = 0;
            var TongTienDS = 0;
            for (var i = 0; i < self.DoanhThuToDay().length; i++) {
                if (dvt < self.DoanhThuToDay()[i].ThanhTien) {
                    dvt = self.DoanhThuToDay()[i].ThanhTien;
                }
                TongTienDS = TongTienDS + self.DoanhThuToDay()[i].ThanhTien;
            }
            self.TongDoanhSo(TongTienDS);
            if (dvt >= 1000000000) {
                self.DonViTinh_DoanhSo(1000000000)
                self.dvt_Doanhso(" tỷ");
            }
            if (dvt >= 1000000 & dvt < 1000000000) {
                self.DonViTinh_DoanhSo(1000000)
                self.dvt_Doanhso(" tr");
            }
            if (dvt >= 1000 & dvt < 1000000) {
                self.DonViTinh_DoanhSo(1000)
                self.dvt_Doanhso(" k");
            }
            for (var i = 0; i < data.LstChiNhanh.length; i++) {
                for (var j = 0; j < data.LstDate.length; j++) {
                    for (var k = 0; k < data.LstData.length; k++) {
                        if (data.LstData[k].TenChiNhanh == data.LstChiNhanh[i].TenChiNhanh && data.LstData[k].NgayLapHoaDon == data.LstDate[j].NgayLapHoaDon) {
                            self.arrDS.push(data.LstData[k].ThanhTien);
                            break;
                        }
                        if (k == data.LstData.length - 1) {
                            self.arrDS.push(0);
                        }
                    }
                }
                var obj = {
                    //name: data.LstChiNhanh[i].TenChiNhanh,
                    name: data.LstChiNhanh[i].TenChiNhanh + '<br> <span class="thucthu">' + Highcharts.numberFormat(data.LstChiNhanh[i].ThanhTien, 0, '.', ',') + '</span>',
                    turboThreshold: i,
                    _colorIndex: i,
                    data: self.arrDS()
                }
                self.data_DoanhSo.push(obj);
                self.arrDS([]);
            }
            for (var i = 0; i < data.LstDate.length; i++) {
                if (data.LstDate[i].NgayLapHoaDon.length == 1) {
                    _loadDate = "0" + data.LstDate[i].NgayLapHoaDon;
                }
                else {
                    _loadDate = data.LstDate[i].NgayLapHoaDon;
                }
                self.arrNT.push(_loadDate);
            }
            self.loadDoanhSo();
        });
    }
    self.getThucThuToday = function () {
        self.date_ThucThu("Ngày: ");
        self.arrTT([]);
        self.arrDSTT([]);
        self.data_ThucThu([])
        var array_Seach = {
            dayStart: timeStart,
            dayEnd: timeEnd,
            ID_NguoiDung: _IDNguoiDung,
            ID_DonVi: self.ID_DonViSeachThucThu()
        }
        ajaxHelper(BH_DoanhThuUri + "TongQuan_BieuDoThucThuToDay", "POST", array_Seach).done(function (data) {
            //ajaxHelper(BH_DoanhThuUri + "TongQuan_BieuDoThucThuToDay?dayStart=" + timeStart + "&dayEnd=" + timeEnd + "&ID_NguoiDung=" + _IDNguoiDung + "&ID_DonVi=" + self.ID_DonViSeachThucThu(), "GET").done(function (data) {
            self.ThucThuToDay(data.LstData);
            var DVTDS
            var dvt = 0;
            var TongTienDS = 0;
            for (var i = 0; i < self.ThucThuToDay().length; i++) {
                if (dvt < self.ThucThuToDay()[i].ThanhTien) {
                    dvt = self.ThucThuToDay()[i].ThanhTien;
                }
                TongTienDS = TongTienDS + self.ThucThuToDay()[i].ThanhTien;
            }
            self.TongThucThu(TongTienDS);
            if (dvt >= 1000000000) {
                self.DonViTinh_ThucThu(1000000000)
                self.dvt_ThucThu(" tỷ");
            }
            if (dvt >= 1000000 & dvt < 1000000000) {
                self.DonViTinh_ThucThu(1000000)
                self.dvt_ThucThu(" tr");
            }
            if (dvt >= 1000 & dvt < 1000000) {
                self.DonViTinh_ThucThu(1000)
                self.dvt_ThucThu(" k");
            }
            for (var i = 0; i < data.LstChiNhanh.length; i++) {
                for (var j = 0; j < data.LstDate.length; j++) {
                    for (var k = 0; k < data.LstData.length; k++) {
                        if (data.LstData[k].TenChiNhanh == data.LstChiNhanh[i].TenChiNhanh && data.LstData[k].NgayLapHoaDon == data.LstDate[j].NgayLapHoaDon) {
                            self.arrDSTT.push(data.LstData[k].ThanhTien);
                            break;
                        }
                        if (k == data.LstData.length - 1) {
                            self.arrDSTT.push(0);
                        }
                    }
                }
                var obj = {
                    name: data.LstChiNhanh[i].TenChiNhanh + '<br> <span class="thucthu">' + Highcharts.numberFormat(data.LstChiNhanh[i].ThanhTien, 0, '.', ',') + '</span>',
                    turboThreshold: i,
                    _colorIndex: i,
                    data: self.arrDSTT()
                }
                self.data_ThucThu.push(obj);
                self.arrDSTT([]);
            }
            for (var i = 0; i < data.LstDate.length; i++) {
                if (data.LstDate[i].NgayLapHoaDon.length == 1) {
                    _loadDateThucThu = "0" + data.LstDate[i].NgayLapHoaDon;
                }
                else {
                    _loadDateThucThu = data.LstDate[i].NgayLapHoaDon;
                }
                self.arrTT.push(_loadDateThucThu);
            }
            self.loadThucThu();
        });
    }
    self.pie_ChiNhanh = ko.observableArray();
    self.TongTienHang_ChiNhanh = ko.observableArray();
    self.getDoanhThuChiNhanh_Pie = function () {
        self.pie_ChiNhanh([]);
        self.TongTienHang_ChiNhanh([]);
        ajaxHelper(BH_DoanhThuUri + "getDoanhThu_PieChiNhanh?dayStart=" + timeStart_CN + "&dayEnd=" + timeEnd_CN + "&ID_NguoiDung=" + _IDNguoiDung, "GET").done(function (data) {
            for (var i = 0; i < data.LstData.length; i++) {
                data.LstData[i].name = data.LstData[i].name + '<br> <span class="thucthu">' + Highcharts.numberFormat(data.LstData[i].vl, 0, '.', ',') + '</span>'
            }
            self.pie_ChiNhanh(data.LstData);
            self.TongTienHang_ChiNhanh(data.TongTien);
            self.loadDoanhSo_ChiNhanh();
        });
    }
    self.getDoanhThuChiNhanh_Pie();
    self.column_ChiNhanh = ko.observableArray();
    self.doanhso_ColumnChiNhanh = ko.observableArray();
    self.DonViTinh_ChiNhanh = ko.observable();
    self.dvt_ChiNhanh = ko.observable();
    self.ListAdvertisement = ko.observableArray();
    function loadadvertisement() {
        $.getJSON("/api/DanhMuc/TongQuanAPI/GetAdvertisement", function (data) {
            if (data.res === true) {
                self.ListAdvertisement(data.dataSoure);
            }
        });
    }
    loadadvertisement();
    self.getDoanhThuChiNhanh_Column = function () {
        self.doanhso_ColumnChiNhanh([]);
        ajaxHelper(BH_DoanhThuUri + "getDoanhThu_ColumnChiNhanh?dayStart=" + timeStart_CN + "&dayEnd=" + timeEnd_CN + "&ID_NguoiDung=" + _IDNguoiDung, "GET").done(function (data) {
            self.column_ChiNhanh(data.LstData);
            self.TongTienHang_ChiNhanh(data.TongTien);
            for (var i = 0; i < data.LstData.length; i++) {
                var obj = {
                    //name: data.LstData[i].name,
                    name: data.LstData[i].name + '<br> <span class="thucthu">' + Highcharts.numberFormat(data.LstData[i].y, 0, '.', ',') + '</span>',
                    data: [data.LstData[i].y]
                }
                self.doanhso_ColumnChiNhanh.push(obj);
            }
            var dvt = 0;
            for (var i = 0; i < data.LstData.length; i++) {
                if (dvt < data.LstData[i].y) {
                    dvt = data.LstData[i].y;
                }
            }
            if (dvt >= 1000000000) {
                self.DonViTinh_ChiNhanh(1000000000);
                self.dvt_ChiNhanh(" tỷ");
            }
            if (dvt >= 1000000 & dvt < 1000000000) {
                self.DonViTinh_ChiNhanh(1000000);
                self.dvt_ChiNhanh(" tr");
            }
            if (dvt >= 1000 & dvt < 1000000) {
                self.DonViTinh_ChiNhanh(1000);
                self.dvt_ChiNhanh(" k");
            }
            self.loadDoanhSo_ChiNhanh_Column();
        });
    }
    self.getThucThuToHour = function () {
        self.date_ThucThu("Giờ: ");
        self.arrTT([]);
        self.arrDSTT([]);
        self.data_ThucThu([])
        var array_Seach = {
            dayStart: timeStart,
            dayEnd: timeEnd,
            ID_NguoiDung: _IDNguoiDung,
            ID_DonVi: self.ID_DonViSeachThucThu()
        }
        ajaxHelper(BH_DoanhThuUri + "TongQuan_BieuDoThucThuToHour", "POST", array_Seach).done(function (data) {
            //ajaxHelper(BH_DoanhThuUri + "TongQuan_BieuDoThucThuToHour?dayStart=" + timeStart + "&dayEnd=" + timeEnd + "&ID_NguoiDung=" + _IDNguoiDung + "&ID_DonVi=" + self.ID_DonViSeachThucThu(), "GET").done(function (data) {
            self.ThucThuToDay(data.LstData);
            var DVTDS
            var dvt = 0;
            var TongTienDS = 0;
            for (var i = 0; i < self.ThucThuToDay().length; i++) {
                if (dvt < self.ThucThuToDay()[i].ThanhTien) {
                    dvt = self.ThucThuToDay()[i].ThanhTien;
                }
                TongTienDS = TongTienDS + self.ThucThuToDay()[i].ThanhTien;
            }
            self.TongThucThu(TongTienDS);
            if (dvt >= 1000000000) {
                self.DonViTinh_ThucThu(1000000000);
                self.dvt_ThucThu(" tỷ");
            }
            if (dvt >= 1000000 & dvt < 1000000000) {
                self.DonViTinh_ThucThu(1000000);
                self.dvt_ThucThu(" tr");
            }
            if (dvt >= 1000 & dvt < 1000000) {
                self.DonViTinh_ThucThu(1000);
                self.dvt_ThucThu(" k");
            }
            for (var i = 0; i < data.LstChiNhanh.length; i++) {
                for (var j = 0; j < data.LstDate.length; j++) {
                    for (var k = 0; k < data.LstData.length; k++) {
                        if (data.LstData[k].TenChiNhanh == data.LstChiNhanh[i].TenChiNhanh && data.LstData[k].NgayLapHoaDon == data.LstDate[j].NgayLapHoaDon) {
                            self.arrDSTT.push(data.LstData[k].ThanhTien);
                            break;
                        }
                        if (k == data.LstData.length - 1) {
                            self.arrDSTT.push(0);
                        }
                    }
                }
                var obj = {
                    //name: data.LstChiNhanh[i].TenChiNhanh,
                    name: data.LstChiNhanh[i].TenChiNhanh + '<br> <span class="thucthu">' + Highcharts.numberFormat(data.LstChiNhanh[i].ThanhTien, 0, '.', ',') + '</span>',
                    turboThreshold: i,
                    _colorIndex: i,
                    data: self.arrDSTT()
                }
                self.data_ThucThu.push(obj);
                self.arrDSTT([]);
            }
            for (var i = 0; i < data.LstDate.length; i++) {
                _loadDateThucThu = data.LstDate[i].NgayLapHoaDon + ":00";
                self.arrTT.push(_loadDateThucThu);
            }
            self.loadThucThu();
        });
    }
    self.getDoanhThuToHour = function () {
        self.date_Doanhso("Giờ: ");
        self.arrNT([]);
        self.arrDS([]);
        self.data_DoanhSo([])
        var array_Seach = {
            dayStart: timeStart,
            dayEnd: timeEnd,
            ID_NguoiDung: _IDNguoiDung,
            ID_DonVi: self.ID_DonViSeach()
        }
        ajaxHelper(BH_DoanhThuUri + "TongQuan_BieuDoDoanhThuToHour", "POST", array_Seach).done(function (data) {
            //ajaxHelper(BH_DoanhThuUri + "TongQuan_BieuDoDoanhThuToHour?dayStart=" + timeStart + "&dayEnd=" + timeEnd + "&ID_NguoiDung=" + _IDNguoiDung + "&ID_DonVi=" + self.ID_DonViSeach(), "GET").done(function (data) {
            self.DoanhThuToDay(data.LstData);
            var DVTDS
            var dvt = 0;
            var TongTienDS = 0;
            for (var i = 0; i < self.DoanhThuToDay().length; i++) {
                if (dvt < self.DoanhThuToDay()[i].ThanhTien) {
                    dvt = self.DoanhThuToDay()[i].ThanhTien;
                }
                TongTienDS = TongTienDS + self.DoanhThuToDay()[i].ThanhTien;
            }
            self.TongDoanhSo(TongTienDS);
            if (dvt >= 1000000000) {
                self.DonViTinh_DoanhSo(1000000000);
                self.dvt_Doanhso(" tỷ");
            }
            if (dvt >= 1000000 & dvt < 1000000000) {
                self.DonViTinh_DoanhSo(1000000);
                self.dvt_Doanhso(" tr");
            }
            if (dvt >= 1000 & dvt < 1000000) {
                self.DonViTinh_DoanhSo(1000);
                self.dvt_Doanhso(" k");
            }
            for (var i = 0; i < data.LstChiNhanh.length; i++) {
                for (var j = 0; j < data.LstDate.length; j++) {
                    for (var k = 0; k < data.LstData.length; k++) {
                        if (data.LstData[k].TenChiNhanh == data.LstChiNhanh[i].TenChiNhanh && data.LstData[k].NgayLapHoaDon == data.LstDate[j].NgayLapHoaDon) {
                            self.arrDS.push(data.LstData[k].ThanhTien);
                            break;
                        }
                        if (k == data.LstData.length - 1) {
                            self.arrDS.push(0);
                        }
                    }
                }
                var obj = {
                    //name: data.LstChiNhanh[i].TenChiNhanh,
                    name: data.LstChiNhanh[i].TenChiNhanh + '<br> <span class="thucthu">' + Highcharts.numberFormat(data.LstChiNhanh[i].ThanhTien, 0, '.', ',') + '</span>',
                    turboThreshold: i,
                    _colorIndex: i,
                    data: self.arrDS()
                }
                self.data_DoanhSo.push(obj);
                self.arrDS([]);
            }
            for (var i = 0; i < data.LstDate.length; i++) {
                _loadDate = data.LstDate[i].NgayLapHoaDon + ":00";
                self.arrNT.push(_loadDate);
            }
            self.loadDoanhSo();
        });
    }
    self.getBieuHangHoa = function () {
        var datime = new Date();
        if (_LoaiBieu === "Theo doanh thu") {
            if (_NgayLoadBieu === "Hôm nay") {
                timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
                timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
                getDMDoanhThuTT();
            }
            else if (_NgayLoadBieu === "Hôm qua") {
                var dt1 = new Date();
                var dt2 = new Date();
                timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
                timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
                getDMDoanhThuTT();
            }
            else if (_NgayLoadBieu.trim() === "Tháng này") {
                timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
                getDMDoanhThuTT();
            }
            else if (_NgayLoadBieu === "Tháng trước") {
                timeStart = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
                timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                getDMDoanhThuTT();
            }
            else { }
        }
        else {
            if (_NgayLoadBieu === "Hôm nay") {
                timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
                timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
                self.getDMDoanhThuSL();
            }
            else if (_NgayLoadBieu === "Hôm qua") {
                var dt1 = new Date();
                var dt2 = new Date();
                timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
                timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
                self.getDMDoanhThuSL();
            }
            else if (_NgayLoadBieu.trim() === "Tháng này") {
                timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
                self.getDMDoanhThuSL();
            }
            else if (_NgayLoadBieu === "Tháng trước") {
                timeStart = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
                timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                self.getDMDoanhThuSL();
            }
            else { }
        }
    }
    //load hàng bán chạy theo doanh thu
    self.DonViTinh_DoanhThu = ko.observable();
    function getDMDoanhThuTT() {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('TongQuan', lc_CTQuyen) > -1) {
            self.arrHH = ko.observableArray();
            self.arrDT = ko.observableArray();
            var arrDV = self.Top10_ListAllDonVi().filter(x => x.checkSearch === 1 && x.ID !== null).map(function (x) { return x.ID });
            var param = {
                IDChiNhanhs: arrDV,
                DateFrom: timeStart,
                DateTo: timeEnd,
            }

            ajaxHelper(BH_DoanhThuUri + "getListDoanhThuTT", "POST", param).done(function (x) {
                if (x.res) {
                    let data = x.dataSoure;
                    self.DoanhThuTT(data);
                    if (data.length > 0)
                        var dvt = self.DoanhThuTT()[0].ThanhTien;
                    if (dvt >= 1000000000) {
                        //_MauSoDVT = 1000000000
                        self.DonViTinh_DoanhThu(1000000000);
                        self.DonViTinh(" tỷ")
                    }
                    if (dvt >= 1000000 & dvt < 1000000000) {
                        //_MauSoDVT = 1000000
                        self.DonViTinh_DoanhThu(1000000);
                        self.DonViTinh(" tr")
                    }
                    if (dvt >= 1000 & dvt < 1000000) {
                        //_MauSoDVT = 1000
                        self.DonViTinh_DoanhThu(1000);
                        self.DonViTinh(" k")
                    }
                    for (var i = 0; i < self.DoanhThuTT().length; i++) {
                        _loadHangHoa = self.DoanhThuTT()[i].MaHangHoa + ": " + self.DoanhThuTT()[i].TenHangHoa;
                        //_data = parseFloat(self.DoanhThuTT()[i].ThanhTien / _MauSoDVT).toFixed(2) * 1;
                        self.arrDT.push(self.DoanhThuTT()[i].ThanhTien);
                        self.arrHH.push(_loadHangHoa);
                    }
                    nameChar = "Doanh Thu";
                    self.loadBieuDo();
                }
            });
        }
    }

    self.getDMDoanhThuSL = function () {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('TongQuan', lc_CTQuyen) > -1) {
            self.DonViTinh_DoanhThu(1);
            self.DonViTinh("");
            self.arrHH = ko.observableArray();
            self.arrDT = ko.observableArray();
            var arrDV = self.Top10_ListAllDonVi().filter(x => x.checkSearch === 1 && x.ID !== null).map(function (x) { return x.ID });
            var param = {
                IDChiNhanhs: arrDV,
                DateFrom: timeStart,
                DateTo: timeEnd,
            }
            ajaxHelper(BH_DoanhThuUri + "getListDoanhThuSL", "POST", param).done(function (x) {
                if (x.res) {
                    let data = x.dataSoure;
                    self.DoanhThuTT(data);
                    for (var i = 0; i < self.DoanhThuTT().length; i++) {
                        _loadHangHoa = self.DoanhThuTT()[i].MaHangHoa + ": " + self.DoanhThuTT()[i].TenHangHoa;
                        _data = self.DoanhThuTT()[i].SoLuong;
                        self.arrDT.push(_data);
                        self.arrHH.push(_loadHangHoa);
                    }
                    nameChar = "Số lượng";
                    self.loadBieuDo();
                }
            });
        }
    }
    self.loadDoanhSo1 = function () {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('TongQuan', lc_CTQuyen) > -1) {
            var chart = Highcharts.chart('panel-content', {
                chart: {
                    type: 'column'
                },
                title: {
                    text: ''
                },
                xAxis: {
                    categories: self.arrNT()
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: 'Total fruit consumption'
                    },
                    stackLabels: {
                        enabled: true,
                        style: {
                            fontWeight: 'bold',
                            color: (Highcharts.theme && Highcharts.theme.textColor) || 'black'
                        }
                    }
                },

                plotOptions: {
                    column: {
                        stacking: 'normal',
                        dataLabels: {
                            // enabled: true,
                            // color: (Highcharts.theme && Highcharts.theme.dataLabelsColor) || 'white'
                        }
                    }
                },
                series: [{
                    name: "Doanh số",
                    data: self.arrDS(),
                    maxPointWidth: 30
                }],

            });
        }
    }
    self.loadDoanhSo_ChiNhanh = function () {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        var viewPrint = true;
        if (self.pie_ChiNhanh().length > 0)
            viewPrint = true;
        else
            viewPrint = false;
        if ($.inArray('TongQuan', lc_CTQuyen) > -1) {
            var chart = Highcharts.chart('panel-contentChiNhanh', {
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false,
                    type: 'pie',
                    polar: false,
                },
                title: {
                    text: ''
                },
                tooltip: {
                    formatter: function () {
                        return '<b style=\"text-transform: capitalize; font-weight: normal;\">' + this.point.name + '</b>: ' + Highcharts.numberFormat(this.point.vl, 0, '.', ',');
                    },
                    shared: false
                },
                plotOptions: {
                    pie: {
                        point: {
                            events: {
                                legendItemClick: function () {
                                    var Tongtien = this.vl;
                                    if (this.visible) {
                                        var DS = self.TongTienHang_ChiNhanh() - Tongtien;
                                        self.TongTienHang_ChiNhanh(DS);
                                    }
                                    else {
                                        var DS = self.TongTienHang_ChiNhanh() + Tongtien;
                                        self.TongTienHang_ChiNhanh(DS);
                                    }
                                }
                            }
                        },
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            distance: 40,
                            enabled: true,
                            format: "<b style=\"text-transform: capitalize; font-weight: normal;\">{point.name}: {point.percentage:.2f} % </b>",
                        },
                        style: {
                            color: "black",
                        },
                        showInLegend: true
                    }
                },
                series: [{
                    name: 'Doanh số',
                    colorByPoint: true,
                    animation: false,
                    data: self.pie_ChiNhanh()
                }],
                legend: {
                    enabled: true
                },
                credits: {
                    enabled: false,
                },
                exporting: {
                    enabled: viewPrint,
                    buttons: {
                        contextButton: {
                            symbol: 'url(Template/DuLieuGoc/print_24.png)',
                            //x: -62,    
                        }
                    }
                },
                navigation: {
                    buttonOptions: {
                        align: 'right',
                        verticalAlign: 'bottom',
                        theme: {
                            states: {
                                hover: {
                                    fill: 'none'
                                },
                                select: {
                                    fill: 'none'
                                }
                            }
                        }
                    }
                },
                colors: [
                    "#0097c4",
                    "#ef6c00",
                    "#8085e9",
                    "#2979ff",
                    "#8085e9",
                    "#f15c80",
                    "#e4d354",
                    "#2b908f",
                    "#f45b5b",
                    "#91e8e1"
                ]
            });
        }
    };
    self.loadDoanhSo_ChiNhanh_Column = function () {
        var viewPrint = true;
        if (self.doanhso_ColumnChiNhanh().length > 0)
            viewPrint = true;
        else
            viewPrint = false;
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('TongQuan', lc_CTQuyen) > -1) {
            var chart = Highcharts.chart('panel-contentChiNhanh_Column', {
                chart: {
                    type: 'column',
                    polar: false
                },
                title: {
                    text: ''
                },
                subtitle: {
                    text: ''
                },
                xAxis: {
                    categories: [
                        ''
                    ],
                    crosshair: true
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: ''
                    },
                    labels: {
                        formatter: function () {
                            if (this.value == 0)
                                return 0
                            else
                                return (Highcharts.numberFormat(this.value / self.DonViTinh_ChiNhanh(), 1, '.', ',')).replace('.0', '') + self.dvt_ChiNhanh();
                        }
                    }
                },
                tooltip: {
                    headerFormat: "<span style=\"font-size:10px\"> Doanh thu</span><table>",
                    //pointFormat: "<tr><td style=\"color:{series.color};padding:0; text-align:Left; text-transform: capitalize; font-weight: normal;\">{series.name}: </td><td style=\"padding:0;text-align:Right;text-transform: capitalize; font-weight: normal;\"><b>{point.y:.2f} " + self.dvt_Doanhso() + "</b></td></tr>",
                    footerFormat: "</table>",
                    shared: false,
                    useHTML: true,
                    formatter: function () {
                        return '<b>' + this.series.name + '</b>: ' + Highcharts.numberFormat(this.y, 0, '.', ',');
                    }
                },
                plotOptions: {
                    series: {
                        events: {
                            legendItemClick: function () {
                                var Tongtien = 0;
                                for (var i = 0; i < this.data.length; i++) {
                                    Tongtien = Tongtien + this.data[i].y;
                                }
                                if (this.visible) {
                                    var DS = self.TongTienHang_ChiNhanh() - Tongtien;
                                    self.TongTienHang_ChiNhanh(DS);
                                }
                                else {
                                    var DS = self.TongTienHang_ChiNhanh() + Tongtien;
                                    self.TongTienHang_ChiNhanh(DS);
                                }
                            }
                        },
                        //stacking: "normal",
                        animation: false,
                        pointWidth: 30,
                        dataLabels: {
                            style: {
                                color: "contrast",
                                fontSize: "11px",
                                fontWeight: "bold",
                                textOutline: "1px 1px contrast"
                            }
                        }
                    },
                    column: {
                        pointPadding: 0.2,
                        borderWidth: 0
                    }
                },
                series: self.doanhso_ColumnChiNhanh(),
                colors: [
                    "#0097c4",
                    "#ef6c00",
                    "#8085e9",
                    "#2979ff",
                    "#8085e9",
                    "#f15c80",
                    "#e4d354",
                    "#2b908f",
                    "#f45b5b",
                    "#91e8e1"
                ],
                legend: {
                    align: "center",
                    verticalAlign: "bottom",
                    x: 20
                },
                credits: {
                    enabled: false,
                },
                navigation: {
                    buttonOptions: {
                        align: 'right',
                        verticalAlign: 'bottom',
                        theme: {
                            states: {
                                hover: {
                                    fill: 'none'
                                },
                                select: {
                                    fill: 'none'
                                }
                            }
                        }
                    }
                },
                exporting: {
                    enabled: viewPrint,
                    buttons: {
                        contextButton: {
                            symbol: 'url(Template/DuLieuGoc/print_24.png)',
                            //x: -62,    
                        }
                    }
                }
            });
        }
    };
    self.loadDoanhSo = function () {
        var viewPrint = true;
        if (self.data_DoanhSo().length > 0)
            viewPrint = true;
        else
            viewPrint = false;
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('TongQuan', lc_CTQuyen) > -1) {
            var chart = Highcharts.chart('panel-content', {
                chart: {
                    type: 'column',
                    polar: false
                },
                title: {
                    text: ''
                },
                subtitle: {
                    text: ''
                },
                xAxis: {
                    categories: self.arrNT(),
                    crosshair: true
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: ''
                    },
                    labels: {
                        //format: "{value} " + self.dvt_Doanhso()
                        formatter: function () {
                            if (this.value == 0)
                                return 0
                            else
                                return (Highcharts.numberFormat(this.value / self.DonViTinh_DoanhSo(), 1, '.', ',')).replace('.0', '') + self.dvt_Doanhso();
                        }
                    }
                },
                tooltip: {
                    footerFormat: "</table>",
                    shared: false,
                    useHTML: true,
                    formatter: function () {
                        var res = this.series.name.split("<br>");
                        return "<span style=\"font-size:10px\">" + self.date_Doanhso() + this.key + "<br> </span><table>" + '<b style=\"color:{series.color};padding:0; text-align:Left; text-transform: capitalize; font-weight: normal;\">' + res[0] + '</b>: ' + Highcharts.numberFormat(this.y, 0, '.', ',');
                    }
                },
                plotOptions: {
                    series: {
                        events: {
                            legendItemClick: function () {
                                var Tongtien = 0;
                                for (var i = 0; i < this.data.length; i++) {
                                    Tongtien = Tongtien + this.data[i].y;
                                }
                                if (this.visible) {
                                    var DS = self.TongDoanhSo() - Tongtien;
                                    self.TongDoanhSo(DS);
                                }
                                else {
                                    var DS = self.TongDoanhSo() + Tongtien;
                                    self.TongDoanhSo(DS);
                                }
                            }
                        },
                        stacking: "normal",
                        animation: false,
                        // pointWidth: 30,
                        maxPointWidth: 30,
                        dataLabels: {
                            style: {
                                color: "contrast",
                                fontSize: "11px",
                                fontWeight: "bold",
                                textOutline: "1px 1px contrast"
                            }
                        }
                    },
                    column: {
                        pointPadding: 0.2,
                        borderWidth: 0
                    }
                },
                series: self.data_DoanhSo(),
                navigation: {
                    buttonOptions: {
                        align: 'right',
                        verticalAlign: 'bottom',
                        theme: {
                            states: {
                                hover: {
                                    fill: 'none'
                                },
                                select: {
                                    fill: 'none'
                                }
                            }
                        }
                    }
                },
                colors: [
                    "#0097c4",
                    "#ef6c00",
                    "#8085e9",
                    "#2979ff",
                    "#8085e9",
                    "#f15c80",
                    "#e4d354",
                    "#2b908f",
                    "#f45b5b",
                    "#91e8e1"
                ],
                legend: {
                    align: "center",
                    verticalAlign: "bottom",
                },
                credits: {
                    enabled: false,
                },
                exporting: {
                    enabled: viewPrint,
                    buttons: {
                        contextButton: {
                            symbol: 'url(Template/DuLieuGoc/print_24.png)',
                            //x: -62,    
                        }
                    }
                }
            });
        }
    }
    self.loadThucThu = function () {
        var viewPrint = true;
        if (self.data_ThucThu().length > 0)
            viewPrint = true;
        else
            viewPrint = false;
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('TongQuan', lc_CTQuyen) > -1) {
            var chart = Highcharts.chart('panel-contentThucThu', {
                chart: {
                    type: 'column',
                    polar: false
                },
                title: {
                    text: ''
                },
                subtitle: {
                    text: ''
                },
                xAxis: {
                    categories: self.arrTT(),
                    crosshair: true
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: ''
                    },
                    labels: {
                        //format: "{value} " + self.dvt_Doanhso()
                        formatter: function () {
                            if (this.value == 0)
                                return 0
                            else
                                return (Highcharts.numberFormat(this.value / self.DonViTinh_ThucThu(), 1, '.', ',')).replace('.0', '') + self.dvt_ThucThu();
                        }
                    }
                },
                tooltip: {
                    footerFormat: "</table>",
                    shared: false,
                    useHTML: true,
                    formatter: function () {
                        var res = this.series.name.split("<br>");
                        return "<span style=\"font-size:10px\">" + self.date_ThucThu() + this.key + "<br> </span><table>" + '<b style=\"color:{series.color};padding:0; text-align:Left; text-transform: capitalize; font-weight: normal;\">' + res[0] + '</b>: ' + Highcharts.numberFormat(this.y, 0, '.', ',');
                    }
                },
                plotOptions: {
                    series: {
                        events: {
                            legendItemClick: function () {
                                var Tongtien = 0;
                                for (var i = 0; i < this.data.length; i++) {
                                    Tongtien = Tongtien + this.data[i].y;
                                }
                                if (this.visible) {
                                    var DS = self.TongThucThu() - Tongtien;
                                    self.TongThucThu(DS);
                                }
                                else {
                                    var DS = self.TongThucThu() + Tongtien;
                                    self.TongThucThu(DS);
                                }
                            }
                        },
                        stacking: "normal",
                        animation: false,
                        // pointWidth: 30,
                        maxPointWidth: 30,
                        dataLabels: {
                            style: {
                                color: "contrast",
                                fontSize: "11px",
                                fontWeight: "bold",
                                textOutline: "1px 1px contrast"
                            }
                        }
                    },
                    column: {
                        pointPadding: 0.2,
                        borderWidth: 0
                    }
                },
                series: self.data_ThucThu(),
                navigation: {
                    buttonOptions: {
                        align: 'right',
                        verticalAlign: 'bottom',
                        theme: {
                            states: {
                                hover: {
                                    fill: 'none'
                                },
                                select: {
                                    fill: 'none'
                                }
                            }
                        }
                    }
                },
                colors: [
                    "#0097c4",
                    "#ef6c00",
                    "#8085e9",
                    "#2979ff",
                    "#8085e9",
                    "#f15c80",
                    "#e4d354",
                    "#2b908f",
                    "#f45b5b",
                    "#91e8e1"
                ],
                legend: {
                    align: "center",
                    verticalAlign: "bottom",
                },
                credits: {
                    enabled: false,
                },
                exporting: {
                    enabled: viewPrint,
                    buttons: {
                        contextButton: {
                            symbol: 'url(Template/DuLieuGoc/print_24.png)',
                            //x: -62,    
                        }
                    }
                }
            });
        }
    }
    self.loadBieuDo = function () {
        var viewPrint = true;
        if (self.arrDT().length > 0)
            viewPrint = true;
        else
            viewPrint = false;
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('TongQuan', lc_CTQuyen) > -1) {
            var chart = Highcharts.chart('panel-content1', {
                chart: {
                    type: 'bar'
                },
                title: {
                    text: ''
                },
                subtitle: {
                    text: ''
                },

                // đưa danh sách hàng hóa vào cột x
                xAxis: {
                    categories: self.arrHH(),
                    crosshair: true
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: ''
                    },
                    labels: {
                        formatter: function () {
                            if (this.value == 0)
                                return 0
                            else
                                return (Highcharts.numberFormat(this.value / self.DonViTinh_DoanhThu(), 1, '.', ',')).replace('.0', '') + self.DonViTinh();
                        }

                    },
                    plotLines: [{
                        width: 100,

                    }]
                },
                tooltip: {
                    footerFormat: "</table>",
                    shared: false,
                    useHTML: true,
                    formatter: function () {
                        return "<span style=\"font-size:10px\">" + this.key + "<br> </span><table>" + '<b style=\"color:{series.color};padding:0; text-align:Left; text-transform: capitalize; font-weight: normal;\">' + this.series.name + '</b>: ' + Highcharts.numberFormat(this.y, 0, '.', ',');
                    }
                },
                // hiển thị giá trị lên đầu cột
                plotOptions: {
                    //bar: {
                    //    dataLabels: {
                    //        enabled: true
                    //    }
                    //}
                },
                // đưa giá trị tương ứng vào hàng hóa trong cột y
                series: [{
                    name: nameChar,
                    data: self.arrDT(),
                    maxPointWidth: 30
                }],
                colors: [
                    //"#32b7b3",
                    "#0097c4"
                ],
                credits: {
                    enabled: false
                },
                legend: {
                    enabled: false
                },
                item: {
                    enabled: false
                },
                navigation: {
                    buttonOptions: {
                        theme: {
                            states: {
                                hover: {
                                    fill: 'none'
                                },
                                select: {
                                    fill: 'none'
                                }
                            }
                        }
                    }
                },
                exporting: {
                    enabled: viewPrint,
                    buttons: {
                        contextButton: {
                            symbol: 'url(Template/DuLieuGoc/print_24.png)',
                        }
                    }
                }
            });
        }
    }
    //Lua chon ngày
    $(function () {
        $('#datein').dcalendar().on('dateselected', function (e) {
            self._ThanhTien(0);
            self._TraHang(0);
            self.HD_CungKy(0);
            self.KH_CungKy(0);
            self.KH_TaoMoi(0);
            self.KH_QuayLai(0);

            var dt = new Date(e.date);
            timeStart2 = moment(dt).format('YYYY-MM-DD');
            timeEnd2 = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
            self.ToDay(moment(timeStart2).format('DD/MM/YYYY'))
            var datime = new Date();
            var timess = moment(datime).format('YYYY-MM-DD');
            if (timeStart2 === timess)
                self.ToDay("hôm nay");
            self.ResultToday();
            getList_SuKienToDay();

        });
    });
    function getDiary() {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('TongQuan', lc_CTQuyen) > -1) {
            ajaxHelper(BH_DoanhThuUri + "getDiary?&IDchinhanh=" + _IDchinhanh +
                '&TongQuan_XemDS_PhongBan=' + self.TongQuan_XemDS_PhongBan() + '&TongQuan_XemDS_HeThong=' + self.TongQuan_XemDS_HeThong() + '&ID_NguoiDung=' + _IDDoiTuong, "GET").done(function (data) {
                    self.DiaryBH(data);
                });
        }
    }
    self.SuKienToDay = ko.observableArray();
    function getList_SuKienToDay() {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('TongQuan', lc_CTQuyen) > -1) {
            ajaxHelper(BH_DoanhThuUri + "getList_SuKienToDay_v2?&ID_DonVi=" + _IDchinhanh + '&date=' + timeStart2, "GET").done(function (data) {
                if (data.res === true) {
                    self.SuKienToDay(data.dataSoure.data);
                }
            });
        }
    }

    self.InsertQuickly = function (type) {
        localStorage.setItem('InsertQuickly', type);
        var url = '';
        switch (type) {
            case 1:
                url = "/#/Product";
                break;
            case 2:
                url = "/#/Customers";
                break;
            case 3:
                url = "/#/PurchaseOrderItem2";
                break;
            case 4:
                url = "/#/Work";
                break;
            case 5:
                url = "/$/BanHang";
                break;
        }
        window.open(url);
    }
    // load danh sách
    self.LoadKhachHang = function (item) {
        localStorage.setItem('SinhNhatKhachHang', SetObj_Localstore());
        var url = "/#/Customers";
        window.open(url);
    }
    function SetObj_Localstore() {
        return JSON.stringify(
            {
                FromDate: moment(timeStart2, 'YYYY-MM-DD').format('DD/MM/YYYY'),
                ToDate: moment(timeEnd2, 'YYYY-MM-DD').add('days', -1).format('DD/MM/YYYY')
            });
    }
    self.LoadKhachHangTaoMoi = function (item) {
        localStorage.setItem('KhachHangTaoMoi', SetObj_Localstore());
        var url = "/#/Customers";
        window.open(url);
    }
    self.LoadCongViec = function (item) {
        localStorage.setItem('calendarKieuBang', 1);
        localStorage.setItem('CongViecKhachHang', SetObj_Localstore());
        var url = "/#/Work";
        window.open(url);
    }
    self.LoadLichHen = function (item) {
        localStorage.setItem('calendarKieuBang', 1);
        localStorage.setItem('LichHenKhachHang', SetObj_Localstore());
        var url = "/#/Work";
        window.open(url);
    }
    self.LoadLoHangSapHetHan = function (item) {
        localStorage.setItem('ThongBaoLoHangSapHetHan', item.SoLoHetHan);
        var url = "/#/Shipment";
        window.open(url);
    }
    self.LoadLoHangDaHetHan = function (item) {
        localStorage.setItem('ThongBaoLoHangDaHetHan', item.SoLoHetHan);
        var url = "/#/Shipment";
        window.open(url);
    }
    //trinhpv tạo dữ liệu mẫu
    function NS_SuDung() {
        ajaxHelper(BH_DoanhThuUri + 'check_CreateDL', 'GET').done(function (data) {
            if (data) {
                ajaxHelper(BH_DoanhThuUri + 'update_QuanLyTheoLoHang', 'Get').done(function (data1) {
                    $('#CreateDL').modal("show");
                });
            }
            else {
                $('#CreateDL').modal("hide");
            }
        })
    }
    NS_SuDung();

    self.CreateDLGoc = function () {
        callAjax_CreateDuLieuGoc();
    }
    var NoiDung_NhatKy = null;
    function callAjax_CreateDuLieuGoc() {
        $('#CreateDL').modal("hide");
        hidewait('table_h');
        var myData = {};
        //myData.objChotSo_ChiTiet = self.DonVi_ChotSo();
        $.ajax({
            data: myData,
            url: BH_DoanhThuUri + "Creater_DuLieuMau?ID_DonVi=" + _IDchinhanh + "&ID_NhanVien=" + _id_NhanVien,
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (item) {
                NoiDung_NhatKy = 'Khởi tạo dữ liệu mẫu'
                callAjax_NKSD();
                $("div[id ^= 'wait']").text("");
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Tạo dữ liệu mẫu thành công!", "success");
                self.ResultToday();
                self.getBieuHangHoa();
                self.NgayLoadDoanhSo(_NgayDoanhSo);
                self.getBieuDoanhSo();
                self.getBieuThucThu();
                self.getBieuDoanhSo_ChiNhanh();
                getDiary();
                getList_SuKienToDay();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $("div[id ^= 'wait']").text("");
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Tạo dữ liệu mẫu không thành công!", "danger");
            },
            complete: function (item) {
            }
        })
    }
    function callAjax_NKSD() {
        var myData = {};
        //myData.objChotSo_ChiTiet = self.DonVi_ChotSo();
        $.ajax({
            data: myData,
            url: BH_DoanhThuUri + "NK_SuDung?ID_DonVi=" + _IDchinhanh + "&ID_NhanVien=" + _id_NhanVien + "&NoiDung=" + NoiDung_NhatKy,
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (item) {
            },
            error: function (jqXHR, textStatus, errorThrown) {
            },
            complete: function (item) {
            }
        })
    }
    self.ignore = function () {
        NoiDung_NhatKy = 'Bỏ qua'
        callAjax_NKSD();
        $('#CreateDL').modal("hide");
    }
    self.SaveLocal = function () {
        var lc_CreateDL = [];
        var ob2 =
        {
            ID: _id_NhanVien
        }
        lc_CreateDL.unshift(ob2);
        localStorage.setItem('lc_CreateDL', JSON.stringify(lc_CreateDL));
    }
    //getallquyen

    function hidewait(o) {
        $('.' + o).append('<div id="wait"><img src="/Content/images/wait.gif" width="64" height="64" /><div class="happy-wait">' +
            ' </div>' +
            '</div>')
    }

}
ko.applyBindings(new ViewModelTongQuan());

