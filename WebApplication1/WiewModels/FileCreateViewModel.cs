using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebApplication1.ViewModels
{
    public class FileCreateViewModel
    {
        [Required(ErrorMessage = "Dosya başlığı/adı zorunludur.")]
        [StringLength(200)]
        public string FileName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen bir dosya seçin.")]
        public IFormFile? UploadFile { get; set; }

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        public int CategoryId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}