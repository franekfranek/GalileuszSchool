//class model
function Class(data) {
    var self = this;
    self.Id = ko.observable(data.calendarEventId);
    self.Title = ko.observable(data.calendarEvent.title);
    self.Course = ko.observable(data.calendarEvent.course.name);
    self.Date = ko.observable(data.calendarEvent.start.substring(0, 10));
    if (data.isPaid === true) {
        self.IsPaid = ko.observable("Yes");
    } else self.IsPaid = ko.observable("No");

    if (data.isPresent === true) {
        self.IsPresent = ko.observable("Present");
    } else self.IsPresent = ko.observable("Absent");
    self.Price = ko.observable(data.calendarEvent.course.price);
    self.Checked = ko.observable(data.isPaid);
    self.IsPresent2 = ko.observable(data.isPresent);
}

function ViewModel() {

    var self = this;
    //DATA
    //list of classes per student
    self.classes = ko.observableArray([]);
    //who is logged
    self.isStudentStatus = ko.observable();
    self.isTeacherStatus = ko.observable();
    self.grandTotal = ko.pureComputed(function () {
        var total = 0;
        $.each(self.classes(), function () {
            console.log(this);
            if (this.Checked() === true && this.IsPaid() === 'No') {
                total += this.Price()
            }
        })
        return total;
    });
    self.isTest = ko.observable(true);


    //METHODS
    self.getClasses = function () {
        $.ajax({
            url: '/account/GetClasses',
            type: 'get',
            success: function (res) {
                console.log(res);
                var mappedClasses = $.map(res, function (item) {
                    return new Class(item);
                });
                self.classes(mappedClasses);
                console.log(mappedClasses);
            }
        });
    }

    //determine who is logged in
    self.isStudentOrTeacher = function () {
        $.get('/homework/isstudentorteacher').done(function (res) {
            self.isStudentStatus(res.isStudent);
            self.isTeacherStatus(res.isTeacher);
            if (self.isStudentStatus() === true) {
                self.getClasses();
            }

        });
    }

    //establish what user is logged in
    self.isStudentOrTeacher();
}

ko.applyBindings(new ViewModel());