function AgregarFiltro(e) {
  var atr = e.getAttribute("data-value");
  var url = window.location.href;
  url += url.indexOf("?") !== -1 ? `&filtros=${atr}` : `?filtros=${atr}`;
  window.location.href = url;
}
function QuitarFiltro(e) {
  var url = window.location.href;
  if (url.indexOf("?") !== -1) {
    var urlParts = url.split("?"),
      baseUrl = urlParts[0],
      queryParams = urlParts[1].split("&"),
      filtro = encodeURI(e.getAttribute("data-value")),
      param;
    for (i = queryParams.length - 1; i >= 0; i -= 1) {
      param = queryParams[i].split("=")[1];
      if (param.toLowerCase() === filtro) {
        queryParams.splice(i, 1);
      }
    }
    if (queryParams.length) baseUrl = baseUrl + "?" + queryParams.join("&");
    window.location.href = baseUrl;
  }
}
function OrdenarProductos(e) {
  var url = window.location.href,
    ordenIdx = url.indexOf("orden");
  if (ordenIdx !== -1) {
    var param = url.slice(ordenIdx, ordenIdx + 7);
    url = url.replace(param, `orden=${e}`);
  } else if (
    url.indexOf("filtros") !== -1 ||
    url.indexOf("query") !== -1 ||
    url.indexOf("pag") !== -1
  ) {
    url += `&orden=${e}`;
  } else {
    url += `?orden=${e}`;
  }
  window.location.href = url;
}

document.querySelectorAll(".page-link").forEach((a) =>
  a.addEventListener("click", function (e) {
    const url = new URLSearchParams(location.search);
    url.set("pag", e.target.dataset.pag);
    var href = `${location.pathname}?${url.toString()}`;
    e.target.setAttribute("href", href);
  }),
);
