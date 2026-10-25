$(document).ready(async function () {
  var categoriasSelect = document.getElementById("Categorias");
  var marcasSelect = document.getElementById("Marcas");

  const response = await fetch("/Categoria/GetCategoriasApi");
  const response2 = await fetch("/Marca/GetMarcasApi");

  const categorias = await response.json();
  const marcas = await response2.json();

  categorias.forEach((c) => {
    categoriasSelect.innerHTML +=
      "<option value=" + c.id + ">" + c.nombre + "</option>";
  });

  marcas.forEach((m) => {
    marcasSelect.innerHTML +=
      "<option value=" + m.id + ">" + m.nombre + "</option>";
  });
});

async function GetAtributos(e) {
  var atributosDiv = document.getElementById("Atributos");
  const response = await fetch(
    "/Categoria/GetCategoriaAtributos?categoriaId=" + e,
    {
      credentials: "include",
      method: "POST",
    },
  );
  const atributos = await response.json();
  atributosDiv.innerHTML = "";
  if (atributos.length > 0) {
    atributosDiv.innerHTML =
      '<label class="mb-3">Marca los atributos que quieras utilizar para los Skus:</label>';
  }
  for (var i = 0; i < atributos.length; i++) {
    var a = atributos[i];
    atributosDiv.innerHTML += `
                        <div class="mb-2">
                        <input class="form-check-input me-1" type="checkbox" value=${a.id} name=Atributos[${i}] /><label class="form-check-label">${a.nombre}</label>
                        </div>
                        `;
  }
}
