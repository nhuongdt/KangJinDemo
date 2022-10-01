var vmTiepNhanXe = new Vue({
    el: '#TiepNhanXeModal',
    components: {
        'nhanviens': ComponentChoseStaff,
        'my-date-time': cpmDatetime,
        'customers': cmpChoseCustomer,
        'cars': cmpChoseCar,
        'cmp-list-chose': cmpYesNo,
    },
    created: function () {
        let self = this;
        self.GuidEmpty = '00000000-0000-0000-0000-000000000000';
    },
    data: {
        saveOK: false,
        isNew: true,
        isLoading: false,
        staffName: '',
        adviserName: '',
        customerName: '',
        biensoXe: '',
        trangthaiPhieuTN: 'Đang sửa',
        phieuTiepNhanOld: {},
        ListImgOld: [],
        cmp: {
            title: '',
            mes: '',
            show: false,
            //ListChose: [
            //    { ID: 0, Text: 'Thay đổi thông tin phiếu tiếp nhận', Value: true },
            //    { ID: 1, Text: 'Cập nhật lại công ty bảo hiểm cho hóa đơn', Value: false },
            //    { ID: 2, Text: 'Cập nhật lại khách hàng cho hóa đơn', Value: false },
            //    { ID: 3, Text: 'Cập nhật lại phiếu thu liên quan', Value: false },
            //],
            ListChose: [],
        },
        role: {
            Xe: {},
            PhieuTiepNhan: {},
            KhachHang: {},
            BaoHiem: {},
        },
        inforLogin: {
            ID_NhanVien: null,
            ID_User: null,
            UserLogin: null,
            ID_DonVi: null,
            TenNhanVien: '',
        },
        newPhieuTiepNhan: {
            ID: null,
            MaPhieuTiepNhan: '',
            ID_Xe: null,
            NgayVaoXuong: moment(new Date()).format('YYYY-MM-DD HH:mm'),
            ID_NhanVien: null,
            ID_CoVanDichVu: null,
            ID_KhachHang: null,
            ID_BaoHiem: null,
            NguoiLienHeBH: '',
            SoDienThoaiLienHeBH: '',
            NgayXuatXuongDuKien: null,
            NgayXuatXuong: null,
            TenLienHe: '',
            SoDienThoaiLienHe: '',
            GhiChu: '',
            SoKmVao: 0,
            SoKmRa: 0,
            LaChuXe: false,
            TrangThai: 1,
        },
        carChosing: {
            ID: '',
            BienSo: '',
            TenMauXe: '',
            TenHangXe: '',
            SoKhung: '',
            SoMay: '',
        },
        ChuXe: {},
        insurenceChosing: {
            ID: '',
            MaBaoHiem: '',
            TenBaoHiem: '',
        },
        customerChosing: {
            ID: '',
            MaDoiTuong: '',
            TenDoiTuong: '',
            Email: '',
            DienThoai: '',
            DiaChi: ''
        },
        listData: {
            NhanViens: [],
            BienSos: [],
            HangMucSuaChuas: [],
            GiayToDinhKems: [],
            PhieuSuaChuas: [],
            MauXe: [],
            HangXe: [],
            LoaiXe: [],
            TrangThaiPhieuTN: [
                { ID: 1, Text: "Đang sửa" },
                { ID: 2, Text: "Hoàn thành" },
                //{ ID: 3, Text: "Đã xuất xưởng" },
                { ID: 0, Text: "Hủy" },
            ]
        }
    },
    methods: {
        CheckRole: function (maquyen) {
            return VHeader.Quyen.indexOf(maquyen) > -1;
        },
        ResetPhieuTiepNhap: function () {
            var self = this;
            self.saveOK = false;
            self.isNew = true;
            self.isLoading = false;
            self.adviserName = '';
            self.ListImgOld = [];
            self.staffName = self.inforLogin.TenNhanVien;
            self.trangthaiPhieuTN = 'Đang sửa';

            self.newPhieuTiepNhan = {
                ID: null,
                MaPhieuTiepNhan: '',
                ID_Xe: null,
                ID_DonVi: self.inforLogin.ID_DonVi,
                NgayVaoXuong: moment(new Date()).format('YYYY-MM-DD HH:mm'),
                ID_NhanVien: self.inforLogin.ID_NhanVien,
                ID_CoVanDichVu: null,
                ID_KhachHang: null,
                ID_BaoHiem: null,
                NguoiLienHeBH: '',
                SoDienThoaiLienHeBH: '',
                NgayXuatXuongDuKien: null,
                TenLienHe: '',
                SoDienThoaiLienHe: '',
                GhiChu: '',
                SoKmVao: 0,
                SoKmRa: 0,
                TrangThai: 1,
            };
            self.carChosing = {
                ID: null,
                BienSo: '',
                TenMauXe: '',
                TenHangXe: '',
                TenLoaiXe: '',
                SoKhung: '',
                SoMay: '',
                HopSo: '',
                DungTich: '',
                MauSon: '',
                NamSanXuat: '',
            };
            self.customerChosing = {
                ID: null,
                MaDoiTuong: '',
                TenDoiTuong: '',
                Email: '',
                DienThoai: '',
                DiaChi: ''
            };
            self.insurenceChosing = {
                ID: '',
                MaBaoHiem: '',
                TenBaoHiem: '',
            };
        },
        Reset_ThongTinXe: function () {
            var self = this;
            self.carChosing = {
                ID: null,
                BienSo: '',
                TenMauXe: '',
                TenHangXe: '',
                TenLoaiXe: '',
                SoKhung: '',
                SoMay: '',
                HopSo: '',
                DungTich: '',
                MauSon: '',
                NamSanXuat: '',
            };
            self.customerChosing = {
                ID: '',
                MaDoiTuong: '',
                TenDoiTuong: '',
                Email: '',
                DienThoai: '',
                DiaChi: ''
            };
            self.insurenceChosing = {
                ID: '',
                MaBaoHiem: '',
                TenBaoHiem: '',
            };
            self.newPhieuTiepNhan.LaChuXe = false;
            self.newPhieuTiepNhan.ID_Xe = null;
            self.newPhieuTiepNhan.ID_KhachHang = null;
        },
        showModalTiepNhanXe: function () {
            var self = this;
            self.ResetPhieuTiepNhap();
            self.listData.HangMucSuaChuas = [];
            self.cmp.ListChose = [];
            self.listData.HangMucSuaChuas.push(self.CreateNew_ItemFix());
            self.listData.GiayToDinhKems = [];
            self.listData.GiayToDinhKems.push(self.CreateNew_ItemAttach());
            $("#TiepNhanXeModal").modal('show');
            console.log('tnx')
        },
        UpdatePhieuTiepNhan: function (thongtin, hangmuc, vatdung) {
            var self = this;
            self.isNew = false;
            self.saveOK = false;
            self.isLoading = false;
            self.staffName = thongtin.NhanVienTiepNhan;
            self.adviserName = thongtin.CoVanDichVu;
            self.phieuTiepNhanOld = $.extend({}, thongtin);

            switch (thongtin.TrangThai) {
                case 1:
                    self.trangthaiPhieuTN = 'Đang sửa';
                    break;
                case 2:
                    self.trangthaiPhieuTN = 'Hoàn thành';
                    break;
                case 3:
                    self.trangthaiPhieuTN = 'Đã xuất xưởng';
                    break;
                case 0:
                    self.trangthaiPhieuTN = 'Hủy';
                    break;
            }

            self.newPhieuTiepNhan = {
                ID: thongtin.ID,
                MaPhieuTiepNhan: thongtin.MaPhieuTiepNhan,
                ID_Xe: thongtin.ID_Xe,
                ID_DonVi: self.inforLogin.ID_DonVi,
                ID_NhanVien: thongtin.ID_NhanVien,
                ID_CoVanDichVu: thongtin.ID_CoVanDichVu,
                NgayVaoXuong: moment(thongtin.NgayVaoXuong).format('YYYY-MM-DD HH:mm'),
                ID_NhanVien: thongtin.ID_NhanVien,
                ID_KhachHang: thongtin.ID_KhachHang,
                NgayXuatXuong: thongtin.NgayXuatXuong ? moment(thongtin.NgayXuatXuong).format('YYYY-MM-DD HH:mm') : '',
                NgayXuatXuongDuKien: thongtin.NgayXuatXuongDuKien ? moment(thongtin.NgayXuatXuongDuKien).format('YYYY-MM-DD HH:mm') : '',
                TenLienHe: thongtin.TenLienHe,
                SoDienThoaiLienHe: thongtin.SoDienThoaiLienHe,
                GhiChu: thongtin.GhiChu,
                SoKmVao: thongtin.SoKmVao,
                SoKmRa: thongtin.SoKmRa,
                LaChuXe: thongtin.LaChuXe,
                TrangThai: thongtin.TrangThai,

                ID_BaoHiem: thongtin.ID_BaoHiem,
                NguoiLienHeBH: thongtin.NguoiLienHeBH,
                SoDienThoaiLienHeBH: thongtin.SoDienThoaiLienHeBH,
            };

            self.customerChosing = {
                ID: thongtin.ID_KhachHang,
                MaDoiTuong: thongtin.MaDoiTuong,
                TenDoiTuong: thongtin.TenDoiTuong,
                Email: thongtin.Email,
                DienThoai: thongtin.DienThoaiKhachHang,
                DiaChi: thongtin.DiaChi
            };
            self.insurenceChosing = {
                ID: thongtin.ID_BaoHiem,
                MaBaoHiem: thongtin.MaBaoHiem,
                TenBaoHiem: thongtin.TenBaoHiem,
            };
            self.ChuXe = {
                ID: thongtin.ID_ChuXe,
                MaDoiTuong: '',
                TenDoiTuong: thongtin.ChuXe,
                Email: thongtin.ChuXe_Email,
                DienThoai: thongtin.ChuXe_SDT,
                DiaChi: thongtin.ChuXe_DiaChi
            }

            self.carChosing = {
                ID: thongtin.ID_Xe,
                BienSo: thongtin.BienSo,
                TenMauXe: thongtin.TenMauXe,
                TenHangXe: thongtin.TenHangXe,
                TenLoaiXe: thongtin.TenLoaiXe,
                SoKhung: thongtin.SoKhung,
                SoMay: thongtin.SoMay,
                DungTich: thongtin.DungTich,
                HopSo: thongtin.HopSo,
                MauSon: thongtin.MauSon,
                NamSanXuat: thongtin.NamSanXuat,
            };

            hangmuc.map(function (x) {
                x['MaPhieuTiepNhan'] = thongtin.MaPhieuTiepNhan // get infor old --> used to delete image
                x['FileName'] = commonStatisJs.CheckNull(x.Anh) ? '' : x.Anh.split('/')[x.Anh.split('/').length - 1]// get file name
                x['InforImage'] = [
                    {
                        file: x.Anh,
                        URLAnh: x.Anh
                    }
                ]
            });
            vatdung.map(function (x) {
                x['MaPhieuTiepNhan'] = thongtin.MaPhieuTiepNhan
                x['FileName'] = commonStatisJs.CheckNull(x.FileDinhKem) ? '' : x.FileDinhKem.split('/')[x.FileDinhKem.split('/').length - 1]
                x['InforImage'] = [
                    {
                        file: x.FileDinhKem,
                        URLAnh: x.FileDinhKem
                    }
                ]
            })

            // get list path img old
            self.ListImgOld = [];
            for (let i = 0; i < hangmuc.length; i++) {
                let hm = hangmuc[i];
                if (!commonStatisJs.CheckNull(hm.Anh)) {
                    self.ListImgOld.push(hm.Anh);
                }
            }
            for (let i = 0; i < vatdung.length; i++) {
                let hm = vatdung[i];
                if (!commonStatisJs.CheckNull(hm.FileDinhKem)) {
                    self.ListImgOld.push(hm.FileDinhKem);
                }
            }
            self.listData.HangMucSuaChuas = hangmuc;
            self.listData.HangMucSuaChuas.push(self.CreateNew_ItemFix());

            self.listData.GiayToDinhKems = vatdung;
            self.listData.GiayToDinhKems.push(self.CreateNew_ItemAttach());
            $('#TiepNhanXeModal').modal('show');
        },
        FocusInput_AfterSelect: function (elm) {
            $(elm).closest('div').hide();
            $(elm).closest('div').prev('focus');
        },

        GetInforCar_byID: function (id, type = 0) {
            var self = this;
            if (!commonStatisJs.CheckNull(id)) {
                $.getJSON('/api/DanhMuc/GaraAPI/GetInforCar_ByID?id=' + id).done(function (x) {
                    if (x.res && x.dataSoure.length > 0) {
                        if (type === 2) {
                            vmThemMoiXe.ShowModalUpdate(x.dataSoure[0]);
                        }
                        else {
                            self.carChosing = x.dataSoure[0];
                        }
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(x.mess);
                    }
                });
            }
        },

        GetThongTinChuXe: function () {
            var self = this;
            // get infor customer by id_Xe
            $.getJSON('/api/DanhMuc/GaraAPI/GetInforCustomer_byIDXe?idXe=' + self.newPhieuTiepNhan.ID_Xe).done(function (x) {
                if (x.res) {
                    if (x.dataSoure.length > 0) {
                        self.ChuXe = x.dataSoure[0];
                    }
                    else {
                        self.ChuXe = {};
                    }
                }
                else {
                    commonStatisJs.ShowMessageDanger(x.mess);
                }
            });
        },
        Change_LaChuXe: function () {
            var self = this;
            if (!self.role.PhieuTiepNhan.CapNhat || !self.role.PhieuTiepNhan.ThemMoi) {
                return false;
            }
            if (self.newPhieuTiepNhan.LaChuXe) {
                if (commonStatisJs.CheckNull(self.newPhieuTiepNhan.ID_Xe)) {
                    commonStatisJs.ShowMessageDanger('Chưa nhập thông tin xe');
                    return;
                }
                if (!self.ChuXe.ID) {
                    commonStatisJs.ShowMessageDanger('Chưa đăng ký chủ xe');
                    return false;
                }
                self.newPhieuTiepNhan.ID_KhachHang = self.ChuXe.ID;
                self.customerChosing = self.ChuXe;
            }
            else {
                // reset customer
                self.newPhieuTiepNhan.ID_KhachHang = null;
                self.customerChosing = {
                    ID: null,
                    MaDoiTuong: '',
                    TenDoiTuong: '',
                    Email: '',
                    DienThoai: '',
                    DiaChi: ''
                };
            }
        },
        ChangeCreator: function (item) {
            this.staffName = item.TenNhanVien;
            this.newPhieuTiepNhan.ID_NhanVien = item.ID;
            this.FocusInput_AfterSelect(event.currentTarget);
        },
        ResetAdviser: function () {
            let self = this;
            self.newPhieuTiepNhan.ID_CoVanDichVu = null;
            self.adviserName = '';
        },
        ChangeAdviser: function (item) {
            this.newPhieuTiepNhan.ID_CoVanDichVu = item.ID;
            this.adviserName = item.TenNhanVien;// cố vấn
            this.FocusInput_AfterSelect(event.currentTarget);
        },
        ChangeNgayTiepNhan: function (e) {
            var dt = moment(e).format('YYYY-MM-DD HH:mm');
            this.newPhieuTiepNhan.NgayVaoXuong = dt;
        },
        ChangeNgayXuatXuong: function (e) {
            var dt = moment(e).format('YYYY-MM-DD HH:mm');
            this.newPhieuTiepNhan.NgayXuatXuongDuKien = dt;
        },
        showModalNewCar: function () {
            vmThemMoiXe.inforLogin = this.inforLogin;
            vmThemMoiXe.ShowModalNewCar();
        },
        ChangeCar: function (item) {
            var self = this;
            self.newPhieuTiepNhan.ID_Xe = item.ID;
            // reset infor cus
            self.newPhieuTiepNhan.LaChuXe = false;
            self.newPhieuTiepNhan.ID_KhachHang = null;
            self.customerChosing.TenDoiTuong = '';
            self.customerChosing.Email = '';
            self.customerChosing.DienThoai = '';
            self.customerChosing.DiaChi = '';

            // use when print phieuTN
            self.GetInforCar_byID(item.ID);
            self.GetThongTinChuXe();
        },
        ChangeCustomer: function (item) {
            var self = this;
            self.newPhieuTiepNhan.ID_KhachHang = item.ID;
            self.customerChosing = item;
            self.FocusInput_AfterSelect(event.currentTarget);
        },
        ResetInsurence: function () {
            var self = this;
            self.newPhieuTiepNhan.ID_BaoHiem = null;
            self.newPhieuTiepNhan.NguoiLienHeBH = '';
            self.newPhieuTiepNhan.SoDienThoaiLienHeBH = '';
            self.insurenceChosing = {
                ID: null,
                MaBaoHiem: '',
                TenBaoHiem: '',
            };
        },
        ChangeInsurence: function (item) {
            var self = this;
            self.newPhieuTiepNhan.ID_BaoHiem = item.ID;
            self.insurenceChosing = {
                ID: item.ID,
                MaBaoHiem: item.MaDoiTuong,
                TenBaoHiem: item.TenDoiTuong,
            };
            if (!commonStatisJs.CheckNull(event)) {
                self.FocusInput_AfterSelect(event.currentTarget);
            }
        },
        CreateNew_ItemFix: function () {
            return {
                IDRandom: CreateIDRandom('fix_'),
                TenHangMuc: '',
                TinhTrang: '',
                PhuongAnSuaChua: '',
                Anh: '',
                FileName: '',
                InforImage: [],
            }
        },
        CreateNew_ItemAttach: function () {
            return {
                IDRandom: CreateIDRandom('att_'),
                TieuDe: '',
                SoLuong: '',
                FileDinhKem: '',
                FileName: '',
                InforImage: [],
            }
        },
        AddNew_ItemFix: function (index) {
            var self = this;
            var $this = $(event.currentTarget);
            if (self.listData.HangMucSuaChuas[index].TenHangMuc !== '') {
                var obj = self.CreateNew_ItemFix();
                self.listData.HangMucSuaChuas.insert(index + 1, obj);
                $(function () {
                    $this.closest('tr').next().find('td').eq(1).find('input').focus();
                })
            }
        },
        Remove_ItemFix: function (index) {
            var self = this;
            if (self.listData.HangMucSuaChuas.length > 1) {
                self.listData.HangMucSuaChuas.splice(index, 1);
            }
        },
        AddNew_ItemAttach: function (index) {
            var self = this;
            var $this = $(event.currentTarget);
            if (self.listData.GiayToDinhKems[index].TenHangMuc !== '') {
                var obj = self.CreateNew_ItemAttach();
                self.listData.GiayToDinhKems.insert(index + 1, obj);
                $(function () {
                    $this.closest('tr').next().find('td').eq(1).find('input').focus();
                })
            }
        },
        Remove_ItemAttach: function (index) {
            var self = this;
            if (self.listData.GiayToDinhKems.length > 1) {
                self.listData.GiayToDinhKems.splice(index, 1);
            }
        },
        ZoomImage: function (index, type = 1) {

            var self = this;
            var arrImg = [];
            switch (type) {
                case 1:
                    for (let i = 0; i < self.listData.HangMucSuaChuas.length; i++) {
                        let itFor = self.listData.HangMucSuaChuas[i];
                        if (!commonStatisJs.CheckNull(itFor.Anh)) {
                            arrImg.push({
                                src: itFor.Anh.indexOf("data:image") >= 0 ? itFor.Anh : Open24FileManager.hostUrl + itFor.Anh,
                                caption: itFor.TenHangMuc,
                                active: index === i,
                            });
                        }
                    }
                    break;
                case 2:
                    for (let i = 0; i < self.listData.GiayToDinhKems.length; i++) {
                        let itFor = self.listData.GiayToDinhKems[i];
                        if (!commonStatisJs.CheckNull(itFor.FileDinhKem)) {
                            arrImg.push({
                                src: itFor.FileDinhKem.indexOf("data:image") >= 0 ? itFor.FileDinhKem : Open24FileManager.hostUrl + itFor.FileDinhKem,
                                caption: itFor.TieuDe,
                                active: index === i,
                            });
                        }
                    }
                    break;
            }
            VImageView.ListImg = arrImg;
            VImageView.openModal();
        },
        HangMuc_ChoseImage: function (index) {
            var self = this;
            var checkSize = commonStatisJs.checkSizeImage(event);
            if (!checkSize) {
                return;
            }

            var files = event.target.files;
            for (let k = 0; k < files.length; k++) {
                let f = files[k];

                if (!f.type.match('image.*')) {
                    continue;
                }

                let reader = new FileReader();
                // Closure to capture the file information.
                reader.onload = (function (thisFile) {
                    return function (e) {
                        for (let i = 0; i < self.listData.HangMucSuaChuas.length; i++) {
                            if (i === index) {
                                self.listData.HangMucSuaChuas[i].InforImage = [{
                                    file: thisFile,
                                    URLAnh: e.target.result
                                }]
                                self.listData.HangMucSuaChuas[i].FileName = thisFile.name;
                                self.listData.HangMucSuaChuas[i].Anh = e.target.result;// show when zoom img
                                break;
                            }
                        }
                        let arr = self.listData.HangMucSuaChuas;
                        self.listData.HangMucSuaChuas = $.extend([], true, arr);
                    };
                })(f);
                reader.readAsDataURL(f);
            }
        },
        VatDung_ChoseImage: function (index) {
            var self = this;
            var checkSize = commonStatisJs.checkSizeImage(event);
            if (!checkSize) {
                return;
            }

            var files = event.target.files;
            for (let k = 0; k < files.length; k++) {
                let f = files[k];

                if (!f.type.match('image.*')) {
                    continue;
                }
                let reader = new FileReader();
                reader.onload = (function (thisFile) {
                    return function (e) {
                        for (let i = 0; i < self.listData.GiayToDinhKems.length; i++) {
                            if (i === index) {
                                self.listData.GiayToDinhKems[i].InforImage = [{
                                    file: thisFile,
                                    URLAnh: e.target.result
                                }]
                                self.listData.GiayToDinhKems[i].FileName = thisFile.name;
                                self.listData.GiayToDinhKems[i].FileDinhKem = e.target.result;
                                break;
                            }
                        }
                        let arr = self.listData.GiayToDinhKems;
                        self.listData.GiayToDinhKems = $.extend([], true, arr);
                    };
                })(f);
                reader.readAsDataURL(f);
            }
        },
        Put_HangMucSuaChua: function () {
            var self = this;
            var myData = {
                idPhieuTN: self.newPhieuTiepNhan.ID,
                hangmuc: self.listData.HangMucSuaChuas.filter(x => x.TenHangMuc !== '' || x.TinhTrang !== ''),
            }
            let pathFile = '/ImageUpload/0973474985/PhieuTiepNhan/' + self.newPhieuTiepNhan.MaPhieuTiepNhan;
            for (let i = 0; i < myData.hangmuc.length; i++) {
                let itFor = myData.hangmuc[i];
                if (!commonStatisJs.CheckNull(itFor.FileName)) {
                    itFor.Anh = pathFile.concat('/', itFor.FileName);
                }
            }

            ajaxHelper('/api/DanhMuc/GaraAPI/' + 'Put_HangMucSuaChua', 'POST', myData).done(function (x) {
                if (x.res === true) {

                }
            });
        },
        Put_HangMucSuaChuaUpload: function (_hangmuc) {
            var self = this;
            var myData = {
                idPhieuTN: self.newPhieuTiepNhan.ID,
                hangmuc: _hangmuc,
            }

            ajaxHelper('/api/DanhMuc/GaraAPI/' + 'Put_HangMucSuaChua', 'POST', myData).done(function (x) {
                if (x.res === true) {

                }
            });
        },
        Put_GiayToKemTheo: function () {
            var self = this;
            var myData = {
                idPhieuTN: self.newPhieuTiepNhan.ID,
                vatdung: self.listData.GiayToDinhKems.filter(x => x.TieuDe !== ''),
            }
            let pathFile = '/ImageUpload/0973474985/PhieuTiepNhan/' + self.newPhieuTiepNhan.MaPhieuTiepNhan;
            for (let i = 0; i < myData.vatdung.length; i++) {
                let itFor = myData.vatdung[i];
                if (!commonStatisJs.CheckNull(itFor.FileName)) {
                    itFor.FileDinhKem = pathFile.concat('/', itFor.FileName);
                }
            }
            ajaxHelper('/api/DanhMuc/GaraAPI/' + 'Put_GiayToKemTheo', 'POST', myData).done(function (x) {
                if (x.res === true) {

                }
            });
        },
        Put_GiayToKemTheoUpload: function (_vatdung) {
            var self = this;
            var myData = {
                idPhieuTN: self.newPhieuTiepNhan.ID,
                vatdung: _vatdung
            }
            ajaxHelper('/api/DanhMuc/GaraAPI/' + 'Put_GiayToKemTheo', 'POST', myData).done(function (x) {
                if (x.res === true) {

                }
            });
        },
        DeleteImageOld_ByPhieuTiepNhan: function () {
            var self = this;

            var arrFileNew = [];
            for (let i = 0; i < self.listData.HangMucSuaChuas.length; i++) {
                let hm = self.listData.HangMucSuaChuas[i];
                for (let k = 0; k < hm.InforImage.length; k++) {
                    if (typeof (hm.InforImage[k].file) !== 'object') {
                        arrFileNew.push(hm.Anh);
                    }
                }
            }
            for (let i = 0; i < self.listData.GiayToDinhKems.length; i++) {
                let hm = self.listData.GiayToDinhKems[i];
                for (let k = 0; k < hm.InforImage.length; k++) {
                    if (typeof (hm.InforImage[k].file) !== 'object') {
                        arrFileNew.push(hm.FileDinhKem);
                    }
                }
            }
            // get file exist in old, but not in new
            var fileDelete = $.grep(self.ListImgOld, function (x) {
                return $.inArray(x, arrFileNew) == -1;
            })
            Open24FileManager.RemoveFiles(fileDelete);
            //var myData = {
            //    lstFile: fileDelete,
            //}
            //ajaxHelper('/api/DanhMuc/DM_DoiTuongAPI/' + 'DeleteFile_inFolder', 'POST', myData).done(function (x) {
            //});
        },
        UploadFile: function () {
            var self = this;

            //vật dụng đính kèm
            let myDataVatDung = {};
            myDataVatDung.Subdomain = VHeader.SubDomain;
            myDataVatDung.Function = "5";
            myDataVatDung.Id = self.newPhieuTiepNhan.ID;
            let vatdung = self.listData.GiayToDinhKems.filter(x => x.TieuDe !== '');
            for (let i = 0; i < vatdung.length; i++) {
                let itFor = vatdung[i];
                if (itFor.InforImage.length > 0 && !commonStatisJs.CheckNull(itFor.InforImage[0].file)) {
                    var formData = new FormData();
                    formData.append("files", itFor.InforImage[0].file);

                    myDataVatDung.files = formData;
                    var result = Open24FileManager.UploadImage(myDataVatDung);
                    if (result.length > 0) {
                        vatdung[i].FileDinhKem = result[0];
                    }
                }
            }
            self.Put_GiayToKemTheoUpload(vatdung);
            //Hạng mục sửa chữa
            let myDataHangMuc = {};
            myDataHangMuc.Subdomain = VHeader.SubDomain;
            myDataHangMuc.Function = "5";
            myDataHangMuc.Id = self.newPhieuTiepNhan.ID;
            let hangmuc = self.listData.HangMucSuaChuas.filter(x => x.TenHangMuc !== '' || x.TinhTrang !== '');
            for (let i = 0; i < hangmuc.length; i++) {
                let itFor = hangmuc[i];
                if (itFor.InforImage.length > 0 && !commonStatisJs.CheckNull(itFor.InforImage[0].file)) {
                    var formData = new FormData();
                    formData.append("files", itFor.InforImage[0].file);

                    myDataHangMuc.files = formData;
                    var result = Open24FileManager.UploadImage(myDataHangMuc);
                    if (result.length > 0) {
                        hangmuc[i].Anh = result[0];
                    }
                }
            }
            self.Put_HangMucSuaChuaUpload(hangmuc);
            //$.ajax({
            //    type: "POST",
            //    url: '/api/DanhMuc/DM_DoiTuongAPI/' + "ImageUpload?rootFolder=PhieuTiepNhan&subFolder=" + maphieuTN,
            //    data: formData,
            //    dataType: 'json',
            //    contentType: false,
            //    processData: false,
            //}).done(function (x) {
            //});

        },
        AddNew_PhieuTiepNhan: function () {
            var self = this;
            if (commonStatisJs.CheckNull(self.newPhieuTiepNhan.ID_KhachHang)) {
                commonStatisJs.ShowMessageDanger('Vui lòng chọn khách hàng');
                return;
            }
            if (commonStatisJs.CheckNull(self.newPhieuTiepNhan.ID_Xe)) {
                commonStatisJs.ShowMessageDanger('Vui lòng nhập biển số xe');
                return;
            }
            if (self.newPhieuTiepNhan.SoKmRa === 0) {
                self.newPhieuTiepNhan.SoKmRa = self.newPhieuTiepNhan.SoKmVao;
            }
            var vatdungSL0 = self.listData.GiayToDinhKems.filter(x => x.SoLuong === '' && x.TieuDe !== '');
            if (vatdungSL0.length > 0) {
                commonStatisJs.ShowMessageDanger('Vui lòng nhập số lượng vật dụng');
                return;
            }
            self.isLoading = true;

            var myData = self.newPhieuTiepNhan;

            var noidung = self.isNew ? 'Thêm mới phiếu tiếp nhận xe ' : 'Cập nhật phiếu tiếp nhận xe ';
            var noidungct = ' <br /> - Biển số: '.concat(self.carChosing.BienSo,
                ' <br /> - Số km vào: ', myData.SoKmVao,
                ' <br /> - Khách hàng: ', self.customerChosing.TenDoiTuong, ' (', self.customerChosing.MaDoiTuong, '), ',
                ' <br /> - Công ty bảo hiểm: ', self.insurenceChosing.TenBaoHiem, ' (', self.insurenceChosing.MaBaoHiem, '), ',
                ' <br /> - Cố vấn: ', self.adviserName);

            var diary = {
                ID_DonVi: self.inforLogin.ID_DonVi,
                ID_NhanVien: self.inforLogin.ID_NhanVien,
                ChucNang: 'Tiếp nhận xe',
                NoiDung: noidung,
                NoiDungChiTiet: noidung.concat(noidungct),
            }

            if (self.isNew) {
                myData.ID = self.GuidEmpty;
                myData.NguoiTao = self.inforLogin.UserLogin;

                ajaxHelper('/api/DanhMuc/GaraAPI/' + 'Post_PhieuTiepNhan', 'POST', myData).done(function (x) {
                    if (x.res === true) {
                        self.saveOK = true;
                        self.newPhieuTiepNhan.ID = x.dataSoure.ID;
                        self.newPhieuTiepNhan.MaPhieuTiepNhan = x.dataSoure.MaPhieuTiepNhan;
                        commonStatisJs.ShowMessageSuccess('Tiếp nhận xe thành công');

                        // save diary
                        diary.LoaiNhatKy = 1;
                        diary.NoiDung = diary.NoiDung.concat(x.dataSoure.MaPhieuTiepNhan);
                        Insert_NhatKyThaoTac_1Param(diary);

                        //self.Put_HangMucSuaChua();
                        //self.Put_GiayToKemTheo();
                        self.DeleteImageOld_ByPhieuTiepNhan();
                        self.UploadFile();
                        $('#TiepNhanXeModal').modal('hide');
                    }
                    else {
                        self.saveOK = false;
                        commonStatisJs.ShowMessageDanger(x.mess);
                    }
                    self.isLoading = false;
                });
            }
            else {
                self.newPhieuTiepNhan.NguoiSua = self.inforLogin.UserLogin;

                let checkUpdate = false;
                if (self.phieuTiepNhanOld.ID_BaoHiem !== self.newPhieuTiepNhan.ID_BaoHiem) {
                    checkUpdate = true;
                }

                if (checkUpdate === false) {
                    if (self.phieuTiepNhanOld.ID_KhachHang !== self.newPhieuTiepNhan.ID_KhachHang) {
                        checkUpdate = true;
                    }
                }

                if (checkUpdate) {
                    ajaxHelper('/api/DanhMuc/GaraAPI/PTN_CheckChangeCus', 'POST', self.newPhieuTiepNhan).done(function (x) {
                        if (x.res) {
                            console.log(x.dataSoure)
                            self.cmp.ListChose = [];
                            if (x.dataSoure.length > 0) {
                                let exInvoice = $.grep(x.dataSoure, function (o) {
                                    return $.inArray(o.Loai, [1, 2, 3, 4]) > -1;
                                });
                                if (exInvoice.length > 0) {
                                    self.cmp.title = 'Xác nhận cập nhật';

                                    self.cmp.ListChose.push({ ID: 0, Text: 'Chỉ cập nhật thông tin phiếu tiếp nhận', Value: true });

                                    if (x.dataSoure.filter(x => x.Loai === 3).length > 0) {
                                        self.cmp.mes = '<li class="floatleft"> Tồn tại hóa đơn khác khách hàng </li>';
                                        self.cmp.ListChose.push({ ID: 3, Text: 'Cập nhật lại khách hàng cho hóa đơn + Phiếu thu liên quan', Value: true });
                                    }
                                    else {
                                        if (x.dataSoure.filter(x => x.Loai === 1).length > 0) {
                                            self.cmp.mes += '<li class="floatleft"> Tồn tại hóa đơn khác khách hàng </li>';
                                            self.cmp.ListChose.push({ ID: 1, Text: 'Cập nhật lại khách hàng cho hóa đơn', Value: true });
                                        }
                                    }

                                    if (x.dataSoure.filter(x => x.Loai === 4).length > 0) {
                                        self.cmp.mes += '<li class="floatleft"> Tồn tại hóa đơn khác công ty bảo hiểm </li>';
                                        self.cmp.ListChose.push({ ID: 4, Text: 'Cập nhật lại công ty bảo hiểm cho hóa đơn + Phiếu thu liên quan', Value: true });
                                    }
                                    else {
                                        if (x.dataSoure.filter(x => x.Loai === 2).length > 0) {
                                            if (x.dataSoure.filter(x => x.Loai === 2).length > 0) {
                                                self.cmp.mes += '<li class="floatleft"> Tồn tại hóa đơn khác công ty bảo hiểm </li>';
                                                self.cmp.ListChose.push({ ID: 2, Text: 'Cập nhật lại công ty bảo hiểm cho hóa đơn', Value: true });
                                            }
                                        }
                                    }

                                    self.cmp.show = true;
                                    $('#ptn_xacnhan_capnhat').modal('show');
                                    self.isLoading = false;
                                }
                                else {
                                    self.isLoading = true;
                                    self.PutPhieuTiepNhan(myData, diary);
                                }
                            }
                            else {
                                self.isLoading = true;
                                self.PutPhieuTiepNhan(myData, diary);
                            }
                        }
                        else {
                            self.isLoading = false;
                            commonStatisJs.ShowMessageDanger(x.mess);
                        }
                    })
                }
                else {
                    self.isLoading = true;
                    self.PutPhieuTiepNhan(myData, diary);
                }
            }
        },
        PutPhieuTiepNhan: function (myData, diary) {
            var self = this;
            ajaxHelper('/api/DanhMuc/GaraAPI/' + 'Put_PhieuTiepNhan', 'POST', myData).done(function (x) {
                if (x.res === true) {
                    self.saveOK = true;
                    self.newPhieuTiepNhan.MaPhieuTiepNhan = x.dataSoure.MaPhieuTiepNhan;
                    commonStatisJs.ShowMessageSuccess('Cập nhật phiếu tiếp nhận thành công');

                    diary.LoaiNhatKy = 2;
                    diary.NoiDung = diary.NoiDung.concat(x.dataSoure.MaPhieuTiepNhan);
                    diary.NoiDungChiTiet = diary.NoiDungChiTiet.concat(' <br /> - <b> Thông tin cũ : </b>',
                        ' <br /> - Mã phiếu: ', self.phieuTiepNhanOld.MaPhieuTiepNhan,
                        ' <br /> - Biển số: ', self.phieuTiepNhanOld.BienSo,
                        ' <br /> - Khách hàng: ', self.phieuTiepNhanOld.TenDoiTuong,
                        ' <br /> - Công ty bảo hiểm: ', self.phieuTiepNhanOld.TenBaoHiem,
                        ' (', self.phieuTiepNhanOld.MaBaoHiem, ') <br /> - Cố vấn: ', self.phieuTiepNhanOld.CoVanDichVu,
                        ' <br /> - Số km vào: ', self.phieuTiepNhanOld.SoKmVao);
                    Insert_NhatKyThaoTac_1Param(diary);

                    //self.Put_HangMucSuaChua();
                    //self.Put_GiayToKemTheo();
                    self.DeleteImageOld_ByPhieuTiepNhan();
                    self.UploadFile();
                    self.LichBaoDuong_Update();
                    $('#TiepNhanXeModal').modal('hide');
                }
                else {
                    self.saveOK = false;
                    commonStatisJs.ShowMessageDanger(x.mess);
                }
                self.isLoading = false;
            });
        },

        showModalCustomer: function () {
            var self = this;
            vmThemMoiKhach.inforLogin = self.inforLogin;
            vmThemMoiKhach.showModalAdd();
        },
        UpdateCustomer: async function () {
            let self = this;
            vmThemMoiKhach.inforLogin = self.inforLogin;

            let cus = await vmThemMoiKhach.GetInforKhachHangFromDB_ByID(self.newPhieuTiepNhan.ID_KhachHang);
            if (cus !== null && cus.length > 0) {
                vmThemMoiKhach.showModalUpdate(cus[0]);
            }
        },
        ResetCustomer: function () {
            var self = this;
            self.newPhieuTiepNhan.ID_KhachHang = null;
            self.customerChosing = {
                ID: null,
                MaDoiTuong: '',
                TenDoiTuong: '',
                Email: '',
                DienThoai: '',
                DiaChi: ''
            };
        },
        Change_TrangThaiPhieuTN: function (item) {
            var self = this;
            self.newPhieuTiepNhan.TrangThai = item.ID;
            self.trangthaiPhieuTN = item.Text;
        },

        LichBaoDuong_Update: function () {
            var self = this;
            var chenhlechKM = formatNumberToFloat(self.newPhieuTiepNhan.SoKmVao) - self.phieuTiepNhanOld.SoKmVao;
            if (chenhlechKM != 0) {
                ajaxHelper('/api/DanhMuc/GaraAPI/' + 'UpdateLichBaoDuong_whenUpdateSoKM_ofPhieuTN?idPhieuTN=' +
                    self.newPhieuTiepNhan.ID + '&chenhLechKM=' + chenhlechKM, 'GET').done(function (x) {
                        console.log(x);
                    })
            }
        },
        Agree_thaydoiPTN: function (typeChosed) {
            var self = this;
            ajaxHelper('/api/DanhMuc/GaraAPI/' + 'Put_PhieuTiepNhan', 'POST', self.newPhieuTiepNhan).done(function (x) {
                if (x.res === true) {
                    self.saveOK = true;
                    self.newPhieuTiepNhan.MaPhieuTiepNhan = x.dataSoure.MaPhieuTiepNhan;

                    let noidung = 'Cập nhật phiếu tiếp nhận xe ';
                    let noidungct = ' <br /> - Biển số: '.concat(self.carChosing.BienSo,
                        ' <br /> - Số km vào: ', self.newPhieuTiepNhan.SoKmVao,
                        ' <br /> - Khách hàng: ', self.customerChosing.TenDoiTuong, ' (', self.customerChosing.MaDoiTuong, '), ',
                        ' <br /> - Công ty bảo hiểm: ', self.insurenceChosing.TenBaoHiem, ' (', self.insurenceChosing.MaBaoHiem, '), ',
                        ' <br /> - Cố vấn: ', self.adviserName,
                        ' <br /> - <b> Thông tin cũ : </b>',
                        ' <br /> - Mã phiếu: ', self.phieuTiepNhanOld.MaPhieuTiepNhan,
                        ' <br /> - Biển số: ', self.phieuTiepNhanOld.BienSo,
                        ' <br /> - Khách hàng: ', self.phieuTiepNhanOld.TenDoiTuong,
                        ' <br /> - Công ty bảo hiểm: ', self.phieuTiepNhanOld.TenBaoHiem,
                        ' (', self.phieuTiepNhanOld.MaBaoHiem, ') <br /> - Cố vấn: ', self.phieuTiepNhanOld.CoVanDichVu,
                        ' <br /> - Số km vào: ', self.phieuTiepNhanOld.SoKmVao);

                    let diary = {
                        ID_DonVi: self.inforLogin.ID_DonVi,
                        ID_NhanVien: self.inforLogin.ID_NhanVien,
                        ChucNang: 'Tiếp nhận xe',
                        NoiDung: noidung + x.dataSoure.MaPhieuTiepNhan,
                        NoiDungChiTiet: noidung.concat(noidungct),
                    }
                    Insert_NhatKyThaoTac_1Param(diary);

                    self.ChangePTN_updateCus(typeChosed);
                    //self.Put_HangMucSuaChua();
                    //self.Put_GiayToKemTheo();
                    self.DeleteImageOld_ByPhieuTiepNhan();
                    self.UploadFile();
                    self.LichBaoDuong_Update();
                    commonStatisJs.ShowMessageSuccess('Cập nhật phiếu tiếp nhận thành công');
                    $('#TiepNhanXeModal').modal('hide');
                }
                else {
                    self.saveOK = false;
                    commonStatisJs.ShowMessageDanger(x.mess);
                }
                self.isLoading = false;
            });
        },
        ChangePTN_updateCus: function (arrType) {
            var self = this;
            let sLoai = 'Cập nhật thông tin phiếu tiếp nhận, ', sOldNew = '';

            let arr = arrType.map(function (x) { return x.ID });

            if (arr.filter(x => x === 1).length > 0 || arr.filter(x => x === 3).length > 0) {
                sLoai += 'khách hàng, ';
                sOldNew = '<br /> - Khách hàng cũ: '.concat(self.phieuTiepNhanOld.TenDoiTuong,
                    '<br /> - Khách hàng mới: ', self.customerChosing.TenDoiTuong);
            }
            if (arr.filter(x => x === 2).length > 0 || arr.filter(x => x === 4).length > 0) {
                sLoai += 'công ty bảo hiểm, ';
                sOldNew += '<br /> - Cty bảo hiểm cũ: '.concat(self.phieuTiepNhanOld.TenBaoHiem,
                    '<br /> - Cty bảo hiểm mới: ', self.insurenceChosing.TenBaoHiem);
            }

            if (arr.filter(x => x === 1).length > 0 || arr.filter(x => x === 2).length > 0) {
                sLoai += 'phiếu thu liên quan,';
                sOldNew += ' <br /> - Cập nhật phiếu thu liên quan: Có '
            }
            else {
                sOldNew += ' <br /> - Cập nhật phiếu thu liên quan: Không '
            }

            sLoai = Remove_LastComma(sLoai);

            let myData = {
                objPhieuTN: self.phieuTiepNhanOld,
                arrType: arr,
            }

            console.log(myData);
            ajaxHelper('/api/DanhMuc/GaraAPI/ChangePTN_updateCus', 'POST', myData).done(function (x) {
                if (x.res) {
                    let diary = {
                        ID_DonVi: self.inforLogin.ID_DonVi,
                        ID_NhanVien: self.inforLogin.ID_NhanVien,
                        ChucNang: 'Tiếp nhận xe',
                        NoiDung: 'Cập nhật phiếu tiếp nhận - ' + sLoai,
                        NoiDungChiTiet: 'Cập nhật phiếu tiếp nhận - '.concat(sLoai,
                            '<br /><b> Thông tin chi tiết: </b>', sOldNew),
                    }
                    Insert_NhatKyThaoTac_1Param(diary);
                }
                $('#ptn_xacnhan_capnhat').modal('hide');
                self.cmp.show = false;
                self.isLoading = false;
            })
        },
        insertBaoHiem: function () {
            vThemMoiBaoHiem.ShowModalThemMoi();
        },
        updateBaoHiem: function () {
            let self = this;
            vThemMoiBaoHiem.GetInforKhachHangFromDB_ByID(self.newPhieuTiepNhan.ID_BaoHiem, true);
        },
        updateCar: function () {
            let self = this;
            vmThemMoiXe.inforLogin = self.inforLogin;
            self.GetInforCar_byID(self.newPhieuTiepNhan.ID_Xe, 2);
        }
    },
})

// insert object at index
Array.prototype.insert = function (index, item) {
    this.splice(index, 0, item);
};

$(function () {
    $('#ThemMoiXemModal').on('hidden.bs.modal', function () {
        if (vmThemMoiXe.saveOK) {
            vmTiepNhanXe.newPhieuTiepNhan.ID_Xe = vmThemMoiXe.newCar.ID;
            vmTiepNhanXe.carChosing = vmThemMoiXe.newCar;
            vmTiepNhanXe.GetInforCar_byID(vmThemMoiXe.newCar.ID);
            vmTiepNhanXe.GetThongTinChuXe();
        }
    })

    $('#ThemMoiKhachHang').on('hidden.bs.modal', function () {
        // check if isShowing modal themmoixe
        if (!$('#ThemMoiXemModal').hasClass('in')) {
            if (vmThemMoiKhach.saveOK) {
                vmTiepNhanXe.newPhieuTiepNhan.ID_KhachHang = vmThemMoiKhach.customerDoing.ID;

                let diachi = vmThemMoiKhach.customerDoing.DiaChi;

                let quanhuyen = vmThemMoiKhach.customerDoing.TenQuanHuyen + ', ';
                if (commonStatisJs.CheckNull(quanhuyen)) quanhuyen = '';

                let tinhthanh = vmThemMoiKhach.customerDoing.TenTinhThanh;
                if (commonStatisJs.CheckNull(tinhthanh)) tinhthanh = '';

                diachi = Remove_LastComma(diachi.concat(', ', quanhuyen, tinhthanh));

                vmTiepNhanXe.customerChosing = {
                    ID: vmThemMoiKhach.customerDoing.ID,
                    MaDoiTuong: vmThemMoiKhach.customerDoing.MaDoiTuong,
                    TenDoiTuong: vmThemMoiKhach.customerDoing.TenDoiTuong,
                    Email: vmThemMoiKhach.customerDoing.Email,
                    DienThoai: vmThemMoiKhach.customerDoing.DienThoai,
                    DiaChi: diachi
                }
            }
        }
    })

    $('#DoiTacBaoHiem').on('hidden.bs.modal', function () {
        if (vThemMoiBaoHiem.isSaveOk) {
            vmTiepNhanXe.ChangeInsurence(vThemMoiBaoHiem.objBaoHiem);
        }
    })
})
