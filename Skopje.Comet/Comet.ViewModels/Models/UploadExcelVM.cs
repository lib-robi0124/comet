using Comet.DTO.DTOs;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Comet.ViewModels.Models
{
    public class UploadExcelVM
    {
        [Required]
        [Display(Name = "Excel file")]
        public IFormFile ExcelFile { get; set; }

        public bool OverwriteExisting { get; set; }

        public ImportResultDto Result { get; set; }
    }
}
