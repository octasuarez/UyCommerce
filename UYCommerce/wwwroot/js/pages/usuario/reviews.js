async function CrearReview(e) {
  let form = new FormData(document.getElementById(e.dataset.formId));
  let body = Object.fromEntries(form);
  const response = await fetch("/Usuario/AgregarReview", {
    headers: { "Content-Type": "application/json" },
    method: "POST",
    credentials: "include",
    body: JSON.stringify(body),
  })
    .then((response) => {
      if (response.ok) {
        location.reload();
      }
    })
    .catch((error) => {});
}

async function UpdateReview(e) {
  let form = new FormData(document.getElementById(e.dataset.formId));
  let body = Object.fromEntries(form);
  const response = await fetch("/Usuario/UpdateReview", {
    headers: { "Content-Type": "application/json" },
    method: "POST",
    credentials: "include",
    body: JSON.stringify(body),
  })
    .then((response) => {
      if (response.ok) {
        location.reload();
      }
    })
    .catch((error) => {});
}
