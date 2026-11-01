using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.DTOs.ExternalServices.Paypal;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IPagosService
    {
        Task<CreateOrderResponse> CreateOrderPaypal(int userId);
        Task<CaptureOrderResponse> CaptureOrderPaypal(string ordenId, int userId);
    }
}
