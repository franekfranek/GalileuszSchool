$(document).ready(function () {
    GetUsers();
    //console.log("checking...")
    $('[data-toggle="tooltip"]').tooltip();

    //------------->DETAILS USER
    $(document).on('click', '#detailsUser', function () {
        var userId = $(this).data('user-id');
        var userName = $(this).data('user-name');

        document.getElementById("detailsModalTitle").innerHTML = "User " +  userName + " details";

        $.ajax({
            type: 'Get',
            data: { id: userId },
            url: '/admin/User/FindUser',
            beforeSend: function () {
                $('#loader').removeClass('hidden');
            },
            success: function (result) {
                $('#detailsUserModal #id').text(result.id);
                $('#detailsUserModal #phoneNumberConfirmed').text(result.PhoneNumberConfirmed);
                $('#detailsUserModal #accessFailedCount').text(result.AccessFailedCount);
                $('#detailsUserModal #securityStamp').text(result.SecurityStamp);
            },
            complete: function () {
                $('#loader').addClass('hidden');
            }
        })
    });


    //////------------->DELETE
    $(document).on('click', '#deleteUserLink', function () {

        var userId = $(this).data('user-id');
        var userName = $(this).data('user-name');


        var x = $('#userModal').text("Remove user " + userName + " ?");
        console.log(userName);

        $('#deleteUserModal #deleteUserId').val(userId);
        $('#deleteUserModal #deleteUserName').val(userName);
    });

    $('#deleteUser').on('click', function (e) {

        e.preventDefault();
        var userId = $('input#deleteUserId').val();
        var userName = $('#deleteUserName').val();
        console.log(userId);
        console.log(userName);


        $.ajax({
            type: 'GET',
            data: { id: userId },
            url: '/admin/User/Delete',
            beforeSend: function () {
                $('#loader').removeClass('hidden');
            },
            success: function () {
                $('#deleteUserModal').modal('hide');

                var notf = $(document).find('#divNotification');
                notf.attr("class", "alert alert-warning notification");
                notf.html("You removed " + userName + " !").show();
                setTimeout(function () {
                    notf.hide("slow");
                }, 2000);
                GetUsers();
            },
            error: function (response) {
                $('#deleteUserModal').modal('hide');

                var notf = $(document).find('#divNotification');
                notf.attr("class", "alert alert-danger notification");
                notf.html(response.responseJSON.text).show();
                setTimeout(function () {
                    notf.hide("slow");
                }, 2000);
                GetUsers();
            },
            complete: function () {
                $('#loader').addClass('hidden');
            }

        })
    });

    
});

var GetUsers = function () {

    $.ajax({
        type: "post",
        url: "/Admin/User/GetUsers",
        dataType: 'json',
        beforeSend: function () {
            $('#loader').removeClass('hidden');
        },
        success: function (data) {
            bindDataTable(data);
        },
        complete: function () {
            $('#loader').addClass('hidden');
        }
    })
};

var table;
var bindDataTable = function (data) {
    if ($.fn.dataTable.isDataTable("#usersTable")) {
        $("#usersTable").DataTable()
            .clear()
            .rows.add(data)
            .draw();
    } else {


        table = $("#usersTable").dataTable({

            data: data,
            columns: [

                { 'data': 'userName' },
                { 'data': 'isStudent' },
                { 'data': 'phoneNumber' },
                { 'data': 'email' },
                { 'data': 'emailConfirmed' },

                {
                    data: null, render: function (data, type, row) {
                        return '<a href="#detailsUserModal" id="detailsUser" data-toggle="modal" data-user-id="' + data.id + '"data-user-name='
                            + data.userName + '><i class="material-icons" data-toggle="tooltip" title="Details">&#xE8D2;</i></a>';
                    }
                },
                {
                    data: null, render: function (data, type, row) {
                        return '<a href="#deleteUserModal" id="deleteUserLink" data-toggle="modal" data-user-id="' + data.id + '"data-user-name='
                            + data.userName + '><i class="material-icons" data-toggle="tooltip" title="Delete">&#xE872;</i></a>';
                    }
                },
            ]
        });
    }
}

