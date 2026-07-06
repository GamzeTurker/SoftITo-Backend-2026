using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EduCore.Models
{
    public class Lesson
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
        [Required(ErrorMessage = "Ders başlığı zorunludur.")]
        [StringLength(150)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Video linki zorunludur.")]
        [StringLength(512)]
        public string VideoUrl { get; set; }
        [Required]
        [Range(1, 1000, ErrorMessage = "Ders süresi en az 1 dakika olmalıdır.")]
        public int DurationMinutes { get; set; }
        [Required]
        public int SortOrder { get; set; } // Ders sıralaması
    }
}
