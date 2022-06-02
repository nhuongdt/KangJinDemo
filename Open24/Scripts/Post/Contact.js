var View = function () {
    var self = this;
    //gửi liên hệ
    self.fullname_lh = ko.observable();
    self.email_lh = ko.observable();
    self.phone_lh = ko.observable();
    self.address_lh = ko.observable();
    self.note_lh = ko.observable();

    self.guilienhe = function () {
        var fullname = self.fullname_lh();
        var email = self.email_lh();
        var phone = self.phone_lh();
        var address = self.address_lh();
        var note = self.note_lh();

        var chkname_lh = $("#fullname_lh").val();
        var chkemail_lh = $("#email_lh").val();
        var chkphone_lh = $("#phone_lh").val();

        if (chkname_lh !== "" && chkemail_lh !== "" && chkphone_lh !== "") {
            var model = {
                FullName: fullname,
                Email: email,
                Phone: phone,
                Address: address,
                Note: note
            };
            $.ajax({
                data: model,
                url: '/Open24Api/PostAPI/' + "ThemLienHe",
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8", 
                success: function (item) {
                    self.fullname_lh("");
                    self.email_lh("");
                    self.phone_lh("");
                    self.address_lh("");
                    self.note_lh("");
                    alert('Success');
                }
            });
        } else {
            alert("Vui lòng nhập đầy đủ những thông tin cần thiết");
        }
    }
}
ko.applyBindings(new View());

$('.viewmapHN').on("click", function () {
    $("#showHN").toggle();
});

$('.viewmapHCM').on("click", function () {
    $("#showHCM").toggle();
});