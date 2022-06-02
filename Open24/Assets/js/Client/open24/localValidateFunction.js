var localValidate = localValidate || (function () {
    var validatePhone = function (phone) {
        if (!validateNull(phone)) {
            var fld = phone.trim();
            var phoneno = /^\(?([0]{1}[1-9]{1}[0-9]{2})\)?[]?([0-9]{3})[]?([0-9]{3})$/;
            var phoneno1 = /^\(?[0]{1}[1-9]{1}[0-9]{3}\)?[]?([0-9]{3})[]?([0-9]{3})$/;
            var phoneno3 = /^\(?[0]{1}[1-9]{1}[0-9]{4}\)?[]?([0-9]{3})[]?([0-9]{3})$/;
            var phoneno4 = /^\(?[0]{1}[1-9]{1}[0-9]{5}\)?[]?([0-9]{4})[]?([0-9]{4})$/;
            var phoneno2 = /^\(?[0]{1}[1-9]{1}[0-9]{6}\)?[]?([0-9]{4})[]?([0-9]{4})$/;
            var allow = allow1 = allow2 = false;

            allow = fld.match(phoneno2);
            allow1 = fld.match(phoneno3);
            allow2 = fld.match(phoneno4);

            if (fld.match(phoneno) || fld.match(phoneno1) || allow || allow1 || allow2) {

                return true;
            }
        }
        return false;
    };
    var convertFormC = function (str) {
        if (!validateNull(str)) {
            return str.normalize('NFC');
        }
        return '';
    };
    var convertDate = function (input) {
        if (!validateNull(input)) {
            return moment(input).format('DD/MM/YYYY');
        }
        return "";
    };
    var Convetvchar = function (obj) {
        if (!obj)
            return "";
        var str = obj;
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
    };
    var ConvertUrl = function (obj) {
        var url = Convetvchar(obj).replace('.', '-');
        url = url.trim().replace(/ /g, '-');
        var splitarray = url.split('-').filter(o => o !== '');
        return splitarray.join('-');
    };
    var converToTimeview = function (config) {
        if (!validateNull(config)) {
            var dt = new Date(config);
            var y = dt.getFullYear();
            var mo = dt.getMonth() + 1;
            var d = dt.getDate();
            var h = dt.getHours();
            var mi = dt.getMinutes();

            var weekday = new Array(7);
            weekday[0] = "Chủ nhật";
            weekday[1] = "Thứ hai";
            weekday[2] = "Thứ ba";
            weekday[3] = "Thứ tư";
            weekday[4] = "Thứ năm";
            weekday[5] = "Thứ sáu";
            weekday[6] = "Thứ bảy";
            var n = weekday[dt.getDay()];

            var date = n + ", " + d + "/" + mo + "/" + y + " | " + h + ":" + mi + " GMT+7";
            return date
        }
        return "";
    };
    var validateEmail = function (input) {
        if (!validateNull(input)) {
            var email = input.trim();
            var res = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            if (res.test(email)) {
                return true;
            }
        }
        return false;
    };
    var SubStringContent = function (input, lenth = 150) {

        if (!validateNull(input) && input.length > lenth) {
            return input.substring(0, lenth) + "...";

        }
        return input;
    };
    var validateNull = function (input) {
        return (input === null
            || input === undefined
            || input.replace(/\s+/g, '') === "");
    }
    return {
        CheckPhoneNumber: validatePhone,
        CheckEmail: validateEmail,
        CheckNull: validateNull,
        convertStrFormC: convertFormC,
        SubStringContent: SubStringContent,
        convertDate: convertDate,
        converToTimeview: converToTimeview,
        Convetvchar: Convetvchar,
        ConvertUrl: ConvertUrl
    }
})();