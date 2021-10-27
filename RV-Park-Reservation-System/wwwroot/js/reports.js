var dtsiteCat;
var dtsite;
var dtsiteRate;
var dtspecEvent;

$(document).ready(function () {
    loadLists();
});

function loadLists() {
    dtsiteCat = $('#DTsiteCat').DataTable({
        "ajax": {
            "url": "/api/siteCategory",
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

function DeleteCat(url) {
    swal({
        title: "Are you sure you want to delete?",
        text: "You will not be able to restore this data!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: 'DELETE',
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dtsiteCat.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}

function DeleteSiteRate(url) {
    swal({
        title: "Are you sure you want to delete?",
        text: "You will not be able to restore this data!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: 'DELETE',
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dtsiteRate.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}

function DeleteSpecialEvent(url) {
    swal({
        title: "Are you sure you want to delete?",
        text: "You will not be able to restore this data!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: 'DELETE',
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dtspecEvent.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}