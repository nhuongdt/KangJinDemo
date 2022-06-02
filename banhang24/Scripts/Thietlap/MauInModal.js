var dataMauIn = function() {

    var self = this;
    self.CTHoaDonPrint = ko.observableArray(item1);
    self.CTHoaDonPrintMH = ko.observableArray(item2);
    self.InforHDprintf = ko.observable(item3);

    self.DM_MauIn = ko.observableArray();
};
var container2VM = dataMauIn();
ko.applyBindings(container2VM, document.getElementById('print_HoaDonDraft'));

