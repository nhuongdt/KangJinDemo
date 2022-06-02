
function bottomrightnotify(messages, btstyle) {
    $.notify({
        message: messages
    }, {
            type: btstyle,
            delay:100
        });
}

function ShowMessage_Danger(msg) {
    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + msg, "danger");
}

function ShowMessage_Success(msg) {
    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + msg, "success");
}

$.notifyDefaults({
    allow_dismiss: false,
    placement: {
        from: 'bottom',
        align: 'right'
    },
    animate: {
        enter: 'animated fadeInRight margin-bttom-70',
        exit: 'animated fadeOutRight'
    }
});

var removeByAttr = function (arr, attr, value) {
    var i = arr.length;
    while (i--) {
        if (arr[i]
            && arr[i].hasOwnProperty(attr)
            && (arguments.length > 2 && arr[i][attr] === value)) {

            arr.splice(i, 1);

        }
    }
    return arr;
};