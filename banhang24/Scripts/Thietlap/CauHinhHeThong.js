var CauHinhModel = function () {
    var self = this;
    self.ID = ko.observable();
    self.ID_DonVi = ko.observable();
    self.GiaVonTrungBinh = ko.observable(false);
    self.CoDonViTinh = ko.observable(false);
    self.DatHang = ko.observable(false);
    self.XuatAm = ko.observable(false);
    self.DatHangXuatAm = ko.observable(false);
    self.LoHang = ko.observable(false);
    self.SoLuongTrenChungTu = ko.observable(false);
    self.TinhNangTichDiem = ko.observable(false);
    self.GioiHanThoiGianTraHang = ko.observable(false);
    self.SanPhamCoThuocTinh = ko.observable(false);
    self.BanVaChuyenKhiHangDaDat = ko.observable(false);
    self.TinhNangSanXuatHangHoa = ko.observable(false);
    self.SuDungCanDienTu = ko.observable(false);
    self.KhoaSo = ko.observable(false);
    self.InBaoGiaKhiBanHang = ko.observable(false);
    self.QuanLyKhachHangTheoDonVi = ko.observable(false);
    self.KhuyenMai = ko.observable(false);
    self.SuDungMauInMacDinh = ko.observable(false);
    self.ApDungGopKhuyenMai = ko.observable(false);
    self.ThongTinChiTietNhanVien = ko.observable(false);
    self.BanHangOffline = ko.observable(false);
    self.ThoiGianNhacHanSuDungLo = ko.observable(1);
    self.SuDungMaChungTu = ko.observable(false);
    self.ThietLapCongChuan = ko.observable(false);
    self.ChoPhepTrungSoDienThoai = ko.observable(false);

    self.SetData = function (item) {
        self.ID(item.ID);
        self.ID_DonVi(item.ID_DonVi);
        self.GiaVonTrungBinh(item.GiaVonTrungBinh);
        self.CoDonViTinh(item.CoDonViTinh);
        self.DatHang(item.DatHang);
        self.XuatAm(item.XuatAm);
        self.DatHangXuatAm(item.DatHangXuatAm);
        self.LoHang(item.LoHang);
        self.SoLuongTrenChungTu(item.SoLuongTrenChungTu);
        self.TinhNangTichDiem(item.TinhNangTichDiem);
        self.GioiHanThoiGianTraHang(item.GioiHanThoiGianTraHang);
        self.SanPhamCoThuocTinh(item.SanPhamCoThuocTinh);
        self.BanVaChuyenKhiHangDaDat(item.BanVaChuyenKhiHangDaDat);
        self.TinhNangSanXuatHangHoa(item.TinhNangSanXuatHangHoa);
        self.SuDungCanDienTu(item.SuDungCanDienTu);
        self.KhoaSo(item.KhoaSo);
        self.InBaoGiaKhiBanHang(item.InBaoGiaKhiBanHang);
        self.QuanLyKhachHangTheoDonVi(item.QuanLyKhachHangTheoDonVi);
        self.KhuyenMai(item.KhuyenMai);
        self.SuDungMauInMacDinh(item.SuDungMauInMacDinh);
        self.ApDungGopKhuyenMai(item.ApDungGopKhuyenMai);
        self.ThongTinChiTietNhanVien(item.ThongTinChiTietNhanVien);
        self.BanHangOffline(item.BanHangOffline);
        self.ThoiGianNhacHanSuDungLo(item.ThoiGianNhacHanSuDungLo);
        self.SuDungMaChungTu(item.SuDungMaChungTu);
        self.ChoPhepTrungSoDienThoai(item.ChoPhepTrungSoDienThoai);
    };
};

var ModelTichDiem = function () {
    var self = this;
    self.ID = ko.observable();
    self.ID_CauHinh = ko.observable();
    self.TyLeDoiDiem = ko.observable();
    self.ThanhToanBangDiem = ko.observable(false);
    self.DiemThanhToan = ko.observable();
    self.TienThanhToan = ko.observable();
    self.TichDiemGiamGia = ko.observable(false);
    self.TichDiemHoaDonDiemThuong = ko.observable(false);
    self.ToanBoKhachHang = ko.observable(true);
    self.KhoiTaoTichDiem = ko.observable(false);
    self.SoLanMua = ko.observable();
    self.TichDiemHoaDonGiamGia = ko.observable(false);

    self.SetData = function (item) {
        self.ID(item.ID);
        self.ID_CauHinh(item.ID_CauHinh);
        self.TyLeDoiDiem(item.TyLeDoiDiem);
        self.ThanhToanBangDiem(item.ThanhToanBangDiem);
        self.DiemThanhToan(item.DiemThanhToan);
        self.TienThanhToan(item.TienThanhToan);
        self.TichDiemGiamGia(item.TichDiemGiamGia);
        self.TichDiemHoaDonDiemThuong(item.TichDiemHoaDonDiemThuong);
        self.ToanBoKhachHang(item.ToanBoKhachHang);
        self.KhoiTaoTichDiem(item.KhoiTaoTichDiem);
        self.SoLanMua(item.SoLanMua);
        self.TichDiemHoaDonGiamGia(item.TichDiemHoaDonGiamGia);
    }
};

var Model_GioiHanTraHang = function () {
    var self = this;
    self.ID = ko.observable();
    self.ID_CauHinh = ko.observable();
    self.SoNgayGioiHan = ko.observable();
    self.ChoPhepTraHang = ko.observable(true);
    self.SetData = function (item) {
        self.ID(item.ID);
        self.ID_CauHinh(item.ID_CauHinh);
        self.SoNgayGioiHan(item.SoNgayGioiHan);
        self.ChoPhepTraHang(item.ChoPhepTraHang);
    }
}

var modelThongTinCuaHang = function () {
    var self = this;
    self.ID = ko.observable();
    self.TenCongTy = ko.observable();
    self.DiaChi = ko.observable();
    self.SoDienThoai = ko.observable();
    self.DiaChiNganHang = ko.observable();
    self.DiaChiNganHangRemove = ko.observable();
    self.Website = ko.observable();
    self.HanSuDung = ko.observable();
    self.NgayCongChuan = ko.observable();
    self.SetData = function (item) {
        self.ID(item[0].ID);
        self.TenCongTy(item[0].TenCongTy);
        self.DiaChi(item[0].DiaChi);
        self.SoDienThoai(item[0].SoDienThoai);
        if (item[0].DiaChiNganHang !== "") {
            self.DiaChiNganHang(Open24FileManager.hostUrl + item[0].DiaChiNganHang);
            self.DiaChiNganHangRemove(item[0].DiaChiNganHang);
        }
        else {
            self.DiaChiNganHang(item[0].DiaChiNganHang);
        }
        self.Website(item[0].Website);
        self.HanSuDung(moment(item[0].HanSuDung).format("DD/MM/YYYY"));

        self.NgayCongChuan(item[0].NgayCongChuan);
    }
}

var ViewModel = function () {
    var self = this;
    var _IDNhomNguoiDung = $('.idnhomnguoidung').text();
    var _IDNguoiDung = $('.idnguoidung').text();
    var _IDchinhanh = $('#hd_IDdDonVi').val();
    var _IDNhanVien = $('.idnhanvien').text();
    self.error = ko.observable();
    var ThietLapUri = '/api/DanhMuc/HT_ThietLapAPI/';
    var DonViUri = '/api/DanhMuc/DM_DonViAPI/';
    var _TenDonVi = null;
    self.newTichDiem = ko.observable(new ModelTichDiem());
    self.newGioiHanTH = ko.observable(new Model_GioiHanTraHang);
    self.newThongTinCH = ko.observable(new modelThongTinCuaHang());
    self.ThietLap = ko.observable(new CauHinhModel());
    self.selectedNKH = ko.observable();
    self.TodayCS = ko.observable();

    var dt1 = new Date();
    var _time = dt1.getFullYear() + "-" + (dt1.getMonth() + 1) + "-" + dt1.getDate();
    self.TodayCS(moment(dt1).format("DD/MM/YYYY"));
    function ajaxHelper(uri, method, data) {
        self.error(''); // Clear error message
        return $.ajax({
            type: method,
            url: uri,
            dataType: 'json',
            contentType: 'application/json',
            data: data ? JSON.stringify(data) : null,
            statusCode: {
                404: function () {
                    self.error("Page not found");
                },
            }
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
            });
    };

    //load quyền
    function loadQuyenIndex() {
        var arrQuyen = [];
        ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetHT_NhomNguoiDung?idnguoidung=" + _IDNguoiDung + '&iddonvi=' + _IDchinhanh, 'GET').done(function (data) {

            if (data.HT_Quyen_NhomDTO.length > 0) {
                for (var i = 0; i < data.HT_Quyen_NhomDTO.length; i++) {
                    arrQuyen.push(data.HT_Quyen_NhomDTO[i].MaQuyen);
                }
            }
            localStorage.setItem('lc_CTQuyen', JSON.stringify(arrQuyen));
        });
        ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/' + 'GetCauHinhHeThong/' + _IDchinhanh, 'GET').done(function (data) {
            localStorage.setItem('lc_CTThietLap', JSON.stringify(data));
        });
    }
    loadQuyenIndex();

    self.MangNhomKH = ko.observableArray();

    self.clickShowTichDiem = function (item) {
        $('#modalpopupTichDiem').modal('show');
        self.MangNhomKH([]);
        ajaxHelper(ThietLapUri + "GetTichDiemByID_CauHinh?id=" + self.ThietLap().ID(), 'GET').done(function (data) {
            self.newTichDiem().SetData(data);
            self.MangNhomKH(data.HT_CauHinh_TichDiemApDung);
            if (self.MangNhomKH().length > 0) {
                for (var i = 0; i < self.MangNhomKH().length; i++) {
                    $('#selec-all-KH li').each(function () {
                        if ($(this).attr('id') === self.MangNhomKH()[i].ID) {
                            $(this).find('.fa-check').remove();
                            $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>');
                        }
                    });
                }
                $('#choose_NguoiBan input').remove();
            }
            else {
                $('#choose_NguoiBan input').remove();
                $('#choose_NguoiBan').append('<input type="text" id="dllNhomKH" readonly="readonly" class="dropdown" placeholder="--- Chọn nhóm khách hàng ---">');
                $('#selec-all-KH li').each(function () {
                    $(this).find('.fa-check').remove();
                });
            }
            if (data.ThanhToanBangDiem === false) {
                document.getElementById("diemthanhtoan").disabled = true;
                document.getElementById("tienthanhtoan").disabled = true;
                document.getElementById("solanmua").disabled = true;
            }
            if (data.ToanBoKhachHang === true) {
                document.getElementById("dllNhomKH").disabled = true;
            }
        });
    }

    self.clickShowGioiHanTH = function (item) {
        ajaxHelper(ThietLapUri + "GetGioiHanTraHangByID_CauHinh?id=" + self.ThietLap().ID(), 'GET').done(function (data) {
            self.newGioiHanTH().SetData(data);
            $('#datail-TH').modal('show');
        });
    }

    self.addGioiHanTH = function () {
        var _songayTH = self.newGioiHanTH().SoNgayGioiHan();
        var _choPhepTH = self.newGioiHanTH().ChoPhepTraHang();
        var _id = self.newGioiHanTH().ID();

        var objTH = {
            ID: _id,
            ID_CauHinh: self.ThietLap().ID(),
            SoNgayGioiHan: _songayTH,
            ChoPhepTraHang: _choPhepTH
        }

        var myData = {};
        myData.objTraHang = objTH;
        myData.id = _id;

        if (_id === '00000000-0000-0000-0000-000000000000') {
            $.ajax({
                data: myData,
                url: ThietLapUri + "Post_GioiHanTH",
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",

                success: function (item) {
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Thiết lập thành công", "success");
                    $('#datail-TH').modal('hide');
                },
                statusCode: {
                    404: function () {
                        self.error("page not found");
                    },
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                    $('#datail-TH').modal('hide');
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Thiết lập thất bại", "danger");
                },
                complete: function () {
                }
            })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                });
        }
        else {
            $.ajax({
                data: myData,
                url: ThietLapUri + "Put_GioiHanTH",
                type: 'PUT',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",

                success: function (item) {
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Thiết lập thành công", "success");
                    $('#datail-TH').modal('hide');
                },
                statusCode: {
                    404: function () {
                        self.error("page not found");
                    },
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                    $('#datail-TH').modal('hide');
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Thiết lập thất bại", "danger");
                },
                complete: function () {
                }
            })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                });
        }
    }

    self.addTichDiem = function () {
        var _tyleTichDiem = self.newTichDiem().TyLeDoiDiem();
        var _choThanhToanBangDiem = self.newTichDiem().ThanhToanBangDiem();
        var _diemThanhToan = self.newTichDiem().DiemThanhToan();
        var _tienThanhToan = self.newTichDiem().TienThanhToan();
        var _tichDiemGiamGia = self.newTichDiem().TichDiemGiamGia();
        var _tichDiemHoaDonDiemThuong = self.newTichDiem().TichDiemHoaDonDiemThuong();
        var _toanBoKhachHang = self.newTichDiem().ToanBoKhachHang();
        var _solanmua = self.newTichDiem().SoLanMua();
        var _tichdiemhoadongiamgia = self.newTichDiem().TichDiemHoaDonGiamGia();
        //var _idNhomKhachHang = self.selectedNKH();
        var _id = self.newTichDiem().ID();
        //if (_idNhomKhachHang == undefined) {
        //    _idNhomKhachHang = '00000000-0000-0000-0000-000000000000';
        //}
        if (_tyleTichDiem === 0) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Tỷ lệ quy đổi điểm thưởng chưa đúng. Số tiền quy đổi sang 1 điểm thưởng phải lớn hơn 0", "danger");
            $('#txtTyLeDoiDiem').select();
            return false;
        }

        if (_toanBoKhachHang === false && self.MangNhomKH().length === 0) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn chưa chọn nhóm khách hàng", "danger");
            $('#dllNhomKH').focus();
            return false;
        }

        var objTichDiem = {
            ID: _id,
            ID_CauHinh: self.ThietLap().ID(),
            TyLeDoiDiem: _tyleTichDiem,
            ThanhToanBangDiem: _choThanhToanBangDiem,
            DiemThanhToan: _diemThanhToan,
            TienThanhToan: _tienThanhToan,
            TichDiemGiamGia: _tichDiemGiamGia,
            TichDiemHoaDonDiemThuong: _tichDiemHoaDonDiemThuong,
            ToanBoKhachHang: _toanBoKhachHang,
            SoLanMua: _solanmua,
            TichDiemHoaDonGiamGia: _tichdiemhoadongiamgia,
            KhoiTaoTichDiem: self.newTichDiem().KhoiTaoTichDiem(),
        }
        var myData = {};
        myData.objTichDiem = objTichDiem;
        myData.objNhomKhachHang = self.MangNhomKH();
        myData.id = _id;

        var tennhom = '';
        for (let i = 0; i < self.MangNhomKH().length; i++) {
            tennhom += self.MangNhomKH()[i].TenNhomDoiTuong + ', ';
        }
        var chucnang = 'Thiết lập tích điểm';
        var apdung = _toanBoKhachHang ? 'cho toàn bộ khách hàng' : 'theo nhóm: ' + tennhom;
        var sttdiem = _choThanhToanBangDiem ? ', '.concat(_diemThanhToan, ' điểm = ', _tienThanhToan, ' vnd') : '';
        var noidungct = 'Nội dung chi tiết: <br />'.concat('- Tỷ lệ quy đổi điểm thưởng: ', _tyleTichDiem, ' vnd = 1 điểm thưởng',
            ' <br /> - Cho phép thanh toán bằng điểm: ', _choThanhToanBangDiem, sttdiem,
            ' <br /> - Thanh toán bằng điểm sau: ', _solanmua, ' lần mua',
            ' <br /> - Không tích điểm cho sản phẩm giảm giá: ', _tichDiemGiamGia,
            ' <br /> - Không tích điểm cho hóa đơn có giảm giá: ', _tichdiemhoadongiamgia,
            ' <br /> - Không tích điểm cho hóa đơn thanh toán bằng điểm thưởng: ', _tichDiemHoaDonDiemThuong,
            ' <br /> - Áp dụng ', apdung,
            ' <br /> - Khởi tạo tích điểm cho hàng hóa: ' + self.newTichDiem().KhoiTaoTichDiem());
        if (_id === '00000000-0000-0000-0000-000000000000') {
            $.ajax({
                data: myData,
                url: ThietLapUri + "Post_TichDiem",
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",

                success: function (item) {
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Thiết lập thành công", "success");
                    $('#modalpopupTichDiem').modal('hide');

                    let diary = {
                        ID_NhanVien: _IDNhanVien,
                        ID_DonVi: _IDchinhanh,
                        ChucNang: chucnang,
                        NoiDung: chucnang.concat(' - Thêm mới'),
                        NoiDungChiTiet: noidungct,
                        LoaiNhatKy: 1
                    };
                    saveDiary(diary);
                },
                statusCode: {
                    404: function () {
                        self.error("page not found");
                    },
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                    $('#modalpopupTichDiem').modal('hide');
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Thiết lập thất bại", "danger");
                },
                complete: function () {
                }
            })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                });
        } else {
            $.ajax({
                data: myData,
                url: ThietLapUri + "Put_TichDiem",
                type: 'PUT',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",

                success: function (item) {
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Thiết lập thành công", "success");
                    $('#modalpopupTichDiem').modal('hide');
                    let diary = {
                        ID_NhanVien: _IDNhanVien,
                        ID_DonVi: _IDchinhanh,
                        ChucNang: chucnang,
                        NoiDung: chucnang.concat(' - Cập nhật'),
                        NoiDungChiTiet: noidungct,
                        LoaiNhatKy: 2 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
                    };
                    saveDiary(diary);
                    var tichdiem = objTichDiem.KhoiTaoTichDiem ? 1 : 0;
                    UpdateHangHoa_TichDiem(tichdiem);
                },
                statusCode: {
                    404: function () {
                        self.error("page not found");
                    },
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                    $('#modalpopupTichDiem').modal('hide');
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Thiết lập thất bại", "danger");
                },
                complete: function () {
                }
            })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                });
        }
    }

    function saveDiary(diary) {
        var myData = {
            objDiary: diary
        };
        ajaxHelper('/api/DanhMuc/SaveDiary/' + "post_NhatKySuDung", 'POST', myData).done(function () {

        });
    }

    function UpdateHangHoa_TichDiem(tichdiem) {
        $.getJSON('/api/DanhMuc/DM_HangHoaAPI/' + 'UpdateHangHoa_TichDiem?tichdiem=' + tichdiem, function (x) {

        });
    }
    self.NhomKhachHang = ko.observableArray();
    function getallNhomKH() {
        ajaxHelper('/api/DanhMuc/DM_NhomDoiTuongAPI/' + 'GetDM_NhomDoiTuong?loaiDoiTuong=' + 1, 'GET').done(function (data) {
            self.NhomKhachHang(data);
        })
    }
    getallNhomKH();
    self.selectedNewNKH = function (item) {

        if (!self.MangNhomKH().some(o => o.ID === item.ID)) {
            self.MangNhomKH.push(item);
        }
        $('#selec-all-KH li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
                $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>');
            }
        });
        $('#choose_NguoiBan input').remove();
    }

    self.CloseNKH = function (item) {

        self.MangNhomKH.remove(item);
        if (self.MangNhomKH().length === 0) {
            $('#choose_NguoiBan').append('<input type="text" id="dllNhomKH" readonly="readonly" class="dropdown" placeholder="--- Chọn nhóm khách hàng ---">');
        }
        // remove check
        $('#selec-all-KH li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
    }

    self.NhacLoHangTrungGian = ko.observable();

    self.GetThietLapHeThong = function () {
        self.resetThietLap();
        ajaxHelper(ThietLapUri + "GetCauHinhHeThong/" + _IDchinhanh, 'GET').done(function (data) {
            self.ThietLap().SetData(data);
            self.NhacLoHangTrungGian(data.ThoiGianNhacHanSuDungLo);
            if (data.TinhNangTichDiem === true) {
                $('.tichdiem').show();
            }
            if (data.KhoaSo === true) {
                $('.chotso').show();
            }
            if (data.DatHang === true) {
                $('.dathang').show();
            }
            if (data.KhuyenMai === true) {
                $('.khuyenmai').show();
            }
            if (data.LoHang === true) {
                $('.lohang').show();
            }
            if (data.GioiHanThoiGianTraHang === true) {
                $('.gioihanthoitrahang').show();
            }
            if (data.SuDungMaChungTu === 1) {
                $('.machungtu').show();
            }
            localStorage.setItem('lc_CTThietLap', JSON.stringify(data));
        });
    };

    self.LoadThongTinKichHoat = function () {
        ajaxHelper('api/DanhMuc/ThietLapApi/' + 'GetThongTinKH', 'GET').done(function (data) {
            if (data) {
                $('#CheckKichHoatSMS').prop('checked', true);
                $('.popup-active-sms').show();
                $('.img-active-sms').hide();
            }
            else {
                $('#CheckKichHoatSMS').prop('checked', false);
                $('.popup-active-sms').hide();
                $('.img-active-sms').show();
            }
        });
    };

    self.LoGo = ko.observable(undefined);
    self.GetThongTinCuaHang = function () {
        ajaxHelper(ThietLapUri + "GetThongTinCuaHang/" + _IDchinhanh, 'GET').done(function (data) {
            //var newDate = moment(new Date()).format('YYYY/MM/DD HH:mm:ss');
            //var hansd = moment(data[0].HanSuDung).format('YYYY/MM/DD HH:mm:ss');
            //var diff = Math.abs(newDate - hansd);
            //console.log('diff: ' + diff)
            self.LoGo(data.DiaChiNganHang);
            self.files([]);
            $('#blahHH1').show();
            self.newThongTinCH().SetData(data);
        });
    };

    function GetThongTinCuaHang() {
        self.GetThongTinCuaHang();
    }
    GetThongTinCuaHang();
    self.resetThietLap = function () {
        self.ThietLap(new CauHinhModel());
    }

    self.CapNhatQuanLyTheoLo = ko.observable(false);

    self.LuuNgayNhacLo = function () {
        var _ngaynhacnho = self.ThietLap().ThoiGianNhacHanSuDungLo();
        ajaxHelper(ThietLapUri + 'UpdateSoNgayNhacLoHang?ngay=' + _ngaynhacnho, 'GET').done(function (data) {
            if (data === "") {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Lưu thiết lập thành công", "success");
                $('#modalPopup_lohanghoa').modal('hide');
            }
        });
    };

    self.HuyNhacLoHang = function () {
        self.ThietLap().ThoiGianNhacHanSuDungLo(self.NhacLoHangTrungGian());
    };

    self.LuuThietLap = function (formElement) {
        //giá vốn trung bình
        var cbGiaVonTrungBinhClass = document.getElementById("cbGiaVonTrungBinh").getAttribute("class");
        //console.log(cbGiaVonTrungBinhClass.indexOf("cl"));
        if (cbGiaVonTrungBinhClass.indexOf("cl") > 0) {
            self.ThietLap().GiaVonTrungBinh(true);
        }
        else {
            self.ThietLap().GiaVonTrungBinh(false);
        }
        //có đơn vị tính
        var cbCoDonViTinhClass = document.getElementById("cbDonViTinh").getAttribute("class");
        if (cbCoDonViTinhClass.indexOf("cl") > 0) {
            self.ThietLap().CoDonViTinh(true);
        }
        else {
            self.ThietLap().CoDonViTinh(false);
        }
        //tính năng đặt hàng
        var cbDatHangClass = document.getElementById("cbDatHang").getAttribute("class");
        if (cbDatHangClass.indexOf("cl") > 0) {
            self.ThietLap().DatHang(true);
        }
        else {
            self.ThietLap().DatHang(false);
        }
        //đặt hàng xuất âm
        var cbDatHangXuatAmClass = document.getElementById("cbDatHangXuatAm").getAttribute("class");
        if (cbDatHangXuatAmClass.indexOf("cl") > 0) {
            self.ThietLap().DatHangXuatAm(true);
        }
        else {
            self.ThietLap().DatHangXuatAm(false);
        }
        //lô hàng
        var cbLoHangClass = document.getElementById("cbLoHang").getAttribute("class");
        if (cbLoHangClass.indexOf("cl") > 0) {
            self.ThietLap().LoHang(true);
        }
        else {
            self.ThietLap().LoHang(false);
        }
        //tích điểm

        var cbTichDiemClass = document.getElementById("cbTichDiem").getAttribute("class");
        if (cbTichDiemClass.indexOf("cl") > 0) {
            self.ThietLap().TinhNangTichDiem(true);
        }
        else {
            self.ThietLap().TinhNangTichDiem(false);
        }

        //giới hạn thời gian TH
        var cbGioiHanThoiGianTHClass = document.getElementById("cbGioiHanThoiGianTH").getAttribute("class");
        if (cbGioiHanThoiGianTHClass.indexOf("cl") > 0) {
            self.ThietLap().GioiHanThoiGianTraHang(true);
        }
        else {
            self.ThietLap().GioiHanThoiGianTraHang(false);
        }
        //sản phẩm có thuộc tính
        var cbSPCoThuocTinhClass = document.getElementById("cbSanPhamCoThuocTinh").getAttribute("class");
        if (cbSPCoThuocTinhClass.indexOf("cl") > 0) {
            self.ThietLap().SanPhamCoThuocTinh(true);
        }
        else {
            self.ThietLap().SanPhamCoThuocTinh(false);
        }
        //bán và chuyển khi đặt hàng
        //var cbBanVaChuyenKhiDHClass = document.getElementById("cbBanVaChuyenKhiHangDaDat").getAttribute("class");
        //if (cbBanVaChuyenKhiDHClass.indexOf("cl") > 0) {
        //    self.ThietLap().BanVaChuyenKhiHangDaDat(true);
        //}
        //else {
        //    self.ThietLap().BanVaChuyenKhiHangDaDat(false);
        //}
        var cbcbBanHangOffline = document.getElementById("cbBanHangOffline").getAttribute("class");
        if (cbcbBanHangOffline.indexOf("cl") > 0) {
            self.ThietLap().BanHangOffline(true);
        }
        else {
            self.ThietLap().BanHangOffline(false);
        }

        //sản xuất HH
        //var cbSanXuatHHClass = document.getElementById("cbTinhNangSXHangHoa").getAttribute("class");
        //if (cbSanXuatHHClass.indexOf("cl") > 0) {
        //    self.ThietLap().TinhNangSanXuatHangHoa(true);
        //}
        //else {
        //    self.ThietLap().TinhNangSanXuatHangHoa(false);
        //}
        //bán hàng xuất âm
        var cbXuatAmClass = document.getElementById("cbXuatAm").getAttribute("class");
        if (cbXuatAmClass.indexOf("cl") > 0) {
            self.ThietLap().XuatAm(true);
        }
        else {
            self.ThietLap().XuatAm(false);
        }
        //có đơn vị tính
        var cbDonViTinhClass = document.getElementById("cbDonViTinh").getAttribute("class");
        if (cbDonViTinhClass.indexOf("cl") > 0) {
            self.ThietLap().CoDonViTinh(true);
        }
        else {
            self.ThietLap().CoDonViTinh(false);
        }
        //sử dụng cân điện tử
        //var cbSuDungCanDTClass = document.getElementById("cbSuDungCanDienTu").getAttribute("class");
        //if (cbSuDungCanDTClass.indexOf("cl") > 0) {
        //    self.ThietLap().SuDungCanDienTu(true);
        //}
        //else {
        //    self.ThietLap().SuDungCanDienTu(false);
        //}
        //khóa sổ
        var cbKhoSoClass = document.getElementById("cbKhoaSo").getAttribute("class");
        if (cbKhoSoClass.indexOf("cl") > 0) {
            self.ThietLap().KhoaSo(true);
        }
        else {
            self.ThietLap().KhoaSo(false);
        }
        //In báo giá khi BH
        //var cbInBaoGiaKhiBHClass = document.getElementById("cbInBaoGiaKhiBH").getAttribute("class");
        //if (cbInBaoGiaKhiBHClass.indexOf("cl") > 0) {
        //    self.ThietLap().InBaoGiaKhiBanHang(true);
        //}
        //else {
        //    self.ThietLap().InBaoGiaKhiBanHang(false);
        //}
        //Quản lý KH theo đơn vị
        var cbQuanLyKHTheoDonViClass = document.getElementById("cbQuanLyKHTheoDonVi").getAttribute("class");
        if (cbQuanLyKHTheoDonViClass.indexOf("cl") > 0) {
            self.ThietLap().QuanLyKhachHangTheoDonVi(true);
        }
        else {
            self.ThietLap().QuanLyKhachHangTheoDonVi(false);
        }
        //số lượng hàng hóa trên chứng từ
        //var cbSoLuongTrenChungTuClass = document.getElementById("cbSoLuongTrenChungTu").getAttribute("class");
        //if (cbSoLuongTrenChungTuClass.indexOf("cl") > 0) {
        //    self.ThietLap().SoLuongTrenChungTu(true);
        //}
        //else {
        //    self.ThietLap().SoLuongTrenChungTu(false);
        //}

        var cbKhuyenMai = document.getElementById("cbKhuyenMai").getAttribute("class");
        if (cbKhuyenMai.indexOf("cl") > 0) {
            self.ThietLap().KhuyenMai(true);
        }
        else {
            self.ThietLap().KhuyenMai(false);
        }

        var cbChiTietNhanVien = document.getElementById("cbChiTietNV").getAttribute("class");
        if (cbChiTietNhanVien.indexOf("cl") > 0) {
            self.ThietLap().ThongTinChiTietNhanVien(true);
        }
        else {
            self.ThietLap().ThongTinChiTietNhanVien(false);
        }

        var cbTinhNangMauInMD = document.getElementById("cbTinhNangMauInMD").getAttribute("class");
        if (cbTinhNangMauInMD.indexOf("cl") > 0) {
            self.ThietLap().SuDungMauInMacDinh(true);
        }
        else {
            self.ThietLap().SuDungMauInMacDinh(false);
        }

        var cbMauChungTu = document.getElementById("cbMauChungTu").getAttribute("class");
        if (cbMauChungTu.indexOf("cl") > 0) {
            self.ThietLap().SuDungMaChungTu(1);
        }
        else {
            self.ThietLap().SuDungMaChungTu(0);
        }

        var cbSameSDT = document.getElementById("cbSameSDT").getAttribute("class");
        if (cbSameSDT.indexOf("cl") > 0) {
            self.ThietLap().ChoPhepTrungSoDienThoai(1);
        }
        else {
            self.ThietLap().ChoPhepTrungSoDienThoai(0);
        }

        var _id = self.ThietLap().ID();
        var _checkGVTB = self.ThietLap().GiaVonTrungBinh();
        var _datHang = self.ThietLap().DatHang();
        var _tichDiem = self.ThietLap().TinhNangTichDiem();
        var _gioiHanThoiGianTH = self.ThietLap().GioiHanThoiGianTraHang();
        var _sanPhamCoThuocTinh = self.ThietLap().SanPhamCoThuocTinh();
        var _banVaChuyenKhiDH = self.ThietLap().BanVaChuyenKhiHangDaDat();
        var _datHangXuatAm = self.ThietLap().DatHangXuatAm();
        var _lohang = self.ThietLap().LoHang();
        var _xuatAm = self.ThietLap().XuatAm();
        var _sanXuatHangHoa = self.ThietLap().TinhNangSanXuatHangHoa();
        var _coDonViTinh = self.ThietLap().CoDonViTinh();
        var _suDungCanDienTu = self.ThietLap().SuDungCanDienTu();
        var _khoaSo = self.ThietLap().KhoaSo();
        var _inBaoGiaKhiBH = self.ThietLap().InBaoGiaKhiBanHang();
        var _quanLyKHTheoDonVi = self.ThietLap().QuanLyKhachHangTheoDonVi();
        var _soluongTrenChungTu = self.ThietLap().SoLuongTrenChungTu();
        var _khuyenMai = self.ThietLap().KhuyenMai();
        var _mauInMacDinh = self.ThietLap().SuDungMauInMacDinh();
        var _gopKhuyenMai = self.ThietLap().ApDungGopKhuyenMai();
        var _thongtinctnhanvien = self.ThietLap().ThongTinChiTietNhanVien();
        var _thoigiannhacnholo = self.ThietLap().ThoiGianNhacHanSuDungLo();
        var _sudungMaChungTu = self.ThietLap().SuDungMaChungTu();
        var _trungSDT = self.ThietLap().ChoPhepTrungSoDienThoai();

        var objThietLap = {
            ID: _id,
            GiaVonTrungBinh: _checkGVTB,
            CoDonViTinh: _coDonViTinh,
            DatHang: _datHang,
            XuatAm: _xuatAm,
            DatHangXuatAm: _datHangXuatAm,
            LoHang: _lohang,
            TinhNangTichDiem: _tichDiem,
            GioiHanThoiGianTraHang: _gioiHanThoiGianTH,
            SanPhamCoThuocTinh: _sanPhamCoThuocTinh,
            BanVaChuyenKhiHangDaDat: _banVaChuyenKhiDH,
            TinhNangSanXuatHangHoa: _sanXuatHangHoa,
            SuDungCanDienTu: _suDungCanDienTu,
            KhoaSo: _khoaSo,
            InBaoGiaKhiBanHang: _inBaoGiaKhiBH,
            QuanLyKhachHangTheoDonVi: _quanLyKHTheoDonVi,
            SoLuongTrenChungTu: _soluongTrenChungTu,
            KhuyenMai: _khuyenMai,
            SuDungMauInMacDinh: _mauInMacDinh,
            ApDungGopKhuyenMai: _gopKhuyenMai,
            ThongTinChiTietNhanVien: _thongtinctnhanvien,
            BanHangOffline: self.ThietLap().BanHangOffline(),
            ThoiGianNhacHanSuDungLo: _thoigiannhacnholo,
            SuDungMaChungTu: _sudungMaChungTu,
            ChoPhepTrungSoDienThoai: _trungSDT,
        };
        var myData = {};
        myData.objThietLap = objThietLap;
        myData.id_donvi = _IDchinhanh;
        //if (self.CapNhatQuanLyTheoLo() === true) {
        //    ajaxHelper('/api/DanhMuc/DM_HangHoaAPI/' + 'CapNhatQuanLyTheoLoHangAllHH', 'GET').done(function (data) {
        //    })
        //}
        localStorage.setItem('lc_CTThietLap', JSON.stringify(myData.objThietLap));
        ajaxHelper(ThietLapUri + "GetTichDiemByID_CauHinh?id=" + self.ThietLap().ID(), 'GET').done(function (data) {
            if (data.ID === "00000000-0000-0000-0000-000000000000" && self.ThietLap().TinhNangTichDiem() === true) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Thiết lập lựa chọn tính năng tích điểm. Bạn cần thiết lập chi tiết tích điểm trước khi lưu", "danger");
                return false;
            }
            else {
                $.ajax({
                    data: myData,
                    url: ThietLapUri + "Put_HT_CauHinhPhanMem/",
                    type: 'PUT',
                    async: true,
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",

                    success: function (item) {
                        self.LuuTinhNangLS(objThietLap);
                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Thiết lập thành công", "success");
                    },
                    statusCode: {
                        404: function () {
                            self.error("page not found");
                        },
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Thiết lập thất bại", "danger");
                    },
                    complete: function () {
                    }
                })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                    });
            }
        });
    }

    self.ShowThongBaoLoHang = function (item) {
        var cbLoHangClass = document.getElementById("cbLoHang").getAttribute("class");
        if (cbLoHangClass.indexOf("cl") > 0) {
            $('#modalpopup_ShowThongBaoLo').show();
        }
    }

    self.clostShowTBLo = function () {
        $('#modalpopup_ShowThongBaoLo').hide();
        self.CapNhatQuanLyTheoLo(false);
        self.ThietLap().LoHang(true);
        $('#cbLoHang').addClass('cl');
    }
    self.DongYTBLo = function () {
        $('#modalpopup_ShowThongBaoLo').hide();
        self.CapNhatQuanLyTheoLo(false);
        self.ThietLap().LoHang(true);
        $('#cbLoHang').addClass('cl');
    }

    self.LuuThongTin = function (formElement) {
        var _id = self.newThongTinCH().ID();
        var _website = self.newThongTinCH().Website();
        var _tencongty = self.newThongTinCH().TenCongTy();
        var _diachi = self.newThongTinCH().DiaChi();
        var _sodienthoai = self.newThongTinCH().SoDienThoai();
        var _logo = self.newThongTinCH().DiaChiNganHang();
        var myData = {};
        var ThongTinCH = {
            ID: _id,
            TenCongTy: _tencongty,
            DiaChi: _diachi,
            SoDienThoai: _sodienthoai,
            DiaChiNganHang: _logo,
            Website: _website,
            NgayCongChuan: self.newThongTinCH().NgayCongChuan(),
        }
        myData.objThongTinCH = ThongTinCH;
        $.ajax({
            data: myData,
            url: ThietLapUri + "Put_HT_ThongTinCuaHang/",
            type: 'PUT',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",

            success: function (item) {
                self.InsertImage(item.ID);
                self.LuuThongTinCHLS();
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật thông tin cửa hàng thành công", "success");
            },
            statusCode: {
                404: function () {
                    self.error("page not found");
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cập nhật thông tin cửa hàng thất bại", "danger");
            },
            complete: function () {
            }
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
            });
    }

    self.LuuThongTinCHLS = function () {
        var objDiary = {
            ID_NhanVien: _IDNhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Thiết lập cửa hàng",
            NoiDung: "Cập nhật thông tin cửa hàng",
            NoiDungChiTiet: "Cập nhật thông tin cửa hàng",
            LoaiNhatKy: 2 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        saveDiary(objDiary);
    }

    self.LuuTinhNangLS = function (item) {
        var objDiary = {
            ID_NhanVien: _IDNhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Thiết lập cửa hàng",
            NoiDung: "Cập nhật thông tin thiết lập tính năng: - Bán và chuyển khi hàng đã đặt: " + (item.BanVaChuyenKhiHangDaDat === true ? "có" : "không") + "- Sản phẩm có đơn vị tính: " + (item.CoDonViTinh === true ? "có" : "không") + "- Tính năng đặt hàng: " + (item.DatHang === true ? "có" : "không") + "- Cho phép đặt hàng khi hết tồn kho: " + (item.DatHangXuatAm === true ? "có" : "không") + "- Giá vốn trung bình: " + (item.GiaVonTrungBinh === true ? "có" : "không") + "- Giới hạn thời gian trả hàng: " + (item.GioiHanThoiGianTraHang === true ? "có" : "không") +
                "- Cho phép in báo giá khi bán hàng: " + (item.InBaoGiaKhiBanHang === true ? "có" : "không") + "- Tính năng khóa sổ: " + (item.KhoaSo === true ? "có" : "không") + "- Sự dụng tính năng khuyến mại: " + (item.KhuyenMai === true ? "có" : "không") + "- Tính năng quản lý khách hàng theo chi nhánh: " + (item.QuanLyKhachHangTheoDonVi === true ? "có" : "không") + "- Sản phẩm có thuộc tính: " + (item.SanPhamCoThuocTinh === true ? "có" : "không") + "- Hiển thị tổng số lượng hàng hóa trên chứng từ: " + (item.SoLuongTrenChungTu === true ? "có" : "không") +
                "- Sử dụng cân điện tử: " + (item.SuDungCanDienTu === true ? "có" : "không") + "- Tính năng sản xuất hàng hóa: " + (item.TinhNangSanXuatHangHoa === true ? "có" : "không") + "- Tính năng tích điểm: " + (item.TinhNangTichDiem === true ? "có" : "không") + "- Bán & Chuyển hàng, Trả hàng nhập khi hết hàng tồn kho: " + (item.XuatAm === true ? "có" : "không") + "- Quản lý theo lô hàng: " + (item.LoHang === true ? "có" : "không"),
            NoiDungChiTiet: "Cập nhật thông tin thiết lập tính năng:</br> - Bán và chuyển khi hàng đã đặt: " + (item.BanVaChuyenKhiHangDaDat === true ? "có" : "không") + "</br>- Sản phẩm có đơn vị tính: " + (item.CoDonViTinh === true ? "có" : "không") + "</br>- Tính năng đặt hàng: " + (item.DatHang === true ? "có" : "không") + "</br>- Cho phép đặt hàng khi hết tồn kho: " + (item.DatHangXuatAm === true ? "có" : "không") + "</br>- Giá vốn trung bình: " + (item.GiaVonTrungBinh === true ? "có" : "không") + "</br>- Giới hạn thời gian trả hàng: " + (item.GioiHanThoiGianTraHang === true ? "có" : "không") +
                "</br>- Cho phép in báo giá khi bán hàng: " + (item.InBaoGiaKhiBanHang === true ? "có" : "không") + "</br>- Tính năng khóa sổ: " + (item.KhoaSo === true ? "có" : "không") + "</br>- Sự dụng tính năng khuyến mại: " + (item.KhuyenMai === true ? "có" : "không") + "</br>- Tính năng quản lý khách hàng theo chi nhánh: " + (item.QuanLyKhachHangTheoDonVi === true ? "có" : "không") + "</br>- Sản phẩm có thuộc tính: " + (item.SanPhamCoThuocTinh === true ? "có" : "không") + "</br>- Hiển thị tổng số lượng hàng hóa trên chứng từ: " + (item.SoLuongTrenChungTu === true ? "có" : "không") +
                "</br>- Sử dụng cân điện tử: " + (item.SuDungCanDienTu === true ? "có" : "không") + "</br>- Tính năng sản xuất hàng hóa: " + (item.TinhNangSanXuatHangHoa === true ? "có" : "không") + "</br>- Tính năng tích điểm: " + (item.TinhNangTichDiem === true ? "có" : "không") + "</br>- Bán & Chuyển hàng, Trả hàng nhập khi hết hàng tồn kho: " + (item.XuatAm === true ? "có" : "không") + "- Quản lý theo lô hàng: " + (item.LoHang === true ? "có" : "không"),
            LoaiNhatKy: 2 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        saveDiary(objDiary);
    }

    self.files = ko.observableArray([]);
    self.fileSelect = function (elemet, event) {
        var files = event.target.files;// FileList object
        // Loop through the FileList and render image files as thumbnails.
        for (var i = 0; i < files.length; i++) {
            var f = files[i];
            // Only process image files.
            if (!f.type.match('image.*')) {
                continue;
            }
            var size = parseFloat(f.size / 1024).toFixed(2);
            if (size > 2048) {
                //$('#blahHH1').show();
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Dung lượng file không lớn quá 2 Mb", "danger");
            }
            if (size <= 2048) {
                $('#blahHH1').hide();
                var reader = new FileReader();
                // Closure to capture the file information.
                reader.onload = (function (theFile) {
                    return function (e) {
                        self.files([]);
                        self.files.push(new FileModel(theFile, e.target.result));
                    };
                })(f);
                // Read in the image file as a data URL.
                reader.readAsDataURL(f);
            }
        }
    };

    self.InsertImage = function (id) {
        //self.files(self.DM_HangHoa_Anh());
        //for (var i = 0; i < self.files().length; i++) {
        var i = 0;
        if (i < self.files().length) {
            var formData = new FormData();
            formData.append("files", self.files()[i].file);
            //$.ajax({
            //    type: "POST",
            //    url: ThietLapUri + "ImageUpload/" + id,
            //    data: formData,
            //    dataType: 'json',
            //    contentType: false,
            //    processData: false,
            //    success: function (response) {
            //        console.log('Thành công: ' + response);
            //        
            //    },
            //    error: function (jqXHR, textStatus, errorThrown) {
            //        console.log('err');
            //    }
            //});
            let myData = {};
            myData.Subdomain = VHeader.SubDomain;
            myData.Function = "7"; //7. Công ty
            myData.Id = id;
            myData.files = formData;
            var result = Open24FileManager.UploadImage(myData);
            if (result.length > 0) {
                $.ajax({
                    url: ThietLapUri + "UpdateAnhCongTy?id=" + id,
                    type: "POST",
                    data: JSON.stringify(result),
                    contentType: "application/json",
                    dataType: "JSON",
                    success: function (data, textStatus, jqXHR) {
                        if (self.newThongTinCH().DiaChiNganHang() !== "") {
                            Open24FileManager.RemoveFiles([self.newThongTinCH().DiaChiNganHang()]);
                        }
                        location.reload();
                    },
                    error: function (jqXHR, textStatus, errorThrown) {

                    }
                });
            }
        }
    }

    self.DonVis = ko.observableArray();
    self.chotso_ArrDonVi = ko.observableArray();
    self.chotso_txtSearch = ko.observable();

    function getListDonVis() {
        arrIDHang = [];
        ajaxHelper(DonViUri + "ChotSo_GetListDonVi", "GET").done(function (data) {
            self.DonVis(data);
            self.chotso_ArrDonVi(data);
            arrIDHang = data.filter(x => x.KhoaSo && x.NgayChotSo !== null).map(function (x) {
                return x.ID;
            })
            $(function () {
                $('.datetimepicker2').datetimepicker({
                    format: 'DD/MM/YYYY'
                });
                $('#datetimepicker2').datetimepicker({
                    format: 'DD/MM/YYYY'
                });
                $(".date input").click(function () {
                    $(this).parent(".date").next(".apdung").toggle();
                    $(".apdung").mouseup(function () {
                        return false
                    });
                    $(".date input").mouseup(function () {
                        return false
                    });
                    $(document).mouseup(function () {
                        $(".apdung").hide();
                    });
                });
            });
        });
    }
    getListDonVis();
    //trinhpv chốt sổ
    $('#txtDate').on('dp.change', function (e) {
        thisDate = $(this).val();
        var t = thisDate.split(" ");
        var t1 = t[0].split("/").reverse().join("-")
        thisDate = moment(t1).format('MM/DD/YYYY')
        //console.log(thisDate);
        _timeStart = moment(new Date(thisDate)).format('YYYY-MM-DD');
        var dt = new Date(thisDate);
        _timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
        console.log(_timeEnd)
    });

    self.chotso_SearchChiNhanh = function () {
        var arr = [];
        if (commonStatisJs.CheckNull(self.chotso_txtSearch())) {
            arr = self.DonVis();
        }
        else {
            var txt = locdau(self.chotso_txtSearch());
            arr = self.DonVis().filter(e =>
                locdau(e.TenDonVi).indexOf(txt) >= 0
                || locdau(e.MaDonVi).indexOf(txt) >= 0
            );
        }
        self.chotso_ArrDonVi(arr);
    }

    self.checkAll_DonVi = function (obj) {
        console.log("a")
        if ($(obj).is(':checked')) {
            for (var i = 0; i < self.DonVis().length; i++) {
                var $thisip = $(self.DonVis()[i].ID);
                $thisip.is(':checked')
            }
        }
    };

    self.clickDateChotSo = function (item) {
        var $this = event.currentTarget;
        $($this).datetimepicker(
            {
                format: "d/m/Y",
                timepicker: false,
                defaultDate: new Date(),
                mask: true,
                maxDate: new Date(),
            });
    }

    self.DonVi_ChotSo = ko.observableArray();
    self.save_ChotSo = function () {
        self.DonVi_ChotSo([])
        if (arrIDHang.length < 1) {
            bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Bạn cần lựa chọn chi nhánh cần khóa sổ", "danger");
        }
        else {
            var dt = new Date();
            let sNKy = '';
            for (var i = 0; i < self.DonVis().length; i++) {
                var $this = $('#input_' + self.DonVis()[i].ID);
                for (var j = 0; j < arrIDHang.length; j++) {
                    if (arrIDHang[j] === self.DonVis()[i].ID) {
                        var thisDate = $this.val();
                        var t = thisDate.split(" ");
                        var t1 = t[0].split("/").reverse().join("-")
                        thisDate = moment(t1).format('MM/DD/YYYY')
                        _timeStart = moment(new Date(thisDate)).format('YYYY-MM-DD');
                        var _timeEnd = moment(new Date(dt)).format('YYYY-MM-DD');
                        if (new Date(_timeStart) > new Date(_timeEnd) || thisDate === "Invalid date") {
                            bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Ngày chốt sổ không được để trống và không được lớn hơn ngày hiện tại", "danger");
                            return false;
                        }
                        else {
                            var obj =
                            {
                                ID_DonVi: self.DonVis()[i].ID,
                                NgayChotSo: _timeStart
                            }
                            self.DonVi_ChotSo.push(obj);
                            sNKy += '<br />'.concat(self.DonVis()[i].TenDonVi, ' - ', moment(_timeStart, 'YYYY-MM-DD').format('DD/MM/YYYY'));
                        }
                    }
                }
            }
            var myData = {};
            myData.objChotSo = self.DonVi_ChotSo();
            callAjaxAdd(myData, sNKy); //insert chốt sổ
        }
        //console.log(self.DonVi_ChotSo());
    }
    var BH_KhuyenMaiUri = '/api/DanhMuc/BH_KhuyenMaiAPI/';
    function callAjaxAdd(myData, sNKy) {
        $.ajax({
            data: myData,
            url: BH_KhuyenMaiUri + "PostBH_ChotSo",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (item) {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Thực hiện khóa sổ thành công", "success");
                $('#datail-TH4').modal('hide');
                callAjaxAdd_ChotSo_ChiTiet();

                let diary = {
                    ID_DonVi: VHeader.IdDonVi,
                    ID_NhanVien: VHeader.IdNhanVien,
                    LoaiNhatKy: 1,
                    ChucNang: 'Thiết lập cửa hàng - Khóa sổ',
                    NoiDung: 'Thiết lập tính năng khóa sổ chi nhánh',
                    NoiDungChiTiet: 'Các chi nhánh khóa sổ gồm: '.concat(sNKy)
                }
                Insert_NhatKyThaoTac_1Param(diary);
            },
            statusCode: {
                404: function () {
                    self.error("page not found");
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Thực hiện khóa sổ không thành công", "danger");
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
            },
            complete: function (item) {
            }
        })
    }

    function callAjaxAdd_ChotSo_ChiTiet() {
        var myData = {};
        myData.objChotSo_ChiTiet = self.DonVi_ChotSo();
        $.ajax({
            data: myData,
            url: BH_KhuyenMaiUri + "PostBH_ChotSoChiTiet",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (item) {
            },
            error: function (jqXHR, textStatus, errorThrown) {
            },
            complete: function (item) {
            }
        })
    }
    self.SelectDonVi = function (item) {
        console.log(item.ID);
        for (var i = 0; i < self.DonVis().length; i++) {
            console.log(self.DonVis()[i].ID, item.ID)
            if (self.DonVis()[i].ID === item.ID) {
                var $this = $('#input_' + self.DonVis()[i].ID);
            }
        }
    }

    //SMS tính năng
    self.KichHoatSms = function () {
        ajaxHelper('/api/DanhMuc/ThietLapApi/' + 'CheckKichHoatSMS', 'GET').done(function (data) {
            if (data) {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Kích hoạt tính nắng SMS thành công", "success");
                $('.popup-active-sms').show();
                $('.img-active-sms').hide();
                location.reload();
            }
            else {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Kích hoạt tính nắng SMS thất bại", "danger");
                $('.popup-active-sms').hide();
                $('.img-active-sms').show();
                location.reload();
            }
        });
    }
    self.HuyKichHoatSms = function () {
        ajaxHelper('/api/DanhMuc/ThietLapApi/' + 'HuyKichHoatSMS', 'GET').done(function (data) {
            if (data) {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Hủy kích hoạt tính nắng SMS thành công", "success");
                $('.popup-active-sms').hide();
                $('.img-active-sms').show();
                location.reload();
            }
            else {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Hủy kích hoạt tính nắng SMS thất bại", "danger");
                $('.popup-active-sms').show();
                $('.img-active-sms').hide();
                location.reload();
            }
        });
    }
    //$('#CheckKichHoatSMS').click(function () { 
    //    var check = $('#CheckKichHoatSMS').is(":checked");
    //    if (check) {
    //        ajaxHelper('/api/DanhMuc/ThietLapApi/' + 'CheckKichHoatSMS', 'GET').done(function (data) {
    //            if (data) {
    //                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Kích hoạt tính nắng SMS thành công", "success");
    //                $('.tab-detail-table-sms').show();
    //            }
    //            else {
    //                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Kích hoạt tính nắng SMS thất bại", "danger");
    //                $('.tab-detail-table-sms').hide();
    //            }
    //        });
    //    }
    //    else {
    //        ajaxHelper('/api/DanhMuc/ThietLapApi/' + 'HuyKichHoatSMS', 'GET').done(function (data) {
    //            if (data) {
    //                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Hủy kích hoạt tính nắng SMS thành công", "success");
    //                $('.tab-detail-table-sms').hide();
    //            }
    //            else {
    //                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Hủy kích hoạt tính nắng SMS thất bại", "danger");
    //                $('.tab-detail-table-sms').show();
    //            }
    //        });
    //    }
    //});

    self.booleadAddBrand = ko.observable(true);
    self.IDBrandName = ko.observable();
    self.clickShowBrandName = function () {
        self.booleadAddBrand(true);
        self.IDBrandName('00000000-0000-0000-0000-000000000000');
        $('.txtEditThemBrand').html("Thêm mới BrandName");
        $('#txtTenBrandName').val("");
        $('#txtGhiChuBrand').val("");
        $('#exampleModal').modal('show');
        $('#exampleModal').on('shown.bs.modal', function () {
            $('#txtTenBrandName').focus();
            $('#txtTenBrandName').prop('disabled', false);
        });
    };

    var userLogin = $('#txtTenTaiKhoan').text();
    self.ThemMoiBrandName = function () {
        document.getElementById("btnThemMoiBrand").disabled = true;
        document.getElementById("btnThemMoiBrand").lastChild.data = " Đang lưu";
        var tenBrandName = $('#txtTenBrandName').val();
        var ghiChu = $('#txtGhiChuBrand').val();
        var _id = self.IDBrandName();
        if (tenBrandName === "" || tenBrandName === null || tenBrandName === undefined) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng nhập tên BrandName", "danger");
            document.getElementById("btnThemMoiBrand").disabled = false;
            document.getElementById("btnThemMoiBrand").lastChild.data = " Lưu";
            return false;
        }

        var specialChars = "<>!#$%^&*()+[]{}?:;|'\"\\,/~`=' '";
        var check = function (string) {
            for (i = 0; i < specialChars.length; i++) {
                if (string.indexOf(specialChars[i]) > -1) {
                    return true;
                }
            }
            document.getElementById("btnThemMoiBrand").disabled = false;
            document.getElementById("btnThemMoiBrand").lastChild.data = " Lưu";
            return false;
        };
        if (check($('#txtTenBrandName').val()) === false) {
        }
        else {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Tên BrandName không được chứa kí tự đặc biệt", "danger");
            $('#txtTenBrandName').focus();
            document.getElementById("btnThemMoiBrand").disabled = false;
            document.getElementById("btnThemMoiBrand").lastChild.data = " Lưu";
            return false;
        }

        var myData = {};
        var objBrand = {
            ID: _id,
            Name: tenBrandName,
            Note: ghiChu
        };
        myData.objBrandName = objBrand;
        if (self.booleadAddBrand() === true) {
            ajaxHelper('api/DanhMuc/ThietLapApi/' + 'CheckBrandNameExist?nameBrand=' + tenBrandName, 'GET').done(function (data) {
                if (data) {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Tên BrandName đã tồn tại", "danger");
                    $('#txtTenBrandName').select();
                    document.getElementById("btnThemMoiBrand").disabled = false;
                    document.getElementById("btnThemMoiBrand").lastChild.data = " Lưu";
                    return false;
                }
                else {
                    $.ajax({
                        data: myData,
                        url: '/api/DanhMuc/ThietLapApi/' + "PostBrandName",
                        type: 'POST',
                        async: true,
                        dataType: 'json',
                        contentType: "application/x-www-form-urlencoded; charset=UTF-8",

                        success: function (item) {
                            bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Thêm BrandName thành công", "success");
                        },
                        statusCode: {
                            404: function () {
                                self.error("page not found");
                            },
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Thêm BrandName thất bại", "danger");
                        },
                        complete: function (item) {
                            $("#exampleModal").modal("hide");
                            document.getElementById("btnThemMoiBrand").disabled = false;
                            document.getElementById("btnThemMoiBrand").lastChild.data = " Lưu";
                            getallBrandName();
                        }
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                            self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                        });
                }
            });
        }
        else {
            ajaxHelper('api/DanhMuc/ThietLapApi/' + 'CheckBrandNameExistEdit?nameBrand=' + tenBrandName + '&id=' + _id, 'GET').done(function (data) {
                if (data) {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Tên BrandName đã tồn tại", "danger");
                    $('#txtTenBrandName').select();
                    document.getElementById("btnThemMoiBrand").disabled = false;
                    document.getElementById("btnThemMoiBrand").lastChild.data = " Lưu";
                    return false;
                }
                else {
                    $.ajax({
                        data: myData,
                        url: '/api/DanhMuc/ThietLapApi/' + "PutBrandName",
                        type: 'POST',
                        async: true,
                        dataType: 'json',
                        contentType: "application/x-www-form-urlencoded; charset=UTF-8",

                        success: function (item) {
                            getallBrandName();
                            bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật BrandName thành công", "success");
                        },
                        statusCode: {
                            404: function () {
                                self.error("page not found");
                            },
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cập nhật BrandName thất bại", "danger");
                        },
                        complete: function (item) {
                            document.getElementById("btnThemMoiBrand").disabled = false;
                            document.getElementById("btnThemMoiBrand").lastChild.data = " Lưu";
                            $("#exampleModal").modal("hide");
                        }
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                            self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                        });
                }
            });
        }
    };

    self.EditBrandName = function (item) {
        ajaxHelper('api/DanhMuc/ThietLapApi/' + 'GetBrandNameById?id=' + item.ID, 'GET').done(function (data) {
            $('#exampleModal').modal('show');
            $('.txtEditThemBrand').html("Cập nhật BrandName");
            self.booleadAddBrand(false);
            $('#exampleModal').on('shown.bs.modal', function () {
                if (self.booleadAddBrand() === false) {
                    $('#txtTenBrandName').val(data.BrandName);
                    if (data.Status === 1) {
                        $('#txtTenBrandName').prop('disabled', true);
                    }
                    else {
                        $('#txtTenBrandName').prop('disabled', false);
                    }
                    $('#txtGhiChuBrand').val(data.GhiChu);
                    $('#txtTenBrandName').select();
                    self.IDBrandName(data.ID);
                }
            });
        });
    };

    self.DeleteBrandName = function (item) {
        ajaxHelper('api/DanhMuc/ThietLapApi/' + 'DeleteBrandName?id=' + item.ID, 'GET').done(function (data) {
            bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật BrandName thành công", "success");
            getallBrandName();
        })
    };

    self.BrandNames = ko.observableArray();
    function getallBrandName() {
        ajaxHelper('api/DanhMuc/ThietLapApi/' + 'GetallBrandName', 'GET').done(function (data) {
            self.BrandNames(data);
        });
    };
    getallBrandName();

    // tin mẫu
    self.LoaiTins = ko.observableArray();

    self.selectedLoaiTin = ko.observable();

    self.selectedLoaiTin.subscribe(function () {

    });

    function GetListLoaiTinSMS() {
        ajaxHelper('/api/DanhMuc/ThietLapAPI/' + 'GetListLoaiTinSMS', 'GET').done(function (data) {
            var arr = data.map(function (x) {
                return {
                    value: x.ID,
                    TenLoai: x.Name,
                }
            })
            self.LoaiTins(arr);
        })
    }
    GetListLoaiTinSMS();

    self.booleadAddMauTin = ko.observable(true);
    self.IDMauTin = ko.observable();
    self.clickShowThemTinMau = function () {
        $('#exampleModalMauTin').modal('show');
        self.IDMauTin('00000000-0000-0000-0000-000000000000');
        self.booleadAddMauTin(true);
        self.selectedLoaiTin(undefined);
        $('.txtEditThemMauTin').html("Thêm mới mẫu tin");
        $('#exampleModalMauTin').on('shown.bs.modal', function () {
            if (self.booleadAddMauTin() === true) {
                $('#txtNoiDungMauTin').val("");
                $('#txtLoaiTin').val("");
                $('#checkLaMacDinh').prop('checked', true);
            }
        });
    };

    self.AddMauTin = function () {
        var _loaiTin = self.selectedLoaiTin();
        var _noiDungTin = $('#txtNoiDungMauTin').val();
        var _id = self.IDMauTin();
        var _laMacDinh = $('#checkLaMacDinh').is(":checked");

        if (_loaiTin === "" || _loaiTin === null || _loaiTin === undefined) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng chọn loại tin nhắn", "danger");
            return false;
        }

        if (_noiDungTin === "" || _noiDungTin === null || _noiDungTin === undefined) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng nhập nội dung tin mẫu", "danger");
            return false;
        }

        var obj = {
            ID: _id,
            NoiDung: _noiDungTin,
            LoaiTin: _loaiTin,
            LaMacDinh: _laMacDinh,
            NguoiTao: userLogin
        };

        var myData = {};
        myData.objMauTin = obj;

        //myData.objMauTin = obj;
        console.log(333, myData)
        if (self.booleadAddMauTin() === true) {
            $.ajax({
                data: myData,
                url: '/api/DanhMuc/ThietLapApi/' + "PostMauTin",
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",

                success: function (item) {
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Thêm mẫu tin thành công", "success");
                },
                statusCode: {
                    404: function () {
                        self.error("page not found");
                    },
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Thêm mẫu tin thất bại", "danger");
                },
                complete: function (item) {
                    $("#exampleModalMauTin").modal("hide");
                    getallMauTin();
                }
            })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                });
        }
        else {
            $.ajax({
                data: myData,
                url: '/api/DanhMuc/ThietLapApi/' + "PutMauTin",
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",

                success: function (item) {
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "cập nhật mẫu tin thành công", "success");
                },
                statusCode: {
                    404: function () {
                        self.error("page not found");
                    },
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cập nhật mẫu tin thất bại", "danger");
                },
                complete: function (item) {
                    $("#exampleModalMauTin").modal("hide");
                    getallMauTin();
                }
            })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                });
        }
    }


    self.MauTins = ko.observableArray();
    function getallMauTin() {
        ajaxHelper('api/DanhMuc/ThietLapApi/' + 'GetAllMauTin', 'GET').done(function (data) {
            self.MauTins(data);
        });
    };
    getallMauTin();

    self.EditMauTin = function (item) {
        ajaxHelper('api/DanhMuc/ThietLapApi/' + 'GetMauTinByID?id=' + item.ID, 'GET').done(function (data) {
            $('#exampleModalMauTin').modal('show');
            $('.txtEditThemMauTin').html("Cập nhật mẫu tin");
            self.booleadAddMauTin(false);
            $('#exampleModalMauTin').on('shown.bs.modal', function () {
                if (self.booleadAddMauTin() === false) {
                    $('#txtNoiDungMauTin').val(data.NoiDungTin);
                    self.selectedLoaiTin(data.LoaiTin);
                    if (data.LaMacDinh === true) {
                        $('#checkLaMacDinh').prop('checked', true);
                    }
                    else {
                        $('#checkLaMacDinh').prop('checked', false);
                    }
                    self.IDMauTin(data.ID);
                }
            });
        });
    };

    self.XoaMauTin = function (item) {
        ajaxHelper('api/DanhMuc/ThietLapApi/' + 'DeleteMauTin?id=' + item.ID, 'GET').done(function (data) {
            bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Xóa mẫu tin thành công", "success");
            getallMauTin();
        });
    };
    // TuanDl 
    self.ListMauIn = ko.observableArray();
    self.ListMauInPTN = ko.observableArray();
    self.ListMauInDathang = ko.observableArray();
    self.ListMauInHoaDon = ko.observableArray();
    self.ListMauInGoiDV = ko.observableArray();
    self.ListMauInTraHang = ko.observableArray();
    self.ListMauInDoiTraHang = ko.observableArray();
    self.ListMauInNhapHang = ko.observableArray();
    self.ListMauInTraHangNhap = ko.observableArray();
    self.ListMauInChuyenHang = ko.observableArray();
    self.ListMauInPhieuThu = ko.observableArray();
    self.ListMauInPhieuChi = ko.observableArray();
    self.ListMauInXuatHuy = ko.observableArray();
    self.ListMauInDieuChinh = ko.observableArray();
    self.ListMauInTheGiaTri = ko.observableArray();
    self.ListMauInPhieuLuong = ko.observableArray();

    function loadMauIn() {
        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/LoadListMauIn',
            type: 'GET',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                $.each(result, function (index, value) {
                    switch (value.Key) {
                        case KeyPhieuTiepNhan:
                            self.ListMauInPTN(value.ListSelectMauIn);
                            $('#PhieuTiepNhan').val(value.selected);
                            break;
                        case KeyDatHang:
                            self.ListMauInDathang(value.ListSelectMauIn);
                            $('#DatHang').val(value.selected);
                            break;
                        case KeyHoaDon:
                            self.ListMauInHoaDon(value.ListSelectMauIn);
                            $('#HoaDon').val(value.selected);
                            break;
                        case KeyGoiDichVu:
                            self.ListMauInGoiDV(value.ListSelectMauIn);
                            $('#GoiDichVu').val(value.selected);
                            break;
                        case KeyTraHang:
                            self.ListMauInTraHang(value.ListSelectMauIn);
                            $('#TraHang').val(value.selected);
                            break;
                        case KeyDoiTraHang:
                            self.ListMauInDoiTraHang(value.ListSelectMauIn);
                            $('#DoiTraHang').val(value.selected);
                            break;
                        case KeyTheGiaTri:
                            self.ListMauInTheGiaTri(value.ListSelectMauIn);
                            $('#TheGiaTri').val(value.selected);
                            break;
                        case KeyNhapHang:
                            self.ListMauInNhapHang(value.ListSelectMauIn);
                            $('#NhapHang').val(value.selected);
                            break;
                        case KeyTraHangNhap:
                            self.ListMauInTraHangNhap(value.ListSelectMauIn);
                            $('#TraHangNhap1').val(value.selected);
                            break;
                        case KeyChuyenHang:
                            self.ListMauInChuyenHang(value.ListSelectMauIn);
                            $('#ChuyenHang').val(value.selected);
                            break;
                        case KeyPhieuThu:
                            self.ListMauInPhieuThu(value.ListSelectMauIn);
                            $('#PhieuThu').val(value.selected);
                            break;
                        case KeyPhieuChi:
                            self.ListMauInPhieuChi(value.ListSelectMauIn);
                            $('#PhieuChi').val(value.selected);
                            break;
                        case KeyXuatHuy:
                            self.ListMauInXuatHuy(value.ListSelectMauIn);
                            $('#XuatHuy').val(value.selected);
                            break;
                        case KeyDieuChinh:
                            self.ListMauInDieuChinh(value.ListSelectMauIn);
                            $('#DieuChinh').val(value.selected);
                            break;
                        case KeyPhieuLuong:
                            self.ListMauInPhieuLuong(value.ListSelectMauIn);
                            $('#PhieuLuong').val(value.selected);
                            break;
                    }
                });
            }
        });
    }
    loadMauIn();
    self.UpdateSelectDefaultMauIn = function () {

        var model = [{
            MaChungTu: KeyDatHang,
            MauInID: $('#DatHang').val()
        },
        {
            MaChungTu: KeyHoaDon,
            MauInID: $('#HoaDon').val()
        },
        {
            MaChungTu: KeyGoiDichVu,
            MauInID: $('#GoiDichVu').val()
        },
        {
            MaChungTu: KeyTraHang,
            MauInID: $('#TraHang').val()
        },
        {
            MaChungTu: KeyDoiTraHang,
            MauInID: $('#DoiTraHang').val()
        },
        {
            MaChungTu: KeyNhapHang,
            MauInID: $('#NhapHang').val()
        },
        {
            MaChungTu: KeyTraHangNhap,
            MauInID: $('#TraHangNhap1').val()
        },
        {
            MaChungTu: KeyChuyenHang,
            MauInID: $('#ChuyenHang').val()
        },
        {
            MaChungTu: KeyPhieuThu,
            MauInID: $('#PhieuThu').val()
        },
        {
            MaChungTu: KeyPhieuChi,
            MauInID: $('#PhieuChi').val()
        },
        {
            MaChungTu: KeyXuatHuy,
            MauInID: $('#XuatHuy').val()
        },
        {
            MaChungTu: KeyDieuChinh,
            MauInID: $('#DieuChinh').val()
        },
        {
            MaChungTu: KeyTheGiaTri,
            MauInID: $('#TheGiaTri').val()
        },
        {
            MaChungTu: KeyPhieuLuong,
            MauInID: $('#PhieuLuong').val()
        },
        {
            MaChungTu: KeyPhieuTiepNhan,
            MauInID: $('#PhieuTiepNhan').val()
        }
        ];
        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/UpdateDefaultMauIn',
            type: 'POST',
            data: JSON.stringify(model),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                if (result.res === true) {
                    $('#datail-TH7').modal('hide');
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + result.mess, 'success');
                }
                else {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + result.mess, 'danger');
                    console.log(log);
                }
            }
        });
    }

    // thiet lap machungtu
    var _machinhanh = '';
    var today = new Date();
    var date = today.getDate();
    var month = today.getMonth() + 1;  // because month from  0 - 11
    var year = today.getFullYear().toString();
    if (date < 10) {
        date = '0' + date;
    }
    if (month < 10) {
        month = '0' + month;
    }
    function GetMaChiNhanh_byID() {
        ajaxHelper('/api/DanhMuc/DM_DonViAPI/GetMaChiNhanh_byID?idDonVi=' + _IDchinhanh, 'GET').done(function (data) {
            if (data != null) {
                _machinhanh = data;
                for (var i = 0; i < modelMaChungTu.TblSetup().length; i++) {
                    modelMaChungTu.TblSetup()[i].MaChiNhanh = data;
                }
                modelMaChungTu.TblSetup($.extend(true, [], modelMaChungTu.TblSetup()))
            }
        })
    }
    GetMaChiNhanh_byID();

    function GetSTT(dodai) {
        let stt = '';
        if (dodai !== 0) {
            for (var i = 0; i < dodai - 1; i++) {
                stt = '0' + stt;
            }
            stt = stt + '1';
        }
        return stt;
    }

    self.IsGara = ko.observable(VHeader.IdNganhNgheKinhDoanh === 'C16EDDA0-F6D0-43E1-A469-844FAB143014');

    self.showThietLapMaChungTu = function () {
        // check exits ChiNhanh has MaChiNhanh= null or empty
        ajaxHelper('/api/DanhMuc/DM_DonViAPI/Check_MaChiNhanhEmpty').done(function (data) {
            if (data === false) {
                if (self.ThietLap().SuDungMaChungTu() == 1) {
                    // only get data if was setup SuDungMaChungTu = 1
                    ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/Get_HTMaChungTu').done(function (x) {
                        if (x.res === true) {
                            var data = x.data;
                            if (data.length > 0) {
                                modelMaChungTu.isUpdate(true);
                                var arrKiTuNganCach = modelMaChungTu.KiTuNganCachs();
                                var arrNgayThangNam = modelMaChungTu.NgayThangNams();

                                let arrID = [];
                                for (var i = 0; i < data.length; i++) {
                                    arrID.push(data[i].ID_LoaiChungTu);
                                }

                                var arrNotSet = $.grep(modelMaChungTu.TblSetup(), function (x) {
                                    return $.inArray(x.ID_LoaiChungTu, arrID) === -1;
                                });

                                var arrSet = [];
                                for (var j = 0; j < data.length; j++) {
                                    let itemFor = data[j];
                                    itemFor.KiTuNganCachs = arrKiTuNganCach;
                                    itemFor.NgayThangNams = arrNgayThangNam;

                                    switch (itemFor.ID_LoaiChungTu) {
                                        case 33:
                                            itemFor.MaLoaiChungTu = '';
                                            break;
                                        case 34:
                                            itemFor.MaLoaiChungTu = '';
                                            break;
                                    }

                                    // concat string --> Mau
                                    let sDate = '';
                                    let machinhanh = '';
                                    let kitu1 = '';
                                    if (itemFor.SuDungMaDonVi) {
                                        machinhanh = _machinhanh;
                                        kitu1 = itemFor.KiTuNganCach1;
                                    }
                                    itemFor.MaChiNhanh = _machinhanh;
                                    switch (itemFor.NgayThangNam) {
                                        case 'ddMMyyyy':
                                            sDate = sDate.concat(date, month, year);
                                            break;
                                        case 'ddMMyy':
                                            sDate = sDate.concat(date, month, year.substr(2, 2));
                                            break;
                                        case 'MMyy':
                                            sDate = sDate.concat(month, year.substr(2, 2));
                                            break;
                                        case 'MMyyyy':
                                            sDate = sDate.concat(month, year);
                                            break;
                                        case 'yyyyMMdd':
                                            sDate = sDate.concat(year, month, date);
                                            break;
                                        case 'yyMMdd':
                                            sDate = sDate.concat(year.substr(2, 2), month, date);
                                            break;
                                        case 'yyMM':
                                            sDate = sDate.concat(year.substr(2, 2), month);
                                            break;
                                        case 'yyyyMM':
                                            sDate = sDate.concat(year, month);
                                            break;
                                        case 'yyyy':
                                            sDate = year;
                                            break;
                                    }
                                    itemFor.GiaTriNgay = sDate;
                                    let stt = GetSTT(itemFor.DoDaiSTT);
                                    itemFor.Mau = machinhanh.concat(kitu1, itemFor.MaLoaiChungTu, itemFor.KiTuNganCach2, sDate, itemFor.KiTuNganCach3, stt);
                                    arrSet.push(itemFor);
                                }
                                // push if not setup
                                for (var i = 0; i < arrNotSet.length; i++) {
                                    arrSet.push(arrNotSet[i]);
                                }

                                if (!self.IsGara()) {
                                    arrSet = $.grep(arrSet, function (x) {
                                        return x.ID_LoaiChungTu !== 25;
                                    })
                                }
                                // sort by ID_LoaiChungTu
                                var arrSort = arrSet.sort(function (a, b) {
                                    let x = a.ID_LoaiChungTu, y = b.ID_LoaiChungTu;
                                    return x > y ? 1 : x < y ? -1 : 0;
                                });
                                modelMaChungTu.TblSetup($.extend(true, [], arrSort))
                            }
                            else {
                                modelMaChungTu.isUpdate(false);
                            }
                            $('#mdThietLapMaChungTu').modal('show');
                        }
                        else {
                            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + x.mes, 'danger');
                        }
                    })
                }
                else {
                    $('#mdThietLapMaChungTu').modal('show');
                }
            }
            else {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + 'Vui lòng cập nhật lại mã chi nhánh đang rỗng để thực hiện chức năng này', 'danger');
            }
        })
    }

    //ZaloApp
    self.zaloHref = ko.observable("#");
    self.getZaloConnectHref = function () {
        ajaxHelper("/api/DanhMuc/HT_API/getZaloCodeChallenge").done(function (data) {
            if (data.res === true) {
                let hreftemp = "https://oauth.zaloapp.com/v4/oa/permission?app_id=4340987146639588225&redirect_uri=https%3A%2F%2Fopen24.vn%2Fhome%2Fzalologin&code_challenge=" + "" + "&state=" + VHeader.SubDomain;
                self.zaloHref = ko.observable(hreftemp);
            }
        });
    };
    self.zaloConnected = ko.observable(false);
    self.zaloAOInfo = ko.observable({
        avartar: "",
        cover: "",
        description: "",
        is_verified: false,
        name: "",
        oa_id: 0
    });
    self.GetStatusZaloConnection = function () {
        if (self.zaloConnected() == false) {
            ajaxHelper("/api/DanhMuc/HT_API/GetZaloConnectStatus?subdomain=" + VHeader.SubDomain).done(function (data) {
                if (data.res === true) {
                    self.zaloConnected(true);
                    self.zaloAOInfo(data.dataSoure);
                    console.log(self.zaloAOInfo().name);
                }
                else {
                    self.zaloConnected(false);
                    self.getZaloConnectHref();
                }
            });
        }
    }

    self.OpenZaloConnect = function () {
        var width, height, url;
        height = 600;
        width = 600;
        url = self.zaloHref();
        var leftPosition, topPosition;
        //Allow for borders.
        leftPosition = (window.screen.width / 2) - ((width / 2) + 10);
        //Allow for title and status bars.
        topPosition = (window.screen.height / 2) - ((height / 2) + 50);
        var popup = window.open(url, "Open24 connect with Zalo",
            "status=no,height=" + height + ",width=" + width + ",resizable=yes,left="
            + leftPosition + ",top=" + topPosition + ",screenX=" + leftPosition + ",screenY="
            + topPosition + ",toolbar=no,menubar=no,scrollbars=no,location=no,directories=no");
        window.open(self.zaloHref(), 'Open24 connect with Zalo', 'width=600,height=600');

        var popupTick = setInterval(function () {
            if (popup.closed) {
                clearInterval(popupTick);
                console.log('window closed!');
            }
        }, 500);

        return false;
    }
    //End ZaloApp
    //Template Đặt lịch
    self.GetTemplateDatLich = function () {
        $.ajax({
            url: "/api/ht_api/GetTemplate?s=" + "0973474985",
            type: 'GET',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (data) {
                //let head = document.getElementById("Iframe").contentWindow.document.head;
                //var s = document.createElement("script");
                //s.type = "text/javascript";
                //s.src = "https://temp.open24.vn/contents/lib/vue.min.js";
                //head.append(s);
                //head.append('<script type="text/javascript" src="https://temp.open24.vn/contents/lib/bootstrap-5.1.3-dist/js/bootstrap.js"></script>');
                //head.append('<link rel="stylesheet" href="https://temp.open24.vn/contents/lib/bootstrap-5.1.3-dist/css/bootstrap.css">');
                //head.append('<script type="text/javascript" src="https://temp.open24.vn/contents/lib/moment.js"></script>');
                //head.append('<link rel="stylesheet" href="https://temp.open24.vn/contents/css/template1.css">');
                let iframe = $("#Iframe").contents().find('body');
                iframe.html(data.dataSoure.html);
                /*console.log(encodeURI(data.dataSoure.html));*/
                //let opiframe = document.createElement('iframe');

                //opiframe.src = 'data:text/html;charset=utf-8,' + encodeURI(data.dataSoure.html);
                ///*opiframe.contentWindow.document = data.dataSoure.html;*/
                //document.getElementById("opiframe").appendChild(opiframe);
                console.log(vTemplate);
            },
        })
    }

    self.GetTemplateDatLich();
    //End Template Đặt lịch
};

var FileModel = function (filef, srcf) {
    var self = this;
    this.file = filef;
    this.URLAnh = srcf;
};
ko.applyBindings(new ViewModel(), document.getElementById('divPage'));
var modelMaChungTu = new Model_SetupMaChungTu()
ko.applyBindings(modelMaChungTu, document.getElementById('div2'));

var arrIDHang = [];
function getIDHH(obj) {
    var thisID = $(obj).attr('id');
    if ($(obj).is(':checked')) {
        if (thisID !== undefined && !(jQuery.inArray(thisID, arrIDHang) > -1)) {
            arrIDHang.push(thisID);
        }
    }
    else {
        //remove item in arrID
        $.map(arrIDHang, function (item, i) {
            if (item === thisID) {
                arrIDHang.splice(i, 1);
            }
        });
    }
}
function SetCheckAllHH(obj) {
    var isChecked = $(obj).is(":checked");
    $('.checkall_DonVi input[type=checkbox]').each(function () {
        $(this).prop('checked', isChecked);
    })
    if (isChecked) {
        $('.checkall_DonVi input[type=checkbox]').each(function () {
            var thisID = $(this).attr('id');
            if (thisID !== undefined && !(jQuery.inArray(thisID, arrIDHang) > -1)) {
                arrIDHang.push(thisID);
            }
        })
    }
    else {
        $('.checkall_DonVi input[type=checkbox]').each(function () {
            var thisID = $(this).attr('id');
            if (thisID !== undefined && (jQuery.inArray(thisID, arrIDHang) > -1)) {
                $.map(arrIDHang, function (item, i) {
                    if (item === thisID) {
                        arrIDHang.splice(i, 1);
                    }
                })
            }
        })
    }
}