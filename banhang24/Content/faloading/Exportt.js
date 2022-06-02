
(function ($) {
    $.fn.wordExport = function (input) {
        var defaults = {
            fileName: "Export",
            style: "",
            img: {
                w: 140,
                h: 140,
                l: 120,
                t:50
            }
        };
        var option = $.extend(defaults, input);
        var markup = $(this).clone();
        markup.each(function () {
            var self = $(this);
            if (self.is(':hidden'))
                self.remove();
        });
        var images = Array();
        var img = markup.find('img');
        for (var i = 0; i < img.length; i++) {
            var canvas = document.createElement("CANVAS");
            var context = canvas.getContext('2d');
            var uri = canvas.toDataURL("image/png");
            $(img[i]).attr("src", img[i].src);
            img[i].width = option.img.w;
            img[i].height = option.img.h;
            images[i] = {
                type: uri.substring(uri.indexOf(":") + 1, uri.indexOf(";")),
                encoding: uri.substring(uri.indexOf(";") + 1, uri.indexOf(",")),
                location: $(img[i]).attr("src"),
                data: uri.substring(uri.indexOf(",") + 1)
            };
           
        }
        var preHtml = "<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:w='urn:schemas-microsoft-com:office:word' xmlns='http://www.w3.org/TR/REC-html40'>"+
            "<head><meta charset='utf-8'><title>Export HTML To Doc</title></head><body>";
        preHtml += option.style;
        var postHtml = "</body></html>";
        var html = preHtml + markup.html() + postHtml;
        var blob = new Blob(['\ufeff', html], {
            type: 'application/msword'
        });
        var url = 'data:application/vnd.ms-word;charset=utf-8,' + encodeURIComponent(html);
        var downloadLink = document.createElement("a");
        document.body.appendChild(downloadLink);
        if (navigator.msSaveOrOpenBlob) {
            navigator.msSaveOrOpenBlob(blob, option.filename+".doc");
        } else {
            // Create a link to the file
            downloadLink.href = url;
            // Setting the file name
            downloadLink.download = option.fileName+".doc";
            //triggering the function
            downloadLink.click();
        }

        document.body.removeChild(downloadLink);
    };
})(jQuery);

