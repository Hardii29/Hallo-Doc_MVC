using System;
using System.Collections.Generic;
using Assignment.Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Entity.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<StudentDatum> StudentData { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User ID = postgres;Password=&^54UYtr;Server=localhost;Database=Student;Integrated Security=true;Pooling=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("Course_pkey");
        });

        modelBuilder.Entity<StudentDatum>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("StudentData_pkey");

            entity.HasOne(d => d.CourseNavigation).WithMany(p => p.StudentData)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("StudentData_CourseId_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
