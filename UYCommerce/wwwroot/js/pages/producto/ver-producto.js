
function cambioAtributo(e) {

    let loading =
    `<div class="overlay">
    <div class="overlay bg-light fadein50" style="z-index:100;"></div>
            <div class="d-flex justify-content-center h-100 overlay">
            <div class="spinner-border mx-auto my-auto" role="status">
                    <span class="visually-hidden">Loading...</span>
            </div>
        </div>
    </div>`;

    let detalles = document.getElementById('detalles');
    detalles.innerHTML += loading;

    let key = "/" + detalles.dataset.skuKey;

    if (e.id == 0) {

        key += `_${e.value}`;

    } else {
        key += "_";
        let selects = e.parentElement.querySelectorAll('select');
        for (i = 0; i < selects.length; i++) {
            key += `${selects[i].value}-`;
        }

        key = key.slice(0,-1);
    }

    let url = key.toLowerCase();
    window.location.href = url;
}

function AgregarProductoAlCarrito(e) {


    let qty = document.getElementById("qty");

    let loading =
        `<div class="overlay">
    <div class="overlay bg-light fadein50" style="z-index:100;"></div>
    <div class="d-flex justify-content-center h-100 overlay">
    <div class="spinner-border mx-auto my-auto" role="status">
            <span class="visually-hidden">Loading...</span>
    </div>
    </div>
    </div>`;

    let detalles = document.getElementById('detalles');
    detalles.innerHTML += loading;



    $.ajax({
        type: 'POST',
        url: "/Carrito/AgregarProductoAlCarrito",
        data: { skuId: e.value, quantity: qty.value},
        success: function (response) {
            location.reload();
        }

    })
}

function AgregarPregunta() {

    var preguntaTxt = document.getElementById('txtPregunta');

    let detalles = document.getElementById('detalles');
    let productoId = detalles.dataset.productoId;
    var formData =
    {
        Contenido: preguntaTxt.value, ProductoId: productoId
    };

    $.ajax({

        type: 'POST',
        url: '/Usuario/AgregarPregunta',
        data: formData,
        success: function (response) {
            ShowPregunta(response.pregunta, response.count)
        },
        error: function (response) {
        }
    })
  }

function ShowPregunta(pregunta,count){

    preguntas = document.getElementById('Preguntas');
    var preg_existentes = count > 1 ? preguntas.innerHTML : "";
    preguntas.innerHTML =
        `<h5>${pregunta.nombreUsuario}</h5>
        <p>${pregunta.contenido}</p>
        <p class="fs-6 fw-lighter"> Publicado el ${pregunta.fecha}</p>
        ${preg_existentes}
        `;

    document.getElementById('txtPregunta').value = "";
    document.getElementById('pcount').innerHTML = `${count} preguntas`;
}

function cambiarImagen(e) {

    var imgmain = document.getElementById("imgmain");
    imgmain.src = e.src;
    document.querySelector('.active-img').classList.remove('active-img');
    e.classList.add('active-img')
}

document.addEventListener("DOMContentLoaded", function () {
    const scrollImages = document.querySelector(".scroll-images");
    const scrollLength = scrollImages.scrollWidth - scrollImages.clientWidth;
    const leftButton = document.querySelector(".left");
    const rightButton = document.querySelector(".right");

    function checkScroll() {
        const currentScroll = scrollImages.scrollLeft;
        if (currentScroll === 0) {
            leftButton.setAttribute("disabled", "true");
            rightButton.removeAttribute("disabled");
        } else if (currentScroll === scrollLength) {
            rightButton.setAttribute("disabled", "true");
            leftButton.removeAttribute("disabled");
        } else {
            leftButton.removeAttribute("disabled");
            rightButton.removeAttribute("disabled");
        }
    }

    scrollImages.addEventListener("scroll", checkScroll);
    window.addEventListener("resize", checkScroll);
    checkScroll();

    function leftScroll() {
        scrollImages.scrollBy({
            left: -400,
            behavior: "smooth"
        });
    }

    function rightScroll() {
        scrollImages.scrollBy({
            left: 400,
            behavior: "smooth"
        });
    }

    leftButton.addEventListener("click", leftScroll);
    rightButton.addEventListener("click", rightScroll);
});