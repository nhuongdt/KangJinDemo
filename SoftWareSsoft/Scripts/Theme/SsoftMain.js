$(document).ready(function () {
    $(".click-menu,.bg-fuzzy").click(function () {
        $(".menu-main").toggleClass("visi");
        $(".bg-fuzzy").toggle();

    });
    $('.dung-thu-pm').on('click', function () {
        vmDangKyCuaHang.refresh($(this).data('id'), 2);
    });
    $('.dat-mua-pm').on('click', function () {
        vmDangKyCuaHang.refresh($(this).data('id'), 1);
    });
    $('.btn-home-tin-tuc').on('click','li', function () {

        vmbody.GetRecruitmentHome($(this).data('id'));
    });
    $('.list-tinh-thanh-sl').on('click', 'li', function () {
        vmbody.SelectedTTKH($(this).data('id'));
        $('.tree-detail').hide();
        $('.span-tt').text($(this).find('a').text());
    });
    $('.list-san-pham-sl').on('click', 'li', function () {

        vmbody.SelectedSPKH($(this).data('id'));
        $('.tree-detail').hide();
        $('.span-sp').text($(this).find('a').text());
    });
});
    var vmDangKyCuaHang = new Vue({
        el: '#ModalDangKyDungThuSsoft',
        data: {
            SoDienThoai: "",
            HoVaTen: "",
            Email: "",
            GhiChu: "",
            error: "",
            DiaChi: "",
            type: "",
            Sofware: "",
            ListTinhThanh:[],
            Type:2,
        },
        methods: {
            messageError: function (input) {
                this.error = input;
            },
            refresh: function (software, type) {
                this.ID = null;
                this.SoDienThoai = "";
                this.HoVaTen = "";
                this.Email = "";
                this.error = '';
                this.GhiChu = "";
                this.DiaChi = "";
                this.Sofware = software;
                this.Type = type;
                $('#phoneNumber').val('');
            },

            GetTinhThanh: function () {
                var self = this;
                $.getJSON("/SsoftApi/News/GetAllTinhThanh", function (data) {
                        self.ListTinhThanh = data;
                        vmbody.ListTinhThanh = data;
                });
            },
            Save: function (event) {
                var self = this;
                self.SoDienThoai = $('#phoneNumber').val().replace(/[()-]/g, '').replace(/\s+/g, '');
                if (localValidate.CheckNull(self.HoVaTen)) {
                    self.messageError("Vui lòng nhập họ tên");
                } 
                else if (localValidate.CheckNull(self.SoDienThoai)) {
                    self.messageError("Vui lòng nhập số điện thoại");
                }
                else if ((self.SoDienThoai.length !== 10 && self.SoDienThoai.length !== 11) || !$.isNumeric(self.SoDienThoai)) {
                    self.messageError("Số điện thoại không đúng định dạng");
                }
                else if ($('#select-tinhthanh').val() === '' || $('#select-tinhthanh').val() ===null) {
                    self.messageError("Vui lòng chọn tỉnh thành");
                }
                else if (!localValidate.CheckNull(self.Email) && !localValidate.CheckEmail(self.Email)) {
                    self.messageError("Địa chỉ Email không hợp lệ");
                }
                else {
                    $.ajax({
                        type: 'GET',
                        url: "https://geoip-db.com/json/",
                        success: function (data) {
                            self.CallInsert(JSON.parse(data));
                        },
                        timeout: 3000,      // 3 seconds
                        error: function (qXHR, textStatus, errorThrown) {
                            self.CallInsert(null);
                            if (textStatus === "timeout") {
                                console.log(qXHR);
                            }
                        }
                    });
                   
                }
            },
            CallInsert: function (ipAdress) {
                var self = this;
                var model = {
                    FullName: localValidate.convertStrFormC(self.HoVaTen),
                    Phone: localValidate.convertStrFormC(self.SoDienThoai),
                    Email: localValidate.convertStrFormC(self.Email),
                    Address: $('#select-tinhthanh').val(),
                    Note: localValidate.convertStrFormC(self.GhiChu),
                    Software: localValidate.convertStrFormC(self.Sofware),
                    Type: self.Type,
                };
                var diachi = "";
                var ip4 = "";
                if (ipAdress !== null && ipAdress !== undefined) {
                    diachi = (ipAdress.city !== '' && ipAdress.city !== null) ? ipAdress.city + "-" + ipAdress.country_name : ipAdress.country_name;
                    ip4 = ipAdress.IPv4;
                }
                $.ajax({
                    data: model,
                    url: '/SsoftApi/ApiProduct/OrderedSsoftSoftWare?ip4=' + ip4 + '&ipAdress=' + diachi.trim(),
                    type: 'POST',
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (item) {
                        if (item.res === true) {
                            $('#ModalDangKyDungThuSsoft').find('.modal-header').find('button').click();
                            if (self.Type === 2) {
                                alert("Đăng ký dùng thử thành công");
                            }
                            else {
                                alert("Đặt mua thành công");
                            }
                        }
                        else {
                            self.messageError(item.mess);
                        }
                    }
                });
            }

        }
})
    var vmbody = new Vue({
        el: '#renderBody',
        data: {
            ListNews: [],
            ListNewsDate: [],
            ListNewsView: [],
            ListCustomer: [],
            ListCustomerDate: [],
            ListProduct: [],
            ListTinhThanh: [],
            listRecruitment: [],
            listRecruitmentHN: [],
            listRecruitmentHCM: [],
            listRecruitmentGroup: [],
            listCustomerProduct: [],
            contact: {
                TenNguoiLienHe: '',
                SoDienThoai:'',
                Email: '',
                DiaChi: '',
                GhiChu: ''
            },
            objseachCustomer: {
                search: '',
                tinhthanhId: '',
                productId:''
            },
            ObjSendReq: {
                HoTen: '',
                ID_TuyenDung:null,
                GioiTinh: false,
                Email: '',
                SoDienThoai: '',
                DiaChi: '',
                TruongTotNghiep: '',
                NgaySinh:null,
                HeDaoTao: '',
                ChuyenNganh: '',
                KyNang:'',
            },
            page: 0,
            visible:true,
            error: "",
            priductId: null,
        },
        methods: {
            messageError: function (input) {
                this.error = input;
            },
            refreshContact: function () {
                this.contact = {
                    TenNguoiLienHe: '',
                    SoDienThoai: '',
                    Email: '',
                    DiaChi: '',
                    GhiChu: ''
                };
            },
            refresh: function () {
                this.ListNews = [];
                this.ListNewsDate = [];
                this.ListCustomer = [];
                this.ListCustomerDate = [];
                this.ListProduct = [];
                this.ListTinhThanh = [];
                this.ListNewsView = [];
                this.listRecruitmentGroup = [];
                this.objseachCustomer = {
                    search: '',
                    tinhthanhId: '',
                    productId: ''
                }
                this.page = 0;
                this.error = '';
            },
            // Bài viết
            GetNewsAll: function (page) {
                var self = this;
                $.ajax({
                    type: 'GET',
                    url: "/SsoftApi/News/GetNewsPage?page=" + page,
                    success: function (data) {
                        if (data.res === true)
                        {
                            if (self.ListNews.length > 0) {
                                for (var i = 0; i < data.dataSoure.length; i++) {
                                    self.ListNews.push(data.dataSoure[i]);
                                }
                            }
                            else {
                                self.ListNews = data.dataSoure;
                            }
                            if (data.dataSoure.length === 0) {
                                self.visible = false;
                            }
                        }
                        else {
                            console.log(data.mess);
                        }
                    }
                });
            },
            GetNewsHome: function () {
                var self = this;
                $.ajax({
                    type: 'GET',
                    url: "/SsoftApi/News/GetNewsHome",
                    success: function (data) {
                        if (data.res === true) {
                                self.ListNews = data.dataSoure;
                           
                        }
                        else {
                            console.log(data.mess);
                        }
                    }
                });
            },
            GetNewsDate: function (event) {
                var self = this;
                $.ajax({
                    type: 'GET',
                    url: "/SsoftApi/News/GetNewsDate" ,
                    success: function (data) {
                        if (data.res === true)
                        { self.ListNewsDate = data.dataSoure; }
                    }
                });
            },
            GetNewsView: function (event) {
                var self = this;
                $.getJSON("/SsoftApi/News/GetNewsOrderbyView", function (data) {
                    if (data.res === true) {
                        self.ListNewsView = data.dataSoure;
                    } else {
                        console.log(data.mess);
                    }
                });
            },
            NextNewsPage: function () {
                this.page = this.page + 1;
                this.GetNewsAll(this.page);
            },
            // Khách hàng
            GetCustomerDate: function (event) {
                var self = this;
                $.getJSON("/SsoftApi/ApiCustomer/GetCustomerOrderbyDate", function (data) {
                    if (data.res === true) {
                        self.ListCustomerDate = data.dataSoure;
                    } else {
                        console.log(data.mess);
                    }
                });
            },
            GetCustomerpage: function () {
                var self = this;
                $.ajax({
                    type: 'GET',
                    url: "/SsoftApi/ApiCustomer/GetCustomerPage?"+
                                        "text=" + self.objseachCustomer.search +
                                        "&adress=" + self.objseachCustomer.tinhthanhId +
                                        "&product=" + self.objseachCustomer.productId +
                                        "&page=" + self.page ,
                    success: function (data) {
                        if (data.res === true) {

                            if (self.ListCustomer.length > 0 && self.page!==0) {
                                for (var i = 0; i < data.dataSoure.length; i++) {
                                    self.ListCustomer.push(data.dataSoure[i]);
                                }
                            }
                            else {
                                self.ListCustomer = data.dataSoure;
                            }
                            if (data.dataSoure.length === 0) {
                                self.visible = false;
                            }
                            self.page += 1;
                            
                        }
                        else {
                            console.log(data.mess);
                        }
                    }
                });
            },
            GetCustomerHome: function () {
                var self = this;
                $.ajax({
                    type: 'GET',
                    url: "/SsoftApi/ApiCustomer/GetCustomerPage?" +
                                            "text=" + self.objseachCustomer.search +
                                            "&adress=" + self.objseachCustomer.tinhthanhId +
                                            "&product=" + self.objseachCustomer.productId +
                                            "&page=0" ,

                    success: function (data) {
                        if (data.res === true) {
                            self.ListCustomer = data.dataSoure.slice(0,4);
                        }
                        else {
                            console.log(data.mess);
                        }
                    }
                });
            },
            keySearchCustomer: function (event) {
                if (event.key === "Enter") {
                    this.page = 0;
                    this.GetCustomerpage();
                    this.visible = true;
                }
            },
            SelectedTTKH: function (id) {
                this.objseachCustomer.tinhthanhId = id;
                this.page = 0;
                this.GetCustomerpage();
                this.visible = true;
            },
            SelectedSPKH: function (id) {
                this.objseachCustomer.productId = id;
                this.page = 0;
                this.GetCustomerpage();
                this.visible = true;
            },
            NextCustomerPage: function () {
                this.GetCustomerpage();
            },
            

            // Tuyển dụng
            GetRecruitmentPage: function (event) {
                var self = this;
                $.getJSON("/SsoftApi/ApiRecruitment/GetRecruitmentPage", function (data) {
                    if (data.res === true) {
                        self.listRecruitment = data.dataSoure;
                    } else {                           
                        console.log(data.mess);
                    }
                });
            },
            GetRecruitmentHome: function () {
                var self = this;
                $.getJSON("/SsoftApi/ApiRecruitment/GetRecruitmentHome" , function (data) {
                    if (data.res === true) {
                        self.listRecruitmentHN = data.dataSoure.dataHN;
                        self.listRecruitmentHCM = data.dataSoure.dataHCM;
                    } else {
                        console.log(data.mess);
                    }
                });
            },
            GetRecruitmentGroup: function () {
                var self = this;
                $.getJSON("/SsoftApi/ApiRecruitment/GetRecruitmentGroup", function (data) {
                    if (data.res === true) {
                        self.listRecruitmentGroup = data.dataSoure;
                        self.listRecruitmentGroup.insert(0, { ID: null, TenNhomBaiViet: 'Tất cả', GhiChu: '' });
                        if (data.dataSoure.length > 0) {
                            self.GetRecruitmentDetailGroup(data.dataSoure[0].ID);
                        }
                       
                    } else {
                        console.log(data.mess);
                    }
                });
            },
            GetRecruitmentDetailGroup: function (groupId, Id = 0) {
                var self = this;
                $.getJSON("/SsoftApi/ApiRecruitment/GetRecruitmentDetailGroup?groupReq=" + groupId, function (data) {
                    if (data.res === true) {
                        self.listRecruitment = data.dataSoure.filter(o=>o.ID!==Id);
                    } else {
                        console.log(data.mess);
                    }
                });
            },
            selectedrecruitment: function (id, e) {
                this.GetRecruitmentDetailGroup(id);
                $('.tree-group-recruitment ul li').each(function () {
                    $(this).removeClass("recruitment-active");
                });
                $(e.currentTarget).addClass("recruitment-active");
            },
            
            SendRecruitment: function () {
                var self = this;
                var fileUpload = $("#imageUploadForm").get(0);
                var files = fileUpload.files;
                var fileData = new FormData();
                if (files.length <= 0 ) {
                    self.messageError("Vui lòng chọn file đính kèm");
                    return;
                }
                for (var i = 0; i < files.length; i++) {
                    fileData.append(files[i].name, files[i]);
                }
                if (localValidate.CheckNull(self.ObjSendReq.HoTen)) {
                    self.messageError("Vui lòng nhập họ tên");
                }
                else if (!localValidate.CheckNull(self.ObjSendReq.Email) && !localValidate.CheckEmail(self.ObjSendReq.Email)) {
                    self.messageError("Địa chỉ Email không hợp lệ");
                }
                else if (localValidate.CheckNull(self.ObjSendReq.SoDienThoai)) {
                    self.messageError("Vui lòng nhập số điện thoại");
                }
               
                else {
                    self.ObjSendReq.GioiTinh = $('#gioitinhsl').val();
                    var model= self.ObjSendReq;
                    $.ajax({
                        data: model,
                        url: '/SsoftApi/ApiRecruitment/InsertHoSo',
                        type: 'POST',
                        dataType: 'json',
                        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                        success: function (result) {
                            if (result.res === true) {

                                $.ajax({
                                    data: fileData,
                                    url: '/SsoftApi/ApiRecruitment/UploadFile?KeyId=' + result.dataSoure +"&nameFile='5534'",
                                    type: 'POST',
                                    processData: false,
                                    contentType: false,
                                    success: function (result) {
                                        if (result.res === true) {
                                            $('#blah').text('');
                                            $('#gioitinhsl').val('')
                                            fileData = new FormData();
                                            self.ObjSendReq = {
                                                HoTen: '',
                                                GioiTinh: false,
                                                ID_TuyenDung: null,
                                                Email: '',
                                                SoDienThoai: '',
                                                DiaChi: '',
                                                TruongTotNghiep: '',
                                                NgaySinh: null,
                                                HeDaoTao: '',
                                                ChuyenNganh: '',
                                                KyNang: '',
                                            };
                                            alert("Nộp hồ sơ ứng tuyển thành công!");
                                       
                                        }
                                        else {
                                            self.messageError("Đã xảy ra lỗi vui lòng thử lại sau")
                                        }
                                    }
                                });
                            }
                            else {
                                self.messageError(result.mess);
                            }
                        }
                    });
                }
            },

            GetProductTinhThanh: function () {
                var self = this;
                $.getJSON("/SsoftApi/ApiProduct/GetCombobox", function (data) {
                    if (data.res === true) {
                        self.ListProduct = data.dataSoure;
                    } else {
                        console.log(data.mess);
                    }
                });
                //$.getJSON("/SsoftApi/ApiProduct/GetAllTinhThanh", function (data) {
                //        self.ListTinhThanh = data
                //});
            },
            SendContact: function () {

                var self = this;
                var model = self.contact;
                if (localValidate.CheckNull(self.contact.TenNguoiLienHe)) {
                    alert("Vui lòng nhập họ tên liên hệ");
                }
                else if (localValidate.CheckNull(self.contact.SoDienThoai)) {
                    alert("Vui lòng nhập số điện thoại");
                }
                else if (localValidate.CheckNull(self.contact.DiaChi)) {
                    alert("Vui lòng nhập địa chỉ");
                }
                else if (localValidate.CheckNull(self.contact.Email) || !localValidate.CheckEmail(self.contact.Email)) {
                    alert("Vui lòng kiểm tra lại thông tin Email đã đầy đủ chưa");
                }
                else {
                    $.ajax({
                        data: model,
                        url: '/SsoftApi/ApiHome/SendContact',
                        type: 'POST',
                        dataType: 'json',
                        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                        success: function (item) {
                            if (item.res === true) {
                                alert(item.mess);
                                self.refreshContact();
                            }
                            else {
                                alert(item.mess);
                            }
                        }
                    });
                }
            },
            GetCustomerForProduct: function (keyId) {
                var self = this;
                self.priductId = keyId;
                $.getJSON("/SsoftApi/ApiCustomer/GetCustomerForproduct?keyId=" + keyId + '&page=' + self.page, function (data) {
                    if (data.res === true) {
                        self.page = self.page + 1;
                        for (var i = 0; i < data.dataSoure.length; i++) {
                            self.listCustomerProduct.push(data.dataSoure[i]);
                        }
                        if (data.dataSoure.length === 0) {
                            self.visible = false;
                        }

                    } else {
                        console.log(data.mess);
                    }
                });
            },
            NextCustomerProductPage: function () {
                this.GetCustomerForProduct(this.priductId );
            }
            


        }
    })
    vmDangKyCuaHang.GetTinhThanh();
$('#ModalDangKyDungThuSsoft').on('shown.bs.modal', function (e) {
    $('.dk-ho-ten').focus();
    })
Array.prototype.insert = function (index, item) {
    this.splice(index, 0, item);
};
