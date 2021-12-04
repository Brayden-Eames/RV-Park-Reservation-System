

//Loads the date picker, timepicker, and alert module. 
$(document).ready(function () {
    $('.alert').alert()
    $(".datepicker").datepicker({ minDate: -5, maxDate: "+6m" })
    $("#timepicker").timepicker({ 'minTime': '11:00 AM', 'maxTime': '9:00 PM', step: '30' })
});

//Loads the modal modules from bootstrap. 
$('#myModal').on('shown.bs.modal', function () {
    $('#myInput').trigger('focus')
})


//Global reservation count to show checkbuffer method. 
var reservationBufferCount = 0;

//Checks that there are 2 weeks between client reservations. 
function checkBuffer() {

    //Gets start date value 
    var date = { date: document.getElementById('startDate').value };

    //gets the reservations that are within 2 weeks of selected date. 
    var json = $.getJSON("/api/ReservationBuffer", date, function (reservation) {
        if (reservation > 0) {
            reservationBufferCount = reservation;

        }
        else {
            reservationBufferCount = reservation;
        }

    });
    json.done(function () {
        //updates the user if they cannot book that date. 
        if (reservationBufferCount > 0) {
            swal('Error', 'There must be 2 weeks between reservations.', 'error')
            return true;
        }
        else {
            return false;
        }
    })


}



//Checks dates to make sure they are valid. 
function checkDates() {

    //Gets start date and end date. 
    var startDate = new Date(document.getElementById('startDate').value);
    var endDate = new Date(document.getElementById('endDate').value);

    //Checks the start date is before the end date. 
    if (document.getElementById('startDate').value != "" && document.getElementById('endDate').value != "") {
        if (document.getElementById('startDate').value > document.getElementById('endDate').value) {
            swal('Error', 'Please select an end date that is past the start date for this reservation', 'error')

            return false;
        }
        //Calculates the total days between the start and end date. 
        var dayDiff = Math.round(Math.abs((endDate - startDate) / (1000 * 60 * 60 * 24)));

        //Checks if days are between april and october and throws a warning that only 14 days are allowed. 
        if ((startDate.getMonth() >= 3 && startDate.getMonth() <= 9) || (endDate.getMonth() >= 3 && endDate.getMonth() <= 9)) {
            if (dayDiff > 14) {
                swal('warning', 'Maximum length of stay April-October is 14 consecutive days except for those traveling on PCS orders. Long term stays are allowed October 15th-April 1st. For detailed information on this policy, please contact the FamCamp Office.', 'warning')
                return false;
            }

        }
        //Sets the total cost. 
        var totalCost = (dayDiff * 25);
        document.getElementById('totalCost').value = totalCost;

    }

}

//Validates the user input. 
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
        //checks if > 14 days between reservations
        checkBuffer();
    }


    if (reservationBufferCount > 0) {
        return false;
    }

    //Checks valid number of adults. 
    if ($('#numAdults').val() <= 0) {
        swal('warning', 'Please enter a valid number of adults. ', 'warning')
        return false;
    }

    //Checks that user agreed to the famcamp pet policy if they are bringing pets.  
    if (document.getElementById('numPets').value > 0 && document.getElementById('breedPolicyAgreement').checked == false) {
        swal('warning', 'Please acknolwdge FamCamp\'s pet policy. ', 'warning')
        return false;
    }
    return true;
}


//Loads the reservations from the selected dates and vehicle length. 
function loadReservations() {

    //Checks if a start date, end date, and vehicle length selected.  
    if (document.getElementById('endDate').value == '' || document.getElementById('startDate').value == ''
        || (document.getElementById('ddlVehicleLength').value == 24 && document.getElementById('ddlVehicleType').value != 7)) {
        swal('Error', 'Please select a start date, end date, and vehicle type to see available resevations', 'error')
        return false;
    }


    //Creates a date json object. 
    var dates = { date1: document.getElementById('startDate').value, date2: document.getElementById('endDate').value, vehicleLength: document.getElementById('ddlVehicleLength').value };

    //Calls the booking reservation controller to load the available sites. 
    $.getJSON("/api/BookingReservations", dates, function (sites) {
    }).done(function (sites) {

        //Displays the site drop down list and populates it with the site data. 
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
            if (p['siteID'] == 47) {
                $('#ddlSites').append($('<option></option>').val(p['siteID']).html("Site: " + p['siteNumber'] + " Type: " + p['siteDescription'] + " Price: 17.00$/Night"));
            }
            else {
                $('#ddlSites').append($('<option></option>').val(p['siteID']).html("Site: " + p['siteNumber'] + " Type: " + p['siteDescription'] + " Price: 25.00$/Night"));

            }
        });
    });


}

//Displays the continue to payment button. 
function continuePayment() {
    $('#btnPayment').css('display', 'block');
}

//Checks the pet policy check box
function checkAgreement() {
    $('#breedPolicyAgreement').prop("checked", true);
}

//Hides 'Update Reservation' Button when a new site and dates are picked (forces the user to proceed to pay for added days).
function hideUpdateBtn() {
    $('#submitUpdate').css('display', 'none');
}

//removes end date value from the new end date field (to prevent error box from showing up while selecting new dates)
function clearEndDate() {
    document.getElementById("endDate").value = "";
}

//Hides the site drop down list if any information is changed. 
function hideLots() {
    $('#btnPayment').css('display', 'none');
    $('#ddlSitesDiv').css('display', 'none');
    $('#btnSeeReservations').css('display', 'block');
}

//Hides the vehicle length if tent is selected and displays it, if it is changed from tent. 
function VehicleSelected() {

    if ($('#ddlVehicleType').val() == 7) {
        $('#ddlVehicleLength').css('display', 'none');
        $('#ddlVehicleLength').val(24).change();
    }
    else {
        $('#ddlVehicleLength').css('display', 'block');
    }
}

//Shows the pet policy and agreement if the user plans to bring pets checkbox is checked. 
function pets() {

    $('.petWrapper').css('display', 'block');
    $('#petsCheckbox').css('display', 'none');
}

//checks for changes if a new user is selected or if a generic user is selected from the admin booking page. 
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
        $('.custNameFields').css('display', 'block');
        $('.custNameFields').css('visibility', 'visible');
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


//this needs to be removed, is a temporary solution.
function vehicleTypeCheck(option) {
    if (option == 1) {
        document.getElementById("ddlVehicleType").value = "Motor Home";
    }
    else if (option == 2) {
        document.getElementById("ddlVehicleType").value = "Travel Trailer";
    }
    else if (option == 3) {
        document.getElementById("ddlVehicleType").value = "5th Wheel";
    }
    else if (option == 4) {
        document.getElementById("ddlVehicleType").value = "Pop Up";
    }
    else if (option == 5) {
        document.getElementById("ddlVehicleType").value = "Van";
    }
    else if (option == 6) {
        document.getElementById("ddlVehicleType").value = "Other";
    }
    else if (option == 7) {
        document.getElementById("ddlVehicleType").value = "Tent";
    }
}