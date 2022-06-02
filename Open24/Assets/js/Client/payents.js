var payents = function () {
    var self = this;
    self.listCart = ko.observableArray();
    self.TT_OrderName = ko.observable('--- Chọn tỉnh thành quận huyện ---');
    self.TT_OrderID = ko.observable();
    self.TT_ReceivedName = ko.observable('--- Chọn tỉnh thành quận huyện ---');
    self.TT_ReceivedID = ko.observable();
    self.PR_Count = ko.observable();
    self.listTinhThanh = ko.observableArray();
    self.listQuanHuyen = ko.observableArray();
    self.MoneyComputed = ko.observable();
    self.payment = ko.observable(0);
    self.visibleButton = ko.observable(false);
    $("body").on('CartOnsuccess', function () {
        loadCart();
    });
    function loadCart() {
        var request = LocalIndexDB.Connect().getAll();
        request.onsuccess = function (evt) {
            self.listCart(request.result);
            if (request.result.length > 0) {
                self.visibleButton(true);
            }
            else {
                self.visibleButton(false);
            }
            self.PR_Count('(' + self.listCart().length + ' sản phẩm)');
            self.MoneyComputed(FormatVND(self.listCart().reduce((a, b) => +a + +b.salesDevice_Money, 0)));
        };
    }
    function loadForm() {
        $.getJSON("/Open24Api/ApiProduct/GetAllTinhThanh", function (data) {
            self.listTinhThanh(data);
        });
    }
    loadForm();
    self.selectChangeTT = function (item) {
        self.TT_OrderID(item.ID);
        self.TT_OrderName(item.Value);
        $(".list-kv1").hide();
    }
    self.selectChangeTTReceived = function (item) {
        self.TT_ReceivedID(item.ID);
        self.TT_ReceivedName(item.Value);
        $(".list-kv1").hide();
    }
    function showerror(id,mess)
    {
        $(id).next('.error-order').text(mess);
    }
    function Hideerror(id) {
        $(id).next('.error-order').text('');
    }
    self.AddOrder = function () {
        var checkReceived = $('.form-group input[type=checkbox]').is(":checked");
        var result = true;
        if (validateNull('#OrderName')) {
            showerror('#OrderName', "Vui lòng nhập họ tên");
            result = false;
        }
        if (validateNull('#OrderEmail')) {
            showerror('#OrderEmail',"Vui lòng nhập Email");
            result = false;
        }
        if (!validateEmail('#OrderEmail')) {
            showerror('#OrderEmail',"Email không hợp lệ");
            result = false;
        }
        if (validateNull('#OrderPhone')) {
            showerror('#OrderPhone',"Vui lòng nhập số điện thoại");
            result = false;
        }
        if (!validatePhone('#OrderPhone')) {
            showerror('#OrderPhone',"Số điện thoại không hợp lệ");
            result = false;
        }
        if (validateNull('#OrderAdress')) {
            showerror('#OrderAdress', "Vui lòng nhập địa chỉ");
            result = false;
        }
        if (self.TT_OrderID() === null || self.TT_OrderID() === undefined) {
            result = false;
            $('#DSTinhThanh1').closest('.list-kv1').prev('.kv12').css('border', '1px solid red');
            $('#DSTinhThanh1').closest('.list-kv1').next('.error-order').text( "Vui lòng chọn tỉnh thành quận huyện");
        }
        else {
            $('#DSTinhThanh1').closest('.list-kv1').next('.error-order').text('');
            $('#DSTinhThanh1').closest('.list-kv1').prev('.kv12').css('border', '1px solid #ccc');
        }
        if (checkReceived) {
            if (validateNull('#ReceivedName')) {
                showerror('#ReceivedName', "Vui lòng nhập họ tên");
                result = false;
            }
            if (validateNull('#ReceivedPhone')) {
                showerror('#ReceivedPhone', "Vui lòng nhập số điện thoại");
                result = false;
            }
            if (!validatePhone('#ReceivedPhone')) {
                showerror('#ReceivedPhone', "Số điện thoại không hợp lệ");
                result = false;
            }
            if (validateNull('#ReceivedAdress')) {
                showerror('#ReceivedAdress', "Vui lòng nhập địa chỉ");
                result = false;
            }
            if (self.TT_ReceivedID() === null || self.TT_ReceivedID() === undefined) {
                result = false;
                $('#DSTinhThanh2').closest('.list-kv1').prev('.kv12').css('border', '1px solid red');
                $('#DSTinhThanh2').closest('.list-kv1').next('.error-order').text("Vui lòng chọn tỉnh thành quận huyện");
            }
            else {
                $('#DSTinhThanh2').closest('.list-kv1').next('.error-order').text('');
                $('#DSTinhThanh2').closest('.list-kv1').prev('.kv12').css('border', '1px solid #ccc');
            }
        }
        if (result) {

            var listproduct = self.listCart().map(x => {
                return {
                    SalesDevice_ID: x.salesDevice_Id,
                    Quantity: x.salesDevice_Quantity,
                    Price: x.salesDevice_Price,
                    Encoder: x.salesDevice_Encoder,
                    Money: x.salesDevice_Money,
                    Name: x.salesDevice_Name
                };
            });
            var model = {
                UserOrder: $('#OrderName').val(),
                EmailOrder: $('#OrderEmail').val(),
                PhoneOrder: $('#OrderPhone').val(),
                AdressOrder: $('#OrderAdress').val(),
                UserReceived: $('#ReceivedName').val(),
                PhoneReceived: $('#ReceivedPhone').val(),
                AdressReceived: $('#ReceivedAdress').val(),
                TinhThanhOrder_ID: self.TT_OrderID(),
                CheckReceived: checkReceived,
                payment: self.payment(),
                TinhThanhReceived_ID: self.TT_ReceivedID(),
                Note: $('#Note').val(),
                ProductDevices: listproduct
            }
            console.log(model);
            $.ajax({
                url: "/Open24Api/ApiProduct/SaveOrder",
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: model,
                success: function (result) {
                    if (result.res === true) {
                        LocalIndexDB.Connect().clear();
                        window.location.href = "/dat-hang-thanh-cong/" + result.DataSoure;
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

    function HideError()
    {
        $('.error-order').val(null);
        $('.error-order').hide();
    }
    //HideError();
    function validatePhone(id) {
        if (!validateNull(id)) {
            var fld = $(id).val().trim();
            var phoneno = /^\(?([0]{1}[1-9]{1}[0-9]{2})\)?[]?([0-9]{3})[]?([0-9]{3})$/;
            var phoneno1 = /^\(?[0]{1}[1-9]{1}[0-9]{3}\)?[]?([0-9]{3})[]?([0-9]{3})$/;
            var phoneno3 = /^\(?[0]{1}[1-9]{1}[0-9]{4}\)?[]?([0-9]{3})[]?([0-9]{3})$/;
            var phoneno4 = /^\(?[0]{1}[1-9]{1}[0-9]{5}\)?[]?([0-9]{4})[]?([0-9]{4})$/;
            var phoneno2 = /^\(?[0]{1}[1-9]{1}[0-9]{6}\)?[]?([0-9]{4})[]?([0-9]{4})$/;
            var allow = allow1 = allow2 = false;

            allow = fld.match(phoneno2);
            allow1 = fld.match(phoneno3);
            allow2 = fld.match(phoneno4);

            if (!(fld.match(phoneno)) && !fld.match(phoneno1) && !allow && !allow1 && !allow2) {

                $(id).css('border', '1px solid red');
                return false;
            }
            else {
                $(id).css('border', '1px solid #ccc');
            }
        }
        return true;
    };
    function validateEmail(id) {
        if (!validateNull(id)) {
            var email = $(id).val().trim();
            var res = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            if (res.test(email)) {
                    $(id).css('border', '1px solid #ccc');
                }
                else {
                $(id).css('border', '1px solid red');
                return false;
                }
        }
        return true;
    };
    function validateNull(id) {
        var result = ($(id).val() === null
            || $(id).val() === undefined
            || $(id).val().replace(/\s+/g, '') === "");
        if (result) {
            $(id).css('border', '1px solid red');
        }
        else {
            $(id).css('border', '1px solid #ccc');
            Hideerror(id);
        }
        return result;
    }
    return self;
}
var payent = new payents();
ko.applyBindings(payent);
function ResetTT2() {
    payent.TT_ReceivedID(null);
    payent.TT_ReceivedName('--- Chọn tỉnh thành quận huyện ---');
    $(".list-kv1").hide();
}
function ResetTT1() {
    payent.TT_OrderID(null);
    payent.TT_OrderName('--- Chọn tỉnh thành quận huyện ---');
    $(".list-kv1").hide();
}
$(".round").next(".dettail-pay").show();
$(".choose-pay").click(function () {
    payent.payment($(this).data("id"));
    $(".choose-pay").removeClass("round");
    $(".dettail-pay").hide();
    $(this).addClass("round");
    $(this).next(".dettail-pay").show();

});