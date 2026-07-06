$(document).ready(function () {
    ShowStore();
});

// LIST
function ShowStore() {

    $.ajax({
        url: '/Store/StoreList',
        type: 'GET',
        success: function (result) {

            var html = '';

            $.each(result, function (i, item) {

                html += '<tr>';
                html += '<td>' + item.id + '</td>';
                html += '<td>' + item.storeName + '</td>';
                html += '<td>' + item.city + '</td>';
                html += '<td>' + item.phone + '</td>';
                html += '<td>' + item.employeeCapacity + '</td>';
                html += '<td>';
                html += '<button class="btn btn-warning btn-sm" onclick="EditStore(' + item.id + ')">Edit</button> ';
                html += '<button class="btn btn-danger btn-sm" onclick="DeleteStore(' + item.id + ')">Delete</button>';
                html += '</td>';
                html += '</tr>';
            });

            $('#table_data').html(html);
        }
    });
}
$('#searchBox').on('keyup', function () {

    var value = $(this).val().toLowerCase();

    $("#table_data tr").filter(function () {

        $(this).toggle(
            $(this).text().toLowerCase().indexOf(value) > -1
        );

    });
});
// ADD
function AddStore() {

    var obj = {
        Id: $('#storeId').val(),
        StoreName: $('#storeName').val(),
        City: $('#storeCity').val(),
        Phone: $('#storePhone').val(),
        EmployeeCapacity: $('#storeCapacity').val()
    };

    $.post('/Store/AddStore', obj, function () {

        $('#storeModal').modal('hide');
        ClearStore();
        ShowStore();
    });
}

// EDIT
function EditStore(id) {

    $.get('/Store/Edit?id=' + id, function (res) {

        $('#storeId').val(res.id);
        $('#storeName').val(res.storeName);
        $('#storeCity').val(res.city);
        $('#storePhone').val(res.phone);
        $('#storeCapacity').val(res.employeeCapacity);

        $('#btnAdd').hide();
        $('#btnUpdate').show();
        $('#btnConfirmDelete').hide();
        $('#storeModal').modal('show');
    });
}

// UPDATE
function UpdateStore() {

    var obj = {
        Id: $('#storeId').val(),
        StoreName: $('#storeName').val(),
        City: $('#storeCity').val(),
        Phone: $('#storePhone').val(),
        EmployeeCapacity: $('#storeCapacity').val()
    };

    $.post('/Store/Update', obj, function () {

        $('#storeModal').modal('hide');
        $('#btnAdd').show();
        $('#btnUpdate').hide();
        ShowStore();
    });
}

// DELETE
function DeleteStore(id) {

    $.get('/Store/Edit?id=' + id, function (res) {

        $('#storeId').val(res.id);

        $('#storeName').val(res.storeName);
        $('#storeCity').val(res.city);
        $('#storePhone').val(res.phone);
        $('#storeCapacity').val(res.employeeCapacity);

        $('#btnAdd').hide();
        $('#btnUpdate').hide();
        $('#btnConfirmDelete').show();

        $('#storeModal').modal('show');
    });
}

function ConfirmDelete() {

    var id = $('#storeId').val();

    if (confirm("Silmek istiyor musunuz?")) {

        $.get('/Store/Delete?id=' + id, function () {

            $('#storeModal').modal('hide');
            ShowStore();
            ClearStore();
        });
    }
}

// CLEAR
function ClearStore() {

    $('#storeId').val('');
    $('#storeName').val('');
    $('#storeCity').val('');
    $('#storePhone').val('');
    $('#storeCapacity').val('');

    $('#btnAdd').show();
    $('#btnUpdate').hide();
    $('#btnConfirmDelete').hide();
}