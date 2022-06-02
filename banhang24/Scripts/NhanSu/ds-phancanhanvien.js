var vmPhieuPhanCa = new Vue({
    el: '#PhanCaLamViec',
    data: {
        databind: {
            pageview: '',
            pagenow: 1,
            data: [],
            listpage: [],
            isprev: false,
            isnext: false,
            countpage: 0,
        },
        listdata: {
            CaLamViec: [],
        },
        curentpage: {
            text: '',
            tungay: '',
            denngay: '',
            typetime: 5,
            typetimeold: 5,
            TrangThai: [],
            pagesize: 10,
            order: null,
            sort: false,
            pagenow: 1,
            textdate: 'Tháng này',
            ChiNhanh: [],
            LoaiCa: [],

            textdateNgayTao: 'Toàn thời gian',
            ngaytaoTu: '',
            ngaytaoDen: '',
            typetimeNgayTao: 0,
            typetimeNgayTaoOld: 0,

            TenCaChosing: '--Chọn ca --',
        },
        chitietphieu: {
            modelChiTietCa: [],
            modelNhanVienCa: []
        },
        Key_Form: "KeyFormPhieuPhanCa",
        ID_DonVi: $('#hd_IDdDonVi').val(),
        listpagesize: [10, 20, 30],
        listcolumn: [],
        listchinhanhold: [],
        ClickFilter: {
            ChiNhanh: true,
            ThoiGian: true,
            TrangThai: true,
            LoaiCa: true,
            CaLamViec: false,
        },
        role: {
            NhanSu: false,
        },
    },
    methods: {
        GetListCaLamViec: function () {
            var self = this;
            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetListCaTheoChiNhanh?id=" + self.ID_DonVi, function (data) {
                if (data.res) {
                    self.listdata.CaLamViec = data.dataSoure.filter(x => x.TrangThai!==0);
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });
        },
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
            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetListChiNhanhNhanVien?id=" + $('.idnhanvien').text(), function (data) {
                if (data.res) {
                    data.dataSoure.filter(x => x.Id == self.ID_DonVi).map(x => x.Checked = true);
                    self.curentpage.ChiNhanh = data.dataSoure;
                    self.listchinhanhold = commonStatisJs.CopyArray(data.dataSoure);
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });

            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetListLoaiCaFilter", function (data) {
                if (data.res) {
                    self.curentpage.LoaiCa = data.dataSoure;
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });
            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetListColumnPhieu", function (data) {
                if (data.res) {
                    self.listcolumn = data.dataSoure;
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });
        },
        GetForSearchPhanCa: function (resetpage = false) {
            var self = this;
            $('#table-reponsive').gridLoader();
            if (resetpage) {
                self.curentpage.pagenow = 1;
            }
            var arrDV = self.curentpage.ChiNhanh.filter(x => x.Checked).map(x => x.Id);
            if (arrDV.length === 0) {
                arrDV = [self.ID_DonVi];
            }
            var model = {
                Text: self.curentpage.text,
                TuNgay: self.curentpage.tungay,
                DenNgay: self.curentpage.denngay,
                TypeTime: self.curentpage.typetime,
                pageSize: self.curentpage.pagesize,
                pageNow: self.curentpage.pagenow,
                TrangThai: self.curentpage.TrangThai.map(function (x) { return x }),
                ListDonVi: arrDV,
                LoaiCa: self.curentpage.LoaiCa.filter(o => o.Checked).map(o => o.Id),
                IDNhanVien: $('.idnhanvien').text(),
                TypeTimeNgayTao: self.curentpage.typetimeNgayTao,
                NgayTaoTu: self.curentpage.ngaytaoTu,
                NgayTaoDen: self.curentpage.ngaytaoDen,
            }
            console.log('model', model)
            $.ajax({
                data: model,
                url: "/api/DanhMuc/NS_NhanSuAPI/GetForSearchPhanCa",
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    $('#table-reponsive').gridLoader({ show: false });
                    if (data.res === true) {
                        self.databind = data.dataSoure;
                        self.curentpage.pagenow = self.databind.pagenow;
                        commonStatisJs.sleep(100).then(() => {
                            self.LoadFirst();
                        });
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
        ChangeChiNhanh: function (item) {
            item.Checked = !item.Checked;
            this.GetForSearchPhanCa(true);
        },
        ResetChoseCa: function () {
            var self = this;
            for (let i = 0; i < self.listdata.CaLamViec.length; i++) {
                self.listdata.CaLamViec[i].Checked = false;
            }
            self.curentpage.TenCaChosing = '--Chọn ca --';
            self.ClickFilter.CaLamViec = false;
        },
        ChangeCaLamViec: function (item) {
            var self = this;
            // remove item has chosed & chose new item
            for (let i = 0; i < self.listdata.CaLamViec.length; i++) {
                if (self.listdata.CaLamViec[i].Id !== item.Id) {
                    self.listdata.CaLamViec[i].Checked = false;
                }
            }
            item.Checked = !item.Checked;
            self.curentpage.TenCaChosing = item.Ten;
            self.ClickFilter.CaLamViec = true;
        },
        ChangeLoaiCa: function (item) {
            item.Checked = !item.Checked;
            this.GetForSearchPhanCa(true);
        },
        AddNewCaLamViec: function () {
            vmEditCaLamViec.listchinhanh = commonStatisJs.CopyArray(this.curentpage.ChiNhanh);
            vmEditCaLamViec.AddNew();
        },
        EditCaLamViec: function () {
            var self = this;
            vmEditCaLamViec.listchinhanh = commonStatisJs.CopyArray(self.curentpage.ChiNhanh);
            var caChosed = self.listdata.CaLamViec.filter(x => x.Checked);
            caChosed[0].MaCa = caChosed[0].Ma;
            caChosed[0].TenCa = caChosed[0].Ten;
            caChosed[0].TrangThai = '1';
            vmEditCaLamViec.Edit(caChosed[0]);
        },
        SelectRow: function (item, event) {
            var self = this;
            var heigth = 0;
            var heightold = 0;
            var setTop = 0;
            var $this = $(event.target).closest('tr');
            var css = $this.next(".op-tr-hide").css('display');
            $(".op-tr-hide").hide();
            $(".op-tr-show").removeClass('tr-active');
            setTop = 34 + parseInt($this.height() * ($this.index() / 2));
            $('.line-right').height(0).css("margin-top", "0px");
            self.chitietphieu = { modelChiTietCa: [], modelNhanVienCa: [] };
            if (css === 'none') {
                $this.addClass('tr-active');
                $this.next(".op-tr-hide").toggle();
                $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetChiTietPhieuPhanCa?idPhieu=" + item.ID, function (data) {
                    if (data.res) {
                        self.chitietphieu = data.dataSoure;
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(data.mess);
                    }
                    commonStatisJs.sleep(100).then(() => {

                        heightold = $this.next().height();
                        heigth = parseInt($this.height()) + heightold;
                        $('.line-right').height(heigth).css("margin-top", setTop + "px");
                        $this.closest('tbody').closest('table').closest('.table-reponsive').removeClass('tablescroll');
                    });
                });

            }
            else {
                if (!$this.closest('tbody').closest('table').closest('.table-reponsive').hasClass('tablescroll')) {
                    $this.closest('tbody').closest('table').closest('.table-reponsive').addClass('tablescroll');
                }
            }
        },
        AddTrangThai: function (ev) {
            var value = $(ev.target).val();
            if (this.curentpage.TrangThai.some(o => o === value)) {
                this.curentpage.TrangThai = this.curentpage.TrangThai.filter(o => o !== value);
            }
            else {
                this.curentpage.TrangThai.push(value);
            }
            this.GetForSearchPhanCa(true);
        },
        keymonitor: function (event) {
            if (event.key === "Enter") {
                this.GetForSearchPhanCa(true);
            }
        },
        SelectPageSize: function (e) {
            this.GetForSearchPhanCa(true);
        },
        SelectPage: function (item) {
            this.curentpage.pagenow = item;
            this.GetForSearchPhanCa();
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
            this.GetForSearchPhanCa();
        },
        // Ẩn hiện cột
        ShowColumn: function () {
            $(".dropdown-list").toggle();
        },
        LoadFirst: function () {
            var selft = this;
            var result = LocalCaches.LoadColumnGrid(selft.Key_Form);
            result.forEach(function (element) {
                $('.' + element.NameClass).hide();
                var model = selft.listcolumn.find(o => o.Key === element.NameClass);
                if (model !== null) {
                    model.Checked = false;
                }
            });
        },
        SelectColum: function (item, index, event) {
            var self = this;
            item.Checked = !item.Checked;
            LocalCaches.AddColumnHidenGrid(self.Key_Form, item.Key, index);
            $('.' + item.Key).toggle();
        },
        ChangeCheckedColum: function (item, index, event) {
            var self = this;
            LocalCaches.AddColumnHidenGrid(self.Key_Form, item.Key, index);
            $('.' + item.Key).toggle();
        },
        // Thêm sửa xóa
        AddNew: function () {
            vmmodalEditPhieuPhanca.AddNew();
        },
        Update: function (item) {
            vmmodalEditPhieuPhanca.Update(commonStatisJs.CopyObject(item), this.chitietphieu);
        },
        Delete: function (item) {
            vmmodalEditPhieuPhanca.DeletePhanCa(commonStatisJs.CopyObject(item));
        },

        Export: function () {
            var self = this;
            $('#table-reponsive').gridLoader();

            var arrDV = self.curentpage.ChiNhanh.filter(x => x.Checked).map(x => x.Id);
            if (arrDV.length === 0) {
                arrDV = [self.ID_DonVi];
            }

            var model = {
                Text: self.curentpage.text,
                TuNgay: self.curentpage.tungay,
                DenNgay: self.curentpage.denngay,
                TypeTime: self.curentpage.typetime,
                pageSize: self.databind.totalRows,
                pageNow: 1,
                TrangThai: self.curentpage.TrangThai.map(function (x) { return x }),
                ListDonVi: arrDV,
                LoaiCa: self.curentpage.LoaiCa.filter(o => o.Checked).map(o => o.Id),
                IDNhanVien: $('.idnhanvien').text(),
                TypeTimeNgayTao: self.curentpage.typetimeNgayTao,
                NgayTaoTu: self.curentpage.ngaytaoTu,
                NgayTaoDen: self.curentpage.ngaytaoDen,
            }
            console.log('modelExport ', model)

            $.ajax({
                data: model,
                url: "/api/DanhMuc/NS_NhanSuAPI/ExportExcelToPhieuPhanCa",
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
                },
                error: function (result) {
                    $('#table-reponsive').gridLoader({ show: false });
                    console.log(result);
                }
            });
        }
    },
    computed: {

        TypeLoaiCa: function () {
            return vmmodalEditPhieuPhanca.typeloaica;
        }
    },
});

$('.newDateTime').on('apply.daterangepicker', function (ev, picker) {
    vmPhieuPhanCa.curentpage.tungay = picker.startDate.format('MM/DD/YYYY');
    vmPhieuPhanCa.curentpage.denngay = picker.endDate.format('MM/DD/YYYY');
    vmPhieuPhanCa.GetForSearchPhanCa(true);
});

$('.newDateTimeNgayTao').on('apply.daterangepicker', function (ev, picker) {
    vmPhieuPhanCa.curentpage.ngaytaoTu = picker.startDate.format('MM/DD/YYYY');
    vmPhieuPhanCa.curentpage.ngaytaoDen = picker.endDate.format('MM/DD/YYYY');
    vmPhieuPhanCa.GetForSearchPhanCa(true);
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
        vmPhieuPhanCa.curentpage.typetime = null;
    }
    else {
        vmPhieuPhanCa.curentpage.typetime = vmPhieuPhanCa.curentpage.typetimeold;
    }
    vmPhieuPhanCa.GetForSearchPhanCa(true);
});
$('#SelectNgaySinh').on('click', 'ul li', function () {
    vmPhieuPhanCa.curentpage.textdate = $(this).find('a').text();
    vmPhieuPhanCa.curentpage.typetime = $(this).val();
    vmPhieuPhanCa.curentpage.typetimeold = $(this).val();
    vmPhieuPhanCa.GetForSearchPhanCa(true);
});

$('#divFilterNgayTao').on('click', '.radio-menu input[type="radio"]', function () {
    var dataid = $(this).data('id');
    $('#divFilterNgayTao .form-group').each(function () {
        $(this).find('.conten-choose').find('input').removeAttr('disabled');
        if (dataid !== $(this).find('.radio-menu').find('input').data('id')) {
            $(this).find('.conten-choose').find('input').attr('disabled', 'disabled');
        }
    });
    if ($(this).data('id') !== 1) {
        vmPhieuPhanCa.curentpage.typetimeNgayTao = null;
    }
    else {
        vmPhieuPhanCa.curentpage.typetimeNgayTao = vmPhieuPhanCa.curentpage.typetimeNgayTaoOld;
    }
    vmPhieuPhanCa.GetForSearchPhanCa(true);
});
$('#ddlNgayTao').on('click', 'ul li', function () {
    vmPhieuPhanCa.curentpage.textdateNgayTao = $(this).find('a').text();
    vmPhieuPhanCa.curentpage.typetimeNgayTao = $(this).val();
    vmPhieuPhanCa.curentpage.typetimeNgayTaoOld = $(this).val();
    vmPhieuPhanCa.GetForSearchPhanCa(true);
});
$('body').on('AddPhanCaLamViecSucces', function () {
    vmPhieuPhanCa.GetForSearchPhanCa(true);
    $(".op-tr-hide").hide();
    $(".op-tr-show").removeClass('tr-active');
    $('.line-right').height(0).css("margin-top", "0px");
});
$('body').on('AddCaLamViecSucces', function () {
    vmPhieuPhanCa.GetListCaLamViec();
    vmPhieuPhanCa.ResetChoseCa();
});

vmPhieuPhanCa.GetListData();
vmPhieuPhanCa.GetListCaLamViec();
vmPhieuPhanCa.GetForSearchPhanCa();