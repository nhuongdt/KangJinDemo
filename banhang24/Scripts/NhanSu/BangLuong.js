var vmBangLuong = new Vue({
    el: '#BangLuongNhanSu',
    data: {
        isNew: true,
        loadding: false,
        databind: {
            pageview: '',
            pagenow: 1,
            data: [],
            listpage: [],
            isprev: false,
            isnext: false,
            countpage: 0,
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
            idbangluong: null
        },
        curentpage: {
            text: '',
            tungay: '',
            denngay: '',
            typetime: 5,
            typetimeold: 5,
            TrangThai: [],
            pagesize: 10,
            pagenow: 1,
            textdate: 'Tháng này',
            chinhanhid: $('#hd_IDdDonVi').val(),
            ID_NhanVien: $('.idnhanvien').text(),
        },
        Key_Form: "KeyFormBangLuong",
        listpagesize: [10, 20, 30],
        listdata: {
            ChiNhanh: [],
            PhongBan: [],
            Column: [],
            KyTinhCong: [],
            ListChiTiet: []
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
            GhiChu: '',
        },
        role: {
            PheDuyet: false,
            NhanSu: false,
        },
        thisDom: null,
        cssTop: 0
    },
    methods: {
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
            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetAllKyTinhCong", function (data) {
                if (data.res) {
                    self.listdata.KyTinhCong = data.dataSoure;
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });
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

        // Tìm kiếm danh sách bảng lương
        GetForSearchBangLuong: function (resetpage = false) {
            var self = this;
            self.HideTabTableDetail();
            $('#table-reponsive').gridLoader();
            if (resetpage) {
                self.curentpage.pagenow = 1;
            }
            var model = {
                Text: self.curentpage.text,
                TuNgay: self.curentpage.tungay,
                DenNgay: self.curentpage.denngay,
                TypeTime: self.curentpage.typetime,
                pageSize: self.curentpage.pagesize,
                pageNow: self.curentpage.pagenow,
                TrangThai: self.curentpage.TrangThai,
                KyTinhCongs: self.listdata.KyTinhCong.filter(o => o.Checked).map(o => o.ID),
                IDNhanVien: $('.idnhanvien').text(),
            }
            var url = "/api/DanhMuc/NS_NhanSuAPI/GetSearchForBangLuong";
            $.ajax({
                data: model,
                url: url,
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    $('#table-reponsive').gridLoader({ show: false });
                    if (data.res === true) {
                        self.databind = data.dataSoure;
                        self.curentpage.pagenow = self.databind.pagenow;
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
            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetBangLuongChiTiet?Id=" + self.datachitiet.idbangluong + "&pageItem=" + self.datachitiet.page, function (data) {
                if (data.res) {
                    self.datachitiet = data.dataSoure;
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

        // Show tính lại bảng lương
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
            $('#modalpopup_TinhLaiBangLuong').modal('show');
        },
        SaveTinhLaiBangLuong: function () {
            var self = this;
            self.loadding = true;
            $.ajax({
                data: self.objectAddBangLuong,
                url: "/api/DanhMuc/NS_NhanSuAPI/TinhLaiBangLuong",
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    if (data.res === true) {
                        commonStatisJs.ShowMessageSuccess(data.mess);
                        $('#modalpopup_TinhLaiBangLuong').modal('hide');
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
        // Show thêm mới bảng lương
        AddBangLuong: function () {
            this.isNew = true;
            this.objectAddBangLuong = {
                ID: null,
                MaBangLuong: '',
                TenBangLuong: '',
                TrangThai: false,
                ID_KyTinhCong: null,
                GhiChu: '',
                NgayThanhToanLuong: null
            };
            $('#modalThemMoiBangLuong').modal('show');
        },

        // show update bảng lương
        UpdatebangLuong: function (item) {
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
            $('#selectKyTinhCong').val(model.ID_KyTinhCong)
            $('#modalThemMoiBangLuong').modal('show');
        },

        // Tạo mới bảng lương
        CreateBangLuong: function () {
            var self = this;
            self.loadding = true;
            if (commonStatisJs.CheckNull(self.objectAddBangLuong.TenBangLuong)) {
                commonStatisJs.ShowMessageDanger("Vui lòng nhập tên bảng lương");
                self.loadding = false;
                return;
            }
            if (commonStatisJs.CheckNull(self.objectAddBangLuong.ID_KyTinhCong)) {
                commonStatisJs.ShowMessageDanger("Vui lòng kỳ tính công");
                self.loadding = false;
                return;
            }
            else {
                var url = "/api/DanhMuc/NS_NhanSuAPI/InsertBangLuong";
                if (!self.isNew) {
                    url = "/api/DanhMuc/NS_NhanSuAPI/UpdateBangLuong";
                }
                var model = {
                    ID: self.objectAddBangLuong.ID,
                    MaBangLuong: self.objectAddBangLuong.MaBangLuong,
                    TenBangLuong: self.objectAddBangLuong.TenBangLuong,
                    TrangThai: self.objectAddBangLuong.TrangThai === true ? 2 : 1,
                    ID_KyTinhCong: self.objectAddBangLuong.ID_KyTinhCong,
                    GhiChu: self.objectAddBangLuong.GhiChu,
                    NgayThanhToanLuong: commonStatisJs.convertDateToDateServer(self.objectAddBangLuong.NgayThanhToanLuong),
                }
                $.ajax({
                    data: model,
                    url: url + "?" + "ID_DonVi=" + $('#hd_IDdDonVi').val() + "&ID_NhanVien=" + $('.idnhanvien').text(),
                    type: 'POST',
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (data) {
                        if (data.res === true) {
                            commonStatisJs.ShowMessageSuccess(data.mess);
                            $('#modalThemMoiBangLuong').modal('hide');
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
            }

        },

        // export file excel
        ExportChiTietNhanVien: function (item) {
            var self = this;
            $('#table-reponsive').gridLoader();
            $.ajax({
                data: item,
                url: "/api/DanhMuc/NS_NhanSuAPI/ExportExcelBangLuongChiTiet",
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
            $.ajax({
                data: item,
                url: "/api/DanhMuc/NS_NhanSuAPI/PheDuyetBangLuong",
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
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

        HuyPheDuyet: function (item) {
            var self = this;
            commonStatisJs.ConfirmDialog_OKCancel('Hủy phê duyệt', 'Bạn có chắc chắn muốn hủy phê duyệt bảng lương <b>' + item.TenBangLuong + '</b> không?',
                function () {
                    $('#table-reponsive').gridLoader();
                    $.ajax({
                        data: item,
                        url: "/api/DanhMuc/NS_NhanSuAPI/HuyPheDuyet",
                        type: 'POST',
                        dataType: 'json',
                        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                        success: function (data) {
                            debugger
                            $('#table-reponsive').gridLoader({ show: false });
                            if (data.res === true) {
                                commonStatisJs.ShowMessageSuccess(data.mess);
                                self.GetForSearchBangLuong(true);

                                let kytinhcong = $.grep(self.listdata.KyTinhCong, function (x) {
                                    return x.ID === item.ID_KyTinhCong;
                                });
                                let diary = {
                                    LoaiNhatKy: 3,
                                    ID_DonVi: self.curentpage.chinhanhid,
                                    ID_NhanVien: self.curentpage.ID_NhanVien,
                                    ChucNang: 'Bảng lương',
                                    NoiDung: "Hủy phê duyệt bảng lương : " + item.TenBangLuong,
                                    NoiDungChiTiet: "Thông tin bảng lương bị hủy phê duyệt"
                                        + "<br/> Tên: " + item.TenBangLuong
                                        + "<br/> Kỳ tính công (kỳ: " + kytinhcong[0].Ky + '), Từ: ' + commonStatisJs.convertDateTime(kytinhcong[0].TuNgay) + ' đến ' + commonStatisJs.convertDateTime(kytinhcong[0].DenNgay)
                                        + "<br/> Ngày thanh toán: " + item.NgayThanhToanLuong != null ? commonStatisJs.convertDateTime(data.NgayThanhToanLuong) : '' +
                                        + "<br/> Ghi chú bảng lương: " + item.GhiChu,
                                };
                                self.SaveDiary(diary);
                            }
                            else {
                                commonStatisJs.ShowMessageDanger(data.mess);
                            }
                        }
                    });
                });
        },

        XoaBangLuong: function (item) {

        },

        SaveDiary: function (objNhatKy) {
            var diary = {};
            diary.objDiary = objNhatKy;
            ajaxHelper('/api/DanhMuc/SaveDiary/' + "post_NhatKySuDung", 'post', diary).done(function () {

            });
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
    vmBangLuong.GetForSearchBangLuong(true);
})
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
})