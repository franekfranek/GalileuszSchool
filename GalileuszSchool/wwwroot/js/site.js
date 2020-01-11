$(function () {
    //WINDOW CONFIRMATION POPUP a here is a tag 
    if ($("a.confirmDeletion").length) {
        $("a.confirmDeletion").click(() => {
            if (!confirm("Confirm deletion")) return false;
        });
    }
    // TEMP DATA NOTIFICATION HIDE
    if ($("div.alert.notification").length) {
        setTimeout(() => {
            $("div.alert.notification").fadeOut();
        }, 2000);
    }
});