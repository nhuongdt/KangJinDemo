//===============================
// Create indexDB
//===============================
var LocalIndexDB = LocalIndexDB || (function () {
    var indexedDB = window.indexedDB || window.mozIndexedDB || window.webkitIndexedDB || window.msIndexedDB || window.shimIndexedDB;
    var OpenIndexDb;
    var createDB = function (NameDb,version,listParamester) {
        OpenIndexDb = indexedDB.open(NameDb, version);
        var IndexDatabase;
        OpenIndexDb.onupgradeneeded = function () {
            IndexDatabase = OpenIndexDb.result;
            for (var i = 0; i < listParamester.length; i++)
            {
                if (IndexDatabase.objectStoreNames.contains(listParamester[i].Name)) {
                    IndexDatabase.deleteObjectStore(listParamester[i].Name);
                }
                var store = IndexDatabase.createObjectStore(listParamester[i].Name, { keyPath: "ID", autoIncrement: true });
                var param = listParamester[i].Colum;
                for (var j = 0; j < param.length; j++) {
                    store.createIndex(param[j].key, param[j].name, { unique: false });
                }
            }
        };
        OpenIndexDb.onsuccess = function () {
            console.log("IndexDB is Open");
            $("body").trigger("IndexDBSuccess");
        };
    }
    var GetConnectIndexDB = function (CartProduct) {
        var db = OpenIndexDb.result;
        var transaction = db.transaction([CartProduct], "readwrite");
        return transaction.objectStore([CartProduct]);
    }
    return {
        Create: createDB,
        Connect: GetConnectIndexDB
    }
})();

var LocalStatic = LocalStatic || (function () {

    var stripUnicode = function (slug) {
        slug = slug.toLowerCase();
        slug = slug.replace(/á|à|ả|ạ|ã|ă|ắ|ằ|ẳ|ẵ|ặ|â|ấ|ầ|ẩ|ẫ|ậ/gi, 'a');
        slug = slug.replace(/é|è|ẻ|ẽ|ẹ|ê|ế|ề|ể|ễ|ệ/gi, 'e');
        slug = slug.replace(/i|í|ì|ỉ|ĩ|ị/gi, 'i');
        slug = slug.replace(/ó|ò|ỏ|õ|ọ|ô|ố|ồ|ổ|ỗ|ộ|ơ|ớ|ờ|ở|ỡ|ợ/gi, 'o');
        slug = slug.replace(/ú|ù|ủ|ũ|ụ|ư|ứ|ừ|ử|ữ|ự/gi, 'u');
        slug = slug.replace(/ý|ỳ|ỷ|ỹ|ỵ/gi, 'y');
        slug = slug.replace(/đ/gi, 'd');
        return slug;
    };
    var containsAll = function (needles, haystack) {
        for (var i = 0, len = needles.length; i < len; i++) {
            if (needles[i] === '') continue;
            if (haystack.toLowerCase().search(new RegExp(stripUnicode(needles[i]), "i")) < 0) return false;
        }
        return true;
    };
    return {
        SearchFullText: containsAll
    };
})();