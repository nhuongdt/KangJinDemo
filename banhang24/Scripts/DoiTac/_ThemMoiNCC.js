var PartialVendorGroup = function () {
    var self = this;
    self.ID = ko.observable();
    self.TenNhomDoiTuong = ko.observable();
    self.LoaiDoiTuong = 1;
    self.GhiChu = ko.observable();

    self.SetData = function (item) {
        self.ID(item.ID);
        self.TenNhomDoiTuong(item.TenNhomDoiTuong);
        self.GhiChu(item.GhiChu);
    }
}


var FormModel_NewVendor = function () {
    var self = this;
    self.ID = ko.observable();
    self.MaDoiTuong = ko.observable();
    self.TenDoiTuong = ko.observable();
    self.ID_NhomDoiTuong = ko.observable();
    self.ID_QuanHuyen = ko.observable();
    self.ID_TinhThanh = ko.observable();
    self.LaCaNhan = ko.observable();
    self.Email = ko.observable();
    self.DiaChi = ko.observable();
    self.DienThoai = ko.observable();
    self.MaSoThue = ko.observable();
    self.GhiChu = ko.observable();

    self.SetData = function (item) {
        self.ID(item.ID);

        if (item.ID_NhomDoiTuong !== null && item.ID_NhomDoiTuong.indexOf('0000') === -1) {
            let idNhom = Remove_LastComma(item.ID_NhomDoiTuong).toLowerCase();
            self.ID_NhomDoiTuong(idNhom);
        }
        else {
            self.ID_NhomDoiTuong(undefined);
        }

        if (item.ID_QuanHuyen !== null && item.ID_QuanHuyen !== const_GuidEmpty) {
            self.ID_QuanHuyen(item.ID_QuanHuyen);
        }
        else {
            self.ID_QuanHuyen(undefined);
        }

        if (item.ID_TinhThanh !== null && item.ID_TinhThanh !== const_GuidEmpty) {
            self.ID_TinhThanh(item.ID_TinhThanh);
        }
        else {
            self.ID_TinhThanh(undefined);
        }

        self.MaDoiTuong(item.MaDoiTuong);
        self.TenDoiTuong(item.TenDoiTuong);
        self.Email(item.Email);
        self.DiaChi(item.DiaChi);
        self.DienThoai(item.DienThoai);
        self.GhiChu(item.GhiChu);
        self.MaSoThue(item.MaSoThue);
    }
}

var PartialView_NewVendor = function () {
    var self = this;
    var DMDoiTuongUri = '/api/DanhMuc/DM_DoiTuongAPI/';
    var DMNhomDoiTuongUri = '/api/DanhMuc/DM_NhomDoiTuongAPI/';
    var user = $('#txtUserLogin').val(); // get at ViewBag
    var idNhanVien = $('.idnhanvien').text();
    var idDonVi = $('#hd_IDdDonVi').val();
    var sLoai = 'nhà cung cấp';
    var today = new Date();
    self.TinhThanhs = ko.observableArray();
    self.NhomDoiTuongs = ko.observableArray();// used at modal _ThemMoiNCC
    self.QuanHuyens = ko.observableArray();
    self.DoAction = ko.observable(0); // assign = true when delete/insert/update finished
    self.AddStatus = ko.observable(false);// insert (true), update(false)
    self.booleanAddNhomDT = ko.observable(false);
    self.booleanAdd = ko.observableArray(false);
    self.IsAddAtModal = ko.observable(false);
    self.CustomerDoing = ko.observableArray();// doing insert/update
    self.GroupDoing = ko.observableArray();
    self.newDoiTuong = ko.observable(new FormModel_NewVendor());
    self.newNhomDoiTuong = ko.observable(new PartialVendorGroup());

    function Enable_btnSaveNhomDT() {
        document.getElementById("btnLuuNhomDoiTuong").disabled = false;
        document.getElementById("btnLuuNhomDoiTuong").lastChild.data = "Lưu";
    }

    // add NhomNCC in modal 
    self.showpopAddNhomNCC_popup = function () {
        self.IsAddAtModal(true);
        self.booleanAddNhomDT(true);
        self.newNhomDoiTuong(new PartialVendorGroup());
        $('#modalNhomNCC').modal('show');
    };

    // add nhom ncc
    self.addNhomDoiTuong = function () {
        self.DoAction(2);//  insert/update NhomNCC

        document.getElementById("btnLuuNhomDoiTuong").disabled = true;
        document.getElementById("btnLuuNhomDoiTuong").lastChild.data = " Đang lưu";
        var _id = self.newNhomDoiTuong().ID();
        var _tenNhomDoiTuong = self.newNhomDoiTuong().TenNhomDoiTuong();
        var _ghiChu = self.newNhomDoiTuong().GhiChu();

        if (_tenNhomDoiTuong === '' || _tenNhomDoiTuong === undefined) {
            ShowMessage_Danger('Vui lòng nhập tên nhóm ' + sLoai);
            Enable_btnSaveNhomDT();
            return;
        }

        var DM_NhomDoiTuong = {
            ID: _id,
            TenNhomDoiTuong: _tenNhomDoiTuong,
            LoaiDoiTuong: 2,
            GhiChu: _ghiChu,
            TenNhomDoiTuong_KhongDau: locdau(_tenNhomDoiTuong),
            TenNhomDoiTuong_KyTuDau: GetChartStart(_tenNhomDoiTuong),
            NguoiTao: user,
        };
        if (self.booleanAddNhomDT() === true) {
            $.ajax({
                data: DM_NhomDoiTuong,
                url: DMNhomDoiTuongUri + "PostDM_NhomDoiTuong",
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (item) {
                    DM_NhomDoiTuong.ID = item.ID;
                    self.NhomDoiTuongs.unshift(item);
                    if (self.IsAddAtModal()) {
                        self.newDoiTuong().ID_NhomDoiTuong(item.ID);
                    }
                    self.GroupDoing(DM_NhomDoiTuong);
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    Enable_btnSaveNhomDT();
                },
                complete: function () {
                    $("#modalNhomNCC").modal("hide");
                    Enable_btnSaveNhomDT();
                    ShowMessage_Success('Thêm mới nhóm ' + sLoai + ' thành công');
                }
            })
        }
        // edit
        else {
            DM_NhomDoiTuong.NguoiSua = user;
            var myData = {
                id: _id,
                objNhomDoiTuong: DM_NhomDoiTuong,
            };
            self.GroupDoing(DM_NhomDoiTuong);

            $.ajax({
                url: DMNhomDoiTuongUri + "PutDM_NhomDoiTuong",
                type: 'PUT',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: myData,
                success: function () {
                    ShowMessage_Success('Cập nhật nhóm ' + sLoai + ' thành công');
                    // only at modal NCC
                    for (var i = 0; i < self.NhomDoiTuongs().length; i++) {
                        if (self.NhomDoiTuongs()[i].ID === _id) {
                            self.NhomDoiTuongs.remove(self.NhomDoiTuongs()[i]);
                            break;
                        }
                    }
                    self.NhomDoiTuongs.push(DM_NhomDoiTuong);
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    ShowMessage_Danger('Cập nhật nhóm ' + sLoai + ' thất bại');
                },
                complete: function () {
                    Enable_btnSaveNhomDT();
                    $("#modalNhomNCC").modal("hide");
                }
            })
        }
    }

    self.newDoiTuong().ID_TinhThanh.subscribe(function (newValue) {
        if (newValue !== undefined) {
            getListQuanHuyen(newValue);
            self.newDoiTuong().ID_TinhThanh(newValue);
        }
    });

    function getListQuanHuyen(id) {
        if (id !== undefined) {
            ajaxHelper(DMDoiTuongUri + "GetListQuanHuyen?idTinhThanh=" + id, 'GET').done(function (x) {
                if (x.data.length > 0) {
                    self.QuanHuyens(x.data);
                }
            });
        }
    }

    self.newDoiTuong().ID_QuanHuyen.subscribe(function (newVal) {
        self.newDoiTuong().ID_QuanHuyen(newVal);
    });

    self.filterProvince = function (item, inputString) {
        var itemSearch = locdau(item.TenTinhThanh);
        var locdauInput = locdau(inputString);
        // nen bat chat khong cho nhap dau cach o MaKH
        var arr = itemSearch.split(/\s+/);
        var sThreechars = '';

        for (var i = 0; i < arr.length; i++) {
            sThreechars += arr[i].toString().split('')[0];
        }

        return itemSearch.indexOf(locdauInput) > -1 ||
            sThreechars.indexOf(locdauInput) > -1;
    }

    self.filterDistrict = function (item, inputString) {

        var itemSearch = locdau(item.TenQuanHuyen);
        var locdauInput = locdau(inputString);
        // nen bat chat khong cho nhap dau cach o MaKH
        var arr = itemSearch.split(/\s+/);
        var sThreechars = '';

        for (var i = 0; i < arr.length; i++) {
            sThreechars += arr[i].toString().split('')[0];
        }
        return itemSearch.indexOf(locdauInput) > -1 ||
            sThreechars.indexOf(locdauInput) > -1;
    }

    function Enable_btnSaveDoiTuong() {
        document.getElementById("btnLuuDoiTuong").disabled = false;
        document.getElementById("btnLuuDoiTuong").lastChild.data = "Lưu";
    }

    // insert/update Nha cung cap
    self.addKH_NCC = function (formElement) {
        self.DoAction(1);
        document.getElementById("btnLuuDoiTuong").disabled = true;
        document.getElementById("btnLuuDoiTuong").lastChild.data = " Đang lưu";

        var _id = self.newDoiTuong().ID();
        var _tenDoiTuong = self.newDoiTuong().TenDoiTuong();
        var _idTinhThanh = self.newDoiTuong().ID_TinhThanh();
        var _idQuanHuyen = self.newDoiTuong().ID_QuanHuyen();
        var _maDT = self.newDoiTuong().MaDoiTuong();
        var _laCaNhan = self.newDoiTuong().LaCaNhan();
        var _idNhomDT = self.newDoiTuong().ID_NhomDoiTuong();

        var msgCheck = CheckInput(self.newDoiTuong());
        if (msgCheck !== '') {
            ShowMessage_Danger(msgCheck);
            Enable_btnSaveDoiTuong();
            return false;
        }

        var DM_DoiTuong = {
            ID: _id,
            ID_NhomDoiTuong: null, // not use this field
            MaDoiTuong: self.newDoiTuong().MaDoiTuong(),
            TenDoiTuong: _tenDoiTuong,
            TenDoiTuong_KhongDau: locdau(_tenDoiTuong),
            TenDoiTuong_ChuCaiDau: GetChartStart(_tenDoiTuong),
            DienThoai: self.newDoiTuong().DienThoai(),
            Email: self.newDoiTuong().Email(),
            DiaChi: self.newDoiTuong().DiaChi(),
            GioiTinhNam: true,
            MaSoThue: self.newDoiTuong().MaSoThue(),
            LoaiDoiTuong: 2,
            GhiChu: self.newDoiTuong().GhiChu(),
            LaCaNhan: _laCaNhan,
            ID_TinhThanh: _idTinhThanh,
            ID_QuanHuyen: _idQuanHuyen,
            ID_DonVi: idDonVi,
            NguoiTao: user, // user dang nhap
            ID_NhanVienPhuTrach: idNhanVien, // default NVPhuTrach = NV login
        };
        console.log('DM_DoiTuong', DM_DoiTuong, 'idNhom ', _idNhomDT);

        if (navigator.onLine) {
            // insert Nha cung cap
            if (self.booleanAdd() === true) {

                $.ajax({
                    data: DM_DoiTuong,
                    url: DMDoiTuongUri + "PostDM_DoiTuong",
                    type: 'POST',
                    async: true,
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (obj) {
                        if (obj.res === true) {
                            var item = obj.data;
                            // insert nhomNCC in DM_DoiTuong_Nhom (one group)
                            var lstDM_DoiTuong_Nhom = [];
                            var tenNhom = 'Nhóm mặc định';
                            // 0: Nhom mac dinh
                            if (_idNhomDT !== null && _idNhomDT !== undefined && _idNhomDT !== 0) {
                                var objDTNhom = {
                                    ID_DoiTuong: item.ID,
                                    ID_NhomDoiTuong: _idNhomDT,
                                }
                                lstDM_DoiTuong_Nhom.push(objDTNhom);
                                self.Insert_ManyNhom(lstDM_DoiTuong_Nhom);

                                // get tenNhom 
                                var itemNhom = $.grep(self.NhomDoiTuongs(), function (x) {
                                    return x.ID === _idNhomDT;
                                });

                                if (itemNhom.length > 0) {
                                    tenNhom = itemNhom[0].TenNhomDoiTuong;
                                }
                            }

                            DM_DoiTuong.TenNhomDT = tenNhom;
                            DM_DoiTuong.ID_NhomDoiTuong = _idNhomDT;
                            DM_DoiTuong.ID = item.ID;
                            DM_DoiTuong.MaDoiTuong = item.MaDoiTuong;
                            self.CustomerDoing(DM_DoiTuong);

                            ShowMessage_Success("Thêm mới " + sLoai + " thành công");
                        }
                        else {
                            ShowMessage_Danger(obj.mes);
                        }
                        $("#modalPopuplg_NCC").modal("hide");
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        ShowMessage_Danger('Thêm mới ' + sLoai + ' thất bại');
                    },
                    complete: function () {
                        Enable_btnSaveDoiTuong();
                    }
                })
            }
            // update Nha cung cap
            else {
                DM_DoiTuong.NguoiSua = user;
                var myData = {
                    id: _id,
                    objDoiTuong: DM_DoiTuong,
                };

                $.ajax({
                    url: DMDoiTuongUri + "PutDM_DoiTuong",
                    type: 'PUT',
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    data: myData,
                    success: function (obj) {
                        if (obj.res === true) {
                            var item = obj.data;
                            // update DM_DoiTuong_Nhom (many Group)
                            var lstDM_DoiTuong_Nhom = [];
                            var tenNhom = 'Nhóm mặc định';
                            if (_idNhomDT !== null && _idNhomDT !== undefined && _idNhomDT !== 0) {
                                var objDTNhom = {
                                    ID_DoiTuong: item.ID,
                                    ID_NhomDoiTuong: _idNhomDT,
                                }
                                lstDM_DoiTuong_Nhom.push(objDTNhom);

                                // get tenNhom 
                                var itemNhom = $.grep(self.NhomDoiTuongs(), function (x) {
                                    return x.ID === _idNhomDT;
                                });

                                if (itemNhom.length > 0) {
                                    tenNhom = itemNhom[0].TenNhomDoiTuong;
                                }
                                self.Update_ManyNhom(lstDM_DoiTuong_Nhom, false);
                            }

                            DM_DoiTuong.MaDoiTuong = item.MaDoiTuong;
                            DM_DoiTuong.TenNhomDT = tenNhom;
                            DM_DoiTuong.ID_NhomDoiTuong = _idNhomDT;

                            // bind at lst NCC
                            self.CustomerDoing(DM_DoiTuong);

                            Enable_btnSaveDoiTuong();
                            $("#modalPopuplg_NCC").modal("hide");
                        }
                        else {
                            ShowMessage_Danger(obj.mes);
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        ShowMessage_Danger('Cập nhật nhà cung cấp thất bại');
                    },
                    complete: function () {
                        Enable_btnSaveDoiTuong();
                    }
                })
            }
        }
        else {
            self.CustomerDoing(DM_DoiTuong);
        }
        $('.line-right').height(0).css("margin-top", "0px");
    }

    function CheckInput(obj) {

        var sReturn = '';
        var id = obj.ID();
        var maDT = obj.MaDoiTuong();
        var tenDT = obj.TenDoiTuong();
        var email = obj.Email();
        var phone = obj.DienThoai();
        var idTinhThanh = obj.ID_TinhThanh();
        var idQuanHuyen = obj.ID_QuanHuyen();

        if (tenDT === null || tenDT === "" || tenDT === undefined) {
            sReturn = 'Vui lòng nhập tên ' + sLoai + '  <br />';
        }

        // check MaKhachHang nhập kí tự đặc biệt 
        if (CheckChar_Special(maDT)) {
            sReturn += 'Mã ' + sLoai + ' không được chứa kí tự đặc biệt <br />';
        }

        if (email !== '' && email !== undefined && email !== null) {
            var re = /^(([^<>()[\]\\.,;:\s@#ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴ\"]+(\.[^<>()[\]\\.,;:\s@#ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴ\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/i;
            var valReturn = re.test(email);
            if (valReturn === false) {
                sReturn += 'Email không hợp lệ <br />';
            }
        }

        if (Is_undefined_empty_GuidEmpty(idTinhThanh) === false) {
            var itemTT = $.grep(self.TinhThanhs(), function (item) {
                return item.ID === idTinhThanh;
            });
            if (itemTT.length === 0) {
                sReturn += 'Tỉnh thành không tồn tại trong hệ thống <br />';
            }
        }

        if (Is_undefined_empty_GuidEmpty(idQuanHuyen) === false) {
            var itemQH = $.grep(self.QuanHuyens(), function (item) {
                return item.ID === idQuanHuyen;
            });
            if (itemQH.length === 0) {
                sReturn += 'Quận huyện không tồn tại trong hệ thống <br />';
            }
        }
        return sReturn;
    }

    self.Insert_ManyNhom = function (lstNhom) {
        var myData = {};
        myData.lstDM_DoiTuong_Nhom = lstNhom;

        ajaxHelper(DMDoiTuongUri + 'PostDM_DoiTuong_Nhom', 'POST', myData).done(function (x) {
        }).fail(function () {
            ShowMessage_Danger("Thêm mới " + sLoai + " thất bại");
        })
    }

    self.Update_ManyNhom = function (lstNhom, isMoveGroup) {
        lstNhom = $.unique(lstNhom);

        var myData = {
            lstDM_DoiTuong_Nhom: lstNhom
        };
        ajaxHelper(DMDoiTuongUri + 'PutDM_DoiTuong_Nhom', 'POST', myData).done(function (x) {

        }).fail(function () {
            ShowMessage_Danger("Cập nhật " + sLoai + " thất bại");
        })
    }

    self.ValidateEmail = function (item) {
        if (item.Email() !== '' && item.Email() !== undefined && item.Email() !== null) {
            var re = /^(([^<>()[\]\\.,;:\s@#ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴ\"]+(\.[^<>()[\]\\.,;:\s@#ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴ\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/i;
            var valReturn = re.test(item.Email().trim());
            if (valReturn === false) {
                ShowMessage_Danger('Email không hợp lệ');
                self.checkEmail(false);
            }
            else {
                self.checkEmail(true);
            }
        }
    }
}

