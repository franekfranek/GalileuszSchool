window.fbAsyncInit = function () {
    FB.init({
        appId: '482202485962412',
        cookie: true,
        xfbml: true,
        version: 'v7.0'
    });

    FB.AppEvents.logPageView();

};

(function (d, s, id) {
    var js, fjs = d.getElementsByTagName(s)[0];
    if (d.getElementById(id)) { return; }
    js = d.createElement(s); js.id = id;
    js.src = "https://connect.facebook.net/en_US/sdk.js";
    fjs.parentNode.insertBefore(js, fjs);
}(document, 'script', 'facebook-jssdk'));


function checkLoginState() {
    FB.getLoginStatus(function (response) {
        console.log(typeof (response.authResponse.accessToken));
        statusChangeCallback(response);
    });
}

function statusChangeCallback(response) {

    $.ajax({
        method: "POST",
        url: '/Account/SignInWithFb',
        data: { accessToken: response.authResponse.accessToken},
        success: function (res) {
            console.log(res);
        }
    });
}