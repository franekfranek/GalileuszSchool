$(document).ready(function () {
    GetCourses();
    //console.log("checking...")
    $('[data-toggle="tooltip"]').tooltip();

    //ADD STUDENT
    $('#addStudentModal').on('show.bs.modal', function (e) {


        var courseId = $(e.relatedTarget).data('course-id');
        var courseName = $(e.relatedTarget).data('course-name');


        document.getElementById("modalTitle").innerHTML = "Add student to " + courseName + " course";

        $(e.currentTarget).find('input[name="courseId"]').val(courseId);
    });

    //------------->DELETE
    $(document).on('click','#deleteModal', function () {
        
        var courseId = $(this).data('course-id');
        var courseName = $(this).data('course-name');
        
        

        document.getElementById("deleteModalTitle").innerHTML = "Delete " + courseName + " course ?";


        $('#deleteCourseModal #deleteId').val(courseId);
        $('#deleteCourseModal #deleteCourseName').val(courseName);
    });

    $('#deleteCourse').on('click', function (e) {

        e.preventDefault();
        var courseId = $('input#deleteId').val();
        var courseName = $('#deleteCourseName').val();
        console.log(courseId);
        
        $.ajax({
            type: 'GET',
            data: {id: courseId},
            url: '/admin/Courses/Delete',
            success: function () {
                $('#deleteCourseModal').modal('hide');

                var notf = $(document).find('#divNotification');
                notf.html("You deleted " + courseName + " course!").show();
                setTimeout(function () {
                    notf.hide("slow");
                }, 2000);
                GetCourses();
            },
            error: function () {
                $('#deleteCourseModal').modal('hide');

                var notf = $(document).find('#divNotification');
                notf.attr("class", "alert alert-danger notification");
                notf.html("An error occurred! Please try again").show();
                setTimeout(function () {
                    notf.hide("slow");
                }, 2000);
                GetCourses();
            }


            
        })
    });

    //------------->EDIT
    $(document).on('click','#editModal', function () {
        var courseId = $(this).data('course-id');
        
        $.ajax({
            type: 'Get',
            data: { id: courseId },
            url: '/admin/Courses/FindCourse',
            success: function (result) {
                $('#editCourseModal #idEdit').val(result.id);
                $('#editCourseModal #editCourseName').val(result.name);
                $('#editCourseModal #level').val(result.level);
                $('#editCourseModal #description').val(result.description);
                $('#editCourseModal #price').val(result.price);
                $('#editCourseModal #teacherId').val(result.teacherId);
            }
        })
    });

    $('#editCourse').on('click', function (e) {

        $("form[name='edit-course']").validate({

            rules: {
                editCourseName: "required",
                level: "required",
                description: "required",
                price: "required",
                teacherId: "required"

            },

            messages: {
                editCourseName: "Please enter course name",
                level: "Please select level",
                description: "Please write description",
                teacherId: "Plase select a teacher"


            }
        });

        var isValidate = $("form[name='edit-course']").valid();

        if (isValidate) {
            e.preventDefault();
            var data = $('#editCourseForm').serialize();
            var name = $('#editCourseName').val();
            var idEdit = $('#idEdit').val();
            console.log(data);

            $.ajax({
                type: 'POST',
                data: data,
                url: '/admin/Courses/Edit',
                success: function () {

                    $('#editCourseModal').modal('hide');

                    var notf = $(document).find('#divNotification');
                    notf.html("You edited " + name + " course!").show();
                    setTimeout(function () {
                        notf.hide("slow");
                    }, 2000);
                    GetCourses();
                },
                error: function () {
                    $('#editCourseModal').modal('hide');

                    var notf = $(document).find('#divNotification');
                    notf.attr("class", "alert alert-danger notification");
                    notf.html("An error occurred! Please try again").show();
                    setTimeout(function () {
                        notf.hide("slow");
                    }, 2000);
                    GetCourses();
                }
            })
        }
    });

    //------------->DETAILS
    $(document).on('click', '#detailsModal', function () {
        //var id = $(this).parent().find('#id').val();
        var courseId = $(this).data('course-id');
        var courseName = $(this).data('course-name');
        
        document.getElementById("detailsModalTitle").innerHTML = courseName + " course details";

        $.ajax({
            type: 'Get',
            data: { id: courseId },
            url: '/admin/Courses/FindCourse',
            success: function (result) {
                $('#detailsCourseModal #id').text(result.id);
                $('#detailsCourseModal #name').text(result.name);
                $('#detailsCourseModal #level').text(result.level);
                $('#detailsCourseModal #description').text(result.description);
                $('#detailsCourseModal #price').text(result.price);
                $('#detailsCourseModal #slug').text(result.slug);
                $('#detailsCourseModal #sorting').text(result.sorting);
                $('#detailsCourseModal #teacherId').text(result.teacherId);
            }
        })
    });

    //------------->ADD COURSE
    $('#addCourse').on('click', function (e) {

        $("form[name='create-new-course']").validate({

            rules: {
                name: "required",
                level: "required",
                description: "required",
                price: "required",
                teacherId: "required"

            },

            messages: {
                courseName: "Please enter course name",
                level: "Please select level",
                description: "Please write description",
                teacherId: "Plase select a teacher"


            }
        });

        var isValidate = $("form[name='create-new-course']").valid();

        if (isValidate) {
            e.preventDefault();
            var data = $('#create-course-form').serialize();
            var name = $('#name').val();
            console.log(data);

            $.ajax({
                type: 'POST',
                data: data,
                url: '/admin/Courses/Create',
                success: function (res) {
                    console.log(res);
                    $('#createCourseModal').modal('hide');

                    $('#createCourseModal').on('hidden.bs.modal', function () {
                        $(this).find('form').trigger('reset');
                    })

                    var notf = $(document).find('#divNotification');
                    if (res == "error") {
                        notf.attr("class", "alert alert-danger notification");
                        notf.html("Course already exists!").show();
                    } else {
                        notf.html("You added " + name + " course!").show();
                    }
                    
                    setTimeout(function () {
                        notf.hide("slow");
                    }, 2000);
                    GetCourses();
                },
                error: function (response) {
                    $('#createCourseModal').modal('hide');

                    $('#createCourseModal').on('hidden.bs.modal', function () {
                        $(this).find('form').trigger('reset');
                    })

                    var notf = $(document).find('#divNotification');
                    notf.attr("class", "alert alert-danger notification");
                    notf.html("An error " + response.status + " occurred! Please try again").show();
                    setTimeout(function () {
                        notf.hide("slow");
                    }, 2000);
                    GetCourses();
                }
            })
        }
    });
});

var GetCourses = function () {

    $.ajax({

        type: "post",
        url: "/Admin/Courses/GetCourses",
        dataType: 'json',
        success: function (data) {

            bindDataTable(data);

        }

    })
};

var table;
var bindDataTable = function (data) {
    if ($.fn.dataTable.isDataTable("#myDataTable")) {
        $("#myDataTable").DataTable()
            .clear()
            .rows.add(data)
            .draw();
    } else {


        table = $("#myDataTable").dataTable({

            data: data,
            columns: [

                { 'data': 'name' },
                { 'data': 'level' },
                { 'data': 'description' },
                { 'data': 'price' },
                { 'data': 'teacher.lastName' },
                {
                    data: null, render: function (data, type, row) {
                        return '<a href="#addStudentModal" class="btn small btn-success" data-toggle="modal" data-course-id="' + data.id + '"data-course-name=' +
                            data.name + '><i class="material-icons" data-toggle="tooltip" title="Add student">&#xE147</i><span >New Student</span></a>';
                    }
                },
                {
                    data: null, render: function (data, type, row) {
                        return '<a href="#detailsCourseModal" id="detailsModal" data-toggle="modal" data-course-id="' + data.id + '"data-course-name='
                            + data.name + '><i class="material-icons" data-toggle="tooltip" title="Details">&#xE8D2;</i></a>';
                    }
                },
                {
                    data: null, render: function (data, type, row) {
                        return '<a href="#editCourseModal" id="editModal" data-toggle="modal" data-course-id="' + data.id + '"data-course-name='
                            + data.name + '><i class="material-icons" data-toggle="tooltip" title="Edit">&#xE254;</i></a>';
                    }
                },
                {
                    data: null, render: function (data, type, row) {
                        return '<a href="#deleteCourseModal" id="deleteModal" data-toggle="modal" data-course-id="' + data.id + '"data-course-name='
                            + data.name + '><i class="material-icons" data-toggle="tooltip" title="Delete">&#xE872;</i></a>';
                    }
                },
            ]
        });
    }
}

