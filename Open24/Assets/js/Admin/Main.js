//===============================
// Kiểm tra gmail
//===============================
function validateEmail(sEmail) {
    if (sEmail === undefined
        || sEmail===null
        || sEmail.replace(/\s+/g, '') === "") {
        return true;
    }
    var filter = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    if (filter.test(sEmail.replace(/\s+/g, ''))) {
        return true;
    }
    else {
        return false;
    }
}
//===============================
// Kiểm tra Số điện thoại
//===============================
function validatePhone(txtPhone) {
    if (txtPhone === undefined
        || txtPhone === null
        || txtPhone.replace(/\s+/g, '') === "") {
        return true;
    }
    var filter = /^((\+[1-9]{1,4}[ \-]*)|(\([0-9]{2,3}\)[ \-]*)|([0-9]{2,4})[ \-]*)*?[0-9]{3,4}?[ \-]*[0-9]{3,4}?$/;
    if (filter.test(txtPhone.replace(/\s+/g, ''))) { 
        return true;
    }
    else {
        return false;
    }
}
//===============================
// Hiện thị trạng thái
//===============================
function ConvertStatuts(value) {
    if (value === true) {
        return "Hoạt động"
    }
    else if (value === false) {
        return "Không hoạt động"
    }
    else {
        return "";
    }
};
//===============================
// Check ký tự đặc biệt
//===============================
var format = /[!@#$%^&*()+\=\[\]{};':"\\|<>\/]+/;
function validatespecialcharacters(object){
    if (format.test(object)) {
        return true;
    } else {
        return false;
    }
}
//===============================
// Show thông báo khi lỗi call ajax
//===============================
 function exception(result) {
        console.log(result);
        AlertError("Đã xảy ra lỗi.");
}
//===============================
// Show thông báo với popup
//===============================
function Showmessage(result) {
    $('#showmmessage').html(result);
    $('#Modalpopup').modal('show');
    setTimeout(function () {
        $('#Modalpopup').modal('hide')
    }, 10000);
 }

//===============================
// Chuyển kiểu số sang vnd
//===============================
function FormatVND(number) {
    if (number === null || number === undefined || !$.isNumeric(number)) {
        number = 0;
    }
    return number.toLocaleString('it-IT', { style: 'currency', currency: 'VND' });
}

function AlertError(mess) {
    return $.growl.error({
        title: "Lỗi !",
        message: mess
    });
}
function AlertNotice(mess) {
    return $.growl.notice({
        title: "Thông báo !",
        message: mess
    });
}
function Alertwarning(mess) {
    return $.growl.warning({
        title: "Cảnh báo !",
        message: mess
    });
}