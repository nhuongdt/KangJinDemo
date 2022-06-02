var urlApiHT = '/api/DanhMuc/HT_API/';
Vue.component('modal-loading', {
    template: `<div class="op24-loading-bg" hidden> 
        <div class="op24-loading-box">
            <div class="op24-loading-obj">
                <img src="/Content/images/op-logo.png" class="op24-loading-img"></img>
                <div class="op24-loading-box-spiner"></div>
                <div class="op24-loading-box-text">{{textstatus}}</div>
                <div class="op24-loading-box-text">Quý khách vui lòng không tắt trình duyệt</div>
            </div>
        </div>
    </div>`,
    props: ['textstatus']
});

var VueModalLoading = new Vue({
    el: '#modalloading',
    data: {
        TextStatus: ''
    },
    methods: {
        ShowModal: function (text) {
            let self = this;
            self.TextStatus = text;
            $("#modalloading").fadeIn();
        },
        LoadTextStatus: async function (funcInput) {
            let self = this;
            let myData = {};
            let checkIntvl = 0;
            myData.ProgressName = funcInput;
            var intvl = setInterval(function () {
                if (checkIntvl === 1) {
                    clearInterval(intvl);
                    $.ajax({
                        traditional: true,
                        url: urlApiHT + 'ResetProcessText',
                        type: 'POST',
                        dataType: 'json',
                        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                        data: myData,
                        success: function (data) {
                            
                        }
                    });
                    self.HideModal();
                }
                else {
                    $.ajax({
                        traditional: true,
                        url: urlApiHT + 'GetProcessText',
                        type: 'POST',
                        dataType: 'json',
                        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                        data: myData,
                        success: function (data) {
                            if (data !== "") {
                                if (data !== "1") {
                                    self.TextStatus = data;
                                }
                                else {
                                    checkIntvl = 1;
                                }
                            }
                        }
                    });
                }
            }, 1000);
        },
        HideModal: function () {
            $("#modalloading").fadeOut();
        }
    }
});