function CustomerDetail() {
    var self = this;
    //===============================
    // Khai báo chung
    //===============================
    self.ListAdress = ko.observableArray();
    self.ListBusiness = ko.observableArray();
    self.nameCustomer = ko.observable();
    self.Note = ko.observable();
    self.CreatedDate = ko.observable();
    self.Description = ko.observable();
    self.Createby = ko.observable();
    self.listNews = ko.observableArray();
    //===============================
    // Load dữ liệu lúc vào form
    //===============================

    function LoadForm() {
        $.ajax({
            url: "/Open24Api/ApiCustomer/GetCustomerNewDate",
            type: 'GET',
            async: true,
            dataType: 'json',
            contentType: 'application/json',
            success: function (result) {
                if (result.res === true) {
                    self.listNews(result.DataSoure);
                }
                else { 
                    console.log(result.mess);
                }
            },
            error: function (result) {
                console.log(result);
            }
        });
        var id = $('#CustomerClientId').val();
        var url = "/Open24Api/ApiCustomer/GetViewDetail/" + id;
        $.ajax({
            url: url ,
            type: 'GET',
            async: true,
            dataType: 'json',
            contentType: 'application/json',
            success: function (result) {
                if (result.res === true) {
                    self.nameCustomer(result.DataSoure.Name);
                    self.Note(result.DataSoure.Note);
                    self.Description(result.DataSoure.Description);
                    self.Createby(result.DataSoure.CreatedBy);
                    var dt = new Date(result.DataSoure.CreatedDate);
                    var y = dt.getFullYear();
                    var mo = dt.getMonth() + 1;
                    var d = dt.getDate();
                    var h = dt.getHours();
                    var mi = dt.getMinutes();

                    var weekday = new Array(7);
                    weekday[0] = "Chủ nhật";
                    weekday[1] = "Thứ hai";
                    weekday[2] = "Thứ ba";
                    weekday[3] = "Thứ tư";
                    weekday[4] = "Thứ năm";
                    weekday[5] = "Thứ sáu";
                    weekday[6] = "Thứ bảy";
                    var n = weekday[dt.getDay()];

                    var date = n + ", " + d + "/" + mo + "/" + y + " | " + h + ":" + mi + " GMT+7";

                    self.CreatedDate(date);
                }
                else {
                    console.log(result.mess);
                }
            },
            error: function (result) {
                console.log(result);
            }
        });
    }

    LoadForm();

};
ko.applyBindings(new CustomerDetail());

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
        var a = moment(config).format('DD/MM/YYYY');
        return a;
    }
}
