// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function AgregarAFavoritos(e) {

    var element = e.firstElementChild.classList;

    var skuId = e.parentElement.getAttribute('id');

    console.log(skuId);

    var data = { "skuId": skuId }

    $.ajax({
        type: 'POST',
        url: '/Producto/AgregarAFavoritos',
        dataType: 'json',
        data: { skuId: skuId },
        success: function (response) {

            if (response.msj === "agregado") {

                element.replace('bi-heart', 'bi-heart-fill');
                element.add('text-secondary', 'text-danger')

            } else {

                element.replace('bi-heart-fill', 'bi-heart')
                element.replace('text-danger', 'text-secondary')
            }
        },
        error: function () {
            window.location.href = '/Login';
        }
    })

}