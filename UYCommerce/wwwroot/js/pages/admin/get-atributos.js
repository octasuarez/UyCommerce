async function CrearAtributo() {
  let nombre = document.getElementById("atributoNombre").value;

  const response = await fetch("/Atributo/CrearAtributo", {
    method: "POST",
    credentials: "include",
    headers: { "Content-Type": "application/json", Accept: "application/json" },
    body: JSON.stringify(nombre),
  })
    .then((response) => {
      return response.json();
    })
    .then((msg) => {
      document.getElementById("error").innerText = msg;
    });
}

function AgregarInput(e) {
  let element = document.getElementById(e.dataset.divId);
  let input = element.getElementsByTagName("input");
  if (!input.length > 0) {
    element.innerHTML += `<div>
                    <div class="d-flex">
                        <input id="${e.dataset.id}input" data-atributo-id=${e.dataset.id} class="form-control me-2" type="text" name="Valor" placeholder="Valor"/>
                        <button data-input="${e.dataset.id}input" onclick="AgregarValor(this)" type="button" class="btn btn-primary me-2">Guardar</button>
                        <button onclick="BorrarInput(this.parentElement)" type="button" class="btn btn-danger">X</button>
                    </div>
                    <span class="text-danger"></span>
                </div>
                `;
  }
}

function BorrarInput(e) {
  e.parentElement.remove();
}

async function AgregarValor(e) {
  let input = document.getElementById(e.dataset.input);
  let Valor = input.value;
  let AtributoId = input.dataset.atributoId;

  let error = e.parentElement.nextElementSibling;

  e.setAttribute("disabled", "");

  const response = await fetch("/Atributo/AgregarValor", {
    method: "POST",
    credentials: "include",
    headers: { "Content-Type": "application/json", Accept: "application/json" },
    body: JSON.stringify({ Valor, AtributoId }),
  })
    .then((response) => {
      return response.json();
    })
    .then((result) => {
      if (result.status === 200) {
        e.parentElement.remove();
        location.reload();
      } else {
        setTimeout(function () {
          e.removeAttribute("disabled");
          error.innerText = result.error;
        }, 1000);
      }
    });
}
