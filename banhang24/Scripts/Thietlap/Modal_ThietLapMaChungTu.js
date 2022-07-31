var Model_SetupMaChungTu = function () {
    var self = this;
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
    var elmCheck = '<i class="fa fa-check check-after-li" style="display:block"></i>';

    self.isUpdate = ko.observable(false);
    self.KiTuPhanCach1 = ko.observableArray();
    self.KiTuPhanCach2 = ko.observableArray();
    self.KiTuPhanCach3 = ko.observableArray();
    self.ItemNgayThangNam = ko.observableArray();
    self.ItemDoDaiSTT = ko.observableArray();
    self.IsGara = ko.observable(VHeader.IdNganhNgheKinhDoanh === 'C16EDDA0-F6D0-43E1-A469-844FAB143014');
    self.KiTuNganCachs = ko.observableArray([
        { ID: 1, Text: '_' },
        { ID: 2, Text: '-' },
        { ID: 3, Text: '.' },
    ])
    self.NgayThangNams = ko.observableArray([
        { ID: 0, Text: '(none)' },
        { ID: 1, Text: 'ddMMyyyy' },
        { ID: 2, Text: 'ddMMyy' },
        { ID: 3, Text: 'MMyy' },
        { ID: 4, Text: 'MMyyyy' },
        { ID: 5, Text: 'yyyyMMdd' },
        { ID: 6, Text: 'yyMMdd' },
        { ID: 7, Text: 'yyMM' },
        { ID: 8, Text: 'yyyyMM' },
        { ID: 9, Text: 'yyyy' },
    ])
    self.TblSetup = ko.observableArray([
        { ID_LoaiChungTu: 33, ApDung: false, SuDungMaDonVi: false, KiTuNganCach1: '', KiTuNganCach2: '', KiTuNganCach3: '', NgayThangNam: '', GiaTriNgay: '', DoDaiSTT: 0, LoaiChungTu: 'Khách hàng', MaChiNhanh: '', MaLoaiChungTu: '', KiTuNganCachs: self.KiTuNganCachs(), NgayThangNams: self.NgayThangNams(), Mau: '' },
        { ID_LoaiChungTu: 34, ApDung: false, SuDungMaDonVi: false, KiTuNganCach1: '', KiTuNganCach2: '', KiTuNganCach3: '', NgayThangNam: '', GiaTriNgay: '', DoDaiSTT: 0, LoaiChungTu: 'Nhà cung cấp', MaChiNhanh: '', MaLoaiChungTu: '', KiTuNganCachs: self.KiTuNganCachs(), NgayThangNams: self.NgayThangNams(), Mau: '' },
        { ID_LoaiChungTu: 1, ApDung: false, SuDungMaDonVi: false, KiTuNganCach1: '', KiTuNganCach2: '', KiTuNganCach3: '', NgayThangNam: '', GiaTriNgay: '', DoDaiSTT: 0, LoaiChungTu: 'Hóa đơn bán lẻ', MaChiNhanh: '', MaLoaiChungTu: 'HDBL', KiTuNganCachs: self.KiTuNganCachs(), NgayThangNams: self.NgayThangNams(), Mau: '' },
        { ID_LoaiChungTu: 3, ApDung: false, SuDungMaDonVi: false, KiTuNganCach1: '', KiTuNganCach2: '', KiTuNganCach3: '', NgayThangNam: '', GiaTriNgay: '', DoDaiSTT: 0, LoaiChungTu: 'Báo giá', MaChiNhanh: '', MaLoaiChungTu: 'BG', KiTuNganCachs: self.KiTuNganCachs(), NgayThangNams: self.NgayThangNams(), Mau: '' },
        { ID_LoaiChungTu: 31, ApDung: false, SuDungMaDonVi: false, KiTuNganCach1: '', KiTuNganCach2: '', KiTuNganCach3: '', NgayThangNam: '', GiaTriNgay: '', DoDaiSTT: 0, LoaiChungTu: 'Đặt hàng nhà cung cấp', MaChiNhanh: '', MaLoaiChungTu: 'PO', KiTuNganCachs: self.KiTuNganCachs(), NgayThangNams: self.NgayThangNams(), Mau: '' },
        { ID_LoaiChungTu: 4, ApDung: false, SuDungMaDonVi: false, KiTuNganCach1: '', KiTuNganCach2: '', KiTuNganCach3: '', NgayThangNam: '', GiaTriNgay: '', DoDaiSTT: 0, LoaiChungTu: 'Phiếu nhập kho', MaChiNhanh: '', MaLoaiChungTu: 'PNK', KiTuNganCachs: self.KiTuNganCachs(), NgayThangNams: self.NgayThangNams(), Mau: '' },
        { ID_LoaiChungTu: 6, ApDung: false, SuDungMaDonVi: false, KiTuNganCach1: '', KiTuNganCach2: '', KiTuNganCach3: '', NgayThangNam: '', GiaTriNgay: '', DoDaiSTT: 0, LoaiChungTu: 'Trả hàng', MaChiNhanh: '', MaLoaiChungTu: 'TH', KiTuNganCachs: self.KiTuNganCachs(), NgayThangNams: self.NgayThangNams(), Mau: '' },
        { ID_LoaiChungTu: 7, ApDung: false, SuDungMaDonVi: false, KiTuNganCach1: '', KiTuNganCach2: '', KiTuNganCach3: '', NgayThangNam: '', GiaTriNgay: '', DoDaiSTT: 0, LoaiChungTu: 'Trả hàng nhà cung cấp', MaChiNhanh: '', MaLoaiChungTu: 'THN', KiTuNganCachs: self.KiTuNganCachs(), NgayThangNams: self.NgayThangNams(), Mau: '' },
        { ID_LoaiChungTu: 8, ApDung: false, SuDungMaDonVi: false, KiTuNganCach1: '', KiTuNganCach2: '', KiTuNganCach3: '', NgayThangNam: '', GiaTriNgay: '', DoDaiSTT: 0, LoaiChungTu: 'Xuất kho', MaChiNhanh: '', MaLoaiChungTu: 'PXK', KiTuNganCachs: self.KiTuNganCachs(), NgayThangNams: self.NgayThangNams(), Mau: '' },
        { ID_LoaiChungTu: 9, ApDung: false, SuDungMaDonVi: false, KiTuNganCach1: '', KiTuNganCach2: '', KiTuNganCach3: '', NgayThangNam: '', GiaTriNgay: '', DoDaiSTT: 0, LoaiChungTu: 'Phiếu kiểm kê', MaChiNhanh: '', MaLoaiChungTu: 'PKK', KiTuNganCachs: self.KiTuNganCachs(), NgayThangNams: self.NgayThangNams(), Mau: '' },
        { ID_LoaiChungTu: 10, ApDung: false, SuDungMaDonVi: false, KiTuNganCach1: '', KiTuNganCach2: '', KiTuNganCach3: '', NgayThangNam: '', GiaTriNgay: '', DoDaiSTT: 0, LoaiChungTu: 'Chuyển hàng', MaChiNhanh: '', MaLoaiChungTu: 'CH', KiTuNganCachs: self.KiTuNganCachs(), NgayThangNams: self.NgayThangNams(), Mau: '' },
        { ID_LoaiChungTu: 11, ApDung: false, SuDungMaDonVi: false, KiTuNganCach1: '', KiTuNganCach2: '', KiTuNganCach3: '', NgayThangNam: '', GiaTriNgay: '', DoDaiSTT: 0, LoaiChungTu: 'Phiếu thu', MaChiNhanh: '', MaLoaiChungTu: 'SQPT', KiTuNganCachs: self.KiTuNganCachs(), NgayThangNams: self.NgayThangNams(), Mau: '' },
        { ID_LoaiChungTu: 12, ApDung: false, SuDungMaDonVi: false, KiTuNganCach1: '', KiTuNganCach2: '', KiTuNganCach3: '', NgayThangNam: '', GiaTriNgay: '', DoDaiSTT: 0, LoaiChungTu: 'Phiếu chi', MaChiNhanh: '', MaLoaiChungTu: 'SQPC', KiTuNganCachs: self.KiTuNganCachs(), NgayThangNams: self.NgayThangNams(), Mau: '' },
        { ID_LoaiChungTu: 19, ApDung: false, SuDungMaDonVi: false, KiTuNganCach1: '', KiTuNganCach2: '', KiTuNganCach3: '', NgayThangNam: '', GiaTriNgay: '', DoDaiSTT: 0, LoaiChungTu: 'Gói dịch vụ', MaChiNhanh: '', MaLoaiChungTu: 'GDV', KiTuNganCachs: self.KiTuNganCachs(), NgayThangNams: self.NgayThangNams(), Mau: '' },
        { ID_LoaiChungTu: 22, ApDung: false, SuDungMaDonVi: false, KiTuNganCach1: '', KiTuNganCach2: '', KiTuNganCach3: '', NgayThangNam: '', GiaTriNgay: '', DoDaiSTT: 0, LoaiChungTu: 'Thẻ giá trị', MaChiNhanh: '', MaLoaiChungTu: 'TGT', KiTuNganCachs: self.KiTuNganCachs(), NgayThangNams: self.NgayThangNams(), Mau: '' },
        { ID_LoaiChungTu: 25, ApDung: false, SuDungMaDonVi: false, KiTuNganCach1: '', KiTuNganCach2: '', KiTuNganCach3: '', NgayThangNam: '', GiaTriNgay: '', DoDaiSTT: 0, LoaiChungTu: 'Hóa đơn sữa chữa', MaChiNhanh: '', MaLoaiChungTu: 'HDSC', KiTuNganCachs: self.KiTuNganCachs(), NgayThangNams: self.NgayThangNams(), Mau: '' },
        { ID_LoaiChungTu: 26, ApDung: false, SuDungMaDonVi: false, KiTuNganCach1: '', KiTuNganCach2: '', KiTuNganCach3: '', NgayThangNam: '', GiaTriNgay: '', DoDaiSTT: 0, LoaiChungTu: 'Phiếu tiếp nhận', MaChiNhanh: '', MaLoaiChungTu: 'PTN', KiTuNganCachs: self.KiTuNganCachs(), NgayThangNams: self.NgayThangNams(), Mau: '' },
    ]);

    if (!self.IsGara()) {
        // cai nay chi apdung neu hethong chua cai dat lan nao
        let arr = $.grep(self.TblSetup(), function (x) {
            return $.inArray(x.ID_LoaiChungTu,[25,26]) === -1;
        })
        self.TblSetup(arr);
    }

    self.Chose_KiTuPhanCach = function (item, parent, ngancach) {
        var waring = '';
        var itemDoing = $.grep(self.TblSetup(), function (x) {
            return x.ID_LoaiChungTu === parent.ID_LoaiChungTu
        });
        if (itemDoing.length > 0) {
            switch (ngancach) {
                case 1:
                    if (!itemDoing[0].SuDungMaDonVi) {
                        waring = 'Vui lòng chọn tích chi nhánh nếu muốn thiết lập ký tự ngăn cách ' + ngancach;
                    }
                    break;
                case 2:
                    break;
                case 3:
                    if (commonStatisJs.CheckNull(itemDoing[0].GiaTriNgay)) {
                        waring = 'Vui lòng chọn định dạng ngày/tháng/năm nếu muốn lập ký tự ngăn cách ' + ngancach;
                    }
                    break;
            }
        }

        if (waring !== '') {
            ShowMessage_Danger(waring);
            return;
        }

        let text = item.Text;
        var $this = event.currentTarget;
        var thisInput = $($this).closest('.ddl-kituphancach').find('input');
        if (ngancach === 4) {
            if (item.ID === 0) {// chose (none)
                text = '';
            }
        }
        thisInput.val(text);

        var dodai = parent.DoDaiSTT;
        let stt = GetSTT(dodai);

        // update & bind again table
        var mau = '';
        for (var i = 0; i < self.TblSetup().length; i++) {
            let itemFor = self.TblSetup()[i];
            if (itemFor.ID_LoaiChungTu === parent.ID_LoaiChungTu) {
                let sDate = '';
                let machinhanh = '';
                let kitu1 = '';
                if (itemFor.SuDungMaDonVi) {
                    machinhanh = itemFor.MaChiNhanh;
                    kitu1 = text;
                }
                switch (ngancach) {
                    case 1:
                        self.TblSetup()[i].KiTuNganCach1 = kitu1;
                        break;
                    case 2:
                        self.TblSetup()[i].KiTuNganCach2 = text;
                        break;
                    case 3:
                        self.TblSetup()[i].KiTuNganCach3 = text;
                        break;
                    case 4:
                        switch (item.ID) {
                            case 1: //ddMMyyyy
                                sDate = sDate.concat(date, month, year);
                                break;
                            case 2: //ddMMyy
                                sDate = sDate.concat(date, month, year.substr(2, 2));
                                break;
                            case 3: //MMyy
                                sDate = sDate.concat(month, year.substr(2, 2));
                                break;
                            case 4: //MMyyyy
                                sDate = sDate.concat(month, year);
                                break;
                            case 5: //yyyyMMdd
                                sDate = sDate.concat(year, month, date);
                                break;
                            case 6: //yyMMdd
                                sDate = sDate.concat(year.substr(2, 2), month, date);
                                break;
                            case 7: //yyMM
                                sDate = sDate.concat(year.substr(2, 2), month);
                                break;
                            case 8: //yyyyMM
                                sDate = sDate.concat(year, month);
                                break;
                            case 9: //yyyy
                                sDate = year;
                                break;
                        }
                        self.TblSetup()[i].GiaTriNgay = sDate;
                        self.TblSetup()[i].NgayThangNam = text;
                        break;
                }
                let kitu3 = '';
                if (self.TblSetup()[i].GiaTriNgay !== '') {
                    kitu3 = self.TblSetup()[i].KiTuNganCach3;
                }
                self.TblSetup()[i].Mau = machinhanh.concat(self.TblSetup()[i].KiTuNganCach1, itemFor.MaLoaiChungTu, self.TblSetup()[i].KiTuNganCach2, self.TblSetup()[i].GiaTriNgay, kitu3, stt);
                mau = self.TblSetup()[i].Mau;
                break;
            }
        }
        // assign Mau
        $($this).closest('tr').find('td:last-child').text(mau);
        // append check after
        $($this).closest('ul').find('.group-p1 i').remove();
        $($this).children('.group-p1').append(elmCheck);
    }

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

    self.EditDoDai = function (data) {
        var $this = event.currentTarget;
        var val = $($this).val();

        let dodai = parseInt(val);
        let stt = GetSTT(dodai);
        var mau = '';
        for (var i = 0; i < self.TblSetup().length; i++) {
            let itemFor = self.TblSetup()[i];
            if (itemFor.ID_LoaiChungTu === data.ID_LoaiChungTu) {
                let machinhanh = '';
                let kitu1 = '';
                if (itemFor.SuDungMaDonVi) {
                    machinhanh = itemFor.MaChiNhanh;
                    kitu1 = itemFor.KiTuNganCach1;
                }
                let kitu3 = '';
                if (self.TblSetup()[i].GiaTriNgay !== '') {
                    kitu3 = self.TblSetup()[i].KiTuNganCach3;
                }
                self.TblSetup()[i].DoDaiSTT = dodai;
                self.TblSetup()[i].Mau = machinhanh.concat(kitu1, itemFor.MaLoaiChungTu, self.TblSetup()[i].KiTuNganCach2, self.TblSetup()[i].GiaTriNgay, kitu3, stt);
                mau = self.TblSetup()[i].Mau;
            }
        }
        $($this).closest('tr').find('td:last-child').text(mau);
    }

    self.ChoseChiNhanh = function (data) {
        // check if MaChiNhanh = ''
        if (self.TblSetup()[0].MaChiNhanh === '') {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + 'Vui lòng cập nhật lại Mã chi nhánh trước khi thực hiện chức năng này', 'danger');
        }
        var $this = event.currentTarget;
        var isCheck = $($this).is(':checked');
        var maChiNhanh = '';
        var mau = '';
        var kitu1 = '';
        if (isCheck) {
            maChiNhanh = self.TblSetup()[0].MaChiNhanh;
            kitu1 = $($this).closest('td').next().find('input').val();
        }
        for (var i = 0; i < self.TblSetup().length; i++) {
            let itemFor = self.TblSetup()[i];
            if (itemFor.ID_LoaiChungTu === data.ID_LoaiChungTu) {
                self.TblSetup()[i].KiTuNganCach1 = kitu1;
                self.TblSetup()[i].SuDungMaDonVi = isCheck;
                let kitu3 = '';
                if (itemFor.GiaTriNgay !== '') {
                    kitu3 = itemFor.KiTuNganCach3;
                }
                let stt = GetSTT(itemFor.DoDaiSTT);
                self.TblSetup()[i].Mau = maChiNhanh.concat(kitu1, itemFor.MaLoaiChungTu, itemFor.KiTuNganCach2, itemFor.GiaTriNgay, kitu3, stt);
                mau = self.TblSetup()[i].Mau;
                break;
            }
        }
        $($this).closest('tr').find('td:last-child').text(mau);
    }

    self.ChoseApDung = function (data) {
        var $this = event.currentTarget;
        var isCheck = $($this).is(':checked');
        for (var i = 0; i < self.TblSetup().length; i++) {
            let itemFor = self.TblSetup()[i];
            if (itemFor.ID_LoaiChungTu === data.ID_LoaiChungTu) {
                self.TblSetup()[i].ApDung = isCheck;
                break;
            }
        }
    }

    self.EditKiTuNganCach = function (data, ngancach) {
        var $this = event.currentTarget;
        var text = $($this).val();

        var dodai = data.DoDaiSTT;
        let stt = GetSTT(dodai);

        // update table
        var mau = '';
        for (var i = 0; i < self.TblSetup().length; i++) {
            let itemFor = self.TblSetup()[i];
            if (itemFor.ID_LoaiChungTu === data.ID_LoaiChungTu) {
                let machinhanh = '';
                let kitu1 = '';
                if (itemFor.SuDungMaDonVi) {
                    machinhanh = itemFor.MaChiNhanh;
                    kitu1 = text;
                }
                switch (ngancach) {
                    case 1:
                        self.TblSetup()[i].KiTuNganCach1 = kitu1;
                        break;
                    case 2:
                        self.TblSetup()[i].KiTuNganCach2 = text;
                        break;
                    case 3:
                        self.TblSetup()[i].KiTuNganCach3 = text;
                        break;
                }
                let kitu3 = '';
                if (self.TblSetup()[i].GiaTriNgay !== '') {
                    kitu3 = self.TblSetup()[i].KiTuNganCach3;
                }
                self.TblSetup()[i].Mau = machinhanh.concat(kitu1, itemFor.MaLoaiChungTu, self.TblSetup()[i].KiTuNganCach2, self.TblSetup()[i].GiaTriNgay, kitu3, stt);
                mau = self.TblSetup()[i].Mau;
                break;
            }
        }
        // assign Mau
        $($this).closest('tr').find('td:last-child').text(mau);
        // append check after (ok)
        $($this).closest('.ddl-kituphancach').find('.group-p1 i').remove();
        $($this).closest('.ddl-kituphancach').find('.group-p1').each(function () {
            if ($(this).children('a').text() === text) {
                $(this).append(elmCheck);
                return false;
            }
        })
    }

    self.Save = function () {
        var arrApply = $.grep(self.TblSetup(), function (x) {
            return x.ApDung === true;
        });

        var err = '';
        for (var i = 0; i < arrApply.length; i++) {
            if (arrApply[i].SuDungMaDonVi === false) {
                arrApply[i].KiTuNganCach1 = '';
            }
            if (arrApply[i].DoDaiSTT === 0) {
                err += arrApply[i].LoaiChungTu;
            }
            if (commonStatisJs.CheckNull(arrApply[i].GiaTriNgay)) {
                arrApply[i].KiTuNganCach3 = '';
            }
        }
        if (err !== '') {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + 'Vui lòng nhập độ dài cho ' + err, 'danger');
            return false;
        }
        // delete, after add again
        var obj = { lstThietLap: arrApply };
        $.ajax({
            data: obj,
            url: '/api/DanhMuc/HT_ThietLapAPI/' + "Post_HTMaChungTu",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i> ' + 'Thiết lập mã chứng từ thành công', 'success');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + 'Thiết lập mã chứng từ thất bại', 'danger');
            },
            complete: function () {
                $("#mdThietLapMaChungTu").modal("hide");
            }
        });
    }
}

function keypressNumber(e) {
    var keyCode = window.event.keyCode || e.which;
    if (keyCode < 48 || keyCode > 57) {
        // 8: tab; 127: delete
        if (keyCode === 8 || keyCode === 127) {
            return;
        }
        return false;
    }
}

function CheckKiTuNganCach(e) {
    var keyCode = window.event.keyCode || e.which;
    // _95, -45, .46, ''32
    if (keyCode === 95 || keyCode === 45 || keyCode === 46 || keyCode === 32) {
        return true;
    }
    return false;
}
