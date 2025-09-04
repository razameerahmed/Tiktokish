using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phonenumber { get; set; }

    public string? Fullname { get; set; }

    public string Passwordhash { get; set; } = null!;

    public string? Avatarurl { get; set; }

    public string? Biometric { get; set; }

    public bool Isactive { get; set; }

    public DateTime Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public bool Isverified { get; set; }

    public string Role { get; set; } = null!;

    public DateTime? Lastloginat { get; set; }

    public string? Locale { get; set; }

    public string? Devicetype { get; set; }

    public string? Lastlocation { get; set; }
}
