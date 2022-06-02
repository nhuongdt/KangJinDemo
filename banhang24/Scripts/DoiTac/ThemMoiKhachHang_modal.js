var FileModel = function (filef, srcf) {
    var self = this;
    this.file = filef;
    this.URLAnh = srcf;
};

var FormModel_NewNguonKhach = function () {
    var self = this;
    self.ID = ko.observable();
    self.TenNguonKhach = ko.observable();

    self.SetData = function (item) {
        self.ID(item.ID);
        self.TenNguonKhach(item.TenNguonKhach);
    }
}

var FormModel_NewNhomDoiTuong = function () {
    var self = this;
    self.ID = ko.observable();
    self.TenNhomDoiTuong = ko.observable();
    self.LoaiDoiTuong = loaiDoiTuong;
    self.GhiChu = ko.observable();
    self.GiamGia = ko.observable();
    self.GiamGiaTheoPhanTram = ko.observable(true);

    self.SetData = function (item) {
        self.ID(item.ID);
        self.TenNhomDoiTuong(item.TenNhomDoiTuong);
        self.GhiChu(item.GhiChu);
        self.GiamGia(item.GiamGia);
        self.GiamGiaTheoPhanTram(item.GiamGiaTheoPhanTram);
    }
}

var FormModel_NewKhachHang = function () {
    var self = this;
    self.ID = ko.observable();
    self.MaDoiTuong = ko.observable();
    self.ID_NhomDoiTuong = ko.observable();
    self.ID_QuanHuyen = ko.observable();
    self.ID_TinhThanh = ko.observable();
    self.ID_NguonKhach = ko.observable();
    self.ID_NguoiGioiThieu = ko.observable();
    self.ID_NhanVienPhuTrach = ko.observable();
    self.LaCaNhan = ko.observable();

    self.TenDoiTuong = ko.observable();
    self.Email = ko.observable();
    self.DiaChi = ko.observable();
    self.DienThoai = ko.observable();
    self.NgaySinh_NgayTLap = ko.observable();
    self.GioiTinhNam = ko.observable(false);
    self.MaSoThue = ko.observable();
    self.GhiChu = ko.observable();
    self.LoaiDoiTuong = loaiDoiTuong;
    self.NoHienTai = ko.observable(0);
    self.NoCanTra = ko.observable(0);
    self.CongTy = ko.observable();
    self.DiaChiChiNhanh = ko.observable();
    self.DienThoaiChiNhanh = ko.observable();
    self.TongTichDiem = ko.observable();
    self.GhiChu = ko.observable();
    self.NgayGiaoDichGanNhat = ko.observable();

    self.ID_VungMien = ko.observable(); // used at DieuKien NangNhom
    self.ID_KhuVuc = ko.observable();
    self.DinhDang_NgaySinh = ko.observable();
    self.TongBan = ko.observable(0);
    self.TongMua = ko.observable(0);
    self.TongBanTruTraHang = ko.observable(0);
    self.SoLanMuaHang = ko.observable(0);
    $('#SL_TrangThaiKH').val(undefined);// add new: reset text TrangThai

    self.SetData = function (item) {
        self.ID(item.ID);
        self.CongTy(item.CongTy); // TenCuaHang
        self.DienThoaiChiNhanh(item.DienThoaiChiNhanh);
        self.DiaChiChiNhanh(item.DiaChiChiNhanh);

        // ID_NhomDoiTuong.toString() --> avoid ID_DoiTuong = 0 (nhom mac dinh)
        if (item.ID_NhomDoiTuong !== null && item.ID_NhomDoiTuong.toString().indexOf('0000') === -1) {
            self.ID_NhomDoiTuong(item.ID_NhomDoiTuong);
        }
        else {
            self.ID_NhomDoiTuong(undefined);
        }

        if (item.ID_QuanHuyen !== null && item.ID_QuanHuyen.indexOf('0000') === -1) {
            self.ID_QuanHuyen(item.ID_QuanHuyen);
        }
        else {
            self.ID_QuanHuyen(undefined);
        }

        if (item.ID_TinhThanh !== null && item.ID_TinhThanh.indexOf('0000') === -1) {
            self.ID_TinhThanh(item.ID_TinhThanh);
        }
        else {
            self.ID_TinhThanh(undefined);
        }

        if (item.ID_NguonKhach !== null && item.ID_NguonKhach.indexOf('0000') === -1) {
            self.ID_NguonKhach(item.ID_NguonKhach);
        }
        else {
            self.ID_NguonKhach(undefined);
        }

        if (item.ID_NguoiGioiThieu !== null && item.ID_NguoiGioiThieu.indexOf('0000') === -1) {
            self.ID_NguoiGioiThieu(item.ID_NguoiGioiThieu);
            $('#modalPopuplg_KH .txtKhachHang').val(item.NguoiGioiThieu);
        }
        else {
            self.ID_NguoiGioiThieu(undefined);
            $('#modalPopuplg_KH .txtKhachHang').val('');
        }

        if (item.ID_NhanVienPhuTrach !== null && item.ID_NhanVienPhuTrach.indexOf('0000') === -1) {
            self.ID_NhanVienPhuTrach(item.ID_NhanVienPhuTrach);
            let allNhanVien = newModal_AddKhachHang.NhanVienAllChiNhanh();
            let itemNV = $.grep(allNhanVien, function (x) {
                return x.ID === item.ID_NhanVienPhuTrach;
            });
            if (itemNV.length > 0) {
                $('#modalKH_txtNnguoiTao').val(itemNV[0].TenNhanVien);
            }
        }
        else {
            self.ID_NhanVienPhuTrach(undefined);
        }

        if (item.ID_TrangThai !== null) {
            $('#SL_TrangThaiKH').val(item.ID_TrangThai);
        }
        else {
            $('#SL_TrangThaiKH').val(undefined);
        }

        self.LaCaNhan(item.LaCaNhan);
        self.MaDoiTuong(item.MaDoiTuong);
        self.TenDoiTuong(item.TenDoiTuong);
        self.Email(item.Email);
        self.DiaChi(item.DiaChi);
        self.DienThoai(item.DienThoai);
        self.GioiTinhNam(item.GioiTinhNam);
        self.GhiChu(item.GhiChu);
        var ngaysinh = item.NgaySinh_NgayTLap;
        if (ngaysinh === null || ngaysinh === undefined) {
            ngaysinh = "";
        }
        else {
            ngaysinh = moment(item.NgaySinh_NgayTLap, "YYYY-MM-DD hh:mm:ss").format("DD/MM/YYYY");
        }
        self.NgaySinh_NgayTLap(ngaysinh);
        self.MaSoThue(item.MaSoThue);
        self.NoHienTai(item.NoHienTai);
        self.NoCanTra(item.NoCanTra);
        self.CongTy(item.CongTy);
        self.DienThoaiChiNhanh(item.DienThoaiChiNhanh);
        self.DiaChiChiNhanh(item.DiaChiChiNhanh);
        self.TongTichDiem(item.TongTichDiem);
        self.DinhDang_NgaySinh(item.DinhDang_NgaySinh);
        self.TongBan(item.TongBan);
        self.TongMua(item.TongMua);
        self.TongBanTruTraHang(item.TongBanTruTraHang);
        self.SoLanMuaHang(item.SoLanMuaHang);
        self.NgayGiaoDichGanNhat(item.NgayGiaoDichGanNhat);
    }
};

var PartialView_NewKhachHang = function () {
    var self = this;
    var DMDoiTuongUri = '/api/DanhMuc/DM_DoiTuongAPI/';
    var DMNhomDoiTuongUri = '/api/DanhMuc/DM_NhomDoiTuongAPI/';
    var DMNguonKhachUri = '/api/DanhMuc/DM_NguonKhachAPI/';
    //var DateNgaySinh;
    var user = $('#txtUserLogin').val(); // get at ViewBag
    var userID = $('#txtUserID').val();
    var idNhanVien = $('.idnhanvien').text();
    var idDonVi = $('#hd_IDdDonVi').val();
    var sLoai = 'khách hàng';
    if (loaiDoiTuong === 2) {
        sLoai = 'nhà cung cấp';
    }
    var today = new Date();
    self.DinhDangNgaySinh = ko.observable();
    self.TinhThanhs = ko.observableArray();
    self.NhomDoiTuongDB = ko.observableArray();
    self.checkEmail = ko.observable();
    self.NhomDoiTuongs = ko.observableArray();// used at modal _ThemMoiNCC
    self.NguonKhachs = ko.observableArray();
    self.QuanHuyens = ko.observableArray();
    self.VungMiens = ko.observableArray();
    self.HaveImage_Select = ko.observable();
    self.FilesSelect = ko.observableArray();
    self.IsInsertNguon = ko.observable();
    self.NguonKhachChosed = ko.observableArray();
    self.ListAllDoiTuong = ko.observableArray();
    self.NhomDoiTuongChosed = ko.observableArray();
    self.TrangThaiKhachHang = ko.observableArray();
    self.NhanViens = ko.observableArray();
    self.NhanVienAllChiNhanh = ko.observableArray();
    self.selectManyNhomDT = ko.observable();
    self.ListTypeNgaySinh = ko.observableArray([
        { Value: 'dd/MM/yyyy', Text: 'Theo ngày/tháng/năm' },
        { Value: 'dd/MM', Text: 'Theo ngày/tháng' },
        { Value: 'MM/yyyy', Text: 'Theo tháng/năm' },
        { Value: 'yyyy', Text: 'Theo năm' },
    ])
    self.autoUpdate = ko.observable(false);
    self.DoAction = ko.observable(0); // assign = true when delete/insert/update finished
    self.AddStatus = ko.observable(false);// insert (true), update(false)
    self.newDoiTuong = ko.observable(new FormModel_NewKhachHang());
    self.newNguonKhach = ko.observable(new FormModel_NewNguonKhach());
    self.newNhomDoiTuong = ko.observable(new FormModel_NewNhomDoiTuong());
    self.booleanAddNhomDT = ko.observable(false);
    self.checkAutoUapdate = ko.observable(false);
    self.DieuKienNangNhom = ko.observableArray();
    self.checkGTNam = ko.observable('1');
    self.checkGTNu = ko.observable('1');
    self.checkCapNhat = ko.observable('2');
    self.FileImgs = ko.observableArray();
    self.booleanAdd = ko.observableArray(false);
    self.DM_NhomDoiTuong_ChiTiets_Unique = ko.observableArray();
    self.NgaySinhOld_KhachHang = ko.observable();
    self.IsAddAtModal = ko.observable(false);
    self.ChiNhanhs = ko.observableArray();

    self.IsModalNguon_modalKH = ko.observable(false);
    self.IsInsertNguon = ko.observable(false);
    self.popNhomKH_ChiNhanhChosed = ko.observableArray();
    self.QuanLyKhachHangTheoDonVi = ko.observable(false);

    self.CustomerDoing = ko.observableArray();// doing insert/update
    self.SourceCustomerDoing = ko.observableArray();
    self.GroupDoing = ko.observableArray();

    var arrCompare = [
        { ID: 1, KieuSoSanh: '>' },
        { ID: 2, KieuSoSanh: '>=' },
        { ID: 3, KieuSoSanh: '=' },
        { ID: 4, KieuSoSanh: '<=' },
        { ID: 5, KieuSoSanh: '<' },
        { ID: 6, KieuSoSanh: 'Khác' },
    ]
    var arrCompare36 = [arrCompare[2], arrCompare[5]];

    self.MangSoSanh = ko.observableArray(arrCompare);
    self.MangDieuKien = ko.observableArray([
        { ID: 1, TenDieuKien: 'Tổng mua (trừ trả hàng)', },
        { ID: 2, TenDieuKien: 'Tổng mua', },
        { ID: 3, TenDieuKien: 'Thời gian mua hàng', },
        { ID: 4, TenDieuKien: 'Số lần mua hàng', },
        { ID: 5, TenDieuKien: 'Công nợ hiện tại', },
        { ID: 6, TenDieuKien: 'Tháng sinh', },
        { ID: 7, TenDieuKien: 'Tuổi', },
        { ID: 8, TenDieuKien: 'Giới tính', },
        { ID: 9, TenDieuKien: 'Khu vực', },
        { ID: 10, TenDieuKien: 'Vùng miền', },
    ]);
    self.pageThangSinh = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
    self.pageTS = ko.observable(self.pageThangSinh[0]);

    function getListQuanHuyen(id) {
        if (id !== undefined) {
            ajaxHelper(DMDoiTuongUri + "GetListQuanHuyen?idTinhThanh=" + id, 'GET').done(function (x) {
                if (x.data.length > 0) {
                    self.QuanHuyens(x.data);
                }
            });
        }
    }

    function Insert_HT_ThongBao(DM_DoiTuong) {
        // remind birthday customer
        var objAdd =
            {
                ID_DonVi: idDonVi,
                LoaiThongBao: 3,
                NoiDungThongBao: "<p onclick=\"loaddadoc('" + 'key' + "')\"> Khách hàng <a onclick=\"loadthongbao('3'," +
                "'" + DM_DoiTuong.MaDoiTuong + "', '" + 'key' + "')\">"
                + " <span class=\"blue\">" + DM_DoiTuong.TenDoiTuong + " </span>" + "</a> có sinh nhật hôm nay</p>",
                NguoiDungDaDoc: '',
            };
        ajaxHelper('/api/DanhMuc/HT_API/' + 'Post_HT_ThongBao', 'POST', objAdd).done(function (x) {

        })
    }

    function Delete_HT_ThongBao(maDoiTuong) {
        var ymd = moment(today).format('YYYY-MM-DD');
        var where = "  convert(varchar, getdate(), 23) = '" + ymd + "' AND LoaiThongBao= 3 and NoiDungThongBao like N'%" + maDoiTuong + "%'";
        ajaxHelper('/api/DanhMuc/HT_API/' + 'Delete_HT_ThongBao?where=' + where, 'DELETE').done(function (x) {
        })
    }

    self.GetTypeNgaySinh = function () {
        var type = self.newDoiTuong().DinhDang_NgaySinh();
        $('#lstTypeNgaySinh li').each(function () {
            $(this).children('i').remove();
        });
        switch (type) {
            case 'dd/MM':
                $('#lstTypeNgaySinh li:eq(1)').append(elmCheck);
                break;
            case 'MM/yyyy':
                $('#lstTypeNgaySinh li:eq(2)').append(elmCheck);
                break;
            case 'yyyy':
                $('#lstTypeNgaySinh li:eq(3)').append(elmCheck);
                break;
            default:
                $('#lstTypeNgaySinh li:eq(0)').append(elmCheck);
                break;
        }
    }

    self.ChangeType_NgaySinh = function (item) {
        self.newDoiTuong().DinhDang_NgaySinh(item.Value);
        self.GetTypeNgaySinh();
        switch (item.Value) {
            case 'dd/MM':
                DateNgaySinh.val(null);
                DateNgaySinh.datepicker("destroy");
                DateNgaySinh.mask('99/99').focus();
                break;
            case 'MM/yyyy':
                DateNgaySinh.val(null);
                DateNgaySinh.datepicker("destroy");
                DateNgaySinh.mask('99/9999').focus();
                break;
            case 'yyyy':
                DateNgaySinh.val(null);
                DateNgaySinh.datepicker("destroy");
                DateNgaySinh.mask('9999').focus();
                break;
            default:
                DateNgaySinh.val(null);
                refreshDate();
                DateNgaySinh.mask('99/99/9999').focus();
                break;
        }
    }

    self.fileSelect = function (elemet, event) {
        var files = event.target.files;// FileList object
        // Loop through the FileList and render image files as thumbnails.

        var countErrType = 0;
        var countErrSize = 0;
        var errFileSame = '';
        var err = '';

        // Check Type file & Size
        for (var i = 0; i < files.length; i++) {

            if (!files[i].type.match('image.*')) {
                countErrType += 1;
            }

            var size = parseFloat(files[i].size / 1024).toFixed(2);
            if (size > 2048) {
                countErrSize += 1;
            }

            // check trung ten file
            for (var j = 0; j < self.FileImgs().length; j++) {

                var arrPath = self.FileImgs()[j].URLAnh.split('/');
                var fileName = arrPath[arrPath.length - 1];

                if (fileName === files[i].name) {
                    errFileSame += files[i].name + ', ';
                }
            }
        }

        // remove comma ,
        if (errFileSame !== '') {
            errFileSame = errFileSame.substr(0, errFileSame.length - 2)
        }

        if (countErrType > 0) {
            err = countErrType + ' file chưa đúng định dạng. ';
        }

        if (countErrSize > 0) {
            if (countErrType > 0) {
                if (errFileSame === '') {
                    // err type + error size
                    err += '<br />' + countErrSize + ' file có dung lượng > 2MB';
                }
                else {
                    // err type + error size + error exist file
                    err += '<br />' + countErrSize + ' file có dung lượng > 2MB' + '<br />' + ' File ' + errFileSame + ' đã tồn tại';
                }
            }
            else {
                // err size
                if (errFileSame === '') {
                    err = countErrSize + ' file có dung lượng > 2MB'
                }
                else {
                    // err size + error exist file
                    err = countErrSize + ' file có dung lượng > 2MB' + '<br />' + 'File ' + errFileSame + ' đã tồn tại';
                }
            }
        }
        else {
            if (countErrType > 0) {
                if (errFileSame === '') {
                    // err type
                    err = err;
                }
                else {
                    // err type + error exist file
                    err += '<br />' + 'File ' + errFileSame + ' đã tồn tại';
                }
            }
            else {
                // not err
                if (errFileSame === '') {
                    err = '';
                }
                else {
                    // error exist file
                    err = 'File ' + errFileSame + ' đã tồn tại';
                }
            }
        }

        if (err !== '') {
            ShowMessage_Danger(err);
        }

        for (var i = 0; i < files.length; i++) {
            var f = files[i];

            // Only process image files.
            if (!f.type.match('image.*')) {
                continue;
            }
            var size = parseFloat(f.size / 1024).toFixed(2);

            if (size <= 2048) {
                var reader = new FileReader();
                // Closure to capture the file information.
                reader.onload = (function (theFile) {
                    return function (e) {
                        self.FilesSelect.push(new FileModel(theFile, e.target.result));
                    };
                })(f);

                // Read in the image file as a data URL.
                reader.readAsDataURL(f);
                self.HaveImage_Select(true);
            }
        }
    };

    self.ShowModal_InsertNguon_modalKH = function () {
        self.newNguonKhach(new FormModel_NewNguonKhach());
        self.IsModalNguon_modalKH(true);
        self.IsInsertNguon(true);
        $('#NguonKhach').modal('show');
    }

    self.InsertNguonKhach = function () {
        var tenNguon = self.newNguonKhach().TenNguonKhach();
        var _id = self.newNguonKhach().ID();

        if (tenNguon === undefined || tenNguon !== undefined && (Remove_LastComma(tenNguon) === '')) {
            ShowMessage_Danger('Vui lòng nhập tên nguồn khách');
            return false;
        }

        var DM_NguonKhach = {
            TenNguonKhach: tenNguon,
            NguoiTao: user,
        };

        if (self.IsInsertNguon()) {
            ajaxHelper(DMNguonKhachUri + 'PostDM_NguonKhachHang', 'POST', DM_NguonKhach).done(function (item) {
                self.NguonKhachs.unshift(item);
                self.SourceCustomerDoing(item);
                $('#NguonKhach').modal('hide');
                ShowMessage_Success('Thêm mới nguồn khách thành công');
            });
        }
        else {
            DM_NguonKhach.NguoiSua = userID;
            DM_NguonKhach.ID = _id;

            ajaxHelper(DMNguonKhachUri + 'PutDM_NguonKhachHang', 'PUT', DM_NguonKhach).done(function (item) {
                for (var i = 0; i < self.NguonKhachs().length; i++) {
                    if (self.NguonKhachs()[i].ID === _id) {
                        self.NguonKhachs.splice(i, 1);
                        break;
                    }
                }
                self.NguonKhachs.unshift(DM_NguonKhach);
                self.SourceCustomerDoing(DM_NguonKhach);

                $('#NguonKhach').modal('hide');
                ShowMessage_Success('Cập nhật nguồn khách thành công');
            });

            // after update Nguon, assign again IsInsertNguon = true;
            self.IsInsertNguon(true);
        }
    }

    self.CloseNguonKhach = function () {
        self.NguonKhachChosed([]);

        $('#ddlNguonKhach li').each(function () {
            $(this).find('.fa-check').remove();
        });
        $('#choose_NguonKhach').append('<input type="text" class="dropdown form-control" placeholder="Chọn nguồn">');
    }

    self.ChoseNguonKhach = function (item) {

        self.newDoiTuong().ID_NguonKhach(item.ID); // assign ID_NguonKhach --> add new KhachHang
        self.NguonKhachChosed(item);

        var idNguon = item.ID;
        // thêm dấu check vào đối tượng được chọn (OK)
        $('#ddlNguonKhach li').each(function () {

            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
                $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>');
            }
            else {
                $(this).find('.fa-check').remove();
            }
        });

        // add class 'choose-person' : overflow, set width li
        $('#choose_NguonKhach input').remove();
        $('#choose_NguonKhach').addClass('choose-person');
    }

    self.KeyUp_GiamGiaNhomKH = function () {
        var $this = $('#txtGiamGiaNhomKH');
        formatNumberObj($this);

        var valThis = formatNumberToFloat($this.val());
        if ($('#LaPhanTram').hasClass('active-re')) {
            if (valThis > 100) {
                $this.val(100);
            }
        }
    }

    self.choose_PhanTram = function (item) {
        var isPtramOld = $('#LaPhanTram').hasClass('active-re');
        if (isPtramOld) {
            self.newNhomDoiTuong().GiamGiaTheoPhanTram(false);
        }
        else {
            self.newNhomDoiTuong().GiamGiaTheoPhanTram(true);
        }
        event.stopImmediatePropagation();// avoid call again in jquery
    };

    function Enable_btnSaveNhomDT() {
        document.getElementById("btnLuuNhomDoiTuong").disabled = false;
        document.getElementById("btnLuuNhomDoiTuong").lastChild.data = "Lưu";
    }

    self.xoaNhomDT = function () {
        self.DoAction(5);  // delete nhomKH
        var idNhomDT = self.newNhomDoiTuong().ID();

        // DM_NhomDoiTuong: update TrangThai=0
        // DM_NhomDoiTuong_ChiTiet: delete all row have ID_NhomDoiTuong
        // DM_DoiTuong_Nhom: delete all row have ID_NhomDoiTuong
        dialogConfirm('Thông báo xóa', 'Bạn có chắc chắn muốn xóa nhóm ' + sLoai + ' tên: <b>' + self.newNhomDoiTuong().TenNhomDoiTuong() + ' </b> không?', function () {
            ajaxHelper(DMNhomDoiTuongUri + 'DeleteDM_NhomDoiTuong/' + idNhomDT, 'DELETE').done(function (msg) {
                $('#modalAddGroup').modal('hide');

                if (msg === "") {
                    var lstNhomDB = $.grep(self.NhomDoiTuongDB(), function (x) {
                        return x.ID !== idNhomDT;
                    });
                    self.NhomDoiTuongDB($.extend(true, [], lstNhomDB));
                    ShowMessage_Success('Xóa nhóm ' + sLoai + ' thành công ');
                }
                else {
                    ShowMessage_Danger(msg);
                }
            });
        })
    }

    self.addNhomDoiTuong_NangNhom = function () {
        self.DoAction(2);  // insert/update Nhom KH

        document.getElementById("btnLuuNhomDoiTuong").disabled = true;
        document.getElementById("btnLuuNhomDoiTuong").lastChild.data = " Đang lưu";

        var tenNhom = self.newNhomDoiTuong().TenNhomDoiTuong();
        var giamgia = formatNumberToFloat(self.newNhomDoiTuong().GiamGia());
        var ptram = self.newNhomDoiTuong().GiamGiaTheoPhanTram();
        var ghichu = self.newNhomDoiTuong().GhiChu();
        ghichu = ghichu === undefined ? '' : ghichu;

        if (tenNhom === '' || tenNhom === undefined) {
            ShowMessage_Danger('Vui lòng nhập tên nhóm khách hàng');
            Enable_btnSaveNhomDT();
            return;
        }
        if (giamgia > 100 & ptram === true) {
            ShowMessage_Danger('Giảm giá không được lớn hơn 100%');
            Enable_btnSaveNhomDT();
            return;
        }
        for (var i = 0; i < self.DieuKienNangNhom().length; i++) {
            var LoaiHinhThuc = self.DieuKienNangNhom()[i].LoaiHinhThuc;
            if ((LoaiHinhThuc === 1 || LoaiHinhThuc === 2 || LoaiHinhThuc === 4 || LoaiHinhThuc === 5 || LoaiHinhThuc === 7) && self.DieuKienNangNhom()[i].GiaTri === '') {
                ShowMessage_Danger('Vui lòng nhập giá trị ' + self.DieuKienNangNhom()[i].HinhThuc);
                Enable_btnSaveNhomDT();
                return;
            }
            else if ((LoaiHinhThuc === 3) && self.DieuKienNangNhom()[i].TimeBy === '') {
                ShowMessage_Danger('Vui lòng nhập giá trị ' + self.DieuKienNangNhom()[i].HinhThuc);
                Enable_btnSaveNhomDT();
                return;
            }
            else if ((LoaiHinhThuc === 9) && self.DieuKienNangNhom()[i].KhuVuc === null) {
                ShowMessage_Danger('Vui lòng nhập giá trị ' + self.DieuKienNangNhom()[i].HinhThuc);
                Enable_btnSaveNhomDT();
                return;
            }
            else if ((LoaiHinhThuc === 10) && self.DieuKienNangNhom()[i].VungMien === null) {
                ShowMessage_Danger('Vui lòng nhập giá trị ' + self.DieuKienNangNhom()[i].HinhThuc);
                Enable_btnSaveNhomDT();
                return;
            }
        }

        var tb = "Thêm mới";
        var idNhomDT = self.newNhomDoiTuong().ID();
        if (idNhomDT === undefined || idNhomDT === const_GuidEmpty) {
            idNhomDT = null;
        }
        if (idNhomDT !== null) {
            tb = "Cập nhật";
        }
        var DM_NhomDoiTuong = {
            ID: idNhomDT,
            TenNhomDoiTuong: tenNhom,
            LoaiDoiTuong: 1,
            GhiChu: ghichu,
            TenNhomDoiTuong_KhongDau: locdau(tenNhom),
            TenNhomDoiTuong_KyTuDau: GetChartStart(tenNhom),
            TuDongCapNhat: self.autoUpdate(),
            GiamGia: giamgia,
            GiamGiaTheoPhanTram: ptram,
            NhomDT_DonVi: self.popNhomKH_ChiNhanhChosed(),
        };
        // add to search in Vue
        DM_NhomDoiTuong.Text_Search = tenNhom.concat(DM_NhomDoiTuong.TenNhomDoiTuong_KhongDau, ' ', DM_NhomDoiTuong.TenNhomDoiTuong_KyTuDau);

        // save NhomDoiTuong_DonVi
        var arrID = [];
        if (self.popNhomKH_ChiNhanhChosed().length === 0) {
            // get all ChiNhanh
            for (var i = 0; i < self.ChiNhanhs().length; i++) {
                arrID.push(self.ChiNhanhs()[i].ID);
            }
        }
        else {
            for (var i = 0; i < self.popNhomKH_ChiNhanhChosed().length; i++) {
                arrID.push(self.popNhomKH_ChiNhanhChosed()[i].ID);
            }
        }

        // format date in list DKNangNhom
        for (var i = 0; i < self.DieuKienNangNhom().length; i++) {
            if (self.DieuKienNangNhom()[i].TimeBy !== null) {
                self.DieuKienNangNhom()[i].TimeBy = moment(self.DieuKienNangNhom()[i].TimeBy, 'DD/MM/YYYY').format('YYYY-MM-DD');
            }
        }

        var myData = {};
        myData.objNhomDoiTuong = [DM_NhomDoiTuong];
        myData.objNhomDoiTuongChiTiet = self.DieuKienNangNhom();
        myData.lstIDChiNhanh = arrID;
        console.log('myData', myData);

        // self.autoUpdate(): rdcokhong1 (true: HeThongCapNhat, false: HeThongkhongCapNhat)
        $.ajax({
            data: myData,
            url: DMDoiTuongUri + "Creater_NangNhomDoiTuong_ChiNhanh?ID_NhomDoiTuong=" + idNhomDT + "&User=" + user
            + "&Autocheck=" + self.autoUpdate() + "&ID_DonVi=" + idDonVi + "&ID_NhanVien=" + idNhanVien + "&phuongthuc=" + self.checkCapNhat(),
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",

            success: function (item) {
                if (idNhomDT !== null) {
                    for (var i = 0; i < self.NhomDoiTuongDB().length; i++) {
                        if (self.NhomDoiTuongDB()[i].ID === idNhomDT) {
                            self.NhomDoiTuongDB.remove(self.NhomDoiTuongDB()[i]);
                            break;
                        }
                    }
                    self.NhomDoiTuongDB.unshift(DM_NhomDoiTuong);
                }
                else {
                    DM_NhomDoiTuong.ID = item;
                    self.NhomDoiTuongDB.unshift(DM_NhomDoiTuong);

                    // set newNhom into NhomDoiTuongChosed
                    if (self.IsAddAtModal()) {
                        self.newDoiTuong().ID_NhomDoiTuong(item);
                        for (var i = 0; i < self.NhomDoiTuongChosed().length; i++) {
                            var objNhom = {
                                ID: self.NhomDoiTuongChosed()[i].ID,
                                TenNhomDoiTuong: self.NhomDoiTuongChosed()[i].TenNhomDoiTuong
                            };
                            self.selectManyNhomDT(objNhom);
                        }
                        var objNhom2 = {
                            ID: item,
                            TenNhomDoiTuong: DM_NhomDoiTuong.TenNhomDoiTuong,
                        };
                        self.selectManyNhomDT(objNhom2);
                    }
                }
                self.GroupDoing(DM_NhomDoiTuong);

                // checkCapNhat(): rdcokhong (2. CapNhat, 3. KhongCapNhat)
                if (self.checkCapNhat() === '3') {
                    ShowMessage_Success(tb + ' nhóm ' + sLoai + ' thành công');
                }
                else {
                    var ID = idNhomDT;
                    if (idNhomDT === null)
                        ID = item;
                    $.ajax({
                        data: myData,
                        url: DMDoiTuongUri + "insert_DoiTuong_Nhom?ID_NhomDoiTuong=" + ID + "&LoaiCapNhat=" + self.checkCapNhat(),
                        type: 'POST',
                        async: true,
                        dataType: 'json',
                        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                        success: function (item) {
                            ShowMessage_Success(tb + ' nhóm ' + sLoai + ' thành công');
                        },
                    });
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                ShowMessage_Danger(tb + ' nhóm ' + sLoai + 'không thành công')
            },
            complete: function () {
                $("#modalAddGroup").modal("hide");
                Enable_btnSaveNhomDT();
            }
        })
    }

    // add NhomNCC in modal 
    self.showpopAddNhomNCC_popup = function () {
        self.IsAddAtModal(true);
        self.booleanAddNhomDT(true);
        self.newNhomDoiTuong(new FormModel_NewNhomDoiTuong());
        $('#modalNhomNCC').modal('show');
    };

    // add nhom ncc
    self.addNhomDoiTuong = function () {
        self.DoAction(2);//  insert/update NhomNCC

        document.getElementById("btnLuuNhomDoiTuong").disabled = true;
        document.getElementById("btnLuuNhomDoiTuong").lastChild.data = " Đang lưu";
        var _id = self.newNhomDoiTuong().ID();
        var _tenNhomDoiTuong = self.newNhomDoiTuong().TenNhomDoiTuong();
        var _ghiChu = self.newNhomDoiTuong().GhiChu();

        if (_tenNhomDoiTuong === '' || _tenNhomDoiTuong === undefined) {
            ShowMessage_Danger('Vui lòng nhập tên nhóm ' + sLoai);
            Enable_btnSaveNhomDT();
            return;
        }

        var DM_NhomDoiTuong = {
            ID: _id,
            TenNhomDoiTuong: _tenNhomDoiTuong,
            LoaiDoiTuong: loaiDoiTuong,
            GhiChu: _ghiChu,
            TenNhomDoiTuong_KhongDau: locdau(_tenNhomDoiTuong),
            TenNhomDoiTuong_KyTuDau: GetChartStart(_tenNhomDoiTuong),
            NguoiTao: user,
        };
        if (self.booleanAddNhomDT() === true) {
            $.ajax({
                data: DM_NhomDoiTuong,
                url: DMNhomDoiTuongUri + "PostDM_NhomDoiTuong",
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (item) {
                    DM_NhomDoiTuong.ID = item.ID;
                    self.NhomDoiTuongs.unshift(item);
                    if (self.IsAddAtModal()) {
                        self.newDoiTuong().ID_NhomDoiTuong(item.ID);
                    }
                    self.GroupDoing(DM_NhomDoiTuong);
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    Enable_btnSaveNhomDT();
                },
                complete: function () {
                    $("#modalNhomNCC").modal("hide");
                    Enable_btnSaveNhomDT();
                    ShowMessage_Success('Thêm mới nhóm ' + sLoai + ' thành công');
                }
            })
        }
        // edit
        else {
            DM_NhomDoiTuong.NguoiSua = user;
            var myData = {
                id: _id,
                objNhomDoiTuong: DM_NhomDoiTuong,
            };
            self.GroupDoing(DM_NhomDoiTuong);

            $.ajax({
                url: DMNhomDoiTuongUri + "PutDM_NhomDoiTuong",
                type: 'PUT',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: myData,
                success: function () {
                    ShowMessage_Success('Cập nhật nhóm ' + sLoai + ' thành công');
                    // only at modal NCC
                    for (var i = 0; i < self.NhomDoiTuongs().length; i++) {
                        if (self.NhomDoiTuongs()[i].ID === _id) {
                            self.NhomDoiTuongs.remove(self.NhomDoiTuongs()[i]);
                            break;
                        }
                    }
                    self.NhomDoiTuongs.push(DM_NhomDoiTuong);
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    ShowMessage_Danger('Cập nhật nhóm ' + sLoai + ' thất bại');
                },
                complete: function () {
                    Enable_btnSaveNhomDT();
                    $("#modalNhomNCC").modal("hide");
                }
            })
        }
    }

    self.refreshTab = function () {
        $('.nav-tabs li').each(function () {
            $(this).removeClass('active');
        });
        $('#tabThongTin').addClass('active');
        $('#thietlap').removeClass('active');
        $('#capnhap').removeClass('active');
        $('#thongtin').addClass('active');
        $('#thongtin').addClass('in');
        self.checkCapNhat('2');
    }

    // click add NhomKH at modal them moi KH
    self.ShowModalAddNhomKH_popup = function () {
        //if (self.RoleInsert_Cus()) {
        $('#modalAddGroup').modal('show');
        self.booleanAddNhomDT(true);
        self.IsAddAtModal(true);
        self.autoUpdate(false);
        self.newNhomDoiTuong(new FormModel_NewNhomDoiTuong());
        self.DieuKienNangNhom([]);
        self.refreshTab();
        //}
        //else {
        //    ShowMessage_Danger('Không có quyền thêm mới nhóm khách hàng');
        //}
    }

    self.newDoiTuong().ID_TinhThanh.subscribe(function (newValue) {
        if (newValue !== undefined) {
            getListQuanHuyen(newValue);
            self.newDoiTuong().ID_TinhThanh(newValue);
        }
    });

    self.SetCheck_MangDieuKien = function (item) {
        var ddlDK = $(event.currentTarget).parent().next('#selec-all-DieuKien');
        $(ddlDK).find('li').each(function () {
            $(this).find('i').remove();
            if ($(this).attr('id') === item.LoaiHinhThuc.toString()) {
                $(this).append(elmCheck);
            }
        });
    }

    function AssignAgain_ListDKNangNhom() {
        var arr = self.DieuKienNangNhom();
        self.DieuKienNangNhom($.extend(true, [], arr));
    }

    self.DieuKienNangNhom_ChangeGiaTri = function (item) {
        var $this = $(event.currentTarget);
        formatNumberObj($this);

        var gtri = formatNumberToFloat($this.val());
        for (var i = 0; i < self.DieuKienNangNhom().length; i++) {
            if (self.DieuKienNangNhom()[i].IDRandom === item.IDRandom) {
                self.DieuKienNangNhom()[i].GiaTri = gtri;
                break;
            }
        }
    }

    self.DKNangNhom_ChangeKhuVuc = function (item) {
        var idProvince = self.newDoiTuong().ID_KhuVuc();
        var province = $.grep(self.TinhThanhs(), function (x) {
            return x.ID === idProvince;
        });
        if (province.length > 0) {
            for (var i = 0; i < self.DieuKienNangNhom().length; i++) {
                if (self.DieuKienNangNhom()[i].IDRandom === item.IDRandom) {
                    self.DieuKienNangNhom()[i].ID_KhuVuc = idProvince;
                    self.DieuKienNangNhom()[i].KhuVuc = province[0].TenTinhThanh;
                    break;
                }
            }
        }
    }

    self.DKNangNhom_ChangeVungMien = function (item) {
        var idVung = self.newDoiTuong().ID_VungMien();
        var vung = $.grep(self.VungMiens(), function (x) {
            return x.ID === idVung;
        });
        if (vung.length > 0) {
            for (var i = 0; i < self.DieuKienNangNhom().length; i++) {
                if (self.DieuKienNangNhom()[i].IDRandom === item.IDRandom) {
                    self.DieuKienNangNhom()[i].ID_VungMien = idVung;
                    self.DieuKienNangNhom()[i].VungMien = vung[0].TenVung;
                    break;
                }
            }
        }
    }

    self.newDoiTuong().ID_NguoiGioiThieu.subscribe(function (newVal) {
        self.newDoiTuong().ID_NguoiGioiThieu(newVal);
    });

    self.newDoiTuong().ID_NhanVienPhuTrach.subscribe(function (newVal) {
        self.newDoiTuong().ID_NhanVienPhuTrach(newVal);
    });

    self.newDoiTuong().ID_QuanHuyen.subscribe(function (newVal) {
        self.newDoiTuong().ID_QuanHuyen(newVal);
    });

    self.filterProvince = function (item, inputString) {
        var itemSearch = locdau(item.TenTinhThanh);
        var locdauInput = locdau(inputString);
        // nen bat chat khong cho nhap dau cach o MaKH
        var arr = itemSearch.split(/\s+/);
        var sThreechars = '';

        for (var i = 0; i < arr.length; i++) {
            sThreechars += arr[i].toString().split('')[0];
        }

        return itemSearch.indexOf(locdauInput) > -1 ||
            sThreechars.indexOf(locdauInput) > -1;
    }

    self.filterProvinceVM = function (item, inputString) {
        var itemSearch = locdau(item.TenVung);
        var locdauInput = locdau(inputString);
        // nen bat chat khong cho nhap dau cach o MaKH
        var arr = itemSearch.split(/\s+/);
        var sThreechars = '';

        for (var i = 0; i < arr.length; i++) {
            sThreechars += arr[i].toString().split('')[0];
        }
        return itemSearch.indexOf(locdauInput) > -1 ||
            sThreechars.indexOf(locdauInput) > -1;
    }

    self.filterDistrict = function (item, inputString) {

        var itemSearch = locdau(item.TenQuanHuyen);
        var locdauInput = locdau(inputString);
        // nen bat chat khong cho nhap dau cach o MaKH
        var arr = itemSearch.split(/\s+/);
        var sThreechars = '';

        for (var i = 0; i < arr.length; i++) {
            sThreechars += arr[i].toString().split('')[0];
        }
        return itemSearch.indexOf(locdauInput) > -1 ||
            sThreechars.indexOf(locdauInput) > -1;
    }

    self.filterDoiTuong = function (item, inputString) {

        var itemSearch = locdau(item.TenDoiTuong);
        var itemSearch2 = locdau(item.MaDoiTuong);
        var locdauInput = locdau(inputString);
        // nen bat chat khong cho nhap dau cach o MaKH
        var arr = itemSearch.split(/\s+/);
        var sThreechars = '';

        for (var i = 0; i < arr.length; i++) {
            sThreechars += arr[i].toString().split('')[0];
        }
        return itemSearch.indexOf(locdauInput) > -1 || itemSearch2.indexOf(locdauInput) > -1 ||
            sThreechars.indexOf(locdauInput) > -1;
    }

    self.filterNhanVien = function (item, inputString) {

        var itemSearch = locdau(item.TenNhanVien);
        var itemSearch2 = locdau(item.MaNhanVien);
        var locdauInput = locdau(inputString);
        // nen bat chat khong cho nhap dau cach o MaKH
        var arr = itemSearch.split(/\s+/);
        var sThreechars = '';

        for (var i = 0; i < arr.length; i++) {
            sThreechars += arr[i].toString().split('')[0];
        }
        return itemSearch.indexOf(locdauInput) > -1 || itemSearch2.indexOf(locdauInput) > -1 ||
            sThreechars.indexOf(locdauInput) > -1;
    }

    self.filterNguonKH = function (item, inputString) {

        var itemSearch = locdau(item.TenNguonKhach);
        var locdauInput = locdau(inputString);
        // nen bat chat khong cho nhap dau cach o MaKH
        var arr = itemSearch.split(/\s+/);
        var sThreechars = '';

        for (var i = 0; i < arr.length; i++) {
            sThreechars += arr[i].toString().split('')[0];
        }
        return itemSearch.indexOf(locdauInput) > -1 ||
            sThreechars.indexOf(locdauInput) > -1;
    }

    function Enable_btnSaveDoiTuong() {
        document.getElementById("btnLuuDoiTuong").disabled = false;
        document.getElementById("btnLuuDoiTuong").lastChild.data = "Lưu";
    }

    // insert/update Nha cung cap
    self.addKH_NCC = function (formElement) {
        self.DoAction(1);
        document.getElementById("btnLuuDoiTuong").disabled = true;
        document.getElementById("btnLuuDoiTuong").lastChild.data = " Đang lưu";

        var _id = self.newDoiTuong().ID();
        var _tenDoiTuong = self.newDoiTuong().TenDoiTuong();
        var _idTinhThanh = self.newDoiTuong().ID_TinhThanh();
        var _idQuanHuyen = self.newDoiTuong().ID_QuanHuyen();
        var _maDT = self.newDoiTuong().MaDoiTuong();
        var _laCaNhan = self.newDoiTuong().LaCaNhan();
        var _noHienTai = self.newDoiTuong().NoHienTai();
        var _tongMua = self.newDoiTuong().TongMua();
        var _idNhomDT = self.newDoiTuong().ID_NhomDoiTuong();

        var msgCheck = CheckInput(self.newDoiTuong());
        if (msgCheck !== '') {
            ShowMessage_Danger(msgCheck);
            Enable_btnSaveDoiTuong();
            return false;
        }

        var DM_DoiTuong = {
            ID: _id,
            ID_NhomDoiTuong: null, // not use this field
            MaDoiTuong: self.newDoiTuong().MaDoiTuong(),
            TenDoiTuong: _tenDoiTuong,
            TenDoiTuong_KhongDau: locdau(_tenDoiTuong),
            TenDoiTuong_ChuCaiDau: GetChartStart(_tenDoiTuong),
            DienThoai: self.newDoiTuong().DienThoai(),
            Email: self.newDoiTuong().Email(),
            DiaChi: self.newDoiTuong().DiaChi(),
            GioiTinhNam: true,
            MaSoThue: self.newDoiTuong().MaSoThue(),
            LoaiDoiTuong: loaiDoiTuong,
            GhiChu: self.newDoiTuong().GhiChu(),
            LaCaNhan: _laCaNhan,
            ID_TinhThanh: _idTinhThanh,
            ID_QuanHuyen: _idQuanHuyen,
            ID_DonVi: idDonVi,
            NguoiTao: user, // user dang nhap
            ID_NhanVienPhuTrach: idNhanVien, // default NVPhuTrach = NV login

            // get to do bind after update
            TongMua: _tongMua,
            NoHienTai: _noHienTai,
        };
        console.log('DM_DoiTuong', DM_DoiTuong, 'idNhom ', _idNhomDT);

        if (navigator.onLine) {
            // insert Nha cung cap
            if (self.booleanAdd() === true) {

                $.ajax({
                    data: DM_DoiTuong,
                    url: DMDoiTuongUri + "PostDM_DoiTuong",
                    type: 'POST',
                    async: true,
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (obj) {
                        if (obj.res === true) {
                            var item = obj.data;
                            // insert nhomNCC in DM_DoiTuong_Nhom (one group)
                            var lstDM_DoiTuong_Nhom = [];
                            var tenNhom = 'Nhóm mặc định';
                            // 0: Nhom mac dinh
                            if (_idNhomDT !== null && _idNhomDT !== undefined && _idNhomDT !== 0) {
                                var objDTNhom = {
                                    ID_DoiTuong: item.ID,
                                    ID_NhomDoiTuong: _idNhomDT,
                                }
                                lstDM_DoiTuong_Nhom.push(objDTNhom);
                                self.Insert_ManyNhom(lstDM_DoiTuong_Nhom);

                                // get tenNhom 
                                var itemNhom = $.grep(self.NhomDoiTuongDB(), function (x) {
                                    return x.ID === _idNhomDT;
                                });

                                if (itemNhom.length > 0) {
                                    tenNhom = itemNhom[0].TenNhomDoiTuong;
                                }
                            }

                            DM_DoiTuong.ID = item.ID;
                            DM_DoiTuong.MaDoiTuong = item.MaDoiTuong;
                            DM_DoiTuong.TenNhomDT = tenNhom;
                            DM_DoiTuong.ID_NhomDoiTuong = _idNhomDT;
                            self.CustomerDoing(DM_DoiTuong);

                            ShowMessage_Success("Thêm mới " + sLoai + " thành công");
                        }
                        else {
                            ShowMessage_Danger(obj.mes);
                        }
                        $("#modalPopuplg_NCC").modal("hide");
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        ShowMessage_Danger('Thêm mới ' + sLoai + ' thất bại');
                    },
                    complete: function () {
                        Enable_btnSaveDoiTuong();
                    }
                })
            }
            // update Nha cung cap
            else {
                DM_DoiTuong.NguoiSua = user;
                var myData = {
                    id: _id,
                    objDoiTuong: DM_DoiTuong,
                };

                $.ajax({
                    url: DMDoiTuongUri + "PutDM_DoiTuong",
                    type: 'PUT',
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    data: myData,
                    success: function (obj) {
                        if (obj.res === true) {
                            var item = obj.data;
                            // update DM_DoiTuong_Nhom (many Group)
                            var lstDM_DoiTuong_Nhom = [];
                            var tenNhom = 'Nhóm mặc định';
                            if (_idNhomDT !== null && _idNhomDT !== undefined && _idNhomDT !== 0) {
                                var objDTNhom = {
                                    ID_DoiTuong: item.ID,
                                    ID_NhomDoiTuong: _idNhomDT,
                                }
                                lstDM_DoiTuong_Nhom.push(objDTNhom);

                                // get tenNhom 
                                var itemNhom = $.grep(self.NhomDoiTuongDB(), function (x) {
                                    return x.ID === _idNhomDT;
                                });

                                if (itemNhom.length > 0) {
                                    tenNhom = itemNhom[0].TenNhomDoiTuong;
                                }

                                self.Update_ManyNhom(lstDM_DoiTuong_Nhom, false);
                            }

                            DM_DoiTuong.MaDoiTuong = item.MaDoiTuong;
                            DM_DoiTuong.TenNhomDT = tenNhom;
                            DM_DoiTuong.ID_NhomDoiTuong = _idNhomDT;
                            self.CustomerDoing(DM_DoiTuong);

                            Enable_btnSaveDoiTuong();
                            $("#modalPopuplg_NCC").modal("hide");
                        }
                        else {
                            ShowMessage_Danger(obj.mes);
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        ShowMessage_Danger('Cập nhật nhà cung cấp thất bại');
                    },
                    complete: function () {
                        Enable_btnSaveDoiTuong();
                    }
                })
            }
        }
        else {
            self.CustomerDoing(DM_DoiTuong);
        }
        $('.line-right').height(0).css("margin-top", "0px");
    }

    // insert/update KhachHang
    self.addKhachHang = function (formElement) {
        document.getElementById("btnLuuDoiTuong").disabled = true;
        document.getElementById("btnLuuDoiTuong").lastChild.data = " Đang lưu";
        self.DoAction(1);  // insert/update KH

        var _id = self.newDoiTuong().ID();
        var _tenDoiTuong = self.newDoiTuong().TenDoiTuong();
        var _ngaySinh = self.newDoiTuong().NgaySinh_NgayTLap();
        var _maDT = self.newDoiTuong().MaDoiTuong();
        var _laCaNhan = self.newDoiTuong().LaCaNhan();
        var _typeNgaySinh = self.newDoiTuong().DinhDang_NgaySinh();
        var _noHienTai = self.newDoiTuong().NoHienTai();
        var _tongTichDiem = self.newDoiTuong().TongTichDiem();
        var _tongBan = self.newDoiTuong().TongBan();
        var _tongMua = self.newDoiTuong().TongMua();
        var _tongBanTruTraHang = self.newDoiTuong().TongBanTruTraHang();
        var _solanMuahang = self.newDoiTuong().SoLanMuaHang();

        var _idTinhThanh = self.newDoiTuong().ID_TinhThanh();
        _idTinhThanh = (_idTinhThanh === undefined || _idTinhThanh === '' ? null : _idTinhThanh);

        var _idQuanHuyen = self.newDoiTuong().ID_QuanHuyen();
        _idQuanHuyen = (_idQuanHuyen === undefined || _idQuanHuyen === '' ? null : _idQuanHuyen);

        var _idNguonKhach = self.newDoiTuong().ID_NguonKhach();
        _idNguonKhach = (_idNguonKhach === undefined || _idNguonKhach === '' ? null : _idNguonKhach);

        var _idNguoigioiThieu = self.newDoiTuong().ID_NguoiGioiThieu();
        _idNguoigioiThieu = (_idNguoigioiThieu === undefined || _idNguoigioiThieu === '' ? null : _idNguoigioiThieu);

        var _idNVienPhuTrach = self.newDoiTuong().ID_NhanVienPhuTrach();
        _idNVienPhuTrach = (_idNVienPhuTrach === undefined || _idNVienPhuTrach === '' ? null : _idNVienPhuTrach);

        var dtNow = new Date();
        var _yearNow = dtNow.getFullYear();

        // cho phep nhap NgaySinh = null
        if (_ngaySinh === undefined || _ngaySinh === 'Invalid date' || _ngaySinh === '') {
            self.newDoiTuong().NgaySinh_NgayTLap(null);
            _typeNgaySinh = null;
        }

        var msgCheck = CheckInput(self.newDoiTuong());
        if (msgCheck !== '') {
            ShowMessage_Danger(msgCheck);
            Enable_btnSaveDoiTuong();
            return false;
        }

        // if update: ngaysinh = dd/MM/yyyy
        if (_id !== null && _id !== undefined) {
            if (_ngaySinh !== null && _ngaySinh !== undefined) {
                if (_ngaySinh.length === 10) {
                    switch (_typeNgaySinh) {
                        case 'dd/MM':
                            if (_ngaySinh.substr(6, 4) !== _yearNow) {
                                _typeNgaySinh = 'dd/MM/yyyy';
                            }
                            else {
                                _ngaySinh = _ngaySinh.substr(0, 5);
                            }
                            break;
                        case 'MM/yyyy':
                            if (_ngaySinh.substr(0, 2) !== '01') {
                                _typeNgaySinh = 'dd/MM/yyyy';
                            }
                            else {
                                _ngaySinh = _ngaySinh.substr(3, 7);
                            }
                            break;
                        case 'yyyy':
                            if (_ngaySinh.substr(0, 5) !== '01/01') {
                                _typeNgaySinh = 'dd/MM/yyyy';
                            }
                            else {
                                _ngaySinh = _ngaySinh.substr(6, 4);
                            }
                            break;
                        default:
                            _ngaySinh = _ngaySinh;
                            _typeNgaySinh = 'dd/MM/yyyy';
                    }
                }
            }
        }

        switch (_typeNgaySinh) {
            case 'dd/MM':
                _ngaySinh = _ngaySinh + '/' + _yearNow;
                break;
            case 'MM/yyyy':
                _ngaySinh = '01/' + _ngaySinh;
                break;
            case 'yyyy':
                _ngaySinh = '01/01/' + _ngaySinh;
                break;
            case null:
                _ngaySinh = null;
            default:
                _typeNgaySinh = 'dd/MM/yyyy';
                break;
        }

        if (_ngaySinh !== null) {
            _ngaySinh = moment(_ngaySinh, 'DD/MM/YYYY').format('YYYY-MM-DD');

            var checkNS = CheckNgaySinh(_ngaySinh);
            if (!checkNS) {
                Enable_btnSaveDoiTuong();
                return;
            }
        }

        if (_idNguonKhach === undefined) {
            _idNguonKhach = null;
        }
        var tenTrangThai = '';
        var idTrangThai = $('#SL_TrangThaiKH').val();
        idTrangThai = idTrangThai === '' ? null : idTrangThai;
        var itemCusType = $.grep(self.TrangThaiKhachHang(), function (x) {
            return x.ID === idTrangThai;
        });
        if (itemCusType.length > 0) {
            tenTrangThai = itemCusType[0].Name;
        }

        var DM_DoiTuong = {
            ID: _id,
            ID_NhomDoiTuong: null, // not use this field
            MaDoiTuong: self.newDoiTuong().MaDoiTuong(),
            TenDoiTuong: _tenDoiTuong,
            DienThoai: self.newDoiTuong().DienThoai(),
            Email: self.newDoiTuong().Email(),
            DiaChi: self.newDoiTuong().DiaChi(),
            GioiTinhNam: self.newDoiTuong().GioiTinhNam(),
            NgaySinh_NgayTLap: _ngaySinh,
            MaSoThue: self.newDoiTuong().MaSoThue(),
            LoaiDoiTuong: loaiDoiTuong,
            GhiChu: self.newDoiTuong().GhiChu(),
            ID_NguonKhach: _idNguonKhach,  // get ID_NguonKhach from NguonKach Insert
            ID_NguoiGioiThieu: _idNguoigioiThieu,
            ID_NhanVienPhuTrach: _idNVienPhuTrach,
            LaCaNhan: _laCaNhan,
            ID_TinhThanh: _idTinhThanh,
            ID_QuanHuyen: _idQuanHuyen,
            ID_DonVi: idDonVi,
            NguoiTao: user, // user dang nhap
            DinhDang_NgaySinh: _typeNgaySinh,

            // get to do bind after update
            TongBan: _tongBan,
            TongMua: _tongMua,
            NoHienTai: _noHienTai,
            TongBanTruTraHang: _tongBanTruTraHang,
            TongTichDiem: _tongTichDiem,
            SoLanMuaHang: _solanMuahang,
            ID_TrangThai: idTrangThai,
            TrangThaiKhachHang: tenTrangThai, // bind in list
            TenDoiTuong_ChuCaiDau: GetChartStart(_tenDoiTuong),
            TenDoiTuong_KhongDau: locdau(_tenDoiTuong),
            NgayGiaoDichGanNhat: self.newDoiTuong().NgayGiaoDichGanNhat(),
        };

        console.log('DM_DoiTuong', DM_DoiTuong);

        if (navigator.onLine) {
            // insert DoiTuong
            if (self.booleanAdd() === true) {
                callAjaxAdd(DM_DoiTuong);
            }
            // update DoiTuong
            else {
                DM_DoiTuong.NguoiSua = user;
                var myData = {
                    id: _id,
                    objDoiTuong: DM_DoiTuong,
                };
                callAjaxUpdate(myData);
            }
        }
        else {
            ShowMessage_Danger('Không có kết nối Internet. Vui lòng thêm mới sau');
            Enable_btnSaveDoiTuong();
            return false;
        }
        $('.line-right').height(0).css("margin-top", "0px");
    }

    function CheckInput(obj) {

        var sReturn = '';
        var id = obj.ID();
        var maDT = obj.MaDoiTuong();
        var tenDT = obj.TenDoiTuong();
        var date1 = obj.NgaySinh_NgayTLap();
        var date2 = moment(new Date()).format('YYYY-MM-DD');
        var email = obj.Email();
        var phone = obj.DienThoai();
        var idTinhThanh = obj.ID_TinhThanh();
        var idQuanHuyen = obj.ID_QuanHuyen();
        var idNguoiGioiThieu = obj.ID_NguoiGioiThieu();
        var idNguoiQuanLy = obj.ID_NhanVienPhuTrach();

        if (tenDT === null || tenDT === "" || tenDT === undefined) {
            sReturn = 'Vui lòng nhập tên ' + sLoai + '  <br />';
        }

        // check MaKhachHang nhập kí tự đặc biệt 
        if (CheckChar_Special(maDT)) {
            sReturn += 'Mã ' + sLoai + ' không được chứa kí tự đặc biệt <br />';
        }

        if (email !== '' && email !== undefined && email !== null) {
            var re = /^(([^<>()[\]\\.,;:\s@#ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴ\"]+(\.[^<>()[\]\\.,;:\s@#ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴ\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/i;
            var valReturn = re.test(email);
            if (valReturn === false) {
                sReturn += 'Email không hợp lệ <br />';
            }
        }

        if (Is_undefined_empty_GuidEmpty(idTinhThanh) === false) {
            var itemTT = $.grep(self.TinhThanhs(), function (item) {
                return item.ID === idTinhThanh;
            });
            if (itemTT.length === 0) {
                sReturn += 'Tỉnh thành không tồn tại trong hệ thống <br />';
            }
        }

        if (Is_undefined_empty_GuidEmpty(idQuanHuyen) === false) {
            var itemQH = $.grep(self.QuanHuyens(), function (item) {
                return item.ID === idQuanHuyen;
            });
            if (itemQH.length === 0) {
                sReturn += 'Quận huyện không tồn tại trong hệ thống <br />';
            }
        }

        if (Is_undefined_empty_GuidEmpty(idNguoiQuanLy) === false) {
            var itemNV = $.grep(self.NhanVienAllChiNhanh(), function (item) {
                return item.ID === idNguoiQuanLy;
            });
            if (itemNV.length === 0) {
                sReturn += 'Người quản lý không tồn tại trong hệ thống <br />';
            }
        }
        return sReturn;
    }

    function CheckNgaySinh(valDate) {
        var dateNow = moment(new Date()).format('YYYY-MM-DD');
        if (valDate !== null && valDate !== '') {

            var check = isValidDateYYYYMMDD(valDate);
            if (!check) {
                ShowMessage_Danger("Ngày sinh chưa đúng định dạng");
                return false;
            }

            // if type ='dd/MM' --> not compare date
            var typeNgaySinh = self.newDoiTuong().DinhDang_NgaySinh();
            if (typeNgaySinh !== null && typeNgaySinh !== 'dd/MM') {
                if (valDate > dateNow) {
                    ShowMessage_Danger("Ngày sinh không được lớn hơn ngày hiện tại");
                    return false;
                }
            }
        }
        return true;
    }

    function callAjaxAdd(DM_DoiTuong) {
        $.ajax({
            data: DM_DoiTuong,
            url: DMDoiTuongUri + "PostDM_DoiTuong",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",

            success: function (obj) {
                if (obj.res === true) {
                    var item = obj.data;
                    ShowMessage_Success("Thêm mới " + sLoai + " thành công");

                    // insert DM_DoiTuong_Nhom (many Group)
                    var lstDM_DoiTuong_Nhom = [];
                    var sNhoms = '';
                    var idNhoms = '';

                    for (var i = 0; i < self.NhomDoiTuongChosed().length; i++) {
                        var itemFor = self.NhomDoiTuongChosed()[i];
                        var objDTNhom = {
                            ID_DoiTuong: item.ID,
                            ID_NhomDoiTuong: itemFor.ID,
                        }

                        lstDM_DoiTuong_Nhom.push(objDTNhom);
                        sNhoms += itemFor.TenNhomDoiTuong + ', ';
                        idNhoms += itemFor.ID + ', ';
                    }
                    self.InsertImage(item.ID, item.MaDoiTuong);
                    DM_DoiTuong.ID = item.ID;
                    DM_DoiTuong.MaDoiTuong = item.MaDoiTuong;
                    DM_DoiTuong.ID_NhomDoiTuong = idNhoms; // gán ID_Nhom lần 1 để check nâng nhóm
                    DM_DoiTuong.TenNhomDT = sNhoms;
                    self.CustomerDoing(DM_DoiTuong);// assign to to unshift in list customer  at KhachHang.js
                    self.UpdateNhomKH_DB(item.ID);

                    // remind birthday KH if NgaySinh_NgayTLap is today
                    var mmdd = moment(today).format('MM-DD');
                    var ngaysinhNew = DM_DoiTuong.NgaySinh_NgayTLap;
                    if (ngaysinhNew !== null) {
                        ngaysinhNew = moment(ngaysinhNew, 'YYYY-MM-DD').format('MM-DD');
                        if (ngaysinhNew === mmdd) {
                            Insert_HT_ThongBao(DM_DoiTuong);
                        }
                    }
                    $("#modalPopuplg_KH").modal("hide");
                    $("#modalPopuplg_NCC").modal("hide");
                }
                else {
                    ShowMessage_Danger(obj.mes);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                ShowMessage_Danger("Thêm mới " + sLoai + " thất bại");
            },
            complete: function () {
                Enable_btnSaveDoiTuong();
            }
        })
    }

    function callAjaxUpdate(myData) {
        $.ajax({
            url: DMDoiTuongUri + "PutDM_DoiTuong",
            type: 'PUT',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: myData,
            success: function (obj) {
                if (obj.res === true) {
                    var item = obj.data;
                    // update DM_DoiTuong_Nhom (many Group)
                    var lstDM_DoiTuong_Nhom = [];
                    var sNhoms = '';
                    var idNhoms = ',';
                    for (var i = 0; i < self.NhomDoiTuongChosed().length; i++) {
                        var itemFor = self.NhomDoiTuongChosed()[i];
                        var objDTNhom = {
                            ID_DoiTuong: item.ID,
                            ID_NhomDoiTuong: itemFor.ID,
                        }

                        lstDM_DoiTuong_Nhom.push(objDTNhom);
                        idNhoms += itemFor.ID + ', ';
                        sNhoms += itemFor.TenNhomDoiTuong + ', ';
                    }

                    // bind again TongDiem, No, TongBan (show list DoiTuong)
                    myData.objDoiTuong.ID_NhomDoiTuong = idNhoms; // gán ID_Nhom lần 1 để check nâng nhóm
                    myData.objDoiTuong.TenNhomDT = sNhoms;
                    myData.objDoiTuong.MaDoiTuong = item.MaDoiTuong;
                    self.CustomerDoing(myData.objDoiTuong);

                    self.InsertImage(item.ID, item.MaDoiTuong);
                    //self.NangNhom_KhachHang(item, 1);
                    self.UpdateNhomKH_DB(item.ID);

                    // remind birthday customer
                    var mmdd = moment(today).format('MM-DD');
                    var ngaysinhNew = myData.objDoiTuong.NgaySinh_NgayTLap;
                    if (ngaysinhNew !== null) {
                        ngaysinhNew = moment(ngaysinhNew, 'YYYY-MM-DD').format('MM-DD');
                        if (ngaysinhNew === mmdd) {
                            // insert if ngayneew !== ngayold
                            if (self.NgaySinhOld_KhachHang() !== ngaysinhNew) {
                                Insert_HT_ThongBao(myData.objDoiTuong);
                            }
                        }
                        else {
                            // delete if ngayold = to day
                            if (self.NgaySinhOld_KhachHang() === mmdd) {
                                Delete_HT_ThongBao(item.MaDoiTuong);
                            }
                        }
                    }
                    else {
                        // delete if ngayold = to day
                        if (self.NgaySinhOld_KhachHang() === mmdd) {
                            Delete_HT_ThongBao(item.MaDoiTuong);
                        }
                    }
                    $("#modalPopuplg_KH").modal("hide");
                    $("#modalPopuplg_NCC").modal("hide");
                }
                else {
                    ShowMessage_Danger(obj.mes);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                ShowMessage_Danger("Cập nhật khách hàng thất bại");
            },
            complete: function () {
                Enable_btnSaveDoiTuong();
            }
        })
    }
    // use update/delete nhom KH after NanngNhom
    self.UpdateNhomKH_DB = function (idDoiTuong) {
        var itemDT = self.CustomerDoing();
        if (itemDT != undefined) {
            var lstNhomNang = [];
            var arrIDNhom = $.unique(itemDT.ID_NhomDoiTuong.trim().split(','));
            for (var i = 0; i < arrIDNhom.length; i++) {
                if (arrIDNhom[i].trim() !== '') {
                    var objDTNhom = {
                        ID_DoiTuong: idDoiTuong,
                        ID_NhomDoiTuong: arrIDNhom[i].trim(),
                    }
                    lstNhomNang.push(objDTNhom);
                }
            }

            if (lstNhomNang.length > 0) {
                self.Update_ManyNhom(lstNhomNang, false);
            }
            else {
                // delete all nhom of DoiTuon in DB
                ajaxHelper(DMDoiTuongUri + 'DeleteAllNhom_ofDoiTuong?idDoiTuong=' + idDoiTuong, 'PUT').done(function (x) {

                })
            }
        }
    }

    self.Insert_ManyNhom = function (lstNhom) {
        var myData = {};
        myData.lstDM_DoiTuong_Nhom = lstNhom;

        ajaxHelper(DMDoiTuongUri + 'PostDM_DoiTuong_Nhom', 'POST', myData).done(function (x) {
        }).fail(function () {
            ShowMessage_Danger("Thêm mới " + sLoai + " thất bại");
        })
    }

    self.Update_ManyNhom = function (lstNhom, isMoveGroup) {
        lstNhom = $.unique(lstNhom.sort());
        lstNhom = $.grep(lstNhom, function (x) {
            return x.ID_NhomDoiTuong !== const_GuidEmpty;
        });
        if (lstNhom.length > 0) {
            var myData = {
                lstDM_DoiTuong_Nhom: lstNhom
            };
            ajaxHelper(DMDoiTuongUri + 'PutDM_DoiTuong_Nhom', 'POST', myData).done(function (x) {

            }).fail(function () {
                ShowMessage_Danger("Cập nhật " + sLoai + " thất bại");
            })
        }
    }

    self.NangNhom_KhachHang = function (objUp, loaiCheck) {

        // 1. Khachhang, 2. HoaDon, 3. PhieuThu
        var idDoiTuong = '';
        switch (loaiCheck) {
            case 1:
                idDoiTuong = objUp.ID;
                break;
            case 2:
            case 3:
                idDoiTuong = objUp.ID_DoiTuong;
                break;
        }

        var arrAddDB = [];
        if (idDoiTuong !== null && idDoiTuong !== undefined) {
            var itemDT = self.CustomerDoing();
            if (itemDT !== null && itemDT !== undefined) {

                if (loaiCheck === 3) {
                    // Thu: giảm nợ
                    if (objUp.LoaiHoaDon === 11) {
                        itemDT.NoHienTai = itemDT.NoHienTai - formatNumberToInt(objUp.TongTienThu);
                    }
                    else {
                        // Chi: tăng nợ
                        itemDT.NoHienTai = itemDT.NoHienTai + formatNumberToInt(objUp.TongTienThu);
                    }
                }

                var thangsinh = 0;
                var ngaySinhFull = null;
                var namsinh = 0;
                var dtNow = new Date();
                var monthNow = (dtNow.getMonth() + 1).toString();
                var dateNow = (dtNow.getDate()).toString();
                var yearNow = (dtNow.getFullYear()).toString();

                if (itemDT.NgaySinh_NgayTLap !== null) {
                    ngaySinhFull = new Date(moment(itemDT.NgaySinh_NgayTLap).format('YYYY-MM-DD'));
                }

                if (ngaySinhFull !== null && itemDT.DinhDang_NgaySinh !== 'yyyy') {
                    if (itemDT.DinhDang_NgaySinh !== 'yyyy') {
                        // get ThangSinh of KH (chi get nhung KH co dih dang ngay thang la d/M/y, d/M, M/y)
                        thangsinh = ngaySinhFull.getMonth() + 1;
                    }
                    if (itemDT.DinhDang_NgaySinh.indexOf('yyyy') > -1) {
                        // get NamSinh of KH (chi get nhung KH co dih dang ngay thang la d/M/y, M/y, y)
                        namsinh = ngaySinhFull.getFullYear();
                    }
                }

                var arrNhomAdd = [];
                var arrIDNhomChiTiet = [];
                for (var i = 0, li = self.DM_NhomDoiTuong_ChiTiets_Unique().length; i < li; i++) {

                    var add = false;
                    var haveCondition = false;
                    var itemFor = self.DM_NhomDoiTuong_ChiTiets_Unique()[i];

                    arrIDNhomChiTiet.push(itemFor.ID_NhomDoiTuong);

                    for (var j = 0; j < itemFor.Conditions.length; j++) {

                        var itemCondition = itemFor.Conditions[j];
                        var gtriSo = itemCondition.GiaTriSo;

                        switch (itemCondition.LoaiDieuKien) {
                            case 1:// TongBanTruTraHang
                                // neu chua co/ hoac thoa man if 1 thi moi check tiep
                                if (haveCondition === false || (haveCondition && add)) {
                                    switch (itemCondition.LoaiSoSanh) {
                                        case 1:
                                            haveCondition = true;
                                            add = (itemDT.TongBanTruTraHang > gtriSo);
                                            break;
                                        case 2:
                                            haveCondition = true;
                                            add = (itemDT.TongBanTruTraHang >= gtriSo);
                                            break;
                                        case 3:
                                            haveCondition = true;
                                            add = (itemDT.TongBanTruTraHang === gtriSo);
                                            break;
                                        case 4:
                                            haveCondition = true;
                                            add = (itemDT.TongBanTruTraHang <= gtriSo);
                                            break;
                                        case 5:
                                            haveCondition = true;
                                            add = (itemDT.TongBanTruTraHang < gtriSo);
                                            break;
                                    }
                                }
                                break;
                            case 2: // TongBan
                                if (haveCondition === false || (haveCondition && add)) {
                                    switch (itemCondition.LoaiSoSanh) {
                                        case 1:
                                            haveCondition = true;
                                            add = (itemDT.TongBan > gtriSo);
                                            break;
                                        case 2:
                                            haveCondition = true;
                                            add = (itemDT.TongBan >= gtriSo);
                                            break;
                                        case 3:
                                            haveCondition = true;
                                            add = (itemDT.TongBan === gtriSo);
                                            break;
                                        case 4:
                                            haveCondition = true;
                                            add = (itemDT.TongBan <= gtriSo);
                                            break;
                                        case 5:
                                            haveCondition = true;
                                            add = (itemDT.TongBan < gtriSo);
                                            break;
                                    }
                                }
                                break;
                            // LoaiDieuKien = 3. Thời gian mua hàng (todo)
                            case 4:// SoLanMuaHang
                                if (haveCondition === false || (haveCondition && add)) {
                                    switch (itemCondition.LoaiSoSanh) {
                                        case 1:
                                            haveCondition = true;
                                            add = (itemDT.SoLanMuaHang > gtriSo);
                                            break;
                                        case 2:
                                            haveCondition = true;
                                            add = (itemDT.SoLanMuaHang >= gtriSo);
                                            break;
                                        case 3:
                                            haveCondition = true;
                                            add = (itemDT.SoLanMuaHang === gtriSo);
                                            break;
                                        case 4:
                                            haveCondition = true;
                                            add = (itemDT.SoLanMuaHang <= gtriSo);
                                            break;
                                        case 5:
                                            haveCondition = true;
                                            add = (itemDT.SoLanMuaHang < gtriSo);
                                            break;
                                    }
                                }
                                break;
                            case 5: // NoHienTai
                                if (haveCondition === false || (haveCondition && add)) {
                                    switch (itemCondition.LoaiSoSanh) {
                                        case 1:
                                            haveCondition = true;
                                            add = (itemDT.NoHienTai > gtriSo);
                                            break;
                                        case 2:
                                            haveCondition = true;
                                            add = (itemDT.NoHienTai >= gtriSo);
                                            break;
                                        case 3:
                                            haveCondition = true;
                                            add = (itemDT.NoHienTai === gtriSo);
                                            break;
                                        case 4:
                                            haveCondition = true;
                                            add = (itemDT.NoHienTai <= gtriSo);
                                            break;
                                        case 5:
                                            haveCondition = true;
                                            add = (itemDT.NoHienTai < gtriSo);
                                            break;
                                    }
                                }
                                break;
                            case 6: // ThangSinh
                                if (thangsinh > 0) {
                                    if (haveCondition === false || (haveCondition && add)) {
                                        switch (itemCondition.LoaiSoSanh) {
                                            case 1:
                                                haveCondition = true;
                                                add = (thangsinh > gtriSo);
                                                break;
                                            case 2:
                                                haveCondition = true;
                                                add = (thangsinh >= gtriSo);
                                                break;
                                            case 3:
                                                haveCondition = true;
                                                add = (thangsinh === gtriSo);
                                                break;
                                            case 4:
                                                haveCondition = true;
                                                add = (thangsinh <= gtriSo);
                                                break;
                                            case 5:
                                                haveCondition = true;
                                                add = (thangsinh < gtriSo);
                                                break;
                                        }
                                    }
                                }
                                else {
                                    // assign to do esc for loop (neu thoa man DK6, nhung thang = 0 --> ESC)
                                    haveCondition = true;
                                    add = false;
                                }
                                break;
                            case 7: // Tuoi
                                if (namsinh !== 0) {
                                    // format again NgaySingFull
                                    ngaySinhFull = moment(ngaySinhFull).format('YYYY-MM-DD');
                                    // get namSinh can thiet cho tuoi
                                    let namsinhSubstract = yearNow - itemCondition.GiaTriSo;
                                    let ngayFullCompare1 = moment((namsinhSubstract - 1) + '-' + monthNow + '-' + dateNow, 'YYYY-MM-DD').format('YYYY-MM-DD');
                                    let ngayFullCompare2 = moment(namsinhSubstract + '-' + monthNow + '-' + dateNow, 'YYYY-MM-DD').format('YYYY-MM-DD');

                                    if (haveCondition === false || (haveCondition && add)) {
                                        switch (itemCondition.LoaiSoSanh) {
                                            case 1:
                                                haveCondition = true;
                                                add = (ngaySinhFull < ngayFullCompare1);
                                                break;
                                            case 2:
                                                haveCondition = true;
                                                add = (ngaySinhFull <= ngayFullCompare2);
                                                break;
                                            case 3:
                                                haveCondition = true;
                                                add = (ngaySinhFull === ngayFullCompare2);
                                                break;
                                            case 4:
                                                haveCondition = true;
                                                add = (ngaySinhFull >= ngayFullCompare2);
                                                break;
                                            case 5:
                                                haveCondition = true;
                                                add = (ngaySinhFull > ngayFullCompare2);
                                                break;
                                        }
                                    }
                                }
                                else {
                                    haveCondition = true;
                                    add = false;
                                }
                                break;
                            case 8: // GioiTinh
                                if (haveCondition === false || (haveCondition && add)) {
                                    haveCondition = true;
                                    add = (itemCondition.GiaTriBool === itemDT.GioiTinhNam);
                                }
                                break;
                            case 9: // KhuVuc
                                if (haveCondition === false || (haveCondition && add)) {
                                    haveCondition = true;
                                    if (itemDT.ID_TinhThanh !== null) {
                                        add = (itemCondition.GiaTriKhuVuc.trim().toLowerCase() === itemDT.ID_TinhThanh.trim().toLowerCase());
                                    }
                                }
                                break;
                            // LoaiDieuKien = 10  (VungMien_TODO)
                        }

                        if (haveCondition && add === false) {
                            // neu ton tai 1 dieu kien khong thoan man ==> ESC for loop
                            break;
                        }
                    }

                    if (haveCondition && add) {
                        arrNhomAdd.push(self.DM_NhomDoiTuong_ChiTiets_Unique()[i].ID_NhomDoiTuong);
                    }
                }

                var arrIDNhomOld = [];
                if (itemDT.ID_NhomDoiTuong !== null) {
                    arrIDNhomOld = itemDT.ID_NhomDoiTuong.toLowerCase().split(',');
                    arrIDNhomOld = arrIDNhomOld.filter(x => x !== '');
                }
                console.log('arrIDNhomOld ', arrIDNhomOld);

                for (var i = 0; i < arrNhomAdd.length; i++) {
                    // neu doituong da thuoc nhom nay --> khong can add nua
                    if ($.inArray(arrNhomAdd[i].trim().toLowerCase(), arrIDNhomOld) === -1) {
                        var DM_DoiTuong_Nhom = {
                            ID_DoiTuong: idDoiTuong,
                            ID_NhomDoiTuong: arrNhomAdd[i],
                        };
                        arrAddDB.push(DM_DoiTuong_Nhom);
                    }
                }

                // REMOVE NHOM 
                console.log('arrIDNhomChiTiet ', arrIDNhomChiTiet);

                var arrIDRemove = [];
                for (var i = 0; i < arrIDNhomOld.length; i++) {
                    // if exist at  arrIDNhomOld(old) && exist in lst arrIDNhomChiTiet , but not exist in arrNhomAdd(new) --> remove
                    if ($.inArray(arrIDNhomOld[i].trim(), arrNhomAdd) === -1 &&
                        $.inArray(arrIDNhomOld[i].trim(), arrIDNhomChiTiet) > -1) {
                        arrIDRemove.push(arrIDNhomOld[i].trim());
                    }
                }
                console.log('arrIDRemove ', arrIDRemove);

                // get nhom not exist in lst remove, exist in arrAddDB && exist in arrIDNhomOld --> assign again DoiTuongs
                var idNhomAgain = '';
                var sNhomsAgain = '';
                var arrNhomAgain = [];
                for (var i = 0; i < arrAddDB.length; i++) {
                    if ($.inArray(arrAddDB[i].ID_NhomDoiTuong.trim(), arrIDRemove) === -1) {
                        arrNhomAgain.push(arrAddDB[i].ID_NhomDoiTuong.trim());
                    }
                }

                for (var i = 0; i < arrIDNhomOld.length; i++) {
                    if ($.inArray(arrIDNhomOld[i], arrIDRemove) === -1) {
                        arrNhomAgain.push(arrIDNhomOld[i].trim());
                    }
                }

                arrNhomAgain = $.grep(arrNhomAgain, function (x) {
                    return x !== '';
                });
                arrNhomAgain = $.unique(arrNhomAgain);
                if (arrNhomAgain.length > 0) {
                    for (var i = 0; i < arrNhomAgain.length; i++) {
                        var idNhom = arrNhomAgain[i].toLowerCase();
                        var itemNhom = $.grep(self.NhomDoiTuongDB(), function (x) {
                            return x.ID.toString().toLowerCase() === idNhom;
                        });

                        if (itemNhom.length > 0) {
                            idNhomAgain += idNhom + ',';
                            sNhomsAgain += itemNhom[0].TenNhomDoiTuong + ',';
                        }
                    }
                }
                // get all TenNhomDT after add many group
                sNhomsAgain = Remove_LastComma(sNhomsAgain);
                if (sNhomsAgain === '') {
                    sNhomsAgain = 'Nhóm mặc định';
                }

                if (loaiCheck !== 1) {
                    objUp = itemDT;
                }

                // push DoiTuongs after NangNhom
                objUp.ID_NhomDoiTuong = idNhomAgain;
                objUp.TenNhomDT = sNhomsAgain;

                // add thong bao
                if (arrIDNhomOld.length !== arrNhomAgain.length) {
                    ShowMessage_Success("Đã tự động cập nhật nhóm cho khách hàng " + objUp.MaDoiTuong);
                }
            }
        }
    }

    self.DeleteImg = function (item, event) {
        if (item.ID !== undefined) {
            dialogConfirm('Thông báo xóa', 'Bạn có chắc chắn muốn xóa ảnh của khách hàng <b>' + self.newDoiTuong().MaDoiTuong() + '</b> không?', function () {
                $.ajax({
                    type: "DELETE",
                    url: DMDoiTuongUri + "DeleteDM_DoiTuong_Anh/" + item.ID,
                    dataType: 'json',
                    contentType: 'application/json',
                    success: function (result) {
                        ShowMessage_Success("Xóa ảnh khách hàng thành công");
                    },
                    error: function (error) {
                        $('#modalPopuplgDelete').modal('hide');
                        ShowMessage_Danger("Xóa ảnh " + sLoai + " thất bại");
                    }
                });

                self.FilesSelect.remove(item);
                self.FileImgs.remove(item);

                if (self.FilesSelect().length === 0) {
                    self.HaveImage_Select(false);
                    self.AnhDaiDien([]);
                    $('#file').val('');
                }
            })

        } else {
            self.FilesSelect.remove(item);

            if (self.FilesSelect().length === 0) {
                self.HaveImage_Select(false);
                self.AnhDaiDien([]);
                $('#file').val('');
            }
        }
    }

    self.InsertImage = function (idDoiTuong, maDoiTuong) {
        for (var i = 0; i < self.FilesSelect().length; i++) {

            var formData = new FormData();
            formData.append("file", self.FilesSelect()[i].file);

            $.ajax({
                type: "POST",
                url: DMDoiTuongUri + "ImageUpload?id=" + idDoiTuong + '&pathFolder=AnhKhachHang%2f' + maDoiTuong,
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                async: false,
                success: function (response) {

                },
                error: function (jqXHR, textStatus, errorThrown) {
                }
            });
        }
    }

    self.ValidateEmail = function (item) {
        if (item.Email() !== '' && item.Email() !== undefined && item.Email() !== null) {
            var re = /^(([^<>()[\]\\.,;:\s@#ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴ\"]+(\.[^<>()[\]\\.,;:\s@#ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴ\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/i;
            var valReturn = re.test(item.Email().trim());
            if (valReturn === false) {
                ShowMessage_Danger('Email không hợp lệ');
                self.checkEmail(false);
            }
            else {
                self.checkEmail(true);
            }
        }
    }
    // Chon nhieu nhom KH
    self.selectManyNhomDT = function (item) {
        var arr = [];
        for (var i = 0; i < self.NhomDoiTuongChosed().length; i++) {
            if ($.inArray(self.NhomDoiTuongChosed()[i], arr) === -1) {
                arr.push(self.NhomDoiTuongChosed()[i].ID);
            }
        }
        if ($.inArray(item.ID, arr) === -1) {
            self.NhomDoiTuongChosed.push(item);
        }

        var arrID_After = [];
        for (var i = 0; i < self.NhomDoiTuongChosed().length; i++) {
            arrID_After.push(self.NhomDoiTuongChosed()[i].ID)
        }

        // thêm dấu check vào đối tượng được chọn (OK)
        $('#ddlNhomDT li').each(function () {
            if ($.inArray($(this).attr('id'), arrID_After) > -1) {
                $(this).find('.fa-check').remove();
                $(this).append(elmCheck);
            }
            else {
                $(this).find('.fa-check').remove();
            }
        });

        // add class 'choose-person' : overflow, set width li
        $('#choose_NhomDT').addClass('choose-person');
        $('#choose_NhomDT input').remove();
    }

    self.CloseNhomDT = function (item) {
        self.NhomDoiTuongChosed.remove(item);
        if (self.NhomDoiTuongChosed().length === 0) {
            $('#choose_NhomDT').append('<input type="text" class="dropdown form-control" placeholder="Chọn nhóm">');
        }

        // remove check
        $('#ddlNhomDT li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
    }

    self.ThemDieuKienNN = function () {
        let idRandom = CreateIDRandom('DK');
        var ob1 = {
            IDRandom: idRandom,
            HinhThuc: 'Tổng mua (trừ trả hàng)',
            LoaiHinhThuc: 1,
            LoaiSoSanh: '>',
            SoSanh: 1,
            GiaTri: 0,
            TimeBy: moment(today).format('DD/MM/YYYY'),
            GioiTinh: true,
            ThangSinh: 1,
            ID_KhuVuc: null,
            KhuVuc: null,
            ID_VungMien: null,
            VungMien: null
        };
        self.DieuKienNangNhom.push(ob1);
    }

    self.DeleteDKNangNhom = function (item) {
        self.DieuKienNangNhom.remove(item);
    }

    self.SelectedDieuKien = function (item, parent) {
        var ss = '>';
        var lss = 1;
        switch (item.ID) {
            case 9:
            case 10:
                ss = 'Khác';
                lss = 6;
                break;
            case 8:
                ss = '=';
                lss = 3;
                break;
        }

        for (var i = 0; i < self.DieuKienNangNhom().length; i++) {
            if (self.DieuKienNangNhom()[i].IDRandom === parent.IDRandom) {
                self.DieuKienNangNhom()[i].LoaiHinhThuc = item.ID;
                self.DieuKienNangNhom()[i].HinhThuc = item.TenDieuKien;
                self.DieuKienNangNhom()[i].LoaiSoSanh = ss;
                self.DieuKienNangNhom()[i].SoSanh = lss;
                self.DieuKienNangNhom()[i].GiaTri = 0;
                self.DieuKienNangNhom()[i].TimeBy = moment(today).format('DD/MM/YYYY');
                self.DieuKienNangNhom()[i].GioiTinh = true;
                self.DieuKienNangNhom()[i].ThangSinh = 1;
                self.DieuKienNangNhom()[i].ID_KhuVuc = null;
                self.DieuKienNangNhom()[i].ID_VungMien = null;
                break;
            }
        }
        AssignAgain_ListDKNangNhom();
    }

    // lựa chọn hình thức
    self.SelectedSoSanh = function (item, parent) {
        for (var i = 0; i < self.DieuKienNangNhom().length; i++) {
            if (self.DieuKienNangNhom()[i].IDRandom === parent.IDRandom) {
                self.DieuKienNangNhom()[i].LoaiSoSanh = item.KieuSoSanh;
                self.DieuKienNangNhom()[i].SoSanh = item.ID;
                break;
            }
        }
        AssignAgain_ListDKNangNhom();
        //thêm dấu check vào đối tượng được chọn
        var ddlSoSanh = $(event.currentTarget).closest('#selec-all-SoSanh');
        $(ddlSoSanh).find('li').each(function () {
            $(this).find('i').remove();
            if ($(this).attr('idss') === item.ID.toString()) {
                $(this).append(elmCheck);
                return false;
            }
        });
    }
    self.resetCheckSS = function (item) {
        if (item.LoaiHinhThuc === 9 || item.LoaiHinhThuc === 10) {
            self.MangSoSanh(arrCompare36);
        }
        else {
            self.MangSoSanh(arrCompare);
        }
        var ddlSoSanh = $(event.currentTarget).parent().next();
        $(ddlSoSanh).find('li').each(function () {
            $(this).find('i').remove();
            if ($(this).attr('idss') === item.SoSanh.toString()) {
                $(this).append(elmCheck);
                return false;
            }
        });
    }

    self.resetTimeBy = function (item) {
        var $this = $(event.currentTarget);
        $($this).datetimepicker({
            format: 'd/m/Y',
            timepicker: false,
            defaultDate: new Date(),
            mask: true,
            maxDate: new Date(),
            onSelectDate: function (e, evt) {
                let dtNew = moment(e).format('DD/MM/YYYY');
                for (var i = 0; i < self.DieuKienNangNhom().length; i++) {
                    if (self.DieuKienNangNhom()[i].IDRandom === item.IDRandom) {
                        self.DieuKienNangNhom()[i].TimeBy = dtNew;
                        break;
                    }
                }
                $($this).datetimepicker('destroy');
            }
        })
    }

    // lựa chọn giới tính
    self.SelectedGTNam = function (item, parent) {
        for (var i = 0; i < self.DieuKienNangNhom().length; i++) {
            if (self.DieuKienNangNhom()[i].IDRandom === item.IDRandom) {
                self.DieuKienNangNhom()[i].GioiTinh = true;
                break;
            }
        }
        AssignAgain_ListDKNangNhom();
    }

    self.SelectedGTNu = function (item) {
        for (var i = 0; i < self.DieuKienNangNhom().length; i++) {
            if (self.DieuKienNangNhom()[i].IDRandom === item.IDRandom) {
                self.DieuKienNangNhom()[i].GioiTinh = false;
                break;
            }
        }
        AssignAgain_ListDKNangNhom();
    }

    self.refreshTab = function () {
        $('.nav-tabs li').each(function () {
            $(this).removeClass('active');
        });
        $('#tabThongTin').addClass('active');
        $('#thietlap').removeClass('active');
        $('#capnhap').removeClass('active');
        $('#thongtin').addClass('active');
        $('#thongtin').addClass('in');
        self.checkCapNhat('2');
    }

    // nhom KH theo chi nhanh
    self.popNhomKH_choseChiNhanh = function (item) {
        $('#popNhomKH_divChiNhanhChosed input').remove();
        var id = $(event.currentTarget).attr('id');
        if (id === const_GuidEmpty) {
            $('#popNhomKH_divChiNhanhChosed').append('<input type="text" class="dropdown form-control" placeholder="--- Tất cả ---">');
            // remove all check after
            $('#popNhomKH_ChiNhanhs i').remove();
            $('#popNhomKH_ChiNhanhs ul:eq(0) li:eq(0)').append(elmCheck);
            self.popNhomKH_ChiNhanhChosed([]);
        }
        else {
            $('#popNhomKH_ChiNhanhs ul:eq(0) li:eq(0) i').remove();// remove check 'Tat ca'
            var arrIDCN = [];
            for (var i = 0; i < self.popNhomKH_ChiNhanhChosed().length; i++) {
                if ($.inArray(self.popNhomKH_ChiNhanhChosed()[i].ID, arrIDCN) === -1) {
                    arrIDCN.push(self.popNhomKH_ChiNhanhChosed()[i].ID);
                }
            }
            if ($.inArray(item.ID, arrIDCN) === -1) {
                self.popNhomKH_ChiNhanhChosed.push(item);
            }

            // add check after li
            $('#popNhomKH_ChiNhanhs li').each(function () {
                if ($(this).attr('id') === item.ID) {
                    $(this).find('i').remove();
                    $(this).append(elmCheck);
                }
            });
        }
    }

    self.popNhomKH_CloseChiNhanh = function (item) {
        self.popNhomKH_ChiNhanhChosed.remove(item);
        if (self.popNhomKH_ChiNhanhChosed().length === 0) {
            $('#popNhomKH_divChiNhanhChosed').append('<input type="text" class="dropdown form-control" placeholder="--- Tất cả ---">');
        }
        // remove check
        RemoveCheckAfter_byID('popNhomKH_ChiNhanhs', item.ID);
    }
}

var DateNgaySinh;
function refreshDate() {
    DateNgaySinh = $("#txtNgaySinh").datepicker({
        showOn: 'focus',
        altFormat: "dd/mm/yy",
        buttonImage: '/Content/images/icon/ngaysinh.png',
        //showOn: "button",
        buttonImageOnly: true,
        dateFormat: "dd/mm/yy"
    });
}

