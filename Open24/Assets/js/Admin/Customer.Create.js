function CustomerCreate() {
    var self = this;
    //===============================
    // Khai báo chung
    //===============================
    self.nameCustomer = ko.observable();
    self.title = ko.observable();
    self.Area = ko.observable('');
    self.Phone = ko.observable('');
    self.Email = ko.observable('');
    self.Note = ko.observable();
    self.Description = ko.observable();
    self.koStatus = ko.observable(true);
    self.ListAdress = ko.observableArray();
    self.ListBusiness = ko.observableArray();
    self.IsNew = ko.observable(true);
    self.Images = ko.observable();
    self.Url = ko.observable();
    self.Id = ko.observable();
    self.koTinhThanh = ko.observable();
    self.DistrictCity = ko.observable();
    self.koSearch = ko.observable();
    self.listcity = ko.observableArray();
    self.prioritize = ko.observable();
    //===============================
    // Cập nhật đối tác
    //===============================
    self.UpdateCustomer = function () {
        if (self.IsNew() === true) 
        {
            Insert();
        }
        else if (self.IsNew() === false)
        {
            Update();
        }

    }
    //===============================
    // chọn khu vực
    //===============================
    self.clickselect = function (value)
    {
        self.koTinhThanh(value.TEN);
        self.DistrictCity(value.ID);
        $('.fa-caret-down').show();
        $('.fa-caret-up').hide();
        $('.list-kv1').hide();
    }
    //===============================
    // Tìm kiếm khu vực
    //===============================
    self.lists = ko.observableArray();
    self.searchKhuVuc = function (d,e)
    {
        if (e.keyCode === 13) {
            if (self.koSearch() !== null && self.koSearch() !== '') {
                self.lists.removeAll();
                var list = self.listcity();
                for (var x in list) {
                    if (list[x].TEN.toLowerCase().indexOf(self.koSearch().toLowerCase()) >= 0) {
                        self.lists.push(list[x]);
                    }
                }
                self.ListAdress(self.lists());
            }
            else {
                self.ListAdress(self.listcity());
            }
        }

    }

    //===============================
    // Thêm mới đối tác
    //===============================

    function Insert()
    {
        var mota = CKEDITOR.instances['txtDescription'].getData();

        var fileUpload = $("#imageUploadForm").get(0);
        var files = fileUpload.files;
        var fileData = new FormData();
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
        }
        fileData.append('username', 'manas');

        if (self.nameCustomer() === undefined
            || self.nameCustomer() === null
            || self.nameCustomer().replace(/\s+/g, '') === "") {

            AlertError("Vui lòng nhập tên đối tác.");
        }
        else if (self.Url() === undefined
            || self.Url() === null
            || self.Url().replace(/\s+/g, '') === "") {
            AlertError("Vui lòng nhập url bài viết.");
            return;
        }
        else if (validatespecialcharacters(self.nameCustomer())) {

            AlertError("Tên đối tác không được chứa ký tự đặc biệt.");
        }
        else if (mota === undefined
            || mota === null
            || mota.replace(/\s+/g, '') === "") {
            AlertError("Vui lòng nhập mô tả về đối tác.");
        }
        else if (self.Note() === undefined
            || self.Note() === null
            || self.Note().replace(/\s+/g, '') === "") {
            AlertError("Vui lòng nhập ghi chú.");
        }
        else if (!validateEmail(self.Email())) {
            AlertError("Địa chỉ Email không hợp lệ.");
        }
        else if (!validatePhone(self.Phone())) {
            AlertError("Số điện thoại không hợp lệ.");
        }
        else if (self.DistrictCity() === '' || self.DistrictCity() === null || self.DistrictCity() === undefined) {
            AlertError("Vui lòng nhập khu vực.");
            return;
        }
        else {

            $.ajax({
                data: fileData,
                url: '/Open24Api/ApiCustomer/UploadImagesCustomer',
                type: 'POST',
                processData: false,
                contentType: false,
                success: function (result) {
                    if (result.res === true) {
                        var model = {
                            Name: self.nameCustomer(),
                            Adress: self.Area(),
                            DistrictCity: self.DistrictCity(),
                            TypeBusiness: $('#selectBusiness').val(),
                            Description: mota,
                            Note: self.Note(),
                            Status: self.koStatus(),
                            Phone: self.Phone(),
                            Email: self.Email(),
                            Images: result.DataSoure,
                            Url: self.Url()
                        };
                        $.ajax({
                            data: model,
                            url: '/Open24Api/ApiCustomer/InsertCustomer',
                            type: 'POST',
                            async: false,
                            dataType: 'json',
                            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                            success: function (result) {
                             
                                if (result.res === true) {
                                    AlertNotice(result.mess);
                                    location.href = "/AdminPage/Customer/Index";
                                }
                                else
                                {
                                    AlertError(result.mess);
                                }
                            },
                            error: function (result) {
                                exception(result);
                            }
                        });

                    }
                    else {
                        AlertError(result.mess);
                    }
                },
                error: function (result) {
                    exception(result);
                }
            });

        }

    }
    self.nameCustomerChanged = function () {

        self.Url(localValidate.ConvertUrl(self.nameCustomer()));
    }
    self.UrlChanged = function () {

        self.Url(localValidate.ConvertUrl(self.Url()));
    }
    //===============================
    // Sửa  đối tác
    //===============================

    function Update() {

        var mota = CKEDITOR.instances['txtDescription'].getData();
        if (self.nameCustomer() === undefined
            || self.nameCustomer() === null
            || self.nameCustomer().replace(/\s+/g, '') === "") {

            AlertError("Vui lòng nhập tên đối tác.");
            return;
        }
        else if (self.Url()  === undefined
            || self.Url()  === null
            || self.Url() .replace(/\s+/g, '') === "") {
            AlertError("Vui lòng nhập url bài viết.");
            return;
        }
        else if (mota === undefined
            || mota === null
            || mota.replace(/\s+/g, '') === "") {
            AlertError("Vui lòng nhập mô tả về đối tác.");
            return;
        }
        else if (validatespecialcharacters(self.nameCustomer())) {
            AlertError("Tên đối tác không được chứa ký tự đặc biệt.");
            return;
        }
        else if (self.Note() === undefined
            || self.Note() === null
            || self.Note().replace(/\s+/g, '') === "") {
            AlertError("Vui lòng nhập ghi chú.");
            return;
        }
        else if (!validateEmail(self.Email())) {
            AlertError("Địa chỉ Email không hợp lệ.");
            return;
        }
        else if (!validatePhone(self.Phone())) {
            AlertError("Số điện thoại không hợp lệ.");
            return;
        }
        else if (self.DistrictCity() === '' || self.DistrictCity() === null || self.DistrictCity() === undefined) {
            AlertError("Vui lòng nhập khu vực.");
            return;
        }
        // Không thay đổi ảnh
        if (self.Images() === $('#blah').attr('src')
            || ((self.Images() === null || self.Images() === '')
                && ($('#blah').attr('src') === undefined || $('#blah').attr('src') === '' || $('#blah').attr('src')===null)) ) {
           
            AjaxUpdate(self.Images(), mota);
        }
        else {
            var fileUpload = $("#imageUploadForm").get(0);
            var files = fileUpload.files;
            var fileData = new FormData();
            for (var i = 0; i < files.length; i++) {
                fileData.append(files[i].name, files[i]);
            }
            fileData.append('username', 'manas');

                $.ajax({
                    data: fileData,
                    url: '/Open24Api/ApiCustomer/UploadImagesCustomer',
                    type: 'POST',
                    processData: false,
                    contentType: false,
                    success: function (result) {
                        if (result.res === true) {
                            AjaxUpdate(result.DataSoure, mota);
                        }
                        else {
                            AlertError(result.mess);
                        }
                    },
                    error: function (result) {
                        exception(result);
                    }
                });

            }
        

    }

    function AjaxUpdate(images,mota)
    {
        var model = {
            ID: self.Id(),
            Name: self.nameCustomer(),
            Adress: self.Area(),
            DistrictCity: self.DistrictCity(),
            TypeBusiness: $('#selectBusiness').val(),
            Description: mota,
            Note: self.Note(),
            Status: self.koStatus(),
            Phone: self.Phone(),
            Email: self.Email(),
            Images: images,
            prioritize: self.prioritize(),
            Url: self.Url(),
        };
        $.ajax({
            data: model,
            url: '/Open24Api/ApiCustomer/UpdateCustomer',
            type: 'POST',
            async: false,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                if (result.res === true) {
                    location.href = "/AdminPage/Customer/Index";
                }
                else {
                    AlertError(result.mess);
                }
            },
            error: function (result) {
                exception(result);
            }
        });
    }

    //===============================
    // Load dữ liệu lúc vào form
    //===============================

    function GetValueCombobox() {
        self.title("Thêm mới đối tác");
        $.ajax({
            url: '/Open24Api/ApiCustomer/GetValueCombobox',
            type: 'GET',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                var id = window.sessionStorage.getItem("CustomerId");
                if (id !== null && id !== '') {
                    self.IsNew(false);
                    self.title("Cập nhật đối tác");
                    $('#prioritize').show();
                    $.ajax({
                        url: '/Open24Api/ApiCustomer/GetDetail/' + id,
                        type: 'GET',
                        async: true,
                        dataType: 'json',
                        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                        success: function (result) {
                            if (result.res === true) {
                                CKEDITOR.instances['txtDescription'].setData(result.DataSoure.Description);
                                self.Id(result.DataSoure.ID);
                                self.nameCustomer(result.DataSoure.Name);
                                self.Note(result.DataSoure.Note);
                                self.Area(result.DataSoure.Adress);
                                self.Phone(result.DataSoure.Phone);
                                self.Email(result.DataSoure.Email);
                                self.koStatus(result.DataSoure.Status);
                                self.DistrictCity(result.DataSoure.DistrictCity);
                                self.prioritize(result.DataSoure.prioritize);
                                let aray = result.DataSoure.Url.split('.')[0].split('/')[2].split("-");
                                aray.pop();
                                self.Url(aray.join('-'));
                                $('#selectBusiness').val(result.DataSoure.TypeBusiness);
                                self.Images(result.DataSoure.Images);
                                $('#blah').attr('src', self.Images())
                                    .width(200)
                                    .height(150);
                                var list = self.listcity();
                                for (var x in list) {
                                    if (list[x].ID.toLowerCase().indexOf(self.DistrictCity().toLowerCase()) >= 0) {
                                        self.koTinhThanh(list[x].TEN);
                                        return;
                                    }
                                }
                            }
                            else {
                                AlertError(result.mess);
                            }
                        },
                        error: function (result) {
                            exception(result);
                        }
                    });
                }
                if (result.res === true) {
                    self.listcity(result.DataSoure.DataTT);
                   
                    self.ListBusiness(result.DataSoure.DataNN);
                    self.ListAdress(result.DataSoure.DataTT);
                }
                else {
                    alert(result.mess);
                }
            },
            error: function (result) {
                exception(result);
            }
        });
    }

    GetValueCombobox();

};
ko.applyBindings(new CustomerCreate());

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
