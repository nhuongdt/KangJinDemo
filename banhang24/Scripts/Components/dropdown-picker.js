Vue.component('dropdown-picker', {
    template: `<div class="gara-dropdown-picker">
                    <div class="gara-dropdown-picker-searchholder">
                        <i class="material-icons">search</i>
                        <input v-bind:placeholder="GetPlaceholder()" v-model="text" />
                    </div>
                    <ul class="gara-dropdown-picker-list">
                        <li class="dropdown-item">
                            <label><input type="checkbox" v-model="checkAll" />Chọn tất cả</label>
                        </li>
                        <li class="dropdown-item" v-for="item in FilterTextSearch()" v-on:click="ListClick">
                            <label><input type="checkbox" v-model="item.Checked"/>{{ item.Value1 !== null && item.Value1 !== '' && item.Value1 !== undefined ? item.Value1 + ' - ' : '' }}{{item.Value2}}</label>
                        </li>

                    </ul>

                </div>`,
    props: ['placeholder', 'listdata'],
    methods: {
        GetPlaceholder: function () {
            return this.placeholder;
        },
        FilterTextSearch: function () {
            return this.listdata.filter(p => commonStatisJs.convertVieToEng(p.Value1).match(commonStatisJs.convertVieToEng(this.text))
                || commonStatisJs.convertVieToEng(p.Value2).match(commonStatisJs.convertVieToEng(this.text)));
        },
        SelectAll: async function () {
            let self = this;
            if (self.setFromList === false) {
                if (self.listdata.filter(p => p.Checked === true).length < self.listdata.length) {
                    self.listdata.forEach(function (part, index, theArrray) {
                        theArrray[index].Checked = true;
                    });
                }
                else {
                    self.listdata.forEach(function (part, index, theArrray) {
                        theArrray[index].Checked = false;
                    });
                }
            }
        },
        ListClick: function () {
            let self = this;
            self.setFromList = false;
        }
    },
    data: function () {
        return {
            text: '',
            checkAll: true,
            setFromList: false
        }
    },
    watch: {
        checkAll: function (newVal, oldVal) {
            let self = this;
            self.SelectAll().then(function () {
                self.setFromList = false;
            });
        },
        listdata: {
            handler: function () {
                let self = this;
                if (self.listdata.length !== 0) {
                    if (self.listdata.filter(p => p.Checked === true).length === self.listdata.length) {
                        self.setFromList = true;
                        self.checkAll = true;
                    }
                    else {
                        self.setFromList = true;
                        self.checkAll = false;
                    }
                }
            },
            deep: true,
            immediate: true
        }
    }
});