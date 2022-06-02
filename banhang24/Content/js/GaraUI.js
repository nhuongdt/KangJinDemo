//$(window).on("load", function () {
$(document).ready(function (){
    
    $(".tr-prev-hide td:not(.td-func)").on("click", function () {
        $(this).parent().toggleClass("active");
        $(this).parent().next().toggleClass("active");
        console.log("hit");
    });
    $(".enable-detail").click(function () {
        $(this).siblings().removeClass("active");
        $(this).addClass("active");
        $(this).parent().siblings().removeClass("active");
        $(this).parent().siblings(".gara-tab-detail").addClass("active");

    });
    $(".enable-bill").click(function () {
        $(this).siblings().removeClass("active");
        $(this).addClass("active");
        $(this).parent().siblings().removeClass("active");
        $(this).parent().siblings(".gara-tab-bill").addClass("active");

    });
    $(".enable-warehouse").click(function () {
        $(this).siblings().removeClass("active");
        $(this).addClass("active");
        $(this).parent().siblings().removeClass("active");
        $(this).parent().siblings(".gara-tab-warehouse").addClass("active");

    });
    $(".enable-content").click(function () {
        $(this).siblings().removeClass("active");
        $(this).addClass("active");
        $(this).parent().siblings().removeClass("active");
        $(this).parent().siblings(".gara-tab-content").addClass("active");

    });

    $("#button-filter > button").click(function () {
        $("#button-filter > div").toggle();
    });

    $(document).mouseup(function (e) {
        var container = $("#button-filter > div");

        if (!container.is(e.target) && container.has(e.target).length === 0) {
            container.hide();
        }

    });
    $(".gara-search-HH").click(
        function () {
            $(this).siblings(".gara-search-dropbox").show();
            $(this).select().focus();
        });

    $(document).mouseup(function (e) {
        var container = $(".gara-search-dropbox");
        if (!container.is(e.target) && container.has(e.target).length === 0) {
            container.hide();
        }
    });

  
});