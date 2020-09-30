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

//good example of binding in submitted/unsubmitted badge in teacher section
//for further usege XD https://stackoverflow.com/questions/33442589/how-to-convert-the-boolean-value-to-yes-or-no-in-knockout
//    < span data - bind="boolean: myBooleanVar" ></span > < !--(Defaults to "Yes" or "No")-->
//        <span data-bind="boolean: myBooleanVar, trueText: 'Absolutely', falseText: 'Negative'"></span>


//ko.bindingHandlers.boolean = {
//    update: function (element, valueAccessor, allBindings) {
//        var bool = ko.utils.unwrapObservable(valueAccessor()),
//            trueText = allBindings.get('trueText') || 'Yes',
//            falseText = allBindings.get('falseText') || 'No';
//        $(element).text(bool ? trueText : falseText);
//    },
//};


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
    this.Course = ko.observable(data.course);
    
}

//student's homework model
function StudentHomework(data) {
    var self = this;
    self.HomeworkId = ko.observable(data.homeworkId);
    self.StudentId = ko.observable(data.studentId );
    self.IsDone = ko.observable(data.isDone);
    self.SolutionTextContent = ko.observable(data.solutionTextContent);
    self.StudentSubmissionDate = ko.observable(data.studentSubmissionDate);
    self.ImageSolution = ko.observable(data.imageSolution);
    self.ImageSrc = ko.computed(function () {
        return self.ImageSolution() ? '/media/studentHomeworkSolutions/' + self.ImageSolution() : false;
    })
}

//student model
function Student(data, id) {
    var that = this;
    that.Id = ko.observable(data.id);
    that.FullName = ko.observable(data.firstName + ' ' + data.lastName);
    that.Email = ko.observable(data.email);
    that.CurrentHomeworkIsDone = ko.observable("");

    for (var i = 0; i < data.studentHomeworks?.length; i++) {
        if (data.studentHomeworks[i].homeworkId === id) {
            if (data.studentHomeworks[i].isDone === true) {
                that.CurrentHomeworkIsDone(true);
            } else that.CurrentHomeworkIsDone(false);
        }
    }
    that.Status = ko.computed(function () {
        return that.CurrentHomeworkIsDone() ? 'Submitted' : 'Not Submitted';
    });
}

function Course(data) {
    var self = this;
    self.Id = ko.observable(data.id);
    self.Name = ko.observable(data.name);
    self.Level = ko.observable(data.level);
    self.Description = ko.observable(data.description);
}


function ViewModel() {

    // DATA
    var self = this;//this has to be here so this contextes dont mix
    //modal
    self.showDialog = ko.observable(false);
    self.Title = ko.observable("");
    self.TextContent = ko.observable("");
    self.HomeworkPicture = ko.observable("");
    self.modalTextFile = ko.observable();

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
    //togle student homework solution in teacher's section 
    self.toggleStudentHomework = ko.observable(false);

    //list of students 
    self.studentsPerHomework = ko.observableArray([]);
    self.studentNotAssignedYet = ko.observableArray([]);
    self.selectedStudents = ko.observableArray([]);

    //student solution submmision
    self.studentSolutionText = ko.observable("");
    self.studentSolutionPicture = ko.observable("");

    //student homework for teacher view
    self.studentSolutiuonToShow = ko.observable("");
    self.currentStudentName = ko.observable("");
    self.currentStudentMessage = ko.pureComputed({
        read: function () {
            return self.currentStudentName() + "'s solution: ";
        },
        write: function (value) {
            return value;
        },
        owner: self
    })

    //courses
    self.coursesAvailable = ko.observableArray([]);
    self.selectedCourse = ko.observable("");

    // OPERATIONS

    //ONLY TEACHER METHODS
    //save homework
    this.submit = function () {
        var data = new FormData();
        data.append('Title', self.Title());
        data.append('TextContent', self.TextContent());
        data.append('PhotoContent', self.HomeworkPicture());
        data.append('Course', self.selectedCourse().Name());
        //for (var entry of data.entries()){
        //    console.log(entry[0], entry[1]);
        //}

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
            beforeSend: function () {
                $('#loader').removeClass('hidden');
            },
            success: function (res) {
                $('#createHomeworkModal').on('hidden.bs.modal', function () {
                    $(this).find('form').trigger('reset');
                    $(this).find('textarea').val('');
                    self.HomeworkPicture("");
                    self.currentFileSrc("");
                    self.goToFolder('All');
                })
            },
            error: function (res) {
                $.notify("Error please try again", "error");
            },
            complete: function () {
                $('#loader').addClass('hidden');
                $.notify("Homework: " + self.Title() + " created!", "success");
            }
        });
        self.showDialog(false);
    }
    //wtihout the focus on textarea file was not saved to observable
    self.focusOnTextArea = function () {
        var textbox = $('#homeworkContent');
        textbox.focus();
    }
    self.loadStudentsPerHomework = function (homework) {
        $.ajax({
            url: '/studentHomework/studentsByHomework',
            data: { id: homework.Id },
            type: 'get',
            beforeSend: function () {
                $('#loader').removeClass('hidden');
            },
            success: function (res) {
                var mappedStudents = $.map(res, function (item) {
                    return new Student(item, self.chosenHomeworkData().Id());
                });
                self.studentsPerHomework(mappedStudents);
                self.loadStudentsWithoutHomework(self.studentsPerHomework());
            },
            error: function (res) {
                $.notify("Error please try again", "error");

            },
            complete: function () {
                $('#loader').addClass('hidden');
            }
        });
    }

    //show student's solutions text and picture 
    self.showStudentHomework = function (student) {
        self.toggleStudentHomework(true);
        self.currentStudentName(student.FullName());
        self.studentSolutiuonToShow("");
        $.ajax({
            type: 'get',
            url: '/studentHomework/getSolution',
            dataType: 'json',
            traditional: true,
            data: {
                studentId: student.Id(),
                homeworkId: self.chosenHomeworkData().Id()
            },
            beforeSend: function () {
                $('#loader').removeClass('hidden');
            },
            success: function (res) {
                var solutiuon = new StudentHomework(res);
                
                self.studentSolutiuonToShow(solutiuon);
            },
            error: function (res) {
                $.notify("Error please try again", "error");

            },
            complete: function () {
                $('#loader').addClass('hidden');
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
            beforeSend: function () {
                $('#loader').removeClass('hidden');
            },
            success: function (res) {
                var mappedStudents = $.map(res, function (item) {
                    return new Student(item);
                });
                self.studentNotAssignedYet(mappedStudents);
            },
            error: function (res) {
                $.notify("Error please try again", "error");

            },
            complete: function () {
                $('#loader').addClass('hidden');
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
            beforeSend: function () {
                $('#loader').removeClass('hidden');
            },
            success: function (res) {
                self.selectedStudents([]);
                self.goToHomework(self.chosenHomeworkData());

            },
            error: function (res) {
                $.notify("Error please try again", "error");

            },
            complete: function () {
                $('#loader').addClass('hidden');
                $.notify("Students added!", "success");
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
            beforeSend: function () {
                $('#loader').removeClass('hidden');
            },
            success: function (res) {
                self.goToFolder('All');
                self.studentSolutionText = ko.observable("");
                self.studentSolutionPicture = ko.observable("");

            },
            error: function (res) {
                $.notify("Error please try again", "error");

            },
            complete: function () {
                $('#loader').addClass('hidden');
                $.notify("Homework submitted!", "success");
            }
        });
    }

    self.getStudentHomeworkWithFlag = function (isDone) {
        $.ajax({
            url: '/studentHomework/GetAllStudentHomeworkWithFlag',
            data: { isDone: isDone },
            type: 'get',
            beforeSend: function () {
                $('#loader').removeClass('hidden');
            },
            success: function (res) {
                if (res.length > 0) {
                    $(".book-homework").attr("title", "You have " + res.length + " homework(s) unsubmited.");
                    $(".book-homework").css("display", "block");
                    $("#txt").text(res.length);
                } else {
                    $("#txt").remove();
                }
                
            },
            error: function (res) {
                $.notify("Error please try again", "error");

            },
            complete: function () {
                $('#loader').addClass('hidden');
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
            self.loadCkEditor();
        }
        else {
            self.loadStudentsPerHomework(homework);
        }
        
    };

    //get clicked homework details 
    self.getStudentHomeworkDetails = function (homeworkId) {
        $.ajax({
            url: '/studentHomework/GetCurrentStudentHomework',
            data: { id: homeworkId },
            type: 'get',
            beforeSend: function () {
                $('#loader').removeClass('hidden');
            },
            success: function (res) {
                self.chosenStudentHomework(new StudentHomework(res));
                self.isCurrenHomeworkDone(self.chosenStudentHomework().IsDone());
            },
            error: function (res) {
                $.notify("Error please try again", "error");

            },
            complete: function () {
                $('#loader').addClass('hidden');
            }
        });
    }

    // BEHAVIOURS
    //determine which folders to load
    self.whichFolders = function (who) {
        if (who.isTeacher) self.folders(['All', 'Submitted', 'Unsubmitted']);
        else self.folders(['All', 'Submitted', 'Unsubmitted']);
    }

    //load filtered homeworks
    self.goToFolder = function (folder) {
        self.currentStudentMessage("");
        self.chosenFolderId(folder);
        $.ajax({
            type: 'get',
            url: '/homework/gethomeworks',
            data: { option: folder },
            beforeSend: function () {
                $('#loader').removeClass('hidden');
            },
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
            },
            error: function (res) {
                $.notify("Error please try again", "error");

            },
            complete: function () {
                $('#loader').addClass('hidden');
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

    self.getCourses = function () {
        $.ajax({
            url: '/Homework/GetCoursesOfTeacher',
            type: 'get',
            beforeSend: function () {
                $('#loader').removeClass('hidden');
            },
            success: function (res) {
                var mappedCourses = $.map(res, function (item) {
                    return new Course(item);
                });
                self.coursesAvailable(mappedCourses);
            },
            error: function (res) {
                $.notify("Error please try again", "error");

            },
            complete: function () {
                $('#loader').addClass('hidden');
            }
        });
    }

    //determine who is logged in
    self.isStudentOrTeacher = function () {
        $.get('/homework/isstudentorteacher').done(function (res) {
            self.isStudentStatus(res.isStudent);
            self.isTeacherStatus(res.isTeacher);
            self.whichFolders(res);
            self.checkIfTeacherLoggedAndLoadCourses();  
        });  
    }
    //load courses teacher's courses
    self.checkIfTeacherLoggedAndLoadCourses = function () {
        if (self.isTeacherStatus() === true) {
            self.getCourses();
        }
    }
    self.loadCkEditor = function () {
        if (self.isStudentStatus() === true && self.toggleDetailedView() === true) {
            //CKEDITOR.editorConfig = function (config) {
            //    config.language = 'fr';
            //    config.uiColor = '#AADC6E';
            //};
            
            CKEDITOR.replace('editor1', {
                //extraPlugins: 'font'
            });
        }
    }

    //establish what user is logged in
    self.isStudentOrTeacher();
    // show all homework by default
    self.goToFolder('All');
    
}

ko.applyBindings(new ViewModel());

