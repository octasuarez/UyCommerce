using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Application.DTOs;
using UyCommerce.Domain.Entities;
using UyCommerce.Domain.Exceptions;

namespace UyCommerce.Application.Services
{
    public class PreguntaService : IPreguntaService
    {
        private readonly IPreguntaRepository _preguntaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IProductoRepository _productoRepository;

        public PreguntaService(IPreguntaRepository preguntaRepository
            , IUsuarioRepository usuarioRepository
            , IProductoRepository productoRepository)
        {
            _preguntaRepository = preguntaRepository;
            this._usuarioRepository = usuarioRepository;
            this._productoRepository = productoRepository;
        }

        public async Task<Pregunta> GetPreguntaById(int id)
        {
            return await _preguntaRepository.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(Pregunta), id);
        }

        public async Task<Pregunta> GetPreguntaByRespuestaId(int respuestaId)
        {
            return await _preguntaRepository.GetPreguntaByRespuestaId(respuestaId)
                ?? throw new NotFoundException(nameof(Pregunta),respuestaId);
        }

        public async Task<List<Pregunta>> GetPreguntas(bool? answered)
        {
            return await _preguntaRepository.GetPreguntas(answered);
        }

        public async Task UpdatePreguntaRespuesta(RespuestaDTO respuesta)
        {
            if(respuesta.PreguntaId < 0 || string.IsNullOrEmpty(respuesta.Contenido))
            {
                throw new Exception("Respuesta invalida.");
            }
            var pregunta = await GetPreguntaById(respuesta.PreguntaId);
            pregunta.Respuesta = new Respuesta()
            {
                Contenido = respuesta.Contenido,
                Fecha = DateTime.Now,
                PreguntaId = pregunta.Id
            };
            await _preguntaRepository.UpdateAsync(pregunta);
        }
        public async Task CreatePregunta(CreatePreguntaDTO preguntaDto, int userId)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(userId)
                ?? throw new NotFoundException(nameof(Usuario), userId);
            var producto = await _productoRepository.GetByIdAsync(preguntaDto.ProductoId)
                ?? throw new NotFoundException(nameof(Producto), preguntaDto.ProductoId);
            Pregunta pregunta = new()
            {
                Contenido = preguntaDto.Contenido,
                Usuario = usuario,
                Producto = producto,
                Fecha = preguntaDto.Fecha,
            };
            await _preguntaRepository.CreateAsync(pregunta);
        }
    }
}
