$(document).ready(function () {
    ShowEmployee();
});

// LIST
function ShowEmployee() {

    $.ajax({
        url: '/Employee/EmployeeList',
        type: 'GET',
        success: function (result) {

            var html = '';

            $.each(result, function (i, item) {

                html += '<tr>';
                html += '<td>' + item.id + '</td>';
                html += '<td>' + item.name + '</td>';
                html += '<td>' + item.position + '</td>';
                html += '<td>' + item.salary + '</td>';
                html += '<td>' + item.phone + '</td>';
                html += '<td>';
                html += '<button class="btn btn-warning btn-sm" onclick="EditEmployee(' + item.id + ')">Edit</button> ';
                html += '<button class="btn btn-danger btn-sm" onclick="DeleteEmployee(' + item.id + ')">Delete</button>';
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
    function AddEmployee() {

        var obj = {
            Id: $('#empId').val(),
            Name: $('#empName').val(),
            Position: $('#empPosition').val(),
            Salary: $('#empSalary').val(),
            Phone: $('#empPhone').val()
        };

        $.post('/Employee/AddEmployee', obj, function () {

            $('#empModal').modal('hide');
            ClearEmployee();
            ShowEmployee();
        });
    }

    // EDIT
    function EditEmployee(id) {

        $.get('/Employee/Edit?id=' + id, function (res) {

            $('#empId').val(res.id);
            $('#empName').val(res.name);
            $('#empPosition').val(res.position);
            $('#empSalary').val(res.salary);
            $('#empPhone').val(res.phone);

            $('#btnAdd').hide();
            $('#btnUpdate').show();
            $('#btnConfirmDelete').hide();
            $('#empModal').modal('show');
        });
    }

    // UPDATE
    function UpdateEmployee() {

        var obj = {
            Id: $('#empId').val(),
            Name: $('#empName').val(),
            Position: $('#empPosition').val(),
            Salary: $('#empSalary').val(),
            Phone: $('#empPhone').val()
        };

        $.post('/Employee/Update', obj, function () {

            $('#empModal').modal('hide');
            $('#btnAdd').show();
            $('#btnUpdate').hide();
            ShowEmployee();
        });
    }

    // DELETE
    function DeleteEmployee(id) {

        $.get('/Employee/Edit?id=' + id, function (res) {

            $('#empId').val(res.id);

            $('#empName').val(res.name);
            $('#empPosition').val(res.position);
            $('#empSalary').val(res.salary);
            $('#empPhone').val(res.phone);

            $('#btnAdd').hide();
            $('#btnUpdate').hide();
            $('#btnConfirmDelete').show();

            $('#empModal').modal('show');
        });
    }

    function ConfirmDelete() {

        var id = $('#empId').val();

        if (confirm("Silmek istiyor musunuz?")) {

            $.get('/Employee/Delete?id=' + id, function () {

                $('#empModal').modal('hide');
                ShowEmployee();
                ClearEmployee();
            });
        }
    }

    // CLEAR
    function ClearEmployee() {

        $('#empId').val('');
        $('#empName').val('');
        $('#empPosition').val('');
        $('#empSalary').val('');
        $('#empPhone').val('');

        $('#btnAdd').show();
        $('#btnUpdate').hide();
        $('#btnConfirmDelete').hide();
    }