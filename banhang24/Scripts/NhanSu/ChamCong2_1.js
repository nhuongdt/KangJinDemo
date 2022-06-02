var vmChamCong = new Vue({
    el: '#ChamCong',
    components: {
        'component-search-date': ComponentSearchDate
    },
    props: {
        textdate: {
            type: String,
        },
        typetime: {
            type: Number,
        },
        typetimeold: {
            type: Number,
        },
        textdaterange: {
            type: String,
        },
    },
    created: function () {
        var self = this;
        self.URLAPI_NHANSU = '/api/DanhMuc/NS_NhanSuAPI/';
        self.URLAPI_NHANVIEN = '/api/DanhMuc/NS_NhanVienAPI/';
    },
    data: {
        loadding: false,
        ID_NhanVien: $('.idnhanvien').text(),
        databind: {
            pageview: '',
            pagenow: 1,
            data: [],
            listpage: [],
            isprev: false,
            isnext: false,
            countpage: 0,
            sum: {},
        },
        BangLuongChiTiet: [],

        curentpage: {
            text: '',
            tungay: moment().startOf('month').format('YYYY-MM-DD'),
            denngay: moment().endOf('month').format('YYYY-MM-DD'),
            typetime: 0,
            typetimeold: 5, // hôm nay --> năm này
            TrangThai: '1', // 2.all, 0.da nghiviec, 1.dang lam
            pagesize: 30,
            order: null,
            sort: false,
            pagenow: 1,
            totalRow: 0,
            textdate: 'Tháng này',
            textdaterange: moment().startOf('week').format('DD/MM/YYYY') + ' - ' + moment().endOf('week').format('DD/MM/YYYY'),
            chinhanhid: $('#hd_IDdDonVi').val(),
            kytinhcongId: null,
            kytinhcong: null,
            phongbanId: null,
            month: (new Date()).getMonth(),
            year: (new Date()).getFullYear(),

            TenCaChosing: '--Chọn ca --',
        },
        Key_Form: "KeyFormChamCong",
        listpagesize: [30, 50, 100],
        listdata: {
            ChiNhanh: [],
            PhongBan: [],
            Column: [],
            KyTinhCong: [],
            PhieuPhanCa: [],
            KyHieuCong: [],
            CaLamViec: [],

        },
        objectadd: {
            kytinhcongId: null,
        },
        changecong: {
            KyHieuCongId: null,
            CongBoSungId: null,
            SoPhutDiMuon: '',
            SoGioOT: '',
            TatCaNhanVien: false,
            Ngay: 1,
            IsNew: true,
            GhiChu: null,
            TrangThaiCong: 1, // 1.taomoi, 2.tamluu bangluong ,3.daduyetbangluong = dachot bangluong, 4. dathanhtoan bangluong
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
            KyTinhCong: true,
            CaLamViec: false,
        },
        Role: {
            CaLamViec: {
                View: false,
                Insert: false,
            }
        }
    },
    methods: {
        KhoiTaoDuLieuChamCong: function () {
            var self = this;
            $.getJSON(self.URLAPI_NHANSU + "CheckKhoiTaoDuLieuChamCong", function (data) {
                if (data.res) {
                    if (!data.dataSoure) {
                        commonStatisJs.ConfirmDialog_OKCancel('Khởi tạo dữ liệu chấm công',
                            'Bạn có muốn khởi tạo dữ liệu chấm công gồm <b> Danh mục ngày, ký hiệu công </b> không?',
                            function () {
                                $.getJSON(self.URLAPI_NHANSU + "SetUpThamSoCong", function (data) {
                                    if (data.res) {
                                        $('#modalPopuplgDelete').modal('hide');
                                        commonStatisJs.ShowMessageSuccess(data.mess);
                                    }
                                    else {
                                        commonStatisJs.ShowMessageDanger(data.mess);
                                    }
                                });
                            }, function () {
                                $('#modalPopuplgDelete').modal('hide');
                                return false;
                            });
                    }
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });
        },
        ChangeKyHieuCong: function (item) {
            var self = this;
            if (self.changecong.KyHieuCongId !== undefined) {
                let congQuyDoi = this.listdata.KyHieuCong.filter(o => o.ID === self.changecong.KyHieuCongId)[0];
                if (congQuyDoi.CongQuyDoi === 0) {
                    self.changecong.SoGioOT = 0;
                    self.changecong.SoPhutDiMuon = 0;
                    self.changecong.ShowOT = false;
                }
                else {
                    self.changecong.ShowOT = true;
                }
            }
        },

        ChangeTime: function (objChild) {
            var self = this;
            self.curentpage.pagenow = 1;
            self.curentpage.textdate = objChild.Text;
            self.curentpage.tungay = objChild.FromDate;
            self.curentpage.denngay = objChild.ToDate;
            self.curentpage.typetime = 0;
            self.curentpage.typetimeold = objChild.TypeTimeLi;
            self.curentpage.month = new Date(self.curentpage.tungay).getMonth();
            self.curentpage.year = new Date(self.curentpage.tungay).getFullYear();
            self.GetForSearchChamCong();
        },
        ChangeDateRange: function () {
            var self = this;
            self.curentpage.typetime = 1;
            self.GetForSearchChamCong();
        },

        GetListCaLamViec: function () {
            var self = this;
            $.getJSON(self.URLAPI_NHANSU + "GetListCaTheoChiNhanh?id=" + self.curentpage.chinhanhid, function (data) {
                if (data.res) {
                    vmDanhMucCa.listdata.CaLamViec = data.dataSoure;
                    self.listdata.CaLamViec = data.dataSoure.filter(x => x.TrangThai !== 0);
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });
        },

        GetListData: function () {
            var self = this;
            $.getJSON(self.URLAPI_NHANSU + "GetListChiNhanhNhanVien?id=" + $('.idnhanvien').text(), function (data) {
                if (data.res) {
                    self.listdata.ChiNhanh = data.dataSoure;
                    vmEditCaLamViec.listchinhanh = data.dataSoure;
                    self.GetPhongBan(self.curentpage.chinhanhid);
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });

            // get all kyhieucong
            $.getJSON(self.URLAPI_NHANSU + "GetForSearchKyHieuCong?idDonVi=" + self.curentpage.chinhanhid, function (data) {
                if (data.res) {
                    self.listdata.KyHieuCong = data.dataSoure;
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });
        },

        ChangeTrangThaiNV: function (val) {
            var self = this;
            self.curentpage.TrangThai = val.toString();
            self.GetForSearchChamCong();
        },

        GetForSearchChamCong: function (resetpage = false) {
            var self = this;
            self.HideTabTableDetail();
            $('#table-reponsive').gridLoader();
            if (resetpage) {
                self.curentpage.pagenow = 1;
            }
            let where = '';
            let text = self.curentpage.text;
            if (!commonStatisJs.CheckNull(text)) {
                text = text.trim()
            }
            var model = {
                Text: text,
                TypeTime: self.curentpage.typetime,
                pageSize: self.curentpage.pagesize,
                pageNow: self.curentpage.pagenow - 1,
                ListDonVi: [self.curentpage.chinhanhid],
                PhonBanId: self.curentpage.phongbanId,
                ID_CalamViec: null,
                TuNgay: self.curentpage.tungay,
                DenNgay: self.curentpage.denngay,
                ListCa: self.listdata.CaLamViec.filter(x => x.Checked).map(function (x) { return x.Id }),
                IDNhanVien: self.ID_NhanVien,
                TrangThai: self.GetTrangThaiNV(),
            }
            var url = self.URLAPI_NHANSU + "GetForSearchChamCong";
            if (self.LoaiCong === 2) {
                url = self.URLAPI_NHANSU + "GetForSearchBangCong";
            }
            $.ajax({
                data: model,
                url: url,
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (x) {
                    $('#table-reponsive').gridLoader({ show: false });
                    console.log(x)
                    if (x.res === true) {
                        var result = x.dataSoure;
                        self.databind = result;
                        self.databind.listpage = result.ListPage;
                        self.databind.pageview = result.PageView;
                        self.databind.countpage = result.NumOfPage;
                        self.databind.pagenow = self.curentpage.pagenow;
                        self.databind.sum = result.sum;
                        self.curentpage.totalRow = result.TotalRow;
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(x.mess);
                    }
                },
                error: function (result) {
                    $('#table-reponsive').gridLoader({ show: false });
                }
            });
        },

        GetChiTietBangCong: function (item) {
            var self = this;
            var model = {
                pageSize: 100,
                pageNow: self.curentpage.pagenow - 1,
                ListDonVi: [self.curentpage.chinhanhid],
                ListCa: [],
                IDNhanVien: item.ID_NhanVien,
                TuNgay: self.curentpage.tungay,
                DenNgay: self.curentpage.denngay
            }
            $.ajax({
                data: model,
                url: self.URLAPI_NHANSU + 'GetChiTietBangCong',
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            }).done(function (x) {
                if (x.res === true) {
                    var result = x.dataSoure;
                    self.chitietbangcong = result;
                }
                else {
                    commonStatisJs.ShowMessageDanger(x.mess);
                }
            }).always(function () {
                $('#table-reponsive').gridLoader({ show: false });
            });
        },

        GetChiTietBangLuong: function (item) {
            var self = this;
            var model = {
                PageSize: self.curentpage.pagesize,
                CurrentPage: self.curentpage.pagenow - 1,
                LstIDChiNhanh: [self.curentpage.chinhanhid],
                LstKieuLuong: [],
                LstIDNhanVien: [item.ID_NhanVien],
                FromDate: self.curentpage.tungay,
                ToDate: self.curentpage.denngay
            }

            $.ajax({
                data: model,
                url: self.URLAPI_NHANSU + 'GetBangLuongChiTiet_ofNhanVien',
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            }).done(function (x) {
                if (x.res === true) {
                    self.BangLuongChiTiet = x.dataSoure.data;
                }
                else {
                    commonStatisJs.ShowMessageDanger(x.mess);
                }
            }).always(function () {
                $('#table-reponsive').gridLoader({ show: false });
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
            $.getJSON(self.URLAPI_NHANSU + "GetListPhongBanTheoChiNhanh?id=" + id, function (data) {
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

        RemoveCong_fromDB: function (item) {
            var self = this;
            $.getJSON(self.URLAPI_NHANSU + 'RemoveCong_ofNhanVien?idChiNhanh=' + self.curentpage.chinhanhid + '&idNhanVien=' + item.ID_NhanVien
                + '&idCaLamViec=' + item.ID_CaLamViec
                + '&fromdate=' + self.curentpage.tungay
                + '&todate=' + self.curentpage.denngay, function (x) {
                    if (x.res === true) {
                        commonStatisJs.ShowMessageSuccess(x.mess);
                        self.GetForSearchChamCong();
                    }
                });
        },

        RemoveCongNV: function (item) {
            var self = this;
            $.getJSON(self.URLAPI_NHANSU + 'XoaCong_CheckExistBangLuong?idNhanVien=' + item.ID_NhanVien
                + '&fromdate=' + self.curentpage.tungay
                + '&todate=' + self.curentpage.denngay, function (x) {
                    if (x.res === true) {
                        var mes = '';
                        var trangthai = 0;
                        if (x.dataSoure !== null && x.dataSoure.length > 0) {
                            trangthai = x.dataSoure.Data[0].TrangThai;
                            switch (trangthai) {
                                case 1:
                                case 2:
                                    mes = 'Công thuộc bảng lương tạm lưu. ';
                                    break;
                                case 3:
                                    mes = 'Công thuộc bảng lương đã chốt. Không thể xóa';
                                    break;
                                case 4:
                                    mes = 'Công thuộc bảng lương đã thanh toán. Không thể xóa';
                                    break;
                            }
                        }

                        if (trangthai > 2) {
                            commonStatisJs.ShowMessageDanger(mes);
                            return;
                        }

                        let ct = item.TenNhanVien.concat(' </b>  từ ngày <b>', moment(self.curentpage.tungay, 'YYYY-MM-DD').format('DD/MM/YYYY'),
                            ' </b> đến ngày <b>', moment(self.curentpage.denngay, 'YYYY-MM-DD').format('DD/MM/YYYY'));
                        commonStatisJs.ConfirmDialog_OKCancel('Xóa công', mes.concat('Bạn có chắc chắn muốn xóa công của nhân viên <b> ', ct, ' không?'),
                            function () {
                                vmChamCong.RemoveCong_fromDB(item);

                                ct = 'Xóa công nhân viên '.concat(ct);
                                var diary = {
                                    ID_DonVi: self.curentpage.chinhanhid,
                                    ID_NhanVien: self.ID_NhanVien,
                                    ChucNang: 'Chấm công',
                                    LoaiNhatKy: 3,
                                    NoiDung: ct,
                                    NoiDungChiTiet: ct,
                                }

                                var myDataNK = {};
                                myDataNK.objDiary = diary;
                                $.ajax({
                                    url: '/api/DanhMuc/SaveDiary/' + "post_NhatKySuDung",
                                    type: 'POST',
                                    async: true,
                                    dataType: 'json',
                                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                                    data: myDataNK,
                                });
                                $('#modalPopuplgDelete').modal('hide');
                            }, function () {
                                $('#modalPopuplgDelete').modal('hide');
                                return false;
                            });
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(x.mess);
                    }
                });
        },

        // Chấm thủ công cho nhân vien
        CapNhatCong: function (item, ngay, giatri, event) {
            var self = this;
            var KyHieuCong = self.listdata.KyHieuCong.filter(o => o.KyHieu === giatri);
            if (KyHieuCong.length > 0) {
                let roleUpdate = parseInt($('#ChamCong_Update').val());
                if (roleUpdate === 0) {
                    commonStatisJs.ShowMessageDanger('Không có quyền cập nhật công');
                    return false;
                }
            }
            else {
                let roleInsert = parseInt($('#ChamCong_Insert').val());
                if (roleInsert === 0) {
                    commonStatisJs.ShowMessageDanger('Không có quyền chấm công');
                    return false;
                }
            }

            var dateChose = moment(new Date(item.Nam, item.Thang - 1, ngay)).format('YYYYMMDD');
            var dateNow = moment(new Date()).format('YYYYMMDD');
            if (dateChose > dateNow) {
                commonStatisJs.ShowMessageDanger('Ngày chấm vượt quá ngày hiện tại');
                return false;
            }
            // check nếu ca thuộc phiếu phân ca không thuộc khoảng thời gian chấm công
            var phieuFrom = moment(item.TuNgay).format('YYYY-MM-DD');
            var phieuTo = item.DenNgay;
            var ngayChosing = moment(new Date(self.curentpage.year, self.curentpage.month, ngay)).format('YYYY-MM-DD');
            var mes = 'Chưa phân ca cho nhân viên ' + item.TenNhanVien + ' trong khoảng thời gian này';
            var disNgay = '0';
            switch (ngay) {
                case 1:
                    disNgay = item.DisNgay1;
                    break;
                case 2:
                    disNgay = item.DisNgay2;
                    break;
                case 3:
                    disNgay = item.DisNgay3;
                    break;
                case 4:
                    disNgay = item.DisNgay4;
                    break;
                case 5:
                    disNgay = item.DisNgay5;
                    break;
                case 6:
                    disNgay = item.DisNgay6;
                    break;
                case 7:
                    disNgay = item.DisNgay7;
                    break;
                case 8:
                    disNgay = item.DisNgay8;
                    break;
                case 9:
                    disNgay = item.DisNgay9;
                    break;
                case 10:
                    disNgay = item.DisNgay10;
                    break;
                case 11:
                    disNgay = item.DisNgay11;
                    break;
                case 12:
                    disNgay = item.DisNgay12;
                    break;
                case 13:
                    disNgay = item.DisNgay13;
                    break;
                case 14:
                    disNgay = item.DisNgay14;
                    break;
                case 15:
                    disNgay = item.DisNgay15;
                    break;
                case 16:
                    disNgay = item.DisNgay16;
                    break;
                case 17:
                    disNgay = item.DisNgay17;
                    break;
                case 18:
                    disNgay = item.DisNgay18;
                    break;
                case 19:
                    disNgay = item.DisNgay19;
                    break;
                case 20:
                    disNgay = item.DisNgay20;
                    break;
                case 21:
                    disNgay = item.DisNgay21;
                    break;
                case 22:
                    disNgay = item.DisNgay22;
                    break;
                case 23:
                    disNgay = item.DisNgay23;
                    break;
                case 24:
                    disNgay = item.DisNgay24;
                    break;
                case 25:
                    disNgay = item.DisNgay25;
                    break;
                case 26:
                    disNgay = item.DisNgay26;
                    break;
                case 27:
                    disNgay = item.DisNgay27;
                    break;
                case 28:
                    disNgay = item.DisNgay28;
                    break;
                case 29:
                    disNgay = item.DisNgay29;
                    break;
                case 30:
                    disNgay = item.DisNgay30;
                    break;
                case 31:
                    disNgay = item.DisNgay31;
                    break;
            }

            if (disNgay === '1') {
                commonStatisJs.ShowMessageDanger(mes);
                return false;
            }

            var KyHieuCong = self.listdata.KyHieuCong.filter(o => o.KyHieu === giatri);
            var $this = $(event.target);

            //$('.popup-cham-cong').css('top', ($this.position().top + 155) + 'px');
            //$('.popup-cham-cong').css('left', ($this.position().left) + 'px');
            var currentScrollLeft = $("#tableChamCong").scrollLeft();
            var currentScrollTop = $("#tableChamCong").scrollTop();
            var frameWidth = $("#tableChamCong").width();
            var frameHeight = $("#tableChamCong").height() + 7;

            var tableWidth = $("#tableChamCong table").width();
            var tableHeight = $("#tableChamCong table").height();
            var ResultPositionLeft, ResultPositionRight;

            if ($this.position().top > frameHeight / 2) { //góc phần 2 phía trên
                $('.componnet-follow').removeClass("y1");
                $('.componnet-follow').addClass("y2");
                ResultPositionRight = ($this.position().top + currentScrollTop - 239);
            } else {
                //góc phần 2 phía dưới
                $('.componnet-follow').removeClass("y2");
                $('.componnet-follow').addClass("y1");
                ResultPositionRight = ($this.position().top + currentScrollTop + 30);
            }

            if ($this.position().left > frameWidth / 2) { //góc phần 2 phía trái
                ResultPositionLeft = ($this.position().left + currentScrollLeft) - 410;
                $('.componnet-follow').removeClass("x1");
                $('.componnet-follow').addClass("x2");
            } else {
                ResultPositionLeft = ($this.position().left + currentScrollLeft); //góc phần 2 phía phải
                $('.componnet-follow').removeClass("x2");
                $('.componnet-follow').addClass("x1");
            }

            $('.componnet-follow').css('top', ResultPositionRight + 'px');
            $('.componnet-follow').css('left', ResultPositionLeft + 'px');

            if (KyHieuCong.length > 0) {
                if (KyHieuCong[0].CongQuyDoi === 0) {
                    self.changecong.ShowOT = false;
                }
                else {
                    self.changecong.ShowOT = true;// hiển thị giờ OT/đi muộn
                }
                self.changecong.ItemNow = item;
                self.changecong.Ngay = ngay;
                self.changecong.IsNew = false;
                self.changecong.TatCaNhanVien = false;

                $.getJSON(self.URLAPI_NHANSU + "GetCongBoSungByNgay?idDonVi=" + self.curentpage.chinhanhid
                    + '&idNhanVien=' + self.changecong.ItemNow.ID_NhanVien
                    + '&idCaLamViec=' + self.changecong.ItemNow.ID_CaLamViec
                    + "&date=" + item.Thang + "," + item.Nam + "," + ngay, function (data) {
                        if (data.res && data.dataSoure !== null) {
                            if (data.dataSoure.TrangThai === 3 || data.dataSoure.TrangThai === 4) {
                                commonStatisJs.ShowMessageDanger('Công đã tạo bảng lương. Không thể cập nhật');
                                return false;
                            }
                            self.changecong.GhiChu = data.dataSoure.GhiChu;
                            self.changecong.KyHieuCongId = KyHieuCong[0].ID;
                            self.changecong.CongBoSungId = data.dataSoure.ID;
                            self.changecong.SoGioOT = data.dataSoure.SoGioOT;
                            self.changecong.SoPhutDiMuon = data.dataSoure.SoPhutDiMuon;
                            self.changecong.TrangThaiCong = data.dataSoure.TrangThai;
                            $(".componnet-follow").show();
                        }
                        else {
                            commonStatisJs.ShowMessageDanger(data.mess);
                        }
                    });
            }
            else {
                self.changecong = {
                    KyHieuCongId: null,
                    SoPhutDiMuon: 0,
                    SoGioOT: 0,
                    Ngay: ngay,
                    TatCaNhanVien: false,
                    IsNew: true,
                    ItemNow: item,
                    CongBoSungId: null,
                    GhiChu: null,
                    ShowOT: true,
                    TrangThaiCong: 1,
                };
                $(".componnet-follow").show();
            }
        },

        FormatDate: function (year, month, day) {
            var yyyyMMdd = moment(new Date(year, month - 1, day)).format('YYYY-MM-DD');
            return yyyyMMdd;
        },

        SaveCongToDB: function (kyhieucong) {
            var self = this;
            self.loadding = true;

            var search = {
                Text: self.curentpage.text,
                TypeTime: self.curentpage.typetime,
                pageSize: self.curentpage.totalRow,
                pageNow: 0,
                ListDonVi: [self.curentpage.chinhanhid],
                KyTinhCongId: self.curentpage.kytinhcongId,
                PhonBanId: self.curentpage.phongbanId,
                IDNhanVien: self.changecong.ItemNow.ID_NhanVien,
                ID_CaLamViec: self.changecong.ItemNow.ID_CaLamViec,
                TuNgay: self.curentpage.tungay,
                DenNgay: self.curentpage.denngay,
                ListCa: [self.changecong.ItemNow.ID_CaLamViec],
                TrangThai: self.GetTrangThaiNV(),
            };

            var model = {
                Search: search,
                ID_KyHieuCong: kyhieucong[0].ID,
                ID_ChiTietCong: self.changecong.ItemNow.ID,
                ID_DonVi: self.curentpage.chinhanhid,
                ID_NhanVien: self.changecong.ItemNow.ID_NhanVien,
                ID_CaLamViec: self.changecong.ItemNow.ID_CaLamViec,
                Ngay: self.changecong.Ngay,
                Thang: self.changecong.ItemNow.Thang,
                Nam: self.changecong.ItemNow.Nam,
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
                MaCa: self.changecong.ItemNow.MaCa,
                TenCa: self.changecong.ItemNow.TenCa,
            }

            var url = self.URLAPI_NHANSU + "AddChamThuCong";
            $.ajax({
                data: model,
                url: url + "?" + "ID_DonVi=" + self.curentpage.chinhanhid + "&ID_NhanVien=" + $('.idnhanvien').text(),
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    if (data.res === true) {
                        commonStatisJs.ShowMessageSuccess(data.mess);

                        // update trangthai in ns_phieuphanca
                        $.getJSON(self.URLAPI_NHANSU + 'ChamCong_UpdatePhieuPhanCa?idPhieuPC=' + self.changecong.ItemNow.ID_PhieuPhanCa, function () {

                        });

                        self.GetForSearchChamCong();

                    }
                    else {
                        commonStatisJs.ShowMessageDanger(data.mess);
                    }
                    self.loadding = false;
                    $(".componnet-follow").hide();
                },
                error: function (result) {
                    self.loadding = false;
                }
            });
        },

        SaveChangeCong: function () {
            var self = this;
            if (self.changecong.TrangThaiCong === 3) {
                commonStatisJs.ShowMessageDanger("Công đã chốt lương, Không thể thay đổi");
                return false;
            }

            var kyhieucong = self.listdata.KyHieuCong.filter(o => o.ID === self.changecong.KyHieuCongId);
            if (kyhieucong.length <= 0) {
                commonStatisJs.ShowMessageDanger("Vui lòng chọn ký hiệu công");
                return
            }
            self.loadding = true;

            $.getJSON(self.URLAPI_NHANSU + 'CheckCong_ExistBangLuong?idChiNhanh=' + self.curentpage.chinhanhid + '&ngay=' + self.changecong.Ngay
                + '&thang=' + self.changecong.ItemNow.Thang
                + '&nam=' + self.changecong.ItemNow.Nam, function (x) {
                    if (x.res === true) {
                        if (x.dataSoure === true) {
                            commonStatisJs.ConfirmDialog_OKCancel('Cập nhật công', 'Các thay đổi sẽ ảnh hưởng đến bảng lương tạm. Bạn có chắc chắn muốn cập nhật không?',
                                function () {
                                    // cập nhật công + update TrangThai BangLuong
                                    self.SaveCongToDB(kyhieucong);
                                    $('#modalPopuplgDelete').modal('hide');
                                }, function () {
                                    $('#modalPopuplgDelete').modal('hide');
                                    self.loadding = false;
                                    // không làm gì cả
                                    return false;
                                });
                        }
                        else {
                            self.SaveCongToDB(kyhieucong);
                        }
                    }
                    else {
                        self.loadding = false;
                    }
                });
        },

        GetTrangThaiNV: function () {
            var self = this;
            var arrTTNV = [0, 1];
            switch (self.trangthaiNV) {
                case 1:
                    arrTTNV = [1];
                    break;
                case 0:
                    arrTTNV = [0];
                    break;
            }
            return arrTTNV;
        },

        Export: function () {
            var self = this;
            $('#table-reponsive').gridLoader();
            var model = {
                Text: self.curentpage.text,
                TypeTime: self.curentpage.typetime,
                pageSize: 200,
                pageNow: self.curentpage.pagenow - 1,
                ListDonVi: [self.curentpage.chinhanhid],
                PhonBanId: self.curentpage.phongbanId,
                ID_CalamViec: null,
                TuNgay: self.curentpage.tungay,
                DenNgay: self.curentpage.denngay,
                ListCa: self.listdata.CaLamViec.filter(x => x.Checked).map(function (x) { return x.Id }),
                TrangThai: self.GetTrangThaiNV(),
            }
            $.ajax({
                data: model,
                url: self.URLAPI_NHANSU + "ExportExcelToChamCong",
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    $('#table-reponsive').gridLoader({ show: false });
                    if (data.res === true) {
                        window.location.href = self.URLAPI_NHANSU + "DownloadFileExecl?fileSave=" + data.dataSoure;
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(data.mess);
                    }
                },
                error: function (result) {
                    $('#table-reponsive').gridLoader({ show: false });
                }
            });
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
                self.GetChiTietBangCong(item);
            }
            else {
                $('.line-right').height(0).css("margin-top", "0px");
                if (!$this.closest('tbody').closest('table').closest('.table-reponsive').hasClass('tablescroll')) {
                    $this.closest('tbody').closest('table').closest('.table-reponsive').addClass('tablescroll');
                }
            }
            // active tab first
            var tabContent = $this.closest('tbody').find('.tab-content');
            $(tabContent).prev().find('li').removeClass('active');
            $(tabContent).prev().find('li').eq(0).addClass('active');

            $(tabContent).children('div').removeClass('active');
            $(tabContent).children('div').eq(0).addClass('active');
        },

        HideTabTableDetail: function () {
            $(".op-tr-hide").hide();
            $(".op-tr-show").removeClass('tr-active');
            $('.line-right').height(0).css("margin-top", "0px");
            if (!$('.table-reponsive').hasClass('tablescroll')) {
                $('.table-reponsive').addClass('tablescroll');
            }
        },

        ChangeTabBangCong: function (event, item) {
            var self = this;
            var heigth = 0;
            var setTop = 0;
            var $this = $(event.target).closest('ul').closest('ul').closest('tr').prev();
            setTop = 51 + parseInt($this.height() * ($this.index() / 2));
            commonStatisJs.sleep(100).then(() => {
                heigth = parseInt($this.height()) + $this.next().height();
                $('.line-right').height(heigth).css("margin-top", setTop + "px");
            });
            var valLi = $(event.target).closest('li').val();
            if (parseInt(valLi) === 3) {
                self.GetChiTietBangLuong(item);
            }
        },
        ShowModalChiTietLuong: function (item) {
            // todo
        },

        ExportBangCong: function (item) {
            var self = this;
            var model = {
                ListDonVi: [self.curentpage.chinhanhid],
                IDNhanVien: item.ID_NhanVien,
                TuNgay: self.curentpage.tungay,
                DenNgay: self.curentpage.denngay,
                ListCa: [],
                pageSize: 0,
                pageNow: 100,
                Sort: false,
            };
            $('#table-reponsive').gridLoader();
            $.ajax({
                data: model,
                url: self.URLAPI_NHANSU + "ExportBangCong",
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    $('#table-reponsive').gridLoader({ show: false });

                    if (data.res === true) {
                        window.location.href = self.URLAPI_NHANSU + "DownloadFileExecl?fileSave=" + data.dataSoure;
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(data.mess);
                    }
                }
            });
        },

        // themmoi phieuphanca
        AddPhieuPhanCa: function () {
            var self = this;
            vmmodalEditPhieuPhanca.AddNew();
        },
        ShowModalKyHeuCong: function () {
            vmKyHieuCong.ModalShow();
        },
        ShowModalNgayNghiLe: function () {
            vmNgayNghiLe.ModalShow();
        },
        ShowModalDanhMucCa: function () {
            vmDanhMucCa.listdata.ChiNhanh = commonStatisJs.CopyArray(this.listdata.ChiNhanh);
            vmDanhMucCa.ModalShow();
        },
        ResetChoseCa: function () {
            var self = this;
            for (let i = 0; i < self.listdata.CaLamViec.length; i++) {
                self.listdata.CaLamViec[i].Checked = false;
            }
            self.curentpage.TenCaChosing = '--Chọn ca --';
            self.ClickFilter.CaLamViec = false;
            self.GetForSearchChamCong();
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
            self.GetForSearchChamCong();
        },
        AddNewCaLamViec: function () {
            vmEditCaLamViec.AddNew();
        },
        EditCaLamViec: function () {
            var self = this;
            var caChosed = self.listdata.CaLamViec.filter(x => x.Checked);
            caChosed[0].MaCa = caChosed[0].Ma;
            caChosed[0].TenCa = caChosed[0].Ten;
            caChosed[0].TrangThai = '1';
            vmEditCaLamViec.Edit(caChosed[0]);
        },
    },
    computed: {
        ListThang: function () {
            let array = [];
            let month = this.curentpage.month;
            let year = this.curentpage.year;
            let fromDate = new Date(this.curentpage.tungay);
            let toDate = new Date(this.curentpage.denngay);
            let monthFrom = fromDate.getMonth();
            let monthTo = toDate.getMonth();
            let yearFrom = fromDate.getMonth();
            let yearTo = toDate.getMonth();
            let end = new Date(year, month + 1, 0).getDate() + 1;
            let start = 1;
            let ngaynghi = [];
            if (yearFrom === yearTo) {
                if (monthFrom === monthTo) {
                    start = fromDate.getDate();
                    end = toDate.getDate() + 1;
                    month = monthFrom;

                    ngaynghi = $.grep(vmNgayNghiLe.databind.data, function (x) {
                        return x.LoaiNgay !== 0 && x.Ngay !== null && (new Date(x.Ngay)).getMonth() === monthFrom;
                    });
                }
            }

            for (let i = start; i < end; i++) {
                let color = '', title='';
                for (let k = 0; k < ngaynghi.length; k++) {
                    let thisdate = new Date(ngaynghi[k].Ngay);
                    if (thisdate.getDate() === i) {
                        if (ngaynghi[k].LoaiNgay === 1) {
                            color = 'blue';
                            title = 'Ngày nghỉ';
                        }
                        else {
                            color = 'red';
                            title = 'Ngày lễ';
                        }
                        break;
                    }
                }
                array.push({ Ngay: i, Thu: GetDayOfWeek(year, month, i), Color: color, Title: title });
            }
            return array;
        },
        TitleThang: function () {
            var title = '';
            if (this.curentpage.typetime === 0) {
                switch (parseInt(this.curentpage.typetimeold)) {
                    case 1:
                    case 2:
                        title = this.curentpage.textdate.concat(', ngày ', moment(this.curentpage.tungay).format('DD/MM/YYYY'));
                        break;
                    case 3:
                    case 4:
                    case 7:
                    case 8:
                        title = this.curentpage.textdate.concat(', từ ', moment(this.curentpage.tungay).format('DD/MM/YYYY'), ' - ', moment(this.curentpage.denngay).format('DD/MM/YYYY'));
                        break;
                    case 5:
                    case 6:
                        title = "Tháng " + (this.curentpage.month + 1) + " Năm " + this.curentpage.year;
                        break;
                    case 9:
                    case 10:
                        title = " Năm " + this.curentpage.year;
                        break;
                    case 0:
                        title = this.curentpage.textdate;
                        break;
                }
            }
            else {
                title = 'Từ '.concat(moment(this.curentpage.tungay).format('DD/MM/YYYY'), ' - ', moment(this.curentpage.denngay).format('DD/MM/YYYY'));
            }
            return title;
        },
        HideTdByDate: function () {
        },
        trangthaiNV: function () {
            return parseInt(this.curentpage.TrangThai)
        }
    },
});
//vmChamCong.KhoiTaoDuLieuChamCong();
vmChamCong.GetListData();
vmChamCong.GetListCaLamViec();
vmChamCong.GetForSearchChamCong();

$(document).mouseup(function () {
    $('.popup-cham-cong').mouseup(function () {
        return false
    });
    $('.popup-cham-cong').hide();
});

$('.dtchamcong').datepicker({
    format: "mm/yyyy",
    startView: 1, // 1.months, 0.days
    minViewMode: 1,
    language: "vi"
}).on('changeMonth', function (time) {
    //vmChamCong.ChangeTime(time)
});

$(function () {
    $('.daterange').on('apply.daterangepicker', function (ev, picker) {
        var fromDate = picker.startDate;
        var toDate = picker.endDate;
        vmChamCong.curentpage.textdaterange = fromDate.format('DD/MM/YYYY') + ' - ' + toDate.format('DD/MM/YYYY');
        vmChamCong.curentpage.tungay = fromDate.format('YYYY-MM-DD');
        vmChamCong.curentpage.denngay = toDate.format('YYYY-MM-DD');
        vmChamCong.ChangeDateRange();
    });

    $('.radio-menu').on('click', function () {
        if ($(this).children('input').val() === '1') {
            var daterange = vmChamCong.curentpage.textdaterange.split('-');
            vmChamCong.curentpage.tungay = moment(daterange[0].trim(), 'DD/MM/YYYY').format('YYYY-MM-DD');
            vmChamCong.curentpage.denngay = moment(daterange[1].trim(), 'DD/MM/YYYY').format('YYYY-MM-DD');
            vmChamCong.ChangeDateRange();
        }
    });
})


function GetDayOfWeek(year, month, date) {
    let d = new Date(year, month, date);
    let thu = d.getDay();
    if (thu === 0) {
        return 'CN';
    } else {
        for (let i = 1; i <= 6; i++) {
            if (thu === i) {
                return 'T'.concat(i + 1);
            }
        }
    }
}

$('body').on('AddPhanCaLamViecSucces', function () {
    vmChamCong.GetForSearchChamCong(true);
    $(".op-tr-hide").hide();
    $(".op-tr-show").removeClass('tr-active');
    $('.line-right').height(0).css("margin-top", "0px");
});
$('body').on('AddCaLamViecSucces', function () {
    vmChamCong.GetListCaLamViec();
    vmChamCong.ResetChoseCa();
});
$('body').on('SaveKyHieuCongSuccess', function () {
    vmChamCong.GetForSearchChamCong(true);
});
