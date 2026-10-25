document.querySelectorAll(".add-to-cart").forEach((e) =>
  e.addEventListener("click", (e) => {
    var element = e.currentTarget;
    $.ajax({
      url: "/Carrito/AgregarProductoAlCarrito",
      method: "post",
      data: { skuId: element.dataset.skuId, quantity: "1" },
      success: (response) => {
        location.reload();
      },
      error: (response) => {},
    });
  }),
);

document.querySelectorAll(".delete-btn").forEach((e) =>
  e.addEventListener("click", (e) => {
    var element = e.currentTarget;

    $.ajax({
      url: "/Usuario/AgregarAFavoritos",
      method: "post",
      data: { skuId: element.dataset.skuId },
      success: (response) => {
        location.reload();
      },
      error: (response) => {},
    });
  }),
);
