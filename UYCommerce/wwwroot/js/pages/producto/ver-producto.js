
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

    let key = "/" + detalles.dataset.skuKey + "/";

    if (e.id == 0) {

        key += `${e.value}`;

    } else {

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