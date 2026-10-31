function initPayPalCheckout() {
    paypal
        .Buttons({
            async createOrder() {
                try {
                    const response = await fetch("/create-paypal-order", {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json",
                        },
                    });

                    const orderData = await response.json();

                    if (orderData.id) {
                        return orderData.id;
                    } else {
                        const errorDetail = orderData?.details?.[0];
                        const errorMessage = errorDetail
                            ? `${errorDetail.issue} ${errorDetail.description} (${orderData.debug_id})`
                            : JSON.stringify(orderData);

                        throw new Error(errorMessage);
                    }
                } catch (error) {
                    resultMessage(`Lo sentimos, no se pudo iniciar su orden...<br><br>${error}`);
                }
            },
            async onApprove(data, actions) {
                try {
                    const response = await fetch(`/orders/${data.orderID}/capture`, {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json",
                        },
                    });

                    const orderData = await response.json();
                    const errorDetail = orderData?.details?.[0];

                    if (errorDetail?.issue === "INSTRUMENT_DECLINED") {
                        return actions.restart();
                    } else if (errorDetail) {
                        throw new Error(errorDetail);
                    } else if (!orderData.purchase_units) {
                        throw new Error(JSON.stringify(orderData));
                    } 
                    window.location.href = `/Compras/${orderData.id}`;
                } catch (error) {
                    resultMessage(`Lo sentimos, su compra no pudo ser concretada...<br><br>${error}`);
                }
            },
            onError(err) {
                resultMessage(`Lo sentimos, su compra no pudo ser concretada...<br><br>${err}`);
            },
        })
        .render("#paypal-button-container")
}

function resultMessage(message) {
  const container = document.querySelector("#result-message");
  container.innerHTML = message;
}

$(window).on("load", function () {
  $("#loading").hide();
});

function ModificarQty(e) {
  let formData = {
    skuId: e.getAttribute("data-value"),
    quantity: e.getAttribute("data-qty"),
  };
  $.ajax({
    type: "POST",
    url: '@Url.Action("AgregarProductoAlCarrito", "Carrito")',
    data: formData,
    success: function (response) {
      location.reload();
    },
  });
}

function EliminarProductoCarrito(e) {
  let formData = {
    skuId: e.getAttribute("data-value"),
  };
  $.ajax({
    type: "POST",
    url: '@Url.Action("EliminarProductoCarrito", "Carrito")',
    data: formData,
    success: function (response) {
      location.reload();
    },
  });
}
