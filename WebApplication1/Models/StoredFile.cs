using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class StoredFile
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Dosya adı zorunludur.")]
        [StringLength(200)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string StoredFileName { get; set; } = string.Empty;

        [StringLength(20)]
        public string FileExtension { get; set; } = string.Empty;

        public long FileSize { get; set; }

        [Required]
        public string FilePath { get; set; } = string.Empty;

        public int DownloadCount { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }
    }
}