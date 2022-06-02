function bussiness() {
    var self = this;
    //===============================
    // Khai báo chung
    //===============================
    self.koSearch = ko.observable();
    self.ListBusiness = ko.observableArray();
    self.ListBusinessDetail = ko.observableArray();
    self.PageItem = ko.observable();
    self.pageCount = ko.observable();
    self.page = ko.observable();
    self.listBussinessAll = ko.observableArray();
    self.PageItemDetail = ko.observable();
    self.pageCountDetail = ko.observable();
    self.pageDetail = ko.observable();
    self.listImage = ko.observableArray();
    self.TN_STT = ko.observable();
    self.TN_Ten = ko.observable();
    self.TN_TieuDe = ko.observable();
    self.TN_Status = ko.observable();
    self.TN_IsEdit = ko.observable(true);
    self.TN_ID = ko.observable();
    self.NganhNgheId = ko.observable();
    self.sort = ko.observable(0);
    self.Colum = ko.observable(1);
    self.KoTenNganhNghe = ko.observable();
    self.ID = ko.observable();
    self.koStatus = ko.observable(true);
    self.checkUpdateInsert = ko.observable(1);
    self.updateImage = ko.observable(false);
    //===============================
    // Cập nhật Ngành nghề kinh doanh
    //===============================
    self.UpdateBusiness = function (model)
    {
        self.checkUpdateInsert(0);
        self.koStatus(model.Status);
        self.KoTenNganhNghe(model.TenNganhNghe);
        self.ID(model.ID);
        tree.uncheckAll();
        self.updateImage(false);
        $('#imageBanner').attr('src', model.Image);
        $('#imageBannerMobile').attr('src', model.ImageMobile);
        $('#textTitle').text("Sửa đổi ngành nghề kinh doanh.");
        $('#myPermisssion').modal('show');
        $('#SHowLoad').show();
        $.getJSON('/Open24Api/Business/GetDetailRole?KeyId=' + model.ID, function (result) {
            $.each(result, function (i, item) {
                var node = tree.getNodeById(item);
                if (node !== null && node !== undefined) {
                    tree.check(node);
                }
            });
            $('#SHowLoad').hide();
        });
       
    }
     //===============================
    // Xóa nhóm ngành nghề kinh doanh
    //===============================
    self.deleteBusiness = function (model)
    {
        if (confirm('Bạn có chắc chắn muốn xóa ngành nghề này không?')) {

            $.ajax({
                url: '/Open24Api/Business/Delete',
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: model,
                success: function (result) {
                    if (result.res === true) {
                        AlertNotice(result.mess);
                        GetValueBusiness();
                        loadcomboboxSelectBusiness();
                    }
                    else {
                        AlertError(result.mess);
                    }
                }
            });
         
        }
    }
    //===============================
    // Click thêm mới nghành nghề kinh
    // doanh
    //===============================
    self.AddBusiness = function ()
    {
        self.updateImage(false);
        $('#imageBanner').attr('src', '/Content/images/imgdemo.png');
        $('#imageBannerMobile').attr('src', '/Content/images/imgdemo.png');
        self.checkUpdateInsert(1);
        self.koStatus(true);
        self.KoTenNganhNghe(null);
        tree.uncheckAll();
        $('#textTitle').text("Thêm mới ngành nghề kinh doanh.")
        $('#myPermisssion').modal('show');
        $('#SHowLoad').hide();
    }
    //===============================
    // Click tìm kiếm
    //===============================

    self.SearchGrid = function (d, e) {
        if (e.keyCode === 13) {
            var object = {
                Search: self.koSearch(),
                Page: self.page(),
                Limit: $('#SelectedLimit').val(),
                Columname: self.Colum(),
                Sort: self.sort()
            };

            FilterGrid(object);
        }
    }
    //===============================
    // Phân trang 
    //===============================
    self.ClickPrevious = function ()
    {
        if (self.page() > 1) {
            var object = {
                Search: self.koSearch(),
                Page: self.page() - 1,
                Limit: $('#SelectedLimit').val(),
                Columname: self.Colum(),
                Sort: self.sort()
            };

            FilterGrid(object);
        }
    }

    self.ClickNext = function ()
    {
        if (self.page() < self.pageCount()) {
            var object = {
                Search: self.koSearch(),
                Page: self.page() + 1,
                Limit: $('#SelectedLimit').val(),
                Columname: self.Colum(),
                Sort: self.sort()
            };
            FilterGrid(object);
        }
    }

    self.netPageKeyup = function (d, e) {
        if (e.keyCode === 13) {
            if (self.page() > self.pageCount()
                || self.pageCount() === 1
                || !$.isNumeric(self.page())) {
                self.page(1);
            }
            var object = {
                Search: self.koSearch(),
                Page: self.page(),
                Limit: $('#SelectedLimit').val(),
                Columname: self.Colum(),
                Sort: self.sort()
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
            Columname: self.Colum(),
            Sort: self.sort()
        };

        FilterGrid(object);
    });

    $('#sortMa').click(function () {
        self.Colum(0);
        SortGrid(this);
    });
    $('#sortTen').click(function () {
        self.Colum(1);
        SortGrid(this);
    });

    $('#sortCreateDate').click(function () {
        self.Colum(2);
        SortGrid(this);
    });
    $('#sortCreateBy').click(function () {
        self.Colum(3);
        SortGrid(this);
    });

    $('#sortModiDate').click(function () {
        self.Colum(4);
        SortGrid(this);
    });
    $('#sortModBy').click(function () {
        self.Colum(5);
        SortGrid(this);
    });
    $('#sortStatus').click(function () {
        self.Colum(6);
        SortGrid(this);
    });

    function SortGrid($item)
    {
        $("#iconSort").remove();
        if (self.sort() === 0)
        {
            self.sort(1);
            $item.innerHTML += " <i id='iconSort' class='fa fa-caret-down pull-right' aria-hidden='true'></i>";
        }
        else
        {
            self.sort(0);
            $item.innerHTML += " <i id='iconSort' class='fa fa-caret-up pull-right' aria-hidden='true'></i>";
        }
        var object = {
            Search: self.koSearch(),
            Page: self.page(),
            Limit: $('#SelectedLimit').val(),
            Columname: self.Colum(),
            Sort: self.sort()
        };
        FilterGrid(object);
    }

    //===============================
    // Tìm kiếm gridview chung
    //===============================
    function FilterGrid(model) {
        $.ajax({
            url: '/Open24Api/Business/SearchGrid',
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: model,
            success: function (result) {
                if (result.res === true) {
                    self.ListBusiness(result.DataSoure.Data);
                    self.PageItem(result.DataSoure.PageItem);
                    self.pageCount(result.DataSoure.PageCount);
                    self.page(result.DataSoure.Page);
                }
                else {
                    alert(result.mess);
                }
            },
            error: function () {
                alert("Đã xảy ra lỗi.");
            }
        });
    }
    //===============================
    // Load dữ liệu lúc vào form
    //===============================
    function GetValueBusiness()
    {
        $.ajax({
            url: '/Open24Api/Business/GetAllPage',
            type: 'GET',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                if (result.res === true) {
                    self.ListBusiness(result.DataSoure.Data);
                    self.PageItem(result.DataSoure.PageItem);
                    self.pageCount(result.DataSoure.PageCount);
                    self.page(result.DataSoure.Page);
                }
                else {
                    exception(result.mess);
                }
            },
            error: function (result) {
                exception(result);
            }
        });

       
    }
    function loadcomboboxSelectBusiness()
    {
        $.ajax({
            url: '/Open24Api/Business/GetAll',
            type: 'GET',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                self.listBussinessAll(result);
            },
            error: function (result) {
                exception(result);
            }
        });
    }
    GetValueBusiness();
    loadcomboboxSelectBusiness();
    //===============================
    // Load dữ liệu tree role
    //===============================
    var tree = $('#tree').tree({
        primaryKey: 'id',
        uiLibrary: 'bootstrap',
        dataSource: '/Open24Api/Business/GetAllRole',
        checkboxes: true,
    });

    //===============================
    // Lưu quyền
    //===============================
    self.SavePermisssion = function () {
        if (self.KoTenNganhNghe() === null
            || self.KoTenNganhNghe() === undefined
            || self.KoTenNganhNghe().replace(/\s+/g, '') === "") {
            AlertError("Vui lòng nhập tên ngành nghề kinh doanh");
            return;
        }

        var image = null;
        if (self.updateImage()) {
            image = $('#imageBanner').attr('src');
        }

        if (self.checkUpdateInsert() === 0) {
            $('#SHowLoad').show();
            var model = { ID: self.ID, TenNganhNghe: self.KoTenNganhNghe(), Image: image, ImageMobile: $('#imageBannerMobile').attr('src'), checkList: tree.getCheckedNodes(), Status: self.koStatus() };
            $.ajax({
                url: '/Open24Api/Business/Update',
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: model,
                success: function (result) {
                    $('#SHowLoad').hide();
                    if (result.res === true) {
                        GetValueBusiness();
                        loadcomboboxSelectBusiness();
                        AlertNotice(result.mess);
                        $('#myPermisssion').modal('hide');
                    }
                    else {
                        AlertError(result.mess);
                    }
                }
            });


        }
        else {
            $('#SHowLoad').show();
            var model_2 = { TenNganhNghe: self.KoTenNganhNghe(), Image: image, ImageMobile: $('#imageBannerMobile').attr('src'), checkList: tree.getCheckedNodes(), Status: self.koStatus() };
            $.ajax({
                url: '/Open24Api/Business/Insert',
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: model_2,
                success: function (result) {
                    $('#SHowLoad').hide();
                    if (result.res === true) {
                        GetValueBusiness();
                        AlertNotice(result.mess);
                        $('#myPermisssion').modal('hide');
                    }
                    else {
                        AlertError(result.mess);
                    }
                }
            });

        }
    };
    //===============================
    // load chi tiết tính năng từng 
    // ngành nghề
    //===============================
    $('#example tbody').on('click', 'tr', function () {
        if ($(this).attr('class') !== null
            && $(this).attr('class') !== undefined
            && $(this).attr('class').indexOf("tr-show")>-1
            && $(this).next().attr('class').indexOf("block") < 0) {
            self.NganhNgheId($(this).attr('id'));
            loadDetaiBussiness();
        }
    });
    function loadDetaiBussiness()
    {
        $.ajax({
            url: '/Open24Api/Business/GetDetailForId/' + self.NganhNgheId(),
            type: 'GET',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                if (result.res === true) {
                    self.ListBusinessDetail(result.DataSoure.Data);
                    self.PageItemDetail(result.DataSoure.PageItem);
                    self.pageCountDetail(result.DataSoure.PageCount);
                    self.pageDetail(result.DataSoure.Page);
                }
                else {
                    exception(result.mess);
                }
            },
            error: function (result) {
                exception(result);
            }
        });
    }
    //===============================
    // Cập nhật tính năng ngành nghề
    //===============================
    self.DetailUpdateBusiness = function (item) {
        self.listImage($.extend(true, [], item.AnhTinhNangNghanhNghes));
        self.TN_IsEdit(false);
        self.TN_ID(item.Id);
        self.TN_STT(item.STT);
        $('#selectNganhNghe').val(item.Id_NganhNghe);
        $('#selectNganhNghe').prop('disabled', true);
        CKEDITOR.instances['TN_NoiDung'].setData(item.NoiDung);
        self.TN_TieuDe(item.TieuDe);
        self.TN_Ten(item.TenTinhNang);
        self.TN_Status(item.Status);
        $('#myModalLabel').text("Cập nhật tính năng ngành nghề kinh doanh")
        $('#myModal2').modal('show');
        $('#SHowLoad').hide();
    }
    //===============================
    // Xóa tính năng ngành nghề
    //===============================
    self.DetaildeleteBusiness = function (model) {
        if (confirm('Bạn có chắc chắn muốn xóa tính năng này không?')) {

            $.ajax({
                url: '/Open24Api/Business/DeleteDetail',
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: model,
                success: function (result) {
                    if (result.res === true) {
                        AlertNotice(result.mess);
                        GetValueBusiness();
                        loadcomboboxSelectBusiness();
                    }
                    else {
                        AlertError(result.mess);
                    }
                }
            });

        }
    }

    //===============================
    // Thêm mới tính năng ngành nghề
    //===============================
    self.DetailInsertBussines = function () {
        self.listImage([]);
        $('#selectNganhNghe').prop('disabled', false);
        self.TN_IsEdit(true);
        CKEDITOR.instances['TN_NoiDung'].setData("");
        self.TN_TieuDe(null);
        self.TN_STT(null);
        self.TN_Ten(null);
        self.TN_Status(true);
        $('#myModalLabel').text("Thêm mới tính năng ngành nghề kinh doanh")
        $('#myModal2').modal('show');
        $('#SHowLoad').hide();
    }


    //===============================
    // Tác vụ lưu khi thêm mới hoặc sửa
    //===============================
    self.EditTinhNangNghanhNghe = function () {
        if (self.TN_Ten() === null
            || self.TN_Ten() === undefined
            || self.TN_Ten().replace(/\s+/g, '') === "") {
            AlertError("Vui lòng nhập tên tính năng ngành nghề kinh doanh.");
            return;
        }

        var model;
        var url;
        $('#SHowLoad').show();
        if(self.TN_IsEdit()){// insert
            model = {
                Name: self.TN_Ten(),
                Title: self.TN_TieuDe(),
                Note: CKEDITOR.instances['TN_NoiDung'].getData(),
                NganhNgheId: $('#selectNganhNghe').val(),
                Status: self.TN_Status(),
                Images: self.listImage(),
                STT: self.TN_STT()
            };
             url = '/Open24Api/Business/InsertDetail';
          
        }
        else {// update
            model = {
                ID: self.TN_ID(),
                Name: self.TN_Ten(),
                Title: self.TN_TieuDe(),
                Note: CKEDITOR.instances['TN_NoiDung'].getData(),
                Status: self.TN_Status(),
                Images: self.listImage(),
                STT: self.TN_STT()
            };
            url = '/Open24Api/Business/UpdateDetail';
        }
        $.ajax({
            url: url,
            type: 'POST',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: model,
            success: function (result) {
                $('#SHowLoad').hide();
                if (result.res === true) {
                    AlertNotice(result.mess);
                    loadDetaiBussiness();
                    $('#myModal2').modal('hide');
                }
                else {
                    AlertError(result.mess);
                }
            }
        });
    }

    //===============================
    // Phân trang chi tiết
    //===============================
    self.ClickPreviousDetail = function () {
        if (self.pageDetail() > 1) {
            self.pageDetail(self.pageDetail() - 1);

            FilterGridDetail()
        }
    }

    self.ClickNextDetail = function () {
        if (self.pageDetail() < self.pageCountDetail()) {
            self.pageDetail(self.pageDetail() + 1) ;
            FilterGridDetail();
        }
    }

    self.netPageDetailKeyup = function (d, e) {
        if (e.keyCode === 13) {
            if (self.pageDetail() > self.pageCountDetail()
                || self.pageCountDetail() === 1
                || !$.isNumeric(self.pageDetail())) {
                self.pageDetail(1);
            }
            FilterGridDetail();

        }
    }
    self.ChangeSelectedLimitDetail = function ()
    {

        self.pageDetail(1);
        FilterGridDetail();
    }
        //===============================
        // Tìm kiếm gridview chi tiết
        //===============================
        function FilterGridDetail() {
            $.ajax({
                url: '/Open24Api/Business/SearchGridDetail?id=' + self.NganhNgheId() + '&page=' + self.pageDetail() + '&numberpage=' + $('#SelectedLimitDetail').val(),
                type: 'GET',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (result) {
                    if (result.res === true) {
                        self.ListBusinessDetail(result.DataSoure.Data);
                        self.PageItemDetail(result.DataSoure.PageItem);
                        self.pageCountDetail(result.DataSoure.PageCount);
                        self.pageDetail(result.DataSoure.Page);
                    }
                    else {
                        alert(result.mess);
                    }
                },
                error: function (result) {
                    alert("Đã xảy ra lỗi.");
                }
            });
         }
        //===============================
        // Xóa chi tiết ảnh
        //===============================
        self.removeImage = function (item) {
            self.listImage.remove(item);
        }
    return self;
}
var bussinessModel = new bussiness();
ko.applyBindings(bussinessModel);

//===============================
// thay đổi ảnh banner
//===============================
$(document).ready(function () {
    $('#selectImage').on('click', function (e) {
        e.preventDefault();
        var finder = new CKFinder();
        finder.selectActionFunction = function (url) {
            bussiness.updateImage(true);
            $('#imageBanner').attr('src', url);
        };
        finder.popup();
    });
    $('#selectImageMobile').on('click', function (e) {
        e.preventDefault();
        var finder = new CKFinder();
        finder.selectActionFunction = function (url) {
            bussiness.updateImage(true);
            $('#imageBannerMobile').attr('src', url);
        };
        finder.popup();
    });
    $('#selectImageDetail').on('click', function (e) {
        e.preventDefault();
        var finder = new CKFinder();
        finder.selectActionFunction = function (url) {
            
            if (!bussiness.listImage().some(x => x.SrcImage.indexOf(url) > -1))
            {
                bussiness.listImage.push({ Id: null, SrcImage: url, Note:''});
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
        return "Hoạt động"
    }
    else if (value === false) {
        return "Không hoạt động"
    }
    else {
        return "";
    }
}
//===============================
// Hiện thị trạng thái
//===============================
function ConvertTrangthaiAnHien(value) {
    if (value === true) {
        return "Hiện thị"
    }
    else if (value === false) {
        return "ẩn"
    }
    else {
        return "";
    }
}