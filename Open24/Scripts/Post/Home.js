var View = function () {
    var self = this;

    self.listNewsHome = ko.observableArray();

    self.pageSizesHome = [6];
    self.pageSizeHome = ko.observable(self.pageSizesHome[0]);
    self.currentPage = ko.observable(0);
    //đăng ký sử dụng
    self.fullname_dksd = ko.observable();
    self.phone_dksd = ko.observable();
    self.optLineOfBusiness_dksd = ko.observableArray();
    self.companyName_dksd = ko.observable();
    self.storeAddress_dksd = ko.observable();
    self.userName_dksd = ko.observable();
    self.password_dksd = ko.observable();
    self.repassword_dksd = ko.observable();
    self.webAddress_dksd = ko.observable();
    self.email_dksd = ko.observable();

    //Form đăng ký------------------------------------------------------------------------------------------------
    function getNganhNgheKinhDoanh() {
        ajaxHelper('/Open24Api/PostAPI/' + "GetNganhNgheKinhDoanh", 'GET').done(function (data) {
            var a = JSON.stringify(data);
            self.optLineOfBusiness_dksd(JSON.parse(a));
        });
    }
    getNganhNgheKinhDoanh();
    
    self.dangkysudung = function () {
        var fullName = self.fullname_dksd();
        var phone = self.phone_dksd();
        var lineOfBusiness = $("#selLineOfBusiness_dksd").val();
        var storeName = self.companyName_dksd();
        var storeAddress = self.storeAddress_dksd();
        var userName = self.userName_dksd();
        var password = self.password_dksd(); 
        var repassword = self.repassword_dksd();
        var webAddress = self.webAddress_dksd();
        var email = self.email_dksd();

        var chkPhone = $("#phonedksd").val();
        var chkstorename = $("#storenamedksd").val();
        var chkstoreaddress = $("#storeaddressdksd").val();
        var chkusername = $("#usernamedksd").val();
        var chkpassword = $("#passworddksd").val();
        var chkrepassword = $("#repassworddksd").val();
        var subdomainVal = $("#subdomaindksd").val();
        var phoneDB = "";
        var subdomainDB = "";
        var usernameDB = "";
        if (chkPhone != "" && chkstorename != "" && chkstoreaddress != "" && chkusername != "" && chkpassword != "" && chkrepassword != "") {
            ajaxHelper('/Open24Api/PostAPI/' + "CheckDangKy", 'GET').done(function (data) {
                var a = JSON.stringify(data);
                var b = JSON.parse(a);

                for (var i = 0; i < b.length; i++) {
                    if (b.hasOwnProperty(i)) {
                        phoneDB = b[i].SoDienThoai;
                        subdomainDB = b[i].SubDomain;
                        usernameDB = b[i].TenTaiKhoan;

                        if (chkPhone == phoneDB) {
                            $("#validatePhone").text("Số điện thoại này đã được sử dụng để đăng ký. Hãy nhập số điện thoại khác.");
                            //alert("Địa chỉ WebSite đã được sử dụng. Hãy nhập địa chỉ Web khác.");
                            break;
                        } else {
                            $("#validatePhone").text("");
                            //console.log("1");
                        }
                        if (subdomainVal == subdomainDB) {
                            $("#validateWeb").text("Địa chỉ WebSite đã được sử dụng. Hãy nhập địa chỉ Web khác.");
                            //alert("Địa chỉ WebSite đã được sử dụng. Hãy nhập địa chỉ Web khác.");
                            break;
                        } else {
                            $("#validateWeb").text("");
                        }
                    }
                }
            });

            if (chkpassword != chkrepassword) {
                $("#validatePass").text("Mật khẩu chưa đúng. Nhập lại mật khẩu");
                //alert("Mật khẩu chưa đúng. Nhập lại mật khẩu");
            } else {
                $("#validatePass").text("");
                var model = {
                    SoDienThoai: phone,
                    SubDomain: webAddress,
                    TenCuaHang: storeName,
                    DiaChi: storeAddress,
                    Email: email,
                    HoTen: fullName,
                    ID_NganhKinhDoanh: lineOfBusiness,
                    UserKT: userName,
                    MatKhauKT: password
                };
                $.ajax({
                    data: model,
                    url: '/Open24Api/PostAPI/' + "DangKySuDung",
                    type: 'POST',
                    async: true,
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (item) {
                        alert('success');
                    }
                });
            }
        }
        else {
            alert("Vui lòng nhập đầy đủ thông tin!");
        }



    }
    
}
ko.applyBindings(new View());

function ajaxHelper(uri, method, data) {
    return $.ajax({
        type: method,
        url: uri,
        dataType: 'json',
        contentType: 'application/json',
        data: data ? JSON.stringify(data) : null,
        async: false,
        statusCode: {
            404: function () {
                console.log("Page not found");
            },
        }
    })
        .fail(function (jqXHR, textStatus, errorThrown) {
            console.log(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
        });
}