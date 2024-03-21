using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UYCommerce.Data;
using UYCommerce.Models;
using UYCommerce.Services;

namespace UYCommerce.Controllers
{
    public class AuthController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly ShopContext _context;

        public AuthController(ShopContext context, IEmailService emailService)
        {
            _emailService = emailService;
            _context = context;
        }

        [HttpGet]
        [Route("Login")]
        public IActionResult Login(string? returnUrl)
        {
            return View();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {

            if (ModelState.IsValid)
            {

                var usuario = await _context.Usuarios
                    .Include(u => u.Rol)
                    .Include(u => u.Carrito).ThenInclude(c => c.Productos)
                    .FirstOrDefaultAsync(u => u.Email!.ToLower() == loginModel.Email!.ToLower());


                if (usuario is null)
                {
                    ViewBag.msjLogin = "No existe ese usuario";
                    return View("Login", loginModel);
                }
                if (usuario.Password != loginModel.Password)
                {
                    ViewBag.msjLogin = "Contraseña incorrecta";
                    return View("Login", loginModel);
                }
                if (usuario.Activo == false)
                {
                    ViewBag.msjLogin = "Usuario inactivo";
                    return View("Login", loginModel);
                }


                var claims = new List<Claim>
                    {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Email!),
                    new Claim("FullName", usuario.Nombre!),
                    new Claim(ClaimTypes.Role, usuario.Rol!.Nombre!),
                    new Claim("CarritoId", usuario.Carrito.Id.ToString()),
                    };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);


                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    // Refreshing the authentication session should be allowed.

                    //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    // The time at which the authentication ticket expires. A 
                    // value set here overrides the ExpireTimeSpan option of 
                    // CookieAuthenticationOptions set with AddCookie.

                    IsPersistent = loginModel.RememberMe,
                    // Whether the authentication session is persisted across 
                    // multiple requests. When used with cookies, controls
                    // whether the cookie's lifetime is absolute (matching the
                    // lifetime of the authentication ticket) or session-based.

                    //IssuedUtc = <DateTimeOffset>,
                    // The time at which the authentication ticket was issued.

                    //RedirectUri = <string>
                    // The full path or absolute URI to be used as an http 
                    // redirect response value.
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                HttpContext.Session.SetString("cartItems", usuario.Carrito.Productos!.Count.ToString());

                return RedirectToAction("Index", "Home");
            }

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
        public async Task<IActionResult> AgregarUsuario(RegisterModel registerModel)
        {

            if (ModelState.IsValid)
            {
                var usuarioExiste = _context.Usuarios.FirstOrDefault(u => u.Email == registerModel.Email);
                if (usuarioExiste is not null)
                    return View("Register");

                var rol = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == "User");

                Usuario usuario = new()
                {
                    Nombre = registerModel.Nombre,
                    Email = registerModel.Email.ToLower(),
                    Password = registerModel.Password,
                    Rol = rol
                };

                await _context.Usuarios.AddAsync(usuario);
                await _context.SaveChangesAsync();

                LoginModel loginModel = new() { Email = registerModel.Email, Password = registerModel.Password };
                return await Login(loginModel);

                //return RedirectToAction("Index", "Home");
            }

            return View("Register");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync(
       CookieAuthenticationDefaults.AuthenticationScheme);

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

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

            if (usuario != null)
            {
                var codigoExiste = await _context.Codigos.FirstOrDefaultAsync(c => c.UsuarioId == usuario.Id);
                if (usuario.Activo == true)
                {
                    usuario.Activo = false;
                    _context.Usuarios.Update(usuario);
                }

                Codigo? codigo = new();

                if (codigoExiste is not null)
                {
                    if (codigoExiste.FechaExpiracion > DateTime.Now)
                    {
                        codigo = codigoExiste;
                    }
                    else
                    {
                        codigoExiste = new Codigo(Guid.NewGuid().ToString()[..5], usuario.Id);
                        _context.Codigos.Update(codigoExiste);
                    }
                }
                else
                {
                    codigo = new Codigo(Guid.NewGuid().ToString()[..6], usuario.Id);
                    await _context.Codigos.AddAsync(codigo);
                }

                var content = "Tu codigo es: " + codigo.CodigoRecuperacion;
                Message message = new(email, "UyCommerce Recuperacion Contraseña", content);

                _emailService.SendEmail(message);

                await _context.SaveChangesAsync();

                return View("CodigoRecuperacion", usuario.Id);
            }

            return View("RecuperarPwd");
        }

        [HttpPost]
        public async Task<IActionResult> VerificarCodigoRecuperacion(string codigo, int usrId)
        {

            var codigoDB = await _context.Codigos.FirstOrDefaultAsync(c => c.CodigoRecuperacion == codigo);

            if (codigoDB is null || codigoDB.UsuarioId != usrId)
                return View("NotFound");

            return View("CambiarPwd", new PasswordRecuperacionModel { Codigo = codigoDB.CodigoRecuperacion });
        }

        [HttpPost]
        public async Task<IActionResult> CambiarPasswordRecuperacion(PasswordRecuperacionModel request)
        {
            if (ModelState.IsValid)
            {
                var codigoDB = await _context.Codigos.FirstOrDefaultAsync(c => c.CodigoRecuperacion == request.Codigo);

                if (codigoDB is null)
                {
                    ViewBag.msg = "No existe ese codigo";
                    return View("CambiarPwd");
                }

                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == codigoDB.UsuarioId);

                if (usuario is not null)
                {
                    usuario.Password = request.Password;
                    usuario.Activo = true;
                    _context.Update(usuario);
                    _context.Remove(codigoDB);
                    await _context.SaveChangesAsync();
                    return Redirect("/Login");
                }
                return View("NotFound");
            }
            return View("CambiarPwd", request);
        }
    }
}

