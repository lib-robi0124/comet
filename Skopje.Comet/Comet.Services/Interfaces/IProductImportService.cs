using Comet.DTO.DTOs;

namespace Comet.Services.Interfaces
{
    public interface IProductImportService
    {
        Task<ImportResultDto> ImportFromExcelAsync(Stream stream, bool overwrite);
    }
}
