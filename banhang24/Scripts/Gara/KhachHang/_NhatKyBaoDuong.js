var vmNhatKyBaoDuong = new Vue({
    el: '#vmNhatKyBaoDuong',
    components: {
    },
    data: {
        saveOK: false,
        isNew: true,
        txtPhuTung: '',
        txtDiary: '',
        statusExpire: 0,
        isCheckAll: false,

        listApply: [],
        listData: {
            AllPhuTungs: [],
            PhuTungs: [],
            AllDiarys: [],
            Diarys: [],
        },
    },
    created: function () {
        var self = this;
    },
    methods: {
        GetListPhuTungBaoDuong_byCar: function (idCar, idPhieuTN) {
            var self = this;
            if (!commonStatisJs.CheckNull(idCar) && idCar !== const_GuidEmpty) {
                var param = {
                    ID_Xe: idCar,
                    ID_PhieuTiepNhan: idPhieuTN,
                    //NgayBaoDuongFrom: moment(new Date()).add(-1, 'months').format('YYYY-MM-DD'),
                    //NgayBaoDuongTo: moment(new Date()).add(1, 'months').format('YYYY-MM-DD'),
                    TrangThais: '1,3',
                }
                ajaxHelper('/api/DanhMuc/GaraAPI/GetLichBaoDuong', 'POST', param).done(function (x) {
                    if (x.res) {
                        self.listData.PhuTungs = x.dataSoure;
                        self.listData.AllPhuTungs = x.dataSoure;

                        newModelBanLe.BaoDuong_TongPhuTung(x.dataSoure.length);
                    }
                });
            }
        },
        GetNhatKyBaoDuong_byCar: function (idCar) {
            var self = this;
            if (!commonStatisJs.CheckNull(idCar) && idCar !== const_GuidEmpty) {
                ajaxHelper('/api/DanhMuc/GaraAPI/GetNhatKyBaoDuong_byCar?idCar=' + idCar
                    , 'GET').done(function (x) {
                        if (x.res) {
                            self.listData.Diarys = x.dataSoure;
                            self.listData.AllDiarys = x.dataSoure;
                        }
                    });
            }
        },
        Insert_LichNhacBaoDuong: function (idHoaDon, cthd, isPrint = true) {

            if (!commonStatisJs.CheckNull(idHoaDon) && idHoaDon !== const_GuidEmpty) {
                let arrID = cthd.map(function (x) {
                    return x.ID_LichBaoDuong
                }).filter(x => x !== null);
                let myData = {
                    arrIDLich: arrID,
                    status: 2,// daxuly
                }

                ajaxHelper('/api/DanhMuc/GaraAPI/LichNhacBaoDuong_updateStatus', 'POST', myData).done(function (x1) {
                    ajaxHelper('/api/DanhMuc/GaraAPI/Insert_LichNhacBaoDuong?idHoaDon=' + idHoaDon, 'GET').done(function (x) {
                        if (isPrint == false) {
                            window.close();
                        }
                    });
                });
            }
        },
        Upadate_LichBaoDuong: function (idHoaDon, hangthaymoi, ngaylapNew, ngaylapOld) {
            if (!commonStatisJs.CheckNull(hangthaymoi) && hangthaymoi.length > 0) {
                var myData = {
                    ID_HoaDon: idHoaDon,
                    IDHangHoas: hangthaymoi,
                    NgayLapHoaDonOld: ngaylapOld,
                }
                ajaxHelper('/api/DanhMuc/GaraAPI/UpdateHD_UpdateBaoDuong', 'POST', myData)
                    .done(function (x) {
                        console.log('UpdateHD_UpdateBaoDuong ', x);
                    })
            }

            //ajaxHelper('/api/DanhMuc/GaraAPI/UpdateLichBD_whenChangeNgayLapHD?idHoaDon=' + idHoaDon +
            //    '&ngaylapOld=' + ngaylapOld + '&ngaylapNew=' + ngaylapNew, 'GET')
            //    .done(function (x) {
            //        console.log('UpdateLichBD_whenChangeNgayLapHD ', x);
            //    })
        },

        showModal: function () {
            var self = this;
            self.isCheckAll = false;
            self.saveOK = false;
            self.listApply = [];
            $('#vmNhatKyBaoDuong').modal('show');
        },
        Change_statusExpire: function () {
            var self = this;
            self.listData.PhuTungs = self.GetPhuTung_byStatusExprire();
        },
        ChangeLanBaoDuong: function (indexParent, indexCT) {
            var self = this;
            for (let i = 0; i < self.listData.PhuTungs.length; i++) {
                let forOut = self.listData.PhuTungs[i];
                let lanBD = forOut.LanBaoDuong;
                if (i === indexParent) {
                    for (let k = 0; k < forOut.CTBaoDuongs.length; k++) {
                        let forIn = forOut.CTBaoDuongs[k];
                        if (k === indexCT) {
                            forIn.isCheck = true;
                            lanBD = forIn.LanBaoDuong;
                        }
                        else {
                            forIn.isCheck = false;
                        }
                    }
                    forOut.LanBaoDuong = lanBD;
                    break;
                }
            }
            var arr = self.listData.PhuTungs;
            self.listData.PhuTungs = $.extend([], true, arr);
        },
        GetPhuTung_byStatusExprire: function () {
            var self = this;
            var arr = [];
            switch (parseInt(self.statusExpire)) {
                case 0://all
                    arr = self.listData.AllPhuTungs;
                    break;
                case 1://conhan
                    arr = self.listData.AllPhuTungs.filter(x => x.HanBaoHanh === null
                        || moment(x.HanBaoHanh).format('YYYY-MM-DD') >= moment(new Date()).format('YYYY-MM-DD'));
                    break;
                case 2://hethan
                    arr = self.listData.AllPhuTungs.filter(x => x.HanBaoHanh !== null
                        && moment(x.HanBaoHanh).format('YYYY-MM-DD') < moment(new Date()).format('YYYY-MM-DD'));
                    break;
            }
            return arr;
        },
        SearchPhuTung: function () {
            var self = this;
            var arr = self.GetPhuTung_byStatusExprire();
            var txt = locdau(self.txtPhuTung);
            if (txt !== '') {
                arr = arr.filter(x => locdau(x.TenHangHoa).indexOf(txt) > -1
                    || locdau(x.MaHangHoa).indexOf(txt) > -1);
            }
            self.listData.PhuTungs = arr;
        },
        SearchDiary: function () {
            var self = this;
            var txt = locdau(self.txtDiary);
            if (txt !== '') {
                arr = self.listData.AllDiarys.filter(x => locdau(x.TenHangHoa).indexOf(txt) > -1
                    || locdau(x.MaHangHoa).indexOf(txt) > -1
                    || locdau(x.MaHoaDon).indexOf(txt) > -1);
            }
            self.listData.Diarys = arr;
        },
        ChangeCheckAll: function () {
            var self = this;
            for (let i = 0; i < self.listData.PhuTungs.length; i++) {
                let itFor = self.listData.PhuTungs[i];
                itFor.isCheckParent = self.isCheckAll;
            }
            var arr = self.listData.PhuTungs;
            self.listData.PhuTungs = $.extend([], true, arr);
        },
        Agree: function () {
            var self = this;
            self.saveOK = true;
            var self = this;
            for (let i = 0; i < self.listData.PhuTungs.length; i++) {
                let itFor = self.listData.PhuTungs[i];
                if (itFor.isCheckParent) {
                    for (let j = 0; j < itFor.CTBaoDuongs.length; j++) {
                        let forIn = itFor.CTBaoDuongs[j];
                        if (forIn.isCheck) {
                            self.listApply.push(forIn);
                        }
                    }
                }
            }
            if (self.listApply.length === 0) {
                self.saveOK = false;
                commonStatisJs.ShowMessageDanger('Vui lòng chọn lịch bảo dưỡng');
                return;
            }
            $('#vmNhatKyBaoDuong').modal('hide');
            console.log(2, self.listApply)
        },
    }
})
