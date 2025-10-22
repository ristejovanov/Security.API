namespace Security.Shared.Dtos
{
    public class ReturnClientDto
    {
        public Guid ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
    }
}
