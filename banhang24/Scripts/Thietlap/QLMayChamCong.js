var urlApiChamCong = '/api/DanhMuc/NS_NhanSuAPI/'
var VMayChamCong = new Vue({
    el: '#vMayChamCong',
    data: {
        databind: {
            //pageview: '',
            //pagenow: 1,
            data: []
            //listpage: [],
            //isprev: false,
            //isnext: false,
            //countpage: 0,
        },
        isNew: true,
        loadding: false,
        data: {},
        Role: {}
    },
    methods: {
        LoadMayChamCong: function () {
            let self = this;
            if (VueChiNhanh.databind.data.length !== 0) {
                self.GetForSearchMayChamCong();
            }
            else {

                setTimeout(self.LoadMayChamCong, 250);
            }
        },
        GetForSearchMayChamCong: function () {
            let self = this;
            let lstIdChiNhanh = [""];
            let myData = {};
            VueChiNhanh.databind.data.filter(p => p.CNChecked === true).forEach(p => lstIdChiNhanh.push(p.ID));
            myData.IDs = lstIdChiNhanh;
            $.ajax({
                traditional: true,
                url: urlApiChamCong + 'GetListMayChamCongByChiNhanh',
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: myData,
                success: function (data) {
                    self.databind.data = data.dataSoure.data;
                }
            });
        },
        AddNewMayChamCong: function () {
            VModalMCC.AddNewMCC();
        },
        Edit: function (item) {
            VModalMCC.EditMCC(commonStatisJs.CopyObject(item));
        }
    }
});
VMayChamCong.LoadMayChamCong();

$('body').on('AddMCCSucces', function () {
    VMayChamCong.LoadMayChamCong();
});
var VModalMCC = new Vue({
    el: "#AddEditMCC",
    data: {
        isNew: true,
        data: {},
        KieuMaySelected: '1',
        lstKieuMay: [
            { text: 'Realand', value: '1' }
        ],
        ChiNhanhSelected: '1',
        lstChiNhanh: [],
        KieuKetNoiSelected: '1',
        lstKieuKetNoi: [
            { text: 'Ip/Domain', value: '1' },
            { text: 'P2P', value: '2' }
        ]
    },
    methods: {
        AddNewMCC: function () {
            let self = this;
            self.isNew = true;
            self.data = {
                ID: null,
                MaMCC: '',
                TenMCC: '',
                TenHienThi: '',
                IP: '',
                Port: '5500',
                SoSeries: '',
                MatMa: '',
                IDMay: ''
            };
            VModalMCC.GetListDMDonVi();
            self.ChiNhanhSelected = '1';
            self.KieuKetNoiSelected = '1';
            $('#themmoimaychamcong').modal('show');
        },
        SaveMCC: function () {
            let self = this;
            if (self.ChiNhanhSelected === '1') {
                commonStatisJs.ShowMessageDanger("Vui lòng chọn chi nhánh.");
                return;
            }
            if (self.data.IP === '') {
                commonStatisJs.ShowMessageDanger("Vui lòng nhập địa chỉ kết nối.");
                return;
            }
            if (self.data.Port === '') {
                commonStatisJs.ShowMessageDanger("Vui lòng nhập cổng kết nối.");
                return;
            }
            if (self.data.SoSeries === '') {
                commonStatisJs.ShowMessageDanger("Vui lòng số series máy chấm công.");
                return;
            }

            let model = {
                ID: self.data.ID,
                MaMCC: self.data.MaMCC,
                TenMCC: self.data.TenMCC,
                TenHienThi: self.data.TenHienThi,
                ID_ChiNhanh: self.ChiNhanhSelected,
                LoaiKetNoi: self.KieuKetNoiSelected,
                IP: self.data.IP,
                Port: self.data.Port,
                SoSeries: self.data.SoSeries,
                MatMa: self.data.MatMa,
                SoDangKy: self.data.IDMay,
                LoaiHinh: self.KieuMaySelected
            };
            let urlsave = urlApiChamCong + "AddUpdateMayChamCong";
            $.ajax({
                data: model,
                url: urlsave + "?" + "ID_DonVi=" + $('#hd_IDdDonVi').val() + "&ID_NhanVien=" + $('.idnhanvien').text(),
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    if (data.res === true) {
                        commonStatisJs.ShowMessageSuccess(data.mess);
                        $('#themmoimaychamcong').modal('hide');
                        $('body').trigger('AddMCCSucces');
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(data.mess);
                    }
                },
                error: function (result) {
                    console.log(result);
                }
            });
        },
        EditMCC: function (item) {
            let self = this;
            self.isNew = false;
            self.data = item;
            VModalMCC.GetListDMDonVi();
            self.ChiNhanhSelected = item.IDChiNhanh;
            self.KieuKetNoiSelected = item.LoaiKetNoi;
            $('#themmoimaychamcong').modal('show');
        },
        GetListDMDonVi: function () {
            let self = this;
            self.lstChiNhanh = VueChiNhanh.databind.data;
        }
    },
    computed: {
        TitlePopup: function () {
            if (this.isNew) {
                return "Thêm mới máy chấm công";
            }
            else {
                return "Cập nhật máy chấm công";
            }
        }
    }
});

var filterTarget = "QuanLyMayChamCong";