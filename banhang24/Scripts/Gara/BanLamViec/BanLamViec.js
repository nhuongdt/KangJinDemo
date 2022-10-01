var workTable = new Vue({
    el: '#BanLamViec',
    components: {
        'car-model': ComponentChoseModelCar,
        'cmp-print': cmpPrint,
    },
    created: function () {
        var self = this;
        self.textSearch = '';
        self.ID_DonVi = $('#hd_IDdDonVi').val();
        self.TenDonVi = $('#_txtTenDonVi').text();
        self.GaraAPI = "/api/DanhMuc/GaraAPI/";
        self.HoaDonAPI = "/api/DanhMuc/BH_HoaDonAPI/";
        self.ID_PhieuTiepNhan = '%%';
        self.GuidEmpty = '00000000-0000-0000-0000-000000000000';
        self.roleHeader = false;
        self.isFirstLoad = true;
        self.role = {
            PhieuTiepNhan: { ThemMoi: true, Xem: true },
            HangXe: {},
            LoaiXe: {},
            MauXe: {},
            Xe: {
                XuatXuong: true
            },
            BaoGia: { ThemMoi: true, Duyet: true, XuLy: true, CapNhat: true, In: true },
            HoaDon: { ThemMoi: true, SuaDoi: true, In: true },
            XuatKho: {},
        };

        self.PageLoad();

        console.log('blv');
        self.MauIn = {
            PhieuTiepNhan: [],
            HoaDon: [],
            BaoGia: [],
            ListData: {
                ChiTietMua: [],
                ChiTietDoiTra: [],
                HoaDon: [],
                HangMucSuaChua: [],
                VatDungKemTheo: [],
            }
        }

        // get role at header
        if (VHeader) {
            self.roleHeader = true;
            self.role.PhieuTiepNhan.ThemMoi = self.CheckRole('PhieuTiepNhan_ThemMoi');
        }
        self.CheckRole_inPage();

        // mau in dathang
        $.getJSON("/api/DanhMuc/ThietLapApi/GetListMauIn?typeChungTu=DH&idDonVi=" + self.ID_DonVi, function (data) {
            self.MauIn.BaoGia = data;
        });

        $.getJSON("/api/DanhMuc/ThietLapApi/GetListMauIn?typeChungTu=HDBL&idDonVi=" + self.ID_DonVi, function (data) {
            self.MauIn.HoaDon = data;
        });

        $.getJSON("/api/DanhMuc/ThietLapApi/GetListMauIn?typeChungTu=PTN&idDonVi=" + self.ID_DonVi, function (data) {
            self.MauIn.PhieuTiepNhan = data;
            vmXuatXuong.MauIn.ListMauIn = data;
        });

        $.getJSON("/api/DanhMuc/NS_NhanVienAPI/" + 'GetUserCookies', function (nv_nd) {
            self.ID_NhanVien = nv_nd.ID_NhanVien;
            self.ID_User = nv_nd.ID;
            self.UserLogin = nv_nd.TaiKhoan;
            self.ID_DonVi = nv_nd.ID_DonVi;

            vmThemMoiNhomKhach.inforLogin = {
                ID_NhanVien: nv_nd.ID_NhanVien,
                ID_User: nv_nd.ID,
                UserLogin: nv_nd.TaiKhoan,
                ID_DonVi: nv_nd.ID_DonVi,
            };

            ajaxHelper('/api/DanhMuc/DM_DonViAPI/' + 'GetListDonVi_User?ID_NguoiDung=' + self.ID_User, 'GET').done(function (data) {
                if (data !== null && data.length > 0) {
                    vmThemMoiNhomKhach.listData.ChiNhanhs = data;
                    vmThanhToan.listData.ChiNhanhs = data;
                }
            });
        });

        if (VHeader.Quyen.length === 0) {
            ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetListQuyen_OfNguoiDung", 'GET').done(function (data) {
                if (data !== "" && data.length > 0) {
                    self.listData.Quyen_NguoiDung = data;

                    self.role.PhieuTiepNhan.ThemMoi = self.CheckRole('PhieuTiepNhan_ThemMoi');
                    self.role.PhieuTiepNhan.Xem = self.CheckRole('PhieuTiepNhan_XemDS');
                    self.role.PhieuTiepNhan.CapNhat = self.CheckRole('PhieuTiepNhan_CapNhat');
                    self.role.PhieuTiepNhan.Xoa = self.CheckRole('PhieuTiepNhan_Xoa');
                    self.role.PhieuTiepNhan.In = self.CheckRole('PhieuTiepNhan_In');
                    self.role.BaoGia.ThemMoi = self.CheckRole('DatHang_ThemMoi');
                    self.role.HoaDon.ThemMoi = self.CheckRole('HoaDon_ThemMoi');
                }
            });
        }

        ajaxHelper("/api/DanhMuc/NS_NhanVienAPI/" + "GetNS_NhanVien_InforBasic?idDonVi=" + self.ID_DonVi, 'GET').done(function (data) {
            self.listData.NhanViens = data;
            vmThemMoiKhach.listData.NhanViens = data;
            vmThanhToan.listData.NhanViens = data;
            vmHoaHongHoaDon.listData.NhanViens = data;
        });



        // account bank
        ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/' + 'GetAllTaiKhoanNganHang_ByDonVi?idDonVi=' + self.ID_DonVi, 'GET').done(function (x) {
            if (x.res === true) {
                self.listData.AccountBanks = x.data;
            }
        })

        // khoan thuchi
        $.getJSON('/api/DanhMuc/Quy_HoaDonAPI/' + 'GetQuy_KhoanThuChi').done(function (x) {
            if (x.res === true) {
                self.listData.KhoanThuChis = x.data;
            }
        })

        $.getJSON('/api/DanhMuc/HT_API/' + 'GetHT_CongTy').done(function (data) {
            if (data != null) {
                self.listData.CongTy = data;
            }
        });

        // trangthaikhach
        $.getJSON('/api/DanhMuc/ChamSocKhachHangAPI/' + 'GetTrangThaiTimKiem').done(function (data) {
            if (data.res === true) {
                vmThemMoiKhach.listData.TrangThaiKhachs = data.dataSoure.ttkhachhang;
            }
        });

        // nguonkhach
        $.getJSON('/api/DanhMuc/DM_NguonKhachAPI/' + 'GetDM_NguonKhach').done(function (data) {
            vmThemMoiKhach.listData.NguonKhachs = data;
        });

        // nhomkhach
        ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/' + "GetCauHinhHeThong/" + self.ID_DonVi).done(function (data) {
            if (data !== null) {
                self.listData.ThietLap = data;

                $.getJSON('/api/DanhMuc/DM_DoiTuongAPI/' + 'GetNhomDoiTuong_DonVi?loaiDT=1').done(function (obj) {
                    if (obj.res === true) {
                        let data = obj.data;
                        for (var i = 0; i < data.length; i++) {
                            let tenNhom = data[i].TenNhomDoiTuong;
                            tenNhom = tenNhom.concat(' ', locdau(tenNhom), ' ', GetChartStart(tenNhom));
                            data[i].Text_Search = tenNhom;
                        }
                        if (data.QuanLyKhachHangTheoDonVi) {
                            // only get Nhom chua cai dat ChiNhanh or in this ChiNhanh
                            var arrNhom = [];
                            for (var i = 0; i < data.length; i++) {
                                if (data[i].NhomDT_DonVi.length > 0) {
                                    let ex = $.grep(data[i].NhomDT_DonVi, function (x) {
                                        return x.ID === idDonVi;
                                    });
                                    if (ex.length) {
                                        arrNhom.push(data[i]);
                                    }
                                }
                                else {
                                    arrNhom.push(data[i]);
                                }
                            }
                            vmThemMoiKhach.listData.NhomKhachs = arrNhom;
                        }
                        else {
                            vmThemMoiKhach.listData.NhomKhachs = data;
                        }
                    }
                });
            }
        });

        // tinhthanh, quanhuyen
        $.getJSON('/api/DanhMuc/BaseApi/' + "GetAllTinhThanh").done(function (x) {
            var data = x.map(p => ({
                ID: p.Key,
                val2: p.Value
            }));
            vmThemMoiKhach.listData.TinhThanhs = data;
            vmThemMoiKhach.listData.ListTinhThanhSearch = data;
        });

        $.getJSON('/api/DanhMuc/DM_DoiTuongAPI/' + 'getList_VungMien').done(function (data) {
            vmThemMoiNhomKhach.listData.VungMiens = data;
        });
    },
    data: {
        textSearch: '',
        manufacturerNameParent: '--Chọn hãng xe--',
        Paging_PhieuSuaChua: {
            CurrentPage: 1,
            PageSize: 10,
            ListPage: [],
            PageView: '',
            NumberOfPage: 0,
        },
        Paging_BaoGia: {
            CurrentPage: 0,
            PageSize: 20,
        },
        Paging_HoaDon: {
            CurrentPage: 0,
            PageSize: 20,
        },
        listData: {
            NhanViens: [],
            AccountBanks: [],
            KhoanThuChis: [],
            Quyen_NguoiDung: [],
            PhieuSuaChuas: [],
            BaoGias: [],
            HoaDons: [],
            ThongTinXe: {},
            HangMucSuaChuas: [],
            VatDungKemTheos: [],
            CongTy: [],
            ThietLap: [],
            HoaDonChiTiets: [],
        },
        itemChosing: {},
        inforCus: {},
        inforInsurance: {},
        filterTrangThai: [1, 2],
        wasChotSo: false,

        arrID_HDPrint: [],
        loaiHoaDonPrint: 25,

        ListHeader: [],
        ListHeaderAll: [{ colName: 'STT', colText: 'STT', colShow: true, index: 1, Css: '' },
        { colName: 'BienSoXe', colText: 'Biển số xe', colShow: true, index: 2, Css: '' },
        { colName: 'MaPhieuTiepNhan', colText: 'Phiếu tiếp nhận', colShow: true, index: 3, Css: '' },
        { colName: 'SoKMVao', colText: 'Số Km vào', colShow: false, index: 4, Css: '' },
        { colName: 'NgayTiepNhan', colText: 'Ngày tiếp nhận', colShow: true, index: 5, Css: '' },
        { colName: 'MauHangXe', colText: 'Mẫu - Hãng xe', colShow: true, index: 6, Css: '' },
        { colName: 'TenKhachHang', colText: 'Khách hàng', colShow: true, index: 7, Css: '' },
        { colName: 'SDTKhachHang', colText: 'SĐT', colShow: true, index: 8, Css: '' },
        { colName: 'TenCongTyBaoHiem', colText: 'Cty bảo hiểm', colShow: false, index: 9, Css: '' },
        ],
    },
    watch: {
        ListHeader: {
            handler: function () {
                let header = localStorage.getItem('lstHeader');
                if (header !== null) {
                    header = JSON.parse(header);
                    for (let i = 0; i < header.length; i++) {
                        if (header[i].Key === 'BLV_PhieuTiepNhan') {
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
        isCheckPrint: function (item) {
            let self = this;
            return $.inArray(item.ID, self.arrID_HDPrint) > -1;
        },
        PageLoad: function () {
            let self = this;
            self.InitListHeader();
        },
        InitListHeader: function () {
            let self = this;
            let exist = false;
            let header = localStorage.getItem('lstHeader');
            if (header !== null) {
                header = JSON.parse(header);
                for (let i = 0; i < header.length; i++) {
                    if (header[i].Key === 'BLV_PhieuTiepNhan') {
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
                    Key: 'BLV_PhieuTiepNhan',
                    List: self.ListHeaderAll,
                };
                header.push(obj);
                localStorage.setItem('lstHeader', JSON.stringify(header));
            }
            else {
                if (self.ListHeader.length !== self.ListHeaderAll.length) {
                    // id exist cache && column is add new --> assign again cache
                    for (let i = 0; i < header.length; i++) {
                        if (header[i].Key === 'BLV_PhieuTiepNhan') {
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
            let data = self.ListHeader.find(x => x.colName === colName);
            if (data !== undefined) {
                return data.colShow;
            }
            return false;
        },
        CheckRole_inPage: function () {
            var self = this;
            self.role.PhieuTiepNhan.Xem = self.CheckRole('PhieuTiepNhan_XemDS');
            self.role.PhieuTiepNhan.CapNhat = self.CheckRole('PhieuTiepNhan_CapNhat');
            self.role.PhieuTiepNhan.Xoa = self.CheckRole('PhieuTiepNhan_Xoa');
            self.role.PhieuTiepNhan.In = self.CheckRole('PhieuTiepNhan_In');

            vmTiepNhanXe.role.Xe.ThemMoi = self.CheckRole('DanhMucXe_ThemMoi');
            vmTiepNhanXe.role.Xe.CapNhat = self.CheckRole('DanhMucXe_CapNhat');
            vmTiepNhanXe.role.KhachHang.ThemMoi = self.CheckRole('KhachHang_ThemMoi');
            vmTiepNhanXe.role.KhachHang.CapNhat = self.CheckRole('KhachHang_CapNhat');
            vmTiepNhanXe.role.BaoHiem.ThemMoi = self.CheckRole('BaoHiem_ThemMoi');
            vmTiepNhanXe.role.BaoHiem.CapNhat = self.CheckRole('BaoHiem_CapNhat');
            vmTiepNhanXe.role.PhieuTiepNhan = self.role.PhieuTiepNhan;

            self.role.BaoGia.ThemMoi = self.CheckRole('DatHang_ThemMoi');
            self.role.BaoGia.CapNhat = self.CheckRole('DatHang_CapNhat');
            self.role.BaoGia.In = self.CheckRole('DatHang_In');
            self.role.BaoGia.Xoa = self.CheckRole('DatHang_Xoa');
            self.role.BaoGia.XuLy = self.CheckRole('DatHang_TaoHoaDon');
            self.role.BaoGia.Duyet = self.CheckRole('DatHang_DuyetBaoGia');

            self.role.HoaDon.ThemMoi = self.CheckRole('HoaDon_ThemMoi');
            self.role.HoaDon.SuaDoi = self.CheckRole('HoaDon_SuaDoi');
            self.role.HoaDon.In = self.CheckRole('HoaDon_In');
            self.role.HoaDon.Xoa = self.CheckRole('HoaDon_Xoa');
            self.role.HoaDon.XuLy = self.CheckRole('HoaDon_TaoHoaDon');
            self.role.HoaDon.ThanhToan = self.CheckRole('KhachHang_ThanhToanNo');
            self.role.XuatKho.ThemMoi = self.CheckRole('XuatHuy_ThemMoi');
            self.role.HoaDon.NhapHang = self.CheckRole('NhapHang_ThemMoi');
            self.role.HoaDon.SuaChiPhiDV = self.CheckRole('HoaDon_SuaChiPhiDichVu');

            vmThemMoiKhach.role.KhachHang.ThemMoi = vmTiepNhanXe.role.KhachHang.ThemMoi;
            vmThemMoiKhach.role.KhachHang.CapNhat = self.CheckRole('KhachHang_CapNhat');
        },
        filterTrangThaiClick: function (options) {

            let self = this;
            self.Paging_PhieuSuaChua.CurrentPage = 1;
            switch (options) {
                case 0:
                    self.filterTrangThai = [1, 2];
                    $("#TrangThaiXe > span").html("Toàn bộ xe");
                    $("#TrangThaiXe").css("background", "#555555");
                    break;
                case 1:
                    self.filterTrangThai = [1];
                    $("#TrangThaiXe").css("background", "#75BF72");
                    $("#TrangThaiXe > span").html("Xe đang sửa");
                    break;
                case 2:
                    self.filterTrangThai = [2];
                    $("#TrangThaiXe").css("background", "#FF7208");
                    $("#TrangThaiXe > span").html("Xe sửa xong");
                    break;
                default: self.filterTrangThai = [1, 2];
                    break;

            }
            self.Gara_GetListPhieuTiepNhan(true);
        },
        CheckRole: function (maquyen) {
            return VHeader.Quyen.indexOf(maquyen) > -1;
        },
        ResetCurrentPage: function () {
            var self = this;
            self.Paging_BaoGia = {
                CurrentPage: 0,
                PageSize: 20,
            };
            self.Paging_HoaDon = {
                CurrentPage: 0,
                PageSize: 20,
            };
        },
        PhieuTiepNhan_ChangePage: function (value) {
            var self = this;
            if (self.Paging_PhieuSuaChua.CurrentPage !== value.currentPage) {
                self.Paging_PhieuSuaChua.CurrentPage = value.currentPage;
                self.Gara_GetListPhieuTiepNhan();
            } else if (this.PageSize !== value.pageSize) {
                self.Paging_PhieuSuaChua.PageSize = value.pageSize;
                self.Gara_GetListPhieuTiepNhan();
            }
        },
        Gara_GetListPhieuTiepNhan: function (firstLoad = false) {
            var self = this;
            var paramPhieuSC = {}
            paramPhieuSC.IdChiNhanhs = [self.ID_DonVi];
            paramPhieuSC.CurrentPage = self.Paging_PhieuSuaChua.CurrentPage;
            paramPhieuSC.TrangThais = self.filterTrangThai;
            paramPhieuSC.TextSearch = self.textSearch;
            paramPhieuSC.PageSize = self.Paging_PhieuSuaChua.PageSize;

            ajaxHelper(self.GaraAPI + "GetListPhieuTiepNhan_v2", 'POST', paramPhieuSC).done(function (x) {
                if (x.res) {
                    self.listData.PhieuSuaChuas = x.dataSoure.data;
                    self.Paging_PhieuSuaChua.ListPage = x.dataSoure.ListPage;
                    self.Paging_PhieuSuaChua.NumberOfPage = x.dataSoure.NumberOfPage;
                    self.Paging_PhieuSuaChua.PageView = x.dataSoure.PageView;
                    if (x.dataSoure.data.length > 0) {
                        // default idphieutienhan --> focus first row
                        self.ID_PhieuTiepNhan = x.dataSoure.data[0].ID;
                        self.listData.ThongTinXe = x.dataSoure.data[0];
                        if (firstLoad) {
                            self.Gara_GetListBaoGia();
                            self.Gara_GetListHoaDonSuaChua();
                        }
                    }
                }
            });
        },
        Gara_GetListBaoGia: function (closeWindow = false) {
            var self = this;
            var paramBG = {
                LstIDChiNhanh: [self.ID_DonVi],
                TextSearch: '%%',
                ID_HangXe: self.ID_PhieuTiepNhan,// muon tam truong
                CurrentPage: self.Paging_BaoGia.CurrentPage,
                PageSize: self.Paging_BaoGia.PageSize,
                FromDate: '2016-01-01',
                ToDate: moment(new Date()).add(1, 'days').format('YYYY-MM-DD'),
                TrangThai: '0,1',
            }
            ajaxHelper(self.GaraAPI + "Gara_GetListBaoGia", 'POST', paramBG).done(function (x) {
                if (x.res) {
                    self.listData.BaoGias = x.dataSoure;

                    if (closeWindow && !$.isEmptyObject(workTable.itemChosing)) {
                        let hdChosing = $.grep(x.dataSoure, function (x) {
                            return x.ID === self.itemChosing.ID;
                        });
                        if (hdChosing.length > 0) {
                            self.ChoseItem_inList(hdChosing[0], 3);
                        }
                    }
                }
            });
        },
        Gara_GetListHoaDonSuaChua: function (closeWindow = false) {
            var self = this;
            var paramHD = {
                LstIDChiNhanh: [self.ID_DonVi],
                TextSearch: '%%',
                ID_HangXe: self.ID_PhieuTiepNhan,// muon tam truong
                CurrentPage: self.Paging_HoaDon.CurrentPage,
                PageSize: self.Paging_HoaDon.PageSize,
                FromDate: '2016-01-01',
                ToDate: moment(new Date()).add(1, 'days').format('YYYY-MM-DD'),
                TrangThai: '0,1',
            }
            ajaxHelper(self.GaraAPI + "Gara_GetListHoaDonSuaChua", 'POST', paramHD).done(function (x) {
                if (x.res) {
                    self.listData.HoaDons = x.dataSoure;
                    if (closeWindow && !$.isEmptyObject(workTable.itemChosing)) {
                        let hdChosing = $.grep(x.dataSoure, function (x) {
                            return x.ID === self.itemChosing.ID;
                        });
                        if (hdChosing.length > 0) {
                            self.ChoseItem_inList(hdChosing[0], 25);
                        }
                    }
                }
            });
        },
        PhieuTiepNhan_GetThongTinChiTiet: function (ct) {
            var self = this;
            self.listData.ThongTinXe = ct;
        },
        PhieuTiepNhan_GetTinhTrangXe: function (isUpdate = false) {
            var self = this;
            $.getJSON(self.GaraAPI + "PhieuTiepNhan_GetTinhTrangXe?id=" + self.ID_PhieuTiepNhan).done(function (x) {
                if (x.res) {
                    self.listData.HangMucSuaChuas = x.dataSoure.hangmuc;
                    self.listData.VatDungKemTheos = x.dataSoure.vatdung;

                    self.MauIn.ListData.HangMucSuaChua = self.listData.HangMucSuaChuas.map(function (item, index) {
                        return {
                            STT: index + 1,
                            TenHangMuc: item.TenHangMuc,
                            TinhTrang: item.TinhTrang,
                            PhuongAnSuaChua: item.PhuongAnSuaChua,
                        }
                    });
                    self.MauIn.ListData.VatDungKemTheo = self.listData.VatDungKemTheos.map(function (item, index) {
                        return {
                            STT: index + 1,
                            TieuDe: item.TieuDe,
                            SoLuong: item.SoLuong,
                        }
                    });

                    if (isUpdate) {
                        vmTiepNhanXe.UpdatePhieuTiepNhan(self.listData.ThongTinXe, self.listData.HangMucSuaChuas, self.listData.VatDungKemTheos);
                    }
                }
            });
        },
        showModalNewCar: function () {
            vmThemMoiXe.inforLogin = {
                ID_NhanVien: self.ID_NhanVien,
                ID_User: self.ID_User,
                UserLogin: self.UserLogin,
                ID_DonVi: self.ID_DonVi,
            };
            vmThemMoiXe.ShowModalNewCar();
        },
        ThemMoiPhieuTiepNhan: function () {
            var self = this;
            self.InitData_TiepNhanXe();
            vmTiepNhanXe.showModalTiepNhanXe();
        },
        UpdatePhieuTiepNhan: function (item) {
            var self = this;
            self.ID_PhieuTiepNhan = item.ID;
            self.InitData_TiepNhanXe();
            self.PhieuTiepNhan_GetTinhTrangXe(true);
        },

        getTenNhanVien_Login: function () {
            var self = this;
            var tenNV = '';
            let nvien = $.grep(self.listData.NhanViens, function (x) {
                return x.ID === self.ID_NhanVien;
            });
            if (nvien.length > 0) {
                tenNV = nvien[0].TenNhanVien;
            }
            return tenNV;
        },
        InitData_TiepNhanXe: function () {
            var self = this;
            vmTiepNhanXe.listData.NhanViens = self.listData.NhanViens;
            // set defaul nvlapphieu
            vmTiepNhanXe.inforLogin = {
                ID_NhanVien: self.ID_NhanVien,
                ID_User: self.ID_User,
                UserLogin: self.UserLogin,
                ID_DonVi: self.ID_DonVi,
                TenNhanVien: self.getTenNhanVien_Login(),
            };
        },
        hideModalTiepNhanXe: function (thongtin) {
            this.ID_PhieuTiepNhan = thongtin.ID;
            this.listData.PhieuSuaChuas.unshift(thongtin);
        },
        Get_SDTCoVan: function () {
            var self = this;
            // nvien covan
            let covan_SDT = '';
            let covan = $.grep(self.listData.NhanViens, function (y) {
                return y.ID === self.listData.ThongTinXe.ID_CoVanDichVu;
            });
            if (covan.length > 0) {
                covan_SDT = covan[0].SoDienThoai;
            }
            return covan_SDT;
        },
        GetInfor_PhieuTiepNhan: function (item) {
            var self = this;
            self.isFirstLoad = false;
            self.ResetCurrentPage();
            self.ID_PhieuTiepNhan = item.ID;
            self.arrID_HDPrint = [];

            self.Gara_GetListBaoGia();
            self.Gara_GetListHoaDonSuaChua();
            item.DiaChiKhachHang = item.DiaChi;
            self.PhieuTiepNhan_GetThongTinChiTiet(item);
            self.PhieuTiepNhan_GetTinhTrangXe();
            self.MauIn.ListData.HoaDon = $.extend({}, item);
            self.MauIn.ListData.HoaDon.LH_Ten = item.TenLienHe;
            self.MauIn.ListData.HoaDon.LH_SDT = item.SoDienThoaiLienHe;
            self.MauIn.ListData.HoaDon.PTN_GhiChu = item.GhiChu;

            self.MauIn.ListData.HoaDon.TenBaoHiem = item.TenBaoHiem;
            self.MauIn.ListData.HoaDon.BH_TenLienHe = item.NguoiLienHeBH;
            self.MauIn.ListData.HoaDon.BH_SDTLienHe = item.SoDienThoaiLienHeBH;
            self.MauIn.ListData.HoaDon.BH_DiaChi = '';
            self.MauIn.ListData.HoaDon.BH_Email = '';
            self.MauIn.ListData.HoaDon.CoVan_SDT = self.Get_SDTCoVan();

            if (self.listData.CongTy.length > 0) {
                self.MauIn.ListData.HoaDon.LogoCuaHang = Open24FileManager.hostUrl + self.listData.CongTy[0].DiaChiNganHang;
                self.MauIn.ListData.HoaDon.TenCuaHang = self.listData.CongTy[0].TenCongTy;
                self.MauIn.ListData.HoaDon.DiaChiCuaHang = self.listData.CongTy[0].DiaChi;
                self.MauIn.ListData.HoaDon.DienThoaiCuaHang = self.listData.CongTy[0].SoDienThoai;
            }
            var obj = self.GetInforChiNhanh();
            self.MauIn.ListData.HoaDon.DiaChiChiNhanh = obj.DiaChiChiNhanh;
            self.MauIn.ListData.HoaDon.DienThoaiChiNhanh = obj.DienThoaiChiNhanh;
        },
        GetInforChiNhanh: function () {
            var self = this;
            let diachi = '', dienthoai = '';
            var chinhanh = $.grep(vmThemMoiNhomKhach.listData.ChiNhanhs, function (x) {
                return x.ID === self.ID_DonVi;
            });
            if (chinhanh.length > 0) {
                diachi = chinhanh[0].DiaChi;
                dienthoai = chinhanh[0].SoDienThoai;
            }
            return {
                TenChiNhanh: self.TenDonVi,
                DiaChiChiNhanh: diachi,
                DienThoaiChiNhanh: dienthoai,
            }
        },
        XoaPhieuTiepNhan: function (item) {
            var self = this;
            if (self.listData.HoaDons.length > 0) {
                commonStatisJs.ShowMessageDanger('Phiếu tiếp nhận đã tạo hóa đơn. Không được hủy');
                return;
            }
            commonStatisJs.ConfirmDialog_OKCancel('Xóa phiếu tiếp nhận',
                'Bạn có chắc chắn muốn xóa phiếu tiếp nhận <b>'.concat(item.MaPhieuTiepNhan, ' </b> không ? '), function () {
                    $.getJSON(self.GaraAPI + 'PhieuTiepNhan_UpdateTrangThai?id=' + item.ID + '&status=0').done(function (x) {
                        if (x.res) {
                            commonStatisJs.ShowMessageSuccess("Xóa phiếu thành công");
                            var diary = {
                                ID_DonVi: self.ID_DonVi,
                                ID_NhanVien: self.ID_NhanVien,
                                LoaiNhatKy: 3,
                                ChucNang: 'Bàn làm việc - Phiếu tiếp nhận',
                                NoiDung: 'Xóa phiếu tiếp nhận '.concat(item.MaPhieuTiepNhan),
                                NoiDungChiTiet: 'Xóa phiếu tiếp nhận '.concat(item.MaPhieuTiepNhan, ', Người xóa: ', self.UserLogin)
                            }
                            Insert_NhatKyThaoTac_1Param(diary);

                            // remove from list
                            for (let i = 0; i < self.listData.PhieuSuaChuas.length; i++) {
                                if (self.listData.PhieuSuaChuas[i].ID === item.ID) {
                                    self.listData.PhieuSuaChuas.splice(i, 1);
                                    break;
                                }
                            }
                        }
                        else {
                            commonStatisJs.ShowMessageDanger("Xóa phiếu thất bại");
                        }
                    })
                });
        },
        showModalXuatXuong_Out: function (isOut) {
            var self = this;
            self.showModalXuatXuong(null, true);
        },
        showModalXuatXuong: function (item, isOut = false) {
            var self = this;
            if (!isOut) {
                if (commonStatisJs.CheckNull(self.ID_PhieuTiepNhan)) {
                    commonStatisJs.ShowMessageDanger('Vui lòng chọn xe để xuất xưởng');
                    return;
                }
                self.GetInfor_PhieuTiepNhan(item);
            }

            vmXuatXuong.inforLogin = {
                ID_NhanVien: self.ID_NhanVien,
                ID_User: self.ID_User,
                UserLogin: self.UserLogin,
                ID_DonVi: self.ID_DonVi,
            };

            vmXuatXuong.phieuXuat = {
                ID: self.ID_PhieuTiepNhan,
                MaPhieuTiepNhan: self.listData.ThongTinXe.MaPhieuTiepNhan,
                BienSo: self.listData.ThongTinXe.BienSo,
                NgayXuatXuong: moment(new Date()).format('YYYY-MM-DD HH:mm'),
                SoKmRa: self.listData.ThongTinXe.SoKmRa,
                XuatXuong_GhiChu: '',
            };
            vmXuatXuong.MauIn.ListData.HoaDon = $.extend({}, self.MauIn.ListData.HoaDon);

            if (self.listData.HoaDons.length > 0) {
                $.getJSON(self.GaraAPI + 'CheckHoaDon_DaXuLy?idHoaDon=' + self.listData.HoaDons[0].ID + '&loaiHoaDon=8').done(function (x) {
                    if (x === false) {
                        $.getJSON(self.GaraAPI + 'Check_HDSC_ContainsHangHoa?idHoaDon=' + self.listData.HoaDons[0].ID).done(function (x) {
                            if (x) {
                                dialogConfirm('Thông báo', 'Chưa xuất kho hóa đơn sửa chữa <b> ' + self.listData.HoaDons[0].MaHoaDon + '</b>. Bạn có chắc chắn muốn xuất xưởng không?', function () {
                                    vmXuatXuong.ShowModalXuatXuong();
                                })
                            }
                        })
                    }
                    else {
                        vmXuatXuong.ShowModalXuatXuong();
                    }
                })
            } else {
                vmXuatXuong.ShowModalXuatXuong();
            }
        },
        showModalChiTietHoaDon: function (id) {
            var self = this;
            vmHoaHongDV.listData.NhanViens = self.listData.NhanViens;
            vmChiTietHoaDon.showModalChiTietHoaDon(id);
        },
        Duyet_HuyBaoGia: function (item, type) {
            var self = this;
            var title = '';
            var statusText = '';
            var status = '';
            var title = '';
            switch (type) {
                case false:// duyet
                    title = 'Duyệt';
                    statusText = 'Đã duyệt';
                    status = '0';
                    break;
                case true:// huy duyet
                    title = 'Hủy duyệt';
                    statusText = 'Chờ duyệt';
                    status = '1';
                    break;
                case null:// duyet
                    title = 'Hủy';
                    statusText = 'Đã hủy';
                    status = '2';
                    break;

            }
            commonStatisJs.ConfirmDialog_OKCancel(title.concat(' báo giá'),
                'Bạn có chắc chắn muốn '.concat(title.toLocaleLowerCase(), ' báo giá <b> ', item.MaHoaDon, ' </b> không ? '), function () {
                    title = title.toLocaleLowerCase();

                    ajaxHelper(self.GaraAPI + 'Duyet_HuyBaoGia?id=' + item.ID + '&trangthai=' + type, 'GET').done(function (x) {
                        if (x.res) {
                            commonStatisJs.ShowMessageSuccess('Đã ' + title + ' báo giá thành công');
                            item.TrangThai = 0;
                            item.TrangThaiText = 'Đã ' + title;
                            // savediary
                            var diary = {
                                ID_DonVi: self.ID_DonVi,
                                ID_NhanVien: self.ID_NhanVien,
                                ChucNang: 'Bàn làm việc - Báo giá',
                                LoaiNhatKy: 1,
                                NoiDung: title.concat(' báo giá ', item.MaHoaDon),
                                NoiDungChiTiet: title.concat(' báo giá ', item.MaHoaDon,
                                    ', người ', title, ': ', self.UserLogin),
                            }
                            Insert_NhatKyThaoTac_1Param(diary);

                            for (let i = 0; i < self.listData.BaoGias.length; i++) {
                                if (self.listData.BaoGias[i].ID === item.ID) {
                                    self.listData.BaoGias[i].TrangThaiText = statusText;
                                    self.listData.BaoGias[i].TrangThai = status;
                                    break;
                                }
                            }
                        }
                        else {
                            commonStatisJs.ShowMessageDanger(title + ' báo giá thất bại');
                        }
                    })

                })
        },
        Huy_HoaDon: function (item, loaiHD) {
            var self = this;
            //var msgBottom = '';
            var msgDialog = '';
            var idHoaDon = item.ID;
            var mahoadon = item.MaHoaDon;
            item.DiemGiaoDich = 0; // tạm thời gán điểm giao dịch = 0
            var sLoai = '';
            var urlCheck = '';
            switch (loaiHD) {
                case 25:
                    urlCheck = 'CheckHoaDon_DaXuLy?idHoaDon=' + idHoaDon + '&loaiHoaDon=8';
                    sLoai = 'hóa đơn';
                    msgBottom = "Hóa đơn đã có phiếu xuất kho, không thể hủy";
                    break;
                case 3:
                    urlCheck = 'CheckBaoGia_DaTaoHoaDonSuaChua_VaXuatKho?id=' + idHoaDon;
                    idHoaDon = item.ID;
                    sLoai = 'báo giá';
                    msgBottom = "Báo giá đã tạo hóa đơn và xuất kho. Không thể hủy";
                    break;
            }
            // khong duoc huy: hoadon da xuatkho, baogia: da co hoadon + xuatkho
            ajaxHelper(self.GaraAPI + urlCheck, 'GET').done(function (x) {
                if (x === true) {
                    commonStatisJs.ShowMessageDanger(msgBottom);
                    return;
                }
                else {
                    if (item.LoaiHoaDon !== 3) {
                        msgDialog = 'Có muốn hủy hóa đơn <b>' + mahoadon + '</b> cùng những phiếu liên quan không?'
                    }
                    else {
                        if (item.KhachDaTra > 0) {
                            msgDialog = 'Có muốn hủy hóa đơn <b>' + mahoadon + '</b> cùng tiền đặt cọc không?'
                        }
                        else {
                            msgDialog = 'Có muốn hủy hóa đơn <b>' + mahoadon + '</b> cùng những phiếu liên quan không?'
                        }
                    }
                    // move dialogConfirm() in this
                    dialogConfirm('Thông báo xóa', msgDialog, function () {
                        ajaxHelper(self.HoaDonAPI + "Huy_HoaDon?id=" + idHoaDon +
                            '&nguoiSua=' + self.UserLogin + '&iddonvi=' + self.ID_DonVi, 'post').done(function (x) {
                                commonStatisJs.ShowMessageSuccess("Cập nhật " + sLoai + " thành công");
                                var objDiary = {
                                    ID_NhanVien: self.ID_NhanVien,
                                    ID_DonVi: self.ID_DonVi,
                                    ChucNang: 'Bàn làm việc -' + sLoai,
                                    NoiDung: "Xóa " + sLoai + ": " + mahoadon,
                                    LoaiNhatKy: 3
                                };
                                if (item.LoaiHoaDon !== 3) {
                                    // HuyHD : tru diem (cong diem am)
                                    // Huy TraHang: cong diem
                                    // HuyDatHang: khong thuc hien gi ca
                                    var diemGiaoDich = item.DiemGiaoDich;
                                    if (diemGiaoDich > 0 && item.ID_DoiTuong !== null) {
                                        if (item.LoaiHoaDon === 25) {
                                            diemGiaoDich = -diemGiaoDich;
                                        }
                                        ajaxHelper('/api/DanhMuc/DM_DoiTuongAPI' + 'HuyHD_UpdateDiem?idDoiTuong=' + item.ID_DoiTuong + '&diemGiaoDich=' + diemGiaoDich, 'POST').done(function (data) {
                                        });
                                    }
                                }
                                Insert_NhatKyThaoTac_1Param(objDiary);

                                if (loaiHD === 3) {
                                    for (let i = 0; i < self.listData.BaoGias.length; i++) {
                                        if (self.listData.BaoGias[i].ID === item.ID) {
                                            self.listData.BaoGias.splice(i, 1);
                                            break;
                                        }
                                    }
                                }
                                else {
                                    for (let i = 0; i < self.listData.HoaDons.length; i++) {
                                        if (self.listData.HoaDons[i].ID === item.ID) {
                                            self.listData.HoaDons.splice(i, 1);
                                            break;
                                        }
                                    }
                                    self.HuyHoaDon_UpdateLichBaoDuong(idHoaDon);
                                }
                            }).always(function () {
                                $('#wait').remove();
                                $('#modalPopuplgDelete').modal('hide');
                            }).fail(function () {
                                commonStatisJs.ShowMessageDanger("Cập nhật " + sLoai + " thất bại");
                            })
                    })
                }
            });
        },
        HuyHoaDon_UpdateLichBaoDuong: function (idHoaDon) {
            var self = this;
            ajaxHelper(self.GaraAPI + 'HuyHoaDon_UpdateLichBaoDuong?idHoaDon=' + idHoaDon, 'GET').done(function (x) {
            });
        },

        PhieuTiepNhan_ClickDaSuaXong: function (item) {
            var self = this;
            $.getJSON(self.GaraAPI + 'PhieuTiepNhan_UpdateTrangThai?id=' + item.ID + '&status=2').done(function (x) {
                if (x.res) {
                    commonStatisJs.ShowMessageSuccess("Cập nhật phiếu thành công");
                    var diary = {
                        ID_DonVi: self.ID_DonVi,
                        ID_NhanVien: self.ID_NhanVien,
                        LoaiNhatKy: 2,
                        ChucNang: 'Bàn làm việc - Phiếu tiếp nhận',
                        NoiDung: 'Cập nhật phiếu tiếp nhận - Xe đã sửa xong ',
                        NoiDungChiTiet: 'Cập nhật phiếu tiếp nhận '.concat(item.MaPhieuTiepNhan, ', Đã sửa xong xe ', item.BienSo)
                    }
                    Insert_NhatKyThaoTac_1Param(diary);
                    item.TrangThai = 2;
                }
                else {
                    commonStatisJs.ShowMessageDanger("Cập nhật thất bại");
                }
            })
        },
        PhieuTiepNhan_ClickDaSuaXong1: function () {
            var self = this;
            $.getJSON(self.GaraAPI + 'PhieuTiepNhan_UpdateTrangThai?id=' + self.listData.ThongTinXe.ID + '&status=2').done(function (x) {
                if (x.res) {
                    commonStatisJs.ShowMessageSuccess("Cập nhật phiếu thành công");
                    var diary = {
                        ID_DonVi: self.ID_DonVi,
                        ID_NhanVien: self.ID_NhanVien,
                        LoaiNhatKy: 2,
                        ChucNang: 'Bàn làm việc - Phiếu tiếp nhận',
                        NoiDung: 'Cập nhật phiếu tiếp nhận - Xe đã sửa xong ',
                        NoiDungChiTiet: 'Cập nhật phiếu tiếp nhận '.concat(self.listData.ThongTinXe.MaPhieuTiepNhan, ', Đã sửa xong xe ', self.listData.ThongTinXe.BienSo)
                    }
                    Insert_NhatKyThaoTac_1Param(diary);
                    self.listData.PhieuSuaChuas.find(p => p.ID === self.listData.ThongTinXe.ID).TrangThai = 2;
                }
                else {
                    commonStatisJs.ShowMessageDanger("Cập nhật thất bại");
                }
            });
            $('#modal-xacnhan-suaxong').modal('hide');
        },
        PhieuTiepNhan_ClickDaSuaXong1_GuiTin: function () {
            let self = this;
            var lstDienThoai = [];
            if (self.listData.ThongTinXe.DienThoaiKhachHang !== '') {
                lstDienThoai.push({
                    SoDienThoai: self.listData.ThongTinXe.DienThoaiKhachHang,
                    TenDoiTuong: self.listData.ThongTinXe.TenDoiTuong
                });
            }
            if (self.listData.ThongTinXe.SoDienThoaiLienHe !== '') {
                lstDienThoai.push({
                    SoDienThoai: self.listData.ThongTinXe.SoDienThoaiLienHe,
                    TenDoiTuong: self.listData.ThongTinXe.TenLienHe
                });
            }
            if (lstDienThoai.length > 0) {
                self.PhieuTiepNhan_ClickDaSuaXong1();
                VSendSms.ListSoDienThoai = lstDienThoai;
                VSendSms.BienSoXe = self.listData.ThongTinXe.BienSo;
                VSendSms.openModal();
            }
            else {
                commonStatisJs.ShowMessageDanger('Không có số điện thoại liên hệ. Vui lòng kiểm tra thông tin khách hàng và phiếu tiếp nhận.');
            }
        },
        PhieuTiepNhan_XacNhanSuaXong: function () {
            $('#modal-xacnhan-suaxong').modal('show');
        },
        AddBaoGia: function (item) {
            var self = this;
            item.LoaiHoaDon = 3;
            localStorage.setItem('phieuTN', JSON.stringify(item));
            self.GotoGara('TN_taobaogia');
        },
        AddHoaDon: function (item) {
            var self = this;
            item.LoaiHoaDon = 25;
            localStorage.setItem('phieuTN', JSON.stringify(item));
            self.GotoGara('TN_taohoadon');
        },

        newHoaDon: function (item, trangthaiHD) {
            var obj = $.extend({}, item);
            obj.IDRandom = CreateIDRandom('HD_');
            obj.TongGiamGiaKM_HD = item.TongGiamGia + item.KhuyeMai_GiamGia;
            obj.PhaiThanhToanDB = 0;
            obj.TongGiaGocHangTra = 0;
            obj.TongChiPhiHangTra = 0;
            obj.HoanTraThuKhac = 0;
            obj.TongTienTra = 0;
            obj.PhaiTraKhach = 0;
            obj.DaTraKhach = 0;
            obj.GiaoHang = false;
            obj.TongGiamGiaDB = 0;
            obj.DiemGiaoDichDB = 0;
            obj.PTGiamDB = 0;
            obj.IsTraNhanh = false;
            obj.TongTienMua = 0;
            obj.PTThueDB = 0;
            obj.TongThueDB = 0;
            obj.TrangThaiHD = trangthaiHD; // 6.HD DatHang dang xuly, 8. capnhat hdsc sau khi thanhtoan, 3.capnhat baogia
            obj.Status = 1; // important

            obj.IsChose = true; // tức là đang thao tác với hóa đơn này
            obj.HoanTraTamUng = 0;
            obj.IsKhuyenMaiHD = false;
            obj.IsOpeningKMaiHD = false;

            obj.PTGiamGiaHH = 0;
            obj.TongGiamGiaHang = false;
            obj.TongTien = false;

            PTChietKhauHH = 0;
            TongGiamGiaHang = 0;
            TongTienHangChuaCK = 0;
            TongTienKhuyenMai_CT = 0;
            TongGiamGiaKhuyenMai_CT = 0;

            // Tich Diem
            obj.TTBangDiem = 0;
            obj.DiemGiaoDich = 0;
            obj.DiemQuyDoi = 0;
            obj.DiemHienTai = 0;
            obj.DiemCong = 0;
            obj.DiemKhuyenMai = 0;

            obj.ID_NhomDTApplySale = null;
            obj.CreateTime = 0;
            obj.TenViTriHD = '';
            obj.BH_NhanVienThucHiens = [];
            obj.TienTheGiaTri = 0;
            obj.ThoiGianThucHien = 0;
            obj.StatusOffline = false;
            obj.IsActive = '';
            return obj;
        },
        SetDefaultPropertierCTHD: function (ct) {
            ct.RoleChangePrice = true;
            ct.ShowEditQuyCach = false;
            ct.IsKhuyenMai = false;
            ct.IsOpeningKMai = false;
            ct.TenKhuyenMai = '';
            ct.HangHoa_KM = [];
            ct.DiemKhuyenMai = 0;
            ct.GhiChu_NVThucHien = '';
            ct.BH_NhanVienThucHien = [];
            ct.GhiChu_NVTuVan = '';
            ct.GhiChu_NVThucHienPrint = ''; // used to printHoaDon (not show %ChietKhau)
            ct.GhiChu_NVTuVanPrint = '';
            ct.ListDonViTinh = [];
            ct.ShowWarningQuyCach = false;
            ct.SoLuongQuyCach = 0;
            ct.ThanhPhan_DinhLuong = [];
            ct.HangCungLoais = [];
            ct.LaConCungLoai = false;
            ct.TenViTri = '';
            ct.TimeStart = 0;
            ct.QuaThoiGian = 0;
            ct.TimeRemain = 0;
            ct.ThoiGianThucHien = 0;
            ct.ThoiGian = moment(new Date()).format('YYYY-MM-DD HH:mm');
            if (!commonStatisJs.CheckNull(ct.DonViTinh)) {
                ct.ListDonViTinh = ct.DonViTinh;
            }
            return ct;
        },
        GetPTChietKhauHang_HeaderBH: function (cthd) {
            var ptCKHang = 0;
            var arr = $.grep(cthd, function (x) {
                return x.PTChietKhau === cthd[0].PTChietKhau;
            });
            if (arr.length === cthd.length) {
                ptCKHang = cthd[0].PTChietKhau;
            }

            var gtri = 0;
            var arrDonGiaBH = $.grep(cthd, function (x) {
                return x.DonGiaBaoHiem > 0;
            });
            if (arrDonGiaBH.length === cthd.length) {
                gtri = RoundDecimal(arrDonGiaBH[0].DonGiaBaoHiem / (arrDonGiaBH[0].DonGia - arrDonGiaBH[0].TienChietKhau) * 100, 3);
            }
            return {
                PTChietKhauHH: ptCKHang,
                HeaderBH: gtri,
            }
        },
        GetNgaySX_NgayHH: function (ctDoing) {
            var ngaysx = ctDoing.NgaySanXuat;
            if (!commonStatisJs.CheckNull(ngaysx)) {
                ngaysx = moment(ngaysx).format('DD/MM/YYYY');
            }
            var hansd = ctDoing.NgayHetHan;
            if (!commonStatisJs.CheckNull(hansd)) {
                hansd = moment(hansd).format('DD/MM/YYYY');
            }
            return {
                NgaySanXuat: ngaysx,
                NgayHetHan: hansd,
            }
        },
        AssignTPDinhLuong_toCTHD: function (itemCT) {
            if (itemCT.ThanhPhan_DinhLuong !== null && itemCT.ThanhPhan_DinhLuong !== undefined) {
                for (let k = 0; k < itemCT.ThanhPhan_DinhLuong.length; k++) {
                    let tpDL = itemCT.ThanhPhan_DinhLuong[k];
                    itemCT.ThanhPhan_DinhLuong[k].STT = k + 1;
                    itemCT.ThanhPhan_DinhLuong[k].isDefault = false;
                    itemCT.ThanhPhan_DinhLuong[k].SoLuongQuyCach = tpDL.SoLuong * tpDL.QuyCach;
                    itemCT.ThanhPhan_DinhLuong[k].SoLuongMacDinh = tpDL.SoLuong / itemCT.SoLuong; // assign = SoLuongDinhLuong_BanDau
                    itemCT.ThanhPhan_DinhLuong[k].GiaVonAfter = tpDL.SoLuong * tpDL.GiaVon;
                }
                return itemCT.ThanhPhan_DinhLuong;
            }
            else {
                return [];
            }
        },
        AssignNVThucHien_toCTHD: function (itemCT) {
            if (commonStatisJs.CheckNull(itemCT.BH_NhanVienThucHien)) {
                itemCT.BH_NhanVienThucHien = [];
            }

            var nvTH = '';
            var nvTV = '';
            var nvTH_Print = '';
            var nvTV_Print = '';

            var arrNVien = itemCT.BH_NhanVienThucHien;
            itemCT.BH_NhanVienThucHien = [];

            for (let j = 0; j < arrNVien.length; j++) {
                let itemFor = arrNVien[j];
                let isNVThucHien = itemFor.ThucHien_TuVan;
                let tienCK = itemFor.TienChietKhau;
                let gtriPtramCK = itemFor.PT_ChietKhau;
                let isPTram = gtriPtramCK > 0 ? true : false;
                let gtriCK_TH = 0;
                let gtriCK_TV = 0;
                let tacVu = 1;
                let ckMacDinh = gtriPtramCK;
                if (isNVThucHien) {
                    if (itemFor.TheoYeuCau) {
                        tacVu = 3;  // thuchien theo yeucau
                    }
                    else {
                        tacVu = 1;
                    }
                    if (isPTram) {
                        gtriCK_TH = gtriPtramCK;
                        nvTH += itemFor.TenNhanVien + ' (' + gtriCK_TH + ' %), ';
                    }
                    else {
                        gtriCK_TH = tienCK;
                        nvTH += itemFor.TenNhanVien + ' (' + formatNumber(gtriCK_TH) + ' đ), ';
                        ckMacDinh = tienCK / itemFor.HeSo / itemCT.SoLuong;
                    }
                    nvTH_Print += itemFor.TenNhanVien + ', ';
                }
                else {
                    tacVu = 2;
                    if (isPTram) {
                        gtriCK_TV = gtriPtramCK;
                        nvTV += itemFor.TenNhanVien + ' (' + gtriCK_TV + ' %), ';
                    }
                    else {
                        gtriCK_TV = tienCK;
                        nvTV += itemFor.TenNhanVien + ' (' + formatNumber(gtriCK_TV) + ' đ), ';
                        ckMacDinh = tienCK / itemFor.HeSo / itemCT.SoLuong;
                    }
                    nvTV_Print += itemFor.TenNhanVien + ', ';
                }
                let idRandom = CreateIDRandom('IDRandomCK_');
                let itemNV = {
                    IDRandom: idRandom,
                    ID_NhanVien: itemFor.ID_NhanVien,
                    TenNhanVien: itemFor.TenNhanVien,
                    ThucHien_TuVan: isNVThucHien,
                    TienChietKhau: tienCK,
                    PT_ChietKhau: gtriPtramCK,
                    TheoYeuCau: itemFor.TheoYeuCau,
                    TacVu: tacVu,
                    HeSo: itemFor.HeSo,
                    TinhChietKhauTheo: itemFor.TinhChietKhauTheo,
                    ChietKhauMacDinh: ckMacDinh,
                    TinhHoaHongTruocCK: itemFor.TinhHoaHongTruocCK,
                }
                itemCT.BH_NhanVienThucHien.push(itemNV);
            }

            itemCT.GhiChu_NVThucHien = (nvTH === '' ? '' : 'Thực hiện: ' + Remove_LastComma(nvTH));
            itemCT.GhiChu_NVThucHienPrint = (nvTH_Print === '' ? '' : + Remove_LastComma(nvTH_Print));
            itemCT.GhiChu_NVTuVan = (nvTV === '' ? '' : 'Tư vấn: ' + Remove_LastComma(nvTV));
            itemCT.GhiChu_NVTuVanPrint = (nvTV_Print === '' ? '' : + Remove_LastComma(nvTV_Print));
            itemCT.HoaHongTruocChietKhau = arrNVien.length > 0 ? arrNVien[0].TinhHoaHongTruocCK : itemCT.HoaHongTruocChietKhau;
            return itemCT;
        },
        AssignThanhPhanComBo_toCTHD: function (itemCT) {
            let self = this;
            if (commonStatisJs.CheckNull(itemCT.ThanhPhanComBo)) {
                return [];
            }
            var arrComBo = [];
            for (let k = 0; k < itemCT.ThanhPhanComBo.length; k++) {
                let combo = itemCT.ThanhPhanComBo[k];
                combo.IDRandom = CreateIDRandom('combo_');
                combo = self.AssignNVThucHien_toCTHD(combo);
                combo.ThanhPhan_DinhLuong = self.AssignTPDinhLuong_toCTHD(combo);
                let dateLot = self.GetNgaySX_NgayHH(combo);
                combo.NgaySanXuat = dateLot.NgaySanXuat;
                combo.NgayHetHan = dateLot.NgayHetHan;
                combo.LotParent = false;
                combo.DM_LoHang = [];

                if (itemCT.LoaiHoaDon !== 3) {
                    combo.TongPhiDichVu = combo.PhiDichVu * combo.SoLuong;
                    if (combo.LaPTPhiDichVu) {
                        combo.TongPhiDichVu = RoundDecimal(combo.PhiDichVu * combo.ThanhTien / 100, 3);
                    }
                }

                combo.LoaiHoaDon = itemCT.LoaiHoaDon;
                combo.MaHoaDon = itemCT.MaHoaDon;
                combo.ShowEditQuyCach = itemCT.ShowEditQuyCach;
                combo.RoleChangePrice = itemCT.RoleChangePrice;
                combo.ID_ViTri = itemCT.ID_ViTri;
                combo.TenViTri = itemCT.TenPhongBan;

                combo.SoLuongMacDinh = itemCT.SoLuong === 0 ? combo.SoLuong : combo.SoLuong / itemCT.SoLuong;
                combo.SoLuongDaMua = 0;
                combo.CssWarning = false;
                combo.IsKhuyenMai = false;
                combo.IsOpeningKMai = false;
                combo.TenKhuyenMai = '';
                combo.HangHoa_KM = [];
                combo.UsingService = false;
                combo.ListDonViTinh = [];
                combo.ShowWarningQuyCach = false;
                combo.SoLuongQuyCach = 0;
                combo.HangCungLoais = [];
                combo.LaConCungLoai = false;
                combo.TimeStart = 0;
                combo.QuaThoiGian = 0;
                combo.TimeRemain = 0;
                combo.ThoiGian = moment(new Date()).format('YYYY-MM-DD HH:mm');
                arrComBo.push(combo);
            }
            return arrComBo;
        },

        XuLyBaoGia: function (item) {
            // chi get cthd chua duoc xuly
            var self = this;
            var newObj = self.newHoaDon(item, 6);
            newObj.NgayLapHoaDon = moment(item.NgayLapHoaDon).format('YYYY-MM-DD HH:mm');
            newObj.DaThanhToan = 0;
            newObj.KhachDaTra = item.KhachDaTra;
            newObj.LoaiHoaDon = 3;
            newObj.TongGiamGiaDB = item.TongGiamGia;// used to check if xulyhoadon lan 2
            newObj.BienSo = self.listData.ThongTinXe.BienSo;
            newObj.TongThueKhachHang = item.TongTienThue;
            newObj.CongThucBaoHiem = 13;
            newObj.DuyetBaoGia = !item.ChoThanhToan;
            newObj.ID_Xe = self.listData.ThongTinXe.ID_Xe;

            var tonggiamgiahang = 0, tongtienhangchuaCK = 0;
            var chitiets = [];
            ajaxHelper(self.HoaDonAPI + 'GetCTHoaDon_afterDatHang?idHoaDon=' + item.ID).done(function (data) {
                if (data.length > 0) {
                    var cthd = data.sort(function (a, b) {
                        var x = a.SoThuTu,
                            y = b.SoThuTu;
                        return x < y ? -1 : x > y ? 1 : 0;
                    });
                    var ctConLai = $.grep(cthd, function (x) {
                        return x.SoLuongConLai > 0;
                    });
                    if (ctConLai.length === 0) {
                        commonStatisJs.ShowMessageDanger('Đã xử lý hết báo giá');
                        return;
                    }

                    var arrIDQuiDoi = [];
                    var cthdLoHang = [];
                    var updatePrice = self.CheckRole('HoaDon_ThayDoiGia');
                    for (let i = 0; i < cthd.length; i++) {
                        let ctNew = cthd[i];
                        delete cthd[i]["ID_HoaDon"];  // delete avoid error

                        let dateLot = self.GetNgaySX_NgayHH(ctNew);
                        ctNew.NgaySanXuat = dateLot.NgaySanXuat;
                        ctNew.NgayHetHan = dateLot.NgayHetHan;

                        ctNew.MaHoaDon = item.MaHoaDon;
                        ctNew.LoaiHoaDon = 3;
                        ctNew.SoLuongDaMua = 0;
                        ctNew.DVTinhGiam = 'VND';
                        ctNew.GiaBan = ctNew.DonGia;
                        if (ctNew.PTChietKhau > 0) {
                            ctNew.DVTinhGiam = '%';
                        }
                        // lo hang
                        let quanlytheolo = ctNew.QuanLyTheoLoHang;
                        ctNew.DM_LoHang = [];
                        ctNew.LotParent = quanlytheolo;
                        ctNew.ID_ChiTietGoiDV = null;
                        ctNew.ChatLieu = '3';
                        ctNew.RoleChangePrice = updatePrice;
                        ctNew.ShowEditQuyCach = false;
                        let quycach = ctNew.QuyCach === null || ctNew.QuyCach === 0 ? 1 : ctNew.QuyCach;
                        ctNew.QuyCach = quycach;
                        ctNew.CssWarning = false;
                        ctNew = self.SetDefaultPropertierCTHD(ctNew);
                        ctNew.IDRandomHD = newObj.IDRandom;

                        tonggiamgiahang += ctNew.SoLuong * ctNew.TienChietKhau;
                        tongtienhangchuaCK += ctNew.SoLuong * ctNew.DonGia;

                        // get nvth
                        let nvth = $.grep(self.listData.HoaDonChiTiets, function (o) {
                            return o.ID === ctNew.ID;
                        });
                        if (nvth.length > 0) {
                            ctNew.BH_NhanVienThucHien = nvth[0].BH_NhanVienThucHien;
                            ctNew.GhiChu_NVThucHien = nvth[0].GhiChu_NVThucHien;
                            ctNew.GhiChu_NVThucHienPrint = nvth[0].GhiChu_NVThucHienPrint;
                            ctNew.GhiChu_NVTuVan = nvth[0].GhiChu_NVTuVan;
                            ctNew.GhiChu_NVTuVanPrint = nvth[0].GhiChu_NVTuVanPrint;
                        }

                        // get tpdinhluong
                        let tpdl = $.grep(self.listData.HoaDonChiTiets, function (o) {
                            return o.ID === ctNew.ID;
                        });
                        if (tpdl.length > 0) {
                            ctNew.ThanhPhan_DinhLuong = self.AssignTPDinhLuong_toCTHD(tpdl[0]);
                        }

                        let combo = $.grep(vmThanhPhanCombo.AllComBo_ofHD, function (x) {
                            return x.ID_ParentCombo === ctNew.ID_ParentCombo;
                        });
                        if (combo.length > 0) {
                            ctNew.ThanhPhanComBo = combo;
                            ctNew.ThanhPhanComBo = self.AssignThanhPhanComBo_toCTHD(ctNew);
                        }
                        else {
                            ctNew.ThanhPhanComBo = [];
                        }

                        // check exist in cthdLoHang
                        let objLot = {};
                        if ($.inArray(ctNew.ID_DonViQuiDoi, arrIDQuiDoi) === -1) {
                            arrIDQuiDoi.unshift(ctNew.ID_DonViQuiDoi);
                            // push CTHD
                            ctNew.SoThuTu = cthdLoHang.length + 1;
                            ctNew.IDRandom = CreateIDRandom('RandomCT_');
                            if (quanlytheolo) {
                                objLot = $.extend({}, ctNew);
                                objLot.HangCungLoais = [];
                                objLot.DM_LoHang = [];
                                ctNew.DM_LoHang.push(objLot);
                            }
                            cthdLoHang.unshift(ctNew);
                        }
                        else {
                            // find in cthdLoHang with same ID_QuiDoi
                            for (let j = 0; j < cthdLoHang.length; j++) {
                                if (cthdLoHang[j].ID_DonViQuiDoi === ctNew.ID_DonViQuiDoi) {
                                    if (quanlytheolo) {
                                        objLot = $.extend({}, ctNew);
                                        objLot.HangCungLoais = [];
                                        objLot.DM_LoHang = [];
                                        objLot.LotParent = false;
                                        objLot.IDRandom = CreateIDRandom('RandomCT_');
                                        cthdLoHang[j].DM_LoHang.push(objLot);
                                    }
                                    else {
                                        ctNew.IDRandom = CreateIDRandom('RandomCT_');
                                        ctNew.LaConCungLoai = true;
                                        cthdLoHang[j].HangCungLoais.push(ctNew);
                                    }
                                    break;
                                }
                            }
                        }
                    }

                    // asign agin tonggiamgia to hd
                    let ptck = self.GetPTChietKhauHang_HeaderBH(cthd);
                    newObj.PTChietKhauHH = ptck.PTChietKhauHH;
                    newObj.TongGiamGiaHang = tonggiamgiahang;
                    newObj.TongTienHangChuaCK = tongtienhangchuaCK;
                    newObj.HeaderBH_GiaTriPtram = 0;
                    newObj.HeaderBH_Type = 1;

                    // xuly nhieu baogia cungluc
                    let hd_XLBG = localStorage.getItem('lcXuLiDonHang');
                    if (hd_XLBG === null) {
                        hd_XLBG = [];
                    }
                    else {
                        hd_XLBG = JSON.parse(hd_XLBG);
                        hd_XLBG = $.grep(hd_XLBG, function (itemXL) {
                            return itemXL.MaHoaDon !== item.MaHoaDon;
                        });
                    }
                    hd_XLBG.push(newObj);

                    var cthd_XLBG = localStorage.getItem('lcCTDatHang');
                    if (cthd_XLBG === null) {
                        cthd_XLBG = [];
                    }
                    else {
                        cthd_XLBG = JSON.parse(cthd_XLBG);
                        cthd_XLBG = $.grep(cthd_XLBG, function (x) {
                            return x.MaHoaDon !== item.MaHoaDon;
                        })
                    }

                    // push in cache CTHD (xu li nhieu CT cung luc)
                    for (let i = 0; i < cthdLoHang.length; i++) {
                        cthd_XLBG.push(cthdLoHang[i]);
                    }

                    localStorage.setItem('lcCTDatHang', JSON.stringify(cthd_XLBG));
                    localStorage.setItem('lcXuLiDonHang', JSON.stringify(hd_XLBG));
                    localStorage.setItem('maHDCache', item.MaHoaDon);// used to get at gara.js (phieu dang xuly)
                    self.GotoGara('TN_xulyBG');
                }
            });
        },
        CapNhatHoaDon: function (item, loaiHD) {
            // chi update neu dathang chua duoc xuly
            var self = this;
            var loaiCheck = loaiHD == 3 ? 25 : 8;
            // neu hoadon da xuatkho, hoac baogia da tao hoadon --> khong dc sua
            $.getJSON(self.GaraAPI + 'CheckHoaDon_DaXuLy?idHoaDon=' + item.ID + '&loaiHoaDon=' + loaiCheck).done(function (x) {
                if (x == true) {
                    if (loaiHD === 3) {
                        commonStatisJs.ShowMessageDanger('Báo giá đã tạo hóa đơn. Không thể sửa đổi');
                    }
                    else {
                        commonStatisJs.ShowMessageDanger('Hóa đơn đã xuất kho. Không thể sửa đổi');
                    }
                    return;
                }
                var newObj = self.newHoaDon(item, loaiHD === 3 ? 3 : 8);
                newObj.DaThanhToan = 0;
                newObj.SoDuDatCoc = 0;
                newObj.DuyetBaoGia = !item.ChoThanhToan;
                newObj.LoaiHoaDon = loaiHD;
                newObj.BienSo = self.listData.ThongTinXe.BienSo;
                newObj.ID_Xe = self.listData.ThongTinXe.ID_Xe;
                newObj.KhachDaTra = loaiHD === 3 ? 0 : item.KhachDaTra;

                let thueKH = item.TongThueKhachHang;
                if (commonStatisJs.CheckNull(thueKH)) {
                    newObj.TongThueKhachHang = newObj.TongTienThue;
                }
                let congthuc = item.CongThucBaoHiem;
                if (commonStatisJs.CheckNull(congthuc)) {
                    newObj.CongThucBaoHiem = 13;
                }
                if (vmHoaHongHoaDon.GridNVienBanGoi_Chosed.length > 0) {
                    newObj.BH_NhanVienThucHiens = vmHoaHongHoaDon.GridNVienBanGoi_Chosed;
                }

                var tonggiamgiahang = 0, tongtienhangchuaCK = 0;
                var updatePrice = loaiHD == 3 ? self.CheckRole('DatHang_ThayDoiGia') : self.CheckRole('HoaDon_ThayDoiGia');
                var chitiets = [];
                ajaxHelper(self.HoaDonAPI + 'SP_GetChiTietHD_byIDHoaDon?idHoaDon=' + item.ID).done(function (data) {
                    if (data.length > 0) {
                        var cthd = data.sort(function (a, b) {
                            var x = a.SoThuTu,
                                y = b.SoThuTu;
                            return x < y ? -1 : x > y ? 1 : 0;
                        });

                        let arrIDQuiDoi = [];
                        let cthdLoHang = [];
                        for (let i = 0; i < cthd.length; i++) {
                            let ctNew = cthd[i];

                            delete cthd[i]["ID_HoaDon"];  // delete avoid error

                            let dateLot = self.GetNgaySX_NgayHH(ctNew);
                            ctNew.NgaySanXuat = dateLot.NgaySanXuat;
                            ctNew.NgayHetHan = dateLot.NgayHetHan;

                            ctNew.MaHoaDon = item.MaHoaDon;
                            ctNew.LoaiHoaDon = loaiHD;
                            ctNew.SoLuongDaMua = 0;
                            ctNew.DVTinhGiam = 'VND';
                            ctNew.GiaBan = ctNew.DonGia;
                            if (ctNew.PTChietKhau > 0) {
                                ctNew.DVTinhGiam = '%';
                            }
                            // lo hang
                            let quanlytheolo = ctNew.QuanLyTheoLoHang;
                            ctNew.DM_LoHang = [];
                            ctNew.LotParent = quanlytheolo;
                            ctNew.RoleChangePrice = updatePrice;
                            ctNew.ShowEditQuyCach = false;
                            let quycach = ctNew.QuyCach === null || ctNew.QuyCach === 0 ? 1 : ctNew.QuyCach;
                            ctNew.QuyCach = quycach;
                            ctNew.CssWarning = false;
                            ctNew = self.SetDefaultPropertierCTHD(ctNew);

                            if (loaiHD !== 3) {
                                ctNew.TongPhiDichVu = ctNew.SoLuong * ctNew.PhiDichVu;
                                if (ctNew.LaPTPhiDichVu) {
                                    ctNew.TongPhiDichVu
                                        = Math.round(ctNew.SoLuong * ctNew.GiaBan * ctNew.PhiDichVu / 100);
                                }
                            }

                            tonggiamgiahang += ctNew.SoLuong * ctNew.TienChietKhau;
                            tongtienhangchuaCK += ctNew.SoLuong * ctNew.DonGia;

                            // get nvth
                            let nvth = $.grep(self.listData.HoaDonChiTiets, function (o) {
                                return o.ID === ctNew.ID;
                            });
                            if (nvth.length > 0) {
                                ctNew.BH_NhanVienThucHien = nvth[0].BH_NhanVienThucHien;
                                ctNew.GhiChu_NVThucHien = nvth[0].GhiChu_NVThucHien;
                                ctNew.GhiChu_NVThucHienPrint = nvth[0].GhiChu_NVThucHienPrint;
                                ctNew.GhiChu_NVTuVan = nvth[0].GhiChu_NVTuVan;
                                ctNew.GhiChu_NVTuVanPrint = nvth[0].GhiChu_NVTuVanPrint;
                                ctNew.HoaHongTruocChietKhau = nvth[0].HoaHongTruocChietKhau;
                            }

                            // get tpdinhluong
                            let tpdl = $.grep(self.listData.HoaDonChiTiets, function (o) {
                                return o.ID === ctNew.ID;
                            });
                            if (tpdl.length > 0) {
                                ctNew.ThanhPhan_DinhLuong = self.AssignTPDinhLuong_toCTHD(tpdl[0]);
                            }

                            let combo = $.grep(vmThanhPhanCombo.AllComBo_ofHD, function (x) {
                                return x.ID_ParentCombo === ctNew.ID_ParentCombo;
                            });
                            if (combo.length > 0) {
                                ctNew.ThanhPhanComBo = combo;
                                ctNew.ThanhPhanComBo = self.AssignThanhPhanComBo_toCTHD(ctNew);
                            }
                            else {
                                ctNew.ThanhPhanComBo = [];
                            }

                            // check exist in cthdLoHang
                            let objLot = {};
                            if ($.inArray(ctNew.ID_DonViQuiDoi, arrIDQuiDoi) === -1) {
                                arrIDQuiDoi.unshift(ctNew.ID_DonViQuiDoi);

                                ctNew.SoThuTu = cthdLoHang.length + 1;
                                ctNew.IDRandom = CreateIDRandom('RandomCT_');
                                ctNew.IDRandomHD = newObj.IDRandom;
                                if (quanlytheolo) {
                                    objLot = $.extend({}, ctNew);
                                    objLot.HangCungLoais = [];
                                    objLot.DM_LoHang = [];
                                    ctNew.DM_LoHang.push(objLot);
                                }
                                cthdLoHang.unshift(ctNew);
                            }
                            else {
                                // find in cthdLoHang with same ID_QuiDoi
                                for (let j = 0; j < cthdLoHang.length; j++) {
                                    if (cthdLoHang[j].ID_DonViQuiDoi === ctNew.ID_DonViQuiDoi) {
                                        if (quanlytheolo) {
                                            objLot = $.extend({}, ctNew);
                                            objLot.HangCungLoais = [];
                                            objLot.DM_LoHang = [];
                                            objLot.LotParent = false;
                                            objLot.IDRandom = CreateIDRandom('RandomCT_');
                                            cthdLoHang[j].DM_LoHang.push(objLot);
                                        }
                                        else {
                                            ctNew.IDRandom = CreateIDRandom('RandomCT_');
                                            ctNew.LaConCungLoai = true;
                                            cthdLoHang[j].HangCungLoais.push(ctNew);
                                        }
                                        break;
                                    }
                                }
                            }
                        }

                        // push in cache CTHD (xu li nhieu CT cung luc)
                        for (let i = 0; i < cthdLoHang.length; i++) {
                            chitiets.push(cthdLoHang[i]);
                        }

                        // asign again tonggiamgia to hd
                        let ptck = self.GetPTChietKhauHang_HeaderBH(cthd);
                        newObj.PTChietKhauHH = ptck.PTChietKhauHH;
                        newObj.TongGiamGiaHang = tonggiamgiahang;
                        newObj.TongTienHangChuaCK = tongtienhangchuaCK;
                        newObj.HeaderBH_GiaTriPtram = 0;
                        newObj.HeaderBH_Type = 1;

                        localStorage.setItem('lcHDSaoChep', JSON.stringify(newObj));
                        localStorage.setItem('lcCTHDSaoChep', JSON.stringify(chitiets));
                        self.GotoGara('TN_updateHD');
                    }
                });

                let cacheCP = localStorage.getItem('lcChiPhi');
                if (cacheCP !== null) {
                    cacheCP = JSON.parse(cacheCP);
                    // remove & add again
                    cacheCP = $.grep(cacheCP, function (x) {
                        return x.ID_HoaDon !== item.ID;
                    });
                }
                else {
                    cacheCP = [];
                }
                if (VueChiPhi.ListChiPhi.length > 0) {
                    let arrCP = $.extend([], true, VueChiPhi.ListChiPhi);
                    for (let m = 0; m < arrCP.length; m++) {
                        let for1 = arrCP[m];
                        for (let n = for1.ChiTiets.length - 1; n > -1; n--) {
                            let for2 = for1.ChiTiets[n];
                            if (for2.ID_NhaCungCap === null) {
                                for1.ChiTiets.splice(n, 1);
                            }
                        }
                        cacheCP.push(for1);
                    }

                    for (let m = cacheCP.length - 1; m > -1; m--) {
                        if (cacheCP[m].ChiTiets.length === 0) {
                            cacheCP.splice(m, 1);
                        }
                    }
                }
                if (cacheCP.length > 0) {
                    localStorage.setItem('lcChiPhi', JSON.stringify(cacheCP));
                }
            })
        },

        XuatKho: function (item) {
            ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'GetCTHDSuaChua_afterXuatKho?idHoaDon=' + item.ID).done(function (x) {
                //if (x.res && x.dataSoure.length === 0) {
                //    commonStatisJs.ShowMessageDanger('Hóa đơn đã xuất kho đủ số lượng');
                //    return;
                //}
                localStorage.setItem('lcXK_EditOpen', JSON.stringify([item]));
                localStorage.setItem('XK_createfrom', 3);
                window.open('/#/XuatKhoChiTiet');
            });
        },

        GotoGara: function (cacheValue, typePage = 0) {
            var target = '_blank';
            if (window.matchMedia('(display-mode: standalone)').matches || (window.navigator.standalone) || document.referrer.includes('android-app://')) {
                target = '_self'
            }
            var url = '/g/Gara';
            switch (typePage) {
                case 0://gara
                    localStorage.setItem('gara_CreateFrom', cacheValue);
                    break;
                case 1:// nhaphang
                    url = '/#/PurchaseOrderItem2';
                    break;
            }
            var newwindow = window.open(url, target);
            var popupTick = setInterval(function () {
                if (!commonStatisJs.CheckNull(newwindow)) {
                    if (newwindow.closed) {
                        clearInterval(popupTick);
                        switch (cacheValue) {
                            case 'TN_taobaogia':
                                workTable.Gara_GetListBaoGia(true);
                                break;
                            case 'TN_taohoadon':
                                workTable.Gara_GetListHoaDonSuaChua(true);
                                break;
                            case 'TN_xulyBG':
                            case 'TN_updateHD':
                                workTable.Gara_GetListBaoGia(true);
                                workTable.Gara_GetListHoaDonSuaChua(true);
                                break;
                            default:
                                workTable.Gara_GetListHoaDonSuaChua(true);
                                break;
                        }
                        $(".gara-section-list-body li").removeClass("open");
                    }
                }
            }, 500);
        },
        showModalThanhToan: function (item) {
            var self = this;
            var tenNV = '';
            let nvien = $.grep(self.listData.NhanViens, function (x) {
                return x.ID === self.ID_NhanVien;
            });
            if (nvien.length > 0) {
                tenNV = nvien[0].TenNhanVien;
            }
            vmThanhToan.inforLogin = {
                ID_NhanVien: self.ID_NhanVien,
                ID_User: self.ID_User,
                UserLogin: self.UserLogin,
                ID_DonVi: self.ID_DonVi,
                TenNhanVien: tenNV,
            };
            if (self.listData.CongTy.length > 0) {
                vmThanhToan.inforCongTy = {
                    TenCongTy: self.listData.CongTy[0].TenCongTy,
                    DiaChiCuaHang: self.listData.CongTy[0].DiaChi,
                    LogoCuaHang: Open24FileManager.hostUrl + self.listData.CongTy[0].DiaChiNganHang,
                    TenChiNhanh: self.TenDonVi,
                };
            }
            item.DienThoai = item.DienThoaiKhachHang;
            vmThanhToan.listData.AccountBanks = self.listData.AccountBanks;
            vmThanhToan.listData.KhoanThuChis = self.listData.KhoanThuChis.filter(x => x.LaKhoanThu);

            vmThanhToan.showModalThanhToan(item);
        },
        GetinforCus_byID: async function (idCus) {
            if (!commonStatisJs.CheckNull(idCus)) {
                let date = moment(new Date()).format('YYYY-MM-DD HH:mm');
                let xx = await ajaxHelper('/api/DanhMuc/DM_DoiTuongAPI/' + "GetInforKhachHang_ByID?idDoiTuong=" + idCus + '&idChiNhanh=' + VHeader.IdDonVi
                    + '&timeStart=2016-01-01&timeEnd=' + date + '&wasChotSo=false', 'GET').done(function () {
                    })
                    .then(function (data) {
                        if (data !== null && data.length > 0) {
                            return data[0];
                        }
                        return {};
                    })
                return xx;
            }
            return {};
        },
        ChoseItem_inList: function (item, loai) {
            var self = this;
            item.LoaiHoaDon = loai;
            self.itemChosing = item;

            self.wasChotSo = false;
            let chotSo = VHeader.CheckKhoaSo(moment(item.NgayLapHoaDon).format('YYYY-MM-DD'), item.ID_DonVi);
            if (chotSo) {
                self.wasChotSo = true;
            }

            vmThanhPhanCombo.GetAllCombo_byIDHoaDon(item.ID);
            VueChiPhi.CTHD_GetChiPhiDichVu([item.ID]);
            vmHoaHongHoaDon.GetChietKhauHoaDon_byID(item, null, false);

            ajaxHelper(self.HoaDonAPI + 'SP_GetChiTietHD_byIDHoaDon_ChietKhauNV?idHoaDon=' + item.ID).done(function (data) {
                let arrHH = data.filter(x => x.LaHangHoa);
                let arrDV = data.filter(x => x.LaHangHoa === false);

                let tongDV = 0, tongDV_truocVAT = 0, tongDV_truocCK = 0;
                let tongHH = 0, tongHH_truocVAT = 0, tongHH_truocCK = 0;
                let DV_tongthue = 0, DV_tongCK = 0, DV_tongSL = 0;
                let HH_tongthue = 0, HH_tongCK = 0, HH_tongSL = 0;
                for (let k = 0; k < arrHH.length; k++) {
                    let itFor = arrHH[k];
                    let soluong = formatNumberToFloat(itFor.SoLuong);
                    HH_tongSL += soluong;
                    tongHH += formatNumberToFloat(itFor.ThanhToan);
                    tongHH_truocVAT += formatNumberToFloat(itFor.ThanhTien);
                    tongHH_truocCK += soluong * formatNumberToFloat(itFor.DonGia);
                    HH_tongthue += soluong * formatNumberToFloat(itFor.TienThue);
                    HH_tongCK += soluong * formatNumberToFloat(itFor.TienChietKhau);
                }
                for (let k = 0; k < arrDV.length; k++) {
                    let itFor = arrDV[k];
                    let soluong = formatNumberToFloat(itFor.SoLuong);
                    DV_tongSL += soluong;
                    tongDV += formatNumberToFloat(itFor.ThanhToan);
                    tongDV_truocVAT += formatNumberToFloat(itFor.ThanhTien);
                    tongDV_truocCK += soluong * formatNumberToFloat(itFor.DonGia);
                    DV_tongthue += soluong * formatNumberToFloat(itFor.TienThue);
                    DV_tongCK += soluong * formatNumberToFloat(itFor.TienChietKhau);
                }

                self.itemChosing.TongGiamGiaHang = DV_tongCK + HH_tongCK;
                self.itemChosing.TongTienHangChuaCK = tongDV_truocCK + tongHH_truocCK;

                self.itemChosing.TongSL_DichVu = DV_tongSL;
                self.itemChosing.TongTienDichVu = tongDV;
                self.itemChosing.TongThue_DichVu = DV_tongthue;
                self.itemChosing.TongCK_DichVu = DV_tongCK;
                self.itemChosing.TongTienDichVu_TruocCK = tongDV_truocCK;
                self.itemChosing.TongTienDichVu_TruocVAT = tongDV_truocVAT;

                self.itemChosing.TongSL_PhuTung = HH_tongSL;
                self.itemChosing.TongTienPhuTung = tongHH;
                self.itemChosing.TongThue_PhuTung = HH_tongthue;
                self.itemChosing.TongCK_PhuTung = HH_tongCK;
                self.itemChosing.TongTienPhuTung_TruocCK = tongHH_truocCK;
                self.itemChosing.TongTienPhuTung_TruocVAT = tongHH_truocVAT;

                // get nvthuchien, tuvan
                for (let i = 0; i < data.length; i++) {
                    let forIn = data[i];
                    let lstNV_TH = '';
                    let lstNV_TV = '';
                    if (forIn.BH_NhanVienThucHien != null && forIn.BH_NhanVienThucHien.length > 0) {
                        for (let j = 0; j < forIn.BH_NhanVienThucHien.length; j++) {
                            let itemFor = forIn.BH_NhanVienThucHien[j];
                            let isNVThucHien = itemFor.ThucHien_TuVan;
                            let tienCK = itemFor.TienChietKhau;
                            let gtriPtramCK = itemFor.PT_ChietKhau;
                            let isPTram = gtriPtramCK > 0 ? true : false;
                            let gtriCK_TH = 0;
                            let gtriCK_TV = 0;
                            let tacVu = 1;
                            let ckMacDinh = gtriPtramCK;
                            if (isNVThucHien) {
                                if (isPTram) {
                                    gtriCK_TH = gtriPtramCK;
                                    lstNV_TH += itemFor.TenNhanVien.concat(' (', gtriCK_TH, '%), ');
                                }
                                else {
                                    gtriCK_TH = tienCK;
                                    lstNV_TH += itemFor.TenNhanVien.concat(' (', formatNumber(tienCK), '), ');
                                    ckMacDinh = tienCK / itemFor.HeSo / forIn.SoLuong;
                                }
                            }
                            else {
                                tacVu = 2;
                                if (isPTram) {
                                    gtriCK_TV = gtriPtramCK;
                                    lstNV_TV += itemFor.TenNhanVien.concat(' (', gtriCK_TV, '%), ');
                                }
                                else {
                                    gtriCK_TV = tienCK;
                                    lstNV_TV += itemFor.TenNhanVien.concat(' (', formatNumber(tienCK), '), ');
                                    ckMacDinh = tienCK / itemFor.HeSo / forIn.SoLuong;
                                }
                            }

                            let idRandom = CreateIDRandom('CKNV_');
                            forIn.BH_NhanVienThucHien[j].IDRandom = idRandom;
                            forIn.BH_NhanVienThucHien[j].TacVu = tacVu;
                            forIn.BH_NhanVienThucHien[j].ChietKhauMacDinh = ckMacDinh;
                        }

                        data[i].HoaHongTruocChietKhau = forIn.BH_NhanVienThucHien[0].TinhHoaHongTruocCK;
                    }
                    // assign GhiChu_NVThucHien
                    data[i].GhiChu_NVThucHien = (lstNV_TH === '' ? '' : '- Thực hiện: ' + Remove_LastComma(lstNV_TH));
                    data[i].GhiChu_NVTuVan = (lstNV_TV === '' ? '' : '- Tư vấn: ' + Remove_LastComma(lstNV_TV));
                    data[i].GhiChu_NVThucHienPrint = (lstNV_TH === '' ? '' : Remove_LastComma(lstNV_TH));
                    data[i].GhiChu_NVTuVanPrint = (lstNV_TV === '' ? '' : Remove_LastComma(lstNV_TV));
                }
                self.listData.HoaDonChiTiets = data;
            })
        },
        GetInforHDPrint: async function () {
            let self = this;
            let objPrint = $.extend({}, self.listData.ThongTinXe);
            let hdChosing = self.itemChosing;
            let phaiThanhToan = formatNumberToInt(hdChosing.PhaiThanhToan);
            let daThanhToan = formatNumberToInt(hdChosing.KhachDaTra);
            let tongcong = hdChosing.TongThanhToan;
            let cus_DebitHD = hdChosing.PhaiThanhToan - hdChosing.KhachDaTra;

            let cus = await self.GetinforCus_byID(hdChosing.ID_DoiTuong);
            let baohiem = await self.GetinforCus_byID(hdChosing.ID_BaoHiem);

            let cus_NoHienTai = 0, bh_NoHienTai = 0;
            if (!$.isEmptyObject(cus)) {
                cus_NoHienTai = cus.NoHienTai;
            }
            if (!$.isEmptyObject(baohiem)) {
                bh_NoHienTai = baohiem.NoHienTai;
            }

            let cus_DebitOld = cus_NoHienTai - cus_DebitHD;
            cus_DebitOld = cus_DebitOld < 0 ? 0 : cus_DebitOld;
            let bh_DebitHD = hdChosing.PhaiThanhToanBaoHiem - hdChosing.BaoHiemDaTra;
            let bh_DebitOld = bh_NoHienTai - bh_DebitHD;
            bh_DebitOld = bh_DebitOld < 0 ? 0 : bh_DebitOld;

            objPrint.PhaiThanhToanBaoHiem = formatNumber(hdChosing.PhaiThanhToanBaoHiem);
            objPrint.KhachDaTra = formatNumber(hdChosing.KhachDaTra);
            objPrint.TongTienDichVu = formatNumber(hdChosing.TongTienDichVu);
            objPrint.TongTienPhuTung = formatNumber(hdChosing.TongTienPhuTung);
            objPrint.TongGiamGiaHang = formatNumber(hdChosing.TongGiamGiaHang);
            objPrint.TongTienPhuTung_TruocVAT = formatNumber(hdChosing.TongTienPhuTung_TruocVAT);
            objPrint.TongTienDichVu_TruocVAT = formatNumber(hdChosing.TongTienDichVu_TruocVAT);

            objPrint.TongTienPhuTung_TruocCK = formatNumber3Digit(hdChosing.TongTienPhuTung_TruocCK);
            objPrint.TongTienDichVu_TruocCK = formatNumber3Digit(hdChosing.TongTienDichVu_TruocCK);

            objPrint.TongThue_PhuTung = hdChosing.TongThue_PhuTung;
            objPrint.TongCK_PhuTung = hdChosing.TongCK_PhuTung;
            objPrint.TongThue_DichVu = hdChosing.TongThue_DichVu;
            objPrint.TongCK_DichVu = hdChosing.TongCK_DichVu;
            objPrint.TongSL_DichVu = hdChosing.TongSL_DichVu;
            objPrint.TongSL_PhuTung = hdChosing.TongSL_PhuTung;

            objPrint.DiaChiKhachHang = objPrint.DiaChi;
            objPrint.CoVan_SDT = self.Get_SDTCoVan();

            objPrint.TenBaoHiem = hdChosing.TenBaoHiem;
            objPrint.BH_SDT = hdChosing.DienThoaiBaoHiem;
            objPrint.BH_Email = hdChosing.BH_Email;
            objPrint.BH_DiaChi = hdChosing.BH_DiaChi;
            objPrint.BH_TenLienHe = hdChosing.LienHeBaoHiem;
            objPrint.BH_SDTLienHe = hdChosing.SoDienThoaiLienHeBaoHiem;
            objPrint.MaSoThue = hdChosing.MaSoThue;
            objPrint.TaiKhoanNganHang = hdChosing.TaiKhoanNganHang;

            objPrint.MaHoaDon = hdChosing.MaHoaDon;
            objPrint.MaHoaDonTraHang = hdChosing.MaBaoGia;
            objPrint.NgayLapHoaDon = moment(hdChosing.NgayLapHoaDon).format('DD/MM/YYYY HH:mm:ss');
            objPrint.NgayVaoXuong = moment(self.listData.ThongTinXe.NgayVaoXuong).format('DD/MM/YYYY HH:mm');
            objPrint.NgayXuatXuongDuKien = self.listData.ThongTinXe.NgayXuatXuongDuKien ?
                moment(self.listData.ThongTinXe.NgayXuatXuongDuKien).format('DD/MM/YYYY HH:mm') : '';
            objPrint.PTN_GhiChu = self.listData.ThongTinXe.GhiChu;

            objPrint.Ngay = moment(hdChosing.NgayLapHoaDon).format('DD');
            objPrint.Thang = moment(hdChosing.NgayLapHoaDon).format('MM');
            objPrint.Nam = moment(hdChosing.NgayLapHoaDon).format('YYYY');
            objPrint.DienGiai = hdChosing.DienGiai;

            objPrint.TongTienHang = formatNumber(hdChosing.TongTienHang);
            objPrint.TongGiamGia = formatNumber(hdChosing.TongGiamGia);
            objPrint.PhaiThanhToan = formatNumber(phaiThanhToan);
            objPrint.DaThanhToan = formatNumber(daThanhToan);
            objPrint.BaoHiemDaTra = formatNumber(hdChosing.BaoHiemDaTra);
            objPrint.TongTienBHDuyet = formatNumber(hdChosing.TongTienBHDuyet);
            objPrint.KhauTruTheoVu = formatNumber(hdChosing.KhauTruTheoVu);
            objPrint.SoVuBaoHiem = formatNumber(hdChosing.SoVuBaoHiem);
            objPrint.GiamTruBoiThuong = formatNumber(hdChosing.GiamTruBoiThuong);
            objPrint.BHThanhToanTruocThue = formatNumber(hdChosing.BHThanhToanTruocThue);
            objPrint.TongTienThueBaoHiem = formatNumber(hdChosing.TongTienThueBaoHiem);
            objPrint.TongTienThue = formatNumber(hdChosing.TongTienThue);
            objPrint.PTThueBaoHiem = formatNumber(hdChosing.PTThueBaoHiem);
            objPrint.PTThueHoaDon = formatNumber(hdChosing.PTThueHoaDon);
            objPrint.TongThueKhachHang = formatNumber(hdChosing.TongThueKhachHang);
            objPrint.TongChiPhi = formatNumber(hdChosing.TongChiPhi);
            objPrint.TongCong = formatNumber(tongcong);
            objPrint.TongThanhToan = formatNumber(tongcong);
            objPrint.BH_TienBangChu = DocSo(hdChosing.BaoHiemDaTra);
            objPrint.KH_TienBangChu = DocSo(hdChosing.KhachDaTra);
            objPrint.BH_ConThieu = formatNumber3Digit(hdChosing.PhaiThanhToanBaoHiem - hdChosing.BaoHiemDaTra);

            let conno = formatNumberToInt(hdChosing.TongThanhToan) - daThanhToan - hdChosing.BaoHiemDaTra;
            objPrint.NoTruoc = formatNumber3Digit(cus_DebitOld);
            objPrint.NoSau = formatNumber(cus_NoHienTai);
            objPrint.BH_NoTruoc = bh_DebitOld;
            objPrint.BH_NoSau = bh_NoHienTai;
            objPrint.TienBangChu = DocSo(tongcong);
            objPrint.TienKhachThieu = formatNumber(hdChosing.PhaiThanhToan - daThanhToan);
            objPrint.HD_ConThieu = conno;

            let mat = 0, pos = 0, ck = 0, tgt = 0, coc = 0, diem = 0;
            mat = hdChosing.Khach_TienMat + hdChosing.BH_TienMat;
            pos = hdChosing.Khach_TienPOS + hdChosing.BH_TienPOS;
            ck = hdChosing.Khach_TienCK + hdChosing.BH_TienCK;
            tgt = hdChosing.Khach_TheGiaTri + hdChosing.BH_TheGiaTri;
            coc = hdChosing.Khach_TienCoc + hdChosing.BH_TienCoc;
            diem = hdChosing.Khach_TienDiem + hdChosing.BH_TienDiem;

            let pthuc = '';
            if (mat > 0) {
                pthuc += 'Tiền mặt, ';
            }
            if (pos > 0) {
                pthuc += 'POS, ';
            }
            if (ck > 0) {
                pthuc += 'Chuyển khoản, ';
            }
            if (tgt > 0) {
                pthuc += 'Thẻ giá trị, ';
            }
            if (diem > 0) {
                pthuc += 'Điểm, ';
            }
            if (coc > 0) {
                pthuc += 'Tiền cọc, ';
            }
            objPrint.PhuongThucTT = Remove_LastComma(pthuc);
            objPrint.TienMat = mat;
            objPrint.TienATM = pos;
            objPrint.ChuyenKhoan = ck;
            objPrint.TienTheGiaTri = tgt;
            objPrint.TienDoiDiem = diem;

            // nvien laphd
            let nvienban = '';
            let nvien = $.grep(self.listData.NhanViens, function (x) {
                return x.ID === self.itemChosing.ID_NhanVien;
            });
            if (nvien.length > 0) {
                nvienban = nvien[0].TenNhanVien;
            }
            objPrint.NhanVienBanHang = nvienban;
            objPrint.TenChiNhanh = self.TenDonVi; // chi nhanh ban hang

            if (self.listData.CongTy.length > 0) {
                objPrint.LogoCuaHang = Open24FileManager.hostUrl + self.listData.CongTy[0].DiaChiNganHang;
                objPrint.TenCuaHang = self.listData.CongTy[0].TenCongTy;
                objPrint.DiaChiCuaHang = self.listData.CongTy[0].DiaChi;
                objPrint.DienThoaiCuaHang = self.listData.CongTy[0].SoDienThoai;
            }

            let cn = self.GetInforChiNhanh();
            objPrint.DiaChiChiNhanh = cn.DiaChiChiNhanh;
            objPrint.DienThoaiChiNhanh = cn.DienThoaiChiNhanh;
            return objPrint;
        },
        InHoaDon: async function (isPrintID, val) {
            var self = this;
            var hd = await self.GetInforHDPrint();
            var cthd = self.listData.HoaDonChiTiets;
            hd.TongGiamGia = formatNumber(self.itemChosing.TongGiamGia);
            hd.KhuyeMai_GiamGia = self.itemChosing.KhuyeMai_GiamGia;
            hd.TongTienHangChuaCK = formatNumber(self.itemChosing.TongTienHangChuaCK);
            hd.TongGiamGiaHD_HH = formatNumber(formatNumberToFloat(self.itemChosing.TongGiamGia)
                + formatNumberToFloat(self.itemChosing.KhuyeMai_GiamGia)
                + self.itemChosing.TongGiamGiaHang);

            var lstCT = [];
            for (let i = 0; i < cthd.length; i++) {
                let itFor = $.extend({}, true, cthd[i]);
                let price = formatNumberToInt(itFor.DonGia);
                let sale = formatNumberToInt(itFor.GiamGia);
                let giaban = formatNumberToInt(itFor.GiaBan);
                let bh_tt = itFor.SoLuong * formatNumberToFloat(itFor.DonGiaBaoHiem);
                thuocTinh = itFor.ThuocTinh_GiaTri;
                thuocTinh = thuocTinh === null || thuocTinh === '' ? '' : thuocTinh.substr(1);
                itFor.DonGia = formatNumber(price);
                itFor.TienChietKhau = formatNumber(sale);
                itFor.GiaBan = formatNumber(giaban);
                itFor.SoLuong = formatNumber3Digit(itFor.SoLuong);
                itFor.ThanhTien = formatNumber3Digit(itFor.ThanhTien);
                itFor.ThanhToan = formatNumber3Digit(itFor.ThanhToan);
                itFor.ThuocTinh_GiaTri = thuocTinh;
                itFor.ThanhTien = formatNumber3Digit(itFor.ThanhTien);
                itFor.ThuocTinh_GiaTri = thuocTinh;
                itFor.DonGiaBaoHiem = formatNumber3Digit(itFor.DonGiaBaoHiem);
                itFor.BH_ThanhTien = formatNumber3Digit(bh_tt);

                // nvthuchien, tuvan co in %ck 
                let th_CoCK = '';
                let tv_CoCK = '';
                if (itFor.BH_NhanVienThucHien != null) {
                    for (let j = 0; j < itFor.BH_NhanVienThucHien.length; j++) {
                        let for2 = itFor.BH_NhanVienThucHien[j];
                        if (for2.ThucHien_TuVan) {
                            th_CoCK += for2.TenNhanVien.concat(for2.PT_ChietKhau > 0 ? ' ('.concat(for2.PT_ChietKhau, ' %)') : ' ('.concat(formatNumber(for2.TienChietKhau), ')'), ', ');
                        }
                        else {
                            tv_CoCK += for2.TenNhanVien.concat(for2.PT_ChietKhau > 0 ? ' ('.concat(for2.PT_ChietKhau, ' %)') : ' ('.concat(formatNumber(for2.TienChietKhau), ')'), ', ');
                        }
                    }
                }
                itFor.NVThucHienDV_CoCK = Remove_LastComma(th_CoCK);
                itFor.NVTuVanDV_CoCK = Remove_LastComma(tv_CoCK);

                let combo = $.grep(vmThanhPhanCombo.AllComBo_ofHD, function (x) {
                    return x.ID_ParentCombo === itFor.ID_ParentCombo;
                });
                if (combo.length > 0) {
                    itFor.ThanhPhanComBo = combo;
                }
                else {
                    itFor.ThanhPhanComBo = [];
                }
                lstCT.push(itFor);
            }
            if (self.listData.HoaDonChiTiets.length > 0) {
                var url = '/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=' + val + '&idDonVi=' + self.ID_DonVi;
                if (isPrintID) {
                    url = '/api/DanhMuc/ThietLapApi/GetContentFIlePrint?idMauIn=' + val;
                }
                ajaxHelper(url, 'GET').done(function (result) {
                    var data = result;
                    data = data.concat('<script src="/Scripts/knockout-3.4.2.js"></script>');
                    data = data.concat("<script > var item1=" + JSON.stringify(lstCT)
                        + "; var item2=[];"
                        + " var item4 =", JSON.stringify(self.MauIn.ListData.HangMucSuaChua) + ";"
                    + " var item5 =", JSON.stringify(self.MauIn.ListData.VatDungKemTheo) + ";"
                    + " var item3=" + JSON.stringify(hd) + "; </script>");
                    data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                    PrintExtraReport(data);
                })
            }
        },

        ChangeCheck_AllItem: function (loaiHD = 25) {
            let self = this;
            let $this = $(event.currentTarget);
            let val = $this.is(':checked');
            self.loaiHoaDonPrint = loaiHD;

            if (val) {
                switch (loaiHD) {
                    case 3:
                        self.arrID_HDPrint = self.listData.BaoGias.map(function (x) {
                            return x.ID;
                        });
                        $('.baogiaxe .gara-section-body input[type=checkbox]').prop('checked', true);
                        $('.hoadonxe .gara-section-body input[type=checkbox]').prop('checked', false);
                        break;
                    case 25:
                        self.arrID_HDPrint = self.listData.HoaDons.map(function (x) {
                            return x.ID;
                        });
                        $('.baogiaxe .gara-section-body input[type=checkbox]').prop('checked', false);
                        $('.hoadonxe .gara-section-body input[type=checkbox]').prop('checked', true);
                        break;
                }
            }
            else {
                self.arrID_HDPrint = [];
                $('.baogiaxe .gara-section-list-body input[type=checkbox]').prop('checked', false);
                $('.hoadonxe .gara-section-list-body input[type=checkbox]').prop('checked', false);
            }
        },
        ChangeCheck_Item: function (item, loaiHD = 25) {
            let self = this;
            let $this = $(event.currentTarget);
            let val = $this.is(':checked');
            let idHoaDon = item.ID;
            self.loaiHoaDonPrint = loaiHD;

            let arrIDBG = self.listData.BaoGias.map(function (x) {
                return x.ID;
            });
            let arrIDHD = self.listData.HoaDons.map(function (x) {
                return x.ID;
            });

            if (val) {
                if (loaiHD === 3) {
                    self.arrID_HDPrint = $.grep(self.arrID_HDPrint, function (x) {
                        return $.inArray(x, arrIDHD) == -1;
                    })
                }
                else {
                    self.arrID_HDPrint = $.grep(self.arrID_HDPrint, function (x) {
                        return $.inArray(x, arrIDBG) == -1;
                    })
                }

                if ($.inArray(idHoaDon, self.arrID_HDPrint) === -1) {
                    self.arrID_HDPrint.push(idHoaDon);
                }
            }
            else {
                self.arrID_HDPrint = $.grep(self.arrID_HDPrint, function (x) {
                    return x !== idHoaDon;
                })
            }

            if (loaiHD === 3) {
                $('.baogiaxe .gara-section-list-header input[type=checkbox]').prop('checked', self.arrID_HDPrint.length === self.listData.BaoGias.length);
            }
            else {
                $('.hoadonxe .gara-section-list-header input[type=checkbox]').prop('checked', self.arrID_HDPrint.length === self.listData.HoaDons.length);
            }
        },

        PrintMerger: function (loaiHD = 25) {
            let self = this;
            let lstInvoice = self.listData.HoaDons;
            if (loaiHD === 3) {
                lstInvoice = self.listData.BaoGias;
            }

            let tongtienhang = 0, tongchiphi = 0, tongtienthue = 0, tongthueBH = 0, tongthueKH = 0, tonggiamgia = 0, tongthanhtoan = 0;
            let khachphaitra = 0, khachdatra = 0, tongBHDyet = 0, khautrutheovu = 0, chetai = 0, bhCanTratruocVAT = 0,
                bhCanTrasauVAT = 0, baohiemdatra = 0, diemgiaodich = 0, dathanhtoan = 0;
            let thutucoc = 0, tienmat = 0, chuyenkhoan = 0, pos = 0, tiendoidiem = 0, thegiatri = 0, conno = 0, khachConNo = 0, bhConNo = 0;
            let maHD = '', maBG = '', ngaylapHD = '', nguoitaoHD = '', tenNhanVien = '', diengiai = '', ptthueBH = 0,
                ptThueHD = 0, ptGiamGiaHD = 0, ptGiamTruBoiThuong = 0, masothue = '', tkNganHang = '';

            for (let i = 0; i < self.arrID_HDPrint.length; i++) {
                let hd = $.grep(lstInvoice, function (x) {
                    return x.ID === self.arrID_HDPrint[i];
                });
                if (hd.length > 0) {
                    maHD += hd[0].MaHoaDon + ', ';
                    maBG += hd[0].MaBaoGia + ', ';
                    nguoitaoHD = hd[0].NguoiTaoHD;
                    diengiai += hd[0].DienGiai + ' <br /> ';
                    tenNhanVien = hd[0].TenNhanVien;
                    ngaylapHD = hd[0].NgayLapHoaDon;
                    masothue = hd[0].MaSoThue;
                    tkNganHang = hd[0].TaiKhoanNganHang;

                    ptthueBH = hd[0].PTThueBaoHiem;
                    ptThueHD = hd[0].PTThueHoaDon;
                    ptGiamGiaHD = hd[0].TongChietKhau;
                    ptGiamTruBoiThuong = hd[0].PTGiamTruBoiThuong;

                    diemgiaodich += hd[0].DiemGiaoDich;
                    tongtienhang += hd[0].TongTienHang;
                    tongchiphi += hd[0].TongChiPhi;
                    tongtienthue += hd[0].TongTienThue;
                    tongthueBH += hd[0].TongTienThueBaoHiem;
                    tongthueKH += hd[0].TongThueKhachHang;
                    tonggiamgia += hd[0].TongGiamGia;
                    tongthanhtoan += hd[0].TongThanhToan;

                    khachphaitra += hd[0].PhaiThanhToan;
                    khachdatra += hd[0].KhachDaTra;
                    tongBHDyet += hd[0].TongTienBHDuyet;
                    khautrutheovu += hd[0].KhauTruTheoVu;
                    chetai += hd[0].GiamTruBoiThuong;
                    bhCanTratruocVAT += hd[0].BHThanhToanTruocThue;
                    bhCanTrasauVAT += hd[0].PhaiThanhToanBaoHiem;
                    baohiemdatra += hd[0].BaoHiemDaTra;

                    thutucoc += hd[0].Khach_TienCoc + hd[0].BH_TienCoc;
                    tienmat += hd[0].Khach_TienMat + hd[0].BH_TienMat;
                    chuyenkhoan += hd[0].Khach_TienCK + hd[0].BH_TienCK;
                    pos += hd[0].Khach_TienPOS + hd[0].BH_TienPOS;
                    tiendoidiem += hd[0].Khach_TienDiem + hd[0].BH_TienDiem;
                    thegiatri += hd[0].Khach_TheGiaTri + hd[0].BH_TheGiaTri;

                    khachConNo += (hd[0].PhaiThanhToan - hd[0].KhachDaTra);
                    bhConNo += (hd[0].PhaiThanhToanBaoHiem - hd[0].BaoHiemDaTra);
                }
            }

            conno = khachConNo + bhConNo;
            dathanhtoan = khachdatra + baohiemdatra;

            let ptn = self.listData.ThongTinXe;

            let objHD = {
                MaHoaDon: Remove_LastComma(maHD),
                MaHoaDonTraHang: Remove_LastComma(maBG),
                NgayLapHoaDon: moment(ngaylapHD).format('DD/MM/YYYY HH:mm:ss'),
                Ngay: moment(ngaylapHD).format('DD'),
                Thang: moment(ngaylapHD).format('MM'),
                Nam: moment(ngaylapHD).format('YYYY'),

                LoaiHoaDon: loaiHD,
                NguoiTao: nguoitaoHD,
                DienGiai: diengiai,
                NhanVienBanHang: tenNhanVien,
                TenGiaBan: 'Bảng giá chuẩn',

                ChoThanhToan: false,
                NgayApDungGoiDV: null,
                HanSuDungGoiDV: null,
                DiemGiaoDich: diemgiaodich,
                TongChiPhi: tongchiphi,

                TongTienHang: tongtienhang,
                PhaiThanhToan: khachphaitra,
                TongChietKhau: ptGiamGiaHD,
                TongGiamGia: tonggiamgia,
                DaThanhToan: dathanhtoan,
                KhachDaTra: khachdatra,
                BaoHiemDaTra: baohiemdatra,
                PTThueHoaDon: ptThueHD,
                TongTienThue: tongtienthue,
                PTThueBaoHiem: ptthueBH,
                TongTienThueBaoHiem: tongthueBH,
                PhaiThanhToanBaoHiem: bhCanTrasauVAT,
                TongThanhToan: tongthanhtoan,
                SoVuBaoHiem: '',
                KhauTruTheoVu: khautrutheovu,
                GiamTruBoiThuong: chetai,
                TongTienThueBaoHiem: tongthueBH,
                BHThanhToanTruocThue: bhCanTratruocVAT,
                TongTienBHDuyet: tongBHDyet,
                TongThueKhachHang: tongthueKH,
                TongCong: tongthanhtoan,

                PTThueKhachHang: 0,
                GiamTruThanhToanBaoHiem: 0,
                PTGiamTruBoiThuong: ptGiamTruBoiThuong,
                CongThucBaoHiem: 0,

                KhuyeMai_GiamGia: 0,
                TongGiamGiaKM_HD: tonggiamgia,
                TongTienTra: 0,
                TongTienMua: 0,
                TongGiaGocHangTra: 0,
                TongChiPhiHangTra: 0,
                HoanTraThuKhac: 0,
                DaTraKhach: 0,
                PhaiTraKhach: 0,

                TienMat: tienmat,
                TienATM: pos,
                TienGui: chuyenkhoan,
                TienTheGiaTri: thegiatri,
                TTBangDiem: tiendoidiem,
                DiemQuyDoi: 0,
                TienThua: 0,

                MaDoiTuong: ptn.MaDoiTuong,
                TenDoiTuong: ptn.TenDoiTuong,
                DienThoaiKhachHang: ptn.DienThoaiKhachHang,
                DiaChiKhachHang: ptn.DiaChi,
                MaPhieuTiepNhan: ptn.MaPhieuTiepNhan,
                TenBaoHiem: ptn.TenBaoHiem,
                BienSo: ptn.BienSo,
                NgayVaoXuong: moment(ptn.NgayVaoXuong).format('DD/MM/YYYY HH:mm'),
                NgayXuatXuongDuKien: ptn.NgayXuatXuongDuKien ? moment(ptn.NgayXuatXuongDuKien).format('DD/MM/YYYY HH:mm') : '',
                LienHeBaoHiem: ptn.LienHeBaoHiem,
                SoDienThoaiLienHeBaoHiem: ptn.SoDienThoaiLienHeBaoHiem,
                PTN_GhiChu: ptn.GhiChu,
                CoVan_SDT: self.Get_SDTCoVan(),
                CoVanDichVu: ptn.CoVanDichVu,
                NhanVienTiepNhan: ptn.NhanVienTiepNhan,
                SoKhung: ptn.SoKhung,
                SoMay: ptn.SoMay,
                HopSo: ptn.HopSo,
                DungTich: ptn.DungTich,
                NamSanXuat: ptn.NamSanXuat,
                MauSon: ptn.MauSon,
                TenLoaiXe: ptn.TenLoaiXe,
                TenMauXe: ptn.TenMauXe,
                TenHangXe: ptn.TenHangXe,
                ChuXe: ptn.ChuXe,
                MaSoThue: masothue,
                TaiKhoanNganHang: tkNganHang,

                BH_SDT: '',
                BH_Email: '',
                BH_DiaChi: '',
                BH_TenLienHe: '',
                BH_SDTLienHe: '',

                ChiPhi_GhiChu: '',
                PTChietKhauHH: 0,
                TongGiamGiaHang: 0,
                TongTienHangChuaCK: 0,
                TongTienKhuyenMai_CT: 0,
                TongGiamGiaKhuyenMai_CT: 0,
                SoDuDatCoc: 0,
                NoTruoc: 0,
                BH_NoTruoc: 0,
                BH_NoSau: 0,
                HD_ConThieu: conno,
                TienKhachThieu: khachConNo,
                BH_ConThieu: bhConNo,
            }

            let pthuc = '';
            if (tienmat > 0) {
                pthuc += 'Tiền mặt, ';
            }
            if (pos > 0) {
                pthuc += 'POS, ';
            }
            if (chuyenkhoan > 0) {
                pthuc += 'Chuyển khoản, ';
            }
            if (thegiatri > 0) {
                pthuc += 'Thẻ giá trị, ';
            }
            if (tiendoidiem > 0) {
                pthuc += 'Điểm, ';
            }
            if (thutucoc > 0) {
                pthuc += 'Tiền cọc, ';
            }
            objHD.PhuongThucTT = Remove_LastComma(pthuc);
            objHD.TienBangChu = DocSo(objHD.TongCong);
            objHD.KH_TienBangChu = DocSo(khachdatra);
            objHD.BH_TienBangChu = DocSo(baohiemdatra);

            ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'GetChiTietHD_MultipleHoaDon?arrID_HoaDon=' + self.arrID_HDPrint, 'GET').done(function (data) {
                if (data !== null) {

                    let sumSoLuong = 0, sumGiamGiaHang = 0, tongtienhang_truocCK = 0;
                    let cthdPrint = [];
                    for (let j = 0; j < data.length; j++) {
                        let ctFor = $.extend({}, true, data[j]);
                        ctFor.SoThuTu = j + 1;
                        ctFor.BH_ThanhTien = ctFor.SoLuong * ctFor.DonGiaBaoHiem;
                        ctFor.HH_ThueTong = ctFor.SoLuong * ctFor.TienThue;
                        ctFor.TongChietKhau = ctFor.SoLuong * ctFor.GiamGia;
                        ctFor.ThanhTienTruocCK = ctFor.SoLuong * ctFor.DonGia;
                        cthdPrint.push(ctFor);

                        sumSoLuong += ctFor.SoLuong;
                        sumGiamGiaHang += ctFor.TongChietKhau;
                        tongtienhang_truocCK += ctFor.ThanhTienTruocCK;
                    }
                    objHD.TongGiamGiaHang = sumGiamGiaHang;
                    objHD.TongTienHangChuaCK = tongtienhang_truocCK;
                    objHD.TongSoLuongHang = sumSoLuong;
                    objHD.TongGiamGiaHD_HH = sumGiamGiaHang + tonggiamgia;

                    let arrHH = data.filter(x => x.LaHangHoa);
                    let arrDV = data.filter(x => x.LaHangHoa === false);

                    let tongDV = 0, tongDV_truocVAT = 0, tongDV_truocCK = 0;
                    let tongHH = 0, tongHH_truocVAT = 0, tongHH_truocCK = 0;
                    let DV_tongthue = 0, DV_tongCK = 0, DV_tongSL = 0;
                    let HH_tongthue = 0, HH_tongCK = 0, HH_tongSL = 0;
                    for (let k = 0; k < arrHH.length; k++) {
                        let itFor = arrHH[k];
                        let soluong = formatNumberToFloat(itFor.SoLuong);
                        HH_tongSL += soluong;
                        tongHH += formatNumberToFloat(itFor.ThanhToan);
                        tongHH_truocVAT += formatNumberToFloat(itFor.ThanhTien);
                        tongHH_truocCK += soluong * formatNumberToFloat(itFor.DonGia);
                        HH_tongthue += soluong * formatNumberToFloat(itFor.TienThue);
                        HH_tongCK += soluong * formatNumberToFloat(itFor.TienChietKhau);
                    }
                    for (let k = 0; k < arrDV.length; k++) {
                        let itFor = arrDV[k];
                        let soluong = formatNumberToFloat(itFor.SoLuong);
                        DV_tongSL += soluong;
                        tongDV += formatNumberToFloat(itFor.ThanhToan);
                        tongDV_truocVAT += formatNumberToFloat(itFor.ThanhTien);
                        tongDV_truocCK += soluong * formatNumberToFloat(itFor.DonGia);
                        DV_tongthue += soluong * formatNumberToFloat(itFor.TienThue);
                        DV_tongCK += soluong * formatNumberToFloat(itFor.TienChietKhau);
                    }

                    objHD.TongSL_DichVu = DV_tongSL;
                    objHD.TongTienDichVu = tongDV;
                    objHD.TongThue_DichVu = DV_tongthue;
                    objHD.TongCK_DichVu = DV_tongCK;
                    objHD.TongTienDichVu_TruocCK = tongDV_truocCK;
                    objHD.TongTienDichVu_TruocVAT = tongDV_truocVAT;

                    objHD.TongSL_PhuTung = HH_tongSL;
                    objHD.TongTienPhuTung = tongHH;
                    objHD.TongThue_PhuTung = HH_tongthue;
                    objHD.TongCK_PhuTung = HH_tongCK;
                    objHD.TongTienPhuTung_TruocCK = tongHH_truocCK;
                    objHD.TongTienPhuTung_TruocVAT = tongHH_truocVAT;

                    console.log('cthdPrint', cthdPrint)
                    let maChungTu = 'HDBL';
                    if (loaiHD === 3) {
                        maChungTu = 'DH';
                    }
                    $.ajax({
                        url: '/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=' + maChungTu + '&idDonVi=' + VHeader.IdDonVi,
                        type: 'GET',
                        dataType: 'json',
                        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                        success: function (result) {
                            let data = result;
                            data = data.concat('<script src="/Scripts/knockout-3.4.2.js"></script>');
                            data = data.concat("<script > var item1=", JSON.stringify(cthdPrint)
                                , "; var item2= [] "
                                , "; var item3=", JSON.stringify(objHD)
                                , "; var item4 =", JSON.stringify(self.MauIn.ListData.HangMucSuaChua)
                                , "; var item5 =", JSON.stringify(self.MauIn.ListData.VatDungKemTheo)
                                , "; </script>");
                            data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>"); // MauInTeamplate.js: used to bind data in knockout
                            PrintExtraReport(data);
                        }
                    });
                }
            })
        },

        updateCar: function (id) {
            // get infor car
            var self = this;
            vmThemMoiXe.inforLogin = {
                ID_NhanVien: self.ID_NhanVien,
                ID_User: self.ID_User,
                UserLogin: self.UserLogin,
                ID_DonVi: self.ID_DonVi,
                TenNhanVien: self.getTenNhanVien_Login(),
            };
            window.open('#/DanhSachXe?' + self.listData.ThongTinXe.BienSo, '_blank');
        },
        updateCus: async function (id) {
            var self = this;
            let cus = await self.GetinforCus_byID(id);
            if (!$.isEmptyObject(cus)) {
                cus.TenTrangThai = '';
                cus.TenNguoiGioiThieu = cus.NguoiGioiThieu;
                cus.LoaiDoiTuong = 1;
                vmThemMoiKhach.showModalUpdate(cus);
                vmThemMoiKhach.inforLogin = {
                    ID_NhanVien: self.ID_NhanVien,
                    ID_User: self.ID_User,
                    UserLogin: self.UserLogin,
                    ID_DonVi: self.ID_DonVi,
                    TenNhanVien: self.getTenNhanVien_Login(),
                };
            }
        },
        NhapHang: function (item) {
            var self = this;
            let cthd = self.listData.HoaDonChiTiets;
            console.log(cthd)
            var arr = [];
            for (let i = 0; i < cthd.length; i++) {
                let forOut = cthd[i];
                switch (parseInt(forOut.LoaiHangHoa)) {
                    case 1:
                        arr.push(forOut);
                        break;
                    case 2:
                        if (!commonStatisJs.CheckNull(forOut.ThanhPhan_DinhLuong)) {
                            for (let j = 0; j < forOut.ThanhPhan_DinhLuong.length; j++) {
                                let forIn = forOut.ThanhPhan_DinhLuong[j];
                                arr.push(forIn);
                            }
                        }
                        break;
                    case 3:
                        let combo = $.grep(vmThanhPhanCombo.AllComBo_ofHD, function (x) {
                            return x.ID_ParentCombo === forOut.ID_ParentCombo;
                        });

                        if (combo.length > 0) {
                            for (let k = 0; k < combo.length; k++) {
                                let for1 = combo[k];
                                switch (parseInt(for1.LoaiHangHoa)) {
                                    case 1:
                                        arr.push(for1);
                                        break;
                                    case 2:
                                        if (!commonStatisJs.CheckNull(for1.ThanhPhan_DinhLuong)) {
                                            for (let j = 0; j < for1.ThanhPhan_DinhLuong.length; j++) {
                                                let forIn = for1.ThanhPhan_DinhLuong[j];
                                                arr.push(forIn);
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                        break;
                }
            }

            if (arr.length === 0) {
                commonStatisJs.ShowMessageDanger('Hóa đơn chỉ bao gồm dịch vụ. Không thể xuất kho');
                return;
            }

            let sum = arr.reduce(function (x, item) {
                return x + (item.SoLuong * item.GiaNhap);
            }, 0);

            let cthdLoHang = [], arrIDQuiDoi = [];
            for (let i = 0; i < arr.length; i++) {
                var ctNew = $.extend({}, arr[i]);
                delete ctNew["ID"];
                ctNew.TenDoiTuong = '';
                ctNew.TongTienHangChuaCK = 0;
                ctNew.TongGiamGiaHang = 0;
                ctNew.PTChietKhauHH = 0;
                ctNew.PTThueHD = 0;
                ctNew.TongGiamGia = 0;
                ctNew.TongTienThue = 0;
                ctNew.TongChietKhau = 0;
                ctNew.MaHoaDon = '';
                ctNew.DienGiai = item.DienGiai;
                ctNew.ID_DoiTuong = null;
                ctNew.NgayLapHoaDon = new Date();
                ctNew.ID_HoaDon = const_GuidEmpty;

                ctNew.TongTienHang = sum;
                ctNew.PhaiThanhToan = sum;
                ctNew.TongThanhToan = sum;
                ctNew.KhachDaTra = sum;
                ctNew.DaThanhToan = sum;
                ctNew.ID_NhanVien = item.ID_NhanVien;

                if (commonStatisJs.CheckNull(ctNew.ThuocTinh_GiaTri)) {
                    ctNew.ThuocTinh_GiaTri = '';
                }
                if (commonStatisJs.CheckNull(ctNew.DonViTinh)) {
                    ctNew.DonViTinh = [];
                }
                if (commonStatisJs.CheckNull(ctNew.TyLeChuyenDoi)) {
                    ctNew.TyLeChuyenDoi = 1;
                }
                if (commonStatisJs.CheckNull(ctNew.MaLoHang)) {
                    ctNew.MaLoHang = '';
                }

                let idLoHang = ctNew.ID_LoHang;
                let quanLiTheoLo = !commonStatisJs.CheckNull(ctNew.ID_LoHang);
                let ngaysx = ctNew.NgaySanXuat !== null ? moment(ctNew.NgaySanXuat).format('DD/MM/YYYY') : '';
                let hethan = ctNew.NgayHetHan !== null ? moment(ctNew.NgayHetHan).format('DD/MM/YYYY') : '';

                if (ngaysx === 'Invalid date') {
                    ngaysx = '';
                }
                if (hethan === 'Invalid date') {
                    hethan = '';
                }
                ctNew.NgaySanXuat = ngaysx;
                ctNew.NgayHetHan = hethan;

                ctNew.DM_LoHang = [];
                ctNew.ID_LoHang = idLoHang;
                ctNew.LotParent = quanLiTheoLo;
                ctNew.QuanLyTheoLoHang = quanLiTheoLo;
                ctNew.SoThuTu = cthdLoHang.length + 1;
                ctNew.HangCungLoais = [];
                ctNew.LaConCungLoai = false;
                ctNew.ID_ChiTietGoiDV = null;
                ctNew.ID_ChiTietDinhLuong = null;
                ctNew.DVTinhGiam = '%';
                ctNew.PTChietKhau = 0;
                ctNew.TienChietKhau = 0;
                ctNew.PTThue = 0;
                ctNew.TienThue = 0;
                ctNew.DonGia = ctNew.GiaNhap;
                ctNew.ThanhTien = ctNew.SoLuong * ctNew.GiaNhap;
                ctNew.ThanhToan = ctNew.ThanhTien;

                if ($.inArray(ctNew.ID_DonViQuiDoi, arrIDQuiDoi) === -1) {
                    arrIDQuiDoi.unshift(ctNew.ID_DonViQuiDoi);
                    ctNew.IDRandom = CreateIDRandom('CTHD_');
                    if (quanLiTheoLo) {
                        let objLot = $.extend({}, ctNew);
                        objLot.HangCungLoais = [];
                        objLot.DM_LoHang = [];
                        ctNew.DM_LoHang.push(objLot);
                    }
                    cthdLoHang.push(ctNew);
                }
                else {
                    // find in cthdLoHang with same ID_QuiDoi
                    for (let j = 0; j < cthdLoHang.length; j++) {
                        if (cthdLoHang[j].ID_DonViQuiDoi === ctNew.ID_DonViQuiDoi) {
                            if (quanLiTheoLo) {
                                let exLo = false;
                                for (let k = 0; k < cthdLoHang[j].DM_LoHang.length; k++) {
                                    let forLot = cthdLoHang[j].DM_LoHang[k];
                                    if (forLot.ID_LoHang === ctNew.ID_LoHang) {
                                        exLo = true;
                                        cthdLoHang[j].DM_LoHang[k].SoLuong = forLot.SoLuong + ctNew.SoLuong;
                                        cthdLoHang[j].DM_LoHang[k].ThanhTien = cthdLoHang[j].DM_LoHang[k].SoLuong * forLot.GiaNhap;
                                    }
                                }
                                if (!exLo) {
                                    let objLot = $.extend({}, ctNew);
                                    objLot.LotParent = false;
                                    objLot.HangCungLoais = [];
                                    objLot.DM_LoHang = [];
                                    objLot.IDRandom = CreateIDRandom('RandomCT_');
                                    cthdLoHang[j].DM_LoHang.push(objLot);
                                }
                            }
                            else {
                                cthdLoHang[j].SoLuong = cthdLoHang[j].SoLuong + ctNew.SoLuong;
                                cthdLoHang[j].ThanhTien = cthdLoHang[j].SoLuong * cthdLoHang[j].GiaNhap;
                            }
                            break;
                        }
                    }
                }
            }

            localStorage.setItem('lc_CTSaoChep', JSON.stringify(cthdLoHang));
            localStorage.setItem('typeCacheNhapHang', 3);
            self.GotoGara('', 1);
        },
        CapNhatChiPhi: function (item) {
            VueChiPhi.ShowModal(1);
        },
        //Start Đặt lịch
        ShowModalDanhSachDatLich: function () {
            vModalDatLich.ShowModalDanhSachDatLich();
        }
        //End Đặt lịch
    }
})
workTable.Gara_GetListPhieuTiepNhan(true);

$(function () {
    $('#TiepNhanXeModal').on('hidden.bs.modal', function () {
        if (vmTiepNhanXe.saveOK) {
            var phieuTN = vmTiepNhanXe.newPhieuTiepNhan;
            let mauxe = vmTiepNhanXe.carChosing.TenMauXe;
            if (commonStatisJs.CheckNull(mauxe)) {
                mauxe = '';
            }
            let hangxe = vmTiepNhanXe.carChosing.TenHangXe;
            if (commonStatisJs.CheckNull(hangxe)) {
                hangxe = '';
            }
            var thongtin = {
                ID: phieuTN.ID,
                ID_KhachHang: phieuTN.ID_KhachHang,
                ID_Xe: phieuTN.ID_Xe,
                ID_CoVanDichVu: phieuTN.ID_CoVanDichVu,
                ID_NhanVien: phieuTN.ID_NhanVien,
                MaPhieuTiepNhan: phieuTN.MaPhieuTiepNhan,
                NgayVaoXuong: phieuTN.NgayVaoXuong,
                NgayXuatXuongDuKien: phieuTN.NgayXuatXuongDuKien,
                SoKmVao: phieuTN.SoKmVao,
                SoKmRa: phieuTN.SoKmVao,
                TenLienHe: phieuTN.TenLienHe,
                SoDienThoaiLienHe: phieuTN.SoDienThoaiLienHe,
                GhiChu: phieuTN.GhiChu,
                BienSo: vmTiepNhanXe.carChosing.BienSo,
                SoKhung: vmTiepNhanXe.carChosing.SoKhung,
                SoMay: vmTiepNhanXe.carChosing.SoMay,
                HopSo: vmTiepNhanXe.carChosing.HopSo,
                DungTich: vmTiepNhanXe.carChosing.DungTich,
                NamSanXuat: vmTiepNhanXe.carChosing.NamSanXuat,
                MauSon: vmTiepNhanXe.carChosing.MauSon,
                TenLoaiXe: vmTiepNhanXe.carChosing.TenLoaiXe,
                TenMauXe: mauxe,
                TenHangXe: hangxe,
                NhanVienTiepNhan: vmTiepNhanXe.staffName,
                CoVanDichVu: vmTiepNhanXe.adviserName,
                TrangThai: phieuTN.TrangThai,
                LaChuXe: phieuTN.LaChuXe,
                ID_BaoHiem: phieuTN.ID_BaoHiem,
                NguoiLienHeBH: phieuTN.NguoiLienHeBH,
                SoDienThoaiLienHeBH: phieuTN.SoDienThoaiLienHeBH,

                // khachhang
                MaDoiTuong: vmTiepNhanXe.customerChosing.MaDoiTuong,
                TenDoiTuong: vmTiepNhanXe.customerChosing.TenDoiTuong,
                DienThoaiKhachHang: vmTiepNhanXe.customerChosing.DienThoai,
                Email: vmTiepNhanXe.customerChosing.Email,
                DiaChi: vmTiepNhanXe.customerChosing.DiaChi,
                // baohiem
                MaBaoHiem: vmTiepNhanXe.insurenceChosing.MaBaoHiem,
                TenBaoHiem: vmTiepNhanXe.insurenceChosing.TenBaoHiem,
                // chu xe
                ID_ChuXe: vmTiepNhanXe.ChuXe.ID,
                ChuXe: vmTiepNhanXe.ChuXe.TenDoiTuong,
                ChuXe_SDT: vmTiepNhanXe.ChuXe.DienThoai,
                ChuXe_Email: vmTiepNhanXe.ChuXe.DienThoai,
                ChuXe_DiaChi: vmTiepNhanXe.ChuXe.DiaChi,
            }

            if (vmTiepNhanXe.isNew === false) {
                for (let i = 0; i < workTable.listData.PhieuSuaChuas.length; i++) {
                    if (workTable.listData.PhieuSuaChuas[i].ID === phieuTN.ID) {
                        workTable.listData.PhieuSuaChuas.splice(i, 1);
                        break;
                    }
                }
            }
            workTable.isFirstLoad = true;
            workTable.ID_PhieuTiepNhan = phieuTN.ID;
            workTable.hideModalTiepNhanXe(thongtin);
            workTable.PhieuTiepNhan_GetThongTinChiTiet(thongtin);
            workTable.PhieuTiepNhan_GetTinhTrangXe();
            if (vmTiepNhanXe.isNew) {
                workTable.listData.BaoGias = [];
                workTable.listData.HoaDons = [];
            }
            else {
                // if change customer/ insurance
                let pnOld = vmTiepNhanXe.phieuTiepNhanOld;
                if (pnOld.ID_KhachHang !== phieuTN.ID_KhachHang || pnOld.ID_BaoHiem !== phieuTN.ID_BaoHiem) {
                    workTable.Gara_GetListHoaDonSuaChua();
                    workTable.Gara_GetListBaoGia();
                }
            }
        }
    })
    $('#XuatXuongModal').on('hidden.bs.modal', function () {
        if (vmXuatXuong.saveOK) {
            for (let i = 0; i < workTable.listData.PhieuSuaChuas.length; i++) {
                if (workTable.listData.PhieuSuaChuas[i].ID === vmXuatXuong.phieuXuat.ID) {
                    workTable.listData.PhieuSuaChuas.splice(i, 1);
                    break;
                }
            }

            workTable.isFirstLoad = true;
            if (workTable.listData.PhieuSuaChuas.length > 0) {
                workTable.ID_PhieuTiepNhan = workTable.listData.PhieuSuaChuas[0].ID;
                workTable.PhieuTiepNhan_GetThongTinChiTiet(workTable.listData.PhieuSuaChuas[0]);
                workTable.PhieuTiepNhan_GetTinhTrangXe();
                workTable.Gara_GetListBaoGia();
                workTable.Gara_GetListHoaDonSuaChua();
            }
            else {
                workTable.listData.HoaDons = [];
                workTable.listData.BaoGias = [];
            }
        }
    })

    $('#ThuTienHoaDonModal').on('hidden.bs.modal', function () {
        if (vmThanhToan.saveOK) {
            workTable.Gara_GetListHoaDonSuaChua();
            workTable.Gara_GetListBaoGia();
        }
    })
    $('#vmChiPhiHoaDon').on('hidden.bs.modal', function () {
        if (VueChiPhi.saveOK) {
            VueChiPhi.CTHD_GetChiPhiDichVu([workTable.itemChosing.ID]);
            workTable.Gara_GetListHoaDonSuaChua();
        }
    })

    $(window.document).on('shown.bs.modal', '.modal', function () {
        window.setTimeout(function () {
            $('[autofocus]', this).focus();
            $('[autofocus]').select();
        }.bind(this), 100);
    });
})