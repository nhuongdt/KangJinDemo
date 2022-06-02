var NewProduct = function () {
    var self = this;
    self.MaHangHoa = ko.observable();
    self.TenHangHoa = ko.observable();
    self.ID_NhomHang = ko.observable();
    self.GiaBan = ko.observable();
    self.GiaVon = ko.observable();
    self.TonKho = ko.observable();
    self.TenDonViTinh = ko.observable();
    self.LaHangHoa = ko.observable(true);
    self.QuanLyTheoLoHang = ko.observable(false);
}

var NewProductGroup = function () {
    var self = this;
    self.TenNhomHangHoa = ko.observable();
    self.LaNhomHangHoa = ko.observable();
    self.ID_Parent = ko.observable();
}

var PartialAddProduct = function () {
    var self = this;
    self.ID_NhanVien = ko.observable();
    self.ID_DonVi = ko.observable();
    self.UserLogin = ko.observable();
    self.AddProductSuccess = ko.observable(false);
    self.filterGroup = ko.observable();
    self.NhomHangHoas = ko.observableArray();
    self.NhomHangHoas_ByKind = ko.observableArray();
    self.ProductAfterAdd = ko.observable();
    self.newProduct = ko.observable(new NewProduct());
    self.newProductGroup = ko.observable(new NewProductGroup());

    function EnableBtnAddProduct() {
        document.getElementById("btnLuuHangHoa").disabled = false;
        document.getElementById("btnLuuHangHoa").lastChild.data = " Lưu";
    }

    self.ChangeLaHangHoa = function (x) {
        if (self.newProduct().LaHangHoa()) {
            $('#divgiavon').css('display', '');
            $('#divtonkho').css('display', '');
            $('#title-partialproduct').text('Thêm mới hàng hóa');
        }
        else {
            $('#divgiavon').css('display', 'none');
            $('#divtonkho').css('display', 'none');
            $('#title-partialproduct').text('Thêm mới dịch vụ');
        }
        var arr = $.grep(self.NhomHangHoas(), function (x) {
            return x.LaNhomHangHoa === self.newProduct().LaHangHoa();
        });
        self.NhomHangHoas_ByKind(arr);
    }

    self.ChoseGroup = function (item) {
        var obj = event.currentTarget;
        let idChosed = const_GuidEmpty;
        if (item.id === undefined) {
            $('.tenNhomHH').text('---Chọn nhóm---');
        }
        else {
            idChosed = item.id;
            $('.tenNhomHH').text(item.text);
            $(obj).closest('.group-parent').find('span').empty();
            $(obj).children('span').append(element_appendCheck);
        }
        self.newProduct().ID_NhomHang(idChosed);
        self.newProductGroup().ID_Parent(idChosed);
    }

    self.arrFilterGroup = ko.computed(function () {
        var _filter = locdau(self.filterGroup());

        return arrFilter = ko.utils.arrayFilter(self.NhomHangHoas_ByKind(), function (prod) {
            var chon = true;
            var name = locdau(prod.text);
            var sSearch = '';

            // find in children
            for (let i = 0; i < prod.children.length; i++) {
                sSearch += locdau(prod.children[i].text);
            }
            if (chon && _filter) {
                chon = name.indexOf(_filter) > -1 || sSearch.indexOf(_filter) > -1;
            }
            return chon;
        });
    });

    self.saveProduct = function () {
        if (navigator.onLine) {
            document.getElementById("btnLuuHangHoa").disabled = true;
            document.getElementById("btnLuuHangHoa").lastChild.data = "Đang lưu";

            ajaxHelper('/api/DanhMuc/DM_HangHoaAPI/' + 'GioiHanSoMatHang', 'GET').done(function (data) {
                if (data) {
                    ShowMessage_Danger('Cửa hàng đã đạt số mặt hàng quy định, không thể thêm mới');
                    return false;
                }
                else {
                    let code = self.newProduct().MaHangHoa();
                    let name = self.newProduct().TenHangHoa();
                    let dvt = self.newProduct().TenDonViTinh();

                    if (code !== undefined) {
                        if (Remove_LastComma(code) === '') {
                            code = '';
                        }
                    }

                    var sLoai = 'hàng hóa';
                    if (self.newProduct().LaHangHoa() === false) {
                        sLoai = 'dịch vụ';
                    }
                    if (name === undefined || (name !== undefined && Remove_LastComma(name) === '')) {
                        ShowMessage_Danger('Vui lòng nhập tên ' + sLoai);
                        EnableBtnAddProduct();
                        return false;
                    }
                    if (dvt === undefined) {
                        dvt = '';
                    }

                    var price = self.newProduct().GiaBan();
                    var costprice = self.newProduct().GiaVon();
                    var inventory = self.newProduct().TonKho();
                    var idNhomHang = self.newProduct().ID_NhomHang();
                    if (price === undefined || locdau(price) === '') {
                        price = 0;
                    }
                    if (costprice === undefined || locdau(costprice) === '') {
                        costprice = 0;
                    }
                    if (inventory === undefined || locdau(inventory) === '') {
                        inventory = 0;
                    }
                    if (idNhomHang === null || idNhomHang === undefined) {
                        if (self.newProduct().LaHangHoa()) {
                            idNhomHang = const_GuidEmpty;
                        }
                        else {
                            idNhomHang = '00000000-0000-0000-0000-000000000001'
                        }
                    }

                    let DM_HangHoa = {
                        MaHangHoa: code,
                        TenHangHoa: name,
                        TenHangHoa_KhongDau: locdau(name),
                        TenHangHoa_KyTuDau: GetChartStart(name),
                        GiaBan: price,
                        GiaVon: costprice,
                        TonKho: inventory,
                        ID_NhomHang: idNhomHang,
                        ID_NhanVien: self.ID_NhanVien(),
                        ID_DonVi: self.ID_DonVi(),
                        NguoiTao: self.UserLogin(),
                        DuocBanTrucTiep: true,
                        QuanLyTheoLoHang: self.newProduct().QuanLyTheoLoHang(),
                        LaHangHoa: self.newProduct().LaHangHoa(),
                        TenDonViTinh: dvt,
                        DonViTinh: [],
                    };
                    console.log(DM_HangHoa);
                    ajaxHelper('/api/DanhMuc/DM_HangHoaAPI/' + 'PostDM_HangHoaDV', 'POST', { objNew: DM_HangHoa }).done(function (x) {
                        if (x.res === true) {
                            self.AddProductSuccess(true);

                            let obj = x.data;
                            DM_HangHoa.ID = obj.ID;
                            DM_HangHoa.MaHangHoa = obj.MaHangHoa;
                            DM_HangHoa.ID_DonViQuiDoi = obj.ID_DonViQuiDoi;
                            DM_HangHoa.Name = obj.MaHangHoa.toLowerCase().concat(DM_HangHoa.TenHangHoa, ' ', DM_HangHoa.TenHangHoa_KhongDau,
                                ' ', DM_HangHoa.TenHangHoa_KyTuDau);
                            DM_HangHoa.ID_NhomHangHoa = DM_HangHoa.ID_NhomHang;
                            DM_HangHoa.ThuocTinh_GiaTri = '';
                            DM_HangHoa.PhiDichVu = 0;
                            DM_HangHoa.LaPTPhiDichVu = '';
                            DM_HangHoa.HoaHongTruocChietKhau = 0;
                            DM_HangHoa.SoPhutThucHien = 0;
                            DM_HangHoa.GhiChuHH = '';
                            DM_HangHoa.MaLoHang = '';
                            DM_HangHoa.NgaySanXuat = '';
                            DM_HangHoa.NgayHetHan = '';
                            DM_HangHoa.ID_LoHang = null;
                            DM_HangHoa.LaDonViChuan = true;
                            DM_HangHoa.TyLeChuyenDoi = 1;
                            DM_HangHoa.QuyCach = 1;
                            DM_HangHoa.ThoiGianBaoHanh = 0;
                            DM_HangHoa.ID_ChiTietGoiDV = null;
                            DM_HangHoa.SoGoiDV = 0;
                            DM_HangHoa.HanSuDungGoiDV_Min = moment(new Date()).format('YYYYMMDD');
                            DM_HangHoa.CssImg = '';
                            DM_HangHoa.SrcImage = '/Content/images/iconbepp18.9/gg-37.png';
                            DM_HangHoa.BackgroundColor = getRandomColor_Temp(0, 7);
                            DM_HangHoa.LoaiHangHoa = DM_HangHoa.LaHangHoa ? 1 : 2;

                            self.ProductAfterAdd(DM_HangHoa);
                            ShowMessage_Success('Thêm mới ' + sLoai + ' thành công');
                            $('#partialAddProduct').modal('hide');
                        }
                        else {
                            ShowMessage_Danger(x.mes);
                        }
                        EnableBtnAddProduct();
                    });
                }
            });
        }
        else {
            ShowMessage_Danger('Không có kết nối internet. Vui lòng thử lại sau');
        }
    }

    self.ShowPopupAddGroup = function () {
        $('#partialGroupProduct').modal('show');
        if (self.newProduct().LaHangHoa()) {
            $('#tilte-partialNhomHH').text('Thêm nhóm hàng hóa');
        }
        else {
            $('#tilte-partialNhomHH').text('Thêm nhóm dịch vụ');
        }
        self.newProductGroup(new NewProductGroup());
        $('#partialGroupProduct .tenNhomHH').text('---Chọn nhóm---');
    }

    self.saveProductGroup = function () {
        var name = self.newProductGroup().TenNhomHangHoa();
        var idParent = self.newProductGroup().ID_Parent();
        var sLoai = 'hàng hóa';
        if (self.newProduct().LaHangHoa() === false) {
            sLoai = 'dịch vụ';
        }
        if (name === undefined || (name !== undefined && Remove_LastComma(name) === '')) {
            ShowMessage_Danger('Vui lòng nhập tên nhóm ' + sLoai);
            EnableBtnAddProduct();
            return false;
        }
        var objNhomHH = {
            TenNhomHangHoa: name,
            LaNhomHangHoa: self.newProduct().LaHangHoa(),
            ID_Parent: idParent,
            NguoiTao: self.UserLogin(),
        };
        ajaxHelper('/api/DanhMuc/DM_NhomHangHoaAPI/PostDM_NhomHangHoa', 'POST', objNhomHH).done(function (x) {
            objNhomHH.ID = x.ID;
            $('.tenNhomHH').text(name);
            self.newProduct().ID_NhomHang(x.ID);

            var objNhatKy = {
                LoaiNhatKy: 1,
                ID_DonVi: self.ID_DonVi(),
                ID_NhanVien: self.ID_NhanVien(),
                ChucNang: 'Thêm nhóm ' + sLoai,
                NoiDung: 'Thêm nhóm '.concat(sLoai, ' : tên nhóm: ', name),
                NoiDungChiTiet: 'Thêm nhóm '.concat(sLoai, ' ', name, '. Người tạo: ', self.UserLogin()),
            };
            Insert_NhatKyThaoTac_1Param(objNhatKy);
            $('#partialGroupProduct').modal('hide');
            ShowMessage_Success('Thêm mới nhóm ' + sLoai + ' thành công');
        })
    }
}