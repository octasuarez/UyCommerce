async function Contacto() {
  let form = new FormData(document.getElementById("contactForm"));
  let body = Object.fromEntries(form);

  let btn = document.getElementById("send");
  btn.innerHTML = `<div class="spinner-grow" role="status">
                                    <span class="visually-hidden">Loading...</span>
                                </div>`;

  const response = fetch("/Home/Contacto", {
    headers: { "Content-Type": "application/json", Accept: "application/json" },
    credentials: "include",
    method: "POST",
    body: JSON.stringify(body),
  })
    .then((response) => {
      if (response.ok) {
        alert("se envió correctamente");
        btn.innerHTML = `<p class="text-success">Enviado</p>`;
        document.getElementById("errors").innerText = "";
      } else {
        btn.innerHTML = `<button type="button" onclick="Contacto()" class="btn btn-dark">Enviar</button>`;
        return response.json();
      }
    })
    .then((message) => {
      if (message != null) {
        document.getElementById("errors").innerText = message;
      } else {
        document.getElementById("Asunto").value = "";
        document.getElementById("Nombre").value = "";
        document.getElementById("Mensaje").value = "";
      }
    });
}
