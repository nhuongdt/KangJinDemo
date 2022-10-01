var cpmDatetime = {
    props: {
        dateChose: { default: '' },
        roleChangeDate: { default: true },
        format: {
            default: 'DD/MM/YYYY HH:mm'
        }
    },
    template: `<input class="form-control op-js-component-datetimepicker" 
            :disabled="!roleChangeDate"
            v-model="dateFormat" 
            v-on:click="formatDatetime" 
            > 
`,
    mounted: function () {
        let self = this;
        let typeFormat = 'd/m/Y H:i';
        switch (self.format) {
            case 'DD/MM/YYYY':
                typeFormat = 'd/m/Y';
                break;
        }
        $('.op-js-component-datetimepicker').datetimepicker(
            {
                format: typeFormat,
                defaultDate: new Date(),
                mask: true,
                scrollMonth: false,
                maxDate: new Date(),
            })
    },
    methods: {
        formatDatetime: function () {
            var self = this;
            let typeFormat = 'd/m/Y H:i';
            switch (self.format) {
                case 'DD/MM/YYYY':
                    typeFormat = 'd/m/Y';
                    break;
            }
            $(event.currentTarget).datetimepicker(
                {
                    format: typeFormat,
                    defaultDate: new Date(),
                    mask: true,
                    scrollMonth: false,
                    //onSelectDate: function (e) {
                    //    console.log('select date')
                    //    self.$emit('change-date', e);
                    //},
                    onChangeDateTime: function (dp, $input) {
                        if (!commonStatisJs.CheckNull(dp)) {
                            self.$emit('change-date', dp);
                        }
                    }
                })
        },
    },
    computed: {
        dateFormat: function () {
            let self = this;
            let formatFrom = '', formatTo = '';
            switch (self.format) {
                case 'DD/MM/YYYY':
                    formatFrom = 'YYYY-MM-DD';
                    formatTo = 'DD/MM/YYYY';
                    break;
                default:
                    formatFrom = 'YYYY-MM-DD HH:mm';
                    formatTo = 'DD/MM/YYYY HH:mm';
                    break;
            }
            var xx = self.dateChose !== null && self.dateChose !== '' ?
                moment(self.dateChose, formatFrom).format(formatTo) : '';
            return xx;
        }
    },
}

var cmpDateRange = {
    props: {
        id: { default: 'txtDaterange' },
        disable: { default: false },
        dateRange: { default: moment(new Date()).format('DD/MM/YYYY').concat(' - ', moment(new Date()).format('DD/MM/YYYY')) },
    },
    template: ` <input type="text" class="form-control" v-model="dateRange" :id="id" onclick="this.select()"/>`,
    mounted: function () {
        var self = this;
        var input = $('#' + this.id);
        input.daterangepicker({
            "opens": "right",
            "drop": "auto",
            locale: {
                "format": 'DD/MM/YYYY',
                "separator": " - ",
                "applyLabel": "Tìm kiếm",
                "cancelLabel": "Hủy",
                "fromLabel": "Từ",
                "toLabel": "Đến",
                "customRangeLabel": "Custom",
                "daysOfWeek": [
                    "CN",
                    "T2",
                    "T3",
                    "T4",
                    "T5",
                    "T6",
                    "T7"
                ],
                "monthNames": [
                    "Tháng 1",
                    "Tháng 2",
                    "Tháng 3",
                    "Tháng 4",
                    "Tháng 5",
                    "Tháng 6",
                    "Tháng 7",
                    "Tháng 8",
                    "Tháng 9",
                    "Tháng 10",
                    "Tháng 11",
                    "Tháng 12"
                ],
                "firstDay": 1
            }
        });
        input.on('apply.daterangepicker', function (ev, picker) {
            self.$emit('daterange-change', picker);
        });
    }
}

var cmpNgaySinh = {
    props: ['typeFormat', 'dateChose'],
    template: `
        <div class="form-news">
            <div class="choose-date-yaer">
                <div class="dropdown add-customer">
                    <input type='text' class="form-control" autocomplete="off" id="txtNgaySinh" v-model="dateChose" v-on:click="Innit" />
                    <button class="btn btn-input-right flex " style="justify-content:center" tabindex="-1" href="javascript:void(0)" data-toggle="dropdown" >
                        <i class="fa fa-caret-down" aria-hidden="true"></i>
                    </button>
                    <ul class="dropdown-menu floatleft" >
                        <li v-on:click="OnSelect(item)" v-for="(item,index) in listAll" >
                            <a style="background:transparent">
                                <span> {{item.Text}} </span>
                                <span v-if="typeFormat === item.Value" class="span-check"> 
                                        <i class="fa fa-check" style="display: block; color:var(--color-primary)">  </i>    
                                </span>
                            </a>
                         
                        </li>
                    </ul>
                </div>
            </div>
        </div>,
`,
    data: function () {
        return {
            listAll: [
                { Value: 'dd/MM/yyyy', Text: 'Theo ngày/tháng/năm' },
                { Value: 'dd/MM', Text: 'Theo ngày/tháng' },
                { Value: 'MM/yyyy', Text: 'Theo tháng/năm' },
                { Value: 'yyyy', Text: 'Theo năm' }
            ],
        }
    },
    methods: {
        Innit: function () {
            var $this = $(event.currentTarget);
            $this.datepicker({
                showOn: 'focus',
                altFormat: "dd/mm/yy",
                buttonImageOnly: true,
                dateFormat: "dd/mm/yy"
            });
        },
        OnSelect: function (item) {
            var self = this;
            self.typeFormat = item.Value;
            self.dateChose = null;

            var input = $(event.currentTarget).closest('div').find('input');
            input.datepicker("destroy");

            switch (item.Value) {
                case 'dd/MM':
                    input.mask('99/99').focus();
                    break;
                case 'MM/yyyy':
                    input.mask('99/9999').focus();
                    break;
                case 'yyyy':
                    input.mask('9999').focus();
                    break;
                default:
                    input.mask('99/99/9999').focus();
                    break;
            }
            self.$emit('on-select-item', item);
        }
    }
}

var cmpPrint = {
    props: ['idDonVi', 'listMauIn', 'MaChungTu', 'listData', 'rolePrint'],
    template: `
    <div class="btn-group dropdown-mauin" v-if="rolePrint">
         <button type="button" class="btn btn-main " v-on:click="Print(false, MaChungTu)">In</button> 
        <button type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true" 
         class="btn dropdown-toggle dropdown-toggle-split btn-main">
            <i class="fa fa-angle-down"></i>
        </button>
        <ul class="dropdown-menu">  
             <li v-for="(item, index) in listMauIn" >
                <a v-on:click="Print(true, item.Key)">
                {{item.Value}}
                </a>
             </li>
        </ul>
    </div>
`,
    methods: {
        Print: function (isPrintID, val) {
            var self = this;
            var url = '/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=' + val + '&idDonVi=' + self.idDonVi;
            if (isPrintID) {
                url = '/api/DanhMuc/ThietLapApi/GetContentFIlePrint?idMauIn=' + val;
            }
            var item1 = [], item2 = [], item3 = [], item4 = [], item5 = [];
            switch (self.MaChungTu) {
                case 'PTN':
                    item4 = self.listData.HangMucSuaChua ? self.listData.HangMucSuaChua : [];
                    item5 = self.listData.VatDungKemTheo ? self.listData.VatDungKemTheo : [];
                    break;
                default:
                    item1 = self.listData.ChiTietMua ? self.listData.ChiTietMua : [];
                    item2 = self.listData.ChiTietDoiTra ? self.listData.ChiTietDoiTra : [];
                    break;
            }
            item3 = self.listData.HoaDon ? $.extend({}, self.listData.HoaDon) : [];
            if (item3.NgayVaoXuong) {
                item3.NgayVaoXuong = moment(item3.NgayVaoXuong).format('DD/MM/YYYY HH:mm');
            }
            if (!commonStatisJs.CheckNull(item3.NgayXuatXuongDuKien)) {
                item3.NgayXuatXuongDuKien = moment(item3.NgayXuatXuongDuKien).format('DD/MM/YYYY HH:mm');
            }
            else {
                item3.NgayXuatXuongDuKien = '';
            }

            console.log(item1, item2, item3, item4, item5)

            ajaxHelper(url, 'GET').done(function (result) {
                var data = result;
                data = data.concat('<script src="/Scripts/knockout-3.4.2.js"></script>');
                data = data.concat("<script > var item1= ", JSON.stringify(item1), ';',
                    " var item2= ", JSON.stringify(item2), ' ;',
                    " var item3= ", JSON.stringify(item3), ' ;',
                    " var item4= ", JSON.stringify(item4), ' ;',
                    " var item5= ", JSON.stringify(item5), ' ;',
                    " </script>");
                data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                PrintExtraReport(data); // assign content HTML into frame
            })
        }
    }
}
