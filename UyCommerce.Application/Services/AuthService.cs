using Konscious.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Application.Contracts.ServicesAbstractions;
using UyCommerce.Domain.Entities;
using UyCommerce.Domain.Exceptions;

namespace UyCommerce.Application.Services
{
    public class AuthService : IAuthService
    {
        public IUsuarioRepository _usuarioRepository { get; }
        public AuthService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Usuario?> AuthenticateUsuario(string email, string password)
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
    }
}
