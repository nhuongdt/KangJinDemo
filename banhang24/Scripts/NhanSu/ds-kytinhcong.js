var vmKyTinhCong = new Vue({
    el: '#KyTinhCong',
    data: {
        databind: {
            pageview: '',
            pagenow: 1,
            data: [],
            listpage: [],
            isprev: false,
            isnext: false,
            countpage: 0,
        },
        curentpage: {
            text: '',
            tungay: '',
            denngay: '',
            typetime: 5,
            typetimeold: 5,
            TrangThai: ['1'],
            pagesize: 10,
            order: null,
            sort: false,
            pagenow: 1,
            textdate: 'Tháng này',
            ChiNhanh: [],
            LoaiCa: []
        },
        Key_Form: "KeyFormKyTinhCong",
        listpagesize: [10, 20, 30],
        listcolumn: [],
        datachitiet: {
            KyHieuCong:[],
            NgayNghiLe: []
        },
        objectChotCong: {},
        objectFilter: {
            ThoiGian: true,
            TrangThai: true
        }
    },
    methods: {
        GetForSearchKyTinhCong: function (resetpage = false) {
            var self = this;
            $('#table-reponsive').gridLoader();
            if (resetpage) {
                self.curentpage.pagenow = 1;
            }
            var model = {
                Text: self.curentpage.text,
                TuNgay: self.curentpage.tungay,
                DenNgay: self.curentpage.denngay,
                TypeTime: self.curentpage.typetime,
                pageSize: self.curentpage.pagesize,
                pageNow: self.curentpage.pagenow,
                TrangThai: self.curentpage.TrangThai
            }
            $.ajax({
                data: model,
                url: "/api/DanhMuc/NS_NhanSuAPI/GetForSearchKyTinhCong",
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    $('#table-reponsive').gridLoader({ show: false });
                    if (data.res === true) {
                        self.databind = data.dataSoure;
                        self.curentpage.pagenow = self.databind.pagenow;
                        //commonStatisJs.sleep(100).then(() => {
                        //    self.LoadFirst();
                        //});
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(data.mess);
                    }
                },
                error: function (result) {
                    $('#table-reponsive').gridLoader({ show: false });
                    console.log(result);
                }
            });
        },
        keymonitor: function (event) {
            if (event.key === "Enter") {
                this.GetForSearchKyTinhCong(true);
            }
        },
        SelectPageSize: function (e) {
            this.GetForSearchKyTinhCong(true);
        },
        AddTrangThai: function (ev) {
            var value = $(ev.target).val();
            if (this.curentpage.TrangThai.some(o => o === value)) {
                this.curentpage.TrangThai = this.curentpage.TrangThai.filter(o => o !== value);
            }
            else {
                this.curentpage.TrangThai.push(value);
            }
            this.GetForSearchKyTinhCong(true);
        },
        SelectPage: function (item) {
            this.curentpage.pagenow = item;
            this.GetForSearchKyTinhCong();
        },
        ButtonSelectPage: function (item, isfirstpage = null) {
            if (isfirstpage === null) {
                this.curentpage.pagenow += item;
                if (this.curentpage.pagenow <= 0) {
                    this.curentpage.pagenow = 1;
                }
                else if (this.curentpage.pagenow <= 0) {
                    this.curentpage.pagenow = 1;
                }
            }
            else if (isfirstpage === true) {
                this.curentpage.pagenow = 1;
            }
            else {
                this.curentpage.pagenow = this.databind.countpage;
            }
            this.GetForSearchKyTinhCong();
        },
        SelectRow: function (item, event) {
            var self = this;
            var heigth = 0;
            var heightold = 0;
            var setTop = 0;
            var $this = $(event.target).closest('tr');
            var css = $this.next(".op-tr-hide").css('display');
            $(".op-tr-hide").hide();
            $(".op-tr-show").removeClass('tr-active');
            setTop = 34 + parseInt($this.height() * ($this.index() / 2));
            $('.line-right').height(0).css("margin-top", "0px");
            if (css === 'none') {
                $this.addClass('tr-active');
                $this.next(".op-tr-hide").toggle();

                $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetChiTietKyTinhCong?id=" + item.ID, function (data) {
                    if (data.res) {
                        self.datachitiet = data.dataSoure;

                        commonStatisJs.sleep(100).then(() => {

                            heightold = $this.next().height();
                            heigth = parseInt($this.height()) + heightold;
                            $('.line-right').height(heigth).css("margin-top", setTop + "px");
                            $this.closest('tbody').closest('table').closest('.table-reponsive').removeClass('tablescroll');
                        });
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(data.mess);
                    }
                });

            }
            else {
                if (!$this.closest('tbody').closest('table').closest('.table-reponsive').hasClass('tablescroll')) {
                    $this.closest('tbody').closest('table').closest('.table-reponsive').addClass('tablescroll');
                }
            }
        },
        AddNewKyTinhCong: function () {

            
            vmEditKyTinhCong.AddNew();
        },
        Edit: function (item) {
            vmEditKyTinhCong.Edit(commonStatisJs.CopyObject(item));
        },
        Delete: function (item) {
            vmEditKyTinhCong.Delete(item);
        },
        ChotKyTinhCong: function (item) {
            this.objectChotCong = item;
            $('#modalChotKyTinhCong').modal('show');
        },
        SaveChotKyTinhCong: function () {
            var self = this;
            $('#table-reponsive').gridLoader();
            $.ajax({
                data: self.objectChotCong,
                url: "/api/DanhMuc/NS_NhanSuAPI/ChotKyTinhCong?" + "ID_DonVi=" + $('#hd_IDdDonVi').val() + "&ID_NhanVien=" + $('.idnhanvien').text() ,
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    $('#table-reponsive').gridLoader({ show: false });
                    if (data.res === true) {
                        commonStatisJs.ShowMessageSuccess(data.mess);
                        self.GetForSearchKyTinhCong();
                        $('#modalChotKyTinhCong').modal('hide');
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(data.mess);
                    }
                },
                error: function (result) {
                    $('#table-reponsive').gridLoader({ show: false });
                    console.log(result);
                }
            });
        },
        MoKyTinhCong: function (item) {
            var self = this;
            $('#table-reponsive').gridLoader();
            $.ajax({
                data: item,
                url: "/api/DanhMuc/NS_NhanSuAPI/MoKyTinhCong?" + "ID_DonVi=" + $('#hd_IDdDonVi').val() + "&ID_NhanVien=" + $('.idnhanvien').text(),
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    $('#table-reponsive').gridLoader({ show: false });
                    if (data.res === true) {
                        commonStatisJs.ShowMessageSuccess(data.mess);
                        self.GetForSearchKyTinhCong();
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(data.mess);
                    }
                }
            });
        }
    },
    computed: {
        
    },
});
$('.newDateTime').on('apply.daterangepicker', function (ev, picker) {
    vmKyTinhCong.curentpage.tungay = picker.startDate.format('MM/DD/YYYY');
    vmKyTinhCong.curentpage.denngay = picker.endDate.format('MM/DD/YYYY');
    vmKyTinhCong.GetForSearchKyTinhCong(true);
});
$('#SinhNhatSL ').on('click', '.radio-menu input[type="radio"]', function () {
    var dataid = $(this).data('id');
    $('#SinhNhatSL .form-group').each(function () {
        $(this).find('.conten-choose').find('input').removeAttr('disabled');
        if (dataid !== $(this).find('.radio-menu').find('input').data('id')) {
            $(this).find('.conten-choose').find('input').attr('disabled', 'disabled');
        }
    });
    if ($(this).data('id') !== 1) {
        vmKyTinhCong.curentpage.typetime = null;
    }
    else {
        vmKyTinhCong.curentpage.typetime = vmKyTinhCong.curentpage.typetimeold;
    }
    vmKyTinhCong.GetForSearchKyTinhCong(true);
});
$('#SelectNgaySinh').on('click', 'ul li', function () {
    vmKyTinhCong.curentpage.textdate = $(this).find('a').text();
    vmKyTinhCong.curentpage.typetime = $(this).val();
    vmKyTinhCong.curentpage.typetimeold = $(this).val();
    vmKyTinhCong.GetForSearchKyTinhCong(true);
});
$('body').on('AddKyTinhCongSucces', function () {
    vmKyTinhCong.GetForSearchKyTinhCong(true);
});
vmKyTinhCong.GetForSearchKyTinhCong(true);