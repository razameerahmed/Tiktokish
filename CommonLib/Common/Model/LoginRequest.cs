namespace Common.Model

{
    public class LoginRequest
    {
        //public string Identifier { get; set; } = null!; // Can be Username, Email, or PhoneNumber
        //public string Password { get; set; } = null!;

        public required string Username { get; set; }
        public required string? FullName { get; set; }
        public required string Password { get; set; }
        public required string? PhoneNumber { get; set; }         // Optional, but unique if provided
        public required string? CountryCode { get; set; }         // e.g. "+86"
        public required string? Email { get; set; }               // Optional, unique if provided
        public DateTime DateOfBirth { get; set; }       // Required for age restriction
        public string Gender { get; set; } = "Other";             // Optional ("Male", "Female", "Other")
        public string? ProfilePictureUrl { get; set; }   // Default avatar if null
        public string? ReferralCode { get; set; }        // Optional, for invite rewards
        public required string DeviceId { get; set; }            // For tracking / anti-fraud (TikTok style)
        public required string DeviceIp { get; set; }            // For tracking / anti-fraud (TikTok style)
        public DateTime? LastLoginAt { get; set; }      // Updated on each login
        public string? correlationId { get; set; } = null!;
        public bool? isSuccess { get; set; } = false;
		public string? Token { get; set; } = null!;

	}
}