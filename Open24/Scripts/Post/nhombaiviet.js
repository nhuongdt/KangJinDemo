var View = function () {
    var self = this;

    self.CateGroup = ko.observableArray();

    function getListCateGroup() {
        ajaxHelper('/Open24Api/PostAPI/' + "GetListCateGroup", 'GET').done(function (data) {
            var a = JSON.stringify(data);
            self.CateGroup(JSON.parse(a));
        });
    }
    getListCateGroup();
};
ko.applyBindings(new View());


function ajaxHelper(uri, method, data) {

    return $.ajax({
        type: method,
        url: uri,
        dataType: 'json',
        contentType: 'application/json',
        data: data ? JSON.stringify(data) : null,
        async: false,
        statusCode: {
            404: function () {
                console.log("Page not found");
            },
        }
    })
        .fail(function (jqXHR, textStatus, errorThrown) {
            console.log(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
        }); 
}