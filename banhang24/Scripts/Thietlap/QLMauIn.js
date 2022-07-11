
//===============================
// Ckeditor
//===============================
//CKEDITOR.config.allowedContent = true;
//CKEDITOR.replace('txtCkeditor', {
//    height: '42em',
//    toolbar: [[
//        'Bold', 'Italic', 'Underline', 'Subscript',
//        'Superscript', 'Image', 'Table', 'NumberedList',
//        'BulletedList', 'Outdent', 'Indent', 'JustifyLeft',
//        'JustifyCenter', 'JustifyRight', 'JustifyBlock', 'TextColor', 'Maximize', 'Source',
//        'FontSize', 'Format', 'Font'
//    ]]
//},
//    {
//        entermode: CKEDITOR.ENTER_BR,
//    });


String.prototype.allReplace = function (obj) {
    var retStr = this;
    for (var x in obj) {
        retStr = retStr.replace(new RegExp(x, 'g'), obj[x]);
    }
    return retStr;
};

ko.bindingHandlers.iframeContent = {
    update: function (element, valueAccessor) {
        var value = ko.unwrap(valueAccessor());
        element.contentWindow.document.close(); // Clear the content
        element.contentWindow.document.write(value);
    }
};
var PrintModel_HoaDon = function () {
    //===============================
    // Declare parameter
    //===============================
    var self = this;
    self.TenCuaHang = ko.observable();
    self.ListMauIn = ko.observableArray([]);
    self.MaChungTu = ko.observable("DH");
    self.TenMauInAddNew = ko.observable();
    self.TitleEdit = ko.observable();
    self.dataCkeditor = ko.observable();
    self.selectedKhoGiay = ko.observable();
    self.Showbutton = ko.observable(true);
    self.MauInID = ko.observable();
    self.checkloadMauIn = ko.observable(true);
    self.CheckVisibleEdit = ko.computed(function () {
        return (self.MauInID() !== null && self.MauInID() !== "" && self.MauInID() !== "00000000-0000-0000-0000-000000000000");
    });
    self.CheckInsertUpdate = ko.observable(false);

    self.ShowPopupEdit = function () {
        $('#SelecttedKhoGiay').prop('disabled', false);
        self.CheckInsertUpdate(false);
        $('#SelecttedKhoGiay').val(self.selectedKhoGiay());
        self.TitleEdit("Cập nhật mẫu in");
        $('#themmoi').modal('show');
    }
    self.ShowPopupAddNew = function () {
        $('#SelecttedKhoGiay').prop('disabled', false);
        self.CheckInsertUpdate(true);
        self.MauInID(null);
        self.TenMauInAddNew(null);
        self.TitleEdit("Thêm mới mẫu in");
        $('#themmoi').modal('show');
    }
    //===============================
    // Lưu dữ liệu mâu in
    //===============================
    self.EditDuLieuMauIn = function () {
        if (self.MauInID() === null || self.MauInID() === "00000000-0000-0000-0000-000000000000") {
            self.CheckInsertUpdate(true);
            self.MauInID(null);
            self.TenMauInAddNew(null);
            self.TitleEdit("Thêm mới mẫu in");
            $('#themmoi').modal('show');
            $('#SelecttedKhoGiay').prop('disabled', true);
        }
        else {
            var model = JSON.stringify({
                Id: self.MauInID(),
                //DuLieuMauIn: CKEDITOR.instances['txtCkeditor'].getData()
                DuLieuMauIn: tinymce.get("txtCkeditor").getContent()
            });


            $.ajax({
                data: model,
                url: "/t/SaveMauIn",
                type: 'POST',
                dataType: 'json',
                contentType: "application/json; charset=utf-8",
                success: function (result) {

                    AjaxOnSuccess(result);
                }
            });
        }
    }
    self.selectedMauIn = ko.observable();
    //===============================
    // Select MauIn
    //===============================
    self.selectedMauIn.subscribe(function (newval) {
        if (self.checkloadMauIn() === true) {
            loadData('/t/EventChangeLoaiMauIn?id=' + newval + '&&MaChungTu=' + self.MaChungTu());
        }
        else {
            self.checkloadMauIn(false);
        }
    });
    //===============================
    // Thêm mới dữ liệu mẫu in
    //===============================
    self.AddNewMauIn = function () {
        if (self.TenMauInAddNew() !== null && self.TenMauInAddNew().replace(/\s+/g, '') !== "") {
            if (self.MaChungTu()==='DH') {
                self.MaChungTu('BG');
            }
            var khogiay = $('#SelecttedKhoGiay').val();
            var model = {
                Id: self.MauInID(),
                Name: self.TenMauInAddNew(),
                KhoGiayId: khogiay,
                MaChungTu: self.MaChungTu()
            };
            var Url = "/t/EditMauIn";
            if (self.CheckInsertUpdate() === true) {
                Url = "/t/AddNewMauIn";
            }
            $.ajax({
                data: model,
                url: Url,
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (result) {
                    AjaxOnSuccess(result);
                }
            });
        }
        else {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> Vui lòng nhập tên mẫu in', 'danger');
        }

    }
    //===============================
    // Delete mẫu in
    //===============================
    self.DeleteMauIn = function () {
        $.ajax({
            data: JSON.stringify({ Id: self.MauInID() }),
            url: "/t/DeleteMauIn",
            type: 'POST',
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                AjaxOnSuccess(result);
            }
        });
    }

    self.CopyMauIn = function () {
        $.ajax({
            data: JSON.stringify({ Id: self.MauInID() }),
            url: "/t/CopyMauIn",
            type: 'POST',
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                AjaxOnSuccess(result);
            }
        });

        let table = $('#InputPrint');
        TableToExcel.convert(table[0], { // html code may contain multiple tables so here we are refering to 1st table tag
            name: `export.xlsx`, // fileName you could use any name
            sheet: {
                name: 'Sheet 1' // sheetName
            }
        });
    }
    //===============================
    // Return call ajax success
    //===============================
    function AjaxOnSuccess(result) {
        if (result.res === true) {
            $('#themmoi').modal('hide');
            bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + result.mess, 'success');
            self.dataCkeditor(result.data.Data);
            self.selectedKhoGiay(result.data.selectedKhoGiay);
            self.ListMauIn(result.data.listMauIn);
            self.TenMauInAddNew(result.data.Name);
            self.MauInID(result.data.SelectedMauIn);
            $('#selectMauIn').val(result.data.SelectedMauIn);
            //CKEDITOR.instances['txtCkeditor'].setData(modelMauIn.dataCkeditor());
            tinymce.get("txtCkeditor").setContent(modelMauIn.dataCkeditor());
        }
        else {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + result.mess, 'danger');
        }
    }
    self.dataIframe = ko.computed(function () {
        var data = SetConvertDataTest(self.dataCkeditor());
        data = data.concat('<script src="/Scripts/knockout-3.4.2.js"></script>');
        data = data.concat("<script > var logoImage='" + logoImage + "'; </script>");
        data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/DataMauIn.js'></script>");
        return data;
    });
    self.IsInvoice = ko.observable(true);
    return self;
};
var modelMauIn = new PrintModel_HoaDon();
ko.applyBindings(modelMauIn);

$(document).ready(function () {
    tinymce.get("txtCkeditor").on('change', function () {
        //modelMauIn.dataCkeditor(CKEDITOR.instances['txtCkeditor'].getData());
        modelMauIn.dataCkeditor(tinymce.get("txtCkeditor").getContent());
    });

})

//===============================
// Load Mau in and set data for view
//===============================
function loadData(url) {
    $.ajax({
        url: url,
        dataType: 'json',
        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
        success: function (result) {
            modelMauIn.MauInID(result.data.SelectedMauIn);
            modelMauIn.ListMauIn(result.data.listMauIn);
            modelMauIn.dataCkeditor(result.data.Data);
            modelMauIn.TenMauInAddNew(result.data.Name);
            modelMauIn.selectedKhoGiay(result.data.selectedKhoGiay);
            if (result.res === true) {
                modelMauIn.Showbutton(true);
            }
            else {
                modelMauIn.Showbutton(false);
            }
            //CKEDITOR.instances['txtCkeditor'].setData(result.data.Data);
            tinymce.get("txtCkeditor").setContent(result.data.Data);


        }
    });
}
function loadMauIn(url, machungtu) {
    $.ajax({
        url: url, type: 'GET',
        dataType: 'json',
        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
        success: function (result) {
            modelMauIn.MaChungTu(machungtu);
            modelMauIn.MauInID(result.data.SelectedMauIn);
            modelMauIn.checkloadMauIn(false);
            setTimeout(function () { modelMauIn.checkloadMauIn(true); }, 1000);
            modelMauIn.ListMauIn(result.data.listMauIn);
            modelMauIn.dataCkeditor(result.data.Data);
            modelMauIn.TenMauInAddNew(result.data.Name);
            modelMauIn.selectedKhoGiay(result.data.selectedKhoGiay);

            if (result.res === true) {
                modelMauIn.Showbutton(true);
            }
            else {
                modelMauIn.Showbutton(false);
            }
            modelMauIn.selectedMauIn(result.data.SelectedMauIn);
            //CKEDITOR.instances['txtCkeditor'].setData(result.data.Data);
            tinymce.get("txtCkeditor").setContent(result.data.Data);
            //$('#txtCkeditor').html(result.data.Data);
            //$('#txtCkeditor').html("abc");
        }
    });
}
//===============================
// Change Mau in
//===============================
var keyid;
$('.establish-left').on('click', 'li a', function () {
    keyid = $(this).closest('li').data("id");
    if (keyid === 'HDBL' || keyid === 'DH') {
        modelMauIn.IsInvoice(true);
    }
    else {
        modelMauIn.IsInvoice(false)
    }
    loadMauIn('/t/EventChangeMauIn?MaChungTu=' + keyid + '&&typeKhoGiay=0', keyid);
    $('.establish-left li').each(function (i) {
        $(this).removeClass('active');
    });
    $(this).closest('li').addClass('active');
});

function Replace_TheoNhom(content) {
    content = content.allReplace(
        {
            '{TenNhomHangHoa}': '<span data-bind=\"text: TenNhomHangHoa\"> </span>',
            '{SoThuTuNhom}': '<span data-bind=\"text: SoThuTuNhom\"></span>',
            '{SoThuTuNhom_LaMa}': '<span data-bind=\"text: SoThuTuNhom_LaMa\"></span>',
            '{TongTienTheoNhom}': '<span data-bind=\"text: TongTienTheoNhom\"></span>',
            '{TongTienTheoNhom_TruocVAT}': '<span data-bind=\"text: TongTienTheoNhom_TruocVAT\"></span>',
            '{TongTienTheoNhom_TruocCK}': '<span data-bind=\"text: TongTienTheoNhom_TruocCK\"></span>',
            '{TongSLTheoNhom}': '<span data-bind=\"text: TongSLTheoNhom\"></span>',
            '{TongThueTheoNhom}': '<span data-bind=\"text: TongThueTheoNhom\"></span>',
            '{TongCKTheoNhom}': '<span data-bind=\"text: TongCKTheoNhom\"></span>',
        });
    return content;
}

function Replace_HangMucSC(content) {
    if (content.indexOf("{TinhTrang}") > -1 || content.indexOf("{TenHangMuc}") > -1 || content.indexOf("{PhuongAnSuaChua}") > -1) {
        let hm_from = -1, hm_to = -1;
        if (content.indexOf("{TenHangMuc}") > -1) {
            hm_from = content.lastIndexOf("tbody", content.indexOf("{TenHangMuc}")) - 1;
            hm_to = content.indexOf("tbody", content.indexOf("{TenHangMuc}")) + 6;
        }
        else {
            if (content.indexOf("{TinhTrang}") > -1) {
                hm_from = content.lastIndexOf("tbody", content.indexOf("{TinhTrang}")) - 1;
                hm_to = content.indexOf("tbody", content.indexOf("{TinhTrang}")) + 6;
            }
        }
        if (hm_from != -1 && hm_to != -1) {
            let hm_tbl = content.substr(hm_from, hm_to - hm_from);
            let hm_tblGoc = hm_tbl;
            hm_tbl = hm_tbl.replace("tbody", "tbody data-bind=\"foreach: HangMucSuaChua\"");
            hm_tbl = hm_tbl.replace("{STT}", "<span data-bind=\"text: STT\"></span>");
            hm_tbl = hm_tbl.replace("{TenHangMuc}", "<span data-bind=\"text: TenHangMuc\"></span>");
            hm_tbl = hm_tbl.replace("{TinhTrang}", "<span data-bind=\"text: TinhTrang\"></span>");
            hm_tbl = hm_tbl.replace("{PhuongAnSuaChua}", "<span data-bind=\"text: PhuongAnSuaChua\"></span>");
            content = content.replace(hm_tblGoc, hm_tbl);
        }
    }
    return content;
}

function Replace_VatDungKemTheo(content) {
    if (content.indexOf("{TieuDe}") != -1) {
        let vd_from = content.lastIndexOf("tbody", content.indexOf("{TieuDe}")) - 1;
        let vd_to = content.indexOf("tbody", vd_from + 5) + 6;
        let vd_tbl = content.substr(vd_from, vd_to - vd_from);
        let vd_tblGoc = vd_tbl;
        vd_tbl = vd_tbl.replace("tbody", "tbody data-bind=\"foreach: VatDungKemTheo\"");
        vd_tbl = vd_tbl.replace("{STT}", "<span data-bind=\"text: STT\"></span>");
        vd_tbl = vd_tbl.replace("{TieuDe}", "<span data-bind=\"text: TieuDe\"></span>");
        vd_tbl = vd_tbl.replace("{SoLuong}", "<span data-bind=\"text: SoLuong\"></span>");
        content = content.replace(vd_tblGoc, vd_tbl);
    }
    return content;
}

function ReplaceCTHD(content) {
    content = content.allReplace(
        {
            '{MaHangHoa}': '<span data-bind=\"text: MaHangHoa\"></span>',
            '{TonLuyKe}': '<span data-bind=\"text: TonLuyKe\"></span>',
            '{MaLoHang}': '<span data-bind=\"text: MaLoHang\"></span>',
            '{TenHangHoa}': '<span data-bind=\"text: TenHangHoa\"></span>',
            '{TenHangHoaThayThe}': '<span data-bind=\"text: TenHangHoaThayThe\"></span>',
            '{GiaVonHienTai}': '<span data-bind=\"text: GiaVonHienTai\"></span>',
            '{GiaVonMoi}': '<span data-bind=\"text: GiaVonMoi\"></span>',
            '{ChenhLech}': '<span data-bind=\"text: ChenhLech\"></span>',
            '{DonViTinh}': '<span data-bind=\"text: DonViTinh\"></span>',
            '{DonGia}': '<span data-bind=\"text: DonGia\"></span>',
            '{SoLuong}': '<span data-bind=\"text: SoLuong\"></span>',
            '{PTChietKhauHH}': '<span data-bind=\"text: PTChietKhau\"></span>',
            '{GiamGia}': '<span data-bind=\"text: TienChietKhau\"></span>',
            '{GiaBan}': '<span data-bind=\"text: GiaBan\"></span>',
            '{ThanhTienTruocCK}': '<span data-bind=\"text: ThanhTienTruocCK\"></span>',
            '{ThanhToan}': '<span data-bind=\"text: ThanhToan\"></span>',
            '{ThanhTien}': '<span data-bind=\"text: ThanhTien\"></span>',
            '{TonKho}': '<span data-bind=\"text: TonKho\"></span>',
            '{KThucTe}': '<span data-bind=\"text: KThucTe\"></span>',
            '{SLLech}': '<span data-bind=\"text: SLLech\"></span>',
            '{GiaTriLech}': '<span data-bind=\"text: GiaTriLech\"></span>',
            '{ThuocTinh_GiaTri}': '<span data-bind=\"text: ThuocTinh_GiaTri\"></span>',
            '{SLDVDaSuDung}': '<span data-bind=\"text: SoLuongDVDaSuDung\"></span>',
            '{SLDVConLai}': '<span data-bind=\"text: SoLuongDVConLai\"></span>',
            '{GhiChu}': '<span data-bind=\"text: GhiChu\"></span>',
            '{TenViTri}': '<span data-bind=\"text: TenViTri\"></span>',
            '{ThoiGianBatDau}': '<span data-bind=\"text: TimeStart\"></span>',
            '{SoPhutThucHien}': '<span data-bind=\"text: ThoiGianThucHien\"></span>',
            '{QuaThoiGian}': '<span data-bind=\"text: QuaThoiGian\"></span>',
            '{GhiChuHH}': '<span data-bind=\"text: GhiChuHH\"></span>',
            '{PTThue}': '<span data-bind=\"text: PTThue\"></span>',
            '{TienThue}': '<span data-bind=\"text: TienThue\"></span>',
            '{HH_ThueTong}': '<span data-bind=\"text: HH_ThueTong\"></span>',
            '{DonGiaBaoHiem}': '<span data-bind=\"text: DonGiaBaoHiem\"></span>',
            '{BH_ThanhTien}': '<span data-bind=\"text: BH_ThanhTien\"></span>',
            '{PTChiPhi}': '<span data-bind=\"text: PTChiPhi\"></span>',
            '{TienChiPhi}': '<span data-bind=\"text: TienChiPhi\"></span>',
            '{TongChietKhau}': '<span data-bind=\"text: TongChietKhau\"></span>',

            '{GhiChu_NVThucHien}': '<span data-bind=\"text: GhiChu_NVThucHien\"></span>',
            '{GhiChu_NVTuVan}': '<span data-bind=\"text: GhiChu_NVTuVan\"></span>',
            '{NVTuVanDV_CoCK}': '<span data-bind=\"text: NVTuVanDV_CoCK\"></span>',
            '{NVThucHienDV_CoCK}': '<span data-bind=\"text: NVThucHienDV_CoCK\"></span>',

            '{SoLuongChuyen}': '<span data-bind=\"text: SoLuongChuyen\"></span>',
            '{SoLuongNhan}': '<span data-bind=\"text: SoLuongNhan\"></span>',
            '{GiaChuyen}': '<span data-bind=\"text: GiaChuyen\"></span>',

            '{SoLuongHuy}': '<span data-bind=\"text: SoLuongHuy\"></span>',
            '{GiaVon}': '<span data-bind=\"text: GiaVon\"></span>',
            '{GiaTriHuy}': '<span data-bind=\"text: GiaTriHuy\"></span>',
        });
    return content;
}

function CheckRowNextNhom(temptable, row2Str, row2, row2From, row1, row1Str) {
    var lastRow = temptable.lastIndexOf("/tr>") + 5;
    var lastStr = temptable.substr(row2From, lastRow - row2From);
    var last = lastStr;

    if (lastStr.indexOf("{TongTienTheoNhom") > -1) {
        row2 = ''.concat(" <!--ko foreach: $data.HangHoas --> ", row2, " <!--/ko-->");
        temptable = temptable.replace(row2Str, row2);
        last = ''.concat(last, " <!--/ko --> ");
    }
    else {
        row1 = ''.concat(row1, " <!--ko foreach: $data.HangHoas --> ");
        // khong co cot tongtien theonhom
        last = ''.concat(last, " <!--/ko-->", " <!--/ko-->");
    }
    temptable = temptable.replace(lastStr, last);
    temptable = temptable.replace(row1Str, row1);
    return temptable;
}

function replaceBetween(origin, startIndex, endIndex, insertion) {
    return origin.substring(0, startIndex) + insertion + origin.substring(endIndex);
}

function SetConvertDataTest(strInput) {
    if (keyid === "IMV") {
        if (strInput !== null && strInput !== undefined && strInput !== "") {
            strInput = strInput.allReplace({
                '{TenCuaHang}': '<span data-bind=\"text: TenCuaHang\"></span>',
                '{TenSanPham}': '<span data-bind=\"text: TenSanPham\"></span>',
                '{MaVach}': '<img data-bind=\"attr:{src: MaVach}\"  />',
                '{MaHangHoa}': '<span data-bind=\"text: MaHangHoa\"></span>',
                '{Gia}': '<span data-bind=\"text: Gia\"></span>',
            });
            return strInput;
        }
        return "";
    }

    if (strInput !== null && strInput !== undefined && strInput !== "") {
        var result = '';
        result = strInput;
        if (strInput.indexOf('{TenHangHoaMoi}') > -1) {
            let open = result.lastIndexOf("tbody", result.indexOf("{TenHangHoa}")) - 1;
            let close = result.indexOf("tbody", result.indexOf("{TenHangHoa}")) + 6;
            let temptable = result.substr(open, close - open);
            let temptable1 = temptable;
            temptable = temptable.allReplace(
                {
                    'tbody': 'tbody data-bind=\"foreach: CTHoaDonPrint\"',
                    '{STT}': '<span data-bind=\"text: SoThuTu\"></span>'
                });
            temptable = ReplaceCTHD(temptable);
            result = result.replace(temptable1, temptable);
            let openTbl2 = result.lastIndexOf("tbody", result.indexOf("{TenHangHoaMoi}")) - 1;
            let closeTbl2 = result.indexOf("tbody", result.indexOf("{TenHangHoaMoi}")) + 6;
            let temptable2 = result.substr(openTbl2, closeTbl2 - openTbl2);
            let temptableMH = temptable2;
            temptable2 = temptable2.allReplace(
                {
                    'tbody': 'tbody data-bind=\"foreach: CTHoaDonPrintMH\"',
                    '{STT}': '<span data-bind=\"text: SoThuTu\"></span>',
                    '{TenHangHoaMoi}': '<span data-bind=\"text: TenHangHoa\"></span>'
                });
            temptable2 = ReplaceCTHD(temptable2);
            result = result.replace(temptableMH, temptable2);
        }
        else {
            if (strInput.indexOf("{Nhom_HangHoaDV}") == -1 && (strInput.indexOf("{TheoHangHoa}") > -1 || strInput.indexOf("{TheoDichVu}") > -1)) {
                var open = result.lastIndexOf("tbody", result.indexOf("{TenHangHoa")) - 1;
                var close = result.indexOf("tbody", result.indexOf("{TenHangHoa")) + 6;
                var temptable = result.substr(open, close - open);
                var temptable1 = temptable;

                var strDV = "<!-- ko foreach:  $root.CTHoaDonPrint().filter(x=> x.LaHangHoa === false) -->";
                var strHH = "<!-- ko foreach:  $root.CTHoaDonPrint().filter(x=> x.LaHangHoa) -->";

                var indexHH = temptable1.indexOf("TheoHangHoa");
                var indexDV = temptable1.indexOf("TheoDichVu");

                var row1From = temptable.indexOf("<tr");
                var row1To = temptable.indexOf("/tr>");
                var row1Str = temptable.substr(row1From, row1To);
                var row1 = row1Str;
                row1Str = row1Str.replace("<tr", " <tr data-bind=\"visible: $index()===0\"");

                if (indexHH < indexDV) {
                    row1Str = ''.concat(strHH, row1Str);
                }
                else {
                    row1Str = ''.concat(strDV, row1Str);
                }

                var row2From = temptable.indexOf("<tr", temptable.indexOf("<tr") + 1);
                var row2To = temptable.indexOf("<tr", row2From + 1);
                var row2Str = temptable.substr(row2From, row2To - row2From);
                var row2 = row2Str;
                row2Str = ''.concat(row2Str, "<!--/ko--> ");
                // use other
                if (temptable.indexOf("{TongTienPhuTung") == -1) {
                    // find tr3
                    var row3From = row2To;
                    var row3To = temptable.indexOf("<tr", row3From + 1);
                    var row3Str = temptable.substr(row3From, row3To - row3From);
                    var row3 = row3Str;
                    row3Str = row3Str.replace("<tr",
                        " <tr data-bind=\"visible: $index()===0\"");

                    if (indexHH < indexDV) {
                        row3Str = ''.concat(strDV, row3Str);
                    }
                    else {
                        row3Str = ''.concat(strHH, row3Str);
                    }

                    var row4From = row3To;
                    var row4To = temptable.indexOf("<tr", row4From + 1);
                    if (row4To == -1) {
                        row4To = temptable.lastIndexOf("tr>") + 3;
                    }
                    var row4Str = temptable.substr(row4From, row4To - row4From);
                    var row4 = row4Str;
                    row4Str = ''.concat(row4Str, "<!--/ko--> ");
                    temptable = replaceBetween(temptable, row4From, row4To, row4Str);

                    temptable = temptable.replace(row1, row1Str);
                    temptable = temptable.replace(row2, row2Str);
                    temptable = temptable.replace(row3, row3Str);
                }
                else {
                    // row3: tongtienphutung
                    var row3From = row2To;
                    var row3To = temptable.indexOf("<tr", row3From + 1);
                    var row3Str = temptable.substr(row3From, row3To - row3From);

                    var row4From = row3To;
                    var row4To = temptable.indexOf("<tr", row4From + 1);
                    var row4Str = temptable.substr(row4From, row4To - row4From);
                    var row4 = row4Str;
                    row4Str = row4Str.replace("<tr", " <tr data-bind=\"visible: $index()===0\"");

                    var row5From = row4To;
                    var row5To = temptable.indexOf("<tr", row5From + 1);
                    var row5Str = temptable.substr(row5From, row5To - row5From);
                    var row5 = row5Str;
                    row5Str = ''.concat(row5Str, "<!--/ko--> ");

                    if (indexHH < indexDV) {
                        row4Str = ''.concat(strDV, row4Str);
                    }
                    else {
                        row4Str = ''.concat(strHH, row4Str);
                    }
                    temptable = replaceBetween(temptable, row5From, row5To, row5Str);
                    temptable = temptable.replace(row2, row2Str);
                    temptable = temptable.replace(row1, row1Str);
                    temptable = temptable.replace(row4, row4Str);
                }

                temptable = temptable.allReplace(
                    {
                        '{STT}': '<span data-bind=\"text: $index() + 1\"></span>'
                    });
                temptable = ReplaceCTHD(temptable);
                result = result.replace(temptable1, temptable);

                result = result.replace("{TheoDichVu}", "");
                result = result.replace("{TheoHangHoa}", "");
            }
            else {
                if (strInput.indexOf('{TheoNhomHang}') > -1) {
                    var open = result.lastIndexOf("tbody", result.indexOf("{TenNhomHangHoa}")) - 1;
                    var close = result.indexOf("tbody", result.indexOf("{TenHangHoa")) + 6;
                    var temptable = result.substr(open, close - open);
                    var temptable1 = temptable;

                    var row1From = temptable.indexOf("<tr");
                    var row1To = temptable.indexOf("/tr>") - 3;
                    var row1Str = temptable.substr(row1From, row1To);
                    var row1 = row1Str;
                    row1 = ''.concat(" <!--ko foreach: CTHoaDonPrint_TheoNhom --> ", row1);

                    var row2From = temptable.indexOf("<tr", temptable.indexOf("<tr") + 1);
                    var row2To = temptable.indexOf("<tr", row2From + 1);
                    var row2Str = '';
                    var row2 = '';

                    if (row2To < 0) {
                        temptable = CheckRowNextNhom(temptable, row2Str, row2, row2From, row1, row1Str);
                    }
                    else {
                        row2Str = temptable.substr(row2From, row2To - row2From);
                        row2 = row2Str;

                        var row3From = row2To;
                        var row3To = temptable.indexOf("<tr", row3From + 1);
                        if (row3To < 0) {
                            row2From = row3From;
                            temptable = CheckRowNextNhom(temptable, row2Str, row2, row2From, row1, row1Str);
                        }
                        else {
                            row2 = ''.concat(" <!--ko foreach: $data.HangHoas --> ", row2, " <!--/ko-->");

                            var row3Str = temptable.substr(row3From, row3To - row3From);
                            var row3 = row3Str;
                            if (temptable.indexOf("{TongTienTheoNhom") > -1) {
                                row3 = ''.concat(row3, " <!--/ko --> ");
                            }
                            else {
                                row3 = ''.concat(row3Str, " <!--/ko --> ");
                            }
                            temptable = temptable.replace(row3Str, row3);
                            temptable = temptable.replace(row2Str, row2);
                            temptable = temptable.replace(row1Str, row1);
                        }
                    }

                    temptable = temptable.allReplace(
                        {
                            '{STT}': '<span data-bind=\"text: SoThuTu\"></span>'
                        });
                    temptable = ReplaceCTHD(temptable);
                    result = result.replace(temptable1, temptable);

                    result = result.replace('{TheoNhomHang}', '');
                    result = result.replace("{TenNhomHangHoa}",
                        " <span data-bind=\"text: TenNhomHangHoa\"> </span>");
                    result = result.replace('{SoThuTuNhom}', ' <span data-bind="text: SoThuTuNhom"> </span>');
                    result = result.replace('{SoThuTuNhom_LaMa}', ' <span data-bind="text: SoThuTuNhom_LaMa"> </span>');

                    result = result.replace('{TongTienTheoNhom}',
                        '<span data-bind="text: TongTienTheoNhom" > </span>');
                    result = result.replace('{TongTienTheoNhom_TruocVAT}',
                        '<span data-bind="text: TongTienTheoNhom_TruocVAT" > </span>');
                    result = result.replace("{TongTienTheoNhom_TruocCK}",
                        "<span data-bind=\"text: TongTienTheoNhom_TruocCK\" > </span>");

                    result = result.replace("{TongSLTheoNhom}",
                        "<span data-bind=\"text: TongSLTheoNhom\" > </span>");
                    result = result.replace("{TongThueTheoNhom}",
                        "<span data-bind=\"text: TongThueTheoNhom\" > </span>");
                    result = result.replace("{TongCKTheoNhom}",
                        "<span data-bind=\"text: TongCKTheoNhom\" > </span>");
                }
                else {
                    if (strInput.indexOf('{Combo}') > -1) {
                        let open = result.lastIndexOf("tbody", result.indexOf("{Combo}")) - 1;
                        let close = result.indexOf("tbody", result.indexOf("{Combo}")) + 6;
                        let temptable = result.substr(open, close - open);
                        let temptable1 = temptable;

                        let row1From = temptable.indexOf("<tr");
                        let row1To = temptable.indexOf("/tr>") - 3;
                        let row1Str = temptable.substr(row1From, row1To);

                        let row2From = temptable.indexOf("<tr", row1From + 1);
                        let row2To = temptable.indexOf("<tr", row2From + 1);
                        let row2Str = temptable.substr(row2From, row2To - row2From);
                        let row2 = ''.concat(" <!--ko foreach: CTHoaDonPrint --> ", row2Str);

                        let nextRowFrom = row2To;
                        let nextRowTo;
                        let nextRowStr;

                        function CheckRow3() {
                            nextRowTo = temptable.indexOf("<tr", nextRowFrom + 1);
                            nextRowStr = temptable.substr(nextRowFrom, nextRowTo - nextRowFrom);

                            if (nextRowTo > 0) {
                                CheckRowTP();
                            }
                            else {
                                ReplaceDetail();
                            }
                        }
                        CheckRow3();
                        function CheckRowTP() {
                            if (nextRowStr.indexOf("{ThanhPhan}") > -1) {
                                let row4From = nextRowTo;
                                let row4To = temptable.indexOf("<tr", row4From + 1);
                                if (row4To < 0) {
                                    row4To = temptable.lastIndexOf("/tr>") + 5;
                                }
                                else {
                                    if (row4To < 0) {
                                        row4To = temptable.lastIndexOf("/tr>") + 5;
                                    }
                                    else {
                                        var rNextTo = temptable.indexOf("<tr", row4To + 1);
                                        if (rNextTo < 0) {
                                            rNextTo = temptable.lastIndexOf("/tr>") + 5;
                                        }
                                        var strTemp = temptable.substr(row4From, rNextTo - row4From);
                                        if (strTemp.indexOf("{GhiChu_NVThucHien}") > -1 || strTemp.indexOf("{GhiChu_NVTuVan}") > -1) {
                                            row4To = rNextTo;
                                        }
                                    }
                                }
                                let row4Str = temptable.substr(row4From, row4To - row4From);
                                let row4 = ''.concat(" <!--ko foreach: $data.ThanhPhanComBo --> ", row4Str,
                                    "<!--/ko--> <!--/ko-->");

                                temptable = temptable.replace(row4Str, row4);
                                temptable = temptable.replace(nextRowStr, ""); // delete row contains {ThanhPhan}
                                ReplaceDetail();
                            }
                            else {
                                nextRowFrom = nextRowTo;
                                CheckRow3();
                            }
                        }

                        function ReplaceDetail() {
                            temptable = temptable.replace(row1Str, "");
                            temptable = temptable.replace(row2Str, row2);

                            temptable = temptable.replace(/{STT}/g, "<span data-bind=\"text: SoThuTu\"></span>");
                            temptable = ReplaceCTHD(temptable);
                            result = result.replace(temptable1, temptable);
                        }
                    }
                    else {
                        if (strInput.indexOf("{TheoHangHoa_Nhom}") > -1 || strInput.indexOf("{TheoDichVu_Nhom}") > -1) {
                            // tblhanghoa
                            let startHH = result.indexOf("{TheoHangHoa_Nhom}");
                            let opentblHH = result.indexOf("tbody", startHH) - 1;
                            let closeblHH = result.indexOf("tbody", opentblHH + 6);
                            let sTblHH = result.substr(opentblHH, closeblHH - opentblHH + 6);
                            let sTblHH_goc = sTblHH;

                            let hh_headerFrom = result.indexOf("thead", startHH) - 1;
                            let hh_headerTo = result.indexOf("thead", hh_headerFrom + 5);
                            let hh_sHeader = result.substr(hh_headerFrom, hh_headerTo - hh_headerFrom + 6);
                            let hh_sHeaderGoc = hh_sHeader;

                            let hh_row1From = sTblHH.indexOf("<tr");
                            let hh_row1To = sTblHH.indexOf("/tr>", hh_row1From + 3) + 4;

                            let hh_row2From = sTblHH.indexOf("<tr", hh_row1To);
                            let hh_row2To = sTblHH.indexOf("/tr>", hh_row2From) + 6;
                            let hh_row2Str = sTblHH.substr(hh_row1To, hh_row2To - hh_row2From);
                            let hh_row2 = hh_row2Str;

                            // find row contain sum phutung
                            if (sTblHH.indexOf("{TongSL_PhuTung}") > -1 || sTblHH.indexOf("{TongThue_PhuTung}") > -1 || sTblHH.indexOf("{TongCK_PhuTung}") > -1 || sTblHH.indexOf("{TongTienPhuTung_TruocCK}") > -1 || sTblHH.indexOf("{TongTienPhuTung_TruocVAT}") > -1 || sTblHH.indexOf("{TongTienPhuTung}") > -1) {
                                let hh_lastRowFrom = sTblHH.lastIndexOf("<tr", sTblHH.indexOf("</tbody"));
                                let hh_lastRowTo = sTblHH.lastIndexOf("</tr", sTblHH.indexOf("</tbody")) + 5;
                                let hh_lastRowStr = sTblHH.substr(hh_lastRowFrom, hh_lastRowTo - hh_lastRowFrom);
                                let hh_lastRow = hh_lastRowStr;
                                hh_lastRowStr = ''.concat(" <!-- ko if: $root.CTHoaDonPrint_VatTu().length - 1 == $index() --> ", hh_lastRowStr, " <!--/ko-->");
                                sTblHH = sTblHH.replace(hh_lastRow, hh_lastRowStr);
                            }

                            hh_row2Str = ''.concat(" <!--ko foreach: $data.HangHoas --> ", hh_row2, " <!--/ko-->");
                            sTblHH = ''.concat("<!--ko foreach: $root.CTHoaDonPrint_VatTu --> ", sTblHH, " <!--/ko-->");
                            hh_sHeader = ''.concat(" <!-- ko if: $root.CTHoaDonPrint_VatTu().length > 0 --> ", hh_sHeader, " <!--/ko-->");

                            sTblHH = sTblHH.replace(hh_row2, hh_row2Str);
                            sTblHH = Replace_TheoNhom(sTblHH);
                            sTblHH = ReplaceCTHD(sTblHH);
                            sTblHH = sTblHH.replace("{STT}", "<span data-bind=\"text: SoThuTu\"></span>");


                            // tblDichVu
                            let startDV = result.indexOf("{TheoDichVu_Nhom}");
                            let opentblDV = result.indexOf("tbody", startDV) - 1;
                            let closeblDV = result.indexOf("tbody", opentblDV + 6);
                            let sTblDV = result.substr(opentblDV, closeblDV - opentblDV + 6);
                            let sTblDV_goc = sTblDV;

                            let dv_headerFrom = result.indexOf("thead", startDV) - 1;
                            let dv_headerTo = result.indexOf("thead", dv_headerFrom + 5);
                            let dv_sHeader = result.substr(dv_headerFrom, dv_headerTo - dv_headerFrom + 6);
                            let dv_sHeaderGoc = dv_sHeader;

                            let dv_row1From = sTblDV.indexOf("<tr");
                            let dv_row1To = sTblDV.indexOf("/tr>", hh_row1From + 3) + 4;

                            let dv_row2From = sTblDV.indexOf("<tr", dv_row1To);
                            let dv_row2To = sTblDV.indexOf("/tr>", dv_row2From) + 5;
                            let dv_row2Str = sTblDV.substr(dv_row2From, dv_row2To - dv_row2From);
                            let dv_row2 = dv_row2Str;

                            // find row contain sum dichvu
                            if (sTblDV.indexOf("{TongSL_DichVu}") > -1 || sTblDV.indexOf("{TongThue_DichVu}") > -1 || sTblDV.indexOf("{TongCK_DichVu}") > -1 || sTblDV.indexOf("{TongTienDichVu_TruocCK}") > -1 || sTblDV.indexOf("{TongTienDichVu_TruocVAT}") > -1 || sTblDV.indexOf("{TongTienDichVu}") > -1) {
                                let dv_lastRowFrom = sTblDV.lastIndexOf("<tr", sTblDV.indexOf("</tbody"));
                                let dv_lastRowTo = sTblDV.lastIndexOf("</tr>", sTblDV.indexOf("</tbody")) + 5;
                                let dv_lastRowStr = sTblDV.substr(dv_lastRowFrom, dv_lastRowTo - dv_lastRowFrom);
                                let dv_lastRow = dv_lastRowStr;
                                dv_lastRowStr = ''.concat(" <!-- ko if: $root.CTHoaDonPrint_DichVu().length - 1 == $index() --> ", dv_lastRowStr, " <!--/ko-->");
                                sTblDV = sTblDV.replace(dv_lastRow, dv_lastRowStr);
                            }

                            dv_row2Str = ''.concat(" <!--ko foreach: $data.HangHoas --> ", dv_row2, " <!--/ko-->");
                            sTblDV = ''.concat("<!--ko foreach: $root.CTHoaDonPrint_DichVu --> ", sTblDV, " <!--/ko-->");
                            dv_sHeader = ''.concat(" <!-- ko if: $root.CTHoaDonPrint_DichVu().length > 0 --> ", dv_sHeader, " <!--/ko-->");

                            sTblDV = sTblDV.replace(dv_row2, dv_row2Str);
                            sTblDV = Replace_TheoNhom(sTblDV);
                            sTblDV = ReplaceCTHD(sTblDV);
                            sTblDV = sTblDV.replace("{STT}", "<span data-bind=\"text: SoThuTu\"></span>");


                            result = result.replace("{TheoHangHoa_Nhom}", "");
                            result = result.replace("{TheoDichVu_Nhom}", "");
                            result = result.replace(sTblHH_goc, sTblHH);
                            result = result.replace(sTblDV_goc, sTblDV);
                            result = result.replace(hh_sHeaderGoc, hh_sHeader);
                            result = result.replace(dv_sHeaderGoc, dv_sHeader);
                        }
                        else {
                            if (strInput.indexOf("{Nhom_HangHoaDV}") > -1) {
                                let startHH = strInput.indexOf("{Nhom_HangHoaDV}");
                                let openTbl = strInput.indexOf("tbody", startHH) - 1;
                                let closeTbl = strInput.indexOf("tbody", openTbl + 6);
                                let tbl = strInput.substr(openTbl, closeTbl - openTbl + 6);
                                let tbl_goc = tbl;

                                if (tbl_goc.indexOf("{TenNhomHangHoa}") > -1) {
                                    let row1From = tbl_goc.indexOf("<tr", tbl_goc.indexOf("tbody"));
                                    let row1To = tbl_goc.indexOf("/tr>", row1From + 4) + 4;
                                    let row1 = tbl_goc.substr(row1From, row1To - row1From);
                                    let row1_goc = row1;

                                    row1 = ''.concat("<!--ko foreach: {data: $root.CTHoaDon_TheoNhom_VaHHDV, as :'itNhom'} -->", row1);

                                    let row2From = tbl_goc.indexOf("<tr", row1To);
                                    let row2To = tbl_goc.indexOf("/tr>", row2From + 2) + 4;
                                    let row2 = tbl_goc.substr(row2From, row2To - row2From);
                                    let row2_goc = row2;

                                    // find row 2: hanghoa or dichvu
                                    let strDV = "<!--ko foreach: {data: itNhom.ListDichVus.DichVus, as: 'itDV'} -->";
                                    let strHH = "<!--ko foreach: {data: itNhom.ListHangHoas.HangHoas, as: 'itHH'} -->";

                                    if (row2.indexOf("{TheoHangHoa}") > -1) {
                                        row2 = row2.replace("<tr", " <tr data-bind=\"visible: $index()===0\"");
                                        row2 = ''.concat(strHH, row2);

                                        // row3: thongtin hang
                                        let row3From = tbl_goc.indexOf("<tr", row2To);
                                        let row3To = tbl_goc.indexOf("/tr>", row3From + 3) + 4;
                                        let row3 = tbl_goc.substr(row3From, row3To - row3From);
                                        let row3_goc = row3;

                                        let row4From = row3To;
                                        let row4To = tbl_goc.indexOf("/tr>", row4From + 2) + 4;
                                        let row4 = tbl_goc.substr(row4From, row4To - row4From);
                                        let row4_goc = row4;

                                        if (row4.indexOf("{NhomHH_ThanhToan") > -1
                                            || row4.indexOf("{NhomHH_TruocCK") > -1
                                            || row4.indexOf("{NhomHH_TruocVAT") > -1) {
                                            // hanghoa - cotong
                                            row4 = row4.replace("<tr", " <tr data-bind=\"visible: itNhom.ListHangHoas.HangHoas.length === $index() + 1\"");
                                            row4 = ''.concat(row4, "<!--/ko -->");

                                            let row5From = row4To;
                                            let row5To = tbl_goc.indexOf("/tr>", row5From + 2) + 5;
                                            let row5 = tbl_goc.substr(row5From, row5To - row5From);
                                            let row5_goc = row5;

                                            if (row5.indexOf("{TheoDichVu}") > -1) {
                                                row5 = row5.replace("<tr", " <tr data-bind=\"visible: $index()===0\"");
                                                row5 = ''.concat(strDV, row5);

                                                let row6From = row5To;
                                                let row6To = tbl_goc.indexOf("/tr>", row6From + 2) + 5;
                                                let row6 = tbl_goc.substr(row6From, row6To - row6From);
                                                let row6_goc = row6;

                                                let row7From = row6To;
                                                let row7To = tbl_goc.indexOf("/tr>", row7From + 2) + 5;
                                                let row7 = tbl_goc.substr(row7From, row7To - row7From);
                                                let row7_goc = row7;

                                                if (row7.indexOf("{NhomDV_ThanhToan") > -1
                                                    || row7.indexOf("{NhomDV_TruocCK") > -1
                                                    || row7.indexOf("{NhomDV_TruocVAT") > -1) {
                                                    row7 = row7.replace("<tr", " <tr data-bind=\"visible: itNhom.ListDichVus.DichVus.length === $index() + 1\"");

                                                    // tong hh + dv theo nhom
                                                    let row8From = row7To;
                                                    let row8To = tbl_goc.indexOf("/tr>", row8From + 2) + 5;
                                                    if (row8To > 0) {
                                                        let row8 = tbl_goc.substr(row8From, row8To - row8From);
                                                        let row8_goc = row8;
                                                        if (row8.indexOf("{TongTienTheoNhom") > -1) {
                                                            row7 = ''.concat(row7, " <!--/ko -->");
                                                            row8 = ''.concat(row8, " <!--/ko -->");
                                                            tbl = tbl.replace(row8_goc, row8);
                                                        }
                                                        else {
                                                            row7 = ''.concat(row7, " <!--/ko --> <!--/ko -->");
                                                        }
                                                    }
                                                    else {
                                                        row7 = ''.concat(row7, " <!--/ko --> <!--/ko -->");
                                                    }
                                                    tbl = tbl.replace(row7_goc, row7);
                                                }
                                                else {
                                                    row6 = ''.concat(row6, " <!--/ko --> <!--/ko --> ");
                                                    tbl = tbl.replace(row6_goc, row6);
                                                }
                                                tbl = tbl.replace(row5_goc, row5);
                                                tbl = tbl.replace(row4_goc, row4);
                                            }
                                            else {
                                                // err
                                            }

                                            tbl = tbl.replace(row2_goc, row2);

                                        }
                                    }
                                    else {
                                        if (row2.indexOf("{TheoDichVu}") > -1) {
                                            row2 = row2.replace("<tr", " <tr data-bind=\"visible: $index()===0\"");
                                            row2 = ''.concat(strDV, row2);

                                            // row3: thongtin hang
                                            let row3From = tbl_goc.indexOf("<tr", row2To);
                                            let row3To = tbl_goc.indexOf("/tr>", row3From + 3) + 4;
                                            let row3 = tbl_goc.substr(row3From, row3To - row3From);
                                            let row3_goc = row3;

                                            let row4From = row3To;
                                            let row4To = tbl_goc.indexOf("/tr>", row4From + 2) + 4;
                                            let row4 = tbl_goc.substr(row4From, row4To - row4From);
                                            let row4_goc = row4;

                                            if (row4.indexOf("{NhomDV_ThanhToan") > -1
                                                || row4.indexOf("{NhomDV_TruocCK") > -1
                                                || row4.indexOf("{NhomDV_TruocVAT") > -1) {
                                                // hanghoa - cotong
                                                row4 = row4.replace("<tr", " <tr data-bind=\"visible: itNhom.ListDichVus.DichVus.length === $index() + 1\"");
                                                row4 = ''.concat(row4, "<!--/ko -->");

                                                let row5From = row4To;
                                                let row5To = tbl_goc.indexOf("/tr>", row5From + 2) + 5;
                                                let row5 = tbl_goc.substr(row5From, row5To - row5From);
                                                let row5_goc = row5;

                                                if (row5.indexOf("{TheoHangHoa}") > -1) {
                                                    row5 = row5.replace("<tr", " <tr data-bind=\"visible: $index()===0\"");
                                                    row5 = ''.concat(strHH, row5);

                                                    let row6From = row5To;
                                                    let row6To = tbl_goc.indexOf("/tr>", row6From + 2) + 5;
                                                    let row6 = tbl_goc.substr(row6From, row6To - row6From);
                                                    let row6_goc = row6;

                                                    let row7From = row6To;
                                                    let row7To = tbl_goc.indexOf("/tr>", row7From + 2) + 5;
                                                    let row7 = tbl_goc.substr(row7From, row7To - row7From);
                                                    let row7_goc = row7;

                                                    if (row7.indexOf("{NhomHH_ThanhToan") > -1
                                                        || row7.indexOf("{NhomHH_TruocCK") > -1
                                                        || row7.indexOf("{NhomHH_TruocVAT") > -1) {
                                                        row7 = row7.replace("<tr", " <tr data-bind=\"visible: itNhom.ListHangHoas.HangHoas.length === $index() + 1\"");

                                                        // tong hh + dv theo nhom
                                                        let row8From = row7To;
                                                        let row8To = tbl_goc.indexOf("/tr>", row8From + 2) + 5;
                                                        if (row8To > 0) {
                                                            let row8 = tbl_goc.substr(row8From, row8To - row8From);
                                                            let row8_goc = row8;
                                                            if (row8.indexOf("{TongTienTheoNhom") > -1) {
                                                                row7 = ''.concat(row7, " <!--/ko -->");
                                                                row8 = ''.concat(row8, " <!--/ko -->");
                                                                tbl = tbl.replace(row8_goc, row8);
                                                            }
                                                            else {
                                                                row7 = ''.concat(row7, " <!--/ko --> <!--/ko -->");
                                                            }
                                                        }
                                                        else {
                                                            row7 = ''.concat(row7, " <!--/ko --> <!--/ko -->");
                                                        }
                                                        tbl = tbl.replace(row7_goc, row7);
                                                    }
                                                    else {
                                                        row6 = ''.concat(row6, " <!--/ko --> <!--/ko --> ");
                                                        tbl = tbl.replace(row6_goc, row6);
                                                    }
                                                    tbl = tbl.replace(row5_goc, row5);
                                                    tbl = tbl.replace(row4_goc, row4);
                                                }
                                                else {
                                                    // err
                                                }

                                                tbl = tbl.replace(row2_goc, row2);

                                            }
                                        }
                                        else {
                                            // err
                                        }
                                    }

                                    tbl = tbl.replace(/{STT}/g, "<span data-bind=\"text: SoThuTu\"></span>");
                                    tbl = tbl.replace(row1_goc, row1);
                                    result = result.replace(tbl_goc, tbl);
                                    result = ReplaceCTHD(result);
                                }
                                else {
                                    // err
                                }

                                result = result.replace("{Nhom_HangHoaDV}", "");
                                result = result.replace("{TheoHangHoa}", "");
                                result = result.replace("{TheoDichVu}", "");
                                result = result.replace("{NhomDV_TruocCK}", "<span data-bind=\"text: itNhom.ListDichVus.NhomDV_TruocCK\"></span>");
                                result = result.replace("{NhomDV_TruocVAT}", "<span data-bind=\"text: itNhom.ListDichVus.NhomDV_TruocVAT\"></span>");
                                result = result.replace("{NhomDV_ThanhToan}", "<span data-bind=\"text: itNhom.ListDichVus.NhomDV_ThanhToan\"></span>");
                                result = result.replace("{NhomHH_TruocCK}", "<span data-bind=\"text: itNhom.ListHangHoas.NhomHH_TruocCK\"></span>");
                                result = result.replace("{NhomHH_TruocVAT}", "<span data-bind=\"text: itNhom.ListHangHoas.NhomHH_TruocVAT\"></span>");
                                result = result.replace("{NhomHH_ThanhToan}", "<span data-bind=\"text: itNhom.ListHangHoas.NhomHH_ThanhToan\"></span>");
                                result = Replace_TheoNhom(result);
                            }
                            else {
                                if (strInput.indexOf('{TenHangHoa') > -1) {
                                    let open = result.lastIndexOf("tbody", result.indexOf("{TenHangHoa")) - 1;
                                    let close = result.indexOf("tbody", result.indexOf("{TenHangHoa")) + 6;
                                    let temptable = result.substr(open, close - open);
                                    let temptable1 = temptable;

                                    let row1From = temptable.indexOf("<tr");
                                    let row1To = temptable.indexOf("/tr>") - 4;
                                    let row1Str = temptable.substr(row1From, row1To);
                                    let row1 = row1Str;

                                    let nextRowFrom = row1To;

                                    row1Str = ''.concat(" <!--ko foreach: $data.CTHoaDonPrint --> ", row1Str);
                                    if (row1Str.indexOf("{SoLuong") > -1 || row1Str.indexOf("{ThanhTien") > -1
                                        || row1Str.indexOf("{GiaVon") > - 1) {
                                        let lastRowFrom = row1To;
                                        let lastRowTo = temptable.indexOf("<tr", lastRowFrom + 1);
                                        if (lastRowTo < 0) {
                                            lastRowTo = temptable.lastIndexOf("/tr>") + 5;
                                        }
                                        let lastRowStr = temptable.substr(lastRowFrom, lastRowTo - lastRowFrom);
                                        let lastRow = lastRowStr;
                                        if (lastRowStr.indexOf("{GhiChu_NVThucHien}") > -1 || lastRowStr.indexOf("{GhiChu_NVTuVan}") > -1
                                            || lastRowStr.indexOf("{GiaVonMoi}") > -1) {
                                            lastRowStr = ''.concat(lastRowStr, "<!--/ko-->");
                                            temptable = temptable.replace(lastRow, lastRowStr);
                                        }
                                        else {
                                            row1Str = ''.concat(row1Str, "<!--/ko-->");
                                        }
                                        ReplaceDetail();
                                    }
                                    else {
                                        CheckRowNext();
                                    }
                                    function CheckRowNext() {
                                        let nextRowTo = temptable.indexOf("<tr", nextRowFrom + 1);
                                        if (nextRowTo < 0) {
                                            nextRowTo = temptable.lastIndexOf("/tr>") + 5;
                                        }
                                        let nextRowStr = temptable.substr(nextRowFrom, nextRowTo - nextRowFrom);
                                        let nextRow = nextRowStr;

                                        if (nextRowStr.indexOf("{SoLuong") > -1 || nextRowStr.indexOf("{ThanhTien") > -1
                                            || row1Str.indexOf("{GiaVon") > -1) {
                                            let lastRowFrom = nextRowTo;
                                            let lastRowTo = temptable.indexOf("<tr", lastRowFrom + 1);
                                            if (lastRowTo < 0) {
                                                lastRowTo = temptable.lastIndexOf("/tr>") + 5;
                                            }
                                            let lastRowStr = temptable.substr(lastRowFrom, lastRowTo - lastRowFrom);
                                            let lastRow = lastRowStr;
                                            if (lastRowStr.indexOf("{GhiChu_NVThucHien}") > -1 || lastRowStr.indexOf("{GhiChu_NVTuVan}") > -1) {
                                                lastRowStr = ''.concat(lastRowStr, "<!--/ko-->");
                                                temptable = temptable.replace(lastRow, lastRowStr);
                                            }
                                            else {
                                                nextRowStr = ''.concat(nextRowStr, "<!--/ko-->");
                                                temptable = temptable.replace(nextRow, nextRowStr);
                                            }
                                            ReplaceDetail();
                                        }
                                        else {
                                            nextRowFrom = nextRowTo;
                                            CheckRowNext();
                                        }
                                    }

                                    function ReplaceDetail() {
                                        temptable = temptable.replace(row1, row1Str);
                                        temptable = temptable.replace("{STT}", "<span data-bind=\"text: SoThuTu\"></span>");
                                        temptable = ReplaceCTHD(temptable);
                                        result = result.replace(temptable1, temptable);
                                    }
                                }

                            }
                        }
                    }
                }
            }
        }

        result = Replace_HangMucSC(result);
        result = Replace_VatDungKemTheo(result);

        result = result.allReplace(
            {
                '{Logo}': '<img data-bind=\"attr: {src: LogoCuaHang}\" style=\"width:100%;\" />',
                '{TenCuaHang}': '<span data-bind=\"text: TenCuaHang\"></span>',
                '{DienThoaiCuaHang}': '<span data-bind=\"text: DienThoaiCuaHang\"></span>',
                '{TenChiNhanh}': '<span data-bind=\"text: TenChiNhanh\"></span>',
                '{DienThoaiChiNhanh}': '<span data-bind=\"text: DienThoaiChiNhanh\"></span>',
                '{DiaChiChiNhanh}': '<span data-bind=\"text: DiaChiChiNhanh\"></span>',

                '{TienTraKhach}': "<span data-bind=\"text: $root.PhaiTraKhach\"></span>",
                '{TongTienHoaDonMua}': "<span data-bind=\"text: $root.TongTienHoaDonMua\"></span>",
                '{MaKhachHang}': "<span data-bind=\"text: MaDoiTuong\"></span>",
                '{TongDiemKhachHang}': "<span data-bind=\"text: TongTichDiem\"></span>",
                '{KhachCanTra}': "<span data-bind=\"text: $root.PhaiThanhToan\"></span>",
                '{ThoiGianGiao}': "<span data-bind=\"text: ThoiGianGiao\"></span>",
                '{NgayBan}': '<span data-bind=\"text: NgayLapHoaDon\"></span>',
                '{NgayLapHoaDon}': '<span data-bind=\"text: NgayLapHoaDon\"></span>',
                '{HanSuDungGoiDV}': '<span data-bind=\"text: HanSuDungGoiDV\"></span>',
                '{NgayApDungGoiDV}': '<span data-bind=\"text: NgayApDungGoiDV\"></span>',
                '{MaHoaDon}': '<span data-bind=\"text: MaHoaDon\"></span>',
                '{MaHoaDonTraHang}': '<span data-bind=\"text: MaHoaDonTraHang\"></span>',
                '{TenKhachHang}': '<span data-bind=\"text: TenDoiTuong\"></span>',
                '{TenNhaCungCap}': '<span data-bind=\"text: TenDoiTuong\"></span>',
                '{DiaChi}': '<span data-bind=\"text: DiaChiKhachHang\"></span>',
                '{DienThoai}': '<span data-bind=\"text: DienThoaiKhachHang\"></span>',
                '{NhanVienBanHang}': '<span data-bind=\"text: NhanVienBanHang\"></span>',
                '{NguoiTao}': '<span data-bind=\"text: NguoiTaoHD\"></span>',
                '{TenPhongBan}': "<span data-bind=\"text: TenPhongBan\"></span>",
                '{NgaySinhKH}': '<span data-bind=\"text: NgaySinh_NgayTLap\"></span>',
                '{TenNhomKhach}': '<span data-bind=\"text: TenNhomKhach\"></span>',
                '{MaSoThue}': '<span data-bind=\"text: MaSoThue\"></span>',
                '{TaiKhoanNganHang}': '<span data-bind=\"text: TaiKhoanNganHang\"></span>',

                '{TongTienHang}': '<span data-bind=\"text: $root.TongTienHang\"></span>',
                '{TongTienHDSauGiamGia}': '<span data-bind=\"text: $root.TongTienHDSauGiamGia\"></span>',
                '{TongTienHDSauVAT}': '<span data-bind=\"text: $root.TongTienHDSauVAT\"></span>',
                '{DaThanhToan}': '<span data-bind=\"text: $root.DaThanhToan\"></span>',
                '{TTBangTienCoc}': '<span data-bind=\"text: $root.TTBangTienCoc\"></span>',
                '{SoDuDatCoc}': '<span data-bind=\"text: $root.SoDuDatCoc\"></span>',
                '{ChietKhauHoaDon}': '<span data-bind=\"text: $root.TongGiamGia\"></span>',
                '{DiaChiCuaHang}': '<span data-bind=\"text: DiaChiCuaHang\"></span>',
                '{PhiTraHang}': '<span data-bind=\"text: TongChiPhi\"></span>',

                '{TongTienTraHang}': '<span data-bind=\"text: TongTienTraHang\"></span>',
                '{TongTienTra}': '<span data-bind=\"text: $root.TongTienTra\"></span>',
                '{TongCong}': '<span data-bind=\"text: $root.TongCong\"></span>',
                '{TongSoLuongHang}': '<span data-bind=\"text: $root.TongSoLuongHang\"></span>',
                '{ChiPhiNhap}': '<span data-bind=\"text: ChiPhiNhap\"></span>',
                '{DienGiai}': '<span data-bind=\"text: $root.DienGiai\"></span>',
                '{NoTruoc}': '<span data-bind=\"text: NoTruoc\"></span>',
                '{NoSau}': '<span data-bind=\"text: $root.NoSau\"></span>',
                '{TienKhachThieu}': '<span data-bind=\"text: $root.TienKhachThieu\"></span>',
                '{BH_NoTruoc}': '<span data-bind=\"text: $root.BH_NoTruoc\"></span>',
                '{BH_NoSau}': '<span data-bind=\"text: $root.BH_NoSau\"></span>',

                '{ChiNhanhChuyen}': '<span data-bind=\"text: ChiNhanhChuyen\"></span>',
                '{NguoiChuyen}': '<span data-bind=\"text: NguoiChuyen\"></span>',
                '{ChiNhanhNhan}': '<span data-bind=\"text: ChiNhanhNhan\"></span>',
                '{NguoiNhan}': '<span data-bind=\"text: NguoiNhan\"></span>',
                '{MaChuyenHang}': '<span data-bind=\"text: MaHoaDon\"></span>',
                '{GhiChuChiNhanhChuyen}': '<span data-bind=\"text: GhiChuChiNhanhChuyen\"></span>',
                '{TongSoLuongChuyen}': '<span data-bind=\"text: TongSoLuongChuyen\"></span>',
                '{TongSoLuongNhan}': '<span data-bind=\"text: TongSoLuongNhan\"></span>',
                '{TongTienChuyen}': '<span data-bind=\"text: TongTienChuyen\"></span>',
                '{TongTienNhan}': '<span data-bind=\"text: TongTienNhan\"></span>',


                '{MaPhieu}': '<span data-bind=\"text: MaPhieu\"></span>',
                '{NguoiNopTien}': '<span data-bind=\"text: NguoiNopTien\"></span>',
                '{NguoiNhanTien}': '<span data-bind=\"text: NguoiNopTien\"></span>',
                '{GiaTriPhieu}': '<span data-bind=\"text: GiaTriPhieu\"></span>',
                '{NguoiNhan}': '<span data-bind=\"text: NguoiNhan\"></span>',
                '{MaChuyenHang}': '<span data-bind=\"text: MaHoaDon\"></span>',
                '{NoiDungThu}': '<span data-bind=\"text: NoiDungThu\"></span>',
                '{TienBangChu}': '<span data-bind=\"text: TienBangChu\"></span>',
                '{KH_TienBangChu}': '<span data-bind=\"text: KH_TienBangChu\"></span>',
                '{GhiChuChiNhanhChuyen}': '<span data-bind=\"text: GhiChuChiNhanhChuyen\"></span>',
                '{ChiNhanhBanHang}': '<span data-bind=\"text: ChiNhanhBanHang\"></span>',
                '{HoaDonLienQuan}': '<span data-bind=\"text: HoaDonLienQuan\"></span>',
                '{KhoanMucThuChi}': '<span data-bind=\"text: KhoanMucThuChi\"></span>',

                '{NguoiCanBang}': '<span data-bind=\"text: NguoiCanBang\"></span>',
                '{TrangThaiKK}': '<span data-bind=\"text: TrangThaiKK\"></span>',
                '{NgayTao}': '<span data-bind=\"text: NgayTao\"></span>',
                '{NgayCanBang}': '<span data-bind=\"text: NgayCanBang\"></span>',
                '{TongThucTe}': '<span data-bind=\"text: TongThucTe\"></span>',
                '{TongLechTang}': '<span data-bind=\"text: TongLechTang\"></span>',
                '{TongLechGiam}': '<span data-bind=\"text: TongLechGiam\"></span>',
                '{TongChenhLech}': '<span data-bind=\"text: TongChenhLech\"></span>',

                '{TienMat}': '<span data-bind=\"text: $root.TienMat\"></span>',
                '{TienPOS}': '<span data-bind=\"text: $root.TienGui\"></span>',
                '{TienChuyenKhoan}': '<span data-bind=\"text: TienATM\"></span>',
                '{TongGiamGiaHang}': '<span data-bind=\"text: $root.TongGiamGiaHang\"></span>',
                '{TongTienThue}': '<span data-bind=\"text: $root.TongTienThue\"></span>',
                '{TongThueKhachHang}': '<span data-bind=\"text: $root.TongThueKhachHang\"></span>',
                '{PTThueHoaDon}': '<span data-bind=\"text: $root.PTThueHoaDon\"></span>',
                '{TienThuaTraKhach}': '<span data-bind=\"text: $root.TienThua\"></span>',
                '{PTChietKhauHD}': '<span data-bind=\"text: $root.TongChietKhau\"></span>',
                '{TongTienHangChuaChietKhau}': '<span data-bind=\"text: $root.TongTienHangChuaCK\"></span>',
                '{TongGiamGiaHD_HH}': '<span data-bind=\"text: $root.TongGiamGiaHD_HH\"></span>',
                '{ChietKhauNVHoaDon}': '<span data-bind=\"text: ChietKhauNVHoaDon\"></span>',
                '{ChietKhauNVHoaDon_InGtriCK}': '<span data-bind=\"text: ChietKhauNVHoaDon_InGtriCK\"></span>',
                '{PhuongThucTT}': '<span data-bind=\"text: PhuongThucTT\"></span>',

                '{TenNganHangPOS}': '<span data-bind=\"text: TenNganHangPOS\"></span>',
                '{TenChuThePOS}': '<span data-bind=\"text: TenChuThePOS\"></span>',
                '{SoTaiKhoanPOS}': '<span data-bind=\"text: SoTaiKhoanPOS\"></span>',
                '{TenNganHangChuyenKhoan}': '<span data-bind=\"text: TenNganHangChuyenKhoan\"></span>',
                '{TenChuTheChuyenKhoan}': '<span data-bind=\"text: TenChuTheChuyenKhoan\"></span>',
                '{SoTaiKhoanChuyenKhoan}': '<span data-bind=\"text: SoTaiKhoanChuyenKhoan\"></span>',

                // value card
                '{MucNap}': '<span data-bind=\"text: TongChiPhi\"></span>',
                '{KhuyenMai}': '<span data-bind=\"text: $root.TongGiamGia\"></span>',
                '{ThanhTien}': '<span data-bind=\"text: TongTienHang\"></span>',
                '{TongTien}': '<span data-bind=\"text: TongTienHang\"></span>',

                '{TongTaiKhoanThe}': '<span data-bind=\"text: TongTaiKhoanThe\"></span>',
                '{TongSuDungThe}': '<span data-bind=\"text: TongSuDungThe\"></span>',
                '{SoDuConLai}': '<span data-bind=\"text: SoDuConLai\"></span>',
                '{TienDoiDiem}': '<span data-bind=\"text: $root.TienDoiDiem\"></span>',
                '{TienTheGiaTri}': '<span data-bind=\"text: $root.TienTheGiaTri\"></span>',

                '{Ngay}': '<span data-bind=\"text: Ngay\"></span>',
                '{Thang}': '<span data-bind=\"text: Thang\"></span>',
                '{Nam}': '<span data-bind=\"text: Nam\"></span>',

                '{MaPhieuTiepNhan}': '<span data-bind=\"text: MaPhieuTiepNhan\"></span>',
                '{NgayVaoXuong}': '<span data-bind=\"text: NgayVaoXuong\"></span>',
                '{NgayXuatXuong}': '<span data-bind=\"text: NgayXuatXuong\"></span>',
                '{NgayHoanThanhDuKien}': '<span data-bind=\"text: NgayXuatXuongDuKien\"></span>',
                '{CoVanDichVu}': '<span data-bind=\"text: CoVanDichVu\"></span>',
                '{CoVan_SDT}': '<span data-bind=\"text: CoVan_SDT\"></span>',
                '{NhanVienTiepNhan}': '<span data-bind=\"text: NhanVienTiepNhan\"></span>',

                // xe
                '{BienSo}': '<span data-bind=\"text: BienSo\"></span>',
                '{TenMauXe}': '<span data-bind=\"text: TenMauXe\"></span>',
                '{TenLoaiXe}': '<span data-bind=\"text: TenLoaiXe\"></span>',
                '{TenHangXe}': '<span data-bind=\"text: TenHangXe\"></span>',
                '{SoKhung}': '<span data-bind=\"text: SoKhung\"></span>',
                '{SoMay}': '<span data-bind=\"text: SoMay\"></span>',
                '{SoKmVao}': '<span data-bind=\"text: SoKmVao\"></span>',
                '{SoKmRa}': '<span data-bind=\"text: SoKmRa\"></span>',
                '{HopSo}': '<span data-bind=\"text: HopSo\"></span>',
                '{DungTich}': '<span data-bind=\"text: DungTich\"></span>',
                '{MauSon}': '<span data-bind=\"text: MauSon\"></span>',
                '{NamSanXuat}': '<span data-bind=\"text: NamSanXuat\"></span>',

                '{TongSL_DichVu}': '<span data-bind=\"text: $root.TongSL_DichVu\"></span>',
                '{TongTienDichVu}': '<span data-bind=\"text: $root.TongTienDichVu\"></span>',
                '{TongTienDichVu_TruocVAT}': '<span data-bind=\"text: $root.TongTienDichVu_TruocVAT\"></span>',
                '{TongThue_DichVu}': '<span data-bind=\"text: $root.TongThue_DichVu\"></span>',
                '{TongCK_DichVu}': '<span data-bind=\"text: $root.TongCK_DichVu\"></span>',
                '{TongTienDichVu_TruocCK}': '<span data-bind=\"text: $root.TongTienDichVu_TruocCK\"></span>',

                '{TongSL_PhuTung}': '<span data-bind=\"text: $root.TongSL_PhuTung\"></span>',
                '{TongThue_PhuTung}': '<span data-bind=\"text: $root.TongThue_PhuTung\"></span>',
                '{TongCK_PhuTung}': '<span data-bind=\"text: $root.TongCK_PhuTung\"></span>',
                '{TongTienPhuTung}': '<span data-bind=\"text: $root.TongTienPhuTung\"></span>',
                '{TongTienPhuTung_TruocVAT}': '<span data-bind=\"text: $root.TongTienPhuTung_TruocVAT\"></span>',
                '{TongTienPhuTung_TruocCK}': '<span data-bind=\"text: $root.TongTienPhuTung_TruocCK\"></span>',

                '{PhaiThanhToanBaoHiem}': '<span data-bind=\"text: $root.PhaiThanhToanBaoHiem\"></span>',
                '{TongThanhToan}': '<span data-bind=\"text: $root.TongThanhToan\"></span>',
                '{PTN_GhiChu}': '<span data-bind=\"text: PTN_GhiChu\"></span>',

                '{ChuXe}': '<span data-bind=\"text: ChuXe\"></span>',
                '{ChuXe_SDT}': '<span data-bind=\"text: ChuXe_SDT\"></span>',
                '{ChuXe_DiaChi}': '<span data-bind=\"text: ChuXe_DiaChi\"></span>',
                '{ChuXe_Email}': '<span data-bind=\"text: ChuXe_Email\"></span>',

                '{LH_Ten}': '<span data-bind=\"text: LH_Ten\"></span>',
                '{LH_SDT}': '<span data-bind=\"text: LH_SDT\"></span>',

                '{TenBaoHiem}': '<span data-bind=\"text: TenBaoHiem\"></span>',
                '{BH_SDT}': '<span data-bind=\"text: BH_SDT\"></span>',
                '{BH_Email}': '<span data-bind=\"text: BH_Email\"></span>',
                '{BH_DiaChi}': '<span data-bind=\"text: BH_DiaChi\"></span>',
                '{BH_TenLienHe}': '<span data-bind=\"text: BH_TenLienHe\"></span>',
                '{BH_SDTLienHe}': '<span data-bind=\"text: BH_SDTLienHe\"></span>',
                '{BaoHiemDaTra}': '<span data-bind=\"text: BaoHiemDaTra\"></span>',
                '{BH_TienBangChu}': '<span data-bind=\"text: BH_TienBangChu\"></span>',

                '{TongTienBHDuyet}': '<span data-bind=\"text: $root.TongTienBHDuyet\"></span>',
                '{PTThueBaoHiem}': '<span data-bind=\"text: $root.PTThueBaoHiem\"></span>',
                '{TongTienThueBaoHiem}': '<span data-bind=\"text: $root.TongTienThueBaoHiem\"></span>',
                '{SoVuBaoHiem}': '<span data-bind=\"text: $root.SoVuBaoHiem\"></span>',
                '{KhauTruTheoVu}': '<span data-bind=\"text: $root.KhauTruTheoVu\"></span>',
                '{PTGiamTruBoiThuong}': '<span data-bind=\"text: $root.PTGiamTruBoiThuong\"></span>',
                '{GiamTruBoiThuong}': '<span data-bind=\"text: $root.GiamTruBoiThuong\"></span>',
                '{BHThanhToanTruocThue}': '<span data-bind=\"text: $root.BHThanhToanTruocThue\"></span>',

                // phieuluong
                '{NgayLapPhieu}': '<span data-bind=\"text: $root.NgayLapPhieu\"></span>',
                '{TenBangLuong}': '<span data-bind=\"text: $root.TenBangLuong\"></span>',
                '{KyTinhLuong}': '<span data-bind=\"text: $root.KyTinhLuong\"></span>',
                '{MaBangLuongChiTiet}': '<span data-bind=\"text: $root.MaBangLuongChiTiet\"></span>',
                '{MaNhanVien}': '<span data-bind=\"text: $root.MaNhanVien\"></span>',
                '{TenNhanVien}': '<span data-bind=\"text: $root.TenNhanVien\"></span>',
                '{NgayCongChuan}': '<span data-bind=\"text: $root.NgayCongChuan\"></span>',
                '{NgayCongThuc}': '<span data-bind=\"text: $root.NgayCongThuc\"></span>',
                '{LuongCoBan}': '<span data-bind=\"text: $root.LuongCoBan\"></span>',
                '{LuongChinh}': '<span data-bind=\"text: $root.LuongChinh\"></span>',
                '{LuongOT}': '<span data-bind=\"text: $root.LuongOT\"></span>',
                '{PhuCapCoBan}': '<span data-bind=\"text: $root.PhuCapCoBan\"></span>',
                '{PhuCapKhac}': '<span data-bind=\"text: $root.PhuCapKhac\"></span>',
                '{ChietKhau}': '<span data-bind=\"text: $root.ChietKhau\"></span>',
                '{TongGiamTru}': '<span data-bind=\"text: $root.TongGiamTru\"></span>',
                '{LuongSauGiamTru}': '<span data-bind=\"text: $root.LuongSauGiamTru\"></span>',
                '{TruTamUngLuong}': '<span data-bind=\"text: $root.TruTamUngLuong\"></span>',
                '{ThucLinh}': '<span data-bind=\"text: $root.ThucLinh\"></span>',
                '{ThanhToan}': '<span data-bind=\"text: $root.ThanhToan\"></span>',
                '{NguoiLapPhieu}': '<span data-bind=\"text: $root.NguoiLapPhieu\"></span>',
            });
        return result;
    }
    return "";
};