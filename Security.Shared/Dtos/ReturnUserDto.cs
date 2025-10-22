namespace Security.Shared.Dtos
{
    public class ReturnUserDto
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Culture { get; set; } = string.Empty;
    }
}
