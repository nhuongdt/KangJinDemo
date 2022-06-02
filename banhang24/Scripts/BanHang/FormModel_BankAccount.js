var FormModel_BankAccount = function () {
    var self = this;
    self.ID = ko.observable();
    self.ID_NganHang = ko.observable();
    self.ID_DonVi = ko.observable();
    self.TenChuThe = ko.observable();
    self.SoTaiKhoan = ko.observable();
    self.GhiChu = ko.observable();
    self.TaiKhoanPOS = ko.observable(true);
    self.TenNganHang = ko.observable(true);

    self.Setdata = function (item) {
        self.ID(item.ID);
        self.ID_NganHang(item.ID_NganHang);
        self.ID_DonVi(item.ID_DonVi);
        self.TenChuThe(item.TenChuThe);
        self.SoTaiKhoan(item.SoTaiKhoan);
        self.GhiChu(item.GhiChu);
        self.TaiKhoanPOS(item.TaiKhoanPOS);
        self.TenNganHang(item.TenNganHang);
    }
}
