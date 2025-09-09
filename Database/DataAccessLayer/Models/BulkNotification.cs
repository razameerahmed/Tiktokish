using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class BulkNotification
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string? MobileNumber { get; set; }

    public string? Email { get; set; }

    public string Message { get; set; } = null!;

    public int IsSent { get; set; }

    public int IsExported { get; set; }

    public int NotificationType { get; set; }

    public int? Priority { get; set; }

    public DateOnly? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateOnly? UpdatedOn { get; set; }

    public string? UpdatedBy { get; set; }
}
