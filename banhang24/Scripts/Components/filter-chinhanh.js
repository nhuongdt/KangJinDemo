Vue.component('filter-chinhanh', {
    template: `<div class="dropdown op-tag-picker" id="dddonvi">
                    <div  data-toggle="dropdown" aria-expanded="false" >
                        <ul>
                            <li  v-for="item in listchinhanh.filter(p => p.CNChecked === true)">
                                <span>{{item.TenDonVi}}</span>&nbsp;
                                <span v-on:click="RemoveChiNhanh(item.ID, $event)">
                                    <i class="fa fa-times"></i>
                                </span>
                            </li>
                            <li  v-if="listchinhanh.filter(p => p.CNChecked === true).length === 0" style="width: 99% !important;text-align: center;">
                                <span>Tất cả</span>
                            </li>
                        </ul>

                    </div>
                    <ul class="dropdown-menu">
                            <li v-for="item in listchinhanh.filter(p => p.CNChecked === false)" v-on:click="SelectChiNhanh(item.ID, $event)">
                                    <a>
                                    {{item.TenDonVi}}
                                    </a>
                                </li>
                        </ul>
                    </div>`,
    props: ['listchinhanh'],
    methods:
    {
        SelectChiNhanh: function (idselect, event) {
            event.stopPropagation();
            let itemupdate = this.listchinhanh.find(p => p.ID === idselect);
            itemupdate.CNChecked = true;
            this.oncallLoadData();
            event.preventDefault();
            return false;
        },
        RemoveChiNhanh: function (idselect, event) {
            event.stopPropagation();
            let itemupdate = this.listchinhanh.find(p => p.ID === idselect);
            itemupdate.CNChecked = false;
            this.oncallLoadData();
            event.preventDefault();
            return false;
        },
        oncallLoadData: function () {
            this.$emit('callfunctionloaddata');
        }
    }
});