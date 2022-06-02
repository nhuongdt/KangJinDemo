var FormMode_NhomKhuVuc = function () {
    var self = this;
    self.TenKhuVuc = ko.observable();
    self.ID = ko.observable();
    self.GhiChu = ko.observable();
    self.setdata = function (item) {
        self.ID(item.ID);
        self.TenKhuVuc(item.TenKhuVuc);
        self.GhiChu(item.GhiChu);
    }
};

var FormModel_ViTri = function () {
    var self = this;
    self.MaViTri = ko.observable();
    self.TenViTri = ko.observable();
    self.GhiChu = ko.observable();
    self.ID = ko.observable();
    self.ID_KhuVuc = ko.observable();
    self.SetData = function (item) {
        self.ID(item.ID);
        self.ID_KhuVuc(item.ID_KhuVuc);
        self.TenViTri(item.TenViTri);
        self.GhiChu(item.GhiChu);
    }
};

var ViewModel = function () {
    var self = this;

    var NhomKVUri = '/api/DanhMuc/DM_KhuVucAPI/';
    var DMViTriUri = '/api/DanhMuc/DM_ViTriAPI/';
    var _IDchinhanh = $('#hd_IDdDonVi').val();
    var _IDNhanVien = $('.idnhanvien').text();
    var _IDNhomNguoiDung = $('.idnhomnguoidung').text();
    var _IDNguoiDung = $('.idnguoidung').text();
    self.TotalRecord = ko.observable(0);
    self.PageCount = ko.observable();
    self.TotalRecordHD = ko.observable(0);
    self.PageCountHD = ko.observable();
    self.NhomKhuVucs = ko.observableArray();
    self.ViTris = ko.observableArray();
    self.error = ko.observable();
    self.booleanAdd = ko.observable(true);
    self.newViTri = ko.observable(new FormModel_ViTri());
    self.HoaDons = ko.observableArray();

    self.filter = ko.observable();
    self.deleteTenViTri = ko.observable();
    self.deleteTenKhuVuc = ko.observable();
    self.deleteID = ko.observable();

    self.newNhomKhuVuc = ko.observable(new FormMode_NhomKhuVuc());
    self._ThemMoiNhomKV = ko.observable(true);

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
                    console.log("404")
                },
            }
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                console.log("fail");
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
    //function getAllDMViTris() {
    //    ajaxHelper(DMViTriUri + "GetListViTris", 'GET').done(function (data) {
    //        self.ViTris(data);
    //        var arr = [];
    //        for (var i = 0; i < data.length; i++) {
    //            var obj = {
    //                TenViTri: data[i].TenViTri,
    //                GhiChu: data[i].GhiChu,
    //                TenKhuVuc: data[i].TenKhuVuc,
    //            }
    //            arr.push(obj);
    //        }
    //        localStorage.setItem('lcExportViTri', JSON.stringify(arr));
    //    });
    //}

    function getAllDMKhuVucs() {
        ajaxHelper(NhomKVUri + "GetDM_KhuVuc", 'GET').done(function (data) {
            self.NhomKhuVucs(data);
        });
    }

    self.onEnterSearch = function (d, e) {
        var search = $('#txtSearch').val();
        if (e.keyCode === 13) {
            ajaxHelper(DMViTriUri + "GetListViTris?condition=" + search, 'GET').done(function (data) {
                self.ViTris(data);
            });
        }
        else {
            return true;
        }
    }

    function GetKhuVucByIDNhom(idKhuVuc) {
        self.currentPage(0);
        SearchPhongBan();
    }
    self.changeddlNhomKhuVuc = function () {
        GetKhuVucByIDNhom(this.newNhomKhuVuc().ID());
    }

    self.themmoicapnhatkhuvuc = function () {
        $('.btnxoakv').css('display', 'none');
        self.resetKhuVuc();
        self._ThemMoiNhomKV(true);
        $(".op-js-modal").modal('show');
        $('#txtTenKhuVuc').focus();
    }
    self.IDViTri = ko.observable();
    self.loadHoaDon = function (item) {
        self.currentPageHD(0);
        SearchHoaDon(item.ID);
    }

    self.addNhomKhuVuc = function (formElement) {

        var _idNhomKV = self.newNhomKhuVuc().ID();
        var _tenNhomKV = self.newNhomKhuVuc().TenKhuVuc();
        var _ghiChu = self.newNhomKhuVuc().GhiChu();

        if (_tenNhomKV == null || _tenNhomKV == "" || _tenNhomKV == "undefined") {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng nhập tên nhóm bàn", "danger");
            $("#txtTenKhuVuc").focus();
            return false;
        }

        ajaxHelper(NhomKVUri + "Check_TenKhuVucExist?tenKhuVuc=" + _tenNhomKV, 'POST').done(function (data) {
            if (data) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Tên nhóm bàn đã tồn tại", "danger");
                $('#txtTenKhuVuc').focus();
            }
            else {
                var objNhomKV = {
                    ID: _idNhomKV,
                    TenKhuVuc: _tenNhomKV,
                    GhiChu: _ghiChu
                };
                if (self._ThemMoiNhomKV() === true) {
                    $.ajax({
                        url: NhomKVUri + "PostDM_KhuVuc",
                        type: 'POST',
                        dataType: 'json',
                        data: objNhomKV ? JSON.stringify(objNhomKV) : null,
                        contentType: 'application/json',
                        success: function (item) {
                            self.NhomKhuVucs.push(item);
                            self.newViTri().ID_KhuVuc(item.ID);
                            bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Thêm nhóm bàn thành công", "success");
                            $(".op-js-modal").modal('hide');
                            $(".modal-ontop").hide();
                        },
                        statusCode: {
                            404: function () {
                                self.error("page not found");
                            },
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Thêm nhóm phòng bàn thất bại", "danger");
                            $(".op-js-modal").modal('hide');
                            $(".modal-ontop").hide();
                        },
                        complete: function () {
                            // window.location.href = '/Student/Index/';
                            self.resetKhuVuc();
                        }
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                            self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                        });
                }
            }
        });
    }

    self.editNhomKhuVuc = function (formElement) {
        var _idNhomKV = self.newNhomKhuVuc().ID();
        var _tenNhomKV = self.newNhomKhuVuc().TenKhuVuc();
        var _ghiChu = self.newNhomKhuVuc().GhiChu();

        if (_tenNhomKV == null || _tenNhomKV == "" || _tenNhomKV == "undefined") {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng nhập tên nhóm bàn", "danger");

            return false;
        }
        ajaxHelper(NhomKVUri + "Check_TenKhuVucEditExist?tenKhuVuc=" + _tenNhomKV + "&id=" + _idNhomKV, 'POST').done(function (data) {
            if (data) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Tên nhóm bàn đã tồn tại", "danger");
                $('#txtTenKhuVuc').focus();
            }
            else {
                var objNhomKV = {
                    ID: _idNhomKV,
                    TenKhuVuc: _tenNhomKV,
                    GhiChu: _ghiChu
                };
                if (self._ThemMoiNhomKV() === false) {
                    var myData = {};
                    myData.id = _idNhomKV;
                    myData.dM_NhomKhuVuc = objNhomKV;
                    myData.GhiChu = _ghiChu;
                    $.ajax({
                        url: NhomKVUri + "PutDM_KhuVuc",
                        type: 'PUT',
                        dataType: 'json',
                        //contentType: 'application/json',
                        // data: book ? JSON.stringify(book) : null,
                        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                        data: myData,
                        success: function () {
                            for (var i = 0; i < self.NhomKhuVucs().length; i++) {
                                if (self.NhomKhuVucs()[i].ID === _idNhomKV) {
                                    self.NhomKhuVucs.remove(self.NhomKhuVucs()[i]);
                                    break;
                                }
                            }
                            self.NhomKhuVucs.push(objNhomKV);

                        },
                        statusCode: {
                            404: function () {
                                self.error("page not found");
                            },
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                            $("#modalPopup_EditNhomKV").modal("hide");
                        },
                        complete: function () {
                            // window.location.href = '/Student/Index/';
                            $("#modalPopup_EditNhomKV").modal("hide");
                            bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật nhóm phòng bàn thành công", "success");
                            self.resetKhuVuc();
                        }
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                            self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                        });
                }
            }
        });
    }

    self.reset_ViTri = function () {
        self.newViTri(new FormModel_ViTri());
    }

    self.resetKhuVuc = function () {
        self.newNhomKhuVuc(new FormMode_NhomKhuVuc());
    }

    self.showPopupAddViTri = function () {
        self.reset_ViTri();
        self.booleanAdd(true);
        $('#modalPopuplg_ViTri').modal('show');
        $('.notyfi-none').css('display', 'none');
        $('.notyfi-none-nhom').css('display', 'none');
        $('#modalPopuplg_ViTri').on('shown.bs.modal', function () {
            $('#txtTenViTri').focus();
        })
    }

    self.TenViTriCu = ko.observable();
    self.editVT = function (item) {
        ajaxHelper(DMViTriUri + "GetDM_ViTri/" + item.ID, 'GET').done(function (data) {
            self.TenViTriCu(item.TenViTri);
            self.newViTri().SetData(item);
            self.booleanAdd(false);
        });
        $('#modalPopuplg_ViTri').modal('show');
        $('#modalPopuplg_ViTri').on('shown.bs.modal', function () {
            $('#txtTenViTri').focus().select();
        })
        $('.notyfi-none').css('display', 'none');
        $('.notyfi-none-nhom').css('display', 'none');
    }

    $('#modalPopup_EditNhomKV').on('shown.bs.modal', function () {
        $('#txtTenKhuVucedit').focus();
        $('#txtTenKhuVucedit').select();
    });

    self.editKV = function () {
        $('.btnxoakv').css('display', 'block');
        self.booleanAdd(false);
        ajaxHelper(NhomKVUri + "GetDM_KhuVuc/" + self.newNhomKhuVuc().ID(), 'GET').done(function (data) {
            self._ThemMoiNhomKV(false);
            self.newNhomKhuVuc().setdata(data);
        });
        $('#modalPopup_EditNhomKV').modal('show');
    }

    //self.addViTri = function (formElement) {
    //    var _maViTri = self.newViTri().MaViTri();
    //    var strTenViTri = self.newViTri().TenViTri();
    //    var _id = self.newViTri().ID();
    //    var _idNhomKV = self.newViTri().ID_KhuVuc() !== null ? self.newViTri().ID_KhuVuc() : null;
    //    var _ghiChu = self.newViTri().GhiChu();
    //    var strTenKhuVuc = self.newViTri().TenKhuVuc;

    //    if (strTenViTri == null || strTenViTri == "" || strTenViTri == "undefined") {
    //        //bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Không được để trống tên Phòng bàn!", "danger");
    //        $('.notyfi-none').css('display','block');
    //        $('.notyfi-none').html('Vui lòng nhập tên phòng bàn trước khi lưu!!!');
    //        $('#txtTenViTri').focus();
    //        return false;
    //    }
    //    else {
    //        $('.notyfi-none').css('display', 'none');
    //    }

    //    if (_idNhomKV === undefined) {
    //        //bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn phải chọn nhóm bàn!", "danger");
    //        $('.notyfi-none-nhom').css('display', 'block');
    //        $('.notyfi-none').css('display', 'none');
    //        $('.notyfi-none-nhom').html('Vui lòng chọn nhóm phòng bàn trước khi lưu!!!');
    //        $('#ddlNhomVT').focus();
    //        return false;
    //    }
    //    else {
    //        $('.notyfi-none-nhom').css('display', 'none');
    //    }

    //    var DM_ViTri = {
    //        ID: _id,
    //        MaViTri: _maViTri,
    //        TenViTri: strTenViTri,
    //        GhiChu: _ghiChu,
    //        ID_KhuVuc: _idNhomKV,
    //        TenKhuVuc: strTenKhuVuc,
    //    };

    //    if (self.booleanAdd() === false) {
    //        var myData = {};
    //        myData.id = _id;
    //        myData.objNewViTri = DM_ViTri;
    //        $.ajax({
    //            url: DMViTriUri + "PutDM_ViTri",
    //            type: 'PUT',
    //            dataType: 'json',
    //            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
    //            data: myData,
    //            success: function () {
    //                for (var i = 0; i < self.ViTris().length; i++) {
    //                    if (self.ViTris()[i].ID === _id) {
    //                        self.ViTris.remove(self.ViTris()[i]);
    //                        break;
    //                    }
    //                }
    //                self.ViTris.push(DM_ViTri);
    //            },
    //            statusCode: {
    //                404: function () {
    //                    self.error("page not found");
    //                },
    //            },
    //            error: function (jqXHR, textStatus, errorThrown) {
    //                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
    //            },
    //            complete: function () {
    //                $("#modalPopuplg_ViTri").modal("hide");
    //                getAllDMViTris();
    //                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật phòng bàn thành công!", "success");
    //                $('.notyfi-none').css('display', 'none');
    //                $('.notyfi-none-nhom').css('display', 'none');
    //            }
    //        })
    //            .fail(function (jqXHR, textStatus, errorThrown) {
    //                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
    //            });
    //    }
    //    else{
    //    ajaxHelper(DMViTriUri + "Check_TenVitriExist?tenViTri=" + strTenViTri, 'POST').done(function (data) {
    //        console.log('bàn : ' + data);
    //        if (data) {
    //            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Tên bàn đã tồn tại!", "danger");
    //        }
    //        else {

    //            var myData = {};
    //            myData.objNewViTri = DM_ViTri;

    //            if (self.booleanAdd() === true) {

    //                $.ajax({
    //                    data: myData,
    //                    url: DMViTriUri + "PostDM_ViTri",
    //                    type: 'POST',
    //                    async: true,
    //                    dataType: 'json',
    //                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",

    //                    success: function (item) {
    //                        self.ViTris.push(item);
    //                    },
    //                    statusCode: {
    //                        404: function () {
    //                            self.error("page not found");
    //                        },
    //                    },
    //                    error: function (jqXHR, textStatus, errorThrown) {
    //                        self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
    //                    },
    //                    complete: function () {
    //                        $("#modalPopuplg_ViTri").modal("hide");
    //                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Thêm phòng bàn thành công!", "success");
    //                        $('.notyfi-none').css('display', 'none');
    //                        $('.notyfi-none-nhom').css('display', 'none');
    //                    }
    //                })
    //                    .fail(function (jqXHR, textStatus, errorThrown) {
    //                        self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
    //                    });
    //            }         
    //        }
    //    });
    //    }
    //}

    self.addViTri = function (formElement) {
        document.getElementById("luuViTri").disabled = true;
        document.getElementById("luuViTri").lastChild.data = " Đang lưu";
        var _maViTri = self.newViTri().MaViTri();
        var strTenViTri = self.newViTri().TenViTri();
        var _id = self.newViTri().ID();
        var _idNhomKV = self.newViTri().ID_KhuVuc() !== null ? self.newViTri().ID_KhuVuc() : null;
        var _ghiChu = self.newViTri().GhiChu();
        var strTenKhuVuc = self.newViTri().TenKhuVuc;

        if (strTenViTri == null || strTenViTri == "" || strTenViTri == "undefined") {
            //bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Không được để trống tên Phòng bàn!", "danger");
            //$('.notyfi-none').css('display', 'block');
            //$('.notyfi-none-nhom').css('display', 'none');
            //$('.notyfi-none').html('Vui lòng nhập tên phòng bàn trước khi lưu!!!');
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng nhập tên phòng bàn trước khi lưu", "danger");
            $('#txtTenViTri').focus();
            document.getElementById("luuViTri").disabled = false;
            document.getElementById("luuViTri").lastChild.data = " Lưu";
            return false;
        }
        //else {
        //    $('.notyfi-none').css('display', 'none');
        //}

        if (_idNhomKV === undefined) {
            //bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn phải chọn nhóm bàn!", "danger");
            //$('.notyfi-none-nhom').css('display', 'block');
            //$('.notyfi-none').css('display', 'none');
            //$('.notyfi-none-nhom').html('Vui lòng chọn nhóm phòng bàn trước khi lưu!!!');
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng chọn nhóm bàn", "danger");
            $('#ddlNhomVT').focus();
            document.getElementById("luuViTri").disabled = false;
            document.getElementById("luuViTri").lastChild.data = " Lưu";
            return false;
        }
        //else {
        //    $('.notyfi-none-nhom').css('display', 'none');
        //}

        var DM_ViTri = {
            ID: _id,
            MaViTri: _maViTri,
            TenViTri: strTenViTri,
            GhiChu: _ghiChu,
            ID_KhuVuc: _idNhomKV,
            TenKhuVuc: strTenKhuVuc,
        };
        if (self.booleanAdd() === true) {
            var myData = {};
            myData.objNewViTri = DM_ViTri;
            if (strTenViTri !== null && strTenViTri !== "") {
                // check ma trung khi them moi
                ajaxHelper(DMViTriUri + "Check_TenVitriExist?tenViTri=" + strTenViTri + '&id_khuvuc=' + _idNhomKV, 'POST').done(function (data) {
                    if (data) {
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Tên bàn đã tồn tại", "danger");
                        document.getElementById("luuViTri").disabled = false;
                        document.getElementById("luuViTri").lastChild.data = " Lưu";
                        return false;
                    }
                    else {
                        callAjaxAdd(myData);
                    }
                })
            }
            else {
                callAjaxAdd(myData);
            }
        }
        // edit
        else {

            var myData = {};
            myData.id = _id;
            myData.objNewViTri = DM_ViTri;
            if (strTenViTri !== null && strTenViTri !== "") {
                // check ma trung khi update
                ajaxHelper(DMViTriUri + "Check_TenVitriExistEDit?tenViTri=" + strTenViTri + '&id_khuvuc=' + _idNhomKV + '&id_vitri=' + _id, 'POST').done(function (data) {
                    if (data) {
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Tên bàn đã tồn tại", "danger");
                        document.getElementById("luuViTri").disabled = false;
                        document.getElementById("luuViTri").lastChild.data = " Lưu";
                        return false;
                    }
                    else {
                        callAjaxUpdate(myData);
                    }
                })
            }
            else {
                callAjaxUpdate(myData);
                self.UpdateViTriLS(strTenViTri);
            }
        }
    }

    function callAjaxAdd(myData) {
        $.ajax({
            data: myData,
            url: DMViTriUri + "PostDM_ViTri",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",

            success: function (item) {
                self.AddViTriLS(item.TenViTri);
                SearchPhongBan();
            },
            statusCode: {
                404: function () {
                    self.error("page not found");
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                document.getElementById("luuViTri").disabled = false;
                document.getElementById("luuViTri").lastChild.data = " Lưu";
            },
            complete: function () {
                document.getElementById("luuViTri").disabled = false;
                document.getElementById("luuViTri").lastChild.data = " Lưu";
                $("#modalPopuplg_ViTri").modal("hide");
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Thêm phòng bàn thành công", "success");
                $('.notyfi-none').css('display', 'none');
                $('.notyfi-none-nhom').css('display', 'none');
            }
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
            });
    }

    function callAjaxUpdate(myData) {

        $.ajax({
            url: DMViTriUri + "PutDM_ViTri",
            type: 'PUT',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: myData,
            success: function () {
                SearchPhongBan();
            },
            statusCode: {
                404: function () {
                    self.error("page not found");
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                document.getElementById("luuViTri").disabled = false;
                document.getElementById("luuViTri").lastChild.data = " Lưu";
            },
            complete: function () {
                document.getElementById("luuViTri").disabled = false;
                document.getElementById("luuViTri").lastChild.data = " Lưu";
                $("#modalPopuplg_ViTri").modal("hide");
                SearchPhongBan();
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật phòng bàn thành công", "success");
                $('.notyfi-none').css('display', 'none');
                $('.notyfi-none-nhom').css('display', 'none');
            }
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
            });
    }

    self.AddViTriLS = function (tenphongban) {
        var objDiary = {
            ID_NhanVien: _IDNhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Phòng bàn",
            NoiDung: "Thêm mới phòng bàn : " + tenphongban,
            NoiDungChiTiet: "Thêm mới phòng bàn : " + tenphongban,
            LoaiNhatKy: 1 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
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

    self.UpdateViTriLS = function (tenphongban) {
        var objDiary = {
            ID_NhanVien: _IDNhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Phòng bàn",
            NoiDung: "Cập nhật phòng bàn : " + self.TenViTriCu() + " thành: " + tenphongban,
            NoiDungChiTiet: "Cập nhật phòng bàn : " + self.TenViTriCu() + " thành: " + tenphongban,
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

    self.XoaViTriLS = function (tenphongban) {
        var objDiary = {
            ID_NhanVien: _IDNhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Phòng bàn",
            NoiDung: "Xóa phòng bàn : " + tenphongban,
            NoiDungChiTiet: "Xóa phòng bàn : " + tenphongban,
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

    self.exportToExcelViTris = function () {
        //tableToExcel('tblDanhMucHangHoa', 'dmHangHoas.xls');
        var data = localStorage.getItem('lcExportViTri');
        if (data != null) {
            data = JSON.parse(data);
            console.log(data);
            alasql("SELECT [TenViTri] AS [Tên hàng hóa], " +
                " [GhiChu] AS [Ghi chú], " +
                " [TenKhuVuc] AS [Tên nhóm bàn] " +
                " INTO XLSX('PhongBan.xlsx', { headers: true }) FROM ? ", [data]);
        }
        localStorage.removeItem('lcExportViTri');
    }
    //Xóa phòng bàn
    self.modalDelete = function (item) {
        self.deleteTenViTri(item.TenViTri);
        self.deleteID(item.ID);
        console.log(self.deleteTenViTri);
        $('#modalpopup_deleteVT').modal('show');
    };
    self.modalDeleteKV = function (KhuVucs) {
        self.deleteTenKhuVuc(self.newNhomKhuVuc().TenKhuVuc());
        self.deleteID(self.newNhomKhuVuc().ID());
        console.log(self.deleteTenKhuVuc);
        $('#modalpopup_deleteKV').modal('show');
    };
    self.xoaVT = function (ViTris) {
        $.ajax({
            type: "DELETE",
            url: DMViTriUri + "DeleteDM_ViTri/" + ViTris.deleteID(),
            dataType: 'json',
            contentType: 'application/json',
            success: function (result) {
                self.XoaViTriLS(self.deleteTenViTri())
                SearchPhongBan();
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật phòng bàn thành công", "success");
            },
            error: function (error) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cập nhật phòng bàn thất bại", "danger");
            }
        })
    };
    self.xoaKV = function (KhuVucs) {
        $.ajax({
            type: "DELETE",
            url: NhomKVUri + "DeleteDM_KhuVuc/" + KhuVucs.deleteID(),
            dataType: 'json',
            contentType: 'application/json',
            success: function (result) {
                SearchPhongBan();
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật nhóm phòng bàn thành công", "success");
                $("#modalPopup_EditNhomKV").modal("hide");
                getAllDMKhuVucs();
            },
            error: function (error) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Cập nhật nhóm phòng bàn thất bại", "danger");
            }
        })
    };

    //var tableToExcel = (function () {
    //    var uri = 'data:application/vnd.ms-excel;base64,'
    //        , template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table>{table}</table></body></html>'
    //        , base64 = function (s) { return window.btoa(unescape(encodeURIComponent(s))) }
    //        , format = function (s, c) { return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; }) }
    //    return function (table, name) {
    //        if (!table.nodeType) table = document.getElementById(table)
    //        var ctx = { worksheet: name || 'Worksheet', table: table.innerHTML }
    //        window.location.href = uri + base64(format(template, ctx))
    //    }
    //})()

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
    self.arrPagging = ko.observableArray();

    self.pageSizesHD = [10, 20, 30, 40, 50];
    self.pageSizeHD = ko.observable(self.pageSizesHD[0]);
    self.currentPageHD = ko.observable(0);
    self.fromitemHD = ko.observable(1);
    self.toitemHD = ko.observable();
    //lọc vi trí
    self.filteredDMViTri = ko.computed(function () {
        var first = self.currentPage() * self.pageSize();
        var _filter = self.filter();

        var arrFilter = ko.utils.arrayFilter(self.ViTris(), function (prod) {
            var chon = true;
            var arr = locdau(prod.TenViTri).toLowerCase().split(/\s+/);
            var sSearch = '';

            for (var i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }
            if (chon && _filter) {
                chon = (locdau(prod.TenViTri).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
                    locdau(prod.GhiChu).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
                    locdau(prod.ID_KhuVuc).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
                    sSearch.indexOf(locdau(_filter).toLowerCase()) >= 0
                );
            }
            return chon;
        });
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

    $("#textSearch").keypress(function (e) {
        if (e.keyCode == 13) {
            self.currentPage(0);
            SearchPhongBan();
        }
    });
    function SearchPhongBan(isGoToNext = false) {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('HangHoa_XemDS', lc_CTQuyen) > -1) {
            var txtMaHDon = self.filter();

            if (txtMaHDon === undefined) {
                txtMaHDon = "";
            }
            $('.table_h').gridLoader();
            ajaxHelper(DMViTriUri + 'GetListViTris_where?currentPage=' + self.currentPage() + '&pageSize=' + self.pageSize() + '&idkhuvuc=' + self.newNhomKhuVuc().ID() +
                '&maHoaDon=' + txtMaHDon,
                'GET').done(function (data) {
                    $('.table_h').gridLoader({ show: false });
                    self.ViTris(data);
                });
            if (!isGoToNext) {
                // page count
                ajaxHelper(DMViTriUri + 'GetPageCountViTris_Where?currentPage=' + self.currentPage() + '&pageSize=' + self.pageSize() + '&idkhuvuc=' + self.newNhomKhuVuc().ID() +
                    '&maHoaDon=' + txtMaHDon,
                    'GET').done(function (data) {
                        $('.table_h').gridLoader({ show: false });
                        self.TotalRecord(data.TotalRecord);
                        self.PageCount(data.PageCount);
                    });
            }
        }
    }

    function SearchHoaDon(id, isGoToNext = false) {
        self.IDViTri(id);
        $('.table_h').gridLoader();
        ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'LoadHoaDonByIDViTri?currentPage=' + self.currentPageHD() + '&pageSize=' + self.pageSizeHD() + "&idvitri=" + id,
            'GET').done(function (data) {
                $('.table_h').gridLoader({ show: false });
                self.HoaDons(data);
            });
        if (!isGoToNext) {
            // page count
            ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'GetPageCountLoadHoaDon?currentPage=' + self.currentPageHD() + '&pageSize=' + self.pageSizeHD() + "&idvitri=" + id,
                'GET').done(function (data) {
                    $('.table_h').gridLoader({ show: false });
                    self.TotalRecordHD(data.TotalRecord);
                    self.PageCountHD(data.PageCount);
                });
        }
    }

    self.ResetCurrentPage = function () {
        
        self.currentPage(0);
        SearchPhongBan();
    };
    self.PageResults = ko.computed(function () {
        if (self.ViTris() !== null) {

            self.fromitem((self.currentPage() * self.pageSize()) + 1);

            if (((self.currentPage() + 1) * self.pageSize()) > self.ViTris().length) {
                var fromItem = (self.currentPage() + 1) * self.pageSize();
                if (fromItem < self.TotalRecord()) {
                    self.toitem((self.currentPage() + 1) * self.pageSize());
                }
                else {
                    self.toitem(self.TotalRecord());
                }
            } else {
                self.toitem((self.currentPage() * self.pageSize()) + self.pageSize());
            }
        }
    });

    self.PageList_Display = ko.computed(function () {
        var arrPage = [];

        var allPage = self.PageCount();
        var currentPage = self.currentPage();

        if (allPage > 4) {

            var i = 0;
            if (currentPage === 0) {
                i = parseInt(self.currentPage()) + 1;
            }
            else {
                i = self.currentPage();
            }

            if (allPage >= 5 && currentPage > allPage - 5) {
                if (currentPage >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (var i = allPage - 5; i < allPage; i++) {
                        var obj = {
                            pageNumber: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    // get currentPage - 2 , currentPage, currentPage + 2
                    for (var j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                        var obj = {
                            pageNumber: j + 1,
                        };
                        arrPage.push(obj);
                    }
                }
            }
            else {
                // get 5 trang dau
                if (i >= 2) {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumber: i - 1,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
                else {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumber: i,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
            }
        }
        else {
            // neu chi co 1 trang --> khong hien thi DS trang
            if (allPage > 1) {
                for (var i = 0; i < allPage; i++) {
                    var obj = {
                        pageNumber: i + 1,
                    };
                    arrPage.push(obj);
                }
            }
        }
        return arrPage;
    });

    self.VisibleStartPage = ko.computed(function () {
        if (self.PageList_Display().length > 0) {
            return self.PageList_Display()[0].pageNumber !== 1;
        }
    });

    self.VisibleEndPage = ko.computed(function () {
        if (self.PageList_Display().length > 0) {
            return self.PageList_Display()[self.PageList_Display().length - 1].pageNumber !== self.PageCount();
        }
    });

    self.GoToPageHD = function (page) {
        if (page.pageNumber !== '.') {
            self.currentPage(page.pageNumber - 1);
            SearchPhongBan(true);
        }
    };

    self.StartPage = function () {
        self.currentPage(0);
        SearchPhongBan(true);
    }

    self.BackPage = function () {
        if (self.currentPage() > 1) {
            self.currentPage(self.currentPage() - 1);
            SearchPhongBan(true);
        }
    }

    self.GoToNextPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.currentPage() + 1);
            SearchPhongBan(true);
        }
    }

    self.EndPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.PageCount() - 1);
            SearchPhongBan(true);
        }
    }
    self.GetClassHD = function (page) {
        return ((page.pageNumber - 1) === self.currentPage()) ? "click" : "";
    };


    self.PageResultsHD = ko.computed(function () {
        if (self.HoaDons() !== null) {

            self.fromitemHD((self.currentPageHD() * self.pageSizeHD()) + 1);

            if (((self.currentPageHD() + 1) * self.pageSizeHD()) > self.HoaDons().length) {
                var fromItem = (self.currentPageHD() + 1) * self.pageSizeHD();
                if (fromItem < self.TotalRecordHD()) {
                    self.toitemHD((self.currentPageHD() + 1) * self.pageSizeHD());
                }
                else {
                    self.toitemHD(self.TotalRecordHD());
                }
            } else {
                self.toitemHD((self.currentPageHD() * self.pageSizeHD()) + self.pageSizeHD());
            }
        }
    });

    self.PageList_DisplayHD = ko.computed(function () {
        var arrPage = [];

        var allPage = self.PageCountHD();
        var currentPage = self.currentPageHD();

        if (allPage > 4) {

            var i = 0;
            if (currentPage === 0) {
                i = parseInt(self.currentPageHD()) + 1;
            }
            else {
                i = self.currentPageHD();
            }

            if (allPage >= 5 && currentPage > allPage - 5) {
                if (currentPage >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (var i = allPage - 5; i < allPage; i++) {
                        var obj = {
                            pageNumberHD: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    // get currentPage - 2 , currentPage, currentPage + 2
                    if (currentPage == 1) {
                        for (var j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumberHD: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    } else {
                        for (var j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                            var obj = {
                                pageNumberHD: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    }
                }
            }
            else {
                // get 5 trang dau
                if (i >= 2) {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumberHD: i - 1,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
                else {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumberHD: i,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
            }
        }
        else {
            // neu chi co 1 trang --> khong hien thi DS trang
            if (allPage > 1) {
                for (var i = 0; i < allPage; i++) {
                    var obj = {
                        pageNumberHD: i + 1,
                    };
                    arrPage.push(obj);
                }
            }
        }
        return arrPage;
    });

    self.VisibleStartPageHD = ko.computed(function () {
        if (self.PageList_DisplayHD().length > 0) {
            return self.PageList_DisplayHD()[0].pageNumberHD !== 1;
        }
    });

    self.VisibleEndPageHD = ko.computed(function () {
        if (self.PageList_DisplayHD().length > 0) {
            return self.PageList_DisplayHD()[self.PageList_DisplayHD().length - 1].pageNumberHD !== self.PageCountHD();
        }
    });

    self.GoToPage_Load = function (page) {
        if (page.pageNumberHD !== '.') {
            self.currentPageHD(page.pageNumberHD - 1);
            SearchHoaDon(self.IDViTri(), true);
        }
    };

    self.StartPage_Load = function () {
        self.currentPageHD(0);
        SearchHoaDon(self.IDViTri(), true);
    }

    self.BackPage_Load = function () {
        if (self.currentPageHD() > 1) {
            self.currentPageHD(self.currentPageHD() - 1);
            SearchHoaDon(self.IDViTri(), true);
        }
    }

    self.GoToNextPage_Load = function () {
        if (self.currentPageHD() < self.PageCountHD() - 1) {
            self.currentPageHD(self.currentPageHD() + 1);
            SearchHoaDon(self.IDViTri(), true);
        }
    }

    self.EndPage_Load = function () {
        if (self.currentPageHD() < self.PageCountHD() - 1) {
            self.currentPageHD(self.PageCountHD() - 1);
            SearchHoaDon(self.IDViTri(), true);
        }
    }
    self.GetClass_Load = function (page) {
        return ((page.pageNumberHD - 1) === self.currentPageHD()) ? "click" : "";
    };

    self.linkphieuHD = function (item) {
        console.log(item.MaHoaDon);
        localStorage.setItem('FindHD', item.MaHoaDon);
        if (item.LoaiHoaDon == 1) {
            var url = "/#/Invoices";
            window.open(url);
        }
        if (item.LoaiHoaDon == 6) {
            var url = "/#/Returns";
            window.open(url);
        }
    }

    //self.exportToExcelViTris = function () {
    //    tableToExcel('tblDanhMucViTri', 'dmViTris.xls');
    //}
    self.onFileSelectedEvent = function (vm, evt) {
        /* set up XMLHttpRequest */
        if (evt.target.files.length > 0) {
            var url = URL.createObjectURL(evt.target.files[0]);
            var oReq = new XMLHttpRequest();
            oReq.open("GET", url, true);
            oReq.responseType = "arraybuffer";

            oReq.onload = function (e) {
                var arraybuffer = oReq.response;
                /* convert data to binary string */
                var data = new Uint8Array(arraybuffer);
                var arr = new Array();
                for (var i = 0; i !== data.length; ++i) arr[i] = String.fromCharCode(data[i]);
                var bstr = arr.join("");
                /* Call XLSX */
                var workbook = XLSX.read(bstr, { type: "binary" });
                /* DO SOMETHING WITH workbook HERE */
                var first_sheet_name = workbook.SheetNames[0];
                /* Get worksheet */
                var worksheet = workbook.Sheets[first_sheet_name];
                console.log(XLSX.utils.sheet_to_json(worksheet, { raw: true }));
                var dataObjects = XLSX.utils.sheet_to_json(worksheet, { raw: true });
                if (dataObjects.length > 0) {
                    //for (var k = 0; k != dataObjects.length; ++k) {
                    //    var objdataInsert = dataObjects[k];
                    //    alert(objdataInsert.col2);
                    //    //
                    // var _idNhomHH=";
                    // var strTenHangHoa="";
                    // var _ghiChu="";
                    // var HHoa = {
                    //    ID_NhomHang: _idNhomHH,
                    //    TenHangHoa: strTenHangHoa,
                    //    GhiChu: _ghiChu
                    //};

                    //    var myData = {};
                    //    myData.objNewHH = HHoa;
                    //    //
                    //    $.ajax({
                    //        url: DMHangHoaUri + "PostDM_HangHoa",
                    //        type: 'POST',
                    //        async: true,
                    //        dataType: 'json',
                    //        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    //        data: myData,
                    //        success: function (item) {
                    //            self.HangHoas.push(item);
                    //        },
                    //        statusCode: {
                    //            404: function () {
                    //                self.error("page not found");
                    //            },
                    //        },
                    //        error: function (jqXHR, textStatus, errorThrown) {
                    //            self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                    //        },
                    //        complete: function () {
                    //        }
                    //    })
                    //      .fail(function (jqXHR, textStatus, errorThrown) {
                    //          self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                    //      });
                    //}
                }
            }
            oReq.send();
        }
    }
    //getAllDMViTris();
    SearchPhongBan();
    getAllDMKhuVucs();
}

ko.applyBindings(new ViewModel());
function hidewait(o) {
    $('.' + o).append('<div id="wait"><img src="/Content/images/wait.gif" width="64" height="64" /><div class="happy-wait">' +
        ' </div>' +
        '</div>')
}
//format number
function formatNumberObj(obj) {
    if (obj !== null) {
        var objVal = $(obj).val();
        $(obj).val(objVal.toString().replace(/,/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ","));
        return objVal;
    }
}
function formatNumberToInt(objVal) {
    return parseInt(objVal.replace(/,/g, ''));
}
function formatNumber(number) {
    if (number === undefined || number == null) {
        return 0;
    }
    else {
        return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
}