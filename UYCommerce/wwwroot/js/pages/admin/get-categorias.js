function FeaturedChanged(e) {
  let categoriaId = e.dataset.categoriaId;
  let checked = e.checked;

  e.disabled = true;

  $.ajax({
    url: "/Categoria/Feature",
    method: "POST",
    data: { categoriaId: categoriaId, featured: e.checked },
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
