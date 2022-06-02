var cmpNguonKhach = {
    props: ['listAll', 'textSearch', 'listSearch', 'idChosing', 'showbutton'],
    template: `

            <div class="dropdown">
                    <div class="" data-toggle="dropdown"> 
                        <input class="form-control" placeholder="Chọn nguồn khách" style="padding-right: 27px!important"
                             v-model="textSearch" v-on:keyup="search" />
                    </div>

                    <button class="btn css-btn-right no-margin" v-on:click="showModalAdd" v-if="showbutton">
                                 <i class="material-icons">add</i>
                        </button>
                    <ul style="width:100%" class=" dropdown-menu">
                        <li v-on:click="OnSelect(null)">
                            <a> -- Chọn nguồn -- </a>
                        </li>
                        <li v-on:click="OnSelect(item)" v-for="(item, index) in listSearch">
                            <a class="flex flex-bettwen">
                            {{item.TenNguonKhach}}
                          
                            </a>
                        </li>
                    </ul>
                  
               
            </div>
`,
    methods: {
        showList: function () {
          /*  $(event.currentTarget).next().show();*/
        },
        search: function () {
            var self = this;
            if (commonStatisJs.CheckNull(self.textSearch)) {
                self.listSearch = self.listAll.slice(0, 20);
            }
            else {
                let txt = commonStatisJs.convertVieToEng(self.textSearch);
                self.listSearch = self.listAll.filter(e =>
                    commonStatisJs.convertVieToEng(e.TenNguonKhach).indexOf(txt) >= 0
                    || e.TenNguonKhach.indexOf(txt) >= 0
                );
            }
        },
        OnSelect: function (item) {
            this.$emit('on-select-item', item);
         /*   $(event.currentTarget).closest('div').hide();*/
        },
        showModalAdd: function () {
            this.$emit('show-modal-add');
        }
    }
};

var cmpTrangThaiKhach = {
    props: ['listAll', 'textSearch', 'listSearch', 'idChosing', 'showbutton'],
    template: `

    <div class="dropdown ">
        <input data-toggle="dropdown" class="form-control " placeholder="Chọn trạng thái" 
            v-model="textSearch" v-on:keyup="search" v-on:click="showList" />
        <button class="btn css-btn-right no-margin" v-on:click="showModalAdd" v-if="showbutton">
            <i class="material-icons">add</i>
        </button>
        <ul style="width:100%" class="dropdown-menu">
            <li v-on:click="OnSelect(null)">
                <a> -- Chọn trạng thái -- </a>
            </li>
            <li v-on:click="OnSelect(item)" v-for="(item, index) in listSearch">
                <a style="width:100%" class="gara-component-item-nv">
                    <span class="seach-hh">{{item.Name}}</span>
                    <span v-if="idChosing == item.ID" class="span-check"> 
                        <i class="fa fa-check">  </i> 
                    </span>
                </a>
            </li>
        </ul>
    </div>

`,
    methods: {
        showList: function () {
         /*   $(event.currentTarget).next().show();*/
        },
        search: function () {
            var self = this;
            if (commonStatisJs.CheckNull(self.textSearch)) {
                self.listSearch = self.listAll.slice(0, 20);
            }
            else {
                let txt = commonStatisJs.convertVieToEng(self.textSearch);
                self.listSearch = self.listAll.filter(e =>
                    commonStatisJs.convertVieToEng(e.Name).indexOf(txt) >= 0
                    || e.Name.indexOf(txt) >= 0
                );
            }
        },
        OnSelect: function (item) {
            this.$emit('on-select-item', item);
          /*  $(event.currentTarget).closest('div').hide();*/
        },
        showModalAdd: function () {
            this.$emit('show-modal-add');
        }
    }
}

var cmpNhomKhach = {
    props: ['listAll', 'listSearch', 'showbutton', 'listChosed'],
    template: `
      
                <div class="op-tag-picker dropdown nopadding">
                    <div class=" " data-toggle="dropdown" aria-expanded="false">
                        <input type="text" class="form-control"  placeholder="Chọn nhóm">
                        <ul >
                            <li class="" v-for="(item,index) in listChosed">
                                <span> {{item.TenNhomDoiTuong}}
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
                   <i class="material-icons">add</i>
                    </button>
                    <ul   class="dropdown-menu  "style="width:100%">
                            <li class="flex flex-between"style="width:100%" v-on:click="OnSelect(item)" v-for="(item,index) in listAll">
                               <a >  {{item.TenNhomDoiTuong}}</a>
                            </li>
                    </ul>
                   
                </div>
           

`,
    data: function () {
        return {
            listChosed: [],
            listAll: [],
        }
    },
    methods: {
        showList: function () {
           /* $(event.currentTarget).next().show();*/
        },
        OnSelect: function (item) {
            var self = this;
            var arr = self.listChosed.map(function (x) { return x.ID });
            if ($.inArray(item.ID, arr) === -1) {
                self.listChosed.push(item);
            }
            this.$emit('on-select-item', item, self.listChosed);
        /*    $(event.currentTarget).closest('div').hide();*/
        },
        RemoveItem: function (item) {
            var self = this;
            for (let i = 0; i < self.listChosed.length; i++) {
                if (self.listChosed[i].ID === item.ID) {
                    self.listChosed.splice(i, 1);
                    break;
                }
            }
            this.$emit('remove-item', self.listChosed);
        },
        showModalAdd: function () {
            this.$emit('show-modal-add-group');
        },
    }
}

var cmpNhomNCC = {
    props: ['listAll', 'textSearch', 'listSearch', 'idChosing', 'showbutton'],
    template: `

        <div class="dropdown">
            <input class="form-control" placeholder="Chọn nhóm" data-toggle="dropdown"
                                v-model="textSearch" 
                                v-on:click="showList"
                                v-on:keyup="search"
                                v-on:keyup.13="keyEnter"
                                v-on:keyup.up="keyUp"
                                v-on:keyup.down="keyDown"/>
            <button class="btn css-btn-right no-margin" v-on:click="showModalAdd" v-if="showbutton">
                <i class="material-icons">add</i>
            </button>
        
            <ul class="dropdown-menu" style="width:100%">
                <li v-on:click="OnSelect(null)">
                    <a> -- Nhóm mặc định -- </a>
                </li>
                    <li v-on:click="OnSelect(item)"
                        v-for="(item, index) in listSearch">
                    <a href="javascript:void(0)" class="gara-component-item-nv">
                        <span class="seach-hh">{{item.TenNhomDoiTuong}}</span>
                        <span v-if="idChosing == item.ID" class="span-check"> 
                                <i class="fa fa-check">  </i> 
                        </span>
                    </a>
                </li>
                </ul>
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
                    commonStatisJs.convertVieToEng(e.TenNhomDoiTuong).indexOf(txt) >= 0
                    || e.TenNhomDoiTuong.indexOf(txt) >= 0
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
            /*$(event.currentTarget).closest('div').hide();*/
        },
        showModalAdd: function () {
            this.$emit('show-modal-add-group');
        }
    }
}

