//var treeDepartment='',
var vmCopySetupSalary = new Vue({
    el: '#modalCopySetupSalary',
    data: {
        titlePopup: 'Sao chép thiết lập lương',
        loadfirst: 0,
        isNew: true,
        loadding: false,
        apiNhanSu: '/api/DanhMuc/NS_NhanSuAPI/',
        treeDepartment: '',
        listdata: {
            LoaiThietLap: [{ ID: 1, Text: 'Lương', Checked: false }, { ID: 5, Text: 'Phụ cấp', Checked: false }, { ID: 6, Text: 'Giảm trừ', Checked: false }],
            idChiNhanh: $('#hd_IDdDonVi').val(),
            PhongBan: [],
            NhanVienHadSetUp: [],
            AllNhanVien: [],
            NhanVienSelect: [],
            searchPhong: '',
            searchStaffSetup: '',
            searchStaff: '',
            searchStaffApply: '',
            checkAllNhanVien: false,
            checkAllNhanVienApply: false,
            updateNVSetup: false,
        },
        dataComputed: {
            ListNhanVien: [],
            ListNhanVienSelect: [],
        }
    },
    methods: {
        GetListData: function () {
            var self = this;
            $.getJSON('/api/DanhMuc/NS_NhanVienAPI/' + "GetListNhanVien_HadSetupSalary?idChiNhanh=" + self.listdata.idChiNhanh, function (data) {
                if (data.res) {
                    self.listdata.NhanVienHadSetUp = data.dataSoure.Data;
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });
            vmCopySetupSalary.SelectNhanVienByPhongBan(vmCopySetupSalary.listdata.idChiNhanh, true);
        },

        showModal: function () {
            var self = this;
            self.isNew = true;
            self.listdata.NhanVienHadSetUp.map(x => x.Active = false);
            self.listdata.LoaiThietLap.map(x => x.Checked = false);
            self.listdata.NhanVienSelect = [];
            vmCopySetupSalary.SelectNhanVienByPhongBan(self.listdata.idChiNhanh, true);

            console.log('saochepTLLuong')
            self.treeDepartment = $('#treeDepartment').tree({
                primaryKey: 'id',
                uiLibrary: 'bootstrap',
                dataSource: self.listdata.PhongBan,
                checkboxes: false,
            }).on('select', function (e, node, id) {
                vmCopySetupSalary.SelectNhanVienByPhongBan(id, false);
            })
            $('#modalCopySetupSalary').modal('show');
        },

        saoChepFrom: function (nvien, lstLoaiLuong) {
            var self = this;
            self.isNew = false;
            self.listdata.NhanVienHadSetUp.map(x => x.Active = false);
            self.listdata.NhanVienHadSetUp.filter(x => x.ID === nvien.ID).map(x => x.Active = true);

            self.listdata.LoaiThietLap.map(x => x.Checked = false);
            self.listdata.LoaiThietLap.filter(x => lstLoaiLuong.contains(x.ID)).map(x => x.Checked = true);

            self.listdata.NhanVienSelect = [];
            vmCopySetupSalary.SelectNhanVienByPhongBan(self.listdata.idChiNhanh, true);

            console.log('saoChepFrom')
            self.treeDepartment = $('#treeDepartment').tree({
                primaryKey: 'id',
                uiLibrary: 'bootstrap',
                dataSource: self.listdata.PhongBan,
                checkboxes: false,
            }).on('select', function (e, node, id) {
                vmCopySetupSalary.SelectNhanVienByPhongBan(id, false);
            })
            $('#modalCopySetupSalary').modal('show');
        },

        searchPhongBan: function () {
            var self = this;
            var arr = [];
            if (commonStatisJs.CheckNull(self.listdata.searchPhong)) {
                arr = self.listdata.PhongBan;
            }
            else {
                arr = $.grep(self.listdata.PhongBan, function (e) {
                    var result = (commonStatisJs.convertVieToEng(e.text).indexOf(commonStatisJs.convertVieToEng(self.listdata.searchPhong)) >= 0
                        || (e.children.length > 0 && (e.children.some(self.evensearch1) || e.children.some(self.evensearch2))));
                    return result;
                });

            }
            self.treeDepartment.destroy();
            self.treeDepartment = $('#treeDepartment').tree({
                primaryKey: 'id',
                uiLibrary: 'bootstrap',
                dataSource: arr,
                checkboxes: false,
            }).on('select', function (e, node, id) {
                vmCopySetupSalary.SelectNhanVienByPhongBan(id);
            });
        },

        ChoseLoaiLuong: function (item) {
            var self = this;
            item.Checked = !item.Checked;
            return false;
        },

        ChoseNhanVien_HasSetup: function (item) {
            var self = this;
            self.listdata.NhanVienHadSetUp.filter(x => x.ID !== item.ID).map(x => x.Active = false);// reset all nvien active = false
            item.Active = !item.Active;
        },


        // Get nhân viên theo phòng ban hay là chi nhánh nếu  isAll=true
        SelectNhanVienByPhongBan: function (id, isAll = false) {
            var $this = $(event.currentTarget);
            var self = this;
            var url = self.apiNhanSu + "GetListNhanVienTheoPhongBan";
            if (isAll) {
                url = self.apiNhanSu + "GetListNhanVienTheoChiNhanh";
                $('#treeDepartment .treename').addClass('active');
            }
            else {
                $('#treeDepartment .treename').removeClass('active');
            }
            $.getJSON(url + "?id=" + id, function (data) {
                if (data.res) {
                    self.listdata.AllNhanVien = data.dataSoure;
                    // assign propertis Active= false for list NhanVien (column left) if nvien had apply (column right)
                    var list = self.listdata.NhanVienSelect.map(function (item) { return item.Id });
                    self.listdata.AllNhanVien.filter(o => list.contains(o.Id)).map(c => c.Active = false);
                    self.ChoseNVienApply();
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });
        },

        evensearch1: function (o) {
            return commonStatisJs.convertVieToEng(o.text).indexOf(commonStatisJs.convertVieToEng(this.listdata.searchPhong)) >= 0;
        },

        evensearch2: function (o) {
            return o.children.length > 0 && o.children.some(this.evensearch1);
        },

        ChoseNVienApply: function () {
            var self = this;
            self.dataComputed.ListNhanVien = self.listdata.AllNhanVien.filter(c => c.Active);
            self.dataComputed.ListNhanVienSelect = self.listdata.NhanVienSelect;
        },

        searchNVienAll: function () {
            var self = this;
            self.dataComputed.ListNhanVien = self.listdata.AllNhanVien.filter(e => e.Active
                && (commonStatisJs.convertVieToEng(e.Ten).indexOf(commonStatisJs.convertVieToEng(self.listdata.searchStaff)) >= 0
                    || e.Ma.indexOf(commonStatisJs.convertVieToEng(self.listdata.searchStaff)) >= 0
                ));
        },

        searchNVienApply: function () {
            var self = this;
            self.dataComputed.ListNhanVienSelect = self.listdata.NhanVienSelect.filter(e =>
                commonStatisJs.convertVieToEng(e.Ten).indexOf(commonStatisJs.convertVieToEng(self.listdata.searchStaffApply)) >= 0
                || e.Ma.indexOf(commonStatisJs.convertVieToEng(self.listdata.searchStaffApply)) >= 0
            );
        },

        // Xử lý chọn xóa nhân viên để áp dụng
        NextNhanVien: function (item) {
            var self = this;
            item.Checked = false;
            item.Active = false;
            self.listdata.NhanVienSelect.unshift(item);
            self.ChoseNVienApply();
        },

        PrevNhanVien: function (item) {
            var self = this;
            var model = self.listdata.AllNhanVien.filter(o => o.Id === item.Id);
            if (model.length > 0) {
                model[0].Checked = false;
                model[0].Active = true;
            }
            self.listdata.NhanVienSelect = self.listdata.NhanVienSelect.filter(o => o.Id !== item.Id);
            self.ChoseNVienApply();
        },

        NextListNhanVien: function () {
            var self = this;
            var datanext = self.listdata.AllNhanVien.filter(o => o.Checked && o.Active);
            datanext.map(o => o.Checked = false);
            datanext.map(o => o.Active = false);
            self.listdata.NhanVienSelect = self.listdata.NhanVienSelect.concat(datanext);
            self.listdata.checkAllNhanVien = false;
            self.ChoseNVienApply();
        },

        PrevListNhanVien: function () {
            var self = this;
            var dataprev = self.listdata.NhanVienSelect.filter(o => o.Checked).map(function (item) { return item.Id });
            var resul = self.listdata.AllNhanVien.filter(o => dataprev.contains(o.Id));
            resul.map(c => c.Checked = false);
            resul.map(c => c.Active = true);
            self.listdata.NhanVienSelect = self.listdata.NhanVienSelect.filter(o => !dataprev.contains(o.Id));
            self.listdata.checkAllNhanVienApply = false;
            self.ChoseNVienApply();
        },

        ApDung: function () {
            var self = this;
            var kieuluong = [];
            var lst = self.listdata.LoaiThietLap.filter(x => x.Checked);
            var saochepTuNV = self.listdata.NhanVienHadSetUp.filter(x => x.Active);
            var nvApply = self.listdata.NhanVienSelect.map(function (x) { return x.Id });

            if (lst.length === 0) {
                commonStatisJs.ShowMessageDanger('Vui lòng chọn kiểu thiết lập lương để sao chép');
                return false;
            }
            if (saochepTuNV.length === 0) {
                commonStatisJs.ShowMessageDanger('Vui lòng chọn nhân viên để sao chép');
                return false;
            }
            if (nvApply.length === 0) {
                commonStatisJs.ShowMessageDanger('Vui lòng chọn nhân viên áp dụng');
                return false;
            }

            if (lst.filter(x => x.ID === 5).length > 0) {
                kieuluong = [51, 52, 53];
            }
            if (lst.filter(x => x.ID === 6).length > 0) {
                kieuluong.push(61);
                kieuluong.push(62);
                kieuluong.push(63);
            }
            if (lst.filter(x => x.ID === 1).length > 0) {
                kieuluong.push(1);
                kieuluong.push(2);
                kieuluong.push(3);
                kieuluong.push(4);
            }

            var param = {
                ID_ChiNhanh: self.listdata.idChiNhanh,
                ID_NhanVien: saochepTuNV[0].ID,
                LstIDNhanVien: nvApply,
                LstKieuLuong: kieuluong,
                ID_NhanVienLogin: $('.idnhanvien').text(),
                UpdateNVSetup: self.listdata.updateNVSetup,
            }
            console.log(param);

            $.ajax({
                data: param,
                url: '/api/DanhMuc/NS_NhanVienAPI/' + "SaoChepThietLapLuong",
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (x) {
                 
                    if (x.res) {
                        commonStatisJs.ShowMessageSuccess(x.mess);
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(x.mess);
                    }
                    $('#modalCopySetupSalary').modal('hide');
                }
            });
        },

        
    },
    computed: {
        selectAllNhanVien: function () {
            var self = this;
            self.listdata.AllNhanVien.forEach(function (user) {
                if (user.Active)
                    user.Checked = self.listdata.checkAllNhanVien
            });
        },
        selectAllNhanVienApply: function () {
            var self = this;
            self.listdata.NhanVienSelect.forEach(function (user) {
                user.Checked = self.listdata.checkAllNhanVienApply
            });
        },
        ListNhanVienSetUp: function () {
            var self = this;
            if (commonStatisJs.CheckNull(self.listdata.searchStaffSetup)) {
                return self.listdata.NhanVienHadSetUp;
            }
            else {

                return self.listdata.NhanVienHadSetUp.filter(e =>
                    commonStatisJs.convertVieToEng(e.TenNhanVien).indexOf(commonStatisJs.convertVieToEng(self.listdata.searchStaffSetup)) >= 0
                    || e.MaNhanVien.indexOf(commonStatisJs.convertVieToEng(self.listdata.searchStaffSetup)) >= 0
                );
            }
        },
    },
});
vmCopySetupSalary.GetListData();
$('body').on('InsertNvluongphucapSuccess', function () {
    vmCopySetupSalary.GetListData();
});
