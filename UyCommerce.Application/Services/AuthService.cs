using Konscious.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
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
        private readonly IConfiguration _configuration;

        public IUsuarioRepository _usuarioRepository { get; }
        public AuthService(IUsuarioRepository usuarioRepository, IEmailService emailService, IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _emailService = emailService;
            this._configuration = configuration;
        }

        public async Task<Usuario?> AuthenticateUsuarioAsync(string email, string password)
        {
            var usuario = await _usuarioRepository.GetUsuarioByEmailAsync(email)
                ?? throw new NotFoundException(nameof(Usuario), email);
            if (!usuario.Activo) throw new Exception("Usuario inhabilitado");
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
            foreach (var param in _params)
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
                ?? throw new NotFoundException(nameof(Rol), "User");
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

        public async Task CreateRecuperacionPassword(string email, string url)
        {
            var usuario = await _usuarioRepository.GetUsuarioByEmailAsync(email)
                ?? throw new NotFoundException(nameof(Usuario), email);
            usuario.Activo = false;
            var codigoDb = await _usuarioRepository.GetCodigoRecuperacionUsuarioAsync(usuario.Id);
            if (codigoDb is not null)
            {
                codigoDb.FechaExpiracion = DateTime.Now.AddDays(-1);
                await _usuarioRepository.UpdateCodigoRecuperacion(codigoDb);
            }
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            var codigoRecuperacion = HashSHA256(token);
            Codigo codigo = new(codigoRecuperacion, usuario.Id, DateTime.UtcNow.AddMinutes(5));
            await _usuarioRepository.CreateCodigoRecuperacion(codigo);
            await _usuarioRepository.UpdateAsync(usuario);
            await SendRecuperacionPasswordEmail(email, url, token);
        }

        private async Task SendRecuperacionPasswordEmail(string email, string url, string token)
        {
            string mailSubject = _configuration["RecuperacionPasswordAsunto"]
                ?? throw new Exception("Asunto de correo de recuperacion sin definir.");
            string mailBody = _configuration["RecuperacionPasswordBody"]
                ?? throw new Exception("Body de correo de recuperacion sin definir.");
            mailBody = mailBody.Replace("#URL#", url).Replace("#TOKEN#", token);
            await _emailService.SendEmail(email, mailSubject, mailBody);
        }

        private static string HashSHA256(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash).ToLower();
        }

        public async Task<bool> VerifyCodigoRecuperacion(string codigo)
        {
            var codigoHash = HashSHA256(codigo);
            var codigoDb = await _usuarioRepository.GetCodigoRecuperacionByCodigoAsync(codigoHash)
                ?? throw new NotFoundException(nameof(Codigo), codigo);
            return codigoDb.FechaExpiracion >= DateTime.UtcNow;
        }

        public async Task ChangePasswordUsuario(string codigo, string email, string password)
        {
            var isCodigoValid = await VerifyCodigoRecuperacion(codigo);
            if (!isCodigoValid) throw new Exception("Codigo recuperacion invalido.");
            var usuario = await _usuarioRepository.GetUsuarioByEmailAsync(email)
                ?? throw new NotFoundException(nameof(Usuario), email);
            var codigoDb = await _usuarioRepository.GetCodigoRecuperacionUsuarioAsync(usuario.Id)
                ?? throw new NotFoundException(nameof(Codigo), codigo);
            var codigosMatch = FixedTimeEquals(HashSHA256(codigo), codigoDb.CodigoRecuperacion);
            if (codigosMatch)
            {
                usuario.Activo = true;
                usuario.Password = await HashPassword(password);
                codigoDb.FechaExpiracion = DateTime.UtcNow.AddDays(-1);
                await _usuarioRepository.UpdateAsync(usuario);
                await _usuarioRepository.UpdateCodigoRecuperacion(codigoDb);
                await _emailService.SendEmail(email, _configuration["CambioPasswordAsunto"]!, _configuration["CambioPasswordBody"]!);
            }
        }

        private static bool FixedTimeEquals(string data, string expected)
        {
            var codigoBytes = Encoding.UTF8.GetBytes(data);
            var matchBytes = Encoding.UTF8.GetBytes(expected);
            return CryptographicOperations.FixedTimeEquals(codigoBytes, matchBytes);
        }
    }
}
