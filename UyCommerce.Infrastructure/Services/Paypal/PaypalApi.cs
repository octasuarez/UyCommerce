using System;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http.Json;
using System.Text.Json;
using UyCommerce.Application.DTOs.ExternalServices.Paypal;
using UyCommerce.Application.Contracts.Persistance.Services;

namespace UyCommerce.Infrastructure.Services.Paypal
{
    public class PaypalAPI : IPaypalService
    {
        private readonly string? _clientId;
        private readonly string? _secretKey;
        private readonly string? _baseUrl;
        private TokenResponse? _token;
        private DateTime? _expires;
        private static HttpClient _client = new();

        public PaypalAPI(string? clientId, string? secretKey, string? baseUrl)
        {
            this._clientId = clientId;
            this._secretKey = secretKey;
            this._baseUrl = baseUrl;
        }

        private async Task<string?> GenerateAccessToken()
        {
            if(_token != null && _expires > DateTime.Now)
            {
                return _token.access_token;
            }
            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_secretKey}"));
            var content = new Dictionary<string, string> {
                { "grant_type", "client_credentials" }
            };
            var url = $"{_baseUrl}/v1/oauth2/token";
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post,
                Headers =
                {
                    Authorization = new AuthenticationHeaderValue("Basic",auth)
                },
                Content = new FormUrlEncodedContent(content)
            };

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<TokenResponse>(data);
            _token = token;
            _expires = DateTime.Now.AddSeconds((int)token.expires_in);
            return token?.access_token;
        }

        
        public async Task<CreateOrderResponse?> CreateOrder(List<Item> items, string total)
        {
            var token = await GenerateAccessToken();
            var url = $"{_baseUrl}/v2/checkout/orders";
            var requestBody = new CreateOrderRequest
            {
                intent = "CAPTURE",
                purchase_units = new List<PurchaseUnit>
                {
                    new PurchaseUnit
                    {
                        items = items,
                        amount = new Amount
                        {
                            currency_code = "USD",
                            value = total,
                            breakdown = new Breakdown
                            {
                                item_total = new ItemTotal
                                {
                                    currency_code = "USD",
                                    value = total
                                }
                            }
                        }
                    }
                }
            };

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post,
                Headers =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", token),
                },
                Content = JsonContent.Create(requestBody),
            };

            request.Headers.Add("Prefer", "return=representation");
            request.Headers.Add("PayPal-Request-Id", Guid.NewGuid().ToString());
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CreateOrderResponse>(data);
        }


        public async Task<CaptureOrderResponse?> CaptureOrder(string orderId)
        {
            var token = await GenerateAccessToken();
            var url = $"{_baseUrl}/v2/checkout/orders/{orderId}/capture";
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post,
                Headers =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", token),
                },
                Content = JsonContent.Create(new EmptyResult())
            };
            request.Headers.Add("Prefer", "return=representation");
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CaptureOrderResponse>(data);
        }

    }
}


