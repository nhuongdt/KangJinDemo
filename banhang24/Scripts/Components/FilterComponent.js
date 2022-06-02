//Create component dropdown-donvi
var urlApiChiNhanh = '/api/DanhMuc/DM_DonViAPI/'
Vue.component('dropdown-donvi', {
    template: `<div class="dropdown op-tag-picker" id="dddonvi">
                    <div  data-toggle="dropdown" aria-expanded="false" >
                        <ul>
                            <li  v-for="item in listdataselected">
                                <span>{{item.TenDonVi}}</span>&nbsp;
                                <span v-on:click="VueChiNhanh.RemoveChiNhanh(item.ID, $event)">
                                    <i class="fa fa-times"></i>
                                </span>
                            </li>
                        </ul>

                    </div>
                   
                        <ul class=" dropdown-menu">
                            <li v-for="item in listdata" v-on:click="VueChiNhanh.SelectChiNhanh(item.ID, $event)">{{item.TenDonVi}}</li>
                        </ul>
                   
                </div>`,
    props: ['listdataselected', 'listdata']
});

var VueChiNhanh = new Vue({
    el: '#vFilterChiNhanh',
    data: {
        databind: {
            data: []
        }
    },
    methods: {
        LoadChiNhanh: function () {
            let self = this;
            $.ajax({
                url: urlApiChiNhanh + "GetListDonVi_User",
                type: 'GET',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    if (data !== null) {
                        data.map(function (item) {
                            if (item['ID'] === $('#hd_IDdDonVi').val()) {
                                item['CNChecked'] = true;
                            }
                            else {
                                item['CNChecked'] = false;
                            }
                        });
                        self.databind.data = data;
                    }
                    else {
                        commonStatisJs.ShowMessageDanger("Có lỗi xảy ra trong quá trình tải dữ liệu. Vui lòng kiểm tra lại.");
                    }
                }
            });
        },
        SelectChiNhanh: function (idselect, event) {
            event.stopPropagation();
            let self = this;
            let itemupdate = self.databind.data.find(p => p.ID === idselect);
            itemupdate.CNChecked = true;
            event.preventDefault();
            return false;
        },
        RemoveChiNhanh: function (idselect, event) {
            event.stopPropagation();
            let self = this;
            let itemupdate = self.databind.data.find(p => p.ID === idselect);
            itemupdate.CNChecked = false;
            event.preventDefault();
            SelectFilterTarget();
            return false;
        }
    }
});
VueChiNhanh.LoadChiNhanh();

//Dropdown close event
$('#dddonvi').on('hide.bs.dropdown', function () {
    SelectFilterTarget();
});

function SelectFilterTarget() {
    switch (filterTarget) {
        case "QuanLyMayChamCong":
            VMayChamCong.LoadMayChamCong();
            break;
        case "TaiDuLieuMayChamCong":
            VueMayChamCong.LoadMayChamCong();
            break;
    }
};