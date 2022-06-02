Vue.component('filter-loaixe', {
    template: `<div class="outselect" id="dddonvi">
                    <div class="choose-person floatleft " data-toggle="dropdown" aria-expanded="false" >
                        <ul>
                            <li class="li-input-span" v-for="item in listloaixe.filter(p => p.Checked === true)">
                                <span>{{item.TenLoaiXe}}</span>&nbsp;
                                <span v-on:click="RemoveLoaiXe(item.ID, $event)">
                                    <i class="fa fa-times"></i>
                                </span>
                            </li>
                        </ul>

                    </div>
                    <div class="selec-person dropdown-menu">
                        <ul>
                            <li v-for="item in listloaixe.filter(p => p.Checked === false)" v-on:click="SelectLoaiXe(item.ID, $event)">{{item.TenLoaiXe}}</li>
                        </ul>
                    </div>
                </div>`,
    props: ['listloaixe'],
    methods:
    {
        SelectLoaiXe: function (idselect, event) {
            event.stopPropagation();
            let itemupdate = this.listloaixe.find(p => p.ID === idselect);
            itemupdate.Checked = true;
            this.oncallLoadData();
            event.preventDefault();
            return false;
        },
        RemoveLoaiXe: function (idselect, event) {
            event.stopPropagation();
            let itemupdate = this.listloaixe.find(p => p.ID === idselect);
            itemupdate.Checked = false;
            this.oncallLoadData();
            event.preventDefault();
            return false;
        },
        oncallLoadData: function () {
            this.$emit('callfunctionloaddata');
        }
    }
});