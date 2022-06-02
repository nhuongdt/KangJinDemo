var vmNewProduct = new Vue({
    el: '#vmNewProduct',
    components: {

    },
    computed: {
        sLoaiHang: function () {
            let self = this;
            let txt = '';
            switch (self.newProduct.LoaiHangHoa) {
                case 1:
                    txt = 'hàng hóa';
                    break;
                case 2:
                    txt = 'dịch vụ';
                    break;
                case 3:
                    txt = 'combo';
                    break;
            }
            return txt;
        }
    },
    created: function () {
        let self = this;
        let idDonVi = $('#txtDonVi').val();
        if (commonStatisJs.CheckNull(idDonVi)) {
            self.inforLogin = {
                ID_DonVi: VHeader.IdDonVi,
                ID_NhanVien: VHeader.IdNhanVien,
                ID_User: VHeader.IdNguoiDung,
                UserLogin: VHeader.UserLogin,
            }
        }
        self.urlAPI = {
            HangHoa: '/api/DanhMuc/DM_HangHoaAPI/',
        }
    },
    data: {
        isNew: true,
        saveOK: false,
        isLoading: false,
        role: {
            HangHoa: {
                Insert: false,
                Update: false,
                Delete: false,
            },
            NhomHangHoa: {
                Insert: false,
                Update: false,
                Delete: false,
            },
        },
        newProduct: {
            ID: null,
            ID_Xe: null,
            ID_NhomHang: null,
            LaHangHoa: true,
            LoaiHangHoa: 1,
            MaHangHoa: '',
            TenHangHoa: '',
            GiaBan: '',
            GiaVon: '',
            TonKho: '',
            LoaiBaoHanh: 0,
            ThoiGianBaoHanh: 0,
            TheoDoi: true,
            DuocBanTrucTiep: true,
            QuanLyTheoLoHang: true,
            QuanLyBaoDuong: 0,
            DichVuTheoGio: 0,
            DuocTichDiem: 0,
            SoKmBaoHanh: 0,
            HoaHongTruocChietKhau: 0,
            HienThiDatLich: 0,
            IDViTris: '',

            QuanLyDonViTinh: false,
            QuanLyThuocTinh: false,
        },
        ListImg: [],
        DinhLuongDichVu: [],
        DonViTinhs: [
            {
                ID: null,
                MaHangHoa: '',
                ID_HangHoa: null,
                TenDonViTinh: '',
                TyLeChuyenDoi: 1,
                LaDonViChuan: true,
                GiaVon: 0,
                GiaNhap: 0,
                GiaBan: 0,
                GhiChu: '',
                Xoa: false,
                ThuocTinhGiaTri: '',
            },
        ]
    },
    methods: {
        GetInforProduct_byID: function (idHangHoa, type = 0) {// type (1.show modal, 0.only get)
            let self = this;
            if (!commonStatisJs.CheckNull(idHangHoa)) {

                ajaxHelper(self.urlAPI.HangHoa + "GetDM_HangHoa?id=" + idHangHoa + '&iddonvi='
                    + self.inforLogin.ID_DonVi, 'GET').done(function (data) {
                        console.log(data);
                        self.newProduct = data;

                        if (data.HangHoa_ThuocTinh.length > 0) {
                            self.newProduct.QuanLyThuocTinh = true;
                        }
                        else {
                            self.newProduct.QuanLyThuocTinh = false;
                        }

                        if (!commonStatisJs.CheckNull(data.DonViTinhChuan)) {
                            self.newProduct.QuanLyDonViTinh = true;
                        }
                        else {
                            self.newProduct.QuanLyDonViTinh = false;
                        }

                        if (data.DinhLuongDichVu.length > 0) {
                            self.DinhLuongDichVu = data.DinhLuongDichVu;
                        }
                        else {
                            self.DinhLuongDichVu = [];
                        }

                        if (type === 1) {
                            $('#vmNewProduct').modal('show');
                        }
                    })
            }

        },
        showInsert: function () {
            let self = this;
            self.saveOK = false;
            self.isLoading = false;

            self.newProduct = {
                ID: null,
                ID_Xe: null,
                ID_NhomHang: null,
                LaHangHoa: true,
                LoaiHangHoa: 1,
                MaHangHoa: '',
                TenHangHoa: '',
                GiaBan: '',
                GiaVon: '',
                TonKho: '',
                LoaiBaoHanh: 0,
                ThoiGianBaoHanh: 0,
                TheoDoi: true,
                DuocBanTrucTiep: true,
                QuanLyTheoLoHang: true,
                QuanLyBaoDuong: 0,
                DichVuTheoGio: 0,
                DuocTichDiem: 0,
                SoKmBaoHanh: 0,
                HoaHongTruocChietKhau: 0,
                HienThiDatLich: 0,
                IDViTris: '',
                QuanLyDonViTinh: false,
            };

            $('#vmNewProduct').modal('show');
        },
        showUpdate: function (item) {
            let self = this;
            self.saveOK = false;
            self.isLoading = false;
            $('#vmNewProduct').modal('show');
        },
    }
})