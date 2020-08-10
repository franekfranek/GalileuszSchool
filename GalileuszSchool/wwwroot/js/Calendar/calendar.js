document.addEventListener('DOMContentLoaded', generateCalendar);

function generateCalendar() {
    var calendarEl = document.getElementById('calendar');
    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'timeGridWeek',
        allDaySlot: false,
        hiddenDays: [0],
        slotMinTime: "08:00:00",
        slotMaxTime: "20:00:00",
        contentHeight: 625,
        selectable: true,
        editable: true,
        events: '/calendar/GetEvents',
        //eventClick: function (info) {
        //    info.jsEvent.preventDefault();

        //    if (info.event.url) {
        //        window.open(info.event.url);
        //    } else {
        //        Swal.fire(info.event.title, 'Start: ' + info.event.start + ' End: ' + info.event.end, 'question');
        //    }
        //},
        select: function (info) {
            // alert('selected ' + info.startStr + ' to ' + info.endStr);
            $('#createEvent').modal('show');
            $('#eventStart').val(info.startStr);
            $('#eventEnd').val(info.endStr);
            //saveEvent(info);
            //calendar.fullCalendar("refetchEvents");
            
        }
    });
    
    calendar.render();
}

//CREATE EVENT
$('#createNewEvent').on('click', function (e) {
    e.preventDefault();

    var data = {};
    $("#createEventform").serializeArray().map(function (x) { data[x.name] = x.value; }); 
    var mappedEvent = new Event(data);
    console.log(mappedEvent);

    $.ajax({
        url: "/calendar/create",
        data: mappedEvent,
        type: "post",
        //headers: {
        //    RequestVerificationToken:
        //        $('input:hidden[name="__RequestVerificationToken"]').val()
        //},
        success: function (res) {
            console.log(res);
            $('#createEvent').modal('hide');
            $('#createEvent').on('hidden.bs.modal', function () {
                $(this).find('form').trigger('reset');
                $(this).find('textarea').val('');
                generateCalendar();
            })
        }, error: function (res) {
            console.log(res);
        }
    });
    


    

    });

//EVENT MODEL
function Event(data) {
    console.log(data);
    this.Id = data.id;
    this.Title = data.title;
    this.Description = data.description;
    this.Start = data.start;
    this.End = data.end;
    this.CourseId = data.course;

}    
