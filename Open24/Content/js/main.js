$(document).ready(function () {
    $(".click-menu,.bg-fuzzy").click(function () {
        $(".menu-main").toggleClass("visi");
        $(".bg-fuzzy").toggle();

    });
});
$(function () {
    $(window).scroll(function () {
        var e = $(window).scrollTop();
        if (e > 100) {
            $("#toTop").show()
        } else {
            $("#toTop").hide()
        }
    });
    $('#toTop').click(function () {
        $('body,html').animate({
            scrollTop: 0
        })
    });
});
(function ($) {

    'use strict';

    $(document).on('show.bs.tab', '.nav-tabs-responsive [data-toggle="tab"]', function (e) {
        var $target = $(e.target);
        var $tabs = $target.closest('.nav-tabs-responsive');
        var $current = $target.closest('li');
        var $parent = $current.closest('li.dropdown');
        $current = $parent.length > 0 ? $parent : $current;
        var $next = $current.next();
        var $prev = $current.prev();
        var updateDropdownMenu = function ($el, position) {
            $el
                .find('.dropdown-menu')
                .removeClass('pull-xs-left pull-xs-center pull-xs-right')
                .addClass('pull-xs-' + position);
        };

        $tabs.find('>li').removeClass('next prev');
        $prev.addClass('prev');
        $next.addClass('next');

        updateDropdownMenu($prev, 'left');
        updateDropdownMenu($current, 'center');
        updateDropdownMenu($next, 'right');
    });

})(jQuery);
function ToggleNext(ele) {
    $(ele).next().slideToggle()
    $(ele).children("a").toggleClass("arrow-up")
}