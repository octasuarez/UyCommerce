﻿using System;
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

namespace UYCommerce.Controllers
{
    public class AuthController : Controller
    {
        private readonly ShopContext _context;

        public AuthController(ShopContext context)
        {

            _context = context;
        }

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

                var usuario = await _context.Usuarios
                    .Include(u => u.Rol)
                    .Include(u => u.Carrito)
                    .FirstOrDefaultAsync(u => u.Email == loginModel.Email!.ToLower());

                if (usuario is not null && usuario.Password == loginModel.Password)
                {

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
                        //AllowRefresh = <bool>,
                        // Refreshing the authentication session should be allowed.

                        //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                        // The time at which the authentication ticket expires. A 
                        // value set here overrides the ExpireTimeSpan option of 
                        // CookieAuthenticationOptions set with AddCookie.

                        //IsPersistent = true,
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

                    if (loginModel.RememberMe)
                    {
                        authProperties.IsPersistent = true;
                    }

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);


                    return RedirectToAction("Index", "Home");
                }
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

                var rol = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Cliente");

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

            return RedirectToAction("Login", "Auth");
        }
    }
}

