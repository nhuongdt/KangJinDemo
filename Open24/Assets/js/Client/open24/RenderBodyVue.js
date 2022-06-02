var vmbody = new Vue({
    el: '#renderBody',
    data: {
        ListNews: [],
        ListCustomer: [],
        IsShowCustomer: true,
        CuahangDangKy: {
            FullName: '',
            Phone: '',
            Software: '',
            Note: '',
            Address: ''
        },
        KhachHangGioiThieu: {
            StoreOpen: '',
            Phone: '',
            Name: '',
            Note: '',
        },
        reSrc: 'https://www.youtube.com/embed/_EZwS3JZIfo?rel=0&controls=0&autoplay=1',
        showModal: false
    },
    methods: {
        GetCustomerHome: function () {
            var self = this;
            $.ajax({
                type: 'GET',
                url: "/Open24Api/ApiCustomer/GetHome",
                success: function (data) {

                    if (data.res === true) {
                        self.ListCustomer = data.DataSoure;

                    }
                    else {
                        console.log(data.mess);
                    }

                }
            });
        },
        GetNewsHome: function () {
            var self = this;
            $.ajax({
                type: 'GET',
                url: "/Open24Api/ApiPost/GetHome",
                success: function (data) {
                    if (data.res === true) {
                        self.ListNews = data.DataSoure;

                    }
                    else {
                        console.log(data.mess);
                    }
                }
            });
        },
        Clickdangky: function () {
            var self = this;
            console.log('click dăng ký renderbody');
            if (localValidate.CheckNull(self.CuahangDangKy.FullName)) {
                alert("Vui lòng nhập họ tên");
            }
            else if (localValidate.CheckNull(self.CuahangDangKy.Phone)) {
                alert("Vui lòng nhập số điện thoại");
            }
            else if (localValidate.CheckNull(self.CuahangDangKy.Software)) {
                alert("Vui lòng chọn ngành nghề bạn kinh doanh");
            }
            else {
                $.ajax({
                    type: 'GET',
                    url: "https://geoip-db.com/json/",
                    success: function (data) {
                        self.CallInsert(JSON.parse(data));
                    },
                    timeout: 3000,
                    error: function (qXHR, textStatus, errorThrown) {
                        self.CallInsert(null);
                        if (textStatus === "timeout") {
                            console.log(qXHR);
                        }
                    }
                });
            }
        },
        CallInsert: function (ipAdress) {
            var self = this;
            var model = {
                FullName: localValidate.convertStrFormC(self.CuahangDangKy.FullName),
                Phone: localValidate.convertStrFormC(self.CuahangDangKy.Phone),
                Note: localValidate.convertStrFormC(self.CuahangDangKy.Note),
                Software: localValidate.convertStrFormC(self.CuahangDangKy.Software),
                Address: localValidate.convertStrFormC(self.CuahangDangKy.Address),
            };
            var diachi = "";
            var ip4 = "";
            if (ipAdress !== null && ipAdress !== undefined) {
                diachi = (ipAdress.city !== '' && ipAdress.city !== null) ? ipAdress.city + "-" + ipAdress.country_name : ipAdress.country_name;
                ip4 = ipAdress.IPv4;
            }
            $.ajax({
                data: model,
                url: '/Open24Api/ApiHome/OrderedOpen24SoftWare?ip4=' + ip4 + '&ipAdress=' + diachi.trim(),
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (item) {
                    if (item.res === true) {
                        window.location.href = "/dang-ky-dung-thu-thanh-cong";
                        self.CuahangDangKy.FullName = '';
                        self.CuahangDangKy.Phone = '';
                        self.CuahangDangKy.Note = '';
                        self.CuahangDangKy.Address = "";
                    }
                    else {
                        alert(item.mess);
                    }
                }
            });
        },
        changenganhnghe: function (value) {
            if (!localValidate.CheckNull(value)) {
                this.CuahangDangKy.Software = value;
                $('#linhvuckd').val(value);
            }
        },
        changeVideo: function () {
            var self = this;
            $('#myVideo').each(function () {
                var frame = document.getElementById("myVideo");
                frame.contentWindow.postMessage('{"event":"command","func":"playVideo","args":""}', '*');
            });
            $('#myVideo').attr("src", self.reSrc);
            $("#myModal").modal("show");
        },
        ClickGioiThieu: function () {
            var self = this;
            if (localValidate.CheckNull(self.KhachHangGioiThieu.StoreOpen)) {
                alert("Vui lòng nhập địa chỉ gian hàng của bạn");
            }
            else if (localValidate.CheckNull(self.KhachHangGioiThieu.Name)) {
                alert("Vui lòng nhập tên người bạn giới thiệu");
            }
            else if (localValidate.CheckNull(self.KhachHangGioiThieu.Phone)) {
                alert("Vui lòng nhập số điện thoại người bạn giới thiệu");
            }
            else if (!localValidate.CheckPhoneNumber(self.KhachHangGioiThieu.Phone)) {
                alert("Số điện thoại không hợp lệ");
            }
            else {
                var model = self.KhachHangGioiThieu;
                $.ajax({
                    url: "/Open24Api/ApiStoreRegistration/SaveContract",
                    type: 'POST',
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    data: model,
                    success: function (result) {
                        if (result.res === true) {
                            self.showModal = true;
                            self.KhachHangGioiThieu = {
                                StoreOpen: '',
                                Phone: '',
                                Name: '',
                                Note: '',
                            };
                        } else {
                            alert(result.mess);
                        }
                    },
                    error: function (result) {
                        console.log(result);
                    }
                });
            }
        }
    },
    created: function () {
        let self = this;
        self.GetCustomerHome();
        self.GetNewsHome();
    }
});