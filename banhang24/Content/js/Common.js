function PrintExtraReport(content, content1, number, close = 0) {
    content1 = content1 || '';
    number = number || 1;
    var xContent = '';
    for (let i = 0; i < number; i++) {
        let onepage = content;
        if (i < number - 1) {
            onepage = content.concat('<p style="page-break-before: always">');
        }
        xContent = xContent + onepage;
    }
    var allContent = '<html>' +
        '<head>'.concat(`<style> tr.mauin-tr-netlien td { border-bottom: 1px solid #ccc; } 
                                tr.mauin-tr-netdut td { border-bottom: 1px dashed #ccc; }
                                table.mauin-table-baoquanh{
                                    border: 1px solid black;
                                }
                                table.mauin-table-baoquanh td,table.mauin-table-baoquanh th{
                                    border: none;
                                }
            </style>
`,
            '</head > ',
            '<body> ', xContent,
            content1,
            '</body> </html>');
    var newIframe = document.createElement('iframe');
    newIframe.width = '0';
    newIframe.height = '0';
    newIframe.src = 'about:blank';
    document.body.appendChild(newIframe);
    newIframe.contentWindow.contents = allContent;
    newIframe.src = 'javascript:window["contents"]';
    newIframe.focus();
    newIframe.onload = function () {
        newIframe.contentWindow.addEventListener('afterprint', (evt) => {
            if (close === 1) {
                window.close();
            }
        });
        setTimeout(function () {
            newIframe.contentWindow.print();
        }, 1000);
    }
    return;
}
function PrintExtraReportTr(content) {
    var frame1 = $('<iframe />');
    frame1[0].name = "frame1";
    frame1.css({ "position": "absolute", "top": "-100000px" });
    $("body").append(frame1);
    var frameDoc = frame1[0].contentWindow ? frame1[0].contentWindow : frame1[0].contentDocument.document ? frame1[0].contentDocument.document : frame1[0].contentDocument;
    frameDoc.document.open();
    //Create a new HTML document.
    frameDoc.document.write('<html><head>');
    //Append the external CSS file.   
    frameDoc.document.write('<link href="/Content/VariablesStyle.css" rel="stylesheet" type="text/css" />');
    frameDoc.document.write('<link href="/Content/style.css" rel="stylesheet" type="text/css" />');
    frameDoc.document.write('</head><body>');
    frameDoc.document.write(content);
    frameDoc.document.write('</body></html>');
    frameDoc.document.close();
    setTimeout(function () {
        window.frames["frame1"].focus();
        window.frames["frame1"].print();
        $('body').trigger('successPrint');
        frame1.remove();
    }, 1500);
}
function InMaVach(mauin, ten, ma, gia, item, sobanghi, banggiaid, hanghoaid) {
    var style = '<link href="/Content/StylePrint.css" rel="stylesheet" type="text/css" />';
    if (item === 2 || item === '2') {
        style = '<link href="/Content/StylePrint2.css" rel="stylesheet" type="text/css" />';
    }
    else if (item === 65 || item === '65') {
        style = '<link href="/Content/StylePrint65.css" rel="stylesheet" type="text/css" />';
    }
    $.ajax({
        url: "/t/InMaVach" + "?mauId=" + mauin + "&masp=" + ma
            + "&tensp=" + ten + "&gia=" + gia + "&item=" + item + "&hanghoaId=" + hanghoaid + "&banggiaId=" + banggiaid + "&sobanghi=" + sobanghi
        , cache: false,
        success: function (result) {
            console.log(1);
            var frame1 = $('<iframe  />');
            frame1[0].name = "framePrint";
            frame1.css({ "position": "absolute", "top": "-100000px" });
            $("body").append(frame1);
            var frameDoc = frame1[0].contentWindow ? frame1[0].contentWindow : frame1[0].contentDocument.document ? frame1[0].contentDocument.document : frame1[0].contentDocument;
            frameDoc.document.open();
            frameDoc.document.write('<html><head>');
            frameDoc.document.write(style);
            frameDoc.document.write('</head><body>');
            frameDoc.document.write(result);
            frameDoc.document.write('</body></html>');
            frameDoc.document.close();
            setTimeout(function () {
                window.frames["framePrint"].focus();
                window.frames["framePrint"].print();
                frame1.remove();
            }, 500);
        }
    });
}
function InListMaVach(objectmauin, banggiaId, idmauin, item) {
    var style = '<link href="/Content/StylePrint.css" rel="stylesheet" type="text/css" />';
    switch (parseInt(item)) {
        case 2:
            style = '<link href="/Content/StylePrint2.css" rel="stylesheet" type="text/css" />';
            break;
        case 65:
            style = '<link href="/Content/StylePrint65.css" rel="stylesheet" type="text/css" />';
            break;
    }
    var list = [];
    for (let i = 0; i < objectmauin.length; i++) {
        list.push({
            MauInId: idmauin,
            TenSP: objectmauin[i].TenHangHoa,
            MaSP: objectmauin[i].MaHangHoa,
            GiaSP: objectmauin[i].GiaBan,
            SoBanGhi: objectmauin[i].TonKho,
            Item: item,
            ID_HangHoa: objectmauin[i].ID
        });
    }
    var model = {
        Item: item,
        data: list,
        BangGiaId: banggiaId
    };
    $.ajax({
        data: ko.toJSON({ model: model }),
        traditional: true,
        url: "/t/PostInMaVach",
        type: 'POST',
        contentType: 'application/json',
        success: function (result) {
            var frame1 = $('<iframe  />');
            frame1[0].name = "framePrint";
            frame1.css({ "position": "absolute", "top": "-100000px" });
            $("body").append(frame1);
            var frameDoc = frame1[0].contentWindow ? frame1[0].contentWindow : frame1[0].contentDocument.document ? frame1[0].contentDocument.document : frame1[0].contentDocument;
            frameDoc.document.open();
            frameDoc.document.write('<html><head>');
            frameDoc.document.write(style);
            frameDoc.document.write('</head><body>');
            frameDoc.document.write(result);
            frameDoc.document.write('</body></html>');
            frameDoc.document.close();
            setTimeout(function () {
                window.frames["framePrint"].focus();
                window.frames["framePrint"].print();
                frame1.remove();
            }, 500);
        },
        error: function (e) {
            alert("Đã xảy ra lỗi.");
        }
    });
}
var LocalCaches = LocalCaches || (function () {
    var NameFilterCache = "FIlterAdvanced";
    var NameScreenHangHoa = "TB_HangHoa";
    var KeyQuanLyLo = "Key_QuanLyLoHang";
    var loadFIlterAdvanced = function (name) {
        var result = JSON.parse(localStorage.getItem(NameFilterCache));
        if (result === null || result === [] || result === undefined) {
            ResestFIlterAdvanced();
        }
        else if (result.some(c => c.Key === name)) {
            return result.filter(o => o.Key === name)[0].Value;
        }
        return false;
    };
    var SetitemFilterAdvanced = function (name) {
        var result = JSON.parse(localStorage.getItem(NameFilterCache));
        if (result === null || result === [] || result === undefined) {
            ResestFIlterAdvanced(name);
        }
        else {
            var person = result.find(function (p) {
                return p.Key === name;
            });
            if (person) {
                person.Value = !person.Value;
            }
            localStorage.setItem(NameFilterCache, JSON.stringify(result));
        }
    };
    var ResestFIlterAdvanced = function () {
        var model = [{
            Key: NameScreenHangHoa,
            Value: false
        }];
        localStorage.setItem(NameFilterCache, JSON.stringify(model));
    };
    // -- Cache ẩn hiện column grid
    // get list chache width key
    var LoadShowHideColumnGrid = function (name) {
        var current = localStorage.getItem(name);
        if (!current) {
            current = [];
            localStorage.setItem(name, JSON.stringify(current));
        } else {
            current = JSON.parse(current);
        }
        return current;
    };
    var CheckColumnGridWithObj = function (name, objects) {
        var current = localStorage.getItem(name);
        if (!current || current === '[]') {
            localStorage.setItem(name, JSON.stringify(objects));
        }
    };
    // add column for cache
    var AddClassIdForColumnGrid = function (name, clas, value) {
        var current = localStorage.getItem(name);
        if (!current) {
            current = [];
        } else {
            current = JSON.parse(current);
        }
        if (current.length > 0) {
            for (let i = 0; i < current.length; i++) {
                if (current[i].NameClass === clas) {
                    current.splice(i, 1);
                    break;
                }
                if (i === current.length - 1) {
                    current.push({
                        NameClass: clas,
                        Value: value
                    });
                    break;
                }
            }
        }
        else {
            current.push({
                NameClass: clas,
                Value: value
            });
        }
        localStorage.setItem(name, JSON.stringify(current));
    };
    // load first load form
    var LoadFirstColumnGrid = function (key, $this, array) {
        var result = LoadShowHideColumnGrid(key);
        for (let i = 0; i < result.length; i++) {
            $('.' + result[i].NameClass).each(function () {
                if ($(this).css('display') !== 'none') {
                    $(this).toggle();
                }
            });
        }
        for (let i = 0; i < array.length; i++) {
            $('.' + array[i].Key).each(function () {
                if ($(this).css('display') === 'none' && !result.some(x => x.NameClass === array[i].Key)) {
                    $(this).toggle();
                }
            });
        }
        $this.each(function () {
            if (result.some(o => o.NameClass === $(this).val())) {
                this.checked = false;
            }
        });
    };
    var RemoveLoHang = function (Key_Form) {
        // 40 kiểu bảng dữ liệu báo cáo enum TypeReport
        for (let i = 1; i < 41; i++) {
            localStorage.removeItem(Key_Form + i + "_LOHANG");
        }
    };
    var LoadFormLoHang = function (Key_Form, $thiseach, item, $thisinput, IsLoadFirst, listcheck) {
        var KeyLo = Key_Form + item + "_LOHANG";
        if (IsLoadFirst) {
            $.getJSON("api/DanhMuc/ThietLapApi/CheckQuanLyLo", function (data) {
                var current = localStorage.getItem(KeyLo);
                if (data.toString() !== current) {
                    localStorage.removeItem(Key_Form + item);
                    if (data.toString() === "false") {
                        $thiseach.each(function (i) {
                            var valueCheck = $(this).find('input[type = checkbox]').val();
                            if (valueCheck.toLowerCase().indexOf("_lohang") >= 0) {
                                AddClassIdForColumnGrid(Key_Form + item, valueCheck, i);
                            }
                        });
                    }
                    RemoveLoHang(Key_Form);
                    localStorage.setItem(KeyQuanLyLo, data);
                    localStorage.setItem(KeyLo, data);
                }
                LoadFirstColumnGrid(Key_Form + item, $thisinput, listcheck);
                $('.table-reponsive').css('display', 'block');
                IsLoadFirst = false;
                return IsLoadFirst;
            });
        }
        else {
            var current = localStorage.getItem(KeyQuanLyLo);
            var page = localStorage.getItem(KeyLo);
            if (!current) {
                IsLoadFirst = true;
            }
            else {
                if (!page || page.toString() !== current) {
                    localStorage.removeItem(Key_Form + item);
                    if (current === "false") {
                        $thiseach.each(function (i) {
                            var valueCheck = $(this).find('input[type = checkbox]').val();
                            if (valueCheck.toLowerCase().indexOf("_lohang") >= 0) {
                                AddClassIdForColumnGrid(Key_Form + item, valueCheck, i);
                            }
                        });
                    }
                    localStorage.setItem(KeyLo, current);
                }
            }
            LoadFirstColumnGrid(Key_Form + item, $thisinput, listcheck);
            $('.table-reponsive').css('display', 'block');
            return IsLoadFirst;
        }
    };
    //-------------------
    var Key_ThemeColor = "themeColor";
    var SetThemeColor = function (key) {
        localStorage.setItem(Key_ThemeColor, key);
    };
    var GetThemeColor = function () {
        if (localStorage.getItem(Key_ThemeColor) === null) {
            return 0;
        }
        else {
            return localStorage.getItem(Key_ThemeColor);
        }
    };
    return {
        loadFIlterAdvanced: loadFIlterAdvanced,
        keyHangHoa: NameScreenHangHoa,
        setitemFilterAdvanced: SetitemFilterAdvanced,
        LoadColumnGrid: LoadShowHideColumnGrid,
        AddColumnHidenGrid: AddClassIdForColumnGrid,
        LoadFirstColumnGrid: LoadFirstColumnGrid,
        SetThemeColor: SetThemeColor,
        GetThemeColor: GetThemeColor,
        CheckColumnGridWithObj: CheckColumnGridWithObj,
        RemoveLoHang: RemoveLoHang,
        LoadFormLoHang: LoadFormLoHang
    };
})();
function setColorThemeProperty(property, value) {
    document.documentElement.style.setProperty(property, value);
}
var commonStatisJs = commonStatisJs || (function () {
    var ChangeThemeColor = function (a, isSet) {
        var type = '';
        if (a !== null && a !== undefined) {
            type = a.toString();
        }
        if (isSet !== false) {
            LocalCaches.SetThemeColor(type);
        }
        switch (type) {
            case "0": {
                setColorThemeProperty('--color-main', '#1374ad ');
                setColorThemeProperty('--color-primary', '#19669a  ');
                setColorThemeProperty('--color-secondary', '#bae1f7 ');
                setColorThemeProperty('--color-special', '#ff4000  ');
            }
                break;
            case "1":
                {
                    setColorThemeProperty('--color-main', '#0aaddd ');
                    setColorThemeProperty('--color-primary', '#0aaddd  ');
                    setColorThemeProperty('--color-secondary', '#bae1f7 ');
                    setColorThemeProperty('--color-special', '#ff6b01  ');
                }

                break;
            case "2":
                {
                    setColorThemeProperty('--color-main', '#57BB49 ');
                    setColorThemeProperty('--color-primary', '#3a8230  ');
                    setColorThemeProperty('--color-secondary', '#eef8ec ');
                    setColorThemeProperty('--color-special', '#009FEF  ');
                }

                break;
            case "3":
                {
                    setColorThemeProperty('--color-main', '#a60c12 ');
                    setColorThemeProperty('--color-primary', '#a60c12  ');
                    setColorThemeProperty('--color-secondary', '#EE1B22 ');
                    setColorThemeProperty('--color-special', '#009FEF  ');
                }

                break;
            case "4":
                {
                    setColorThemeProperty('--color-main', '#ff6600 ');
                    setColorThemeProperty('--color-primary', '#b34700  ');
                    setColorThemeProperty('--color-secondary', '#EDBB99 ');
                    setColorThemeProperty('--color-special', '#009FEF  ');
                }

                break;
            case "5":
                {
                    setColorThemeProperty('--color-main', '#ec5885 ');
                    setColorThemeProperty('--color-primary', '#d43968');
                    setColorThemeProperty('--color-secondary', '#ffe3eb ');
                    setColorThemeProperty('--color-special', '#009FEF  ');
                }

                break;
            case "6":
                {
                    setColorThemeProperty('--color-main', '#674dff ');
                    setColorThemeProperty('--color-primary', '#1b00b3  ');
                    setColorThemeProperty('--color-secondary', '#e9e6ff ');
                    setColorThemeProperty('--color-special', '#009FEF  ');
                }

                break;
            case "7":
                {
                    setColorThemeProperty('--color-main', '#c68039 ');
                    setColorThemeProperty('--color-primary', '#8a5928  ');
                    setColorThemeProperty('--color-secondary', '#f9f2eb ');
                    setColorThemeProperty('--color-special', '#009FEF  ');
                }

                break;
            case "8":
                {
                    setColorThemeProperty('--color-main', ' linear-gradient(60deg, #2580B3 0%, #CBBACC 100%) ');
                    setColorThemeProperty('--color-primary', '#2580B3  ');
                    setColorThemeProperty('--color-secondary', '#d5eaf6 ');
                    setColorThemeProperty('--color-special', '#009FEF  ');
                }

                break;
            case "9":
                {
                    setColorThemeProperty('--color-main', ' linear-gradient(60deg,  rgba(252,93,174,1) 0%, rgba(253,155,120,1) 100%) ');
                    setColorThemeProperty('--color-primary', 'rgba(252,93,174,1)  ');
                    setColorThemeProperty('--color-secondary', '#fee6f3 ');
                    setColorThemeProperty('--color-special', '#009FEF  ');
                }

                break;
            case "10":
                {
                    setColorThemeProperty('--color-main', ' linear-gradient(60deg, #000851 0%, #1CB5E0 100%)   ');
                    setColorThemeProperty('--color-primary', '#1CB5E0');
                    setColorThemeProperty('--color-secondary', '#d2f1f9 ');
                    setColorThemeProperty('--color-special', '#009FEF  ');
                }

                break;
            case "11":
                {
                    setColorThemeProperty('--color-main', ' #000000 ');
                    setColorThemeProperty('--color-primary', '#262626 ');
                    setColorThemeProperty('--color-secondary', '#cccccc ');
                    setColorThemeProperty('--color-special', '#009FEF  ');
                }

                break;
            case "12":
                {
                    setColorThemeProperty('--color-main', 'linear-gradient(90deg, #4b6cb7 0%, #182848 100%) ');
                    setColorThemeProperty('--color-primary', '#4b6cb7  ');
                    setColorThemeProperty('--color-secondary', '#d9e1f2 ');
                    setColorThemeProperty('--color-special', '#009FEF  ');
                }

                break;
            case "13":
                {
                    setColorThemeProperty('--color-main', 'linear-gradient(60deg, #fee140  0%, #fa709a 100%) ');
                    setColorThemeProperty('--color-primary', '#fa709a  ');
                    setColorThemeProperty('--color-secondary', '#fdcedc ');
                    setColorThemeProperty('--color-special', '#009FEF  ');
                }

                break;
            case "14":
                {
                    setColorThemeProperty('--color-main', 'linear-gradient(60deg, #870000 0%, #190A05    100%)');
                    setColorThemeProperty('--color-primary', '#870000');
                    setColorThemeProperty('--color-secondary', '#6b8cae');
                    setColorThemeProperty('--color-special', '#009FEF');
                }

                break;
            case "15":
                {
                    setColorThemeProperty('--color-main', '#666666');
                    setColorThemeProperty('--color-primary', '#666666  ');
                    setColorThemeProperty('--color-secondary', '#999999 ');
                    setColorThemeProperty('--color-special', '#009FEF  ');
                }

                break;
            case "16":
                setColorThemeProperty('--color-main', '#0aaddd ');
                setColorThemeProperty('--color-primary', '#0885aa  ');
                setColorThemeProperty('--color-secondary', '#e7f8fe ');
                setColorThemeProperty('--color-special', '#ff6b01  ');

                break;
            default:
                setColorThemeProperty('--color-main', '#1374ad');//1374ad
                setColorThemeProperty('--color-primary', '#0aaddd  ');
                setColorThemeProperty('--color-secondary', '#bae1f7 ');
                setColorThemeProperty('--color-special', '#ff6b01  ');
                break;
        }
    };

    var PrintExtraReportTr = function (content) {
        var frame1 = $('<iframe />');
        frame1[0].name = "frame1";
        frame1.css({ "position": "absolute", "top": "-100000px", "width": "100%" });
        $("body").append(frame1);
        var frameDoc = frame1[0].contentWindow ? frame1[0].contentWindow : frame1[0].contentDocument.document ? frame1[0].contentDocument.document : frame1[0].contentDocument;
        frameDoc.document.open();
        frameDoc.document.write('<html><head>');
        frameDoc.document.write('<link href="/Content/bootstrap.css" rel="stylesheet" type="text/css" />');
        frameDoc.document.write('<link href="/Content/VariablesStyle.css" rel="stylesheet" type="text/css" />');
        frameDoc.document.write('<link href="/Content/ssoftvn.css" rel="stylesheet" type="text/css" />');
        frameDoc.document.write('<link href="/Content/font-awesome.css" rel="stylesheet" type="text/css" />');
        frameDoc.document.write('<link href="/Content/style.css" rel="stylesheet" type="text/css" />');
        frameDoc.document.write('</head><body>');
        frameDoc.document.write(content);
        frameDoc.document.write('</body></html>');
        frameDoc.document.close();
        setTimeout(function () {
            window.frames["frame1"].focus();
            window.frames["frame1"].print();
            frame1.remove();
        }, 1500);
    };
    var validatePhone = function (phone) {
        if (!validateNull(phone)) {
            var fld = phone.trim().replace(/\s+/g, '');
            var filter = /^((\+[1-9]{1,4}[ \-]*)|(\([0-9]{2,3}\)[ \-]*)|([0-9]{2,4})[ \-]*)*?[0-9]{3,4}?[ \-]*[0-9]{3,4}?$/;
            if (filter.test(fld)) {
                return true;
            }
            else {
                return false;
            }
        }
        return false;
    };
    var convertFormC = function (str) {
        if (!validateNull(str)) {
            return str.normalize('NFC');
        }
        return '';
    };
    var validateEmail = function (input) {
        if (!validateNull(input)) {
            var email = input.trim();
            var res = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            if (res.test(email)) {
                return true;
            }
        }
        return false;
    };
    var validateNull = function (input) {
        return input === null
            || input === undefined
            || input.toString().replace(/\s+/g, '') === "";
    };
    var parseDate = function (str) {
        var mdy = str.split('/');
        return new Date(mdy[2], mdy[0] - 1, mdy[1]);
    };
    var datediff = function (first, second) {
        //format mm/dd/yyyy
        // Take the difference between the dates and divide by milliseconds per day.
        // Round to nearest whole number to deal with DST.
        return Math.round((parseDate(second) - parseDate(first)) / (1000 * 60 * 60 * 24));
    };
    var change_alias = function (alias) {
        var str = alias;
        str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
        str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
        str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
        str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
        str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
        str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
        str = str.replace(/đ/g, "d");
        str = str.replace(/!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'|\"|\&|\#|\[|\]|~|\$|`|-|{|}|\||\\/g, "");
        str = str.replace(/ + /g, "");
        str = str.replace(/ /g, '');
        str = str.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, ""); // Huyền sắc hỏi ngã nặng 
        str = str.replace(/\u02C6|\u0306|\u031B/g, ""); // Â, Ê, Ă, Ơ, Ư
        str = str.trim();
        return str;
    };
    var convertDateToServer = function (input) {
        if (input !== null && input !== undefined && input !== '') {
            var result = input.split('/');
            if (result.length >= 3) { return result[1] + "/" + result[0] + "/" + result[2]; }
        }
        return "";
    };
    var convertDate = function (input) {
        if (!validateNull(input)) {
            return moment(input).format('DD/MM/YYYY');
        }
        return "";
    };
    var convertMMYY = function (input) {
        if (!validateNull(input)) {
            return moment(input).format('DD/MM');
        }
        return "";
    };
    var convertDateToDateServer = function (input) {
        if (!validateNull(input)) {
            return moment(input).format('YYYY-MM-DD HH:mm:ss');
        }
        return "";
    };
    var convertTime = function (input) {
        if (!validateNull(input)) {
            return moment(input).format('HH:mm');
        }
        return "";
    };
    var validateDate = function (dateString) {
        let dateformat = /^(0?[1-9]|1[0-2])[\/](0?[1-9]|[1-2][0-9]|3[01])[\/]\d{4}$/;

        // Match the date format through regular expression      
        if (dateString.match(dateformat)) {
            let operator = dateString.split('/');

            // Extract the string into month, date and year      
            let datepart = [];
            if (operator.length > 1) {
                pdatepart = dateString.split('/');
            }
            let month = parseInt(datepart[0]);
            let day = parseInt(datepart[1]);
            let year = parseInt(datepart[2]);

            // Create list of days of a month      
            let ListofDays = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
            if (month == 1 || month > 2) {
                if (day > ListofDays[month - 1]) {
                    ///This check is for Confirming that the date is not out of its range      
                    return false;
                }
            } else if (month == 2) {
                let leapYear = false;
                if ((!(year % 4) && year % 100) || !(year % 400)) {
                    leapYear = true;
                }
                if ((leapYear == false) && (day >= 29)) {
                    return false;
                } else
                    if ((leapYear == true) && (day > 29)) {
                        console.log('Invalid date format!');
                        return false;
                    }
            }
        } else {
            console.log("Invalid date format!");
            return false;
        }
        return true;
    }
    var convertVieToEng = function (obj) {
        if (!obj)
            return "";
        var str = obj;
        str = str.toLowerCase();
        str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
        str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
        str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
        str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
        str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
        str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
        str = str.replace(/đ/g, "d");
        str = str.replace(/^\-+|\-+$/g, "");

        // Some system encode vietnamese combining accent as individual utf-8 characters
        str = str.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, ""); // Huyền sắc hỏi ngã nặng 
        str = str.replace(/\u02C6|\u0306|\u031B/g, ""); // Â, Ê, Ă, Ơ, Ư
        return str;
    };
    var FormatVND = function (number) {
        if (number === null || number === undefined || !$.isNumeric(number)) {
            number = 0;
        }
        return number.toLocaleString('it-IT', { style: 'currency', currency: 'VND' });
    };

    var FormatNumber_andRound = function (number, decimalDot = 3) {
        if (number === undefined || number === null) {
            return 0;
        }
        else {
            number = parseFloat(number);
            number = Math.round(number * Math.pow(10, decimalDot)) / Math.pow(10, decimalDot);
            if (number !== null) {
                var lastone = number.toString().split('').pop();
                if (lastone !== '.') {
                    number = parseFloat(number);
                }
            }
            if (isNaN(number)) {
                number = 0;
            }
            return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        }
    };

    var formatNumberToFloat = function (str) {
        if (str === undefined || str === null) {
            return 0;
        }
        else {
            var value = parseFloat(str.toString().replace(/,/g, ''));
            if (isNaN(value)) {
                return 0;
            }
            else {
                return value;
            }
        }
    };

    var RoundDecimal = function (number, location = 2) {
        number = Math.round(number * Math.pow(10, location)) / Math.pow(10, location);
        if (number !== null) {
            var lastone = number.toString().split('').pop();
            if (lastone !== '.') {
                number = parseFloat(number);
            }
        }
        if (isNaN(number) || number === Infinity) {
            number = 0;
        }
        return number;
    };

    var InputFormatNumber = function (input) {
        if (validateNull(input))
            return "1";
        return input.toString().replace(/,/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    };
    var ShowMessageDanger = function (msg) {
        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + msg, "danger");
    };
    var ShowMessageSuccess = function (msg) {
        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + msg, "success");
    };
    var FirstChar_UpperCase = function (str) {
        if (str) {
          return  str.replace(/\w/, c => c.toUpperCase());
        }
        return '';
    }
    var Remove_LastComma = function (str) {
        if (str !== null && str !== undefined && str.length > 1) {
            return str.replace(/(^[,\s]+)|([,\s]+$)/g, '');
        }
        else {
            return '';
        }
    };
    var SubStringContent = function (input, lenth) {
        lenth = lenth || 150;
        if (!validateNull(input) && input.length > lenth) {
            return input.substring(0, lenth) + "...";
        }
        return input;
    };
    var sleep = function (time) {
        return new Promise((resolve) => setTimeout(resolve, time));
    };
    var CopyArray = function (ar) {
        return $.extend(true, [], ar);// Merge object2 into object1
    };
    var parseInt = function (input) {
        if (!validateNull(input)) {
            return parseInt(input);
        }
        return 0;
    };
    var CopyObject = function (item) {
        var obj = Object.assign({}, item);
        return obj;
    };
    var ConfirmDialog_OKCancel = function (title, message, onConfirm, fClose) {
        if (fClose === undefined) {
            fClose = function () {
                modal.modal("hide");
                return false;
            };
        }
        var modal = $("#modalPopuplgDelete");
        modal.modal("show");
        $('#modalPopuplgDelete #header-confirm-delete').empty().append(title);
        $("#confirmMessage").empty().append(message);
        $("#confirmOkDelete").off().one('click', onConfirm);
        $("#confirmCancel").off().one("click", fClose);
    };
    var keypressNumber_limitNumber = function (event, obj) {

        var keyCode = event.keyCode || event.which;
        var $this = $(obj).val();

        // 46(.), 48(0), 57(9)
        if ((keyCode !== 46 || $(obj).val().indexOf('.') !== -1) && (keyCode < 48 || keyCode > 57)) {
            if (event.which !== 46 || $this.indexOf('.') !== -1) {
                //alert('Chỉ được nhập một dấu .');
            }
            event.preventDefault();
        }
        // get postion current of cursor
        var pos = $(obj).getCursorPosition();
        if ($this.indexOf(".") > -1 && $this.split('.')[1].length > 2) {

            var lenNumber = $this.length;
            // if pos nam sau chu so thap phan --> khong cho add them so nua
            if (pos > lenNumber - 3) {
                event.preventDefault();
            }
        }
    };
    var tableRowFocus = function (ele) {
        $(ele).siblings().removeClass('active');

        if ($(ele).hasClass('active')) {
            $(ele).removeClass("active");
            $(ele).next().removeClass("active");
            document.getElementsByClassName('table-frame')[0].classList.remove('table-active');
        }
        else {
            $(ele).addClass("active");
            $(ele).next().addClass("active");
            document.getElementsByClassName('table-frame')[0].classList.add('table-active');
        }
    };

    var tableRowRemoveFocus = function () {
        $('.tr-prev-hide').removeClass('active');
        $('.op-js-tr-hide').removeClass('active');
        document.getElementsByClassName('table-frame')[0].classList.remove('table-active');
    };
    var checkSizeImage = function (elm) {
        var files = elm.target.files;// FileList object
        var countErrType = 0;
        var countErrSize = 0;
        var errFileSame = '';
        var err = '';

        // Check Type file & Size
        for (let i = 0; i < files.length; i++) {
            if (!files[i].type.match('image.*')) {
                countErrType += 1;
            }
            let size = parseFloat(files[i].size / 1024).toFixed(2);
            if (size > 2048) {
                countErrSize += 1;
            }
        }

        // remove comma ,
        if (errFileSame !== '') {
            errFileSame = errFileSame.substr(0, errFileSame.length - 2)
        }

        if (countErrType > 0) {
            err = countErrType + ' file chưa đúng định dạng. ';
        }

        if (countErrSize > 0) {
            if (errFileSame === '') {
                err = countErrSize + ' file có dung lượng > 2MB'
            }
        }

        if (err !== '') {
            ShowMessageDanger(err);
            return false;
        }
        return true;
    };

    var encodeURI = function (str) {
        if (validateNull(str)) {
            return '';
        }
        return encodeURIComponent(str);
    }
    var decodeURI = function (str) {
        if (validateNull(str)) {
            return '';
        }
        return decodeURIComponent(str);
    }
    var NPOI_ExportExcel = async function (url, method, data, fileName){
        const dataStream =  await $.ajax({
        url: url,
        type: method,
        data: data ? JSON.stringify(data) : null, 
        contentType: 'application/json',
        xhrFields: {
            responseType: 'blob'
        }
    }).done().then(function (blob) {
        return blob;
    });   
    if (dataStream) {
        let link = document.createElement('a');
        link.href = window.URL.createObjectURL(dataStream);
        link.download = fileName;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        return true;
    }
    return false;
    }

    return {
        ChangeThemeColor: ChangeThemeColor,
        URLEncoding: encodeURI,
        URLDecoding: decodeURI,
        CheckPhoneNumber: validatePhone,
        CheckEmail: validateEmail,
        CheckNull: validateNull,
        convertStrFormC: convertFormC,
        convertVarchar: change_alias,
        convertDateToServer: convertDateToServer,
        convertDateToDateServer: convertDateToDateServer,
        convertVieToEng: convertVieToEng,
        ConvertVnd: FormatVND,
        InputFormatVnd: InputFormatNumber,
        roundDecimal: RoundDecimal,
        FormatNumber3Digit: FormatNumber_andRound,
        FormatNumberToFloat: formatNumberToFloat,
        printExtraReport: PrintExtraReportTr,
        convertDateTime: convertDate,
        SubStringContent: SubStringContent,
        datediff: datediff,
        convertTime: convertTime,
        ShowMessageSuccess: ShowMessageSuccess,
        ShowMessageDanger: ShowMessageDanger,
        Remove_LastComma: Remove_LastComma,
        sleep: sleep,
        CopyArray: CopyArray,
        parseInt: parseInt,
        convertMMYY: convertMMYY,
        validateDate: validateDate,
        CopyObject: CopyObject,
        ConfirmDialog_OKCancel: ConfirmDialog_OKCancel,
        Keypress_limitNumber: keypressNumber_limitNumber,
        TableRowFocus: tableRowFocus,
        TableRowRemoveFocus: tableRowRemoveFocus,
        FirstChar_UpperCase: FirstChar_UpperCase,
        checkSizeImage: checkSizeImage,
        NPOI_ExportExcel: NPOI_ExportExcel
    };
})();
commonStatisJs.ChangeThemeColor(LocalCaches.GetThemeColor(), false);
Date.prototype.addDays = function (days) {
    var date = new Date(this.valueOf());
    date.setDate(date.getDate() + days);
    return date;
};
Array.prototype.contains = function (a) {
    return this.indexOf(a) !== -1;
};


$(document).on('show.bs.modal', '.modal', function (event) {
    var zIndex = 1040 + (10 * $('.modal:visible').length);
    $(this).css('z-index', zIndex);
    setTimeout(function () {
        $('.modal-backdrop').not('.modal-stack').css('z-index', zIndex - 1).addClass('modal-stack');
    }, 0);
});