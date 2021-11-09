// Set your publishable key: remember to change this to your live publishable key in production
// See your keys here: https://dashboard.stripe.com/apikeys
const stripe = Stripe('pk_test_51JhX39C1OcDeDQ0XknryEu26l0Sh687xS7zBdQ6dFnw2Og5NI52bMrinthhk1S5cU8PhRkwJspsSQ1UmXmUfFwUV00kNdmAsJ0');
var host = window.location.protocol + "//" + window.location.host;

(async () => {
    const appearance = {
        theme: 'night',
        labels: 'floating'


    };

    
    $.getJSON("/api/Payment", function (payment) {
        
        const { client_secret: clientSecret } = payment;
        const elements = stripe.elements({ clientSecret, appearance });
        const paymentElement = elements.create('payment');
        paymentElement.mount('#payment-element');

        const form = document.getElementById('payment-form');

        form.addEventListener('submit', async (event) => {
            //event.preventDefault();

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
                console.log(error);
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







