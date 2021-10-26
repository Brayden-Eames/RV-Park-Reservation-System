var dataTable;

$(document).ready(function () {
    loadList();
});

function loadList() {
    dataTable = $('#DT_load').DataTable({
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
            { data: "siteLastModifiedDate", width: "12%"},
            { data: "siteLastModifiedBy", width: "12%" },
            {
                data: "siteID", width: "25%",
                "render": function (data) {
                    return `<div class="text-center">
                            <a href="/Admin/Sites/Upsert?id=${data}"
                            class ="btn btn-success text-white style="cursor:pointer; width=100px;"> <i class="far fa-edit"></i> Edit</a>                            
                            <a onClick=Delete('/api/sites/'+${data})
                            class ="btn btn-danger text-white style="cursor:pointer; width=100px;"> <i class="far fa-trash-alt"></i> Delete</a>
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

function Delete(url) {
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
                    else
                        toastr.error(data.message);
                }
            })
        }
    })
}