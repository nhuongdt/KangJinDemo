function AccountLogin() {
    var self = this;
    self.userName = ko.observable();
    self.password = ko.observable();
    self.myMessage = ko.observable();
    //===============================
    // click đăng nhập
    //===============================
    self.LoginAccount = function () {
        var user = self.userName();
        var pass = self.password();
        if (user !== undefined
            && user.replace(/\s+/g, '') !== ""
            && pass !== undefined
            && pass.replace(/\s+/g, '') !== "")
        {
            var userLogin = {
                UserName: user,
                UserPassword: pass
            };
            $.ajax({
                url: '/AdminPage/Account/Login',
                type: 'POST',
                data: ko.toJSON({ UserLogin: userLogin }),
                contentType: 'application/json',
                success: function (result) {
                    if (result.res === true) { 
                        self.myMessage();
                        location.href = "/AdminPage/Home/Index";
                    }
                    else {
                        self.myMessage(result.mess);
                    }
                },
                error: function () {
                    self.myMessage("Đã xảy ra lỗi.");
                }
            });
        }
        else
        {
            self.myMessage("Vui lòng điền đầy đủ thông tin tài khoản và mật khẩu.");
        }
    };

    self.loginKeyUp = function (d, e) {
        if (e.keyCode === 13) {
            self.LoginAccount();
        }

    };
}
ko.applyBindings(new AccountLogin());