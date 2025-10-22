
using Security.Shared.Enums;

namespace Security.Shared
{
    public class DataResponse<T> 
    {
        public EDataResponseCode ResponseCode { get; set; }
        public T? Data { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string CorrelationId { get; set; } = string.Empty;
    }
}
