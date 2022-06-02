
var vmCaLamViec = new Vue({
    el: '#CaLamViec',
    data: {
        databind: {
            pageview: '',
            pagenow: 1,
            data: [],
            listpage: [],
            isprev: false,
            isnext: false,
            countpage: 0,
        },
        curentpage: {
            text: '',
            tungay: '',
            denngay: '',
            typetime: 9,
            typetimeold: 9,
            trangthai: [1],
            pagesize: 10,
            order: null,
            sort: false,
            pagenow: 1,
            idChiNhanh: $('#hd_IDdDonVi').val(),
            textdate: 'Năm này',
            listchinhanh:[]
        },
        fileimport: {
            filetext: '',
            exitfile: false,
            file: null,
            textmess: '',
            isSave:false
        },
        Key_Form: "KeyFormCaLamViec",
        listpagesize: [10, 20, 30],
        listcolumn: [],
        listchinhanhold: [],
        ClickFilter: {
            ChiNhanh: true,
            ThoiGian: true,
            TrangThai: true
        },
        role: {
            NhanSu: false,
        },
    },
    methods: {
        // Lấy dữ liệu mặc định
        GetListData: function () {
            var self = this;

            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/CheckKhoiTaoDuLieuChamCong", function (data) {
                if (data.res) {
                    if (!data.dataSoure) {
                        $('#modalpopup_KhoiTaoDuLieuChamCong').modal('show');
                    }
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });
            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetRoleNhanSu", function (data) {
                if (data.res) {
                    self.role.NhanSu = data.dataSoure;
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });
        },
        GetForSearchCalamviec: function (resetpage = false) {
            var self = this;
            $('#table-reponsive').gridLoader();
            if (resetpage) {
                self.curentpage.pagenow = 1;
            }
            var arrDV = self.curentpage.listchinhanh.filter(x => x.Checked).map(x => x.Id);
            if (arrDV.length === 0) {
                arrDV = [self.curentpage.idChiNhanh];
            }
            var model = {
                Text:self.curentpage.text,
                TuNgay: self.curentpage.tungay,
                DenNgay: self.curentpage.denngay,
                TypeTime: self.curentpage.typetime,
                pageSize: self.curentpage.pagesize,
                pageNow: self.curentpage.pagenow,
                TrangThai: self.curentpage.trangthai,
                Order: self.curentpage.order,
                Sort: self.curentpage.sort,
                IDNhanVien: $('.idnhanvien').text(),
                ListDonVi: arrDV,
            }
            console.log('GetForSearchCalamviec', model);
            $.ajax({
                data: model,
                url: "/api/DanhMuc/NS_NhanSuAPI/GetForSearchCalamviec",
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    $('#table-reponsive').gridLoader({ show: false });
                    if (data.res === true) {
                        self.databind = data.dataSoure;
                        self.curentpage.pagenow = self.databind.pagenow;
                        commonStatisJs.sleep(100).then(() => {
                            self.LoadFirst();
                        });
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(data.mess);
                    }
                },
                error: function (result) {
                    $('#table-reponsive').gridLoader({ show: false });
                    console.log(result);
                }
            });
        },
        SelectPage: function (item) {
            this.curentpage.pagenow = item;
            this.GetForSearchCalamviec();
        },
        ButtonSelectPage: function (item, isfirstpage = null) {
            if (isfirstpage === null) {
                this.curentpage.pagenow += item;
                if (this.curentpage.pagenow <= 0) {
                    this.curentpage.pagenow = 1;
                }
                else if (this.curentpage.pagenow <= 0) {
                    this.curentpage.pagenow = 1;
                }
            }
            else if (isfirstpage === true) {
                this.curentpage.pagenow = 1;
            }
            else {
                this.curentpage.pagenow = this.databind.countpage;
            }
            this.GetForSearchCalamviec();
        },
        SelectRow: function (event) {
            var heigth = 0;
            var heightold = 0;
            var setTop = 0;
            var $this = $(event.target).closest('tr');
            var css = $this.next(".op-tr-hide").css('display');
            $(".op-tr-hide").hide();
            $(".op-tr-show").removeClass('tr-active');
            setTop = 35 + parseInt($this.height() * ($this.index() / 2));
            if (css === 'none') {
                $this.addClass('tr-active');
                $this.next(".op-tr-hide").toggle();
                heightold = $this.next().height();
                heigth = parseInt($this.height()) + heightold;
                $('.line-right').height(heigth).css("margin-top", setTop + "px");
                $this.closest('tbody').closest('table').closest('.table-reponsive').removeClass('tablescroll');

            }
            else {
                $('.line-right').height(0).css("margin-top", "0px");
                if (!$this.closest('tbody').closest('table').closest('.table-reponsive').hasClass('tablescroll')) {
                    $this.closest('tbody').closest('table').closest('.table-reponsive').addClass('tablescroll');
                }
            }
        },
        AddTrangThai: function (key) {
            if (this.curentpage.trangthai.some(o => o === key)) {
                this.curentpage.trangthai = this.curentpage.trangthai.filter(o => o !== key);
            }
            else {
                this.curentpage.trangthai.push(key);
            }
            this.GetForSearchCalamviec(true);
        },
        keymonitor: function (event) {
            if (event.key === "Enter") {
                this.GetForSearchCalamviec(true);
            }
        },
        SelectPageSize: function (e) {
            this.GetForSearchCalamviec(true);
        },
        GetColumShowHie: function (e) {
            var self = this;
            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetColumnCaLamViec", function (data) {
                self.listcolumn = data.dataSoure;
            });
            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetListChiNhanhNhanVien?id=" + $('.idnhanvien').text(), function (data) {
                if (data.res) {
                    data.dataSoure.filter(x => x.Id == self.curentpage.idChiNhanh).map(x => x.Checked = true);
                    self.curentpage.listchinhanh = data.dataSoure;
                    self.listchinhanhold= commonStatisJs.CopyArray(data.dataSoure);
                }
                else {
                    console.log(data.mess);
                }
            });
        },
        LoadFirst: function () {
            var selft = this;
            var result = LocalCaches.LoadColumnGrid(selft.Key_Form);
            result.forEach(function (element) {
                $('.' + element.NameClass).hide();
                var model = selft.listcolumn.find(o => o.Key === element.NameClass);
                if (model !== null) {
                    model.Checked = false;
                }
            });
        },
        SelectColum: function (item, index, event) {
            var self = this;
            item.Checked = !item.Checked;
            LocalCaches.AddColumnHidenGrid(self.Key_Form, item.Key, index);
            $('.' + item.Key).toggle();
        },
        ChangeCheckedColum: function (item, index, event) {
            var self = this;
            LocalCaches.AddColumnHidenGrid(self.Key_Form, item.Key, index);
            $('.' + item.Key).toggle();
        },
        // Thêm sửa xóa 
        AddCaLamViec: function () {
            vmEditCaLamViec.listchinhanh = commonStatisJs.CopyArray(this.listchinhanhold);
            vmEditCaLamViec.AddNew();
        },

        UpdateCaLamViec: function (item) {
            vmEditCaLamViec.listchinhanh = commonStatisJs.CopyArray(this.listchinhanhold);
            vmEditCaLamViec.Edit(commonStatisJs.CopyObject(item));
        },

        CopyCaLamViec: function (item) {
            vmEditCaLamViec.listchinhanh = commonStatisJs.CopyArray(this.listchinhanhold);
            vmEditCaLamViec.Copy(commonStatisJs.CopyObject(item));
        },

        DeleteCaLamViec: function (item) {
            vmEditCaLamViec.Delete(item);
        },
        // export inport
        ExportCaLamViec: function () {
            $('#table-reponsive').gridLoader();
            var self = this;
            let arrDV = self.curentpage.listchinhanh.filter(x => x.Checked).map(x => x.Id);
            if (arrDV.length === 0) {
                arrDV = [self.curentpage.idChiNhanh];
            }
            var model = {
                Text: self.curentpage.text,
                TuNgay: self.curentpage.tungay,
                DenNgay: self.curentpage.denngay,
                TypeTime: self.curentpage.typetime,
                pageSize: self.curentpage.pagesize,
                pageNow: self.curentpage.pagenow,
                TrangThai: self.curentpage.trangthai,
                Order: self.curentpage.order,
                Sort: self.curentpage.sort,
                IDNhanVien: $('.idnhanvien').text(),
                ListDonVi: arrDV,
            }
            $.ajax({
                data: model,
                url: "/api/DanhMuc/NS_NhanSuAPI/ExportExcelToCaLamViec",
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    $('#table-reponsive').gridLoader({ show: false });
                    if (data.res === true) {
                        window.location.href = "/api/DanhMuc/NS_NhanSuAPI/DownloadFileExecl?fileSave=" + data.dataSoure;
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(data.mess);
                    }
                }, error: function (result) {
                    $('#table-reponsive').gridLoader({ show: false });
                    console.log(result);
                }
            });
        },

        ShowPopupImport: function () {
            this.removeImage();
            $('#ModalImport').modal('show');
        },

        DownloadFileTeamplateXLS: function () {
            var url = '/api/DanhMuc/DM_HangHoaAPI/' + "Download_TeamplateImport?fileSave=" + "CaLamViec_ChamCong/FileImport_ThongTinCaLamViec.xls";
            window.open(url);
        },
        DownloadFileTeamplateXLSX: function () {
            var url = '/api/DanhMuc/DM_HangHoaAPI/' + "Download_TeamplateImport?fileSave=" + "CaLamViec_ChamCong/FileImport_ThongTinCaLamViec.xlsx";
            window.open(url)
        },
        ShowColumn: function () {
            $(".dropdown-list").toggle();
        },
        ChangeSelectFile: function (e) {
            var files = e.target.files || e.dataTransfer.files;
            if (!files.length)
                return;
            this.createFile(files[0]);
        },
        onClick: function (event) {
            event.target.value = ''
        },
        OnClickChiNhanh: function (item) {
            item.Checked = !item.Checked;
            this.GetForSearchCalamviec(true);
        },
        createFile(file) {
            var self = this;
            self.fileimport.filetext = file.name;
            self.fileimport.file = file;
            this.fileimport.exitfile = true;
            this.fileimport.textmess = '';
        },
        removeImage: function () {
            this.fileimport = {
                filetext: '',
                exitfile: false,
                file: null,
                textmess: '',
                isSave: false
            };
        },
        SaveImport: function () {
            var self = this;
            var fileData = new FormData();
            fileData.append(this.fileimport.filetext, self.fileimport.file);
            self.fileimport.isSave = true;
            $.ajax({
                data: fileData,
                url: '/api/DanhMuc/NS_NhanSuAPI/ImportFileCaLamViec?ID_DonVi='+ $('#hd_IDdDonVi').val() + "&ID_NhanVien=" + $('.idnhanvien').text(),
                type: 'POST',
                processData: false,
                contentType: false,
                success: function (result) {
                    if (result.res === true) {
                        self.GetForSearchCalamviec(true);
                        self.removeImage();
                        if (result.dataSoure.succes) {
                            commonStatisJs.ShowMessageSuccess(result.dataSoure.mess);
                            $('#ModalImport').modal('hide');
                        }
                        else {
                            self.fileimport.textmess=result.dataSoure.mess;
                            window.location.href = "/api/DanhMuc/NS_NhanSuAPI/DownloadFileExecl?fileSave=" + result.dataSoure.file;
                        }
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(result.mess);
                        self.fileimport.isSave = false;
                    }
                },
                error: function (result) {
                    console.log(result);
                    self.fileimport.isSave = false;
                }
            });
        },
        SetupThamSoTinhCong: function () {
            var self = this;
            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/SetUpThamSoCong", function (data) {
                if (data.res) {
                    $('#modalpopup_KhoiTaoDuLieuChamCong').modal('hide');
                    commonStatisJs.ShowMessageSuccess(data.mess);

                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });
        }
    },
    computed: {
    },
});
$('body').on('AddCaLamViecSucces', function () {
    vmCaLamViec.GetForSearchCalamviec();
})
$('.newDateTime').on('apply.daterangepicker', function (ev, picker) {
    vmCaLamViec.curentpage.tungay = picker.startDate.format('MM/DD/YYYY');
    vmCaLamViec.curentpage.denngay = picker.endDate.format('MM/DD/YYYY');
    vmCaLamViec.GetForSearchCalamviec(true);
});
$('#SinhNhatSL ').on('click', '.radio-menu input[type="radio"]', function () {
    var dataid = $(this).data('id');
    $('#SinhNhatSL .form-group').each(function () {
        $(this).find('.conten-choose').find('input').removeAttr('disabled');
        if (dataid !== $(this).find('.radio-menu').find('input').data('id')) {
            $(this).find('.conten-choose').find('input').attr('disabled', 'disabled');
        }
    });
    if ($(this).data('id') !== 1) {
        vmCaLamViec.curentpage.typetime=null;
    }
    else {
        vmCaLamViec.curentpage.typetime = vmCaLamViec.curentpage.typetimeold;
    }
    vmCaLamViec.GetForSearchCalamviec(true);
});
$('#SelectNgaySinh').on('click', 'ul li', function () {
    vmCaLamViec.curentpage.textdate = $(this).find('a').text();
    vmCaLamViec.curentpage.typetime = $(this).val();
    vmCaLamViec.curentpage.typetimeold = $(this).val();
    vmCaLamViec.GetForSearchCalamviec(true);
});
//$("#mytable").colResizable({
//    liveDrag: true,
//    gripInnerHtml: "<div class='grip'></div>",
//    draggingClass: "dragging",
//    resizeMode: 'overflow',
//    hoverCursor: "col-resize",
//    dragCursor: "col-resize",
//});
$('.table-border').on('click', 'tr th', function () {
    $('.table-border tr th').each(function () {
        $(this).find('i').remove();
    })
    if (vmCaLamViec.curentpage.order === $(this).data("id")) {
        vmCaLamViec.curentpage.sort = !vmCaLamViec.curentpage.sort;
        if (vmCaLamViec.curentpage.sort === true) {
            $(this).html($(this).text() + '<i class="fa fa-caret-up" aria-hidden="true"></i>');
        }
        else {
            $(this).html($(this).text() + '<i class="fa fa-caret-down" aria-hidden="true"></i>');
        }
    }
    else {
        vmCaLamViec.curentpage.sort =true;
        $(this).html($(this).text() + '<i class="fa fa-caret-up" aria-hidden="true"></i>');
    }
    vmCaLamViec.curentpage.order = $(this).data("id");
    vmCaLamViec.GetForSearchCalamviec();
})
vmCaLamViec.GetColumShowHie();
vmCaLamViec.GetForSearchCalamviec();
vmCaLamViec.GetListData();