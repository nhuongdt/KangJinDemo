// validate ky tu dac biet
function isValid(str) {
    return !/[~`!@#$%\^&*()+=\-\[\]\\';,/{}|\\":<>\?]/g.test(str);
};
// khong cho phep nhap chu
function keypressNumber(e) {
    var keyCode = window.event.keyCode || e.which;
    if (keyCode < 48 || keyCode > 57) {
        // 8: tab; 127: delete
        if (keyCode == 8 || keyCode == 127) {
            return;
        }
        return false;
    }
}

shortcut.add('F3', function () {
    $('#txtAutoHangHoa').focus().select();
});
// validate nhap email
function isValidEmail(email) {
    var pattern = /^([a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+(\.[a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+)*|"((([ \t]*\r\n)?[ \t]+)?([\x01-\x08\x0b\x0c\x0e-\x1f\x7f\x21\x23-\x5b\x5d-\x7e\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|\\[\x01-\x09\x0b\x0c\x0d-\x7f\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))*(([ \t]*\r\n)?[ \t]+)?")@(([a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.)+([a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.?$/i;
    return pattern.test(email);
};

var FormModel_NhanVien = function () {
    var self = this;
    self.ID = ko.observable();
    self.MaNhanVien = ko.observable();
    self.TenNhanVien = ko.observable();
    self.NgaySinh = ko.observable();
    self.NguyenQuan = ko.observable();
    this.Email = ko.observable();
    this.GioiTinh = ko.observable(true);
    self.ThuongTru = ko.observable();
    self.DienThoaiDiDong = ko.observable();
    self.SoCMND = ko.observable();
    self.SoBHXH = ko.observable();
    self.GhiChu = ko.observable();
    this.DaNghiViec = ko.observable(true);
    self.Image = ko.observable();
    //self.DaNghiViec = ko.observableArray();



    self.SetData = function (item) {
        self.ID(item.ID);
        self.MaNhanVien(item.MaNhanVien);
        self.TenNhanVien(item.TenNhanVien);
        self.NgaySinh(moment(item.NgaySinh, "YYYY-MM-DD hh:mm:ss").format("DD/MM/YYYY"));
        self.NguyenQuan(item.NguyenQuan);
        self.Email(item.Email);
        this.GioiTinh(item.GioiTinh);
        this.ThuongTru(item.ThuongTru);
        this.DienThoaiDiDong(item.DienThoaiDiDong);
        self.SoCMND(item.SoCMND);
        self.SoBHXH(item.SoBHXH);
        self.GhiChu(item.GhiChu);
        if (item.DaNghiViec == false)
            this.DaNghiViec(true);
        else
            this.DaNghiViec(false);
        self.Image(item.Image);
        //this.DaNghiViec(item.DaNghiViec);
        //self.DaNghiViec(item.DaNghiViec);
    };
}

var ViewModel = function () {
    //Khai báo
    var self = this;
    var NhanVienUri = '/api/DanhMuc/NS_NhanVienAPI/';
    var NhomHHUri = '/api/DanhMuc/DM_NhomHangHoaAPI/';
    var ReportUri = '/api/DanhMuc/ReportAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/';
    self.NhanVienChitiets = ko.observableArray();
    self.item_UpdateChietKhau = ko.observableArray();
    self.NhanViens = ko.observableArray();
    self.error = ko.observable();
    self.filter = ko.observable();
    self.NhomHangHoas = ko.observableArray();
    self._ThemMoiNhanVien = ko.observable(true);
    self.HangHoas_seach = ko.observableArray();
    self.selectedHH = ko.observable();
    self.MaHangHoa_Search = ko.observable();
    self.RowsStart = ko.observable('1');
    self.RowsEnd = ko.observable('10');
    self.Loc_TinhTrangKD = ko.observable('2');
    self.LoaiSP_HH = ko.observable(true);
    self.LoaiSP_DV = ko.observable(true);
    self.checkCapNhat = ko.observable('2');
    var _pageNumber = 1;
    var _pageSize = 5;
    var AllPage;
    self.ApDung_CK = ko.observable(0);
    var _id_ChiNhanh_Defeaul = {
        ID: $('#hd_IDdDonVi').val()
    }
    self.ChiNhanh_Defeaul = ko.observableArray();
    self.ChiNhanh_Defeaul.push(_id_ChiNhanh_Defeaul);
    //self.DaNghiViec = ko.observableArray([
    //    { name: "Đang làm việc", value: "true" },
    //    { name: "Đã nghỉ việc", value: "false" }
    //]);
    //self.selectedDaNghiViec = ko.observable();
    var _maHH_Seach = ""; //tuấn sửa
    self.deleteTenHangHoa = ko.observable();
    self.deleteID = ko.observable();
    self.newNhanVien = ko.observable(new FormModel_NhanVien());
    self._Ten = ko.observable();
    self.TieuDeThemNhanVien = ko.observable('Cập nhật nhân viên');
    self.MangChiNhanhNhanVien = ko.observableArray();
    var id_NhomHang_Seach = null;
    var _iddonvi = $('#hd_IDdDonVi').val(); // lấy ID chi nhánh _header.cshtml
    var _id_NhanVien = $('.idnhanvien').text();
    self.booleanAdd = ko.observable(true);

    self.selectedNhanVien = ko.observable();
    console.log(self.selectedNhanVien());
    self.selectedTenNhanVien = ko.computed(function () {
        return $("#ddlNhanVien option[value='" + self.selectedNhanVien() + "']").text();
    });

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

    //select
    var _nhanvienchuaCD = null;
    var _nhanviendaCD = null;
    self.NhanVienSaoChep1 = ko.observableArray();
    self.NhanVienSaoChep2 = ko.observableArray();
    function getlist_NhanVienChuaCaiDat() {
        ajaxHelper(NhanVienUri + "getlistNhanVien_CaiDatChietKhau?ID_DonVi=" + _iddonvi + "&MaNhanVien=" + _nhanvienchuaCD + "&TrangThai=" + 0, 'GET').done(function (data) {
            self.NhanVienSaoChep1(data);
        });
    };
    getlist_NhanVienChuaCaiDat();
    function getlist_NhanVienDaCaiDat() {
        ajaxHelper(NhanVienUri + "getlistNhanVien_CaiDatChietKhau?ID_DonVi=" + _iddonvi + "&MaNhanVien=" + _nhanviendaCD + "&TrangThai=" + 1, 'GET').done(function (data) {
            self.NhanVienSaoChep2(data);
        });
    };
    getlist_NhanVienDaCaiDat();
    //function getAllNhanVien() {
    //    ajaxHelper(NhanVienUri + "GetNS_NhanVien", 'GET').done(function (data) {
    //        self.NhanViens(data);
    //    });
    //};
    self.clearn_ChuaCaiDat = function () {
        $('#all_nhanvienchuacaidat').prop('checked', false);
        $('#all_nhanvienchuacaidat').removeClass('squarevt');
        $('.NhanVien_ChuaCaiDat input').each(function () {
            $(this).prop('checked', false);
            var thisID = $(this).attr('id');
            $.map(arrID_NhanVien, function (item, i) {
                if (item === thisID) {
                    arrID_NhanVien.splice(i, 1);
                }
            })
        })
        console.log(arrID_NhanVien);
    }
    self.clearn_DaCaiDat = function () {
        $('#all_nhanviendacaidat').prop('checked', false);
        $('#all_nhanviendacaidat').removeClass('squarevt');
        $('.NhanVien_DaCaiDat input').each(function () {
            $(this).prop('checked', false);
            var thisID = $(this).attr('id');
            $.map(arrID_NhanVien, function (item, i) {
                if (item === thisID) {
                    arrID_NhanVien.splice(i, 1);
                }
            })
        })
        console.log(arrID_NhanVien);
    }
    $('.select_CapNhat input').on('click', function () {
        self.checkCapNhat($(this).val());
        console.log(self.checkCapNhat());
    });
    self.ApDung_SaoChep = function () {
        document.getElementById("add_HoaHong").disabled = true;
        document.getElementById("add_HoaHong").lastChild.data = " Đang lưu";
        if (arrID_NhanVien.length != 0) {
            var _ID_NhanVien;
            for (var i = 0; i < arrID_NhanVien.length; i++) {
                if (i == 0)
                    _ID_NhanVien = arrID_NhanVien[i];
                else
                    _ID_NhanVien = _ID_NhanVien + "," + arrID_NhanVien[i];
            }
            SaoChep_CaiDatHoaHong(_ID_NhanVien);
        }
        else {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn chưa chọn nhân viên để thực hiện sao chép", "danger");
            document.getElementById("add_HoaHong").disabled = false;
            document.getElementById("add_HoaHong").lastChild.data = " Áp dụng";
        }
    };
    function SaoChep_CaiDatHoaHong(item) {
        $('.table-reponsive').gridLoader();
        var array_Seach = {
            ID_DonVi: _iddonvi,
            ID_NhanVien: self.selectedNhanVien(),
            ID_NhanVien_new: item,
            PhuongThuc: parseInt(self.checkCapNhat()),
        }
        ajaxHelper(NhanVienUri + "SaoChep_CaiDatHoaHong", "POST", array_Seach).done(function (data) {
            console.log(data);
            if (data != "SCCDHH")
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Sao chép cài đặt hoa hồng không thành công", "danger");
            else
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Sao chép cài đặt hoa hồng thành công", "success");
            document.getElementById("add_HoaHong").disabled = false;
            document.getElementById("add_HoaHong").lastChild.data = " Áp dụng";
            $('#modalSaoChepNhanVien').modal("hide");
            getlist_NhanVienChuaCaiDat();
            getlist_NhanVienDaCaiDat();
            self.clearn_ChuaCaiDat();
            self.clearn_DaCaiDat();
            $('.table-reponsive').gridLoader({ show: false });
            $("div[id ^= 'wait']").text("");
        });
    }
    function GetAllNhomHH() {
        ajaxHelper('/api/DanhMuc/DM_NhomHangHoaAPI/' + 'GetDM_NhomHangHoa', 'GET').done(function (data) {
            localStorage.setItem('lc_NhomHangHoas', JSON.stringify(data));
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
    var time = null
    self.NoteNhomHang = function () {
        clearTimeout(time);
        time = setTimeout(
            function () {
                self.NhomHangHoas([]);
                tk = $('#SeachNhomHang').val();
                if (tk.trim() != '') {
                    ajaxHelper(ReportUri + "GetListID_NhomHangHoa?TenNhomHang=" + tk, 'GET').done(function (data) {
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
    self.SelectRepoert_NhomHangHoa = function (item) {

        var idNhanVien = self.selectedNhanVien();
        if (idNhanVien == null || idNhanVien === undefined || idNhanVien === '') {
            bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i> ' + "Vui lòng chọn nhân viên để nhập chiết khấu", "danger");
            return false;
        }
        var it = item;
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
        self.changeddlNhomHangHoa(it);
    }

    $('.SelectALLNhomHang').on('click', function () {
        $('#tatcanhh').addClass('yellow')
        $('.ss-li .li-oo').removeClass("yellow");
        _pageNumber = 1;
        id_NhomHang_Seach = null;
        getAllNhanVienChiTiet();
    });

    //function getAllDMNhomHangHoas() {
    //    ajaxHelper(NhomHHUri + "GetDM_NhomHangHoa", 'GET').done(function (data) {
    //        self.NhomHangHoas(data);
    //    });
    //};
    //getAllDMNhomHangHoas();

    function getChiTietNhanVienbyID(id) {
        ajaxHelper(NhanVienUri + "GetChiTietNhanVien/" + id, 'GET').done(function (data) {
            self.NhanVienChitiets(data);
        })
    };
    //getAllNhanVien();

    self.resetNhanVien = function () {
        self.newNhanVien(new FormModel_NhanVien());
    };

    //show modal
    self.themmoinhanvien = function () {
        refreshChiNhanh();
        self.TieuDeThemNhanVien('Thêm mới nhân viên');
        self._ThemMoiNhanVien(true);
        self.booleanAdd(true);
        self.resetNhanVien();
        $('#modalPopuplg_NV').modal('show');
    };
    $('#modalPopuplg_NV').on('shown.bs.modal', function () {
        $('#txtTenNhanVien').focus();
    });

    self.capnhatnhanvien = function (item) {
        self.TieuDeThemNhanVien('Cập nhật nhân viên');
        ajaxHelper(NhanVienUri + "getlistQTCongTac?ID_NhanVien=" + self.selectedNhanVien(), "GET").done(function (data) {
            self.MangChiNhanhNhanVien(data);
            self.listAddChiNhanh(data);
            if (data.length == 0)
                refreshChiNhanh();
        });
        ajaxHelper(NhanVienUri + "GetNS_NhanVien/" + this.selectedNhanVien(), 'GET').done(function (data) {
            self.newNhanVien().SetData(data);
            self._ThemMoiNhanVien(false);
            console.log(data);
        });
        self.booleanAdd(false);
        $('#modalPopuplg_NV').modal('show');
    }

    self.getChiTietNhanVien = function () {
        self._Ten(self.selectedTenNhanVien() === "" ? "cài đặt hoa hồng" : self.selectedTenNhanVien());
        getAllNhanVienChiTiet();
    };
    //THem/Sua
    self.addNhanVien = function (formElement) {
        var _manhanvien = self.newNhanVien().MaNhanVien();
        _manhanvien = _manhanvien == "" ? undefined : (_manhanvien == null ? undefined : _manhanvien);
        var _id = self.newNhanVien().ID();
        var _tennhanvien = self.newNhanVien().TenNhanVien();
        var _ngaysinh = self.newNhanVien().NgaySinh();
        var _quequan = self.newNhanVien().NguyenQuan();
        var _email = this.newNhanVien().Email();
        _email = _email == "" ? null : _email;
        var _dienthoai = this.newNhanVien().DienThoaiDiDong();
        var _gioitinh = this.newNhanVien().GioiTinh();
        var _socmt = this.newNhanVien().SoCMND();
        var _sobh = this.newNhanVien().SoBHXH();
        var _ghichu = this.newNhanVien().GhiChu();
        var _diachi = this.newNhanVien().ThuongTru();
        var _danghiviec = false;
        if (this.newNhanVien().DaNghiViec() == false)
            _danghiviec = true;
        else
            _danghiviec = false;
        //var _danghiviec = this.newNhanVien().DaNghiViec();

        if (!isValid(_manhanvien)) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Mã nhân viên không được nhập ký tự đặc biệt!", "danger");
            $('#txtMaNhanVien').focus();
            return false;
        }
        if (_tennhanvien == null || _tennhanvien == "" || _tennhanvien == "undefined") {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng nhập tên nhân viên!", "danger");
            $('#txtTenNhanVien1').focus();
            return false;
        }
        var dtNow = new Date(Date.now());
        var date1 = '';
        if (_ngaysinh === undefined || _ngaysinh === 'Invalid date' || _ngaysinh === "") {
            _ngaysinh = moment(dtNow).format('YYYY-MM-DD HH:mm:ss')
            date1 = moment(dtNow).format('DD/MM/YYYY');
        }
        else {
            _ngaysinh = moment(_ngaysinh, "DD/MM/YYYY").format("YYYY/MM/DD hh:mm:ss");
            date1 = self.newNhanVien().NgaySinh();
        }

        // check ngaysinh > Date now
        var date2 = moment(dtNow).format('DD/MM/YYYY');
        if ($.datepicker.parseDate('dd/mm/yy', date1) > $.datepicker.parseDate('dd/mm/yy', date2)) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ngày sinh không được lớn hơn ngày hiện tại!", "danger");
            return false;
        }
        if (_email != null) {
            if (!isValidEmail(_email)) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Email không đúng định dạng!", "danger");
                return false;
            }
        }
        //if (self.listAddChiNhanh().some(o => o.ID_ChiNhanh === null || o.ID_PhongBan === null)) {
        //    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng chọn chi nhánh, phòng ban", "danger");
        //    return false;
        //}
        var Nhan_Vien = {
            ID: _id,
            MaNhanVien: _manhanvien,
            TenNhanVien: _tennhanvien,
            NgaySinh: _ngaysinh,
            NguyenQuan: _quequan,
            ThuongTru: _diachi,
            Email: _email,
            DienThoaiDiDong: _dienthoai,
            GioiTinh: _gioitinh,
            SoCMND: _socmt,
            SoBHXH: _sobh,
            GhiChu: _ghichu,
            DaNghiViec: _danghiviec
        };

        //console.log('111' + JSON.stringify(Nhan_Vien));
        //Them
        if (self.booleanAdd() === true) {
            var myData = {};
            myData.objNVien = Nhan_Vien;
            if (self.listAddChiNhanh()[0].ID_ChiNhanh == null) {
                myData.objQuaTrinhCongTac = [{
                    ID: 1,
                    ID_ChiNhanh: $('#hd_IDdDonVi').val(),
                    ID_PhongBan: null,
                    LaMacDinh: true,
                    Text_ChiNhanh: $("#_txtTenDonVi").text(),
                    Text_PhongBan: "Phòng ban mặc định"
                }];
            }
            else {
                myData.objQuaTrinhCongTac = self.listAddChiNhanh();
            }
            //myData.objQuaTrinhCongTac = self.listAddChiNhanh();
            $.ajax({
                data: Nhan_Vien,
                url: NhanVienUri + "Check_Exist",
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
        //Sua
        else {
            var myData = {};
            myData.id = _id;
            myData.objNVien = Nhan_Vien;
            myData.objQuaTrinhCongTac = self.listAddChiNhanh();
            //if (self.MangChiNhanhNhanVien().length > 0) {
            //    myData.objQuaTrinhCongTac = self.MangChiNhanhNhanVien();
            //}
            //else {
            //    myData.objQuaTrinhCongTac = self.ChiNhanh_Defeaul();
            //}
            var dem = 0;
            var arrChiNhanhNV = [];
            ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + 'GetListVaiTro?idnhanvien=' + _id, 'GET').done(function (data) {
                for (var i = 0; i < myData.objQuaTrinhCongTac.length; i++) {
                    arrChiNhanhNV.push(myData.objQuaTrinhCongTac[i].ID);
                }
                for (var i = 0; i < data.length; i++) {
                    if ($.inArray(data[i].ID_DonVi, arrChiNhanhNV) > -1) {
                        dem = dem + 1;
                    }
                }
                if (dem === data.length) {
                    callAjaxUpdate(myData);
                }
                else {
                    bottomrightnotify("Không thể thay đổi các chi nhánh đã được sử dụng cho phân quyền người dùng", "danger");
                    return false;
                }
            })


        }
    };

    function callAjaxAdd(myData) {
        var formDataTR = new FormData();
        var totalFilesTR = document.getElementById("imageUploadFormTR").files.length;
        for (var i = 0; i < totalFilesTR; i++) {
            var file = document.getElementById("imageUploadFormTR").files[i];
            formDataTR.append("imageUploadFormTR", file);
        }
        $.ajax({
            data: myData,
            url: NhanVienUri + "PostNS_NhanVien?ID_DonVi=" + _iddonvi + "&ID_NhanVien=" + _id_NhanVien,
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (item) {
                if (totalFilesTR > 0) {
                    $.ajax({
                        type: "POST",
                        url: NhanVienUri + "UploadImageStaff?nhanvienId=" + item.ID,
                        data: formDataTR,
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        success: function (response) {
                            bottomrightnotify("Thêm mới nhân viên thành công", "success");
                            getAllNSNhanVien();
                        },
                    });
                }
                else {
                    bottomrightnotify("Thêm mới nhân viên thành công", "success");
                    getAllNSNhanVien();
                }
                //bottomrightnotify("Thêm mới nhân viên thành công", "success");
                //$("#modalPopuplg_NV").modal("hide");
                //getAllNSNhanVien();
            },
            statusCode: {
                404: function () {
                    self.error("page not found");
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                bottomrightnotify("Thêm mới nhân viên thất bại", "danger");
            },
            complete: function () {
                $("#modalPopuplg_NV").modal("hide");
            }
        })
    }

    function callAjaxUpdate(myData) {
        var formDataTR = new FormData();
        var totalFilesTR = document.getElementById("imageUploadFormTR").files.length;
        for (var i = 0; i < totalFilesTR; i++) {
            var file = document.getElementById("imageUploadFormTR").files[i];
            formDataTR.append("imageUploadFormTR", file);
        }
        $.ajax({
            url: NhanVienUri + "PutNS_NhanVien?ID_DonVi=" + _iddonvi + "&ID_NhanVien=" + _id_NhanVien,
            type: 'PUT',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: myData,
            success: function () {
                if (totalFilesTR > 0) {
                    $.ajax({
                        type: "POST",
                        url: NhanVienUri + "UploadImageStaff?nhanvienId=" + self.newNhanVien().ID(),
                        data: formDataTR,
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        success: function (response) {
                            bottomrightnotify("Cập nhật nhân viên thành công", "success");
                            getAllNSNhanVien();
                        },
                    });
                }
                else {
                    bottomrightnotify("Cập nhật nhân viên thành công", "success");
                    getAllNSNhanVien();
                }
                //bottomrightnotify('Cập nhật nhân viên thành công', 'success');
                //self.selectNameNV(myData.objNVien.TenNhanVien);
                //getAllNSNhanVien();
            },
            statusCode: {
                404: function () {
                    self.error("page not found");
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                bottomrightnotify("Cập nhật nhân viên thất bại", "danger");
            },
            complete: function () {
                $("#modalPopuplg_NV").modal("hide");
            }
        })
    }

    //self.addAndNewGiaBan = function () { };
    var choose_Update = 1;
    var ID_ChietKhau = "";
    var up_giaban = null
    self.txtChietKhau = function (item) {
        choose_Update = 1;
        self.item_UpdateChietKhau(item);
        ID_ChietKhau = item.ID;
        up_giaban = item.GiaBan;
        setTimeout(function () {
            $('#number_' + ID_ChietKhau).focus().select();
            if (item.LaPTChietKhau == 1) {
                chietkhau_LPT = true;
                $('#pt_' + ID_ChietKhau).addClass('gb');
                $('#vnd_' + ID_ChietKhau).removeClass('gb');
            }
            else {
                chietkhau_LPT = false;
                $('#pt_' + ID_ChietKhau).removeClass('gb');
                $('#vnd_' + ID_ChietKhau).addClass('gb');
            }
        }, 100);
    }
    var ID_ThucHien = "";
    self.txtThucHien = function (item) {
        choose_Update = 3;
        self.item_UpdateChietKhau(item);
        ID_ThucHien = item.ID;
        up_giaban = item.GiaBan;
        setTimeout(function () {
            $('#number1_' + ID_ThucHien).focus().select();
            if (item.LaPTYeuCau == 1) {
                ThucHien_LPT = true;
                $('#pt1_' + ID_ThucHien).addClass('gb');
                $('#vnd1_' + ID_ThucHien).removeClass('gb');
            }
            else {
                ThucHien_LPT = false;
                $('#pt1_' + ID_ThucHien).removeClass('gb');
                $('#vnd1_' + ID_ThucHien).addClass('gb');
            }
        }, 100);
    }
    var ID_TuVan = "";
    self.txtTuVan = function (item) {
        choose_Update = 2;
        self.item_UpdateChietKhau(item);
        ID_TuVan = item.ID;
        up_giaban = item.GiaBan;
        setTimeout(function () {
            $('#number2_' + ID_TuVan).focus().select();
            if (item.LaPTTuVan == 1) {
                TuVan_LPT = true;
                $('#pt2_' + ID_TuVan).addClass('gb');
                $('#vnd2_' + ID_TuVan).removeClass('gb');
            }
            else {
                TuVan_LPT = false;
                $('#pt2_' + ID_TuVan).removeClass('gb');
                $('#vnd2_' + ID_TuVan).addClass('gb');
            }
        }, 100);
    }
    var chietkhau_LPT = false;
    //$('#cbapdung_' + ID_ChietKhau).attr('disabled', 'false');
    //$('#cbapdung1_' + ID_ThucHien).attr('disabled', 'false');
    //$('#cbapdung2_' + ID_TuVan).attr('disabled', 'false');
    self.btnChieuKhauVND = function () {
        chietkhau_LPT = false;
        //$('#cbapdung_' + ID_ChietKhau).attr('disabled', 'false');
    }
    self.btnChieuKhauPT = function () {
        chietkhau_LPT = true;
        //$('#cbapdung_' + ID_ChietKhau).removeAttr('disabled');
    }
    var ThucHien_LPT = false;
    self.btnThucHienVND = function () {
        ThucHien_LPT = false;
        //$('#cbapdung1_' + ID_ThucHien).attr('disabled', 'false');
    }
    self.btnThucHienPT = function () {
        ThucHien_LPT = true;
        //$('#cbapdung1_' + ID_ThucHien).removeAttr('disabled');
    }
    var TuVan_LPT = false;
    self.btnTuVanVND = function () {
        TuVan_LPT = false;
        //$('#cbapdung2_' + ID_TuVan).attr('disabled', 'false');
    }
    self.btnTuVanPT = function () {
        TuVan_LPT = true;
        //$('#cbapdung2_' + ID_TuVan).removeAttr('disabled');
    }
    var chkChietKhau = false;
    self.chkChietKhau = function () {
        if (chkChietKhau) {
            chkChietKhau = false;
            self.ApDung_CK('0');
        }
        else {
            chkChietKhau = true;
            self.ApDung_CK(1);
        }
        console.log(chkChietKhau)
    }
    var chkThucHien = false;
    self.chkThucHien = function () {
        if (chkThucHien)
            chkThucHien = false;
        else
            chkThucHien = true;
        console.log(chkThucHien)
    }
    var chkTuVan = false;
    self.chkTuVan = function () {
        if (chkTuVan)
            chkTuVan = false;
        else
            chkTuVan = true;
        console.log(chkTuVan)
    }
    self.insertNhomHangHoa = ko.observable();
    var _id_NhomHangHoa = null;
    self.showIsertChietKhau = function (item) {

        _id_NhomHangHoa = item.ID;
        self.insertNhomHangHoa(item.TenNhomHangHoa);
        $('#modalpopup_Insert').modal("show");
    }
    self.addChiTietNhanVien = function (item) {
        if (self.selectedNhanVien() != null) {
            $.ajax({
                type: "GET",
                url: NhanVienUri + "AddChiTiet/" + _id_NhomHangHoa + "?idnhanvien=" + self.selectedNhanVien() + "&iddonvi=" + _iddonvi,
                success: function (result) {
                    if (result === true) {
                        var objDiary = {
                            ID_NhanVien: _id_NhanVien,
                            ID_DonVi: _iddonvi,
                            ChucNang: "cài đặt hoa hồng",
                            NoiDung: "Cập nhật hàng hóa cài đặt hoa hồng theo nhân viên: " + self._Ten(),
                            NoiDungChiTiet: "Cập nhật hàng hóa cài đặt hoa hồng theo nhân viên: " + self._Ten(),
                            LoaiNhatKy: 2 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
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
                            },
                        })
                        self.getChiTietNhanVien();
                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i> ' + "Cập nhật hàng hóa cài đặt hoa hồng thành công", "success");
                    }
                    else {
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cập nhật hàng hóa cài đặt hoa hồng thất bại", "danger");
                    }
                }
            });
        }
        else {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Chưa chọn nhân viên để thực hiện cài đặt hoa hồng", "danger");
        }
    };

    self.modalDelete = function (item) {
        self.deleteTenHangHoa(item.TenHangHoa);
        self.deleteID(item.ID);
        //console.log(self.deleteTenHangHoa);
        $('#modalpopup_delete').modal('show');
    };
    self.modalDeleteAllChietKhau = function () {
        $('#modalpopup_deleteAll').modal('show');
    }
    self.deleteChiTietNhanVien = function () {
        $.ajax({
            type: "GET",
            url: NhanVienUri + "deleteChiTiet/" + self.deleteID(),
            success: function (result) {
                if (result === true) {
                    self.getChiTietNhanVien();
                    bottomrightnotify("Xóa hàng hóa cài đặt hoa hồng thành công", "success");
                }
                else {
                    bottomrightnotify("Xóa hàng hóa cài đặt hoa hồng thất bại!", "danger");
                }
            }
        })
    };
    self.deleteAllChietKhauNhanVien = function () {
        $.ajax({
            type: "GET",
            url: NhanVienUri + "deleteAllChietKhau?ID_NhanVien=" + self.selectedNhanVien() + "&ID_NhomHang=" + id_NhomHang_Seach + "&maHH=" + _maHH_Seach + "&ID_DonVi=" + _iddonvi,
            success: function (result) {
                bottomrightnotify("Xóa hàng hóa cài đặt hoa hồng thành công", "success");
                getAllNhanVienChiTiet();
            },
            error: function (error) {
                bottomrightnotify("Xóa hàng hóa cài đặt hoa hồng thất bại!", "danger");
            }
        })
    };

    $('#text_MaHangHoa').keypress(function (e) {
        _maHH_Seach = $(this).val();
        console.log(_maHH_Seach);
    });
    $('#text_MaHangHoa').keypress(function (e) {
        if (e.keyCode == 13) {
            getAllNhanVienChiTiet();
        }
    });
    self.RowsHangHoaChietKhau = ko.observable();
    self.ListPage = ko.observableArray();
    function LoadingForm(IsShow) {
        $('.table-show').gridLoader({ show: IsShow });
    }
    var _laHangHoa = 2;
    var TinhTrangHH = 2;
    //Loại hàng
    var _ckHangHoa = 1;
    var _ckDichVu = 1;
    $('.choose_LoaiHang input').on('click', function () {
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
        _pageNumber = 1;
        getAllNhanVienChiTiet();
    })

    $('.chose_TinhTrangKD input').on('click', function () {
        TinhTrangHH = $(this).val();
        _pageNumber = 1;
        self.Loc_TinhTrangKD($(this).val());
        getAllNhanVienChiTiet();
    });
    function getAllNhanVienChiTiet() {
        LoadingForm(true);
        console.log('_pageNumber', _pageNumber);
        var idNhanVien = self.selectedNhanVien();

        ajaxHelper(NhanVienUri + "GetListNhanVienNhomHang?MaHH=" + _maHH_Seach + "&ID_NhanVien=" + idNhanVien + "&ID_DonVi=" + _iddonvi + "&LaHangHoa=" + _laHangHoa + "&TinhTrang=" + TinhTrangHH + "&ID_NhomHang=" + id_NhomHang_Seach + "&nuberPage=" + _pageNumber + "&pageSize=" + _pageSize, 'GET').done(function (data) {
            self.NhanVienChitiets(data.LstData);
            console.log(data);
            if (self.NhanVienChitiets().length != 0) {
                $(".Report_Empty").hide();
                $(".page").show();
                self.RowsStart((_pageNumber - 1) * _pageSize + 1);
                self.RowsEnd((_pageNumber - 1) * _pageSize + self.NhanVienChitiets().length)
            }
            else {
                $(".Report_Empty").show();
                $(".page").hide();
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            //self.ListPage(data.numberPage);
            AllPage = data.numberPage;
            self.selecPage();
            self.ReserPage();
            self.RowsHangHoaChietKhau(data.Rowcount);
            $("div[id ^= 'wait']").remove();
            LoadingForm(false);
        });
    };
    self.getAllNhanVienChiTiet = function (item) {
        id_NhomHang_Seach = null;
        //getAllNhanVienChiTiet();
    }

    //getctnhanvien by nhom hàng

    function GetNhanVienCTByIDNhom(id) {
        var idNhanVien = self.selectedNhanVien();
        if (idNhanVien == undefined || idNhanVien == null) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i>' + "Chưa chọn nhân viên cài đặt hoa hồng", "danger");
        }
        else {
            ajaxHelper(NhanVienUri + "GetListNhanVienNhomHang?id=" + id + "&idNhanVien=" + idNhanVien, 'GET').done(function (data) {
                self.NhanVienChitiets(data);
            });
        }
    }
    self.changeddlNhomHangHoa = function (item) {
        id_NhomHang_Seach = item.ID;
        //GetNhanVienCTByIDNhom();
        getAllNhanVienChiTiet();
    }
    // cài đặt chiết khấu mới
    self.addChietKhauNhanVien = function () {
        var giatri_CK = formatNumberToFloat($('#number_' + ID_ChietKhau).val());
        if (giatri_CK > 100 && chietkhau_LPT == true) {
            $("#modalpopup_Update").modal("show");
        }
        else {
            var mydata = {};
            if ($('#cbapdung_' + ID_ChietKhau).is(":checked")) {
                mydata.objData = self.NhanVienChitiets();
            }
            else {
                mydata.objData = [self.item_UpdateChietKhau()];
            }
            $.ajax({
                url: NhanVienUri + "update_ChietKhau?ChietKhau=" + giatri_CK + "&LaPhanTram=" + chietkhau_LPT,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                data: mydata,
                success: function (result) {
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng thành công", "success");
                    self.getChiTietNhanVien();
                },
                error: function (error) {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cập nhật cài đặt hoa hồng thất bại", "danger");
                    $('#modalpopup_deleteGB').modal('hide');
                }
            })
        }
    }
    self.addChietKhauNhanVien_YeuCau = function () {
        var giatri_CK = formatNumberToFloat($('#number1_' + ID_ThucHien).val());
        if (giatri_CK > 100 && ThucHien_LPT == true) {
            $("#modalpopup_Update").modal("show");
        }
        else {
            var mydata = {};
            if ($('#cbapdung1_' + ID_ThucHien).is(":checked")) {
                mydata.objData = self.NhanVienChitiets();
            }
            else {
                mydata.objData = [self.item_UpdateChietKhau()];
            }
            $.ajax({
                url: NhanVienUri + "update_ChietKhau_YeuCau?ChietKhau=" + giatri_CK + "&LaPhanTram=" + ThucHien_LPT,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                data: mydata,
                success: function (result) {
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng thành công", "success");
                    self.getChiTietNhanVien();
                },
                error: function (error) {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cập nhật cài đặt hoa hồng thất bại", "danger");
                    $('#modalpopup_deleteGB').modal('hide');
                }
            })
        }
    }// Hoa hồng gói dịch vụ
    self.addChietKhauNhanVien_TuVan = function () {
        var giatri_CK = formatNumberToFloat($('#number2_' + ID_TuVan).val());
        if (giatri_CK > 100 && TuVan_LPT == true) {
            $("#modalpopup_Update").modal("show");
        }
        else {
            var mydata = {};
            if ($('#cbapdung2_' + ID_TuVan).is(":checked")) {
                mydata.objData = self.NhanVienChitiets();
            }
            else {
                mydata.objData = [self.item_UpdateChietKhau()];
            }
            $.ajax({
                url: NhanVienUri + "update_ChietKhau_TuVan?ChietKhau=" + giatri_CK + "&LaPhanTram=" + TuVan_LPT,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                data: mydata,
                success: function (result) {
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng thành công", "success");
                    self.getChiTietNhanVien();
                },
                error: function (error) {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cập nhật cài đặt hoa hồng thất bại", "danger");
                    $('#modalpopup_deleteGB').modal('hide');
                }
            })
        }
    }
    self.ignore_ChietKhauNhanVien = function () {
        if (choose_Update == 1) {
            var giatri_CK = formatNumberToFloat($('#number_' + ID_ChietKhau).val());
            var mydata = {};
            if ($('#cbapdung_' + ID_ChietKhau).is(":checked")) {
                mydata.objData = self.NhanVienChitiets();
            }
            else {
                mydata.objData = [self.item_UpdateChietKhau()];
            }
            $.ajax({
                url: NhanVienUri + "update_ChietKhau?ChietKhau=" + giatri_CK + "&LaPhanTram=" + chietkhau_LPT,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                data: mydata,
                success: function (result) {
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng thành công", "success");
                    self.getChiTietNhanVien();
                },
                error: function (error) {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cập nhật cài đặt hoa hồng thất bại", "danger");
                    $('#modalpopup_deleteGB').modal('hide');
                }
            })
        }
        else if (choose_Update == 2) {
            var giatri_CK = formatNumberToFloat($('#number2_' + ID_TuVan).val());
            var mydata = {};
            if ($('#cbapdung2_' + ID_TuVan).is(":checked")) {
                mydata.objData = self.NhanVienChitiets();
            }
            else {
                mydata.objData = [self.item_UpdateChietKhau()];
            }
            $.ajax({
                url: NhanVienUri + "update_ChietKhau_TuVan?ChietKhau=" + giatri_CK + "&LaPhanTram=" + TuVan_LPT,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                data: mydata,
                success: function (result) {
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng thành công", "success");
                    self.getChiTietNhanVien();
                },
                error: function (error) {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cập nhật cài đặt hoa hồng thất bại", "danger");
                    $('#modalpopup_deleteGB').modal('hide');
                }
            })
        }
        else {
            var giatri_CK = formatNumberToFloat($('#number1_' + ID_ThucHien).val());
            var mydata = {};
            if ($('#cbapdung1_' + ID_ThucHien).is(":checked")) {
                mydata.objData = self.NhanVienChitiets();
            }
            else {
                mydata.objData = [self.item_UpdateChietKhau()];
            }
            $.ajax({
                url: NhanVienUri + "update_ChietKhau_YeuCau?ChietKhau=" + giatri_CK + "&LaPhanTram=" + ThucHien_LPT,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                data: mydata,
                success: function (result) {
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng thành công", "success");
                    self.getChiTietNhanVien();
                },
                error: function (error) {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cập nhật cài đặt hoa hồng thất bại", "danger");
                    $('#modalpopup_deleteGB').modal('hide');
                }
            })
        }
    }
    //chiet khau
    //self.addChietKhauNhanVien = function () {
    //    var giatri_CK = formatNumberToInt($('#number_' + ID_ChietKhau).val());
    //    if (chietkhau_LPT == false) {
    //        if (up_giaban >= giatri_CK) {
    //            ajaxHelper(NhanVienUri + "update_ChietKhau?ID=" + ID_ChietKhau + "&ChietKhau=" + giatri_CK + "&LaPhanTram=" + false).done(function (data) {
    //                if (data != "CNTC")
    //                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng không thành công", "danger");
    //                else
    //                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng thành công", "success");
    //                getAllNhanVienChiTiet();
    //            });

    //        }
    //        else {
    //            ajaxHelper(NhanVienUri + "update_ChietKhau?ID=" + ID_ChietKhau + "&ChietKhau=" + up_giaban + "&LaPhanTram=" + false).done(function (data) {
    //                if (data != "CNTC")
    //                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng không thành công", "danger");
    //                else
    //                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng thành công", "success");
    //                getAllNhanVienChiTiet();
    //            });
    //        }
    //    }
    //    else {
    //        if ($('#cbapdung_' + ID_ChietKhau).is(":checked")) {
    //            var dk = 1;
    //            self.update_CK = ko.observableArray();
    //            ajaxHelper(NhanVienUri + "GetList_AllNhanVienNhomHang?id=" + id_NhomHang_Seach + "&idNhanVien=" + self.selectedNhanVien() + "&maHH=" + _maHH_Seach + "&ID_DonVi=" + _iddonvi + "&nuberPage=" + _pageNumber + "&pageSize=" + _pageSize, 'GET').done(function (data) {
    //                self.update_CK(data.LstData);
    //                if (giatri_CK >= 100) {
    //                    for (var i = 0; i < self.update_CK().length; i++) {
    //                        ajaxHelper(NhanVienUri + "update_ChietKhau?ID=" + self.update_CK()[i].ID + "&ChietKhau=" + '100' + "&LaPhanTram=" + true).done(function (data) {
    //                            if (data != "CNTC")
    //                                dk = 2;
    //                        });
    //                    }
    //                    if (dk != 1)
    //                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng không thành công", "danger");
    //                    else
    //                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng thành công", "success");
    //                    getAllNhanVienChiTiet();
    //                }
    //                else {
    //                    var dk = 1;
    //                    for (var i = 0; i < self.update_CK().length; i++) {
    //                        ajaxHelper(NhanVienUri + "update_ChietKhau?ID=" + self.update_CK()[i].ID + "&ChietKhau=" + giatri_CK + "&LaPhanTram=" + true).done(function (data) {
    //                            if (data != "CNTC")
    //                                dk = 2;
    //                        });
    //                    }
    //                    if (dk != 1)
    //                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng không thành công", "danger");
    //                    else
    //                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng thành công", "success");
    //                    getAllNhanVienChiTiet();
    //                }
    //            });
    //        }
    //        else {
    //            if (giatri_CK >= 100) {
    //                ajaxHelper(NhanVienUri + "update_ChietKhau?ID=" + ID_ChietKhau + "&ChietKhau=" + '100' + "&LaPhanTram=" + true).done(function (data) {
    //                    if (data != "CNTC")
    //                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng không thành công", "danger");
    //                    else
    //                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng thành công", "success");
    //                    getAllNhanVienChiTiet();
    //                });
    //            }

    //            else {
    //                ajaxHelper(NhanVienUri + "update_ChietKhau?ID=" + ID_ChietKhau + "&ChietKhau=" + giatri_CK + "&LaPhanTram=" + true).done(function (data) {
    //                    if (data != "CNTC")
    //                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng không thành công", "danger");
    //                    else
    //                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng thành công", "success");
    //                    getAllNhanVienChiTiet();
    //                });
    //            }
    //        }
    //    }
    //}
    //self.addChietKhauNhanVien_YeuCau = function () {
    //    var giatri_CK = formatNumberToInt($('#number1_' + ID_ThucHien).val());
    //    if (ThucHien_LPT == false) {
    //        if (up_giaban >= giatri_CK) {
    //            ajaxHelper(NhanVienUri + "update_ChietKhau_YeuCau?ID=" + ID_ThucHien + "&ChietKhau=" + giatri_CK + "&LaPhanTram=" + false).done(function (data) {
    //                if (data != "CNTC")
    //                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng không thành công", "danger");
    //                else
    //                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng thành công", "success");
    //                getAllNhanVienChiTiet();
    //            });

    //        }
    //        else {
    //            ajaxHelper(NhanVienUri + "update_ChietKhau_YeuCau?ID=" + ID_ThucHien + "&ChietKhau=" + up_giaban + "&LaPhanTram=" + false).done(function (data) {
    //                if (data != "CNTC")
    //                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng không thành công", "danger");
    //                else
    //                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng thành công", "success");
    //                getAllNhanVienChiTiet();
    //            });
    //        }
    //    }
    //    else {
    //        if ($('#cbapdung1_' + ID_ThucHien).is(":checked")) {
    //            var dk = 1;
    //            self.update_CK = ko.observableArray();
    //            ajaxHelper(NhanVienUri + "GetList_AllNhanVienNhomHang?id=" + id_NhomHang_Seach + "&idNhanVien=" + self.selectedNhanVien() + "&maHH=" + _maHH_Seach + "&ID_DonVi=" + _iddonvi + "&nuberPage=" + _pageNumber + "&pageSize=" + _pageSize, 'GET').done(function (data) {
    //                self.update_CK(data.LstData);
    //                if (giatri_CK >= 100) {
    //                    for (var i = 0; i < self.update_CK().length; i++) {
    //                        ajaxHelper(NhanVienUri + "update_ChietKhau_YeuCau?ID=" + self.update_CK()[i].ID + "&ChietKhau=" + '100' + "&LaPhanTram=" + true).done(function (data) {
    //                            if (data != "CNTC")
    //                                dk = 2;
    //                        });
    //                    }
    //                    if (dk != 1)
    //                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng không thành công", "danger");
    //                    else
    //                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng thành công", "success");
    //                    getAllNhanVienChiTiet();
    //                }
    //                else {
    //                    var dk = 1;
    //                    for (var i = 0; i < self.update_CK().length; i++) {
    //                        ajaxHelper(NhanVienUri + "update_ChietKhau_YeuCau?ID=" + self.update_CK()[i].ID + "&ChietKhau=" + giatri_CK + "&LaPhanTram=" + true).done(function (data) {
    //                            if (data != "CNTC")
    //                                dk = 2;
    //                        });
    //                    }
    //                    if (dk != 1)
    //                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng không thành công", "danger");
    //                    else
    //                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng thành công", "success");
    //                    getAllNhanVienChiTiet();
    //                }
    //            });
    //        }
    //        else {
    //            if (giatri_CK >= 100) {
    //                ajaxHelper(NhanVienUri + "update_ChietKhau_YeuCau?ID=" + ID_ThucHien + "&ChietKhau=" + '100' + "&LaPhanTram=" + true).done(function (data) {
    //                    if (data != "CNTC")
    //                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng không thành công", "danger");
    //                    else
    //                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng thành công", "success");
    //                    getAllNhanVienChiTiet();
    //                });
    //            }

    //            else {
    //                ajaxHelper(NhanVienUri + "update_ChietKhau_YeuCau?ID=" + ID_ThucHien + "&ChietKhau=" + giatri_CK + "&LaPhanTram=" + true).done(function (data) {
    //                    if (data != "CNTC")
    //                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng không thành công", "danger");
    //                    else
    //                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng thành công", "success");
    //                    getAllNhanVienChiTiet();
    //                });
    //            }
    //        }
    //    }
    //}// Hoa hồng gói dịch vụ
    //self.addChietKhauNhanVien_TuVan = function () {
    //    var giatri_CK = formatNumberToInt($('#number2_' + ID_TuVan).val());
    //    console.log(giatri_CK, up_giaban);
    //    //return false;
    //    if (TuVan_LPT == false) {
    //        if (up_giaban >= giatri_CK) {
    //            ajaxHelper(NhanVienUri + "update_ChietKhau_TuVan?ID=" + ID_TuVan + "&ChietKhau=" + giatri_CK + "&LaPhanTram=" + false).done(function (data) {
    //                if (data != "CNTC")
    //                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng không thành công", "danger");
    //                else
    //                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng thành công", "success");
    //                getAllNhanVienChiTiet();
    //            });

    //        }
    //        else {
    //            ajaxHelper(NhanVienUri + "update_ChietKhau_TuVan?ID=" + ID_TuVan + "&ChietKhau=" + up_giaban + "&LaPhanTram=" + false).done(function (data) {
    //                if (data != "CNTC")
    //                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng không thành công", "danger");
    //                else
    //                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng thành công", "success");
    //                getAllNhanVienChiTiet();
    //            });
    //        }
    //    }
    //    else {
    //        if ($('#cbapdung2_' + ID_TuVan).is(":checked")) {
    //            var dk = 1;
    //            self.update_CK = ko.observableArray();
    //            ajaxHelper(NhanVienUri + "GetList_AllNhanVienNhomHang?id=" + id_NhomHang_Seach + "&idNhanVien=" + self.selectedNhanVien() + "&maHH=" + _maHH_Seach + "&ID_DonVi=" + _iddonvi + "&nuberPage=" + _pageNumber + "&pageSize=" + _pageSize, 'GET').done(function (data) {
    //                self.update_CK(data.LstData);
    //                if (giatri_CK >= 100) {
    //                    for (var i = 0; i < self.update_CK().length; i++) {
    //                        ajaxHelper(NhanVienUri + "update_ChietKhau_TuVan?ID=" + self.update_CK()[i].ID + "&ChietKhau=" + '100' + "&LaPhanTram=" + true).done(function (data) {
    //                            if (data != "CNTC")
    //                                dk = 2;
    //                        });
    //                    }
    //                    if (dk != 1)
    //                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng không thành công", "danger");
    //                    else
    //                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng thành công", "success");
    //                    getAllNhanVienChiTiet();
    //                }
    //                else {
    //                    var dk = 1;
    //                    for (var i = 0; i < self.update_CK().length; i++) {
    //                        ajaxHelper(NhanVienUri + "update_ChietKhau_TuVan?ID=" + self.update_CK()[i].ID + "&ChietKhau=" + giatri_CK + "&LaPhanTram=" + true).done(function (data) {
    //                            if (data != "CNTC")
    //                                dk = 2;
    //                        });
    //                    }
    //                    if (dk != 1)
    //                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng không thành công", "danger");
    //                    else
    //                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng thành công", "success");
    //                    getAllNhanVienChiTiet();
    //                }
    //            });
    //        }
    //        else {
    //            if (giatri_CK >= 100) {
    //                ajaxHelper(NhanVienUri + "update_ChietKhau_TuVan?ID=" + ID_TuVan + "&ChietKhau=" + '100' + "&LaPhanTram=" + true).done(function (data) {
    //                    if (data != "CNTC")
    //                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng không thành công", "danger");
    //                    else
    //                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng thành công", "success");
    //                    getAllNhanVienChiTiet();
    //                });
    //            }

    //            else {
    //                ajaxHelper(NhanVienUri + "update_ChietKhau_TuVan?ID=" + ID_TuVan + "&ChietKhau=" + giatri_CK + "&LaPhanTram=" + true).done(function (data) {
    //                    if (data != "CNTC")
    //                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng không thành công", "danger");
    //                    else
    //                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng thành công", "success");
    //                    getAllNhanVienChiTiet();
    //                });
    //            }
    //        }
    //    }
    //}
    self.ApDungChietKhau = function () {
        var _id = this.ID;
        var objUpdate = [];
        var mydata = {};
        //debugger;
        var giaban = formatNumberToInt($('#giavon_' + rowid).val());// gia von de so sanh 

        if ($('#vnd_' + rowid).hasClass('gb')) {
            if (chietkhaumoi <= giaban) {
                console.log(chietkhaumoi);
                console.log(giaban);
                var objUpdatetemp = {
                    ID: _id,
                    ChietKhau: chietkhau,
                    LaPhanTram: laphantram
                };
                console.log(objUpdatetemp);
                objUpdate.push(objUpdatetemp);
                //    }
                //}
            }
            // max = gia von
            else {
                var objUpdatetemp = {
                    ID: _id,
                    ChietKhau: giaban,
                    LaPhanTram: false
                };
                console.log(objUpdatetemp);
                objUpdate.push(objUpdatetemp);
            }
        }


        if ($('#vnd_' + rowid).hasClass('gb') === false) {
            if (chietkhaumoi <= "100") {
                if ($('#cbapdung_' + rowid).is(":checked")) {
                    var objUpdateAll = self.NhanVienChitiets();
                    for (var i = 0; i < objUpdateAll.length; i++) {
                        objUpdateAll[i].ChietKhau = chietkhaumoi;
                        objUpdateAll[i].LaPhanTram = true;
                        objUpdate.push(objUpdateAll[i]);
                    }
                } else {
                    if ($('#cbapdung_' + rowid).is(":checked")) {
                        var objUpdateAllMinus = self.NhanVienChitiets();
                        for (var i = 0; i < objUpdateAllMinus.length; i++) {
                            objUpdateAllMinus[i].ChietKhau = chietkhaumoi;
                            objUpdateAllMinus[i].LaPhanTram = true;
                            objUpdate.push(objUpdateAllMinus[i]);
                        }
                    }
                    else {
                        var objUpdatetemp = {
                            ID: _id,
                            ChietKhau: chietkhau,
                            LaPhanTram: laphantram
                        };
                        objUpdate.push(objUpdatetemp);
                    }
                }
            }
            else {
                debugger;
                if ($('#cbapdung_' + rowid).is(":checked")) {
                    var objUpdateAll = self.NhanVienChitiets();
                    for (var i = 0; i < objUpdateAll.length; i++) {
                        objUpdateAll[i].ChietKhau = 100;
                        objUpdateAll[i].LaPhanTram = true;
                        objUpdate.push(objUpdateAll[i]);
                    }
                } else {
                    if ($('#cbapdung_' + rowid).is(":checked")) {
                        var objUpdateAllMinus = self.NhanVienChitiets();
                        for (var i = 0; i < objUpdateAllMinus.length; i++) {
                            objUpdateAllMinus[i].ChietKhau = 100;
                            objUpdateAllMinus[i].LaPhanTram = true;
                            objUpdate.push(objUpdateAllMinus[i]);
                        }
                    }
                    else {
                        var objUpdatetemp = {
                            ID: _id,
                            ChietKhau: 100,
                            LaPhanTram: laphantram
                        };
                        objUpdate.push(objUpdatetemp);
                    }
                }
            }
        }
        mydata.objData = objUpdate;

        $.ajax({
            url: NhanVienUri + "PutChietKhauChiTiet",
            type: 'POST',
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            data: mydata,
            success: function (result) {
                self.getChiTietNhanVien();
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật cài đặt hoa hồng thành công", "success");
            },
            error: function (error) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cập nhật cài đặt hoa hồng thất bại", "danger");
                $('#modalpopup_deleteGB').modal('hide');
            }
        })
    };

    // yeucau
    self.ApDungYeuCau = function () {
        var _id = this.ID;
        var objUpdate = [];
        var mydata = {};
        debugger;
        var giaban = formatNumberToInt($('#giavon_' + rowid).val());// gia von de so sanh 

        if ($('#vnd1_' + rowid).hasClass('gb')) {
            if (yeucaumoi <= giaban) {
                //if ($('#cbapdung1_' + rowid).is(":checked")) {
                //    var objUpdateAll = self.NhanVienChitiets();
                //    for (var i = 0; i < objUpdateAll.length; i++) {
                //        objUpdateAll[i].YeuCau = yeucaumoi;
                //        objUpdateAll[i].LaPhanTram_YeuCau = false;
                //        objUpdate.push(objUpdateAll[i]);
                //    }
                //} else {
                //    if ($('#cbapdung1_' + rowid).is(":checked")) {
                //        var objUpdateAllMinus = self.NhanVienChitiets();
                //        for (var i = 0; i < objUpdateAllMinus.length; i++) {
                //            objUpdateAllMinus[i].YeuCau = yeucaumoi;
                //            objUpdateAllMinus[i].LaPhanTram_YeuCau = false;
                //            objUpdate.push(objUpdateAllMinus[i]);
                //        }
                //    }
                //    else {
                var objUpdatetemp = {
                    ID: _id,
                    YeuCau: yeucau,
                    LaPhanTram_YeuCau: laphantram_yeucau
                };
                objUpdate.push(objUpdatetemp);
                //    }
                //}
            }
            else {
                var objUpdatetemp = {
                    ID: _id,
                    YeuCau: giaban,
                    LaPhanTram_YeuCau: false
                };
                objUpdate.push(objUpdatetemp);
            }
        }
        if ($('#vnd1_' + rowid).hasClass('gb') === false) {
            if (yeucaumoi <= "100") {
                if ($('#cbapdung1_' + rowid).is(":checked")) {
                    var objUpdateAll = self.NhanVienChitiets();
                    for (var i = 0; i < objUpdateAll.length; i++) {
                        objUpdateAll[i].YeuCau = yeucaumoi;
                        objUpdateAll[i].LaPhanTram_YeuCau = true;
                        objUpdate.push(objUpdateAll[i]);
                    }
                } else {
                    if ($('#cbapdung1_' + rowid).is(":checked")) {
                        var objUpdateAllMinus = self.NhanVienChitiets();
                        for (var i = 0; i < objUpdateAllMinus.length; i++) {
                            objUpdateAllMinus[i].YeuCau = yeucaumoi;
                            objUpdateAllMinus[i].LaPhanTram_YeuCau = true;
                            objUpdate.push(objUpdateAllMinus[i]);
                        }
                    }
                    else {
                        var objUpdatetemp = {
                            ID: _id,
                            YeuCau: yeucau,
                            LaPhanTram_YeuCau: laphantram_yeucau
                        };
                        objUpdate.push(objUpdatetemp);
                    }
                }
            }
            else {
                if ($('#cbapdung1_' + rowid).is(":checked")) {
                    var objUpdateAll = self.NhanVienChitiets();
                    for (var i = 0; i < objUpdateAll.length; i++) {
                        objUpdateAll[i].YeuCau = 100;
                        objUpdateAll[i].LaPhanTram_YeuCau = true;
                        objUpdate.push(objUpdateAll[i]);
                    }
                } else {
                    if ($('#cbapdung1_' + rowid).is(":checked")) {
                        var objUpdateAllMinus = self.NhanVienChitiets();
                        for (var i = 0; i < objUpdateAllMinus.length; i++) {
                            objUpdateAllMinus[i].YeuCau = 100;
                            objUpdateAllMinus[i].LaPhanTram_YeuCau = true;
                            objUpdate.push(objUpdateAllMinus[i]);
                        }
                    }
                    else {
                        var objUpdatetemp = {
                            ID: _id,
                            YeuCau: 100,
                            LaPhanTram_YeuCau: laphantram_yeucau
                        };
                        objUpdate.push(objUpdatetemp);
                    }
                }
            }
        }
        mydata.objData = objUpdate;

        $.ajax({
            url: NhanVienUri + "PutYeuCauChiTiet",
            type: 'POST',
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            data: mydata,
            success: function (result) {
                self.getChiTietNhanVien();
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật yêu cầu thành công", "success");
            },
            error: function (error) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cập nhật yêu cầu thất bại", "danger");
                $('#modalpopup_deleteGB').modal('hide');
            }
        })
    };

    // tu van
    self.ApDungTuVan = function () {
        var _id = this.ID;
        var objUpdate = [];
        var mydata = {};
        debugger;
        var giaban = formatNumberToInt($('#giavon_' + rowid).val());// gia von de so sanh 
        if ($('#vnd2_' + rowid).hasClass('gb')) {
            if (tuvanmoi <= giaban) {
                //if ($('#cbapdung2_' + rowid).is(":checked")) {
                //    var objUpdateAll = self.NhanVienChitiets();
                //    for (var i = 0; i < objUpdateAll.length; i++) {
                //        objUpdateAll[i].TuVan = tuvanmoi;
                //        objUpdateAll[i].LaPhanTram_TuVan = false;
                //        objUpdate.push(objUpdateAll[i]);
                //    }
                //} else {
                //    if ($('#cbapdung2_' + rowid).is(":checked")) {
                //        var objUpdateAllMinus = self.NhanVienChitiets();
                //        for (var i = 0; i < objUpdateAllMinus.length; i++) {
                //            objUpdateAllMinus[i].TuVan = tuvanmoi;
                //            objUpdateAllMinus[i].LaPhanTram_TuVan = false;
                //            objUpdate.push(objUpdateAllMinus[i]);
                //        }
                //    }
                //    else {
                var objUpdatetemp = {
                    ID: _id,
                    TuVan: tuvan,
                    LaPhanTram_TuVan: laphantram_tuvan
                };
                objUpdate.push(objUpdatetemp);
                //    }
                //}
            }
            else {
                var objUpdatetemp = {
                    ID: _id,
                    TuVan: giaban,
                    LaPhanTram_TuVan: false
                };
                objUpdate.push(objUpdatetemp);
            }
        }
        if ($('#vnd2_' + rowid).hasClass('gb') === false) {
            if (tuvanmoi <= "100") {
                if ($('#cbapdung2_' + rowid).is(":checked")) {
                    var objUpdateAll = self.NhanVienChitiets();
                    for (var i = 0; i < objUpdateAll.length; i++) {
                        objUpdateAll[i].TuVan = tuvanmoi;
                        objUpdateAll[i].LaPhanTram_TuVan = true;
                        objUpdate.push(objUpdateAll[i]);
                    }
                } else {
                    if ($('#cbapdung2_' + rowid).is(":checked")) {
                        var objUpdateAllMinus = self.NhanVienChitiets();
                        for (var i = 0; i < objUpdateAllMinus.length; i++) {
                            objUpdateAllMinus[i].TuVan = tuvanmoi;
                            objUpdateAllMinus[i].LaPhanTram_TuVan = true;
                            objUpdate.push(objUpdateAllMinus[i]);
                        }
                    }
                    else {
                        var objUpdatetemp = {
                            ID: _id,
                            TuVan: tuvan,
                            LaPhanTram_TuVan: laphantram_tuvan

                        };
                        objUpdate.push(objUpdatetemp);
                    }
                }
            }
            else {
                if ($('#cbapdung2_' + rowid).is(":checked")) {
                    var objUpdateAll = self.NhanVienChitiets();
                    for (var i = 0; i < objUpdateAll.length; i++) {
                        objUpdateAll[i].TuVan = 100;
                        objUpdateAll[i].LaPhanTram_TuVan = true;
                        objUpdate.push(objUpdateAll[i]);
                    }
                } else {
                    if ($('#cbapdung2_' + rowid).is(":checked")) {
                        var objUpdateAllMinus = self.NhanVienChitiets();
                        for (var i = 0; i < objUpdateAllMinus.length; i++) {
                            objUpdateAllMinus[i].TuVan = 100;
                            objUpdateAllMinus[i].LaPhanTram_TuVan = true;
                            objUpdate.push(objUpdateAllMinus[i]);
                        }
                    }
                    else {
                        var objUpdatetemp = {
                            ID: _id,
                            TuVan: 100,
                            LaPhanTram_TuVan: laphantram_tuvan

                        };
                        objUpdate.push(objUpdatetemp);
                    }
                }
            }
        }

        mydata.objData = objUpdate;

        $.ajax({
            url: NhanVienUri + "PutTuVanChiTiet",
            type: 'POST',
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            data: mydata,
            success: function (result) {
                self.getChiTietNhanVien();
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật tư vấn thành công", "success");
            },
            error: function (error) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cập nhật tư vấn thất bại", "danger");
                $('#modalpopup_deleteGB').modal('hide');
            }
        })
    };

    //trinhpv import HangHoaXuatHuy
    //Download file teamplate excel format (*.xls)
    var DMHangHoaUri = '/api/DanhMuc/DM_HangHoaAPI/';
    self.DownloadFileTeamplateXLS = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_CaiDatHoaHong.xls";
        window.location.href = url;
    }
    //Download file teamplate excel format (*.xlsx)
    self.DownloadFileTeamplateXLSX = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_CaiDatHoaHong.xlsx";
        window.location.href = url;
    }
    self.fileNameExcel = ko.observable("Lựa chọn file Excel...");
    $(".btnImportExcel").hide();
    $(".filterFileSelect").hide();
    self.deleteFileSelect = function () {
        self.fileNameExcel("Lựa chọn file Excel...");
        $(".filterFileSelect").hide();
        $(".btnImportExcel").hide();
        $(".NoteImport").show();
        document.getElementById('imageUploadForm').value = "";
    }
    self.refreshFileSelect = function () {
        self.importChietKhau();
    }
    self.SelectedFileImport = function (vm, evt) {
        self.fileNameExcel(evt.target.files[0].name)
        $(".NoteImport").hide();
        $(".btnImportExcel").show();
        $(".filterFileSelect").show();
        self.loiExcel([]);
    }
    self.showmodalImport = function () {
        console.log(self.selectedNhanVien());
        if (self.selectedNhanVien() != undefined)
            $('#myModalinport').modal('show');
        else
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn chưa chọn nhân viên cài đặt hoa hồng", "danger");
    }
    self.loiExcel = ko.observableArray();
    self.importChietKhau = function () {
        LoadingForm(true);
        var formData = new FormData();
        var totalFiles = document.getElementById("imageUploadForm").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("imageUploadForm").files[i];
            formData.append("imageUploadForm", file);
        }
        $.ajax({
            type: "POST",
            url: NhanVienUri + "ImfortExcelChietKhau",
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (item) {
                self.loiExcel(item);
                if (self.loiExcel().length > 0) {
                    $(".BangBaoLoi").show();
                    $(".btnImportExcel").hide();
                    $(".refreshFile").show();
                    $(".deleteFile").hide();
                    LoadingForm(false);
                }
                else {
                    $.ajax({
                        type: "POST",
                        url: NhanVienUri + "getList_DanhSachHangChietKhau?ID_ChiNhanh=" + $('#hd_IDdDonVi').val() + "&ID_NhanVien=" + self.selectedNhanVien(),
                        data: formData,
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        success: function (item) {
                            $('#myModalinport').modal('hide');
                            self.getChiTietNhanVien();
                            bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i> ' + "Import hàng hóa cài đặt hoa hồng thành công", "success");
                        },
                        statusCode: {
                            500: function (item) {
                                LoadingForm(false);
                                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Import danh sách hàng cài đặt hoa hồng thất bại", "danger");
                            }
                        },
                    })
                }
                LoadingForm(false);
            },
            statusCode: {
                406: function (item) {
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + item.responseJSON.Message, "danger")
                    LoadingForm(false);
                },
                500: function (item) {
                    LoadingForm(false);
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Import danh sách hàng cài đặt hoa hồng thất bại", "danger");
                }
            },
        });
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
        return str;
    }
    self.listAddChiNhanh = ko.observableArray();
    var ID_news = 1;
    refreshChiNhanh();
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
    function EditStaffphongbanchinhanh(item) {
        refreshChiNhanh();
        if (item != null && item.length > 0) {
            var t = 1;
            for (var i = 0; i < item.length; i++) {
                item[i].ID = t;
                t++;
            }
            self.listAddChiNhanh(item);
            self.listAddChiNhanh.refresh;
        }
    }

    self.AddChiNhanh = function () {
        if (self.listAddChiNhanh().some(o => o.ID_ChiNhanh === null || o.ID_PhongBan === null)) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> Vui lòng chọn chi nhánh phòng ban trước khi thêm ', "danger");

        }
        else {
            ID_news += 1;
            self.listAddChiNhanh.push({
                ID: ID_news,
                ID_ChiNhanh: null,
                Text_ChiNhanh: "",
                ID_PhongBan: null,
                Text_PhongBan: "",
                listPhongBan: [],
                LaMacDinh: false
            });
        }
    }
    self.RemoveChiNhanh = function (item) {
        self.listAddChiNhanh(self.listAddChiNhanh().filter(o => o.ID !== item.ID));
        self.listAddChiNhanh.refresh();
    };
    self.SeletecChiNhanh = function (root, item) {
        root.ID_ChiNhanh = item.ID;
        root.Text_ChiNhanh = item.TenDonVi;
        self.listAddChiNhanh.refresh();
        $.getJSON(NhanVienUri + "GetTreePhongBan?chinhanhId=" + item.ID, function (data) {
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
    //phân trang
    self.pageSizes = [10, 30, 50];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.currentPage = ko.observable(0);
    self.fromitem = ko.observable(1);
    self.toitem = ko.observable();
    self.arrPagging = ko.observableArray();

    self.PageCount = ko.computed(function () {
        if (self.pageSize()) {
            if (self.arrPagging() !== null) {
                return Math.ceil(self.arrPagging().length / self.pageSize());
            }
            else {
                return 0
            }
        }
        else {
            return 1;
        }
    });

    self.filteredNhanVienChitiets = ko.computed(function () {
        var first = self.currentPage() * self.pageSize();
        var _filter = self.filter();
        var arrFilter = '';

        arrFilter = ko.utils.arrayFilter(self.NhanVienChitiets(), function (prod) {
            var chon = true;
            var arr = locdau(prod.TenHangHoa).toLowerCase().split(/\s+/);
            var sSearch = '';
            for (var i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }
            chon = (locdau(prod.TenHangHoa).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
                locdau(prod.MaHangHoa).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
                sSearch.indexOf(locdau(_filter).toLowerCase()) >= 0
            );
            return chon;
        });
        if (arrFilter !== null) {
            self.arrPagging(arrFilter);

            self.fromitem((self.currentPage() * self.pageSize()) + 1);

            if (((self.currentPage() + 1) * self.pageSize()) > arrFilter.length) {
                self.toitem(arrFilter.length)
            } else {
                self.toitem(first + self.pageSize());
            }

            if (self.PageCount() > 1) {
                if (self.currentPage() === self.PageCount()) {
                    self.currentPage(0);
                }
                return arrFilter.slice(first, first + self.pageSize());
            }
            else {
                return arrFilter;
            }
        }
    });


    self.PageResults = ko.computed(function () {
        if (self.NhanVienChitiets() !== null) {
            self.fromitem((self.currentPage() * self.pageSize()) + 1);
            if (((self.currentPage() + 1) * self.pageSize()) > self.NhanVienChitiets().length) {
                self.toitem(self.NhanVienChitiets().length)
            } else {
                self.toitem((self.currentPage() * self.pageSize()) + self.pageSize());
            }

            if (self.PageCount() > 1) {
                return self.NhanVienChitiets().slice(self.currentPage() * self.pageSize(), (self.currentPage() * self.pageSize()) + self.pageSize())
            }
            else {
                return self.NhanVienChitiets();
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
        self.currentPage(1);
        _pageSize = self.pageSize();
        _pageNumber = 1;
        getAllNhanVienChiTiet();
    };

    self.GoToPage = function (page) {
        self.currentPage(page);
    };

    self.GetClass = function (page) {
        return (page === self.currentPage()) ? "click" : "";
    };
    //trinhpv
    self.filterSeach = function (item, inputString) {
    }
    self.SelectedHHEnterkey = function () {
        self.MaHangHoa_Search($('#txtAutoHangHoa').val().toUpperCase());
        getChiTietHangHoaByMaHH($('#txtAutoHangHoa').val().toUpperCase());
    }
    function getChiTietHangHoaByMaHH(MaHH) {
        if (self.selectedNhanVien() != undefined) {
            $.ajax({
                type: "GET",
                url: NhanVienUri + "AddChiTietbyMaHH?maHH=" + MaHH + "&idnhanvien=" + self.selectedNhanVien() + "&iddonvi=" + _iddonvi,
                success: function (result) {
                    if (result === true) {
                        self.getChiTietNhanVien();
                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i> ' + "Cập nhật hàng hóa cài đặt hoa hồng thành công", "success");
                    }
                    else {
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Mã hàng hóa: " + self.MaHangHoa_Search() + " đã tồn tại trong danh sách cài đặt hoa hồng nhân viên!", "danger");
                    }
                    self.selectedHH(undefined);
                    $('#txtAutoHangHoa').val(self.MaHangHoa_Search());
                    $('#text_MaHangHoa').focus();
                    $('#txtAutoHangHoa').focus().select();
                }
            });
        }
        else
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn chưa chọn nhân viên cài đặt hoa hồng", "danger");
    }
    self.DonVis = ko.observableArray();
    self.SelectedAllDonVi = function () {
        self.MangChiNhanhNhanVien(self.DonVis());
        for (var i = 0; i < self.MangChiNhanhNhanVien().length; i++) {
            $('#selec-all-DonVi li').each(function () {
                if ($(this).attr('id') === self.MangChiNhanhNhanVien()[i].ID) {
                    $(this).find('.fa-check').remove();
                    $(this).append('<i class="fa fa-check check-after-li"></i>')
                }
            });
        }
    }
    self.SelectedDonVi = function (item) {
        var arrIDDonVi = [];
        for (var i = 0; i < self.MangChiNhanhNhanVien().length; i++) {
            if ($.inArray(self.MangChiNhanhNhanVien()[i], arrIDDonVi) === -1) {
                arrIDDonVi.push(self.MangChiNhanhNhanVien()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDDonVi) === -1) {
            self.MangChiNhanhNhanVien.push(item);
        }
        //$('#choose_DonVi input').remove();
        // thêm dấu check vào đối tượng được chọn
        $('#selec-all-DonVi li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
                $(this).append('<i class="fa fa-check check-after-li"></i>')
            }
        });
        console.log(self.MangChiNhanhNhanVien())
    }
    self.CloseDonVi = function (item) {
        self.MangChiNhanhNhanVien.remove(item);
        if (self.MangChiNhanhNhanVien().length === 0) {
            //$('#choose_DonVi').append('<input type="text" class="dropdown" placeholder="Chọn chi nhánh...">');
        }
        // remove check
        $('#selec-all-DonVi li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
        console.log(self.MangChiNhanhNhanVien())
    }
    // lấy về danh sách đơn vị
    var BH_DonViUri = '/api/DanhMuc/DM_DonViAPI/';
    function getDonVi() {
        ajaxHelper(BH_DonViUri + "GetListDonVi1", "GET").done(function (data) {
            self.DonVis(data);
        });
    }
    getDonVi();
    //xuất file excel
    self.ExportExcel = function () {
        if (self.selectedNhanVien() != undefined) {
            if (self.NhanVienChitiets().length < 1) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Không có dữ liệu để xuất file Excel", "danger");
            }
            else {
                var objDiary = {
                    ID_NhanVien: _id_NhanVien,
                    ID_DonVi: _iddonvi,
                    ChucNang: "Cài đặt hoa hồng",
                    NoiDung: "Xuất báo cáo hàng hóa cài đặt hoa hồng theo nhân viên: " + self._Ten(),
                    NoiDungChiTiet: "Xuất báo cáo hàng hóa cài đặt hoa hồng theo nhân viên: " + self._Ten(),
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
                        var columnHide = null;
                        var url = NhanVienUri + "ExportExcelChietKhau_NhanVien?MaHH=" + _maHH_Seach + "&ID_NhanVien=" + self.selectedNhanVien() + "&ID_DonVi=" + _iddonvi + "&LaHangHoa=" + _laHangHoa + "&TinhTrang=" + TinhTrangHH + "&ID_NhomHang=" + id_NhomHang_Seach + "&columnsHide=" + columnHide + "&TenChiNhanh=" + $('#_txtTenDonVi').text() + "&TenNhanVien=" + $('#txtAuto').val();
                        window.location.href = url;
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
        }
        else {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn chưa chọn nhân viên cài đặt hoa hồng", "danger");
        }
    }
    self.selectNameNV = ko.observable();
    //lấy về danh sách nhân viên
    var _IDchinhanh = $('#hd_IDdDonVi').val();
    var _tennhanvien_seach = null;
    var timer1 = null;
    self.NoteNhanVien = function () {
        clearTimeout(timer1);
        _tennhanvien_seach = $('#txtAuto').val();
        console.log(_tennhanvien_seach);
        timer1 = setTimeout(getAllNSNhanVien(), 500);
    }
    $('#txtSearchNV_ChuaCaiDat').keypress(function (e) {
        if (e.keyCode == 13)
            _nhanvienchuaCD = $(this).val();
        getlist_NhanVienChuaCaiDat();
    });
    $('#txtSearchNV_DaCaiDat').keypress(function (e) {
        if (e.keyCode == 13)
            _nhanviendaCD = $(this).val();
        getlist_NhanVienDaCaiDat();
    });
    $('#txtAuto').keypress(function (e) {
        if (e.keyCode == 13) {
            self.SelectNhanVien(self.NhanViens()[0]);
        }
    });

    function getAllNSNhanVien() {
        ajaxHelper(NhanVienUri + "getListNhanVien_DonVi?ID_ChiNhanh=" + _IDchinhanh + "&nameNV=" + _tennhanvien_seach, 'GET').done(function (data) {
            self.NhanViens(data);
            if (_tennhanvien_seach === '') {
                self.selectedNhanVien(undefined);
            }
        });
    }
    getAllNSNhanVien();
    self.SelectNhanVien = function (item) {
        console.log('a');
        self.selectNameNV(item.TenNhanVien);
        self.selectedNhanVien(item.ID);
        id_NhomHang_Seach = null;
        getAllNhanVienChiTiet();
    }
    // phân trang
    self.currentPage = ko.observable(1);
    self.GetClass = function (page) {
        return (page.SoTrang === self.currentPage()) ? "click" : "";
    };
    self.selecPage = function () {
        if (AllPage > 4) {
            for (var i = 3; i < AllPage; i++) {
                self.ListPage.pop(i + 1);
            }
            self.ListPage.push({ SoTrang: 4 });
            self.ListPage.push({ SoTrang: 5 });
        }
        else {
            for (var i = 0; i < 6; i++) {
                self.ListPage.pop(i);
            }
            for (var j = 0; j < AllPage; j++) {
                self.ListPage.push({ SoTrang: j + 1 });
            }
        }
        $('#StartPage').hide();
        $('#BackPage').hide();
        $('#NextPage').show();
        $('#EndPage').show();
    }
    self.ReserPage = function (item) {
        //self.selecPage();
        if (_pageNumber > 1 && AllPage > 5/* && nextPage < AllPage - 1*/) {
            if (_pageNumber > 3 && _pageNumber < AllPage - 1) {
                for (var i = 0; i < 5; i++) {
                    self.ListPage.replace(self.ListPage()[i], { SoTrang: parseInt(_pageNumber) + i - 2 });
                }
            }
            else if (parseInt(_pageNumber) === parseInt(AllPage) - 1) {
                for (var i = 0; i < 5; i++) {
                    self.ListPage.replace(self.ListPage()[i], { SoTrang: parseInt(_pageNumber) + i - 3 });
                }
            }
            else if (_pageNumber == AllPage) {
                for (var i = 0; i < 5; i++) {
                    self.ListPage.replace(self.ListPage()[i], { SoTrang: parseInt(_pageNumber) + i - 4 });
                }
            }
            else if (_pageNumber < 4) {
                for (var i = 0; i < 5; i++) {
                    self.ListPage.replace(self.ListPage()[i], { SoTrang: 1 + i });
                }
            }
        }
        else if (_pageNumber == 1 && AllPage > 5) {
            for (var i = 0; i < 5; i++) {
                self.ListPage.replace(self.ListPage()[i], { SoTrang: parseInt(_pageNumber) + i });
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
    self.NextPage = function (item) {
        if (_pageNumber < AllPage) {
            _pageNumber = _pageNumber + 1;
            self.ReserPage();
            getAllNhanVienChiTiet();
        }
    };
    self.BackPage = function (item) {
        if (_pageNumber > 1) {
            _pageNumber = _pageNumber - 1;
            self.ReserPage();
            getAllNhanVienChiTiet();
        }
    };
    self.EndPage = function (item) {
        _pageNumber = AllPage;
        self.ReserPage();
        getAllNhanVienChiTiet();
    };
    self.StartPage = function (item) {
        _pageNumber = 1;
        self.ReserPage();
        getAllNhanVienChiTiet();
    };
    self.gotoNextPage = function (item) {
        _pageNumber = item.SoTrang;
        self.ReserPage();
        getAllNhanVienChiTiet();
    }
}

ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        //initialize datepicker with some optional options
        var options = allBindingsAccessor().datepickerOptions || {};

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

var ctgiavm = new ViewModel();
ko.applyBindings(ctgiavm)

function keypressEnterSelected(e) {
    if (e.keyCode == 13) {
        ctgiavm.SelectedHHEnterkey();
    }
}
function itemSelected_LoHang(item) {
    ctgiavm.SelectedHHEnterkey_LoHang(item);
}
function itemSelected() {
    ctgiavm.SelectedHHEnterkey();
}
function hidewait(o) {
    $('.' + o).append('<div id="wait"><img src="/Content/images/wait.gif" width="64" height="64" /><div class="happy-wait">' +
        ' </div>' +
        '</div>')
}
var arrID_NhanVien = [];
function SetCheckAllChildsCCD(obj) {
    var thisID = $(obj).attr('id');
    $(obj).removeClass('squarevt');
    var isChecked = $(obj).is(":checked");
    $('.NhanVien_ChuaCaiDat input').each(function () {
        $(this).prop('checked', isChecked);
    })

    if (isChecked) {
        $('.NhanVien_ChuaCaiDat input').each(function () {
            var thisID = $(this).attr('id');
            if (thisID !== undefined && !(jQuery.inArray(thisID, arrID_NhanVien) > -1)) {
                arrID_NhanVien.push(thisID);
            }
        })
    }
    else {
        $('.NhanVien_ChuaCaiDat input').each(function () {
            var thisID = $(this).attr('id');
            if (thisID !== undefined && (jQuery.inArray(thisID, arrID_NhanVien) > -1)) {
                $.map(arrID_NhanVien, function (item, i) {
                    if (item === thisID) {
                        arrID_NhanVien.splice(i, 1);
                    }
                })
            }
        })
    }
    console.log(arrID_NhanVien);
}
function SetCheckAllChild2sCCD(obj) {
    $('#all_nhanvienchuacaidat').prop('checked', false);
    var count = 0;
    var countcheck = 0;
    var thisID = $(obj).attr('id');
    $('#all_nhanvienchuacaidat').addClass('squarevt');
    $('.NhanVien_ChuaCaiDat input').each(function () {
        if ($(this).is(':checked')) {
            countcheck = countcheck + 1;
        }
        count += 1;
    })
    if (count === countcheck) {
        $('#all_nhanvienchuacaidat').removeClass('squarevt');
        $('#all_nhanvienchuacaidat').prop('checked', true);
    }
    if (countcheck === 0) {
        $('#all_nhanvienchuacaidat').removeClass('squarevt');
        $('#all_nhanvienchuacaidat').prop('checked', false);
    }
    if ($(obj).is(':checked')) {
        if (thisID !== undefined && !(jQuery.inArray(thisID, arrID_NhanVien) > -1)) {
            arrID_NhanVien.push(thisID);
        }
    }
    else {
        //remove item in arrID_NhanVien
        $.map(arrID_NhanVien, function (item, i) {
            if (item === thisID) {
                arrID_NhanVien.splice(i, 1);
            }
        })
    }
    console.log(arrID_NhanVien);
}
function SetCheckAllChildsDCD(obj) {
    var thisID = $(obj).attr('id');
    $(obj).removeClass('squarevt');
    var isChecked = $(obj).is(":checked");
    $('.NhanVien_DaCaiDat input').each(function () {
        $(this).prop('checked', isChecked);
    })

    if (isChecked) {
        $('.NhanVien_DaCaiDat input').each(function () {
            var thisID = $(this).attr('id');
            if (thisID !== undefined && !(jQuery.inArray(thisID, arrID_NhanVien) > -1)) {
                arrID_NhanVien.push(thisID);
            }
        })
    }
    else {
        $('.NhanVien_DaCaiDat input').each(function () {
            var thisID = $(this).attr('id');
            if (thisID !== undefined && (jQuery.inArray(thisID, arrID_NhanVien) > -1)) {
                $.map(arrID_NhanVien, function (item, i) {
                    if (item === thisID) {
                        arrID_NhanVien.splice(i, 1);
                    }
                })
            }
        })
    }
    console.log(arrID_NhanVien);
}
function SetCheckAllChild2sDCD(obj) {
    $('#all_nhanviendacaidat').prop('checked', false);
    var count = 0;
    var countcheck = 0;
    var thisID = $(obj).attr('id');
    $('#all_nhanviendacaidat').addClass('squarevt');
    $('.NhanVien_DaCaiDat input').each(function () {
        if ($(this).is(':checked')) {
            countcheck = countcheck + 1;
        }
        count += 1;
    })
    if (count === countcheck) {
        $('#all_nhanviendacaidat').removeClass('squarevt');
        $('#all_nhanviendacaidat').prop('checked', true);
    }
    if (countcheck === 0) {
        $('#all_nhanviendacaidat').removeClass('squarevt');
        $('#all_nhanviendacaidat').prop('checked', false);
    }
    if ($(obj).is(':checked')) {
        if (thisID !== undefined && !(jQuery.inArray(thisID, arrID_NhanVien) > -1)) {
            arrID_NhanVien.push(thisID);
        }
    }
    else {
        //remove item in arrID_NhanVien
        $.map(arrID_NhanVien, function (item, i) {
            if (item === thisID) {
                arrID_NhanVien.splice(i, 1);
            }
        })
    }
    console.log(arrID_NhanVien);
}
ko.observableArray.fn.refresh = function () {
    var data = this().slice(0);
    this([]);
    this(data);
};