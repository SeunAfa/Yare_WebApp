////////////////////////////////////////////
//Shopping Bag List Show & Hide Component
const shoppingBag_Icon = document.getElementById("shoppingBag-Icon");
const continueShopping_Btn = document.querySelector(".continueShopping-Btn");
const shoppingBag_Container = document.getElementById("shoppingBag-Container");
const digitalStoreFront_Container = document.getElementById("digitalStoreFront-Container");

//Show Shopping Cart 
shoppingBag_Icon.addEventListener("click", () => {
    if (shoppingBag_Container.style.display !== "block") {
        gsap.fromTo("#shoppingBag-Container", {
            x: 1000
        }, {
            x: 0,
            duration: 0.50,
            onStart: function () {

                shoppingBag_Container.style.display = "block";
                digitalStoreFront_Container.classList.add("shoppingBag-contentActive");

            }
        });
        document.body.classList.add('noscroll');
        killAddToCartBtnScrollTriggers(); // Disable the ScrollTriggers for secondary addToCart Btn
    }
});

//Hide Shopping Cart 
continueShopping_Btn.addEventListener("click", () => {
    gsap.fromTo("#shoppingBag-Container", {
        x: 0
    }, {
        x: 1000,
        duration: 0.60,
        onComplete: function () {
            shoppingBag_Container.style.display = "none";
            digitalStoreFront_Container.classList.remove("shoppingBag-contentActive");
        },
    });
    document.body.classList.remove('noscroll');
    detailsPgAddToCartBtnScrollTriggers(); // Re-enable the ScrollTriggers for secondary addToCart Btn
});

///////////////////////////////////////
//Shopping Bag List Methods Component
function sendAjaxRequest(url, cartItemId) {
    var xhr = new XMLHttpRequest();
    xhr.open("POST", url, true);
    xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
    xhr.onload = function () {
        if (xhr.status === 200) {
            updateCart(xhr.responseText);
        } else {
            console.error("AJAX request failed:", xhr.responseText);
        }
    };
    xhr.send("cartItemId=" + encodeURIComponent(cartItemId));
}

//Update cart data from features operations
function updateCart(response) {
    try {
        var data = JSON.parse(response);

        if (data.success) {
            //Format currency 
            var formatter = new Intl.NumberFormat('en-GB', {
                style: 'currency',
                currency: 'GBP'
            });

            
            function parsePrice(priceStr) {
                return parseFloat(priceStr.replace(/[^\d.-]/g, ''));
            }

            var totalItemCount = 0;

            data.items.forEach(function (item) {
                var price = parsePrice(item.price);
                var count = parseInt(item.count, 10);

                if (!isNaN(price) && !isNaN(count)) {
                    totalItemCount += count;

                    //Shoppingcart display price for each product by (count * price) and count 
                    var itemElement = document.querySelector('.shoppingBag-Item[data-id="' + item.id + '"]');
                    if (itemElement) {
                        //Get count 
                        var countElement = itemElement.querySelector('.item-count strong');
                        if (countElement) {
                            countElement.textContent = count;
                        }

                        //Display price for each product by count * price
                        var priceElement = itemElement.querySelector('.Countprice');
                        if (priceElement) {
                            priceElement.textContent = formatter.format(price * count);
                        }
                    }

                    //Delivery Details Pg display price for each product by (count * price) and count 
                    var deliveryItemElement = document.querySelector('.checkout-product-Item[data-id="' + item.id + '"]');
                    if (deliveryItemElement) {
                        var deliveryCountElement = deliveryItemElement.querySelector('.shoppingBag-info-Text .item-count');
                        var deliveryPriceElement = deliveryItemElement.querySelector('.shoppingBag-info-Text .Countprice');

                        if (deliveryCountElement) {
                            deliveryCountElement.textContent = count;
                        }

                        if (deliveryPriceElement) {
                            deliveryPriceElement.textContent = formatter.format(price * count);
                        }
                    }

                } else {
                    console.error("Invalid price or count for item:", item);
                }
            });

            //Shoppingcart total 
            var orderTotalElement = document.querySelector('.OrderTotal strong');
            var orderTotal = 0;
            if (orderTotalElement) {
                orderTotal = parsePrice(data.orderTotal);
                if (!isNaN(orderTotal)) {
                    orderTotalElement.textContent = formatter.format(orderTotal);
                } else {
                    console.error("Invalid order total:", data.orderTotal);
                }
            }

            //Delivery Details total 
            var deliveryOrderTotalElement = document.querySelector('.checkout-card-bodyRight-Bottom .OrderTotal strong');
            if (deliveryOrderTotalElement) {
                var deliveryOrderTotal = parsePrice(data.orderTotal);
                if (!isNaN(deliveryOrderTotal)) {
                    deliveryOrderTotalElement.textContent = formatter.format(deliveryOrderTotal);
                } else {
                    console.error("Invalid delivery order total:", data.orderTotal);
                }
            }

            //Each product data in shoppingcart 
            document.querySelectorAll('.shoppingBag-Item').forEach(function (itemElement) {
                var itemId = itemElement.getAttribute('data-id');
                if (!data.items.find(item => item.id == itemId)) {
                    itemElement.remove();
                }
            });

            //Each product data in shoppingcart for deliveryDetails Pg
            document.querySelectorAll('.checkout-product-Item').forEach(function (deliveryItemElement) {
                var itemId = deliveryItemElement.getAttribute('data-id');
                if (!data.items.find(item => item.id == itemId)) {
                    deliveryItemElement.remove();
                }
            });

            //If shopping bag has no items display following messgaes 
            if (data.items.length === 0) {
                document.getElementById('shoppingBag-productItems-Top').innerHTML = '<p class="text-white">Your shopping bag is currently empty.</p>';
            }

            toggleCheckoutButton(orderTotal);
            // Update the cart counter with the total item count
            updateCartCounter(totalItemCount);

        } else {
            console.error("Error updating cart:", data.message);
        }
    } catch (e) {
        console.error("Failed to parse JSON response:", e);
    }
}

//Display cart count if order items is not 0
function updateCartCounter(count) {
    var cartCounterElement = document.querySelector('.cartCounter');
    if (cartCounterElement) {
        if (count === 0) {
            cartCounterElement.textContent = '';
        } else {
            cartCounterElement.textContent = count;
        }
    }
}

//Display checkout Btn if order total is not 0
function toggleCheckoutButton(orderTotal) {
    var checkoutBtn = document.getElementById('checkoutBtn');
    var shoppingBag_productItemsBottom = document.getElementById('shoppingBag-productItems-bottom');
    if (checkoutBtn && shoppingBag_productItemsBottom) {
        if (orderTotal === 0) {
            checkoutBtn.style.display = 'none';
            shoppingBag_productItemsBottom.style.display = 'none';

        } else {
            checkoutBtn.style.display = 'block';
            shoppingBag_productItemsBottom.style.display = 'block';
        }
    }
}

//Display order total element exisiting 
function getInitialOrderTotal() {
    var orderTotalElement = document.querySelector('.OrderTotal strong');
    if (orderTotalElement) {
        return parseFloat(orderTotalElement.textContent.replace(/[^\d.-]/g, ''));
    }

    return 0;

}

// Update the cart counter on page load
document.addEventListener('DOMContentLoaded', function () {
    var initialOrderTotal = getInitialOrderTotal();
    toggleCheckoutButton(initialOrderTotal);

    updateCartCounter(document.querySelectorAll('.shoppingBag-Item').length);
});

//Increment Btn 
document.querySelectorAll('.addBtn').forEach(function (button) {
    button.addEventListener('click', function (e) {
        e.preventDefault();
        var cartItemId = this.getAttribute('data-id');
        sendAjaxRequest(urls.increment, cartItemId);
    });
});

//Decrement Btn
document.querySelectorAll('.minusBtn').forEach(function (button) {
    button.addEventListener('click', function (e) {
        e.preventDefault();
        var cartItemId = this.getAttribute('data-id');
        sendAjaxRequest(urls.decrement, cartItemId);
    });
});

//Remove Btn 
document.querySelectorAll('.removeItemBtn').forEach(function (button) {
    button.addEventListener('click', function (e) {
        e.preventDefault();
        var cartItemId = this.getAttribute('data-id');
        sendAjaxRequest(urls.remove, cartItemId);
    });
});
