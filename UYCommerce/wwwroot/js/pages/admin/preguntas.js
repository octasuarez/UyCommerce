async function AgregarRespuesta(e) {
  let formData = new FormData(document.getElementById(e.dataset.formId));
  let form = Object.fromEntries(formData);
  const response = await fetch("/Admin/AgregarRespuesta", {
    headers: { "Content-Type": "application/json" },
    method: "POST",
    credentials: "include",
    body: JSON.stringify(form),
  })
    .then((response) => {
      if (response.ok) {
        return response.json();
      }
    })
    .then((responseJson) => {
      let respuesta = document.getElementById(e.dataset.replyId);
      respuesta.innerHTML = `<p class="me-2">${responseJson.contenido}</p>
                    <p class="text-secondary">${Date.parse(responseJson.fecha)}</p>
                    `;
    })
    .catch((error) => {});
}
