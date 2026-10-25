async function EliminarReview(e) {
  let reviewId = e.dataset.reviewId;
  const response = await fetch("/Admin/EliminarReview", {
    headers: { "Content-Type": "application/json" },
    credentials: "include",
    method: "POST",
    body: JSON.stringify(reviewId),
  }).then((response) => {
    if (response.ok) {
      location.reload();
    } else {
    }
  });
}
