var vmThemMoiXe = new Vue({
    el: '#ThemMoiXemModal',
    components: {
        'car-manufacturer': ComponentChoseManufacturer,
        'car-model': ComponentChoseModelCar,
        'car-type': ComponentChoseTypeCar,
        'customers': cmpChoseCustomer,
    },
    data: {
        saveOK: false,
        isNew: true,
        LaHangHoa: false,
        isLoading: false,
        modelCarName: '',
        manufacturerName: '',
        typeCarName: '',
        OldCar: {},
        role: {
            HangXe: {},
            LoaiXe: {},
            MauXe: {},
            KhachHang: {},
        },
        inforLogin: {
            ID_NhanVien: null,
            ID_User: null,
            UserLogin: null,
            ID_DonVi: null,
        },
        customerChosing: { TenDoiTuong: '', ID: null },

        listData: {
            HangXe: [],
            LoaiXe: [],
            MauXe: [],
            KhachHang: [],
        },
        newCar: {
            ID: null,
            ID_MauXe: null,// ~ type
            ID_KhachHang: null,
            BienSo: '',
            SoKhung: '',
            SoMay: '',
            MauXe: '',// color
            DungTich: '',
            HopSo: '',
            GhiChu: '',
            TrangThai: 1,
            ID_HangXe: null,
            ID_LoaiXe: null,
            TenMauXe: '',
            TenHangXe: '',
            DienThoai: '',
            MauSon: '',
            NguoiSoHuu: 0
        },
    },
    created: function () {
        var self = this;
        self.GaraAPI = '/api/DanhMuc/GaraAPI/';
        self.Guid_Empty = '00000000-0000-0000-0000-000000000000';

        let idDonVi = $('#txtDonVi').val();
        if (commonStatisJs.CheckNull(idDonVi)) {
            self.inforLogin = {
                ID_NhanVien: VHeader.IdNhanVien,
                ID_User: VHeader.IdNguoiDung,
                UserLogin: VHeader.UserLogin,
                ID_DonVi: VHeader.IdDonVi,
            };
        }
        self.role.ThemMoi = self.CheckRole('DanhMucXe_ThemMoi');
        self.role.CapNhat = self.CheckRole('DanhMucXe_CapNhat');
        self.role.Xoa = self.CheckRole('DanhMucXe_Xoa');
        self.role.NhapFile = self.CheckRole('DanhMucXe_NhapFile');
        self.role.XuatFile = self.CheckRole('DanhMucXe_XuatFile');
        self.role.KhachHang.ThemMoi = self.CheckRole('KhachHang_ThemMoi');
        self.role.KhachHang.CapNhat = self.CheckRole('KhachHang_CapNhat');
        self.role.NhatKyHoatDongXe = self.CheckRole('HoatDongXe');

        self.GetAllModelCar();
        self.GetAllHangXes();
        self.GetAllLoaiXes();
    },
    methods: {
        CheckRole: function (maquyen) {
            if (!commonStatisJs.CheckNull($('#txtDonVi').val())) {
                return true;
            }
            else {
                if (VHeader) {
                    var role = $.grep(VHeader.Quyen, function (x) {
                        return x.indexOf(maquyen) > -1;
                    });
                    return role.length > 0;
                }
            }
            return false;
        },
        GetAllModelCar: function () {
            var self = this;
            $.getJSON(self.GaraAPI + "GetAllDanhMucMauXe").done(function (x) {
                if (x.res) {
                    self.listData.MauXe = x.dataSoure;
                }
            });
        },
        GetAllHangXes: function () {
            var self = this;
            $.getJSON(self.GaraAPI + "GetAllHangXes").done(function (x) {
                if (x.res) {
                    self.listData.HangXe = x.dataSoure;
                }
            });
        },
        GetAllLoaiXes: function () {
            var self = this;
            $.getJSON(self.GaraAPI + "GetAllLoaiXes").done(function (x) {
                if (x.res) {
                    self.listData.LoaiXe = x.dataSoure;
                }
            });
        },
        GetInforCar_byID: function (id, type = 1) {
            let self = this;
            if (!commonStatisJs.CheckNull(id)) {
                $.getJSON('/api/DanhMuc/GaraAPI/GetInforCar_ByID?id=' + id).done(function (x) {
                    if (x.res && x.dataSoure.length > 0) {
                        console.log(x.dataSoure)
                        if (type === 2) {
                            vmThemMoiXe.ShowModalUpdate(x.dataSoure[0]);
                        }
                        self.OldCar = $.extend({}, x.dataSoure[0]);
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(x.mess);
                    }
                });
            }
        },

        ShowModalNewCar: function (chuxe = null) {
            var self = this;
            self.saveOK = false;
            self.isNew = true;
            self.LaHangHoa = false;
            self.modelCarName = '';
            self.manufacturerName = '';
            self.typeCarName = '';
            self.customerChosing =
            {
                TenDoiTuong: '',
                ID: null
            };

            self.newCar = {
                ID: null,
                ID_MauXe: null,
                ID_HangXe: self.Guid_Empty,
                ID_LoaiXe: self.Guid_Empty,
                ID_KhachHang: null,
                BienSo: '',
                SoKhung: '',
                NamSanXuat: '',
                SoMay: '',
                MauXe: '',
                DungTich: '',
                HopSo: '',
                GhiChu: '',
                TrangThai: 1,
                MauSon: '',
                DienThoai: '',
                NguoiSoHuu: 0
            }
            if (chuxe !== null) {
                self.customerChosing.TenDoiTuong = chuxe.TenDoiTuong;
                self.customerChosing.DienThoai = chuxe.DienThoai;
                self.newCar.ID_KhachHang = chuxe.ID;
            }
            $('#ThemMoiXemModal').modal('show');
        },
        ShowModalUpdate: function (item) {
            var self = this;
            self.saveOK = false;
            self.isNew = false;
            self.LaHangHoa = item.ID_HangHoa !== null;
            self.OldCar = $.extend({}, item);
            self.modelCarName = item.TenMauXe;
            self.manufacturerName = item.TenHangXe;
            self.typeCarName = item.TenLoaiXe;
            self.customerChosing.TenDoiTuong = item.TenDoiTuong;
            self.customerChosing.DienThoai = item.DienThoai;
            self.newCar = {
                ID: item.ID,
                ID_MauXe: item.ID_MauXe,
                ID_HangXe: item.ID_HangXe,
                ID_LoaiXe: item.ID_LoaiXe,
                ID_KhachHang: item.ID_KhachHang,
                BienSo: item.BienSo,
                SoKhung: item.SoKhung,
                NamSanXuat: item.NamSanXuat,
                SoMay: item.SoMay,
                MauXe: item.MauXe,
                DungTich: item.DungTich,
                HopSo: item.HopSo,
                GhiChu: item.GhiChu,
                TrangThai: item.TrangThai,
                MauSon: item.MauSon,
                DienThoai: item.DienThoai,
                NguoiSoHuu: item.NguoiSoHuu
            }
            $('#ThemMoiXemModal').modal('show');
        },

        Reset_MauXe: function () {
            var self = this;
            self.newCar.ID_MauXe = null;
            self.newCar.ID_HangXe = self.Guid_Empty;
            self.newCar.ID_LoaiXe = self.Guid_Empty;
            self.modelCarName = '';
            self.manufacturerName = '';
            self.typeCarName = '';
        },
        changeModelCarParent: function (item) {
            var self = this;
            self.modelCarName = item.TenMauXe;
            self.newCar.ID_MauXe = item.ID;
            self.newCar.TenMauXe = item.TenMauXe;// bind at modal tiepnhanxe

            // get loaixe, hangxe by mauxe
            var hangxe = $.grep(self.listData.HangXe, function (x) {
                return x.ID === item.ID_HangXe;
            });
            if (hangxe.length > 0) {
                self.manufacturerName = hangxe[0].TenHangXe;
                self.newCar.ID_HangXe = hangxe[0].ID;
            }

            var loaixe = $.grep(self.listData.LoaiXe, function (x) {
                return x.ID === item.ID_LoaiXe;
            });
            if (loaixe.length > 0) {
                self.typeCarName = loaixe[0].TenLoaiXe;
                self.newCar.ID_LoaiXe = loaixe[0].ID;
            }
            if (event !== undefined) {
                $(event.currentTarget).closest('div').hide();
            }
        },
        changeManufacturerParent: function (item) {
            let self = this;
            self.manufacturerName = item.TenHangXe;
            self.newCar.ID_HangXe = item.ID;
            $(event.currentTarget).closest('div').hide();
        },
        changeTypeCarParent: function (item) {
            let self = this;
            self.typeCarName = item.TenLoaiXe;
            self.newCar.ID_LoaiXe = item.ID;
            $(event.currentTarget).closest('div').hide();
        },
        ChangeCustomer: function (item) {
            var self = this;
            self.newCar.ID_KhachHang = item.ID;
            self.customerChosing = item;

            var elm = $(event.currentTarget);
            $(elm).closest('div').hide();
            $(elm).closest('div').prev('focus');
        },
        AddNewCar: function () {
            var self = this;
            var myData = self.newCar;

            var noidungct = myData.BienSo.concat("<br /> <b> Thông tin chi tiết: </b>",
                '<br /> - Mẫu xe: ', self.modelCarName,
                '<br /> - Số khung: ', myData.SoKhung,
                '<br /> - Số máy: ', myData.SoMay,
                '<br /> - Chủ xe: ', self.customerChosing.TenDoiTuong,
                '<br /> - Xe là hàng hóa: ', self.LaHangHoa ,
            );

            if (self.isNew) {
                myData.ID = self.Guid_Empty;
                myData.NguoiTao = self.inforLogin.UserLogin;
                ajaxHelper(self.GaraAPI + 'Post_DanhMucXe', 'post', myData).done(function (x) {
                    if (x.res) {
                        self.saveOK = true;
                        self.newCar.ID = x.dataSoure.ID;

                        commonStatisJs.ShowMessageSuccess("Thêm mới xe thành công");
                        let diary = {
                            ID_DonVi: self.inforLogin.ID_DonVi,
                            ID_NhanVien: self.inforLogin.ID_NhanVien,
                            ChucNang: "Danh mục xe",
                            NoiDung: "Thêm mới xe ".concat(myData.BienSo),
                            NoiDungChiTiet: "Thêm mới xe ".concat(noidungct),
                            LoaiNhatKy: 1
                        }
                        Insert_NhatKyThaoTac_1Param(diary);

                        if (self.LaHangHoa) {
                            //dialogConfirm('Thông báo', 'Bạn có muốn thêm mới hàng hóa với cùng biển số xe <b> ' + self.newCar.Biéno + '</b> không?', function () {
                            //self.AddXe_toHangHoa();
                            //})
                        }
                        $('#ThemMoiXemModal').modal('hide');
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(x.mess);
                    }
                }).always(function () {
                    self.isLoading = false;
                });
            }
            else {
                myData.NguoiSua = self.inforLogin.UserLogin;
                ajaxHelper(self.GaraAPI + 'Put_DanhMucXe', 'post', myData).done(function (x) {
                    if (x.res) {
                        self.saveOK = true;
                        commonStatisJs.ShowMessageSuccess("Cập nhật xe thành công");
                        let diary = {
                            ID_DonVi: self.inforLogin.ID_DonVi,
                            ID_NhanVien: self.inforLogin.ID_NhanVien,
                            ChucNang: "Danh mục xe",
                            NoiDung: "Cập nhật xe ".concat(myData.BienSo),
                            NoiDungChiTiet: "Cập nhật xe ".concat(noidungct,
                                '<br /> <b> Thông tin cũ: </b>',
                                '<br /> - Biển số: ', self.OldCar.BienSo,
                                '<br /> - Mẫu xe: ', self.OldCar.TenMauXe,
                                '<br /> - Số khung: ', self.OldCar.SoKhung,
                                '<br /> - Số máy: ', self.OldCar.SoMay,
                                '<br /> - Chủ xe: ', self.OldCar.TenDoiTuong, ' (', self.OldCar.MaDoiTuong, ')',
                                '<br /> - Xe là hàng hóa: ', !commonStatisJs.CheckNull(self.OldCar.ID_HangHoa)
                            ),
                            LoaiNhatKy: 2
                        }
                        Insert_NhatKyThaoTac_1Param(diary);

                        if (self.LaHangHoa) {
                            //self.AddXe_toHangHoa();
                        }
                        else {
                            if (!commonStatisJs.CheckNull(self.OldCar.ID_HangHoa)) {
                                //self.DMHangHoa_ResetIDXe();
                            }
                        }
                        $('#ThemMoiXemModal').modal('hide');
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(x.mess);
                    }
                }).always(function () {
                    self.isLoading = false;
                });
            }
            self.Update_MauXe();
        },
        AddXe_toHangHoa: function () {
            let self = this;
            let objHangHoa = {
                MaHangHoa: self.newCar.BienSo,
                TenHangHoa: self.newCar.BienSo,
                TenHangHoa_KhongDau: locdau(self.newCar.BienSo),
                TenHangHoa_KyTuDau: GetChartStart(self.newCar.BienSo),
                GiaBan: 0,
                GiaVon: 0,
                TonKho: 0,
                ID_NhomHang: null,
                ID_NhanVien: self.inforLogin.ID_NhanVien,
                ID_DonVi: self.inforLogin.ID_DonVi,
                NguoiTao: self.inforLogin.UserLogin,
                DuocBanTrucTiep: true,
                QuanLyTheoLoHang: false,
                LaHangHoa: true,
                LoaiHangHoa: 1,
                TenDonViTinh: '',
                ID_Xe: self.newCar.ID,
            };

            ajaxHelper('/api/DanhMuc/DM_HangHoaAPI/' + 'PostDM_HangHoaDV', 'POST', { objNew: objHangHoa })
                .done(function (x) {
                    console.log('addHangHoa ', x);
                })
        },
        DMHangHoa_ResetIDXe: function (idHangHoa) {
            if (!commonStatisJs.CheckNull(idHangHoa)) {
                ajaxHelper('/api/DanhMuc/DM_HangHoaAPI/' + 'DMHangHoa_UpdateIDXe?idHangHoa=' + idHangHoa, 'GET')
                    .done(function (x) {
                    })
            }
        },

        showModalHangXe: function () {
            vmHangXe.inforLogin = this.inforLogin;
            vmHangXe.ShowModalAddNew();
        },
        showModalLoaiXe: function () {
            vmLoaiXe.inforLogin = this.inforLogin;
            vmLoaiXe.ShowModalAddNew();
        },
        showModalMauXe: function () {
            var self = this;
            vmMauXe.listData.HangXe = self.listData.HangXe;
            vmMauXe.listData.LoaiXe = self.listData.LoaiXe;
            vmMauXe.inforLogin = self.inforLogin;
            vmMauXe.ShowModalAddNew();
        },
        CheckSave: function () {
            let self = this;
            //if (commonStatisJs.CheckNull(self.newCar.ID_MauXe)) {
            //    commonStatisJs.ShowMessageDanger('Vui lòng chọn mẫu xe');
            //    return;
            //}
            if (commonStatisJs.CheckNull(self.newCar.BienSo)) {
                commonStatisJs.ShowMessageDanger('Vui lòng nhập biển số xe');
                return;
            }
            self.isLoading = true;

            let datacheck = {};
            datacheck.BienSo = self.newCar.BienSo;
            datacheck.New = self.isNew;
            datacheck.IDXe = self.newCar.ID;
            $.ajax({
                url: self.GaraAPI + "CheckSaveDanhMucXe",
                type: 'POST',
                dataType: 'json',
                data: datacheck,
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    if (data.res === true) {
                        if (data.dataSoure.BienSo) {
                            commonStatisJs.ShowMessageDanger("Biển số đã tồn tại.");
                            self.isLoading = false;
                            return true;
                        }
                        else {
                            self.AddNewCar();
                        }
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(data.mess);
                        self.isLoading = false;
                        return true;
                    }
                }
            });
        },
        showModalCustomer: function () {
            var self = this;
            vmThemMoiKhach.inforLogin = self.inforLogin;
            vmThemMoiKhach.showModalAdd();
        },
        ResetCustomer: function () {
            var self = this;
            self.newCar.ID_KhachHang = null;
            self.customerChosing = {
                ID: null,
                TenDoiTuong: '',
                DienThoai: '',
            }
        },
        UpdateCustomer: function () {
            var self = this;
            vmThemMoiKhach.inforLogin = self.inforLogin;
            vmThemMoiKhach.GetInforKhachHangFromDB_ByID(self.newCar.ID_KhachHang, true);
        },
        showModal_UpdateMauXe: function () {
            var self = this;
            let objModel = {
                ID: self.newCar.ID_MauXe,
                ID_HangXe: self.newCar.ID_HangXe,
                ID_LoaiXe: self.newCar.ID_LoaiXe,
                TenMauXe: self.modelCarName,
                TenHangXe: self.manufacturerName,
                TenLoaiXe: self.typeCarName,
                GhiChu: '',
                TrangThai: 1,
            }
            vmMauXe.ShowModalUpdate(objModel)
        },
        Update_MauXe: function () {
            let self = this;
            if (self.newCar.ID_MauXe !== self.Guid_Empty && !commonStatisJs.CheckNull(self.newCar.ID_MauXe)) {
                // get ghichu old of mauxe
                let mauxeOld = $.grep(self.listData.MauXe, function (x) {
                    return x.ID === self.newCar.ID_MauXe;
                });
                let ghichuMX = '', idHangOld = self.Guid_Empty, idLoaiOld = self.Guid_Empty;
                if (mauxeOld.length > 0) {
                    ghichuMX = mauxeOld[0].GhiChu;
                    idHangOld = mauxeOld[0].ID_HangXe;
                    idLoaiOld = mauxeOld[0].ID_LoaiXe;
                }

                let myData = {
                    ID_HangXe: self.newCar.ID_HangXe,
                    ID_LoaiXe: self.newCar.ID_LoaiXe,
                    TenMauXe: self.modelCarName,
                    GhiChu: ghichuMX,
                    TrangThai: 1,
                };
                if (idHangOld !== self.newCar.ID_HangXe || idLoaiOld !== self.newCar.ID_LoaiXe) {
                    myData.ID = self.newCar.ID_MauXe;
                    myData.NguoiSua = self.inforLogin.UserLogin;

                    ajaxHelper(self.GaraAPI + 'Put_MauXe', 'POST', myData).done(function (x) {
                        if (x.res) {
                            let diary = {
                                ID_DonVi: self.inforLogin.ID_DonVi,
                                ID_NhanVien: self.inforLogin.ID_NhanVien,
                                ChucNang: "Cập nhật xe - Thay đổi thông tin mẫu xe",
                                NoiDung: "Cập nhật mẫu xe ".concat(self.modelCarName),
                                NoiDungChiTiet: "Cập nhật mẫu xe ".concat(self.modelCarName, '<br /> <b> Thông tin cũ: </b>',
                                    '<br /> - Hãng xe: ', self.OldCar.TenHangXe,
                                    '<br /> - Loại xe: ', self.OldCar.TenLoaiXe,
                                    '<br /> <b> Thông tin mới: </b>',
                                    '<br /> - Hãng xe: ', self.manufacturerName,
                                    '<br /> - Loại xe: ', self.typeCarName,
                                ),
                                LoaiNhatKy: 2
                            }
                            Insert_NhatKyThaoTac_1Param(diary);
                        }
                        else {
                            commonStatisJs.ShowMessageDanger(x.mes);
                        }
                    })
                }
            }
        },
    }
})

$(function () {
    $('#ThemMoiHangXe').on('hidden.bs.modal', function () {
        if (vmHangXe.saveOK) {
            var newObj = {
                ID: vmHangXe.ID,
                MaHangXe: vmHangXe.MaHangXe,
                TenHangXe: vmHangXe.TenHangXe,
            }
            vmThemMoiXe.manufacturerName = newObj.TenHangXe;
            vmThemMoiXe.newCar.ID_HangXe = newObj.ID;
            if (!vmHangXe.isNew && newObj.ID !== null && newObj.ID !== Guid_Empty) {
                for (let i = 0; i < vmThemMoiXe.listData.HangXe.length; i++) {
                    if (vmThemMoiXe.listData.HangXe[i].ID === vmHangXe.ID) {
                        vmThemMoiXe.listData.HangXe.splice(i, 1);
                        break;
                    }
                }
                vmThemMoiXe.listData.HangXe.unshift(newObj);
            }
        }
    });
    $('#ThemMoiLoaiXe').on('hidden.bs.modal', function () {
        if (vmLoaiXe.saveOK) {
            var newObj = {
                ID: vmLoaiXe.ID,
                MaLoaiXe: vmLoaiXe.MaLoaiXe,
                TenLoaiXe: vmLoaiXe.TenLoaiXe,
            }
            vmThemMoiXe.typeCarName = vmLoaiXe.TenLoaiXe;
            vmThemMoiXe.newCar.ID_LoaiXe = vmLoaiXe.ID;
            if (!vmLoaiXe.isNew && newObj.ID !== null && newObj.ID !== Guid_Empty) {
                for (let i = 0; i < vmThemMoiXe.listData.LoaiXe.length; i++) {
                    if (vmThemMoiXe.listData.LoaiXe[i].ID === vmLoaiXe.ID) {
                        vmThemMoiXe.listData.LoaiXe.splice(i, 1);
                        break;
                    }
                }
            }
            vmThemMoiXe.listData.LoaiXe.unshift(newObj);
        }
    });
    $('#ThemMoiMauXe').on('hidden.bs.modal', function () {
        if (vmMauXe.saveOK) {
            var newObj = {
                ID: vmMauXe.ID,
                ID_HangXe: vmMauXe.ID_HangXe,
                ID_LoaiXe: vmMauXe.ID_LoaiXe,
                TenMauXe: vmMauXe.TenMauXe,
            }
            vmThemMoiXe.modelCarName = vmMauXe.TenMauXe;
            if (!vmMauXe.isNew) {
                for (let i = 0; i < vmThemMoiXe.listData.MauXe.length; i++) {
                    if (vmThemMoiXe.listData.MauXe[i].ID === vmMauXe.ID) {
                        vmThemMoiXe.listData.MauXe.splice(i, 1);
                        break;
                    }
                }
            }
            vmThemMoiXe.changeModelCarParent(newObj);
        }
    });
    $('#ThemMoiKhachHang').on('hidden.bs.modal', function () {
        // check if isShowing modal themmoixe
        if ($('#ThemMoiXemModal').hasClass('in')) {
            if (vmThemMoiKhach.saveOK) {
                vmThemMoiXe.customerChosing.TenDoiTuong = vmThemMoiKhach.customerDoing.TenDoiTuong;
                vmThemMoiXe.customerChosing.DienThoai = vmThemMoiKhach.customerDoing.DienThoai;
                vmThemMoiXe.newCar.ID_KhachHang = vmThemMoiKhach.customerDoing.ID;
            }
        }
    })
})

