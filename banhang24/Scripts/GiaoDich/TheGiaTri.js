var ViewModel = function () {
    var self = this;
    var BH_HoaDonUri = '/api/DanhMuc/BH_HoaDonAPI/';
    var DMDoiTuongUri = '/api/DanhMuc/DM_DoiTuongAPI/';
    var DMNhomDoiTuongUri = '/api/DanhMuc/DM_NhomDoiTuongAPI/';
    var DMNguonKhachUri = '/api/DanhMuc/DM_NguonKhachAPI/';
    var DMHangHoaUri = '/api/DanhMuc/DM_HangHoaAPI/';
    var Quy_HoaDonUri = '/api/DanhMuc/Quy_HoaDonAPI/';
    var CSKHUri = '/api/DanhMuc/ChamSocKhachHangAPI/';
    const loaiDoiTuong = 1;
    self.TodayBC = ko.observable('Toàn thời gian');

    var columnHide = '';
    self.NgayLapHD_Update = ko.observable();
    self.selectedKH = ko.observable();
    self.TT_HoanThanh = ko.observable(true);
    self.TT_DaHuy = ko.observable();
    self.filter = ko.observable();

    self.LaHD_NapThe = ko.observable(true);
    self.LaHD_HoanTraThe = ko.observable();

    self.MucNapTu = ko.observable();
    self.MucNapDen = ko.observable();
    self.KhuyenMaiTu = ko.observable();
    self.KhuyenMaiDen = ko.observable();
    self.ChietKhauTu = ko.observable();
    self.ChietKhauDen = ko.observable();

    self.MangNhomDV = ko.observableArray();
    self.MangIDDV = ko.observableArray();

    //nhân viên chiết khấu
    self.index_NVienBanGoi = ko.observable(0);
    self.ListNVien_BanGoi = ko.observableArray();
    self.GridNVienBanGoi_Chosed = ko.observableArray();

    // Thanh toan = The, NganHang
    self.LichSuThanhToan = ko.observableArray();
    self.GDVChosing = ko.observable();
    self.NhanViens_ChiNhanh = ko.observableArray(); // lstNhanvien with ID_DonVi (duplicate NVien)

    self.ListAccountPOS = ko.observableArray();
    self.ListAccountChuyenKhoan = ko.observableArray();
    self.ListTypeMauIn = ko.observableArray();

    var DiaryUri = '/api/DanhMuc/SaveDiary/';
    self.checkEmail = ko.observable(true);
    self.NhomDoiTuongs = ko.observableArray();
    self.TinhThanhs = ko.observableArray();
    self.QuanHuyens = ko.observableArray();
    self.NguonKhachs = ko.observableArray();
    self.NhanViens = ko.observableArray();
    self.CongTy = ko.observableArray();

    // the giatri
    self.TongTaiKhoanThe = ko.observable(0);
    self.SoDuTheGiaTri = ko.observable(0);
    self.SuDungThe = ko.observable(0);
    self.HoanTraTheGiaTri = ko.observable(0);

    self.filter = ko.observable();
    self.filterNgayTao = ko.observable("0");
    self.filterNgayTao_Quy = ko.observable("0");
    self.filterNgayTao_Input = ko.observable();

    self.filterTongBan = ko.observable("0");
    self.filterDateTongBan_Quy = ko.observable("0");
    self.filterDateTongBan_Input = ko.observable();

    self.pageSizes = [10, 20, 30, 40, 50];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.currentPage = ko.observable(0);
    self.fromitem = ko.observable(1);
    self.toitem = ko.observable();
    self.TotalRecord = ko.observable();
    self.PageCount = ko.observable();
    self.filterNgayLapHD = ko.observable("0");
    self.filterNgayLapHD_Input = ko.observable(); // ngày cụ thể
    self.filterNgayLapHD_Quy = ko.observable(0); // Theo quý

    self.ContinueImport = ko.observable(false);
    self.GhiChuQuyHD = ko.observable();
    self.NgayDieuChinh = ko.observable(moment(today).format('DD/MM/YYYY'));
    self.TongThuDieuChinh = ko.observable();
    self.NguoiNopTien = ko.observable();
    self.ID_NguoiNopTien = ko.observable();
    self.ListAllDoiTuong = ko.observableArray();
    self.TrangThaiKhachHang = ko.observableArray();

    // Thanh toan No
    self.ListHDisDebit = ko.observableArray();
    self.InforHDprintf = ko.observableArray();
    self.ItemHoaDon = ko.observableArray();
    self.ChotSo_ChiNhanh = ko.observableArray();

    // chon nhieu ChiNhanh 
    self.ChiNhanhs = ko.observableArray();
    self.ChiNhanhChosed = ko.observableArray();
    self.selectedChiNhanh = ko.observable();

    self.Allow_ChangeTimeSoQuy = ko.observable(false);
    self.Show_BtnThanhToanCongNo = ko.observable(false);

    // chon nhieu nhom
    self.NhomDoiTuongDB = ko.observableArray();
    self.NhomDoiTuongChosed = ko.observableArray();

    self.ListCheckBox = ko.observableArray();
    self.NumberColum_Div2 = ko.observable();

    //sort
    self.columsort = ko.observable(null);
    self.sort = ko.observable(0);
    self.ListIDNhanVienQuyen = ko.observableArray();

    //footer
    self.TongMucNapAll = ko.observable(0);
    self.TongKhuyenMaiAll = ko.observable(0);
    self.TongTienNapAll = ko.observable(0);
    self.TongChietKhauAll = ko.observable(0);
    self.SoDuSauNapAll = ko.observable(0);
    self.PhaiThanhToanAll = ko.observable(0);
    self.TienMatAll = ko.observable(0);
    self.TienATMAll = ko.observable(0);
    self.TienGuiAll = ko.observable(0);
    self.KhachDaTraAll = ko.observable(0);
    self.error = ko.observable();
    var userID = '';
    var idNhanVien = '';
    var idDonVi = '';
    var sLoai = 'khách hàng';
    var Key_Form = "ENapTien";
    var today = new Date();

    if (VHeader) {
        userID = VHeader.IdNguoiDung;
        userLogin = VHeader.UserLogin;
        idNhanVien = VHeader.IdNhanVien;
        idDonVi = VHeader.IdDonVi;

        vmThanhToan.inforLogin = {
            ID_DonVi: VHeader.IdDonVi,
            ID_NhanVien: VHeader.IdNhanVien,
            UserLogin: VHeader.UserLogin,
            TenNhanVien: VHeader.TenNhanVien,
            ID_User: VHeader.IdNguoiDung
        }
        vmThemMoiKhach.inforLogin = vmThanhToan.inforLogin;
    }

    function Page_Load() {
        LoadColumnCheck();
        getListQuanHuyen();
        GetDM_NguonKHang();
        getListNhanVien();
        getListNhanVienCK();
        GetAllChiNhanh();
        GetListIDNhanVien_byUserLogin();
        LoadTrangThai();
        loadMauIn();
        GetDM_TaiKhoanNganHang();
        GetAllQuy_KhoanThuChi();
        GetInforCongTy();
        GetCauHinhHeThong();
    }
    Page_Load();

    function GetCauHinhHeThong() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/' + "GetCauHinhHeThong/" + VHeader.IdDonVi, 'GET').done(function (data) {
                if (data !== null) {
                    vmThemMoiKhach.QuanLyKhachHangTheoDonVi = data.QuanLyKhachHangTheoDonVi;
                }
                getListNhomDT();
            });
        }
    }

    function LoadColumnCheck() {
        ajaxHelper('/api/DanhMuc/BaseAPI/' + "GetListColumnInvoices?loaiHD=22", 'GET').done(function (data) {
            self.ListCheckBox(data);
            self.NumberColum_Div2(Math.ceil(data.length / 2));
            LoadHtmlGrid();
        });
    }

    // hide/show column from checkbox
    $('#myList').on('change', 'ul li input[type = checkbox]', function () {
        var valueCheck = $(this).val();
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
        LocalCaches.AddColumnHidenGrid('ENapTien', valueCheck, valueCheck);
        $('.' + valueCheck).toggle();
    });

    function GetListIDNhanVien_byUserLogin() {
        ajaxHelper(CSKHUri + 'GetListNhanVienLienQuanByIDLoGin_inDepartment?idnvlogin=' + idNhanVien
            + '&idChiNhanh=' + idDonVi, 'GET').done(function (data) {
                self.ListIDNhanVienQuyen(data);

                GetHT_Quyen_ByNguoiDung();
            });
    }

    function getListTinhThanh() {
        ajaxHelper(DMDoiTuongUri + "GetListTinhThanh", 'GET').done(function (x) {
            if (x.res === true) {
                //self.TinhThanhs(x.data);

            }
        });
    }

    function getListQuanHuyen(id) {
        if (id !== undefined) {
            ajaxHelper(DMDoiTuongUri + "GetListQuanHuyen?idTinhThanh=" + id, 'GET').done(function (x) {
                if (x.data !== null) {
                    self.QuanHuyens(x.data);
                }
            });
        }
    }

    function getListNhomDT() {
        ajaxHelper(DMDoiTuongUri + "GetNhomDoiTuong_DonVi?loaiDT=1", 'GET').done(function (obj) {
            if (obj.res === true) {
                let data = obj.data;
                for (var i = 0; i < data.length; i++) {
                    let tenNhom = data[i].TenNhomDoiTuong;
                    tenNhom = tenNhom.concat(' ', locdau(tenNhom), ' ', GetChartStart(tenNhom));
                    data[i].Text_Search = tenNhom;
                }
                if (vmThemMoiKhach.QuanLyKhachHangTheoDonVi) {
                    // only get Nhom chua cai dat ChiNhanh or in this ChiNhanh
                    var arrNhom = [];
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].NhomDT_DonVi.length > 0) {
                            let ex = $.grep(data[i].NhomDT_DonVi, function (x) {
                                return x.ID === idDonVi;
                            });
                            if (ex.length) {
                                arrNhom.push(data[i]);
                            }
                        }
                        else {
                            arrNhom.push(data[i]);
                        }
                    }
                    self.NhomDoiTuongs(arrNhom);
                }
                else {
                    self.NhomDoiTuongs(data);
                }
                vmThemMoiKhach.listData.NhomKhachs = self.NhomDoiTuongs();
            }
        });
    }

    function LoadTrangThai() {
        ajaxHelper(CSKHUri + 'GetTrangThaiTimKiem', 'GET').done(function (data) {
            if (data.res === true) {
                var lst = data.dataSoure.ttkhachhang;
                vmThemMoiKhach.listData.TrangThaiKhachs = lst;
            }
        });
    };

    self.showModalUpdateTGT = function (item) {
        vmThanhToanGara.listData.ChietKhauHoaDons = vmHoaHongHoaDon.listData.ChietKhauHoaDons;
        vmThemMoiTheNap.showModalUpdate(item.ID);
    };

    self.showpopupNapTien = function () {
        vmThanhToanGara.listData.ChietKhauHoaDons = vmHoaHongHoaDon.listData.ChietKhauHoaDons;
        vmThemMoiTheNap.showModalAddNew();
    };

    self.showPopHoanTra = function () {
        vmTraLaiTGT.showModalAddNew();
    }

    self.showPopupAddKH = function () {
        vmThemMoiKhach.showModalAdd();
    };


    self.HoaDons = ko.observableArray();

    function Enable_BtnSave() {
        $('#btnSave').removeAttr('disabled');
        document.getElementById("btnSave").lastChild.data = "Lưu";
    }

    function shareMoney_QuyHD(phaiTT, tienmat, tienPOS, chuyenkhoan) {

        if (tienPOS >= phaiTT) {
            return {
                TienMat: 0,
                TienPOS: Math.abs(phaiTT),
                TienChuyenKhoan: 0,
            }
        }
        else {
            phaiTT = phaiTT - tienPOS;

            if (chuyenkhoan >= phaiTT) {
                return {
                    TienMat: 0,
                    TienPOS: tienPOS,
                    TienChuyenKhoan: Math.abs(phaiTT),
                }
            }
            else {
                phaiTT = phaiTT - chuyenkhoan;

                if (tienmat >= phaiTT) {

                    return {
                        TienMat: Math.abs(phaiTT),
                        TienPOS: tienPOS,
                        TienChuyenKhoan: chuyenkhoan,
                    }
                }
                else {
                    phaiTT = phaiTT - tienmat;
                    return {
                        TienMat: tienmat,
                        TienPOS: tienPOS,
                        TienChuyenKhoan: chuyenkhoan,
                    }
                }
            }
        }
    }

    //Thêm mới thẻ nạp
    self.NapTienThe = function () {
        $('#btnSave').attr('disabled', 'disabled');
        document.getElementById("btnSave").lastChild.data = "Đang lưu";

        var _maHoaDon = self.newHoaDon().MaHoaDon();
        var _iddoituong = self.selectedKH();
        var _mucnap = self.newHoaDon().TongChiPhi();
        var _khuyenmai = $('#txtKhuyenMai_TheNapTien').val();
        var _tongtiennap = $('#txtTongTienHang_TheNapTien').html();
        var _sodusaunap = $('#txtSoDuSauNap').html(); // TongTienThue
        var _tonggiamgia = $('#txtTienGiam_TheNapTien').val();
        var _phaithanhtoan = $('#txtPhaiThanhToan_TheNapTien').html();
        var _tienkhachdua = $('#txtTienKhachTra_TheNapTien').val();
        var _ngaynap = $('#data_ngaynap2').val();
        var _ngayLapHoaDon = moment(_ngaynap, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');

        if (_iddoituong === undefined) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn chưa chọn khách cần nạp thẻ", "danger");
            Enable_BtnSave();
            return false;
        }

        if (_mucnap === 0) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Tiền nạp phải lớn hơn 0", "danger");
            $('#txtMucNap_TheNapTien').select();
            Enable_BtnSave();
            return false;
        }

        //if (_tienkhachdua < _phaithanhtoan) {
        //    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn cần thanh toán đủ số tiền nạp", "danger");
        //    $('#txtTienKhachTra_TheNapTien').select();
        //    return false;
        //}
        var myData = {};
        var HoaDon = {
            MaHoaDon: _maHoaDon,
            ID_DoiTuong: _iddoituong,
            ID_NhanVien: idNhanVien,
            ID_DonVi: idDonVi,
            NgayLapHoaDon: _ngayLapHoaDon,
            TongChiPhi: _mucnap,
            TongChietKhau: _khuyenmai,
            TongTienHang: _tongtiennap,
            TongTienThue: _sodusaunap,
            TongGiamGia: _tonggiamgia,
            PhaiThanhToan: _phaithanhtoan,
            DienGiai: self.newHoaDon().DienGiai(),
            ChoThanhToan: false,
            LoaiHoaDon: 22,
            NguoiTao: $('#txtTenTaiKhoan').text().trim()
        };

        // nhuongdt - set default TienMat = tienkhachdua
        if (formatNumberToFloat(self.GiaTriTienMat()) === 0 && formatNumberToFloat(self.GiaTriTienNH()) === 0 && formatNumberToFloat(self.GiaTriTienChuyenKhoan()) === 0) {
            self.GiaTriTienMat(_tienkhachdua);
        }

        let mat = formatNumberToFloat(self.GiaTriTienMat());
        let pos = formatNumberToFloat(self.GiaTriTienNH());
        let ck = formatNumberToFloat(self.GiaTriTienChuyenKhoan());
        let objQuy = shareMoney_QuyHD(formatNumberToFloat(_phaithanhtoan), mat, pos, ck);
        mat = objQuy.TienMat;
        pos = objQuy.TienPOS;
        ck = objQuy.TienChuyenKhoan;
        if (pos === 0) {
            self.selectIDTaiKhoanPOS(undefined);
        }
        if (ck === 0) {
            self.selectIDTaiKhoanChuyenK(undefined);
        }

        myData.objHoaDon = HoaDon;
        myData.idnhanvien = idNhanVien;
        myData.GiaTriTienMat = mat;
        myData.GiaTriTienNH = pos;
        myData.GiaTriTienChuyenKhoan = ck;
        myData.IDTKPOS = self.selectIDTaiKhoanPOS();
        myData.IDTKChuyenKhoan = self.selectIDTaiKhoanChuyenK();
        ajaxHelper(BH_HoaDonUri + "Check_MaHoaDonExist?maHoaDon=" + _maHoaDon, 'POST').done(function (data) {
            if (data) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Mã hóa đơn đã tồn tại", "danger");
                $('#txtMaHoaDonNapThe').focus();
                Enable_BtnSave();
                return false;
            }
            else {
                $.ajax({
                    data: myData,
                    url: BH_HoaDonUri + "PostBH_HoaDonNapThe",
                    type: 'POST',
                    async: true,
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",

                    success: function (item) {
                        SearchTheNap();
                        if (self.GridNVienBanGoi_Chosed().length > 0) {
                            var myDataNV = {};
                            myDataNV.objCT = self.GridNVienBanGoi_Chosed();
                            myDataNV.idthegiatri = item.ID;
                            myDataNV.idquyhoadon = item.ID_PhieuChi;
                            $.ajax({
                                data: myDataNV,
                                url: BH_HoaDonUri + "PostNhanVien_ThucHien",
                                type: "POST",
                                async: true,
                                dataType: 'json',
                                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                                success: function (url) {
                                }
                            });
                        }
                    },
                    statusCode: {
                        404: function () {
                            self.error("page not found");
                        },
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                    },
                    complete: function (item) {
                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + 'Nạp thẻ thành công', 'success');
                        $('#modalNapTheGiaTri').modal('hide');
                        Enable_BtnSave();
                    }
                })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                    });
            }
        });


        var Quy_HoaDon = {
            ID_DoiTuong: _iddoituong,
            LoaiHoaDon: 11,
            TongTienThu: self.GiaTriTienMat() + self.GiaTriTienNH() + self.GiaTriTienChuyenKhoan()
        }

        if (Quy_HoaDon.ID_DoiTuong !== null) {
            UpdateNhom_KhachHang(Quy_HoaDon);
        }
    };

    self.linkphieu = function (item) {
        localStorage.setItem('FindMaPhieuChi', item.MaPhieuThu);
        window.open('/#/CashFlow', '_blank');
    };
    self.gotoKhachHang = function (item) {
        localStorage.setItem('FindKhachHang', item.MaKhachHang);
        window.open('/#/Customers', '_blank');
    };

    //nhân viên chiết khấu
    self.index_NVienBanGoi = ko.observable(0);
    self.ListNVien_BanGoi = ko.observableArray();
    self.GridNVienBanGoi_Chosed = ko.observableArray();


    function Get_SoDuTheGiaTri_ofKhachHang(idDoiTuong, datetime, isChoseKH) {
        ajaxHelper(DMDoiTuongUri + 'Get_SoDuTheGiaTri_ofKhachHang?idDoiTuong=' + idDoiTuong + '&datetime=' + datetime, 'GET').done(function (data) {
            if (data != null && data.length > 0) {
                // used to get when print
                self.TongTaiKhoanThe(data[0].TongThuTheGiaTri);
                self.SoDuTheGiaTri(data[0].SoDuTheGiaTri);
                self.SuDungThe(data[0].SuDungThe);
                self.HoanTraTheGiaTri(data[0].HoanTraTheGiaTri);// tien su dung the đến thời điểm hiện tại

                if (isChoseKH) {
                    $('#txtSoDuHienTai').html(formatNumber(data[0].SoDuTheGiaTri));
                    $('#btnModalLichSu').show();
                    tiennap();
                    SearchLSNapTien();
                }
            }
            else {
                if (isChoseKH) {
                    $('#txtSoDuHienTai').html(0);
                    $('#btnModalLichSu').show();
                    tiennap();
                    SearchLSNapTien();
                }
            }
        });
    }

    function GetDM_NguonKHang() {
        if (navigator.onLine) {
            ajaxHelper(DMNguonKhachUri + 'GetDM_NguonKhach', 'GET').done(function (data) {
                vmThemMoiKhach.listData.NguonKhachs = data;
            });
        }
    }

    function getListNhanVien() {
        // get all NhanVien all ChiNhanh --> because, share many ChiNhanh
        ajaxHelper("/api/DanhMuc/NS_NhanVienAPI/" + 'GetNhanVien_NguoiDung', 'GET').done(function (x) {
            if (x.res === true) {
                let data = x.data;
                var lstNV_byDonVi = $.grep(data, function (x) {
                    return x.ID_DonVi === idDonVi;
                });
                self.NhanViens(lstNV_byDonVi);

                vmHoaHongHoaDon.listData.NhanViens = lstNV_byDonVi;
                vmThanhToanGara.listData.NhanViens = lstNV_byDonVi;
                vmThanhToan.listData.NhanViens = lstNV_byDonVi;
                vmThemMoiKhach.listData.NhanViens = lstNV_byDonVi;
            }
        });
    }

    //xuất file thẻ nạp
    self.ExportTheNap = function () {
        columnHide = '';
        var columns = localStorage.getItem(Key_Form);
        if (columns !== null) {
            columns = JSON.parse(columns);
            for (let i = 0; i < self.ListCheckBox().length; i++) {
                for (let j = 0; j < columns.length; j++) {
                    if (columns[j].Value === self.ListCheckBox()[i].Key) {
                        columnHide += i + '_';
                        break;
                    }
                }
            }
        }
        SearchTheNap(true);
    };

    self.DownloadFileExportXLSX = function (url) {
        var url1 = DMHangHoaUri + "Download_fileExcel?fileSave=" + url;
        window.location.href = url1;
    }

    self.selectedCN = function (item) {
        $("#iconSort").remove();
        var arrDV = [];
        for (var i = 0; i < self.MangNhomDV().length; i++) {
            if ($.inArray(self.MangNhomDV()[i], arrDV) === -1) {
                arrDV.push(self.MangNhomDV()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrDV) === -1) {
            self.MangNhomDV.push(item);
        };
        SearchTheNap();
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
        SearchTheNap();
        // remove checks
        $('#selec-all-DV li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
    }

    self.GetListTheNap = function () {
        SearchTheNap();
    }

    function SearchTheNap(isExport = false) {
        var maHDFind = localStorage.getItem('FindHD');
        if (maHDFind !== null) {
            self.filter(maHDFind);
            self.filterNgayLapHD('0');
            self.filterNgayLapHD_Quy(0);
        }
        var arrDV = [];
        LoadHtmlGrid();

        for (var i = 0; i < self.MangNhomDV().length; i++) {
            if ($.inArray(self.MangNhomDV()[i], arrDV) === -1) {
                arrDV.push(self.MangNhomDV()[i].ID);
            }
        }

        self.MangIDDV(arrDV);

        // avoid error in Store procedure
        if (self.MangIDDV().length === 0) {
            self.MangIDDV([idDonVi]);
        }

        var loaikhuyenmai = 0; // khuyến mại theo %
        var loaichietkhau = 0; // chiết khấu theo %
        if ($('.loaikhuyenmai').hasClass('active-ck')) {
            loaikhuyenmai = 1; // khuyến mại theo VNĐ
        }
        if ($('.loaichietkhau').hasClass('active-ck')) {
            loaichietkhau = 1; // chiết khấu theo VNĐ
        }
        var MucNapTu = $('.currencytu').val();
        var MucNapDen = $('.currencyden').val();
        var txtMaHDon = self.filter();

        var statusInvoice = 1;
        if (self.TT_DaHuy()) {
            if (self.TT_HoanThanh()) {
                statusInvoice = 3; // HT + DH
            }
            else {
                statusInvoice = 2; //DH
            }
        }
        else {
            if (self.TT_HoanThanh()) {
                statusInvoice = 1; // HT
            }
            else {
                statusInvoice = 4;
            }
        }

        let arrLoaiHD = [];
        if (self.LaHD_NapThe()) {
            arrLoaiHD.push(22);
        }
        if (self.LaHD_HoanTraThe()) {
            arrLoaiHD.push(32);
        }

        if (arrLoaiHD.length === 0) {
            arrLoaiHD = [22, 32];
        }

        // NgayLapHoaDon
        var _now = new Date();  //current date of week
        var currentWeekDay = _now.getDay();
        var lessDays = currentWeekDay === 0 ? 6 : currentWeekDay - 1;
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
                    dayStart = moment(new Date(_now.setDate(_now.getDate() - 1))).format('YYYY-MM-DD');
                    break;
                case 3:
                    // tuan nay
                    self.TodayBC('Tuần này');
                    dayStart = moment(new Date(_now.setDate(_now.getDate() - lessDays - 1))).format('YYYY-MM-DD'); // start of wwek
                    dayEnd = moment(new Date(_now.setDate(_now.getDate() + 6))).add('days', 1).format('YYYY-MM-DD'); // end of week
                    break;
                case 4:
                    // tuan truoc
                    self.TodayBC('Tuần trước');
                    dayStart = moment().weekday(-6).format('YYYY-MM-DD');
                    dayEnd = moment(dayStart, 'YYYY-MM-DD').add(6, 'days').add('days', 1).format('YYYY-MM-DD'); // add day in moment.js
                    break;
                case 5:
                    // 7 ngay qua
                    self.TodayBC('7 ngày qua');
                    dayEnd = moment(_now).add('days', 1).format('YYYY-MM-DD');
                    dayStart = moment(new Date(_now.setDate(_now.getDate() - 7))).format('YYYY-MM-DD');
                    break;
                case 6:
                    // thang nay
                    self.TodayBC('Tháng này');
                    dayStart = moment(new Date(_now.getFullYear(), _now.getMonth(), 1)).format('YYYY-MM-DD');
                    dayEnd = moment(new Date(_now.getFullYear(), _now.getMonth() + 1, 0)).add('days', 1).format('YYYY-MM-DD');
                    break;
                case 7:
                    // thang truoc

                    self.TodayBC('Tháng trước');
                    dayStart = moment(new Date(_now.getFullYear(), _now.getMonth() - 1, 1)).format('YYYY-MM-DD');
                    dayEnd = moment(new Date(_now.getFullYear(), _now.getMonth(), 0)).add('days', 1).format('YYYY-MM-DD');
                    break;
                case 10:
                    // quy nay
                    self.TodayBC('Quý này');
                    dayStart = moment().startOf('quarter').format('YYYY-MM-DD');
                    dayEnd = moment().endOf('quarter').add('days', 1).format('YYYY-MM-DD');
                    break;
                case 11:
                    self.TodayBC('Quý trước');
                    var prevQuarter = moment().quarter() - 1;
                    if (prevQuarter === 0) {
                        // get quy 4 cua nam truoc 01/10/... -->  31/21/...
                        let prevYear = moment().year() - 1;
                        dayStart = prevYear + '-' + '10-01';
                        dayEnd = moment().year() + '-' + '01-01';
                    }
                    else {
                        dayStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                        dayEnd = moment().quarter(prevQuarter).endOf('quarter').add(1, 'days').format('YYYY-MM-DD');
                    }
                    break;
                case 12:
                    // nam nay
                    self.TodayBC('Năm nay');
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
            // chon ngay cu the
            var arrDate = self.filterNgayLapHD_Input().split('-');
            dayStart = moment(arrDate[0], 'DD/MM/YYYY').format('YYYY-MM-DD');
            dayEnd = moment(arrDate[1], 'DD/MM/YYYY').add(1, 'days').format('YYYY-MM-DD');
            self.TodayBC('Từ ' + moment(arrDate[0], 'DD/MM/YYYY').format('DD/MM/YYYY') + ' đến ' + moment(arrDate[1], 'DD/MM/YYYY').format('DD/MM/YYYY'));
        }

        var model = {
            currentPage: self.currentPage(),
            pageSize: self.pageSize(),
            loaiHoaDon: 22,
            maHoaDon: txtMaHDon,
            mucnaptu: MucNapTu === '' ? 0 : MucNapTu,
            mucnapden: MucNapDen === '' ? null : MucNapDen,
            loaikhuyenmai: loaikhuyenmai,
            khuyenmaitu: 0,
            khuyenmaiden: null,
            loaichietkhau: loaichietkhau,
            chietkhautu: 0,
            chietkhauden: null,
            trangThai: statusInvoice,
            dayStart: dayStart,
            dayEnd: dayEnd,
            arrChiNhanh: self.MangIDDV(),
            iddonvi: idDonVi,
            columnsHide: columnHide,
            ArrLoaiHoaDon: arrLoaiHD,
        };

        console.log('model ', model)
        var url = 'GetListTheNap';
        if (isExport) {
            model.pageSize = 1000;
            url = 'XuatFileThenNap';
        }
        $('.content-table').gridLoader();
        ajaxHelper(BH_HoaDonUri + url, 'POST', model).done(function (obj) {
            $('.content-table').gridLoader({ show: false });
            if (isExport) {
                self.DownloadFileExportXLSX(obj);

                var objDiary = {
                    ID_NhanVien: idNhanVien,
                    ID_DonVi: idDonVi,
                    ChucNang: "Thẻ nạp",
                    NoiDung: "Xuất danh sách hóa đơn thẻ nạp",
                    NoiDungChiTiet: "Xuất danh sách hóa đơn thẻ nạp",
                    LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
                };
                Insert_NhatKyThaoTac_1Param(objDiary);
            }
            else {
                localStorage.removeItem('FindHD');
                if (obj.res === true && obj.lst.length > 0) {
                    self.HoaDons(obj.lst);

                    let itFirst = obj.lst[0];
                    self.TotalRecord(itFirst.TotalRow);
                    self.PageCount(itFirst.TotalPage);
                    self.TongMucNapAll(itFirst.TongMucNapAll);
                    self.TongKhuyenMaiAll(itFirst.TongKhuyenMaiAll);
                    self.TongTienNapAll(itFirst.TongTienNapAll);
                    self.TongChietKhauAll(itFirst.TongChietKhauAll);
                    self.SoDuSauNapAll(itFirst.SoDuSauNapAll);
                    self.PhaiThanhToanAll(itFirst.PhaiThanhToanAll);
                    self.TienMatAll(itFirst.TienMatAll);
                    self.TienATMAll(itFirst.TienATMAll);
                    self.TienGuiAll(itFirst.TienGuiAll);
                    self.KhachDaTraAll(itFirst.KhachDaTraAll);

                    LoadHtmlGrid();
                }
                else {
                    self.HoaDons([]);
                    self.TotalRecord(0);
                    self.PageCount(0);
                    self.TongMucNapAll(0);
                    self.TongKhuyenMaiAll(0);
                    self.TongTienNapAll(0);
                    self.TongChietKhauAll(0);
                    self.SoDuSauNapAll(0);
                    self.PhaiThanhToanAll(0);
                    self.TienMatAll(0);
                    self.TienATMAll(0);
                    self.TienGuiAll(0);
                    self.KhachDaTraAll(0);
                }
            }
        });
    }
    SearchTheNap();

    self.filterNgayLapHD.subscribe(function (newVal) {
        self.currentPage(0);
        SearchTheNap();
    });

    $('#txtNgayTaoInput').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        SearchTheNap();
    });

    $('.choseNgayTao li').on('click', function () {
        $('#txtNgayTao').val($(this).text());
        self.filterNgayLapHD_Quy($(this).val());
        self.currentPage(0);
        SearchTheNap();
    });

    $('#txtMaHoaDonNT').keypress(function (e) {
        if (e.keyCode === 13) {
            self.currentPage(0);
            SearchTheNap();
        }
    });

    self.clickiconSearch = function () {
        SearchTheNap();
    };

    $('.currencytu').keypress(function (e) {
        if (e.keyCode === 13) {
            self.currentPage(0);
            SearchTheNap();
        }
    });

    $('.currencyden').keypress(function (e) {
        if (e.keyCode === 13) {
            self.currentPage(0);
            SearchTheNap();
        }
    });

    $('.currencytu1').keypress(function (e) {
        if (e.keyCode === 13) {
            self.currentPage(0);
            SearchTheNap();
        }
    });

    $('.currencyden1').keypress(function (e) {
        if (e.keyCode === 13) {
            self.currentPage(0);
            SearchTheNap();
        }
    });

    $('.currencytu2').keypress(function (e) {
        if (e.keyCode === 13) {
            self.currentPage(0);
            SearchTheNap();
        }
    });

    $('.currencyden2').keypress(function (e) {
        if (e.keyCode === 13) {
            self.currentPage(0);
            SearchTheNap();
        }
    });

    self.ResetCurrentPage = function () {

        self.currentPage(0);
        //GetPageCountHoaDon();
        SearchTheNap();
    };

    self.TT_HoanThanh.subscribe(function (newVal) {
        self.currentPage(0);
        SearchTheNap();
    });
    self.LaHD_NapThe.subscribe(function (newVal) {
        self.currentPage(0);
        SearchTheNap();
    });
    self.LaHD_HoanTraThe.subscribe(function (newVal) {
        self.currentPage(0);
        SearchTheNap();
    });

    self.TT_DaHuy.subscribe(function (newVal) {
        self.currentPage(0);
        SearchTheNap();
    });

    self.PageResults = ko.computed(function () {
        if (self.HoaDons() !== null) {

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
        }
    });

    self.PageList_Display = ko.computed(function () {
        var arrPage = [];

        var allPage = self.PageCount();
        var currentPage = self.currentPage();

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
            SearchTheNap();
        }
    };

    self.StartPage = function () {
        self.currentPage(0);
        SearchTheNap();
    };

    self.BackPage = function () {
        if (self.currentPage() > 1) {
            self.currentPage(self.currentPage() - 1);
            SearchTheNap();
        }
    };

    self.GoToNextPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.currentPage() + 1);
            SearchTheNap();
        }
    };

    self.EndPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.PageCount() - 1);
            SearchTheNap();
        }
    };

    self.GetClassHD = function (page) {
        return ((page.pageNumber - 1) === self.currentPage()) ? "click" : "";
    };

    self.editTheNap = function (formElement) {
        if (self.NgayLapHD_Update() === undefined) {
            self.NgayLapHD_Update(moment(formElement.NgayLapHoaDon).format('DD/MM/YYYY HH:mm'));
        }

        var check = CheckNgayLapHD_format(self.NgayLapHD_Update(), formElement.ID_DonVi);

        if (!check) {
            return;
        }
        var _id = formElement.ID;
        var ghichu = $('#txtGhiChuUD' + formElement.ID).val();
        var _ngayLapHoaDon = moment(self.NgayLapHD_Update(), 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm:ss');
        ajaxHelper(BH_HoaDonUri + 'UpdateThoiGianPhieuNap?id=' + _id + '&time=' + _ngayLapHoaDon + '&ghichu=' + ghichu, 'GET').done(function (data) {
            if (data === "") {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật hóa đơn nạp thẻ thành công", "success");
                SearchTheNap();
            }
        });
    };

    self.selectedNV = ko.observable(idNhanVien);// NVien lap phieuthu
    self.NoHienTai = ko.observable();

    self.showpopupThanhtoan = function (item) {
        ajaxHelper(BH_HoaDonUri + 'GetChietKhauNV_HoaDon?idHoaDon=' + item.ID, 'GET').done(function (obj) {
            if (obj.res === true) {
                item.BH_NhanVienThucHiens = obj.data;
                item.TongThanhToan = item.PhaiThanhToan;
                item.PhaiThanToanBaoHiem = 0;
                item.BaoHiemDaTra = 0;
                item.TongTienThue = 0;
                item.MaDoiTuong = item.MaKhachHang;
                item.TenDoiTuong = item.TenKhachHang;
                item.DienThoai = item.SoDienThoai;
                item.LoaiHoaDon = 22;
                vmThanhToan.showModalThanhToan(item);
            }
        })
    }

    self.KhoanThuChis = ko.observableArray();
    self.KhoanChis = ko.observableArray();
    self.selectID_KhoanThu = ko.observableArray();

    function GetDM_TaiKhoanNganHang() {
        if (navigator.onLine) {
            ajaxHelper(Quy_HoaDonUri + 'GetAllTaiKhoanNganHang_ByDonVi?idDonVi=' + idDonVi, 'GET').done(function (x) {
                if (x.res === true) {
                    let data = x.data;
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].TaiKhoanPOS === true) {
                            self.ListAccountPOS.push(data[i]);
                        }
                        else {
                            self.ListAccountChuyenKhoan.push(data[i]);
                        }
                    }
                    vmThanhToanGara.listData.AccountBanks = data; // use at form new TheGiaTri
                    vmThanhToan.listData.AccountBanks = data;// use at btnThanhToan
                }
            })
        }
    }

    function GetAllQuy_KhoanThuChi() {
        ajaxHelper(Quy_HoaDonUri + 'GetQuy_KhoanThuChi', 'GET').done(function (x) {
            if (x.res === true) {
                let data = x.data;
                var khoanthu = $.grep(data, function (x) {
                    return x.LaKhoanThu === true;
                });
                self.KhoanThuChis(khoanthu);

                var khoanchi = $.grep(data, function (x) {
                    return x.LaKhoanThu === false;
                });
                self.KhoanChis(khoanchi);
                vmThanhToan.listData.KhoanThuChis = khoanthu;
                vmThanhToanGara.listData.KhoanThuChis = data;
            }
        })
    }

    function getListNhanVienCK() {
        ajaxHelper("/api/DanhMuc/NS_NhanVienAPI/" + "GetNS_NhanVien_InforBasic?idDonVi=" + idDonVi, 'GET').done(function (data) {
            self.NhanViens(data);
        });
    }


    self.HuyBoThe = function (item) {
        var idThe = item.ID;
        ajaxHelper(BH_HoaDonUri + 'CheckTheDaSuDung?id=' + idThe, 'GET').done(function (data) {
            if (data === "") {
                dialogConfirm('Thông báo hủy', 'Bạn có chắc chắn muốn xóa thẻ <b>' + item.MaHoaDon + '</b> không?', function () {
                    ajaxHelper(BH_HoaDonUri + 'XoaTheNap?id=' + idThe, 'GET').done(function (data) {
                        ShowMessage_Success("Cập nhật hóa đơn nạp thẻ thành công");
                        SearchTheNap();
                        var objDiary = {
                            ID_NhanVien: idNhanVien,
                            ID_DonVi: idDonVi,
                            ChucNang: "Thẻ giá trị",
                            NoiDung: "Hủy thẻ nạp có mã hóa đơn" + item.MaHoaDon,
                            NoiDungChiTiet: "Hủy thẻ nạp có mã hóa đơn" + item.MaHoaDon,
                            LoaiNhatKy: 3 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
                        };
                        Insert_NhatKyThaoTac_1Param(objDiary);
                    });
                });
            }
            else {
                ShowMessage_Danger(data);
            }
        });
    };


    self.ClickTab_NhatKyThanhToan = function (item) {
        // load data from Quy_HoaDon
        ajaxHelper(Quy_HoaDonUri + 'GetQuyHoaDon_byIDHoaDon?idHoaDon=' + item.ID + '&idHoaDonParent=' + null, 'GET').done(function (data) {
            self.LichSuThanhToan(data);
        });
    }

    self.wasKhoaSo = ko.observable(false);

    self.LoadChiTiet = function (formElement) {
        self.wasKhoaSo(VHeader.CheckKhoaSo(moment(formElement.NgayLapHoaDon).format('YYYY-MM-DD'), formElement.ID_DonVi));

        self.GDVChosing(formElement);
        self.NgayLapHD_Update(undefined);
        var ngaylapHD = moment(formElement.NgayLapHoaDon).format('YYYY-MM-DD HH:mm:ss');
        Get_SoDuTheGiaTri_ofKhachHang(formElement.ID_DoiTuong, ngaylapHD, false);
        $('.txtNgayLapHD').datetimepicker({
            timepicker: true,
            mask: true,
            format: 'd/m/Y H:i',
            maxDate: new Date(),
            onChangeDateTime: function (dp, $input) {
                self.NgayLapHD_Update($input.val());
                CheckNgayLapHD_format(self.NgayLapHD_Update(), formElement.ID_DonVi);
            }
        });

        var thisObj = event.currentTarget;
        var ulTab = $(thisObj).next().find('.op-object-detail.nav-tabs');
        ulTab.children('li').removeClass('active');
        ulTab.children('li').eq(0).addClass('active');
        // active tabcontent
        ulTab.next().children('.tab-pane').removeClass('active');
        ulTab.next().children('.tab-pane:eq(0)').addClass('active');
    };

    function CheckNgayLapHD_format(valDate, idChiNhanh) {

        var dateNow = moment(new Date()).format('YYYY-MM-DD HH:mm');
        var ngayLapHD = moment(valDate, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');

        if (valDate === '') {
            ShowMessage_Danger('Vui lòng nhập ngày lập phiếu nhập');
            return false;
        }

        if (valDate.indexOf('_') > -1) {
            ShowMessage_Danger('Ngày lập phiếu nhập chưa đúng định dạng');
            return false;
        }

        if (ngayLapHD > dateNow) {
            ShowMessage_Danger('Ngày lập phiếu nhập vượt quá thời gian hiện tại');
            return false;
        }
        let chotSo = VHeader.CheckKhoaSo(moment(ngayLapHD, 'YYYY-MM-DD HH:mm').format('YYYY-MM-DD'), idChiNhanh);
        if (chotSo) {
            ShowMessage_Danger(VHeader.warning.ChotSo.Update);
            return false;
        }
        return true;
    }

    self.DownloadFileTeamplateXLS_Export = function (pathFile) {
        var url = "/api/DanhMuc/DM_HangHoaAPI/Download_fileExcel?fileSave=" + pathFile;
        window.location.href = url;
    };

    self.formatDateTime = function () {
        $('.datepicker_mask').datetimepicker(
            {
                format: "d/m/Y H:i",
                mask: true,
                timepicker: true,
            });
    }

    function GetAllChiNhanh() {
        ajaxHelper('/api/DanhMuc/DM_DonViAPI/' + 'GetListDonVi_User?ID_NguoiDung=' + userID, 'GET').done(function (data) {
            if (data !== null && data.length > 0) {
                self.ChiNhanhs(data);

                var obj = {
                    ID: idDonVi,
                    TenDonVi: $('#_txtTenDonVi').text()
                }
                self.MangNhomDV.push(obj);
                vmThemMoiNhomKhach.listData.ChiNhanhs = data;
            }
        });
    }

    function CheckQuyenExist(maquyen) {
        var role = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen === maquyen;
        });
        return role.length > 0;
    }

    // Check Quyen by ID_User
    self.Quyen_NguoiDung = ko.observableArray();
    function GetHT_Quyen_ByNguoiDung() {
        ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetHT_NhomNguoiDung?idnguoidung=" + userID + '&iddonvi=' + idDonVi, 'GET').done(function (data) {
            if (data !== null) {
                self.Quyen_NguoiDung(data.HT_Quyen_NhomDTO);
                self.Allow_ChangeTimeSoQuy(CheckQuyenExist('SoQuy_ThayDoiThoiGian'));
                self.Show_BtnThanhToanCongNo(CheckQuyenExist('KhachHang_ThanhToanNo'));

                vmThemMoiKhach.role.KhachHang.CapNhat = CheckQuyenExist('KhachHang_CapNhat');
                vmThemMoiKhach.role.KhachHang.ThemMoi = CheckQuyenExist('KhachHang_ThemMoi');
                vmThemMoiKhach.role.NhomKhachHang.ThemMoi = CheckQuyenExist('NhomKhachHang_ThemMoi');
            }
            else {
                ShowMessage_Danger('Không có quyền xem danh sách ' + sLoai);
            }
        });
    }

    $(window.document).on('shown.bs.modal', '.modal', function () {
        window.setTimeout(function () {
            $('[autofocus]', this).focus();
            $('[autofocus]').select();
        }.bind(this), 100);

        $('.datepicker_mask').datetimepicker({
            timepicker: false,
            mask: false,
            format: 'd/m/Y H:i',
        });
    });
    $.datetimepicker.setLocale('vi');

    function LoadHtmlGrid() {
        LocalCaches.LoadFirstColumnGrid('ENapTien', $('#myList ul li input[type = checkbox]'), self.ListCheckBox());
    }

    // print
    self.CongTy = ko.observableArray();
    function GetInforCongTy() {
        ajaxHelper('/api/DanhMuc/HT_API/' + 'GetHT_CongTy', 'GET').done(function (data) {
            if (data !== null) {
                self.CongTy(data);
                vmThemMoiTheNap.inforCongTy = {
                    TenCongTy: self.CongTy()[0].TenCongTy,
                    DiaChiCuaHang: self.CongTy()[0].DiaChi,
                    LogoCuaHang: Open24FileManager.hostUrl + self.CongTy()[0].DiaChiNganHang,
                    TenChiNhanh: VHeader.TenDonVi,
                };
                vmTraLaiTGT.inforCongTy = vmThemMoiTheNap.inforCongTy;
            }
        });
    }

    function loadMauIn() {
        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/GetListMauIn?typeChungTu=TGT' + '&idDonVi=' + idDonVi,
            type: 'GET',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                self.ListTypeMauIn(result);
            }
        });
    }

    function GetInforHDPrint(objHD) {
        var hd = $.extend({}, objHD);
        hd.NgayLapHoaDon = moment(hd.NgayLapHoaDon).format('DD/MM/YYYY HH:mm:ss');;
        hd.TenChiNhanh = hd.TenDonVi;
        hd.DienThoaiKhachHang = hd.SoDienThoai;
        hd.MucNap = formatNumber(hd.MucNap);
        hd.KhuyenMai = formatNumber(hd.KhuyenMaiVND);
        hd.TongTienHang = formatNumber(hd.MucNap);
        hd.TongGiamGia = formatNumber(hd.ChietKhauVND);
        let conno = formatNumberToFloat(hd.PhaiThanhToan) - formatNumberToFloat(hd.KhachDaTra);
        hd.NoSau = formatNumber(conno);
        hd.PhaiThanhToan = formatNumber(hd.PhaiThanhToan);
        hd.DaThanhToan = formatNumber(hd.KhachDaTra);
        hd.DienGiai = hd.GhiChu;
        hd.TenCuaHang = $('#hd_TenDonVi').val();
        hd.TienBangChu = DocSo(hd.KhachDaTra);
        if (self.CongTy().length > 0) {
            hd.LogoCuaHang = Open24FileManager.hostUrl + self.CongTy()[0].DiaChiNganHang;
        }

        // TheGiaTri
        hd.TongTaiKhoanThe = formatNumber(self.TongTaiKhoanThe());
        var dasudung = self.SuDungThe() - self.HoanTraTheGiaTri();
        hd.TongSuDungThe = formatNumber(dasudung);
        hd.SoDuConLai = formatNumber(self.SoDuTheGiaTri());

        hd.TienMat = formatNumber3Digit(hd.TienMat);
        hd.TienATM = formatNumber3Digit(hd.TienATM);
        hd.TienGui = formatNumber3Digit(hd.TienGui);
        hd.ChuyenKhoan = hd.TienGui;

        let pthuc = '';
        if (formatNumberToFloat(objHD.TienMat) > 0) {
            pthuc += 'Tiền mặt, ';
        }
        if (formatNumberToFloat(objHD.TienATM) > 0) {
            pthuc += 'POS, ';
        }
        if (formatNumberToFloat(objHD.TienGui) > 0) {
            pthuc += 'Chuyển khoản, ';
        }
        hd.PhuongThucTT = Remove_LastComma(pthuc);

        return hd;
    }

    function GetInforPhieuThu(objHD) {
        objHD.TenCuaHang = self.CongTy()[0].TenCongTy;
        objHD.DiaChiCuaHang = self.CongTy()[0].DiaChi;
        objHD.LogoCuaHang = Open24FileManager.hostUrl + self.CongTy()[0].DiaChiNganHang;
        objHD.ChiNhanhBanHang = objHD.TenChiNhanh;
        objHD.MaPhieu = objHD.MaHoaDon;
        objHD.NgayLapHoaDon = moment(objHD.NgayLapHoaDon).format('DD/MM/YYYY HH:mm:ss');
        objHD.NguoiNhanTien = objHD.NguoiNopTien;
        objHD.DiaChiKhachHang = self.ItemHoaDon().DiaChiKhachHang;
        objHD.DienThoaiKhachHang = self.ItemHoaDon().DienThoaiKhachHang;
        objHD.TienBangChu = DocSo(formatNumberToInt(objHD.TongTienThu));
        objHD.GiaTriPhieu = formatNumber(objHD.TongTienThu);
        return objHD;
    }

    self.InPhieuThu = function (item) {
        var itemHDFormat = GetInforPhieuThu(item);
        self.InforHDprintf(itemHDFormat);
        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=SQPT&idDonVi=' + idDonVi,
            type: 'GET',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                var data = result;
                data = data.concat('<script src="/Scripts/knockout-3.4.2.js"></script>');
                data = data.concat("<script > var item1=[], item4=[], item5=[] ; var item2=[] ;var item3=" + JSON.stringify(itemHDFormat) + "; </script>");
                data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                PrintExtraReport(data);
            }
        });
    }

    self.InTheNap = function (item) {
        var itemHDFormat = GetInforHDPrint(item, false);
        self.InforHDprintf(itemHDFormat);

        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=TGT&idDonVi=' + idDonVi,
            type: 'GET',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                var data = result;
                data = data.concat('<script src="/Scripts/knockout-3.4.2.js"></script>');
                data = data.concat("<script > var item1=[]"
                    + "; var item2= [], item4= [], item5 =[]"
                    + "; var item3=" + JSON.stringify(self.InforHDprintf()) + "; </script>");
                data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>"); // MauInTeamplate.js: used to bind data in knockout
                PrintExtraReport(data); // assign content HTML into frame
            }
        });
    }

    self.InTheNap_ByID = function (item, key) {
        var itemHDFormat = GetInforHDPrint(item, false);
        self.InforHDprintf(itemHDFormat);

        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/GetContentFIlePrint_ValueCard?idMauIn=' + key,
            type: 'GET',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                var data = result;
                data = data.concat('<script src="/Scripts/knockout-3.4.2.js"></script>');
                data = data.concat("<script > var item1= [], item4= [], item5 =[]" + "; var item2= []" + ";var item3=" + JSON.stringify(itemHDFormat) + "; </script>");
                data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                PrintExtraReport(data);
            }
        });
    }

    self.vm_showModalDiscount = function (item) {
        let phaiTT = formatNumberToFloat(item.PhaiThanhToan);
        let daTT = formatNumberToFloat(item.KhachDaTra);
        let obj = {
            ID: item.ID,
            LoaiHoaDon: 22,
            MaHoaDon: item.MaHoaDon,
            TongThanhToan: phaiTT,
            TongTienThue: 0,
            ThucThu: daTT,
            DaThuTruoc: daTT,
            ConNo: phaiTT - daTT,
            TongPhiNganHang: 0,
        }
        vmHoaHongHoaDon.GetChietKhauHoaDon_byID(obj);
    }

    self.ShowPopup_InforHD_PhieuThu = function (item, itHD) {
        vmThanhToan.showModalUpdate(item.ID, itHD.ConNo);
    }
};

var modelNapThe = new ViewModel();
ko.applyBindings(modelNapThe, document.getElementById('modalTheNap'));

$(function () {
    $('#vmThemMoiTheNap').on('hidden.bs.modal', function () {
        if (vmThemMoiTheNap.saveOK) {
            modelNapThe.GetListTheNap();
            vmThemMoiKhach.NangNhomKhachHang(vmThemMoiTheNap.newHoaDon.ID_DoiTuong);
        }
    })

    $(window.document).on('shown.bs.modal', '.modal', function () {
        window.setTimeout(function () {
            $('[autofocus]', this).focus();
            $('[autofocus]').select();
        }.bind(this), 100);
    });
})

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
