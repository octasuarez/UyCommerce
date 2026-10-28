using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IAuthService
    {
        Task<Usuario?> AuthenticateUsuario(string email, string password);
    }
}
