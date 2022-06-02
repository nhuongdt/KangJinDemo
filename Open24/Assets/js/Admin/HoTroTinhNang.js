var CustomerDetail = function (Isform,id) {
    var self = this;
    //===============================
    // Khai báo chung
    //===============================
    self.listGroup = ko.observable();
    self.title = ko.observable("Thêm mới tính năng phần mềm");
    self.tentinhnang = ko.observable();
    self.video = ko.observable();
    self.trangthai = ko.observable(true);
    self.ID_cha = ko.observable();
    self.ID = ko.observable();
    self.ID_chatext = ko.observable();
    self.GhiChu = ko.observable();
    self.ViTri = ko.observable();
    self.btnDelete  = function (id) {
        if (confirm('Bạn có chắc chắn muốn xóa tính năng phần mềm không?')) {
            $.ajax({
                url: '/Open24Api/ApiHoTro/DeleteTinhNang?id=' + id,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (result) {
                    if (result.res === true) {
                        loadtree();
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
        } else {
            return;
        }
    }

    self.btnUpdate = function (id) {
        window.location.href = "/AdminPage/HoTro/EditTinhNang?id="+id;
    }
    self.AddNew = function (id) {
        window.location.href = "/AdminPage/HoTro/EditTinhNang";

    }
    
    $('.selec-person').on('click', 'ul li .text-tree', function () {
        self.ID_chatext($(this).text());
        self.ID_cha($(this).data('id'));
                                                         
    });
    function loadcombobox() {
        $.ajax({
            url: '/Open24Api/ApiHoTro/Loadtree',
            type: 'GET',
            async: true,
            dataType: 'json',
            contentType: 'application/json',
            success: function (result) {
                result.insert(0, { id: null, text: 'Nhóm cha', children:[]});
                self.listGroup(result);
              
            },
            error: function (result) {
                exception(result);
            }
        });

    }

    self.save = function () {
        var noidung = CKEDITOR.instances['noidung'].getData();
        var fileUpload = $("#imageUploadForm").get(0);
        var files = fileUpload.files;
        var fileData = new FormData();
       
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
        }
        if (self.tentinhnang() === undefined
            || self.tentinhnang() === null
            || self.tentinhnang() === "") {
            AlertError("Vui lòng nhập tên tính năng.");
        }
        else {

            $.ajax({
                data: fileData,
                url: '/Open24Api/ApiHoTro/UploadImages',
                type: 'POST',
                processData: false,
                contentType: false,
                success: function (result) {
                    if (result.res === true) {
                        var model = {
                            ID: self.ID(),
                            Ten: self.tentinhnang(),
                            TrangThai: self.trangthai(),
                            Icon: result.mess,
                            ID_Cha: self.ID_cha(),
                            NoiDung: noidung,
                            Video: self.video(),
                            ViTri: self.ViTri(),
                            GhiChu: self.GhiChu()
                        }
                        EditTinhNang(model);
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

    function EditTinhNang(model) {
        $.ajax({
            url: '/Open24Api/ApiHoTro/EditTinhNang',
            type: 'POST',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: model,
            success: function (result) {
                if (result.res === true) {
                    AlertNotice(result.mess);
                    window.location.href = "/AdminPage/HoTro/TinhNang";
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
    function addnew() {
        self.title("Thêm mới tính năng phần mềm");
        self.tentinhnang('');
        self.video('');
        self.trangthai(true);
        self.ID_cha(null);
        self.ID(null);
        self.ViTri(0);
       self.GhiChu('');
        self.ID_chatext('');
    }
    function update(model) {
        self.title("Cập nhật tính năng phần mềm");
        self.tentinhnang(model.Ten);
        self.video(model.Video);
        self.trangthai(model.TrangThai);
        self.ID_cha(model.ID_Cha);
        self.ID(model.ID);
        self.ID_chatext(model.Textcha);
        self.GhiChu(model.GhiChu);
        $('.anh-icon').find('i').hide();
        self.ViTri(model.ViTri);
        $('#blah')
            .attr('src', model.Icon)
            .width("100%")
            .height("100%").show();
        setTimeout(function () { CKEDITOR.instances['noidung'].setData(model.NoiDung); }, 1000);
       
    }
    //===============================
    // Load dữ liệu tree GroupPost
    //===============================
    function loadtree() {
        $.ajax({
            url: '/Open24Api/ApiHoTro/Loadtree',
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
    if (Isform === 1) {
        loadtree();
    }
    else {
        loadcombobox();
        if (id === null || id === undefined ||id===0)
        {
            addnew();
        }
        else {
            $.ajax({
                url: '/Open24Api/ApiHoTro/GetDefaultTinhNang?id='+id,
                type: 'GET',
                async: true,
                dataType: 'json',
                contentType: 'application/json',
                success: function (result) {
                    if (result.res === true)
                    { update(result.DataSoure); }
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
    return self;
};

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
Array.prototype.insert = function (index, item) {
    this.splice(index, 0, item);
};