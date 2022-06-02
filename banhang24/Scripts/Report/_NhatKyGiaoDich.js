var modelDiaryTrans = function () {
    var self = this;
    self.ChiNhanhs = ko.observableArray();
    self.ChiNhanhChosed = ko.observableArray();
    self.ArrDonVi = ko.observableArray();
    self.LstIDDonVi = ko.observableArray();
    self.ListAllDoiTuong = ko.observableArray();
    self.NhatKyGiaoDich_HoaDonBan = ko.observableArray();
    self.NhatKyGiaoDich_GoiDichVu = ko.observableArray();
    self.NhatKyGiaoDich_TheGiaTri = ko.observableArray();
    self.NhatKyGiaoDich_HDSC = ko.observableArray();
    self.ID_KhachHang = ko.observable();
    self.FromDate = ko.observable();
    self.ToDate = ko.observable();
    self.PageSize = ko.observable(10);
    self.Export_ChiNhanh = ko.observable();

    self.HD_IsActive = ko.observable(false);
    self.GDV_IsActive = ko.observable(false);
    self.TGT_IsActive = ko.observable(false);
    self.HDSC_IsActive = ko.observable(false);

    self.HD_SumTienHang = ko.observable(0);
    self.HD_SumTienThue = ko.observable(0);
    self.HD_SumGiamGia = ko.observable(0);
    self.HD_SumPhaiThanhToan = ko.observable(0);
    self.HD_SumTienMat = ko.observable(0);
    self.HD_SumTienGui = ko.observable(0);
    self.HD_SumThuTuThe = ko.observable(0);
    self.HD_SumDaThanhToan = ko.observable(0);

    self.GDV_SumTienHang = ko.observable(0);
    self.GDV_SumTienThue = ko.observable(0);
    self.GDV_SumGiamGia = ko.observable(0);
    self.GDV_SumPhaiThanhToan = ko.observable(0);
    self.GDV_SumTienMat = ko.observable(0);
    self.GDV_SumTienGui = ko.observable(0);
    self.GDV_SumThuTuThe = ko.observable(0);
    self.GDV_SumDaThanhToan = ko.observable(0);

    self.TGT_SumTienHang = ko.observable(0);
    self.TGT_SumChiPhi = ko.observable(0);
    self.TGT_SumChietKhau = ko.observable(0);
    self.TGT_SumGiamGia = ko.observable(0);
    self.TGT_SumPhaiThanhToan = ko.observable(0);
    self.TGT_SumTienMat = ko.observable(0);
    self.TGT_SumTienGui = ko.observable(0);
    self.TGT_SumDaThanhToan = ko.observable(0);

    self.HDSC_SumTienHang = ko.observable(0);
    self.HDSC_SumChiPhi = ko.observable(0);
    self.HDSC_SumChietKhau = ko.observable(0);
    self.HDSC_SumGiamGia = ko.observable(0);
    self.HDSC_SumPhaiThanhToan = ko.observable(0);
    self.HDSC_SumTienMat = ko.observable(0);
    self.HDSC_SumTienGui = ko.observable(0);
    self.HDSC_SumDaThanhToan = ko.observable(0);

    self.HD_CurrentPage = ko.observable(0);
    self.HD_TotalRow = ko.observableArray();
    self.HD_TotalPage = ko.observableArray();
    self.HD_VisiblePrev = ko.observable(false);
    self.HD_VisibleNext = ko.observable(false);
    self.HD_PageView = ko.observable('');
    self.HD_ListPage = ko.observableArray();

    self.GDV_CurrentPage = ko.observable(0);
    self.GDV_TotalRow = ko.observableArray();
    self.GDV_TotalPage = ko.observableArray();
    self.GDV_VisiblePrev = ko.observable(false);
    self.GDV_VisibleNext = ko.observable(false);
    self.GDV_PageView = ko.observable('');
    self.GDV_ListPage = ko.observableArray();

    self.TGT_CurrentPage = ko.observable(0);
    self.TGT_TotalRow = ko.observableArray();
    self.TGT_TotalPage = ko.observableArray();
    self.TGT_VisiblePrev = ko.observable(false);
    self.TGT_VisibleNext = ko.observable(false);
    self.TGT_PageView = ko.observable('');
    self.TGT_ListPage = ko.observableArray();

    self.HDSC_CurrentPage = ko.observable(0);
    self.HDSC_TotalRow = ko.observableArray();
    self.HDSC_TotalPage = ko.observableArray();
    self.HDSC_VisiblePrev = ko.observable(false);
    self.HDSC_VisibleNext = ko.observable(false);
    self.HDSC_PageView = ko.observable('');
    self.HDSC_ListPage = ko.observableArray();

    self.IsGara = ko.observable(VHeader.IdNganhNgheKinhDoanh.toUpperCase() === 'C16EDDA0-F6D0-43E1-A469-844FAB143014');

    self.formatDate = function () {
        $('#modalDiaryTrans .datepicker_mask').datetimepicker(
            {
                format: "d/m/Y",
                mask: true,
                timepicker: false,
                maxDate: new Date(),
            });
        self.SearchNhatKy();
    }

    function ResetFilterSearch() {
        self.HD_CurrentPage(0);
        self.GDV_CurrentPage(0);
        self.TGT_CurrentPage(0);
        self.HDSC_CurrentPage(0);
    }

    self.FromDate.subscribe(function () {
        ResetFilterSearch();
        self.SearchNhatKy();
    });
    self.ToDate.subscribe(function () {
        ResetFilterSearch();
        self.SearchNhatKy();
    });

    self.CloseDonVi = function (item) {
        var TenChiNhanh = '';
        var arrID = [];
        self.ChiNhanhChosed.remove(item);
        var tenchinhanhs = '';

        if (item.ID === '00000000-0000-0000-0000-0000-000000000000') {
            arrID = $.map(self.ChiNhanhs(), function (x) {
                return x.ID;
            });
            tenchinhanhs = 'Tất cả chi nhánh';
        }
        else {
            self.ArrDonVi.unshift(item);

            if (self.ChiNhanhChosed().length === 0) {
                $("#modalDiaryTrans .nameDonVi").attr("placeholder", "Chọn chi nhánh...");
                arrID = $.map(self.ChiNhanhs(), function (x) {
                    return x.ID;
                });
                tenchinhanhs = 'Tất cả chi nhánh';
            }
            else {
                arrID = $.map(self.ChiNhanhChosed(), function (x) {
                    return x.ID;
                });

                for (let i = 0; i < self.ChiNhanhChosed().length; i++) {
                    tenchinhanhs += self.ChiNhanhChosed()[i].TenDonVi + ', ';
                }
                tenchinhanhs = Remove_LastComma(tenchinhanhs);
            }
            $('#modalDiaryTrans .dropdown-menu li').each(function () {
                if ($(this).attr('id') === item.ID) {
                    $(this).find('i').remove();
                }
            });
        }
        self.LstIDDonVi(arrID);
        self.Export_ChiNhanh(tenchinhanhs);
        self.SearchNhatKy();
    }

    self.SelectedDonVi = function (item) {
        event.stopPropagation();
        var arrIDDonVi = [];
        var tenchinhanhs = '';
        // all
        if (item.ID === undefined) {
            let arrID = $.map(self.ChiNhanhs(), function (x) {
                return x.ID;
            });
            self.LstIDDonVi(arrID);
            arrIDDonVi = self.ArrDonVi().map(function (x) {
                return x.ID;
            });
            // push again lstDV has chosed
            for (let i = 0; i < self.ChiNhanhChosed().length; i++) {
                if ($.inArray(self.ChiNhanhChosed()[i].ID, arrIDDonVi) === -1 && self.ChiNhanhChosed()[i].ID !== '00000000-0000-0000-0000-0000-000000000000') {
                    self.ArrDonVi().unshift(self.ChiNhanhChosed()[i]);
                }
            }
            self.ChiNhanhChosed([{
                ID: '00000000-0000-0000-0000-0000-000000000000', TenDonVi: 'Tất cả chi nhánh'
            }]);
            tenchinhanhs = 'Tất cả chi nhánh';
        }
        else {
            for (var i = 0; i < self.ChiNhanhChosed().length; i++) {
                if ($.inArray(self.ChiNhanhChosed()[i].ID, arrIDDonVi) === -1) {
                    arrIDDonVi.push(self.ChiNhanhChosed()[i].ID);
                }
                if (self.ChiNhanhChosed()[i].ID === '00000000-0000-0000-0000-0000-000000000000') {
                    self.ChiNhanhChosed().splice(i, 1);
                }
            }
            if ($.inArray(item.ID, arrIDDonVi) === -1) {
                self.ChiNhanhChosed.push(item);
                $("#modalDiaryTrans .nameDonVi").removeAttr('placeholder');
            }

            //thêm dấu check vào đối tượng được chọn
            $('#modalDiaryTrans .dropdown-menu li').each(function () {
                if ($(this).attr('id') === item.ID) {
                    $(this).find('i').remove();
                    $(this).append('<i class="fa fa-check check-after-li"></i>')
                }
            });
            let arrID = $.map(self.ChiNhanhChosed(), function (x) {
                return x.ID;
            });
            self.LstIDDonVi(arrID);

            for (let i = 0; i < self.ChiNhanhChosed().length; i++) {
                tenchinhanhs += self.ChiNhanhChosed()[i].TenDonVi + ', ';
            }
            tenchinhanhs = Remove_LastComma(tenchinhanhs);
        }
        // remove donvi has chosed in lst
        var arr = $.grep(self.ArrDonVi(), function (x) {
            return x.ID !== item.ID;
        });
        self.ArrDonVi(arr);
        self.Export_ChiNhanh(tenchinhanhs);
        event.preventDefault();
        return false;
    }

    self.VisibleBanHang = ko.computed(function () {
        var hasData = self.NhatKyGiaoDich_HoaDonBan().length > 0;
        self.HD_IsActive(hasData);
        return hasData;
    });
    self.VisibleHDSC = ko.computed(function () {
        var hasData = self.NhatKyGiaoDich_HDSC().length > 0 && self.IsGara();
        self.HDSC_IsActive(self.VisibleBanHang() === false && hasData);
        return hasData;
    });

    self.VisibleGoiDV = ko.computed(function () {
        var hasData = self.NhatKyGiaoDich_GoiDichVu().length > 0;
        self.GDV_IsActive(self.VisibleBanHang() === false && self.VisibleHDSC() === false && hasData);
        return hasData;
    });
    self.VisibleTheGiaTri = ko.computed(function () {
        var hasData = self.NhatKyGiaoDich_TheGiaTri().length > 0;
        self.TGT_IsActive(self.VisibleBanHang() === false && self.VisibleHDSC() === false && self.VisibleGoiDV() === false && hasData);
        return hasData;
    })

    self.ChangeTab = function (type) {
        switch (type) {
            case 1:
                self.HD_IsActive(true);
                self.GDV_IsActive(false);
                self.TGT_IsActive(false);
                self.HDSC_IsActive(false);
                break;
            case 19:
                self.GDV_IsActive(true);
                self.HD_IsActive(false);
                self.TGT_IsActive(false);
                self.HDSC_IsActive(false);
                break;
            case 22:
                self.TGT_IsActive(true);
                self.GDV_IsActive(false);
                self.HD_IsActive(false);
                self.HDSC_IsActive(false);
                break;
            case 25:
                self.HDSC_IsActive(true);
                self.GDV_IsActive(false);
                self.HD_IsActive(false);
                self.TGT_IsActive(false);
                break;
        }
    }

    function GetNhatKyGiaoDich_ofKhachHang(loaichungtu, isExport) {
        var currentpage = 0;
        switch (loaichungtu) {
            case 1:
                currentpage = self.HD_CurrentPage();
                break;
            case 19:
                currentpage = self.GDV_CurrentPage();
                break;
            case 22:
                currentpage = self.TGT_CurrentPage();
                break;
            case 25:
                currentpage = self.HDSC_CurrentPage();
                break;
        }
        var param = {
            LstIDChiNhanh: self.LstIDDonVi(),
            LstIDNhomKhach: [self.ID_KhachHang()], // muontam: ID_KhachHang
            LoaiChungTus: loaichungtu,
            FromDate: moment(self.FromDate(), 'DD/MM/YYYY').format('YYYY-MM-DD'),
            ToDate: moment(self.ToDate(), 'DD/MM/YYYY').add('days', 1).format('YYYY-MM-DD'),
            TextSearch: '',
            CurrentPage: currentpage,
            PageSize: self.PageSize(),
        }
        if (isExport) {
            param.Export_Time = 'Từ ngày '.concat(self.FromDate(), ' đến ngày ', self.ToDate());
            param.Export_ChiNhanh = self.Export_ChiNhanh();
            param.CurrentPage = 0;
            param.PageSize = 1000;

            ajaxHelper('/api/DanhMuc/ReportAPI/Export_NhatKyGiaoDichKhachHang', 'POST', param).done(function (url) {
                $('.table-reponsive').gridLoader({ show: false });
                if (url !== "") {
                    self.DownloadFileTeamplateXLSX(url);

                    var mydata = {
                        objDiary: {
                            ID_NhanVien: $('.idnhanvien').text(),
                            ID_DonVi: $('#hd_IDdDonVi').val(),
                            ChucNang: "Báo cáo bán hàng theo khách hàng",
                            NoiDung: "Xuất báo cáo khách hàng - nhật ký giao dịch",
                            NoiDungChiTiet: "Nội dung chi tiết:".concat('<br /> - Chi nhánh: ', param.Export_ChiNhanh, ' <br /> - Thời gian: ', param.Export_Time),
                            LoaiNhatKy: 6,
                        }
                    }
                    ajaxHelper('/api/DanhMuc/SaveDiary/post_NhatKySuDung', 'POST', mydata).done(function (url) {

                    });
                }
            })
        }
        else {
            ajaxHelper('/api/DanhMuc/ReportAPI/' + "GetNhatKyGiaoDich_ofKhachHang", 'POST', param).done(function (x) {
                if (x.res) {
                    switch (loaichungtu) {
                        case 1:
                            self.NhatKyGiaoDich_HoaDonBan(x.data);
                            self.HD_SumTienHang(x.SumTienHang);
                            self.HD_SumTienThue(x.SumTienThue);
                            self.HD_SumGiamGia(x.SumGiamGia);
                            self.HD_SumPhaiThanhToan(x.SumPhaiThanhToan);
                            self.HD_SumTienMat(x.SumTienMat);
                            self.HD_SumTienGui(x.SumTienGui);
                            self.HD_SumThuTuThe(x.SumThuTuThe);
                            self.HD_SumDaThanhToan(x.SumDaThanhToan);

                            self.HD_TotalPage(x.TotalPage);
                            self.HD_VisiblePrev(x.VisiblePrev);
                            self.HD_VisibleNext(x.VisibleNext);
                            self.HD_PageView(x.PageView_Text);
                            self.HD_ListPage(x.ListPage);
                            break;
                        case 19:
                            self.NhatKyGiaoDich_GoiDichVu(x.data);
                            self.GDV_SumTienHang(x.SumTienHang);
                            self.GDV_SumTienThue(x.SumTienThue);
                            self.GDV_SumGiamGia(x.SumGiamGia);
                            self.GDV_SumPhaiThanhToan(x.SumPhaiThanhToan);
                            self.GDV_SumTienMat(x.SumTienMat);
                            self.GDV_SumTienGui(x.SumTienGui);
                            self.GDV_SumThuTuThe(x.SumThuTuThe);
                            self.GDV_SumDaThanhToan(x.SumDaThanhToan);

                            self.GDV_TotalPage(x.TotalPage);
                            self.GDV_VisiblePrev(x.VisiblePrev);
                            self.GDV_VisibleNext(x.VisibleNext);
                            self.GDV_PageView(x.PageView_Text);
                            self.GDV_ListPage(x.ListPage);
                            break;
                        case 22:
                            self.NhatKyGiaoDich_TheGiaTri(x.data);
                            self.TGT_SumTienHang(x.SumTienHang);
                            self.TGT_SumChiPhi(x.SumChiPhi);
                            self.TGT_SumChietKhau(x.SumChietKhau);
                            self.TGT_SumGiamGia(x.SumGiamGia);
                            self.TGT_SumPhaiThanhToan(x.SumPhaiThanhToan);
                            self.TGT_SumTienMat(x.SumTienMat);
                            self.TGT_SumTienGui(x.SumTienGui);
                            self.TGT_SumDaThanhToan(x.SumDaThanhToan);

                            self.TGT_TotalPage(x.TotalPage);
                            self.TGT_VisiblePrev(x.VisiblePrev);
                            self.TGT_VisibleNext(x.VisibleNext);
                            self.TGT_PageView(x.PageView_Text);
                            self.TGT_ListPage(x.ListPage);
                            break;
                        case 25:
                            console.log('param', x.data)

                            self.NhatKyGiaoDich_HDSC(x.data);
                            self.HDSC_SumTienHang(x.SumTienHang);
                            self.HDSC_SumChiPhi(x.SumChiPhi);
                            self.HDSC_SumChietKhau(x.SumChietKhau);
                            self.HDSC_SumGiamGia(x.SumGiamGia);
                            self.HDSC_SumPhaiThanhToan(x.SumPhaiThanhToan);
                            self.HDSC_SumTienMat(x.SumTienMat);
                            self.HDSC_SumTienGui(x.SumTienGui);
                            self.HDSC_SumDaThanhToan(x.SumDaThanhToan);

                            self.HDSC_TotalPage(x.TotalPage);
                            self.HDSC_VisiblePrev(x.VisiblePrev);
                            self.HDSC_VisibleNext(x.VisibleNext);
                            self.HDSC_PageView(x.PageView_Text);
                            self.HDSC_ListPage(x.ListPage);
                            break;
                    }
                    //console.log(x);
                }
            });
        }
    }

    self.SearchNhatKy = function () {
        GetNhatKyGiaoDich_ofKhachHang(1);
        GetNhatKyGiaoDich_ofKhachHang(19);
        GetNhatKyGiaoDich_ofKhachHang(22);
        GetNhatKyGiaoDich_ofKhachHang(25);
    }

    self.ID_KhachHang.subscribe(function () {
        self.SearchNhatKy();
    });

    self.Export = function () {
        GetNhatKyGiaoDich_ofKhachHang('1,19,22,25', true);
    }

    self.DownloadFileTeamplateXLSX = function (item) {
        console.log(item);
        var url = "/api/DanhMuc/DM_HangHoaAPI/Download_fileExcel?fileSave=" + item;
        window.location.href = url;
    }

    self.StartPage = function (type) {
        switch (type) {
            case 1:
                self.HD_CurrentPage(0);

                break;
            case 19:
                self.GDV_CurrentPage(0);
                break;
            case 22:
                self.TGT_CurrentPage(0);
                break;
            case 25:
                self.HDSC_CurrentPage(0);
                break;
        }
        GetNhatKyGiaoDich_ofKhachHang(type);
    }
    self.BackPage = function (type) {
        switch (type) {
            case 1:
                if (self.HD_CurrentPage() > 1) {
                    self.HD_CurrentPage(self.HD_CurrentPage() - 1);
                }
                break;
            case 19:
                if (self.GDV_CurrentPage() > 1) {
                    self.GDV_CurrentPage(self.GDV_CurrentPage() - 1);
                }
                break;
            case 22:
                currentpage = self.TGT_CurrentPage();
                if (self.TGT_CurrentPage() > 1) {
                    self.TGT_CurrentPage(self.TGT_CurrentPage() - 1);
                }
                break;
            case 25:
                currentpage = self.HDSC_CurrentPage();
                if (self.HDSC_CurrentPage() > 1) {
                    self.HDSC_CurrentPage(self.HDSC_CurrentPage() - 1);
                }
                break;
        }
        GetNhatKyGiaoDich_ofKhachHang(type);
    }

    self.GotoPage = function (type, sotrang) {
        sotrang = sotrang - 1;
        switch (type) {
            case 1:
                self.HD_CurrentPage(sotrang);
                break;
            case 19:
                self.GDV_CurrentPage(sotrang);
                break;
            case 22:
                self.TGT_CurrentPage(sotrang);
                break;
            case 25:
                self.HDSC_CurrentPage(sotrang);
                break;
        }
        GetNhatKyGiaoDich_ofKhachHang(type);
    }

    self.GoToNextPage = function (type) {
        switch (type) {
            case 1:
                self.HD_CurrentPage(self.HD_CurrentPage() + 1);
                break;
            case 19:
                self.GDV_CurrentPage(self.GDV_CurrentPage() + 1);
                break;
            case 22:
                self.TGT_CurrentPage(self.TGT_CurrentPage() + 1);
                break;
            case 22:
                self.HDSC_CurrentPage(self.HDSC_CurrentPage() + 1);
                break;
        }
        GetNhatKyGiaoDich_ofKhachHang(type);
    }
    self.EndPage = function (type) {
        switch (type) {
            case 1:
                self.HD_CurrentPage(self.HD_TotalPage() - 1);
                break;
            case 19:
                self.GDV_CurrentPage(self.GDV_TotalPage() - 1);
                break;
            case 22:
                self.TGT_CurrentPage(self.TGT_TotalPage() - 1);
                break;
            case 25:
                self.HDSC_CurrentPage(self.HDSC_TotalPage() - 1);
                break;
        }
        GetNhatKyGiaoDich_ofKhachHang(type);
    }
}
