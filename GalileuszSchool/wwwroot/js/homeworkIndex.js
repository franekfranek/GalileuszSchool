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

function Homework(data) {
    this.Id = ko.observable(data.id);
    this.Title = ko.observable(data.title);
    this.Slug = ko.observable(data.slug);
    this.IsDone = ko.observable(data.isDone);
    this.CreationDate = ko.observable(data.creationDate.substring(0, 10));
    this.solutionTextContent = ko.observable(data.solutionTextContent);
    this.TextContent = ko.observable(data.textContent);
    this.TeacherName = ko.observable(data.teacher.firstName + " " + data.teacher.lastName);
    this.studentSubmissionDate = ko.observable(data.studentSubmissionDate);
}



function ViewModel() {

    // DATA
    var self = this;//this has to be here so this contextes dont mix
    
    //modal
    this.showDialog = ko.observable(false);
    self.Title = ko.observable("");
    self.TextContent = ko.observable("");

    //list of homeworks
    self.homeworks = ko.observableArray([]);
    //folders
    self.folders = ko.observable();
    self.chosenFolderId = ko.observable();

    //who is logged
    self.isStudentStatus = ko.observable();
    self.isTeacherStatus = ko.observable();

    //detailed homeworks
    self.chosenHomeworkData = ko.observable();

    //toggle view
    self.toggleDetailedView = ko.observable(true);

    // OPERATIONS

    //save homework
    this.submit = function () {
        var homework = {};
        homework.Title= self.Title();
        homework.TextContent = self.TextContent();
        
        $.ajax({
            url: "/homework/create",
            data: ko.toJS(homework),
            type: "post",
            success: function (res) {
                console.log(res);
                $('#createHomeworkModal').on('hidden.bs.modal', function () {
                    $(this).find('form').trigger('reset');
                    $(this).find('textarea').val('');
                    self.goToFolder('All');
                })
            }, error: function (res) {
                console.log(res);
            }
        });
        self.showDialog(false);
    }
    self.goToHomework = function (homework) {
        self.chosenFolderId('All');
        //console.log(homework.Id);
        self.toggleDetailedView(true);
        $.get("/homework/findhomework", { id: homework.Id }, self.chosenHomeworkData).done(function (res) { console.log(res);});
    };

    // BEHAVIOURS
    //determine which folders to load
    self.whichFolders = function (who) {
        console.log(who.isTeacher);
        if (who.isTeacher) self.folders(['All', 'Assigned', 'Not Assigned']);
        else self.folders(['All', 'Done', 'Undone'])
    }

    //load filteres homeworks
    self.goToFolder = function (folder) {
        self.chosenFolderId(folder);
        $.ajax({
            type: 'get',
            url: '/homework/gethomeworks',
            data: { option: folder },
            success: function (result) {
                //convert it to Homework instances, then populate self.homeworks
                var mappedHomeworks = $.map(result, function (item) {
                    if(item.studentSubmissionDate.substring(0, 1) === '0'){
                        item.studentSubmissionDate = 'Not yet'
                    }
                    else {
                        item.studentSubmissionDate = item.studentSubmissionDate.substring(0, 10);
                    }
                    return new Homework(item)
                });
                self.homeworks(mappedHomeworks);
                console.log(mappedHomeworks);
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
    //self.goToFolder('All');
}

ko.applyBindings(new ViewModel());