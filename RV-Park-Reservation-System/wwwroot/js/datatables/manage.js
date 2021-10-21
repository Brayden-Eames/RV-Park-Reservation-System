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
            {
                data: "siteCategory", width: "35%",
                "render": function (data) {
                    return `<div class="text-center">
                            <a href="/Admin/SiteCategories/Upsert?id=${data}"
                            class ="btn btn-success text-white style="cursor:pointer; width=100px;"><i class="far fa-edit"></i> Edit</a>
                            <a onClick=DeleteCat('/api/siteCategory/'+${data})
                            class ="btn btn-danger text-white style="cursor:pointer; width=100px;"><i class="far fa-trash-alt"></i> Delete</a>
                    </div>`;
                }
            }
        ],
        "language": {
            "emptyTable": "no data found."
        },
        "width": "100%"
    });

    dtsite = $('#DTsite').DataTable({
        "ajax": {
            "url": "/api/site",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { data: "siteNumber", width: "20%" },
            { data: "siteLength", width: "20%" },
            { data: "siteDescription", width: "40%" },
            {
                data: "siteID", width: "20%",
                "render": function (data) {
                    return `<div class="text-center">
                            <a href="/Admin/Sites/Upsert?id=${data}"
                            class ="btn btn-success text-white style="cursor:pointer; width=100px;"><i class="far fa-edit"></i> Edit</a>
                            <a onClick=DeleteSite('/api/site/'+${data})
                            class ="btn btn-danger text-white style="cursor:pointer; width=100px;"><i class="far fa-trash-alt"></i> Delete</a>
                    </div>`;
                }
            }
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
            {
                data: "rateID", width: "20%",
                "render": function (data) {
                    return `<div class="text-center">
                            <a href="/Admin/SiteRates/Upsert?id=${data}"
                            class ="btn btn-success text-white style="cursor:pointer; width=100px;"><i class="far fa-edit"></i> Edit</a>
                            <a onClick=DeleteSiteRate('/api/site_rate/'+${data})
                            class ="btn btn-danger text-white style="cursor:pointer; width=100px;"><i class="far fa-trash-alt"></i> Delete</a>
                    </div>`;
                }
            }
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

function DeleteSite(url) {
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
                        dtsite.ajax.reload();
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