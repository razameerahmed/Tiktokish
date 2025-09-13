using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Feed
{
    public long FeedId { get; set; }

    public string Username { get; set; } = null!;

    public string VideoUrl { get; set; } = null!;

    public string? Caption { get; set; }

    public string? ThumbnailUrl { get; set; }

    public int LikesCount { get; set; }

    public int CommentsCount { get; set; }

    public int SharesCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }
}
