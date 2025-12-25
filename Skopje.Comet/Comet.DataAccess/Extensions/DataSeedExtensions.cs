using Comet.Domain.Entities;
using Comet.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Comet.DataAccess.Extensions
{
    public static class DataSeedExtensions
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
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

        }
    }
}
