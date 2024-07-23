var vmChamCong = new Vue({
    el: '#ChamCong',
    data: {
        loadding: false,
        databind: {
            pageview: '',
            pagenow: 1,
            data: [],
            listpage: [],
            isprev: false,
            isnext: false,
            countpage: 0,
            sum: {}
        },
        curentpage: {
            text: '',
            tungay: '',
            denngay: '',
            typetime: 5,
            typetimeold: 5,
            TrangThai: ['1'],
            pagesize: 10,
            order: null,
            sort: false,
            pagenow: 1,
            textdate: 'Tháng này',
            chinhanhid: $('#hd_IDdDonVi').val(),
            kytinhcongId: null,
            kytinhcong: null,
            phongbanId: null
        },
        Key_Form: "KeyFormChamCong",
        listpagesize: [10, 20, 30],
        listdata: {
            ChiNhanh: [],
            PhongBan: [],
            Column: [],
            KyTinhCong: [],
            PhieuPhanCa: [],
            KyHieuCong: []
        },
        objectadd: {
            kytinhcongId: null,
        },
        changecong: {
            KyHieuCongId: null,
            CongBoSungId: null,
            SoPhutDiMuon: 0,
            SoGioOT: 0,
            TatCaNhanVien: false,
            Ngay: 1,
            IsNew: true,
            GhiChu: null,
            ItemNow: {},
        },
        LoaiCong: 1,
        chitietbangcong: {
            congBoSung: [],
            bangCongChinh: [],
            congOT: [],
        },
        ClickFilter: {
            BangCong: true,
            ChiNhanh: true,
            PhongBan: true,
            KyTinhCong: true
        }
    },
    methods: {
        GetListData: function () {
            var self = this;
            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetListChiNhanhNhanVien?id=" + $('.idnhanvien').text(), function (data) {
                if (data.res) {
                    self.listdata.ChiNhanh = data.dataSoure;
                    self.curentpage.chinhanhid = $('#hd_IDdDonVi').val();
                    self.GetPhongBan(self.curentpage.chinhanhid);
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });

            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetAllKyTinhCong", function (data) {
                if (data.res) {
                    self.listdata.KyTinhCong = data.dataSoure;
                    self.curentpage.kytinhcongId = data.dataSoure[0].ID;
                    self.objectadd.kytinhcongId = data.dataSoure[0].ID;
                    self.curentpage.kytinhcong = data.dataSoure[0];
                    self.GetForSearchChamCong(true);
                    self.LoadPPCAddHoSo();
                    self.GetKyHieuCongByKyTinhCong();
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });

        },

        GetKyHieuCongByKyTinhCong: function () {
            var self = this;
            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetForSearchKyHieuCongByKy?id=" + self.curentpage.kytinhcongId, function (data) {
                if (data.res) {
                    self.listdata.KyHieuCong = data.dataSoure;
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });
        },

        GetForSearchChamCong: function (resetpage = false) {
            var self = this;
            self.HideTabTableDetail();
            $('#table-reponsive').gridLoader();
            if (resetpage) {
                self.curentpage.pagenow = 1;
            }
            var model = {
                Text: self.curentpage.text,
                TypeTime: self.curentpage.typetime,
                pageSize: self.curentpage.pagesize,
                pageNow: self.curentpage.pagenow,
                ListDonVi: [self.curentpage.chinhanhid],
                KyTinhCongId: self.curentpage.kytinhcongId,
                PhonBanId: self.curentpage.phongbanId,
            }
            var url = "/api/DanhMuc/NS_NhanSuAPI/GetForSearchChamCong";
            if (self.LoaiCong === 2) {
                url = "/api/DanhMuc/NS_NhanSuAPI/GetForSearchBangCong";
            }
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
                        //commonStatisJs.sleep(100).then(() => {
                        //    self.LoadFirst();
                        //});
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

        SelectPageSize: function (e) {
            this.GetForSearchChamCong(true);

        },

        keymonitor: function (event) {
            if (event.key === "Enter") {
                this.GetForSearchChamCong(true);
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
            this.GetForSearchChamCong();
        },

        SelectPage: function (item) {
            this.curentpage.pagenow = item;
            this.GetForSearchChamCong();
        },
        // Change chi nhánh
        SelectChiNhanh: function () {
            if (this.curentpage.chinhanhid !== undefined && this.curentpage.chinhanhid !== null) {
                this.GetPhongBan(this.curentpage.chinhanhid);
                this.GetForSearchChamCong(true);
            }
        },
        // Get phòng ban theo chi nhánh
        GetPhongBan: function (id) {
            var self = this;
            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetListPhongBanTheoChiNhanh?id=" + id, function (data) {
                if (data.res) {
                    self.listdata.PhongBan = data.dataSoure;
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }

            });
        },
        // Get nhân viên theo phòng ban hay là chi nhánh nếu  isAll=true
        SelectNhanVienByPhongBan: function (event, id, isAll = false) {
            var self = this;
            $('.treename').removeClass('yellow');
            if (isAll) {
                $('.tatca').addClass('yellow');
                self.curentpage.phongbanId = null;
            }
            else {
                $(event.target).addClass('yellow');
                self.curentpage.phongbanId = id;
            }
            this.GetForSearchChamCong(true);
        },
        // Change kỳ tính công
        SelectKyTinhCong: function () {
            var self = this;
            self.curentpage.kytinhcong = self.listdata.KyTinhCong.filter(o => o.ID === self.curentpage.kytinhcongId)[0];
            self.GetKyHieuCongByKyTinhCong();
            this.GetForSearchChamCong(true);
        },

        ShowPopupImport: function () {

        },

        //Thêm hồ sơ chám công
        AddHoSoChamCong: function () {
            this.listdata.ChiNhanh.map(o => o.Checked = false);
            this.listdata.PhieuPhanCa.map(o => o.Checked = false);
            $('#modalPopupHoSoChamCong').modal('show');
        },

        SaveAddHoSoChamCong: function () {
            var self = this;
            var listPhanCa = self.listdata.PhieuPhanCa.filter(o => o.Checked).map(o => o.ID);
            if (listPhanCa.length <= 0) {
                commonStatisJs.ShowMessageDanger("Vui lòng chọn phiếu phân ca");
                return;
            }
            if (self.objectadd.kytinhcongId === null || self.objectadd.kytinhcongId === undefined) {
                commonStatisJs.ShowMessageDanger("Vui lòng chọn kỳ tính công");
                return;
            }

            self.loadding = true;
            var url = "/api/DanhMuc/NS_NhanSuAPI/InsertHoSoChamCong";
            var data = {
                IDKyTinhCong: self.objectadd.kytinhcongId,
                ListPhieuPhanCa: listPhanCa
            };
            $.ajax({
                data: data,
                url: url + "?" + "ID_DonVi=" + $('#hd_IDdDonVi').val() + "&ID_NhanVien=" + $('.idnhanvien').text(),
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    if (data.res === true) {
                        commonStatisJs.ShowMessageSuccess(data.mess);
                        $('#modalPopupHoSoChamCong').modal('hide');
                        self.GetForSearchChamCong(true);
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

        // Change chi nhánh ở thêm mới hồ sơ
        AddChangeChiNhanh: function (item) {
            item.Checked = !item.Checked;
            this.LoadPPCAddHoSo();

        },
        // Change kỳ tính công ở thêm mới hồ sơ
        AddChangeKyTinhCong: function () {
            this.LoadPPCAddHoSo();

        },
        // Load dữ liệu khi thêm hồ sơ chấm công
        LoadPPCAddHoSo: function () {
            var self = this;
            if (self.objectadd.kytinhcongId === null || self.objectadd.kytinhcongId === undefined) {
                return false;
            }
            else {
                var listChiNhanh = self.listdata.ChiNhanh.filter(o => o.Checked).map(o => o.Id);
                var kytinhcong = self.listdata.KyTinhCong.filter(o => o.ID === self.objectadd.kytinhcongId);
                var model = {
                    TuNgay: kytinhcong[0].TuNgay,
                    DenNgay: kytinhcong[0].DenNgay,
                    ListDonVi: listChiNhanh,
                    IDNhanVien: $('.idnhanvien').text()
                }
                $.ajax({
                    data: model,
                    url: "/api/DanhMuc/NS_NhanSuAPI/GetPhanCaByChiNhanh",
                    type: 'POST',
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (data) {
                        if (data.res === true) {
                            self.listdata.PhieuPhanCa = data.dataSoure;
                        }
                        else {
                            commonStatisJs.ShowMessageDanger(data.mess);
                        }
                    },
                    error: function (result) {
                        console.log(result);
                    }
                });
            }
        },
        // Chấm thủ công cho nhân vien
        CapNhatCong: function (item, ngay, giatri, event) {
            var self = this;
            var checkRole = parseInt($('#ID_RoleChamCong').val());
            if (self.curentpage.kytinhcong.TrangThai !== 2
                && checkRole === 1
                && ngay <= new Date(self.curentpage.kytinhcong.DenNgay).getDate()
                && ngay >= new Date(self.curentpage.kytinhcong.TuNgay).getDate()
            ) {
                var KyHieuCong = self.listdata.KyHieuCong.filter(o => o.KyHieu === giatri);
                self.changecong = {
                    KyHieuCongId: null,
                    SoPhutDiMuon: 0,
                    SoGioOT: 0,
                    Ngay: ngay,
                    TatCaNhanVien: false,
                    IsNew: true,
                    ItemNow: item,
                    CongBoSungId: null,
                    GhiChu: null
                };
                var $this = $(event.target);
                $('.popup-cham-cong').css('top', ($this.position().top + 90) + 'px');
                $('.popup-cham-cong').css('left', ($this.position().left) + 'px');
                if (KyHieuCong.length > 0) {
                    self.changecong.IsNew = false;
                    $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetCongBoSungByNgay?IdCong=" + item.ID + "&&date=" + item.Thang + "," + item.Nam + "," + ngay, function (data) {
                        if (data.res) {
                            self.changecong.GhiChu = data.dataSoure.GhiChu;
                            self.changecong.KyHieuCongId = KyHieuCong[0].ID;
                            self.changecong.CongBoSungId = data.dataSoure.ID;
                            self.changecong.SoGioOT = data.dataSoure.SoGioOT;
                            self.changecong.SoPhutDiMuon = data.dataSoure.SoPhutDiMuon;
                            $('.popup-cham-cong').show();
                        }
                        else {
                            commonStatisJs.ShowMessageDanger(data.mess);
                        }
                    });
                }
                else {
                    $('.popup-cham-cong').show();

                }
            }
        },

        SaveChangeCong: function () {
            var self = this;
            self.loadding = true;
            var search = {
                Text: self.curentpage.text,
                TypeTime: self.curentpage.typetime,
                pageSize: self.curentpage.pagesize,
                pageNow: self.curentpage.pagenow,
                ListDonVi: [self.curentpage.chinhanhid],
                KyTinhCongId: self.curentpage.kytinhcongId,
                PhonBanId: self.curentpage.phongbanId,
            };
            var kyhieucong = self.listdata.KyHieuCong.filter(o => o.ID === self.changecong.KyHieuCongId);
            if (kyhieucong.length <= 0) {
                commonStatisJs.ShowMessageDanger("Vui lòng chọn ký hiệu công");
                self.loadding = false;
                return
            }
            var model = {
                Search: search,
                ID_KyHieuCong: kyhieucong[0].ID,
                ID_ChiTietCong: self.changecong.ItemNow.ID,
                Ngay: self.changecong.Ngay,
                KyHieuCong: kyhieucong[0].KyHieu,
                SoGioOT: self.changecong.SoGioOT,
                SoPhutDiMuon: self.changecong.SoPhutDiMuon,
                Cong: kyhieucong[0].CongQuyDoi,
                ApDungToanNhanVien: self.changecong.TatCaNhanVien,
                IsNew: self.changecong.IsNew,
                MaNhanVien: self.changecong.ItemNow.MaNhanVien,
                TenNhanVien: self.changecong.ItemNow.TenNhanVien,
                ID_CongBoSung: self.changecong.CongBoSungId,
                GhiChu: self.changecong.GhiChu,
            }
            var url = "/api/DanhMuc/NS_NhanSuAPI/AddChamThuCong";
            $.ajax({
                data: model,
                url: url + "?" + "ID_DonVi=" + $('#hd_IDdDonVi').val() + "&ID_NhanVien=" + $('.idnhanvien').text(),
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    if (data.res === true) {
                        commonStatisJs.ShowMessageSuccess(data.mess);
                        self.GetForSearchChamCong();
                        $('.popup-cham-cong').hide();
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

        Export: async function () {
            var self = this;
            $('#table-reponsive').gridLoader();
            var model = {
                Text: self.curentpage.text,
                TypeTime: self.curentpage.typetime,
                pageSize: self.curentpage.pagesize,
                pageNow: self.curentpage.pagenow,
                ListDonVi: [self.curentpage.chinhanhid],
                KyTinhCongId: self.curentpage.kytinhcongId,
                PhonBanId: self.curentpage.phongbanId,
            }
            let fileName = "DanhSachChamCong_" + model.TuNgay.Value.ToString("MM_yyyy");
            await commonStatisJs.NPOI_ExportExcel(self.URLAPI_NHANSU + "ExportExcelToChamCong", 'POST', model, fileName);
            //$.ajax({
            //    data: model,
            //    url: "/api/DanhMuc/NS_NhanSuAPI/ExportExcelToChamCong",
            //    type: 'POST',
            //    dataType: 'json',
            //    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            //    success: function (data) {
            //        $('#table-reponsive').gridLoader({ show: false });
            //        if (data.res === true) {
            //            window.location.href = "/api/DanhMuc/NS_NhanSuAPI/DownloadFileExecl?fileSave=" + data.dataSoure;
            //        }
            //        else {
            //            commonStatisJs.ShowMessageDanger(data.mess);
            //        }
            //    },
            //    error: function (result) {
            //        $('#table-reponsive').gridLoader({ show: false });
            //        console.log(result);
            //    }
            //});
        },

        // Thay đổi loại báo cáo công
        ChangeLoaiCong: function (giatri) {
            if (this.LoaiCong !== giatri) {
                this.LoaiCong = giatri;
                this.GetForSearchChamCong(true);
            }
        },
        // click row bảng công
        SelectRow: function (item, event) {
            var self = this;
            var heigth = 0;
            var heightold = 0;
            var setTop = 0;
            var $this = $(event.target).closest('tr');
            var css = $this.next(".op-tr-hide").css('display');
            $(".op-tr-hide").hide();
            $(".op-tr-show").removeClass('tr-active');
            setTop = 51 + parseInt($this.height() * ($this.index() / 2));
            if (css === 'none') {
                $this.addClass('tr-active');
                $this.next(".op-tr-hide").toggle();
                $('#table-reponsive').gridLoader();
                var model = {
                    ListDonVi: item.ListID
                };
                $.ajax({
                    data: model,
                    url: "/api/DanhMuc/NS_NhanSuAPI/GetChiTietBangCong",
                    type: 'POST',
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (data) {
                        $('#table-reponsive').gridLoader({ show: false });

                        if (data.res === true) {
                            self.chitietbangcong = data.dataSoure;
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
                    }
                });

            }
            else {
                $('.line-right').height(0).css("margin-top", "0px");
                if (!$this.closest('tbody').closest('table').closest('.table-reponsive').hasClass('tablescroll')) {
                    $this.closest('tbody').closest('table').closest('.table-reponsive').addClass('tablescroll');
                }
            }
        },

        HideTabTableDetail: function () {
            $(".op-tr-hide").hide();
            $(".op-tr-show").removeClass('tr-active');
            $('.line-right').height(0).css("margin-top", "0px");
            if (!$('.table-reponsive').hasClass('tablescroll')) {
                $('.table-reponsive').addClass('tablescroll');
            }
        },

        ChangeTabBangCong: function (event) {
            var self = this;
            var heigth = 0;
            var setTop = 0;
            var $this = $(event.target).closest('ul').closest('ul').closest('tr').prev();
            setTop = 51 + parseInt($this.height() * ($this.index() / 2));
            commonStatisJs.sleep(100).then(() => {
                heigth = parseInt($this.height()) + $this.next().height();
                $('.line-right').height(heigth).css("margin-top", setTop + "px");
            });
        },
        ExportBangCong: function (item) {
            var model = {
                ListDonVi: item.ListID,
                IDNhanVien: item.ID_NhanVien,
                KyTinhCongId: this.curentpage.kytinhcongId,
            };
            $('#table-reponsive').gridLoader();
            $.ajax({
                data: model,
                url: "/api/DanhMuc/NS_NhanSuAPI/ExportBangCong",
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
        }
    },
    computed: {
        ListThang: function () {
            var array = [];
            var date = new Date();
            if (this.curentpage.kytinhcong !== null) {
                date = new Date(this.curentpage.kytinhcong.TuNgay);
            }
            var lastDay = new Date(date.getFullYear(), date.getMonth() + 1, 0).getDate();
            for (var i = 0; i < lastDay; i++) {
                array.push((i + 1));
            }
            return array;
        },
        TitleThang: function () {
            var date = new Date();
            if (this.curentpage.kytinhcong !== null) {
                date = new Date(this.curentpage.kytinhcong.TuNgay);
            }
            return "Tháng " + (date.getMonth() + 1) + " Năm " + date.getFullYear();

        }
    },
});
vmChamCong.GetListData();
$(document).mouseup(function () {
    $('.popup-cham-cong').mouseup(function () {
        return false
    });
    $('.popup-cham-cong').hide();
});