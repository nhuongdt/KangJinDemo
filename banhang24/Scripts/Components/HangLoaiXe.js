//  hangxe
var componentListManufacturer = {
    props: ['ID', 'MaHangXe', 'TenHangXe'],
    template: `
    <li v-on:click="choseManufacturer">
        <a href="javascript:void(0)">
            <span class="tit-seach">{{MaHangXe}} </span>
            <span class="seach-hh">{{TenHangXe}}</span>
            <span></span>
        </a>
    </li>
`,
    methods: {
        choseManufacturer: function () {
            this.$emit('chose-manufacturer', this);
        }
    }
}
var ComponentChoseManufacturer = {
    props: {
        textSearch: { default: '' },
        showBtnAdd: { default: false },
        showBtnUpdate: { default: false },
        manufacturers: { default: [] },
        searchList: { default: [] },
    },
    components: {
        manufacturer: componentListManufacturer,
    },
    template: `
    <div class="gara-bill-infor-button shortlabel">
        <div v-on:click="showModalManufacturer" v-if="showBtnAdd">
            <a>
                <i class="material-icons">add</i>
            </a>
        </div>
        <input class="gara-search-HH" placeholder="Chọn hãng xe" v-model="textSearch" 
            v-on:click="showList" 
            v-on:keyup="searchManufacturer" 
            v-on:keyup.13="keyEnter"
            v-on:keyup.up="keyUp"
            v-on:keyup.down="keyDown"/>
        <div class="gara-search-dropbox drop-search">
            <ul>
                <manufacturer v-for="(item, index) in searchList"
                              v-bind:ma-hang-xe="item.MaHangXe"
                              v-bind:id = "item.ID"
                              v-bind:ten-hang-xe="item.TenHangXe"
                              v-bind:style="[indexFocus === index ? {'background':'#d9d9d9'} : {'background':'none'}]"
                              v-on:chose-manufacturer="ChangeManufacturer(item)"></manufacturer>
            </ul>
        </div>
    </div>
`,
    data: function () {
        return {
            textSearch: '',
            manufacturers: [],
            searchList: [],
            indexFocus: 0,
        }
    },
    created: function () {
        this.indexFocus = 0;
    },
    methods: {
        showList: function () {
            $(event.currentTarget).next().show();
        },
        ChangeManufacturer: function (item) {
            this.$emit('change-manufacturer-parent', item);
            $(event.currentTarget).closest('div').hide();
        },
        searchManufacturer: function () {
            var self = this;
            if (commonStatisJs.CheckNull(self.textSearch)) {
                self.searchList = self.manufacturers.slice(0, 20);
                this.$emit('reset-item-chose');
            }
            else {
                self.searchList = self.manufacturers.filter(e =>
                    commonStatisJs.convertVieToEng(e.MaHangXe).indexOf(commonStatisJs.convertVieToEng(self.textSearch)) >= 0
                    || commonStatisJs.convertVieToEng(e.TenHangXe).indexOf(commonStatisJs.convertVieToEng(self.textSearch)) >= 0
                    || e.TenHangXe.indexOf(commonStatisJs.convertVieToEng(self.textSearch)) >= 0
                );
            }
        },
        keyEnter: function () {
            let self = this;
            let chosing = $.grep(self.searchList, function (item, index) {
                return index === self.indexFocus;
            });
            if (chosing.length > 0) {
                this.$emit('change-manufacturer-parent', chosing[0]);
            }
            $(event.currentTarget).next().hide();
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
        showModalManufacturer: function () {
            this.$emit('show-modal-manufacture');
        }
    }
};

// mau xe
var componentListModelCar = {
    props: ['ID', 'TenMauXe'],
    template: `
    <li v-on:click="choseModelCar">
        <a href="javascript:void(0)">
            <span class="seach-hh">{{TenMauXe}}</span>
            <span></span>
        </a>
    </li>
`,
    methods: {
        choseModelCar: function () {
            this.$emit('chose-model-car', this);
        }
    }
}
var ComponentChoseModelCar = {
    props: {
        textSearch: { default: '' },
        modelName: { default: '' },
        showBtnAdd: { default: false },
        showBtnUpdate: { default: false },
        models: { default: [] },
        searchList: { default: [] },
    },
    components: {
        'modelcars': componentListModelCar,
    },

    template: `
    <div class="gara-bill-infor-button shortlabel">
        <div v-on:click="showModalModelCar" v-if="showBtnAdd">
            <a>
                <i class="material-icons">add</i>
            </a>
        </div>
        <div v-on:click="showModaUpdate" v-if="showBtnUpdate">
            <a>
                <i class="far fa-edit"></i>
            </a>
        </div>
        <input class="gara-search-HH " placeholder="Chọn mẫu xe" v-model="textSearch" 
v-on:blur="Reset_MauXe"
v-on:keyup="searchModelCar"
  v-on:click="showList" />
        <div class="gara-search-dropbox drop-search "
            v-bind:style = "[isShowList? {'display':'block'} : {'display':'none'}]"
            >
            <ul>
                <modelcars v-for="item in searchList"
                           v-bind:id="item.ID"
                           v-bind:ten-mau-xe="item.TenMauXe"
                           v-on:chose-model-car="ChangeModelCar(item)"></modelcars>
            </ul>
        </div>
    </div>
`,
    data: function () {
        return {
            textSearch: '',
            models: [],
            searchList: [],
            isShowList: false,
        }
    },
    methods: {
        showList: function () {
            $(event.currentTarget).next().show();
            this.isShowList = false;
        },
        searchModelCar: function () {
            var self = this;
            if (commonStatisJs.CheckNull(self.textSearch)) {
                self.searchList = self.models.slice(0, 20);
            }
            else {
                self.searchList = self.models.filter(e =>
                    commonStatisJs.convertVieToEng(e.TenMauXe).indexOf(commonStatisJs.convertVieToEng(self.textSearch)) >= 0
                    || e.TenMauXe.indexOf(commonStatisJs.convertVieToEng(self.textSearch)) >= 0
                );
            }
            self.isShowList = self.searchList.length > 0;
        },
        ChangeModelCar: function (item) {
            this.$emit('change-modelcar-parent', item);
            this.isShowList = false;
        },
        showModalModelCar: function () {
            this.$emit('show-modal-modelcar');
        },
        showModaUpdate: function () {
            this.$emit('show-modal-update');
        },
        Reset_MauXe: function () {
            var self = this;
            if (commonStatisJs.CheckNull(self.textSearch)) {
                this.$emit('reset-modelcar');
            }
        }
    }
};

// loai xe
var componentListTypeCar = {
    props: ['ID', 'MaLoaiXe', 'TenLoaiXe'],
    template: `
    <li v-on:click="choseTypeCar">
        <a href="javascript:void(0)">
            <span class="seach-hh">{{TenLoaiXe}}</span>
            <span></span>
        </a>
    </li>
`,
    methods: {
        choseTypeCar: function () {
            this.$emit('chose-model-car', this);
        }
    }
}
var ComponentChoseTypeCar = {
    props: {
        textSearch: { default: '' },
        indexFocus: { default: 0 },
        showBtnAdd: { default: false },
        showBtnUpdate: { default: false },
        searchList: { default: [] },
        types: { default: [] },
    },
    components: {
        'typecars': componentListTypeCar,
    },
    template: `
    <div class="gara-bill-infor-button shortlabel">
        <div v-on:click="showModalTypeCar" v-if="showBtnAdd">
            <a>
                <i class="material-icons">add</i>
            </a>
        </div>
         <div v-on:click="showModaUpdate" v-if="showBtnUpdate">
            <a>
                <i class="far fa-edit">add</i>
            </a>
        </div>
        <input class="gara-search-HH " placeholder="Chọn loại xe" v-model="textSearch" 
v-on:keyup="searchTypeCar" 
  v-on:click="showList" />
        <div class="gara-search-dropbox drop-search ">
            <ul>
                <typecars v-for="item in searchList"
                           v-bind:id="item.ID"
                           v-bind:ten-loai-xe="item.TenLoaiXe"
                           v-on:chose-model-car="ChangeTypeCar(item)"></modelcars>
            </ul>
        </div>
    </div>
`,
    data: function () {
        return {
            textSearch: '',
            searchList: [],
        }
    },
    created: function () {
        this.indexFocus = 0;
    },
    methods: {
        showList: function () {
            $(event.currentTarget).next().show();
        },
        searchTypeCar: function () {
            var self = this;
            if (commonStatisJs.CheckNull(self.textSearch)) {
                self.searchList = self.types.slice(0, 20);
                this.$emit('reset-item-chose');
            }
            else {
                self.searchList = self.types.filter(e =>
                    commonStatisJs.convertVieToEng(e.TenLoaiXe).indexOf(commonStatisJs.convertVieToEng(self.textSearch)) >= 0
                    || e.TenLoaiXe.indexOf(commonStatisJs.convertVieToEng(self.textSearch)) >= 0
                );
            }
        },
        ChangeTypeCar: function (item) {
            this.$emit('change-typecar-parent', item);
        },
        showModalTypeCar: function () {
            this.$emit('show-modal-cartype');
        },
        showModaUpdate: function () {
            this.$emit('show-modal-update');
        },
    }
};

// bienso xe
var componentCar = {
    props: ['ID', 'BienSo', 'IDMauXe'],
    template: `
    <li v-on:click="choseCar">
        <a href="javascript:void(0)">
            <span class="seach-hh">{{BienSo}}</span>
            <span></span>
        </a>
    </li>
`,
    methods: {
        choseCar: function () {
            this.$emit('chose-car', this);
        }
    }
}
var cmpChoseCar = {
    props: {
        textSearch: { default: '' },
        indexFocus: { default: 0 },
        showBtnAdd: { default: true },
        showBtnUpdate: { default: false },
        laHangHoa: { default: 2 },
        nguoisohuu: {default: 2}
    },
    components: {
        'cars': componentCar,
    },
    template: `
            <div class="gara-detail-input" style="position:relative">
                    <div class="gara-absolute-button" v-on:click="showModalNewCar" v-if="showBtnAdd">
                       <a class="gara-button-icon">
                       <i class="material-icons">add</i></a>
                    </div>   
                    <div class="gara-absolute-button" v-on:click="showModalUpdate" v-if="showBtnUpdate">
                       <a class="gara-button-icon">
                       <i class="far fa-edit"></i></a>
                    </div>
                    <input class="form-control gara-search-HH" 
                        placeholder="Tìm biển số"
                        v-on:blur="ResetCar"
                        v-on:click="showList"
                        v-model="textSearch" 
                        v-on:keyup="searchCar"/>
                        <div class="gara-search-dropbox drop-search"
                            v-bind:style= "[searchList.length > 0 && isShowList? {'display':'block'} : {'display':'none'}]"
                            >
                            <ul>
                              <cars v-for="(item, index) in searchList"
                                   v-bind:id="item.ID"
                                   v-bind:id-mau-xe="item.ID"
                                   v-bind:bien-so="item.BienSo"
                                   v-on:chose-car="ChangeCar(item)"></cars>                 
                            </ul>
                        </div>

            </div>
`,
    data: function () {
        return {
            textSearch: '',
            searchList: [],
            isShowList: false,
        }
    },
    created: function () {
        this.indexFocus = 0;
    },
    methods: {
        showList: function () {
            $(event.currentTarget).next().show();
            var self = this;
            self.isShowList = true;
        },
        searchCar: function () {
            var self = this;
            var txt = locdau(self.textSearch);
            $.getJSON("/api/DanhMuc/GaraAPI/" + "JqAuto_SearchXe?txt=" + txt
                + '&statusTN=1&idCustomer=&laHangHoa=' + self.laHangHoa + '&nguoisohuu=' + self.nguoisohuu).done(function (x) {
                    if (x.res) {
                        self.searchList = x.dataSoure;
                        self.isShowList = true;
                        if (x.dataSoure.length > 0) {
                            $(event.currentTarget).next().show();
                        }
                    }
                });
        },
        ChangeCar: function (item) {
            this.$emit('change-car-parent', item);
            this.isShowList = false;
            this.searchList = [];
        },
        showModalNewCar: function () {
            this.$emit('show-modal-car');
        },
        showModalUpdate: function () {
            this.$emit('show-modal-update');
        },
        ResetCar: function () {
            var self = this;
            if (commonStatisJs.CheckNull(self.textSearch)) {
                this.$emit('reset-car');
            }
        }
    },
};

$(document).mouseup(function (e) {
    var container = $(".gara-search-dropbox");
    if (!container.is(e.target) && container.has(e.target).length === 0) {
        container.hide();
    }
});