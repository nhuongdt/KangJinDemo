var modelThuNgan = new NewModel_HoaDon();

$(function() {
    var hub = $.connection.AlertHub;
    hub.client.send = function(jsonObj) {
        LoadCTHD_FromNhaBep(jsonObj);
    };

    $.connection.hub.start().done(function() {
        console.log("Now connected, connection ID: " + $.connection.hub.id);
        $('#btnThongBaoNhaBep').click(function () {
            $(this).attr('disabled', 'disabled');
            $(this).removeClass('bg7');
            $(this).addClass('btnDisable');

            InsertDB_ThongBaoNB(hub);
        });
    }).fail(function() {
        console.log("Could not connect!");
    });
});

function InsertDB_ThongBaoNB(hub) {
    var idVT = localStorage.getItem('phongbanIDCache');
    var listAllCTHD = localStorage.getItem('listAllCTHD');
    var arrTBao = [];
    if (listAllCTHD !== null) {
        listAllCTHD = JSON.parse(listAllCTHD);
        //update Cache CTHD with Satus = 1;
        for (var i = 0; i < listAllCTHD.length; i++) {
            if (listAllCTHD[i].MaHoaDon === _maHoaDon && listAllCTHD[i].ID_ViTri === idVT) {
                var _soluongYC = listAllCTHD[i].SoLuong;
                //truong hop sluong ycau da giam di, sau do tang soluong ycau len
                if (listAllCTHD[i].Bep_SoLuongYeuCau > 0) {
                    _soluongYC = _soluongYC - (listAllCTHD[i].Bep_SoLuongYeuCau +
                        listAllCTHD[i].Bep_SoLuongChoCungUng + listAllCTHD[i].Bep_SoLuongHoanThanh) + listAllCTHD[i].Bep_SoLuongYeuCau;
                }
                listAllCTHD[i].Bep_SoLuongYeuCau = _soluongYC;
                listAllCTHD[i].Status = 1;
                arrTBao.push(listAllCTHD[i]);
            }
        }

        var lcHD = localStorage.getItem('lstHDTemp');
        if (lcHD !== null) {
            lcHD = JSON.parse(lcHD);
            var itemEx = [];
            $.map(lcHD, function(item) {
                if (item.ID_ViTri === idVT && item.MaHoaDon === _maHoaDon) {
                    item.ChoThanhToan = true;
                    itemEx = item;
                    return;
                }
            })

            //chi luu vao DBkhi online
            if (navigator.onLine) {
                var myData = {};
                myData.objHoaDon = itemEx;
                myData.objCTHoaDon = arrTBao;

                //chi update cache CTHD if chon KH
                localStorage.setItem('listAllCTHD', JSON.stringify(listAllCTHD));
                $.ajax({
                    data: myData,
                    url: '/api/DanhMuc/BH_HoaDonAPI/' + "ThongBaoNhabep",
                    type: 'POST',
                    async: true,
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",

                    success: function(item) {
                        //update ID -> cache with maHoaDon
                        var lstHD = JSON.parse(localStorage.getItem('lstHDTemp'));
                        for (var i = 0; i < lstHD.length; i++) {
                            if (lstHD[i].ID_ViTri === idVT && lstHD[i].MaHoaDon === _maHoaDon) {
                                lstHD[i].ID = item.ID;
                                lstHD[i].MaHoaDon = item.MaHoaDon;
                                break;
                            }
                        }
                        localStorage.setItem('lstHDTemp', JSON.stringify(lstHD));

                        //read again PhongBanSelected with ID_ViTri new
                        var arrAfterMove = $.grep(lstHD, function(item) {
                            return item.ID_ViTri === itemEx.ID_ViTri;
                        })
                        modelThuNgan.PhongBanSelected(arrAfterMove);
                        //bind infor HoaDon
                        var arrReturn = GetArr_InforHD(itemEx);
                        modelThuNgan.HoaDons(arrReturn);

                        $('.tab-table-right li').removeClass('active');
                        $('.tab-table-right li:eq(0)').addClass('active');

                        // update ID_CTHD in Cache CTHD (todo)
                        for (var i = 0; i < item.BH_HoaDon_ChiTiet.length; i++) {
                            for (var j = 0; j < listAllCTHD.length; j++) {
                                if (listAllCTHD[j].ID_ViTri === idVT && listAllCTHD[j].MaHoaDon === itemEx.MaHoaDon &&
                                    listAllCTHD[j].ID_DonViQuiDoi === item.BH_HoaDon_ChiTiet[i].ID_DonViQuiDoi) {
                                    listAllCTHD[j].ID = item.BH_HoaDon_ChiTiet[i].ID;
                                    listAllCTHD[j].MaHoaDon = item.MaHoaDon;
                                    continue;
                                }
                            }
                        }
                        localStorage.setItem('listAllCTHD', JSON.stringify(listAllCTHD));
                        console.log('insertDB_TBNB ' + JSON.stringify(listAllCTHD))

                        _maHoaDon = item.MaHoaDon;

                        hub.server.send(JSON.stringify(listAllCTHD));

                        // style width
                        $("#tab-table-right").css("right", "0");
                        $("#tab-table-right").css("left", "20");
                        var widthArr = modelThuNgan.PhongBanSelected().length * 200;
                        $("#tab-table-right").width(widthArr);

                        //read again CTHoaDon
                        $(function() {
                            var liFirst = $('.tab-table-right li:eq(0) a').text();
                            // Get CTHoaDon t/ung
                            var ctHD = $.grep(listAllCTHD, function(item) {
                                return item.MaHoaDon === liFirst;
                            });
                            modelThuNgan.HangHoaAfterAdds(ctHD);
                        });
                        bottomrightnotify('Cập nhật thành công', 'success');
                    },
                    statusCode: {
                        404: function() {
                            console.log("page not found");
                        },
                    },
                    error: function(jqXHR, textStatus, errorThrown) {
                        console.log('err');
                    },
                    complete: function() {
                        $('.bs-example-modal-lg').modal('hide');
                        //btnThongBaoNhaBep
                        $('.nt1').attr('disabled', 'disabled');
                    }
                })
            }
        }
    }
}

function LoadCTHD_FromNhaBep(jsonObj) {
    console.log('maHD ' + _maHoaDon);
    console.log('loafFromBep_ ' + jsonObj);

    var idVT = localStorage.getItem('phongbanIDCache');
    var lstCTHD = jsonObj;
    if (lstCTHD !== null) {
        lstCTHD = JSON.parse(lstCTHD);
        //var arrCTHD = $.grep(lstCTHD, function (item) {
        //    return item.MaHoaDon === _maHoaDon && item.ID_ViTri === idVT;
        //});
        modelThuNgan.HangHoaAfterAdds(lstCTHD);
        console.log('HangHoaAfterAdds ' + JSON.stringify(modelThuNgan.HangHoaAfterAdds()));
    }
}

