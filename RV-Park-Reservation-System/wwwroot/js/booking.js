﻿


$(document).ready(function () {
    console.log("ready!");
   
    $('.alert').alert()
    $(".datepicker").datepicker({ minDate: 1, maxDate: "+6m" })
    $("#timepicker").timepicker({ 'minTime': '11:00 AM', 'maxTime': '9:00 PM', step: '30' })
});

$('#myModal').on('shown.bs.modal', function () {
    $('#myInput').trigger('focus')
})

function checkDates() {
    var startDate = new Date(document.getElementById('startDate').value);
    var endDate = new Date(document.getElementById('endDate').value);
    console.log(document.getElementById('endDate').value);
    if (document.getElementById('startDate').value != "" && document.getElementById('endDate').value != "") {
        if (document.getElementById('startDate').value > document.getElementById('endDate').value) {
            swal('Error', 'Please select an end date that is past the start date for this reservation', 'error')
            return false;
        }
        var dayDiff = Math.round(Math.abs((endDate - startDate) / (1000 * 60 * 60 * 24)));
        console.log(dayDiff);
        if ((startDate.getMonth() >= 3 && startDate.getMonth() <= 9) || (endDate.getMonth() >= 3 && endDate.getMonth() <= 9)) {
            if (dayDiff > 14) {
                swal('warning', 'Maximum length of stay April-October is 14 consecutive days except for those traveling on PCS orders. Long term stays are allowed October 15th-April 1st. For detailed information on this policy, please contact the FamCamp Office.', 'warning')
                return false;
            }

        }


        var totalCost = (dayDiff * 25);
        document.getElementById('totalCost').value = totalCost;
        console.log(totalCost);


	}



   
}
function loadReservations() {
    if (document.getElementById('endDate').value == null || document.getElementById('startDate').value == null || document.getElementById('ddlVehicleLength').value == 0) {
        swal('Error', 'Please select a start date, end date, and vehicle type to see available resevations', 'error')
        return false;
	}
    $('#btnSeeReservations').css('display', 'none');
    $('#ddlSitesDiv').css('display', 'block');


    var dates = { date1: document.getElementById('startDate').value, date2: document.getElementById('endDate').value, vehicleLength: document.getElementById('ddlVehicleLength').value};

    $.getJSON("/api/BookingReservations", dates, function (sites) {
        /* Remove all options from the select list */
        $('#ddlSites').empty();
        $('#ddlSites').append($('<option></option>').val(0).html("- Please select a Lot -"));
        /* Insert the new ones from the array above */
        $.each(sites, function (i, p) {
            $('#ddlSites').append($('<option></option>').val(p).html("Lot " + p));
        });

    });


}
function continuePayment() {
    $('#btnPayment').css('display', 'block');
}
function checkAgreement() {
    $('#breedPolicyAgreement').prop("checked", true);
}

function pets() {

        $('.petWrapper').css('display', 'block');
    $('#petsCheckbox').css('display', 'none');
}