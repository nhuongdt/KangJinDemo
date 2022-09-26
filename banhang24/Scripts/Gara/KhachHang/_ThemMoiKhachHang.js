var vmThemMoiKhach = new Vue({
    el: '#ThemMoiKhachHang',
    components: {
        'nguonkhachs': cmpNguonKhach,
        'trangthais': cmpTrangThaiKhach,
        'nhomkhachs': cmpNhomKhach,
        'customers': cmpChoseCustomer,
        'nhanviens': ComponentChoseStaff,
        'ngaysinh': cmpNgaySinh
    },
    created: function () {
        var self = this;
        self.SubDomain = $('#subDomain').val();
        self.inforLogin.ID_DonVi = $('#txtDonVi').val();
        self.ImgHost = Open24FileManager.hostUrl;
        self.UrlDoiTuongAPI = '/api/DanhMuc/DM_DoiTuongAPI/';
        self.ToDay = new Date();
        if (commonStatisJs.CheckNull(self.SubDomain)) {
            self.SubDomain = VHeader.SubDomain;
        }
        console.log('vmThemMoiKH ')
        $.getJSON(self.UrlDoiTuongAPI + 'GetListTinhThanh').done(function (x) {
            if (x.res === true) {
                let data = x.data;
                var province = data.map(function (p) {
                    return {
                        ID: p.ID,
                        val2: p.TenTinhThanh
                    }
                });
                self.listData.TinhThanhs = province;
                self.listData.ListTinhThanhSearch = province;
                vmThemMoiNhomKhach.listData.TinhThanhs = province;
                vmThemMoiNhomKhach.listData.ListTinhThanhSearch = province;
            }
        })

        if (commonStatisJs.CheckNull(self.inforLogin.ID_DonVi)) {
            self.inforLogin = {
                ID_NhanVien: VHeader.IdNhanVien,
                ID_User: VHeader.IdNguoiDung,
                UserLogin: VHeader.UserLogin,
                ID_DonVi: VHeader.IdDonVi,
            };
        }
    },
    computed: {
        sLoaiDoiTuong: function () {
            let self = this;
            let txt = '';
            switch (self.newCustomer.LoaiDoiTuong) {
                case 1:
                    txt = 'khách hàng';
                    break;
                case 4:
                    txt = 'người giới thiệu';
                    break;
            }
            return txt;
        },
        showDiv: function () {
            let self = this;
            let show = true;
            switch (self.newCustomer.LoaiDoiTuong) {
                case 4:
                    show = false;
                    break;
            }
            return show;
        },
    },
    data: {
        saveOK: false,
        isNew: true,
        isLoading: false,
        QuanLyKhachHangTheoDonVi: false,
        error: '',
        role: {
            // role nhom + nguon + trangthai = role customer
            KhachHang: {},
            NhomKhach: { ThemMoi: false },
        },
        customerDoing: {},
        customerOld: {},
        NhomKhachChosed: [],
        inforLogin: {
            ID_NhanVien: null,
            ID_User: null,
            UserLogin: null,
            ID_DonVi: null,
        },
        listData: {
            Quyen: [],
            NhomKhachs: [],
            NguonKhachs: [],
            TrangThaiKhachs: [],
            TinhThanhs: [],
            QuanHuyens: [],
            NhanViens: [],
            ListTinhThanhSearch: [],
            ListQuanHuyenSearch: [],
        },
        newCustomer: {
            ID: null,
            LoaiDoiTuong: 1,
            MaDoiTuong: '',
            TenDoiTuong: '',
            LaCaNhan: true,
            GioiTinhNam: true,
            NgaySinh_NgayTLap: null,
            DinhDang_NgaySinh: 'dd/MM/yyyy',
            Email: '',
            DiaChi: '',
            DienThoai: '',
            ID_TinhThanh: null,
            ID_QuanHuyen: null,
            ID_NguonKhach: null,
            ID_TrangThai: null,
            ID_NguoiGioiThieu: null,
            ID_NhanVienPhuTrach: null,
            IDNhomKhachs: '',
            ListIDNhomKhach: [],
            TheoDoi: false,// false: dang hoatdong, true: daxoa
            ChiaSe: false,
            ID_Index: 0,
            MaSoThue: '',
            TaiKhoanNganHang: '',

            TenNguoiGioiThieu: '',
            TenTrangThai: '',
            TenNhomKhachs: '',
            TenNguonKhach: '',
            TenNhanVienPhuTrach: '',
            TenTinhThanh: '',
            TenQuanHuyen: '',
        },
        FileSelects: [],
        HaveImage: false,
        ImgHost: ""
    },
    methods: {
        showModalAdd: function (loaiDT = 1) {
            var self = this;
            self.isNew = true;
            self.saveOK = false;
            self.isLoading = false;
            self.HaveImage = false;
            self.FileSelects = [];
            self.NhomKhachChosed = [];

            console.log('41')
            self.newCustomer = {
                ID: null,
                LoaiDoiTuong: loaiDT,
                MaDoiTuong: '',
                TenDoiTuong: '',
                LaCaNhan: true,
                GioiTinhNam: true,
                NgaySinh_NgayTLap: null,
                DinhDang_NgaySinh: 'dd/MM/yyyy',
                Email: '',
                DiaChi: '',
                DienThoai: '',
                ID_TinhThanh: null,
                ID_QuanHuyen: null,
                ID_NguonKhach: null,
                ID_TrangThai: null,
                ID_NguoiGioiThieu: null,
                ID_NhanVienPhuTrach: self.inforLogin.ID_NhanVien,
                IDNhomKhachs: '',
                ListIDNhomKhach: [],
                TheoDoi: false,
                ChiaSe: false,
                ID_Index: 0,
                MaSoThue: '',
                TongBanTruTraHang: 0,
                TongBan: 0,
                TongMua: 0,
                TongTichDiem: 0,
                SoLanMuaHang: 0,
                NoHienTai: 0,
                NgayGiaoDichGanNhat: null,
                NgayTao: new Date(),
                NguoiTao: self.inforLogin.UserLogin,
                ID_DonVi: self.inforLogin.ID_DonVi,
                GhiChu: '',
                TaiKhoanNganHang: '',

                TenNguoiGioiThieu: '',
                TenTrangThai: '',
                TenNhomKhachs: '',
                TenNguonKhach: '',
                TenNhanVienPhuTrach: '',
                TenTinhThanh: '',
                TenQuanHuyen: '',
            };
            var nv = $.grep(self.listData.NhanViens, function (x) {
                return x.ID === self.inforLogin.ID_NhanVien;
            });
            if (nv.length > 0) {
                self.newCustomer.TenNhanVienPhuTrach = nv[0].TenNhanVien;
            }
            if (self.QuanLyKhachHangTheoDonVi) {
                // getlistnhom by chinhanh
                var arrNhomWithDV = $.extend([], self.listData.NhomKhachs);
                arrNhomWithDV.map(function (x) {
                    x["IDDonVis"] = x.NhomDT_DonVi.map(function (y) {
                        return y.ID
                    })
                });
                var arrNhomDV = $.grep(arrNhomWithDV, function (x) {
                    return x.IDDonVis.length > 0 && $.inArray(self.inforLogin.ID_DonVi, x.IDDonVis) > -1;
                });
                if (arrNhomDV.length === 1) {
                    self.newCustomer.ListIDNhomKhach = [arrNhomDV[0].ID];
                    self.newCustomer.TenNhomKhachs = arrNhomDV[0].TenNhomDoiTuong;
                    self.NhomKhachChosed = arrNhomDV;
                }
            }
            $('#ThemMoiKhachHang').modal('show');
        },

        GetInforKhachHangFromDB_ByID: function (idCus, isShow = false) {// used to update cus at phieutiepnhan
            let self = this;
            let date = moment(new Date()).format('YYYY-MM-DD HH:mm');
            if (!commonStatisJs.CheckNull(idCus)) {
                ajaxHelper('/api/DanhMuc/DM_DoituongAPI/' + "GetInforKhachHang_ByID?idDoiTuong=" + idCus
                    + '&idChiNhanh=' + self.inforLogin.ID_DonVi
                    + '&timeStart=' + date + '&timeEnd=' + date + '&wasChotSo=false', 'GET').done(function (data) {
                        if (data !== null) {
                            if (isShow) {
                                self.showModalUpdate(data[0]);
                            }
                        }
                    });
            }
        },

        showModalUpdate: async function (item) {
            var self = this;
            self.isNew = false;
            self.saveOK = false;

            self.listData.ListTinhThanhSearch = self.listData.TinhThanhs;
            if (!commonStatisJs.CheckNull(item.ID_TinhThanh)) {
                let province = $.grep(self.listData.TinhThanhs, function (x) {
                    return x.ID === item.ID_TinhThanh;
                });
                if (province.length > 0) {
                    tentinh = province[0].val2;
                }
                await self.LoadQuanHuyen(item.ID_TinhThanh);
            }

            if (commonStatisJs.CheckNull(item.TenNguoiGioiThieu)) {
                if (!commonStatisJs.CheckNull(item.NguoiGioiThieu)) {
                    item.TenNguoiGioiThieu = item.NguoiGioiThieu;
                }
                else {
                    item.TenNguoiGioiThieu = '';
                }
            }

            item.ID_DonVi = self.inforLogin.ID_DonVi;
            self.newCustomer = item;
            self.customerOld = $.extend({}, item);
            self.customerDoing = item;
            self.GetListImg_ofKhachHang();  // load img

            var arrNhom = $.unique(item.ID_NhomDoiTuong.split(','));
            self.newCustomer.ListIDNhomKhach = arrNhom.map(function (x) { return x.trim() });

            if (!commonStatisJs.CheckNull(item.NgaySinh_NgayTLap)) {
                self.newCustomer.NgaySinh_NgayTLap = moment(item.NgaySinh_NgayTLap).format('DD/MM/YYYY');
            }

            // getnhom by id
            self.NhomKhachChosed = [];
            for (let i = 0; i < self.newCustomer.ListIDNhomKhach.length; i++) {
                let idNhom = self.newCustomer.ListIDNhomKhach[i].trim().toLowerCase();
                let nhom = $.grep(self.listData.NhomKhachs, function (x) {
                    return x.ID === idNhom;
                });
                if (nhom.length > 0) {
                    self.NhomKhachChosed.push(nhom[0]);
                }
            }

            // get nguonkhach, trangthai, nvphutrach
            var tennguon = '', trangthai = '', nvphutrach = '', tentinh = '';
            if (!commonStatisJs.CheckNull(item.ID_NguonKhach)) {
                let nguon = $.grep(self.listData.NguonKhachs, function (x) {
                    return x.ID === item.ID_NguonKhach;
                });
                if (nguon.length > 0) {
                    tennguon = nguon[0].TenNguonKhach;
                }
            }
            if (!commonStatisJs.CheckNull(item.ID_TrangThai)) {
                let ttkhach = $.grep(self.listData.TrangThaiKhachs, function (x) {
                    return x.ID === item.ID_TrangThai;
                });
                if (ttkhach.length > 0) {
                    trangthai = ttkhach[0].Name;
                }
            }

            let nvPT = [];
            if (!commonStatisJs.CheckNull(item.ID_NhanVienPhuTrach)) {
                nvPT = $.grep(self.listData.NhanViens, function (x) {
                    return x.ID === item.ID_NhanVienPhuTrach;
                });
                if (nvPT.length > 0) {
                    nvphutrach = nvPT[0].TenNhanVien;
                }
            }
            if (nvPT.length === 0) {
                nvPT = $.grep(self.listData.NhanViens, function (x) {
                    return x.ID === self.inforLogin.ID_NhanVien;
                });
            }
            if (nvPT.length > 0) {
                nvphutrach = nvPT[0].TenNhanVien;
                self.newCustomer.TenNhanVienPhuTrach = nvPT[0].TenNhanVien;
            }

            self.newCustomer.TenNguonKhach = tennguon;
            self.newCustomer.TenTrangThai = trangthai;
            self.newCustomer.TenNhanVienPhuTrach = nvphutrach;
            self.newCustomer.TenTinhThanh = tentinh;
            $('#ThemMoiKhachHang').modal('show');
        },
        ValidateEmail: function () {
            var self = this;
            if (commonStatisJs.CheckNull(self.newCustomer.Email)) {
                var re = /^(([^<>()[\]\\.,;:\s@#ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴ\"]+(\.[^<>()[\]\\.,;:\s@#ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴ\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/i;
                var valReturn = re.test(self.newCustomer.Email.trim());
                if (valReturn === false) {
                    self.error = "Email không hợp lệ";
                    return;
                }
                self.error = '';
            }
        },
        SearchTinhThanh: function (value) {
            let self = this;
            var txt = commonStatisJs.convertVieToEng(value.searchkey);
            if (txt === '') {
                self.newCustomer.ID_TinhThanh = '';
                self.newCustomer.ID_QuanHuyen = '';
                self.listData.ListQuanHuyenSearch = [];
            }

            self.listData.ListTinhThanhSearch = self.listData.TinhThanhs
                .filter(p => commonStatisJs.convertVieToEng(p.val1).match(txt)
                    || commonStatisJs.convertVieToEng(p.val2).match(txt)
                    || commonStatisJs.convertVieToEng(p.val3).match(txt));

        },
        SearchQuanHuyen: function (value) {
            let self = this;
            var txt = commonStatisJs.convertVieToEng(value.searchkey);
            if (txt === '') {
                self.newCustomer.ID_QuanHuyen = '';
            }
            self.listData.ListQuanHuyenSearch = self.listData.QuanHuyens
                .filter(p => commonStatisJs.convertVieToEng(p.val1).match(txt)
                    || commonStatisJs.convertVieToEng(p.val2).match(txt)
                    || commonStatisJs.convertVieToEng(p.val3).match(txt));

        },
        ChoseNguoiGioiThieu: function (item) {
            var self = this;
            self.newCustomer.ID_NguoiGioiThieu = item.ID;
            self.newCustomer.TenNguoiGioiThieu = item.TenDoiTuong;
        },
        ChoseNVPhuTrach: function (item) {
            var self = this;
            self.newCustomer.ID_NhanVienPhuTrach = item.ID;
            self.newCustomer.TenNhanVienPhuTrach = item.TenNhanVien;
        },
        ChoseNguonKhach: function (item) {
            var self = this;
            if (item === null) {
                self.newCustomer.ID_NguonKhach = null;
                self.newCustomer.TenNguonKhach = '-- Chọn nguồn --';
            }
            else {
                self.newCustomer.ID_NguonKhach = item.ID;
                self.newCustomer.TenNguonKhach = item.TenNguonKhach;
            }
        },
        ChoseNhomKhach: function (item, lst) {
            var self = this;
            self.newCustomer.ListIDNhomKhach = lst.map(function (x) { return x.ID });
            self.NhomKhachChosed = lst;
        },
        RemoveNhomKhach: function (lst) {
            var self = this;
            self.newCustomer.ListIDNhomKhach = lst.map(function (x) { return x.ID });
            self.NhomKhachChosed = lst;
        },
        ChoseTrangThai: function (item) {
            var self = this;
            if (item === null) {
                self.newCustomer.ID_TrangThai = null;
                self.newCustomer.TenTrangThai = '-- Chọn trạng thái --';
            }
            else {
                self.newCustomer.ID_TrangThai = item.ID;
                self.newCustomer.TenTrangThai = item.Name;
            }
        },
        ChoseTinhThanh: function (item) {
            var self = this;
            var $this = $(event.currentTarget);
            var tenTinhThanh = $this.find('.seach-hh').text();
            self.newCustomer.ID_TinhThanh = item.id;
            self.newCustomer.TenTinhThanh = tenTinhThanh;
            self.LoadQuanHuyen(item.id);
        },
        ChoseQuanHuyen: function (item) {
            var self = this;
            var $this = $(event.currentTarget);
            var tenquanhuyen = $this.find('.seach-hh').text();
            self.newCustomer.ID_QuanHuyen = item.id;
            self.newCustomer.TenQuanHuyen = tenquanhuyen;
        },
        LoadQuanHuyen: async function (idtinhthanh) {
            let self = this;
            await $.getJSON('/api/DanhMuc/BaseApi/' + "GetQuanHuyen?tinhthanhID=" + idtinhthanh).done(function (x) {
                let data = x.map(p => ({
                    ID: p.Key,
                    val2: p.Value
                }));
                self.listData.QuanHuyens = data;
                self.listData.ListQuanHuyenSearch = data;
            });
        },
        GetListImg_ofKhachHang: function () {
            var self = this;
            var idCus = self.customerDoing.ID;
            ajaxHelper(self.UrlDoiTuongAPI + 'GetImages_DoiTuong/' + idCus, 'GET').done(function (data) {
                if (data !== null && data.length > 0) {
                    self.HaveImage = true;
                    self.FileSelects = data.map(p => {
                        p.URLAnh = self.ImgHost + p.URLAnh;
                        return p;
                    });
                }
                else {
                    self.HaveImage = false;
                    self.FileSelects = [];
                }
            })
        },
        fileSelect: function (elm) {
            var self = this;
            var files = elm.target.files;
            var countErrType = 0;
            var countErrSize = 0;
            var errFileSame = '';
            var err = '';

            // Check Type file & Size
            for (let i = 0; i < files.length; i++) {

                if (!files[i].type.match('image.*')) {
                    countErrType += 1;
                }

                var size = parseFloat(files[i].size / 1024).toFixed(2);
                if (size > 2048) {
                    countErrSize += 1;
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

            for (let i = 0; i < files.length; i++) {
                let f = files[i];

                // Only process image files.
                if (!f.type.match('image.*')) {
                    continue;
                }
                let size = parseFloat(f.size / 1024).toFixed(2);

                if (size <= 2048) {
                    var reader = new FileReader();
                    // Closure to capture the file information.
                    reader.onload = (function (theFile) {
                        return function (e) {
                            self.FileSelects.push({
                                file: theFile,
                                URLAnh: e.target.result,
                            });
                        };
                    })(f);

                    // Read in the image file as a data URL.
                    reader.readAsDataURL(f);
                    self.HaveImage = true;
                }
            }
        },
        InsertImage: function () {
            var self = this;

            let formData = new FormData();
            for (let i = 0; i < self.FileSelects.length; i++) {
                formData.append("files", self.FileSelects[i].file);
            }
            let myData = {};
            myData.Subdomain = self.SubDomain;
            myData.Function = "1";
            myData.Id = self.customerDoing.ID;
            myData.files = formData;
            var result = Open24FileManager.UploadImage(myData);
            if (result.length > 0) {
                $.ajax({
                    url: self.UrlDoiTuongAPI + "UpdateAnhDoiTuong?id=" + self.customerDoing.ID,
                    type: "POST",
                    data: JSON.stringify(result),
                    contentType: "application/json",
                    dataType: "JSON",
                    success: function (data, textStatus, jqXHR) {
                    },
                    error: function (jqXHR, textStatus, errorThrown) {

                    }
                });
            }
        },
        DeleteImg: function (item, index) {
            var self = this;
            if (commonStatisJs.CheckNull(item.ID)) {
                for (let i = 0; i < self.FileSelects.length; i++) {
                    if (i === index) {
                        self.FileSelects.splice(i, 1);
                        break;
                    }
                }
            }
            else {
                commonStatisJs.ConfirmDialog_OKCancel('Xác nhận xóa', 'Bạn có chắc chắn muốn xóa ảnh này không?', function () {
                    ajaxHelper(self.UrlDoiTuongAPI + "DeleteDM_DoiTuong_Anh/" + item.ID, 'DELETE').done(function () {
                        ShowMessage_Success("Xóa ảnh khách hàng thành công");
                        Open24FileManager.RemoveFiles([item.URLAnh.replace(self.ImgHost, "")]);
                        for (let i = 0; i < self.FileSelects.length; i++) {
                            if (self.FileSelects[i].ID === item.ID) {
                                self.FileSelects.splice(i, 1);
                                break;
                            }
                        }

                        if (self.FileSelects.length === 0) {
                            self.HaveImage = false;
                            $('#file').val('');
                        }
                    }).always(function () {
                        $('#modalPopuplgDelete').modal('hide');
                    })
                });
            }
        },

        CheckSave: function () {
            let self = this;
            let cus = self.newCustomer;
            if (commonStatisJs.CheckNull(cus.TenDoiTuong)) {
                ShowMessage_Danger('Vui lòng nhập tên ' + self.sLoaiDoiTuong);
                return false;
            }
            if (commonStatisJs.CheckNull(cus.DienThoai)) {
                ShowMessage_Danger('Vui lòng nhập số điện thoại');
                return false;
            }
            return true;
        },

        SaveCustomer: function () {
            var self = this;
            var id = self.newCustomer.ID;
            var ngaysinh = self.newCustomer.NgaySinh_NgayTLap;
            var dinhdangNS = self.newCustomer.DinhDang_NgaySinh;
            var yearNow = (new Date()).getFullYear();
            var idQuanHuyen = self.newCustomer.ID_QuanHuyen;
            idQuanHuyen = idQuanHuyen === '' ? null : idQuanHuyen;
            var idTinhThanh = self.newCustomer.ID_TinhThanh;
            idTinhThanh = idTinhThanh === '' ? null : idTinhThanh;

            let check = self.CheckSave();
            if (!check) {
                return;
            }

            var sNgayDinh = '';

            if (id !== null && id !== undefined) {
                if (ngaysinh !== null && ngaysinh !== undefined) {
                    if (ngaysinh.length === 10) {
                        switch (dinhdangNS) {
                            case 'dd/MM':
                                if (ngaysinh.substr(6, 4) !== yearNow) {
                                    dinhdangNS = 'dd/MM/yyyy';
                                }
                                else {
                                    ngaysinh = ngaysinh.substr(0, 5);
                                }
                                break;
                            case 'MM/yyyy':
                                if (ngaysinh.substr(0, 2) !== '01') {
                                    dinhdangNS = 'dd/MM/yyyy';
                                }
                                else {
                                    ngaysinh = ngaysinh.substr(3, 7);
                                }
                                break;
                            case 'yyyy':
                                if (ngaysinh.substr(0, 5) !== '01/01') {
                                    dinhdangNS = 'dd/MM/yyyy';
                                }
                                else {
                                    ngaysinh = ngaysinh.substr(6, 4);
                                }
                                break;
                            default:
                                ngaysinh = ngaysinh;
                                dinhdangNS = 'dd/MM/yyyy';
                        }
                    }
                }
            }

            if (!commonStatisJs.CheckNull(ngaysinh)) {
                switch (dinhdangNS) {
                    case 'dd/MM':
                        sNgayDinh = 'Ngày sinh: '.concat(ngaysinh);
                        ngaysinh = ngaysinh + '/' + yearNow;
                        break;
                    case 'MM/yyyy':
                        sNgayDinh = 'Ngày sinh: '.concat(ngaysinh);
                        ngaysinh = '01/' + ngaysinh;
                        break;
                    case 'yyyy':
                        sNgayDinh = 'Năm sinh: '.concat(ngaysinh);
                        ngaysinh = '01/01/' + ngaysinh;
                        break;
                    case null:
                        ngaysinh = null;
                    default:
                        sNgayDinh = 'Ngày sinh: '.concat(ngaysinh);
                        dinhdangNS = 'dd/MM/yyyy';
                        break;
                }
                ngaysinh = moment(ngaysinh, 'DD/MM/YYYY').format('YYYY-MM-DD');
            }
            else {
                ngaysinh = null;
                dinhdangNS = 'dd/MM/yyyy';
            }

            var tennhoms = ''
            if (self.NhomKhachChosed.length === 0) {
                tennhoms = 'Nhóm mặc định';
            }
            else {
                tennhoms = self.NhomKhachChosed.map(function (x) { return x.TenNhomDoiTuong }).toString();
            }

            var DM_DoiTuong = $.extend({}, self.newCustomer);
            if (commonStatisJs.CheckNull(DM_DoiTuong.MaDoiTuong)) {
                DM_DoiTuong.MaDoiTuong = DM_DoiTuong.MaDoiTuong.trim();
            }
            DM_DoiTuong.ID_NhomDoiTuong = null;// not use properties
            DM_DoiTuong.TenDoiTuong = DM_DoiTuong.TenDoiTuong.trim();
            DM_DoiTuong.TenDoiTuong_ChuCaiDau = GetChartStart(self.newCustomer.TenDoiTuong);
            DM_DoiTuong.TenDoiTuong_KhongDau = locdau(self.newCustomer.TenDoiTuong);
            DM_DoiTuong.DinhDang_NgaySinh = dinhdangNS;
            DM_DoiTuong.ID_DonVi = self.inforLogin.ID_DonVi;
            DM_DoiTuong.NguoiTao = self.inforLogin.UserLogin;
            DM_DoiTuong.NgaySinh_NgayTLap = ngaysinh;
            DM_DoiTuong.ID_QuanHuyen = idQuanHuyen;
            DM_DoiTuong.ID_TinhThanh = idTinhThanh;
            DM_DoiTuong.ID = id === null ? '00000000-0000-0000-0000-000000000000' : id;
            DM_DoiTuong.TenNhomKhachs = tennhoms;
            DM_DoiTuong.IDNhomKhachs = DM_DoiTuong.ListIDNhomKhach.toString();// used to assign at DS KhachHang

            if (commonStatisJs.CheckNull(DM_DoiTuong.TheoDoi)) {
                DM_DoiTuong.TheoDoi = false;
            }

            self.isLoading = true;

            sNgayDinh = sNgayDinh !== '' ? ' <br /> Ngày sinh: '.concat(ngaysinh) : '';

            // insert DoiTuong
            if (self.isNew === true) {
                self.AddCustomer(DM_DoiTuong, sNgayDinh);
            }
            // update DoiTuong
            else {
                DM_DoiTuong.NguoiSua = self.inforLogin.UserLogin;
                var myData = {
                    id: id,
                    objDoiTuong: DM_DoiTuong,
                };
                self.UpdateCustomer(myData, sNgayDinh);
            }
        },

        AddCustomer: function (DM_DoiTuong, sNgayDinh) {
            var self = this;
            ajaxHelper(self.UrlDoiTuongAPI + 'PostDM_DoiTuong', 'POST', DM_DoiTuong).done(function (x) {
                console.log('xx ', x);
                if (x.res === true) {
                    self.saveOK = true;
                    var item = x.data;
                    ShowMessage_Success("Thêm mới " + self.sLoaiDoiTuong + " thành công");

                    DM_DoiTuong.MaDoiTuong = item.MaDoiTuong;
                    DM_DoiTuong.ID = item.ID;
                    self.customerDoing = DM_DoiTuong;
                    self.newCustomer.MaDoiTuong = item.MaDoiTuong;
                    self.newCustomer.ID = item.ID;

                    // add properties ID_DoiTuong,ID to list
                    var lstNhom = $.map(DM_DoiTuong.ListIDNhomKhach, function (xx) {
                        return {
                            ID: xx,
                            ID_DoiTuong: item.ID,
                        }
                    });
                    self.UpdateNhomKhachHang(lstNhom);
                    self.InsertImage();

                    // remind birthday KH if NgaySinh_NgayTLap is today
                    var mmdd = moment(self.ToDay).format('MM-DD');
                    var ngaysinhNew = DM_DoiTuong.NgaySinh_NgayTLap;
                    if (ngaysinhNew !== null) {
                        ngaysinhNew = moment(ngaysinhNew, 'YYYY-MM-DD').format('MM-DD');
                        if (ngaysinhNew === mmdd) {
                            self.Insert_HT_ThongBao(self.newCustomer);
                        }
                    }
                    self.NangNhomKhachHang(item.ID);

                    var diary = {
                        ID_DonVi: self.inforLogin.ID_DonVi,
                        ID_NhanVien: self.inforLogin.ID_NhanVien,
                        LoaiNhatKy: 1,
                        ChucNang: commonStatisJs.FirstChar_UpperCase(self.sLoaiDoiTuong),
                        NoiDung: 'Thêm mới '.concat(self.sLoaiDoiTuong, ' ', DM_DoiTuong.TenDoiTuong, ' (', DM_DoiTuong.MaDoiTuong, ')'),
                        NoiDungChiTiet: 'Thêm mới '.concat(self.sLoaiDoiTuong, ' ', DM_DoiTuong.TenDoiTuong, ' (', DM_DoiTuong.MaDoiTuong, ') <br /> Điện thoại: ',
                            DM_DoiTuong.DienThoai ? DM_DoiTuong.DienThoai : '', sNgayDinh,
                            ' <br /> Địa chỉ: ', DM_DoiTuong.DiaChi,
                            ' <br /> Nhóm khách: ', DM_DoiTuong.TenNhomKhachs)
                    }
                    Insert_NhatKyThaoTac_1Param(diary);
                    $("#ThemMoiKhachHang").modal("hide");
                }
                else {
                    self.saveOK = false;
                    ShowMessage_Danger(x.mes);
                }
            }).always(function () {
                self.isLoading = false;
            }).fail(function (x) {
                if (x.responseText.indexOf('NgaySinh_NgayTLap') > -1) {
                    ShowMessage_Danger('Sai ngày sinh hoặc chưa đúng định dạng');
                }
            })
        },
        UpdateCustomer: function (myData, sNgayDinh) {
            var self = this;
            ajaxHelper(self.UrlDoiTuongAPI + 'PutDM_DoiTuong', 'PUT', myData).done(function (x) {
                if (x.res === true) {
                    self.saveOK = true;
                    var item = x.data;
                    ShowMessage_Success("Cập nhật " + self.sLoaiDoiTuong + " thành công");

                    var obj = myData.objDoiTuong;
                    obj.MaDoiTuong = item.MaDoiTuong;
                    self.customerDoing = obj;

                    self.newCustomer.MaDoiTuong = item.MaDoiTuong;

                    var lstNhom = $.map(obj.ListIDNhomKhach, function (xx) {
                        return {
                            ID: xx,
                            ID_DoiTuong: obj.ID,
                        }
                    });
                    self.UpdateNhomKhachHang(lstNhom);
                    self.InsertImage();

                    // remind birthday KH if NgaySinh_NgayTLap is today
                    var mmdd = moment(self.ToDay).format('MM-DD');
                    var ngaysinhNew = obj.NgaySinh_NgayTLap;
                    if (!commonStatisJs.CheckNull(ngaysinhNew) && obj.DinhDang_NgaySinh.indexOf('dd') > -1) {
                        ngaysinhNew = moment(ngaysinhNew, 'YYYY-MM-DD').format('MM-DD');
                        if (ngaysinhNew === mmdd) {
                            self.Insert_HT_ThongBao(self.newCustomer);
                        }
                    }
                    self.NangNhomKhachHang(item.ID);

                    var diary = {
                        ID_DonVi: self.inforLogin.ID_DonVi,
                        ID_NhanVien: self.inforLogin.ID_NhanVien,
                        LoaiNhatKy: 2,
                        ChucNang: commonStatisJs.FirstChar_UpperCase(self.sLoaiDoiTuong),
                        NoiDung: 'Cập nhật '.concat(self.sLoaiDoiTuong, ' ', obj.TenDoiTuong, ' (', obj.MaDoiTuong, ')'),
                        NoiDungChiTiet: 'Cập nhật '.concat(self.sLoaiDoiTuong, ' ', obj.TenDoiTuong, ' (', obj.MaDoiTuong, ') <br /> Điện thoại: ',
                            obj.DienThoai, sNgayDinh,
                            ' <br /> Địa chỉ: ', obj.DiaChi,
                            ' <br /> Nhóm khách: ', obj.TenNhomKhachs,
                            ' <br /> <b> Thông tin cũ: </b>',
                            ' <br /> - Tên đối tượng: ', self.customerOld.TenDoiTuong, ' ( ', self.customerOld.MaDoiTuong, ') ',
                            '<br /> - Điện thoại: ', self.customerOld.DienThoai,
                            '<br /> - Ngày sinh: ', self.customerOld.NgaySinh_NgayTLap,
                            ' <br /> - Nhóm cũ: ', self.customerOld.TenNhomDT)
                    };
                    Insert_NhatKyThaoTac_1Param(diary);
                    $("#ThemMoiKhachHang").modal("hide");
                }
                else {
                    self.saveOK = false;
                    if (x.mes.indexOf('NgaySinh_NgayTLap') > -1) {
                        ShowMessage_Danger('Sai ngày sinh hoặc chưa đúng định dạng');
                    }
                    else {
                        ShowMessage_Danger(x.mes);
                    }
                }
            }).always(function () {
                self.isLoading = false;
            }).fail(function (x) {
                if (x.responseText.indexOf('NgaySinh_NgayTLap') > -1) {
                    ShowMessage_Danger('Sai ngày sinh hoặc chưa đúng định dạng');
                }
            })
        },
        UpdateNhomKhachHang: function (lstNhom, isChuyenNhom = false) {
            var self = this;
            var lst = [];
            if (self.newCustomer.LoaiDoiTuong === 1) {
                for (let i = 0; i < lstNhom.length; i++) {
                    let itFor = lstNhom[i];
                    if (!commonStatisJs.CheckNull(itFor.ID) && itFor.ID.trim() !== '00000000-0000-0000-0000-000000000000') {
                        let obj = {
                            ID_DoiTuong: itFor.ID_DoiTuong,
                            ID_NhomDoiTuong: itFor.ID,
                        }
                        lst.push(obj);
                    }
                }
                if (lst.length > 0) {
                    var myData = {
                        lstDM_DoiTuong_Nhom: lst
                    };

                    ajaxHelper(self.UrlDoiTuongAPI + 'PutDM_DoiTuong_Nhom', 'POST', myData).done(function (x) {
                        if (isChuyenNhom) {
                            var diary = {
                                ID_DonVi: self.inforLogin.ID_DonVi,
                                ID_NhanVien: self.inforLogin.ID_NhanVien,
                                LoaiNhatKy: 2,
                                ChucNang: 'Khách hàng',
                                NoiDung: 'Chuyển '.concat(lst.length, ' khách hàng đến nhóm mới'),
                                NoiDungChiTiet: 'Chuyển '.concat(lst.length, ' khách hàng đến nhóm mới'),
                            }
                            Insert_NhatKyThaoTac_1Param(diary);
                        }

                    }).fail(function (x) {
                    })
                }
            }
        },
        NangNhomKhachHang: function (id) {
            var self = this;
            if (id !== null && id !== const_GuidEmpty && self.newCustomer.LoaiDoiTuong === 1) {
                ajaxHelper(self.UrlDoiTuongAPI + 'NangNhomKhachhang_byIDDoituong?idDoituong=' + id
                    + '&idChiNhanh=' + self.inforLogin.ID_DonVi, 'GET').done(function (x) {
                        if (x.res) {
                            // todo get rows success in sql
                            //ShowMessage_Success("Đã tự động cập nhật nhóm cho khách hàng " + madoituong);
                        }
                    })
            }
        },
        Insert_HT_ThongBao: function (DM_DoiTuong) {
            var self = this;
            // remind birthday customer
            var objAdd =
            {
                ID_DonVi: self.inforLogin.ID_DonVi,
                LoaiThongBao: 3,
                NoiDungThongBao: "<p onclick=\"loaddadoc('" + 'key' + "')\"> Khách hàng <a onclick=\"loadthongbao('3'," +
                    "'" + DM_DoiTuong.MaDoiTuong + "', '" + 'key' + "')\">"
                    + " <span class=\"blue\">" + DM_DoiTuong.TenDoiTuong + " </span>" + "</a> có sinh nhật hôm nay</p>",
                NguoiDungDaDoc: '',
            };
            ajaxHelper('/api/DanhMuc/HT_API/' + 'Post_HT_ThongBao', 'POST', objAdd).done(function (x) {

            })
        },

        ShowModalAddNhomKhach: function () {
            var self = this;
            vmThemMoiNhomKhach.listData.TinhThanhs = self.listData.TinhThanhs;
            vmThemMoiNhomKhach.listData.ListTinhThanhSearch = self.listData.TinhThanhs;
            vmThemMoiNhomKhach.inforLogin = self.inforLogin;
            vmThemMoiNhomKhach.showModalAdd();
        },
        Change_DinhDangNgaySinh: function (item) {
            var self = this;
            self.newCustomer.DinhDang_NgaySinh = item.Value;
            self.newCustomer.NgaySinh_NgayTLap = '';

            if (item.Value === 'dd/MM/yyyy') {
                $("#txtNgaySinh").datepicker({
                    showOn: 'focus',
                    altFormat: "dd/mm/yy",
                    showOn: "button",
                    buttonImageOnly: true,
                    dateFormat: "dd/mm/yy",
                    buttonImage: '/Content/images/icon/ngaysinh.png',
                })
            }
        },

        showModalNguonKhach: function () {
            var self = this;
            vmNguonKhach.inforLogin = self.inforLogin;
            vmNguonKhach.showModalAdd();
        },
        showModalTrangThaiKhach: function () {
            var self = this;
            vmTrangThaiKhach.inforLogin = self.inforLogin;
            vmTrangThaiKhach.showModalAdd();
        },
    },
    watch: {
        newCustomer: {
            handler(val, oldVal) {
                if (val.LaCaNhan === "false") {
                    $('#newCustomer_gender').hide()
                } else {
                    $('#newCustomer_gender').show()
                }
            }
            , deep: true
        }
    }


})

$(window.document).on('shown.bs.modal', '.modal', function () {
    $.datepicker.setDefaults($.datepicker.regional["vi"]);
    $("#txtNgaySinh").datepicker({
        showOn: 'focus',
        altFormat: "dd/mm/yy",
        showOn: "button",
        buttonImageOnly: true,
        dateFormat: "dd/mm/yy",
        buttonImage: '/Content/images/icon/ngaysinh.png',
    }).mask('99/99/9999').on("change", function (e) {
        vmThemMoiKhach.newCustomer.NgaySinh_NgayTLap = e.target.value;
    });
});

$(document).mouseup(function (e) {
    var container = $(".dropdown-menu");
    if (!container.is(e.target) && container.has(e.target).length === 0) {
        container.hide();
    }
});

$(function () {
    $('#ThemNhomKhachHang').on('hidden.bs.modal', function () {
        if (vmThemMoiNhomKhach.saveOK) {
            if (vmThemMoiNhomKhach.isNew) {
                var idNhomDT = vmThemMoiNhomKhach.newGroup.ID;
                for (let i = 0; i < vmThemMoiKhach.listData.NhomKhachs.length; i++) {
                    let itFor = vmThemMoiKhach.listData.NhomKhachs[i];
                    if (itFor.ID === idNhomDT) {
                        vmThemMoiKhach.listData.NhomKhachs.splice(i, 1);
                        break;
                    }
                }
                for (let i = 0; i < vmThemMoiNhomKhach.newGroup.NhomDT_DonVi.length; i++) {
                    vmThemMoiNhomKhach.newGroup.NhomDT_DonVi[i].ID_NhomDoiTuong = idNhomDT;
                }
                vmThemMoiKhach.listData.NhomKhachs.unshift(vmThemMoiNhomKhach.newGroup);
                vmThemMoiKhach.NhomKhachChosed.unshift(vmThemMoiNhomKhach.newGroup);
                vmThemMoiKhach.newCustomer.ListIDNhomKhach = [idNhomDT];
            }
        }
    })

    $('#modalNguonKhach').on('hidden.bs.modal', function () {
        if ($('#ThemMoiKhachHang').hasClass('in')) {
            if (vmNguonKhach.saveOK) {
                for (let i = 0; i < vmThemMoiKhach.listData.NguonKhachs.length; i++) {
                    if (vmThemMoiKhach.listData.NguonKhachs.ID === obj.ID) {
                        vmThemMoiKhach.listData.NguonKhachs.splice(i, 1);
                        break;
                    }
                }
                vmThemMoiKhach.newCustomer.ID_NguonKhach = vmNguonKhach.newNguonKhach.ID;
                vmThemMoiKhach.newCustomer.TenNguonKhach = vmNguonKhach.newNguonKhach.TenNguonKhach;
                vmThemMoiKhach.listData.NguonKhachs.unshift({
                    ID: vmNguonKhach.newNguonKhach.ID,
                    TenNguonKhach: vmNguonKhach.newNguonKhach.TenNguonKhach,
                })
            }
        }
    })
    $('#modalStatus_Customer').on('hidden.bs.modal', function () {
        if ($('#ThemMoiKhachHang').hasClass('in')) {
            if (vmTrangThaiKhach.saveOK) {
                if (vmTrangThaiKhach.isNew) {
                    vmThemMoiKhach.newCustomer.ID_TrangThai = vmTrangThaiKhach.newTrangThai.ID;
                    vmThemMoiKhach.newCustomer.TenTrangThai = vmTrangThaiKhach.newTrangThai.TenTrangThai;
                    for (let i = 0; i < vmThemMoiKhach.listData.TrangThaiKhachs.length; i++) {
                        if (vmThemMoiKhach.listData.TrangThaiKhachs.ID === obj.ID) {
                            vmThemMoiKhach.listData.TrangThaiKhachs.splice(i, 1);
                            break;
                        }
                    }
                }
            }
        }
    })
})
