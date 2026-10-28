using Konscious.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using UyCommerce.Application.Common.Enums;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Application.Contracts.Persistance.Services;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Domain.Entities;
using UyCommerce.Domain.Exceptions;

namespace UyCommerce.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IEmailService _emailService;

        public IUsuarioRepository _usuarioRepository { get; }
        public AuthService(IUsuarioRepository usuarioRepository, IEmailService emailService)
        {
            _usuarioRepository = usuarioRepository;
            _emailService = emailService;
        }

        public async Task<Usuario?> AuthenticateUsuarioAsync(string email, string password)
        {
            var usuario = await _usuarioRepository.GetUsuarioByEmailAsync(email)
                ?? throw new NotFoundException(nameof(Usuario),email);
            if(!usuario.Activo) throw new Exception("Usuario inhabilitado");
            return await VerifyPassword(password, usuario.Password) == true ? usuario : null;
        }

        private async Task<bool> VerifyPassword(string password, string phc)
        {
            string[] phcParts = phc.Split('$');
            string algorithm = phcParts[0];
            string version = phcParts[1];
            string[] _params = phcParts[2].Split(',');
            byte[] salt = Convert.FromBase64String(PadBase64String(phcParts[3]));
            byte[] expectedHash = Convert.FromBase64String(PadBase64String(phcParts[4]));
            int m = 0, t = 0, p = 0;
            foreach(var param in _params)
            {
                var paramValue = param.Split('=');
                m = paramValue[0] == "m" ? int.Parse(paramValue[1]) : m;
                t = paramValue[0] == "t" ? int.Parse(paramValue[1]) : t;
                p = paramValue[0] == "p" ? int.Parse(paramValue[1]) : p;
            }
            var hash = await Argon2idHash(password, salt, m, t, p);
            return CryptographicOperations.FixedTimeEquals(hash, expectedHash);
        }

        public static async Task<string> HashPassword(string password)
        {
            int m = 19456, t = 2, p = 1;
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hash = await Argon2idHash(password, salt, m, t, p);
            string b64(byte[] b) => Convert.ToBase64String(b).TrimEnd('=');
            return $"argon2id$v=19$m={m},t={t},p={p}${b64(salt)}${b64(hash)}";
        }

        private static async Task<byte[]> Argon2idHash(string password, byte[] salt, int memorySize, int iterations, int degreeOfParallelism)
        {
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                MemorySize = memorySize,
                Iterations = iterations,
                DegreeOfParallelism = degreeOfParallelism
            };
            return await argon2.GetBytesAsync(32);
        }

        private static string PadBase64String(string b64)
        {
            return b64.PadRight(b64.Length + (4 - b64.Length % 4) % 4, '=');
        }

        public async Task<bool> RegisterUsuarioAsync(string name, string email, string password)
        {
            var usuario = await _usuarioRepository.GetUsuarioByEmailAsync(email);
            if (usuario is not null) throw new Exception("Correo electronico ya registrado");
            var rolUsuario = await _usuarioRepository.GetRolByNombre("User")
                ?? throw new NotFoundException(nameof(Rol),"User");
            Usuario newUsuario = new()
            {
                Nombre = name,
                Email = email,
                Activo = true,
                Rol = rolUsuario,
                Password = await HashPassword(password)
            };
            return await _usuarioRepository.CreateAsync(newUsuario) > 0;
        }

        public async Task SendRecuperacionPasswordEmail(string email)
        {
            var usuario = await _usuarioRepository.GetUsuarioByEmailAsync(email)
                ?? throw new NotFoundException(nameof(Usuario), email);
            usuario.Activo = false;
            var codigoDb = await _usuarioRepository.GetCodigoRecuperacionUsuarioAsync(usuario.Id);
            if(codigoDb is not null)
            {
                codigoDb.FechaExpiracion = DateTime.Now.AddDays(-1);
                await _usuarioRepository.UpdateCodigoRecuperacion(codigoDb);
            }
            Codigo codigo = new(Guid.NewGuid().ToString()[..6], usuario.Id, DateTime.Now.AddMinutes(2));
            await _usuarioRepository.CreateCodigoRecuperacion(codigo);
            await _usuarioRepository.UpdateAsync(usuario);
            string mailAsunto = "UyCommerce Recuperacion Contraseña";
            string mailBody = "Tu codigo es: " + codigo.CodigoRecuperacion;
            await _emailService.SendEmail(email, mailAsunto, mailBody);
        }
    }
}
