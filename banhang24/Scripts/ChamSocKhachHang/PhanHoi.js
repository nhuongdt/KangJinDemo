// validate ky tu dac biet
function isValid(str) {
    return !/[~`!@#$%\^&*()+=\-\[\]\\';,/{}|\\":<>\?]/g.test(str);
};
var Key_Form = "Key_PhieuPhanHoi";
$('#selectColumn').on('click', '.dropdown-list li input[type = checkbox]', function (i) {
    var valueCheck = $(this).val();
    LocalCaches.AddColumnHidenGrid(Key_Form, valueCheck, i);
    $('.' + valueCheck).toggle();
});
function loadHtmlGrid() {
    LocalCaches.LoadFirstColumnGrid(Key_Form, $('#selectColumn ul li input[type = checkbox]'), []);
}
loadHtmlGrid();
var FormModel_LoaiPH = function () {
    var self = this;
    self.ID = ko.observable();
    self.TenLoaiTuVanLichHen = ko.observable();
    self.ID_NguoiTao = ko.observable();
    self.TuVan_LichHen = ko.observable();
    self.NgayTao = ko.observable();
    self.ID_NguoiSua = ko.observable();
    self.NgaySua = ko.observable();

    self.setdata = function (item) {
        self.ID(item.ID);
        self.TenLoaiTuVanLichHen(item.TenLoaiTuVanLichHen);
        self.ID_NguoiTao(item.ID_NguoiTao);
        self.TuVan_LichHen(item.TuVan_LichHen);
        self.NgayTao(item.NgayTao);
        self.ID_NguoiSua(item.ID_NguoiSua);
        self.NgaySua(item.NgaySua);
    }
};

var FormModel_NewPhanHoi = function () {
    var self = this;
    self.ID = ko.observable();
    self.ID_KhachHang = ko.observable();
    self.ID_LoaiTuVan = ko.observable();
    self.ID_NhanVien = ko.observable();
    self.Ma_TieuDe = ko.observable();
    self.NgayGio = ko.observable();
    self.ThoiGianHenLai = ko.observable();
    self.NgayGioKetThuc = ko.observable();
    self.TrangThai = ko.observableArray();
    self.NoiDung = ko.observable();
    self.TraLoi = ko.observable();
    self.MucDoPhanHoi = ko.observableArray();

    self.SetData = function (item) {
        self.ID(item.ID);
        self.Ma_TieuDe(item.Ma_TieuDe);
        self.ID_KhachHang(item.ID_KhachHang);
        self.ID_LoaiTuVan(item.ID_LoaiTuVan);
        self.ID_NhanVien(item.ID_NhanVien);
        self.NgayGio(item.NgayGio);
        self.NgayGioKetThuc(item.NgayGioKetThuc);
        self.ThoiGianHenLai(item.ThoiGianHenLai);
        self.TrangThai(item.TrangThai);
        self.NoiDung(item.NoiDung);// noi dung trả lời
        self.TraLoi(item.TraLoi);// nội dung phản hồi
        self.MucDoPhanHoi(item.MucDoPhanHoi);
    };
};
var ViewModel = function () {
    var CSKHUri = '/api/DanhMuc/ChamSocKhachHangAPI/';
    var DMDoiTuongUri = '/api/DanhMuc/DM_DoiTuongAPI/';
    var NhanVienUri = '/api/DanhMuc/NS_NhanVienAPI/';
    var DM_LoaiTVLHUri = '/api/DanhMuc/DM_LoaiTuVanLichHenAPI/';
    var self = this;
    self.deletePhieuPhanHoi = ko.observable();
    self.deleteID = ko.observable();
    self.LoaiTuVanLichHens = ko.observableArray();
    var idNhanVien = $('.idnhanvien').text();
    var idDonVi = $('#hd_IDdDonVi').val();
    self.filterNV = ko.observable();
    self.NhanViens = ko.observableArray();
    self.selectedNV = ko.observable();

    self.filterKH = ko.observable();
    self.DoiTuongs = ko.observableArray();
    self.selectedKH = ko.observable();

    self.ckThap = ko.observable(true);
    self.ckTB = ko.observable(true);
    self.ckCao = ko.observable(true);

    self.ckChuaXuLy = ko.observable(true);
    self.ckDangXuLy = ko.observable(true);
    self.ckDaXong = ko.observable(true);

    self.deleteTenLoaiPhanHoi = ko.observable();
    self.error = ko.observable();
    self.filter = ko.observable();
    self.PhanHois = ko.observableArray();
    self.LoaiPhanHois = ko.observableArray();
    self.selectedLoaiPhanHoi = ko.observable();
    self.booleanAdd = ko.observable(true);
    self._ThemLoaiPH = ko.observable(true);
    self.newPhanHoi = ko.observable(new FormModel_NewPhanHoi());
    self.newLoaiPhanHoi = ko.observable(new FormModel_LoaiPH());

    self.pageSizes = [10, 30, 40, 50];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.currentPage = ko.observable(0);
    self.fromitem = ko.observable(1);
    self.toitem = ko.observable();
    self.arrPagging = ko.observableArray();
    self.filterNgayLapHD = ko.observable("0");
    self.filterNgayLapHD_Input = ko.observable(); // ngày cụ thể
    self.filterNgayLapHD_Quy = ko.observable(0); // Theo quý

    self.MucDoPhanHoi = ko.observableArray([
        { name: "Cao", value: "1" },
        { name: "Trung bình", value: "2" },
        { name: "Thấp", value: "3" }
    ]);
    self.selectedMucDoPhanHoi = ko.observable();

    self.TrangThai = ko.observableArray([
        { name: "Chưa xử lý", value: "1" },
        { name: "Đang xử lý", value: "2" },
        { name: "Đã xong", value: "3" }
    ]);
    self.selectedTrangThai = ko.observable();

    self.ListIDNhanVienQuyen = ko.observableArray();
    function LoadID_NhanVien() {
        ajaxHelper(CSKHUri + 'GetListNhanVienLienQuanByIDLoGin_inDepartment?idnvlogin=' + idNhanVien
            + '&idChiNhanh=' + idDonVi + '&funcName=' + funcName, 'GET').done(function (data) {
            self.ListIDNhanVienQuyen(data);
            var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
            if ($.inArray('PhanHoi_ThemMoi', lc_CTQuyen) > -1) {
                $('.ThemMoiPhanHoi').show();
            }
            else {
                $('.ThemMoiPhanHoi').hide();
            }
            SearchPhanHoi();
        })
    }
    LoadID_NhanVien();

    self.LoadQuyen = function () {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('PhanHoi_CapNhat', lc_CTQuyen) > -1) {
            $('.txtCapNhatPH').show();
        }
        else {
            $('.txtCapNhatPH').hide();
        }
        if ($.inArray('PhanHoi_Xoa', lc_CTQuyen) > -1) {
            $('.txtXoaPhanHoi').show();
        }
        else {
            $('.txtXoaPhanHoi').hide();
        }

    }

    self.themmoiphanhoi = function () {
        self.resetTextBox();

        self.booleanAdd(true);
        $('#myModalphanhoi').modal('show');
        $('#txtMaPhieu').focus();
    }

    self.editPhanHoi = function (item) {
        ajaxHelper(CSKHUri + "GetPhanHoi/" + item.ID, 'GET').done(function (data) {
            self.newPhanHoi().SetData(data);
            self.booleanAdd(false);
            self.selectedMucDoPhanHoi(data.MucDoPhanHoi);
            self.selectedLoaiPhanHoi(data.ID_LoaiTuVan);
            self.selectedKH(data.ID_KhachHang);
            self.selectedNV(data.ID_NhanVien);
            $('#lstKhachHang span').each(function () {
                $(this).empty();
            });
            $('#lstNhanVien span').each(function () {
                $(this).empty();
            });
            if (data.ID_KhachHang !== null) {
                $('#txtKhachHang').text(data.TenKhachHang);
                $(function () {
                    $('span[id=spanCheckKhachHang_' + data.ID_KhachHang + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
                });
            }
            else {
                $('#txtKhachHang').text("--- Chọn khách hàng ---");
            }
            if (data.ID_NhanVien !== null) {
                $('#txtNhanVienChiaSe').text(data.TenNV);
                $(function () {
                    $('span[id=spanCheckNhanVien_' + data.ID_NhanVien + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
                });
            }
            else {
                $('#txtNhanVienChiaSe').text("--- Chọn nhân viên ---");
            }
            $('#txtThoiGianPH').val(moment(data.NgayGio, "YYYY-MM-DD HH:mm").format('DD/MM/YYYY HH:mm'));
            $('#txtThoiGianHenLaiPH').val(moment(data.ThoiGianHenLai, "YYYY-MM-DD HH:mm").format('DD/MM/YYYY HH:mm'));
        });
        $('#myModalphanhoi').modal('show');
     
    }

    self.resetTextBox = function () {
        self.newPhanHoi(new FormModel_NewPhanHoi());
        self.newPhanHoi().TrangThai("1");
        self.selectedMucDoPhanHoi(1);
        self.selectedLoaiPhanHoi(undefined);
        self.selectedKH(undefined);
        self.selectedNV(undefined);
        $('#txtKhachHang').text("--- Chọn khách hàng ---");
        $('#lstKhachHang span').each(function () {
            $(this).empty();
        });
        $('#txtNhanVienChiaSe').text("--- Chọn nhân viên ---");
        $('#lstNhanVien span').each(function () {
            $(this).empty();
        });
        $('#txtThoiGianPH').val("");
        $('#txtThoiGianHenLaiPH').val("");
    }

    // Reset
    self.resetLoaiTVLH = function () {
        self.newLoaiPhanHoi(new FormModel_LoaiPH());

    }

    self.themmoiloaiphanhoi = function () {
        self.resetLoaiTVLH();
        self._ThemLoaiPH(true);
        $('#myModalphanloai').modal('show');
    }

    self.editloaiphanhoi = function (item) {
        ajaxHelper(DM_LoaiTVLHUri + "GetLoaiTuVan/" + this.selectedLoaiPhanHoi(), 'GET').done(function (data) {
            self.newLoaiPhanHoi().setdata(data);
            self._ThemLoaiPH(false);
        });
        $('#myModalphanloai').modal('show');
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
                }
            }
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
            });
    }


    function getAllDMLoaiTuVanLichHens() {
        ajaxHelper(DM_LoaiTVLHUri + "GetDM_LoaiPhanHoi", 'GET').done(function (data) {
            self.LoaiTuVanLichHens(data);
        });
    }
    getAllDMLoaiTuVanLichHens();

    self.addLoaiTuVanLichHen = function (formElement) {
        var _idLoaiTuVanLichHen = self.newLoaiPhanHoi().ID();
        var _tenLoaiTuVanLichHen = self.newLoaiPhanHoi().TenLoaiTuVanLichHen();

        if (_tenLoaiTuVanLichHen === null || _tenLoaiTuVanLichHen === "" || _tenLoaiTuVanLichHen === "undefined") {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Không được để trống tên loại tư vấn!", "danger");
            $('#txtTenLoaiTuVan').focus();
            return false;
        }
        var objLoaiTuVan = {
            ID: _idLoaiTuVanLichHen,
            TenLoaiTuVanLichHen: _tenLoaiTuVanLichHen
        };
        if (self._ThemLoaiPH() === true) {
            var myData = {};
            myData.objLoaiTVLH = objLoaiTuVan;
            //ajaxHelper(DM_LoaiTVLHUri + "Check_TenLoaiTuVanLichHenExist?tenLoaiTuVan=" + _tenLoaiTuVanLichHen, 'POST').done(function (data) {
            //    if (data) {
            //        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Tên loại tư vấn đã tồn tại!", "danger");
            //        getAllDMLoaiTuVanLichHens();
            //        return false;
            //    }
            //else {
            $.ajax({
                url: DM_LoaiTVLHUri + "PostLoaiPhanHoi",
                type: 'POST',
                dataType: 'json',
                data: myData,
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (item) {
                    self.LoaiTuVanLichHens.push(item);
                    self.selectedLoaiPhanHoi(item.ID);
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Thêm mới loại phản hồi thành công!", "success");
                },
                statusCode: {
                    404: function () {
                        self.error("page not found");
                    },
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                    $("#myModalphanloai").modal("hide");
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Thêm mới loại phản hồi thất bại!", "danger");
                },
                complete: function () {
                    $("#myModalphanloai").modal("hide");
                    self.resetLoaiTVLH();

                }
            })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                    $("#myModalphanloai").modal("hide");
                });
            //    }
            //});
        }
        else {
            var myData = {};
            myData.id = _idLoaiTuVanLichHen;
            myData.objLoaiTVLH = objLoaiTuVan;
            $.ajax({
                url: DM_LoaiTVLHUri + "PutLoaiPhanHoi",
                type: 'PUT',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: myData,
                success: function () {
                    $("#myModalphanloai").modal("hide");
                    getAllDMLoaiTuVanLichHens();
                },
                statusCode: {
                    404: function () {
                        self.error("page not found");
                    },
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                },
                complete: function () {
                    bottomrightnotify('Cập nhật loại phản hồi thành công !', 'success');
                }
            })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                });
        }
    }

    //THem/Sua
    self.addPhanHoi = function (formElement) {
        var _matieude = self.newPhanHoi().Ma_TieuDe();
        _matieude = _matieude === "" ? undefined : (_matieude === null ? undefined : _matieude);
        var _id = self.newPhanHoi().ID();
        var _idkhachhang = self.selectedKH();
        var _idnhanvien = self.selectedNV();
        var _ngaygio = $('#txtThoiGianPH').val();
        var _noidung = self.newPhanHoi().NoiDung(); // noi dung tra loi
        var _traloi = self.newPhanHoi().TraLoi();// noi dung phan hoi
        var _trangthai = self.newPhanHoi().TrangThai();
        var _mucdophanhoi = self.selectedMucDoPhanHoi();
        var _idloaiph = self.selectedLoaiPhanHoi();
        var _thoigianphanhoilai = $('#txtThoiGianHenLaiPH').val();

        if (_idkhachhang === undefined) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng chọn khách hàng", "danger");
            $('#txtKhachHang').focus();
            return false;
        }

        if (_idnhanvien === undefined) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng chọn nhân viên tư vấn", "danger");
            $('#txtAuto').focus();
            return false;
        }

        if (_idloaiph === undefined || _idloaiph === null) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng chọn loại phản hồi", "danger");
            $('#ddlLoaiTVLH1').focus();
            return false;
        }

        if (moment(_ngaygio, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') === "Invalid date") {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng chọn thời gian", "danger");
            $('#txtThoiGianPH').select();
            return false;
        }

        if (!isValid(_matieude)) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Mã phiếu không được nhập kí tự đặc biệt!", "danger");
            return false;
        }

        if ((moment(_ngaygio, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') > moment(_thoigianphanhoilai, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm')) && moment(_thoigianphanhoilai, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') !== "Invalid date") {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Thời gian hẹn lại phải lớn hơn thời gian bắt đầu", "danger");
            $('#txtThoiGianHenLaiPH').focus();
            return false;
        }


        var Phan_Hoi = {
            ID: _id,
            Ma_TieuDe: _matieude,
            NgayGio: moment(_ngaygio, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm'),
            ThoiGianHenLai: moment(_thoigianphanhoilai, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') === "Invalid date" ? null : moment(_thoigianphanhoilai, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm'),
            ID_KhachHang: _idkhachhang,
            ID_NhanVien: _idnhanvien,
            ID_LoaiTuVan: _idloaiph,
            NoiDung: _noidung,
            TraLoi: _traloi,
            TrangThai: _trangthai,
            MucDoPhanHoi: _mucdophanhoi
        };
        //console.log('111' + JSON.stringify(Phan_Hoi));
        //Them
        if (self.booleanAdd() === true) {
            var myData = {};
            myData.objPhanHoi = Phan_Hoi;

            // check Exist code, phone (check in server code)
            $.ajax({
                data: Phan_Hoi,
                url: CSKHUri + "Check_Exist",
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",

                success: function (item) {
                    // khong dong popup neu item !== ''
                    if (item === '') {
                        callAjaxAdd(myData);
                    }
                    else {
                        bottomrightnotify(item, "danger");
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
            })
        }
        else {
            var myData = {};
            myData.id = _id;
            myData.objPhanHoi = Phan_Hoi;
            $.ajax({
                data: Phan_Hoi,
                url: CSKHUri + "Check_Exist",
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",

                success: function (item) {
                    // khong dong popup neu item !== ''
                    if (item === '') {
                        callAjaxUpdate(myData);
                    }
                    else {
                        bottomrightnotify(item, "danger");
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
            })
        }
    };

    function callAjaxAdd(myData) {
        $.ajax({
            data: myData,
            url: CSKHUri + "PostPhanHoi",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",

            success: function (item) {
                bottomrightnotify("Thêm mới phiếu phản hồi thành công!", "success");
                
            },
            statusCode: {
                404: function () {
                    self.error("page not found");
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                bottomrightnotify("Thêm mới phiếu phản hồi thất bại!", "danger");
            },
            complete: function () {
                $("#myModalphanhoi").modal("hide");
                SearchPhanHoi();
            }
        })
    }

    function callAjaxUpdate(myData) {
        //console.log('myData' + JSON.stringify(myData))
        $.ajax({
            url: CSKHUri + "PutPhanHoi",
            type: 'PUT',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: myData,
            success: function () {
                $("#myModalphanhoi").modal("hide");
                SearchPhanHoi();
            },
            statusCode: {
                404: function () {
                    self.error("page not found");
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
            },
            complete: function () {
                bottomrightnotify('Cập nhật phiếu phản hồi thành công !', 'success');
            }
        })
    }

    self.modalDelete = function (item) {
        self.deletePhieuPhanHoi(item.Ma_TieuDe);
        self.deleteID(item.ID);
        $('#modalpopup_deletePhanHoi').modal('show');
    };
    //Xóa

    self.modalDeleteLoaiPhanHoi = function (LoaiPhanHois) {
        self.deleteTenLoaiPhanHoi(self.newLoaiPhanHoi().TenLoaiTuVanLichHen());
        self.deleteID(self.newLoaiPhanHoi().ID());
        $('#modalpopup_deleteLoaiTuVan').modal('show');
    };

    self.XoaPhanHoiLichSu = function (tieude) {
        var objDiary = {
            ID_NhanVien: idNhanVien,
            ID_DonVi: idDonVi,
            ChucNang: "Phản hồi",
            NoiDung: "Xóa phản hồi : " + tieude,
            NoiDungChiTiet: "Xóa phản hồi : " + tieude,
            LoaiNhatKy: 3 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
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
    self.xoaPhanHoi = function (PhanHois) {
        $.ajax({
            type: "DELETE",
            url: CSKHUri + "Delete_PhanHoi/" + PhanHois.deleteID(),
            dataType: 'json',
            contentType: 'application/json',
            success: function (result) {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Xóa phiếu phản hồi thành công !", "success");
                self.XoaPhanHoiLichSu(PhanHois.deletePhieuPhanHoi());
                SearchPhanHoi();
            },
            error: function (error) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Xóa phiếu phản hồi thất bại!", "danger");
            }
        })
    };

    // Xoa loai phan hoi
    self.xoaLoaiPhanHoi = function (LoaiPhanHois) {
        //console.log(JSON.stringify(LoaiPhanHois));
        $.ajax({
            type: "DELETE",
            url: DM_LoaiTVLHUri + "Delete_LoaiTuVan/" + self.newLoaiPhanHoi().ID(),
            dataType: 'json',
            contentType: 'application/json',
            success: function (result) {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Xóa loại phản hồi thành công !", "success");
                getAllDMLoaiTuVanLichHens();
            },
            error: function (error) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Xóa loại phản hồi thất bại!", "danger");
            }
        })
    };
    //auto search khách hàng
    function getAllDMDoiTuong() {
        ajaxHelper(DMDoiTuongUri + "GetListKhachHang?loaiDoiTuong=1", 'GET').done(function (data) {
            self.DoiTuongs(data);
        });
    }
    getAllDMDoiTuong();

    self.selectedChooseKhachHang = ko.observable();
    self.selectedChooseKhachHang.subscribe(function (newValue) {
        if (newValue !== undefined) {
            self.selectedKH(newValue.ID);
            $('#txtKhachHang').text(newValue.TenDoiTuong);
            $('#lstKhachHang span').each(function () {
                $(this).empty();
            });
            $(function () {
                $('span[id=spanCheckKhachHang_' + newValue.ID + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
            });
        }
    })
    //khách hàng filter
    self.filterKhachHang = ko.observable();
    self.arrFilterKhachHang = ko.computed(function () {
        var _filter = self.filterKhachHang();

        return arrFilter = ko.utils.arrayFilter(self.DoiTuongs(), function (prod) {
            var chon = true;
            var arr = locdau(prod.TenDoiTuong).toLowerCase().split(/\s+/);
            var sSearch = '';

            for (var i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }

            var arr1 = locdau(prod.MaDoiTuong).toLowerCase().split(/\s+/);
            var sSearch1 = '';

            for (var i = 0; i < arr1.length; i++) {
                sSearch1 += arr1[i].toString().split('')[0];
            }

            if (chon && _filter) {
                chon = (locdau(prod.TenDoiTuong).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
                    sSearch.indexOf(locdau(_filter).toLowerCase()) >= 0 || locdau(prod.MaDoiTuong).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
                    sSearch1.indexOf(locdau(_filter).toLowerCase()) >= 0
                );
            }
            return chon;
        });
    });

    self.clickFistChonKhachHang = function () {
        self.selectedKH(undefined);
        $('#lstKhachHang span').each(function () {
            $(this).empty();
        });
        $('#txtKhachHang').text('--- Chọn khách hàng ---');
    };

    //auto search nhân viên
    function getAllNSNhanVien() {
        ajaxHelper("/api/DanhMuc/NS_NhanVienAPI/" + "GetNS_NhanVien_DaTaoND?idDonVi=" + idDonVi, 'GET').done(function (data) {
            if (data !== null) {
                self.NhanViens(data);
            }
        });
    }
    getAllNSNhanVien();

    self.selectedChooseNhanVien = ko.observable();
    self.selectedChooseNhanVien.subscribe(function (newValue) {
        if (newValue !== undefined) {
            self.selectedNV(newValue.ID);
            $('#txtNhanVienChiaSe').text(newValue.TenNhanVien);
            $('#lstNhanVien span').each(function () {
                $(this).empty();
            });
            $(function () {
                $('span[id=spanCheckNhanVien_' + newValue.ID + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>');
            });
        }
    })
    //nhân viên chia sẻ
    self.filterNhanVien = ko.observable();
    self.arrFilterNhanVien = ko.computed(function () {
        var _filter = self.filterNhanVien();

        return arrFilter = ko.utils.arrayFilter(self.NhanViens(), function (prod) {
            var chon = true;
            var arr = locdau(prod.TenNhanVien).toLowerCase().split(/\s+/);
            var sSearch = '';

            for (var i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }

            var arr1 = locdau(prod.MaNhanVien).toLowerCase().split(/\s+/);
            var sSearch1 = '';

            for (var i = 0; i < arr1.length; i++) {
                sSearch1 += arr1[i].toString().split('')[0];
            }

            if (chon && _filter) {
                chon = (locdau(prod.TenNhanVien).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
                    sSearch.indexOf(locdau(_filter).toLowerCase()) >= 0 || locdau(prod.MaNhanVien).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
                    sSearch1.indexOf(locdau(_filter).toLowerCase()) >= 0
                );
            }
            return chon;
        });
    });

    self.clickFistChonNhanVien = function () {
        self.selectedNV(undefined);
        $('#lstNhanVien span').each(function () {
            $(this).empty();
        });
        $('#txtNhanVienChiaSe').text('--- Chọn nhân viên ---');
    }


    function locdau(obj) {
        if (!obj)
            return "";
        var str = obj;
        str = str.toString().toLowerCase();
        str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
        str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
        str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
        str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
        str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
        str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
        str = str.replace(/đ/g, "d");
        str = str.replace(/^\-+|\-+$/g, "");
        return str;
    }

    self.MangLoaiPhanHoi = ko.observableArray();

    self.selectedLoaiTuVanFilter = function (item) {
        var arrLCV = [];
        for (var i = 0; i < self.MangLoaiPhanHoi().length; i++) {
            if ($.inArray(self.MangLoaiPhanHoi()[i], arrLCV) === -1) {
                arrLCV.push(self.MangLoaiPhanHoi()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrLCV) === -1) {
            self.MangLoaiPhanHoi.push(item);
        }
        SearchPhanHoi();
        // thêm dấu check vào đối tượng được chọn
        $('#selec-all-TuVan li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
                $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
            }
        });
        $('#choose_LoaiTuVan input').remove();
    }

    self.CloseTuVan = function (item) {
        self.MangLoaiPhanHoi.remove(item);
        if (self.MangLoaiPhanHoi().length === 0) {
            $('#choose_LoaiTuVan').append('<input type="text" id="dllTuVan" readonly="readonly" class="dropdown" placeholder="Chọn loại phản hồi">');
        }
        SearchPhanHoi();
        // remove checks
        $('#selec-all-TuVan li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
    }

    //phân trang
    self.MangLoaiPhanHoiFilter = ko.observableArray();
    self.TotalRecord = ko.observableArray();
    self.PageCount = ko.observableArray();

    self.TodayBC = ko.observable();
    self.MangIDNhanVien = ko.observableArray();
    function SearchPhanHoi() {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('PhanHoi_XemDS', lc_CTQuyen) > -1) {
            var arrLCV = [];
            for (var i = 0; i < self.MangLoaiPhanHoi().length; i++) {
                if ($.inArray(self.MangLoaiPhanHoi()[i], arrLCV) === -1) {
                    arrLCV.push(self.MangLoaiPhanHoi()[i].ID);
                }
            }

            self.MangLoaiPhanHoiFilter(arrLCV);
            var arrIDNV = [];
            for (var i = 0; i < self.ListIDNhanVienQuyen().length; i++) {
                if ($.inArray(self.ListIDNhanVienQuyen()[i], arrIDNV) === -1) {
                    arrIDNV.push(self.ListIDNhanVienQuyen()[i]);
                }
            }
            self.MangIDNhanVien(arrIDNV);
            var statusInvoice = 1;
            if (self.ckThap()) {
                if (self.ckTB()) {

                    if (self.ckCao()) {
                        statusInvoice = 6;
                    } else {
                        statusInvoice = 5;
                    }
                }
                else {
                    if (self.ckCao()) {
                        statusInvoice = 4;
                    } else {
                        statusInvoice = 1; // HT
                    }
                }
            }
            else {
                if (self.ckTB()) {
                    if (self.ckCao()) {
                        statusInvoice = 0;
                    } else {
                        statusInvoice = 3;
                    }
                } else {
                    if (self.ckCao()) {
                        statusInvoice = 2;
                    } else {
                        statusInvoice = 7;
                    }
                }
            }

            var xuly = 1;
            if (self.ckDaXong()) {
                if (self.ckDangXuLy()) {

                    if (self.ckChuaXuLy()) {
                        xuly = 6;
                    } else {
                        xuly = 5;
                    }
                }
                else {
                    if (self.ckChuaXuLy()) {
                        xuly = 4;
                    } else {
                        xuly = 1; // HT
                    }
                }
            }
            else {
                if (self.ckDangXuLy()) {
                    if (self.ckChuaXuLy()) {
                        xuly = 0;
                    } else {
                        xuly = 3;
                    }
                } else {
                    if (self.ckChuaXuLy()) {
                        xuly = 2;
                    } else {
                        xuly = 7;
                    }
                }
            }
            var _now = new Date();  //current date of week
            var currentWeekDay = _now.getDay();
            var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1;
            var dayStart = '';
            var dayEnd = '';

            if (self.filterNgayLapHD() === '0') {

                switch (self.filterNgayLapHD_Quy()) {
                    case 0:
                        // all
                        self.TodayBC('Toàn thời gian');
                        dayStart = '2016-01-01';
                        dayEnd = '9999-01-01';
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
                        // quy truoc = currQuarter -1; // if (currQuarter -1 == 0) --> (assign = 1)
                        self.TodayBC('Quý trước');
                        var prevQuarter = (moment().quarter() - 1 === 0) ? 1 : moment().quarter() - 1;
                        dayStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                        dayEnd = moment().quarter(prevQuarter).endOf('quarter').add('days', 1).format('YYYY-MM-DD');
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

            $('.table-reponsive').gridLoader();
            ajaxHelper(CSKHUri + 'GetAllPhanHoiWhere?currentPage=' + self.currentPage() + '&pageSize=' + self.pageSize() + '&arrLoaiLichHen=' + self.MangLoaiPhanHoiFilter() + '&dayStart=' + dayStart + '&dayEnd=' + dayEnd + '&txtSearch=' + self.filter() + '&trangthai=' + statusInvoice + '&xuly=' + xuly + '&arrMangIDNhanVien=' + self.MangIDNhanVien() + '&iddonvi=' + idDonVi, 'GET').done(function (data) {
                self.PhanHois(data.LstData);
                self.TotalRecord(data.TotalRow);
                self.PageCount(data.PageCount);
                $('.table-reponsive').gridLoader({ show: false });
            })
        }
    }

    self.clickSearchPH = function () {
        SearchPhanHoi();
    }

    self.ckCao.subscribe(function (newVal) {
        self.currentPage(0);
        SearchPhanHoi();
    });

    self.ckThap.subscribe(function (newVal) {
        self.currentPage(0);
        SearchPhanHoi();
    });

    self.ckTB.subscribe(function (newVal) {
        self.currentPage(0);
        SearchPhanHoi();
    });

    self.ckChuaXuLy.subscribe(function (newVal) {
        self.currentPage(0);
        SearchPhanHoi();
    });

    self.ckDangXuLy.subscribe(function (newVal) {
        self.currentPage(0);
        SearchPhanHoi();
    });

    self.ckDaXong.subscribe(function (newVal) {
        self.currentPage(0);
        SearchPhanHoi();
    });

    $('#txtSearchPhanHoi').keypress(function (e) {
        if (e.keyCode === 13) {
            self.currentPage(0);
            SearchPhanHoi();
        }
    })
    self.ResetCurrentPage = function () {
        self.currentPage(0);
        SearchPhanHoi();
    };

    self.filterNgayLapHD.subscribe(function (newVal) {
        self.currentPage(0);
        SearchPhanHoi();
    });
    $('#txtNgayTaoInput').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        SearchPhanHoi();
    });

    $('.choseNgayTao li').on('click', function () {
        $('#txtNgayTao').val($(this).text());
        self.filterNgayLapHD_Quy($(this).val());
        self.currentPage(0);
        SearchPhanHoi();
    });

    self.PageResults = ko.computed(function () {
        if (self.PhanHois() !== null) {

            self.fromitem((self.currentPage() * self.pageSize()) + 1);

            if (((self.currentPage() + 1) * self.pageSize()) > self.PhanHois().length) {
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
            SearchPhanHoi();
        }
    };

    self.StartPage = function () {
        self.currentPage(0);
        SearchPhanHoi();
    }

    self.BackPage = function () {
        if (self.currentPage() > 1) {
            self.currentPage(self.currentPage() - 1);
            SearchPhanHoi();
        }
    }

    self.GoToNextPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.currentPage() + 1);
            SearchPhanHoi();
        }
    }

    self.EndPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.PageCount() - 1);
            SearchPhanHoi();
        }
    }
    self.GetClassHD = function (page) {
        return ((page.pageNumber - 1) === self.currentPage()) ? "click" : "";
    };

}
ko.applyBindings(new ViewModel());
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