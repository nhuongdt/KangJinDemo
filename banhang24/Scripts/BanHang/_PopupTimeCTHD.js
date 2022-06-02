var popupTime = function () {
    var self = this;
    self.DateStart = ko.observable();
    self.TimeStart = ko.observable();
    self.DateEnd = ko.observable();
    self.TimeEnd = ko.observable();
    self.TotalTime = ko.observable();
    self.DichVu_isDoing = ko.observableArray();
    self.IsNhaHang = ko.observable(true);
    self.Stopping = ko.observable(false);

    function GetHours_andMinuteBetweenDate() {
        var start = self.DateStart().concat(' ', self.TimeStart());
        var end = self.DateEnd().concat(' ', self.TimeEnd());
        var diff = (new Date(end) - new Date(start)) / 1000;
        var hours = Math.floor(diff / 3600);
        var minutes = Math.floor(diff % 3600) / 60;
        return hours.toString().concat(' giờ ', minutes, ' phút');
    }

    self.ShowPopup = function (ctDoing, isNhaHang = true) {
        self.IsNhaHang(isNhaHang);
        self.Stopping(ctDoing.Stop);
        self.DichVu_isDoing(ctDoing);
        self.DateStart(moment(ctDoing.ThoiGian).format('YYYY-MM-DD'));
        self.DateEnd(moment(ctDoing.ThoiGianHoanThanh).format('YYYY-MM-DD'));
        self.TimeStart(moment(ctDoing.ThoiGian).format('HH:mm'));
        self.TimeEnd(moment(ctDoing.ThoiGianHoanThanh).format('HH:mm'));
        self.TotalTime(GetHours_andMinuteBetweenDate());
        $('#popup-timer').show();
    }

    self.Stop = function (stop) {
        self.Stopping(stop);
        var idRandomHD = self.DichVu_isDoing().IDRandomHD;
        var cacheName = 'listAllCTHD';
        if (self.IsNhaHang() === false) {
            cacheName = 'lstCTHDLe';
        }
        var cthd = localStorage.getItem(cacheName);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            for (let i = 0; i < cthd.length; i++) {
                if (cthd[i].IDRandom === self.DichVu_isDoing().IDRandom) {
                    cthd[i].Stop = stop;
                    if (stop) {
                        cthd[i].ThoiGianHoanThanh = moment(new Date()).format('YYYY-MM-DD HH:mm');
                    }
                    self.DichVu_isDoing(cthd[i]);
                    break;
                }
            }
            localStorage.setItem(cacheName, JSON.stringify(cthd));
            $('body').trigger('popuptime_ChangeTime');
        }
    }

    function UpdateThoiGian_CTHD() {
        var cacheName = 'listAllCTHD';
        if (self.IsNhaHang() === false) {
            cacheName = 'lstCTHDLe';
        }
        var cthd = localStorage.getItem(cacheName);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            for (let i = 0; i < cthd.length; i++) {
                if (cthd[i].IDRandom === self.DichVu_isDoing().IDRandom) {
                    cthd[i].ThoiGian = self.DateStart().concat(' ', self.TimeStart());
                    cthd[i].ThoiGianHoanThanh = self.DateEnd().concat(' ', self.TimeEnd());
                    let sogio = newModelBanLe.GetHoursBetweenDates_ToCurrent(cthd[i].ThoiGian, cthd[i].ThoiGianHoanThanh);
                    cthd[i].SoLuong = sogio;
                    cthd[i].ThanhTien = sogio * cthd[i].GiaBan;

                    self.TotalTime(GetHours_andMinuteBetweenDate());
                    self.DichVu_isDoing(cthd[i]);
                    break;
                }
            }
            console.log('cthd_changetime', cthd)
            localStorage.setItem(cacheName, JSON.stringify(cthd));
            $('body').trigger('popuptime_ChangeTime');
        }
    }

    // start = thoigian (db), end = thoigianhoanthanh(db)
    // type: 1.start, 2.end
    self.ChangeTime = function (type) {
        if (type === 2 && self.Stopping === false) {
            return;
        }
        var $this = event.currentTarget;
        $($this).datetimepicker({
            datepicker: false,
            step: 5,
            format: 'H:m',
            onSelectTime: function (e, evt) {
                var hour = e.getHours() > 9 ? e.getHours() : "0" + e.getHours();
                var min = e.getMinutes() > 9 ? e.getMinutes() : "0" + e.getMinutes();
                $(evt).val(hour + ":" + min);
                timeAfter = hour + ":" + min;
                console.log('timeAfter ', timeAfter, self.DateStart(), self.DateEnd());

                if (type === 1) {
                    self.TimeStart(timeAfter);
                }
                else {
                    self.TimeEnd(timeAfter);
                }

                if (self.DateStart() === self.DateEnd()) {
                    if (self.TimeStart() > self.TimeEnd()) {
                        ShowMessage_Danger('Thời gian bắt đầu không được lớn hơn thời gian kết thúc');
                        return;
                    }
                }

                let timeNow = moment(new Date()).format('HH:mm');
                let dateNow = moment(new Date()).format('YYYY-MM-DD');
                if (self.DateStart() === dateNow) {
                    if (self.TimeStart() > timeNow) {
                        ShowMessage_Danger('Thời gian kông được vượt quá thời gian hiện tại');
                        return;
                    }
                }

                UpdateThoiGian_CTHD();
                $($this).datetimepicker('destroy');
            }
        });
    }

    self.ChangeDate = function () {
        var dateS = $('#popup-timer .inarow:eq(0) .date-picker').val(); 
        var dateE = $('#popup-timer .inarow:eq(1) .date-picker').val();
        self.DateStart(moment(dateS, 'DD/MM/YYYY').format('YYYY-MM-DD'));
        self.DateEnd(moment(dateE, 'DD/MM/YYYY').format('YYYY-MM-DD'));

        console.log('ChangeDate ', self.DateStart(), self.DateEnd());
        if (self.DateStart() > self.DateEnd()) {
            ShowMessage_Danger('Ngày bắt đầu không được lớn hơn ngày kết thúc');
            return;
        }
        UpdateThoiGian_CTHD();
    }
}