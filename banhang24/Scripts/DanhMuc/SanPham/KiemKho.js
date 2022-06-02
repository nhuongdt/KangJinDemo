var Model_ProductDetail = function () {
    var self = this;
    self.ID_Random = ko.observable();
    self.ID_HangHoa = ko.observable();
    self.ID_DonViQuiDoi = ko.observable();
    self.ID_LoHang = ko.observable();
    self.MaHangHoa = ko.observable();
    self.TenHangHoa = ko.observable();
    self.ThuocTinh_GiaTri = ko.observable();
    self.SoLuong = ko.observable();
    self.ThanhTien = ko.observable();
    self.TienChietKhau = ko.observable();
    self.GiaVon = ko.observable();
    self.QuanLyTheoLoHang = ko.observable();
    self.LaHangHoa = ko.observable();
    self.MaLoHang = ko.observable();
    self.NgaySanXuat = ko.observable();
    self.NgayHetHan = ko.observable();
    self.DonViTinh = ko.observableArray();

    self.SetData = function (item) {
        self.ID_Random(item.ID_Random);
        self.ID_HangHoa(item.ID_HangHoa);
        self.ID_DonViQuiDoi(item.ID_DonViQuiDoi);
        self.ID_LoHang(item.ID_LoHang);
        self.MaHangHoa(item.MaHangHoa);
        self.TenHangHoa(item.TenHangHoa);
        self.ThuocTinh_GiaTri(item.ThuocTinh_GiaTri);
        self.SoLuong(item.SoLuong);
        self.ThanhTien(item.ThanhTien);
        self.TienChietKhau(item.TienChietKhau);
        self.GiaVon(item.GiaVon);
        self.QuanLyTheoLoHang(item.QuanLyTheoLoHang);
        self.LaHangHoa(item.LaHangHoa);
        self.MaLoHang(item.MaLoHang);
        self.NgaySanXuat(item.NgaySanXuat);
        self.NgayHetHan(item.NgayHetHan);
        self.DonViTinh = ko.observableArray(item.DonViTinh);
    }
}

var Model_StockInvoice = function () {
    var self = this;
    self.ID_Random = ko.observable();
    self.ID_DonVi = ko.observable();
    self.MaHoaDon = ko.observable();
    self.NgayLapHoaDon = ko.observable(moment(new Date()).format('YYYY-MM-DD HH:mm'));
    self.ID_NhanVien = ko.observable();
    self.TongGiamGia = ko.observable(); // tong chenhlech
    self.TongChiPhi = ko.observable(); // tong soluong lech tang
    self.TongTienHang = ko.observable(); // tong soluong lech giam
    self.TongSLThucTe = ko.observable(); // tong soluong thuc te
    self.NguoiTao = ko.observable();
    self.ChoThanhToan = ko.observable();
    self.DienGiai = ko.observable();

    self.SetData = function (item) {
        self.ID_Random(item.ID_Random);
        self.ID_DonVi(item.ID_DonVi);
        self.MaHoaDon(item.MaHoaDon);
        self.NgayLapHoaDon(item.NgayLapHoaDon);
        self.ID_NhanVien(item.ID_NhanVien);
        self.TongGiamGia(item.TongGiamGia);
        self.TongChiPhi(item.TongChiPhi);
        self.TongTienHang(item.TongTienHang);
        self.NguoiTao(item.NguoiTao);
        self.ChoThanhToan(item.ChoThanhToan);
        self.DienGiai(item.DienGiai);
    }
}

var FormModel_DetailStock = function () {
    var self = this;
    var _idDonVi = $('#hd_IDdDonVi').val();
    var _idNhanVien = $('.idnhanvien').text();
    var _lcStockDetail = 'lcStockDetail';

    self.IsNhapNhanh = ko.observable(false);
    self.IsPrint = ko.observable(false);
    self.SoLienIn = ko.observable(1);
    self.DonViTinhs = ko.observableArray();
    self.NhapNhanh_SoLuong = ko.observable(0);
    self.HHKiemKhos = ko.observableArray();
    self.selectedHH = ko.observable();
    self.NhomHangHoas = ko.observable();
    self.KiemGanDays = ko.observableArray();
    self.ItemChosing = ko.observable();
    self.HangHoaAfterAdd = ko.observableArray();
    self.newHoaDon = ko.observable(new Model_StockInvoice());
    self.newProduct = ko.observable(new Model_ProductDetail());

    self.JqAutoSelectItem = function (itemChose) {
        ajaxHelper(DMHangHoaUri + 'GetInforProduct_ByIDQuidoi?idQuiDoi=' + itemChose.ID_DonViQuiDoi
            + '&idChiNhanh=' + _idDonVi + '&idLoHang=' + itemChose.ID_LoHang).done(function (x) {
            if (x.res === true) {
                let lst = x.data;
                let item = [];
                if (itemChose.QuanLyTheoLoHang) {
                    // find in lst
                    let exItem = $.grep(lst, function (o) {
                        return o.ID_LoHang === itemChose.ID_LoHang;
                    });
                    if (exItem.length > 0) {
                        item = exItem[0];
                    }
                }
                else {
                    item = lst[0];
                }
                item.ID_HangHoa = item.ID;
                self.ItemChosing(item);

                if (self.IsNhapNhanh()) {
                    AddCTHD(item, 1);
                }
                else {
                    $('#txtSoLuongHang').focus();
                }
            }
        });
    }

    self.JqAutoSelect_Enter = function () {
        if (self.IsNhapNhanh()) {
            let mahh = $('#txtHangHoaauto').val();
            ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "GetHangHoa_ByMaHangHoa?mahh=" + mahh + '&iddonvi=' + _idDonVi, 'GET').done(function (data) {
                if (data.length > 0) {
                    data = data.filter(p => p.LaHangHoa === true);
                    if (data.length > 0) {
                        data[0].ID_HangHoa = data[0].ID;
                        self.ItemChosing(data[0]);
                        AddCTHD(data[0], 1);
                    }
                }
                else {
                    ShowMessage_Danger('Mã hàng không tồn tại');
                }
            });
        }
        else {
            $('#txtSoLuongHang').focus();
        }
    }

    function AddCTHD1(item, soluong) {
        self.HangHoaAfterAdd.push()
    }

    function AddCTHD(item, soluong) {
        var lstCT = localStorage.getItem(_lcStockDetail);
        if (lstCT !== null) {
            lstCT = JSON.parse(lstCT);
        }
        else {
            lstCT = [];
        }

        var itemEx = $.grep(lstCT, function (x) {
            return x.ID_DonViQuiDoi === item.ID_DonViQuiDoi;
        });
        if (itemEx.length > 0) {
            for (let i = 0; i < lstCT.length; i++) {
                if (lstCT[i].ID_DonViQuiDoi === item.ID_DonViQuiDoi && lstCT[i].ID_LoHang === item.ID_LoHang) {
                    lstCT[i].SoLuong = itFor.SoLuong + soluong;
                    lstCT[i].ThanhTien = lstCT[i].SoLuong * lstCT[i].GiaVon;
                    break;
                }
            }
        }
        else {
            let newCT = newCTHD(item, soluong);
            lstCT.unshift(newCT);
        }

        // update stt
        let stt = 0;
        for (let i = lstCT.length - 1; i >= 0; i--) {
            lstCT[i].SoThuTu = stt;
            stt = stt + 1;
        }
        lstCT = UpdateAgain_ListDVT(item.ID_HangHoa, lstCT);
        localStorage.setItem(_lcStockDetail, JSON.stringify(lstCT));

        self.HangHoaAfterAdd(lstCT);
    }

    function UpdateAgain_DonViTinhCTHD(idHangHoa, cthd) {
        let cthd_sameIDHangHoa = $.grep(cthd, function (x) {
            return x.ID_HangHoa === idHangHoa;
        });

        // get all dvt of this hanghoa
        var arrDVT = [];
        for (let i = 0; i < cthd_sameIDHangHoa.length; i++) {
            let itFor = cthd_sameIDHangHoa[i];
            if (arrDVT.filter(x => x.ID_DonViQuiDoi === itFor.ID_DonViQuiDoi).length === 0) {
                arrDVT.push({ ID_DonViQuiDoi: itFor.ID_DonViQuiDoi, TenDonViTinh: itFor.TenDonViTinh, Xoa: false });
            }

            for (let j = 0; j < itFor.DonViTinh.length; j++) {
                // check exist in arrDVT & push
                let itFor2 = itFor.DonViTinh[j];
                if (arrDVT.filter(x => x.ID_DonViQuiDoi === itFor2.ID_DonViQuiDoi).length === 0) {
                    arrDVT.push({ ID_DonViQuiDoi: itFor2.ID_DonViQuiDoi, TenDonViTinh: itFor2.TenDonViTinh, Xoa: itFor2.Xoa });
                }
            }
        }

        // update again lst DVT to cthd
        let find = 0;
        for (let i = 0; i < cthd.length; i++) {
            if (cthd[i].ID_HangHoa === idHangHoa) {
                // get arrQuiDoi exist
                let arrIDQuiDoi = [];
                let arrEx = $.grep(cthd_sameIDHangHoa, function (x) {
                    return x.ID_DonViQuiDoi !== cthd[i].ID_DonViQuiDoi;
                });
                for (let k = 0; k < arrEx.length; k++) {
                    arrIDQuiDoi.push(arrEx[k].ID_DonViQuiDoi);
                }
                cthd[i].DonViTinh = $.grep(arrDVT, function (x) {
                    return $.inArray(x.ID_DonViQuiDoi, arrIDQuiDoi) === -1;
                });
                find = find + 1;
                if (find === cthd_sameIDHangHoa.length) {
                    i = cthd.length;// esc for loop
                }
            }
        }
        return cthd;
    }

    function newCTHD(itemHH, soluong) {
        var lotParent = itemHH.QuanLyTheoLoHang ? true : false;
        var ngaysx = moment(itemHH.NgaySanXuat).format('DD/MM/YYYY');
        var hethan = moment(itemHH.NgayHetHan).format('DD/MM/YYYY');
        if (ngaysx === 'Invalid date') {
            ngaysx = '';
        }
        if (hethan === 'Invalid date') {
            hethan = '';
        }
        var tonkhoDB = parseFloat((itemHH.TonKho / itemHH.TyLeChuyenDoi).toFixed(3));
        return {
            ID: itemHH.ID_HangHoa,
            ID_HangHoa: itemHH.ID_HangHoa,
            SoThuTu: 1,
            ID_Random: CreateIDRandom('CTHD_'),
            ID_DonViQuiDoi: itemHH.ID_DonViQuiDoi,
            MaHangHoa: itemHH.MaHangHoa,
            TenHangHoa: itemHH.TenHangHoa,
            TenDonViTinh: itemHH.TenDonViTinh,
            DonViTinh: itemHH.DonViTinh,
            TyLeChuyenDoi: itemHH.TyLeChuyenDoi,
            ThuocTinh_GiaTri: itemHH.ThuocTinh_GiaTri,
            QuanLyTheoLoHang: itemHH.QuanLyTheoLoHang,
            GiaVon: itemHH.GiaVon,
            SoLuong: tonkhoDB,//SLThuc
            ThucTe: soluong,
            ThanhTien: soluong,
            TienChietKhau: tonkhoDB - soluong, //SLLech
            ThanhToan: (tonkhoDB - soluong) * itemHH.GiaVon , // GiaTriLech
            GhiChu: '',
            ID_LoHang: itemHH.ID_LoHang,
            MaLoHang: itemHH.MaLoHang,
            NgaySanXuat: ngaysx,
            NgayHetHan: hethan,
            DM_LoHang: [],
            LotParent: lotParent,
        }
    }

}

var modelDetailStock = new FormModel_DetailStock();
ko.applyBindings(modelDetailStock);

function jqAutoSelectItem(item) {
    modelDetailStock.JqAutoSelectItem(item);
}

function keypressEnterSelected(e) {
    if (e.keyCode === 13) {
        modelDetailStock.JqAutoSelect_Enter();
    }
}