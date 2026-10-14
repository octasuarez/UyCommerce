using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UyCommerce.Application.DTOs;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Domain.Exceptions;

namespace UYCommerce.Controllers
{
    public class AtributoController : Controller
    {
        private readonly IAtributoService _atributoService;
        public AtributoController(IAtributoService atributoService) => this._atributoService = atributoService;
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CrearAtributo()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetAtributosJson()
        {
            return Ok(await _atributoService.GetAtributosAsync());
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> CrearAtributo([FromBody] string nombre)
        {
            if (string.IsNullOrEmpty(nombre)) 
                return BadRequest("Atributo invalido.");
            try
            {
                await _atributoService.AddAtributoAsync(nombre);
            }
            catch (ArgumentOutOfRangeException e)
            {
                return BadRequest(e.Message);
            }
            catch(Exception e)
            {
                return StatusCode(500, "No se pudo dar de alta el atributo.");
            }
            return Ok("Atributo creado exitosamente.");
        }

        [HttpPost]
        public async Task<IActionResult> AgregarValor([FromBody] AgregarValorDTO atributoValor)
        {

            if (string.IsNullOrEmpty(atributoValor.Valor) || atributoValor.atributoId < 0)
                return BadRequest("escriba un valor");
            try
            {
                await _atributoService.AddAtributoValorAsync(atributoValor);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok("Se agrego el valor exitosamente.");
        }


    }
}

