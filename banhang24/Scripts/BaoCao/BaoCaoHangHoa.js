$(function() {
    var ViewModel = function() {
        var self = this;
        self.ColumnsExcelBH = ko.observableArray();
        self.ColumnsExcelLN = ko.observableArray();
        self.ColumnsExcelXNT = ko.observableArray();
        self.ColumnsExcelXNTCT = ko.observableArray();
        self.ColumnsExcelXH = ko.observableArray();
        self.ColumnsExcelXHCTQ = ko.observableArray();
        self.ColumnsExcelCHCT = ko.observableArray();
        self.ColumnsExcelNHCTQ = ko.observableArray();
        self.ColumnsExcelTHN = ko.observableArray();
        self.ColumnsExcelTHNCT = ko.observableArray();
        self.ColumnsExcelNHNCC = ko.observableArray();
        self.ColumnsExcelCTNHNCC = ko.observableArray();
        self.ColumnsExcelNV = ko.observableArray();
        self.ColumnsExcelKH = ko.observableArray();
        self.ColumnsExcelNCC = ko.observableArray();
        self.ColumnsExcelTK = ko.observableArray();
        self.ColumnsExcelTHHNKTQ = ko.observableArray();
        self.ColumnsExcelTHHNKCT = ko.observableArray();
        self.ColumnsExcelTHHXKTQ = ko.observableArray();
        self.ColumnsExcelTHHXKCT = ko.observableArray();

        var cacheExcelBH = true;
        var cacheExcelLN = true;
        var cacheExcelXNT = true;
        var cacheExcelXNTCT = true;
        var cacheExcelXH = true;
        var cacheExcelXHCTQ = true;
        var cacheExcelCHCT = true;
        var cacheExcelNHCTQ = true;
        var cacheExcelTHN = true;
        var cacheExcelTHNCT = true;
        var cacheExcelNHNCC = true;
        var cacheExcelCTNHNCC = true;
        var cacheExcelNV = true;
        var cacheExcelKH = true;
        var cacheExcelNCC = true;
        var cacheExcelTK = true;
        var cacheExcelTHHXKTQ = true;
        var cacheExcelTHHXKCT = true;
        var cacheExcelTHHNKTQ = true;
        var cacheExcelTHHNKCT = true;


        var thisDate;
        self.MangChiNhanh = ko.observableArray();
        self.DonVis = ko.observableArray();
        self.ChungTus = ko.observableArray();
        self.searchDonVi = ko.observableArray()
        var _idDonViSeach = $('#hd_IDdDonVi').val();
        var _idChungTuSeach = null;
        var _nameDonViSeach = null;
        var BH_DonViUri = '/api/DanhMuc/DM_DonViAPI/'
        var _IDDoiTuong = $('.idnguoidung').text();

        self.Loc_Table = ko.observable('1');
        self.Select_Table = ko.observable('1');
        self.Select_Tabletk = ko.observable('1');
        self.Select_Tablencc = ko.observable('1');
        self.LoaiSP_HH = ko.observable(true);
        self.LoaiSP_DV = ko.observable(true);
        self.HangHoas = ko.observableArray();
        //self.ReportHH_BanHang = ko.observableArray();
        //self.ReportHH_LoiNhuan = ko.observableArray();
        //self.ReportHH_XuatNhapTon = ko.observableArray();
        //self.ReportHH_XuatNhapTonChiTiet = ko.observableArray();
        //self.ReportHH_NhanVien = ko.observableArray();
        //self.ReportHH_KhachHang = ko.observableArray();
        //self.ReportHH_NhaCungCap = ko.observableArray();
        self.TongCongHH_BanHang = ko.observableArray();
        self.TongCongHH_LoiNhuan = ko.observableArray();
        self.TongCongHH_XuatNhapTon = ko.observableArray();
        self.TongCongHH_XuatNhapTonChiTiet = ko.observableArray();
        self.TongCongHH_NhanVien = ko.observableArray();
        self.TongCongHH_SoLuongXuatHuy = ko.observable();
        self.TongCongHH_GiaTriXuatHuy = ko.observable();
        self.TongCongHH_KhachHang = ko.observableArray();
        self.TongCongHH_NhaCungCap = ko.observableArray();

        self.ReportHH_BanHangPrint = ko.observableArray();
        self.ReportHH_LoiNhuanPrint = ko.observableArray();
        self.ReportHH_XuatNhapTonPrint = ko.observableArray();
        self.ReportHH_XuatNhapTonChiTietPrint = ko.observableArray();
        self.ReportHH_NhanVienPrint = ko.observableArray();
        self.ReportHH_XuatHuyPrint = ko.observableArray();
        self.ReportHH_KhachHangPrint = ko.observableArray();
        self.ReportHH_NhaCungCapPrint = ko.observableArray();
        self.TongCongHH_BanHangPrint = ko.observableArray();
        self.TongCongHH_LoiNhuanPrint = ko.observableArray();
        self.TongCongHH_XuatNhapTonPrint = ko.observableArray();
        self.TongCongHH_XuatNhapTonChiTietPrint = ko.observableArray();
        self.TongCongHH_NhanVienPrint = ko.observableArray();
        self.TongCongHH_SoLuongXuatHuyPrint = ko.observable();
        self.TongCongHH_GiaTriXuatHuyPrint = ko.observable();
        self.TongCongHH_KhachHangPrint = ko.observableArray();
        self.TongCongHH_NhaCungCapPrint = ko.observableArray();
        self.ReportHH_XuatChuyenHangPrint = ko.observableArray();
        self.ReportHH_XuatChuyenHangChiTietPrint = ko.observableArray();
        self.ReportHH_NhapChuyenHangPrint = ko.observableArray();
        self.ReportHH_NhapChuyenHangChiTietPrint = ko.observableArray();



        self.MoiQuanTam = ko.observable('bán hàng');
        var _id_DonVi = $('#hd_IDdDonVi').val();
        self.TenChiNhanh = ko.observable($('.branch label').text());
        self.TodayBC = ko.observable('Hôm nay');
        self.SumNumberPageReport = ko.observableArray();
        self.RowsStart = ko.observable('1');
        self.RowsEnd = ko.observable('10');
        self.RowsStart_CH = ko.observable('1');
        self.RowsEnd_CH = ko.observable('10');
        var AllPageHangHoa;
        self.RowsStart_NH = ko.observable('1');
        self.RowsEnd_NH = ko.observable('10');
        var AllPageHangHoaNH;
        self.SumRowsHangHoa = ko.observable();
        self.NhomHangHoas = ko.observableArray();
        self.TongCongCH_SoLuong = ko.observable();
        self.TongCongCH_GiaTri = ko.observable();
        self.TongCongNH_SoLuong = ko.observable();
        self.TongCongNH_GiaTri = ko.observable();
        self.Loc_TinhTrangKD = ko.observable('2');
        var TinhTrangHH = 2;
        $('#txtMaHH').focus();
        var dt1 = new Date();
        var tk = null;
        //var _timeStart = '2015-09-26'
        //var _timeEnd = moment(new Date(dt1.setDate(dt1.getDate() + 1))).format('YYYY-MM-DD');
        var _timeStart = dt1.getFullYear() + "-" + (dt1.getMonth() + 1) + "-" + dt1.getDate();
        var _timeEnd = moment(new Date(dt1.setDate(dt1.getDate() + 1))).format('YYYY-MM-DD');
        var _tonkhoStart = _timeStart;
        var _tonkhoEnd = _timeEnd;
        var _maHH = null;
        var _laHangHoa = 2;
        var _ckHangHoa = 1;
        var _ckDichVu = 1;
        self.pageNumberBH = ko.observable(1);
        self.pageNumberLN = ko.observable(1);
        self.pageNumberNHNCC = ko.observable(1);
        self.pageNumberNHCTNCC = ko.observable(1);
        self.pageNumberTHN = ko.observable(1);
        self.pageNumberTHNCT = ko.observable(1);
        self.pageNumberCH = ko.observable(1);
        self.pageNumberHDCH = ko.observable(1);
        self.pageNumberXH = ko.observable(1);
        self.pageNumberNV = ko.observable(1);
        self.pageNumberKH = ko.observable(1);
        self.pageNumberNCC = ko.observable(1);
        self.pageNumberTK = ko.observable(1);
        self.pageNumberXNT = ko.observable(1);
        self.pageNumberXNTCT = ko.observable(1);
        self.pageNumberTHNK = ko.observable(1);
        self.pageNumberTHNKCT = ko.observable(1);
        self.pageNumberTHXK = ko.observable(1);
        self.pageNumberTHXKCT = ko.observable(1);

        self.pageSize = ko.observable(10);
        var ReportUri = '/api/DanhMuc/ReportAPI/';
        var DiaryUri = '/api/DanhMuc/SaveDiary/';
        var _id_NhanVien = $('.idnhanvien').text();
        $('.TongCongHH_LoiNhuan').hide();
        $('.TongCongHH_XNT').hide();
        $('.TongCongHH_XNTChiTiet').hide();
        $('.TongCongXuatHuy').hide();
        $('.TongCongHH_NhanVien').hide();
        $('.TongCongHH_KhachHang').hide();
        $('.TongCongHH_NhaCungCap').hide();
        $('.TongCongChuyenHang').hide();
        $('.TongCongHDXChuyenHang').hide();
        $('.TongCongNhapHangNCC').hide();
        $('.TongCongNhapHangChiTietNCC').hide();
        $('.TongCongTraHangNhapChiTiet').hide();
        $('.TongCongTraHangNhap').hide();
        $('.TongCongHHTonKho').hide();
        $('.TongCongCTTongHopNhapKho').hide();
        $('.TongCongTQTongHopNhapKho').hide();
        $('.TongCongCTTongHopXuatKho').hide();
        $('.TongCongTQTongHopXuatKho').hide();
        $('.TongHH_BanHang').hide();

        //trinhpv phân quyền
        self.BCHangHoa = ko.observable();
        self.BCHH_BanHang = ko.observable();
        self.BCHH_BanHang_XuatFile = ko.observable();
        self.BCHH_ChuyenHang = ko.observable();
        self.BCHH_ChuyenHang_XuatFile = ko.observable();
        self.BCHH_KhachHang = ko.observable();
        self.BCHH_KhachHang_XuatFile = ko.observable();
        self.BCHH_LoiNhuan = ko.observable();
        self.BCHH_LoiNhuan_XuatFile = ko.observable();
        self.BCHH_NCC = ko.observable();
        self.BCHH_NCC_XuatFile = ko.observable();
        self.BCHH_NhanVien = ko.observable();
        self.BCHH_NhanVien_XuatFile = ko.observable();
        self.BCHH_TraHangNhap = ko.observable();
        self.BCHH_TraHangNhap_XuatFile = ko.observable();
        self.BCHH_XuatHuy = ko.observable();
        self.BCHH_XuatHuy_XuatFile = ko.observable();
        self.BCHH_TonKho = ko.observable();
        self.BCHH_TonKho_XuatFile = ko.observable();
        self.BCHH_XuatNhapTon = ko.observable();
        self.BCHH_XuatNhapTon_XuatFile = ko.observable();
        self.BCHH_XuatNhapTonChiTiet = ko.observable();
        self.BCHH_XuatNhapTonChiTiet_XuatFile = ko.observable();
        function getQuyen_NguoiDung() {
            //quyền xem báo cáo
            ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCHH_BanHang", "GET").done(function(data) {
                self.BCHH_BanHang(data);
                self.getListReportHH_BanHang();
            })
            ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCHH_BanHang_XuatFile", "GET").done(function(data) {
                self.BCHH_BanHang_XuatFile(data);
            })
            ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCHH_ChuyenHang", "GET").done(function(data) {
                self.BCHH_ChuyenHang(data);
            })
            ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCHH_ChuyenHang_XuatFile", "GET").done(function(data) {
                self.BCHH_ChuyenHang_XuatFile(data);
            })
            ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCHH_KhachHang", "GET").done(function(data) {
                self.BCHH_KhachHang(data);
            })
            ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCHH_KhachHang_XuatFile", "GET").done(function(data) {
                self.BCHH_KhachHang_XuatFile(data);
            })
            ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCHH_LoiNhuan", "GET").done(function(data) {
                self.BCHH_LoiNhuan(data);
            })
            ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCHH_LoiNhuan_XuatFile", "GET").done(function(data) {
                self.BCHH_LoiNhuan_XuatFile(data);
            })
            ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCHH_NCC", "GET").done(function(data) {
                self.BCHH_NCC(data);
            })
            ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCHH_NCC_XuatFile", "GET").done(function(data) {
                self.BCHH_NCC_XuatFile(data);
            })
            ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCHH_NhanVien", "GET").done(function(data) {
                self.BCHH_NhanVien(data);
            })
            ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCHH_NhanVien_XuatFile", "GET").done(function(data) {
                self.BCHH_NhanVien_XuatFile(data);
            })
            ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCHH_TraHangNhap", "GET").done(function(data) {
                self.BCHH_TraHangNhap(data);
            })
            ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCHH_TraHangNhap_XuatFile", "GET").done(function(data) {
                self.BCHH_TraHangNhap_XuatFile(data);
            })
            ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCHH_XuatHuy", "GET").done(function(data) {
                self.BCHH_XuatHuy(data);
            })
            ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCHH_XuatHuy_XuatFile", "GET").done(function(data) {
                self.BCHH_XuatHuy_XuatFile(data);
            })
            ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCHH_XuatNhapTon", "GET").done(function(data) {
                self.BCHH_XuatNhapTon(data);
            })
            ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCHH_XuatNhapTon_XuatFile", "GET").done(function(data) {
                self.BCHH_XuatNhapTon_XuatFile(data);
            })
            ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCHH_XuatNhapTonChiTiet", "GET").done(function(data) {
                self.BCHH_XuatNhapTonChiTiet(data);
            })
            ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCHH_XuatNhapTonChiTiet_XuatFile", "GET").done(function(data) {
                self.BCHH_XuatNhapTonChiTiet_XuatFile(data);
            })
            ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCHH_TonKho", "GET").done(function(data) {
                self.BCHH_TonKho(data);
            })
            ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCHH_TonKho_XuatFile", "GET").done(function(data) {
                self.BCHH_TonKho_XuatFile(data);
            })
        }
        getQuyen_NguoiDung();
        self.currentPage = ko.observable(1);
        self.currentPageHangHoa = ko.observable(1);
        self.currentPageHangHoaNH = ko.observable(1);
        var _ID_NhomHang = null;
        //Select table Report
        self.check_kieubang = ko.observable('2');
        //load đơn vị
        function getDonVi() {
            ajaxHelper(BH_DonViUri + "GetDonVi_byUserSearch?ID_NguoiDung=" + _IDDoiTuong + "&TenDonVi=" + _nameDonViSeach, "GET").done(function(data) {
                self.DonVis(data);
                self.searchDonVi(data);
                if (self.DonVis().length < 2)
                    $('.showChiNhanh').hide();
                else
                    $('.showChiNhanh').show();
                for (var i = 0; i < self.DonVis().length; i++) {
                    if (self.DonVis()[i].ID == _id_DonVi) {
                        self.TenChiNhanh(self.DonVis()[i].TenDonVi);
                        self.SelectedDonVi(self.DonVis()[i]);
                    }
                }
            });
        }
        getDonVi();
        //Lua chon don vi
        self.IDSelectedDV = ko.observableArray();
        $(document).on('click', '.per_ac1 li', function() {
            var ch = $(this).index();
            $(this).remove();
            var li = document.getElementById("selec-person");
            var list = li.getElementsByTagName("li");
            for (var i = 0; i < list.length; i++) {
                $("#selec-person ul li").eq(ch).find(".fa-check").css("display", "none");
            }
            var nameDV = _idDonViSeach.split('-');
            _idDonViSeach = null;
            for (var i = 0; i < nameDV.length; i++) {
                if (nameDV[i].trim() != $(this).text().trim()) {
                    if (_idDonViSeach == null) {
                        _idDonViSeach = nameDV[i];
                    }
                    else {
                        _idDonViSeach = nameDV[i] + "-" + _idDonViSeach;
                    }
                }
            }
            if (_idDonViSeach.trim() == "null") {
            }
            else {
            }

        })

        self.CloseDonVi = function(item) {
            _idDonViSeach = null;
            var TenChiNhanh;
            self.MangChiNhanh.remove(item);
            for (var i = 0; i < self.MangChiNhanh().length; i++) {
                if (_idDonViSeach == null) {
                    _idDonViSeach = self.MangChiNhanh()[i].ID;
                    TenChiNhanh = self.MangChiNhanh()[i].TenDonVi;
                }
                else {
                    _idDonViSeach = self.MangChiNhanh()[i].ID + "," + _idDonViSeach;
                    TenChiNhanh = TenChiNhanh + ", " + self.MangChiNhanh()[i].TenDonVi;
                }
            }
            if (self.MangChiNhanh().length === 0) {
                $("#NoteNameDonVi").attr("placeholder", "Chọn chi nhánh...");
                TenChiNhanh = 'Tất cả chi nhánh.'
                for (var i = 0; i < self.searchDonVi().length; i++) {
                    if (_idDonViSeach == null)
                        _idDonViSeach = self.searchDonVi()[i].ID;
                    else
                        _idDonViSeach = self.searchDonVi()[i].ID + "," + _idDonViSeach;
                }
            }
            self.TenChiNhanh(TenChiNhanh);
            $('#selec-all-DonVi li').each(function() {
                if ($(this).attr('id') === item.ID) {
                    $(this).find('i').remove();
                }
            });
            _pageNumber = 1;
            if (_kieubang == 1) {
                if (self.check_kieubang() == 1) {
                    self.getListReportHH_BanHang_BieuDo();
                }
                else {
                    self.getListReportHH_BanHang();
                }
            }
            else if (_kieubang == 2) {
                if (self.check_kieubang() == 1)
                    self.getListReportHH_LoiNhuan_BieuDo();
                else
                    self.getListReportHH_LoiNhuan();
            }
            else if (_kieubang == 3)
                self.getListReportHH_XuatNhapTon();
            else if (_kieubang == 4)
                self.getListReportHH_XuatNhapTonChiTiet();
            else if (_kieubang == 5)
                self.getListReportHH_XuatHuy();
            else if (_kieubang == 6)
                self.getListReportHH_NhanVien();
            else if (_kieubang == 7)
                self.getListReportHH_KhachHang();
            else if (_kieubang == 8)
                self.getListReportHH_NhaCungCap();
            else if (_kieubang == 9) {
                if (dk_tab == 1)
                    self.selectXuatChuyenHang();
                else
                    self.selectNhapChuyenHang();
            }
            else if (_kieubang == 10) {
                if (dk_tabncc == 1) {
                    if (_selectTabncc == 1)
                        self.getListReportHH_NhapHangNCC();
                    else
                        self.getListReportHH_NhapHangChiTietNCC();
                }
                else {
                    if (_selectTabncc == 1)
                        self.getListReportHH_TraHangNhap();
                    else
                        self.getListReportHH_TraHangNhapChiTiet();
                }
            }

            self.ReserPage();
        }

        self.SelectedDonVi = function(item) {
            _idDonViSeach = null;
            var TenChiNhanh;
            var arrIDDonVi = [];
            for (var i = 0; i < self.MangChiNhanh().length; i++) {
                if ($.inArray(self.MangChiNhanh()[i], arrIDDonVi) === -1) {
                    arrIDDonVi.push(self.MangChiNhanh()[i].ID);
                }
            }
            if ($.inArray(item.ID, arrIDDonVi) === -1) {
                self.MangChiNhanh.push(item);
                $('#NoteNameDonVi').removeAttr('placeholder');
                for (var i = 0; i < self.MangChiNhanh().length; i++) {
                    if (_idDonViSeach == null) {
                        _idDonViSeach = self.MangChiNhanh()[i].ID;
                        TenChiNhanh = self.MangChiNhanh()[i].TenDonVi;
                    }
                    else {
                        _idDonViSeach = self.MangChiNhanh()[i].ID + "," + _idDonViSeach;
                        TenChiNhanh = TenChiNhanh + ", " + self.MangChiNhanh()[i].TenDonVi;
                    }
                }
                self.TenChiNhanh(TenChiNhanh);
                _pageNumber = 1;
                if (_kieubang == 1) {
                    if (self.check_kieubang() == 1) {
                        self.getListReportHH_BanHang_BieuDo();
                    }
                    else {
                        self.getListReportHH_BanHang();
                    }
                }
                else if (_kieubang == 2) {
                    if (self.check_kieubang() == 1)
                        self.getListReportHH_LoiNhuan_BieuDo();
                    else
                        self.getListReportHH_LoiNhuan();
                }
                else if (_kieubang == 3)
                    self.getListReportHH_XuatNhapTon();
                else if (_kieubang == 4)
                    self.getListReportHH_XuatNhapTonChiTiet();
                else if (_kieubang == 5)
                    self.getListReportHH_XuatHuy();
                else if (_kieubang == 6)
                    self.getListReportHH_NhanVien();
                else if (_kieubang == 7)
                    self.getListReportHH_KhachHang();
                else if (_kieubang == 8)
                    self.getListReportHH_NhaCungCap();
                else if (_kieubang == 9) {
                    if (dk_tab == 1)
                        self.selectXuatChuyenHang();
                    else
                        self.selectNhapChuyenHang();
                }
                else if (_kieubang == 10) {
                    if (dk_tabncc == 1) {
                        if (_selectTabncc == 1)
                            self.getListReportHH_NhapHangNCC();
                        else
                            self.getListReportHH_NhapHangChiTietNCC();
                    }
                    else {
                        if (_selectTabncc == 1)
                            self.getListReportHH_TraHangNhap();
                        else
                            self.getListReportHH_TraHangNhapChiTiet();
                    }
                }
                self.ReserPage();
            }
            //thêm dấu check vào đối tượng được chọn
            $('#selec-all-DonVi li').each(function() {
                if ($(this).attr('id') === item.ID) {
                    $(this).find('i').remove();
                    $(this).append('<i class="fa fa-check check-after-li"></i>')
                }
            });

        }
        //lọc đơn vị
        self.NoteNameDonVi = function() {
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
        $('#NoteNameDonVi').keypress(function(e) {
            if (e.keyCode == 13 && self.DonVis().length > 0) {
                self.SelectedDonVi(self.DonVis()[0]);
            }
        });
        self.MangChungTu = ko.observableArray();
        self.searchChungTu = ko.observableArray();
        self.getListDM_LoaiChungTuNhapKho = function(item) {
            //console.log(dk_tabtk, _selectTabtk);
            self.MangChungTu([])
            ajaxHelper(ReportUri + "getListDM_LoaiChungTu?LoaiChungTu=" + item).done(function(data) {
                self.ChungTus(data);
                self.searchChungTu(data);
                for (var i = 0; i < data.length; i++) {
                    if (i == 0)
                        _idChungTuSeach = data[i].ID;
                    else
                        _idChungTuSeach = _idChungTuSeach + "," + data[i].ID;
                }
                if (_selectTabtk == 1) {
                    $('.NhapKhoTongQuan').show();
                    self.getListReportHangHoa_TongHopNhapKho();
                }
                else {
                    $('.NhapKhoChiTiet').show();
                    self.getListReportHangHoa_TongHopNhapKhoChiTiet();
                }
            });
        }
        self.getListDM_LoaiChungTuXuatKho = function(item) {
            self.MangChungTu([])
            ajaxHelper(ReportUri + "getListDM_LoaiChungTu?LoaiChungTu=" + item).done(function(data) {
                self.ChungTus(data);
                self.searchChungTu(data);
                for (var i = 0; i < data.length; i++) {
                    if (i == 0)
                        _idChungTuSeach = data[i].ID;
                    else
                        _idChungTuSeach = _idChungTuSeach + "," + data[i].ID;
                }
                if (_selectTabtk == 1) {
                    $('.XuatKhoTongQuan').show();
                    self.getListReportHangHoa_TongHopXuatKho();
                }
                else {
                    $('.XuatKhoChiTiet').show();
                    self.getListReportHangHoa_TongHopXuatKhoChiTiet();
                }
            });
        }
        self.CloseChungTu = function(item) {
            _idChungTuSeach = null;
            self.MangChungTu.remove(item);
            for (var i = 0; i < self.MangChungTu().length; i++) {
                if (_idChungTuSeach == null) {
                    _idChungTuSeach = self.MangChungTu()[i].ID;
                }
                else {
                    _idChungTuSeach = self.MangChungTu()[i].ID + "," + _idChungTuSeach;
                }
            }
            if (self.MangChungTu().length === 0) {
                $("#NoteNameChungTu").attr("placeholder", "Chọn chứng từ...");
                for (var i = 0; i < self.searchChungTu().length; i++) {
                    if (_idChungTuSeach == null)
                        _idChungTuSeach = self.searchChungTu()[i].ID;
                    else
                        _idChungTuSeach = self.searchChungTu()[i].ID + "," + _idChungTuSeach;
                }
            }
            $('#selec-all-ChungTu li').each(function() {
                if ($(this).attr('id') == item.ID) {
                    $(this).find('i').remove();
                }
            });
            _pageNumber = 1;
            if (dk_tabtk == 2) {
                if (_selectTabtk == 1) {
                    self.getListReportHangHoa_TongHopNhapKho();
                }
                else {
                    self.getListReportHangHoa_TongHopNhapKhoChiTiet();
                }
            }
            else if (dk_tabtk == 3) {
                if (_selectTabtk == 1) {
                    self.getListReportHangHoa_TongHopXuatKho();
                }
                else {
                    self.getListReportHangHoa_TongHopXuatKhoChiTiet();
                }
            }
            self.ReserPage();
        }
        self.SelectedChungTu = function(item) {
            _idChungTuSeach = null;
            var arrIDChungTu = [];
            for (var i = 0; i < self.MangChungTu().length; i++) {
                if ($.inArray(self.MangChungTu()[i], arrIDChungTu) === -1) {
                    arrIDChungTu.push(self.MangChungTu()[i].ID);
                }
            }
            if ($.inArray(item.ID, arrIDChungTu) === -1) {
                self.MangChungTu.push(item);
                $('#NoteNameChungTu').removeAttr('placeholder');
                for (var i = 0; i < self.MangChungTu().length; i++) {
                    if (_idChungTuSeach == null) {
                        _idChungTuSeach = self.MangChungTu()[i].ID;
                    }
                    else {
                        _idChungTuSeach = self.MangChungTu()[i].ID + "," + _idChungTuSeach;
                    }
                }
                _pageNumber = 1;
                if (dk_tabtk == 2) {
                    if (_selectTabtk == 1) {
                        self.getListReportHangHoa_TongHopNhapKho();
                    }
                    else {
                        self.getListReportHangHoa_TongHopNhapKhoChiTiet();
                    }
                }
                else if (dk_tabtk == 3) {
                    if (_selectTabtk == 1) {
                        self.getListReportHangHoa_TongHopXuatKho();
                    }
                    else {
                        self.getListReportHangHoa_TongHopXuatKhoChiTiet();
                    }
                }
                self.ReserPage();
            }
            //thêm dấu check vào đối tượng được chọn
            $('#selec-all-ChungTu li').each(function() {
                if ($(this).attr('id') == item.ID) {
                    $(this).find('i').remove();
                    $(this).append('<i class="fa fa-check check-after-li"></i>')
                }
            });

        }
        self.NoteNameChungTu = function() {
            var arrChungTu = [];
            var itemSearch = locdau($('#NoteNameChungTu').val().toLowerCase());
            for (var i = 0; i < self.searchChungTu().length; i++) {
                var locdauInput = locdau(self.searchChungTu()[i].TenChungTu).toLowerCase();
                var R = locdauInput.split(itemSearch);
                if (R.length > 1) {
                    arrChungTu.push(self.searchChungTu()[i]);
                }
            }
            self.ChungTus(arrChungTu);
            if ($('#NoteNameChungTu').val() == "") {
                self.ChungTus(self.searchChungTu());
            }
        }
        $('#NoteNameChungTu').keypress(function(e) {
            if (e.keyCode == 13 && self.ChungTus().length > 0) {
                self.SelectedChungTu(self.ChungTus()[0]);
            }
        });

        $('.chose_kieubang li').on('click', function() {
            //self.check_kieubang($(this).val());
            $("#BieuDo a ").removeClass("box-tab");
            $("#BaoCao a ").removeClass("box-tab");
            if ($(this).val() == 1) {
                self.check_kieubang('1');
                $('#info').removeClass("active")
                $('#home').addClass("active")
                $('#home123').addClass("active")
            }
            else if ($(this).val() == 2) {
                self.check_kieubang('2');
                $('#home').removeClass("active")
                $('#home123').removeClass("active")
                $('#info').addClass("active")
            }
            if (_kieubang == 1) {
                if (self.check_kieubang() == 1) {
                    self.getListReportHH_BanHang_BieuDo();
                }
                else {
                    self.getListReportHH_BanHang();
                }
            }
            else if (_kieubang == 2) {
                if (self.check_kieubang() == 1)
                    self.getListReportHH_LoiNhuan_BieuDo();
                else
                    self.getListReportHH_LoiNhuan();
            }
        })
        var _kieubang = 1;
        var _selectTab = 1;
        var _selectTabtk = 1;
        var _selectTabncc = 1;
        $('.chooseTableBC input').on('click', function() {
            $(".Report_Empty").hide();
            var TenChiNhanh = null;
            if (self.MangChiNhanh().length > 0) {
                for (var i = 0; i < self.MangChiNhanh().length; i++) {
                    if (TenChiNhanh == null) {
                        TenChiNhanh = self.MangChiNhanh()[i].TenDonVi;
                    }
                    else {
                        TenChiNhanh = TenChiNhanh + ", " + self.MangChiNhanh()[i].TenDonVi;
                    }
                }
            }
            else {
                TenChiNhanh = 'Tất cả chi nhánh.'
            }
            self.TenChiNhanh(TenChiNhanh);
            $('.showChiNhanh').show();
            $('.showLoaiHang').show()

            self.Loc_Table($(this).val())
            _kieubang = $(this).val();
            self.hideTableReport();
            _pageNumber = 1;
            if ($(this).val() == 1) {
                $('.DateRG').show();
                $('.table_BanHang').show();
                $('#BieuDo').show();
                $(".column-hide").show();
                $('.list_HHBanHang').show();
                $(".list_HHLoiNhuan").hide();
                $(".list_HHXuatNhapTon").hide();
                $(".list_HHKhachTheoHang").hide();
                $(".list_HHNhanVien").hide();
                $(".list_HHNCC").hide();
                $(".list_HHXuatNhapTonChiTiet").hide();
                $(".list_HHXuatHuy").hide();
                $(".list_HHTraNhapHang").hide();
                $(".list_HHCHXuatTQ ").hide();
                $(".list_HHCHXuatCT").hide();
                $(".list_HHCHNhapTQ").hide();
                $(".list_HHTonKho").hide();

                self.MoiQuanTam('bán hàng');
                if (self.check_kieubang() == 1) {
                    self.getListReportHH_BanHang_BieuDo();
                }
                else {
                    self.getListReportHH_BanHang();
                }
            }
            else if ($(this).val() == 2) {
                $('.DateRG').show();
                $(".column-hide").show();
                $('.list_HHBanHang').hide();
                $(".list_HHLoiNhuan").show();
                $(".list_HHXuatNhapTon").hide();
                $(".list_HHKhachTheoHang").hide();
                $(".list_HHNhanVien").hide();
                $(".list_HHNCC").hide();
                $(".list_HHXuatNhapTonChiTiet").hide();
                $(".list_HHXuatHuy").hide();
                $(".list_HHTraNhapHang").hide();
                $(".list_HHCHXuatTQ ").hide();
                $(".list_HHCHXuatCT").hide();
                $(".list_HHCHNhapTQ").hide();
                $(".list_HHTonKho").hide();
                $('#BieuDo').show();
                $('.table_LoiNhuan').show();
                self.MoiQuanTam('lợi nhuận');
                if (self.check_kieubang() == 1)
                    self.getListReportHH_LoiNhuan_BieuDo();
                else
                    self.getListReportHH_LoiNhuan();
            }
            else if ($(this).val() == 3) {
                $('.DateRG').show();
                $(".column-hide").show();
                $('.list_HHBanHang').hide();
                $(".list_HHLoiNhuan").hide();
                $(".list_HHXuatNhapTon").show();
                $(".list_HHKhachTheoHang").hide();
                $(".list_HHNhanVien").hide();
                $(".list_HHNCC").hide();
                $('.showLoaiHang').hide();
                $(".list_HHXuatNhapTonChiTiet").hide();
                $(".list_HHXuatHuy").hide();
                $(".list_HHTraNhapHang").hide();
                $(".list_HHCHXuatTQ ").hide();
                $(".list_HHCHXuatCT").hide();
                $(".list_HHCHNhapTQ").hide();
                $('.showChiNhanh').hide();
                $(".list_HHTonKho").hide();
                $('#BieuDo').hide();
                $('.TrangThaiKD').show();
                self.check_kieubang('2');
                $("#BieuDo a ").removeClass("box-tab");
                $('.table_XuatNhapTon').show();
                $('#home').removeClass("active");
                $('#home123').removeClass("active");
                $('#info').addClass("active");
                self.MoiQuanTam('xuất nhập tồn');
                for (var i = 0; i < self.searchDonVi().length; i++) {
                    if (self.searchDonVi()[i].ID == _id_DonVi) {
                        self.TenChiNhanh(self.searchDonVi()[i].TenDonVi);
                    }
                }
                self.getListReportHH_XuatNhapTon();
            }
            else if ($(this).val() == 4) {
                $('.DateRG').show();
                $(".column-hide").hide();
                $('.showLoaiHang').hide()
                $('.showChiNhanh').hide();
                $(".list_HHXuatNhapTonChiTiet").show();
                $('.list_HHBanHang').hide();
                $(".list_HHLoiNhuan").hide();
                $(".list_HHXuatNhapTon").hide();
                $(".list_HHKhachTheoHang").hide();
                $(".list_HHNhanVien").hide();
                $(".list_HHNCC").hide();
                $(".list_HHXuatHuy").hide();
                $(".list_HHTraNhapHang").hide();
                $(".list_HHCHXuatTQ ").hide();
                $(".list_HHCHXuatCT").hide();
                $(".list_HHCHNhapTQ").hide();
                $(".list_HHTonKho").hide();
                $('.TrangThaiKD').show();
                $('#BieuDo').hide();
                self.check_kieubang('2');
                $("#BieuDo a ").removeClass("box-tab");
                $('.table_XuatNhapTonChiTiet').show();
                $('#home').removeClass("active");
                $('#home123').removeClass("active");
                $('#info').addClass("active");
                self.MoiQuanTam('xuất nhập tồn chi tiết');
                for (var i = 0; i < self.searchDonVi().length; i++) {
                    if (self.searchDonVi()[i].ID == _id_DonVi) {
                        self.TenChiNhanh(self.searchDonVi()[i].TenDonVi);
                    }
                }
                self.getListReportHH_XuatNhapTonChiTiet();
            }
            else if ($(this).val() == 5) {
                $('.DateRG').show();
                $('#BieuDo').hide();
                $(".column-hide").hide();
                $(".list_HHXuatNhapTonChiTiet").hide();
                $('.list_HHBanHang').hide();
                $(".list_HHLoiNhuan").hide();
                $(".list_HHXuatNhapTon").hide();
                $(".list_HHKhachTheoHang").hide();
                $(".list_HHNhanVien").hide();
                $(".list_HHNCC").hide();
                $(".list_HHXuatHuy").show();
                $(".list_HHTraNhapHang").hide();
                $(".list_HHCHXuatTQ ").hide();
                $(".list_HHCHXuatCT").hide();
                $(".list_HHCHNhapTQ").hide();
                $(".list_HHTonKho").hide();
                self.check_kieubang('2');
                $("#BieuDo a ").removeClass("box-tab");
                $('.table_XuatHuy').show();
                $('#home').removeClass("active");
                $('#home123').removeClass("active");
                $('#info').addClass("active");
                self.MoiQuanTam('xuất kho');
                self.getListReportHH_XuatHuy();
            }
            else if ($(this).val() == 6) {
                $('.DateRG').show();
                $('#BieuDo').hide();
                $(".column-hide").show();
                $('.list_HHBanHang').hide();
                $(".list_HHLoiNhuan").hide();
                $(".list_HHXuatNhapTon").hide();
                $(".list_HHKhachTheoHang").hide();
                $(".list_HHNhanVien").show();
                $(".list_HHNCC").hide();
                $(".list_HHXuatNhapTonChiTiet").hide();
                $(".list_HHXuatHuy").hide();
                $(".list_HHTraNhapHang").hide();
                $(".list_HHCHXuatTQ ").hide();
                $(".list_HHCHXuatCT").hide();
                $(".list_HHCHNhapTQ").hide();
                $(".list_HHTonKho").hide();
                self.check_kieubang('2');
                $("#BieuDo a ").removeClass("box-tab");
                $('#home123').removeClass("active");
                $('.table_NVtheohangban').show();
                $('#home').removeClass("active");
                $('#info').addClass("active");
                self.MoiQuanTam('nhân viên');
                self.getListReportHH_NhanVien();
            }
            else if ($(this).val() == 7) {
                $('.DateRG').show();
                $('#BieuDo').hide();
                self.check_kieubang('2');
                $(".column-hide").show();
                $('.list_HHBanHang').hide();
                $(".list_HHLoiNhuan").hide();
                $(".list_HHXuatNhapTon").hide();
                $(".list_HHKhachTheoHang").show();
                $(".list_HHNhanVien").hide();
                $(".list_HHNCC").hide();
                $(".list_HHXuatNhapTonChiTiet").hide();
                $(".list_HHXuatHuy").hide();
                $(".list_HHTraNhapHang").hide();
                $(".list_HHCHXuatTQ ").hide();
                $(".list_HHCHXuatCT").hide();
                $(".list_HHCHNhapTQ").hide();
                $(".list_HHTonKho").hide();
                $('#BieuDo').hide();
                $("#BieuDo a ").removeClass("box-tab");
                $('.table_KHtheohangban').show();
                $('#home').removeClass("active");
                $('#home123').removeClass("active");
                $('#info').addClass("active");
                self.MoiQuanTam('khách hàng');
                self.getListReportHH_KhachHang();

            }
            else if ($(this).val() == 9) {
                //$('.showChiNhanh').hide();
                $('.DateRG').show();
                $(".column-hide").hide();
                $('.list_HHBanHang').hide();
                $(".list_HHLoiNhuan").hide();
                $(".list_HHXuatNhapTon").hide();
                $(".list_HHKhachTheoHang").hide();
                $(".list_HHNhanVien").hide();
                $(".list_HHNCC").hide();
                $(".list_HHXuatNhapTonChiTiet").hide();
                $(".list_HHTraNhapHang").hide();
                $(".list_HHXuatHuy").hide();
                $(".list_HHCHXuatTQ ").show();
                $(".list_HHCHXuatCT").hide();
                $(".list_HHCHNhapTQ").hide();
                $(".list_HHTonKho").hide();
                self.check_kieubang('2');
                $("#BieuDo a ").removeClass("box-tab");
                //$('.table_ChuyenHang').show();
                $('.Tab_ChuyenHang').show();
                $('#home').removeClass("active");
                $('#home123').removeClass("active");
                $('#info').addClass("active");
                self.MoiQuanTam('chuyển hàng');
                //for (var i = 0; i < self.searchDonVi().length; i++) {
                //    if (self.searchDonVi()[i].ID == _id_DonVi) {
                //        self.TenChiNhanh(self.searchDonVi()[i].TenDonVi);
                //    }
                //}
                if (dk_tab == 1)
                    self.selectXuatChuyenHang();
                //if (_selectTab == 1)
                //    self.getListReportHH_ChuyenHang();
                //else
                //    self.getListReportHangHoa_HoaDonXuatChuyenHang()
                else
                    self.selectNhapChuyenHang();
                //if (_selectTab == 1)
                //    self.getListReportHH_NhapChuyenHang();
                //else
                //    self.getListReportHangHoa_HoaDonNhapChuyenHang()
            }
            else if ($(this).val() == 10) {
                $('.DateRG').show();
                $('#BieuDo').hide();
                $(".column-hide").hide();
                $('.list_HHBanHang').hide();
                $(".list_HHLoiNhuan").hide();
                $(".list_HHXuatNhapTon").hide();
                $(".list_HHKhachTheoHang").hide();
                $(".list_HHNhanVien").hide();
                $(".list_HHNCC").hide();
                $(".list_HHXuatNhapTonChiTiet").hide();
                $(".list_HHXuatHuy").hide();
                $(".list_HHTraNhapHang").show();
                $(".list_HHCHXuatTQ ").hide();
                $(".list_HHCHXuatCT").hide();
                $(".list_HHCHNhapTQ").hide();
                $(".list_HHTonKho").hide();

                $('.table_NTNhap').hide();
                $('.table_NTNhapCT').hide();
                $('.table_NTTraCT').hide();
                $('.table_NTTra').hide();

                self.check_kieubang('2');
                $("#BieuDo a ").removeClass("box-tab");
                $('.table_TraHangNhap').show();
                $('#home').removeClass("active");
                $('#home123').removeClass("active");
                $('#info').addClass("active");
                if (dk_tabncc == 1) {
                    if (_selectTabncc == 1) {
                        $('.table_NTNhap').show();
                        self.getListReportHH_NhapHangNCC();
                    }
                    else {
                        $('.table_NTNhapCT').show();
                        self.getListReportHH_NhapHangChiTietNCC();
                    }
                }
                else {
                    if (_selectTabncc == 1) {
                        $('.table_NTTra').show();
                        self.getListReportHH_TraHangNhap();
                    }

                    else {
                        $('.table_NTTraCT').show();
                        self.getListReportHH_TraHangNhapChiTiet();
                    }
                }
            }
            else if ($(this).val() == 11) {
                $('.showChiNhanh').hide();
                $(".column-hide").hide();
                $(".column-hide").hide();
                $('.list_HHBanHang').hide();
                $(".list_HHLoiNhuan").hide();
                $(".list_HHXuatNhapTon").hide();
                $(".list_HHKhachTheoHang").hide();
                $(".list_HHNhanVien").hide();
                $(".list_HHNCC").hide();
                $(".list_HHXuatNhapTonChiTiet").hide();
                $(".list_HHXuatHuy").hide();
                $(".list_HHTraNhapHang").hide();
                $(".list_HHCHXuatTQ ").hide();
                $(".list_HHCHXuatCT").hide();
                $(".list_HHCHNhapTQ").hide();
                $(".list_HHTonKho").show();
                $('.showLoaiHang').hide()
                $('#BieuDo').hide();
                self.check_kieubang('2');
                $("#BieuDo a ").removeClass("box-tab");
                $('.Tab_HangNhapKho').show();
                $('#home').removeClass("active");
                $('#home123').removeClass("active");
                $('#info').addClass("active");
                self.MoiQuanTam('tồn kho');
                $('.TrangThaiKD').show();
                if (dk_tabtk == 1) {
                    $('.DatePK').show();
                    self.getListReportHangHoa_TonKho();
                }
                else if (dk_tabtk == 2) {
                    $('.DateRG').show();
                    if (_selectTabtk == 1)
                        self.getListReportHangHoa_TongHopNhapKho();
                    else
                        self.getListReportHangHoa_TongHopNhapKhoChiTiet()
                }
                else {
                    $('.DateRG').show();
                    if (_selectTabtk == 1)
                        self.getListReportHangHoa_TongHopXuatKho();
                    else
                        self.getListReportHangHoa_TongHopXuatKhoChiTiet()
                }
            }
            else {
                $('.DateRG').show();
                $('#BieuDo').hide();
                $(".column-hide").show();
                $('.list_HHBanHang').hide();
                $(".list_HHLoiNhuan").hide();
                $(".list_HHXuatNhapTon").hide();
                $(".list_HHKhachTheoHang").hide();
                $(".list_HHNhanVien").hide();
                $(".list_HHNCC").show();
                $(".list_HHCHXuatTQ ").hide();
                $(".list_HHCHXuatCT").hide();
                $(".list_HHCHNhapTQ").hide();
                $(".list_HHTonKho").hide();
                self.check_kieubang('2');
                $("#BieuDo a ").removeClass("box-tab");
                $('.table_NCCtheohangnhap').show();
                $('#home').removeClass("active");
                $('#home123').removeClass("active");
                $('#info').addClass("active");
                self.MoiQuanTam('Nhà cung cấp');
                self.getListReportHH_NhaCungCap();
            }

        })
        $('.chose_TinhTrangKD input').on('click', function() {
            TinhTrangHH = $(this).val();
            _pageNumber = 1;
            self.Loc_TinhTrangKD($(this).val());
            if (_kieubang == 11) {
                if (dk_tabtk == 1) {
                    self.getListReportHangHoa_TonKho();
                }
                else if (dk_tabtk == 2) {
                    if (_selectTabtk == 1)
                        self.getListReportHangHoa_TongHopNhapKho();
                    else
                        self.getListReportHangHoa_TongHopNhapKhoChiTiet()
                }
                else {
                    if (_selectTabtk == 1)
                        self.getListReportHangHoa_TongHopXuatKho();
                    else
                        self.getListReportHangHoa_TongHopXuatKhoChiTiet()
                }
            }
            else if (_kieubang == 3)
                self.getListReportHH_XuatNhapTon();
            else if (_kieubang == 4)
                self.getListReportHH_XuatNhapTonChiTiet();
        });
        $('#datetimepicker_mask').keypress(function(e) {
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
                    self.TodayBC($(this).val())
                    self.getListReportHangHoa_TonKho();
                    self.ReserPage();
                }
                else {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Định dạng thời gian không chính xác!", "danger");
                }
            }
        });
        $('#datetimepicker_mask').on('change.dp', function(e) {
            dktime = $(this).val();
            thisDate = $(this).val();
            var t = thisDate.split(" ");
            var t1 = t[0].split("/").reverse().join("-")
            thisDate = moment(t1).format('MM/DD/YYYY')
            _tonkhoStart = moment(new Date(thisDate)).format('YYYY-MM-DD');
            var dt = new Date(thisDate);
            _tonkhoEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
            if (thisDate != 'Invalid date') {
                self.TodayBC($(this).val())
                self.getListReportHangHoa_TonKho();
                self.ReserPage();
            }
            else {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Định dạng thời gian không chính xác!", "danger");
            }
        });
        self.hideTableReport = function() {
            $('.table_BanHang').hide();
            $('.table_LoiNhuan').hide();
            $('.table_XuatNhapTon').hide();
            $('.table_XuatNhapTonChiTiet').hide();
            $(".table_tonkho").hide();
            $('.table_XuatHuy').hide();
            $(".Tab_HangXuatKho").hide();
            $(".Tab_HangNhapKho").hide();
            $('.table_NVtheohangban').hide();
            $('.table_KHtheohangban').hide();
            $('.table_NCCtheohangnhap').hide();
            //$('.table_ChuyenHang').hide();
            //$('.table_NhapHang').hide();
            $('.Tab_ChuyenHang').hide();
            $('.table_TraHangNhap').hide();
            $('.DateRG').hide();
            $('.DatePK').hide();
            $('.TrangThaiKD').hide();
        }
        self.hideTableReport();
        $('.table_BanHang').show();
        $('.DateRG').show();
        //Select time report
        var _rdTime = 1;
        $('.ip_DateReport').attr('disabled', 'false');
        //$('.ip_DateReport').addClass("StartImport");
        $('.choose_TimeReport input').on('click', function() {
            _rdTime = $(this).val()
            if ($(this).val() == 1) {
                $('.ip_TimeReport').removeAttr('disabled');
                $('.dr_TimeReport').attr("data-toggle", "dropdown");
                //$('.ip_TimeReport').removeClass("StartImport");
                $('.ip_DateReport').attr('disabled', 'false');
                // $('.ip_DateReport').addClass("StartImport");
                self.TodayBC($('.ip_TimeReport').val())
                var _rdoNgayPage = $('.ip_TimeReport').val();
                var datime = new Date();
                //Toàn thời gian
                if (_rdoNgayPage === "Toàn thời gian") {
                    _timeStart = '2015-09-26'
                    _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
                }
                //Hôm nay
                else if (_rdoNgayPage === "Hôm nay") {
                    _timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
                    _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
                }
                //Hôm qua
                else if (_rdoNgayPage === "Hôm qua") {
                    var dt1 = new Date();
                    var dt2 = new Date();
                    _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
                    _timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
                }
                //Tuần này
                else if (_rdoNgayPage === "Tuần này") {
                    var currentWeekDay = datime.getDay();
                    var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1
                    _timeStart = moment(new Date(datime.setDate(datime.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
                    _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 7))).format('YYYY-MM-DD'); // end of week
                }
                //Tuần trước
                else if (_rdoNgayPage === "Tuần trước") {
                    _timeEnd = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() + 1))).format('YYYY-MM-DD');
                    _timeStart = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() - 6))).format('YYYY-MM-DD');
                }
                //7 ngày qua
                else if (_rdoNgayPage === "7 ngày qua") {
                    _timeStart = moment(new Date(datime.setDate(datime.getDate() - 6))).format('YYYY-MM-DD');
                    var newtime = new Date();
                    _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                }
                //Tháng này
                else if (_rdoNgayPage === "Tháng này") {
                    _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                    _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
                }
                //Tháng trước
                else if (_rdoNgayPage === "Tháng trước") {
                    _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
                    _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                }
                //30 ngày qua
                else if (_rdoNgayPage === "30 ngày qua") {
                    _timeStart = moment(new Date(datime.setDate(datime.getDate() - 29))).format('YYYY-MM-DD');
                    var newtime = new Date();
                    _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                }
                //Quý này
                else if (_rdoNgayPage === "Quý này") {
                    _timeStart = moment().startOf('quarter').format('YYYY-MM-DD');
                    var newtime = new Date(moment().endOf('quarter'));
                    _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                }
                // Quý trước
                else if (_rdoNgayPage === "Quý trước") {
                    var prevQuarter = (moment().quarter() - 1 === 0) ? 1 : moment().quarter() - 1;
                    _timeStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                    var newtime = new Date(moment().quarter(prevQuarter).endOf('quarter'));
                    _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                }
                //Năm này
                else if (_rdoNgayPage === "Năm này") {
                    _timeStart = moment().startOf('year').format('YYYY-MM-DD');
                    var newtime = new Date(moment().endOf('year'));
                    _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                }
                //năm trước
                else if (_rdoNgayPage === "Năm trước") {
                    var prevYear = moment().year() - 1;
                    _timeStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                    var newtime = new Date(moment().year(prevYear).endOf('year'));
                    _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                }
                _pageNumber = 1;
                if (_kieubang == 1) {
                    if (self.check_kieubang() == 1) {
                        self.getListReportHH_BanHang_BieuDo();
                    }
                    else {
                        self.getListReportHH_BanHang();
                    }
                }
                else if (_kieubang == 2) {
                    if (self.check_kieubang() == 1)
                        self.getListReportHH_LoiNhuan_BieuDo();
                    else
                        self.getListReportHH_LoiNhuan();
                }
                else if (_kieubang == 3)
                    self.getListReportHH_XuatNhapTon();
                else if (_kieubang == 4)
                    self.getListReportHH_XuatNhapTonChiTiet();
                else if (_kieubang == 5)
                    self.getListReportHH_XuatHuy();
                else if (_kieubang == 6)
                    self.getListReportHH_NhanVien();
                else if (_kieubang == 7)
                    self.getListReportHH_KhachHang();
                else if (_kieubang == 8)
                    self.getListReportHH_NhaCungCap();
                else if (_kieubang == 9) {
                    if (dk_tab == 1)
                        self.selectXuatChuyenHang();
                    else
                        self.selectNhapChuyenHang();
                }
                else if (_kieubang == 10)
                    if (dk_tabncc == 1) {
                        if (_selectTabncc == 1)
                            self.getListReportHH_NhapHangNCC();
                        else
                            self.getListReportHH_NhapHangChiTietNCC();
                    }
                    else {
                        if (_selectTabncc == 1)
                            self.getListReportHH_TraHangNhap();
                        else
                            self.getListReportHH_TraHangNhapChiTiet();
                    }
                else if (_kieubang == 11) {
                    if (dk_tabtk == 1)
                        self.getListReportHangHoa_TonKho();
                    else if (dk_tabtk == 2)
                        if (_selectTabtk == 1)
                            self.getListReportHangHoa_TongHopNhapKho();
                        else
                            self.getListReportHangHoa_TongHopNhapKhoChiTiet()
                    else
                        if (_selectTabtk == 1)
                            self.getListReportHangHoa_TongHopXuatKho();
                        else
                            self.getListReportHangHoa_TongHopXuatKhoChiTiet()
                }
                self.ReserPage();
            }
            else if ($(this).val() == 2) {
                $('.ip_DateReport').removeAttr('disabled');
                $('.ip_TimeReport').attr('disabled', 'false');
                $('.dr_TimeReport').removeAttr('data-toggle');
                if ($('.ip_DateReport').val() != "") {
                    thisDate = $('.ip_DateReport').val();
                    var t = thisDate.split("-");
                    var t1 = t[0].trim().split("/").reverse().join("-")
                    var thisDateStart = moment(t1).format('MM/DD/YYYY')
                    var t2 = t[1].trim().split("/").reverse().join("-")
                    var thisDateEnd = moment(t2).format('MM/DD/YYYY')
                    _timeStart = moment(new Date(thisDateStart)).format('YYYY-MM-DD');
                    var dt = new Date(thisDateEnd);
                    _timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
                    self.TodayBC($('.ip_DateReport').val())
                    _pageNumber = 1;
                    if (_kieubang == 1) {
                        if (self.check_kieubang() == 1) {
                            self.getListReportHH_BanHang_BieuDo();
                        }
                        else {
                            self.getListReportHH_BanHang();
                        }
                    }
                    else if (_kieubang == 2) {
                        if (self.check_kieubang() == 1)
                            self.getListReportHH_LoiNhuan_BieuDo();
                        else
                            self.getListReportHH_LoiNhuan();
                    }
                    else if (_kieubang == 3)
                        self.getListReportHH_XuatNhapTon();
                    else if (_kieubang == 4)
                        self.getListReportHH_XuatNhapTonChiTiet();
                    else if (_kieubang == 5)
                        self.getListReportHH_XuatHuy();
                    else if (_kieubang == 6)
                        self.getListReportHH_NhanVien();
                    else if (_kieubang == 7)
                        self.getListReportHH_KhachHang();
                    else if (_kieubang == 8)
                        self.getListReportHH_NhaCungCap();
                    else if (_kieubang == 9) {
                        if (dk_tab == 1)
                            self.selectXuatChuyenHang();
                        else
                            self.selectNhapChuyenHang();
                    }
                    else if (_kieubang == 10)
                        if (dk_tabncc == 1) {
                            if (_selectTabncc == 1)
                                self.getListReportHH_NhapHangNCC();
                            else
                                self.getListReportHH_NhapHangChiTietNCC();
                        }
                        else {
                            if (_selectTabncc == 1)
                                self.getListReportHH_TraHangNhap();
                            else
                                self.getListReportHH_TraHangNhapChiTiet();
                        }
                    else if (_kieubang == 11) {
                        if (dk_tabtk == 1)
                            self.getListReportHangHoa_TonKho();
                        else if (dk_tabtk == 2)
                            if (_selectTabtk == 1)
                                self.getListReportHangHoa_TongHopNhapKho();
                            else
                                self.getListReportHangHoa_TongHopNhapKhoChiTiet()
                        else
                            if (_selectTabtk == 1)
                                self.getListReportHangHoa_TongHopXuatKho();
                            else
                                self.getListReportHangHoa_TongHopXuatKhoChiTiet()
                    }
                }
            }
        })
        $('.newDateTime').on('apply.daterangepicker', function(ev, picker) {
            $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
            console.log(picker.startDate.format('DD/MM/YYYY'), picker.endDate.format('DD/MM/YYYY'))
            LoaiBieuDo = 2;
            var dt = new Date(picker.endDate.format('MM/DD/YYYY'));
            _timeStart = picker.startDate.format('YYYY-MM-DD');
            _timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');// picker.endDate.format('YYYY-MM-DD');
            console.log(_timeStart, _timeEnd);
            self.TodayBC($(this).val())
            _pageNumber = 1;
            if (_kieubang == 1) {
                if (self.check_kieubang() == 1) {
                    self.getListReportHH_BanHang_BieuDo();
                }
                else {
                    self.getListReportHH_BanHang();
                }
            }
            else if (_kieubang == 2) {
                if (self.check_kieubang() == 1)
                    self.getListReportHH_LoiNhuan_BieuDo();
                else
                    self.getListReportHH_LoiNhuan();
            }
            else if (_kieubang == 3)
                self.getListReportHH_XuatNhapTon();
            else if (_kieubang == 4)
                self.getListReportHH_XuatNhapTonChiTiet();
            else if (_kieubang == 5)
                self.getListReportHH_XuatHuy();
            else if (_kieubang == 6)
                self.getListReportHH_NhanVien();
            else if (_kieubang == 7)
                self.getListReportHH_KhachHang();
            else if (_kieubang == 8)
                self.getListReportHH_NhaCungCap();
            else if (_kieubang == 9) {
                if (dk_tab == 1)
                    self.selectXuatChuyenHang();
                else
                    self.selectNhapChuyenHang();
            }
            else if (_kieubang == 10)
                if (dk_tabncc == 1) {
                    if (_selectTabncc == 1)
                        self.getListReportHH_NhapHangNCC();
                    else
                        self.getListReportHH_NhapHangChiTietNCC();
                }
                else {
                    if (_selectTabncc == 1)
                        self.getListReportHH_TraHangNhap();
                    else
                        self.getListReportHH_TraHangNhapChiTiet();
                }
            else if (_kieubang == 11) {
                if (dk_tabtk == 1)
                    self.getListReportHangHoa_TonKho();
                else if (dk_tabtk == 2)
                    if (_selectTabtk == 1)
                        self.getListReportHangHoa_TongHopNhapKho();
                    else
                        self.getListReportHangHoa_TongHopNhapKhoChiTiet()
                else
                    if (_selectTabtk == 1)
                        self.getListReportHangHoa_TongHopXuatKho();
                    else
                        self.getListReportHangHoa_TongHopXuatKhoChiTiet()
            }
            //self.ReserPage();
        });
        $('#txtDate').on('dp.change', function(e) {
            thisDate = $(this).val();
            var t = thisDate.split(" ");
            var t1 = t[0].split("/").reverse().join("-")
            thisDate = moment(t1).format('MM/DD/YYYY')
            //console.log(thisDate);
            _timeStart = moment(new Date(thisDate)).format('YYYY-MM-DD');
            var dt = new Date(thisDate);
            _timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
            self.TodayBC($(this).val())
            _pageNumber = 1;
            if (_kieubang == 1) {
                if (self.check_kieubang() == 1) {
                    self.getListReportHH_BanHang_BieuDo();
                }
                else {
                    self.getListReportHH_BanHang();
                }
            }
            else if (_kieubang == 2) {
                if (self.check_kieubang() == 1)
                    self.getListReportHH_LoiNhuan_BieuDo();
                else
                    self.getListReportHH_LoiNhuan();
            }
            else if (_kieubang == 3)
                self.getListReportHH_XuatNhapTon();
            else if (_kieubang == 4)
                self.getListReportHH_XuatNhapTonChiTiet();
            else if (_kieubang == 5)
                self.getListReportHH_XuatHuy();
            else if (_kieubang == 6)
                self.getListReportHH_NhanVien();
            else if (_kieubang == 7)
                self.getListReportHH_KhachHang();
            else if (_kieubang == 8)
                self.getListReportHH_NhaCungCap();
            else if (_kieubang == 9) {
                if (dk_tab == 1)
                    self.selectXuatChuyenHang();
                else
                    self.selectNhapChuyenHang();

                //if (dk_tab == 1)
                //    self.getListReportHH_ChuyenHang();
                //else
                //    self.getListReportHH_NhapChuyenHang();
            }
            else if (_kieubang == 10)
                if (dk_tabncc == 1) {
                    if (_selectTabncc == 1)
                        self.getListReportHH_NhapHangNCC();
                    else
                        self.getListReportHH_NhapHangChiTietNCC();
                }
                else {
                    if (_selectTabncc == 1)
                        self.getListReportHH_TraHangNhap();
                    else
                        self.getListReportHH_TraHangNhapChiTiet();
                }
            else if (_kieubang == 11) {
                if (dk_tabtk == 1)
                    self.getListReportHangHoa_TonKho();
                else if (dk_tabtk == 2)
                    if (_selectTabtk == 1)
                        self.getListReportHangHoa_TongHopNhapKho();
                    else
                        self.getListReportHangHoa_TongHopNhapKhoChiTiet()
                else
                    if (_selectTabtk == 1)
                        self.getListReportHangHoa_TongHopXuatKho();
                    else
                        self.getListReportHangHoa_TongHopXuatKhoChiTiet()
            }
        });

        $('.choose_txtTime li').on('click', function() {
            self.TodayBC($(this).text())
            var _rdoNgayPage = $(this).val();
            var datime = new Date();
            //Toàn thời gian
            if (_rdoNgayPage === 13) {
                _timeStart = '2015-09-26'
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //Hôm nay
            else if (_rdoNgayPage === 1) {
                _timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //Hôm qua
            else if (_rdoNgayPage === 2) {
                var dt1 = new Date();
                var dt2 = new Date();
                _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
                _timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
            }
            //Tuần này
            else if (_rdoNgayPage === 3) {
                var currentWeekDay = datime.getDay();
                var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1
                _timeStart = moment(new Date(datime.setDate(datime.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 7))).format('YYYY-MM-DD'); // end of week
            }
            //Tuần trước
            else if (_rdoNgayPage === 4) {
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() + 1))).format('YYYY-MM-DD');
                _timeStart = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() - 6))).format('YYYY-MM-DD');
            }
            //7 ngày qua
            else if (_rdoNgayPage === 5) {
                _timeStart = moment(new Date(datime.setDate(datime.getDate() - 6))).format('YYYY-MM-DD');
                var newtime = new Date();
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //Tháng này
            else if (_rdoNgayPage === 6) {
                _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
            }
            //Tháng trước
            else if (_rdoNgayPage === 7) {
                _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
                _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
            }
            //30 ngày qua
            else if (_rdoNgayPage === 8) {
                _timeStart = moment(new Date(datime.setDate(datime.getDate() - 29))).format('YYYY-MM-DD');
                var newtime = new Date();
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //Quý này
            else if (_rdoNgayPage === 9) {
                _timeStart = moment().startOf('quarter').format('YYYY-MM-DD');
                var newtime = new Date(moment().endOf('quarter'));
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            // Quý trước
            else if (_rdoNgayPage === 10) {
                var prevQuarter = (moment().quarter() - 1 === 0) ? 1 : moment().quarter() - 1;
                _timeStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                var newtime = new Date(moment().quarter(prevQuarter).endOf('quarter'));
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //Năm này
            else if (_rdoNgayPage === 11) {
                _timeStart = moment().startOf('year').format('YYYY-MM-DD');
                var newtime = new Date(moment().endOf('year'));
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //năm trước
            else if (_rdoNgayPage === 12) {
                var prevYear = moment().year() - 1;
                _timeStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                var newtime = new Date(moment().year(prevYear).endOf('year'));
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            _pageNumber = 1;
            if (_kieubang == 1) {
                if (self.check_kieubang() == 1) {
                    self.getListReportHH_BanHang_BieuDo();
                }
                else {
                    self.getListReportHH_BanHang();
                }
            }
            else if (_kieubang == 2) {
                if (self.check_kieubang() == 1)
                    self.getListReportHH_LoiNhuan_BieuDo();
                else
                    self.getListReportHH_LoiNhuan();
            }
            else if (_kieubang == 3)
                self.getListReportHH_XuatNhapTon();
            else if (_kieubang == 4)
                self.getListReportHH_XuatNhapTonChiTiet();
            else if (_kieubang == 5)
                self.getListReportHH_XuatHuy();
            else if (_kieubang == 6)
                self.getListReportHH_NhanVien();
            else if (_kieubang == 7)
                self.getListReportHH_KhachHang();
            else if (_kieubang == 8)
                self.getListReportHH_NhaCungCap();
            else if (_kieubang == 9) {
                if (dk_tab == 1)
                    self.selectXuatChuyenHang();
                else
                    self.selectNhapChuyenHang();
                //if (dk_tab == 1)
                //    self.getListReportHH_ChuyenHang();
                //else
                //    self.getListReportHH_NhapChuyenHang();
            }
            else if (_kieubang == 10)
                if (dk_tabncc == 1) {
                    if (_selectTabncc == 1)
                        self.getListReportHH_NhapHangNCC();
                    else
                        self.getListReportHH_NhapHangChiTietNCC();
                }
                else {
                    if (_selectTabncc == 1)
                        self.getListReportHH_TraHangNhap();
                    else
                        self.getListReportHH_TraHangNhapChiTiet();
                }
            else if (_kieubang == 11) {
                if (dk_tabtk == 1)
                    self.getListReportHangHoa_TonKho();
                else if (dk_tabtk == 2)
                    if (_selectTabtk == 1)
                        self.getListReportHangHoa_TongHopNhapKho();
                    else
                        self.getListReportHangHoa_TongHopNhapKhoChiTiet()
                else
                    if (_selectTabtk == 1)
                        self.getListReportHangHoa_TongHopXuatKho();
                    else
                        self.getListReportHangHoa_TongHopXuatKhoChiTiet()
            }
            self.ReserPage();
        })
        // Select LoaiHang
        $('.choose_LoaiHang input').on('click', function() {
            if ($(this).val() == 1) {
                if (_ckHangHoa == 1 & _ckDichVu == 1) {
                    _ckHangHoa = 0;
                    _laHangHoa = 0;
                }
                else if (_ckHangHoa == 0 & _ckDichVu == 1) {
                    _ckHangHoa = 1;
                    _laHangHoa = 2;
                }
                else if (_ckHangHoa == 1 & _ckDichVu == 0) {
                    _ckHangHoa = 0;
                    _laHangHoa = 3;
                }
                else if (_ckHangHoa == 0 & _ckDichVu == 0) {
                    _ckHangHoa = 1;
                    _laHangHoa = 1;
                }
            }
            if ($(this).val() == 2) {
                if (_ckHangHoa == 1 & _ckDichVu == 1) {
                    _ckDichVu = 0;
                    _laHangHoa = 1;
                }
                else if (_ckHangHoa == 1 & _ckDichVu == 0) {
                    _ckDichVu = 1;
                    _laHangHoa = 2;
                }
                else if (_ckHangHoa == 0 & _ckDichVu == 1) {
                    _ckDichVu = 0;
                    _laHangHoa = 3;
                }
                else if (_ckHangHoa == 0 & _ckDichVu == 0) {
                    _ckDichVu = 1;
                    _laHangHoa = 0;
                }
            }
            console.log(_laHangHoa);
            _pageNumber = 1;
            if (_kieubang == 1) {
                if (self.check_kieubang() == 1) {
                    self.getListReportHH_BanHang_BieuDo();
                }
                else {
                    self.getListReportHH_BanHang();
                }
            }
            else if (_kieubang == 2) {
                if (self.check_kieubang() == 1)
                    self.getListReportHH_LoiNhuan_BieuDo();
                else
                    self.getListReportHH_LoiNhuan();
            }
            else if (_kieubang == 3)
                self.getListReportHH_XuatNhapTon();
            else if (_kieubang == 4)
                self.getListReportHH_XuatNhapTonChiTiet();
            else if (_kieubang == 5)
                self.getListReportHH_XuatHuy();
            else if (_kieubang == 6)
                self.getListReportHH_NhanVien();
            else if (_kieubang == 7)
                self.getListReportHH_KhachHang();
            else if (_kieubang == 8)
                self.getListReportHH_NhaCungCap();
            else if (_kieubang == 9) {
                if (dk_tab == 1)
                    self.selectXuatChuyenHang();
                else
                    self.selectNhapChuyenHang();
            }
            else if (_kieubang == 10)
                if (dk_tabncc == 1) {
                    if (_selectTabncc == 1)
                        self.getListReportHH_NhapHangNCC();
                    else
                        self.getListReportHH_NhapHangChiTietNCC();
                }
                else {
                    if (_selectTabncc == 1)
                        self.getListReportHH_TraHangNhap();
                    else
                        self.getListReportHH_TraHangNhapChiTiet();
                }
            else if (_kieubang == 11) {
                if (dk_tabtk == 1)
                    self.getListReportHangHoa_TonKho();
                else if (dk_tabtk == 2)
                    if (_selectTabtk == 1)
                        self.getListReportHangHoa_TongHopNhapKho();
                    else
                        self.getListReportHangHoa_TongHopNhapKhoChiTiet()
                else
                    if (_selectTabtk == 1)
                        self.getListReportHangHoa_TongHopXuatKho();
                    else
                        self.getListReportHangHoa_TongHopXuatKhoChiTiet()
            }
        })
        // Key Event maHH
        self.SelectMaHH = function() {
            _maHH = $('#txtMaHH').val();
            console.log(_maHH);
        }
        $('#txtMaHH').keypress(function(e) {
            if (e.keyCode == 13) {
                _pageNumber = 1;
                console.log(_kieubang)
                if (_kieubang == 1) {
                    if (self.check_kieubang() == 1) {
                        self.getListReportHH_BanHang_BieuDo();
                    }
                    else {
                        self.getListReportHH_BanHang();
                    }
                }
                else if (_kieubang == 2) {
                    if (self.check_kieubang() == 1)
                        self.getListReportHH_LoiNhuan_BieuDo();
                    else
                        self.getListReportHH_LoiNhuan();
                }
                else if (_kieubang == 3)
                    self.getListReportHH_XuatNhapTon();
                else if (_kieubang == 4)
                    self.getListReportHH_XuatNhapTonChiTiet();
                else if (_kieubang == 5)
                    self.getListReportHH_XuatHuy();
                else if (_kieubang == 6)
                    self.getListReportHH_NhanVien();
                else if (_kieubang == 7)
                    self.getListReportHH_KhachHang();
                else if (_kieubang == 8)
                    self.getListReportHH_NhaCungCap();
                else if (_kieubang == 9) {
                    if (dk_tab == 1)
                        self.selectXuatChuyenHang();
                    else
                        self.selectNhapChuyenHang();
                }
                else if (_kieubang == 10)
                    if (dk_tabncc == 1) {
                        if (_selectTabncc == 1)
                            self.getListReportHH_NhapHangNCC();
                        else
                            self.getListReportHH_NhapHangChiTietNCC();
                    }
                    else {
                        if (_selectTabncc == 1)
                            self.getListReportHH_TraHangNhap();
                        else
                            self.getListReportHH_TraHangNhapChiTiet();
                    }
                else if (_kieubang == 11) {
                    if (dk_tabtk == 1)
                        self.getListReportHangHoa_TonKho();
                    else if (dk_tabtk == 2)
                        if (_selectTabtk == 1)
                            self.getListReportHangHoa_TongHopNhapKho();
                        else
                            self.getListReportHangHoa_TongHopNhapKhoChiTiet()
                    else
                        if (_selectTabtk == 1)
                            self.getListReportHangHoa_TongHopXuatKho();
                        else
                            self.getListReportHangHoa_TongHopXuatKhoChiTiet()
                }
            }
        })
        //GetListNhomHangHoa
        //function getNhomHangHoa() {
        //    ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "GetDM_NhomHangHoa", 'GET').done(function (data) {
        //        self.NhomHangHoas(data);
        //        console.log(self.NhomHangHoas());
        //    })
        //}
        //getNhomHangHoa();
        function GetAllNhomHH() {
            self.NhomHangHoas([]);
            ajaxHelper(ReportUri + "GetListID_NhomHangHoa?TenNhomHang=" + tk, 'GET').done(function(data) {
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
        GetAllNhomHH();
        self.Selected_IDNhomHang = function(item) {
            ajaxHelper(ReportUri + "getList_ID_NhomHangHoa?ID_NhomHang=" + item.ID, "GET").done(function(data) {
                console.log(data);
            });
        }
        var time = null
        self.NoteNhomHang = function() {
            clearTimeout(time);
            time = setTimeout(
                function() {
                    self.NhomHangHoas([]);
                    tk = $('#SeachNhomHang').val();
                    if (tk.trim() != '') {
                        ajaxHelper(ReportUri + "GetListID_NhomHangHoa?TenNhomHang=" + tk, 'GET').done(function(data) {
                            console.log(data);
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
                        })
                    }
                    else {
                        GetAllNhomHH();
                    }
                }, 300);
        };
        self.SelectRepoert_NhomHangHoa = function(item) {
            _ID_NhomHang = item.ID;
            console.log(_ID_NhomHang);
            if (item.ID == undefined) {
                $('.li-oo').removeClass("yellow")
                $('#tatcanhh a').css("display", "block");
                $('#tatcanhh').addClass("yellow")
            }
            else {
                $('.ss-li .li-oo').removeClass("yellow");
                $('#tatcanhh').removeClass("yellow")
                $('.li-pp').removeClass("yellow");
                $('#tatcanhh a').css("display", "none");
                $('#' + item.ID).addClass("yellow");
            }
            _pageNumber = 1;
            if (_kieubang == 1) {
                if (self.check_kieubang() == 1) {
                    self.getListReportHH_BanHang_BieuDo();
                }
                else {
                    self.getListReportHH_BanHang();
                }
            }
            else if (_kieubang == 2) {
                if (self.check_kieubang() == 1)
                    self.getListReportHH_LoiNhuan_BieuDo();
                else
                    self.getListReportHH_LoiNhuan();
            }
            else if (_kieubang == 3)
                self.getListReportHH_XuatNhapTon();
            else if (_kieubang == 4)
                self.getListReportHH_XuatNhapTonChiTiet();
            else if (_kieubang == 5)
                self.getListReportHH_XuatHuy();
            else if (_kieubang == 6)
                self.getListReportHH_NhanVien();
            else if (_kieubang == 7)
                self.getListReportHH_KhachHang();
            else if (_kieubang == 8)
                self.getListReportHH_NhaCungCap();
            else if (_kieubang == 9) {
                if (dk_tab == 1)
                    self.selectXuatChuyenHang();
                else
                    self.selectNhapChuyenHang();
            }
            else if (_kieubang == 10)
                if (dk_tabncc == 1) {
                    if (_selectTabncc == 1)
                        self.getListReportHH_NhapHangNCC();
                    else
                        self.getListReportHH_NhapHangChiTietNCC();
                }
                else {
                    if (_selectTabncc == 1)
                        self.getListReportHH_TraHangNhap();
                    else
                        self.getListReportHH_TraHangNhapChiTiet();
                }
            else if (_kieubang == 11) {
                if (dk_tabtk == 1)
                    self.getListReportHangHoa_TonKho();
                else if (dk_tabtk == 2)
                    if (_selectTabtk == 1)
                        self.getListReportHangHoa_TongHopNhapKho();
                    else
                        self.getListReportHangHoa_TongHopNhapKhoChiTiet()
                else
                    if (_selectTabtk == 1)
                        self.getListReportHangHoa_TongHopXuatKho();
                    else
                        self.getListReportHangHoa_TongHopXuatKhoChiTiet()
            }
        }
        $('.SelectALLNhomHang').on('click', function() {
            $('.SelectALLNhomHang li').addClass('SelectReport')
            $('.SelectNhomHang li').each(function() {
                $(this).removeClass('SelectReport');
            });
            _ID_NhomHang = null;
            _pageNumber = 1;
            if (_kieubang == 1) {
                if (self.check_kieubang() == 1) {
                    self.getListReportHH_BanHang_BieuDo();
                }
                else {
                    self.getListReportHH_BanHang();
                }
            }
            else if (_kieubang == 2) {
                if (self.check_kieubang() == 1)
                    self.getListReportHH_LoiNhuan_BieuDo();
                else
                    self.getListReportHH_LoiNhuan();
            }
            else if (_kieubang == 3)
                self.getListReportHH_XuatNhapTon();
            else if (_kieubang == 4)
                self.getListReportHH_XuatNhapTonChiTiet();
            else if (_kieubang == 5)
                self.getListReportHH_XuatHuy();
            else if (_kieubang == 6)
                self.getListReportHH_NhanVien();
            else if (_kieubang == 7)
                self.getListReportHH_KhachHang();
            else if (_kieubang == 8)
                self.getListReportHH_NhaCungCap();
            else if (_kieubang == 9) {
                if (dk_tab == 1)
                    self.selectXuatChuyenHang();
                else
                    self.selectNhapChuyenHang();
            }
            else if (_kieubang == 10)
                if (dk_tabncc == 1) {
                    if (_selectTabncc == 1)
                        self.getListReportHH_NhapHangNCC();
                    else
                        self.getListReportHH_NhapHangChiTietNCC();
                }
                else {
                    if (_selectTabncc == 1)
                        self.getListReportHH_TraHangNhap();
                    else
                        self.getListReportHH_TraHangNhapChiTiet();
                }
            else if (_kieubang == 11) {
                if (dk_tabtk == 1)
                    self.getListReportHangHoa_TonKho();
                else if (dk_tabtk == 2)
                    if (_selectTabtk == 1)
                        self.getListReportHangHoa_TongHopNhapKho();
                    else
                        self.getListReportHangHoa_TongHopNhapKhoChiTiet()
                else
                    if (_selectTabtk == 1)
                        self.getListReportHangHoa_TongHopXuatKho();
                    else
                        self.getListReportHangHoa_TongHopXuatKhoChiTiet()
            }
        });

        self.RefreshPrint = function() {
            self.ReportHH_BanHangPrint([]);
            self.ReportHH_LoiNhuanPrint([]);
            self.ReportHH_XuatNhapTonPrint([]);
            self.ReportHH_XuatNhapTonChiTietPrint([]);
            self.ReportHH_XuatHuyPrint([]);
            self.ReportHH_NhanVienPrint([]);
            self.ReportHH_KhachHangPrint([]);
            self.ReportHH_NhaCungCapPrint([]);

            self.ReportHH_XuatChuyenHangPrint([]);
            self.ReportHH_HDChuyenHangPrint([]);
            self.ReportHH_NhapChuyenHangPrint([]);
            self.ReportHH_TraHangNhapPrint([]);
            self.ReportHH_TonKhoPrint([]);
            self.ReportHH_TongHopNhapKhoChiTietPrint([]);
            self.ReportHH_TongHopNhapKhoPrint([]);
            self.ReportHH_TongHopXuatKhoChiTietPrint([]);
            self.ReportHH_TongHopXuatKhoPrint([]);
        }
        //GetListHangHoa_BanHang
        var _pageNumber = 1;
        var _pageSize = 10;
        var _pageNumberHangHoa = 1;
        var _pageSizeHangHoa = 10;
        var _pageNumberHangHoaNH = 1;
        var _pageSizeHangHoaNH = 10;
        var AllPage;
        var AllPageHangHoa;
        self.BH_SoLuongBan = ko.observable();
        self.BH_GiaTriBan = ko.observable();
        self.BH_SoLuongTra = ko.observable();
        self.BH_GiaTriTra = ko.observable();
        self.BH_DoanhThuThuan = ko.observable();
        self.getListReportHH_BanHang = function() {
            if (self.BCHH_BanHang() == "BCHH_BanHang") {
                $(".PhanQuyen").hide();
                self.RefreshPrint();
                hidewait('table_h');
                _pageNumber = 1;
                self.pageNumberBH(_pageNumber);
                ajaxHelper(ReportUri + "getListReportHH_BanHang?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function(data) {
                    self.ReportHH_BanHangPrint(data.LstDataPrint);
                    self.BH_SoLuongBan(data.SoLuongBan);
                    self.BH_GiaTriBan(data.GiaTriBan);
                    self.BH_SoLuongTra(data.SoLuongTra);
                    self.BH_GiaTriTra(data.GiaTriTra);
                    self.BH_DoanhThuThuan(data.DoanhThuThuan);
                    self.SumNumberPageReport(data.LstPageNumber);
                    AllPage = self.SumNumberPageReport().length;
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    $("div[id ^= 'wait']").text("");
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                $(".TongHH_BanHang").hide();
            }
        }
        self.ReportHH_BanHang = ko.computed(function(x) {
            var first = (self.pageNumberBH() - 1) * self.pageSize();
            if (self.ReportHH_BanHangPrint() !== null) {
                if (self.ReportHH_BanHangPrint().length != 0) {
                    $(".page").show();
                    $('.TongHH_BanHang').show();
                    $(".Report_Empty").hide();
                    self.RowsStart((self.pageNumberBH() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumberBH() - 1) * self.pageSize() + self.ReportHH_BanHangPrint().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.TongHH_BanHang').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.ReportHH_BanHangPrint().slice(first, first + _pageSize);
            }
            return null;
        })

        // GetListHangHoa_LoiNhuan
        self.LN_SoLuongBan = ko.observable();
        self.LN_GiaTriBan = ko.observable();
        self.LN_SoLuongTra = ko.observable();
        self.LN_GiaTriTra = ko.observable();
        self.LN_DoanhThuThuan = ko.observable();
        self.LN_TongGiaVon = ko.observable();
        self.LN_LoiNhuan = ko.observable();
        self.LN_TySuat = ko.observable();
        self.getListReportHH_LoiNhuan = function() {
            if (self.BCHH_LoiNhuan() == "BCHH_LoiNhuan") {
                $(".PhanQuyen").hide();
                self.RefreshPrint();
                hidewait('table_h');
                _pageNumber = 1;
                self.pageNumberLN(_pageNumber);
                ajaxHelper(ReportUri + "getListReportHH_LoiNhuan?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach + "&ID_NguoiDung=" + _IDDoiTuong, "GET").done(function(data) {
                    self.ReportHH_LoiNhuanPrint(data.LstDataPrint);
                    self.LN_SoLuongBan(data.SoLuongBan);
                    self.LN_GiaTriBan(data.GiaTriBan);
                    self.LN_SoLuongTra(data.SoLuongTra);
                    self.LN_GiaTriTra(data.GiaTriTra);
                    self.LN_DoanhThuThuan(data.DoanhThuThuan);
                    self.LN_TongGiaVon(data.TongGiaVon);
                    self.LN_LoiNhuan(data.LoiNhuan);
                    self.LN_TySuat(data.TySuat);
                    self.SumNumberPageReport(data.LstPageNumber);
                    AllPage = self.SumNumberPageReport().length;
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    $("div[id ^= 'wait']").text("");
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                $(".TongCongHH_LoiNhuan").hide();
            }

        }
        self.ReportHH_LoiNhuan = ko.computed(function(x) {
            var first = (self.pageNumberLN() - 1) * self.pageSize();
            if (self.ReportHH_LoiNhuanPrint() !== null) {
                if (self.ReportHH_LoiNhuanPrint().length != 0) {
                    $('.TongCongHH_LoiNhuan').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumberLN() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumberLN() - 1) * self.pageSize() + self.ReportHH_LoiNhuanPrint().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.TongCongHH_LoiNhuan').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.ReportHH_LoiNhuanPrint().slice(first, first + _pageSize);
            }
            return null;
        })
        // GetListHangHoa_XuatNhapTon
        self.SumHangHoa_TonDauKy = ko.observable();
        self.SumHangHoa_GiaTriDauKy = ko.observable();
        self.SumHangHoa_SoLuongNhap = ko.observable();
        self.SumHangHoa_GiaTriNhap = ko.observable();
        self.SumHangHoa_SoLuongXuat = ko.observable();
        self.SumHangHoa_GiaTriXuat = ko.observable();
        self.SumHangHoa_TonCuoiKy = ko.observable();
        self.SumHangHoa_GiaTriCuoiKy = ko.observable();
        self.getListReportHH_XuatNhapTon = function() {
            if (self.BCHH_XuatNhapTon() == "BCHH_XuatNhapTon") {
                $(".PhanQuyen").hide();
                self.RefreshPrint();
                hidewait('table_h');
                _pageNumber = 1;
                self.pageNumberXNTCT(_pageNumber);
                ajaxHelper(ReportUri + "getListReportHH_XuatNhapTon?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _id_DonVi + "&TinhTrang=" + TinhTrangHH + "&ID_NguoiDung=" + _IDDoiTuong, "GET").done(function (data) {
                    self.ReportHH_XuatNhapTonPrint(data.LstDataPrint);
                    self.SumHangHoa_TonDauKy(data.DoanhThu);
                    self.SumHangHoa_GiaTriDauKy(data.DoanhThuThuan);
                    self.SumHangHoa_SoLuongNhap(data.GiamGiaHD);
                    self.SumHangHoa_GiaTriNhap(data.GiaTriTra);
                    self.SumHangHoa_SoLuongXuat(data.LoiNhuanGop);
                    self.SumHangHoa_GiaTriXuat(data.TongGiaVon);
                    self.SumHangHoa_GiaTriCuoiKy(data.TongTienHang);
                    self.SumNumberPageReport(data.LstPageNumber);
                    AllPage = self.SumNumberPageReport().length;
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    $("div[id ^= 'wait']").text("");
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                $(".TongCongHH_XNT").hide();
            }

        }
        self.ReportHH_XuatNhapTon = ko.computed(function(x) {
            var first = (self.pageNumberXNT() - 1) * self.pageSize();
            if (self.ReportHH_XuatNhapTonPrint() !== null) {
                if (self.ReportHH_XuatNhapTonPrint().length != 0) {
                    $('.TongCongHH_XNT').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumberXNT() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumberXNT() - 1) * self.pageSize() + self.ReportHH_XuatNhapTonPrint().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.TongCongHH_XNT').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.ReportHH_XuatNhapTonPrint().slice(first, first + _pageSize);
            }
            return null;
        })
        // GetListHangHoa_XuatNhapTonChiTiet
        self.CT_TonDauKy = ko.observable();
        self.CT_GiaTriDauKy = ko.observable();
        self.CT_SoLuongNhap_NCC = ko.observable();
        self.CT_SoLuongNhap_Kiem = ko.observable();
        self.CT_SoLuongNhap_Tra = ko.observable();
        self.CT_SoLuongNhap_Chuyen = ko.observable();
        self.CT_SoLuongNhap_SX = ko.observable();
        self.CT_SoLuongXuat_Ban = ko.observable();
        self.CT_SoLuongXuat_Huy = ko.observable();
        self.CT_SoLuongXuat_NCC = ko.observable();
        self.CT_SoLuongXuat_Kiem = ko.observable();
        self.CT_SoLuongXuat_Chuyen = ko.observable();
        self.CT_SoLuongXuat_SX = ko.observable();
        self.CT_TonCuoiKy = ko.observable();
        self.CT_GiaTriCuoiKy = ko.observable();
        self.getListReportHH_XuatNhapTonChiTiet = function() {
            if (self.BCHH_XuatNhapTonChiTiet() == "BCHH_XuatNhapTonChiTiet") {
                self.RefreshPrint();
                hidewait('table_h');
                $(".PhanQuyen").hide();
                _pageNumber = 1;
                self.pageNumberXNTCT(_pageNumber);
                ajaxHelper(ReportUri + "getListReportHH_XuatNhapTonChiTietPRC?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _id_DonVi + "&TinhTrang=" + TinhTrangHH + "&ID_NguoiDung=" + _IDDoiTuong, "GET").done(function(data) {
                    self.ReportHH_XuatNhapTonChiTietPrint(data.LstDataPrint);
                    self.CT_TonDauKy(data.TonDauKy);
                    self.CT_GiaTriDauKy(data.GiaTriDauKy);
                    self.CT_SoLuongNhap_NCC(data.SoLuongNhap_NCC);
                    self.CT_SoLuongNhap_Kiem(data.SoLuongNhap_Kiem);
                    self.CT_SoLuongNhap_Tra(data.SoLuongNhap_Tra);
                    self.CT_SoLuongNhap_Chuyen(data.SoLuongNhap_Chuyen);
                    self.CT_SoLuongNhap_SX(data.SoLuongNhap_SX);
                    self.CT_SoLuongXuat_Ban(data.SoLuongXuat_Ban);
                    self.CT_SoLuongXuat_Huy(data.SoLuongXuat_Huy);
                    self.CT_SoLuongXuat_NCC(data.SoLuongXuat_NCC);
                    self.CT_SoLuongXuat_Kiem(data.SoLuongXuat_Kiem);
                    self.CT_SoLuongXuat_Chuyen(data.SoLuongXuat_Chuyen);
                    self.CT_SoLuongXuat_SX(data.SoLuongXuat_SX);
                    self.CT_TonCuoiKy(data.TonCuoiKy);
                    self.CT_GiaTriCuoiKy(data.GiaTriCuoiKy);
                    self.SumNumberPageReport(data.LstPageNumber);
                    AllPage = self.SumNumberPageReport().length;
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    $("div[id ^= 'wait']").text("");
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                $(".TongCongHH_XNTChiTiet").hide();
            }
        }
        self.ReportHH_XuatNhapTonChiTiet = ko.computed(function(x) {
            var first = (self.pageNumberXNTCT() - 1) * self.pageSize();
            if (self.ReportHH_XuatNhapTonChiTietPrint() !== null) {
                if (self.ReportHH_XuatNhapTonChiTietPrint().length != 0) {
                    $('.TongCongHH_XNTChiTiet').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumberXNTCT() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumberXNTCT() - 1) * self.pageSize() + self.ReportHH_XuatNhapTonChiTietPrint().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.TongCongHH_XNTChiTiet').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.ReportHH_XuatNhapTonChiTietPrint().slice(first, first + _pageSize);
            }
            return null;
        })
        // GetListHangHoa_Xuathuy
        self.getListReportHH_XuatHuy = function() {
            if (self.BCHH_XuatHuy() == "BCHH_XuatHuy") {
                self.RefreshPrint();
                hidewait('table_h');
                $(".PhanQuyen").hide();
                _pageNumber = 1;
                self.pageNumberXH(_pageNumber);
                ajaxHelper(ReportUri + "getListReportHH_XuatHuy?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach + "&ID_NguoiDung=" + _IDDoiTuong, "GET").done(function (data) {
                    self.ReportHH_XuatHuyPrint(data.LstDataPrint);
                    self.TongCongHH_SoLuongXuatHuy(data._lailo);
                    self.TongCongHH_GiaTriXuatHuy(data._thanhtien);
                    self.SumNumberPageReport(data.LstPageNumber);
                    AllPage = self.SumNumberPageReport().length;
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    $("div[id ^= 'wait']").text("");
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                $(".TongCongXuatHuy").hide();
            }
        }
        self.ReportHH_XuatHuy = ko.computed(function(x) {
            var first = (self.pageNumberXH() - 1) * self.pageSize();
            if (self.ReportHH_XuatHuyPrint() !== null) {
                if (self.ReportHH_XuatHuyPrint().length != 0) {
                    $('.TongCongXuatHuy').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumberXH() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumberXH() - 1) * self.pageSize() + self.ReportHH_XuatHuyPrint().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.TongCongXuatHuy').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.ReportHH_XuatHuyPrint().slice(first, first + _pageSize);
            }
            return null;
        })
        // GetListHangHoa_TraHangNhap
        self.TongCongTHN_SoLuong = ko.observable();
        self.TongCongTHN_GiaTri = ko.observable();
        self.ReportHH_TraHangNhap = ko.observableArray();
        self.ReportHH_TraHangNhapPrint = ko.observableArray();
        self.getListReportHH_TraHangNhap = function() {
            if (self.BCHH_TraHangNhap() == "BCHH_TraHangNhap") {
                self.RefreshPrint();
                self.MoiQuanTam('trả hàng nhập');
                hidewait('table_h');
                $(".PhanQuyen").hide();
                _pageNumber = 1;
                self.pageNumberTHN(_pageNumber);
                ajaxHelper(ReportUri + "getListReportHH_TraHangNhap?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function(data) {
                    self.ReportHH_TraHangNhapPrint(data.LstDataPrint);
                    self.TongCongTHN_SoLuong(data._lailo);
                    self.TongCongTHN_GiaTri(data._thanhtien);
                    self.SumNumberPageReport(data.LstPageNumber);
                    AllPage = self.SumNumberPageReport().length;
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    $("div[id ^= 'wait']").text("");
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                $(".TongCongTraHangNhap").hide();
            }
        }
        self.ReportHH_TraHangNhap = ko.computed(function(x) {
            var first = (self.pageNumberTHN() - 1) * self.pageSize();
            if (self.ReportHH_TraHangNhapPrint() !== null) {
                if (self.ReportHH_TraHangNhapPrint().length != 0) {
                    $('.TongCongTraHangNhap').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumberTHN() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumberTHN() - 1) * self.pageSize() + self.ReportHH_TraHangNhapPrint().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.TongCongTraHangNhap').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.ReportHH_TraHangNhapPrint().slice(first, first + _pageSize);
            }
            return null;
        })

        self.TongCongNHNCC_SoLuong = ko.observable();
        self.TongCongNHNCC_GiaTri = ko.observable();
        self.ReportHH_NhapHangNCCPrint = ko.observableArray();
        self.getListReportHH_NhapHangNCC = function() {
            if (self.BCHH_TraHangNhap() == "BCHH_TraHangNhap") {
                $(".PhanQuyen").hide();
                self.RefreshPrint();
                self.MoiQuanTam('nhập hàng NCC');
                _pageNumber = 1;
                self.pageNumberNHNCC(_pageNumber);
                hidewait('table_h');
                ajaxHelper(ReportUri + "getListReportHH_NhapHangNCC?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function(data) {
                    self.ReportHH_NhapHangNCCPrint(data.LstDataPrint);
                    self.TongCongNHNCC_SoLuong(data._lailo);
                    self.TongCongNHNCC_GiaTri(data._thanhtien);
                    self.SumNumberPageReport(data.LstPageNumber);
                    AllPage = self.SumNumberPageReport().length;
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    $("div[id ^= 'wait']").text("");
                });

            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                $(".TongCongNhapHangNCC").hide();
            }

        }
        self.ReportHH_NhapHangNCC = ko.computed(function(x) {
            var first = (self.pageNumberNHNCC() - 1) * self.pageSize();
            if (self.ReportHH_NhapHangNCCPrint() !== null) {
                if (self.ReportHH_NhapHangNCCPrint().length != 0) {
                    $('.TongCongNhapHangNCC').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumberNHNCC() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumberNHNCC() - 1) * self.pageSize() + self.ReportHH_NhapHangNCCPrint().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.TongCongNhapHangNCC').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.ReportHH_NhapHangNCCPrint().slice(first, first + _pageSize);
            }
            return null;
        })
        self.TongCongNHCTNCC_SoLuong = ko.observable();
        self.TongCongNHCTNCC_DonGia = ko.observable();
        self.TongCongNHCTNCC_GiaTri = ko.observable();
        self.ReportHH_NhapHangChiTietNCCPrint = ko.observableArray();
        self.getListReportHH_NhapHangChiTietNCC = function() {
            if (self.BCHH_TraHangNhap() == "BCHH_TraHangNhap") {
                self.RefreshPrint();
                self.MoiQuanTam('chi tiết nhập hàng NCC');
                _pageNumber = 1;
                self.pageNumberNHCTNCC(_pageNumber);
                hidewait('table_h');
                $(".PhanQuyen").hide();
                ajaxHelper(ReportUri + "getListReportHH_NhapHangChiTietNCC?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function(data) {
                    self.ReportHH_NhapHangChiTietNCCPrint(data.LstDataPrint);
                    self.TongCongNHCTNCC_SoLuong(data._soluong);
                    self.TongCongNHCTNCC_DonGia(data._tienvon);
                    self.TongCongNHCTNCC_GiaTri(data._thanhtien);
                    self.SumNumberPageReport(data.LstPageNumber);
                    AllPage = self.SumNumberPageReport().length;
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    $("div[id ^= 'wait']").text("");
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                $(".TongCongNhapHangChiTietNCC").hide();
            }


        }
        self.ReportHH_NhapHangChiTietNCC = ko.computed(function(x) {
            var first = (self.pageNumberNHCTNCC() - 1) * self.pageSize();
            if (self.ReportHH_NhapHangChiTietNCCPrint() !== null) {
                if (self.ReportHH_NhapHangChiTietNCCPrint().length != 0) {
                    $('.TongCongNhapHangChiTietNCC').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumberNHCTNCC() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumberNHCTNCC() - 1) * self.pageSize() + self.ReportHH_NhapHangChiTietNCCPrint().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.TongCongNhapHangChiTietNCC').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.ReportHH_NhapHangChiTietNCCPrint().slice(first, first + _pageSize);
            }
            return null;
        })

        self.TongCongTHNCT_SoLuong = ko.observable();
        self.TongCongTHNCT_DonGia = ko.observable();
        self.TongCongTHNCT_GiaTri = ko.observable();
        self.ReportHH_TraHangNhapChiTietPrint = ko.observableArray();
        self.getListReportHH_TraHangNhapChiTiet = function() {
            if (self.BCHH_TraHangNhap() == "BCHH_TraHangNhap") {
                self.RefreshPrint();
                self.MoiQuanTam('chi tiết trả hàng nhập');
                _pageNumber = 1;
                self.pageNumberTHNCT(_pageNumber);
                hidewait('table_h');
                $(".PhanQuyen").hide();
                ajaxHelper(ReportUri + "getListReportHH_TraHangNhapChiTiet?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function(data) {
                    self.ReportHH_TraHangNhapChiTietPrint(data.LstDataPrint);
                    self.TongCongTHNCT_SoLuong(data._soluong);
                    self.TongCongTHNCT_DonGia(data._tienvon);
                    self.TongCongTHNCT_GiaTri(data._thanhtien);
                    self.SumNumberPageReport(data.LstPageNumber);
                    AllPage = self.SumNumberPageReport().length;
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    $("div[id ^= 'wait']").text("");
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                $(".TongCongTraHangNhapChiTiet").hide();
            }
        }
        self.ReportHH_TraHangNhapChiTiet = ko.computed(function(x) {
            var first = (self.pageNumberTHNCT() - 1) * self.pageSize();
            if (self.ReportHH_TraHangNhapChiTietPrint() !== null) {
                if (self.ReportHH_TraHangNhapChiTietPrint().length != 0) {
                    $('.TongCongTraHangNhapChiTiet').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumberTHNCT() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumberTHNCT() - 1) * self.pageSize() + self.ReportHH_TraHangNhapChiTietPrint().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.TongCongTraHangNhapChiTiet').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.ReportHH_TraHangNhapChiTietPrint().slice(first, first + _pageSize);
            }
            return null;
        })
        // GetListHangHoa_XuatChuyenHang
        var dk_tab = 1;
        var dk_tabtk = 1;
        var dk_tabncc = 1;
        self.selectXuatChuyenHang = function() {
            $('.table_TongQuan1').hide();
            $('.table_TongQuan2').hide();
            $('.table_ChiTiet').hide();

            dk_tab = 1;
            if (_selectTab == 1) {
                $('.table_TongQuan1').show();
                self.getListReportHH_ChuyenHang();
                $(".list_HHCHXuatTQ").show();
                $(".list_HHCHXuatCT").hide();
                $(".list_HHCHNhapTQ").hide();
            }
            else {
                $('.table_ChiTiet').show();
                self.getListReportHangHoa_HoaDonXuatChuyenHang();
                $(".list_HHCHXuatTQ").hide();
                $(".list_HHCHXuatCT").show();
                $(".list_HHCHNhapTQ").hide();
            }
        }
        self.selectNhapChuyenHang = function() {
            dk_tab = 2;
            $('.table_TongQuan1').hide();
            $('.table_TongQuan2').hide();
            $('.table_ChiTiet').hide();
            $(".list_HHCHNhapTQ").show();
            if (_selectTab == 1) {
                $('.table_TongQuan2').show();
                self.getListReportHH_NhapChuyenHang();
                $(".list_HHCHNhapTQ").show();
                $(".list_HHCHXuatTQ").hide();
                $(".list_HHCHXuatCT").hide();
            }
            else {
                $('.table_ChiTiet').show();
                self.getListReportHangHoa_HoaDonNhapChuyenHang();
                $(".list_HHCHNhapTQ").hide();
                $(".list_HHCHXuatTQ").hide();
                $(".list_HHCHXuatCT").show();
            }
        }

        $('.Select_TableCH input').on('click', function() {
            self.Select_Table($(this).val())
            _selectTab = $(this).val();
            if (dk_tab == 1)
                self.selectXuatChuyenHang();
            else
                self.selectNhapChuyenHang();
        });
        self.getListReportHH_ChuyenHang = function() {
            if (self.BCHH_ChuyenHang() == "BCHH_ChuyenHang") {
                $(".PhanQuyen").hide();
                self.RefreshPrint();
                hidewait('table_h');
                //dk_tab = 1;
                _pageNumber = 1;
                self.pageNumberCH(_pageNumber);
                ajaxHelper(ReportUri + "getListReportHH_XuatChuyenHang?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach + "&ID_NguoiDung=" + _IDDoiTuong, "GET").done(function (data) {
                    self.ReportHH_XuatChuyenHangPrint(data.LstDataPrint);
                    self.TongCongCH_SoLuong(data._lailo);
                    self.TongCongCH_GiaTri(data._thanhtien);
                    self.SumNumberPageReport(data.LstPageNumber);
                    AllPage = self.SumNumberPageReport().length;
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    $("div[id ^= 'wait']").text("");
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                $(".TongCongChuyenHang").hide();
            }

        }
        self.ReportHH_XuatChuyenHang = ko.computed(function(x) {
            var first = (self.pageNumberCH() - 1) * self.pageSize();
            if (dk_tab == 1) {
                if (self.ReportHH_XuatChuyenHangPrint() !== null) {
                    if (self.ReportHH_XuatChuyenHangPrint().length != 0) {
                        $('.TongCongChuyenHang').show();
                        $(".Report_Empty").hide();
                        $(".page").show();
                        self.RowsStart((self.pageNumberCH() - 1) * self.pageSize() + 1);
                        self.RowsEnd((self.pageNumberCH() - 1) * self.pageSize() + self.ReportHH_XuatChuyenHangPrint().slice(first, first + self.pageSize()).length)
                    }
                    else {
                        $('.TongCongChuyenHang').hide();
                        $(".Report_Empty").show();
                        $(".page").hide();
                        self.RowsStart('0');
                        self.RowsEnd('0');
                    }
                    return self.ReportHH_XuatChuyenHangPrint().slice(first, first + _pageSize);
                }
                return null;
            }
            else {
                if (self.ReportHH_NhapChuyenHangPrint() !== null) {
                    if (self.ReportHH_NhapChuyenHangPrint().length != 0) {
                        $('.TongCongChuyenHang').show();
                        $(".Report_Empty").hide();
                        $(".page").show();
                        self.RowsStart((self.pageNumberCH() - 1) * self.pageSize() + 1);
                        self.RowsEnd((self.pageNumberCH() - 1) * self.pageSize() + self.ReportHH_NhapChuyenHangPrint().slice(first, first + self.pageSize()).length)
                    }
                    else {
                        $('.TongCongChuyenHang').hide();
                        $(".Report_Empty").show();
                        $(".page").hide();
                        self.RowsStart('0');
                        self.RowsEnd('0');
                    }
                    return self.ReportHH_NhapChuyenHangPrint().slice(first, first + _pageSize);
                }
                return null;
            }


        })
        self.ReportHH_HDChuyenHang = ko.observableArray();
        self.ReportHH_HDChuyenHangPrint = ko.observableArray();
        self.ReportHH_HDNhapChuyenHangPrint = ko.observableArray();
        self.TongCongHD_SoLuong = ko.observable();
        self.TongCongHD_ThanhTien = ko.observable();
        var sl_tab = 1;
        self.getListReportHangHoa_HoaDonXuatChuyenHang = function() {
            if (self.BCHH_ChuyenHang() == "BCHH_ChuyenHang") {
                $(".PhanQuyen").hide();
                self.RefreshPrint();
                hidewait('table_h');
                _pageNumber = 1;
                self.pageNumberHDCH(_pageNumber);
                ajaxHelper(ReportUri + "ReportHangHoa_HoaDonXuatChuyenHang?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function(data) {
                    self.ReportHH_HDChuyenHangPrint(data.LstDataPrint);
                    self.ReportHH_HDChuyenHang(self.ReportHH_HDChuyenHangPrint().slice(0, 0 + _pageSize));
                    self.TongCongHD_SoLuong(data._lailo);
                    self.TongCongHD_ThanhTien(data._thanhtien);
                    self.SumNumberPageReport(data.LstPageNumber);
                    AllPage = self.SumNumberPageReport().length;
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    if (self.ReportHH_HDChuyenHang().legend != 0)
                    {
                        $('.TongCongHDNChuyenHang').hide();
                        $(".TongCongHDXChuyenHang").show();
                        $(".Report_Empty").hide();
                        $(".page").show();
                        self.RowsStart(1);
                        self.RowsEnd((self.pageNumberHDCH() - 1) * self.pageSize() + self.ReportHH_HDChuyenHang().length);
                    }
                    else
                    {
                        self.RowsStart('0');
                        self.RowsEnd('0');
                        $('.TongCongHDNChuyenHang').hide();
                        $(".TongCongHDXChuyenHang").hide();
                        $(".Report_Empty").show();
                        $(".page").hide();
                    }
                    $("div[id ^= 'wait']").text("");
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                $(".TongCongHDXChuyenHang").hide();
                $(".TongCongHDNChuyenHang").hide();
            }

        }
        self.getListReportHangHoa_HoaDonNhapChuyenHang = function() {
            if (self.BCHH_ChuyenHang() == "BCHH_ChuyenHang") {
                $(".PhanQuyen").hide();
                self.RefreshPrint();
                hidewait('table_h');
                console.log(_selectTab);
                _pageNumber = 1;
                self.pageNumberHDCH(_pageNumber);
                ajaxHelper(ReportUri + "ReportHangHoa_HoaDonNhapChuyenHang?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function(data) {
                    //self.ReportHH_HDChuyenHangPrint(data.LstDataPrint);
                    self.ReportHH_HDNhapChuyenHangPrint(data.LstDataPrint);
                    self.ReportHH_HDChuyenHang(self.ReportHH_HDNhapChuyenHangPrint().slice(0, 0 + _pageSize));
                    self.TongCongHD_SoLuong(data._lailo);
                    self.TongCongHD_ThanhTien(data._thanhtien);
                    self.SumNumberPageReport(data.LstPageNumber);
                    AllPage = self.SumNumberPageReport().length;
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    if (self.ReportHH_HDChuyenHang().legend != 0) {
                        $('.TongCongHDNChuyenHang').show();
                        $(".TongCongHDXChuyenHang").hide();
                        $(".Report_Empty").hide();
                        $(".page").show();
                        self.RowsStart(1);
                        self.RowsEnd((self.pageNumberHDCH() - 1) * self.pageSize() + self.ReportHH_HDChuyenHang().length);
                    }
                    else {
                        self.RowsStart('0');
                        self.RowsEnd('0');
                        $('.TongCongHDNChuyenHang').hide();
                        $(".TongCongHDXChuyenHang").hide();
                        $(".Report_Empty").show();
                        $(".page").hide();
                    }
                    $("div[id ^= 'wait']").text("");
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                $(".TongCongHDNChuyenHang").hide();
                $(".TongCongHDXChuyenHang").hide();
            }

        }
        self.ReportHH_HDChuyenHang1 = ko.computed(function(x) {
            var first = (self.pageNumberHDCH() - 1) * self.pageSize();
            if (dk_tab == 1) {
                if (self.ReportHH_HDChuyenHangPrint() !== null) {
                    if (self.ReportHH_HDChuyenHangPrint().length != 0) {
                        $('.TongCongHDXChuyenHang').show();
                        $('.TongCongHDNChuyenHang').hide();
                        $(".Report_Empty").hide();
                        //$(".page").show();
                        self.RowsStart((self.pageNumberHDCH() - 1) * self.pageSize() + 1);
                        self.RowsEnd((self.pageNumberHDCH() - 1) * self.pageSize() + self.ReportHH_HDChuyenHangPrint().slice(first, first + self.pageSize()).length)
                    }
                    else {
                        $('.TongCongHDXChuyenHang').hide();
                        $('.TongCongHDNChuyenHang').hide();
                        $(".Report_Empty").show();
                        //$(".page").hide();
                        self.RowsStart('0');
                        self.RowsEnd('0');
                    }
                    self.ReportHH_HDChuyenHang(self.ReportHH_HDChuyenHangPrint().slice(first, first + _pageSize));
                    return self.ReportHH_HDChuyenHangPrint().slice(first, first + _pageSize);
                }
                return null;
            }
            else {
                if (self.ReportHH_HDNhapChuyenHangPrint() !== null) {
                    if (self.ReportHH_HDNhapChuyenHangPrint().length != 0) {
                        $(".Report_Empty").hide();
                        $(".page").show();
                        $('.TongCongHDNChuyenHang').hide();
                        $(".TongCongHDXChuyenHang").show();
                        self.RowsStart((self.pageNumberHDCH() - 1) * self.pageSize() + 1);
                        self.RowsEnd((self.pageNumberHDCH() - 1) * self.pageSize() + self.ReportHH_HDNhapChuyenHangPrint().slice(first, first + self.pageSize()).length)
                    }
                    else {
                        $('.TongCongHDNChuyenHang').hide();
                        $(".TongCongHDXChuyenHang").hide();
                        $(".Report_Empty").show();
                        $(".page").hide();
                        self.RowsStart('0');
                        self.RowsEnd('0');
                    }
                    self.ReportHH_HDChuyenHang(self.ReportHH_HDNhapChuyenHangPrint().slice(first, first + _pageSize));
                    return self.ReportHH_HDNhapChuyenHangPrint().slice(first, first + _pageSize);
                }
                return null;
            }
        })

        self.SumRowsHangHoaChuyenHang = ko.observable();
        self.SumNumberPageReportHangHoa = ko.observableArray();
        self.ReportHH_XuatChuyenHangChiTiet = ko.observableArray();
        var _id_HangHoaChuyenHang;
        var _maHangHoaCH;
        self.getListReportHH_XuatChuyenHangChiTiet = function(item) {

            hidewait('table_h');
            _id_HangHoaChuyenHang = item.ID;
            _maHangHoaCH = item.MaHangHoa;
            ajaxHelper(ReportUri + "getListReportHH_XuatChuyenHangChiTiet?ID_DonViQuiDoi=" + _id_HangHoaChuyenHang + "&ID_ChiNhanh=" + _idDonViSeach + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumberHangHoa + "&pageSize=" + _pageSizeHangHoa + "&ID_NguoiDung=" + _IDDoiTuong, "GET").done(function (data) {
                self.ReportHH_XuatChuyenHangChiTiet(data.LstData);
                console.log(data);
                if (self.ReportHH_XuatChuyenHangChiTiet().length != 0) {
                    self.RowsStart_CH((_pageNumberHangHoa - 1) * _pageSizeHangHoa + 1);
                    self.RowsEnd_CH((_pageNumberHangHoa - 1) * _pageSizeHangHoa + self.ReportHH_XuatChuyenHangChiTiet().length)
                }
                else {
                    self.RowsStart_CH('0');
                    self.RowsEnd_CH('0');
                }
                self.SumNumberPageReportHangHoa(data.LstPageNumber);
                AllPageHangHoa = self.SumNumberPageReportHangHoa().length;
                self.selecPageHangHoa();
                self.ReserPageHangHoa();
                self.SumRowsHangHoaChuyenHang(data.Rowcount);
                $("div[id ^= 'wait']").text("");
            });
        }
        self.SelectedPageNumberReportHH_XuatChuyenHangChiTiet = function() {
            hidewait('table_h');
            ajaxHelper(ReportUri + "getListReportHH_XuatChuyenHangChiTiet?ID_DonViQuiDoi=" + _id_HangHoaChuyenHang + "&ID_ChiNhanh=" + _idDonViSeach + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumberHangHoa + "&pageSize=" + _pageSizeHangHoa, "GET").done(function(data) {
                self.ReportHH_XuatChuyenHangChiTiet(data.LstData);
                self.RowsStart_CH((_pageNumberHangHoa - 1) * _pageSizeHangHoa + 1);
                self.RowsEnd_CH((_pageNumberHangHoa - 1) * _pageSizeHangHoa + self.ReportHH_XuatChuyenHangChiTiet().length)
                $("div[id ^= 'wait']").text("");
            });
        }

        // GetListHangHoa_NhapChuyenHang
        self.getListReportHH_NhapChuyenHang = function() {
            if (self.BCHH_ChuyenHang() == "BCHH_ChuyenHang") {
                self.RefreshPrint();
                hidewait('table_h');
                $(".PhanQuyen").hide();
                //dk_tab = 2
                _pageNumber = 1;
                self.pageNumberCH(_pageNumber);
                ajaxHelper(ReportUri + "getListReportHH_NhapChuyenHang?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach + "&ID_NguoiDung=" + _IDDoiTuong, "GET").done(function (data) {
                    self.ReportHH_NhapChuyenHangPrint(data.LstDataPrint);
                    self.TongCongNH_SoLuong(data._lailo);
                    self.TongCongNH_GiaTri(data._thanhtien);
                    self.SumNumberPageReport(data.LstPageNumber);
                    AllPage = self.SumNumberPageReport().length;
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    $("div[id ^= 'wait']").text("");
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                $(".TongCongChuyenHang").hide();
            }

        }
        self.SumRowsHangHoaNhapChuyenHang = ko.observable();
        self.SumNumberPageReportNhapHangHoa = ko.observableArray();
        self.ReportHH_NhapChuyenHangChiTiet = ko.observableArray();
        var _id_HangHoaNhapChuyenHang;
        var _maHangHoaCH;
        self.getListReportHH_NhapChuyenHangChiTiet = function(item) {
            hidewait('table_h');
            _id_HangHoaNhapChuyenHang = item.ID;
            _maHangHoaCH = item.MaHangHoa;
            ajaxHelper(ReportUri + "getListReportHH_NhapChuyenHangChiTiet?ID_DonViQuiDoi=" + _id_HangHoaNhapChuyenHang + "&ID_ChiNhanh=" + _idDonViSeach + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumberHangHoaNH + "&pageSize=" + _pageSizeHangHoaNH + "&ID_NguoiDung=" + _IDDoiTuong, "GET").done(function (data) {
                self.ReportHH_NhapChuyenHangChiTiet(data.LstData);
                if (self.ReportHH_NhapChuyenHangChiTiet().length != 0) {
                    self.RowsStart_NH((_pageNumberHangHoaNH - 1) * _pageSizeHangHoaNH + 1);
                    self.RowsEnd_NH((_pageNumberHangHoaNH - 1) * _pageSizeHangHoaNH + self.ReportHH_NhapChuyenHangChiTiet().length)
                }
                else {
                    self.RowsStart_NH('0');
                    self.RowsEnd_NH('0');
                }
                self.SumNumberPageReportNhapHangHoa(data.LstPageNumber);
                AllPageHangHoaNH = self.SumNumberPageReportNhapHangHoa().length;
                self.selecPageHangHoaNH();
                self.ReserPageHangHoaNH();
                self.SumRowsHangHoaNhapChuyenHang(data.Rowcount);
                $("div[id ^= 'wait']").text("");
            });

        }
        self.SelectedPageNumberReportHH_NhapChuyenHangChiTiet = function() {
            hidewait('table_h');
            ajaxHelper(ReportUri + "getListReportHH_NhapChuyenHangChiTiet?ID_DonViQuiDoi=" + _id_HangHoaNhapChuyenHang + "&ID_ChiNhanh=" + _idDonViSeach + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumberHangHoaNH + "&pageSize=" + _pageSizeHangHoaNH, "GET").done(function(data) {
                self.ReportHH_NhapChuyenHangChiTiet(data.LstData);
                self.RowsStart_NH((_pageNumberHangHoaNH - 1) * _pageSizeHangHoaNH + 1);
                self.RowsEnd_NH((_pageNumberHangHoaNH - 1) * _pageSizeHangHoaNH + self.ReportHH_NhapChuyenHangChiTiet().length)
                $("div[id ^= 'wait']").text("");
            });
        }
        // hàng hóa tồn kho
        $('.rdo_checkTable').hide();
        $('.NhapKhoChiTiet').hide();
        $('.NhapKhoTongQuan').hide();
        $('.XuatKhoChiTiet').hide();
        $('.XuatKhoTongQuan').hide();
        $('.showChungTu').hide();
        $('.Tab_HangNhapKho input').on('click', function() {
            self.Select_Tabletk($(this).val())
            _selectTabtk = $(this).val();
            $('.NhapKhoChiTiet').hide();
            $('.NhapKhoTongQuan').hide();
            $('.XuatKhoChiTiet').hide();
            $('.XuatKhoTongQuan').hide();
            $('.table_TonKho').hide();
            if (dk_tabtk == 2) {
                if (_selectTabtk == 1) {
                    $('.NhapKhoTongQuan').show();
                    self.getListReportHangHoa_TongHopNhapKho();
                    $(".list_HHTonKho").show();
                    $(".list_HHTonKhoNhapKhoTQ").hide();
                    $(".list_HHTonKhoXuatKhoTQ").hide();
                    $(".list_HHTonKhhoNhapCT").hide();
                    $(".list_HHTonKhoXuatpCT").hide();
                }
                else {
                    $('.NhapKhoChiTiet').show();
                    self.getListReportHangHoa_TongHopNhapKhoChiTiet();
                    $(".list_HHTonKho").hide();
                    $(".list_HHTonKhoNhapKhoTQ").hide();
                    $(".list_HHTonKhoXuatKhoTQ").hide();
                    $(".list_HHTonKhhoNhapCT").show();
                    $(".list_HHTonKhoXuatpCT").hide();
                }
            }
            else if (dk_tabtk == 3) {
                if (_selectTabtk == 1) {
                    $('.XuatKhoTongQuan').show();
                    self.getListReportHangHoa_TongHopXuatKho();
                    $(".list_HHTonKho").hide();
                    $(".list_HHTonKhoNhapKhoTQ").hide();
                    $(".list_HHTonKhoXuatKhoTQ").show();
                    $(".list_HHTonKhhoNhapCT").hide();
                    $(".list_HHTonKhoXuatpCT").hide();
                }
                else {
                    $('.XuatKhoChiTiet').show();
                    self.getListReportHangHoa_TongHopXuatKhoChiTiet();
                    $(".list_HHTonKho").hide();
                    $(".list_HHTonKhoNhapKhoTQ").hide();
                    $(".list_HHTonKhoXuatKhoTQ").hide();
                    $(".list_HHTonKhhoNhapCT").hide();
                    $(".list_HHTonKhoXuatpCT").show();
                }
            }
        });
        $('.table_TraHangNhap input').on('click', function() {
            self.Select_Tablencc($(this).val())
            _selectTabncc = $(this).val();
            $('.table_NTNhap').hide();
            $('.table_NTNhapCT').hide();
            $('.table_NTTraCT').hide();
            $('.table_NTTra').hide();
            if (dk_tabncc == 1) {
                if (_selectTabncc == 1) {
                    $('.table_NTNhap').show();
                    self.getListReportHH_NhapHangNCC();
                    $(".list_HHTraNhapHang").show();
                    $(".list_HHTraNhapHangTab2").hide();
                    $(".list_HHTraNhapHangCT2").hide();
                    $(".list_HHTraNhapHangCT1").hide();
                }
                else {
                    $('.table_NTNhapCT').show();
                    self.getListReportHH_NhapHangChiTietNCC();
                    $(".list_HHTraNhapHang").hide();
                    $(".list_HHTraNhapHangTab2").hide();
                    $(".list_HHTraNhapHangCT2").hide();
                    $(".list_HHTraNhapHangCT1").show();
                }
            }
            else if (dk_tabncc == 2) {
                if (_selectTabncc == 1) {
                    $('.table_NTTra').show();
                    self.getListReportHH_TraHangNhap();
                    $(".list_HHTraNhapHang").hide();
                    $(".list_HHTraNhapHangTab2").show();
                    $(".list_HHTraNhapHangCT2").hide();
                    $(".list_HHTraNhapHangCT1").hide();
                }
                else {
                    $('.table_NTTraCT').show();
                    self.getListReportHH_TraHangNhapChiTiet();
                    $(".list_HHTraNhapHang").hide();
                    $(".list_HHTraNhapHangTab2").hide();
                    $(".list_HHTraNhapHangCT1").hide();
                    $(".list_HHTraNhapHangCT2").show();
                }
            }
        });
        self.SelectNhapHangNCC = function() {
            $('.table_NTNhap').hide();
            $('.table_NTNhapCT').hide();
            $('.table_NTTraCT').hide();
            $('.table_NTTra').hide();
            dk_tabncc = 1;
            if (_selectTabncc == 1) {
                $('.table_NTNhap').show();
                self.getListReportHH_NhapHangNCC();
                $(".list_HHTraNhapHang").show();
                $(".list_HHTraNhapHangTab2").hide();
                $(".list_HHTraNhapHangCT1").hide();
                $(".list_HHTraNhapHangCT2").hide();
            }
            else {
                $('.table_NTNhapCT').show();
                self.getListReportHH_NhapHangChiTietNCC();
                $(".list_HHTraNhapHang").hide();
                $(".list_HHTraNhapHangTab2").hide();
                $(".list_HHTraNhapHangCT1").show();
                $(".list_HHTraNhapHangCT2").hide();
            }
        }
        self.SelectTraHangNCC = function() {
            $('.table_NTNhap').hide();
            $('.table_NTNhapCT').hide();
            $('.table_NTTraCT').hide();
            $('.table_NTTra').hide();
            dk_tabncc = 2;
            if (_selectTabncc == 1) {
                $('.table_NTTra').show();
                self.getListReportHH_TraHangNhap();
                $(".list_HHTraNhapHang").hide();
                $(".list_HHTraNhapHangTab2").show();
                $(".list_HHTraNhapHangCT1").hide();
                $(".list_HHTraNhapHangCT2").hide();
            }
            else {
                $('.table_NTTraCT').show();
                self.getListReportHH_TraHangNhapChiTiet();
                $(".list_HHTraNhapHang").hide();
                $(".list_HHTraNhapHangTab2").hide();
                $(".list_HHTraNhapHangCT1").hide();
                $(".list_HHTraNhapHangCT2").show();
            }
        }
        self.selectHHTonKho = function() {
            self.MoiQuanTam('tồn kho');
            $('.DatePK').show();
            $('.DateRG').hide();
            $('.rdo_checkTable').hide();
            $('.NhapKhoChiTiet').hide();
            $('.NhapKhoTongQuan').hide();
            $('.XuatKhoChiTiet').hide();
            $('.XuatKhoTongQuan').hide();
            $('.showChungTu').hide();
            $('.table_TonKho').show();
            $(".list_HHTonKhoXuatpCT").hide();
            $(".list_HHTonKhhoNhapCT").hide();
            $(".list_HHTonKhoXuatKhoTQ").hide();
            $(".list_HHTonKhoNhapKhoTQ").hide();
            $(".list_HHTonKho").show();
            dk_tabtk = 1;
            self.getListReportHangHoa_TonKho();
        }
        self.selectHHTongHopNhapKho = function() {
            $(".Report_Empty").hide();
            self.MoiQuanTam('tổng hợp nhập kho');
            $('.DateRG').show();
            $('.DatePK').hide();
            $('.rdo_checkTable').show();
            $('.showChungTu').show();
            $('.NhapKhoChiTiet').hide();
            $('.NhapKhoTongQuan').hide();
            $('.XuatKhoChiTiet').hide();
            $('.XuatKhoTongQuan').hide();
            $('.table_TonKho').hide();
            $('.list_HHTonKho').hide();
            $(".list_HHTonKhoXuatpCT").hide();
            $(".list_HHTonKhoXuatKhoTQ").hide();
            $(".list_HHTonKho").hide();
            if (_selectTabtk == 1) {
                $(".list_HHTonKhhoNhapCT").hide();
                $('.list_HHTonKhoNhapKhoTQ').show();
            }
            else {
                $(".list_HHTonKhhoNhapCT").show();
                $('.list_HHTonKhoNhapKhoTQ').hide();
            }
            dk_tabtk = 2;
            self.getListDM_LoaiChungTuNhapKho("4,6,9,10");
        }
        self.selectHHTongHopXuatKho = function() {
            $(".Report_Empty").hide();
            self.MoiQuanTam('tổng hợp xuất kho');
            $('.DateRG').show();
            $('.DatePK').hide();
            $('.rdo_checkTable').show();
            $('.showChungTu').show();
            $('.NhapKhoChiTiet').hide();
            $('.NhapKhoTongQuan').hide();
            $('.XuatKhoChiTiet').hide();
            $('.XuatKhoTongQuan').hide();
            $('.table_TonKho').hide();
            $('.list_HHTonKhoNhapKhoTQ').hide();
            $(".list_HHTonKho").hide();
            $(".list_HHTonKhhoNhapCT").hide();
            if (_selectTabtk == 1) {
                $(".list_HHTonKhoXuatpCT").hide();
                $(".list_HHTonKhoXuatKhoTQ").show();
            }
            else {
                $(".list_HHTonKhoXuatpCT").show();
                $(".list_HHTonKhoXuatKhoTQ").hide();
            }
            dk_tabtk = 3;
            self.getListDM_LoaiChungTuXuatKho("1,5,7,8,10");
        }

        self.ReportHH_TonKho = ko.observableArray();
        self.ReportHH_TonKhoPrint = ko.observableArray();
        self.TongCongTK_SoLuong = ko.observable();
        self.TongCongTK_ThanhTien = ko.observable();
        self.getListReportHangHoa_TonKho = function() {
            if (self.BCHH_TonKho() == "BCHH_TonKho") {
                $(".PhanQuyen").hide();                                                      
                self.RefreshPrint();
                hidewait('table_h');
                _pageNumber = 1;
                self.pageNumberTK(_pageNumber)
                ajaxHelper(ReportUri + "getListReportHH_TonKho?maHH=" + _maHH + "&timeStart=" + _tonkhoStart + "&timeEnd=" + _tonkhoEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach + "&TinhTrang=" + TinhTrangHH + "&ID_NguoiDung=" + _IDDoiTuong, "GET").done(function (data) {
                    self.ReportHH_TonKhoPrint(data.LstDataPrint);
                    self.ReportHH_TonKho(self.ReportHH_TonKhoPrint().slice(0, 10));
                    self.TongCongTK_SoLuong(data._tienvon);
                    self.TongCongTK_ThanhTien(data._thanhtien);
                    self.SumNumberPageReport(data.LstPageNumber);
                    AllPage = self.SumNumberPageReport().length;
                    if (self.ReportHH_TonKhoPrint().length != 0) {
                        $('.TongCongHHTonKho').show();
                        self.RowsStart((self.pageNumberTK() - 1) * self.pageSize() + 1);
                        self.RowsEnd((self.pageNumberTK() - 1) * self.pageSize() + self.ReportHH_TonKho().length)
                    }
                    else {
                        $('.TongCongHHTonKho').hide();
                        self.RowsStart('0');
                        self.RowsEnd('0');
                    }
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    $("div[id ^= 'wait']").text("");
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                $(".TongCongHHTonKho").hide();
            }
        }
        self.ReportHH_TonKho1 = ko.computed(function(x) {
            var first = (self.pageNumberTK() - 1) * self.pageSize();
            // console.log(dk_tabtk);
            if (dk_tabtk == 1) {
                if (self.ReportHH_TonKhoPrint() !== null) {
                    if (self.ReportHH_TonKhoPrint().length != 0) {
                        $('.TongCongHHTonKho').show();
                        $(".Report_Empty").hide();
                        $(".page").show();
                        self.RowsStart((self.pageNumberTK() - 1) * self.pageSize() + 1);
                        self.RowsEnd((self.pageNumberTK() - 1) * self.pageSize() + self.ReportHH_TonKhoPrint().slice(first, first + self.pageSize()).length)
                    }
                    else {
                        $('.TongCongHHTonKho').hide();
                        $(".Report_Empty").show();
                        $(".page").hide();
                        self.RowsStart('0');
                        self.RowsEnd('0');
                    }
                    self.ReportHH_TonKho(self.ReportHH_TonKhoPrint().slice(first, first + _pageSize));
                    return self.ReportHH_TonKhoPrint().slice(first, first + _pageSize);
                }
                return null;
            }
            return null;
        })
        self.ReportHH_TongHopNhapKho = ko.observableArray();
        self.ReportHH_TongHopNhapKhoPrint = ko.observableArray();
        self.TongCongTHNK_SoLuong = ko.observable();
        self.TongCongTHNK_ThanhTien = ko.observable();
        self.getListReportHangHoa_TongHopNhapKho = function() {
            if (self.BCHH_TonKho() == "BCHH_TonKho") {
                $(".PhanQuyen").hide();
                self.ReportHH_TongHopNhapKhoPrint([]);
                self.RefreshPrint();
                hidewait('table_h');
                _pageNumber = 1;
                self.pageNumberTHNK(_pageNumber);
                ajaxHelper(ReportUri + "getListReportHH_TongHopNhapKho?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach + "&LoaiChungTu=" + _idChungTuSeach + "&TinhTrang=" + TinhTrangHH, "GET").done(function(data) {
                    self.ReportHH_TongHopNhapKhoPrint(data.LstDataPrint);
                    self.ReportHH_TongHopNhapKho(self.ReportHH_TongHopNhapKhoPrint().slice(0, 10));
                    self.TongCongTHNK_SoLuong(data._tienvon);
                    self.TongCongTHNK_ThanhTien(data._thanhtien);
                    self.SumNumberPageReport(data.LstPageNumber);
                    AllPage = self.SumNumberPageReport().length;
                    if (self.ReportHH_TongHopNhapKho().length != 0) {
                        $('.TongCongTQTongHopNhapKho').show();
                        $(".Report_Empty").hide();
                        $(".page").show();
                        self.RowsStart((self.pageNumberTHNK() - 1) * self.pageSize() + 1);
                        self.RowsEnd((self.pageNumberTHNK() - 1) * self.pageSize() + self.ReportHH_TongHopNhapKho().length)
                    }
                    else {
                        $('.TongCongTQTongHopNhapKho').hide();
                        $(".Report_Empty").show();
                        $(".page").hide();
                        self.RowsStart('0');
                        self.RowsEnd('0');
                    }
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    $("div[id ^= 'wait']").text("");
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                $(".TongCongTQTongHopNhapKho").hide();
            }

        }
        self.ReportHH_TongHopNhapKho1 = ko.computed(function(x) {
            var first = (self.pageNumberTHNK() - 1) * self.pageSize();
            if (dk_tabtk == 2 && _selectTabtk == 1) {
                if (self.ReportHH_TongHopNhapKhoPrint() !== null) {
                    if (self.ReportHH_TongHopNhapKhoPrint().length != 0) {
                        $('.TongCongTQTongHopNhapKho').show();
                        //$(".Report_Empty").hide();
                        //$(".page").show();
                        self.RowsStart((self.pageNumberTHNK() - 1) * self.pageSize() + 1);
                        self.RowsEnd((self.pageNumberTHNK() - 1) * self.pageSize() + self.ReportHH_TongHopNhapKhoPrint().slice(first, first + self.pageSize()).length)
                    }
                    else {
                        $('.TongCongTQTongHopNhapKho').hide();
                        //$(".Report_Empty").show();
                        //$(".page").hide();
                        self.RowsStart('0');
                        self.RowsEnd('0');
                    }
                    self.ReportHH_TongHopNhapKho(self.ReportHH_TongHopNhapKhoPrint().slice(first, first + _pageSize));
                    return self.ReportHH_TongHopNhapKhoPrint().slice(first, first + _pageSize);
                }
                return null;
            }
        })
        self.ReportHH_TongHopNhapKhoChiTiet = ko.observableArray();
        self.ReportHH_TongHopNhapKhoChiTietPrint = ko.observableArray();
        self.TongCongTHNKCT_SoLuong = ko.observable();
        self.TongCongTHNKCT_ThanhTien = ko.observable();
        self.getListReportHangHoa_TongHopNhapKhoChiTiet = function() {
            if (self.BCHH_TonKho() == "BCHH_TonKho") {
                self.RefreshPrint();
                hidewait('table_h');
                $(".PhanQuyen").hide();
                _pageNumber = 1;
                self.pageNumberTHNKCT(_pageNumber);
                ajaxHelper(ReportUri + "getListReportHH_TongHopNhapKhoChiTiet?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach + "&LoaiChungTu=" + _idChungTuSeach + "&TinhTrang=" + TinhTrangHH, "GET").done(function(data) {
                    self.ReportHH_TongHopNhapKhoChiTietPrint(data.LstDataPrint);
                    self.ReportHH_TongHopNhapKhoChiTiet(self.ReportHH_TongHopNhapKhoChiTietPrint().slice(0, 10));
                    self.TongCongTHNKCT_SoLuong(data._tienvon);
                    self.TongCongTHNKCT_ThanhTien(data._thanhtien);
                    self.SumNumberPageReport(data.LstPageNumber);
                    AllPage = self.SumNumberPageReport().length;
                    if (self.ReportHH_TongHopNhapKhoChiTiet().length != 0) {
                        $('.TongCongCTTongHopNhapKho').show();
                        $(".Report_Empty").hide();
                        $(".page").show();
                        self.RowsStart((self.pageNumberTHNKCT() - 1) * self.pageSize() + 1);
                        self.RowsEnd((self.pageNumberTHNKCT() - 1) * self.pageSize() + self.ReportHH_TongHopNhapKhoChiTiet().length)
                    }
                    else {
                        $('.TongCongCTTongHopNhapKho').hide();
                        $(".Report_Empty").show();
                        $(".page").hide();
                        self.RowsStart('0');
                        self.RowsEnd('0');
                    }
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    $("div[id ^= 'wait']").text("");
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                $(".TongCongCTTongHopNhapKho").hide();
            }

        }
        self.ReportHH_TongHopNhapKhoChiTiet1 = ko.computed(function(x) {
            var first = (self.pageNumberTHNKCT() - 1) * self.pageSize();
            if (dk_tabtk == 2 && _selectTabtk == 2) {
                if (self.ReportHH_TongHopNhapKhoChiTietPrint() !== null) {
                    if (self.ReportHH_TongHopNhapKhoChiTietPrint().length != 0) {
                        $('.TongCongCTTongHopNhapKho').show();
                        //$(".Report_Empty").hide();
                        //$(".page").show();
                        self.RowsStart((self.pageNumberTHNKCT() - 1) * self.pageSize() + 1);
                        self.RowsEnd((self.pageNumberTHNKCT() - 1) * self.pageSize() + self.ReportHH_TongHopNhapKhoChiTietPrint().slice(first, first + self.pageSize()).length)
                    }
                    else {
                        $('.TongCongCTTongHopNhapKho').hide();
                        //$(".Report_Empty").show();
                        //$(".page").hide();
                        self.RowsStart('0');
                        self.RowsEnd('0');
                    }
                    self.ReportHH_TongHopNhapKhoChiTiet(self.ReportHH_TongHopNhapKhoChiTietPrint().slice(first, first + _pageSize));
                    return self.ReportHH_TongHopNhapKhoChiTietPrint().slice(first, first + _pageSize);
                }
                return null;
            }
        })
        self.ReportHH_TongHopXuatKho = ko.observableArray();
        self.ReportHH_TongHopXuatKhoPrint = ko.observableArray();
        self.TongCongTHXK_SoLuong = ko.observable();
        self.TongCongTHXK_ThanhTien = ko.observable();
        self.getListReportHangHoa_TongHopXuatKho = function() {
            if (self.BCHH_TonKho() == "BCHH_TonKho") {
                self.RefreshPrint();
                hidewait('table_h');
                $(".PhanQuyen").hide();
                _pageNumber = 1;
                self.pageNumberTHXK(_pageNumber);
                ajaxHelper(ReportUri + "getListReportHH_TongHopXuatKho?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach + "&LoaiChungTu=" + _idChungTuSeach + "&TinhTrang=" + TinhTrangHH, "GET").done(function(data) {
                    self.ReportHH_TongHopXuatKhoPrint(data.LstDataPrint);
                    self.ReportHH_TongHopXuatKho(self.ReportHH_TongHopXuatKhoPrint().slice(0, 10));
                    self.TongCongTHXK_SoLuong(data._tienvon);
                    self.TongCongTHXK_ThanhTien(data._thanhtien);
                    self.SumNumberPageReport(data.LstPageNumber);
                    AllPage = self.SumNumberPageReport().length;
                    if (self.ReportHH_TongHopXuatKho().length != 0) {
                        $('.TongCongTQTongHopXuatKho').show();
                        $(".Report_Empty").hide();
                        $(".page").show();
                        self.RowsStart((self.pageNumberTHXK() - 1) * self.pageSize() + 1);
                        self.RowsEnd((self.pageNumberTHXK() - 1) * self.pageSize() + self.ReportHH_TongHopXuatKho().length)
                    }
                    else {
                        $('.TongCongTQTongHopXuatKho').hide();
                        $(".Report_Empty").show();
                        $(".page").hide();
                        self.RowsStart('0');
                        self.RowsEnd('0');
                    }
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    $("div[id ^= 'wait']").text("");
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                $(".TongCongTQTongHopXuatKho").hide();
            }
        }
        self.ReportHH_TongHopXuatKho1 = ko.computed(function(x) {
            var first = (self.pageNumberTHXK() - 1) * self.pageSize();
            if (dk_tabtk == 3 && _selectTabtk == 1) {
                if (self.ReportHH_TongHopXuatKhoPrint() !== null) {
                    if (self.ReportHH_TongHopXuatKhoPrint().length != 0) {
                        $('.TongCongTQTongHopXuatKho').show();
                        //$(".Report_Empty").hide();
                        //$(".page").show();
                        self.RowsStart((self.pageNumberTHXK() - 1) * self.pageSize() + 1);
                        self.RowsEnd((self.pageNumberTHXK() - 1) * self.pageSize() + self.ReportHH_TongHopXuatKhoPrint().slice(first, first + self.pageSize()).length)
                    }
                    else {
                        $('.TongCongTQTongHopXuatKho').hide();
                        $(".Report_Empty").show();
                        $(".page").hide();
                        self.RowsStart('0');
                        self.RowsEnd('0');
                    }
                    self.ReportHH_TongHopXuatKho(self.ReportHH_TongHopXuatKhoPrint().slice(first, first + _pageSize));
                    return self.ReportHH_TongHopXuatKhoPrint().slice(first, first + _pageSize);
                }
                return null;
            }
        })
        self.ReportHH_TongHopXuatKhoChiTiet = ko.observableArray();
        self.ReportHH_TongHopXuatKhoChiTietPrint = ko.observableArray();
        self.TongCongTHXKCT_SoLuong = ko.observable();
        self.TongCongTHXKCT_ThanhTien = ko.observable();
        self.getListReportHangHoa_TongHopXuatKhoChiTiet = function() {
            if (self.BCHH_TonKho() == "BCHH_TonKho") {
                self.RefreshPrint();
                hidewait('table_h');
                _pageNumber = 1;
                $(".PhanQuyen").hide();
                self.pageNumberTHXKCT(_pageNumber);
                ajaxHelper(ReportUri + "getListReportHH_TongHopXuatKhoChiTiet?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach + "&LoaiChungTu=" + _idChungTuSeach + "&TinhTrang=" + TinhTrangHH, "GET").done(function(data) {
                    self.ReportHH_TongHopXuatKhoChiTietPrint(data.LstDataPrint);
                    self.ReportHH_TongHopXuatKhoChiTiet(self.ReportHH_TongHopXuatKhoChiTietPrint().slice(0, 10));
                    self.TongCongTHXKCT_SoLuong(data._tienvon);
                    self.TongCongTHXKCT_ThanhTien(data._thanhtien);
                    self.SumNumberPageReport(data.LstPageNumber);
                    AllPage = self.SumNumberPageReport().length;
                    if (self.ReportHH_TongHopXuatKhoChiTiet().length != 0) {
                        $('.TongCongCTTongHopXuatKho').show();
                        $(".Report_Empty").hide();
                        $(".page").show();
                        self.RowsStart((self.pageNumberTHXKCT() - 1) * self.pageSize() + 1);
                        self.RowsEnd((self.pageNumberTHXKCT() - 1) * self.pageSize() + self.ReportHH_TongHopXuatKhoChiTiet().length)
                    }
                    else {
                        $('.TongCongCTTongHopXuatKho').hide();
                        $(".Report_Empty").show();
                        $(".page").hide();
                        self.RowsStart('0');
                        self.RowsEnd('0');
                    }
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    $("div[id ^= 'wait']").text("");
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                $(".TongCongCTTongHopXuatKho").hide();
            }
        }
        self.ReportHH_TongHopXuatKhoChiTiet1 = ko.computed(function(x) {
            var first = (self.pageNumberTHXKCT() - 1) * self.pageSize();
            if (dk_tabtk == 3 && _selectTabtk == 2) {
                if (self.ReportHH_TongHopXuatKhoChiTietPrint() !== null) {
                    if (self.ReportHH_TongHopXuatKhoChiTietPrint().length != 0) {
                        $('.TongCongCTTongHopXuatKho').show();
                        
                        self.RowsStart((self.pageNumberTHXKCT() - 1) * self.pageSize() + 1);
                        self.RowsEnd((self.pageNumberTHXKCT() - 1) * self.pageSize() + self.ReportHH_TongHopXuatKhoChiTietPrint().slice(first, first + self.pageSize()).length)
                    }
                    else {
                        $('.TongCongCTTongHopXuatKho').hide();
                        
                        self.RowsStart('0');
                        self.RowsEnd('0');
                    }
                    self.ReportHH_TongHopXuatKhoChiTiet(self.ReportHH_TongHopXuatKhoChiTietPrint().slice(first, first + _pageSize));
                    return self.ReportHH_TongHopXuatKhoChiTietPrint().slice(first, first + _pageSize);
                }
                return null;
            }
        })
        // GetListHangHoa_NhanVien
        self.NV_SoLuongNhanVien = ko.observable();
        self.NV_SoLuongBan = ko.observable();
        self.NV_GiaTri = ko.observable();
        self.getListReportHH_NhanVien = function() {
            if (self.BCHH_NhanVien() == "BCHH_NhanVien") {
                $(".PhanQuyen").hide();
                self.RefreshPrint();
                hidewait('table_h');
                _pageNumber = 1;
                self.pageNumberNV(_pageNumber);
                ajaxHelper(ReportUri + "getListReportHH_NhanVien?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function(data) {
                    self.ReportHH_NhanVienPrint(data.LstDataPrint);
                    self.NV_SoLuongNhanVien(data._thanhtien);
                    self.NV_SoLuongBan(data._lailo);
                    self.NV_GiaTri(data._tienvon)
                    self.SumNumberPageReport(data.LstPageNumber);
                    AllPage = self.SumNumberPageReport().length;
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    $("div[id ^= 'wait']").text("");
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                $(".TongCongHH_NhanVien").hide();
            }
        }
        self.ReportHH_NhanVien = ko.computed(function(x) {
            var first = (self.pageNumberNV() - 1) * self.pageSize();
            if (self.ReportHH_NhanVienPrint() !== null) {
                if (self.ReportHH_NhanVienPrint().length != 0) {
                    $('.TongCongHH_NhanVien').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumberNV() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumberNV() - 1) * self.pageSize() + self.ReportHH_NhanVienPrint().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.TongCongHH_NhanVien').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.ReportHH_NhanVienPrint().slice(first, first + _pageSize);
            }
            return null;
        })
        // GetListHangHoa_KhachHang
        self.KH_SoLuongKhachHang = ko.observable();
        self.KH_SoLuongMua = ko.observable();
        self.KH_GiaTri = ko.observable();
        self.getListReportHH_KhachHang = function() {
            if (self.BCHH_KhachHang() == "BCHH_KhachHang") {
                self.RefreshPrint();
                hidewait('table_h');
                $(".PhanQuyen").hide();
                _pageNumber = 1;
                self.pageNumberKH(_pageNumber);
                ajaxHelper(ReportUri + "getListReportHH_KhachHang?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function(data) {
                    self.ReportHH_KhachHangPrint(data.LstDataPrint);
                    self.KH_SoLuongKhachHang(data._thanhtien);
                    self.KH_SoLuongMua(data._lailo);
                    self.KH_GiaTri(data._tienvon)
                    self.SumNumberPageReport(data.LstPageNumber);
                    AllPage = self.SumNumberPageReport().length;
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    $("div[id ^= 'wait']").text("");
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                $(".TongCongHH_KhachHang").hide();
            }
        }
        self.ReportHH_KhachHang = ko.computed(function(x) {
            var first = (self.pageNumberKH() - 1) * self.pageSize();
            if (self.ReportHH_KhachHangPrint() !== null) {
                if (self.ReportHH_KhachHangPrint().length != 0) {
                    $('.TongCongHH_KhachHang').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumberKH() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumberKH() - 1) * self.pageSize() + self.ReportHH_KhachHangPrint().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.TongCongHH_KhachHang').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.ReportHH_KhachHangPrint().slice(first, first + _pageSize);
            }
            return null;
        })
        // GetListHangHoa_NhaCungCap
        self.NCC_SoLuongNhaCungCap = ko.observable();
        self.NCC_SoLuongSanPham = ko.observable();
        self.NCC_GiaTri = ko.observable();
        self.getListReportHH_NhaCungCap = function() {
            if (self.BCHH_NCC() == "BCHH_NCC") {
                $(".PhanQuyen").hide();
                self.RefreshPrint();
                hidewait('table_h');
                ajaxHelper(ReportUri + "getListReportHH_NhaCungCap?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function(data) {
                    self.ReportHH_NhaCungCapPrint(data.LstDataPrint);
                    self.NCC_SoLuongNhaCungCap(data._thanhtien);
                    self.NCC_SoLuongSanPham(data._lailo);
                    self.NCC_GiaTri(data._tienvon)
                    self.SumNumberPageReport(data.LstPageNumber);
                    AllPage = self.SumNumberPageReport().length;
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    $("div[id ^= 'wait']").text("");
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                $(".TongCongHH_NhaCungCap").hide();
            }

        }
        self.ReportHH_NhaCungCap = ko.computed(function(x) {
            var first = (self.pageNumberNCC() - 1) * self.pageSize();
            if (self.ReportHH_NhaCungCapPrint() !== null) {
                if (self.ReportHH_NhaCungCapPrint().length != 0) {
                    $('.TongCongHH_NhaCungCap').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumberNCC() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumberNCC() - 1) * self.pageSize() + self.ReportHH_NhaCungCapPrint().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.TongCongHH_NhaCungCap').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.ReportHH_NhaCungCapPrint().slice(first, first + _pageSize);
            }
            return null;
        })
        self.GetClass = function(page) {
            return (page.SoTrang === self.currentPage()) ? "click" : "";
        };
        self.GetClassHangHoa = function(page) {
            return (page.SoTrang === self.currentPageHangHoa()) ? "click" : "";
        };
        self.GetClassHangHoaNH = function(page) {
            return (page.SoTrang === self.currentPageHangHoaNH()) ? "click" : "";
        };
        // phân trang chi tiết HangHoa
        self.selecPageHangHoa = function() {
            // AllPageHangHoa = self.SumNumberPageReport().length;

            if (AllPageHangHoa > 4) {
                for (var i = 3; i < AllPageHangHoa; i++) {
                    self.SumNumberPageReportHangHoa.pop(i + 1);
                }
                self.SumNumberPageReportHangHoa.push({ SoTrang: 4 });
                self.SumNumberPageReportHangHoa.push({ SoTrang: 5 });
            }
            else {
                for (var i = 0; i < 6; i++) {
                    self.SumNumberPageReportHangHoa.pop(i);
                }
                for (var j = 0; j < AllPageHangHoa; j++) {
                    self.SumNumberPageReportHangHoa.push({ SoTrang: j + 1 });
                }
            }
            $('#StartPageHangHoa').hide();
            $('#BackPageHangHoa').hide();
            $('#NextPageHangHoa').show();
            $('#EndPageHangHoa').show();
        }
        self.ReserPageHangHoa = function(item) {
            //self.selecPage();
            if (_pageNumberHangHoa > 1 && AllPageHangHoa > 5/* && nextPage < AllPageHangHoa - 1*/) {
                if (_pageNumberHangHoa > 3 && _pageNumberHangHoa < AllPageHangHoa - 1) {
                    for (var i = 0; i < 5; i++) {
                        self.SumNumberPageReportHangHoa.replace(self.SumNumberPageReportHangHoa()[i], { SoTrang: parseInt(_pageNumberHangHoa) + i - 2 });
                    }
                }
                else if (parseInt(_pageNumberHangHoa) === parseInt(AllPageHangHoa) - 1) {
                    for (var i = 0; i < 5; i++) {
                        self.SumNumberPageReportHangHoa.replace(self.SumNumberPageReportHangHoa()[i], { SoTrang: parseInt(_pageNumberHangHoa) + i - 3 });
                    }
                }
                else if (_pageNumberHangHoa == AllPageHangHoa) {
                    for (var i = 0; i < 5; i++) {
                        self.SumNumberPageReportHangHoa.replace(self.SumNumberPageReportHangHoa()[i], { SoTrang: parseInt(_pageNumberHangHoa) + i - 4 });
                    }
                }
                else if (_pageNumberHangHoa < 4) {
                    for (var i = 0; i < 5; i++) {
                        //console.log(_pageNumberHangHoa)
                        self.SumNumberPageReportHangHoa.replace(self.SumNumberPageReportHangHoa()[i], { SoTrang: 1 + i });
                    }
                }
            }
            else if (_pageNumberHangHoa == 1 && AllPageHangHoa > 5) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportHangHoa.replace(self.SumNumberPageReportHangHoa()[i], { SoTrang: parseInt(_pageNumberHangHoa) + i });
                }
            }
            if (_pageNumberHangHoa > 1) {
                $('#StartPageHangHoa').show();
                $('#BackPageHangHoa').show();
            }
            else {
                $('#StartPageHangHoa').hide();
                $('#BackPageHangHoa').hide();
            }
            if (_pageNumberHangHoa == AllPageHangHoa) {
                $('#NextPageHangHoa').hide();
                $('#EndPageHangHoa').hide();
            }
            else {
                $('#NextPageHangHoa').show();
                $('#EndPageHangHoa').show();
            }
            self.currentPageHangHoa(parseInt(_pageNumberHangHoa));
        }
        self.NextPageHangHoa = function(item) {
            if (_pageNumberHangHoa < AllPageHangHoa) {
                _pageNumberHangHoa = _pageNumberHangHoa + 1;
                self.ReserPageHangHoa();
                self.SelectedPageNumberReportHH_XuatChuyenHangChiTiet();
            }
        };
        self.BackPageHangHoa = function(item) {
            if (_pageNumberHangHoa > 1) {
                _pageNumberHangHoa = _pageNumberHangHoa - 1;
                self.ReserPageHangHoa();
                self.SelectedPageNumberReportHH_XuatChuyenHangChiTiet();
            }
        };
        self.EndPageHangHoa = function(item) {
            _pageNumberHangHoa = AllPageHangHoa;
            self.ReserPageHangHoa();
            self.SelectedPageNumberReportHH_XuatChuyenHangChiTiet();
        };
        self.StartPageHangHoa = function(item) {
            _pageNumberHangHoa = 1;
            self.ReserPageHangHoa();
            self.SelectedPageNumberReportHH_XuatChuyenHangChiTiet();
        };
        self.gotoNextPageHangHoa = function(item) {
            _pageNumberHangHoa = item.SoTrang;
            self.ReserPageHangHoa();
            self.SelectedPageNumberReportHH_XuatChuyenHangChiTiet();
        }

        // phân trang chi tiết NhapHangChuyen
        self.selecPageHangHoaNH = function() {
            // AllPageHangHoa = self.SumNumberPageReport().length;

            if (AllPageHangHoaNH > 4) {
                for (var i = 3; i < AllPageHangHoaNH; i++) {
                    self.SumNumberPageReportNhapHangHoa.pop(i + 1);
                }
                self.SumNumberPageReportNhapHangHoa.push({ SoTrang: 4 });
                self.SumNumberPageReportNhapHangHoa.push({ SoTrang: 5 });
            }
            else {
                for (var i = 0; i < 6; i++) {
                    self.SumNumberPageReportNhapHangHoa.pop(i);
                }
                for (var j = 0; j < AllPageHangHoaNH; j++) {
                    self.SumNumberPageReportNhapHangHoa.push({ SoTrang: j + 1 });
                }
            }
            $('#StartPageHangHoaNH').hide();
            $('#BackPageHangHoaNH').hide();
            $('#NextPageHangHoaNH').show();
            $('#EndPageHangHoaNH').show();
        }
        self.ReserPageHangHoaNH = function(item) {
            //self.selecPage();
            if (_pageNumberHangHoaNH > 1 && AllPageHangHoaNH > 5/* && nextPage < AllPageHangHoa - 1*/) {
                if (_pageNumberHangHoaNH > 3 && _pageNumberHangHoaNH < AllPageHangHoaNH - 1) {
                    for (var i = 0; i < 5; i++) {
                        self.SumNumberPageReportNhapHangHoa.replace(self.SumNumberPageReportNhapHangHoa()[i], { SoTrang: parseInt(_pageNumberHangHoaNH) + i - 2 });
                    }
                }
                else if (parseInt(_pageNumberHangHoaNH) === parseInt(AllPageHangHoaNH) - 1) {
                    for (var i = 0; i < 5; i++) {
                        self.SumNumberPageReportNhapHangHoa.replace(self.SumNumberPageReportNhapHangHoa()[i], { SoTrang: parseInt(_pageNumberHangHoaNH) + i - 3 });
                    }
                }
                else if (_pageNumberHangHoaNH == AllPageHangHoaNH) {
                    for (var i = 0; i < 5; i++) {
                        self.SumNumberPageReportNhapHangHoa.replace(self.SumNumberPageReportNhapHangHoa()[i], { SoTrang: parseInt(_pageNumberHangHoaNH) + i - 4 });
                    }
                }
                else if (_pageNumberHangHoaNH < 4) {
                    for (var i = 0; i < 5; i++) {
                        //console.log(_pageNumberHangHoa)
                        self.SumNumberPageReportNhapHangHoa.replace(self.SumNumberPageReportNhapHangHoa()[i], { SoTrang: 1 + i });
                    }
                }
            }
            else if (_pageNumberHangHoaNH == 1 && AllPageHangHoaNH > 5) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportNhapHangHoa.replace(self.SumNumberPageReportNhapHangHoa()[i], { SoTrang: parseInt(_pageNumberHangHoaNH) + i });
                }
            }
            if (_pageNumberHangHoaNH > 1) {
                $('#StartPageHangHoaNH').show();
                $('#BackPageHangHoaNH').show();
            }
            else {
                $('#StartPageHangHoaNH').hide();
                $('#BackPageHangHoaNH').hide();
            }
            if (_pageNumberHangHoaNH == AllPageHangHoaNH) {
                $('#NextPageHangHoaNH').hide();
                $('#EndPageHangHoaNH').hide();
            }
            else {
                $('#NextPageHangHoaNH').show();
                $('#EndPageHangHoaNH').show();
            }
            self.currentPageHangHoaNH(parseInt(_pageNumberHangHoaNH));
        }
        self.NextPageHangHoaNH = function(item) {
            if (_pageNumberHangHoaNH < AllPageHangHoaNH) {
                _pageNumberHangHoaNH = _pageNumberHangHoaNH + 1;
                self.ReserPageHangHoaNH();
                self.SelectedPageNumberReportHH_NhapChuyenHangChiTiet();
            }
        };
        self.BackPageHangHoaNH = function(item) {
            if (_pageNumberHangHoaNH > 1) {
                _pageNumberHangHoaNH = _pageNumberHangHoaNH - 1;
                self.ReserPageHangHoaNH();
                self.SelectedPageNumberReportHH_NhapChuyenHangChiTiet();
            }
        };
        self.EndPageHangHoaNH = function(item) {
            _pageNumberHangHoaNH = AllPageHangHoaNH;
            self.ReserPageHangHoaNH();
            self.SelectedPageNumberReportHH_NhapChuyenHangChiTiet();
        };
        self.StartPageHangHoaNH = function(item) {
            _pageNumberHangHoaNH = 1;
            self.ReserPageHangHoaNH();
            self.SelectedPageNumberReportHH_NhapChuyenHangChiTiet();
        };
        self.gotoNextPageHangHoaNH = function(item) {
            _pageNumberHangHoaNH = item.SoTrang;
            self.ReserPageHangHoaNH();
            self.SelectedPageNumberReportHH_NhapChuyenHangChiTiet();
        }
        //Phân trang
        self.selecPage = function() {
            if (AllPage > 4) {
                for (var i = 3; i < AllPage; i++) {
                    self.SumNumberPageReport.pop(i + 1);
                }
                self.SumNumberPageReport.push({ SoTrang: 4 });
                self.SumNumberPageReport.push({ SoTrang: 5 });
            }
            else {
                for (var i = 0; i < 6; i++) {
                    self.SumNumberPageReport.pop(i);
                }
                for (var j = 0; j < AllPage; j++) {
                    self.SumNumberPageReport.push({ SoTrang: j + 1 });
                }
            }
            $('#StartPage').hide();
            $('#BackPage').hide();
            $('#NextPage').show();
            $('#EndPage').show();
        }
        self.ReserPage = function(item) {
            LoadHtmlGrid(cacheExcelBH, 1, "banhangHH");
            LoadHtmlGrid(cacheExcelLN, 2, "loinhuanHH");
            LoadHtmlGrid(cacheExcelTHN, 19, "trahangnhapHH");
            LoadHtmlGrid(cacheExcelNHNCC, 9, "nhaphangNCC");
            LoadHtmlGrid(cacheExcelCTNHNCC, 18, "chitietnhaphangNCC");
            LoadHtmlGrid(cacheExcelTHNCT, 20, "trahangnhapchitietHH");
            LoadHtmlGrid(cacheExcelXHCTQ, 10, "chuyenhangxuatTQHH");
            LoadHtmlGrid(cacheExcelCHCT, 11, "chuyenhangxuatCTHH");
            LoadHtmlGrid(cacheExcelNHCTQ, 12, "chuyenhangnhapTQHH");
            LoadHtmlGrid(cacheExcelXH, 8, "xuathuyHH");
            LoadHtmlGrid(cacheExcelNV, 5, "nhanvientheohangbanHH");
            LoadHtmlGrid(cacheExcelNV, 4, "khachtheohangbanHH");
            LoadHtmlGrid(cacheExcelNV, 6, "NCCtheohangnhapHH");
            LoadHtmlGrid(cacheExcelTK, 13, "tonkhoHH");
            LoadHtmlGrid(cacheExcelTHHNKTQ, 14, "tonghopnhapkhoTQHH");
            LoadHtmlGrid(cacheExcelTHHNKCT, 16, "tonghopnhapkhoCTHH");
            LoadHtmlGrid(cacheExcelTHHXKTQ, 15, "tonghopxuatkhoTQHH");
            LoadHtmlGrid(cacheExcelTHHXKCT, 17, "tonghopxuatkhoCTHH");
            LoadHtmlGrid(cacheExcelXNT, 3, "xuatnhaptonHH");
            LoadHtmlGrid(cacheExcelXNTCT, 7, "xuatnhaptonchitietHH");

            //self.selecPage();
            if (_pageNumber > 1 && AllPage > 5/* && nextPage < AllPage - 1*/) {
                if (_pageNumber > 3 && _pageNumber < AllPage - 1) {
                    for (var i = 0; i < 5; i++) {
                        self.SumNumberPageReport.replace(self.SumNumberPageReport()[i], { SoTrang: parseInt(_pageNumber) + i - 2 });
                    }
                }
                else if (parseInt(_pageNumber) === parseInt(AllPage) - 1) {
                    for (var i = 0; i < 5; i++) {
                        self.SumNumberPageReport.replace(self.SumNumberPageReport()[i], { SoTrang: parseInt(_pageNumber) + i - 3 });
                    }
                }
                else if (_pageNumber == AllPage) {
                    for (var i = 0; i < 5; i++) {
                        self.SumNumberPageReport.replace(self.SumNumberPageReport()[i], { SoTrang: parseInt(_pageNumber) + i - 4 });
                    }
                }
                else if (_pageNumber < 4) {
                    for (var i = 0; i < 5; i++) {
                        self.SumNumberPageReport.replace(self.SumNumberPageReport()[i], { SoTrang: 1 + i });
                    }
                }
            }
            else if (_pageNumber == 1 && AllPage > 5) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReport.replace(self.SumNumberPageReport()[i], { SoTrang: parseInt(_pageNumber) + i });
                }
            }
            if (_pageNumber > 1) {
                $('#StartPage').show();
                $('#BackPage').show();
            }
            else {
                $('#StartPage').hide();
                $('#BackPage').hide();
            }
            if (_pageNumber == AllPage) {
                $('#NextPage').hide();
                $('#EndPage').hide();
            }
            else {
                $('#NextPage').show();
                $('#EndPage').show();
            }

            self.currentPage(parseInt(_pageNumber));
        }
        self.NextPage = function(item) {
            if (_pageNumber < AllPage) {
                _pageNumber = _pageNumber + 1;
                if (_kieubang == 1)
                    self.pageNumberBH(_pageNumber);
                else if (_kieubang == 2)
                    self.pageNumberLN(_pageNumber);
                else if (_kieubang == 3)
                    self.pageNumberXNT(_pageNumber);
                else if (_kieubang == 4)
                    self.pageNumberXNTCT(_pageNumber);
                else if (_kieubang == 5)
                    self.pageNumberXH(_pageNumber);
                else if (_kieubang == 6)
                    self.pageNumberNV(_pageNumber);
                else if (_kieubang == 7)
                    self.pageNumberKH(_pageNumber);
                else if (_kieubang == 8)
                    self.pageNumberNCC(_pageNumber);
                else if (_kieubang == 9) {
                    if (_selectTab == 1)
                        self.pageNumberCH(_pageNumber);
                    else
                        self.pageNumberHDCH(_pageNumber);
                }
                else if (_kieubang == 10) {
                    if (dk_tabncc == 1) {
                        if (_selectTabncc == 1)
                            self.pageNumberNHNCC(_pageNumber);
                        else
                            self.pageNumberNHCTNCC(_pageNumber);
                    }
                    else if (dk_tabncc == 2) {
                        if (_selectTabncc == 1)
                            self.pageNumberTHN(_pageNumber);
                        else
                            self.pageNumberTHNCT(_pageNumber);
                    }
                }
                else if (_kieubang == 11) {
                    self.pageNumberTK(_pageNumber);
                    self.pageNumberTHNK(_pageNumber);
                    self.pageNumberTHNKCT(_pageNumber);
                    self.pageNumberTHXK(_pageNumber);
                    self.pageNumberTHXKCT(_pageNumber);
                }
                self.ReserPage();
            }
        };
        self.BackPage = function(item) {
            if (_pageNumber > 1) {
                _pageNumber = _pageNumber - 1;
                if (_kieubang == 1)
                    self.pageNumberBH(_pageNumber);
                else if (_kieubang == 2)
                    self.pageNumberLN(_pageNumber);
                else if (_kieubang == 3)
                    self.pageNumberXNT(_pageNumber);
                else if (_kieubang == 4)
                    self.pageNumberXNTCT(_pageNumber);
                else if (_kieubang == 5)
                    self.pageNumberXH(_pageNumber);
                else if (_kieubang == 6)
                    self.pageNumberNV(_pageNumber);
                else if (_kieubang == 7)
                    self.pageNumberKH(_pageNumber);
                else if (_kieubang == 8)
                    self.pageNumberNCC(_pageNumber);
                else if (_kieubang == 9) {
                    if (_selectTab == 1)
                        self.pageNumberCH(_pageNumber);
                    else
                        self.pageNumberHDCH(_pageNumber);
                }
                else if (_kieubang == 10) {
                    if (dk_tabncc == 1) {
                        if (_selectTabncc == 1)
                            self.pageNumberNHNCC(_pageNumber);
                        else
                            self.pageNumberNHCTNCC(_pageNumber);
                    }
                    else if (dk_tabncc == 2) {
                        if (_selectTabncc == 1)
                            self.pageNumberTHN(_pageNumber);
                        else
                            self.pageNumberTHNCT(_pageNumber);
                    }
                }
                else if (_kieubang == 11) {
                    self.pageNumberTK(_pageNumber);
                    self.pageNumberTHNK(_pageNumber);
                    self.pageNumberTHNKCT(_pageNumber);
                    self.pageNumberTHXK(_pageNumber);
                    self.pageNumberTHXKCT(_pageNumber);
                }
                self.ReserPage();
            }
        };
        self.EndPage = function(item) {
            _pageNumber = AllPage;
            if (_kieubang == 1)
                self.pageNumberBH(_pageNumber);
            else if (_kieubang == 2)
                self.pageNumberLN(_pageNumber);
            else if (_kieubang == 3)
                self.pageNumberXNT(_pageNumber);
            else if (_kieubang == 4)
                self.pageNumberXNTCT(_pageNumber);
            else if (_kieubang == 5)
                self.pageNumberXH(_pageNumber);
            else if (_kieubang == 6)
                self.pageNumberNV(_pageNumber);
            else if (_kieubang == 7)
                self.pageNumberKH(_pageNumber);
            else if (_kieubang == 8)
                self.pageNumberNCC(_pageNumber);
            else if (_kieubang == 9) {
                if (_selectTab == 1)
                    self.pageNumberCH(_pageNumber);
                else
                    self.pageNumberHDCH(_pageNumber);
            }
            else if (_kieubang == 10) {
                if (dk_tabncc == 1) {
                    if (_selectTabncc == 1)
                        self.pageNumberNHNCC(_pageNumber);
                    else
                        self.pageNumberNHCTNCC(_pageNumber);
                }
                else if (dk_tabncc == 2) {
                    if (_selectTabncc == 1)
                        self.pageNumberTHN(_pageNumber);
                    else
                        self.pageNumberTHNCT(_pageNumber);
                }
            }
            else if (_kieubang == 11) {
                self.pageNumberTK(_pageNumber);
                self.pageNumberTHNK(_pageNumber);
                self.pageNumberTHNKCT(_pageNumber);
                self.pageNumberTHXK(_pageNumber);
                self.pageNumberTHXKCT(_pageNumber);
            }
            self.ReserPage();
        };
        self.StartPage = function(item) {
            _pageNumber = 1;
            if (_kieubang == 1)
                self.pageNumberBH(_pageNumber);
            else if (_kieubang == 2)
                self.pageNumberLN(_pageNumber);
            else if (_kieubang == 3)
                self.pageNumberXNT(_pageNumber);
            else if (_kieubang == 4)
                self.pageNumberXNTCT(_pageNumber);
            else if (_kieubang == 5)
                self.pageNumberXH(_pageNumber);
            else if (_kieubang == 6)
                self.pageNumberNV(_pageNumber);
            else if (_kieubang == 7)
                self.pageNumberKH(_pageNumber);
            else if (_kieubang == 8)
                self.pageNumberNCC(_pageNumber);
            else if (_kieubang == 9) {
                if (_selectTab == 1)
                    self.pageNumberCH(_pageNumber);
                else
                    self.pageNumberHDCH(_pageNumber);
            }
            else if (_kieubang == 10) {
                if (dk_tabncc == 1) {
                    if (_selectTabncc == 1)
                        self.pageNumberNHNCC(_pageNumber);
                    else
                        self.pageNumberNHCTNCC(_pageNumber);
                }
                else if (dk_tabncc == 2) {
                    if (_selectTabncc == 1)
                        self.pageNumberTHN(_pageNumber);
                    else
                        self.pageNumberTHNCT(_pageNumber);
                }
            }
            else if (_kieubang == 11) {
                self.pageNumberTK(_pageNumber);
                self.pageNumberTHNK(_pageNumber);
                self.pageNumberTHNKCT(_pageNumber);
                self.pageNumberTHXK(_pageNumber);
                self.pageNumberTHXKCT(_pageNumber);
            }
            self.ReserPage();
        };
        self.gotoNextPage = function(item) {
            _pageNumber = item.SoTrang;
            if (_kieubang == 1)
                self.pageNumberBH(_pageNumber);
            else if (_kieubang == 2)
                self.pageNumberLN(_pageNumber);
            else if (_kieubang == 3)
                self.pageNumberXNT(_pageNumber);
            else if (_kieubang == 4)
                self.pageNumberXNTCT(_pageNumber);
            else if (_kieubang == 5)
                self.pageNumberXH(_pageNumber);
            else if (_kieubang == 6)
                self.pageNumberNV(_pageNumber);
            else if (_kieubang == 7)
                self.pageNumberKH(_pageNumber);
            else if (_kieubang == 8)
                self.pageNumberNCC(_pageNumber);
            else if (_kieubang == 9) {
                if (_selectTab == 1)
                    self.pageNumberCH(_pageNumber);
                else
                    self.pageNumberHDCH(_pageNumber);
            }
            else if (_kieubang == 10) {
                if (dk_tabncc == 1) {
                    if (_selectTabncc == 1)
                        self.pageNumberNHNCC(_pageNumber);
                    else
                        self.pageNumberNHCTNCC(_pageNumber);
                }
                else if (dk_tabncc == 2) {
                    if (_selectTabncc == 1)
                        self.pageNumberTHN(_pageNumber);
                    else
                        self.pageNumberTHNCT(_pageNumber);
                }
            }
            else if (_kieubang == 11) {
                self.pageNumberTK(_pageNumber);
                self.pageNumberTHNK(_pageNumber);
                self.pageNumberTHNKCT(_pageNumber);
                self.pageNumberTHXK(_pageNumber);
                self.pageNumberTHXKCT(_pageNumber);
            }
            self.ReserPage();
        }
        //Xuất file excel

        $('#hhma').click(function() {
            $(".hhma").toggle();
            self.addColum(1, $(this).val());
            addClass(".hhma", "hhma", $(this).val(), "banhangHH");
        });
        $('#hhname').click(function() {
            $(".hhname").toggle();
            self.addColum(1, $(this).val());
            addClass(".hhname", "hhname", $(this).val(), "banhangHH");
        });
        $('#hhslban  ').click(function() {
            $(".hhslban  ").toggle();
            self.addColum(1, $(this).val());
            addClass(".hhslban", "hhslban", $(this).val(), "banhangHH");
        });
        $('#hhgiatri  ').click(function() {
            $(".hhgiatri  ").toggle();
            self.addColum(1, $(this).val());
            addClass(".hhgiatri", "hhgiatri", $(this).val(), "banhangHH");
        });
        $('#hhsltra  ').click(function() {
            $(".hhsltra  ").toggle();
            self.addColum(1, $(this).val());
            addClass(".hhsltra", "hhsltra", $(this).val(), "banhangHH");
        });
        $('#hhgiatritra  ').click(function() {
            $(".hhgiatritra  ").toggle();
            self.addColum(1, $(this).val());
            addClass(".hhgiatritra", "hhgiatritra", $(this).val(), "banhangHH");
        });
        $('#hhdoanhthu   ').click(function() {
            $(".hhdoanhthu   ").toggle();
            self.addColum(1, $(this).val());
            addClass(".hhdoanhthu", "hhdoanhthu", $(this).val(), "banhangHH");
        });


        $('#lnma ').click(function() {
            $(".lnma ").toggle();
            self.addColum(2, $(this).val());
            addClass(".lnma", "lnma", $(this).val(), "loinhuanHH");
        });
        $('#lnname ').click(function() {
            $(".lnname ").toggle();
            self.addColum(2, $(this).val());
            addClass(".lnname", "lnname", $(this).val(), "loinhuanHH");
        });
        $('#lnslban   ').click(function() {
            $(".lnslban   ").toggle();
            self.addColum(2, $(this).val());
            addClass(".lnslban", "lnslban", $(this).val(), "loinhuanHH");
        });
        $('#lndoanhthu   ').click(function() {
            $(".lndoanhthu   ").toggle();
            self.addColum(2, $(this).val());
            addClass(".lndoanhthu", "lndoanhthu", $(this).val(), "loinhuanHH");
        });
        $('#lnsltra   ').click(function() {
            $(".lnsltra   ").toggle();
            self.addColum(2, $(this).val());
            addClass(".lnsltra", "lnsltra", $(this).val(), "loinhuanHH");
        });
        $('#lngttra   ').click(function() {
            $(".lngttra   ").toggle();
            self.addColum(2, $(this).val());
            addClass(".lngttra", "lngttra", $(this).val(), "loinhuanHH");
        });
        $('#lndoanhthuthuan    ').click(function() {
            $(".lndoanhthuthuan    ").toggle();
            self.addColum(2, $(this).val());
            addClass(".lndoanhthuthuan", "lndoanhthuthuan", $(this).val(), "loinhuanHH");
        });
        $('#lntongvon    ').click(function() {
            $(".lntongvon    ").toggle();
            self.addColum(2, $(this).val());
            addClass(".lntongvon", "lntongvon", $(this).val(), "loinhuanHH");
        });
        $('#lnloinhuan    ').click(function() {
            $(".lnloinhuan    ").toggle();
            self.addColum(2, $(this).val());
            addClass(".lnloinhuan", "lnloinhuan", $(this).val(), "loinhuanHH");
        });
        $('#lntysuat     ').click(function() {
            $(".lntysuat     ").toggle();
            self.addColum(2, $(this).val());
            addClass(".lntysuat", "lntysuat", $(this).val(), "loinhuanHH");
        });


        $('#xnma ').click(function() {
            $(".xnma ").toggle();
            self.addColum(3, $(this).val());
            addClass(".xnma", "xnma", $(this).val(), "xuatnhaptonHH");
        });
        $('#xnname ').click(function() {
            $(".xnname ").toggle();
            self.addColum(3, $(this).val());
            addClass(".xnname", "xnname", $(this).val(), "xuatnhaptonHH");
        });
        $('#xnton    ').click(function() {
            $(".xnton    ").toggle();
            self.addColum(3, $(this).val());
            addClass(".xnton", "xnton", $(this).val(), "xuatnhaptonHH");
        });
        $('#xngiatri    ').click(function() {
            $(".xngiatri    ").toggle();
            self.addColum(3, $(this).val());
            addClass(".xngiatri", "xngiatri", $(this).val(), "xuatnhaptonHH");
        });
        $('#xngiatrinhap    ').click(function() {
            $(".xngiatrinhap    ").toggle();
            self.addColum(3, $(this).val());
            addClass(".xngiatrinhap", "xngiatrinhap", $(this).val(), "xuatnhaptonHH");
        });
        $('#xnslnhap    ').click(function() {
            $(".xnslnhap    ").toggle();
            self.addColum(3, $(this).val());
            addClass(".xnslnhap", "xnslnhap", $(this).val(), "xuatnhaptonHH");
        });
        $('#xnslxuat     ').click(function() {
            $(".xnslxuat     ").toggle();
            self.addColum(3, $(this).val());
            addClass(".xnslxuat", "xnslxuat", $(this).val(), "xuatnhaptonHH");
        });
        $('#xngtxuat     ').click(function() {
            $(".xngtxuat     ").toggle();
            self.addColum(3, $(this).val());
            addClass(".xngtxuat", "xngtxuat", $(this).val(), "xuatnhaptonHH");
        });
        $('#xntoncuoi     ').click(function() {
            $(".xntoncuoi     ").toggle();
            self.addColum(3, $(this).val());
            addClass(".xntoncuoi", "xntoncuoi", $(this).val(), "xuatnhaptonHH");
        });
        $('#xngtcuoi      ').click(function() {
            $(".xngtcuoi      ").toggle();
            self.addColum(3, $(this).val());
            addClass(".xngtcuoi", "xngtcuoi", $(this).val(), "xuatnhaptonHH");
        });

        $('#khma      ').click(function() {
            $(".khma    ").toggle();
            self.addColum(4, $(this).val());
            addClass(".khma", "khma", $(this).val(), "khachtheohangbanHH");
        });

        $('#khname     ').click(function() {
            $(".khname     ").toggle();
            self.addColum(4, $(this).val());
            addClass(".khname", "khname", $(this).val(), "khachtheohangbanHH");
        });
        $('#khsl      ').click(function() {
            $(".khsl      ").toggle();
            self.addColum(4, $(this).val());
            addClass(".khsl", "khsl", $(this).val(), "khachtheohangbanHH");
        });
        $('#khslmua      ').click(function() {
            $(".khslmua      ").toggle();
            self.addColum(4, $(this).val());
            addClass(".khslmua", "khslmua", $(this).val(), "khachtheohangbanHH");
        });
        $('#khgiatri       ').click(function() {
            $(".khgiatri       ").toggle();
            self.addColum(4, $(this).val());
            addClass(".khgiatri", "khgiatri", $(this).val(), "khachtheohangbanHH");
        });

        $('#nvma      ').click(function() {
            $(".nvma    ").toggle();
            self.addColum(5, $(this).val());
            addClass(".nvma", "nvma", $(this).val(), "nhanvientheohangbanHH");
        });
        $('#nvname     ').click(function() {
            $(".nvname     ").toggle();
            self.addColum(5, $(this).val());
            addClass(".nvname", "nvname", $(this).val(), "nhanvientheohangbanHH");
        });
        $('#nvsl       ').click(function() {
            $(".nvsl       ").toggle();
            self.addColum(5, $(this).val());
            addClass(".nvsl", "nvsl", $(this).val(), "nhanvientheohangbanHH");
        });
        $('#nvslban       ').click(function() {
            $(".nvslban       ").toggle();
            self.addColum(5, $(this).val());
            addClass(".nvslban", "nvslban", $(this).val(), "nhanvientheohangbanHH");
        });
        $('#nvgiatri        ').click(function() {
            $(".nvgiatri        ").toggle();
            self.addColum(5, $(this).val());
            addClass(".nvgiatri", "nvgiatri", $(this).val(), "nhanvientheohangbanHH");
        });

        $('#nccma      ').click(function() {
            $(".nccma    ").toggle();
            self.addColum(6, $(this).val());
            addClass(".nccma", "nccma", $(this).val(), "NCCtheohangnhapHH");
        });
        $('#nccname     ').click(function() {
            $(".nccname     ").toggle();
            self.addColum(6, $(this).val());
            addClass(".nccname", "nccname", $(this).val(), "NCCtheohangnhapHH");
        });
        $('#nccsl        ').click(function() {
            $(".nccsl        ").toggle();
            self.addColum(6, $(this).val());
            addClass(".nccsl", "nccsl", $(this).val(), "NCCtheohangnhapHH");
        });
        $('#nccslsanpham        ').click(function() {
            $(".nccslsanpham        ").toggle();
            self.addColum(6, $(this).val());
            addClass(".nccslsanpham", "nccslsanpham", $(this).val(), "NCCtheohangnhapHH");
        });
        $('#nccgiatri         ').click(function() {
            $(".nccgiatri         ").toggle();
            self.addColum(6, $(this).val());
            addClass(".nccgiatri", "nccgiatri", $(this).val(), "NCCtheohangnhapHH");
        });

        $('#xntoncuoi').click(function() {
            $(".xntoncuoi        ").toggle();
            self.addColum(7, $(this).val());
            addClass(".xntoncuoi", "xntoncuoi", $(this).val(), "xuatnhaptonchitietHH");
        });
        $('#xntgtcuoi').click(function() {
            $(".xntgtcuoi").toggle();
            self.addColum(7, $(this).val());
            addClass(".xntgtcuoi", "xntgtcuoi", $(this).val(), "xuatnhaptonchitietHH");
        });
        $('#xntxuat').click(function() {
            $(".xntxuat").toggle();
            self.addColum(7, $(this).val());
            addClass(".xntxuat", "xntxuat", $(this).val(), "xuatnhaptonchitietHH");
        });
        $('#xntslnhap').click(function() {
            $(".xntslnhap").toggle();
            self.addColum(7, $(this).val());
            addClass(".xntslnhap", "xntslnhap", $(this).val(), "xuatnhaptonchitietHH");
        });
        $('#xntgiatri').click(function() {
            $(".xntgiatri").toggle();
            self.addColum(7, $(this).val());
            addClass(".xntgiatri", "xntgiatri", $(this).val(), "xuatnhaptonchitietHH");
        });
        $('#xntton').click(function() {
            $(".xntton").toggle();
            self.addColum(7, $(this).val());
            addClass(".xntton", "xntton", $(this).val(), "xuatnhaptonchitietHH");
        });
        $('#xntname').click(function() {
            $(".xntname").toggle();
            self.addColum(7, $(this).val());
            addClass(".xntname", "xntname", $(this).val(), "xuatnhaptonchitietHH");
        });
        $('#xntma').click(function() {
            $(".xntma").toggle();
            self.addColum(7, $(this).val());
            addClass(".xntma", "xntma", $(this).val(), "xuatnhaptonchitietHH");
        });

        $('#xhma').click(function() {
            $(".xhma").toggle();
            self.addColum(8, $(this).val());
            addClass(".xhma", "xhma", $(this).val(), "xuathuyHH");
        });
        $('#xhname').click(function() {
            $(".xhname").toggle();
            self.addColum(8, $(this).val());
            addClass(".xhname", "xhname", $(this).val(), "xuathuyHH");
        });
        $('#xhsl').click(function() {
            $(".xhsl").toggle();
            self.addColum(8, $(this).val());
            addClass(".xhsl", "xhsl", $(this).val(), "xuathuyHH");
        });
        $('#xhgiarihuy').click(function() {
            $(".xhgiarihuy").toggle();
            self.addColum(8, $(this).val());
            addClass(".xhgiarihuy", "xhgiarihuy", $(this).val(), "xuathuyHH");
        });

        $('#thnma ').click(function() {
            $(".thnma ").toggle();
            self.addColum(9, $(this).val());
            addClass(".thnma", "thnma", $(this).val(), "nhaphangNCC");
        });
        $('#thnname').click(function() {
            $(".thnname").toggle();
            self.addColum(9, $(this).val());
            addClass(".thnname", "thnname", $(this).val(), "nhaphangNCC");
        });
        $('#thnsl ').click(function() {
            $(".thnsl ").toggle();
            self.addColum(9, $(this).val());
            addClass(".thnsl", "thnsl", $(this).val(), "nhaphangNCC");
        });
        $('#thngiatritra ').click(function() {
            $(".thngiatritra ").toggle();
            self.addColum(9, $(this).val());
            addClass(".thngiatritra", "thngiatritra", $(this).val(), "nhaphangNCC");
        });

        $("#thntqcode").click(function() {
            $(".thntqcode").toggle();
            self.addColum(19, $(this).val());
            addClass(".thntqcode", "thntqcode", $(this).val(), "trahangnhapHH");
        });
        $("#thntqname").click(function() {
            $(".thntqname").toggle();
            self.addColum(19, $(this).val());
            addClass(".thntqname", "thntqname", $(this).val(), "trahangnhapHH");
        });
        $("#thntqnumber").click(function() {
            $(".thntqnumber").toggle();
            self.addColum(19, $(this).val());
            addClass(".thntqnumber", "thntqnumber", $(this).val(), "trahangnhapHH");
        });
        $("#thntqgiatri").click(function() {
            $(".thntqgiatri").toggle();
            self.addColum(19, $(this).val());
            addClass(".thntqgiatri", "thntqgiatri", $(this).val(), "trahangnhapHH");
        });

        $("#thnctma ").click(function() {
            $(".thnctma ").toggle();
            self.addColum(18, $(this).val());
            addClass(".thnctma", "thnctma", $(this).val(), "chitietnhaphangNCC");
        });
        $("#thmctngay ").click(function() {
            $(".thmctngay ").toggle();
            self.addColum(18, $(this).val());
            addClass(".thmctngay", "thmctngay", $(this).val(), "chitietnhaphangNCC");
        });
        $("#thnctmahang ").click(function() {
            $(".thnctmahang ").toggle();
            self.addColum(18, $(this).val());
            addClass(".thnctmahang", "thnctmahang", $(this).val(), "chitietnhaphangNCC");
        });
        $("#thnctten ").click(function() {
            $(".thnctten ").toggle();
            self.addColum(18, $(this).val());
            addClass(".thnctten", "thnctten", $(this).val(), "chitietnhaphangNCC");
        });
        $("#thnctsoluong ").click(function() {
            $(".thnctsoluong ").toggle();
            self.addColum(18, $(this).val());
            addClass(".thnctsoluong", "thnctsoluong", $(this).val(), "chitietnhaphangNCC");
        });
        $("#thnctdongia ").click(function() {
            $(".thnctdongia ").toggle();
            self.addColum(18, $(this).val());
            addClass(".thnctdongia", "thnctdongia", $(this).val(), "chitietnhaphangNCC");
        });
        $("#thnctthanhtien ").click(function() {
            $(".thnctthanhtien ").toggle();
            self.addColum(18, $(this).val());
            addClass(".thnctthanhtien", "thnctthanhtien", $(this).val(), "chitietnhaphangNCC");
        });

        $("#thnctma1 ").click(function() {
            $(".thnctma1 ").toggle();
            self.addColum(20, $(this).val());
            addClass(".thnctma1", "thnctma1", $(this).val(), "trahangnhapchitietHH");
        });
        $("#thmctngay1 ").click(function() {
            $(".thmctngay1 ").toggle();
            self.addColum(20, $(this).val());
            addClass(".thmctngay1", "thmctngay1", $(this).val(), "trahangnhapchitietHH");
        });
        $("#thnctmahang1 ").click(function() {
            $(".thnctmahang1 ").toggle();
            self.addColum(20, $(this).val());
            addClass(".thnctmahang1", "thnctmahang1", $(this).val(), "trahangnhapchitietHH");
        });
        $("#thnctten1 ").click(function() {
            $(".thnctten1 ").toggle();
            self.addColum(20, $(this).val());
            addClass(".thnctten1", "thnctten1", $(this).val(), "trahangnhapchitietHH");
        });
        $("#thnctsoluong1 ").click(function() {
            $(".thnctsoluong1 ").toggle();
            self.addColum(20, $(this).val());
            addClass(".thnctsoluong1", "thnctsoluong1", $(this).val(), "trahangnhapchitietHH");
        });
        $("#thnctdongia1 ").click(function() {
            $(".thnctdongia1 ").toggle();
            self.addColum(20, $(this).val());
            addClass(".thnctdongia1", "thnctdongia1", $(this).val(), "trahangnhapchitietHH");
        });
        $("#thnctthanhtien1 ").click(function() {
            $(".thnctthanhtien1 ").toggle();
            self.addColum(20, $(this).val());
            addClass(".thnctthanhtien1", "thnctthanhtien1", $(this).val(), "trahangnhapchitietHH");
        });

        $('#chxuattq').click(function() {
            $(".chxuattq ").toggle();
            self.addColum(10, $(this).val());
            addClass(".chxuattq", "chxuattq", $(this).val(), "chuyenhangxuatTQHH");
        });
        $('#chxuatnametq').click(function() {
            $(".chxuatnametq").toggle();
            self.addColum(10, $(this).val());
            addClass(".chxuatnametq", "chxuatnametq", $(this).val(), "chuyenhangxuatTQHH");
        });
        $('#chxuatsltq').click(function() {
            $(".chxuatsltq").toggle();
            self.addColum(10, $(this).val());
            addClass(".chxuatsltq", "chxuatsltq", $(this).val(), "chuyenhangxuatTQHH");
        });
        $('#chxuatgiatrichuyentq').click(function() {
            $(".chxuatgiatrichuyentq ").toggle();
            self.addColum(10, $(this).val());
            addClass(".chxuatgiatrichuyentq", "chxuatgiatrichuyentq", $(this).val(), "chuyenhangxuatTQHH");
        });

        $('#chxuatmact').click(function() {
            $(".chxuatmact ").toggle();
            self.addColum(11, $(this).val());
            addClass(".chxuatmact", "chxuatmact", $(this).val(), "chuyenhangxuatCTHH");
        });
        $('#chxuatngayct').click(function() {
            $(".chxuatngayct").toggle();
            self.addColum(11, $(this).val());
            addClass(".chxuatngayct", "chxuatngayct", $(this).val(), "chuyenhangxuatCTHH");
        });
        $('#chxuatmahangt').click(function() {
            $(".chxuatmahangt").toggle();
            self.addColum(11, $(this).val());
            addClass(".chxuatmahangt", "chxuatmahangt", $(this).val(), "chuyenhangxuatCTHH");
        });
        $('#chxuatnamect').click(function() {
            $(".chxuatnamect ").toggle();
            self.addColum(11, $(this).val());
            addClass(".chxuatnamect", "chxuatnamect", $(this).val(), "chuyenhangxuatCTHH");
        });
        $('#chxuatchinhanhct').click(function() {
            $(".chxuatchinhanhct ").toggle();
            self.addColum(11, $(this).val());
            addClass(".chxuatchinhanhct", "chxuatchinhanhct", $(this).val(), "chuyenhangxuatCTHH");
        });
        $('#chxuatchinhanhnhanct').click(function() {
            $(".chxuatchinhanhnhanct").toggle();
            self.addColum(11, $(this).val());
            addClass(".chxuatchinhanhnhanct", "chxuatchinhanhnhanct", $(this).val(), "chuyenhangxuatCTHH");
        });
        $('#chxuatslct ').click(function() {
            $(".chxuatslct ").toggle();
            self.addColum(11, $(this).val());
            addClass(".chxuatslct", "chxuatslct", $(this).val(), "chuyenhangxuatCTHH");
        });
        $('#chxuatdongiact ').click(function() {
            $(".chxuatdongiact  ").toggle();
            self.addColum(11, $(this).val());
            addClass(".chxuatdongiact", "chxuatdongiact", $(this).val(), "chuyenhangxuatCTHH");
        });
        $('#chxuatthanhtienct  ').click(function() {
            $(".chxuatthanhtienct   ").toggle();
            self.addColum(11, $(this).val());
            addClass(".chxuatthanhtienct", "chxuatthanhtienct", $(this).val(), "chuyenhangxuatCTHH");
        });

        $('#chnhaptq').click(function() {
            $(".chnhaptq").toggle();
            self.addColum(12, $(this).val());
            addClass(".chnhaptq", "chnhaptq", $(this).val(), "chuyenhangnhapTQHH");
        });
        $('#chnhapnametq ').click(function() {
            $(".chnhapnametq ").toggle();
            self.addColum(12, $(this).val());
            addClass(".chnhapnametq", "chnhapnametq", $(this).val(), "chuyenhangnhapTQHH");
        });
        $('#chnhapsltq').click(function() {
            $(".chnhapsltq").toggle();
            self.addColum(12, $(this).val());
            addClass(".chnhapsltq", "chnhapsltq", $(this).val(), "chuyenhangnhapTQHH");
        });
        $('#chnhapgiatritq').click(function() {
            $(".chnhapgiatritq").toggle();
            self.addColum(12, $(this).val());
            addClass(".chnhapgiatritq", "chnhapgiatritq", $(this).val(), "chuyenhangnhapTQHH");
        });

        $('#tknhomhang  ').click(function() {
            $(".tknhomhang   ").toggle();
            self.addColum(13, $(this).val());
            addClass(".tknhomhang", "tknhomhang", $(this).val(), "tonkhoHH");
        });
        $('#tkma').click(function() {
            $(".tkma").toggle();
            self.addColum(13, $(this).val());
            addClass(".tkma", "tkma", $(this).val(), "tonkhoHH");
        });
        $('#tkname ').click(function() {
            $(".tkname ").toggle();
            self.addColum(13, $(this).val());
            addClass(".tkname", "tkname", $(this).val(), "tonkhoHH");
        });
        $('#tksl ').click(function() {
            $(".tksl ").toggle();
            self.addColum(13, $(this).val());
            addClass(".tksl", "tksl", $(this).val(), "tonkhoHH");
        });
        $('#tkgiatri ').click(function() {
            $(".tkgiatri ").toggle();
            self.addColum(13, $(this).val());
            addClass(".tkgiatri", "tkgiatri", $(this).val(), "tonkhoHH");
        });

        $('#tknknhomhang  ').click(function() {
            $(".tknknhomhang   ").toggle();
            self.addColum(14, $(this).val());
            addClass(".tknknhomhang", "tknknhomhang", $(this).val(), "tonghopnhapkhoTQHH");
        });
        $('#tknkma').click(function() {
            $(".tknkma").toggle();
            self.addColum(14, $(this).val());
            addClass(".tknkma", "tknkma", $(this).val(), "tonghopnhapkhoTQHH");
        });
        $('#tknkname ').click(function() {
            $(".tknkname ").toggle();
            self.addColum(14, $(this).val());
            addClass(".tknkname", "tknkname", $(this).val(), "tonghopnhapkhoTQHH");
        });
        $('#tknksl  ').click(function() {
            $(".tknksl  ").toggle();
            self.addColum(14, $(this).val());
            addClass(".tknksl", "tknksl", $(this).val(), "tonghopnhapkhoTQHH");
        });
        $('#tknkgiatri  ').click(function() {
            $(".tknkgiatri  ").toggle();
            self.addColum(14, $(this).val());
            addClass(".tknkgiatri", "tknkgiatri", $(this).val(), "tonghopnhapkhoTQHH");
        });

        $('#tkxknhomhang  ').click(function() {
            $(".tkxknhomhang   ").toggle();
            self.addColum(15, $(this).val());
            addClass(".tkxknhomhang", "tkxknhomhang", $(this).val(), "tonghopxuatkhoTQHH");
        });
        $('#tkxkma').click(function() {
            $(".tkxkma").toggle();
            self.addColum(15, $(this).val());
            addClass(".tkxkma", "tkxkma", $(this).val(), "tonghopxuatkhoTQHH");
        });
        $('#tkxkname ').click(function() {
            $(".tkxkname ").toggle();
            self.addColum(15, $(this).val());
            addClass(".tkxkname", "tkxkname", $(this).val(), "tonghopxuatkhoTQHH");
        });
        $('#tkxksl   ').click(function() {
            $(".tkxksl   ").toggle();
            self.addColum(15, $(this).val());
            addClass(".tkxksl", "tkxksl", $(this).val(), "tonghopxuatkhoTQHH");
        });
        $('#tkxkgiatri  ').click(function() {
            $(".tkxkgiatri  ").toggle();
            self.addColum(15, $(this).val());
            addClass(".tkxkgiatri", "tkxkgiatri", $(this).val(), "tonghopxuatkhoTQHH");
        });

        $('#tknhaploaict  ').click(function() {
            $(".tknhaploaict   ").toggle();
            self.addColum(16, $(this).val());
            addClass(".tknhaploaict", "tknhaploaict", $(this).val(), "tonghopnhapkhoCTHH");
        });
        $('#tknhapma').click(function() {
            $(".tknhapma").toggle();
            self.addColum(16, $(this).val());
            addClass(".tknhapma", "tknhapma", $(this).val(), "tonghopnhapkhoCTHH");
        });
        $('#tknhapngay ').click(function() {
            $(".tknhapngay ").toggle();
            self.addColum(16, $(this).val());
            addClass(".tknhapngay", "tknhapngay", $(this).val(), "tonghopnhapkhoCTHH");
        });
        $('#tknhapnhom   ').click(function() {
            $(".tknhapnhom   ").toggle();
            self.addColum(16, $(this).val());
            addClass(".tknhapnhom", "tknhapnhom", $(this).val(), "tonghopnhapkhoCTHH");
        });
        $('#tknhapmahang  ').click(function() {
            $(".tknhapmahang   ").toggle();
            self.addColum(16, $(this).val());
            addClass(".tknhapmahang", "tknhapmahang", $(this).val(), "tonghopnhapkhoCTHH");
        });
        $('#tknhapname').click(function() {
            $(".tknhapname").toggle();
            self.addColum(16, $(this).val());
            addClass(".tknhapname", "tknhapname", $(this).val(), "tonghopnhapkhoCTHH");
        });
        $('#tknhapsl  ').click(function() {
            $(".tknhapsl  ").toggle();
            self.addColum(16, $(this).val());
            addClass(".tknhapsl", "tknhapsl", $(this).val(), "tonghopnhapkhoCTHH");
        });
        $('#tknhapgiatrinhap').click(function() {
            $(".tknhapgiatrinhap ").toggle();
            self.addColum(16, $(this).val());
            addClass(".tknhapgiatrinhap", "tknhapgiatrinhap", $(this).val(), "tonghopnhapkhoCTHH");
        });

        $('#tkxuatloaict  ').click(function() {
            $(".tkxuatloaict   ").toggle();
            self.addColum(17, $(this).val());
            addClass(".tkxuatloaict", "tkxuatloaict", $(this).val(), "tonghopxuatkhoCTHH");
        });
        $('#tkxuatpma').click(function() {
            $(".tkxuatpma").toggle();
            self.addColum(17, $(this).val());
            addClass(".tkxuatpma", "tkxuatpma", $(this).val(), "tonghopxuatkhoCTHH");
        });
        $('#tkxuatngay ').click(function() {
            $(".tkxuatngay ").toggle();
            self.addColum(17, $(this).val());
            addClass(".tkxuatngay", "tkxuatngay", $(this).val(), "tonghopxuatkhoCTHH");
        });
        $('#tkxuatnhom   ').click(function() {
            $(".tkxuatnhom   ").toggle();
            self.addColum(17, $(this).val());
            addClass(".tkxuatnhom", "tkxuatnhom", $(this).val(), "tonghopxuatkhoCTHH");
        });
        $('#tkxuatmahang  ').click(function() {
            $(".tkxuatmahang   ").toggle();
            self.addColum(17, $(this).val());
            addClass(".tkxuatmahang", "tkxuatmahang", $(this).val(), "tonghopxuatkhoCTHH");
        });
        $('#tkxuatname').click(function() {
            $(".tkxuatname").toggle();
            self.addColum(17, $(this).val());
            addClass(".tkxuatname", "tkxuatname", $(this).val(), "tonghopxuatkhoCTHH");
        });
        $('#tkxuatsl  ').click(function() {
            $(".tkxuatsl  ").toggle();
            self.addColum(17, $(this).val());
            addClass(".tkxuatsl", "tkxuatsl", $(this).val(), "tonghopxuatkhoCTHH");
        });
        $('#tkxuatgiatrinhap').click(function() {
            $(".tkxuatgiatrinhap ").toggle();
            self.addColum(17, $(this).val());
            addClass(".tkxuatgiatrinhap", "tkxuatgiatrinhap", $(this).val(), "tonghopxuatkhoCTHH");
        });
        self.addColum = function(values, item) {
            switch (values) {
                case 1:
                    if (self.ColumnsExcelBH().length < 1) {
                        self.ColumnsExcelBH.push(item);
                    }
                    else {
                        for (var i = 0; i < self.ColumnsExcelBH().length; i++) {
                            if (self.ColumnsExcelBH()[i] === item) {
                                self.ColumnsExcelBH.splice(i, 1);
                                break;
                            }
                            if (i == self.ColumnsExcelBH().length - 1) {
                                self.ColumnsExcelBH.push(item);
                                break;
                            }
                        }
                    }
                    self.ColumnsExcelBH.sort();
                    break;
                case 2:
                    if (self.ColumnsExcelLN().length < 1) {
                        self.ColumnsExcelLN.push(item);
                    }
                    else {
                        for (var i = 0; i < self.ColumnsExcelLN().length; i++) {
                            if (self.ColumnsExcelLN()[i] === item) {
                                self.ColumnsExcelLN.splice(i, 1);
                                break;
                            }
                            if (i == self.ColumnsExcelLN().length - 1) {
                                self.ColumnsExcelLN.push(item);
                                break;
                            }
                        }
                    }
                    self.ColumnsExcelLN.sort();
                    break;
                case 3:
                    if (self.ColumnsExcelXNT().length < 1) {
                        self.ColumnsExcelXNT.push(item);
                    }
                    else {
                        for (var i = 0; i < self.ColumnsExcelXNT().length; i++) {
                            if (self.ColumnsExcelXNT()[i] === item) {
                                self.ColumnsExcelXNT.splice(i, 1);
                                break;
                            }
                            if (i == self.ColumnsExcelXNT().length - 1) {
                                self.ColumnsExcelXNT.push(item);
                                break;
                            }
                        }
                    }
                    self.ColumnsExcelXNT.sort();
                    break;
                case 4:
                    if (self.ColumnsExcelKH().length < 1) {
                        self.ColumnsExcelKH.push(item);
                    }
                    else {
                        for (var i = 0; i < self.ColumnsExcelKH().length; i++) {
                            if (self.ColumnsExcelKH()[i] === item) {
                                self.ColumnsExcelKH.splice(i, 1);
                                break;
                            }
                            if (i == self.ColumnsExcelKH().length - 1) {
                                self.ColumnsExcelKH.push(item);
                                break;
                            }
                        }
                    }
                    self.ColumnsExcelKH.sort();
                    break;
                case 5:
                    if (self.ColumnsExcelNV().length < 1) {
                        self.ColumnsExcelNV.push(item);
                    }
                    else {
                        for (var i = 0; i < self.ColumnsExcelNV().length; i++) {
                            if (self.ColumnsExcelNV()[i] === item) {
                                self.ColumnsExcelNV.splice(i, 1);
                                break;
                            }
                            if (i == self.ColumnsExcelNV().length - 1) {
                                self.ColumnsExcelNV.push(item);
                                break;
                            }
                        }
                    }
                    self.ColumnsExcelNV.sort();
                    break;
                case 6:
                    if (self.ColumnsExcelNCC().length < 1) {
                        self.ColumnsExcelNCC.push(item);
                    }
                    else {
                        for (var i = 0; i < self.ColumnsExcelNCC().length; i++) {
                            if (self.ColumnsExcelNCC()[i] === item) {
                                self.ColumnsExcelNCC.splice(i, 1);
                                break;
                            }
                            if (i == self.ColumnsExcelNCC().length - 1) {
                                self.ColumnsExcelNCC.push(item);
                                break;
                            }
                        }
                    }
                    self.ColumnsExcelNCC.sort();
                    break;
                case 7:
                    if (self.ColumnsExcelXNTCT().length < 1) {
                        self.ColumnsExcelXNTCT.push(item);
                    }
                    else {
                        for (var i = 0; i < self.ColumnsExcelXNTCT().length; i++) {
                            if (self.ColumnsExcelXNTCT()[i] === item) {
                                self.ColumnsExcelXNTCT.splice(i, 1);
                                break;
                            }
                            if (i == self.ColumnsExcelXNTCT().length - 1) {
                                self.ColumnsExcelXNTCT.push(item);
                                break;
                            }
                        }
                    }
                    self.ColumnsExcelXNTCT.sort();
                    break;
                case 8:
                    if (self.ColumnsExcelXH().length < 1) {
                        self.ColumnsExcelXH.push(item);
                    }
                    else {
                        for (var i = 0; i < self.ColumnsExcelXH().length; i++) {
                            if (self.ColumnsExcelXH()[i] === item) {
                                self.ColumnsExcelXH.splice(i, 1);
                                break;
                            }
                            if (i == self.ColumnsExcelXH().length - 1) {
                                self.ColumnsExcelXH.push(item);
                                break;
                            }
                        }
                    }
                    self.ColumnsExcelXH.sort();
                    break;
                case 9:
                    if (self.ColumnsExcelNHNCC().length < 1) {
                        self.ColumnsExcelNHNCC.push(item);
                    }
                    else {
                        for (var i = 0; i < self.ColumnsExcelNHNCC().length; i++) {
                            if (self.ColumnsExcelNHNCC()[i] === item) {
                                self.ColumnsExcelNHNCC.splice(i, 1);
                                break;
                            }
                            if (i == self.ColumnsExcelNHNCC().length - 1) {
                                self.ColumnsExcelNHNCC.push(item);
                                break;
                            }
                        }
                    }
                    self.ColumnsExcelNHNCC.sort();
                    break;
                case 18:
                    if (self.ColumnsExcelCTNHNCC().length < 1) {
                        self.ColumnsExcelCTNHNCC.push(item);
                    }
                    else {
                        for (var i = 0; i < self.ColumnsExcelCTNHNCC().length; i++) {
                            if (self.ColumnsExcelCTNHNCC()[i] === item) {
                                self.ColumnsExcelCTNHNCC.splice(i, 1);
                                break;
                            }
                            if (i == self.ColumnsExcelCTNHNCC().length - 1) {
                                self.ColumnsExcelCTNHNCC.push(item);
                                break;
                            }
                        }
                    }
                    self.ColumnsExcelCTNHNCC.sort();
                    break;
                case 19:
                    if (self.ColumnsExcelTHN().length < 1) {
                        self.ColumnsExcelTHN.push(item);
                    }
                    else {
                        for (var i = 0; i < self.ColumnsExcelTHN().length; i++) {
                            if (self.ColumnsExcelTHN()[i] === item) {
                                self.ColumnsExcelTHN.splice(i, 1);
                                break;
                            }
                            if (i == self.ColumnsExcelTHN().length - 1) {
                                self.ColumnsExcelTHN.push(item);
                                break;
                            }
                        }
                    }
                    self.ColumnsExcelTHN.sort();
                    break;
                case 20:
                    if (self.ColumnsExcelTHNCT().length < 1) {
                        self.ColumnsExcelTHNCT.push(item);
                    }
                    else {
                        for (var i = 0; i < self.ColumnsExcelTHNCT().length; i++) {
                            if (self.ColumnsExcelTHNCT()[i] === item) {
                                self.ColumnsExcelTHNCT.splice(i, 1);
                                break;
                            }
                            if (i == self.ColumnsExcelTHNCT().length - 1) {
                                self.ColumnsExcelTHNCT.push(item);
                                break;
                            }
                        }
                    }
                    self.ColumnsExcelTHNCT.sort();
                    break;
                case 10:
                    if (self.ColumnsExcelXHCTQ().length < 1) {
                        self.ColumnsExcelXHCTQ.push(item);
                    }
                    else {
                        for (var i = 0; i < self.ColumnsExcelXHCTQ().length; i++) {
                            if (self.ColumnsExcelXHCTQ()[i] === item) {
                                self.ColumnsExcelXHCTQ.splice(i, 1);
                                break;
                            }
                            if (i == self.ColumnsExcelXHCTQ().length - 1) {
                                self.ColumnsExcelXHCTQ.push(item);
                                break;
                            }
                        }
                    }
                    self.ColumnsExcelXHCTQ.sort();
                    break;
                case 11:
                    if (self.ColumnsExcelCHCT().length < 1) {
                        self.ColumnsExcelCHCT.push(item);
                    }
                    else {
                        for (var i = 0; i < self.ColumnsExcelCHCT().length; i++) {
                            if (self.ColumnsExcelCHCT()[i] === item) {
                                self.ColumnsExcelCHCT.splice(i, 1);
                                break;
                            }
                            if (i == self.ColumnsExcelCHCT().length - 1) {
                                self.ColumnsExcelCHCT.push(item);
                                break;
                            }
                        }
                    }
                    self.ColumnsExcelCHCT.sort();
                    break;
                case 12:
                    if (self.ColumnsExcelNHCTQ().length < 1) {
                        self.ColumnsExcelNHCTQ.push(item);
                    }
                    else {
                        for (var i = 0; i < self.ColumnsExcelNHCTQ().length; i++) {
                            if (self.ColumnsExcelNHCTQ()[i] === item) {
                                self.ColumnsExcelNHCTQ.splice(i, 1);
                                break;
                            }
                            if (i == self.ColumnsExcelNHCTQ().length - 1) {
                                self.ColumnsExcelNHCTQ.push(item);
                                break;
                            }
                        }
                    }
                    self.ColumnsExcelNHCTQ.sort();
                    break;
                case 13:
                    if (self.ColumnsExcelTK().length < 1) {
                        self.ColumnsExcelTK.push(item);
                    }
                    else {
                        for (var i = 0; i < self.ColumnsExcelTK().length; i++) {
                            if (self.ColumnsExcelTK()[i] === item) {
                                self.ColumnsExcelTK.splice(i, 1);
                                break;
                            }
                            if (i == self.ColumnsExcelTK().length - 1) {
                                self.ColumnsExcelTK.push(item);
                                break;
                            }
                        }
                    }
                    self.ColumnsExcelTK.sort();
                    break;
                case 14:
                    if (self.ColumnsExcelTHHNKTQ().length < 1) {
                        self.ColumnsExcelTHHNKTQ.push(item);
                    }
                    else {
                        for (var i = 0; i < self.ColumnsExcelTHHNKTQ().length; i++) {
                            if (self.ColumnsExcelTHHNKTQ()[i] === item) {
                                self.ColumnsExcelTHHNKTQ.splice(i, 1);
                                break;
                            }
                            if (i == self.ColumnsExcelTHHNKTQ().length - 1) {
                                self.ColumnsExcelTHHNKTQ.push(item);
                                break;
                            }
                        }
                    }
                    self.ColumnsExcelTHHNKTQ.sort();
                    break;
                case 15:
                    if (self.ColumnsExcelTHHXKTQ().length < 1) {
                        self.ColumnsExcelTHHXKTQ.push(item);
                    }
                    else {
                        for (var i = 0; i < self.ColumnsExcelTHHXKTQ().length; i++) {
                            if (self.ColumnsExcelTHHXKTQ()[i] === item) {
                                self.ColumnsExcelTHHXKTQ.splice(i, 1);
                                break;
                            }
                            if (i == self.ColumnsExcelTHHXKTQ().length - 1) {
                                self.ColumnsExcelTHHXKTQ.push(item);
                                break;
                            }
                        }
                    }
                    self.ColumnsExcelTHHXKTQ.sort();
                    break;
                case 16:
                    if (self.ColumnsExcelTHHNKCT().length < 1) {
                        self.ColumnsExcelTHHNKCT.push(item);
                    }
                    else {
                        for (var i = 0; i < self.ColumnsExcelTHHNKCT().length; i++) {
                            if (self.ColumnsExcelTHHNKCT()[i] === item) {
                                self.ColumnsExcelTHHNKCT.splice(i, 1);
                                break;
                            }
                            if (i == self.ColumnsExcelTHHNKCT().length - 1) {
                                self.ColumnsExcelTHHNKCT.push(item);
                                break;
                            }
                        }
                    }
                    self.ColumnsExcelTHHNKCT.sort();
                    break;
                case 17:
                    if (self.ColumnsExcelTHHXKCT().length < 1) {
                        self.ColumnsExcelTHHXKCT.push(item);
                    }
                    else {
                        for (var i = 0; i < self.ColumnsExcelTHHXKCT().length; i++) {
                            if (self.ColumnsExcelTHHXKCT()[i] === item) {
                                self.ColumnsExcelTHHXKCT.splice(i, 1);
                                break;
                            }
                            if (i == self.ColumnsExcelTHHXKCT().length - 1) {
                                self.ColumnsExcelTHHXKCT.push(item);
                                break;
                            }
                        }
                    }
                    self.ColumnsExcelTHHXKCT.sort();

            }
            if (values == 1) {

            }
        }
        //===============================
        // Load lai form lưu cache bộ lọc 
        // trên grid 
        //===============================
        function LoadHtmlGrid(cacheExcel, vals, caches) {
            if (window.localStorage) {
                var current = localStorage.getItem(caches);
                if (!current) {
                    current = [];
                    cacheExcel = false;
                    localStorage.setItem(caches, JSON.stringify(current));
                } else {
                    current = JSON.parse(current);
                    for (var i = 0; i < current.length; i++) {
                        $(current[i].NameClass).addClass("operation");
                        document.getElementById(current[i].NameId).checked = false;
                        if (cacheExcel) {
                            switch (vals) {
                                case 1:
                                    self.addColum(1, current[i].Value);
                                    if (i == current.length - 1)
                                        cacheExcelBH = false;
                                    break;
                                case 2:
                                    self.addColum(2, current[i].Value);
                                    if (i == current.length - 1)
                                        cacheExcelLN = false;
                                    break;
                                case 3:
                                    // làm tiếp
                                    self.addColum(3, current[i].Value);
                                    if (i == current.length - 1)
                                        cacheExcelXNT = false;
                                    break;
                                case 4:
                                    self.addColum(4, current[i].Value);
                                    if (i == current.length - 1)
                                        cacheExcelKH = false;
                                    break;
                                case 5:
                                    self.addColum(5, current[i].Value);
                                    if (i == current.length - 1)
                                        cacheExcelNV = false;
                                    break;
                                case 6:
                                    self.addColum(6, current[i].Value);
                                    if (i == current.length - 1)
                                        cacheExcelNCC = false;
                                    break;
                                case 7:
                                    self.addColum(7, current[i].Value);
                                    if (i == current.length - 1)
                                        cacheExcelXNTCT = false;
                                    break;
                                case 8:
                                    self.addColum(8, current[i].Value);
                                    if (i == current.length - 1)
                                        cacheExcelXH = false;
                                    break;
                                case 9:
                                    self.addColum(9, current[i].Value);
                                    if (i == current.length - 1)
                                        cacheExcelNHNCC = false;
                                    break;
                                case 18:
                                    self.addColum(18, current[i].Value);
                                    if (i == current.length - 1)
                                        cacheExcelCTNHNCC = false;
                                    break;
                                case 19:
                                    self.addColum(19, current[i].Value);
                                    if (i == current.length - 1)
                                        cacheExcelTHN = false;
                                    break;
                                case 20:
                                    self.addColum(20, current[i].Value);
                                    if (i == current.length - 1)
                                        cacheExcelTHNCT = false;
                                    break;
                                case 10:
                                    self.addColum(10, current[i].Value);
                                    if (i == current.length - 1)
                                        cacheExcelXHCTQ = false;
                                    break;
                                case 11:
                                    self.addColum(11, current[i].Value);
                                    if (i == current.length - 1)
                                        cacheExcelCHCT = false;
                                    break;
                                case 12:
                                    self.addColum(12, current[i].Value);
                                    if (i == current.length - 1)
                                        cacheExcelNHCTQ = false;
                                    break;
                                case 13:
                                    self.addColum(13, current[i].Value);
                                    if (i == current.length - 1)
                                        cacheExcelTK = false;
                                    break;
                                case 14:
                                    self.addColum(14, current[i].Value);
                                    if (i == current.length - 1)
                                        cacheExcelTHHNKTQ = false;
                                    break;
                                case 15:
                                    self.addColum(15, current[i].Value);
                                    if (i == current.length - 1)
                                        cacheExcelTHHXKTQ = false;
                                    break;
                                case 16:
                                    self.addColum(16, current[i].Value);
                                    if (i == current.length - 1)
                                        cacheExcelTHHNKCT = false;
                                    break;
                                case 17:
                                    self.addColum(17, current[i].Value);
                                    if (i == current.length - 1)
                                        cacheExcelTHHXKCT = false;
                            }
                        }
                    }
                }
            }
        }

        //===============================
        // Add Các tham số cần lưu lại để
        // cache khi load lại form  
        //===============================
        function addClass(name, id, value, caches) {

            var current = localStorage.getItem(caches);
            if (!current) {
                current = [];
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
            localStorage.setItem(caches, JSON.stringify(current));
        }

        self.ExportExcel = function() {
            var objDiary = {
                ID_NhanVien: _id_NhanVien,
                ID_DonVi: _id_DonVi,
                ChucNang: "Báo cáo hàng hóa",
                NoiDung: "Xuất báo cáo hàng hóa theo " + self.MoiQuanTam(),
                NoiDungChiTiet: "Xuất báo cáo hàng hóa theo " + self.MoiQuanTam(),
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
                success: function(item) {
                    var columnHide = null;
                    var columnHide1 = null;
                    if (_kieubang == 1 && self.ReportHH_BanHang().length != 0) {
                        if (self.BCHH_BanHang() == "BCHH_BanHang" && self.BCHH_BanHang_XuatFile() == "BCHH_BanHang_XuatFile") {
                            for (var i = 0; i < self.ColumnsExcelBH().length; i++) {
                                if (i == 0) {
                                    columnHide = self.ColumnsExcelBH()[i];
                                }
                                else {
                                    columnHide = self.ColumnsExcelBH()[i] + "_" + columnHide;
                                }
                            }
                            var url = ReportUri + "ExportExcelHH_BanHang?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                            window.location.href = url;
                        }
                        else
                            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                    }
                    else if (_kieubang == 2 && self.ReportHH_LoiNhuan().length != 0) {
                        if (self.BCHH_LoiNhuan() == "BCHH_LoiNhuan" && self.BCHH_LoiNhuan_XuatFile() == "BCHH_LoiNhuan") {
                            for (var i = 0; i < self.ColumnsExcelLN().length; i++) {
                                if (i == 0) {
                                    columnHide = self.ColumnsExcelLN()[i];
                                }
                                else {
                                    columnHide = self.ColumnsExcelLN()[i] + "_" + columnHide;
                                }
                            }
                            var url = ReportUri + "ExportExcelHH_LoiNhuan?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh() + "&ID_NguoiDung=" + _IDDoiTuong;
                            window.location.href = url;
                        }
                        else
                            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                    }
                    else if (_kieubang == 3 && self.ReportHH_XuatNhapTon().length != 0) {
                        if (self.BCHH_XuatNhapTon() == "BCHH_XuatNhapTon" && self.BCHH_XuatNhapTonChiTiet() == "BCHH_XuatNhapTonChiTiet") {
                            for (var i = 0; i < self.ColumnsExcelXNT().length; i++) {
                                if (i == 0) {
                                    columnHide = self.ColumnsExcelXNT()[i];
                                }
                                else {
                                    columnHide = self.ColumnsExcelXNT()[i] + "_" + columnHide;
                                }
                            }
                            var url = ReportUri + "ExportExcelHH_XuatNhapTon?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _id_DonVi + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh() + "&TinhTrang=" + TinhTrangHH + "&ID_NguoiDung=" + _IDDoiTuong;
                            window.location.href = url;
                        }
                        else
                            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                    }
                    else if (_kieubang == 4 && self.ReportHH_XuatNhapTonChiTiet().length != 0) {
                        if (self.BCHH_XuatNhapTonChiTiet() == "BCHH_XuatNhapTonChiTiet" && self.BCHH_XuatNhapTonChiTiet_XuatFile() == "BCHH_XuatNhapTonChiTiet_XuatFile") {

                            for (var i = 0; i < self.ColumnsExcelXNTCT().length; i++) {
                                if (i == 0) {
                                    columnHide = self.ColumnsExcelXNTCT()[i];
                                }
                                else {
                                    columnHide = self.ColumnsExcelXNTCT()[i] + "_" + columnHide;
                                }
                            }
                            var url = ReportUri + "ExportExcelHH_XuatNhapTonChiTiet?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _id_DonVi + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh() + "&TinhTrang=" + TinhTrangHH + "&ID_NguoiDung=" + _IDDoiTuong;
                            window.location.href = url;
                        }
                        else
                            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                    }
                    else if (_kieubang == 5 && self.ReportHH_XuatHuy().length != 0) {
                        if (self.BCHH_XuatHuy() == "BCHH_XuatHuy" && self.BCHH_XuatHuy_XuatFile() == "BCHH_XuatHuy_XuatFile") {
                            for (var i = 0; i < self.ColumnsExcelXH().length; i++) {
                                if (i == 0) {
                                    columnHide = self.ColumnsExcelXH()[i];
                                }
                                else {
                                    columnHide = self.ColumnsExcelXH()[i] + "_" + columnHide;
                                }
                            }
                            var url = ReportUri + "ExportExcelHH_XuatHuy?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh() + "&ID_NguoiDung=" + _IDDoiTuong;
                            window.location.href = url;
                        }
                        else
                            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                    }
                    else if (_kieubang == 6 && self.ReportHH_NhanVien().length != 0) {
                        if (self.BCHH_NhanVien() == "BCHH_NhanVien" && self.BCHH_NhanVien_XuatFile() == "BCHH_NhanVien_XuatFile") {
                            for (var i = 0; i < self.ColumnsExcelNV().length; i++) {
                                if (i == 0) {
                                    columnHide = self.ColumnsExcelNV()[i];
                                }
                                else {
                                    columnHide = self.ColumnsExcelNV()[i] + "_" + columnHide;
                                }
                            }
                            var url = ReportUri + "ExportExcelHH_NhanVien?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                            window.location.href = url;
                        }
                        else
                            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                    }
                    else if (_kieubang == 7 && self.ReportHH_KhachHang().length != 0) {
                        if (self.BCHH_KhachHang() == "BCHH_KhachHang" && self.BCHH_KhachHang_XuatFile() == "BCHH_KhachHang_XuatFile") {
                            for (var i = 0; i < self.ColumnsExcelKH().length; i++) {
                                if (i == 0) {
                                    columnHide = self.ColumnsExcelKH()[i];
                                }
                                else {
                                    columnHide = self.ColumnsExcelKH()[i] + "_" + columnHide;
                                }
                            }
                            var url = ReportUri + "ExportExcelHH_KhachHang?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                            window.location.href = url;
                        }
                        else
                            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                    }
                    else if (_kieubang == 8 && self.ReportHH_NhaCungCap().length != 0) {
                        if (self.BCHH_NCC == "BCHH_NCC" && self.BCHH_NCC_XuatFile() == "BCHH_NCC_XuatFile") {
                            for (var i = 0; i < self.ColumnsExcelNCC().length; i++) {
                                if (i == 0) {
                                    columnHide = self.ColumnsExcelNCC()[i];
                                }
                                else {
                                    columnHide = self.ColumnsExcelNCC()[i] + "_" + columnHide;
                                }
                            }
                            var url = ReportUri + "ExportExcelHH_NhaCungCap?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                            window.location.href = url;
                        }
                        else
                            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                    }
                    else if (_kieubang == 9) {
                        if (self.BCHH_ChuyenHang() == "BCHH_ChuyenHang" && self.BCHH_ChuyenHang_XuatFile() == "BCHH_ChuyenHang_XuatFile") {
                            if (_selectTab == 1) {
                                for (var i = 0; i < self.ColumnsExcelXHCTQ().length; i++) {
                                    if (i == 0) {
                                        columnHide = self.ColumnsExcelXHCTQ()[i];
                                    }
                                    else {
                                        columnHide = self.ColumnsExcelXHCTQ()[i] + "_" + columnHide;
                                    }
                                }
                                for (var i = 0; i < self.ColumnsExcelNHCTQ().length; i++) {
                                    if (i == 0) {
                                        columnHide1 = self.ColumnsExcelNHCTQ()[i];
                                    }
                                    else {
                                        columnHide1 = self.ColumnsExcelNHCTQ()[i] + "_" + columnHide1;
                                    }
                                }
                                var url = ReportUri + "ExportExcelHH_ChuyenHang?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&columnsHide=" + columnHide + "&columnsHide1=" + columnHide1 + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh() + "&ID_NguoiDung=" + _IDDoiTuong;
                                window.location.href = url;
                            }
                            else {
                                for (var i = 0; i < self.ColumnsExcelCHCT().length; i++) {
                                    if (i == 0) {
                                        columnHide = self.ColumnsExcelCHCT()[i];
                                    }
                                    else {
                                        columnHide = self.ColumnsExcelCHCT()[i] + "_" + columnHide;
                                    }
                                }
                                var url = ReportUri + "ExportExcelHH_HDChuyenHang?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                                window.location.href = url;
                            }
                        }
                        else
                            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                    }
                    else if (_kieubang == 10) {
                        if (self.BCHH_TraHangNhap() == "BCHH_TraHangNhap" && self.BCHH_TraHangNhap_XuatFile() == "BCHH_TraHangNhap_XuatFile") {
                            if (dk_tabncc == 1) {
                                if (_selectTabncc == 1) {
                                    for (var i = 0; i < self.ColumnsExcelNHNCC().length; i++) {
                                        if (i == 0) {
                                            columnHide = self.ColumnsExcelNHNCC()[i];
                                        }
                                        else {
                                            columnHide = self.ColumnsExcelNHNCC()[i] + "_" + columnHide;
                                        }
                                    }
                                    var url = ReportUri + "ExportExcelHH_NhapHangNCC?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                                    window.location.href = url;
                                }
                                else {
                                    for (var i = 0; i < self.ColumnsExcelCTNHNCC().length; i++) {
                                        if (i == 0) {
                                            columnHide = self.ColumnsExcelCTNHNCC()[i];
                                        }
                                        else {
                                            columnHide = self.ColumnsExcelCTNHNCC()[i] + "_" + columnHide;
                                        }
                                    }
                                    var url = ReportUri + "ExportExcelHH_NhapHangChiTietNCC?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                                    window.location.href = url;
                                }
                            }
                            else {
                                if (_selectTabncc == 1) {
                                    for (var i = 0; i < self.ColumnsExcelTHN().length; i++) {
                                        if (i == 0) {
                                            columnHide = self.ColumnsExcelTHN()[i];
                                        }
                                        else {
                                            columnHide = self.ColumnsExcelTHN()[i] + "_" + columnHide;
                                        }
                                    }
                                    var url = ReportUri + "ExportExcelHH_TraHangNhap?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                                    window.location.href = url;
                                }
                                else {
                                    for (var i = 0; i < self.ColumnsExcelTHNCT().length; i++) {
                                        if (i == 0) {
                                            columnHide = self.ColumnsExcelTHNCT()[i];
                                        }
                                        else {
                                            columnHide = self.ColumnsExcelTHNCT()[i] + "_" + columnHide;
                                        }
                                    }
                                    var url = ReportUri + "ExportExcelHH_TraHangNhapChiTiet?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                                    window.location.href = url;
                                }
                            }
                        }
                        else
                            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                    }
                    else if (_kieubang == 11) {
                        if (self.BCHH_TonKho() == "BCHH_TonKho" && self.BCHH_TonKho_XuatFile() == "BCHH_TonKho_XuatFile") {
                            if (dk_tabtk == 1) {
                                if (self.ReportHH_TonKho().length != 0) {
                                    for (var i = 0; i < self.ColumnsExcelTK().length; i++) {
                                        if (i == 0) {
                                            columnHide = self.ColumnsExcelTK()[i];
                                        }
                                        else {
                                            columnHide = self.ColumnsExcelTK()[i] + "_" + columnHide;
                                        }
                                    }
                                    var url = ReportUri + "ExportExcelHH_TonKho?maHH=" + _maHH + "&timeStart=" + _tonkhoStart + "&timeEnd=" + _tonkhoEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh() + "&TinhTrang=" + TinhTrangHH + "&ID_NguoiDung=" + _IDDoiTuong;
                                    window.location.href = url;
                                }
                                else {
                                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Không có dữ liệu để xuất file excel!", "danger");
                                }
                            }
                            else if (dk_tabtk == 2) {
                                if (self.ReportHH_TongHopNhapKho().length != 0) {
                                    for (var i = 0; i < self.ColumnsExcelTHHNKTQ().length; i++) {
                                        if (i == 0) {
                                            columnHide = self.ColumnsExcelTHHNKTQ()[i];
                                        }
                                        else {
                                            columnHide = self.ColumnsExcelTHHNKTQ()[i] + "_" + columnHide;
                                        }
                                    }
                                    for (var i = 0; i < self.ColumnsExcelTHHNKCT().length; i++) {
                                        if (i == 0) {
                                            columnHide1 = self.ColumnsExcelTHHNKCT()[i];
                                        }
                                        else {
                                            columnHide1 = self.ColumnsExcelTHHNKCT()[i] + "_" + columnHide1;
                                        }
                                    }
                                    var url = ReportUri + "ExportExcelHH_TongHopNhapKho?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&columnsHide=" + columnHide + "&columnsHide1=" + columnHide1 + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh() + "&LoaiChungTu=" + _idChungTuSeach + "&TinhTrang=" + TinhTrangHH;
                                    window.location.href = url;
                                }
                                else {
                                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Không có dữ liệu để xuất file excel!", "danger");
                                }
                            }
                            else {
                                if (self.ReportHH_TongHopXuatKho().length != 0) {
                                    for (var i = 0; i < self.ColumnsExcelTHHXKTQ().length; i++) {
                                        if (i == 0) {
                                            columnHide = self.ColumnsExcelTHHXKTQ()[i];
                                        }
                                        else {
                                            columnHide = self.ColumnsExcelTHHXKTQ()[i] + "_" + columnHide;
                                        }
                                    }
                                    for (var i = 0; i < self.ColumnsExcelTHHXKCT().length; i++) {
                                        if (i == 0) {
                                            columnHide1 = self.ColumnsExcelTHHXKCT()[i];
                                        }
                                        else {
                                            columnHide1 = self.ColumnsExcelTHHXKCT()[i] + "_" + columnHide1;
                                        }
                                    }
                                    var url = ReportUri + "ExportExcelHH_TongHopXuatKho?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&columnsHide=" + columnHide + "&columnsHide1=" + columnHide1 + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh() + "&LoaiChungTu=" + _idChungTuSeach + "&TinhTrang=" + TinhTrangHH;
                                    window.location.href = url;
                                }
                                else {
                                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Không có dữ liệu để xuất file excel!", "danger");
                                }

                            }
                        }
                        else
                            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                    }
                    else {
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Không có dữ liệu để xuất file excel!", "danger");
                    }
                },
                statusCode: {
                    404: function() {
                    },
                },
                error: function(jqXHR, textStatus, errorThrown) {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ghi nhật ký sử dụng thất bại!", "danger");
                },
                complete: function() {

                }
            })


        }
        self.ExportChiTietChuyenHang = function() {
            var objDiary = {
                ID_NhanVien: _id_NhanVien,
                ID_DonVi: _id_DonVi,
                ChucNang: "Báo cáo hàng hóa",
                NoiDung: "Xuất báo cáo chi tiết xuất hàng hóa theo chuyển hàng, Mã hàng: " + _maHangHoaCH,
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
                success: function(item) {

                },
                statusCode: {
                    404: function() {
                    },
                },
                error: function(jqXHR, textStatus, errorThrown) {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ghi nhật ký sử dụng thất bại!", "danger");
                },
                complete: function() {
                    var columnHide = null;
                    var url = ReportUri + "ExportExcelHH_XuatChuyenHangChiTiet?ID_DonViQuiDoi=" + _id_HangHoaChuyenHang + "&ID_ChiNhanh=" + _idDonViSeach + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&columnsHide=" + columnHide + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh() + "&ID_NguoiDung=" + _IDDoiTuong;
                    window.location.href = url;
                }
            })
        }
        self.ExportChiTietNhapChuyenHang = function() {
            var objDiary = {
                ID_NhanVien: _id_NhanVien,
                ID_DonVi: _id_DonVi,
                ChucNang: "Báo cáo hàng hóa",
                NoiDung: "Xuất báo cáo chi tiết nhập hàng hóa theo chuyển hàng, Mã hàng: " + _maHangHoaCH,
                NoiDungChiTiet: "Xuất báo cáo chi tiết nhập hàng hóa theo chuyển hàng, Mã hàng: " + _maHangHoaCH,
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
                success: function(item) {
                },
                statusCode: {
                    404: function() {
                    },
                },
                error: function(jqXHR, textStatus, errorThrown) {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ghi nhật ký sử dụng thất bại!", "danger");
                },
                complete: function() {
                    var columnHide = null;
                    var url = ReportUri + "ExportExcelHH_NhapChuyenHangChiTiet?ID_DonViQuiDoi=" + _id_HangHoaNhapChuyenHang + "&ID_ChiNhanh=" + _idDonViSeach + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&columnsHide=" + columnHide + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh() + "&ID_NguoiDung=" + _IDDoiTuong;
                    window.location.href = url;
                }
            })
        }
        //trinhpv load bieudo
        self.arrHHDS = ko.observableArray();
        self.arrDTDS = ko.observableArray();
        self.arrHHSL = ko.observableArray();
        self.arrDTSL = ko.observableArray();
        self.DoanhThuDS = ko.observableArray();
        self.DoanhThuSL = ko.observableArray();
        self.DonViTinhDS = ko.observable();
        self.DonViTinhSL = ko.observable();
        var _dataDS;
        var _data;
        var nameChar;
        var style;
        var style1;
        self.getListReportHH_BanHang_BieuDo = function() {
            ajaxHelper(ReportUri + "getListReportHH_BanHang_BieuDo?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function(data) {
                self.DoanhThuDS(data.Lst_BieuDo1);
                self.arrDTDS([]);
                self.arrHHDS([]);
                self.arrHHSL([]);
                self.arrDTSL([]);
                if (self.DoanhThuDS().length != 0) {
                    var _MauSoDVT = 1;
                    var _loadHangHoa = "'";
                    //var dvt = 0; //self.DoanhThuDS()[0].Rowsn;
                    //for (var i = 0; i < self.DoanhThuDS().length; i++) {
                    //    if (dvt < self.DoanhThuDS()[i].Rowsn) {
                    //        dvt = self.DoanhThuDS()[i].Rowsn;
                    //    }
                    //}
                    //if (dvt >= 1000000000) {
                    //    _MauSoDVT = 1000000000
                    //    self.DonViTinhDS("Đơn vị tính: hàng tỷ")
                    //}
                    //else if (dvt >= 1000000 & dvt < 1000000000) {
                    //    _MauSoDVT = 1000000
                    //    self.DonViTinhDS("Đơn vị tính: hàng triệu")
                    //}
                    //else if (dvt >= 1000 & dvt < 1000000) {
                    //    _MauSoDVT = 1000
                    //    self.DonViTinhDS("Đơn vị tính: hàng nghìn")
                    //}
                    //else {
                    //    self.DonViTinhDS("Đơn vị tính: hàng đơn vị");
                    //}

                    for (var i = 0; i < self.DoanhThuDS().length; i++) {
                        _loadHangHoa = self.DoanhThuDS()[i].Columnss;
                        // _data = parseFloat(self.DoanhThuDS()[i].Rowsn / _MauSoDVT).toFixed(3) * 1;
                        self.arrDTDS.push(self.DoanhThuDS()[i].Rowsn);
                        self.arrHHDS.push(_loadHangHoa);
                    }
                    style = 'Top 10 sản phẩm có doanh thu cao nhất (đã trừ trả hàng)';
                    nameChar = "Doanh thu thuần";
                    self.loadBieuDoDS();

                    self.DoanhThuSL(data.Lst_BieuDo2);

                    var _MauSoDVT = 1;
                    var _loadHangHoa = "'";
                    //var dvt = 0; //self.DoanhThuDS()[0].Rowsn;
                    //for (var i = 0; i < self.DoanhThuSL().length; i++) {
                    //    if (dvt < self.DoanhThuSL()[i].Rowsn) {
                    //        dvt = self.DoanhThuSL()[i].Rowsn;
                    //    }
                    //}
                    //if (dvt >= 1000000000) {
                    //    _MauSoDVT = 1000000000
                    //    self.DonViTinhSL("Đơn vị tính: hàng tỷ")
                    //}
                    //else if (dvt >= 1000000 & dvt < 1000000000) {
                    //    _MauSoDVT = 1000000
                    //    self.DonViTinhSL("Đơn vị tính: hàng triệu")
                    //}
                    //else if (dvt >= 1000 & dvt < 1000000) {
                    //    _MauSoDVT = 1000
                    //    self.DonViTinhSL("Đơn vị tính: hàng nghìn")
                    //}
                    //else {
                    //    self.DonViTinhSL("Đơn vị tính: hàng đơn vị");
                    //}
                    for (var i = 0; i < self.DoanhThuSL().length; i++) {
                        _loadHangHoa = self.DoanhThuSL()[i].Columnss;
                        //_data = parseFloat(self.DoanhThuSL()[i].Rowsn / _MauSoDVT).toFixed(3) * 1;
                        self.arrDTSL.push(self.DoanhThuSL()[i].Rowsn);
                        self.arrHHSL.push(_loadHangHoa);
                    }
                    style1 = 'Top 10 sản phẩm bán chạy theo số lượng (đã trừ trả hàng)';
                    nameChar = "Số lượng";
                    self.loadBieuDoSL();
                }
                else {
                    style = "Báo cáo không có dữ liệu.";
                    style1 = '';
                    self.DonViTinhDS([]);
                    self.DonViTinhSL([]);
                    self.loadBieuDoDS();
                    self.loadBieuDoSL();
                    console.log(self.DonViTinhSL());
                }
            });
        }
        self.getListReportHH_LoiNhuan_BieuDo = function() {
            console.log('a');
            ajaxHelper(ReportUri + "getListReportHH_LoiNhuan_BieuDo?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach + "&ID_NguoiDung=" + _IDDoiTuong, "GET").done(function (data) {
                self.DoanhThuDS(data.Lst_BieuDo1);
                self.arrDTDS([]);
                self.arrHHDS([]);
                self.arrHHSL([]);
                self.arrDTSL([]);
                if (self.DoanhThuDS().length != 0) {
                    var _MauSoDVT = 1;
                    var _loadHangHoa = "'";
                    //var dvt = 0; //self.DoanhThuDS()[0].Rowsn;
                    //for (var i = 0; i < self.DoanhThuDS().length; i++) {
                    //    if (dvt < self.DoanhThuDS()[i].Rowsn) {
                    //        dvt = self.DoanhThuDS()[i].Rowsn;
                    //    }
                    //}
                    //if (dvt >= 1000000000) {
                    //    _MauSoDVT = 1000000000
                    //    self.DonViTinhDS("Đơn vị tính: hàng tỷ")
                    //}
                    //else if (dvt >= 1000000 & dvt < 1000000000) {
                    //    _MauSoDVT = 1000000
                    //    self.DonViTinhDS("Đơn vị tính: hàng triệu")
                    //}
                    //else if (dvt >= 1000 & dvt < 1000000) {
                    //    _MauSoDVT = 1000
                    //    self.DonViTinhDS("Đơn vị tính: hàng nghìn")
                    //}
                    //else {
                    //    self.DonViTinhDS("Đơn vị tính: hàng đơn vị");
                    //}
                    for (var i = 0; i < self.DoanhThuDS().length; i++) {
                        _loadHangHoa = self.DoanhThuDS()[i].Columnss;
                        //_data = parseFloat(self.DoanhThuDS()[i].Rowsn / _MauSoDVT).toFixed(3) * 1;
                        self.arrDTDS.push(self.DoanhThuDS()[i].Rowsn);
                        self.arrHHDS.push(_loadHangHoa);
                    }
                    style = 'Top 10 hàng hóa có lợi nhuận cao nhất';
                    nameChar = "Lợi nhuận";
                    self.loadBieuDoDS();

                    self.DoanhThuSL(data.Lst_BieuDo2);
                    var _MauSoDVT = 1;
                    var _loadHangHoa = "'";
                    //self.DonViTinhSL("Đơn vị tính: phần trăm (%)");
                    for (var i = 0; i < self.DoanhThuSL().length; i++) {
                        _loadHangHoa = self.DoanhThuSL()[i].Columnss;
                        //_data = parseFloat(self.DoanhThuSL()[i].Rowsn / _MauSoDVT).toFixed(3) * 1;
                        self.arrDTSL.push(self.DoanhThuSL()[i].Rowsn);
                        self.arrHHSL.push(_loadHangHoa);
                    }
                    style1 = 'Top 10 hàng hóa có tỷ suất cao nhất';
                    //style1 = '';
                    nameChar = "Tỷ suất";
                    self.loadBieuDoSL();
                }
                else {
                    style = "Báo cáo không có dữ liệu.";
                    style1 = '';
                    self.DonViTinhDS([]);
                    self.DonViTinhSL([]);
                    self.loadBieuDoDS();
                    self.loadBieuDoSL();
                }
            });
        }

        self.loadBieuDoDS = function() {
            var viewPrint = true;
            if (self.arrDTDS().length > 0)
                viewPrint = true;
            else
                viewPrint = false;
            var chart = Highcharts.chart('chart', {
                chart: {
                    type: 'bar'
                },
                title: {
                    text: style
                },
                subtitle: {
                    text: ''
                },
                // đưa danh sách hàng hóa vào cột x
                xAxis: {
                    categories: self.arrHHDS()
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: 'Population (millions)',
                        align: 'high'
                    },
                    labels: {
                        overflow: 'justify'
                    },
                    plotLines: [{

                        width: 30,

                    }]
                },
                // hiển thị giá trị lên đầu cột
                plotOptions: {
                    bar: {
                        dataLabels: {
                            enabled: false
                        }
                    }
                },
                tooltip: {
                    footerFormat: "</table>",
                    shared: false,
                    useHTML: true,
                    formatter: function() {
                        return "<span style=\"font-size:10px\">" + this.key + "<br> </span><table>" + '<b style=\"color:{series.color};padding:0; text-align:Left; text-transform: capitalize; \">' + this.series.name + ': ' + Highcharts.numberFormat(this.y, 0, '.', ',') + '</b>';
                    }
                },
                colors: [
                    "#32b7b3",
                ],
                credits: {
                    enabled: false,
                },
                legend: {
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
                            //x: -62,    
                        }
                    }
                },
                // đưa giá trị tương ứng vào hàng hóa trong cột y
                series: [{
                    name: nameChar,
                    data: self.arrDTDS(),
                    maxPointWidth: 30
                }]
            });
        }

        self.loadBieuDoSL = function() {
            var viewPrint = true;
            if (self.arrDTSL().length > 0)
                viewPrint = true;
            else
                viewPrint = false;
            var chart = Highcharts.chart('panel-content1', {
                chart: {
                    type: 'bar'
                },
                title: {
                    text: style1
                },
                subtitle: {
                    text: ''
                },

                // đưa danh sách hàng hóa vào cột x
                xAxis: {
                    categories: self.arrHHSL()
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: 'Population (millions)',
                        align: 'high'
                    },
                    labels: {
                        overflow: 'justify',
                        formatter: function() {
                            if (this.value == 0)
                                return 0
                            else
                                if (nameChar == "Tỷ suất")
                                    return this.value + ' %';
                                else
                                    return this.value
                        }

                    },
                    plotLines: [{
                        width: 30,
                    }]
                },
                // hiển thị giá trị lên đầu cột
                plotOptions: {
                    bar: {
                        dataLabels: {
                            enabled: false
                        }
                    }
                },
                tooltip: {
                    footerFormat: "</table>",
                    shared: false,
                    useHTML: true,
                    formatter: function() {
                        if (nameChar == "Số lượng")
                            return "<span style=\"font-size:10px\">" + this.key + "<br> </span><table>" + '<b style=\"color:{series.color};padding:0; text-align:Left; text-transform: capitalize; \">' + this.series.name + ': ' + Highcharts.numberFormat(this.y, 1, '.', ',').replace('.0', '') + '</b>';
                        else
                            return "<span style=\"font-size:10px\">" + this.key + "<br> </span><table>" + '<b style=\"color:{series.color};padding:0; text-align:Left; text-transform: capitalize; \">' + this.series.name + ': ' + Highcharts.numberFormat(this.y, 2, '.', ',').replace('.00', '') + ' %</b>';
                    }
                },
                colors: [
                    "#32b7b3",
                ],
                credits: {
                    enabled: false,
                },
                legend: {
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
                            //x: -62,    
                        }
                    }
                },
                // đưa giá trị tương ứng vào hàng hóa trong cột y
                series: [{
                    name: nameChar,
                    data: self.arrDTSL(),
                    maxPointWidth: 30
                }]
            });
        }
    }
    ko.applyBindings(new ViewModel());

    function hidewait(o) {
        $('.' + o).append('<div id="wait"><img src="/Content/images/wait.gif" width="64" height="64" /></div>')
    }
});