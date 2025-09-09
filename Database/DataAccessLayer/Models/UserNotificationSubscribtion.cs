using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class UserNotificationSubscribtion
{
    public int PkUserSubscribtionId { get; set; }

    public string Username { get; set; } = null!;

    public int NotificationType { get; set; }

    public bool IsAllowed { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime UpdatedOn { get; set; }
}
