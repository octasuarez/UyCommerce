async function DeleteImage(e) {
  let imageId = e.dataset.imageId;

  $.ajax({
    url: "/Admin/DeleteCarouselImage",
    method: "post",
    data: { imageId: imageId },
    success: () => {
      location.reload();
    },
  });
}

async function AddImage(e) {
  if (e.files.length > 0) {
    let formData = new FormData();
    var file = e.files[0];
    formData.append("file", file);

    $.ajax({
      url: "/Admin/CreateCarouselImage",
      method: "post",
      data: formData,
      processData: false,
      contentType: false,
      success: () => {
        location.reload();
      },
      error: (response) => {
        ShowToast("error", response);
      },
    });
  }
}

async function ChangeImage(e) {
  if (e.files.length > 0) {
    let formData = new FormData();
    var file = e.files[0];
    formData.append("file", file);
    formData.append("imageId", e.dataset.imageId);

    $.ajax({
      url: "/Admin/ChangeCarouselImage",
      method: "post",
      data: formData,
      processData: false,
      contentType: false,
      success: () => {
        location.reload();
      },
      error: (response) => {
        ShowToast("error", response);
      },
    });
  }
}
