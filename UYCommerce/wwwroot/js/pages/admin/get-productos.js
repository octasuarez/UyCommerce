async function EliminarSku(e) {
  let skuId = e.dataset.skuId;

  const response = await fetch("/Producto/EliminarSkuProducto", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    credentials: "include",
    body: JSON.stringify(skuId),
  })
    .then((response) => {
      if (response.ok) {
        document.getElementById(skuId).remove();
        ShowToast("", "Eliminado");
      } else {
        return response.json();
      }
    })
    .then((responseJson) => {
      if (responseJson) {
        ShowToast("error", responseJson.error);
      }
    });
}

async function EliminarProducto(e) {
  let productoId = e.dataset.productoId;

  const response = await fetch("/Producto/EliminarProducto", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    credentials: "include",
    body: JSON.stringify(productoId),
  })
    .then((response) => {
      if (response.ok) {
        alert("Deleted");
        location.reload();
        ShowToast("", "Eliminado");
      } else {
        return response.json();
      }
    })
    .then((responseJson) => {
      if (responseJson) {
        ShowToast("error", responseJson.error);
      }
    });
}

function FeaturedChanged(e) {
  let productId = e.dataset.productoId;
  let checked = e.checked;

  e.disabled = true;

  $.ajax({
    url: "/Producto/Feature",
    method: "POST",
    data: { productId: productId, featured: e.checked },
    success: function (result) {
      ShowToast("", result.msg);
    },
    error: function (result) {
      ShowToast(result);
    },
    statusCode: {
      401: function () {
        location.href = "/login";
      },
    },
  });

  if (checked === false) {
    e.removeAttribute("checked");
  } else {
    e.setAttribute("checked", "");
  }

  setTimeout(function () {
    document.getElementById(e.id).disabled = false;
  }, 3000);
}
