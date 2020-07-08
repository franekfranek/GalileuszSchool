//modal binding
ko.bindingHandlers.modal = {
    init: function (element, valueAccessor) {
        $(element).modal({
            show: false
        });

        var value = valueAccessor();
        if (ko.isObservable(value)) {
            // Update 28/02/2018
            // Thank @HeyJude for fixing a bug on
            // double "hide.bs.modal" event firing.
            // Use "hidden.bs.modal" event to avoid
            // bootstrap running internal modal.hide() twice.
            $(element).on('hidden.bs.modal', function () {
                value(false);
            });
        }

    },
    update: function (element, valueAccessor) {
        var value = valueAccessor();
        if (ko.utils.unwrapObservable(value)) {
            $(element).modal('show');
        } else {
            $(element).modal('hide');
        }
    }
}



//class models
//teacher's homework model
function Homework(data) {
    this.Id = ko.observable(data.id);
    this.Title = ko.observable(data.title);
    this.Slug = ko.observable(data.slug);
    this.CreationDate = ko.observable(data.creationDate.substring(0, 10));
    this.TextContent = ko.observable(data.textContent);
    this.TeacherName = ko.observable(data.teacher.firstName + " " + data.teacher.lastName);
    this.ImageContent = ko.observable(data.imageContent);
    
}

//student's homework model
function StudentHomework(data) {
    this.HomeworkId = ko.observable(data.homeworkId);
    this.StudentId = ko.observable(data.studentId );
    this.IsDone = ko.observable(data.isDone);
    this.SolutionTextContent = ko.observable(data.solutionTextContent);
    this.StudentSubmissionDate = ko.observable(data.studentSubmissionDate);
    this.ImageSolution = ko.observable(data.imageSolution);
}

//student model
function Student(data, id) {
    this.Id = ko.observable(data.id);
    this.FullName = ko.observable(data.firstName + ' ' + data.lastName);
    this.Email = ko.observable(data.email);
    this.CurrentHomeworkIsDone = ko.observable("");

    for (var i = 0; i < data.studentHomeworks?.length; i++) {
        if (data.studentHomeworks[i].homeworkId === id) {
            if (data.studentHomeworks[i].isDone === true) {
                this.CurrentHomeworkIsDone("Submitted");
            } else this.CurrentHomeworkIsDone("Not submitted");

            
        }
    }
}


function ViewModel() {

    // DATA
    var self = this;//this has to be here so this contextes dont mix
    //modal
    self.showDialog = ko.observable(false);
    self.Title = ko.observable("");
    self.TextContent = ko.observable("");
    self.HomeworkPicture = ko.observable("");

    self.currentFileSrc = ko.observable("");

    //list of homeworks
    self.homeworks = ko.observableArray([]);
    //folders
    self.folders = ko.observable();
    self.chosenFolderId = ko.observable();

    //who is logged
    self.isStudentStatus = ko.observable();
    self.isTeacherStatus = ko.observable();

    //detailed homeworks
    self.chosenHomeworkData = ko.observable("");
    self.chosenHomeworkImageSrc = ko.observable("");
    self.isFileAttached = ko.observable("");
    self.chosenStudentHomework = ko.observable("");
    self.isCurrenHomeworkDone = ko.observable(false);
      
    //toggle view
    self.toggleDetailedView = ko.observable(true);

    //list of students 
    self.studentsPerHomework = ko.observableArray([]);
    self.studentNotAssignedYet = ko.observableArray([]);
    self.selectedStudents = ko.observableArray([]);

    //student solution submmision
    self.studentSolutionText = ko.observable("");
    self.studentSolutionPicture = ko.observable("");
  
    // OPERATIONS


    //ONLY TEACHER METHODS
    //save homework
    this.submit = function () {
        var data = new FormData();

        data.append('Title', self.Title());
        data.append('TextContent', self.TextContent());
        data.append('PhotoContent', self.HomeworkPicture());
        
        $.ajax({
            url: "/homework/create",
            data: data,
            type: "post",
            cache: false,
            contentType: false,
            processData: false,
            headers: {
                RequestVerificationToken:
                    $('input:hidden[name="__RequestVerificationToken"]').val()
            },
            success: function (res) {
                console.log(res);
                $('#createHomeworkModal').on('hidden.bs.modal', function () {
                    $(this).find('form').trigger('reset');
                    $(this).find('textarea').val('');
                    self.HomeworkPicture("");
                    self.currentFileSrc("");
                    self.goToFolder('All');
                })
            }, error: function (res) {
                console.log(res);
            }
        });
        self.showDialog(false);
    }
    self.loadStudentsPerHomework = function (homework) {
        $.ajax({
            url: '/studentHomework/studentsByHomework',
            data: { id: homework.Id },
            type: 'get',
            success: function (res) {
                console.log(res);
                var mappedStudents = $.map(res, function (item) {
                    return new Student(item, self.chosenHomeworkData().Id());
                });
                self.studentsPerHomework(mappedStudents);
                console.log(mappedStudents);
                self.loadStudentsWithoutHomework(self.studentsPerHomework());
            }
        });
    }

    self.loadStudentsWithoutHomework = function (students) {
        var ids = [];
        for (var i = 0; i < students.length; i++) {
            ids[i] = students[i].Id;
        }

        $.ajax({
            type: 'get',
            url: '/studentHomework/getRestOfStudent',
            dataType: 'json',
            traditional: true,
            data: { alreadyAssignedStudents: ids },
            success: function (res) {
                var mappedStudents = $.map(res, function (item) {
                    return new Student(item);
                });
                self.studentNotAssignedYet(mappedStudents);
            }
        });
    }
    //save students assigned to homework to db
    self.saveStudents = function () {
        var ids = [];
        for (var i = 0; i < self.selectedStudents().length; i++) {
            ids.push(self.selectedStudents()[i].Id);
        }
        $.ajax({
            type: 'post',
            url: '/studentHomework/AddStudentHomeworks',
            dataType: 'json',
            traditional: true,
            data: {
                homeworkId: self.chosenHomeworkData().Id,
                studentsIds: ids
            },
            success: function (res) {
                self.selectedStudents([]);
                self.goToHomework(self.chosenHomeworkData());

            }
        });
    }

    // check all the checkboxes for student to homework assigments
    //https://knockoutjs.com/documentation/computed-writable.html newer
    self.SelectAll = ko.dependentObservable({
        read: function () {
            return self.selectedStudents().length === self.studentNotAssignedYet().length;
        },
        write: function (newValue) {
            self.selectedStudents(self.selectedStudents().length === self.studentNotAssignedYet().length ? [] : self.studentNotAssignedYet().slice(0));
        }
    });
    
    //ONLY STUDENT METHODS
    self.savaStudentSolution = function () {
        var data = new FormData();

        data.append('HomeworkId', self.chosenHomeworkData().Id());
        data.append('SolutionTextContent', self.studentSolutionText());
        data.append('PhotoSolution', self.studentSolutionPicture());


        for (var key of data.entries()) {
            console.log(key[0] + ', ' + key[1]);
        }

        $.ajax({
            url: "/studenthomework/AddStudentSolution",
            data: data,
            type: "post",
            cache: false,
            contentType: false,
            processData: false,
            headers: {
                RequestVerificationToken:
                    $('input:hidden[name="__RequestVerificationToken"]').val()
            },
            success: function (res) {
                self.goToFolder('All');
                self.studentSolutionText = ko.observable("");
                self.studentSolutionPicture = ko.observable("");
                console.log(res);

            }, error: function (res) {
                console.log(res);
            }
        });
    }

    self.getStudentHomeworkWithFlag = function (isDone) {
        console.log('hej');
        $.ajax({
            url: '/studentHomework/GetAllStudentHomeworkWithFlag',
            data: { isDone: isDone },
            type: 'get',
            success: function (res) {
                if (res.length > 0) {
                    $(".book-homework").attr("title", "You have " + res.length + " homework(s) unsubmited.");
                    $(".book-homework").css("display", "block");
                    $("#txt").text(res.length);
                } else {
                    $("#txt").remove();
                }
                
            }
        });
    }

    //USED BY BOTH T AND S
    //display specific homework's details
    self.goToHomework = function (homework) {
        self.toggleDetailedView(true);
        self.chosenHomeworkData(homework);
        if (self.chosenHomeworkData().ImageContent() !== null) {
            self.isFileAttached(true);
            self.chosenHomeworkImageSrc('/media/homeworks/' + self.chosenHomeworkData().ImageContent());
        }else{
            self.isFileAttached(false);
        };

        if (self.isStudentStatus() === true) {
            self.getStudentHomeworkDetails(homework.Id);
        }
        else {
            self.loadStudentsPerHomework(homework);
        }
        //$.get("/homework/findhomework",
        //    { id: homework.Id },
        //    self.chosenHomeworkData).done(function (res) {
        //        self.loadStudentsPerHomework(res);
        //        self.currentHomeworkId(res.id);
        //        console.log(self.chosenHomeworkData());
        //    });
    };

    //get clicked homework details 
    self.getStudentHomeworkDetails = function (homeworkId) {
        $.ajax({
            url: '/studentHomework/GetCurrentStudentHomework',
            data: { id: homeworkId },
            type: 'get',
            success: function (res) {
                self.chosenStudentHomework(new StudentHomework(res));
                self.isCurrenHomeworkDone(self.chosenStudentHomework().IsDone());
            }
        });
    }

    // BEHAVIOURS
    //determine which folders to load
    self.whichFolders = function (who) {
        console.log(who.isTeacher);
        if (who.isTeacher) self.folders(['All', 'Submitted', 'Unsubmitted']);
        else self.folders(['All', 'Submitted', 'Unsubmitted']);
    }

    //load filtered homeworks
    self.goToFolder = function (folder) {
        self.chosenFolderId(folder);
        $.ajax({
            type: 'get',
            url: '/homework/gethomeworks',
            data: { option: folder },
            success: function (result) {
                //convert it to Homework instances, then populate self.homeworks
                var mappedHomeworks = $.map(result, function (item) {
                    return new Homework(item)
                });
                self.homeworks(mappedHomeworks); 
                self.toggleDetailedView(false);

                if (self.isStudentStatus() === true) {
                    self.getStudentHomeworkWithFlag(false);
                }
            }
        });
    };

    self.fileSelect = function (elemet, event) {
        var file = event.target.files[0];// FileList object which is part of event properties for current element!
        // render image file as thumbnail.
        var reader = new FileReader();

        // Closure to capture the file information.
        reader.onload = (function (theFile) {
            return function (e) {
                self.currentFileSrc(e.target.result);
                if (self.showDialog() === false) {
                    self.studentSolutionPicture(theFile);
                } else {
                    self.HomeworkPicture(theFile);
                }
            };
        })(file);
        // Read in the image file as a data URL.
        reader.readAsDataURL(file);
    }

    //determine who is logged in
    self.isStudentOrTeacher = function () {
        $.get('/homework/isstudentorteacher').done(function (res) {
            self.isStudentStatus(res.isStudent);
            self.isTeacherStatus(res.isTeacher);
            self.whichFolders(res);
        });  
    }



    self.isStudentOrTeacher();
    // Show all homework by default
    self.goToFolder('All');
}

ko.applyBindings(new ViewModel());