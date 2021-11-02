﻿var dataTable;

$(document).ready(function () {
	loadList();
});

function loadList() {
	dataTable = $('#DT_load').DataTable({
		"ajax": {
			"url": "/api/clientReservation",
			"type": "GET",
			"datatype": "json"
		},
		"columns": [
			{ data: "resStartDate", width: "20%" },
			{ data: "resEndDate", width: "20%" },
			{ data: "resVehicleLength", width: "15%" },
			{ data: "resNumAdults", width: "5%" },
			{ data: "resNumChildren", width: "5%" },
			{ data: "resNumPets", width: "5%" },
			{
				data: "resID", width: "30%",
				"render": function (data) {
					return `<div class="text-center">
                            <a href="/Admin/Categories/Upsert?id=${data}"
                            class ="btn btn-success text-white style="cursor:pointer; width=100px;"> <i class="far fa-edit"></i>Edit</a>
                            <a onClick=Delete('/api/clientReservation/'+${data})
                            class ="btn btn-danger text-white style="cursor:pointer; width=100px;"> <i class="far fa-trash-alt"></i>Cancel</a>
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
function Delete(url) {
	console.log(url);
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