function CreateMarca() {
  let form = new FormData(document.getElementById("form-marca"));
  let nombre = Object.fromEntries(form).nombre;

  $.ajax({
    url: "/Marca/CreateMarca",
    method: "post",
    data: { nombre: nombre },
    dataType: "json",
    statusCode: {
      200: () => {
        location.href = "/admin/marcas";
      },
      400: (data) => {
        let error = data.responseJSON.error;
        document.getElementById("error").innerText = error;
      },
    },
  });
}

function DeleteMarca(e) {
  $.ajax({
    url: "/Marca/DeleteMarca",
    method: "post",
    data: { marcaId: e.dataset.marcaId },
    success: () => {
      location.reload();
      ShowToast("Success", "Marca eliminada");
    },
    error: () => {
      ShowToast("", "error");
    },
  });
}
