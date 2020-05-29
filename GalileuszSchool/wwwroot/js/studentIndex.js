function previewFile(element) {
    let preview;
    if (element.id === "studentCreateImg") {
        preview = document.getElementById("targetImg");
    } else {
        preview = document.getElementById("targetImgStudentEdit");
        preview.src = "";
    };
    
    let file = document.getElementById(element.id).files[0];
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
                notf.attr("class", "alert alert-warning notification");
                notf.html("You removed " + studentName + " " + studentLastName + " !").show();
                setTimeout(function () {
                    notf.hide("slow");
                }, 2000);
                GetStudents();
            },
            error: function (response) {
                $('#deleteStudentModal').modal('hide');

                var notf = $(document).find('#divNotification');
                notf.attr("class", "alert alert-danger notification");
                notf.html(response.responseJSON.text).show();
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
                $('#editStudentModal #idStudentEdit').val(result.id);
                $('#editStudentModal #targetImgStudentEdit').attr('src', '/media/students/'+ result.image);
                $('#editStudentModal #editStudentFirstName').val(result.firstName);
                $('#editStudentModal #editStudentLastName').val(result.lastName);
                $('#editStudentModal #editStudentPhone').val(result.phoneNumber);
            }
        })
    });

    $('#editStudentPost').on('click', function (e) {

        $.validator.addMethod('customphone', function (value, element) {
            return this.optional(element) || /^(\([0-9]{3}\)|[0-9]{3}-)[0-9]{3}-[0-9]{3}$/.test(value);
        }, "Pattern is 000-000-000");

        $("form[name='edit-student']").validate({

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

        var isValidate = $("form[name='edit-student']").valid();

        if (isValidate) {
            e.preventDefault();
            var studentId = $('#idStudentEdit').val();
            var studnetFirstName = $('#editStudentFirstName').val();
            var studentLastName = $('#editStudentLastName').val();
            var studentPhone = $('#editStudentPhone').val();
            var studentPic = $('#studentImageEdit').prop('files')[0];

            var data = new FormData();
            data.append('Id', studentId);
            data.append('FirstName', studnetFirstName);
            data.append('LastName', studentLastName);
            data.append('PhoneNumber', studentPhone);
            data.append('ImageUpload', studentPic);
            for (var key of data.entries()) {
                console.log(key[0] + ', ' + key[1]);
            }
            $.ajax({
                type: 'POST',
                data: data,
                url: '/admin/Students/Edit',
                cache: false,
                contentType: false,
                processData: false,
                headers: {
                    RequestVerificationToken:
                        $('input:hidden[name="__RequestVerificationToken"]').val()
                },
                success: function () {

                    $('#editStudentModal').modal('hide');

                    var notf = $(document).find('#divNotification');
                    notf.attr("class", "alert alert-success notification");
                    notf.html("You edited " + studnetFirstName + " " + studentLastName + " !").show();
                    setTimeout(function () {
                        notf.hide("slow");
                    }, 2000);
                    GetStudents();
                },
                error: function (response) {
                    $('#editStudentModal').modal('hide');

                    var notf = $(document).find('#divNotification');
                    notf.attr("class", "alert alert-danger notification");
                    notf.html(response.responseJSON.text).show();
                    setTimeout(function () {
                        notf.hide("slow");
                    }, 2000);
                    GetStudents();
                }
            })
        }
    });

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
                    notf.attr("class", "alert alert-success notification");
                    notf.html("You added " + studnetFirstName + " " + studentLastName).show();

                    setTimeout(function () {
                        notf.hide("slow");
                    }, 2000);
                    GetStudents();
                },
                error: function (response) {
                    $('#createStudentModal').modal('hide');

                    $('#createStudentModal').on('hidden.bs.modal', function () {
                        $(this).find('form').trigger('reset');
                    })

                    var notf = $(document).find('#divNotification');
                    notf.attr("class", "alert alert-danger notification");
                    notf.html(response.responseJSON.text).show();

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

