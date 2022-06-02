var Open24FileManager = (function () {
    /*var hostUrl = "https://localhost:7015/";*/
    var hostUrl = "https://drive.open24.vn/";
    /*FileType: 1. Picture, 2. Document, 3. Other
     *Function: 1. Khách hàng, 2. Hàng hóa, 3. Hóa đơn, 4. Xe, 5. Phiếu tiếp nhận, 6. Người dùng (User),
     *          7. Công ty (logo), 8. Nhân viên, 9. Quảng cáo, 10. Khác
     * Khai báo obj upload
        let myData = {};
        let files = new FormData();
        for(let i = 0; i < Files.length; i++)
        {
            files.append("files", Files[i]);
        }
        myData.Subdomain = "0973474985";
        myData.Function = "1";
        myData.Id = "3FA7627F-CCB4-4ECC-BED8-C30326120003";
        myData.files = files;
        var result = Open24FileManager.UploadImage(myData);
       
        result là mảng string url file sau khi upload thành công
     */
    
    var UploadImage = function (obj) {
        var result = [];
        $.ajax({
            url: hostUrl + "api/File/Upload?Subdomain=" + obj.Subdomain
                + "&FileType=1&Function=" + obj.Function + "&Id=" + obj.Id ,
            type: "POST",
            data: obj.files,
            processData: false,
            contentType: false,
            async: false,
            success: function (data, textStatus, jqXHR) {
                result = data;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                /*return [];*/
            }
        });
        return result;
    };
    var UploadImageT = function (obj) {
        var result = [];
        $.ajax({
            url: hostUrl + "api/File/UploadT?Subdomain=" + obj.Subdomain
                + "&FileType=1&Function=" + obj.Function + "&Id=" + obj.Id,
            type: "POST",
            data: obj.files,
            processData: false,
            contentType: false,
            async: false,
            success: function (data, textStatus, jqXHR) {
                result = data;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                /*return [];*/
            }
        });
        return result;
    };
    var UploadImageM = function (obj) {
        var result = [];
        $.ajax({
            url: hostUrl + "api/File/UploadM?Subdomain=" + obj.Subdomain
                + "&FileType=1&Function=" + obj.Function + "&Id=" + obj.Id,
            type: "POST",
            data: JSON.stringify(obj.files),
            contentType: "application/json",
            dataType: "JSON",
            async: false,
            success: function (data, textStatus, jqXHR) {
                result = data;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                /*return [];*/
            }
        });
        return result;
    };

    var UploadDocument = function (obj) {
        var result = [];
        $.ajax({
            url: hostUrl + "api/File/Upload?Subdomain=" + obj.Subdomain
                + "&FileType=2&Function=" + obj.Function + "&Id=" + obj.Id,
            type: "POST",
            data: obj.files,
            processData: false,
            contentType: false,
            async: false,
            success: function (data, textStatus, jqXHR) {
                result = data;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                /*return [];*/
            }
        });
        return result;
    };
    /**
     * files là mảng string
     */
    var RemoveFiles = function (files) {
        $.ajax({
            url: hostUrl + "api/File/Delete",
            type: "POST",
            data: JSON.stringify(files),
            contentType: "application/json",
            dataType: "JSON",
            success: function (data, textStatus, jqXHR) {
            },
            error: function (jqXHR, textStatus, errorThrown) {
                
            }
        });
    }

    var DeleteDirectory = function (obj) {
        $.ajax({
            url: hostUrl + "api/File/DeleteDirectory?Subdomain=" + obj.Subdomain
                + "&FileType=1&Function=" + obj.Function + "&name=" + obj.Id,
            type: "GET",
            processData: false,
            contentType: false,
            async: false,
            success: function (data, textStatus, jqXHR) {
                result = data;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                /*return [];*/
            }
        });
    }

    return {
        UploadImage: UploadImage,
        UploadImageT: UploadImageT,
        UploadImageM: UploadImageM,
        hostUrl: hostUrl,
        RemoveFiles: RemoveFiles,
        DeleteDirectory: DeleteDirectory,
        UploadDocument: UploadDocument
    }
})();