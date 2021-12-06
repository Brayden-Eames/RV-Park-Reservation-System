var dataTable;
var width = window.innerWidth;
console.log(width)

$(document).ready(function () {
	loadList();
});

function loadList() {
	if (width >= 990) {
		dataTable = $('#DT_load').DataTable({
			"ajax": {
				"url": "/api/clientReservation",
				"type": "GET",
				"datatype": "json"
			},
			"columns": [
				{ data: "resStartDate", width: "20%", "render": function (data) { return moment(data).format("MM/DD/YYYY, hh:mm a"); }  },
				{ data: "resEndDate", width: "20%", "render": function (data) { return moment(data).format("MM/DD/YYYY, hh:mm a"); }  },
				{ data: "resVehicleLength", width: "15%" },
				{ data: "resNumAdults", width: "5%" },
				{ data: "resNumChildren", width: "5%" },
				{ data: "resNumPets", width: "5%" },
				{
					data: "resID", width: "30%",
					"render": function (data) {
						return `<div class="text-center">
                            <a href="/Client/Upsert?id=${data}"
                            class ="btn btn-success text-white" style="cursor:pointer; width=100px;"> <i class="far fa-edit"></i>Edit</a>
                            <a onClick=Delete('/api/clientReservation/'+${data})
                            class ="btn btn-danger text-white style="cursor:pointer; width=100px;"> <i class="far fa-trash-alt"></i>Cancel</a>
							<a href="/Client/UpsertPayment?id=${data}"
							class ="btn btn-info text-white" style="cursor:pointer; width=100px;"> <i class="far fa-credit-card"></i>Pay</a>
                            </div>`;
					},


				}
			],
			"language": {
				"emptyTable": "no data found."
			},

			"width": "100%"
		});
	} else {
		dataTable = $('#DT_load_mobile').DataTable({
			"ajax": {
				"url": "/api/clientReservation",
				"type": "GET",
				"datatype": "json"
			},
			"columns": [
				{ data: "resStartDate", width: "20%", "render": function (data) { return moment(data).format("MM/DD/YYYY, hh:mm a"); }  },
				{ data: "resEndDate", width: "20%", "render": function (data) { return moment(data).format("MM/DD/YYYY, hh:mm a"); }  },
				{
					data: "resID", width: "60%",
					"render": function (data) {
						return `<div class="text-center">
                            <a href="/Client/Upsert?id=${data}"
                            class ="btn btn-success text-white" style="cursor:pointer; width=100px;" data-toggle="tooltip" data-placement="top" title="Edit Reservation"> <i class="far fa-edit"></i></a>
                            <a onClick=Delete('/api/clientReservation/'+${data})
                            class ="btn btn-danger text-white style="cursor:pointer; width=100px;" data-toggle="tooltip" data-placement="top" title="Delete Reservation"> <i class="far fa-trash-alt"></i></a>
							<a href="/Client/UpsertPayment?id=${data}"
							class ="btn btn-info text-white" style="cursor:pointer; width=100px;" data-toggle="tooltip" data-placement="top" title="Make a Payment"> <i class="far fa-credit-card"></i></a>
                            </div>`;
					},
				}
			],
			"language": {
				"emptyTable": "no data found."
			},

			"width": "100%"
		});
    }
}
function Delete(url) {

	swal({
		title: "Are you sure you want to cancel?",
		text: "Cancellations made at least 4 days prior will be charged a $10.00 fee. "+
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