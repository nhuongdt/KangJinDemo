$(document).ready(function () {

var hotrophanmem = function (Isform, id, nhomId) {
    var self = this;
    //===============================
    // Khai báo chung
    //===============================
    self.listnhomnganh = ko.observableArray();
    self.listResult = ko.observableArray();
    self.listCauHoileft = ko.observableArray();
    self.listCauHoiright = ko.observableArray();
    self.page = ko.observable(1);
    self.groupNhom = ko.observable(id);
    self.groupNhomtinhnang = ko.observable(nhomId);
    self.tentitle = ko.observable();
    self.video = ko.observable();
    self.noidung = ko.observable();
    self.ListTinhNang = ko.observableArray();
    self.url = ko.observable(window.location.href);
    function loadHoTro() {
        $.ajax({
            url: '/Open24Api/ApiHoTro/GetViewHoTro',
            type: 'GET',
            async: true,
            dataType: 'json',
            contentType: 'application/json',
            success: function (result) {
                if (result.res === true) {

                    self.listnhomnganh(result.DataSoure.NhomNganh);
                    self.listResult(result.DataSoure.TinhNang);
                }
                else {
                    alert(result.mess);
                }

            },
            error: function (result) {
                alert(result);
            }
        });

    }

    function loadCauHoi() {
        $.ajax({
            url: '/Open24Api/ApiHoTro/GetViewCauHoi?page='+self.page(),
            type: 'GET',
            async: true,
            dataType: 'json',
            contentType: 'application/json',
            success: function (result) {
                if (result.res === true) {
                    var length = result.DataSoure.length;
                    if (length <= 0) {
                        $('#Addnew').hide();
                    }
                    var max = length / 2;
                    var itemleft = self.listCauHoileft();
                    itemleft.push.apply(itemleft, result.DataSoure.slice(max, length) );
                    var itemright = self.listCauHoiright();
                    itemright.push.apply(itemright, result.DataSoure.slice(0, max))
                    self.listCauHoileft(itemright);
                    self.listCauHoiright(itemleft);
                }
                else {
                    alert(result.mess);
                }

            },
            error: function (result) {
                alert(result);
            }
        });

    }

    function loadHoTroChiTiet() {
        $.ajax({
            url: '/Open24Api/ApiHoTro/GetViewTinhNang?group=' + self.groupNhomtinhnang() + '&tinhnang=' + self.groupNhom(),
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            success: function (result) {
                if (result.res === true) {
                    self.listResult(result.DataSoure.json);
                    self.ListTinhNang(result.DataSoure.list);
                    if (self.groupNhomtinhnang() !== 0 && self.groupNhom() === 0) {
                        $('.ht-tree-left ul li').each(function (i) {
                            if (i === 0) {
                                $(this).find('a').find('i').each(function () {
                                    $(this).toggle();
                                });
                                if ($(this).find('ul').find('li').length > 0)
                                {
                                    $($(this).find('ul')[0]).show();
                                    $(this).find('ul').find('li').each(function (i) {
                                        if (i === 0) {
                                            $(this).find('a').find('i').each(function () {
                                                $(this).toggle();
                                            });
                                            if ($(this).find('ul').find('li').length > 0) {
                                                $($(this).find('ul')[0]).show();
                                                $(this).find('ul').find('li').each(function (i) {
                                                    if (i === 0) {
                                                        $(this).find('a').addClass('hotro-ac');
                                                        loadfirst($(this).find('a').data('id')); return

                                                    }
                                                });
                                            }
                                            else {
                                                $(this).find('a').addClass('hotro-ac');
                                                loadfirst($(this).find('a').data('id')); return
                                            }

                                        }
                                    });
                                }
                                else {
                                    $(this).find('a').addClass('hotro-ac');
                                    loadfirst($(this).find('a').data('id')); return
                                }
                                    
                            }
                        });
                    }
                    else {
                        $('.ht-tree-left ul li').each(function (i) {
                            if ($(this).find('a').data('id') === result.DataSoure.ong) {
                                $(this).find('a').find('i').each(function () {
                                    $(this).toggle();
                                });
                                if ($(this).find('ul').find('li').length > 0) {
                                    $($(this).find('ul')[0]).show();
                                    $(this).find('ul').find('li').each(function (i) {
                                        if ($(this).find('a').data('id') === result.DataSoure.cha) {
                                            $(this).find('a').find('i').each(function () {
                                                $(this).toggle();
                                            });
                                            if ($(this).find('ul').find('li').length > 0) {
                                                $($(this).find('ul')[0]).show();
                                                $(this).find('ul').find('li').each(function (i) {
                                                    if ($(this).find('a').data('id') === result.DataSoure.con || (i === 0 && result.DataSoure.con === 0)) {
                                                        $(this).find('a').addClass('hotro-ac');
                                                        loadfirst($(this).find('a').data('id'));
                                                        return
                                                    }

                                                });
                                            }
                                            else {
                                                $(this).find('a').addClass('hotro-ac');
                                                loadfirst($(this).find('a').data('id'));
                                                return
                                            }

                                        }
                                        else {
                                            if (i === 0 && result.DataSoure.cha === 0) {
                                                $(this).find('a').find('i').each(function () {
                                                    $(this).toggle();
                                                });
                                                if ($(this).find('ul').find('li').length > 0) {
                                                    $($(this).find('ul')[0]).show();
                                                    $(this).find('ul').find('li').each(function (i) {
                                                        if (i === 0) {
                                                            $(this).find('a').addClass('hotro-ac');
                                                            loadfirst($(this).find('a').data('id'));
                                                            return
                                                        }
                                                    });
                                                }
                                                else {
                                                    $(this).find('a').addClass('hotro-ac');
                                                    loadfirst($(this).find('a').data('id'));
                                                    return
                                                }
                                            }
                                        }
                                    });
                                }
                                else {
                                    $(this).find('a').addClass('hotro-ac');
                                    loadfirst($(this).find('a').data('id'));
                                    return
                                }

                            }
                        });
                    }
                }
                else {
                    alert(result.mess);
                }

            },
            error: function (result) {
                alert(result);
            }
        });

    }
    function loadfirst(id) {
        var data = self.ListTinhNang().filter(o => o.ID == id);
        if (data.length > 0) {
            self.tentitle(data[0].Ten);
            self.video(data[0].Video);
            self.noidung(data[0].NoiDung);
            $('.video').show()
        }
    }
    self.SelectTinhNang = function (model) {
        if (model.IsCha === true) {
            window.location.href = '/huong-dan-su-dung/' + self.groupNhomtinhnang() + '/' + model.id + '/danh-muc-tinh-nang-' + model.Title;
        }
        if (model.children.length <= 0) {
            self.tentitle(model.text);
            self.video(model.video);
            self.noidung(model.noidung);
            $('.video').show()
        }
    }
    self.nextpage = function () {
        self.page(self.page() + 1);
        loadCauHoi();
    }
    if (Isform === 1) {
        loadCauHoi();
        loadHoTro();
    }
    else {
        loadHoTroChiTiet();
    }
    console.log(1)
    return self;
};

})
