
//===============================
// Chuyển kiểu số sang vnd
//===============================
function FormatVND(number) {
    if (number === null || number === undefined || !$.isNumeric(number)) {
        number = 0;
    }
    return number.toLocaleString('it-IT', { style: 'currency', currency: 'VND' });
}
//===============================
// Create indexDB
//===============================
var LocalIndexDB = LocalIndexDB || (function () {
    var indexedDB = window.indexedDB || window.mozIndexedDB || window.webkitIndexedDB || window.msIndexedDB || window.shimIndexedDB;
    // Open (or create) the database
    var OpenIndexDb;
    var CartProduct = 'CartDevices';
    var createDB = function () {
        OpenIndexDb = indexedDB.open("Open24", 1);
        var IndexDatabase;
        OpenIndexDb.onupgradeneeded = function () {

            IndexDatabase = OpenIndexDb.result;
            if (IndexDatabase.objectStoreNames.contains(CartProduct)) {
                IndexDatabase.deleteObjectStore(CartProduct);
            }
            var store = IndexDatabase.createObjectStore(CartProduct, { keyPath: "ID", autoIncrement: true });
            store.createIndex("salesDevice_Id", "salesDevice_Id", { unique: false });
            store.createIndex("salesDevice_Name", "salesDevice_Name", { unique: false });
            store.createIndex("salesDevice_Price", "salesDevice_Price", { unique: false });
            store.createIndex("salesDevice_Quantity", "salesDevice_Quantity", { unique: false });
            store.createIndex("salesDevice_Img", "salesDevice_Img", { unique: false });
            store.createIndex("salesDevice_Money", "salesDevice_Money", { unique: false });
            store.createIndex("salesDevice_Encoder", "salesDevice_Encoder", { unique: false });
            store.createIndex("Client_Id", "Client_Id", { unique: false });
        };
        OpenIndexDb.onsuccess = function () {
            console.log("Cart is Open");
            $("body").trigger("CartOnsuccess");
        };
    }
    var GetConnectIndexDB = function(){
            var db = OpenIndexDb.result;
            var transaction = db.transaction([CartProduct], "readwrite");
            return transaction.objectStore([CartProduct]);
    }
    return {
        Create: createDB,
        Connect: GetConnectIndexDB
    }
})();

//===============================
// Create guid ID
//===============================
    var crypto = window.crypto || window.msCrypto || null; // IE11 fix
    var Guid = Guid || (function () {
        var EMPTY = '00000000-0000-0000-0000-000000000000';
        var _padLeft = function (paddingString, width, replacementChar) {
            return paddingString.length >= width ? paddingString : _padLeft(replacementChar + paddingString, width, replacementChar || ' ');
        };
        var _s4 = function (number) {
            var hexadecimalResult = number.toString(16);
            return _padLeft(hexadecimalResult, 4, '0');
        };
        var _cryptoGuid = function () {
            var buffer = new window.Uint16Array(8);
            window.crypto.getRandomValues(buffer);
            return [_s4(buffer[0]) + _s4(buffer[1]), _s4(buffer[2]), _s4(buffer[3]), _s4(buffer[4]), _s4(buffer[5]) + _s4(buffer[6]) + _s4(buffer[7])].join('-');
        };
        var _guid = function () {
            var currentDateMilliseconds = new Date().getTime();
            return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (currentChar) {
                var randomChar = (currentDateMilliseconds + Math.random() * 16) % 16 | 0;
                currentDateMilliseconds = Math.floor(currentDateMilliseconds / 16);
                return (currentChar === 'x' ? randomChar : (randomChar & 0x7 | 0x8)).toString(16);
            });
        };
        var create = function () {
            var hasCrypto = crypto != 'undefined' && crypto !== null,
                hasRandomValues = typeof (window.crypto.getRandomValues) != 'undefined';
            return (hasCrypto && hasRandomValues) ? _cryptoGuid() : _guid();
        };

        return {
            newGuid: create,
            empty: EMPTY
        };
    })();

    //===============================
    // set and get cookie
    //===============================
    var LocalCookies = LocalCookies || (function () {
        var writeCookie = function (key, value, days) {
            var date = new Date();

            // Default at 365 days.
            days = days || 365;

            // Get unix milliseconds at current time plus number of days
            date.setTime(+ date + (days * 86400000)); //24 * 60 * 60 * 1000

            window.document.cookie = key + "=" + value + "; expires=" + date.toGMTString() + "; path=/";

            return value;
        };
        var ReadCookie = function (key) {
            cookieList = document.cookie.split('; ');
            cookies = {};
            for (i = cookieList.length - 1; i >= 0; i--) {
                cookie = cookieList[i].split('=');
                cookies[cookie[0]] = cookie[1];
            }
            return cookies[key];
        };
        return {
            Set: writeCookie,
            Get: ReadCookie
        }
    })();

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
        var validateNull = function (input) {
            return (input === null
                || input === undefined
                || input.replace(/\s+/g, '') === "");
        };
        var convertDate = function (input) {
            if (!validateNull(input)) {
                return moment(input).format('DD/MM/YYYY');
            }
            return "";
        };
        var convertDateServer = function (input) {
            if (!validateNull(input)) {
                return moment(input).format('MM/DD/YYYY');
            }
            return "";
        };
        var SubStringContent = function (input, lenth=150) {

            if (!validateNull(input) && input.length > lenth) {
                return input.substring(0, lenth) + "...";
                
            }
            return input;
        };
        var ConvertSize = function (input) {
            if (input != null)
            {
                if (input > 1001) {
                    return precise_round(input / 1000, 2) + " KB";
                }
                else if (input > 1000000) {
                    return precise_round(input / 1000000, 2) + " MB";
                }
                else {

                    return input + " B";
                }
            }
            return "0";
        };
        var precise_round = function (num, decimals) {
            var t = Math.pow(10, decimals);
            return (Math.round((num * t) + (decimals > 0 ? 1 : 0) * (Math.sign(num) * (10 / Math.pow(100, decimals)))) / t).toFixed(decimals);
        };
        var ConvertTitleDateTime = function (config) {
            var date = new Date(config);
            var weekday = new Array(7);
            weekday[0] = "Chủ nhật";
            weekday[1] = "Thứ hai";
            weekday[2] = "Thứ ba";
            weekday[3] = "Thứ tư";
            weekday[4] = "Thứ năm";
            weekday[5] = "Thứ sáu";
            weekday[6] = "Thứ bảy";
            var n = weekday[date.getDay()];
            var h = date.getHours(), m = date.getMinutes();
            var _time = (h > 12) ? (h - 12 + ':' + m + ' PM') : (h + ':' + m + ' AM');
            var date = n + ", " + date.getDate() + "/" + (date.getMonth() + 1) + "/" + date.getFullYear() + " | " + _time;
            return date
        };
        var sleep = function (time) {
            return new Promise((resolve) => setTimeout(resolve, time));
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
            var url = Convetvchar(obj);
            url = url.trim().replace(/ /g, '-');
            return url;
        };
        return {
            CheckPhoneNumber: validatePhone,
            CheckEmail: validateEmail,
            CheckNull: validateNull,
            convertStrFormC: convertFormC,
            convertDate: convertDate,
            ConvertTitleDateTime: ConvertTitleDateTime,
            sleep: sleep,
            SubStringContent: SubStringContent,
            convertDateServer: convertDateServer,
            ConvertSize: ConvertSize,
            Convetvchar: Convetvchar,
            ConvertUrl: ConvertUrl
        }
    })();