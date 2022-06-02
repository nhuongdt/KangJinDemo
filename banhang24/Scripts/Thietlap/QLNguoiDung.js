var FormModel_VaiTro = function () {
    var self = this;
    self.MaNhom = ko.observable();
    self.TenNhom = ko.observable();
    self.ID = ko.observable();
    self.HT_Quyen_NhomDTO = ko.observableArray();
    self.setdata = function (item) {
        self.MaNhom(item.MaNhom);
        self.TenNhom(item.TenNhom);
        self.ID(item.ID);
        self.HT_Quyen_NhomDTO(item.HT_Quyen_NhomDTO);
    }
};

var ViewModel = function () {
    var self = this;
    var HTNguoiDungs = '/api/DanhMuc/HT_NguoiDungAPI/';
    var _IDNhanVien = $('.idnhanvien').text();
    self.TotalRecord = ko.observable(0);
    self.PageCount = ko.observable();
    self.selectedNV = ko.observable();
    self.selectedDV = ko.observable();
    self.selectedVaiTro = ko.observable();
    self.selectedChange = ko.observable();
    self.Loc_Admin = ko.observable("2");
    self.Loc_HoatDong = ko.observable("0");
    self.NhanViens = ko.observableArray();
    self.DonVis = ko.observableArray();

    self.error = ko.observable();
    self.deleteID = ko.observable();
    self.filter = ko.observable();
    self.Quyen_NguoiDung = ko.observableArray();
    self.currentUser = ko.observable();
    self.currentVaiTro = ko.observable();

    self.NguoiDung = ko.observableArray();
    self.QuyenChas = ko.observableArray();
    self.ListDonVis = ko.observableArray();

    self.XemGiaVonND = ko.observable(false);
    self.DonViTrungGian = ko.observableArray();
    self.ID_DonViND = ko.observable();
    self.roleUpdateUser = ko.observable(false);

    self.NhomNguoiDungs = ko.observableArray();
    self.QuyenEdit = ko.observableArray();
    self.NguoiDungFilterbyRole = ko.observableArray();
    self.availableUser = ko.observable();
    self._ThemMoiND = ko.observable(true);
    self.newVaiTro = ko.observable(new FormModel_VaiTro);
    self.isGara = ko.observable(false);
    //phân trang
    self.pageSizes = [10, 20, 30, 40, 50];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.currentPage = ko.observable(0);
    self.fromitem = ko.observable(1);
    self.toitem = ko.observable();
    self.arrPagging = ko.observableArray();
    self.arrPagging = ko.observableArray();

    var _IDchinhanh = VHeader.IdDonVi;
    var shopCookies = $('#shopCookies').val();
    if (shopCookies === 'C16EDDA0-F6D0-43E1-A469-844FAB143014') {
        self.isGara(true);
    }

    function PageLoad() {
        getAllDMNhanVien();
        SearchNguoiDung();
        getAllVaiTro();
    }

    function getAllDMNhanVien() {
        ajaxHelper("/api/DanhMuc/NS_NhanVienAPI/" + "GetListNhanVienChuaCoND?iddonvi=" + _IDchinhanh, 'GET').done(function (data) {
            self.NhanViens(data);
            self.Quyen_NguoiDung = VHeader.Quyen;//  get quyen from header
            self.roleUpdateUser(CheckRoleExist('NguoiDung_CapNhat'));
            vmThemMoiNguoiDung.listData.NhanViens = data;
        });
    }

    function CheckRoleExist(maquyen) {
        return $.inArray(maquyen, self.Quyen_NguoiDung) > -1;
    }

    self.getAllNhanVienEditND = function (id) {
        ajaxHelper("/api/DanhMuc/NS_NhanVienAPI/" + "GetListNhanVienEdit?idnhanvien=" + id, 'GET').done(function (data) {
            self.NhanViens(data);
        });
    }

    self.ShowPopupCapNhat = function (item, event) {
        var checked = ($(event.target).is(':checked'));
        self.XemGiaVonND(checked);
        var mes = 'Bạn muốn cho người dùng xem giá vốn và lợi nhuận?';
        if (!checked) {
            mes = 'Bạn không muốn cho người dùng xem giá vốn và lợi nhuận?';
        }
        dialogConfirm('Thông báo', mes, function () {
            self.CapNhatQuyenXemGiaVon();
        })
    }

    self.CapNhatQuyenXemGiaVon = function () {
        var myData = {};
        myData.id = self.currentUser().ID;
        var HT_NguoiDung = {
            XemGiaVon: self.XemGiaVonND()
        }
        myData.objNewND = HT_NguoiDung;
        ajaxHelper(HTNguoiDungs + "PutHT_NguoiDungXemGiaVon", 'PUT', myData).done(function () {
            for (let i = 0; i < self.NguoiDung().length; i++) {
                if (self.NguoiDung()[i].ID === self.currentUser().ID) {
                    self.NguoiDung()[i].XemGiaVon = self.XemGiaVonND();
                    break;
                }
            }
            ShowMessage_Success('Cập nhật người dùng thành công');
        }).fail(function (x) {
            ShowMessage_Danger('Cập nhật người dùng thất bại');
        }).always(function (x) {
            $('#exampleModalCenterTrue').modal('hide');
            $('#exampleModalCenterFalse').modal('hide');
        });
    }

    self.txtSearchCN = ko.observableArray();
    self.arrChiNhanh_filter = ko.observableArray();
    self.txtSearchRole = ko.observableArray();
    self.arrRole_filter = ko.observableArray();

    self.FilterChiNhanh = function () {
        if (commonStatisJs.CheckNull(self.txtSearchCN())) {
            self.arrChiNhanh_filter(self.ListDonVis());
            return;
        }
        var txt = locdau(self.txtSearchCN());
        var arr = $.grep(self.ListDonVis(), function (x) {
            return locdau(x.TenDonVi).indexOf(txt) > -1;
        });
        self.arrChiNhanh_filter(arr);
    }

    self.SearchRole = function () {
        if (!self.txtSearchRole()) {
            self.arrRole_filter(self.NhomNguoiDungs());
            return;
        }
        var txt = locdau(self.txtSearchRole());
        var arr = $.grep(self.NhomNguoiDungs(), function (x) {
            return locdau(x.TenNhom).indexOf(txt) > -1;
        });
        self.arrRole_filter(arr);
    }

    self.getallQuyen = function (item) {
        self.XemGiaVonND(item.XemGiaVon);
        var dem = 0;
        var lenght = 0;
        $("#phanquyen" + item.ID).find(".checkthugon").removeClass("check");
        $('.khongphanquyencn').hide();
        self.selectedVaiTro(item.IDNhomNguoiDung);
        $('.loadgetallquyen').gridLoader();
        ajaxHelper(HTNguoiDungs + "GetAllQuyen", 'GET').done(function (data) {
            self.QuyenChas([]);
            for (var i = 0; i < data.length; i++) {
                if (data[i].QuyenCha == "") {
                    var objParent = {
                        MaQuyen: data[i].MaQuyen,
                        TenQuyen: data[i].TenQuyen,
                        DuocSuDung: true,
                        Childs: [],
                    }
                    for (var j = 0; j < data.length; j++) {
                        if (data[j].MaQuyen !== data[i].MaQuyen && data[j].QuyenCha === data[i].MaQuyen) {
                            var objChild =
                            {
                                MaQuyen: data[j].MaQuyen,
                                TenQuyen: data[j].TenQuyen,
                                QuyenCha: data[i].QuyenCha,
                                DuocSuDung: data[j].DuocSuDung,
                                Child2s: []
                            };
                            for (var k = 0; k < data.length; k++) {
                                if (data[k].QuyenCha !== null && data[k].QuyenCha === data[j].MaQuyen) {
                                    var objChild2 =
                                    {
                                        MaQuyen: data[k].MaQuyen,
                                        TenQuyen: data[k].TenQuyen,
                                        QuyenCha: data[j].QuyenCha,
                                        DuocSuDung: data[k].DuocSuDung
                                    };
                                    objChild.Child2s.push(objChild2);
                                }
                            }
                            objParent.Childs.push(objChild);
                        }
                    }
                    self.QuyenChas.push(objParent);
                }
            }

            ajaxHelper(HTNguoiDungs + "GetHT_NhomNguoiDung?idnguoidung=" + item.ID + '&iddonvi=' + item.ID_DonVi, 'GET').done(function (data1) {
                self.newVaiTro().setdata(data1);
                if (data1.HT_Quyen_NhomDTO.length > 0) {
                    for (var i = 0; i < self.QuyenChas().length; i++) {
                        for (var j = 0; j < self.QuyenChas()[i].Childs.length; j++) {
                            var child = self.QuyenChas()[i].Childs[j];
                            for (var z = 0; z < data1.HT_Quyen_NhomDTO.length; z++) {
                                if (child.MaQuyen === data1.HT_Quyen_NhomDTO[z].MaQuyen) {
                                    child.DuocSuDung = true;
                                }
                            }
                            for (var k = 0; k < self.QuyenChas()[i].Childs[j].Child2s.length; k++) {
                                var chid2 = self.QuyenChas()[i].Childs[j].Child2s[k];
                                lenght = self.QuyenChas()[i].Childs[j].Child2s.length;
                                for (var h = 0; h < data1.HT_Quyen_NhomDTO.length; h++) {
                                    var quyenDTO = data1.HT_Quyen_NhomDTO[h];
                                    if (chid2.MaQuyen === quyenDTO.MaQuyen) {
                                        chid2.DuocSuDung = true;
                                        dem = dem + 1;
                                    }
                                }
                                if (dem === lenght) {
                                    child.DuocSuDung = true;
                                }
                            }
                            dem = 0;
                            lenght = 0;
                        }
                    }


                    for (var k = 0; k < self.QuyenChas().length; k++) {
                        var demtrueChild = 0;
                        for (var i = 0; i < self.QuyenChas()[k].Childs.length; i++) {
                            var count = 0;
                            var countCheck = 0;
                            if (self.QuyenChas()[k].Childs[i].DuocSuDung === true) {
                                demtrueChild = demtrueChild + 1;
                            }
                            //count = self.QuyenChas()[k].Childs[i].Child2s.lenght;
                            for (var j = 0; j < self.QuyenChas()[k].Childs[i].Child2s.length; j++) {
                                count = count + 1;
                                if (self.QuyenChas()[k].Childs[i].Child2s[j].DuocSuDung === true) {
                                    countCheck = countCheck + 1;
                                }
                            }
                            if (count !== countCheck) {
                                $('#phanquyen' + item.ID).find('#allquyencheck' + self.QuyenChas()[k].Childs[i].MaQuyen).addClass('op-checkbox-square');

                            }
                            if (countCheck === 0) {
                                $('#phanquyen' + item.ID).find('#allquyencheck' + self.QuyenChas()[k].Childs[i].MaQuyen).removeClass('op-checkbox-square');
                            }

                        }

                        if (demtrueChild === 0) {
                            self.QuyenChas()[k].DuocSuDung = false;
                        }
                    }

                    self.QuyenEdit(self.QuyenChas());
                    $('.khongphanquyencn').css('display', 'none');
                }
                else {
                    self.QuyenEdit([]);
                    $('.khongphanquyencn').css('display', 'block');
                }
                $('.loadgetallquyen').gridLoader({ show: false });
            });
        });


        ajaxHelper("/api/DanhMuc/DM_DonViAPI/" + "getListIDDonViVaiTro?idnhanvien=" + item.ID_NhanVien + '&idnguoidung=' + item.ID, 'GET').done(function (data) {
            self.ListDonVis(data);
            self.arrChiNhanh_filter(data);
            self.DonViTrungGian(data);
            for (var i = 0; i < self.ListDonVis().length; i++) {
                if (self.ListDonVis()[i].ID_DonVi === item.ID_DonVi) {
                    $('#phanquyen' + item.ID).find('#checkvaitro' + item.ID_DonVi).addClass('active-chinhanh');
                }
            }
        });
    }

    self.LoadQuyenByIDChiNhanh = function (item) {
        $('.permission-left ul li').each(function (i) {
            $(this).removeClass('active-chinhanh');
        });
        $('#phanquyen' + item.IDNguoiDung).find('#checkvaitro' + item.ID_DonVi).addClass('active-chinhanh');
        self.ChangeDsQuyen(item);
    }

    self.arrVaiTroDonVi = function (item) {
        $('.permission-left ul li').each(function (i) {
            $(this).removeClass('active-chinhanh');
        });
        $('#phanquyen' + item.IDNguoiDung).find('#checkvaitro' + item.ID_DonVi).addClass('active-chinhanh');

        if (item.ID_VaiTro === undefined) {
            item.ID_VaiTro = '00000000-0000-0000-0000-000000000000';
        }
        var myData = {};
        myData.objVaiTro = item;
        $.ajax({
            url: HTNguoiDungs + "Put_HT_NguoiDung_Nhom",
            type: 'PUT',
            dataType: 'json',
            data: myData,
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function () {
                //bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật vai trò thành công cho chi nhánh " + item.TenDonVi, "success");
                if (item.ID_VaiTro === '00000000-0000-0000-0000-000000000000') {
                    item.ID = '00000000-0000-0000-0000-000000000000';
                    self.ChangeDsQuyen(item);
                }
                else {
                    ajaxHelper(HTNguoiDungs + 'GetHT_NguoiDung_Nhom?idnguoidung=' + item.IDNguoiDung + '&iddonvi=' + item.ID_DonVi, 'GET').done(function (data) {
                        item.ID = data.ID;
                        self.ChangeDsQuyen(data);
                    })
                }
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

    self.clickthugon = function (item) {
        $("#phanquyen" + item.ID).find(".checkthugon").toggleClass("check");
        if ($("#phanquyen" + item.ID).find(".checkthugon").hasClass("check")) {
            $(".col3 .check-sque").hide();
        }
        else {
            $(".col3 .check-sque").show();
        }
    }

    function getallQuyen() {
        self.QuyenChas([]);
        ajaxHelper(HTNguoiDungs + "GetAllQuyen/", 'GET').done(function (data) {

            for (var i = 0; i < data.length; i++) {
                if (data[i].QuyenCha == "") {
                    var objParent = {
                        MaQuyen: data[i].MaQuyen,
                        TenQuyen: data[i].TenQuyen,
                        Childs: [],
                    }
                    for (var j = 0; j < data.length; j++) {
                        if (data[j].MaQuyen !== data[i].MaQuyen && data[j].QuyenCha === data[i].MaQuyen) {
                            var objChild =
                            {
                                MaQuyen: data[j].MaQuyen,
                                TenQuyen: data[j].TenQuyen,
                                QuyenCha: data[i].QuyenCha,
                                DuocSuDung: data[j].DuocSuDung,
                                Child2s: []
                            };
                            for (var k = 0; k < data.length; k++) {
                                if (data[k].QuyenCha !== null && data[k].QuyenCha === data[j].MaQuyen) {
                                    var objChild2 =
                                    {
                                        MaQuyen: data[k].MaQuyen,
                                        TenQuyen: data[k].TenQuyen,
                                        QuyenCha: data[j].QuyenCha,
                                        DuocSuDung: data[k].DuocSuDung
                                    };
                                    objChild.Child2s.push(objChild2);
                                }
                            }
                            objParent.Childs.push(objChild);
                        }
                    }
                    self.QuyenChas.push(objParent);
                }
            }
        });
    }

    self.loadLaiQuyen = function (id) {
        self.QuyenChas([]);
        arrMaQuyen = [];
        ajaxHelper(HTNguoiDungs + "GetAllQuyen/", 'GET').done(function (data) {

            for (var i = 0; i < data.length; i++) {
                if (data[i].QuyenCha == null) {
                    var objParent = {
                        MaQuyen: data[i].MaQuyen,
                        TenQuyen: data[i].TenQuyen,
                        Childs: [],
                    }
                    for (var j = 0; j < data.length; j++) {
                        if (data[j].MaQuyen !== data[i].MaQuyen && data[j].QuyenCha === data[i].MaQuyen) {
                            var objChild =
                            {
                                MaQuyen: data[j].MaQuyen,
                                TenQuyen: data[j].TenQuyen,
                                QuyenCha: data[i].QuyenCha,
                                DuocSuDung: data[j].DuocSuDung,
                                Child2s: []
                            };
                            for (var k = 0; k < data.length; k++) {
                                if (data[k].QuyenCha !== null && data[k].QuyenCha === data[j].MaQuyen) {
                                    var objChild2 =
                                    {
                                        MaQuyen: data[k].MaQuyen,
                                        TenQuyen: data[k].TenQuyen,
                                        QuyenCha: data[j].QuyenCha,
                                        DuocSuDung: data[k].DuocSuDung
                                    };
                                    objChild.Child2s.push(objChild2);
                                }
                            }
                            objParent.Childs.push(objChild);
                        }
                    }
                    self.QuyenChas.push(objParent);
                }
            }
        });
    }


    self.deleteTenNguoiDung = ko.observable();

    self.modalDelete = function (item) {
        self.deleteID(item.ID);
        self.deleteTenNguoiDung(item.TaiKhoan);
        $('#modalpopup_deleteND').modal('show');
    };
    self.xoaND = function (NguoiDung) {
        //ajaxHelper(HTNguoiDungs + "CheckAdmin?idnd=" + self.newNguoiDung().ID(), 'POST').done(function (data) {
        //    if (data) {
        //        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Tài khoản là Admin không thể xóa", "danger");
        //        return false;
        //    }
        //    else {
        //        ajaxHelper(HTNguoiDungs + 'DeleteHT_NguoiDung?id=' + self.newNguoiDung().ID(), 'GET').done(function (data) {
        //            if (data === "") {
        //                self.XoaLSNguoiDung();
        //                SearchNguoiDung();
        //                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Xóa người dùng thành công", "success");
        //            }
        //            else {
        //                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + data, "danger");
        //            }
        //        })
        //    }
        //})
    };
    self.XoaLSNguoiDung = function (taikhoan) {
        var objDiary = {
            ID_NhanVien: _IDNhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Quản lý người dùng",
            NoiDung: "Xóa người dùng có tài khoản : " + taikhoan,
            NoiDungChiTiet: "Xóa người dùng có tài khoản : " + taikhoan,
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
    self.showPopupAddND = function () {
        var insert = CheckRoleExist('NguoiDung_ThemMoi');
        if (!insert) {
            ShowMessage_Danger('Không có quyền thêm mới người dùng');
            return;
        }
        ajaxHelper(HTNguoiDungs + 'GioiHanSoNguoiDung', 'GET').done(function (data) {
            if (data) {
                ShowMessage_Danger('Cửa hàng đã đạt số người dùng quy định, không thể thêm mới');
                return;
            }

            vmThemMoiNguoiDung.ShowModalAdd();
        });
    }

    self.resetTextBox = function () {
        self.newVaiTro(new FormModel_VaiTro());
    }
    self.showpopupvaitro = function () {
        $('#modalPopuplg_VaiTro').modal('show');
        $('.addvaitro').removeClass("check");
        self.resetTextBox();
        arrMaQuyen = [];
        getallQuyen();
    }

    self.xoaNhomND = function (item) {
        self.selectedChange();
        $.ajax({
            type: "DELETE",
            url: HTNguoiDungs + "DeleteHT_NhomNguoiDung/" + self.selectedChange(),
            dataType: 'json',
            contentType: 'application/json',
            success: function (result) {
                $('#modalPopuplg_EditVaiTro').modal('hide');
                self.XoaLSNhomND(self.newVaiTro().TenNhom())
                getAllVaiTro();
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Xóa vai trò thành công", "success");
            },
            error: function (error) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vai trò đã được gán cho người dùng, không thể xóa", "danger");
            }
        })
    }
    self.XoaLSNhomND = function (tennhom) {
        var objDiary = {
            ID_NhanVien: _IDNhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Quản lý người dùng",
            NoiDung: "Xóa vai trò : " + tennhom,
            NoiDungChiTiet: "Xóa vai trò : " + tennhom,
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

    self.editVaiTro = function (item) {
        showVaitro();
        var idnguoidung = item.ID;
        self.selectedChange(idnguoidung);
        self.currentVaiTro(item);

        $(".editcheck").removeClass("check");
        $(".editcheck").removeClass("check1");
        var dem = 0;
        var lenght = 0;
        //self.loadLaiQuyen(item.ID);
        self.QuyenChas([]);
        arrMaQuyen = [];
        ajaxHelper(HTNguoiDungs + "GetAllQuyen", 'GET').done(function (data) {
            for (var i = 0; i < data.length; i++) {
                if (data[i].QuyenCha == "") {
                    var objParent = {
                        MaQuyen: data[i].MaQuyen,
                        TenQuyen: data[i].TenQuyen,
                        DuocSuDung: true,
                        Childs: [],
                    };
                    for (var j = 0; j < data.length; j++) {
                        if (data[j].MaQuyen !== data[i].MaQuyen && data[j].QuyenCha === data[i].MaQuyen) {
                            var objChild =
                            {
                                MaQuyen: data[j].MaQuyen,
                                TenQuyen: data[j].TenQuyen,
                                QuyenCha: data[i].QuyenCha,
                                DuocSuDung: data[j].DuocSuDung,
                                Child2s: []
                            };
                            for (var k = 0; k < data.length; k++) {
                                if (data[k].QuyenCha !== null && data[k].QuyenCha === data[j].MaQuyen) {
                                    var objChild2 =
                                    {
                                        MaQuyen: data[k].MaQuyen,
                                        TenQuyen: data[k].TenQuyen,
                                        QuyenCha: data[j].QuyenCha,
                                        DuocSuDung: data[k].DuocSuDung
                                    };
                                    objChild.Child2s.push(objChild2);
                                }
                            }
                            objParent.Childs.push(objChild);
                        }
                    }
                    self.QuyenChas.push(objParent);
                }
            }

            ajaxHelper(HTNguoiDungs + "GetHT_NhomNguoiDungEDit/" + idnguoidung, 'GET').done(function (data1) {
                self.newVaiTro().setdata(data1);
                for (var i = 0; i < self.QuyenChas().length; i++) {
                    for (var j = 0; j < self.QuyenChas()[i].Childs.length; j++) {
                        var child = self.QuyenChas()[i].Childs[j];
                        for (var z = 0; z < data1.HT_Quyen_NhomDTO.length; z++) {
                            if (child.MaQuyen === data1.HT_Quyen_NhomDTO[z].MaQuyen) {
                                child.DuocSuDung = true;
                                if ($.inArray(child.MaQuyen, arrMaQuyen) === -1) {
                                    arrMaQuyen.push(child.MaQuyen);
                                }
                            }
                        }
                        for (var k = 0; k < self.QuyenChas()[i].Childs[j].Child2s.length; k++) {
                            var chid2 = self.QuyenChas()[i].Childs[j].Child2s[k];
                            lenght = self.QuyenChas()[i].Childs[j].Child2s.length;
                            for (var h = 0; h < data1.HT_Quyen_NhomDTO.length; h++) {
                                var quyenDTO = data1.HT_Quyen_NhomDTO[h];
                                if (chid2.MaQuyen === quyenDTO.MaQuyen) {
                                    chid2.DuocSuDung = true;
                                    if ($.inArray(chid2.MaQuyen, arrMaQuyen) === -1) {
                                        arrMaQuyen.push(chid2.MaQuyen);
                                    }
                                    dem = dem + 1;
                                }
                            }
                            if (dem === lenght) {
                                child.DuocSuDung = true;
                            }
                        }
                        dem = 0;
                        lenght = 0;
                    }
                }
                self.QuyenEdit(self.QuyenChas());
               
                var listquyen = self.QuyenChas();
                listquyen.forEach(function (item1) {
                    var listquyenlvl2 = item1.Childs;
                   
                    item1.DuocSuDung ? $(item1.MaQuyen).prop('checked', true) : $(item1.MaQuyen).prop('checked', false);
                    var quyen2check = 0;


                    listquyenlvl2.forEach(function (item2) {
                      
                        var listquyenlvl3 = item2.Child2s;
                        var quyen3check = 0;
                        listquyenlvl3.forEach(function (item3) {
                            if (item3.DuocSuDung) {
                                $(item3.MaQuyen).prop('checked', true);
                                quyen3check++;
                            }
                            else {
                                $(item3.MaQuyen).prop('checked', false);
                            }
                        });
                        switch (quyen3check) {
                            case 0:
                                $('#editcheck' + item2.MaQuyen).prop('checked', false);
                                $('#editcheck' + item2.MaQuyen).removeClass('op-checkbox-square');

                                break;
                            case listquyenlvl3.length:
                                $('#editcheck' + item2.MaQuyen).prop('checked', true);
                                $('#editcheck' + item2.MaQuyen).removeClass('op-checkbox-square');
                                quyen2check++;
                                break;
                           
                            default:
                                $('#editcheck' + item2.MaQuyen).prop('checked', false);
                                $('#editcheck' + item2.MaQuyen).addClass('op-checkbox-square');
                                break;
                        }
                    });

                    switch (quyen2check) {
                        case 0:
                            $('#' + item1.MaQuyen).prop('checked', false);
                            $('#' + item1.MaQuyen).removeClass('op-checkbox-square');
                            break;
                        case listquyenlvl2.length:
                            $('#' + item1.MaQuyen).prop('checked', true);
                            $('#' + item1.MaQuyen).removeClass('op-checkbox-square');
                            quyen2check++;
                            break;
                     
                        default:
                            $('#' + item1.MaQuyen).prop('checked', false);
                            $('#' + item1.MaQuyen).addClass('op-checkbox-square');
                            break;
                    }

                });
                $("#role_edit").show();
                $("#role_edit").siblings().hide();
              
            });
        });
    }

    //self.ChangeNDByIDNhom = function (item) {
    //    self.selectedVaiTro();
    //}

    function ChangeNDByIDNhom(idnhomnd) {
        SearchNguoiDung();
    }

    self.ChangeNDByIDNhom = function () {
        ChangeNDByIDNhom(self.selectedChange());
    }

    self.ChangeDsQuyen = function (item) {
        var dem = 0;
        var lenght = 0;
        $('.loadgetallquyen').gridLoader();
        $("div[id ^= 'wait']").css('display', 'block');
        self.QuyenChas([]);
        arrMaQuyen = [];
        if (item.ID === '00000000-0000-0000-0000-000000000000') {
            self.QuyenEdit([]);
            $('.loadgetallquyen').gridLoader({ show: false });
            $('.khongphanquyencn').css('display', 'block');
        }
        else {
            $('.loadgetallquyen').gridLoader({ show: false });
            ajaxHelper(HTNguoiDungs + "GetAllQuyen", 'GET').done(function (data) {
                for (var i = 0; i < data.length; i++) {
                    if (data[i].QuyenCha == "") {
                        var objParent = {
                            MaQuyen: data[i].MaQuyen,
                            TenQuyen: data[i].TenQuyen,
                            DuocSuDung: true,
                            Childs: [],
                        }
                        for (var j = 0; j < data.length; j++) {
                            if (data[j].MaQuyen !== data[i].MaQuyen && data[j].QuyenCha === data[i].MaQuyen) {
                                var objChild =
                                {
                                    MaQuyen: data[j].MaQuyen,
                                    TenQuyen: data[j].TenQuyen,
                                    QuyenCha: data[i].QuyenCha,
                                    DuocSuDung: data[j].DuocSuDung,
                                    Child2s: []
                                };
                                for (var k = 0; k < data.length; k++) {
                                    if (data[k].QuyenCha !== null && data[k].QuyenCha === data[j].MaQuyen) {
                                        var objChild2 =
                                        {
                                            MaQuyen: data[k].MaQuyen,
                                            TenQuyen: data[k].TenQuyen,
                                            QuyenCha: data[j].QuyenCha,
                                            DuocSuDung: data[k].DuocSuDung
                                        };
                                        objChild.Child2s.push(objChild2);
                                    }
                                }
                                objParent.Childs.push(objChild);
                            }
                        }
                        self.QuyenChas.push(objParent);
                    }
                }
            });
            ajaxHelper(HTNguoiDungs + "GetHT_NhomNguoiDung?idnguoidung=" + item.IDNguoiDung + '&iddonvi=' + item.ID_DonVi, 'GET').done(function (data) {
                self.newVaiTro().setdata(data);
                for (var i = 0; i < self.QuyenChas().length; i++) {
                    for (var j = 0; j < self.QuyenChas()[i].Childs.length; j++) {
                        var child = self.QuyenChas()[i].Childs[j];
                        for (var z = 0; z < data.HT_Quyen_NhomDTO.length; z++) {
                            if (child.MaQuyen === data.HT_Quyen_NhomDTO[z].MaQuyen) {
                                child.DuocSuDung = true;
                                if ($.inArray(child.MaQuyen, arrMaQuyen) == -1) {
                                    arrMaQuyen.push(child.MaQuyen);
                                }
                            }
                        }
                        for (var k = 0; k < self.QuyenChas()[i].Childs[j].Child2s.length; k++) {
                            var chid2 = self.QuyenChas()[i].Childs[j].Child2s[k];
                            lenght = self.QuyenChas()[i].Childs[j].Child2s.length;
                            for (var h = 0; h < data.HT_Quyen_NhomDTO.length; h++) {
                                var quyenDTO = data.HT_Quyen_NhomDTO[h];
                                if (chid2.MaQuyen === quyenDTO.MaQuyen) {
                                    chid2.DuocSuDung = true;
                                    if ($.inArray(chid2.MaQuyen, arrMaQuyen) == -1) {
                                        arrMaQuyen.push(chid2.MaQuyen);
                                    }
                                    dem = dem + 1;
                                }
                            }
                            if (dem === lenght) {
                                child.DuocSuDung = true;
                            }
                        }
                        dem = 0;
                        lenght = 0;
                    }
                }

                for (var k = 0; k < self.QuyenChas().length; k++) {
                    var demtruechild = 0;
                    for (var i = 0; i < self.QuyenChas()[k].Childs.length; i++) {
                        var count = 0;
                        var countCheck = 0;
                        if (self.QuyenChas()[k].Childs[i].DuocSuDung == true) {
                            demtruechild = demtruechild + 1;
                        }
                        for (var j = 0; j < self.QuyenChas()[k].Childs[i].Child2s.length; j++) {
                            count = count + 1;
                            if (self.QuyenChas()[k].Childs[i].Child2s[j].DuocSuDung === true) {
                                countCheck = countCheck + 1;
                            }
                        }
                        if (count !== countCheck) {
                            $('#phanquyen' + item.ID).find('#allquyencheck' + self.QuyenChas()[k].Childs[i].MaQuyen).addClass('op-checkbox-square');
                        }
                        if (countCheck === 0) {
                            $('#phanquyen' + item.ID).find('#allquyencheck' + self.QuyenChas()[k].Childs[i].MaQuyen).removeClass('op-checkbox-square');
                        }
                    }
                    if (demtruechild === 0) {
                        self.QuyenChas()[k].DuocSuDung = false;
                    }
                }
                self.QuyenEdit(self.QuyenChas());
                $('.loadgetallquyen').gridLoader({ show: false });
                $('.khongphanquyencn').css('display', 'none');
            });
        }
    }
    function arr_diff(a1, a2) {

        var a = [], diff = [];

        for (var i = 0; i < a1.length; i++) {
            a[a1[i]] = true;
        }

        for (var i = 0; i < a2.length; i++) {
            if (a[a2[i]]) {
                delete a[a2[i]];
            } else {
                a[a2[i]] = true;
            }
        }

        for (var k in a) {
            diff.push(k);
        }

        return diff;
    }

    function getAllMaQuyen() {
        var arrMaQuyen=[];
        $('.op-main-role input').each(function () {
            var id = $(this).attr('id');
            var isCheck = $(this).is(':checked') || $(this).hasClass('.op-checkbox-square')
            id = id.replace("editcheck", "");
            if (isCheck == true) { arrMaQuyen.push(id) }


        })
      
      
    }
    self.SuaVaiTro = function (formElement) {
        var _tennhom = self.newVaiTro().TenNhom();
        var _idnhomnd = self.newVaiTro().ID();
        var VaiTro = {
            ID: _idnhomnd,
            TenNhom: _tennhom,
        };
        var _IDNhomNguoiDung = $('.idnhomnguoidung').text();
        var myData = {};
        myData.idnhom = _idnhomnd;
        myData.objVaiTro = VaiTro;
        myData.idnhanvien = _IDNhanVien;
        myData.iddonvi = _IDchinhanh;
        //myData.objQuyenNhom = arrMaQuyen;
        getAllMaQuyen()
        myData.objQuyenNhom = arrMaQuyen;

        //$('#wait').remove();
        $('.op-js-loadsuavt').gridLoader();
        $.ajax({
            url: HTNguoiDungs + "PutHTVaiTro",
            type: 'PUT',
            dataType: 'json',
            data: myData,
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (item) {
                getAllVaiTro();
                $('.op-js-loadsuavt').gridLoader({ show: false });
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật vai trò thành công", "success");
                $('#modalPopuplg_EditVaiTro').modal('hide');
                //SearchNguoiDung();
                //location.reload();
            },
            statusCode: {
                404: function () {
                    self.error("page not found");
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                $('#modalPopuplg_EditVaiTro').modal('hide');
            },
            complete: function () {
            }
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
            });

    }

    self.addVaiTro = function (formElement) {
        var _tennhom = self.newVaiTro().TenNhom();
        var _idnhomnd = self.newVaiTro().ID();

        if (_tennhom == null || _tennhom == "" || _tennhom == "undefined") {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng nhập tên vai trò", "danger");
            $('#txtTenNhomVaiTro').select();
            return false;
        }

        var VaiTro = {
            ID: _idnhomnd,
            TenNhom: _tennhom,
        };

        var myData = {};
        myData.idnhom = _idnhomnd;
        myData.idnhanvien = _IDNhanVien,
            myData.iddonvi = _IDchinhanh,
            myData.objVaiTro = VaiTro;
        myData.objQuyenNhom = arrMaQuyen;
        $('.op-js-loadsuavt').gridLoader();
        $.ajax({
            url: HTNguoiDungs + "PostHT_NhomNguoiDung",
            type: 'POST',
            dataType: 'json',
            data: myData,
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (item) {
                self.NhomNguoiDungs.unshift(item);
                $('.op-js-loadsuavt').gridLoader({ show: false });
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Thêm vai trò thành công", "success");
                $('#modalPopuplg_VaiTro').modal('hide');
                self.arrRole_filter(self.NhomNguoiDungs());
                $("#role_edit").show();
                $("#role-content").hide();
                $("#user-content").hide();
                $("#dummy-content").hide();
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

    self.Login = function (formElement) {
        var _taiKhoan = self.newNguoiDung().TaiKhoan();
        var _matKhau = self.newNguoiDung().MatKhau();
        if (_taiKhoan == null || _taiKhoan == "" || _taiKhoan == "undefined") {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng nhập tên tài khoản", "danger");
            $('#txtUser').focus();
            return false;
        }

        if (_matKhau == null || _matKhau == "" || _matKhau == "undefined") {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng nhập mật khẩu", "danger");
            $('#txtPassWord').focus();
            return false;
        }

        ajaxHelper(HTNguoiDungs + "Check_LoGin?tenTaiKhoan=" + _taiKhoan + '&matKhau=' + _matKhau, 'POST').done(function (data) {
            if (data) {
                window.location.href = '/Home/Index';
            }
            else {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Tên tài khoản hoặc mật khẩu không chính xác", "danger");
                $('#txtPassWord').focus();
            }
        })
    }

    self.clickLoadHD = function (item) {
       
        $(event.currentTarget).closest('div').find('.user-detail').show();
        self.getallQuyen(item);
        GetCurrentUser(item.ID);
        showNguoidung();
    }
self.NgungHoatDong = function () {
        ajaxHelper(HTNguoiDungs + "NgungHoatDong?id=" + self.currentUser().ID
            + '&idnhanvien=' + self.currentUser().ID_NhanVien
            + '&iddonvi=' + self.currentUser().ID_DonVi, 'GET').done(function () {
                SearchNguoiDung();
                ShowMessage_Success('Cập nhật người dùng thành công');
            })
    }

    self.ChoPhepHoatDong = function () {
        ajaxHelper(HTNguoiDungs + "ChoHoatDong?id=" + self.currentUser().ID
            + '&idnhanvien=' + self.currentUser().ID_NhanVien
            + '&iddonvi=' + self.currentUser().ID_DonVi, 'GET').done(function () {
                SearchNguoiDung();
                ShowMessage_Success('Cập nhật người dùng thành công');
            })
    }

    function getAllVaiTro() {
        ajaxHelper(HTNguoiDungs + "GetListVaiTro", 'GET').done(function (data) {
            self.NhomNguoiDungs(data);
            vmThemMoiNguoiDung.listData.NhomNguoiDungs = data;
            self.arrRole_filter(data);
        });
    }

    function GetCurrentUser(id) {
        var currentUser = $.grep(self.NguoiDung(), function (x) {
            return x.ID === id;
        });
        if (currentUser.length > 0) {
            currentUser[0].TenNhanVien = currentUser[0].TenNguoiDung;
            currentUser[0].ID_NhomNguoiDung = currentUser[0].IDNhomNguoiDung;
            self.currentUser(currentUser[0]);
        }
    }

    self.editND = function (item) {
        GetCurrentUser(item.ID);
        if (self.currentUser() !== undefined) {
            var nvien = {
                ID: item.ID_NhanVien,
                TenNhanVien: item.TenNguoiDung,
                MaNhanVien: item.MaNhanVien,
            }

            // remove & add again
            var lstNV = $.grep(self.NhanViens(), function (x) {
                return x.ID !== item.ID_NhanVien;
            });
            lstNV.unshift(nvien);
            vmThemMoiNguoiDung.listData.NhanViens = lstNV;

            vmThemMoiNguoiDung.ShowModalUpdate(self.currentUser());
        }
    }

    //export To Excel dmHangHoas
    var tableToExcel = (function () {
        var uri = 'data:application/vnd.ms-excel;base64,'
            , template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table>{table}</table></body></html>'
            , base64 = function (s) { return window.btoa(unescape(encodeURIComponent(s))) }
            , format = function (s, c) { return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; }) }
        return function (table, name) {
            if (!table.nodeType) table = document.getElementById(table)
            var ctx = { worksheet: name || 'Worksheet', table: table.innerHTML }
            window.location.href = uri + base64(format(template, ctx))
        }
    })()

    self.exportToExcelHangHoas = function () {
        tableToExcel('tblDanhMucHangHoa', 'dmHangHoas.xls');
    }

    // import from excel dmhanghoa
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
                var dataObjects = XLSX.utils.sheet_to_json(worksheet, { raw: true });
                if (dataObjects.length > 0) {

                }
            }
            oReq.send();
        }
    }

    self.importFromExcelHangHoas = function () {
        $("#fileLoader").click();
    }

    // Fetch the initial data.
    self.nguoidung_arrDonVi = ko.observableArray();
    
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


    $("#textFindND").keypress(function (e) {
        if (e.keyCode == 13) {
            self.currentPage(0);
            SearchNguoiDung();
        }
    });
    self.SearchNguoiDungClick = function () {
        SearchNguoiDung();
    }

    self.SearchNguoiDungFilter = function (item) {
        self.selectedChange(item.ID);
        SearchNguoiDung2();
    }
   
    function SearchNguoiDung(isGoToNext = false) {
        var findTK = localStorage.getItem('findTK');
        if (findTK !== null) {
            self.filter(findTK);
        }
        var txtMaHDon = self.filter();

        if (txtMaHDon === undefined) {
            txtMaHDon = "";
        }
        var trangthai = 1;
        if (self.Loc_HoatDong() == '0') {
            trangthai = 3;
        }
        if (self.Loc_HoatDong() == '2') {
            trangthai = 2;
        }
        if (self.Loc_HoatDong() == '1') {
            trangthai = 1;
        }
        $('.table_h').gridLoader();
        ajaxHelper(HTNguoiDungs + 'GetListNguoiDung_where?currentPage='
            + self.currentPage() + '&pageSize=' + self.pageSize() + '&idnhomnguoidung=' + self.selectedChange() +
            '&maHoaDon=' + txtMaHDon + '&trangthai=' + trangthai,
            'GET').done(function (data) {
                $('.table_h').gridLoader({ show: false });
                self.NguoiDung(data);

                //self.NguoiDungFilterbyRole(data);
                LoadHtmlGrid();

            });
        if (!isGoToNext) {
            // page count
            ajaxHelper(HTNguoiDungs + 'GetPageCountND_Where?currentPage=' + self.currentPage() + '&pageSize=' + self.pageSize()
                + '&idnhomnguoidung=' + self.selectedChange() +
                '&maHoaDon=' + txtMaHDon + '&trangthai=' + trangthai,
                'GET').done(function (data) {
                    $('.table_h').gridLoader({ show: false });
                    self.TotalRecord(data.TotalRecord);
                    self.PageCount(data.PageCount);
                });
        }
        localStorage.removeItem('findTK');
    }

    function SearchNguoiDung2(isGoToNext = false) {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('NguoiDung_XemDS', lc_CTQuyen) > -1) {
            var findTK = localStorage.getItem('findTK');
            if (findTK !== null) {
                self.filter(findTK);
            }
            var txtMaHDon = self.filter();

            if (txtMaHDon === undefined) {
                txtMaHDon = "";
            }
            var trangthai = 1;
            if (self.Loc_HoatDong() == '0') {
                trangthai = 3;
            }
            if (self.Loc_HoatDong() == '2') {
                trangthai = 2;
            }
            if (self.Loc_HoatDong() == '1') {
                trangthai = 1;
            }
            $('.table_h').gridLoader();
            ajaxHelper(HTNguoiDungs + 'GetListNguoiDung_where?currentPage=' + self.currentPage()
                + '&pageSize=' + self.pageSize() + '&idnhomnguoidung=' + self.selectedChange() +
                '&maHoaDon=' + txtMaHDon + '&trangthai=' + trangthai,
                'GET').done(function (data) {
                    $('.table_h').gridLoader({ show: false });

                    self.NguoiDungFilterbyRole(data);
                });
            if (!isGoToNext) {
                // page count
                ajaxHelper(HTNguoiDungs + 'GetPageCountND_Where?currentPage=' + self.currentPage() + '&pageSize=' + self.pageSize()
                    + '&idnhomnguoidung=' + self.selectedChange() +
                    '&maHoaDon=' + txtMaHDon + '&trangthai=' + trangthai,
                    'GET').done(function (data) {
                        $('.table_h').gridLoader({ show: false });
                        self.TotalRecord(data.TotalRecord);
                        self.PageCount(data.PageCount);
                    });
            }
            localStorage.removeItem('findTK');
        }
    }

    self.ResetCurrentPage = function () {
        self.currentPage(0);
        SearchNguoiDung();
    };
    self.PageResults = ko.computed(function () {
        if (self.NguoiDung() !== null) {

            self.fromitem((self.currentPage() * self.pageSize()) + 1);

            if (((self.currentPage() + 1) * self.pageSize()) > self.NguoiDung().length) {
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

    self.Loc_HoatDong.subscribe(function (newVal) {
        self.currentPage(0);
        SearchNguoiDung();
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
            SearchNguoiDung(true);
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
            SearchNguoiDung(true);
        }
    }

    self.EndPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.PageCount() - 1);
            SearchNguoiDung(true);
        }
    }
    self.GetClassHD = function (page) {
        return ((page.pageNumber - 1) === self.currentPage()) ? "click" : "";
    };
    //===============================
    // Load lai form lưu cache bộ lọc 
    // trên grid 
    //===============================
    function LoadHtmlGrid() {
        if (window.localStorage) {
            var current = localStorage.getItem('QLnguoidung');
            if (!current) {
                current = [];
                localStorage.setItem('QLnguoidung', JSON.stringify(current));
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

        var current = localStorage.getItem('QLnguoidung');
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
                if (i == current.length - 1) {
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
        localStorage.setItem('QLnguoidung', JSON.stringify(current));
    }

    PageLoad();

    $('#vmThemMoiNguoiDung').on('hidden.bs.modal', function () {
        if (vmThemMoiNguoiDung.saveOK) {
            for (let i = 0; i < self.NguoiDung().length; i++) {
                if (self.NguoiDung()[i].ID === vmThemMoiNguoiDung.newUser.ID) {
                    self.NguoiDung.splice(i, 1);
                    break;
                }
            }

            var newUser = vmThemMoiNguoiDung.newUser;
            var obj = {
                DangHoatDong: true,
                ID: newUser.ID,
                IDNhomNguoiDung: newUser.ID_NhomNguoiDung,
                ID_DonVi: newUser.ID_DonVi,
                ID_NhanVien: newUser.ID_NhanVien,
                ID_NhomNguoiDung: newUser.ID_NhomNguoiDung,
                LaAdmin: false,
                MaNhanVien: newUser.MaNhanVien,
                MatKhau: newUser.MatKhau,
                SoDuTaiKhoan: 0,
                TaiKhoan: newUser.TaiKhoan,
                TenDonVi: newUser.TenDonVi,
                TenNguoiDung: newUser.TaiKhoan,
                TenNhanVien: newUser.TenNhanVien,
                TenNhom: newUser.TenVaiTro,
                TrangThai: true,
                XemGiaVon: false,
            }
            self.NguoiDung.unshift(obj);
            self.currentUser(obj);
            self.getallQuyen(obj);
            // remove nv
            var lstNV = $.grep(vmThemMoiNguoiDung.listData.NhanViens, function (x) {
                return x.ID !== newUser.ID_NhanVien;
            });
            vmThemMoiNguoiDung.listData.NhanViens = lstNV;
        }
    })
};
vmodel = new ViewModel()
ko.applyBindings(vmodel, document.getElementById("IdQuanLyNguoiDung"));
function hidewait(o) {
    $('.' + o).append('<div id="wait"><img src="/Content/images/wait.gif" width="64" height="64" /><div class="happy-wait">' +
        ' </div>' +
        '</div>')
}

//check box
var arrMaQuyen = [];
function pushMaquyen(val) {
    if ($.inArray(val, arrMaQuyen) === -1) {
        arrMaQuyen.push(val);
    }
}
function removeMaquyen( val) {
    var index = arrMaQuyen.indexOf(val);
    if (index > -1) {
        arrMaQuyen.splice(index, 1);
    }
}

function getMaQuyenlvl3(obj) {
    var thisID = $(obj).attr('id');//id của quyền này
    let quyencha = $(obj).closest(".op-role-lvl-2");//quyền cha
    let checkboxquyencha = quyencha.find('h3 input.op-checkbox-lvl-2');//check box quyền cha của quyền này
    let soquyen = quyencha.find(".op-role-lvl-3").length;//tổng số quyền cùng mục với quyền này
    let checked = 0;//số checkbox cùng mục đã check
    $(obj).removeClass('op-checkbox-square');
    $(obj).is(':checked') ? pushMaquyen(thisID) : removeMaquyen(thisID);
    
    quyencha.find(".op-role-lvl-3").each(function () {
        if ($(this).find('input[type="checkbox"]').is(':checked')) {
            checked++;
        };
    });
    switch (checked) {
        case 0:
            checkboxquyencha.prop('checked', false);
            checkboxquyencha.removeClass('op-checkbox-square')
            break;
        case soquyen:
            checkboxquyencha.prop('checked', true);
            checkboxquyencha.removeClass('op-checkbox-square')
            break;
        default:
            checkboxquyencha.prop('checked', false);
            checkboxquyencha.addClass('op-checkbox-square')
            break;
    }
   
};
function CheckAllRoleLvl3(obj) {
    var isChecked = $(obj).is(":checked");
    let thisID = $(obj).attr('id').replace("editcheck", "");
    let childrole = $(obj).closest(".op-role-lvl-2").find(".op-role-lvl-3  input[type='checkbox']");
   
    let quyencha = $(obj).closest(".op-role-lvl-1");//quyền cha
    let checkboxquyencha = quyencha.children('h3').find(' label input[type="checkbox"]');//check box quyền cha của quyền này
    let soquyen = quyencha.find(".op-role-lvl-2").length;//tổng số quyền cùng mục với quyền này
    let checked = 0;//số checkbox cùng mục đã check

    //check về thuộc tính, loại bỏ op-checkbox-square
    $(obj).removeClass('op-checkbox-square');
    $(obj).is(':checked') ? pushMaquyen(thisID) : removeMaquyen(thisID);

    //check các quyền con dưới quyền này
    childrole.each(function () {
        $(this).prop('checked', isChecked);
        var childroleid = $(this).attr('id')
        isChecked ? pushMaquyen(childroleid) : removeMaquyen(childroleid);
    });
   
    
    ////check về thuộc tính, loại bỏ op-checkbox-square
    quyencha.find(".op-role-lvl-2").each(function () {
        if ($(this).find('h3 label input[type="checkbox"]').is(':checked')) {
            checked++;
        };
    
    })

    switch (checked) {
        case 0:
            checkboxquyencha.prop('checked', false);
            checkboxquyencha.removeClass('op-checkbox-square')
            break;
        case soquyen:
            checkboxquyencha.prop('checked', true);
            checkboxquyencha.removeClass('op-checkbox-square')
            break;
        default:
            checkboxquyencha.prop('checked', false);
            checkboxquyencha.addClass('op-checkbox-square')
            break;
    }
    
}
function CheckAllRoleLvl2(obj) {
    var isChecked = $(obj).is(":checked");
    let thisID = $(obj).attr('id').replace("editcheck", "");
    $(obj).removeClass('op-checkbox-square');
    $(obj).is(':checked') ? pushMaquyen(thisID) : removeMaquyen(thisID);
}
function checkAllRole(ele) {
    var thisID = $(ele).attr('id');
    var isChecked = $(ele).is(':checked');

    $(ele).removeClass("op-checkbox-square");
    var inputEle = $(ele).closest('.op-role-lvl-1').find('input');
    inputEle.each(function () {
        $(this).prop('checked', isChecked);;
        isChecked == true ? pushMaquyen($(this).attr('id').replace("editcheck", "")) : removeMaquyen($(this).attr('id').replace("editcheck", ""))
    })

}
function CheckMainRole(ele) {
    let obj = $(ele);
    var Sub = obj.parent().parent().next().find("li");
    if (obj.is(':checked')) {
       
        Sub.each(function () {
        
            if (!($(this).find("input.subcheck").is(':checked'))) {
                $(this).find("input.subcheck").click();
            }

        });
    } else {
       
        Sub.each(function () {
     
            if (($(this).find("input.subcheck").is(':checked'))) {
                $(this).find("input.subcheck").click();

            }

        });
    }

}
function showNguoidung() {
    $('#user_edit').show();
    $('#user_edit').siblings().hide();
}
function showVaitro() {
    $('#user_edit').show();
    $('#user_edit').siblings().hide();
}

function closePaneContent() {
    $('#dummy-content').show();
    $('#dummy-content').siblings().hide();
} 

