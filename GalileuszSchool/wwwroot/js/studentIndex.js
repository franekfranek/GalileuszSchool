//$(document).ready(function () {
//    $('#studentImage').change(function () {
//        var File = this.files;

//        if(File && File[0])
//        ReadImage(File[0])
//    })
//})

//var ReadImage = function (file) {
//    var reader = FileReader();
//    var image = new Image;

//    reader.readAsDataUrl(file);
//    reader.onload = function (_file) {
//        image.src = _file.target.result;
//        image.onload = function () {

//            var height = this.height;
//            var width = this.width;
//            var type = this.type;
//            var size = ~~(file.size / 1024) + "KB";

//            $('#targetImg').attr('src', _file.target.result);
//        }
//    }
//}

function previewFile(element) {
    console.log(element);
    
    let preview = document.getElementById(element);
    let file = document.querySelector("input[type=file").files[0];
    const reader = new FileReader();

    reader.addEventListener("load", function () {
        preview.src = reader.result;
    }, false);

    if (file) {
        reader.readAsDataURL(file);
    }
}

$(document).ready(function () {
    GetStudents();
    //console.log("checking...")
    $('[data-toggle="tooltip"]').tooltip();

    //------------->DELETE
    $(document).on('click', '#deleteStudentLink', function () {

        var studentId = $(this).data('student-id');
        var studentFirstName = $(this).data('student-name');
        var studentLastName = $(this).data('student-lastname');


        document.getElementById("deleteModalTitleStudent").innerHTML = "Delete " + studentFirstName + " " + studentLastName + " ?";


        $('#deleteStudentModal #deleteStudentId').val(studentId);
        $('#deleteStudentModal #deleteStudentName').val(studentFirstName);
        $('#deleteStudentModal #deleteStudentLastName').val(studentLastName);
    });

    $('#deleteStudent').on('click', function (e) {

        e.preventDefault();
        var studentId = $('input#deleteStudentId').val();
        var studentName = $('#deleteStudentName').val();
        var studentLastName = $('#deleteStudentLastName').val();


        $.ajax({
            type: 'GET',
            data: { id: studentId },
            url: '/admin/Students/Delete',
            success: function () {
                $('#deleteStudentModal').modal('hide');

                var notf = $(document).find('#divNotification');
                notf.html("You removed " + studentName + " " + studentLastName + " !").show();
                setTimeout(function () {
                    notf.hide("slow");
                }, 2000);
                GetStudents();
            }

        })
    });

    //------------->EDIT
    $(document).on('click', '#editStudentLink', function () {
        var studentId = $(this).data('student-id');

        $.ajax({
            type: 'Get',
            data: { id: studentId },
            url: '/admin/Students/FindStudent',
            success: function (result) {
                console.log(result )
                $('#editStudentModal #idStudentEdit').val(result.id);
                $('#editStudentModal #targetImgStudentEdit').attr('src', '/media/students/'+ result.image);
                $('#editStudentModal #editStudentFirstName').val(result.firstName);
                $('#editStudentModal #editStudentLastName').val(result.lastName);
                $('#editStudentModal #editStudentPhone').val(result.phoneNumber);
            }
        })
    });

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

    //------------->CREATE STUDENT
    $('#createNewStudent').on('click', function (e) {

        $.validator.addMethod('customphone', function (value, element) {
            return this.optional(element) || /^(\([0-9]{3}\)|[0-9]{3}-)[0-9]{3}-[0-9]{3}$/.test(value);
        }, "Pattern is 000-000-000"); 

        $("form[name='create-new-student']").validate({

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

            }
        });

        var isValidate = $("form[name='create-new-student']").valid();

        if (isValidate) {
            e.preventDefault();

            var studnetFirstName = $('#studentFirstName').val();
            var studentLastName = $('#studentLastName').val();
            var studentPhone = $('#phoneNumber').val();
            var studentPic = $('#studentCreateImg').prop('files')[0];

            var data = new FormData();
            data.append('FirstName', studnetFirstName);
            data.append('LastName', studentLastName);
            data.append('PhoneNumber', studentPhone);
            data.append('ImageUpload', studentPic);

            for (var key of data.entries()) {
                console.log(key[0] + ', ' + key[1]);
            }
            debugger
            $.ajax({
                type: 'POST',
                data: data,
                url: '/Admin/Students/Create',
                cache: false,
                contentType: false,
                processData: false,
                headers: {
                    RequestVerificationToken:
                        $('input:hidden[name="__RequestVerificationToken"]').val()
                },
                success: function () {

                    $('#createStudentModal').modal('hide');

                    $('#createStudentModal').on('hidden.bs.modal', function () {
                        $(this).find('form').trigger('reset');
                    })
                    var notf = $(document).find('#divNotification');
                    notf.html("You added " + studnetFirstName + " " + studentLastName).show();
                    setTimeout(function () {
                        notf.hide("slow");
                    }, 2000);
                    GetStudents();
                }
                
            })
        }
    });
});

var GetStudents = function () {

    $.ajax({
        type: "post",
        url: "/Admin/Students/GetStudents",
        dataType: 'json',
        success: function (data) {
            //console.log(data);
            bindDataTable(data);
        }
    })
};


var table;
var bindDataTable = function (data) {
    if ($.fn.dataTable.isDataTable("#studentTable")) {
        $("#studentTable").DataTable()
            .clear()
            .rows.add(data)
            .draw();
    } else {

        table = $("#studentTable").dataTable({

            data: data,
            columns: [
                {
                    data: null, render: function (data, type, row) {
                        return '<img src="/media/students/' + data.image + '" class="img-fluid" height="100" width="100"/>';
                    }
                },
                { 'data': 'firstName' },
                { 'data': 'lastName' },
                { 'data': 'phoneNumber' },
                {
                    data: null, render: function (data, type, row) {
                        return '<a href="#editStudentModal" id="editStudentLink" data-toggle="modal" data-student-id="' + data.id + '"data-student-name="'
                            + data.firstName + '"data-student-lastname=' + data.lastName + '><i class="material-icons" data-toggle="tooltip" title="Delete">&#xE254;</i></a>';
                    }
                },
                {
                    data: null, render: function (data, type, row) {
                        return '<a href="#deleteStudentModal" id="deleteStudentLink" data-toggle="modal" data-student-id="' + data.id + '"data-student-name="'
                            + data.firstName + '"data-student-lastname=' + data.lastName + '><i class="material-icons" data-toggle="tooltip" title="Delete">&#xE872;</i></a>';
                    }
                },
            ]
        });
    }
}

