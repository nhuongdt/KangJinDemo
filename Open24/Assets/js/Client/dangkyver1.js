

var cuahangdangky = function () {
    var self = this;
    self.HoTen = ko.observable();
    self.TenCuaHangKinhDoanh = ko.observable();
    self.SoDienThoai = ko.observable();
    self.Email = ko.observable();
    self.TenGianHang = ko.observable();
    self.TenDangNhap = ko.observable();
    self.MatKhauopen = ko.observable();
    self.linkdangnhap = ko.computed(function () {
        return "https://" + self.TenGianHang() + ".open24.vn";
    }, this);
    self.ChangeCuaHang = function () {
        var tengianhan = change_alias(localValidate.convertStrFormC(self.TenCuaHangKinhDoanh()));
     
        self.TenGianHang(tengianhan);
    }
    self.ChangeTenDangNhap = function () {
        var tendangnhap = change_alias(localValidate.convertStrFormC(self.TenDangNhap()));
        self.TenDangNhap(tendangnhap);
    }
    self.ChangeTenGianHang = function () {
        var TenGianHang = change_alias(localValidate.convertStrFormC(self.TenGianHang()));
        self.TenGianHang(TenGianHang);
    }

    function alertMessageError(mess) {
        $('.dk_Notification').text(mess);
    }

    self.ChangeSoDienThoai = function () {
        var sodienthoai = change_alias(localValidate.convertStrFormC(self.SoDienThoai()));
        console.log(sodienthoai)
        self.TenDangNhap(sodienthoai);
    }
    self.save = function () {

        if (self.TenCuaHangKinhDoanh() === null
            || self.TenCuaHangKinhDoanh() === undefined
            || self.TenCuaHangKinhDoanh().replace(/\s+/g, '') === "") {
            alertMessageError("Vui lòng nhập tên cửa hàng kinh doanh.");
        }
       
        else if (self.TenGianHang() === null
            || self.TenGianHang() === undefined
            || self.TenGianHang() === "") {
            alertMessageError("Vui lòng nhập địa chỉ Open24.");
        }
        else if (self.TenDangNhap() === null
            || self.TenDangNhap() === undefined
            || self.TenDangNhap().replace(/\s+/g, '') === "") {
            alertMessageError("Vui lòng nhập tên đăng nhập.");
        }
        else if (self.MatKhauopen() === null
            || self.MatKhauopen() === undefined
            || self.MatKhauopen().replace(/\s+/g, '') === "") {
            alertMessageError("Vui lòng nhập mật khẩu.");
        }
        else if ($("#cbdongy").prop("checked") !== true) {
            alertMessageError("Vui lòng tích chọn đồng ý điều khoản open24.");
        }
        else if (!ValiDatePassword(self.MatKhauopen())) {
            alertMessageError("Mật khẩu chứa tiếng việt, vui lòng nhập lại.");
        }
        else {
            $('.fuzzy').show();
            $('.wait-res').show();
            $('.dk_Notification').text('');
            var model = {
                HoTen: localValidate.convertStrFormC(self.HoTen()),
                SoDienThoai: localValidate.convertStrFormC(self.SoDienThoai()),
                SubDomain: localValidate.convertStrFormC(self.TenGianHang()),
                TenCuaHang: localValidate.convertStrFormC(self.TenCuaHangKinhDoanh()),
                Email: localValidate.convertStrFormC(self.Email()),
                MaNganhKinhDoanh: $('#linhvuckd').val(),
                MatKhauKT: localValidate.convertStrFormC(self.MatKhauopen()),
                UserKT: localValidate.convertStrFormC(self.TenDangNhap()),
                DienThoaiNhanVien: SoDienThoaiNhanVien,
                TenNhanVien: TenNhanVienDangKy
            };
            $.ajax({
                data: model,
                url: '/Open24Api/PostAPI/' + "DangKySuDungv2",
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (result) {
                    $('.fuzzy').hide();
                    $('.wait-res').hide();
                    if (result.res) {
                        localStorage.setItem('reg_code', self.TenGianHang());
                        localStorage.setItem('reg_username', self.TenDangNhap());
                        localStorage.setItem('reg_companyname', self.TenCuaHangKinhDoanh());
                        window.location.href = "/dang-ky-thanh-cong";
                    }
                    else {
                        alertMessageError(result.mess);
                    }
                },
                error: function (xhr, status, error) {
                    $('.fuzzy').hide();
                    $('.wait-res').hide();
                    console.log(status);
                    console.log(xhr);
                }
            });
        }
    }
    function DangKyCuHang(diachi, ip4) {
        $('.fuzzy').show();
        $('.wait-res').show();
        $('.dk_Notification').text('');
        var model = {
            HoTen: localValidate.convertStrFormC(self.HoTen()),
            SoDienThoai: localValidate.convertStrFormC(self.SoDienThoai()),
            SubDomain: localValidate.convertStrFormC(self.TenGianHang()),
            TenCuaHang: localValidate.convertStrFormC(self.TenCuaHangKinhDoanh()),
            Email: localValidate.convertStrFormC(self.Email()),
            MaNganhKinhDoanh: $('#linhvuckd').val(),
            MatKhauKT: localValidate.convertStrFormC(self.MatKhauopen()),
            KhuVuc_DK: diachi,
            DiaChiIP_DK: ip4,
            UserKT: localValidate.convertStrFormC(self.TenDangNhap())
        };
        $.ajax({
            data: model,
            url: '/Open24Api/PostAPI/' + "DangKySuDungv1",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                $('.fuzzy').hide();
                $('.wait-res').hide();
                if (result.res) {
                    alertMessageError('');
                    $('.dk-op-b1').toggle();
                    $('.dk-op-b2').toggle();
                    $('tencuahangdangky').focus();
                    if (!$('.progress-bar').hasClass('ac-dk-succes'))
                        $('.progress-bar').addClass('ac-dk-succes');
                }
                else {
                    alertMessageError(result.mess);
                }
            },
            error: function (xhr, status, error) {
                $('.fuzzy').hide();
                $('.wait-res').hide();
                console.log(status);
                console.log(xhr);
            }
        });
    }

    $('.btn-dk-next').on('click', function () {
        //self.SoDienThoai().replace(/\D/g, '')
        //console.log('|' + self.SoDienThoai().replace(/\D/g, '')+'|')
        if (!localValidate.CheckPhoneNumber(self.SoDienThoai())) {
            alertMessageError("Số điên thoại bạn nhập chưa đúng");
            $('.sodienthoai').focus();
        } else {
            if (self.SoDienThoai() === null
                || self.SoDienThoai() === undefined
                || self.SoDienThoai() === "") {
                alertMessageError("Vui lòng nhập số điện thoại.");
                $('.sodienthoai').focus();
            }
            else if (self.SoDienThoai().length < 10) {
                alertMessageError("Số điện thoại không hợp lệ.");
                $('.sodienthoai').focus();
            }
            else if (CheckEmail(self.Email()) === false) {
                alertMessageError("Địa chỉ email không hợp lệ.");
                $('.email').focus();
            }
            else if ($('#linhvuckd').val() === null || $('#linhvuckd').val() === undefined || $('#linhvuckd').val() === '-1') {
                alertMessageError("Vui lòng chọn ngành nghề kinh doanh.");
            }
            else {
                $.ajax({
                    type: 'GET',
                    url: "https://geoip-db.com/json/",
                    success: function (result) {
                        var data = JSON.parse(result);
                        var diachi = (data.city !== '' && data.city !== null) ? data.city + " - " + data.country_name : data.country_name;
                        DangKyCuHang(diachi, data.IPv4);
                    },
                    timeout: 5000,      // 3 seconds
                    error: function (qXHR, textStatus, errorThrown) {
                        DangKyCuHang("", "");
                        if (textStatus === "timeout") {
                            console.log(qXHR);
                        }
                    }
                });

            }
    }
    })

    $('.btn-dk-back').on('click', function () {
        alertMessageError('');
        $('.dk-op-b1').toggle();
        $('.dk-op-b2').toggle();
        $('.progress-bar').removeClass('ac-dk-succes');
    })

    $('.hoten').focus();
}
ko.applyBindings(new cuahangdangky());
function CheckEmail(value) {
    if (value !== null && value !== undefined && value.replace(/\s+/g, '') !== "") {
        var email = value.trim();
        var res = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;

        if (res.test(email)) {
            return true;
        }
        else {
            return false;
        }
    }
    else {
        return true;
    }
};
var Checkvietnamse = [
    "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
    "đ",
    "é", "è", "ẻ", "ẽ", "ẹ", "ê", "ế", "ề", "ể", "ễ", "ệ",
    "í", "ì", "ỉ", "ĩ", "ị",
    "ó", "ò", "ỏ", "õ", "ọ", "ô", "ố", "ồ", "ổ", "ỗ", "ộ", "ơ", "ớ", "ờ", "ở", "ỡ", "ợ",
    "ú", "ù", "ủ", "ũ", "ụ", "ư", "ứ", "ừ", "ử", "ữ", "ự",
    "ý", "ỳ", "ỷ", "ỹ", "ỵ"];
function ValiDatePassword(value) {
    var str = value;
    if (str === null || str === undefined || str.replace(/\s+/g, '') === "") {
        return false;
    }
    for (var i = 0; i < Checkvietnamse.length; i++) {
        if (str.indexOf(Checkvietnamse[i]) >= 0)
        { return false; }
    }
    return true;
}

//===============================
// Hiện thị Datetime
//===============================
function ConvertDate(config) {
    if (config === undefined
        || config === null
        || config.replace(/\s+/g, '') === "") {
        return "";
    }
    else {
        var a = moment(config).format('DD/MM/YYYY');
        return a;
    }
}
$(".dk-tt-left .form-focus .form-group input").on('keydown blur', function (e) {
    if (e.keyCode === 9 || e.keyCode === 13) {
        $(this).closest('.form-group').closest('.form-focus').next('.form-focus').find('.form-group').find('input').focus()
    }

});