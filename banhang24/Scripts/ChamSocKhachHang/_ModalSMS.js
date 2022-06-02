var PartialView_SMS = function () {
    var self = this;
    var ThietLapAPI = '/api/DanhMuc/ThietLapApi/';
    var DMDoiTuongUri = '/api/DanhMuc/DM_DoiTuongAPI/';
    var _idDonVi = $('#hd_IDdDonVi').val();
    var _userID = $('.idnguoidung').text();
    self.BrandNames = ko.observableArray();
    self.GiaTienMotTrangTin = ko.observableArray();
    self.IDBrandNameChoose = ko.observable();
    self.SoDuTaiKhoan = ko.observableArray();
    self.SMSMaus = ko.observableArray();
    self.AllSMSMaus = ko.observableArray();
    self.Popup_LichHenChosed = ko.observableArray([]);
    self.LoaiCongViecs = ko.observableArray();
    self.NhanViens = ko.observableArray();
    self.LichHen_byDate = ko.observableArray();
    self.LoaiTinNhanGui = ko.observableArray();
    self.SaveSuscess = ko.observable(0);
    self.LoaiTins = ko.observableArray([
        { ID: 1, TenLoai: 'Giao dịch' },
        { ID: 2, TenLoai: 'Sinh nhật' },
        { ID: 3, TenLoai: 'Tin thường' },
        { ID: 4, TenLoai: 'Lịch hẹn' },
    ]);

    function SearchLichHen_Popup() {
        var arrLoaiCV = self.LoaiCongViecs().map(function (i, e) {
            return i.ID;
        });
        arrLoaiCV.push(const_GuidEmpty);
        let arrNV = self.NhanViens().map(function (i, e) {
            return i.ID;
        });
        arrNV.push(const_GuidEmpty);

        var date = $('#txtNgayDatLich').val();
        var from = moment(date, 'DD/MM/YYYY').format('YYYY-MM-DD');
        var to = moment(date, 'DD/MM/YYYY').add(1, 'days').format('YYYY-MM-DD');

        var model = {
            ID_DonVis: [iddonvi],
            IDLoaiTuVans: arrLoaiCV,
            IDNhanVienPhuTrachs: arrNV,
            TrangThaiCVs: [1],// dangxuly,
            MucDoUuTien: '%%',//all
            LoaiDoiTuong: 1,
            PhanLoai: '%%',// lichhen + cv
            FromDate: from,
            ToDate: to,
            TextSearch: '',
            CurrentPage: 0,
            PageSize: 100,
            TypeShow: 1,
            ID_KhachHang: '%%',
            IDNhomKH: '',
        }
        ajaxHelper(DMDoiTuongUri + 'SMS_LichHen?status=' + parseInt(self.Loc_TrangThaiGui()), 'POST', model).done(function (x) {
            self.Popup_LichHenChosed([]);
            if (x.res === true) {
                self.LichHen_byDate(x.data);
                console.log(2, x.data)
            }
        });
    }

    $('#txtNoiDungTin').keyup(function () {
        var chars = this.value.length,
            messages = Math.ceil(chars / 140),
            remaining = messages > 1 ? (messages - 1) * 140 + (chars % ((messages - 1) * 140) || (messages - 1) * 140) : (messages - 1) * 140 + (chars % (messages * 140) || messages * 140);
        var totalmes = messages * 140;
        if (messages !== 0) {
            $('#remaining').text(remaining + '/' + totalmes);
            $('#messages').text(' (' + messages + ' tin nhắn)');
        }
        else {
            $('#remaining').text("");
            $('#messages').text("");
        }
    });

    self.selectedMauTin = function (item) {
        if (item !== undefined) {
            if (item.ID !== undefined) {
                var data = self.SMSMaus().filter(p => p.ID === item.ID);
                $('#txtNoiDungTin').val(data[0].NoiDungTin);
                self.LoaiTinNhanGui(data[0].LoaiTin);
                var chars = data[0].NoiDungTin.length,
                    messages = Math.ceil(chars / 140),
                    remaining = messages > 1 ? (messages - 1) * 140 + (chars % ((messages - 1) * 140) || (messages - 1) * 140) : (messages - 1) * 140 + (chars % (messages * 140) || messages * 140);
                var totalmes = messages * 140;
                $('#remaining').text(remaining + '/' + totalmes);
                $('#messages').text(' (' + messages + ' tin nhắn)');
                if (item.NoiDungTin.length > 100) {
                    var tr = item.NoiDungTin.substr(0, 105);
                    var mangTr = tr.split(" ");
                    var chuoi = mangTr[0];
                    for (var j = 1; j < mangTr.length - 1; j++) {
                        chuoi = chuoi + " " + mangTr[j];
                    }
                    item.NoiDungTin = chuoi + "...";
                    $('#txtMauTinChoose').html(item.NoiDungTin);
                }
                else {
                    $('#txtMauTinChoose').html(item.NoiDungTin);
                }
                $('#ChooseMauTin li').each(function () {
                    $(this).find('.fa-check').remove();
                });
                $('#ChooseMauTin li').each(function () {
                    if ($(this).attr('id') === item.ID) {
                        $(this).find('.fa-check').remove();
                        $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                    }
                });
            }
            else {
                $('#txtMauTinChoose').html("Chọn mẫu tin");
                $('#txtNoiDungTin').val("");
                $('#remaining').text("");
                $('#messages').text("");
                $('#ChooseMauTin li').each(function () {
                    $(this).find('.fa-check').remove();
                });
            }
        }
        else {
            $('#txtMauTinChoose').html("Chọn mẫu tin");
            $('#txtNoiDungTin').val("");
            $('#remaining').text("");
            $('#messages').text("");
            $('#ChooseMauTin li').each(function () {
                $(this).find('.fa-check').remove();
            });
        }
    };

    self.selectedBrandName = function (item) {
        if (item !== undefined) {
            if (item.ID !== undefined) {
                $('#txtBrandNames').html(item.BrandName);
                self.IDBrandNameChoose(item.ID);
                ajaxHelper(ThietLapAPI + 'GetGiaTienTrenTinNhan?id_brand=' + item.ID, "GET").done(function (data) {
                    self.GiaTienMotTrangTin(data);
                })
                $('#selectedBrandName li').each(function () {
                    $(this).find('.fa-check').remove();
                });
                $('#selectedBrandName li').each(function () {
                    if ($(this).attr('id') === item.ID) {
                        $(this).find('.fa-check').remove();
                        $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                    }
                });
            }
            else {
                self.IDBrandNameChoose(undefined);
                $('#selectedBrandName li').each(function () {
                    $(this).find('.fa-check').remove();
                });
                $('#txtBrandNames').html("Chọn BrandName");
            }
        }
        else {
            self.IDBrandNameChoose(undefined);
            $('#txtBrandNames').html("Chọn BrandName");
            $('#selectedBrandName li').each(function () {
                $(this).find('.fa-check').remove();
            });
        }
    };

    self.selectedLoaiTin = function (item) {
        if (item !== undefined) {
            var today = new Date();
            $('#txtLoaiTin').html(item.TenLoai);

            var id = parseInt(item.ID);
            self.SMSMaus(self.AllSMSMaus().filter(p => p.LoaiTin === id));
            self.LoaiTinNhanGui(id);

            switch (id) {
                case 1:// giaodich
                    $('#hideDoiTuongGui').hide();
                    $('#sinhnhatkhachhang, #lichhen').hide();
                    $('#hoadongiaodich').show();
                    $('#btnGuiTinNhanGD').show();
                    $('#btnGuiTinNhan,#btnGuiTinNhanSN').hide();
                    $('#selectedHoaDon li').each(function () {
                        $(this).find('.fa-check').remove();
                    });
                    $('#txtNgaySN').val(moment(today).format('DD/MM/YYYY'));
                    break;
                case 2:// sinhnhat
                    $('#hideDoiTuongGui').hide();
                    $('#sinhnhatkhachhang').show();
                    $('#hoadongiaodich, #lichhen').hide();
                    $('#btnGuiTinNhanSN').show();
                    $('#btnGuiTinNhan,#btnGuiTinNhanGD,#btnGuiTin').hide();
                    $('#selectedDoiTuongSN li').each(function () {
                        $(this).find('.fa-check').remove();
                    });
                    $('#txtNgaySN').val(moment(today).format('DD/MM/YYYY'));
                    break;
                case 3: //tinthuong
                    $('#hideDoiTuongGui').show();
                    $('#sinhnhatkhachhang,#hoadongiaodich, #lichhen').hide();
                    $('#btnGuiTinNhan').show();
                    $('#btnGuiTinNhanSN,#btnGuiTinNhanGD,#btnGuiTin').hide();
                    break;
                case 4:// lichhen
                    $('#hideDoiTuongGui').hide();
                    $('#lichhen').show();
                    $('#sinhnhatkhachhang, #hoadongiaodich').hide();
                    $('#btnGuiTin').show();
                    $('#btnGuiTinNhan,#btnGuiTinNhanGD, #btnGuiTinNhanSN').hide();
                    break;
            }

            $('#selectedLoaiTin li').each(function () {
                $(this).find('.fa-check').remove();
            });
            $('#selectedLoaiTin li').each(function () {
                if (parseInt($(this).attr('id')) === item.ID) {
                    $(this).find('.fa-check').remove();
                    $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                }
            });
        }
    };

    $('#txtNgayDatLich').on('change', function () {
        SearchLichHen_Popup();
    });

    self.Popup_ChoseAppointment = function (item) {
        if (item.ID !== undefined) {
            var all = $.grep(self.Popup_LichHenChosed(), function (x) {
                return x.ID === 0;
            });
            if (all.length > 0) {
                ShowMessage_Danger('Bạn đã chọn gửi tin cho toàn bộ khách hàng');
                return false;
            }
            var arrDT = [];
            for (var i = 0; i < self.Popup_LichHenChosed().length; i++) {
                if ($.inArray(self.Popup_LichHenChosed()[i], arrDT) === -1) {
                    arrDT.push(self.Popup_LichHenChosed()[i].ID);
                }
            }
            if ($.inArray(item.ID, arrDT) === -1) {
                self.Popup_LichHenChosed.push(item);
            }
            $('#ddlAppointemnt li').each(function () {
                if ($(this).attr('id') === item.ID) {
                    $(this).find('.fa-check').remove();
                    $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                }
            });
        }
        else {
            self.Popup_LichHenChosed([{ ID: 0, TenDoiTuong: 'Toàn bộ khách hàng' }]);
        }
    };

    self.Popup_RemoveAppointment = function (item) {
        self.Popup_LichHenChosed.remove(item);
        $('#ddlAppointemnt li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
    };

    function Enable_BtnGuiTin(idButton) {
        document.getElementById(idButton).disabled = false;
        document.getElementById(idButton).lastChild.data = " Gửi";
    }

    self.SendSMS = function () {
        document.getElementById("btnGuiTin").disabled = true;
        document.getElementById("btnGuiTin").lastChild.data = " Đang lưu";
        var noiDungTin = $('#txtNoiDungTin').val();
        var lenghtTinNhan = noiDungTin.length;
        var soTinGui = Math.ceil(lenghtTinNhan / 140);
        var idbrand = self.IDBrandNameChoose();
        if (idbrand === undefined) {
            ShowMessage_Danger("Vui lòng chọn BrandName gửi tin");
            Enable_BtnGuiTin("btnGuiTin");
            return false;
        }
        if (self.Popup_LichHenChosed().length === 0) {
            ShowMessage_Danger("Vui lòng chọn khách hàng cần gửi tin");
            Enable_BtnGuiTin("btnGuiTin");
            return false;
        }
        if (noiDungTin === "" || noiDungTin === null || noiDungTin === undefined || noiDungTin === "undefined") {
            ShowMessage_Danger('Vui lòng nhập nội dung tin nhắn');
            Enable_BtnGuiTin("btnGuiTin");
            return false;
        }
        if (self.GiaTienMotTrangTin() * soTinGui * self.Popup_LichHenChosed().length > self.SoDuTaiKhoan()) {
            ShowMessage_Danger("Số dư không đủ để gửi tin. Vui lòng nạp thêm tiền");
            Enable_BtnGuiTin("btnGuiTin");
            return false;
        }
        // get customer has phone
        var cusHasPhone = $.grep(self.Popup_LichHenChosed(), function (x) {
            return x.DienThoai !== '';
        });
        var j = 0;
        function guitin() {
            if (j < cusHasPhone.length) {
                let itFor = cusHasPhone[j];
                let myData = {};
                let objTin = {
                    SoDienThoai: itFor.DienThoai,
                    NoiDung: noiDungTin,
                    SoTinGui: soTinGui,
                    LoaiTinNhan: self.LoaiTinNhanGui(),
                    ID_NguoiGui: _userID,
                    ID_DonVi: _idDonVi,
                    ID_KhachHang: itFor.ID
                };
                myData.objTinNhan = objTin;
                myData.ID_BrandName = idbrand;
                console.log(22, myData)
                ajaxHelper(ThietLapAPI + 'PostTinNhan', 'POST', myData).done(function () {
                    j++;
                    guitin();
                }).fail(function (err) {
                    console.log(err)
                });
            }
            else {
                ShowMessage_Success('Gửi tin nhắn thành công');
                Enable_BtnGuiTin("btnGuiTin");
                $('#modalSMS').modal('hide');
                self.SaveSuscess(2);
            }
        }
        guitin();
    };
};