﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Models;

public partial class TiktokishContext : DbContext
{
	private readonly string _connectionString;
	public TiktokishContext(string connectionString)
		{
			_connectionString = connectionString;
		}

    public TiktokishContext(DbContextOptions<TiktokishContext> options)
        : base(options)
    {
    }

    public virtual DbSet<UserInfo> UserInfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=AS-BSD-RAZAMER\\RAZAMEER;User ID=sa;Password=avanza@123;Database=Tiktokish;Trusted_Connection=False;Encrypt=False;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserInfo>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("UserInfo");

            entity.Property(e => e.Email).HasMaxLength(500);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Mobile).HasMaxLength(200);
            entity.Property(e => e.Password).HasMaxLength(200);
            entity.Property(e => e.UserName).HasMaxLength(200);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
