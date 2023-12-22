var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#myTable').DataTable({
        "ajax": { url: 'https://localhost:7196/customers/getallcustomers' },
        buttons: [
            { extend: 'edit',  }
        ],
        select: true,
        "columns": [
            { data: 'id', "width": "15%", className: 'select-checkbox',targets:0 },
            { data: 'name', "width": "10%" },
            {
                data: 'image',
                "render": function (data) {
                    return `<div >                             
                                <img src="${data}" alt="" class="form-control image-box"/>
                            </div>`
                },
                "width": "10%",
                orderable: false
            },
            { data: 'address', "width": "25%" },
            { data: 'city', "width": "15%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div >
                                
                                <a href="/customers/create?id=${data}" class="btn btn-primary mx-2">
                                    <i class="bi bi-pencil-square"></i> Edit
                                </a>
                            </div>`
                },
                "width": "10%",
                orderable: false
            },
            {
                data: 'id',
                "render": function (data) {
                    return `<div >
                                <button class="btn btn-danger form-control"
                                onClick="DeleteCustomer('${data}')"
                                > Delete &nbsp; <i class="bi bi-trash-fill"></i> </button>
                            </div>`
                },
                "width": "10%",
                orderable: false
            }
        ]
    });
}
//Activate an inline edit on click of a table cell
//<button class="btn btn-success form-control"
//onClick="EditCustomer('${data}')"
//> Edit &nbsp;&nbsp;<i class="bi bi-pencil-square"></i> </button>
table.on('click', 'tbody td:not(:first-child)', function (e) {
    editor.inline(this);
});
function DeleteCustomer(id) {
    swal({
        title: "Are you sure?",
        text: `Customer with id : ${id} will be deleted!`,
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
    .then((willDelete) => {
        if (willDelete) { 
            $.ajax({
                url: `https://localhost:7196/customers/deletecustomer?id=${id}`,
                type: 'DELETE',
                success: function (data) {
                    toastr.success(`Customer id : ${id} Deleted Successfully`);
                    dataTable.ajax.reload();
                }
            });
        }
        else {
            swal(`Your Customer with id ${id} is safe!`);
        }
    });
}
function EditCustomer(data) {
    alert(data);
}