$(document).ready(function () {
    GetTeachers();
    //console.log("checking...")
    $('[data-toggle="tooltip"]').tooltip();

    ////------------->DELETE
    $(document).on('click', '#deleteTeacher', function () {

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
            success: function () {
                $('#deleteTeacherModal').modal('hide');

                var notf = $(document).find('#divNotification');
                notf.html("You removed" + teacherName + " " + teacherLastName + " !").show();
                setTimeout(function () {
                    notf.hide("slow");
                }, 2000);
                GetTeachers();
            }

        })
    });

    //------------->EDIT
    //$(document).on('click', '#editModal', function () {
    //    var courseId = $(this).data('course-id');

    //    $.ajax({
    //        type: 'Get',
    //        data: { id: courseId },
    //        url: '/admin/Courses/FindCourse',
    //        success: function (result) {
    //            $('#editCourseModal #idEdit').val(result.id);
    //            $('#editCourseModal #editCourseName').val(result.name);
    //            $('#editCourseModal #level').val(result.level);
    //            $('#editCourseModal #description').val(result.description);
    //            $('#editCourseModal #price').val(result.price);
    //            $('#editCourseModal #teacherId').val(result.teacherId);
    //        }
    //    })
    //});

    //$('#editCourse').on('click', function (e) {

    //    $("form[name='edit-course']").validate({

    //        rules: {
    //            editCourseName: "required",
    //            level: "required",
    //            description: "required",
    //            price: "required",
    //            teacherId: "required"

    //        },

    //        messages: {
    //            editCourseName: "Please enter course name",
    //            level: "Please select level",
    //            description: "Please write description",
    //            teacherId: "Plase select a teacher"


    //        }
    //    });

    //    var isValidate = $("form[name='edit-course']").valid();

    //    if (isValidate) {
    //        e.preventDefault();
    //        var data = $('#editCourseForm').serialize();
    //        var name = $('#editCourseName').val();
    //        var idEdit = $('#idEdit').val();
    //        console.log(data);

    //        $.ajax({
    //            type: 'POST',
    //            data: data,
    //            url: '/admin/Courses/Edit',
    //            success: function () {

    //                $('#editCourseModal').modal('hide');

    //                var notf = $(document).find('#divNotification');
    //                notf.html("You edited " + name + " course!").show();
    //                setTimeout(function () {
    //                    notf.hide("slow");
    //                }, 2000);
    //                GetCourses();
    //            }
    //        })
    //    }
    //});

    //------------->CREATE TEACHER
    $('#createTeacher').on('click', function (e) {

        $.validator.addMethod('customphone', function (value, element) {
            return this.optional(element) || /^(\([0-9]{3}\)|[0-9]{3}-)[0-9]{3}-[0-9]{3}$/.test(value);
        }, "Pattern is 000-000-000"); 

        $("form[name='create-new-teacher']").validate({

            rules: {
                FirstName: "required",
                LastName: "required",
                PhoneNumber: {
                    required: true,
                    customphone: true,
                    }
            },

            messages: {
                FirstName: "Please enter first name",
                LastName: "Please enter last name",
                //PhoneNumber: "Pattern is 000-000-000"
            }
        });

        var isValidate = $("form[name='create-new-teacher']").valid();

        if (isValidate) {
            e.preventDefault();
            var data = $('#create-teacher-form').serialize();
            var firstName = $('#teacherFirstName').val();
            var lastName = $('#teacherLastName').val();
            console.log(data);

            $.ajax({
                type: 'POST',
                data: data,
                url: '/admin/Teachers/Create',
                success: function () {

                    $('#createTeacherModal').modal('hide');

                    $('#createTeacherModal').on('hidden.bs.modal', function () {
                        $(this).find('form').trigger('reset');
                    })
                    var notf = $(document).find('#divNotification');
                    notf.html("You added " + firstName+ " " + lastName + " to the database!").show();
                    setTimeout(function () {
                        notf.hide("slow");
                    }, 2000);
                    GetTeachers();
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
        success: function (data) {
            console.log(data);
            bindDataTable(data);

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
        debugger
    } else {


        table = $("#teachersTable").dataTable({

            data: data,
            columns: [

                { 'data': 'firstName' },
                { 'data': 'lastName' },
                { 'data': 'phoneNumber' },
                
                {
                    data: null, render: function (data, type, row) {
                        return '<a href="#editCourseModal" id="editModal" data-toggle="modal" data-course-id="' + data.id + '"data-course-name='
                            + data.name + '><i class="material-icons" data-toggle="tooltip" title="Edit">&#xE254;</i></a>';
                    }
                },
                {
                    data: null, render: function (data, type, row) {
                        return '<a href="#deleteTeacherModal" id="deleteTeacher" data-toggle="modal" data-teacher-id="' + data.id + '"data-teacher-name="'
                            + data.firstName + '"data-teacher-lastname=' + data.lastName + '><i class="material-icons" data-toggle="tooltip" title="Delete">&#xE872;</i></a>';
                    }
                },
            ]
        });
    }
}

