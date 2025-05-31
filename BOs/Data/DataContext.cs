using BOs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOs.Data
{
    public partial class DataContext : DbContext
    {

        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Class> Classes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(GetConnectionString());
        private string GetConnectionString()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true).Build();
            return configuration["ConnectionStrings:DefaultConnection"];
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Role
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.HasKey(r => r.RoleId);

                entity.Property(r => r.RoleName)
                      .IsRequired()
                      .HasMaxLength(100)
                      .IsUnicode(true);

                entity.HasMany(r => r.Accounts)
                      .WithOne(a => a.Role)
                      .HasForeignKey(a => a.RoleId);

                entity.HasData(
                    new Role { RoleId = 1, RoleName = "Admin" },
                    new Role { RoleId = 2, RoleName = "Nurse" },
                    new Role { RoleId = 3, RoleName = "Parent" }
                );
            });
            #endregion

            #region Account
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.HasKey(a => a.AccountId);

                entity.Property(a => a.Email)
                      .IsRequired()
                      .HasMaxLength(255)
                      .IsUnicode(false);

                entity.Property(a => a.Password)
                      .IsRequired()
                      .HasMaxLength(255)
                      .IsUnicode(false);

                entity.Property(a => a.Name)
                      .IsRequired()
                      .HasMaxLength(150)
                      .IsUnicode(true);

                entity.Property(a => a.Phone)
                      .HasColumnType("varchar(20)");

                entity.Property(a => a.DateOfBirth).IsRequired();
                entity.Property(a => a.CreatedAt).IsRequired();
                entity.Property(a => a.UpdateAt).IsRequired();
                entity.Property(a => a.Status).IsRequired();

                entity.HasMany(a => a.Students)
                      .WithOne(s => s.Parent)
                      .HasForeignKey(s => s.ParentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region Class
            modelBuilder.Entity<Class>(entity =>
            {
                entity.ToTable("Class");

                entity.HasKey(c => c.ClassId);

                entity.Property(c => c.ClassName)
                      .IsRequired()
                      .HasMaxLength(50)
                      .IsUnicode(true);

                entity.HasMany(c => c.Students)
                      .WithOne(s => s.Class)
                      .HasForeignKey(s => s.ClassId);
            });
            #endregion

            #region Student
            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Student");

                entity.HasKey(s => s.StudentId);

                entity.Property(s => s.Fullname)
                      .IsRequired()
                      .HasMaxLength(200)
                      .IsUnicode(true);

                entity.Property(s => s.StudentCode)
                      .IsRequired()
                      .HasMaxLength(50)
                      .IsUnicode(false);

                entity.Property(s => s.Gender)
                      .IsRequired()
                      .HasMaxLength(10)
                      .IsUnicode(false);

                entity.Property(s => s.DateOfBirth).IsRequired();
                entity.Property(s => s.CreatedAt).IsRequired();
                entity.Property(s => s.UpdateAt).IsRequired();

                entity.HasIndex(s => s.StudentCode).IsUnique();

                entity.HasOne(s => s.Parent)
                      .WithMany(a => a.Students)
                      .HasForeignKey(s => s.ParentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Class)
                      .WithMany(c => c.Students)
                      .HasForeignKey(s => s.ClassId);
            });
            #endregion

            OnModelCreatingPartial(modelBuilder);
        }


        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
