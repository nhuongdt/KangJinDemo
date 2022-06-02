Vue.component('page-list', {
    template: `<div class="page">
                  
                            <div class="flex flex-row">
                                <label>Số bản ghi:</label>
                                <select id="SelecttedPage" v-model="pagesize" class="form-control seleted-page">
                                    <option v-for="item in listpagesize" v-bind:value="item">{{item}}</option>
                                </select>
                            </div>
                       
                        <div class="flex flex-end">
                            <a href="javascript:void(0)" v-show="isprev" v-on:click="VuePageList.ButtonSelectPage(0,true)"><i class="fa fa-step-backward" aria-hidden="true"></i></a>
                            <a href="javascript:void(0)" v-show="isprev" v-on:click="VuePageList.ButtonSelectPage(-1)"><i class="fa fa-caret-left" aria-hidden="true"></i></a>
                            <a href="javascript:void(0)" v-show="!isprev" class="a-disable"><i class="fa fa-step-backward" aria-hidden="true"></i></a>
                            <a href="javascript:void(0)" v-show="!isprev" class="a-disable"><i class="fa fa-caret-left" aria-hidden="true"></i></a>
                            <ul class="list-page">
                                <li v-for="(item, index) in listpage" v-show="listpage.length>0">
                                    <a href="javascript:void(0)" v-on:click="VuePageList.SelectPage(item)" v-bind:class="[item==currentpage ? 'click' : '']">{{item}}</a>
                                </li>
                            </ul>
                            <a href="javascript:void(0)" v-show="!isnext" class="a-disable"><i class="fa fa-caret-right" aria-hidden="true"></i></a>
                            <a href="javascript:void(0)" v-show="!isnext" class="a-disable"><i class="fa fa-step-forward" aria-hidden="true"></i></a>
                            <a href="javascript:void(0)" v-show="isnext" v-on:click="VuePageList.ButtonSelectPage(1)"><i class="fa fa-caret-right" aria-hidden="true"></i></a>
                            <a href="javascript:void(0)" v-show="isnext" v-on:click="VuePageList.ButtonSelectPage(0,false)"><i class="fa fa-step-forward" aria-hidden="true"></i></a>
                            <div class="total-recos" style="    margin-top: 4px;">
                                {{pageview}}
                            </div>
                        </div>
                   
                </div>`,
    props: ['pagesize', 'listpagesize', 'isprev', 'isnext', 'listpage', 'currentpage', 'pageview']
});
var VuePageList = new Vue({
    el: '#PageList',
    data: {
        currentPage: 1,
        ListPage: [1],
        ListPageSize: [10, 20, 30, 40, 50],
        PageSize: 10,
        isprev: false,
        isnext: false,
        PageView: "",
        NumberOfPage: 1
    },
    methods: {
        ButtonSelectPage: function (item, isfirstpage = null) {
            
            if (isfirstpage === null) {
                this.currentPage += item;
                if (this.currentPage <= 0) {
                    this.currentPage = 1;
                }
                else if (this.currentPage <= 0) {
                    this.currentPage = 1;
                }
            }
            else if (isfirstpage === true) {
                this.currentPage = 1;
            }
            else {
                this.currentPage = this.NumberOfPage;
            }
            this.CheckIsprevIsnext();
            this.LoadData();
        },
        SelectPage: function (item) {
            this.currentPage = item;
            this.CheckIsprevIsnext();
            this.LoadData();
        },
        CheckIsprevIsnext: function () {
            if (this.currentPage === 1 && this.NumberOfPage > 1) {
                this.isnext = true;
                this.isprev = false;
            }
            else if (this.currentPage === this.NumberOfPage && this.NumberOfPage > 1) {
                this.isnext = false;
                this.isprev = true;
            }
            else if (this.currentPage === 1 && this.NumberOfPage === 1) {
                this.isnext = false;
                this.isprev = false;
            }
            else if (this.NumberOfPage === 0) {
                this.isnext = false;
                this.isprev = false;
            }
            else {
                this.isnext = true;
                this.isprev = true;
            }
        },
        LoadData: function () {
            switch (filterTarget) {
                case "TaiDuLieuMayChamCong":
                    VueDataMayChamCong.LoadDataFromDatabase();
                    break;
                default:
                    break;
            }
        }
    }
})