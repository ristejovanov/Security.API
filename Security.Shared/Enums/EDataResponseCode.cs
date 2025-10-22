
using System.Text.Json.Serialization;

namespace Security.Shared.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EDataResponseCode
    {
        Success = 0,
        InvalidInputParameter = 1,
        NotFound = 2,
        Conflict = 3,
        Unauthorized = 4,
        InternalError = 5,
    }
}
