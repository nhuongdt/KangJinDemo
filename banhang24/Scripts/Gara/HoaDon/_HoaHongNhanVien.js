var vmHoaHongHoaDon = new Vue({
    el: '#HoaHongNhanVienHD',
    components: {
        'nvien-hoadon-search': cmpSearchNVDisscount,
    },
    data: {
        saveOK: false,
        isNew: true,
        IsShareDiscount: '2',
        LoaiChietKhauHD_NV: '2',
        RoleChange_ChietKhauNV: true,
        ID_DonVi: $('#txtDonVi').val(),
        itemChosing: {},
        inforPhieuThu: {},
        inforHoaDon: {
            ID: null,
            MaHoaDon: '',
            LoaiHoaDon: 1,
            TongThanhToan: 0,
            TongTienThue: 0,
            DaThuTruoc: 0,
            ThucThu: 0,
            ConNo: 0,
        },
        GridNVienBanGoi_Chosed: [],
        listData: {
            NhanViens: [],
            ChietKhauHoaDons: [],
        },
    },
    created: function () {
        var self = this;
        var idDonVi = self.ID_DonVi;
        if (commonStatisJs.CheckNull(idDonVi)) {
            self.ID_DonVi = VHeader.IdDonVi;

            self.RoleChange_ChietKhauNV = VHeader.Quyen.indexOf('HoaHong_ThayDoi') > -1;
        }
        self.GetChietKhauHoaDon_ByDonVi();
        console.log('ckHoaDon')
    },
    methods: {
        GetChietKhauHoaDon_byID: function (objHoaDon, objPhieuThu = null, isUpdate = true ) {
            var self = this;
            let idPhieuThu = null;
            self.inforPhieuThu = {};
            if (objPhieuThu != null) {
                idPhieuThu = objPhieuThu.ID;
                self.inforPhieuThu = objPhieuThu;
            }
            ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'GetChietKhauNV_byIDHoaDon?idHoaDon=' + objHoaDon.ID
                + '&idPhieuThu=' + idPhieuThu, 'GET').done(function (x) {
                    if (x.res === true) {
                        self.GridNVienBanGoi_Chosed = x.data;
                        for (let i = 0; i < self.GridNVienBanGoi_Chosed.length; i++) {
                            let itFor = self.GridNVienBanGoi_Chosed[i];
                            itFor.ChietKhauMacDinh = itFor.PT_ChietKhau;
                            switch (itFor.TinhChietKhauTheo) {
                                case 3:
                                    itFor.ChietKhauMacDinh = itFor.TienChietKhau / itFor.HeSo;
                                    break;
                            }
                        }
                    }
                    else {
                        self.GridNVienBanGoi_Chosed = [];
                    }

                    if (isUpdate) {
                        self.showModalUpdate(objHoaDon);
                    }
                });
        },
        GetChietKhauHoaDon_ByDonVi: function () {
            var self = this;
            let param = {
                ID_DonVi: self.ID_DonVi,
                CurrentPage: 0,
                PageSize: 0,
            }
            if (navigator.onLine) {
                ajaxHelper('/api/DanhMuc/NS_NhanVienAPI/' + 'Get_ChietKhauHoaDon_byDonVi', 'POST', param).done(function (x) {
                    if (x.res === true) {
                        self.listData.ChietKhauHoaDons = x.DataSoure;
                    }
                })
            }
        },
        showModal: function (item) {
            var self = this;
            self.saveOK = false;
            self.isNew = true;
            self.inforHoaDon = item;
            self.inforPhieuThu = {};
            $('#HoaHongNhanVienHD').modal('show');
        },
        showModalUpdate: function (item) {// used to update all ck nhanvien
            var self = this;
            self.IsShareDiscount = '2';
            self.saveOK = false;
            self.isNew = false;
            self.inforHoaDon = item;

            // check share discount
            var heso = 1;
            var lenNVHD = self.GridNVienBanGoi_Chosed.length;
            if (lenNVHD > 0) {
                heso = self.GridNVienBanGoi_Chosed[0].HeSo;
                let arrSameHS = $.grep(self.GridNVienBanGoi_Chosed, function (x) {
                    return x.HeSo === heso;
                });
                if (lenNVHD === arrSameHS.length && heso !== 1) {
                    self.IsShareDiscount = '1';
                }
            }

            // update chietkhaumacdinh
            for (let i = 0; i < lenNVHD; i++) {
                let itFor = self.GridNVienBanGoi_Chosed[i];
                itFor.ChietKhauMacDinh = itFor.PT_ChietKhau;
                switch (itFor.TinhChietKhauTheo) {
                    case 1:
                        itFor.TienChietKhau = itFor.PT_ChietKhau * self.inforHoaDon.ThucThu * itFor.HeSo / 100;
                        break;
                    case 3:
                        itFor.ChietKhauMacDinh = itFor.TienChietKhau / itFor.HeSo;
                        break;
                }
            }
            $('#HoaHongNhanVienHD').modal('show');
        },

        newNhanVien_ChietKhauHoaDon: function (itemCK, itemNV, exitChietKhau) {
            var self = this;
            var doanhThu = self.inforHoaDon.TongThanhToan - self.inforHoaDon.TongTienThue;
            var thucThu = self.inforHoaDon.ThucThu;
            if (exitChietKhau) {
                var tinhCKTheo = parseInt(itemCK.TinhChietKhauTheo);
                var valChietKhau = itemCK.GiaTriChietKhau;
                var tienCK_NV = 0; // used to assign in Grid
                var ptramCK = 0;
                switch (tinhCKTheo) {
                    case 1:
                        ptramCK = valChietKhau;
                        tienCK_NV = Math.round((valChietKhau / 100) * thucThu);
                        break;
                    case 2:
                        ptramCK = valChietKhau;
                        tienCK_NV = Math.round((valChietKhau / 100) * doanhThu);
                        break;
                    case 3:
                        tienCK_NV = valChietKhau;
                        break;
                }
                return {
                    ID_NhanVien: itemNV.ID,
                    ID_HoaDon: null,
                    MaNhanVien: itemNV.MaNhanVien,
                    TenNhanVien: itemNV.TenNhanVien,
                    ThucHien_TuVan: false,
                    TheoYeuCau: false,
                    HeSo: 1,
                    TinhChietKhauTheo: tinhCKTheo.toString(),
                    TienChietKhau: tienCK_NV,
                    PT_ChietKhau: ptramCK,
                    ChietKhauMacDinh: valChietKhau,
                }
            }
            else {
                return {
                    ID_NhanVien: itemNV.ID,
                    ID_HoaDon: null,
                    MaNhanVien: itemNV.MaNhanVien,
                    TenNhanVien: itemNV.TenNhanVien,
                    ThucHien_TuVan: false,
                    TheoYeuCau: false,
                    HeSo: 1,
                    TinhChietKhauTheo: '2',
                    TienChietKhau: 0,
                    PT_ChietKhau: 0,
                    ChietKhauMacDinh: 0,
                }
            }
        },

        Change_IsShareDiscount: function (x) {
            var self = this;
            self.HoaHongHD_UpdateHeSo_AndBind();
        },
        HoaHongHD_RemoveNhanVien: function (index) {
            var self = this;
            for (let i = 0; i < self.GridNVienBanGoi_Chosed.length; i++) {
                if (i === index) {
                    self.GridNVienBanGoi_Chosed.splice(i, 1);
                    break;
                }
            }
            self.HoaHongHD_UpdateHeSo_AndBind();
        },
        HoaHongHD_EditChietKhau: function () {
            var self = this;
            var item = self.itemChosing;
            var thisObj = event.currentTarget;
            var gtriNhap = formatNumberToFloat($(thisObj).val());
            var tinhCKTheo = parseInt(self.LoaiChietKhauHD_NV);
            var ptramCK = 0;
            if (tinhCKTheo === 3) {
                formatNumberObj($(thisObj))
            }
            else {
                if (gtriNhap > 100) {
                    $(thisObj).val(100);
                }
            }
            // get gtri after check % or VND
            var gtriCK_After = formatNumberToFloat($(thisObj).val());
            if (tinhCKTheo !== 3) {
                ptramCK = gtriCK_After;
            }
            var tienCK = self.CaculatorAgain_TienDuocNhan(gtriCK_After, item.HeSo, tinhCKTheo);
            for (let i = 0; i < self.GridNVienBanGoi_Chosed.length; i++) {
                if (i === item.Index) {
                    self.GridNVienBanGoi_Chosed[i].PT_ChietKhau = ptramCK;
                    self.GridNVienBanGoi_Chosed[i].TienChietKhau = tienCK;
                    self.GridNVienBanGoi_Chosed[i].ChietKhauMacDinh = gtriCK_After;
                    break;
                }
            }
        },
        AddNhanVien_BanGoi: function (item) {
            var self = this;
            var idNhanVien = item.ID;
            // check IDNhanVien exist in grid with same TacVu
            var itemEx = $.grep(self.GridNVienBanGoi_Chosed, function (x) {
                return x.ID_NhanVien === idNhanVien;
            });
            if (itemEx.length > 0) {
                ShowMessage_Danger('Nhân viên ' + itemEx[0].TenNhanVien + ' đã được chọn');
                return;
            }
            var loaiHD = self.inforHoaDon.LoaiHoaDon.toString();
            // get all ChietKhau HoaDon with LoaiHD
            var lstCK = $.grep(self.listData.ChietKhauHoaDons, function (x) {
                return x.ChungTuApDung.indexOf(loaiHD) > -1;
            })
            // remove ChungTu not apply LoaiHoaDon (ex: loaiHD= 1, but ChungTu contain 19
            var arrAfter = [];
            for (let i = 0; i < lstCK.length; i++) {
                var arrChungTu = lstCK[i].ChungTuApDung.split(',');
                if ($.inArray(loaiHD, arrChungTu) > -1) {
                    arrAfter.push(lstCK[i]);
                }
            }
            var exist = false;
            for (let i = 0; i < arrAfter.length; i++) {
                let itemOut = arrAfter[i];
                for (let j = 0; j < itemOut.NhanViens.length; j++) {
                    if (itemOut.NhanViens[j].ID === idNhanVien) {
                        let newObj = self.newNhanVien_ChietKhauHoaDon(itemOut, item, true);
                        newObj.ID_HoaDon = self.inforHoaDon.ID;
                        self.GridNVienBanGoi_Chosed.unshift(newObj);
                        exist = true;
                        break;
                    }
                }
            }
            if (exist === false) {
                let newObj = self.newNhanVien_ChietKhauHoaDon(null, item, false);
                newObj.ID_HoaDon = self.inforHoaDon.ID;
                self.GridNVienBanGoi_Chosed.push(newObj);
            }
            self.HoaHongHD_UpdateHeSo_AndBind();
        },

        HoaHongHD_UpdateHeSo_AndBind: function () {
            var self = this;
            var heso = 1;
            var lenGrid = self.GridNVienBanGoi_Chosed.length;
            if (self.IsShareDiscount === '1') {
                heso = RoundDecimal(1 / lenGrid, 2);
            }
            var thucthu = formatNumberToFloat(self.inforHoaDon.ThucThu);
            var doanhthu = formatNumberToFloat(self.inforHoaDon.TongThanhToan) - self.inforHoaDon.TongTienThue;
            for (let i = 0; i < self.GridNVienBanGoi_Chosed.length; i++) {
                let itemFor = self.GridNVienBanGoi_Chosed[i];
                let tinhCKTheo = parseInt(itemFor.TinhChietKhauTheo);
                let ptCK = itemFor.PT_ChietKhau;
                let tienCK = itemFor.TienChietKhau;
                switch (tinhCKTheo) {
                    case 1:
                        tienCK = Math.round(thucthu * ptCK / 100 * heso);
                        break;
                    case 2:
                        tienCK = Math.round(doanhthu * ptCK / 100 * heso);
                        break;
                    case 3:// vnd, keep heso =1
                        if (heso !== 1) {
                            tienCK = itemFor.ChietKhauMacDinh * heso;
                        }
                        else {
                            tienCK = itemFor.ChietKhauMacDinh / heso;
                        }
                        break;
                }
                self.GridNVienBanGoi_Chosed[i].HeSo = heso;
                self.GridNVienBanGoi_Chosed[i].TienChietKhau = tienCK;
            }
        },

        HoaHongHD_ShowDivChietKhau: function (item, index) {
            var self = this;
            var thisObj = $(event.currentTarget);
            if (self.RoleChange_ChietKhauNV === false) {
                ShowMessage_Danger('Không có quyền thay đổi chiết khấu nhân viên');
                return false;
            }
            var pos = thisObj.closest('td').position();
            $('#jsDiscount').show();
            $('#jsDiscount').css({
                left: (pos.left - 100) + "px",
                top: (pos.top + 31 + 37) + "px"
            });

            item.Index = index;
            self.itemChosing = item;
            var tinhCKTheo = parseInt(item.TinhChietKhauTheo);
            var gtriCK = 0;
            switch (tinhCKTheo) {
                case 1:
                    gtriCK = item.ChietKhauMacDinh;
                    break;
                case 2:
                    gtriCK = item.ChietKhauMacDinh;
                    break;
                case 3:
                    gtriCK = formatNumber(item.ChietKhauMacDinh);
                    break;
            }
            self.LoaiChietKhauHD_NV = tinhCKTheo.toString();
            $(function () {
                let inputNext = $('#jsDiscount').children('div').eq(0).find('input');
                $(inputNext).val(gtriCK);
                $(inputNext).focus().select();
            });
        },
        CaculatorAgain_TienDuocNhan: function (gtriCK, heso, tinhCKTheo) {
            var self = this;
            var doanhthu = self.inforHoaDon.TongThanhToan - self.inforHoaDon.TongTienThue;
            var thucthu = self.inforHoaDon.ThucThu;
            var tienCK = 0;
            switch (parseInt(tinhCKTheo)) {
                case 1:
                    tienCK = Math.round(thucthu * gtriCK / 100 * heso);
                    break;
                case 2:
                    tienCK = Math.round(doanhthu * gtriCK / 100 * heso);
                    break;
                case 3:
                    if (heso !== 1) {
                        tienCK = gtriCK * heso;
                    }
                    else {
                        tienCK = gtriCK / heso;
                    }
                    break;
            }
            return tienCK;
        },

        HoaHongHD_ChangeLoaiChietKhau: function (loaiCK) {
            var self = this;
            var item = self.itemChosing;
            var gtriCK = item.ChietKhauMacDinh;
            var ptramCK = gtriCK;
            var chietKhauMacDinh = 0;
            var doanhthu = formatNumberToFloat(self.inforHoaDon.TongThanhToan) - self.inforHoaDon.TongTienThue;
            var thucthu = formatNumberToFloat(self.inforHoaDon.ThucThu);
            var loaiCK_Old = parseInt(self.LoaiChietKhauHD_NV);
            if (loaiCK_Old === 3) {
                switch (loaiCK) {
                    case 1:// thuc thu
                        ptramCK = gtriCK = RoundDecimal(gtriCK / thucthu * 100);
                        chietKhauMacDinh = ptramCK;
                        break;
                    case 2:// doanh thu
                        ptramCK = gtriCK = RoundDecimal(gtriCK / doanhthu * 100);
                        chietKhauMacDinh = ptramCK;
                        break;
                    case 3:
                        ptramCK = 0;
                        break;
                }
            }
            else {
                switch (loaiCK) {
                    case 3:
                        if (loaiCK_Old === 1) {
                            gtriCK = Math.round(ptramCK * thucthu) / 100;
                        }
                        if (loaiCK_Old === 2) {
                            gtriCK = Math.round(ptramCK * doanhthu) / 100;
                        }
                        ptramCK = 0;
                        chietKhauMacDinh = gtriCK;
                        break;
                }
            }
            var tienCK = self.CaculatorAgain_TienDuocNhan(gtriCK, item.HeSo, loaiCK);
            for (let i = 0; i < self.GridNVienBanGoi_Chosed.length; i++) {
                if (i === item.Index) {
                    self.GridNVienBanGoi_Chosed[i].TinhChietKhauTheo = loaiCK.toString();
                    self.GridNVienBanGoi_Chosed[i].PT_ChietKhau = ptramCK;
                    self.GridNVienBanGoi_Chosed[i].TienChietKhau = tienCK;
                    if (chietKhauMacDinh !== 0 || (chietKhauMacDinh === 0 && tienCK === 0)) {
                        self.GridNVienBanGoi_Chosed[i].ChietKhauMacDinh = chietKhauMacDinh;
                    }
                    break;
                }
            }
            self.HoaHongHD_UpdateHeSo_AndBind();
            $(event.currentTarget).closest('div').prev().find('input').select().focus();
        },
        HoaHongHD_EditHeSo: function (item, index) {
            var self = this;
            var thisObj = event.currentTarget;
            var gtriCK = item.ChietKhauMacDinh;
            var heso = formatNumberToFloat($(thisObj).val());
            var tienCK = self.CaculatorAgain_TienDuocNhan(gtriCK, heso, item.TinhChietKhauTheo);
            for (let i = 0; i < self.GridNVienBanGoi_Chosed.length; i++) {
                if (index === i) {
                    self.GridNVienBanGoi_Chosed[i].TienChietKhau = tienCK;
                    break;
                }
            }
        },

        AgreeDiscount: function () {
            var self = this;
            self.saveOK = true;
            if (self.isNew === false) {
                self.UpdateChietKhauNVHoaDon_toDB();
            }
            else {
                $('#HoaHongNhanVienHD').modal('hide');
            }
        },

        UpdateChietKhauNVHoaDon_toDB: function () {
            var self = this;
            var lstNV = [];
            var nviens = '';
            for (let i = 0; i < self.GridNVienBanGoi_Chosed.length; i++) {
                let itFor = self.GridNVienBanGoi_Chosed[i];
                itFor.ID_HoaDon = self.inforHoaDon.ID;
                nviens += itFor.TenNhanVien + ', ';
                lstNV.push(itFor);
            }
            console.log('lstNV ', lstNV)

            var sLoaiHD = '';
            switch (self.inforHoaDon.LoaiHoaDon) {
                case 1:
                    sLoaiHD = 'Hóa đơn bán lẻ';
                    break;
                case 25:
                    sLoaiHD = 'Hóa đơn sửa chữa';
                    break;
                case 19:
                    sLoaiHD = 'Gói dịch vụ';
                    break;
                case 22:
                    sLoaiHD = 'Thẻ giá trị';
                    break;
            }

            if (!$.isEmptyObject(self.inforPhieuThu) && !commonStatisJs.CheckNull(self.inforPhieuThu.ID)) {
                // only update ckhoadon theo phieuthu lienquan
                let myData = {
                    objCT: self.GridNVienBanGoi_Chosed,
                    idthegiatri: self.inforHoaDon.ID,
                    idquyhoadon: self.inforPhieuThu.ID
                };

                ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/PostNhanVien_ThucHien', 'POST', myData).done(function (x) {
                    if (x.res) {
                        ShowMessage_Success('Cập nhật chiết khấu nhân viên thành công');
                        let diary = {
                            ID_DonVi: VHeader.IdDonVi,
                            ID_NhanVien: VHeader.IdNhanVien,
                            LoaiNhatKy: 2,
                            ChucNang: sLoaiHD + ' - Cập nhật chiết khấu nhân viên',
                            NoiDung: 'Cập nhật chiết khấu nhân viên cho '.concat(sLoaiHD.toLowerCase(), ' ', self.inforHoaDon.MaHoaDon),
                            NoiDungChiTiet: 'Cập nhật chiết khấu nhân viên cho '.concat(sLoaiHD.toLowerCase(), self.inforHoaDon.MaHoaDon,
                                ' gồm: ', nviens, ' <br /> Mã phiếu thu ', self.inforPhieuThu.MaHoaDon),
                        }
                        Insert_NhatKyThaoTac_1Param(diary);
                        $('#HoaHongNhanVienHD').modal('hide');
                    }
                    else {
                        ShowMessage_Danger(x.mes);
                    }
                });
            }
            else {
                // update all ck nhanvien theo hoadon
                ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/UpdateCKNhanVien_HoaDon?nv=' + lstNV + '&idHoaDon=' + self.inforHoaDon.ID, 'POST', lstNV).done(function (x) {
                    if (x.res) {
                        ShowMessage_Success('Cập nhật chiết khấu nhân viên thành công');
                        let diary = {
                            ID_DonVi: VHeader.IdDonVi,
                            ID_NhanVien: VHeader.IdNhanVien,
                            LoaiNhatKy: 2,
                            ChucNang: sLoaiHD + ' - Sửa đổi chiết khấu nhân viên',
                            NoiDung: 'Sửa đổi chiết khấu nhân viên cho '.concat(sLoaiHD.toLowerCase(), ' ', self.inforHoaDon.MaHoaDon),
                            NoiDungChiTiet: 'Sửa đổi chiết khấu nhân viên cho '.concat(sLoaiHD.toLowerCase(), ' ', self.inforHoaDon.MaHoaDon, ' gồm: ', nviens),
                        }
                        Insert_NhatKyThaoTac_1Param(diary);
                        $('#HoaHongNhanVienHD').modal('hide');
                    }
                    else {
                        ShowMessage_Danger(x.mes);
                    }
                });
            }
        },
    },
})
