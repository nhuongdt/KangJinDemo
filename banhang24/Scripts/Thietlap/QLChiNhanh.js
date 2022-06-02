var FormModel_ChiNhanh = function () {
    var self = this;
    self.TenDonVi = ko.observable();
    self.DiaChi = ko.observable();
    self.ID = ko.observable();
    self.SoDienThoai = ko.observable();
    self.MaDonVi = ko.observable();
    self.setdata = function (item) {
        self.TenDonVi(item.TenDonVi);
        self.DiaChi(item.DiaChi);
        self.ID(item.ID);
        self.SoDienThoai(item.SoDienThoai);
        self.MaDonVi(item.MaDonVi);
    }
};
var ViewModel = function () {
    var self = this;
    self.Loc_HoatDong = ko.observable("1");
    var DMDonVis = '/api/DanhMuc/DM_DonViAPI/';
    var _txtTenTaiKhoan = $('#txtTenTaiKhoan').text();
    var _IDNhomNguoiDung = $('.idnhomnguoidung').text();
    var _IDNguoiDung = $('.idnguoidung').text();
    var _IDNhanVien = $('.idnhanvien').text();
    var _IDchinhanh = $('#hd_IDdDonVi').val();
    self.error = ko.observable();
    self.deleteID = ko.observable();
    self.filter = ko.observable();
    self.booleanAdd = ko.observable(true);
    self.DonVis = ko.observableArray();
    self.IPAddress = ko.observable(false);
    $.getJSON('https://jsonip.com/?callback=?', function (data) {
        self.IPAddress = data.ip;
        if (data.ip === '123.24.206.173') {
            self.IPAddress = true;
        }
    });
    //
    self.newDonVi = ko.observable(new FormModel_ChiNhanh);

    function ajaxHelper(uri, method, data) {
        self.error(''); // Clear error message
        return $.ajax({
            type: method,
            url: uri,
            dataType: 'json',
            contentType: 'application/json',
            data: data ? JSON.stringify(data) : null,
            statusCode: {
                404: function () {
                    self.error("Page not found");
                },
            }
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
            });
    }
    //load quyền
    function loadQuyenIndex() {
        var arrQuyen = [];
        ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetHT_NhomNguoiDung?idnguoidung=" + _IDNguoiDung + '&iddonvi=' + _IDchinhanh, 'GET').done(function (data) {
            if (data.HT_Quyen_NhomDTO.length > 0) {
                for (var i = 0; i < data.HT_Quyen_NhomDTO.length; i++) {
                    arrQuyen.push(data.HT_Quyen_NhomDTO[i].MaQuyen);
                }
            }
            localStorage.setItem('lc_CTQuyen', JSON.stringify(arrQuyen));
        });
        ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/' + 'GetCauHinhHeThong/' + _IDchinhanh, 'GET').done(function (data) {
            localStorage.setItem('lc_CTThietLap', JSON.stringify(data));
        });
    }
    loadQuyenIndex();


    function getAllDonVi() {
        $('.table_h').gridLoader();
        var txtCN = self.filter();

        if (txtCN === undefined) {
            txtCN = "";
        }
        ajaxHelper(DMDonVis + "GetListDonVi?hoatdong=" + self.Loc_HoatDong() + '&txtCN=' + txtCN, 'GET').done(function (data) {
            self.DonVis([]);
            if (data === null) {
                self.DonVis([]);
            } else {
                self.DonVis(data);
            }
            $('.table_h').gridLoader({ show: false });
        });
        LoadHtmlGrid();
    }
    getAllDonVi();


    self.modalDelete = function (item) {
        self.deleteID(item.ID);
        self.TenChiNhanhCu(item.TenDonVi);
        $('#modalpopup_deleteCN').modal('show');
    };
    self.xoaCN = function (DonVis) {
        if (DonVis.deleteID() === 'd93b17ea-89b9-4ecf-b242-d03b8cde71de') {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Không thể xóa chi nhánh khởi tạo ban đầu", "danger");
            return false;
        }
        ajaxHelper(DMDonVis + 'DeleteDM_DonVi?id=' + DonVis.deleteID(), 'GET').done(function (data) {
            if (data === "") {
                self.XoaLSChiNhanh();
                getAllDonVi();
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật hệ thống chi nhánh thành công", "success");
            }
            else {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + data, "danger");
            }
        })
    };

    self.resetChiNhanh = function () {
        self.newDonVi(new FormModel_ChiNhanh());
    }

    self.showPopupAddCN = function () {
        ajaxHelper(DMDonVis + 'GioiHanSoChiNhanh', 'GET').done(function (data) {
            if (data) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cửa hàng đã đạt số chi nhánh quy định, không thể thêm mới", "danger");
            }
            else {
                self.booleanAdd(true);
                self.resetChiNhanh();
                $('#modalPopuplg_ChiNhanh').modal('show');
                $('#modalPopuplg_ChiNhanh').on('shown.bs.modal', function () {
                    $('#txtTenDonVi').focus();
                });
            }
        });
    }

    self.TenChiNhanhCu = ko.observable();

    self.editCN = function (item) {
        self.TenChiNhanhCu(item.TenDonVi);
        self.newDonVi().setdata(item);
        self.booleanAdd(false);
        $('#modalPopuplg_ChiNhanh').modal('show');
        $('#modalPopuplg_ChiNhanh').on('shown.bs.modal', function () {
            $('#txtTenDonVi').focus().select();
        })
    }

    self.addChiNhanh = function (formElement) {
        var _tenDonVi = self.newDonVi().TenDonVi();
        var _diaChi = self.newDonVi().DiaChi();
        var _id = self.newDonVi().ID();
        var _soDienThoai = self.newDonVi().SoDienThoai();
        var _maDonVi = self.newDonVi().MaDonVi();
        if (_maDonVi === '' || _maDonVi === undefined) {
            ShowMessage_Danger('Vui lòng nhập mã chi nhánh');
            $('#txtMaDonVi').select();
            return false;
        }
        if (_tenDonVi === null || _tenDonVi === "" || _tenDonVi === "undefined" || _tenDonVi === undefined) {
            ShowMessage_Danger('Vui lòng tên chi nhánh');
            $('#txtTenDonVi').select();
            return false;
        }
        var DM_DonVi = {
            ID: _id,
            TenDonVi: _tenDonVi,
            DiaChi: _diaChi,
            SoDienThoai: _soDienThoai,
            MaDonVi: _maDonVi,
            NguoiTao: _txtTenTaiKhoan
        };
        if (self.booleanAdd() === true) {
            ajaxHelper(DMDonVis + 'GioiHanSoChiNhanh', 'GET').done(function (data) {
                if (data) {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cửa hàng đã đạt số chi nhánh quy định, không thể thêm mới", "danger");
                }
                var myData = {};
                myData.objNewDonVi = DM_DonVi;
                myData.iddonvi = _IDchinhanh;
                //return false;
                $.ajax({
                    url: DMDonVis + "PostDM_DonVi?idnhanvien=" + _IDNhanVien,
                    type: 'POST',
                    dataType: 'json',
                    data: myData,
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (item) {
                        self.DonVis.push(item);
                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Thêm chi nhánh thành công", "success");
                        getAllDonVi();
                        $('#modalPopuplg_ChiNhanh').modal('hide');
                    },
                    statusCode: {
                        404: function () {
                            self.error("page not found");
                        },
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                        $('#modalPopuplg_ChiNhanh').modal('hide');
                    },
                    complete: function () {
                    }
                })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                    });
            });
        }
        else {
            var myData1 = {};
            myData1.id = _id;
            myData1.objNewDonVi = DM_DonVi;

            $.ajax({
                url: DMDonVis + "PutDM_DonVi",
                type: 'PUT',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: myData1,
                success: function () {
                    getAllDonVi();
                    self.UpdateLSChiNhanh(_tenDonVi)
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật chi nhánh thành công", "success");
                    $('#modalPopuplg_ChiNhanh').modal('hide');
                },
                statusCode: {
                    404: function () {
                        self.error("page not found");
                    },
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                },
                complete: function () {
                }
            })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                });
        }
    }

    self.UpdateLSChiNhanh = function (tendonvinew) {
        var objDiary = {
            ID_NhanVien: _IDNhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Quản lý chi nhánh",
            NoiDung: "Cập nhật chi nhánh : " + self.TenChiNhanhCu() + " thành: " + tendonvinew,
            NoiDungChiTiet: "Cập nhật chi nhánh : " + self.TenChiNhanhCu() + " thành: " + tendonvinew,
            LoaiNhatKy: 2 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        var myData = {};
        myData.objDiary = objDiary;
        $.ajax({
            url: '/api/DanhMuc/SaveDiary/' + "post_NhatKySuDung",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: myData,
            success: function (item) {
            },
            statusCode: {
                404: function () {
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ghi nhật ký sử dụng thất bại", "danger");
            },
            complete: function () {

            }
        })
    }
    self.XoaLSChiNhanh = function () {
        var objDiary = {
            ID_NhanVien: _IDNhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Quản lý chi nhánh",
            NoiDung: "Xóa chi nhánh : " + self.TenChiNhanhCu(),
            NoiDungChiTiet: "Xóa chi nhánh : " + self.TenChiNhanhCu(),
            LoaiNhatKy: 3 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        var myData = {};
        myData.objDiary = objDiary;
        $.ajax({
            url: '/api/DanhMuc/SaveDiary/' + "post_NhatKySuDung",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: myData,
            success: function (item) {
            },
            statusCode: {
                404: function () {
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ghi nhật ký sử dụng thất bại", "danger");
            },
            complete: function () {

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

        // Some system encode vietnamese combining accent as individual utf-8 characters
        str = str.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, ""); // Huyền sắc hỏi ngã nặng 
        str = str.replace(/\u02C6|\u0306|\u031B/g, ""); // Â, Ê, Ă, Ơ, Ư

        return str;
    }

    //phân trang
    self.pageSizes = [10, 20, 30, 40, 50];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.currentPage = ko.observable(0);
    self.fromitem = ko.observable(1);
    self.toitem = ko.observable();
    self.arrPagging = ko.observableArray();

    self.PageCount = ko.computed(function () {
        if (self.pageSize()) {
            if (self.arrPagging() !== null) {
                return Math.ceil(self.arrPagging().length / self.pageSize());
            }
            else {
                return 0;
            }
        }
        else {
            return 1;
        }
    });

    self.PageResults = ko.computed(function () {
        if (self.DonVis() !== null) {

            self.fromitem((self.currentPage() * self.pageSize()) + 1);

            if (((self.currentPage() + 1) * self.pageSize()) > self.DonVis().length) {
                var fromItem = (self.currentPage() + 1) * self.pageSize();
                if (fromItem < self.DonVis().length) {
                    self.toitem((self.currentPage() + 1) * self.pageSize());
                }
                else {
                    self.toitem(self.DonVis().length);
                }
            } else {
                self.toitem((self.currentPage() * self.pageSize()) + self.pageSize());
            }
        }
    });
    //lọc vi trí
    self.filteredDM_DonVi = ko.computed(function () {
        LoadHtmlGrid();
        var first = self.currentPage() * self.pageSize();
        var _filter = self.filter();
        var _hoatdong = self.Loc_HoatDong();
        var arrFilter = null;
        if (self.DonVis() !== null) {
            arrFilter = ko.utils.arrayFilter(self.DonVis(), function (prod) {
                var chon = true;
                if (_hoatdong === "1") {
                    chon = prod.TrangThai === true;
                }
                if (_hoatdong === "2") {
                    chon = prod.TrangThai === false;
                } else {
                    chon = true;
                }

                var arr = locdau(prod.TenDonVi).toLowerCase().split(/\s+/);
                var sSearch = '';

                for (var i = 0; i < arr.length; i++) {
                    sSearch += arr[i].toString().split('')[0];
                }
                if (chon && _filter) {
                    chon = (locdau(prod.TenDonVi).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
                        sSearch.indexOf(locdau(_filter).toLowerCase()) >= 0
                    );
                }
                return chon;
            });
        }
        if (arrFilter !== null) {
            self.arrPagging(arrFilter);

            self.fromitem((self.currentPage() * self.pageSize()) + 1);

            if (((self.currentPage() + 1) * self.pageSize()) > arrFilter.length) {
                self.toitem(arrFilter.length)
            } else {
                self.toitem((self.currentPage() * self.pageSize()) + self.pageSize());
            }

            if (self.PageCount() > 1) {
                if (self.currentPage() === self.PageCount()) {
                    self.currentPage(0);
                }

                return arrFilter.slice(self.currentPage() * self.pageSize(), (self.currentPage() * self.pageSize()) + self.pageSize());
            }
            else {
                return arrFilter;
            }
        }
    });

    self.Loc_HoatDong.subscribe(function (newValue) {
        getAllDonVi();
    });

    self.NgungHoatDong = function (item) {
        if (!commonStatisJs.CheckNull(item.ID) && item.ID.toLowerCase() === 'd93b17ea-89b9-4ecf-b242-d03b8cde71de') {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Không thể ngừng hoạt động chi nhánh khởi tạo ban đầu", "danger");
            return false;
        }
        else {
            $.ajax({
                type: "DELETE",
                url: DMDonVis + "NgungHoatDong?id=" + item.ID + '&idnhanvien=' + _IDNhanVien + '&iddonvi=' + _IDchinhanh,
                dataType: 'json',
                contentType: 'application/json',
                success: function (result) {
                    getAllDonVi();
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật chi nhánh thành công!", "success");
                },
                error: function (error) {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cập nhật chi nhánh thất bại!", "danger");
                }
            });
        }
    }

    self.ChoPhepHoatDong = function (item) {
        if (self.IPAddress) {
            $.ajax({
                type: "DELETE",
                url: DMDonVis + "ChoHoatDong?id=" + item.ID + '&idnhanvien=' + _IDNhanVien + '&iddonvi=' + item.ID,
                dataType: 'json',
                contentType: 'application/json',
                success: function (result) {
                    getAllDonVi();
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật chi nhánh thành công!", "success");
                },
                error: function (error) {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cập nhật chi nhánh thất bại!", "danger");
                }
            });
        }
        else {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cập nhật chi nhánh thất bại!", "danger");
        }
    }

    self.PageList = ko.computed(function () {
        if (self.PageCount() > 1) {
            return Array.apply(null, {
                length: self.PageCount()
            }).map(Number.call, Number);
        }
    });

    self.ResetCurrentPage = function () {
        self.currentPage(0);
    };

    self.GoToPage = function (page) {
        self.currentPage(page);
    };

    self.GetClass = function (page) {
        return (page === self.currentPage()) ? "click" : "";
    };

    self.NDDonVi = ko.observableArray();
    self.getNDbyID_DonVi = function (item) {
        self.NDDonVi([]);
        ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + 'getallNDByID_DonVi/' + item.ID, 'GET').done(function (data) {
            self.NDDonVi(data);
        });


        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if (item.TrangThai === false) {
            if ($.inArray('ChiNhanh_CapNhat', lc_CTQuyen) > -1 && self.IPAddress) {
                $('.clickChoHDCN').show();
            }
            else {
                $('.clickChoHDCN').hide();
            }
        }
        else {
            if ($.inArray('ChiNhanh_CapNhat', lc_CTQuyen) > -1) {
                $('.clickNgungHDCN').show();
            }
            else {
                $('.clickNgungHDCN').hide();
            }
        }
        if ($.inArray('ChiNhanh_CapNhat', lc_CTQuyen) > -1) {
            $('.editCN').show();
        }
        if ($.inArray('ChiNhanh_Xoa', lc_CTQuyen) > -1) {
            $('.xoaCN').show();
        }

    }

    self.clickFindND = function (item) {
        localStorage.setItem('findTK', item.TaiKhoan);
        //window.location.href = "/#/SetUpUsers/";
        clickloadForm('SetUpUsers')
    }
    //===============================
    // Load lai form lưu cache bộ lọc 
    // trên grid 
    //===============================
    function LoadHtmlGrid() {
        if (window.localStorage) {
            var current = localStorage.getItem('QLchinhanh');
            if (!current) {
                current = [];
                localStorage.setItem('QLchinhanh', JSON.stringify(current));
            } else {
                current = JSON.parse(current);
                for (var i = 0; i < current.length; i++) {
                    $(current[i].NameClass).addClass("operation");
                    document.getElementById(current[i].NameId).checked = false;

                }
            }
        }
    }
    //===============================
    // Add Các tham số cần lưu lại để 
    // cache khi load lại form
    //===============================
    function addClass(name, id, value) {

        var current = localStorage.getItem('QLchinhanh');
        if (!current) {
            current = [];
        } else {
            current = JSON.parse(current);
        }
        if (current.length > 0) {
            for (var i = 0; i < current.length; i++) {
                if (current[i].NameId === id.toString()) {
                    current.splice(i, 1);
                    break;
                }
                if (i === current.length - 1) {
                    current.push({
                        NameClass: name,
                        NameId: id,
                        Value: value
                    });
                    break;
                }
            }
        }
        else {
            current.push({
                NameClass: name,
                NameId: id,
                Value: value
            });
        }
        localStorage.setItem('QLchinhanh', JSON.stringify(current));
    }
    $("#e0").click(function () {
        $(".m0").toggle();
        addClass(".m0", "e0", $(this).val());
    });
    $("#e1").click(function () {
        $(".m1").toggle();
        addClass(".m1", "e1", $(this).val());
    });
    $("#e2").click(function () {
        $(".m2").toggle();
        addClass(".m2", "e2", $(this).val());
    });
    $("#e3").click(function () {
        $(".m3").toggle();
        addClass(".m3", "e3", $(this).val());
    });
    $("#e4").click(function () {
        $(".m4").toggle();
        addClass(".m4", "e4", $(this).val());
    });
}

ko.applyBindings(new ViewModel());
function hidewait(o) {
    $('.' + o).append('<div id="wait"><img src="/Content/images/wait.gif" width="64" height="64" /><div class="happy-wait">' +
        ' </div>' +
        '</div>')
}