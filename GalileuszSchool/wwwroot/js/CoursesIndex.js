$(document).ready(function () {
    GetCourses();
    //console.log("checking...")
    $('[data-toggle="tooltip"]').tooltip();


    //passing data to modal
    $('#addStudentModal').on('show.bs.modal', function (e) {


        var courseId = $(e.relatedTarget).data('course-id');
        var courseName = $(e.relatedTarget).data('course-name');


        document.getElementById("modalTitle").innerHTML = "Add student to " + courseName + " course";

        $(e.currentTarget).find('input[name="courseId"]').val(courseId);
    });


    $(document).on('click','#deleteModal', function () {
        console.log('hejjj');
        var courseId = $(this).data('course-id');
        var courseName = $(this).data('course-name');
       
        debugger;

        document.getElementById("deleteModalTitle").innerHTML = "Delete " + courseName + " course ?";


        $('#deleteCourseModal #id').val(courseId);
    });

    $('#deleteCourse').on('click', function (e) {

            e.preventDefault();
            var name = $('#name').val();

            $.ajax({
                type: 'GET',
                data: data,
                url: '/admin/Courses/Create',
                success: function (response) {

                    $('#createCourseModal').modal('hide');

                    $('#createCourseModal').on('hidden.bs.modal', function () {
                        $(this).find('form').trigger('reset');
                    })
                    var notf = $(document).find('#divNotification');
                    notf.html("You added " + name + " course!").show();
                    setTimeout(function () {
                        notf.hide("slow");
                    }, 2000);
                }
            })
        
    });






    $(document).on('click','#editModal', function (e) {
        var courseId = $(this).data('course-id');
        
        $.ajax({
            type: 'Get',
            data: { id: courseId },
            url: '/admin/Courses/FindCourse',
            success: function (result) {
                $('#editCourseModal #id').val(result.id);
                $('#editCourseModal #courseName').val(result.name);
                $('#editCourseModal #level').val(result.level);
                $('#editCourseModal #description').val(result.description);
                $('#editCourseModal #price').val(result.price);
                $('#editCourseModal #teacherId').val(result.teacherId);
            }
        })
    });

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
        debugger;


        if (isValidate) {
            e.preventDefault();
            var data = $('#create-course-form').serialize();
            var name = $('#name').val();

            $.ajax({
                type: 'POST',
                data: data,
                url: '/admin/Courses/Create',
                success: function (response) {

                    $('#createCourseModal').modal('hide');

                    $('#createCourseModal').on('hidden.bs.modal', function () {
                        $(this).find('form').trigger('reset');
                    })
                    var notf = $(document).find('#divNotification');
                    notf.html("You added " + name + " course!").show();
                    setTimeout(function () {
                        notf.hide("slow");
                    }, 2000);




                    //mainContainer
                    //+ response.name +
                    //var text = "You successufully added course!" + response.name;
                    //$("#mainContainer").load("/Courses/showTempData", { message: text });

                    //$.ajax({
                    //    type: 'GET',
                    //    url: '/admin/Courses/Index',
                    //    success: function (data) {
                    //        $('#table').html(data);
                    //    }
                    //})



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

var bindDataTable = function (data) {
    console.log(data)
    $("#myDataTable").dataTable({

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

