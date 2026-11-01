using System;
using System.Collections.Generic;
using System.Text;

namespace UyCommerce.Application.DTOs.ExternalServices.Paypal
{
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
}
