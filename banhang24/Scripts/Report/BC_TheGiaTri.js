var ViewReportValueCard = function () {
    var self = this;

    var ReportUri = '/api/DanhMuc/ReportAPI/';
    var _tenDonVi = $('#_txtTenDonVi').text();
    var _idDonVi = $('#hd_IDdDonVi').val();
    var _userLogin = $('#txtTenTaiKhoan').text();
    var _userID = $('.idnguoidung').text();
    var _idNhanVien = $('.idnhanvien').text();
    var _elmCheckAfterLi = '<i class="fa fa-check check-after-li" style="display:block"></i>';
    var Key_Form = 'Key_RpValueCardBalance';

    self.Quyen_NguoiDung = ko.observableArray();
    self.ListCheckBox = ko.observableArray();
    self.NumberColum_Div2 = ko.observable();
    self.TypeReport = ko.observable(1);
    self.RdoTypeTime = ko.observable('1');
    self.Status = ko.observable('1');
    self.LiInput_Time = ko.observable(6);
    self.ChiNhanhs = ko.observableArray();
    self.ChiNhanhChosed = ko.observableArray();
    self.TodayBC = ko.observable('Tháng này');
    self.TenChiNhanhs = ko.observable(_tenDonVi);
    self.ReportBalanceCard = ko.observableArray();
    self.ReportDiaryCard = ko.observableArray();
    self.ReportServiceUsed = ko.observableArray();
    self.ArrDonVi = ko.observableArray();
    self.LstIDDonVi = ko.observableArray([_idDonVi]);

    self.Allow_ChangeTimeSoQuy = ko.observable(false);
    self.Show_BtnUpdateSoQuy = ko.observable(false);
    self.Show_BtnDeleteSoQuy = ko.observable(false);

    // paging
    self.PageSizes = ko.observableArray([10, 20, 30]);
    self.CurrentPageSize = ko.observable(self.PageSizes()[0]);
    self.CurrentPage = ko.observable(0);
    self.TotalRow = ko.observable(0);
    self.TotalPage = ko.observable(0);
    self.FromItem = ko.observable(0);
    self.ToItem = ko.observable(0);

    // sum footer
    self.TongPhatSinhTang = ko.observable();
    self.TongPhatSinhGiam = ko.observable();
    self.TongDuCuoi = ko.observable();

    function Page_Load() {
        GetAllChiNhanh();
        GetHT_Quyen_ByNguoiDung();
        //LoadCheckBox(5);
    }

    function LoadCheckBox(typeCheck) {
        $.getJSON("api/DanhMuc/BaseApi/GetCheckedStatic?type=" + typeCheck, function (data) {
            self.ListCheckBox(data);
            self.NumberColum_Div2(Math.ceil(data.length / 2));
            LoadHtmlGrid();
        });
    }

    function LoadHtmlGrid() {
        LocalCaches.LoadFirstColumnGrid(Key_Form, $('#select-column ul li input[type = checkbox]'), self.ListCheckBox());
    }

    function AddCheckAfterLi(idChiNhanh) {
        $('#selec-all-DonVi li').each(function () {
            if ($(this).attr('id') === idChiNhanh) {
                $(this).find('.fa-check').remove();
                $(this).append(_elmCheckAfterLi);
            }
        });
        $('#choose_TenDonVi input').remove();
    }

    function GetAllChiNhanh() {
        ajaxHelper('/api/DanhMuc/DM_DonViAPI/' + "GetListDonViByIDNguoiDung?idnhanvien=" + _idNhanVien, 'GET').done(function (data) {

            var arrSortbyName = data.sort((a, b) => a.TenDonVi.localeCompare(b.TenDonVi, undefined, { caseFirst: "upper" }));
            self.ChiNhanhs(arrSortbyName);

            var obj = {
                ID: _idDonVi,
                TenDonVi: $('#_txtTenDonVi').html()
            }
            self.ChiNhanhChosed.push(obj);

            AddCheckAfterLi(_idDonVi);
            self.ArrDonVi(self.ChiNhanhs());
            self.SearchReport();
        });
    }

    function CheckQuyenExist(maquyen) {
        var role = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen === maquyen;
        });
        return role.length > 0;
    }

    function GetHT_Quyen_ByNguoiDung() {
        ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetHT_NhomNguoiDung?idnguoidung=" + _userID + '&iddonvi=' + _idDonVi, 'GET').done(function (data) {
            if (data !== null) {
                var allRole = data.HT_Quyen_NhomDTO;
                self.Quyen_NguoiDung(allRole);

                self.Show_BtnUpdateSoQuy(CheckQuyenExist('SoQuy_CapNhat'));
                self.Show_BtnDeleteSoQuy(CheckQuyenExist('SoQuy_Xoa'));
                self.Allow_ChangeTimeSoQuy(CheckQuyenExist('SoQuy_ThayDoiThoiGian'));

                SearchReport();
            }
            else {
                ShowMessage_Danger('Không có quyền xem báo cáo');
            }
        });
    }

    self.selectedCN = function (item) {
        event.stopPropagation();
        var sTenChiNhanhs = '';
        var arrIDSearch = [];
        var arrDV = [];
        if (item.ID === undefined) {
            arrIDSearch = $.map(self.ChiNhanhs(), function (x) {
                return x.ID;
            });
            arrDV = self.ArrDonVi().map(function (x) {
                return x.ID;
            });
            // push again lstDV has chosed
            for (let i = 0; i < self.ChiNhanhChosed().length; i++) {
                if ($.inArray(self.ChiNhanhChosed()[i].ID, arrDV) === -1 && self.ChiNhanhChosed()[i].ID !== '00000000-0000-0000-0000-0000-000000000000') {
                    self.ArrDonVi().unshift(self.ChiNhanhChosed()[i]);
                }
            }
            self.ChiNhanhChosed([{
                ID: '00000000-0000-0000-0000-0000-000000000000', TenDonVi: 'Tất cả chi nhánh'
            }]);
            sTenChiNhanhs = 'Tất cả chi nhánh';
        }
        else {
           
            for (var i = 0; i < self.ChiNhanhChosed().length; i++) {
                sTenChiNhanhs += self.ChiNhanhChosed()[i].TenDonVi + ',';
                if ($.inArray(self.ChiNhanhChosed()[i].ID, arrDV) === -1) {
                    arrDV.push(self.ChiNhanhChosed()[i].ID);
                }
                if (self.ChiNhanhChosed()[i].ID === '00000000-0000-0000-0000-0000-000000000000') {
                    self.ChiNhanhChosed().splice(i, 1);
                }
            }
            if ($.inArray(item.ID, arrDV) === -1) {
                self.ChiNhanhChosed.push(item);
                sTenChiNhanhs += item.TenDonVi + ', '; // used to bind at head Report
            }
            sTenChiNhanhs = Remove_LastComma(sTenChiNhanhs);
            arrIDSearch = $.map(self.ChiNhanhChosed(), function (x) {
                return x.ID;
            });
        }
        // remove donvi has chosed in lst
        var arr = $.grep(self.ArrDonVi(), function (x) {
            return x.ID !== item.ID;
        });
        self.ArrDonVi(arr);
        self.LstIDDonVi(arrIDSearch);
        self.TenChiNhanhs(sTenChiNhanhs);
        event.preventDefault();
        return false;
    }

    self.CloseDV = function (item) {
        self.ChiNhanhChosed.remove(item);
        var arrID = [];
        var sTenChiNhanhs = '';
        if (item.ID === '00000000-0000-0000-0000-0000-000000000000') {
            arrID = $.map(self.ChiNhanhs(), function (x) {
                return x.ID;
            });
            sTenChiNhanhs = 'Tất cả chi nhánh';
        }
        else {
            self.ArrDonVi.unshift(item);

            if (self.ChiNhanhChosed().length === 0) {
                arrID = $.map(self.ChiNhanhs(), function (x) {
                    return x.ID;
                });
                sTenChiNhanhs = 'Tất cả chi nhánh';
            }
            else {
                for (var i = 0; i < self.ChiNhanhChosed().length; i++) {
                    sTenChiNhanhs += self.ChiNhanhChosed()[i].TenDonVi + ', ';
                }
                sTenChiNhanhs = Remove_LastComma(sTenChiNhanhs);
                arrID = $.map(self.ChiNhanhChosed(), function (x) {
                    return x.ID;
                });
            }
        }
        self.TenChiNhanhs(sTenChiNhanhs);
        self.LstIDDonVi(arrID);
        SearchReport();
    }

    $('.choose_txtTime li').on('click', function () {
        $('.ip_TimeReport').val($(this).text());
        self.LiInput_Time($(this).val());
        SearchReport();
    });

    $('.ip_DateReport ').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        SearchReport();
    });

    $('#txtSearch').keypress(function (e) {
        var keyPress = e.keyCode || e.which;
        if (keyPress === 13) {
            self.CurrentPage(0);
            SearchReport();
        }
    })

    self.ClickBtnSearch = function () {
        self.CurrentPage(0);
        SearchReport();
    }

    self.RdoTypeTime.subscribe(function (newVal) {
        SearchReport();
    });

    self.Status.subscribe(function (newVal) {
        SearchReport();
    });

    $('.chose_kieubang').on('click', 'li', function () {

        $(".chose_kieubang li").each(function (i) {
            $(this).find('a').removeClass('box-tab');
        });
        $(this).find('a').addClass('box-tab');

        switch (parseInt($(this).val())) {
            case 1:
                $('.showChiNhanh').hide();
                $('#sodu').addClass('active');
                $('#nhatkysd').removeClass('active');
                $('#nhatkysddichvu').removeClass('active');
                $('#txtSearch').attr('placeholder', 'Nhập mã, tên khách hàng');
                $('#hdReport').text('Báo cáo số dư thẻ giá trị');
                Key_Form = 'Key_RpValueCardBalance';
                LoadCheckBox(5);
                break;
            case 2:
                $('.showChiNhanh').show();
                $('#nhatkysd').addClass('active');
                $('#sodu').removeClass('active');
                $('#nhatkysddichvu').removeClass('active');
                $('#txtSearch').attr('placeholder', 'Nhập mã, tên khách hàng');
                $('#hdReport').text('Báo cáo nhật ký thẻ giá trị');
                Key_Form = 'Key_RpValueCardHisUsed';
                LoadCheckBox(6);
                break;
            case 3:
                $('.showChiNhanh').show();
                $('#nhatkysddichvu').addClass('active');
                $('#sodu').removeClass('active');
                $('#nhatkysd').removeClass('active');
                $('#txtSearch').attr('placeholder', 'Nhập mã, tên khách hàng, mã, tên hàng hóa');
                $('#hdReport').text('Báo cáo nhật ký sử dụng dịch vụ thẻ giá trị');
                Key_Form = 'Key_RpValueCardServiceUsed';
                LoadCheckBox(13);
                break;
        }
        self.TypeReport($(this).val());
        self.CurrentPage(0);
        SearchReport();
    });

    function HideShow_btnExportExcel(maQuyen) {
        var role = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen === maQuyen;
        });
        if (role.length > 0) {
            $('#btnExport').show();
        }
        else {
            $('#btnExport').hide();
        }
    }

    self.SearchReport = function () {
        SearchReport(false);
    }

    async function SearchReport(isExport) {
        var valSearch = $('#txtSearch').val();
        var typeReport = parseInt(self.TypeReport());

        //  check role & hide/show btnExport
        var roleView = 'BCSoDuThe_XemDS';
        var roleExport = 'BCSoDuThe_XuatFile';
        if (typeReport === 2) {
            roleView = 'BCNhatKySuDungThe_XemDS';
            roleExport = 'BCNhatKySuDungThe_XuatFile';
        }
        var itemRole = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen === roleView;
        });
        if (itemRole.length === 0) {
            $('.PhanQuyen').show();
            $('#btnExport').hide();
            return false;
        }
        else {
            $('.PhanQuyen').hide();
            itemRole = $.grep(self.Quyen_NguoiDung(), function (x) {
                return x.MaQuyen === roleExport;
            });
            if (itemRole.length === 0) {
                $('#btnExport').hide();
            }
            else {
                $('#btnExport').show();
            }
        }

        var idChiNhanhs = '';
        for (let i = 0; i < self.ChiNhanhChosed().length; i++) {
            idChiNhanhs += self.ChiNhanhChosed()[i].ID + ',';
        }
        // avoid error in Store procedure
        if (idChiNhanhs === '') {
            idChiNhanhs = _idDonVi;
        }
        idChiNhanhs = Remove_LastComma(idChiNhanhs);

        // NgayLapHoaDon
        var _now = new Date();  //current date of week
        var currentWeekDay = _now.getDay();
        var lessDays = currentWeekDay === 0 ? 6 : currentWeekDay - 1;
        var dayStart = '';
        var dayEnd = '';
        var dateChose = '';

        if (self.RdoTypeTime() === '1') {
            var dateFrom = '';
            var dateTo = '';

            switch (self.LiInput_Time()) {
                case 0:
                    // all
                    self.TodayBC('Toàn thời gian');
                    dayStart = '20160101';
                    dayEnd = moment(_now).add('days', 1).format('YYYY-MM-DD');
                    break;
                case 1:
                    // hom nay
                    self.TodayBC('Hôm nay');
                    dayStart = dayEnd = moment(_now).format('YYYY-MM-DD');
                    self.TodayBC('Ngày: '.concat(moment(dayStart, 'YYYY-MM-DD').format('DD/MM/YYYY')));
                    break;
                case 2:
                    // hom qua
                    self.TodayBC('Hôm qua');
                    dayStart = dayEnd = moment(_now).subtract('days', 1).format('YYYY-MM-DD');
                    self.TodayBC('Ngày: '.concat(moment(dayStart, 'YYYY-MM-DD').format('DD/MM/YYYY')));
                    break;
                case 3:
                    // tuan nay
                    self.TodayBC('Tuần này');
                    dayStart = moment().startOf('week').add('days', 1).format('YYYY-MM-DD');
                    dayEnd = moment().endOf('week').add('days', 1).format('YYYY-MM-DD');

                    dateFrom = moment(dayStart, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    dateTo = moment(dayEnd, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    self.TodayBC('Từ ' + dateFrom + ' Đến ' + dateTo);
                    break;
                case 4:
                    // tuan truoc
                    self.TodayBC('Tuần trước');
                    dayStart = moment().weekday(-6).format('YYYY-MM-DD');
                    dayEnd = moment(dayStart, 'YYYY-MM-DD').add(6, 'days').format('YYYY-MM-DD'); // add day in moment.js

                    dateFrom = moment(dayStart, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    dateTo = moment(dayEnd, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    self.TodayBC('Từ ' + dateFrom + ' Đến ' + dateTo);
                    break;
                case 5:
                    // 7 ngay qua
                    self.TodayBC('7 ngày qua');
                    dayEnd = moment(_now).format('YYYY-MM-DD');
                    dayStart = moment(_now).subtract('days', 6).format('YYYY-MM-DD');

                    dateFrom = moment(dayStart, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    dateTo = moment(dayEnd, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    self.TodayBC('Từ ' + dateFrom + ' Đến ' + dateTo);
                    break;
                case 6:
                    // thang nay
                    self.TodayBC('Tháng này');
                    dayStart = moment().startOf('month').format('YYYY-MM-DD');
                    dayEnd = moment().endOf('month').format('YYYY-MM-DD'); // add them 1 ngày 01-month-year --> compare in SQL

                    dateFrom = moment(dayStart, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    dateTo = moment(dayEnd, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    self.TodayBC('Từ ' + dateFrom + ' Đến ' + dateTo);
                    break;
                case 7:
                    // thang truoc
                    self.TodayBC('Tháng trước');
                    dayStart = moment().subtract('months', 1).startOf('month').format('YYYY-MM-DD');
                    dayEnd = moment().subtract('months', 1).endOf('month').format('YYYY-MM-DD');

                    dateFrom = moment(dayStart, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    dateTo = moment(dayEnd, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    self.TodayBC('Từ ' + dateFrom + ' Đến ' + dateTo);
                    break;
                case 10:
                    // quy nay
                    self.TodayBC('Quý này');
                    dayStart = moment().startOf('quarter').format('YYYY-MM-DD');
                    dayEnd = moment().endOf('quarter').format('YYYY-MM-DD');

                    dateFrom = moment(dayStart, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    dateTo = moment(dayEnd, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    self.TodayBC('Từ ' + dateFrom + ' Đến ' + dateTo);
                    break;
                case 11:
                    // quy truoc = currQuarter -1; // if (currQuarter -1 == 0) --> (assign = 1)
                    self.TodayBC('Quý trước');
                    var prevQuarter = moment().quarter() - 1;
                    if (prevQuarter === 0) {
                        // get quy 4 cua nam truoc 01/10/... -->  31/21/...
                        var prevYear1 = moment().year() - 1;
                        dayStart = prevYear1 + '' + '1001';
                        dayEnd = prevYear1 + '' + '1231';
                    }
                    else {
                        dayStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                        dayEnd = moment().quarter(prevQuarter).endOf('quarter').format('YYYY-MM-DD');
                    }

                    dateFrom = moment(dayStart, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    dateTo = moment(dayEnd, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    self.TodayBC('Từ ' + dateFrom + ' Đến ' + dateTo);
                    break;
                case 12:
                    // nam nay
                    self.TodayBC('Năm này');
                    dayStart = moment().startOf('year').format('YYYY-MM-DD');
                    dayEnd = moment().endOf('year').format('YYYY-MM-DD');

                    dateFrom = moment(dayStart, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    dateTo = moment(dayEnd, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    self.TodayBC('Từ ' + dateFrom + ' Đến ' + dateTo);
                    break;
                case 13:
                    // nam truoc
                    self.TodayBC('Năm trước');
                    var prevYear = moment().year() - 1;
                    dayStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                    dayEnd = moment().year(prevYear).endOf('year').format('YYYY-MM-DD');

                    dateFrom = moment(dayStart, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    dateTo = moment(dayEnd, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    self.TodayBC('Từ ' + dateFrom + ' Đến ' + dateTo);
                    break;
            }
        }
        else {
            var arrDate = $('.ip_DateReport').val().split('-');
            dayStart = moment(arrDate[0], 'DD/MM/YYYY').format('YYYY-MM-DD');
            dayEnd = moment(arrDate[1], 'DD/MM/YYYY').format('YYYY-MM-DD');

            self.TodayBC('Từ ' + moment(arrDate[0], 'DD/MM/YYYY').format('DD/MM/YYYY') + ' đến ' + moment(arrDate[1], 'DD/MM/YYYY').format('DD/MM/YYYY'));
        }

        var ParamReportValueCard = {
            TextSearch: valSearch,
            ID_ChiNhanhs: idChiNhanhs,
            DateFrom: dayStart,
            DateTo: dayEnd,
            Status: self.Status(),
            TextReport: self.TenChiNhanhs(),
            CurrentPage: self.CurrentPage(),
            PageSize: self.CurrentPageSize(),
            ColumnsHide: '',
            LstIDChiNhanh: self.LstIDDonVi(),
        }
        console.log(2, ParamReportValueCard);

        $('.table-reponsive').gridLoader();

        if (isExport) {
            let columnHide = '';
            // get list columhide
            var cacheHideColumn2 = localStorage.getItem(Key_Form);
            if (cacheHideColumn2 !== null) {
                cacheHideColumn2 = JSON.parse(cacheHideColumn2);

                var arrColumn = [];
                for (var i = 0; i < cacheHideColumn2.length; i++) {
                    var itemFor = cacheHideColumn2[i];

                    // find in list checkbox
                    for (var j = 0; j < self.ListCheckBox().length; j++) {
                        if (self.ListCheckBox()[j].Key === itemFor.Value && $.inArray(itemFor.Value, arrColumn) === -1) {
                            arrColumn.push(itemFor.Value);
                            columnHide += j + '_';
                            break;
                        }
                    }
                }
                ParamReportValueCard.ColumnsHide = columnHide;
                console.log('columnHide', columnHide);
            }

            var funcNameExcel = 'ExportExcel_ValueCard_Balance';
            var noidungNhatKy = "Xuất excel báo cáo ";
            let fileNameExport = 'BaoCaoSoDuTheGiaTri.xlsx';

            switch (typeReport) {
                case 1:
                    noidungNhatKy = noidungNhatKy + "số dư thẻ giá trị";
                    break;
                case 2:
                    funcNameExcel = 'ExportExcel_ValueCard_HisUsed';
                    noidungNhatKy = noidungNhatKy + "nhật ký sử dụng thẻ giá trị";
                    fileNameExport = 'BaoCaoNhatKyTheGiaTri.xlsx';
                    break;
                case 3:
                    funcNameExcel = 'ExportExcel_ValueCard_ServiceUsed';
                    noidungNhatKy = noidungNhatKy + "nhật ký sử dụng dịch vụ thẻ giá trị";
                    fileNameExport = 'BCNhatKySuDungDichVuTheGiaTri.xlsx';
                    break;
            }
            ParamReportValueCard.PageSize = self.TotalRow(); // export all row

            const exportOK = await commonStatisJs.NPOI_ExportExcel(ReportUri + funcNameExcel, 'POST', ParamReportValueCard, fileNameExport);
            $('.table-reponsive').gridLoader({ show: false });
            if (exportOK) {
                var noidungChiTiet = noidungNhatKy.concat('. Người xuất:  ', _userLogin, '. Chi nhánh: ', self.TenChiNhanhs(),
                    '. Thời gian: ', moment(dayStart, 'YYYY-MM-DD').format('DD/MM/YYYY'), ' - ', moment(dayEnd, 'YYYY-MM-DD').format('DD/MM/YYYY'));

                var objDiary = {
                    ID_NhanVien: _idNhanVien,
                    ID_DonVi: _idDonVi,
                    ChucNang: $('#hdReport').text(),
                    NoiDung: noidungNhatKy,
                    NoiDungChiTiet: noidungChiTiet,
                    LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
                };
                Insert_NhatKyThaoTac_1Param(objDiary);
            }
            

           
        }
        else {
            var funcName = 'ReportBalance_ValueCard';
            switch (typeReport) {
                case 2:
                    funcName = 'ReportDiary_ValueCard';
                    break;
                case 3:
                    funcName = 'ReportServiceUsed_ValueCard';
                    break;
            }

            ajaxHelper(ReportUri + funcName, 'POST', ParamReportValueCard).done(function (obj) {
                $('.table-reponsive').gridLoader({ show: false });
                if (obj.res === true) {
                    console.log(obj);
                    var arrGird = obj.data;
                    switch (typeReport) {
                        case 1:
                            self.ReportBalanceCard(obj.data);
                            // load list check & hide colum after finish load list
                            LoadCheckBox(5);
                            self.TongDuCuoi(obj.TongDuCuoi);
                            break;
                        case 2:
                            self.ReportDiaryCard(obj.data);
                            LoadCheckBox(6);
                            self.TongDuCuoi(obj.TongDuCuoi);
                            break;
                        case 3:
                            self.ReportServiceUsed(obj.data);
                            LoadCheckBox(13);
                            break;
                    }
                    self.TotalRow(obj.TotalRow);
                    self.TotalPage(obj.TotalPage);
                    self.TongPhatSinhTang(obj.TongTang);
                    self.TongPhatSinhGiam(obj.TongGiam);

                    Caculator_FromToPaging(arrGird);
                    if (obj.TotalRow === 0) {
                        $('.Report_Empty').show();
                    }
                    else {
                        $('.Report_Empty').hide();
                    }
                }
                else {
                    console.log(obj.mes);
                }
            })
        }
    }

    Page_Load();

    self.DownloadFileTeamplateXLSX = function (pathFile) {
        var url = "/api/DanhMuc/DM_HangHoaAPI/Download_fileExcel?fileSave=" + pathFile;
        window.location.href = url;
    }

    self.ExportExcel = function () {
        SearchReport(true);
    }

    // bao cao nhat ky su dung the
    function ResetInforSearch() {
        self.CurrentPage(0);
        //self.CurrentPageSize(self.PageSizes()[0]);
        self.Status('1');
        self.RdoTypeTime('1');
        self.LiInput_Time(6);
        self.ChiNhanhChosed([{ ID: _idDonVi, TenDonVi: _tenDonVi }]);
        self.TenChiNhanhs(_tenDonVi);
        self.TodayBC('Tháng này');

        // remove all check
        $('#selec-all-DonVi li .fa-check').remove();
        // addd again check with id_DonVi
        AddCheckAfterLi(_idDonVi);
        $('.ip_TimeReport').val('Tháng này');
        $('#txtSearch').val('');
    }

    self.ChangePageSize = function (item) {
        self.CurrentPage(0);
        SearchReport();
    };

    function Caculator_FromToPaging(arrGird) {
        self.FromItem((self.CurrentPage() * self.CurrentPageSize()) + 1);

        if (((self.CurrentPage() + 1) * self.CurrentPageSize()) > arrGird.length) {
            var fromItem = (self.CurrentPage() + 1) * self.CurrentPageSize();
            if (fromItem < self.TotalRow()) {
                self.ToItem((self.CurrentPage() + 1) * self.CurrentPageSize());
            }
            else {
                self.ToItem(self.TotalRow());
            }
        } else {
            self.ToItem((self.CurrentPage() * self.CurrentPageSize()) + self.CurrentPageSize());
        }
    }

    // paging
    self.PageListPaging = ko.computed(function () {
        var arrPage = [];

        var allPage = self.TotalPage();
        var currentPage = self.CurrentPage();

        if (allPage > 4) {

            var i = 0;
            if (currentPage === 0) {
                i = parseInt(self.CurrentPage()) + 1;
            }
            else {
                i = self.CurrentPage();
            }

            if (allPage >= 5 && currentPage > allPage - 5) {
                if (currentPage >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (var k = allPage - 5; k < allPage; k++) {
                        var obj1 = {
                            pageNumber: k + 1,
                        };
                        arrPage.push(obj1);
                    }
                }
                else {
                    if (currentPage === 1) {
                        for (let j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            var obj2 = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj2);
                        }
                    }
                    else {
                        // get currentPage - 2 , currentPage, currentPage + 2
                        for (let j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                            var obj3 = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj3);
                        }
                    }
                }
            }
            else {
                // get 5 trang dau
                if (i >= 2) {
                    while (arrPage.length < 5) {
                        var obj4 = {
                            pageNumber: i - 1,
                        };
                        arrPage.push(obj4);
                        i = i + 1;
                    }
                }
                else {
                    while (arrPage.length < 5) {
                        var obj5 = {
                            pageNumber: i,
                        };
                        arrPage.push(obj5);
                        i = i + 1;
                    }
                }
            }
        }
        else {
            // neu chi co 1 trang --> khong hien thi DS trang
            if (allPage > 1) {
                for (var m = 0; m < allPage; m++) {
                    var obj = {
                        pageNumber: m + 1,
                    };
                    arrPage.push(obj);
                }
            }
        }
        return arrPage;
    });

    self.VisibleStartPage = ko.computed(function () {
        if (self.PageListPaging().length > 0) {
            return self.PageListPaging()[0].pageNumber !== 1;
        }
    });

    self.VisibleEndPage = ko.computed(function () {
        if (self.PageListPaging().length > 0) {
            return self.PageListPaging()[self.PageListPaging().length - 1].pageNumber !== self.TotalPage();
        }
    })

    self.ResetCurrentPage = function () {
        self.CurrentPage(0);
        SearchReport();
    };

    self.GoToPage = function (page) {
        self.CurrentPage(page.pageNumber - 1);
        SearchReport();
    };

    self.GetClass = function (page) {
        return ((page.pageNumber - 1) === self.CurrentPage()) ? "click" : "";
    };

    self.StartPage = function () {
        self.CurrentPage(0);
        SearchReport();
    }

    self.BackPage = function () {
        if (self.CurrentPage() > 1) {
            self.CurrentPage(self.CurrentPage() - 1);
            SearchReport();
        }
    }

    self.GoToNextPage = function () {
        if (self.CurrentPage() < self.TotalPage() - 1) {
            self.CurrentPage(self.CurrentPage() + 1);
            SearchReport();
        }
    }

    self.EndPage = function () {
        if (self.CurrentPage() < self.TotalPage() - 1) {
            self.CurrentPage(self.TotalPage() - 1);
            SearchReport();
        }
    }

    self.GotoKhachHang = function (item) {
        localStorage.setItem('FindKhachHang', item.MaDoiTuong);
        var url = "/#/Customers";
        window.open(url);
    }

    self.GotoHangHoa = function (item) {
        localStorage.setItem('loadMaHang', item.MaHangHoa);
        var url = "/#/Product";
        window.open(url);
    }

    $('#select-column').on('change', 'ul li input[type = checkbox]', function () {
        var valueCheck = $(this).val();
        // valueCheck (1) = class name, valueCheck(2) = value  --> pass to func 
        // add/remove class is hidding in list cache {NameClass, Value}
        LocalCaches.AddColumnHidenGrid(Key_Form, valueCheck, valueCheck);
        $('.' + valueCheck).toggle();
    });
    $('#select-column').on('click', 'ul li', function (i) {
        if ($(this).find('input[type = checkbox]').is(':checked')) {
            $(this).find('input[type = checkbox]').prop("checked", false);
        }
        else {
            $(this).find('input[type = checkbox]').prop("checked", true);
        }
        var valueCheck = $(this).find('input[type = checkbox]').val();
        LocalCaches.AddColumnHidenGrid(Key_Form, valueCheck, valueCheck);
        $('.' + valueCheck).toggle();
    });

    self.Modal_HoaDons = ko.observableArray();
    self.Modal_SoQuy = ko.observableArray();
    self.MaHoaDon_MoPhieu = ko.observable();
    self.LoaiHoaDon_MoPhieu = ko.observable();
    self.TongSLHang = ko.observable();

    self.ShowPopup_InforHD = function (item) {
        var maHoaDon = Remove_LastComma(item.MaHoaDon);

        self.MaHoaDon_MoPhieu(maHoaDon);

        ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'Get_InforHoaDon_byMaHoaDon?maHoaDon=' + maHoaDon, 'GET').done(function (data) {
            if (data !== null) {
                self.Modal_HoaDons(data);
                self.LoaiHoaDon_MoPhieu(data.LoaiHoaDon);

                var slHang = data.BH_HoaDon_ChiTiet.reduce(function (_this, val) {
                    return _this + val.SoLuong;
                }, 0);

                self.TongSLHang(slHang);

                $('#modalpopup_PhieuBH').modal('show');
            }
        })
    }

    self.ShowPopup_InforHD_PhieuThu = function (item) {

        self.MaHoaDon_MoPhieu(item.MaHoaDon);

        switch (item.LoaiHoaDonSQ) {
            case 11:
            case 12:
                Get_InforSoQuy_HoaDonLienQuan_ByID(item.ID);
                break;
            default:
                self.ShowPopup_InforHD(item);
                break;
        }
    }

    self.ShowPopup_PhieuThuChi = function (item) {
        Get_InforSoQuy_HoaDonLienQuan_ByID(item.ID_PhieuThuChi);
    }

    function Get_InforSoQuy_HoaDonLienQuan_ByID(id) {
        ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/' + 'Get_InforSoQuy_HoaDonLienQuan_ByID?id=' + id, 'GET').done(function (data) {
            if (data !== null) {
                data.NgayLapHoaDon = moment(data.NgayLapHoaDon).format('DD/MM/YYYY HH:mm');
                self.LoaiHoaDon_MoPhieu(data.LoaiHoaDon);// used to check when MoPhieu

                self.Modal_SoQuy(data);

                // set text after bind data
                $('#ttpt_KhachHang').text('Nhà cung cấp');

                if (data.LoaiHoaDon === 11) {
                    $('#ttpt_title').text('Phiếu thu');
                    $('#ttpt_hdDaThu').text('Đã thu');
                    $('#ttpt_hdCanThu').text('Còn cần thu');
                }
                else {
                    $('#ttpt_title').text('Phiếu chi');
                    $('#ttpt_hdDaThu').text('Đã chi');
                    $('#ttpt_hdCanThu').text('Còn cần chi');
                }
                $('#modalpopup_SoQuy .modal-footer div:gt(0)').css('display', 'none');
                $('#modalpopup_SoQuy').modal('show');
            }
        })
    }

    self.ClickMoPhieu = function () {
        localStorage.setItem('FindHD', self.MaHoaDon_MoPhieu());

        var url = '';
        console.log('self.LoaiHoaDon_MoPhieu() ', self.LoaiHoaDon_MoPhieu())

        switch (self.LoaiHoaDon_MoPhieu()) {
            case 1:
                url = "/#/Invoices";
                break;
            case 3:
                url = "/#/Order";
                break;
            case 4:
                url = "/#/PurchaseOrder";
                break;
            case 6:
                url = "/#/Returns";
                break;
            case 7:
                url = "/#/PurchaseReturns";
                break;
            case 8:
                url = "/#/DamageItems";
                break;
            case 9:
                url = "/#/StockTakes";
                break;
            case 10:
                url = "/#/Transfers";
                break;
            case 11:
            case 12:
                localStorage.removeItem('FindHD');
                localStorage.setItem('FindMaPhieuChi', self.MaHoaDon_MoPhieu());
                url = '/#/CashFlow';
                break;
            case 19:
                url = "/#/ServicePackage";
                break;
        }
        if (url !== '') {
            window.open(url);
        }
    }
}
var reportValueCard = new ViewReportValueCard();
ko.applyBindings(reportValueCard);
$('#selec-all-DonVi').parent().on('hide.bs.dropdown', function () {
    reportValueCard.SearchReport();
});