
$(document).ready(function () {
    var currentPage = $("#currentPage").val();
    if (currentPage != null) {
        $("#ID_chosen").val("Open24 " + currentPage);
    } else {
        $("#ID_chosen").val("Open24 " + 'Sale')
    }
    $("#ID_platform a").click(function () {

        var id = $(this).parent().attr("data-id");
        console.log(id)

        switch (id) {
            case "1":
                $("#ID_chosen").val("Open24 Gara")
                break;
            case "2":
                $("#ID_chosen").val("Open24 Beauty")
                break;
            case "3":
                $("#ID_chosen").val("Open24 Sale")
                break;
        }
    })
})

var formDKTV = new Vue({
    el: "#modalTuVan",
    data: {
        FullName: "",
        Email: "",
        Phone: "",
        Type: 1,
        Company: "",
        Address: "",
        Noted: "",
        Software: "",
        SoftwareCODE: "",
    },
    methods: {
        Clickdangky: function () {
            var self = this;
            console.log('click đăng ký modal')
            switch (self.Software) {
                case 'Open24 Sale':
                    self.SoftwareCODE = 'STMN';
                    break;
                case 'Open24 Gara':
                    self.SoftwareCODE = 'OTXMXDD';
                    break;
                case 'Open24 Beauty':
                    self.SoftwareCODE = 'STPG';
                    break;
            }
            console.log(self);
            if (localValidate.CheckNull(self.FullName)) {
                alert("Vui lòng nhập họ tên");
            }
            else if (localValidate.CheckNull(self.Phone)) {
                alert("Vui lòng nhập số điện thoại");
            }
            else if (localValidate.CheckNull(self.SoftwareCODE)) {
                alert("Vui lòng chọn ngành nghề bạn kinh doanh");
            }
            else {
                $.ajax({
                    type: 'GET',
                    url: "https://geoip-db.com/json/",
                    success: function (data) {
                        self.CallInsert(JSON.parse(data));
                    },
                    timeout: 3000,      // 3 seconds
                    error: function (qXHR, textStatus, errorThrown) {
                        self.CallInsert(null);
                        if (textStatus === "timeout") {
                            console.log(qXHR);
                        }
                    }
                });
            }
        },
        CallInsert: function (IpAddress) {
            var self = this;

            var diachi = "";
            var ip4 = "";
            if (IpAddress !== null && IpAddress !== undefined) {
                diachi = (IpAddress.city !== '' && IpAddress.city !== null) ? IpAddress.city + "-" + IpAddress.country_name : IpAddress.country_name;
                ip4 = IpAddress.IPv4;
            }
           
            var model = {
                FullName: self.FullName,
                Phone: self.Phone,
                Note: self.Noted,
                Software: self.SoftwareCODE,
                
                Address: self.Address,
                Company: self.Company,
                Email: self.Email,

                Type: self.Type,
                IPv4: ip4,
                IpAddress: diachi.trim()

            };
            console.log(model)
            $.ajax({
                data: model,
                url: '/Open24Api/ApiHome/FormDangKyTuVan',
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (item) {
                    if (item.res === true) {
                        window.location.href = "/dang-ky-dung-thu-thanh-cong";
                        self.FullName = '';
                        self.Phone = '';
                        self.Note = '';
                        self.Address = "";
                    }
                    else {
                        alert(item.mess);
                    }
                }
            });
        },

    },
    created: function () {
        let self = this;
        console.log(self);
    },
    watch: {
        FullName: function () {
            let self = this;
            console.log(self.FullName)
        },
        Email: function () {
            let self = this;
            console.log(self.Email)
        },
        Address: function () {
            let self = this;
            console.log(self.Address)
        },
        Noted: function () {
            let self = this;
            console.log(self.Noted)
        },
        Software: function () {
            let self = this;
            console.log(self.Software)
        },

        Company: function () {
            let self = this;
            console.log(self.Company)
        },
    },
    computed: {

        Softwave: function () {
            return "Open24 " + currentPage != null ? currentPage : "Sale";
        }
    }
})