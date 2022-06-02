var AppChat = function () {
    var self = this;
    //===============================
    // Khai báo chung
    //===============================
    self.listUserAppChat = ko.observableArray();
    self.listAppChatDetail = ko.observableArray();

    //===============================
    // Show chi tiết
    //===============================
    self.ShowValue = function (value)
    {
        self.listAppChatDetail(value.ChatPage);
        $('#myModal').modal('show');
    }
    
    //===============================
    // Load dữ liệu nhóm người dùng
    //===============================
    function GetUserAppChat() {
        $.getJSON('/Open24Api/AppChat/GetAll').done(function (data) { 
            self.listUserAppChat(data);
        });
        setTimeout(GetUserAppChat, 60000);
    
    }
    GetUserAppChat();

}
ko.applyBindings(new AppChat());


//===============================
// Hiện thị Datetime
//===============================
function ConvertDate(config) {
    if (config === undefined
        || config === null
        || config.replace(/\s+/g, '') === "") {
        return "";
    }
    else {
        var a = moment(config).format('DD/MM/YYYY  HH:mm:ss');
        return a;
    }
}
