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
    this.IsDone = ko.observable(data.isDone);
    this.CreationDate = ko.observable(data.creationDate.substring(0, 10));
    this.solutionTextContent = ko.observable(data.solutionTextContent);
    this.TextContent = ko.observable(data.textContent);
    this.TeacherName = ko.observable(data.teacher.firstName + " " + data.teacher.lastName);
    //this.studentSubmissionDate = ko.observable(data.studentSubmissionDate);
}

//student's homework model
function StudentHomework(data) {
    this.Id = ko.observable(data.id);
    this.Title = ko.observable(data.title);
    this.Slug = ko.observable(data.slug);
    this.IsDone = ko.observable(data.isDone);
    this.CreationDate = ko.observable(data.creationDate.substring(0, 10));
    this.solutionTextContent = ko.observable(data.solutionTextContent);
    this.TextContent = ko.observable(data.textContent);
    this.TeacherName = ko.observable(data.teacher.firstName + " " + data.teacher.lastName);
    //this.studentSubmissionDate = ko.observable(data.studentSubmissionDate);
}

function Student(data) {
    this.Id = ko.observable(data.id);
    this.FullName = ko.observable(data.firstName + ' ' + data.lastName);
    this.Email = ko.observable(data.email);
}
ko
function ViewModel() {

    // DATA
    var self = this;//this has to be here so this contextes dont mix
    
    //modal
    this.showDialog = ko.observable(false);
    self.Title = ko.observable("");
    self.TextContent = ko.observable("");
    self.HomeworkPicture = ko.observable("");

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
    self.currentHomeworkId = ko.observable();
    self.currentHomeworkObject = ko.observable();
    self.chosenHomeworkImageSrc = ko.computed(function () {
        return '/media/homeworks/' + self.chosenHomeworkData().imageContent;
    });

    //toggle view
    self.toggleDetailedView = ko.observable(true);

    //list of students 
    self.studentsPerHomework = ko.observableArray([]);
    self.studentNotAssignedYet = ko.observableArray([]);


    self.selectedStudents = ko.observableArray([]);
    // OPERATIONS

    //save homework
    this.submit = function () {
        var data = new FormData();

        data.append('Title', self.Title());
        data.append('TextContent', self.TextContent());
        data.append('PhotoContent', self.HomeworkPicture());
        

        for (var key of data.entries()) {
            console.log(key[0] + ', ' + key[1]);
        }
        
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
                    self.goToFolder('All');
                })
            }, error: function (res) {
                console.log(res);
            }
        });
        self.showDialog(false);
    }

    self.fileSelect = function (elemet, event) {
        var file = event.target.files[0];// FileList object which is part of event properties for current element!
        // render image file as thumbnail.
        var reader = new FileReader();

        // Closure to capture the file information.
        reader.onload = (function (theFile) {
            return function (e) {
                console.log(theFile);
                self.HomeworkPicture(theFile);
            };
        })(file);
        // Read in the image file as a data URL.
        reader.readAsDataURL(file);
    }
   

    //find specific homework
    self.goToHomework = function (homework) {
        self.chosenFolderId('All');
        self.toggleDetailedView(true);
        $.get("/homework/findhomework",
            { id: homework.Id },
            self.chosenHomeworkData).done(function (res) {
                self.loadStudentsPerHomework(res);
                self.currentHomeworkId(res.id);
                self.currentHomeworkObject(homework);
                console.log(self.chosenHomeworkImageSrc());
            });
    };

    self.loadStudentsPerHomework = function (homework) {
        $.ajax({
            url: '/studentHomework/studentsByHomework',
            data: { id: homework.id },
            type: 'get',
            success: function (res) {
                var mappedStudents = $.map(res, function (item) {
                    return new Student(item);
                });
                self.studentsPerHomework(mappedStudents);
                self.loadStudentsWithoutHomework(self.studentsPerHomework());
            }
        })
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


    // check all the checkboxes
    //https://knockoutjs.com/documentation/computed-writable.html newer
    self.SelectAll = ko.dependentObservable({
        read: function () {
            return self.selectedStudents().length === self.studentNotAssignedYet().length;
        },
        write: function (newValue) {
            self.selectedStudents(self.selectedStudents().length === self.studentNotAssignedYet().length ? [] : self.studentNotAssignedYet().slice(0));
        }
    });

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
                homeworkId: self.currentHomeworkId(),
                studentsIds: ids
            },
            success: function (res) {
                self.selectedStudents([]);
                self.goToHomework(self.currentHomeworkObject());
               
            }
        }); 
    }

    // BEHAVIOURS
    //determine which folders to load
    self.whichFolders = function (who) {
        console.log(who.isTeacher);
        if (who.isTeacher) self.folders(['All', 'Assigned', 'Not Assigned']);
        else self.folders(['All', 'Done', 'Undone'])
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
                    //if(item.studentSubmissionDate.substring(0, 1) === '0'){
                    //    item.studentSubmissionDate = 'Not yet'
                    //}
                    //else {
                    //    item.studentSubmissionDate = item.studentSubmissionDate.substring(0, 10);
                    //}
                    return new Homework(item)
                });
                self.homeworks(mappedHomeworks);
                self.toggleDetailedView(false);
            }
        });
    };

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