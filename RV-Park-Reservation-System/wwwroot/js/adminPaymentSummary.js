// Set your publishable key: remember to change this to your live publishable key in production
// See your keys here: https://dashboard.stripe.com/apikeys
const stripe = Stripe('pk_test_51JhX39C1OcDeDQ0XknryEu26l0Sh687xS7zBdQ6dFnw2Og5NI52bMrinthhk1S5cU8PhRkwJspsSQ1UmXmUfFwUV00kNdmAsJ0');
var host = window.location.protocol + "//" + window.location.host;
function Warning() {
    swal({
        title: "Are you sure?",
        text: "This will delete your old reservation and create you a new one. This process cannot be undone!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then(function (willDelete) {
        if (willDelete) {
            console.log(document.getElementById('resID').value);
            $.ajax({
                type: 'DELETE',
                url: '/api/adminReservationUpdate/' + document.getElementById('resID').value,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);

                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            }).done(function () {

                document.getElementById("payment-form").submit();
                return true;

            });
        }
        else {
            return false;
        }
    })
}

//Loads the stripe elements in the page when a payment intent exists. 
(async () => {
    const appearance = {
        theme: 'night',
        labels: 'floating'


    };

    //Gets the payment intent client secret. 
    $.getJSON("/api/Payment", function (payment) {
        
        const { client_secret: clientSecret } = payment;
        const elements = stripe.elements({ clientSecret, appearance });
        const paymentElement = elements.create('payment');
        paymentElement.mount('#payment-element');

        const form = document.getElementById('payment-form');

        form.addEventListener('submit', async (event) => {
            //event.preventDefault();
            console.log('paymentSummaryjs');
            
            const { error } = await stripe.confirmPayment({
                //`Elements` instance that was used to create the Payment Element
                elements,
                redirect: "if_required",
                confirmParams: {
                    
                    return_url: host + "/Client/PaymentConfirmation"
                },
            });

            if (error) {
                // This point will only be reached if there is an immediate error when
                // confirming the payment. Show error to your customer (e.g., payment
                // details incomplete)
                console.log('This is the error', error);
                const messageContainer = document.querySelector('#error-message');
                messageContainer.textContent = error.message;
            } else {
                console.log("success");
                //code before the pause

                // Your customer will be redirected to your `return_url`. For some payment
                // methods like iDEAL, your customer will be redirected to an intermediate
                // site first to authorize the payment, then redirected to the `return_url`.
            }
           

        });


    });


	


})();







