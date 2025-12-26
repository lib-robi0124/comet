using Comet.Domain.Entities;
using Comet.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Comet.DataAccess.Extensions
{
    public static class DataSeedExtensions
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            // Create a password hasher instance
            var passwordHasher = new PasswordHasher<User>();

            // Hash passwords
            var adminPasswordHash = passwordHasher.HashPassword(null, "admin123");
            var reportPasswordHash = passwordHasher.HashPassword(null, "report123");
            var customerPasswordHash = passwordHasher.HashPassword(null, "customer123");

            modelBuilder.Entity<Role>().HasData(
              new Role { Id = 1, Name = "Admin" },
              new Role { Id = 2, Name = "Report" },
              new Role { Id = 3, Name = "Customer" }
           );
            modelBuilder.Entity<Product>().HasData(
                   new Product
                   {
                       Id = 1,
                       ProductCode = "2501998/11",
                       ProductCategory = ProductCategory.FourthChoice,
                       ProductType = ProductType.PPG_Coil,
                       ColorTopSide = "RAL 9002 LTP",
                       ColorBottomSide = null,
                       Grade = "DX51D +Z",
                       ZincCoating = "Z 140",
                       Thickness = 0.75m,
                       Width = 1500,
                       GrossWeight = 1465m,
                       NetWeight = 1465m,
                       Defects = "unpainted spots",
                       Price = null
                   },
                   new Product
                     {
                         Id = 2,
                         ProductCode = "2503658/41",
                         ProductCategory = ProductCategory.FourthChoice,
                         ProductType = ProductType.PPG_Coil,
                         ColorTopSide = "RAL 9006",
                         ColorBottomSide = "LZS 7030",
                         Grade = "S250GD +Z",
                         ZincCoating = "Z 140",
                         Thickness = 0.50m,
                         Width = 1250,
                         GrossWeight = 0.890m,
                         NetWeight = 0.880m,
                         Defects = "unpainted spots",
                         Price = null
                     }
            );
            // ===== Seed LibertyUsers (Admin and Report) =====
            modelBuilder.Entity<LibertyUser>().HasData(
                new LibertyUser
                {
                    Id = 1,
                    Email = "admin@liberty.com",
                    Password = adminPasswordHash, 
                    RoleId = 1, 
                    FullName = "Liberty Admin"
                },
                new LibertyUser
                {
                    Id = 2,
                    Email = "reports@liberty.com",
                    Password = reportPasswordHash, 
                    RoleId = 2, 
                    FullName = "Report User"
                }
            );
            // ===== Seed BuyerUsers (Customers) =====
            modelBuilder.Entity<BuyerUser>().HasData(
                new BuyerUser
                {
                    Id = 3,
                    Email = "customer1@buyer.com",
                    Password = customerPasswordHash, 
                    RoleId = 3, 
                    CompanyName = "SteelWorks Inc",
                    ContactPerson = "John Smith"
                },
                new BuyerUser
                {
                    Id = 4,
                    Email = "customer2@buyer.com",
                    Password = "hashed_password_here", // You should hash this password
                    RoleId = 3, // Customer role
                    CompanyName = "MetalPro Industries",
                    ContactPerson = "Jane Doe"
                }
            );
        }
    }
}
