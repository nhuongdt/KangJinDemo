var vmLoginSsoft = new Vue({
    el: '#LoginSsoft',
    data: {
        User: {
            TaiKhoan: "",
            MatKhau: "",
            GhiNho: false,
        },
    },
    methods: {
        Submit: function () {
            var self = this;
            var model = self.User;
            $.ajax({
                data: model,
                url: '/SsoftApi/ApiUser/LoginAcount',
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (result) {
                    if (result.res === true) {
                        location.href = "/Admin/Home/Index";
                    }
                    else {
                        $('.error').text(result.mess);
                    }
                }
            });
        }
    }
});