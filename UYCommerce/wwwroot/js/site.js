
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

function ShowToast(title, msg) {

    let toast = `<div id="toast" class="toast-container position-fixed bottom-0 start-50 translate-middle-x p-3">
                        <div id="liveToast" class="toast fadein100 show" role="alert" aria-live="assertive" aria-atomic="true">
                            <div class="toast-header">
                                <img src="" class="rounded me-2" alt="">
                                <strong class="me-auto">${title}</strong>
                                <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
                            </div>
                            <div class="toast-body">
                                ${msg}
                            </div>
                        </div>
                    </div>`

    document.body.innerHTML += toast;

    setTimeout(() => {
        document.getElementById('liveToast').classList.remove('fadein100');
        document.getElementById('liveToast').classList.add('fadeOut');
        setTimeout(() => {
            document.getElementById('toast').remove();
        }, 1000)
    }, 3000)

}