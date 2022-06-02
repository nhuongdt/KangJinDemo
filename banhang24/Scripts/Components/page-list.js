Vue.component('page-list', {
    template: `<div class="page ">
                            <div class="flex flex-row">
                                <label>Số bản ghi:</label>
                                <select id="SelecttedPage" v-model="pagesize" class="form-control seleted-page">
                                    <option v-for="item in ListPageSize" v-bind:value="item">{{item}}</option>
                                </select>
                            </div>
                       <div class="flex flex-end">
                            <a href="javascript:void(0)" v-on:click="[isprev ? ButtonSelectPage(0,true): '']" v-bind:class="[!isprev ? 'a-disable' : '']"><i class="fa fa-step-backward" aria-hidden="true"></i></a>
                            <a href="javascript:void(0)" v-on:click="[isprev ? ButtonSelectPage(-1) : '']" v-bind:class="[!isprev ? 'a-disable' : '']"><i class="fa fa-caret-left" aria-hidden="true"></i></a>
                            <ul class="list-page">
                                <li v-for="(item, index) in listpage" v-show="listpage.length>0">
                                    <a href="javascript:void(0)" v-on:click="SelectPage(item)" v-bind:class="[item==currentpage ? 'click' : '']">{{item}}</a>
                                </li>
                            </ul>
                            <a href="javascript:void(0)" v-on:click="[isnext ? ButtonSelectPage(1): '']" v-bind:class="[!isnext ? 'a-disable' : '']"><i class="fa fa-caret-right" aria-hidden="true"></i></a>
                            <a href="javascript:void(0)" v-on:click="[isnext ? ButtonSelectPage(0,false): '']" v-bind:class="[!isnext ? 'a-disable' : '']"><i class="fa fa-step-forward" aria-hidden="true"></i></a>
                            <div class="total-recos" style="padding-right: 3px;">
                                {{pageview}}
                            </div>
                        </div>
                </div>`,
    props: ['pagesize', 'listpage', 'currentpage', 'pageview', 'numberofpage'],
    data: function () {
        return {
            ListPageSize: [10, 20, 30, 40, 50],
            //Numpage: this.numberofpage,
            isprev: false,
            isnext: false
        }
    },
    methods:
    {
        ButtonSelectPage: function (item, isfirstpage = null) {
            if (isfirstpage === null) {
                this.currentpage += item;
                if (this.currentpage <= 0) {
                    this.currentpage = 1;
                }
                else if (this.currentpage <= 0) {
                    this.currentpage = 1;
                }
            }
            else if (isfirstpage === true) {
                this.currentpage = 1;
            }
            else {
                this.currentpage = this.numberofpage;
            }
            this.CheckIsprevIsnext();
            this.LoadData();
        },
        SelectPage: function (item) {
            this.currentpage = item;
            this.CheckIsprevIsnext();
            this.LoadData();
        },
        CheckIsprevIsnext: function () {
            if (this.currentpage === 1 && this.numberofpage > 1) {
                this.isnext = true;
                this.isprev = false;
            }
            else if (this.currentpage === this.numberofpage && this.numberofpage > 1) {
                this.isnext = false;
                this.isprev = true;
            }
            else if (this.currentpage === 1 && this.numberofpage === 1) {
                this.isnext = false;
                this.isprev = false;
            }
            else if (this.numberofpage === 0) {
                this.isnext = false;
                this.isprev = false;
            }
            else {
                this.isnext = true;
                this.isprev = true;
            }
        },
        LoadData: function () {
            let currentPage = this.currentpage;
            let pageSize = this.pagesize;
            this.$emit('pageselected', { currentPage, pageSize });
        }
    },
    created: function () {
        this.CheckIsprevIsnext();
    },
    watch: {
        numberofpage: function (newval, oldval) {
            this.CheckIsprevIsnext();
        },
        pagesize: function () {
            this.LoadData();
        }
    }
});