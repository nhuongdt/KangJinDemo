var FormModel_GiaBan = function () {
    //khai báo model
    var self = this;

    self.ID = ko.observable();
    self.TenGiaBan = ko.observable();
    self.ApDung = ko.observable(true);
    self.TuNgay = ko.observable();
    var time = new Date();
    self.DenNgay = ko.observable(time.setFullYear(time.getFullYear() + 1));

    self.GhiChu = ko.observable();
    self.TatCaDoiTuong = ko.observable(true);
    self.TatCaDonVi = ko.observable(true);
    self.TatCaNhanVien = ko.observable(true);


    self.SetData = function (item) {
        self.ID(item.ID);
        self.TenGiaBan(item.TenGiaBan);
        self.ApDung(item.ApDung);
        self.TuNgay(item.TuNgay);
        self.DenNgay(item.DenNgay);
        self.GhiChu(item.GhiChu);
        self.TatCaDoiTuong(item.TatCaDoiTuong);
        self.TatCaDonVi(item.TatCaDonVi);
        self.TatCaNhanVien(item.TatCaNhanVien);
    };
}

var CTGiaViewModel = function () {

    //Khai báo
    var self = this;
    var GiaBanUri = '/api/DanhMuc/DM_GiaBanAPI/';
    var NhomHHUri = '/api/DanhMuc/DM_NhomHangHoaAPI/';
    var _IDchinhanh = $('#hd_IDdDonVi').val();
    var _IDNhomNguoiDung = $('.idnhomnguoidung').text();
    var _IDNguoiDung = $('.idnguoidung').text();
    var _id_NhanVien = VHeader.IdNhanVien;
    var _userLogin = VHeader.UserLogin;
    var DMHangHoaUri = '/api/DanhMuc/DM_HangHoaAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/';
    self.ContinueImport = ko.observable(false);
    self.GiaBanChitiets = ko.observableArray();
    self.GiaBans = ko.observableArray();
    self.ChiNhanhs = ko.observableArray();
    self.NhomDoiTuongs = ko.observableArray();
    self.selectedCN = ko.observable();
    self.selectedNB = ko.observable();
    self.selectedNKH = ko.observable();
    self.ID_GiaBanAD = ko.observable();

    self.NguoiDungs = ko.observableArray();
    self.HangHoas = ko.observableArray();
    self.filterFind = ko.observable();
    self.columsort = ko.observable(null);
    self.sort = ko.observable(null);


    self.selectedHH = ko.observable();
    self.error = ko.observable();
    self.deleteID = ko.observable();
    self.filter = ko.observable();
    self.NhomHangHoas = ko.observableArray();
    self._ThemMoiGiaBan = ko.observable(true);
    self.deleteTenHangHoa = ko.observable();
    self.newGiaBan = ko.observable(new FormModel_GiaBan());
    self._TenBangGia = ko.observable();
    self.selectedGiaBan = ko.observable();
    self.selectedGiaBanName = ko.computed(function () {
        return $("#ddlGiaBan option[value='" + self.selectedGiaBan() + "']").text();
    });
    self.visibleImport = ko.observable(true);
    self.notvisibleImport = ko.computed(function () {
        return !self.visibleImport();
    });

    function logGiaBan() {
    }
    function ajaxHelper(uri, method, data) {
        self.error(''); // Clear error message
        return $.ajax({
            type: method,
            url: uri,
            dataType: 'json',
            contentType: 'application/json',
            data: data ? JSON.stringify(data) : null,
            statusCode: {
                404: function () {
                    self.error("Page not found");
                },
            }
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
            });
    };

    var ReportUri = '/api/DanhMuc/ReportAPI/';
    var _IDDoiTuong = $('.idnguoidung').text();
    //load quyền
    function loadQuyenIndex() {
        var arrQuyen = [];
        ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetHT_NhomNguoiDung?idnguoidung=" + _IDNguoiDung + '&iddonvi=' + _IDchinhanh, 'GET').done(function (data) {
            if (data.HT_Quyen_NhomDTO.length > 0) {
                for (var i = 0; i < data.HT_Quyen_NhomDTO.length; i++) {
                    arrQuyen.push(data.HT_Quyen_NhomDTO[i].MaQuyen);
                }
            }
            localStorage.setItem('lc_CTQuyen', JSON.stringify(arrQuyen));
        });
        ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/' + 'GetCauHinhHeThong/' + _IDchinhanh, 'GET').done(function (data) {
            localStorage.setItem('lc_CTThietLap', JSON.stringify(data));
        });
    }
    loadQuyenIndex();

    self.HangHoa_GiaBan = ko.observable();
    self.HangHoa_XemGiaVon = ko.observable();
    function getQuyen_NguoiDung() {
        ajaxHelper(ReportUri + "getQuyenXemGiaVon?ID_NguoiDung=" + _IDDoiTuong + "&MaQuyen=" + "HangHoa_XemGiaVon", "GET").done(function (data) {
            self.HangHoa_XemGiaVon(data);
        });
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _IDchinhanh + "&MaQuyen=" + "HangHoa_GiaBan", "GET").done(function (data) {
            self.HangHoa_GiaBan(data);
        });
    };
    getQuyen_NguoiDung();

    self.MangNhomDV = ko.observableArray();
    self.selectedCN = function (item) {
        self.newGiaBan().TatCaDonVi(false);
        var arrDV = [];
        for (var i = 0; i < self.MangNhomDV().length; i++) {
            if ($.inArray(self.MangNhomDV()[i], arrDV) === -1) {
                arrDV.push(self.MangNhomDV()[i].ID);

            }
        }
        if ($.inArray(item.ID, arrDV) === -1) {
            self.MangNhomDV.push(item);
        }

        $('#selec-all-DV li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
                $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>');
            }
        });
        $('#choose_TenDonVi input').remove();
    }

    self.MangNhomNgayTrongTuan = ko.observableArray();
    self.selectedNgay = function (item) {
        var arrNgay = [];
        for (var i = 0; i < self.MangNhomNgayTrongTuan().length; i++) {
            if ($.inArray(self.MangNhomNgayTrongTuan()[i], arrNgay) === -1) {
                arrNgay.push(self.MangNhomNgayTrongTuan()[i].TenNgay);

            }
        }
        if ($.inArray(item.TenNgay, arrNgay) === -1) {
            self.MangNhomNgayTrongTuan.push(item);
        }

        $('#selec-all-Ngay li').each(function () {
            if ($(this).attr('id') === item.TenNgay) {
                $(this).find('.fa-check').remove();
                $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>');
            }
        });
        $('#choose_Ngay input').remove();
    }

    self.ChooseAllNgayTT = function () {
        self.MangNhomNgayTrongTuan([]);
        $('#selec-all-Ngay li').each(function () {
            $(this).find('.fa-check').remove();
        });
        $('#choose_Ngay input').remove();
        $('#choose_Ngay').append('<input type="text" id="dllNgayTrongTuan" readonly="readonly" class="dropdown" placeholder="--- Tất cả ---">');
    }

    self.CloseNgay = function (item) {
        self.MangNhomNgayTrongTuan.remove(item);
        if (self.MangNhomNgayTrongTuan().length === 0) {
            $('#choose_Ngay').append('<input type="text" id="dllNgayTrongTuan" readonly="readonly" class="dropdown" placeholder="--- Tất cả ---">');
        }
        // remove check
        $('#selec-all-Ngay li').each(function () {
            if ($(this).attr('id') === item.TenNgay) {
                $(this).find('.fa-check').remove();
            }
        });
    }

    self.MangNghiepVuAD = ko.observableArray();
    self.selectedNghiepVu = function (item) {
        var arrAD = [];
        for (var i = 0; i < self.MangNghiepVuAD().length; i++) {
            if ($.inArray(self.MangNghiepVuAD()[i], arrAD) === -1) {
                arrAD.push(self.MangNghiepVuAD()[i].value);

            }
        }
        if ($.inArray(item.value, arrAD) === -1) {
            self.MangNghiepVuAD.push(item);
        }

        $('#selec-all-NV li').each(function () {
            if ($(this).attr('id') === item.value) {
                $(this).find('.fa-check').remove();
                $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
            }
        });
        $('#choose_NghiepVu input').remove();
    }

    self.ChooseAllNghiepVu = function () {
        self.MangNghiepVuAD([]);
        $('#selec-all-NV li').each(function () {
            $(this).find('.fa-check').remove();
        });
        $('#choose_NghiepVu input').remove();
        $('#choose_NghiepVu').append('<input type="text" id="dllNghiepVu" readonly="readonly" class="dropdown" placeholder="--- Tất cả ---">');
    }

    self.ChooseAllDV = function () {
        self.MangNhomDV([]);
        $('#selec-all-DV li').each(function () {
            $(this).find('.fa-check').remove();
        });
        $('#choose_TenDonVi input').remove();
        $('#choose_TenDonVi').append('<input type="text" id="dllChiNhanh" readonly="readonly" class="dropdown" placeholder="--- Tất cả ---">');
    }

    self.ChooseAllKH = function () {
        self.MangNhomNKH([]);
        $('#selec-all-NKH li').each(function () {
            $(this).find('.fa-check').remove();
        });
        $('#NhomKhachHang input').remove();
        $('#NhomKhachHang').append('<input type="text" id="dllNhomKH" readonly="readonly" class="dropdown" placeholder="--- Tất cả ---">');
    }

    self.ChooseAllNV = function () {
        self.MangNhomNB([]);
        $('#selec-all-NB li').each(function () {
            $(this).find('.fa-check').remove();
        });
        $('#choose-NguoiBan input').remove();
        $('#choose-NguoiBan').append('<input type="text" id="dllNguoiBan" readonly="readonly" class="dropdown" placeholder="--- Tất cả ---">');
    }

    self.CloseNghiepVu = function (item) {
        self.MangNghiepVuAD.remove(item);
        if (self.MangNghiepVuAD().length === 0) {
            $('#choose_NghiepVu').append('<input type="text" id="dllNghiepVu" readonly="readonly" class="dropdown" placeholder="--- Tất cả ---">');
        }
        // remove check
        $('#selec-all-NV li').each(function () {
            if ($(this).attr('id') === item.value) {
                $(this).find('.fa-check').remove();
            }
        });
    }

    self.CloseDV = function (item) {
        self.MangNhomDV.remove(item);
        if (self.MangNhomDV().length === 0) {
            $('#choose_TenDonVi').append('<input type="text" id="dllChiNhanh" readonly="readonly" class="dropdown" placeholder="--- Tất cả ---">');
        }
        // remove check
        $('#selec-all-DV li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
        if (self.MangNhomDV().length === 0) {
            self.newGiaBan().TatCaDonVi(true);
        }
    }

    self.MangNhomNB = ko.observableArray();
    self.selectedNB = function (item) {
        self.newGiaBan().TatCaNhanVien(false);
        var arrNB = [];
        for (var i = 0; i < self.MangNhomNB().length; i++) {
            if ($.inArray(self.MangNhomNB()[i], arrNB) === -1) {
                arrNB.push(self.MangNhomNB()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrNB) === -1) {
            self.MangNhomNB.push(item);
        }

        $('#selec-all-NB li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
                $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
            }
        });
        $('#choose-NguoiBan input').remove();
    }

    self.CloseNB = function (item) {
        self.MangNhomNB.remove(item);
        if (self.MangNhomNB().length === 0) {
            $('#choose-NguoiBan').append('<input type="text" id="dllNguoiBan" readonly="readonly" class="dropdown " placeholder="--- Tất cả ---">');
        }
        // remove check
        $('#selec-all-NB li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
        if (self.MangNhomNB().length === 0) {
            self.newGiaBan().TatCaNhanVien(true);
        }
    }


    self.MangNhomNKH = ko.observableArray();
    self.selectedNKH = function (item) {
        self.newGiaBan().TatCaDoiTuong(false);
        var arrNKH = [];
        for (var i = 0; i < self.MangNhomNKH().length; i++) {
            if ($.inArray(self.MangNhomNKH()[i], arrNKH) === -1) {
                arrNKH.push(self.MangNhomNKH()[i].ID);

            }
        }
        if ($.inArray(item.ID, arrNKH) === -1) {
            self.MangNhomNKH.push(item);
        }
        //$('#choose_DonVi input').remove();
        // thêm dấu check vào đối tượng được chọn
        $('#selec-all-NKH li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
                $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
            }
        });
        $('#choose-NhomKhachHang input').remove();
    }

    self.CloseNKH = function (item) {
        self.MangNhomNKH.remove(item);
        if (self.MangNhomNKH().length === 0) {
            $('#choose-NhomKhachHang').append('<input type="text" id="dllNhomKH" readonly="readonly" class="dropdown " placeholder="--- Tất cả ---">');
        }
        // remove check
        $('#selec-all-NKH li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
        if (self.MangNhomNKH().length === 0) {
            self.newGiaBan().TatCaDoiTuong(true);
        }
    }
    //select
    function getAllGiaBan() {
        ajaxHelper(GiaBanUri + "GetDM_GiaBanByIDDonVi?iddonvi=" + _IDchinhanh, 'GET').done(function (data) {
            if (data != null) {
                self.GiaBans(data);
            }
        });
    };

    self.selectedGiaBan.subscribe(function (newValue) {
        $("#iconSort").remove();
        self.currentPage(0);
        self.columsort(null);
        self.sort(null);
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if (newValue != undefined) {
            self._TenBangGia(self.selectedGiaBanName() === "" ? "Bảng giá chuẩn" : self.selectedGiaBanName());
            SearchHangHoa();
            if ($.inArray('ThietLapGia_Import', lc_CTQuyen) > -1) {
                $('.btnImportCTG').show();
            }
        }
        else {
            self._TenBangGia(self.selectedGiaBanName() === "" ? "Bảng giá chuẩn" : self.selectedGiaBanName());
            $('.btnImportCTG').hide();
            SearchHangHoa();
        }
    })

    //Download file teamplate excel format (*.xls)
    self.DownloadFileTeamplateXLS = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_BangGia.xls";
        window.location.href = url;
    }
    //Download file teamplate excel format (*.xlsx)
    self.DownloadFileTeamplateXLSX = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_BangGia.xlsx";
        window.location.href = url;
    }
    self.showimportBG = function () {
        $('#myModalinport').modal('show');
        self.refreshFileSelect();
        $(".BangBaoLoi").hide();
        $(".NoteImport").show();
    }
    self.fileNameExcel = ko.observable("Lựa chọn file Excel...");
    self.refreshFileSelect = function () {
        self.fileNameExcel("Lựa chọn file Excel...");
        $(".filterFileSelect").hide();
        $(".btnImportExcel").hide();
        self.visibleImport(true);
        document.getElementById('imageUploadForm').value = "";
    }
    $(".filterFileSelect").hide();
    $(".btnImportExcel").hide();
    self.SelectedFileImport = function (vm, evt) {
        self.fileNameExcel(evt.target.files[0].name)
        self.visibleImport(false);
        $(".filterFileSelect").show();
        $(".btnImportExcel").show();
        $(".NoteImport").show();
        $(".BangBaoLoi").hide();
    }
    self.ShowandHide = function () {
        self.insertArticleNews();
    }
    self.loiExcel = ko.observableArray();
    $(".BangBaoLoi").hide();
    self.insertArticleNews = function () {
        $('.table_h10').gridLoader();
        var formData = new FormData();
        var totalFiles = document.getElementById("imageUploadForm").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("imageUploadForm").files[i];
            formData.append("imageUploadForm", file);
        }
        $.ajax({
            type: "POST",
            url: DMHangHoaUri + "ImfortExcelToBangGia?ID_DonVi=" + _IDchinhanh + "&ID_BangGia=" + self.selectedGiaBan(),
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (item) {
                self.loiExcel(item);
                if (self.loiExcel() != null) {
                    self.visibleImport(true);
                    $(".BangBaoLoi").show();
                    $(".NoteImport").hide();
                    $(".filterFileSelect").hide();
                    $(".btnImportExcel").hide();
                    $('.table_h10').gridLoader({ show: false });

                }
                else {
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Import bảng giá thành công", "success");
                    $('#myModalinport').hide();
                    $('.table_h10').gridLoader({ show: false });
                    self.refreshFileSelect();
                    SearchHangHoa();
                    var objDiary = {
                        ID_NhanVien: _id_NhanVien,
                        ID_DonVi: _IDchinhanh,
                        ChucNang: "Chương trình giá",
                        NoiDung: "Import hàng hóa vào bảng giá: " + self._TenBangGia(),
                        NoiDungChiTiet: "Import hàng hóa vào bảng giá: " + self._TenBangGia(),
                        LoaiNhatKy: 5 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
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
                                $('.table_h10').gridLoader({ show: false });
                            },
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            $('.table_h10').gridLoader({ show: false });
                            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ghi nhật ký sử dụng thất bại", "danger");
                        },
                        complete: function () {
                            $('.table_h10').gridLoader({ show: false });
                        }
                    })
                    document.getElementById('imageUploadForm').value = "";
                    $(".NoteImport").show();
                    $(".filterFileSelect").hide();
                    $(".btnImportExcel").hide();
                    $(".BangBaoLoi").hide();
                    $("#myModalinport").modal("hide");
                    SearchHangHoa();
                }
                $('.table_h10').gridLoader({ show: false });
            },
            statusCode: {
                404: function () {
                    $('.table_h10').gridLoader({ show: false });
                },
                406: function (item) {
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + item.responseJSON.Message, "danger")
                    $('.table_h10').gridLoader({ show: false });
                },
                500: function (item) {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Import bảng giá thất bại", "danger");
                    $('.table_h10').gridLoader({ show: false });
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $('.table_h10').gridLoader({ show: false });
            },
        });
    }
    self.addRownError = ko.observableArray();
    self.DoneWithError = function () {
        var rownError = null;
        self.addRownError([]);
        for (var i = 0; i < self.loiExcel().length; i++) {
            if (self.addRownError().length < 1) {
                self.addRownError.push(self.loiExcel()[i].rowError);
            }
            else {
                for (var j = 0; j < self.addRownError().length; j++) {
                    if (self.addRownError()[j] === self.loiExcel()[i].rowError) {
                        break;
                    }
                    if (j == self.addRownError().length - 1) {
                        self.addRownError.push(self.loiExcel()[i].rowError);
                        break;
                    }
                }
            }
        }
        for (var i = 0; i < self.addRownError().length; i++) {
            if (i == 0)
                rownError = self.addRownError()[i];
            else
                rownError = rownError + "_" + self.addRownError()[i];
        }
        $('.table_h10').gridLoader();
        var formData = new FormData();
        var totalFiles = document.getElementById("imageUploadForm").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("imageUploadForm").files[i];
            formData.append("imageUploadForm", file);
        }
        $.ajax({
            type: "POST",
            url: DMHangHoaUri + "ImportBangGiaBoQuaLoi?ID_DonVi=" + _IDchinhanh + "&RownError=" + rownError + '&ID_BangGia=' + self.selectedGiaBan(),
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (item) {
                //self.loiExcel([]);
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Import bảng giá thành công", "success");
                var objDiary = {
                    ID_NhanVien: _id_NhanVien,
                    ID_DonVi: _IDchinhanh,
                    ChucNang: "Chương trình giá",
                    NoiDung: "Import hàng hóa vào bảng giá: " + self._TenBangGia(),
                    NoiDungChiTiet: "Import hàng hóa vào bảng giá: " + self._TenBangGia(),
                    LoaiNhatKy: 5 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
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
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ghi nhật ký sử dụng thất bại!", "danger");
                    },
                    complete: function () {

                    }
                })

                document.getElementById('imageUploadForm').value = "";
                $(".NoteImport").show();
                $(".filterFileSelect").hide();
                $(".btnImportExcel").hide();
                $(".BangBaoLoi").hide();
                $("#myModalinport").modal("hide");
                SearchHangHoa();
                $('.table_h10').gridLoader({ show: false });
            },
            statusCode: {
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Import bảng giá thất bại", "danger");
                $('.table_h10').gridLoader({ show: false });
            },
        });
        //$("div[id ^= 'wait']").text("");
    }

    $('.startImport').attr('disabled', 'false');
    $('.startImport').removeClass("btn-green");
    $('.startImport').addClass("StartImport");
    $('.choseContinue input').on('click', function () {

        if ($(this).val() == 0) {
            $(this).val(1);
            $('.startImport').removeAttr('disabled');
            $('.startImport').addClass("btn-green");
            $('.startImport').removeClass("StartImport");
        }
        else {
            $(this).val(0);
            $('.startImport').attr('disabled', 'false');
            $('.startImport').removeClass("btn-green");
            $('.startImport').addClass("StartImport");
        }
    });

    self.NgayTrongTuans = ko.observableArray([
        { TenNgay: "Thứ 2", value: "2" },
        { TenNgay: "Thứ 3", value: "3" },
        { TenNgay: "Thứ 4", value: "4" },
        { TenNgay: "Thứ 5", value: "5" },
        { TenNgay: "Thứ 6", value: "6" },
        { TenNgay: "Thứ 7", value: "7" },
        { TenNgay: "Chủ nhật", value: "1" }
    ]);

    self.NghiepVuADs = ko.observableArray([
        { TenNghiepVu: "Bán hàng", value: "1" },
        { TenNghiepVu: "Đặt hàng", value: "3" },
        { TenNghiepVu: "Trả hàng", value: "6" },
        { TenNghiepVu: "Gói dịch vụ", value: "19" }
    ]);


    function getAllChiNhanh() {
        ajaxHelper('/api/DanhMuc/DM_DonViAPI/' + "GetListDonVi1", 'GET').done(function (data) {
            self.ChiNhanhs(data);
        });
    }
    getAllChiNhanh();

    function getAllNhanVien() {
        ajaxHelper('/api/DanhMuc/NS_NhanVienAPI/' + "GetListNhanViens", 'GET').done(function (data) {
            self.NguoiDungs(data);
        });
    }
    getAllNhanVien();

    function getListNhomDT() {
        ajaxHelper('/api/DanhMuc/DM_NhomDoiTuongAPI/' + "GetDM_NhomDoiTuong?loaiDoiTuong=" + 1, 'GET').done(function (data) {
            self.NhomDoiTuongs(data);
        });
    }
    getListNhomDT();

    function getAllGiaBanChiTiet(id) {
        if (id === undefined) {
            id = '00000000-0000-0000-0000-000000000000';
        }
        var idBangGia = self.selectedGiaBan();
        if (idBangGia === undefined) {
            idBangGia = '00000000-0000-0000-0000-000000000000';
        }
        SearchHangHoa();
        self._TenBangGia(self.selectedGiaBanName() === "" ? "Bảng giá chuẩn" : self.selectedGiaBanName());
        //ajaxHelper(GiaBanUri + "GetListGiaBans1?id=" + id + "&idGiaBan=" + idBangGia, 'GET').done(function (data) {
        //    self.getChiTietGiaBan(data);
        //    $('#wait').remove();
        //});
    };
    self.getAllGiaBanChiTiet = function (item) {
        $("#iconSort").remove();
        self.columsort(null);
        self.sort(null);
        $('.table_h').gridLoader();
        $('.li-oo').removeClass("yellow")
        $('#tatcanhh a').css("display", "block");
        $('#tatcanhh').addClass("yellow")
        //getAllGiaBanChiTiet(item.ID);
        self.arrIDNhomHang([]);
        SearchHangHoa();
    }

    var time = null
    self.NoteNhomHang = function () {
        //clearTimeout(time);
        //time = setTimeout(
        //    function () {
        tk = $('#SeachNhomHang').val();
        if (tk.trim() != '') {
            ajaxHelper('/api/DanhMuc/ReportAPI/' + "GetListID_NhomHangHoaByTen?TenNhomHang=" + tk, 'GET').done(function (data) {
                self.NhomHangHoas([]);
                for (var i = 0; i < data.length; i++) {
                    if (data[i].ID_Parent == null) {
                        var objParent = {
                            ID: data[i].ID,
                            TenNhomHangHoa: data[i].TenNhomHangHoa,
                            Childs: [],
                        }
                        for (var j = 0; j < data.length; j++) {
                            if (data[j].ID !== data[i].ID && data[j].ID_Parent === data[i].ID) {
                                var objChild =
                                {
                                    ID: data[j].ID,
                                    TenNhomHangHoa: data[j].TenNhomHangHoa,
                                    ID_Parent: data[i].ID,
                                    Child2s: []
                                };
                                for (var k = 0; k < data.length; k++) {
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
                    else {
                        var objParent = {
                            ID: data[i].ID,
                            TenNhomHangHoa: data[i].TenNhomHangHoa,
                            Childs: [],
                        }
                        for (var j = 0; j < data.length; j++) {
                            if (data[j].ID !== data[i].ID && data[j].ID_Parent === data[i].ID) {
                                var objChild =
                                {
                                    ID: data[j].ID,
                                    TenNhomHangHoa: data[j].TenNhomHangHoa,
                                    ID_Parent: data[i].ID,
                                    Child2s: []
                                };
                                for (var k = 0; k < data.length; k++) {
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
                if (self.NhomHangHoas().length > 10) {
                    $('.close-goods').css('display', 'block');
                }
            })
        }
        else {
            GetAllNhomHH();
        }
        //}, 300);
    };



    function GetAllNhomHH() {
        ajaxHelper('/api/DanhMuc/DM_NhomHangHoaAPI/' + 'GetDM_NhomHangHoa', 'GET').done(function (data) {
            localStorage.setItem('lc_NhomHangHoas', JSON.stringify(data));
            self.NhomHangHoas([]);
            for (var i = 0; i < data.length; i++) {
                if (data[i].ID_Parent == null) {
                    var objParent = {
                        ID: data[i].ID,
                        TenNhomHangHoa: data[i].TenNhomHangHoa,
                        Childs: [],
                    }

                    for (var j = 0; j < data.length; j++) {
                        if (data[j].ID !== data[i].ID && data[j].ID_Parent === data[i].ID) {
                            var objChild =
                            {
                                ID: data[j].ID,
                                TenNhomHangHoa: data[j].TenNhomHangHoa,
                                ID_Parent: data[i].ID,
                                Child2s: []
                            };

                            for (var k = 0; k < data.length; k++) {
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
        });
    };
    GetAllNhomHH();


    self.modalDeleteGiaBan = function (GiaBans) {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('ThietLapGia_Xoa', lc_CTQuyen) > -1) {
            self.deleteID(self.newGiaBan().ID());
            self.deleteTenHangHoa(self.newGiaBan().TenGiaBan());
            ajaxHelper(GiaBanUri + 'CheckBangGia_wasUse/' + self.deleteID(), 'GET').done(function (exist) {
                if (exist) {
                    ShowMessage_Danger('Có giao dịch liên quan đến bảng giá. Không thể xóa');
                }
                else {
                    $('#modalpopup_deleteGB').modal('show');
                }
            })
        }
    };

    self.xoaGiaBan = function () {
        ajaxHelper(GiaBanUri + "DeleteDM_GiaBan/" + self.deleteID(), 'GET').done(function (x) {
            if (x.res) {
                ShowMessage_Success('Xóa bảng giá thành công');
                let diary = {
                    ID_NhanVien: _id_NhanVien,
                    ID_DonVi: _IDchinhanh,
                    ChucNang: "Thiết lập giá",
                    NoiDung: "Xóa bảng giá : " + self.deleteTenHangHoa(),
                    NoiDungChiTiet: "Xóa bảng giá : ".concat(self.deleteTenHangHoa(), ', Người xóa: ', _userLogin),
                    LoaiNhatKy: 3 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
                };
                Insert_NhatKyThaoTac_1Param(diary);
            }
            else {
                ShowMessage_Danger(x.mes);
            }
            SearchHangHoa();
            getAllGiaBan();
        })
    };

    self.resetGiaBan = function () {
        self.MangNhomDV([]);
        self.MangNhomNB([]);
        self.MangNhomNKH([]);
        self.ID_GiaBanAD(undefined);
        self.newGiaBan(new FormModel_GiaBan());
    };
    //show modal
    self.themmoicapnhatgiaban = function () {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('ThietLapGia_ThemMoi', lc_CTQuyen) > -1) {

            $('#modalPopuplg_giaban').on('shown.bs.modal', function () {
                var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
                if ($.inArray('ThietLapGia_ThemMoi', lc_CTQuyen) > -1) {
                    $('.addGiaBan').show();
                }
                else {
                    $('.addGiaBan').hide();
                }
            })

            $('.modal-title').text('Thêm bảng giá')
            self._ThemMoiGiaBan(true);
            self.MangNhomDV([]);
            self.MangNhomNB([]);
            self.MangNhomNKH([]);
            self.MangNhomNgayTrongTuan([]);
            self.MangNghiepVuAD([]);
            self.ID_GiaBanAD(undefined);
            self.newGiaBan(new FormModel_GiaBan());
            $('#choose_TenDonVi input').remove();
            $('#choose_TenDonVi').append('<input type="text" id="dllChiNhanh" readonly="readonly" class="dropdown " placeholder="--- Tất cả ---">');

            $('#choose-NguoiBan input').remove();
            $('#choose-NguoiBan').append('<input type="text" id="dllNguoiBan" readonly="readonly" class="dropdown " placeholder="--- Tất cả ---">');

            $('#choose-NhomKhachHang input').remove();
            $('#choose-NhomKhachHang').append('<input type="text" id="dllNhomKH" readonly="readonly" class="dropdown " placeholder="--- Tất cả ---">');
            $('#choose_Ngay input').remove();
            $('#choose_Ngay').append('<input type="text" id="dllNgayTrongTuan" readonly="readonly" class="dropdown " placeholder="--- Tất cả ---">');
            $('#choose_NghiepVu input').remove();
            $('#choose_NghiepVu').append('<input type="text" id="dllNghiepVu" readonly="readonly" class="dropdown " placeholder="--- Tất cả ---">');
            $('.outselect').addClass('open1');


            $('#selec-all-NKH li').each(function () {
                $(this).find('.fa-check').remove();
            });
            $('#selec-all-NB li').each(function () {
                $(this).find('.fa-check').remove();
            });
            $('#selec-all-DV li').each(function () {
                $(this).find('.fa-check').remove();
            });
            $('#selec-all-Ngay li').each(function () {
                $(this).find('.fa-check').remove();
            });
            $('#selec-all-NV li').each(function () {
                $(this).find('.fa-check').remove();
            });
            //document.getElementById("dllChiNhanh").disabled = true;
            //document.getElementById("dllNguoiBan").disabled = true;
            //document.getElementById("dllNhomKH").disabled = true;
            $('#modalPopuplg_giaban').modal('show');
            $('.btnxoa').css('display', 'none');
            $('.updateHHTab li').each(function () {
                $(this).removeClass('active');
            })


            $("a[href='#thongtin']").parent('li').addClass('active');
            $('#thongtin').addClass('active');
            $('#phamviapdung').removeClass('active');
            $('#btnGiaBan').css('display', 'none');
            $(function () {
                $('#txtTenBangGia').select();
            })
        }
    };

    $('#modalPopuplg_giaban').on('shown.bs.modal', function () {
        $('#txtTenBangGia').focus();
        $('#txtTenBangGia').select();
    });

    self.capnhatgiaban = function (item) {
        self.MangNhomDV([]);
        self.MangNhomNB([]);
        self.MangNhomNKH([]);
        self.MangNhomNgayTrongTuan([]);
        self.MangNghiepVuAD([]);
        self._ThemMoiGiaBan(false);

        $('#selec-all-NKH li').each(function () {
            $(this).find('.fa-check').remove();
        });
        $('#selec-all-NB li').each(function () {
            $(this).find('.fa-check').remove();
        });
        $('#selec-all-DV li').each(function () {
            $(this).find('.fa-check').remove();
        });

        //if ($.inArray('ThietLapGia_CapNhat', lc_CTQuyen) > -1) {
        $('#btnGiaBan').css('display', 'inline-block');
        $('.modal-title').text('Sửa bảng giá')
        ajaxHelper(GiaBanUri + "GetGiaBan_ApDung/" + self.selectedGiaBan(), 'GET').done(function (data) {
            if (data[0].TatCaDonVi == true) {
                $('.outselectDV').addClass('open1');
            } else {
                $('.outselectDV').removeClass('open1');
            }
            if (data[0].TatCaNhanVien == true) {
                $('.outselectNB').addClass('open1');
            } else {
                $('.outselectNB').removeClass('open1');
            }
            if (data[0].TatCaDoiTuong == true) {
                $('.outselectNKH').addClass('open1');
            } else {
                $('.outselectNKH').removeClass('open1');
            }
            self.ID_GiaBanAD(data[0].ID);
            self.newGiaBan().SetData(data[0]);
            var ChuoiNgay = data[0].NgayTrongTuan.split(",");
            for (var i = 0; i < ChuoiNgay.length; i++) {
                for (var j = 0; j < self.NgayTrongTuans().length; j++) {
                    if (ChuoiNgay[i] === self.NgayTrongTuans()[j].value) {
                        self.MangNhomNgayTrongTuan.push(self.NgayTrongTuans()[j]);
                    }
                }
            }
            var ChuoiPTApDung = data[0].LoaiChungTuApDung.split(",");
            for (var i = 0; i < ChuoiPTApDung.length; i++) {
                for (var j = 0; j < self.NghiepVuADs().length; j++) {
                    if (ChuoiPTApDung[i] === self.NghiepVuADs()[j].value) {
                        self.MangNghiepVuAD.push(self.NghiepVuADs()[j]);
                    }
                }
            }
            if (self.MangNhomNgayTrongTuan().length === 7) {
                $('#selec-all-Ngay li').each(function () {
                    $(this).find('.fa-check').remove();
                });
                self.MangNhomNgayTrongTuan([]);
            }
            if (self.MangNghiepVuAD().length === 3) {
                $('#selec-all-NV li').each(function () {
                    $(this).find('.fa-check').remove();
                });
                self.MangNghiepVuAD([]);
            }
            if (self.MangNhomNgayTrongTuan().length > 0) {
                for (var i = 0; i < self.MangNhomNgayTrongTuan().length; i++) {
                    $('#selec-all-Ngay li').each(function () {
                        if ($(this).attr('id') === self.MangNhomNgayTrongTuan()[i].TenNgay) {
                            $(this).find('.fa-check').remove();
                            $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                        }
                    });
                }
                $('#choose_Ngay input').remove();
            }
            else {
                $('#choose_Ngay input').remove();
                $('#choose_Ngay').append('<input type="text" id="dllNgayTrongTuan" readonly="readonly" class="dropdown" placeholder="--- Tất cả ---">');
                $('#selec-all-Ngay li').each(function () {
                    $(this).find('.fa-check').remove();
                });
            }
            if (self.MangNghiepVuAD().length > 0) {
                for (var i = 0; i < self.MangNghiepVuAD().length; i++) {
                    $('#selec-all-NV li').each(function () {
                        if ($(this).attr('id') === self.MangNghiepVuAD()[i].value) {
                            $(this).find('.fa-check').remove();
                            $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                        }
                    });
                }
                $('#choose_NghiepVu input').remove();
            }
            else {
                $('#choose_NghiepVu input').remove();
                $('#choose_NghiepVu').append('<input type="text" id="dllNghiepVu" readonly="readonly" class="dropdown" placeholder="--- Tất cả ---">');
                $('#selec-all-NV li').each(function () {
                    $(this).find('.fa-check').remove();
                });
            }
        });
        //get list nhom donvi bang giá
        ajaxHelper(GiaBanUri + "getLisDonViGB?id_giaban=" + self.selectedGiaBan(), "GET").done(function (data) {
            self.MangNhomDV(data);
            if (self.MangNhomDV().length > 0) {
                $('#choose_TenDonVi').attr("data-toggle", "dropdown");
                $('#choose_TenDonVi input').remove();
                for (var i = 0; i < self.MangNhomDV().length; i++) {
                    $('#selec-all-DV li').each(function () {
                        if ($(this).attr('id') === self.MangNhomDV()[i].ID) {
                            $(this).find('.fa-check').remove();
                            $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                        }
                    });
                }
            }
            else {
                $('#choose_TenDonVi input').remove();
                self.MangNhomDV([]);
                $('#choose_TenDonVi').append('<input type="text" id="dllChiNhanh" readonly="readonly" class="dropdown" placeholder="--- Tất cả ---">');
                $('#selec-all-DV li').each(function () {
                    $(this).find('.fa-check').remove();
                });
            }

        });

        //getlistnhom nhanvien bảng giá
        ajaxHelper(GiaBanUri + "getlistNhanVienBG?id_giaban=" + self.selectedGiaBan(), "GET").done(function (data) {
            self.MangNhomNB(data);
            if (self.MangNhomNB().length > 0) {
                $('#choose-NguoiBan').attr("data-toggle", "dropdown");
                $('#choose-NguoiBan input').remove();
                for (var i = 0; i < self.MangNhomNB().length; i++) {
                    $('#selec-all-NB li').each(function () {
                        if ($(this).attr('id') === self.MangNhomNB()[i].ID) {
                            $(this).find('.fa-check').remove();
                            $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                        }
                    });
                }
            }
            else {
                $('#choose-NguoiBan input').remove();
                self.MangNhomNB([]);
                $('#choose-NguoiBan').append('<input type="text" id="dllNguoiBan" readonly="readonly" class="dropdown " placeholder="--- Tất cả ---">');
                $('#selec-all-NB li').each(function () {
                    $(this).find('.fa-check').remove();
                });
            }
        });

        //getlist nhomkhachhang bảng giá
        ajaxHelper(GiaBanUri + "getlistNhomKHangBG?id_giaban=" + self.selectedGiaBan(), "GET").done(function (data) {
            self.MangNhomNKH(data);
            if (self.MangNhomNKH().length > 0) {
                $('#choose-NhomKhachHang').attr("data-toggle", "dropdown");
                $('#choose-NhomKhachHang input').remove();
                for (var i = 0; i < self.MangNhomNKH().length; i++) {
                    $('#selec-all-NKH li').each(function () {
                        if ($(this).attr('id') === self.MangNhomNKH()[i].ID) {
                            $(this).find('.fa-check').remove();
                            $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                        }
                    });
                }
            }
            else {
                $('#choose-NhomKhachHang input').remove();
                self.MangNhomNKH([]);
                $('#choose-NhomKhachHang').append('<input type="text" id="dllNhomKH" readonly="readonly" class="dropdown " placeholder="--- Tất cả ---">');
                $('#selec-all-NKH li').each(function () {
                    $(this).find('.fa-check').remove();
                });
            }
        });


        $('#modalPopuplg_giaban').modal('show');
        $('#modalPopuplg_giaban').on('shown.bs.modal', function () {
            var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
            if ($.inArray('ThietLapGia_CapNhat', lc_CTQuyen) > -1) {
                $('.editGiaBan').css('display', 'inline-block')
            }
            else {
                $('.editGiaBan').hide();
            }
        })
        $('.btnxoa').css('display', 'block');
        $('.updateHHTab li').each(function () {
            $(this).removeClass('active');
        })
        $("a[href='#thongtin']").parent('li').addClass('active');
        $('#thongtin').addClass('active');
        $('#phamviapdung').removeClass('active');
        //}
    }

    self.getChiTietGiaBan = function () {
        self._TenBangGia(self.selectedGiaBanName() === "" ? "Bảng giá chuẩn" : self.selectedGiaBanName());
        self.currentPage(0);
        SearchHangHoa();
        $(".li-oo").removeClass("yellow");
    };
    //self.getChiTietGiaBan();

    //add to database
    self.arrGiaBanApDung = ko.observableArray();
    self.addGiaBan = function (formElement) {
        document.getElementById("btnLuuGiaBan").disabled = true;
        document.getElementById("btnLuuGiaBan").lastChild.data = " Đang lưu";
        var _idgiaban = self.newGiaBan().ID();
        var _tengiaban = self.newGiaBan().TenGiaBan();
        var _apdung = self.newGiaBan().ApDung();
        var _tungay = $('#txtTuNgay').val();
        var _denngay = $('#txtDenNgay').val();
        var _ghichu = self.newGiaBan().GhiChu();
        var _tatcadonvi = self.newGiaBan().TatCaDonVi();
        var _tatcadoituong = self.newGiaBan().TatCaDoiTuong();
        var _tatcanhanvien = self.newGiaBan().TatCaNhanVien();
        if (_tatcadonvi == false && self.MangNhomDV().length == 0) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng chọn phạm vi theo chi nhánh", "danger");
            $('#dllChiNhanh').focus();
            document.getElementById("btnLuuGiaBan").disabled = false;
            document.getElementById("btnLuuGiaBan").lastChild.data = " Lưu";
            return false;
        }

        if (_tatcanhanvien == false && self.MangNhomNB().length == 0) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng chọn phạm vi theo người bán", "danger");
            $('#dllNguoiBan').focus();
            document.getElementById("btnLuuGiaBan").disabled = false;
            document.getElementById("btnLuuGiaBan").lastChild.data = " Lưu";
            return false;
        }

        if (_tatcadoituong == false && self.MangNhomNKH().length == 0) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng chọn phạm vi theo khách hàng", "danger");
            $('#dllNhomKH').focus();
            document.getElementById("btnLuuGiaBan").disabled = false;
            document.getElementById("btnLuuGiaBan").lastChild.data = " Lưu";
            return false;
        }

        if (_tengiaban === undefined) {

            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng nhập tên bảng giá", "danger");
            document.getElementById("txtTenBangGia").focus();
            document.getElementById("btnLuuGiaBan").disabled = false;
            document.getElementById("btnLuuGiaBan").lastChild.data = " Lưu";
            return false;
        }
        else {
            if (_tengiaban.toString().trim() === "") {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Tên bảng giá không được để trống!", "danger");
                document.getElementById("txtTenBangGia").focus();
                document.getElementById("btnLuuGiaBan").disabled = false;
                document.getElementById("btnLuuGiaBan").lastChild.data = " Lưu";
                return false;
            }
        }
        if (moment(_tungay, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') > moment(_denngay, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm')) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Thời gian Kết thúc phải lớn hơn thời gian bắt đầu", "danger");
            document.getElementById("btnLuuGiaBan").disabled = false;
            document.getElementById("btnLuuGiaBan").lastChild.data = " Lưu";
            return false;
        }

        var ChuoiNgayTT = "";
        if (self.MangNhomNgayTrongTuan().length > 0) {
            for (var i = 0; i < self.MangNhomNgayTrongTuan().length; i++) {
                ChuoiNgayTT = ChuoiNgayTT + "," + self.MangNhomNgayTrongTuan()[i].value;
            }
        }
        else {
            ChuoiNgayTT = "1,2,3,4,5,6,7";
        }

        var loaiChungTuAD = "";
        if (self.MangNghiepVuAD().length > 0) {
            for (var i = 0; i < self.MangNghiepVuAD().length; i++) {
                loaiChungTuAD = loaiChungTuAD + "," + self.MangNghiepVuAD()[i].value;
            }
        }
        else {
            loaiChungTuAD = "1,3,6,19";
        }

        var gb = {
            ID: _idgiaban,
            TenGiaBan: _tengiaban,
            ApDung: _apdung,
            TuNgay: moment(_tungay, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm'),
            DenNgay: moment(_denngay, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm'),
            GhiChu: _ghichu,
            TatCaDonVi: _tatcadonvi,
            TatCaDoiTuong: _tatcadoituong,
            TatCaNhanVien: _tatcanhanvien,
            NgayTrongTuan: ChuoiNgayTT,
            LoaiChungTuApDung: loaiChungTuAD
        };
        self.arrGiaBanApDung([]);
        if (self.MangNhomDV().length > 0 && self.MangNhomNB().length > 0 && self.MangNhomNKH().length > 0) {
            for (var i = 0; i < self.MangNhomDV().length; i++) {
                for (var j = 0; j < self.MangNhomNB().length; j++) {
                    for (var k = 0; k < self.MangNhomNKH().length; k++) {
                        var objParent = {
                            ID_DonVi: self.MangNhomDV()[i].ID,
                            ID_NhanVien: self.MangNhomNB()[j].ID,
                            ID_NhomKhachHang: self.MangNhomNKH()[k].ID,
                        }
                        self.arrGiaBanApDung.push(objParent);
                    }
                }
            }
        }

        if (self.MangNhomDV().length > 0 && self.MangNhomNB().length == 0 && self.MangNhomNKH().length == 0) {
            for (var i = 0; i < self.MangNhomDV().length; i++) {
                var objParent = {
                    ID_DonVi: self.MangNhomDV()[i].ID,
                    ID_NhanVien: null,
                    ID_NhomKhachHang: null,
                }
                self.arrGiaBanApDung.push(objParent);
            }
        }

        if (self.MangNhomDV().length == 0 && self.MangNhomNB().length > 0 && self.MangNhomNKH().length == 0) {
            for (var i = 0; i < self.MangNhomNB().length; i++) {
                var objParent = {
                    ID_DonVi: null,
                    ID_NhanVien: self.MangNhomNB()[i].ID,
                    ID_NhomKhachHang: null,
                }
                self.arrGiaBanApDung.push(objParent);
            }
        }

        if (self.MangNhomDV().length == 0 && self.MangNhomNB().length == 0 && self.MangNhomNKH().length > 0) {
            for (var i = 0; i < self.MangNhomNKH().length; i++) {
                var objParent = {
                    ID_DonVi: null,
                    ID_NhanVien: null,
                    ID_NhomKhachHang: self.MangNhomNKH()[i].ID,
                }
                self.arrGiaBanApDung.push(objParent);
            }
        }

        if (self.MangNhomDV().length > 0 && self.MangNhomNB().length > 0 && self.MangNhomNKH().length == 0) {
            for (var i = 0; i < self.MangNhomDV().length; i++) {
                for (var j = 0; j < self.MangNhomNB().length; j++) {
                    var objParent = {
                        ID_DonVi: self.MangNhomDV()[i].ID,
                        ID_NhanVien: self.MangNhomNB()[j].ID,
                        ID_NhomKhachHang: null,
                    }
                    self.arrGiaBanApDung.push(objParent);
                }
            }
        }

        if (self.MangNhomDV().length > 0 && self.MangNhomNB().length == 0 && self.MangNhomNKH().length > 0) {
            for (var i = 0; i < self.MangNhomDV().length; i++) {
                for (var j = 0; j < self.MangNhomNKH().length; j++) {
                    var objParent = {
                        ID_DonVi: self.MangNhomDV()[i].ID,
                        ID_NhanVien: null,
                        ID_NhomKhachHang: self.MangNhomNKH()[j].ID,
                    }
                    self.arrGiaBanApDung.push(objParent);
                }
            }
        }

        if (self.MangNhomDV().length == 0 && self.MangNhomNB().length > 0 && self.MangNhomNKH().length > 0) {
            for (var i = 0; i < self.MangNhomNB().length; i++) {
                for (var j = 0; j < self.MangNhomNKH().length; j++) {
                    var objParent = {
                        ID_DonVi: null,
                        ID_NhanVien: self.MangNhomNB()[i].ID,
                        ID_NhomKhachHang: self.MangNhomNKH()[j].ID,
                    }
                    self.arrGiaBanApDung.push(objParent);
                }
            }
        }

        if (self.MangNhomDV().length == 0 && self.MangNhomNB().length == 0 && self.MangNhomNKH().length == 0) {
            self.arrGiaBanApDung([]);
        }
        var mydata = {};
        mydata.objNewGiaBan = gb;
        mydata.objGiaBanApDung = self.arrGiaBanApDung();

        if (self._ThemMoiGiaBan() === true) {
            $.ajax({
                url: GiaBanUri + "PostGiaBan",
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: mydata,
                success: function (item) {
                    self.GiaBans.push(item);
                    self.selectedGiaBan(item.ID);
                    self.ThemMoiBangGiaLS(item.TenGiaBan);
                    SearchHangHoa();
                    $(".seach-price").css("display", "block");
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Thêm mới bảng giá thành công", "success");
                    document.getElementById("btnLuuGiaBan").disabled = false;
                    document.getElementById("btnLuuGiaBan").lastChild.data = " Lưu";
                    $('#modalPopuplg_giaban').modal('hide');
                },
                statusCode: {
                    404: function () {
                        document.getElementById("btnLuuGiaBan").disabled = false;
                        document.getElementById("btnLuuGiaBan").lastChild.data = " Lưu";
                        self.error("Page not found");
                    }
                },
                error: function (jqXHR, textStatus, errorThrow) {
                    document.getElementById("btnLuuGiaBan").disabled = false;
                    document.getElementById("btnLuuGiaBan").lastChild.data = " Lưu";
                    self.error(textStatus + ": " + errorThrow + ": " + jqXHR.responseText);
                    bottomrightnotify(jqXHR.responseText.replace(/"/g, ''), "danger");
                },
                complete: function () {

                }
            })
                .fail(function (jqXHR, textStatus, errorThrow) {
                    document.getElementById("btnLuuGiaBan").disabled = false;
                    document.getElementById("btnLuuGiaBan").lastChild.data = " Lưu";
                    self.error(textStatus + ": " + errorThrow + ": " + jqXHR.responseText);
                });
        }
    };

    self.editGiaBan = function (formElement) {
        document.getElementById("btnLuuGiaBan").disabled = true;
        document.getElementById("btnLuuGiaBan").lastChild.data = " Đang lưu";
        var _idgiaban = self.newGiaBan().ID();
        var _tengiaban = self.newGiaBan().TenGiaBan();
        var _apdung = self.newGiaBan().ApDung();
        var _tungay = $('#txtTuNgay').val();
        var _denngay = $('#txtDenNgay').val();
        var _ghichu = self.newGiaBan().GhiChu();
        var _tatcadonvi = self.newGiaBan().TatCaDonVi();
        var _tatcadoituong = self.newGiaBan().TatCaDoiTuong();
        var _tatcanhanvien = self.newGiaBan().TatCaNhanVien();
        if (_tatcadonvi == false && self.MangNhomDV().length == 0) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng chọn phạm vi theo chi nhánh", "danger");
            $('#dllChiNhanh').focus();
            document.getElementById("btnLuuGiaBan").disabled = false;
            document.getElementById("btnLuuGiaBan").lastChild.data = " Lưu";
            return false;
        }

        if (_tatcanhanvien == false && self.MangNhomNB().length == 0) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng chọn phạm vi theo người bán", "danger");
            $('#dllNguoiBan').focus();
            document.getElementById("btnLuuGiaBan").disabled = false;
            document.getElementById("btnLuuGiaBan").lastChild.data = " Lưu";
            return false;
        }

        if (_tatcadoituong == false && self.MangNhomNKH().length == 0) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng chọn phạm vi theo khách hàng", "danger");
            $('#dllNhomKH').focus();
            document.getElementById("btnLuuGiaBan").disabled = false;
            document.getElementById("btnLuuGiaBan").lastChild.data = " Lưu";
            return false;
        }

        if (_tengiaban === undefined) {

            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng nhập tên bảng giá", "danger");
            document.getElementById("txtTenBangGia").focus();
            document.getElementById("btnLuuGiaBan").disabled = false;
            document.getElementById("btnLuuGiaBan").lastChild.data = " Lưu";
            return false;
        }
        else {
            if (_tengiaban.toString().trim() === "") {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng nhập tên bảng giá", "danger");
                document.getElementById("txtTenBangGia").focus();
                document.getElementById("btnLuuGiaBan").disabled = false;
                document.getElementById("btnLuuGiaBan").lastChild.data = " Lưu";
                return false;
            }
        }
        if (moment(_tungay, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') > moment(_denngay, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm')) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Thời gian Kết thúc phải lớn hơn thời gian bắt đầu", "danger");
            document.getElementById("btnLuuGiaBan").disabled = false;
            document.getElementById("btnLuuGiaBan").lastChild.data = " Lưu";
            return false;
        }

        var ChuoiNgayTT = "";
        if (self.MangNhomNgayTrongTuan().length > 0) {
            for (var i = 0; i < self.MangNhomNgayTrongTuan().length; i++) {
                ChuoiNgayTT = ChuoiNgayTT + "," + self.MangNhomNgayTrongTuan()[i].value;
            }
        }
        else {
            ChuoiNgayTT = "1,2,3,4,5,6,7";
        }

        var loaiChungTuAD = "";
        if (self.MangNghiepVuAD().length > 0) {
            for (var i = 0; i < self.MangNghiepVuAD().length; i++) {
                loaiChungTuAD = loaiChungTuAD + "," + self.MangNghiepVuAD()[i].value;
            }
        }
        else {
            loaiChungTuAD = "1,3,6";
        }

        var gb = {
            ID: _idgiaban,
            TenGiaBan: _tengiaban,
            ApDung: _apdung,
            TuNgay: moment(_tungay, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm'),
            DenNgay: moment(_denngay, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm'),
            GhiChu: _ghichu,
            TatCaDonVi: _tatcadonvi,
            TatCaDoiTuong: _tatcadoituong,
            TatCaNhanVien: _tatcanhanvien,
            NgayTrongTuan: ChuoiNgayTT,
            LoaiChungTuApDung: loaiChungTuAD
        };
        self.arrGiaBanApDung([]);
        if (self.MangNhomDV().length > 0 && self.MangNhomNB().length > 0 && self.MangNhomNKH().length > 0) {
            for (var i = 0; i < self.MangNhomDV().length; i++) {
                for (var j = 0; j < self.MangNhomNB().length; j++) {
                    for (var k = 0; k < self.MangNhomNKH().length; k++) {
                        var objParent = {
                            ID_DonVi: self.MangNhomDV()[i].ID,
                            ID_NhanVien: self.MangNhomNB()[j].ID,
                            ID_NhomKhachHang: self.MangNhomNKH()[k].ID,
                        }
                        self.arrGiaBanApDung.push(objParent);
                    }
                }
            }
        }

        if (self.MangNhomDV().length > 0 && self.MangNhomNB().length == 0 && self.MangNhomNKH().length == 0) {
            for (var i = 0; i < self.MangNhomDV().length; i++) {
                var objParent = {
                    ID_DonVi: self.MangNhomDV()[i].ID,
                    ID_NhanVien: null,
                    ID_NhomKhachHang: null,
                }
                self.arrGiaBanApDung.push(objParent);
            }
        }

        if (self.MangNhomDV().length == 0 && self.MangNhomNB().length > 0 && self.MangNhomNKH().length == 0) {
            for (var i = 0; i < self.MangNhomNB().length; i++) {
                var objParent = {
                    ID_DonVi: null,
                    ID_NhanVien: self.MangNhomNB()[i].ID,
                    ID_NhomKhachHang: null,
                }
                self.arrGiaBanApDung.push(objParent);
            }
        }

        if (self.MangNhomDV().length == 0 && self.MangNhomNB().length == 0 && self.MangNhomNKH().length > 0) {
            for (var i = 0; i < self.MangNhomNKH().length; i++) {
                var objParent = {
                    ID_DonVi: null,
                    ID_NhanVien: null,
                    ID_NhomKhachHang: self.MangNhomNKH()[i].ID,
                }
                self.arrGiaBanApDung.push(objParent);
            }
        }

        if (self.MangNhomDV().length > 0 && self.MangNhomNB().length > 0 && self.MangNhomNKH().length == 0) {
            for (var i = 0; i < self.MangNhomDV().length; i++) {
                for (var j = 0; j < self.MangNhomNB().length; j++) {
                    var objParent = {
                        ID_DonVi: self.MangNhomDV()[i].ID,
                        ID_NhanVien: self.MangNhomNB()[j].ID,
                        ID_NhomKhachHang: null,
                    }
                    self.arrGiaBanApDung.push(objParent);
                }
            }
        }

        if (self.MangNhomDV().length > 0 && self.MangNhomNB().length == 0 && self.MangNhomNKH().length > 0) {
            for (var i = 0; i < self.MangNhomDV().length; i++) {
                for (var j = 0; j < self.MangNhomNKH().length; j++) {
                    var objParent = {
                        ID_DonVi: self.MangNhomDV()[i].ID,
                        ID_NhanVien: null,
                        ID_NhomKhachHang: self.MangNhomNKH()[j].ID,
                    }
                    self.arrGiaBanApDung.push(objParent);
                }
            }
        }

        if (self.MangNhomDV().length == 0 && self.MangNhomNB().length > 0 && self.MangNhomNKH().length > 0) {
            for (var i = 0; i < self.MangNhomNB().length; i++) {
                for (var j = 0; j < self.MangNhomNKH().length; j++) {
                    var objParent = {
                        ID_DonVi: null,
                        ID_NhanVien: self.MangNhomNB()[i].ID,
                        ID_NhomKhachHang: self.MangNhomNKH()[j].ID,
                    }
                    self.arrGiaBanApDung.push(objParent);
                }
            }
        }

        if (self.MangNhomDV().length == 0 && self.MangNhomNB().length == 0 && self.MangNhomNKH().length == 0) {
            self.arrGiaBanApDung([]);
        }
        var mydata = {};
        mydata.objNewGiaBan = gb;
        mydata.objGiaBanApDung = self.arrGiaBanApDung();
        if (self._ThemMoiGiaBan() === false) {
            $.ajax({
                url: GiaBanUri + "PutGiaBan",
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: mydata,
                success: function (item) {
                    getAllGiaBan();
                    self._TenBangGia(item.TenGiaBan);
                    self.selectedGiaBan(item.ID);

                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật bảng giá thành công", "success");
                    $('#modalPopuplg_giaban').modal('hide');
                    document.getElementById("btnLuuGiaBan").disabled = false;
                    document.getElementById("btnLuuGiaBan").lastChild.data = " Lưu";

                    let diary = {
                        ID_NhanVien: _id_NhanVien,
                        ID_DonVi: _IDchinhanh,
                        ChucNang: "Thiết lập giá",
                        NoiDung: "Cập nhật bảng giá : " + item.TenGiaBan,
                        NoiDungChiTiet: "Cập nhật bảng giá : ".concat(item.TenGiaBan, ', Người sửa: ', _userLogin),
                        LoaiNhatKy: 2 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
                    };
                    Insert_NhatKyThaoTac_1Param(diary);
                },
                statusCode: {
                    404: function () {
                        document.getElementById("btnLuuGiaBan").disabled = false;
                        document.getElementById("btnLuuGiaBan").lastChild.data = " Lưu";
                        self.error("Page not found");
                    }
                },
                error: function (jqXHR, textStatus, errorThrow) {
                    document.getElementById("btnLuuGiaBan").disabled = false;
                    document.getElementById("btnLuuGiaBan").lastChild.data = " Lưu";
                    self.error(textStatus + ": " + errorThrow + ": " + jqXHR.responseText);
                },
                complete: function () {
                }
            })
                .fail(function (jqXHR, textStatus, errorThrow) {
                    document.getElementById("btnLuuGiaBan").disabled = false;
                    document.getElementById("btnLuuGiaBan").lastChild.data = " Lưu";
                    self.error(textStatus + ": " + errorThrow + ": " + jqXHR.responseText);
                });
        }
    };

    self.ThemMoiBangGiaLS = function (tenbanggia) {
        var objDiary = {
            ID_NhanVien: $('.idnhanvien').text(),
            ID_DonVi: _IDchinhanh,
            ChucNang: "Thiết lập giá",
            NoiDung: "Thêm mới bảng giá : " + tenbanggia,
            NoiDungChiTiet: "Thêm mới bảng giá : " + tenbanggia,
            LoaiNhatKy: 1 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        var myData = {};
        myData.objDiary = objDiary;
        $.ajax({
            url: '/api/DanhMuc/SaveDiary/' + "post_NhatKySuDung",
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

            }
        })
    }

    self.addAndNewGiaBan = function () { };

    self.addChiTietGiaBan = function (item) {
        var idBangGia = self.selectedGiaBan();
        if (idBangGia === undefined) {
            idBangGia = '00000000-0000-0000-0000-000000000000';
        }
        self.changeddlNhomHangHoaAddChiTiet(item);
        ajaxHelper(GiaBanUri + "AddChiTiet?idnhomhanghoa=" + self.arrIDNhomHang() + "&idgiaban=" + idBangGia, 'GET').done(function (data) {
            if (data === "") {
                self.arrIDNhomHang([]);
                getAllGiaBanChiTiet();
                //$('#wait').remove();
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật dữ liệu thành công!", "success");
            }
            else {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Nhóm đã tồn tại trong bảng giá chuẩn", "danger");
            }
        })
    };


    self.changeddlNhomHangHoaAddChiTiet = function (item) {
        var arrIDChilds = [];
        var lcNhomHH = localStorage.getItem('lc_NhomHangHoas');
        if (lcNhomHH !== null) {
            lcNhomHH = JSON.parse(lcNhomHH);
            for (var i = 0; i < lcNhomHH.length; i++) {
                if (lcNhomHH[i].ID === item.ID) {
                    for (var j = 0; j < lcNhomHH.length; j++) {
                        if (lcNhomHH[j].ID_Parent === item.ID) {
                            // get ID_Child level 1
                            arrIDChilds.push(lcNhomHH[j].ID);
                            for (var k = 0; k < lcNhomHH.length; k++) {
                                if (lcNhomHH[k].ID_Parent === lcNhomHH[j].ID) {
                                    // get ID_Child level 2
                                    arrIDChilds.push(lcNhomHH[k].ID);
                                }
                            }
                        }
                    }
                    arrIDChilds.push(lcNhomHH[i].ID);
                    break;
                }
            }
        }
        self.arrIDNhomHang(arrIDChilds);
    }
    //self.selectedNKH.subscribe(function (newValue) {
    //})

    self.ID_GiaBanAD.subscribe(function (newValue) {
    })

    //self.selectedCN.subscribe(function (newValue) {
    //})

    //self.selectedNB.subscribe(function (newValue) {
    //})

    self.selectedHH.subscribe(function (newValue) {
        if (newValue === undefined) {
            self.getChiTietGiaBan(null);
        }
        else {
            $.ajax({
                type: "GET",
                url: GiaBanUri + "AddChiTietHang?iddonviqd=" + newValue + "&idgiaban=" + self.selectedGiaBan(),
                success: function (result) {
                    if (result === true) {
                        self.getChiTietGiaBan();
                        $('#txtAutoHangHoa').focus().select();
                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật hàng hóa thành công", "success");
                    }
                    else {
                        $('#txtAutoHangHoa').focus().select();
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Sản phẩm đã tồn tại trong bảng giá", "danger");
                    }
                }
            });
        }
    });


    self.modalDelete = function (item) {
        self.deleteTenHangHoa(item.TenHangHoa);
        self.deleteID(item.ID);
        $('#modalpopup_delete').modal('show');
    };

    self.modalDeleteAllChiTiet = function (item) {
        $('#modalpopup_deleteAllCT').modal('show');
    };

    self.deleteChiTietGiaBan = function () {
        $.ajax({
            type: "DELETE",
            url: GiaBanUri + "DeleteAChiTietGiaBan/" + self.deleteID(),
            success: function (result) {
                if (result === undefined) {
                    self.getChiTietGiaBan();
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật hàng hóa thành công", "success");
                }
                else {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cập nhật hàng hóa thất bại", "danger");
                }
            }
        })
    };

    //getctgiaban by nhom hàng
    self.arrIDNhomHang = ko.observableArray();


    self.changeddlNhomHangHoa = function (item) {
        $("#iconSort").remove();
        self.columsort(null);
        self.sort(null);
        $('.table_h').gridLoader();
        $('.ss-li .li-oo').removeClass("yellow");

        $('#tatcanhh').removeClass("yellow")
        $('.li-pp').removeClass("yellow");

        $('#tatcanhh a').css("display", "none");
        $('#' + item.ID).addClass("yellow")
        if (item.ID === undefined) {
            getAllGiaBanChiTiet(item.ID);
        } else {
            var idBangGia = self.selectedGiaBan();
            if (idBangGia === undefined) {
                idBangGia = '00000000-0000-0000-0000-000000000000';
            }
            var arrIDChilds = [];
            var lcNhomHH = localStorage.getItem('lc_NhomHangHoas');
            if (lcNhomHH !== null) {
                lcNhomHH = JSON.parse(lcNhomHH);
                for (var i = 0; i < lcNhomHH.length; i++) {
                    if (lcNhomHH[i].ID === item.ID) {
                        for (var j = 0; j < lcNhomHH.length; j++) {
                            if (lcNhomHH[j].ID_Parent === item.ID) {
                                // get ID_Child level 1
                                arrIDChilds.push(lcNhomHH[j].ID);
                                for (var k = 0; k < lcNhomHH.length; k++) {
                                    if (lcNhomHH[k].ID_Parent === lcNhomHH[j].ID) {
                                        // get ID_Child level 2
                                        arrIDChilds.push(lcNhomHH[k].ID);
                                    }
                                }
                            }
                        }
                        arrIDChilds.push(lcNhomHH[i].ID);
                        break;
                    }
                }
            }
            self.arrIDNhomHang(arrIDChilds);
            self.currentPage(0);
            SearchHangHoa();
        }
    }

    self.popoverEditGia = function (item) {
        $("[data-toggle=popover]").popover({
            container: 'body',
            html: true,
            content: function () {
                return $('#popover-content').html();
            },
            placement: 'left'
        });
    };
    self.allChiTiets = ko.observableArray();

    self.ApDungGiaBan = function (item) {
        var c = document.getElementById('choose-price_' + rowid).value;
        var _id = this.ID;
        var objUpdate = [];
        var mydata = {};
        var applyAll = $('#cbapdung_' + rowid).is(":checked");
        var isSub = $("#plus_" + rowid).hasClass('active-re');// trừ
        var isPTram = $('#donViTinh_' + rowid).hasClass('active-re');
        var tenBangGia = self.selectedGiaBanName();
        var chitiet = '';

        switch (parseInt(c)) {
            case 0:
                chitiet = ' Giá hiện tại';
                break;
            case 1:
                chitiet = ' Giá chung';
                break;
            case 2:
                chitiet = ' Giá cuối';
                break;
            case 3:
                chitiet = ' Giá vốn';
                break;
        }

        if (tenBangGia === "") {
            if (applyAll && isSub == false) {
                if (isPTram == false) {
                    chitiet = chitiet.concat(' + ', formatNumber(priceAdd), ' VND');

                    ajaxHelper(GiaBanUri + 'PutGiaBanChiTietChungCongVND?LoaiGiaChon=' + c + '&giaTri=' + priceAdd + '&idnhomhang=' + self.arrIDNhomHang() + '&iddonvi=' + _IDchinhanh, 'POST').done(function (data) {
                        SearchHangHoa();
                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật bảng giá thành công", "success");
                    });
                }
                if (isPTram == true) {
                    chitiet = chitiet.concat(' + ', phantram, ' %');
                    ajaxHelper(GiaBanUri + 'PutGiaBanChiTietChungCongPhanTram?LoaiGiaChon=' + c + '&giaTri=' + phantram + '&idnhomhang=' + self.arrIDNhomHang() + '&iddonvi=' + _IDchinhanh, 'POST').done(function (data) {
                        SearchHangHoa();
                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật bảng giá thành công", "success");
                    });
                }
            } else {
                if (applyAll) {
                    if (isPTram === false) {
                        chitiet = chitiet.concat(' - ', formatNumber(priceAdd), ' VND');
                        ajaxHelper(GiaBanUri + 'PutGiaBanChiTietChungTruVND?LoaiGiaChon=' + c + '&giaTri=' + priceAdd + '&idnhomhang=' + self.arrIDNhomHang() + '&iddonvi=' + _IDchinhanh, 'POST').done(function (data) {
                            SearchHangHoa();
                            bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật bảng giá thành công", "success");
                        });
                    }
                    if (isPTram === true) {
                        chitiet = chitiet.concat(' - ', phantram, ' %');
                        ajaxHelper(GiaBanUri + 'PutGiaBanChiTietChungTruPhanTram?LoaiGiaChon=' + c + '&giaTri=' + phantram + '&idnhomhang=' + self.arrIDNhomHang() + '&iddonvi=' + _IDchinhanh, 'POST').done(function (data) {
                            SearchHangHoa();
                            bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật bảng giá thành công", "success");
                        });
                    }
                }
                else {
                    if (isPTram == false) {
                        chitiet = chitiet.concat(' + ', formatNumber(priceAdd), ' VND');
                    }
                    if (isPTram === true) {
                        chitiet = chitiet.concat(' - ', phantram, ' %');
                    }
                    var objUpdatetemp = {
                        ID: _id,
                        GiaBan: giamoi
                    };
                    objUpdate.push(objUpdatetemp);
                    mydata.ID = _id;
                    mydata.objData = objUpdate;
                    $.ajax({
                        url: GiaBanUri + "PutGiaBanChiTietChung",
                        type: 'POST',
                        dataType: 'json',
                        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                        data: mydata,
                        success: function (result) {
                            bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật bảng giá thành công", "success");
                            SearchHangHoa();
                        },
                        error: function (error) {
                            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cập nhật bảng giá thất bại", "danger");
                            $('#modalpopup_deleteGB').modal('hide');
                        }
                    })
                }
            }
        }
        else {
            if (applyAll && isSub == false) {
                if (isPTram == false) {
                    chitiet = chitiet.concat(' + ', formatNumber(priceAdd), ' VND');
                    ajaxHelper(GiaBanUri + 'PutGiaBanChiTietCongVND?LoaiGiaChon=' + c + '&giaTri=' + priceAdd + '&id_giaban=' + self.selectedGiaBan() + '&idnhomhang=' + self.arrIDNhomHang() + '&iddonvi=' + _IDchinhanh, 'POST').done(function (data) {
                        SearchHangHoa();
                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật bảng giá thành công", "success");
                    });
                }
                if (isPTram == true) {
                    chitiet = chitiet.concat(' + ', phantram, ' %');
                    ajaxHelper(GiaBanUri + 'PutGiaBanChiTietCongPhanTram?LoaiGiaChon=' + c + '&giaTri=' + phantram + '&id_giaban=' + self.selectedGiaBan() + '&idnhomhang=' + self.arrIDNhomHang() + '&iddonvi=' + _IDchinhanh, 'POST').done(function (data) {
                        SearchHangHoa();
                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật bảng giá thành công", "success");
                    });
                }
            } else {
                if (applyAll) {
                    if (isPTram == false) {
                        chitiet = chitiet.concat(' - ', formatNumber(priceAdd), ' VND');
                        ajaxHelper(GiaBanUri + 'PutGiaBanChiTietTruVND?LoaiGiaChon=' + c + '&giaTri=' + priceAdd + '&id_giaban=' + self.selectedGiaBan() + '&idnhomhang=' + self.arrIDNhomHang() + '&iddonvi=' + _IDchinhanh, 'POST').done(function (data) {
                            SearchHangHoa();
                            bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật bảng giá thành công", "success");
                        });
                    }
                    if (isPTram == true) {
                        chitiet = chitiet.concat(' - ', phantram, ' %');
                        ajaxHelper(GiaBanUri + 'PutGiaBanChiTietTruPhanTram?LoaiGiaChon=' + c + '&giaTri=' + phantram + '&id_giaban=' + self.selectedGiaBan() + '&idnhomhang=' + self.arrIDNhomHang() + '&iddonvi=' + _IDchinhanh, 'POST').done(function (data) {
                            SearchHangHoa();
                            bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật bảng giá thành công", "success");
                        });
                    }
                }
                else {
                    if (isPTram == false) {
                        chitiet = chitiet.concat(' + ', formatNumber(priceAdd), ' VND');
                    }
                    if (isPTram === true) {
                        chitiet = chitiet.concat(' - ', phantram, ' %');
                    }

                    var objUpdatetemp1 = {
                        ID: _id,
                        GiaBan: giamoi
                    };
                    objUpdate.push(objUpdatetemp1);
                    mydata.objData = objUpdate;

                    $.ajax({
                        url: GiaBanUri + "PutGiaBanChiTiet",
                        type: 'POST',
                        dataType: 'json',
                        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                        data: mydata,
                        success: function (result) {
                            bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật bảng giá thành công", "success");
                            SearchHangHoa();
                        },
                        error: function (error) {
                            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cập nhật bảng giá thất bại", "danger");
                            $('#modalpopup_deleteGB').modal('hide');
                        }
                    })

                }
            }
        }

        tenBangGia = tenBangGia === '' ? 'Bảng giá chuẩn' : tenBangGia;
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: 'Cập nhật bảng giá',
            NoiDung: '',
            LoaiNhatKy: 2
        }
        if (applyAll) {
            let sNhom = '';
            for (let i = 0; i < self.arrIDNhomHang().length; i++) {
                let nhom = $.grep(self.NhomHangHoas(), function (x) {
                    return x.ID === self.arrIDNhomHang()[i];
                });
                if (nhom.length > 0) {
                    sNhom += nhom[0].TenNhomHangHoa + ', ';
                }
            }
            sNhom = sNhom === '' ? '' : 'thuộc nhóm ' + Remove_LastComma(sNhom);
            objDiary.NoiDung = 'Cập nhật giá bán cho '.concat(self.TotalRecord(), ' sản phẩm ', sNhom, ' ở <b> ', tenBangGia, '</b>');
            objDiary.NoiDungChiTiet = 'Giá mới ='.concat(chitiet);
        }
        else {
            objDiary.NoiDung = 'Cập nhật giá bán cho sản phẩm  <b>'.concat(item.TenHangHoa, ' </b> thuộc bảng giá ', tenBangGia);
            objDiary.NoiDungChiTiet = 'Giá mới ='.concat(chitiet,
                '<br /> - Người sửa: ', VHeader.UserLogin,
                '<br /> <b> Thông tin cũ: </b>',
                '<br />- Giá nhập cuối: ', formatNumber3Digit(self.ItemOld().GiaNhapCuoi),
                '<br />- Giá chung: ', formatNumber3Digit(self.ItemOld().GiaChung),
                '<br />- Giá bán cũ: ', formatNumber3Digit(self.ItemOld().GiaMoi)
            );
        }
        SaveDiary(objDiary);
    }

    function SaveDiary(obj) {
        var myData = { objDiary: obj };
        ajaxHelper(DiaryUri + "post_NhatKySuDung", 'POST', myData).done(function (data) {
        });
    }

    self.ItemOld = ko.observable();
    self.ShowItem = function (item) {
        self.ItemOld(item);
    }

    self.ApDungGiaBanNow = function (item) {

        var gianew = $('#' + item.ID).val();
        if (gianew == "") {
            gianew = 0;
        }
        var objUpdate = [];
        var mydata = {};

        let tenBG = '';
        let diary = {
            ID_DonVi: VHeader.IdDonVi,
            ID_NhanVien: VHeader.IdNhanVien,
            LoaiNhatKy: 2,
            ChucNang: 'Danh mục bảng giá',
            NoiDung: 'Cập nhật giá bán cho hàng hóa '.concat(item.TenHangHoa),
            NoiDungChiTiet: 'Cập nhật giá bán cho hàng hóa '.concat(item.TenHangHoa, ' (', item.MaHangHoa, ')',
                '<br />- Người sửa: ', VHeader.UserLogin,
                '<br />- Giá cũ: ', formatNumber3Digit(self.ItemOld().GiaMoi),
                '<br />- Giá mới: ', formatNumber3Digit(gianew)
            ),
        };

        if (self.selectedGiaBanName() === "") {
            tenBG = '<br />- Tên bảng giá: Bảng giá chuẩn';

            var objUpdatetemp = {
                ID: item.ID,
                GiaBan: gianew
            };
            objUpdate.push(objUpdatetemp);
            mydata.ID = item.IDQuyDoi;
            mydata.objData = objUpdate;
            $.ajax({
                url: GiaBanUri + "PutGiaBanChiTietChung",
                type: 'POST',
                dataType: 'json',
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                data: mydata,
            }).done(function (x) {
                diary.NoiDungChiTiet = diary.NoiDungChiTiet.concat(tenBG);
                Insert_NhatKyThaoTac_1Param(diary);
            })
        }
        else {
            tenBG = '<br />- Tên bảng giá: ' + self.selectedGiaBanName();
            let objUpdatetemp1 = {
                ID: item.ID,
                GiaBan: gianew
            };
            objUpdate.push(objUpdatetemp1);
            mydata.objData = objUpdate;

            $.ajax({
                url: GiaBanUri + "PutGiaBanChiTiet",
                type: 'POST',
                dataType: 'json',
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                data: mydata,
            }).done(function () {
                ShowMessage_Success('Cập nhật bảng giá thành công');
                diary.NoiDungChiTiet = diary.NoiDungChiTiet.concat(tenBG);
                Insert_NhatKyThaoTac_1Param(diary);
            }).fail(function () {
                ShowMessage_Danger('Cập nhật bảng giá thất bại');
            })
        }
    }
    self.test = function (data, event) {
        var gianew = formatNumberToInt($('#' + data.ID).val());
        if (data.GiaMoi !== gianew) {
            if (gianew == "") {
                gianew = 0;
            }
            var objUpdate = [];
            var mydata = {};
            if (event.which == 13) {
                if (self.selectedGiaBanName() === "") {
                    var objUpdatetemp = {
                        ID: data.ID,
                        GiaBan: gianew
                    };
                    objUpdate.push(objUpdatetemp);
                    mydata.ID = data.ID;
                    mydata.objData = objUpdate;
                    $.ajax({
                        url: GiaBanUri + "PutGiaBanChiTietChung",
                        type: 'POST',
                        dataType: 'json',
                        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                        data: mydata,
                        success: function (result) {
                            bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật bảng giá thành công!", "success");
                            $('#' + data.ID).closest('tr').next().find('.newprice').select();
                            $('#' + data.ID).next(".callprice").toggle();
                            $('#' + data.ID).closest('tr').next().find('.newprice').next(".callprice").toggle();
                            rowid = $('#' + data.ID).closest('tr').next().find('.newprice').attr('id');
                        },
                        error: function (error) {
                            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cập nhật bảng giá thất bại", "danger");
                            $('#modalpopup_deleteGB').modal('hide');
                        }
                    })
                }
                else {
                    var objUpdatetemp1 = {
                        ID: data.ID,
                        GiaBan: gianew
                    };
                    objUpdate.push(objUpdatetemp1);
                    mydata.objData = objUpdate;

                    $.ajax({
                        url: GiaBanUri + "PutGiaBanChiTiet",
                        type: 'POST',
                        dataType: 'json',
                        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                        data: mydata,
                        success: function (result) {
                            bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật bảng giá thành công!", "success");
                            $('#' + data.ID).closest('tr').next().find('.newprice').select();
                            $('#' + data.ID).next(".callprice").toggle();
                            $('#' + data.ID).closest('tr').next().find('.newprice').next(".callprice").toggle();
                            rowid = $('#' + data.ID).closest('tr').next().find('.newprice').attr('id');
                        },
                        error: function (error) {
                            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cập nhật bảng giá thất bại", "danger");
                            $('#modalpopup_deleteGB').modal('hide');
                        }
                    })
                }
            }
        }
        else {
            if (event.which == 13) {
                $('#' + data.ID).closest('tr').next().find('.newprice').select();
                $('#' + data.ID).next(".callprice").toggle();
                $('#' + data.ID).closest('tr').next().find('.newprice').next(".callprice").toggle();
                rowid = $('#' + data.ID).closest('tr').next().find('.newprice').attr('id');
            }
        }
    }
    //lọc theo cột
    //Tìm kiếm JQAuTo
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

    function locdau(obj) {
        if (!obj)
            return "";
        var str = obj;
        str = str.toLowerCase();
        str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
        str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
        str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
        str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
        str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
        str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
        str = str.replace(/đ/g, "d");
        str = str.replace(/^\-+|\-+$/g, "");

        // Some system encode vietnamese combining accent as individual utf-8 characters
        str = str.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, ""); // Huyền sắc hỏi ngã nặng 
        str = str.replace(/\u02C6|\u0306|\u031B/g, ""); // Â, Ê, Ă, Ơ, Ư

        return str;
    }
    //phân trang
    self.pageSizes = [10, 20, 30];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.currentPage = ko.observable(0);
    self.fromitem = ko.observable(1);
    self.toitem = ko.observable();
    self.arrPagging = ko.observableArray();
    self.arrPagging = ko.observableArray();

    getAllGiaBan();
    //getListHangHoa();
    getAllGiaBanChiTiet();

    self.PageCount = ko.observable();
    function GetPageCountHoaDon() {
        ajaxHelper(GiaBanUri + 'GetPageCountHoaDon?pageSize=' + self.pageSize(), 'GET').done(function (data) {
            self.PageCount(data);
        })
    }

    self.TotalRecord = ko.observable(0);
    function GetTotalRecord() {
        ajaxHelper(GiaBanUri + 'GetTotalRecord/', 'GET').done(function (data) {
            self.TotalRecord(data);
        })
    }


    self.xoaAllChiTietBangGia = function () {
        $('.table_price').gridLoader();
        ajaxHelper(GiaBanUri + "deleteChiTiet?idgiaban=" + self.selectedGiaBan(), 'GET').done(function (data) {
            if (data === "") {
                getAllGiaBanChiTiet();
                $('.table_price').gridLoader({ show: false });
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Xóa dữ liệu thành công!", "success");
            }
            else {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Xóa chi tiết bảng giá thất bại", "danger");
            }
        })
    }


    self.PageResults = ko.computed(function () {
        if (self.GiaBanChitiets() !== null) {

            self.fromitem((self.currentPage() * self.pageSize()) + 1);

            if (((self.currentPage() + 1) * self.pageSize()) > self.GiaBanChitiets().length) {
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

    self.PageList = ko.computed(function () {
        if (self.PageCount() > 1) {
            return Array.apply(null, {
                length: self.PageCount()
            }).map(Number.call, Number);
        }
    });

    self.ResetCurrentPage = function () {
        self.currentPage(0);
        SearchHangHoa();
    };

    self.GoToPage = function (page) {
        self.currentPage(page);
    };

    self.GetClass = function (page) {
        return (page === self.currentPage()) ? "click" : "";
    };

    //=====Paging BangGia=====
    self.GetClassHD = function (page) {
        return ((page.pageNumber - 1) === self.currentPage()) ? "click" : "";
    };

    $('#txtBangGiaFilter').keypress(function (e) {
        $("#iconSort").remove();
        self.columsort(null);
        self.sort(null);
        if (e.keyCode === 13) {
            self.currentPage(0);
            SearchHangHoa();
        }
    })

    var txtMaHDon_Excel;
    var isGoToNext = false;
    function SearchHangHoa(isGoToNext) {
        var txtMaHDon = self.filter();
        if (txtMaHDon === undefined) {
            txtMaHDon = "";
        }
        txtMaHDon_Excel = txtMaHDon;
        // NgayLapHoaDon
        // get list HoaDon
        $('.table_price').gridLoader({
            style: "top:222px"
        });
        ajaxHelper(GiaBanUri + 'GetListGiaBans_where?currentPage=' + self.currentPage() + '&pageSize=' + self.pageSize() + '&idnhomhang=' + self.arrIDNhomHang() +
            '&maHoaDon=' + txtMaHDon + '&_id=' + self.selectedGiaBan() + '&columsort=' + self.columsort() + '&sort=' + self.sort() + '&iddonvi=' + _IDchinhanh,
            'GET').done(function (data) {
                $('.table_price').gridLoader({ show: false });
                self.GiaBanChitiets(data.lstBG);
                self.TotalRecord(data.Rowcount);
                self.PageCount(data.pageCount);
                LoadHtmlGrid();
            });
        //allGiaChiTiet();
    }

    self.clickiconSearchBG = function () {
        SearchHangHoa();
    }

    self.PageList_Display = ko.computed(function () {
        var arrPage = [];

        var allPage = self.PageCount();
        var currentPage = self.currentPage();

        //if (allPage > 4) {

        //    var i = 0;
        //    if (currentPage === 0) {
        //        i = parseInt(self.currentPage()) + 1;
        //    }
        //    else {
        //        i = self.currentPage();
        //    }

        //    if (allPage >= 5 && currentPage > allPage - 5) {
        //        if (currentPage >= allPage - 2) {
        //            // get 5 trang cuoi cung
        //            for (var i = allPage - 5; i < allPage; i++) {
        //                var obj = {
        //                    pageNumber: i + 1,
        //                };
        //                arrPage.push(obj);
        //            }
        //        }
        //        else {
        //            // get currentPage - 2 , currentPage, currentPage + 2
        //            for (var j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
        //                var obj = {
        //                    pageNumber: j + 1,
        //                };
        //                arrPage.push(obj);
        //            }
        //        }
        //    }
        //    else {
        //        // get 5 trang dau
        //        if (i >= 2) {
        //            while (arrPage.length < 5) {
        //                var obj = {
        //                    pageNumber: i - 1,
        //                };
        //                arrPage.push(obj);
        //                i = i + 1;
        //            }
        //        }
        //        else {
        //            while (arrPage.length < 5) {
        //                var obj = {
        //                    pageNumber: i,
        //                };
        //                arrPage.push(obj);
        //                i = i + 1;
        //            }
        //        }
        //    }
        //}
        //else {
        //    // neu chi co 1 trang --> khong hien thi DS trang
        //    if (allPage > 1) {
        //        for (var i = 0; i < allPage; i++) {
        //            var obj = {
        //                pageNumber: i + 1,
        //            };
        //            arrPage.push(obj);
        //        }
        //    }
        //}

        if (allPage > 5) {
            if (currentPage > 2 && currentPage < (allPage - 2)) {
                for (let i = 0; i < 5; i++) {
                    arrPage.push({ pageNumber: currentPage - 1 + i });
                }
            }
            else if (currentPage >= (allPage - 2)) {
                for (let i = 0; i < 5; i++) {
                    arrPage.push({ pageNumber: allPage - 4 + i });
                }
            }
            else {
                for (let i = 0; i < 5; i++) {
                    arrPage.push({ pageNumber: 1 + i });
                }
            }
        }
        else {
            if (allPage !== 0) {
                for (let i = 0; i < allPage; i++) {
                    arrPage.push({ pageNumber: 1 + i });
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
            SearchHangHoa(true);
        }
    };

    self.StartPage = function () {
        self.currentPage(0);
        SearchHangHoa(true);
    }

    self.BackPage = function () {
        if (self.currentPage() > 1) {
            self.currentPage(self.currentPage() - 1);
            SearchHangHoa(true);
        }
    }

    self.GoToNextPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.currentPage() + 1);
            SearchHangHoa(true);
        }
    }

    self.EndPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.PageCount() - 1);
            SearchHangHoa(true);
        }
    }

    //sort theo colum bảng giá
    $('#myTableBG thead tr').on('click', 'th', function () {
        var id = $(this).attr('id');
        if (id === "txtMaHang") {
            self.columsort("MaHang");
            SortGrid(id);
        }
        if (id === "txttenhang") {
            self.columsort("TenHang");
            SortGrid(id);
        }
        if (id === "txttennhomhang") {
            self.columsort("TenNhomHang");
            SortGrid(id);
        }
        if (id === "txtgiavon") {
            self.columsort("GiaVon");
            SortGrid(id);
        }
        if (id === "txtgianhapcuoi") {
            self.columsort("GiaNhapCuoi");
            SortGrid(id);
        }
        if (id === "txtgiachung") {
            self.columsort("GiaChung");
            SortGrid(id);
        }
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
        SearchHangHoa();
    };
    //===============================
    // Load lai form lưu cache bộ lọc 
    // trên grid
    //===============================
    var cacheExcel = true;
    function LoadHtmlGrid() {
        if (window.localStorage) {
            var current = localStorage.getItem('danhsachgia');
            if (!current) {
                current = [];
                cacheExcel = false;
                localStorage.setItem('danhsachgia', JSON.stringify(current));
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
    // Add Các tham số cần lưu lại đẻ 
    // cache khi load lại form
    //===============================
    function addClass(name, id, value) {

        var current = localStorage.getItem('danhsachgia');
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
        localStorage.setItem('danhsachgia', JSON.stringify(current));
    }


    

    // Trinhpv xuất excel
    self.ExportExcel_GiaBan = async function () {

        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Danh sách giá",
            NoiDung: "Xuất báo cáo danh sách giá",
            NoiDungChiTiet: "Xuất báo cáo danh sách giá",
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
            success:async function (item) {
                var columnHide = null;
                for (var i = 0; i < self.ColumnsExcel().length; i++) {
                    if (i == 0) {
                        columnHide = self.ColumnsExcel()[i];
                    }
                    else {
                        columnHide = self.ColumnsExcel()[i] + "_" + columnHide;
                    }
                }          

                const ok = await commonStatisJs.NPOI_ExportExcel(GiaBanUri + 'ExportExcel_GiaBan?idnhomhang=' + self.arrIDNhomHang() +
                    '&maHoaDon=' + txtMaHDon_Excel + '&_id=' + self.selectedGiaBan() + "&columnsHide=" + columnHide + '&iddonvi=' + _IDchinhanh, 'GET', null, 'DanhMucGiaBan.xlsx'); 
                               
            },
            statusCode: {
                404: function () {
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ghi nhật ký sử dụng thất bại!", "danger");
            },
            complete: function () {

            }
        })
    }
    self.ColumnsExcel = ko.observableArray();
    self.addColum = function (item) {
        if (self.ColumnsExcel().length < 1) {
            self.ColumnsExcel.push(item);
        }
        else {
            for (var i = 0; i < self.ColumnsExcel().length; i++) {
                if (self.ColumnsExcel()[i] === item) {
                    self.ColumnsExcel.splice(i, 1);
                    break;
                }
                if (i == self.ColumnsExcel().length - 1) {
                    self.ColumnsExcel.push(item);
                    break;
                }
            }
        }
        self.ColumnsExcel.sort();
    }

    $("#cbmahang").click(function () {
        $(".mahang").toggle();
        addClass(".mahang", "cbmahang", $(this).val());
        self.addColum($(this).val());
    });
    $("#cbtenhang").click(function () {
        $(".tenhang").toggle();
        addClass(".tenhang", "cbtenhang", $(this).val());
        self.addColum($(this).val());
    });

    $("#cbtendonvitinh").click(function () {
        $(".tendonvitinh").toggle();
        addClass(".tendonvitinh", "cbtendonvitinh", $(this).val());
        self.addColum($(this).val());
    });

    $("#cbtennhomhang").click(function () {
        $(".tennhomhang").toggle();
        addClass(".tennhomhang", "cbtennhomhang", $(this).val());
        self.addColum($(this).val());
    });

    $("#cbgiavon").click(function () {
        $(".giavon").toggle();
        addClass(".giavon", "cbgiavon", $(this).val());
        self.addColum($(this).val());
    });

    $("#cbgianhapcuoi").click(function () {
        $(".gianhapcuoi").toggle();
        addClass(".gianhapcuoi", "cbgianhapcuoi", $(this).val());
        self.addColum($(this).val());
    });

    $("#cbgiamoi").click(function () {
        $(".giamoi").toggle();
        addClass(".giamoi", "cbgiamoi", $(this).val());
        self.addColum($(this).val());
    });
};

ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {

        //initialize datepicker with some optional options
        var options = {
            format: 'DD/MM/YYYY HH:mm',
            defaultDate: new Date()
        };

        if (allBindingsAccessor() !== undefined) {
            if (allBindingsAccessor().datepickerOptions !== undefined) {
                options.format = allBindingsAccessor().datepickerOptions.format !== undefined ? allBindingsAccessor().datepickerOptions.format : options.format;
            }
        }

        $(element).datetimepicker(options);

        //when a user changes the date, update the view model
        ko.utils.registerEventHandler(element, "dp.change", function (event) {
            var value = valueAccessor();
            if (ko.isObservable(value)) {
                value(event.date);
            }
        });

        var defaultVal = $(element).val();
        var value = valueAccessor();
        value(moment(defaultVal, options.format));
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var thisFormat = 'DD/MM/YYYY HH:mm';

        if (allBindingsAccessor() !== undefined) {
            if (allBindingsAccessor().datepickerOptions !== undefined) {
                thisFormat = allBindingsAccessor().datepickerOptions.format !== undefined ? allBindingsAccessor().datepickerOptions.format : thisFormat;
            }
        }

        var value = valueAccessor();
        var unwrapped = ko.utils.unwrapObservable(value());

        if (unwrapped === undefined || unwrapped === null) {
            element.value = new moment(new Date()).format(thisFormat);
        } else {
            element.value = moment(unwrapped).format(thisFormat);
        }
    }


};

var ctgiavm = new CTGiaViewModel();
ko.applyBindings(ctgiavm);
function hidewait(o) {
    $('.' + o).append('<div id="wait"><img src="/Content/images/wait.gif" width="64" height="64" /><div class="happy-wait">' +
        ' </div>' +
        '</div>')
}
function selectedCT(obj) {
    if ($(obj).children().length === 0) {
        $(obj).append('<i class="fa fa-check" aria-hidden="true"></i><i class="fa fa-times"></i>');
    }
    if ($(obj).children().length > 0) {
        arrIDchose.push($(obj).attr('id'));
    }
}
$('input[type=text]').click(function () {
    $(this).select();
});
