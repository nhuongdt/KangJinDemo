var vmBangLuong = new Vue({
    el: '#BangLuongNhanSu',
    data: {
        isNew: true,
        loadding: false,
        API_NhanSu: 'api/DanhMuc/NS_NhanSuAPI/',
        UserLogin: $('#txtTenTaiKhoan').text().trim(),
        databind: {
            pageview: '',
            pagenow: 1,
            data: [],
            listpage: [],
            isprev: false,
            isnext: false,
            countpage: 0,

            PhaiThanhToan: 0,
            TongDaTra: 0,
            TongConLai: 0,
        },
        datachitiet: {
            pageview: '',
            pagenow: 1,
            page: 1,
            data: [],
            listpage: [],
            isprev: false,
            isnext: false,
            countpage: 0,
            idbangluong: null,

            TongLuongChinh: 0,
            TongLuongOT: 0,
            TongPhuCapCoBan: 0,
            TongPhuCapKhac: 0,
            TongKhenThuong: 0,
            TongKyLuat: 0,
            TongChietKhau: 0,
            TongLuongTruocGiamTru: 0,
            TongTienPhatAll: 0,
            TongLuongSauGiamTru: 0,
            TongDaTra: 0,
            TongConLai: 0,
        },
        curentpage: {
            text: '',
            tungay: '',
            denngay: '',
            typetime: 6,
            typetimeold: 6,
            TrangThai: ["1", "2", "3", '4'],
            pagesize: 10,
            pagenow: 1,
            textdate: 'Tháng trước',
            chinhanhid: $('#hd_IDdDonVi').val(),
            ID_NhanVien: $('.idnhanvien').text(),
        },
        Key_Form: "KeyFormBangLuong",
        listpagesize: [10, 20, 30],
        listdata: {
            ChiNhanh: [],
            CongTy: [],
            PhongBan: [],
            Column: [],
            KyTinhCong: [],
            ListChiTiet: [],
            ListMauIn: [],
        },
        BangLuong: 1,
        ClickFilter: {
            LoaiLuong: true,
            ThoiGian: true,
            KyTinhCong: true,
            TrangThai: true
        },
        objectAddBangLuong: {
            ID: null,
            MaBangLuong: '',
            TenBangLuong: '',
            TrangThai: false,
            ID_KyTinhCong: null,
            TuNgay: moment().startOf('month').format('DD/MM/YYYY'),
            DenNgay: moment().endOf('month').add(1, 'days').format('DD/MM/YYYY'),
            GhiChu: '',
        },
        role: {
            PheDuyet: false,
            NhanSu: false,
        },
        thisDom: null,
        cssTop: 0,

        ListHeader: [],
        ListHeaderAll: [
            { colName: 'MaBangLuong', colText: 'Mã bảng lương', colShow: true, index: 1, Css: '' },
            { colName: 'TenBangLuong', colText: 'Tên bảng lương', colShow: true, index: 2, Css: '' },
            { colName: 'KyTinhLuong', colText: 'Kỳ tính lương', colShow: true, index: 3, Css: '' },
            { colName: 'NVLapPhieu', colText: 'NV lập phiếu', colShow: false, index: 4, Css: '' },
            { colName: 'PhaiThanhToan', colText: 'Phải thanh toán', colShow: true, index: 5, Css: '' },
            { colName: 'TruTamUng', colText: 'Trừ tạm ứng', colShow: true, index: 6, Css: '' },
            { colName: 'TongThanhToan', colText: 'Tổng thanh toán', colShow: true, index: 7, Css: '' },
            { colName: 'ConLai', colText: 'Còn lại', colShow: true, index: 8, Css: '' },
            { colName: 'NgayThanhToan', colText: 'Ngày thanh toán', colShow: true, index: 9, Css: '' },
            { colName: 'NgayTao', colText: 'Ngày tạo', colShow: false, index: 10, Css: '' },
            { colName: 'NguoiTao', colText: 'Người tạo', colShow: false, index: 11, Css: '' },
            { colName: 'TrangThai', colText: 'Trạng thái', colShow: false, index: 12, Css: '' },
            { colName: 'GhiChu', colText: 'Ghi chú', colShow: false, index: 13, Css: '' },
        ],
    },
    created: function () {
        var self = this;
        self.InitListHeader();
        self.GetListMauIn();
        self.GetInforCongTy();
        console.log('bl')
    },
    watch: {
        ListHeader: {
            handler: function () {
                let header = localStorage.getItem('lstHeader');
                if (header !== null) {
                    header = JSON.parse(header);
                    for (let i = 0; i < header.length; i++) {
                        if (header[i].Key === 'DanhMucBangLuong') {
                            header[i].List = this.ListHeader;
                            localStorage.setItem('lstHeader', JSON.stringify(header));
                            break;
                        }
                    }
                }
            },
            deep: true
        }
    },
    methods: {
        InitListHeader: function () {
            let self = this;
            let exist = false;
            let header = localStorage.getItem('lstHeader');
            if (header !== null) {
                header = JSON.parse(header);
                for (let i = 0; i < header.length; i++) {
                    if (header[i].Key === 'DanhMucBangLuong') {
                        self.ListHeader = header[i].List;
                        exist = true;
                        break;
                    }
                }
            }
            else {
                header = [];
            }
            if (!exist) {
                self.ListHeader = self.ListHeaderAll;

                let obj = {
                    Key: 'DanhMucBangLuong',
                    List: self.ListHeaderAll,
                };
                header.push(obj);
                localStorage.setItem('lstHeader', JSON.stringify(header));
            }
            else {
                if (self.ListHeader.length !== self.ListHeaderAll.length) {
                    // id exist cache && column is add new --> assign again cache
                    for (let i = 0; i < header.length; i++) {
                        if (header[i].Key === 'DanhMucBangLuong') {
                            self.ListHeader = self.ListHeaderAll;
                            header[i].List = self.ListHeaderAll;
                            break;
                        }
                    }
                    localStorage.setItem('lstHeader', JSON.stringify(header));
                }
            }
        },
        CheckColShow: function (colName) {
            let self = this;
            return self.ListHeader.find(x => x.colName === colName).colShow;
        },
        SetRole: function (value) {
            if (value === 'True') {
                this.role.PheDuyet = true;
            }
            else {
                this.role.PheDuyet = false;
            }
        },

        // Lấy dữ liệu mặc định
        GetListData: function () {
            var self = this;
            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetRoleNhanSu", function (data) {
                if (data.res) {
                    self.role.NhanSu = data.dataSoure;
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });
            self.GetForSearchBangLuong(true);
        },
        GetListMauIn: function () {
            var self = this;
            $.getJSON("/api/DanhMuc/ThietLapApi/GetListMauIn?typeChungTu=BL&idDonVi=" + VHeader.IdDonVi, function (data) {
                self.listdata.ListMauIn = data;
            });
        },
        GetInforCongTy: function () {
            var self = this;
            ajaxHelper('/api/DanhMuc/HT_API/' + 'GetHT_CongTy', 'GET').done(function (data) {
                if (data !== null) {
                    self.listdata.CongTy = data;
                }
            });
        },

        // Tìm kiếm danh sách bảng lương
        GetForSearchBangLuong: function (resetpage = false) {
            var self = this;
            self.HideTabTableDetail();
            $('#table-reponsive').gridLoader();
            if (resetpage) {
                self.curentpage.pagenow = 1;
            }
            var model = {
                ListDonVi: [self.curentpage.chinhanhid],
                Text: self.curentpage.text,
                TuNgay: self.curentpage.tungay,
                DenNgay: self.curentpage.denngay,
                TypeTime: self.curentpage.typetime,
                pageSize: self.curentpage.pagesize,
                pageNow: self.curentpage.pagenow,
                TrangThai: self.curentpage.TrangThai,
                IDNhanVien: $('.idnhanvien').text(),
            }
            var url = self.API_NhanSu + "GetAllBangLuong";
            $.ajax({
                data: model,
                url: url,
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    $('#table-reponsive').gridLoader({ show: false });
                    console.log('model ', data)
                    if (data.res === true) {
                        self.databind = data.dataSoure;
                        self.curentpage.pagenow = self.databind.pagenow;
                        if (data.dataSoure.data.length > 0) {
                            self.databind.PhaiThanhToan = data.dataSoure.data[0].TongPhaiTra;
                            self.databind.TongDaTra = data.dataSoure.data[0].TongDaTra;
                            self.databind.TongConLai = data.dataSoure.data[0].TongConLai;
                            self.databind.TongThanhToan = data.dataSoure.data[0].TongThanhToan;
                            self.databind.TongTamUng = data.dataSoure.data[0].TongTamUng;
                        }
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(data.mess);
                    }
                },
                error: function (result) {
                    $('#table-reponsive').gridLoader({ show: false });
                    console.log(result);
                }
            });

        },

        // phân trang bảng lương chi tiết nhân viên
        GetForSearchBangLuongChiTiet: function () {
            var self = this;
            $('#table-reponsive').gridLoader();
            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetBangLuongChiTiet?Id=" + self.datachitiet.idbangluong
                + "&currentPage=" + (self.datachitiet.page - 1) + '&pageSize=' + 50, function (data) {
                    if (data.res) {
                        self.datachitiet = data.dataSoure;

                        if (data.dataSoure.data.length > 0) {
                            let footer = data.dataSoure.data[0].data[0];
                            self.datachitiet.TongNgayCongThuc = footer.TongNgayCongThuc;
                            self.datachitiet.TongLuongChinh = footer.TongLuongChinh;
                            self.datachitiet.TongLuongOT = footer.TongLuongOT;
                            self.datachitiet.TongPhuCapCoBan = footer.TongPhuCapCoBan;
                            self.datachitiet.TongPhuCapKhac = footer.TongPhuCapKhac;
                            self.datachitiet.TongKhenThuong = footer.TongKhenThuong;
                            self.datachitiet.TongKyLuat = footer.TongKyLuat;
                            self.datachitiet.TongChietKhau = footer.TongChietKhau;
                            self.datachitiet.TongLuongTruocGiamTru = footer.TongLuongTruocGiamTru;
                            self.datachitiet.TongTienPhatAll = footer.TongTienPhatAll;
                            self.datachitiet.TongLuongSauGiamTru = footer.TongLuongSauGiamTru;
                            self.datachitiet.TongDaTra = footer.TongDaTra;
                            self.datachitiet.TongConLai = footer.TongConLai;
                        }
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(data.mess);
                    }

                    $('#table-reponsive').gridLoader({ show: false });
                    self.thisDom.closest('tbody').closest('table').closest('.table-reponsive').removeClass('tablescroll');
                    commonStatisJs.sleep(100).then(() => {
                        var heightold = self.thisDom.next().height();
                        var heigth = parseInt(self.thisDom.height()) + heightold;
                        $('.line-right').height(heigth).css("margin-top", self.cssTop + "px");
                    });
                });

        },

        // ẩn hiện giao diện 
        HideTabTableDetail: function () {
            $(".op-tr-hide").hide();
            $(".op-tr-show").removeClass('tr-active');
            $('.line-right').height(0).css("margin-top", "0px");
            if (!$('.table-reponsive').hasClass('tablescroll')) {
                $('.table-reponsive').addClass('tablescroll');
            }
        },

        //---------tìm kiếm
        AddTrangThai: function (ev) {
            var value = $(ev.target).val();
            if (this.curentpage.TrangThai.some(o => o === value)) {
                this.curentpage.TrangThai = this.curentpage.TrangThai.filter(o => o !== value);
            }
            else {
                this.curentpage.TrangThai.push(value);
            }
            this.GetForSearchBangLuong(true);
        },

        keymonitor: function (event) {
            if (event.key === "Enter") {
                this.GetForSearchBangLuong(true);
            }
        },

        ButtonSelectPage: function (item, isfirstpage = null) {
            if (isfirstpage === null) {
                this.curentpage.pagenow += item;
                if (this.curentpage.pagenow <= 0) {
                    this.curentpage.pagenow = 1;
                }
                else if (this.curentpage.pagenow <= 0) {
                    this.curentpage.pagenow = 1;
                }
            }
            else if (isfirstpage === true) {
                this.curentpage.pagenow = 1;
            }
            else {
                this.curentpage.pagenow = this.databind.countpage;
            }
            this.GetForSearchBangLuong();
        },

        ButtonDetailSelectPage: function (item, isfirstpage = null) {
            if (isfirstpage === null) {
                this.datachitiet.page += item;
                if (this.datachitiet.page <= 0) {
                    this.datachitiet.page = 1;
                }
                else if (this.datachitiet.page <= 0) {
                    this.datachitiet.page = 1;
                }
            }
            else if (isfirstpage === true) {
                this.datachitiet.page = 1;
            }
            else {
                this.datachitiet.page = this.datachitiet.countpage;
            }
            this.GetForSearchBangLuongChiTiet();
        },

        SelectPage: function (item) {
            this.curentpage.pagenow = item;
            this.GetForSearchBangLuong();
        },

        SelectDetailPage: function (item) {
            this.datachitiet.page = item;
            this.GetForSearchBangLuongChiTiet();
        },

        SelectPageSize: function (e) {
            this.GetForSearchBangLuong(true);
        },

        // click show chi tiết
        SelectRow: function (item, event) {
            var self = this;
            var $this = $(event.target).closest('tr');
            var css = $this.next(".op-tr-hide").css('display');
            $(".op-tr-hide").hide();
            $(".op-tr-show").removeClass('tr-active');
            self.cssTop = 34 + parseInt($this.height() * ($this.index() / 2));
            $('.line-right').height(0).css("margin-top", "0px");
            if (css === 'none') {
                $this.addClass('tr-active');
                $this.next(".op-tr-hide").toggle();
                self.thisDom = $this;
                self.datachitiet.idbangluong = item.ID;
                self.datachitiet.page = 1;
                self.GetForSearchBangLuongChiTiet();

            }
            else {
                if (!$this.closest('tbody').closest('table').closest('.table-reponsive').hasClass('tablescroll')) {
                    $this.closest('tbody').closest('table').closest('.table-reponsive').addClass('tablescroll');
                }
            }
        },

        ChangeKyTinhCong: function (item) {
            item.Checked = !item.Checked;
            this.GetForSearchBangLuong(true);
        },

        // Thay đổi loại báo cáo lương
        ChangeLoaiCong: function (giatri) {
            if (this.BangLuong !== giatri) {
                this.BangLuong = giatri;
                this.GetForSearchBangLuong(true);
            }
        },

        // ------- end tìm kiếm

        HuyBangLuong: function (item) {
            var self = this;
            var mes = 'Bạn có chắc chắn muốn hủy bảng lương này không?';
            if (item.TrangThai === 4) {
                mes = 'Bảng lương đã <b> thanh toán</b>. Bạn có muốn hủy bảng lương và <b> các phiếu chi </b> liên quan không?';
            }
            commonStatisJs.ConfirmDialog_OKCancel('Hủy bảng lương', mes, function (x) {
                $.getJSON(self.API_NhanSu + 'HuyBangLuong?idBangLuong=' + item.ID, function (x) {
                    if (x.res === true) {
                        commonStatisJs.ShowMessageSuccess(x.mess);
                    }
                });
            }, function (x) {

            });
        },


        TinhLaiBangLuong: function (item) {
            this.isNew = false;
            var model = commonStatisJs.CopyObject(item);
            this.objectAddBangLuong = {
                ID: model.ID,
                MaBangLuong: model.MaBangLuong,
                TenBangLuong: model.TenBangLuong,
                TrangThai: model.TrangThai === 1 ? false : true,
                ID_KyTinhCong: model.ID_KyTinhCong,
                GhiChu: model.GhiChu,
                NgayThanhToanLuong: model.NgayThanhToanLuong
            };
            commonStatisJs.ConfirmDialog_OKCancel('Tính lại bảng lương', 'Hệ thống sẽ cập nhật lại lương cho nhân viên. Bạn có chắc chắn muốn cập nhật không?', function (x) {
                vmBangLuong.SaveTinhLaiBangLuong();
            }, function (x) {

            });
        },

        SaveTinhLaiBangLuong: function () {
            var self = this;
            self.loadding = true;
            $.ajax({
                data: null,
                url: self.API_NhanSu + "TinhLaiBangLuong?idBangLuong=" + self.objectAddBangLuong.ID + '&nguoiSua=' + self.UserLogin,
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    console.log(data);
                    if (data.res === true) {
                        commonStatisJs.ShowMessageSuccess(data.mess);

                        var diary = {
                            ID_DonVi: VHeader.IdDonVi,
                            ID_NhanVien: self.curentpage.ID_NhanVien,
                            LoaiNhatKy: 1,
                            ChucNang: 'Bảng lương - Tính lại lương',
                            NoiDung: 'Tính lại bảng lương ' + self.objectAddBangLuong.MaBangLuong,
                            NoiDungChiTiet: 'Tính lại bảng lương '.concat(self.objectAddBangLuong.MaBangLuong, ' <br /> - Nhân viên thực hiện:', self.UserLogin)
                        }
                        self.SaveDiary(diary);

                        self.GetForSearchBangLuong(true);
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(data.mess);
                    }
                    self.loadding = false;
                },
                error: function (result) {
                    console.log(result);
                    self.loadding = false;
                }
            });
        },

        GotoSalaryDetail: function (id, fromDate, toDate) {
            var self = this;
            var lcbangluong = {
                ID: id,
                TuNgay: fromDate,
                DenNgay: toDate,
                GhiChu: self.objectAddBangLuong.GhiChu,
                MaBangLuong: self.objectAddBangLuong.MaBangLuong,
            }
            console.log('lcbangluong', lcbangluong)
            localStorage.setItem('lcbangluong', JSON.stringify(lcbangluong));
            localStorage.setItem('fromSalary', 1);
            $('#modalThemMoiBangLuong').modal('hide');
            window.open('#/SalaryDetail', '_blank');
        },

        AddBangLuong: function () {
            this.isNew = true;
            this.objectAddBangLuong = {
                ID: null,
                MaBangLuong: '',
                TenBangLuong: '',
                TrangThai: false,
                ID_KyTinhCong: null,
                GhiChu: '',
                NgayThanhToanLuong: null,
                TuNgay: moment().add(-1, 'months').startOf('month').format('DD/MM/YYYY'),
                DenNgay: moment().add(-1, 'months').endOf('month').format('DD/MM/YYYY'),
            };
            console.log('objectAddBangLuong ', this.objectAddBangLuong)
            $('#modalThemMoiBangLuong').modal('show');
        },

        UpdatebangLuong: function (item) {
            console.log(item);
            this.isNew = false;
            var model = commonStatisJs.CopyObject(item);
            this.objectAddBangLuong = {
                ID: model.ID,
                ID_NhanVienDuyet: model.ID_NhanVienDuyet,
                MaBangLuong: model.MaBangLuong,
                TenBangLuong: model.TenBangLuong,
                TuNgay: moment(model.TuNgay).format('YYYY-MM-DD'),
                DenNgay: moment(model.DenNgay).format('YYYY-MM-DD'),
                TrangThai: model.TrangThai,
                GhiChu: model.GhiChu,
                NgayThanhToanLuong: model.NgayThanhToanLuong,
            };
            var ctluong = [];
            for (let i = 0; i < this.datachitiet.data.length; i++) {
                for (let j = 0; j < this.datachitiet.data[i].data.length; j++) {
                    ctluong.push(this.datachitiet.data[i].data[j]);
                }
            }
            console.log(ctluong);
            localStorage.setItem('fromSalary', 2);
            localStorage.setItem('lcbangluong', JSON.stringify(this.objectAddBangLuong));
            localStorage.setItem('lcChiTietLuong', JSON.stringify(ctluong));
            window.open('#/SalaryDetail', '_blank');
        },

        CreateBangLuong: function () {
            var self = this;
            var fromDate = self.objectAddBangLuong.TuNgay;
            var toDate = self.objectAddBangLuong.DenNgay;
            if (fromDate === '' || toDate === '') {
                commonStatisJs.ShowMessageDanger("Vui lòng nhập thời gian tính lương");
                self.loadding = false;
                return;
            }
            fromDate = moment(fromDate, 'DD/MM/YYYY').format('YYYY-MM-DD');
            toDate = moment(toDate, 'DD/MM/YYYY').format('YYYY-MM-DD');

            $.getJSON(self.API_NhanSu + 'CheckBangLuongExist?idDonVi=' + self.curentpage.chinhanhid + '&fromDate=' + fromDate + '&toDate=' + toDate, function (x) {
                console.log(x)
                if (x.res) {
                    if (x.dataSoure !== '') {
                        commonStatisJs.ConfirmDialog_OKCancel('Tạo bảng lương', 'Tồn tại bảng lương tạm trong khoảng thời gian này. Bạn có muốn cập nhật không?', function () {
                            self.GotoSalaryDetail(x.dataSoure, fromDate, toDate);
                        }, function () {
                            $('#modalThemMoiBangLuong').modal('hide');
                        });
                    }
                    else {
                        self.GotoSalaryDetail('00000000-0000-0000-0000-000000000000', fromDate, toDate);
                    }
                }
                else {
                    commonStatisJs.ShowMessageDanger(x.mess);
                }
            });
        },

        showmodalThanhToan: function (item) {
            var self = this;
            self.datachitiet.idbangluong = item.ID;
            vmThanhToanLuong.ModalShow(item.ID, item.MaBangLuong);
        },

        ExportChiTietNhanVien: function (item) {
            var self = this;
            $('#table-reponsive').gridLoader();
            $.ajax({
                data: item,
                url: self.API_NhanSu + "ExportExcelBangLuongChiTiet",
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    $('#table-reponsive').gridLoader({ show: false });
                    if (data.res === true) {
                        window.location.href = "/api/DanhMuc/NS_NhanSuAPI/DownloadFileExecl?fileSave=" + data.dataSoure;
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(data.mess);
                    }
                }
            });
        },

        PheDuyetbangLuong: function (item) {
            var self = this;
            $('#table-reponsive').gridLoader();
            console.log(item);
            item.ID_NhanVienDuyet = self.curentpage.ID_NhanVien;
            $.ajax({
                data: item,
                url: self.API_NhanSu + "PheDuyetBangLuong",
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    console.log(data);
                    $('#table-reponsive').gridLoader({ show: false });
                    if (data.res === true) {
                        commonStatisJs.ShowMessageSuccess(data.mess);
                        self.GetForSearchBangLuong(true);
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(data.mess);
                    }
                }
            });
        },

        MoLaiBangLuongDaChot: function (item) {
            var self = this;

            $.ajax({
                data: item,
                url: self.API_NhanSu + "MoLaiBangLuongDaChot",
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    console.log(data);
                    $('#table-reponsive').gridLoader({ show: false });
                    if (data.res === true) {
                        commonStatisJs.ShowMessageSuccess(data.mess);
                        self.GetForSearchBangLuong(true);
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(data.mess);
                    }
                }
            });
        },

        ShowModalChiTietLuong: function (item) {
            // todo
        },

        SaveDiary: function (objNhatKy) {
            var diary = {};
            diary.objDiary = objNhatKy;
            $.ajax({
                data: diary,
                url: "/api/DanhMuc/SaveDiary/post_NhatKySuDung",
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (x) {

                }
            });
        },
        InBangluong: function (type, bluong, ctluong = null) {
            var self = this;
            var ngaylap = moment(bluong.NgayTao).format('DD/MM/YYYY');
            var url = '';

            var arr = [];
            switch (type) {
                case 1:
                    let ngay = '', thang = '', nam = '', ngayTT = '';
                    if (!commonStatisJs.CheckNull(ctluong.NgayThanhToan)) {
                        ngay = moment(ctluong.NgayThanhToan).format('DD');
                        thang = moment(ctluong.NgayThanhToan).format('MM');
                        nam = moment(ctluong.NgayThanhToan).format('YYYY');
                        ngayTT = moment(ctluong.NgayThanhToan).format('DD/MM/YYYY');
                    }

                    let itPrint = $.extend({}, true, ctluong);
                    var thucnhan = itPrint.LuongSauGiamTru - itPrint.TruTamUngLuong;
                    itPrint.TongGiamTru = formatNumber3Digit(ctluong.TongTienPhat);
                    itPrint.LuongCoBan = formatNumber3Digit(ctluong.LuongCoBan);
                    itPrint.LuongOT = formatNumber3Digit(ctluong.LuongOT);
                    itPrint.LuongCoBan = formatNumber3Digit(ctluong.LuongCoBan);
                    itPrint.LuongChinh = formatNumber3Digit(ctluong.LuongChinh);
                    itPrint.PhuCapCoBan = formatNumber3Digit(ctluong.PhuCapCoBan);
                    itPrint.PhuCapKhac = formatNumber3Digit(ctluong.PhuCapKhac);
                    itPrint.ChietKhau = formatNumber3Digit(ctluong.ChietKhau);
                    itPrint.LuongSauGiamTru = formatNumber3Digit(ctluong.LuongSauGiamTru);
                    itPrint.TruTamUngLuong = formatNumber3Digit(ctluong.TruTamUngLuong);
                    itPrint.ThucLinh = formatNumber3Digit(thucnhan);
                    itPrint.ThanhToan = formatNumber3Digit(ctluong.ThanhToan);
                    itPrint.TienBangChu = DocSo(thucnhan);

                    itPrint.Ngay = ngay;
                    itPrint.Thang = thang;
                    itPrint.Nam = nam;
                    itPrint.NgayThanhToan = ngayTT;
                    itPrint.NgayLapPhieu = ngaylap;
                    itPrint.TenBangLuong = bluong.TenBangLuong;

                    itPrint.LogoCuaHang = Open24FileManager.hostUrl + self.listdata.CongTy[0].DiaChiNganHang;
                    itPrint.TenCuaHang = self.listdata.CongTy[0].TenCongTy;
                    itPrint.DiaChiCuaHang = self.listdata.CongTy[0].DiaChi;
                    itPrint.DienThoaiCuaHang = self.listdata.CongTy[0].SoDienThoai;
                    itPrint.TenChiNhanh = VHeader.TenDonVi;
                    itPrint.DiaChiChiNhanh = bluong.DiaChiChiNhanh;
                    itPrint.DienThoaiChiNhanh = bluong.DienThoaiChiNhanh;

                    arr = [itPrint];
                    url = '/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=BL&idDonVi=' + VHeader.IdDonVi;
                    break;
                case 2:
                    url = '/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=BL&idDonVi=' + VHeader.IdDonVi;
                    break;
                case 3:
                    url = '/api/DanhMuc/ThietLapApi/GetContentFIlePrint?idMauIn=' + ctluong.Key;
                    break;
            }
            if (type !== 1) {
                let lenCT = self.datachitiet.data.length;
                for (let i = 0; i < lenCT; i++) {
                    let itFor = self.datachitiet.data[i];

                    let ngay = '', thang = '', nam = '', ngayTT = '';
                    let itPrint = $.extend({}, true, itFor.data[0]);
                    itPrint.MaNhanVien = itFor.MaNhanVien;
                    itPrint.TenNhanVien = itFor.TenNhanVien;

                    if (!commonStatisJs.CheckNull(itPrint.NgayThanhToan)) {
                        ngay = moment(itPrint.NgayThanhToan).format('DD');
                        thang = moment(itPrint.NgayThanhToan).format('MM');
                        nam = moment(itPrint.NgayThanhToan).format('YYYY');
                        ngayTT = moment(itPrint.NgayThanhToan).format('DD/MM/YYYY');
                    }
                    var thucnhan = itPrint.LuongSauGiamTru - itPrint.TruTamUngLuong;
                    itPrint.TongGiamTru = formatNumber3Digit(itPrint.TongTienPhat);
                    itPrint.LuongCoBan = formatNumber3Digit(itPrint.LuongCoBan);
                    itPrint.LuongOT = formatNumber3Digit(itPrint.LuongOT);
                    itPrint.LuongCoBan = formatNumber3Digit(itPrint.LuongCoBan);
                    itPrint.LuongChinh = formatNumber3Digit(itPrint.LuongChinh);
                    itPrint.PhuCapCoBan = formatNumber3Digit(itPrint.PhuCapCoBan);
                    itPrint.PhuCapKhac = formatNumber3Digit(itPrint.PhuCapKhac);
                    itPrint.ChietKhau = formatNumber3Digit(itPrint.ChietKhau);
                    itPrint.LuongSauGiamTru = formatNumber3Digit(itPrint.LuongSauGiamTru);
                    itPrint.TruTamUngLuong = formatNumber3Digit(itPrint.TruTamUngLuong);
                    itPrint.ThucLinh = formatNumber3Digit(thucnhan);
                    itPrint.ThanhToan = formatNumber3Digit(itPrint.ThanhToan);
                    itPrint.TienBangChu = DocSo(thucnhan);

                    itPrint.Ngay = ngay;
                    itPrint.Thang = thang;
                    itPrint.Nam = nam;
                    itPrint.NgayThanhToan = ngayTT;
                    itPrint.NgayLapPhieu = ngaylap;

                    itPrint.LogoCuaHang = Open24FileManager.hostUrl + self.listdata.CongTy[0].DiaChiNganHang;
                    itPrint.TenCuaHang = self.listdata.CongTy[0].TenCongTy;
                    itPrint.DiaChiCuaHang = self.listdata.CongTy[0].DiaChi;
                    itPrint.DienThoaiCuaHang = self.listdata.CongTy[0].SoDienThoai;
                    itPrint.TenChiNhanh = VHeader.TenDonVi;
                    itPrint.DiaChiChiNhanh = bluong.DiaChiChiNhanh;
                    itPrint.DienThoaiChiNhanh = bluong.DienThoaiChiNhanh;
                    itPrint.TenBangLuong = bluong.TenBangLuong;

                    arr.push(itPrint);
                }
            }

            ajaxHelper(url, 'GET').done(function (result) {
                var data = result;
                data = data.concat('<script src="/Scripts/knockout-3.4.2.js"></script>');
                data = data.concat("<script > var item1=" + JSON.stringify(arr) + "; </script>");
                data = data.concat('<script> var dataMauIn = function () {' +
                    'var self = this;' +
                    'self.ListHoaDon_ChiTietHoaDonPrint = ko.observableArray(item1);' +
                    'self.Count_ListHoaDons = ko.computed(function () { ' +
                    'return self.ListHoaDon_ChiTietHoaDonPrint().length;' +
                    '})' +
                    '};' +
                    'ko.applyBindings(new dataMauIn()) </script>');
                PrintExtraReport(data);
            })
        },
        ExportExcel: function () {
            let self = this;
            if (self.databind.length > 0) {
                var colHide = [];
                for (let i = 0; i < self.ListHeader.length; i++) {
                    let itFor = self.ListHeader[i];
                    if (!itFor.colShow) {
                        colHide.push(i);
                    }
                }
            }
        },
    },
    computed: {
        TitlePopup: function () {
            if (this.isNew) {

                return "Thêm mới bảng lương";
            }
            else {

                return "Cập nhật bảng lương";
            }
        }
    },
});
$('.datetime').on('change', function () {
    var dateParts = $(this).val().split("/");
    if (dateParts.length >= 3) {
        var dateObject = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);
        vmBangLuong.objectAddBangLuong.NgayThanhToanLuong = dateObject;
    }
})
vmBangLuong.GetListData();
$('.newDateTime').on('apply.daterangepicker', function (ev, picker) {
    vmBangLuong.curentpage.tungay = picker.startDate.format('MM/DD/YYYY');
    vmBangLuong.curentpage.denngay = picker.endDate.format('MM/DD/YYYY');
    //vmBangLuong.GetForSearchBangLuong(true);

    vmBangLuong.objectAddBangLuong.TuNgay(picker.startDate.format('MM/DD/YYYY'));
    vmBangLuong.objectAddBangLuong.DenNgay(picker.endDate.format('MM/DD/YYYY'));
});
$('#SinhNhatSL ').on('click', '.radio-menu input[type="radio"]', function () {
    var dataid = $(this).data('id');
    $('#SinhNhatSL .form-group').each(function () {
        $(this).find('.conten-choose').find('input').removeAttr('disabled');
        if (dataid !== $(this).find('.radio-menu').find('input').data('id')) {
            $(this).find('.conten-choose').find('input').attr('disabled', 'disabled');
        }
    });
    if ($(this).data('id') !== 1) {
        vmBangLuong.curentpage.typetime = null;
    }
    else {
        vmBangLuong.curentpage.typetime = vmBangLuong.curentpage.typetimeold;
    }
    vmBangLuong.GetForSearchBangLuong(true);
})
$('#SelectNgaySinh').on('click', 'ul li', function () {

    if (vmBangLuong.curentpage.typetime !== null) {
        vmBangLuong.curentpage.textdate = $(this).find('a').text();
        vmBangLuong.curentpage.typetime = $(this).val();
        vmBangLuong.curentpage.typetimeold = $(this).val();
        vmBangLuong.GetForSearchBangLuong(true);
    }
});

$(function () {
    $('.newDatesingle').datetimepicker({
        timepicker: false,
        mask: true,
        format: 'd/m/Y',
        onChangeDateTime: function (dp, $input, a) {
            let dtChange = moment(dp).format('DD/MM/YYYY');
            if (parseInt($($input).attr('data-id')) === 1) {
                vmBangLuong.objectAddBangLuong.TuNgay = dtChange;
            }
            else {
                vmBangLuong.objectAddBangLuong.DenNgay = dtChange;
            }
        },
    });
})
$('body').on('ThanhToanLuongSuccess', function () {
    vmBangLuong.GetForSearchBangLuongChiTiet();
});