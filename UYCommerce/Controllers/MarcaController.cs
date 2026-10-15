using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UyCommerce.Infrastructure.Persistance;
using UyCommerce.Domain.Entities;
using UyCommerce.Domain.Exceptions;
using UyCommerce.Application.Contracts.ServicesAbstractions;

namespace UYCommerce.Controllers
{
    public class MarcaController : Controller
    {
        private readonly IMarcaService _marcaService;

        public MarcaController(IMarcaService marcaService)
        {
            _marcaService = marcaService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("admin/marcas")]
        public async Task<IActionResult> GetMarcas()
        {
            var marcas = await _marcaService.GetMarcasWithProductosAsync();
            return View("../Admin/GetMarcas", marcas);
        }

        [HttpGet]
        public async Task<IActionResult> GetMarcasApi()
        {
            var marcas = _marcaService.GetMarcasAsync();
            return Ok(marcas);
        }

        [HttpGet]
        public IActionResult CreateMarca()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> CreateMarca(string nombre)
        {
            try
            {
                await _marcaService.AddMarcaAsync(nombre);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok();
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> DeleteMarca(int marcaId) {

            try
            {
                await _marcaService.DeleteMarcaAsync(marcaId);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            return Ok();
        }

        [HttpGet]
        public async Task<Marca?> GetMarcaById(int id) {

            var marca = await _marcaService.GetMarcaAndProductosByIdAsync(id);
            return marca;
        }
    }
}

