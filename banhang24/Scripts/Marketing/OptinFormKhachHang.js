var ViewModalKhachHang = function () {
    var self = this;
    self.TenOF = ko.observable();
    self.NoiDungOF = ko.observable();
    self.GhiChuOF = ko.observable();
    self.TableTruongThongTin = ko.observableArray();
    self.ThongBaoThanhCongOF = ko.observable('');
    self.WebDieuHuongOF = ko.observable();
    self.TrangThaiThoiGianOF = ko.observable(1);
    var thisDate1 = new Date;
    //self.TuNgayOF = ko.observable(moment(new Date(thisDate1)).format('YYYY-MM-DD'));
    self.TuNgayOF = ko.observable();
    self.DenNgayOF = ko.observable(null);
    self.ButtonNameOF = ko.observable('');
    self.ThongBaoHieuLucOF = ko.observable('');
    self.MaNhungOF = ko.observable();
    self.ListNhanVienOF = ko.observableArray();
    self.ID_TinhThanh = ko.observable();
    self.ID_QuanHuyen = ko.observable();
    self.GoiY = ko.observable();
    self.ID_OptinForm = ko.observable(null);
    self.ID_OptinFormUpdate = ko.observable(null);
    self.ID_OptinFormBieuDo = ko.observable(null);
    self.DM_OptinForm = ko.observableArray();
    var OptinFormUri = '/api/DanhMuc/OptinFormAPI/';
    var DMDoiTuongUri = '/api/DanhMuc/DM_DoiTuongAPI/';
    self.ID_KhuVuc = ko.observable();
    self.ID_ViTri = ko.observable();
    self.LoaiOF = ko.observable(1);
    self.LoaiTruongThongTin = ko.observable(1);
    self.NguoiTaoOF = ko.observable();
    self.RowsStart = ko.observable('1');
    self.RowsEnd = ko.observable('10');
    self.pageNumber = ko.observable(1);
    self.pageSize = ko.observable(10);
    self.SumRowsOF = ko.observable();
    self.SumNumberPageReport = ko.observableArray();
    self.LoaiBaoCao = ko.observable("OptinForm");
    self.arrNT = ko.observableArray();
    self.SoLuotDangKy = ko.observableArray();
    self.DM_LinkKhachHangDangKy = ko.observableArray();
    var itemOF_Delete = null;
    self.deleteTenOF = ko.observable();
    var _pageNumber = 1;
    var _pageSize = 10;
    var AllPage;
    var datime = new Date();
    var _timeStart = moment(new Date(datime.setDate(datime.getDate() - 6))).format('YYYY-MM-DD');
    var newtime = new Date();
    var _timeBC = moment(new Date(newtime.setDate(newtime.getDate()))).format('YYYY-MM-DD');
    var _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
    var _id_NhanVien = $('.idnhanvien').text();
    var _id_DonVi = $('#hd_IDdDonVi').val();// lấy ID chi nhánh
    var lc_CapNhat = JSON.parse(localStorage.getItem('lc_CapNhat'));
    self.nameNhanVien = ko.observable();
    self.NameBieuDo = ko.observable(' Đăng ký khách hàng');
    self.getNameNhanVien = function () {
        ajaxHelper('/api/DanhMuc/TQ_DoanhThuAPI/' + "getNameNhanVien?ID_NhanVien=" + _id_NhanVien, "GET").done(function (data) {
            self.NguoiTaoOF(data);
            console.log(self.NguoiTaoOF());
        });
    }
    self.getNameNhanVien();
    $('#text_ThongBaoThangCongOF').keyup(function () {
        self.ThongBaoThanhCongOF($(this).val());
    });
    $('#text_ThongBaoHieuLucOF').keyup(function () {
        self.ThongBaoHieuLucOF($(this).val());
    });
    $('#text_ButtonNameOF').keyup(function () {
        self.ButtonNameOF($(this).val());
    });
    $('#text_WebDieuHuongOF').keyup(function () {
        self.WebDieuHuongOF($(this).val());
    });
    $('#text_TenOF').keyup(function () {
        self.TenOF($(this).val());
    });
    $('#text_NoiDungOF').keyup(function () {
        self.NoiDungOF($(this).val());
    });
    $('#text_GhiChuOF').keyup(function () {
        self.GhiChuOF($(this).val());
    });
    $('#datetime_TuNgay').keypress(function (e) {
        if (e.keyCode == 13) {
            dktime = $(this).val();
            thisDate = $(this).val();
            var t = thisDate.split(" ");
            var t1 = t[0].split("/").reverse().join("-")
            thisDate = moment(t1).format('MM/DD/YYYY')
            if (thisDate != 'Invalid date') {
                self.TuNgayOF(moment(new Date(thisDate)).format('YYYY-MM-DD'));
                console.log(self.TuNgayOF());
            }
            else {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Định dạng thời gian không chính xác!", "danger");
            }
        }
    });
    $('#datetime_TuNgay').on('change.dp', function (e) {
        dktime = $(this).val();
        thisDate = $(this).val();
        var t = thisDate.split(" ");
        var t1 = t[0].split("/").reverse().join("-")
        thisDate = moment(t1).format('MM/DD/YYYY')
        self.TuNgayOF(moment(new Date(thisDate)).format('YYYY-MM-DD'));
        console.log(self.TuNgayOF());
    });
    $('#datetime_DenNgay').keypress(function (e) {
        if (e.keyCode == 13) {
            dktime = $(this).val();
            thisDate = $(this).val();
            var t = thisDate.split(" ");
            var t1 = t[0].split("/").reverse().join("-")
            thisDate = moment(t1).format('MM/DD/YYYY')
            if (thisDate != 'Invalid date') {
                self.DenNgayOF(moment(new Date(thisDate)).format('YYYY-MM-DD'));
                console.log(self.DenNgayOF());
            }
            else {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Định dạng thời gian không chính xác!", "danger");
            }
        }
    });
    $('#datetime_DenNgay').on('change.dp', function (e) {
        dktime = $(this).val();
        thisDate = $(this).val();
        var t = thisDate.split(" ");
        var t1 = t[0].split("/").reverse().join("-")
        thisDate = moment(t1).format('MM/DD/YYYY')
        self.DenNgayOF(moment(new Date(thisDate)).format('YYYY-MM-DD'));
        console.log(self.DenNgayOF());
    });
   
    var subdoman;
    var host;
    function get_SubDomain() {
        ajaxHelper(OptinFormUri + "get_SubDomain", "GET").done(function (data) {
            subdoman = data;
            host = 'https://' + subdoman + '.open24.vn' + '/Scripts/SDK/sdkcustomer.js#version=v3.1';
            console.log(subdoman, host)
            self.getHTML_OptinFormKhachHang();
        });
        
    }
    
    self.getHTML_OptinFormKhachHang = function () {
        var manhung = '@*The code is for later body*@'
            + '<div id="open24-root" style=" margin-top: 15px;"></div>'
            + "@*The code to the end body*@"
            + '<script> (function (d, s, id,dataid) {'
            + ' var js, fjs = d.getElementsByTagName(s)[0];'
            + '   if (d.getElementById(id)) return;'
            + '    js = d.createElement(s); js.id = id;'
            + "  document.getElementById('open24-root').setAttribute('data-id', dataid);"
            + "  js.src = " + host
            + '  fjs.parentNode.insertBefore(js, fjs);'
            + " }(document, 'script', 'open24-root-id'," + subdoman + "," + self.ID_OptinForm() +"))</script>";
        self.MaNhungOF(manhung);
    };
    
    self.TinhThanhs = ko.observableArray();
    self.QuanHuyens = ko.observableArray();
    if (lc_CapNhat != null) {
        self.TenOF(lc_CapNhat.TenOptinForm);
        self.GhiChuOF(lc_CapNhat.GhiChu);
        self.NoiDungOF(lc_CapNhat.NoiDung);
        self.MaNhungOF(lc_CapNhat.MaNhung);
        self.ID_OptinForm(lc_CapNhat.ID);
        self.ID_OptinFormUpdate(lc_CapNhat.ID);
        $('#copy_Code').removeAttr('disabled');
        $('#text_code').removeAttr('disabled');
        $('#text_code').off('selectstart', false);
    }
    else
    {
        get_SubDomain();
        $('#copy_Code').attr('disabled', 'disabled');
        $('#text_code').on('selectstart', false);
        $('#text_code').attr('disabled', 'disabled');
    }
    function getListTinhThanh() {
        ajaxHelper(DMDoiTuongUri + "GetListTinhThanh", 'GET').done(function (x) {
            if (x.res === true) {
                self.TinhThanhs(x.data);
            }
        });
    }
    getListTinhThanh();

    self.getListQuanHuyen = function (id) {
        if (id !== undefined) {
            ajaxHelper(DMDoiTuongUri + "GetListQuanHuyen?idTinhThanh=" + id, 'GET').done(function (data) {
                if (data != null) {
                    self.QuanHuyens(data);
                }
            });
        }
    }
    self.FilterProvince = ko.computed(function () {
        var inputSearch = $('#text_TinhThanh').val();
        var locdauInput = locdau(inputSearch);
        return $.grep(self.TinhThanhs(), function (x) {
            var itemSearch = locdau(x.TenTinhThanh);
            return itemSearch.indexOf(locdauInput) > -1;
        });

    });
    self.ID_KhuVuc.subscribe(function (newValue) {
        self.ID_TinhThanh(newValue);
        self.ID_QuanHuyen(null);
        self.ID_ViTri(null);
        self.getListQuanHuyen(newValue);
    });
    //JqAutu Quận huyện
    self.FilterProvinceQH = ko.computed(function () {
        var inputSearch = $('#text_QuanHuyen').val();
        var locdauInput = locdau(inputSearch);
        return $.grep(self.QuanHuyens(), function (x) {
            var itemSearch = locdau(x.TenQuanHuyen);
            return itemSearch.indexOf(locdauInput) > -1;
        });

    });
    self.ID_ViTri.subscribe(function (newValue) {
        self.ID_QuanHuyen(newValue);
    });
    self.OF_TruongThongTin = ko.observableArray();
    function getList_TruongThongTinOF() {
        ajaxHelper(OptinFormUri + "getList_TruongThongTinOF?ID_OptinForm=" + self.ID_OptinForm() + "&LoaiTruongThongTin=" + self.LoaiTruongThongTin()).done(function (data) {
            self.OF_TruongThongTin(data.LstData);
            self.GoiY(self.OF_TruongThongTin()[11].GoiY);
            self.ThongBaoThanhCongOF(self.OF_TruongThongTin()[0].NoiDungThongBao);
            self.WebDieuHuongOF(self.OF_TruongThongTin()[0].WebDieuHuong);
            self.ButtonNameOF(self.OF_TruongThongTin()[0].ButtonName);
            self.ThongBaoHieuLucOF(self.OF_TruongThongTin()[0].NoiDungHieuLuc);
            self.TrangThaiThoiGianOF(self.OF_TruongThongTin()[0].TrangThaiThoiGian);
            if (self.TrangThaiThoiGianOF()) {
                $('#datetime_TuNgay').prop("readonly", false);
                $('#datetime_DenNgay').prop("readonly", false);
                $('._thong-bao-hieu-luc').prop("readonly", false);
                $('#datetime_TuNgay').removeAttr('disabled');
                $('#datetime_DenNgay').removeAttr('disabled');
            }
            else {
                $('#datetime_TuNgay').prop("readonly", true);
                $('#datetime_DenNgay').prop("readonly", true);
                $('._thong-bao-hieu-luc').prop("readonly", true);
                $('#datetime_TuNgay').attr('disabled', 'false');
                $('#datetime_DenNgay').attr('disabled', 'false');
            }
            self.TuNgayOF(self.OF_TruongThongTin()[0].TuNgay);
            self.DenNgayOF(self.OF_TruongThongTin()[0].DenNgay);
            $('#datetime_DenNgay').val("");
            $('#datetime_TuNgay').datetimepicker({
                timepicker: false,
                mask: true, // '9999/19/39 29:59' - digit is the maximum possible for a cell
                format: 'd/m/Y',
                value: moment(self.TuNgayOF()).format('DD/MM/YYYY')
            });
            $('#datetime_DenNgay').datetimepicker({
                timepicker: false,
                mask: true, // '9999/19/39 29:59' - digit is the maximum possible for a cell
                format: 'd/m/Y',
                value: self.DenNgayOF() != null ? moment(self.DenNgayOF()).format('DD/MM/YYYY') : null
            });
            if (self.OF_TruongThongTin()[0].checkSuDung != 1)
                $('#AnhDaiDien').hide();
            else
                $('#AnhDaiDien').show();
            if (self.OF_TruongThongTin()[1].checkSuDung != 1)
                $('#TenKhachHang').hide();
            else
                $('#TenKhachHang').show();
            if (self.OF_TruongThongTin()[2].checkSuDung != 1)
                $('#GioiTinh').hide();
            else
                $('#GioiTinh').show();
            if (self.OF_TruongThongTin()[3].checkSuDung != 1)
                $('#NgaySinh').hide();
            else
                $('#NgaySinh').show();
            if (self.OF_TruongThongTin()[4].checkSuDung != 1)
                $('#SoDienThoai').hide();
            else
                $('#SoDienThoai').show();
            if (self.OF_TruongThongTin()[5].checkSuDung != 1)
                $('#Email').hide();
            else
                $('#Email').show();
            if (self.OF_TruongThongTin()[6].checkSuDung != 1)
                $('#DiaChi').hide();
            else
                $('#DiaChi').show();
            if (self.OF_TruongThongTin()[7].checkSuDung != 1)
                $('#TinhThanh').hide();
            else
                $('#TinhThanh').show();
            if (self.OF_TruongThongTin()[8].checkSuDung != 1)
                $('#QuanHuyen').hide();
            else
                $('#QuanHuyen').show();
            if (self.OF_TruongThongTin()[9].checkSuDung != 1)
                $('#MaSoThue').hide();
            else
                $('#MaSoThue').show();
            if (self.OF_TruongThongTin()[10].checkSuDung != 1)
                $('#KhachLe').hide();
            else
                $('#KhachLe').show();
            if (self.OF_TruongThongTin()[11].checkSuDung != 1)
                $('#NguoiGioiThieu').hide();
            else
                $('#NguoiGioiThieu').show();
            if (self.OF_TruongThongTin()[0].checkBatBuoc != 1)
                $('#AnhDaiDien label span').hide();
            else
                $('#AnhDaiDien label span').show();
            if (self.OF_TruongThongTin()[1].checkBatBuoc != 1)
                $('#TenKhachHang label span').hide();
            else
                $('#TenKhachHang label span').show();
            if (self.OF_TruongThongTin()[2].checkBatBuoc != 1)
                $('#GioiTinh label span').hide();
            else
                $('#GioiTinh label span').show();
            if (self.OF_TruongThongTin()[3].checkBatBuoc != 1)
                $('#NgaySinh label span').hide();
            else
                $('#NgaySinh label span').show();
            if (self.OF_TruongThongTin()[4].checkBatBuoc != 1)
                $('#SoDienThoai label span').hide();
            else
                $('#SoDienThoai label span').show();
            if (self.OF_TruongThongTin()[5].checkBatBuoc != 1)
                $('#Email label span').hide();
            else
                $('#Email label span').show();
            if (self.OF_TruongThongTin()[6].checkBatBuoc != 1)
                $('#DiaChi label span').hide();
            else
                $('#DiaChi label span').show();
            if (self.OF_TruongThongTin()[7].checkBatBuoc != 1)
                $('#TinhThanh label span').hide();
            else
                $('#TinhThanh label span').show();
            if (self.OF_TruongThongTin()[8].checkBatBuoc != 1)
                $('#QuanHuyen label span').hide();
            else
                $('#QuanHuyen label span').show();
            if (self.OF_TruongThongTin()[9].checkBatBuoc != 1)
                $('#MaSoThue label span').hide();
            else
                $('#MaSoThue label span').show();
            if (self.OF_TruongThongTin()[10].checkBatBuoc != 1)
                $('#KhachLe label span').hide();
            else
                $('#KhachLe label span').show();
            if (self.OF_TruongThongTin()[11].checkBatBuoc != 1)
                $('#NguoiGioiThieu label span').hide();
            else
                $('#NguoiGioiThieu label span').show();
            if (self.OF_TruongThongTin()[11].GoiY == '')
                $('#tolTipGoiY').hide();
            else
                $('#tolTipGoiY').show();
        });
    };
    getList_TruongThongTinOF();
    self.OF_SoLuotTruyCap = ko.observable();
    self.OF_SoLuotDangKy = ko.observable();
    self.getList_OptinFrom = function () {
        ajaxHelper(OptinFormUri + "getList_OptinForm?LoaiOF=" + self.LoaiOF()).done(function (data) {
            self.DM_OptinForm(data.LstData);
            AllPage = data.numberPage;
            self.selecPage();
            self.ReserPage();
            self.SumRowsOF(data.Rowcount);
            self.OF_SoLuotTruyCap(data.a1);
            self.OF_SoLuotDangKy(data.a2);
            if (data.LstData.length > 0)
            {
                self.NameBieuDo(self.DM_OptinForm()[0].TenOptinForm);   
                self.ID_OptinFormBieuDo(self.DM_OptinForm()[0].ID);
                if ($('#tab_khachhang').hasClass('active'))
                    self.getList_SoLuotDangKy(self.DM_OptinForm()[0].ID);
            }
           
        });
    };
    self.getList_OptinFrom();
    self.DM_OptinForm_page = ko.computed(function (x) {
        var first = (self.pageNumber() - 1) * self.pageSize();
        if (self.DM_OptinForm() !== null) {
            if (self.DM_OptinForm().length != 0) {
                //$('.TC_ChiTiet').show();
                //$(".Report_Empty").hide();
                $(".page").show();
                self.RowsStart((self.pageNumber() - 1) * self.pageSize() + 1);
                self.RowsEnd((self.pageNumber() - 1) * self.pageSize() + self.DM_OptinForm().slice(first, first + self.pageSize()).length)
            }
            else {
                //$('.TC_ChiTiet').hide();
                //$(".Report_Empty").show();
                $(".page").hide();
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            return self.DM_OptinForm().slice(first, first + self.pageSize());
        }
        return null;
    });
    //lựa chọn optinForm
    $('#OptinForm_filter').on('change', function () {
        self.ID_OptinForm($(this).val())
        getList_TruongThongTinOF();
    });
    self.valueSD_changed = function (item) {
        var dk = item.checkSuDung;
        if (dk)
            item.checkSuDung = 1;
        else
            item.checkSuDung = 0
    }
    self.valueBB_changed = function (item) {
        var dk = item.checkBatBuoc;
        if (dk)
            item.checkBatBuoc = 1;
        else
            item.checkBatBuoc = 0
    }
    function isValid(str) {
        return !/[~`!@#$%\^&*()+=\-\[\]\\';,/{}|\\":<>\?]/g.test(str);
    };
    // Thêm mới
    self.Create_OptinForm = function () {
        localStorage.removeItem('lc_CapNhat');
        //window.location.href = '/#/AddOptinFormKH';
        if ($('#tab_khachhang').hasClass('active')) {
            location.href = "/#/AddOptinFormKH";
        }
        else {
            location.href = "/#/AddOptinFormLH";
        }
    }
    self.Add_OptinForm = function () {
        document.getElementById("add_HoanThanh").disabled = true;
        //document.getElementById("add_HoanThanh").lastChild.data = " Đang lưu";
        var HoaDon;
        var OptinForm = {
            TenOptinForm: self.TenOF(),
            LoaiOptinForm: self.LoaiOF(),
            NoiDung: self.NoiDungOF(),
            MaNhung: self.MaNhungOF(),
            TuNgay: self.TuNgayOF(),
            DenNgay: self.DenNgayOF(),
            TrangThaiThoiGian: self.TrangThaiThoiGianOF() == 1 ? true : false,
            NguoiTao: self.NguoiTaoOF(),
            GhiChu: self.GhiChuOF()
        }
        var objThietLapThongBao = {
            NoiDungThongBao: self.ThongBaoThanhCongOF(),
            WebDieuHuong: self.WebDieuHuongOF(),
            NoiDungHieuLuc: self.ThongBaoHieuLucOF(),
            ButtonName: self.ButtonNameOF()
        }
        var myData = {};
        myData.OptinForm = OptinForm;
        myData.objThietLapTruongThongTin = self.OF_TruongThongTin();
        myData.objThietLapThongBao = objThietLapThongBao;
        if (self.ID_OptinFormUpdate() != null)
            callAjaxUpdate(myData);
        else
            callAjaxAdd(myData);
    }

    function callAjaxAdd(myData) {
        $.ajax({
            data: myData,
            url: OptinFormUri + "PostOF_KhachHang?ID_NhanVien=" + _id_NhanVien + "&ID_DonVi=" + _id_DonVi,
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (item) {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Tạo OptinForm thành công", "success");
                self.ID_OptinForm(item.ID);
                self.ID_OptinFormUpdate(item.ID);
                self.getHTML_OptinFormKhachHang();
                $('#copy_Code').removeAttr('disabled');
                $('#text_code').removeAttr('disabled');
                $('#text_code').off('selectstart', false);
                document.getElementById("add_HoanThanh").disabled = false;
                localStorage.removeItem('lc_CapNhat');
                //location.href = "/#/OptinForm";
                //self.getList_OptinFrom();
            },
            statusCode: {
                404: function (item) {
                    self.error("page not found");
                    document.getElementById("add_HoanThanh").disabled = false;
                },
                500: function (item) {
                    document.getElementById("add_HoanThanh").disabled = false;
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Tạo OptinForm không thành công", "danger");
                }
            },
            complete: function (item) {
                document.getElementById("add_HoanThanh").disabled = false;

            }
        })
    }
    // Sửa
    self.edit_OptinForm = function (item) {
        localStorage.removeItem('lc_CapNhat');
        localStorage.setItem('lc_CapNhat', JSON.stringify(item));
        window.location.href = '/#/AddOptinFormKH';
    }
    self.edit_TrangThaiOF = function (item) {
        var dk = item.TrangThai;
        if (dk)
            item.TrangThai = 1;
        else
            item.TrangThai = 0
        var myData = {};
        myData.OptinForm = item;
        $.ajax({
            data: myData,
            url: OptinFormUri + "Edit_OptinForm?ID_NhanVien=" + _id_NhanVien + "&ID_DonVi=" + _id_DonVi,
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (item) {
            },
            statusCode: {
                404: function (item) {
                    self.error("page not found");
                },
                500: function (item) {
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật OptinForm không thành công", "danger");
                }
            },
            complete: function (item) {
            }
        })
    }
    function callAjaxUpdate(myData) {
        $.ajax({
            data: myData,
            url: OptinFormUri + "PutOF_KhachHang?ID_NhanVien=" + _id_NhanVien + "&ID_DonVi=" + _id_DonVi + "&ID_OptinForm=" + self.ID_OptinFormUpdate(),
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (item) {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật OptinForm thành công", "success");
                document.getElementById("add_HoanThanh").disabled = false;
                localStorage.removeItem('lc_CapNhat');
                location.href = "/#/OptinForm";
            },
            statusCode: {
                404: function (item) {
                    self.error("page not found");
                    document.getElementById("add_HoanThanh").disabled = false;
                },
                500: function (item) {
                    document.getElementById("add_HoanThanh").disabled = false;
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật OptinForm không thành công", "danger");
                }
            },
            complete: function (item) {
                document.getElementById("add_HoanThanh").disabled = false;

            }
        })
    }
    // xóa
   
    self.modalDelete = function (item) {
        itemOF_Delete = item;
        self.deleteTenOF(item.TenOptinForm);
        $('#modalpopup_deleteHD').modal('show');
    };
    self.Delete_TrangThaiOF = function () {
        var myData = {};
        myData.OptinForm = itemOF_Delete;
        $.ajax({
            data: myData,
            url: OptinFormUri + "Delete_OptinForm?ID_NhanVien=" + _id_NhanVien + "&ID_DonVi=" + _id_DonVi,
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (item) {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Xóa OptinForm thành công", "success");
                $('#modalpopup_deleteHD').modal('hide');
                self.getList_OptinFrom();
            },
            statusCode: {
                404: function (item) {
                    self.error("page not found");
                },
                500: function (item) {
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Xóa OptinForm không thành công", "danger");
                }
            },
            complete: function (item) {
            }
        })
    }
    // biểu đồ
    self.selectedRownt_OptinForm = function (item) {
        self.ID_OptinFormBieuDo(item.ID);
        self.NameBieuDo(item.TenOptinForm);
        if ($('#tab_khachhang').hasClass('active'))
            self.getList_SoLuotDangKy(self.ID_OptinFormBieuDo());
    };
    $('.newDateTime').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        var dt = new Date(picker.endDate.format('MM/DD/YYYY'));
        var dtBC = new Date(picker.endDate.format('MM/DD/YYYY'));
        _timeStart = picker.startDate.format('YYYY-MM-DD');
        _timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');// picker.endDate.format('YYYY-MM-DD');
        var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate()))).format('YYYY-MM-DD');
        if ($('#tab_khachhang').hasClass('active'))
        self.getList_SoLuotDangKy(self.ID_OptinFormBieuDo());
    });
    self.arrDS = ko.observableArray();
    self.getList_SoLuotDangKy = function (item) {
        self.arrNT([]);
        self.arrDS([]);
        self.SoLuotDangKy([]);
        ajaxHelper(OptinFormUri + "getList_BieuDoDangKyOF?ID_OptinForm=" + item + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd, "GET").done(function (data) {
            for (var i = 0; i < data.LstData.length; i++) {
                self.arrDS.push(data.LstData[i].SoLuotDangKy);
                self.arrNT.push(data.LstData[i].ThoiGian);
            }
            var obj = {
                name: "Số lượt đăng ký",
                turboThreshold: 0,
                _colorIndex: 0,
                data: self.arrDS()
                //data: [5, 10, 12, 20, 10, 30, 35, 15]
            }
            self.SoLuotDangKy.push(obj);
            self.getList_BieuDo();
        });
        ajaxHelper(OptinFormUri + "getList_linkKhachHangDangKyOF?ID_OptinForm=" + item, "GET").done(function (data) {
            self.DM_LinkKhachHangDangKy(data.LstData);
        });
    }
    self.getList_BieuDo = function () {
        var chart = Highcharts.chart('lineChartKhachHang', {
            chart: {
                type: 'line',
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
            series: self.SoLuotDangKy(),
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
                "#0097c4"
            ],
            legend: {
                align: "center",
                verticalAlign: "bottom",
            },
            credits: {
                enabled: false,
            },
        });
    }
    self.show_DanhSachDoiTuongDangKy = function () {
        var item = {
            //Check_TrangThaiXuLy: 1,
            //Check_TrangThaiChuaXuLy: 1,
            //Check_TrangThaiHuyBo: 0,
            //Check_TrangThaiFormBat: 1,
            //Check_TrangThaiFormTat: 1,
            //Check_TrangThaiFormXoa: 0,
            ID_OptinForm: null,
            Ten_OptinForm: null,
            LoaiOptinForm: 1
        };
        localStorage.setItem('lc_DoiTuongOF', JSON.stringify(item));
        location.href = "/#/DanhSachDangKyOptinForm";
        //var url = "/#/DanhSachDangKyOptinForm";
        //window.open(url);
    }

    self.show_DanhSachDoiTuongDangKyIndex = function (item) {
        localStorage.setItem('lc_DoiTuongOF', JSON.stringify(item));
        location.href = "/#/DanhSachDangKyOptinForm";
    }
    // copy mã nhúng
    self.Copy_MaNhung = function (item) {
        var $temp = $("<input>");
        $("body").append($temp);
        $temp.val(item.MaNhung).select();
        document.execCommand("copy");
        $temp.remove();
    };
    //Phân trang
    self.currentPage = ko.observable(1);
    self.GetClass = function (page) {
        return (page.SoTrang === self.currentPage()) ? "click" : "";
    };
    self.selecPage = function () {
        self.SumNumberPageReport([]);
        if (AllPage > 4) {
            self.SumNumberPageReport.push({ SoTrang: 1 });
            self.SumNumberPageReport.push({ SoTrang: 2 });
            self.SumNumberPageReport.push({ SoTrang: 3 });
            self.SumNumberPageReport.push({ SoTrang: 4 });
            self.SumNumberPageReport.push({ SoTrang: 5 });
        }
        else {
            for (var j = 0; j < AllPage; j++) {
                self.SumNumberPageReport.push({ SoTrang: j + 1 });
            }
        }
        $('#StartPage').hide();
        $('#BackPage').hide();
        $('#NextPage').show();
        $('#EndPage').show();
    }
    self.ReserPage = function (item) {
        // loadHtmlGrid();
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
    self.NextPage = function (item) {
        if (_pageNumber < AllPage) {
            _pageNumber = _pageNumber + 1;
            self.pageNumber(_pageNumber);
            self.ReserPage();
        }
    };
    self.BackPage = function (item) {
        if (_pageNumber > 1) {
            _pageNumber = _pageNumber - 1;
            self.pageNumber(_pageNumber);
            self.ReserPage();
        }
    };
    self.EndPage = function (item) {
        _pageNumber = AllPage;
        self.pageNumber(_pageNumber);
        self.ReserPage();
    };
    self.StartPage = function (item) {
        _pageNumber = 1;
        self.pageNumber(_pageNumber);
        self.ReserPage();
    };
    self.gotoNextPage = function (item) {
        _pageNumber = item.SoTrang;
        self.pageNumber(_pageNumber);
        self.ReserPage();
    }
};
ko.applyBindings(new ViewModalKhachHang());