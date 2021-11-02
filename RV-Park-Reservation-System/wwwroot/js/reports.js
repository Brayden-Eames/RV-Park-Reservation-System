var dtsiteCat;
var dtsite;
var dtsiteRate;
var dtspecEvent;

$(document).ready(function () {
    loadLists();
});


function loadLists() {
    dtsiteCat = $('#DTsiteCat').DataTable({
        dom: 'Bfrtip',
        buttons: [
            'csv', 'excel', 'pdf', 'print'
        ],
        "ajax": {
            "url": "/api/reports",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { data: "siteCategoryName", width: "20%" },
            { data: "siteCategoryDescription", width: "35%" },
            { data: "locationID", width: "20%" },            
        ],
        "language": {
            "emptyTable": "no data found."
        },
        "width": "100%"
    });

    dtsiteRate = $('#DTsiteRate').DataTable({
        dom: 'Bfrtip',
        buttons: [ 'csv', 'excel', 'pdf', 'print'],
        "ajax": {
            "url": "/api/site_rate",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { data: "rateAmount", width: "20%" },
            { data: "rateStartDate", width: "20%" },
            { data: "rateEndDate", width: "20%" },
            { data: "siteCategoryID", width: "20%" },           
        ],
        "language": {
            "emptyTable": "no data found."
        },
        "width": "100%"
    });

    dtspecEvent = $('#DTspecEvent').DataTable({
        dom: 'Bfrtip',
        buttons: ['csv', 'excel', 'pdf', 'print'],
        "ajax": {
            "url": "/api/specialEvent",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { data: "eventName", width: "20%" },
            { data: "eventStartDate", width: "15%" },
            { data: "eventEndDate", width: "15%" },
            { data: "eventDescription", width: "25%" },            
        ],
        "language": {
            "emptyTable": "no data found."
        },
        "width": "100%"
    });
}



