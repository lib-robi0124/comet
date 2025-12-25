namespace Comet.Domain.Entities
{
    public abstract class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public virtual bool CanSubmitPrice() => false;
    }
}
