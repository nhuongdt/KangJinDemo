var componentListStaff = {
    props: {
        ID: { default: null },
        MaNhanVien: { default: '' },
        TenNhanVien: { default: '' },
        isChose: { default: '' },
    },
    template: `
        <li v-on:click="choseStaff">
             <a href="javascript:void(0)" class="gara-component-item-nv">
                 <div class="flex flex-between">
                    <span>{{MaNhanVien}}</span>
                     <span v-if="isChose" class="span-check"> <i class="fa fa-check">  </i> </span>
                </div> 
                <span class="seach-hh" style="color:black">{{TenNhanVien}}</span>
            </a>
        </li>
`,
    methods: {
        choseStaff: function () {
            this.$emit('chose-staff', this);
        }
    }
}
var ComponentChoseStaff = {
    props: {
        staffName: { default: '' },
        textSearch: { default: '' },
        staffs: { default: [] },
        searchList: { default: [] },
        idChosing: { default: null },
        showbuttonReset: { default: false },
    },
    components: {
        'staffs': componentListStaff,
    },
    template: `
    <div class="gara-bill-infor-button shortlabel">
         <div class="gara-absolute-button" style="text-align: center" v-on:click="resetItemChose" v-if="showbuttonReset">
                <a class="gara-button-icon">
                 <i class="fa fa-times" style="color:red"></i>
                </a>
            </div> 
        <input class="gara-search-HH " placeholder="Chọn nhân viên" style="padding-right: 27px!important"
                onclick= "this.select()"
                v-model="textSearch" v-on:keyup="searchStaff" v-on:click="showList" />
        <div class="gara-search-dropbox drop-search ">
               <ul>
                <staffs v-for="(item, index) in searchList"
                           v-bind:id="item.ID"
                           v-bind:ma-nhan-vien="item.MaNhanVien"
                           v-bind:ten-nhan-vien="item.TenNhanVien"
                           v-bind:is-chose="idChosing == item.ID"
                           v-on:chose-staff="ChangeStaff(item)"></staffs>
                 </ul>
        </div>
    </div>
`,
    data: function () {
        return {
            idChosing: null,
            textSearch: '',
            staffs: [],
            searchList: [],
        }
    },
    methods: {
        showList: function () {
            $(event.currentTarget).next().show();
        },
        searchStaff: function () {
            var self = this;
            if (commonStatisJs.CheckNull(self.textSearch)) {
                self.searchList = self.staffs.slice(0, 20);
                self.idChosing = null;
                self.$emit('reset-item-chose');
            }
            else {
                let txt = commonStatisJs.convertVieToEng(self.textSearch);
                self.searchList = self.staffs.filter(e =>
                    commonStatisJs.convertVieToEng(e.MaNhanVien).indexOf(txt) >= 0
                    || commonStatisJs.convertVieToEng(e.TenNhanVien).indexOf(txt) >= 0
                    || e.TenNhanVien.indexOf(txt) >= 0
                );
            }
        },
        ChangeStaff: function (item) {
            this.idChosing = item.ID;
            this.$emit('change-staff-parent', item);
            $(event.currentTarget).closest('div').hide();
        },
        resetItemChose: function () {
            let self = this;
            self.customers = [];
            self.idChosing = null;
            self.$emit('reset-item-chose');
        }
    }
};

// list search NV show left
var cmpSearchNVDisscount = {
    props: {
        listAll: { default: [] },
        listSearch: { default: [] },
        idChosing: { default: null},
        showCol2: { default: true },
        showCol3: { default: true },
        showCol4: { default: false },
        showCol5: { default: false },
    },
    template: `
    <div class="flex flex-column">
            <div class="position-relative">
                    <input class="form-control textSearchNV" placeholder="Tìm nhân viên" style="padding-left:30px;" autocomplete="off"
                                v-model="textSearch"
                                v-on:keyup="search"
                                v-on:keyup.13="keyEnter"
                                v-on:keyup.up="keyUp"
                                v-on:keyup.down="keyDown" />
                    <i class="material-icons" style="position:absolute; top:3px; left:5px;">search</i>
            </div>
            <ul style="max-height:400px;overflow:auto; overflow-x:hidden; width:100%" >
                <li v-for="(item, index) in listSearch" class="list-img-user"
                    v-bind:style="[idChosing === item.ID_NhanVien ? {'background':'#f9f9f9'} : {'background':'none'}]"
                    v-on:click="SelectItem(item)">
                    <label style="display:flex; align-items:center; padding:7px" class="floatleft">
                            <span class="img-user">
                                <img v-bind:src="item.URLAnh === '' || item.URLAnh === undefined  ? item.GioiTinh === true ? '/Content/images/icon/gioi-tinh-nam.png' : '/Content/images/icon/gioi-tinh-nu.png' : Open24FileManager.hostUrl + item.URLAnh" />
                            </span>
                            <span class="detail-user-discount">
                                <span class="floatleft" style="font-weight:normal">
                                    Mã NV: <span>{{item.MaNhanVien}}</span>
                                </span>
                                <span class="floatleft">{{item.TenNhanVien}}</span>
                                <span class="floatleft" style="font-weight:normal" v-if="showCol2">SDT :<span>{{item.SoDienThoai}}</span> </span>
                                <span class="floatleft" style="font-weight:normal" v-if="showCol3">
                                    Trạng thái : <span
                                              :class="{red: item.StatusCV==1}">
                                            {{item.StatusCV==1?'Đang bận':'Đang rảnh'}}
                                            </span>
                                </span>
                                <small class="floatleft"  v-if="showCol4">{{item.TenPhongBan}}</small>
                            </span>
                         <input type="checkbox" v-if="showCol5" v-on:click="choseNhanVien(item)" />
                        </label>
                    </li>
            </ul>
        </div>
`,
    data: function () {
        return {
            indexFocus: 0,
            textSearch: '',
            listAll: [],
            listSearch: [],
        }
    },
    methods: {
        showList: function () {
            $(event.currentTarget).next().show();
        },
        search: function () {
            var self = this;
            if (commonStatisJs.CheckNull(self.textSearch)) {
                self.listSearch = self.listAll.slice(0, 20);
            }
            else {
                let txt = commonStatisJs.convertVieToEng(self.textSearch);
                self.listSearch = self.listAll.filter(e =>
                    commonStatisJs.convertVieToEng(e.MaNhanVien).indexOf(txt) >= 0
                    || commonStatisJs.convertVieToEng(e.TenNhanVien).indexOf(txt) >= 0
                    || e.TenNhanVien.indexOf(txt) >= 0
                );
            }
            $(event.currentTarget).next().show();
        },
        keyEnter: function () {
            var self = this;
            var chosing = $.grep(self.listSearch, function (item, index) {
                return index === self.indexFocus;
            });
            if (chosing.length > 0) {
                this.$emit('select-item', chosing[0]);
            }
        },
        keyUp: function () {// key = 38
            var self = this;
            if (self.indexFocus < 0) {
                self.indexFocus = 0;
            }
            else {
                self.indexFocus = self.indexFocus - 1;
            }
        },
        keyDown: function () {// key = 40
            let self = this;
            if (self.indexFocus > self.listSearch.length) {
                self.indexFocus = 0;
            }
            else {
                self.indexFocus = self.indexFocus + 1;
            }
        },
        SelectItem: function (item) {
            let self = this;
            self.$emit('select-item', item);
        },
        choseNhanVien: function (item) {
            let self = this;
            this.$emit('select-item-check', item);
        }
    }
};

// customer
var cmpListCustomer = {
    props: ['ID', 'MaDoiTuong', 'TenDoiTuong', 'DienThoai', 'Email', 'DiaChi'],
    template: `
        <li v-on:click="choseCustomer">
            <a href="javascript:void(0)">
                <div class="flex flex-between">
                    <span>{{MaDoiTuong}}</span>
                    <span v-if="DienThoai"> SĐT: {{DienThoai}}</span>
                </div> 
                <span class="seach-hh" style="color:black">{{TenDoiTuong}}</span>
               
            </a>
        </li>
`,
    methods: {
        choseCustomer: function () {
            this.$emit('chose-customer', this);
        }
    }
}
var cmpChoseCustomer = {
    props: {
        textSearch: { default: '' },
        loaiDoiTuong: { default: 1 },
        showbutton: { default: false },// btnAdd
        showbuttonUpdate: { default: false },
        showbuttonReset: { default: false },
        disableSearch: { default: false },
        idChiNhanh: { default: false },
    },
    components: {
        'customers': cmpListCustomer,
    },
    template: `
        <div class="gara-detail-input" style="position: relative;">
            <div class="gara-absolute-button" v-on:click="showModal" v-if="showbutton">
                <a class="gara-button-icon">
                 <i class="material-icons">add</i>
                </a>
            </div> 
            <div class="gara-absolute-button" v-on:click="showModalUpdate" v-if="showbuttonUpdate">
                <a class="gara-button-icon">
                 <i class="far fa-edit"></i>
                </a>
            </div> 
            <div class="gara-absolute-button" style="text-align: center" v-on:click="resetItemChose" v-if="showbuttonReset">
                <a class="gara-button-icon">
                 <i class="fa fa-times" style="color:red"></i>
                </a>
            </div> 
            <input :placeholder="placeholderTxt" class="form-control gara-search-HH" 
                :disabled="disableSearch"
                v-model="textSearch" 
                v-on:keyup="searchCustomer" 
                v-on:click="showList"> 
             <div class="gara-search-dropbox drop-search">
                 <ul>
                 <customers v-for="item in customers"
                           v-bind:id="item.ID"
                           v-bind:ma-doi-tuong="item.MaNguoiNop"
                           v-bind:ten-doi-tuong="item.NguoiNopTien"
                           v-bind:dien-thoai="item.SoDienThoai"
                           v-bind:email="item.Email"
                           v-bind:dia-chi="item.DiaChi"
                           v-on:chose-customer="ChoseCustomer(item)"></customers>
                </ul>

             </div>
        </div>
`,
    created: function () {
        this.timmer = null;
    },
    data: function () {
        return {
            idChiNhanh: null,
            textSearch: '',
            customers: [],
            loaiDoiTuong: 1,
        }
    },
    methods: {
        showList: function () {
            $(event.currentTarget).next().show();
            $(event.currentTarget).focus().select();
        },
        searchCustomer: function () {
            var self = this;
            clearTimeout(self.timmer);
            self.timmer = setTimeout(function () {
                self.searchDB();
            }, 300);
            $(event.currentTarget).next().show();
        },
        searchDB: function () {
            var self = this;
            var txt = locdau(self.textSearch).trim();
            if (commonStatisJs.CheckNull(txt)) {
                self.customers = [];
                this.$emit('reset-customer-parent');
                return;
            }
            $.getJSON("/api/DanhMuc/DM_DoiTuongAPI/" + "JqAuto_SearchDoiTuong?loaiDoiTuong="
                + self.loaiDoiTuong + "&txtSearch=" + txt + '&idChiNhanh=' + self.idChiNhanh).done(function (data) {
                    if (self.loaiDoiTuong === 1) {
                        data = $.grep(data, function (x) {
                            return x.ID.indexOf('00000000') === -1;
                        });
                    }
                    self.customers = data;
                });
        },
        ChoseCustomer: function (item) {
            var self = this;
            var cus = {
                ID: item.ID,
                MaDoiTuong: item.MaNguoiNop,
                TenDoiTuong: item.NguoiNopTien,
                DienThoai: item.SoDienThoai,
                Email: item.Email,
                DiaChi: item.DiaChi,
            }
            this.$emit('change-customer-parent', cus);
            $(event.currentTarget).closest('div').hide();
            self.customers = [];
        },
        showModal: function () {
            this.$emit('show-modal-customer');
        },
        showModalUpdate: function () {
            this.$emit('show-modal-update');
        },
        resetItemChose: function () {
            self.customers = [];
            this.$emit('reset-customer-parent');
        }
    },
    computed: {
        placeholderTxt: function () {
            var text = '';
            switch (this.loaiDoiTuong) {
                case 1:
                    text = 'Khách hàng';
                    break;
                case 2:
                    text = 'Nhà cung cấp';
                    break;
                case 3:
                    text = 'Bảo hiểm';
                    break;
                case 4:
                    text = 'Nhân viên';
                    break;
            }
            return text;
        }
    }
};

// products
var cmpChoseProduct = {
    props: {
        ConTonKho: { default: 0 },
        LoaiHangHoa: { default: '%%' },// 11.hanghoa, 12.dichvu, 23.combo, %%: all
        QuanLyTheoLo: { default: '%%' },
        showTonKho: { default: false },
        showGiaVon: { default: false },
        showImage: { default: false },
        idBangGia: { default: '00000000-0000-0000-0000-000000000000' },
        idChiNhanh: { default: null },//#d9d9d9
    },
    template: `
       <div style=" position:relative" class="jsSearchProduct op-search-list" v-bind:class="{ showImageList: showImage }">

            <i class="material-icons icon-searchs">search</i>
            <input type="text" class="form-control gara-search-HH " style="padding-left:30px"
                    v-model="textSearch"
                    v-on:keyup="searchProduct"
                    v-on:keyup.13="KeyEnter"
                    v-on:keyup.up="KeyUp"
                    v-on:keyup.down="KeyDown"
                    placeholder="Tìm kiếm" aria-label="Tìm kiếm">

                <ul class="gara-search-dropbox" style="width:100%;top:100%" v-on:scroll.passive="scrollList">
                    <li v-for="(item, index) of searchList" v-on:click="ChoseProduct(item)"   class="flex"
                        v-bind:style="[$parent.IndexFocus === index ? {'background':'red'} : {'background':'none'}]">
                            <img v-bind:src="item.SrcImage" v-if="showImage" alt="Ảnh tìm kiếm" />
                            <div class="op-hh-detail">
                                <div class="flex flex flex-between" >
                                    <span class="bold">
                                        {{ item.TenHangHoa }}
                                        <span style="font-style:normal">
                                            <span title="Thuộc tính giá trị" style="color:#ff6a00">{{item.ThuocTinh_GiaTri}}</span>
                                            <span title="Đơn vị tính" style="color:#007acc">{{ item.TenDonViTinh }}</span>
                                        </span>
                                    </span>
                                    <span v-if="showTonKho">
                                        <span>  Tồn kho:</span>
                                        <span class="red bold">{{ formatNumber3Digit(item.TonKho) }}</span>
                                    </span>
                                </div>
                                <div class="flex flex-between">
                                      <span>
                                        <span> Mã:</span>
                                        <span class="bold">
                                            {{ item.MaHangHoa }}
                                        </span>
                                    </span>
                                    <span v-if="!showGiaVon">
                                        <span> Giá bán:</span>
                                        <span>
                                            {{ formatNumber(item.GiaBan) }}
                                        </span>
                                    </span>
                                    <span v-if="showGiaVon">
                                        <span> Giá vốn:</span>
                                        <span>
                                            {{ formatNumber3Digit(item.GiaVon) }}
                                        </span>
                                    </span>
                                    
                                </div>
                                <div class="flex flex-between">
                                    <span v-if="item.MaLoHang">
                                        <span> Lô:</span>
                                        <span class="red">{{ item.MaLoHang }}</span>
                                    </span>
                                    <span v-if="item.NgaySanXuat">
                                        <span> NSX:</span>
                                        <span style="color:green">
                                            {{ item.NgaySanXuat==null?'': moment(item.NgaySanXuat).format('DD/MM/YYYY')}}
                                        </span>
                                    </span>
                                    <span v-if="item.NgayHetHan">
                                        <span> HSD:</span>
                                        <span style="color:red">
                                            {{ item.NgayHetHan==null?'': moment(item.NgayHetHan).format('DD/MM/YYYY') }}
                                        </span>
                                    </span>
                                </div>
                                
                            </div>
                      
                    </li>
                </ul>
            
        </div>
`,
    data: function () {
        return {
            textSearch: '',
            searchList: [],
            currentPage: 0,
            pageSize: 30,
            IndexFocus: 0,
        }
    },
    methods: {
        formatNumber: function (number) {
            if (number === undefined || number === null) {
                return 0;
            }
            else {
                return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            }
        },
        searchProduct: function () {
            let self = this;
            clearTimeout(self.timmer);
            let keyCode = event.keyCode || event.which;
            if ($.inArray(keyCode, [13, 38, 40]) > -1) {
                if (keyCode === 13 && self.searchList.length > 0 || keyCode === 38 || keyCode === 40) {
                    return;
                }
            }
            self.timmer = setTimeout(function () {
                self.currentPage = 0;
                self.searchDB();
            }, 300);
            $(event.currentTarget).next().show();
        },
        searchDB: function () {
            var self = this;
            var txt = locdau(self.textSearch);
            if (self.idBangGia === undefined) {
                self.idBangGia = "00000000-0000-0000-0000-000000000000";
            }
            if (self.QuanLyTheoLo === undefined) {
                self.QuanLyTheoLo = "%%";
            }
            if (self.LaHangHoa === undefined) {
                self.LaHangHoa = "%%";
            }
            var param = {
                ID_ChiNhanh: self.idChiNhanh,
                ID_BangGia: self.idBangGia,
                TextSearch: txt,
                LaHangHoa: self.LoaiHangHoa,
                QuanLyTheoLo: self.QuanLyTheoLo,
                ConTonKho: self.ConTonKho,
                CurrentPage: self.currentPage,
                PageSize: self.pageSize,
            };
            $.ajax({
                type: 'POST',
                url: "/api/DanhMuc/DM_HangHoaAPI/" + "Gara_JqAutoHangHoa",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: param ? JSON.stringify(param) : null,
            }).done(function (x) {
                if (x.res) {
                    if (self.currentPage === 0) {
                        self.searchList = x.dataSoure;
                    }
                    else {
                        for (let i = 0; i < x.dataSoure.length; i++) {
                            self.searchList.push(x.dataSoure[i]);
                        }
                    }
                }
                else {
                    console.log(x.mess);
                }
            });
        },
        ChoseProduct: function (item) {
            this.$emit('chose-product', item);
            $(event.currentTarget).closest('.jsSearchProduct').find('.gara-search-dropbox').hide();
        },
        KeyEnter: function () {
            var self = this;
            var chosing = $.grep(self.searchList(), function (item, index) {
                return index === self.IndexFocus;
            });
            if (chosing.length > 0) {
                self.ChoseProduct(chosing[0]);
            }
        },
        KeyUp: function () {
            var self = this;
            if (self.IndexFocus < 0) {
                self.IndexFocus = 0;
            }
            else {
                self.IndexFocus = self.IndexFocus - 1;
            }
        },
        KeyDown: function () {
            var self = this;
            if (self.IndexFocus > self.searchList.length) {
                self.IndexFocus = 0;
            }
            else {
                self.IndexFocus = self.IndexFocus + 1;
            }
        },
        scrollList: function () {
            let self = this;
            let elem = event.currentTarget;
            if (elem.scrollTop > (elem.scrollHeight - elem.offsetHeight - 200)) {
                if (elem.scrollTop + elem.clientHeight >= elem.scrollHeight) {
                    let lenData = self.searchList.length;
                    if (lenData === (self.currentPage + 1) * self.pageSize) {
                        self.currentPage += 1;
                        self.searchDB();
                    }
                }
            }
        }
    }
};

// account bank
var cmpListAccountBank = {
    props: ['ID', 'TenChuThe', 'TenNganHang', 'isChose'],
    template: `
        <li v-on:click="choseAccount">
                <div>
                <span style="color:var(--color-main)">{{TenChuThe}}</span>
                <span class="span-check" v-if="isChose"> <i class="fa fa-check">  </i> </span>
            </div>
            <div>
                <span >{{TenNganHang}}</span>
            </div>
        </li>
`,
    methods: {
        choseAccount: function () {
            this.$emit('chose-account', this);
        }
    }
}
var cmpChoseAccountBank = {
    props: {
        textSearch: { default: [] },
        formType: { default: 0 },// 1.soquy, 2.thanh toán nhiều tk ngân hàng, 0.conlai
        idChosing: { default: null },
        searchList: { default: [] },
        accounts: { default: [] },
    },
    components: {
        'accounts': cmpListAccountBank,
    },
    template: `
        <div class="gara-bill-infor-button shortlabel">
        <div v-if="formType === 1">
            <a>
                <i class="fa fa-eye" ></i>
            </a>
        </div>   
        <div v-if="formType === 2" v-on:click="addRow">
            <a title="Thêm hình thức thanh toán cùng loại">
               <i class="fal fa-plus"></i>
            </a>
        </div>
        <input class="gara-search-HH" placeholder="Chọn tài khoản" v-model="textSearch" v-on:keyup="searchAccount" v-on:click="showList"/>
        <div class="gara-search-dropbox drop-search">
            <ul>
                <li v-on:click="ResetAccount">
                    <div>
                        <span style="color:var(--color-main)">--Chọn tài khoản --- </span>
                        <span v-if="idChosing==null" style="float: right;"> <i class="fa fa-check">  </i> </span>
                     </div>
                </li>
             <accounts v-for="(item, index) in searchList"
                v-bind:id="item.ID"
                v-bind:ten-chu-the="item.TenChuThe"
                v-bind:ten-ngan-hang="item.TenNganHang"
                v-bind:is-chose="idChosing!=null && idChosing.toUpperCase() == item.ID.toUpperCase()"
                v-on:chose-account="ChangeAccount(item)"></accounts>
            </ul>
        </div>
    </div>
`,
    methods: {
        showList: function () {
            $(event.currentTarget).next().show();
        },
        searchAccount: function () {
            var self = this;
            if (commonStatisJs.CheckNull(self.textSearch)) {
                this.$emit('reset-account');
                self.searchList = self.accounts.slice(0, 20);
            }
            else {
                var txt = locdau(self.textSearch);
                self.searchList = self.accounts.filter(e =>
                    locdau(e.TenChuThe).indexOf(txt) >= 0
                    || locdau(e.TenNganHang).indexOf(txt) >= 0
                );
            }
        },
        ChangeAccount: function (item) {
            this.$emit('change-account-parent', item);
            $(event.currentTarget).closest('div').hide();
        },
        ResetAccount: function () {
            this.$emit('reset-account');
            $(event.currentTarget).closest('div').hide();
        },
        addRow: function () {
            this.$emit('add-row');
        }
    }
};

// khoan thuchi
var cmpListKhoanThu = {
    props: ['ID', 'NoiDungThuChi', 'isChose'],
    template: `
        <li v-on:click="choseKhoanThuChi">
            <div>
                <span style="color:var(--color-main)">{{NoiDungThuChi}}</span>
                <span v-if="isChose" style="float: right;"> <i class="fa fa-check">  </i> </span>
            </div>
        </li>
`,
    methods: {
        choseKhoanThuChi: function () {
            this.$emit('chose-khoan-thu', this);
        }
    }
}
var cmpChoseKhoanThu = {
    props: {
        textSearch: { default: '' },
        idChosing: { default: null },
        showbuttonAdd: { default: false },
        showbuttonUpdate: { default: false },
        LaKhoanThu: { default: true },
        ListAll: { default: [] },
    },
    components: {
        'khoan-thu-chi': cmpListKhoanThu,
    },
    template: `
        <div class="gara-bill-infor-button shortlabel">
         <div class="gara-absolute-button" v-on:click="showModal" v-if="showbuttonAdd">
                <a class="gara-button-icon">
                 <i class="fal fa-plus"></i>
                </a>
            </div> 
            <div class="gara-absolute-button" v-on:click="showModalUpdate" v-if="showbuttonUpdate">
                <a class="gara-button-icon">
                 <i class="far fa-edit"></i>
                </a>
            </div> 
        <input class="gara-search-HH" :placeholder="placeholderValue" v-model="textSearch" v-on:keyup="search"
        v-on:click="showList"/>
        <div class="gara-search-dropbox drop-search">
            <ul>
                <li v-on:click="ResetKhoanThu">
                <div>
                    <span style="color:var(--color-main)">--{{placeholderValue}} --- </span>
                    <span v-if="idChosing==null" style="float: right;"> <i class="fa fa-check">  </i> </span>
                    </div>
                </li>
             <khoan-thu-chi v-for="(item, index) in searchList" 
                v-bind:id="item.ID"
                v-bind:noi-dung-thu-chi="item.NoiDungThuChi"
                v-bind:is-chose="idChosing == item.ID"
                v-on:chose-khoan-thu="ChangeKhoanThu(item)"></khoan-thu-chi>
            </ul>
        </div>
    </div>
`,
    data: function () {
        return {
            searchList: [],
        }
    },
    methods: {
        showList: function () {
            var self = this;
            self.search();// clear txtSearch old
            $(event.currentTarget).next().show();
        },
        search: function () {
            var self = this;
            let arr = self.ListAll.filter(x => x.LaKhoanThu === self.LaKhoanThu);
            if (commonStatisJs.CheckNull(self.textSearch)) {
                self.searchList = arr.slice(0, 20);
                this.$emit('reset-khoan-thu');
            }
            else {
                let txt = locdau(self.textSearch);
                self.searchList = $.grep(arr, function (e) {
                    return locdau(e.NoiDungThuChi).indexOf(txt) > -1 || e.NoiDungThuChi.indexOf(txt) > -1
                });
            }
        },
        ChangeKhoanThu: function (item) {
            this.$emit('change-khoan-thu', item);
            $(event.currentTarget).closest('div').hide();
        },
        ResetKhoanThu: function () {
            this.$emit('reset-khoan-thu');
            $(event.currentTarget).closest('div').hide();
        },
        showModal: function () {
            this.$emit('show-modal-add');
        },
        showModalUpdate: function () {
            this.$emit('show-modal-update');
        }
    },
    computed: {
        placeholderValue: function () {
            let self = this;
            self.searchList = self.ListAll.filter(x => x.LaKhoanThu === self.LaKhoanThu).slice(0, 20);
            return self.LaKhoanThu ? 'Chọn khoản thu' : 'Chọn khoản chi';
        },
    }
};

// chinhanh
var cmpChiNhanh = {
    props: {
        idChosing: { default: null },
        textSearch: { default: '' },
        listAll: { default: [] },
        showbutton: { default: false },
    },
    data: function () {
        return {
            listSearch: [],
        }
    },
    template: `

<div class="outselect add-customer">
        <div class="gara-bill-infor-button shortlabel">
        <div v-if="showbutton">
            <a v-on:click="showModalAdd">
                <i class="fal fa-plus"></i>
            </a>
        </div>
        <input class="gara-search-HH " placeholder="Chọn chi nhánh" style="padding-right: 27px!important"
                        v-model="textSearch" 
                        v-on:click="showList"
                        v-on:keyup="search"
                        v-on:keyup.13="keyEnter"
                        v-on:keyup.up="keyUp"
                        v-on:keyup.down="keyDown"/>
        <div class="gara-search-dropbox drop-search">
               <ul>
                     <li v-on:click="OnSelect(item)"
                          v-for="(item, index) in listSearch">
                        <div>
                            <span style="color: var(--color-primary);">{{item.TenDonVi}}</span> 
                            <span class="span-check" style="float:right" v-if="idChosing == item.ID">
                                <i class="fa fa-check"></i>
                            </span>
                        </div> 
                    </li>
                 </ul>
        </div>
    </div>
     </div>

`,
    methods: {
        showList: function () {
            $(event.currentTarget).next().show();
        },
        search: function () {
            var self = this;
            if (commonStatisJs.CheckNull(self.textSearch)) {
                self.listSearch = self.listAll.slice(0, 20);
            }
            else {
                let txt = commonStatisJs.convertVieToEng(self.textSearch);
                self.listSearch = self.listAll.filter(e =>
                    commonStatisJs.convertVieToEng(e.TenDonVi).indexOf(txt) >= 0
                    || locdau(e.MaDonVi).indexOf(txt) >= 0
                );
            }
        },
        keyEnter: function () {
            var self = this;
            var chosing = $.grep(self.searchList, function (item, index) {
                return index === self.indexFocus;
            });
            if (chosing.length > 0) {
                this.$emit('on-select-item', chosing[0]);
            }
        },
        keyUp: function () {// key = 38
            var self = this;
            if (self.indexFocus < 0) {
                self.indexFocus = 0;
            }
            else {
                self.indexFocus = self.indexFocus - 1;
            }
        },
        keyDown: function () {// key = 40
            var self = this;
            if (self.indexFocus > self.searchList.length) {
                self.indexFocus = 0;
            }
            else {
                self.indexFocus = self.indexFocus + 1;
            }
        },
        OnSelect: function (item) {
            this.$emit('on-select-item', item);
            $(event.currentTarget).closest('div').hide();
        },
        showModalAdd: function () {
            this.$emit('show-modal-add-chinhanh');
        }
    }
}

var cmpVaiTro = {
    props: ['listAll', 'textSearch', 'listSearch', 'idChosing'],
    template: `

<div class="outselect add-customer">
        <div class="gara-bill-infor-button shortlabel">
        <input class="gara-search-HH " placeholder="Chọn vai trò" style="padding-right: 27px!important"
                        onclick="this.select()"
                        v-model="textSearch" 
                        v-on:click="showList"
                        v-on:keyup="search"
                        v-on:keyup.13="keyEnter"
                        v-on:keyup.up="keyUp"
                        v-on:keyup.down="keyDown"/>
        <div class="gara-search-dropbox drop-search">
               <ul>
                    <li v-on:click="OnSelect(item)"
                        v-for="(item, index) in listSearch">
                        <div>
                            <span style="color: var(--color-primary);">{{item.TenNhom}}</span> 
                            <span class="span-check" style="float:right" v-if="idChosing == item.ID">
                                <i class="fa fa-check"></i>
                            </span>
                        </div> 
                    </li>
                 </ul>
        </div>
    </div>
     </div>

`,
    methods: {
        showList: function () {
            $(event.currentTarget).next().show();
        },
        search: function () {
            var self = this;
            if (commonStatisJs.CheckNull(self.textSearch)) {
                self.listSearch = self.listAll.slice(0, 20);
            }
            else {
                let txt = commonStatisJs.convertVieToEng(self.textSearch);
                self.listSearch = self.listAll.filter(e =>
                    commonStatisJs.convertVieToEng(e.TenNhom).indexOf(txt) >= 0
                );
            }
        },
        keyEnter: function () {
            var self = this;
            var chosing = $.grep(self.searchList, function (item, index) {
                return index === self.indexFocus;
            });
            if (chosing.length > 0) {
                this.$emit('on-select-item', chosing[0]);
            }
        },
        keyUp: function () {// key = 38
            var self = this;
            if (self.indexFocus < 0) {
                self.indexFocus = 0;
            }
            else {
                self.indexFocus = self.indexFocus - 1;
            }
        },
        keyDown: function () {// key = 40
            var self = this;
            if (self.indexFocus > self.searchList.length) {
                self.indexFocus = 0;
            }
            else {
                self.indexFocus = self.indexFocus + 1;
            }
        },
        OnSelect: function (item) {
            this.$emit('on-select-item', item);
            $(event.currentTarget).closest('div').hide();
        },
    }
}

var cmpLoaiChungTu = {
    props: {
        listAll: { default: '' },
        textSearch: { default: '' },
        idChosing: { default: null },
    },
    data: function () {
        return {
            listSearch: [],
        }
    },
    template: `

<div class="outselect add-customer">
        <div class="gara-bill-infor-button shortlabel">
        <input class="gara-search-HH " placeholder="Chọn chứng từ" style="padding-right: 27px!important"
                        onclick="this.select()"
                        v-model="textSearch" 
                        v-on:click="showList"
                        v-on:keyup="search"
                        v-on:keyup.13="keyEnter"
                        v-on:keyup.up="keyUp"
                        v-on:keyup.down="keyDown"/>
        <div class="gara-search-dropbox drop-search">
               <ul>
                    <li v-on:click="OnSelect(item)"
                        v-for="(item, index) in listSearch">
                        <div>
                            <span style="color: var(--color-primary);">{{item.TenLoaiChungTu}}</span> 
                            <span class="span-check" style="float:right" v-if="idChosing == item.ID">
                                <i class="fa fa-check"></i>
                            </span>
                        </div> 
                    </li>
                 </ul>
        </div>
    </div>
     </div>

`,
    created: function () {
        this.listSearch = this.listAll;
    },
    methods: {
        showList: function () {
            var self = this;
            self.listSearch = self.listAll.slice(0, 20);
            $(event.currentTarget).next().show();
        },
        search: function () {
            var self = this;
            if (commonStatisJs.CheckNull(self.textSearch)) {
                self.listSearch = self.listAll.slice(0, 20);
                this.$emit('reset-item');
            }
            else {
                let txt = commonStatisJs.convertVieToEng(self.textSearch);
                self.listSearch = self.listAll.filter(e =>
                    commonStatisJs.convertVieToEng(e.TenLoaiChungTu).indexOf(txt) >= 0
                );
            }
        },
        keyEnter: function () {
            var self = this;
            var chosing = $.grep(self.listSearch, function (item, index) {
                return index === self.indexFocus;
            });
            if (chosing.length > 0) {
                this.$emit('on-select-item', chosing[0]);
            }
        },
        keyUp: function () {// key = 38
            var self = this;
            if (self.indexFocus < 0) {
                self.indexFocus = 0;
            }
            else {
                self.indexFocus = self.indexFocus - 1;
            }
        },
        keyDown: function () {// key = 40
            var self = this;
            if (self.indexFocus > self.listSearch.length) {
                self.indexFocus = 0;
            }
            else {
                self.indexFocus = self.indexFocus + 1;
            }
        },
        OnSelect: function (item) {
            this.$emit('on-select-item', item);
            $(event.currentTarget).closest('div').hide();
        },
    }
}


// sử dụng cho ddl đã được get sẵn data, select 1 item
// Text1: mã/sdt, Text2: Tên
var cmpDropdown1Item = {
    props: {
        loaiDoiTuong: {
            default: 1,// 1.khachhang, 2.NCC, 3.baohiem, 4.nhanvien
        },
        idChiNhanh: {
            default: null,
        },
        listAll: {
            default: [],
        },
        listSearch: {
            default: [],
        },
        textSearch: {
            default: '',
        },
        idChosing: {
            default: null,
        },
        colShow: {
            default: 1,
        },
        showbutton: {
            default: false,
        },
        showModal: {
            default: false,
        },
        placeholder: {
            default: '--Chọn--',
        }
    },
    template: `

<div class="outselect add-customer">
     <div class="gara-bill-infor-button shortlabel">
        <div class="gara-absolute-button" v-if="showbutton" v-on:click="showModal">
            <a class="gara-button-icon" title="Thêm mới" v-if="idChosing == null" >
                <i class="fal fa-plus"></i>
            </a>
            <a class="gara-button-icon" title="Cập nhật" v-if="idChosing !== null">
                <i class="fal fa-edit"></i>
            </a>
        </div> 
        <input class="gara-search-HH" style="padding-right: 27px!important"
                        onclick="this.select()"
                        v-bind:placeholder="GetPlaceholder()"
                        v-model="textSearch" 
                        v-on:click="showList"
                        v-on:keyup="search"
                        v-on:keyup.13="keyEnter"
                        v-on:keyup.up="keyUp"
                        v-on:keyup.down="keyDown"/>
        <div class="gara-search-dropbox drop-search">
               <ul>
                    <li v-for="(item, index) in listSearch" 
                        v-on:click="OnSelect(item)"
                        >
                          <div v-if="colShow >= 2">
                                <span style="color:var(--color-main)" >{{item.Text1}}</span>
                                <span style="float:right" v-if="colShow >= 3">Giá bán: {{formatNumber(item.Text3)}}</span>
                           </div>
                           <div>
                                <span >{{item.Text2}}</span>
                                <span class="span-check" v-if="idChosing == item.ID">
                                    <i class="fa fa-check"> </i> 
                                </span>
                           </div>
                    </li>
                 </ul>
        </div>
        
    </div>
 </div>

`,
    created: function () {
        this.timmer = null;
    },
    methods: {
        showList: function () {
            $(event.currentTarget).next().show();
        },
        GetPlaceholder: function () {
            if (commonStatisJs.CheckNull(this.placeholder)) {
                return 'Chọn';
            }
            return this.placeholder;
        },
        search: function () {
            var self = this;
            if (self.listAll.length > 0) {
                if (commonStatisJs.CheckNull(self.textSearch)) {
                    self.listSearch = self.listAll.slice(0, 20);
                    this.$emit('reset-item');
                }
                else {
                    let txt = locdau(self.textSearch);
                    self.listSearch = self.listAll.filter(e =>
                        locdau(e.Text1).indexOf(txt) >= 0
                        || locdau(e.Text2).indexOf(txt) >= 0
                    );
                }
            }
            else {
                clearTimeout(self.timmer);
                self.timmer = setTimeout(function () {
                    self.searchDB();
                }, 300);
                $(event.currentTarget).next().show();
            }
        },
        searchDB: function () {
            var self = this;
            var txt = locdau(self.textSearch).trim();
            if (commonStatisJs.CheckNull(txt)) {
                self.customers = [];
                this.$emit('reset-customer-parent');
                return;
            }
            $.getJSON("/api/DanhMuc/DM_DoiTuongAPI/" + "JqAuto_SearchDoiTuong?loaiDoiTuong="
                + self.loaiDoiTuong + "&txtSearch=" + txt + '&idChiNhanh=' + self.idChiNhanh).done(function (data) {
                    data = $.grep(data, function (x) {
                        return x.ID.indexOf('00000000') === -1;
                    });
                    data = data.map(function (x) {
                        return {
                            ID: x.ID,
                            Text1: x.MaNguoiNop,
                            Text2: x.NguoiNopTien,
                            Text3: x.SoDienThoai
                        }
                    })
                    self.listSearch = data;
                });
        },
        keyEnter: function () {
            var self = this;
            var chosing = $.grep(self.searchList, function (item, index) {
                return index === self.indexFocus;
            });
            if (chosing.length > 0) {
                this.$emit('on-select-item', chosing[0]);
            }
        },
        keyUp: function () {// key = 38
            var self = this;
            if (self.indexFocus < 0) {
                self.indexFocus = 0;
            }
            else {
                self.indexFocus = self.indexFocus - 1;
            }
        },
        keyDown: function () {// key = 40
            var self = this;
            if (self.indexFocus > self.searchList.length) {
                self.indexFocus = 0;
            }
            else {
                self.indexFocus = self.indexFocus + 1;
            }
        },
        OnSelect: function (item) {
            this.$emit('on-select-item', item);
            $(event.currentTarget).closest('div').hide();
        },
        showModal: function () {
            this.$emit('show-modal');
        },
    },
}

var cmpDropdownMultipleItem = {
    props: {
        listAll: { default: function () { return [] } },
        listSearch: { default: function () { return [] } },
        listChosed: { default: function () { return [] } },
        textSearch: { default: '' },
        typeData: { default: 1 },
        showbutton: { default: false },
        placehoder: { default: 'Tìm kiếm' },
        colshow: { default: 1 },
        showItemAll: { default: false },
        haveCondition: { default: false },
    },
    template: `
      
                <div class="op-tag-picker dropdown nopadding">
                    <div class=" " data-toggle="dropdown" aria-expanded="false">
                        <input type="text" class="form-control" 
                                    v-bind:placeholder="GetPlaceholder()"
                                    v-model="textSearch" 
                                    v-on:click="showList"
                                    v-on:keyup="search">
                        <ul >
                            <li class="" v-for="(item,index) in listChosed">
                                <span> {{item.Text1}}
                                </span> - 
                                <span> {{item.Text2}}
                                </span>&nbsp;
                                <span v-on:click="RemoveItem(item)">
                                    <i class="fa fa-times"></i>
                                </span>
                            </li>
                        </ul>
                    </div>
                    <button class="btn css-btn-right no-margin"  style="z-index:2; margin:0"
                            v-if="showbutton"
                            v-on:click="showModalAdd">
                   <i class="fal fa-plus"></i>
                    </button>
                    <ul class="dropdown-menu  "style="width:100%">
                         <li v-on:click="OnSelect(null)" v-if="showItemAll">
                            <div>
                                <span style="color:var(--color-main)">--Tat ca --- </span>
                             </div>
                         </li>
                        <li v-for="(item, index) in listAfter" 
                            v-on:click="OnSelect(item)"
                            >
                              <div v-if="colshow >= 2">
                                    <span style="color:var(--color-main)" >{{lbl1}} {{item.Text1}}</span>
                               </div>
                               <div v-if="colshow >= 1">
                                    <span >{{lbl2}} {{item.Text2}}</span>
                               </div>
                        </li>
                    </ul>
                   
                </div>
`,
    created: function () {
        let self = this;
        self.timmer = null;
        self.ID_DonVi = $('#txtDonVi').val();
        if (commonStatisJs.CheckNull(self.ID_DonVi)) {
            self.ID_DonVi = VHeader.IdDonVi;
        }
        //self.listAfter = self.listSearch;
    },
    data: function () {
        return {
            listChosed: [],
            listAfter: [],
        }
    },
    methods: {
        GetPlaceholder: function () {
            return this.placeholder;
        },
        showList: function () {
            var self = this;
            self.listAfter = self.listSearch;
            $(event.currentTarget).next().show();
        },
        search: function () {
            var self = this;
            console.log(53)
            if (self.listAll.length > 0) {
                if (commonStatisJs.CheckNull(self.textSearch)) {
                    self.listSearch = self.listAll.slice(0, 20);
                }
                else {
                    let txt = locdau(self.textSearch);
                    self.listSearch = self.listAll.filter(e =>
                        locdau(e.Text1).indexOf(txt) >= 0
                        || locdau(e.Text2).indexOf(txt) >= 0
                    );
                }
            }
            else {
                // lấy từ DB nếu không có điều kiện lọc nào (ex: from- to)
                if (!self.haveCondition) {
                    var self = this;
                    clearTimeout(self.timmer);
                    self.timmer = setTimeout(function () {
                        self.searchDB();
                    }, 300);
                    $(event.currentTarget).next().show();
                }
            }
        },
        searchDB: function () {
            var self = this;
            var txt = locdau(self.textSearch).trim();
            if (commonStatisJs.CheckNull(txt)) {
                self.listSearch = [];
                this.$emit('reset-item-chose');
                return;
            }
            let url = '';
            switch (parseInt(self.TypeData)) {
                case 1:
                    url = '/api/DanhMuc/DM_DoiTuongAPI/JqAuto_SearchDoiTuong?loaiDoiTuong=1'
                        + "&txtSearch=" + txt + '&idChiNhanh=' + self.ID_DonVi;
                    break;
                case 2:
                    url = '/api/DanhMuc/DM_DoiTuongAPI/JqAuto_SearchDoiTuong?loaiDoiTuong=2'
                        + "&txtSearch=" + txt + '&idChiNhanh=' + self.ID_DonVi;
                    break;
                case 3://todo
                    url = '/api/DanhMuc/DM_DoiTuongAPI/JqAuto_SearchDoiTuong';
                    break;
                case 4://todo
                    url = '/api/DanhMuc/DM_DoiTuongAPI/JqAuto_SearchDoiTuong';
                    break;
            }
            $.getJSON(url).done(function (data) {
                data = $.grep(data, function (x) {
                    return x.ID.indexOf('00000000') === -1;
                })
                data = data.map(function (item) {
                    return {
                        ID: item.ID,
                        Text1: item.SoDienThoai,
                        Text2: item.NguoiNopTien,
                        Text3: item.MaNguoiNop,
                    }
                })
                console.log('dt ', data)
                self.listSearch = data;
                self.listAfter = data;
            });
        },
        OnSelect: function (item) {
            var self = this;
            if (event !== undefined) {
                event.stopPropagation();
            }
            if (item === null) {
                // chose all
                self.listSearch = self.listAll;
                self.listAfter = self.listSearch;
                self.listChosed = [{ ID: null, Text1: '', Text2: 'Tất cả' }];
            }
            else {
                let arr = self.listChosed.map(function (x) { return x.ID });
                if ($.inArray(item.ID, arr) === -1) {
                    self.listChosed.push(item);
                    arr.push(item.ID)

                    let arrAfter = $.grep(self.listSearch, function (x) {
                        return $.inArray(x.ID, arr) === -1;
                    })
                    self.listAfter = arrAfter;
                }
                // remove item all
                self.listChosed = self.listChosed.filter(x => x.ID !== null);
            }
            this.$emit('on-select-item', self.listChosed);

            if (event !== undefined) {
                event.preventDefault();
            }
            return false;
        },
        RemoveItem: function (item) {
            var self = this;
            let arr = self.listAfter.map(function (x) { return x.ID });
            if (item.ID != null && $.inArray(item.ID, arr) === -1) {
                self.listAfter.unshift(item);
            }

            for (let i = 0; i < self.listChosed.length; i++) {
                if (self.listChosed[i].ID === item.ID) {
                    self.listChosed.splice(i, 1);
                    break;
                }
            }
            this.$emit('on-select-item', self.listChosed);
        },
        showModalAdd: function () {
            this.$emit('show-modal-add-group');
        },
    },
    computed: {
        lbl1: function () {
            if (commonStatisJs.CheckNull(this.typeData)) {
                return '';
            }
            let txt = '';
            switch (parseInt(this.typeData)) {
                case 1:
                case 2:
                    txt = 'SĐT:';
                    break;
            }
            return txt;
        },
        lbl2: function () {
            if (commonStatisJs.CheckNull(this.typeData)) {
                return '';
            }
            let txt = '';
            switch (parseInt(this.typeData)) {
                case 1:
                case 2:
                    txt = 'Tên:';
                    break;
            }
            return txt;
        },
    },
}


$(document).mouseup(function (e) {
    var container = $(".gara-search-dropbox");
    if (!container.is(e.target) && container.has(e.target).length === 0) {
        container.hide();
    }
});


