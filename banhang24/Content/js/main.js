$('.op-filter-title ').on('click', function () {
    $(this).next(".op-filter-container").slideToggle();
    $(this).find("a i").not(".plus_not").toggle();
    
    $(".op-filter-title span").css("display", "block");
});
$('.plus_not ').on('click', function () {
    $(this).next(".op-filter-container").slideToggle();
    $(this).find("a i").not(".plus_not").toggle();
    $(".op-filter-title span").css("display", "block");
});
$('.information ul li').eq(0).css("display", "block");
$('.newprice').click(function () {
    console.log($(this).next(".callprice"));
    $(".callprice").css("display", "none");
    $(this).next(".callprice").css("display", "block");
});
$(".btn-dropdown button").click(function () {
    $(".dropdown-list").toggle();
});

function del() {
    console.log("run");
    $(".col-md-1").click(function () {
        $(this).parent(".inner-setup").css("display", "none");
    });
    
}
$('.newprice').click(function () {
    $(".callprice").css("display", "none");
    $(this).next(".callprice").css("display", "block");
});

$(".dropdown-list,.month-oll,.dropdown-list input").mouseup(function () {
    return false;
});
$(".dropdown-toggle,.btn-dropdown,.month").mouseup(function () {
    return false;
});

$(document).mouseup(function () {
    $(".btn-dropdown button").mouseup(function () {
        return false;
    });
    $(".dropdown-list").mouseup(function () {
        return false;
    });
    $(".dropdown-list,.month-oll ").hide();
}); 

$(function () {
    $(".draggable").draggable({ handle: ".modal-header" });
   
});

$(document).on('click', '.bnt-print1', function () {
    $(".modal-ontop1").show();
});
$(document).on('click', '.close-print1', function () {  
    $(".modal-ontop1").hide();
});
$(document).on('click', '.close-print', function () {
    $(".printpage").hide();
    $(".printpageThaoTac").hide();
    $(".modal-ontop").hide();
    $(".modal-ontop1").hide();
});
$(document).on('click', '.click-price', function () {
    $(".price-public").show();
});
$(".op-filter-title").click(function () {
    $(this).next().toggle()
})