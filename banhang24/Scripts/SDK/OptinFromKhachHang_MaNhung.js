var viewmodaldangky = function () {
    var self = this;
    console.log(sub, utm_user, ID_OptinForm);
    var link = window.location.href;
    //var OptinFormUri = 'https://0973474985.open24.test/api/DanhMuc/OptinFormKhachHangAPI/';
    var OptinFormUri = 'https://localhost:44382/api/DanhMuc/OptinFormKhachHangAPI/';
    //var OptinFormUri = 'https://' + sub+'.open24.vn/api/DanhMuc/OptinFormKhachHangAPI/';
    self.LoaiTruongThongTin = ko.observable(1);
    self.GoiY = ko.observable();
    self.ThongBaoThanhCongOF = ko.observable();
    self.WebDieuHuongOF = ko.observable();
    self.ButtonNameOF = ko.observable();
    self.ThongBaoHieuLucOF = ko.observable();
    self.TrangThaiThoiGianOF = ko.observable();
    self.OF_TruongThongTin = ko.observableArray();
    self.TinhThanhs = ko.observableArray();
    self.QuanHuyens = ko.observableArray();
    self.Image = ko.observable('');
    self.GioiTinh = ko.observable();
    self.LaCaNhan = ko.observable();
    self.NgaySinh = ko.observable(null);
    self.ID_TinhThanhOF = ko.observable(null);
    self.ID_QuanHuyenOF = ko.observable(null);
    self.ID_NhanVienPhuTrach = ko.observable(null);
    function getList_TruongThongTinOF() {
        $.ajax({
            url: OptinFormUri + "getList_TruongThongTinOF?SubDomain=" + sub + "&ID_OptinForm=" + ID_OptinForm + "&LoaiTruongThongTin=" + self.LoaiTruongThongTin(), cache: false,
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            success: function (data) {
                self.OF_TruongThongTin(data.LstData);
                console.log(self.OF_TruongThongTin());
                self.GoiY(self.OF_TruongThongTin()[11].GoiY);
                $('#tolTipGoiY span').text(self.GoiY());
                self.ThongBaoThanhCongOF(self.OF_TruongThongTin()[0].NoiDungThongBao);
                self.WebDieuHuongOF(self.OF_TruongThongTin()[0].WebDieuHuong);
                self.ButtonNameOF(self.OF_TruongThongTin()[0].ButtonName);
                if (self.ButtonNameOF() === '' || self.ButtonNameOF() == null) {
                    $('._op24-dk').text("Đăng ký");
                }
                else {
                    $('._op24-dk').text(self.ButtonNameOF());
                }
                self.ThongBaoHieuLucOF(self.OF_TruongThongTin()[0].NoiDungHieuLuc);
                self.TrangThaiThoiGianOF(self.OF_TruongThongTin()[0].TrangThaiThoiGian);
                if (self.OF_TruongThongTin()[0].checkSuDung != 1)
                    $('#AnhDaiDien').hide();
                else
                    $('#AnhDaiDien').show();
                if (self.OF_TruongThongTin()[1].checkSuDung != 1)
                    $('#TenKhachHang').hide();
                else
                    $('#TenKhachHang').show();
                if (self.OF_TruongThongTin()[2].checkSuDung != 1)
                    $('#GioiTinh').hide();
                else
                    $('#GioiTinh').show();
                if (self.OF_TruongThongTin()[3].checkSuDung != 1)
                    $('#NgaySinh').hide();
                else
                    $('#NgaySinh').show();
                if (self.OF_TruongThongTin()[4].checkSuDung != 1)
                    $('#SoDienThoai').hide();
                else
                    $('#SoDienThoai').show();
                if (self.OF_TruongThongTin()[5].checkSuDung != 1)
                    $('#Email').hide();
                else
                    $('#Email').show();
                if (self.OF_TruongThongTin()[6].checkSuDung != 1)
                    $('#DiaChi').hide();
                else
                    $('#DiaChi').show();
                if (self.OF_TruongThongTin()[7].checkSuDung != 1)
                    $('#TinhThanh').hide();
                else
                    $('#TinhThanh').show();
                if (self.OF_TruongThongTin()[8].checkSuDung != 1)
                    $('#QuanHuyen').hide();
                else
                    $('#QuanHuyen').show();
                if (self.OF_TruongThongTin()[9].checkSuDung != 1)
                    $('#MaSoThue').hide();
                else
                    $('#MaSoThue').show();
                if (self.OF_TruongThongTin()[10].checkSuDung != 1)
                    $('#KhachLe').hide();
                else
                    $('#KhachLe').show();
                if (self.OF_TruongThongTin()[11].checkSuDung != 1)
                    $('#NguoiGioiThieu').hide();
                else
                    $('#NguoiGioiThieu').show();
                if (self.OF_TruongThongTin()[0].checkBatBuoc != 1)
                    $('#AnhDaiDien label span').hide();
                else
                    $('#AnhDaiDien label span').show();
                if (self.OF_TruongThongTin()[1].checkBatBuoc != 1)
                    $('#TenKhachHang label span').hide();
                else
                    $('#TenKhachHang label span').show();
                if (self.OF_TruongThongTin()[2].checkBatBuoc != 1)
                    $('#GioiTinh label span').hide();
                else
                    $('#GioiTinh label span').show();
                if (self.OF_TruongThongTin()[3].checkBatBuoc != 1)
                    $('#NgaySinh label span').hide();
                else
                    $('#NgaySinh label span').show();
                if (self.OF_TruongThongTin()[4].checkBatBuoc != 1)
                    $('#SoDienThoai label span').hide();
                else
                    $('#SoDienThoai label span').show();
                if (self.OF_TruongThongTin()[5].checkBatBuoc != 1)
                    $('#Email label span').hide();
                else
                    $('#Email label span').show();
                if (self.OF_TruongThongTin()[6].checkBatBuoc != 1)
                    $('#DiaChi label span').hide();
                else
                    $('#DiaChi label span').show();
                if (self.OF_TruongThongTin()[7].checkBatBuoc != 1)
                    $('#TinhThanh label span').hide();
                else
                    $('#TinhThanh label span').show();
                if (self.OF_TruongThongTin()[8].checkBatBuoc != 1)
                    $('#QuanHuyen label span').hide();
                else
                    $('#QuanHuyen label span').show();
                if (self.OF_TruongThongTin()[9].checkBatBuoc != 1)
                    $('#MaSoThue label span').hide();
                else
                    $('#MaSoThue label span').show();
                if (self.OF_TruongThongTin()[10].checkBatBuoc != 1)
                    $('#KhachLe label span').hide();
                else
                    $('#KhachLe label span').show();
                if (self.OF_TruongThongTin()[11].checkBatBuoc != 1)
                    $('#NguoiGioiThieu label span').hide();
                else
                    $('#NguoiGioiThieu label span').show();
                if (self.OF_TruongThongTin()[11].GoiY == '')
                    $('#tolTipGoiY').hide();
                else
                    $('#tolTipGoiY').show();
            },
            complete: function (jqXHR) {
                console.log(jqXHR);
            }
        })
    };
    getList_TruongThongTinOF();
    function getListTinhThanh() {
        $.ajax({
            url: OptinFormUri + "GetListTinhThanh?SubDomain=" + sub, cache: false,
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            success: function (data) {
                if (data != null) {
                    self.TinhThanhs(data);
                    console.log(data);
                }
            },
            complete: function (jqXHR) {
                console.log(jqXHR);
            }
        })
    };
    getListTinhThanh();
    $('#hk_tinhthanh_filter').on('change', function () {
        self.ID_TinhThanhOF($(this).val());
        self.ID_QuanHuyenOF(null);
        self.getListQuanHuyen($(this).val())
    });
    $('#hk_quanhuyen_filter').on('change', function () {
        self.ID_QuanHuyenOF($(this).val());
    });
    self.getListQuanHuyen = function (id) {
        if (id !== undefined) {
            $.ajax({
                url: OptinFormUri + "GetListQuanHuyen?idTinhThanh=" + id + "&SubDomain=" + sub, cache: false,
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                },
                success: function (data) {
                    if (data != null) {
                        self.QuanHuyens(data);
                        console.log(data);
                    }
                },
                complete: function (jqXHR) {
                    console.log(jqXHR);
                }
            })
        }
    };
    function getID_NhanVienPhuTrach (MaNhanVien) {
        if (MaNhanVien !== '') {
            $.ajax({
                url: OptinFormUri + "getID_NhanVienPhuTrach?SubDomain=" + sub + "&MaNhanVien=" + MaNhanVien, cache: false,
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                },
                success: function (data) {
                    if (data != null) {
                        self.ID_NhanVienPhuTrach(data);
                        console.log(data);
                    }
                },
                complete: function (jqXHR) {
                    console.log(jqXHR);
                }
            })
        }
    };
    getID_NhanVienPhuTrach(utm_user);
    self.FilterProvince = ko.computed(function () {
        var inputSearch = $('#text_TinhThanh').val();
        var locdauInput = locdau(inputSearch);
        return $.grep(self.TinhThanhs(), function (x) {
            var itemSearch = locdau(x.TenTinhThanh);
            return itemSearch.indexOf(locdauInput) > -1;
        });
    });
    //JqAutu Quận huyện
    self.FilterProvinceQH = ko.computed(function () {
        var inputSearch = $('#text_QuanHuyen').val();
        var locdauInput = locdau(inputSearch);
        return $.grep(self.QuanHuyens(), function (x) {
            var itemSearch = locdau(x.TenQuanHuyen);
            return itemSearch.indexOf(locdauInput) > -1;
        });
    });
    $('#add_HoanThanh').on('click', function () {
        var checkbox = document.getElementsByName("gender");
        for (var i = 0; i < checkbox.length; i++) {
            if (checkbox[i].checked === true) {
                self.GioiTinh(checkbox[i].value);
            }
        }
        var checkbox1 = document.getElementsByName("gender1");
        for (var i = 0; i < checkbox1.length; i++) {
            if (checkbox1[i].checked === true) {
                self.LaCaNhan(checkbox1[i].value);
            }
        }
        callAjaxAdd_Image();
    });
   
    $('#_op24-date-ngay-sinh').on('change.dp', function (e) {
        dktime = $(this).val();
        thisDate = $(this).val();
        var t = thisDate.split(" ");
        var t1 = t[0].split("/").reverse().join("-")
        thisDate = moment(t1).format('MM/DD/YYYY')
        self.NgaySinh(moment(new Date(thisDate)).format('YYYY-MM-DD'));
        if (self.NgaySinh() === 'Invalid date')
            self.NgaySinh(null);
        console.log(self.NgaySinh());
    });
    self.add_KhachHang = function () {
        document.getElementById("add_HoanThanh").disabled = true;
        var OptinForm_DoiTuong = {
            TenDoiTuong: $('#txt_TenKhachHang').val(),
            AnhDaiDien: self.Image(),
            GioiTinh: self.GioiTinh(),
            NgaySinh: self.NgaySinh(),
            SoDienThoai: $('#txt_SoDienThoai').val(),
            Email: $('#txt_Email').val(),
            DiaChi: $('#txt_DiaChi').val(),
            ID_TinhThanh: self.ID_TinhThanhOF(),
            ID_QuanHuyen: self.ID_QuanHuyenOF(),
            NguoiGioiThieu: $('#txt_NguoiGioiThieu').val(),
            MaSoThue: $('#txt_MaSoThue').val(),
            LaCaNhan: self.LaCaNhan() == 1 ? true : false,
            ID_NhanVienPhuTrach: self.ID_NhanVienPhuTrach(),
        }
        console.log(OptinForm_DoiTuong);
        if (self.OF_TruongThongTin()[1].checkBatBuoc == 1 && OptinForm_DoiTuong.TenDoiTuong == '') {
            alert("Bạn chưa nhập tên khách hàng")
            return false;
        }
        if (self.OF_TruongThongTin()[3].checkBatBuoc == 1 && OptinForm_DoiTuong.NgaySinh == null) {
            alert("Bạn chưa nhập ngày sinh")
            return false;
        }
        if (self.OF_TruongThongTin()[4].checkBatBuoc == 1 && OptinForm_DoiTuong.SoDienThoai == '') {
            alert("Bạn chưa nhập số điện thoại")
            return false;
        }
        if (self.OF_TruongThongTin()[5].checkBatBuoc == 1 && OptinForm_DoiTuong.Email == '') {
            alert("Bạn chưa nhập Email")
            return false;
        }
        if (self.OF_TruongThongTin()[6].checkBatBuoc == 1 && OptinForm_DoiTuong.DiaChi == '') {
            alert("Bạn chưa nhập địa chỉ")
            return false;
        }
        if (self.OF_TruongThongTin()[7].checkBatBuoc == 1 && OptinForm_DoiTuong.ID_TinhThanh == null) {
            alert("Bạn chưa chọn tỉnh thành")
            return false;
        }
        if (self.OF_TruongThongTin()[8].checkBatBuoc == 1 && OptinForm_DoiTuong.ID_QuanHuyen == null) {
            alert("Bạn chưa chọn quận huyện")
            return false;
        }
        if (self.OF_TruongThongTin()[9].checkBatBuoc == 1 && OptinForm_DoiTuong.MaSoThue == '') {
            alert("Bạn chưa nhập mã số thuế")
            return false;
        }
        if (self.OF_TruongThongTin()[11].checkBatBuoc == 1 && OptinForm_DoiTuong.NguoiGioiThieu == '') {
            alert("Bạn chưa nhập người giới thiệu")
            return false;
        }
        if (OptinForm_DoiTuong.Email != '' && OptinForm_DoiTuong.Email !== undefined && OptinForm_DoiTuong.Email !== null) {
            var re = /^(([^<>()[\]\\.,;:\s@#ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴ\"]+(\.[^<>()[\]\\.,;:\s@#ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴ\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/i;
            var valReturn = re.test(OptinForm_DoiTuong.Email);
            if (valReturn === false) {
                alert("Email không hợp lệ")
                return false;
            }
        }
        var myData = {};
        myData.optinForm_DoiTuong = OptinForm_DoiTuong;
        callAjaxAdd(myData);
    }
    function callAjaxAdd(myData) {
        $.ajax({
            data: myData,
            url: OptinFormUri + "InsertOF_KhachHang?SubDomain=" + sub + "&ID_OptinForm=" + ID_OptinForm + "&Link=" + link,
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (item) {
                alert(self.ThongBaoThanhCongOF())
                //bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + self.ThongBaoThanhCongOF(), "success");
                document.getElementById("add_HoanThanh").disabled = false;
                //Điều hướng
                //location.href = "/#/OptinForm";
            },
            statusCode: {
                404: function (item) {
                    self.error("page not found");
                    document.getElementById("add_HoanThanh").disabled = false;
                },
                500: function (item) {
                    document.getElementById("add_HoanThanh").disabled = false;
                    alert('Đăng ký thông tin khách hàng không thành công');
                    //bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Đăng ký thông tin khách hàng không thành công", "danger");
                }
            },
            complete: function (item) {
                document.getElementById("add_HoanThanh").disabled = false;

            }
        })
    }
    function callAjaxAdd_Image() {
        var formDataTR = new FormData();
        var totalFilesTR = document.getElementById("_op24_imageUploadForm").files.length;
        for (var i = 0; i < totalFilesTR; i++) {
            var file = document.getElementById("_op24_imageUploadForm").files[i];
            formDataTR.append("_op24_imageUploadForm", file);
        }
        if (self.OF_TruongThongTin()[0].checkBatBuoc == 1 && totalFilesTR < 1) {
            alert("Bạn chưa nhập ảnh đại diện")
            return false;
        }
        if (totalFilesTR > 0) {
            $.ajax({
                type: "POST",
                url: OptinFormUri + "UploadImageStaff?SubDomain=" + sub,
                data: formDataTR,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (response) {
                    self.Image(response.mess);
                    self.add_KhachHang();
                },
            });
        }
        else {
            self.Image("");
            self.add_KhachHang();
        }
    }
    return self;
}
function ajaxHelper(uri, method = "Get", data) {

    return $.ajax({
        type: method,
        url: uri,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: data ? JSON.stringify(data) : null,
        statusCode: {
            404: function () {
            },
        }
    })
}
function locdau(obj) {
    if (!obj)
        return "";
    var str = obj;
    str = str.toLowerCase();
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/^\-+|\-+$/g, "");
    return str;
};
    ko.applyBindings(new viewmodaldangky());
