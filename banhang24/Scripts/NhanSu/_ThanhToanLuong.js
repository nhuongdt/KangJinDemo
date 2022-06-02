var vmThanhToanLuong = new Vue({
    el: '#modalCashSalary',
    data: {
        ID_DonVi: $('#hd_IDdDonVi').val(),
        UserLogin: $('#txtTenTaiKhoan').text().trim(),
        ID_NhanVien: $('.idnhanvien').text().trim(),
        CurrentPage: 0,
        PageSize: 100,
        loading: false,
        defaultValue: {
            ID_KhoanThuChi: '0',
            ID_NhanVien: $('.idnhanvien').text().trim(),
        },
        newPhieuChi: {
            ID_KhoanThuChi: null,
            ID_NhanVien: $('.idnhanvien').text().trim(),
            NgayLapHoaDon: moment(new Date()).format('DD/MM/YYYY HH:mm'),
            NguoiNopTien: '', // tennhanvien of bangluongct
            TongTienThu: 0,
            NoiDungThu: '',
            TienMat: 0,
            TienGui: 0,
            ID_TaiKhoanNganHang: null,
            ID_NganHang: null,
            HachToanKinhDoanh: true,
            PhieuDieuChinhCongNo: 0,

            ID_BangLuong: null,
            MaBangLuong: '',
            TongThanhToan: 0,
            TongLuongNhan: 0,
            TongDaTra: 0,
            TongCanTra: 0,
            TienThua: 0,
            TenTaiKhoanChosing: '-- Chọn tài khoản --',
            LblTienThua: 'Tiền thừa',
        },

        listdata: {
            BangLuongChiTiet: [],
            KhoanChi: [],
            NhanVienLapPhieu: [],
            TaiKhoanNganHang: [],
        },
        Role: {
            Insert: false,
            Update: false,
            Delete: false,
            NhanSu: false,
        },
    },
    methods: {
        GetData: function () {
            var self = this;
            $.getJSON("/api/DanhMuc/NS_NhanVienAPI/GetNS_NhanVien_InforBasic?idDonVi=" + self.ID_DonVi, function (result) {
                self.listdata.NhanVienLapPhieu = result;
            });

            $.getJSON("/api/DanhMuc/Quy_HoaDonAPI/GetQuy_KhoanThuChi", function (result) {
                if (result.res) {
                    let khoanchi = $.grep(result.data, function (x) {
                        return x.LaKhoanThu === false && x.TinhLuong === false;
                    });

                    let obj = {
                        ID: '0',
                        NoiDungThuChi: '-- Chọn tài khoản --',
                    }
                    khoanchi.unshift(obj);
                    self.listdata.KhoanChi = khoanchi;
                }
            });

            $.getJSON("/api/DanhMuc/Quy_HoaDonAPI/GetAllTaiKhoanNganHang_ByDonVi?idDonVi=" + self.ID_DonVi, function (result) {
                if (result.res) {
                    let tkCK = $.grep(result.data, function (x) {
                        return x.TaiKhoanPOS === false;
                    });
                    self.listdata.TaiKhoanNganHang = tkCK;
                }
            });
        },
        ModalShow: function (idBangLuong, mabangluong) {
            var self = this;
            self.newPhieuChi.ID_BangLuong = idBangLuong;
            self.newPhieuChi.MaBangLuong = mabangluong;
            self.newPhieuChi.ID_NhanVien = self.defaultValue.ID_NhanVien;
            self.newPhieuChi.ID_KhoanThuChi = null;
            self.newPhieuChi.ID_TaiKhoanNganHang = null;
            self.newPhieuChi.NoiDungThu = '';
            self.newPhieuChi.NgayLapHoaDon = moment(new Date()).format('DD/MM/YYYY HH:mm');
            vmThanhToanLuong.ChoseAccountCK(self);

            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetListDebitSalaryDetail?idBangLuong=" + idBangLuong + "&textSearch=''" +
                '&currentPage=' + self.CurrentPage + '&pageSize=' + self.PageSize, function (data) {
                    console.log(data)
                    if (data.res) {
                        self.listdata.BangLuongChiTiet = data.dataSoure;
                        if (data.dataSoure.length > 0) {
                            let itFrist = data.dataSoure[0];
                            self.newPhieuChi.TongCanTra = itFrist.TongCanTra;
                            self.newPhieuChi.TongLuongNhan = itFrist.TongLuongNhan;
                            self.newPhieuChi.TongDaTra = itFrist.TongDaTra;
                            self.newPhieuChi.TongTamUng = itFrist.TongTamUng;
                            self.newPhieuChi.TongTruTamUngThucTe = itFrist.TongTruTamUngThucTe;
                            self.newPhieuChi.TongCanTraSauTamUng = itFrist.TongCanTraSauTamUng;
                            self.newPhieuChi.TienMat = commonStatisJs.FormatNumber3Digit(itFrist.TongCanTraSauTamUng);
                            self.newPhieuChi.TienGui = 0;

                        }
                        self.EditTienMat();

                        $('#modalCashSalary').modal('show');
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(data.mess);
                    }
                });
        },
        ClickDatetime: function () {
            var self = this;
            $('.datepicker_mask').datetimepicker(
                {
                    format: "d/m/Y H:i",
                    mask: true,
                    timepicker: false,
                    onChangeDateTime: function (dp, $input, a) {
                        let dtChange = moment(dp).format('DD/MM/YYYY HH:mm');
                        self.newPhieuChi.NgayLapHoaDon = dtChange;
                    },
                });
        },
        ChoseKhoanChi: function (event) {
            var self = this;
            var value = event.target.value;
            if (value === '0') {
                self.newPhieuChi.ID_KhoanThuChi = null;
            }
            else {
                self.newPhieuChi.ID_KhoanThuChi = value;
            }
        },
        ChoseNhanVien: function (event) {
            var self = this;
            self.newPhieuChi.ID_NhanVien = event.target.value;
        },
        ChoseAccountCK: function (item) {
            var self = this;
            if (item.ID === undefined) {
                self.newPhieuChi.TienMat = commonStatisJs.FormatNumber3Digit(self.newPhieuChi.TongCanTraSauTamUng);
                self.newPhieuChi.TienGui = 0;
                self.newPhieuChi.ID_TaiKhoanNganHang = null;
                self.newPhieuChi.ID_NganHang = null;
                self.newPhieuChi.TenTaiKhoanChosing = '-- Chọn tài khoản --';
                self.EditTienMat();
            }
            else {
                self.newPhieuChi.TienMat = 0;
                self.newPhieuChi.TienGui = commonStatisJs.FormatNumber3Digit(self.newPhieuChi.TongCanTraSauTamUng);
                self.newPhieuChi.ID_TaiKhoanNganHang = item.ID;
                self.newPhieuChi.ID_NganHang = item.ID_NganHang;
                self.newPhieuChi.TenTaiKhoanChosing = item.TenChuThe;
                self.EditTienCK();
            }
        },
        filterAcCK: function () {

        },
        EditTienMat: function () {
            var self = this;
            var tienmat = commonStatisJs.FormatNumberToFloat(self.newPhieuChi.TienMat);
            var tiengui = commonStatisJs.FormatNumberToFloat(self.newPhieuChi.TienGui);
            var tongtra = tienmat + tiengui;
            self.newPhieuChi.TongThanhToan = tongtra;
            self.newPhieuChi.TienMat = commonStatisJs.FormatNumber3Digit(tienmat);

            let tientra = 0;
            for (let i = 0; i < self.listdata.BangLuongChiTiet.length; i++) {
                if (tongtra <= self.listdata.BangLuongChiTiet[i].CanTraSauTamUng) {
                    self.listdata.BangLuongChiTiet[i].TienTra = commonStatisJs.FormatNumber3Digit(tongtra);
                    self.listdata.BangLuongChiTiet[i].TienMat = tongtra;
                    self.listdata.BangLuongChiTiet[i].TienGui = 0;
                    tientra += commonStatisJs.FormatNumberToFloat(self.listdata.BangLuongChiTiet[i].TienTra);

                    if (i === 0 && self.listdata.BangLuongChiTiet.length > 0) {
                        for (let j = 1; j < self.listdata.BangLuongChiTiet.length; j++) {
                            self.listdata.BangLuongChiTiet[j].TienTra = 0;
                            self.listdata.BangLuongChiTiet[j].TienMat = 0;
                            self.listdata.BangLuongChiTiet[j].TienGui = 0;
                        }
                    }
                    break;
                }
                else {
                    self.listdata.BangLuongChiTiet[i].TienMat = self.listdata.BangLuongChiTiet[i].CanTraSauTamUng;
                    self.listdata.BangLuongChiTiet[i].TienGui = 0;
                    self.listdata.BangLuongChiTiet[i].TienTra = commonStatisJs.FormatNumber3Digit(self.listdata.BangLuongChiTiet[i].CanTraSauTamUng);
                    tongtra = tongtra - commonStatisJs.FormatNumberToFloat(self.listdata.BangLuongChiTiet[i].TienTra);
                    tientra += commonStatisJs.FormatNumberToFloat(self.listdata.BangLuongChiTiet[i].TienTra);
                }
            }
            self.newPhieuChi.TongTienThu = tientra;
            vmThanhToanLuong.SetValue_TienThua();
        },
        EditTienCK: function () {
            var self = this;
            var tienmat = commonStatisJs.FormatNumberToFloat(self.newPhieuChi.TienMat);
            var tiengui = commonStatisJs.FormatNumberToFloat(self.newPhieuChi.TienGui);
            var tongtra = tienmat + tiengui;
            self.newPhieuChi.TienGui = commonStatisJs.FormatNumber3Digit(tiengui);
            self.newPhieuChi.TongThanhToan = tongtra;
            let tientra = 0;
            for (let i = 0; i < self.listdata.BangLuongChiTiet.length; i++) {
                if (tongtra <= self.listdata.BangLuongChiTiet[i].CanTraSauTamUng) {
                    self.listdata.BangLuongChiTiet[i].TienTra = commonStatisJs.FormatNumber3Digit(tongtra);
                    self.listdata.BangLuongChiTiet[i].TienGui = tongtra;
                    self.listdata.BangLuongChiTiet[i].TienMat = 0;
                    tientra += commonStatisJs.FormatNumberToFloat(self.listdata.BangLuongChiTiet[i].TienTra);

                    if (i === 0 && self.listdata.BangLuongChiTiet.length > 0) {
                        for (let j = 1; j < self.listdata.BangLuongChiTiet.length; j++) {
                            self.listdata.BangLuongChiTiet[j].TienTra = 0;
                            self.listdata.BangLuongChiTiet[j].TienMat = 0;
                            self.listdata.BangLuongChiTiet[j].TienGui = 0;
                        }
                    }
                    break;
                }
                else {
                    self.listdata.BangLuongChiTiet[i].TienGui = self.listdata.BangLuongChiTiet[i].CanTraSauTamUng;
                    self.listdata.BangLuongChiTiet[i].TienMat = 0;
                    self.listdata.BangLuongChiTiet[i].TienTra = commonStatisJs.FormatNumber3Digit(self.listdata.BangLuongChiTiet[i].CanTraSauTamUng);
                    tongtra = tongtra - commonStatisJs.FormatNumberToFloat(self.listdata.BangLuongChiTiet[i].TienTra);
                    tientra += commonStatisJs.FormatNumberToFloat(self.listdata.BangLuongChiTiet[i].TienTra);
                }
            }
            self.newPhieuChi.TongTienThu = tientra;
            vmThanhToanLuong.SetValue_TienThua();
        },
        EditTienTamUng: function (ct, index) {
            var self = this;
            //var $this = $(event.currentTarget);
            var tamungText = ct.TienTamUng;
            if (tamungText.indexOf('.') === -1) {
                tamungText = commonStatisJs.FormatNumber3Digit(commonStatisJs.FormatNumberToFloat(tamungText), 3);
            }
            var tamungFloat = commonStatisJs.FormatNumberToFloat(ct.TienTamUng);

            for (let i = 0; i < self.listdata.BangLuongChiTiet.length; i++) {
                let itFor = self.listdata.BangLuongChiTiet[i];
                if (i === index) {
                    let conlai = itFor.LuongThucNhan - itFor.DaTra;
                    if (tamungFloat > itFor.TamUngLuong) {
                        if (tamungFloat > conlai) {
                            self.listdata.BangLuongChiTiet[i].TienTamUng = commonStatisJs.FormatNumber3Digit(conlai, 3);
                            self.listdata.BangLuongChiTiet[i].CanTraSauTamUng = 0;
                            self.listdata.BangLuongChiTiet[i].TienGui = 0;
                            self.listdata.BangLuongChiTiet[i].TienMat = 0;
                        }
                        else {
                            self.listdata.BangLuongChiTiet[i].TienTamUng = commonStatisJs.FormatNumber3Digit(itFor.TamUngLuong, 3);
                            self.listdata.BangLuongChiTiet[i].CanTraSauTamUng = itFor.LuongThucNhan - itFor.TamUngLuong - itFor.DaTra;

                            if (self.newPhieuChi.ID_TaiKhoanNganHang === null) {
                                self.listdata.BangLuongChiTiet[i].TienGui = 0;
                                self.listdata.BangLuongChiTiet[i].TienMat = itFor.CanTraSauTamUng;
                            }
                            else {
                                self.listdata.BangLuongChiTiet[i].TienGui = itFor.CanTraSauTamUng;
                                self.listdata.BangLuongChiTiet[i].TienMat = 0;
                            }
                        }
                    }
                    else {
                        if (tamungFloat > itFor.ConCanTra) {
                            self.listdata.BangLuongChiTiet[i].TienTamUng = commonStatisJs.FormatNumber3Digit(conlai, 3);
                            self.listdata.BangLuongChiTiet[i].CanTraSauTamUng = 0;
                            self.listdata.BangLuongChiTiet[i].TienGui = 0;
                            self.listdata.BangLuongChiTiet[i].TienMat = 0;
                        }
                        else {
                            self.listdata.BangLuongChiTiet[i].TienTamUng = tamungText;
                            self.listdata.BangLuongChiTiet[i].CanTraSauTamUng = itFor.LuongThucNhan - tamungFloat - itFor.DaTra;

                            if (self.newPhieuChi.ID_TaiKhoanNganHang === null) {
                                self.listdata.BangLuongChiTiet[i].TienGui = 0;
                                self.listdata.BangLuongChiTiet[i].TienMat = self.listdata.BangLuongChiTiet[i].CanTraSauTamUng;
                            }
                            else {
                                self.listdata.BangLuongChiTiet[i].TienGui = self.listdata.BangLuongChiTiet[i].CanTraSauTamUng;
                                self.listdata.BangLuongChiTiet[i].TienMat = 0;
                            }
                        }
                    }
                    self.listdata.BangLuongChiTiet[i].TienTra = commonStatisJs.FormatNumber3Digit(self.listdata.BangLuongChiTiet[i].CanTraSauTamUng);
                    break;
                }
            }

            // sum tongtra
            var tongTra = 0;
            var tongTruTamUng = 0;
            var tongCanTraSauTamUng = 0;

            for (let i = 0; i < self.listdata.BangLuongChiTiet.length; i++) {
                tongTra += commonStatisJs.FormatNumberToFloat(self.listdata.BangLuongChiTiet[i].TienTra);
                tongTruTamUng += commonStatisJs.FormatNumberToFloat(self.listdata.BangLuongChiTiet[i].TienTamUng);
                tongCanTraSauTamUng += self.listdata.BangLuongChiTiet[i].CanTraSauTamUng;
            }

            self.newPhieuChi.TongTienThu = tongTra;
            self.newPhieuChi.TongThanhToan = tongTra;
            self.newPhieuChi.TongTruTamUngThucTe = tongTruTamUng;
            self.newPhieuChi.TongCanTraSauTamUng = tongCanTraSauTamUng;

            // set value txtTienMat, tienCK
            if (self.newPhieuChi.ID_TaiKhoanNganHang === null) {
                self.newPhieuChi.TienGui = 0;
                self.newPhieuChi.TienMat = commonStatisJs.FormatNumber3Digit(tongTra);
            }
            else {
                self.newPhieuChi.TienGui = commonStatisJs.FormatNumber3Digit(tongTra);
                self.newPhieuChi.TienMat = 0;
            }
            vmThanhToanLuong.SetValue_TienThua();
            var keycode = event.keycode || event.which;
            if (keycode === 13) {
                if (index !== self.listdata.BangLuongChiTiet.length - 1) {
                    $('#modalCashSalary tbody tr').eq(index + 1).find('td').eq(6).find('input').focus().select();
                }
            }
        },
        EditDetailTienTra: function (ct, index) {
            var self = this;
            var tientra = commonStatisJs.FormatNumberToFloat(ct.TienTra);

            for (let i = 0; i < self.listdata.BangLuongChiTiet.length; i++) {
                let itFor = self.listdata.BangLuongChiTiet[i];
                if (i === index) {
                    if (tientra > itFor.CanTraSauTamUng) {
                        self.listdata.BangLuongChiTiet[i].TienTra = commonStatisJs.FormatNumber3Digit(itFor.CanTraSauTamUng);
                        if (self.newPhieuChi.ID_TaiKhoanNganHang === null) {
                            self.listdata.BangLuongChiTiet[i].TienGui = 0;
                            self.listdata.BangLuongChiTiet[i].TienMat = itFor.CanTraSauTamUng;
                        }
                        else {
                            self.listdata.BangLuongChiTiet[i].TienGui = itFor.CanTraSauTamUng;
                            self.listdata.BangLuongChiTiet[i].TienMat = 0;
                        }
                    }
                    else {
                        self.listdata.BangLuongChiTiet[i].TienTra = commonStatisJs.FormatNumber3Digit(tientra);
                        if (self.newPhieuChi.ID_TaiKhoanNganHang === null) {
                            self.listdata.BangLuongChiTiet[i].TienGui = 0;
                            self.listdata.BangLuongChiTiet[i].TienMat = tientra;
                        }
                        else {
                            self.listdata.BangLuongChiTiet[i].TienGui = tientra;
                            self.listdata.BangLuongChiTiet[i].TienMat = 0;
                        }
                    }
                    break;
                }
            }

            var tongTra = 0;
            for (let i = 0; i < self.listdata.BangLuongChiTiet.length; i++) {
                tongTra += commonStatisJs.FormatNumberToFloat(self.listdata.BangLuongChiTiet[i].TienTra);
            }

            self.newPhieuChi.TongTienThu = tongTra;
            self.newPhieuChi.TongThanhToan = tongTra;
            if (self.newPhieuChi.ID_TaiKhoanNganHang === null) {
                self.newPhieuChi.TienGui = 0;
                self.newPhieuChi.TienMat = commonStatisJs.FormatNumber3Digit(tongTra);
            }
            else {
                self.newPhieuChi.TienGui = commonStatisJs.FormatNumber3Digit(tongTra);
                self.newPhieuChi.TienMat = 0;
            }
            vmThanhToanLuong.SetValue_TienThua();
            var keycode = event.keycode || event.which;
            if (keycode === 13) {
                if (index !== self.listdata.BangLuongChiTiet.length - 1) {
                    $('#modalCashSalary tbody tr').eq(index + 1).find('td').eq(8).find('input').focus().select();
                }
            }
        },
        SetValue_TienThua: function () {
            var self = this;
            var tienthua = self.newPhieuChi.TongCanTra - self.newPhieuChi.TongThanhToan - self.newPhieuChi.TongTruTamUngThucTe;
            if (tienthua > 0) {
                self.newPhieuChi.LblTienThua = 'Tiền thiếu';
            }
            else {
                self.newPhieuChi.LblTienThua = 'Tiền thừa';
            }
            self.newPhieuChi.TienThua = Math.abs(tienthua);
        },
        ThanhToan: function () {
            var self = this;
            self.loading = true;
            var ngaylapPT = moment(self.newPhieuChi.NgayLapHoaDon, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');
            var lstPost = [];
            for (let i = 0; i < self.listdata.BangLuongChiTiet.length; i++) {
                let itFor = self.listdata.BangLuongChiTiet[i];
                let tientra = commonStatisJs.FormatNumberToFloat(itFor.TienTra);
                let trutamung = commonStatisJs.FormatNumberToFloat(itFor.TienTamUng);
                if (tientra > 0 || trutamung > 0) {
                    let newQCT = {
                        MaBangLuongChiTiet: itFor.MaBangLuongChiTiet,
                        ID_BangLuongChiTiet: itFor.ID_BangLuongChiTiet,
                        ID_NhanVien: itFor.ID_NhanVien,
                        TienMat: itFor.TienMat,
                        TienGui: itFor.TienGui,
                        TienThu: tientra,
                        ID_KhoanThuChi: self.newPhieuChi.ID_KhoanThuChi,
                        ID_TaiKhoanNganHang: self.newPhieuChi.ID_TaiKhoanNganHang,
                        ID_NganHang: self.newPhieuChi.ID_NganHang,
                        ID_DoiTuong: '00000000-0000-0000-0000-000000000000',
                        TruTamUngLuong : trutamung 
                    }
                    let newQuyHD = {
                        ID_DonVi: self.ID_DonVi,
                        LoaiHoaDon: 12,
                        NgayLapHoaDon: ngaylapPT,
                        ID_NhanVien: self.newPhieuChi.ID_NhanVien,
                        NguoiNopTien: itFor.TenNhanVien,
                        NoiDungThu: self.newPhieuChi.NoiDungThu,
                        TongTienThu: newQCT.TienThu,
                        NguoiTao: self.UserLogin,
                        HachToanKinhDoanh: self.newPhieuChi.HachToanKinhDoanh,
                        PhieuDieuChinhCongNo: 0,
                        ThuCuaNhieuDoiTuong: false,
                    }

                    let dataPost = {
                        Quy_HoaDon: newQuyHD,
                        Quy_HoaDon_ChiTiet: [newQCT],
                    }
                    lstPost.push(dataPost);
                }
            }
            var myData = {
                lstQuy: lstPost,
            }
            console.log('lstPost', lstPost);

            $.ajax({
                data: myData,
                url: '/api/DanhMuc/Quy_HoaDonAPI/PostQuy_HoaDon_BangLuong',
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (result) {
                    console.log('PostQuy_HoaDon_BangLuong', result)
                    if (result.res) {
                        commonStatisJs.ShowMessageSuccess('Tạo phiếu thanh toán lương thành công');

                        $.getJSON('/api/DanhMuc/NS_NhanSuAPI/UpdateNgayThanhToanLuong?idBangLuong=' + self.newPhieuChi.ID_BangLuong, '&ngaythanhtoan=' + ngaylapPT, function () {
                        });

                        // save diary todo
                        var noidung = 'Tạo phiếu chi cho bảng lương '.concat(self.newPhieuChi.MaBangLuong,
                            ', Tổng chi: ', commonStatisJs.FormatNumber3Digit(self.newPhieuChi.TongTienThu),
                            ', Ngày thanh toán: ', self.newPhieuChi.NgayLapHoaDon);
                        var khoanchi = self.newPhieuChi.ID_KhoanThuChi === null ? '' : ' <br /> - Khoản chi:'.concat(self.listdata.KhoanChi.filter(x => x.ID === self.newPhieuChi.ID_KhoanThuChi)[0].NoiDungThuChi);
                        var diary = {
                            ID_DonVi: self.ID_DonVi,
                            ID_NhanVien: self.ID_NhanVien,
                            LoaiNhatKy: 1,
                            ChucNang: 'Bảng lương - Thanh toán',
                            NoiDung: noidung,
                            NoiDungChiTiet: noidung.concat(khoanchi, ' <br /> - Phương thức thanh toán:', self.newPhieuChi.ID_TaiKhoanNganHang === null ? ' Tiền mặt' : ' Chuyển khoản',
                                ' <br /> - Người lập phiếu: ', self.listdata.NhanVienLapPhieu.filter(x => x.ID === self.newPhieuChi.ID_NhanVien)[0].TenNhanVien,
                                self.newPhieuChi.NoiDungThu === '' ? '' : ' <br /> - Nội dung chi: ', self.newPhieuChi.NoiDungThu)
                        }
                        vmBangLuong.SaveDiary(diary);
                    }
                    else {
                        commonStatisJs.ShowMessageDanger('Tạo phiếu thanh toán lương thất bại');
                    }
                },
                error: function (result) {
                },
                complete: function () {
                    self.loading = false;
                    $('#modalCashSalary').modal('hide');
                }
            });
        },
    },
    computed: {
        hideShowTamUng: function () {
            var selt = this;
            return selt.listdata.BangLuongChiTiet.filter(x => x.TamUngLuong > 0).length > 0;
        },
        hideShowDaTra: function () {
            var selt = this;
            return selt.listdata.BangLuongChiTiet.filter(x => x.DaTra > 0).length > 0;
        }
    }
});
vmThanhToanLuong.GetData();