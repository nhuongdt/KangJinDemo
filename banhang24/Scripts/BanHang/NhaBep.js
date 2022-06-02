var Model_NhaBep = function () {
    var self = this;
    var BH_HoaDonUri = "/api/DanhMuc/BH_HoaDonAPI/";
    self.ThucDonYC = ko.observableArray();
    self.ThucDonWait = ko.observableArray();
    self.NhomHangHoas = ko.observableArray();
    self.InforHDprintf = ko.observableArray();
    self.DM_MauIn = ko.observableArray();

    var _idChiNhanh = $('#txtDonVi_Kitchen').val();
    var connected = false;

    function GetThucDonYC_FromDB() {
        ajaxHelper(BH_HoaDonUri + "SP_GetThucDonYeuCau?idDonVi=" + _idChiNhanh, 'GET').done(function (data) {
            for (var i = 0; i < data.length; i++) {
                data[i].WaitMinutes = '';
                data[i].Bep_SoLuong_ConLai = data[i].Bep_SoLuongYeuCau - data[i].Bep_SoLuongChoCungUng;

                if (data[i].ThoiGian !== null) {
                    data[i].WaitMinutes = getMinutesBetweenDates(data[i].ThoiGian);
                }
                self.ThucDonYC.push(data[i]);
            }
            localStorage.setItem('foodRequest', JSON.stringify(self.ThucDonYC()));
        });
    }

    function GetThucDonWait() {
        // read CTHD with Bep hoan thanh < Bep yeu cau
        ajaxHelper(BH_HoaDonUri + "SP_GetThucDonWait?idDonVi=" + _idChiNhanh, 'GET').done(function (data) {
            for (var i = 0; i < data.length; i++) {
                data[i].WaitMinutes = '';
                if (data[i].ThoiGian !== null) {
                    data[i].WaitMinutes = getMinutesBetweenDates(data[i].ThoiGian);
                }
                self.ThucDonWait.push(data[i]);
            }

            localStorage.setItem('foodWait', JSON.stringify(self.ThucDonWait()));
        });
    }

    function UpDate_BepHoanThanh(myData, hub) {
        console.log('UpDate_BepHoanThanh ', myData);

        $.ajax({
            url: BH_HoaDonUri + "Update_CTHoaDon_Bep",
            type: 'POST',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: myData,
            success: function (obj) {
                if (obj.res === true) {
                    var data = obj.data;
                    console.log(data);

                    var maHoaDon = myData.objUpdate.MaHoaDon;
                    var idViTri = myData.objUpdate.ID_ViTri;

                    // bind again lstWait
                    var arrWait = JSON.parse(localStorage.getItem('foodWait'));
                    for (var i = 0; i < arrWait.length; i++) {
                        if (arrWait[i].ID === myData.objUpdate.ID) {
                            if (data.Bep_SoLuongHoanThanh > 0 && data.Bep_SoLuongChoCungUng === 0) {
                                arrWait.splice(i, 1);
                            }
                            else {
                                arrWait[i].Bep_SoLuongChoCungUng = data.Bep_SoLuongChoCungUng;
                                arrWait[i].Bep_SoLuongHoanThanh = data.Bep_SoLuongHoanThanh;
                            }
                        }
                    }
                    localStorage.setItem('foodWait', JSON.stringify(arrWait));

                    // update nguoc tro lai cho ThucDonYC
                    var foodRq = localStorage.getItem('foodRequest');
                    if (foodRq !== null) {
                        foodRq = JSON.parse(foodRq);
                        for (var i = 0; i < foodRq.length; i++) {
                            if (foodRq[i].ID === myData.id) {
                                foodRq[i].Bep_SoLuongChoCungUng = data.Bep_SoLuongChoCungUng;
                                foodRq[i].Bep_SoLuongHoanThanh = data.Bep_SoLuongHoanThanh;
                                foodRq[i].Bep_SoLuongYeuCau = data.Bep_SoLuongYeuCau;
                                break;
                            }
                        }
                        localStorage.setItem('foodRequest', JSON.stringify(foodRq));
                    }

                    LoadThucDonYC_andWait_thisPage(hub, JSON.stringify(foodRq), JSON.stringify(arrWait), JSON.stringify(data));
                    SendDataNhaBep_toThuNgan(hub, idViTri, maHoaDon, JSON.stringify(data));
                }
                else {
                    console.log(obj.mes)
                }
            },
        })
    }

    function Update_BepHoanThanh_Huy(myData, hub) {
        $.ajax({
            url: BH_HoaDonUri + "Update_CTHoaDon_Bep",
            type: 'POST',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: myData,
            success: function (obj) {
                if (obj.res === true) {
                    var data = obj.data;
                    console.log(data);

                    data.MaHoaDon = myData.objUpdate.MaHoaDon;
                    data.TenPhongBan = myData.objUpdate.TenPhongBan;
                    data.WaitMinutes = myData.objUpdate.WaitMinutes;
                    data.TenHangHoa = myData.objUpdate.TenHangHoa;
                    data.ID_HoaDon = myData.objUpdate.ID_HoaDon;
                    data.ID_ViTri = myData.objUpdate.ID_ViTri;
                    data.ThoiGian = myData.objUpdate.ThoiGian;

                    // bind again lstWait
                    var arrWait = JSON.parse(localStorage.getItem('foodWait'));
                    for (var i = 0; i < arrWait.length; i++) {
                        if (arrWait[i].ID === data.ID) {
                            if (data.Bep_SoLuongChoCungUng === 0) {
                                arrWait.splice(i, 1);
                            }
                            else {
                                arrWait[i].Bep_SoLuongChoCungUng = data.Bep_SoLuongChoCungUng;
                                arrWait[i].Bep_SoLuongYeuCau = data.Bep_SoLuongYeuCau;
                            }
                        }
                    }
                    localStorage.setItem('foodWait', JSON.stringify(arrWait));

                    // bind again lstYeuCau
                    var arrRequest = JSON.parse(localStorage.getItem('foodRequest'));
                    for (var i = 0; i < arrRequest.length; i++) {
                        if (arrRequest[i].ID === data.ID) {
                            arrRequest.splice(i, 1);
                        }
                    }
                    arrRequest.unshift(data);
                    localStorage.setItem('foodRequest', JSON.stringify(arrRequest));

                    LoadThucDonYC_andWait_thisPage(hub, JSON.stringify(arrRequest), JSON.stringify(arrWait), JSON.stringify(data));
                    SendDataNhaBep_toThuNgan(hub, myData.objUpdate.ID_ViTri, myData.objUpdate.MaHoaDon, JSON.stringify(data));
                }
                else {
                    console.log(obj.mes);
                }
            },
        })
    }

    self.getHangHoa_ByIDNhom = function () {
        var lc_HangHoas = localStorage.getItem('lc_HangHoasBH');
        if (lc_HangHoas !== null) {
            lc_HangHoas = JSON.parse(lc_HangHoas);
            self.currentPage(0);
            // get arrHangHoa multiple ID
            var arrHangHoa = [];
            if (arrIDNhomHang.length > 0) {
                for (var i = 0; i < arrIDNhomHang.length; i++) {
                    for (var j = 0; j < lc_HangHoas.length; j++) {
                        if (arrIDNhomHang[i] === lc_HangHoas[j].ID_NhomHangHoa) {
                            arrHangHoa.push(lc_HangHoas[j]);
                        }
                    }
                }
                self.HangHoas(arrHangHoa);
            }
            else {
                self.HangHoas(lc_HangHoas);
            }
        }
    }

    self.NhomHHs = ko.observableArray();

    function GetAllNhomHH() {
        ajaxHelper('/api/DanhMuc/DM_NhomHangHoaAPI/' + 'GetDM_NhomHangHoa', 'GET').done(function (data) {
            for (var i = 0; i < data.length; i++) {
                if (data[i].ID_Parent == null) {
                    var objParent = {
                        ID: data[i].ID,
                        TenNhomHangHoa: data[i].TenNhomHangHoa,
                        Childs: [],
                        //Child2s: [{ TenNhomHangHoa: 'aa', ID: '232' }]
                    }

                    for (var j = 0; j < data.length; j++) {
                        if (data[j].ID !== data[i].ID && data[j].ID_Parent === data[i].ID) {
                            var objChild =
                                {
                                    ID: data[j].ID,
                                    TenNhomHangHoa: data[j].TenNhomHangHoa,
                                    ID_Parent: data[i].ID,
                                    Child2s: []
                                };

                            for (var k = 0; k < data.length; k++) {
                                if (data[k].ID_Parent !== null && data[k].ID_Parent === data[j].ID) {
                                    var objChild2 =
                                        {
                                            ID: data[k].ID,
                                            TenNhomHangHoa: data[k].TenNhomHangHoa,
                                            ID_Parent: data[j].ID,
                                        };
                                    objChild.Child2s.push(objChild2);
                                }
                            }
                            objParent.Childs.push(objChild);
                        }
                    }
                    self.NhomHangHoas.push(objParent);

                }
            }
            //localStorage.setItem('lc_NhomHH', JSON.stringify(self.NhomHangHoas));
        });
    };

    GetAllNhomHH();

    function Delete_PropertiesVAT_whenUpdateCTHD(item) {
        delete item["TienThue"];
        delete item["PTThue"];
    }

    function PlayAudio() {
        var audio = new Audio('../data/hu.mp3');
        audio.muted = true; // fix err play() failed because the user didn't interact with the document at brower
        audio.play();
    }

    function InHoaDon() {
        var mauin = "";
        if (self.ThucDonYC().length > 0) {
            self.InforHDprintf().TenPhongBan = self.ThucDonYC()[0].TenPhongBan;
            if (self.DM_MauIn().length > 0) {
                let dulieuMauIn = ReplaceString_toData(self.DM_MauIn()[0].DuLieuMauIn);
                let dulieuMauIn1 = '<script src="/Scripts/knockout-3.4.2.js"></script>';
                dulieuMauIn1 = dulieuMauIn1.concat("<script > var item1=" + JSON.stringify(self.ThucDonYC())
                    + "; var item2=[], item4=[], item5 =[]"
                    + "; var item3=" + JSON.stringify(self.InforHDprintf()) + "; </script>");
                dulieuMauIn1 = dulieuMauIn1.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                PrintExtraReport(dulieuMauIn, dulieuMauIn1, 1);
            }
        }
    }

    $(function () {
        var hub = $.connection.AlertHub;
        hub.client.send = function (jsonObj, parameter2) {
            var idChiNhanhSend = parameter2;
            // load again if same ChiNhanh
            if (parameter2 == _idChiNhanh) {
                loadData_FromTBNB(jsonObj);
                //PlayAudio();
            }
        };

        hub.client.bindAgain_ThucDonYC_Wait = function (lstRequest, lstWait, dataDB) {

            if (lstRequest === null) {
                self.ThucDonYC([]);
            }
            else {
                lstRequest = JSON.parse(lstRequest);
                self.ThucDonYC(lstRequest);
            }

            if (lstWait === null) {
                self.ThucDonWait([]);
            }
            else {
                lstWait = JSON.parse(lstWait)
                self.ThucDonWait(lstWait);
            }
            localStorage.setItem('foodRequest', JSON.stringify(self.ThucDonYC()));
            localStorage.setItem('foodWait', JSON.stringify(self.ThucDonWait()));
        };

        // used to DongBoHoa (pass arrID_HoaDon)
        hub.client.receiveData_fromThuNgan = function (obj) {
            if (obj.Func === 5) {
                // not use
                if (obj.IDChiNhanh === _idChiNhanh) {
                    loadData_FromTBNB(obj.CTHD);
                }
            }
            else {
                var arrID_HoaDon = obj.ID_HoaDons;
                var lstRq = localStorage.getItem('foodRequest');
                if (lstRq !== null) {
                    lstRq = JSON.parse(lstRq);
                    lstRq = $.grep(lstRq, function (x) {
                        return $.inArray(x.ID_HoaDon, arrID_HoaDon) === -1;
                    });
                    self.ThucDonYC(lstRq);
                    localStorage.setItem('foodRequest', JSON.stringify(lstRq));
                }

                var lstWait = localStorage.getItem('foodWait');
                if (lstWait !== null) {
                    lstWait = JSON.parse(lstWait);
                    lstWait = $.grep(lstWait, function (x) {
                        return $.inArray(x.ID_HoaDon, arrID_HoaDon) === -1;
                    });
                    self.ThucDonWait(lstWait);
                    localStorage.setItem('foodWait', JSON.stringify(lstWait));
                }
            }
        };

        $.connection.hub.start().done(function () {
            connected = true;
            GetThucDonWait();
            GetThucDonYC_FromDB();

            self.DaXong1 = function (item) {
                var _idCTHD = item.ID;
                var _sluongHT = 0;
                if (item.Bep_SoLuongHoanThanh !== null) {
                    _sluongHT = item.Bep_SoLuongHoanThanh;
                }
                item.Bep_SoLuongHoanThanh = _sluongHT + 1;
                item.Bep_SoLuongChoCungUng = item.Bep_SoLuongChoCungUng - 1;

                Delete_PropertiesVAT_whenUpdateCTHD(item);

                var _objUpdate = item;
                var myData = {};
                myData.id = _idCTHD;
                myData.objUpdate = _objUpdate;

                UpDate_BepHoanThanh(myData, hub);
            };

            self.DaXongAll = function (item) {
                var _idCTHD = item.ID;
                item.Bep_SoLuongHoanThanh = item.Bep_SoLuongHoanThanh + item.Bep_SoLuongChoCungUng;
                item.Bep_SoLuongChoCungUng = 0;

                Delete_PropertiesVAT_whenUpdateCTHD(item);

                var _objUpdate = item;
                var myData = {};
                myData.id = _idCTHD;
                myData.objUpdate = _objUpdate;

                UpDate_BepHoanThanh(myData, hub);
            };

            self.ChoCheBien1 = function (item) {
                //update CTHD in DB
                var _idCTHD = item.ID;
                var _sluongConlai = 0;
                if (item.Bep_SoLuongChoCungUng !== null) {
                    _sluongConlai = item.Bep_SoLuongChoCungUng;
                }
                item.Bep_SoLuongChoCungUng = _sluongConlai + 1;

                if (item.Bep_SoLuongYeuCau !== null) {
                    item.Bep_SoLuongYeuCau = item.Bep_SoLuongYeuCau - 1;
                }

                Delete_PropertiesVAT_whenUpdateCTHD(item);

                var _objUpdate = item;
                var myData = {};
                myData.id = _idCTHD;
                myData.objUpdate = _objUpdate;

                Update_BepYeuCau_Wait(myData, hub);
                //PlayAudio();
            }

            self.ChoCheBienAll = function (item) {
                var _idCTHD = item.ID;
                var soluongWait = 0;

                for (var i = 0; i < self.ThucDonWait().length; i++) {
                    if (self.ThucDonWait()[i].ID === _idCTHD) {
                        soluongWait = self.ThucDonWait()[i].Bep_SoLuongChoCungUng;
                        break;
                    }
                }
                item.Bep_SoLuongChoCungUng = item.Bep_SoLuongYeuCau + soluongWait;
                item.Bep_SoLuongYeuCau = 0;

                Delete_PropertiesVAT_whenUpdateCTHD(item);

                var _objUpdate = item;
                var myData = {};
                myData.id = _idCTHD;
                myData.objUpdate = _objUpdate;

                Update_BepYeuCau_Wait(myData, hub);
                //PlayAudio();
            }

            self.HuyDaXong1 = function (item) {
                var _idCTHD = item.ID;
                item.Bep_SoLuongChoCungUng = item.Bep_SoLuongChoCungUng - 1;
                item.Bep_SoLuongYeuCau = item.Bep_SoLuongYeuCau + 1;

                Delete_PropertiesVAT_whenUpdateCTHD(item);

                var _objUpdate = item;
                var myData = {};
                myData.id = _idCTHD;
                myData.objUpdate = _objUpdate;

                Update_BepHoanThanh_Huy(myData, hub);
                //PlayAudio();
            }

        }).fail(function () {
            connected = false;
            console.log("Could not connect!");
        });

        $.connection.hub.disconnected(function () {
            connected = false;
            console.log('disconnect hub')
            setTimeout(function () {
                $.connection.hub.start().done(function () {
                    connected = true;
                    console.log('connect again hub');
                });;
            }, 5000);
        });
    });

    function Update_BepYeuCau_Wait(myData, hub) {
        console.log('Update_BepYeuCau_Wait ', myData);
        $.ajax({
            url: BH_HoaDonUri + "Update_CTHoaDon_Bep",
            type: 'POST',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: myData,
            success: function (obj) {
                if (obj.res === true) {
                    var data = obj.data;
                    console.log(data);

                    // get data from DB --> assign ThucDonWait
                    data.MaHoaDon = myData.objUpdate.MaHoaDon;
                    data.TenPhongBan = myData.objUpdate.TenPhongBan;
                    data.WaitMinutes = myData.objUpdate.WaitMinutes;
                    data.TenHangHoa = myData.objUpdate.TenHangHoa;
                    data.ID_HoaDon = myData.objUpdate.ID_HoaDon;
                    data.ID_ViTri = myData.objUpdate.ID_ViTri;
                    data.ThoiGian = myData.objUpdate.ThoiGian;

                    // bind again lstYeuCau
                    var arrRequest = localStorage.getItem('foodRequest');
                    arrRequest = JSON.parse(arrRequest);
                    for (var i = 0; i < arrRequest.length; i++) {
                        if (arrRequest[i].ID === myData.objUpdate.ID) {
                            if (data.Bep_SoLuongYeuCau === 0) {
                                arrRequest.splice(i, 1);
                            }
                            else {
                                arrRequest[i].Bep_SoLuongYeuCau = data.Bep_SoLuongYeuCau;
                                arrRequest[i].Bep_SoLuongChoCungUng = data.Bep_SoLuongChoCungUng;
                            }
                        }
                    }
                    localStorage.setItem('foodRequest', JSON.stringify(arrRequest));

                    // bind again lstWait
                    var arrWait = JSON.parse(localStorage.getItem('foodWait'));
                    for (var i = 0; i < arrWait.length; i++) {
                        if (arrWait[i].ID === data.ID) {
                            arrWait.splice(i, 1);
                        }
                    }
                    arrWait.unshift(data);
                    localStorage.setItem('foodWait', JSON.stringify(arrWait));

                    LoadThucDonYC_andWait_thisPage(hub, JSON.stringify(arrRequest), JSON.stringify(arrWait), JSON.stringify(data));
                    SendDataNhaBep_toThuNgan(hub, myData.objUpdate.ID_ViTri, myData.objUpdate.MaHoaDon, JSON.stringify(data));
                }
                else {
                    console.log(obj.mes);
                }
            },
        });
    }

    function loadData_FromTBNB(jsonObj) {

        var _now = moment(new Date()).format('YYYY-MM-DD HH:mm');
        var listAllCTHD = jsonObj;
        var arrThucDonYC = [];
        var arrThucDonWait = [];
        if (listAllCTHD !== null) {
            listAllCTHD = JSON.parse(listAllCTHD);
            // load ThucDonYC
            for (let i = 0; i < listAllCTHD.length; i++) {
                if (listAllCTHD[i].Bep_SoLuongYeuCau > 0) {
                    listAllCTHD[i].WaitMinutes = getMinutesBetweenDates(listAllCTHD[i].ThoiGian);
                    arrThucDonYC.push(listAllCTHD[i]);
                }
            }
            // don't bind dichvu theogio
            arrThucDonYC = $.grep(arrThucDonYC, function (x) {
                return x.LaHangHoa || (x.LaHangHoa === false && x.DichVuTheoGio === 0);
            });

            arrThucDonYC.sort(function (a, b) {
                var x = a.ThoiGian,
                    y = b.ThoiGian;
                return x < y ? -1 : x > y ? 1 : 0;
            });

            //console.log('arrThucDonYC ' + JSON.stringify(arrThucDonYC))
            self.ThucDonYC(arrThucDonYC);

            localStorage.setItem('foodRequest', JSON.stringify(self.ThucDonYC()));

            // load ThucDonWait
            for (var i = 0; i < listAllCTHD.length; i++) {
                if (listAllCTHD[i].Bep_SoLuongChoCungUng > 0) {
                    listAllCTHD[i].WaitMinutes = getMinutesBetweenDates(listAllCTHD[i].ThoiGian);
                    arrThucDonWait.push(listAllCTHD[i]);
                }
            }
            self.ThucDonWait(arrThucDonWait);
            localStorage.setItem('foodWait', JSON.stringify(self.ThucDonWait()));
        }
    }

    function LoadThucDonYC_andWait_thisPage(hub, lstRequest, lstWait, dataDB) {
        if (navigator.onLine && connected) {
            hub.server.updateThucDonYC_andWait(lstRequest, lstWait, dataDB);
        }
    }

    function SendDataNhaBep_toThuNgan(hub, idViTri, maHoaDon, dataDB) {
        if (navigator.onLine && connected) {
            hub.server.sendData_NhaBep_toThuNgan(idViTri, maHoaDon, dataDB);
        }
    }

    timer();
    function timer() {
        $('.wait-minute').each(function () {
            var start = $(this).closest('tr').children('td').eq(0).find('div span:last-child').text();
            var sVal = getMinutesBetweenDates(start);
            $(this).text(sVal);
            setTimeout(timer, 1000);
        });

        $('.wait-minuteHT').each(function () {
            var start = $(this).closest('tr').children('td').eq(1).find('div span:last-child').text();
            var sVal = getMinutesBetweenDates(start);
            $(this).text(sVal);
            setTimeout(timer, 10000);
        });
    }
};
ko.applyBindings(new Model_NhaBep());

// check box
var arrIDNhomHang = [];

function SetCheckAll(obj) {
    var isChecked = $(obj).is(":checked");
    $('input[type=checkbox]').each(function () {
        $(this).prop('checked', isChecked);
    })
    if (isChecked) {
        $('input[type=checkbox]').each(function () {
            var thisID = $(this).attr('id');
            if (thisID !== undefined && !(jQuery.inArray(thisID, arrIDNhomHang) > -1)) {
                arrIDNhomHang.push(thisID);
            }
        })
    }
    else {
        arrIDNhomHang = [];
    }
}

function getIDNhomHH(obj) {
    var thisID = $(obj).attr('id');
    if ($(obj).is(':checked')) {
        if (thisID !== undefined && !(jQuery.inArray(thisID, arrIDNhomHang) > -1)) {
            arrIDNhomHang.push(thisID);
        }
    }
    else {
        // remove item in arrID
        $.map(arrIDNhomHang, function (item, i) {
            if (item === thisID) {
                arrIDNhomHang.splice(item, 1);
                return;
            }
        })
    }
}

function getMinutesBetweenDates(startDate) {

    var start = moment(startDate, 'DD/MM/YYYY HH:mm:ss').format('YYYY-MM-DD HH:mm:ss');

    var diff = (new Date() - new Date(start)) / 1000;

    var hours = Math.floor(diff / 3600);
    var minutes = Math.floor(diff % 3600) / 60;

    if (hours > 24) {
        // lay phan nguyen Math.floor
        return Math.floor(hours / 24) + ' ngày trước'
    }
    else {
        if (hours >= 1) {
            return hours + ' giờ trước';
        }
        else {
            if (minutes > 1) {
                return Math.floor(minutes) + ' phút trước';
            }
            else {
                return 'vài giây trước';
            }
        }
    }
}
