

        $('#myModal').on('shown.bs.modal', function () {
            $('#myInput').trigger('focus')
        })

/*        $(document).ready(function () {

            $('[data-toggle="popover"]').popover({
                content: $('#calendarContainer'),
                html: true,
                trigger: "click",
            });

        });




        function CreateCalendar() {
            var calendarEl = document.getElementById('calendar');
            var calendar = new FullCalendar.Calendar(calendarEl, {

                select: function (selectionInfo) {
                    selectionInfo.allDay = true;
                    var startdate = new Date(selectionInfo.start);
                    var startstring = startdate.getDate() + "/" + (startdate.getMonth() + 1) + "/" + startdate.getFullYear() + " " +
                        startdate.getHours() + ":" + startdate.getMinutes();
                    var enddate = new Date(selectionInfo.end);
                    var endstring = enddate.getDate() + "/" + (enddate.getMonth() + 1) + "/" + enddate.getFullYear() + " " +
                        enddate.getHours() + ":" + enddate.getMinutes();
                    console.log(startstring + " " + endstring);
                    document.getElementById('dateStart').value = startstring;
                    document.getElementById('dateEnd').value = endstring;
                    $('.dates').popover('hide');
                },


                events: '/JSON/SpecialEvents.json',
                initialView: 'dayGridMonth',
                selectable: true,
                dayMaxEvents: true,
                height: 400,


            });
            calendar.render();
        };*/

        $(function () {
            $("#datepicker").datepicker({ minDate: 1, maxDate: "+6M" });
            $("#timepicker").timepicker({ 'minTime': '11:00 AM', 'maxTime': '9:00 PM', step: '30' });
        });

        function checkAgreement() {
            $('#breedPolicyAgreement').prop("checked", true);
        }

        function pets() {

             $('.petWrapper').css('display', 'block');
            $('#petsCheckbox').css('display', 'none');
        }