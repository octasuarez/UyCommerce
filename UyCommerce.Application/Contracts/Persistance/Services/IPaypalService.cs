using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.DTOs.ExternalServices.Paypal;

namespace UyCommerce.Application.Contracts.Persistance.Services
{
    public interface IPaypalService
    {
        Task<CreateOrderResponse?> CreateOrder(List<Item> items, string total);
        Task<CaptureOrderResponse?> CaptureOrder(string orderId);
    }
}
