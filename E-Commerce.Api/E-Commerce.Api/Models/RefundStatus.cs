using System.Text.Json.Serialization;

namespace E_Commerce.Api.Models
{
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RefundStatus
    {
        Pending = 1,
        Completed = 6,
        Failed = 7
    }
}