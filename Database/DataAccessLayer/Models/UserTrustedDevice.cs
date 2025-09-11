using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class UserTrustedDevice
{
    public string PkTrustedDeviceId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string? ReceiverId { get; set; }

    public string? DeviceName { get; set; }

    public string Devicetype { get; set; } = null!;

    public string? DeviceIp { get; set; }

    public DateTime? DeviceFirstSignIn { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedOn { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public decimal? PushNotiIsAllowed { get; set; }

    public decimal IsAllowed { get; set; }

    public string? OsDistribution { get; set; }

    public string? DeviceFcmToken { get; set; }

    public string? DeviceCountryCode { get; set; }

    public decimal? Blacklist { get; set; }

    public string? Bmv { get; set; }

    public string? Make { get; set; }

    public string? Model { get; set; }
}
