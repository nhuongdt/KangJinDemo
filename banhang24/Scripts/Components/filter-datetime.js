Vue.component('filter-datetime', {
    props: {
        typetime: { default: 0 },
        radioname: { default: 'rdo' },
        selectvalue: { default: '3' },
        showThreeChose: { default: false },
    },
    template: `
<aside class="op-filter-container" >
    <div class="menuCheckbox">
        <div class="form-group floatleft">
            <div class="radio-menu">
                <input type="radio" v-bind:name="radioname" value="0" v-model="typetime" v-on:click="changetypetime($event)">
            </div>
            <div class="conten-choose">
                <div class=" choose-date ">
                    <div data-toggle="dropdown" v-bind:style="[typetime == 1 ? {'pointer-events':'none'} : '']" aria-expanded="true">
                        <input type="text" class="dropdown form-control choose-date-show" placeholder="Tháng này"
                                v-bind:value="showtext()"
                                autocomplete="off"   
                                :disabled="typetime==1">

                    </div>
                    <div class="dropdown-menu menuleft_dlldate" >
                        <div class="col-md-4 col-sm-4">
                            <h3>Theo ngày và tuần</h3>
                            <ul>
                                <li v-for="itemTuan in Tuan" v-bind:value="itemTuan.value" v-on:click="changetime($event)"><a href="javascript:void(0);">{{ itemTuan.text }}</a></li>
                            </ul>
                        </div>
                        <div class="col-md-4 col-sm-4">
                            <h3>Theo tháng và quý</h3>
                            <ul>
                                <li v-for="itemThang in Thang" v-bind:value="itemThang.value" v-on:click="changetime($event)"><a href="javascript:void(0);">{{ itemThang.text }}</a></li>
                            </ul>
                        </div><div class="col-md-4 col-sm-4">
                            <h3>Theo năm</h3>
                            <ul>
                                <li v-for="itemNam in Nam" v-bind:value="itemNam.value" v-on:click="changetime($event)"><a href="javascript:void(0);">{{ itemNam.text }}</a></li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="form-group floatleft">
            <div class="radio-menu" >
                <input type="radio" v-bind:name="radioname" value="1" v-model="typetime" v-on:click="changetypetime($event)"/>
            </div>
            <div class="conten-choose" v-bind:style="[typetime == 0 ? {'pointer-events':'none'} : '']">
                <div class="floatleft form-wrap ">
                    <input type='text' class="form-control daterange" placeholder="Lựa chọn khác" v-bind:name="radioname" v-bind:id="radioname + '-menuleft-daterange'" autocomplete="off"
                           :disabled="typetime==0"/>
                </div>
            </div>
        </div>
            
        <div class="form-group floatleft" v-if="showThreeChose">
            <div class="radio-menu" >
                <input type="radio" v-bind:name="radioname" value="2" v-model="typetime" v-on:click="changetypetime($event)"/>
            </div>
            <div class="conten-choose" v-bind:style="[typetime == 0 ? {'pointer-events':'none'} : '']">
                <div class="floatleft form-wrap ">
                    <input type='text' class="form-control op-js-component-datetimepicker" 
                                        autocomplete="off"
                             title="Đến ngày" placeholder="Đến ngày" 
                            v-bind:name="radioname"  
                           :disabled="typetime==0"
                           v-model="dateFormat" 
                            v-on:click="formatDatetime"/>
                </div>
            </div>
        </div>

    </div>
</aside>
`,
    data: function () {
        return {
            Tuan: [{ value: 1, text: 'Hôm nay' }, { value: 2, text: 'Hôm qua' }, { value: 3, text: 'Tuần này' }, { value: 4, text: 'Tuần trước' }],
            Thang: [{ value: 5, text: 'Tháng này' }, { value: 6, text: 'Tháng trước' }, { value: 7, text: 'Quý này' }, { value: 8, text: 'Quý trước' }],
            Nam: [{ value: 9, text: 'Năm nay' }, { value: 10, text: 'Năm trước' }, { value: 0, text: 'Toàn thời gian' }],
            SelectedType: this.selectvalue,
            SelectTypetime: this.typetime,
            dateFormat: moment(new Date()).format('DD/MM/YYYY')
        }
    },

    methods: {
        showtext: function () {
            var selTuan = this.Tuan.find(p => p.value === this.SelectedType);
            if (selTuan !== undefined) {
                return selTuan.text;
            }
            else {
                var selThang = this.Thang.find(p => p.value === this.SelectedType);
                if (selThang !== undefined) {
                    return selThang.text;
                }
                else {
                    var selNam = this.Nam.find(p => p.value === this.SelectedType);
                    if (selNam !== undefined) {
                        return selNam.text;
                    }
                    else {
                        return "undefined";
                    }
                }
            }

        },
        changetime(event) {
            var self = this;
            var $this = $(event.currentTarget);
            var val = parseInt($this.val());
            if (self.SelectedType !== val) {
                self.SelectedType = val;
                if (self.SelectTypetime === 0) {
                    self.setvaluedate(self.SelectedType);
                }
            }

        },
        changetypetime(event) {
            var self = this;
            var $this = $(event.currentTarget);
            var val = parseInt($this.val());
            if (self.SelectTypetime !== val) {
                self.SelectTypetime = val;

                switch (self.SelectTypetime) {
                    case 0:// ngay, thang, nam
                        self.setvaluedate(self.SelectedType);
                        break;
                    case 2:// denngay
                        let obj = {
                            fromdate: '2016-01-01',
                            todate: moment(self.dateFormat, 'DD/MM/YYYY').add('days',1).format('YYYY-MM-DD'),
                            radioselect: val,
                        }
                        self.$emit('callfunction', obj);
                        break;
                }
            }
        },
        setvaluedate(val) {
            var self = this;
            var _now = new Date();
            var fromdate = '';
            var todate = '';

            switch (val) {
                case 0:
                    var nextYear = moment().year() + 1;
                    fromdate = '2016-01-01';
                    todate = moment().year(nextYear).endOf('year').format('YYYY-MM-DD');
                    break;
                case 1:
                    fromdate = moment(_now).format('YYYY-MM-DD');
                    todate = fromdate;
                    break;
                case 2:
                    fromdate = moment(_now).add(-1, 'days').format('YYYY-MM-DD');
                    todate = fromdate;
                    break;
                case 3:
                    fromdate = moment().startOf('week').add(1, 'days').format('YYYY-MM-DD');
                    todate = moment().endOf('week').add(1, 'days').format('YYYY-MM-DD');
                    break;
                case 4:
                    fromdate = moment().startOf('week').subtract(6, 'days').format('YYYY-MM-DD');
                    todate = moment().startOf('week').format('YYYY-MM-DD');
                    break;
                case 5:
                    fromdate = moment().startOf('month').format('YYYY-MM-DD');
                    todate = moment().endOf('month').format('YYYY-MM-DD');
                    break;
                case 6:
                    fromdate = moment().subtract(1, 'months').startOf('month').format('YYYY-MM-DD');
                    todate = moment().subtract(1, 'months').endOf('month').format('YYYY-MM-DD');
                    break;
                case 7:
                    fromdate = moment().startOf('quarter').format('YYYY-MM-DD');
                    todate = moment().endOf('quarter').format('YYYY-MM-DD');
                    break;
                case 8://quy truoc
                    var prevQuarter = moment().quarter() - 1;
                    if (prevQuarter === 0) {
                        fromdate = moment().year(moment().year() - 1).quarter(4).startOf('quarter').format('YYYY-MM-DD');
                        todate = moment().year(moment().year() - 1).quarter(4).endOf('quarter').format('YYYY-MM-DD');
                    }
                    else {
                        fromdate = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                        todate = moment().quarter(prevQuarter).endOf('quarter').format('YYYY-MM-DD');
                    }
                    break;
                case 9://nam nay
                    fromdate = moment().startOf('year').format('YYYY-MM-DD');
                    todate = moment().endOf('year').format('YYYY-MM-DD');
                    break;
                case 10:// nam truoc
                    var prevYear = moment().year() - 1;
                    fromdate = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                    todate = moment().year(prevYear).endOf('year').format('YYYY-MM-DD');
                    break;
            }
            todate = moment(todate).add('days', 1).format('YYYY-MM-DD');
            var radioselect = self.SelectTypetime;
            self.$emit('callfunction', { fromdate, todate, radioselect });
        },
        formatDatetime: function () {
            var self = this;
            $(event.currentTarget).datetimepicker(
                {
                    format: 'd/m/Y',
                    defaultDate: new Date(),
                    mask: true,
                    scrollMonth: false,
                    onChangeDateTime: function (dp, $input) {
                        if (!commonStatisJs.CheckNull(dp)) {
                            self.dateFormat = moment(dp).format('DD/MM/YYYY');
                            self.$emit('change-date', dp);
                        }
                    }
                })
        },
    },
    created: function () {
        this.setvaluedate(this.selectvalue);
    },
    mounted: function () {
        var self = this;
        var input = $('input[name="' + this.radioname + '"]');
        input.on('apply.daterangepicker', function (ev, picker) {
            var fromdate = picker.startDate.format('YYYY-MM-DD');
            var todate = picker.endDate.format('YYYY-MM-DD');
            todate = moment(todate).add('days', 1).format('YYYY-MM-DD');
            var radioselect = self.SelectTypetime;
            self.$emit('callfunction', { fromdate, todate, radioselect });
        });

        $('.op-js-component-datetimepicker').datetimepicker(
            {
                format: 'd/m/Y',
                defaultDate: new Date(),
                mask: true,
                scrollMonth: false,
                maxDate: new Date(),
            })
    },
    watch: {
        typetime: function () {
            let self = this;
            self.SelectTypetime = self.typetime;
        }
    }
});


