
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
LocalIndexDB.Create();


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

    

