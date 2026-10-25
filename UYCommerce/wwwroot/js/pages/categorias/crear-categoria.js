$(document).ready(async function () {
  let atributos = document.getElementById("atributos");

  const response = await fetch("/Atributo/GetAtributosJson", {
    method: "GET",
    headers: { "Content-Type": "application/json", Accept: "application/json" },
    credentials: "include",
  })
    .then((response) => {
      if (response.ok) {
        return response.json();
      }
    })
    .then((result) => {
      atributos.innerHTML = "";

      for (var i = 0; i < result.length; i++) {
        var a = result[i];
        atributos.innerHTML += `<div class="d-flex">
                            <input class="form-check-input me-2" type="checkbox" value=${a.id} name="Atributos"/><span class="form-check-label">${a.nombre}</span>
                            </div>`;
      }
    });
});
async function CategoriaPadreInput(e) {
  if (e.value == "false") {
    const response = await fetch("/Categoria/GetCategoriasApi", {
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
      },
      credentials: "include",
      method: "GET",
    })
      .then((response) => {
        return response.json();
      })
      .then((categorias) => {
        var element = document.getElementById("CategoriaPadre");

        if (categorias.length > 0 && !element) {
          let span = document.createElement("span");
          span.innerText = "Sub categoria de:";
          span.id = "CatPadreSpan";
          span.className = "fw-semibold me-2";

          let select = document.createElement("select");

          select.name = "CategoriaPadreId";
          select.id = "CategoriaPadre";
          select.className = "card";

          for (var i = 0; i < categorias.length; i++) {
            var c = categorias[i];
            var option = document.createElement("option");
            option.value = c.id;
            option.innerText = c.nombre;
            select.appendChild(option);
          }

          document.getElementById("selectCatPadre").appendChild(span);
          document.getElementById("selectCatPadre").appendChild(select);
        } else {
        }
      });
  } else {
    let element = document.getElementById("selectCatPadre");
    if (element) {
      element.innerHTML = "";
    }
  }
}
