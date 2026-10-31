using System;
using System.Collections.Generic;
using System.Text;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    internal interface IPagosService
    {
        Task<CreateOrderResponse> CreateOrder(int userId);
    }
}
