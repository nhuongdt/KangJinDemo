

Vue.component('modal', {
    template: '#modal-template',
    props: ['show'],
    methods: {
        savePost: function () {
            this.$emit('close');
        }, close: function () {
            this.$emit('close');
        },
    }
});


$('#linhvuckd').on('change', function () {
    vmbody.CuahangDangKy.Software = $(this).val();
})
$('#ModalDangKyDungThuSsoft').on('shown.bs.modal', function (e) {
    $('.dk-ho-ten').focus();
})
Array.prototype.insert = function (index, item) {
    this.splice(index, 0, item);
};
$('body').on('hidden.bs.modal', '.modal', function () {
    $('#myVideo').attr("src", '');
});
$(".list-useful a.services-wrap")
    .mouseenter(function () {
        if ($(window).width() > 768) {
            $(this).animate({
                'marginLeft': '-=50px'
            }, 200);
            $(this).find('.icon').find('i').each(function () {
                $(this).toggle();
            });
        }
        
    })
    .mouseleave(function () {
        if ($(window).width() > 768) {
            $(this).animate({
                'marginLeft': '+=50px'
            }, 200);
            $(this).find('.icon').find('i').each(function () {
                $(this).toggle();
            });
        }
        
    });
$('.btn-dang-ky-ct').on('click', function () {
    window.location.href = "/dung-thu-mien-phi";
});
$('.RegisterBtn').on('click', function () {
    window.location.href = "/dung-thu-mien-phi";
});

$('.btn-tham-gia-ngay').on('click', function () {
    $("html, body").animate({ scrollTop: ($(document).height() - 1250) }, 1000);

});
$('.btn-gioi-thieu ').on('click', function () {
    $("html, body").animate({ scrollTop: ($(document).height() - 1250) }, 1000);
});
$('.btn-price-dung-thu').on('click',function () {
    window.location.href = "/dung-thu-mien-phi";
});

$(document).mouseup(function () {
    $('.choose-select').removeClass('choose-open');
});
$('.choose-select').on('click', '.select-text', function () {
    $(".select-text").mouseup(function () {
        return false
    });
    var check = $(this).closest('.choose-select').hasClass('choose-open');
    $('.choose-select').removeClass('choose-open');
    if (!check) {
        $(this).closest('.choose-select').addClass('choose-open');
    }
});

