using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class UserPwdHistory
{
    public string PwdHistoryId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}
