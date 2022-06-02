
$(document).ready(function () {
    $('.slidetoggle').each(function () {
        $(this).prepend('<div class="slidetoggle-indicator"></div>');
        let optg = 0;
        $(this).children(' a').each(function () {
            optg = $(this).innerWidth() + optg;
        })
        $(this).width(optg);
        ToggleTo($(this).children('a.selected'))
    });
    function ToggleTo(item) {
        $(item).siblings('.slidetoggle-indicator').css('left', $(item).position().left);
        $(item).siblings('.slidetoggle-indicator').width($(item).innerWidth());
        $(item).siblings('.slidetoggle-indicator').height($(item).innerHeight());
        $(item).css('color', 'white');
        $(item).siblings().css('color', 'black');
    }
    $('.slidetoggle a').click(function () {
        ToggleTo(this)
    })

})