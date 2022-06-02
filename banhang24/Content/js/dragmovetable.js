var MyLib = function ( $specialRows) {
    console.log(1);
    var title = [];
    var buildFooterRows = function () {
        $specialRows.each(function (i) {
            var $row = $(this);
            $row.empty().html(title[i]);
        });
    };
    var buildFooterTemplate =  '<td></td><td></td><td></td><td></td>';
    var buildFooterTemplates = function () {
        title = [];
        $specialRows.each(function () {
            var $row = $(this);
            title.push($row[0].innerHTML);
            $row.empty().html(buildFooterTemplate);
        });
    };
    return {
        startDragDropMove: function ($this ) {
            $this.sorttable({
                placeholder: 'placeholder',
                opacity: 0.5, axis: "x"
            }).disableSelection();
        },
        refresh: function ($this,$thead) {
            this.startDragDropMove($this);
        },
}
};