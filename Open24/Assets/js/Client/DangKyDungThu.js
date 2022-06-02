

var cuahangdangky = function () {
    var self = this;
    self.TenCuaHangKinhDoanh = ko.observable();
    self.SoDienThoai = ko.observable();
    self.Email = ko.observable();
    self.TenGianHang = ko.observable();
    self.TenDangNhap = ko.observable();
    self.MatKhau = ko.observable();
    self.NganhNgheKey = ko.observable();
    self.NghanhNgheValue = ko.observable();
    self.ChangeCuaHang = function () {
        var tengianhan = change_alias(localValidate.convertStrFormC(self.TenCuaHangKinhDoanh()));
        if (tengianhan === '' || tengianhan === null || tengianhan === undefined) {
            if (!$('#tengianhang').find('.p-dk').hasClass('dk-title-input')) {
                $('#tengianhang').find('.p-dk').addClass('dk-title-input');
                $('#tengianhang').find('input').hide();
                $('#tengianhang').find('.poss-op').hide();
            }
        }
        else {
            $('#tengianhang').find('.p-dk').removeClass('dk-title-input');
            $('#tengianhang').find('input').show();
            $('#tengianhang').find('.poss-op').show();
            
        }
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
    self.save = function () {

        if (self.TenCuaHangKinhDoanh() === null
            || self.TenCuaHangKinhDoanh() === undefined
            || self.TenCuaHangKinhDoanh().replace(/\s+/g, '') === "") {
            alertMessageError("Vui lòng nhập tên cửa hàng kinh doanh.");
        }
        else if (self.SoDienThoai() === null
            || self.SoDienThoai() === undefined
            || self.SoDienThoai() === "") {
            alertMessageError("Vui lòng nhập số điện thoại.");
        }
        else if (CheckEmail(self.Email()) === false) {
            alertMessageError("Địa chỉ email không hợp lệ.");
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
        else if (self.MatKhau() === null
            || self.MatKhau() === undefined
            || self.MatKhau().replace(/\s+/g, '') === "") {
            alertMessageError("Vui lòng nhập mật khẩu.");
        }
        else if (self.NganhNgheKey() === null
            || self.NganhNgheKey() === undefined
            || self.NganhNgheKey().replace(/\s+/g, '') === "") {
            NganhNgheKey("Vui lòng chọn ngành nghề kinh doanh.");
        }
        else if ($("#cbdongy").prop("checked") !== true) {
            alertMessageError("Vui lòng tích chọn đồng ý điều khoản open24.");
        }
        else if (!ValiDatePassword(self.MatKhau())) {
            alertMessageError("Mật khẩu chứa tiếng việt, vui lòng nhập lại.");
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
    function DangKyCuHang(diachi, ip4) {
        $('.fuzzy').show();
        $('.wait-res').show();
        $('#dk_Notification').text('');
        var model = {
            SoDienThoai: localValidate.convertStrFormC(self.SoDienThoai()),
            SubDomain: localValidate.convertStrFormC(self.TenGianHang()),
            TenCuaHang: localValidate.convertStrFormC(self.TenCuaHangKinhDoanh()),
            Email: localValidate.convertStrFormC(self.Email()),
            MaNganhKinhDoanh: self.NganhNgheKey(),
            MatKhauKT: localValidate.convertStrFormC(self.MatKhau()),
            KhuVuc_DK: diachi,
            DiaChiIP_DK: ip4,
            UserKT: localValidate.convertStrFormC(self.TenDangNhap())
        };
        $.ajax({
            data: model,
            url: '/Open24Api/PostAPI/' + "DangKySuDung",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                $('.fuzzy').hide();
                $('.wait-res').hide();
                if (result.res) {
                    $('#dk_Notification').text('');
                    var host = "https://" + window.location.hostname + "/dang-ky-thanh-cong/" + result.DataSoure;
                    window.location = host;
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

    function alertMessageError(mess) {
        $('#dk_Notification').text(mess);
    }

    self.ChangeSoDienThoai = function () {
        var sodienthoai = change_alias(localValidate.convertStrFormC(self.SoDienThoai()));
        if (sodienthoai === '' || sodienthoai === null || sodienthoai === undefined) {
            if (!$('#tendangnhap').find('.p-dk').hasClass('dk-title-input')) {
                $('#tendangnhap').find('.p-dk').addClass('dk-title-input');
                $('#tendangnhap').find('input').hide();
            }
        }
        else {
            $('#tendangnhap').find('.p-dk').removeClass('dk-title-input');
            $('#tendangnhap').find('input').show();

        }
        self.TenDangNhap(sodienthoai);
    }
    $('.drop-nganh-nghe .dropdown-menu a').click(function (e) {
        e.preventDefault();
        self.NganhNgheKey($(this).data('id'));
        self.NghanhNgheValue($(this).text());
        $('.p-nn-dk').removeClass('dk-title-input');
        $(this).closest('.drop-nganh-nghe').closest('.form-input-dk').find('.p-dk').removeClass('dk-title-input');
    setTimeout($.proxy(function () {
        if ('ontouchstart' in document.documentElement) {
            $(this).siblings('.dropdown-backdrop ').off().remove();
        }
    }, this), 0);
    }); 
}
ko.applyBindings(new cuahangdangky());
function CheckEmail(value) {
    debugger
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
function ValiDatePassword( value) {
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
$('.form-drop-nn').on('click', function () {
    $('#select_nganhnghe').toggle();
    $("#select_nganhnghe").mouseup(function () {
        return false
    });
    $(".form-drop-nn").mouseup(function () {
        return false
    });
    $(document).mouseup(function () {
        $("#select_nganhnghe").hide();
    });
});
