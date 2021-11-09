var dtsiteCat;
var dtsite;
var dtsiteRate;
var dtspecEvent;

$(document).ready(function () {
    loadLists();
});

function loadLists() {
    dtdodAffiliation = $('#DTdodAffiliation').DataTable({
        "ajax": {
            "url": "/api/dodAffiliation",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { data: "dodAffiliationType", width: "80%" },
            {
                data: "dodAffiliationID", width: "20%",
                "render": function (data) {
                    return `<div class="text-center">
                            <a href="/Admin/DODAffiliations/Upsert?id=${data}"
                            class ="btn btn-success text-white style="cursor:pointer; width=100px;"> <i class="far fa-edit"></i> Edit</a>                            
                            <a onClick=DeleteDODAffiliation('/api/dodAffiliation/'+${data})
                            class ="btn btn-danger text-white style="cursor:pointer; width=100px;"> <i class="far fa-trash-alt"></i> Delete</a>
                    </div>`;
                }
            }
        ],
        "language": {
            "emptyTable": "no data found."
        }
    });

    dtpaymentReason = $('#DTpaymentReason').DataTable({
        "ajax": {
            "url": "/api/paymentReason",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { data: "payReasonName", width: "80%" },
            {
                data: "payReasonID", width: "20%",
                "render": function (data) {
                    return `<div class="text-center">
                            <a href="/Admin/PaymentReasons/Upsert?id=${data}"
                            class ="btn btn-success text-white style="cursor:pointer; width=100px;"> <i class="far fa-edit"></i> Edit</a>                            
                            <a onClick=DeletePaymentReason('/api/paymentReason/'+${data})
                            class ="btn btn-danger text-white style="cursor:pointer; width=100px;"> <i class="far fa-trash-alt"></i> Delete</a>
                    </div>`;
                }
            }
        ],
        "language": {
            "emptyTable": "no data found."
        }
    });

    dtpaymentType = $('#DTpaymentType').DataTable({
        "ajax": {
            "url": "/api/paymentType",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { data: "payType", width: "80%" },
            {
                data: "payTypeID", width: "20%",
                "render": function (data) {
                    return `<div class="text-center">
                            <a href="/Admin/PaymentTypes/Upsert?id=${data}"
                            class ="btn btn-success text-white style="cursor:pointer; width=100px;"> <i class="far fa-edit"></i> Edit</a>                            
                            <a onClick=DeletePaymentType('/api/paymentType/'+${data})
                            class ="btn btn-danger text-white style="cursor:pointer; width=100px;"> <i class="far fa-trash-alt"></i> Delete</a>
                    </div>`;
                }
            }
        ],
        "language": {
            "emptyTable": "no data found."
        }
    });

    dtreservationStatus = $('#DTreservationStatus').DataTable({
        "ajax": {
            "url": "/api/reservationStatus",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { data: "resStatusName", width: "30%" },
            { data: "resStatusDescription", width: "50%" },
            {
                data: "resStatusID", width: "20%",
                "render": function (data) {
                    return `<div class="text-center">
                            <a href="/Admin/ReservationStatuses/Upsert?id=${data}"
                            class ="btn btn-success text-white style="cursor:pointer; width=100px;"> <i class="far fa-edit"></i> Edit</a>                            
                            <a onClick=DeleteReservationStatus('/api/reservationStatus/'+${data})
                            class ="btn btn-danger text-white style="cursor:pointer; width=100px;"> <i class="far fa-trash-alt"></i> Delete</a>
                    </div>`;
                }
            }
        ],
        "language": {
            "emptyTable": "no data found."
        }
    });

    dtsecurityQuestion = $('#DTsecurityQuestion').DataTable({
        "ajax": {
            "url": "/api/securityQuestion",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { data: "questionText", width: "80%" },
            {
                data: "questionID", width: "20%",
                "render": function (data) {
                    return `<div class="text-center">
                            <a href="/Admin/SecurityQuestions/Upsert?id=${data}"
                            class ="btn btn-success text-white style="cursor:pointer; width=100px;"> <i class="far fa-edit"></i> Edit</a>                            
                            <a onClick=DeleteSecurityQuestion('/api/securityQuestion/'+${data})
                            class ="btn btn-danger text-white style="cursor:pointer; width=100px;"> <i class="far fa-trash-alt"></i> Delete</a>
                    </div>`;
                }
            }
        ],
        "language": {
            "emptyTable": "no data found."
        }
    });

    dtserviceStatus = $('#DTserviceStatus').DataTable({
        "ajax": {
            "url": "/api/serviceStatus",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { data: "serviceStatusType", width: "80%" },
            {
                data: "serviceStatusID", width: "20%",
                "render": function (data) {
                    return `<div class="text-center">
                            <a href="/Admin/ServiceStatuses/Upsert?id=${data}"
                            class ="btn btn-success text-white style="cursor:pointer; width=100px;"> <i class="far fa-edit"></i> Edit</a>                            
                            <a onClick=DeleteServiceStatus('/api/serviceStatus/'+${data})
                            class ="btn btn-danger text-white style="cursor:pointer; width=100px;"> <i class="far fa-trash-alt"></i> Delete</a>
                    </div>`;
                }
            }
        ],
        "language": {
            "emptyTable": "no data found."
        }
    });

    dtsite = $('#DTsites').DataTable({
        "ajax": {
            "url": "/api/sites",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { data: "site_Category.siteCategoryName", width: "15%" },
            { data: "siteNumber", width: "5%" },
            { data: "siteLength", width: "5%" },
            { data: "siteDescription", width: "25%" },
            { data: "siteLastModifiedDate", width: "12%" },
            { data: "siteLastModifiedBy", width: "12%" },
            {
                data: "siteID", width: "25%",
                "render": function (data) {
                    return `<div class="text-center">
                            <a href="/Admin/Sites/Upsert?id=${data}"
                            class ="btn btn-success text-white style="cursor:pointer; width=100px;"> <i class="far fa-edit"></i> Edit</a>                            
                            <a onClick=DeleteSite('/api/sites/'+${data})
                            class ="btn btn-danger text-white style="cursor:pointer; width=100px;"> <i class="far fa-trash-alt"></i> Delete</a>
                    </div>`;
                }
            }
        ],
        "language": {
            "emptyTable": "no data found."
        }
    });

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
                            class ="btn btn-success text-white style="cursor:pointer;"><i class="far fa-edit"></i> Edit</a>
                            <a onClick=DeleteCat('/api/siteCategory/'+${data})
                            class ="btn btn-danger text-white style="cursor:pointer;"><i class="far fa-trash-alt"></i> Delete</a>
                    </div>`;
                }
            }
        ],
        "language": {
            "emptyTable": "no data found."
        }
    });

    dtsiteRate = $('#DTsiteRate').DataTable({
        "ajax": {
            "url": "/api/site_rate",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { data: "rateAmount", width: "20%", "render": function (data) { return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', minimumFractionDigits: 2 }).format(data); }  },
            { data: "rateStartDate", width: "20%", "render": function (data) { return moment(data).format("MM/DD/YYYY, hh:mm a"); } },
            { data: "rateEndDate", width: "20%", "render": function (data) { return moment(data).format("MM/DD/YYYY, hh:mm a"); } },
            { data: "siteCategoryID", width: "20%" },
            {
                data: "rateID", width: "20%",
                "render": function (data) {
                    return `<div class="text-center">
                            <a href="/Admin/SiteRates/Upsert?id=${data}"
                            class ="btn btn-success text-white style="cursor:pointer;"><i class="far fa-edit"></i> Edit</a>
                            <a onClick=DeleteSiteRate('/api/site_rate/'+${data})
                            class ="btn btn-danger text-white style="cursor:pointer;"><i class="far fa-trash-alt"></i> Delete</a>
                    </div>`;
                }
            }
        ],
        "language": {
            "emptyTable": "no data found."
        }
    });

    dtspecEvent = $('#DTspecEvent').DataTable({
        "ajax": {
            "url": "/api/specialEvent",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { data: "eventName", width: "10%" },
            { data: "eventStartDate", width: "10%", "render": function (data) { return moment(data).format("MM/DD/YYYY, hh:mm a"); } },
            { data: "eventEndDate", width: "10%", "render": function (data) { return moment(data).format("MM/DD/YYYY, hh:mm a"); } },
            { data: "eventDescription", width: "30%" },
            { data: "daily_Surcharge", width: "10%", "render": function (data) { return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', minimumFractionDigits: 2 }).format(data); } },
            { data: "weekly_Surcharge", width: "10%", "render": function (data) { return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', minimumFractionDigits: 2 }).format(data); } },
            {
                data: "eventID", width: "20%",
                "render": function (data) {
                    return `<div class="text-center">
                            <a href="/Admin/SpecialEvents/Upsert?id=${data}"
                            class ="btn btn-success text-white style="cursor:pointer;"><i class="far fa-edit"></i> Edit</a>
                            <a onClick=DeleteSpecialEvent('/api/specialEvent/'+${data})
                            class ="btn btn-danger text-white style="cursor:pointer;"><i class="far fa-trash-alt"></i> Delete</a>
                    </div>`;
                }
            }
        ],
        "language": {
            "emptyTable": "no data found."
        }
    });

    dtvehicleType = $('#DTvehicleType').DataTable({
        "ajax": {
            "url": "/api/vehicleType",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { data: "typeName", width: "30%" },
            { data: "typeDescription", width: "40%" },
            {
                data: "typeID", width: "30%",
                "render": function (data) {
                    return `<div class="text-center">
                            <a href="/Admin/VehicleTypes/Upsert?id=${data}"
                            class ="btn btn-success text-white style="cursor:pointer;"><i class="far fa-edit"></i> Edit</a>
                            <a onClick=DeleteVehicleType('/api/vehicleType/'+${data})
                            class ="btn btn-danger text-white style="cursor:pointer;"><i class="far fa-trash-alt"></i> Delete</a>
                    </div>`;
                }
            }
        ],
        "language": {
            "emptyTable": "no data found."
        }
    });
}

function DeleteDODAffiliation(url) {
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
                        dtdodAffiliation.ajax.reload();
                    }
                    else
                        toastr.error(data.message);
                }
            })
        }
    })
}

function DeletePaymentReason(url) {
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
                        dtpaymentReason.ajax.reload();
                    }
                    else
                        toastr.error(data.message);
                }
            })
        }
    })
}

function DeletePaymentType(url) {
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
                        dtpaymentType.ajax.reload();
                    }
                    else
                        toastr.error(data.message);
                }
            })
        }
    })
}

function DeleteReservationStatus(url) {
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
                        dtreservationStatus.ajax.reload();
                    }
                    else
                        toastr.error(data.message);
                }
            })
        }
    })
}

function DeleteSecurityQuestion(url) {
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
                        dtsecurityQuestion.ajax.reload();
                    }
                    else
                        toastr.error(data.message);
                }
            })
        }
    })
}

function DeleteServiceStatus(url) {
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
                        dtserviceStatus.ajax.reload();
                    }
                    else
                        toastr.error(data.message);
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
                    }
                    else
                        toastr.error(data.message);
                }
            })
        }
    })
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

function DeleteVehicleType(url) {
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
                        dtvehicleType.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}