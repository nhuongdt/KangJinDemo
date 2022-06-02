var CustomerDetail=function() {
    var self = this;
    //===============================
    // Khai báo chung
    //===============================
    self.ListCbbGroupPost = ko.observable();
    self.koId = ko.observable();
    self.koDescription = ko.observable();
    self.koStatus = ko.observable(true);
    self.koPostName = ko.observable();
    self.koInsert = ko.observable();
    self.CreateDate = ko.observable();
    self.CreateBy = ko.observable();
    self.listGroup = ko.observableArray();

    //===============================
    // Click button thêm mới
    //===============================
    self.AddNewPostGroup = function () {
        LoadForm(0);
        self.koInsert(true);
        self.koId(null);
        self.koPostName("");
        self.koDescription("");
        $('#SelectParentId').val(0);
        self.koStatus(true);
        $('.modal-title').text("Thêm mới nhóm bài viết");
        $('#myModal').modal('show');
    }

    //===============================
    //  update load thông tin
    //===============================
    self.UpdatePostGroup = function (id) {
        LoadForm(id);
        $.ajax({
            url: "/Open24Api/ApiGroupPost/GetByGroupPost?GroupId=" + id, 
            type: 'GET',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                if (result.res === true) {
                    self.koInsert(false);
                    self.koId(result.DataSoure.ID);
                    self.koPostName(result.DataSoure.Name);
                    self.koDescription(result.DataSoure.Description);
                    self.koStatus(result.DataSoure.Status);
                    self.CreateBy(result.DataSoure.CreateBy);
                    self.CreateDate(result.DataSoure.CreateDate);
                    $('#SelectParentId').val(result.DataSoure.ParentID);
                    $('.modal-title').text("Cập nhật nhóm bài viết");
                    $('#myModal').modal('show');
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
    // Xóa nhóm bài viêt
    //===============================
    self.deletePostGroup = function (id) {
        if (confirm('Bạn có chắc chắn muốn xóa không?')) {
            var model = { ID: id };
            $.ajax({
                data: model,
                url: "/Open24Api/ApiGroupPost/DeletePostGroup",
                type: 'POST',
                async: true,
             
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (result) {
                    if (result.res === true) {
                        AlertNotice(result.mess);
                        loadtree();
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
    //===============================
    // Cập nhật thông tin nhóm bài viết
    //===============================
    self.SaveUpdatepost = function () {
        var name = self.koPostName();
        if (name === null || name === undefined || name.replace(/\s+/g, '') === "") {
            AlertError( "Vui lòng nhập tên nhóm bài viết.");
            return false;
        }
        else if (self.koInsert()) {
            InsertPostGroup();
        }
        else if (!self.koInsert()) {
            UpdatePostgroup();
        }
    }
    function InsertPostGroup() {
        var model = {
            Name: self.koPostName(),
            ParentID: $('#SelectParentId').val(),
            Description: self.koDescription(),
            Status: self.koStatus()
        }
        $.ajax({
            data: model,
            url: "/Open24Api/ApiGroupPost/InsertPostGroup",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                if (result.res === true) {
                    AlertNotice(result.mess);
                    loadtree();
                    $('#myModal').modal('hide');
                } else {
                    AlertError(result.mess);
                }
            },
            error: function (result) {
                exception(result);
            }
        });
    }

    function UpdatePostgroup() {
        var model = {
            ID: self.koId(),
            Name: self.koPostName(),
            ParentID: $('#SelectParentId').val(),
            Description: self.koDescription(),
            Status: self.koStatus(),
            CreateDate: self.CreateDate(),
            CreateBy: self.CreateBy()
        }
        $.ajax({
            data: model,
            url: "/Open24Api/ApiGroupPost/UpdatePostGroup",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                if (result.res === true) {
                    AlertNotice(result.mess);
                    loadtree();
                    $('#myModal').modal('hide');
                } else {
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
    function LoadForm(valueId) {
        $.ajax({
            url: "/Open24Api/ApiGroupPost/LoadAllCombobox?GroupId=" + valueId,
            type: 'GET',
            async: true,
            dataType: 'json',
            contentType: 'application/json',
            success: function (result) {
                if (result.res === true) {
                    self.ListCbbGroupPost(result.DataSoure);
                }
                else {
                    alert(result.mess);
                }
            },
            error: function (result) {
                exception(result);
            }
        });

        $("#iconSort").remove();

    }

    LoadForm(0);

    //===============================
    // Load dữ liệu tree GroupPost
    //===============================
    function loadtree() {
        $.ajax({
            url: '/Open24Api/ApiGroupPost/Loadtree',
            type: 'GET',
            async: true,
            dataType: 'json',
            contentType: 'application/json',
            success: function (result) {
                self.listGroup(result);
                $('#tree').treeview({
                    expandIcon: 'glyphicon glyphicon-chevron-right',
                    collapseIcon: 'glyphicon glyphicon-chevron-down',
                    selectedBackColor: "#2eb1d0",
                    data: result,
                    showTags: true,
                    highlightSelected: true,
                });
            },
            error: function (result) {
                exception(result);
            }
        });

    }

    loadtree();
    return self;
};
var seltknockout = new CustomerDetail();
ko.applyBindings(seltknockout);

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
// click Update
//===============================
function btnUpdateGroupPost(id) {
    if (id === null || id === '') {
        AlertError("Không lấy được nhóm cần sửa");
    }
    else {
        seltknockout.UpdatePostGroup(id);
    }
}
//===============================
// click xóa nhóm
//===============================
function btnDeleteGrouppost(id) {
    if (id === null || id === '') {
        AlertError("Không lấy được nhóm cần xóa");
    }
    else {
        seltknockout.deletePostGroup(id);
        
    }
}