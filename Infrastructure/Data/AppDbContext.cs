using BusinessCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public partial class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<UnitMeasure> UnitMeasures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>(entity =>
            {
                entity.ToTable("Item");

                entity.HasIndex(e => e.Id, "IX_ItemCode")
                    .IsUnique();

                entity.HasIndex(e => e.UnitMeasureId, "IX_Item_UnitMeasureId");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.DeletedBy).HasMaxLength(100);

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.ModifiedBy).HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.HasOne(d => d.UnitMeasure)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.UnitMeasureId);
            });

            modelBuilder.Entity<UnitMeasure>(entity =>
            {
                entity.ToTable("UnitMeasure");

                entity.HasIndex(e => e.Id, "IX_UnitMeasureCode")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.DeletedBy).HasMaxLength(100);

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.ModifiedBy).HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1000);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
