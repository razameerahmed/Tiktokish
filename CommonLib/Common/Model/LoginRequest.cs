namespace Common.Model

{
    public class LoginRequest
    {
        public string Identifier { get; set; } = null!; // Can be Username, Email, or PhoneNumber
        public string Password { get; set; } = null!;
        public string correlationId { get; set; } = null!;
        public bool isSuccess { get; set; } = false;
		public string Token { get; set; } = null!;
	}
}