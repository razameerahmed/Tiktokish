using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class AuditLog
{
    public string Username { get; set; } = null!;

    public string ActionTimestamp { get; set; } = null!;

    public string ActionType { get; set; } = null!;

    public string ActionEntity { get; set; } = null!;

    public string? ActionDetail { get; set; }

    public int AuditLogId { get; set; }
}
