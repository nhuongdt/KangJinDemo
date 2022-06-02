var ComponentSearchDate = {
    props: ['typetime', 'textdate', 'textdaterange', 'fromdate', 'todate', 'typetimeli'],// declare at parent
    //   v-on:input="ChangeTime($event.target.value)"
    //v-on:click="changetime($event)"
    template: `\\
<aside class="op-filter-container" >
    <div class="menuCheckbox">
        <div class="form-group floatleft">
            <div class="radio-menu" v-on:click="changetypetime($event)" >
                <input type="radio" name="rdoDate" value="0" v-model="typetime">
            </div>
            <div class="conten-choose">
                <div class=" choose-date ">
                    <div data-toggle="dropdown" aria-expanded="true">
                        <input type="text" class="dropdown form-control choose-date-show" placeholder="Tháng này"
                                v-model="textdate"
                                autocomplete="off"   
                                :disabled="typetime===1">

                    </div>
                    <div class="dropdown-menu menuleft_dlldate" >
                        <div class="col-md-4 col-sm-4">
                            <h3>Theo ngày và tuần</h3>
                            <ul>
                                <li value="1" v-on:click="changetime($event)"><a href="javascript:void(0);">Hôm nay</a></li>
                                <li value="2" v-on:click="changetime($event)"><a href="javascript:void(0);">Hôm qua</a></li>
                                <li value="3" v-on:click="changetime($event)"><a href="javascript:void(0);">Tuần này</a></li>
                                <li value="4" v-on:click="changetime($event)"><a href="javascript:void(0);">Tuần trước</a></li>
                            </ul>
                        </div>
                        <div class="col-md-4 col-sm-4">
                            <h3>Theo tháng và quý</h3>
                            <ul>
                                <li value="5" v-on:click="changetime($event)"><a href="javascript:void(0);">Tháng này</a></li>
                                <li value="6" v-on:click="changetime($event)"><a href="javascript:void(0);">Tháng trước</a></li>
                                <li value="7" v-on:click="changetime($event)"><a href="javascript:void(0);">Quý này</a></li>
                                <li value="8" v-on:click="changetime($event)"><a href="javascript:void(0);">Quý trước</a></li>
                            </ul>
                        </div><div class="col-md-4 col-sm-4">
                            <h3>Theo năm</h3>
                            <ul>
                                <li value="9" v-on:click="changetime($event)"><a href="javascript:void(0);">Năm này</a></li>
                                <li value="10" v-on:click="changetime($event)"><a href="javascript:void(0);">Năm trước</a></li>
                                <li value="0" v-on:click="changetime($event)"><a href="javascript:void(0);">Toàn thời gian</a></li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="form-group floatleft">
            <div class="radio-menu" >
                <input type="radio" name="rdoDate" value="1" v-model="typetime" />
            </div>
            <div class="conten-choose">
                <div class="floatleft form-wrap ">
                    <input type='text' class="form-control daterange" placeholder="Lựa chọn khác" name="daterange" id="menuleft-daterange" autocomplete="off"
                           v-model="textdaterange"
                           :disabled="typetime===0" />
                </div>
            </div>
        </div>
    </div>
</aside>
`,
    data: function () {
    },
   
    methods: {
        changetime(event) {
            var self = this;
            var $this = $(event.currentTarget);
            var val = parseInt($this.val());
            var text = $this.text();
            self.setvaluedate(val, text);
        },
        changetypetime(event) {
            var self = this;
            var $this = $(event.currentTarget);
            self.setvaluedate(self.typetimeli, self.textdate);
        },
        setvaluedate(val, text) {
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
                    var prevQuarter = (moment().quarter() - 1 === 0) ? 1 : moment().quarter() - 1;
                    fromdate = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                    todate = moment().quarter(prevQuarter).endOf('quarter').format('YYYY-MM-DD');
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
            var obj = { TypeTimeLi: val, FromDate: fromdate, ToDate: todate, Text: text };
            self.$emit('change-time-parent', obj);
        },
    },
};
$(function () {
    $('.daterange').daterangepicker({
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
});

