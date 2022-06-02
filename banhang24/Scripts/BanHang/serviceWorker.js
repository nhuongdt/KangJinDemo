var CACHE_VERSION = 1;
//var subDomain = $('#subDomain').val();
//var db = new Dexie(_subDomain);

var CURRENT_CACHES = {
    nhahang: 'nhahang-v' + CACHE_VERSION,
    banle: 'banle-v' + CACHE_VERSION,
};
//var CACHE_NAME = 'banle-v' + CACHE_VERSION;
var urlsBanLeToCache = [
    '/Content/partial.css',
    '/Content/Banhang.css',
    '/Content/bootstrap.css',
    '/Content/ssoftvn.css',
    '/Content/font-awesome.css',
    '/Content/style.css',
    '/Content/responsive.css',
    '/Content/VariablesStyle.css',
    '/Content/printJS/print.min.css',
    '/Content//bootstrap-datepicker.css',
    '/Content/js/Datetime/jquery.datetimepicker.css',
    '/Content/base/jquery-ui.css',
    '/Content/darkmode.css',

    '/bundles/JqueryBootstrapKnockout',
    '/bundles/DateTimePicker',
    '/Content/css',
 
    '/bundles/BanLe',
    '/$/BanLe',

    '/Content/Treeview/gijgo.js',
    '/Scripts/jquery.cookie.js',
    '/Scripts/ThietLap/MauInTeamplate.js',
    '/Scripts/knockout-jqAutocomplete.min.js' ,

    '/Content/images/banhang24.png',
    '/Content/images/iconbepp18.9/timkiem.png',
    '/Content/images/bill-icon/Giaodien24hh-26.png',
    '/Content/images/iconbepp18.9/dongbodulieu.png',
    '/Content/images/iconbepp18.9/tuychon.png',
    '/Content/images/bill-icon/Giaodien24hh-29.png',
    '/Content/images/bill-icon/Giaodien24hh-30.png',
    '/Content/images/bill-icon/Giaodien24hh-31.png',
    '/Content/images/bill-icon/Giaodien24hh-33.png',
    '/Content/images/bill-icon/Giaodien24hh-32.png',
    '/Content/images/bill-icon/Giaodien24hh-34.png',
    '/Content/images/bill-icon/up1.png',
    '/Content/images/open24.vn.png',
    '/Content/images/bill-icon/Giaodien24hh-24.png',
    '/Content/images/iconketchen/m4.png',
    '/Content/images/icon/ngaysinh.png',
    '/Content/images/icon/Iconthemmoi-16.png',
    '/Content/images/iconbepp18.9/innhap.png',
    '/Content/images/iconbepp18.9/guiggmail.png',
    '/Content/images/print/icccoo-11.png',
    '/Content/images/iconbepp18.9/xtat.png',
    '/Content/images/icon/Iconthemmoi-14.png',
    '/Content/images/iconbepp18.9/gg-37.png',
    '/Content/images/wait.gif',
    '/Content/images/anhhh/tracuutonkhooo-49.png',
    '/Content/images/icon/right2.png',
    '/Content/images/print/nhin.png',
    '/Content/images/khuyenmai.png ',
    '/Content/images/giamgia.png',
    '/Content/images/photo.png',
    '/Content/images/icon/Iconthemmoi-17.png',
    '/imageLogo/0973474985/Logo.jpg',
    '/Content/images/anhhh/print.png',
    '/Content/images/anhhh/logo24.png ',
    '/Content/images/anhhh/place.png ',
    '/Content/images/icon/gioi-tinh-nam.png ',
    '/Content/images/icon/gioi-tinh-nu.png',
    '/Content/images/icon/services.png',
    '/Content/images/icon/discount.png',
    '/Content/images/icon/nhanvien-thuchien.png',
    '/Content/images/icon/nhanvien-tu-van.png',
    '/Content/images/icon/user-discount-1.png',
    '/Content/images/icon/user-discount.png',
    '/Content/images/icon/icon-folder.png',
    '/Content/images/icon/nhat-ky-dich-vu.png',
    '/Content/images/nhahang/vi-tri.png',
    '/Content/images/logo-open24-min.png ',
    '/Content/images/print-min.png ',
    '/Content/images/coner.png ',
    '/Content/images/nhin-min.png ',
    '/Content/images/hotro/iconhotro-10.png',
    '/Content/images/hotro/iconhotro-11.png',
    '/Content/images/hotro/iconhotro-12.png',
    '/Content/images/conner-white.png'
];

var urlsNhaHangToCache = [
    '/Content/partial.css',
    '/Content/Banhang.css',
    '/Content/bootstrap.css',
    '/Content/ssoftvn.css',
    '/Content/font-awesome.css',
    '/Content/style.css',
    '/Content/responsive.css',
    '/Content/VariablesStyle.css',
    '/Content/printJS/print.min.css',
    '/Content//bootstrap-datepicker.css',
    '/Content/js/Datetime/jquery.datetimepicker.css',
    '/Content/base/jquery-ui.css',
    '/Content/darkmode.css',

    '/bundles/JqueryBootstrapKnockout',
    '/bundles/DateTimePicker',
    '/Content/css',

    '/bundles/BanLe',
    '/$/BanLe',

    '/Content/Treeview/gijgo.js',
    '/Scripts/jquery.cookie.js',
    '/Scripts/ThietLap/MauInTeamplate.js',
    '/Scripts/knockout-jqAutocomplete.min.js',

    '/Content/images/banhang24.png',
    '/Content/images/iconbepp18.9/timkiem.png',
    '/Content/images/bill-icon/Giaodien24hh-26.png',
    '/Content/images/iconbepp18.9/dongbodulieu.png',
    '/Content/images/iconbepp18.9/tuychon.png',
    '/Content/images/bill-icon/Giaodien24hh-29.png',
    '/Content/images/bill-icon/Giaodien24hh-30.png',
    '/Content/images/bill-icon/Giaodien24hh-31.png',
    '/Content/images/bill-icon/Giaodien24hh-33.png',
    '/Content/images/bill-icon/Giaodien24hh-32.png',
    '/Content/images/bill-icon/Giaodien24hh-34.png',
    '/Content/images/bill-icon/up1.png',
    '/Content/images/open24.vn.png',
    '/Content/images/bill-icon/Giaodien24hh-24.png',
    '/Content/images/iconketchen/m4.png',
    '/Content/images/icon/ngaysinh.png',
    '/Content/images/icon/Iconthemmoi-16.png',
    '/Content/images/iconbepp18.9/innhap.png',
    '/Content/images/iconbepp18.9/guiggmail.png',
    '/Content/images/print/icccoo-11.png',
    '/Content/images/iconbepp18.9/xtat.png',
    '/Content/images/icon/Iconthemmoi-14.png',
    '/Content/images/iconbepp18.9/gg-37.png',
    '/Content/images/wait.gif',
    '/Content/images/anhhh/tracuutonkhooo-49.png',
    '/Content/images/icon/right2.png',
    '/Content/images/print/nhin.png',
    '/Content/images/khuyenmai.png ',
    '/Content/images/giamgia.png',
    '/Content/images/photo.png',
    '/Content/images/icon/Iconthemmoi-17.png',
    '/imageLogo/0973474985/Logo.jpg',
    '/Content/images/anhhh/print.png',
    '/Content/images/anhhh/logo24.png ',
    '/Content/images/anhhh/place.png ',
    '/Content/images/icon/gioi-tinh-nam.png ',
    '/Content/images/icon/gioi-tinh-nu.png',
    '/Content/images/icon/services.png',
    '/Content/images/icon/discount.png',
    '/Content/images/icon/nhanvien-thuchien.png',
    '/Content/images/icon/nhanvien-tu-van.png',
    '/Content/images/icon/user-discount-1.png',
    '/Content/images/icon/user-discount.png',
    '/Content/images/icon/icon-folder.png',
    '/Content/images/icon/nhat-ky-dich-vu.png',
    '/Content/images/nhahang/vi-tri.png',
    '/Content/images/logo-open24-min.png ',
    '/Content/images/print-min.png ',
    '/Content/images/coner.png ',
    '/Content/images/nhin-min.png ',
    '/Content/images/hotro/iconhotro-10.png',
    '/Content/images/hotro/iconhotro-11.png',
    '/Content/images/hotro/iconhotro-12.png',
    '/Content/images/conner-white.png'
];

importScripts('/Scripts/BanHang/cache-polyfill.js');

self.addEventListener('install', function (event) {
    // Perform install steps
    event.waitUntil(
        caches.open(CURRENT_CACHES.banle)
            .then(function (cache) {
                console.log('Opened cache');
                return cache.addAll(urlsBanLeToCache);
            })
    );
});

//self.addEventListener('install', function (event) {
//    // Perform install steps
//    event.waitUntil(
//        caches.open('banle-v5')
//            .then(function (cache) {
//                console.log('Opened cache');
//                return cache.addAll([
//                    '/Content/Treeview/gijgo.js',
//                    '/Scripts/jquery.cookie.js',
//                    '/Scripts/ThietLap/MauInTeamplate.js',
//                    '/Scripts/knockout-jqAutocomplete.min.js',
//                ]);
//            })
//    ); 
//});

// get/set data from cache
self.addEventListener('fetch', function (event) {
    event.respondWith(
        // caches.match: bộ nhớ đệm
        caches.match(event.request)
            .then(function (response) {
                console.log('response ', response)
                // Cache hit - return response
                if (response) {
                    return response;
                }
               // if not found --> get from network
                return fetch(event.request).then(function (response) {
                    console.log('response ', response)
                    // Check if we received a valid response
                    if (!response || response.status !== 200 || response.type !== 'basic') {
                        return response;
                    }
                    // copy and set to cache & brower
                    var responseToCache = response.clone();
                    caches.open(CURRENT_CACHES.banle)
                        .then(function (cache) {
                            cache.put(event.request, responseToCache);
                        });
                    return response;
                });
            })
    );
});

// update cache
self.addEventListener('activate', function (event) {
    console.log('Activating…');
    var cacheAllowlist = [CURRENT_CACHES.banle];
    event.waitUntil(
        caches.keys().then(function (cacheNames) {
            return Promise.all(
                cacheNames.map(function (cacheName) {
                    console.log('cacheName …', cacheName);
                    //CreateDB_IndexDB();
                    if (cacheAllowlist.indexOf(cacheName) === -1) {
                        return caches.delete(cacheName);
                    }
                })
            );
        })
    );
});

function CreateDB_IndexDB() {
    //var db = new Dexie(_subDomain);
    db.version(1).stores({
        DM_DoiTuong: 'ID, Name_Phone',
        DM_HangHoa: 'ID, Name',
        NS_NhanVien: '++id',
        DM_GiaBan: 'ID',
        HDDatHang_UpdateStatus: 'ID, Status',
        HDDatHang_Offline: 'ID',
        HeThongTichDiem: 'ID_TichDiem',
        DM_MauIn: 'ID',
        Quyen_NguoiDung: '++id',
        HT_NguoiDung: 'ID',
        DM_QuanHuyen: 'ID',
        DM_LoHang: '++id',
        DM_DonVi: 'ID',
        KhachHang_HangHoa: 'ID_DoiTuong',
        ChietKhau_NhanVien: '++id',
        DM_KhuyenMai: 'ID',
        DM_TaiKhoanNganHang: 'ID',
        DM_NhanVienLienQuan: '++id',
        DM_ViTri: 'ID',
        DM_KhuVuc: 'ID',
        DM_DoiTuong_TrangThai: 'ID',
        DinhLuong_DichVu: 'ID',
    });
}
