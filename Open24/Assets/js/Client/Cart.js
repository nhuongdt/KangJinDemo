
var Cart = function () {
    var self = this;
    self.listMenu = ko.observableArray();
    self.listDataRelated = ko.observableArray();
    self.DevicesDetail = ko.observableArray();
    self.ViewImg = ko.observable();
    function Loadform() {
        if (location.pathname.split('/').length > 3) {
            $.ajax({
                url: "/Open24Api/ApiSalesDevice/GetDetailDevice/" + location.pathname.split('/')[2],
                type: 'GET',
                contentType: 'application/json',
                success: function (result) {
                    if (result.res === true) {
                        if (result.DataSoure.detail.SalesImgDevices.length > 0) {
                            self.ViewImg(result.DataSoure.detail.SalesImgDevices[0].SrcImage);
                        }
                        self.DevicesDetail(result.DataSoure.detail);
                        self.listMenu(result.DataSoure.listMenu);
                        self.listDataRelated(result.DataSoure.dataRelated);
                    } else {
                        console.log(result.mess);
                    }
                },
                error: function (result) {
                    console.log(result);
                }
            });
        }
    };

    Loadform();
    self.ChangeImg = function (item) {
        self.ViewImg(item.SrcImage);
    }
    const CartCookieName = "cartId";
    self.addcart = function ()
    {
        var objectStore = LocalIndexDB.Connect();
        var request = objectStore.getAll();
        request.onsuccess = function (evt) {
            var data = request.result;
            var CartId = LocalCookies.Get(CartCookieName);
            if (CartId === null || CartId === undefined) {
                Cart = Guid.newGuid();
                LocalCookies.Set(CartCookieName, Cart, 10);
            }

            if (data.some(o => o.salesDevice_Id === self.DevicesDetail().ID && o.Client_Id === CartId)) {

                var objectStoreRequest = objectStore.openCursor(data.filter(o => o.salesDevice_Id === self.DevicesDetail().ID && o.Client_Id === CartId)[0].ID);

                objectStoreRequest.onsuccess = function (event) {
                    var cursor = event.target.result;
                    if (cursor) {
                        var myRecord = cursor.value;
                        if (myRecord !== null && myRecord !== undefined) {
                            myRecord.salesDevice_Quantity += 1;
                            myRecord.salesDevice_Money = myRecord.salesDevice_Quantity * myRecord.salesDevice_Price;
                            cursor.update(myRecord);
                        }
                    }
                    window.location.href = "/Gio-hang";
                };

            }
            else {
                var price = self.DevicesDetail().IsSalePrice == true ? self.DevicesDetail().PriceSale : self.DevicesDetail().Price;
                var model = {
                    salesDevice_Id: self.DevicesDetail().ID,
                    salesDevice_Name: self.DevicesDetail().Name,
                    salesDevice_Price: price,
                    salesDevice_Quantity: 1,
                    salesDevice_Img: self.ViewImg(),
                    salesDevice_Money: (1 * price),
                    Client_Id: CartId,
                };
                objectStore.add(model);
                window.location.href = "/Gio-hang";
            }
        };
    }
}
ko.applyBindings(new Cart());
