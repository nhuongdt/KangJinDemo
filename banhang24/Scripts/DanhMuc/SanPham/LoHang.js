ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        //initialize datepicker with some optional options
        var options = {
            format: 'DD/MM/YYYY',
            defaultDate: new Date()
        };

        if (allBindingsAccessor() !== undefined) {
            if (allBindingsAccessor().datepickerOptions !== undefined) {
                options.format = allBindingsAccessor().datepickerOptions.format !== undefined ? allBindingsAccessor().datepickerOptions.format : options.format;
            }
        }

        $(element).datetimepicker(options);

        //when a user changes the date, update the view model
        ko.utils.registerEventHandler(element, "dp.change", function (event) {
            var value = valueAccessor();
            if (ko.isObservable(value)) {
                value(event.date);
            }
        });

        var defaultVal = $(element).val();
        var value = valueAccessor();
        value(moment(defaultVal, options.format));
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var thisFormat = 'DD/MM/YYYY';
        if (allBindingsAccessor() !== undefined) {
            if (allBindingsAccessor().datepickerOptions !== undefined) {
                thisFormat = allBindingsAccessor().datepickerOptions.format !== undefined ? allBindingsAccessor().datepickerOptions.format : thisFormat;
            }
        }

        var value = valueAccessor();
        var unwrapped = ko.utils.unwrapObservable(value());
        if (unwrapped === undefined || unwrapped === null) {
            element.value = new moment(new Date());
            console.log("undefined");
        } else {
            element.value = moment(unwrapped).format(thisFormat);
        }
    }
};

var FormModel_NewLoHang = function () {
    var self = this;
    self.ID = ko.observable();
    self.MaLoHang = ko.observable();
    self.NgaySanXuat = ko.observable();
    self.NgayHetHan = ko.observable();
    self.TrangThai = ko.observable();

    self.SetData = function (item) {
        self.ID(item.ID);
        self.MaLoHang(item.MaLoHang);
        self.NgaySanXuat(item.NgaySanXuat);
        self.NgayHetHan(item.NgayHetHan);
        self.TrangThai(item.TrangThai);
    };
};

var LoHangViewModel = function () {
    var self = this;
    var GiaBanUri = '/api/DanhMuc/DM_GiaBanAPI/';
    var NhomHHUri = '/api/DanhMuc/DM_NhomHangHoaAPI/';
    var _IDchinhanh = $('#hd_IDdDonVi').val();
    var _IDNhomNguoiDung = $('.idnhomnguoidung').text();
    var _IDNguoiDung = $('.idnguoidung').text();
    var _id_NhanVien = $('.idnhanvien').text();
    var DMHangHoaUri = '/api/DanhMuc/DM_HangHoaAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/';
    var userLogin = $('#txtTenTaiKhoan').text();

    self.newLoHang = ko.observable(new FormModel_NewLoHang());
    self.Loc_TonKho = ko.observable('0');
    self.filter = ko.observable();
    self.filterNgayLapHD = ko.observable("0");
    self.filterNgayLapHD_Input = ko.observable(); // ngày cụ thể

    self.HangHoa_XemGiaVon = ko.observable();
    self.HangHoa_GiaBan = ko.observable();
    self.HangHoa_GiaVon = ko.observable();
    var ReportUri = '/api/DanhMuc/ReportAPI/';
    var _IDDoiTuong = $('.idnguoidung').text();
    function getQuyen_NguoiDung() {
        ajaxHelper(ReportUri + "getQuyenXemGiaVon?ID_NguoiDung=" + _IDDoiTuong + "&MaQuyen=" + "HangHoa_XemGiaVon", "GET").done(function (data) {
            self.HangHoa_XemGiaVon(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _IDchinhanh + "&MaQuyen=" + "HangHoa_GiaBan", "GET").done(function (data) {
            self.HangHoa_GiaBan(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _IDchinhanh + "&MaQuyen=" + "HangHoa_GiaVon", "GET").done(function (data) {
            self.HangHoa_GiaVon(data);
        })
    };
    getQuyen_NguoiDung();
    self.NhomHangHoas = ko.observableArray();
    function GetAllNhomHH() {
        ajaxHelper('/api/DanhMuc/DM_NhomHangHoaAPI/' + 'GetDM_NhomHangHoa', 'GET').done(function (data) {
            localStorage.setItem('lc_NhomHangHoas', JSON.stringify(data));
            self.NhomHangHoas([]);
            for (var i = 0; i < data.length; i++) {
                if (data[i].ID_Parent == null) {
                    var objParent = {
                        ID: data[i].ID,
                        TenNhomHangHoa: data[i].TenNhomHangHoa,
                        Childs: [],
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
        });
    };
    GetAllNhomHH();

    self.NoteNhomHang = function () {
        //clearTimeout(time);
        //time = setTimeout(
        //    function () {
        tk = $('#SeachNhomHang').val();
        if (tk.trim() != '') {
            ajaxHelper('/api/DanhMuc/ReportAPI/' + "GetListID_NhomHangHoaByTen?TenNhomHang=" + tk, 'GET').done(function (data) {
                self.NhomHangHoas([]);
                for (var i = 0; i < data.length; i++) {
                    if (data[i].ID_Parent == null) {
                        var objParent = {
                            ID: data[i].ID,
                            TenNhomHangHoa: data[i].TenNhomHangHoa,
                            Childs: [],
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
                    else {
                        var objParent = {
                            ID: data[i].ID,
                            TenNhomHangHoa: data[i].TenNhomHangHoa,
                            Childs: [],
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
                if (self.NhomHangHoas().length > 10) {
                    $('.close-goods').css('display', 'block');
                }
            })
        }
        else {
            GetAllNhomHH();
        }
        //}, 300);
    };
    self.arrThuocTinh = ko.observableArray();
    function getallThuocTinh() {
        ajaxHelper(DMHangHoaUri + "GetallThuocTinh", 'GET').done(function (data) {
            self.arrThuocTinh(data);
            if (self.arrThuocTinh().length > 0) {
                $(".close-goods").click(function () {
                    $(this).prev(".op-filter-container ").toggleClass("scoll-tt");
                });
                $('.attribute-tt ').on('click', function () {
                    $(this).next(".op-filter-container").slideToggle();
                    $(this).find("a i").not(".plus_not").toggle();
                    $(".op-filter-title span").css("display", "block");
                    //$(this).parent(".boxLeft").toggleClass("border-b");
                });
            }
        });
    }

    getallThuocTinh();
    $('.hideloadGiatri').hide();
    self.arrGiaTriThuocTinh = ko.observableArray();
    self.LoadGiaTri = function (item) {
        ajaxHelper(DMHangHoaUri + "getGiaTriTTByID_ThuocTinh?idthuoctinh=" + item.ID, 'GET').done(function (data) {
            self.arrGiaTriThuocTinh(data);
        });
        $(".op-filter-container  ul").click(function () {
            var hClass = $(this).parent(".op-filter-container").hasClass('scoll-tt');
            if (hClass == "true") {
                $(this).parent(".op-filter-container").addClass('scoll-tt');
            }
            else {
                $(this).parent(".op-filter-container").removeClass('scoll-tt');
            }

        });
    }

    self.arrListThuocTinh = ko.observableArray();
    self.ListThuocTinh = ko.observableArray();
    self.hienThiGiaTri = function (item) {
        if (item.ID_ThuocTinh !== undefined) {
            $('span[id=txtTenThuocTinh_' + item.ID_ThuocTinh + ']').html(item.GiaTri);
            var index = self.arrListThuocTinh().map(function (e) {
                return e.ID_ThuocTinh;
            }).indexOf(item.ID_ThuocTinh);
            if (index >= 0) {
                self.arrListThuocTinh.remove(self.arrListThuocTinh()[index]);
                self.arrListThuocTinh.push(item);
            }
            else {
                self.arrListThuocTinh.push(item);
            }
        }
        else {
            var index1 = self.arrListThuocTinh().map(function (e) { return e.ID_ThuocTinh; }).indexOf(item.ID);
            if (index1 >= 0) {
                self.arrListThuocTinh.remove(self.arrListThuocTinh()[index1]);
            }
            $('span[id=txtTenThuocTinh_' + item.ID + ']').html('---' + item.TenThuocTinh + '---');
        }
        self.ListThuocTinh([]);
        for (var i = 0; i < self.arrListThuocTinh().length; i++) {
            self.ListThuocTinh.push(self.arrListThuocTinh()[i].GiaTri + self.arrListThuocTinh()[i].ID_ThuocTinh.toUpperCase())
        }
        SearchHangHoa();
    }
    self.arrIDNhomHang = ko.observableArray([]);

    function getAllDMLoHang() {
        self.arrIDNhomHang([]);
        SearchHangHoa();
    }

    self.changeddlNhomHangHoa = function (item) {
        //$("#iconSort").remove();
        //self.columsort(null);
        //self.sort(null);
        //self.currentPage(0);
        $('.ss-li .li-oo').removeClass("yellow");
        $('#tatcanhh').removeClass("yellow")
        $('.li-pp').removeClass("yellow");
        $('#tatcanhh a').css("display", "none");
        $('#' + item.ID).addClass("yellow");
        if (item.ID == undefined) {
            getAllDMLoHang();
        } else {
            var arrIDChilds = [];
            var lcNhomHH = localStorage.getItem('lc_NhomHangHoas');
            if (lcNhomHH !== null) {
                lcNhomHH = JSON.parse(lcNhomHH);
                for (var i = 0; i < lcNhomHH.length; i++) {
                    if (lcNhomHH[i].ID === item.ID) {
                        for (var j = 0; j < lcNhomHH.length; j++) {
                            if (lcNhomHH[j].ID_Parent === item.ID) {
                                // get ID_Child level 1
                                arrIDChilds.push(lcNhomHH[j].ID);
                                for (var k = 0; k < lcNhomHH.length; k++) {
                                    if (lcNhomHH[k].ID_Parent === lcNhomHH[j].ID) {
                                        // get ID_Child level 2
                                        arrIDChilds.push(lcNhomHH[k].ID);
                                    }
                                }
                            }
                        }
                        arrIDChilds.push(lcNhomHH[i].ID);
                        break;
                    }
                }
            }
            self.arrIDNhomHang(arrIDChilds);
            SearchHangHoa();

        }

        $(".checkboxsetHH").prop("checked", false);
    }

    self.getAllDMLoHang = function () {
        $(".checkboxsetHH").prop("checked", false);
        self.arrIDNhomHang([]);
        SearchHangHoa();

        $('.li-oo').removeClass("yellow")
        $('#tatcanhh a').css("display", "block");
        $('#tatcanhh').addClass("yellow")
    }

    $('#txtSearchLoHang').keypress(function (e) {
        if (e.keyCode === 13) {
            self.currentPage(0);
            SearchHangHoa();
        }
    })

    self.showpopupLoHang = function (item) {
        self.newLoHang(new FormModel_NewLoHang());
        ajaxHelper(DMHangHoaUri + 'GetDM_LoHang?id=' + item.ID_LoHang, 'GET').done(function (data) {
            self.newLoHang().SetData(data);
            $('#modalPopuplg_EditLoHang').modal('show');
        })
    }

    self.editLoHang = function () {
        var _id = self.newLoHang().ID();
        var _maloHang = (self.newLoHang().MaLoHang());
        var _ngaySanXuat = moment($('#txtNgaySanXuat').val(), 'DD/MM/YYYY').format("YYYY-MM-DD");
        var _ngayHethan = moment($('#txtNgayHetHan').val(), 'DD/MM/YYYY').format("YYYY-MM-DD");
        var _trangThai = self.newLoHang().TrangThai();

        var specialChars = "<>!#$%^&+{}?:;|'\"\\,./~`=' '"
        var check = function (string) {
            for (i = 0; i < specialChars.length; i++) {
                if (string.indexOf(specialChars[i]) > -1) {
                    return true
                }
            }
            return false;
        }
        if (check(_maloHang) == false) {
        }
        else {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Mã lô hàng không được chứa ký tự đặc biệt", "danger");
            $('#txtMaLoHang').val("");
            $('#txtMaLoHang').focus();
            return false;
        }
        if (_ngaySanXuat !== "Invalid date" && _ngayHethan !== "Invalid date" && _ngaySanXuat > _ngayHethan) {
            $('#txtNgaySanXuat').focus();
            $('#txtNgaySanXuat').val("");
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + 'Ngày hết hạn không được bé hơn ngày sản xuất', 'danger');
            return false;
        }

        var myData = {};
        myData.ID = _id;
        DM_LoHang = {
            ID: _id,
            MaLoHang: _maloHang,
            NgaySanXuat: _ngaySanXuat === "Invalid date" ? null : _ngaySanXuat,
            NgayHetHan: _ngayHethan === "Invalid date" ? null : _ngayHethan,
            TrangThai: _trangThai,
            NguoiSua: userLogin
        }
        myData.objLoHang = DM_LoHang;

        $.ajax({
            url: DMHangHoaUri + "PutDM_LoHang",
            type: 'PUT',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: myData,
            success: function (item) {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Sửa lô hàng thành công", "success");
                $('#modalPopuplg_EditLoHang').modal('hide');
                SearchHangHoa();
            },
            error: function (jqXHR, textStatus, errorThrown) {
            }
        });
    }
    //phân trang
    self.pageSizes = [10, 20, 30];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.currentPage = ko.observable(0);
    self.fromitem = ko.observable(1);
    self.toitem = ko.observable();

    self.TotalRecord = ko.observable();
    self.PageCount = ko.observable();
    self.DMLoHangs = ko.observableArray();

    $('#txtNgayTaoInput').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        SearchHangHoa();
    });

    self.filterNgayLapHD.subscribe(function (newVal) {
        self.currentPage(0);
        SearchHangHoa();
    });

    self.ExportDMHHtoExcel = async function () {
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Danh Mục lô hàng",
            NoiDung: "Xuất báo cáo danh sách lô hàng",
            NoiDungChiTiet: "Xuất báo cáo danh sách lô hàng",
            LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        var columnHide = null;
        for (var i = 0; i < self.ColumnsExcel().length; i++) {
            if (i == 0) {
                columnHide = self.ColumnsExcel()[i];
            }
            else {
                columnHide = self.ColumnsExcel()[i] + "_" + columnHide;
            }
        }
        const exportOK = await commonStatisJs.NPOI_ExportExcel(DMHangHoaUri + 'ExportExel_DMLoHang?idnhomhang=' + self.arrIDNhomHang() +
            '&maHoaDon=' + txtmMaHDon_Excel + '&tonkho=' + txtTonKho_Excel + '&columnsHide=' + columnHide + '&iddonvi=' + _IDchinhanh + '&listthuoctinh=' + self.ListThuocTinh() + '&dayStart=' + dayStart_Excel + '&dayEnd=' + dayEnd_Excel + '&time=' + self.TodayBC(), 'GET', null, 'DanhMucLoHang.xlsx');
        if (exportOK) {
            Insert_NhatKyThaoTac_1Param(objDiary);
        }
    }

    self.ColumnsExcel = ko.observableArray();
    var arrColumnsExcel = [];
    self.addColum = function (item) {
        if (self.ColumnsExcel().length < 1) {
            self.ColumnsExcel.push(item);
        }
        else {
            for (var i = 0; i < self.ColumnsExcel().length; i++) {
                if (self.ColumnsExcel()[i] === item) {
                    self.ColumnsExcel.splice(i, 1);
                    break;
                }
                if (i == self.ColumnsExcel().length - 1) {
                    self.ColumnsExcel.push(item);
                    break;
                }
            }
        }
        self.ColumnsExcel.sort();
    }

    var txtmMaHDon_Excel;
    var txtTonKho_Excel;
    var dayStart_Excel;
    var dayEnd_Excel;

    self.TodayBC = ko.observable();

    self.LoadQuyen = function () {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('LoHang_CapNhat', lc_CTQuyen) > -1) {
            $('.txtLoHangCapNhat').show();
        }
        else {
            $('.txtLoHangCapNhat').hide();
        }
    }

    function SearchHangHoa() {
        $('.line-right').height(0).css("margin-top", "0px");
        $('.prev-tr-hide .check-group input').each(function () {
            $(this).prop('checked', false);
        });
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('LoHang_XuatFile', lc_CTQuyen) > -1) {
            $('.xuatfilelohang').show();
        }
        else {
            $('.xuatfilelohang').hide();
        }
        if ($.inArray('LoHang_XemDS', lc_CTQuyen) > -1) {
            var FindLoHang = localStorage.getItem('FindLoHang');
            if (FindLoHang !== null) {
                self.filter(FindLoHang);
            }
            var txtMaHDon = self.filter();

            if (txtMaHDon === undefined) {
                txtMaHDon = "";
            }
            txtmMaHDon_Excel = txtMaHDon;
            var tonkho = 1;
            if (self.Loc_TonKho() === '0') {
                tonkho = 3;
            }
            if (self.Loc_TonKho() === '2') {
                tonkho = 2;
            }
            if (self.Loc_TonKho() === '1') {
                tonkho = 1;
            }
            if (self.Loc_TonKho() === '3') {
                tonkho = 4;
            }
            if (self.Loc_TonKho() === '4') {
                tonkho = 5;
            }
            txtTonKho_Excel = tonkho;
            var dayStart = null;
            var dayEnd = null;
            var _now = new Date();
            if (self.filterNgayLapHD() === '0') {
                self.TodayBC('Toàn thời gian');
            }
            else {
                if ($('#txtNgayTaoInput').val() !== "") {
                    var arrDate = $('#txtNgayTaoInput').val().split('-');

                    dayStart = moment(arrDate[0], 'DD/MM/YYYY').format('YYYY-MM-DD');
                    dayEnd = moment(arrDate[1], 'DD/MM/YYYY').format('YYYY-MM-DD');
                    self.TodayBC('Từ ' + moment(arrDate[0], 'DD/MM/YYYY').format('DD/MM/YYYY') + ' đến ' + moment(arrDate[1], 'DD/MM/YYYY').format('DD/MM/YYYY'));
                }
                else {
                    self.TodayBC('Từ ' + moment().startOf('day').endOf('day').format('DD/MM/YYYY') + ' đến ' + moment().startOf('day').endOf('day').format('DD/MM/YYYY'));
                }
            }
            var check = 0;
            var thongbaolosaphethan = localStorage.getItem('ThongBaoLoHangSapHetHan');
            if (thongbaolosaphethan === null) {
                var thongbaolodahethan = localStorage.getItem('ThongBaoLoHangDaHetHan');
                if (thongbaolodahethan === null) {
                    dayStart_Excel = dayStart;
                    dayEnd_Excel = dayEnd;
                    //check
                    $('.table-HH').gridLoader();
                    ajaxHelper(DMHangHoaUri + 'GetListDMLoHang?currentPage=' + self.currentPage() + '&pageSize=' + self.pageSize() + '&idnhomhang=' + self.arrIDNhomHang() +
                        '&maHoaDon=' + txtMaHDon + '&tonkho=' + tonkho + '&iddonvi=' + _IDchinhanh + '&listthuoctinh=' + self.ListThuocTinh() + '&dayStart=' + dayStart + '&dayEnd=' + dayEnd + '&checkngay=' + check,
                        'GET').done(function (data1) {
                            $('.table-HH').gridLoader({ show: false });

                            self.DMLoHangs(data1.data.lstHH);
                            self.TotalRecord(data1.TotalRecord);
                            self.PageCount(data1.PageCount);
                        });
                    localStorage.removeItem('FindLoHang');
                    localStorage.removeItem('ThongBaoLoHangSapHetHan');
                    localStorage.removeItem('ThongBaoLoHangDaHetHan');
                }
                else {
                    self.filterNgayLapHD("1");
                    dayStart = dayEnd = new Date();
                    dayStart = moment(dayStart).format('YYYY-MM-DD');
                    dayEnd = moment(dayEnd).format('YYYY-MM-DD');
                    dayStart_Excel = dayStart;
                    dayEnd_Excel = dayEnd;
                    tonkho = 1;
                    self.Loc_TonKho('1');
                    var setDayStart = moment(dayStart, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    var setDayEnd = moment(dayEnd, 'YYYY-MM-DD').format('DD/MM/YYYY');

                    $(function () {
                        $('#txtNgayTaoInput').daterangepicker({
                            timePicker: true,
                            startDate: setDayStart,
                            endDate: setDayEnd,
                            locale: {
                                format: 'DD/MM/YYYY'
                            },
                            timePicker24Hour: true
                        });
                    });

                    $('.table-HH').gridLoader();
                    ajaxHelper(DMHangHoaUri + 'GetListDMLoHang?currentPage=' + self.currentPage() + '&pageSize=' + self.pageSize() + '&idnhomhang=' + self.arrIDNhomHang() +
                        '&maHoaDon=' + txtMaHDon + '&tonkho=' + tonkho + '&iddonvi=' + _IDchinhanh + '&listthuoctinh=' + self.ListThuocTinh() + '&dayStart=' + dayStart + '&dayEnd=' + dayEnd + '&checkngay=' + check,
                        'GET').done(function (data1) {
                            $('.table-HH').gridLoader({ show: false });

                            self.DMLoHangs(data1.data.lstHH);
                            self.TotalRecord(data1.TotalRecord);
                            self.PageCount(data1.PageCount);
                        });
                    localStorage.removeItem('FindLoHang');
                    localStorage.removeItem('ThongBaoLoHangSapHetHan');
                    localStorage.removeItem('ThongBaoLoHangDaHetHan');
                }
            }
            else {
                self.filterNgayLapHD("1");
                ajaxHelper(DMHangHoaUri + 'GetNgayNhacLoHetHan?iddonvi=' + _IDchinhanh, 'GET').done(function (data) {
                    if (data !== 0) {
                        check = data;
                        dayStart = dayEnd = new Date();
                        dayStart = moment(dayStart).add('days', data).format('YYYY-MM-DD');
                        dayEnd = moment(dayEnd).add('days', data).format('YYYY-MM-DD');
                        dayStart_Excel = dayStart;
                        dayEnd_Excel = dayEnd;
                        tonkho = 1;
                        self.Loc_TonKho('1');
                        var setDayStart = moment(dayStart, 'YYYY-MM-DD').format('DD/MM/YYYY');
                        var setDayEnd = moment(dayEnd, 'YYYY-MM-DD').format('DD/MM/YYYY');
                        //$('#txtNgayTaoInput').val(setDayStart + ' - ' + setDayEnd);

                        $(function () {
                            $('#txtNgayTaoInput').daterangepicker({
                                timePicker: true,
                                startDate: setDayStart,
                                endDate: setDayEnd,
                                locale: {
                                    format: 'DD/MM/YYYY'
                                },
                                timePicker24Hour: true
                            });
                        });
                        $('.table-HH').gridLoader();
                        ajaxHelper(DMHangHoaUri + 'GetListDMLoHang?currentPage=' + self.currentPage() + '&pageSize=' + self.pageSize() + '&idnhomhang=' + self.arrIDNhomHang() +
                            '&maHoaDon=' + txtMaHDon + '&tonkho=' + tonkho + '&iddonvi=' + _IDchinhanh + '&listthuoctinh=' + self.ListThuocTinh() + '&dayStart=' + dayStart + '&dayEnd=' + dayEnd + '&checkngay=' + check,
                            'GET').done(function (data1) {
                                $('.table-HH').gridLoader({ show: false });

                                self.DMLoHangs(data1.data.lstHH);
                                self.TotalRecord(data1.TotalRecord);
                                self.PageCount(data1.PageCount);
                            });
                        localStorage.removeItem('FindLoHang');
                        localStorage.removeItem('ThongBaoLoHangSapHetHan');
                        localStorage.removeItem('ThongBaoLoHangDaHetHan');
                    }
                });
            }
        }
    }
    SearchHangHoa();

    self.clickiconSearchBG = function () {
        SearchHangHoa();
    }

    self.PageResults = ko.computed(function () {
        if (self.DMLoHangs() !== null) {

            self.fromitem((self.currentPage() * self.pageSize()) + 1);

            if (((self.currentPage() + 1) * self.pageSize()) > self.DMLoHangs().length) {
                var fromItem = (self.currentPage() + 1) * self.pageSize();
                if (fromItem < self.TotalRecord()) {
                    self.toitem((self.currentPage() + 1) * self.pageSize());
                }
                else {
                    self.toitem(self.TotalRecord());
                }
            } else {
                self.toitem((self.currentPage() * self.pageSize()) + self.pageSize());
            }
        }
    });


    self.PageList_Display = ko.computed(function () {
        var arrPage = [];
        var allPage = self.PageCount();
        var currentPage = self.currentPage();

        if (allPage > 4) {

            var i = 0;
            if (currentPage === 0) {
                i = parseInt(self.currentPage()) + 1;
            }
            else {
                i = self.currentPage();
            }

            if (allPage >= 5 && currentPage > allPage - 5) {
                if (currentPage >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (var i = allPage - 5; i < allPage; i++) {
                        var obj = {
                            pageNumber: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    // get currentPage - 2 , currentPage, currentPage + 2
                    if (currentPage == 1) {
                        for (var j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    } else {
                        for (var j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                            var obj = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    }
                }
            }
            else {
                // get 5 trang dau
                if (i >= 2) {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumber: i - 1,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
                else {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumber: i,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
            }
        }
        else {
            // neu chi co 1 trang --> khong hien thi DS trang
            if (allPage > 1) {
                for (var i = 0; i < allPage; i++) {
                    var obj = {
                        pageNumber: i + 1,
                    };
                    arrPage.push(obj);
                }
            }
        }
        return arrPage;
    });

    self.VisibleStartPage = ko.computed(function () {
        if (self.PageList_Display().length > 0) {
            return self.PageList_Display()[0].pageNumber !== 1;
        }
    });

    self.VisibleEndPage = ko.computed(function () {
        if (self.PageList_Display().length > 0) {
            return self.PageList_Display()[self.PageList_Display().length - 1].pageNumber !== self.PageCount();
        }
    });

    self.GoToPageHD = function (page) {
        if (page.pageNumber !== '.') {
            self.currentPage(page.pageNumber - 1);
            SearchHangHoa();
            //$(".checkboxsetHH").prop("checked", false);
        }
    };

    self.StartPage = function () {
        self.currentPage(0);
        SearchHangHoa();
    }

    self.BackPage = function () {
        if (self.currentPage() > 1) {
            self.currentPage(self.currentPage() - 1);
            SearchHangHoa();
        }
    }

    self.GoToNextPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.currentPage() + 1);
            SearchHangHoa();
        }
    }

    self.EndPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.PageCount() - 1);
            SearchHangHoa();
        }
    }

    self.ResetCurrentPage = function () {
        self.currentPage(0);
        SearchHangHoa();
    };

    self.GetClassHD = function (page) {
        return ((page.pageNumber - 1) === self.currentPage()) ? "click" : "";
    };

    self.Loc_TonKho.subscribe(function (newVal) {
        self.currentPage(0);
        SearchHangHoa();
    });
    self.ListHeaderInit = [
        { colName: 'colMaHang', colText: 'Mã hàng', colShow: true, index: 0 },
        { colName: 'colTenHang', colText: 'Tên hàng', colShow: true, index: 1 },
        { colName: 'colDonViTinh', colText: 'Đvt', colShow: true, index: 2 },
        { colName: 'colNhomHang', colText: 'Nhóm hàng', colShow: true, index: 3 },
        { colName: 'colGiaBan', colText: 'Giá bán', colShow: true, index: 4 },
        { colName: 'colSoLuongTon', colText: 'Số lượng tồn', colShow: true, index: 5 },
        { colName: 'colGiaVon', colText: 'Giá vốn', colShow: true, index: 6 },
        { colName: 'colGiaTriTon', colText: 'Giá trị tồn', colShow: true, index: 7 },
        { colName: 'colMaLoHang', colText: 'Mã lô hàng', colShow: true, index: 8 },
        { colName: 'colNgaySanXuat', colText: 'Ngày sản xuất', colShow: true, index: 9 },
        { colName: 'colNgayHetHan', colText: 'Ngày hết hạn', colShow: true, index: 10 },
        { colName: 'colTrangThai', colText: 'Trạng thái', colShow: true, index: 11 },
        { colName: 'colNgayConHan', colText: 'Ngày còn hạn/Hết hạn', colShow: true, index: 12 },
    ];

    self.ListHeader = ko.observableArray([
        { colName: 'colMaHang', colText: 'Mã hàng', colShow: true, index: 0 },
        { colName: 'colTenHang', colText: 'Tên hàng', colShow: true, index: 1 },
        { colName: 'colDonViTinh', colText: 'Đvt', colShow: true, index: 2 },
        { colName: 'colNhomHang', colText: 'Nhóm hàng', colShow: true, index: 3 },
        { colName: 'colGiaBan', colText: 'Giá bán', colShow: true, index: 4 },
        { colName: 'colSoLuongTon', colText: 'Số lượng tồn', colShow: true, index: 5 },
        { colName: 'colGiaVon', colText: 'Giá vốn', colShow: true, index: 6 },
        { colName: 'colGiaTriTon', colText: 'Giá trị tồn', colShow: true, index: 7 },
        { colName: 'colMaLoHang', colText: 'Mã lô hàng', colShow: true, index: 8 },
        { colName: 'colNgaySanXuat', colText: 'Ngày sản xuất', colShow: true, index: 9 },
        { colName: 'colNgayHetHan', colText: 'Ngày hết hạn', colShow: true, index: 10 },
        { colName: 'colTrangThai', colText: 'Trạng thái', colShow: true, index: 11 },
        { colName: 'colNgayConHan', colText: 'Ngày còn hạn/Hết hạn', colShow: true, index: 12 },
    ]);
    /*self.ListHeader(self.ListHeaderInit);*/
    self.ListHeader.subscribe(function (newVal) {
        console.log(self.ListHeaderInit.filter(p => p.colShow === true).length);
    });

    //self.ShowHideHeader = function () {
    //    console.log(self.ListHeaderInit.filter(p=>p.colShow === true).length);
    //    /*self.ListHeader(self.ListHeaderInit);*/
    //}
}
var DMLoHang = new LoHangViewModel();
ko.applyBindings(DMLoHang);
