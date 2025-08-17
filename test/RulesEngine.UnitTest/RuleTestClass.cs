// Copyright (c) Microsoft Corporation.
//  Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace RulesEngine.UnitTest
{
    [ExcludeFromCodeCoverage]
    public class RuleTestClass
    {
        [JsonPropertyName("country")]
        public string Country { get; set; }
        [JsonPropertyName("loyaltyFactor")]
        public int LoyaltyFactor { get; set; }
        [JsonPropertyName("totalPurchasesToDate")]
        public int TotalPurchasesToDate { get; set; }
    }
}
