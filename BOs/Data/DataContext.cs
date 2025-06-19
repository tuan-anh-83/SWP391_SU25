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
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<HealthRecord> HealthRecords { get; set; }
        public DbSet<MedicalEvent> MedicalEvents { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<ParentMedicationRequest> ParentMedicationRequests { get; set; }
        public DbSet<ParentMedicationDetail> ParentMedicationDetails { get; set; }
        public DbSet<MedicalSupply> MedicalSupplies { get; set; }
        public DbSet<Vaccine> Vaccines { get; set; }
        public DbSet<VaccinationCampaign> VaccinationCampaigns { get; set; }
        public DbSet<VaccinationConsent> VaccinationConsents { get; set; }
        public DbSet<VaccinationRecord> VaccinationRecords { get; set; }
        public DbSet<VaccinationFollowUp> VaccinationFollowUps { get; set; }
        public DbSet<HealthCheck> HealthChecks { get; set; }

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

                entity.HasKey(r => r.RoleID);

                entity.Property(r => r.RoleName)
                      .IsRequired()
                      .HasMaxLength(100)
                      .IsUnicode(true);

                entity.HasMany(r => r.Accounts)
                      .WithOne(a => a.Role)
                      .HasForeignKey(a => a.RoleID);

                entity.HasData(
                    new Role { RoleID = 1, RoleName = "Admin" },
                    new Role { RoleID = 2, RoleName = "Nurse" },
                    new Role { RoleID = 3, RoleName = "Parent" }
                );
            });
            #endregion

            #region Account
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.HasKey(a => a.AccountID);

                entity.Property(a => a.Email)
                      .IsRequired()
                      .HasMaxLength(255)
                      .IsUnicode(false);

                entity.Property(a => a.Password)
                      .IsRequired()
                      .HasMaxLength(255)
                      .IsUnicode(false);

                entity.Property(a => a.Fullname)
                      .IsRequired()
                      .HasMaxLength(150)
                      .IsUnicode(true);

                entity.Property(a => a.Address)
                      .IsRequired()
                      .HasMaxLength(1000)
                      .IsUnicode(true);

                entity.Property(a => a.PhoneNumber)
                      .HasMaxLength(15);


                entity.Property(a => a.DateOfBirth).IsRequired();
                entity.Property(a => a.CreatedAt).IsRequired();
                entity.Property(a => a.UpdateAt).IsRequired();
                entity.Property(a => a.Status).IsRequired();

                entity.Property(a => a.Image).HasColumnType("varbinary(max)"); // Lưu file nhị phân

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

            #region PasswordResetToken
            modelBuilder.Entity<PasswordResetToken>(entity =>
            {
                entity.ToTable("PasswordResetToken");
                entity.HasKey(e => e.PasswordResetTokenID);
                entity.Property(e => e.PasswordResetTokenID).ValueGeneratedOnAdd();
                entity.Property(e => e.Token)
                      .IsRequired()
                      .HasMaxLength(255);
                entity.Property(e => e.Expiration).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Account)
                      .WithMany(a => a.PasswordResetTokens)
                      .HasForeignKey(e => e.AccountID)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.HasKey(c => c.CategoryID);

                entity.Property(c => c.Name)
                      .IsRequired()
                      .HasMaxLength(100)
                      .IsUnicode(true);

                entity.HasMany(c => c.Blogs)
                      .WithOne(b => b.Category)
                      .HasForeignKey(b => b.CategoryID);
            });
            #endregion

            #region Blog
            modelBuilder.Entity<Blog>(entity =>
            {
                entity.ToTable("Blog");

                entity.HasKey(b => b.BlogID);

                entity.Property(b => b.Title)
                      .IsRequired()
                      .HasMaxLength(255)
                      .IsUnicode(true);

                entity.Property(b => b.Description)
                      .HasMaxLength(500)
                      .IsUnicode(true);

                entity.Property(b => b.Content)
                      .IsRequired()
                      .IsUnicode(true);

                entity.Property(b => b.Image)
                      .HasColumnType("varbinary(max)")  // Dữ liệu nhị phân lớn
                      .IsRequired(false);

                entity.HasOne(b => b.Account)
                      .WithMany() // Nếu bạn có List<Blog> trong Account thì sửa thành .WithMany(a => a.Blogs)
                      .HasForeignKey(b => b.AccountID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(b => b.Category)
                      .WithMany(c => c.Blogs)
                      .HasForeignKey(b => b.CategoryID)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region HealthRecord
            modelBuilder.Entity<HealthRecord>(entity =>
            {
                entity.ToTable("HealthRecord");

                entity.HasKey(h => h.HealthRecordId);

                entity.Property(h => h.StudentName)
                      .IsRequired()
                      .HasMaxLength(200)
                      .IsUnicode(true);

                entity.Property(h => h.StudentCode)
                      .IsRequired()
                      .HasMaxLength(50)
                      .IsUnicode(false);

                entity.Property(h => h.Gender)
                      .IsRequired()
                      .HasMaxLength(10)
                      .IsUnicode(false);

                entity.Property(h => h.DateOfBirth).IsRequired();
                entity.Property(h => h.Height).IsRequired();
                entity.Property(h => h.Weight).IsRequired();
                entity.Property(h => h.BMI).IsRequired();
                entity.Property(h => h.NutritionStatus).IsRequired();

                entity.HasOne(h => h.Student)
                      .WithMany()
                      .HasForeignKey(h => h.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(h => h.Parent)
                      .WithMany()
                      .HasForeignKey(h => h.ParentId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(hr => new { hr.ParentId, hr.StudentId }).IsUnique();
            });
            #endregion

            #region Medication      
            modelBuilder.Entity<Medication>(entity =>
            {
                entity.ToTable("Medication");

                entity.HasKey(m => m.MedicationId);

                entity.Property(m => m.Name)
                      .IsRequired()
                      .HasMaxLength(200)
                      .IsUnicode(true);

                entity.Property(m => m.Type)
                      .IsRequired()
                      .HasMaxLength(100)
                      .IsUnicode(true);

                entity.Property(m => m.Usage)
                      .HasMaxLength(500)
                      .IsUnicode(true);

                entity.Property(m => m.ExpiredDate).IsRequired();
            });
            #endregion

            #region MedicalEvent
            modelBuilder.Entity<MedicalEvent>(entity =>
            {
                entity.ToTable("MedicalEvent");

                entity.HasKey(me => me.MedicalEventId);

                entity.Property(me => me.Type)
                      .IsRequired()
                      .HasMaxLength(100)
                      .IsUnicode(true);

                entity.Property(me => me.Description)
                      .IsRequired()
                      .HasMaxLength(1000)
                      .IsUnicode(true);

                entity.Property(me => me.Note)
                      .HasMaxLength(1000)
                      .IsUnicode(true);

                entity.Property(me => me.Date).IsRequired();

                entity.HasOne(me => me.Student)
                      .WithMany()
                      .HasForeignKey(me => me.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(me => me.Nurse)
                      .WithMany()
                      .HasForeignKey(me => me.NurseId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(me => me.Medications)
                      .WithMany(m => m.MedicalEvents)
                      .UsingEntity(j => j.ToTable("MedicalEventMedication"));

                entity.HasMany(e => e.MedicalSupplies)
                      .WithMany(s => s.MedicalEvents)
                      .UsingEntity(j => j.ToTable("MedicalEventMedicalSupply"));
            });
            #endregion

            #region ParentMedicationRequest
            modelBuilder.Entity<ParentMedicationRequest>(entity =>
            {
                entity.ToTable("ParentMedicationRequest");

                entity.HasKey(r => r.RequestId);

                entity.Property(r => r.ParentNote)
                      .HasMaxLength(1000)
                      .IsUnicode(true);

                entity.Property(r => r.NurseNote)
                      .HasMaxLength(1000)
                      .IsUnicode(true);

                entity.Property(r => r.Status)
                      .IsRequired()
                      .HasMaxLength(50)
                      .IsUnicode(true);

                entity.Property(r => r.DateCreated)
                      .IsRequired();

                entity.HasOne(r => r.Parent)
                      .WithMany()
                      .HasForeignKey(r => r.ParentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Student)
                      .WithMany()
                      .HasForeignKey(r => r.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(r => r.Medications)
                      .WithOne(m => m.Request)
                      .HasForeignKey(m => m.RequestId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region ParentMedicationDetail 
            modelBuilder.Entity<ParentMedicationDetail>(entity =>
            {
                entity.ToTable("ParentMedicationDetail");
                entity.HasKey(m => m.MedicationDetailId);

                entity.Property(m => m.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
                entity.Property(m => m.Type).HasMaxLength(100).IsUnicode(true);
                entity.Property(m => m.Usage).HasMaxLength(500).IsUnicode(true);
                entity.Property(m => m.Dosage).HasMaxLength(100).IsUnicode(true);
                entity.Property(m => m.Note).HasMaxLength(500).IsUnicode(true);
            });
            #endregion

            #region MedicalSupply 
            modelBuilder.Entity<MedicalSupply>(entity =>
            {
                entity.ToTable("MedicalSupply");
                entity.HasKey(s => s.MedicalSupplyId);
                entity.Property(s => s.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
                entity.Property(s => s.Type).HasMaxLength(100).IsUnicode(true);
                entity.Property(s => s.Description).HasMaxLength(500).IsUnicode(true);
                entity.Property(s => s.ExpiredDate).IsRequired(false);
            });
            #endregion

            #region Vaccine
            modelBuilder.Entity<Vaccine>(entity =>
            {
                entity.ToTable("Vaccine");
                entity.HasKey(v => v.VaccineId);
                entity.Property(v => v.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
                entity.Property(v => v.Description).IsRequired().HasMaxLength(1000).IsUnicode(true);
                entity.HasMany(v => v.Campaigns)
                      .WithOne(c => c.Vaccine)
                      .HasForeignKey(c => c.VaccineId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region VaccinationCampaign
            modelBuilder.Entity<VaccinationCampaign>(entity =>
            {
                entity.ToTable("VaccinationCampaign");
                entity.HasKey(c => c.CampaignId);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
                entity.Property(c => c.Description).HasMaxLength(1000).IsUnicode(true);
                entity.Property(c => c.Date).IsRequired();
                entity.HasOne(c => c.Vaccine)
                      .WithMany(v => v.Campaigns)
                      .HasForeignKey(c => c.VaccineId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(c => c.Consents)
                      .WithOne(consent => consent.Campaign)
                      .HasForeignKey(consent => consent.CampaignId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(c => c.Records)
                      .WithOne(r => r.Campaign)
                      .HasForeignKey(r => r.CampaignId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region VaccinationConsent
            modelBuilder.Entity<VaccinationConsent>(entity =>
            {
                entity.ToTable("VaccinationConsent");
                entity.HasKey(c => c.ConsentId);
                entity.Property(c => c.IsAgreed).IsRequired(false);
                entity.Property(c => c.Note).HasMaxLength(1000).IsUnicode(true);
                entity.Property(c => c.DateConfirmed).IsRequired(false);
                entity.HasOne(c => c.Campaign)
                      .WithMany(ca => ca.Consents)
                      .HasForeignKey(c => c.CampaignId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(c => c.Student)
                      .WithMany()
                      .HasForeignKey(c => c.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(c => c.Parent)
                      .WithMany()
                      .HasForeignKey(c => c.ParentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region VaccinationRecord
            modelBuilder.Entity<VaccinationRecord>(entity =>
            {
                entity.ToTable("VaccinationRecord");
                entity.HasKey(r => r.RecordId);
                entity.Property(r => r.Result).IsRequired().HasMaxLength(50).IsUnicode(true);
                entity.Property(r => r.ImmediateReaction).HasMaxLength(500).IsUnicode(true);
                entity.Property(r => r.Note).HasMaxLength(1000).IsUnicode(true);
                entity.Property(r => r.DateInjected).IsRequired();
                entity.HasOne(r => r.Campaign)
                      .WithMany(c => c.Records)
                      .HasForeignKey(r => r.CampaignId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(r => r.Student)
                      .WithMany()
                      .HasForeignKey(r => r.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(r => r.Nurse)
                      .WithMany()
                      .HasForeignKey(r => r.NurseId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region VaccinationFollowUp
            modelBuilder.Entity<VaccinationFollowUp>(entity =>
            {
                entity.ToTable("VaccinationFollowUp");
                entity.HasKey(f => f.FollowUpId);
                entity.Property(f => f.Reaction).HasMaxLength(500).IsUnicode(true);
                entity.Property(f => f.Note).HasMaxLength(1000).IsUnicode(true);
                entity.Property(f => f.Date).IsRequired();
                entity.HasOne(f => f.Record)
                      .WithMany()
                      .HasForeignKey(f => f.RecordId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region HealthCheck
            modelBuilder.Entity<HealthCheck>(entity =>
            {
                entity.ToTable("HealthCheck");

                entity.HasKey(h => h.HealthCheckID);

                entity.Property(h => h.Result)
                      .HasMaxLength(500)
                      .IsUnicode(true);

                entity.Property(h => h.Date).IsRequired();
                entity.Property(h => h.Weight);
                entity.Property(h => h.Height);
                entity.Property(h => h.BMI);
                entity.Property(h => h.NutritionStatus);
                entity.Property(h => h.HealthCheckDescription);

                entity.HasOne(h => h.Student)
                      .WithMany()
                      .HasForeignKey(h => h.StudentID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(h => h.Nurse)
                      .WithMany()
                      .HasForeignKey(h => h.NurseID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(h => h.Parent)
                      .WithMany()
                      .HasForeignKey(h => h.ParentID)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            OnModelCreatingPartial(modelBuilder);
        }


        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
