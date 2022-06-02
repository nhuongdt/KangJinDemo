var vmLoaiBaoHiem = new Vue({
    el: '#vLoaiBaoHiem',
    data: {
        isNew: true,
        loadding: false,
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
            TrangThai: [],
            pagesize: 10,
            pagenow: 1,
            textdate: 'Tháng này',
            chinhanhid: $('#hd_IDdDonVi').val(),
        },
        Key_Form: "KeyFormLoaiBaoHiem",
        listpagesize: [10, 20, 30],
        listdata: {
            ChiNhanh: [],
            PhongBan: [],
            Column: [],
            KyTinhCong: []
        },
        ClickFilter: {
            ThoiGian: true,
            TrangThai: true
        },
        role: {
            PheDuyet: false,
            NhanSu: false,
        }
    },
    methods: {
      

        // Lấy dữ liệu mặc định
        GetListData: function () {
            var self = this;
            self.GetForSearchLoaiBaoHiem(true);
        },

        // Tìm kiếm danh sách bảng lương
        GetForSearchLoaiBaoHiem: function (resetpage = false) {
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
                TrangThai: self.curentpage.TrangThai,
                IDNhanVien: $('.idnhanvien').text(),
            }
            var url = "/api/DanhMuc/NS_NhanSuAPI/GetSearchForLoaiBaoHiem";
            $.ajax({
                data: model,
                url: url,
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    $('#table-reponsive').gridLoader({ show: false });
                    if (data.res === true) {
                        self.databind = data.dataSoure;
                        self.curentpage.pagenow = self.databind.pagenow;
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

        

      

        //---------tìm kiếm
        AddTrangThai: function (ev) {
            var value = $(ev.target).val();
            if (this.curentpage.TrangThai.some(o => o === value)) {
                this.curentpage.TrangThai = this.curentpage.TrangThai.filter(o => o !== value);
            }
            else {
                this.curentpage.TrangThai.push(value);
            }
            this.GetForSearchLoaiBaoHiem(true);
        },

        keymonitor: function (event) {
            if (event.key === "Enter") {
                this.GetForSearchLoaiBaoHiem(true);
            }
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
            this.GetForSearchLoaiBaoHiem();
        },

    
        SelectPage: function (item) {
            this.curentpage.pagenow = item;
            this.GetForSearchLoaiBaoHiem();
        },

        SelectPageSize: function (e) {
            this.GetForSearchLoaiBaoHiem(true);
        },
        
        // ------- end tìm kiếm

        Insert:function () {
            vmEditLoaiBaoHiem.AddNew();
        },
        Edit: function (item) {
            vmEditLoaiBaoHiem.Edit(commonStatisJs.CopyObject(item));
         },
          Delete: function (item) {
              vmEditLoaiBaoHiem.Delete(item);
          }
       
    },
    computed: {
        TitlePopup: function () {
            if (this.isNew) {

                return "Thêm mới loại bảo hiểm";
            }
            else {

                return "Cập nhật loại bảo hiểm";
            }
        }
    },
});
vmLoaiBaoHiem.GetListData();
$('.newDateTime').on('apply.daterangepicker', function (ev, picker) {
    vmLoaiBaoHiem.curentpage.tungay = picker.startDate.format('MM/DD/YYYY');
    vmLoaiBaoHiem.curentpage.denngay = picker.endDate.format('MM/DD/YYYY');
    vmLoaiBaoHiem.GetForSearchLoaiBaoHiem(true);
})
$('#SinhNhatSL ').on('click', '.radio-menu input[type="radio"]', function () {
    var dataid = $(this).data('id');
    $('#SinhNhatSL .form-group').each(function () {
        $(this).find('.conten-choose').find('input').removeAttr('disabled');
        if (dataid !== $(this).find('.radio-menu').find('input').data('id')) {
            $(this).find('.conten-choose').find('input').attr('disabled', 'disabled');
        }
    });
    if ($(this).data('id') !== 1) {
        vmLoaiBaoHiem.curentpage.typetime = null;
    }
    else {
        vmLoaiBaoHiem.curentpage.typetime = vmLoaiBaoHiem.curentpage.typetimeold;
    }
    vmLoaiBaoHiem.GetForSearchLoaiBaoHiem(true);
})
$('#SelectNgaySinh').on('click', 'ul li', function () {

    if (vmLoaiBaoHiem.curentpage.typetime !== null) {
        vmLoaiBaoHiem.curentpage.textdate = $(this).find('a').text();
        vmLoaiBaoHiem.curentpage.typetime = $(this).val();
        vmLoaiBaoHiem.curentpage.typetimeold = $(this).val();
        vmLoaiBaoHiem.GetForSearchLoaiBaoHiem(true);
    }
})
$('body').on("AddLoaiBaoHiemSucces", function () {
    vmLoaiBaoHiem.GetForSearchLoaiBaoHiem(true);
});