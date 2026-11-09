using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IAuthService
    {
        Task<Usuario> AuthenticateUsuario(string email, string password);
        Task<bool> RegisterUsuario(string name, string email, string password);
        Task CreateRecuperacionPassword(string email, string url);
        Task<bool> VerifyCodigoRecuperacion(string codigo);
        Task ChangePasswordUsuario(string codigo, string email, string password);
    }
}
