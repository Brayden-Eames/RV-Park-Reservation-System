var dtsiteCat;
var dtsite;
var dtsiteRate;
var DTActivity;

$(document).ready(function () {
    loadLists();
});


function loadLists() {

    //dtsiteCat = $('#DTsiteCat').DataTable({
    //    dom: 'Bfrtip',
    //    buttons: [
    //        'csv', 'excel', 'pdf', 'print'
    //    ],
    //    "ajax": {
    //        "url": "/api/reports",
    //        "type": "GET",
    //        "datatype": "json"
    //    },
    //    "columns": [
    //        { data: "siteCategoryName", width: "20%" },
    //        { data: "siteCategoryDescription", width: "35%" },
    //        { data: "locationID", width: "20%" },            
    //    ],
    //    "language": {
    //        "emptyTable": "no data found."
    //    },
    //    "width": "100%"
    //});

    //dtsiteRate = $('#DTsiteRate').DataTable({
    //    dom: 'Bfrtip',
    //    buttons: [ 'csv', 'excel', 'pdf', 'print'],
    //    "ajax": {
    //        "url": "/api/site_rate",
    //        "type": "GET",
    //        "datatype": "json"
    //    },
    //    "columns": [
    //        { data: "rateAmount", width: "20%" },
    //        { data: "rateStartDate", width: "20%" },
    //        { data: "rateEndDate", width: "20%" },
    //        { data: "siteCategoryID", width: "20%" },           
    //    ],
    //    "language": {
    //        "emptyTable": "no data found."
    //    },
    //    "width": "100%"
    //});

    $('#DTActivity thead tr').clone(true).addClass('filters').appendTo('#DTActivity thead'); //this clones the first row

    DTActivity = $('#DTActivity').DataTable({
        orderCellsTop: true,
        fixedHeader: true,
        initComplete: function () {
            var api = this.api();

            // For each column
            api.columns().eq(0).each(function (colIdx) {
                    // Set the header cell to contain the input element
                    var cell = $('.filters th').eq(
                        $(api.column(colIdx).header()).index()
                    );
                    var title = $(cell).text().trim();
                    $(cell).html('<input class="w-100" type="text" placeholder="Sort: '+title+'" />');

                    // On every keypress in this input
                    $(
                        'input',
                        $('.filters th').eq($(api.column(colIdx).header()).index())
                    )
                        .off('keyup change').on('keyup change', function (e) {
                            e.stopPropagation();

                            // Get the search value
                            $(this).attr('title', $(this).val());
                            var regexr = '({search})'; //$(this).parents('th').find('select').val();

                            var cursorPosition = this.selectionStart;
                            // Search the column for that value
                            api.column(colIdx).search(
                                    this.value != ''
                                        ? regexr.replace('{search}', '(((' + this.value + ')))')
                                        : '',
                                    this.value != '',
                                    this.value == ''
                            ).draw();

                            $(this).focus()[0].setSelectionRange(cursorPosition, cursorPosition);
                        });
                    });
             },

        dom: 'Bfrtip',
        buttons: ['csv', 'excel', 'pdf', 'print'],
        "ajax": {
            "url": "/api/reports",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [            
            { data: 'siteNumber', width: "14%" },
            { data: 'name', width: "14%" },
            { data: "nights", width: "14%" },
            { data: "checkIn", width: "14%" },
            { data: "checkOut", width: "14%" },
            { data: "status", width: "14%" },
        ],        
        "language": {
            "emptyTable": "no data found."
        },       
        "width": "100%"
    });
}
