using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Comet.ViewModels.Models
{
    // Custom validation attribute for Excel files
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_extensions.Contains(extension))
                {
                    return new ValidationResult($"Please upload an Excel file (.xlsx or .xls). Got: {extension}");
                }
            }
            return ValidationResult.Success;
        }
    }

    public class UploadExcelVM
    {
        [Required(ErrorMessage = "Excel file is required")]
        [Display(Name = "Excel File")]
        [AllowedExtensions(new string[] { ".xlsx", ".xls" })]
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
