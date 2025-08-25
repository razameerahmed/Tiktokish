using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class UserInfo
{
    public int Id { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? Mobile { get; set; }

    public string? Email { get; set; }

    public bool? IsActive { get; set; }
}
