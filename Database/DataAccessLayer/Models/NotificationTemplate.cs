using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class NotificationTemplate
{
    public decimal PkTemplateId { get; set; }

    public int? NotificationTypeId { get; set; }

    public string? Action { get; set; }

    public string TemplateName { get; set; } = null!;

    public string? Subject { get; set; }

    public string? Body { get; set; }

    public string? AttachmentFile { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedOn { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public virtual NotificationType? NotificationType { get; set; }
}
