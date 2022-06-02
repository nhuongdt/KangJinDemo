var NewsUser = function () {
    var self = this;
    //===============================
    // Khai báo chung
    //===============================
    self.koId = ko.observableArray();
    self.koBirthDay = ko.observable();
    self.UserGroupList = ko.observableArray();
    self.LisstUser = ko.observableArray();
    self.koUserName = ko.observable();
    self.koPassword = ko.observable();
    self.koName = ko.observable();
    self.koAddress = ko.observable();
    self.koEmail = ko.observable();
    self.koPhone = ko.observable();
    self.koStatus = ko.observable(true);


    self.pageCount = ko.observable();
    self.PageIten = ko.observable();
    self.page = ko.observable(); 
    self.koSearch = ko.observable();
    self.sort = ko.observable();
    self.colum = ko.observable();
    //===============================
    // Link thêm mới nhóm người dùng
    //===============================
    self.ShowUserGroup = function ()
    {

    }
    //===============================
    // Show popup update người dùng
    //===============================
    self.btnUpdateUser = function (value) {
        self.koId(value.ID);
        self.koUserName(value.UserName);
        self.koPassword = ko.observable();
        self.koName(value.Name);
        self.koAddress(value.Address);
        self.koEmail(value.Email);
        self.koPhone(value.Phone);
        self.koStatus(value.Status);
        var $datepicker = $('#datetimepicker');
        $datepicker.datetimepicker();
        $datepicker.datetimepicker({ dateFormat: 'MM/DD/YYYY' });
        if (value.BirthDay !== undefined
            && value.BirthDay !== null
            && value.BirthDay.replace(/\s+/g, '') !== "") {
            var date = moment(value.BirthDay).format('MM/DD/YYYY');

            $datepicker.val(date);
        }
        $('#selCategoryNews').val(value.GroupID);
        $('#myModal').modal('show');
    };
     //===============================
    // Cập nhật người dùng
    //===============================
    self.SaveUpdateUser = function () {
        if (self.koName() === undefined
            || self.koName() === null
            || self.koName().replace(/\s+/g, '') === "") {

            AlertError("Vui lòng nhập họ tên.");
        }
        else if (self.koPassword() !== undefined
            && self.koPassword().replace(/\s+/g, '') !== ""
            && self.koPassword().replace(/\s+/g, '').length < 6) {
            AlertError("Mật khẩu chứa ít nhất 6 ký tự.");
        }
        else if (!validateEmail(self.koEmail())) {
            AlertError("Địa chỉ Email không hợp lệ.");
        }
        else if (!validatePhone(self.koPhone())) {
            AlertError("Số điện thoại không hợp lệ.");
        }
        else if (validatespecialcharacters(self.koUserName())) {
            AlertError("Tài khoản không được chứa ký tự đặc biệt.");
        }
        else if (validatespecialcharacters(self.koName())) {
            AlertError("Họ tên không được chứa ký tự đặc biệt.");
        }
        else {
            var object = {
                ID: self.koId(),
                Password: self.koPassword(),
                Name: self.koName(),
                BirthDay: $('#datetimepicker').val(),
                GroupID: $('#selCategoryNews').val(),
                Address: self.koAddress(),
                Email: self.koEmail(),
                Phone: self.koPhone(),
                Status: self.koStatus(),
            }
            $.ajax({
                url: '/AdminPage/News_User/UpdateUser',
                type: 'POST',
                data: ko.toJSON({ model: object }),
                contentType: 'application/json',
                success: function (result) {
                    if (result.res === true) {
                        AlertNotice(result.mess);
                        $('#myModal').modal('hide');
                        getAllArticle();
                    }
                    else
                    {
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
    // Xóa người dùng
    //===============================
    self.deleteUser = function (value) {
        if (confirm('Bạn có chắc chắn muốn xóa không?')) {
            $.ajax({
                url: '/AdminPage/News_User/DeleteUser',
                type: 'POST',
                data: ko.toJSON({ user: value }),
                contentType: 'application/json',
                success: function (result) {
                    if (result.res === true) {
                        AlertNotice(result.mess);
                        getAllArticle();
                    }
                    else {
                        AlertError(result.mess)
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
    // link sang màn hình thêm mới
    //===============================
    self.AddNewUser = function () {
        location.href = "/AdminPage/News_User/Create";
    }

    //===============================
    // Load dữ liệu người dùng
    //===============================
    function getAllArticle() {
        $.ajax({
            url: '/AdminPage/News_User/GetAll',
            type: 'POST',
            async: true,
            contentType: 'application/json',
            success: function (result) {
                if (result.res === true) {
                    self.pageCount(result.DataSoure.PageCount);
                    self.page(result.DataSoure.Page);
                    self.LisstUser(result.DataSoure.Data);
                    self.PageIten(result.DataSoure.PageItem);
                }
                else
                {
                    AlertError(result.mess);
                }
            },
            error: function () {
                alert("Đã xảy ra lỗi.");
            }
        });
        $("#iconSort").remove();
    };
    getAllArticle();
    //===============================
    // Load dữ liệu nhóm người dùng
    //===============================
    function GetGroupUser() {
        $.ajax({
            url: '/AdminPage/News_User/GetGroupUser',
            type: 'GET',
            async: true,
            contentType: 'application/json',
            success: function (result) {
                if (result.res === true) {
                    self.UserGroupList(result.DataSoure);
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
    GetGroupUser();

    //===============================
    // select số bản ghi trang
    //===============================
    $('#SelectedLimit').on('change', function () {
             self.page(1);
            FilterGrid();


    });

    //===============================
    // Nhập trang
    //===============================
    self.netPageKeyup = function (d, e) {
        if (e.keyCode === 13) {
            if (self.page() > self.pageCount()
                || self.pageCount() === 1
                || !$.isNumeric(self.page())) {
                self.page(1);
            }
            FilterGrid();

        }
    }

    //===============================
    // Next trang
    //===============================
    self.ClickNext = function () {
        if (self.page() < self.pageCount()) {
            self.page(self.page() + 1);
            FilterGrid();
        }

    }
    //===============================
    // click Tìm kiếm gridview
    //===============================
    self.SearchGrid = function (d, e) {
        if (e.keyCode === 13) {

            FilterGrid();
        }
    }
    //===============================
    // back trang
    //===============================
    self.ClickPrevious = function () {
        if (self.page() > 1) {
            self.page(self.page() - 1);

            FilterGrid();
        }
    }

    //===============================
    // Tìm kiếm gridview chung
    //===============================
    function FilterGrid() {
        var object = {
            Search: self.koSearch(),
            Page: self.page(),
            Limit: $('#SelectedLimit').val(),
            Columname: self.colum(),
            Sort: self.sort()
        };
        $.ajax({
            url: '/AdminPage/News_User/GetDataForShearchUser',
            type: 'POST',
            contentType: 'application/json',
            data: ko.toJSON({
                daTatable: object
            }),
            success: function (result) {
                if (result.res === true) {
                    self.page(result.DataSoure.Page);
                    self.pageCount(result.DataSoure.PageCount);
                    self.LisstUser(result.DataSoure.Data);
                    self.PageIten(result.DataSoure.PageItem);
                }
                else
                {
                    AlertError(result.mess);
                }
            },
            error: function () {
                alert("Đã xảy ra lỗi.");
            }
        });
    }
    //===============================
    // SortGrid
    //===============================

    $('#sortName').click(function () {
        self.colum(0);
        SortGrid(this);
    });

    $('#sortUsename').click(function () {
        self.colum(1);
        SortGrid(this);
    });
    $('#sortBir').click(function () {
        self.colum(2);
        SortGrid(this);
    });

    $('#sortAdress').click(function () {
        self.colum(3);
        SortGrid(this);
    });
    $('#sortemail').click(function () {
        self.colum(4);
        SortGrid(this);
    });
    $('#sortPhone').click(function () {
        self.colum(5);
        SortGrid(this);
    });
    $('#sortCearedate').click(function () {
        self.colum(6);
        SortGrid(this);
    });
    $('#sortCreateBy').click(function () {
        self.colum(7);
        SortGrid(this);
    });
    $('#sortStatus').click(function () {
        self.colum(8);
        SortGrid(this);
    });
    function SortGrid(item) {
        $("#iconSort").remove();
        if (self.sort() === 0) {
            self.sort(1);
            item.innerHTML += " <i id='iconSort' class='fa fa-caret-down pull-right' aria-hidden='true'></i>";
        }
        else {
            self.sort(0);
            item.innerHTML += " <i id='iconSort' class='fa fa-caret-up pull-right' aria-hidden='true'></i>";
        }
        FilterGrid();
    };
}
ko.applyBindings(new NewsUser());


//===============================
// Hiện thị Datetime
//===============================
function ConvertDate(config) {
    if (config === undefined
        || config === null
        || config.replace(/\s+/g, '') === "")
    {
        return "";
    }
    else {
        var a = moment(config).format('DD/MM/YYYY');
        return a;
    }
}
