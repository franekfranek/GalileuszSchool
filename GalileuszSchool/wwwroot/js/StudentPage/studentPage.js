//class model
function Class(data) {
    var self = this;
    var courseName = data.calendarEvent.course.name;
    self.Id = ko.observable(data.calendarEventId);
    //cutting the course name ending for better view
    self.Title = ko.observable(data.calendarEvent.title.substring(0, data.calendarEvent.title.indexOf(courseName)));
    self.Course = ko.observable(courseName);
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
    self.noClassesYet = ko.observable(false);

    self.grandTotal = ko.pureComputed(function () {
        var total = 0;
        $.each(self.classes(), function () {
            if (this.Checked() === true && this.IsPaid() === 'No') {
                total += this.Price()
            }
        })
        return total;
    });
    self.isTest = ko.observable(true);
    self.dotPayLink = ko.pureComputed(function () {
        var link = "https://ssl.dotpay.pl/t2/?id=123456&amount=" + self.grandTotal() + "&currency=PLN&description=Test";
        return link;
    });


    //METHODS
    self.getClasses = function () {
        $.ajax({
            url: '/account/GetClasses',
            type: 'get',
            beforeSend: function () {
                $('#loader').removeClass('hidden');
            },
            success: function (res) {
                if (res.length !== 0) {
                    self.noClassesYet(false);
                    var mappedClasses = $.map(res, function (item) {
                        return new Class(item);
                    });
                    self.classes(mappedClasses);
                }else {
                    self.noClassesYet(true);
                }
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
            if (self.isStudentStatus() === true) {
                self.getClasses();
            }

        });
    }

    //establish what user is logged in
    self.isStudentOrTeacher();
}
window.vm = new ViewModel();
ko.applyBindings(vm);


window.onload = function () {
     onGooglePayLoaded();
}
const baseRequest = {
    apiVersion: 2,
    apiVersionMinor: 0
};

const allowedCardNetworks = ["AMEX", "DISCOVER", "INTERAC", "JCB", "MASTERCARD", "VISA"];
const allowedCardAuthMethods = ["PAN_ONLY", "CRYPTOGRAM_3DS"];
const tokenizationSpecification = {
    type: 'PAYMENT_GATEWAY',
    parameters: {
        'gateway': 'example',
        'gatewayMerchantId': 'exampleGatewayMerchantId'
    }
};
const baseCardPaymentMethod = {
    type: 'CARD',
    parameters: {
        allowedAuthMethods: allowedCardAuthMethods,
        allowedCardNetworks: allowedCardNetworks
    }
};
const cardPaymentMethod = Object.assign(
    {},
    baseCardPaymentMethod,
    {
        tokenizationSpecification: tokenizationSpecification
    }
);
let paymentsClient = null;

function getGoogleIsReadyToPayRequest() {
    return Object.assign(
        {},
        baseRequest,
        {
            allowedPaymentMethods: [baseCardPaymentMethod]
        }
    );
}

function getGooglePaymentDataRequest() {
    const paymentDataRequest = Object.assign({}, baseRequest);
    paymentDataRequest.allowedPaymentMethods = [cardPaymentMethod];
    paymentDataRequest.transactionInfo = getGoogleTransactionInfo();
    paymentDataRequest.merchantInfo = {
        // @todo a merchant ID is available for a production environment after approval by Google
        // See {@link https://developers.google.com/pay/api/web/guides/test-and-deploy/integration-checklist|Integration checklist}
        // merchantId: '01234567890123456789',
        merchantName: 'Example Merchant'
    };
    return paymentDataRequest;
}

function getGooglePaymentsClient() {
    if (paymentsClient === null) {
        paymentsClient = new google.payments.api.PaymentsClient({ environment: 'TEST' });
    }
    return paymentsClient;
}

function onGooglePayLoaded() {
    const paymentsClient = getGooglePaymentsClient();
    paymentsClient.isReadyToPay(getGoogleIsReadyToPayRequest())
        .then(function (response) {
            if (response.result) {
                addGooglePayButton();
                // @todo prefetch payment data to improve performance after confirming site functionality
                // prefetchGooglePaymentData();
            }
        })
        .catch(function (err) {
            // show error in developer console for debugging
            console.error(err);
        });
}

function addGooglePayButton() {
    const paymentsClient = getGooglePaymentsClient();
    const button =
        paymentsClient.createButton({
            onClick: onGooglePaymentButtonClicked,
            buttonType: 'short'
        });

    document.getElementById('container').appendChild(button);
}

function getGoogleTransactionInfo() {
    var total = vm.grandTotal();
    return {
        countryCode: 'PL',
        currencyCode: 'PLN',
        totalPriceStatus: 'FINAL',
        // set to cart total
        totalPrice: String(total)
    };
}

function prefetchGooglePaymentData() {
    const paymentDataRequest = getGooglePaymentDataRequest();
    // transactionInfo must be set but does not affect cache
    paymentDataRequest.transactionInfo = {
        totalPriceStatus: 'NOT_CURRENTLY_KNOWN',
        currencyCode: 'USD'
    };
    const paymentsClient = getGooglePaymentsClient();
    paymentsClient.prefetchPaymentData(paymentDataRequest);
}

function onGooglePaymentButtonClicked() {
    const paymentDataRequest = getGooglePaymentDataRequest();
    paymentDataRequest.transactionInfo = getGoogleTransactionInfo();

    const paymentsClient = getGooglePaymentsClient();
    paymentsClient.loadPaymentData(paymentDataRequest)
        .then(function (paymentData) {
            // handle the response
            processPayment(paymentData);
        })
        .catch(function (err) {
            // show error in developer console for debugging
            console.error(err);
        });
}

function processPayment(paymentData) {
    // show returned data in developer console for debugging
    console.log(paymentData);
    // @todo pass payment token to your gateway to process payment
    paymentToken = paymentData.paymentMethodData.tokenizationData.token;
}




