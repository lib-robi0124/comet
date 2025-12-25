using Comet.DataAccess.Extensions;
using Comet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Comet.DataAccess.DataContext
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<LibertyUser> LibertyUsers { get; set; }
        public DbSet<BuyerUser> BuyerUsers { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.SeedData();
            // ===== Role Configuration =====
            modelBuilder.Entity<Role>()
                .Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);

            // ===== Product Configuration =====
            modelBuilder.Entity<Product>()
                .Property(p => p.ProductCode)
                .IsRequired()
                .HasMaxLength(50);
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.ProductCode)
                .IsUnique();
            modelBuilder.Entity<Product>()
                .Property(p => p.ProductCategory)
                .HasConversion<string>()
                .IsRequired();
            modelBuilder.Entity<Product>()
                .Property(p => p.ProductType)
                .HasConversion<string>()
                .IsRequired();
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Product>()
                .Property(p => p.ColorTopSide)
                .HasMaxLength(30);
            modelBuilder.Entity<Product>()
                .Property(p => p.ColorBottomSide)
                .HasMaxLength(30);
            modelBuilder.Entity<Product>()
                .Property(p => p.Grade)
                .HasMaxLength(20);
            modelBuilder.Entity<Product>()
                .Property(p => p.ZincCoating)
                .HasMaxLength(20);
            modelBuilder.Entity<Product>()
                .Property(p => p.Defects)
                .HasMaxLength(500);
            // ===== User Configuration =====
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<LibertyUser>("LibertyUser")
                .HasValue<BuyerUser>("BuyerUser");
            modelBuilder.Entity<User>()
                .ToTable("Users"); 
            modelBuilder.Entity<LibertyUser>()
                .ToTable("LibertyUsers"); 
            modelBuilder.Entity<BuyerUser>()
                .ToTable("BuyerUsers");
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);
            modelBuilder.Entity<User>()
                    .Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(200);
            modelBuilder.Entity<User>()
                    .Property(u => u.Password)
                    .IsRequired()
                    .HasMaxLength(500);
            modelBuilder.Entity<User>()
                    .HasIndex(u => u.Email)
                    .IsUnique();
            modelBuilder.Entity<User>()
                    .HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            // ===== LibertyUser Configuration =====
            modelBuilder.Entity<LibertyUser>()
                .Property(lu => lu.FullName)
                .IsRequired()
                .HasMaxLength(200);
            modelBuilder.Entity<LibertyUser>()
                .HasIndex(lu => lu.FullName)
                .IsUnique(false);
            modelBuilder.Entity<LibertyUser>()
                .HasMany(lu => lu.Products)
                .WithOne()
                .OnDelete(DeleteBehavior.SetNull);
            // ===== BuyerUser Configuration =====
            modelBuilder.Entity<BuyerUser>()
                .Property(bu => bu.CompanyName)
                .IsRequired()
                .HasMaxLength(200);
            modelBuilder.Entity<BuyerUser>()
                .HasIndex(bu => bu.CompanyName)
                .IsUnique(false);
            modelBuilder.Entity<BuyerUser>()
                .Property(bu => bu.ContactPerson)
                .IsRequired()
                .HasMaxLength(200);
            modelBuilder.Entity<BuyerUser>()
                .HasMany(bu => bu.Products)
                .WithOne()
                .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
