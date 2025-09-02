namespace Common.Model
{
    public class LoginResponse
    {
        public string? Token { get; set; } = null!;
        public string? Username { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public string Role { get; set; } = "User";
        public bool? IsVerified { get; set; }
    }

}