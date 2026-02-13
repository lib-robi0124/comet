using Comet.ViewModels.Models;
using System.ComponentModel.DataAnnotations;

namespace Comet.ViewModels.Admin
{
    public class AdminProductImportVM : UploadExcelVM
    {
        [Display(Name = "Send email notification after import")]
        public bool SendEmailNotification { get; set; }

        [Display(Name = "Validate only (don't save)")]
        public bool ValidateOnly { get; set; }

        public DateTime? LastImportDate { get; set; }
        public List<ImportHistoryVM> ImportHistory { get; set; } = new();
    }
}
