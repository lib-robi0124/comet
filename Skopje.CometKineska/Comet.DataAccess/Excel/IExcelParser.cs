using Comet.ViewModels.Models;

namespace Comet.DataAccess.Excel
{
    public interface IExcelParser
    {
        List<T> Parse<T>(Stream excelStream) where T : class, new();
        Task<ImportResult> ParseProductsAsync(Stream excelStream);
    }
}
