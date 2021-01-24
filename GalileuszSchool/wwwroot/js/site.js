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

$("#menu-toggle").click(function (e) {
    console.log(e);
    e.preventDefault();
    $("#wrapper").toggleClass("toggled");
});

var map;
function initMap() {
    var coordinates = { lat: 52.199636, lng: 20.933612 }
    map = new google.maps.Map(document.getElementById("map"), {
        center: coordinates,
        zoom: 15
    });

    var marker = new google.maps.Marker({
        position: coordinates,
        map: map,
        title: "Galileusz School"
    });
}

function linkToSocialMedia(option) {
    switch (option) {
        case 'git':
            window.location = "https://github.com/franekfranek";
            break;
        case 'li':
            window.location = "https://linkedin.com/in/franciszek-zawadzki-0821961a2";
            break;
        case 'so':
            window.location = "https://stackoverflow.com/users/12086742/kenarf";
            break;
        default:
            console.log("error");
    };
};
