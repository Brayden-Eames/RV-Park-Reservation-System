$(document).ready(function () {
    $('.alert').alert()
    $(".datepicker").datepicker({ minDate: 1, maxDate: "+6m" })
    $("#timepicker").timepicker({ 'minTime': '11:00 AM', 'maxTime': '9:00 PM', step: '30' })
});


$('#myModal').on('shown.bs.modal', function () {
    $('#myInput').trigger('focus')
})


var reservationBufferCount = 0;
function checkBuffer() {
    
    var date = { date: document.getElementById('startDate').value };

    var json = $.getJSON("/api/ReservationBuffer", date , function (reservation) {
		if (reservation > 0) {
            reservationBufferCount = reservation;

        }
		else {
            reservationBufferCount = reservation;
		}

    });
    json.done(function() {
        if (reservationBufferCount > 0) {
            swal('Error', 'There must be 2 weeks between reservations.', 'error')
            return true;
        }
        else {
            return false;
        }
    })
   

}




function checkDates() {


    var startDate = new Date(document.getElementById('startDate').value);
    var endDate = new Date(document.getElementById('endDate').value);

    if (document.getElementById('startDate').value != "" && document.getElementById('endDate').value != "") {
        if (document.getElementById('startDate').value > document.getElementById('endDate').value) {
            swal('Error', 'Please select an end date that is past the start date for this reservation', 'error')

            return false;
        }
        var dayDiff = Math.round(Math.abs((endDate - startDate) / (1000 * 60 * 60 * 24)));
       
        if ((startDate.getMonth() >= 3 && startDate.getMonth() <= 9) || (endDate.getMonth() >= 3 && endDate.getMonth() <= 9)) {
            if (dayDiff > 14) {
                swal('warning', 'Maximum length of stay April-October is 14 consecutive days except for those traveling on PCS orders. Long term stays are allowed October 15th-April 1st. For detailed information on this policy, please contact the FamCamp Office.', 'warning')
                return false;
            }

        }
        var totalCost = (dayDiff * 25);
        document.getElementById('totalCost').value = totalCost;

    }
    
}
function checkinput() {
    var startDate = new Date(document.getElementById('startDate').value);
    var endDate = new Date(document.getElementById('endDate').value);

    //Checks start date is less than end date
    if (document.getElementById('startDate').value != "" && document.getElementById('endDate').value != "") {
        if (document.getElementById('startDate').value > document.getElementById('endDate').value) {
            swal('Error', 'Please select an end date that is past the start date for this reservation', 'error')

            return false;
        }
    }

    //Checks if difference in dates is less than 14 while in busy months
    var dayDiff = Math.round(Math.abs((endDate - startDate) / (1000 * 60 * 60 * 24)));
    if ((startDate.getMonth() >= 3 && startDate.getMonth() <= 9) || (endDate.getMonth() >= 3 && endDate.getMonth() <= 9)) {
        if (dayDiff > 14) {
            swal('warning', 'Maximum length of stay April-October is 14 consecutive days except for those traveling on PCS orders. Long term stays are allowed October 15th-April 1st. For detailed information on this policy, please contact the FamCamp Office.', 'warning')
            return false;
        }

    }

    //checks if > 14 days between reservations
    checkBuffer();
    if (reservationBufferCount > 0) {
        return false;
    }

    if ($('#numAdults').val() <=0) {
        swal('warning', 'Please enter a valid number of adults. ', 'warning')
        return false;
    }
    console.log(document.getElementById('numPets').value);
    if (document.getElementById('numPets').value > 0 &&  document.getElementById('breedPolicyAgreement').checked == false) {
        swal('warning', 'Please acknolwdge FamCamps pet policy. ', 'warning')
        return false;
    }


    
    
    return true;
}

function loadReservations() {

    if (document.getElementById('endDate').value == '' || document.getElementById('startDate').value == ''
        || (document.getElementById('ddlVehicleLength').value == 20 && document.getElementById('ddlVehicleType').value != 7))
    {
        swal('Error', 'Please select a start date, end date, and vehicle type to see available resevations', 'error')
        return false;
	}



    var dates = { date1: document.getElementById('startDate').value, date2: document.getElementById('endDate').value, vehicleLength: document.getElementById('ddlVehicleLength').value};
  
    $.getJSON("/api/BookingReservations", dates, function (sites) {
        var count = Object.keys(sites).length;

        if (count == 0) {
            $('#btnSeeReservations').css('display', 'block');
            $('#ddlSitesDiv').css('display', 'none');
            swal('Error', 'No reservations available for the selected dates and vehicle length.', 'error')
            return false;
        }
        $('#btnSeeReservations').css('display', 'none');
        $('#ddlSitesDiv').css('display', 'block');
        /* Remove all options from the select list */
        $('#ddlSites').empty();
        $('#ddlSites').append($('<option></option>').val(0).html("- Please select a Lot -"));
        /* Insert the new ones from the array above */
        $.each(sites, function (i, p) {
			if (p['siteID'] == 47 ) {
                $('#ddlSites').append($('<option></option>').val(p['siteID']).html("Site: " + p['siteNumber'] + " Type: " + p['siteDescription'] + " Price: 17.00$/Night"));
            }
			else {
                $('#ddlSites').append($('<option></option>').val(p['siteID']).html("Site: " + p['siteNumber'] + " Type: " + p['siteDescription'] + " Price: 25.00$/Night"));

			}
        });

    });


}
function continuePayment() {
    $('#btnPayment').css('display', 'block');
}
function checkAgreement() {
    $('#breedPolicyAgreement').prop("checked", true);
}
function hideLots() {
    $('#btnPayment').css('display', 'none');
    $('#ddlSitesDiv').css('display', 'none');
    $('#btnSeeReservations').css('display', 'block');
}

function VehicleSelected() {
    console.log($('#ddlVehicleType').val())
	if ($('#ddlVehicleType').val() == 7) {
        $('#ddlVehicleLength').css('display', 'none');
        $('#ddlVehicleLength').val(20).change() ;
    }
	else {
        $('#ddlVehicleLength').css('display', 'block');
	}
}
function pets() {

        $('.petWrapper').css('display', 'block');
    $('#petsCheckbox').css('display', 'none');
}


function checkChange(option) {
    if (option.value == 'newUser') {
        $('.createAccount').css('display', 'block');
        $('.createAccount').css('visibility', 'visible');
        $('.genericAccount').css('display', 'none');
        $('.genericAccount').css('visibility', 'hidden');
        document.getElementById("inputFirstName").value = "";
        document.getElementById("inputLastName").value = "";
        document.getElementById("inputEmail").value = "";
        document.getElementById("inputPhoneNumber").value = "";
        document.getElementById("ddlDODAffiliation").value = "";
        document.getElementById("ddlServiceStatusType").value = "";
    }
    if (option.value == 'genericUser') {
        $('.genericAccount').css('display', 'block');
        $('.genericAccount').css('visibility', 'visible');
        $('.createAccount').css('display', 'none');
        $('.createAccount').css('visibility', 'hidden');
        document.getElementById("inputFirstName").value = "Generic";
        document.getElementById("inputLastName").value = "Account";
        document.getElementById("inputEmail").value = "genericaccount@gmail.com";
        document.getElementById("inputPhoneNumber").value = "8888888888";
        document.getElementById("ddlDODAffiliation").value = 1;
        document.getElementById("ddlServiceStatusType").value = 1;

    }
    if (option.value == 'chooseDefault') {
        $('.genericAccount').css('display', 'none');
        $('.createAccount').css('display', 'none');
    }
}