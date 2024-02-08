using System;
using System.Collections.Generic;
using Hallo_Doc.Models;
using Microsoft.EntityFrameworkCore;

namespace Hallo_Doc.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<AspnetUser> AspnetUsers { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User ID = postgres;Password=&^54UYtr;Server=localhost;Database=Hallo-Doc;Integrated Security=true;Pooling=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("Admin_pkey");

            entity.ToTable("Admin");

            entity.Property(e => e.AdminId).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<AspnetUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("aspnetusers_pkey");

            entity.ToTable("ASPNetUsers");

            entity.Property(e => e.Id)
                .HasMaxLength(128)
                .HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .HasColumnName("email");
            entity.Property(e => e.Emailconfirmed)
                .HasColumnType("bit(1)")
                .HasColumnName("emailconfirmed");
            entity.Property(e => e.Passwordhash)
                .HasColumnType("character varying")
                .HasColumnName("passwordhash");
            entity.Property(e => e.Phonenumber)
                .HasColumnType("character varying")
                .HasColumnName("phonenumber");
            entity.Property(e => e.Phonenumberconfirmed)
                .HasColumnType("bit(1)")
                .HasColumnName("phonenumberconfirmed");
            entity.Property(e => e.Securitystamp)
                .HasColumnType("character varying")
                .HasColumnName("securitystamp");
            entity.Property(e => e.Twofactorenabled)
                .HasColumnType("bit(1)")
                .HasColumnName("twofactorenabled");
            entity.Property(e => e.Username)
                .HasMaxLength(256)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("request_pkey");

            entity.ToTable("request");

            entity.Property(e => e.RequestId).UseIdentityAlwaysColumn();
            entity.Property(e => e.ConfirmationNumber).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.Ip)
                .HasMaxLength(20)
                .HasColumnName("IP");
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            entity.HasOne(d => d.User).WithMany(p => p.Requests)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("UserId");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Userid)
                .UseIdentityAlwaysColumn()
                .HasColumnName("userid");
            entity.Property(e => e.Aspnetuserid)
                .HasMaxLength(128)
                .HasColumnName("aspnetuserid");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.Createdby)
                .HasMaxLength(128)
                .HasColumnName("createdby");
            entity.Property(e => e.Createddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createddate");
            entity.Property(e => e.Dob)
                .HasMaxLength(100)
                .HasColumnName("DOB");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(100)
                .HasColumnName("firstname");
            entity.Property(e => e.Intdate).HasColumnName("intdate");
            entity.Property(e => e.Ip)
                .HasMaxLength(20)
                .HasColumnName("ip");
            entity.Property(e => e.Isdeleted)
                .HasColumnType("bit(1)")
                .HasColumnName("isdeleted");
            entity.Property(e => e.Ismobile)
                .HasColumnType("bit(1)")
                .HasColumnName("ismobile");
            entity.Property(e => e.Isrequestwithemail)
                .HasColumnType("bit(1)")
                .HasColumnName("isrequestwithemail");
            entity.Property(e => e.Lastname)
                .HasMaxLength(100)
                .HasColumnName("lastname");
            entity.Property(e => e.Mobile)
                .HasMaxLength(20)
                .HasColumnName("mobile");
            entity.Property(e => e.Modifiedby)
                .HasMaxLength(128)
                .HasColumnName("modifiedby");
            entity.Property(e => e.Modifieddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifieddate");
            entity.Property(e => e.Regionid).HasColumnName("regionid");
            entity.Property(e => e.State)
                .HasMaxLength(100)
                .HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Street)
                .HasMaxLength(100)
                .HasColumnName("street");
            entity.Property(e => e.Strmonth)
                .HasMaxLength(20)
                .HasColumnName("strmonth");
            entity.Property(e => e.Stryear).HasColumnName("stryear");
            entity.Property(e => e.Zipcode)
                .HasMaxLength(10)
                .HasColumnName("zipcode");

            entity.HasOne(d => d.Aspnetuser).WithMany(p => p.Users)
                .HasForeignKey(d => d.Aspnetuserid)
                .HasConstraintName("fk_aspnetuserid");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
