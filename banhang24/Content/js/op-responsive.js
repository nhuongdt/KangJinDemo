$(document).ready(function () {
   
    var $w = $(document).width();
    var $h = $(document).height();
    $(".chinhanh > li").click(function () {
        if ($(this).children(".main-drop").is(":hidden")) {
            $(this).siblings().find(".main-drop").fadeOut("fast");
            $(this).children('.main-drop').show();
        } 

    });
    $(".message-archor").click(function () {
        $(this).next('.message-popup').toggle();
    });
    $(document).mouseup(function (e) {

        var container = $(".main-drop");

        if (!container.is(e.target) && container.has(e.target).length === 0) {

            container.fadeOut("fast");

        }
    }); 
    $(document).mouseup(function (e) {

        var container = $(".message-popup");

        if (!container.is(e.target) && container.has(e.target).length === 0) {

            container.fadeOut("fast");

        }
    }); 
   
    $(".click-menu").click(function () {
        $(this).find("i").toggle();
        $(".menu-horizontal-inner").toggleClass("left0");
        $(".bg-op").toggle();
    }, function () {
        $(this).find("i").toggle();
        $(".menu-horizontal-inner").toggleClass("left0");
        $(".bg-op").toggle();
    });
    //nền menu
    $(".bg-op").click(function () {
        $(".click-menu i").toggle();
        $(".menu-horizontal-inner").removeClass("left0");
        $(".bg-op").hide();
        $(".col-left").removeClass("showed");
        $("#op-popup-thongbao").removeClass("showed");
        if (window.innerWidth < 768) {
            $('.op-menu').removeClass('expanded')
        }
        setTimeout(function () { Tawk_API.showWidget();  }, 400);
        //Tawk_API.showWidget();
    });

    $(".kv2Btn1").click(function () {
        $(".bg-op").click();
     
    });
    // Jquery draggable
    $('.modal-dialog').draggable({
        handle: ".modal-header"
    });

    var lastestVersion = "1.15";
    var version =  localStorage.getItem("version");
    $("#countNum").html("15 giây");
    var count = 15;
    $("#version").html(version);
    $("#lastestVersion").html(lastestVersion) ;
    if (version !== lastestVersion)
    {
        localStorage.setItem("version", lastestVersion);
        //$("#versionmodal").modal("show");
        //var x = setInterval(function () {
        //    document.getElementById("countNum").innerHTML = count + " giây ";
        //    count = count - 1;
        //    if (count < 0  ) {
        //        clearInterval(x);
        //        location.reload(true);
        //    }
        //}, 1000);
    } else {
        $("#versionupdate").hide();
    };

   


    
});
function collapseSubFilter(ele) {
   
    $(ele).hide();
    $(ele).next().show()
    $('.op-filter-container').slideUp()
}

function expandSubFilter(ele) {
    
    $(ele).hide();
    $(ele).prev().show()
    $('.op-filter-container').slideDown()
}
function toggleSubFilter(ele) {
    $(ele).siblings('.op-filter-container').slideToggle()
}
function closeFilter() {
    $('.bg-op').click()
}