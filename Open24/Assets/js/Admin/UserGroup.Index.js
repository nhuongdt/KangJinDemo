
$(document).ready(function () {
    var NewsUserGroup = function () {
        var self = this;
        //===============================
        // Khai báo chung
        //===============================
        self.LisstUserGroup = ko.observableArray();
        self.koId = ko.observableArray();
        self.koUserGroup = ko.observableArray();
        self.koDescription = ko.observableArray();
        self.pageCount = ko.observable();
        self.PageIten = ko.observable();
        self.page = ko.observable();
        self.koSearch = ko.observable();
        self.koStatus = ko.observable(true);
        var checkInsert = false;

      

        //===============================
        // Show popup update người dùng
        //===============================
        self.btnUpdateUserGroup = function (value) {
            Loadtab(); 
            tree.uncheckAll();
            checkInsert = false;
            self.koId(value.ID);
            self.koUserGroup(value.GroupName);
            self.koDescription(value.Description);
            self.koStatus(value.Status);
            $('.modal-title').text("Cập nhật nhóm người dùng");
            $('#myModal').modal('show');
            $.ajax({
                url: '/AdminPage/News_UserGroup/GetUserRole',
                type: 'POST',
                data: ko.toJSON({ UserGroupid: value.ID }),
                contentType: 'application/json',
                success: function (result) {
                    $.each(result, function (i, item) {
                        var node = tree.getNodeById(item);
                        if (node !== null && node !== undefined) {
                            tree.check(node);
                        }
                    });
                },
                error: function () {
                    alert("Đã xảy ra lỗi.");
                }
            });
        };
        //===============================
        // Xóa nhóm người dùng
        //===============================
        self.deleteUserGroup = function (value) {
            if (confirm('Việc xóa nhóm người dùng sẽ ảnh hưởng tới các tài khoản đang sử dụng. Bạn có chắc chắn muốn xóa không?')) {
                $.ajax({
                    url: '/AdminPage/News_UserGroup/Delete',
                    type: 'POST',
                    data: ko.toJSON({ UserGroup: value }),
                    contentType: 'application/json',
                    success: function (result) {
                        if (result.res === true) {

                            AlertNotice(result.mess);
                            getAllUserGroup();
                        }
                        else {

                            AlertError(result.mess);
                        }
                    },
                    error: function () {
                        alert("Đã xảy ra lỗi.");
                    }
                });
            } else {
                return;
            }
        };

        //===============================
        // Show popup Thêm mới người dùng
        //===============================
        self.AddNewUserGroup = function () {
            Loadtab();
            tree.uncheckAll();
            checkInsert = true;
            self.koId(null);
            self.koUserGroup(null);
            self.koDescription(null);
            self.koStatus(true);
            $('.modal-title').text("Thêm mới nhóm người dùng");
            $('#myModal').modal('show');
        }

        //===============================
        // Load dữ liệu người dùng
        //===============================
        function getAllUserGroup() {
            $.ajax({
                url: '/AdminPage/News_UserGroup/GetAllUserGroup',
                type: 'GET',
                async: true,
                contentType: 'application/json',
                success: function (result) {
                    if (result.res === true) {
                        self.pageCount(result.DataSoure.PageCount);
                        self.page(result.DataSoure.Page);
                        self.LisstUserGroup(result.DataSoure.Data);
                        self.PageIten(result.DataSoure.PageItem);
                    } else
                    {
                        AlertError(result.mess);
                    }
                },
                error: function () {
                    alert("Đã xảy ra lỗi.");
                }
            });

        };
        getAllUserGroup();
        //===============================
        // Load dữ liệu tree role
        //===============================
        var tree = $('#tree').tree({
            primaryKey: 'id',
            uiLibrary: 'bootstrap',
            dataSource: '/AdminPage/News_UserGroup/GetAllRole',
            checkboxes: true,
        });


        //===============================
        // select số bản ghi trang
        //===============================
        $('#SelectedLimit').on('change', function () {
            self.page(1);
                var object = {
                    Page: self.page(),
                    Limit: $('#SelectedLimit').val(),
                    Columname: colum,
                    Sort: sort
                };

                FilterGrid(object);
        });
        
        //===============================
        // Nhập trang
        //===============================
        self.netPageKeyup = function (d, e) {
            if (e.keyCode === 13) {
                if (self.page() > self.pageCount()
                    || self.pageCount()===1
                    || !$.isNumeric(self.page())) {
                    self.page(1);
                }
                    var object = {
                        Page: self.page(),
                        Limit: $('#SelectedLimit').val(),
                        Columname: colum,
                        Sort: sort
                    };
                    FilterGrid(object);
               
            }
        }

        //===============================
        // Next trang
        //===============================
        self.ClickNext = function ()
        {
            if (self.page() < self.pageCount()){
                var object = {
                    Page: self.page() + 1,
                    Limit: $('#SelectedLimit').val(),
                    Columname: colum,
                    Sort: sort
                };
                FilterGrid(object);
            }

        }
        //===============================
        // click Tìm kiếm gridview
        //===============================
        self.SearchGrid = function (d,e)
        {
            if (e.keyCode === 13) {
                var object = {
                    Search: self.koSearch(),
                    Page: self.page(),
                    Limit: $('#SelectedLimit').val(),
                    Columname: colum,
                    Sort: sort
                };

                FilterGrid(object);
            }
        }
        //===============================
        // back trang
        //===============================
        self.ClickPrevious = function ()
        {
            if (self.page() > 1) {
                var object = {
                    Page: self.page() - 1,
                    Limit: $('#SelectedLimit').val(),
                    Columname: colum,
                    Sort: sort
                };

                FilterGrid(object);
            }
        }

        //===============================
        // Tìm kiếm gridview chung
        //===============================
        function FilterGrid(object)
        {
            $.ajax({
                url: '/AdminPage/News_UserGroup/GetDataUserGroup',
                type: 'POST',
                contentType: 'application/json',
                data: ko.toJSON({
                    daTatable: object
                }),
                success: function (result) {
                    self.page(result.Page);
                    self.pageCount(result.PageCount);
                    self.LisstUserGroup(result.Data);
                    self.PageIten(result.PageItem);
                },
                error: function () {
                    alert("Đã xảy ra lỗi.");
                }
            });
        }

        //===============================
        // Cập nhật người dùng
        //===============================
        $('#saveImageFolder').on('click', function () {
            if (self.koUserGroup() === null
                || self.koUserGroup() === undefined
                || self.koUserGroup().replace(/\s+/g, '') === "") {
                AlertError("Vui lòng nhập tên nhóm người dùng");
                return;
            }
            else if (validatespecialcharacters(self.koUserGroup())) {
                AlertError("Tên nhóm không được chứa ký tự đặc biệt.");
            }
            else {
                if (!checkInsert) {
                    var object = {
                        ID: self.koId(),
                        GroupName: self.koUserGroup(),
                        Description: self.koDescription(),
                        Status: self.koStatus(),
                    };
                    $.ajax({
                        url: '/AdminPage/News_UserGroup/Update',
                        type: 'POST',
                        data: ko.toJSON({ UserGroup: object, checkList: tree.getCheckedNodes() }),
                        contentType: 'application/json',
                        success: function (result) {
                            if (result.res === true) {
                                AlertNotice(result.mess);
                                $('#myModal').modal('hide');
                                getAllUserGroup();
                            }
                            else
                            {
                                AlertError( result.mess);
                            }
                        },
                        error: function () {
                            alert("Đã xảy ra lỗi.");
                        }
                    });

                }
                else {

                    var object = {
                        GroupName: self.koUserGroup(),
                        Description: self.koDescription(),
                        Status: self.koStatus(),
                    };
                    $.ajax({
                        url: '/AdminPage/News_UserGroup/Create',
                        type: 'POST',
                        data: ko.toJSON({ UserGroup: object, checkList: tree.getCheckedNodes() }),
                        contentType: 'application/json',
                        success: function (result) {
                            if (result.res === true) {
                                AlertNotice(result.mess);
                                $('#myModal').modal('hide');
                                getAllUserGroup();
                            }
                            else {
                                AlertError(alertDanger, result.mess);
                            }
                        },
                        error: function () {
                            alert("Đã xảy ra lỗi.");
                        }
                    });

                }
            }
        });

        //===============================
        // SortGrid
        //===============================
        var sort = 0;
        var colum = 0;

        $('#sortgroup').click(function () {
            colum = 0;
            SortGrid(this);
        });
        $('#sortDetail').click(function () {
            colum = 1;
            SortGrid(this);
        });

        $('#sortCreatdate').click(function () {
            colum = 2;
            SortGrid(this);
        });
        $('#sortCreatby').click(function () {
            colum = 3;
            SortGrid(this);
        });

        $('#sortModifydate').click(function () {
            colum = 4;
            SortGrid(this);
        });
        $('#sortModifyby').click(function () {
            colum = 5;
            SortGrid(this);
        });
        $('#sortstatus').click(function () {
            colum = 6;
            SortGrid(this);
        });
        function SortGrid(item) {
            $("#iconSort").remove();
            if (sort === 0) {
                sort = 1;
                item.innerHTML += " <i id='iconSort' class='fa fa-caret-down pull-right' aria-hidden='true'></i>";
            }
            else {
                sort = 0;
                item.innerHTML += " <i id='iconSort' class='fa fa-caret-up pull-right' aria-hidden='true'></i>";
            }
            var object = {
                Search: self.koSearch(),
                Page: self.page(),
                Limit: $('#SelectedLimit').val(),
                Columname: colum,
                Sort: sort
            };
            FilterGrid(object);
        };
    }
    ko.applyBindings(new NewsUserGroup());
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
function Loadtab()
{
    $("#tab_content1").addClass("active");
    $("#tab_content1").addClass("in");
    $("#home-tab").parent("li").addClass("active");
    $("#profile-tab").parent("li").removeClass("active");
    $("#tab_content2").removeClass("active");
}