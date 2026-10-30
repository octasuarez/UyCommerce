using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Domain.Exceptions;
using UYCommerce.ViewModels;

namespace UYCommerce.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) => this._authService = authService;

        [HttpGet]
        [Route("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var usuario = await _authService.AuthenticateUsuarioAsync(loginModel.Email.ToLower(), loginModel.Password);
                    if (usuario is not null)
                    {
                        var claims = new List<Claim>{
                            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                            new(ClaimTypes.Name, usuario.Email!),
                            new("FullName", usuario.Nombre!),
                            new(ClaimTypes.Role, usuario.Rol!.Nombre!),
                            new("CarritoId", usuario.Carrito.Id.ToString()),
                        };
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            AllowRefresh = true,
                            IsPersistent = loginModel.RememberMe,
                        };
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);
                        HttpContext.Session.SetString("cartItems", usuario.Carrito.Productos.Count.ToString());
                        return (usuario.Rol.Nombre == "Admin") ? RedirectToAction("Dashboard", "Admin") : RedirectToAction("Index", "Home");
                    }
                }
                catch (NotFoundException)
                {
                }
                catch (Exception)
                {
                }
            }
            ViewBag.msjLogin = "Usuario o contraseña incorrectos.";
            return View("Login", loginModel);
        }

        [HttpGet]
        [Route("Register")]
        public IActionResult AgregarUsuario()
        {
            return View("Register");
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            if (ModelState.IsValid && registerModel.Password == registerModel.ConfirmPassword)
            {
                try
                {
                    var success = await _authService.RegisterUsuarioAsync(registerModel.Nombre, registerModel.Email, registerModel.Password);
                    if (success) return await Login(new LoginModel { Email = registerModel.Email, Password = registerModel.Password});
                }
                catch (Exception)
                {
                }
            }
            return View("Register", registerModel);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.SetString("cartItems", "");
            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        [Route("RecuperarPwd")]
        public IActionResult RecuperarPassword()
        {
            return View("RecuperarPwd");
        }

        [HttpPost]
        [Route("RecuperarPassword")]
        public async Task<IActionResult> RecuperarPassword(string email)
        {
            try
            {
                var request = HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}/VerificarCodigoRecuperacion";
                await _authService.CreateRecuperacionPassword(email, baseUrl);
            }
            catch (Exception)
            {
            }
            return View("RecuperarPwd");
        }

        [HttpGet]
        [Route("/VerificarCodigoRecuperacion/{codigo}")]
        public async Task<IActionResult> VerificarCodigoRecuperacion(string codigo)
        {
            try
            {
                var codigoIsValid = await _authService.VerifyCodigoRecuperacion(codigo);
                if(codigoIsValid) return View("CambiarPwd", new PasswordRecuperacionModel { Codigo = codigo });
            }
            catch (Exception){}
            return View("NotFound");
        }

        [HttpPost]
        public async Task<IActionResult> CambiarPasswordRecuperacion(PasswordRecuperacionModel request)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _authService.ChangePasswordUsuario(request.Codigo, request.Email, request.Password);
                    return Redirect("/Login");
                }
                catch (Exception)
                {
                    return View("NotFound");
                }
            }
            return View("CambiarPwd", request);
        }
    }
}

