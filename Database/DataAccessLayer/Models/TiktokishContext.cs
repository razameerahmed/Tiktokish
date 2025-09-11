using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Models;

public partial class TiktokishContext : DbContext
{
    public TiktokishContext()
    {
    }

    public TiktokishContext(DbContextOptions<TiktokishContext> options)
        : base(options)
    {
    }

    public TiktokishContext(string connectionString)
    {
    }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<BulkNotification> BulkNotifications { get; set; }

    public virtual DbSet<NotificationTemplate> NotificationTemplates { get; set; }

    public virtual DbSet<NotificationType> NotificationTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserNotificationSubscribtion> UserNotificationSubscribtions { get; set; }

    public virtual DbSet<UserOtp> UserOtps { get; set; }

    public virtual DbSet<UserPwdHistory> UserPwdHistories { get; set; }

    public virtual DbSet<UserSessionHistory> UserSessionHistories { get; set; }

    public virtual DbSet<UserTrustedDevice> UserTrustedDevices { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-HK2464R\\SQLEXPRESS;Database=Tiktokish;User ID=sa;Password=avanza@123;Trusted_Connection=False;Encrypt=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditLogId).HasName("PK__AUDIT_LO__EB5F6CBD0FE7CC92");

            entity.ToTable("AUDIT_LOG");

            entity.Property(e => e.ActionDetail)
                .IsUnicode(false)
                .HasColumnName("ACTION_DETAIL");
            entity.Property(e => e.ActionEntity)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ACTION_ENTITY");
            entity.Property(e => e.ActionTimestamp)
                .HasMaxLength(14)
                .IsUnicode(false)
                .HasColumnName("ACTION_TIMESTAMP");
            entity.Property(e => e.ActionType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ACTION_TYPE");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("USERNAME");
        });

        modelBuilder.Entity<BulkNotification>(entity =>
        {
            entity.ToTable("BULK_NOTIFICATION");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATED_BY");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("CREATED_ON");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.IsExported).HasColumnName("IS_EXPORTED");
            entity.Property(e => e.IsSent).HasColumnName("IS_SENT");
            entity.Property(e => e.Message)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("MESSAGE");
            entity.Property(e => e.MobileNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MOBILE_NUMBER");
            entity.Property(e => e.NotificationType).HasColumnName("NOTIFICATION_TYPE");
            entity.Property(e => e.Priority)
                .HasDefaultValue(0)
                .HasColumnName("PRIORITY");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("UPDATED_BY");
            entity.Property(e => e.UpdatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("UPDATED_ON");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("USERNAME");
        });

        modelBuilder.Entity<NotificationTemplate>(entity =>
        {
            entity.HasKey(e => e.PkTemplateId).HasName("PK_NOTIF_TEMPLATE");

            entity.ToTable("NOTIFICATION_TEMPLATES");

            entity.Property(e => e.PkTemplateId)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("PK_TEMPLATE_ID");
            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ACTION");
            entity.Property(e => e.AttachmentFile)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("ATTACHMENT_FILE");
            entity.Property(e => e.Body).HasColumnName("BODY");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CREATED_BY");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATED_ON");
            entity.Property(e => e.NotificationTypeId).HasColumnName("NOTIFICATION_TYPE_ID");
            entity.Property(e => e.Subject)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SUBJECT");
            entity.Property(e => e.TemplateName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TEMPLATE_NAME");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("UPDATED_BY");
            entity.Property(e => e.UpdatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("UPDATED_ON");

            entity.HasOne(d => d.NotificationType).WithMany(p => p.NotificationTemplates)
                .HasForeignKey(d => d.NotificationTypeId)
                .HasConstraintName("FK_NOTIFI_TYPE");
        });

        modelBuilder.Entity<NotificationType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__NOTIFICA__41F99A5209FC1DA3");

            entity.ToTable("NOTIFICATION_TYPE");

            entity.Property(e => e.TypeId)
                .ValueGeneratedNever()
                .HasColumnName("TYPE_ID");
            entity.Property(e => e.TypeName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("TYPE_NAME");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__USERS__3214EC276B0E977D");

            entity.ToTable("USER");

            entity.HasIndex(e => e.Email, "UQ__USERS__161CF72428C2E798").IsUnique();

            entity.HasIndex(e => e.Phonenumber, "UQ__USERS__8F2B07B15BB828B8").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__USERS__B15BE12EE28652EC").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Avatarurl)
                .HasMaxLength(500)
                .HasColumnName("AVATARURL");
            entity.Property(e => e.Biometric)
                .HasMaxLength(400)
                .HasColumnName("BIOMETRIC");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("CREATEDAT");
            entity.Property(e => e.Devicetype)
                .HasMaxLength(100)
                .HasColumnName("DEVICETYPE");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .HasColumnName("FULLNAME");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("ISACTIVE");
            entity.Property(e => e.Isverified).HasColumnName("ISVERIFIED");
            entity.Property(e => e.Lastlocation)
                .HasMaxLength(200)
                .HasColumnName("LASTLOCATION");
            entity.Property(e => e.Lastloginat).HasColumnName("LASTLOGINAT");
            entity.Property(e => e.Locale)
                .HasMaxLength(10)
                .HasColumnName("LOCALE");
            entity.Property(e => e.Passwordhash)
                .HasMaxLength(255)
                .HasColumnName("PASSWORDHASH");
            entity.Property(e => e.Phonenumber)
                .HasMaxLength(20)
                .HasColumnName("PHONENUMBER");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasDefaultValue("User")
                .HasColumnName("ROLE");
            entity.Property(e => e.Updatedat).HasColumnName("UPDATEDAT");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("USERNAME");
        });

        modelBuilder.Entity<UserNotificationSubscribtion>(entity =>
        {
            entity.HasKey(e => e.PkUserSubscribtionId).HasName("PK_ACCOUNT_SUBSCRIBTION");

            entity.ToTable("USER_NOTIFICATION_SUBSCRIBTION");

            entity.Property(e => e.PkUserSubscribtionId).HasColumnName("PK_USER_SUBSCRIBTION_ID");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasDefaultValue("system")
                .HasColumnName("CREATED_BY");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATED_ON");
            entity.Property(e => e.IsAllowed).HasColumnName("IS_ALLOWED");
            entity.Property(e => e.NotificationType).HasColumnName("NOTIFICATION_TYPE");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasDefaultValue("system")
                .HasColumnName("UPDATED_BY");
            entity.Property(e => e.UpdatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("UPDATED_ON");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("USERNAME");
        });

        modelBuilder.Entity<UserOtp>(entity =>
        {
            entity.ToTable("USER_OTP");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(19, 0)")
                .HasColumnName("ID");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATED_BY");
            entity.Property(e => e.CreatedOn)
                .HasPrecision(3)
                .HasColumnName("CREATED_ON");
            entity.Property(e => e.EmailOtp)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("EMAIL_OTP");
            entity.Property(e => e.ExpiryDate)
                .HasPrecision(6)
                .HasColumnName("EXPIRY_DATE");
            entity.Property(e => e.InvalidRetry)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("INVALID_RETRY");
            entity.Property(e => e.Issplit)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ISSPLIT");
            entity.Property(e => e.Otpexpiry)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("OTPEXPIRY");
            entity.Property(e => e.Otptype)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("OTPTYPE");
            entity.Property(e => e.RetryCount)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("RETRY_COUNT");
            entity.Property(e => e.SmsOtp)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("SMS_OTP");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("UPDATED_BY");
            entity.Property(e => e.UpdatedOn)
                .HasPrecision(3)
                .HasColumnName("UPDATED_ON");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("USERNAME");
        });

        modelBuilder.Entity<UserPwdHistory>(entity =>
        {
            entity.HasKey(e => e.PwdHistoryId);

            entity.ToTable("USER_PWD_HISTORY");

            entity.Property(e => e.PwdHistoryId)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasDefaultValue("0")
                .HasColumnName("PWD_HISTORY_ID");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATED_ON");
            entity.Property(e => e.Password)
                .HasMaxLength(200)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.UpdatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("UPDATED_ON");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("USERNAME");
        });

        modelBuilder.Entity<UserSessionHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("USER_SESSION_HISTORY_PK");

            entity.ToTable("USER_SESSION_HISTORY");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(38, 0)")
                .HasColumnName("ID");
            entity.Property(e => e.AccessToken)
                .HasMaxLength(999)
                .IsUnicode(false)
                .HasColumnName("ACCESS_TOKEN");
            entity.Property(e => e.AccessTokenExpiry)
                .HasColumnType("datetime")
                .HasColumnName("ACCESS_TOKEN_EXPIRY");
            entity.Property(e => e.Comments)
                .HasMaxLength(255)
                .HasColumnName("COMMENTS");
            entity.Property(e => e.LoginTime)
                .HasPrecision(3)
                .HasColumnName("LOGIN_TIME");
            entity.Property(e => e.LogoutTime)
                .HasPrecision(3)
                .HasColumnName("LOGOUT_TIME");
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(999)
                .IsUnicode(false)
                .HasColumnName("REFRESH_TOKEN");
            entity.Property(e => e.RefreshTokenExpiry)
                .HasColumnType("datetime")
                .HasColumnName("REFRESH_TOKEN_EXPIRY");
            entity.Property(e => e.SessionClearBy)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("SESSION_CLEAR_BY");
            entity.Property(e => e.SessionClearOn)
                .HasPrecision(3)
                .HasColumnName("SESSION_CLEAR_ON");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("USERNAME");
        });

        modelBuilder.Entity<UserTrustedDevice>(entity =>
        {
            entity.HasKey(e => new { e.PkTrustedDeviceId, e.Username });

            entity.ToTable("USER_TRUSTED_DEVICE");

            entity.Property(e => e.PkTrustedDeviceId)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("PK_TRUSTED_DEVICE_ID");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("USERNAME");
            entity.Property(e => e.Blacklist)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(38, 0)")
                .HasColumnName("BLACKLIST");
            entity.Property(e => e.Bmv)
                .HasMaxLength(225)
                .IsUnicode(false)
                .HasColumnName("BMV");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATED_BY");
            entity.Property(e => e.CreatedOn)
                .HasPrecision(3)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("CREATED_ON");
            entity.Property(e => e.DeviceCountryCode)
                .HasMaxLength(30)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("DEVICE_COUNTRY_CODE");
            entity.Property(e => e.DeviceFcmToken)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("DEVICE_FCM_TOKEN");
            entity.Property(e => e.DeviceFirstSignIn)
                .HasPrecision(3)
                .HasColumnName("DEVICE_FIRST_SIGN_IN");
            entity.Property(e => e.DeviceIp)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("DEVICE_IP");
            entity.Property(e => e.DeviceName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DEVICE_NAME");
            entity.Property(e => e.Devicetype)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("DEVICETYPE");
            entity.Property(e => e.IsAllowed)
                .HasColumnType("decimal(38, 0)")
                .HasColumnName("IS_ALLOWED");
            entity.Property(e => e.Make)
                .HasMaxLength(225)
                .IsUnicode(false)
                .HasColumnName("MAKE");
            entity.Property(e => e.Model)
                .HasMaxLength(225)
                .IsUnicode(false)
                .HasColumnName("MODEL");
            entity.Property(e => e.OsDistribution)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("OS_DISTRIBUTION");
            entity.Property(e => e.PushNotiIsAllowed)
                .HasColumnType("decimal(38, 0)")
                .HasColumnName("PUSH_NOTI_IS_ALLOWED");
            entity.Property(e => e.ReceiverId)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("RECEIVER_ID");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("UPDATED_BY");
            entity.Property(e => e.UpdatedOn)
                .HasPrecision(3)
                .HasColumnName("UPDATED_ON");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
