using System.Text.Json.Serialization;

namespace Imaging.ExternalService.BingModels
{
    /// <summary>
    /// Defines a merchant's offer.
    /// </summary>
    public class Offer : Thing
    {
        /// <summary>
        /// Gets or sets seller for this offer.
        /// </summary>
        [JsonPropertyName("seller")]
        public Thing? Seller { get; set; }

        /// <summary>
        /// Gets or sets the item's price.
        /// </summary>
        [JsonPropertyName("price")]
        public double? Price { get; set; }

        /// <summary>
        /// Gets or sets the monetary currency. For example, USD. Possible values include:
        /// 'USD', 'CAD', 'GBP', 'EUR', 'COP', 'JPY', 'CNY', 'AUD', 'INR', 'AED', 'AFN', 'ALL', 'AMD', 'ANG', 'AOA', 'ARS', 'AWG', 'AZN',
        /// 'BAM', 'BBD', 'BDT', 'BGN', 'BHD', 'BIF', 'BMD', 'BND', 'BOB', 'BOV', 'BRL', 'BSD', 'BTN', 'BWP', 'BYR', 'BZD', 'CDF', 'CHE',
        /// 'CHF', 'CHW', 'CLF', 'CLP', 'COU', 'CRC', 'CUC', 'CUP', 'CVE', 'CZK', 'DJF', 'DKK', 'DOP', 'DZD', 'EGP', 'ERN', 'ETB', 'FJD',
        /// 'FKP', 'GEL', 'GHS', 'GIP', 'GMD', 'GNF', 'GTQ', 'GYD', 'HKD', 'HNL', 'HRK', 'HTG', 'HUF', 'IDR', 'ILS', 'IQD', 'IRR', 'ISK',
        /// 'JMD', 'JOD', 'KES', 'KGS', 'KHR', 'KMF', 'KPW', 'KRW', 'KWD', 'KYD', 'KZT', 'LAK', 'LBP', 'LKR', 'LRD', 'LSL', 'LYD', 'MAD',
        /// 'MDL', 'MGA', 'MKD', 'MMK', 'MNT', 'MOP', 'MRO', 'MUR', 'MVR', 'MWK', 'MXN', 'MXV', 'MYR', 'MZN', 'NAD', 'NGN', 'NIO', 'NOK',
        /// 'NPR', 'NZD', 'OMR', 'PAB', 'PEN', 'PGK', 'PHP', 'PKR', 'PLN', 'PYG', 'QAR', 'RON', 'RSD', 'RUB', 'RWF', 'SAR', 'SBD', 'SCR',
        /// 'SDG', 'SEK', 'SGD', 'SHP', 'SLL', 'SOS', 'SRD', 'SSP', 'STD', 'SYP', 'SZL', 'THB', 'TJS', 'TMT', 'TND', 'TOP', 'TRY', 'TTD',
        /// 'TWD', 'TZS', 'UAH', 'UGX', 'UYU', 'UZS', 'VEF', 'VND', 'VUV', 'WST', 'XAF', 'XCD', 'XOF', 'XPF', 'YER', 'ZAR', 'ZMW'.
        /// </summary>
        [JsonPropertyName("priceCurrency")]
        public string? PriceCurrency { get; set; }

        /// <summary>
        /// Gets or sets the item's availability. Possible values include:
        /// 'Discontinued', 'InStock', 'InStoreOnly', 'LimitedAvailability', 'OnlineOnly', 'OutOfStock', 'PreOrder', 'SoldOut'.
        /// </summary>
        [JsonPropertyName("availability")]
        public string? Availability { get; set; }

        /// <summary>
        /// Gets or sets an aggregated rating that indicates how well the product has been rated by others.
        /// </summary>
        [JsonPropertyName("aggregateRating")]
        public AggregateRating? AggregateRating { get; set; }

        /// <summary>
        /// Gets or sets the last date that the offer was updated. The date is in the form YYYY-MM-DD.
        /// </summary>
        [JsonPropertyName("lastUpdated")]
        public string? LastUpdated { get; set; }
    }
}
