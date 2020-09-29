$(document).ready(function () {
    GetTeachers();
    //console.log("checking...")
    $('[data-toggle="tooltip"]').tooltip();


    //-----------VALIDATIONS
    $.validator.addMethod('customphone', function (value, element) {
        return this.optional(element) || /^(\([0-9]{3}\)|[0-9]{3}-)[0-9]{3}-[0-9]{3}$/.test(value);
    }, "Pattern is 000-000-000"); 

    $.validator.addMethod("laxEmail", function (value, element) {
        // allow any non-whitespace characters as the host part
        return this.optional(element) || /^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@(?:\S{1,63})$/.test(value);
    }, 'Please enter a valid email address.');


    ////------------->DELETE
    $(document).on('click', '#deleteTeacherLink', function () {

        var teacherId = $(this).data('teacher-id');
        var teacherFirstName = $(this).data('teacher-name');
        var teacherLastName = $(this).data('teacher-lastname');
        console.log(teacherLastName);


        document.getElementById("deleteModalTitle").innerHTML = "Remove " + teacherFirstName + " " + teacherLastName + " ?";


        $('#deleteTeacherModal #deleteTeacherId').val(teacherId);
        $('#deleteTeacherModal #deleteTeacherName').val(teacherFirstName);
        $('#deleteTeacherModal #deleteTeacherLastName').val(teacherLastName);
    });

    $('#deleteTeacher').on('click', function (e) {

        e.preventDefault();
        var teacherId = $('input#deleteTeacherId').val();
        var teacherName = $('#deleteTeacherName').val();
        var teacherLastName = $('#deleteTeacherLastName').val();
        

        $.ajax({
            type: 'GET',
            data: { id: teacherId },
            url: '/admin/Teachers/Delete',
            beforeSend: function () {
                $('#loader').removeClass('hidden');
            },
            success: function () {
                $('#deleteTeacherModal').modal('hide');

                var notf = $(document).find('#divNotification');
                notf.attr("class", "alert alert-warning notification");
                notf.html("You removed " + teacherName + " " + teacherLastName + " !").show();
                setTimeout(function () {
                    notf.hide("slow");
                }, 2000);
                GetTeachers();
            },
            error: function (response) {
                $('#deleteTeacherModal').modal('hide');

                var notf = $(document).find('#divNotification');
                notf.attr("class", "alert alert-danger notification");
                notf.html(response.responseJSON.text).show();
                setTimeout(function () {
                    notf.hide("slow");
                }, 2000);
                GetTeachers();
            },
            complete: function () {
                $('#loader').addClass('hidden');
            }

        })
    });

    //------------->EDIT
    $(document).on('click', '#editTeacher', function () {
        var teacherId = $(this).data('teacher-id');

        $.ajax({
            type: 'Get',
            data: { id: teacherId },
            url: '/admin/Teachers/FindTeacher',
            beforeSend: function () {
                $('#loader').removeClass('hidden');
            },
            success: function (result) {
                $('#editTeacherModal #idTeacherEdit').val(result.id);
                $('#editTeacherModal #editTeacherFirstName').val(result.firstName);
                $('#editTeacherModal #editTeacherLastName').val(result.lastName);
                $('#editTeacherModal #editTeacherPhone').val(result.phoneNumber);
                $('#editTeacherModal #editTeacherEmail').val(result.email);
            },
            complete: function () {
                $('#loader').addClass('hidden');
            }
        })
    });

    $('#editTeacherPost').on('click', function (e) {

        $("form[name='edit-teacher']").validate({

            rules: {
                FirstName: "required",
                LastName: "required",
                PhoneNumber: {
                    required: true,
                    customphone: true,
                },
                Email: {
                    required: true,
                    laxEmail: true,
                }
            },

            messages: {
                FirstName: "Please enter first name",
                LastName: "Please enter last name",
            }
        });

        var isValidate = $("form[name='edit-teacher']").valid();

        if (isValidate) {
            e.preventDefault();
            var data = $('#editTeacherForm').serialize();
            var name = $('#editTeacherFirstName').val();
            var lastName = $('#editTeacherLastName').val();
            

            $.ajax({
                type: 'POST',
                data: data,
                url: '/admin/Teachers/Edit',
                beforeSend: function () {
                    $('#loader').removeClass('hidden');
                },
                success: function () {

                    $('#editTeacherModal').modal('hide');
                    $("#editTeacherModal").appendTo("body");

                    var notf = $(document).find('#divNotification');
                    notf.attr("class", "alert alert-success notification");
                    notf.html("You edited " + name + " " + lastName + " !").show();
                    setTimeout(function () {
                        notf.hide("slow");
                    }, 2000);
                    GetTeachers();
                },
                error: function (response) {
                    $('#editTeacherModal').modal('hide');

                    var notf = $(document).find('#divNotification');
                    notf.attr("class", "alert alert-danger notification");
                    notf.html(response.responseJSON.text).show();
                    setTimeout(function () {
                        notf.hide("slow");
                    }, 2000);
                    GetTeachers();
                },
                complete: function () {
                    $('#loader').addClass('hidden');
                }
            })
        }
    });
    
    //------------->CREATE TEACHER
    $('#createTeacher').on('click', function (e) {

        $("form[name='create-new-teacher']").validate({

            rules: {
                FirstName: "required",
                LastName: "required",
                PhoneNumber: {
                    required: true,
                    customphone: true,
                },
                Email: {
                    required: true,
                    laxEmail: true
                }
            },

            messages: {
                FirstName: "Please enter first name",
                LastName: "Please enter last name",
            }
        });

        var isValidate = $("form[name='create-new-teacher']").valid();

        if (isValidate) {
            e.preventDefault();
            var data = $('#create-teacher-form').serialize();
            var firstName = $('#teacherFirstName').val();
            var lastName = $('#teacherLastName').val();

            $.ajax({
                type: 'POST',
                data: data,
                url: '/admin/Teachers/Create',
                beforeSend: function () {
                    $('#loader').removeClass('hidden');
                },
                success: function () {

                    $('#createTeacherModal').modal('hide');

                    $('#createTeacherModal').on('hidden.bs.modal', function () {
                        $(this).find('form').trigger('reset');
                    });
                    var notf = $(document).find('#divNotification');
                    notf.attr("class", "alert alert-success notification");
                    notf.html("You added " + firstName+ " " + lastName + " to the database!").show();
                    setTimeout(function () {
                        notf.hide("slow");
                    }, 2000);
                    GetTeachers();
                },
                error: function (response) {
                    $('#createTeacherModal').modal('hide');

                    $('#createTeacherModal').on('hidden.bs.modal', function () {
                        $(this).find('form').trigger('reset');
                    })

                    var notf = $(document).find('#divNotification');
                    notf.attr("class", "alert alert-danger notification");
                    notf.html(response.responseJSON.text).show();
                    setTimeout(function () {
                        notf.hide("slow");
                    }, 2000);
                    GetTeachers();
                },
                complete: function () {
                    $('#loader').addClass('hidden');
                }
            })
        }
    });
});

var GetTeachers = function () {

    $.ajax({
        type: "post",
        url: "/Admin/Teachers/GetTeachers",
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
    if ($.fn.dataTable.isDataTable("#teachersTable")) {
        $("#teachersTable").DataTable()
            .clear()
            .rows.add(data)
            .draw();
    } else {


        table = $("#teachersTable").dataTable({

            data: data,
            columns: [

                { 'data': 'firstName' },
                { 'data': 'lastName' },
                { 'data': 'phoneNumber' },
                { 'data': 'email' },
                
                {
                    data: null, render: function (data, type, row) {
                        return '<a href="#editTeacherModal" id="editTeacher" data-toggle="modal" data-teacher-id="' + data.id + '"data-teacher-name="'
                            + data.firstName + '"data-teacher-lastname=' + data.lastName + '><i class="material-icons" data-toggle="tooltip" title="Edit">&#xE254;</i></a>';
                    }
                },
                {
                    data: null, render: function (data, type, row) {
                        return '<a href="#deleteTeacherModal" id="deleteTeacherLink" data-toggle="modal" data-teacher-id="' + data.id + '"data-teacher-name="'
                            + data.firstName + '"data-teacher-lastname=' + data.lastName + '><i class="material-icons" data-toggle="tooltip" title="Delete">&#xE872;</i></a>';
                    }
                },
            ]
        });
    }
}

