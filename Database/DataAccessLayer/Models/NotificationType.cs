using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class NotificationType
{
    public int TypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<NotificationTemplate> NotificationTemplates { get; set; } = new List<NotificationTemplate>();
}
