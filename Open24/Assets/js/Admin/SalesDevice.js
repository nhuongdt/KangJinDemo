function SalesDevice() {
    var self = this;
    //===============================
    // Khai báo chung
    //===============================
    self.koSearch = ko.observable();
    self.ListSalesDevice = ko.observableArray();
    self.SalesGroupDevices = ko.observableArray();
    self.SalesGroupDevicesActive = ko.observableArray();
    self.PageItem = ko.observable();
    self.pageCount = ko.observable();
    self.page = ko.observable();

    self.Devices_ID = ko.observable();
    self.Devices_Title = ko.observable();
    self.Devices_Name = ko.observable();
    self.Devices_Trademark = ko.observable();
    self.Devices_TimeGuarantee = ko.observable();
    self.Devices_Status = ko.observable();
    self.Devices_Price = ko.observable();
    self.Devices_PriceSale = ko.observable();
    self.Devices_IsSalePrice = ko.observable();
    self.Devices_SalesImgDevices = ko.observableArray();

    self.GroupDevice_ID = ko.observable();
    self.GroupDevice_Name = ko.observable();
    self.GroupDevice_Note = ko.observable();
    self.GroupDevice_Status = ko.observable();
    //===============================
    // Click tìm kiếm
    //===============================

    self.SearchGrid = function (d, e) {
        if (e.keyCode === 13) {
            var object = {
                Search: self.koSearch(),
                Page: self.page(),
                Limit: $('#SelectedLimit').val(),
                Columname: null,
                Sort: null
            };
            FilterGrid(object);
        }
    }
    //===============================
    // Phân trang 
    //===============================
    self.ClickPrevious = function () {
        if (self.page() > 1) {
            self.page(self.page() - 1);
            var object = {
                Search: self.koSearch(),
                Page: self.page(),
                Limit: $('#SelectedLimit').val(),
                Columname: null,
                Sort: null
            };
            FilterGrid(object);
        }
    }

    self.ClickNext = function () {
        if (self.page() < self.pageCount) {
            self.page(self.page() + 1);
            var object = {
                Search: self.koSearch(),
                Page: self.page(),
                Limit: $('#SelectedLimit').val(),
                Columname: null,
                Sort: null
            };
            FilterGrid(object);
        }
    }

    self.netPageKeyup = function (d, e) {
        if (e.keyCode === 13) {
            if (self.page() > self.pageCount()
                || self.pageCount() === 1
                || self.page() < 1
                || !$.isNumeric(self.page())) {
                self.page(1);
            }
            var object = {
                Search: self.koSearch(),
                Page: self.page(),
                Limit: $('#SelectedLimit').val(),
                Columname: null,
                Sort: null
            };
            FilterGrid(object);

        }
    }

    $('#SelectedLimit').on('change', function () {
        self.page(1);
        var object = {
            Search: self.koSearch(),
            Page: self.page(),
            Limit: $('#SelectedLimit').val(),
            Columname: null,
            Sort: null
        };
        FilterGrid(object);

    });
    function FilterGrid(model) {
            $.ajax({
                url: '/Open24Api/ApiSalesDevice/SearchGrid',
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: model,
                success: function (result) {
                    if (result.res === true) {
                        self.ListSalesDevice(result.DataSoure.Data);
                        self.PageItem(result.DataSoure.PageItem);
                        self.pageCount(result.DataSoure.PageCount);
                        self.page(result.DataSoure.Page);
                    }
                    else {
                        AlertError(result.mess);
                    }
                },
                error: function () {
                    alert("Đã xảy ra lỗi.");
                }
            });
    }
    //===============================
    // Load lúc vào trang
    //===============================
    function getGroupDevice()
    {

        $.getJSON('/Open24Api/ApiSalesDevice/GetGroupDevices', function (data) {
            self.SalesGroupDevices(data);
            self.SalesGroupDevicesActive(data.filter(x => x.Status===true))
        });
    }
    function loadFirst()
    {
        self.koSearch(null);
        self.page(1);
        var object = {
            Search: self.koSearch(),
            Page: self.page(),
            Limit: $('#SelectedLimit').val(),
            Columname: null,
            Sort: null
        };
        getGroupDevice();
        FilterGrid(object);
    }
    loadFirst();
    $('#ModalSalesDevices').on('hidden.bs.modal', function () {
        $(this).find('.nav-tabs a:first').tab('show');
    });
    //===============================
    // Thêm mới trang thiết bị
    //===============================
    self.AddSalesDiveces = function () {
        self.Devices_Title('Thêm mới thiết bị bán hàng');
        self.Devices_ID(null);
        self.Devices_Name(null);
        self.Devices_Trademark(null);
        self.Devices_TimeGuarantee(null);
        self.Devices_Status(true);
        self.Devices_Price(0);
        self.Devices_PriceSale(0);
        self.Devices_IsSalePrice(false);
        CKEDITOR.instances['CK_ApplicationReal'].setData(null);
        CKEDITOR.instances['CK_DigitalInformation'].setData(null);
        CKEDITOR.instances['CK_SpecialPoint'].setData(null);
        self.Devices_SalesImgDevices([]);
        $('#ModalSalesDevices').modal('show');
    }
    //===============================
    // Cập nhật trang thiết bị
    //===============================
    self.UpdateSalesDiveces = function (item) {
        self.Devices_Title('Cập nhật thiết bị bán hàng');
        self.Devices_SalesImgDevices($.extend(true, [], item.SalesImgDevices));
        self.Devices_ID(item.ID);
        self.Devices_Name(item.Name);
        self.Devices_Trademark(item.Trademark);
        self.Devices_TimeGuarantee(item.TimeGuarantee);
        self.Devices_Status(item.Status);
        self.Devices_Price(item.Price);
        $('#selectSalesGroupDevices').val(item.GroupDeviceId)
        self.Devices_PriceSale(item.PriceSale);
        self.Devices_IsSalePrice(item.IsSalePrice);
        CKEDITOR.instances['CK_ApplicationReal'].setData(item.ApplicationReal);
        CKEDITOR.instances['CK_DigitalInformation'].setData(item.DigitalInformation);
        CKEDITOR.instances['CK_SpecialPoint'].setData(item.SpecialPoint);
        $('#ModalSalesDevices').modal('show');
    }
    //===============================
    // Xóa trang thiết bị
    //===============================
    self.DeleteSalesDiveces = function (model) {
        if (confirm('Bạn có chắc chắn muốn xóa thiết bị này không?')) {

            $.ajax({
                url: '/Open24Api/ApiSalesDevice/DeleteSalesDevices',
                type: 'POST',
                data: JSON.stringify({ ID: model.ID }),
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    if (result.res === true) {
                        loadFirst();
                        AlertNotice(result.mess);
                    }
                    else {
                        AlertError(result.mess);
                    }
                }
            });

        }


    }
    //===============================
    // Xóa ảnh chi tiết
    //===============================
    self.removeImg = function (item) {
        self.Devices_SalesImgDevices.remove(item);
    }
    //===============================
    // Thêm mới hoặc update dữ liệu 
    // vào DB
    //===============================
    self.saveSalesDeices = function () {

        if (self.Devices_Name() === undefined
            || self.Devices_Name()=== null
            || self.Devices_Name().replace(/\s+/g, '') === "") {

            AlertError("Vui lòng nhập tên thiết bị.");
        }
        else if (self.Devices_Price() === undefined
            || self.Devices_Price() === null
            || self.Devices_Price() === "") {
            AlertError("Vui lòng nhập giá thiết bị.");
        }
        else if((self.Devices_PriceSale() === undefined
            || self.Devices_PriceSale() === null
            || self.Devices_PriceSale() === ""
            )&& self.Devices_IsSalePrice() == true) {
            AlertError("Khi tích chọn giảm giá vui lòng nhập giá sau giảm.");

        }
        else {
            var model = {
                ID: self.Devices_ID(),
                Name: self.Devices_Name(),
                Trademark:self.Devices_Trademark(),
                GroupDeviceId: $('#selectSalesGroupDevices').val(),
                TimeGuarantee: self.Devices_TimeGuarantee,
                SpecialPoint: CKEDITOR.instances['CK_SpecialPoint'].getData(),
                ApplicationReal: CKEDITOR.instances['CK_ApplicationReal'].getData(),
                DigitalInformation: CKEDITOR.instances['CK_DigitalInformation'].getData(),
                Status: self.Devices_Status(),
                Price: self.Devices_Price(),
                PriceSale: self.Devices_PriceSale(),
                IsSalePrice: self.Devices_IsSalePrice(),
                SalesImgDevices: self.Devices_SalesImgDevices()
            }
            $.ajax({
                url: '/Open24Api/ApiSalesDevice/SaveSalesDevices',
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: model,
                success: function (result) {
                    if (result.res === true) {
                        $('#ModalSalesDevices').modal('hide');
                        loadFirst();
                        AlertNotice(result.mess);
                    }
                    else {
                        AlertError(result.mess);
                    }
                },
                error: function () {
                    alert("Đã xảy ra lỗi.");
                }
            });
        }
    }

    //===============================
    // Mở popup nhóm trang thiết bị
    //===============================
    self.GroupDevices = function () {
        self.BackSalesGroupDevice();
        $('#ModalSalesGroupDevices').modal('show');
    }
    //===============================
    // update nhóm trang thiết bị
    //===============================
    self.UpdateSalesGroupDiveces = function (item) {
        self.GroupDevice_ID(item.ID);
        self.GroupDevice_Name(item.Name);
        self.GroupDevice_Note(item.Note);
        self.GroupDevice_Status(item.Status);
        $('#ListGroupDevice').hide();
        $('#NewGroupDeviceView').show();
    }
    //===============================
    // thêm mới nhóm thiết bị
    //===============================
    self.AddSalesGroupDevice = function () {
        self.GroupDevice_ID(null);
        self.GroupDevice_Name(null);
        self.GroupDevice_Note(null);
        self.GroupDevice_Status(true);
        $('#ListGroupDevice').hide();
        $('#NewGroupDeviceView').show();
    }
    //===============================
    // Xóa nhóm trang thiết bị
    //===============================
    self.DeleteSalesGroupDiveces = function (model) {
        if (confirm('Bạn có chắc chắn muốn xóa thiết bị này không?')) {

            $.ajax({
                url: '/Open24Api/ApiSalesDevice/DeleteSalesGroupDevices',
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: model,
                success: function (result) {
                    if (result.res === true) {
                        getGroupDevice();
                        AlertNotice(result.mess);
                    }
                    else {
                        AlertError(result.mess);
                    }
                }
            });

        }

    }
    //===============================
    // Thêm mới hoặc sửa nhóm thiết bị
    //===============================
    self.SaveSalesGroupDevice = function () {
        if (self.GroupDevice_Name() === undefined
            || self.GroupDevice_Name() === null
            || self.GroupDevice_Name().replace(/\s+/g, '') === "") {

            AlertError("Vui lòng nhập tên nhóm thiết bị.");
        }

        else {
            var model = {
                ID: self.GroupDevice_ID(),
                Name: self.GroupDevice_Name(),
                Note: self.GroupDevice_Note(),
                Status: self.GroupDevice_Status(),
            }
            $.ajax({
                url: '/Open24Api/ApiSalesDevice/SaveSalesGroupDevices',
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: model,
                success: function (result) {
                    if (result.res === true) {
                        getGroupDevice();
                        self.BackSalesGroupDevice();
                        AlertNotice(result.mess);
                    }
                    else {
                        AlertError(result.mess);
                    }
                },
                error: function () {
                    alert("Đã xảy ra lỗi.");
                }
            });
        }
    }

    self.BackSalesGroupDevice = function () {
        self.GroupDevice_ID(null);
        self.GroupDevice_Name(null);
        self.GroupDevice_Note(null);
        self.GroupDevice_Status(true);
        $('#ListGroupDevice').show();
        $('#NewGroupDeviceView').hide();
    }
    return self;
};
var SalesDevices = new SalesDevice();
ko.applyBindings(SalesDevices);
//===============================
// thay đổi ảnh 
//===============================
$(document).ready(function () {
    $('#selectImage').on('click', function (e) {
        e.preventDefault();
        var finder = new CKFinder();
        finder.selectActionFunction = function (url) {
            if (!SalesDevices.Devices_SalesImgDevices().some(x => x.SrcImage.indexOf(url) > -1)) {
                SalesDevices.Devices_SalesImgDevices.push({ ID: 0, SrcImage: url, SalesDeviceID: self.Devices_ID });
             
            }
          
        };
        finder.popup();
    });

});
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
//===============================
// Hiện thị trạng thái
//===============================
function ConvertTrangthai(value) {
    if (value === true) {
        return "Hiện thị"
    }
    else if (value === false) {
        return "Ẩn"
    }
    else {
        return "";
    }
};
function ConvertTrangthaiActive(value) {
    if (value === true) {
        return "Hoạt động"
    }
    else if (value === false) {
        return "Không hoạt động"
    }
    else {
        return "";
    }
};