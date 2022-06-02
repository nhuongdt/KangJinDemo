var vmXuatXuong = new Vue({
    el: '#XuatXuongModal',
    components: {
        'my-date-time': cpmDatetime
    },
    data: {
        saveOK: false,
        isNew: true,
        inforLogin: {
            ID_NhanVien: null,
            ID_User: null,
            UserLogin: null,
            ID_DonVi: null,
            TenNhanVien: '',
        },
        phieuXuat: {
            ID: null,
            MaPhieuTiepNhan: '',
            BienSo: '',
            NgayXuatXuong: '',
            SoKmRa: '',
            XuatXuong_GhiChu: '',
        },
        MauIn: {
            ListMauIn: [],
            ListData: {
                HoaDon: {},
                HangMucSuaChua: [],
                VatDungKemTheo: [],
            },
        }
    },
    methods: {
        ShowModalXuatXuong: function () {
            var self = this;
            self.saveOK = false;
            self.isNew = true;
            $('#XuatXuongModal').modal('show');
        },
        ChangeNgayXuatXuong: function (e) {
            var dt = moment(e).format('YYYY-MM-DD HH:mm');
            this.phieuXuat.NgayXuatXuong = dt;
        },
        XuatXuong: function () {
            var self = this;
            if (commonStatisJs.CheckNull(self.phieuXuat.NgayXuatXuong)) {
                commonStatisJs.ShowMessageDanger('Vui lòng nhập ngày xuất xưởng');
                return;
            }
            var myData = self.phieuXuat;
            console.log(myData)
            ajaxHelper('/api/DanhMuc/GaraAPI/PhieuTN_XuatXuong', 'post', myData).done(function (x) {
                if (x.res) {
                    self.saveOK = true;
                    var diary = {
                        ID_DonVi: self.inforLogin.ID_DonVi,
                        ID_NhanVien: self.inforLogin.ID_NhanVien,
                        LoaiNhatKy: 1,
                        ChucNang: 'Bàn làm việc - Xuất xưởng',
                        NoiDung: 'Xuất xưởng xe '.concat(self.phieuXuat.BienSo),
                        NoiDungChiTiet: 'Xuất xưởng xe '.concat(self.phieuXuat.BienSo,
                            ', Ngày xuất: ', moment(self.phieuXuat.NgayXuatXuong, 'YYYY-MM-DD HH:mm').format('DD/MM/YYYY HH:mm'),
                            ', Người xuất: ', self.inforLogin.UserLogin,
                            ', Số Km ra:', self.phieuXuat.SoKmRa),
                    };
                    Insert_NhatKyThaoTac_1Param(diary);
                }
                else {
                    self.saveOK = false;
                }
                $('#XuatXuongModal').modal('hide');
            })
        },

        Print: function (isPrintID, val) {
            var self = this;
            var url = '/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=' + val
                + '&idDonVi=' + self.inforLogin.ID_DonVi;
            if (isPrintID) {
                url = '/api/DanhMuc/ThietLapApi/GetContentFIlePrint?idMauIn=' + val;
            }

            var item3 = self.MauIn.ListData.HoaDon;
            var item4 = self.MauIn.ListData.HangMucSuaChua ? self.MauIn.ListData.HangMucSuaChua : [];
            var item5 = self.MauIn.ListData.VatDungKemTheo ? self.MauIn.ListData.VatDungKemTheo : [];

            if (!commonStatisJs.CheckNull(item3.NgayVaoXuong)) {
                item3.NgayVaoXuong = moment(item3.NgayVaoXuong).format('DD/MM/YYYY HH:mm');
            }
            if (!commonStatisJs.CheckNull(self.phieuXuat.NgayXuatXuong)) {
                item3.NgayXuatXuong = moment(self.phieuXuat.NgayXuatXuong).format('DD/MM/YYYY HH:mm');
            }
            else {
                item3.NgayXuatXuong = '';
            }
            if (!commonStatisJs.CheckNull(self.phieuXuat.NgayXuatXuongDuKien)) {
                item3.NgayXuatXuongDuKien = moment(self.phieuXuat.NgayXuatXuongDuKien).format('DD/MM/YYYY HH:mm');
            }
            else {
                item3.NgayXuatXuongDuKien = '';
            }
            console.log(2, item3)

            ajaxHelper(url, 'GET').done(function (result) {
                var data = result;
                data = data.concat('<script src="/Scripts/knockout-3.4.2.js"></script>');
                data = data.concat("<script > var item1= [];",
                    " var item2= [] ; ",
                    " var item3= ", JSON.stringify(item3), ' ;',
                    " var item4= ", JSON.stringify(item4), ' ;',
                    " var item5= ", JSON.stringify(item5), ' ;',
                    " </script>");
                data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                PrintExtraReport(data);
            });
            self.XuatXuong();
        }
    }
})

