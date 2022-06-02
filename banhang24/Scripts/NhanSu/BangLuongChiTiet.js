var vmSalaryDetail = new Vue({
    el: '#salarydetail',
    data: {
        idDonVi: $('#hd_IDdDonVi').val(),
        idNhanVien: $('.idnhanvien').text(),
        userLogin: $('#txtTenTaiKhoan').text().trim(),
        urlAPINhanSu: '/api/DanhMuc/NS_NhanSuAPI/',
        urlAPINhanVien: '/api/DanhMuc/NS_NhanVienAPI/',
        ctSalary: 'ctSalary',
        ctBasic: 'ctBasic',
        txtSearchStaff: '',
        txtSearchUser: '',
        pagesize: 10,
        currentpage: 0,
        loadding: false,

        filter: {
            isCheckAll: true,
            arrFilter: [
                { ID: 1, Value: true },
                { ID: 2, Value: true },
                { ID: 3, Value: true },
                { ID: 4, Value: true },
            ]
        },
        luongot: {
            hesoluong: 100,
            loaiOT: 1,// 1.ot hành chính (ngày lễ nghỉ), 2. ot them gio
            LaPhanTram: false,
            applyAllNVien: false,
            applyAllCa: false,
            index: 0,// vị trí dòng cần update (nếu không apply all)
            arrIDNhanVien: [],
            isCheckAll: false,
            chitiet: [],
        },
        phucap: {
            index: 0,
            giatri: 0,
            isCheckAll: false,
            isPTramLuong: false,
            LaPhuCapTheoNgay: false,
            applyAllNVien: false,
            arrIDNhanVien: [],
            chitiet: [],
            tongthanhtien: 0,
        },
        giamtru: {
            index: 0,
            giatri: 0,
            isCheckAll: false,
            LaGiamTruTheoLan: false,
            isPTramLuong: false,
            applyAllNVien: false,
            arrIDNhanVien: [],
            chitiet: [],
            tongthanhtien: 0,
        },
        listdata: {
            AllStaffs: [],
            AllUsers: [],
            StaffSearchs: [],
            UserSearchs: [],
            LuongChinh: [],
            LuongOT: [],
            PhuCap: [],
            GiamTru: [],
        },
        bangluongchitiet: [],
        bangluong: {
            ID: '00000000-0000-0000-0000-000000000000',
            MaBangLuong: '',
            TenBangLuong: '',
            KyTinhLuong: '',
            TuNgay: '',
            DenNgay: '',
            TuNgayYYMMDD: moment().startOf('month').format('YYYY-MM-DD'),
            DenNgayYYMMDD: moment().endOf('month').format('YYYY-MM-DD'),
            ID_DonVi: '',
            NgayCongChuan: 0,
            TrangThai: 1,
            GhiChu: '',
            NgayLapBangLuong: moment(new Date()).format('DD/MM/YY HH:mm'),
            TenChiNhanh: $('#_txtTenDonVi').text(),

            LuongChinh: 0,
            LuongOT: 0,
            HoaHong: 0,
            PhuCap: 0,
            KhenThuong: 0,
            GiamTru: 0,
            TongLuong: 0
        },
    },
    created: function () {
        var self = this;
        self.Guid_Empty = '00000000-0000-0000-0000-000000000000';
        // declare const
        self.BOX_LOADING = `<div class="loading-box">
                                    <div class="loading-bg">
                                        <div class="loading-obj">
                                            <div class="loader"></div>
                                            <div class=" loading loading-done"><i class="material-icons">check</i></div>
                                        </div>
                                    </div>
                                    </div>`;
        // only get nvien co taikhoan dangnhap
        $.getJSON(self.urlAPINhanVien + 'GetNS_NhanVien_DaTaoND?idDonVi=' + self.idDonVi).done(function (data) {
            if (data !== null) {
                self.listdata.AllUsers = data;
                self.listdata.UserSearchs = data.slice(0, 20);
                // defaul user = userlogin
                let user = $.grep(data, function (x) {
                    return x.ID === self.idNhanVien;
                });
                if (user.length > 0) {
                    self.choseStaff(user[0], 2);
                }
            }
        });

        var fromSalary = localStorage.getItem('fromSalary');
        if (fromSalary !== null) {
            fromSalary = parseInt(fromSalary);
            var lcBangLuong = localStorage.getItem('lcbangluong');
            if (lcBangLuong !== null) {
                lcBangLuong = JSON.parse(lcBangLuong);
                switch (fromSalary) {
                    case 1:// insert
                        self.tinhLuong(lcBangLuong);
                        break;
                    case 2://update
                        var from = moment(lcBangLuong.TuNgay).format('DD/MM/YYYY');
                        var to = moment(lcBangLuong.DenNgay).format('DD/MM/YYYY');
                        self.bangluong = {
                            ID: lcBangLuong.ID,
                            MaBangLuong: lcBangLuong.MaBangLuong,
                            TenBangLuong: lcBangLuong.TenBangLuong,
                            KyTinhLuong: from.concat(' - ', to),
                            ID_DonVi: self.idDonVi,
                            ID_NhanVienDuyet: lcBangLuong.ID_NhanVienDuyet,
                            TuNgay: from,
                            DenNgay: to,
                            TuNgayYYMMDD: lcBangLuong.TuNgay,
                            DenNgayYYMMDD: lcBangLuong.DenNgay,
                            GhiChu: lcBangLuong.GhiChu,
                            TenChiNhanh: $('#_txtTenDonVi').text(),
                        }

                        var luongchinh = 0;
                        var luongot = 0;
                        var hoahong = 0;
                        var phucap = 0;
                        var khenthuong = 0;
                        var giamtru = 0;
                        var tongluong = 0;
                        var ngaycongchuan = 0;

                        var ctluong = localStorage.getItem('lcChiTietLuong');
                        if (ctluong !== null) {
                            ctluong = JSON.parse(ctluong);
                            ngaycongchuan = ctluong[0].NgayCongChuan;
                            luongchinh += ctluong[0].TongLuongChinh;
                            luongot += ctluong[0].TongLuongOT;
                            hoahong += ctluong[0].ChietKhau;
                            phucap += ctluong[0].TongPhuCapKhac + ctluong[0].TongPhuCapCoBan;
                            khenthuong += ctluong[0].TongKhenThuong;
                            giamtru += ctluong[0].TongTienPhatAll;
                            tongluong += ctluong[0].TongLuongSauGiamTru;

                            for (let i = 0; i < ctluong.length; i++) {
                                let itFor = ctluong[i];
                                ctluong[i].LuongThucNhan = itFor.LuongSauGiamTru;
                                ctluong[i].TongLuongNhan = itFor.LuongChinh; // assign to save DB
                                ctluong[i].PhuCap = itFor.PhuCapCoBan + itFor.PhuCapKhac;
                                ctluong[i].GiamTruCoDinh_TheoPTram = 0;
                                ctluong[i].PhuCapCoDinh_TheoPtramLuong = 0;

                                ctluong[i].LuongOT_isCheck = false;
                                ctluong[i].PhuCap_isCheck = false;
                                ctluong[i].GiamTru_isCheck = false;
                                ctluong[i].ListLuongOT = [];
                                ctluong[i].ListPhuCap = [];
                                ctluong[i].ListGiamTru = [];
                            }

                            self.bangluong.NgayCongChuan = ngaycongchuan;
                            self.bangluong.LuongChinh = luongchinh;
                            self.bangluong.LuongOT = luongot;
                            self.bangluong.HoaHong = hoahong;
                            self.bangluong.PhuCap = phucap;
                            self.bangluong.GiamTru = giamtru;
                            self.bangluong.TongLuong = tongluong;
                            self.bangluongchitiet = ctluong;

                            localStorage.setItem(self.ctSalary, JSON.stringify(self.bangluongchitiet));
                            localStorage.setItem(self.ctBasic, JSON.stringify(self.bangluong));
                        }
                        localStorage.removeItem('lcbangluong');
                        localStorage.removeItem('lcChiTietLuong');
                        localStorage.removeItem('fromSalary');
                        break;
                }
            }
        }
        else {
            var ct = localStorage.getItem(self.ctSalary);
            if (ct !== null) {
                ct = JSON.parse(ct);
                self.bangluongchitiet = ct;

                let bl = localStorage.getItem(self.ctBasic);
                if (bl !== null) {
                    bl = JSON.parse(bl);
                    self.bangluong = bl;
                }
            }
        }
    },
    methods: {
        locdau: function (obj) {
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
        },
        formatNumberToFloat: function (objVal) {
            if (objVal === undefined || objVal === null) {
                return 0;
            }
            else {
                var value = parseFloat(objVal.toString().replace(/,/g, ''));
                if (isNaN(value)) {
                    return 0;
                }
                else {
                    return value;
                }
            }
        },
        formatNumber3Digit: function (number, decimalDot) {
            var self = this;
            decimalDot = decimalDot || 3;
            if (number === undefined || number === null) {
                return 0;
            }
            else {
                number = self.formatNumberToFloat(number);
                number = Math.round(number * Math.pow(10, decimalDot)) / Math.pow(10, decimalDot);
                if (number !== null) {
                    var lastone = number.toString().split('').pop();
                    if (lastone !== '.') {
                        number = parseFloat(number);
                    }
                }
                if (isNaN(number)) {
                    number = 0;
                }
                return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            }
        },
        ajaxHelper: function (uri, method, data) {
            return $.ajax({
                type: method,
                url: uri,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: data ? JSON.stringify(data) : null,
            });
        },
        saveDiary: function (obj) {
            var self = this;
            var myData = {
                objDiary: obj,
            };
            self.ajaxHelper('/api/DanhMuc/SaveDiary/' + "post_NhatKySuDung", 'POST', myData).done(function (x) {

            });
        },
        getNhanVien_coBangLuong: function () {
            console.log(22)
            var self = this;
            var fromDate = '';
            var toDate = '';
            let bl = localStorage.getItem(self.ctBasic);
            if (bl !== null) {
                bl = JSON.parse(bl);
                fromDate = bl.TuNgayYYMMDD;
                toDate = bl.DenNgayYYMMDD;
            }
            else {
                fromDate = self.bangluong.TuNgayYYMMDD;
                toDate = self.bangluong.TuNgayYYMMDD;
            }
            $.getJSON(self.urlAPINhanVien + 'GetNhanVien_CoBangLuong?fromDate=' + fromDate + '&toDate=' + toDate + '&idDonVi=' + self.idDonVi).done(function (x) {
                if (x.res === true) {
                    self.listdata.AllStaffs = x.dataSoure;
                    self.listdata.StaffSearchs = x.dataSoure.slice(0, 20);
                }
                else {
                 
                }
            });
        },
        seachStaff: function (e) {
            var self = this;
            var txt = commonStatisJs.convertVieToEng(self.txtSearchStaff);
            var staffs = self.listdata.AllStaffs.filter(function (x) {
                return x['NameFull'].indexOf(txt) > -1;
            }).slice(0, 20);
            self.listdata.StaffSearchs = staffs;

            vmSalaryDetail.loadFocus(e, '.drop-list-nv', 1);
        },
        seachUserLogin: function (e) {
            var self = this;
            var txt = commonStatisJs.convertVieToEng(self.txtSearchUser);
            var users = self.listdata.AllUsers.filter(function (x) {
                return x['NameFull'].indexOf(txt) > -1;
            }).slice(0, 20);
            self.listdata.UserSearchs = users;
            vmSalaryDetail.loadFocus(e, '.drop-list-creator', 2);
        },
        loadFocus: function (event, elm, type) {
            var self = this;
            var keycode = event.keyCode || event.which;
            var lstLi = $(elm).find('li');
            var lenLi = lstLi.length;
            var liFocus = $(elm).find('.hover-li-ddl');
            var index = 0;
            if (liFocus.length === 0) {
                index = -1;
            }
            else {
                index = $(liFocus).index();
            }
            $(lstLi).removeClass('hover-li-ddl');
            $(elm).css('display', 'block');

            switch (keycode) {
                case 13:
                    if (index !== -1) {
                        var itChosed = [];
                        if (type === 1) {
                            itChosed = $.grep(self.listdata.StaffSearchs, function (e, i) {
                                return i === index;
                            });
                        }
                        else {
                            itChosed = $.grep(self.listdata.UserSearchs, function (e, i) {
                                return i === index;
                            });
                        }
                        if (itChosed.length > 0) {
                            vmSalaryDetail.choseStaff(itChosed[0], type);
                            $(elm).css('display', 'none');
                        }
                    }
                    break;
                case 37:
                case 38:
                    index = index - 1;
                    $(lstLi).eq(index).addClass('hover-li-ddl');

                    if (index < 0) {
                        index = lenLi - 1;
                        $(elm).children('ul').stop().animate({
                            scrollTop: $(elm).children('ul').offset().top + 200
                        }, 200);
                    }
                    else if (index > 0 && index < 5) {
                        $(elm).children('ul').stop().animate({
                            scrollTop: $(elm).children('ul').offset().top - 200
                        }, 200);
                    }
                    break;
                case 39:
                case 40:
                    index = index + 1;
                    $(lstLi).eq(index).addClass('hover-li-ddl');

                    if (index >= lenLi) {
                        $(elm).children('ul').stop().animate({
                            scrollTop: $(elm).children('ul').offset().top - 200
                        }, 200);
                    }
                    else if (index > 4 && index < lenLi) {
                        $(elm).children('ul').stop().animate({
                            scrollTop: $(elm).children('ul').offset().top + 100
                        }, 200);
                    }
                    break;
            }
        },
        choseStaff: function (item, type) {
            var self = this;
            var bl = localStorage.getItem(self.ctBasic);
            if (bl !== null) {
                bl = JSON.parse(bl);

                if (type == 1) {
                    self.txtSearchStaff = item.TenNhanVien;
                    let exStaff = $.grep(self.bangluongchitiet, function (x) {
                        return x.ID_NhanVien === item.ID;
                    });
                    // check staff exist bangluongct
                    if (exStaff.length > 0) {
                        commonStatisJs.ShowMessageDanger('Nhân viên đã tồn tại trong bảng lương');
                        return;
                    }

                    var param = {
                        LstIDChiNhanh: [self.idDonVi],
                        LstIDNhanVien: [item.ID],
                        LstKieuLuong: [1, 2, 3, 4],
                        FromDate: self.bangluong.TuNgayYYMMDD,
                        ToDate: self.bangluong.DenNgayYYMMDD,
                        CurrentPage: 0,
                        PageSize: 20,
                    };
                    self.ajaxHelper(self.urlAPINhanSu + 'TinhLuongNhanVien', 'POST', param).done(function (x) {
                        console.log(x)
                        if (x.res === true) {
                            let dataSource = x.dataSoure.data;
                            if (dataSource.length > 0) {
                                dataSource[0].LuongOT_isCheck = false;
                                self.bangluongchitiet.unshift(dataSource[0]);

                                localStorage.setItem(self.ctSalary, JSON.stringify(self.bangluongchitiet));// save to cache
                                self.updateInforLuong();
                            }
                            else {
                                commonStatisJs.ShowMessageDanger('Chưa thiết lập lương cho nhân viên');
                            }
                        }
                    });
                }
                else {
                    self.txtSearchUser = item.TenNhanVien;
                    self.bangluong.ID_NhanVienDuyet = item.ID;

                    bl = self.bangluong;
                    localStorage.setItem(self.ctBasic, JSON.stringify(bl));
                }
            }
        },

        filter_checkAll: function () {
            var self = this;
            for (let i = 0; i < self.filter.arrFilter.length; i++) {
                self.filter.arrFilter[i].Value = self.filter.isCheckAll;
            }
            self.tinhLuong();
        },
        filter_checkOne: function () {
            var self = this;
            self.filter.isCheckAll = (self.filter.arrFilter.filter(x => x.Value === true).length === 4);// check all if all = true
            self.tinhLuong();
        },

        tinhLuong: function (obj) {
            console.log('lcBangLuong ', obj)
            var self = this;
            var from = '', to = '', id = self.Guid_Empty;
            if (obj != undefined) {
                if (obj.ID !== undefined) {
                    from = obj.TuNgay;
                    to = obj.DenNgay;
                    id = obj.ID;
                    self.bangluong.ID = id;
                    self.bangluong.GhiChu = obj.GhiChu;
                }
                else {
                    from = self.bangluong.TuNgayYYMMDD;
                    to = self.bangluong.DenNgayYYMMDD;
                }
            }
            else {
                from = self.bangluong.TuNgayYYMMDD;
                to = self.bangluong.DenNgayYYMMDD;
            }

            var lstKieu = $.map(self.filter.arrFilter.filter(x => x.Value === true), function (y) {
                return y.ID;
            });
            lstKieu.push(0);

            $('salarydetail').gridLoader({ show: true, iconloading: self.BOX_LOADING });
            var param = {
                LstIDChiNhanh: [self.idDonVi],
                LstIDNhanVien: [],
                LstKieuLuong: lstKieu,
                FromDate: from,
                ToDate: to,
                CurrentPage: 0,
                PageSize: 50,
            };


            self.ajaxHelper(self.urlAPINhanSu + 'TinhLuongNhanVien', 'POST', param).done(function (x) {
                console.log(x)
                if (x.res === true) {
                    var dataSource = x.dataSoure;
                    self.bangluongchitiet = dataSource.data;
                    // set default some properties
                    for (let i = 0; i < self.bangluongchitiet.length; i++) {
                        self.bangluongchitiet[i].LuongOT_isCheck = false;
                        self.bangluongchitiet[i].PhuCap_isCheck = false;
                        self.bangluongchitiet[i].GiamTru_isCheck = false;
                        self.bangluongchitiet[i].ListLuongOT = [];
                        self.bangluongchitiet[i].ListPhuCap = [];
                        self.bangluongchitiet[i].ListGiamTru = [];
                    }
                    // sumluong
                    let fromDate = moment(from, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    let toDate = moment(to, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    self.bangluong = {
                        ID: id,
                        TenBangLuong: 'Bảng lương '.concat(fromDate, ' - ', toDate),
                        KyTinhLuong: fromDate.concat(' - ', toDate),
                        ID_DonVi: self.idDonVi,
                        ID_NhanVienDuyet: self.idNhanVien,
                        TuNgay: fromDate,
                        DenNgay: toDate,
                        TuNgayYYMMDD: from,
                        DenNgayYYMMDD: to,
                        NgayCongChuan: dataSource.data.length > 0 ? dataSource.data[0].NgayCongChuan : 0,
                        TenChiNhanh: $('#_txtTenDonVi').text(),
                        GhiChu: self.bangluong.GhiChu,

                        LuongChinh: dataSource.LuongChinh,
                        LuongOT: dataSource.LuongOT,
                        HoaHong: dataSource.HoaHong,
                        PhuCap: dataSource.PhuCap,
                        KhenThuong: dataSource.KhenThuong,
                        GiamTru: dataSource.GiamTru,
                        TongLuong: dataSource.TongLuong,
                    }

                    // if update bangluong--> get id, mabangluong old
                    if (id === self.Guid_Empty) {
                        let bangluongCache = localStorage.getItem(self.ctBasic);
                        if (bangluongCache !== null) {
                            bangluongCache = JSON.parse(bangluongCache);
                            self.bangluong.ID = bangluongCache.ID;
                            self.bangluong.MaBangLuong = bangluongCache.MaBangLuong;
                        }
                    }
                    console.log('self.bangluong ', self.bangluong)
                    localStorage.setItem(self.ctSalary, JSON.stringify(self.bangluongchitiet));
                    localStorage.setItem(self.ctBasic, JSON.stringify(self.bangluong));
                }

                localStorage.removeItem('lcbangluong');
                localStorage.removeItem('fromSalary');
            });
        },
        showModalLuongChinh: function (item) {
            var self = this;
            if (item.LuongChinh > 0 && navigator.onLine) {
                self.listdata.LuongChinh = [];

                let tungay = self.bangluong.TuNgayYYMMDD;
                let denngay = self.bangluong.DenNgayYYMMDD;
                $.getJSON(self.urlAPINhanSu + 'GetLuongChinh_ofNhanVien?idChiNhanh=' + self.idDonVi + '&idNhanVien=' + item.ID_NhanVien
                    + '&tungay=' + tungay + '&denngay=' + denngay + '&ngaycongchuan=' + item.NgayCongChuan).done(function (x) {
                        console.log(x)
                        if (x.res === true) {
                            self.listdata.LuongChinh = x.dataSoure;
                            $('#luongchinh_modal').modal('show');
                        }
                    });
            }
        },

        luongOT_checkAll: function () {
            var self = this;
            if (self.luongot.isCheckAll) {
                self.luongot.arrIDNhanVien = $.map(self.bangluongchitiet, function (x) {
                    return x.ID_NhanVien;
                });
            }
            else {
                self.luongot.arrIDNhanVien = [];
            }

            self.phucap.arrIDNhanVien = [];
            self.giamtru.arrIDNhanVien = [];
            self.phucap.isCheckAll = false;
            self.giamtru.isCheckAll = false;

            for (let i = 0; i < self.bangluongchitiet.length; i++) {
                self.bangluongchitiet[i].LuongOT_isCheck = self.luongot.isCheckAll;
                self.bangluongchitiet[i].PhuCap_isCheck = false;
                self.bangluongchitiet[i].GiamTru_isCheck = false;
            }
        },
        luongOT_changeCheck: function (ctluong) {
            var self = this;
            self.luongot.arrIDNhanVien = [];
            for (let i = 0; i < self.bangluongchitiet.length; i++) {
                let itFor = self.bangluongchitiet[i];
                if (itFor.LuongOT_isCheck) {
                    self.luongot.arrIDNhanVien.push(itFor.ID_NhanVien);
                }
            }
            if (self.luongot.arrIDNhanVien.length > 0) {
                for (let i = 0; i < self.bangluongchitiet.length; i++) {
                    self.bangluongchitiet[i].GiamTru_isCheck = false;
                    self.bangluongchitiet[i].PhuCap_isCheck = false;
                }
                self.giamtru.arrIDNhanVien = [];
                self.phucap.arrIDNhanVien = [];
            }
        },
        showModalLuongOT: function (item) {
            var self = this;
            self.listdata.LuongOT = [];// reset
            self.luongot.isCheckAll = false;
            vmSalaryDetail.luongOT_checkAll();
            self.luongot.arrIDNhanVien = [item.ID_NhanVien];
            vmSalaryDetail.showmany_luongOT();
        },
        showmany_luongOT: function () {
            var self = this;
            var ctluong = localStorage.getItem(self.ctSalary);
            if (ctluong !== null) {
                ctluong = JSON.parse(ctluong);
                var myData = $.map(self.luongot.arrIDNhanVien, function (x) {
                    return x;
                });
                if (navigator.onLine && myData.length > 0) {
                    var tungay = self.bangluong.TuNgayYYMMDD;
                    var denngay = self.bangluong.DenNgayYYMMDD;
                    var ngaycongchuan = self.bangluong.NgayCongChuan;
                    console.log(myData)

                    self.ajaxHelper(self.urlAPINhanSu + 'GetLuongOT_ofNhanVien?idChiNhanh=' + self.idDonVi + '&lstIDNhanVien= ' + myData
                        + '&tungay=' + tungay + '&denngay=' + denngay + '&ngaycongchuan=' + ngaycongchuan, 'POST', myData).done(function (x) {
                            console.log(x)
                            if (x.res === true) {
                                let data = x.dataSoure;
                                let allLuongOT = [];
                                // get arrNV had setup luongot
                                let nvHadSet = [];
                                //for (let i = 0; i < self.bangluongchitiet.length; i++) {
                                //    let itFor = self.bangluongchitiet[i];
                                //    if ($.type(itFor.ListLuongOT) !== 'array' && $.inArray(itFor.ID_NhanVien, myData) > -1) {// check not object && thuoc nhanvien dang chon
                                //        nvHadSet.push(itFor.ID_NhanVien);
                                //        allLuongOT.push(itFor.ListLuongOT);
                                //    }
                                //}

                                // assign lungot for nv not setup
                                for (let j = 0; j < data.length; j++) {
                                    let forOut = data[j];
                                    if ($.inArray(forOut.ID_NhanVien, nvHadSet) === -1) {
                                        for (let i = 0; i < self.bangluongchitiet.length; i++) {
                                            let forIn = self.bangluongchitiet[i];
                                            if (forOut.ID_NhanVien === forIn.ID_NhanVien) {
                                                self.bangluongchitiet[i].ListLuongOT = forOut;
                                                allLuongOT.push(forOut);
                                                break;
                                            }
                                        }
                                    }
                                }
                                self.listdata.LuongOT = allLuongOT;
                                $('#lamthem_modal').modal('show');

                                ctluong = self.bangluongchitiet;
                                localStorage.setItem(self.ctSalary, JSON.stringify(ctluong));
                            }
                        });
                }
            }

        },

        phucap_checkAll: function () {
            var self = this;
            if (self.phucap.isCheckAll) {
                self.phucap.arrIDNhanVien = $.map(self.bangluongchitiet, function (x) {
                    return x.ID_NhanVien;
                });
            }
            else {
                self.phucap.arrIDNhanVien = [];
            }
            self.luongot.arrIDNhanVien = [];
            self.giamtru.arrIDNhanVien = [];
            self.luongot.isCheckAll = false;
            self.giamtru.isCheckAll = false;

            for (let i = 0; i < self.bangluongchitiet.length; i++) {
                self.bangluongchitiet[i].PhuCap_isCheck = self.phucap.isCheckAll;
                self.bangluongchitiet[i].LuongOT_isCheck = false;
                self.bangluongchitiet[i].GiamTru_isCheck = false;
            }
        },
        phucap_changeCheck: function (ctluong) {
            var self = this;
            self.phucap.arrIDNhanVien = [];
            for (let i = 0; i < self.bangluongchitiet.length; i++) {
                let itFor = self.bangluongchitiet[i];
                if (itFor.PhuCap_isCheck) {
                    self.phucap.arrIDNhanVien.push(itFor.ID_NhanVien);
                }
            }
            if (self.phucap.arrIDNhanVien.length > 0) {
                for (let i = 0; i < self.bangluongchitiet.length; i++) {
                    self.bangluongchitiet[i].LuongOT_isCheck = false;
                    self.bangluongchitiet[i].GiamTru_isCheck = false;
                }
                self.luongot.arrIDNhanVien = [];
                self.giamtru.arrIDNhanVien = [];
            }
        },
        showModalPhuCap: function (item) {
            var self = this;
            self.phucap.tongthanhtien = self.formatNumber3Digit(item.PhuCap);

            self.listdata.PhuCap = [];// reset
            self.phucap.isCheckAll = false;
            vmSalaryDetail.phucap_checkAll();
            self.phucap.arrIDNhanVien = [item.ID_NhanVien];
            vmSalaryDetail.showmany_PhuCap();
        },
        showmany_PhuCap: function () {
            var self = this;

            var tungay = self.bangluong.TuNgayYYMMDD;
            var denngay = self.bangluong.DenNgayYYMMDD;
            var lstID = $.map(self.phucap.arrIDNhanVien, function (x) {
                return x;
            });

            var ctluong = localStorage.getItem(self.ctSalary);
            if (ctluong !== null) {
                ctluong = JSON.parse(ctluong);

                var allPhuCap = [];
                // get arrNV had setup phucap
                var nvHadSet = [];
                for (let i = 0; i < ctluong.length; i++) {
                    let itFor = ctluong[i];
                    //if ($.type(itFor.ListPhuCap) !== 'array' && $.inArray(itFor.ID_NhanVien, lstID) > -1) {// check not object && thuoc nhanvien dang chon
                    //    nvHadSet.push(itFor.ID_NhanVien);
                    //    allPhuCap.push(itFor.ListPhuCap);
                    //}
                }

                self.ajaxHelper(self.urlAPINhanSu + 'PhuCap_ofNhanVien?idChiNhanh=' + self.idDonVi + '&lstIDNhanVien= ' + lstID
                    + '&tungay=' + tungay + '&denngay=' + denngay, 'POST', lstID).done(function (x) {
                        console.log(x)
                        if (x.res === true) {
                            let data = x.dataSoure;

                            // assign phucap for nv not setup
                            for (let j = 0; j < data.length; j++) {
                                let forOut = data[j];
                                if ($.inArray(forOut.ID_NhanVien, nvHadSet) === -1) {
                                    for (let i = 0; i < self.bangluongchitiet.length; i++) {
                                        let forIn = self.bangluongchitiet[i];
                                        if (forOut.ID_NhanVien === forIn.ID_NhanVien) {
                                            for (let k = 0; k < forOut.LstLoaiPhuCap.length; k++) {
                                                let loaipc = forOut.LstLoaiPhuCap[k];
                                                // update thanhtien neu phucap codinh %
                                                if (loaipc.LoaiPhuCap === 53) {
                                                    for (let m = 0; m < loaipc.LstDetail.length; m++) {
                                                        let detail = loaipc.LstDetail[m];
                                                        forOut.LstLoaiPhuCap[k].LstDetail[m].ThanhTien =
                                                            self.formatNumber3Digit(forIn.LuongChinh * self.formatNumberToFloat(detail.PhuCapCoDinh) / 100);
                                                        console.log('forOut', forOut)
                                                    }
                                                }
                                            }
                                            self.bangluongchitiet[i].ListPhuCap = forOut;
                                            allPhuCap.push(forOut);
                                            break;
                                        }
                                    }
                                }
                            }
                            self.listdata.PhuCap = allPhuCap;

                            ctluong = self.bangluongchitiet;
                            localStorage.setItem(self.ctSalary, JSON.stringify(ctluong));

                            $('#phucap_modal').modal('show');
                        }
                    });
            }
        },

        giamtru_checkAll: function () {
            var self = this;
            if (self.giamtru.isCheckAll) {
                self.giamtru.arrIDNhanVien = $.map(self.bangluongchitiet, function (x) {
                    return x.ID_NhanVien;
                });
            }
            else {
                self.giamtru.arrIDNhanVien = [];
            }

            self.luongot.arrIDNhanVien = [];
            self.phucap.arrIDNhanVien = [];
            self.luongot.isCheckAll = false;
            self.phucap.isCheckAll = false;

            for (let i = 0; i < self.bangluongchitiet.length; i++) {
                self.bangluongchitiet[i].GiamTru_isCheck = self.giamtru.isCheckAll;
                self.bangluongchitiet[i].PhuCap_isCheck = false;
                self.bangluongchitiet[i].LuongOT_isCheck = false;
            }
        },
        giamtru_changeCheck: function (ctluong) {
            var self = this;
            self.giamtru.arrIDNhanVien = [];
            for (let i = 0; i < self.bangluongchitiet.length; i++) {
                let itFor = self.bangluongchitiet[i];
                if (itFor.GiamTru_isCheck) {
                    self.giamtru.arrIDNhanVien.push(itFor.ID_NhanVien);
                }
            }
            if (self.giamtru.arrIDNhanVien.length > 0) {
                for (let i = 0; i < self.bangluongchitiet.length; i++) {
                    self.bangluongchitiet[i].LuongOT_isCheck = false;
                    self.bangluongchitiet[i].PhuCap_isCheck = false;
                }
                self.luongot.arrIDNhanVien = [];
                self.phucap.arrIDNhanVien = [];
            }
        },
        showModalGiamTru: function (item) {
            var self = this;
            self.giamtru.tongthanhtien = self.formatNumber3Digit(item.TongTienPhat);

            self.listdata.GiamTru = [];// reset
            self.giamtru.isCheckAll = false;
            vmSalaryDetail.giamtru_checkAll();
            self.giamtru.arrIDNhanVien = [item.ID_NhanVien];
            vmSalaryDetail.showmany_GiamTru();
        },
        showmany_GiamTru: function () {
            var self = this;

            var tungay = self.bangluong.TuNgayYYMMDD;
            var denngay = self.bangluong.DenNgayYYMMDD;
            var lstID = $.map(self.giamtru.arrIDNhanVien, function (x) {
                return x;
            });

            var ctluong = localStorage.getItem(self.ctSalary);
            if (ctluong !== null) {
                ctluong = JSON.parse(ctluong);

                var allGiamTru = [];
                var nvHadSet = [];
                for (let i = 0; i < ctluong.length; i++) {
                    let itFor = ctluong[i];
                    //if ($.type(itFor.ListGiamTru) !== 'array' && $.inArray(itFor.ID_NhanVien, lstID) > -1) {// check not object && thuoc nhanvien dang chon
                    //    nvHadSet.push(itFor.ID_NhanVien);
                    //    allGiamTru.push(itFor.ListGiamTru);
                    //}
                }

                self.ajaxHelper(self.urlAPINhanSu + 'GiamTru_ofNhanVien?idChiNhanh=' + self.idDonVi + '&lstIDNhanVien= ' + lstID
                    + '&tungay=' + tungay + '&denngay=' + denngay, 'POST', lstID).done(function (x) {
                     
                        if (x.res === true) {
                            let data = x.dataSoure;

                            for (let j = 0; j < data.length; j++) {
                                let forOut = data[j];
                                if ($.inArray(forOut.ID_NhanVien, nvHadSet) === -1) {
                                    for (let i = 0; i < self.bangluongchitiet.length; i++) {
                                        let forIn = self.bangluongchitiet[i];
                                        if (forOut.ID_NhanVien === forIn.ID_NhanVien) {
                                            for (let k = 0; k < forOut.LstLoaiGiamTru.length; k++) {
                                                let loaipc = forOut.LstLoaiGiamTru[k];
                                                if (loaipc.LoaiGiamTru === 63) {
                                                    for (let m = 0; m < loaipc.LstDetail.length; m++) {
                                                        let detail = loaipc.LstDetail[m];
                                                        forOut.LstLoaiGiamTru[k].LstDetail[m].ThanhTien =
                                                            self.formatNumber3Digit(forIn.TongLuongNhan * self.formatNumberToFloat(detail.GiamTruCoDinh) / 100);
                                                    }
                                                }
                                            }
                                            self.bangluongchitiet[i].ListGiamTru = forOut;
                                            allGiamTru.push(forOut);
                                            break;
                                        }
                                    }
                                }
                            }
                            self.listdata.GiamTru = allGiamTru;

                            ctluong = self.bangluongchitiet;
                            localStorage.setItem(self.ctSalary, JSON.stringify(ctluong));

                            $('#giamtru_modal').modal('show');
                        }
                    });
            }
        },

        showModalKhenThuong: function (item) {

        },

        ctluong_removeItem: function (index) {
            var self = this;
            self.bangluongchitiet.splice(index, 1);
            vmSalaryDetail.updateInforLuong();
        },
        ctluong_removeAll: function () {
            var self = this;
            self.bangluongchitiet = [];
            vmSalaryDetail.updateInforLuong();
        },

        updateInforLuong: function () {
            var self = this;
            var luongchinh = 0;
            var luongOT = 0;
            var hoahong = 0;
            var phucap = 0;
            var khenthuong = 0;
            var giamtru = 0;
            var tongluong = 0;

            for (let i = 0; i < self.bangluongchitiet.length; i++) {
                let ctluong = self.bangluongchitiet[i];
                luongchinh += ctluong.LuongChinh;
                luongOT += ctluong.LuongOT;
                hoahong += ctluong.ChietKhau;
                phucap += ctluong.PhuCap;
                khenthuong += ctluong.KhenThuong;
                giamtru += ctluong.TongTienPhat;
                tongluong = luongchinh + luongOT + hoahong + phucap - giamtru;
            }
            self.bangluong.LuongChinh = luongchinh;
            self.bangluong.LuongOT = luongOT;
            self.bangluong.HoaHong = hoahong;
            self.bangluong.PhuCap = phucap;
            self.bangluong.KhenThuong = khenthuong;
            self.bangluong.GiamTru = giamtru;
            self.bangluong.TongLuong = tongluong;
            localStorage.setItem(self.ctBasic, JSON.stringify(self.bangluong));
        },
        changeKyLamViec: function (type) {
            var self = this;
            if (self.bangluong.TuNgayYYMMDD !== null && self.bangluong.DenNgayYYMMDD !== null) {
                self.tinhLuong({ TuNgay: self.bangluong.TuNgayYYMMDD, DenNgay: self.bangluong.DenNgayYYMMDD });
            }
        },

        SaveSalary: function (status) {
            var self = this;
            // set default some value for bangluongct
            // ThueTNCN, MienGiamThue, BaoHiem, KyLuat, BaoHiemCongTyDong, DoanhSo
            var bangluongDTO = {
                ID: self.bangluong.ID,
                ID_DonVi: self.idDonVi,
                ID_NhanVienDuyet: self.bangluong.ID_NhanVienDuyet,
                MaBangLuong: self.bangluong.MaBangLuong,
                TenBangLuong: self.bangluong.TenBangLuong,
                TuNgay: self.bangluong.TuNgayYYMMDD,
                DenNgay: self.bangluong.DenNgayYYMMDD,
                NguoiTao: self.userLogin,
                TrangThai: status,
                GhiChu: self.bangluong.GhiChu,
            };

            for (let i = 0; i < self.bangluongchitiet.length; i++) {
                self.bangluongchitiet[i].TongTienPhat += self.bangluongchitiet[i].GiamTruCoDinh_TheoPTram;
            }

            var myData = {
                bangluong: bangluongDTO,
                bangluongchitiet: self.bangluongchitiet,
            }
            console.log(myData);
            self.loadding = true;
            self.ajaxHelper(self.urlAPINhanSu + "PostBangLuong", 'POST', myData).done(function (x) {
             
                if (x.res === true) {
                    commonStatisJs.ShowMessageSuccess('Thêm mới bảng lương thành công');

                    self.bangluongchitiet = [];
                    window.location.href = '#/payroll';
                    localStorage.removeItem(self.ctBasic);
                    localStorage.removeItem(self.ctSalary);
                }
                else {
                    if (x.mess === x.log) {
                        commonStatisJs.ShowMessageDanger(x.mess);
                    }
                }
            }).fail(function (x) {
             
            }).always(function (x) {
                self.loadding = false;
            });
        },
        GotoListBangLuong: function () {
            localStorage.removeItem(self.ctBasic);
            localStorage.removeItem(self.ctSalary);
            window.location.href = '#/payroll';
        },

        luongchinh_EditLuongCoBan: function (index, otca) {
            var self = this;
            let luongCB_new = self.formatNumberToFloat(otca.LuongCoBanQuyDoi);
            // only apply with luongca/gio
            for (let i = 0; i < self.listdata.LuongChinh.length; i++) {
                let forRoot = self.listdata.LuongChinh[i];
                // same ID_NhanVien
                if (forRoot.ID_NhanVien === otca.ID_NhanVien) {
                    for (let j = 0; j < forRoot.LstLuongChinh.length; j++) {
                        let forParent = forRoot.LstLuongChinh[j];
                        if (forParent.LoaiLuong === otca.LoaiLuong) {
                            for (let m = 0; m < forParent.ListCa.length; m++) {
                                if (forParent.ListCa[m].ID_CaLamViec === otca.ID_CaLamViec) {
                                    for (var k = 0; k < forParent.ListCa[m].LstDetail.length; k++) {
                                        let itFor = forParent.ListCa[m].LstDetail[k];
                                        if (index === k) {
                                            // tinhlai hesoluong
                                            if (itFor.LaPhanTram) {
                                                self.listdata.LuongChinh[i].LstLuongChinh[j].ListCa[m].LstDetail[k].HeSoLuong = commonStatisJs.roundDecimal(luongCB_new / self.formatNumberToFloat(itFor.LuongCoBan) * 100);
                                            }
                                            else {
                                                self.listdata.LuongChinh[i].LstLuongChinh[j].ListCa[m].LstDetail[k].HeSoLuong = luongCB_new;
                                            }
                                            self.listdata.LuongChinh[i].LstLuongChinh[j].ListCa[m].LstDetail[k].LuongCoBanQuyDoi = self.formatNumber3Digit(luongCB_new);
                                            self.listdata.LuongChinh[i].LstLuongChinh[j].ListCa[m].LstDetail[k].ThanhTien = luongCB_new * self.formatNumberToFloat(itFor.SoCaApDung);
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        },
        luongchinh_EditSoGioApDung: function (index, otca) {
            var self = this;
            var sogioNew = self.formatNumberToFloat(otca.SoCaApDung);

            for (let i = 0; i < self.listdata.LuongChinh.length; i++) {
                let forRoot = self.listdata.LuongChinh[i];
                // same ID_NhanVien
                if (forRoot.ID_NhanVien === otca.ID_NhanVien) {
                    for (let j = 0; j < forRoot.LstLuongChinh.length; j++) {
                        let forParent = forRoot.LstLuongChinh[j];
                        if (forParent.LoaiLuong === otca.LoaiLuong) {
                            for (let m = 0; m < forParent.ListCa.length; m++) {
                                if (forParent.ListCa[m].ID_CaLamViec === otca.ID_CaLamViec) {
                                    for (var k = 0; k < forParent.ListCa[m].LstDetail.length; k++) {
                                        let itFor = forParent.ListCa[m].LstDetail[k];
                                        if (index === k) {
                                            let luongCoBanQD = self.formatNumberToFloat(itFor.LuongCoBanQuyDoi);
                                            switch (otca.LoaiLuong) {
                                                case 1:
                                                    break;
                                                case 2:
                                                    luongCoBanQD = self.formatNumberToFloat(itFor.LuongCoBan) / itFor.NgayCongChuan;
                                                    break;
                                            }
                                            self.listdata.LuongChinh[i].LstLuongChinh[j].ListCa[m].LstDetail[k].ThanhTien = luongCoBanQD * sogioNew;
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        },
        luongchinh_AgreeChange: function () {
            var self = this;
            var ctluong = localStorage.getItem(self.ctSalary);
            if (ctluong !== null) {
                ctluong = JSON.parse(ctluong);
                for (let m = 0; m < ctluong.length; m++) {
                    for (let n = 0; n < self.listdata.LuongChinh.length; n++) {
                        if (ctluong[m].ID_NhanVien === self.listdata.LuongChinh[n].ID_NhanVien) {
                            ctluong[m].ListLuongOT = self.listdata.LuongChinh[n];
                            // update luongchinh in ctluong nhanvien
                            let luongchinh = 0;
                            let forRoot = self.listdata.LuongChinh[n];
                            for (let j = 0; j < forRoot.LstLuongChinh.length; j++) {
                                let forParent = forRoot.LstLuongChinh[j];
                                for (var i = 0; i < forParent.ListCa.length; i++) {
                                    for (let k = 0; k < forParent.ListCa[i].LstDetail.length; k++) {
                                        let itFor = forParent.ListCa[i].LstDetail[k];
                                        luongchinh += self.formatNumberToFloat(itFor.ThanhTien);
                                    }
                                }
                            }
                            ctluong[m].LuongChinh = luongchinh;
                            ctluong[m].LuongThucNhan = luongchinh + ctluong[m].LuongOT + ctluong[m].PhuCap + ctluong[m].KhenThuong - ctluong[m].TongTienPhat;
                            break;
                        }
                    }
                }

                self.bangluongchitiet = ctluong;
                localStorage.setItem(self.ctSalary, JSON.stringify(ctluong));
            }
            self.updateInforLuong();
            $('#luongchinh_modal').modal('hide');
        },

        ShowDiv_HeSoLuong: function (indexCT, otca, event) {
            var self = this;
            var $this = $(event.target).position();
            var $tr = $(event.target).closest('.tbl-row').position();
            var $td = $(event.target).closest('div').position();
            var $tbl = $(event.target).closest('.forRoot');
            var $tbframe = $($tbl).closest('.table-frame'); // neu chon nhieu nv
            var heightScrool = $tbframe.scrollTop();
            console.log('tr', $tr, 'td', $td, 'this ', $this, heightScrool)

            self.luongot.index = indexCT;
            self.luongot.chitiet = otca;
            self.luongot.LaPhanTram = otca.LaPhanTram;
            self.luongot.applyAllCa = false;
            self.luongot.applyAllNVien = false;

            if (otca.LaPhanTram) {
                self.luongot.hesoluong = otca.HeSoLuong;
            }
            else {
                self.luongot.hesoluong = self.formatNumber3Digit(otca.HeSoLuong);
            }
            let allheight = 0;
            var tblmanyNV = $tbframe.siblings('.table-frame').length;
            if (tblmanyNV > 1) {
                let frame = $('#lamthem_modal .modal-body').children('.table-frame');
                // get height table OT mỗi nhân viên
                for (let i = 0; i < $tbframe.index(); i++) {
                    allheight += $(frame).eq(i).height();
                }
            }

            if ($tbl.index() === 1) {
                $('.change-hesoluong').css('top', ($tr.top + $td.top + $this.top + allheight + 130 - heightScrool) + 'px'); // 130: heigth of ...?? không nhớ
            }
            else {
                let frame = $('#lamthem_modal .table-frame').eq($tbframe.index()).children();
                // get height mỗi row (theo ngay, theo ca) OT 
                for (let i = 1; i < $tbl.index(); i++) {
                    allheight += $(frame).eq(i).height();
                }
                $('.change-hesoluong').css('top', ($tr.top + $td.top + $this.top + allheight + 130 - heightScrool) + 'px');
            }
            $('.change-hesoluong').css('left', ($tr.left + $td.left + $this.left) + 'px');

            $('.change-hesoluong').show();
        },
        thietLapOT_ClickVND: function () {
            var self = this;
            var ctDoing = self.luongot.chitiet;
            self.luongot.hesoluong = self.formatNumber3Digit(self.formatNumberToFloat(ctDoing.Luong1GioCongCoBan) * self.formatNumber3Digit(self.luongot.hesoluong) / 100);
            self.luongot.LaPhanTram = false;

            for (let i = 0; i < self.listdata.LuongOT.length; i++) {
                let forRoot = self.listdata.LuongOT[i];
                // same ID_NhanVien
                if (forRoot.ID_NhanVien === ctDoing.ID_NhanVien) {
                    for (let j = 0; j < forRoot.LstLuongOT.length; j++) {
                        let forParent = forRoot.LstLuongOT[j];
                        if (forParent.LoaiLuong === ctDoing.LoaiLuong) {
                            for (let m = 0; m < forParent.ListCa.length; k++) {
                                if (forParent.ListCa[m].ID_CaLamViec === ctDoing.ID_CaLamViec) {
                                    for (var k = 0; k < forParent.ListCa[m].LstDetail.length; k++) {
                                        let itFor = forParent.ListCa[m].LstDetail[k];
                                        if (self.luongot.index === k) {
                                            self.listdata.LuongOT[i].LstLuongOT[j].ListCa[m].LstDetail[k].HeSoLuong = self.formatNumberToFloat(itFor.Luong1GioCongQuyDoi);
                                            self.listdata.LuongOT[i].LstLuongOT[j].ListCa[m].LstDetail[k].LaPhanTram = 0;
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        },
        thietLapOT_ClickPtram: function () {
            var self = this;
            var ctDoing = self.luongot.chitiet;
            self.luongot.hesoluong = commonStatisJs.roundDecimal(self.formatNumberToFloat(ctDoing.Luong1GioCongQuyDoi) / self.formatNumberToFloat(ctDoing.Luong1GioCongCoBan) * 100);
            self.luongot.LaPhanTram = true;

            for (let i = 0; i < self.listdata.LuongOT.length; i++) {
                let forRoot = self.listdata.LuongOT[i];
                // same ID_NhanVien
                if (forRoot.ID_NhanVien === ctDoing.ID_NhanVien) {
                    for (let j = 0; j < forRoot.LstLuongOT.length; j++) {
                        let forParent = forRoot.LstLuongOT[j];
                        if (forParent.LoaiLuong === ctDoing.LoaiLuong) {
                            for (let m = 0; m < forParent.ListCa.length; k++) {
                                if (forParent.ListCa[m].ID_CaLamViec === ctDoing.ID_CaLamViec) {
                                    for (var k = 0; k < forParent.ListCa[m].LstDetail.length; k++) {
                                        let itFor = forParent.ListCa[m].LstDetail[k];
                                        if (self.luongot.index === k) {
                                            self.listdata.LuongOT[i].LstLuongOT[j].ListCa[m].LstDetail[k].HeSoLuong = self.luongot.hesoluong;
                                            self.listdata.LuongOT[i].LstLuongOT[j].ListCa[m].LstDetail[k].LaPhanTram = 1;
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        },
        luongot_EditHeSoLuong: function () {
            var self = this;
            let hesoluong = self.formatNumberToFloat(self.luongot.hesoluong);
            var luong1gioQD = hesoluong;
            if (self.luongot.LaPhanTram) {
                luong1gioQD = self.formatNumberToFloat(self.luongot.chitiet.Luong1GioCongCoBan) * hesoluong / 100;
            }
            self.luongot.hesoluong = self.formatNumber3Digit(hesoluong);

            for (let i = 0; i < self.listdata.LuongOT.length; i++) {
                let forRoot = self.listdata.LuongOT[i];
                // same ID_NhanVien
                if (forRoot.ID_NhanVien === self.luongot.chitiet.ID_NhanVien) {
                    for (let j = 0; j < forRoot.LstLuongOT.length; j++) {
                        let forParent = forRoot.LstLuongOT[j];
                        if (forParent.LoaiLuong === self.luongot.chitiet.LoaiLuong) {
                            for (let m = 0; m < forParent.ListCa.length; m++) {
                                if (forParent.ListCa[m].ID_CaLamViec === self.luongot.chitiet.ID_CaLamViec) {
                                    for (var k = 0; k < forParent.ListCa[m].LstDetail.length; k++) {
                                        let itFor = forParent.ListCa[m].LstDetail[k];
                                        if (self.luongot.index === k) {
                                            if (itFor.LaPhanTram) {
                                                self.listdata.LuongOT[i].LstLuongOT[j].ListCa[m].LstDetail[k].HeSoLuong = self.luongot.hesoluong;
                                            }
                                            else {
                                                self.listdata.LuongOT[i].LstLuongOT[j].ListCa[m].LstDetail[k].HeSoLuong = luong1gioQD;
                                            }
                                            self.listdata.LuongOT[i].LstLuongOT[j].ListCa[m].LstDetail[k].Luong1GioCongQuyDoi = self.formatNumber3Digit(luong1gioQD);
                                            self.listdata.LuongOT[i].LstLuongOT[j].ListCa[m].LstDetail[k].ThanhTien = self.formatNumber3Digit(luong1gioQD * self.formatNumberToFloat(itFor.SoGioOTApDung));
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        },
        luongot_EditLuongCoBan: function (index, otca) {
            var self = this;
            let luongCB_new = self.formatNumberToFloat(otca.Luong1GioCongQuyDoi.toString().replace(/,/g, ""));

            for (let i = 0; i < self.listdata.LuongOT.length; i++) {
                let forRoot = self.listdata.LuongOT[i];
                // same ID_NhanVien
                if (forRoot.ID_NhanVien === otca.ID_NhanVien) {
                    for (let j = 0; j < forRoot.LstLuongOT.length; j++) {
                        let forParent = forRoot.LstLuongOT[j];
                        if (forParent.LoaiLuong === otca.LoaiLuong) {
                            for (let m = 0; m < forParent.ListCa.length; m++) {
                                if (forParent.ListCa[m].ID_CaLamViec === otca.ID_CaLamViec) {
                                    for (var k = 0; k < forParent.ListCa[m].LstDetail.length; k++) {
                                        let itFor = forParent.ListCa[m].LstDetail[k];
                                        if (index === k) {
                                            // tinhlai hesoluong
                                            if (itFor.LaPhanTram) {
                                                self.listdata.LuongOT[i].LstLuongOT[j].ListCa[m].LstDetail[k].HeSoLuong = commonStatisJs.roundDecimal(luongCB_new / self.formatNumberToFloat(itFor.Luong1GioCongCoBan) * 100);
                                            }
                                            else {
                                                self.listdata.LuongOT[i].LstLuongOT[j].ListCa[m].LstDetail[k].HeSoLuong = luongCB_new;
                                            }
                                            self.listdata.LuongOT[i].LstLuongOT[j].ListCa[m].LstDetail[k].Luong1GioCongQuyDoi = self.formatNumber3Digit(luongCB_new);
                                            self.listdata.LuongOT[i].LstLuongOT[j].ListCa[m].LstDetail[k].ThanhTien = self.formatNumber3Digit(luongCB_new * self.formatNumberToFloat(itFor.SoGioOTApDung));
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        },
        luongot_EditSoGioApDung: function (index, otca) {
            var self = this;
            var sogioNew = self.formatNumberToFloat(otca.SoGioOTApDung);

            for (let i = 0; i < self.listdata.LuongOT.length; i++) {
                let forRoot = self.listdata.LuongOT[i];
                // same ID_NhanVien
                if (forRoot.ID_NhanVien === otca.ID_NhanVien) {
                    for (let j = 0; j < forRoot.LstLuongOT.length; j++) {
                        let forParent = forRoot.LstLuongOT[j];
                        if (forParent.LoaiLuong === otca.LoaiLuong) {
                            for (let m = 0; m < forParent.ListCa.length; m++) {
                                if (forParent.ListCa[m].ID_CaLamViec === otca.ID_CaLamViec) {
                                    for (var k = 0; k < forParent.ListCa[m].LstDetail.length; k++) {
                                        let itFor = forParent.ListCa[m].LstDetail[k];
                                        if (index === k) {
                                            let luong1gioQD = self.formatNumberToFloat(itFor.Luong1GioCongQuyDoi);
                                            self.listdata.LuongOT[i].LstLuongOT[j].ListCa[m].LstDetail[k].ThanhTien = self.formatNumber3Digit(luong1gioQD * sogioNew);
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        },
        AgreeChangeLuongOT: function () {
            var self = this;
            var ctluong = localStorage.getItem(self.ctSalary);
            if (ctluong !== null) {
                ctluong = JSON.parse(ctluong);
                for (let m = 0; m < ctluong.length; m++) {
                    for (let n = 0; n < self.listdata.LuongOT.length; n++) {
                        if (ctluong[m].ID_NhanVien === self.listdata.LuongOT[n].ID_NhanVien) {
                            ctluong[m].ListLuongOT = self.listdata.LuongOT[n];

                            // update luongot in ctluong nhanvien
                            let luongot = 0;
                            let forRoot = self.listdata.LuongOT[n];
                            for (let j = 0; j < forRoot.LstLuongOT.length; j++) {
                                let forParent = forRoot.LstLuongOT[j];
                                for (var i = 0; i < forParent.ListCa.length; i++) {
                                    for (let k = 0; k < forParent.ListCa[i].LstDetail.length; k++) {
                                        let itFor = forParent.ListCa[i].LstDetail[k];
                                        luongot += self.formatNumberToFloat(itFor.ThanhTien);
                                    }
                                }
                            }
                            ctluong[m].LuongOT = luongot;
                            ctluong[m].LuongThucNhan = ctluong[m].LuongChinh + luongot + ctluong[m].PhuCap + ctluong[m].KhenThuong - ctluong[m].TongTienPhat;
                            break;
                        }
                    }
                }

                self.bangluongchitiet = ctluong;
                localStorage.setItem(self.ctSalary, JSON.stringify(ctluong));
            }
            self.updateInforLuong();
            $('#lamthem_modal').modal('hide');
        },
        Agree_HeSoLuong: function () {
            var self = this;

            if (self.luongot.applyAllNVien) {
                for (let i = 0; i < self.listdata.LuongOT.length; i++) {
                    let forRoot = self.listdata.LuongOT[i];
                    for (let j = 0; j < forRoot.LstLuongOT.length; j++) {
                        let forParent = forRoot.LstLuongOT[j];
                        for (let m = 0; m < forParent.ListCa.length; m++) {
                            for (var k = 0; k < forParent.ListCa[m].LstDetail.length; k++) {
                                let itFor = forParent.ListCa[m].LstDetail[k];
                                // chi apdung same loaingay (thuong, thu7, chunhat, nghi, le)
                                if (itFor.LoaiNgayThuong_Nghi === self.luongot.chitiet.LoaiNgayThuong_Nghi) {
                                    let luong1gioQD = 0;
                                    if (self.luongot.chitiet.LaPhanTram) {
                                        luong1gioQD = self.formatNumberToFloat(itFor.Luong1GioCongCoBan) * self.formatNumberToFloat(self.luongot.hesoluong) / 100;
                                        self.listdata.LuongOT[i].LstLuongOT[j].ListCa[m].LstDetail[k].HeSoLuong = self.luongot.hesoluong;
                                    }
                                    else {
                                        luong1gioQD = self.formatNumberToFloat(self.luongot.hesoluong);
                                        self.listdata.LuongOT[i].LstLuongOT[j].ListCa[m].LstDetail[k].HeSoLuong = luong1gioQD;
                                    }
                                    self.listdata.LuongOT[i].LstLuongOT[j].ListCa[m].LstDetail[k].LaPhanTram = self.luongot.LaPhanTram;
                                    self.listdata.LuongOT[i].LstLuongOT[j].ListCa[m].LstDetail[k].Luong1GioCongQuyDoi = self.formatNumber3Digit(luong1GioQuyDoi);
                                    self.listdata.LuongOT[i].LstLuongOT[j].ListCa[m].LstDetail[k].ThanhTien = self.formatNumber3Digit(luong1gioQD * self.formatNumberToFloat(itFor.SoGioOTApDung));
                                }
                            }
                        }
                    }
                }
            }
            else {
                // chi ap dung cho 1 nhan vien
                if (self.luongot.applyAllCa) {
                    for (let i = 0; i < self.listdata.LuongOT.length; i++) {
                        let forRoot = self.listdata.LuongOT[i];
                        // same ID_NhanVien
                        if (forRoot.ID_NhanVien === self.luongot.chitiet.ID_NhanVien) {
                            for (let j = 0; j < forRoot.LstLuongOT.length; j++) {
                                let forParent = forRoot.LstLuongOT[j];
                                for (let m = 0; m < forParent.ListCa.length; m++) {
                                    for (var k = 0; k < forParent.ListCa[m].LstDetail.length; k++) {
                                        let itFor = forParent.ListCa[m].LstDetail[k];
                                        //same (thu7/cn/nghi/le)
                                        if (itFor.LoaiNgayThuong_Nghi === self.luongot.chitiet.LoaiNgayThuong_Nghi) {
                                            let luong1gioQD = 0;
                                            if (self.luongot.chitiet.LaPhanTram) {
                                                luong1gioQD = self.formatNumberToFloat(itFor.Luong1GioCongCoBan) * self.formatNumberToFloat(self.luongot.hesoluong) / 100;
                                                self.listdata.LuongOT[i].LstLuongOT[j].ListCa[m].LstDetail[k].HeSoLuong = self.luongot.hesoluong;
                                            }
                                            else {
                                                luong1gioQD = self.formatNumberToFloat(self.luongot.hesoluong);
                                                self.listdata.LuongOT[i].LstLuongOT[j].ListCa[m].LstDetail[k].HeSoLuong = luong1gioQD;
                                            }
                                            self.listdata.LuongOT[i].LstLuongOT[j].ListCa[m].LstDetail[k].LaPhanTram = self.luongot.LaPhanTram;
                                            self.listdata.LuongOT[i].LstLuongOT[j].ListCa[m].LstDetail[k].Luong1GioCongQuyDoi = self.formatNumber3Digit(luong1gioQD);
                                            self.listdata.LuongOT[i].LstLuongOT[j].ListCa[m].LstDetail[k].ThanhTien = self.formatNumber3Digit(luong1gioQD * self.formatNumberToFloat(itFor.SoGioOTApDung));
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
                else {
                    // chi apdung cho ban ghi hientai
                }
            }
            $('.change-hesoluong').hide();
        },

        // not sue
        phucap_EditLuongCoBan: function (index, pc) {
            var self = this;
            var luongCB_new = 0;
            console.log('loai ', pc.LoaiPhuCap)
            switch (pc.LoaiPhuCap) {
                case 51:// theo ngay
                    luongCB_new = self.formatNumberToFloat(pc.PhuCapTheoNgay.toString().replace(/,/g, ""));
                    break;
                case 52:// codinh vnd
                case 53:
                    luongCB_new = self.formatNumberToFloat(pc.PhuCapCoDinh.toString().replace(/,/g, ""));
                    break;
            }

            for (let i = 0; i < self.listdata.PhuCap.length; i++) {
                let forRoot = self.listdata.PhuCap[i];
                // same ID_NhanVien
                if (forRoot.ID_NhanVien === pc.ID_NhanVien) {
                    for (let j = 0; j < forRoot.LstLoaiPhuCap.length; j++) {
                        let forParent = forRoot.LstLoaiPhuCap[j];
                        if (forParent.LoaiPhuCap === pc.LoaiPhuCap) {
                            for (let k = 0; k < forParent.LstDetail.length; k++) {
                                let itFor = forParent.LstDetail[k];
                                if (index === k) {
                                    switch (pc.LoaiPhuCap) {
                                        case 51:// theo ngay cong
                                            self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].ThanhTienPC_TheoNgay = self.formatNumber3Digit(luongCB_new * itFor.SoNgayDiLam);
                                            self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].PhuCapTheoNgay = self.formatNumber3Digit(luongCB_new);
                                            break;
                                        case 52: // theo vnd
                                            self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].ThanhTien = self.formatNumber3Digit(luongCB_new);
                                            self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].PhuCapCoDinh = self.formatNumber3Digit(luongCB_new);
                                            break;
                                        case 53:// theo % luong
                                            // find luongchinh of nv
                                            var luongct = $.grep(self.bangluongchitiet, function (x) {
                                                return x.ID_NhanVien === pc.ID_NhanVien;
                                            });
                                            self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].PhuCapCoDinh = luongCB_new;
                                            self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].ThanhTien = luongCB_new * luongct[0].LuongChinh / 100;
                                            break;
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        },
        phucap_EditMucPhuCap: function () {
            var self = this;
            var thanhtien = 0;
            var gtri = self.formatNumberToFloat(self.phucap.giatri.toString().replace(/,/g, ""));
            let luongct = $.grep(self.bangluongchitiet, function (x) {
                return x.ID_NhanVien === self.phucap.chitiet.ID_NhanVien;
            });
            switch (self.phucap.chitiet.LoaiPhuCap) {
                case 51:
                    thanhtien = self.formatNumber3Digit(gtri * self.phucap.chitiet.SoNgayDiLam);
                    break;
                case 52:// codinh vnd
                    thanhtien = self.formatNumber3Digit(gtri);
                    break;
                case 53:
                    if (gtri > 100) {
                        self.phucap.giatri = 100;
                    }
                    thanhtien = self.formatNumber3Digit(self.phucap.giatri * luongct[0].LuongChinh / 100);
                    break;
            }
            self.phucap.giatri = self.formatNumber3Digit(gtri);
            for (let i = 0; i < self.listdata.PhuCap.length; i++) {
                let forRoot = self.listdata.PhuCap[i];
                // same ID_NhanVien
                if (forRoot.ID_NhanVien === self.phucap.chitiet.ID_NhanVien) {
                    for (let j = 0; j < forRoot.LstLoaiPhuCap.length; j++) {
                        let forParent = forRoot.LstLoaiPhuCap[j];
                        if (forParent.LoaiPhuCap === self.phucap.chitiet.LoaiPhuCap) {
                            for (let k = 0; k < forParent.LstDetail.length; k++) {
                                let itFor = forParent.LstDetail[k];
                                if (k === self.phucap.chitiet.index) {
                                    switch (self.phucap.chitiet.LoaiPhuCap) {
                                        case 51:
                                            self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].PhuCapTheoNgay = self.formatNumber3Digit(gtri);
                                            self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].ThanhTienPC_TheoNgay = thanhtien;
                                            break;
                                        case 52:
                                            self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].PhuCapCoDinh = self.formatNumber3Digit(gtri);
                                            self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].ThanhTien = thanhtien;
                                            break;
                                        case 53:
                                            self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].PhuCapCoDinh = gtri;
                                            self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].ThanhTien = thanhtien;
                                            break;
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        },
        showdiv_PhuCapLuong: function (indexCT, pc, event) {
            var self = this;
            pc.index = indexCT;
            self.phucap.applyAllNVien = false;
            self.phucap.chitiet = pc;
            var gtri = 0;
            switch (pc.LoaiPhuCap) {
                case 51:
                    self.phucap.LaPhuCapTheoNgay = true;
                    self.phucap.isPTramLuong = false;
                    gtri = self.formatNumber3Digit(pc.PhuCapTheoNgay);
                    break;
                case 52:
                    self.phucap.LaPhuCapTheoNgay = false;
                    self.phucap.isPTramLuong = false;
                    gtri = self.formatNumber3Digit(pc.PhuCapCoDinh);
                    break;
                case 53:
                    self.phucap.LaPhuCapTheoNgay = false;
                    self.phucap.isPTramLuong = true;
                    gtri = pc.PhuCapCoDinh;
                    break;
            }
            self.phucap.giatri = gtri;

            var $this = $(event.target).position();
            var $tr = $(event.target).closest('.tbl-row').position();
            var $td = $(event.target).closest('div').position();
            var $tbl = $(event.target).closest('.tbl-row').parent().parent();
            var $tbframe = $($tbl).closest('.table-frame'); // neu chon nhieu nv
            //console.log('tr', $tr, 'td', $td, 'this ', $this)

            $('.change-phucap').css('top', ($tr.top + $td.top + 65) + 'px'); // 50: height of header + 1 row
            $('.change-phucap').css('left', ($tbframe.width() * 0.35 + 45) + 'px');
            $(function () {
                $('.change-phucap > input').focus();
            })
            $('.change-phucap').show();
        },
        OnOff_PhuCapTheoNgay: function () {
            var self = this;
            var gtriphucap = self.formatNumberToFloat(self.phucap.giatri);
            self.phucap.isPTramLuong = false;

            for (let i = 0; i < self.listdata.PhuCap.length; i++) {
                let forRoot = self.listdata.PhuCap[i];
                if (forRoot.ID_NhanVien === self.phucap.chitiet.ID_NhanVien) {
                    for (let j = 0; j < forRoot.LstLoaiPhuCap.length; j++) {
                        let forParent = forRoot.LstLoaiPhuCap[j];
                        if (forParent.LoaiPhuCap === self.phucap.chitiet.LoaiPhuCap) {
                            for (let k = 0; k < forParent.LstDetail.length; k++) {
                                let itFor = forParent.LstDetail[k];
                                if (k === self.phucap.chitiet.index) {
                                    if (self.phucap.LaPhuCapTheoNgay) {
                                        self.phucap.chitiet.LoaiPhuCap = 51;
                                        self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].PhuCapCoDinh = 0;
                                        self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].ThanhTien = 0;
                                        self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].PhuCapTheoNgay = self.phucap.giatri;
                                        self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].ThanhTienPC_TheoNgay = self.formatNumber3Digit(itFor.SoNgayDiLam * gtriphucap);
                                    }
                                    else {
                                        self.phucap.chitiet.LoaiPhuCap = 52;
                                        self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].PhuCapCoDinh = self.phucap.giatri;
                                        self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].ThanhTien = self.phucap.giatri;
                                        self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].PhuCapTheoNgay = 0;
                                        self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].ThanhTienPC_TheoNgay = 0;
                                    }
                                    self.listdata.PhuCap[i].LstLoaiPhuCap[j].LoaiPhuCap = self.phucap.chitiet.LoaiPhuCap;
                                    self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].LoaiPhuCap = self.phucap.chitiet.LoaiPhuCap;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        },
        phucap_PtramOrVND: function () {
            var self = this;
            self.phucap.LaPhuCapTheoNgay = false;

            var gtriold = self.formatNumberToFloat(self.phucap.giatri);
            var gtrinew = 0;
            var luongct = $.grep(self.bangluongchitiet, function (x) {
                return x.ID_NhanVien === self.phucap.chitiet.ID_NhanVien;
            });

            self.phucap.isPTramLuong = !self.phucap.isPTramLuong;
            if (self.phucap.isPTramLuong) {
                gtrinew = commonStatisJs.roundDecimal(self.formatNumberToFloat(gtriold) / luongct[0].LuongChinh * 100);
            }
            else {
                gtrinew = self.formatNumber3Digit(self.formatNumberToFloat(gtriold) * luongct[0].LuongChinh / 100);
            }
            self.phucap.giatri = gtrinew;

            for (let i = 0; i < self.listdata.PhuCap.length; i++) {
                let forRoot = self.listdata.PhuCap[i];
                // same ID_NhanVien
                if (forRoot.ID_NhanVien === self.phucap.chitiet.ID_NhanVien) {
                    for (let j = 0; j < forRoot.LstLoaiPhuCap.length; j++) {
                        let forParent = forRoot.LstLoaiPhuCap[j];
                        if (forParent.LoaiPhuCap === self.phucap.chitiet.LoaiPhuCap) {
                            for (let k = 0; k < forParent.LstDetail.length; k++) {
                                let itFor = forParent.LstDetail[k];
                                if (k === self.phucap.chitiet.index) {
                                    let thanhtien = 0;
                                    if (self.phucap.isPTramLuong) {
                                        thanhtien = self.formatNumber3Digit(self.phucap.giatri * luongct[0].LuongChinh / 100);

                                        self.phucap.chitiet.LoaiPhuCap = 53;
                                        self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].PhuCapCoDinh = self.phucap.giatri;
                                    }
                                    else {
                                        thanhtien = self.phucap.giatri;
                                        self.phucap.chitiet.LoaiPhuCap = 52;
                                        self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].PhuCapCoDinh = thanhtien;
                                    }
                                    self.listdata.PhuCap[i].LstLoaiPhuCap[j].LoaiPhuCap = self.phucap.chitiet.LoaiPhuCap;
                                    self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].LoaiPhuCap = self.phucap.chitiet.LoaiPhuCap;
                                    self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].ThanhTien = thanhtien;
                                    self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].PhuCapTheoNgay = 0;
                                    self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].ThanhTienPC_TheoNgay = 0;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        },
        phucap_applyAllNhanVien: function () {
            var self = this;
            var gtriphucap = self.formatNumberToFloat(self.phucap.giatri);

            if (self.phucap.applyAllNVien) {
                for (let i = 0; i < self.listdata.PhuCap.length; i++) {
                    let forRoot = self.listdata.PhuCap[i];
                    for (let j = 0; j < forRoot.LstLoaiPhuCap.length; j++) {
                        let forParent = forRoot.LstLoaiPhuCap[j];
                        for (let k = 0; k < forParent.LstDetail.length; k++) {
                            let itFor = forParent.LstDetail[k];
                            for (let m = 0; m < self.bangluongchitiet.length; m++) {
                                let ctluong = self.bangluongchitiet[m];
                                if (itFor.ID_NhanVien === ctluong.ID_NhanVien) {
                                    switch (self.phucap.chitiet.LoaiPhuCap) {
                                        case 51:
                                            self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].PhuCapCoDinh = 0;
                                            self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].ThanhTien = 0;
                                            self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].PhuCapTheoNgay = self.phucap.giatri;
                                            self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].ThanhTienPC_TheoNgay = self.formatNumber3Digit(gtriphucap * itFor.SoNgayDiLam);
                                            break;
                                        case 52:
                                            self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].PhuCapCoDinh = self.phucap.giatri;
                                            self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].ThanhTien = self.phucap.giatri;
                                            self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].PhuCapTheoNgay = 0;
                                            self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].ThanhTienPC_TheoNgay = 0;
                                            break;
                                        case 53:
                                            self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].PhuCapCoDinh = gtriphucap;
                                            self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].ThanhTien = self.formatNumber3Digit(gtriphucap * ctluong.LuongChinh / 100);
                                            self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].PhuCapTheoNgay = 0;
                                            self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].ThanhTienPC_TheoNgay = 0;
                                            break;
                                    }
                                    self.listdata.PhuCap[i].LstLoaiPhuCap[j].LoaiPhuCap = self.phucap.chitiet.LoaiPhuCap;// set again for parent
                                    self.listdata.PhuCap[i].LstLoaiPhuCap[j].LstDetail[k].LoaiPhuCap = self.phucap.chitiet.LoaiPhuCap;// set for child
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            $('.change-phucap').hide();
        },
        phucap_AgreeChange: function () {
            var self = this;
            var ctluong = localStorage.getItem(self.ctSalary);
            if (ctluong !== null) {
                ctluong = JSON.parse(ctluong);
                for (let m = 0; m < ctluong.length; m++) {
                    for (let n = 0; n < self.listdata.PhuCap.length; n++) {
                        if (ctluong[m].ID_NhanVien === self.listdata.PhuCap[n].ID_NhanVien) {
                            ctluong[m].ListPhuCap = self.listdata.PhuCap[n];

                            // update phucap in ctluong nhanvien
                            let phucap = 0;
                            let forRoot = self.listdata.PhuCap[n];
                            for (let j = 0; j < forRoot.LstLoaiPhuCap.length; j++) {
                                let forParent = forRoot.LstLoaiPhuCap[j];
                                for (let k = 0; k < forParent.LstDetail.length; k++) {
                                    let itFor = forParent.LstDetail[k];
                                    phucap += self.formatNumberToFloat(itFor.ThanhTien) + self.formatNumberToFloat(itFor.ThanhTienPC_TheoNgay);
                                }
                            }

                            ctluong[m].PhuCap = phucap;
                            ctluong[m].LuongThucNhan = ctluong[m].LuongChinh + ctluong[m].LuongOT + ctluong[m].PhuCap + ctluong[m].KhenThuong - ctluong[m].TongTienPhat;
                            break;
                        }
                    }
                }

                self.bangluongchitiet = ctluong;
                localStorage.setItem(self.ctSalary, JSON.stringify(ctluong));
            }
            self.updateInforLuong();
            $('#phucap_modal').modal('hide');
        },
        // not use: giamtru_EditLuongCoBan
        giamtru_EditLuongCoBan: function (index, pc) {
            var self = this;
            var luongCB_new = 0;
            switch (pc.LoaiGiamTru) {
                case 1:// theo ngay
                    luongCB_new = self.formatNumberToFloat(pc.GiamTruTheoLan.toString().replace(/,/g, ""));
                    break;
                case 2:// codinh vnd
                case 3:
                    luongCB_new = self.formatNumberToFloat(pc.GiamTruCoDinh.toString().replace(/,/g, ""));
                    break;
            }

            for (let i = 0; i < self.listdata.GiamTru.length; i++) {
                let forRoot = self.listdata.GiamTru[i];
                // same ID_NhanVien
                if (forRoot.ID_NhanVien === pc.ID_NhanVien) {
                    for (let j = 0; j < forRoot.LstLoaiGiamTru.length; j++) {
                        let forParent = forRoot.LstLoaiGiamTru[j];
                        if (forParent.LoaiGiamTru === pc.LoaiGiamTru) {
                            for (let k = 0; k < forParent.LstDetail.length; k++) {
                                let itFor = forParent.LstDetail[k];
                                if (index === k) {
                                    switch (pc.LoaiGiamTru) {
                                        case 61:// theo ngay cong
                                            self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].ThanhTienGiamTruTheoLan = self.formatNumber3Digit(luongCB_new * itFor.SoLanDiMuon);
                                            self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].GiamTruTheoLan = self.formatNumber3Digit(luongCB_new);
                                            break;
                                        case 62: // theo vnd
                                            self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].ThanhTien = self.formatNumber3Digit(luongCB_new);
                                            self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].GiamTruCoDinh = self.formatNumber3Digit(luongCB_new);
                                            break;
                                        case 63:// theo % luong
                                            // find luongchinh of nv
                                            var luongct = $.grep(self.bangluongchitiet, function (x) {
                                                return x.ID_NhanVien === pc.ID_NhanVien;
                                            });
                                            self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].GiamTruCoDinh = luongCB_new;
                                            self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].ThanhTien = self.formatNumber3Digit(luongCB_new * luongct[0].TongLuongNhan / 100);
                                            break;
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        },
        giamtru_EditMucGiamTru: function () {
            var self = this;
            var thanhtien = 0;
            var gtri = self.formatNumberToFloat(self.giamtru.giatri.toString().replace(/,/g, ""));
            let luongct = $.grep(self.bangluongchitiet, function (x) {
                return x.ID_NhanVien === self.giamtru.chitiet.ID_NhanVien;
            });
            switch (self.giamtru.chitiet.LoaiGiamTru) {
                case 61:
                    thanhtien = self.formatNumber3Digit(gtri * self.giamtru.chitiet.SoLanDiMuon);
                    break;
                case 62:// codinh vnd
                    thanhtien = self.formatNumber3Digit(gtri);
                    break;
                case 63:
                    if (gtri > 100) {
                        self.giamtru.giatri = 100;
                    }
                    thanhtien = self.formatNumber3Digit(self.giamtru.giatri * luongct[0].TongLuongNhan / 100);
                    break;
            }
            self.giamtru.giatri = self.formatNumber3Digit(gtri);

            for (let i = 0; i < self.listdata.GiamTru.length; i++) {
                let forRoot = self.listdata.GiamTru[i];
                if (forRoot.ID_NhanVien === self.giamtru.chitiet.ID_NhanVien) {
                    for (let j = 0; j < forRoot.LstLoaiGiamTru.length; j++) {
                        let forParent = forRoot.LstLoaiGiamTru[j];
                        if (forParent.LoaiGiamTru === self.giamtru.chitiet.LoaiGiamTru) {
                            for (let k = 0; k < forParent.LstDetail.length; k++) {
                                let itFor = forParent.LstDetail[k];
                                if (k === self.giamtru.chitiet.index) {
                                    switch (self.giamtru.chitiet.LoaiGiamTru) {
                                        case 61:
                                            self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].GiamTruTheoLan = self.formatNumber3Digit(gtri);
                                            self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].ThanhTienGiamTruTheoLan = thanhtien;
                                            break;
                                        case 62:
                                            self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].GiamTruCoDinh = self.formatNumber3Digit(gtri);
                                            self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].ThanhTien = thanhtien;
                                            break;
                                        case 63:
                                            self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].GiamTruCoDinh = gtri;
                                            self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].ThanhTien = thanhtien;
                                            break;
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        },
        showdiv_GiamTruLuong: function (indexCT, pc, event) {
            var self = this;
            self.giamtru.giatri = self.formatNumber3Digit(pc.GiamTruCoDinh);
            self.giamtru.applyAllNVien = false;
            pc.index = indexCT;
            self.giamtru.chitiet = pc;
            switch (pc.LoaiGiamTru) {
                case 61:
                    self.giamtru.LaGiamTruTheoLan = true;
                    self.giamtru.isPTramLuong = false;
                    gtri = self.formatNumber3Digit(pc.GiamTruTheoLan);
                    break;
                case 62:
                    self.giamtru.LaGiamTruTheoLan = false;
                    self.giamtru.isPTramLuong = false;
                    gtri = self.formatNumber3Digit(pc.ThanhTien);
                    break;
                case 63:
                    self.giamtru.LaGiamTruTheoLan = false;
                    self.giamtru.isPTramLuong = true;
                    gtri = self.formatNumber3Digit(pc.GiamTruCoDinh);
                    break;
            }
            self.giamtru.giatri = gtri;

            var $this = $(event.target).position();
            var $tr = $(event.target).closest('.tbl-row').position();
            var $td = $(event.target).closest('div').position();
            var $tbl = $(event.target).closest('.tbl-row').parent().parent();
            var $tbframe = $($tbl).closest('.table-frame'); // neu chon nhieu nv
            //console.log('tr', $tr, 'td', $td, 'this ', $this)

            $('.change-giamtru').css('top', ($tr.top + $td.top + 60) + 'px'); // 50: height of header + 1 row
            $('.change-giamtru').css('left', ($tbframe.width() * 0.35 + 45) + 'px');
            $(function () {
                $('.change-giamtru > input').focus();
            });
            $('.change-giamtru').show();
        },
        OnOff_GiamTruTheoLan: function () {
            var self = this;
            var gtrigiamtru = self.formatNumberToFloat(self.giamtru.giatri);
            self.giamtru.isPTramLuong = false;

            for (let i = 0; i < self.listdata.GiamTru.length; i++) {
                let forRoot = self.listdata.GiamTru[i];
                if (forRoot.ID_NhanVien === self.giamtru.chitiet.ID_NhanVien) {
                    for (let j = 0; j < forRoot.LstLoaiGiamTru.length; j++) {
                        let forParent = forRoot.LstLoaiGiamTru[j];
                        if (forParent.LoaiGiamTru === self.giamtru.chitiet.LoaiGiamTru) {
                            for (let k = 0; k < forParent.LstDetail.length; k++) {
                                let itFor = forParent.LstDetail[k];
                                if (k === self.giamtru.chitiet.index) {
                                    if (self.giamtru.LaGiamTruTheoLan) {
                                        self.giamtru.chitiet.LoaiGiamTru = 61;
                                        self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].GiamTruCoDinh = 0;
                                        self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].ThanhTien = 0;
                                        self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].GiamTruTheoLan = self.giamtru.giatri;
                                        self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].ThanhTienGiamTruTheoLan = self.formatNumber3Digit(itFor.SoLanDiMuon * gtrigiamtru);
                                    }
                                    else {
                                        self.giamtru.chitiet.LoaiGiamTru = 62;
                                        self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].GiamTruCoDinh = self.giamtru.giatri;
                                        self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].ThanhTien = self.giamtru.giatri;
                                        self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].GiamTruTheoLan = 0;
                                        self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].ThanhTienGiamTruTheoLan = 0;
                                    }
                                    self.listdata.GiamTru[i].LstLoaiGiamTru[j].LoaiGiamTru = self.giamtru.chitiet.LoaiGiamTru;
                                    self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].LoaiGiamTru = self.giamtru.chitiet.LoaiGiamTru;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        },
        giamtru_PtramOrVND: function () {
            var self = this;
            self.giamtru.LaGiamTruTheoLan = false;
            self.giamtru.applyAllNVien = false;
            var gtriold = self.formatNumberToFloat(self.giamtru.giatri);
            var gtrinew = 0;

            var luongct = $.grep(self.bangluongchitiet, function (x) {
                return x.ID_NhanVien === self.giamtru.chitiet.ID_NhanVien;
            });

            self.giamtru.isPTramLuong = !self.giamtru.isPTramLuong;
            if (self.giamtru.isPTramLuong) {
                gtrinew = commonStatisJs.roundDecimal(self.formatNumberToFloat(gtriold) / luongct[0].TongLuongNhan * 100);
            }
            else {
                gtrinew = self.formatNumber3Digit(self.formatNumberToFloat(gtriold) * luongct[0].TongLuongNhan / 100);
            }
            self.giamtru.giatri = gtrinew;

            for (let i = 0; i < self.listdata.GiamTru.length; i++) {
                let forRoot = self.listdata.GiamTru[i];
                if (forRoot.ID_NhanVien === self.giamtru.chitiet.ID_NhanVien) {
                    for (let j = 0; j < forRoot.LstLoaiGiamTru.length; j++) {
                        let forParent = forRoot.LstLoaiGiamTru[j];
                        if (forParent.LoaiGiamTru === self.giamtru.chitiet.LoaiGiamTru) {
                            for (let k = 0; k < forParent.LstDetail.length; k++) {
                                let itFor = forParent.LstDetail[k];
                                if (k === self.giamtru.chitiet.index) {
                                    let thanhtien = 0;
                                    if (self.giamtru.isPTramLuong) {
                                        self.giamtru.chitiet.LoaiGiamTru = 63;
                                        thanhtien = self.formatNumber3Digit(self.giamtru.giatri * luongct[0].TongLuongNhan / 100);
                                        self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].GiamTruCoDinh = self.giamtru.giatri;
                                    }
                                    else {
                                        self.giamtru.chitiet.LoaiGiamTru = 62;
                                        thanhtien = self.giamtru.giatri;
                                        self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].GiamTruCoDinh = thanhtien;
                                    }
                                    self.listdata.GiamTru[i].LstLoaiGiamTru[j].LoaiGiamTru = self.giamtru.chitiet.LoaiGiamTru;// set again for parent
                                    self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].LoaiGiamTru = self.giamtru.chitiet.LoaiGiamTru;
                                    self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].ThanhTien = thanhtien;
                                    self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].GiamTruTheoLan = 0;
                                    self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].ThanhTienGiamTruTheoLan = 0;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        },
        giamtru_applyAllNhanVien: function () {
            var self = this;
            var gtrigiamtru = self.formatNumberToFloat(self.giamtru.giatri);

            if (self.giamtru.applyAllNVien) {
                for (let i = 0; i < self.listdata.GiamTru.length; i++) {
                    let forRoot = self.listdata.GiamTru[i];
                    for (let j = 0; j < forRoot.LstLoaiGiamTru.length; j++) {
                        let forParent = forRoot.LstLoaiGiamTru[j];
                        for (let k = 0; k < forParent.LstDetail.length; k++) {
                            let itFor = forParent.LstDetail[k];
                            for (let m = 0; m < self.bangluongchitiet.length; m++) {
                                let ctluong = self.bangluongchitiet[m];
                                if (itFor.ID_NhanVien === ctluong.ID_NhanVien) {
                                    switch (self.giamtru.chitiet.LoaiGiamTru) {
                                        case 61:
                                            self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].GiamTruCoDinh = 0;
                                            self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].ThanhTien = 0;
                                            self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].GiamTruTheoLan = self.giamtru.giatri;
                                            self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].ThanhTienGiamTruTheoLan = self.formatNumber3Digit(gtrigiamtru * itFor.SoLanDiMuon);
                                            break;
                                        case 62:
                                            self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].GiamTruCoDinh = self.giamtru.giatri;
                                            self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].ThanhTien = self.giamtru.giatri;
                                            self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].GiamTruTheoLan = 0;
                                            self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].ThanhTienGiamTruTheoLan = 0;
                                            break;
                                        case 63:
                                            self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].GiamTruCoDinh = gtrigiamtru;
                                            self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].ThanhTien = self.formatNumber3Digit(gtrigiamtru * ctluong.TongLuongNhan / 100);
                                            self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].GiamTruTheoLan = 0;
                                            self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].ThanhTienGiamTruTheoLan = 0;
                                            break;
                                    }
                                    self.listdata.GiamTru[i].LstLoaiGiamTru[j].LoaiGiamTru = self.giamtru.chitiet.LoaiGiamTru;// set again for parent
                                    self.listdata.GiamTru[i].LstLoaiGiamTru[j].LstDetail[k].LoaiGiamTru = self.giamtru.chitiet.LoaiGiamTru;// set for child
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            $('.change-giamtru').hide();
        },
        giamtru_AgreeChange: function () {
            var self = this;
            var ctluong = localStorage.getItem(self.ctSalary);
            if (ctluong !== null) {
                ctluong = JSON.parse(ctluong);
                for (let m = 0; m < ctluong.length; m++) {
                    for (let n = 0; n < self.listdata.GiamTru.length; n++) {
                        if (ctluong[m].ID_NhanVien === self.listdata.GiamTru[n].ID_NhanVien) {
                            ctluong[m].ListGiamTru = self.listdata.GiamTru[n];

                            // update giamtru in ctluong nhanvien
                            let giamtru = 0;
                            let forRoot = self.listdata.GiamTru[n];
                            for (let j = 0; j < forRoot.LstLoaiGiamTru.length; j++) {
                                let forParent = forRoot.LstLoaiGiamTru[j];
                                for (let k = 0; k < forParent.LstDetail.length; k++) {
                                    let itFor = forParent.LstDetail[k];
                                    giamtru += self.formatNumberToFloat(itFor.ThanhTien) + self.formatNumberToFloat(itFor.ThanhTienGiamTruTheoLan);
                                }
                                if (forRoot.LstLoaiGiamTru[j].LoaiGiamTru === 61 || forRoot.LstLoaiGiamTru[j].LoaiGiamTru === 62) {
                                    ctluong[m].GiamTruCoDinh_TheoPTram = 0;
                                }
                            }

                            ctluong[m].TongTienPhat = giamtru;
                            ctluong[m].LuongThucNhan = ctluong[m].LuongChinh + ctluong[m].LuongOT + ctluong[m].PhuCap + ctluong[m].KhenThuong - ctluong[m].TongTienPhat;
                            break;
                        }
                    }
                }

                self.bangluongchitiet = ctluong;
                localStorage.setItem(self.ctSalary, JSON.stringify(ctluong));
            }
            self.updateInforLuong();
            $('#giamtru_modal').modal('hide');
        },
    },
    computed: {
        fValue: {
            // getter
            get: function () {
                return this.value;
            },
            // setter
            set: function (newValue) {
                this.value = newValue.toString().replace(/,/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            }
        },
    }
});
vmSalaryDetail.getNhanVien_coBangLuong();

$(function () {
    $('.date-form').datetimepicker(
        {
            format: "d/m/Y",
            defaultDate: new Date(),
            mask: true,
            maxDate: new Date(),
            timepicker: false,
            onChangeDateTime: function (dp, $input, a) {
                let dtChange = moment(dp).format('DD/MM/YYYY');
                let dtYYMMDD = moment(dp).format('YYYY-MM-DD');

                if (parseInt($($input).attr('data-id')) === 1) {
                    vmSalaryDetail.bangluong.TuNgay = dtChange;
                    vmSalaryDetail.bangluong.TuNgayYYMMDD = dtYYMMDD;
                }
                else {
                    vmSalaryDetail.bangluong.DenNgay = dtChange;
                    vmSalaryDetail.bangluong.DenNgayYYMMDD = dtYYMMDD;
                }
                vmSalaryDetail.changeKyLamViec();
            },
        });
});

$(document).mouseup(function () {
    $('.change-hesoluong, .change-phucap, .change-giamtru').mouseup(function () {
        return false
    });
    $('.change-hesoluong, .change-phucap,.change-giamtru').hide();
});

