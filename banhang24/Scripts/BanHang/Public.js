const LoaiKhuyenMai = {
   HOA_DON:1,
   HANG_HOA:2,
}
const HinhThucKhuyenMai = {
   HD_GIAM_GIAHD:11,
   HD_TANG_HANG:12,
   HD_GIAM_GIA_HANG:13,
   HD_TANG_DIEM:14,

    HH_GIAM_GIA_HANG:21,
   HH_TANG_HANG:22,
   HH_TANG_DIEM:23,
   HH_GIA_BAN_THEO_SO_LUONG:24,
}



function ajaxHelper(uri, method, data) {
    return $.ajax({
        type: method,
        url: uri,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: data ? JSON.stringify(data) : null,
        statusCode: {
            404: function () {
            }
        }
    });
}

function ExpireCookie(minutes) {
    var date = new Date();
    var m = minutes;
    date.setTime(date.getTime() + m * 60 * 1000);
    $.cookie("cookie", "value", { expires: date });
}

function getDataFromCookie(cookieName) {

    var val = $.cookie(cookieName);

    if (!val) {
        return [];
    }
    return JSON.parse(val);
}

function convertCookiesToArray(cookieName) {
    var ckData = getDataFromCookie(cookieName);

    if (ckData === '' || ckData === 'null') {
        console.log(1);
    }
    else {
        ckData = JSON.parse(ckData);
    }
    return ckData;
}

function modalMessage(message) {
    var fClose = function () {
        modal.modal("hide");
    };
    var modal = $("#modalMessage");
    modal.modal("show");
    $("#errMesage").empty().append(message);
    $("#confirmOk").one('click', fClose);
}

function dialogConfirm(title, message, onConfirm) {
    var fClose = function () {
        modal.modal("hide");
        return false;
    };
    var modal = $("#modalPopuplgDelete");
    modal.modal("show");
    $('#header-confirm-delete').empty().append(title);
    $("#confirmMessage").empty().append(message);
    $("#confirmOkDelete").off().one('click', onConfirm);
    $("#confirmCancel").off().one("click", fClose);
}

function dialogConfirm_OKCancel(title, message, onConfirm, fClose) {
    var modal = $("#modalPopuplgDelete");
    modal.modal("show");
    $('#header-confirm-delete').empty().append(title);
    $("#confirmMessage").empty().append(message);
    $("#confirmOkDelete").off().one('click', onConfirm);
    $("#confirmCancel").off().one("click", fClose);
}

function formatNumberObj(obj) {
    var objVal = $(obj).val();
    $(obj).val(objVal.toString().replace(/,/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    return objVal;
}
function formatNumberToInt(objVal) {
    if (objVal === undefined || objVal === null) {
        return 0;
    }
    else {
        var value = parseInt(objVal.toString().replace(/,/g, ''));
        if (isNaN(value)) {
            return 0;
        }
        else {
            return value;
        }
    }
}
function formatNumberToFloat(objVal) {
    if (objVal === undefined || objVal === null) {
        return 0;
    }
    else {
        var value = parseFloat(objVal.toString().replace(/,/g, ''));
        if (isNaN(value)) {
            return 0;
        }
        else {
            return value;
        }
    }
}

//kiểm kho
function formatNumberToFloatKK(objVal) {
    if (objVal === undefined) {
        return null;
    }
    else {
        var value = parseFloat(objVal.toString().replace(/,/g, ''));
        if (isNaN(value)) {
            return null;
        }
        else {
            return value;
        }
    }
}

function formatNumber(number) {
    if (number === undefined || number === null) {
        return 0;
    }
    else {
        return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
}

// lay x so sau dau . (x= decimalDot)
// default: lay 3 so sau dau .
function formatNumber3Digit(number, decimalDot = 2) {
    if (number === undefined || number === null) {
        return 0;
    }
    else {
        number = formatNumberToFloat(number);
        number = Math.round(number * Math.pow(10, decimalDot)) / Math.pow(10, decimalDot);
        if (number !== null) {
            var lastone = number.toString().split('').pop();
            if (lastone !== '.') {
                number = parseFloat(number);
            }
        }
        if (isNaN(number)) {
            number = 0;
        }
        return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
}

function locdau(obj) {
    if (!obj)
        return "";
    var str = obj.trim();;
    str = str.toLowerCase();
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/^\-+|\-+$/g, "");

    // Some system encode vietnamese combining accent as individual utf-8 characters
    str = str.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, ""); // Huyền sắc hỏi ngã nặng 
    str = str.replace(/\u02C6|\u0306|\u031B/g, ""); // Â, Ê, Ă, Ơ, Ư

    return str;
}

function RoundDecimal(data, number = 2) {
    data = Math.round(data * Math.pow(10, number)) / Math.pow(10, number);
    if (data !== null) {
        var lastone = data.toString().split('').pop();
        if (lastone !== '.') {
            data = parseFloat(data);
        }
    }
    if (isNaN(data) || data === Infinity) {
        data = 0;
    }
    return data;
}

function LamTron_denHangTram(number, roundTo = 10) {
    if (number) {
        return Math.ceil(formatNumberToFloat(number) / roundTo) * roundTo;
    }
    else {
        return 0;
    }
}

function chekPhone(e) {

    var keyCode = window.event.keyCode || e.which;
    var arrAscii = [32, 40, 41, 43, 45]; // space,(,),+, -,

    if (keyCode < 48 || keyCode > 57) {
        // 8: tab; 127: delete
        if (keyCode === 8 || keyCode === 127 || arrAscii.indexOf(keyCode) > -1) {
            return;
        }
        return false;
    }
}

function CheckIsNumber(input) {
    if (input !== null && input !== undefined) {
        var regx = /^\d+$/;
        return regx.test(input);
    }
    else {
        return false;
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

function keypressNumberSign(e) {
    var keyCode = window.event.keyCode || e.which;
    if (keyCode < 48 || keyCode > 57) {
        // 8: tab; 127: delete, 45: -
        if (keyCode === 8 || keyCode === 127 || keyCode === 45) {
            return;
        }
        return false;
    }
}

// cho phep nhap 3 so sau phan thap phan
function keypressNumber_limitNumber(event, obj) {

    var keyCode = event.keyCode || event.which;
    var $this = $(obj).val();

    // 46(.), 48(0), 57(9)
    if ((keyCode !== 46 || $(obj).val().indexOf('.') !== -1) && (keyCode < 48 || keyCode > 57)) {
        if (event.which !== 46 || $this.indexOf('.') !== -1) {
            //alert('Chỉ được nhập một dấu .');
        }
        event.preventDefault();
    }
    // get postion current of cursor
    var pos = $(obj).getCursorPosition();
    if ($this.indexOf(".") > -1 && $this.split('.')[1].length > 2) {

        var lenNumber = $this.length;
        // if pos nam sau chu so thap phan --> khong cho add them so nua
        if (pos > lenNumber - 3) {
            event.preventDefault();
        }
    }
}

function hidewait(className) {
    $('.' + className).append('<div id="wait"><img src="/Content/images/wait.gif" width="64" height="64" /><div class="happy-wait">' +
        ' </div>' +
        '</div>');
}
function getRandomColor() {
    var letters = '0123456789ABCDEF';
    var color = '#';
    var colorNotUse = ["#3E4E0C", "#FFFFF", "#BDDD06", "#D5E125", "#D9E8E1", "#D4D2BE", "#AEE8DA", "#ACE557", "#C0E7C2", "#D4E7D7", "#DFE740",
        "#EEE8E8", "#D9E68F", "#F2E2F3", "#C3F3BB", "#D0E6AA", "#BCE9AF", "#DBE8E2", "#E5E5C2", "#F5E1FC", "#FFFBBB"];
    for (var i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    if ($.inArray(color, colorNotUse) > -1) {
        color = '#6F4D2A';
    }
    return color;
}

function getRandomColor_Temp(i, numberColor) {
    numberColor = numberColor || 7;
    //var colorUse = ["#64656e", "#1a9446", "#ba1af9", "#fe4b47", "#1a739a", "#979a4b", "#5b5dbc"];
    var colorUse = ['rgb(141,26,167,0.6)', 'rgb(207, 65, 59,0.6)', 'rgb(31, 92, 114,0.6)', 'rgb(101, 97, 44,0.6)', 'rgb(71, 66, 152,0.6)', 'rgb(76, 77, 86,0.6)', 'rgb(26, 101, 63,0.6)'];
    var div = i % numberColor;
    color = colorUse[div];
    return color;
}

function getDateNow() {
    var today = new Date();
    var date = today.getFullYear() + '-' + (today.getMonth() + 1) + '-' + today.getDate();
    var time = today.getHours() + ":" + today.getMinutes() + ":" + today.getSeconds();
    var dateTime = date + ' ' + time;
    return dateTime;
}

var mangso = ['không', 'một', 'hai', 'ba', 'bốn', 'năm', 'sáu', 'bảy', 'tám', 'chín'];
function DocHangChuc(so, daydu) {
    var chuoi = "";
    chuc = Math.floor(so / 10);
    donvi = so % 10;
    if (chuc > 1) {
        chuoi = " " + mangso[chuc] + " mươi";
        if (donvi === 1) {
            chuoi += " mốt";
        }
    } else if (chuc === 1) {
        chuoi = " mười";
        if (donvi === 1) {
            chuoi += " một";
        }
    } else if (daydu && donvi > 0) {
        chuoi = " lẻ";
    }
    if (donvi === 5 && chuc >= 1) {
        chuoi += " lăm";
    } else if (donvi > 1 || donvi === 1 && chuc === 0) {
        chuoi += " " + mangso[donvi];
    }
    return chuoi;
}
function DocHangTram(so, daydu) {
    var chuoi = "";
    tram = Math.floor(so / 100);
    so = so % 100;
    if (daydu || tram > 0) {
        chuoi = " " + mangso[tram] + " trăm";
        chuoi += DocHangChuc(so, true);
    } else {
        chuoi = DocHangChuc(so, false);
    }
    return chuoi;
}
function DocHangTrieu(so, daydu) {
    var chuoi = "";
    trieu = Math.floor(so / 1000000);
    so = so % 1000000;
    if (trieu > 0) {
        chuoi = DocHangTram(trieu, daydu) + " triệu";
        daydu = true;
    }
    nghin = Math.floor(so / 1000);
    so = so % 1000;
    if (nghin > 0) {
        chuoi += DocHangTram(nghin, daydu) + " nghìn";
        daydu = true;
    }
    if (so > 0) {
        chuoi += DocHangTram(so, daydu);
    }
    return chuoi;
}
function DocSo(so) {
    if (so === 0) return mangso[0];
    var chuoi = "", hauto = "";
    do {
        ty = so % 1000000000;
        so = Math.floor(so / 1000000000);
        if (so > 0) {
            chuoi = DocHangTrieu(ty, true) + hauto + chuoi;
        } else {
            chuoi = DocHangTrieu(ty, false) + hauto + chuoi;
        }
        hauto = " tỷ";
    } while (so > 0);
    return chuoi.trim().substr(0, 1).toUpperCase() + chuoi.substr(2) + ' đồng'; /*; chuoi + ' đồng';*/
}

function GetChartStart(input) {
    if (input === '' || input === null || input.replace(/\s+/g, '') === '') {
        return null;
    }
    else {
        var inputLocdau = locdau(input);
        var result = inputLocdau.split(' ').filter(word => word !== '');
        var output = '';
        $.each(result, function (index, value) {
            output += value.substring(0, 1);
        });
        return output.toLowerCase();
    }
}

// set postion for cursor
$.fn.setCursorPosition = function (pos) {
    this.each(function (index, elem) {
        // surport with IE > 9
        if (elem.setSelectionRange) {
            elem.setSelectionRange(pos, pos);
        } else if (elem.createTextRange) {
            var range = elem.createTextRange();
            range.collapse(true);
            range.moveEnd('character', pos);
            range.moveStart('character', pos);
            range.select();
        }
    });
    return this;
};

// get postion of cursor
$.fn.getCursorPosition = function () {
    var el = $(this).get(0);
    var pos = 0;
    if ('selectionStart' in el) {
        pos = el.selectionStart;
    } else if ('selection' in document) {
        el.focus();
        var Sel = document.selection.createRange();
        var SelLength = document.selection.createRange().text.length;
        Sel.moveStart('character', -el.value.length);
        pos = Sel.text.length - SelLength;
    }
    return pos;
};

function Remove_LastComma(str) {
    if (str !== null && str !== undefined && str.length > 1) {
        return str.replace(/(^[,\s]+)|([,\s]+$)/g, '');
    }
    else {
        return '';
    }
}

function isValidDateYYYYMMDD(dateString) {
    var regEx = /^\d{4}-\d{2}-\d{2}$/;
    if (!dateString.match(regEx)) return false;  // Invalid format
    var d = new Date(dateString);
    if (!d.getTime() && d.getTime() !== 0) return false; // Invalid date
    return d.toISOString().slice(0, 10) === dateString;
}

function ValidateEmail(email) {
    if (email !== null && email !== undefined) {
        var re = /^(([^<>()[\]\\.,;:\s@#ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴ\"]+(\.[^<>()[\]\\.,;:\s@#ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴ\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/i;
        return re.test(email);
    }
    else {
        return false;
    }
}

// count elem (ex: {ID:xxx, count: 2})
function compressArray(original) {
    var compressed = [];
    // make a copy of the input array
    var copy = original.slice(0);

    // first loop goes over every element
    for (var i = 0; i < original.length; i++) {

        var myCount = 0;
        // loop over every element in the copy and see if it's the same
        for (var w = 0; w < copy.length; w++) {
            if (original[i] === copy[w]) {
                // increase amount of times duplicate is found
                myCount++;
                // sets item to undefined
                delete copy[w];
            }
        }

        if (myCount > 0) {
            var a = new Object();
            a.value = original[i];
            a.count = myCount;
            compressed.push(a);
        }
    }

    return compressed;
}

function ReplaceCTHD(content) {
    content = content.allReplace(
        {
            '{MaHangHoa}': '<span data-bind=\"text: MaHangHoa\"></span>',
            '{TenHangHoa}': '<span data-bind=\"text: TenHangHoa\"></span>',
            '{TenHangHoaThayThe}': '<span data-bind=\"text: TenHangHoaThayThe\"></span>',
            '{GiaVonHienTai}': '<span data-bind=\"text: GiaVonHienTai\"></span>',
            '{GiaVonMoi}': '<span data-bind=\"text: GiaVonMoi\"></span>',
            '{ChenhLech}': '<span data-bind=\"text: ChenhLech\"></span>',
            '{DonViTinh}': '<span data-bind=\"text: DonViTinh\"></span>',
            '{DonGia}': '<span data-bind=\"text: DonGia\"></span>',
            '{SoLuong}': '<span data-bind=\"text: SoLuong\"></span>',
            '{PTChietKhauHH}': '<span data-bind=\"text: PTChietKhau\"></span>',
            '{GiamGia}': '<span data-bind=\"text: TienChietKhau\"></span>',
            '{GiaBan}': '<span data-bind=\"text: GiaBan\"></span>',
            '{ThanhTienTruocCK}': '<span data-bind=\"text: ThanhTienTruocCK\"></span>',
            '{ThanhToan}': '<span data-bind=\"text: ThanhToan\"></span>',
            '{ThanhTien}': '<span data-bind=\"text: ThanhTien\"></span>',
            '{TonKho}': '<span data-bind=\"text: TonKho\"></span>',
            '{KThucTe}': '<span data-bind=\"text: KThucTe\"></span>',
            '{SLLech}': '<span data-bind=\"text: SLLech\"></span>',
            '{GiaTriLech}': '<span data-bind=\"text: GiaTriLech\"></span>',
            '{ThuocTinh_GiaTri}': '<span data-bind=\"text: ThuocTinh_GiaTri\"></span>',
            '{SLDVDaSuDung}': '<span data-bind=\"text: SoLuongDVDaSuDung\"></span>',
            '{SLDVConLai}': '<span data-bind=\"text: SoLuongDVConLai\"></span>',
            '{GhiChu}': '<span data-bind=\"text: GhiChu\"></span>',
            '{TenViTri}': '<span data-bind=\"text: TenViTri\"></span>',
            '{ThoiGianBatDau}': '<span data-bind=\"text: TimeStart\"></span>',
            '{SoPhutThucHien}': '<span data-bind=\"text: ThoiGianThucHien\"></span>',
            '{QuaThoiGian}': '<span data-bind=\"text: QuaThoiGian\"></span>',
            '{GhiChuHH}': '<span data-bind=\"text: GhiChuHH\"></span>',
            '{PTThue}': '<span data-bind=\"text: PTThue\"></span>',
            '{TienThue}': '<span data-bind=\"text: TienThue\"></span>',
            '{DonGiaBaoHiem}': '<span data-bind=\"text: DonGiaBaoHiem\"></span>',
            '{BH_ThanhTien}': '<span data-bind=\"text: BH_ThanhTien\"></span>',

            '{GhiChu_NVThucHien}': '<span data-bind=\"text: GhiChu_NVThucHien\"></span>',
            '{GhiChu_NVTuVan}': '<span data-bind=\"text: GhiChu_NVTuVan\"></span>',
            '{NVTuVanDV_CoCK}': '<span data-bind=\"text: NVTuVanDV_CoCK\"></span>',
            '{NVThucHienDV_CoCK}': '<span data-bind=\"text: NVThucHienDV_CoCK\"></span>',

            '{SoLuongChuyen}': '<span data-bind=\"text: SoLuongChuyen\"></span>',
            '{SoLuongNhan}': '<span data-bind=\"text: SoLuongNhan\"></span>',
            '{GiaChuyen}': '<span data-bind=\"text: GiaChuyen\"></span>',

            '{SoLuongHuy}': '<span data-bind=\"text: SoLuongHuy\"></span>',
            '{GiaVon}': '<span data-bind=\"text: GiaVon\"></span>',
            '{GiaTriHuy}': '<span data-bind=\"text: GiaTriHuy\"></span>',
        });
    return content;
}


String.prototype.allReplace = function (obj) {
    var retStr = this;
    for (var x in obj) {
        retStr = retStr.replace(new RegExp(x, 'g'), obj[x]);
    }
    return retStr;
};


// print HoaDon
function ReplaceString_toData(content1) {

    content1 = content1.replace("{TenCuaHang}", "<span data-bind=\"text: InforHDprintf().TenCuaHang\"></span>");
    content1 = content1.replace("{TenChiNhanh}", "<span data-bind=\"text: InforHDprintf().TenChiNhanh\"></span>");
    content1 = content1.replace("{DienThoaiChiNhanh}", "<span data-bind=\"text: InforHDprintf().DienThoaiChiNhanh\"></span>");
    content1 = content1.replace("{DiaChiChiNhanh}", "<span data-bind=\"text: InforHDprintf().DiaChiChiNhanh\"></span>");
    content1 = content1.replace("{Logo}", "<img data-bind=\"attr: {src: InforHDprintf().LogoCuaHang}\" style=\"width:100% \" />");

    // thong tin TK ngan hang cua cua hang
    content1 = content1.replace("{TenNganHangPOS}", "<span data-bind=\"text: InforHDprintf().TenNganHangPOS\"></span>");
    content1 = content1.replace("{TenChuThePOS}", "<span data-bind=\"text: InforHDprintf().TenChuThePOS\"></span>");
    content1 = content1.replace("{SoTaiKhoanPOS}", "<span data-bind=\"text: InforHDprintf().SoTaiKhoanPOS\"></span>");
    content1 = content1.replace("{TenNganHangChuyenKhoan}", "<span data-bind=\"text: InforHDprintf().TenNganHangChuyenKhoan\"></span>");
    content1 = content1.replace("{TenChuTheChuyenKhoan}", "<span data-bind=\"text: InforHDprintf().TenChuTheChuyenKhoan\"></span>");
    content1 = content1.replace("{SoTaiKhoanChuyenKhoan}", "<span data-bind=\"text: InforHDprintf().SoTaiKhoanChuyenKhoan\"></span>");

    content1 = content1.replace("{NgayBan}", "<span data-bind=\"text: InforHDprintf().NgayLapHoaDon\"></span>");
    content1 = content1.replace("{NgayLapHoaDon}", "<span data-bind=\"text: InforHDprintf().NgayLapHoaDon\"></span>");
    content1 = content1.replace("{MaHoaDon}", "<span data-bind=\"text: InforHDprintf().MaHoaDon\"></span>");// hdgoc (when trahang or hdbaogia)
    content1 = content1.replace("{MaHoaDonTraHang}", "<span data-bind=\"text: InforHDprintf().MaHoaDonTraHang\"></span>");
    content1 = content1.replace("{MaKhachHang}", "<span data-bind=\"text: InforHDprintf().MaDoiTuong\"></span>");
    content1 = content1.replace(/{TenKhachHang}/g, "<span data-bind=\"text: InforHDprintf().TenDoiTuong\"></span>");
    content1 = content1.replace("{NgaySinhKH}", "<span data-bind=\"text: InforHDprintf().NgaySinh_NgayTLap\"></span>");
    content1 = content1.replace("{TenNhaCungCap}", "<span data-bind=\"text: InforHDprintf().TenDoiTuong\"></span>");
    content1 = content1.replace("{DiaChi}", "<span data-bind=\"text: InforHDprintf().DiaChiKhachHang\"></span>");
    content1 = content1.replace("{DienThoai}", "<span data-bind=\"text: InforHDprintf().DienThoaiKhachHang\"></span>");
    content1 = content1.replace("{MaSoThue}", "<span data-bind=\"text: InforHDprintf().MaSoThue\"></span>");
    content1 = content1.replace("{TongDiemKhachHang}", "<span data-bind=\"text: InforHDprintf().TongTichDiem\"></span>");
    content1 = content1.replace("{NhanVienBanHang}", "<span data-bind=\"text: InforHDprintf().NhanVienBanHang\"></span>");
    content1 = content1.replace("{TenPhongBan}", "<span data-bind=\"text: InforHDprintf().TenPhongBan\"></span>");
    content1 = content1.replace("{NguoiTao}", "<span data-bind=\"text: InforHDprintf().NguoiTaoHD\"></span>");
    content1 = content1.replace("{TongTaiKhoanThe}", "<span data-bind=\"text: InforHDprintf().TongTaiKhoanThe\"></span>");
    content1 = content1.replace("{TongSuDungThe}", "<span data-bind=\"text: InforHDprintf().TongSuDungThe\"></span>");
    content1 = content1.replace("{SoDuConLai}", "<span data-bind=\"text: InforHDprintf().SoDuConLai\"></span>");
    content1 = content1.replace("{TenViTriHD}", "<span data-bind=\"text: InforHDprintf().TenViTriHD\"></span>");

    content1 = content1.replace("{DienGiai}", "<span data-bind=\"text: $root.InforHDprintf().DienGiai\"></span>");
    content1 = content1.replace("{TongTienHang}", "<span data-bind=\"text: $root.InforHDprintf().TongTienHang\"></span>");
    content1 = content1.replace("{TongTienHDSauGiamGia}", "<span data-bind=\"text:  $root.InforHDprintf().TongTienHDSauGiamGia\"></span>");
    content1 = content1.replace("{DaThanhToan}", "<span data-bind=\"text:  $root.InforHDprintf().DaThanhToan\"></span>");
    content1 = content1.replace("{PTChietKhauHD}", "<span data-bind=\"text:  $root.InforHDprintf().TongChietKhau\"></span>");
    content1 = content1.replace("{ChietKhauHoaDon}", "<span data-bind=\"text:  $root.InforHDprintf().TongGiamGia\"></span>");
    content1 = content1.replace("{DiaChiCuaHang}", "<span data-bind=\"text:  $root.InforHDprintf().DiaChiCuaHang\"></span>");
    content1 = content1.replace("{PhiTraHang}", "<span data-bind=\"text:  $root.InforHDprintf().TongChiPhi\"></span>");
    content1 = content1.replace("{TongTienThue}", "<span data-bind=\"text:  $root.InforHDprintf().TongTienThue\"></span>");
    content1 = content1.replace("{PTThueHoaDon}", "<span data-bind=\"text:  $root.InforHDprintf().PTThueHoaDon\"></span>");
    content1 = content1.replace("{ThueTraLai}", "<span data-bind=\"text:  $root.InforHDprintf().TongThueDB\"></span>");

    content1 = content1.replace("{TongTienTraHang}", "<span data-bind=\"text:  $root.InforHDprintf().TongTienTraHang\"></span>");
    content1 = content1.replace("{TongTienTra}", "<span data-bind=\"text: formatNumber( $root.InforHDprintf().TongTienTra)\"></span>");
    content1 = content1.replace("{TongCong}", "<span data-bind=\"text:  $root.InforHDprintf().TongCong\"></span>");
    content1 = content1.replace("{TongSoLuongHang}", "<span data-bind=\"text:  $root.InforHDprintf().TongSoLuongHang\"></span>");
    content1 = content1.replace("{ChiPhiNhap}", "<span data-bind=\"text: InforHDprintf().ChiPhiNhap\"></span>");
    content1 = content1.replace("{NoTruoc}", "<span data-bind=\"text:  $root.InforHDprintf().NoTruoc\"></span>");
    content1 = content1.replace("{NoSau}", "<span data-bind=\"text:  $root.InforHDprintf().NoSau\"></span>");
    content1 = content1.replace("{TienThuaTraKhach}", "<span data-bind=\"text:  $root.InforHDprintf().TienThua\"></span>");
    content1 = content1.replace("{TienKhachThieu}", "<span data-bind=\"text:  $root.InforHDprintf().TienKhachThieu\"></span>");
    content1 = content1.replace("{DiemGiaoDich}", "<span data-bind=\"text:  $root.InforHDprintf().DiemGiaoDich\"></span>");
    content1 = content1.replace("{TienPOS}", "<span data-bind=\"text:  $root.InforHDprintf().TienATM\"></span>");
    content1 = content1.replace("{TienMat}", "<span data-bind=\"text:  $root.InforHDprintf().TienMat\"></span>");
    content1 = content1.replace("{TienChuyenKhoan}", "<span data-bind=\"text:  $root.InforHDprintf().TienGui\"></span>");
    content1 = content1.replace("{TienDoiDiem}", "<span data-bind=\"text:  $root.InforHDprintf().TienDoiDiem\"></span>");
    content1 = content1.replace("{TienTheGiaTri}", "<span data-bind=\"text:  $root.InforHDprintf().TienTheGiaTri\"></span>");
    content1 = content1.replace("{TongGiamGiaHang}", "<span data-bind=\"text:  $root.InforHDprintf().TongGiamGiaHang\"></span>");
    content1 = content1.replace("{TongTienHangChuaChietKhau}", "<span data-bind=\"text:  $root.InforHDprintf().TongTienHangChuaCK\"></span>");
    content1 = content1.replace("{TongGiamGiaHD_HH}", "<span data-bind=\"text:  $root.InforHDprintf().TongGiamGiaHD_HH\"></span>");
    content1 = content1.replace("{TenNhomKhach}", "<span data-bind=\"text: InforHDprintf().TenNhomKhach\"></span>");
    content1 = content1.replace("{ChietKhauNVHoaDon}", "<span data-bind=\"text: InforHDprintf().ChietKhauNVHoaDon\"></span>");
    content1 = content1.replace("{ChietKhauNVHoaDon_InGtriCK}", "<span data-bind=\"text: InforHDprintf().ChietKhauNVHoaDon_InGtriCK\"></span>");

    // ChuyenHang
    content1 = content1.replace("{ChiNhanhChuyen}", "<span data-bind=\"text: InforHDprintf().ChiNhanhChuyen\"></span>");
    content1 = content1.replace("{NguoiChuyen}", "<span data-bind=\"text: InforHDprintf().NguoiChuyen\"></span>");
    content1 = content1.replace("{ChiNhanhNhan}", "<span data-bind=\"text: InforHDprintf().ChiNhanhNhan\"></span>");
    content1 = content1.replace("{NguoiNhan}", "<span data-bind=\"text: InforHDprintf().NguoiNhan\"></span>");
    content1 = content1.replace("{MaChuyenHang}", "<span data-bind=\"text: InforHDprintf().MaHoaDon\"></span>");
    content1 = content1.replace("{GhiChuChiNhanhChuyen}", "<span data-bind=\"text: InforHDprintf().GhiChuChiNhanhChuyen\"></span>");
    content1 = content1.replace("{TongSoLuongChuyen}", "<span data-bind=\"text: InforHDprintf().TongSoLuongChuyen\"></span>");
    content1 = content1.replace("{TongSoLuongNhan}", "<span data-bind=\"text: InforHDprintf().TongSoLuongNhan\"></span>");
    content1 = content1.replace("{TongTienChuyen}", "<span data-bind=\"text: InforHDprintf().TongTienChuyen\"></span>");
    content1 = content1.replace("{TongTienNhan}", "<span data-bind=\"text: InforHDprintf().TongTienNhan\"></span>");

    // phieu thu, chi
    content1 = content1.replace("{MaPhieu}", "<span data-bind=\"text: InforHDprintf().MaPhieu\"></span>");
    content1 = content1.replace("{NguoiNopTien}", "<span data-bind=\"text: InforHDprintf().NguoiNopTien\"></span>");
    content1 = content1.replace("{NguoiNhanTien}", "<span data-bind=\"text: InforHDprintf().NguoiNopTien\"></span>");
    content1 = content1.replace("{GiaTriPhieu}", "<span data-bind=\"text: InforHDprintf().GiaTriPhieu\"></span>");
    content1 = content1.replace("{NguoiNhan}", "<span data-bind=\"text: InforHDprintf().NguoiNhan\"></span>");
    content1 = content1.replace("{MaChuyenHang}", "<span data-bind=\"text: InforHDprintf().MaHoaDon\"></span>");
    content1 = content1.replace("{NoiDungThu}", "<span data-bind=\"text: InforHDprintf().NoiDungThu\"></span>");
    content1 = content1.replace("{TienBangChu}", "<span data-bind=\"text: InforHDprintf().TienBangChu\"></span>");
    content1 = content1.replace("{GhiChuChiNhanhChuyen}", "<span data-bind=\"text: InforHDprintf().GhiChuChiNhanhChuyen\"></span>");
    content1 = content1.replace("{ChiNhanhBanHang}", "<span data-bind=\"text: InforHDprintf().TenChiNhanh\"></span>");
    content1 = content1.replace("{HoaDonLienQuan}", "<span data-bind=\"text: InforHDprintf().HoaDonLienQuan\"></span>");

    // HD Doi Tra
    content1 = content1.replace("{TongTienHoaDonMua}", "<span data-bind=\"text:  $root.InforHDprintf().TongTienHoaDonMua\"></span>");
    content1 = content1.replace("{TienTraKhach}", "<span data-bind=\"text:  $root.InforHDprintf().PhaiTraKhach\"></span>");
    content1 = content1.replace("{KhachCanTra}", "<span data-bind=\"text:  $root.InforHDprintf().PhaiThanhToan\"></span>");

    content1 = content1.replace("{Ngay}", "<span data-bind=\"text: InforHDprintf().Ngay\"></span>");
    content1 = content1.replace("{Thang}", "<span data-bind=\"text: InforHDprintf().Thang\"></span>");
    content1 = content1.replace("{Nam}", "<span data-bind=\"text: InforHDprintf().Nam\"></span>");

    if (content1.indexOf('{TenHangHoaMoi}') > -1) {
        var open = content1.lastIndexOf("tbody", content1.indexOf("{TenHangHoa}")) - 1;
        var close = content1.indexOf("tbody", content1.indexOf("{TenHangHoa}")) + 6;
        let temptable = content1.substr(open, close - open);
        let temptable1 = temptable;

        temptable = temptable.allReplace(
            {
                'tbody': 'tbody data-bind=\"foreach: CTHoaDonPrint\"',
                '{STT}': '<span data-bind=\"text: SoThuTu\"></span>'
            });
        temptable = ReplaceCTHD(temptable);
        content1 = content1.replace(temptable1, temptable);

        var openTbl2 = content1.lastIndexOf("tbody", content1.indexOf("{TenHangHoaMoi}")) - 1;
        var closeTbl2 = content1.indexOf("tbody", content1.indexOf("{TenHangHoaMoi}")) + 6;
        var temptable2 = content1.substr(openTbl2, closeTbl2 - openTbl2);
        var temptableMH = temptable2;

        temptable2 = temptable2.allReplace(
            {
                'tbody': 'tbody data-bind=\"foreach: CTHoaDonPrintMH\"',
                '{STT}': '<span data-bind=\"text: SoThuTu\"></span>',
                '{TenHangHoaMoi}': '<span data-bind=\"text: TenHangHoa\"></span>'
            });
        temptable2 = ReplaceCTHD(temptable2);
        content1 = content1.replace(temptableMH, temptable2);
    }
    else {
        let open = content1.lastIndexOf("tbody", content1.indexOf("{TenHangHoa")) - 1;
        let close = content1.indexOf("tbody", content1.indexOf("{TenHangHoa")) + 6;
        let temptable = content1.substr(open, close - open);
        let temptable1 = temptable;

        let row1From = temptable.indexOf("<tr");
        let row1To = temptable.indexOf("/tr>") - 4;
        let row1Str = temptable.substr(row1From, row1To);
        let row1 = row1Str;

        let nextRowFrom = row1To;

        row1Str = ''.concat(" <!--ko foreach: $data.CTHoaDonPrint --> ", row1Str);
        if (row1Str.indexOf("{SoLuong") > -1 || row1Str.indexOf("{GiaVon") > -1
            || row1Str.indexOf("{DonGia}") > -1) {
            row1Str = ''.concat(row1Str, "<!--/ko-->");
            ReplaceDetail();
        }
        else {
            CheckRowNextIn();
        }

        function CheckRowNextIn() {
            let nextRowTo = temptable.indexOf("<tr", nextRowFrom + 1);
            if (nextRowTo < 0) {
                nextRowTo = temptable.lastIndexOf("/tr>") + 5;
            }
            let nextRowStr = temptable.substr(nextRowFrom, nextRowTo - nextRowFrom);
            let nextRow = nextRowStr;

            if (nextRowStr.indexOf("{SoLuong") > -1 || nextRowStr.indexOf("{DonGia}") > -1) {
                nextRowStr = ''.concat(nextRowStr, "<!--/ko-->");
                temptable = temptable.replace(nextRow, nextRowStr);
                ReplaceDetail();
            }
            else {
                nextRowFrom = nextRowTo;
                CheckRowNextIn();
            }
        }

        function ReplaceDetail() {
            temptable = temptable.replace(row1, row1Str);
            temptable = temptable.replace("{STT}", "<span data-bind=\"text: SoThuTu\"></span>");
            temptable = ReplaceCTHD(temptable);
            content1 = content1.replace(temptable1, temptable);
        }
    }
    return content1;
}

function PrintExtraReport_Multiple(dataContent) {
    var frame1 = $('<iframe />');
    frame1[0].name = "frame1";
    frame1.css({ "position": "absolute", "top": "-100000px" });
    $("body").append(frame1);
    var frameDoc = frame1[0].contentWindow ? frame1[0].contentWindow : frame1[0].contentDocument.document ? frame1[0].contentDocument.document : frame1[0].contentDocument;
    frameDoc.document.open();
    //Create a new HTML document.
    frameDoc.document.write('<html><head>');
    //Append the external CSS file.
    frameDoc.document.write('</head><body><div style="width:96%">');
    //Append the DIV contents.
    frameDoc.document.write(dataContent);
    //console.log('content ', content)
    frameDoc.document.write('</div></body></html>');
    frameDoc.document.close();
    setTimeout(function () {
        window.frames["frame1"].focus();
        window.frames["frame1"].print();
        frame1.remove();
    }, 500);
}

function CheckChar_Special(str) {
    if (str !== undefined && str !== null) {
        // các kí tự trong khoảng từ a-z hoặc A-Z hoạc 0-9, hoặc kí tự gạch dưới _, gạch ngang -, ngoặc tròn (), ngặc vuông [], dấu chấm
        var regx = /[^A-Za-z0-9_*\[\]\(\)\-\.]/;
        // nếu xâu nhập vào tồn tại 1 kí tự không thuộc những kí tự trên --> retun true
        return regx.test(str.toString().trim());
    }
    else {
        return false;
    }
}

function Is_undefined_empty_GuidEmpty(strID) {
    if (strID !== undefined && strID !== null && strID !== '' && strID.indexOf('00000000-0000-0000-0000-000000000000') === -1) {
        return false;
    }
    else {
        return true;
    }
}

// Don Gia 3 chu so thap phan
// use if ptGiam > 0
function Caculator_Price(ptGiam, soluong, thanhtien) {
    var price = thanhtien * 100 / (soluong * (100 - ptGiam));
    price = Math.round(price * Math.pow(10, 3)) / Math.pow(10, 3);
    var lastone = price.toString().split('').pop();
    if (lastone !== '.') {
        price = parseFloat(price);
    }
    return price;
}

// use if ptGiam == 0
function Caculator_Price_byTienGiam(tienGiam, soluong, thanhtien) {
    var price = (thanhtien + soluong * tienGiam) / soluong;
    price = price * Math.pow(10, 3) / Math.pow(10, 3);// Math.pow: lũy thừa 10 mũ 3
    var lastone = price.toString().split('').pop();
    if (lastone !== '.') {
        price = parseFloat(price);
    }
    return price;
}

// same containsAll in BanHang.js
function SearchTxt_inVue(needles, haystack) {
    needles = $.grep(needles, function (x) {
        return x.replace(/[&\/\\#,+()$~%.'":*?<>{}@]/g, '');
    });
    for (var i = 0, len = needles.length; i < len; i++) {
        if (needles[i] === '') continue;
        if (locdau(haystack).search(new RegExp(locdau(needles[i]), "i")) < 0) return false;
    }
    return true;
}

// use when check into row --> delete many (lt DoiTuong, HoaDon, Lienhe)
$('.choose-commodity').on('addClassForButtonNew', function () {

    $('.add-new').addClass("no-magrin");
});
$('.choose-commodity').on('RemoveClassForButtonNew', function () {

    $('.add-new').removeClass("no-magrin");
});

var reTest = /\(.*?\)\)?/;

function ShowMessage_Danger(msg) {
    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + msg, "danger");
}

function ShowMessage_Success(msg) {
    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + msg, "success");
}

function Insert_NhatKyThaoTac_1Param(objNhatKy) {
    var myDataNK = {
        objDiary: objNhatKy,
    };
    ajaxHelper('/api/DanhMuc/SaveDiary/post_NhatKySuDung', 'POST', myDataNK).done(function (x) {

    })
}

// use trigger to do update GiaVon
function Post_NhatKySuDung_UpdateGiaVon(objNhatKy) {
    var myDataNK = {
        objDiary: objNhatKy
    };
    ajaxHelper('/api/DanhMuc/SaveDiary/Post_NhatKySuDung_UpdateGiaVon', 'POST', myDataNK).done(function (x) {

    })
}

function CreateIDRandom(firstChar) {
    var uniqueId = Math.random().toString(36).substring(2)
        + (new Date()).getTime().toString(36);
    return firstChar + uniqueId;
}

function FormatDatetime_AMPM(date) {
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ' ' + ampm;
    return strTime;
}

function FormatDatetime_24Hours(date) {
    var hours = date.getHours();
    var minutes = date.getMinutes();
    hours = hours < 10 ? '0' + hours : hours;
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes;
    return strTime;
}

function ConvertTimeFrom12To24(amPmString) {
    // '1/6/2019' ngày bất kỳ, thích gõ ngày nào thì gõ :)
    var d = new Date("1/6/2019 " + amPmString);
    return d.getHours() + ':' + d.getMinutes();
}

function ConvertTimeFrom24To12(time24) {
    var ts = time24;
    var H = +ts.substr(0, 2);
    var h = H % 12 || 12;
    h = h < 10 ? "0" + h : h;  // leading 0 at the left for 1 digit hours
    var ampm = H < 12 ? " AM" : " PM";
    ts = h + ts.substr(2, 3) + ampm;
    return ts;
}

// use when lap phieu thu (default ID_NhanVien = null)
var newQuyChiTiet = function (obj) {
    let tienThu = obj.TienThu || 0;
    let tiencoc = obj.TienCoc || 0;
    let tienMat = obj.TienMat || 0;
    let pos = obj.TienPOS || 0;
    let ck = obj.TienChuyenKhoan || 0;
    let thegiatri = obj.TienTheGiaTri || 0;
    let diemQD = obj.DiemThanhToan || 0;
    let idKhoanThu = obj.ID_KhoanThuChi || null;
    let idTaiKhoanNganHang = obj.ID_TaiKhoanNganHang || null;
    let idDoiTuong = obj.ID_DoiTuong || null;
    let idHoaDon = obj.ID_HoaDonLienQuan || null;
    let idNhanVien = obj.ID_NhanVien || null;
    let ghichu = obj.GhiChu || '';

    let tienGui = 0;
    let thuTuThe = 0;
    let hinhthuc = obj.HinhThucThanhToan || 0; // 1.mat, 2.pos, 3.ck, 4.the, 5.diem, 6.coc
    let loaiThanhToan = obj.LoaiThanhToan || 0;

    switch (hinhthuc) {
        case 2:
            tienGui = pos;
            break;
        case 3:
            tienGui = ck;
            break;
        case 4:
            thuTuThe = thegiatri;
            break;
        case 6:
            thuTuThe = tiencoc;
            break;
    }

    return {
        ID_NhanVien: idNhanVien,
        ID_DoiTuong: idDoiTuong,
        TienMat: tienMat,
        TienGui: tienGui,
        ThuTuThe: thuTuThe,
        TienThu: tienThu,
        GhiChu: ghichu,
        ID_HoaDonLienQuan: idHoaDon,
        ID_KhoanThuChi: idKhoanThu,
        ID_TaiKhoanNganHang: idTaiKhoanNganHang,
        DiemThanhToan: diemQD,
        HinhThucThanhToan: hinhthuc,
        LoaiThanhToan: loaiThanhToan,//0. default, 1.coc
    };
};

// use when setup discount staff
var newBH_NhanVienThucHien = function (itemNV, idHoaDon) {
    return {
        ID_HoaDon: idHoaDon,
        ID_NhanVien: itemNV.ID_NhanVien,
        PT_ChietKhau: itemNV.PT_ChietKhau,
        TienChietKhau: itemNV.TienChietKhau,
        ThucHien_TuVan: itemNV.ThucHien_TuVan,
        TheoYeuCau: itemNV.TheoYeuCau,
        TinhChietKhauTheo: itemNV.TinhChietKhauTheo,
        HeSo: itemNV.HeSo
    };
};

function GroupCTHD_byIDQuyDoi(arrCTHD) {
        // group by idDonViQuyDoi, id_Lohang
        let group_arrIDQuyDoi = [], group_arrIDLoHang = [];
        let arrCTNew = [];
        for (let i = 0; i < arrCTHD.length; i++) {
            let itFor = arrCTHD[i];
            if (itFor.LoaiHangHoa === 1) {
                if ($.inArray(itFor.ID_DonViQuiDoi, group_arrIDQuyDoi) === -1) {
                    group_arrIDQuyDoi.push(itFor.ID_DonViQuiDoi);

                    if (itFor.QuanLyTheoLoHang) {
                        if ($.inArray(itFor.ID_LoHang, group_arrIDLoHang) === -1) {
                            group_arrIDLoHang.push(itFor.ID_LoHang);
                            let obj = {
                                TenHangHoa: itFor.TenHangHoa,
                                MaHangHoa: itFor.MaHangHoa,
                                MaLoHang: itFor.MaLoHang,
                                ID_DonViQuiDoi: itFor.ID_DonViQuiDoi,
                                ID_LoHang: itFor.ID_LoHang,
                                SoLuong: formatNumberToFloat(itFor.SoLuong),
                                QuanLyTheoLoHang: itFor.QuanLyTheoLoHang
                            }
                            arrCTNew.push(obj);
                        }
                        else {
                            // find & sum by lo
                            for (let j = 0; j < arrCTNew.length; j++) {
                                let forNew = arrCTNew[j];
                                if (forNew.ID_DonViQuiDoi === itFor.ID_DonViQuiDoi && forNew.ID_LoHang === itFor.ID_LoHang) {
                                    arrCTNew[j].SoLuong += formatNumberToFloat(itFor.SoLuong);
                                    break;
                                }
                            }
                        }
                    }
                    else {
                        let obj = {
                            TenHangHoa: itFor.TenHangHoa,
                            MaHangHoa: itFor.MaHangHoa,
                            MaLoHang: itFor.MaLoHang,
                            ID_DonViQuiDoi: itFor.ID_DonViQuiDoi,
                            ID_LoHang: itFor.ID_LoHang,
                            SoLuong: formatNumberToFloat(itFor.SoLuong),
                            ID_LoHang: null,
                            QuanLyTheoLoHang: itFor.QuanLyTheoLoHang
                        }
                        arrCTNew.push(obj);
                    }
                }
                else {
                    if (itFor.QuanLyTheoLoHang) {
                        if ($.inArray(itFor.ID_LoHang, group_arrIDLoHang) === -1) {
                            group_arrIDLoHang.push(itFor.ID_LoHang);
                            let obj = {
                                TenHangHoa: itFor.TenHangHoa,
                                MaHangHoa: itFor.MaHangHoa,
                                MaLoHang: itFor.MaLoHang,
                                ID_DonViQuiDoi: itFor.ID_DonViQuiDoi,
                                ID_LoHang: itFor.ID_LoHang,
                                SoLuong: formatNumberToFloat(itFor.SoLuong),
                                QuanLyTheoLoHang: itFor.QuanLyTheoLoHang
                            }
                            arrCTNew.push(obj);
                        }
                        else {
                            // find & sum by lo
                            for (let j = 0; j < arrCTNew.length; j++) {
                                let forNew = arrCTNew[j];
                                if (forNew.ID_DonViQuiDoi === itFor.ID_DonViQuiDoi && forNew.ID_LoHang === itFor.ID_LoHang) {
                                    arrCTNew[j].SoLuong += formatNumberToFloat(itFor.SoLuong);
                                    break;
                                }
                            }
                        }
                    }
                    else {
                        // find & sum by lo
                        for (let j = 0; j < arrCTNew.length; j++) {
                            let forNew = arrCTNew[j];
                            if (forNew.ID_DonViQuiDoi === itFor.ID_DonViQuiDoi) {
                                arrCTNew[j].SoLuong += formatNumberToFloat(itFor.SoLuong);
                                break;
                            }
                        }
                    }
                }
            }
        }
        return arrCTNew;
    }

function GetDate_FromTo(textValue) {
    var now = new Date();
    textValue = textValue.toLowerCase();
    var dayStart = '', dayEnd = '';

    switch (textValue) {
        case 'toanthoigian':
            dayStart = '2016-01-01';
            dayEnd = moment(now).format('YYYY-MM-DD');
            break;
        case 'homnay':
            dayStart = moment(now).format('YYYY-MM-DD');
            dayEnd = dayStart;
            break;
        case 'homqua':
            dayStart = moment(now).add('days', -1).format('YYYY-MM-DD');
            dayEnd = dayStart;
            break;
        case 'tuannay':
            dayStart = moment().startOf('week').add('days', 1).format('YYYY-MM-DD');
            dayEnd = moment().endOf('week').add('days', 1).format('YYYY-MM-DD');
            break;
        case 'tuantruoc':
            dayStart = moment().startOf('week').subtract('days', 6).format('YYYY-MM-DD');
            dayEnd = moment().startOf('week').format('YYYY-MM-DD');
            break;
        case 'thangnay':
            dayStart = moment().startOf('month').format('YYYY-MM-DD');
            dayEnd = moment().endOf('month').format('YYYY-MM-DD');
            break;
        case 'thangtruoc':
            dayStart = moment().subtract('months', 1).startOf('month').format('YYYY-MM-DD');
            dayEnd = moment().subtract('months', 1).endOf('month').format('YYYY-MM-DD');
            break;
        case 'quynay':
            dayStart = moment().startOf('quarter').format('YYYY-MM-DD');
            dayEnd = moment().endOf('quarter').format('YYYY-MM-DD');
            break;
        case 'quytruoc':
            var prevQuarter = moment().quarter() - 1;
            if (prevQuarter === 0) {
                // get quy 4 cua nam truoc 01/10/... -->  31/21/...
                let prevYear = moment().year() - 1;
                dayStart = prevYear + '-' + '10-01';
                dayEnd = moment().year() + '-' + '01-01';
                dayEnd = moment(dayEnd).add('days', -1).format('YYYY-MM-DD');
            }
            else {
                dayStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                dayEnd = moment().quarter(prevQuarter).endOf('quarter').format('YYYY-MM-DD');
            }
            break;
        case 'namnay':
            dayStart = moment().startOf('year').format('YYYY-MM-DD');
            dayEnd = moment().endOf('year').format('YYYY-MM-DD');
            break;
        case 'namtruoc':
            var prevYear = moment().year() - 1;
            dayStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
            dayEnd = moment().year(prevYear).endOf('year').format('YYYY-MM-DD');
            break;
        case '7ngayqua':
            dayStart = moment(now).add('days', - 6).format('YYYY-MM-DD');
            dayEnd = moment(now).format('YYYY-MM-DD');
            break;
        case '30ngayqua':
            dayStart = moment(now).add('days', - 30).format('YYYY-MM-DD');
            dayEnd = moment(now).format('YYYY-MM-DD');
            break;
    }
    return {
        FromDate: dayStart,
        ToDate: dayEnd
    };
}

function ResetDropdown(ddlChosed, ddlList, text) {
    $(ddlChosed).find('input').remove();
    $(ddlChosed).append('<input type=\"text\" class=\"dropdown form-control\" placeholder=\"' + text + '\" >');
    $(ddlList).find('i').remove();
}

function RemoveCheckAfter_byID(elm, id) {
    $('#' + elm + ' li').each(function () {
        if ($(this).attr('id') === id) {
            $(this).find('i').remove();
            return false;
        }
    });
}

function SetCheckAfter_inArrID(elm, arrID) {
    $('#' + elm + ' li').each(function () {
        if ($.inArray($(this).attr('id'), arrID) > -1) {
            $(this).append(elmCheck);
        }
    });
}
var elmCheck = '<i class="fa fa-check check-after-li"></i>';// used KhachHang.js

// after chose item in dropdown: add this tag <i>
var element_appendCheck = '<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>';
var const_GuidEmpty = '00000000-0000-0000-0000-000000000000';

var FileModel = function (filef, srcf) {
    var self = this;
    this.file = filef;
    this.URLAnh = srcf;
};

// chi lay xau khong phai la khoang trang
function RemoveItemEmpty(arr) {
    arr = $.grep(arr, function (x) {
        return x !== '' && /\S/.test(x) === true;
    });
    return arr;
}

function CheckValidate_DDMMYYYY(dateStr) {
    const regExp = /^(\d\d?)\/(\d\d?)\/(\d{4})$/;
    let matches = dateStr.match(regExp);

    if (matches !== null && matches.length > 0) {
        let maxDate = [31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

        const month = parseInt(matches[1]);
        const date = parseInt(matches[2]);
        const year = parseInt(matches[3]);

        if (month < 1 || month > 12 || year < 1900) {
            return false;
        }
        if (date < 0 || date > maxDate[month]) {
            return false;
        }
        let leapYear = false;
        if (year % 400 == 0 || (year % 4 == 0 && year % 100 != 0)) {
            leapYear = true;
        }
        if (!leapYear && date > 28) {
            return false;
        }
        return true;
    }
    else {
        return false;
    }
}

function getCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
};

function setCookie(cname, cvalue, exdays) {
    const d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    let expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
};


function getDeviceId() {
    let deviceId = getCookie("deviceId");
    if (deviceId === "") {
        deviceId = uuidv4();
    }
    setCookie("deviceId", deviceId, 7);
    return deviceId;
};

$(window.document).on('shown.bs.modal', '.modal', function () {
    window.setTimeout(function () {
        $('[autofocus]', this).focus();
        $('[autofocus]').select();
    }.bind(this), 100);
});