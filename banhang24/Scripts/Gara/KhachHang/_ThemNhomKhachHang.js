var vmThemMoiNhomKhach = new Vue({
    el: '#ThemNhomKhachHang',
    created: function () {
        this.UrlDoiTuongAPI = '/api/DanhMuc/DM_DoiTuongAPI/';
        this.ToDay = new Date();
        this.MangDieuKien = [
            { ID: 1, TenDieuKien: 'Tổng mua (trừ trả hàng)', },
            { ID: 2, TenDieuKien: 'Tổng mua', },
            { ID: 3, TenDieuKien: 'Thời gian mua hàng', },
            { ID: 4, TenDieuKien: 'Số lần mua hàng', },
            { ID: 5, TenDieuKien: 'Công nợ hiện tại', },
            { ID: 6, TenDieuKien: 'Tháng sinh', },
            { ID: 7, TenDieuKien: 'Tuổi', },
            { ID: 8, TenDieuKien: 'Giới tính', },
            { ID: 9, TenDieuKien: 'Khu vực', },
            //{ ID: 10, TenDieuKien: 'Vùng miền', },
        ];

        this.MangSoSanh = [
            { ID: 1, KieuSoSanh: '>' },
            { ID: 2, KieuSoSanh: '>=' },
            { ID: 3, KieuSoSanh: '=' },
            { ID: 4, KieuSoSanh: '<=' },
            { ID: 5, KieuSoSanh: '<' },
            { ID: 6, KieuSoSanh: 'Khác' },
        ];
        this.ThangSinh = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
    },
    data: {
        saveOK: false,
        isNew: true,
        typeUpdate: 0,//1.themmoi, 2.update, 0.delete
        isLoading: false,
        isCheck_UpdateCus: false,
        isGiamGiaPtram: true,
        DieuKienNangNhom: [],
        ChiNhanhChosed: [],
        QuanLyKhachHangTheoDonVi: true,
        error: '',
        groupOld: {},
        groupOldDetail: [],
        dieukienChosed: 1,

        listData: {
            ChiNhanhs: [],
            VungMiens: [],
            TinhThanhs: [],
            ListTinhThanhSearch: [],
        },
        inforLogin: {
            ID_NhanVien: null,
            ID_User: null,
            UserLogin: null,
            ID_DonVi: null,
        },
        newGroup: {
            ID: null,
            TenNhomDoiTuong: '',
            LoaiDoiTuong: 1,
            GhiChu: '',
            NguoiTao: '',
            GiamGia: 0,
            GiamGiaTheoPhanTram: true,
            TuDongCapNhat: false,
            TenNhomDoiTuong_KhongDau: null,
            TenNhomDoiTuong_KyTuDau: null,
            TrangThai: true,
        },
    },
    methods: {
        showModalAdd: function () {
            var self = this;
            self.isNew = true;
            self.typeUpdate = 1;
            self.saveOK = false;
            self.isLoading = false;
            self.isCheck_UpdateCus = false;
            self.dieukienChosed = 1;
            self.DieuKienNangNhom = [];
            self.error = '';
            self.groupOld = {};
            self.ChiNhanhChosed = [];
            self.QuanLyKhachHangTheoDonVi = true;
            console.log(31)
            self.newGroup = {
                ID: '00000000-0000-0000-0000-000000000000',
                TenNhomDoiTuong: '',
                LoaiDoiTuong: 1,
                GhiChu: '',
                NguoiTao: '',
                GiamGia: 0,
                GiamGiaTheoPhanTram: true,
                TuDongCapNhat: false,
                TenNhomDoiTuong_KhongDau: null,
                TenNhomDoiTuong_KyTuDau: null,
                TrangThai: true,
            };
            $('#ThemNhomKhachHang').modal('show');
        },
        showModalUpdate: async function (item) {
            var self = this;
            self.isCheck_UpdateCus = false;
            self.isNew = false;

            if (item.TuDongCapNhat === null)
                item.TuDongCapNhat = false;
            self.newGroup = item;
            self.groupOld = $.extend({}, item);
            self.typeUpdate = 2;

            // get chinhannh chosed
            if (self.groupOld.NhomDT_DonVi.length !== self.listData.ChiNhanhs.length) {
                self.ChiNhanhChosed = item.NhomDT_DonVi;
            }
            else {
                self.ChiNhanhChosed = [];
            }

            await self.getList_NhomDoiTuongChiTietbyID(item.ID);
            for (let i = 0; i < self.DieuKienNangNhom.length; i++) {
                if (self.DieuKienNangNhom[i].ID_KhuVucFirstLoad != null) {
                    self.DieuKienNangNhom[i].ID_KhuVuc = self.DieuKienNangNhom[i].ID_KhuVucFirstLoad;
                }
            }
            $('#ThemNhomKhachHang').modal('show');
        },
        getList_NhomDoiTuongChiTietbyID: async function (id) {
            var self = this;
            self.DieuKienNangNhom = [];
            await ajaxHelper(self.UrlDoiTuongAPI + "getList_NhomDoiTuongChiTietbyID?ID_NhomDoiTuong=" + id).done(function (data) {
                for (let i = 0; i < data.length; i++) {
                    data[i].IDRandom = CreateIDRandom('DK');
                    if (data[i].TimeBy !== null) {
                        data[i].TimeBy = moment(data[i].TimeBy).format('DD/MM/YYYY');
                    }
                    data[i].GiaTri = formatNumber(data[i].GiaTri);
                    data[i].ID_KhuVucFirstLoad = data[i].ID_KhuVuc;
                    data[i].ID_KhuVuc = null;
                }
                self.DieuKienNangNhom = data;
                self.groupOldDetail = $.extend([], data);
            });
        },
        KeyUp_GiamGiaNhomKH: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            if (self.newGroup.GiamGiaTheoPhanTram) {
                if (formatNumberToFloat($this.val()) > 100) {
                    $this.val(100);
                }
            }
        },
        choose_PhanTram: function () {
            var self = this;
            self.newGroup.GiamGiaTheoPhanTram = !self.newGroup.GiamGiaTheoPhanTram;
        },
        ChangeGioiTinh: function (val, parent) {
            var self = this;
        },
        ResetThoiGian: function (item) {
            var self = this;
            var $this = $(event.currentTarget);
            $($this).datetimepicker({
                format: 'd/m/Y',
                timepicker: false,
                defaultDate: new Date(),
                mask: true,
                onSelectDate: function (e, evt) {
                    let dtNew = moment(e).format('DD/MM/YYYY');
                    for (let i = 0; i < self.DieuKienNangNhom.length; i++) {
                        if (self.DieuKienNangNhom[i].IDRandom === item.IDRandom) {
                            self.DieuKienNangNhom[i].TimeBy = dtNew;
                            break;
                        }
                    }
                    $($this).datetimepicker('destroy');
                }
            })
        },
        ChoseChiNhanh: function (item) {
            var self = this;
            if (item === null) {
                self.ChiNhanhChosed = [];
            }
            else {
                if ($.inArray(item.ID, self.ListIDChiNhanh) === -1) {
                    self.ChiNhanhChosed.push(item);
                }
            }
        },
        RemoveChiNhanh: function (item, index) {
            var self = this;
            self.ChiNhanhChosed.splice(index, 1);
        },
        ChoseThangSinh: function (item, parent) {
            var self = this;
            var $this = $(event.currentTarget);
            for (let i = 0; i < self.DieuKienNangNhom.length; i++) {
                if (self.DieuKienNangNhom[i].IDRandom === parent.IDRandom) {
                    self.DieuKienNangNhom[i].ThangSinh = item;
                    break;
                }
            }
            $this.closest('div').hide();
        },
        ThemDieuKienNN: function () {
            var self = this;
            let idRandom = CreateIDRandom('DK');
            var ob1 = {
                IDRandom: idRandom,
                HinhThuc: 'Tổng mua (trừ trả hàng)',
                LoaiHinhThuc: 1,
                LoaiSoSanh: '>',
                SoSanh: 1,
                GiaTri: 0,
                TimeBy: moment(self.ToDay).format('DD/MM/YYYY'),
                GioiTinh: true,
                ThangSinh: 1,
                ID_KhuVuc: null,
                KhuVuc: '',// tentinhthanh
                ID_VungMien: null,
                VungMien: '',//tenkhuvuc
            };
            self.DieuKienNangNhom.push(ob1);
        },
        ChoseDieuKien: function (item, parent) {
            var self = this;
            var ss = '>';
            var lss = 1;
            switch (item.ID) {
                case 9:
                case 10:
                    ss = 'Khác';
                    lss = 6;
                    break;
                case 8:
                    ss = '=';
                    lss = 3;
                    break;
            }

            for (let i = 0; i < self.DieuKienNangNhom.length; i++) {
                if (self.DieuKienNangNhom[i].IDRandom === parent.IDRandom) {
                    self.DieuKienNangNhom[i].LoaiHinhThuc = item.ID;
                    self.DieuKienNangNhom[i].HinhThuc = item.TenDieuKien;
                    self.DieuKienNangNhom[i].LoaiSoSanh = ss;
                    self.DieuKienNangNhom[i].SoSanh = lss;
                    self.DieuKienNangNhom[i].GiaTri = 0;
                    self.DieuKienNangNhom[i].TimeBy = moment(self.ToDay).format('DD/MM/YYYY');
                    self.DieuKienNangNhom[i].GioiTinh = true;
                    self.DieuKienNangNhom[i].ThangSinh = 1;
                    self.DieuKienNangNhom[i].ID_KhuVuc = null;
                    self.DieuKienNangNhom[i].ID_VungMien = null;
                    break;
                }
            }
        },
        ChoseKieuSoSanh: function (item, parent) {
            var self = this;
            for (let i = 0; i < self.DieuKienNangNhom.length; i++) {
                if (self.DieuKienNangNhom[i].IDRandom === parent.IDRandom) {
                    self.DieuKienNangNhom[i].LoaiSoSanh = item.KieuSoSanh;
                    self.DieuKienNangNhom[i].SoSanh = item.ID;
                    break;
                }
            }
        },
        SetCheck_ItemChosing: function (item, loai) {
            var self = this;
            var $this = $(event.currentTarget)
            $this.next().show();
            switch (loai) {
                case 1:
                    self.dieukienChosed = item.LoaiHinhThuc;
                    break;
                case 2:
                    self.dieukienChosed = item.SoSanh;
                    break;
                case 3:
                    self.dieukienChosed = item.ThangSinh;
                    break;
            }
        },
        RemoveDieuKienNangNhom: function (item, index) {
            var self = this;
            self.DieuKienNangNhom.splice(index, 1);
        },
        DieuKienNangNhom_ChangeGiaTri: function (item) {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            var gtri = formatNumberToFloat($this.val());
            for (let i = 0; i < self.DieuKienNangNhom.length; i++) {
                if (self.DieuKienNangNhom[i].IDRandom === item.IDRandom) {
                    self.DieuKienNangNhom[i].GiaTri = gtri;
                    break;
                }
            }
        },
        ChoseTinhThanh: function (item) {
            var self = this;
            var $this = $(event.currentTarget);
            var idRandom = $this.closest('.outselect').attr('id').split('_')[1];
            var tenTinhThanh = $this.find('.seach-hh').text();
            $this.closest('div').prev().val(tenTinhThanh);
            for (let i = 0; i < self.DieuKienNangNhom.length; i++) {
                if (self.DieuKienNangNhom[i].IDRandom === idRandom) {
                    self.DieuKienNangNhom[i].ID_KhuVuc = item.id;
                    self.DieuKienNangNhom[i].KhuVuc = tenTinhThanh;
                    break;
                }
            }
        },
        ChoseVungMien: function (item, parent) {
            var self = this;
            var $this = $(event.currentTarget);
            for (let i = 0; i < self.DieuKienNangNhom.length; i++) {
                if (self.DieuKienNangNhom[i].IDRandom === parent.IDRandom) {
                    self.DieuKienNangNhom[i].ID_VungMien = item.ID;
                    self.DieuKienNangNhom[i].VungMien = item.TenVung;
                    break;
                }
            }
            $this.closest('div').hide();
        },
        SearchTinhThanh: function (value) {
            var self = this;
            var txt = commonStatisJs.convertVieToEng(value.searchkey);
            if (txt === '') {
                for (let i = 0; i < self.DieuKienNangNhom.length; i++) {
                    if (self.DieuKienNangNhom[i].LoaiHinhThuc === 9) {
                        self.DieuKienNangNhom[i].ID_KhuVuc = '';
                        self.DieuKienNangNhom[i].KhuVuc = '';
                        break;
                    }
                }
            }

            self.listData.ListTinhThanhSearch = self.listData.TinhThanhs
                .filter(p => commonStatisJs.convertVieToEng(p.val1).match(txt)
                    || commonStatisJs.convertVieToEng(p.val2).match(txt)
                    || commonStatisJs.convertVieToEng(p.val3).match(txt));

        },
        SaveNhomKhachHang: function () {
            var self = this;
            var tenNhom = self.newGroup.TenNhomDoiTuong;
            var giamgia = formatNumberToFloat(self.newGroup.GiamGia);
            var ptram = self.newGroup.GiamGiaTheoPhanTram;
            var ghichu = self.newGroup.GhiChu;
            ghichu = ghichu === undefined ? '' : ghichu;
            var sNangcao = '', sNangcaoOld = ' <br /> <b> Thiết lập nâng cao: </b>';

            if (tenNhom === '' || tenNhom === undefined) {
                ShowMessage_Danger('Vui lòng nhập tên nhóm khách hàng');
                return;
            }
            if (giamgia > 100 & ptram === true) {
                ShowMessage_Danger('Giảm giá không được lớn hơn 100%');
                return;
            }
            for (let i = 0; i < self.DieuKienNangNhom.length; i++) {
                let itFor = self.DieuKienNangNhom[i];
                let LoaiHinhThuc = itFor.LoaiHinhThuc;

                if ((LoaiHinhThuc === 1 || LoaiHinhThuc === 2 || LoaiHinhThuc === 4 || LoaiHinhThuc === 5 || LoaiHinhThuc === 7)
                    && itFor.GiaTri === '') {
                    ShowMessage_Danger('Vui lòng nhập giá trị ' + itFor.HinhThuc);
                    return;
                }
                else if ((LoaiHinhThuc === 3) && itFor.TimeBy === '') {
                    ShowMessage_Danger('Vui lòng nhập giá trị ' + itFor.HinhThuc);
                    return;
                }
                else if ((LoaiHinhThuc === 9) && itFor.KhuVuc === null) {
                    ShowMessage_Danger('Vui lòng nhập giá trị ' + itFor.HinhThuc);
                    return;
                }
                else if ((LoaiHinhThuc === 10) && itFor.VungMien === null) {
                    ShowMessage_Danger('Vui lòng nhập giá trị ' + itFor.HinhThuc);
                    return;
                }

                switch (LoaiHinhThuc) {
                    case 1:
                    case 2:
                    case 4:
                    case 5:
                    case 7:
                        sNangcao += '<br /> - '.concat(itFor.HinhThuc, ' ', itFor.LoaiSoSanh, ' ', formatNumber(itFor.GiaTri));
                        break;
                    case 3:
                        sNangcao += '<br /> - '.concat(itFor.HinhThuc, ' ', itFor.LoaiSoSanh, ' ', itFor.TimeBy);
                        break;
                    case 8:
                        sNangcao += '<br /> - '.concat(itFor.HinhThuc, ' ', itFor.LoaiSoSanh, ' ', itFor.GioiTinh ? 'Nam' : 'Nữ');
                        break;
                    case 9:
                        sNangcao += '<br /> - '.concat(itFor.HinhThuc, ' ', itFor.LoaiSoSanh, ' ', itFor.KhuVuc);
                        break;
                    case 10:
                        sNangcao += '<br /> - '.concat(itFor.HinhThuc, ' ', itFor.LoaiSoSanh, ' ', formatNumber(itFor.VungMien));
                        break;
                }
            }
            sNangcao = sNangcao.concat('<br /> - Tự động cập nhật: ', self.newGroup.TuDongCapNhat ? 'Có' : 'Không');

            var tb = "Thêm mới";
            var idNhomDT = self.newGroup.ID;
            if (idNhomDT === undefined || idNhomDT === const_GuidEmpty) {
                idNhomDT = null;
            }
            if (idNhomDT !== null) {
                tb = "Cập nhật";
            }
            var DM_NhomDoiTuong = {
                ID: idNhomDT,
                TenNhomDoiTuong: tenNhom,
                LoaiDoiTuong: 1,
                GhiChu: ghichu,
                TenNhomDoiTuong_KhongDau: locdau(tenNhom),
                TenNhomDoiTuong_KyTuDau: GetChartStart(tenNhom),
                TuDongCapNhat: self.newGroup.TuDongCapNhat,
                GiamGia: giamgia,
                GiamGiaTheoPhanTram: ptram,
                NhomDT_DonVi: self.ChiNhanhChosed,
            };
            // add to search in Vue
            DM_NhomDoiTuong.Text_Search = tenNhom.concat(DM_NhomDoiTuong.TenNhomDoiTuong_KhongDau, ' ', DM_NhomDoiTuong.TenNhomDoiTuong_KyTuDau);

            // save NhomDoiTuong_DonVi
            var arrID = [];
            if (self.ChiNhanhChosed.length === 0) {
                // get all ChiNhanh
                for (let i = 0; i < self.listData.ChiNhanhs.length; i++) {
                    arrID.push(self.listData.ChiNhanhs[i].ID);
                }
            }
            else {
                for (let i = 0; i < self.ChiNhanhChosed.length; i++) {
                    arrID.push(self.ChiNhanhChosed[i].ID);
                }
            }

            // format date in list DKNangNhom
            for (let i = 0; i < self.DieuKienNangNhom.length; i++) {
                if (self.DieuKienNangNhom[i].TimeBy !== null) {
                    self.DieuKienNangNhom[i].TimeBy = moment(self.DieuKienNangNhom[i].TimeBy, 'DD/MM/YYYY').format('YYYY-MM-DD');
                }
            }

            var myData = {};
            myData.objNhomDoiTuong = [DM_NhomDoiTuong];
            myData.objNhomDoiTuongChiTiet = self.DieuKienNangNhom;
            myData.lstIDChiNhanh = arrID;
            console.log('myData', myData);

            // self.autoUpdate(): rdcokhong1 (true: HeThongCapNhat, false: HeThongkhongCapNhat)
            var loaiCapNhat = (self.isCheck_UpdateCus || self.isCheck_UpdateCus === 'true') ? 1 : 3;

            $.ajax({
                data: myData,
                url: self.UrlDoiTuongAPI + "Creater_NangNhomDoiTuong_ChiNhanh?ID_NhomDoiTuong=" + idNhomDT + "&User=" + self.inforLogin.UserLogin
                    + "&Autocheck=" + self.newGroup.TuDongCapNhat + "&ID_DonVi=" + self.inforLogin.ID_DonVi + "&ID_NhanVien="
                    + self.inforLogin.ID_NhanVien + "&phuongthuc=" + loaiCapNhat,
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",

                success: function (item) {
                    self.newGroup.ID = item;
                    self.newGroup.NhomDT_DonVi = DM_NhomDoiTuong.NhomDT_DonVi;
                    self.saveOK = true;

                    // checkCapNhat(): rdcokhong (2. CapNhat, 3. KhongCapNhat)
                    if (self.isCheck_UpdateCus === false) {
                        ShowMessage_Success(tb + ' nhóm khách hàng thành công');
                    }
                    else {
                        if (self.DieuKienNangNhom.length > 0) {
                            ajaxHelper(self.UrlDoiTuongAPI + 'UpdateKhachHang_DuDKNangNhom?idNhomDT=' + self.newGroup.ID
                                + '&lstIDDonVi=' + [self.inforLogin.ID_DonVi], 'POST', [self.inforLogin.ID_DonVi]).done(function (x) {
                                    if (x.res === true) {

                                    }
                                })
                        }
                    }

                    var tenCNs = '';
                    if (self.ChiNhanhChosed.length === 0 || self.ChiNhanhChosed.length === self.listData.ChiNhanhs) {
                        tenCNs = 'Tất cả';
                    }
                    else {
                        tenCNs = self.ChiNhanhChosed.map(function (x) {
                            return x.TenDonVi;
                        }).toString();
                    }

                    let sFirst = self.typeUpdate == 1 ? 'Thêm mới' : 'Cập nhật';
                    sFirst = sFirst.concat(' nhóm khách hàng ', self.newGroup.TenNhomDoiTuong);

                    let detail = '<b> Thông tin nhóm khách hàng: </b>'.concat('<br /> - Tên: ', self.newGroup.TenNhomDoiTuong,
                        '<br /> - Giảm giá: ', self.newGroup.GiamGia, self.newGroup.GiamGiaTheoPhanTram ? ' %' : ' VND',
                        '<br /> - Chi nhánh: ', tenCNs,
                        '<br /> - Ghi chú: ', self.newGroup.GhiChu,
                        '<br /><b> Thiết lập nâng cao: </b>', sNangcao
                    )

                    let detailOld = '';
                    if (self.typeUpdate === 2) {
                        for (let i = 0; i < self.groupOldDetail.length; i++) {
                            let itFor = self.groupOldDetail[i];
                            switch (itFor.LoaiHinhThuc) {
                                case 1:
                                case 2:
                                case 4:
                                case 5:
                                case 7:
                                    sNangcaoOld += '<br /> - '.concat(itFor.HinhThuc, ' ', itFor.LoaiSoSanh, ' ', formatNumber(itFor.GiaTri));
                                    break;
                                case 3:
                                    sNangcaoOld += '<br /> - '.concat(itFor.HinhThuc, ' ', itFor.LoaiSoSanh, ' ', itFor.TimeBy);
                                    break;
                                case 8:
                                    sNangcaoOld += '<br /> - '.concat(itFor.HinhThuc, ' ', itFor.LoaiSoSanh, ' ', itFor.GioiTinh ? 'Nam' : 'Nữ');
                                    break;
                                case 9:
                                    sNangcaoOld += '<br /> - '.concat(itFor.HinhThuc, ' ', itFor.LoaiSoSanh, ' ', itFor.KhuVuc);
                                    break;
                                case 10:
                                    sNangcaoOld += '<br /> - '.concat(itFor.HinhThuc, ' ', itFor.LoaiSoSanh, ' ', formatNumber(itFor.VungMien));
                                    break;
                            }
                        }

                        let chinhanhOld = '';
                        if (self.groupOld.NhomDT_DonVi.length === 0 || self.groupOld.NhomDT_DonVi.length === self.listData.ChiNhanhs.length) {
                            chinhanhOld = ' Tất cả';
                        }
                        else {
                            chinhanhOld = self.groupOld.NhomDT_DonVi.map(function (x) {
                                return x.TenDonVi;
                            }).toString()
                        }
                        detailOld = ' <br />  <br /> <b>Thông tin cũ: </b>'.concat('<br /> - Tên: ', self.groupOld.TenNhomDoiTuong,
                            '<br /> - Giảm giá: ', self.groupOld.GiamGia, self.groupOld.GiamGiaTheoPhanTram ? ' %' : ' VND',
                            '<br /> - Chi nhánh: ', chinhanhOld,
                            '<br /> - Ghi chú: ', self.groupOld.GhiChu,
                            sNangcaoOld
                        )
                    }

                    var diary = {
                        ID_DonVi: self.inforLogin.ID_DonVi,
                        ID_NhanVien: self.inforLogin.ID_NhanVien,
                        LoaiNhatKy: self.typeUpdate === 0 ? 3 : self.typeUpdate,
                        ChucNang: 'Nhóm khách hàng',
                        NoiDung: sFirst,
                        NoiDungChiTiet: ''.concat(detail, detailOld)
                    }

                    Insert_NhatKyThaoTac_1Param(diary);
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    self.saveOK = false;
                    ShowMessage_Danger(tb + ' nhóm khách hàng không thành công')
                },
                complete: function () {
                    $("#ThemNhomKhachHang").modal("hide");
                }
            })
        },
        DeleteNhomKhach: function () {
            var self = this;
            dialogConfirm('Thông báo xóa', 'Bạn có chắc chắn muốn xóa nhóm khách hàng <b>'
                + self.newGroup.TenNhomDoiTuong + ' </b> không?', function () {
                    self.typeUpdate = 0;
                    self.saveOK = true;
                    ajaxHelper('/api/DanhMuc/DM_NhomDoiTuongAPI/' + 'DeleteDM_NhomDoiTuong/' + self.newGroup.ID, 'DELETE').done(function (msg) {
                        $('#ThemNhomKhachHang').modal('hide');
                        if (msg === "") {
                            ShowMessage_Success('Xóa nhóm khách hàng thành công ');
                            var diary = {
                                ID_DonVi: self.inforLogin.ID_DonVi,
                                ID_NhanVien: self.inforLogin.ID_NhanVien,
                                LoaiNhatKy: 3,
                                ChucNang: 'Nhóm khách hàng',
                                NoiDung: 'Xóa nhóm khách hàng '.concat(self.newGroup.TenNhomDoiTuong),
                                NoiDungChiTiet: 'Xóa nhóm khách hàng '.concat(self.newGroup.TenNhomDoiTuong, ', Người xóa: ', self.inforLogin.UserLogin),
                            };
                            Insert_NhatKyThaoTac_1Param(diary);
                        }
                        else {
                            ShowMessage_Danger(msg);
                        }
                    });
                })
        },
    },
    computed: {
        ListIDChiNhanh: function () {
            return vmThemMoiNhomKhach.ChiNhanhChosed.map(function (x) { return x.ID });
        }
    }
})


