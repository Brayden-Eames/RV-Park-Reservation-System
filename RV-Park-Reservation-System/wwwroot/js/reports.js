var DThistoricReservations;
var dtsite;
var DTvacancyReport;
var DTActivity;

function generateVacantSitesData() {
    destroyVacantTable(); //destroys current table to prevent errors.
    let startDate = document.getElementById('startDate').value.toString();
    let endDate = document.getElementById('endDate').value.toString();
    getVacantSitesList(startDate, endDate);
}

function generateHistoricSitesData() {
    destroyHistoricTable(); //destroys current table to prevent errors.
    let startDate = document.getElementById('historicStartDate').value.toString();
    let endDate = document.getElementById('historicEndDate').value.toString();
    getHistoricReservationList(startDate, endDate);
}

function destroyVacantTable() {
    var table = $('#DTvacancyReport').DataTable()
    if (table) {        
        table.destroy();        
    }
}

function destroyHistoricTable() {
    var table = $('#DThistoricReservations').DataTable()
    if (table) {
        table.destroy();
    }
}

function getVacantSitesList(startDate, endDate) {
    
        DTvacancyReport = $('#DTvacancyReport').DataTable({
        dom: 'Bfrtip',
        buttons: [ 'csv', 'excel', 'pdf', 'print'],
        "ajax": {
                "url": '/api/vacancyReport/?startDate=' + startDate + '&endDate=' + endDate,
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { data: "site", width: "15%" },
            { data: "description", width: "20%" },
            { data: "length", width: "15%" },
            { data: "available", width: "30%" },
            { data: "rate", width: "20%" }
        ],
        "language": {
            "emptyTable": "no data found."
        },
        "width": "100%"
    });
}

function getHistoricReservationList(startDate, endDate) {
    DThistoricReservations = $('#DThistoricReservations').DataTable({
        dom: 'Bfrtip',        
        buttons: [
            'csv', 'excel', 'pdf', 'print'
        ],
        "ajax": {
            "url": '/api/historicReservationReport/?startDate='+ startDate + '&endDate=' + endDate,
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { data: "name", width: "14%" },
            { data: "phone", width: "14%" },
            { data: "email", width: "16%" },
            { data: "siteNumber", width: "14%" },
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

//activityList is the upcoming Checkin/Check Out activity. 
function getActivityList() {   

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
                    var inputId = title.replace(/ /g, '');
                    $(cell).html('<input id="'+inputId+'" class="w-100" type="text" placeholder="Sort: '+title+'" />');

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
            "url": "/api/reports", /*this controller is used only for the activity list or the check In/ Check Out report*/
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
