//=================================
// Show Progressbar
//=================================
var progressBar = progressBar || (function () {
    var loaddingBar = 0;
    var EventLoadElementBar = false;
    var timerBar;
    var STTprogressBar = 0;
    var start = function (percent, $element) {
        var progressBarWidth = percent * $element.width() / 100;
        $element.find('div').animate({ width: progressBarWidth }, 200).html(percent + "%&nbsp;");
        if (percent === 100) {
            $element.find('div').animate({ width: 0 }, 0).html(percent + "%&nbsp;");
        }
    };
    var loadTimeBar = function () {
        loaddingBar = loaddingBar + 5;
        start(parseInt(loaddingBar), $('#progressBar'));
        if (EventLoadElementBar) {
            clearTimeout(timerBar);
            setTimeout(function () { LoadTimerBar2(90); }, 200);
        }
        else if (loaddingBar < 90) {
            timerBar = setTimeout(function () { loadTimeBar(); }, 300);
        }
        else if (loaddingBar === 90) {
            STTprogressBar++;
            if (STTprogressBar < 100) {
                loaddingBar = 85;
                timerBar = setTimeout(function () { loadTimeBar(); }, 1500);
            }
            else {
                STTprogressBar = 0;
            }
        }

    };
    var LoadTimerBar2 = function (time) {
        time = time + 5;
        start(time, $('#progressBar'));
        if (time < 100) {
            setTimeout(function () { LoadTimerBar2(time); }, 200);
        }
    };
    var SetTimeBar = function () {
        STTprogressBar = 0;
        loaddingBar = 0;
        EventLoadElementBar = false;
        setTimeout(function () { loadTimeBar(); }, 200);
    };
    var stopTimeBar = function () {
        EventLoadElementBar = true;

    };
    return {
        load: SetTimeBar,
        stop: stopTimeBar
    };
})();


//=================================
// LoadJquery
//=================================
(function ($) {
    $.fn.HtmlLoader = function (options) {
        console.clear();
        progressBar.load();
        var defaults = {
            parentContainerId: "",
            params: {},
            pageName: "",
            fragmentId: "",
            title: '',
            emptyMessage: "Preset:",
            onSucess: function () { },
            onError: function () { },
            elementId: "",
            elementCSS: "statistics_graph"
        };

        options = $.extend(defaults, options);

        if (options.elementId.length === 0
            || options.parentContainerId.length === 0
            || options.pageName.length === 0) {
            return;
        }

        function generateHTML() {
            $('#' + options.elementId).remove();
            var html = "<div id='" + options.elementId + "' class='" + options.elementCSS + "'>";
            html = html + "</div>";
            return html;
        }

        return this.each(function () {
            obj = $(this);
            document.title = options.title;
            $.ajax({
                url: options.pageName + "?Version=" + Date.now(), cache: false,
                success: function (result) {
                    obj.html(generateHTML());
                    $("#" + options.elementId, obj).html(result);
                },
                complete: function (jqXHR) {
                    progressBar.stop();
                    if (jqXHR.status === 403) {
                        window.location.href = jqXHR.responseJSON.LogOnUrl;
                    }
                }
            });
        });
    };
    $.fn.gridLoader = function (options) {
        var defaults = {
            show: true,
            style: "",
            morewidthheight: 60,
            iconloading: '',
        };
        var option = $.extend(defaults, options);
        return this.each(function () {
            if (option.show) {
                var left = $(this).width() / 2 - option.morewidthheight;
                var top = $(this).height() / 2 + option.morewidthheight;
                if (option.iconloading !== '') {
                    $(this).append(option.iconloading);
                }
                else {
                    $(this).addClass('modal-opened').append('<div class="loader-padding" style="left:' + left + 'px;top:' + top + 'px;' + option.style + '"></div>');
                }
            }
            else {
                $('.loader-padding').remove();
                $('.loading-box').remove();
                $(this).removeClass('modal-opened');
            }
        });
    };
    $('.modal-dialog').draggable({
        handle: ".modal-header"
    });

})(jQuery);