var dataMauIn = function () {
    var self = this;
    var item41 = [], item51 = [];
    if (item4 !== null && item4 !== undefined) {
        item41 = item4;
    }
    if (item5 !== null && item5 !== undefined) {
        item51 = item5;
    }
    item1.map(function (x) {
        x["ThanhTienTruocCK"] = RoundDecimal(formatNumberToFloat(x.SoLuong) * formatNumberToFloat(x.DonGia), 3)
        x["TongChietKhau"] = RoundDecimal(formatNumberToFloat(x.SoLuong) * formatNumberToFloat(x.TienChietKhau), 3)
        x["HH_ThueTong"] = RoundDecimal(formatNumberToFloat(x.SoLuong) * formatNumberToFloat(x.TienThue), 3)
    });
    item2.map(function (x) {
        x["ThanhTienTruocCK"] = RoundDecimal(formatNumberToFloat(x.SoLuong) * formatNumberToFloat(x.DonGia), 3)
        x["TongChietKhau"] = RoundDecimal(formatNumberToFloat(x.SoLuong) * formatNumberToFloat(x.TienChietKhau), 3)
        x["HH_ThueTong"] = RoundDecimal(formatNumberToFloat(x.SoLuong) * formatNumberToFloat(x.TienThue), 3)
    });

    item3.TongTienHDSauGiamGia = formatNumberToFloat(item3.TongTienHang)
        - formatNumberToFloat(item3.TongGiamGia)
        - formatNumberToFloat(item3.KhuyeMai_GiamGia);

    item3.TongTienHDSauVAT = formatNumberToFloat(item3.TongTienHang)
        + formatNumberToFloat(item3.TongTienThue)

    self.CTHoaDonPrint = ko.observableArray(item1);
    self.CTHoaDonPrintMH = ko.observableArray(item2);
    self.InforHDprintf = ko.observable(item3);
    self.HangMucSuaChua = ko.observableArray(item41);
    self.VatDungKemTheo = ko.observableArray(item51);

    self.CTHoaDonPrint_VatTu = ko.computed(function () {
        let lst = JSON.parse(JSON.stringify(self.CTHoaDonPrint()));
        lst = lst.filter(x => x.LaHangHoa);
        let arrNhom = [], arrID = [];

        for (let i = 0; i < lst.length; i++) {
            let itfor = lst[i];
            if (!arrID.includes(itfor.ID_NhomHangHoa)) {
                arrID.push(itfor.ID_NhomHangHoa);
                let arrHH = lst.filter(x => x.ID_NhomHangHoa === itfor.ID_NhomHangHoa);
                // assign again STT
                arrHH.map(function (x, i) {
                    x["SoThuTu"] = i + 1;
                })

                let sum = 0, sum_truocVAT = 0, sum_truocCK = 0;
                let sumSL = 0, sumCK = 0, sumVAT = 0;
                for (let k = 0; k < arrHH.length; k++) {
                    let for2 = arrHH[k];
                    sumSL += formatNumberToFloat(for2.SoLuong);
                    sumCK += formatNumberToFloat(for2.TongChietKhau);
                    sumVAT += formatNumberToFloat(for2.TienThue) * formatNumberToFloat(for2.SoLuong);
                    sum += formatNumberToFloat(for2.ThanhToan);
                    sum_truocVAT += formatNumberToFloat(for2.ThanhTien);
                    sum_truocCK += formatNumberToFloat(for2.ThanhTienTruocCK);
                }

                arrNhom.push({
                    SoThuTuNhom: arrNhom.length + 1,
                    SoThuTuNhom_LaMa: ConvertNumber_toRoman(arrNhom.length + 1),
                    ID_NhomHangHoa: itfor.ID_NhomHangHoa,
                    TenNhomHangHoa: itfor.TenNhomHangHoa,
                    TongTienTheoNhom: sum,
                    TongSLTheoNhom: sumSL,
                    TongThueTheoNhom: sumVAT,
                    TongCKTheoNhom: sumCK,
                    TongTienTheoNhom_TruocVAT: sum_truocVAT,
                    TongTienTheoNhom_TruocCK: sum_truocCK,
                    HangHoas: arrHH
                })
            }
        }
        return arrNhom;
    }, this);

    self.CTHoaDonPrint_DichVu = ko.computed(function () {
        let lst = JSON.parse(JSON.stringify(self.CTHoaDonPrint()));
        lst = lst.filter(x => !x.LaHangHoa);
        let arrNhom = [], arrID = [];

        for (let i = 0; i < lst.length; i++) {
            let itfor = lst[i];
            if (!arrID.includes(itfor.ID_NhomHangHoa)) {
                arrID.push(itfor.ID_NhomHangHoa);
                let arrHH = lst.filter(x => x.ID_NhomHangHoa === itfor.ID_NhomHangHoa);
                // assign again STT
                arrHH.map(function (x, i) {
                    x["SoThuTu"] = i + 1;
                })

                let sum = 0, sum_truocVAT = 0, sum_truocCK = 0;
                let sumSL = 0, sumCK = 0, sumVAT = 0;
                for (let k = 0; k < arrHH.length; k++) {
                    let for2 = arrHH[k];
                    sumSL += formatNumberToFloat(for2.SoLuong);
                    sumCK += formatNumberToFloat(for2.TongChietKhau);
                    sumVAT += formatNumberToFloat(for2.TienThue) * formatNumberToFloat(for2.SoLuong);
                    sum += formatNumberToFloat(for2.ThanhToan);
                    sum_truocVAT += formatNumberToFloat(for2.ThanhTien);
                    sum_truocCK += formatNumberToFloat(for2.ThanhTienTruocCK);
                }

                arrNhom.push({
                    SoThuTuNhom: arrNhom.length + 1,
                    SoThuTuNhom_LaMa: ConvertNumber_toRoman(arrNhom.length + 1),
                    ID_NhomHangHoa: itfor.ID_NhomHangHoa,
                    TenNhomHangHoa: itfor.TenNhomHangHoa,
                    TongTienTheoNhom: sum,
                    TongSLTheoNhom: sumSL,
                    TongThueTheoNhom: sumVAT,
                    TongCKTheoNhom: sumCK,
                    TongTienTheoNhom_TruocVAT: sum_truocVAT,
                    TongTienTheoNhom_TruocCK: sum_truocCK,
                    HangHoas: arrHH
                })
            }
        }
        return arrNhom;
    }, this);

    self.CTHoaDonPrint_TheoNhom = ko.computed(function () {
        let arrNhom = [];
        let arrID = [];
        let newChiTietHoaDon = JSON.parse(JSON.stringify(self.CTHoaDonPrint()));
        for (let i = 0; i < newChiTietHoaDon.length; i++) {
            let itfor = newChiTietHoaDon[i];
            if (!arrID.includes(itfor.ID_NhomHangHoa)) {
                arrID.push(itfor.ID_NhomHangHoa);
                let arrHH = newChiTietHoaDon.filter(x => x.ID_NhomHangHoa === itfor.ID_NhomHangHoa);
                // assign again STT
                arrHH.map(function (x, i) {
                    x["SoThuTu"] = i + 1;
                })

                let sum = 0, sum_truocVAT = 0, sum_truocCK = 0;
                let sumSL = 0, sumCK = 0, sumVAT = 0;
                for (let k = 0; k < arrHH.length; k++) {
                    let for2 = arrHH[k];
                    sumSL += formatNumberToFloat(for2.SoLuong);
                    sumCK += formatNumberToFloat(for2.TongChietKhau);
                    sumVAT += formatNumberToFloat(for2.TienThue) * formatNumberToFloat(for2.SoLuong);
                    sum += formatNumberToFloat(for2.ThanhToan);
                    sum_truocVAT += formatNumberToFloat(for2.ThanhTien);
                    sum_truocCK += formatNumberToFloat(for2.ThanhTienTruocCK);
                }

                arrNhom.push({
                    SoThuTuNhom: arrNhom.length + 1,
                    SoThuTuNhom_LaMa: ConvertNumber_toRoman(arrNhom.length + 1),
                    ID_NhomHangHoa: itfor.ID_NhomHangHoa,
                    TenNhomHangHoa: itfor.TenNhomHangHoa,
                    TongTienTheoNhom: sum,
                    TongSLTheoNhom: sumSL,
                    TongThueTheoNhom: sumVAT,
                    TongCKTheoNhom: sumCK,
                    TongTienTheoNhom_TruocVAT: sum_truocVAT,
                    TongTienTheoNhom_TruocCK: sum_truocCK,
                    HangHoas: arrHH
                })
            }
        }
        return arrNhom;
    }, this);

    self.CTHoaDon_TheoNhom_VaHHDV = ko.computed(function () {
        let arrNhom = [], arrID = [];
        let cthdNew = JSON.parse(JSON.stringify(self.CTHoaDonPrint()));
        for (let i = 0; i < cthdNew.length; i++) {
            let itfor = cthdNew[i];
            if (!arrID.includes(itfor.ID_NhomHangHoa)) {
                arrID.push(itfor.ID_NhomHangHoa);
                let arrHH = cthdNew.filter(x => x.ID_NhomHangHoa === itfor.ID_NhomHangHoa);

                let nhom_sum = 0, nhom_sum_truocVAT = 0, nhom_sum_truocCK = 0;
                let nhom_sumSL = 0, nhom_sumCK = 0, nhom_sumVAT = 0;

                // hanghoa theo nhom
                let hh_sum = 0, hh_sum_truocVAT = 0, hh_sum_truocCK = 0;
                let hh_sumSL = 0, hh_sumCK = 0, hh_sumVAT = 0;
                let lstHH = arrHH.filter(x => x.LaHangHoa);
                lstHH.map(function (x, index) {
                    x["SoThuTu"] = index + 1;
                })

                if (lstHH.length > 0) {
                    for (let k = 0; k < lstHH.length; k++) {
                        let for2 = lstHH[k];
                        hh_sumSL += formatNumberToFloat(for2.SoLuong);
                        hh_sumCK += formatNumberToFloat(for2.TongChietKhau);
                        hh_sumVAT += formatNumberToFloat(for2.TienThue) * formatNumberToFloat(for2.SoLuong);
                        hh_sum += formatNumberToFloat(for2.ThanhToan);
                        hh_sum_truocVAT += formatNumberToFloat(for2.ThanhTien);
                        hh_sum_truocCK += formatNumberToFloat(for2.ThanhTienTruocCK);
                    }

                    nhom_sumSL = hh_sumSL;
                    nhom_sumCK = hh_sumCK;
                    nhom_sumVAT = hh_sumVAT;
                    nhom_sum = hh_sum;
                    nhom_sum_truocVAT = hh_sum_truocVAT;
                    nhom_sum_truocCK = hh_sum_truocCK;
                }

                // dichvu theo nhom
                let lstDV = arrHH.filter(x => !x.LaHangHoa);
                lstDV.map(function (x, index) {
                    x["SoThuTu"] = index + 1;
                })

                let dv_sum = 0, dv_sum_truocVAT = 0, dv_sum_truocCK = 0;
                let dv_sumSL = 0, dv_sumCK = 0, dv_sumVAT = 0;
                if (lstDV.length > 0) {
                    for (let k = 0; k < lstDV.length; k++) {
                        let for2 = lstDV[k];
                        dv_sumSL += formatNumberToFloat(for2.SoLuong);
                        dv_sumCK += formatNumberToFloat(for2.TongChietKhau);
                        dv_sumVAT += formatNumberToFloat(for2.TienThue) * formatNumberToFloat(for2.SoLuong);
                        dv_sum += formatNumberToFloat(for2.ThanhToan);
                        dv_sum_truocVAT += formatNumberToFloat(for2.ThanhTien);
                        dv_sum_truocCK += formatNumberToFloat(for2.ThanhTienTruocCK);
                    }

                    nhom_sumSL += dv_sumSL;
                    nhom_sumCK += dv_sumCK;
                    nhom_sumVAT += dv_sumVAT;
                    nhom_sum += dv_sum;
                    nhom_sum_truocVAT += dv_sum_truocVAT;
                    nhom_sum_truocCK += dv_sum_truocCK;
                }

                arrNhom.push({
                    SoThuTuNhom: arrNhom.length + 1,
                    SoThuTuNhom_LaMa: ConvertNumber_toRoman(arrNhom.length + 1),
                    ID_NhomHangHoa: itfor.ID_NhomHangHoa,
                    TenNhomHangHoa: itfor.TenNhomHangHoa,
                    TongTienTheoNhom: nhom_sum,
                    TongSLTheoNhom: nhom_sumSL,
                    TongThueTheoNhom: nhom_sumVAT,
                    TongCKTheoNhom: nhom_sumCK,
                    TongTienTheoNhom_TruocVAT: nhom_sum_truocVAT,
                    TongTienTheoNhom_TruocCK: nhom_sum_truocCK,
                    ListHangHoas: {
                        HangHoas: lstHH,
                        NhomHH_SoLuong: hh_sumSL,
                        NhomHH_TongThue: hh_sumVAT,
                        NhomHH_ChietKhau: hh_sumCK,
                        NhomHH_TruocVAT: hh_sum_truocVAT,
                        NhomHH_TruocCK: hh_sum_truocCK,
                        NhomHH_ThanhToan: hh_sum,
                    },
                    ListDichVus: {
                        DichVus: lstDV,
                        NhomDV_SoLuong: dv_sumSL,
                        NhomDV_TongThue: dv_sumVAT,
                        NhomDV_ChietKhau: dv_sumCK,
                        NhomDV_TruocVAT: dv_sum_truocVAT,
                        NhomDV_TruocCK: dv_sum_truocCK,
                        NhomDV_ThanhToan: dv_sum,
                    },
                })
            }
        }
        return arrNhom;
    }, this);
};
ko.applyBindings(new dataMauIn());
// used to formatNumber when print at ThietLapAPI
function formatNumber(number, decimalDot = 2) {
    if (number === undefined || number === null) {
        return 0;
    }
    else {
        number = formatNumberToFloat(number);
        number = Math.round(number * Math.pow(10, decimalDot)) / Math.pow(10, decimalDot);
        if (number !== null) {
            var lastone = number.toString().split('').pop();
            if (lastone !== '.') {
                number = parseFloat(number);
            }
        }
        if (isNaN(number)) {
            number = 0;
        }
        return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
}
function formatNumberToFloat(objVal) {
    if (objVal === undefined || objVal === null) {
        return 0;
    }
    else {
        var value = parseFloat(objVal.toString().replace(/,/g, ''));
        if (isNaN(value)) {
            return 0;
        }
        else {
            return value;
        }
    }
}

function RoundDecimal(data, number) {
    number = number || 2;
    data = Math.round(data * Math.pow(10, 2)) / Math.pow(10, 2);
    if (data !== null) {
        var lastone = data.toString().split('').pop();
        if (lastone !== '.') {
            data = parseFloat(data);
        }
    }
    if (isNaN(data) || data === Infinity) {
        data = 0;
    }
    return data;
}

function ConvertMinutes_ToHourMinutes(sophut) {
    sophut = parseFloat(sophut);
    sophut = Math.round(sophut * Math.pow(10, 2)) / Math.pow(10, 2);
    var lastone = sophut.toString().split('').pop();
    if (lastone !== '.') {
        sophut = parseFloat(sophut);
    }
    var div = sophut / 60;
    var hours = Math.floor(div);
    var minutes = parseFloat((div - hours) * 60);
    if (hours > 0) {
        return hours.toString().concat(' giờ ', minutes, ' phút');
    }
    return minutes.toString().concat(' phút');
}

function ConvertNumber_toRoman(num) {
    if (!+num)
        return false;
    var digits = String(+num).split(""),
        key = ["", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM",
            "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC",
            "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX"],
        roman = "",
        i = 3;
    while (i--)
        roman = (key[+digits.pop() + (i * 10)] || "") + roman;
    return Array(+digits.join("") + 1).join("M") + roman;
}

