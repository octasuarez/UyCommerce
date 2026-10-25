function ViewPassword(e) {
  var input = document.getElementById("password");
  if (input.getAttribute("type") === "password") {
    input.setAttribute("type", "text");
    e.classList.remove("bi-eye-slash");
    e.classList.add("bi-eye");
  } else {
    input.setAttribute("type", "password");
    e.classList.remove("bi-eye");
    e.classList.add("bi-eye-slash");
  }
}
