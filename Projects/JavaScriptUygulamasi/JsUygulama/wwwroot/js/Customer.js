$(document).ready(function () {
    ShowCustomer();
});

// LIST
function ShowCustomer() {

    $.ajax({
        url: '/Customer/CustomerList',
        type: 'GET',
        success: function (result) {

            var html = '';

            $.each(result, function (i, item) {

                html += '<tr>';
                html += '<td>' + item.id + '</td>';
                html += '<td>' + item.name + '</td>';
                html += '<td>' + item.phone + '</td>';
                html += '<td>' + item.email + '</td>';
                html += '<td>' + item.city + '</td>';
                html += '<td>' + item.address + '</td>';
                html += '<td>' + new Date(item.createdDate).toLocaleDateString('tr-TR') + '</td>';
                html += '<td>';
                html += '<button class="btn btn-warning btn-sm" onclick="EditCustomer(' + item.id + ')">Edit</button> ';
                html += '<button class="btn btn-danger btn-sm" onclick="DeleteCustomer(' + item.id + ')">Delete</button>';
                html += '</td>';
                html += '</tr>';
            });

            $('#table_data').html(html);
        },
        error: function () {
            alert("Data can't get");
        }
    });

};
$('#searchBox').on('keyup', function () {

    var value = $(this).val().toLowerCase();

    $("#table_data tr").filter(function () {

        $(this).toggle(
            $(this).text().toLowerCase().indexOf(value) > -1
        );

    });
});

// ADD
function AddCustomer() {

    var obj = {
        Id: $('#cusId').val(),
        Name: $('#cusName').val(),
        Phone: $('#cusPhone').val(),
        Email: $('#cusEmail').val(),
        City: $('#cusCity').val(),
        Address: $('#cusAddress').val()
    };

    $.post('/Customer/AddCustomer', obj, function () {

        $('#cusModal').modal('hide');
        ClearCustomer();
        ShowCustomer();
    });
}

// EDIT
function EditCustomer(id) {

    $.get('/Customer/Edit?id=' + id, function (res) {

        $('#cusId').val(res.id);
        $('#cusName').val(res.name);
        $('#cusPhone').val(res.phone);
        $('#cusEmail').val(res.email);
        $('#cusCity').val(res.city);
        $('#cusAddress').val(res.address);

        $('#btnAdd').hide();
        $('#btnUpdate').show();
        $('#btnConfirmDelete').hide();
        $('#cusModal').modal('show');
    });
}

// UPDATE
function UpdateCustomer() {

    var obj = {
        Id: $('#cusId').val(),
        Name: $('#cusName').val(),
        Phone: $('#cusPhone').val(),
        Email: $('#cusEmail').val(),
        City: $('#cusCity').val(),
        Address: $('#cusAddress').val()
    };

    $.post('/Customer/Update', obj, function () {

        $('#cusModal').modal('hide');
        $('#btnAdd').show();
        $('#btnUpdate').hide();
        ShowCustomer();
    });
}

// DELETE
function DeleteCustomer(id) {

    $.get('/Customer/Edit?id=' + id, function (res) {

        $('#cusId').val(res.id);

        $('#cusName').val(res.name);
        $('#cusPhone').val(res.phone);

        $('#btnAdd').hide();
        $('#btnUpdate').hide();
        $('#btnConfirmDelete').show();

        $('#cusModal').modal('show');
    });
}
function ConfirmDelete() {

    var id = $('#cusId').val();

    if (confirm("Silmek istiyor musunuz?")) {

        $.get('/Customer/Delete?id=' + id, function () {

            $('#cusModal').modal('hide');
            ShowCustomer();
            ClearCustomer();
        });
    }
}
// CLEAR
function ClearCustomer() {

    $('#cusId').val('');
    $('#cusName').val('');
    $('#cusPhone').val('');
    $('#cusEmail').val('');
    $('#cusCity').val('');
    $('#cusAddress').val('');

    $('#btnAdd').show();
    $('#btnUpdate').hide();
    $('#btnConfirmDelete').hide();
}