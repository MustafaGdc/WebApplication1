using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    public class FileEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Dosya adı zorunludur.")]
        [StringLength(200)]
        public string FileName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        public int CategoryId { get; set; }

        public bool IsActive { get; set; }
    }
}