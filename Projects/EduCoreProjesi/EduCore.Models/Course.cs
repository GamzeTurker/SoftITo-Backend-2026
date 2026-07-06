using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EduCore.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        [Required]
        //public int InstructorId { get; set; }
        //[ForeignKey("InstructorId")]
        //public virtual User Instructor { get; set; }
        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        [Required(ErrorMessage = "Kurs başlığı zorunludur.")]
        [StringLength(150)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Kurs açıklaması zorunludur.")]
        public string Description { get; set; }
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0, 99999.99, ErrorMessage = "Geçersiz fiyat aralığı.")]
        public decimal Price { get; set; } = 0.00m;
        [StringLength(512)]
        public string CoverImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public ICollection<Lesson>? Lessons { get; set; }

        public ICollection<Review>? Reviews { get; set; }

        public ICollection<Enrollment>? Enrollments { get; set; }

        public ICollection<Certificate>? Certificates { get; set; }
    }
}

