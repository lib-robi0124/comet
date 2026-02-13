using Comet.ViewModels.Models;

namespace Comet.ViewModels.Admin
{
    public class ImportResultsVM
    {
        public ImportResult? Result { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ImportTime { get; set; } = string.Empty;
        public bool HasErrors => Result?.Errors?.Any() == true;
        public int SuccessPercentage => Result?.TotalRows > 0
            ? (int)((Result.SuccessfullyImported / (double)Result.TotalRows) * 100) : 0;
    }
}
