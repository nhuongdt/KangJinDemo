var PartieView_NewUserContact = function () {
    var self = this;
    const txtProvince_modal = '#txtProvince_modal';
    const txtDistrict_modal = '#txtDistrict_modal';
    self.ID = ko.observable();
    self.ID_DoiTuong = ko.observable();
    self.MaLienHe = ko.observable();
    self.TenLienHe = ko.observable();
    self.SoDienThoai = ko.observable();
    self.DienThoaiCoDinh = ko.observable();
    self.Email = ko.observable();
    self.NgaySinh = ko.observable();
    self.GhiChu = ko.observable();
    self.NguoiTao = ko.observable();
    self.NguoiSua = ko.observable();
    self.ID_QuanHuyen = ko.observable();
    self.ID_TinhThanh = ko.observable();
    self.DiaChi = ko.observable();
    self.ChucVu = ko.observable();
    self.XungHo = ko.observable();
    self.SetData = function (item) {
        self.ID(item.ID);
        self.MaLienHe(item.MaLienHe);
        self.TenLienHe(item.TenLienHe);
        self.SoDienThoai(item.SoDienThoai);
        self.DienThoaiCoDinh(item.DienThoaiCoDinh);
        self.Email(item.Email);
        self.GhiChu(item.GhiChu);
        self.DiaChi(item.DiaChi);
        self.ChucVu(item.ChucVu);
        self.XungHo(item.XungHo);
        var ngaysinh = item.NgaySinh;
        if (ngaysinh === null || ngaysinh === undefined) {
            ngaysinh = "";
        }
        else {
            ngaysinh = moment(ngaysinh, "YYYY-MM-DD hh:mm:ss").format("DD/MM/YYYY");
        }
        self.NgaySinh(ngaysinh);
        if (item.ID_DoiTuong !== null && item.ID_DoiTuong.indexOf('0000') === -1) {
            self.ID_DoiTuong(item.ID_DoiTuong);
        }
        else {
            self.ID_DoiTuong(undefined);
        }
        if (item.ID_TinhThanh !== null && item.ID_TinhThanh.indexOf('0000') === -1) {
            self.ID_TinhThanh(item.ID_TinhThanh);
            $(txtProvince_modal).text(item.TenTinhThanh);
            var lstDistrict_byIDTinhThanh = $.grep(self.listDistrict(), function (x) {
                return x.ID_TinhThanh === item.ID_TinhThanh;
            });
            self.District_byProvice(lstDistrict_byIDTinhThanh);
        }
        else {
            self.ID_TinhThanh(undefined);
            $(txtProvince_modal).text('--Chọn Tỉnh thành--');
        }
        if (item.ID_QuanHuyen !== null && item.ID_QuanHuyen.indexOf('0000') === -1) {
            self.ID_QuanHuyen(item.ID_QuanHuyen);
            $(txtDistrict_modal).text(item.TenQuanHuyen);
        }
        else {
            self.ID_QuanHuyen(undefined);
            $(txtDistrict_modal).text('--Chọn quận huyện--');
        }
    }
    self.searchTT = ko.observable();
    self.listProvince = ko.observableArray(); // get this list at KhachHang.js
    self.searchDistrict = ko.observable();
    self.listDistrict = ko.observableArray();
    self.District_byProvice = ko.observableArray();// add popup
    self.AllUserContact = ko.observableArray(); // all user contact
    self.FileImgs = ko.observableArray();
    self.FilesSelect = ko.observableArray();
    self.HaveImage_Select = ko.observableArray();
    self.HaveImage = ko.observable(false);
    self.AnhDaiDien = ko.observableArray();
    self.ImageIsZoom = ko.observableArray();
    self.UserLogin = ko.observable();
    self.booleanAdd_LienHe = ko.observable(true);
    var DMLienHeUri = '/api/DanhMuc/DM_LienHeAPI/';
    var DMDoiTuongUri = '/api/DanhMuc/DM_DoiTuongAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/'
    self.ListXungHo = ko.observableArray([
        { ID: 0, Text: '- Xưng hô -' },
        { ID: 1, Text: 'Anh' },
        { ID: 2, Text: 'Chị' },
        { ID: 3, Text: 'Cô' },
        { ID: 4, Text: 'Chú' },
    ])
    self.LoadSearchKhachHang = function (list) {
        var index = -1;
        var model_KH = new Vue({
            el: '#divSearchKH',
            data: function () {
                return {
                    query_Kh: '',
                    data_kh: list,
                }
            },
            methods: {
                reset: function (item) {
                    this.data_kh = item;
                    this.query_Kh = '';
                },
                click: function (item) {
                    self.ChoseCustomer_Modal(item);
                    $('#showseach_Kh').hide();
                },
                submit: function (event) {
                    if (event.keyCode === 13) {
                        var result = this.fillter_KH(this.query_Kh);
                        var focus = false;
                        $('#showseach_Kh ul li').each(function (i) {
                            if ($(this).hasClass('hoverenabled')) {
                                $('#showseach_Kh').hide();
                                focus = true;
                                self.ChoseCustomer_Modal(result[i]);
                            }
                        });
                        if (result.length > 0 && this.query_Kh !== '' && focus === false) {
                            $('#showseach_Kh').hide();
                        }
                    }
                    else if (event.keyCode === 40)//dows
                    {
                        index = index + 1;
                        if (index >= ($("#showseach_Kh ul li").length)) {
                            index = 0;
                            $('#showseach_Kh').stop().animate({
                                scrollTop: $('#showseach_Kh').offset().top - 600
                            }, 1000);
                        }
                        else if (index > 9 && index < $("#showseach_Kh ul li").length) {
                            $('#showseach_Kh').stop().animate({
                                scrollTop: $('#showseach_Kh').offset().top + 500
                            }, 1000);
                        }
                        this.loadFocus();
                    }
                    else if (event.keyCode === 38)//up
                    {
                        index = index - 1;
                        if (index < 0) {
                            index = $("#showseach_Kh ul li").length - 1;
                            $('#showseach_Kh').stop().animate({
                                scrollTop: $('#showseach_Kh').offset().top + 500
                            }, 1000);
                        }
                        else if (index > 0 && index < 10) {
                            $('#showseach_Kh').stop().animate({
                                scrollTop: $('#showseach_Kh').offset().top - 600
                            }, 1000);
                        }
                        this.loadFocus();
                    }
                },
                loadFocus: function () {
                    $('#showseach_Kh ul li').each(function (i) {
                        $(this).removeClass('hoverenabled');
                        if (index === i) {
                            $(this).addClass('hoverenabled');
                        }
                    });
                },
                // Tìm kiếm khách hàg
                fillter_KH: function (value) {
                    if (value === '') return this.data_kh.slice(0, 50);
                    return this.data_kh.filter(function (item) {
                        return SearchTxt_inVue(value.split(" "), item['Name_Phone']) === true;
                    }).slice(0, 50);
                },
            },
            computed: {
                // Return Khách hàng
                SearchKhachHang: function () {
                    var result = this.fillter_KH(this.query_Kh);
                    if (result.length < 1 || this.query_Kh === '') {
                        $('#showseach_Kh').hide();
                    }
                    else {
                        index = 0;
                        $('#showseach_Kh').show();
                    }
                    $('#showseach_Kh ul li').each(function (i) {
                        if (i === 0) {
                            $(this).addClass('hoverenabled');
                        }
                        else {
                            $(this).removeClass('hoverenabled');
                        }
                    });
                    $('#showseach_Kh').stop().animate({
                        scrollTop: $('#showseach_Kh').offset().top - 600
                    }, 1000);
                    return result;
                }
            }
        });
    }
    self.FilterProvince_LienHe = ko.computed(function () {
        if (self.searchTT() === null || self.searchTT() === undefined || self.searchTT() === '') {
            return self.listProvince();
        }
        else {
            return self.listProvince().filter(function (item) {
                return SearchTxt_inVue(self.searchTT().split(" "), item['TenTinhThanh']) === true;
            });
        }
    });
    self.ChoseProvince_Modal = function (item) {
        self.searchTT('');
        self.searchDistrict('');
        $(txtDistrict_modal).text('--Chọn quận huyện--');
        self.ID_QuanHuyen(undefined);
        if (item.ID == undefined) {
            $(txtProvince_modal).text('--Chọn Tỉnh thành--');
            self.ID_TinhThanh(undefined);
        }
        else {
            $(txtProvince_modal).text(item.TenTinhThanh);
            self.ID_TinhThanh(item.ID);
            // get list district by ID_Province
            var lstDistrict_byIDTinhThanh = $.grep(self.listDistrict(), function (x) {
                return x.ID_TinhThanh === item.ID;
            });
            self.District_byProvice(lstDistrict_byIDTinhThanh);
        }
    }
    self.ChoseDistrict_Modal = function (item) {
        if (item.ID == undefined) {
            $(txtDistrict_modal).text('--Chọn quận huyện--');
            self.ID_QuanHuyen(undefined);
        }
        else {
            $(txtDistrict_modal).text(item.TenQuanHuyen);
            self.ID_QuanHuyen(item.ID);
        }
    }
    self.FilterDistrict_LienHe = ko.computed(function () {
        if (self.searchDistrict() === null || self.searchDistrict() === undefined || self.searchDistrict() === '') {
            return self.District_byProvice();
        }
        else {
            return self.District_byProvice().filter(function (item) {
                return SearchTxt_inVue(self.searchDistrict().split(" "), item['TenQuanHuyen']) === true;
            });
        }
    });
    self.ChoseCustomer_Modal = function (item) {
        $('#txtCustomer_modal').val(item.TenDoiTuong);
        self.ID_DoiTuong(item.ID);
    }
    self.formatDateTime = function () {
        $('.datepicker_mask').datetimepicker(
            {
                format: "d/m/Y",
                mask: true,
                timepicker: false,
            });
    }
    function Enable_btnSave() {
        $('#btnSave').removeAttr('disabled');
        $('#btnSave').text('Lưu');
    }
    function GetUserContact_byDoiTuong(idDoiTuong) {
        var sWhere = " DM_LienHe.ID_DoiTuong like '%25" + idDoiTuong + "%25'";
        ajaxHelper('/api/DanhMuc/DM_LienHeAPI/' + 'GetAllUserContact_byWhere_FilterNhanVien?txtSearch=' + sWhere, 'GET').done(function (result) {
        });
    }
    self.addUpdate_UserContract = function (formElement) {
        $('#btnSave').attr('disabled', 'disabled');
        $('#btnSave').text('Đang lưu');
        var id = self.ID();
        var maLienHe = self.MaLienHe();
        var tenLienHe = self.TenLienHe();
        var idQuanHuyen = self.ID_QuanHuyen();
        var idTinhThanh = self.ID_TinhThanh();
        var ngaysinh = self.NgaySinh();
        var email = self.Email();
        var ghichu = self.GhiChu();
        var dienthoai = self.SoDienThoai();
        var dienthoaiCoDinh = self.DienThoaiCoDinh();
        var idDoiTuong = self.ID_DoiTuong();
        var diaChi = self.DiaChi();
        var chucVu = self.ChucVu();
        var xungHo = self.XungHo();
        if (idTinhThanh == undefined) {
            idTinhThanh = null;
        }
        if (idQuanHuyen == undefined) {
            idQuanHuyen = null;
        }
        var msgCheck = CheckInput(self);
        if (msgCheck !== '') {
            ShowMessage_Danger(msgCheck);
            Enable_btnSave();
            return false;
        }
        if (ngaysinh !== null && ngaysinh !== undefined && ngaysinh !== '') {
            ngaysinh = moment(ngaysinh, 'DD/MM/YYYY').format('YYYY-MM-DD');
            var checkNS = isValidDateYYYYMMDD(ngaysinh);
            if (!checkNS) {
                Enable_btnSave();
                return;
            }
        }
        var DM_LienHe = {
            ID: id,
            ID_DoiTuong: idDoiTuong,
            MaLienHe: maLienHe,
            XungHo: xungHo, // 0: anh, chi, co, bac, chu (todo)
            TenLienHe: tenLienHe,
            SoDienThoai: dienthoai,
            DienThoaiCoDinh: dienthoaiCoDinh, //(todo)
            Email: email,
            DiaChi: diaChi,
            ChucVu: chucVu,
            NgaySinh: ngaysinh,
            GhiChu: ghichu,
            ID_TinhThanh: idTinhThanh,
            ID_QuanHuyen: idQuanHuyen,
            NguoiTao: self.UserLogin(),
        };
        console.log('DM_LienHe', DM_LienHe);
        if (navigator.onLine) {
            // insert LienHe
            if (self.booleanAdd_LienHe() === true) {
                $.ajax({
                    url: DMLienHeUri + "AddDM_LienHe",
                    type: 'POST',
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    data: DM_LienHe,
                    success: function (item) {
                        // push in to list
                        self.AllUserContact.unshift(item);
                        Insert_NhatKyThaoTac(item, 1, 1);
                        self.InsertImage(item.ID, item.MaLienHe);
                        ShowMessage_Success('Thêm mới người liên hệ thành công');
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        ShowMessage_Danger('Thêm mới người liên hệ thất bại');
                    },
                    complete: function () {
                        $('#modalPopuplg_Contact').modal('hide');
                        Enable_btnSave();
                    }
                })
            }
            // update LienHe
            else {
                DM_LienHe.NguoiSua = self.UserLogin();
                $.ajax({
                    url: DMLienHeUri + "UpdateDM_LienHe",
                    type: 'PUT',
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    data: DM_LienHe,
                    success: function (item) {
                        for (var i = 0; i < self.AllUserContact().length; i++) {
                            if (self.AllUserContact()[i].ID === id) {
                                self.AllUserContact.remove(self.AllUserContact()[i]);
                                break;
                            }
                        }
                        self.AllUserContact.unshift(item);
                        Insert_NhatKyThaoTac(item, 1, 2);
                        self.InsertImage(item.ID, item.MaLienHe);
                        ShowMessage_Success('Cập nhật người liên hệ thành công');
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        ShowMessage_Danger('Cập nhậtngười liên hệ thất bại');
                    },
                    complete: function () {
                        $('#modalPopuplg_Contact').modal('hide');
                        Enable_btnSave();
                    }
                })
            }
        }
    }
    function Insert_NhatKyThaoTac(objUsing, chucNang = 1, loaiNhatKy = 1) {
        // chuc nang (1.DM_LienHe, 4.Export, 5.Import)
        var tenChucNang = '';
        var noiDung = '';
        var noiDungChiTiet = '';
        var txtFirst = '';
        var tenChucNangLowercase = ' người liên hệ ';
        var style1 = '<a style= \"cursor: pointer\" onclick = \"';
        var style2 = "('";
        var style3 = "')\" >";
        var style4 = '</a>';
        var funcNameKH = 'LoadNguoiLienHe_byMaKH';
        switch (loaiNhatKy) {
            case 1:
                txtFirst = 'Thêm mới ';
                break;
            case 2:
                txtFirst = 'Cập nhật ';
                break;
            case 3:
                txtFirst = 'Xóa ';
                break;
            case 5:
                txtFirst = 'Import ';
                break;
            case 6:
                txtFirst = 'Xuất file ';
                break;
        }
        if (chucNang === 1) {
            tenChucNang = 'Người liên hệ';
            if (loaiNhatKy < 4) {
                // them, sua, xoa
                var maLienHe = objUsing.MaLienHe;
                var ngaySinh = '';
                var tenDoiTuong = '';
                var dienThoai = '';
                if (objUsing.NgaySinh !== null && objUsing.NgaySinh !== undefined) {
                    ngaySinh = 'Ngày sinh: ' + moment(objUsing.NgaySinh, 'YYYY-MM-DD HH:mm:ss').format('DD/MM/YYYY') + ', ';
                }
                if (objUsing.SoDienThoai !== null && objUsing.SoDienThoai !== undefined) {
                    dienThoai = 'Điện thoại: ' + objUsing.SoDienThoai + ', ';
                }
                noiDung = txtFirst.concat(tenChucNangLowercase, maLienHe, ', Tên: ', objUsing.TenLienHe, ', ', ngaySinh, dienThoai, tenDoiTuong);
                noiDungChiTiet = txtFirst.concat(tenChucNangLowercase, style1, funcNameKH, style2, maLienHe, style3, maLienHe, style4, ', tên: ', objUsing.TenLienHe, ', ',
                    ngaySinh, dienThoai, tenDoiTuong);
                noiDungChiTiet = Remove_LastComma(noiDungChiTiet);
                noiDungChiTiet = noiDungChiTiet.concat('<br /> Nhân viên thực hiện: ', self.UserLogin());
            }
            else {
                // import, export
                noiDung = txtFirst.concat('danh sách ', tenChucNangLowercase);
                noiDungChiTiet = noiDung.concat('<br /> Nhân viên thực hiện: ', self.UserLogin())
            }
        }
        // insert HT_NhatKySuDung
        var objNhatKy = {
            ID_NhanVien: $('.idnhanvien').text(),
            ID_DonVi: $('#hd_IDdDonVi').val().trim(),
            ChucNang: tenChucNang,
            LoaiNhatKy: loaiNhatKy,
            NoiDung: noiDung,
            NoiDungChiTiet: noiDungChiTiet,
        };
        var myDataNK = {};
        myDataNK.objDiary = objNhatKy;
        $.ajax({
            url: '/api/DanhMuc/SaveDiary/' + "post_NhatKySuDung",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: myDataNK,
            success: function (x) {
            },
        });
    }
    function CheckInput(obj) {
        var sReturn = '';
        var id = obj.ID();
        var maLienHe = obj.MaLienHe();
        var tenLienHe = obj.TenLienHe();
        var email = obj.Email();
        var phone = obj.SoDienThoai();
        var idTinhThanh = obj.ID_TinhThanh();
        var idQuanHuyen = obj.ID_QuanHuyen();
        var idDoiTuong = obj.ID_DoiTuong();
        if (tenLienHe === null || tenLienHe === "" || tenLienHe === undefined) {
            sReturn = 'Vui lòng nhập tên liên hệ  <br />';
        }
        // insert
        var lst = self.AllUserContact();
        if (id === undefined) {
            for (var i = 0; i < lst.length; i++) {
                if (maLienHe !== undefined && maLienHe !== '' && lst[i].MaLienHe.toLowerCase() === maLienHe.trim().toLowerCase()) {
                    sReturn += 'Mã người liên hệ tồn tại <br />';
                    break;
                }
                if (lst[i].SoDienThoai !== null && phone !== undefined
                    && lst[i].SoDienThoai !== '' && phone !== ''
                    && lst[i].SoDienThoai.trim() === phone.trim()) {
                    sReturn += 'Số điện thoại đã tồn tại <br />';
                    break;
                }
                if (lst[i].Email !== null && email !== undefined
                    && lst[i].Email !== '' && email !== ''
                    && lst[i].Email === email.trim()) {
                    sReturn += 'Email đã tồn tại <br />';
                    break;
                }
            }
        }
        // update
        else {
            for (var i = 0; i < lst.length; i++) {
                if (id !== lst[i].ID) {
                    if (maLienHe !== undefined && maLienHe !== '' && lst[i].MaLienHe === maLienHe.trim()) {
                        sReturn += 'Mã người liên hệ đã tồn tại <br />';
                        break;
                    }
                    if (lst[i].SoDienThoai !== null && lst[i].SoDienThoai !== undefined
                        && phone !== null && lst[i].SoDienThoai !== '' && phone !== ''
                        && lst[i].SoDienThoai.trim() === phone.trim()) {
                        sReturn += 'Số điện thoại đã tồn tại <br />';
                        break;
                    }
                    if (lst[i].Email !== null && email !== undefined
                        && email !== null && lst[i].Email !== '' && email !== ''
                        && lst[i].Email === email.trim()) {
                        sReturn += 'Email đã tồn tại <br />';
                        break;
                    }
                }
            }
        }
        if (CheckChar_Special(maLienHe)) {
            sReturn += 'Mã người liên hệ không được chứa kí tự đặc biệt <br />';
        }
        // Not check ID_DoiTuong, because assign this Customer
        if (email !== '' && email !== undefined && email !== null) {
            var valReturn = ValidateEmail(email);
            if (valReturn === false) {
                sReturn += 'Email không hợp lệ <br />';
            }
        }
        if (idTinhThanh !== undefined && idTinhThanh !== null && idTinhThanh.indexOf('000') === -1) {
            var itemTT = $.grep(self.listProvince(), function (item) {
                return item.ID === idTinhThanh;
            });
            if (itemTT.length === 0) {
                sReturn += 'Tỉnh thành không có trong hệ thống <br />';
            }
        }
        else {
            // find province in list
            var txtTenTT = $(txtProvince_modal).val();
            if (txtTenTT !== '') {
                var itemTT = $.grep(self.listProvince(), function (x) {
                    return x.TenTinhThanh.trim().toLowerCase() === txtTenDT.trim().toLowerCase();
                });
                if (itemTT.length === 0) {
                    sReturn += 'Tỉnh thành không có trong hệ thống <br />';
                }
            }
        }
        if (idQuanHuyen !== undefined && idQuanHuyen !== null && idQuanHuyen.indexOf('000') === -1) {
            var itemQH = $.grep(self.listDistrict(), function (item) {
                return item.ID === idQuanHuyen;
            });
            if (itemQH.length === 0) {
                sReturn += 'Quận huyện không có trong hệ thống <br />';
            }
        }
        else {
            // find province in list
            var txtTenQH = $(txtDistrict_modal).val();
            if (txtTenQH !== '') {
                var itemQH = $.grep(self.listDistrict(), function (x) {
                    return x.TenQuanHuyen.trim().toLowerCase() === txtTenQH.trim().toLowerCase();
                });
                if (itemQH.length === 0) {
                    sReturn += 'Quận huyện không có trong hệ thống <br />';
                }
            }
        }
        return sReturn;
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
    self.InsertImage = function (idUser, codeUser) {
        for (var i = 0; i < self.FilesSelect().length; i++) {
            var formData = new FormData();
            formData.append("file", self.FilesSelect()[i].file);
            // %2f = /
            $.ajax({
                type: "POST",
                url: DMLienHeUri + "ImageUpload?id=" + idUser + '&pathFolder=ImgUserContact%2f' + codeUser, // ImgUserContact + MaLienHe
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                async: false,
                success: function (response) {
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.log('err');
                }
            });
        }
    }
    //function GetAll_District() {
    //    ajaxHelper(DMDoiTuongUri + "GetAllQuanHuyen", 'GET').done(function (x) {
    //        if (x.res === true) {
    //            self.listDistrict(x.data);
    //        }
    //    });
    //}
    //GetAll_District();
    // used to check same MaLienHe
    function GetAllUserContact() {
        var sWhere = ' DM_LienHe.TrangThai !=0 ';
        ajaxHelper(DMLienHeUri + 'GetAllUserContact_byWhere_FilterNhanVien?txtSearch=' + sWhere, 'GET').done(function (data) {
            if (data !== null) {
                self.AllUserContact(data);
            }
        });
    }
    GetAllUserContact();
    self.DeleteImg_UserContact = function (item, event) {
        if (item.ID !== undefined) {
            dialogConfirm('Thông báo xóa', 'Bạn có chắc chắn muốn xóa ảnh của liên hệ <b>' + self.MaLienHe() + '</b> không?', function () {
                $.ajax({
                    type: "DELETE",
                    url: DMLienHeUri + "DeleteDM_LienHe_Anh/" + item.ID,
                    dataType: 'json',
                    contentType: 'application/json',
                    success: function (result) {
                        ShowMessage_Success('Xóa ảnh liên hệ thành công');
                    },
                    error: function (error) {
                        $('#modalPopuplgDelete').modal('hide');
                        ShowMessage_Danger('Xóa ảnh liên hệ thất bại');
                    }
                });
                self.FilesSelect.remove(item);
                self.FileImgs.remove(item);
                if (self.FilesSelect().length == 0) {
                    self.HaveImage_Select(false);
                    self.AnhDaiDien([]);
                    $('#file').val('');
                }
            })
        } else {
            self.FilesSelect.remove(item);
            if (self.FilesSelect().length == 0) {
                self.HaveImage_Select(false);
                self.AnhDaiDien([]);
                $('#file').val('');
            }
        }
    }
};
