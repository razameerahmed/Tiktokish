namespace Common.Model
{
	public class UserLogin
	{
        //public Guid UserId { get; set; }                // Primary Key (UUID instead of int for scalability)
        public required string Username { get; set; }            // Unique
        public required string Password { get; set; }        // Store only hash, never plain password
        public required string PhoneNumber { get; set; }         // Optional, but unique if provided
        public required string CountryCode { get; set; }         // e.g. "+86"
        public required string Email { get; set; }               // Optional, unique if provided
        public DateTime DateOfBirth { get; set; }       // Required for age restriction
        public string Gender { get; set; } = "N/A";             // Optional ("Male", "Female", "Other")

        public string? ProfilePictureUrl { get; set; }   // Default avatar if null
        public string? ReferralCode { get; set; }        // Optional, for invite rewards

        public required string DeviceId { get; set; }            // For tracking / anti-fraud (TikTok style)

        //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }      // Updated on each login
        public string? Token { get; set; } = null!;

    }
}
