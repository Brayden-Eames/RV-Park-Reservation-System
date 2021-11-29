var dataTable;

$(document).ready(function () {
	loadList();
});

function loadList() {
	var custID;
	dataTable = $('#DT_load').DataTable({
		"ajax": {
			"url": "/api/adminReservationUpdate",
			"type": "GET",
			"datatype": "json"
		},
		"columns": [
			{ data: "reservationID", width: "10%" },
			{ data: "fullName", width: "15%" },
			{ data: "startDate", width: "10%", "render": function (data) { return moment(data).format("MM/DD/YYYY"); } },
			{ data: "endDate", width: "10%", "render": function (data) { return moment(data).format("MM/DD/YYYY"); } },
			{ data: "siteNumber", width: "30%" },
			{
				data: "reservationID", width: "10%",
				"render": function (data) {
					return `<div class="text-center">
                            <a href="/Admin/Reservations/Upsert?id=${data}"
                            class ="btn btn-success text-white style="cursor:pointer; width=100px;"> <i class="far fa-edit"></i>Edit</a>`;
				},


			}
		],
		"language": {
			"emptyTable": "no data found."
		},

		"width": "100%"
	});

}
function Delete(url) {

	swal({
		title: "Are you sure you want to cancel?",
		text: "Cancellations made at least 4 days prior will be charged a $10.00 fee. " +
			"Cancellations made less than 4 days prior will be charged a one- day fee." +
			"Cancellation for holidays or special events will be charged a $25.00 fee.",
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
						dataTable.ajax.reload();
					}
					else {
						toastr.error(data.message);
					}
				}
			})
		}
	})
}