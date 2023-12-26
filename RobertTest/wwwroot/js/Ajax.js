var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#myTable').DataTable({
        "ajax": { url: '/customers/getallcustomers' }, 
        select: true,
        "columns": [
            {
                data: 'id',
                "render": function (data) {
                    return `<div></div>`
                },
                "width": "5%", className: 'select-checkbox', targets: 0
            },
            { data: 'name', "width": "5%" },
            {
                data: 'image',
                "render": function (data) {
                    return `<div class="image-box" >                             
                                <img src="${data}" alt="" class="form-control image-box"/>
                            </div>`
                },
                "width": "10%",
                orderable: false
            },
            { data: 'address', "width": "5%" },
            { data: 'city', "width": "5%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div >
                                
                                <a href="/customers/create?id=${data}" class="btn btn-primary mx-2">
                                    <i class="bi bi-pencil-square"></i> Edit
                                </a>
                            </div>`
                },
                "width": "5%",
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
                "width": "5%",
                orderable: false
            },
            {
                data: 'id',
                "render": function (data) {
                    return `<div >
                                <a href="/customers/payment?id=${data}" class="btn btn-success mx-2">
                                     Deposit
                                </a>
                            </div>`
                },
                "width": "5%",
                orderable: false
            }
        ]
    });
}
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
              //  url: `https://localhost:7196/customers/deletecustomer?id=${id}`, 
                url: `https://roberttestapp.azurewebsites.net/customers/deletecustomer?id=${id}`,
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

function Pay(data) {
    alert(data);
    $.ajax({
        url: `https://localhost:7196/customers/payment/${data}`,
       // url: `https://roberttestapp.azurewebsites.net/customers/payment/${data}`,
        type: 'Post',
        success: function (data) {
            console.log(data);
            toastr.success(`Customer id : ${data} Payment`);
            dataTable.ajax.reload();
        }
    });
}