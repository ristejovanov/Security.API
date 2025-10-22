
namespace Security.Shared.Dtos
{
    public class ClientDto
    {
        public Guid ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
