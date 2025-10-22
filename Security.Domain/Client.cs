namespace Security.Domain
{
    public class Client
    {
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public byte[] ApiKeyHash { get; set; }
        public byte[] ApiKeySalt { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

