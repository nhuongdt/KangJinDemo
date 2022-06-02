

var vmmodalEditPhieuPhanca = new Vue({
    el: '#modalEditPhieuPhanca',
    data: {
        isNew: true,
        loadding: false,
        error:'',
        URL_APINhanSu:'/api/DanhMuc/NS_NhanSuAPI/',
        data: {
            ID: null,
            MaPhieu: null,
            TuNgay: null,
            DenNgay: null,
            TrangThai: 1,
            LoaiPhanCa: "1",
            CaTuanId: "1",
            GhiChu: null
        },
        listdata: {
            calamviec: [],
            calamvieccodinh: [],
            chinhanh: [],
            chinhanhid: $('#hd_IDdDonVi').val(),
            phongban: [],
            nhanvien: [],
            nhanvienselect: [],
            searchpb: '',
            searchnv: '',
            searchnvac: '',
            checkallnv: false,
            checkallnvac: false,
        },
        listcatuan: [
            { key: '1', value: [] },
            { key: '2', value: [] },
            { key: '3', value: [] },
            { key: '4', value: [] },
            { key: '5', value: [] },
            { key: '6', value: [] },
            { key: '0', value: [] },

        ],
        chitietphieu: {},
        chinhanhidold: null,
        loadfirst: 0,
        typeloaica: { CaCoDinh: null, CaTuan: null },
        typeloaicachar: { CaCoDinh: null, CaTuan: null },
        role: {
            NhanSu: false,
        },
    },
    methods: {
        GetListRole: function () {
            var self = this;
            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetRoleNhanSu", function (data) {
                if (data.res) {
                    self.role.NhanSu = data.dataSoure;
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });
        },
        // Lấy loại phân ca để so sánh
        GetLoaiCa: function () {
            var self = this;
            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetListLoaiCa", function (data) {
                if (data.res) {
                    self.typeloaicachar = data.dataSoure.newchar;
                    self.typeloaica = data.dataSoure.newnumber;
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });
        },

        AddNew: function () {
            var self = this;
            self.isNew = true;
            self.error = '';
            self.data = {
                ID: null,
                MaPhieu: null,
                TuNgay: null,
                DenNgay: null,
                TrangThai: 1,
                LoaiPhanCa: "3",
                CaTuanId: "1",
                GhiChu: null
            };
            self.listcatuan = [
                { key: '1', value: [] },
                { key: '2', value: [] },
                { key: '3', value: [] },
                { key: '4', value: [] },
                { key: '5', value: [] },
                { key: '6', value: [] },
                { key: '0', value: [] },

            ];
            self.listdata = {
                calamviec: [],
                calamvieccodinh: [],
                chinhanh: [],
                chinhanhid: null,
                phongban: [],
                nhanvien: [],
                nhanvienselect: [],
                searchpb: '',
                searchnv: '',
                searchnvac: '',
                checkallnv: false,
                checkallnvac: false,
            };
            self.chinhanhidold = $('#hd_IDdDonVi').val();
            self.GetListData();
            $('#modalThemMoiPhieuPhanCa').modal('show');
        },

        Update: function (item, list) {
            var self = this;
            self.isNew = false;
            self.error = '';
            self.data = {
                ID: item.ID,
                MaPhieu: item.MaPhieu,
                TuNgay: item.TuNgay,
                DenNgay: item.DenNgay,
                TrangThai: item.TrangThai,
                LoaiPhanCa: item.LoaiPhanCa.toString(),
                CaTuanId: "1",
                GhiChu: item.GhiChu
            };
            self.chinhanhidold = item.ID_DonVi;
            self.chitietphieu = list;
            self.loadfirst = 0;
            self.GetListData();
            $('#modalThemMoiPhieuPhanCa').modal('show');
        },

        GetListData: function () {
            var self = this;
            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetListChiNhanhNhanVien?id=" + $('.idnhanvien').text(), function (data) {
                if (data.res) {
                    self.listdata.chinhanh = data.dataSoure;
                    self.GetPhongBan(self.chinhanhidold);
                    self.SelectNhanVienByPhongBan(self.chinhanhidold, true);
                    self.GetCaLamViec(self.chinhanhidold);
                    self.listdata.chinhanhid = self.chinhanhidold;
                    $('#modalThemMoiPhieuPhanCa #selectchinhanh').val(self.chinhanhidold);
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });
        },
        // Change chi nhánh
        SelectChiNhanh: function () {
            if (this.listdata.chinhanhid !== undefined && this.listdata.chinhanhid !== null) {
                this.GetPhongBan(this.listdata.chinhanhid);
                this.GetCaLamViec(this.listdata.chinhanhid);
                self.listdata.nhanvienselect = [];
                self.listdata.nhanvien = [];
                this.SelectNhanVienByPhongBan(this.listdata.chinhanhid, true);
            }
        },

        // Get phòng ban theo chi nhánh
        GetPhongBan: function (id) {
            var self = this;
            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetListPhongBanTheoChiNhanh?id=" + id, function (data) {
                if (data.res) {
                    self.listdata.phongban = data.dataSoure;
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }

            });
        },

        // Get ca theo chi nhánh
        GetCaLamViec: function (id) {
            var self = this;
            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetListCaTheoChiNhanh?id=" + id, function (data) {
                if (data.res) {
                    self.listdata.calamviec = data.dataSoure.filter(x => x.TrangThai!==0);
                    self.listdata.calamvieccodinh = commonStatisJs.CopyArray(data.dataSoure);
                    // Khi update phiếu
                    if (self.loadfirst !== 2 && !self.isNew) {
                        self.loadfirst += 1;
                        if (self.data.LoaiPhanCa === self.typeloaicachar.CaTuan) {
                            var resultcatuan = self.chitietphieu.modelChiTietCa;
                            for (var i = 0; i < resultcatuan.length; i++) {
                                var model = self.listcatuan.filter(o => o.key === resultcatuan[i].GiaTri.toString());
                                if (model.length > 0) {
                                    var list = resultcatuan[i].listCa.map(o => o.IdCa);
                                    if (model[0].key === '1') {
                                        self.listdata.calamviec.filter(o => list.contains(o.Id)).map(o => o.Checked = true)
                                        model[0].value = self.listdata.calamviec.filter(o => list.contains(o.Id));
                                    }
                                    else {
                                        model[0].value = commonStatisJs.CopyArray(self.listdata.calamviec.filter(o => list.contains(o.Id)));
                                    }
                                }
                            }
                        }
                        else {
                            var listid = self.chitietphieu.modelChiTietCa.map(o => o.IdCa);
                            self.listdata.calamvieccodinh.filter(o => listid.contains(o.Id)).map(o => o.Checked = true);
                        }
                    }
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }

            });
        },

        CheckGoiTrung_CaLamViec: function () {
            var self = this;
            var mes = '';
            var lstChecked = self.listdata.calamvieccodinh.filter(x => x.Checked === true);
            if (lstChecked.length > 0) {
                let giovao = lstChecked[0].GioVao;
                let giora = lstChecked[0].GioRa;
                for (let i = 1; i < lstChecked.length; i++) {
                    if (lstChecked[i].GioVao < giora && giovao < lstChecked[i].GioRa) {
                        mes = 'Ca làm việc bị gối trùng thời gian';
                        break;
                    }
                }
                if (mes !== '') {
                    commonStatisJs.ShowMessageDanger(mes);
                    return false;
                }
            }
            return true;
        },

        ClickSelectCaCoDinh: function (item) {
            var self = this;
            item.Checked = !item.Checked;
            self.CheckGoiTrung_CaLamViec();
        },

        ClearSearchPB: function () {
            var self = this;
            self.listdata.searchpb = ''
        },
        // Get nhân viên theo phòng ban hay là chi nhánh nếu  isAll=true
        SelectNhanVienByPhongBan: function (id, isAll = false) {
            var $this = $(event.currentTarget);
            var self = this;
            self.listdata.searchpb = $this.find('span').html()
            var url = "/api/DanhMuc/NS_NhanSuAPI/GetListNhanVienTheoPhongBan";
            if (isAll) {
                url = "/api/DanhMuc/NS_NhanSuAPI/GetListNhanVienTheoChiNhanh";
            }
            $.getJSON(url + "?id=" + id, function (data) {
                if (data.res) {
                    self.listdata.nhanvien = data.dataSoure;
                    var list = self.listdata.nhanvienselect.map(function (item) { return item.Id });
                    self.listdata.nhanvien.filter(o => list.contains(o.Id)).map(c => c.Active = false);
                    if (self.loadfirst !== 2 && !self.isNew) {
                        self.loadfirst += 1;
                        var listid = self.chitietphieu.modelNhanVienCa.map(o => o.ID);
                        var resultnhanvienca = self.listdata.nhanvien.filter(o => listid.contains(o.Id));
                        resultnhanvienca.map(o => o.Active = false);
                        resultnhanvienca.map(o => o.IsNew = false);
                        self.listdata.nhanvienselect = commonStatisJs.CopyArray(resultnhanvienca);
                    }
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }

            });
        },

        ChangDatetime: function (date, type) {
            var sel = this;
            if (type === 1) {
                sel.data.TuNgay = date;
            }
            else {
                sel.data.DenNgay = date;
            }
        },

        evensearch1: function (o) {
            return commonStatisJs.convertVieToEng(o.text).indexOf(commonStatisJs.convertVieToEng(this.listdata.searchpb)) >= 0;
        },

        evensearch2: function (o) {
            return o.children.length > 0 && o.children.some(this.evensearch1);
        },

        // Xử lý chọn xóa nhân viên để áp dụng
        NextNhanVien: function (item) {
            var self = this;
            item.Checked = false;
            item.Active = false;
            self.listdata.nhanvienselect.push(item);
        },

        PrevNhanVien: function (item) {
            var self = this;
            var model = self.listdata.nhanvien.filter(o => o.Id === item.Id);
            if (model.length > 0) {
                model[0].Checked = false;
                model[0].Active = true;
            }
            self.listdata.nhanvienselect = self.listdata.nhanvienselect.filter(o => o.Id !== item.Id);
        },

        NextListNhanVien: function () {
            var self = this;
            var datanext = self.listdata.nhanvien.filter(o => o.Checked && o.Active);
            datanext.map(o => o.Checked = false);
            datanext.map(o => o.Active = false);
            self.listdata.nhanvienselect = self.listdata.nhanvienselect.concat(datanext);
            self.listdata.checkallnv = false;
        },

        PrevListNhanVien: function () {
            var self = this;
            var dataprev = self.listdata.nhanvienselect.filter(o => o.Checked && o.IsNew).map(function (item) { return item.Id });
            var resul = self.listdata.nhanvien.filter(o => dataprev.contains(o.Id));
            resul.map(c => c.Active = true);
            resul.map(c => c.Checked = false);
            self.listdata.nhanvienselect = self.listdata.nhanvienselect.filter(o => !dataprev.contains(o.Id));
            self.listdata.checkallnvac = false;
        },

        // Chọn ca cho loại ca thứ
        ChangeCaNhanVien: function (item) {
            var self = this;
            item.Checked = !item.Checked;
            var resul = self.listcatuan.filter(o => o.key === self.data.CaTuanId);
            if (resul.length > 0) {

                if (resul[0].value.some(o => o.Id === item.Id)) {
                    resul[0].value = resul[0].value.filter(o => o.Id !== item.Id);
                }
                else {
                    resul[0].value.push(item);
                }
            }
        },

        ChangePhanCaTuan: function () {
            var model = this.listcatuan.filter(o => o.key === this.data.CaTuanId);
            if (model.length > 0) {
                var listId = model[0].value.map(o => o.Id);
                this.listdata.calamviec.map(o => o.Checked = false);
                if (listId !== null && listId.length > 0) {
                    this.listdata.calamviec.filter(o => listId.contains(o.Id)).map(o => o.Checked = true);
                }
            }
        },

        RemoveCaNhanVien: function (key, item) {
            if (this.data.CaTuanId === key) {
                item.Checked = !item.Checked;
            }
            var resul = this.listcatuan.filter(o => o.key === key);
            if (resul.length > 0) {
                resul[0].value = resul[0].value.filter(o => o.Id !== item.Id);
            }
        },

        SavePhieuPhanCaDB: function (model, url) {
            var self = this;
            $.ajax({
                data: model,
                url: url + "?" + "ID_DonVi=" + $('#hd_IDdDonVi').val(),
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    console.log(data);
                    if (data.res === true) {
                        commonStatisJs.ShowMessageSuccess(data.mess);
                        $('#modalThemMoiPhieuPhanCa').modal('hide');
                        $('body').trigger('AddPhanCaLamViecSucces');
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(data.mess);
                    }
                    self.loadding = false;
                },
                error: function (result) {
                    console.log(result);
                    self.loadding = false;
                }
            });
        },

        // Cập nhật phiếu phân ca
        SavePhanCa: function () {
            var self = this;
            if (self.data.TuNgay === null || self.data.TuNgay === undefined) {
                commonStatisJs.ShowMessageDanger("Vui lòng nhập từ ngày");
                return;
            }
            else {
                // check trung thoiwgian trong cung 1 phieu
                var trungca = self.CheckGoiTrung_CaLamViec();
                if (trungca === false) {
                    return false;
                }
                var PhieuPhanCa = {
                    MaPhieu: self.data.MaPhieu,
                    TuNgay: commonStatisJs.convertDateToDateServer(self.data.TuNgay),
                    DenNgay: commonStatisJs.convertDateToDateServer(self.data.DenNgay),
                    TrangThai: self.data.TrangThai,
                    GhiChu: self.data.GhiChu,
                    LoaiPhanCa: self.data.LoaiPhanCa,
                    ID_NhanVienTao: $('.idnhanvien').text(),
                    ID_DonVi: self.listdata.chinhanhid,
                    ID: self.data.ID
                }
                var CaCoDinh = self.listdata.calamvieccodinh.filter(o => o.Checked).map(o => o.Id);
                var NhanVien = self.listdata.nhanvienselect.map(o => o.Id);
                var CaTuan = [];
                self.listcatuan.forEach(function (element) {
                    CaTuan.push({
                        key: element.key,
                        value: element.value.map(o => o.Id)
                    })
                });

                // 1.catuan, 3.ca codinh
                if (parseInt(self.data.LoaiPhanCa) === 3) {
                    if (CaCoDinh.length === 0) {
                        commonStatisJs.ShowMessageDanger("Vui lòng chọn ca thực hiện");
                        return;
                    }
                }
                else {
                    if (CaTuan.filter(x => x.value.length !== 0).length === 0) {
                        commonStatisJs.ShowMessageDanger("Vui lòng chọn ca thực hiện");
                        return;
                    }
                }

                if (NhanVien.length === 0) {
                    commonStatisJs.ShowMessageDanger("Vui lòng chọn nhân viên");
                    return;
                }

                self.loadding = true;
                var model = {
                    PhieuPhanCa, CaCoDinh, CaTuan, NhanVien
                };

                var url = "/api/DanhMuc/NS_NhanSuAPI/UpdatePhanCaLamViec";
                if (!self.isNew) {
                    $.getJSON(self.URL_APINhanSu + "UpdatePhieuPhanCa_CheckExistBangLuongCong?idPhieuPhanCa=" + self.data.ID, function (x) {
                        if (x.res === true) {
                            // mượn tạm trường: MaNhanVien (1. da tao bangluong, 0. nguoclai)
                            if (x.dataSoure.MaNhanVien === "1") {
                                self.error = 'Nhân viên '.concat(commonStatisJs.Remove_LastComma(x.dataSoure.TenNhanVien), ' đã tạo bảng lương. Không thể cập nhật');
                            }
                            else {
                                if (x.dataSoure.MaNhanVien === "0") {
                                    if (x.dataSoure.TenNhanVien !== '') {
                                        commonStatisJs.ConfirmDialog_OKCancel('Cập nhật phiếu phân ca',
                                            'Nhân viên <b> '.concat(commonStatisJs.Remove_LastComma(x.dataSoure.TenNhanVien), ' </b> đã chấm công. Bạn có chắc chắn muốn cập nhật công của nhân viên không?'),
                                            function () {
                                                self.SavePhieuPhanCaDB(model, url);
                                                $('#modalPopuplgDelete').modal('hide');
                                            }, function () {
                                                $('#modalPopuplgDelete').modal('hide');
                                                return false;
                                            });
                                    }
                                    else {
                                        self.SavePhieuPhanCaDB(model, url);
                                    }
                                }
                            }
                        }
                        else {
                            commonStatisJs.ShowMessageDanger(x.mess);
                        }
                        self.loadding = false;
                    });
                }
                else {
                    url = "/api/DanhMuc/NS_NhanSuAPI/InsertPhanCaLamViec";
                    self.SavePhieuPhanCaDB(model, url);
                }
            }
        },

        Exec_XoaPhieuPhanCa: function () {
            var self = this;
            $.ajax({
                data: self.data,
                url: "/api/DanhMuc/NS_NhanSuAPI/DeletePhanCaLamViec?ID_DonVi=" + $('#hd_IDdDonVi').val() + "&ID_NhanVien=" + $('.idnhanvien').text(),
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    if (data.res === true) {
                        commonStatisJs.ShowMessageSuccess(data.mess);
                        $('body').trigger('AddPhanCaLamViecSucces');
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(data.mess);
                        console.log(data);
                    }
                },
                error: function (result) {
                    console.log(result);
                }
            });
        },

        DeletePhanCa: function (item) {
            var self = this;
            self.data = {
                ID: item.ID,
                MaPhieu: item.MaPhieu,
                TuNgay: item.TuNgay,
                DenNgay: item.DenNgay,
                TrangThai: item.TrangThai,
                LoaiPhanCa: item.LoaiPhanCa.toString(),
                CaTuanId: "1",
                GhiChu: item.GhiChu
            };
            $.getJSON(self.URL_APINhanSu + "UpdatePhieuPhanCa_CheckExistBangLuongCong?idPhieuPhanCa=" + self.data.ID, function (x) {
                if (x.res === true) {
                    // mượn tạm trường: MaNhanVien (1. da tao bangluong, 0. nguoclai)
                    if (x.dataSoure.MaNhanVien === "1") {
                        commonStatisJs.ShowMessageDanger('Nhân viên '.concat(commonStatisJs.Remove_LastComma(x.dataSoure.TenNhanVien), ' đã tạo bảng lương. Không xóa phiếu phân ca'));
                    }
                    else {
                        if (x.dataSoure.MaNhanVien === "0") {
                            if (x.dataSoure.TenNhanVien !== '') {
                                commonStatisJs.ConfirmDialog_OKCancel('Cập nhật phiếu phân ca',
                                    'Nhân viên <b> '.concat(commonStatisJs.Remove_LastComma(x.dataSoure.TenNhanVien), ' </b> đã chấm công. Bạn có chắc chắn muốn xóa công của nhân viên không?'),
                                    function () {
                                        self.Exec_XoaPhieuPhanCa();
                                        $('#modalPopuplgDelete').modal('hide');
                                    }, function () {
                                        $('#modalPopuplgDelete').modal('hide');
                                        return false;
                                    });
                            }
                            else {
                                self.Exec_XoaPhieuPhanCa();
                            }
                        }
                    }
                }
                else {
                    commonStatisJs.ShowMessageDanger(x.mess);
                }
            });
        }
    },
    computed: {
        selectAllNv: function () {
            var self = this;
            self.listdata.nhanvien.forEach(function (user) {
                if (user.Active)
                    user.Checked = self.listdata.checkallnv
            });
        },
        selectAllNvAc: function () {
            var self = this;
            self.listdata.nhanvienselect.forEach(function (user) {
                user.Checked = self.listdata.checkallnvac
            });
        },
        TitlePopup: function () {
            if (this.isNew) {
                return "Thêm mới phiếu phân ca";
            }
            else {
                return "Cập nhật phiếu phân ca";
            }
        },
        ListPhongBan: function () {
            var self = this;
            if (commonStatisJs.CheckNull(self.listdata.searchpb)) {
                return self.listdata.phongban;
            }
            else {
                var data = $.grep(self.listdata.phongban, function (e) {
                    var result = (commonStatisJs.convertVieToEng(e.text).indexOf(commonStatisJs.convertVieToEng(self.listdata.searchpb)) >= 0
                        || (e.children.length > 0 && (e.children.some(self.evensearch1) || e.children.some(self.evensearch2))));
                    return result;
                });
                return data;
            }
        },
        ListNhanVien: function () {
            var self = this;
            if (commonStatisJs.CheckNull(self.listdata.searchnv)) {
                return self.listdata.nhanvien.filter(c => c.Active);
            }
            else {
                return self.listdata.nhanvien.filter(e => e.Active
                    && (commonStatisJs.convertVieToEng(e.Ten).indexOf(commonStatisJs.convertVieToEng(self.listdata.searchnv)) >= 0
                        || e.Ma.indexOf(commonStatisJs.convertVieToEng(self.listdata.searchnv)) >= 0
                    ));
            }
        },
        ListNhanVienAc: function () {
            var self = this;
            if (commonStatisJs.CheckNull(self.listdata.searchnvac)) {
                return self.listdata.nhanvienselect;
            }
            else {
                return self.listdata.nhanvienselect.filter(e =>
                    commonStatisJs.convertVieToEng(e.Ten).indexOf(commonStatisJs.convertVieToEng(self.listdata.searchnvac)) >= 0
                    || e.Ma.indexOf(commonStatisJs.convertVieToEng(self.listdata.searchnvac)) >= 0
                );
            }
        },
    },
});
$('.datetime').on('change', function () {
    var dateParts = $(this).val().split("/");
    if (dateParts.length >= 3) {
        var dateObject = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);
        vmmodalEditPhieuPhanca.ChangDatetime(dateObject, $(this).data('id'));
    }
})
vmmodalEditPhieuPhanca.GetLoaiCa();
vmmodalEditPhieuPhanca.GetListRole();