using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Comet.ViewModels.Models
{
    public class UploadExcelVM
    {
        [Required(ErrorMessage = "Excel file is required")]
        [Display(Name = "Excel File")]
        [FileExtensions(Extensions = ".xlsx,.xls", ErrorMessage = "Please upload an Excel file (.xlsx or .xls)")]
        public IFormFile ExcelFile { get; set; } = null!;

        [Display(Name = "Overwrite existing products?")]
        public bool OverwriteExisting { get; set; }

        public ImportResult? Result { get; set; }
    }
    // ImportResult.cs
    public class ImportResult
    {
        public bool Success { get; set; }
        public int TotalRows { get; set; }
        public int SuccessfullyImported { get; set; }
        public int FailedRows { get; set; }
        public List<ImportError> Errors { get; set; } = new();
        public TimeSpan ProcessingTime { get; set; }
    }

    public class ImportError
    {
        public int RowNumber { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
