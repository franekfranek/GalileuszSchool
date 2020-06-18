﻿ko.bindingHandlers.modal = {
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

     // Data
    var self = this;
    this.showDialog = ko.observable(false);


    self.Title = ko.observable("");
    self.TextContent = ko.observable("");
    self.homeworks = ko.observableArray([]);
    //folders
    self.folders = ['All', 'Assigned', 'Not Assigned'];
    self.chosenFolderId = ko.observable();
    self.chosenFolderData = ko.observable();

    // Operations
    //========SAVE NEW HOMEWORK======
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


                })
            }, error: function (res) {
                console.log(res);
            }
        });
        self.showDialog(false);
    }

    // Load initial state from server, convert it to Homework instances, then populate self.homeworks
    //$.getJSON("/homework/gethomeworks", function (allData) {
    //    debugger;
    //    var mappedHomeworks = $.map(allData, function (item) { return new Homework(item) });
    //    self.homeworks(mappedHomeworks);
    //});  

    // Behaviours
    //LOAD FILTERED HOMEWORKS
    self.goToFolder = function (folder) {
        self.chosenFolderId(folder);
        $.ajax({
            type: 'get',
            url: '/homework/gethomeworks',
            data: { option: folder },
            success: function (result) {
                console.log(result);
                var mappedHomeworks = $.map(result, function (item) { return new Homework(item) });
                self.homeworks(mappedHomeworks)
            }
        });
    };
    // Show all homework by default
    self.goToFolder('All');
}

ko.applyBindings(new ViewModel());