async function GetAtributos(e) {
    var atributosDiv = document.getElementById("Atributos");
    const response = await fetch(
        "/Categoria/GetCategoriaAtributos?categoriaId=" + e,
        {
            credentials: "include",
            method: "POST",
        },
    );
    const data = await response.json();
    atributosDiv.innerHTML = "";
    if (data.atributos.length > 0) {
        atributosDiv.innerHTML =
            '<label class="mb-3">Marca los atributos que quieras utilizar para los Skus:</label>';
        for (var i = 0; i < data.atributos.length; i++) {
            var a = data.atributos[i];
            atributosDiv.innerHTML += `
                        <div class="mb-2">
                        <input class="form-check-input me-1" type="checkbox" value=${a.id} name=Atributos[${i}] /><label class="form-check-label">${a.nombre}</label>
                        </div>
                        `;
        }
    } else {
        atributosDiv.innerHTML =
            '';
    }

}
