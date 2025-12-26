using Comet.DTO.DTOs;

namespace Comet.Services.Interfaces
{
    public interface IProductService
    {
        Task ImportFromExcelAsync(List<ProductExcelRowDto> rows);
        Task<List<ProductDto>> GetAllAsync();
    }
}
