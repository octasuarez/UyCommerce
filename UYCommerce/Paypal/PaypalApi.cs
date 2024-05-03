using System;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http.Json;
using System.Text.Json;
using UYCommerce.Models;

namespace UYCommerce.Paypal
{
    public class PaypalAPI
    {
        private readonly string? clientId;
        private readonly string? secretKey;
        private readonly string? baseUrl;
        private static HttpClient client = new HttpClient();

        public PaypalAPI(string? clientId, string? secretKey, string? baseUrl)
        {
            this.clientId = clientId;
            this.secretKey = secretKey;
            this.baseUrl = baseUrl;
        }

        public async Task<string?> GenerateAccessToken()
        {
            string? auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{secretKey}"));

            var content = new Dictionary<string, string> {
                { "grant_type", "client_credentials" }
            };

            var url = $"{baseUrl}/v1/oauth2/token";

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

            HttpResponseMessage response = await client.SendAsync(request);
            var jsonString = response.Content.ReadAsStringAsync().Result;
            TokenResponse? tokenResponse = JsonSerializer.Deserialize<TokenResponse>(jsonString);

            if (response.IsSuccessStatusCode && tokenResponse is not null)
            {
                return tokenResponse.access_token;
            }

            return null;
        }

        
        public async Task<CreateOrderResponse?> CreateOrder(ICollection<ProductoCarrito>? products, string? total)
        {

            if (products is null)
                return null;

            var token = await GenerateAccessToken();

            var url = $"{baseUrl}/v2/checkout/orders";

            List<Item> items = CreateItems(products);

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

            HttpResponseMessage? httpResponse = await client.SendAsync(request);

            var jsonString = httpResponse.Content.ReadAsStringAsync().Result;

            CreateOrderResponse? createOrderResponse = JsonSerializer.Deserialize<CreateOrderResponse>(jsonString);


            if (httpResponse.IsSuccessStatusCode)
            {
                return createOrderResponse;
            }

            return null;
        }


        public async Task<CaptureOrderResponse?> CaptureOrder(string? orderID)
        {
            var token = await GenerateAccessToken();

            var url = $"{baseUrl}/v2/checkout/orders/{orderID}/capture";

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

            HttpResponseMessage? httpResponse = await client.SendAsync(request);

            var jsonResponse = await httpResponse.Content.ReadAsStringAsync();

            if (!httpResponse.IsSuccessStatusCode || httpResponse is null)
            {
                throw new Exception(jsonResponse);
            }

            var response = JsonSerializer.Deserialize<CaptureOrderResponse>(jsonResponse);

            return response;

        }

        private static List<Item> CreateItems(ICollection<ProductoCarrito> products)
        {

            var items = new List<Item>();

            foreach (var p in products)
            {
                items.Add(new Item
                {
                    name = p.Sku!.Key,
                    quantity = p!.Cantidad.ToString(),
                    unit_amount = new Amount
                    {
                        currency_code = "USD",
                        value = p.Sku?.Precio.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)
                    },
                    sku = p!.SkuId.ToString()
                });
            }

            return items;

        }
    }
}


public sealed class TokenResponse
{

    public string? scope { get; set; }
    public string? access_token { get; set; }
    public string? token_type { get; set; }
    public string? app_id { get; set; }
    public int? expires_in { get; set; }
}


public class CreateOrderRequest
{

    public List<PurchaseUnit> purchase_units { get; set; } = new List<PurchaseUnit>();
    public string? intent { get; set; }
}

public class PurchaseUnit
{
    public List<Item>? items { get; set; }
    public Amount? amount { get; set; }
    public Shipping? shipping { get; set; }
}
public class Item
{

    public string? name { get; set; }
    public string? quantity { get; set; }
    public Amount? unit_amount { get; set; }
    public string? sku { get; set; }
}

public class Amount
{

    public string? currency_code { get; set; }
    public string? value { get; set; }
    public Breakdown? breakdown { get; set; }
}

public class Breakdown
{

    public ItemTotal? item_total { get; set; }
}

public class ItemTotal
{
    public string? currency_code { get; set; }
    public string? value { get; set; }
}

public class CreateOrderResponse
{

    public string? id { get; set; }
    public string? create_time { get; set; }
    public string? update_time { get; set; }
    public string? processing_instruction { get; set; }
    public List<Link>? links { get; set; }
    public string? intent { get; set; }
    public string? status { get; set; }
}

public class Link
{
    public string? href { get; set; }
    public string? rel { get; set; }
    public string? method { get; set; }
    public PaymentSource? payment_source { get; set; }
}

public class PaymentSource
{
    public Card? card { get; set; }
    public Paypal? paypal { get; set; }
}

public class Card
{
    public string? name { get; set; }
    public string? last_digits { get; set; }
    public List<string>? available_networks { get; set; }
    public string? type { get; set; }
    public string? brand { get; set; }
    public string? expiry { get; set; }
}

public class Paypal
{
    public string? email_address { get; set; }
    public string? account_id { get; set; }
    public Name? name { get; set; }
    public PhoneNumber? phone_number { get; set; }
    public Address? address { get; set; }

}

public class Name
{
    public string? given_name { get; set; }
    public string? surname { get; set; }
}

public class Address
{
    public string? address_line_1 { get; set; }
    public string? address_line_2 { get; set; }
    public string? admin_area_2 { get; set; }
    public string? admin_area_1 { get; set; }
    public string? postal_code { get; set; }
    public string? country_code { get; set; }

}

public class PhoneNumber
{
    public string? national_number { get; set; }
}

public class Phone
{
    public string? phone_type { get; set; }
    public PhoneNumber? phone_number { get; set; }
}

public class CaptureOrderResponse
{

    public string? create_time { get; set; }
    public string? update_time { get; set; }
    public string? id { get; set; }
    public string? processing_instruction { get; set; }
    public List<PurchaseUnit>? purchase_units { get; set; }
    public List<Link>? links { get; set; }
    public PaymentSource? payment_source { get; set; }
    public Payer? payer { get; set; }

}

public class Payer
{

    public string? email_address { get; set; }
    public string? payer_id { get; set; }
    public Name? name { get; set; }
    public Phone? phone { get; set; }
    public string? birth_date { get; set; }
    public Address? address { get; set; }
}

public class Shipping
{

    public Address? address { get; set; }
}