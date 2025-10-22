namespace Security.Shared.Dtos
{
    public class CreateUserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}
