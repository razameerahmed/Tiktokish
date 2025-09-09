using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class UserOtp
{
    public decimal Id { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedOn { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public string Username { get; set; } = null!;

    public DateTime? ExpiryDate { get; set; }

    public string? SmsOtp { get; set; }

    public string? EmailOtp { get; set; }

    public string? RetryCount { get; set; }

    public short? Status { get; set; }

    public string? Issplit { get; set; }

    public string? Otptype { get; set; }

    public string? InvalidRetry { get; set; }

    public string? Otpexpiry { get; set; }
}
