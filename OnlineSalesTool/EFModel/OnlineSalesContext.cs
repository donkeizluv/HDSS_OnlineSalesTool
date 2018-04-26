using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OnlineSalesTool.EFModel
{
    public partial class OnlineSalesContext : DbContext
    {
        public virtual DbSet<Ability> Ability { get; set; }
        public virtual DbSet<AppUser> AppUser { get; set; }
        public virtual DbSet<Logs> Logs { get; set; }
        public virtual DbSet<OnlineOrder> OnlineOrder { get; set; }
        public virtual DbSet<Pos> Pos { get; set; }
        public virtual DbSet<PosShift> PosShift { get; set; }
        public virtual DbSet<ProcessStage> ProcessStage { get; set; }
        public virtual DbSet<Shift> Shift { get; set; }
        public virtual DbSet<ShiftDetail> ShiftDetail { get; set; }
        public virtual DbSet<ShiftSchedule> ShiftSchedule { get; set; }
        public virtual DbSet<UserAbility> UserAbility { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(@"Server=(localdb)\local;Database=OnlineSales;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ability>(entity =>
            {
                entity.HasIndex(e => e.Ability1)
                    .HasName("U_Ability")
                    .IsUnique();

                entity.Property(e => e.Ability1)
                    .IsRequired()
                    .HasColumnName("Ability")
                    .HasMaxLength(20);

                entity.Property(e => e.Description).HasMaxLength(50);
            });

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.HasIndex(e => e.Username)
                    .HasName("U_Username_AppUser")
                    .IsUnique();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(60);

                entity.Property(e => e.Hr)
                    .HasColumnName("HR")
                    .HasMaxLength(20);

                entity.Property(e => e.LastLogin).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.Phone2).HasMaxLength(20);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Manager)
                    .WithMany(p => p.InverseManager)
                    .HasForeignKey(d => d.ManagerId)
                    .HasConstraintName("FK_AppUser_AppUser");
            });

            modelBuilder.Entity<Logs>(entity =>
            {
                entity.HasKey(e => e.LogId);

                entity.Property(e => e.Class).HasMaxLength(50);

                entity.Property(e => e.Message).HasMaxLength(200);

                entity.Property(e => e.Method).HasMaxLength(50);

                entity.Property(e => e.Timestamp).HasColumnType("datetime");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<OnlineOrder>(entity =>
            {
                entity.HasKey(e => e.OrderId);

                entity.HasIndex(e => e.TrackingNumber)
                    .HasName("U_TrackingNumber_OnlineOrder")
                    .IsUnique();

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(70);

                entity.Property(e => e.Induscontract)
                    .HasColumnName("INDUSContract")
                    .HasMaxLength(20);

                entity.Property(e => e.Indusstatus)
                    .HasColumnName("INDUSStatus")
                    .HasMaxLength(20);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.NatId)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.OrderNumber).HasMaxLength(10);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.PosCode)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Product)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Received).HasColumnType("datetime");

                entity.Property(e => e.TrackingNumber)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.HasOne(d => d.AssignUser)
                    .WithMany(p => p.OnlineOrder)
                    .HasForeignKey(d => d.AssignUserId)
                    .HasConstraintName("FK_OnlineOrder_AppUser");

                entity.HasOne(d => d.Stage)
                    .WithMany(p => p.OnlineOrder)
                    .HasForeignKey(d => d.StageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OnlineOrder_ProcessStage");
            });

            modelBuilder.Entity<Pos>(entity =>
            {
                entity.HasIndex(e => e.PosCode)
                    .HasName("U_Pos")
                    .IsUnique();

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.PosCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.PosName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Pos)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Pos_AppUser");
            });

            modelBuilder.Entity<PosShift>(entity =>
            {
                entity.HasKey(e => new { e.PosId, e.ShiftId });

                entity.HasOne(d => d.Pos)
                    .WithMany(p => p.PosShift)
                    .HasForeignKey(d => d.PosId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PosShift_Pos");

                entity.HasOne(d => d.Shift)
                    .WithMany(p => p.PosShift)
                    .HasForeignKey(d => d.ShiftId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PosShift_Shift");
            });

            modelBuilder.Entity<ProcessStage>(entity =>
            {
                entity.HasKey(e => e.StageId);

                entity.HasIndex(e => e.Stage)
                    .HasName("U_StageName_ProcessStage")
                    .IsUnique();

                entity.Property(e => e.StageId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasMaxLength(150);

                entity.Property(e => e.Stage)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<Shift>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("U_ShiftName_Shift")
                    .IsUnique();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<ShiftDetail>(entity =>
            {
                entity.Property(e => e.EndAt).HasColumnType("time(0)");

                entity.Property(e => e.StartAt).HasColumnType("time(0)");

                entity.HasOne(d => d.Shift)
                    .WithMany(p => p.ShiftDetail)
                    .HasForeignKey(d => d.ShiftId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ShiftDetail_Shift");
            });

            modelBuilder.Entity<ShiftSchedule>(entity =>
            {
                entity.HasKey(e => new { e.ShiftDate, e.PosId, e.UserId, e.ShiftId });

                entity.Property(e => e.ShiftDate).HasColumnType("date");

                entity.HasOne(d => d.Pos)
                    .WithMany(p => p.ShiftSchedule)
                    .HasForeignKey(d => d.PosId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ShiftSchedule_Pos1");

                entity.HasOne(d => d.Shift)
                    .WithMany(p => p.ShiftSchedule)
                    .HasForeignKey(d => d.ShiftId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ShiftSchedule_Shift1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ShiftSchedule)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ShiftSchedule_AppUser");
            });

            modelBuilder.Entity<UserAbility>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.AbilityId });

                entity.HasOne(d => d.Ability)
                    .WithMany(p => p.UserAbility)
                    .HasForeignKey(d => d.AbilityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserAbility_Ability");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserAbility)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserAbility_AppUser");
            });
        }
    }
}
