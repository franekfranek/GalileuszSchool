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


function readURL(input) {
    if (input.files && input.files[0]) {
        let reader = new FileReader();

        reader.onload = function (e) {
            $("img#image").attr("src", e.target.result).width(200).height(200);
        };
        reader.readAsDataURL(input.files[0])
    }
}