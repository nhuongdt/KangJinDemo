var delayTimer;
ko.components.register('jqauto-customer', {
    viewModel: function (params) {
        var self = this;
        self.textSearch = ko.observable();
        self.searchList = ko.observableArray();
        self.indexFocus = ko.observable(0);
        self.roleAdd = params.roleAdd;
        self.showDiary = params.showDiary;
        self.loaiDoiTuong = params.loaiDoiTuong;
        self.showDiv = params.showDiv;
        self.ChangeCustomer = params.changeCus;
        self.AddCustomer = params.addCustomer;

        self.hideList = function () {
            $('jqauto-customer .gara-search-dropbox').hide();
        };
        self.showList = function () {
            $('jqauto-customer .gara-search-dropbox').show();
        };
        self.showPopAdd = function () {
            self.AddCustomer();
        };
        self.search = function () {
            var keyCode = event.keyCode || event.which;
            clearTimeout(delayTimer);

            switch (keyCode) {
                case 38:
                    self.keyUp();
                    break;
                case 40:
                    self.keyDown();
                    break;
                default:
                    if (keyCode !== 13) {
                        self.indexFocus(0);
                    }
                    delayTimer = setTimeout(function () {
                        self.searchDB(keyCode);
                    }, 300);
                    break;
            }
        };
        self.searchDB = function (keyCode) {
            var self = this;
            var txt = locdau(self.textSearch()).trim();
            if (txt === '') {
                self.searchList([]);
                self.ChangeCustomer(
                    {
                        ID: null,
                        NguoiNopTien: '',
                        SoDienThoai: '',
                        DiaChi: '',
                        Email: '',
                    });
                return;
            }
            if (keyCode === 13 && self.searchList().length > 0) {
                self.keyEnter();
            }
            else {
                var idDonVi = $('#hd_IDdDonVi').val();
                if (idDonVi == undefined) {
                    idDonVi = newModelBanLe.HoaDons().ID_DonVi();
                }

                $.getJSON("/api/DanhMuc/DM_DoiTuongAPI/" + "JqAuto_SearchDoiTuong?loaiDoiTuong=" + self.loaiDoiTuong
                    + "&txtSearch=" + txt + '&idChiNhanh=' + idDonVi).done(function (data) {
                        switch (self.loaiDoiTuong) {
                            case 1:
                            case 3:
                                data = $.grep(data, function (x) {
                                    return x.ID.indexOf('00000000') === -1;
                                })
                                break;
                        }

                        self.searchList(data);
                        if (keyCode === 13) {
                            self.keyEnter();
                        }
                        if (data.length > 0) {
                            self.showList();
                        }
                        else {
                            self.hideList();
                        }
                    });
            }
        };
        self.keyEnter = function () {
            var itChose = $.grep(self.searchList(), function (x, index) {
                return index === self.indexFocus();
            });
            if (itChose.length > 0) {
                self.textSearch(itChose[0].NguoiNopTien)
                self.ChangeCustomer(itChose[0]);
                self.searchList([]);
            }
            $('jqauto-customer .gara-search-dropbox').hide();
        };
        self.keyUp = function () {
            var self = this;
            if (self.indexFocus() < 0) {
                self.indexFocus(0);
            }
            else {
                self.indexFocus(self.indexFocus() - 1);
            }
        };
        self.keyDown = function () {
            var self = this;
            if (self.indexFocus() > self.searchList().length) {
                self.indexFocus(0);
            }
            else {
                self.indexFocus(self.indexFocus() + 1);
            }
        };
        self.ChoseItem = function (item) {
            self.ChangeCustomer(item);
            self.textSearch(item.NguoiNopTien);
            self.searchList([]);
        }
        self.UpDownEnter = function (keyCode) {
            var self = this;
            switch (keyCode) {
                case 13:
                    self.keyEnter();
                    break;
                case 38:
                    self.keyUp();
                    break;
                case 40:
                    self.keyDown();
                    break;
            }
        };
    },
    template: `
    <div class="gara-bill-infor-button" 
            data-bind="visible: showDiv">
            <div>
                <a data-bind="visible: roleAdd, click: showPopAdd">
                    <i class="material-icons">add</i>
                </a>
            </div>
            <input class="gara-search-HH _jsInput" autocomplete="off" onclick="this.select()"
                data-bind="attr:{placeholder: loaiDoiTuong==3?'Tìm kiếm bảo hiểm':loaiDoiTuong== 2?'Tìm kiếm nhà cung cấp':'Tìm kiếm khách hàng'},
                style:{'border-bottom': loaiDoiTuong !==3?'1px solid #ccc':'none'},
                value: textSearch, valueUpdate: 'afterkeydown',  event:{keyup: search}"
            />
            <div class="gara-search-dropbox" >
                <ul data-bind="foreach: searchList">
                    <li data-bind="click:$parent.ChoseItem, 
                    style:{background: $parent.indexFocus() === $index()?'#d9d9d9':'none'}">
                        <div>
                            <span style="color:var(--color-main)" data-bind="text: MaNguoiNop"></span>
                        <span style="float:right" data-bind="text: ' SĐT: ' +  SoDienThoai, visible: SoDienThoai"></span> <br/>
                            <span data-bind="text: NguoiNopTien">
                            </span>
                        </div>
                    </li>
                </ul>
            </div>
        </div>
    `,
});

ko.components.register('jqauto-account-bank', {
    viewModel: function (params) {
        var self = this;
        self.indexFocus = ko.observable(0);
        self.searchList = ko.observableArray();
        self.textSearch = ko.observable(params.textSearch());
        self.TaiKhoanNganHang = params.TaiKhoanNganHang;
        self.TaiKhoanPos = params.TaiKhoanPos;
        self.idChose = ko.observable(params.idChose);
        self.Callback = params.callback;
        self.ResetNoChose = params.resetNoChose;

        self.idChose.subscribe(function () {
        });
        self.hideList = function () {
            $('jqauto-account-bank .gara-search-dropbox').hide();
        };
        self.showList = function () {
            $(event.currentTarget).next().show();
            if (commonStatisJs.CheckNull(self.textSearch())) {
                // show first
                var arrByLoai = $.grep(self.TaiKhoanNganHang(), function (x) {
                    return x.TaiKhoanPOS === self.TaiKhoanPos;
                });
                self.searchList(arrByLoai);
            }
        };

        self.search = function () {
            var self = this;
            var keyCode = event.keyCode;
            switch (keyCode) {
                case 13:
                    break;
                case 38:
                    break;
                case 40:
                    break;
                default:
                    var txt = locdau(self.textSearch()).trim();;
                    var arrByLoai = $.grep(self.TaiKhoanNganHang(), function (x) {
                        return x.TaiKhoanPOS === self.TaiKhoanPos;
                    });
                    if (txt === '') {
                        self.searchList(arrByLoai);
                        return;
                    }
                    var arr = $.grep(arrByLoai, function (x) {
                        return locdau(x.TextSearchAuto).indexOf(txt) > -1;
                    })
                    self.searchList(arr);
                    break;
            }
        };
        self.keyEnter = function (item) {
            self.textSearch(item.TenChuThe);
            //self.Callback(item);
            $('jqauto-account-bank .gara-search-dropbox').hide();
        };
        self.keyUp = function () {
            var self = this;
            if (self.indexFocus() < 0) {
                self.indexFocus(0);
            }
            else {
                self.indexFocus(self.indexFocus() - 1);
            }
        };
        self.keyDown = function () {
            var self = this;
            if (self.indexFocus() > self.searchList().length) {
                self.indexFocus(0);
            }
            else {
                self.indexFocus(self.indexFocus() + 1);
            }
        };
        self.ResetAccount = function () {
            self.textSearch('-- Chọn tài khoản --');
            self.idChose(null);
            self.ResetNoChose();
            self.hideList();
        };
        self.Cpn_ChoseAccount = function (item) {
            self.textSearch(item.TenChuThe);
            self.idChose(item.ID);
            self.Callback(item);
            self.hideList();
        };
    },
    template: `
     <div style="position:relative">
            <a style="
                position: absolute;
                top: 0;
                right: 05px;
                color: gray;
                " class="gara-button-icon">
            <i class="material-icons">visibility</i></a>

                <input type="text" class="form-control gara-search-HH"  onclick="this.select()" 
                            placeholder="Tìm kiếm"
                            data-bind="value: textSearch, click: showList,
                             event: {keyup: search}, valueUpdate: 'afterkeydown'">
                <div class="gara-search-dropbox" style="display: block;">
                        <ul>
                            <li data-bind="click: ResetAccount">
                             -- Chọn tài khoản --
                            </li>
                           <!-- ko foreach: searchList -->
                            <li data-bind="click: $parent.Cpn_ChoseAccount,
                            style:{background: $parent.indexFocus() === $index() + 1?'#d9d9d9':'none'}">
                                <div>
                                    <span style="color:var( --color-main)"
                            data-bind="text:TenChuThe"></span>

                                </div>
                                <div>
                                    <span style="color:red; font-weight:bold"
                                    data-bind="text:TenNganHang"></span>

                                </div>
                                <span class="check" data-bind="visible: $parent.idChose() == ID">
                                    <i class="fa fa-check pull-right my-fa-check" style="display:block"></i>
                                </span>
                            </li>
                            <!-- /ko -->
                        </ul>
                    </div>
     </div>
    `,
});

ko.components.register('jqauto-product', {
    viewModel: function (params) {
        var self = this;
        self.textSearch = ko.observable();
        self.searchList = ko.observableArray();
        self.indexFocus = ko.observable(0);
        self.currentPage = ko.observable(0);
        self.pageSize = ko.observable(30);
        self.roleAdd = params.roleAdd;
        self.idChiNhanh = params.idChiNhanh;
        self.formType = params.form == null || params.form === undefined ? 0 : params.form;
        self.roleXemGiaVon = params.roleXemGiaVon;
        self.loaiHangHoa = params.loaiHangHoa;
        self.ChangeHangHoa = params.choseItem;
        self.AddNewItem = params.addItem;

        self.hideList = function () {
            $('jqauto-product .gara-search-dropbox').hide();
        };
        self.showList = function () {
            $('jqauto-product .gara-search-dropbox').show();
        };
        self.showPopAdd = function () {
            self.AddNewItem();
        };
        self.search = function () {
            var keyCode = event.keyCode || event.which;
            clearTimeout(delayTimer);

            switch (keyCode) {
                case 38:
                    self.keyUp();
                    break;
                case 40:
                    self.keyDown();
                    break;
                default:
                    if (keyCode !== 13) {
                        self.indexFocus(0);
                    }
                    delayTimer = setTimeout(function () {
                        self.currentPage(0);
                        self.searchDB(keyCode);
                    }, 300);
                    break;
            }
        };
        self.searchDB = function (keyCode) {
            var self = this;
            var txt = locdau(self.textSearch()).trim();
            if (txt === '') {
                self.searchList([]);
                self.ChangeHangHoa(
                    {
                        ID: null,
                        MaHangHoa: '',
                        MaLoHang: '',
                        TenHangHoa: '',
                        DonGia: 0,
                        TonKho: 0,
                    });
                return;
            }
            console.log('currpage ', self.currentPage());
            if (keyCode === 13 && self.searchList().length > 0) {
                self.keyEnter();
            }
            else {
                let param = {
                    ID_ChiNhanh: self.idChiNhanh(),
                    ID_BangGia: '00000000-0000-0000-0000-000000000000',
                    TextSearch: txt,
                    LaHangHoa: self.loaiHangHoa,
                    QuanLyTheoLo: '%%',
                    Form: self.formType,
                    CurrentPage: self.currentPage(),
                    PageSize: 30,
                };
                console.log('self.formType ', self.formType, self.idChiNhanh())

                let urlAPI = '/api/DanhMuc/DM_HangHoaAPI/Gara_JqAutoHangHoa';
                if (formatNumberToFloat(self.formType) === 18 || formatNumberToFloat(self.formType) === 16) {// dieuchinh gv
                    param = {
                        IDChiNhanhs: [self.idChiNhanh()],
                        TextSearch: txt,
                        DateTo: moment(new Date()).add(1,'minutes').format('YYYY-MM-DD'),
                        CurrentPage: self.currentPage(),
                        PageSize: 30,
                    };
                    urlAPI = '/api/DanhMuc/BH_XuatHuyAPI/JqAutoHangHoa_withGiaVonTieuChuan';
                }
                else {
                    param.ConTonKho = modelTypeSearchProduct.ConTonKho();
                }

                $.ajax({
                    type: 'POST',
                    url: urlAPI,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: param ? JSON.stringify(param) : null,
                }).done(function (x) {
                    if (x.res) {
                        let data = x.dataSoure;

                        data = $.grep(data, function (x) {
                            return x.ID.indexOf('00000000') === -1;
                        })
                        if (self.currentPage() === 0) {
                            self.searchList(data);
                        }
                        else {
                            for (let i = 0; i < data.length; i++) {
                                self.searchList.push(data[i])
                            }
                        }

                        if (keyCode === 13) {
                            self.keyEnter();
                        }

                        if (self.searchList().length > 0) {
                            self.showList();
                        }
                        else {
                            self.hideList();
                        }
                    }
                    else {
                        self.hideList();
                    }
                });
            }
        };
        self.keyEnter = function () {
            let itChose = $.grep(self.searchList(), function (x, index) {
                return index === self.indexFocus();
            });
            if (itChose.length > 0) {
                if (itChose.QuanLyTheoLoHang) {
                    self.textSearch(itChose.MaHangHoa + ' LSX: ' + itChose.MaLoHang);
                }
                else {
                    self.textSearch(itChose.MaHangHoa);
                }
                self.ChangeHangHoa(itChose[0]);
                self.searchList([]);
            }
            $('jqauto-product .gara-search-dropbox').hide();
        };
        self.keyUp = function () {
            var self = this;
            if (self.indexFocus() < 0) {
                self.indexFocus(0);
            }
            else {
                self.indexFocus(self.indexFocus() - 1);
            }
        };
        self.keyDown = function () {
            var self = this;
            if (self.indexFocus() > self.searchList().length) {
                self.indexFocus(0);
            }
            else {
                self.indexFocus(self.indexFocus() + 1);
            }
        };
        self.ChoseItem = function (item) {
            self.ChangeHangHoa(item);
            if (item.QuanLyTheoLoHang) {
                self.textSearch(item.MaHangHoa + ' LSX: ' + item.MaLoHang);
            }
            else {
                self.textSearch(item.MaHangHoa);
            }
            self.searchList([]);
        }
        self.UpDownEnter = function (keyCode) {
            var self = this;
            switch (keyCode) {
                case 13:
                    self.keyEnter();
                    break;
                case 38:
                    self.keyUp();
                    break;
                case 40:
                    self.keyDown();
                    break;
            }
        };

        self.scrollList = function () {
            var elem = event.target;
            if (elem.scrollTop > (elem.scrollHeight - elem.offsetHeight - 200)) {
                if (elem.scrollTop + elem.clientHeight >= elem.scrollHeight) {
                    let lenData = self.searchList().length;
                    if (lenData === (self.currentPage() + 1) * self.pageSize()) {
                        self.currentPage(self.currentPage() + 1);
                        self.searchDB();
                    }
                }
            }
        }//#f5f5f5
    },
    template: `
    <div class="gara-bill-infor-button">
            <div>
                <a data-bind="visible: roleAdd, click: showPopAdd">
                    <i class="material-icons">add</i>
                </a>
            </div>
            <input class="gara-search-HH _jsInput" autocomplete="off" onclick="this.select()"
                placeholder="Tìm hàng hóa theo mã hoặc tên (F3)"
                data-bind="value: textSearch, valueUpdate: 'afterkeydown',  event:{keyup: search}"
            />
            <div class="gara-search-dropbox">
                <ul data-bind="foreach: searchList,event:{scroll: scrollList} ">
                    <li style="margin: 0; cursor: pointer"  data-bind="click: $parent.ChoseItem, 
                    style:{background: $parent.indexFocus() === $index()?'#f5f5f5':'none'}">
             <a style="display: flex; text-decoration: none; cursor: pointer; font-size: 1rem;  color: #111; padding: 13px 0 10px 0px" 
                class="list-search ko-searchListHanghoa">
    
                <div class="flex flex-row" style="flex-wrap:nowrap; width: 100%!important">
                    <div style="border: 1px dotted #ccc;padding: 5px;width: 50px;height: 50px; margin-right:5px;">
                        <img style="width: 40px; height:40px; " 
                        data-bind="attr:{src: SrcImage === null || SrcImage === '' ? '/Content/images/iconbepp18.9/gg-37.png' : Open24FileManager.hostUrl + SrcImage }">
                    </div>
                    <div class="flex flex-column" style="width:100%!important">
                        <small style="display:flex">
                            Mã hàng:
                            <font data-bind="text: MaHangHoa"></font>

                        </small>
                        <div class="flex flex-row">

                            <span>
                                Tên hàng:
                                <b style="color:var(--color-primary)" data-bind="text: TenHangHoa"></b>
                            </span>

                            <span data-bind="text: ThuocTinh_GiaTri" style="color:#ff6a00"></span>
                            <span data-bind="visible: TenDonViTinh" style="color:#0091ff">
                                (<span data-bind="text: TenDonViTinh"></span>)
                            </span>
                        </div>
                        <div class="flex flex-row flex-between">
                            <span style="margin-right:5px" >
                                <span> Giá vốn:</span>
                                <b style="color:green" data-bind="text: $parent.roleXemGiaVon()? formatNumber3Digit(GiaVon,0):0 "></b>
                            </span>
                            <span style="margin-right:5px">
                                <span>Tồn: </span>
                                <b style="color:red" data-bind="text: formatNumber3Digit(TonKho)"></b>
                            </span>


                        </div>
                        <div class="flex flex-row" data-bind="visible: MaLoHang" style="color: #f12b0b">
                            <span style="margin-right:5px">
                                <span>LSX:  </span>
                                <span class="bold" data-bind="text: MaLoHang"></span>
                            </span>
                            <span style="margin-right:5px" data-bind="visible: NgaySanXuat">
                                <span>NSX: </span>
                                <span style="color:green" data-bind="text: moment(NgaySanXuat).format('DD/MM/YYYY')!='Invalid date'? moment(NgaySanXuat).format('DD/MM/YYYY'):''"></span> - </i>
                            </span>
                            <span style="margin-right:5px" data-bind="visible: NgayHetHan">
                                <span>HSD: </span>
                                <span style="color:red" data-bind="text: moment(NgayHetHan).format('DD/MM/YYYY')!='Invalid date'? moment(NgayHetHan).format('DD/MM/YYYY'):''"></span>
                            </span>

                        </div>
           
                    </div>
                </div>
    </a>
                        </li>
                   
                </ul>
            </div>
        </div>
    `,
});

ko.components.register('jqauto-car', {
    viewModel: function (params) {
        var self = this;
        self.textSearch = ko.observable();
        self.searchList = ko.observableArray();
        self.indexFocus = ko.observable(0);
        self.currentPage = ko.observable(0);
        self.pageSize = ko.observable(30);
        self.roleAdd = true;// VHeader.Quyen.indexOf('DanhMucXe_ThemMoi') > -1;
        self.formType = params.form == null || params.form === undefined ? 0 : params.form;
        self.ChangeCar = params.choseItem;
        self.AddNewItem = params.addItem;
        self.UpdateItem = params.updateItem;

        self.hideList = function () {
            $('jqauto-car .gara-search-dropbox').hide();
        };
        self.showList = function () {
            $('jqauto-car .gara-search-dropbox').show();
        };
        self.showPopAdd = function () {
            self.AddNewItem();
        };
        self.showPopUpdate = function () {
            self.UpdateItem();
        };
        self.search = function () {
            var keyCode = event.keyCode || event.which;
            clearTimeout(delayTimer);

            switch (keyCode) {
                case 38:
                    self.keyUp();
                    break;
                case 40:
                    self.keyDown();
                    break;
                default:
                    if (keyCode !== 13) {
                        self.indexFocus(0);
                    }
                    delayTimer = setTimeout(function () {
                        self.currentPage(0);
                        self.searchDB(keyCode);
                    }, 300);
                    break;
            }
        };
        self.searchDB = function (keyCode) {
            var self = this;
            var txt = locdau(self.textSearch()).trim();
            if (txt === '') {
                self.searchList([]);
                self.ChangeCar(
                    {
                        ID: null,
                        BienSo: '',
                    });
                return;
            }

            if (keyCode === 13 && self.searchList().length > 0) {
                self.keyEnter();
            }
            else {
                // lahanghoa =0: chi get xe chua setup idhanghoa
                $.getJSON("/api/DanhMuc/GaraAPI/" + "JqAuto_SearchXe?txt=" + txt
                    + '&statusTN=&idCustomer=&laHangHoa=0').done(function (x) {
                        if (x.res) {
                            self.searchList(x.dataSoure);

                            if (keyCode === 13) {
                                self.keyEnter();
                            }

                            if (self.searchList().length > 0) {
                                self.showList();
                            }
                            else {
                                self.hideList();
                            }
                        }
                    });
            }
        };
        self.keyEnter = function () {
            let itChose = $.grep(self.searchList(), function (x, index) {
                return index === self.indexFocus();
            });
            if (itChose.length > 0) {
                self.textSearch(itChose.BienSo);
                self.ChangeCar(itChose[0]);
                self.searchList([]);
            }
            $('jqauto-car .gara-search-dropbox').hide();
        };
        self.keyUp = function () {
            var self = this;
            if (self.indexFocus() < 0) {
                self.indexFocus(0);
            }
            else {
                self.indexFocus(self.indexFocus() - 1);
            }
        };
        self.keyDown = function () {
            var self = this;
            if (self.indexFocus() > self.searchList().length) {
                self.indexFocus(0);
            }
            else {
                self.indexFocus(self.indexFocus() + 1);
            }
        };
        self.ChoseItem = function (item) {
            self.ChangeCar(item);
            self.textSearch(item.BienSo);
            self.searchList([]);
        }
        self.UpDownEnter = function (keyCode) {
            var self = this;
            switch (keyCode) {
                case 13:
                    self.keyEnter();
                    break;
                case 38:
                    self.keyUp();
                    break;
                case 40:
                    self.keyDown();
                    break;
            }
        };

        self.scrollList = function () {
            var elem = event.target;
            if (elem.scrollTop > (elem.scrollHeight - elem.offsetHeight - 200)) {
                if (elem.scrollTop + elem.clientHeight >= elem.scrollHeight) {
                    let lenData = self.searchList().length;
                    if (lenData === (self.currentPage() + 1) * self.pageSize()) {
                        self.currentPage(self.currentPage() + 1);
                        self.searchDB();
                    }
                }
            }
        };
    },
    template: `
    <div class="gara-bill-infor-button">
            <div>
                <a data-bind="visible: roleAdd, click: showPopAdd" >
                    <i class="material-icons">add</i>
                </a>
            </div>
            <input class="gara-search-HH _jsInput" autocomplete="off" 
                placeholder="Tìm biển số xe"
                data-bind="value: textSearch, valueUpdate: 'afterkeydown',  event:{keyup: search}, click: showPopUpdate"
            />
            <div class="gara-search-dropbox">
                <ul data-bind="foreach: searchList,event:{scroll: scrollList} ">
                    <li style="margin: 0; cursor: pointer"  data-bind="click: $parent.ChoseItem, 
                    style:{background: $parent.indexFocus() === $index()?'#f5f5f5':'none'}">
             <a style="display: flex; text-decoration: none; cursor: pointer; font-size: 1rem;  color: #111; padding: 13px 0 10px 0px" 
                class="list-search ko-searchListHanghoa">
    
                <div class="flex flex-row" style="flex-wrap:nowrap; width: 100%!important">
                    <div class="flex flex-column" style="width:100%!important">
                        <div class="flex flex-row">
                            <span>
                                <b style="color:var(--color-primary)" data-bind="text: BienSo"></b>
                            </span>
                        </div>
                    </div>
                </div>
                 </a>
                        </li>
                   
                </ul>
            </div>
        </div>
    `,
});