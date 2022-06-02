var hub = $.connection.AlertHub;
//var displayHub = $.connection.DisplayHub;

var connected = false;
$.connection.hub.start().done(function () {
    connected = true;
    pingInterval: 60000,
        console.log('Connect! connection Id=' + $.connection.hub.id);
});
$.connection.hub.disconnected(function () {
    console.log('disconnect hub')
    setTimeout(function () {
        console.log('connect again hub')
        $.connection.hub.start();
    }, 3000); // Restart connection after 3 seconds.
});

var Model_DisplayCustomer = function () {
    const lcHD = 'lstHDPos';
    const lcCTHD = 'lstCTHDPos';
    const lcProductKM_HoaDon = 'productKM_HoaDon';
    var _subDomain = $('#subDomain').val();
    var _idUser = $('#txtIDUser').val();
    console.log(_subDomain, _idUser)

    self.SumProduct = ko.observable();
    self.SumQuantity = ko.observable();

    self.HangHoaAfterAdds = ko.observableArray();
    self.HHTang_HoaDon = ko.observableArray();
    self.NewProducts = ko.observableArray();
    self.HangHoaChosed = ko.observableArray();
    self.HoaDons = ko.observableArray();
    self.ListGroupSale = ko.observableArray();
    self.ListHoaDons = ko.observableArray([]);
    self.result_HoaDon = ko.observableArray([]);

    // paging
    self.indexOfHD = ko.observable(0);
    self.pageSize = ko.observable(2);

    function Page_Load() {
        var idRandom = localStorage.getItem('lcIDRandom');
        BindHD_CTHD(idRandom);
    }
    Page_Load();

    function Caculator_AmountProduct(idRandom) {
        var lstCTHD = localStorage.getItem(lcCTHD);
        if (lstCTHD !== null) {
            lstCTHD = JSON.parse(lstCTHD);

            // find hd opening--> get IDRandom
            var lstHD = localStorage.getItem(lcHD);
            lstHD = JSON.parse(lstHD);
            var itemEx = $.grep(lstHD, function (x) {
                return x.IDRandom === idRandom;
            });

            var sumQuantity = 0;
            var sumProduct = 0;
            if (itemEx.length > 0) {
                var arrCTHDopen = $.grep(lstCTHD, function (x) {
                    return x.IDRandomHD === idRandom;
                });

                if (arrCTHDopen.length > 0) {
                    sumProduct = arrCTHDopen.length;

                    for (let i = 0; i < arrCTHDopen.length; i++) {
                        sumQuantity += parseFloat(arrCTHDopen[i].SoLuong);

                        // count HH Khuyen mai of Hang hoa
                        for (let j = 0; j < arrCTHDopen[i].HangHoa_KM.length; j++) {
                            sumQuantity += parseFloat(arrCTHDopen[i].HangHoa_KM[j].SoLuong);
                            sumProduct += 1;
                        }

                        // count Lot in Hang hoa
                        for (let k = 1; k < arrCTHDopen[i].DM_LoHang.length; k++) {
                            sumQuantity += parseFloat(arrCTHDopen[i].DM_LoHang[k].SoLuong);
                        }
                    }
                }

                // count HH Khuyen mai of  Hoa don
                var lstKM_ProductHD = localStorage.getItem(lcProductKM_HoaDon);
                if (lstKM_ProductHD !== null) {
                    lstKM_ProductHD = JSON.parse(lstKM_ProductHD);
                    lstKM_ProductHD = $.grep(lstKM_ProductHD, function (x) {
                        return x.IDRandomHD === idRandom;
                    });

                    for (let k = 0; k < lstKM_ProductHD.length; k++) {
                        sumQuantity += parseFloat(lstKM_ProductHD[k].SoLuong);
                        sumProduct += 1;
                    }
                }
            }

            // round number to 3 decimals (OK)
            var numberRound = Math.round(sumQuantity * 1000) / 1000;
            self.SumQuantity(numberRound);
            self.SumProduct(sumProduct);
        }
    }

    //$("#divDisplayCus").on('LoadInvoice', function (e, param) {
    //    console.log('param ', param)
    //    BindHD_CTHD(param);
    //});

    function ResetInforHD() {
        self.HoaDons([]);
        self.HangHoaAfterAdds([]);
        self.SumQuantity(0);
        self.SumProduct(0)
    }

    function BindHD_CTHD(idRandom) {
        // get cache from local cache
        var lstHD = localStorage.getItem(lcHD);
        var lstCTHD = localStorage.getItem(lcCTHD);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);
            if (lstCTHD !== null) {
                lstCTHD = JSON.parse(lstCTHD);
            }
            else {
                lstCTHD = [];
            }
            // only bind with LoaiHoaDon = 1
            var arrHD = $.grep(lstHD, function (x, index) {
                return x.IDRandom === idRandom && x.LoaiHoaDon === 1;
            });
            var arrCTHD = [];
            if (arrHD.length > 0) {
                self.HoaDons(arrHD[0]);
                arrCTHD = $.grep(lstCTHD, function (x) {
                    return x.IDRandomHD === idRandom;
                });
            }

            if (arrCTHD.length > 0) {
                self.HangHoaAfterAdds(arrCTHD);
                Caculator_AmountProduct(idRandom);
            }
            else {
                ResetInforHD();
            }
        }
        else {
            ResetInforHD();
        }
    }

    hub.client.receiveData_fromBanHang = function (objSend) {
        if (objSend.SubDomain === _subDomain && objSend.ID_User === _idUser) {
            switch (objSend.Func) {
                case 1: //add hanghoa
                    var idRandomSend = objSend.IDRandomHD;
                    var hdSend = $.grep(objSend.HD, function (x) {
                        return x.IDRandom === idRandomSend;
                    });
                    var cthdSend = $.grep(objSend.CTHD, function (x) {
                        return x.IDRandomHD === idRandomSend;
                    });
                    localStorage.setItem(lcHD, JSON.stringify(hdSend));
                    localStorage.setItem(lcCTHD, JSON.stringify(cthdSend));
                    break;
                case 2: // delete CTHD
                    break;
                case 3: // close HD
                    break;
            }
            BindHD_CTHD(objSend.IDRandomHD);
            localStorage.setItem('lcIDRandom', objSend.IDRandom)
        }
    }

    //displayHub.client.online = function (count) {
    //    $("#onlineUsers").html(count);
    //};
}
ko.applyBindings(new Model_DisplayCustomer())
