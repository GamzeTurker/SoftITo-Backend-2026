using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EduCore.Models
{
    public class Enrollment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual User Student { get; set; }
        [Required]
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
        [Range(0, 100)]
        public int ProgressPercent { get; set; } = 0; // %0 - %100 arası kurs tamamlama oranı
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        // Bire Bir (One-to-One) ilişki için Sertifika bağlantısı
        public virtual Certificate Certificate { get; set; }
    }
}
